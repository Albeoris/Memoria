using FF9;
using System;
using System.Collections.Generic;
using System.Linq;
using Memoria;
using UnityEngine;

// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable EmptyConstructor
// ReSharper disable ArrangeThisQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToConstant.Global
// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming

[ExportedType("@ĮMĔńńńńńńńńI!!!ĄÀļě¯ĩń}'Ĉĺā¨M­oÄĲĄ_£ĲkĸcďËĢ`·±DsĦËčâĎÀÿĈ`āy-ĦÖ1bčâ5-ó½Á»ĝSÖđxløý¬ďN'Äıī&Ęêyô§:Æ­ĭE¾$ÄĺėÞÂUòýÇ3%ď¼ÜLñ±1ÝđAOXKaĒįuÐėªÏ|üôóĩ4#gkkoö&Į´<HĮġFāłÝŁlŀpÈëbzr÷Ä$eÍÃRńńńń")]
public class btl_cmd
{
    public btl_cmd()
    {
    }

    public static void ClearCommand(CMD_DATA cmd)
    {
        cmd.next = null;
        cmd.aa = null;
        cmd.tar_id = 0;
        cmd.cmd_no = 0;
        cmd.sub_no = 0;
        cmd.info.cursor = 0;
        cmd.info.stat = 0;
        cmd.info.priority = 0;
        cmd.info.cover = 0;
        cmd.info.dodge = 0;
        cmd.info.reflec = 0;
        cmd.info.meteor_miss = 0;
        cmd.info.short_summon = 0;
        cmd.info.mon_reflec = 0;
    }

    public static void ClearReflecData(BTL_DATA btl)
    {
        for (int index = 0; index < 4; ++index)
            btl.reflec.tar_id[index] = 0;
    }

    public static void InitCommandSystem(FF9StateBattleSystem btlsys)
    {
        btlsys.cur_cmd = null;
        btlsys.cmd_mode = 0;
        btlsys.cmd_status = 2;
        btlsys.cmd_queue.regist = btlsys.cmd_escape.regist = null;
        ClearCommand(btlsys.cmd_queue);
        ClearCommand(btlsys.cmd_escape);
    }

    public static void InitSelectCursor(FF9StateBattleSystem btlsys)
    {
        uint num1 = (uint)((FF9StateSystem.Battle.FF9Battle.btl_cnt & 15) << 8);
        List<Vector3> vector3List1 = new List<Vector3>();
        List<Vector3> vector3List2 = new List<Vector3>();
        List<Color32> color32List = new List<Color32>();
        List<int> intList = new List<int>();
        Vector3 vector3_1 = new Vector3(0.0f, 0.0f, 0.0f);
        vector3List1.Add(vector3_1);
        int num2 = 0;
        while (num2 < 3)
        {
            float f = (float)(num1 / 4096.0 * 360.0);
            Vector3 vector3_2 = new Vector3(vector3_1.x - (int)(68.0 * Mathf.Cos(f)), vector3_1.y - 133f, vector3_1.z - (int)(68.0 * Mathf.Sin(f)));
            vector3_2.y *= -1f;
            vector3List1.Add(vector3_2);
            ++num2;
            num1 = (num1 + 1265U) % 4096U;
        }
        vector3List2.Add(vector3List1[0]);
        vector3List2.Add(vector3List1[2]);
        vector3List2.Add(vector3List1[1]);
        vector3List2.Add(vector3List1[0]);
        vector3List2.Add(vector3List1[3]);
        vector3List2.Add(vector3List1[2]);
        vector3List2.Add(vector3List1[0]);
        vector3List2.Add(vector3List1[1]);
        vector3List2.Add(vector3List1[3]);
        vector3List2.Add(vector3List1[1]);
        vector3List2.Add(vector3List1[2]);
        vector3List2.Add(vector3List1[3]);
        for (int index = 0; index < vector3List2.Count; ++index)
        {
            byte num3 = (byte)Mathf.Floor(index / 3f);
            byte num4 = index >= 9 ? (byte)160 : (byte)(188 - 60 * num3);
            color32List.Add(new Color32(num4, num4, 0, byte.MaxValue));
            intList.Add(index);
        }
        Mesh mesh = new Mesh
        {
            vertices = vector3List2.ToArray(),
            triangles = intList.ToArray(),
            colors32 = color32List.ToArray()
        };
        mesh.RecalculateNormals();
        GameObject gameObject = btlsys.s_cur = new GameObject("selectCursor");
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        Material material = new Material(Shader.Find("PSX/BattleMap_SelectCursor_Abr_1"));
        meshRenderer.material = material;
        gameObject.SetActive(false);
    }

    public static void InitCommand(BTL_DATA btl)
    {
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        if (stateBattleSystem.btl_phase == 2)
            btl.tar_mode = 1;
        btl.sel_mode = 0;
        btl.finger_disp = false;
        //int index1 = btl.bi.player == 0 ? 24 + (btl.bi.line_no - 4) * 3 : btl.bi.line_no * 6;
        //CMD_DATA cmdData = stateBattleSystem.cmd_buffer[index1];
        for (int index2 = 0; index2 < 6; ++index2)
        {
            if (btl.bi.player != 0 || index2 < 3)
            {
                btl.cmd[index2] = new CMD_DATA {regist = btl};
                ClearCommand(btl.cmd[index2]);
            }
            else
                btl.cmd[index2] = null;
        }
        ClearReflecData(btl);
    }

