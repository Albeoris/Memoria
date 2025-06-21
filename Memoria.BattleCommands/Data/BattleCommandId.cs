namespace Memoria.BattleCommands.Data
{
    /// <summary>
    /// Represents battle command identifiers used in combat system.
    /// </summary>
    public enum BattleCommandId : int
    {
        /// <summary>
        /// No command specified.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Basic attack command.
        /// </summary>
        Attack = 1,
        
        /// <summary>
        /// Steal items from enemy.
        /// </summary>
        Steal = 2,
        
        /// <summary>
        /// Jump attack (normal).
        /// </summary>
        Jump = 3,
        
        /// <summary>
        /// Defensive stance.
        /// </summary>
        Defend = 4,
        
        /// <summary>
        /// Escape from battle (unused).
        /// </summary>
        Escape = 5,
        
        /// <summary>
        /// Use an item.
        /// </summary>
        Item = 14,
        
        /// <summary>
        /// Black magic spells.
        /// </summary>
        BlackMagic = 22,
        
        /// <summary>
        /// White magic spells.
        /// </summary>
        WhiteMagicGarnet = 17,
        
        /// <summary>
        /// Blue magic abilities.
        /// </summary>
        BlueMagic = 24
    }
}