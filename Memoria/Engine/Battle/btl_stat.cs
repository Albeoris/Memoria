using System;
using UnityEngine;
using FF9;
using Memoria;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable EmptyConstructor
// ReSharper disable UnusedMember.Global
// ReSharper disable MoreSpecificForeachVariableTypeAvailable
// ReSharper disable RedundantExplicitArraySize
// ReSharper disable InconsistentNaming

[ExportedType("uĳľHńńńńńńńń5!!!ĄÀļěÖK3wĩİrÀ¦ÛÏ³ÒÊ×ÊAķ[DQ1;ĤăêāªĻóJËÞŁ¿ĄĳÛÇĒ,·ªúIčiå÷ĔtīÙÈÊĭ¥¡ÚįpĹľ:lÒĖãÊSçëńńńń")]
public class btl_stat
{
    public btl_stat()
    {
    }

    public static void SaveStatus(PLAYER p, BTL_DATA btl)
    {
        p.status = (byte)(btl.stat.cur & (uint)sbyte.MaxValue);
    }

    public static void InitCountDownStatus(BTL_DATA btl)
    {
        btl.stat.cnt.cdown_max = (short)((60 - btl.elem.wpr << 3) * FF9StateSystem.Battle.FF9Battle.status_data[27].conti_cnt);
    }

    public static void StatusCommandCancel(BTL_DATA btl, uint status)
    {
        if (!btl_cmd.KillCommand2(btl))
            return;
        btl.bi.atb = 0;
        if (btl.bi.player != 0 && !FF9StateSystem.Settings.IsATBFull)
            btl.cur.at = 0;
        if (((int)status & 256) != 0)
            return;
        btl.sel_mode = 0;
        if (btl.bi.player != 0)
            UIManager.Battle.RemovePlayerFromAction(btl.btl_id, true);
        btl.cmd[0].cmd_no = 0;
    }

