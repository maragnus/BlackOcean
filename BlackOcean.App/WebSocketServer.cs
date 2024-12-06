using System.Net.WebSockets;
using System.Text.Json;
using BlackOcean.Simulation;

namespace BlackOcean.App;

public class WebSocketConnection
{
    public WebSocket Socket { get; }
    public Player Player { get; }

    private MemoryStream _readBuffer = new(1024);
    private JsonReaderOptions _jsonReaderOptions = new() { AllowTrailingCommas = true, MaxDepth = 8 };

    public WebSocketConnection(WebSocket socket, Game game)
    {
        Socket = socket;
        Player = game.PlayerManager.GetPlayer("Player1");
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            StartSendingAsync(cancellationToken),
            StartReceivingAsync(cancellationToken));
    }

    public async Task StartSendingAsync(CancellationToken cancellationToken)
    {
        while (!Socket.CloseStatus.HasValue && !cancellationToken.IsCancellationRequested)
        {
            await SendUpdatesAsync();
        }
    }

    public async Task StartReceivingAsync(CancellationToken cancellationToken)
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
                HandleTextMessage(result);
            else if (result.MessageType == WebSocketMessageType.Binary)
                HandleBinaryMessage(result);

            _readBuffer.Seek(0, SeekOrigin.Begin);
        }
    }

    private void HandleBinaryMessage(WebSocketReceiveResult result)
    {
        throw new NotImplementedException();
    }

    private void HandleTextMessage(WebSocketReceiveResult result)
    {
        if (!_readBuffer.TryGetBuffer(out var buffer)) return;
        ReadOnlySpan<byte> span = buffer.AsSpan();
        var reader = new Utf8JsonReader(span, _jsonReaderOptions);
        HandleJsonMessage(JsonElement.ParseValue(ref reader));
    }

    public Task SendUpdatesAsync()
    {
        return Task.CompletedTask;
    }
    
    private void HandleJsonMessage(JsonElement parseValue)
    {
        
    }
}

public class WebSocketServer(Game game)
{
    public const int SyncRateMs = 500;

    private List<WebSocketConnection> _connections = [];
    private SemaphoreSlim _semaphore = new(1);

    public async Task AddSocket(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var connection = new WebSocketConnection(webSocket, game);
        
        await _semaphore.WaitAsync();
        try
        {
            BuildSnapshot(connection);
            _connections.Add(connection);
        }
        finally
        {
            _semaphore.Release();
        }

        await connection.StartAsync(cancellationToken);
    }

    private void BuildSnapshot(WebSocketConnection connection)
    {
    }
}