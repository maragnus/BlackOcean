namespace BlackOcean.Common.Test;

public class ValueTests
{
    [Test]
    public void TrendingValueWorks()
    {
        var trend = new TrendingValue();
        trend.AddValue(100, 0.0);
        trend.AddValue(101, 0.2);
        trend.AddValue(102, 0.4);
        trend.AddValue(103, 0.6);
        trend.AddValue(104, 0.8);
        trend.AddValue(105, 1.0);
        trend.AddValue(106, 1.2);
        Assert.That(trend.GetTrend(), Is.EqualTo(5));
    }
}