namespace BlackOcean.Simulation.ShipSystems;

public abstract class ShipHull(string name) : ShipComponent(name)
{
    public static readonly ShipHull EmptyHull = new PrefabShipHull("No Hull", 0);
    
    public abstract double Mass { get; }
    
    public override void Simulate(SimulationContext context)
    {
        
    }
}

public class PrefabShipHull(string name, double mass) : ShipHull(name)
{
    public override double Mass { get; } = mass;
}