using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation;

public abstract class ShipSystem : ISimulated
{
    public SystemizedBody? Parent { get; set; }
    public string Name { get; protected set; } = "";
    
    public ShipSystem() {}
    public ShipSystem(string name) { Name = name; }
    
    public abstract void Simulate(SimulationContext context);

    public double Heat { get; private set; }
    
    protected void AddHeat(double joules)
    {
        Heat += joules;
    }

    // By default, do not supply resources
    public virtual double WithdrawResource(Material material, double quantity)
    {
        if (material == Materials.Heat)
        {
            var amount = Math.Min(quantity, Heat);
            Heat -= amount;
            return amount;
        }

        return 0;
    }

    // By default, do not store resources
    public virtual  double DepositResource(Material material, double quantity) => 0;

    public virtual double AcceptsResource(Material material) => 0;
}