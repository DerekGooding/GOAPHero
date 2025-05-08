namespace GOAPHero.Core;

/// <summary>
/// Defines the various types of items in the game.
/// </summary>
public enum ItemType
{
    /// <summary>
    /// Items that can be consumed for sustenance.
    /// </summary>
    Food,

    /// <summary>
    /// Items that can be used in combat.
    /// </summary>
    Weapon,

    /// <summary>
    /// Items that can be used for various tasks.
    /// </summary>
    Tool,

    /// <summary>
    /// Items used for healing or treating injuries.
    /// </summary>
    Medical
}