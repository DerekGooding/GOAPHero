namespace GOAPHero.Sensors;

/// <summary>
/// A sensor that returns the inventory items of an agent.
/// </summary>
public class InventorySensor : ISensor<List<(Item item, int quantity)>>
{
    private readonly Func<List<(Item, int)>> _inventoryProvider;

    /// <summary>
    /// Creates a new inventory sensor.
    /// </summary>
    /// <param name="inventoryProvider">A function that provides the inventory items.</param>
    public InventorySensor(Func<List<(Item, int)>> inventoryProvider)
    {
        _inventoryProvider = inventoryProvider;
    }

    /// <summary>
    /// Senses the inventory items.
    /// </summary>
    /// <returns>A list of inventory items and their quantities.</returns>
    public List<(Item item, int quantity)> Sense() => _inventoryProvider();
}
