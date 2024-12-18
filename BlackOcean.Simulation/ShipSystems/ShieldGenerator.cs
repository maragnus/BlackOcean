using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation.ShipSystems;

public enum ShieldType
{
    Ablative,
    Energy
}

public abstract class ShieldGenerator : PoweredShipComponent
{
    public abstract ShieldType ShieldType { get; }
    public override double BaseConsumption => 1.0;
    public override double BaseHeatOutput => 1.0;
    public override double LevelPerSecond => 0.2; // 5 seconds per level
    public override bool Automatable => true;

    public MaterialStore EnergyInput {get;}
    public MaterialStore ShieldOutput { get; set; }

    private double _output;
    
    public ShieldGenerator() : this("Unnamed Shield Generator") {}

    public ShieldGenerator(string name) : base(name)
    {
        EnergyInput = Consumes(Materials.Electricity, BaseConsumption);
        var shieldType = ShieldType == ShieldType.Ablative ? Materials.AblativeShields : Materials.ShieldEnergy;
        ShieldOutput = Supplies(shieldType, BaseOutput);
    }
    
    public override void Simulate(SimulationContext context)
    {
        // Overdrive: 1.5 output, 0.9 efficiency 
        OverdriveHeatOutput = (BaseOutput * 1.5) * (1 - BaseEfficiency * 0.9);
        StandardHeatOutput = (BaseOutput * 1) * (1 - BaseEfficiency * 0.96);
        
        base.Simulate(context);
        
        var energyConsumed = EnergyInput.Consume(CurrentConsumption * context.DeltaTime);
        _output += energyConsumed * CurrentEfficiency;
        
        // Don't process negligible amounts
        if (_output < Materials.Epsilon) return;
        
        var shieldStored = ShieldOutput.Supply(_output);
        var overage = _output - shieldStored;
        _output = 0;
        
        var heat = energyConsumed * (1 - CurrentEfficiency); // Efficiency loss
        heat += overage * 0.25; // 25% of overage is converted into heat
        HeatOutput.Supply(heat);
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