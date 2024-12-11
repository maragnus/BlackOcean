using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using BlackOcean.Simulation;

namespace BlackOcean.App;


public sealed class GameService(Game game, ILogger<GameService> logger)
{
    private Game Game { get; } = game;
    private readonly ConcurrentDictionary<WebSocketConnection, byte> _connections = [];

    public void Initialize() => Game.Initialize();

    public void Shutdown() => Game.Shutdown();

    public async Task Update(double now, double deltaTime)
    {
        logger.LogInformation("Game tick {Time}s {Delta}", now, deltaTime);

        Game.Simulate(now / 1_000.0, deltaTime);
        
        await UpdateConnections();
    }

    private async Task UpdateConnections()
    {
        logger.LogInformation("Processing {ConnectionCount} connections.", _connections.Count);

        var tasks = _connections.Keys
            .Select(c => c.ProcessAsync());
        await Task.WhenAll(tasks);
    }

    public void AddConnection(WebSocketConnection connection)
    {
        logger.LogInformation("Adding connection {Connection}", connection);
        _connections.TryAdd(connection, 0);
    }
    
    public void RemoveConnection(WebSocketConnection connection)
    {
        logger.LogInformation("Removing connection {Connection}", connection);
        _connections.Remove(connection, out _);
    }

}

public sealed class GameHostedService(GameService gameService, ILogger<GameHostedService> logger) : IHostedService, IDisposable
{
    private Timer? _timer;
    private Stopwatch? _clock;
    private long _lastTime;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Initialize the game
        gameService.Initialize();
        
        // Frame zero to awaken everything
        _clock = new Stopwatch();
        UpdateGame(null);

        // Start the update loop at 10 times per second (100ms intervals)
        _timer = new Timer(UpdateGame, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000));
        _clock.Start();
        
        return Task.CompletedTask;
    }

    private async void UpdateGame(object? state)
    {
        var now = _clock!.ElapsedMilliseconds;
        var deltaTime = (now - _lastTime) / 1_000.0;
        _lastTime = now;

        await gameService.Update(now / 1_000.0, deltaTime);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _clock?.Stop();
        _clock = null;

        _timer?.Change(Timeout.Infinite, 0);
        _timer?.Dispose();
        _timer = null;

        gameService.Shutdown();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _clock?.Stop();
        _timer?.Dispose();
    }
}