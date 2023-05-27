using System;
using System.Collections.Generic;
using UnityEngine;
using FF9;
using Memoria;
using Memoria.Data;
using NCalc;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable EmptyConstructor
// ReSharper disable UnusedMember.Global
// ReSharper disable MoreSpecificForeachVariableTypeAvailable
// ReSharper disable RedundantExplicitArraySize
// ReSharper disable InconsistentNaming

public static class btl_stat
{
    public static void SaveStatus(PLAYER p, BTL_DATA btl)
    {
        p.status = btl.stat.cur & BattleStatusConst.OutOfBattle;
        p.status |= p.permanent_status;
    }

    public static void InitCountDownStatus(BTL_DATA btl)
    {
        // cdown_max is now updated in AlterStatus; its initialization there is not important
        btl.stat.cnt.cdown_max = (Int16)((60 - btl.elem.wpr << 3) * FF9StateSystem.Battle.FF9Battle.status_data[27].conti_cnt);
    }

    public static void StatusCommandCancel(BTL_DATA btl, BattleStatus status)
    {
        if (btl.bi.player != 0)
            UIManager.Battle.RemovePlayerFromAction(btl.btl_id, true);
        if (!btl_cmd.KillCommand2(btl))
            return;
        btl.bi.atb = 0;
        if (btl.bi.player != 0 && !FF9StateSystem.Settings.IsATBFull)
            btl.cur.at = 0;
        btl.sel_mode = 0;
    }

