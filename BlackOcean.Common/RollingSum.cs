namespace BlackOcean.Common;

using System.Collections.Generic;

public class RollingSum
{
    private readonly Queue<(double Value, double Timestamp)> _values = new(12);
    private double _sum;

    /// <summary>
    /// Adds a new value with the elapsed time and updates the moving average.
    /// </summary>
    /// <param name="value">The new value to add.</param>
    /// <param name="elapsedTime">The elapsed time (in seconds).</param>
    public double AddValue(double value, double elapsedTime)
    {
        // Add the new value
        _values.Enqueue((value, elapsedTime));
        _sum += value;

        // Remove values older than 1 second
        var purgeTime = elapsedTime - 1.0;
        while (_values.Count > 0 && _values.Peek().Timestamp < purgeTime)
        {
            var old = _values.Dequeue();
            _sum -= old.Value;
        }
        
        return _sum;
    }
}
