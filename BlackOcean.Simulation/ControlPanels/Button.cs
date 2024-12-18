using BlackOcean.Simulation.ShipSystems;

namespace BlackOcean.Simulation.ControlPanels;

[PublicAPI]
public class Button
{
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public bool Pressed { get; set; }
    public bool Available { get; set; }
    public bool Toggle { get; set; }

    public void Populate(CoolingComponent? coolingSystem)
    {
        Toggle = true;

        if (coolingSystem is null)
        {
            Name = "Not Installed";
            Available = false;
            Pressed = false;
            return;
        }
        
        Name = coolingSystem.Name;
        Available = true;
        Pressed = coolingSystem.IsOpen;
    }
    
    public void Populate(StorageComponent? storageSystem)
    {
        Toggle = true;

        if (storageSystem is null)
        {
            Name = "Not Installed";
            Available = false;
            Pressed = false;
            return;
        }
        
        Name = storageSystem.Name;
        Available = true;
        Pressed = storageSystem.Storage.IsEnabled;
    }
}