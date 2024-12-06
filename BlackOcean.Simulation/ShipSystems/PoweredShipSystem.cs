﻿namespace BlackOcean.Simulation.ShipSystems;

public enum PowerLevel
{
    Hibernate = 0,
    Suspend = 1,
    Standard = 2,
    Boost = 3,
    Overdrive = 4
}

public abstract class PoweredShipSystem : ShipSystem
{
    public bool IsOperating { get; private set; }
    public bool Shutdown { get; set; }
    public PowerLevel PowerLevel { get; set; }
    
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

    public void Calculate(SimulationContext context)
    {
        var idealPowerLevel = Shutdown ? 0 : Math.Clamp((double)PowerLevel, 0.0, 4.0);
        CurrentPowerLevel = Math.Clamp(MathD.Approach(CurrentPowerLevel, idealPowerLevel, LevelPerSecond * context.DeltaTime), 0.0, 4.0);

        if (Math.Abs(CurrentPowerLevel - idealPowerLevel) < LevelPerSecond)
            CurrentPowerLevel = idealPowerLevel;

        if (Shutdown && CurrentPowerLevel <= LevelPerSecond)
        {
            IsOperating = false;
            CurrentEfficiency = 0;
            CurrentConsumption = 0;
            CurrentOutput = 0;
            CurrentHeatOutput = 0;
            return;
        }
        
        var efficiency = 1.0 - CurrentPowerLevel * 0.05;
        var consumption = 0.1 + 0.4 * CurrentPowerLevel + 0.05 * Math.Pow(CurrentPowerLevel, 2);
        var output = CurrentPowerLevel * CurrentEfficiency;
        var heatOutput = CurrentPowerLevel * (1.0 - CurrentEfficiency);

        IsOperating = true;
        CurrentEfficiency = efficiency * BaseEfficiency;
        CurrentConsumption = consumption * BaseConsumption;
        CurrentOutput = output * BaseOutput;
        CurrentHeatOutput = heatOutput * BaseHeatOutput;
    }
}