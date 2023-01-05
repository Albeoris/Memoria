using System;
using Memoria;
using Memoria.Data;
using System.Collections.Generic;

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
            btlBonus.card = Byte.MaxValue;
            btlBonus.escape_gil = false;
        }

        public static void CheckBattlePhase(BTL_DATA btl)
        {
            FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
            Int32 num1 = 6; /*btl.bi.player == 0 ? 6 : 6*/
            for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
                if (next.bi.player == btl.bi.player && (!btl_stat.CheckStatus(next, BattleStatus.BattleEnd) || Status.checkCurStat(next, BattleStatus.Death) && next.die_seq != num1))
                    return;
            if (btl.bi.player == 0)
            {
                Int32 num2 = 0;
                for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
                {
                    if (next.bi.player != 0 && (!btl_stat.CheckStatus(next, BattleStatus.BattleEnd) || (next.cur.hp == 0 || Status.checkCurStat(next, BattleStatus.Death)) && btl_stat.CheckStatus(next, BattleStatus.AutoLife) || btl_cmd.CheckSpecificCommand(next, BattleCommandId.SysReraise)))
                    {
                        num2 = 1;
                        break;
                    }
                }
                if (num2 == 0)
                    return;
                if (ff9Battle.btl_seq != 1)
                    ff9Battle.btl_seq = 0;
            }
            else
            {
                for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
                {
                    switch (btl_util.getSerialNumber(next))
                    {
                        case CharacterSerialNumber.EIKO_FLUTE:
                        case CharacterSerialNumber.EIKO_KNIFE:
                            if (!btl_stat.CheckStatus(next, BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Zombie | BattleStatus.Stop))
                            {
                                if (btl_cmd.CheckSpecificCommand(next, BattleCommandId.SysLastPhoenix))
                                    return;
                                if (ff9item.FF9Item_GetCount(RegularItem.PhoenixPinion) > Comn.random8())
                                {
                                    UIManager.Battle.FF9BMenu_EnableMenu(true);
                                    btl_cmd.SetCommand(next.cmd[0], BattleCommandId.SysLastPhoenix, (Int32)BattleAbilityId.RebirthFlame, btl_scrp.GetBattleID(0U), 1U);
                                    return;
                                }
                                break;
                            }
                            goto label_24;
                    }
                }
                label_24:
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

                if (next.IsPlayer == btl.IsPlayer && (!next.IsUnderAnyStatus(BattleStatus.BattleEnd) || (next.CurrentHp == 0 || next.IsUnderStatus(BattleStatus.Death)) && next.IsUnderAnyStatus(BattleStatus.AutoLife) || btl_cmd.CheckSpecificCommand(next.Data, BattleCommandId.SysReraise)))
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
                    foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(player.saExtended))
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

        public static void SetBonus(ENEMY_TYPE et)
        {
            BONUS btlBonus = battle.btl_bonus;
            btlBonus.gil += (Int32)et.bonus.gil;
            btlBonus.exp += et.bonus.exp;
            if (Comn.random8() < et.bonus.item_rate[3] && et.bonus.item[3] != RegularItem.NoItem)
            {
                btlBonus.item.Add(et.bonus.item[3]);
                et.bonus.item[3] = RegularItem.NoItem;
            }
            else if (Comn.random8() < et.bonus.item_rate[2] && et.bonus.item[2] != RegularItem.NoItem)
            {
                btlBonus.item.Add(et.bonus.item[2]);
                et.bonus.item[2] = RegularItem.NoItem;
            }
            else if (Comn.random8() < et.bonus.item_rate[1] && et.bonus.item[1] != RegularItem.NoItem)
            {
                btlBonus.item.Add(et.bonus.item[1]);
                et.bonus.item[1] = RegularItem.NoItem;
            }
            if (Comn.random8() < et.bonus.item_rate[0] && et.bonus.item[0] != RegularItem.NoItem)
            {
                btlBonus.item.Add(et.bonus.item[0]);
                et.bonus.item[0] = RegularItem.NoItem;
            }
            if (btlBonus.card != Byte.MaxValue || et.bonus.card >= 100U || Comn.random8() >= et.bonus.card_rate)
                return;
            btlBonus.card = (Byte)et.bonus.card;
        }

        public static void SavePlayerData(BTL_DATA btl, Boolean removingUnit)
        {
            BattleUnit unit = new BattleUnit(btl);
            PLAYER playerPtr = btl_util.getPlayerPtr(btl);
            playerPtr.trance = unit.IsUnderAnyStatus(BattleStatus.Trance) ? (Byte)0 : btl.trance;
            btl_init.CopyPoints(playerPtr.cur, btl.cur);
            if (removingUnit)
                if (btl.cur.hp == 0)
                    btl_stat.AlterStatus(btl, BattleStatus.Death);
            btl_stat.SaveStatus(playerPtr, btl);
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