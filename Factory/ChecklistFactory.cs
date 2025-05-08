namespace GOAPHero.Factory;

public static class ChecklistFactory
{
    public static IChecklistCondition Create(string key, Func<PerceptionContext, bool> predicate)
        => new LambdaChecklistCondition(key, predicate);

    private class LambdaChecklistCondition : IChecklistCondition
    {
        public string Key { get; }
        private readonly Func<PerceptionContext, bool> _predicate;

        public LambdaChecklistCondition(string key, Func<PerceptionContext, bool> predicate)
        {
            Key = key;
            _predicate = predicate;
        }

        public bool Evaluate(PerceptionContext context) => _predicate(context);
    }
}
