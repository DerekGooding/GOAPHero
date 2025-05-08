namespace GOAPHero.Sensors;

/// <summary>
/// A sensor that returns a constant value.
/// </summary>
/// <typeparam name="T">The type of data sensed.</typeparam>
/// <remarks>
/// Creates a new constant sensor.
/// </remarks>
/// <param name="value">The constant value to return.</param>
public class ConstantSensor<T>(T value) : ISensor<T>
{
    private readonly T _value = value;

    /// <summary>
    /// Senses the constant value.
    /// </summary>
    /// <returns>The constant value.</returns>
    public T Sense() => _value;
}
