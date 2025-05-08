namespace GOAPHero.Sensors;

/// <summary>
/// A sensor that returns the stats of an agent.
/// </summary>
/// <remarks>
/// Creates a new stats sensor.
/// </remarks>
/// <param name="statsProvider">A function that provides the stats.</param>
public class StatsSensor(Func<Dictionary<StatType, float>> statsProvider) : ISensor<Dictionary<StatType, float>>
{
    private readonly Func<Dictionary<StatType, float>> _statsProvider = statsProvider;

    /// <summary>
    /// Senses the stats.
    /// </summary>
    /// <returns>A dictionary of stats and their values.</returns>
    public Dictionary<StatType, float> Sense() => _statsProvider();
}
