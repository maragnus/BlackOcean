using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation.ShipSystems;

public abstract class CoolingComponent : ShipComponent
{
    public bool IsOpen { get; set; } = true;
    public abstract double BaseConsumption { get; }
    public double CurrentConsumption { get; private set; }
    public MaterialStore HeatInput { get; }

    public CoolingComponent() : this("Unnamed Radiator") { }
    public CoolingComponent(string name) : base(name)
    {
        HeatInput = Consumes(Materials.Heat, BaseConsumption);
    }
    
    public override void Simulate(SimulationContext context)
    {
        if (!IsOpen) return;
        
        // Extract up to output from the heat storage (aka coolant)
        var radiation = HeatInput.Consume(BaseConsumption * context.DeltaTime);
        CurrentConsumption = radiation / context.DeltaTime;
        if (radiation > Materials.Epsilon)
        {
            // TODO: Emit radiation
        }
    }
}

public class PrefabCoolingComponent(string name, double baseConsumption) : CoolingComponent(name)
{
    public override double BaseConsumption { get; } = baseConsumption;
}