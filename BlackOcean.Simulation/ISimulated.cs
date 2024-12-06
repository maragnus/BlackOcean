using Jitter2.Dynamics;

namespace BlackOcean.Simulation;

public interface ISimulated
{
    void Simulate(SimulationContext context);
}

public enum SimulationLevel {
    Full,
    Simple
}
