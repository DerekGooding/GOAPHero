namespace GOAP.Factory;

public static class ActionFactory
{
    public static GoapAction Create(
        string name,
        Dictionary<string, bool> preconditions,
        Dictionary<string, bool> effects,
        Func<bool> canExecute,
        Action execute) => new(name, preconditions, effects, canExecute, execute);
}