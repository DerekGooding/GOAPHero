namespace GOAPHero.Sensors;

/// <summary>
/// A sensor that detects objects of a specific type within a certain radius.
/// </summary>
/// <remarks>
/// Creates a new typed proximity sensor.
/// </remarks>
/// <param name="worldObjectsProvider">A function that provides the list of all objects in the world.</param>
/// <param name="position">The position of the sensor.</param>
/// <param name="radius">The detection radius of the sensor.</param>
public class TypedProximitySensor<T>(Func<List<GameObject>> worldObjectsProvider, Vector3 position, float radius) : ISensor<List<T>> where T : GameObject
{
    private readonly Func<List<GameObject>> _worldObjectsProvider = worldObjectsProvider;
    private Vector3 _position = position;
    private readonly float _radius = radius;

    /// <summary>
    /// Updates the position of the sensor.
    /// </summary>
    /// <param name="newPosition">The new position.</param>
    public void UpdatePosition(Vector3 newPosition) => _position = newPosition;

    /// <summary>
    /// Senses objects of the specified type within the detection radius.
    /// </summary>
    /// <returns>A list of objects of the specified type within the detection radius.</returns>
    public List<T> Sense()
    {
        var allObjects = _worldObjectsProvider();
        return [.. allObjects
            .Where(obj => obj is T && Vector3.Distance(_position, obj.Position) <= _radius)
            .Cast<T>()];
    }
}
