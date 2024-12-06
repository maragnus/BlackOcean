namespace BlackOcean.Simulation.ShipSystems;

public class Meter
{
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "none";
    public double Min { get; set; }
    public double Max { get; set; }
    public double Value { get; set; }
    public Band[] Bands { get; set; } = [Band.Default];
}