    public static void SetCommand(CMD_DATA cmd, uint cmd_no, uint sub_no, ushort tar_id, uint cursor)
    {
        BTL_DATA btl = cmd.regist;
        uint num1 = cmd_no;
        switch (num1)
        {
            case 56:
                if ((FF9StateSystem.Battle.FF9Battle.cmd_status & 1) != 0)
                {
                    cmd.sub_no = (byte)sub_no;
                    return;
                }
                FF9StateSystem.Battle.FF9Battle.cmd_status |= 1;
                cmd.aa = FF9StateSystem.Battle.FF9Battle.aa_data[180];
                break;
            case 60:
            case 61:
            case 62:
                cmd.aa = FF9StateSystem.Battle.FF9Battle.aa_data[0];
                break;
            default:
                switch (num1)
                {
                    case 10:
                    case 11:
                        FF9StateSystem.Battle.FF9Battle.cmd_status |= 16;
                        cmd.aa = FF9StateSystem.Battle.FF9Battle.aa_data[sub_no];
                        break;
                    case 14:
                        cmd.aa = FF9StateSystem.Battle.FF9Battle.aa_data[0];
                        break;
                    case 15:
                        cmd.aa = FF9StateSystem.Battle.FF9Battle.aa_data[190];
                        break;
                    default:
                        if ((int)num1 != 50)
                        {
                            if ((int)num1 != 51)
                            {
                                if ((int)num1 == 2)
                                {
                                    if (HasSupportAbility(btl, SupportAbility2.Mug))
                                        sub_no = 181U;
                                    cmd.aa = FF9StateSystem.Battle.FF9Battle.aa_data[sub_no];
                                    break;
                                }
                                cmd.aa = FF9StateSystem.Battle.FF9Battle.aa_data[sub_no];
                                break;
                            }
                            goto case 14;
                        }
                        cmd.aa = FF9StateSystem.Battle.FF9Battle.enemy_attack[sub_no];
                        break;
                }
                break;
        }

        cmd.tar_id = tar_id;
        cmd.cmd_no = (byte)cmd_no;
        cmd.sub_no = (byte)sub_no;
        cmd.info.cursor = (byte)cursor;
        cmd.info.cover = 0;
        cmd.info.dodge = 0;
        cmd.info.reflec = 0;
        if (cmd_no > 48U)
        {
            cmd.info.priority = 1;
        }
        else
        {
            /*int num2 = (int)*/
            btl_stat.RemoveStatus(btl, 32768U);
        }
        if (cmd_no < 55U)
        {
            if (btl_util.getCurCmdPtr() != btl.cmd[4])
            {
                if (btl_mot.checkMotion(btl, 0))
                {
                    btl_mot.setMotion(btl, btl.mot[10]);
                    btl.evt.animFrame = 0;
                }
                else if (btl_mot.checkMotion(btl, 1))
                {
                    btl_mot.setMotion(btl, 11);
                    btl.evt.animFrame = 0;
                }
                else if (btl_mot.checkMotion(btl, 13) && cmd_no < 48U)
                {
                    btl_mot.setMotion(btl, 14);
                    btl.evt.animFrame = 0;
                }
            }
            btl.bi.cmd_idle = 1;
        }
        EnqueueCommand(cmd);
    }

    public static void SetCounter(BTL_DATA btl, uint cmd_no, int sub_no, ushort tar_id)
    {
        if (Status.checkCurStat(btl, 33689603U) || FF9StateSystem.Battle.FF9Battle.btl_phase != 4)
            return;
        SetCommand(btl.cmd[1], cmd_no, (uint)sub_no, tar_id, 0U);
    }

    public static short setPhantomCount(BTL_DATA btl)
    {
        return (short)((60 - btl.elem.wpr) * 4 * 50);
    }

    public static void SetAutoCommand(BTL_DATA btl)
    {
        if (Status.checkCurStat(btl, 1024U))
        {
            SetCommand(btl.cmd[0], 1U, 176U, btl_util.GetRandomBtlID((uint)(Comn.random8() & 1)), 0U);
        }
        else
        {
            if (!Status.checkCurStat(btl, 2048U))
                return;
            SetCommand(btl.cmd[0], 1U, 176U, btl_util.GetRandomBtlID(0U), 0U);
        }
    }

    public static void SetEnemyCommandBySequence(ushort tar_id, uint cmd_no, uint sub_no)
    {
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        BTL_DATA btl = btl_scrp.GetBtlDataPtr(16);
        BTL_DATA[] btlDataArray = stateBattleSystem.btl_data;
        SEQ_WORK_SET seqWorkSet = stateBattleSystem.seq_work_set;
        int num1 = Array.IndexOf(seqWorkSet.AnmOfsList.Distinct().ToArray(), seqWorkSet.AnmOfsList[sub_no]);
        for (int index = 0; index < btlDataArray.Length; ++index)
        {
            if (num1 == btlDataArray[index].typeNo)
            {
                btl = btlDataArray[index];
                break;
            }
        }
        if (btl == null)
            return;
        if ((stateBattleSystem.cmd_status & 1) != 0 || Status.checkCurStat(btl, 33689859U))
        {
            btl.sel_mode = 0;
        }
        else
        {
            CMD_DATA cmd;
            if ((int)cmd_no == 47)
            {
                if (stateBattleSystem.btl_phase != 4)
                {
                    btl.sel_mode = 0;
                    return;
                }
                cmd = btl.cmd[0];
                if (Status.checkCurStat(btl, 1024U))
                {
                    tar_id = btl_util.GetRandomBtlID((uint)(Comn.random8() & 1));
                    sub_no = btl_util.getEnemyTypePtr(btl).p_atk_no;
                }
                else if (Status.checkCurStat(btl, 2048U))
                {
                    tar_id = btl_util.GetRandomBtlID(1U);
                    sub_no = btl_util.getEnemyTypePtr(btl).p_atk_no;
                }
            }
            else if ((int)cmd_no == 53)
                cmd = btl.cmd[1];
            else if ((int)cmd_no == 54)
            {
                cmd = btl.cmd[1];
            }
            else
            {
                btl.sel_mode = 0;
                return;
            }
            cmd.aa = stateBattleSystem.enemy_attack[sub_no];
            cmd.aa.Ref.prog_no = 26;
            cmd.tar_id = tar_id;
            cmd.cmd_no = (byte)cmd_no;
            cmd.sub_no = (byte)sub_no;
            cmd.info.cursor = cmd.aa.Info.cursor < 6 || cmd.aa.Info.cursor >= 13 ? (byte)0 : (byte)1;
            if (cmd_no > 48U)
            {
                cmd.info.priority = 1;
            }
            else
            {
                /*int num2 = (int)*/
                btl_stat.RemoveStatus(cmd.regist, 32768U);
            }
            cmd.info.cover = 0;
            cmd.info.dodge = 0;
            cmd.info.reflec = 0;
            btl.bi.cmd_idle = 1;
            EnqueueCommand(cmd);
        }
    }

