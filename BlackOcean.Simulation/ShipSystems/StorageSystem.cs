using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation.ShipSystems;

public abstract class StorageSystem : ShipSystem
{
    public abstract Material Material { get; }
    public abstract double Capacity { get; }
    public abstract double TransferRate { get; }
    public double RemainingCapacity => Capacity - Amount;
    public bool IsFull => RemainingCapacity < Materials.Epsilon;
    public bool IsEmpty => Amount < Materials.Epsilon;
    public double Amount { get; private set; }
    public bool EnableWithdraw { get; set; } = true;
    public bool EnableDeposit { get; set; } = true;
    
    public StorageSystem() {}
    public StorageSystem(string name) : base(name) { }
    
    public override double WithdrawResource(Material material, double quantity)
    {
        if (!EnableWithdraw) return 0;
        if (material != Material || quantity <= Materials.Epsilon || IsEmpty) return 0;
        var accepted = Math.Min(quantity, Amount);
        Amount -= accepted;
        return accepted;
    }

    public override double DepositResource(Material material, double quantity)
    {
        if (!EnableDeposit) return 0;
        if (material != Material || quantity <= Materials.Epsilon || IsFull) return 0;
        var supplied = Math.Min(quantity, RemainingCapacity);
        Amount += supplied;
        return supplied;
    }

    public override double AcceptsResource(Material material)
    {
        if (Material != material) return 0;
        return Math.Min(TransferRate, RemainingCapacity);
    }

    public override void Simulate(SimulationContext context) { }
}

public class PrefabStorageSystem(string name, Material material, double capacity, double transferRate) : StorageSystem(name)
{
    public override Material Material { get; } = material;
    public override double Capacity { get; } = capacity;
    public override double TransferRate { get; } = transferRate;
}
