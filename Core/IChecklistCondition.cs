namespace GOAP.Core;
public interface IChecklistCondition
{
    string Key { get; }
    bool Evaluate(PerceptionContext context);
}