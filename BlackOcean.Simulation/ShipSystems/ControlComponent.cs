using BlackOcean.Common;
using BlackOcean.Simulation.ControlPanels;
using BlackOcean.Simulation.Definitions;
using BlackOcean.Simulation.Internal;
using static BlackOcean.Simulation.ControlPanels.Status;

namespace BlackOcean.Simulation.ShipSystems;

public class ControlComponent : ShipComponent
{
    public StorageComponent? CoolantStorage { get; set; }
    public CoolingComponent? PassiveCooling { get; set; }
    public CoolingComponent? ActiveCooling { get; set; }

    public EnergyGenerator? MainEnergyGenerator { get; set; }
    public StorageComponent? MainFuelStorage { get; set; }
    public StorageComponent? MainBattery { get; set; }
    public EnergyGenerator? BackupEnergyGenerator { get; set; }
    public StorageComponent? BackupFuelStorage { get; set; }
    public StorageComponent? BackupBattery { get; set; }

    public ShieldGenerator? EnergyShieldGenerator { get; set; }
    public ShieldGenerator? AblativeShieldGenerator { get; set; }
    public StorageComponent? EnergyShieldStorage { get; set; }
    public StorageComponent? AblativeShieldStorage { get; set; }
    
    public ControlPanel ControlPanel { get; } = new();

    private List<ShipComponent>? systems;
    private List<EnergyGenerator> generators = default!;
    private ILookup<Material,StorageComponent> stores = default!;
    private ILookup<ShieldType,ShieldGenerator> shields = default!;
    private List<StorageComponent> batteries = default!;
    private List<CoolingComponent> coolers = default!;
    private List<PoweredShipComponent> powered = default!;

    // TODO: Temporary
    private RandomDrift _radiationDrift = new(-100, 150, 10, 20, 0);
    
    private List<(PoweredShipComponent? component, PoweredComponentControl control)> _poweredSystems = [];
    
    private void Initialize()
    {
        LinkComponents();
        ConfigureControls();
    }

    private void LinkComponents()
    {
        systems = (Body?.Components ?? []).ToList();
        generators = systems.OfType<EnergyGenerator>().OrderByDescending(x=> x.BaseOutput).ToList();
        powered = systems.OfType<PoweredShipComponent>().Except(generators).ToList();
        stores = systems.OfType<StorageComponent>().ToLookup(x => x.Material);
        shields = systems.OfType<ShieldGenerator>().ToLookup(x => x.ShieldType);
        batteries = stores[Materials.Electricity].OrderByDescending(x => x.Capacity).ToList();
        coolers = systems.OfType<CoolingComponent>().OrderByDescending(x => x.BaseConsumption).ToList();
        
        MainEnergyGenerator = generators.FirstOrDefault();
        MainFuelStorage = MainEnergyGenerator is null ? null : stores[MainEnergyGenerator.Fuel].FirstOrDefault();
        MainBattery = batteries.FirstOrDefault();
        BackupEnergyGenerator = generators.Skip(1).FirstOrDefault();
        BackupFuelStorage = BackupEnergyGenerator is null ? null : stores[BackupEnergyGenerator.Fuel].FirstOrDefault();
        BackupBattery = batteries.Skip(1).FirstOrDefault();
        EnergyShieldGenerator = shields[ShieldType.Energy].FirstOrDefault();
        EnergyShieldStorage = stores[Materials.ShieldEnergy].FirstOrDefault();
        AblativeShieldGenerator = shields[ShieldType.Ablative].FirstOrDefault();
        AblativeShieldStorage = stores[Materials.AblativeShields].FirstOrDefault();
        CoolantStorage = stores[Materials.Heat].FirstOrDefault();
        ActiveCooling = coolers.FirstOrDefault();
        PassiveCooling = coolers.Skip(1).FirstOrDefault();
        
        _poweredSystems.Add((MainEnergyGenerator, ControlPanel.Reactor));
        _poweredSystems.Add((BackupEnergyGenerator, ControlPanel.EmergencyReactor));
        _poweredSystems.Add((EnergyShieldGenerator, ControlPanel.EnergyShield));
        _poweredSystems.Add((AblativeShieldGenerator, ControlPanel.AblativeShield));
        _poweredSystems.Add((null, ControlPanel.LifeSupport));
        _poweredSystems.Add((null, ControlPanel.ImpulseDrive));
        _poweredSystems.Add((null, ControlPanel.WarpDrive));
        _poweredSystems.Add((null, ControlPanel.EnergyWeapon));
        _poweredSystems.Add((null, ControlPanel.Scanner));
        _poweredSystems.Add((null, ControlPanel.Communications));
        
        // batteries.Add((BackupBattery, ControlPanel.EmergencyPower));
        // cooling.Add((PassiveCooling, ControlPanel.Radiator));
        // cooling.Add((ActiveCooling, ControlPanel.Cooler));
    }
    
