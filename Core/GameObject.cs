namespace GOAP.Core;

public sealed class GameObject
{
    public string Name { get; init; }
    public Vector3 Position { get; init; }
    public Dictionary<string, object> Properties { get; } = new();

    public GameObject(string name, Vector3 position)
    {
        Name = name;
        Position = position;
    }
}