    public static UInt32 AlterStatus(BTL_DATA btl, BattleStatus status, BTL_DATA inflicter = null)
    {
        BattleUnit unit = new BattleUnit(btl);
        Dictionary<Int32, STAT_DATA> statusData = FF9StateSystem.Battle.FF9Battle.status_data;
        STAT_INFO stat = btl.stat;
        Int32 statusIndex = 0;
        if ((status & stat.invalid) != 0)
            return 0;
        if ((status & stat.permanent) != 0 || ((status & stat.cur) != 0 && (status & BattleStatusConst.NoReset) != 0))
            return 1;
        BattleStatus invalidStatuses = btl.bi.t_gauge == 0 ? BattleStatus.Trance : 0;
        for (Int32 i = 0; i < 32; ++i)
        {
            BattleStatus bsi = (BattleStatus)(1U << i);
            if ((status & bsi) != 0)
                statusIndex = i;
            if ((stat.cur & bsi) != 0)
                invalidStatuses |= statusData[i].invalid;
        }
        if (btl_cmd.CheckSpecificCommand(btl, BattleCommandId.SysStone))
            invalidStatuses |= statusData[0].invalid;
        if ((status & invalidStatuses) != 0)
            return 1;
        if ((status & BattleStatusConst.AlterNoSet) == 0)
        {
            if ((status & BattleStatusConst.CmdCancel) != 0)
                StatusCommandCancel(btl, status);
            stat.cur |= status;
        }
        switch (status)
        {
            case BattleStatus.Petrify:
                if (!btl_cmd.CheckUsingCommand(btl.cmd[2]))
                {
                    if (FF9StateSystem.Battle.FF9Battle.btl_phase > 2 && Configuration.Battle.Speed < 3)
                    {
                        btl_cmd.SetCommand(btl.cmd[2], BattleCommandId.SysStone, 0, btl.btl_id, 0U);
                        break;
                    }
                    stat.cur |= status;
                    btl.bi.atb = 0;
                    SetStatusClut(btl, true);
                    if (FF9StateSystem.Battle.FF9Battle.btl_phase > 2 && (status & BattleStatusConst.CmdCancel) != 0)
                        StatusCommandCancel(btl, status);
                }
                break;
            case BattleStatus.Venom:
                break;
            case BattleStatus.Zombie:
                if (unit.IsPlayer && !unit.IsUnderAnyStatus(BattleStatus.Trance))
                    unit.Trance = 0;
                SetStatusPolyColor(btl);
                break;
            case BattleStatus.Death:
                if (unit.CurrentHp > 0)
                {
                    btl.fig_info |= Param.FIG_INFO_DEATH;
                    new BattleUnit(btl).Kill(inflicter);
                }
                else
                {
                    unit.CurrentHp = 0;
                }
                unit.CurrentAtb = 0;
                if (!btl_cmd.CheckUsingCommand(btl.cmd[2]))
                {
                    //btl_cmd.SetCommand(btl.cmd[2], BattleCommandId.SysDead, 0U, btl.btl_id, 0U);
                    if (!unit.IsPlayer)
                    {
                        if (btl.die_seq == 0)
                        {
                            if (btl.bi.slave != 0)
                                btl.die_seq = 5;
                            else if (btl_util.getEnemyPtr(btl).info.die_atk == 0 || !btl_util.IsBtlBusy(btl, btl_util.BusyMode.CASTER | btl_util.BusyMode.QUEUED_CASTER))
                                btl.die_seq = 1;
                        }
                        btl_sys.CheckForecastMenuOff(btl);
                    }
                }
                //btl_cmd.KillSpecificCommand(btl, BattleCommandId.SysTrans);
                break;
            case BattleStatus.Berserk:
            case BattleStatus.Heat:
            case BattleStatus.Freeze:
                SetStatusPolyColor(btl);
                break;
            case BattleStatus.Stop:
                break;
            case BattleStatus.Trance:
                btl.special_status_old = false;
                if (btl.bi.player == 0)
                    unit.Trance = byte.MaxValue; // For OverTrance purpose
                btl_cmd.SetCommand(btl.cmd[4], BattleCommandId.SysTrans, 0, btl.btl_id, 0U);
                break;
            case BattleStatus.Sleep:
                if (unit.IsPlayer)
                {
                    btl.bi.def_idle = 1;
                    btl_mot.SetDefaultIdle(btl);
                }
                //if (unit.IsPlayer && !btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING) && !btl_util.IsBtlUsingCommand(btl))
                //{
                //    btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING);
                //    btl.evt.animFrame = 0;
                //}
                break;
            case BattleStatus.Haste:
                if (CheckStatus(btl, BattleStatus.Slow))
                {
                    RemoveStatus(btl, BattleStatus.Slow);
                    return 2;
                }
                btl_para.InitATB(btl);
                btl.cur.at_coef = (SByte)(btl.cur.at_coef * 3 / 2);
                stat.cur |= status;
                break;
            case BattleStatus.Slow:
                if (CheckStatus(btl, BattleStatus.Haste))
                {
                    RemoveStatus(btl, BattleStatus.Haste);
                    return 2;
                }
                btl_para.InitATB(btl);
                btl.cur.at_coef = (SByte)(btl.cur.at_coef * 2 / 3);
                stat.cur |= status;
                break;
            case BattleStatus.Vanish:
                btl_mot.HideMesh(btl, btl.mesh_banish, true);
                break;
            case BattleStatus.Mini:
                if ((stat.permanent & BattleStatus.Mini) != 0)
                    return 1;
                if ((stat.cur & BattleStatus.Mini) != 0)
                {
                    btl_stat.RemoveStatus(btl, BattleStatus.Mini);
                    return 2;
                }
                stat.cur ^= status;
                geo.geoScaleUpdate(btl, true);
                break;
        }
        if (btl.bi.player != 0 && (status & BattleStatusConst.Immobilized & BattleStatusConst.IdleDying) != 0 && !btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING))
        {
            btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING);
            btl.evt.animFrame = 0;
        }
        if (FF9StateSystem.Battle.FF9Battle.btl_phase > 2 && (status & BattleStatusConst.BattleEnd) != 0)
            btl_sys.CheckBattlePhase(btl);
        RemoveStatuses(btl, statusData[statusIndex].clear);
        if (CheckStatus(btl, BattleStatusConst.StopAtb))
            btl.bi.atb = 0;
        if ((status & BattleStatusConst.ContiCount) != 0)
        {
            Int16 defaultFactor = (status & BattleStatusConst.ContiBad) != 0 ? (Int16)(60 - btl.elem.wpr << 3) :
                                  (status & BattleStatusConst.ContiGood) != 0 ? (Int16)(btl.elem.wpr << 3) : (Int16)(60 - btl.elem.wpr << 2);
            btl.stat.cnt.conti[statusIndex] = (Int16)(statusData[statusIndex].conti_cnt * defaultFactor);
            if (Configuration.Battle.StatusDurationFormula.Length > 0)
            {
                Expression e = new Expression(Configuration.Battle.StatusDurationFormula);
                e.Parameters["StatusIndex"] = (Int32)statusIndex;
                e.Parameters["IsPositiveStatus"] = (status & BattleStatusConst.ContiGood) != 0;
                e.Parameters["IsNegativeStatus"] = (status & BattleStatusConst.ContiBad) != 0;
                e.Parameters["ContiCnt"] = (Int32)statusData[statusIndex].conti_cnt;
                e.Parameters["OprCnt"] = (Int32)statusData[statusIndex].opr_cnt;
                e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                NCalcUtility.InitializeExpressionUnit(ref e, new BattleUnit(btl), "Target");
                NCalcUtility.InitializeExpressionNullableUnit(ref e, inflicter != null ? new BattleUnit(inflicter) : null, "Inflicter");
                Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                if (val >= 0)
                    btl.stat.cnt.conti[statusIndex] = (Int16)Math.Min(val, Int16.MaxValue);
            }
            btl.stat.cnt.conti[statusIndex] = (Int16)(btl.stat_duration_factor[status] * btl.stat.cnt.conti[statusIndex]);
            if ((status & (BattleStatus.Doom | BattleStatus.GradualPetrify)) != 0u)
            {
                if (Configuration.Mod.TranceSeek && unit.HasSupportAbility(SupportAbility1.AutoRegen))
                {
                    btl.stat.cnt.conti[statusIndex] = (Int16)(statusData[statusIndex].conti_cnt * defaultFactor * 2);
                    btl.stat.cnt.cdown_max = (Int16)Math.Max(1, btl.stat.cnt.conti[statusIndex] / 2);
                }
                else
                {
                    btl.stat.cnt.cdown_max = Math.Max((Int16)1, btl.stat.cnt.conti[statusIndex]);
                }
            }
        }
        if ((status & BattleStatusConst.OprCount) != 0)
            SetOprStatusCount(btl, statusIndex);
        HonoluluBattleMain.battleSPS.AddBtlSPSObj(unit, status);
        if (btl.bi.player != 0)
            BattleAchievement.UpdateAbnormalStatus(status);
        BattleVoice.TriggerOnStatusChange(btl, "Added", status);
        return 2;
    }

    public static UInt32 AlterStatuses(BTL_DATA btl, BattleStatus statuses, BTL_DATA inflicter = null, Boolean usePartialResist = false)
    {
        UInt32 bestResult = 0;
        for (Int32 index = 0; index < 32U; ++index)
        {
            BattleStatus status = (BattleStatus)(1U << index);
            if ((statuses & status) != 0)
            {
                Single rate = btl.stat_partial_resist[status];
                if (usePartialResist && rate > 0f && Comn.random16() < rate * 65536f)
                    bestResult = Math.Max(bestResult, 1);
                else
                    bestResult = Math.Max(bestResult, AlterStatus(btl, status, inflicter));
            }
        }
        return bestResult;
    }

    public static UInt32 RemoveStatus(BTL_DATA btl, BattleStatus status)
    {
        STAT_INFO stat = btl.stat;
        if ((stat.permanent & status) != 0 || (stat.cur & status) == 0 || btl.bi.player == 0 && FF9StateSystem.Battle.FF9Battle.btl_phase == 5 && (status & BattleStatusConst.BattleEnd) != 0)
            return 1;
        stat.cur &= ~status;
        switch (status)
        {
            case BattleStatus.Petrify:
                btl_cmd.KillSpecificCommand(btl, BattleCommandId.SysStone);
                SetStatusClut(btl, false);
                break;
            case BattleStatus.Zombie:
            case BattleStatus.Heat:
            case BattleStatus.Freeze:
                if (CheckStatus(btl, BattleStatusConst.ChgPolyCol))
                    SetStatusPolyColor(btl);
                break;
            case BattleStatus.Death:
                btl.die_seq = 0;
                //btl.bi.dmg_mot_f = 0;
                btl.bi.cmd_idle = 0;
                btl.bi.death_f = 0;
                btl.bi.stop_anim = 0;
                btl.escape_key = 0;
                btl.killer_track = null;
                if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE) || btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE))
                {
                    GeoTexAnim.geoTexAnimPlay(btl.texanimptr, 2);
                    if (btl.bi.player != 0)
                        GeoTexAnim.geoTexAnimPlay(btl.tranceTexanimptr, 2);
                    //btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DISABLE);
                    //btl.evt.animFrame = 0;
                }
                if (!btl_util.IsBtlUsingCommand(btl, out CMD_DATA cmd) || !btl_util.IsCommandDeclarable(cmd.cmd_no))
                    btl.sel_mode = 0;
                btl_cmd.KillSpecificCommand(btl, BattleCommandId.SysDead);
                if ((btl.stat.permanent & BattleStatus.Regen) != 0)
                    SetOprStatusCount(btl, 18);
                break;
            case BattleStatus.Confuse:
                Vector3 eulerAngles = btl.rot.eulerAngles;
                eulerAngles.y = btl.evt.rotBattle.eulerAngles.y;
                btl.rot = Quaternion.Euler(eulerAngles);
                StatusCommandCancel(btl, status);
                break;
            case BattleStatus.Berserk:
                StatusCommandCancel(btl, status);
                if (CheckStatus(btl, BattleStatusConst.ChgPolyCol))
                    SetStatusPolyColor(btl);
                break;
            case BattleStatus.Trance:
                if (btl.bi.player == 0)
                {
                    btl.trance = 0;
                    btl_cmd.SetCommand(btl.cmd[4], BattleCommandId.SysTrans, 0, btl.btl_id, 0U);
                }
                else
                {
                    if (!Configuration.Mod.TranceSeek || btl.gameObject == btl.tranceGo) // TRANCE SEEK - other usage of trance
                    {
                        btl.trance = 0;
                        if (Status.checkCurStat(btl, BattleStatus.Jump))
                        {
                            RemoveStatus(btl, BattleStatus.Jump);
                            btl.SetDisappear(false, 2);
                            btl_mot.setBasePos(btl);
                            btl_mot.setMotion(btl, btl.bi.def_idle);
                            btl.evt.animFrame = 0;
                        }
                        btl_cmd.SetCommand(btl.cmd[4], BattleCommandId.SysTrans, 0, btl.btl_id, 0U);
                    }
                }
                break;
            case BattleStatus.Haste:
            case BattleStatus.Slow:
                btl_para.InitATB(btl);
                break;
            case BattleStatus.Float:
                Single value = 0f;
                btl.pos[1] = value;
                btl.base_pos[1] = value;
                break;
            case BattleStatus.Vanish:
                btl_mot.ShowMesh(btl, btl.mesh_banish, true);
                break;
            case BattleStatus.Doom:
                if (btl.deathMessage != null)
                {
                    Singleton<HUDMessage>.Instance.ReleaseObject(btl.deathMessage);
                    btl.deathMessage = null;
                }
                break;
            case BattleStatus.Mini:
                geo.geoScaleUpdate(btl, true);
                break;
            case BattleStatus.Jump:
                btl.tar_mode = 3;
                btl.bi.atb = 1;
                if (btl.bi.player != 0 && !FF9StateSystem.Settings.IsATBFull)
                    btl.cur.at = 0;
                btl.sel_mode = 0;
                btl.cmd[3].cmd_no = BattleCommandId.None;
                btl.cmd[3].tar_id = 0;
                break;
            case BattleStatus.GradualPetrify:
                if (btl.petrifyMessage != null)
                {
                    Singleton<HUDMessage>.Instance.ReleaseObject(btl.petrifyMessage);
                    btl.petrifyMessage = null;
                }
                btl_cmd.KillSpecificCommand(btl, BattleCommandId.SysStone);
                break;
        }
        HonoluluBattleMain.battleSPS.RemoveBtlSPSObj(btl, status);
        BattleVoice.TriggerOnStatusChange(btl, "Removed", status);
        return 2;
    }

    public static UInt32 RemoveStatuses(BTL_DATA btl, BattleStatus statuses)
    {
        UInt32 num = 1;
        for (Int32 index = 0; index < 32U; ++index)
        {
            BattleStatus status = (BattleStatus)(1U << index);
            if (((Int32)statuses & (Int32)status) != 0 && (Int32)RemoveStatus(btl, status) == 2)
                num = 2U;
        }
        return num;
    }

    public static void MakeStatusesPermanent(BTL_DATA btl, BattleStatus statuses, Boolean flag = true, BTL_DATA inflicter = null)
    {
        if (flag)
        {
            if ((statuses & BattleStatus.Haste) != 0)
                RemoveStatus(btl, BattleStatus.Slow);
            if ((statuses & BattleStatus.Slow) != 0)
                RemoveStatus(btl, BattleStatus.Haste);
            AlterStatuses(btl, statuses, inflicter);
            btl.stat.permanent |= statuses;
            // Permanent statuses should also be registered as current statuses
            //btl.stat.cur &= ~(statuses & btl.stat.cur);
        }
        else
        {
            btl.stat.permanent &= ~statuses;
            btl_stat.RemoveStatuses(btl, statuses);
        }
    }

    public static void SetOprStatusCount(BTL_DATA btl, Int32 statTblNo)
    {
        UInt16 oprIndex;
        UInt16 defaultFactor;
        if (statTblNo == 1)
        {
            oprIndex = 0;
            defaultFactor = (UInt16)((UInt32)btl.elem.wpr << 2);
        }
        else if (statTblNo == 16)
        {
            oprIndex = 1;
            defaultFactor = (UInt16)((UInt32)btl.elem.wpr << 2);
        }
        else
        {
            oprIndex = 2;
            defaultFactor = (UInt16)(60 - btl.elem.wpr << 2);
        }
        btl.stat.cnt.opr[oprIndex] = (Int16)(FF9StateSystem.Battle.FF9Battle.status_data[statTblNo].opr_cnt * defaultFactor);
        if (Configuration.Battle.StatusTickFormula.Length > 0)
        {
            Expression e = new Expression(Configuration.Battle.StatusTickFormula);
            e.Parameters["StatusIndex"] = (Int32)statTblNo;
            e.Parameters["IsPositiveStatus"] = oprIndex == 2;
            e.Parameters["IsNegativeStatus"] = oprIndex == 0 || oprIndex == 1;
            e.Parameters["ContiCnt"] = (Int32)FF9StateSystem.Battle.FF9Battle.status_data[statTblNo].conti_cnt;
            e.Parameters["OprCnt"] = (Int32)FF9StateSystem.Battle.FF9Battle.status_data[statTblNo].opr_cnt;
            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            NCalcUtility.InitializeExpressionUnit(ref e, new BattleUnit(btl), "Target");
            Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
            if (val >= 0)
                btl.stat.cnt.opr[oprIndex] = (Int16)Math.Min(val, Int16.MaxValue);
        }
    }

    public static void SetPresentColor(BTL_DATA btl)
    {
        if (CheckStatus(btl, BattleStatus.Petrify))
            SetStatusClut(btl, true);
        else if (CheckStatus(btl, BattleStatusConst.ChgPolyCol))
            SetStatusPolyColor(btl);
        btl_util.SetBBGColor(btl.gameObject);
        if (btl.bi.player == 0)
            return;
        btl_util.SetBBGColor(btl.weapon_geo);
    }

    private static void SetStatusPolyColor(BTL_DATA btl)
    {
        BBGINFO bbgInfoPtr = battlebg.nf_GetBbgInfoPtr();
        Int16[] numArray1 = new Int16[3];
        Int16[] numArray2 = new Int16[3] { bbgInfoPtr.chr_r, bbgInfoPtr.chr_g, bbgInfoPtr.chr_b };
        if (CheckStatus(btl, BattleStatus.Zombie))
        {
            numArray1[0] = 48;
            numArray1[1] = 72;
            numArray1[2] = 88;
        }
        else if (CheckStatus(btl, BattleStatus.Berserk))
        {
            numArray1[0] = -16;
            numArray1[1] = 40;
            numArray1[2] = 40;
        }
        else if (CheckStatus(btl, BattleStatus.Heat))
        {
            numArray1[0] = -80;
            numArray1[1] = 16;
            numArray1[2] = 72;
        }
        else if (CheckStatus(btl, BattleStatus.Freeze))
        {
            numArray1[0] = 48;
            numArray1[1] = 0;
            numArray1[2] = -96;
        }
        else
            numArray1[0] = numArray1[1] = numArray1[2] = 0;
        for (Int32 index = 0; index < 3; ++index)
            btl.add_col[index] = numArray2[index] - numArray1[index] >= 0 ? numArray2[index] - numArray1[index] <= Byte.MaxValue ? (Byte)((UInt32)numArray2[index] - (UInt32)numArray1[index]) : Byte.MaxValue : (Byte)0;
    }

    public static void SetStatusClut(BTL_DATA btl, Boolean sw)
    {
        GameObject gameObject = btl.gameObject;
        Int32 num = !sw ? 0 : 1;
        if (num != 0)
            GeoTexAnim.geoTexAnimFreezeState(btl);
        else
            GeoTexAnim.geoTexAnimReturnState(btl);
        foreach (Renderer componentsInChild in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            componentsInChild.material.SetFloat("_IsPetrify", num);
        foreach (Renderer componentsInChild in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (Material material in componentsInChild.materials)
                material.SetFloat("_IsPetrify", num);
        }
    }

    public static Boolean CheckStatus(BTL_DATA btl, BattleStatus status)
    {
        return (btl.stat.permanent & status) != 0 || (btl.stat.cur & status) != 0;
    }

    public static void CheckStatusLoop(BTL_DATA btl, Boolean ignoreAtb)
    {
        CheckStatuses(btl, ignoreAtb);
        RotateAfterCheckStatusLoop(btl);
    }

    private static void CheckStatuses(BTL_DATA btl, Boolean ignoreAtb)
    {
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        STAT_INFO stat = btl.stat;
        BattleUnit unit = new BattleUnit(btl);

        SetStatusVfx(unit);

        if (unit.IsUnderStatus(BattleStatus.Death))
        {
            if (Configuration.Mod.TranceSeek && unit.IsPlayer)
            {
                if (unit.Trance == 255 && unit.IsUnderStatus(BattleStatus.Trance) && btl.gameObject != btl.tranceGo) // TRANCE SEEK - Prevent trance if character die in "combo"
                {
                    btl_cmd.KillSpecificCommand(unit.Data, BattleCommandId.SysTrans);
                    unit.RemoveStatus(BattleStatus.Trance);
                    unit.Trance = 254;
                }
                // TRANCE SEEK - Refresh stats on Death
                btl.elem.str = unit.Player.Data.elem.str;
                btl.elem.wpr = unit.Player.Data.elem.wpr;
                btl.elem.mgc = unit.Player.Data.elem.mgc;
                btl.defence.PhysicalDefence = unit.Player.Data.defence.PhysicalDefence;
                btl.defence.PhysicalEvade = unit.Player.Data.defence.PhysicalEvade;
                btl.defence.MagicalDefence = unit.Player.Data.defence.MagicalDefence;
                btl.defence.MagicalEvade = unit.Player.Data.defence.MagicalEvade;
            }
            btl_mot.DieSequence(btl);
            return;
        }
        if (Configuration.Mod.TranceSeek && !unit.IsUnderStatus(BattleStatus.Death | BattleStatus.Trance) && unit.Trance == 255) // TRANCE SEEK - Trance removes Petrify
        {
            unit.RemoveStatus(BattleStatus.Petrify);
            unit.AlterStatus(BattleStatus.Trance);
            return;
        }

        if (unit.IsUnderAnyStatus(BattleStatus.Petrify))
            return;

        if (!unit.IsUnderAnyStatus(BattleStatus.Stop | BattleStatus.Jump))
            btl.bi.atb = 1;

        if (!ignoreAtb && !UIManager.Battle.FF9BMenu_IsEnableAtb())
            return;

        if ((Configuration.TranceMonster.OverTrance) && btl.bi.player == 0 && btl.bi.t_gauge != 0 && !CheckStatus(btl, BattleStatus.Trance))
        {
            if  (UnityEngine.Random.Range(0, Configuration.TranceMonster.OverTranceChance) == 0)
            {
                unit.AlterStatus(BattleStatus.Trance);
                unit.Trance = byte.MaxValue;
            }
        }
        if (btl.bi.atb == 0)
        {
            if (unit.IsUnderStatus(BattleStatus.Jump) && (ff9Battle.cmd_status & 16) == 0 && (stat.cnt.conti[(Int32)BattleStatusNumber.Jump - 1] -= btl.cur.at_coef) < 0)
            {
                if (btl.cmd[3].cmd_no == BattleCommandId.Jump)
                    btl_cmd.SetCommand(btl.cmd[1], BattleCommandId.JumpAttack, (Int32)BattleAbilityId.Spear1, btl.cmd[3].tar_id, Comn.countBits(btl.cmd[3].tar_id) > 1 ? 1u : 0u);
                else
                    btl_cmd.SetCommand(btl.cmd[1], BattleCommandId.JumpTrance, (Int32)BattleAbilityId.Spear2, btl.cmd[3].tar_id, Comn.countBits(btl.cmd[3].tar_id) > 1 ? 1u : 0u);
            }

            return;
        }

        if (unit.IsUnderAnyStatus(BattleStatus.Venom))
        {
            if (stat.cnt.opr[0] <= 0)
            {
                SetOprStatusCount(btl, 1);
                btl_para.SetPoisonDamage(btl);
                btl_para.SetPoisonMpDamage(btl);
                btl2d.Btl2dStatReq(btl);
            }
            else
            {
                stat.cnt.opr[0] -= btl.cur.at_coef;
            }
        }

        if (unit.IsUnderAnyStatus(BattleStatus.Poison))
        {
            if (stat.cnt.opr[1] <= 0)
            {
                SetOprStatusCount(btl, 16);
                btl_para.SetPoisonDamage(btl);
                btl2d.Btl2dStatReq(btl);
            }
            else
            {
                stat.cnt.opr[1] -= btl.cur.at_coef;
            }
        }

        if (unit.IsUnderAnyStatus(BattleStatus.Regen))
        {
            if (stat.cnt.opr[2] <= 0)
            {
                SetOprStatusCount(btl, 18);
                btl_para.SetRegeneRecover(btl);
                btl2d.Btl2dStatReq(btl);
            }
            else
            {
                stat.cnt.opr[2] -= btl.cur.at_coef;
            }          
        }
        if (Configuration.Mod.TranceSeek)
        {
            if (unit.IsUnderAnyStatus(BattleStatus.Virus)) // TRANCE SEEK - Virus mechanic
            {
                if (btl.cur.hp > 0U)
                    btl.cur.hp -= 1U;
                else
                    new BattleUnit(btl).Kill();
            }
        }
        if (unit.IsUnderAnyStatus(BattleStatus.Trance) && btl.bi.slot_no == (Byte)CharacterId.Garnet && (ff9Battle.cmd_status & 4) != 0 && (ff9Battle.cmd_status & 8) == 0)
        {
            if (ff9Battle.phantom_cnt <= 0)
            {
                btl_cmd.SetCommand(btl.cmd[3], BattleCommandId.SysPhantom, (Int32)ff9Battle.phantom_no, btl_util.GetStatusBtlID(1U, 0U), 8U);
                ff9Battle.cmd_status |= 8;
            }
            else
            {
                ff9Battle.phantom_cnt -= btl.cur.at_coef;
            }
        }
        ActiveTimeStatus(btl);
    }

    private static void RotateAfterCheckStatusLoop(BTL_DATA btl)
    {
        if (CheckStatus(btl, BattleStatus.Confuse)
            && !btl_util.IsBtlUsingCommand(btl)
            && (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL)
                || btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING)
                || (btl.bi.player != 0 && btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD))))
        {
            Vector3 eulerAngles = btl.rot.eulerAngles;
            eulerAngles.y += 11.25f;
            btl.rot.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }
    }

    public static void SetStatusVfx(BattleUnit unit)
    {
        BTL_DATA data = unit.Data;
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        if (data.bi.disappear == 0 && !CheckStatus(data, BattleStatus.Petrify))
        {
            BBGINFO bbgInfoPtr = battlebg.nf_GetBbgInfoPtr();
            if (CheckStatus(data, BattleStatusConst.ChgPolyCol))
            {
                if (!FF9StateSystem.Battle.isFade)
                    btl_util.GeoSetABR(data.gameObject, "PSX/BattleMap_StatusEffect");
                btl_util.GeoSetColor2DrawPacket(data.gameObject, data.add_col[0], data.add_col[1], data.add_col[2], Byte.MaxValue);
                if (data.weapon_geo)
                    btl_util.GeoSetColor2DrawPacket(data.weapon_geo, data.add_col[0], data.add_col[1], data.add_col[2], Byte.MaxValue);
            }
            else if (data.special_status_old)
            {
                btl_util.GeoSetColor2DrawPacket(data.gameObject, 255, 255, 255);
            }
            else if (CheckStatus(data, BattleStatus.Shell | BattleStatus.Protect))
            {
                if (!FF9StateSystem.Battle.isFade)
                    btl_util.GeoSetABR(data.gameObject, "PSX/BattleMap_StatusEffect");
                Byte counter = (Byte)(ff9Battle.btl_cnt % 24);
                Int16 r;
                Int16 g;
                Int16 b;
                if ((!CheckStatus(data, BattleStatus.Protect) || !CheckStatus(data, BattleStatus.Shell) ? (!CheckStatus(data, BattleStatus.Protect) ? 1 : 0) : (ff9Battle.btl_cnt % 48 >= 24 ? 1 : 0)) != 0)
                {
                    r = (Int16)(bbgInfoPtr.chr_r - 64);
                    g = (Int16)(bbgInfoPtr.chr_g - -24);
                    b = (Int16)(bbgInfoPtr.chr_b - -72);
                }
                else
                {
                    r = (Int16)(bbgInfoPtr.chr_r - -40);
                    g = (Int16)(bbgInfoPtr.chr_g - -40);
                    b = (Int16)(bbgInfoPtr.chr_b - 80);
                }
                Byte strength = counter >= 8 ? (counter >= 16 ? (Byte)(24U - counter) : (Byte)8) : counter;
                r = (Int16)(r * strength >> 3);
                g = (Int16)(g * strength >> 3);
                b = (Int16)(b * strength >> 3);
                GeoAddColor2DrawPacket(data.gameObject, r, g, b);
                if (data.weapon_geo)
                    GeoAddColor2DrawPacket(data.weapon_geo, r, g, b);
            }
            else if (unit.IsUnderAnyStatus(BattleStatus.Trance) && !unit.IsUnderStatus(BattleStatus.Death) && data.tranceglowenabled)
            {
                if (!FF9StateSystem.Battle.isFade)
                    btl_util.GeoSetABR(data.gameObject, "PSX/BattleMap_StatusEffect");
                Byte counter = (Byte)(ff9Battle.btl_cnt % 16);
                Byte[] glowingColor = new Byte[3];
                glowingColor = unit.IsPlayer ? btl_mot.BattleParameterList[unit.SerialNumber].TranceGlowingColor : data.trancecolor ;
                Int16 r = (Int16)(bbgInfoPtr.chr_r - (128 - glowingColor[0]));
                Int16 g = (Int16)(bbgInfoPtr.chr_g - (128 - glowingColor[1]));
                Int16 b = (Int16)(bbgInfoPtr.chr_b - (128 - glowingColor[2]));
                Byte strength = counter >= 8 ? (Byte)(16U - counter) : counter;
                GeoAddColor2DrawPacket(data.gameObject, (Int16)(r * strength >> 3), (Int16)(g * strength >> 3), (Int16)(b * strength >> 3));
                if (data.weapon_geo)
                    GeoAddColor2DrawPacket(data.weapon_geo, (Int16)(r * strength >> 3), (Int16)(g * strength >> 3), (Int16)(b * strength >> 3));
            }
            else
            {
                // TRANCE SEEK - Friendly Ladybug (Miskoxy), swap wings colors (TODO - Transfert it to Memoria Scripts.DLL ?)
                SetDefaultShader(data);
                if (Configuration.Mod.TranceSeek && data.dms_geo_id == 405)
                {
                    if (unit.Strength == 31)
                    {
                        SetupCustomEnemyPartColor(data.gameObject, 255, 0, 0, true, 3);
                    }
                    else if (unit.Strength == 32)
                    {
                        SetupCustomEnemyPartColor(data.gameObject, 0, 255, 255, true, 3);
                    }
                    else if (unit.Strength == 33)
                    {
                        SetupCustomEnemyPartColor(data.gameObject, 255, 255, 0, true, 3);
                    }
                    else if (unit.Strength == 34)
                    {
                        SetupCustomEnemyPartColor(data.gameObject, 0, 0, 255, true, 3);
                    }
                }
            }
        }
        else if (CheckStatus(data, BattleStatus.Petrify))
        {
            SetDefaultShader(data);
            SetStatusClut(data, true);
        }
        if (FF9StateSystem.Battle.isDebug && FF9StateSystem.Battle.isLevitate)
        {
            Vector3 pos = data.pos;
            if (data.bi.player != 0)
            {
                pos.y = -200 - (Int32)(30 * ff9.rsin((ff9Battle.btl_cnt & 15) << 8) / 4096f);
                pos.y *= -1f;
            }
            data.pos = pos;
        }
        // Prevent auto-floating enemies to have the hovering movement
        if ((data.stat.cur & BattleStatus.Float) != 0u && ((data.stat.permanent & BattleStatus.Float) == 0 || data.bi.player != 0))
        {
            Single y = -200 - (Int32)(30 * ff9.rsin((ff9Battle.btl_cnt & 15) << 8) / 4096f);
            Vector3 vector = data.base_pos;
            vector.y = y;
            vector.y *= -1f;
            data.base_pos = vector;
            vector = data.pos;
            vector.y = y;
            vector.y *= -1f;
            data.pos = vector;
        }
    }

    public static void SetDefaultShader(BTL_DATA btl)
    {
        BBGINFO bbgInfoPtr = battlebg.nf_GetBbgInfoPtr();
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        if ((ff9Battle.btl_load_status & ff9btl.LOAD_CHR) == 0 || (ff9Battle.btl_load_status & ff9btl.LOAD_FADENPC) == 0 || FF9StateSystem.Battle.isFade)
            return;
        btl_util.GeoSetABR(btl.gameObject, "PSX/BattleMap_StatusEffect");
        btl_util.GeoSetColor2DrawPacket(btl.gameObject, bbgInfoPtr.chr_r, bbgInfoPtr.chr_g, bbgInfoPtr.chr_b, Byte.MaxValue);
    }

    public static void GeoAddColor2DrawPacket(GameObject go, Int16 r, Int16 g, Int16 b)
    {
        if (r < 0)
            r = 0;
        if (g < 0)
            g = 0;
        if (b < 0)
            b = 0;
        BBGINFO bbgInfoPtr = battlebg.nf_GetBbgInfoPtr();
        r = (Int16)(bbgInfoPtr.chr_r + r);
        g = (Int16)(bbgInfoPtr.chr_g + g);
        b = (Int16)(bbgInfoPtr.chr_b + b);
        if (r > Byte.MaxValue)
            r = Byte.MaxValue;
        if (g > Byte.MaxValue)
            g = Byte.MaxValue;
        if (b > Byte.MaxValue)
            b = Byte.MaxValue;
        SkinnedMeshRenderer[] componentsInChildren1 = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (Int32 index = 0; index < componentsInChildren1.Length; ++index)
        {
            if (r == 0 && g == 0 && b == 0)
            {
                componentsInChildren1[index].tag = "RGBZero";
                componentsInChildren1[index].enabled = false;
            }
            else
            {
                if (!componentsInChildren1[index].enabled && componentsInChildren1[index].CompareTag("RGBZero"))
                {
                    componentsInChildren1[index].enabled = true;
                    componentsInChildren1[index].tag = String.Empty;
                }
                componentsInChildren1[index].material.SetColor("_Color", new Color32((Byte)r, (Byte)g, (Byte)b, Byte.MaxValue));
            }
        }
        MeshRenderer[] componentsInChildren2 = go.GetComponentsInChildren<MeshRenderer>();
        for (Int32 index = 0; index < componentsInChildren2.Length; ++index)
        {
            if (r == 0 && g == 0 && b == 0)
            {
                componentsInChildren2[index].enabled = false;
            }
            else
            {
                componentsInChildren2[index].enabled = true;
                foreach (Material material in componentsInChildren2[index].materials)
                    material.SetColor("_Color", new Color32((Byte)r, (Byte)g, (Byte)b, Byte.MaxValue));
            }
        }
    }

    public static void SetupCustomEnemyPartColor(GameObject go, short r, short g, short b, bool skinned, int partindex, bool uselight = true)
    {
        if (uselight)
        {
            BBGINFO bBGINFO = battlebg.nf_GetBbgInfoPtr(); // On prend en compte la lumière ambiante
            r += (short)bBGINFO.chr_r;
            g += (short)bBGINFO.chr_g;
            b += (short)bBGINFO.chr_b;
        }
        if (r < 0)
        {
            r = 0;
        }
        if (g < 0)
        {
            g = 0;
        }
        if (b < 0)
        {
            b = 0;
        }
        if (r > 255)
        {
            r = 255;
        }
        if (g > 255)
        {
            g = 255;
        }
        if (b > 255)
        {
            b = 255;
        }
        if (skinned)
        {
            SkinnedMeshRenderer[] componentsInChildren = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (partindex < componentsInChildren.Length)
            {
                if (r == 0 && g == 0 && b == 0)
                {
                    componentsInChildren[partindex].tag = "RGBZero";
                    componentsInChildren[partindex].enabled = false;
                }
                else
                {
                    if (!componentsInChildren[partindex].enabled && componentsInChildren[partindex].CompareTag("RGBZero"))
                    {
                        componentsInChildren[partindex].enabled = true;
                        componentsInChildren[partindex].tag = string.Empty;
                    }
                    componentsInChildren[partindex].material.SetColor("_Color", new Color32((byte)r, (byte)g, (byte)b, 255));
                }
            }
        }
        else
        {
            MeshRenderer[] componentsInChildren2 = go.GetComponentsInChildren<MeshRenderer>();
            if (partindex < componentsInChildren2.Length)
            {
                if (r == 0 && g == 0 && b == 0)
                {
                    componentsInChildren2[partindex].enabled = false;
                }
                else
                {
                    componentsInChildren2[partindex].enabled = true;
                    Material[] materials = componentsInChildren2[partindex].materials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        Material material = materials[i];
                        material.SetColor("_Color", new Color32((byte)r, (byte)g, (byte)b, 255));
                    }
                }
            }
        }
    }
    private static void ActiveTimeStatus(BTL_DATA btl)
    {
        for (Int32 index = 0; index < 32; ++index)
        {
            BattleStatus status = (BattleStatus)(1 << index);
            if ((btl.stat.cur & BattleStatusConst.ContiCount & status) != 0 && (btl.stat.cnt.conti[index] -= btl.cur.at_coef) < 0)
            {
                if ((status & BattleStatus.GradualPetrify) != 0)
                {
                    if (!btl_cmd.CheckUsingCommand(btl.cmd[2]))
                    {
                        if (AlterStatus(btl, BattleStatus.Petrify) == 2)
                        {
                            BattleVoice.TriggerOnStatusChange(btl, "Used", BattleStatus.GradualPetrify);
                        }
                        else
                        {
                            RemoveStatus(btl, BattleStatus.GradualPetrify);
                            btl.fig_info |= Param.FIG_INFO_MISS;
                            btl2d.Btl2dReq(btl);
                        }
                    }
                }
                else if ((status & BattleStatus.Doom) != 0)
                {
                    if (btl_stat.CheckStatus(btl, BattleStatus.EasyKill))
                    {
                        // Enemies affected by Doom but with Easy kill proof (doesn't exist in vanilla) lose 1/5 of their Max HP instead (non-capped, except for avoiding softlocks)
                        // Might want to add a Configuration option for that effect...
                        Int32 doom_damage = (Int32)btl_para.GetLogicalHP(btl, true) / 5;
                        if (doom_damage > Math.Max(btl.cur.hp - 1, 9999))
                            doom_damage = (Int32)btl.cur.hp - 1;
                        if (doom_damage > 0)
                        {
                            BattleVoice.TriggerOnStatusChange(btl, "Used", BattleStatus.Doom);
                            btl_stat.RemoveStatus(btl, status);
                            btl.fig_info = Param.FIG_INFO_DISP_HP;
                            btl_para.SetDamage(new BattleUnit(btl), doom_damage, (Byte)(btl_mot.checkMotion(btl, btl.bi.def_idle) ? 1 : 0));
                            btl2d.Btl2dReq(btl);
                        }
                        else
                        {
                            btl.fig_info |= Param.FIG_INFO_MISS;
                            btl2d.Btl2dReq(btl);
                        }
                    }
                    else
                    {
                        BattleVoice.TriggerOnStatusChange(btl, "Used", BattleStatus.Doom);
                        btl_stat.AlterStatus(btl, BattleStatus.Death);
                        btl2d.Btl2dReq(btl);
                    }
                }
                else
                {
                    RemoveStatus(btl, status);
                }
            }
        }
    }

    public static void InitStatus(BTL_DATA btl)
    {
        STAT_CNT cnt = btl.stat.cnt;
        btl.stat.invalid = btl.stat.permanent = btl.stat.cur = 0U;
        for (Int32 index = 0; index < 3; ++index)
            cnt.opr[index] = 0;
        for (Int32 index = 0; index < 32; ++index)
            cnt.conti[index] = 0;
    }
}

public class StatusModifier : Memoria.Prime.Collections.EntryCollection<BattleStatus, Single>
{
    public StatusModifier(Single defaultValue) : base(defaultValue)
    {
    }
}