    public static uint AlterStatus(BTL_DATA btl, uint status)
    {
        STAT_DATA[] statusData = FF9StateSystem.Battle.FF9Battle.status_data;
        STAT_INFO stat = btl.stat;
        uint statTblNo = 0;
        if (((int)stat.invalid & (int)status) != 0)
            return 0;
        if (((int)stat.permanent & (int)status) != 0 || ((int)stat.cur & (int)status) != 0 && ((int)status & -2013200513) != 0)
            return 1;
        uint num1 = 0;
        uint num2 = 0;
        for (; num1 < 32U; ++num1)
        {
            uint num3 = 1U << (int)num1;
            if (((int)status & (int)num3) != 0)
                statTblNo = num1;
            if (((int)stat.cur & (int)num3) != 0)
                num2 |= statusData[num1].invalid;
        }
        if (btl_cmd.CheckSpecificCommand(btl, 62))
            num2 |= statusData[0].invalid;
        if (((int)num2 & (int)status) != 0)
            return 1;
        if (((int)status & 270008321) == 0)
        {
            if (((int)status & 134403) != 0)
                StatusCommandCancel(btl, status);
            stat.cur |= status;
        }
        uint num4 = statTblNo;
        switch (num4)
        {
            case 6:
                if (btl.bi.player != 0 && !Status.checkCurStat(btl, 16384U))
                    btl.trance = 0;
                SetStatusPolyColor(btl);
                break;
            case 8:
                if (btl.cur.hp > 0)
                {
                    btl.fig_info |= 64;
                    new BattleUnit(btl).Kill();
                }
                else
                    btl.cur.hp = 0;
                btl.cur.at = 0;
                if (!btl_cmd.CheckUsingCommand(btl.cmd[2]))
                {
                    btl_cmd.SetCommand(btl.cmd[2], 60U, 0U, btl.btl_id, 0U);
                    if (btl.bi.player == 0)
                        btl_sys.CheckForecastMenuOff(btl);
                }
                btl_cmd.KillSpecificCommand(btl, 59);
                break;
            case 11:
            case 24:
            case 25:
                SetStatusPolyColor(btl);
                break;
            case 12:
                btl_sys.CheckBattlePhase(btl);
                break;
            case 14:
                btl_cmd.SetCommand(btl.cmd[4], 59U, 0U, btl.btl_id, 0U);
                break;
            case 17:
                if (btl.bi.player != 0 && !btl_mot.checkMotion(btl, 1) && !btl_util.isCurCmdOwner(btl))
                {
                    btl_mot.setMotion(btl, 1);
                    btl.evt.animFrame = 0;
                }
                break;
            case 19:
                if (Status.checkCurStat(btl, 1048576U))
                {
                    RemoveStatus(btl, 1048576U);
                    return 2;
                }
                btl_para.InitATB(btl);
                btl.cur.at_coef = (sbyte)(btl.cur.at_coef * 3 / 2);
                stat.cur |= status;
                break;
            case 20:
                if (Status.checkCurStat(btl, 524288U))
                {
                    RemoveStatus(btl, 524288U);
                    return 2;
                }
                btl_para.InitATB(btl);
                btl.cur.at_coef = (sbyte)(btl.cur.at_coef * 2 / 3);
                stat.cur |= status;
                break;
            case 26:
                btl_mot.HideMesh(btl, btl.mesh_banish, true);
                break;
            case 28:
                stat.cur ^= status;
                if (Status.checkCurStat(btl, 268435456U))
                {
                    geo.geoScaleSet(btl, 2048);
                    btlshadow.FF9ShadowSetScaleBattle(btl_util.GetFF9CharNo(btl), (byte)(btl.shadow_x / 2U), (byte)(btl.shadow_z / 2U));
                    break;
                }
                geo.geoScaleReset(btl);
                btlshadow.FF9ShadowSetScaleBattle(btl_util.GetFF9CharNo(btl), btl.shadow_x, btl.shadow_z);
                break;
            default:
                if ((int)num4 != 0)
                {
                    if ((int)num4 == 1)
                    {
                        if (FF9StateSystem.Battle.FF9Battle.btl_phase > 2)
                            btl_sys.CheckBattlePhase(btl);
                        if (btl.bi.player != 0 && !btl_mot.checkMotion(btl, 1) && !btl_util.isCurCmdOwner(btl))
                        {
                            btl_mot.setMotion(btl, 1);
                            btl.evt.animFrame = 0;
                        }
                    }
                    break;
                }
                if (!btl_cmd.CheckUsingCommand(btl.cmd[2]))
                {
                    if (FF9StateSystem.Battle.FF9Battle.btl_phase > 2)
                    {
                        btl_cmd.SetCommand(btl.cmd[2], 62U, 0U, btl.btl_id, 0U);
                        break;
                    }
                    stat.cur |= status;
                    btl.bi.atb = 0;
                    SetStatusClut(btl, true);
                }
                break;
        }
        RemoveStatuses(btl, statusData[statTblNo].clear);
        if (Status.checkCurStat(btl, 1073746177U))
            btl.bi.atb = 0;
        if (((int)status & -268500992) != 0)
        {
            short num3 = ((int)status & -1693253632) == 0 ? (((int)status & 619446272) == 0 ? (short)(60 - btl.elem.wpr << 2) : (short)(btl.elem.wpr << 3)) : (short)(60 - btl.elem.wpr << 3);
            btl.stat.cnt.conti[statTblNo - 16U] = (short)(statusData[statTblNo].conti_cnt * num3);
        }
        if (((int)status & 327682) != 0)
            SetOprStatusCount(btl, statTblNo);
        HonoluluBattleMain.battleSPS.AddBtlSPSObj(btl, status);
        if (btl.bi.player != 0)
            BattleAchievement.UpdateAbnormalStatus(status);
        return 2;
    }

    public static uint AlterStatuses(BTL_DATA btl, uint statuses)
    {
        uint num1 = 0;
        for (uint index = 0; index < 32U; ++index)
        {
            uint status = 1U << (int)index;
            if (((int)statuses & (int)status) != 0)
            {
                uint num2 = AlterStatus(btl, status);
                if ((int)num1 == 0 && num2 > 0U || (int)num1 == 1 && num2 > 1U)
                    num1 = num2;
            }
        }
        return num1;
    }

