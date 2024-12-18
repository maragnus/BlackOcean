using BlackOcean.Simulation.ShipSystems;

namespace BlackOcean.Simulation.ControlPanels;

[PublicAPI]
public enum Scale
{
    Linear,
    Exp,
    Log,
}

[PublicAPI]
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

    public void Populate(StorageComponent? storageSystem)
    {
        Name = $"{storageSystem?.Material.Abbreviation ?? "N/A"} Store";
        Max = storageSystem?.Capacity ?? 100;
        Value = storageSystem?.Storage.Amount ?? 0;
    }
    
    public void Populate(List<CoolingComponent> coolingSystems)
    {
        Max = coolingSystems.DefaultIfEmpty().Sum(x => x?.BaseConsumption ?? 0);
        Value = coolingSystems.DefaultIfEmpty().Sum(x => x?.CurrentConsumption ?? 0);
    }

    public void Configure(double min, double max, bool rising, double lowPercentage, double highPercentage)
    {
        Min = min;
        Max = max;
        var total = max - min;
        var low = lowPercentage * total + min;
        var high = highPercentage * total + min;
        if (rising)
            Bands = Band.Build(Status.Safe, (low, Status.Warn), (high, Status.Danger));
        else
            Bands = Band.Build(Status.Danger, (low, Status.Warn), (high, Status.Safe));
    }
}