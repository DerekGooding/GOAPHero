namespace GOAPHero.Core;
/// <summary>
/// Defines a condition that can be evaluated against a perception context.
/// </summary>
public interface IChecklistCondition
{
    /// <summary>
    /// Gets the unique key identifying this condition.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Evaluates the condition against the provided perception context.
    /// </summary>
    /// <param name="context">The perception context to evaluate against.</param>
    /// <returns>True if the condition is satisfied, false otherwise.</returns>
    bool Evaluate(PerceptionContext context);
}