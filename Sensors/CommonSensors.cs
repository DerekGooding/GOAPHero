namespace GOAPHero.Sensors;

/// <summary>
/// A sensor that detects objects within a certain radius.
/// </summary>
/// <remarks>
/// Creates a new proximity sensor.
/// </remarks>
/// <param name="worldObjectsProvider">A function that provides the list of all objects in the world.</param>
/// <param name="position">The position of the sensor.</param>
/// <param name="radius">The detection radius of the sensor.</param>
public class ProximitySensor(Func<List<GameObject>> worldObjectsProvider, Vector3 position, float radius) : ISensor<List<GameObject>>
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
    /// Senses objects within the detection radius.
    /// </summary>
    /// <returns>A list of objects within the detection radius.</returns>
    public List<GameObject> Sense()
    {
        var allObjects = _worldObjectsProvider();
        return allObjects.Where(obj => Vector3.Distance(_position, obj.Position) <= _radius).ToList();
    }
}
