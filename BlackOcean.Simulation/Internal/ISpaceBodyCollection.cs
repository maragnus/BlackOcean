namespace BlackOcean.Simulation;

internal interface ISpaceBodyCollection
{
    World? World { get; }
    void Add(SpaceBody body);
    void Remove(SpaceBody body);
}