    public static void SetEnemyCommand(ushort own_id, ushort tar_id, uint cmd_no, uint sub_no)
    {
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        BTL_DATA btlDataPtr = btl_scrp.GetBtlDataPtr(own_id);
        if ((stateBattleSystem.cmd_status & 1) != 0 || Status.checkCurStat(btlDataPtr, 33689859U))
        {
            btlDataPtr.sel_mode = 0;
        }
        else
        {
            CMD_DATA cmd;
            if ((int)cmd_no == 47)
            {
                if (stateBattleSystem.btl_phase != 4)
                {
                    btlDataPtr.sel_mode = 0;
                    return;
                }
                cmd = btlDataPtr.cmd[0];
                if (Status.checkCurStat(btlDataPtr, 1024U))
                {
                    tar_id = btl_util.GetRandomBtlID((uint)(Comn.random8() & 1));
                    sub_no = btl_util.getEnemyTypePtr(btlDataPtr).p_atk_no;
                }
                else if (Status.checkCurStat(btlDataPtr, 2048U))
                {
                    tar_id = btl_util.GetRandomBtlID(1U);
                    sub_no = btl_util.getEnemyTypePtr(btlDataPtr).p_atk_no;
                }
            }
            else if ((int)cmd_no == 53)
                cmd = btlDataPtr.cmd[1];
            else if ((int)cmd_no == 54)
            {
                cmd = btlDataPtr.cmd[1];
            }
            else
            {
                btlDataPtr.sel_mode = 0;
                return;
            }
            cmd.aa = stateBattleSystem.enemy_attack[sub_no];
            cmd.tar_id = tar_id;
            cmd.cmd_no = (byte)cmd_no;
            cmd.sub_no = (byte)sub_no;
            cmd.info.cursor = cmd.aa.Info.cursor < 6 || cmd.aa.Info.cursor >= 13 ? (byte)0 : (byte)1;
            if (cmd_no > 48U)
            {
                cmd.info.priority = 1;
            }
            else
            {
                /*int num = (int)*/
                btl_stat.RemoveStatus(cmd.regist, 32768U);
            }
            cmd.info.cover = 0;
            cmd.info.dodge = 0;
            cmd.info.reflec = 0;
            btlDataPtr.bi.cmd_idle = 1;
            EnqueueCommand(cmd);
        }
    }

    public static void CommandEngine(FF9StateBattleSystem btlsys)
    {
        if (btlsys.cur_cmd == null)
        {
            if (btlsys.cmd_queue.next == null)
                return;
            CMD_DATA cmd = btlsys.cmd_queue.next;
            while (cmd != null && (cmd.cmd_no <= 59 && cmd.cmd_no != 56 && (cmd.cmd_no != 58 && Status.checkCurStat(cmd.regist, 33558531U)) || cmd.cmd_no == 57 && Status.checkCurStat(cmd.regist, 256U)))
                cmd = cmd.next;

            if (cmd == null || !FF9StateSystem.Battle.isDebug && !((BattleHUD)(object)UIManager.Battle).IsNativeEnableAtb() && cmd.cmd_no < 48)
                return;
            if (cmd.cmd_no < 55)
            {
                BTL_DATA btl = cmd.regist;
                if (cmd.cmd_no > 48 && cmd.cmd_no < 53)
                {
                    btl_mot.setMotion(btl, 9);
                    btl.evt.animFrame = 0;
                }
                if (btl.bi.player != 0 && !btl_mot.checkMotion(btl, 9) && (!btl_mot.checkMotion(btl, 17) && !Status.checkCurStat(btl, 1073741824U)))
                {
                    if (!btl_mot.checkMotion(btl, btl.bi.def_idle) || btl.bi.cmd_idle != 0)
                        return;
                    btl_mot.setMotion(btl, 9);
                    btl.evt.animFrame = 0;
                    return;
                }
                if (Status.checkCurStat(btl, 16777216U))
                {
                    /*int num = (int)*/
                    btl_stat.AlterStatus(btl, 256U);
                    return;
                }
            }
            btlsys.cur_cmd = cmd;
            KillCommand(cmd);
        }
        CMD_DATA cmd1 = btlsys.cur_cmd;
        if (cmd1 == null)
            return;
        BTL_DATA btl1 = cmd1.regist;
        switch (btlsys.cmd_mode)
        {
            case 0:
                if (!CheckCommandCondition(btlsys, cmd1) || !CheckTargetCondition(btlsys, cmd1) || !CheckMpCondition(cmd1))
                {
                    ResetItemCount(cmd1);
                    if (btl1 != null && btl1.bi.player != 0 && btl_mot.checkMotion(btl1, 9))
                        btl_mot.setMotion(btl1, 32);
                    btlsys.cmd_mode = 3;
                    break;
                }
                if (btl1 != null && !btl_mot.checkMotion(btl1, 9) && cmd1.cmd_no < 47)
                {
                    btl_mot.setMotion(btl1, 9);
                    btl1.evt.animFrame = 0;
                }
                ++btlsys.cmd_mode;
                break;
            case 1:
                btl_vfx.SelectCommandVfx(cmd1);
                ++btlsys.cmd_mode;
                break;
            case 2:
                if (btl1.bi.player != 0 || cmd1.info.mon_reflec != 0)
                {
                    CheckCommandLoop(cmd1);
                }
                break;
            case 3:
                if (cmd1.cmd_no != 56)
                {
                    ReqFinishCommand();
                    break;
                }
                FinishCommand(btlsys);
                break;
        }
        if (!btl_mot.ControlDamageMotion(cmd1) || btlsys.cmd_mode != 4)
            return;
        FinishCommand(btlsys);
    }

    public static void KillCommand(CMD_DATA cmd)
    {
        BTL_DATA btlData = cmd.regist;
        for (CMD_DATA cmd1 = FF9StateSystem.Battle.FF9Battle.cmd_queue; cmd1 != null; cmd1 = cmd1.next)
        {
            if (cmd1.next == cmd)
            {
                if (btlData != null && cmd != btlData.cmd[3] && cmd != btlData.cmd[4])
                    btlData.bi.cmd_idle = 0;
                DequeueCommand(cmd1, false);
                break;
            }
        }
    }

    public static bool KillCommand2(BTL_DATA btl)
    {
        bool flag = false;
        btl.bi.cmd_idle = 0;
        CMD_DATA cmd1 = FF9StateSystem.Battle.FF9Battle.cmd_queue;
        while (cmd1 != null)
        {
            CMD_DATA cmd2 = cmd1.next;
            if (cmd2 != null && cmd2.regist == btl && cmd2.cmd_no < 54)
            {
                if (cmd2.cmd_no < 48)
                    flag = true;
                ResetItemCount(cmd2);
                DequeueCommand(cmd1, true);
            }
            else
                cmd1 = cmd2;
        }
        return flag;
    }

