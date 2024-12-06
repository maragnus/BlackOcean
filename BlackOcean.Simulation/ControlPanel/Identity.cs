namespace BlackOcean.Simulation.ShipSystems;

public class Identity
{
    public string? ShipName { get; set; }
    public string? CallSign { get; set; }
    public string? OsIdentifier { get; set; }
    public double Velocity { get; set; }
    public double Heading { get; set; }
    public double Mark { get; set; }
    public double VerticalG { get; set; }
    public double LateralG { get; set; }
    public double InteriorRadiation { get; set; }
    public double ExteriorRadiation { get; set; }
}