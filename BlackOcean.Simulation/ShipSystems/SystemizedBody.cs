using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using BlackOcean.Common;
using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation.ShipSystems;

readonly struct MaterialStores()
{
    public readonly List<MaterialStore> Suppliers = [];
    public readonly List<MaterialStore> Consumers = [];
    public readonly List<MaterialStore> Storages = [];
}

public abstract class SystemizedBody(string name) : SpaceBody(name)
{
    public IReadOnlyCollection<ShipComponent> Components => _components;
    public double HeatTrend { get; set; }
    
    private readonly HashSet<ShipComponent> _components = [];
    private readonly Dictionary<Material, MaterialStores> _storages = new();
    private bool _stateHasUpdated;

    public void SystemHasUpdated() => _stateHasUpdated = true;

    public void AddSystem(ShipComponent component)
    {
        component.Body = this;
        _components.Add(component);
        SystemHasUpdated();
    }
    
    public void RemoveSystem(ShipComponent component)
    {
        _components.Add(component);
        component.Body = null;
        SystemHasUpdated();
    }
    
    public override void Simulate(SimulationContext context)
    {
        base.Simulate(context);

        if (_stateHasUpdated) UpdateState();
        
        foreach (var system in Components)
            system.Simulate(context);

        SimulateMaterialTransport(context);
    }
    
    private void SimulateMaterialTransport(SimulationContext context)
    {
        foreach (var (_, materialStorage) in _storages)
        {
            // Fill every consumer from producers
            TransferMaterial(materialStorage.Suppliers, materialStorage.Consumers);
            // Fill every consumer from storages
            TransferMaterial(materialStorage.Storages, materialStorage.Consumers);
            // Fill every storage from producers
            TransferMaterial(materialStorage.Suppliers, materialStorage.Storages);

            // Simulate all of them
            foreach (var supplier in materialStorage.Suppliers) supplier.Simulate(context);
            foreach (var storage in materialStorage.Suppliers) storage.Simulate(context);
            foreach (var consumer in materialStorage.Consumers) consumer.Simulate(context);
        }
    }

    private static void TransferMaterial(List<MaterialStore> source, List<MaterialStore> destination)
    {
        if (source.Count == 0 && destination.Count == 0) return;
        
        var demand = destination.Sum(x => x.Available);

        var supply = ConsumeMaterial(source, demand);
        var consumed = SupplyMaterial(destination, supply);
        var loss = supply - consumed;

        if (loss > Materials.Epsilon)
        {
            Console.WriteLine($"Lost {source[0].Material.Name}: Demand={demand}, Supply={supply}, Loss={loss}");
            Debugger.Break();
        }
    }

    private static double ConsumeMaterial(List<MaterialStore> source, double demand)
    {
        var consumed = 0.0;

        // TODO: Prevent infinite loop
        while (consumed < demand - Materials.Epsilon)
        {
            var destinationCount = source.Count(x => x.Amount >= Materials.Epsilon);
            if (destinationCount == 0) break;
            var each = (demand - consumed) / destinationCount;
            foreach (var storage in source) 
                consumed += storage.Consume(each);
        }

        return consumed;
    }
    
    private static double SupplyMaterial(List<MaterialStore> destination, double supply)
    {
        var supplied = 0.0;
        
        // TODO: Prevent infinite loop
        while (supplied  < supply - Materials.Epsilon)
        {
            var destinationCount = destination.Count(x => x.Available >= Materials.Epsilon);
            if (destinationCount == 0) break;
            var each = (supply - supplied) / destinationCount;
            foreach (var storage in destination)
                supplied += storage.Supply(each);
        }

        return supplied;
    }

    private void UpdateState()
    {
        _storages.Clear();

        var components = _components
            .SelectMany(component => component.Storages)
            .Where(storage => storage.IsEnabled);
        foreach (var storage in components)
        {
            if (!storage.IsEnabled) continue;
            
            var material = _storages.GetOrAdd(storage.Material, _ => new MaterialStores());
            
            var list = storage.StorageType switch
            {
                ComponentStorageType.Supplier => material.Suppliers,
                ComponentStorageType.Consumer => material.Consumers,
                ComponentStorageType.Storage => material.Storages,
                _ => throw new ArgumentOutOfRangeException()
            };
            list.Add(storage);
        }
    }

    public void AddSystems(IEnumerable<ShipComponent> systems)
    {
        foreach (var system in systems)
            AddSystem(system);
        SystemHasUpdated();
    }

    public (double produced, double consumed, double net) AggregateMaterial(Material material)
    {
        // Do not use TryGetValue because it allocates 3x List<> with MaterialStores
        if (!_storages.ContainsKey(material)) return (0.0, 0.0, 0.0);
        
        var stores = _storages[material];
        var produced = stores.Suppliers.Sum(x => x.Supplied);
        var consumed = stores.Consumers.Sum(x => x.Consumed);
        var net = produced - consumed; //stores.Storages.Sum(x => x.Trend);
        return (produced, consumed, net);
    }
}