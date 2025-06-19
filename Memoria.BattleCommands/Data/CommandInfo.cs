using System;

namespace Memoria.BattleCommands.Data
{
    /// <summary>
    /// Contains command selection and execution information for battle commands.
    /// </summary>
    public class CommandInfo
    {
        /// <summary>
        /// Gets or sets the cursor position for command selection.
        /// </summary>
        public Byte Cursor { get; set; }

        /// <summary>
        /// Gets or sets the status flags for the command.
        /// </summary>
        public Byte Status { get; set; }

        /// <summary>
        /// Gets or sets the priority level of the command.
        /// </summary>
        public Byte Priority { get; set; }

        /// <summary>
        /// Gets or sets the cover flags for defensive actions.
        /// </summary>
        public Byte Cover { get; set; }

        /// <summary>
        /// Gets or sets the dodge flags for evasive actions.
        /// </summary>
        public Byte Dodge { get; set; }

        /// <summary>
        /// Gets or sets the reflection flags for magical commands.
        /// </summary>
        public Byte Reflection { get; set; }

        /// <summary>
        /// Gets or sets the meteor miss flags.
        /// </summary>
        public Byte MeteorMiss { get; set; }

        /// <summary>
        /// Gets or sets flags for short summon animations.
        /// </summary>
        public Byte ShortSummon { get; set; }

        /// <summary>
        /// Gets or sets the monster reflection flags.
        /// </summary>
        public Byte MonsterReflection { get; set; }

        /// <summary>
        /// Gets or sets whether the command involves motion animations.
        /// </summary>
        public Boolean CommandMotion { get; set; }

        /// <summary>
        /// Gets or sets the effect counter for multi-hit attacks.
        /// </summary>
        public Int32 EffectCounter { get; set; }

        /// <summary>
        /// Gets or sets custom MP cost override for the command, if any.
        /// </summary>
        public Int32? CustomMPCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reflection is nullified for this command.
        /// </summary>
        public Boolean ReflectNull { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reflection has been checked for this command.
        /// </summary>
        public Boolean HasCheckedReflect { get; set; }

        /// <summary>
        /// Resets all command information to default values.
        /// </summary>
        public void Reset()
        {
            Cursor = 0;
            Status = 0;
            Priority = 0;
            Cover = 0;
            Dodge = 0;
            Reflection = 0;
            MeteorMiss = 0;
            ShortSummon = 0;
            MonsterReflection = 0;
            CommandMotion = false;
            EffectCounter = 0;
            CustomMPCost = null;
            ReflectNull = false;
            HasCheckedReflect = false;
        }
    }
}