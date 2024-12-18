using BlackOcean.Simulation.Definitions;
using BlackOcean.Simulation.ShipSystems;

namespace BlackOcean.Simulation.Tests;

public class ShipTests
{
    private const string GeneratorName = "Uranium Generator 1MW";
    private const string BatteryName = "Battery 100MJ";
    private const string FuelStoreName = "Uranium Store 1L";
    private const string CoolantName = "Coolant 100MJ";
    private const string RadiatorName = "Radiator 72kW";

    [Test]
    public void Prefab()
    {
        var ship = Prefabs.GetShip("Basic Ship");
        InitializeShip(ship);
        SimulateShip(ship, 10);
    }

    [Test]
    public void Batteries()
    {
        var ship = new SpaceShip("Battery Test");
        var reactor = Prefabs.GetGenerator(GeneratorName);
        var battery = Prefabs.GetStorage(BatteryName);
        var fuel = Prefabs.GetStorage(FuelStoreName);
        ship.AddSystem(battery);
        ship.AddSystem(fuel);
        ship.AddSystem(reactor);
        ship.FillStorage(Materials.Uranium, 1.0);
        reactor.TargetPowerLevel = PowerLevel.Standard;
        
        // Simulate 0 seconds to initialize everything
        InitializeShip(ship);
        Assert.Multiple(() =>
        {
            Assert.That(battery.Storage.Amount, Is.EqualTo(0).Within(0.001));
            Assert.That(fuel.Storage.Amount, Is.EqualTo(0).Within(100.0));
        });

        // Simulate 10 seconds
        SimulateShip(ship, 10);
        Assert.Multiple(() =>
        {
            Assert.That(battery.Storage.Amount, Is.GreaterThan(1));
            Assert.That(fuel.Storage.Amount, Is.LessThan(100.0));
        });
    }

    [Test]
    public void Heat()
    {
        // Basic ship with Uranium generator
        var ship = new SpaceShip("Heat Test");
        var reactor = Prefabs.GetGenerator(GeneratorName);
        var battery = Prefabs.GetStorage(BatteryName);
        var fuel = Prefabs.GetStorage(FuelStoreName);
        ship.AddSystem(battery);
        ship.AddSystem(fuel);
        ship.AddSystem(reactor);
        ship.FillStorage(Materials.Uranium, 1.0);
        reactor.TargetPowerLevel = PowerLevel.Standard;
        
        // Initialize and verify baseline
        InitializeShip(ship);
        Assert.That(reactor.HeatOutput.Amount, Is.EqualTo(0).Within(0.001));
        
        // Simulate 10 seconds, generate some heat
        SimulateShip(ship, 10);
        var reactorHeat = reactor.HeatOutput.Amount;
        Assert.That(reactorHeat, Is.GreaterThan(0));

        // Add some coolant storage with TransferRate much greater than reactor
        var coolant = Prefabs.GetStorage(CoolantName);
        ship.AddSystem(coolant);
        
        // Transfer heat from reactor into coolant
        SimulateShip(ship, 10);
        Assert.Multiple(() =>
        {
            Assert.That(coolant.Storage.Amount, Is.GreaterThan(0));
            Assert.That(reactor.HeatOutput.Amount, Is.LessThan(reactorHeat));
        });

        // Add a radiator to purge the heat from storage
        var radiator = Prefabs.GetCooling(RadiatorName);
        ship.AddSystem(radiator);
        radiator.IsOpen = true;
        var storedHeat = coolant.Storage.Amount;
        
        SimulateShip(ship, 10);
        
        // Coolant is cooled by the radiator
        Assert.That(coolant.Storage.Amount, Is.LessThan(storedHeat));
    }
    
    private double _simulationTime = 0.0;
    
    private void InitializeShip(SpaceShip ship)
    {
        _simulationTime = 0;
        ship.Simulate(new SimulationContext(_simulationTime, 0));
    }
    
    private void SimulateShip(SpaceShip ship, int seconds)
    {
        for (var s = 0; s < seconds; s++)
        {
            _simulationTime += seconds;
            ship.Simulate(new SimulationContext(_simulationTime, 1));
        }
    }
}