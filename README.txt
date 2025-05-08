# GOAP Planning System Design

## Overview

The Goal-Oriented Action Planning (GOAP) system is designed to enable autonomous agents to make intelligent decisions based on dynamic environmental data and internal state. The architecture is modular and layered, providing separation of concerns across sensing, reasoning, and execution. This document outlines the system's core layers, responsibilities, interconnections, and includes example C# implementations.

## Layered Architecture

### 1. Sensor Layer

The Sensor Layer is responsible for perceiving the environment and internal state of the agent. Each sensor component operates independently and returns structured data relevant to its domain.

**Sensors:**

* **VisionSensor:** Detects visible objects and their positions.
* **HearingSensor:** Detects sound-emitting objects and estimates proximity.
* **SmellSensor:** Detects odor-emitting objects with estimated intensity.
* **InventorySensor:** Returns the current list of items and quantities held.
* **StatsSensor:** Reports personal statistics such as health, stamina, and hunger.
* **MemorySensor:** Retrieves remembered events or facts from previous encounters.

**Output:** Lists of detected objects, positions, quantities, and statistical values.

**Example:**

```csharp
public interface ISensor<T>
{
    T Sense();
}

public class InventorySensor : ISensor<List<(Item item, int quantity)>>
{
    private readonly Inventory _inventory;

    public InventorySensor(Inventory inventory)
    {
        _inventory = inventory;
    }

    public List<(Item, int)> Sense()
    {
        return _inventory.Items.ToList();
    }
}
```

### 2. List Aggregation Layer

This layer aggregates and normalizes outputs from the Sensor Layer into a unified perception context. The goal is to centralize environmental and internal data to facilitate subsequent logical evaluations.

**Responsibilities:**

* Combine data from all sensors.
* Provide queryable access to the complete perception context.
* Maintain temporal relevance by purging outdated or stale information.

**Example:**

```csharp
public class PerceptionContext
{
    public List<(Item item, int quantity)> InventoryItems { get; set; } = [];
    public Dictionary<StatType, float> Stats { get; set; } = [];
    public List<GameObject> VisibleObjects { get; set; } = [];
    // Additional perception data as needed
}
```

### 3. Checklist Layer

The Checklist Layer derives boolean world-state facts from the aggregated sensor data. Each checklist item encapsulates a logical evaluation that returns true or false based on current perception context.

**Checklist Evaluations:**

* Defined by logical predicates.
* Evaluate presence, quantity, or proximity of specific objects.
* May reference personal statistics or memory components.

**Output:**
A dictionary of condition keys mapped to boolean values, representing the agent's current world state.

**Example:**

```csharp
public class ChecklistCondition
{
    public string Key { get; init; }
    public Func<PerceptionContext, bool> Evaluate { get; init; }
}

var hasFood = new ChecklistCondition
{
    Key = "HasFood",
    Evaluate = ctx => ctx.InventoryItems.Any(i => i.item.Type == ItemType.Food && i.quantity > 0)
};
```

### 4. Planning Layer

This layer implements the core GOAP algorithm. It evaluates available actions against the current world state and identifies a viable sequence of actions that achieve a specified goal state.

**Key Elements:**

* **Action Definitions:** Include preconditions and effects represented as boolean dictionaries.
* **Goal Conditions:** Target world-state conditions the agent aims to satisfy.
* **Planner:** Searches for action sequences that transform the current world state into the goal state by satisfying all preconditions and applying effects iteratively.

**Example:**

```csharp
public class GoapAction
{
    public string Name { get; init; }
    public Dictionary<string, bool> Preconditions { get; init; } = [];
    public Dictionary<string, bool> Effects { get; init; } = [];
    public Func<bool> CanExecute { get; init; }
    public Action Execute { get; init; }
}

public class GoapPlanner
{
    public List<GoapAction> Plan(
        Dictionary<string, bool> currentState,
        Dictionary<string, bool> goal,
        List<GoapAction> availableActions)
    {
        // Simplified forward planner
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
```

### 5. Action Execution Layer

Each action has two functional aspects: a planning definition and an execution implementation.

* **Planning Phase:** Specifies what conditions the action satisfies.
* **Execution Phase:** Contains the real-world effect logic (e.g., modifying inventory, interacting with environment).

**Example:**

```csharp
var huntAction = new GoapAction
{
    Name = "Hunt",
    Preconditions = new() { { "HasWeapon", true } },
    Effects = new() { { "HasFood", true } },
    CanExecute = () => true, // Replace with real condition check
    Execute = () => Debug.Log("Agent goes hunting.")
};
```

### 6. Agent State Machine

The agent operates under an overarching finite state machine (FSM), which governs high-level behavior modes (e.g., idle, combat, survival). Each state is associated with specific checklist conditions and triggers that determine state transitions.

**Responsibilities:**

* Enable or disable specific checklist conditions.
* Determine when replanning is necessary.
* Handle transitions due to failures, timeouts, or external triggers.

**Example:**

```csharp
public enum AgentState
{
    Idle,
    Combat,
    Survival,
    Fleeing
}

public class AgentStateMachine
{
    public AgentState CurrentState { get; private set; } = AgentState.Idle;

    public void UpdateState(Dictionary<string, bool> worldState)
    {
        if (worldState.TryGetValue("IsThreatened", out var threatened) && threatened)
            CurrentState = AgentState.Combat;
        else if (worldState.TryGetValue("NeedsFood", out var hungry) && hungry)
            CurrentState = AgentState.Survival;
        else
            CurrentState = AgentState.Idle;
    }
}
```

## Example Use Case: "HasFood" Condition

1. **Sensor Layer:** InventorySensor detects an apple in inventory.
2. **Checklist Layer:** The "HasFood" condition evaluates to true.
3. **Planning Layer:** Goal to acquire food is skipped; already satisfied.
4. **Execution:** No hunting actions are selected.
5. **If False:** Planner selects the "Hunt" action to fulfill "HasFood".
6. **Execution:** Upon success, the inventory is updated, re-evaluating "HasFood" as true.

## Summary

This architecture provides a clean and extensible model for autonomous agent behavior. The separation between sensing, reasoning, and acting ensures maintainability and scalability in complex simulations. Each layer builds on the previous one, enabling nuanced decision-making rooted in both perception and internal goals. The included code snippets provide a basic framework that can be extended and refined based on the needs of your specific simulation.
