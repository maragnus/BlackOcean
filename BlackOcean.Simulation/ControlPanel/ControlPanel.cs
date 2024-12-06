using static BlackOcean.Simulation.ShipSystems.Status;

namespace BlackOcean.Simulation.ShipSystems;

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

    public Meter Fuel = new() { Name = "Fuel", Unit = "liter", Min = 0, Max = 1, Bands = Band.Build(Safe) };
    public Meter EmergencyFuel = new() { Name = "EP Fuel", Unit = "liter", Min = 0, Max = 1, Bands = Band.Build(Safe) };
    
    public Meter ForwardShield = new() { Name = "F SHD", Unit = "percent", Min = 0, Max = 1, Bands = Band.Build(Danger, (0.33, Warn), (0.66, Safe)) };
    public Meter AftShield = new() { Name = "A SHD", Unit = "percent", Min = 0, Max = 1, Bands = Band.Build(Danger, (0.33, Warn), (0.66, Safe)) };
    
    public Meter Generated = new() { Name = "Generating", Unit = "watt", Min = 0, Max = 1000, Bands = Band.Build(Safe) };
    public Meter Draw = new() { Name = "Draw", Unit = "watt", Min = 0, Max = 1000, Bands = Band.Build(Safe) };
    public Meter Battery = new() { Name = "Battery", Unit = "joule", Min = 0, Max = 1000, Bands = Band.Build(Danger, (250, Warn), (500, Safe)) };
    public Meter EmergencyBattery = new() { Name = "Battery", Unit = "joule", Min = 0, Max = 500, Bands = Band.Build(Danger, (150, Warn), (300, Safe)) };

    public Meter HeatGain = new() { Name = "Heat Gain", Unit = "watt", Min = 0, Max = 10000, Bands = Band.Build(Safe) };
    public Meter HeatPurge = new() { Name = "Heat Purge", Unit = "watt", Min = 0, Max = 10000, Bands = Band.Build(Safe) };
    public Meter HeatStore = new() { Name = "Heat Store", Unit = "joule", Min = 0, Max = 100000, Bands = Band.Build(Safe) };
    
    public Meter InteriorExposure = new() { Name = "Int Exp", Unit = "sievert", Min = 0, Max = 5, Bands = Band.Build(Safe, (3.5, Warn), (4, Danger)) };
    public Meter ExteriorExposure = new() { Name = "Ext Exp", Unit = "sievert", Min = 0, Max = 15, Bands = Band.Build(Safe, (10, Warn), (13, Danger)) };
}