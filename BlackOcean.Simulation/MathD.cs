namespace BlackOcean.Simulation;

public static class MathD
{
    public static double Approach(double currentValue, double approachValue, double increment)
    {
        // Calculate the difference between the current and target value
        var difference = approachValue - currentValue;

        // If the difference is smaller than the increment, move directly to the target
        if (Math.Abs(difference) <= increment)
        {
            return approachValue;
        }

        // Move towards the target by the increment amount, maintaining direction
        return currentValue + Math.Sign(difference) * increment;
    }
}