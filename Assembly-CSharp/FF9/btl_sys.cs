using System;
using Memoria;
using Memoria.Data;
using Memoria.Speedrun;
using System.Collections.Generic;
using NCalc;

// ReSharper disable InconsistentNaming
// ReSharper disable EmptyConstructor
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace FF9
{
    public class btl_sys
    {
        public const Int32 fleeScriptId = 0056;

        public btl_sys()
        {
        }

        public static void InitBattleSystem()
        {
            FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
            ff9Battle.btl_list.next = null;
            ff9Battle.btl_cnt = 0;
            ff9Battle.btl_phase = 0;
            ff9Battle.btl_seq = 0;
            ff9Battle.btl_fade_time = 0;
            ff9Battle.btl_escape_key = 0;
            ff9Battle.btl_escape_fade = 32;
            ff9Battle.btl_load_status = 0;
            ff9Battle.player_load_fade = 0;
            ff9Battle.enemy_load_fade = 0;
            ff9Battle.phantom_no = BattleAbilityId.Void;
            ff9Battle.phantom_cnt = 0;
            ff9Battle.enemy_die = 0;
            battle.btl_bonus.member_flag = 0;
            ClearBattleBonus();
        }

        public static void ClearBattleBonus()
        {
            BONUS btlBonus = battle.btl_bonus;
            btlBonus.gil = 0;
            btlBonus.exp = 0U;
            btlBonus.ap = 0;
            btlBonus.item.Clear();
            btlBonus.card = TetraMasterCardId.NONE;
            btlBonus.escape_gil = false;
        }

        public static void CheckBattlePhase(BTL_DATA btl)
        {
            FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
            Int32 dieSeqEnd = 6; /*btl.bi.player == 0 ? 6 : 6*/
            for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
                if (next.bi.player == btl.bi.player && (!btl_stat.CheckStatus(next, BattleStatusConst.BattleEndFull) || Status.checkCurStat(next, BattleStatus.Death) && next.die_seq != dieSeqEnd))
                    return;
            if (btl.bi.player == 0)
            {
                Boolean playerStillAliver = false;
                for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
                {
                    if (next.bi.player != 0 && (!btl_stat.CheckStatus(next, BattleStatusConst.BattleEndFull) || (next.cur.hp == 0 || Status.checkCurStat(next, BattleStatus.Death)) && btl_stat.CheckStatus(next, BattleStatus.AutoLife) || btl_cmd.CheckSpecificCommand(next, BattleCommandId.SysReraise)))
                    {
                        playerStillAliver = true;
                        break;
                    }
                }
                if (!playerStillAliver)
                    return;
                if (ff9Battle.btl_seq != 1)
                    ff9Battle.btl_seq = 0;
            }
            else
            {
                for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
                {
                    CharacterSerialNumber serialNo = btl_util.getSerialNumber(next);
                    if (serialNo == CharacterSerialNumber.EIKO_FLUTE || serialNo == CharacterSerialNumber.EIKO_KNIFE)
                    {
                        if (!btl_stat.CheckStatus(next, BattleStatusConst.NoRebirthFlame))
                        {
                            if (btl_cmd.CheckSpecificCommand(next, BattleCommandId.SysLastPhoenix))
                                return;
                            Boolean procRebirthFlame = false;
                            if (!String.IsNullOrEmpty(Configuration.Battle.RebirthFlame))
                            {
                                Expression e = new Expression(Configuration.Battle.RebirthFlame);
                                e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                                e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                                NCalcUtility.InitializeExpressionUnit(ref e, new BattleUnit(next), "Caster");
                                procRebirthFlame = NCalcUtility.EvaluateNCalcCondition(e.Evaluate());
                            }
                            else
                            {
                                procRebirthFlame = ff9item.FF9Item_GetCount(RegularItem.PhoenixPinion) > Comn.random8();
                            }
                            if (procRebirthFlame)
                            {
                                UIManager.Battle.FF9BMenu_EnableMenu(true);
                                btl_cmd.SetCommand(next.cmd[0], BattleCommandId.SysLastPhoenix, (Int32)BattleAbilityId.RebirthFlame, btl_scrp.GetBattleID(0U), 1U);
                                return;
                            }
                        }
                        break;
                    }
                }
                ff9Battle.btl_seq = 1;
                UIManager.Battle.SetBattleFollowMessage(BattleMesages.Annihilated);
            }
            UIManager.Battle.FF9BMenu_EnableMenu(false);
            ff9Battle.btl_phase = 5;
            btl_cmd.KillAllCommand(ff9Battle);
        }

        public static void CheckBattleMenuOff(BattleUnit btl)
        {
            FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
            BattleUnit btl1 = null;

            foreach (BattleUnit next in ff9Battle.EnumerateBattleUnits())
            {
                if (next.PlayerIndex == CharacterId.Eiko)
                    btl1 = next;

                if (next.IsPlayer == btl.IsPlayer && (!next.IsUnderAnyStatus(BattleStatusConst.BattleEndFull) || (next.CurrentHp == 0 || next.IsUnderStatus(BattleStatus.Death)) && next.IsUnderAnyStatus(BattleStatus.AutoLife) || btl_cmd.CheckSpecificCommand(next.Data, BattleCommandId.SysReraise)))
                    return;
            }

            if (btl1 != null && btl_cmd.CheckSpecificCommand(btl1.Data, BattleCommandId.SysLastPhoenix))
                return;

            UIManager.Battle.FF9BMenu_EnableMenu(false);
            btl_cmd.KillNormalCommand(ff9Battle);
            ff9Battle.btl_escape_key = 0;
        }

        public static void CheckForecastMenuOff(BTL_DATA btl)
        {
            FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
            for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
                if (next.bi.player == 0 && !Status.checkCurStat(next, BattleStatus.Death))
                    return;
            AutoSplitterPipe.SignalBattleWin();
            UIManager.Battle.FF9BMenu_EnableMenu(false);
            btl_cmd.KillNormalCommand(ff9Battle);
            ff9Battle.btl_escape_key = 0;
        }

        public static UInt32 ManageBattleEnd(FF9StateBattleSystem btlsys)
        {
            FF9StateGlobal ff9 = FF9StateSystem.Common.FF9;
            FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
            switch (ff9.btl_result)
            {
                case 3: // Scripted defeat
                case 6: // Normal defeat
                    if (!btlsys.btl_scene.Info.NoGameOver)
                    {
                        ff9.btl_result = 6;
                        break;
                    }
                    // Non-critical battles, such as battles during the Festival of the Hunt
                    for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
                        if (next.bi.player != 0)
                            SavePlayerData(next, false);
                    break;
                default: // Battle has been won or interrupted
                    if (ff9.btl_result == 1 || ff9.btl_result == 2)
                        BattleAchievement.UpdateEndBattleAchievement();
                    for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
                        if (next.bi.player != 0)
                            SavePlayerData(next, false);
                    break;
            }
            return 1;
        }

        public static battle_start_type_tags StartType(BTL_SCENE_INFO info)
        {
            battle_start_type_tags start_type = battle_start_type_tags.BTL_START_NORMAL_ATTACK;
            if (info.BackAttack)
                start_type = battle_start_type_tags.BTL_START_BACK_ATTACK;
            else if (info.Preemptive)
                start_type = battle_start_type_tags.BTL_START_FIRST_ATTACK;
            Int32 backAttackChance = 24;
            Int32 preemptiveChance = 16;
            Int32 preemptivePriority = 0;
            for (Int32 i = 0; i < 4; i++)
            {
                PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
                if (player != null)
                    foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(player))
                        saFeature.TriggerOnBattleStart(ref backAttackChance, ref preemptiveChance, ref preemptivePriority);
            }
            if (!info.SpecialStart && !info.BackAttack && !info.Preemptive)
            {
                Boolean bAttack = Comn.random8() < backAttackChance;
                Boolean preemptive = Comn.random8() < preemptiveChance;
                if (preemptive && preemptivePriority > 0)
                    start_type = battle_start_type_tags.BTL_START_FIRST_ATTACK;
                else if (bAttack)
                    start_type = battle_start_type_tags.BTL_START_BACK_ATTACK;
                else if (preemptive)
                    start_type = battle_start_type_tags.BTL_START_FIRST_ATTACK;
            }
            if (start_type == battle_start_type_tags.BTL_START_BACK_ATTACK)
                BattleAchievement.UpdateBackAttack();

            return start_type;
        }

        public static void SetBonus(ENEMY enemy)
        {
            // Notes about item drops:
            // - Each defeated enemy can drop the item from the first item slot (100% by default) + 1 item out of the other slots (37.5%, 12.5% or 0.39% by default)
            // - Also, the first item slot is "per enemy" (each enemy can drop one independently) while the other slots are "per enemy type" (even with multiple enemies of the same type, only 1 item can be dropped from these slots)
            // (eg. a fight against 2 Red Dragons always drop 2 Ethers, but it can never drop more than 1 Sapphire)
            FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
            BONUS btlBonus = battle.btl_bonus;
            btlBonus.gil += (Int32)enemy.bonus_gil;
            btlBonus.exp += enemy.bonus_exp;
            if (enemy.bonus_item[3] != RegularItem.NoItem && Comn.random8() < enemy.bonus_item_rate[3])
            {
                btlBonus.item.Add(enemy.bonus_item[3]);
                for (Int32 i = 4; i < 8; i++)
                    if (ff9Battle.btl_data[i].btl_id != 0 && ff9Battle.btl_data[i].bi.player == 0 && btl_util.getEnemyTypePtr(ff9Battle.btl_data[i]) == enemy.et)
                        btl_util.getEnemyPtr(ff9Battle.btl_data[i]).bonus_item[3] = RegularItem.NoItem;
            }
            else if (enemy.bonus_item[2] != RegularItem.NoItem && Comn.random8() < enemy.bonus_item_rate[2])
            {
                btlBonus.item.Add(enemy.bonus_item[2]);
                for (Int32 i = 4; i < 8; i++)
                    if (ff9Battle.btl_data[i].btl_id != 0 && ff9Battle.btl_data[i].bi.player == 0 && btl_util.getEnemyTypePtr(ff9Battle.btl_data[i]) == enemy.et)
                        btl_util.getEnemyPtr(ff9Battle.btl_data[i]).bonus_item[2] = RegularItem.NoItem;
            }
            else if (enemy.bonus_item[1] != RegularItem.NoItem && Comn.random8() < enemy.bonus_item_rate[1])
            {
                btlBonus.item.Add(enemy.bonus_item[1]);
                for (Int32 i = 4; i < 8; i++)
                    if (ff9Battle.btl_data[i].btl_id != 0 && ff9Battle.btl_data[i].bi.player == 0 && btl_util.getEnemyTypePtr(ff9Battle.btl_data[i]) == enemy.et)
                        btl_util.getEnemyPtr(ff9Battle.btl_data[i]).bonus_item[1] = RegularItem.NoItem;
            }
            if (enemy.bonus_item[0] != RegularItem.NoItem && Comn.random8() < enemy.bonus_item_rate[0])
                btlBonus.item.Add(enemy.bonus_item[0]);
            if (btlBonus.card == TetraMasterCardId.NONE && enemy.bonus_card != TetraMasterCardId.NONE && Comn.random8() < enemy.bonus_card_rate)
                btlBonus.card = enemy.bonus_card;
        }

        public static void SavePlayerData(BTL_DATA btl, Boolean removingUnit)
        {
            if (btl.bi.player == 0)
                return;
            BattleUnit unit = new BattleUnit(btl);
            PLAYER player = btl_util.getPlayerPtr(btl);
            player.trance = unit.IsUnderAnyStatus(BattleStatus.Trance) ? (Byte)0 : btl.trance;
            btl_init.CopyPoints(player.cur, btl.cur);
            player.cur.hp = Math.Min(player.cur.hp, player.max.hp);
            player.cur.mp = Math.Min(player.cur.mp, player.max.mp);
            if (removingUnit)
                if (btl.cur.hp == 0)
                    btl_stat.AlterStatus(btl, BattleStatus.Death);
            btl_stat.SaveStatus(player, btl);
            ff9play.FF9Play_UpdateSerialNumber(player);
        }

        public static void DelCharacter(BTL_DATA btl)
        {
            for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list; btlData != null; btlData = btlData.next)
            {
                if (btlData.next == btl)
                {
                    BTL_DATA next = btlData.next;
                    btlData.next = next.next;
                    break;
                }
            }
            for (Int32 i = 0; i < UnifiedBattleSequencer.runningActions.Count; i++)
                if (UnifiedBattleSequencer.runningActions[i].cmd.tar_id == btl.btl_id || UnifiedBattleSequencer.runningActions[i].cmd.regist == btl)
                    UnifiedBattleSequencer.runningActions[i].Cancel();
        }

        public static void AddCharacter(BTL_DATA btl)
        {
            for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list; btlData != null; btlData = btlData.next)
            {
                if (btl.bi.line_no < btlData.next.bi.line_no)
                {
                    btl.next = btlData.next;
                    btlData.next = btl;
                    break;
                }
            }
        }

        public static void CheckEscape(Boolean calc_check)
        {
            FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
            if ((ff9Battle.cmd_status & 1) != 0)
                return;

            if (!ff9Battle.btl_scene.Info.Runaway)
            {
                UIManager.Battle.SetBattleFollowMessage(BattleMesages.CannotEscape);
            }
            else
            {
                if (calc_check && UIManager.Battle.FF9BMenu_IsEnableAtb())
                    SBattleCalculator.CalcMain(null, null, null, fleeScriptId);
            }
        }
    }
}