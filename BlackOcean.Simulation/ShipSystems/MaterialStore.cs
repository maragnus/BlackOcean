using BlackOcean.Common;
using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation;


public enum ComponentStorageType
{
    Supplier,
    Consumer,
    Storage
}

public class MaterialStore(ShipComponent component, Material material, double capacity, ComponentStorageType componentStorageType)
{
    public ShipComponent Component { get; } = component;
    public Material Material { get; } = material;
    public ComponentStorageType StorageType { get; } = componentStorageType;
    public double Capacity { get; } = capacity;
    public double Amount { get; protected set; }
    public double Trend { get; private set; }
    public double Supplied { get; private set; }
    public double Consumed { get; private set; }
    public double Available => Capacity - Amount;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            Component.Body?.SystemHasUpdated();
        }
    }

    private RollingSum _supply = new();
    private double _suppliedAmount;
    private RollingSum _consume = new();
    private double _consumedAmount;
    private bool _isEnabled = true;

    public double Supply(double amount)
    {
        var transfer = Math.Min(Available, amount);
        Amount += transfer;
        _suppliedAmount += transfer;
        return transfer;
    }
    
    public double Consume(double amount)
    {
        var consumed = Math.Min(Amount, amount);
        Amount -= consumed;
        _consumedAmount += consumed;
        return consumed;
    }

    public void Simulate(SimulationContext context)
    {
        // Trend = _trend.AddValue(Amount, context.SimulationTime);
        Consumed = _consume.AddValue(_consumedAmount, context.SimulationTime);
        _consumedAmount = 0;
        Supplied = _supply.AddValue(_suppliedAmount, context.SimulationTime);
        _suppliedAmount = 0;
        Trend = Supplied - Consumed;
    }
}