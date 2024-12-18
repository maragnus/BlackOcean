using BlackOcean.Simulation.Definitions;
using BlackOcean.Simulation.ShipSystems;

namespace BlackOcean.Simulation;

public class TestSectorScenario : EmptySectorScenario
{
    public Faction EnemyFaction { get; private set; } = default!;
    public Faction FriendlyFaction { get; private set; } = default!;
    public Faction PlayerFaction { get; private set; } = default!;
    public SpaceShip EnemyShip { get; private set; } = default!;
    public SpaceShip FriendlyShip { get; private set; } = default!;
    public SpaceShip PlayerShip { get; private set; } = default!;

    public override void Initialize()
    {
        base.Initialize();
        
        EnemyFaction = Game.FactionManager.Add("Enemy");
        EnemyShip = CreateSpaceShip("Enemy Ship", JVector.UnitX * -100, JVector.Zero);
        EnemyFaction.OwnedBodies.Add(EnemyShip);
        
        FriendlyFaction = Game.FactionManager.Add("Friendly");
        FriendlyShip = CreateSpaceShip("Friendly Ship", JVector.UnitZ * 100, JVector.Zero);
        FriendlyFaction.OwnedBodies.Add(FriendlyShip);
        
        PlayerFaction = Game.FactionManager.Add("Player");
        PlayerShip = CreateSpaceShip("Player Ship", JVector.UnitX * 100, JVector.Zero);
        PlayerFaction.OwnedBodies.Add(PlayerShip);
    }

    public SpaceShip CreateSpaceShip(string name, JVector position, JVector lookAt)
    {
        var ship = Prefabs.GetShip("Basic Ship");
        ship.Name = name;
        ship.Sector = Sector;
        ship.Physics.Position = position;
        ship.Physics.LookAt(lookAt, JVector.UnitY);
        return ship;
    }

}