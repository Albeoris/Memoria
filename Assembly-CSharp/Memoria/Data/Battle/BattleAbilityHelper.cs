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
                // A feature that relies on their "Condition" to discriminate whether they apply or not
                set = new FeatureSet();
                FlexibleFeatures.Add(set);
            }
            else
            {
                // A feature linked to a particular Ability ID
                if (!AbilityFeatures.TryGetValue(abilId, out set))
                {
                    set = new FeatureSet();
                    AbilityFeatures.Add(abilId, set);
                }
            }
            foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(input))
            {
                if (String.Equals(formula.Groups[1].Value, "Condition"))
                    set.Condition = formula.Groups[2].Value;
                else if (String.Equals(formula.Groups[1].Value, "Patch"))
                {
                    set.AbilityPatch = formula.Groups[2].Value;
                    if (abilId == BattleAbilityId.Void)
                        Log.Warning($"[{nameof(BattleAbilityHelper)}] \"AbilityPatch\" cannot be used as a Global AA feature");
                }
                else if (String.Equals(formula.Groups[1].Value, "Priority"))
                    set.AbilityPriority = formula.Groups[2].Value;
                else if (String.Equals(formula.Groups[1].Value, "Power"))
                    set.AbilityPowerPatch = formula.Groups[2].Value;
                else if (String.Equals(formula.Groups[1].Value, "HitRate"))
                    set.AbilityHitRatePatch = formula.Groups[2].Value;
                else if (String.Equals(formula.Groups[1].Value, "Element"))
                    set.AbilityElementPatch = formula.Groups[2].Value;
                else if (String.Equals(formula.Groups[1].Value, "Status"))
                    set.AbilityStatusPatch = formula.Groups[2].Value;
                else if (String.Equals(formula.Groups[1].Value, "Target"))
                    set.AbilityTargetPatch = formula.Groups[2].Value;
                else if (String.Equals(formula.Groups[1].Value, "SpecialEffect"))
                    set.AbilitySpecialEffectPatch = formula.Groups[2].Value;
                else if (String.Equals(formula.Groups[1].Value, "GilCost"))
                    set.AbilityGilCost = formula.Groups[2].Value;
                else if (String.Equals(formula.Groups[1].Value, "MPCost"))
                    set.AbilityMPCost = formula.Groups[2].Value;
                else if (String.Equals(formula.Groups[1].Value, "ItemRequirement"))
                    set.AbilityItemRequirement = formula.Groups[2].Value;
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

        public static Boolean ApplySpecialCommandCondition(CMD_DATA cmd)
        {
            try
            {
                BattleAbilityId abilId = btl_util.GetCommandMainActionIndex(cmd);
                Int64 gilCost = 0;
                foreach (FeatureSet feat in GetApplicableFeatures(abilId, new BattleUnit(cmd.regist), cmd: cmd))
                {
                    feat.ApplyBasicModifiers(cmd);
                    if (!feat.ApplyCommandCondition(cmd, ref gilCost))
                        return false;
                }
                PARTY_DATA ff9party = FF9StateSystem.Common.FF9.party;
                if (gilCost > ff9party.gil)
                {
                    UIManager.Battle.SetBattleFollowMessage(BattleMesages.NotEnoughGil);
                    return false;
                }
                ff9party.gil -= (UInt32)gilCost;
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return true;
        }

        public static Boolean GetPatchedMPCost(BattleAbilityId abilId, BattleUnit potentialCaster, ref Int32 mpCost, AA_DATA ability = null, CMD_DATA cmd = null, BattleCommandMenu menu = BattleCommandMenu.None)
        {
            Boolean changeMPCost = false;
            try
            {
                foreach (FeatureSet feat in GetApplicableFeatures(abilId, potentialCaster, ability: ability, cmd: cmd, menu: menu))
                    if (feat.ApplyMPCost(abilId, potentialCaster, ref mpCost, ability: ability, cmd: cmd, menu: menu))
                        changeMPCost = true;
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return changeMPCost;
        }

        public static void SetCustomPriority(CMD_DATA cmd)
        {
            try
            {
                BattleAbilityId abilId = btl_util.GetCommandMainActionIndex(cmd);
                foreach (FeatureSet feat in GetApplicableFeatures(abilId, new BattleUnit(cmd.regist), cmd: cmd))
                    feat.ApplyPriority(cmd);
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
        }

        public static BattleAbilityId Patch(BattleAbilityId abilId, PLAYER character)
        {
            try
            {
                // Flexible features are not taken into account, and there cannot be any condition tied to a patch
                // Using a NCalc formula that conditionally returns -1 can be used instead of a condition
                if (abilId != BattleAbilityId.Void && AbilityFeatures.TryGetValue(abilId, out FeatureSet abilSet))
                    return abilSet.ApplyPatch(abilId, character);
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return abilId;
        }

        private static IEnumerable<FeatureSet> GetApplicableFeatures(BattleAbilityId abilId, BattleUnit caster, AA_DATA ability = null, CMD_DATA cmd = null, BattleCommandMenu menu = BattleCommandMenu.None)
        {
            foreach (FeatureSet flexiSet in FlexibleFeatures)
                if (flexiSet.CheckCondition(abilId, caster, ability, cmd, menu))
                    yield return flexiSet;
            if (abilId != BattleAbilityId.Void && AbilityFeatures.TryGetValue(abilId, out FeatureSet abilSet) && abilSet.CheckCondition(abilId, caster, ability, cmd, menu))
                yield return abilSet;
            yield break;
        }

        private class FeatureSet
        {
            public String Condition = null;
            public String AbilityPatch = null;
            public String AbilityPriority = null;
            public String AbilityPowerPatch = null;
            public String AbilityHitRatePatch = null;
            public String AbilityElementPatch = null;
            public String AbilityStatusPatch = null;
            public String AbilityTargetPatch = null;
            public String AbilitySpecialEffectPatch = null;
            public String AbilityGilCost = null;
            public String AbilityMPCost = null;
            public String AbilityItemRequirement = null;

            public Boolean CheckCondition(BattleAbilityId abilId, BattleUnit caster, AA_DATA ability = null, CMD_DATA cmd = null, BattleCommandMenu menu = BattleCommandMenu.None)
            {
                if (String.IsNullOrEmpty(Condition))
                    return true;
                if (cmd == null && ability == null && abilId == BattleAbilityId.Void)
                    return false;
                Expression c = new Expression(Condition);
                NCalcUtility.InitializeExpressionUnit(ref c, caster, "Caster");
                if (cmd != null)
                {
                    NCalcUtility.InitializeExpressionCommand(ref c, new BattleCommand(cmd));
                    c.Parameters["CommandId"] = (Int32)cmd.cmd_no;
                    c.Parameters["CommandMenu"] = (Int32)cmd.info.cmdMenu;
                }
                else
                {
                    if (ability != null)
                        NCalcUtility.InitializeExpressionRawAbility(ref c, ability, abilId);
                    else
                        NCalcUtility.InitializeExpressionRawAbility(ref c, FF9BattleDB.CharacterActions[abilId], abilId);
                    if (menu == BattleCommandMenu.Ability1 && caster.IsPlayer)
                        c.Parameters["CommandId"] = (Int32)CharacterCommands.CommandSets[caster.Player.PresetId].Get(caster.IsUnderAnyStatus(BattleStatus.Trance), 0);
                    else if (menu == BattleCommandMenu.Ability2 && caster.IsPlayer)
                        c.Parameters["CommandId"] = (Int32)CharacterCommands.CommandSets[caster.Player.PresetId].Get(caster.IsUnderAnyStatus(BattleStatus.Trance), 1);
                    else
                        c.Parameters["CommandId"] = (Int32)BattleCommandId.None;
                    c.Parameters["CommandMenu"] = (Int32)menu;
                }
                c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                return NCalcUtility.EvaluateNCalcCondition(c.Evaluate());
            }

            public void ApplyBasicModifiers(CMD_DATA cmd)
            {
                if (!String.IsNullOrEmpty(AbilityPowerPatch))
                {
                    Expression e = new Expression(AbilityPowerPatch);
                    InitCommandExpression(ref e, cmd);
                    Int64 power = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (power >= 0)
                        cmd.Power = (Byte)power;
                }
                if (!String.IsNullOrEmpty(AbilityHitRatePatch))
                {
                    Expression e = new Expression(AbilityHitRatePatch);
                    InitCommandExpression(ref e, cmd);
                    Int64 hitRate = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (hitRate >= 0)
                        cmd.HitRate = (Byte)hitRate;
                }
                if (!String.IsNullOrEmpty(AbilityElementPatch))
                {
                    Expression e = new Expression(AbilityElementPatch);
                    InitCommandExpression(ref e, cmd);
                    Int64 element = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (element >= 0)
                    {
                        cmd.Element = (EffectElement)element;
                        cmd.ElementForBonus = cmd.Element;
                    }
                }
                if (!String.IsNullOrEmpty(AbilityStatusPatch))
                {
                    Expression e = new Expression(AbilityStatusPatch);
                    InitCommandExpression(ref e, cmd);
                    Int64 status = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (status >= 0)
                        cmd.AbilityStatus = (BattleStatus)status;
                }
                if (!String.IsNullOrEmpty(AbilityTargetPatch))
                {
                    Expression e = new Expression(AbilityTargetPatch);
                    InitCommandExpression(ref e, cmd);
                    Int64 target = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (target >= 0)
                        cmd.tar_id = (UInt16)target;
                }
                if (!String.IsNullOrEmpty(AbilitySpecialEffectPatch))
                {
                    Expression e = new Expression(AbilitySpecialEffectPatch);
                    InitCommandExpression(ref e, cmd);
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
                if (!String.IsNullOrEmpty(AbilityItemRequirement))
                {
                    String[] featureSplit = AbilityItemRequirement.Split(';');
                    Int64 itemType = -1;
                    Int64 itemCount = 1;
                    for (Int32 i = 0; i < featureSplit.Length; i++)
                    {
                        Expression e = new Expression(featureSplit[i]);
                        InitCommandExpression(ref e, cmd);
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
                if (!String.IsNullOrEmpty(AbilityGilCost))
                {
                    Expression e = new Expression(AbilityGilCost);
                    InitCommandExpression(ref e, cmd);
                    Int64 gilCost = NCalcUtility.ConvertNCalcResult(e.Evaluate(), Int64.MinValue);
                    if (gilCost != Int64.MinValue)
                        totalGilCost += gilCost;
                }
                return true;
            }

            public Boolean ApplyMPCost(BattleAbilityId abilId, BattleUnit potentialCaster, ref Int32 mpCost, AA_DATA ability = null, CMD_DATA cmd = null, BattleCommandMenu menu = BattleCommandMenu.None)
            {
                if (ability == null && cmd == null)
                {
                    if (abilId == BattleAbilityId.Void)
                        return false;
                    if (!FF9BattleDB.CharacterActions.TryGetValue(abilId, out ability))
                        return false;
                }
                if (ability == null && cmd != null)
                    ability = cmd.aa;
                if (!String.IsNullOrEmpty(AbilityMPCost))
                {
                    Expression e = new Expression(AbilityMPCost);
                    NCalcUtility.InitializeExpressionUnit(ref e, potentialCaster, "Caster");
                    NCalcUtility.InitializeExpressionRawAbility(ref e, ability, abilId);
                    if (cmd != null)
                    {
                        e.Parameters["CommandId"] = (Int32)cmd.cmd_no;
                        e.Parameters["CommandMenu"] = (Int32)cmd.info.cmdMenu;
                    }
                    else
                    {
                        if (menu == BattleCommandMenu.Ability1 && potentialCaster.IsPlayer)
                            e.Parameters["CommandId"] = (Int32)CharacterCommands.CommandSets[potentialCaster.Player.PresetId].Get(potentialCaster.IsUnderAnyStatus(BattleStatus.Trance), 0);
                        else if (menu == BattleCommandMenu.Ability2 && potentialCaster.IsPlayer)
                            e.Parameters["CommandId"] = (Int32)CharacterCommands.CommandSets[potentialCaster.Player.PresetId].Get(potentialCaster.IsUnderAnyStatus(BattleStatus.Trance), 1);
                        else
                            e.Parameters["CommandId"] = (Int32)BattleCommandId.None;
                        e.Parameters["CommandMenu"] = (Int32)menu;
                    }
                    e.Parameters["MPCost"] = (Int32)mpCost;
                    e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                    e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                    Int64 mp = NCalcUtility.ConvertNCalcResult(e.Evaluate(), Int64.MinValue);
                    if (mp != Int64.MinValue)
                    {
                        mpCost = (Int32)mp;
                        return true;
                    }
                }
                return false;
            }

            public void ApplyPriority(CMD_DATA cmd)
            {
                if (!String.IsNullOrEmpty(AbilityPriority))
                {
                    Expression e = new Expression(AbilityPriority);
                    InitCommandExpression(ref e, cmd);
                    Int64 priority = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (priority >= 0)
                        cmd.info.priority = (Byte)priority;
                }
            }

            public BattleAbilityId ApplyPatch(BattleAbilityId abilId, PLAYER character)
            {
                if (!String.IsNullOrEmpty(AbilityPatch))
                {
                    Expression e = new Expression(AbilityPatch);
                    e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                    e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                    NCalcUtility.InitializeExpressionPlayer(ref e, character);
                    Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                    if (val >= 0 && FF9BattleDB.CharacterActions.ContainsKey((BattleAbilityId)val))
                        return (BattleAbilityId)val;
                }
                return abilId;
            }

            private static void InitCommandExpression(ref Expression e, CMD_DATA cmd)
            {
                BattleUnit caster = new BattleUnit(cmd.regist);
                //BattleUnit target = btl_scrp.FindBattleUnitUnlimited((UInt16)Comn.firstBitSet(cmd.tar_id));
                NCalcUtility.InitializeExpressionUnit(ref e, caster, "Caster");
                //NCalcUtility.InitializeExpressionUnit(ref e, target, "Target");
                NCalcUtility.InitializeExpressionCommand(ref e, new BattleCommand(cmd));
                e.Parameters["IsSingleTarget"] = Comn.countBits(cmd.tar_id) == 1;
                e.Parameters["IsSelfTarget"] = caster.Id == cmd.tar_id;
                e.Parameters["AreCasterAndTargetEnemies"] = (caster.IsPlayer && (cmd.tar_id & 0xF0) == cmd.tar_id) || (!caster.IsPlayer && (cmd.tar_id & 0xF) == cmd.tar_id);
                e.Parameters["AreCasterAndTargetAllies"] = (caster.IsPlayer && (cmd.tar_id & 0xF) == cmd.tar_id) || (!caster.IsPlayer && (cmd.tar_id & 0x0F) == cmd.tar_id);
                e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            }
        }

        private static Dictionary<BattleAbilityId, FeatureSet> AbilityFeatures = new Dictionary<BattleAbilityId, FeatureSet>();
        private static List<FeatureSet> FlexibleFeatures = new List<FeatureSet>();
    }
}
