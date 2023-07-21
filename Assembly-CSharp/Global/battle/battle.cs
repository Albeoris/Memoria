using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using UnityEngine;
using Memoria;
using Memoria.Data;

// ReSharper disable UnusedParameter.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable EmptyConstructor
// ReSharper disable ArrangeStaticMemberQualifier
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable UnusedParameter.Local
// ReSharper disable ClassNeverInstantiated.Global

public static class battle
{
    public const SByte BTL_SYSTEM_FADE_RATE = 32;
    public const Byte BTL_MAP_JUMP_ON = 1;
    public const Byte BTL_LOAD_END_SONG = 2;
    public const Byte BTL_FLAG_TONZURA = 4;
    public const Byte BTL_CONTI_FLD_SONG = 8;
    public const Byte BTL_SONG_FADEOUT = 16;
    public const Byte BTL_PLAY_END_SONG = 32;
    public const Byte BTL_END_LOAD = 64;
    public const Byte BTL_FADE_IN_COUNT = 32;
    public const Byte BTL_FADE_OUT_COUNT = 32;
    public static BONUS btl_bonus;
    public static Boolean isAlreadyShowTutorial;
    public static Boolean isSpecialTutorialWindow;

    public static Byte TRANCE_GAUGE_FLAG => FF9StateSystem.EventState.gEventGlobal[16];

    public static Byte GARNET_DEPRESS_FLAG => FF9StateSystem.EventState.gEventGlobal[17];

    public static Byte GARNET_SUMMON_FLAG => FF9StateSystem.EventState.gEventGlobal[18];

    public static Byte TONBERI_COUNT => FF9StateSystem.EventState.gEventGlobal[192];

    public static Byte SUMMON_RAY_FLAG => FF9StateSystem.EventState.gEventGlobal[193];

    public static Byte SUMMON_ALL_LONG_FLAG => FF9StateSystem.EventState.gEventGlobal[207];

    static battle()
    {
        battle.btl_bonus = new BONUS();
        battle.isAlreadyShowTutorial = false;
    }

    public static void InitBattle()
    {
        SFX.SetCameraPhase(1);
        FF9StateGlobal ff9 = FF9StateSystem.Common.FF9;
        ff9.btl_flag = 0;
        // "steiner_state" is never activated anyways, in none of any "ENCOUNT" or "ENCOUNT2" event script line
        //ff9.GetPlayer(CharacterId.Steiner).info.serial_no = ff9.steiner_state == 0 ? CharacterSerialNumber.STEINER_OUTDOOR : CharacterSerialNumber.STEINER_INDOOR;
        ff9.btl_result = 0;
        btl_sys.InitBattleSystem();
        btl2d.Btl2dInit();
        ++FF9StateSystem.Common.FF9.party.battle_no;
    }

    public static void InitBattleMap()
    {
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        battlebg.nf_InitBattleBG(ff9Battle.map.btlBGInfoPtr, ff9Battle.map.btlBGTexAnimPtr);
        ff9Battle.btl_load_status |= ff9btl.LOAD_BBG;
        btl_cmd.InitCommandSystem(ff9Battle);
        btl_cmd.InitSelectCursor(ff9Battle);
        btlseq.SetupBattleScene();
        battle.btl_bonus.Event = ff9Battle.btl_scene.Info.AfterEvent;
        if (!FF9StateSystem.Battle.isDebug)
            PersistenSingleton<EventEngine>.Instance.ServiceEvents();
        SceneDirector.FF9Wipe_FadeInEx(32);
        ff9Battle.btl_phase = 2;
        battle.isSpecialTutorialWindow = false;
    }

