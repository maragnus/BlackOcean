﻿using BlackOcean.Simulation.ShipSystems;

namespace BlackOcean.Simulation.ControlPanels;

public class PoweredSystem
{
    public string Name = "";
    public string Abbreviation = "";
    public string Icon = "";
    public bool Powered { get; set; }
    public bool Operating { get; set; }
    public int MinLevel = 1;
    public int MaxLevel = 5;
    public int SetLevel { get; set; } = 1;
    public double CurrentLevel = 0;
    public Status[] LevelStatuses = [ Status.Safe, Status.Safe, Status.Safe, Status.Warn, Status.Danger ];
    public double? CurrentHeat { get; set; }
    public Band[]? HeatBands { get; set; }
    public double CurrentOutput { get; set; }
    public double NominalOutput { get; set; }

    private static Band[] DefaultHeatBands = [Band.Default];
    
    public void Populate(PoweredShipSystem? system)
    {
        if (system is null)
        {
            Name = "Not Installed";
            Powered = false;
            Operating = false;
            SetLevel = 1;
            CurrentLevel = 0;
            CurrentHeat = 0;
            HeatBands = DefaultHeatBands;
            CurrentOutput = 0;
            NominalOutput = 0;
            return;
        }
        
        Name = system.Name;
        Powered = !system.Shutdown;
        Operating = system.IsOperating;
        SetLevel = (int)system.TargetPowerLevel;
        CurrentLevel = system.CurrentPowerLevel;
        CurrentHeat = system.Heat;
        CurrentOutput = system.CurrentOutput;
        NominalOutput = system.BaseOutput;
    }
}