using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation.ShipSystems;

public abstract class CoolingSystem : ShipSystem
{
    public bool IsOpen { get; set; }
    public abstract double Output { get; }

    public CoolingSystem() { }
    public CoolingSystem(string name) : base(name) { }
    
    public override void Simulate(SimulationContext context)
    {
        if (!IsOpen) return;
        
        // Extract up to output from the heat storage (aka coolant)
        var radiation = Parent?.WithdrawResource(Materials.Heat, Output * context.DeltaTime) ?? 0;
        if (radiation > Materials.Epsilon)
        {
            // TODO: Emit radiation
        }
    }
}

public class PrefabCoolingSystem(string name, double output) : CoolingSystem(name)
{
    public override double Output { get; } = output;
}