    public static UInt32 BattleMain()
    {
        FF9StateGlobal ff9 = FF9StateSystem.Common.FF9;
        FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
        SFXDataCamera.UpdateCamera();
        if (ff9Battle.btl_phase != 4 && ff9Battle.s_cur != null && ff9Battle.s_cur.activeSelf)
            ff9Battle.s_cur.SetActive(false);
        switch (ff9Battle.btl_phase)
        {
            case 1:
                battle.BattleIdleLoop(ff9, ff9Battle);
                break;
            case 2:
                battle.BattleLoadLoop(ff9, ff9Battle);
                break;
            case 3:
                if (battle.BattleIdleLoop(ff9, ff9Battle))
                {
                    if (ff9.btlMapNo == 336 && !battle.isAlreadyShowTutorial) // Masked Man
                    {
                        PersistenSingleton<UIManager>.Instance.TutorialScene.DisplayMode = TutorialUI.Mode.Battle;
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
                        battle.isAlreadyShowTutorial = true;
                    }
                    if (!FF9StateSystem.Battle.isTutorial)
                    {
                        ff9Battle.btl_phase = 4;
                        ff9Battle.btl_cnt = ff9Battle.btl_cnt & 15;
                        ff9Battle.player_load_fade = 0;
                        for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
                        {
                            UInt16 startSfx;
                            if (next.bi.player == 0 && (startSfx = BTL_SCENE.BtlGetStartSFX()) != UInt16.MaxValue)
                            {
                                btl_util.SetBattleSfx(next, startSfx, 127);
                                break;
                            }
                        }
                        if (battle.SUMMON_RAY_FLAG == 0)
                            UIManager.Battle.FF9BMenu_EnableMenu(true);
                        if (ff9Battle.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_BACK_ATTACK)
                            UIManager.Battle.SetBattleFollowMessage(BattleMesages.BackAttack);
                        else if (ff9Battle.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_FIRST_ATTACK)
                            UIManager.Battle.SetBattleFollowMessage(BattleMesages.PreEmptiveStrike);
                    }
                }
                break;
            case 4:
                battle.BattleMainLoop(ff9, ff9Battle);
                break;
            case 5:
                if (!FF9StateSystem.Battle.isDebug)
                    UIManager.Battle.FF9BMenu_EnableMenu(false);
                battle.BattleTrailingLoop(ff9, ff9Battle);
                if (ff9Battle.btl_seq != 3)
                    ff9Battle.btl_escape_key = 0;
                break;
            case 6:
                battle.BattleIdleLoop(ff9, ff9Battle);
                if (ff9Battle.btl_seq == 1)
                {
                    SceneDirector.FF9Wipe_FadeOutEx(32);
                    ++ff9Battle.btl_seq;
                    break;
                }
                if (ff9Battle.btl_seq == 2 && ff9Battle.btl_fade_time++ > 32)
                    return btl_sys.ManageBattleEnd(ff9Battle);
                break;
            case 7:
            case 8:
                if (battle.BattleIdleLoop(ff9, ff9Battle))
                {
                    ff9.btl_flag |= 64;
                    if (ff9Battle.btl_seq == 0)
                    {
                        if (ff9.btl_result == 3 || ff9.btl_result == 6)
                        {
                            if (ff9Battle.btl_scene.Info.NoGameOver)
                                BattleVoice.TriggerOnBattleInOut("Defeated");
                            else
                                BattleVoice.TriggerOnBattleInOut("GameOver");
                        }
                        if ((!ff9Battle.btl_scene.Info.WinPose && ff9.btl_result == 1) || ff9.btl_result == 2)
                            BattleVoice.TriggerOnBattleInOut("Victory");
                        if (ff9.btl_result != 5)
                            SceneDirector.FF9Wipe_FadeOutEx(32);
                        ++ff9Battle.btl_seq;
                        break;
                    }
                    if (ff9Battle.btl_seq == 1 && ff9Battle.btl_fade_time++ > 32)
                        return btl_sys.ManageBattleEnd(ff9Battle);
                }
                break;
        }
        if (!FF9StateSystem.Battle.isDebug)
            PersistenSingleton<EventEngine>.Instance.ServiceEvents();
        if (ff9Battle.btl_phase == 4 && ff9Battle.cur_cmd_list.Count == 0 && btl_scrp.GetBattleID(1u) == 0)
        {
            // Automatically end a battle when there is no enemy anymore, typically they escaped (warning: enemies that are not targetable but still present don't trigger the end)
            UIManager.Battle.FF9BMenu_EnableMenu(false);
            ff9Battle.btl_escape_key = 0;
            ff9Battle.cmd_status &= 65533;
            ff9Battle.btl_phase = 5;
            ff9Battle.btl_seq = 2;
            btl_cmd.KillAllCommand(ff9Battle);
            for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
                next.bi.cmd_idle = 0;
            // Don't wait for btl_phase to turn to 1
            ff9.btl_result = battle.btl_bonus.exp > 0 ? (Byte)1 : (Byte)7;
            if (ff9.btl_result == 1 && ff9Battle.btl_scene.Info.WinPose)
            {
                ff9Battle.btl_phase = 5;
                ff9Battle.btl_seq = 4;
            }
            else
            {
                if (ff9.btl_result == 1)
                    ff9.btl_result = 2;
                ff9Battle.btl_phase = 8;
                ff9Battle.btl_seq = 0;
                if (ff9.btl_result == 7) // Enemy flee, such as (Magic) Vice or friendly monsters when attacked
                    BattleVoice.TriggerOnBattleInOut("EnemyEscape");
            }
        }
        ++ff9Battle.btl_cnt;
        return 0;
    }

