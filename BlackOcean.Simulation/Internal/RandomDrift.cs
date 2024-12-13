namespace BlackOcean.Simulation.Internal;

public class RandomDrift
{
    private readonly Random _random;

    private double _currentValue;
    private double _targetValue;
    private double _duration;

    private readonly double _minValue;
    private readonly double _maxValue;
    private readonly double _minSeconds;
    private readonly double _maxSeconds;

    private double _time;

    public double CurrentValue => _currentValue;
    
    public RandomDrift(double minValue, double maxValue, double minSeconds, double maxSeconds, double initialValue)
    {
        if (minValue >= maxValue) throw new ArgumentException("minValue must be less than maxValue.");
        if (minSeconds >= maxSeconds) throw new ArgumentException("minSeconds must be less than maxSeconds.");

        _random = new Random();

        _minValue = minValue;
        _maxValue = maxValue;
        _minSeconds = minSeconds;
        _maxSeconds = maxSeconds;
        _currentValue = initialValue;
        Start();
    }

    public void Update(double deltaTime)
    {
        // Increment elapsed time
        _time += deltaTime;

        // Check if interpolation is complete
        if (_time >= _duration)
            Start();
    
        // Smoothly interpolate towards the target value
        var progress = _time / _duration;
        _currentValue = EaseInOut(_currentValue, _targetValue, progress);
    }
    
    private void Start() 
    {
        _targetValue = GetRandomValue();
        _duration = GetRandomTime();
        _time = 0.0;
    }
    
    // Apply the ease-in-out cubic formula
    private static double EaseInOut(double start, double end, double t)
    {
        var p =  t * t * (3.0 - 2.0 * t);
        return start + (end - start) * p;
    }

    private double GetRandomValue() => _minValue + _random.NextDouble() * (_maxValue - _minValue);

    private double GetRandomTime() => _minSeconds + _random.NextDouble() * (_maxSeconds - _minSeconds);
}
