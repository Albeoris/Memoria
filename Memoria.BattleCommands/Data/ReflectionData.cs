using System;

namespace Memoria.BattleCommands.Data
{
    /// <summary>
    /// Represents reflection data for battle commands, containing target information for reflected spells.
    /// </summary>
    public class ReflectionData
    {
        /// <summary>
        /// Initializes a new instance of the ReflectionData class.
        /// </summary>
        public ReflectionData()
        {
            this.TargetIds = new UInt16[4];
        }

        /// <summary>
        /// Gets the array of target identifiers for reflected commands. 
        /// Array contains up to 4 possible reflection targets.
        /// </summary>
        public UInt16[] TargetIds { get; private set; }
    }
}