    private static void BattleMainLoop(FF9StateGlobal sys, FF9StateBattleSystem btlsys)
    {
        //uint id = sys.id;
        FF9StateSystem.Settings.SetTranceFull();
        Boolean flag = false;
        for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
        {
            if (next.bi.disappear == 0)
                btlseq.DispCharacter(next);
            if (!FF9StateSystem.Battle.isDebug && UIManager.Battle.CurrentPlayerIndex != -1 && next.btl_id == 1 << UIManager.Battle.CurrentPlayerIndex && (btlsys.cmd_status & 2) != 0 && (next.flags & geo.GEO_FLAGS_CLIP) == 0)
            {
                flag = true;
                btl_cmd.DispSelectCursor(sys, btlsys, next);
            }

            btl_para.CheckPointData(next);

            // ============ Warning ============
            if (Configuration.Battle.Speed == 0 || Configuration.Battle.Speed >= 3 || next.sel_mode != 0 || next.sel_menu != 0 || next.cur.hp == 0 || next.bi.atb == 0)
                btl_stat.CheckStatusLoop(next, false);
            // =================================
        }
        if (flag)
        {
            if (!btlsys.s_cur.activeSelf)
                btlsys.s_cur.SetActive(true);
        }
        else
            btlsys.s_cur.SetActive(false);
        btl_cmd.CommandEngine(btlsys);
        battle.BattleSubSystem(sys, btlsys);
    }

    private static Boolean BattleIdleLoop(FF9StateGlobal sys, FF9StateBattleSystem btlsys)
    {
        //uint id = sys.id;
        Boolean flag = true;
        foreach (BattleUnit next in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            BTL_DATA data = next.Data;
            if (data.bi.disappear == 0)
            {
                btlseq.DispCharacter(data);
                if (btlsys.btl_phase == 3)
                {
                    //data.bi.stop_anim = 0;
                    //if (data.evt.animFrame >= GeoAnim.geoAnimGetNumFrames(data))
                    //{
                    //    if (!next.IsUnderStatus(BattleStatus.Death))
                    //        btl_mot.setMotion(next, data.bi.def_idle);
                    //    data.evt.animFrame = 0;
                    //}
                    if (!next.IsUnderAnyStatus(BattleStatus.Petrify) && !btl_mot.checkMotion(data, data.bi.def_idle) && !btl_mot.checkMotion(data, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE))
                        flag = false;
                }
                //else if (btlsys.btl_phase == 6 && next.IsPlayer && !next.IsUnderStatus(BattleStatus.BattleEnd) && btlsys.btl_scene.Info.WinPose != 0 && (next.Player.Data.info.win_pose != 0 && data.evt.animFrame >= GeoAnim.geoAnimGetNumFrames(data)))
                //{
                //    btl_mot.setMotion(next.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_WIN_LOOP);
                //    data.evt.animFrame = 0;
                //}
                btl_stat.SetStatusVfx(next);
            }
            btl_mot.DieSequence(data);
        }
        if (btlsys.btl_phase == 7 && !btlsys.btl_scene.Info.NoGameOver && !btl_util.ManageBattleSong(sys, 30, 6))
            flag = false;
        battle.BattleSubSystem(sys, btlsys);
        return flag;
    }

