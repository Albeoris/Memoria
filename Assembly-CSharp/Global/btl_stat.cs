using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Scripts;
using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

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

    public static void StatusCommandCancel(BTL_DATA btl)
    {
        if (btl.bi.player != 0)
            UIManager.Battle.RemovePlayerFromAction(btl.btl_id, true);
        if (!btl_cmd.KillStandardCommands(btl))
            return;
        btl.bi.atb = 0;
        if (btl.bi.player != 0 && !FF9StateSystem.Settings.IsATBFull)
            btl.cur.at = 0;
        btl.sel_mode = 0;
    }

    public static UInt32 AlterStatus(BattleUnit target, BattleStatusId statusId, BattleUnit inflicter = null, Boolean usePartialResist = false, params Object[] parameters)
    {
        Dictionary<BattleStatusId, BattleStatusDataEntry> statusDatabase = FF9StateSystem.Battle.FF9Battle.status_data;
        BTL_DATA btl = target;
        STAT_INFO stat = btl.stat;
        BattleStatus status = statusId.ToBattleStatus();
        Boolean bypassResistances = statusId == BattleStatusId.Death && btl.cur.hp == 0;
        if (!bypassResistances && (status & stat.invalid) != 0)
            return ALTER_RESIST;
        if (!bypassResistances && usePartialResist)
        {
            Single rate = target.PartialResistStatus[statusId];
            if (rate > 0f && UnityEngine.Random.Range(0f, 1f) < rate)
                return ALTER_INVALID;
        }
        if ((status & stat.permanent) != 0 || ((status & stat.cur) != 0 && (status & BattleStatusConst.NoReset) != 0))
            return ALTER_INVALID;
        BattleStatus invalidStatuses = btl.bi.t_gauge == 0 ? BattleStatus.Trance : 0;
        foreach (BattleStatusId curStatus in stat.cur.ToStatusList())
            invalidStatuses |= statusDatabase[curStatus].ImmunityProvided;
        if (btl_cmd.CheckSpecificCommand(btl, BattleCommandId.SysStone))
            invalidStatuses |= statusDatabase[BattleStatusId.Petrify].ImmunityProvided;
        if (!bypassResistances && (status & invalidStatuses) != 0)
            return ALTER_INVALID;
        if (!stat.effects.TryGetValue(statusId, out StatusScriptBase script))
            script = ScriptsLoader.GetStatusScript(statusId);
        if (script == null)
            throw new Exception($"[btl_stat] Trying to apply the status {statusId} but it doesn't have a script.");
        UInt32 result = script.Apply(target, inflicter, parameters);
        if (result != ALTER_SUCCESS)
            return result;
        // The status is successfully (re-)added
        BattleStatusDataEntry statusData = statusDatabase[statusId];
        stat.effects[statusId] = script;
        stat.cur |= status;
        if ((status & BattleStatusConst.CmdCancel) != 0)
            StatusCommandCancel(btl);
        if ((status & BattleStatusConst.ChgPolyClut) != 0)
            SetStatusClut(btl, true);
        if (statusData.SPSEffect >= 0)
            HonoluluBattleMain.battleSPS.AddBtlSPSObj(target, statusId, spsId: statusData.SPSEffect, extraPos: statusData.SPSExtraPos);
        if (statusData.SHPEffect >= 0)
            HonoluluBattleMain.battleSPS.AddBtlSPSObj(target, statusId, shpId: statusData.SHPEffect, extraPos: statusData.SHPExtraPos);
        if (btl.bi.player != 0 && !btl.is_monster_transform)
        {
            if ((status & BattleStatusConst.FrozenAnimation & BattleStatusConst.IdleDying) != 0 && !btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING))
            {
                btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING);
                btl.evt.animFrame = 0;
            }
            else if ((status & BattleStatusConst.FrozenAnimation & BattleStatusConst.IdleDefend) != 0 && !btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE))
            {
                btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE);
                btl.evt.animFrame = 0;
            }
        }
        if (FF9StateSystem.Battle.FF9Battle.btl_phase > FF9StateBattleSystem.PHASE_ENTER && (status & BattleStatusConst.BattleEnd) != 0)
            btl_sys.CheckBattlePhase(btl);
        RemoveStatuses(target, statusData.ClearOnApply);
        if (CheckStatus(btl, BattleStatusConst.StopAtb))
            btl.bi.atb = 0;
        if ((status & BattleStatusConst.ContiCount) != 0)
        {
            Int16 defaultFactor = (status & BattleStatusConst.AnyNegative) != 0 ? (Int16)(60 - btl.elem.wpr << 3) :
                                  (status & BattleStatusConst.AnyPositive) != 0 ? (Int16)(btl.elem.wpr << 3) : (Int16)(60 - btl.elem.wpr << 2);
            stat.conti[statusId] = (Int16)(statusData.ContiCnt * defaultFactor);
            if (!String.IsNullOrEmpty(Configuration.Battle.StatusDurationFormula))
            {
                Expression e = new Expression(Configuration.Battle.StatusDurationFormula);
                e.Parameters["StatusId"] = (Int32)statusId;
                e.Parameters["IsPositiveStatus"] = (status & BattleStatusConst.AnyPositive) != 0;
                e.Parameters["IsNegativeStatus"] = (status & BattleStatusConst.AnyNegative) != 0;
                e.Parameters["ContiCnt"] = (Int32)statusData.ContiCnt;
                e.Parameters["OprCnt"] = (Int32)statusData.OprCnt;
                e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                NCalcUtility.InitializeExpressionUnit(ref e, target, "Target");
                NCalcUtility.InitializeExpressionNullableUnit(ref e, inflicter, "Inflicter");
                Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
                if (val >= 0)
                    stat.conti[statusId] = (Int16)Math.Min(val, Int16.MaxValue);
            }
            stat.conti[statusId] = (Int16)(stat.duration_factor[statusId] * stat.conti[statusId]);
        }
        if (script is IOprStatusScript)
            SetOprStatusCount(target, statusId);
        if (btl.bi.player != 0)
            BattleAchievement.UpdateAbnormalStatus(status);
        BattleVoice.TriggerOnStatusChange(btl, "Added", statusId);
        return result;
    }

    public static UInt32 AlterStatuses(BattleUnit target, BattleStatus statuses, BattleUnit inflicter = null, Boolean usePartialResist = false)
    {
        UInt32 bestResult = 0;
        foreach (BattleStatusId statusId in statuses.ToStatusList())
            bestResult = Math.Max(bestResult, AlterStatus(target, statusId, inflicter, usePartialResist));
        return bestResult;
    }

    public static UInt32 RemoveStatus(BattleUnit unit, BattleStatusId statusId)
    {
        BattleStatus status = statusId.ToBattleStatus();
        BTL_DATA btl = unit;
        STAT_INFO stat = btl.stat;
        if ((stat.permanent & status) != 0 || (stat.cur & status) == 0 || (btl.bi.player == 0 && FF9StateSystem.Battle.FF9Battle.btl_phase == FF9StateBattleSystem.PHASE_MENU_OFF && (status & BattleStatusConst.BattleEnd) != 0))
            return 1;
        if (stat.effects.TryGetValue(statusId, out StatusScriptBase effect))
        {
            if (!effect.Remove())
                return 1;
            stat.effects.Remove(statusId);
        }
        BattleStatusDataEntry statusData = statusId.GetStatData();
        stat.cur &= ~status;
        if ((status & BattleStatusConst.ChgPolyClut) != 0)
            SetStatusClut(btl, CheckStatus(btl, BattleStatusConst.ChgPolyClut));
        if (statusData.SPSEffect >= 0 || statusData.SHPEffect >= 0)
            HonoluluBattleMain.battleSPS.RemoveBtlSPSObj(unit, statusId);
        BattleVoice.TriggerOnStatusChange(btl, "Removed", statusId);
        if (stat.permanent_on_hold != 0)
            MakeStatusesPermanent(unit, stat.permanent_on_hold, true);
        return 2;
    }

    public static UInt32 RemoveStatuses(BattleUnit unit, BattleStatus statuses)
    {
        UInt32 result = 1;
        foreach (BattleStatusId statusId in statuses.ToStatusList())
            result = Math.Max(result, RemoveStatus(unit, statusId));
        return result;
    }

    public static void MakeStatusesPermanent(BattleUnit unit, BattleStatus statuses, Boolean flag = true, BattleUnit inflicter = null)
    {
        STAT_INFO stat = unit.Data.stat;
        if (flag)
        {
            foreach (BattleStatusId statusId in statuses.ToStatusList())
            {
                UInt32 alterResult = AlterStatus(unit, statusId, inflicter);
                if (alterResult == ALTER_SUCCESS_NO_SET) // Try a second time, in case it removed an opposite status
                    AlterStatus(unit, statusId, inflicter);
            }
            stat.permanent |= statuses & stat.cur;
            stat.permanent_on_hold = (stat.permanent_on_hold | statuses) & ~stat.permanent;
            // Permanent statuses should also be registered as current statuses
            //btl.stat.cur &= ~(statuses & btl.stat.cur);
        }
        else
        {
            // Don't remove the status if it wasn't permanent in the first place
            stat.permanent_on_hold &= ~statuses;
            statuses &= stat.permanent;
            stat.permanent &= ~statuses;
            btl_stat.RemoveStatuses(unit, statuses);
        }
    }

    public static void SetOprStatusCount(BattleUnit unit, BattleStatusId statusId)
    {
        // Formula priorities:
        // (1) IOprStatusScript.SetupOpr
        // (2) StatusTickFormula
        // (3) Default hardcoded Opr duration
        BTL_DATA btl = unit;
        UInt16 defaultFactor;
        if (statusId == BattleStatusId.Venom) // Venom
            defaultFactor = (UInt16)((UInt32)btl.elem.wpr << 2);
        else if (statusId == BattleStatusId.Poison) // Poison
            defaultFactor = (UInt16)((UInt32)btl.elem.wpr << 2);
        else // Regen or others
            defaultFactor = (UInt16)(60 - btl.elem.wpr << 2);
        btl.stat.opr[statusId] = (Int16)(statusId.GetStatData().OprCnt * defaultFactor);
        if (btl.stat.effects.TryGetValue(statusId, out StatusScriptBase effect) && (effect as IOprStatusScript)?.SetupOpr != null)
        {
            btl.stat.opr[statusId] = (effect as IOprStatusScript).SetupOpr();
        }
        else if (!String.IsNullOrEmpty(Configuration.Battle.StatusTickFormula))
        {
            Expression e = new Expression(Configuration.Battle.StatusTickFormula);
            e.Parameters["StatusId"] = (Int32)statusId;
            e.Parameters["IsPositiveStatus"] = (BattleStatusConst.AnyPositive & statusId.ToBattleStatus()) != 0;
            e.Parameters["IsNegativeStatus"] = (BattleStatusConst.AnyNegative & statusId.ToBattleStatus()) != 0;
            e.Parameters["ContiCnt"] = (Int32)statusId.GetStatData().ContiCnt;
            e.Parameters["OprCnt"] = (Int32)statusId.GetStatData().OprCnt;
            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            NCalcUtility.InitializeExpressionUnit(ref e, unit, "Target");
            Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
            if (val >= 0)
                btl.stat.opr[statusId] = (Int16)Math.Min(val, Int16.MaxValue);
        }
    }

    public static void SetPresentColor(BTL_DATA btl)
    {
        if (CheckStatus(btl, BattleStatusConst.ChgPolyClut))
            SetStatusClut(btl, true);
        btl_util.SetBBGColor(btl.gameObject);
        if (btl.weapon_geo != null)
            btl_util.SetBBGColor(btl.weapon_geo);
    }

    public static void SetStatusClut(BTL_DATA btl, Boolean sw)
    {
        Int32 petrifyFlag = sw ? 1 : 0;
        if (sw)
            GeoTexAnim.geoTexAnimFreezeState(btl);
        else
            GeoTexAnim.geoTexAnimReturnState(btl);
        foreach (Renderer renderers in btl.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            renderers.material.SetFloat("_IsPetrify", petrifyFlag);
        foreach (Renderer renderers in btl.gameObject.GetComponentsInChildren<MeshRenderer>())
            foreach (Material material in renderers.materials)
                material.SetFloat("_IsPetrify", petrifyFlag);
    }

    public static Boolean CheckStatus(BTL_DATA btl, BattleStatus status)
    {
        // Permanent statuses are also current, at least that's the target behaviour
        //return (btl.stat.permanent & status) != 0 || (btl.stat.cur & status) != 0;
        return (btl.stat.cur & status) != 0;
    }

    public static Boolean CheckStatus(BTL_DATA btl, BattleStatusId statusId)
    {
        return (btl.stat.cur & statusId.ToBattleStatus()) != 0;
    }

    public static void CheckStatusLoop(BattleUnit unit)
    {
        BTL_DATA btl = unit.Data;
        STAT_INFO stat = btl.stat;

        SetStatusVfx(unit);

        if (unit.IsUnderStatus(BattleStatus.Death))
        {
            btl_mot.DieSequence(btl);
            return;
        }

        if (!unit.IsUnderAnyStatus(BattleStatusConst.StopAtb))
            btl.bi.atb = 1;

        if (!UIManager.Battle.FF9BMenu_IsEnableAtb())
            return;

        if (btl.bi.atb == 0)
            return;

        BattleStatus removeList = 0;
        foreach (BattleStatusId statusId in unit.CurrentStatus.ToStatusList())
        {
            if ((statusId.ToBattleStatus() & BattleStatusConst.OprCount) == 0)
                continue;
            if (stat.opr[statusId] <= 0)
            {
                Boolean shouldRemove = false;
                SetOprStatusCount(unit, statusId);
                if (btl.stat.effects.TryGetValue(statusId, out StatusScriptBase effect) && effect is IOprStatusScript)
                    shouldRemove = (effect as IOprStatusScript).OnOpr();
                if (shouldRemove)
                    removeList |= statusId.ToBattleStatus();
            }
            else
            {
                stat.opr[statusId] -= btl.cur.at_coef;
            }
        }
        foreach (BattleStatusId statusId in removeList.ToStatusList())
            RemoveStatus(unit, statusId);

        ActiveTimeStatus(unit);
    }

    public static void RotateAfterCheckStatusLoop(BTL_DATA btl)
    {
        // Dummied
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
        if (CheckStatus(data, BattleStatusConst.ChgPolyClut))
        {
            SetDefaultShader(data);
            SetStatusClut(data, true);
        }
        else if (data.bi.disappear == 0)
        {
            data.CustomGlowEffect.Remove(data.CustomGlowEffect.FirstOrDefault(CustomGlowBTL => CustomGlowBTL.Status != BattleStatusId.None));
            BBGINFO bbgInfoPtr = battlebg.nf_GetBbgInfoPtr();
            Int32 highestPriority = Int32.MinValue;
            Boolean useColor = false;
            foreach (BattleStatusId statusId in unit.CurrentStatus.ToStatusList())
            {
                BattleStatusDataEntry statusData = statusId.GetStatData();
                if (statusData.ColorKind >= 0)
                {
                    BTL_DATA.CUSTOM_GLOW NewGlowEffect = new BTL_DATA.CUSTOM_GLOW()
                    {
                        Status = statusId,
                        ColorKind = statusData.ColorKind,
                        ColorPriority = statusData.ColorPriority,
                        ColorBase = statusData.ColorBase
                    };

                    if (statusData.ColorPriority == highestPriority)
                    {
                        data.CustomGlowEffect.Add(NewGlowEffect);
                    }
                    else if (statusData.ColorPriority > highestPriority)
                    {
                        data.CustomGlowEffect.Remove(data.CustomGlowEffect.FirstOrDefault(CustomGlowBTL => CustomGlowBTL.Status != BattleStatusId.None));
                        data.CustomGlowEffect.Add(NewGlowEffect);
                        highestPriority = statusData.ColorPriority;
                    }
                }
            }

            if (data.CustomGlowEffect.Count > 0)
            {
                Int32 kind = data.CustomGlowEffect[0].ColorKind;
                useColor = true;
                switch (kind)
                {
                    case 0: // Like Freeze or Berserk
                    {
                        Byte r = (Byte)Mathf.Clamp(bbgInfoPtr.chr_r + data.CustomGlowEffect[0].ColorBase[0], 0, Byte.MaxValue);
                        Byte g = (Byte)Mathf.Clamp(bbgInfoPtr.chr_g + data.CustomGlowEffect[0].ColorBase[1], 0, Byte.MaxValue);
                        Byte b = (Byte)Mathf.Clamp(bbgInfoPtr.chr_b + data.CustomGlowEffect[0].ColorBase[2], 0, Byte.MaxValue);
                        if (!FF9StateSystem.Battle.isFade)
                            btl_util.GeoSetABR(data.gameObject, "PSX/BattleMap_StatusEffect", data);
                        btl_util.GeoSetColor2DrawPacket(data.gameObject, r, g, b, Byte.MaxValue);
                        if (data.weapon_geo)
                            btl_util.GeoSetColor2DrawPacket(data.weapon_geo, r, g, b, Byte.MaxValue);
                        break;
                    }
                    case 1: // Like Shell or Protect
                    {
                        Int32 index = ff9Battle.btl_cnt / 24 % data.CustomGlowEffect.Count;
                        Byte counter = (Byte)(ff9Battle.btl_cnt % 24);
                        Byte strength = (Byte)(counter >= 8 ? (counter >= 16 ? (24 - counter) : 8) : counter);
                        Int16 r = (Int16)((bbgInfoPtr.chr_r + data.CustomGlowEffect[index].ColorBase[0]) * strength >> 3);
                        Int16 g = (Int16)((bbgInfoPtr.chr_g + data.CustomGlowEffect[index].ColorBase[1]) * strength >> 3);
                        Int16 b = (Int16)((bbgInfoPtr.chr_b + data.CustomGlowEffect[index].ColorBase[2]) * strength >> 3);

                        if (!FF9StateSystem.Battle.isFade)
                            btl_util.GeoSetABR(data.gameObject, "PSX/BattleMap_StatusEffect", data);
                        GeoAddColor2DrawPacket(data.gameObject, r, g, b);
                        if (data.weapon_geo)
                            GeoAddColor2DrawPacket(data.weapon_geo, r, g, b);
                        break;
                    }
                    case 2: // Trance
                    {
                        if (!unit.IsUnderStatus(BattleStatus.Death) && ((unit.IsPlayer && unit.IsUnderAnyStatus(BattleStatus.Trance)) || (!unit.IsPlayer && unit.Data.enable_trance_glow)))
                        {
                            Byte counter = (Byte)(ff9Battle.btl_cnt % 16);
                            Byte strength = counter >= 8 ? (Byte)(16 - counter) : counter;
                            Int32[] glowingColor = unit.IsPlayer ? btl_mot.BattleParameterList[unit.SerialNumber].TranceGlowingColor : unit.Enemy.Data.trance_glowing_color;
                            Int16 r = (Int16)((bbgInfoPtr.chr_r + glowingColor[0] - 128) * strength >> 3);
                            Int16 g = (Int16)((bbgInfoPtr.chr_g + glowingColor[1] - 128) * strength >> 3);
                            Int16 b = (Int16)((bbgInfoPtr.chr_b + glowingColor[2] - 128) * strength >> 3);
                            if (!FF9StateSystem.Battle.isFade)
                                btl_util.GeoSetABR(data.gameObject, "PSX/BattleMap_StatusEffect", data);
                            GeoAddColor2DrawPacket(data.gameObject, r, g, b);
                            if (data.weapon_geo)
                                GeoAddColor2DrawPacket(data.weapon_geo, r, g, b);
                        }
                        else
                        {
                            useColor = false;
                        }
                        break;
                    }
                }
            }
            if (!useColor)
                SetDefaultShader(data);
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
    }
    public static void AddCustomGlowEffect(BTL_DATA btl, int ColorKind, int ColorPriority, int[] ColorBase, int ID)
    {
        BTL_DATA.CUSTOM_GLOW CustomGlowBTL = new BTL_DATA.CUSTOM_GLOW();
        CustomGlowBTL.ID = ID;
        CustomGlowBTL.ColorKind = ColorKind;
        CustomGlowBTL.ColorPriority = ColorPriority;
        CustomGlowBTL.ColorBase = ColorBase;
        btl.CustomGlowEffect.Add(CustomGlowBTL);
    }

    public static void RemoveCustomGlowEffect(BTL_DATA btl, int ID)
    {
        btl.CustomGlowEffect.Remove(btl.CustomGlowEffect.FirstOrDefault(CustomGlowBTL => CustomGlowBTL.ID == ID));
    }

    public static void ClearAllCustomGlowEffect(BTL_DATA btl)
    {
        btl.CustomGlowEffect.Clear();
    }

    public static void SetDefaultShader(BTL_DATA btl)
    {
        BBGINFO bbgInfoPtr = battlebg.nf_GetBbgInfoPtr();
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        if ((ff9Battle.btl_load_status & ff9btl.LOAD_CHR) == 0 || (ff9Battle.btl_load_status & ff9btl.LOAD_FADENPC) == 0 || FF9StateSystem.Battle.isFade)
            return;
        btl_util.GeoSetABR(btl.gameObject, "PSX/BattleMap_StatusEffect", btl);
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
        foreach (SkinnedMeshRenderer renderer in go.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (r == 0 && g == 0 && b == 0)
            {
                renderer.tag = "RGBZero";
                renderer.enabled = false;
            }
            else
            {
                if (!renderer.enabled && renderer.CompareTag("RGBZero"))
                {
                    renderer.enabled = true;
                    renderer.tag = String.Empty;
                }
                renderer.material.SetColor("_Color", new Color32((Byte)r, (Byte)g, (Byte)b, Byte.MaxValue));
            }
        }
        foreach (MeshRenderer renderer in go.GetComponentsInChildren<MeshRenderer>())
        {
            if (r == 0 && g == 0 && b == 0)
            {
                renderer.enabled = false;
            }
            else
            {
                renderer.enabled = true;
                foreach (Material material in renderer.materials)
                    material.SetColor("_Color", new Color32((Byte)r, (Byte)g, (Byte)b, Byte.MaxValue));
            }
        }
    }

    private static void ActiveTimeStatus(BattleUnit unit)
    {
        BTL_DATA btl = unit;
        STAT_INFO stat = btl.stat;
        foreach (BattleStatusId statusId in (stat.cur & BattleStatusConst.ContiCount).ToStatusList())
        {
            stat.conti[statusId] -= btl.cur.at_coef;
            if (stat.conti[statusId] < 0)
            {
                RemoveStatus(unit, statusId);
            }
        }
    }

    public static void InitStatus(BTL_DATA btl)
    {
        STAT_INFO stat = btl.stat;
        stat.invalid = stat.permanent = stat.cur = stat.permanent_on_hold = 0;
        stat.effects.Clear();
        stat.opr.Clear();
        stat.conti.Clear();
        stat.partial_resist.Clear();
        stat.duration_factor.Clear();
    }

    public const UInt32 ALTER_RESIST = 0; // Cannot apply because of status resistance ("Guard")
    public const UInt32 ALTER_INVALID = 1; // Cannot apply for other reasons ("Miss", status incompatibility etc...)
    public const UInt32 ALTER_SUCCESS_NO_SET = 2; // Had an effect, like removing an opposite status, but didn't register the status in stat.cur
    public const UInt32 ALTER_SUCCESS = 3; // Successfully applied the status
}

public class StatusModifier : Memoria.Prime.Collections.EntryCollection<BattleStatusId, Single>
{
    public StatusModifier(Single defaultValue) : base(defaultValue)
    {
    }
}
