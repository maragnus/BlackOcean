using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation.Scenarios;

public class SolarSystemScenario : Scenario
{
    public static DateTime StartTime = new(2024, 4, 25);

    public Sector StartingSector { get; private set; } = default!;
    
    public override void Initialize()
    {
        var planetaryPositions = PlanetaryPositions.GetPlanetPositions(StartTime);
        foreach (var planetaryPosition in planetaryPositions.Values)
        {
            var sector = Game.SectorManager.Add(planetaryPosition.Name.ToString(), planetaryPosition.Position);
            var planet = new CelestialBody(planetaryPosition.Name.ToString());
            planet.Physics.Radius = planetaryPosition.Radius * 1_000;
            planet.Sector = sector;
            foreach (var moonPosition in planetaryPosition.Moons)
            {
                var moon = new CelestialBody(moonPosition.Name.ToString());
                moon.Physics.Position = moonPosition.Position;
                moon.Physics.Radius = moonPosition.Radius * 1_000;
                moon.Sector = sector;
            }
        }
        
        StartingSector = Game.SectorManager[PlanetName.Earth.ToString()];
    }
    
    public override Player CreatePlayer(string name) => CreatePlayer(StartingSector, name, "Basic Ship");
}