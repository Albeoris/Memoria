using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FF9;
using NCalc;
using Memoria.Prime.CSV;
using System.Runtime.InteropServices;
using UnityEngine.Networking.Types;
//using System.Linq.Expressions;

namespace Memoria.Data
{
    public sealed class CharacterAbilityGems : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public Byte GemsCount;

        public void ParseEntry(String[] raw)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            GemsCount = CsvParser.Byte(raw[2]);
        }

        public void WriteEntry(CsvWriter writer)
        {
            writer.String(Comment);
            writer.Int32(Id);

            writer.Byte(GemsCount);
        }
    }

    public class SupportingAbilityFeature
    {
        public class SupportingAbilityEffectPermanent
        {
            public String Condition = "";
            public Dictionary<String, String> Formula = new Dictionary<String, String>();
        }
        public class SupportingAbilityEffectBattleStartType
        {
            public String Condition = "";
            public Dictionary<String, String> Formula = new Dictionary<String, String>();
            public Int32 PreemptivePriorityDelta = 0;
        }
        public class SupportingAbilityEffectBattleResult
        {
            public String When = "RewardSingle"; // "BattleEnd", "RewardAll", "RewardSingle"
            public String Condition = "";
            public Dictionary<String, String> Formula = new Dictionary<String, String>();
        }
        public class SupportingAbilityEffectBattleInitStatus
        {
            public String Condition = "";
            public BattleStatus PermanentStatus = 0;
            public BattleStatus InitialStatus = 0;
            public BattleStatus ResistStatus = 0;
            public Int32 InitialATB = -1;
        }
        public class SupportingAbilityEffectAbilityUse
        {
            public String When = "EffectDone"; // "BattleScriptStart", "HitRateSetup", "CalcDamage", "Steal", "BattleScriptEnd", "EffectDone"
            public String Condition = "";
            public Dictionary<String, String> Effect = new Dictionary<String, String>();
            public Boolean AsTarget = false;
            public Boolean EvenImmobilized = false;
            public List<Int32> DisableSA = new List<Int32>();
        }
        public class SupportingAbilityEffectCommandStart
        {
            public String Condition = "";
            public Dictionary<String, String> Effect = new Dictionary<String, String>();
            public Boolean EvenImmobilized = false;
        }

        public Int32 Id;
        public List<SupportingAbilityEffectPermanent> PermanentEffect = new List<SupportingAbilityEffectPermanent>();
        public List<SupportingAbilityEffectBattleStartType> BattleStartEffect = new List<SupportingAbilityEffectBattleStartType>();
        public List<SupportingAbilityEffectBattleResult> BattleResultEffect = new List<SupportingAbilityEffectBattleResult>();
        public List<SupportingAbilityEffectBattleInitStatus> StatusEffect = new List<SupportingAbilityEffectBattleInitStatus>();
        public List<SupportingAbilityEffectAbilityUse> AbilityEffect = new List<SupportingAbilityEffectAbilityUse>();
        public List<SupportingAbilityEffectCommandStart> CommandEffect = new List<SupportingAbilityEffectCommandStart>();

        public void TriggerOnEnable(PLAYER play)
        {
            for (Int32 i = 0; i < PermanentEffect.Count; i++)
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
                    if (String.Compare(formula.Key, "MaxHP") == 0) play.max.hp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.max.hp);
                    else if (String.Compare(formula.Key, "MaxMP") == 0) play.max.mp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.max.mp);
                    else if (String.Compare(formula.Key, "Speed") == 0) play.basis.dex = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.basis.dex);
                    else if (String.Compare(formula.Key, "Strength") == 0) play.basis.str = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.basis.str);
                    else if (String.Compare(formula.Key, "Magic") == 0) play.basis.mgc = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.basis.mgc);
                    else if (String.Compare(formula.Key, "Spirit") == 0) play.basis.wpr = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.basis.wpr);
                    else if (String.Compare(formula.Key, "Defence") == 0) play.defence.PhisicalDefence = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.PhisicalDefence);
                    else if (String.Compare(formula.Key, "Evade") == 0) play.defence.PhisicalEvade = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.PhisicalEvade);
                    else if (String.Compare(formula.Key, "MagicDefence") == 0) play.defence.MagicalDefence = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.MagicalDefence);
                    else if (String.Compare(formula.Key, "MagicEvade") == 0) play.defence.MagicalEvade = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.MagicalEvade);
                    else if (String.Compare(formula.Key, "PlayerCategory") == 0) play.category = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.category);
                    else if (String.Compare(formula.Key, "MPCostFactor") == 0) play.mpCostFactor = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.mpCostFactor);
                }
            }
        }

        public void TriggerOnBattleStart(ref Int32 backAttackChance, ref Int32 preemptiveChance, ref Int32 preemptivePriority)
        {
            for (Int32 i = 0; i < BattleStartEffect.Count; i++)
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
                    if (String.Compare(formula.Key, "BackAttack") == 0) backAttackChance = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), backAttackChance);
                    else if (String.Compare(formula.Key, "Preemptive") == 0) preemptiveChance = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), preemptiveChance);
                }
                preemptivePriority += BattleStartEffect[i].PreemptivePriorityDelta;
            }
        }

        public Boolean TriggerOnBattleResult(PLAYER play, BONUS bonus, List<FF9ITEM> bonus_item, String when, UInt32 fleeGil)
        {
            Boolean triggeredAtLeastOnce = false;
            for (Int32 i = 0; i < BattleResultEffect.Count; i++)
                if (String.Compare(BattleResultEffect[i].When, when) == 0)
                {
                    if (BattleResultEffect[i].Condition.Length > 0)
                    {
                        Expression c = new Expression(BattleResultEffect[i].Condition);
                        NCalcUtility.InitializeExpressionPlayer(ref c, play);
                        NCalcUtility.InitializeExpressionBonus(ref c, bonus, bonus_item);
                        c.Parameters["IsFlee"] = FF9StateSystem.Common.FF9.btl_result == 4;
                        c.Parameters["IsFleeByLuck"] = FF9StateSystem.Common.FF9.btl_result == 4 && (FF9StateSystem.Common.FF9.btl_flag & 4) == 0;
                        c.Parameters["FleeGil"] = fleeGil;
                        c.Parameters["Status"] = play.status;
                        c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
                    }
                    triggeredAtLeastOnce = true;
                    foreach (KeyValuePair<String, String> formula in BattleResultEffect[i].Formula)
                    {
                        Expression e = new Expression(formula.Value);
                        NCalcUtility.InitializeExpressionPlayer(ref e, play);
                        NCalcUtility.InitializeExpressionBonus(ref e, bonus, bonus_item);
                        e.Parameters["IsFlee"] = FF9StateSystem.Common.FF9.btl_result == 4;
                        e.Parameters["IsFleeByLuck"] = FF9StateSystem.Common.FF9.btl_result == 4 && (FF9StateSystem.Common.FF9.btl_flag & 4) == 0;
                        e.Parameters["FleeGil"] = fleeGil;
                        e.Parameters["Status"] = play.status;
                        e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (String.Compare(formula.Key, "FleeGil") == 0) fleeGil = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), fleeGil);
                        else if (String.Compare(formula.Key, "Status") == 0) play.status = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.status);
                        else if (String.Compare(formula.Key, "BonusAP") == 0) bonus.ap = (UInt16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), bonus.ap);
                        else if (String.Compare(formula.Key, "BonusCard") == 0) bonus.card = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), bonus.card);
                        else if (String.Compare(formula.Key, "BonusExp") == 0) bonus.exp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), bonus.exp);
                        else if (String.Compare(formula.Key, "BonusGil") == 0) bonus.gil = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), bonus.gil);
                        else
                        {
                            for (Int32 j = 0; j < BattleResultUI.ItemMax; j++)
                                if (String.Compare(formula.Key, "BonusItem" + (j + 1)) == 0)
								{
                                    while (bonus_item.Count < j) bonus_item.Add(new FF9ITEM(Byte.MaxValue, 0));
                                    bonus_item[j].id = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), bonus_item[j].id);
                                    if (bonus_item[j].id == Byte.MaxValue) bonus_item[j].count = 0;
                                }
                                else if (String.Compare(formula.Key, "BonusItemCount" + (j + 1)) == 0)
                                {
                                    while (bonus_item.Count < j) bonus_item.Add(new FF9ITEM(Byte.MaxValue, 0));
                                    bonus_item[j].count = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), bonus_item[j].count);
                                    if (bonus_item[j].count == 0) bonus_item[j].id = Byte.MaxValue;
                                }
                        }
                    }
                }
            return triggeredAtLeastOnce;
        }

        public void TriggerOnStatusInit(BattleUnit unit)
        {
            for (Int32 i = 0; i < StatusEffect.Count; i++)
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
                unit.PermanentStatus |= StatusEffect[i].PermanentStatus;
                unit.CurrentStatus |= StatusEffect[i].InitialStatus;
                unit.ResistStatus |= StatusEffect[i].ResistStatus;
                if (StatusEffect[i].InitialATB >= 0)
                    unit.CurrentAtb = (Int16)Math.Max(unit.MaximumAtb - 1, unit.MaximumAtb * StatusEffect[i].InitialATB / 100);
            }
        }

        public void TriggerOnAbility(BattleCalculator calc, String when, Boolean asTarget)
        {
            if (Id >= 0 && calc.Context.DisabledSA[Id])
                return;
            Boolean canMove = asTarget ? !calc.Target.IsUnderAnyStatus(BattleStatus.NoReaction) : !calc.Caster.IsUnderAnyStatus(BattleStatus.NoReaction);
            for (Int32 i = 0; i < AbilityEffect.Count; i++)
                if (AbilityEffect[i].AsTarget == asTarget && (canMove || AbilityEffect[i].EvenImmobilized) && String.Compare(AbilityEffect[i].When, when) == 0)
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
                        Expression e = new Expression(formula.Value);
                        NCalcUtility.InitializeExpressionUnit(ref e, caster, "Caster");
                        NCalcUtility.InitializeExpressionUnit(ref e, target, "Target");
                        NCalcUtility.InitializeExpressionAbilityContext(ref e, calc);
                        e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (String.Compare(formula.Key, "CasterHP") == 0) caster.CurrentHp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.CurrentHp);
                        else if (String.Compare(formula.Key, "CasterMP") == 0) caster.CurrentMp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.CurrentMp);
                        else if (String.Compare(formula.Key, "CasterATB") == 0) caster.CurrentAtb = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.CurrentAtb);
                        else if (String.Compare(formula.Key, "CasterTrance") == 0) caster.Trance = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Trance);
                        else if (String.Compare(formula.Key, "CasterCurrentStatus") == 0) cCurStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (UInt32)cCurStat);
                        else if (String.Compare(formula.Key, "CasterPermanentStatus") == 0) cAutoStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (UInt32)cAutoStat);
                        else if (String.Compare(formula.Key, "CasterResistStatus") == 0) cResistStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (UInt32)cResistStat);
                        else if (String.Compare(formula.Key, "CasterHalfElement") == 0) caster.HalfElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.HalfElement);
                        else if (String.Compare(formula.Key, "CasterGuardElement") == 0) caster.GuardElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.GuardElement);
                        else if (String.Compare(formula.Key, "CasterAbsorbElement") == 0) caster.AbsorbElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.AbsorbElement);
                        else if (String.Compare(formula.Key, "CasterWeakElement") == 0) caster.WeakElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.WeakElement);
                        else if (String.Compare(formula.Key, "CasterBonusElement") == 0) caster.BonusElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.BonusElement);
                        else if (String.Compare(formula.Key, "CasterSpeed") == 0) caster.Dexterity = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Dexterity);
                        else if (String.Compare(formula.Key, "CasterStrength") == 0) caster.Strength = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Strength);
                        else if (String.Compare(formula.Key, "CasterMagic") == 0) caster.Magic = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Magic);
                        else if (String.Compare(formula.Key, "CasterSpirit") == 0) caster.Will = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Will);
                        else if (String.Compare(formula.Key, "CasterDefence") == 0) caster.PhisicalDefence = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.PhisicalDefence);
                        else if (String.Compare(formula.Key, "CasterEvade") == 0) caster.PhisicalEvade = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.PhisicalEvade);
                        else if (String.Compare(formula.Key, "CasterMagicDefence") == 0) caster.MagicDefence = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.MagicDefence);
                        else if (String.Compare(formula.Key, "CasterMagicEvade") == 0) caster.MagicEvade = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.MagicEvade);
                        else if (String.Compare(formula.Key, "CasterRow") == 0) caster.Row = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.Row);
                        else if (String.Compare(formula.Key, "CasterIsStrengthModified") == 0) caster.StatModifier[0] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), caster.StatModifier[0]);
                        else if (String.Compare(formula.Key, "CasterIsMagicModified") == 0) caster.StatModifier[1] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), caster.StatModifier[1]);
                        else if (String.Compare(formula.Key, "CasterIsDefenceModified") == 0) caster.StatModifier[2] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), caster.StatModifier[2]);
                        else if (String.Compare(formula.Key, "CasterIsEvadeModified") == 0) caster.StatModifier[3] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), caster.StatModifier[3]);
                        else if (String.Compare(formula.Key, "CasterIsMagicDefenceModified") == 0) caster.StatModifier[4] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), caster.StatModifier[4]);
                        else if (String.Compare(formula.Key, "CasterIsMagicEvadeModified") == 0) caster.StatModifier[5] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), caster.StatModifier[5]);
                        else if (String.Compare(formula.Key, "CasterCriticalRateBonus") == 0) caster.CriticalRateBonus = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.CriticalRateBonus);
                        else if (String.Compare(formula.Key, "CasterCriticalRateWeakening") == 0) caster.CriticalRateWeakening = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.CriticalRateWeakening);
                        else if (String.Compare(formula.Key, "TargetHP") == 0) target.CurrentHp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.CurrentHp);
                        else if (String.Compare(formula.Key, "TargetMP") == 0) target.CurrentMp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.CurrentMp);
                        else if (String.Compare(formula.Key, "TargetATB") == 0) target.CurrentAtb = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.CurrentAtb);
                        else if (String.Compare(formula.Key, "TargetTrance") == 0) target.Trance = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Trance);
                        else if (String.Compare(formula.Key, "TargetCurrentStatus") == 0) tCurStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (UInt32)tCurStat);
                        else if (String.Compare(formula.Key, "TargetPermanentStatus") == 0) tAutoStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (UInt32)tAutoStat);
                        else if (String.Compare(formula.Key, "TargetResistStatus") == 0) tResistStat = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (UInt32)tResistStat);
                        else if (String.Compare(formula.Key, "TargetHalfElement") == 0) target.HalfElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.HalfElement);
                        else if (String.Compare(formula.Key, "TargetGuardElement") == 0) target.GuardElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.GuardElement);
                        else if (String.Compare(formula.Key, "TargetAbsorbElement") == 0) target.AbsorbElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.AbsorbElement);
                        else if (String.Compare(formula.Key, "TargetWeakElement") == 0) target.WeakElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.WeakElement);
                        else if (String.Compare(formula.Key, "TargetBonusElement") == 0) target.BonusElement = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.BonusElement);
                        else if (String.Compare(formula.Key, "TargetSpeed") == 0) target.Dexterity = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Dexterity);
                        else if (String.Compare(formula.Key, "TargetStrength") == 0) target.Strength = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Strength);
                        else if (String.Compare(formula.Key, "TargetMagic") == 0) target.Magic = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Magic);
                        else if (String.Compare(formula.Key, "TargetSpirit") == 0) target.Will = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Will);
                        else if (String.Compare(formula.Key, "TargetDefence") == 0) target.PhisicalDefence = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.PhisicalDefence);
                        else if (String.Compare(formula.Key, "TargetEvade") == 0) target.PhisicalEvade = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.PhisicalEvade);
                        else if (String.Compare(formula.Key, "TargetMagicDefence") == 0) target.MagicDefence = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.MagicDefence);
                        else if (String.Compare(formula.Key, "TargetMagicEvade") == 0) target.MagicEvade = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.MagicEvade);
                        else if (String.Compare(formula.Key, "TargetRow") == 0) target.Row = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Row);
                        else if (String.Compare(formula.Key, "TargetIsStrengthModified") == 0) target.StatModifier[0] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), target.StatModifier[0]);
                        else if (String.Compare(formula.Key, "TargetIsMagicModified") == 0) target.StatModifier[1] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), target.StatModifier[1]);
                        else if (String.Compare(formula.Key, "TargetIsDefenceModified") == 0) target.StatModifier[2] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), target.StatModifier[2]);
                        else if (String.Compare(formula.Key, "TargetIsEvadeModified") == 0) target.StatModifier[3] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), target.StatModifier[3]);
                        else if (String.Compare(formula.Key, "TargetIsMagicDefenceModified") == 0) target.StatModifier[4] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), target.StatModifier[4]);
                        else if (String.Compare(formula.Key, "TargetIsMagicEvadeModified") == 0) target.StatModifier[5] = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), target.StatModifier[5]);
                        else if (String.Compare(formula.Key, "TargetCriticalRateBonus") == 0) target.CriticalRateBonus = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.CriticalRateBonus);
                        else if (String.Compare(formula.Key, "TargetCriticalRateWeakening") == 0) target.CriticalRateWeakening = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.CriticalRateWeakening);
                        else if (String.Compare(formula.Key, "EffectCasterFlags") == 0) caster.Flags = (CalcFlag)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)caster.Flags);
                        else if (String.Compare(formula.Key, "CasterHPDamage") == 0) caster.HpDamage = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.HpDamage);
                        else if (String.Compare(formula.Key, "CasterMPDamage") == 0) caster.MpDamage = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), caster.MpDamage);
                        else if (String.Compare(formula.Key, "EffectTargetFlags") == 0) target.Flags = (CalcFlag)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)target.Flags);
                        else if (String.Compare(formula.Key, "HPDamage") == 0) target.HpDamage = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.HpDamage);
                        else if (String.Compare(formula.Key, "MPDamage") == 0) target.MpDamage = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.MpDamage);
                        else if (String.Compare(formula.Key, "FigureInfo") == 0) target.Data.fig_info = (UInt16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), target.Data.fig_info);
                        else if (String.Compare(formula.Key, "Power") == 0) command.Power = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.Power);
                        else if (String.Compare(formula.Key, "AbilityStatus") == 0) command.AbilityStatus = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (UInt32)command.AbilityStatus);
                        else if (String.Compare(formula.Key, "AbilityElement") == 0) command.Element = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)command.Element);
                        else if (String.Compare(formula.Key, "AbilityElementForBonus") == 0) command.Element = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)command.Element);
                        else if (String.Compare(formula.Key, "IsShortRanged") == 0) command.IsShortRange = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), command.IsShortRange);
                        else if (String.Compare(formula.Key, "IsDrain") == 0) context.IsDrain = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), context.IsDrain);
                        else if (String.Compare(formula.Key, "AbilityCategory") == 0) command.AbilityCategory = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.AbilityCategory);
                        else if (String.Compare(formula.Key, "AbilityFlags") == 0) command.AbilityType = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.AbilityType);
                        else if (String.Compare(formula.Key, "Attack") == 0) context.Attack = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.Attack);
                        else if (String.Compare(formula.Key, "AttackPower") == 0) context.AttackPower = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.AttackPower);
                        else if (String.Compare(formula.Key, "DefencePower") == 0) context.DefensePower = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.DefensePower);
                        else if (String.Compare(formula.Key, "StatusRate") == 0) context.StatusRate = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.StatusRate);
                        else if (String.Compare(formula.Key, "HitRate") == 0) context.HitRate = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.HitRate);
                        else if (String.Compare(formula.Key, "Evade") == 0) context.Evade = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.Evade);
                        else if (String.Compare(formula.Key, "EffectFlags") == 0) context.Flags = (BattleCalcFlags)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (UInt16)context.Flags);
                        else if (String.Compare(formula.Key, "DamageModifierCount") == 0) context.DamageModifierCount = (SByte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.DamageModifierCount);
                        else if (String.Compare(formula.Key, "TranceIncrease") == 0) context.TranceIncrease = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.TranceIncrease);
                        else if (String.Compare(formula.Key, "ItemSteal") == 0) context.ItemSteal = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), context.ItemSteal);
                        else if (String.Compare(formula.Key, "Gil") == 0) GameState.Gil = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), GameState.Gil);
                        else if (String.Compare(formula.Key, "Counter") == 0)
						{
                            Int32 attackId = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), 176);
                            if (asTarget)
                                btl_cmd.SetCounter(target.Data, BattleCommandId.Counter, attackId, caster.Id);
                            else
                                btl_cmd.SetCounter(caster.Data, BattleCommandId.Counter, attackId, target.Id);
                        }
                        else if (String.Compare(formula.Key, "ReturnMagic") == 0)
                        {
                            //NCalcUtility.ConvertNCalcResult(e.Evaluate(), 176);
                            if (asTarget)
                                btl_abil.TryReturnMagic(target, caster, command);
                            else
                                btl_abil.TryReturnMagic(caster, target, command);
                        }
                        else if (String.Compare(formula.Key, "AutoItem") == 0)
                        {
                            Int32 itemId = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), 236);
                            if (Configuration.Battle.AutoPotionOverhealLimit >= 0 && Id == 56 && asTarget && (itemId == 236 || itemId == 237))
                            {
                                // A bit hacky but painlessly keeps the "AutoPotionOverhealLimit" option
                                btl_abil.CheckAutoItemAbility(target, command);
                            }
                            else
                            {
                                if (ff9item.FF9Item_GetCount(itemId) != 0)
                                {
                                    UIManager.Battle.ItemRequest(itemId);
                                    if (asTarget)
                                        btl_cmd.SetCounter(target.Data, BattleCommandId.AutoPotion, itemId, target.Id);
                                    else
                                        btl_cmd.SetCounter(caster.Data, BattleCommandId.AutoPotion, itemId, caster.Id);
                                }
                            }
                        }
                    }
                    if (caster.PermanentStatus != cAutoStat) btl_stat.MakeStatusesPermanent(caster.Data, caster.PermanentStatus & ~cAutoStat, false);
                    if (caster.CurrentStatus != cCurStat) btl_stat.RemoveStatuses(caster.Data, caster.CurrentStatus & ~cCurStat);
                    if (caster.ResistStatus != cResistStat) caster.ResistStatus = cResistStat;
                    if (caster.PermanentStatus != cAutoStat) btl_stat.MakeStatusesPermanent(caster.Data, cAutoStat & ~caster.PermanentStatus, true);
                    if (caster.CurrentStatus != cCurStat) btl_stat.AlterStatuses(caster.Data, cCurStat & ~caster.CurrentStatus);
                    if (target.PermanentStatus != tAutoStat) btl_stat.MakeStatusesPermanent(target.Data, target.PermanentStatus & ~tAutoStat, false);
                    if (target.CurrentStatus != tCurStat) btl_stat.RemoveStatuses(target.Data, target.CurrentStatus & ~tCurStat);
                    if (target.ResistStatus != tResistStat) target.ResistStatus = tResistStat;
                    if (target.PermanentStatus != tAutoStat) btl_stat.MakeStatusesPermanent(target.Data, tAutoStat & ~target.PermanentStatus, true);
                    if (target.CurrentStatus != tCurStat) btl_stat.AlterStatuses(target.Data, tCurStat & ~target.CurrentStatus);
                    foreach (Int32 saIndex in AbilityEffect[i].DisableSA)
                        if (saIndex < 64)
                            context.DisabledSA[saIndex] = true;
                }
        }

        public void TriggerOnCommand(BattleUnit abilityUser, BattleCommand command, ref UInt16 tryCover)
        {
            Boolean canMove = !abilityUser.IsUnderAnyStatus(BattleStatus.NoReaction);
            BattleUnit caster = null;
            BattleUnit target = null;
            for (Int32 i = 0; i < CommandEffect.Count; i++)
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
                        if (String.Compare(formula.Key, "Power") == 0) command.Power = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.Power);
                        else if (String.Compare(formula.Key, "AbilityStatus") == 0) command.AbilityStatus = (BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (UInt32)command.AbilityStatus);
                        else if (String.Compare(formula.Key, "AbilityElement") == 0) command.Element = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)command.Element);
                        else if (String.Compare(formula.Key, "AbilityElementForBonus") == 0) command.Element = (EffectElement)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Byte)command.Element);
                        else if (String.Compare(formula.Key, "IsShortRanged") == 0) command.IsShortRange = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), command.IsShortRange);
                        else if (String.Compare(formula.Key, "AbilityCategory") == 0) command.AbilityCategory = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.AbilityCategory);
                        else if (String.Compare(formula.Key, "AbilityFlags") == 0) command.AbilityType = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.AbilityType);
                        else if (String.Compare(formula.Key, "IsReflectNull") == 0) command.IsReflectNull = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), command.IsReflectNull);
                        else if (String.Compare(formula.Key, "IsMeteorMiss") == 0) command.Data.info.meteor_miss = (Byte)(NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), command.IsReflectNull) ? 1 : 0);
                        else if (String.Compare(formula.Key, "IsShortSummon") == 0) command.IsShortSummon = NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), command.IsShortSummon);
                        else if (String.Compare(formula.Key, "TryCover") == 0) tryCover |= NCalcUtility.EvaluateNCalcCondition(e.Evaluate(), false) ? abilityUser.Id : (UInt16)0;
                        else if (String.Compare(formula.Key, "ScriptId") == 0) command.ScriptId = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.ScriptId);
                        else if (String.Compare(formula.Key, "HitRate") == 0) command.HitRate = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.HitRate);
                        else if (String.Compare(formula.Key, "CommandTargetId") == 0) command.Data.tar_id = (UInt16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), command.Data.tar_id);
                    }
                }
        }

        public void ParseFeatures(Int32 id, String featureCode)
        {
            Id = id;
            MatchCollection codeMatches = new Regex(@"^(Permanent|BattleStart|BattleResult|StatusInit|Ability|Command)\b", RegexOptions.Multiline).Matches(featureCode);
            for (Int32 i = 0; i < codeMatches.Count; i++)
            {
                String saCode = codeMatches[i].Groups[1].Value;
                Int32 endPos, startPos = codeMatches[i].Groups[1].Captures[0].Index + codeMatches[i].Groups[1].Value.Length;
                if (i + 1 == codeMatches.Count)
                    endPos = featureCode.Length;
                else
                    endPos = codeMatches[i + 1].Groups[1].Captures[0].Index;
                String saArgs = featureCode.Substring(startPos, endPos - startPos);
                if (String.Compare(saCode, "Permanent") == 0)
                {
                    SupportingAbilityEffectPermanent newEffect = new SupportingAbilityEffectPermanent();
                    foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        if (String.Compare(formula.Groups[1].Value, "Condition") == 0)
                            newEffect.Condition = formula.Groups[2].Value;
                        else
                            newEffect.Formula[formula.Groups[1].Value] = formula.Groups[2].Value;
                    }
                    PermanentEffect.Add(newEffect);
                }
                else if (String.Compare(saCode, "BattleStart") == 0)
                {
                    SupportingAbilityEffectBattleStartType newEffect = new SupportingAbilityEffectBattleStartType();
                    Match priorityDelta = new Regex(@"\bPreemptivePriority\s+([\+-]?\d+)").Match(saArgs);
                    if (priorityDelta.Success)
                        Int32.TryParse(priorityDelta.Groups[1].Value, out newEffect.PreemptivePriorityDelta);
                    foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        if (String.Compare(formula.Groups[1].Value, "Condition") == 0)
                            newEffect.Condition = formula.Groups[2].Value;
                        else
                            newEffect.Formula[formula.Groups[1].Value] = formula.Groups[2].Value;
                    }
                    BattleStartEffect.Add(newEffect);
                }
                else if (String.Compare(saCode, "BattleResult") == 0)
                {
                    SupportingAbilityEffectBattleResult newEffect = new SupportingAbilityEffectBattleResult();
                    Match when = new Regex(@"\bWhen(\w+)\b").Match(saArgs);
                    if (when.Success)
                        newEffect.When = when.Groups[1].Value;
                    foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        if (String.Compare(formula.Groups[1].Value, "Condition") == 0)
                            newEffect.Condition = formula.Groups[2].Value;
                        else
                            newEffect.Formula[formula.Groups[1].Value] = formula.Groups[2].Value;
                    }
                    BattleResultEffect.Add(newEffect);
                }
                else if (String.Compare(saCode, "StatusInit") == 0)
                {
                    SupportingAbilityEffectBattleInitStatus newEffect = new SupportingAbilityEffectBattleInitStatus();
                    foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        if (String.Compare(formula.Groups[1].Value, "Condition") == 0)
                            newEffect.Condition = formula.Groups[2].Value;
                    }
                    foreach (Match statusMatch in new Regex(@"\b((Auto|Initial|Resist)Status|InitialATB)\s+(\w+|\d+)\b").Matches(saArgs))
                    {
                        if (String.Compare(statusMatch.Groups[1].Value, "InitialATB") == 0)
                        {
                            Int32.TryParse(statusMatch.Groups[3].Value, out newEffect.InitialATB);
                        }
                        else
                        {
                            BattleStatus stat = 0;
                            foreach (BattleStatus s in (BattleStatus[])Enum.GetValues(typeof(BattleStatus)))
                                if (String.Compare(statusMatch.Groups[3].Value, s.ToString()) == 0)
                                {
                                    stat = s;
                                    break;
                                }
                            if (stat != 0)
                            {
                                if (String.Compare(statusMatch.Groups[2].Value, "Auto") == 0)
                                    newEffect.PermanentStatus |= stat;
                                else if (String.Compare(statusMatch.Groups[2].Value, "Initial") == 0)
                                    newEffect.InitialStatus |= stat;
                                else if (String.Compare(statusMatch.Groups[2].Value, "Resist") == 0)
                                    newEffect.ResistStatus |= stat;
                            }
                        }
					}
                    StatusEffect.Add(newEffect);
                }
                else if (String.Compare(saCode, "Ability") == 0)
                {
                    SupportingAbilityEffectAbilityUse newEffect = new SupportingAbilityEffectAbilityUse();
                    Match when = new Regex(@"\bWhen(\w+)\b").Match(saArgs);
                    if (when.Success)
                        newEffect.When = when.Groups[1].Value;
                    foreach (Match effect in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        if (String.Compare(effect.Groups[1].Value, "Condition") == 0)
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
                                newEffect.DisableSA.Add(saValue);
                    AbilityEffect.Add(newEffect);
                }
                else if (String.Compare(saCode, "Command") == 0)
                {
                    SupportingAbilityEffectCommandStart newEffect = new SupportingAbilityEffectCommandStart();
                    foreach (Match effect in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(saArgs))
                    {
                        if (String.Compare(effect.Groups[1].Value, "Condition") == 0)
                            newEffect.Condition = effect.Groups[2].Value;
                        else
                            newEffect.Effect[effect.Groups[1].Value] = effect.Groups[2].Value;
                    }
                    if (new Regex(@"\bEvenImmobilized\b").Match(saArgs).Success) newEffect.EvenImmobilized = true;
                    CommandEffect.Add(newEffect);
                }
            }
        }
    }
}