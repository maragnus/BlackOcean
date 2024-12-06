namespace BlackOcean.Simulation.Definitions;

public record BiologicConsumption(string Name, 
    // Calories per day
    int Calories,
    // Liters of water consumed per day
    double Water,
    // Liters of oxygen at 200 bar consumed per day
    double Oxygen);

public record RoboticConsumption(string Name, 
    // Energy consumption per day in kilowatt-hours
    double Energy,
    // Lubricants consumed per day in liters
    double Lubricants);

public static class Consumptions
{
    public static readonly BiologicConsumption HumanYouth = new("Youth", 1600, 2.5, 1.2);
    public static readonly BiologicConsumption HumanAdolescent = new("Adolescent", 2400, 3.3, 1.8);
    public static readonly BiologicConsumption HumanSedentary = new("Sedentary", 2000, 2.5, 1.6);
    public static readonly BiologicConsumption HumanLaborer = new("Laborer", 2800, 4.7, 2.2);

    public static readonly BiologicConsumption SmallPuppy = new("Small Puppy", 400, 0.6, 0.3);
    public static readonly BiologicConsumption SmallDog = new("Small Dog", 450, 0.6, 0.4);
    public static readonly BiologicConsumption MediumPuppy = new("Medium Puppy", 800, 1.2, 0.5);
    public static readonly BiologicConsumption MediumDog = new("Medium Dog", 1000, 1.0, 0.8);
    public static readonly BiologicConsumption LargePuppy = new("Large Puppy", 1200, 1.8, 0.8);
    public static readonly BiologicConsumption LargeDog = new("Large Dog", 2000, 2.0, 1.5);
    public static readonly BiologicConsumption Kitten = new("Kitten", 250, 0.1, 0.1);
    public static readonly BiologicConsumption Cat = new("Cat", 250, 0.2, 0.2);
    
    public static readonly RoboticConsumption CleaningRobot = new("Cleaning Robot", 2.0, 0.01);
    public static readonly RoboticConsumption CargoRobot = new("Cargo Robot", 10.0, 0.05);
    public static readonly RoboticConsumption ConstructionRobot = new("Construction Robot", 20.0, 0.1);
    public static readonly RoboticConsumption MiningRobot = new("Mining Robot", 50.0, 0.2);
    public static readonly RoboticConsumption MaintenanceRobot = new("Maintenance Robot", 5.0, 0.02);
    public static readonly RoboticConsumption SurveyingRobot = new("Surveying Robot", 3.0, 0.01);
    public static readonly RoboticConsumption ScientificRobot = new("Scientific Robot", 4.0, 0.01);
    public static readonly RoboticConsumption SecurityRobot = new("Security Robot", 8.0, 0.03);
}