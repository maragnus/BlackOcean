using System.Reflection;

namespace BlackOcean.Simulation.Definitions;

using static MaterialClassification;

public record Material(
    MaterialClassification Classification,
    string Name,
    string Abbreviation,
    // Weight in grams per liter (g/L)
    double Mass,
    // Fuel: Energy density in megajoules per kilogram (MJ/g)
    // Food: Calories per liter (cal/L)
    double? EnergyDensity = null)
{
    public double MegaJoulesPerLiter { get; } = Mass * EnergyDensity ?? 0;
    
    public virtual bool Equals(Material? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}

public enum MaterialClassification
{
    Unclassified,
    Energy,
    // Stored in liters
    Solid,
    // Stored in liters at 200 bar
    Gas,
    // Stored in liters
    Liquid
}

public static class Materials
{
    public const double Epsilon = 0.0000001; // Smallest acceptable unit of material

    public static readonly IReadOnlyDictionary<string, Material> AllMaterials;

    static Materials()
    {
        AllMaterials =
            typeof(Materials)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(field => field.FieldType == typeof(Material))
                .Select(f => (Material)f.GetValue(null)!)
                .ToDictionary(material => material.Name)
                .AsReadOnly();
    }

    public static readonly Material None = new(Unclassified, "None", "None", 0.0);

    // Electrical energy in megajoules
    public static readonly Material Electricity = new(Energy, "Electricity", "E", 0.0);

    // Heat energy in megajoules
    public static readonly Material Heat = new(Energy, "Heat", "HEAT", 0.0);

    // Unregulated Solar Energy
    public static readonly Material SolarEnergy = new(Energy, "Solar Energy", "SLR", 0.0);

    // High-voltage electromagnetic shield energy
    public static readonly Material ShieldEnergy = new(Energy, "Shield Energy", "SHLD", 0.0);

    // Liquid ablative shield coating
    public static readonly Material AblativeShields = new(Liquid, "Ablative Shielding", "ASHD", 0.0);

    // Water (H2O)
    public static readonly Material Water = new(Liquid, "Water", "H2O", 1000.0);

    // Oxygen (O2)
    public static readonly Material Oxygen = new(Gas, "Oxygen", "O2", 260.0);

    // Nitrogen (N2)
    public static readonly Material Nitrogen = new(Gas, "Nitrogen", "N2", 233.0);

    // Carbon Dioxide (CO2)
    public static readonly Material CarbonDioxide = new(Gas, "Carbon Dioxide", "CO2", 350.0);

    // Hydrogen Gas (H2)
    public static readonly Material HydrogenGas = new(Gas, "Hydrogen Gas", "H2", 14.0, 120.0);

    // Liquid Hydrogen (LH2)
    // Density: ~70.85 grams per liter
    public static readonly Material LiquidHydrogen = new(Liquid, "Liquid Hydrogen", "LH2", 70.85, 120.0);

    // Methane (CH4)
    public static readonly Material Methane = new(Gas, "Methane", "CH4", 128.0, 55.5);

    // Helium (He)
    public static readonly Material Helium = new(Gas, "Helium", "He", 31.0);

    // Helium-3 (He-3)
    // Density at 200 bar: ~26.8 grams per liter
    public static readonly Material Helium3 = new(Gas, "Helium-3", "He-3", 26.8, 630000.0);

    // Deuterium (D2)
    // Density at 200 bar: ~36 grams per liter
    public static readonly Material Deuterium = new(Gas, "Deuterium", "D2", 36.0, 576000.0);

    // Ammonia (NH3)
    public static readonly Material Ammonia = new(Liquid, "Ammonia", "NH3", 682.0, 0.0186);

    // Uranium (U)
    public static readonly Material Uranium = new(Solid, "Uranium", "U", 19050.0, 144000.0);

    // Thorium (Th)
    public static readonly Material Thorium = new(Solid, "Thorium", "Th", 11700.0, 79900.0);

    // Antimatter (AM)
    public static readonly Material Antimatter = new(Energy, "Antimatter", "AM", 70.0, 89875517.874);

    // Food
    public static readonly Material Proteins = new(Liquid, "Proteins", "MNP", 600.0, 4400.0);
    public static readonly Material Carbohydrates = new(Liquid, "Carbohydrates", "MNC", 600.0, 5200.0);
    public static readonly Material Fats = new(Liquid, "Fats", "MNF", 600.0, 8100.0);
    public static readonly Material Micronutrients = new(Liquid, "Micronutrients", "MNV", 600.0, 100.0);
    public static readonly Material Meals = new(Solid, "Meals", "MNV", 800.0, 1200.0);

    // Soil
    public static readonly Material Soil = new(Solid, "Soil", "Soil", 1200.0);

    // Fertilizer
    public static readonly Material Fertilizer = new(Solid, "Fertilizer", "Fert", 1000.0);

    // Aluminum (Al)
    public static readonly Material Aluminum = new(Solid, "Aluminum", "Al", 2700.0);

    // Iron (Fe)
    public static readonly Material Iron = new(Solid, "Iron", "Fe", 7874.0);

    // Silicon (Si)
    public static readonly Material Silicon = new(Solid, "Silicon", "Si", 2330.0);
}