using System;
using System.Collections.Generic;
using FF9;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Scripts;
using UnityEngine;
// ReSharper disable PossibleNullReferenceException

namespace Memoria
{
    public static class SBattleCalculator
    {
        public static readonly BattleScriptFactory[] BaseScripts = ScriptsLoader.GetBaseScripts();
        public static readonly Dictionary<Int32, BattleScriptFactory> ExtendedScripts = ScriptsLoader.GetExtendedScripts();

        public static void Calc(BattleUnit caster, BattleUnit target, BattleCommand command, Byte scriptId)
        {
            CalcMain(caster.Data, target.Data, command, scriptId);
        }

        internal static void CalcMain(BTL_DATA caster, BTL_DATA target, BattleCommand command, Byte scriptId)
        {
            if (caster == null || target == null)
			{
                if (scriptId == btl_sys.fleeScriptId)
                {
                    BattleScriptFactory fleeFactory = FindScriptFactory(scriptId);
                    if (fleeFactory != null)
                        fleeFactory(new BattleCalculator()).Perform();
                    else
                        Log.Warning($"Unknown script id: {scriptId}");
                }
                return;
			}
            command.ScriptId = scriptId;
            BattleCalculator v = new BattleCalculator(caster, target, command);
            BattleScriptFactory factory = FindScriptFactory(scriptId);
            ++btl_cmd.cmd_effect_counter;
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(v.Caster.Data.sa))
                saFeature.TriggerOnAbility(v, "BattleScriptStart", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(v.Target.Data.sa))
                saFeature.TriggerOnAbility(v, "BattleScriptStart", true);

            if (Configuration.Battle.CustomBattleFlagsMeaning == 1)
            {
                if (command.IsShortRange)
                {
                    v.BonusBackstabAndPenaltyLongDistanceAsDamageModifiers();
                    if ((command.AbilityCategory & 8) != 0 && v.Target.IsUnderAnyStatus(BattleStatus.Vanish)) // Is Physical
                        v.Context.Flags |= BattleCalcFlags.Miss;
                }
                if ((command.AbilityType & 0x10) != 0 && caster.bi.player != 0) // Use weapon properties
                {
                    v.ApplyElementAsDamageModifiers(v.Caster.WeaponElement, v.Caster.WeaponElement);
                }
                if ((v.Context.Flags & (BattleCalcFlags.Miss | BattleCalcFlags.Guard)) != 0)
                {
                    SBattleCalculator.CalcResult(v);
                    return;
                }
            }
            if ((command.AbilityCategory & 8) != 0 && v.Target.TryKillFrozen()) // Is Physical
            {
                SBattleCalculator.CalcResult(v);
                return;
            }
            if (factory != null)
            {
                IBattleScript script = factory(v);
                script.Perform();
            }
            else
            {
                Log.Warning($"Unknown script id: {scriptId}");
            }

            CalcResult(v);
        }

