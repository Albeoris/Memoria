using System;
using Memoria.BattleCommands.Data;

namespace Memoria.BattleCommands.Core
{
    /// <summary>
    /// Provides core battle command management functionality including command initialization, cleanup, and processing.
    /// This service contains the extracted and refactored battle command logic from the original btl_cmd class.
    /// </summary>
    public class BattleCommandService
    {
        /// <summary>
        /// Clears all data from a battle command, resetting it to default state.
        /// </summary>
        /// <param name="command">The command data to clear. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
        public static void ClearCommand(CommandData command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            command.Next = null;
            command.TargetId = 0;
            command.MagicCasterId = 0;
            command.CommandId = BattleCommandId.None;
            command.SubNumber = 0;
            command.Info.Reset();
        }

        /// <summary>
        /// Clears the reflection data for a battle command, removing all reflection target information.
        /// </summary>
        /// <param name="command">The command whose reflection data should be cleared. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
        public static void ClearReflectionData(CommandData command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            for (Int32 index = 0; index < command.ReflectionData.TargetIds.Length; ++index)
                command.ReflectionData.TargetIds[index] = 0;
        }

        /// <summary>
        /// Validates that a command data object is in a proper state for execution.
        /// </summary>
        /// <param name="command">The command to validate.</param>
        /// <returns>True if the command is valid for execution, false otherwise.</returns>
        public static Boolean ValidateCommand(CommandData command)
        {
            if (command == null)
                return false;

            // Basic validation - command must have a valid ID and target
            return command.CommandId != BattleCommandId.None && command.TargetId != 0;
        }

        /// <summary>
        /// Creates a new command data object with default initialization.
        /// </summary>
        /// <returns>A new CommandData instance ready for use.</returns>
        public static CommandData CreateCommand()
        {
            return new CommandData();
        }

        /// <summary>
        /// Creates a copy of an existing command data object.
        /// </summary>
        /// <param name="source">The source command to copy. Cannot be null.</param>
        /// <returns>A new CommandData instance with the same values as the source.</returns>
        /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
        public static CommandData CopyCommand(CommandData source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var copy = new CommandData
            {
                TargetId = source.TargetId,
                MagicCasterId = source.MagicCasterId,
                CommandId = source.CommandId,
                SubNumber = source.SubNumber,
                IsShortRange = source.IsShortRange,
                HitRate = source.HitRate,
                Power = source.Power,
                ScriptId = source.ScriptId
            };

            // Copy command info
            copy.Info.Cursor = source.Info.Cursor;
            copy.Info.Status = source.Info.Status;
            copy.Info.Priority = source.Info.Priority;
            copy.Info.Cover = source.Info.Cover;
            copy.Info.Dodge = source.Info.Dodge;
            copy.Info.Reflection = source.Info.Reflection;
            copy.Info.MeteorMiss = source.Info.MeteorMiss;
            copy.Info.ShortSummon = source.Info.ShortSummon;
            copy.Info.MonsterReflection = source.Info.MonsterReflection;
            copy.Info.CommandMotion = source.Info.CommandMotion;
            copy.Info.EffectCounter = source.Info.EffectCounter;
            copy.Info.CustomMPCost = source.Info.CustomMPCost;
            copy.Info.ReflectNull = source.Info.ReflectNull;
            copy.Info.HasCheckedReflect = source.Info.HasCheckedReflect;

            // Copy reflection data
            for (int i = 0; i < source.ReflectionData.TargetIds.Length; i++)
            {
                copy.ReflectionData.TargetIds[i] = source.ReflectionData.TargetIds[i];
            }

            return copy;
        }
    }
}