    private void ConfigureControls()
    {
        foreach (var (component, control) in _poweredSystems)
        {
            control.Available = component is not null;
            control.Name = component?.Name ?? "Not Installed";
            control.Operating = component?.IsOperating ?? false;
            control.SetLevel = (int)(component?.TargetPowerLevel ?? PowerLevel.Standard);
            control.CurrentLevel = component?.CurrentPowerLevel ?? 0;
            control.CurrentHeat = component?.HeatOutput.Amount ?? 0;
            control.CurrentOutput = component?.CurrentOutput ?? 0;
            control.NominalOutput = component?.BaseOutput ??0;
            control.Powered.Available = component is not null;
            control.Powered.Pressed = !(component?.Shutdown ?? true);
            control.Auto.Available = component?.Automatable ?? false;
            control.Auto.Pressed = component?.Automated ?? false;
        }
        
        // Interior Safe: 0-50 µSv/h,  Warn: 50-500 µSv/h,  Danger: 500+ µSv/h
        ControlPanel.InteriorExposure.Interval = "hour";
        ControlPanel.InteriorExposure.Bands = Band.Build(Safe, (0.000050, Warn), (0.000500, Danger));
        ControlPanel.InteriorExposure.Min = 0.00002; //   20 µSv/h
        ControlPanel.InteriorExposure.Max = 0.00100; // 1000 µSv/h
        ControlPanel.InteriorExposure.Scale = Scale.Log;

        // Exterior Safe: 0-500 µSv/h,  Warn: 500-1000 µSv/h,  Danger: 1000+ µSv/h
        ControlPanel.ExteriorExposure.Interval = "hour";
        ControlPanel.ExteriorExposure.Bands = Band.Build(Safe, (0.000500, Warn), (0.001000, Danger));
        ControlPanel.ExteriorExposure.Min = 0.000100; //   20 µSv/h
        ControlPanel.ExteriorExposure.Max = 0.001500; // 1500 µSv/h
        ControlPanel.ExteriorExposure.Scale = Scale.Log;

        var avgHeatGain = powered.Sum(x => x.StandardHeatOutput);
        var maxHeatGain = avgHeatGain * 1.5;
        var maxHeatPurge = coolers.Sum(x => x.BaseConsumption);
        var max = Math.Max(maxHeatGain, maxHeatPurge);
        
        ControlPanel.HeatDelta.Min = -max;
        ControlPanel.HeatDelta.Max = max;
        ControlPanel.HeatDelta.Scale = Scale.Linear;
        ControlPanel.HeatDelta.Bands = Band.Build(Safe, (maxHeatPurge, Warn), (max, Danger));
        
        ControlPanel.HeatPurge.Configure(0, maxHeatPurge, true, 0.50, 0.80);
        ControlPanel.HeatStore.Configure(0, CoolantStorage?.Capacity ?? 0, true, 0.50, 0.80);
        
        ControlPanel.Generated.Configure(0, generators.Sum(x => x.BaseOutput) * 1.5, true, 0.667, 0.85);
        ControlPanel.Draw.Configure(0, generators.Sum(x => x.BaseOutput) * 1.5, false, 0.667, 0.85);

        ControlPanel.Fuel.Configure(0, MainFuelStorage?.Capacity ?? 0, true, 0.10, 0.25);
        ControlPanel.EmergencyFuel.Configure(0, BackupFuelStorage?.Capacity ?? 0, true, 0.10, 0.25);
        ControlPanel.Battery.Configure(0, MainBattery?.Capacity ?? 0, true, 0.25, 0.5);
        ControlPanel.EmergencyBattery.Configure(0, BackupBattery?.Capacity ?? 0, true, 0.25, 0.5);
    }

