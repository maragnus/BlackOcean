using Jitter2.Dynamics;

namespace BlackOcean.Simulation;

public interface IPhysics
{
    JVector Position { get; set; }
    JQuaternion Orientation { get; set; }
    JVector LinearVelocity { get; set; }
    JVector AngularVelocity { get; set; }
    JVector LinearAcceleration { get; set; }
    JVector AngularAcceleration { get; set; }
    double Mass { get; set; }
    double Radius { get; set; }
    SimulationLevel SimulationLevel { get; }
}

public class FullPhysics : IPhysics
{
    public SimulationLevel SimulationLevel => SimulationLevel.Full;
    
    public FullPhysics(World world, IPhysics physics)
    {
        RigidBody = world.CreateRigidBody();
        RigidBody.AffectedByGravity = false;
        Position = physics.Position;
        Orientation = physics.Orientation;
        LinearVelocity = physics.LinearVelocity;
        AngularVelocity = physics.AngularVelocity;
        LinearAcceleration = physics.LinearAcceleration;
        AngularAcceleration = physics.AngularAcceleration;
        Mass = physics.Mass;
        Radius = physics.Radius;
    }

    public RigidBody RigidBody { get; }

    public JVector Position
    {
        get => RigidBody.Position;
        set => RigidBody.Position = value;
    }

    public JQuaternion Orientation
    {
        get => RigidBody.Orientation;
        set => RigidBody.Orientation = value;
    }

    public JVector LinearVelocity
    {
        get => RigidBody.Velocity;
        set => RigidBody.Velocity = value;
    }

    public JVector AngularVelocity
    {
        get => RigidBody.AngularVelocity;
        set => RigidBody.AngularVelocity = value;
    }

    public JVector LinearAcceleration
    {
        get => RigidBody.Force;
        set => RigidBody.Force = value;
    }

    public JVector AngularAcceleration
    {
        get => RigidBody.Torque;
        set => RigidBody.Torque = value;
    }

    public double Mass
    {
        get => RigidBody.Mass;
        set => RigidBody.SetMassInertia(value);
    }

    public double Radius { get; set; }
}

public class SimplePhysics : IPhysics
{
    public SimplePhysics()
    {
        
    }
    
    public SimulationLevel SimulationLevel => SimulationLevel.Simple;

    public SimplePhysics(IPhysics physics)
    {
        Position = physics.Position;
        Orientation = physics.Orientation;
        LinearVelocity = physics.LinearVelocity;
        AngularVelocity = physics.AngularVelocity;
        LinearAcceleration = physics.LinearAcceleration;
        AngularAcceleration = physics.AngularAcceleration;
        Mass = physics.Mass;
        Radius = physics.Radius;
    }
    
    public JVector Position { get; set; }
    public JQuaternion Orientation { get; set; }
    public JVector LinearVelocity { get; set; }
    public JVector AngularVelocity { get; set; }
    public JVector LinearAcceleration { get; set; }
    public JVector AngularAcceleration { get; set; }
    public double Mass { get; set; } = 1.0;
    public double Radius { get; set; } = 1.0;

    public void Simulate(double deltaTime)
    {
        Position += LinearVelocity * deltaTime;
        Orientation += Orientation * deltaTime;
        LinearVelocity += LinearAcceleration * deltaTime;
        AngularVelocity += AngularAcceleration * deltaTime;
    }
}