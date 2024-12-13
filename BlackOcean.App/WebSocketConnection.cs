using System.Net.WebSockets;
using System.Text.Json;
using BlackOcean.Common;
using BlackOcean.Simulation;
using BlackOcean.Simulation.ControlPanels;

namespace BlackOcean.App;

public class WebSocketConnection
{
    public WebSocket Socket { get; }
    public Player Player { get; }

    private MemoryStream _readBuffer = new(1024);
    private MemoryStream _writeBuffer = new(1024);
    private JsonReaderOptions _jsonReaderOptions = new() { AllowTrailingCommas = true, MaxDepth = 8 };

    private ControlPanel? _controlPanelClone;
    private readonly ILogger<WebSocketConnection> _logger;

    public WebSocketConnection(WebSocket socket, Game game, ILogger<WebSocketConnection> logger)
    {
        _logger = logger;
        Socket = socket;
        Player = game.PlayerManager.GetPlayer("Player1");
        _logger.LogInformation("New connection");
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartReceivingAsync(cancellationToken);
    }

    public async Task StartReceivingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start receiving");

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
                HandleTextMessage(result);
            else if (result.MessageType == WebSocketMessageType.Binary)
                HandleBinaryMessage(result);

            _readBuffer.SetLength(0);
            await Task.Yield();
        }
        
        _logger.LogInformation("Stopped receiving");
    }

    private void HandleBinaryMessage(WebSocketReceiveResult result)
    {
        Console.WriteLine("Received binary message");
    }

    private void HandleTextMessage(WebSocketReceiveResult result)
    {
        if (!_writeBuffer.TryGetBuffer(out var buffer)) return;
        ReadOnlySpan<byte> span = buffer.AsSpan();
        var reader = new Utf8JsonReader(span, _jsonReaderOptions);
        StoreJsonMessage(JsonElement.ParseValue(ref reader));
    }
    
    private void StoreJsonMessage(JsonElement parseValue)
    {
        Console.WriteLine(parseValue.ToString());
        // TODO: parse, verify, and queue messages
    }
    
    public async Task<bool> ProcessAsync()
    {
        await ReceiveMessages();
        
        if (Socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
        {
            await SendMessages();
            return true;
        }

        // Returning false removes connection from the list
        return false;
    }

    private record ReplaceMessage(ReplaceBody ControlPanel);
    private record ReplaceBody(ControlPanel Replace);
    private record UpdateMessage(UpdateBody ControlPanel);
    private record UpdateBody(Dictionary<string, object> Updates);
    
    private async Task SendMessages()
    {
        try
        {
            _writeBuffer.SetLength(0);

            if (_controlPanelClone is null)
            {
                _logger.LogInformation("Sending new state");

                _controlPanelClone = ModelUtil.DeepClone(Player.ControlPanel);
                ModelUtil.Serialize(_writeBuffer, new ReplaceMessage(new ReplaceBody(_controlPanelClone)));
                await Socket.SendAsync(_writeBuffer.ToArray(), WebSocketMessageType.Text, true, CancellationToken.None);
                return;
            }

            var diff = ModelUtil.DiffApply(Player.ControlPanel, _controlPanelClone);
            if (diff.Count == 0) return;

            ModelUtil.Serialize(_writeBuffer, new UpdateMessage(new UpdateBody(diff)));
            await Socket.SendAsync(_writeBuffer.ToArray(), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while sending state");
        }
    }

    private async Task ReceiveMessages()
    {
        // Process queued messages from HandleJsonMessage()
    }
}