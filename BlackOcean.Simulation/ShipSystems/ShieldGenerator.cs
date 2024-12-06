using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation.ShipSystems;

public enum ShieldType
{
    Ablative,
    Energy
}

public abstract class ShieldGenerator : PoweredShipSystem
{
    public abstract ShieldType ShieldType { get; }
    public override double BaseConsumption => 1.0;
    public override double BaseHeatOutput => 1.0;
    public override double LevelPerSecond => 0.2; // 5 seconds per level

    public ShieldGenerator() {}
    public ShieldGenerator(string name) : base(name) { }
    
    public override void Simulate(SimulationContext context)
    {
        base.Simulate(context);
        var output = CurrentOutput;

        var material = ShieldType switch
        {
            ShieldType.Ablative => Materials.AblativeShields,
            ShieldType.Energy => Materials.ShieldEnergy,
            _ => Materials.ShieldEnergy
        };
        
        // Send out the shield charge
        var energy = Parent?.DepositResource(material, output) ?? output;
        
        // Any overages generate heat
        AddHeat(output - energy);
    }
}

public class PrefabShieldGenerator(string name, double efficiency, double output, double thermalLimit, ShieldType shieldType)
    : ShieldGenerator(name)
{
    public override double BaseEfficiency { get; } = efficiency;
    public override double BaseOutput { get; } = output;
    public override double ThermalLimit { get; } = thermalLimit;
    public override ShieldType ShieldType { get; } = shieldType;
}