namespace GOAPHero.Sensors;

/// <summary>
/// A sensor that combines the results of multiple sensors.
/// </summary>
/// <typeparam name="T">The type of data sensed.</typeparam>
/// <remarks>
/// Creates a new composite sensor.
/// </remarks>
/// <param name="sensors">The sensors to combine.</param>
public class CompositeSensor<T>(params ISensor<List<T>>[] sensors) : ISensor<List<T>>
{
    private readonly List<ISensor<List<T>>> _sensors = [.. sensors];

    /// <summary>
    /// Adds a sensor to the composite.
    /// </summary>
    /// <param name="sensor">The sensor to add.</param>
    public void AddSensor(ISensor<List<T>> sensor) => _sensors.Add(sensor);

    /// <summary>
    /// Senses data from all child sensors.
    /// </summary>
    /// <returns>A combined list of sensed data.</returns>
    public List<T> Sense()
    {
        var result = new List<T>();
        foreach (var sensor in _sensors)
        {
            result.AddRange(sensor.Sense());
        }
        return result;
    }
}
