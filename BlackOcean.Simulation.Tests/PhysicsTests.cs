using BlackOcean.Simulation.Definitions;
using BlackOcean.Simulation.Scenarios;
using Jitter2.LinearMath;

namespace BlackOcean.Simulation.Tests;

public class PhysicsTests
{
    [Test]
    public void SimplePhysicsMovement()
    {
        var game = Game.StartScenario<TestSectorScenario>();
        var scenario = (TestSectorScenario)game.Scenario;

        scenario.Sector.SimulationLevel = SimulationLevel.Simple;
        
        var playerShip = scenario.PlayerShip;
        
        var startPosition = playerShip.Physics.Position;
        scenario.PlayerShip.Physics.LinearVelocity = JVector.UnitY;
        
        game.Simulate(0.00, 0.00);
        game.Simulate(0.25, 0.25);
        game.Simulate(0.75, 0.50);
        game.Simulate(1.00, 0.25);
        
        Assert.That((playerShip.Physics.Position - startPosition).Length(), Is.EqualTo(1d).Within(0.01));
    }
    
    [Test]
    public void FullPhysicsMovement()
    {
        var game = Game.StartScenario<TestSectorScenario>();
        var scenario = (TestSectorScenario)game.Scenario;

        scenario.Sector.SimulationLevel = SimulationLevel.Full;
        
        var playerShip = scenario.PlayerShip;
        
        var startPosition = playerShip.Physics.Position;
        scenario.PlayerShip.Physics.LinearVelocity = JVector.UnitY;
        
        game.Simulate(0.00, 0.00);
        game.Simulate(0.25, 0.25);
        game.Simulate(0.75, 0.50);
        game.Simulate(1.00, 0.25);
        
        Assert.That((playerShip.Physics.Position - startPosition).Length(), Is.EqualTo(1d).Within(0.01));
    }
    
    [Test]
    public void SplitPhysicsMovement()
    {
        var game = Game.StartScenario<TestSectorScenario>();
        var scenario = (TestSectorScenario)game.Scenario;

        scenario.Sector.SimulationLevel = SimulationLevel.Simple;
        
        var playerShip = scenario.PlayerShip;
        
        var startPosition = playerShip.Physics.Position;
        scenario.PlayerShip.Physics.LinearVelocity = JVector.UnitY;
        
        game.Simulate(0.00, 0.00);
        game.Simulate(0.25, 0.25);
        scenario.Sector.SimulationLevel = SimulationLevel.Full;
        game.Simulate(0.50, 0.25);
        game.Simulate(0.75, 0.25);
        scenario.Sector.SimulationLevel = SimulationLevel.Simple;
        game.Simulate(1.00, 0.25);
        
        Assert.That((playerShip.Physics.Position - startPosition).Length(), Is.EqualTo(1d).Within(0.01));
    }
    
    [Test]
    public void SolarSystem()
    {
        var game = Game.StartScenario<SolarSystemScenario>();
        var scenario = (SolarSystemScenario)game.Scenario;

        var earth = game.SectorManager[PlanetName.Earth.ToString()];
        var objects = earth.ToList();
        
        Assert.Multiple(() =>
        {
            Assert.That(objects.SingleOrDefault(o => o.Name == "Mars"), Is.Not.Null);
            Assert.That(objects.SingleOrDefault(o => o.Name == "Earth"), Is.Not.Null);
            Assert.That(objects.SingleOrDefault(o => o.Name == "Venus"), Is.Not.Null);
        });
    }
}