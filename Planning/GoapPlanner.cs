namespace GOAPHero.Planning;

public class GoapPlanner
{
    public List<GoapAction> Plan(
        Dictionary<string, bool> currentState,
        Dictionary<string, bool> goal,
        List<GoapAction> availableActions)
    {
        var plan = new List<GoapAction>();
        var state = new Dictionary<string, bool>(currentState);

        foreach (var action in availableActions)
        {
            if (action.Preconditions.All(p => state.TryGetValue(p.Key, out var val) && val == p.Value))
            {
                plan.Add(action);
                foreach (var effect in action.Effects)
                {
                    state[effect.Key] = effect.Value;
                }

                if (goal.All(g => state.TryGetValue(g.Key, out var val) && val == g.Value))
                {
                    return plan;
                }
            }
        }

        return [];
    }
}