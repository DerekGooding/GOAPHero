namespace GOAPHero.Core;

/// <summary>
/// Represents a game object in the world with a name, position, and custom properties.
/// </summary>
/// <remarks>
/// Creates a new game object with the specified name and position.
/// </remarks>
/// <param name="name">The name of the game object.</param>
/// <param name="position">The position of the game object in 3D space.</param>
public class GameObject(string name, Vector3 position)
{
    public string Name { get; init; } = name;
    public Vector3 Position { get; init; } = position;
    public Dictionary<string, object> Properties { get; } = [];
}