
using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation.ShipSystems;

public abstract class EnergyGenerator : PoweredShipComponent
{
    public abstract Material Fuel { get; }
    public override double BaseConsumption { get; }
    public override double BaseHeatOutput => 1.0;
    public override double LevelPerSecond => 0.2; // 5 seconds per level
    public MaterialStore FuelInput { get; set; }
    public MaterialStore EnergyOutput { get; set; }
    public override bool Automatable => true;

    private double _energyStored;
    private const double EnergyThreshold = 1; // 1 watt

    public EnergyGenerator() {}
    public EnergyGenerator(string name) : this()
    {
        Name = name;

        // Calculate the consumption of fuel per second based on generator output and material energy density
        BaseConsumption = BaseOutput / Fuel.MegaJoulesPerLiter;

        FuelInput = Consumes(Fuel, BaseConsumption);
        EnergyOutput = Supplies(Materials.Electricity, BaseOutput);
    }

    public override void Simulate(SimulationContext context)
    {
        if (Body is null) return;

        // Convert 100% of the generator overage into heat
        if (EnergyOutput.Amount > Materials.Epsilon)
        {
            var heat = EnergyOutput.Consume(EnergyOutput.Amount);
            HeatOutput.Supply(heat);
        }
        
        // Standard: 1 output, 0.96 efficiency 
        StandardHeatOutput = BaseOutput * (1 - BaseEfficiency * 0.96);
        // Overdrive: 1.5 output, 0.9 efficiency 
        OverdriveHeatOutput = (BaseOutput * 1.5) * (1 - BaseEfficiency * 0.9);
        
        // Perform the power level calculations for efficiency
        base.Simulate(context);
        
        var consumed = FuelInput.Consume(CurrentConsumption * context.DeltaTime);
        if (consumed < Materials.Epsilon) return;

        var energy = consumed * Fuel.MegaJoulesPerLiter;
        _energyStored += energy;
        
        // Estimate actual outputs
        // TODO: Are these obsolete with the EnergyOutput.Trend and HeatOutput.Trend?
        var output = energy / context.DeltaTime;
        CurrentOutput = output * CurrentEfficiency;
        CurrentHeatOutput = CurrentOutput - output;
        
        // Store energy up to _energyThreshold before submitting it to the system
        if (_energyStored < EnergyThreshold) return;
        EnergyOutput.Supply(_energyStored);
        _energyStored = 0;
    }
}

public class PrefabEnergyGenerator(string name, Material fuelMaterial, double efficiency, double output, double thermalLimit)
    : EnergyGenerator(name)
{
    public override double BaseEfficiency { get; } = efficiency;
    public override double BaseOutput { get; } = output;
    public override double ThermalLimit { get; } = thermalLimit;
    public override Material Fuel { get; } = fuelMaterial;
}