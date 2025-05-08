namespace GOAPHero.Factory;

/// <summary>
/// Factory for creating GOAP actions.
/// </summary>
public static class ActionFactory
{
    /// <summary>
    /// Creates a new GOAP action with the specified parameters.
    /// </summary>
    /// <param name="name">The name of the action.</param>
    /// <param name="preconditions">The conditions that must be true before the action can be executed.</param>
    /// <param name="effects">The conditions that will be true after the action is executed.</param>
    /// <param name="canExecute">A function that determines whether the action can be executed.</param>
    /// <param name="execute">The function that executes the action.</param>
    /// <returns>A new GOAP action.</returns>
    public static GoapAction Create(
        string name,
        Dictionary<string, bool> preconditions,
        Dictionary<string, bool> effects,
        Func<bool> canExecute,
        Action execute) => new(name, preconditions, effects, canExecute, execute);
}