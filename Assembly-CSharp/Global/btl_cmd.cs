﻿using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Scripts;
using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
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

public class btl_cmd
{
    /* Notes on commands:
    Each BTL_DATA has 6 potential commands at the same time.
    Only the first 3 are initialized for enemies by default (btl_cmd.InitCommand).
    With Memoria: the 6 commands are all used even for enemies (the last 3 are used for btl_scrp.SetCharacterData(id == 114)).
    btl.cmd[0] -> normal commands (including berserk/confuse)
    btl.cmd[1] -> counter-attacks
    btl.cmd[2] -> reserved for death/stone animation (cmd_no == 60/62, sub_no == 0)
    btl.cmd[3] -> first cast of a double-cast command or Spear (saved duplicate) or Eidolon phantom
    btl.cmd[4] -> reserved for trance animation (cmd_no == 59, sub_no == 0)
    btl.cmd[5] -> reserved for reraise animation (cmd_no == 61, sub_no == 0)
    */
    public btl_cmd()
    {
    }

    public static void ClearCommand(CMD_DATA cmd)
    {
        cmd.next = null;
        cmd.aa = null;
        cmd.tar_id = 0;
        cmd.magic_caster_id = 0;
        cmd.cmd_no = BattleCommandId.None;
        cmd.sub_no = 0;
        cmd.info.Reset();
    }

    public static void ClearReflecData(CMD_DATA cmd)
    {
        for (Int32 index = 0; index < 4; ++index)
            cmd.reflec.tar_id[index] = 0;
    }

    public static void InitCommandSystem(FF9StateBattleSystem btlsys)
    {
        btlsys.cur_cmd_list.Clear();
        btlsys.cmd_status = 2;
        btlsys.cmd_queue.regist = btlsys.cmd_escape.regist = null;
        ClearCommand(btlsys.cmd_queue);
        ClearCommand(btlsys.cmd_escape);
        btl_cmd.next_cmd_delay = 0;
    }

