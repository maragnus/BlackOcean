using System.Collections.Concurrent;
using BlackOcean.Simulation;

namespace BlackOcean.App;

public sealed class GameService(Game game, ILogger<GameService> logger)
{
    private Game Game { get; } = game;
    private readonly HashSet<WebSocketConnection> _connections = [];
    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public void Initialize() => Game.Initialize();

    public void Shutdown() => Game.Shutdown();

    public void Update(double now, double deltaTime)
    {
        Game.Simulate(now, deltaTime);

        _semaphore.Wait();
        try
        {
            foreach (var connection in _connections)
                connection.Process();

            _connections.RemoveWhere(connection => connection.IsClosed);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void AddConnection(WebSocketConnection connection)
    {
        logger.LogInformation("Adding connection {Connection}", connection);
        _semaphore.Wait();
        try
        {
            _connections.Add(connection);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void RemoveConnection(WebSocketConnection connection)
    {
        logger.LogInformation("Removing connection {Connection}", connection);
        _semaphore.Wait();
        try
        {
            _connections.Remove(connection);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}