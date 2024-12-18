using BlackOcean.Common;
using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation.ShipSystems;

public abstract class StorageComponent : ShipComponent
{
    public abstract Material Material { get; }
    public abstract double Capacity { get; }
    public abstract double TransferRate { get; }
    public MaterialStore Storage { get; set; }

    public StorageComponent() : this("Unnamed Storage") {}

    public StorageComponent(string name) : base(name)
    {
        Storage = Stores(Material, Capacity);
    }

    public override void Simulate(SimulationContext context) { }
}

public class PrefabStorageComponent(string name, Material material, double capacity, double transferRate) : StorageComponent(name)
{
    public override Material Material { get; } = material;
    public override double Capacity { get; } = capacity;
    public override double TransferRate { get; } = transferRate;
}
