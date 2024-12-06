
using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation.ShipSystems;

public abstract class EnergyGenerator : PoweredShipSystem
{
    public abstract Material Fuel { get; }
    public override double BaseConsumption => _fuelConsumption;
    public override double BaseHeatOutput => 1.0;
    public override double LevelPerSecond => 0.2; // 5 seconds per level

    private double _fuelConsumption;
    private double _energyStored;
    private double _energyThreshold = 0.01;

    public double FuelStored { get; private set; }
    
    // TODO: Set this to ten seconds of fuel
    public virtual double FuelCapacity { get; } = 1;

    public EnergyGenerator() {}
    public EnergyGenerator(string name) : this()
    {
        Name = name;
    }
    
    public override void Simulate(SimulationContext context)
    {
        if (Parent is null) return;
        
        // TODO: Top off the tank, this should average once per second
        if (FuelStored < FuelCapacity * 0.5)
            FuelStored += Parent.WithdrawResource(Fuel, FuelCapacity - FuelStored);

        // Calculate the consumption of fuel based on generator output and material energy density
        _fuelConsumption = BaseOutput / Fuel.MegaJoulesPerLiter;
        
        // Perform the power level calculations for efficiency
        base.Simulate(context);
        
        var fuelConsumed = CurrentConsumption * context.DeltaTime;
        
        // TODO: Suddenly stop producing power when out of fuel
        // Instead, lower the power level
        if (FuelStored < fuelConsumed) return;
        FuelStored -= fuelConsumed;
        
        // Supply power to ship
        _energyStored += CurrentOutput;
        
        // Store energy up to _energyThreshold before submitting it to the system
        if (_energyStored < _energyThreshold) return;
        
        var supplied = Parent.DepositResource(Materials.Electricity, _energyStored);
        
        // Convert excess power into heat
        var overage = _energyStored - supplied;
        var heatProduced = CurrentHeatOutput + overage;
        AddHeat(heatProduced);
        
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