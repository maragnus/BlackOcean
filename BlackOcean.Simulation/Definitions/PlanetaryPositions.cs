using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace BlackOcean.Simulation.Definitions;

public enum PlanetName
{
    Mercury,
    Venus,
    Earth,
    Mars,
    Jupiter,
    Saturn,
    Uranus,
    Neptune,
}

public enum MoonName
{
    Moon,
    Phobos,
    Deimos,
    Io,
    Europa,
    Ganymede,
    Callisto,
    Mimas,
    Enceladus,
    Tethys,
    Dione,
    Rhea,
    Titan,
    Iapetus,
    Miranda,
    Ariel,
    Umbriel,
    Titania,
    Oberon,
    Triton,
    Nereid
}

public class PlanetaryPosition
{
    public required PlanetName Name { get; init; }
    public required double Radius { get; init; }
    public required JVector Position { get; init; }
    public required IReadOnlyCollection<MoonPosition> Moons { get; init; }
}

public class MoonPosition
{
    public required MoonName Name { get; init; }
    public required double Radius { get; init; }
    public required JVector Position { get; init; }
}

public class PlanetaryPositions(Dictionary<PlanetName,PlanetaryPosition> planets) : IReadOnlyDictionary<PlanetName, PlanetaryPosition>
{
    private readonly record struct PlanetaryData(
        PlanetName Name,
        double r,
        // Semi-major axis (AU)
        double a,
        // Eccentricity
        double e,
        // Inclination
        double i,
        // Mean longitude
        double L,
        // Argument of periapsis
        double w,
        // Longitude of ascending node
        double N)
    {
        public JVector CalculatePosition(DateTime date)
        {
            // Constants
            const double degreesToRadians = Math.PI / 180.0;
            const double radiansToDegrees = 180.0 / Math.PI;
            const double auToKilometers = 1.496e+8;

            // Time in Julian centuries since J2000
            var jd = (date - new DateTime(2000, 1, 1, 12, 0, 0)).TotalDays + 2451545.0;
            var t = (jd - 2451545.0) / 36525.0;

            // Orbital parameters (adjusted for time)
            var i = this.i * degreesToRadians; 
            var L = (this.L + 36000.76983 * t) % 360; 
            var w = this.w; 
            var N = this.N; 

            // Mean anomaly
            var M = (L - w) % 360;
            if (M < 0) M += 360;

            // Solve Kepler's Equation for eccentric anomaly (iterative method)
            var E = M * degreesToRadians; // Start with E ~ M
            for (var j = 0; j < 10; j++)
            {
                E = E - (E - e * Math.Sin(E) - M * degreesToRadians) / (1 - e * Math.Cos(E));
            }

            // True anomaly
            var v = 2 * Math.Atan2(Math.Sqrt(1 + e) * Math.Sin(E / 2), Math.Sqrt(1 - e) * Math.Cos(E / 2));
            v = v * radiansToDegrees;
            if (v < 0) v += 360;

            // Heliocentric distance
            var r = a * (1 - e * e) / (1 + e * Math.Cos(v * degreesToRadians));

            // Heliocentric coordinates in the orbital plane
            var xh = r * (Math.Cos(N * degreesToRadians) * Math.Cos((v + w) * degreesToRadians) -
                          Math.Sin(N * degreesToRadians) * Math.Sin((v + w) * degreesToRadians) * Math.Cos(i));
            var yh = r * (Math.Sin(N * degreesToRadians) * Math.Cos((v + w) * degreesToRadians) +
                          Math.Cos(N * degreesToRadians) * Math.Sin((v + w) * degreesToRadians) * Math.Cos(i));
            var zh = r * (Math.Sin((v + w) * degreesToRadians) * Math.Sin(i));

            return new JVector(xh * auToKilometers, yh * auToKilometers, zh * auToKilometers);
        }
    }

    // Orbital parameters for planets (simplified, epoch J2000)
    private readonly static PlanetaryData[] PlanetaryDataItems = {
        new(PlanetName.Mercury, 2440, 0.387, 0.205, 7.0, 252.250, 77.457, 48.330),
        new(PlanetName.Venus, 6052, 0.723, 0.007, 3.4, 181.979, 131.602, 76.679),
        new(PlanetName.Earth, 6371, 1.000, 0.017, 0.0, 100.464, 102.937, 11.260),
        new(PlanetName.Mars, 3390, 1.524, 0.093, 1.85, 355.453, 336.040, 49.558),
        new(PlanetName.Jupiter, 69911, 5.203, 0.048, 1.3, 34.404, 14.753, 100.556),
        new(PlanetName.Saturn, 58232, 9.537, 0.054, 2.5, 49.944, 92.431, 113.715),
        new(PlanetName.Uranus, 25362, 19.191, 0.047, 0.8, 313.232, 170.964, 74.229),
        new(PlanetName.Neptune, 24622, 30.069, 0.009, 1.8, -55.120, 44.971, 131.721)
    };

