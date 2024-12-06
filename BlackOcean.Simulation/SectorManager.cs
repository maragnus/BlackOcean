using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace BlackOcean.Simulation;

public class SectorManager : ISimulated, IReadOnlyDictionary<string, Sector>
{
    private Dictionary<string, Sector> _sectors;

    public SectorManager()
    {
        _sectors = new Dictionary<string, Sector>();
    }

    public void Simulate(SimulationContext context)
    {
        foreach (var sector in _sectors.Values)
            sector.Simulate(context);
    }

    public Sector this[string key] => _sectors[key];

    public Sector Add(string sectorId, JVector position)
    {
        var sector = new Sector(this, sectorId, position);
        _sectors.Add(sectorId, sector);
        return sector;
    }

    public IEnumerable<string> Keys => _sectors.Keys;
    public IEnumerable<Sector> Values => _sectors.Values;
    public IEnumerator<KeyValuePair<string, Sector>> GetEnumerator() => _sectors.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _sectors.GetEnumerator();
    public void Clear() => _sectors.Clear();
    public bool Contains(KeyValuePair<string, Sector> item) => _sectors.Contains(item);
    public void CopyTo(KeyValuePair<string, Sector>[] array, int arrayIndex) => _sectors.ToList().CopyTo(array, arrayIndex);
    public int Count => _sectors.Count;
    public bool IsReadOnly => false;
    public bool ContainsKey(string key) => _sectors.ContainsKey(key);
    public bool Remove(string key) => _sectors.Remove(key);
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Sector value) => _sectors.TryGetValue(key, out value);
}

public class Sector : IEquatable<Sector>, ISimulated, IReadOnlyCollection<SpaceBody>, ISpaceBodyCollection
{
    public SectorManager Manager { get; }
    public string SectorId { get; }
    public bool IsActive { get; set; }
    public World? World { get; private set; }
    public SimulationLevel SimulationLevel { get; set; }
    public JVector Position { get; }

    private HashSet<SpaceBody> _bodies = [];

    public Sector(SectorManager manager, string id, JVector position)
    {
        Manager = manager;
        SectorId = id;
        Position = position;
    }


    private void ActivateFullSimulation()
    {
        if (World is not null) return;

        World = new World();
        foreach (var body in _bodies)
            body.SetSimulationLevel(SimulationLevel.Full);
    }

    private void DeactivateFullSimulation()
    {
        if (World is null) return;
        
        foreach (var controllable in _bodies)
            controllable.SetSimulationLevel(SimulationLevel.Simple);
        
        World.Clear();
        World = null;
    }
    
    void ISpaceBodyCollection.Add(SpaceBody body)
    {
        _bodies.Add(body);
        body.SetSimulationLevel(SimulationLevel);
    }

    void ISpaceBodyCollection.Remove(SpaceBody body) => _bodies.Remove(body);
    public IEnumerator<SpaceBody> GetEnumerator() => _bodies.GetEnumerator();
    public int Count => _bodies.Count;
    IEnumerator IEnumerable.GetEnumerator() => _bodies.GetEnumerator();

    public void Simulate(SimulationContext context)
    {
        if (SimulationLevel == SimulationLevel.Full && World is null)
            ActivateFullSimulation();
        else if (SimulationLevel == SimulationLevel.Simple && World is not null)
            DeactivateFullSimulation();

        foreach (var body in _bodies)
            body.Simulate(context);

        World?.Step(context.DeltaTime);
    }
    
    public bool Equals(Sector? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return SectorId == other.SectorId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Sector)obj);
    }

    public override int GetHashCode() => SectorId.GetHashCode();
}