    public override void Simulate(SimulationContext context)
    {
        _radiationDrift.Update(context.DeltaTime);

        if (Body is null) return;
        if (systems is null)
            Initialize();
        else
            HandleInput();

        UpdateState();
    }

    private void HandleInput()
    {
        // Apply changes from controls
        foreach (var (component, control) in _poweredSystems)
        {
            if (component is null) continue;
            component.Shutdown = !control.Powered.Pressed;
            component.Automated = component.Automatable && control.Auto.Pressed;
            component.TargetPowerLevel = (PowerLevel)control.SetLevel;
        }
        
        // Automatically regulate generators
        RegulateGenerator(MainBattery, MainEnergyGenerator);
        RegulateGenerator(EnergyShieldStorage, EnergyShieldGenerator);
        RegulateGenerator(AblativeShieldStorage, AblativeShieldGenerator);
    }
    
    private void UpdateState() {
        if (Body is null) return;
        
        foreach (var (component, control) in _poweredSystems)
        {
            if (component is null) continue;
            control.Operating = component?.IsOperating ?? false;
            control.SetLevel = (int)(component?.TargetPowerLevel ?? PowerLevel.Standard);
            control.CurrentLevel = component?.CurrentPowerLevel ?? 0;
            control.CurrentHeat = component?.HeatOutput.Amount ?? 0;
            control.CurrentOutput = component?.CurrentOutput ?? 0;
            control.Powered.Pressed = !(component?.Shutdown ?? true);
            control.Auto.Pressed = component?.Automated ?? false;
            control.Powered.Available = control.Available && (!control.Auto.Available || !control.Auto.Pressed);
        }
        
        ControlPanel.EmergencyPower.Populate(BackupBattery);
        ControlPanel.Radiator.Populate(PassiveCooling);
        ControlPanel.Cooler.Populate(ActiveCooling);

        // Energy production and usage
        var energy = Body.AggregateMaterial(Materials.Electricity);
        ControlPanel.Generated.Value = energy.produced;
        ControlPanel.Draw.Value = energy.consumed;
        ControlPanel.Energy.Value = energy.net;
        
        // Battery Levels
        ControlPanel.Battery.Value = MainBattery?.Storage.Amount ?? 0;
        ControlPanel.EmergencyBattery.Value = BackupBattery?.Storage.Amount ?? 0;

        // Heat production, storage, and dispersion
        var heat = Body.AggregateMaterial(Materials.Heat);
        ControlPanel.HeatDelta.Value = heat.produced;
        ControlPanel.HeatStore.Value = CoolantStorage?.Storage.Amount ?? 0;
        ControlPanel.HeatPurge.Value = heat.consumed;
        
        // Fuel economy
        var fuelType = MainFuelStorage?.Material ?? Materials.Uranium;
        var fuel = Body.AggregateMaterial(fuelType);
        ControlPanel.FuelConsumption.Value = fuel.consumed;
        ControlPanel.FuelEfficiency.Value = MainEnergyGenerator == null || MainEnergyGenerator.CurrentEfficiency == 0
            ? 0 : fuelType.MegaJoulesPerLiter * MainEnergyGenerator.CurrentEfficiency;
        
        // Fuel tanks
        ControlPanel.Fuel.Populate(MainFuelStorage);
        ControlPanel.EmergencyFuel.Populate(BackupFuelStorage);

        // Radiation reading
        ControlPanel.ExteriorExposure.Value = 0.000450 + _radiationDrift.CurrentValue * 0.000001; // 450 µSv/h 
        ControlPanel.InteriorExposure.Value = ControlPanel.ExteriorExposure.Value / 9; // 50 µSv/h
    }

    private static void RegulateGenerator(StorageComponent? battery, PoweredShipComponent? generator)
    {
        if (battery is null 
            || generator is null
            || !generator.Automatable
            || !generator.Automated) return;
        
        var percent = battery.Storage.Amount / battery.Capacity;
        generator.TargetPowerLevel = percent switch
        {
            < 0.2 => PowerLevel.Boost,
            > 0.95 => PowerLevel.Hibernate,
            > 0.8 => PowerLevel.Suspend,
            _ => PowerLevel.Standard
        };
        
        if (generator.Shutdown && percent < 0.8)
            generator.Shutdown = false;
        else if (!generator.Shutdown && percent > 0.99)
            generator.Shutdown = true;
    }    
}
