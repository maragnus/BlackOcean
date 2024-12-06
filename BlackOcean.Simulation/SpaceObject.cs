namespace BlackOcean.Simulation;

public abstract class SpaceBody(string name)
{
    private Sector? _sector;
    private ISpaceBodyCollection? SpaceBodyCollection => _sector;
    public string Name { get; set; } = name;

    public IPhysics Physics { get; private set; } = new SimplePhysics();

    public Sector? Sector
    {
        get => _sector;
        set
        {
            if (ReferenceEquals(_sector, value)) return;
            SetSimulationLevel(SimulationLevel.Simple);
            SpaceBodyCollection?.Remove(this);
            _sector = value;
            SpaceBodyCollection?.Add(this);
        }
    }

    public void Destroy()
    {
        _sector = null;
    }
    
    public void SetSimulationLevel(SimulationLevel level)
    {
        if (level == Physics.SimulationLevel) return;

        var world = SpaceBodyCollection?.World;
        
        // Only change to full if the sector has a Physics World
        if (level == SimulationLevel.Full && world is not null)
            Physics = new FullPhysics(world, Physics);
        
        if (Physics.SimulationLevel != SimulationLevel.Simple)
            Physics = new SimplePhysics(Physics);
    }

    public virtual void Simulate(SimulationContext context)
    {
        if (Physics.SimulationLevel == SimulationLevel.Simple && Physics is SimplePhysics physics)
        {
            physics.Simulate(context.DeltaTime);
        }
    }
}