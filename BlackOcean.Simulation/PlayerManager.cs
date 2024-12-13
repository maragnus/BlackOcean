using System.Collections;
using BlackOcean.Simulation.ControlPanels;

namespace BlackOcean.Simulation;

public class PlayerManager(Game game) : IEnumerable<Player>
{
    private Game Game { get; } = game;
    private readonly Dictionary<string, Player> _players = [];

    public int Count => _players.Count;

    public Player GetPlayer(string name)
    {
        lock (this)
        {
            if (_players.TryGetValue(name, out var player))
                return player;
            return Game.Scenario.CreatePlayer(name);
        }
    }

    public void AddPlayer(Player player)
    {
        lock (this)
        {
            _players.Add(player.Name, player);
        }
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