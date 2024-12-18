namespace BlackOcean.Simulation;

public sealed class Game
{
    public SectorManager SectorManager { get; }
    public PlayerManager PlayerManager { get; }
    public FactionManager FactionManager { get; }
    public Scenario Scenario { get; private set; } = default!;
    public double StartTime { get; set; } = 0;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    private Game()
    {
        SectorManager = new SectorManager();
        PlayerManager = new PlayerManager(this);
        FactionManager = new FactionManager(this);
    }

    public async Task<TResult> Execute<TResult>(Func<TResult> action)
    {
        await _semaphore.WaitAsync();
        try
        {
            return action();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public static Game StartScenario<TScenario>() where TScenario : Scenario, new()
    {
        var game = new Game();
        var scenario = new TScenario();
        
        // private set on Scenario.Game
        typeof(Scenario)!.GetProperty(nameof(Game))!.GetSetMethod(true)!.Invoke(scenario, [game]);
        
        game.Scenario = scenario;
        scenario.Initialize();
        return game;
    }
    
    public void Simulate(double time, double deltaTime)
    {
        _semaphore.Wait();
        try
        {
            var context = new SimulationContext(StartTime + time, deltaTime);
            Scenario.Simulate(context);
            FactionManager.Simulate(context);
            SectorManager.Simulate(context);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Initialize()
    {
        
    }
    
    public void Shutdown()
    {
        
    }
}