namespace GOAPHero.Factory;

public static class ChecklistFactory
{
    public static IChecklistCondition Create(string key, Func<PerceptionContext, bool> predicate)
        => new LambdaChecklistCondition(key, predicate);

    private class LambdaChecklistCondition(string key, Func<PerceptionContext, bool> predicate) : IChecklistCondition
    {
        public string Key { get; } = key;
        private readonly Func<PerceptionContext, bool> _predicate = predicate;

        public bool Evaluate(PerceptionContext context) => _predicate(context);
    }
}
