using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using BlackOcean.Common;
using BlackOcean.Simulation;
using BlackOcean.Simulation.ControlPanels;

namespace BlackOcean.App;

public sealed class WebSocketConnection : IAsyncDisposable
{
    public WebSocket Socket { get; }
    public Game Game { get; }
    public Player? Player { get; set; }
    public bool IsClosed { get; private set; }

    private MemoryStream _readBuffer = new(1024);
    private MemoryStream _writeBuffer = new(1024);
    private JsonReaderOptions _jsonReaderOptions = new() { AllowTrailingCommas = true, MaxDepth = 8 };

    private ControlPanel? _controlPanelClone;
    private readonly ILogger<WebSocketConnection> _logger;
    private CancellationTokenSource? _cancellationSource;
    private Task _task = Task.CompletedTask;

    private Dictionary<string, object?> _controlPanelUpdates = new();
    
    public WebSocketConnection(WebSocket socket, Game game, ILogger<WebSocketConnection> logger)
    {
        _logger = logger;
        Socket = socket;
        Game = game;
        _logger.LogInformation("New connection");
    }
    
    // New connection has been established:
    // * Join to a player
    // * Enter a receiving loop until cancelled
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Player = await Game.PlayerManager.GetPlayer("Player1");

        // `cancellationToken` is connection lost, `_cancellationSource` is we drop the connection
        _cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await StartReceivingAsync(_cancellationSource.Token);
    }

    public async Task StartReceivingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start receiving");

        try
        {
            ArraySegment<byte> buffer = new byte[1024];
            while (!Socket.CloseStatus.HasValue && !cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await Socket.ReceiveAsync(buffer, cancellationToken);
                    _readBuffer.Write(buffer.Array!, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Text)
                    HandleTextMessage();
                else if (result.MessageType == WebSocketMessageType.Binary)
                    HandleBinaryMessage();

                _readBuffer.SetLength(0);
                await Task.Yield();
            }

            _logger.LogInformation("Stopped receiving");
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Connection dropped");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Connection dropped due to exception");
        }
        finally
        {
            IsClosed = true;
        }
    }

    private void HandleBinaryMessage()
    {
        _logger.LogWarning("Received binary message ({MessageSize} bytes)", _readBuffer.Length);
    }

    private void HandleTextMessage()
    {
        if (!_readBuffer.TryGetBuffer(out var buffer)) 
            throw new InvalidOperationException("Buffer is exposable");
        
        ReadOnlySpan<byte> span = buffer.AsSpan();
        var reader = new Utf8JsonReader(span, _jsonReaderOptions);
        var message = JsonElement.ParseValue(ref reader);
        if (message.TryGetProperty("ControlPanel", out var properties))
        {
            lock (_controlPanelUpdates)
            {
                foreach (var property in properties.EnumerateObject())
                {
                    object? value = property.Value.ValueKind switch
                    {
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Number => property.Value.GetDouble(),
                        _ => property.Value.GetString()
                    };
                    _controlPanelUpdates[property.Name] = value;
                }
            }
        }
        else
        {
            _logger.LogWarning("Received unknown message: {Message}", message.ToString());
        }
    }
    
    public void Process()
    {
        if (!_task.IsCompleted) return;
        _task.Dispose();
        _task = ProcessInBackground();
    }
    
    private async Task ProcessInBackground()
    {
        if (_cancellationSource?.IsCancellationRequested == true) return;
        
        ReceiveMessages();

        if (Socket.State is not WebSocketState.Open)
        {
            _cancellationSource?.Cancel();
            return;
        }

        BuildMessage();

        await SendMessage();
    }
    
    private void ReceiveMessages()
    {
        lock (_controlPanelUpdates)
        {
            if (_controlPanelUpdates.Count == 0) return;

            try
            {
                _logger.LogInformation("Handling {MessageCount} messages", _controlPanelUpdates.Count);

                if (Player is not null)
                    ModelUtil.Apply(Player.ControlPanel, _controlPanelUpdates);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while handling messages");
            }
            finally
            {
                _controlPanelUpdates.Clear();
            }
        }
    }
    
    private record ReplaceMessage(ReplaceBody ControlPanel);

    private record ReplaceBody(ControlPanel Replace);

    private record UpdateMessage(UpdateBody ControlPanel);

    private record UpdateBody(Dictionary<string, object> Updates);

    private void BuildMessage()
    {
        try
        {
            _writeBuffer.SetLength(0);

            if (Player is null) return;
            
            if (_controlPanelClone is null)
            {
                _logger.LogInformation("Sending new state");

                _controlPanelClone = ModelUtil.DeepClone(Player.ControlPanel);
                ModelUtil.Serialize(_writeBuffer, new ReplaceMessage(new ReplaceBody(_controlPanelClone)));
                return;
            }

            var diff = ModelUtil.DiffApply(Player.ControlPanel, _controlPanelClone);
            if (diff.Count == 0) return;

            ModelUtil.Serialize(_writeBuffer, new UpdateMessage(new UpdateBody(diff)));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while sending state");
        }
    }

    private async Task SendMessage()
    {
        if (_writeBuffer.Length == 0) return;
        try
        {
            await Socket.SendAsync(_writeBuffer.ToArray(), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while sending state");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_cancellationSource != null)
            await _cancellationSource.CancelAsync();

        await _task;
        Socket.Dispose();
        _cancellationSource?.Dispose();
    }
}