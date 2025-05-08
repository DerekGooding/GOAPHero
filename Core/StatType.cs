namespace GOAPHero.Core;

/// <summary>
/// Defines the various types of stats that an agent can have.
/// </summary>
public enum StatType
{
    /// <summary>
    /// Represents the health of an agent.
    /// </summary>
    Health,

    /// <summary>
    /// Represents the stamina or energy of an agent.
    /// </summary>
    Stamina,

    /// <summary>
    /// Represents the hunger level of an agent.
    /// </summary>
    Hunger,

    /// <summary>
    /// Represents the thirst level of an agent.
    /// </summary>
    Thirst,

    /// <summary>
    /// Represents the morale or mental state of an agent.
    /// </summary>
    Morale
}