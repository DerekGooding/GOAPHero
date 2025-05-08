namespace GOAPHero.Planning;

using GOAPHero.Core;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An advanced GOAP planner that supports costs and heuristics to find optimal plans.
/// </summary>
public class EnhancedGoapPlanner : GoapPlanner
{
    /// <summary>
    /// Represents a node in the A* search algorithm for GOAP planning.
    /// </summary>
    private class PlanNode
    {
        public Dictionary<string, bool> State { get; }
        public GoapAction? Action { get; }
        public PlanNode? Parent { get; }
        public float RunningCost { get; }
        public float HeuristicCost { get; }
        public float TotalCost => RunningCost + HeuristicCost;

        public PlanNode(Dictionary<string, bool> state, GoapAction? action, PlanNode? parent, float runningCost, float heuristicCost)
        {
            State = state;
            Action = action;
            Parent = parent;
            RunningCost = runningCost;
            HeuristicCost = heuristicCost;
        }
    }

    /// <summary>
    /// Finds an action plan to achieve the specified goal from the current state.
    /// </summary>
    /// <param name="currentState">The current world state.</param>
    /// <param name="goal">The goal state to achieve.</param>
    /// <param name="availableActions">The actions available to the agent.</param>
    /// <param name="maxIterations">The maximum number of iterations to perform. Default is 1000.</param>
    /// <returns>A list of actions that achieve the goal, or an empty list if no plan is found.</returns>
    public List<GoapAction> FindPlan(
        Dictionary<string, bool> currentState,
        Dictionary<string, bool> goal,
        List<GoapAction> availableActions,
        int maxIterations = 1000)
    {
        // Check if the goal is already satisfied
        if (goal.All(g => currentState.TryGetValue(g.Key, out var val) && val == g.Value))
        {
            return new List<GoapAction>();
        }

        // Initialize the open and closed sets for A* search
        var openSet = new List<PlanNode>();
        var closedSet = new HashSet<string>(); // Using serialized state as the key
        
        // Create the start node
        var startNode = new PlanNode(
            new Dictionary<string, bool>(currentState),
            null,
            null,
            0,
            CalculateHeuristic(currentState, goal));
        
        openSet.Add(startNode);
        
        int iterations = 0;
        
        while (openSet.Count > 0 && iterations < maxIterations)
        {
            iterations++;
            
            // Find the node with the lowest cost
            var current = openSet.OrderBy(n => n.TotalCost).First();
            openSet.Remove(current);
            
            // Skip if this state has already been processed
            string stateKey = SerializeState(current.State);
            if (closedSet.Contains(stateKey))
                continue;
                
            closedSet.Add(stateKey);
            
            // Check if the goal is achieved
            if (goal.All(g => current.State.TryGetValue(g.Key, out var val) && val == g.Value))
            {
                // Goal achieved, build the plan
                return BuildPlan(current);
            }
            
            // Explore applicable actions
            foreach (var action in availableActions)
            {
                // Skip actions that cannot be executed
                if (!action.CanExecute())
                    continue;
                    
                // Check if the action's preconditions are satisfied
                if (!action.Preconditions.All(p => current.State.TryGetValue(p.Key, out var val) && val == p.Value))
                    continue;
                    
                // Apply the action's effects to create a new state
                var newState = new Dictionary<string, bool>(current.State);
                foreach (var effect in action.Effects)
                {
                    newState[effect.Key] = effect.Value;
                }
                
                // Calculate the cost of the new state
                float actionCost = action.Cost;
                float newRunningCost = current.RunningCost + actionCost;
                float heuristicCost = CalculateHeuristic(newState, goal);
                
                // Create a new node
                var newNode = new PlanNode(
                    newState,
                    action,
                    current,
                    newRunningCost,
                    heuristicCost);
                    
                // Add the new node to the open set
                openSet.Add(newNode);
            }
        }
        
        // No plan found
        return new List<GoapAction>();
    }

    /// <summary>
    /// Calculates a heuristic estimate of the cost to reach the goal from the current state.
    /// </summary>
    /// <param name="state">The current state.</param>
    /// <param name="goal">The goal state.</param>
    /// <returns>A heuristic cost estimate.</returns>
    private float CalculateHeuristic(Dictionary<string, bool> state, Dictionary<string, bool> goal)
    {
        // Simple heuristic: count the number of goal conditions not yet satisfied
        float unsatisfiedGoals = goal.Count(g => 
            !state.TryGetValue(g.Key, out var val) || val != g.Value);
            
        return unsatisfiedGoals;
    }

    /// <summary>
    /// Converts a state dictionary to a string for use as a key in the closed set.
    /// </summary>
    /// <param name="state">The state to serialize.</param>
    /// <returns>A string representation of the state.</returns>
    private string SerializeState(Dictionary<string, bool> state)
    {
        var sortedKeys = state.Keys.OrderBy(k => k);
        return string.Join(";", sortedKeys.Select(k => $"{k}={state[k]}"));
    }

    /// <summary>
    /// Builds a plan by tracing back from the goal node to the start node.
    /// </summary>
    /// <param name="node">The goal node.</param>
    /// <returns>A list of actions in execution order.</returns>
    private List<GoapAction> BuildPlan(PlanNode node)
    {
        var actions = new List<GoapAction>();
        var current = node;
        
        // Trace back from the goal node to the start node
        while (current != null && current.Action != null)
        {
            actions.Add(current.Action);
            current = current.Parent;
        }
        
        // Reverse the actions to get them in execution order
        actions.Reverse();
        return actions;
    }
    
    /// <summary>
    /// Plans a sequence of actions to achieve the specified goal from the current state.
    /// </summary>
    /// <param name="currentState">The current world state.</param>
    /// <param name="goal">The goal state to achieve.</param>
    /// <param name="availableActions">The actions available to the agent.</param>
    /// <returns>A list of actions that achieve the goal, or an empty list if no plan is found.</returns>
    public override List<GoapAction> Plan(
        Dictionary<string, bool> currentState,
        Dictionary<string, bool> goal,
        List<GoapAction> availableActions)
    {
        return FindPlan(currentState, goal, availableActions);
    }
}