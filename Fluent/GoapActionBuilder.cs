namespace GOAPHero.Fluent;

/// <summary>
/// A fluent builder for creating GOAP actions.
/// </summary>
public class GoapActionBuilder
{
    private string _name = "UnnamedAction";
    private readonly Dictionary<string, bool> _preconditions = [];
    private readonly Dictionary<string, bool> _effects = [];
    private Func<bool> _canExecute = () => true;
    private Action _execute = () => { };

    /// <summary>
    /// Sets the name of the action.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>This builder for method chaining.</returns>
    public GoapActionBuilder Named(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds a precondition to the action.
    /// </summary>
    /// <param name="key">The key of the precondition.</param>
    /// <param name="value">The required value of the precondition.</param>
    /// <returns>This builder for method chaining.</returns>
    public GoapActionBuilder Requires(string key, bool value)
    {
        _preconditions[key] = value;
        return this;
    }

    /// <summary>
    /// Adds an effect to the action.
    /// </summary>
    /// <param name="key">The key of the effect.</param>
    /// <param name="value">The value the effect will set.</param>
    /// <returns>This builder for method chaining.</returns>
    public GoapActionBuilder Causes(string key, bool value)
    {
        _effects[key] = value;
        return this;
    }

    /// <summary>
    /// Sets the condition that determines whether the action can be executed.
    /// </summary>
    /// <param name="canExecute">A function that returns true if the action can be executed.</param>
    /// <returns>This builder for method chaining.</returns>
    public GoapActionBuilder When(Func<bool> canExecute)
    {
        _canExecute = canExecute;
        return this;
    }

    /// <summary>
    /// Sets the function that executes the action.
    /// </summary>
    /// <param name="execute">The function to execute.</param>
    /// <returns>This builder for method chaining.</returns>
    public GoapActionBuilder Do(Action execute)
    {
        _execute = execute;
        return this;
    }

    /// <summary>
    /// Builds and returns the GOAP action.
    /// </summary>
    /// <returns>A new GOAP action configured according to this builder.</returns>
    public GoapAction Build() => new(_name, _preconditions, _effects, _canExecute, _execute);
}