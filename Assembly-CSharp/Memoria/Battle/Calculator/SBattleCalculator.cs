using System;
using System.Reflection;
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
        public static void CalcMain(BattleUnit caster, BattleUnit target, BattleCommand command)
        {
            CalcMain(caster.Data, target.Data, command, command.ScriptId);
        }

        public static void CalcMain(BTL_DATA caster, BTL_DATA target, CMD_DATA cmd, BattleActionThread sfxThread = null)
        {
            CalcMain(caster, target, new BattleCommand(cmd), cmd.ScriptId, sfxThread);
        }

        public static void CalcMain(BTL_DATA caster, BTL_DATA target, BattleCommand command, Int32 scriptId, BattleActionThread sfxThread = null)
        {
            try
            {
                if (caster == null || target == null)
                {
                    if (scriptId == btl_sys.fleeScriptId)
                    {
                        BattleScriptFactory fleeFactory = FindScriptFactory(scriptId);
                        if (fleeFactory != null)
                        {
                            fleeFactory(new BattleCalculator()).Perform();
                            return;
                        }
                    }
                    Log.Warning($"[{nameof(CalcMain)}] Unknown script id with no caster/target: {scriptId}");
                    return;
                }
                command.ScriptId = scriptId;
                BattleCalculator v = new BattleCalculator(caster, target, command);
                v.Context.sfxThread = sfxThread;
                BattleScriptFactory factory = FindScriptFactory(scriptId);
                foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(v.Caster))
                    saFeature.TriggerOnAbility(v, "BattleScriptStart", false);
                foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(v.Target))
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
                    if (scriptId != 64) // Script 64 is extensively used for enemy moves that have no effect or only an event scripted effect
                        Log.Warning($"[{nameof(CalcMain)}] Unknown script id: {scriptId}");
                }

                if (v.PerformCalcResult)
                    CalcResult(v);
            }
            catch (Exception err)
            {
                if (err is MissingMemberException)
                {
                    // Most likely, Memoria.Scripts.dll was compiled with another version of Assembly-CSharp.dll that is incompatible
                    MissingMemberException memberErr = err as MissingMemberException;
                    Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    String faultyMod = ScriptsLoader.GetScriptDLL(scriptId).Split('/')[0];
                    btl2d.Btl2dReqSymbol(target, 0x10000u, 0, 0);
                    Log.Error($"[Memoria.Scripts.dll] Error when using the spell {command.AbilityName}\n" +
                        $"The spell script {scriptId} loaded from '{ScriptsLoader.GetScriptDLL(scriptId)}' is incompatible with the current version of Memoria\n" +
                        $"{memberErr.Message}\n" +
                        (String.Compare(faultyMod, "StreamingAssets") == 0 ?
                            $"Try to update Memoria (https://github.com/Albeoris/Memoria/releases)\n" :
                            $"Try to update both Memoria (https://github.com/Albeoris/Memoria/releases) and the mod {faultyMod} (from the Mod Manager)\n") +
                        $"Your current version of Memoria dates from {new DateTime(2000, 1, 1).AddDays(assemblyVersion.Build).AddSeconds(assemblyVersion.Revision * 2):Y}\n" +
                        $" ");
                }
                else
                {
                    Log.Error(err);
                }
            }
        }

        public static void CalcResult(BattleCalculator v)
        {
            BTL_DATA target = v.Target.Data;
            BTL_DATA caster = v.Caster.Data;
            CMD_DATA cmd = v.Command.Data;
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(caster))
                saFeature.TriggerOnAbility(v, "BattleScriptEnd", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(target))
                saFeature.TriggerOnAbility(v, "BattleScriptEnd", true);
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
                        //btl_mot.setMotion(target, BattlePlayerCharacter.PlayerMotionIndex.MP_AVOID);
                        //target.evt.animFrame = 0;
                        Int32 num = btl_mot.GetDirection(target);
                        target.rot.eulerAngles = new Vector3(target.rot.eulerAngles.x, num, target.rot.eulerAngles.z);
                    }
                    else if (target.bi.slave == 0)
                    {
                        if (!btl_util.IsBtlBusy(target, btl_util.BusyMode.CASTER))
                        {
                            if (Configuration.Battle.FloatEvadeBonus > 0 && v.Target.IsUnderPermanentStatus(BattleStatus.Float))
                                target.pos[1] -= -600f;
                            target.pos[2] -= -400f;
                        }
                    }
                    else
                        btl_util.GetMasterEnemyBtlPtr().Data.pos[2] -= -400f;
                    target.bi.dodge = 1;
                    btl_mot.SetDefaultIdle(target);
                    cmd.info.dodge = 1;
                }
            }
            else
            {
                if (Configuration.Battle.CustomBattleFlagsMeaning == 1 && (v.Command.AbilityType & 0x20) != 0) // Has critical
                    v.TryCriticalHit();
                // Note: weapon statuses are added before damage (unlike vanilla), like spell statuses
                if ((v.Context.Flags & BattleCalcFlags.AddStat) != 0 && target.cur.hp > 0)
                    if ((FF9StateSystem.Battle.FF9Battle.add_status[caster.weapon.StatusIndex].Value & BattleStatus.Death) == 0 || !btl_stat.CheckStatus(target, BattleStatus.EasyKill))
                        v.Target.TryAlterStatuses(FF9StateSystem.Battle.FF9Battle.add_status[caster.weapon.StatusIndex].Value, false, v.Caster);
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
                    if (Configuration.Mod.TranceSeek && v.Context.DamageModifierCount > 0) // TRANCE SEEK -> damage is multiplied by 1.25, 1.5, 1.75, 2, 2.25, 2.5...
                    {
                        modifier_factor = 1 + (float)(v.Context.DamageModifierCount * 0.25); 
                    }
                    else
                    {
                        if (v.Caster.IsUnderAnyStatus(BattleStatus.Trance) && v.Caster.PlayerIndex == CharacterId.Steiner)
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
                        if (cmd.info.reflec == 1)
                        {
                            UInt16 reflectMultiplier = 0;
                            for (UInt16 index = 0; index < 4; ++index)
                                if ((cmd.reflec.tar_id[index] & target.btl_id) != 0)
                                    ++reflectMultiplier;
                            v.Target.HpDamage *= reflectMultiplier;
                        }
                        if (!Configuration.Battle.BreakDamageLimit && v.Target.HpDamage > v.Caster.MaxDamageLimit)
                            v.Target.HpDamage = (Int32)v.Caster.MaxDamageLimit;
                        if ((v.Target.Flags & CalcFlag.HpRecovery) != 0)
                        {
                            v.Target.HpDamage = btl_para.SetRecover(v.Target, (UInt32)v.Target.HpDamage);
                        }
                        else
                        {
                            if (FF9StateSystem.Settings.IsDmg9999 && caster.bi.player != 0 && cmd.cmd_no != BattleCommandId.StageMagicZidane && cmd.cmd_no != BattleCommandId.StageMagicBlank && cmd.cmd_no != BattleCommandId.StageMagicMarcus && cmd.cmd_no != BattleCommandId.StageMagicCinna)
                                v.Target.HpDamage = 9999;
                            v.Target.HpDamage = btl_para.SetDamage(v.Target, v.Target.HpDamage, CheckDamageMotion(v) ? (Byte)1 : (Byte)0, cmd);
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
                        if (!Configuration.Battle.BreakDamageLimit && v.Target.MpDamage > v.Caster.MaxMpDamageLimit)
                            v.Target.MpDamage = (Int32)v.Caster.MaxMpDamageLimit;
                        if ((v.Target.Flags & CalcFlag.MpRecovery) != 0)
                            v.Target.MpDamage = btl_para.SetMpRecover(v.Target, (UInt32)v.Target.MpDamage);
                        else
                            v.Target.MpDamage = btl_para.SetMpDamage(v.Target, (UInt32)v.Target.MpDamage);
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
                        btl_mot.SetDamageMotion(v.Target, cmd);
                    CheckDamageReaction(v);
                }
                if (v.Caster.Flags != 0)
                {
                    caster.fig_info |= (UInt16)v.Caster.Flags;
                    if ((v.Caster.Flags & CalcFlag.HpAlteration) != 0)
                    {
                        if (!Configuration.Battle.BreakDamageLimit && v.Caster.HpDamage > v.Caster.MaxDamageLimit)
                            v.Caster.HpDamage = (Int32)v.Caster.MaxDamageLimit;
                        if ((v.Caster.Flags & CalcFlag.HpRecovery) != 0)
                            v.Caster.HpDamage = btl_para.SetRecover(v.Caster, (UInt32)v.Caster.HpDamage);
                        else
                            v.Caster.HpDamage = btl_para.SetDamage(v.Caster, v.Caster.HpDamage, 0, cmd);
                    }
                    if ((v.Caster.Flags & CalcFlag.MpAlteration) != 0)
                    {
                        if (!Configuration.Battle.BreakDamageLimit && v.Caster.MpDamage > v.Caster.MaxMpDamageLimit)
                            v.Caster.MpDamage = (Int32)v.Caster.MaxMpDamageLimit;
                        if ((v.Caster.Flags & CalcFlag.MpRecovery) != 0)
                            v.Caster.MpDamage = btl_para.SetMpRecover(v.Caster, (UInt32)v.Caster.MpDamage);
                        else
                            v.Caster.MpDamage = btl_para.SetMpDamage(v.Caster, (UInt32)v.Caster.MpDamage);
                    }
                }
                if (target.bi.player != 0 && FF9StateSystem.Settings.IsHpMpFull && target.cur.hp != 0)
                {
                    target.cur.hp = target.max.hp;
                    target.cur.mp = target.max.mp;
                }
            }
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(caster))
                saFeature.TriggerOnAbility(v, "EffectDone", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(target))
                saFeature.TriggerOnAbility(v, "EffectDone", true);
            BattleVoice.TriggerOnBattleAct(caster, "HitEffect", cmd, v);
            BattleVoice.TriggerOnHitted(target, v);
            BattleCalculator.FrameAppliedEffectList.Add(v);
            if (target.bi.player != 0 || FF9StateSystem.Battle.isDebug)
                return;
            UInt16 targetId = target.bi.slave == 0 ? target.btl_id : (UInt16)16;
            if (caster.bi.player != 0 && !btl_stat.CheckStatus(target, BattleStatusConst.Immobilized))
            {
                if (btl_util.getEnemyPtr(target).info.die_atk != 0 && target.cur.hp == 0)
                    PersistenSingleton<EventEngine>.Instance.RequestAction(BattleCommandId.EnemyDying, targetId, caster.btl_id, (Int32)cmd.cmd_no, cmd.sub_no, cmd);
                else if (target.cur.hp != 0 && btl_util.IsCommandDeclarable(cmd.cmd_no))
                    PersistenSingleton<EventEngine>.Instance.RequestAction(BattleCommandId.EnemyCounter, targetId, caster.btl_id, (Int32)cmd.cmd_no, cmd.sub_no, cmd);
            }
            PersistenSingleton<EventEngine>.Instance.RequestAction(BattleCommandId.EnemyReaction, targetId, caster.btl_id, (Int32)cmd.cmd_no, cmd.sub_no, cmd);
        }

        public static BattleScriptFactory FindScriptFactory(Int32 scriptId)
        {
            return ScriptsLoader.GetBattleScript(scriptId);
        }

        private static Boolean CheckDamageMotion(BattleCalculator v)
        {
            return ((v.Context.Flags & BattleCalcFlags.AddStat) == 0 || (FF9StateSystem.Battle.FF9Battle.add_status[v.Caster.Data.weapon.StatusIndex].Value & BattleStatusConst.NoReaction) == 0)
                && (v.Command.AbilityCategory & 64) == 0
                && v.Command.Data.info.cover == 0
                && !btl_stat.CheckStatus(v.Target.Data, BattleStatusConst.NoDamageMotion)
                && v.Caster.Data != v.Target.Data;
        }

        private static void CheckDamageReaction(BattleCalculator v)
        {
            if (!UIManager.Battle.FF9BMenu_IsEnable())
                return;
            if (v.Target.IsPlayer == v.Caster.IsPlayer)
                return;
            if (!v.Target.HasTrance || v.Target.Data.cur.hp <= 0 || btl_stat.CheckStatus(v.Target.Data, BattleStatusConst.CannotTrance))
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