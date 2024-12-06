namespace BlackOcean.Simulation;

public class Faction(string name) : ISimulated
{
    public string Name { get; } = name;
    public HashSet<SpaceBody> OwnedBodies { get; } = [];

    public void Simulate(SimulationContext context)
    {
        
    }
}