        public static void CalcResult(BattleCalculator v)
        {
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(v.Caster.Data.sa))
                saFeature.TriggerOnAbility(v, "BattleScriptEnd", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(v.Target.Data.sa))
                saFeature.TriggerOnAbility(v, "BattleScriptEnd", true);
            BTL_DATA target = v.Target.Data;
            BTL_DATA caster = v.Caster.Data;
            v.ConsumeMpAttack();
            if ((v.Context.Flags & BattleCalcFlags.Guard) != 0)
                target.fig_info |= Param.FIG_INFO_GUARD;
            else if ((v.Context.Flags & BattleCalcFlags.Miss) != 0)
            {
                target.fig_info |= Param.FIG_INFO_MISS;
                if (target.cur.hp <= 0)
                    v.Context.Flags &= ~BattleCalcFlags.Dodge;
                if ((v.Context.Flags & BattleCalcFlags.Dodge) != 0)
                {
                    if (target.bi.player != 0)
                    {
                        btl_mot.setMotion(target, 16);
                        target.evt.animFrame = 0;
                        Int32 num = btl_mot.GetDirection(target);
                        target.evt.rotBattle.eulerAngles = new Vector3(target.evt.rotBattle.eulerAngles.x, num, target.evt.rotBattle.eulerAngles.z);
                        target.rot.eulerAngles = new Vector3(target.rot.eulerAngles.x, num, target.rot.eulerAngles.z);
                    }
                    else if (target.bi.slave == 0)
                    {
                        if (Configuration.Battle.FloatEvadeBonus > 0 && v.Target.IsUnderPermanentStatus(BattleStatus.Float))
                            target.pos[1] = target.pos[1] - -600f;
                        target.pos[2] -= -400f;
                    }
                    else
                        btl_util.GetMasterEnemyBtlPtr().Data.pos[2] -= -400f;
                    target.bi.dodge = 1;
                    v.Command.Data.info.dodge = 1;
                }
            }
            else
            {
                if (Configuration.Battle.CustomBattleFlagsMeaning == 1 && (v.Command.AbilityType & 0x20) != 0) // Has critical
                    v.TryCriticalHit();
                // Note: weapon statuses are added before damage (unlike vanilla), like spell statuses
                if ((v.Context.Flags & BattleCalcFlags.AddStat) != 0 && target.cur.hp > 0)
                {
                    if ((FF9StateSystem.Battle.FF9Battle.add_status[(int)caster.weapon.StatusIndex].Value & BattleStatus.Death) == 0 || !Status.checkCurStat(target, BattleStatus.EasyKill))
                    {
                        v.Target.TryAlterStatuses(FF9StateSystem.Battle.FF9Battle.add_status[(int)caster.weapon.StatusIndex].Value, false);
                    }
                }
                if ((v.Command.AbilityCategory & 8) != 0) // Is Physical
                {
                    if (Configuration.Battle.CustomBattleFlagsMeaning == 1)
                        v.RaiseTrouble();
                    if ((v.Context.AddedStatuses & BattleStatus.Confuse) == 0)
                        v.Target.RemoveStatus(BattleStatus.Confuse);
                    if ((v.Context.AddedStatuses & BattleStatus.Sleep) == 0)
                        v.Target.RemoveStatus(BattleStatus.Sleep);
                }
                if ((v.Command.AbilityCategory & 16) != 0 && (v.Context.AddedStatuses & BattleStatus.Vanish) == 0) // Is Magical
                    v.Target.RemoveStatus(BattleStatus.Vanish);

                if (v.Target.Flags != 0)
                {
                    // DamageModifierCount > 0 -> damage is multiplied by 1.5, 2, 2.25, 2.5, 2.625, 2.75...
                    // DamageModifierCount < 0 -> damage is divided by 2, 4, 8, 16, 32...
                    Single modifier_factor = 1.0f;
                    Single modifier_bonus = 0.5f;
                    Byte modifier_index = 0;
                    if (v.Caster.IsUnderAnyStatus(BattleStatus.Trance) && v.Caster.PlayerIndex == CharacterIndex.Steiner)
                        modifier_bonus = 1.0f;
                    while (v.Context.DamageModifierCount > 0)
                    {
                        modifier_factor += modifier_bonus;
                        modifier_index++;
                        if (modifier_index >= 2)
                        {
                            modifier_bonus *= 0.5f;
                            modifier_index = 0;
                        }
                        --v.Context.DamageModifierCount;
                    }
                    while (v.Context.DamageModifierCount < 0)
                    {
                        modifier_factor *= 0.5f;
                        ++v.Context.DamageModifierCount;
                    }
                    target.fig_info |= (UInt16)v.Target.Flags;
                    if ((v.Target.Flags & CalcFlag.HpAlteration) != 0)
                    {
                        v.Target.HpDamage = (Int32)Math.Round(modifier_factor * v.Target.HpDamage);
                        if (v.Command.Data.info.reflec == 1)
                        {
                            UInt16 num1 = 0;
                            for (UInt16 index = 0; index < 4; ++index)
                            {
                                if ((caster.reflec.tar_id[index] & target.btl_id) != 0)
                                    ++num1;
                            }
                            v.Target.HpDamage *= num1;
                        }
                        if (v.Target.HpDamage > 9999)
                            v.Target.HpDamage = 9999;
                        if ((v.Target.Flags & CalcFlag.HpRecovery) != 0)
                        {
                            btl_para.SetRecover(target, (UInt32)v.Target.HpDamage);
                        }
                        else
                        {
                            if (FF9StateSystem.Settings.IsDmg9999 && caster.bi.player != 0 && (v.Command.Data.cmd_no != BattleCommandId.StageMagicZidane && v.Command.Data.cmd_no != BattleCommandId.StageMagicBlank) && (v.Command.Data.cmd_no != BattleCommandId.StageMagicMarcus && v.Command.Data.cmd_no != BattleCommandId.StageMagicCinna))
                                v.Target.HpDamage = 9999;
                            btl_para.SetDamage(v.Target, v.Target.HpDamage, !CheckDamageMotion(v) ? (Byte)0 : (Byte)1);
                            CheckDamageReaction(v);
                        }
                        if (v.Context.IsDrain)
						{
                            v.Caster.Flags |= CalcFlag.HpAlteration;
                            if ((v.Target.Flags & CalcFlag.HpRecovery) == 0)
                                v.Caster.Flags |= CalcFlag.HpRecovery;
                            else
                                v.Caster.Flags &= ~CalcFlag.HpRecovery;
                            v.Caster.HpDamage = v.Target.HpDamage;
                        }
                    }

                    if ((v.Target.Flags & CalcFlag.MpAlteration) != 0)
                    {
                        v.Target.MpDamage = (Int32)Math.Round(modifier_factor * v.Target.MpDamage);
                        if (v.Target.MpDamage > 999)
                            v.Target.MpDamage = 999;
                        if ((v.Target.Flags & CalcFlag.MpRecovery) != 0)
                            btl_para.SetMpRecover(target, (UInt32)v.Target.MpDamage);
                        else
                            btl_para.SetMpDamage(target, (UInt32)v.Target.MpDamage);
                        if (v.Context.IsDrain)
                        {
                            v.Caster.Flags |= CalcFlag.MpAlteration;
                            if ((v.Target.Flags & CalcFlag.MpRecovery) == 0)
                                v.Caster.Flags |= CalcFlag.MpRecovery;
                            else
                                v.Caster.Flags &= ~CalcFlag.MpRecovery;
                            v.Caster.MpDamage = v.Target.MpDamage;
                        }
                    }
                }
                else if ((v.Context.Flags & BattleCalcFlags.DirectHP) != 0)
                {
                    if (CheckDamageMotion(v))
                        btl_mot.SetDamageMotion(v.Target);
                    CheckDamageReaction(v);
                }
                if (v.Caster.Flags != 0)
                {
                    caster.fig_info |= (UInt16)v.Caster.Flags;
                    if ((v.Caster.Flags & CalcFlag.HpAlteration) != 0)
                    {
                        if (v.Caster.HpDamage > 9999)
                            v.Caster.HpDamage = 9999;
                        if ((v.Caster.Flags & CalcFlag.HpRecovery) != 0)
                            btl_para.SetRecover(caster, (UInt32)v.Caster.HpDamage);
                        else
                            btl_para.SetDamage(v.Caster, v.Caster.HpDamage, 0);
                    }
                    if ((v.Caster.Flags & CalcFlag.MpAlteration) != 0)
                    {
                        if (v.Caster.MpDamage > 999)
                            v.Caster.MpDamage = 999;
                        if ((v.Caster.Flags & CalcFlag.MpRecovery) != 0)
                            btl_para.SetMpRecover(caster, (UInt32)v.Caster.MpDamage);
                        else
                            btl_para.SetMpDamage(caster, (UInt32)v.Caster.MpDamage);
                    }
                }
                //if ((v.Context.Flags & BattleCalcFlags.AddStat) != 0 && target.cur.hp > 0 && ((FF9StateSystem.Battle.FF9Battle.add_status[caster.weapon.StatusIndex].Value & BattleStatus.Death) == 0 || !v.Target.IsUnderStatus(BattleStatus.EasyKill)))
                //{
                //    v.Target.TryAlterStatuses((BattleStatus)FF9StateSystem.Battle.FF9Battle.add_status[caster.weapon.StatusIndex].Value, false);
                //}
                if (target.bi.player != 0 && FF9StateSystem.Settings.IsHpMpFull && target.cur.hp != 0)
                {
                    target.cur.hp = target.max.hp;
                    target.cur.mp = target.max.mp;
                }
            }
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(v.Caster.Data.sa))
                saFeature.TriggerOnAbility(v, "EffectDone", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(v.Target.Data.sa))
                saFeature.TriggerOnAbility(v, "EffectDone", true);
            if (target.bi.player != 0 || FF9StateSystem.Battle.isDebug)
                return;
            UInt16 targetId = target.bi.slave == 0 ? target.btl_id : (UInt16)16;
            UInt16 commandAndScript = (UInt16)((UInt32)v.Command.Data.cmd_no << 8 | v.Command.Data.sub_no);
            if (caster.bi.player != 0 && !btl_stat.CheckStatus(target, BattleStatus.Immobilized))
            {
                if (btl_util.getEnemyPtr(target).info.die_atk != 0 && target.cur.hp == 0)
                    PersistenSingleton<EventEngine>.Instance.RequestAction(BattleCommandId.EnemyDying, targetId, caster.btl_id, commandAndScript);
                else if (target.cur.hp != 0 && v.Command.Data.cmd_no < BattleCommandId.BoundaryCheck)
                    PersistenSingleton<EventEngine>.Instance.RequestAction(BattleCommandId.EnemyCounter, targetId, caster.btl_id, commandAndScript);
            }
            PersistenSingleton<EventEngine>.Instance.RequestAction(BattleCommandId.EnemyReaction, targetId, caster.btl_id, commandAndScript);
        }

        public static BattleScriptFactory FindScriptFactory(Int32 scriptId)
        {
            if (scriptId >= 0 && scriptId < BaseScripts.Length)
                return BaseScripts[scriptId];

            BattleScriptFactory result;
            return ExtendedScripts.TryGetValue(scriptId, out result) ? result : null;
        }

        private static Boolean CheckDamageMotion(BattleCalculator v)
        {
            return ((v.Context.Flags & BattleCalcFlags.AddStat) == 0 || (FF9StateSystem.Battle.FF9Battle.add_status[v.Caster.Data.weapon.StatusIndex].Value & BattleStatus.NoReaction) == 0)
                && (v.Command.AbilityCategory & 64) == 0
                && v.Command.Data.info.cover == 0
                && !Status.checkCurStat(v.Target.Data, BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Death | BattleStatus.Stop | BattleStatus.Defend | BattleStatus.Freeze | BattleStatus.Jump)
                && v.Caster.Data != v.Target.Data;
        }

        private static void CheckDamageReaction(BattleCalculator v)
        {
            if (v.Target.Data.bi.player == 0 || v.Caster.Data.bi.player != 0)
                return;
            if (v.Target.Data.bi.t_gauge == 0 || v.Target.Data.cur.hp <= 0 || btl_stat.CheckStatus(v.Target.Data, BattleStatus.CannotTrance))
                return;


            if (v.Target.Trance + v.Context.TranceIncrease < 0)
            {
                v.Target.Trance = 0;
                if (v.Target.InTrance)
                    v.Target.RemoveStatus(BattleStatus.Trance);
            }
            else if (v.Target.Trance + v.Context.TranceIncrease < Byte.MaxValue)
            {
                v.Target.Trance += (Byte)v.Context.TranceIncrease;
            }
            else
            {
                v.Target.Trance = Byte.MaxValue;

                if (FF9StateSystem.Battle.isDebug)
                    return;

                if (Configuration.Battle.NoAutoTrance)
                    return;

                v.Target.AlterStatus(BattleStatus.Trance);
            }
        }
    }
}