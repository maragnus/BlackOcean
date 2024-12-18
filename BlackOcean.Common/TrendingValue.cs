namespace BlackOcean.Common;

public class TrendingValue
{
    private readonly Queue<(double Value, double Timestamp)> _values = new(12);
    private (double Value, double Timestamp) _lastValue = (0, -1);
    
    /// <summary>
    /// Adds a new value with the elapsed time and updates the trend tracking.
    /// </summary>
    /// <param name="value">The current value to track.</param>
    /// <param name="elapsedTime">The elapsed time (in seconds).</param>
    public double AddValue(double value, double elapsedTime)
    {
        // Add the new value
        _lastValue = (value, elapsedTime);
        _values.Enqueue(_lastValue);

        // Remove values older than 1 second
        var purgeTime = elapsedTime - 1.0;
        while (_values.Count > 0 && _values.Peek().Timestamp < purgeTime)
            _values.Dequeue();

        return GetTrend();
    }

    /// <summary>
    /// Gets the trend as the rate of change over the last second.
    /// </summary>
    /// <returns>The trend (change per second).</returns>
    public double GetTrend()
    {
        if (_values.Count < 2) return 0; // Not enough data points to calculate trend

        // Get the first and last items in the queue
        var first = _values.Peek();
        var last = _lastValue;

        // Calculate the rate of change
        var timeDifference = last.Timestamp - first.Timestamp;
        if (timeDifference == 0) return 0; // Avoid division by zero

        return (last.Value - first.Value) / timeDifference;
    }
}