    public static void InitSelectCursor(FF9StateBattleSystem btlsys)
    {
        UInt32 num1 = (UInt32)((FF9StateSystem.Battle.FF9Battle.btl_cnt & 15) << 8);
        List<Vector3> vector3List1 = new List<Vector3>();
        List<Vector3> vector3List2 = new List<Vector3>();
        List<Color32> color32List = new List<Color32>();
        List<Int32> intList = new List<Int32>();
        Vector3 vector3_1 = new Vector3(0.0f, 0.0f, 0.0f);
        vector3List1.Add(vector3_1);
        Int32 num2 = 0;
        while (num2 < 3)
        {
            Single f = (Single)(num1 / 4096.0 * 360.0);
            Vector3 vector3_2 = new Vector3(vector3_1.x - (Int32)(68.0 * Mathf.Cos(f)), vector3_1.y - 133f, vector3_1.z - (Int32)(68.0 * Mathf.Sin(f)));
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
        for (Int32 index = 0; index < vector3List2.Count; ++index)
        {
            Byte num3 = (Byte)Mathf.Floor(index / 3f);
            Byte num4 = index >= 9 ? (Byte)160 : (Byte)(188 - 60 * num3);
            color32List.Add(new Color32(num4, num4, 0, Byte.MaxValue));
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
        Material material = new Material(ShadersLoader.Find("PSX/BattleMap_SelectCursor_Abr_1"));
        meshRenderer.material = material;
        gameObject.SetActive(false);
    }

    public static void InitCommand(BTL_DATA btl)
    {
        FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        if (stateBattleSystem.btl_phase == FF9StateBattleSystem.PHASE_ENTER)
            btl.tar_mode = 1;
        btl.sel_mode = 0;
        btl.finger_disp = false;
        //int index1 = btl.bi.player == 0 ? 24 + (btl.bi.line_no - 4) * 3 : btl.bi.line_no * 6;
        //CMD_DATA cmdData = stateBattleSystem.cmd_buffer[index1];
        for (Int32 i = 0; i < 6; ++i)
        {
            btl.cmd[i] = new CMD_DATA { regist = btl };
            ClearCommand(btl.cmd[i]);
            ClearReflecData(btl.cmd[i]);
        }
    }

    /// <summary>Enqueue a command for a player character</summary>
    public static void SetCommand(CMD_DATA cmd, BattleCommandId commandId, Int32 sub_no, UInt16 tar_id, UInt32 cursor, Boolean forcePriority = false, BattleCommandMenu cmdMenu = BattleCommandMenu.None)
    {
        if (btl_cmd.CheckUsingCommand(cmd))
            return;
        cmd.cmd_no = commandId;
        cmd.sub_no = sub_no;
        BTL_DATA caster = cmd.regist;
        switch (commandId)
        {
            case BattleCommandId.SysEscape:
                if ((FF9StateSystem.Battle.FF9Battle.cmd_status & 1) != 0)
                    return;
                FF9StateSystem.Battle.FF9Battle.cmd_status |= 1;
                break;
        }
        cmd.info.cmdMenu = cmdMenu;
        cmd.SetAAData(btl_util.GetCommandAction(cmd));
        cmd.ScriptId = btl_util.GetCommandScriptId(cmd);
        // ScriptId 80: Double cast with AA specified by "Power" and "Rate"
        if (caster != null && caster.bi.player != 0 && cmd.ScriptId == 80)
        {
            AA_DATA first_aa = FF9StateSystem.Battle.FF9Battle.aa_data[(BattleAbilityId)cmd.Power];
            AA_DATA second_aa = FF9StateSystem.Battle.FF9Battle.aa_data[(BattleAbilityId)cmd.HitRate];
            UInt16 first_tar_id;
            UInt16 second_tar_id;
            if (first_aa.Info.Target == TargetType.AllAlly || first_aa.Info.Target == TargetType.RandomAlly)
            {
                first_tar_id = 0xF;
                second_tar_id = tar_id;
            }
            else if (first_aa.Info.Target == TargetType.AllEnemy || first_aa.Info.Target == TargetType.RandomEnemy)
            {
                first_tar_id = 0xF0;
                second_tar_id = tar_id;
            }
            else if (first_aa.Info.Target == TargetType.Everyone)
            {
                first_tar_id = 0xFF;
                second_tar_id = tar_id;
            }
            else if (first_aa.Info.Target == TargetType.Self)
            {
                first_tar_id = caster.btl_id;
                second_tar_id = tar_id;
            }
            else if (second_aa.Info.Target == TargetType.AllAlly || second_aa.Info.Target == TargetType.RandomAlly)
            {
                first_tar_id = tar_id;
                second_tar_id = 0xF;
            }
            else if (second_aa.Info.Target == TargetType.AllEnemy || second_aa.Info.Target == TargetType.RandomEnemy)
            {
                first_tar_id = tar_id;
                second_tar_id = 0xF0;
            }
            else if (second_aa.Info.Target == TargetType.Everyone)
            {
                first_tar_id = tar_id;
                second_tar_id = 0xFF;
            }
            else if (second_aa.Info.Target == TargetType.Self)
            {
                first_tar_id = tar_id;
                second_tar_id = caster.btl_id;
            }
            else
            {
                if (!first_aa.Info.DefaultAlly && (tar_id & 0xF0) == tar_id)
                {
                    first_tar_id = tar_id;
                    if (!second_aa.Info.DefaultAlly)
                        second_tar_id = 0xF0;
                    else
                        second_tar_id = 0xF;
                }
                else
                {
                    if (!first_aa.Info.DefaultAlly)
                        first_tar_id = 0xF0;
                    else
                        first_tar_id = 0xF;
                    second_tar_id = tar_id;
                }
            }
            UInt32 first_cursor = Comn.countBits(first_tar_id) > 1 ? 1u : 0u;
            UInt32 second_cursor = Comn.countBits(second_tar_id) > 1 ? 1u : 0u;
            CMD_DATA first_cmd = cmd.regist.cmd[3];
            CMD_DATA second_cmd = cmd.regist.cmd[0];
            first_cmd.regist.sel_mode = 1;
            first_cmd.info.CustomMPCost = cmd.aa.MP;
            second_cmd.info.CustomMPCost = 0;
            btl_cmd.SetCommand(first_cmd, commandId, cmd.Power, first_tar_id, first_cursor, cmdMenu: cmdMenu);
            btl_cmd.SetCommand(second_cmd, commandId, cmd.HitRate, second_tar_id, second_cursor, cmdMenu: cmdMenu);
            return;
        }

        BattleAbilityId aaIndex = btl_util.GetCommandMainActionIndex(cmd);
        if (caster != null && aaIndex != BattleAbilityId.Void)
        {
            BattleMagicSwordSet magicSet = UIManager.Battle.GetMagicSwordOfAbility(new BattleUnit(caster), ff9abil.GetAbilityIdFromActiveAbility(aaIndex));
            if (magicSet != null)
                cmd.magic_caster_id = BattleState.GetPlayerUnit(magicSet.Supporter)?.Id ?? 0;
        }
        cmd.tar_id = tar_id;
        cmd.info.cursor = (Byte)cursor;
        cmd.info.cover = 0;
        cmd.info.dodge = 0;
        cmd.info.reflec = 0;
        cmd.IsShortRange = btl_util.IsAttackShortRange(cmd);
        if (!btl_util.IsCommandDeclarable(commandId))
            cmd.info.priority = 1;
        if ((commandId < BattleCommandId.EnemyReaction || commandId > BattleCommandId.BoundaryUpperCheck) && cmd != caster.cmd[3]) // cmd != caster.cmd[3] => Prevent cancel animation with Double Cast.
        {
            //if (btl_util.getCurCmdPtr() != btl.cmd[4])
            //{
            //    if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL))
            //    {
            //        btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_NORMAL_TO_CMD);
            //        btl.evt.animFrame = 0;
            //    }
            //    else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING))
            //    {
            //        btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DYING_TO_CMD);
            //        btl.evt.animFrame = 0;
            //    }
            //    else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE) && commandId < BattleCommandId.BoundaryCheck)
            //    {
            //        btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DEF_TO_IDLE);
            //        btl.evt.animFrame = 0;
            //    }
            //}
            caster.bi.cmd_idle = 1;
            if (Configuration.Battle.Speed < 3 && caster != null && caster.bi.player != 0)
                btl_mot.SetDefaultIdle(caster); // Don't wait for the "Idle" animation to finish its cycle to get ready
        }
        if (commandId == BattleCommandId.SummonGarnet || commandId == BattleCommandId.Phantom || commandId == BattleCommandId.SummonEiko)
            caster.summon_count++;
        BattleAbilityHelper.SetCustomPriority(cmd);
        if (forcePriority)
            cmd.info.priority = 1;
        if (caster != null && cmd == caster.cmd[0])
        {
            btl_stat.RemoveStatuses(new BattleUnit(caster), BattleStatusConst.RemoveOnMainCommand);
            BattleVoice.TriggerOnBattleAct(caster, "CommandInput", cmd);
        }
        EnqueueCommand(cmd);
    }

    public static UInt16 GetRandomTargetForCommand(BTL_DATA caster, BattleCommandId commandId, Int32 subNo)
    {
        CMD_DATA testCmd = new CMD_DATA
        {
            regist = caster,
            cmd_no = commandId,
            sub_no = subNo
        };
        AA_DATA aaData = btl_util.GetCommandAction(testCmd);
        if (aaData.Info.Target == TargetType.All)
            return 0xFF;
        if (aaData.Info.Target == TargetType.Self)
            return caster.btl_id;
        UInt16 targetId = (UInt16)(aaData.Info.DefaultAlly ? 0x0F : 0xF0);
        if (!aaData.Info.ForDead)
            for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
                if ((btl.btl_id & targetId) != 0 && btl_stat.CheckStatus(btl, BattleStatus.Death))
                    targetId &= (UInt16)~btl.btl_id;
        if (aaData.Info.Target != TargetType.All && aaData.Info.Target != TargetType.AllAlly && aaData.Info.Target != TargetType.AllEnemy)
            targetId = (UInt16)Comn.randomID(targetId);
        return targetId;
    }

    public static void SetCounter(BTL_DATA btl, BattleCommandId commandId, Int32 sub_no, UInt16 tar_id)
    {
        if (btl_stat.CheckStatus(btl, BattleStatusConst.PreventCounter) || FF9StateSystem.Battle.FF9Battle.btl_phase != FF9StateBattleSystem.PHASE_NORMAL)
            return;
        if (btl_util.IsCommandMonsterTransformAttack(btl, commandId, sub_no) && btl.monster_transform.attack[btl.bi.def_idle] == null)
            return;
        SetCommand(btl.cmd[1], commandId, sub_no, tar_id, Comn.countBits(tar_id) > 1 ? 1u : 0u, true);
    }

    /// <summary>Enqueue a command for an enemy unit</summary>
    public static void SetEnemyCommand(BattleUnit caster, BattleCommandId cmd_no, Int32 sub_no, UInt16 tar_id)
    {
        FF9StateBattleSystem battleState = FF9StateSystem.Battle.FF9Battle;
        if ((battleState.cmd_status & 1) != 0 || caster.IsUnderAnyStatus(BattleStatusConst.PreventEnemyCmd))
        {
            caster.Data.sel_mode = 0;
            return;
        }
        if (btl_para.IsSpecialHPInDyingState(caster) && cmd_no != BattleCommandId.EnemyDying && battleState.btl_phase == FF9StateBattleSystem.PHASE_NORMAL && btl_scrp.GetBattleID(1) == caster.Id)
        {
            // Avoid bosses to keep attacking or counter-attacking under 10000 HP, especially in Speed modes >= 3 (because their AI script will not enter the ending phase if SFX keep playing)
            caster.Data.sel_mode = 0;
            return;
        }
        CMD_DATA cmd;
        if (cmd_no == BattleCommandId.EnemyAtk)
        {
            if (battleState.btl_phase != FF9StateBattleSystem.PHASE_NORMAL)
            {
                caster.Data.sel_mode = 0;
                return;
            }
            cmd = caster.Data.cmd[0];
        }
        else if (cmd_no == BattleCommandId.EnemyCounter)
            cmd = caster.Data.cmd[1];
        else if (cmd_no == BattleCommandId.EnemyDying)
            cmd = caster.Data.cmd[1];
        else if (cmd_no == BattleCommandId.ScriptCounter1)
        {
            cmd = caster.Data.cmd[3];
            cmd_no = BattleCommandId.EnemyCounter;
        }
        else if (cmd_no == BattleCommandId.ScriptCounter2)
        {
            cmd = caster.Data.cmd[4];
            cmd_no = BattleCommandId.EnemyCounter;
        }
        else if (cmd_no == BattleCommandId.ScriptCounter3)
        {
            cmd = caster.Data.cmd[5];
            cmd_no = BattleCommandId.EnemyCounter;
        }
        else
        {
            caster.Data.sel_mode = 0;
            return;
        }
        if (btl_cmd.CheckUsingCommand(cmd))
        {
            if (cmd_no == BattleCommandId.EnemyAtk)
            {
                caster.Data.sel_mode = 0;
                return;
            }
            else if (cmd_no == BattleCommandId.EnemyDying && cmd.cmd_no != BattleCommandId.EnemyDying)
            {
                while (caster.Data.cmd.Count < 7) // Make sure that EnemyDying is taken into account, even by adding a new command to BTL_DATA
                {
                    cmd = new CMD_DATA { regist = caster.Data };
                    caster.Data.cmd.Add(cmd);
                    ClearCommand(cmd);
                    ClearReflecData(cmd);
                }
                cmd = caster.Data.cmd[6];
            }
            else
            {
                return;
            }
        }
        ClearCommand(cmd);
        ClearReflecData(cmd);
        cmd.cmd_no = cmd_no;
        cmd.sub_no = sub_no;
        cmd.SetAAData(btl_util.GetCommandAction(cmd));
        cmd.ScriptId = btl_util.GetCommandScriptId(cmd);
        cmd.tar_id = tar_id;
        cmd.info.cursor = cmd.aa.Info.Target <= TargetType.ManyEnemy || cmd.aa.Info.Target >= TargetType.Self ? (Byte)0 : (Byte)1;
        cmd.IsShortRange = btl_util.IsAttackShortRange(cmd);
        if (!btl_util.IsCommandDeclarable(cmd_no))
            cmd.info.priority = 1;
        cmd.info.cover = 0;
        cmd.info.dodge = 0;
        cmd.info.reflec = 0;
        caster.Data.bi.cmd_idle = 1;
        if (cmd == caster.Data.cmd[0])
        {
            btl_stat.RemoveStatuses(caster, BattleStatusConst.RemoveOnMainCommand);
            BattleVoice.TriggerOnBattleAct(caster.Data, "CommandInput", cmd);
        }
        EnqueueCommand(cmd);
    }

    public static CMD_DATA GetFirstCommandReadyToDequeue(FF9StateBattleSystem btlsys)
    {
        CMD_DATA cmd = btlsys.cmd_queue.next;
        HashSet<BTL_DATA> busyCasters = new HashSet<BTL_DATA>();
        if (Configuration.Battle.Speed == 4)
            for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                if (btl_util.IsBtlBusy(next, btl_util.BusyMode.ANY_CURRENT))
                    busyCasters.Add(next);
        while (cmd != null)
        {
            if (cmd.regist != null && busyCasters.Contains(cmd.regist) && cmd.cmd_no != BattleCommandId.SysPhantom
                || btl_stat.CheckStatus(cmd.regist, BattleStatusConst.CannotAct) && cmd.cmd_no != BattleCommandId.SysDead && cmd.cmd_no != BattleCommandId.SysReraise && cmd.cmd_no != BattleCommandId.SysStone && cmd.cmd_no != BattleCommandId.SysEscape && cmd.cmd_no != BattleCommandId.SysLastPhoenix
                || btl_stat.CheckStatus(cmd.regist, BattleStatus.Death) && cmd.cmd_no == BattleCommandId.SysPhantom
                || Configuration.Battle.Speed >= 4 && btl_util.IsBtlUsingCommandMotion(cmd.regist)
                || Configuration.Battle.Speed >= 5 && cmd.regist.bi.cover != 0)
            {
                if (Configuration.Battle.Speed == 4)
                {
                    if (cmd.regist != null)
                        busyCasters.Add(cmd.regist);
                    foreach (BTL_DATA next in btl_util.findAllBtlData(cmd.tar_id))
                        busyCasters.Add(next);
                }
                cmd = cmd.next;
                continue;
            }
            break;
        }
        return cmd;
    }

    private static void RunCommandFromQueue(FF9StateBattleSystem btlsys)
    {
        Boolean admitNewCommand = btlsys.cur_cmd == null;
        if (admitNewCommand)
            btl_cmd.next_cmd_delay = 0;
        if (!admitNewCommand && Configuration.Battle.Speed >= 3)
        {
            btl_cmd.next_cmd_delay--;
            if (btl_cmd.next_cmd_delay <= 0)
                admitNewCommand = true;
        }
        if (!admitNewCommand)
            return;
        if (btlsys.cmd_queue.next == null)
            return;
        if (btlsys.cur_cmd_list.Contains(btlsys.cmd_escape))
            return;
        if (Configuration.Battle.Speed < 3)
            for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                if (next.die_seq > 0 && next.die_seq < 6 && next.stat.HasDeathChangerEffect)
                    return;

        CMD_DATA cmd = GetFirstCommandReadyToDequeue(btlsys);
        if (cmd == null || !FF9StateSystem.Battle.isDebug && !UIManager.Battle.IsNativeEnableAtb() && btl_util.IsCommandDeclarable(cmd.cmd_no))
            return;
        if (Configuration.Battle.Speed == 3 && cmd.regist != null && btl_util.IsBtlBusy(cmd.regist, btl_util.BusyMode.ANY_CURRENT))
            return;
        if (cmd.cmd_no == BattleCommandId.SysEscape && btlsys.cur_cmd_list.Count > 0)
            return;
        if (cmd.regist != null && cmd.regist.bi.player != 0 && btl_mot.checkMotion(cmd.regist, BattlePlayerCharacter.PlayerMotionIndex.MP_NORMAL_TO_CMD))
            return;

        btl_cmd.next_cmd_delay = btl_cmd.cmd_delay_max;
        IOverloadOnCommandRunScript overloadedMethod = ScriptsLoader.GetOverloadedMethod(typeof(IOverloadOnCommandRunScript)) as IOverloadOnCommandRunScript;
        if (overloadedMethod != null)
        {
            if (overloadedMethod.OnCommandRun(new BattleCommand(cmd)))
                return;
        }
        else
        {
            // Default method
            if (cmd.cmd_no < BattleCommandId.EnemyReaction || cmd.cmd_no > BattleCommandId.BoundaryUpperCheck)
            {
                BTL_DATA btl = cmd.regist;
                if (btl_stat.CheckStatus(btl, BattleStatus.Heat))
                {
                    if (btl_stat.AlterStatus(new BattleUnit(btl), BattleStatusId.Death) == btl_stat.ALTER_SUCCESS)
                    {
                        BattleVoice.TriggerOnStatusChange(btl, "Used", BattleStatusId.Heat);
                        KillCommand(cmd);
                        return;
                    }
                }
            }
        }
        btlsys.cur_cmd_list.Add(cmd);
        KillCommand(cmd);
    }

    public static void CommandEngine(FF9StateBattleSystem btlsys)
    {
        btl_cmd.RunCommandFromQueue(btlsys);
        List<CMD_DATA> curCommandDuplicate = new List<CMD_DATA>();
        foreach (CMD_DATA cmd in btlsys.cur_cmd_list)
            curCommandDuplicate.Add(cmd);
        foreach (CMD_DATA cmd in curCommandDuplicate)
        {
            BTL_DATA caster = cmd.regist;
            switch (cmd.info.mode)
            {
                case command_mode_index.CMD_MODE_INSPECTION:
                    if (!CheckCommandCondition(btlsys, cmd) || !CheckTargetCondition(btlsys, cmd) || !CheckMpCondition(cmd))
                    {
                        ResetItemCount(cmd);
                        //if (caster != null && caster.bi.player != 0 && btl_mot.checkMotion(caster, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD))
                        //    btl_mot.setMotion(caster, BattlePlayerCharacter.PlayerMotionIndex.MP_CMD_TO_NORMAL);
                        cmd.info.mode = command_mode_index.CMD_MODE_IDLE;
                        break;
                    }
                    //if (caster != null && !btl_mot.checkMotion(caster, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD) && cmd1.cmd_no < BattleCommandId.EnemyAtk)
                    //{
                    //    btl_mot.setMotion(caster, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD);
                    //    caster.evt.animFrame = 0;
                    //}
                    cmd.info.mode = command_mode_index.CMD_MODE_SELECT_VFX;
                    break;
                case command_mode_index.CMD_MODE_SELECT_VFX:
                {
                    if (caster != null && caster.bi.player != 0 && !caster.is_monster_transform && (cmd.cmd_no <= BattleCommandId.RushAttack || cmd.cmd_no > BattleCommandId.BoundaryUpperCheck))
                    {
                        BattlePlayerCharacter.PlayerMotionIndex motion = btl_mot.getMotion(caster);
                        BattlePlayerCharacter.PlayerMotionStance stance = btl_mot.EndingMotionStance(motion);
                        if (stance == BattlePlayerCharacter.PlayerMotionStance.NORMAL || stance == BattlePlayerCharacter.PlayerMotionStance.DYING || stance == BattlePlayerCharacter.PlayerMotionStance.DEFEND || stance == BattlePlayerCharacter.PlayerMotionStance.DISABLE)
                            break;
                        if (stance == BattlePlayerCharacter.PlayerMotionStance.CMD && motion != BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD && caster.animEndFrame)
                            break;
                    }
                    if (!ConfirmValidTarget(cmd))
                        break;
                    UInt16 tryCover = 0;
                    for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                        foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(next))
                            saFeature.TriggerOnCommand(new BattleUnit(next), new BattleCommand(cmd), ref tryCover);
                    if (tryCover != 0)
                    {
                        foreach (BTL_DATA target in btl_util.findAllBtlData(cmd.tar_id))
                        {
                            UInt16 coverId = btl_abil.CheckCoverAbility(target, tryCover);
                            if (coverId != 0)
                            {
                                cmd.tar_id &= (UInt16)~target.btl_id;
                                cmd.tar_id |= coverId;
                                cmd.info.cover = 1;
                                BattleUnit coveringTarget = btl_scrp.FindBattleUnit(coverId);
                                if (caster != null && coveringTarget != null)
                                    BattleVoice.TriggerOnBattleAct(coveringTarget.Data, "Cover", cmd);
                            }
                        }
                    }
                    if (!ConfirmValidTarget(cmd))
                        break;

                    if (caster != null)
                        BattleVoice.TriggerOnBattleAct(caster, "CommandPerform", cmd);
                    btl_vfx.SelectCommandVfx(cmd);

                    if (caster != null && caster.bi.player != 0)
                    {
                        BattleAbilityId aaIndex = btl_util.GetCommandMainActionIndex(cmd);
                        if (aaIndex != BattleAbilityId.Void)
                            FF9StateSystem.EventState.IncreaseAAUsageCounter(aaIndex);
                    }
                    if (BattleHUD.MixCommandSet.TryGetValue(cmd.cmd_no, out MixCommandType mixInfo) && ff9mixitem.MixItemsData.TryGetValue(cmd.sub_no, out MixItems MixChoosen))
                    {
                        if (cmd.sub_no >= 0 || mixInfo.consumeOnFail)
                            foreach (RegularItem ingredient in MixChoosen.Ingredients)
                                UIManager.Battle.ItemUse(ingredient);
                    }
                    else
                    {
                        RegularItem itemId = btl_util.GetCommandItem(cmd);
                        if (itemId != RegularItem.NoItem)
                            UIManager.Battle.ItemUse(itemId);
                    }

                    cmd.info.mode = command_mode_index.CMD_MODE_LOOP;
                    break;
                }
                case command_mode_index.CMD_MODE_LOOP:
                    if (Configuration.Battle.SFXRework || caster.bi.player != 0 || cmd.info.mon_reflec != 0)
                        CheckCommandLoop(cmd);
                    break;
                case command_mode_index.CMD_MODE_IDLE:
                    if (cmd.cmd_no != BattleCommandId.SysEscape)
                    {
                        ReqFinishCommand(cmd);
                        break;
                    }
                    FinishCommand(btlsys, cmd);
                    break;
            }
            if (cmd.info.mode == command_mode_index.CMD_MODE_DONE)
                FinishCommand(btlsys, cmd);
        }
    }

    public static void KillCommand(CMD_DATA cmdToCancel)
    {
        BTL_DATA btl = cmdToCancel.regist;
        for (CMD_DATA cmd = FF9StateSystem.Battle.FF9Battle.cmd_queue; cmd != null; cmd = cmd.next)
        {
            if (cmd.next == cmdToCancel)
            {
                if (btl != null && (btl.bi.player == 0 || (cmdToCancel != btl.cmd[3] && cmdToCancel != btl.cmd[4])))
                    btl.bi.cmd_idle = 0;
                DequeueCommand(cmd, false);
                break;
            }
        }
    }

    public static Boolean KillMainCommand(BTL_DATA btl)
    {
        for (CMD_DATA cmd = FF9StateSystem.Battle.FF9Battle.cmd_queue; cmd != null; cmd = cmd.next)
        {
            if (cmd.next == btl.cmd[0])
            {
                Int32 btlNum = 0;
                while (1 << btlNum != btl.btl_id)
                    ++btlNum;
                btl.bi.cmd_idle = 0;
                DequeueCommand(cmd, false);
                UIManager.Battle.InputFinishList.Remove(btlNum);
                return true;
            }
        }
        return false;
    }

    public static Boolean KillCommand2(BTL_DATA btl)
    {
        Boolean cancelMainCmd = false;
        btl.bi.cmd_idle = 0;
        CMD_DATA parentCmd = FF9StateSystem.Battle.FF9Battle.cmd_queue;
        while (parentCmd != null)
        {
            CMD_DATA cmd = parentCmd.next;
            if (cmd != null && cmd.regist == btl && (cmd.cmd_no < BattleCommandId.EnemyDying || cmd.cmd_no > BattleCommandId.BoundaryUpperCheck))
            {
                if (btl_util.IsCommandDeclarable(cmd.cmd_no))
                    cancelMainCmd = true;
                ResetItemCount(cmd);
                DequeueCommand(parentCmd, true);
            }
            else
            {
                parentCmd = cmd;
            }
        }
        return cancelMainCmd;
    }

    public static void KillCommand3(BTL_DATA btl)
    {
        CMD_DATA parentCmd = FF9StateSystem.Battle.FF9Battle.cmd_queue;
        while (parentCmd != null)
        {
            CMD_DATA cmd = parentCmd.next;
            if (cmd != null && cmd.regist == btl)
            {
                ResetItemCount(cmd);
                DequeueCommand(parentCmd, true);
            }
            else
            {
                parentCmd = cmd;
            }
        }
    }

    public static Boolean KillSpecificCommand(BTL_DATA btl, BattleCommandId cmd_no)
    {
        for (CMD_DATA parentCmd = FF9StateSystem.Battle.FF9Battle.cmd_queue; parentCmd != null; parentCmd = parentCmd.next)
        {
            CMD_DATA cmd = parentCmd.next;
            if (cmd != null && cmd.regist == btl && cmd.cmd_no == cmd_no)
            {
                if (cmd == btl.cmd[0])
                    return KillMainCommand(btl);
                DequeueCommand(parentCmd, true);
                return true;
            }
        }
        return false;
    }

    private static Boolean ConfirmValidTarget(CMD_DATA cmd)
    {
        Boolean valid = btl_util.findAllBtlData(cmd.tar_id).Exists(btl => btl.bi.target != 0 /*&& btl.bi.disappear == 0*/);
        if (!valid)
        {
            ResetItemCount(cmd);
            cmd.info.mode = command_mode_index.CMD_MODE_DONE;
        }
        return valid;
    }

    private static void ResetItemCount(CMD_DATA cmd)
    {
        RegularItem itemId = btl_util.GetCommandItem(cmd);
        if (itemId == RegularItem.NoItem)
            return;
        UIManager.Battle.ItemUnuse(itemId);
    }

    private static void DequeueCommand(CMD_DATA cmd, Boolean escape_check)
    {
        CMD_DATA cmdData = cmd.next;
        cmd.next = cmdData.next;
        cmdData.info.stat = 0;
        cmdData.info.priority = 0;
        if (escape_check && cmdData.cmd_no == BattleCommandId.SysEscape)
            FF9StateSystem.Battle.FF9Battle.cmd_status &= 0xFFFE;
    }

    public static Boolean CheckSpecificCommand(BTL_DATA btl, BattleCommandId cmd_no)
    {
        for (CMD_DATA cmdData = FF9StateSystem.Battle.FF9Battle.cmd_queue.next; cmdData != null; cmdData = cmdData.next)
            if (cmdData.regist == btl && cmdData.cmd_no == cmd_no)
                return true;
        foreach (CMD_DATA cmdData in FF9StateSystem.Battle.FF9Battle.cur_cmd_list)
            if (cmdData.regist == btl && cmdData.cmd_no == cmd_no)
                return true;
        return false;
    }

    public static Boolean CheckSpecificCommand2(BattleCommandId cmd_no)
    {
        for (CMD_DATA cmdData = FF9StateSystem.Battle.FF9Battle.cmd_queue.next; cmdData != null; cmdData = cmdData.next)
            if (cmdData.cmd_no == cmd_no)
                return true;
        foreach (CMD_DATA cmdData in FF9StateSystem.Battle.FF9Battle.cur_cmd_list)
            if (cmdData.cmd_no == cmd_no)
                return true;
        return false;
    }

    public static Boolean CheckUsingCommand(CMD_DATA cmd)
    {
        for (CMD_DATA cmdData = FF9StateSystem.Battle.FF9Battle.cmd_queue.next; cmdData != null; cmdData = cmdData.next)
            if (cmd == cmdData)
                return true;
        foreach (CMD_DATA cmdData in FF9StateSystem.Battle.FF9Battle.cur_cmd_list)
            if (cmdData == cmd)
                return true;
        return false;
    }

    public static Boolean CheckCommandQueued(CMD_DATA cmd)
    {
        for (CMD_DATA cmdData = FF9StateSystem.Battle.FF9Battle.cmd_queue.next; cmdData != null; cmdData = cmdData.next)
            if (cmd == cmdData)
                return true;
        return false;
    }

    public static void EnqueueCommand(CMD_DATA cmd)
    {
        for (CMD_DATA cp = FF9StateSystem.Battle.FF9Battle.cmd_queue; cp != null; cp = cp.next)
        {
            if (cmd.info.priority != 0 && cp.next != null)
            {
                if (cmd.cmd_no > BattleCommandId.SysTrans || cmd.cmd_no == BattleCommandId.EnemyDying)
                {
                    if ((cp.next.cmd_no < BattleCommandId.SysDead || cp.next.cmd_no > BattleCommandId.BoundaryUpperCheck) && cp.next.cmd_no != BattleCommandId.EnemyDying)
                    {
                        InsertCommand(cmd, cp);
                        break;
                    }
                }
                else if (cmd.cmd_no == BattleCommandId.SysTrans)
                {
                    if ((cp.next.cmd_no < BattleCommandId.SysTrans || cp.next.cmd_no > BattleCommandId.BoundaryUpperCheck) && cp.next.cmd_no != BattleCommandId.EnemyDying)
                    {
                        InsertCommand(cmd, cp);
                        break;
                    }
                }
                else if (cp.next.info.priority == 0 || btl_util.IsCommandDeclarable(cp.next.cmd_no))
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

    public static UInt16 CheckReflec(CMD_DATA cmd)
    {
        UInt16 reflectingBtl = 0;
        if (cmd.cmd_no != BattleCommandId.Item && cmd.cmd_no != BattleCommandId.Throw && cmd.cmd_no != BattleCommandId.AutoPotion && (cmd.AbilityCategory & 1) != 0 && !cmd.info.ReflectNull && !cmd.info.HasCheckedReflect)
        {
            UInt16[] targetablePlayers = new UInt16[4];
            UInt16[] targetableEnemies = new UInt16[4];
            Int16 enemyReflectCount = 0;
            Int16 partyReflectCount = 0;
            Int16 tpi = 0;
            Int16 tei = 0;
            for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
            {
                if ((btl.btl_id & cmd.tar_id) != 0)
                {
                    if (btl.bi.target != 0 && (btl.stat.CurrentIncludeOnHold & BattleStatusConst.ApplyReflect) != 0 && (btl.stat.CurrentIncludeOnHold & BattleStatusConst.PreventReflect) == 0)
                    {
                        reflectingBtl |= btl.btl_id;
                        if (btl.bi.player == 1)
                            partyReflectCount++;
                        else
                            enemyReflectCount++;
                        foreach (BattleStatusId reflectStatus in (BattleStatusConst.ApplyReflect & btl.stat.cur).ToStatusList())
                            BattleVoice.TriggerOnStatusChange(btl, "Used", reflectStatus);
                    }
                }
                else if (!btl_stat.CheckStatus(btl, BattleStatus.Death) && btl.bi.target != 0)
                {
                    if (btl.bi.player == 1)
                        targetablePlayers[tpi++] = btl.btl_id;
                    else
                        targetableEnemies[tei++] = btl.btl_id;
                }
            }
            if (partyReflectCount != 0 || enemyReflectCount != 0)
            {
                List<UInt16> reflectList = new List<UInt16>();
                if (tei > 0)
                    for (Int32 i = 0; i < partyReflectCount; i++)
                        reflectList.Add(targetableEnemies[Comn.random8() % tei]);
                if (tpi > 0)
                    for (Int32 i = 0; i < enemyReflectCount; i++)
                        reflectList.Add(targetablePlayers[Comn.random8() % tpi]);
                for (Int32 index = 0; index < 4; ++index) // TODO: have more possible reflects at least with SFXRework system?
                    cmd.reflec.tar_id[index] = index >= reflectList.Count ? (UInt16)0 : reflectList[index];
                if (reflectingBtl == cmd.tar_id)
                {
                    cmd.info.reflec = 1;
                    cmd.tar_id = MergeReflecTargetID(cmd.reflec);
                }
                else
                {
                    cmd.info.reflec = 2;
                    cmd.tar_id = (UInt16)(cmd.tar_id & ~reflectingBtl);
                }
            }
            cmd.info.HasCheckedReflect = true;
        }
        return reflectingBtl;
    }

    // ReSharper disable PossibleNullReferenceException
    public static Boolean CheckCommandCondition(FF9StateBattleSystem btlsys, CMD_DATA cmd)
    {
        BattleUnit caster = cmd.regist == null ? null : new BattleUnit(cmd.regist);
        FF9StateGlobal ff9StateGlobal = FF9StateSystem.Common.FF9;
        if (caster != null)
        {
            if (caster.IsUnderStatus(BattleStatus.Death) && (cmd.cmd_no <= BattleCommandId.EnemyCounter || cmd.cmd_no > BattleCommandId.BoundaryUpperCheck))
                return false;
            if (cmd.cmd_no < BattleCommandId.EnemyReaction || cmd.cmd_no > BattleCommandId.BoundaryUpperCheck)
                caster.FaceTheEnemy();

            // Garnet is depressed.
            if (battle.GARNET_DEPRESS_FLAG != 0 && caster.IsPlayer && caster.PlayerIndex == CharacterId.Garnet && !Configuration.Battle.GarnetConcentrate)
            {
                if (btl_util.IsCommandDeclarable(cmd.cmd_no) && Comn.random8() < 64) // 25%
                {
                    UIManager.Battle.SetBattleFollowMessage(BattleMesages.DaggerCannotConcentrate);
                    return false;
                }
            }

            if (!CheckMagicCondition(cmd))
                return false;

            /*if (cmd.IsShortRange && Comn.countBits(cmd.tar_id) > 1)
            {
                for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                    if (next.out_of_reach && (next.btl_id & cmd.tar_id) != 0)
                        cmd.tar_id &= (UInt16)~next.btl_id;
                if (cmd.tar_id == 0)
                {
                    UIManager.Battle.SetBattleFollowMessage(23, new object[0]);
                    return false;
                }
            }*/
        }

        if (btl_util.IsCommandMonsterTransform(cmd))
            return true;

        Boolean notEnoughItems = false;
        if (BattleHUD.MixCommandSet.TryGetValue(cmd.cmd_no, out MixCommandType mixInfo) && ff9mixitem.MixItemsData.TryGetValue(cmd.sub_no, out MixItems MixChoosen))
        {
            if (cmd.sub_no >= 0 || mixInfo.consumeOnFail)
            {
                Dictionary<RegularItem, Int32> allIngredients = MixChoosen.GetIngredientsAsDict();
                foreach (KeyValuePair<RegularItem, Int32> requirement in allIngredients)
                    if (ff9item.FF9Item_GetCount(requirement.Key) < requirement.Value)
                        notEnoughItems = true;
            }
            if (cmd.sub_no < 0 && mixInfo.failType == FailedMixType.SKIP_TURN)
            {
                if (mixInfo.consumeOnFail)
                {
                    // Consume the mix ingredients attempt now because CMD_MODE_SELECT_VFX will never be reached
                    foreach (RegularItem ingredient in MixChoosen.Ingredients)
                        UIManager.Battle.ItemUse(ingredient);
                }
                UIManager.Battle.SetBattleFollowMessage(5, Localization.GetWithDefault("FailedMixMessage"));
                return false;
            }
        }
        else
        {
            RegularItem itemId = btl_util.GetCommandItem(cmd);
            if (itemId != RegularItem.NoItem && ff9item.FF9Item_GetCount(itemId) == 0)
                notEnoughItems = true;
        }
        if (notEnoughItems)
        {
            UIManager.Battle.SetBattleFollowMessage(BattleMesages.NotEnoughItems);
            return false;
        }
        if (!BattleAbilityHelper.ApplySpecialCommandCondition(cmd))
            return false;

        BattleAbilityId abilId = btl_util.GetCommandMainActionIndex(cmd);
        if (caster != null)
        {
            BattleMagicSwordSet magicSet = UIManager.Battle.GetMagicSwordOfAbility(caster, ff9abil.GetAbilityIdFromActiveAbility(abilId));
            if (magicSet != null && !DecideMagicSword(caster, magicSet))
                return false;
        }

        switch (cmd.cmd_no)
        {
            case BattleCommandId.Jump:
                btl_stat.AlterStatus(caster, BattleStatusId.Jump, caster, false, BattleCommandId.Spear, BattleAbilityId.Spear1, cmd.tar_id);
                break;
            case BattleCommandId.JumpInTrance:
                btl_stat.AlterStatus(caster, BattleStatusId.Jump, caster, false, BattleCommandId.SpearInTrance, BattleAbilityId.Spear2, btl_util.GetStatusBtlID(1, 0));
                break;
            case BattleCommandId.MagicCounter:
                UIManager.Battle.SetBattleFollowMessage(BattleMesages.ReturnMagic, msgCmd: cmd);
                break;
            case BattleCommandId.AutoPotion:
                UIManager.Battle.SetBattleFollowMessage(BattleMesages.AutoPotion, msgCmd: cmd);
                break;
            case BattleCommandId.SummonGarnet:
            case BattleCommandId.SummonEiko:
            case BattleCommandId.Phantom:
                DecideSummonType(cmd);
                break;
            case BattleCommandId.Counter:
                if (abilId == BattleAbilityId.Attack)
                    UIManager.Battle.SetBattleFollowMessage(BattleMesages.CounterAttack, msgCmd: cmd);
                break;
            case BattleCommandId.SysEscape:
                if (btlsys.btl_phase == FF9StateBattleSystem.PHASE_NORMAL)
                {
                    for (BTL_DATA btl = btlsys.btl_list.next; btl != null; btl = btl.next)
                    {
                        if (btl.bi.player != 0 && !btl_stat.CheckStatus(btl, BattleStatusConst.CannotEscape) && btl.cur.hp > 0)
                        {
                            if (!btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE))
                                btl_mot.SetDefaultIdle(btl, false);
                            //{
                            //    btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE);
                            //    btl.evt.animFrame = 0;
                            //}
                            btlsys.btl_phase = FF9StateBattleSystem.PHASE_MENU_OFF;
                            btlsys.btl_seq = FF9StateBattleSystem.SEQ_MENU_OFF_ESCAPE;
                        }
                    }
                    if (btlsys.btl_phase == FF9StateBattleSystem.PHASE_MENU_OFF && btlsys.btl_seq == FF9StateBattleSystem.SEQ_MENU_OFF_ESCAPE)
                    {
                        UIManager.Battle.SetIdle();
                        ++ff9StateGlobal.party.escape_no;
                        if (cmd.sub_no == 0)
                            ff9StateGlobal.btl_flag |= battle.BTL_FLAG_ABILITY_FLEE;
                        KillAllCommand(btlsys);
                    }
                    else
                    {
                        btlsys.cmd_status &= 0xFFFE;
                    }
                    return false;
                }
                break;
            case BattleCommandId.SysPhantom:
                break;
            case BattleCommandId.SysTrans:
                if (caster.IsUnderAnyStatus(BattleStatus.Trance))
                {
                    BattleMesages tranceMessage = BattleMesages.Trance;
                    if (caster.IsUnderPermanentStatus(BattleStatus.Trance))
                    {
                        tranceMessage = BattleMesages.PermanentTrance;
                        String permTrance = FF9TextTool.BattleFollowText((Int32)tranceMessage + 7);
                        if (String.IsNullOrEmpty(permTrance) || permTrance.Length <= 1)
                            tranceMessage = BattleMesages.Trance;
                    }
                    UIManager.Battle.SetBattleFollowMessage(tranceMessage);
                }
                return true;
            case BattleCommandId.SysDead: // Unused anymore
                btl_sys.CheckBattleMenuOff(caster);
                if (caster.Data.die_seq == 0)
                {
                    if (caster.IsPlayer)
                        caster.Data.die_seq = !btl_mot.checkMotion(caster.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE) ? (Byte)1 : (Byte)5;
                    else
                        caster.Data.die_seq = caster.IsSlave || caster.Data.bi.death_f == 0 ? (Byte)1 : (Byte)3;
                }
                return false;
            case BattleCommandId.SysReraise: // Unused anymore
                caster.CurrentHp = 1;
                caster.RemoveStatus(BattleStatusId.Death);
                //caster.Data.bi.dmg_mot_f = 1;
                FF9StateSystem.Settings.SetHPFull();
                return false;
            case BattleCommandId.SysStone:
                btl_stat.AlterStatus(caster, BattleStatusId.Petrify);
                return false;
            case BattleCommandId.None:
            case BattleCommandId.Spear:
            case BattleCommandId.Escape:
            case BattleCommandId.FinishBlow:
                break;
            case BattleCommandId.EnemyAtk:
            case BattleCommandId.BoundaryCheck:
            case BattleCommandId.EnemyCounter:
            case BattleCommandId.EnemyDying:
            case BattleCommandId.EnemyReaction:
            case BattleCommandId.ScriptCounter1:
            case BattleCommandId.ScriptCounter2:
            case BattleCommandId.ScriptCounter3:
                break;
        }
        return true;
    }

    // ReSharper restore PossibleNullReferenceException
    public static Boolean CheckMagicCondition(CMD_DATA cmd)
    {
        if (!btl_stat.CheckStatus(cmd.regist, BattleStatusConst.CannotUseMagic) || (cmd.AbilityCategory & 2) == 0)
            return true;
        UIManager.Battle.SetBattleFollowMessage(BattleMesages.CannotCast);
        foreach (BattleStatusId silenceStatus in (BattleStatusConst.CannotUseMagic & cmd.regist.stat.cur).ToStatusList())
            BattleVoice.TriggerOnStatusChange(cmd.regist, "Used", silenceStatus);
        return false;
    }

    public static void KillNormalCommand(FF9StateBattleSystem btlsys)
    {
        CMD_DATA cp = btlsys.cmd_queue;
        while (cp != null)
        {
            CMD_DATA ncp = cp.next;
            if (ncp != null && ncp.cmd_no != BattleCommandId.EnemyDying && (ncp.cmd_no < BattleCommandId.SysLastPhoenix || ncp.cmd_no > BattleCommandId.BoundaryUpperCheck))
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
            if (ncp != null && (ncp.cmd_no < BattleCommandId.SysTrans || ncp.cmd_no > BattleCommandId.BoundaryUpperCheck))
                ManageDequeueCommand(cp, ncp);
            else
                cp = ncp;
        }
        btlsys.cmd_status &= 0xFFF3;
    }

    public static void ClearSysPhantom(BTL_DATA btl)
    {
        if (btl.bi.player == 0 || btl.bi.slot_no != (Byte)CharacterId.Garnet)
            return;
        KillSpecificCommand(btl, BattleCommandId.SysPhantom);
        FF9StateSystem.Battle.FF9Battle.cmd_status &= 0xFFF3;
    }

    public static void ManageDequeueCommand(CMD_DATA cp, CMD_DATA ncp)
    {
        if (cp == null)
            return;
        //FF9StateBattleSystem stateBattleSystem = FF9StateSystem.Battle.FF9Battle;
        BTL_DATA btl = ncp.regist;
        if (btl_util.IsCommandDeclarable(ncp.cmd_no) && btl != null && !btl_stat.CheckStatus(btl, BattleStatus.Death))
        {
            btl.sel_mode = 0;
            //if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD)
            //    && (btl.bi.slot_no != 6 || stateBattleSystem.cur_cmd != null && stateBattleSystem.cur_cmd.cmd_no != BattleCommandId.DoubleWhiteMagic)
            //    && (btl.bi.slot_no != 1 || stateBattleSystem.cur_cmd != null && stateBattleSystem.cur_cmd.cmd_no != BattleCommandId.DoubleBlackMagic)/*&& stateBattleSystem.cur_cmd.cmd_no != BattleCommandId.MagicSword*/)
            {
                //btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_CMD_TO_NORMAL);
                //btl.evt.animFrame = 0;
                btl.bi.cmd_idle = 0;
            }
        }
        ResetItemCount(ncp);
        DequeueCommand(cp, true);
    }

    private static Boolean CheckMpCondition(CMD_DATA cmd)
    {
        Int32 mp = cmd.GetCommandMPCost();
        if (cmd.regist != null)
            if (BattleAbilityHelper.GetPatchedMPCost(ref mp, btl_util.GetCommandMainActionIndex(cmd), new BattleUnit(cmd.regist), cmd.cmd_no, cmd.info.cmdMenu, cmd.aa, cmd))
                cmd.info.CustomMPCost = mp;

        if (battle.GARNET_SUMMON_FLAG != 0 && (cmd.AbilityType & 4) != 0)
            mp *= 4;

        if (cmd.regist != null)
        {
            if (cmd.regist.bi.player != 0)
                mp = mp * FF9StateSystem.Common.FF9.player[(CharacterId)cmd.regist.bi.slot_no].mpCostFactor / 100;
            if (cmd.cmd_no == BattleCommandId.MagicCounter || ConsumeMp(cmd.regist, mp))
                return true;
        }

        UIManager.Battle.SetBattleFollowMessage(BattleMesages.NotEnoughMp);
        return false;
    }

    private static Boolean CheckTargetCondition(FF9StateBattleSystem btlsys, CMD_DATA cmd)
    {
        UInt16 validTarId = 0;
        if (cmd.tar_id == 0)
            return false;
        Boolean forDead;
        switch (cmd.cmd_no)
        {
            case BattleCommandId.Item:
            case BattleCommandId.AutoPotion:
                forDead = ff9item.GetItemEffect(btl_util.GetCommandItem(cmd)).info.ForDead;
                break;
            case BattleCommandId.SysTrans:
                return true;
            default:
                forDead = cmd.aa.Info.ForDead;
                break;
        }
        for (BTL_DATA btl = btlsys.btl_list.next; btl != null; btl = btl.next)
            if (btl.bi.target != 0 && (btl.btl_id & cmd.tar_id) != 0 && (forDead && btl.bi.player != 0 || !btl_stat.CheckStatus(btl, BattleStatus.Death)) && (!btl.out_of_reach || !cmd.IsShortRange))
                validTarId |= btl.btl_id;
        if (validTarId != 0)
        {
            cmd.tar_id = validTarId;
            return true;
        }
        if ((cmd.info.cursor & 1) == 0)
        {
            if (cmd.IsShortRange)
            {
                UInt16 targetInRange = 0;
                Boolean allowPlayer = (cmd.tar_id & 0xF) != 0;
                Boolean allowEnemy = (cmd.tar_id & 0xF0) != 0;
                for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                    if (((allowEnemy && next.bi.player == 0) || (allowPlayer && next.bi.player == 1)) && next.bi.target != 0 && !next.out_of_reach && (!btl_stat.CheckStatus(next, BattleStatus.Death) || forDead))
                        targetInRange |= next.btl_id;
                cmd.tar_id = (UInt16)Comn.randomID(targetInRange);
                if (cmd.tar_id == 0)
                {
                    if (btl_util.GetRandomBtlID(allowPlayer ? 1U : 0U, forDead) != 0)
                        UIManager.Battle.SetBattleFollowMessage(BattleMesages.CannotReach);
                    return false;
                }
            }
            else
            {
                cmd.tar_id = btl_util.GetRandomBtlID(cmd.tar_id & 15U, forDead);
            }
            if (cmd.tar_id != 0)
                return true;
        }
        return false;
    }

    public static void ReqFinishCommand(CMD_DATA cmd)
    {
        if (cmd.info.reflec == 2 && !Configuration.Battle.SFXRework)
        {
            btl_vfx.LoopBattleVfxForReflect(cmd, (UInt32)cmd.aa.Info.VfxIndex);
            cmd.info.mode = command_mode_index.CMD_MODE_LOOP;
        }
        else
        {
            if (cmd.cmd_no == BattleCommandId.SysDead && cmd.regist.stat.HasDeathChangerEffect && !btl_mot.checkMotion(cmd.regist, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE))
                return;
            cmd.info.mode = command_mode_index.CMD_MODE_DONE;
        }
    }

    public static void FinishCommand(FF9StateBattleSystem btlsys, CMD_DATA cmd)
    {
        BattleUnit caster = cmd.regist == null ? null : new BattleUnit(cmd.regist);
        BattleCommandId commandId = cmd.cmd_no;
        Int32 tranceDelta = 0;
        if (btl_util.IsCommandDeclarable(cmd.cmd_no) && caster != null)
        {
            if (cmd == caster.Data.cmd[0] && cmd.cmd_no != BattleCommandId.Jump && cmd.cmd_no != BattleCommandId.JumpInTrance)
                ResetCurrentBattlerActiveTime(caster);

            if (IsNeedToDecreaseTrance(caster, commandId, cmd))
            {
                // Note: there is a bug in the base game that is documented here
                // https://gamefaqs.gamespot.com/boards/197338-final-fantasy-ix/64320031
                // Because "tranceDelta" was originally a Byte, the formula could overflow
                // resulting in Quina at low level that could stay in trance for up to 64 turns
                // The bug is now fixed; if someone wants to put it back for any reason, it can be done with:
                //  TranceDecreaseFormula = ((300 - caster.Level) / caster.Will * 10) % 256
                tranceDelta = (300 - caster.Level) / caster.Will * 10;
                if (!String.IsNullOrEmpty(Configuration.Battle.TranceDecreaseFormula))
                {
                    Expression e = new Expression(Configuration.Battle.TranceDecreaseFormula);
                    e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                    e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                    NCalcUtility.InitializeExpressionUnit(ref e, caster);
                    NCalcUtility.InitializeExpressionCommand(ref e, new BattleCommand(cmd));
                    tranceDelta = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), tranceDelta);
                }

                if (btl_cmd.half_trance_cmd_list.Contains(cmd.cmd_no))
                    tranceDelta /= 2;

                if (caster.IsPlayer && FF9StateSystem.Settings.IsTranceFull)
                    tranceDelta = 0;

                if (tranceDelta >= 0)
                {
                    if (caster.Trance > tranceDelta)
                        caster.Trance -= (Byte)tranceDelta;
                    else if (!FF9StateSystem.Battle.isDebug)
                        caster.RemoveStatus(BattleStatus.Trance);
                }
                else
                {
                    if (caster.Trance - tranceDelta < Byte.MaxValue)
                        caster.Trance += (Byte)(-tranceDelta);
                    else
                        caster.Trance = Byte.MaxValue;
                }
            }
        }
        else if (cmd.cmd_no < BattleCommandId.EnemyCounter && CheckUsingCommand(caster.Data.cmd[0]))
        {
            caster.Data.bi.cmd_idle = 1;
        }

        if (cmd.info.cover != 0)
        {
            foreach (BTL_DATA target in btl_util.findAllBtlData(cmd.tar_id))
            {
                if (target.bi.cover != 0)
                {
                    if (!FF9StateSystem.Battle.FF9Battle.cur_cmd_list.Any(otherCmd => otherCmd != cmd && otherCmd.info.cover != 0 && (otherCmd.tar_id & target.btl_id) != 0))
                    {
                        if (!btl_util.IsBtlBusy(target.bi.cover_unit, btl_util.BusyMode.CASTER))
                            btl_mot.setBasePos(target.bi.cover_unit);
                        target.bi.cover_unit = null;
                        btl_mot.setBasePos(target);
                        btl_mot.SetDefaultIdle(target);
                    }
                }
            }
        }

        if (cmd.info.dodge != 0)
        {
            foreach (BTL_DATA target in btl_util.findAllBtlData(cmd.tar_id))
            {
                Boolean stillDodging = false;
                foreach (CMD_DATA runningCmd in btlsys.cur_cmd_list)
                    if (runningCmd != cmd && runningCmd.info.dodge != 0 && (runningCmd.tar_id & target.btl_id) != 0)
                        stillDodging = true;
                if (stillDodging)
                    continue;
                BattleUnit unit = new BattleUnit(target);
                unit.IsDodged = false;

                if (unit.IsPlayer)
                    btl_mot.SetDefaultIdle(unit.Data);
                else if (unit.IsSlave)
                    unit = btl_util.GetMasterEnemyBtlPtr(unit);

                if (!unit.IsPlayer)
                    unit.Data.pos[2] = unit.Data.base_pos[2];
            }
        }

        if (caster != null)
        {
            if (cmd.regist != null && cmd == caster.Data.cmd[0])
                UIManager.Battle.RemovePlayerFromAction(cmd.regist.btl_id, true);

            if (caster.IsPlayer && FF9StateSystem.Settings.IsATBFull)
                caster.CurrentAtb = (Int16)(caster.MaximumAtb - 1);

            foreach (BattleStatusId statusId in caster.CurrentStatus.ToStatusList())
                if (caster.Data.stat.effects.TryGetValue(statusId, out StatusScriptBase effect))
                    (effect as IFinishCommandScript)?.OnFinishCommand(cmd, tranceDelta);

            btl_para.CheckPointData(caster);
        }

        btlsys.cur_cmd_list.Remove(cmd);
        if (cmd.info.cmd_motion)
            btl_mot.EndCommandMotion(cmd);
        ClearCommand(cmd);

        if (!FF9StateSystem.Battle.isDebug)
            HonoluluBattleMain.playerEnterCommand = true;
    }

    private static Boolean IsNeedToDecreaseTrance(BattleUnit caster, BattleCommandId commandId, CMD_DATA cmd)
    {
        if (!caster.IsUnderStatus(BattleStatus.Trance) || caster.IsUnderPermanentStatus(BattleStatus.Trance))
            return false;

        if (commandId == BattleCommandId.Jump || commandId == BattleCommandId.JumpInTrance)
            return false;

        return true;
    }

    private static void ResetCurrentBattlerActiveTime(BattleUnit unit)
    {
        if (Configuration.Fixes.IsKeepRestTimeInBattle && unit.MaximumAtb > 0)
            unit.CurrentAtb = (Int16)Math.Max(0, unit.CurrentAtb - unit.MaximumAtb);
        else
            unit.CurrentAtb = 0;

        unit.Data.sel_mode = 0;
    }

    public static UInt16 MergeReflecTargetID(REFLEC_DATA reflec)
    {
        UInt16 num = 0;
        for (Int32 index = 0; index < 4; ++index)
            num |= reflec.tar_id[index];
        return num;
    }

    public static void CheckCommandLoop(CMD_DATA cmd)
    {
        Boolean stillRunning = !Configuration.Battle.SFXRework && SFX.isRunning && FF9StateSystem.Battle.FF9Battle.cur_cmd == cmd;
        stillRunning = stillRunning || UnifiedBattleSequencer.runningActions.Any(action => action.cmd == cmd);
        if (stillRunning)
        {
            if (!Configuration.Battle.SFXRework && cmd.cmd_no == BattleCommandId.SysTrans && SFX.frameIndex == 75)
            {
                Boolean toTrance = btl_stat.CheckStatus(cmd.regist, BattleStatus.Trance);
                cmd.regist.enable_trance_glow = toTrance;
                if (cmd.regist.bi.player != 0)
                {
                    btl_vfx.SetTranceModel(cmd.regist, toTrance);
                    if (toTrance)
                        BattleAchievement.UpdateTranceStatus();
                }
            }
        }
        else
        {
            BTL_DATA btlData = cmd.regist;
            if (!Configuration.Battle.SFXRework)
            {
                if (!FF9StateSystem.Battle.isDebug && (UIManager.Battle.BtlWorkLibra || UIManager.Battle.BtlWorkPeep))
                    return;
                if (cmd.cmd_no == BattleCommandId.Defend)
                {
                    //if (btl_mot.checkMotion(btlData, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_DEF))
                    //{
                    //    if (btlData.evt.animFrame < GeoAnim.geoAnimGetNumFrames(btlData))
                    //        return;
                    //    btl_mot.setMotion(btlData, BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE);
                    //    btlData.evt.animFrame = 0;
                    //}
                }
                else if (cmd.cmd_no == BattleCommandId.Change)
                {
                    if (btl_mot.checkMotion(btlData, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD))
                    {
                        if (!btlData.animEndFrame)
                            return;
                        btl_mot.setMotion(btlData, (Byte)(29U + btlData.bi.row));
                        btlData.evt.animFrame = 0;
                    }
                    if (btl_mot.checkMotion(btlData, (Byte)(29U + btlData.bi.row)))
                    {
                        UInt16 numFrames = (UInt16)GeoAnim.getAnimationLoopFrame(btlData);
                        if (btlData.evt.animFrame < numFrames)
                        {
                            UInt16 num2 = (UInt16)(btlData.evt.animFrame + 1);
                            btlData.pos[2] = btlData.bi.row == 0 ? 400 * num2 / numFrames - 1960 : -400 * num2 / numFrames - 1560;
                            btlData.gameObject.transform.localPosition = btlData.pos;
                            return;
                        }
                        btlData.pos[2] = btlData.bi.row == 0 ? -1560 : -1960;
                        btlData.gameObject.transform.localPosition = btlData.pos;
                        cmd.info.effect_counter++;
                        ExecVfxCommand(btlData, cmd);
                        //btl_mot.setMotion(btlData, BattlePlayerCharacter.PlayerMotionIndex.MP_CMD_TO_NORMAL);
                        //btlData.evt.animFrame = 0;
                        //return;
                    }
                    else
                    {
                        return;
                    }
                    //if (btl_mot.checkMotion(btlData, BattlePlayerCharacter.PlayerMotionIndex.MP_CMD_TO_NORMAL))
                    //{
                    //    if (!btlData.animEndFrame)
                    //        return;
                    //    btl_mot.setMotion(btlData, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL);
                    //    btlData.evt.animFrame = 0;
                    //}
                }
                if (cmd.regist != null)
                    cmd.regist.animFlag &= (UInt16)~EventEngine.afLoop;
                if (cmd.vfxRequest.mexe != null)
                    cmd.vfxRequest.mexe.animFlag &= (UInt16)~EventEngine.afLoop;
            }
            if (cmd.regist != null && cmd.regist.bi.player != 0)
                BattleAchievement.UpdateCommandAchievement(cmd);
            ReqFinishCommand(cmd);
            if (cmd.cmd_no == BattleCommandId.SysTrans)
                btl_stat.SetPresentColor(btlData);
        }
    }

    public static void ExecVfxCommand(BTL_DATA target, CMD_DATA cmd = null, BattleActionThread sfxThread = null)
    {
        if (cmd == null)
            cmd = btl_util.getCurCmdPtr();
        if (cmd == null)
        {
            Debug.LogError("no command!");
            return;
        }

        BTL_DATA caster = cmd.regist;
        switch (cmd.cmd_no)
        {
            case BattleCommandId.Jump:
            case BattleCommandId.JumpInTrance:
                return;
            case BattleCommandId.SysLastPhoenix:
                UInt16 battleId = btl_scrp.GetBattleID(1u);
                UInt16 statusBtlId = btl_util.GetStatusBtlID(1u, BattleStatusConst.BattleEndFull);
                if (battleId == 0 || battleId == statusBtlId)
                {
                    FF9StateBattleSystem btlsys = FF9StateSystem.Battle.FF9Battle;
                    UIManager.Battle.FF9BMenu_EnableMenu(false);
                    if (btlsys.btl_phase != FF9StateBattleSystem.PHASE_MENU_OFF)
                    {
                        btlsys.btl_phase = FF9StateBattleSystem.PHASE_MENU_OFF;
                        btlsys.btl_seq = FF9StateBattleSystem.SEQ_MENU_OFF_VICTORY;
                        KillAllCommand(btlsys);
                    }
                }
                SBattleCalculator.CalcMain(caster, target, cmd, sfxThread);
                return;
            case BattleCommandId.SysTrans:
            {
                BattleCommand command = new BattleCommand(cmd);
                BattleCalculator v = new BattleCalculator(caster, target, command);
                Boolean toTrance = btl_stat.CheckStatus(cmd.regist, BattleStatus.Trance);
                cmd.regist.enable_trance_glow = toTrance;
                command.ScriptId = 0;
                if (cmd.regist.bi.player != 0)
                {
                    btl_vfx.SetTranceModel(cmd.regist, toTrance);
                    if (toTrance)
                        BattleAchievement.UpdateTranceStatus();
                }
                foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(target))
                {
                    saFeature.TriggerOnAbility(v, "BattleScriptStart", true);
                    saFeature.TriggerOnAbility(v, "BattleScriptEnd", true);
                    saFeature.TriggerOnAbility(v, "EffectDone", true);
                }
                return;
            }
            case BattleCommandId.Item:
            case BattleCommandId.AutoPotion:
            default:
                SBattleCalculator.CalcMain(caster, target, cmd, sfxThread);
                return;
        }
    }

