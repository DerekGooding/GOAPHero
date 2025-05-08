namespace GOAPHero.Core;

/// <summary>
/// Represents an action in a GOAP system with enhanced functionality.
/// </summary>
/// <remarks>
/// Creates a new action with the specified parameters.
/// </remarks>
/// <param name="name">The name of the action.</param>
/// <param name="preconditions">The conditions that must be true before the action can be executed.</param>
/// <param name="effects">The conditions that will be true after the action is executed.</param>
/// <param name="canExecute">A function that determines whether the action can be executed.</param>
/// <param name="execute">The function that executes the action.</param>
/// <param name="cost">The cost of executing the action.</param>
/// <param name="executeAsync">An async function that executes the action.</param>
/// <param name="context">Optional context data for the action.</param>
public class EnhancedGoapAction(
    string name,
    Dictionary<string, bool> preconditions,
    Dictionary<string, bool> effects,
    Func<bool> canExecute,
    Action execute,
    float cost = 1.0f,
    Func<Task>? executeAsync = null,
    object? context = null) : GoapAction(name, preconditions, effects, canExecute, execute)
{

    /// <summary>
    /// The cost of executing this action. Higher values make the action less desirable.
    /// </summary>
    public float Cost { get; set; } = cost;

    /// <summary>
    /// An asynchronous function that executes the action.
    /// </summary>
    public Func<Task>? ExecuteAsync { get; set; } = executeAsync;

    /// <summary>
    /// Optional context data for the action.
    /// </summary>
    public object? Context { get; set; } = context;

    /// <summary>
    /// The agent currently performing this action.
    /// </summary>
    public Agent? Agent { get; set; }

    /// <summary>
    /// The target object of this action, if any.
    /// </summary>
    public GameObject? Target { get; set; }

    /// <summary>
    /// The time when this action was started.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// The duration of this action in seconds.
    /// </summary>
    public float Duration { get; set; } = 0;

    /// <summary>
    /// The progress of this action as a value between 0 and 1.
    /// </summary>
    public float Progress { get; set; } = 0;

    /// <summary>
    /// Whether this action is currently in progress.
    /// </summary>
    public bool IsInProgress { get; set; } = false;

    /// <summary>
    /// Whether this action has completed.
    /// </summary>
    public bool IsComplete { get; set; } = false;

    /// <summary>
    /// Whether this action has failed.
    /// </summary>
    public bool HasFailed { get; set; } = false;

    /// <summary>
    /// The reason why this action failed, if any.
    /// </summary>
    public string? FailureReason { get; set; }

    /// <summary>
    /// A callback that is invoked when the action completes.
    /// </summary>
    public Action<bool>? OnComplete { get; set; }

    /// <summary>
    /// A callback that is invoked when the action progresses.
    /// </summary>
    public Action<float>? OnProgress { get; set; }

    /// <summary>
    /// Starts the execution of this action.
    /// </summary>
    public void Start()
    {
        StartTime = DateTime.Now;
        IsInProgress = true;
        Progress = 0;
        IsComplete = false;
        HasFailed = false;
        FailureReason = null;
    }

    /// <summary>
    /// Updates the progress of this action based on elapsed time.
    /// </summary>
    /// <returns>Whether the action is complete.</returns>
    public bool UpdateProgress()
    {
        if (!IsInProgress || IsComplete)
            return IsComplete;

        if (Duration <= 0)
        {
            Progress = 1;
            IsComplete = true;
            IsInProgress = false;
            OnProgress?.Invoke(Progress);
            OnComplete?.Invoke(true);
            return true;
        }

        var elapsed = (float)(DateTime.Now - StartTime).TotalSeconds;
        Progress = Math.Min(elapsed / Duration, 1);
        OnProgress?.Invoke(Progress);

        if (Progress >= 1)
        {
            IsComplete = true;
            IsInProgress = false;
            OnComplete?.Invoke(true);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Marks this action as failed.
    /// </summary>
    /// <param name="reason">The reason why the action failed.</param>
    public void Fail(string reason)
    {
        HasFailed = true;
        FailureReason = reason;
        IsInProgress = false;
        IsComplete = true;
        OnComplete?.Invoke(false);
    }

    /// <summary>
    /// Executes this action asynchronously.
    /// </summary>
    /// <returns>A task that completes when the action is done.</returns>
    public async Task ExecuteAsyncTask()
    {
        Start();
        
        if (ExecuteAsync != null)
        {
            await ExecuteAsync();
        }
        else
        {
            Execute();
        }
        
        IsComplete = true;
        IsInProgress = false;
        Progress = 1;
        OnProgress?.Invoke(Progress);
        OnComplete?.Invoke(true);
    }
}