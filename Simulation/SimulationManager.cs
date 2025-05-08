namespace GOAPHero.Simulation;
/// <summary>
/// Manages a GOAP simulation with multiple agents and a shared environment.
/// </summary>
public class SimulationManager
{
    private readonly List<Agent> _agents = [];
    private readonly List<GameObject> _worldObjects = [];
    private readonly Dictionary<string, object> _globalState = [];
    private CancellationTokenSource? _simulationCts;

    /// <summary>
    /// Event raised when an agent is added to the simulation.
    /// </summary>
    public event Action<Agent>? AgentAdded;

    /// <summary>
    /// Event raised when an object is added to the world.
    /// </summary>
    public event Action<GameObject>? ObjectAdded;

    /// <summary>
    /// Event raised when the simulation starts.
    /// </summary>
    public event Action? SimulationStarted;

    /// <summary>
    /// Event raised when the simulation stops.
    /// </summary>
    public event Action? SimulationStopped;

    /// <summary>
    /// Event raised when an agent executes an action.
    /// </summary>
    public event Action<Agent, GoapAction>? AgentActionExecuted;

    /// <summary>
    /// Event raised on each simulation tick.
    /// </summary>
    public event Action<float>? SimulationTick;

    /// <summary>
    /// Gets all agents in the simulation.
    /// </summary>
    public IReadOnlyList<Agent> Agents => _agents.AsReadOnly();

    /// <summary>
    /// Gets all objects in the world.
    /// </summary>
    public IReadOnlyList<GameObject> WorldObjects => _worldObjects.AsReadOnly();

    /// <summary>
    /// Gets or sets the simulation time scale.
    /// </summary>
    public float TimeScale { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the simulation tick rate in milliseconds.
    /// </summary>
    public int TickRateMs { get; set; } = 100;

    /// <summary>
    /// Gets whether the simulation is currently running.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Creates a new simulation manager.
    /// </summary>
    public SimulationManager()
    {
    }

    /// <summary>
    /// Adds an agent to the simulation.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    public void AddAgent(Agent agent)
    {
        _agents.Add(agent);
        agent.OnActionExecuted += (a, action) => AgentActionExecuted?.Invoke(a, action);
        AgentAdded?.Invoke(agent);
    }

    /// <summary>
    /// Adds an object to the world.
    /// </summary>
    /// <param name="obj">The object to add.</param>
    public void AddWorldObject(GameObject obj)
    {
        _worldObjects.Add(obj);
        ObjectAdded?.Invoke(obj);
    }

    /// <summary>
    /// Gets a world object by its name.
    /// </summary>
    /// <param name="name">The name of the object to find.</param>
    /// <returns>The object with the specified name, or null if not found.</returns>
    public GameObject? GetWorldObject(string name) => _worldObjects.FirstOrDefault(o => o.Name == name);

    /// <summary>
    /// Gets all world objects within a specified radius of a position.
    /// </summary>
    /// <param name="position">The center position.</param>
    /// <param name="radius">The radius to search within.</param>
    /// <returns>A list of objects within the radius.</returns>
    public List<GameObject> GetNearbyObjects(Vector3 position, float radius)
    => [.. _worldObjects.Where(obj =>
    {
        var dx = position.X - obj.Position.X;
        var dy = position.Y - obj.Position.Y;
        var dz = position.Z - obj.Position.Z;
        var distanceSquared = (dx * dx) + (dy * dy) + (dz * dz);
        return distanceSquared <= radius * radius;
    })];

    /// <summary>
    /// Gets or sets a value in the global state.
    /// </summary>
    /// <param name="key">The key to get or set.</param>
    /// <returns>The value for the specified key.</returns>
    public object? this[string key]
    {
        get => _globalState.TryGetValue(key, out var value) ? value : null;
        set
        {
            if (value == null)
                _globalState.Remove(key);
            else
                _globalState[key] = value;
        }
    }

    /// <summary>
    /// Starts the simulation.
    /// </summary>
    public void Start()
    {
        if (IsRunning)
            return;

        IsRunning = true;
        _simulationCts = new CancellationTokenSource();
        Task.Run(() => RunSimulation(_simulationCts.Token));
        SimulationStarted?.Invoke();
    }

    /// <summary>
    /// Stops the simulation.
    /// </summary>
    public void Stop()
    {
        if (!IsRunning)
            return;

        IsRunning = false;
        _simulationCts?.Cancel();
        SimulationStopped?.Invoke();
    }

    /// <summary>
    /// Runs the simulation loop.
    /// </summary>
    /// <param name="token">A cancellation token to stop the simulation.</param>
    private async Task RunSimulation(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                // Calculate delta time based on time scale
                var deltaTime = TickRateMs / 1000.0f * TimeScale;

                // Update all agents
                foreach (var agent in _agents)
                {
                    agent.UpdatePerception();

                    // For simplicity in this example, just execute the next action
                    // In a real simulation, you would have more sophisticated goal selection
                    // based on agent state and current conditions
                    agent.ExecuteNextAction();
                }

                // Trigger tick event
                SimulationTick?.Invoke(deltaTime);

                // Wait for next tick
                await Task.Delay((int)(TickRateMs / TimeScale), token);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error in simulation loop: {ex}");
        }
    }

    /// <summary>
    /// Removes all agents and objects from the simulation.
    /// </summary>
    public void Reset()
    {
        Stop();
        _agents.Clear();
        _worldObjects.Clear();
        _globalState.Clear();
    }
}