using System.Collections;
using BlackOcean.Common;
using BlackOcean.Simulation.ControlPanels;
using BlackOcean.Simulation.ShipSystems;

namespace BlackOcean.Simulation;

public class PlayerManager(Game game) : IEnumerable<Player>
{
    private Game Game { get; } = game;
    private readonly Dictionary<string, Player> _players = [];

    public int Count => _players.Count;

    public async Task<Player> GetPlayer(string name)
    {
        return await Game.Execute(() => 
            _players.GetOrAdd(name, key => Game.Scenario.CreatePlayer(key)));
    }

    public IEnumerator<Player> GetEnumerator() => _players.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class Player
{
    public required string Name { get; set; }
    public required Faction Faction { get; set; }
    public required SpaceShip SpaceShip { get; set; }
    public required ControlPanel ControlPanel { get; set; }
}