    public static uint RemoveStatus(BTL_DATA btl, uint status)
    {
        STAT_INFO stat = btl.stat;
        if (((int)stat.permanent & (int)status) != 0 || ((int)stat.cur & (int)status) == 0 || btl.bi.player == 0 && FF9StateSystem.Battle.FF9Battle.btl_phase == 5 && (status & 4099L) != 0L)
            return 1;
        stat.cur &= ~status;
        switch (status)
        {
            case 1:
                SetStatusClut(btl, false);
                break;
            case 64:
            case 16777216:
            case 33554432:
                if (Status.checkCurStat(btl, 50333760U))
                {
                    SetStatusPolyColor(btl);
                }
                break;
            case 256:
                btl.die_seq = 0;
                btl.bi.dmg_mot_f = 0;
                btl.bi.cmd_idle = 0;
                btl.bi.death_f = 0;
                btl.bi.stop_anim = 0;
                btl.escape_key = 0;
                if (btl_mot.checkMotion(btl, 4) || btl_mot.checkMotion(btl, 8))
                {
                    GeoTexAnim.geoTexAnimPlay(btl.texanimptr, 2);
                    if (btl.bi.player != 0)
                        GeoTexAnim.geoTexAnimPlay(btl.tranceTexanimptr, 2);
                    btl_mot.setMotion(btl, 6);
                    btl.evt.animFrame = 0;
                }
                if (FF9StateSystem.Battle.FF9Battle.cur_cmd.regist != btl || FF9StateSystem.Battle.FF9Battle.cur_cmd.cmd_no > 48)
                    btl.sel_mode = 0;
                btl_cmd.KillSpecificCommand(btl, 60);
                if (((int)btl.stat.permanent & 262144) != 0)
                {
                    SetOprStatusCount(btl, 18U);
                }
                break;
            case 1024:
                Vector3 eulerAngles = btl.rot.eulerAngles;
                eulerAngles.y = btl.evt.rotBattle.eulerAngles.y;
                btl.rot = Quaternion.Euler(eulerAngles);
                StatusCommandCancel(btl, status);
                break;
            case 2048:
                StatusCommandCancel(btl, status);
                if (Status.checkCurStat(btl, 50333760U))
                {
                    SetStatusPolyColor(btl);
                }
                break;
            case 16384:
                btl.trance = 0;
                if (Status.checkCurStat(btl, 1073741824U))
                {
                    RemoveStatus(btl, 1073741824U);
                    btl.SetDisappear(0);
                    btl_mot.setBasePos(btl);
                    btl_mot.setMotion(btl, btl.bi.def_idle);
                    btl.evt.animFrame = 0;
                }
                btl_cmd.SetCommand(btl.cmd[4], 59U, 0U, btl.btl_id, 0U);
                break;
            case 524288:
            case 1048576:
                btl_para.InitATB(btl);
                break;
            case 2097152:
                float value = 0f;
                btl.pos[1] = value;
                btl.base_pos[1] = value;
                break;
            case 67108864:
                btl_mot.ShowMesh(btl, btl.mesh_banish, true);
                break;
            case 134217728:
                if (btl.deathMessage != null)
                {
                    Singleton<HUDMessage>.Instance.ReleaseObject(btl.deathMessage);
                    btl.deathMessage = null;
                }
                break;
            case 268435456:
                geo.geoScaleReset(btl);
                btlshadow.FF9ShadowSetScaleBattle(btl_util.GetFF9CharNo(btl), btl.shadow_x, btl.shadow_z);
                break;
            case 1073741824:
                btl.tar_mode = 3;
                btl.bi.atb = 1;
                if (btl.bi.player != 0 && !FF9StateSystem.Settings.IsATBFull)
                    btl.cur.at = 0;
                btl.sel_mode = 0;
                btl.cmd[3].cmd_no = 0;
                btl.cmd[3].tar_id = 0;
                break;
            case 2147483648:
                if (btl.petrifyMessage != null)
                {
                    Singleton<HUDMessage>.Instance.ReleaseObject(btl.petrifyMessage);
                    btl.petrifyMessage = null;
                }
                btl_cmd.KillSpecificCommand(btl, 62);
                break;
        }
        HonoluluBattleMain.battleSPS.RemoveBtlSPSObj(btl, status);
        return 2;
    }

