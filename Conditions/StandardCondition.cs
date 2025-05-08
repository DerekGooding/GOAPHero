using GOAPHero.Factory;

namespace GOAPHero.Conditions;
/// <summary>
/// A class providing standard checklist conditions for GOAP simulations.
/// </summary>
public static class StandardConditions
{
    /// <summary>
    /// Creates a condition that checks if an item of a specific type is in the inventory.
    /// </summary>
    /// <param name="itemType">The type of item to check for.</param>
    /// <returns>A checklist condition.</returns>
    public static IChecklistCondition HasItemOfType(ItemType itemType)
    {
        var key = $"Has{itemType}";
        return ChecklistFactory.Create(key, context =>
            context.InventoryItems.Any(i => i.item.Type == itemType && i.quantity > 0));
    }

    /// <summary>
    /// Creates a condition that checks if a specific item is in the inventory.
    /// </summary>
    /// <param name="itemName">The name of the item to check for.</param>
    /// <returns>A checklist condition.</returns>
    public static IChecklistCondition HasItem(string itemName)
    {
        var key = $"Has{itemName.Replace(" ", "")}";
        return ChecklistFactory.Create(key, context =>
            context.InventoryItems.Any(i => i.item.Name == itemName && i.quantity > 0));
    }

    /// <summary>
    /// Creates a condition that checks if the inventory has at least a specific quantity of an item.
    /// </summary>
    /// <param name="itemName">The name of the item to check for.</param>
    /// <param name="minQuantity">The minimum required quantity.</param>
    /// <returns>A checklist condition.</returns>
    public static IChecklistCondition HasItemQuantity(string itemName, int minQuantity)
    {
        var key = $"Has{itemName.Replace(" ", "")}{minQuantity}";
        return ChecklistFactory.Create(key, context =>
            context.InventoryItems.Any(i => i.item.Name == itemName && i.quantity >= minQuantity));
    }

    /// <summary>
    /// Creates a condition that checks if a stat is above a specific threshold.
    /// </summary>
    /// <param name="statType">The stat to check.</param>
    /// <param name="threshold">The threshold value.</param>
    /// <returns>A checklist condition.</returns>
    public static IChecklistCondition StatAbove(StatType statType, float threshold)
    {
        var key = $"{statType}Above{threshold}";
        return ChecklistFactory.Create(key, context =>
            context.Stats.TryGetValue(statType, out var value) && value > threshold);
    }

    /// <summary>
    /// Creates a condition that checks if a stat is below a specific threshold.
    /// </summary>
    /// <param name="statType">The stat to check.</param>
    /// <param name="threshold">The threshold value.</param>
    /// <returns>A checklist condition.</returns>
    public static IChecklistCondition StatBelow(StatType statType, float threshold)
    {
        var key = $"{statType}Below{threshold}";
        return ChecklistFactory.Create(key, context =>
            context.Stats.TryGetValue(statType, out var value) && value < threshold);
    }

    /// <summary>
    /// Creates a condition that checks if an object of a specific name is visible.
    /// </summary>
    /// <param name="objectName">The name of the object to check for.</param>
    /// <returns>A checklist condition.</returns>
    public static IChecklistCondition CanSee(string objectName)
    {
        var key = $"CanSee{objectName.Replace(" ", "")}";
        return ChecklistFactory.Create(key, context =>
            context.VisibleObjects.Any(obj => obj.Name == objectName));
    }

    /// <summary>
    /// Creates a condition that checks if an object with a specific property value is visible.
    /// </summary>
    /// <param name="propertyName">The name of the property to check.</param>
    /// <param name="propertyValue">The expected value of the property.</param>
    /// <returns>A checklist condition.</returns>
    public static IChecklistCondition CanSeeObjectWithProperty(string propertyName, object propertyValue)
    {
        var key = $"CanSeeObjectWith{propertyName}{propertyValue}";
        return ChecklistFactory.Create(key, context =>
            context.VisibleObjects.Any(obj =>
                obj.Properties.TryGetValue(propertyName, out var value) &&
                value.Equals(propertyValue)));
    }

    /// <summary>
    /// Creates a condition that checks if an object is nearby (within a certain distance).
    /// </summary>
    /// <param name="objectName">The name of the object to check for.</param>
    /// <param name="maxDistance">The maximum allowed distance.</param>
    /// <param name="agentPosition">A function that provides the agent's position.</param>
    /// <returns>A checklist condition.</returns>
    public static IChecklistCondition IsNearby(string objectName, float maxDistance, Func<Vector3> agentPosition)
    {
        var key = $"IsNear{objectName.Replace(" ", "")}";
        return ChecklistFactory.Create(key, context =>
        {
            var obj = context.VisibleObjects.FirstOrDefault(o => o.Name == objectName);
            if (obj == null) return false;

            var position = agentPosition();
            var dx = position.X - obj.Position.X;
            var dy = position.Y - obj.Position.Y;
            var dz = position.Z - obj.Position.Z;
            var distanceSquared = (dx * dx) + (dy * dy) + (dz * dz);

            return distanceSquared <= maxDistance * maxDistance;
        });
    }

    /// <summary>
    /// Creates a negated version of a condition.
    /// </summary>
    /// <param name="condition">The condition to negate.</param>
    /// <returns>A negated checklist condition.</returns>
    public static IChecklistCondition Not(IChecklistCondition condition)
    {
        var key = $"Not{condition.Key}";
        return ChecklistFactory.Create(key, context => !condition.Evaluate(context));
    }

    /// <summary>
    /// Creates a condition that is true if all the specified conditions are true.
    /// </summary>
    /// <param name="conditions">The conditions to check.</param>
    /// <returns>A combined checklist condition.</returns>
    public static IChecklistCondition All(params IChecklistCondition[] conditions)
    {
        var key = string.Join("And", conditions.Select(c => c.Key));
        return ChecklistFactory.Create(key, context => conditions.All(c => c.Evaluate(context)));
    }

    /// <summary>
    /// Creates a condition that is true if any of the specified conditions are true.
    /// </summary>
    /// <param name="conditions">The conditions to check.</param>
    /// <returns>A combined checklist condition.</returns>
    public static IChecklistCondition Any(params IChecklistCondition[] conditions)
    {
        var key = string.Join("Or", conditions.Select(c => c.Key));
        return ChecklistFactory.Create(key, context => conditions.Any(c => c.Evaluate(context)));
    }
}