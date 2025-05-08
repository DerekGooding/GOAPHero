namespace GOAPHero.Sensors;

/// <summary>
/// A sensor that filters the results of another sensor.
/// </summary>
/// <typeparam name="T">The type of data sensed.</typeparam>
/// <remarks>
/// Creates a new filtered sensor.
/// </remarks>
/// <param name="baseSensor">The base sensor to filter.</param>
/// <param name="filter">The filter predicate.</param>
public class FilteredSensor<T>(ISensor<List<T>> baseSensor, Predicate<T> filter) : ISensor<List<T>>
{
    private readonly ISensor<List<T>> _baseSensor = baseSensor;
    private readonly Predicate<T> _filter = filter;

    /// <summary>
    /// Senses data from the base sensor and applies the filter.
    /// </summary>
    /// <returns>A filtered list of sensed data.</returns>
    public List<T> Sense() => [.. _baseSensor.Sense().Where(item => _filter(item))];
}
