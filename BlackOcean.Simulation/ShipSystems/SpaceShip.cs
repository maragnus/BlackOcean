using BlackOcean.Simulation.Definitions;

namespace BlackOcean.Simulation.ShipSystems;

public class SpaceShip(string name) : SystemizedBody(name)
{
    public ShipHull Hull { get; set; } = ShipHull.EmptyHull;

    public void PowerOn()
    {
        foreach (var system in Components.OfType<PoweredShipComponent>())
        {
            system.TargetPowerLevel = PowerLevel.Standard;
            system.Shutdown = false;
        }
    }
    
    public void Refuel(double percent = 1.0)
    {
        // Fill the fuel tanks
        foreach (var generator in Components.OfType<EnergyGenerator>())
            FillStorage(generator.Fuel, percent);

        // Fill batteries
        FillStorage(Materials.Electricity, percent);
        
        // Fill shields
        FillStorage(Materials.ShieldEnergy, percent);
        FillStorage(Materials.AblativeShields, percent);
    }

    public void FillStorage(Material material, double percent)
    {
        foreach (var storage in Components.OfType<StorageComponent>().Where(x=>x.Material == material))
            storage.Storage.Supply(storage.Storage.Available * percent);
    }
}