    private static void BattleTrailingLoop(FF9StateGlobal sys, FF9StateBattleSystem btlsys)
    {
        //uint id = sys.id;
        Boolean proceedEnd = true;
        if (SFX.IsRunning() || btlsys.cmd_queue.next != null || btlsys.cur_cmd != null)
            proceedEnd = false;

        foreach (BattleUnit next in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            BTL_DATA data = next.Data;
            if (data.bi.disappear == 0)
            {
                btlseq.DispCharacter(data);
                switch (btlsys.btl_seq)
                {
                    case 0:
                        btl_para.CheckPointData(data);
                        if ((!next.IsPlayer && !next.IsUnderAnyStatus(BattleStatusConst.BattleEnd)) || (next.IsPlayer && next.IsUnderStatus(BattleStatus.Death) && !btl_mot.checkMotion(data, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE)))
                            proceedEnd = false;
                        break;
                    case 1:
                        if (next.IsPlayer && !btl_mot.checkMotion(data, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE) && !next.IsUnderAnyStatus(BattleStatusConst.BattleEnd))
                            proceedEnd = false;
                        break;
                    case 2:
                        if (btlsys.cmd_queue.next != null && btlsys.cur_cmd == null && !btl_cmd.CheckSpecificCommand2(BattleCommandId.SysTrans) && !btl_cmd.CheckSpecificCommand2(BattleCommandId.SysDead) && !btl_cmd.CheckSpecificCommand2(BattleCommandId.SysReraise) && !btl_cmd.CheckSpecificCommand2(BattleCommandId.SysStone))
                            proceedEnd = true;
                        btl_para.CheckPointData(data);
                        if (next.IsPlayer)
                            next.TryRemoveStatuses(BattleStatusConst.CancelEvent);
                        if (btlsys.cur_cmd != null)
                            proceedEnd = false;
                        if (next.IsUnderStatus(BattleStatus.Death))
                        {
                            //if (data.die_seq == 0 && !btl_cmd.CheckUsingCommand(data.cmd[2]))
                            //    btl_cmd.SetCommand(data.cmd[2], BattleCommandId.SysDead, 0U, data.btl_id, 0U);
                            if (data.die_seq == 0)
                                data.die_seq = 1;
                            if (next.IsPlayer && data.die_seq != 6 || !next.IsPlayer && data.die_seq != 6)
                                proceedEnd = false;
                            break;
                        }
                        if (!btl_stat.CheckStatus(data, BattleStatusConst.Immobilized) && !btl_mot.checkMotion(data, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL) && !btl_mot.checkMotion(data, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING))
                            proceedEnd = false;
                        break;
                    case 3:
                        btl_cmd.KillAllCommand(btlsys);
                        if (next.IsUnderStatus(BattleStatus.Death))
                        {
                            if (data.die_seq == 0 && !btl_util.IsBtlBusy(data, btl_util.BusyMode.CASTER))
                                data.die_seq = 1;
                            if (!btl_mot.checkMotion(data, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE) || btl_cmd.CheckSpecificCommand(data, BattleCommandId.SysReraise))
                                proceedEnd = false;
                            break;
                        }
                        if (next.IsPlayer && !btl_stat.CheckStatus(data, BattleStatusConst.CannotEscape))
                        {
                            FF9StateSystem.Battle.isFade = true;
                            data.pos[2] -= 100f;
                            btl_util.SetFadeRate(data, btlsys.btl_escape_fade);
                            if (btlsys.btl_escape_fade <= 0)
                            {
                                data.SetDisappear(true, 5);
                                break;
                            }
                            proceedEnd = false;
                            if (btlsys.btl_escape_fade == 32)
                            {
                                btlsnd.ff9btlsnd_sndeffect_play(2906, 0, SByte.MaxValue, 128);
                                btlsnd.ff9btlsnd_sndeffect_play(2907, 0, SByte.MaxValue, 128);
                                btlsnd.ff9btlsnd_sndeffect_play(2908, 0, SByte.MaxValue, 128);
                                btlsys.btl_escape_fade -= 2;
                                BattleVoice.TriggerOnBattleInOut("Flee");
                            }
                        }
                        break;
                }
                btl_stat.SetStatusVfx(next);
            }
            btl_mot.DieSequence(data);
        }
        if (btlsys.btl_seq == 3 && btlsys.btl_escape_fade < 32 && btlsys.btl_escape_fade != 0)
            btlsys.btl_escape_fade -= 2;
        if (proceedEnd)
        {
            switch (btlsys.btl_seq)
            {
                case 0:
                case 4:
                    sys.btl_flag |= 64;
                    sys.btl_result = 1;
                    if (btlsys.btl_scene.Info.WinPose)
                    {
                        if (!btl_util.ManageBattleSong(sys, 30, 5))
                            break;
                        BattleVoice.TriggerOnBattleInOut("VictoryPose");
                        btlsys.btl_phase = 6;
                        for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                        {
                            if (next.bi.player != 0)
                            {
                                /*int num2 = (int)*/
                                btl_stat.RemoveStatuses(next, BattleStatusConst.VictoryClear);
                                if (!btl_stat.CheckStatus(next, BattleStatusConst.BattleEndFull))
                                {
                                    if (next.cur.hp > 0)
                                    {
                                        Int32 num3 = btl_mot.GetDirection(next);
                                        next.rot.eulerAngles = new Vector3(next.rot.eulerAngles.x, num3, next.rot.eulerAngles.z);
                                        if (!next.is_monster_transform)
                                            next.bi.def_idle = btl_stat.CheckStatus(next, BattleStatusConst.IdleDying) || next.special_status_old ? (Byte)1 : (Byte)0;
                                        next.bi.cmd_idle = 0;
                                        btl_mot.SetDefaultIdle(next);
                                        //if (btl_util.getPlayerPtr(next).info.win_pose != 0)
                                        //    btl_mot.setMotion(next, BattlePlayerCharacter.PlayerMotionIndex.MP_WIN);
                                        //else
                                        //    btl_mot.setMotion(next, next.bi.def_idle);
                                        //next.evt.animFrame = 0;
                                    }
                                    else
                                    {
                                        /*int num4 = (int)*/
                                        btl_stat.AlterStatus(next, BattleStatus.Death);
                                    }
                                }
                            }
                        }
                        SFX.SetCamera(2);
                    }
                    else if (btlsys.btl_scene.Info.FieldBGM)
                    {
                        sys.btl_flag |= 8;
                    }
                    btlsys.btl_phase = 6;
                    break;
                case 1:
                    sys.btl_result = 3;
                    btlsys.btl_phase = 7;
                    break;
                case 2:
                    if (!btlsys.btl_scene.Info.WinPose && btlsys.btl_scene.Info.FieldBGM)
                        sys.btl_flag |= 8;
                    btlsys.btl_phase = 1;
                    btl_cmd.KillAllCommand(btlsys);
                    btl_cmd.InitCommandSystem(btlsys);
                    for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                    {
                        btl_cmd.InitCommand(next);
                        if (next.bi.player == 0)
                        {
                            Int32 num2 = btl_mot.GetDirection(next);
                            next.rot.eulerAngles = new Vector3(next.rot.eulerAngles.x, num2, next.rot.eulerAngles.z);
                        }
                    }
                    break;
                case 3:
                    UInt32 gil = (UInt32)battle.btl_bonus.gil;
                    sys.btl_flag |= 64;
                    for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                        if (next.bi.player == 0)
                            gil += btl_util.getEnemyPtr(next).bonus_gil;
                    if (!btlsys.btl_scene.Info.WinPose && btlsys.btl_scene.Info.FieldBGM)
                        sys.btl_flag |= 8;
                    sys.btl_result = 4;
                    btlsys.btl_phase = 8;
                    if ((sys.btl_flag & 4) != 0)
                    {
                        UInt32 gilLost = gil / 10U;
                        if (sys.party.gil > gilLost)
                        {
                            sys.party.gil -= gilLost;
                        }
                        else
                        {
                            gilLost = sys.party.gil;
                            sys.party.gil = 0U;
                        }
                        UIManager.Battle.SetBattleFollowMessage(BattleMesages.DroppedGil, gilLost);
                    }
                    break;
            }
            if (btlsys.btl_phase != 5)
                btlsys.btl_seq = btlsys.btl_phase != 6 || btlsys.btl_scene.Info.WinPose ? (Byte)0 : (Byte)1;
        }
        else
            btl_cmd.CommandEngine(btlsys);
        battle.BattleSubSystem(sys, btlsys);
    }

