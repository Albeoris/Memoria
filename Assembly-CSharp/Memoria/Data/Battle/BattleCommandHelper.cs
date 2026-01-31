using FF9;
using Memoria.Database;
using Memoria.Prime;
using NCalc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Memoria.Data
{
    public static class BattleCommandHelper
    {
        public static void ParseCommandFeature(String input)
        {
            ParseCommandFeature(BattleCommandId.None, input);
        }

        public static void ParseCommandFeature(BattleCommandId cmdId, String input)
        {
            FeatureSet set;
            if (cmdId == BattleCommandId.None)
            {
                // A feature that relies on their "Condition" to discriminate whether it applies or not
                set = new FeatureSet();
                FlexibleFeatures.Add(set);
            }
            else
            {
                // A feature linked to a particular Command ID
                if (!CommandFeatures.TryGetValue(cmdId, out set))
                {
                    set = new FeatureSet();
                    CommandFeatures.Add(cmdId, set);
                }
            }
            foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(input))
            {
                String codeType = formula.Groups[1].Value;
                String codeContent = formula.Groups[2].Value;

                if (String.Equals(codeType, "Condition"))
                {
                    set.Condition = codeContent;
                    set.ConditionExpression = NCalcUtility.PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "Patch"))
                {
                    set.CommandPatch = codeContent;
                    if (cmdId == BattleCommandId.None)
                        Log.Warning($"[{nameof(BattleCommandHelper)}] \"Patch\" cannot be used as a Global CMD feature");
                    else
                        set.CommandPatchExpression = NCalcUtility.PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "Disable"))
                {
                    set.CommandDisable = codeContent;
                    set.CommandDisableExpression = NCalcUtility.PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "HardDisable"))
                {
                    set.CommandHardDisable = codeContent;
                    set.CommandHardDisableExpression = NCalcUtility.PrepareExpression(codeContent);
                }
            }
        }

        public static void ClearCommandFeature(BattleCommandId cmdId)
        {
            CommandFeatures.Remove(cmdId);
        }

        public static void ClearFlexibleCommandFeature()
        {
            FlexibleFeatures.Clear();
        }

        public static Int32 GetCommandEnabledState(BattleCommandId cmdId, BattleCommandMenu menu, PLAYER character, BattleUnit asUnit = null)
        {
            if (cmdId == BattleCommandId.None)
                return 0;
            Int32 enabledState = 2;
            try
            {
                foreach (FeatureSet feat in GetApplicableFeatures(cmdId, menu, character, asUnit))
                {
                    enabledState = Math.Min(enabledState, feat.GetEnabledState(cmdId, menu, character, asUnit));
                    if (enabledState == 0)
                        return enabledState;
                }
                if (asUnit != null && enabledState > 1 && CharacterCommands.Commands.TryGetValue(cmdId, out CharacterCommand ff9command))
                {
                    if (ff9command.Type == CharacterCommandType.Normal || ff9command.Type == CharacterCommandType.Throw || ff9command.Type == CharacterCommandType.Instant)
                    {
                        BattleAbilityId uniqueAbilId = ff9command.GetAbilityId();
                        BattleAbilityId patchedId = BattleAbilityHelper.Patch(uniqueAbilId, character);
                        AA_DATA patchedAbil = FF9StateSystem.Battle.FF9Battle.aa_data[patchedId];
                        if (BattleAbilityHelper.IsAbilityDisabled(patchedId, asUnit, cmdId, menu))
                            enabledState = Math.Min(enabledState, 1);
                    }
                }
                if (asUnit != null && enabledState > 1 && asUnit.IsMonsterTransform)
                {
                    BTL_DATA.MONSTER_TRANSFORM transform = asUnit.Data.monster_transform;
                    if (cmdId == BattleCommandId.Attack && transform.attack[asUnit.Data.bi.def_idle] == null)
                        enabledState = Math.Min(enabledState, 1);
                    else if (transform.disable_commands.Contains(cmdId))
                        enabledState = Math.Min(enabledState, 1);
                }
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return enabledState;
        }

        public static BattleCommandId Patch(BattleCommandId cmdId, BattleCommandMenu menu, PLAYER character, BattleUnit asUnit = null)
        {
            try
            {
                // Flexible features are not taken into account, and there cannot be any condition tied to a patch
                // Using a NCalc formula that conditionally returns -1 can be used instead of a condition
                if (cmdId != BattleCommandId.None && CommandFeatures.TryGetValue(cmdId, out FeatureSet cmdSet))
                    cmdId = cmdSet.ApplyPatch(cmdId, menu, character, asUnit);
                if (asUnit != null && asUnit.IsMonsterTransform && cmdId == asUnit.Data.monster_transform.base_command)
                    cmdId = asUnit.Data.monster_transform.new_command;
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return cmdId;
        }

        private static IEnumerable<FeatureSet> GetApplicableFeatures(BattleCommandId cmdId, BattleCommandMenu menu, PLAYER character, BattleUnit asUnit = null)
        {
            if (cmdId < BattleCommandId.SysEscape || cmdId > BattleCommandId.SysStone)
                foreach (FeatureSet flexiSet in FlexibleFeatures)
                    if (flexiSet.CheckCondition(cmdId, menu, character, asUnit))
                        yield return flexiSet;
            if (cmdId != BattleCommandId.None && CommandFeatures.TryGetValue(cmdId, out FeatureSet cmdSet) && cmdSet.CheckCondition(cmdId, menu, character, asUnit))
                yield return cmdSet;
            yield break;
        }

        private class FeatureSet
        {
            public String Condition = null;
            public Expression ConditionExpression = null;

            public String CommandPatch = null;
            public Expression CommandPatchExpression = null;

            public String CommandDisable = null;
            public Expression CommandDisableExpression = null;

            public String CommandHardDisable = null;
            public Expression CommandHardDisableExpression = null;

            public Boolean CheckCondition(BattleCommandId cmdId, BattleCommandMenu menu, PLAYER character, BattleUnit asUnit = null)
            {
                if (ConditionExpression == null)
                    return cmdId != BattleCommandId.None;

                Expression c = ConditionExpression;
                InitializeExpression(ref c, cmdId, menu, character, asUnit);
                return NCalcUtility.EvaluateNCalcCondition(c.Evaluate());
            }

            public BattleCommandId ApplyPatch(BattleCommandId cmdId, BattleCommandMenu menu, PLAYER character, BattleUnit asUnit = null)
            {
                if (CommandPatchExpression != null)
                {
                    Expression e = CommandPatchExpression;
                    InitializeExpression(ref e, cmdId, menu, character, asUnit);
                    Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (val >= 0 && CharacterCommands.Commands.ContainsKey((BattleCommandId)val))
                        return (BattleCommandId)val;
                }
                return cmdId;
            }

            public Int32 GetEnabledState(BattleCommandId cmdId, BattleCommandMenu menu, PLAYER character, BattleUnit asUnit = null)
            {
                if (CommandHardDisableExpression != null)
                {
                    Expression e = CommandHardDisableExpression;
                    InitializeExpression(ref e, cmdId, menu, character, asUnit);
                    if (NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), false))
                        return 0;
                }
                if (CommandDisableExpression != null)
                {
                    Expression e = CommandDisableExpression;
                    InitializeExpression(ref e, cmdId, menu, character, asUnit);
                    if (NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), false))
                        return 1;
                }
                return 2;
            }

            private void InitializeExpression(ref Expression e, BattleCommandId cmdId, BattleCommandMenu menu, PLAYER character, BattleUnit asUnit = null)
            {
                if (asUnit == null)
                {
                    NCalcUtility.InitializeExpressionNullableUnit(ref e, asUnit);
                    NCalcUtility.InitializeExpressionPlayer(ref e, character);
                }
                else
                {
                    NCalcUtility.InitializeExpressionPlayer(ref e, character);
                    NCalcUtility.InitializeExpressionUnit(ref e, asUnit);
                }
                e.Parameters["CommandId"] = (Int32)cmdId;
                e.Parameters["CommandMenu"] = (Int32)menu;
            }
        }

        private static Dictionary<BattleCommandId, FeatureSet> CommandFeatures = new Dictionary<BattleCommandId, FeatureSet>();
        private static List<FeatureSet> FlexibleFeatures = new List<FeatureSet>();
    }
}