    private readonly record struct MoonData(MoonName Name, PlanetName Planet, double r, double a, double p)
    {
        public JVector CalculatePosition(DateTime date)
        {
            // Orbital calculations for the moon
            var t = (date - new DateTime(2000, 1, 1, 12, 0, 0)).TotalDays; // Days since J2000
            var meanAnomaly = (t / p * 360) % 360; // Mean anomaly in degrees
            var radians = meanAnomaly * Math.PI / 180.0;

            // Moon position in orbital plane (circular orbit assumed)
            var xm = a * Math.Cos(radians);
            var ym = a * Math.Sin(radians);

            return new JVector(xm, ym, 0); // Assuming negligible z-offset for moons
        }
    }

    private readonly static MoonData[] MoonDataItems =
    {
        new(MoonName.Moon, PlanetName.Earth, 1737, 384400, 27.321), // a in KM, p in days
        new(MoonName.Phobos, PlanetName.Mars, 11, 9376, 0.31891),
        new(MoonName.Deimos, PlanetName.Mars, 6, 23463, 1.263),
        new(MoonName.Io, PlanetName.Jupiter, 1822, 421800, 1.769),
        new(MoonName.Europa, PlanetName.Jupiter, 1561, 671100, 3.551),
        new(MoonName.Ganymede, PlanetName.Jupiter, 2631, 1070400, 7.155), // Largest moon
        new(MoonName.Callisto, PlanetName.Jupiter, 2410, 1882700, 16.689),
        new(MoonName.Mimas, PlanetName.Saturn, 198, 185520, 0.942),
        new(MoonName.Enceladus, PlanetName.Saturn, 252, 238020, 1.370),
        new(MoonName.Tethys, PlanetName.Saturn, 531, 294670, 1.888),
        new(MoonName.Dione, PlanetName.Saturn, 561, 377420, 2.737),
        new(MoonName.Rhea, PlanetName.Saturn, 763, 527070, 4.518),
        new(MoonName.Titan, PlanetName.Saturn, 2574, 1221870, 15.945),
        new(MoonName.Iapetus, PlanetName.Saturn, 734, 3560820, 79.321),
        new(MoonName.Miranda, PlanetName.Uranus, 236, 129900, 1.413),
        new(MoonName.Ariel, PlanetName.Uranus, 579, 190900, 2.520),
        new(MoonName.Umbriel, PlanetName.Uranus, 584, 266000, 4.144),
        new(MoonName.Titania, PlanetName.Uranus, 789, 436300, 8.706),
        new(MoonName.Oberon, PlanetName.Uranus, 761, 583500, 13.463),
        new(MoonName.Triton, PlanetName.Neptune, 1353, 354800, -5.8), // Retrograde orbit, negative period
        new(MoonName.Nereid, PlanetName.Neptune, 170, 5513813, 360.13),
    };
    
    public static PlanetaryPositions GetPlanetPositions(DateTime date)
    {
        Dictionary<PlanetName, PlanetaryPosition> positions = new();
        foreach (var planetData in PlanetaryDataItems)
        {
            positions.Add(planetData.Name, new PlanetaryPosition()
            {
                Name = planetData.Name,
                Radius = planetData.r,
                Position = planetData.CalculatePosition(date),
                Moons = MoonDataItems
                    .Where(m => m.Planet == planetData.Name)
                    .Select(moon => new MoonPosition{ Name = moon.Name, Radius = moon.r, Position = moon.CalculatePosition(date)}).ToArray()
            });
        }
        return new PlanetaryPositions(positions);
    }

    private Dictionary<PlanetName, PlanetaryPosition> _positions = planets;
    public IEnumerator<KeyValuePair<PlanetName, PlanetaryPosition>> GetEnumerator() => _positions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _positions.GetEnumerator();
    public int Count => _positions.Count;
    public bool ContainsKey(PlanetName key) => _positions.ContainsKey(key);

    public bool TryGetValue(PlanetName key, [MaybeNullWhen(false)] out PlanetaryPosition value) =>
        _positions.TryGetValue(key, out value);

    public PlanetaryPosition this[PlanetName key] => _positions[key];
    public IEnumerable<PlanetName> Keys => _positions.Keys;
    public IEnumerable<PlanetaryPosition> Values => _positions.Values;
}