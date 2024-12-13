using BlackOcean.Simulation.Definitions;
using BlackOcean.Simulation.ShipSystems;

namespace BlackOcean.Simulation;

public abstract class SystemizedBody(string name) : SpaceBody(name)
{
    private readonly HashSet<ShipSystem> _systems = [];
    public IReadOnlyCollection<ShipSystem> Systems => _systems;

    public void AddSystem(ShipSystem system)
    {
        system.Parent = this;
        _systems.Add(system);
    }
    
    public void RemoveSystem(ShipSystem system)
    {
        _systems.Add(system);
        system.Parent = null;
    }
    
    private readonly HashSet<ShipSystem> _hotSystems = new();
    public override void Simulate(SimulationContext context)
    {
        base.Simulate(context);
        
        _hotSystems.Clear();

        foreach (var system in Systems)
        {
            system.Simulate(context);
            
            // Track systems generating heat
            if (system.Heat > Materials.Epsilon)
                _hotSystems.Add(system);
        }

        if (_hotSystems.Count == 0) return;
        
        // Calculate total excess heat in systems
        var totalHeat = _hotSystems.Sum(system => system.Heat);
        if (totalHeat < Materials.Epsilon) return;

        // Determine total heat absorption capacity
        var totalHeatTransferRate = _systems.Sum(system => system.AcceptsResource(Materials.Heat) * context.DeltaTime);
        if (totalHeatTransferRate < Materials.Epsilon) return;

        // Proportionally withdraw heat from hot systems
        var heatOutput = _hotSystems.Sum(system => system.WithdrawResource(Materials.Heat, system.Heat / totalHeat * totalHeatTransferRate));
        DepositResource(Materials.Heat, heatOutput);
    }
    
    /// <summary>
    /// Request a resource from the ship and return the amount received
    /// </summary>
    /// <returns></returns>
    public double WithdrawResource(Material material, double quantity)
    {
        var acquired = 0.0;
        foreach (var system in Systems)
        {
            acquired += system.WithdrawResource(material, quantity - acquired);
        }
        return acquired;
    }
    
    /// <summary>
    /// Supply a resource to the ship and return the amount that could be accepted
    /// </summary>
    /// <returns>Accepted quantity</returns>
    public double DepositResource(Material material, double quantity)
    {
        var supplied = 0.0;
        foreach (var system in Systems)
        {
            supplied += system.DepositResource(material, quantity - supplied);
            if (supplied - Materials.Epsilon >= quantity) break;
        }
        return supplied;
    }
    
    public void AddSystems(IEnumerable<ShipSystem> systems)
    {
        foreach (var system in systems)
            AddSystem(system);
    }
}

public class SpaceShip(string name) : SystemizedBody(name)
{
    public ShipHull Hull { get; set; } = ShipHull.EmptyHull;

    public void PowerOn()
    {
        foreach (var system in Systems.OfType<PoweredShipSystem>())
        {
            system.TargetPowerLevel = PowerLevel.Standard;
            system.Shutdown = false;
        }
    }
    
    public void Refuel(double percent = 1.0)
    {
        // Fill the fuel tanks
        foreach (var generator in Systems.OfType<EnergyGenerator>())
            FillStorage(generator.Fuel, percent);

        // Fill batteries
        FillStorage(Materials.Electricity, percent);
        
        // Fill shields
        FillStorage(Materials.ShieldEnergy, percent);
        FillStorage(Materials.AblativeShields, percent);
    }

    public void FillStorage(Material material, double percent)
    {
        var storage = Systems.OfType<StorageSystem>()
            .Where(x=>x.Material == material)
            .ToList();
        if (storage.Count == 0) return;
        
        var capacity = storage.DefaultIfEmpty().Sum(x => x?.Capacity ?? 0);
        if (capacity < Materials.Epsilon) return;
        
        DepositResource(material, capacity * percent);
    }
}

public abstract class ShipHull(string name) : ShipSystem(name)
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