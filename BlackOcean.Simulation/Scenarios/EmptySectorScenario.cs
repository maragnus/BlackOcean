namespace BlackOcean.Simulation;

public class EmptySectorScenario : Scenario
{
    public Sector Sector { get; private set; } = default!;
    public event Action<SimulationContext>? OnSimulate;

    public override void Initialize()
    {
        Sector = Game.SectorManager.Add("Test", JVector.Zero);
    }
    
    public override void Simulate(SimulationContext context)
    {
        base.Simulate(context);
        OnSimulate?.Invoke(context);
    }

    public override Player CreatePlayer(string name) => CreatePlayer(Sector, name, "Basic Ship");
}