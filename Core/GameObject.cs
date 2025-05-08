namespace GOAPHero.Core;

public class GameObject(string name, Vector3 position)
{
    public string Name { get; init; } = name;
    public Vector3 Position { get; init; } = position;
    public Dictionary<string, object> Properties { get; } = [];
}