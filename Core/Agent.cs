namespace GOAPHero.Core;

using GOAPHero.Planning;

/// <summary>
/// Represents an autonomous agent that uses GOAP for decision making.
/// </summary>
public class Agent
{
    private readonly string _name;
    private readonly GoapPlanner _planner;
    private readonly Dictionary<string, ISensor<object>> _sensors = new();
    private readonly List<IChecklistCondition> _checklistConditions = new();
    private readonly List<GoapAction> _availableActions = new();
    
    private AgentState _currentState = AgentState.Idle;
    private List<GoapAction> _currentPlan = new();
    private int _currentActionIndex = 0;
    
    /// <summary>
    /// The current world state as perceived by the agent.
    /// </summary>
    public Dictionary<string, bool> WorldState { get; private set; } = new();
    
    /// <summary>
    /// The current perceptual context of the agent.
    /// </summary>
    public PerceptionContext PerceptionContext { get; private set; } = new();
    
    /// <summary>
    /// Current position of the agent in the world.
    /// </summary>
    public Vector3 Position { get; set; } = new(0, 0, 0);

    /// <summary>
    /// The name of the agent.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// The current state of the agent's state machine.
    /// </summary>
    public AgentState CurrentState => _currentState;

    /// <summary>
    /// Raised when the agent executes an action.
    /// </summary>
    public event Action<Agent, GoapAction>? OnActionExecuted;
    
    /// <summary>
    /// Raised when the agent changes state.
    /// </summary>
    public event Action<Agent, AgentState, AgentState>? OnStateChanged;
    
    /// <summary>
    /// Raised when the agent completes a plan.
    /// </summary>
    public event Action<Agent, List<GoapAction>>? OnPlanCompleted;
    
    /// <summary>
    /// Raised when the agent fails to complete a plan.
    /// </summary>
    public event Action<Agent, List<GoapAction>>? OnPlanFailed;

    /// <summary>
    /// Creates a new agent with the specified name.
    /// </summary>
    /// <param name="name">The name of the agent.</param>
    /// <param name="planner">The GOAP planner to use for action planning.</param>
    public Agent(string name, GoapPlanner planner)
    {
        _name = name;
        _planner = planner;
    }

    /// <summary>
    /// Registers a sensor with the agent.
    /// </summary>
    /// <param name="key">The key to identify the sensor.</param>
    /// <param name="sensor">The sensor to register.</param>
    public void RegisterSensor<T>(string key, ISensor<T> sensor) where T : class
    {
        _sensors[key] = (ISensor<object>)(object)sensor;
    }

    /// <summary>
    /// Registers a checklist condition with the agent.
    /// </summary>
    /// <param name="condition">The condition to register.</param>
    public void RegisterCondition(IChecklistCondition condition)
    {
        _checklistConditions.Add(condition);
    }

    /// <summary>
    /// Registers an action that the agent can perform.
    /// </summary>
    /// <param name="action">The action to register.</param>
    public void RegisterAction(GoapAction action)
    {
        _availableActions.Add(action);
    }

    /// <summary>
    /// Sets the agent's state.
    /// </summary>
    /// <param name="newState">The new state.</param>
    public void SetState(AgentState newState)
    {
        if (_currentState == newState) return;
        
        var oldState = _currentState;
        _currentState = newState;
        OnStateChanged?.Invoke(this, oldState, newState);
        
        // When state changes, abandon current plan
        _currentPlan.Clear();
        _currentActionIndex = 0;
    }

    /// <summary>
    /// Updates the agent's perception and world state.
    /// </summary>
    public void UpdatePerception()
    {
        PerceptionContext = new PerceptionContext();
        
        // Update perception context from all sensors
        foreach (var (key, sensor) in _sensors)
        {
            var result = sensor.Sense();
            if (result is List<(Item, int)> inventoryItems)
            {
                PerceptionContext.InventoryItems = inventoryItems;
            }
            else if (result is Dictionary<StatType, float> stats)
            {
                PerceptionContext.Stats = stats;
            }
            else if (result is List<GameObject> visibleObjects)
            {
                PerceptionContext.VisibleObjects = visibleObjects;
            }
            // Add more result types as needed
        }
        
        // Update world state from checklist conditions
        foreach (var condition in _checklistConditions)
        {
            WorldState[condition.Key] = condition.Evaluate(PerceptionContext);
        }
    }

    /// <summary>
    /// Plans a sequence of actions to achieve the specified goal.
    /// </summary>
    /// <param name="goal">The goal to achieve.</param>
    /// <returns>Whether a plan was successfully created.</returns>
    public bool PlanForGoal(Dictionary<string, bool> goal)
    {
        UpdatePerception();
        
        _currentPlan = _planner.Plan(WorldState, goal, _availableActions);
        _currentActionIndex = 0;
        
        return _currentPlan.Count > 0;
    }

    /// <summary>
    /// Executes the next action in the current plan.
    /// </summary>
    /// <returns>Whether an action was executed.</returns>
    public bool ExecuteNextAction()
    {
        if (_currentPlan.Count == 0 || _currentActionIndex >= _currentPlan.Count)
        {
            return false;
        }
        
        var action = _currentPlan[_currentActionIndex];
        
        if (action.CanExecute())
        {
            action.Execute();
            OnActionExecuted?.Invoke(this, action);
            _currentActionIndex++;
            
            // Check if plan is complete
            if (_currentActionIndex >= _currentPlan.Count)
            {
                var completedPlan = new List<GoapAction>(_currentPlan);
                OnPlanCompleted?.Invoke(this, completedPlan);
            }
            
            return true;
        }
        else
        {
            // If action can't execute, plan failed
            var failedPlan = new List<GoapAction>(_currentPlan);
            _currentPlan.Clear();
            _currentActionIndex = 0;
            OnPlanFailed?.Invoke(this, failedPlan);
            return false;
        }
    }

    /// <summary>
    /// Updates the agent, including perception, planning and execution.
    /// </summary>
    /// <param name="goal">The goal to achieve if no plan is active.</param>
    public void Update(Dictionary<string, bool> goal)
    {
        UpdatePerception();
        
        // Check if we need a new plan
        if (_currentPlan.Count == 0 || _currentActionIndex >= _currentPlan.Count)
        {
            PlanForGoal(goal);
        }
        
        ExecuteNextAction();
    }

    /// <summary>
    /// Asynchronously waits for the current plan to complete.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the plan is finished.</returns>
    public async Task WaitForPlanCompletionAsync(CancellationToken cancellationToken = default)
    {
        while (_currentPlan.Count > 0 && _currentActionIndex < _currentPlan.Count)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
                
            await Task.Delay(100, cancellationToken);
            ExecuteNextAction();
        }
    }
}

/// <summary>
/// Represents the possible states of an agent.
/// </summary>
public enum AgentState
{
    /// <summary>
    /// The agent is idle and has no current goal.
    /// </summary>
    Idle,
    
    /// <summary>
    /// The agent is engaged in combat.
    /// </summary>
    Combat,
    
    /// <summary>
    /// The agent is focused on survival needs.
    /// </summary>
    Survival,
    
    /// <summary>
    /// The agent is fleeing from danger.
    /// </summary>
    Fleeing,
    
    /// <summary>
    /// The agent is working on a specific task.
    /// </summary>
    Working,
    
    /// <summary>
    /// The agent is socializing with others.
    /// </summary>
    Socializing,
    
    /// <summary>
    /// The agent is resting.
    /// </summary>
    Resting
}