namespace BlackOcean.Simulation.ShipSystems;

public enum PowerLevel
{
    Shutdown = 0,
    Hibernate = 1,
    Suspend = 2,
    Standard = 3,
    Boost = 4,
    Overdrive = 5
}

public abstract class PoweredShipSystem : ShipSystem
{
    public bool IsOperating { get; private set; }
    public bool Shutdown { get; set; }
    public PowerLevel TargetPowerLevel { get; set; }
    
    public double CurrentPowerLevel { get; private set; }
    public double CurrentConsumption { get; private set; }
    public double CurrentEfficiency { get; private set; }
    public double CurrentOutput { get; private set; }
    public double CurrentHeatOutput { get; private set; }

    public abstract double BaseConsumption { get; }
    public abstract double BaseEfficiency { get; }
    public abstract double BaseOutput { get; }
    public abstract double BaseHeatOutput { get; }
    public abstract double LevelPerSecond { get; }
    public abstract double ThermalLimit { get; }

    public PoweredShipSystem() {}
    public PoweredShipSystem(string name) : base(name) { }
    
    public override void Simulate(SimulationContext context)
    {
        Calculate(context);
    }

    private static readonly double[] Multiplier = [0.0, 0.25, 0.75, 1.0, 1.25, 1.5];
    
    public void Calculate(SimulationContext context)
    {
        if (TargetPowerLevel == PowerLevel.Shutdown) // Use Shutdown = true instead
            TargetPowerLevel = PowerLevel.Hibernate;
        
        var idealPowerLevel = Shutdown ? 0 : Math.Clamp((double)TargetPowerLevel, 0.0, 5.0);
        var currentPowerLevel = Math.Clamp(CurrentPowerLevel, 0.0, 5.0);
        CurrentPowerLevel = MathD.Approach(currentPowerLevel, idealPowerLevel, LevelPerSecond * context.DeltaTime);

        if (Shutdown && CurrentPowerLevel <= LevelPerSecond)
        {
            IsOperating = false;
            CurrentEfficiency = 0;
            CurrentConsumption = 0;
            CurrentOutput = 0;
            CurrentHeatOutput = 0;
            return;
        }
        
        var level = CurrentPowerLevel * 0.2;
        var output = CubicBezier(level, 0, 0.95, 1.6);
        var efficiency = (1 - CubicBezier(level, 0, .5, 0.1)) * BaseEfficiency;
        var consumption = output / efficiency;
        var heatOutput = consumption - output;
        
        IsOperating = true;
        CurrentEfficiency = efficiency * BaseEfficiency;
        CurrentConsumption = consumption * BaseConsumption;
        CurrentOutput = output * BaseOutput;
        CurrentHeatOutput = heatOutput * BaseHeatOutput;
    }

    private static double CubicBezier(double level, double y1, double y2, double multiplier)
    {
        return (Math.Pow(1 - level, 3) * 0 + 3 * Math.Pow(1 - level, 2) * level * y1 + 3 * (1 - level) * Math.Pow(level, 2) * y2 + Math.Pow(level, 3) * 1) * multiplier;
    }
}