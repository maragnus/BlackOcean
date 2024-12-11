using BlackOcean.Simulation.ControlPanels;
using BlackOcean.Simulation.Definitions;
using static BlackOcean.Simulation.ControlPanels.Status;

namespace BlackOcean.Simulation.ShipSystems;

public class ControlSystem : ShipSystem
{
    public StorageSystem? CoolantStorage { get; set; }
    public CoolingSystem? PassiveCooling { get; set; }
    public CoolingSystem? ActiveCooling { get; set; }

    public EnergyGenerator? MainEnergyGenerator { get; set; }
    public StorageSystem? MainFuelStorage { get; set; }
    public StorageSystem? MainBattery { get; set; }
    public EnergyGenerator? BackupEnergyGenerator { get; set; }
    public StorageSystem? BackupFuelStorage { get; set; }
    public StorageSystem? BackupBattery { get; set; }

    public ShieldGenerator? EnergyShieldGenerator { get; set; }
    public ShieldGenerator? AblativeShieldGenerator { get; set; }
    public StorageSystem? EnergyShieldStorage { get; set; }
    public StorageSystem? AblativeShieldStorage { get; set; }

    public ControlPanels.ControlPanel ControlPanel { get; } = new();

    private List<ShipSystem>? systems;
    private List<EnergyGenerator> generators = default!;
    private ILookup<Material,StorageSystem> stores = default!;
    private ILookup<ShieldType,ShieldGenerator> shields = default!;
    private List<StorageSystem> batteries = default!;
    private List<CoolingSystem> coolers = default!;
    private List<PoweredShipSystem> powered = default!;

    public void Initialize()
    {
        systems = (Parent?.Systems ?? []).ToList();
        generators = systems.OfType<EnergyGenerator>().OrderByDescending(x=> x.BaseOutput).ToList();
        powered = systems.OfType<PoweredShipSystem>().Except(generators).ToList();
        stores = systems.OfType<StorageSystem>().ToLookup(x => x.Material);
        shields = systems.OfType<ShieldGenerator>().ToLookup(x => x.ShieldType);
        batteries = stores[Materials.Electricity].OrderByDescending(x => x.Capacity).ToList();
        coolers = systems.OfType<CoolingSystem>().OrderByDescending(x => x.Output).ToList();
        
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

        SetGeneratorMeter(ControlPanel.Generated, generators.Sum(x => x.BaseOutput) * 1.5, 0.33, 0.66);
        SetGeneratorMeter(ControlPanel.Draw, powered.Sum(x => x.BaseConsumption) * 1.5, 0.33, 0.66);
        
        SetStorageMeter(ControlPanel.Fuel, MainFuelStorage?.Capacity ?? 0, 0.10, 0.25);
        SetStorageMeter(ControlPanel.EmergencyFuel, BackupFuelStorage?.Capacity ?? 0, 0.10, 0.25);
        SetStorageMeter(ControlPanel.Battery, MainBattery?.Capacity ?? 0, 0.25, 0.5);
        SetStorageMeter(ControlPanel.EmergencyBattery, BackupBattery?.Capacity ?? 0, 0.25, 0.5);
    }

    private void SetGeneratorMeter(Gauge gauge, double max, double warnPercent, double dangerPercent)
    {
        gauge.Max = max;
        gauge.Bands = Band.Build(Danger, (max * dangerPercent, Warn), (max * warnPercent, Safe));
    }
    
    private void SetStorageMeter(Gauge gauge, double max, double dangerPercent, double warnPercent)
    {
        gauge.Max = max;
        gauge.Bands = Band.Build(Danger, (max * dangerPercent, Warn), (max * warnPercent, Safe));
    }

    public override void Simulate(SimulationContext context)
    {
        if (systems is null) Initialize();
        
        ControlPanel.Reactor.Populate(MainEnergyGenerator);
        ControlPanel.EmergencyReactor.Populate(MainEnergyGenerator);
        ControlPanel.EnergyShield.Populate(EnergyShieldGenerator);
        ControlPanel.AblativeShield.Populate(AblativeShieldGenerator);
        ControlPanel.Radiator.Populate(PassiveCooling);
        ControlPanel.Cooler.Populate(ActiveCooling);
        ControlPanel.EmergencyPower.Populate(BackupBattery);
        
        ControlPanel.Generated.Value = generators.Sum(x => x.CurrentOutput);
        ControlPanel.Draw.Value = powered.Sum(x => x.CurrentConsumption);
        ControlPanel.Battery.Value = MainBattery?.Amount ?? 0;
        ControlPanel.EmergencyBattery.Value = BackupBattery?.Amount ?? 0;
    }
}
