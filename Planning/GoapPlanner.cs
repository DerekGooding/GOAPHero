namespace GOAPHero.Planning;

/// <summary>
/// A basic GOAP planner that finds a sequence of actions to achieve a goal.
/// </summary>
public class GoapPlanner
{
    /// <summary>
    /// Plans a sequence of actions to achieve the specified goal from the current state.
    /// </summary>
    /// <param name="currentState">The current world state.</param>
    /// <param name="goal">The goal state to achieve.</param>
    /// <param name="availableActions">The actions available to the agent.</param>
    /// <returns>A list of actions that achieve the goal, or an empty list if no plan is found.</returns>
    public virtual List<GoapAction> Plan(
        Dictionary<string, bool> currentState,
        Dictionary<string, bool> goal,
        List<GoapAction> availableActions)
    {
        // If the goal is already satisfied, no plan is needed
        if (goal.All(g => currentState.TryGetValue(g.Key, out var val) && val == g.Value))
        {
            return [];
        }
        
        var plan = new List<GoapAction>();
        var state = new Dictionary<string, bool>(currentState);
        
        // Filter actions to only those that can be executed
        var executableActions = availableActions.Where(a => a.CanExecute()).ToList();
        
        // Check if any action can satisfy the goal
        foreach (var action in executableActions)
        {
            if (action.Preconditions.All(p => state.TryGetValue(p.Key, out var val) && val == p.Value))
            {
                var newState = new Dictionary<string, bool>(state);
                foreach (var effect in action.Effects)
                {
                    newState[effect.Key] = effect.Value;
                }
                
                if (goal.All(g => newState.TryGetValue(g.Key, out var val) && val == g.Value))
                {
                    plan.Add(action);
                    return plan;
                }
            }
        }
        
        // Try to build a plan recursively
        return BuildPlanRecursive(state, goal, executableActions, [], 0, 5);
    }
    
    /// <summary>
    /// Recursively builds a plan to achieve the goal.
    /// </summary>
    /// <param name="state">The current state.</param>
    /// <param name="goal">The goal state.</param>
    /// <param name="actions">The available actions.</param>
    /// <param name="currentPlan">The plan being built.</param>
    /// <param name="depth">The current recursion depth.</param>
    /// <param name="maxDepth">The maximum recursion depth.</param>
    /// <returns>A list of actions that achieve the goal, or an empty list if no plan is found.</returns>
    protected List<GoapAction> BuildPlanRecursive(
        Dictionary<string, bool> state,
        Dictionary<string, bool> goal,
        List<GoapAction> actions,
        List<GoapAction> currentPlan,
        int depth,
        int maxDepth)
    {
        // Check if we've reached the maximum recursion depth
        if (depth >= maxDepth)
        {
            return [];
        }
        
        // Check if the goal is already satisfied
        if (goal.All(g => state.TryGetValue(g.Key, out var val) && val == g.Value))
        {
            return new List<GoapAction>(currentPlan);
        }
        
        // Try each action
        foreach (var action in actions)
        {
            // Skip actions that are already in the plan to avoid cycles
            if (currentPlan.Contains(action))
                continue;
                
            // Check if the action's preconditions are satisfied
            if (action.Preconditions.All(p => state.TryGetValue(p.Key, out var val) && val == p.Value))
            {
                // Apply the action's effects
                var newState = new Dictionary<string, bool>(state);
                foreach (var effect in action.Effects)
                {
                    newState[effect.Key] = effect.Value;
                }
                
                // Add the action to the plan
                var newPlan = new List<GoapAction>(currentPlan) { action };
                
                // Check if the goal is now satisfied
                if (goal.All(g => newState.TryGetValue(g.Key, out var val) && val == g.Value))
                {
                    return newPlan;
                }
                
                // Recursively try to build a plan from the new state
                var result = BuildPlanRecursive(newState, goal, actions, newPlan, depth + 1, maxDepth);
                if (result.Count > 0)
                {
                    return result;
                }
            }
        }
        
        // No plan found
        return [];
    }
}