    public static void DispSelectCursor(FF9StateGlobal sys, FF9StateBattleSystem btlsys, BTL_DATA btl)
    {
        GameObject cursorGo = btlsys.s_cur;
        Vector3 pos = btl.gameObject.transform.position;
        Vector3 rot = cursorGo.transform.localRotation.eulerAngles;
        cursorGo.transform.localPosition = new Vector3(pos.x, pos.y + btl.height, pos.z);
        Single angley = (Single)((((btlsys.btl_cnt & 15) << 8) + 1265) % 4096 / 4096f * 360f);
        cursorGo.transform.localRotation = Quaternion.Euler(rot.x, -angley, rot.z);
    }

    private static void DecideSummonType(CMD_DATA cmd)
    {
        AchievementState achievement = FF9StateSystem.Achievement;
        BattleAbilityId abilId = btl_util.GetCommandMainActionIndex(cmd);
        if (abilId == BattleAbilityId.Shiva && !achievement.summon_shiva ||
            abilId == BattleAbilityId.Ifrit && !achievement.summon_ifrit ||
            abilId == BattleAbilityId.Ramuh && !achievement.summon_ramuh ||
            abilId == BattleAbilityId.Atomos && !achievement.summon_atomos ||
            abilId == BattleAbilityId.Odin && !achievement.summon_odin ||
            abilId == BattleAbilityId.Leviathan && !achievement.summon_leviathan ||
            abilId == BattleAbilityId.Bahamut && !achievement.summon_bahamut ||
            abilId == BattleAbilityId.Ark && !achievement.summon_arc ||
            abilId == BattleAbilityId.Carbuncle1 && !achievement.summon_carbuncle_haste ||
            abilId == BattleAbilityId.Carbuncle2 && !achievement.summon_carbuncle_protect ||
            abilId == BattleAbilityId.Carbuncle3 && !achievement.summon_carbuncle_reflector ||
            abilId == BattleAbilityId.Carbuncle4 && !achievement.summon_carbuncle_shell ||
            abilId == BattleAbilityId.Fenrir1 && !achievement.summon_fenrir_earth ||
            abilId == BattleAbilityId.Fenrir2 && !achievement.summon_fenrir_wind ||
            abilId == BattleAbilityId.Phoenix && !achievement.summon_phoenix ||
            abilId == BattleAbilityId.Madeen && !achievement.summon_madeen)
            return;

        if (cmd.regist.cur.mp > cmd.aa.MP * 2)
        {
            if (Comn.random8() < 230)
                cmd.info.short_summon = 1;
        }
        else
        {
            if (Comn.random8() < 170)
                cmd.info.short_summon = 1;
        }
    }

