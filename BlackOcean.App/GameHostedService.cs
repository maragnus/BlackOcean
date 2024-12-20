﻿using System.Diagnostics;

namespace BlackOcean.App;

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
        _timer = new Timer(UpdateGame, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
        _clock.Start();
        
        return Task.CompletedTask;
    }

    private void UpdateGame(object? state)
    {
        var now = _clock!.ElapsedMilliseconds;
        var deltaTime = (now - _lastTime) / 1_000.0;
        _lastTime = now;

        gameService.Update(now / 1_000.0, deltaTime);
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