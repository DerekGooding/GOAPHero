namespace GOAPHero.Core;
/// <summary>
/// Defines a sensor that can sense information of type T from the environment.
/// </summary>
/// <typeparam name="T">The type of data sensed by this sensor.</typeparam>
public interface ISensor<T>
{
    /// <summary>
    /// Senses information from the environment.
    /// </summary>
    /// <returns>The sensed data of type T.</returns>
    T Sense();
}