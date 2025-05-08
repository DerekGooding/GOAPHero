namespace GOAPHero.Core;
public record GoapAction(
    string Name,
    Dictionary<string, bool> Preconditions,
    Dictionary<string, bool> Effects,
    Func<bool> CanExecute,
    Action Execute
);