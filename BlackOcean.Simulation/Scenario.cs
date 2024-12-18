using BlackOcean.Simulation.Definitions;
using BlackOcean.Simulation.ShipSystems;

namespace BlackOcean.Simulation;

public abstract class Scenario() : ISimulated
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    public Game Game { get; private set; } = default!;
    
    public abstract void Initialize();

    public virtual void Simulate(SimulationContext context)
    {
    }

    public abstract Player CreatePlayer(string name);
    
    protected Player CreatePlayer(Sector sector, string playerName, string shipName)
    {
        var controlPanel = new ControlComponent();
        var ship = Prefabs.GetShip(shipName);
        ship.Name = playerName;
        ship.Sector = sector;
        ship.Refuel(0.5);
        ship.AddSystem(controlPanel);
        ship.PowerOn();
        
        var player = new Player
        {
            Name = playerName,
            ControlPanel = controlPanel.ControlPanel,
            Faction = Game.FactionManager.Add(playerName),
            SpaceShip = ship
        };
        return player;
    }
}