    private static void ResetItemCount(CMD_DATA cmd)
    {
        if (cmd.cmd_no != 14 && cmd.cmd_no != 15 && cmd.cmd_no != 51)
            return;
        UIManager.Battle.ItemUnuse(cmd.sub_no);
    }

    private static void DequeueCommand(CMD_DATA cmd, bool escape_check)
    {
        CMD_DATA cmdData = cmd.next;
        cmd.next = cmdData.next;
        cmdData.info.stat = 0;
        cmdData.info.priority = 0;
        if (!escape_check || cmdData.cmd_no != 56)
            return;
        FF9StateSystem.Battle.FF9Battle.cmd_status &= 65534;
    }

    public static bool CheckSpecificCommand(BTL_DATA btl, byte cmd_no)
    {
        for (CMD_DATA cmdData = FF9StateSystem.Battle.FF9Battle.cmd_queue; cmdData != null; cmdData = cmdData.next)
        {
            if (cmdData.regist == btl && cmdData.cmd_no == cmd_no)
                return true;
        }
        CMD_DATA curCmdPtr = btl_util.getCurCmdPtr();
        return curCmdPtr != null && curCmdPtr.cmd_no == cmd_no;
    }

    public static bool CheckSpecificCommand2(byte cmd_no)
    {
        bool flag = false;
        for (CMD_DATA cmdData = FF9StateSystem.Battle.FF9Battle.cmd_queue; cmdData != null; cmdData = cmdData.next)
        {
            if (cmdData.cmd_no == cmd_no)
                flag = true;
        }
        CMD_DATA curCmdPtr = btl_util.getCurCmdPtr();
        if (curCmdPtr != null && curCmdPtr.cmd_no == cmd_no)
            flag = true;
        return flag;
    }

    public static bool CheckUsingCommand(CMD_DATA cmd)
    {
        for (CMD_DATA cmdData = FF9StateSystem.Battle.FF9Battle.cmd_queue; cmdData != null; cmdData = cmdData.next)
        {
            if (cmd == cmdData)
                return true;
        }
        CMD_DATA curCmdPtr = btl_util.getCurCmdPtr();
        return curCmdPtr != null && curCmdPtr == cmd;
    }

    public static void EnqueueCommand(CMD_DATA cmd)
    {
        for (CMD_DATA cp = FF9StateSystem.Battle.FF9Battle.cmd_queue; cp != null; cp = cp.next)
        {
            if (cmd.info.priority != 0 && cp.next != null)
            {
                if (cmd.cmd_no > 59 || cmd.cmd_no == 54)
                {
                    if (cp.next.cmd_no < 60 && cp.next.cmd_no != 54)
                    {
                        InsertCommand(cmd, cp);
                        break;
                    }
                }
                else if (cmd.cmd_no == 59)
                {
                    if (cp.next.cmd_no < 59 && cp.next.cmd_no != 54)
                    {
                        InsertCommand(cmd, cp);
                        break;
                    }
                }
                else if (cp.next.info.priority == 0)
                {
                    InsertCommand(cmd, cp);
                    break;
                }
            }
            else if (cp.next == null)
            {
                InsertCommand(cmd, cp);
                break;
            }
        }
    }

    public static void InsertCommand(CMD_DATA cmd, CMD_DATA cp)
    {
        if (cmd == cp)
            return;
        cmd.next = cp.next;
        cp.next = cmd;
        cmd.info.stat = 1;
    }

    public static ushort CheckReflec(CMD_DATA cmd)
    {
        ushort num1 = 0;
        if (cmd.cmd_no != 14 && cmd.cmd_no != 15 && (cmd.cmd_no != 51 && (cmd.aa.Category & 1) != 0) && !HasSupportAbility(cmd.regist, SupportAbility1.ReflectNull))
        {
            uint num2 = cmd.tar_id >= 16 ? 1U : 0U;
            ushort[] numArray = new ushort[4];
            short num3;
            short num4 = num3 = 0;
            for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
            {
                if ((btl.btl_id & cmd.tar_id) != 0)
                {
                    if (!Status.checkCurStat(btl, 1U) && btl_stat.CheckStatus(btl, 536870912U))
                    {
                        num1 |= btl.btl_id;
                        ++num4;
                    }
                }
                else if (!Status.checkCurStat(btl, 256U) && btl.bi.player == (int)num2 && btl.bi.target != 0)
                    numArray[num3++] = btl.btl_id;
            }
            if (num4 != 0 && num3 != 0)
            {
                for (int index = 0; index < 4; ++index)
                    cmd.regist.reflec.tar_id[index] = index >= num4 ? (ushort)0 : numArray[Comn.random8() % num3];
                if (num1 == cmd.tar_id)
                {
                    cmd.info.reflec = 1;
                    cmd.tar_id = MargeReflecTargetID(cmd.regist.reflec);
                }
                else
                {
                    cmd.info.reflec = 2;
                    cmd.tar_id = (ushort)(cmd.tar_id & (uint)~num1);
                }
            }
        }
        return num1;
    }

    public static void KillCommand3(BTL_DATA btl)
    {
        CMD_DATA cmd1 = FF9StateSystem.Battle.FF9Battle.cmd_queue;
        while (cmd1 != null)
        {
            CMD_DATA cmd2 = cmd1.next;
            if (cmd2 != null && cmd2.regist == btl)
            {
                ResetItemCount(cmd2);
                DequeueCommand(cmd1, true);
            }
            else
                cmd1 = cmd2;
        }
    }

    public static void KillSpecificCommand(BTL_DATA btl, byte cmd_no)
    {
        for (CMD_DATA cmd = FF9StateSystem.Battle.FF9Battle.cmd_queue; cmd != null; cmd = cmd.next)
        {
            if (cmd.next != null && cmd.next.regist == btl && cmd.next.cmd_no == cmd_no)
            {
                DequeueCommand(cmd, true);
                break;
            }
        }
    }