    public static uint RemoveStatuses(BTL_DATA btl, uint statuses)
    {
        uint num = 1;
        for (uint index = 0; index < 32U; ++index)
        {
            uint status = 1U << (int)index;
            if (((int)statuses & (int)status) != 0 && (int)RemoveStatus(btl, status) == 2)
                num = 2U;
        }
        return num;
    }

    public static void SetOprStatusCount(BTL_DATA btl, uint statTblNo)
    {
        ushort num1;
        ushort num2;
        if ((int)statTblNo == 1)
        {
            num1 = 0;
            num2 = (ushort)((uint)btl.elem.wpr << 2);
        }
        else if ((int)statTblNo == 16)
        {
            num1 = 1;
            num2 = (ushort)((uint)btl.elem.wpr << 2);
        }
        else
        {
            num1 = 2;
            num2 = (ushort)(60 - btl.elem.wpr << 2);
        }
        btl.stat.cnt.opr[num1] = (short)(FF9StateSystem.Battle.FF9Battle.status_data[statTblNo].opr_cnt * num2);
    }

    public static void SetPresentColor(BTL_DATA btl)
    {
        if (Status.checkCurStat(btl, 1U))
            SetStatusClut(btl, true);
        else if (Status.checkCurStat(btl, 50333760U))
            SetStatusPolyColor(btl);
        btl_util.SetBBGColor(btl.gameObject);
        if (btl.bi.player == 0)
            return;
        btl_util.SetBBGColor(btl.weapon_geo);
    }

    private static void SetStatusPolyColor(BTL_DATA btl)
    {
        BBGINFO bbgInfoPtr = battlebg.nf_GetBbgInfoPtr();
        short[] numArray1 = new short[3];
        short[] numArray2 = new short[3] {bbgInfoPtr.chr_r, bbgInfoPtr.chr_g, bbgInfoPtr.chr_b};
        if (Status.checkCurStat(btl, 64U))
        {
            numArray1[0] = 48;
            numArray1[1] = 72;
            numArray1[2] = 88;
        }
        else if (Status.checkCurStat(btl, 2048U))
        {
            numArray1[0] = -16;
            numArray1[1] = 40;
            numArray1[2] = 40;
        }
        else if (Status.checkCurStat(btl, 16777216U))
        {
            numArray1[0] = -80;
            numArray1[1] = 16;
            numArray1[2] = 72;
        }
        else if (Status.checkCurStat(btl, 33554432U))
        {
            numArray1[0] = 48;
            numArray1[1] = 0;
            numArray1[2] = -96;
        }
        else
            numArray1[0] = numArray1[1] = numArray1[2] = 0;
        for (int index = 0; index < 3; ++index)
            btl.add_col[index] = numArray2[index] - numArray1[index] >= 0 ? numArray2[index] - numArray1[index] <= byte.MaxValue ? (byte)((uint)numArray2[index] - (uint)numArray1[index]) : byte.MaxValue : (byte)0;
    }

