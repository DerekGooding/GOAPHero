namespace GOAPHero.Core;
/// <summary>
/// Represents an action in a GOAP system.
/// </summary>
/// <remarks>
/// Creates a new action with the specified parameters.
/// </remarks>
/// <param name="name">The name of the action.</param>
/// <param name="preconditions">The conditions that must be true before the action can be executed.</param>
/// <param name="effects">The conditions that will be true after the action is executed.</param>
/// <param name="canExecute">A function that determines whether the action can be executed.</param>
/// <param name="execute">The function that executes the action.</param>
/// <param name="cost">The cost of executing the action. Default is 1.0.</param>
public class GoapAction(
    string name,
    Dictionary<string, bool> preconditions,
    Dictionary<string, bool> effects,
    Func<bool> canExecute,
    Action execute,
    float cost = 1.0f)
{
    /// <summary>
    /// The name of the action.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The conditions that must be true before the action can be executed.
    /// </summary>
    public Dictionary<string, bool> Preconditions { get; } = preconditions;

    /// <summary>
    /// The conditions that will be true after the action is executed.
    /// </summary>
    public Dictionary<string, bool> Effects { get; } = effects;

    /// <summary>
    /// A function that determines whether the action can be executed.
    /// </summary>
    public Func<bool> CanExecute { get; } = canExecute;

    /// <summary>
    /// The function that executes the action.
    /// </summary>
    public Action Execute { get; } = execute;

    /// <summary>
    /// The cost of executing this action. Higher values make the action less desirable.
    /// </summary>
    public float Cost { get; set; } = cost;

    /// <summary>
    /// Returns a string representation of the action.
    /// </summary>
    /// <returns>A string that represents the action.</returns>
    public override string ToString() => $"Action: {Name} (Cost: {Cost})";
}