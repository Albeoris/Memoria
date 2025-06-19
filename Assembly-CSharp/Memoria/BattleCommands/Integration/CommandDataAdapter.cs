using Memoria.BattleCommands.Core;
using Memoria.BattleCommands.Data;

namespace Memoria.BattleCommands.Integration
{
    /// <summary>
    /// Adapter class to convert between original CMD_DATA objects and the new CommandData objects.
    /// This preserves compatibility with existing code while enabling the use of the refactored battle command service.
    /// </summary>
    public static class CommandDataAdapter
    {
        /// <summary>
        /// Converts a CMD_DATA object to a CommandData object for use with the new battle command service.
        /// </summary>
        /// <param name="original">The original CMD_DATA object to convert.</param>
        /// <returns>A new CommandData object with equivalent data.</returns>
        public static CommandData ToCommandData(CMD_DATA original)
        {
            if (original == null)
                return null;

            var converted = new CommandData
            {
                TargetId = original.tar_id,
                MagicCasterId = original.magic_caster_id,
                CommandId = ConvertCommandId(original.cmd_no),
                SubNumber = original.sub_no,
                IsShortRange = original.IsShortRange,
                HitRate = original.HitRate,
                Power = original.Power,
                ScriptId = original.ScriptId
            };

            // Convert command info
            if (original.info != null)
            {
                converted.Info.Cursor = original.info.cursor;
                converted.Info.Status = original.info.stat;
                converted.Info.Priority = original.info.priority;
                converted.Info.Cover = original.info.cover;
                converted.Info.Dodge = original.info.dodge;
                converted.Info.Reflection = original.info.reflec;
                converted.Info.MeteorMiss = original.info.meteor_miss;
                converted.Info.ShortSummon = original.info.short_summon;
                converted.Info.MonsterReflection = original.info.mon_reflec;
                converted.Info.CommandMotion = original.info.cmd_motion;
                converted.Info.EffectCounter = original.info.effect_counter;
                converted.Info.CustomMPCost = original.info.CustomMPCost;
                converted.Info.ReflectNull = original.info.ReflectNull;
                converted.Info.HasCheckedReflect = original.info.HasCheckedReflect;
            }

            // Convert reflection data
            if (original.reflec != null && original.reflec.tar_id != null)
            {
                for (int i = 0; i < System.Math.Min(original.reflec.tar_id.Length, converted.ReflectionData.TargetIds.Length); i++)
                {
                    converted.ReflectionData.TargetIds[i] = original.reflec.tar_id[i];
                }
            }

            return converted;
        }

        /// <summary>
        /// Updates a CMD_DATA object with data from a CommandData object after processing.
        /// </summary>
        /// <param name="original">The original CMD_DATA object to update.</param>
        /// <param name="processed">The processed CommandData object containing updated values.</param>
        public static void UpdateFromCommandData(CMD_DATA original, CommandData processed)
        {
            if (original == null || processed == null)
                return;

            original.tar_id = processed.TargetId;
            original.magic_caster_id = processed.MagicCasterId;
            original.cmd_no = ConvertCommandIdBack(processed.CommandId);
            original.sub_no = processed.SubNumber;
            original.IsShortRange = processed.IsShortRange;
            original.HitRate = processed.HitRate;
            original.Power = processed.Power;
            original.ScriptId = processed.ScriptId;

            // Update command info
            if (original.info != null)
            {
                original.info.cursor = processed.Info.Cursor;
                original.info.stat = processed.Info.Status;
                original.info.priority = processed.Info.Priority;
                original.info.cover = processed.Info.Cover;
                original.info.dodge = processed.Info.Dodge;
                original.info.reflec = processed.Info.Reflection;
                original.info.meteor_miss = processed.Info.MeteorMiss;
                original.info.short_summon = processed.Info.ShortSummon;
                original.info.mon_reflec = processed.Info.MonsterReflection;
                original.info.cmd_motion = processed.Info.CommandMotion;
                original.info.effect_counter = processed.Info.EffectCounter;
                original.info.CustomMPCost = processed.Info.CustomMPCost;
                original.info.ReflectNull = processed.Info.ReflectNull;
                original.info.HasCheckedReflect = processed.Info.HasCheckedReflect;
            }

            // Update reflection data
            if (original.reflec != null && original.reflec.tar_id != null)
            {
                for (int i = 0; i < System.Math.Min(original.reflec.tar_id.Length, processed.ReflectionData.TargetIds.Length); i++)
                {
                    original.reflec.tar_id[i] = processed.ReflectionData.TargetIds[i];
                }
            }
        }

