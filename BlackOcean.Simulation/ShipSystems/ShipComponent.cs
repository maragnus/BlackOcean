using BlackOcean.Simulation.Definitions;
using BlackOcean.Simulation.ShipSystems;

namespace BlackOcean.Simulation;

public abstract class ShipComponent : ISimulated
{
    public SystemizedBody? Body { get; set; }
    public string Name { get; protected set; } = "";
    public IReadOnlyCollection<MaterialStore> Storages => _storages.Values;

    public ShipComponent() {}
    public ShipComponent(string name) { Name = name; }
    
    private Dictionary<Material, MaterialStore> _storages = new();

    public MaterialStore? this[Material material] => Storages.FirstOrDefault(x => x.Material == material);

    public abstract void Simulate(SimulationContext context);

    // This component supplies an amount of this material
    protected MaterialStore Consumes(Material material, double capacity)=> 
        AddStorage(material, capacity, ComponentStorageType.Consumer);

    // This component requests a supply of this material
    protected MaterialStore Supplies(Material material, double capacity) => 
        AddStorage(material, capacity, ComponentStorageType.Supplier);

    // This component requests a supply of this material
    protected MaterialStore Stores(Material material, double capacity) => 
        AddStorage(material, capacity, ComponentStorageType.Storage);
    
    private MaterialStore AddStorage(Material material, double capacity, ComponentStorageType type)
    {
        if (_storages.TryGetValue(material, out var componentStorage))
            return componentStorage;
        var supplier = new MaterialStore(this, material, capacity, type);
        _storages.Add(material, supplier);
        Body?.SystemHasUpdated();
        return supplier;
    }
}