    public static void SetStatusClut(BTL_DATA btl, bool sw)
    {
        GameObject gameObject = btl.gameObject;
        int num = !sw ? 0 : 1;
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

    public static bool CheckStatus(BTL_DATA btl, uint status)
    {
        return ((int)btl.stat.permanent & (int)status) != 0 || ((int)btl.stat.cur & (int)status) != 0;
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
        SetStatusVfx(btl);

        BattleUnit unit = new BattleUnit(btl);
        if (unit.IsUnderStatus(BattleStatus.Disable))
        {
            btl_mot.DieSequence(btl);
            return;
        }

        if (unit.IsUnderStatus(BattleStatus.Stone))
            return;

        if (!unit.IsUnderStatus(BattleStatus.Stop | BattleStatus.Jump))
            btl.bi.atb = 1;

        if (!ignoreAtb && !UIManager.Battle.FF9BMenu_IsEnableAtb())
            return;

        if (btl.bi.atb == 0)
        {
            if (unit.IsUnderStatus(BattleStatus.Jump) && (ff9Battle.cmd_status & 16) == 0 && (stat.cnt.conti[14] -= btl.cur.at_coef) < 0)
            {
                if (btl.cmd[3].cmd_no == 3)
                    btl_cmd.SetCounter(btl, 10U, 185, btl.cmd[3].tar_id);
                else
                    btl_cmd.SetCounter(btl, 11U, 186, btl.cmd[3].tar_id);
            }

            return;
        }

        if (unit.IsUnderStatus(BattleStatus.Poison2))
        {
            if (stat.cnt.opr[0] <= 0)
            {
                SetOprStatusCount(btl, 1U);
                btl_para.SetPoisonDamage(btl);
                btl_para.SetPoisonMpDamage(btl);
                btl2d.Btl2dStatReq(btl);
            }
            else
                stat.cnt.opr[0] -= btl.cur.at_coef;
        }

        if (unit.IsUnderStatus(BattleStatus.Poison))
        {
            if (stat.cnt.opr[1] <= 0)
            {
                SetOprStatusCount(btl, 16U);
                btl_para.SetPoisonDamage(btl);
                btl2d.Btl2dStatReq(btl);
            }
            else
                stat.cnt.opr[1] -= btl.cur.at_coef;
        }

        if (unit.IsUnderStatus(BattleStatus.Regen) || unit.IsUnderPermanentStatus(BattleStatus.Regen))
        {
            if (stat.cnt.opr[2] <= 0)
            {
                SetOprStatusCount(btl, 18U);
                btl_para.SetRegeneRecover(btl);
                btl2d.Btl2dStatReq(btl);
            }
            else
                stat.cnt.opr[2] -= btl.cur.at_coef;
        }

        if (unit.IsUnderStatus(BattleStatus.Trans) && btl.bi.slot_no == 2 && ((ff9Battle.cmd_status & 4) != 0 && (ff9Battle.cmd_status & 8) == 0))
        {
            if (ff9Battle.phantom_cnt <= 0)
            {
                btl_cmd.SetCommand(btl.cmd[3], 57U, ff9Battle.phantom_no, btl_util.GetStatusBtlID(1U, 0U), 8U);
                ff9Battle.cmd_status |= 8;
            }
            else
                ff9Battle.phantom_cnt -= btl.cur.at_coef;
        }

        ActiveTimeStatus(btl);
    }

    private static void RotateAfterCheckStatusLoop(BTL_DATA btl)
    {
        if (Status.checkCurStat(btl, 1024u) && !btl_util.isCurCmdOwner(btl) && (btl_mot.checkMotion(btl, 0) || btl_mot.checkMotion(btl, 1) || (btl.bi.player != 0 && btl_mot.checkMotion(btl, 9))))
        {
            Vector3 eulerAngles = btl.rot.eulerAngles;
            eulerAngles.y += 11.25f;
            btl.rot.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }
    }

    public static void SetStatusVfx(BTL_DATA btl)
    {
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        if (btl.bi.disappear == 0 && !Status.checkCurStat(btl, 1U))
        {
            BBGINFO bbgInfoPtr = battlebg.nf_GetBbgInfoPtr();
            if (Status.checkCurStat(btl, 50333760U))
            {
                if (!FF9StateSystem.Battle.isFade)
                    btl_util.GeoSetABR(btl.gameObject, "PSX/BattleMap_StatusEffect");
                btl_util.GeoSetColor2DrawPacket(btl.gameObject, btl.add_col[0], btl.add_col[1], btl.add_col[2], byte.MaxValue);
                if (btl.weapon_geo)
                    btl_util.GeoSetColor2DrawPacket(btl.weapon_geo, btl.add_col[0], btl.add_col[1], btl.add_col[2], byte.MaxValue);
            }
            else if (Status.checkCurStat(btl, 12582912U))
            {
                if (!FF9StateSystem.Battle.isFade)
                    btl_util.GeoSetABR(btl.gameObject, "PSX/BattleMap_StatusEffect");
                byte num1 = (byte)(ff9Battle.btl_cnt % 24);
                short num2;
                short num3;
                short num4;
                if ((!Status.checkCurStat(btl, 8388608U) || !Status.checkCurStat(btl, 4194304U) ? (!Status.checkCurStat(btl, 8388608U) ? 1 : 0) : (ff9Battle.btl_cnt % 48 >= 24 ? 1 : 0)) != 0)
                {
                    num2 = (short)(bbgInfoPtr.chr_r - 64);
                    num3 = (short)(bbgInfoPtr.chr_g - -24);
                    num4 = (short)(bbgInfoPtr.chr_b - -72);
                }
                else
                {
                    num2 = (short)(bbgInfoPtr.chr_r - -40);
                    num3 = (short)(bbgInfoPtr.chr_g - -40);
                    num4 = (short)(bbgInfoPtr.chr_b - 80);
                }
                byte num5 = (int)num1 >= 8 ? ((int)num1 >= 16 ? (byte)(24U - num1) : (byte)8) : num1;
                short r = (short)(num2 * num5 >> 3);
                short g = (short)(num3 * num5 >> 3);
                short b = (short)(num4 * num5 >> 3);
                GeoAddColor2DrawPacket(btl.gameObject, r, g, b);
                if (btl.weapon_geo)
                    GeoAddColor2DrawPacket(btl.weapon_geo, r, g, b);
            }
            else if (Status.checkCurStat(btl, 16384U) && !Status.checkCurStat(btl, 256U))
            {
                if (btl_util.getSerialNumber(btl) + 19 < btl_init.model_id.Length)
                {
                    if (!FF9StateSystem.Battle.isFade)
                        btl_util.GeoSetABR(btl.gameObject, "PSX/BattleMap_StatusEffect");
                    byte[][] numArray = new byte[8][]
                    {
                        new byte[3] {byte.MaxValue, 96, 96},
                        new byte[3] {104, 120, byte.MaxValue},
                        new byte[3] {byte.MaxValue, 184, 120},
                        new byte[3] {128, 192, 208},
                        new byte[3] {192, 104, 144},
                        new byte[3] {208, 112, 120},
                        new byte[3] {200, 216, 104},
                        new byte[3] {208, 184, 104}
                    };
                    byte num1 = (byte)(ff9Battle.btl_cnt % 16);
                    short num2 = (short)(bbgInfoPtr.chr_r - (128 - numArray[btl.bi.slot_no][0]));
                    short num3 = (short)(bbgInfoPtr.chr_g - (128 - numArray[btl.bi.slot_no][1]));
                    short num4 = (short)(bbgInfoPtr.chr_b - (128 - numArray[btl.bi.slot_no][2]));
                    byte num5 = (int)num1 >= 8 ? (byte)(16U - num1) : num1;
                    GeoAddColor2DrawPacket(btl.gameObject, (short)(num2 * num5 >> 3), (short)(num3 * num5 >> 3), (short)(num4 * num5 >> 3));
                    if (btl.weapon_geo)
                    {
                        GeoAddColor2DrawPacket(btl.weapon_geo, (short)(num2 * num5 >> 3), (short)(num3 * num5 >> 3), (short)(num4 * num5 >> 3));
                    }
                }
            }
            else if ((FF9StateSystem.Battle.battleMapIndex == 892 || FF9StateSystem.Battle.battleMapIndex == 893 || FF9StateSystem.Battle.battleMapIndex >= 865 && FF9StateSystem.Battle.battleMapIndex <= 867) && (btl.btl_id == 32 || btl.btl_id == 64))
            {
                BTL_DATA btlDataPtr = btl_scrp.GetBtlDataPtr(16);
                if (btlDataPtr == null || !Status.checkCurStat(btlDataPtr, 256U))
                {
                }
            }
            else
                SetDefaultShader(btl);
        }
        else if (Status.checkCurStat(btl, 1U))
        {
            SetDefaultShader(btl);
            SetStatusClut(btl, true);
        }
        if (FF9StateSystem.Battle.isDebug && FF9StateSystem.Battle.isLevitate)
        {
            Vector3 pos = btl.pos;
            if (btl.bi.player != 0)
            {
                pos.y = -200 - (int)(30 * ff9.rsin((ff9Battle.btl_cnt & 15) << 8) / 4096f);
                pos.y *= -1f;
            }
            btl.pos = pos;
        }
        if (CheckStatus(btl, 2097152u))
        {
            float y = -200 - (int)(30 * ff9.rsin((ff9Battle.btl_cnt & 15) << 8) / 4096f);
            Vector3 vector = btl.base_pos;
            vector.y = y;
            vector.y *= -1f;
            btl.base_pos = vector;
            vector = btl.pos;
            vector.y = y;
            vector.y *= -1f;
            btl.pos = vector;
        }
    }

    public static void SetDefaultShader(BTL_DATA btl)
    {
        BBGINFO bbgInfoPtr = battlebg.nf_GetBbgInfoPtr();
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        if ((ff9Battle.btl_load_status & 4) == 0 || (ff9Battle.btl_load_status & 32) == 0 || FF9StateSystem.Battle.isFade)
            return;
        btl_util.GeoSetABR(btl.gameObject, "PSX/BattleMap_StatusEffect");
        btl_util.GeoSetColor2DrawPacket(btl.gameObject, bbgInfoPtr.chr_r, bbgInfoPtr.chr_g, bbgInfoPtr.chr_b, byte.MaxValue);
    }

    public static void GeoAddColor2DrawPacket(GameObject go, short r, short g, short b)
    {
        if (r < 0)
            r = 0;
        if (g < 0)
            g = 0;
        if (b < 0)
            b = 0;
        BBGINFO bbgInfoPtr = battlebg.nf_GetBbgInfoPtr();
        r = (short)(bbgInfoPtr.chr_r + r);
        g = (short)(bbgInfoPtr.chr_g + g);
        b = (short)(bbgInfoPtr.chr_b + b);
        if (r > byte.MaxValue)
            r = byte.MaxValue;
        if (g > byte.MaxValue)
            g = byte.MaxValue;
        if (b > byte.MaxValue)
            b = byte.MaxValue;
        SkinnedMeshRenderer[] componentsInChildren1 = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int index = 0; index < componentsInChildren1.Length; ++index)
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
                    componentsInChildren1[index].tag = string.Empty;
                }
                componentsInChildren1[index].material.SetColor("_Color", new Color32((byte)r, (byte)g, (byte)b, byte.MaxValue));
            }
        }
        MeshRenderer[] componentsInChildren2 = go.GetComponentsInChildren<MeshRenderer>();
        for (int index = 0; index < componentsInChildren2.Length; ++index)
        {
            if (r == 0 && g == 0 && b == 0)
            {
                componentsInChildren2[index].enabled = false;
            }
            else
            {
                componentsInChildren2[index].enabled = true;
                foreach (Material material in componentsInChildren2[index].materials)
                    material.SetColor("_Color", new Color32((byte)r, (byte)g, (byte)b, byte.MaxValue));
            }
        }
    }

    private static void ActiveTimeStatus(BTL_DATA btl)
    {
        for (int index = 0; index < 16; ++index)
        {
            uint status = (uint)(65536 << index);
            if (((int)btl.stat.cur & -268500992 & (int)status) != 0 && (btl.stat.cnt.conti[index] -= btl.cur.at_coef) < 0)
            {
                if (((int)status & int.MinValue) != 0)
                {
                    if (!btl_cmd.CheckUsingCommand(btl.cmd[2]) && (int)AlterStatus(btl, 1U) != 2)
                    {
                        RemoveStatus(btl, 2147483648U);
                        btl.fig_info |= 32;
                        btl2d.Btl2dReq(btl);
                    }
                }
                else if (((int)status & 134217728) != 0)
                {
                    AlterStatus(btl, 256U);
                    btl2d.Btl2dReq(btl);
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
        for (uint index = 0; index < 3U; ++index)
            cnt.opr[index] = 0;
        for (uint index = 0; index < 14U; ++index)
            cnt.conti[index] = 0;
    }
}