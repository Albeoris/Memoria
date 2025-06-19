using System;

namespace Memoria.BattleCommands.Data
{
    /// <summary>
    /// Represents a battle command with all associated data including targets, abilities, and execution state.
    /// This is a simplified version of the original CMD_DATA for decoupled battle command processing.
    /// </summary>
    public class CommandData
    {
        /// <summary>
        /// Initializes a new instance of the CommandData class.
        /// </summary>
        public CommandData()
        {
            this.Info = new CommandInfo();
            this.ReflectionData = new ReflectionData();
        }

        /// <summary>
        /// Gets or sets the next command in a linked list of commands.
        /// </summary>
        public CommandData Next { get; set; }

        /// <summary>
        /// Gets or sets the target identifier bitmask for this command.
        /// </summary>
        public UInt16 TargetId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the character casting magic through this command.
        /// </summary>
        public UInt16 MagicCasterId { get; set; }

        /// <summary>
        /// Gets or sets the battle command identifier.
        /// </summary>
        public BattleCommandId CommandId { get; set; }

        /// <summary>
        /// Gets or sets the sub-command number for variants of the main command.
        /// </summary>
        public Int32 SubNumber { get; set; }

        /// <summary>
        /// Gets the command selection and execution information.
        /// </summary>
        public CommandInfo Info { get; private set; }

        /// <summary>
        /// Gets the reflection data for magical commands.
        /// </summary>
        public ReflectionData ReflectionData { get; private set; }

        // Additional properties for battle system compatibility
        /// <summary>
        /// Gets or sets whether this is a short-range attack.
        /// </summary>
        public Boolean IsShortRange { get; set; }

        /// <summary>
        /// Gets or sets the hit rate percentage for this command.
        /// </summary>
        public Int32 HitRate { get; set; }

        /// <summary>
        /// Gets or sets the power/damage value for this command.
        /// </summary>
        public Int32 Power { get; set; }

        /// <summary>
        /// Gets or sets the script identifier for custom command behavior.
        /// </summary>
        public Int32 ScriptId { get; set; }
    }
}