    private static Boolean DecideMagicSword(BattleUnit attacker, BattleMagicSwordSet mgSet)
    {
        if (attacker.IsUnderAnyStatus(BattleStatusConst.Immobilized | mgSet.BeneficiaryBlockingStatus))
        {
            UIManager.Battle.SetBattleFollowMessage(BattleMesages.CombinationFailed);
            return false;
        }

        BattleUnit supporter = FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits().FirstOrDefault(unit => unit.PlayerIndex == mgSet.Supporter);
        if (supporter != null && !supporter.IsUnderAnyStatus(BattleStatusConst.Immobilized | mgSet.SupporterBlockingStatus))
            return true;

        UIManager.Battle.SetBattleFollowMessage(BattleMesages.CombinationFailed);
        return false;
    }

    private static void DecideMeteor(CMD_DATA cmd)
    {
        // Dummied
        if (cmd.regist.level / 2 + cmd.regist.elem.wpr >= Comn.random16() % 100)
            return;
        cmd.info.meteor_miss = 1;
    }

    private static Boolean ConsumeMp(BTL_DATA btl, Int32 mp)
    {
        if (btl == null)
            return false;

        BattleUnit unit = new BattleUnit(btl);

        if (unit.CurrentMp < mp)
            return false;

        if (!FF9StateSystem.Battle.isDebug && (btl.bi.player == 0 || !FF9StateSystem.Settings.IsHpMpFull))
            btl.cur.mp = (UInt32)(btl.cur.mp - mp);

        return true;
    }

    public static List<BattleCommandId> half_trance_cmd_list = new List<BattleCommandId>
    {
        BattleCommandId.DoubleWhiteMagic,
        BattleCommandId.DoubleBlackMagic
    };

    public const Int32 cmd_delay_max = 10;
    public static Int32 next_cmd_delay;
}
