namespace GOAPHero.Sensors;

/// <summary>
/// A sensor that returns a constant value.
/// </summary>
/// <typeparam name="T">The type of data sensed.</typeparam>
public class ConstantSensor<T> : ISensor<T>
{
    private readonly T _value;

    /// <summary>
    /// Creates a new constant sensor.
    /// </summary>
    /// <param name="value">The constant value to return.</param>
    public ConstantSensor(T value)
    {
        _value = value;
    }

    /// <summary>
    /// Senses the constant value.
    /// </summary>
    /// <returns>The constant value.</returns>
    public T Sense() => _value;
}
