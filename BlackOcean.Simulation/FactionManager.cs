using System.Collections;

namespace BlackOcean.Simulation;

public class FactionManager(Game game) : ISimulated, IReadOnlyCollection<Faction>
{
    public Game Game { get; } = game;
    private HashSet<Faction> _factions = [];
    
    public void Simulate(SimulationContext context)
    {
        foreach (var faction in _factions)
            faction.Simulate(context);
    }

    public IEnumerator<Faction> GetEnumerator()=> _factions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _factions.GetEnumerator();
    public Faction Add(string name)
    {
        var faction = new Faction(name);
        _factions.Add(faction);
        return faction;
    }
    public void Clear() => _factions.Clear();
    public bool Contains(Faction item) => _factions.Contains(item);
    public void CopyTo(Faction[] array, int arrayIndex) => _factions.CopyTo(array, arrayIndex);
    public bool Remove(Faction item) => _factions.Remove(item);
    public int Count => _factions.Count;
    public bool IsReadOnly => false;
}