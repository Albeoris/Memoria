using FF9;
using Memoria.Database;
using Memoria.Prime;
using NCalc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Memoria.Data
{
    public static class BattleAbilityHelper
    {
        public static void ParseAbilityFeature(String input)
        {
            ParseAbilityFeature(BattleAbilityId.Void, input);
        }

        public static void ParseAbilityFeature(BattleAbilityId abilId, String input)
        {
            FeatureSet set;
            if (abilId == BattleAbilityId.Void)
            {
                set = new FeatureSet();
                FlexibleFeatures.Add(set);
            }
            else
            {
                if (!AbilityFeatures.TryGetValue(abilId, out set))
                {
                    set = new FeatureSet();
                    AbilityFeatures.Add(abilId, set);
                }
            }

            foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(input))
            {
                String codeType = formula.Groups[1].Value;
                String codeContent = formula.Groups[2].Value;

                if (String.Equals(codeType, "Condition"))
                {
                    set.ConditionExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "Patch"))
                {
                    if (abilId == BattleAbilityId.Void)
                        Log.Warning($"[{nameof(BattleAbilityHelper)}] \"Patch\" cannot be used as a Global AA feature");
                    else
                        set.AbilityPatchExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "Priority"))
                {
                    set.AbilityPriorityExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "Power"))
                {
                    set.AbilityPowerPatchExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "HitRate"))
                {
                    set.AbilityHitRatePatchExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "Element"))
                {
                    set.AbilityElementPatchExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "Status"))
                {
                    set.AbilityStatusPatchExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "Target"))
                {
                    set.AbilityTargetPatchExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "SpecialEffect"))
                {
                    set.AbilitySpecialEffectPatchExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "GilCost"))
                {
                    set.AbilityGilCostExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "MPCost"))
                {
                    set.AbilityMPCostExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "ItemRequirement"))
                {
                    foreach (String part in codeContent.Split(';'))
                    {
                        set.AbilityItemRequirementExpressions.Add(PrepareExpression(part));
                    }
                }
                else if (String.Equals(codeType, "Disable"))
                {
                    set.AbilityDisableExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "HardDisable"))
                {
                    set.AbilityHardDisableExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "DisableInMenu"))
                {
                    set.AbilityDisableInMenuExpression = PrepareExpression(codeContent);
                }
                else if (String.Equals(codeType, "HardDisableInMenu"))
                {
                    set.AbilityHardDisableInMenuExpression = PrepareExpression(codeContent);
                }
            }
        }

        public static void ClearAbilityFeature(BattleAbilityId abilId)
        {
            AbilityFeatures.Remove(abilId);
        }

        public static void ClearFlexibleAbilityFeature()
        {
            FlexibleFeatures.Clear();
        }

        public static Expression PrepareExpression(String code)
        {
            Expression e = new Expression(code);
            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            return e;
        }

        public static Boolean CheckAbilityCondition(BattleAbilityId abilId, BattleUnit caster, BattleCommandId cmdId, BattleCommandMenu menu, AA_DATA ability = null, CMD_DATA cmd = null)
        {
            foreach (FeatureSet feat in GetApplicableFeatures(abilId))
                if (!feat.CheckCondition(abilId, caster, cmdId, menu, ability, cmd))
                    return false;
            return true;
        }

        public static Boolean IsAbilityDisabled(BattleAbilityId abilId, BattleUnit caster, BattleCommandId cmdId, BattleCommandMenu menu)
        {
            return IsAbilityDisabled(abilId, caster, cmdId, menu, false);
        }

        public static Boolean IsAbilityDisabled(BattleAbilityId abilId, BattleUnit caster, BattleCommandId cmdId, BattleCommandMenu menu, Boolean inMenu)
        {
            foreach (FeatureSet feat in GetApplicableFeatures(abilId))
            {
                if (feat.CheckAbilityIsHardDisabled(abilId, caster, cmdId, menu, inMenu))
                    return true;
                if (feat.CheckAbilityIsDisabled(abilId, caster, cmdId, menu, inMenu))
                    return true;
            }
            return false;
        }

        public static Boolean IsAbilityDisabledInMenu(BattleAbilityId abilId, PLAYER player)
        {
            foreach (FeatureSet feat in GetApplicableFeatures(abilId))
            {
                if (feat.CheckAbilityIsHardDisabledInMenu(abilId, player))
                    return true;
                if (feat.CheckAbilityIsDisabledInMenu(abilId, player))
                    return true;
            }
            return false;
        }

        public static Int32 GetActiveAbilityState(BattleAbilityId abilId, BattleUnit caster, BattleCommandId cmdId, BattleCommandMenu menu)
        {
            return GetActiveAbilityState(abilId, caster, cmdId, menu, false);
        }

        public static Int32 GetActiveAbilityState(BattleAbilityId abilId, BattleUnit caster, BattleCommandId cmdId, BattleCommandMenu menu, Boolean inMenu)
        {
            Int32 state = 2;
            foreach (FeatureSet feat in GetApplicableFeatures(abilId))
            {
                if (feat.CheckAbilityIsHardDisabled(abilId, caster, cmdId, menu, inMenu))
                    return 0;
                if (feat.CheckAbilityIsDisabled(abilId, caster, cmdId, menu, inMenu))
                    state = 1;
            }
            return state;
        }

        public static BattleAbilityId Patch(BattleAbilityId abilId, PLAYER character)
        {
            try
            {
                if (abilId != BattleAbilityId.Void && AbilityFeatures.TryGetValue(abilId, out FeatureSet set))
                    abilId = set.ApplyPatch(abilId, character);
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return abilId;
        }

        public static void ApplyBasicModifiers(CMD_DATA cmd)
        {
            try
            {
                BattleAbilityId abilId = (BattleAbilityId)cmd.sub_no;
                foreach (FeatureSet set in GetApplicableFeatures(abilId))
                    set.ApplyBasicModifiers(cmd);
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
        }

        public static Boolean ApplyCommandCondition(CMD_DATA cmd, ref Int64 totalGilCost)
        {
            try
            {
                BattleAbilityId abilId = (BattleAbilityId)cmd.sub_no;
                foreach (FeatureSet set in GetApplicableFeatures(abilId))
                    if (!set.ApplyCommandCondition(cmd, ref totalGilCost))
                        return false;
            }
            catch (Exception err)
            {
                Log.Error(err);
                return true;
            }
            return true;
        }

        public static Int32 ApplyMPCost(Int32 mpCost, BattleAbilityId abilId, BattleUnit potentialCaster, BattleCommandId cmdId, BattleCommandMenu menu, AA_DATA ability = null)
        {
            try
            {
                foreach (FeatureSet set in GetApplicableFeatures(abilId))
                    set.ApplyMPCost(ref mpCost, abilId, potentialCaster, cmdId, menu, ability);
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return mpCost;
        }

        public static void ApplyPriority(CMD_DATA cmd)
        {
            try
            {
                BattleAbilityId abilId = (BattleAbilityId)cmd.sub_no;
                foreach (FeatureSet set in GetApplicableFeatures(abilId))
                    set.ApplyPriority(cmd);
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
        }

        private static IEnumerable<FeatureSet> GetApplicableFeatures(BattleAbilityId abilId)
        {
            foreach (FeatureSet flexiSet in FlexibleFeatures)
                yield return flexiSet;
            if (abilId != BattleAbilityId.Void && AbilityFeatures.TryGetValue(abilId, out FeatureSet abilSet))
                yield return abilSet;
        }

        private class FeatureSet
        {
            public Expression ConditionExpression = null;
            public Expression AbilityPatchExpression = null;
            public Expression AbilityPriorityExpression = null;
            public Expression AbilityPowerPatchExpression = null;
            public Expression AbilityHitRatePatchExpression = null;
            public Expression AbilityElementPatchExpression = null;
            public Expression AbilityStatusPatchExpression = null;
            public Expression AbilityTargetPatchExpression = null;
            public Expression AbilitySpecialEffectPatchExpression = null;
            public Expression AbilityGilCostExpression = null;
            public Expression AbilityMPCostExpression = null;
            public List<Expression> AbilityItemRequirementExpressions = new List<Expression>();

            public Expression AbilityDisableExpression = null;
            public Expression AbilityHardDisableExpression = null;
            public Expression AbilityDisableInMenuExpression = null;
            public Expression AbilityHardDisableInMenuExpression = null;

            public Boolean CheckCondition(BattleAbilityId abilId, BattleUnit caster, BattleCommandId cmdId, BattleCommandMenu menu, AA_DATA ability = null, CMD_DATA cmd = null)
            {
                if (ConditionExpression == null)
                {
                    if (cmd == null && ability == null && abilId == BattleAbilityId.Void)
                        return false;
                    if (ability == null && !FF9BattleDB.CharacterActions.TryGetValue(abilId, out ability))
                        return false;
                    return true;
                }

                Expression c = ConditionExpression;
                NCalcUtility.InitializeExpressionUnit(ref c, caster, "Caster");
                if (cmd != null)
                    NCalcUtility.InitializeExpressionCommand(ref c, new BattleCommand(cmd));
                else if (ability != null)
                    NCalcUtility.InitializeExpressionRawAbility(ref c, ability, abilId);
                else if (FF9BattleDB.CharacterActions.TryGetValue(abilId, out ability))
                    NCalcUtility.InitializeExpressionRawAbility(ref c, ability, abilId);

                c.Parameters["CommandId"] = (Int32)cmdId;
                c.Parameters["CommandMenu"] = (Int32)menu;
                return NCalcUtility.EvaluateNCalcCondition(c.Evaluate());
            }

            public Boolean CheckAbilityIsDisabled(BattleAbilityId abilId, BattleUnit caster, BattleCommandId cmdId, BattleCommandMenu menu, Boolean inMenu)
            {
                if (AbilityDisableExpression == null) return false;
                Expression c = AbilityDisableExpression;
                NCalcUtility.InitializeExpressionUnit(ref c, caster, "Caster");
                NCalcUtility.InitializeExpressionRawAbility(ref c, FF9BattleDB.CharacterActions[abilId], abilId);
                c.Parameters["CommandId"] = (Int32)cmdId;
                c.Parameters["CommandMenu"] = (Int32)menu;
                c.Parameters["CheckInMenu"] = inMenu;
                return NCalcUtility.EvaluateNCalcCondition(c.Evaluate());
            }

            public Boolean CheckAbilityIsHardDisabled(BattleAbilityId abilId, BattleUnit caster, BattleCommandId cmdId, BattleCommandMenu menu, Boolean inMenu)
            {
                if (AbilityHardDisableExpression == null) return false;
                Expression c = AbilityHardDisableExpression;
                NCalcUtility.InitializeExpressionUnit(ref c, caster, "Caster");
                NCalcUtility.InitializeExpressionRawAbility(ref c, FF9BattleDB.CharacterActions[abilId], abilId);
                c.Parameters["CommandId"] = (Int32)cmdId;
                c.Parameters["CommandMenu"] = (Int32)menu;
                c.Parameters["CheckInMenu"] = inMenu;
                return NCalcUtility.EvaluateNCalcCondition(c.Evaluate());
            }

            public Boolean CheckAbilityIsDisabledInMenu(BattleAbilityId abilId, PLAYER play)
            {
                if (AbilityDisableInMenuExpression == null) return false;
                Expression c = AbilityDisableInMenuExpression;
                NCalcUtility.InitializeExpressionPlayer(ref c, play);
                NCalcUtility.InitializeExpressionRawAbility(ref c, FF9BattleDB.CharacterActions[abilId], abilId);
                return NCalcUtility.EvaluateNCalcCondition(c.Evaluate());
            }

            public Boolean CheckAbilityIsHardDisabledInMenu(BattleAbilityId abilId, PLAYER play)
            {
                if (AbilityHardDisableInMenuExpression == null) return false;
                Expression c = AbilityHardDisableInMenuExpression;
                NCalcUtility.InitializeExpressionPlayer(ref c, play);
                NCalcUtility.InitializeExpressionRawAbility(ref c, FF9BattleDB.CharacterActions[abilId], abilId);
                return NCalcUtility.EvaluateNCalcCondition(c.Evaluate());
            }

            public void ApplyBasicModifiers(CMD_DATA cmd)
            {
                if (AbilityPowerPatchExpression != null)
                {
                    Expression e = AbilityPowerPatchExpression;
                    SetCommandParameters(ref e, cmd);
                    Int64 power = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (power >= 0) cmd.Power = (Byte)power;
                }
                if (AbilityHitRatePatchExpression != null)
                {
                    Expression e = AbilityHitRatePatchExpression;
                    SetCommandParameters(ref e, cmd);
                    Int64 hitRate = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (hitRate >= 0) cmd.HitRate = (Byte)hitRate;
                }
                if (AbilityElementPatchExpression != null)
                {
                    Expression e = AbilityElementPatchExpression;
                    SetCommandParameters(ref e, cmd);
                    Int64 element = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (element >= 0)
                    {
                        cmd.Element = (EffectElement)element;
                        cmd.ElementForBonus = cmd.Element;
                    }
                }
                if (AbilityStatusPatchExpression != null)
                {
                    Expression e = AbilityStatusPatchExpression;
                    SetCommandParameters(ref e, cmd);
                    Int64 status = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (status >= 0) cmd.AbilityStatus = (BattleStatus)status;
                }
                if (AbilityTargetPatchExpression != null)
                {
                    Expression e = AbilityTargetPatchExpression;
                    SetCommandParameters(ref e, cmd);
                    Int64 target = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (target >= 0) cmd.tar_id = (UInt16)target;
                }
                if (AbilitySpecialEffectPatchExpression != null)
                {
                    Expression e = AbilitySpecialEffectPatchExpression;
                    SetCommandParameters(ref e, cmd);
                    Int64 vfx = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (vfx >= 0)
                    {
                        cmd.PatchedVfx = (SpecialEffect)vfx;
                        if (cmd.PatchedVfx == SpecialEffect.Meteor__Fail)
                            cmd.info.meteor_miss = 1;
                    }
                }
            }

            public Boolean ApplyCommandCondition(CMD_DATA cmd, ref Int64 totalGilCost)
            {
                if (AbilityItemRequirementExpressions.Count > 0)
                {
                    Int64 itemType = -1;
                    Int64 itemCount = 1;
                    for (Int32 i = 0; i < AbilityItemRequirementExpressions.Count; i++)
                    {
                        Expression e = AbilityItemRequirementExpressions[i];
                        SetCommandParameters(ref e, cmd);
                        if ((i % 2) == 0)
                        {
                            itemType = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                        }
                        else
                        {
                            itemCount = NCalcUtility.ConvertNCalcResult(e.Evaluate(), itemCount);
                            if (itemType >= 0 && ff9item.FF9Item_GetCount((RegularItem)itemType) < itemCount)
                            {
                                UIManager.Battle.SetBattleFollowMessage(BattleMesages.NotEnoughItems);
                                return false;
                            }
                            itemType = -1;
                            itemCount = 1;
                        }
                    }
                    if (itemType >= 0 && ff9item.FF9Item_GetCount((RegularItem)itemType) < itemCount)
                    {
                        UIManager.Battle.SetBattleFollowMessage(BattleMesages.NotEnoughItems);
                        return false;
                    }
                }
                if (AbilityGilCostExpression != null)
                {
                    Expression e = AbilityGilCostExpression;
                    SetCommandParameters(ref e, cmd);
                    Int64 gilCost = NCalcUtility.ConvertNCalcResult(e.Evaluate(), Int64.MinValue);
                    if (gilCost != Int64.MinValue)
                        totalGilCost += gilCost;
                }
                return true;
            }

            public Boolean ApplyMPCost(ref Int32 mpCost, BattleAbilityId abilId, BattleUnit potentialCaster, BattleCommandId cmdId, BattleCommandMenu menu, AA_DATA ability = null)
            {
                if (AbilityMPCostExpression == null) return false;

                if (ability == null)
                {
                    if (abilId == BattleAbilityId.Void) return false;
                    if (!FF9BattleDB.CharacterActions.TryGetValue(abilId, out ability)) return false;
                }

                Expression e = AbilityMPCostExpression;
                NCalcUtility.InitializeExpressionUnit(ref e, potentialCaster, "Caster");
                NCalcUtility.InitializeExpressionRawAbility(ref e, ability, abilId);
                e.Parameters["CommandId"] = (Int32)cmdId;
                e.Parameters["CommandMenu"] = (Int32)menu;
                e.Parameters["MPCost"] = (Int32)mpCost;
                Int64 mp = NCalcUtility.ConvertNCalcResult(e.Evaluate(), Int64.MinValue);
                if (mp != Int64.MinValue)
                {
                    mpCost = (Int32)mp;
                    return true;
                }
                return false;
            }

            public void ApplyPriority(CMD_DATA cmd)
            {
                if (AbilityPriorityExpression != null)
                {
                    Expression e = AbilityPriorityExpression;
                    SetCommandParameters(ref e, cmd);
                    Int64 priority = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (priority >= 0)
                        cmd.info.priority = (Byte)priority;
                }
            }

            public BattleAbilityId ApplyPatch(BattleAbilityId abilId, PLAYER character)
            {
                if (AbilityPatchExpression != null)
                {
                    Expression e = AbilityPatchExpression;
                    NCalcUtility.InitializeExpressionPlayer(ref e, character);
                    Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (val >= 0 && FF9BattleDB.CharacterActions.ContainsKey((BattleAbilityId)val))
                        return (BattleAbilityId)val;
                }
                return abilId;
            }

            private void SetCommandParameters(ref Expression e, CMD_DATA cmd)
            {
                BattleUnit caster = new BattleUnit(cmd.regist);
                NCalcUtility.InitializeExpressionUnit(ref e, caster, "Caster");
                NCalcUtility.InitializeExpressionCommand(ref e, new BattleCommand(cmd));
                e.Parameters["IsSingleTarget"] = Comn.countBits(cmd.tar_id) == 1;
                e.Parameters["IsSelfTarget"] = caster.Id == cmd.tar_id;
                e.Parameters["AreCasterAndTargetEnemies"] = (caster.IsPlayer && (cmd.tar_id & 0xF0) == cmd.tar_id) || (!caster.IsPlayer && (cmd.tar_id & 0xF) == cmd.tar_id);
                e.Parameters["AreCasterAndTargetAllies"] = (caster.IsPlayer && (cmd.tar_id & 0xF) == cmd.tar_id) || (!caster.IsPlayer && (cmd.tar_id & 0x0F) == cmd.tar_id);
            }
        }

        private static Dictionary<BattleAbilityId, FeatureSet> AbilityFeatures = new Dictionary<BattleAbilityId, FeatureSet>();
        private static List<FeatureSet> FlexibleFeatures = new List<FeatureSet>();
    }
}
