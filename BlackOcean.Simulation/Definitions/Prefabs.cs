using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlackOcean.Simulation.ShipSystems;

// ReSharper disable ClassNeverInstantiated.Global

namespace BlackOcean.Simulation.Definitions
{
    public static class Prefabs
    {
        private const string PrefabFileName = "BlackOcean.Simulation.Definitions.ShipSystems.json";

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() },
            AllowTrailingCommas = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = true
        };
        
        static Prefabs()
        {
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(PrefabFileName)!;
            var prefabs = JsonSerializer.Deserialize<PrefabModels.PrefabFile>(stream, JsonSerializerOptions)!;
            Systems = new SystemCollection(prefabs);
        }

        internal static SystemCollection Systems { get; }

        public static StorageSystem GetStorage(string name) => Systems.Storages[name].Build();
        public static EnergyGenerator GetGenerator(string name) => Systems.Generators[name].Build();
        public static ShieldGenerator GetShield(string name) => Systems.Shields[name].Build();
        public static CoolingSystem GetCooling(string name) => Systems.Cooling[name].Build();
        public static SpaceShip GetShip(string name) => Systems.Ships[name].Build();
        public static ShipHull GetHull(string name) => Systems.Hulls[name].Build();
    }

    internal class SystemCollection(PrefabModels.PrefabFile collection)
    {
        public Dictionary<string, PrefabModels.Storage> Storages { get; } = collection.Storages.ToDictionary(x => x.Name);
        public Dictionary<string, PrefabModels.Energy> Generators { get; } = collection.Generators.ToDictionary(x => x.Name);
        public Dictionary<string, PrefabModels.Shield> Shields { get; } = collection.Shields.ToDictionary(x => x.Name);
        public Dictionary<string, PrefabModels.Cooling> Cooling { get; } = collection.Cooling.ToDictionary(x => x.Name);
        public Dictionary<string, PrefabModels.Ship> Ships { get; } = collection.Ships.ToDictionary(x => x.Name);
        public Dictionary<string, PrefabModels.Hull> Hulls { get; } = collection.Hulls.ToDictionary(x => x.Name);
    };
    
    namespace PrefabModels
    {
        internal record PrefabFile(
            Storage[] Storages,
            Energy[] Generators,
            Shield[] Shields,
            Cooling[] Cooling,
            Ship[] Ships,
            Hull[] Hulls
        );

        internal record Storage(string Name, string Material, int Capacity, double? TransferRate)
        {
            public StorageSystem Build() => new PrefabStorageSystem(Name, Materials.AllMaterials[Material], Capacity, TransferRate ?? Capacity);
        }

        internal record Energy(string Name, string Material, double Efficiency, double Output, double ThermalLimit)
        {
            public EnergyGenerator Build() => new PrefabEnergyGenerator(Name, Materials.AllMaterials[Material], Efficiency, Output, ThermalLimit);
        }

        internal record Shield(string Name, double Efficiency, double Output, double ThermalLimit, ShieldType ShieldType)
        {
            public ShieldGenerator Build() => new PrefabShieldGenerator(Name, Efficiency, Output, ThermalLimit, ShieldType);
        }

        internal record Cooling(string Name, double Output)
        {
            public CoolingSystem Build() => new PrefabCoolingSystem(Name, Output);
        }
        
        internal record Hull(string Name, double Mass)
        {
            public ShipHull Build() => new PrefabShipHull(Name, Mass);
        }
        
        internal record Ship(string Name, string Hull, string[] Storages, string[] Generators, string[] Shields, string[] Cooling)
        {
            public SpaceShip Build()
            {
                var ship = new SpaceShip(Name);
                ship.Hull = Prefabs.GetHull(Hull);
                ship.AddSystems(Storages.Select(name => Prefabs.GetStorage(name)));
                ship.AddSystems(Generators.Select(name => Prefabs.GetGenerator(name)));
                ship.AddSystems(Shields.Select(name => Prefabs.GetShield(name)));
                ship.AddSystems(Cooling.Select(name => Prefabs.GetCooling(name)));
                return ship;
            }
        }
    }
}
