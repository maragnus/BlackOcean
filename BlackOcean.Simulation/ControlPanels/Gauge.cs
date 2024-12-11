namespace BlackOcean.Simulation.ControlPanels;

public class Gauge
{
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "none";
    public double Min { get; set; }
    public double Max { get; set; }
    public double Value { get; set; }
    public Band[] Bands { get; set; } = [Band.Default];
}