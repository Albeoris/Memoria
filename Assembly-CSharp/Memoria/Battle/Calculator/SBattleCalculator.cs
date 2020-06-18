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
            BattleCalculator v = new BattleCalculator(caster, target, command);
            BattleScriptFactory factory = FindScriptFactory(scriptId);
            btl_cmd.cmd_effect_counter++;
            if (btl_cmd.IsAttackShortRange(v.Caster, command.Data))
            {
                v.BonusBackstabAndPenaltyLongDistance();
                if ((command.Data.aa.Category & 8) != 0 && v.Target.IsUnderAnyStatus(BattleStatus.Vanish)) // Is Physical
                    v.Context.Flags |= BattleCalcFlags.Miss;
            }
            if (Configuration.Battle.CustomBattleFlagsMeaning == 1 && (command.Data.aa.Type & 0x10) != 0 && caster.bi.player != 0) // Use weapon properties
            {
                v.BonusKillerAbilities();
                v.Caster.BonusWeaponElement();
                if (v.CanAttackWeaponElementalCommand())
                    v.TryAddWeaponStatus();
            }
            if ((v.Context.Flags & (BattleCalcFlags.Miss | BattleCalcFlags.Guard)) != 0)
            {
                if (caster != null && target != null)
                    SBattleCalculator.CalcResult(v);
                return;
            }
            if ((command.Data.aa.Category & 8) != 0) // Is Physical
                if (!v.Target.TryKillFrozen())
                    SBattleCalculator.CalcResult(v);
            if (factory != null)
            {
                IBattleScript script = factory(v);
                script.Perform();
            }
            else
            {
                Log.Warning($"Unknown script id: {scriptId}");
            }

            if (caster != null && target != null)
                CalcResult(v);
        }

        public static void CalcResult(BattleCalculator v)
        {
            BTL_DATA target = v.Target.Data;
            BTL_DATA caster = v.Caster.Data;
            Boolean counterAtk = false;
            if (target.bi.player != 0 && caster.bi.player == 0)
                counterAtk = btl_abil.CheckCounterAbility(v.Target, v.Caster, v.Command);
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
                if (Configuration.Battle.CustomBattleFlagsMeaning == 1 && (v.Command.Data.aa.Type & 0x20) != 0) // Has critical
                    v.TryCriticalHit();
                // Note: weapon statuses are added before damage (unlike vanilla), like spell statuses
                if ((v.Context.Flags & BattleCalcFlags.AddStat) != 0 && target.cur.hp > 0)
                {
                    if ((FF9StateSystem.Battle.FF9Battle.add_status[(int)caster.weapon.StatusIndex].Value & BattleStatus.Death) == 0 || !Status.checkCurStat(target, BattleStatus.EasyKill))
                    {
                        v.Target.TryAlterStatuses(FF9StateSystem.Battle.FF9Battle.add_status[(int)caster.weapon.StatusIndex].Value, false);
                    }
                }
                if ((v.Command.Data.aa.Category & 8) != 0) // Is Physical
                {
//                  v.Target.RaiseTrouble();
                    if ((v.Context.added_status & BattleStatus.Confuse) == 0)
                        v.Target.RemoveStatus(BattleStatus.Confuse);
                    if ((v.Context.added_status & BattleStatus.Sleep) == 0)
                        v.Target.RemoveStatus(BattleStatus.Sleep);
                }

                if ((v.Command.Data.aa.Category & 16) != 0 && (v.Context.added_status & BattleStatus.Vanish) == 0) // Is Magical
                    v.Target.RemoveStatus(BattleStatus.Vanish);

                if (v.Target.Flags != 0)
                {
                    target.fig_info |= (UInt16)v.Target.Flags;
                    if ((v.Target.Flags & CalcFlag.HpAlteration) != 0)
                    {
                        if (v.Command.Data.info.reflec == 1)
                        {
                            UInt16 num1 = 0;
                            for (UInt16 index = 0; index < 4; ++index)
                            {
                                if ((caster.reflec.tar_id[index] & target.btl_id) != 0)
                                    ++num1;
                            }
                            v.Target.HpDamage *= num1;
                            if (v.Caster.HasSupportAbility(SupportAbility1.Reflectx2))
                                v.Target.HpDamage *= 2;
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
                            CheckDamageReaction(v, counterAtk);
                        }
                    }

                    if ((v.Target.Flags & CalcFlag.MpAlteration) != 0)
                    {
                        if (v.Target.MpDamage > 999)
                            v.Target.MpDamage = 999;
                        if ((v.Target.Flags & CalcFlag.MpRecovery) != 0)
                            btl_para.SetMpRecover(target, (UInt32)v.Target.MpDamage);
                        else
                            btl_para.SetMpDamage(target, (UInt32)v.Target.MpDamage);
                    }
                }
                else if ((v.Context.Flags & BattleCalcFlags.DirectHP) != 0)
                {
                    if (CheckDamageMotion(v))
                        btl_mot.SetDamageMotion(v.Target);
                    CheckDamageReaction(v, counterAtk);
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
//              if ((v.Context.Flags & BattleCalcFlags.AddStat) != 0 && target.cur.hp > 0 && ((FF9StateSystem.Battle.FF9Battle.add_status[caster.weapon.StatusIndex].Value & BattleStatus.Death) == 0 || !v.Target.IsUnderStatus(BattleStatus.EasyKill)))
//              {
//                  v.Target.TryAlterStatuses((BattleStatus)FF9StateSystem.Battle.FF9Battle.add_status[caster.weapon.StatusIndex].Value, false);
//              }
                if (target.bi.player != 0 && FF9StateSystem.Settings.IsHpMpFull && target.cur.hp != 0)
                {
                    target.cur.hp = target.max.hp;
                    target.cur.mp = target.max.mp;
                }
            }
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
            return ((v.Context.Flags & BattleCalcFlags.AddStat) == 0 || (FF9StateSystem.Battle.FF9Battle.add_status[v.Caster.Data.weapon.StatusIndex].Value & BattleStatus.NoReaction) == 0) && ((v.Command.Data.aa.Category & 64) == 0 && v.Command.Data.info.cover == 0) && (!Status.checkCurStat(v.Target.Data, BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Death | BattleStatus.Stop | BattleStatus.Defend | BattleStatus.Freeze | BattleStatus.Jump) && v.Caster.Data != v.Target.Data);
        }

        private static void CheckDamageReaction(BattleCalculator v, Boolean counterAtk)
        {
            if (v.Target.Data.bi.player == 0 || v.Caster.Data.bi.player != 0)
                return;
            if (!counterAtk)
                btl_abil.CheckAutoItemAbility(v.Target, v.Command);
            btl_abil.CheckReactionAbility(v.Target.Data, v.Command.Data.aa);
            if (v.Target.Data.bi.t_gauge == 0 || v.Target.Data.cur.hp <= 0 || btl_stat.CheckStatus(v.Target.Data, BattleStatus.CannotTrance))
                return;


            Byte num1 = v.Target.HasSupportAbility(SupportAbility2.HighTide)
                ? v.Target.Data.elem.wpr
                : (Byte)((UInt32)Comn.random16() % v.Target.Data.elem.wpr);

            if (v.Target.Data.trance + num1 < Byte.MaxValue)
            {
                v.Target.Data.trance += num1;
            }
            else
            {
                v.Target.Data.trance = Byte.MaxValue;

                if (FF9StateSystem.Battle.isDebug)
                    return;

                if (Configuration.Battle.NoAutoTrance)
                    return;

                v.Target.AlterStatus(BattleStatus.Trance);
            }
        }
    }
}