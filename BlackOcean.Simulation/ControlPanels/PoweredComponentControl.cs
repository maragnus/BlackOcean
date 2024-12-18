using BlackOcean.Simulation.ShipSystems;

namespace BlackOcean.Simulation.ControlPanels;

[PublicAPI]
public class PoweredComponentControl
{
    public string Name = "";
    public string Abbreviation = "";
    public string Icon = "";
    public bool Available { get; set; }
    public bool Operating { get; set; }
    public int MinLevel = 1;
    public int MaxLevel = 5;
    public int SetLevel { get; set; } = 1;
    public double CurrentLevel = 0;
    public Status[] LevelStatuses = [ Status.Safe, Status.Safe, Status.Safe, Status.Warn, Status.Danger ];
    public double? CurrentHeat { get; set; }
    public Band[]? HeatBands { get; set; }
    public double CurrentOutput { get; set; }
    public double NominalOutput { get; set; }

    public Button Powered { get; set; } = new()
    {
        Name = "On",
        Available = false,
        Icon = "fal fa-power-off",
        Toggle = true,
        Pressed = false
    };
    
    public Button Auto { get; set; } = new()
    {
        Name = "Auto",
        Available = false,
        Icon = "fal fa-microchip-ai",
        Toggle = true,
        Pressed = false
    };

    private static readonly Band[] DefaultHeatBands = [Band.Default];
}