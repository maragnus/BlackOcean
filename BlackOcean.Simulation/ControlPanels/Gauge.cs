using BlackOcean.Simulation.ShipSystems;

namespace BlackOcean.Simulation.ControlPanels;

public enum Scale
{
    Linear,
    Logarithmic,
}

public class Gauge
{
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "none";
    public string? Interval { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double Value { get; set; }
    public Band[] Bands { get; set; } = [Band.Default];
    public Scale Scale { get; set; } = Scale.Linear;

    public void Populate(StorageSystem? storageSystem)
    {
        Name = $"{storageSystem?.Material.Abbreviation ?? "N/A"} Store";
        Max = storageSystem?.Capacity ?? 0;
        Value = storageSystem?.Amount ?? 0;
    }
}