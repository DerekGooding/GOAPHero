namespace GOAPHero.Core;
/// <summary>
/// Represents the perceptual context of an agent, containing information about inventory, stats, and visible objects.
/// </summary>
public class PerceptionContext
{
    /// <summary>
    /// Gets or sets the list of items in the agent's inventory along with their quantities.
    /// </summary>
    public List<(Item item, int quantity)> InventoryItems { get; set; } = [];

    /// <summary>
    /// Gets or sets the agent's current stats and their values.
    /// </summary>
    public Dictionary<StatType, float> Stats { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of objects visible to the agent.
    /// </summary>
    public List<GameObject> VisibleObjects { get; set; } = [];
}