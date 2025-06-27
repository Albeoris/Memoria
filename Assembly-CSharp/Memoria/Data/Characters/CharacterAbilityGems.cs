using FF9;
using Memoria.Prime;
using Memoria.Prime.CSV;
using Memoria.Prime.Text;
using NCalc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Memoria.Data
{
    public sealed class CharacterAbilityGems : ICsvEntry
    {
        public String Comment;
        public SupportAbility Id;

        public Int32 GemsCount;
        public List<SupportAbility> Boosted;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Id = (SupportAbility)CsvParser.Int32(raw[1]);

            GemsCount = CsvParser.Int32(raw[2]);
            Boosted = new List<SupportAbility>();
            if (metadata.HasOption($"Include{nameof(Boosted)}"))
                foreach (Int32 abilId in CsvParser.Int32Array(raw[3]))
                    Boosted.Add((SupportAbility)abilId);
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            writer.String(Comment);
            writer.Int32((Int32)Id);

            writer.Int32(GemsCount);
            if (metadata.HasOption($"Include{nameof(Boosted)}"))
                writer.Int32Array(Boosted.ConvertAll(abilId => (Int32)abilId).ToArray());
        }
    }

    public class SupportingAbilityFeature
    {
        public abstract class SupportingAbilityEffect
        {
            public String Condition = "";
            public String ModFilePath = null;
            public Int32 FeatureLineNumber = -1;
        }

        public class SupportingAbilityEffectPermanent : SupportingAbilityEffect
        {
            public Dictionary<String, String> Formula = new Dictionary<String, String>();
        }
        public class SupportingAbilityEffectBattleStartType : SupportingAbilityEffect
        {
            public Dictionary<String, String> Formula = new Dictionary<String, String>();
            public Int32 PreemptivePriorityDelta = 0;
        }
        public class SupportingAbilityEffectBattleResult : SupportingAbilityEffect
        {
            public String When = "RewardSingle"; // "BattleEnd", "RewardAll", "RewardSingle"
            public Dictionary<String, String> Formula = new Dictionary<String, String>();
        }
        public class SupportingAbilityEffectBattleInitStatus : SupportingAbilityEffect
        {
            public BattleStatus PermanentStatus = 0;
            public BattleStatus InitialStatus = 0;
            public BattleStatus ResistStatus = 0;
            public List<KeyValuePair<BattleStatusId, String>> PartialResistStatus = new List<KeyValuePair<BattleStatusId, String>>();
            public List<KeyValuePair<BattleStatusId, String>> DurationFactorStatus = new List<KeyValuePair<BattleStatusId, String>>();
            public Int32 InitialATB = -1;
        }
        public class SupportingAbilityEffectAbilityUse : SupportingAbilityEffect
        {
            public String When = "EffectDone"; // "BattleScriptStart", "HitRateSetup", "CalcDamage", "Steal", "BattleScriptEnd", "EffectDone"
            public Dictionary<String, String> Effect = new Dictionary<String, String>();
            public Boolean AsTarget = false;
            public Boolean EvenImmobilized = false;
            public List<SupportAbility> DisableSA = new List<SupportAbility>();
        }
        public class SupportingAbilityEffectCommandStart : SupportingAbilityEffect
        {
            public Dictionary<String, String> Effect = new Dictionary<String, String>();
            public Boolean EvenImmobilized = false;
        }

        public SupportAbility Id;
        public List<SupportingAbilityEffectPermanent> PermanentEffect = new List<SupportingAbilityEffectPermanent>();
        public List<SupportingAbilityEffectBattleStartType> BattleStartEffect = new List<SupportingAbilityEffectBattleStartType>();
        public List<SupportingAbilityEffectBattleResult> BattleResultEffect = new List<SupportingAbilityEffectBattleResult>();
        public List<SupportingAbilityEffectBattleInitStatus> StatusEffect = new List<SupportingAbilityEffectBattleInitStatus>();
        public List<SupportingAbilityEffectAbilityUse> AbilityEffect = new List<SupportingAbilityEffectAbilityUse>();
        public List<SupportingAbilityEffectCommandStart> CommandEffect = new List<SupportingAbilityEffectCommandStart>();
        public Boolean EnableAsMonsterTransform = false;
        public Boolean EnableAsEnemy = false;

        public void TriggerSpecialSA(PLAYER play)
        {
            for (Int32 i = 0; i < PermanentEffect.Count; i++)
            {
                try
                {
                    if (!PermanentEffect[i].Formula.ContainsKey("ActivateFreeSA") && !PermanentEffect[i].Formula.ContainsKey("BanishSA") && !PermanentEffect[i].Formula.ContainsKey("HiddenSA")
                        && !PermanentEffect[i].Formula.ContainsKey("ActivateFreeSAByLvl") && !PermanentEffect[i].Formula.ContainsKey("BanishSAByLvl"))
                        continue;
                    if (PermanentEffect[i].Condition.Length > 0)
                    {
                        Expression c = new Expression(PermanentEffect[i].Condition);
                        NCalcUtility.InitializeExpressionPlayer(ref c, play);
                        c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
                    }
                    foreach (KeyValuePair<String, String> formula in PermanentEffect[i].Formula)
                    {
                        String[] featureSplit = formula.Value.Split(';');
                        for (Int32 j = 0; j < featureSplit.Length; j++)
                        {
                            Expression e = new Expression(featureSplit[j]);
                            NCalcUtility.InitializeExpressionPlayer(ref e, play);
                            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                            SupportAbility sa = (SupportAbility)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)SupportAbility.Void);
                            if (sa == SupportAbility.Void)
                                continue;
                            if (String.Equals(formula.Key, "ActivateFreeSA"))
                            {
                                play.saForced.Add(sa);
                            }
                            else if (String.Equals(formula.Key, "BanishSA"))
                            {
                                if (!play.saBanish.Contains(sa))
                                {
                                    ff9abil.DisableHierarchyFromSA(play, sa);
                                    play.saBanish.Add(sa);
                                }
                            }
                            else if (String.Equals(formula.Key, "HiddenSA"))
                            {
                                foreach (SupportAbility saToHide in ff9abil.GetHierarchyFromAnySA(sa))
                                    play.saHidden.Add(saToHide);
                            }
                            else if (String.Equals(formula.Key, "ActivateFreeSAByLvl"))
                            {
                                SupportAbility baseSA = ff9abil.GetBaseAbilityFromBoostedAbility(sa);
                                Int32 levelSA = 0;
                                if (featureSplit.Length > j + 1)
                                {
                                    Expression elvl = new Expression(featureSplit[j + 1]);
                                    NCalcUtility.InitializeExpressionPlayer(ref elvl, play);
                                    elvl.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                                    elvl.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                                    levelSA = Math.Min((Int32)NCalcUtility.ConvertNCalcResult(elvl.Evaluate(), 0), ff9abil.GetBoostedAbilityMaxLevel(play, baseSA));
                                    j++;
                                }
                                Boolean allSA = levelSA == -1;
                                Boolean skip = true;
                                foreach (SupportAbility saToForce in ff9abil.GetHierarchyFromAnySA(baseSA))
                                {
                                    if (skip && saToForce != sa && !allSA)
                                        continue;
                                    if (levelSA < 0 && !allSA)
                                        break;
                                    skip = false;
                                    play.saForced.Add(saToForce);
                                    levelSA--;
                                }
                            }
                            else if (String.Equals(formula.Key, "BanishSAByLvl"))
                            {
                                SupportAbility baseSA = ff9abil.GetBaseAbilityFromBoostedAbility(sa);
                                Int32 levelSA = 0;
                                if (featureSplit.Length > j + 1)
                                {
                                    Expression elvl = new Expression(featureSplit[j + 1]);
                                    NCalcUtility.InitializeExpressionPlayer(ref elvl, play);
                                    elvl.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                                    elvl.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                                    levelSA = (Int32)NCalcUtility.ConvertNCalcResult(elvl.Evaluate(), 0);
                                    j++;
                                }
                                Boolean allSA = levelSA == -1;
                                List<SupportAbility> listSAToBanish = ff9abil.GetHierarchyFromAnySA(baseSA);
                                listSAToBanish.Reverse(); // Start with the maximum boosted one, if it exist.
                                foreach (SupportAbility saToForce in listSAToBanish)
                                {
                                    if (levelSA <= 0 && !allSA)
                                        break;
                                    levelSA--;
                                    play.saBanish.Add(saToForce);
                                }
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    String message = $"Error detected in '{PermanentEffect[i].ModFilePath}' permanent effect at line {PermanentEffect[i].FeatureLineNumber}";
                    if (message != lastErrorMessage)
                    {
                        lastErrorMessage = message;
                        Log.Error(message);
                        Log.Error(err);
                    }
                }
            }
        }

        private static String lastErrorMessage = null;

        public void TriggerOnEnable(PLAYER play)
        {
            for (Int32 i = 0; i < PermanentEffect.Count; i++)
            {
                try
                {
                    if (PermanentEffect[i].Condition.Length > 0)
                    {
                        Expression c = new Expression(PermanentEffect[i].Condition);
                        NCalcUtility.InitializeExpressionPlayer(ref c, play);
                        c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
                    }
                    foreach (KeyValuePair<String, String> formula in PermanentEffect[i].Formula)
                    {
                        Expression e = new Expression(formula.Value);
                        NCalcUtility.InitializeExpressionPlayer(ref e, play);
                        e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (String.Equals(formula.Key, "MaxHP")) play.max.hp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.max.hp);
                        else if (String.Equals(formula.Key, "MaxMP")) play.max.mp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.max.mp);
                        else if (String.Equals(formula.Key, "Speed")) play.elem.dex = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.elem.dex);
                        else if (String.Equals(formula.Key, "Strength")) play.elem.str = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.elem.str);
                        else if (String.Equals(formula.Key, "Magic")) play.elem.mgc = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.elem.mgc);
                        else if (String.Equals(formula.Key, "Spirit")) play.elem.wpr = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.elem.wpr);
                        else if (String.Equals(formula.Key, "Defence")) play.defence.PhysicalDefence = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.PhysicalDefence);
                        else if (String.Equals(formula.Key, "Evade")) play.defence.PhysicalEvade = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.PhysicalEvade);
                        else if (String.Equals(formula.Key, "MagicDefence")) play.defence.MagicalDefence = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.MagicalDefence);
                        else if (String.Equals(formula.Key, "MagicEvade")) play.defence.MagicalEvade = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.MagicalEvade);
                        else if (String.Equals(formula.Key, "PlayerCategory")) play.category = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.category);
                        else if (String.Equals(formula.Key, "MPCostFactor")) play.mpCostFactor = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.mpCostFactor);
                        else if (String.Equals(formula.Key, "MaxHPLimit")) play.maxHpLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.maxHpLimit);
                        else if (String.Equals(formula.Key, "MaxMPLimit")) play.maxMpLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.maxMpLimit);
                        else if (String.Equals(formula.Key, "MaxDamageLimit")) play.maxDamageLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.maxDamageLimit);
                        else if (String.Equals(formula.Key, "MaxMPDamageLimit")) play.maxMpDamageLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.maxMpDamageLimit);
                        else if (String.Equals(formula.Key, "PlayerPermanentStatus")) play.SetPermanentStatus((BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)play.permanent_status));
                        else if (String.Equals(formula.Key, "MaxGems"))
                        {
                            UInt32 newCapa = (UInt32)Math.Max(0, NCalcUtility.ConvertNCalcResult(e.Evaluate(), 0));
                            if (newCapa == UInt32.MaxValue)
                            {
                                play.cur.capa = UInt32.MaxValue;
                            }
                            else if (play.cur.capa + newCapa - play.max.capa >= 0)
                            {
                                play.cur.capa += newCapa - play.max.capa;
                            }
                            else
                            {
                                // Not enough magic stones anymore: let ff9abil.CalculateGemsPlayer handle the situation
                                play.cur.capa = 0;
                                /* Alternative method: disable all the SA and re-launch FF9Play_Update
                                play.cur.capa = newCapa;
                                play.max.capa = newCapa;
                                play.sa[0] = 0u;
                                play.sa[1] = 0u;
                                play.saExtended.Clear();
                                ff9play.FF9Play_Update(play);
                                return;
                                */
                            }
                            play.max.capa = newCapa;
                        }
                    }
                }
                catch (Exception err)
                {
                    String message = $"Error detected in '{PermanentEffect[i].ModFilePath}' permanent effect at line {PermanentEffect[i].FeatureLineNumber}";
                    if (message != lastErrorMessage)
                    {
                        lastErrorMessage = message;
                        Log.Error(message);
                        Log.Error(err);
                    }
                }
            }
        }

        public void TriggerOnBattleStart(ref Int32 backAttackChance, ref Int32 preemptiveChance, ref Int32 preemptivePriority)
        {
            for (Int32 i = 0; i < BattleStartEffect.Count; i++)
            {
                try
                {
                    if (BattleStartEffect[i].Condition.Length > 0)
                    {
                        Expression c = new Expression(BattleStartEffect[i].Condition);
                        c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
                    }
                    foreach (KeyValuePair<String, String> formula in BattleStartEffect[i].Formula)
                    {
                        Expression e = new Expression(formula.Value);
                        e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (String.Equals(formula.Key, "BackAttack")) backAttackChance = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), backAttackChance);
                        else if (String.Equals(formula.Key, "Preemptive")) preemptiveChance = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), preemptiveChance);
                    }
                    preemptivePriority += BattleStartEffect[i].PreemptivePriorityDelta;
                }
                catch (Exception err)
                {
                    String message = $"Error detected in '{BattleStartEffect[i].ModFilePath}' battle start effect at line {BattleStartEffect[i].FeatureLineNumber}";
                    if (message != lastErrorMessage)
                    {
                        lastErrorMessage = message;
                        Log.Error(message);
                        Log.Error(err);
                    }
                }
            }
        }

        public Boolean TriggerOnBattleResult(PLAYER play, BONUS bonus, List<FF9ITEM> bonus_item, String when, UInt32 fleeGil)
        {
            Boolean triggeredAtLeastOnce = false;
            for (Int32 i = 0; i < BattleResultEffect.Count; i++)
            {
                try
                {
                    if (String.Equals(BattleResultEffect[i].When, when))
                    {
                        if (BattleResultEffect[i].Condition.Length > 0)
                        {
                            Expression c = new Expression(BattleResultEffect[i].Condition);
                            NCalcUtility.InitializeExpressionPlayer(ref c, play);
                            NCalcUtility.InitializeExpressionBonus(ref c, bonus, bonus_item);
                            c.Parameters["IsFlee"] = BattleState.IsFlee;
                            c.Parameters["IsFleeByLuck"] = BattleState.IsFleeByLuck;
                            c.Parameters["FleeGil"] = fleeGil;
                            c.Parameters["Status"] = (UInt64)play.status;
                            c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                            c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                            if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                                continue;
                        }
                        triggeredAtLeastOnce = true;
                        foreach (KeyValuePair<String, String> formula in BattleResultEffect[i].Formula)
                        {
                            String[] formulaSplit = formula.Value.Split(';');
                            if (formulaSplit.Length == 0)
                                continue;
                            Expression e = new Expression(formulaSplit[0]);
                            NCalcUtility.InitializeExpressionPlayer(ref e, play);
                            NCalcUtility.InitializeExpressionBonus(ref e, bonus, bonus_item);
                            e.Parameters["IsFlee"] = BattleState.IsFlee;
                            e.Parameters["IsFleeByLuck"] = BattleState.IsFleeByLuck;
                            e.Parameters["FleeGil"] = fleeGil;
                            e.Parameters["Status"] = (UInt64)play.status;
                            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                            if (String.Equals(formula.Key, "FleeGil")) fleeGil = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), fleeGil);
                            else if (String.Equals(formula.Key, "HP")) play.cur.hp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.cur.hp);
                            else if (String.Equals(formula.Key, "MP")) play.cur.mp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.cur.mp);
                            else if (String.Equals(formula.Key, "Trance")) play.trance = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.trance);
                            else if (String.Equals(formula.Key, "Status")) play.status = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)play.status);
                            else if (String.Equals(formula.Key, "BonusAP")) bonus.ap = (UInt16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), bonus.ap);
                            else if (String.Equals(formula.Key, "BonusCard")) bonus.card = (TetraMasterCardId)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)bonus.card);
                            else if (String.Equals(formula.Key, "BonusExp")) bonus.exp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), bonus.exp);
                            else if (String.Equals(formula.Key, "BonusGil")) bonus.gil = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), bonus.gil);
                            else if (String.Equals(formula.Key, "BonusItemAdd"))
                            {
                                List<Expression> allArgs = new List<Expression>(formulaSplit.Length);
                                allArgs.Add(e);
                                for (Int32 argi = 1; argi < formulaSplit.Length; argi++)
                                {
                                    Expression arge = new Expression(formulaSplit[argi]);
                                    NCalcUtility.InitializeExpressionPlayer(ref arge, play);
                                    NCalcUtility.InitializeExpressionBonus(ref arge, bonus, bonus_item);
                                    arge.Parameters["IsFlee"] = BattleState.IsFlee;
                                    arge.Parameters["IsFleeByLuck"] = BattleState.IsFleeByLuck;
                                    arge.Parameters["FleeGil"] = fleeGil;
                                    arge.Parameters["Status"] = (UInt64)play.status;
                                    arge.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                                    arge.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                                    allArgs.Add(arge);
                                }
                                for (Int32 itcounter = 0; itcounter < allArgs.Count; itcounter += 2)
                                {
                                    RegularItem itType = (RegularItem)NCalcUtility.ConvertNCalcResult(allArgs[itcounter].Evaluate(), (Int32)RegularItem.NoItem);
                                    if (itType == RegularItem.NoItem)
                                        continue;
                                    UInt32 count = 1;
                                    if (allArgs.Count > itcounter + 1)
                                        count = (UInt32)NCalcUtility.ConvertNCalcResult(allArgs[itcounter + 1].Evaluate(), 1);
                                    for (Int32 j = 0; j < BattleResultUI.ItemMax; j++)
                                    {
                                        if (bonus_item.Count <= j)
                                            bonus_item.Add(new FF9ITEM(itType, 0));
                                        if (bonus_item[j].id == RegularItem.NoItem)
                                        {
                                            bonus_item[j].id = itType;
                                            bonus_item[j].count = 0;
                                        }
                                        if (bonus_item[j].id == itType)
                                        {
                                            bonus_item[j].count = (Byte)Math.Min(bonus_item[j].count + count, Byte.MaxValue);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (String.Equals(formula.Key, "EachBonusItem"))
                            {
                                for (Int32 j = 0; j < bonus_item.Count; j++)
                                {
                                    if (bonus_item[j].id == RegularItem.NoItem)
                                        continue;
                                    e.Parameters["EachBonusItem"] = (Int32)bonus_item[j].id;
                                    e.Parameters["EachBonusItemCount"] = (Int32)bonus_item[j].count;
                                    RegularItem oldit = bonus_item[j].id;
                                    bonus_item[j].id = (RegularItem)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)bonus_item[j].id);
                                }
                            }
                            else if (String.Equals(formula.Key, "EachBonusItemCount"))
                            {
                                for (Int32 j = 0; j < bonus_item.Count; j++)
                                {
                                    if (bonus_item[j].id == RegularItem.NoItem || bonus_item[j].count == 0)
                                        continue;
                                    e.Parameters["EachBonusItem"] = (Int32)bonus_item[j].id;
                                    e.Parameters["EachBonusItemCount"] = (Int32)bonus_item[j].count;
                                    bonus_item[j].count = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), 0);
                                }
                            }
                            else
                            {
                                for (Int32 j = 0; j < BattleResultUI.ItemMax; j++)
                                {
                                    if (String.Compare(formula.Key, "BonusItem" + (j + 1)) == 0)
                                    {
                                        while (bonus_item.Count <= j) bonus_item.Add(new FF9ITEM(RegularItem.NoItem, 0));
                                        bonus_item[j].id = (RegularItem)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)bonus_item[j].id);
                                        if (bonus_item[j].id == RegularItem.NoItem) bonus_item[j].count = 0;
                                    }
                                    else if (String.Compare(formula.Key, "BonusItemCount" + (j + 1)) == 0)
                                    {
                                        while (bonus_item.Count <= j) bonus_item.Add(new FF9ITEM(RegularItem.NoItem, 0));
                                        bonus_item[j].count = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), bonus_item[j].count);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    String message = $"Error detected in '{BattleResultEffect[i].ModFilePath}' battle result effect at line {BattleResultEffect[i].FeatureLineNumber}";
                    if (message != lastErrorMessage)
                    {
                        lastErrorMessage = message;
                        Log.Error(message);
                        Log.Error(err);
                    }
                }
            }
            return triggeredAtLeastOnce;
        }

        public void TriggerOnStatusInit(BattleUnit unit)
        {
            GetStatusInitQuietly(unit, out BattleStatus permanent, out BattleStatus initial, out BattleStatus resist, out StatusModifier partialResist, out StatusModifier durationFactor, out Int16 atb);
            unit.Data.stat.permanent |= permanent;
            unit.Data.stat.cur |= initial;
            unit.ResistStatus |= resist;
            unit.Data.stat.partial_resist = partialResist;
            unit.Data.stat.duration_factor = durationFactor;
            if (atb >= 0)
                unit.CurrentAtb = atb;
        }

        public void GetStatusInitQuietly(BattleUnit unit, out BattleStatus permanent, out BattleStatus initial, out BattleStatus resist, out StatusModifier partialResist, out StatusModifier durationFactor, out Int16 atb)
        {
            permanent = initial = resist = 0;
            partialResist = unit.PartialResistStatus;
            durationFactor = unit.StatusDurationFactor;
            atb = -1;
            for (Int32 i = 0; i < StatusEffect.Count; i++)
            {
                try
                {
                    if (StatusEffect[i].Condition.Length > 0)
                    {
                        Expression c = new Expression(StatusEffect[i].Condition);
                        NCalcUtility.InitializeExpressionUnit(ref c, unit);
                        c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
                    }
                    permanent |= StatusEffect[i].PermanentStatus;
                    initial |= StatusEffect[i].InitialStatus;
                    resist |= StatusEffect[i].ResistStatus;
                    foreach (KeyValuePair<BattleStatusId, String> kvp in StatusEffect[i].PartialResistStatus)
                    {
                        Expression e = new Expression(kvp.Value);
                        NCalcUtility.InitializeExpressionUnit(ref e, unit);
                        e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        partialResist[kvp.Key] = NCalcUtility.ConvertNCalcResult(e.Evaluate(), 0f);
                    }
                    foreach (KeyValuePair<BattleStatusId, String> kvp in StatusEffect[i].DurationFactorStatus)
                    {
                        Expression e = new Expression(kvp.Value);
                        NCalcUtility.InitializeExpressionUnit(ref e, unit);
                        e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        durationFactor[kvp.Key] = NCalcUtility.ConvertNCalcResult(e.Evaluate(), 1f);
                    }
                    if (StatusEffect[i].InitialATB >= 0)
                        atb = (Int16)Math.Min(unit.MaximumAtb - 1, unit.MaximumAtb * StatusEffect[i].InitialATB / 100);
                }
                catch (Exception err)
                {
                    String message = $"Error detected in '{StatusEffect[i].ModFilePath}' status effect at line {StatusEffect[i].FeatureLineNumber}";
                    if (message != lastErrorMessage)
                    {
                        lastErrorMessage = message;
                        Log.Error(message);
                        Log.Error(err);
                    }
                }
            }
        }

        public void TriggerOnAbility(BattleCalculator calc, String when, Boolean asTarget)
        {
            if (!EnableAsEnemy && !EnableAsMonsterTransform && Id >= 0 && calc.Context.DisabledSA.Contains(Id))
                return;
            Boolean canMove = asTarget ? !calc.Target.IsUnderAnyStatus(BattleStatusConst.Immobilized) : !calc.Caster.IsUnderAnyStatus(BattleStatusConst.Immobilized);
            for (Int32 i = 0; i < AbilityEffect.Count; i++)
            {
                try
                {
                    if (AbilityEffect[i].AsTarget == asTarget && (canMove || AbilityEffect[i].EvenImmobilized) && String.Equals(AbilityEffect[i].When, when))
                    {
                        BattleCaster caster = calc.Caster;
                        BattleTarget target = calc.Target;
                        BattleCommand command = calc.Command;
                        CalcContext context = calc.Context;
                        if (AbilityEffect[i].Condition.Length > 0)
                        {
                            Expression c = new Expression(AbilityEffect[i].Condition);
                            NCalcUtility.InitializeExpressionUnit(ref c, caster, "Caster");
                            NCalcUtility.InitializeExpressionUnit(ref c, target, "Target");
                            NCalcUtility.InitializeExpressionAbilityContext(ref c, calc);
                            c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                            c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                            if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                                continue;
                        }
                        BattleStatus cCurStat = caster.CurrentStatus, cAutoStat = caster.PermanentStatus, cResistStat = caster.ResistStatus;
                        BattleStatus tCurStat = target.CurrentStatus, tAutoStat = target.PermanentStatus, tResistStat = target.ResistStatus;
                        foreach (KeyValuePair<String, String> formula in AbilityEffect[i].Effect)
                        {
                            String[] formulaSplit = formula.Value.Split(';');
                            if (formulaSplit.Length == 0)
                                continue;
                            Expression e = new Expression(formulaSplit[0]);
                            NCalcUtility.InitializeExpressionUnit(ref e, caster, "Caster");
                            NCalcUtility.InitializeExpressionUnit(ref e, target, "Target");
                            NCalcUtility.InitializeExpressionAbilityContext(ref e, calc);
                            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                            if (String.Equals(formula.Key, "CasterHP")) caster.CurrentHp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.CurrentHp);
                            else if (String.Equals(formula.Key, "CasterMP")) caster.CurrentMp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.CurrentMp);
                            else if (String.Equals(formula.Key, "CasterMaxHP")) caster.MaximumHp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.MaximumHp);
                            else if (String.Equals(formula.Key, "CasterMaxMP")) caster.MaximumMp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.MaximumMp);
                            else if (String.Equals(formula.Key, "CasterATB")) caster.CurrentAtb = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.CurrentAtb);
                            else if (String.Equals(formula.Key, "CasterTrance")) caster.Trance = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Trance);
                            else if (String.Equals(formula.Key, "CasterCurrentStatus")) cCurStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)cCurStat);
                            else if (String.Equals(formula.Key, "CasterPermanentStatus")) cAutoStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)cAutoStat);
                            else if (String.Equals(formula.Key, "CasterResistStatus")) cResistStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)cResistStat);
                            else if (String.Equals(formula.Key, "CasterHalfElement")) caster.HalfElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.HalfElement);
                            else if (String.Equals(formula.Key, "CasterGuardElement")) caster.GuardElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.GuardElement);
                            else if (String.Equals(formula.Key, "CasterAbsorbElement")) caster.AbsorbElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.AbsorbElement);
                            else if (String.Equals(formula.Key, "CasterWeakElement")) caster.WeakElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.WeakElement);
                            else if (String.Equals(formula.Key, "CasterBonusElement")) caster.BonusElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.BonusElement);
                            else if (String.Equals(formula.Key, "CasterSpeed")) caster.Dexterity = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Dexterity);
                            else if (String.Equals(formula.Key, "CasterStrength")) caster.Strength = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Strength);
                            else if (String.Equals(formula.Key, "CasterMagic")) caster.Magic = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Magic);
                            else if (String.Equals(formula.Key, "CasterSpirit")) caster.Will = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Will);
                            else if (String.Equals(formula.Key, "CasterDefence")) caster.PhysicalDefence = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.PhysicalDefence);
                            else if (String.Equals(formula.Key, "CasterEvade")) caster.PhysicalEvade = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.PhysicalEvade);
                            else if (String.Equals(formula.Key, "CasterMagicDefence")) caster.MagicDefence = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.MagicDefence);
                            else if (String.Equals(formula.Key, "CasterMagicEvade")) caster.MagicEvade = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.MagicEvade);
                            else if (String.Equals(formula.Key, "CasterRow")) caster.Row = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Row);
                            else if (String.Equals(formula.Key, "CasterSummonCount")) caster.SummonCount = (UInt16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.SummonCount);
                            else if (String.Equals(formula.Key, "CasterCriticalRateBonus")) caster.CriticalRateBonus = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.CriticalRateBonus);
                            else if (String.Equals(formula.Key, "CasterCriticalRateResistance")) caster.CriticalRateResistance = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.CriticalRateResistance);
                            else if (String.Equals(formula.Key, "CasterMaxDamageLimit")) caster.MaxDamageLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.MaxDamageLimit);
                            else if (String.Equals(formula.Key, "CasterMaxMPDamageLimit")) caster.MaxMpDamageLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.MaxMpDamageLimit);
                            else if (String.Equals(formula.Key, "CasterBonusExp") && !caster.IsPlayer) caster.Enemy.Data.bonus_exp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Enemy.Data.bonus_exp);
                            else if (String.Equals(formula.Key, "CasterBonusGil") && !caster.IsPlayer) caster.Enemy.Data.bonus_gil = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Enemy.Data.bonus_gil);
                            else if (String.Equals(formula.Key, "CasterBonusCard") && !caster.IsPlayer) caster.Enemy.Data.bonus_card = (TetraMasterCardId)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)caster.Enemy.Data.bonus_card);
                            else if (String.Equals(formula.Key, "TargetHP")) target.CurrentHp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.CurrentHp);
                            else if (String.Equals(formula.Key, "TargetMP")) target.CurrentMp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.CurrentMp);
                            else if (String.Equals(formula.Key, "TargetMaxHP")) target.MaximumHp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.MaximumHp);
                            else if (String.Equals(formula.Key, "TargetMaxMP")) target.MaximumMp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.MaximumMp);
                            else if (String.Equals(formula.Key, "TargetATB")) target.CurrentAtb = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.CurrentAtb);
                            else if (String.Equals(formula.Key, "TargetTrance")) target.Trance = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Trance);
                            else if (String.Equals(formula.Key, "TargetCurrentStatus")) tCurStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)tCurStat);
                            else if (String.Equals(formula.Key, "TargetPermanentStatus")) tAutoStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)tAutoStat);
                            else if (String.Equals(formula.Key, "TargetResistStatus")) tResistStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)tResistStat);
                            else if (String.Equals(formula.Key, "TargetHalfElement")) target.HalfElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.HalfElement);
                            else if (String.Equals(formula.Key, "TargetGuardElement")) target.GuardElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.GuardElement);
                            else if (String.Equals(formula.Key, "TargetAbsorbElement")) target.AbsorbElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.AbsorbElement);
                            else if (String.Equals(formula.Key, "TargetWeakElement")) target.WeakElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.WeakElement);
                            else if (String.Equals(formula.Key, "TargetBonusElement")) target.BonusElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.BonusElement);
                            else if (String.Equals(formula.Key, "TargetSpeed")) target.Dexterity = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Dexterity);
                            else if (String.Equals(formula.Key, "TargetStrength")) target.Strength = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Strength);
                            else if (String.Equals(formula.Key, "TargetMagic")) target.Magic = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Magic);
                            else if (String.Equals(formula.Key, "TargetSpirit")) target.Will = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Will);
                            else if (String.Equals(formula.Key, "TargetDefence")) target.PhysicalDefence = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.PhysicalDefence);
                            else if (String.Equals(formula.Key, "TargetEvade")) target.PhysicalEvade = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.PhysicalEvade);
                            else if (String.Equals(formula.Key, "TargetMagicDefence")) target.MagicDefence = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.MagicDefence);
                            else if (String.Equals(formula.Key, "TargetMagicEvade")) target.MagicEvade = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.MagicEvade);
                            else if (String.Equals(formula.Key, "TargetRow")) target.Row = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Row);
                            else if (String.Equals(formula.Key, "TargetSummonCount")) target.SummonCount = (UInt16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.SummonCount);
                            else if (String.Equals(formula.Key, "TargetCriticalRateBonus")) target.CriticalRateBonus = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.CriticalRateBonus);
                            else if (String.Equals(formula.Key, "TargetCriticalRateResistance")) target.CriticalRateResistance = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.CriticalRateResistance);
                            else if (String.Equals(formula.Key, "TargetMaxDamageLimit")) target.MaxDamageLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.MaxDamageLimit);
                            else if (String.Equals(formula.Key, "TargetMaxMPDamageLimit")) target.MaxMpDamageLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.MaxMpDamageLimit);
                            else if (String.Equals(formula.Key, "TargetBonusExp") && !target.IsPlayer) target.Enemy.Data.bonus_exp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Enemy.Data.bonus_exp);
                            else if (String.Equals(formula.Key, "TargetBonusGil") && !target.IsPlayer) target.Enemy.Data.bonus_gil = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Enemy.Data.bonus_gil);
                            else if (String.Equals(formula.Key, "TargetBonusCard") && !target.IsPlayer) target.Enemy.Data.bonus_card = (TetraMasterCardId)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)target.Enemy.Data.bonus_card);
                            else if (String.Equals(formula.Key, "EffectCasterFlags")) caster.Flags = (CalcFlag)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.Flags);
                            else if (String.Equals(formula.Key, "CasterHPDamage")) caster.HpDamage = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.HpDamage);
                            else if (String.Equals(formula.Key, "CasterMPDamage")) caster.MpDamage = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.MpDamage);
                            else if (String.Equals(formula.Key, "EffectTargetFlags")) target.Flags = (CalcFlag)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.Flags);
                            else if (String.Equals(formula.Key, "HPDamage")) target.HpDamage = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.HpDamage);
                            else if (String.Equals(formula.Key, "MPDamage")) target.MpDamage = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.MpDamage);
                            else if (String.Equals(formula.Key, "FigureInfo")) target.Data.fig.info = (UInt16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Data.fig.info);
                            else if (String.Equals(formula.Key, "Power")) command.Power = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.Power);
                            else if (String.Equals(formula.Key, "AbilityStatus")) command.AbilityStatus = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)command.AbilityStatus);
                            else if (String.Equals(formula.Key, "AbilityElement")) command.Element = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)command.Element);
                            else if (String.Equals(formula.Key, "AbilityElementForBonus")) command.ElementForBonus = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)command.Element);
                            else if (String.Equals(formula.Key, "IsShortRanged")) command.IsShortRange = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), command.IsShortRange);
                            else if (String.Equals(formula.Key, "IsDrain")) context.IsDrain = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), context.IsDrain);
                            else if (String.Equals(formula.Key, "AbilityCategory")) command.AbilityCategory = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.AbilityCategory);
                            else if (String.Equals(formula.Key, "AbilityFlags")) command.AbilityType = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.AbilityType);
                            else if (String.Equals(formula.Key, "Attack")) context.Attack = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.Attack);
                            else if (String.Equals(formula.Key, "AttackPower")) context.AttackPower = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.AttackPower);
                            else if (String.Equals(formula.Key, "DefencePower")) context.DefensePower = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.DefensePower);
                            else if (String.Equals(formula.Key, "StatusRate")) context.StatusRate = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.StatusRate);
                            else if (String.Equals(formula.Key, "HitRate")) context.HitRate = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.HitRate);
                            else if (String.Equals(formula.Key, "Evade")) context.Evade = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.Evade);
                            else if (String.Equals(formula.Key, "EffectFlags")) context.Flags = (BattleCalcFlags)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (UInt16)context.Flags);
                            else if (String.Equals(formula.Key, "DamageModifierCount")) context.DamageModifierCount = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.DamageModifierCount);
                            else if (String.Equals(formula.Key, "TranceIncrease")) context.TranceIncrease = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.TranceIncrease);
                            else if (String.Equals(formula.Key, "ItemSteal")) context.ItemSteal = (RegularItem)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)context.ItemSteal);
                            else if (String.Equals(formula.Key, "Gil")) GameState.Gil = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), GameState.Gil);
                            else if (String.Equals(formula.Key, "BattleBonusAP")) battle.btl_bonus.ap = (UInt16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), battle.btl_bonus.ap);
                            else if (String.Equals(formula.Key, "Counter"))
                            {
                                List<Expression> allArgs = new List<Expression>(formulaSplit.Length);
                                allArgs.Add(e);
                                for (Int32 argi = 1; argi < formulaSplit.Length; argi++)
                                {
                                    Expression arge = new Expression(formulaSplit[argi]);
                                    NCalcUtility.InitializeExpressionUnit(ref arge, caster, "Caster");
                                    NCalcUtility.InitializeExpressionUnit(ref arge, target, "Target");
                                    NCalcUtility.InitializeExpressionAbilityContext(ref arge, calc);
                                    arge.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                                    arge.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                                    allArgs.Add(arge);
                                }
                                Int32 attackId = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)BattleAbilityId.Attack);
                                BattleCommandId counterCmd = EnableAsEnemy || EnableAsMonsterTransform ? BattleCommandId.EnemyCounter : BattleCommandId.Counter;
                                if (allArgs.Count > 1)
                                    counterCmd = (BattleCommandId)NCalcUtility.ConvertNCalcResult(allArgs[1].Evaluate(), (Int32)BattleCommandId.Counter);
                                if (attackId == (Int32)BattleAbilityId.Void && counterCmd != BattleCommandId.EnemyCounter && counterCmd != BattleCommandId.MagicCounter)
                                    continue;
                                BTL_DATA counterer = asTarget ? target.Data : caster.Data;
                                UInt16 countered = asTarget ? caster.Id : target.Id;
                                if (allArgs.Count > 2)
                                    countered = (UInt16)NCalcUtility.ConvertNCalcResult(allArgs[2].Evaluate(), 0);
                                if (allArgs.Count > 3)
                                {
                                    UInt16 countererId = (UInt16)NCalcUtility.ConvertNCalcResult(allArgs[3].Evaluate(), 0);
                                    if (countererId == 0)
                                        continue;
                                    counterer = btl_scrp.FindBattleUnit((UInt16)Comn.firstBitSet(countererId))?.Data ?? null;
                                    if (counterer == null)
                                        continue;
                                }
                                btl_cmd.SetCounter(counterer, counterCmd, attackId, countered);
                            }
                            else if (String.Equals(formula.Key, "ReturnMagic"))
                            {
                                //NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)BattleAbilityId.Attack);
                                if (asTarget)
                                    btl_abil.TryReturnMagic(target, caster, command);
                                else
                                    btl_abil.TryReturnMagic(caster, target, command);
                            }
                            else if (String.Equals(formula.Key, "AutoItem"))
                            {
                                BTL_DATA counterer = asTarget ? target.Data : caster.Data;
                                if (counterer.is_monster_transform && counterer.monster_transform.disable_commands.Contains(BattleCommandId.Item))
                                    continue;
                                RegularItem itemId = (RegularItem)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)RegularItem.Potion);
                                if (Configuration.Battle.AutoPotionOverhealLimit >= 0 && Id == SupportAbility.AutoPotion && asTarget && (itemId == RegularItem.Potion || itemId == RegularItem.HiPotion))
                                {
                                    // A bit hacky but painlessly keeps the "AutoPotionOverhealLimit" option
                                    btl_abil.CheckAutoItemAbility(target, command);
                                }
                                else
                                {
                                    if (ff9item.FF9Item_GetCount(itemId) != 0)
                                    {
                                        UIManager.Battle.ItemRequest(itemId);
                                        btl_cmd.SetCounter(counterer, BattleCommandId.AutoPotion, (Int32)itemId, counterer.btl_id);
                                    }
                                }
                            }
                        }
                        UpdateUnitStatuses(caster, cCurStat, cAutoStat, cResistStat);
                        UpdateUnitStatuses(target, tCurStat, tAutoStat, tResistStat);
                        foreach (SupportAbility disSA in AbilityEffect[i].DisableSA)
                            context.DisabledSA.Add(disSA);
                    }

                }
                catch (Exception err)
                {
                    String message = $"Error detected in '{AbilityEffect[i].ModFilePath}' ability effect at line {AbilityEffect[i].FeatureLineNumber}";
                    if (message != lastErrorMessage)
                    {
                        lastErrorMessage = message;
                        Log.Error(message);
                        Log.Error(err);
                    }
                }
            }
        }

        public void TriggerOnCommand(BattleUnit abilityUser, BattleCommand command, ref UInt16 tryCover)
        {
            Boolean canMove = !abilityUser.IsUnderAnyStatus(BattleStatusConst.Immobilized);
            BattleUnit caster = null;
            BattleUnit target = null;
            for (Int32 i = 0; i < CommandEffect.Count; i++)
            {
                try
                {
                    if (canMove || CommandEffect[i].EvenImmobilized)
                    {
                        if (caster == null && command.Data.regist != null) caster = new BattleUnit(command.Data.regist);
                        if (target == null && Comn.countBits(command.Data.tar_id) == 1) target = btl_scrp.FindBattleUnit(command.Data.tar_id);
                        if (CommandEffect[i].Condition.Length > 0)
                        {
                            Expression c = new Expression(CommandEffect[i].Condition);
                            NCalcUtility.InitializeExpressionUnit(ref c, abilityUser);
                            //if (caster != null) NCalcUtility.InitializeExpressionUnit(ref c, caster, "Caster");
                            //if (target != null) NCalcUtility.InitializeExpressionUnit(ref c, target, "Target");
                            NCalcUtility.InitializeExpressionCommand(ref c, command);
                            c.Parameters["IsTargeted"] = (command.Data.tar_id & abilityUser.Id) != 0;
                            c.Parameters["IsCasterWellDefined"] = caster != null;
                            c.Parameters["IsSingleTarget"] = target != null;
                            c.Parameters["IsTheCaster"] = caster != null && caster.Id == abilityUser.Id;
                            c.Parameters["IsSelfTarget"] = caster != null && target != null && caster.Id == target.Id;
                            c.Parameters["IsAllyOfTarget"] = target != null && target.IsPlayer == abilityUser.IsPlayer;
                            c.Parameters["IsAllyOfCaster"] = caster != null && caster.IsPlayer == abilityUser.IsPlayer;
                            c.Parameters["IsEnemyOfTarget"] = target != null && target.IsPlayer != abilityUser.IsPlayer;
                            c.Parameters["IsEnemyOfCaster"] = caster != null && caster.IsPlayer != abilityUser.IsPlayer;
                            c.Parameters["AreCasterAndTargetEnemies"] = caster != null && (caster.IsPlayer && (command.Data.tar_id & 0xF0) == command.Data.tar_id || !caster.IsPlayer && (command.Data.tar_id & 0xF) == command.Data.tar_id);
                            c.Parameters["AreCasterAndTargetAllies"] = caster != null && (caster.IsPlayer && (command.Data.tar_id & 0xF) == command.Data.tar_id || !caster.IsPlayer && (command.Data.tar_id & 0x0F) == command.Data.tar_id);
                            c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                            c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                            if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                                continue;
                        }
                        BattleStatus uCurStat = abilityUser.CurrentStatus, uAutoStat = abilityUser.PermanentStatus, uResistStat = abilityUser.ResistStatus;
                        foreach (KeyValuePair<String, String> formula in CommandEffect[i].Effect)
                        {
                            Expression e = new Expression(formula.Value);
                            NCalcUtility.InitializeExpressionUnit(ref e, abilityUser);
                            if (caster != null) NCalcUtility.InitializeExpressionUnit(ref e, caster, "Caster");
                            if (target != null) NCalcUtility.InitializeExpressionUnit(ref e, target, "Target");
                            NCalcUtility.InitializeExpressionCommand(ref e, command);
                            e.Parameters["IsTargeted"] = (command.Data.tar_id & abilityUser.Id) != 0;
                            e.Parameters["IsCasterWellDefined"] = caster != null;
                            e.Parameters["IsSingleTarget"] = target != null;
                            e.Parameters["IsTheCaster"] = caster != null && (caster.Id & abilityUser.Id) != 0;
                            e.Parameters["IsSelfTarget"] = caster != null && target != null && caster.Id == target.Id;
                            e.Parameters["IsAllyOfTarget"] = target != null && target.IsPlayer == abilityUser.IsPlayer;
                            e.Parameters["IsAllyOfCaster"] = caster != null && caster.IsPlayer == abilityUser.IsPlayer;
                            e.Parameters["IsEnemyOfTarget"] = target != null && target.IsPlayer != abilityUser.IsPlayer;
                            e.Parameters["IsEnemyOfCaster"] = caster != null && caster.IsPlayer != abilityUser.IsPlayer;
                            e.Parameters["AreCasterAndTargetEnemies"] = caster != null && (caster.IsPlayer && (command.Data.tar_id & 0xF0) == command.Data.tar_id || !caster.IsPlayer && (command.Data.tar_id & 0xF) == command.Data.tar_id);
                            e.Parameters["AreCasterAndTargetAllies"] = caster != null && (caster.IsPlayer && (command.Data.tar_id & 0xF) == command.Data.tar_id || !caster.IsPlayer && (command.Data.tar_id & 0x0F) == command.Data.tar_id);
                            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                            if (String.Equals(formula.Key, "Power")) command.Power = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.Power);
                            else if (String.Equals(formula.Key, "AbilityStatus")) command.AbilityStatus = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)command.AbilityStatus);
                            else if (String.Equals(formula.Key, "AbilityElement")) command.Element = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)command.Element);
                            else if (String.Equals(formula.Key, "AbilityElementForBonus")) command.ElementForBonus = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)command.Element);
                            else if (String.Equals(formula.Key, "IsShortRanged")) command.IsShortRange = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), command.IsShortRange);
                            else if (String.Equals(formula.Key, "AbilityCategory")) command.AbilityCategory = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.AbilityCategory);
                            else if (String.Equals(formula.Key, "AbilityFlags")) command.AbilityType = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.AbilityType);
                            else if (String.Equals(formula.Key, "IsReflectNull")) command.IsReflectNull = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), command.IsReflectNull);
                            else if (String.Equals(formula.Key, "IsMeteorMiss")) command.Data.info.meteor_miss = (Byte)(NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), command.IsReflectNull) ? 1 : 0);
                            else if (String.Equals(formula.Key, "IsShortSummon")) command.IsShortSummon = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), command.IsShortSummon);
                            else if (String.Equals(formula.Key, "TryCover")) tryCover |= NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), false) ? abilityUser.Id : (UInt16)0;
                            else if (String.Equals(formula.Key, "ScriptId")) command.ScriptId = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.ScriptId);
                            else if (String.Equals(formula.Key, "HitRate")) command.HitRate = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.HitRate);
                            else if (String.Equals(formula.Key, "CommandTargetId")) command.Data.tar_id = (UInt16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.Data.tar_id);
                            else if (String.Equals(formula.Key, "HP")) abilityUser.CurrentHp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.CurrentHp);
                            else if (String.Equals(formula.Key, "MP")) abilityUser.CurrentMp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.CurrentMp);
                            else if (String.Equals(formula.Key, "MaxHP")) abilityUser.MaximumHp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.MaximumHp);
                            else if (String.Equals(formula.Key, "MaxMP")) abilityUser.MaximumMp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.MaximumMp);
                            else if (String.Equals(formula.Key, "ATB")) abilityUser.CurrentAtb = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.CurrentAtb);
                            else if (String.Equals(formula.Key, "Trance")) abilityUser.Trance = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.Trance);
                            else if (String.Equals(formula.Key, "CurrentStatus")) uCurStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)uCurStat);
                            else if (String.Equals(formula.Key, "PermanentStatus")) uAutoStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)uAutoStat);
                            else if (String.Equals(formula.Key, "ResistStatus")) uResistStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)uResistStat);
                            else if (String.Equals(formula.Key, "HalfElement")) abilityUser.HalfElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)abilityUser.HalfElement);
                            else if (String.Equals(formula.Key, "GuardElement")) abilityUser.GuardElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)abilityUser.GuardElement);
                            else if (String.Equals(formula.Key, "AbsorbElement")) abilityUser.AbsorbElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)abilityUser.AbsorbElement);
                            else if (String.Equals(formula.Key, "WeakElement")) abilityUser.WeakElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)abilityUser.WeakElement);
                            else if (String.Equals(formula.Key, "BonusElement")) abilityUser.BonusElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)abilityUser.BonusElement);
                            else if (String.Equals(formula.Key, "Speed")) abilityUser.Dexterity = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.Dexterity);
                            else if (String.Equals(formula.Key, "Strength")) abilityUser.Strength = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.Strength);
                            else if (String.Equals(formula.Key, "Magic")) abilityUser.Magic = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.Magic);
                            else if (String.Equals(formula.Key, "Spirit")) abilityUser.Will = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.Will);
                            else if (String.Equals(formula.Key, "Defence")) abilityUser.PhysicalDefence = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.PhysicalDefence);
                            else if (String.Equals(formula.Key, "Evade")) abilityUser.PhysicalEvade = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.PhysicalEvade);
                            else if (String.Equals(formula.Key, "MagicDefence")) abilityUser.MagicDefence = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.MagicDefence);
                            else if (String.Equals(formula.Key, "MagicEvade")) abilityUser.MagicEvade = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.MagicEvade);
                            else if (String.Equals(formula.Key, "Row")) abilityUser.Row = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.Row);
                            else if (String.Equals(formula.Key, "CriticalRateBonus")) abilityUser.CriticalRateBonus = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.CriticalRateBonus);
                            else if (String.Equals(formula.Key, "CriticalRateResistance")) abilityUser.CriticalRateResistance = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.CriticalRateResistance);
                            else if (String.Equals(formula.Key, "MaxDamageLimit")) abilityUser.MaxDamageLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.MaxDamageLimit);
                            else if (String.Equals(formula.Key, "MaxMPDamageLimit")) abilityUser.MaxMpDamageLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.MaxMpDamageLimit);
                            else if (String.Equals(formula.Key, "BonusExp") && !abilityUser.IsPlayer) abilityUser.Enemy.Data.bonus_exp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.Enemy.Data.bonus_exp);
                            else if (String.Equals(formula.Key, "BonusGil") && !abilityUser.IsPlayer) abilityUser.Enemy.Data.bonus_gil = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), abilityUser.Enemy.Data.bonus_gil);
                            else if (String.Equals(formula.Key, "BonusCard") && !abilityUser.IsPlayer) abilityUser.Enemy.Data.bonus_card = (TetraMasterCardId)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)abilityUser.Enemy.Data.bonus_card);
                            else if (String.Equals(formula.Key, "BattleBonusAP")) battle.btl_bonus.ap = (UInt16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), battle.btl_bonus.ap);
                            else if (String.Equals(formula.Key, "Counter"))
                            {
                                Int32 attackId = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)BattleAbilityId.Attack);
                                if (attackId == (Int32)BattleAbilityId.Void && !EnableAsEnemy && !EnableAsMonsterTransform)
                                    continue;
                                BattleCommandId commandId = EnableAsEnemy || EnableAsMonsterTransform ? BattleCommandId.EnemyCounter : BattleCommandId.Counter;
                                UInt16 counterTarget = btl_cmd.GetRandomTargetForCommand(abilityUser, commandId, attackId);
                                btl_cmd.SetCounter(abilityUser, commandId, attackId, counterTarget);
                            }
                        }
                        UpdateUnitStatuses(abilityUser, uCurStat, uAutoStat, uResistStat);
                    }
                }
                catch (Exception err)
                {
                    String message = $"Error detected in '{CommandEffect[i].ModFilePath}' command effect at line {CommandEffect[i].FeatureLineNumber}";
                    if (message != lastErrorMessage)
                    {
                        lastErrorMessage = message;
                        Log.Error(message);
                        Log.Error(err);
                    }
                }
            }
        }

        private void UpdateUnitStatuses(BattleUnit unit, BattleStatus cur, BattleStatus auto, BattleStatus resist)
        {
            cur &= ~(unit.PermanentStatus & ~auto);
            if (unit.PermanentStatus != auto) btl_stat.MakeStatusesPermanent(unit, unit.PermanentStatus & ~auto, false);
            if (unit.CurrentStatus != cur) btl_stat.RemoveStatuses(unit, unit.CurrentStatus & ~cur);
            if (unit.ResistStatus != resist) unit.ResistStatus = resist;
            if (unit.PermanentStatus != auto) btl_stat.MakeStatusesPermanent(unit, auto & ~unit.PermanentStatus, true);
            if (unit.CurrentStatus != cur) btl_stat.AlterStatuses(unit, cur & ~unit.CurrentStatus);
        }

        public void ParseFeatures(SupportAbility id, String featureCode, String modFilePath, Int32 initialLineNumber)
        {
            Id = id;
            if ((Int32)Id == -3 || (Int32)Id == -4)
                EnableAsEnemy = true;
            MatchCollection codeMatches = new Regex(@"^(Permanent|BattleStart|BattleResult|StatusInit|Ability|Command|EnemyFeature|MorphFeature)\b", RegexOptions.Multiline).Matches(featureCode);
            for (Int32 i = 0; i < codeMatches.Count; i++)
            {
                String saCode = codeMatches[i].Groups[1].Value;
                Int32 endPos, startPos = codeMatches[i].Groups[1].Captures[0].Index + codeMatches[i].Groups[1].Value.Length;
                if (i + 1 == codeMatches.Count)
                    endPos = featureCode.Length;
                else
                    endPos = codeMatches[i + 1].Groups[1].Captures[0].Index;
                String saArgs = featureCode.Substring(startPos, endPos - startPos);
                Int32 lineNumber = initialLineNumber + Regex.Matches(featureCode.Substring(0, startPos), @"\n", RegexOptions.Singleline).Count + 1;
                if (String.Equals(saCode, "Permanent"))
                {
                    SupportingAbilityEffectPermanent newEffect = new SupportingAbilityEffectPermanent();
                    newEffect.ModFilePath = modFilePath;
                    newEffect.FeatureLineNumber = lineNumber;
                    foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        if (String.Equals(formula.Groups[1].Value, "Condition"))
                            newEffect.Condition = formula.Groups[2].Value;
                        else
                            newEffect.Formula[formula.Groups[1].Value] = formula.Groups[2].Value;
                    }
                    PermanentEffect.Add(newEffect);
                }
                else if (String.Equals(saCode, "BattleStart"))
                {
                    SupportingAbilityEffectBattleStartType newEffect = new SupportingAbilityEffectBattleStartType();
                    newEffect.ModFilePath = modFilePath;
                    newEffect.FeatureLineNumber = lineNumber;
                    Match priorityDelta = new Regex(@"\bPreemptivePriority\s+([\+-]?\d+)").Match(saArgs);
                    if (priorityDelta.Success)
                        Int32.TryParse(priorityDelta.Groups[1].Value, out newEffect.PreemptivePriorityDelta);
                    foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        if (String.Equals(formula.Groups[1].Value, "Condition"))
                            newEffect.Condition = formula.Groups[2].Value;
                        else
                            newEffect.Formula[formula.Groups[1].Value] = formula.Groups[2].Value;
                    }
                    BattleStartEffect.Add(newEffect);
                }
                else if (String.Equals(saCode, "BattleResult"))
                {
                    SupportingAbilityEffectBattleResult newEffect = new SupportingAbilityEffectBattleResult();
                    newEffect.ModFilePath = modFilePath;
                    newEffect.FeatureLineNumber = lineNumber;
                    Match when = new Regex(@"\bWhen(\w+)\b").Match(saArgs);
                    if (when.Success)
                        newEffect.When = when.Groups[1].Value;
                    foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        if (String.Equals(formula.Groups[1].Value, "Condition"))
                            newEffect.Condition = formula.Groups[2].Value;
                        else
                            newEffect.Formula[formula.Groups[1].Value] = formula.Groups[2].Value;
                    }
                    BattleResultEffect.Add(newEffect);
                }
                else if (String.Equals(saCode, "StatusInit"))
                {
                    SupportingAbilityEffectBattleInitStatus newEffect = new SupportingAbilityEffectBattleInitStatus();
                    newEffect.ModFilePath = modFilePath;
                    newEffect.FeatureLineNumber = lineNumber;
                    foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        String codeName = formula.Groups[1].Value;
                        if (String.Equals(codeName, "Condition"))
                        {
                            newEffect.Condition = formula.Groups[2].Value;
                        }
                        else if (codeName.StartsWith("PartialResist"))
                        {
                            if (codeName.Substring("PartialResist".Length).TryEnumParse(out BattleStatusId status))
                                newEffect.PartialResistStatus.Add(new KeyValuePair<BattleStatusId, String>(status, formula.Groups[2].Value));
                        }
                        else if (codeName.StartsWith("DurationFactor"))
                        {
                            if (codeName.Substring("DurationFactor".Length).TryEnumParse(out BattleStatusId status))
                                newEffect.DurationFactorStatus.Add(new KeyValuePair<BattleStatusId, String>(status, formula.Groups[2].Value));
                        }
                    }
                    foreach (Match statusMatch in new Regex(@"\b((Auto|Initial|Resist)Status|InitialATB)\s+(\w+|\d+)\b").Matches(saArgs))
                    {
                        if (String.Equals(statusMatch.Groups[1].Value, "InitialATB"))
                        {
                            Int32.TryParse(statusMatch.Groups[3].Value, out newEffect.InitialATB);
                        }
                        else
                        {
                            if (statusMatch.Groups[3].Value.TryEnumParse(out BattleStatus status))
                            {
                                if (String.Equals(statusMatch.Groups[2].Value, "Auto"))
                                    newEffect.PermanentStatus |= status;
                                else if (String.Equals(statusMatch.Groups[2].Value, "Initial"))
                                    newEffect.InitialStatus |= status;
                                else if (String.Equals(statusMatch.Groups[2].Value, "Resist"))
                                    newEffect.ResistStatus |= status;
                            }
                        }
                    }
                    StatusEffect.Add(newEffect);
                }
                else if (String.Equals(saCode, "Ability"))
                {
                    SupportingAbilityEffectAbilityUse newEffect = new SupportingAbilityEffectAbilityUse();
                    newEffect.ModFilePath = modFilePath;
                    newEffect.FeatureLineNumber = lineNumber;
                    Match when = new Regex(@"\bWhen(\w+)\b").Match(saArgs);
                    if (when.Success)
                        newEffect.When = when.Groups[1].Value;
                    foreach (Match effect in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        if (String.Equals(effect.Groups[1].Value, "Condition"))
                            newEffect.Condition = effect.Groups[2].Value;
                        else
                            newEffect.Effect[effect.Groups[1].Value] = effect.Groups[2].Value;
                    }
                    if (new Regex(@"\bAsTarget\b").Match(saArgs).Success) newEffect.AsTarget = true;
                    if (new Regex(@"\bEvenImmobilized\b").Match(saArgs).Success) newEffect.EvenImmobilized = true;
                    Match disableSA = new Regex(@"\bDisableSA\s+(\d+\s+)+").Match(saArgs);
                    Int32 saValue;
                    if (disableSA.Success)
                        foreach (Capture saValueStr in disableSA.Groups[1].Captures)
                            if (Int32.TryParse(saValueStr.Value, out saValue))
                                newEffect.DisableSA.Add((SupportAbility)saValue);
                    AbilityEffect.Add(newEffect);
                }
                else if (String.Equals(saCode, "Command"))
                {
                    SupportingAbilityEffectCommandStart newEffect = new SupportingAbilityEffectCommandStart();
                    newEffect.ModFilePath = modFilePath;
                    newEffect.FeatureLineNumber = lineNumber;
                    foreach (Match effect in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        if (String.Equals(effect.Groups[1].Value, "Condition"))
                            newEffect.Condition = effect.Groups[2].Value;
                        else
                            newEffect.Effect[effect.Groups[1].Value] = effect.Groups[2].Value;
                    }
                    if (new Regex(@"\bEvenImmobilized\b").Match(saArgs).Success) newEffect.EvenImmobilized = true;
                    CommandEffect.Add(newEffect);
                }
                else if (String.Equals(saCode, "EnemyFeature"))
                {
                    EnableAsEnemy = true;
                }
                else if (String.Equals(saCode, "MorphFeature"))
                {
                    EnableAsMonsterTransform = true;
                }
            }
        }
    }
}