    // ReSharper disable PossibleNullReferenceException
    public static bool CheckCommandCondition(FF9StateBattleSystem btlsys, CMD_DATA cmd)
    {
        BTL_DATA btlData = cmd.regist;
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        if (btlData != null)
        {
            if (cmd.cmd_no < 55)
            {
                int num = btl_mot.setDirection(btlData);
                btlData.evt.rotBattle.eulerAngles = new Vector3(btlData.evt.rotBattle.eulerAngles.x, num, btlData.evt.rotBattle.eulerAngles.z);
                btlData.rot.eulerAngles = new Vector3(btlData.rot.eulerAngles.x, num, btlData.rot.eulerAngles.z);
            }
            if (battle.GARNET_DEPRESS_FLAG != 0 && btlData.bi.player != 0 && (btlData.bi.slot_no == 2 && cmd.cmd_no < 48) && Comn.random8() < 64)
            {
                UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.DaggerCannotConcentrate);
                return false;
            }
            if (!CheckMagicCondition(cmd))
                return false;
        }
        byte num1 = cmd.cmd_no;
        switch (num1)
        {
            case 11:
                btlData.cmd[3].tar_id = btl_util.GetStatusBtlID(1U, 0U);
                break;
            case 12:
                label_12:
                /*int num2 = (int)*/
                btl_stat.AlterStatus(btlData, 1073741824U);
                btlData.cmd[3].cmd_no = cmd.cmd_no;
                btlData.cmd[3].tar_id = cmd.cmd_no != 3 ? btl_util.GetStatusBtlID(1U, 0U) : cmd.tar_id;
                cmd.tar_id = btlData.btl_id;
                break;
            case 14:
            case 15:
                label_14:
                if (ff9item.FF9Item_GetCount(cmd.sub_no) == 0)
                {
                    UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.NotEnoughItems);
                    return false;
                }
                if (cmd.cmd_no == 51)
                {
                    UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.AutoPotion);
                }
                break;
            case 16:
            case 18:
            case 20:
                DecideSummonType(cmd);
                break;
            case 22:
            case 23:
                if (cmd.sub_no == 46)
                    DecideMeteor(cmd);
                break;
            case 24:
                if (cmd.sub_no == 82)
                {
                    cmd.tar_id = btl_util.GetRandomBtlID((uint)(Comn.random8() & 1));
                    break;
                }
                if (cmd.sub_no == 93 && ff9item.FF9Item_GetCount(cmd.aa.Ref.power) < btl_util.SumOfTarget(1U))
                {
                    UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.NotEnoughItems);
                    return false;
                }
                break;
            case 28:
            case 29:
                if (cmd.sub_no == 126 || cmd.sub_no == 134)
                {
                    uint num3 = cmd.aa.Ref.power * (uint)btlData.level;
                    if (num3 > ff9StateGlobal.party.gil)
                    {
                        UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.NotEnoughGil);
                        return false;
                    }
                    ff9StateGlobal.party.gil -= num3;
                    break;
                }
                if (cmd.sub_no == 129 || cmd.sub_no == 137)
                {
                    cmd.aa.Ref.attr = (byte)(1 << Comn.random8() % 8);
                }
                break;
            case 31:
                return DecideMagicSword(btlData, cmd.aa.MP);
            default:
                switch (num1)
                {
                    case 49:
                        BTL_DATA btlDataPtr1;
                        if ((btlDataPtr1 = btl_scrp.GetBtlDataPtr(cmd.tar_id)) == null || btlDataPtr1.bi.target == 0 || Status.checkCurStat(btlDataPtr1, 256U))
                            return false;
                        if (btlsys.btl_scene.Info.NoNeighboring != 0 && (btlData.weapon.category & 1) != 0)
                        {
                            UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.CannotReach);
                            return false;
                        }
                        if (cmd.cmd_no == 49)
                        {
                            UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.CounterAttack);
                        }
                        break;
                    case 50:
                        BTL_DATA btlDataPtr2;
                        if ((btlDataPtr2 = btl_scrp.GetBtlDataPtr(cmd.tar_id)) == null || btlDataPtr2.bi.target == 0 || Status.checkCurStat(btlDataPtr2, 256U))
                            return false;
                        UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.ReturnMagic);
                        break;
                    case 51:
                        goto label_14;
                    case 52:
                        if (btlsys.btl_scene.Info.NoNeighboring != 0 && (btlData.weapon.category & 1) != 0)
                        {
                            UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.CannotReach);
                            return false;
                        }
                        if (cmd.cmd_no == 49)
                        {
                            UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.CounterAttack);
                        }
                        break;
                    case 56:
                        if (btlsys.btl_phase == 4)
                        {
                            for (BTL_DATA btl = btlsys.btl_list.next; btl != null; btl = btl.next)
                            {
                                if (btl.bi.player != 0 && !Status.checkCurStat(btl, 1107431747U) && btl.cur.hp > 0)
                                {
                                    if (!btl_mot.checkMotion(btl, 17))
                                    {
                                        btl_mot.setMotion(btl, 17);
                                        btl.evt.animFrame = 0;
                                    }
                                    btlsys.btl_phase = 5;
                                    btlsys.btl_seq = 3;
                                }
                            }
                            if (btlsys.btl_phase == 5 && btlsys.btl_seq == 3)
                            {
                                UIManager.Battle.SetIdle();
                                ++ff9StateGlobal.party.escape_no;
                                if (cmd.sub_no == 0)
                                    ff9StateGlobal.btl_flag |= 4;
                                KillAllCommand(btlsys);
                            }
                            else
                                btlsys.cmd_status &= 65534;
                            return false;
                        }
                        break;
                    case 57:
                        if (cmd.sub_no != btlsys.phantom_no)
                        {
                            cmd.sub_no = btlsys.phantom_no;
                            cmd.aa = btlsys.aa_data[cmd.sub_no];
                        }
                        break;
                    case 59:
                        if (Status.checkCurStat(btlData, 16384U))
                        {
                            UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.Trance);
                            btlData.dms_geo_id = btl_init.GetModelID(btl_util.getSerialNumber(btlData) + 19);
                        }
                        else
                            btlData.dms_geo_id = btl_init.GetModelID(btl_util.getSerialNumber(btlData));
                        return true;
                    case 60:
                        btl_sys.CheckBattleMenuOff(btlData);
                        if (btlData.die_seq == 0)
                            btlData.die_seq = btlData.bi.player == 0 ? btlData.bi.slave != 0 || btlData.bi.death_f == 0 ? (byte)1 : (byte)3 : (!btl_mot.checkMotion(btlData, 4) ? (byte)1 : (byte)5);
                        return false;
                    case 61:
                        btlData.cur.hp = 1;
                        /*int num4 = (int)*/
                        btl_stat.RemoveStatus(btlData, 256U);
                        btlData.bi.dmg_mot_f = 1;
                        FF9StateSystem.Settings.SetHPFull();
                        return false;
                    case 62:
                        btl_stat.StatusCommandCancel(btlData, 1U);
                        btlData.stat.cur |= 1U;
                        btlData.bi.atb = 0;
                        btl_sys.CheckBattlePhase(btlData);
                        /*int num5 = (int)*/
                        btl_stat.RemoveStatus(btlData, 2147483648U);
                        btl_stat.SetStatusClut(btlData, true);
                        return false;
                    default:
                        switch (num1)
                        {
                            case 1:
                                if (btlsys.btl_scene.Info.NoNeighboring != 0 && (btlData.weapon.category & 1) != 0 && cmd.tar_id > 15)
                                {
                                    UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.CannotReach);
                                    return false;
                                }
                                break;
                            case 3:
                                goto label_12;
                        }
                        break;
                }
                break;
        }
        return true;
    }

    // ReSharper restore PossibleNullReferenceException

    public static bool CheckMagicCondition(CMD_DATA cmd)
    {
        if (!Status.checkCurStat(cmd.regist, 8U) || (cmd.aa.Category & 2) == 0)
            return true;
        UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.CannotCast);
        return false;
    }

    public static void KillNormalCommand(FF9StateBattleSystem btlsys)
    {
        CMD_DATA cp = btlsys.cmd_queue;
        while (cp != null)
        {
            CMD_DATA ncp = cp.next;
            if (ncp != null && ncp.cmd_no != 54 && ncp.cmd_no < 58)
                ManageDequeueCommand(cp, ncp);
            else
                cp = ncp;
        }
    }

    public static void KillAllCommand(FF9StateBattleSystem btlsys)
    {
        CMD_DATA cp = btlsys.cmd_queue;
        while (cp != null)
        {
            CMD_DATA ncp = cp.next;
            if (ncp != null && ncp.cmd_no < 59)
                ManageDequeueCommand(cp, ncp);
            else
                cp = ncp;
        }
        btlsys.cmd_status &= 65523;
        btlsys.phantom_no = (byte)(btlsys.phantom_cnt = 0);
    }

    public static void ClearSysPhantom(BTL_DATA btl)
    {
        if (btl.bi.player == 0 || btl.bi.slot_no != 2)
            return;
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        KillSpecificCommand(btl, 57);
        stateBattleSystem.cmd_status &= 65523;
        stateBattleSystem.phantom_no = 0;
        stateBattleSystem.phantom_cnt = 0;
    }

    public static void ManageDequeueCommand(CMD_DATA cp, CMD_DATA ncp)
    {
        if (cp == null)
            return;
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        BTL_DATA btl = ncp.regist;
        if (ncp.cmd_no < 48 && btl != null && !Status.checkCurStat(btl, 256U))
        {
            btl.sel_mode = 0;
            if (btl_mot.checkMotion(btl, 9) && (btl.bi.slot_no != 6 || stateBattleSystem.cur_cmd != null && stateBattleSystem.cur_cmd.cmd_no != 21) && (btl.bi.slot_no != 1 || stateBattleSystem.cur_cmd != null && stateBattleSystem.cur_cmd.cmd_no != 31 && stateBattleSystem.cur_cmd.cmd_no != 23))
            {
                btl_mot.setMotion(btl, 32);
                btl.evt.animFrame = 0;
                btl.bi.cmd_idle = 0;
            }
        }
        ResetItemCount(ncp);
        DequeueCommand(cp, true);
    }

    private static bool CheckMpCondition(CMD_DATA cmd)
    {
        short mp = cmd.aa.MP;
        if (battle.GARNET_SUMMON_FLAG != 0 && (cmd.aa.Type & 4) != 0)
            mp *= 4;
        if (cmd.cmd_no == 50 || ConsumeMp(cmd.regist, mp))
            return true;
        UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.NotEnoughMp);
        return false;
    }

    private static bool CheckTargetCondition(FF9StateBattleSystem btlsys, CMD_DATA cmd)
    {
        ushort num1 = 0;
        if (cmd.tar_id == 0)
            return false;
        ushort num2;
        switch (cmd.cmd_no)
        {
            case 14:
            case 51:
                num2 = ff9item._FF9Item_Info[btl_util.btlItemNum(cmd.sub_no)].info.dead;
                break;
            case 59:
                return true;
            default:
                num2 = cmd.aa.Info.dead;
                break;
        }
        for (BTL_DATA btl = btlsys.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.bi.target != 0 && (btl.btl_id & cmd.tar_id) != 0 && (num2 != 0 && btl.bi.player != 0 || !Status.checkCurStat(btl, 256U)))
                num1 |= btl.btl_id;
        }
        if (num1 != 0)
        {
            cmd.tar_id = num1;
            return true;
        }
        if (cmd.info.cursor == 0 && num2 == 0)
        {
            cmd.tar_id = btl_util.GetRandomBtlID(cmd.tar_id & 15U);
            if (cmd.tar_id != 0)
                return true;
        }
        return false;
    }

    public static void ReqFinishCommand()
    {
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        CMD_DATA cmd = stateBattleSystem.cur_cmd;
        if (cmd.info.reflec == 2)
        {
            cmd.tar_id = MargeReflecTargetID(cmd.regist.reflec);
            btl_vfx.SetBattleVfx(cmd, (uint)cmd.aa.Info.vfx_no, null);
            stateBattleSystem.cmd_mode = 2;
        }
        else
        {
            if (Status.checkCurStat(cmd.regist, 8192U) && cmd.cmd_no == 60 && !btl_mot.checkMotion(cmd.regist, 4))
                return;
            stateBattleSystem.cmd_mode = 4;
        }
    }

    private static void FinishCommand(FF9StateBattleSystem btlsys)
    {
        CMD_DATA cmd = btlsys.cur_cmd;
        BTL_DATA btl1 = cmd.regist;
        if (cmd.cmd_no < 48)
        {
            if (cmd == btl1.cmd[0] && cmd.cmd_no != 3 && cmd.cmd_no != 12)
            {
                ResetCurrentBattlerActiveTime(btl1);
            }
            else if (cmd.cmd_no == 21 && btl1.bi.slot_no == 6 && (cmd == btl1.cmd[3] && !CheckUsingCommand(btl1.cmd[0])))
            {
                ResetCurrentBattlerActiveTime(btl1);
            }
            else if (cmd.cmd_no == 10)
            {
                /*int num = (int)*/
                btl_stat.RemoveStatus(btl1, 1073741824U);
                FF9StateSystem.Battle.FF9Battle.cmd_status &= 65519;
            }
            else if (cmd.cmd_no == 11)
            {
                /*int num = (int)*/
                btl_stat.AlterStatus(btl1, 1073741824U);
                btl1.tar_mode = 2;
                btl1.SetDisappear(1);
                FF9StateSystem.Battle.FF9Battle.cmd_status &= 65519;
            }
            if (Status.checkCurStat(btl1, 16384U) && cmd.cmd_no != 3 && cmd.cmd_no != 12)
            {
                byte num1 = (byte)((300 - btl1.level) / btl1.elem.wpr * 10);
                if (cmd.cmd_no == 21 || cmd.cmd_no == 23)
                    num1 /= 2;
                if (FF9StateSystem.Settings.IsTranceFull)
                    num1 = 0;
                if (btl1.trance > num1)
                    btl1.trance -= num1;
                else if (!FF9StateSystem.Battle.isDebug)
                {
                    /*int num2 = (int)*/
                    btl_stat.RemoveStatus(btl1, 16384U);
                }
                if (cmd.cmd_no == 18 && btlsys.phantom_no != 0)
                {
                    btlsys.cmd_status |= 4;
                    btlsys.phantom_cnt = setPhantomCount(btl1);
                }
            }
        }
        else if (cmd.cmd_no == 57)
        {
            btlsys.cmd_status &= 65527;
            btlsys.phantom_cnt = setPhantomCount(btl1);
        }
        else if (cmd.cmd_no < 53 && CheckUsingCommand(btl1.cmd[0]))
            btl1.bi.cmd_idle = 1;
        if (cmd.info.cover != 0)
        {
            for (BTL_DATA btl2 = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl2 != null; btl2 = btl2.next)
            {
                if (btl2.bi.player != 0)
                {
                    btl_mot.setBasePos(btl2);
                    if (btl2.bi.cover != 0)
                    {
                        btl2.bi.cover = 0;
                        btl_mot.SetDefaultIdle(btl2);
                    }
                }
            }
        }
        if (cmd.info.dodge != 0)
        {
            BTL_DATA btl2 = btl_scrp.GetBtlDataPtr(cmd.tar_id);
            btl2.bi.dodge = 0;
            if (btl2.bi.player != 0)
                btl_mot.SetDefaultIdle(btl2);
            else if (btl2.bi.slave != 0)
                btl2 = btl_util.GetMasterEnemyBtlPtr();
            btl2.pos[2] = btl2.base_pos[2];
        }
        if (cmd.regist != null && cmd == btl1.cmd[0])
            UIManager.Battle.RemovePlayerFromAction(cmd.regist.btl_id, true);
        ClearCommand(cmd);
        btlsys.cur_cmd = null;
        btlsys.cmd_mode = 0;
        if (btl1 != null && btl1.bi.player != 0 && FF9StateSystem.Settings.IsATBFull)
            btl1.cur.at = (short)(btl1.max.at - 1);
        if (FF9StateSystem.Battle.isDebug)
            return;
        HonoluluBattleMain.playerEnterCommand = true;
    }

    private static void ResetCurrentBattlerActiveTime(BTL_DATA btl1)
    {
        if (Configuration.Fixes.IsKeepRestTimeInBattle && btl1.max.at > 0)
            btl1.cur.at = (short)Math.Max(0, btl1.cur.at - btl1.max.at);
        else
            btl1.cur.at = 0;

        btl1.sel_mode = 0;
    }

    public static ushort MargeReflecTargetID(REFLEC_DATA reflec)
    {
        int index = 0;
        ushort num = 0;
        for (; index < 4; ++index)
            num |= reflec.tar_id[index];
        return num;
    }

    public static void CheckCommandLoop(CMD_DATA cmd)
    {
        if (!SFX.isRunning)
        {
            BTL_DATA btlData = cmd.regist;
            if (!FF9StateSystem.Battle.isDebug && (UIManager.Battle.BtlWorkLibra || UIManager.Battle.BtlWorkPeep))
                return;
            if (cmd.cmd_no == 4)
            {
                if (btl_mot.checkMotion(btlData, 12))
                {
                    if (btlData.evt.animFrame < GeoAnim.geoAnimGetNumFrames(btlData))
                        return;
                    btl_mot.setMotion(btlData, 13);
                    btlData.evt.animFrame = 0;
                }
            }
            else if (cmd.cmd_no == 7)
            {
                if (btl_mot.checkMotion(btlData, 9))
                {
                    if (btlData.evt.animFrame < GeoAnim.geoAnimGetNumFrames(btlData, btlData.currentAnimationName))
                        return;
                    btl_mot.setMotion(btlData, (byte)(29U + btlData.bi.row));
                    btlData.evt.animFrame = 0;
                }
                if (btl_mot.checkMotion(btlData, (byte)(29U + btlData.bi.row)))
                {
                    ushort numFrames = GeoAnim.geoAnimGetNumFrames(btlData);
                    ushort num1 = btlData.evt.animFrame;
                    if (num1 < numFrames)
                    {
                        ushort num2 = (ushort)(num1 + 1U);
                        btlData.pos[2] = btlData.bi.row == 0 ? 400 * num2 / numFrames - 1960 : -400 * num2 / numFrames - 1560;
                        btlData.gameObject.transform.localPosition = btlData.pos;
                        return;
                    }
                    ExecVfxCommand(btlData);
                    btl_mot.setMotion(btlData, 32);
                    btlData.evt.animFrame = 0;
                    return;
                }
                if (btl_mot.checkMotion(btlData, 32))
                {
                    if (btlData.evt.animFrame < GeoAnim.geoAnimGetNumFrames(btlData))
                        return;
                    btl_mot.setMotion(btlData, 0);
                    btlData.evt.animFrame = 0;
                }
            }
            BattleAchievement.UpdateCommandAchievement(cmd);
            ReqFinishCommand();
            if (cmd.cmd_no != 59)
                return;
            btl_stat.SetPresentColor(btlData);
        }
        else
        {
            if (cmd.cmd_no != 59 || SFX.frameIndex != 75)
                return;
            BTL_DATA btl = cmd.regist;
            if (Status.checkCurStat(btl, 16384U))
            {
                btl_vfx.SetTranceModel(btl, true);
                BattleAchievement.UpdateTranceStatus();
            }
            else
                btl_vfx.SetTranceModel(btl, false);
        }
    }

    public static void ExecVfxCommand(BTL_DATA target)
    {
        CMD_DATA curCmdPtr = btl_util.getCurCmdPtr();
        if (curCmdPtr == null)
        {
            Debug.LogError("no command!");
        }
        else
        {
            BTL_DATA caster = curCmdPtr.regist;
            byte num = curCmdPtr.cmd_no;
            switch (num)
            {
                case 12:
                    label_7:
                    caster.tar_mode = 2;
                    caster.SetDisappear(1);
                    break;
                case 14:
                    label_8:
                    UIManager.Battle.ItemUse(curCmdPtr.sub_no);
                    SBattleCalculator.CalcMain(caster, target, new BattleCommand(curCmdPtr), ff9item._FF9Item_Info[btl_util.btlItemNum(curCmdPtr.sub_no)].Ref.prog_no);
                    break;
                case 15:
                    UIManager.Battle.ItemUse(curCmdPtr.sub_no);
                    SBattleCalculator.CalcMain(caster, target, new BattleCommand(curCmdPtr), curCmdPtr.aa.Ref.prog_no);
                    break;
                default:
                    switch (num)
                    {
                        case 49:
                        case 52:
                            label_6:
                            SBattleCalculator.CalcMain(caster, target, new BattleCommand(curCmdPtr), caster.weapon.Ref.prog_no);
                            return;
                        case 51:
                            goto label_8;
                        default:
                            switch (num)
                            {
                                case 1:
                                    goto label_6;
                                case 3:
                                    goto label_7;
                                default:
                                    if (num == 58)
                                    {
                                        ushort battleId = btl_scrp.GetBattleID(1U);
                                        ushort statusBtlId = btl_util.GetStatusBtlID(1U, 4355U);
                                        if (battleId == 0 || battleId == statusBtlId)
                                        {
                                            FF9StateBattleSystem btlsys = FF9StateSystem.Battle.FF9Battle;
                                            UIManager.Battle.FF9BMenu_EnableMenu(false);
                                            if (btlsys.btl_phase != 5)
                                            {
                                                btlsys.btl_phase = 5;
                                                btlsys.btl_seq = 0;
                                                KillAllCommand(btlsys);
                                            }
                                        }
                                        SBattleCalculator.CalcMain(caster, target, new BattleCommand(curCmdPtr), curCmdPtr.aa.Ref.prog_no);
                                        return;
                                    }
                                    SBattleCalculator.CalcMain(caster, target, new BattleCommand(curCmdPtr), curCmdPtr.aa.Ref.prog_no);
                                    return;
                            }
                    }
            }
        }
    }

    public static void DispSelectCursor(FF9StateGlobal sys, FF9StateBattleSystem btlsys, BTL_DATA btl)
    {
        GameObject gameObject = btlsys.s_cur;
        Vector3 localPosition = btl.gameObject.transform.localPosition;
        Vector3 eulerAngles = gameObject.transform.localRotation.eulerAngles;
        gameObject.transform.localPosition = new Vector3(localPosition.x, localPosition.y + btl.height, localPosition.z);
        float num = (float)((((btlsys.btl_cnt & 15) << 8) + 1265) % 4096 / 4096.0 * 360.0);
        gameObject.transform.localRotation = Quaternion.Euler(eulerAngles.x, -num, eulerAngles.z);
    }

    private static void DecideSummonType(CMD_DATA cmd)
    {
        AchievementState achievement = FF9StateSystem.Achievement;
        if (cmd.sub_no == 49 && !achievement.summon_shiva ||
            cmd.sub_no == 51 && !achievement.summon_ifrit ||
            cmd.sub_no == 53 && !achievement.summon_ramuh ||
            cmd.sub_no == 55 && !achievement.summon_atomos ||
            cmd.sub_no == 58 && !achievement.summon_odin ||
            cmd.sub_no == 60 && !achievement.summon_leviathan ||
            cmd.sub_no == 62 && !achievement.summon_bahamut ||
            cmd.sub_no == 64 && !achievement.summon_arc ||
            cmd.sub_no == 68 && !achievement.summon_carbuncle_haste ||
            cmd.sub_no == 69 && !achievement.summon_carbuncle_protect ||
            cmd.sub_no == 70 && !achievement.summon_carbuncle_reflector ||
            cmd.sub_no == 71 && !achievement.summon_carbuncle_shell ||
            cmd.sub_no == 66 && !achievement.summon_fenrir_earth ||
            cmd.sub_no == 67 && achievement.summon_fenrir_wind ||
            cmd.sub_no == 72 && !achievement.summon_phoenix ||
            cmd.sub_no == 74 && !achievement.summon_madeen ||
            HasSupportAbility(cmd.regist, SupportAbility2.Boost))
            return;

        if (cmd.regist.cur.mp > cmd.aa.MP * 2)
        {
            if (Comn.random8() >= 230)
                return;
            cmd.info.short_summon = 1;
        }
        else
        {
            if (Comn.random8() >= 170)
                return;
            cmd.info.short_summon = 1;
        }
    }

    private static bool DecideMagicSword(BTL_DATA steiner, short mp)
    {
        if (steiner.cur.mp >= mp)
        {
            for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
            {
                if (btl_util.getSerialNumber(next) == 2)
                {
                    if (!Status.checkCurStat(next, 318905611U) && !Status.checkCurStat(steiner, 318905611U))
                        return true;
                    break;
                }
            }
        }
        UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.CombinationFailed);
        return false;
    }

    private static void DecideMeteor(CMD_DATA cmd)
    {
        if (cmd.regist.level / 2 + cmd.regist.elem.wpr >= Comn.random16() % 100)
            return;
        cmd.info.meteor_miss = 1;
    }

    private static bool ConsumeMp(BTL_DATA btl, short mp)
    {
        if (btl == null)
            return false;

        BattleUnit unit = new BattleUnit(btl);
        if (unit.HasSupportAbility(SupportAbility2.HalfMP))
            mp /= 2;

        if (unit.CurrentMp < mp)
            return false;

        if (!FF9StateSystem.Battle.isDebug && (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull))
            btl.cur.mp -= mp;

        return true;
    }

    #region Memoria

    public static Boolean HasSupportAbility(BTL_DATA btl, SupportAbility1 ability)
    {
        return (btl.sa[0] & (uint)ability) != 0;
    }

    public static Boolean HasSupportAbility(BTL_DATA btl, SupportAbility2 ability)
    {
        return (btl.sa[1] & (uint)ability) != 0;
    }

    #endregion

}