    private static void BattleLoadLoop(FF9StateGlobal sys, FF9StateBattleSystem btlsys)
    {
        //uint id = sys.id;
        Boolean noDelayFade = FF9StateSystem.Settings.cfg.skip_btl_camera == 0 && FF9StateSystem.Battle.isRandomEncounter;
        btlsys.attr |= ff9btl.ATTR.LOADNPC;
        btlsys.attr |= ff9btl.ATTR.LOADCHR;
        if ((noDelayFade || btlsys.btl_cnt > 20) && (btlsys.attr & ff9btl.ATTR.LOADNPC) != 0 && (btlsys.btl_load_status & ff9btl.LOAD_FADENPC) == 0 && (btlsys.btl_load_status & ff9btl.LOAD_INITNPC) != 0)
            btlsys.btl_load_status |= ff9btl.LOAD_FADENPC;
        else if ((noDelayFade || btlsys.btl_cnt > 40) && (btlsys.attr & ff9btl.ATTR.LOADCHR) != 0 && (btlsys.btl_load_status & ff9btl.LOAD_FADECHR) == 0 && (btlsys.btl_load_status & ff9btl.LOAD_INITCHR) != 0)
            btlsys.btl_load_status |= ff9btl.LOAD_FADECHR;

        foreach (BattleUnit next in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
        {
            BTL_DATA data = next.Data;
            if (data.bi.disappear == 0)
            {
                btlseq.DispCharacter(data);
                if (next.IsPlayer)
                    btl_util.SetFadeRate(data, btlsys.player_load_fade);
                else if (!next.IsSlave)
                    btl_util.SetFadeRate(data, btlsys.enemy_load_fade);
                btl_stat.SetStatusVfx(next);
            }
        }
        if ((btlsys.btl_load_status & ff9btl.LOAD_NPC) != 0)
            btlseq.Sequencer();
        if ((btlsys.attr & ff9btl.ATTR.LOADBBG) != 0)
            battlebg.nf_BattleBG();
        if ((btlsys.attr & ff9btl.ATTR.LOADNPC) != 0 && (btlsys.btl_load_status & ff9btl.LOAD_NPC) == 0)
        {
            if ((btlsys.btl_load_status & ff9btl.LOAD_INITNPC) == 0)
            {
                if (!FF9TextTool.IsLoading)
                {
                    btl_init.InitEnemyData(btlsys);
                    btl_init.OrganizeEnemyData(btlsys);
                }
            }
            else if ((btlsys.btl_load_status & ff9btl.LOAD_FADENPC) != 0)
            {
                if (btlsys.enemy_load_fade >= 32)
                    btlsys.btl_load_status |= ff9btl.LOAD_NPC;
                else
                    btlsys.enemy_load_fade += (SByte)(noDelayFade ? 4 : 2);
            }
        }
        if ((btlsys.attr & ff9btl.ATTR.LOADCHR) == 0 || (btlsys.btl_load_status & ff9btl.LOAD_CHR) != 0)
            return;
        if ((btlsys.btl_load_status & ff9btl.LOAD_INITCHR) == 0)
        {
            if (FF9TextTool.IsLoading)
                return;
            btl_init.InitPlayerData(btlsys);
            SettingsState settings = FF9StateSystem.Settings;
            settings.SetATBFull();
            settings.SetHPFull();
            for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                BattleTexAnimWatcher.ForcedNonCullingMesh(next.gameObject);
        }
        else
        {
            if ((btlsys.btl_load_status & ff9btl.LOAD_FADECHR) == 0)
                return;
            if (btlsys.player_load_fade < 32)
            {
                btlsys.player_load_fade += (SByte)(noDelayFade ? 8 : 4);
                return;
            }
            btlsys.btl_load_status |= ff9btl.LOAD_CHR;
            BattleVoice.TriggerOnBattleInOut("BattleStart");
        }
    }

    private static void BattleSubSystem(FF9StateGlobal sys, FF9StateBattleSystem btlsys)
    {
        if (btlsys.enemy_die != 0)
            --btlsys.enemy_die;
        btlseq.Sequencer();
        battlebg.nf_BattleBG();
        SFX.UpdatePlugin();
        UnifiedBattleSequencer.Loop();
        btl2d.Btl2dMain();
        HonoluluBattleMain.battleSPS.GenerateSPS();
    }

    public static void Log(String str)
    {
        //Debug.Log(str);
    }

    public static void ff9ShutdownStateBattleResult()
    {
        if ((FF9StateSystem.Common.FF9.btl_flag & 8) != 0)
            return;

        btlsnd.ff9btlsnd_song_vol_intplall(120, 0);
        SoundLib.GetAllSoundDispatchPlayer().StopCurrentSong(120);
    }
}