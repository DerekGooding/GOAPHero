namespace GOAPHero.Core;
/// <summary>
/// Represents an item that can be stored in an inventory.
/// </summary>
/// <param name="Name">The name of the item.</param>
/// <param name="Type">The type of the item.</param>
public sealed record Item(string Name, ItemType Type);