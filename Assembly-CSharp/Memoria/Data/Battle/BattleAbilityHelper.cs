using FF9;
using Memoria.Prime;
using NCalc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Memoria.Data
{
	public static class BattleAbilityHelper
	{
		private static Dictionary<BattleAbilityId, String> AbilityPatch = new Dictionary<BattleAbilityId, String>();
		private static Dictionary<BattleAbilityId, String> AbilityPriority = new Dictionary<BattleAbilityId, String>();
		private static Dictionary<BattleAbilityId, String> AbilityPowerPatch = new Dictionary<BattleAbilityId, String>();
		private static Dictionary<BattleAbilityId, String> AbilityHitRatePatch = new Dictionary<BattleAbilityId, String>();
		private static Dictionary<BattleAbilityId, String> AbilityElementPatch = new Dictionary<BattleAbilityId, String>();
		private static Dictionary<BattleAbilityId, String> AbilityStatusPatch = new Dictionary<BattleAbilityId, String>();
		private static Dictionary<BattleAbilityId, String> AbilityTargetPatch = new Dictionary<BattleAbilityId, String>();
		private static Dictionary<BattleAbilityId, String> AbilitySpecialEffectPatch = new Dictionary<BattleAbilityId, String>();
		private static Dictionary<BattleAbilityId, String> AbilityGilCost = new Dictionary<BattleAbilityId, String>();
		private static Dictionary<BattleAbilityId, String> AbilityItemRequirement = new Dictionary<BattleAbilityId, String>();

		public static void ParseAbilityFeature(BattleAbilityId id, String input)
		{
			foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(input))
			{
				if (String.Compare(formula.Groups[1].Value, "Patch") == 0)
					AbilityPatch[id] = formula.Groups[2].Value;
				else if (String.Compare(formula.Groups[1].Value, "Priority") == 0)
					AbilityPriority[id] = formula.Groups[2].Value;
				else if (String.Compare(formula.Groups[1].Value, "Power") == 0)
					AbilityPowerPatch[id] = formula.Groups[2].Value;
				else if (String.Compare(formula.Groups[1].Value, "HitRate") == 0)
					AbilityHitRatePatch[id] = formula.Groups[2].Value;
				else if (String.Compare(formula.Groups[1].Value, "Element") == 0)
					AbilityElementPatch[id] = formula.Groups[2].Value;
				else if (String.Compare(formula.Groups[1].Value, "Status") == 0)
					AbilityStatusPatch[id] = formula.Groups[2].Value;
				else if (String.Compare(formula.Groups[1].Value, "Target") == 0)
					AbilityTargetPatch[id] = formula.Groups[2].Value;
				else if (String.Compare(formula.Groups[1].Value, "SpecialEffect") == 0)
					AbilitySpecialEffectPatch[id] = formula.Groups[2].Value;
				else if (String.Compare(formula.Groups[1].Value, "GilCost") == 0)
					AbilityGilCost[id] = formula.Groups[2].Value;
				else if (String.Compare(formula.Groups[1].Value, "ItemRequirement") == 0)
					AbilityItemRequirement[id] = formula.Groups[2].Value;
			}
		}

		public static void ClearAbilityFeature(BattleAbilityId id)
		{
			AbilityPatch.Remove(id);
			AbilityPriority.Remove(id);
			AbilityPowerPatch.Remove(id);
			AbilityHitRatePatch.Remove(id);
			AbilityElementPatch.Remove(id);
			AbilityStatusPatch.Remove(id);
			AbilityTargetPatch.Remove(id);
			AbilitySpecialEffectPatch.Remove(id);
			AbilityGilCost.Remove(id);
			AbilityItemRequirement.Remove(id);
		}

		public static Boolean ApplySpecialCommandCondition(CMD_DATA cmd)
		{
			BattleAbilityId abilId = btl_util.GetCommandMainActionIndex(cmd);
			if (abilId == BattleAbilityId.Void)
				return true;
			try
			{
				String featureStr;
				if (AbilityPowerPatch.TryGetValue(abilId, out featureStr))
				{
					Expression e = new Expression(featureStr);
					InitCommandExpression(ref e, cmd);
					Int64 power = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
					if (power >= 0)
						cmd.Power = (Byte)power;
				}
				if (AbilityHitRatePatch.TryGetValue(abilId, out featureStr))
				{
					Expression e = new Expression(featureStr);
					InitCommandExpression(ref e, cmd);
					Int64 hitRate = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
					if (hitRate >= 0)
						cmd.HitRate = (Byte)hitRate;
				}
				if (AbilityElementPatch.TryGetValue(abilId, out featureStr))
				{
					Expression e = new Expression(featureStr);
					InitCommandExpression(ref e, cmd);
					Int64 element = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
					if (element >= 0)
					{
						cmd.Element = (EffectElement)element;
						cmd.ElementForBonus = cmd.Element;
					}
				}
				if (AbilityStatusPatch.TryGetValue(abilId, out featureStr))
				{
					Expression e = new Expression(featureStr);
					InitCommandExpression(ref e, cmd);
					Int64 status = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
					if (status >= 0)
						cmd.AbilityStatus = (BattleStatus)status;
				}
				if (AbilityTargetPatch.TryGetValue(abilId, out featureStr))
				{
					Expression e = new Expression(featureStr);
					InitCommandExpression(ref e, cmd);
					Int64 target = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
					if (target >= 0)
						cmd.tar_id = (UInt16)target;
				}
				if (AbilitySpecialEffectPatch.TryGetValue(abilId, out featureStr))
				{
					Expression e = new Expression(featureStr);
					InitCommandExpression(ref e, cmd);
					Int64 vfx = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
					if (vfx >= 0)
					{
						cmd.PatchedVfx = (SpecialEffect)vfx;
						if (cmd.PatchedVfx == SpecialEffect.Meteor__Fail)
							cmd.info.meteor_miss = 1;
					}
				}
				if (AbilityItemRequirement.TryGetValue(abilId, out featureStr))
				{
					String[] featureSplit = featureStr.Split(';');
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
				if (AbilityGilCost.TryGetValue(abilId, out featureStr))
				{
					PARTY_DATA partyState = FF9StateSystem.Common.FF9.party;
					Expression e = new Expression(featureStr);
					InitCommandExpression(ref e, cmd);
					Int64 gilCost = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
					if (gilCost >= 0)
					{
						if (gilCost > partyState.gil)
						{
							UIManager.Battle.SetBattleFollowMessage(BattleMesages.NotEnoughGil);
							return false;
						}
						partyState.gil -= (UInt32)gilCost;
					}
				}
			}
			catch (Exception err)
			{
				Log.Error(err);
			}
			return true;
		}

		public static void SetCustomPriority(CMD_DATA cmd)
		{
			BattleAbilityId abilId = btl_util.GetCommandMainActionIndex(cmd);
			if (abilId == BattleAbilityId.Void)
				return;
			String featureStr;
			if (AbilityPriority.TryGetValue(abilId, out featureStr))
			{
				try
				{
					Expression e = new Expression(featureStr);
					InitCommandExpression(ref e, cmd);
					Int64 priority = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
					if (priority >= 0)
						cmd.info.priority = (Byte)priority;
				}
				catch (Exception err)
				{
					Log.Error(err);
				}
			}
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

		public static BattleAbilityId Patch(BattleAbilityId id, PLAYER character)
		{
			String patchStr;
			if (AbilityPatch.TryGetValue(id, out patchStr))
			{
				try
				{
					Expression e = new Expression(patchStr);
					e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
					e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
					NCalcUtility.InitializeExpressionPlayer(ref e, character);
					Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
					if (val >= 0)
						return (BattleAbilityId)val;
				}
				catch (Exception err)
				{
					Log.Error(err);
				}
			}
			return id;
		}
	}
}
