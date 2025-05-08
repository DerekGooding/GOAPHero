namespace GOAPHero.Core;
public class PerceptionContext
{
    public List<(Item item, int quantity)> InventoryItems { get; set; } = [];
    public Dictionary<StatType, float> Stats { get; set; } = [];
    public List<GameObject> VisibleObjects { get; set; } = [];
}