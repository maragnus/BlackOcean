using static BlackOcean.Simulation.ControlPanels.Status;

namespace BlackOcean.Simulation.ControlPanels;

public class ControlPanel
{
    public Identity Identity = new Identity();
    
    public PoweredSystem Reactor = new() { Name = "Reactor", Abbreviation = "RCT", Icon = "starfighter-twin-ion-engine-advanced" };
    public PoweredSystem EmergencyReactor = new() { Name = "Emergency Reactor", Abbreviation = "EPG", Icon = "light-emergency" };
    public PoweredSystem LifeSupport = new() { Name = "Life Support", Abbreviation = "LIS", Icon = "heart-pulse" };
    public PoweredSystem EnergyShield = new() { Name = "Energy Shield Generator", Abbreviation = "SHD^E", Icon = "shield-plus" };
    public PoweredSystem AblativeShield = new() { Name = "Ablative Shield Generator", Abbreviation = "SHD^A", Icon = "shield-quartered" };
    public PoweredSystem ImpulseDrive = new() { Name = "Impulse Drive", Abbreviation = "IMP", Icon = "rocket-launch" };
    public PoweredSystem WarpDrive = new() { Name = "Warp Drive", Abbreviation = "WRP", Icon = "coin-vertical" };
    public PoweredSystem EnergyWeapon = new() { Name = "Energy Weapon", Abbreviation = "LSR", Icon = "raygun" };
    public PoweredSystem Scanner = new() { Name = "Scanners", Abbreviation = "SCN", Icon = "radar" };
    public PoweredSystem Communications = new() { Name = "Communications Array", Abbreviation = "COM", Icon = "satellite-dish" };
    
    public Button ShieldPurge = new() { Name = "SPRG", Icon = "shield-minus", Toggle = true };
    public Button EmergencyPower = new() { Name = "EP", Icon = "car-battery", Toggle = true };
    public Button Radiator = new() { Name = "VENT", Icon = "vent-damper", Toggle = true };
    public Button Cooler = new() { Name = "COOL", Icon = "snowflake", Toggle = true };
    public Button Purge = new() { Name = "PURG", Icon = "eject" };

    public Gauge Fuel = new() { Name = "Fuel", Unit = "liter", Min = 0, Max = 1, Bands = Band.Build(Safe) };
    public Gauge FuelConsumption = new() { Name = "Fuel Consumption", Unit = "liter", Interval = "minute", Min = 0, Max = 1, Bands = Band.Build(Safe) };
    public Gauge FuelEfficiency = new() { Name = "Fuel Efficiency", Unit = "joule", Interval = "liter", Min = 0, Max = 1, Bands = Band.Build(Safe) };
    public Gauge EmergencyFuel = new() { Name = "EP Fuel", Unit = "liter", Min = 0, Max = 1, Bands = Band.Build(Safe) };
    
    public Gauge AblativeShielding = new() { Name = "ABL", Unit = "percent", Min = 0, Max = 1, Bands = Band.Build(Danger, (0.33, Warn), (0.66, Safe)) };
    public Gauge ForwardShield = new() { Name = "F SHD", Unit = "percent", Min = 0, Max = 1, Bands = Band.Build(Danger, (0.33, Warn), (0.66, Safe)) };
    public Gauge AftShield = new() { Name = "A SHD", Unit = "percent", Min = 0, Max = 1, Bands = Band.Build(Danger, (0.33, Warn), (0.66, Safe)) };
    
    public Gauge Generated = new() { Name = "Generating", Unit = "watt", Min = 0, Max = 1000, Bands = Band.Build(Safe) };
    public Gauge Draw = new() { Name = "Draw", Unit = "watt", Min = 0, Max = 1000, Bands = Band.Build(Safe) };
    public Gauge Battery = new() { Name = "Battery", Unit = "joule", Min = 0, Max = 1000, Bands = Band.Build(Danger, (250, Warn), (500, Safe)) };
    public Gauge EmergencyBattery = new() { Name = "Battery", Unit = "joule", Min = 0, Max = 500, Bands = Band.Build(Danger, (150, Warn), (300, Safe)) };

    public Gauge HeatGain = new() { Name = "Heat Gain", Unit = "watt", Min = 0, Max = 10000, Bands = Band.Build(Safe) };
    public Gauge HeatPurge = new() { Name = "Heat Purge", Unit = "watt", Min = 0, Max = 10000, Bands = Band.Build(Safe) };
    public Gauge HeatStore = new() { Name = "Heat Store", Unit = "joule", Min = 0, Max = 100000, Bands = Band.Build(Safe) };
    
    public Gauge InteriorExposure = new() { Name = "Int Exp", Unit = "sievert", Interval = "hour", Min = 0, Max = 5, Bands = Band.Build(Safe, (3.5, Warn), (4, Danger)) };
    public Gauge ExteriorExposure = new() { Name = "Ext Exp", Unit = "sievert", Interval = "hour", Min = 0, Max = 15, Bands = Band.Build(Safe, (10, Warn), (13, Danger)) };
}