namespace GOAPHero.Fluent;

public class GoapActionBuilder
{
    private string _name = "UnnamedAction";
    private readonly Dictionary<string, bool> _preconditions = [];
    private readonly Dictionary<string, bool> _effects = [];
    private Func<bool> _canExecute = () => true;
    private Action _execute = () => {};

    public GoapActionBuilder Named(string name)
    {
        _name = name;
        return this;
    }

    public GoapActionBuilder Requires(string key, bool value)
    {
        _preconditions[key] = value;
        return this;
    }

    public GoapActionBuilder Causes(string key, bool value)
    {
        _effects[key] = value;
        return this;
    }

    public GoapActionBuilder When(Func<bool> canExecute)
    {
        _canExecute = canExecute;
        return this;
    }

    public GoapActionBuilder Do(Action execute)
    {
        _execute = execute;
        return this;
    }

    public GoapAction Build() => new(_name, _preconditions, _effects, _canExecute, _execute);
}