        private static Memoria.BattleCommands.Data.BattleCommandId ConvertCommandId(Memoria.Data.BattleCommandId original)
        {
            // Map the commonly used command IDs
            switch (original)
            {
                case Memoria.Data.BattleCommandId.None:
                    return Memoria.BattleCommands.Data.BattleCommandId.None;
                case Memoria.Data.BattleCommandId.Attack:
                    return Memoria.BattleCommands.Data.BattleCommandId.Attack;
                case Memoria.Data.BattleCommandId.Steal:
                    return Memoria.BattleCommands.Data.BattleCommandId.Steal;
                case Memoria.Data.BattleCommandId.Jump:
                    return Memoria.BattleCommands.Data.BattleCommandId.Jump;
                case Memoria.Data.BattleCommandId.Defend:
                    return Memoria.BattleCommands.Data.BattleCommandId.Defend;
                case Memoria.Data.BattleCommandId.Escape:
                    return Memoria.BattleCommands.Data.BattleCommandId.Escape;
                case Memoria.Data.BattleCommandId.Item:
                    return Memoria.BattleCommands.Data.BattleCommandId.Item;
                case Memoria.Data.BattleCommandId.BlackMagic:
                    return Memoria.BattleCommands.Data.BattleCommandId.BlackMagic;
                case Memoria.Data.BattleCommandId.WhiteMagicGarnet:
                    return Memoria.BattleCommands.Data.BattleCommandId.WhiteMagicGarnet;
                case Memoria.Data.BattleCommandId.BlueMagic:
                    return Memoria.BattleCommands.Data.BattleCommandId.BlueMagic;
                default:
                    // For unmapped commands, default to None to prevent issues
                    return Memoria.BattleCommands.Data.BattleCommandId.None;
            }
        }

        private static Memoria.Data.BattleCommandId ConvertCommandIdBack(Memoria.BattleCommands.Data.BattleCommandId processed)
        {
            // Map back to original enum
            switch (processed)
            {
                case Memoria.BattleCommands.Data.BattleCommandId.None:
                    return Memoria.Data.BattleCommandId.None;
                case Memoria.BattleCommands.Data.BattleCommandId.Attack:
                    return Memoria.Data.BattleCommandId.Attack;
                case Memoria.BattleCommands.Data.BattleCommandId.Steal:
                    return Memoria.Data.BattleCommandId.Steal;
                case Memoria.BattleCommands.Data.BattleCommandId.Jump:
                    return Memoria.Data.BattleCommandId.Jump;
                case Memoria.BattleCommands.Data.BattleCommandId.Defend:
                    return Memoria.Data.BattleCommandId.Defend;
                case Memoria.BattleCommands.Data.BattleCommandId.Escape:
                    return Memoria.Data.BattleCommandId.Escape;
                case Memoria.BattleCommands.Data.BattleCommandId.Item:
                    return Memoria.Data.BattleCommandId.Item;
                case Memoria.BattleCommands.Data.BattleCommandId.BlackMagic:
                    return Memoria.Data.BattleCommandId.BlackMagic;
                case Memoria.BattleCommands.Data.BattleCommandId.WhiteMagicGarnet:
                    return Memoria.Data.BattleCommandId.WhiteMagicGarnet;
                case Memoria.BattleCommands.Data.BattleCommandId.BlueMagic:
                    return Memoria.Data.BattleCommandId.BlueMagic;
                default:
                    return Memoria.Data.BattleCommandId.None;
            }
        }
    }
}