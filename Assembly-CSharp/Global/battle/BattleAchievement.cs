using Assets.SiliconSocial;
using FF9;
using Memoria.Data;
using System;
using System.Linq;

public class BattleAchievement
{
    public static void UpdateEndBattleAchievement()
    {
        Int32 battleMapIndex = FF9StateSystem.Battle.battleMapIndex;
        BTL_SCENE btlScene = FF9StateSystem.Battle.FF9Battle.btl_scene;
        SB2_PATTERN btlPattern = btlScene.PatAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum];
        BattleAchievement.IncreaseNumber(ref BattleAchievement.achievement.enemy_no, btlPattern.MonsterCount);
        AchievementManager.ReportAchievement(AcheivementKey.Defeat100, BattleAchievement.achievement.enemy_no);
        AchievementManager.ReportAchievement(AcheivementKey.Defeat1000, BattleAchievement.achievement.enemy_no);
        AchievementManager.ReportAchievement(AcheivementKey.Defeat10000, BattleAchievement.achievement.enemy_no);
        if (battleMapIndex == 932)
        {
            AchievementManager.ReportAchievement(AcheivementKey.DefeatMaliris, 1);
        }
        else if (battleMapIndex == 933)
        {
            AchievementManager.ReportAchievement(AcheivementKey.DefeatTiamat, 1);
        }
        else if (battleMapIndex == 934)
        {
            AchievementManager.ReportAchievement(AcheivementKey.DefeatKraken, 1);
        }
        else if (battleMapIndex == 935)
        {
            AchievementManager.ReportAchievement(AcheivementKey.DefeatLich, 1);
        }
        else if (battleMapIndex == 211 || battleMapIndex == 57)
        {
            AchievementManager.ReportAchievement(AcheivementKey.DefeatOzma, 1);
        }
        else if (battleMapIndex == 634 || battleMapIndex == 627 || battleMapIndex == 755 || battleMapIndex == 753)
        {
            // Ragtime Mouse (quizz result)
            Int32 genVarRagtimeQuizzSuccess = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Byte, 198));
            Int32 successCount = genVarRagtimeQuizzSuccess >> 3 & 31;
            Int32 successPourcent = successCount * 100 / 16;
            if (successPourcent >= 100)
                AchievementManager.ReportAchievement(AcheivementKey.AllOX, 1);
        }
        else if (battleMapIndex == 920 || battleMapIndex == 921)
        {
            // Friendly Yan
            Int32 genVarFriendlyYan = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Global, EBin.VariableType.Bit, 1584));
            if (genVarFriendlyYan == 1)
                AchievementManager.ReportAchievement(AcheivementKey.YanBlessing, 1);
        }
        else if (battleMapIndex == 339 && BattleAchievement.IsChallengingPlayerIsGarnet())
        {
            AchievementManager.ReportAchievement(AcheivementKey.DefeatBehemoth, 1);
        }
    }

    private static Boolean IsChallengingPlayerIsGarnet()
    {
        for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
            if (next.bi.player != 0 && next.bi.slot_no == (Byte)CharacterId.Garnet)
                return true;
        return false;
    }

    public static void UpdateCommandAchievement(CMD_DATA cmd)
    {
        BattleCommandId cmd_no = cmd.cmd_no;
        if (cmd_no == BattleCommandId.Defend)
        {
            BattleAchievement.IncreaseNumber(ref BattleAchievement.achievement.defence_no, 1);
            AchievementManager.ReportAchievement(AcheivementKey.Defense50, BattleAchievement.achievement.defence_no);
        }
        else if (cmd_no == BattleCommandId.BlackMagic || cmd_no == BattleCommandId.DoubleBlackMagic)
        {
            BattleAchievement.IncreaseNumber(ref BattleAchievement.achievement.blkMag_no, 1);
            AchievementManager.ReportAchievement(AcheivementKey.BlkMag100, BattleAchievement.achievement.blkMag_no);
        }
        else if (cmd_no == BattleCommandId.WhiteMagicGarnet || cmd_no == BattleCommandId.WhiteMagicEiko || cmd_no == BattleCommandId.DoubleWhiteMagic)
        {
            BattleAchievement.IncreaseNumber(ref BattleAchievement.achievement.whtMag_no, 1);
            AchievementManager.ReportAchievement(AcheivementKey.WhtMag200, BattleAchievement.achievement.whtMag_no);
        }
        else if (cmd_no == BattleCommandId.BlueMagic)
        {
            BattleAchievement.IncreaseNumber(ref BattleAchievement.achievement.bluMag_no, 1);
            AchievementManager.ReportAchievement(AcheivementKey.BluMag100, BattleAchievement.achievement.bluMag_no);
        }
        else if (cmd_no == BattleCommandId.SummonGarnet || cmd_no == BattleCommandId.Phantom || cmd_no == BattleCommandId.SummonEiko)
        {
            BattleAchievement.IncreaseNumber(ref BattleAchievement.achievement.summon_no, 1);
            AchievementManager.ReportAchievement(AcheivementKey.Summon50, BattleAchievement.achievement.summon_no);
            BattleAbilityId abilId = btl_util.GetCommandMainActionIndex(cmd);
            if (abilId == BattleAbilityId.Shiva)
            {
                BattleAchievement.achievement.summon_shiva = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonShiva, 1);
            }
            else if (abilId == BattleAbilityId.Ifrit)
            {
                BattleAchievement.achievement.summon_ifrit = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonIfrit, 1);
            }
            else if (abilId == BattleAbilityId.Ramuh)
            {
                BattleAchievement.achievement.summon_ramuh = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonRamuh, 1);
            }
            else if (abilId == BattleAbilityId.Atomos)
            {
                BattleAchievement.achievement.summon_atomos = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonAtomos, 1);
            }
            else if (abilId == BattleAbilityId.Odin)
            {
                BattleAchievement.achievement.summon_odin = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonOdin, 1);
            }
            else if (abilId == BattleAbilityId.Leviathan)
            {
                BattleAchievement.achievement.summon_leviathan = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonLeviathan, 1);
            }
            else if (abilId == BattleAbilityId.Bahamut)
            {
                BattleAchievement.achievement.summon_bahamut = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonBahamut, 1);
            }
            else if (abilId == BattleAbilityId.Ark)
            {
                BattleAchievement.achievement.summon_arc = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonArk, 1);
            }
            else if (abilId == BattleAbilityId.Carbuncle1 || abilId == BattleAbilityId.Carbuncle2 || abilId == BattleAbilityId.Carbuncle3 || abilId == BattleAbilityId.Carbuncle4)
            {
                if (abilId == BattleAbilityId.Carbuncle1)
                    BattleAchievement.achievement.summon_carbuncle_haste = true;
                else if (abilId == BattleAbilityId.Carbuncle2)
                    BattleAchievement.achievement.summon_carbuncle_protect = true;
                else if (abilId == BattleAbilityId.Carbuncle3)
                    BattleAchievement.achievement.summon_carbuncle_reflector = true;
                else if (abilId == BattleAbilityId.Carbuncle4)
                    BattleAchievement.achievement.summon_carbuncle_shell = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonCarbuncle, 1);
            }
            else if (abilId == BattleAbilityId.Fenrir1 || abilId == BattleAbilityId.Fenrir2)
            {
                if (abilId == BattleAbilityId.Fenrir1)
                    BattleAchievement.achievement.summon_fenrir_earth = true;
                else if (abilId == BattleAbilityId.Fenrir2)
                    BattleAchievement.achievement.summon_fenrir_wind = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonFenrir, 1);
            }
            else if (abilId == BattleAbilityId.Phoenix)
            {
                BattleAchievement.achievement.summon_phoenix = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonPhoenix, 1);
            }
            else if (abilId == BattleAbilityId.Madeen)
            {
                BattleAchievement.achievement.summon_madeen = true;
                AchievementManager.ReportAchievement(AcheivementKey.SummonMadeen, 1);
            }
        }
        else if (cmd_no == BattleCommandId.Steal)
        {
            Int32 totalProgress = BattleAchievement.achievement.increaseStealCount();
            AchievementManager.ReportAchievement(AcheivementKey.Steal50, totalProgress);
        }
        else if (cmd_no == BattleCommandId.SysLastPhoenix)
        {
            AchievementManager.ReportAchievement(AcheivementKey.RebirthFlame, 1);
        }
    }

    public static void UpdateBackAttack()
    {
        BattleAchievement.IncreaseNumber(ref BattleAchievement.achievement.backAtk_no, 1);
        AchievementManager.ReportAchievement(AcheivementKey.BackAttack30, BattleAchievement.achievement.backAtk_no);
    }

    public static void UpdateAbnormalStatus(BattleStatus status)
    {
        BattleAchievement.achievement.abnormal_status |= (UInt32)status;
        if ((BattleAchievement.achievement.abnormal_status & (UInt32)BattleStatusConst.Achievement) == (UInt32)BattleStatusConst.Achievement)
            AchievementManager.ReportAchievement(AcheivementKey.AbnormalStatus, 1);
    }

    public static void UpdateTranceStatus()
    {
        BattleAchievement.IncreaseNumber(ref BattleAchievement.achievement.trance_no, 1);
        AchievementManager.ReportAchievement(AcheivementKey.Trance1, BattleAchievement.achievement.trance_no);
        AchievementManager.ReportAchievement(AcheivementKey.Trance50, BattleAchievement.achievement.trance_no);
    }

    public static void UpdateParty()
    {
        PLAYER[] member = FF9StateSystem.Common.FF9.party.member;
        CharacterSerialNumber[] femaleList = new CharacterSerialNumber[]
        {
            CharacterSerialNumber.GARNET_LH_ROD,
            CharacterSerialNumber.GARNET_LH_RACKET,
            CharacterSerialNumber.GARNET_SH_ROD,
            CharacterSerialNumber.GARNET_SH_RACKET,
            CharacterSerialNumber.KUINA,
            CharacterSerialNumber.EIKO_FLUTE,
            CharacterSerialNumber.EIKO_RACKET,
            CharacterSerialNumber.FREIJA,
            CharacterSerialNumber.BEATRIX
        };
        CharacterSerialNumber[] maleList = new CharacterSerialNumber[]
        {
            CharacterSerialNumber.ZIDANE_DAGGER,
            CharacterSerialNumber.ZIDANE_SWORD,
            CharacterSerialNumber.VIVI,
            CharacterSerialNumber.STEINER_OUTDOOR,
            CharacterSerialNumber.STEINER_INDOOR,
            CharacterSerialNumber.SALAMANDER,
			//CharacterSerialNumber.CINNA, // Counting these would trigger the achievement when fighting Masked Man, I guess
			//CharacterSerialNumber.MARCUS,
			//CharacterSerialNumber.BLANK,
			//CharacterSerialNumber.BLANK_ARMOR
		};
        Int32 femaleCount = 0;
        Int32 maleCount = 0;
        for (Int32 i = 0; i < 4; i++)
        {
            if (member[i] != null)
            {
                CharacterSerialNumber serial_no = member[i].info.serial_no;
                if (femaleList.Contains(serial_no))
                    femaleCount++;
                if (maleList.Contains(serial_no))
                    maleCount++;
            }
        }
        Debug.Log(String.Concat(new Object[]
        {
            "femaleCount = ",
            femaleCount,
            ", maleCount = ",
            maleCount
        }));
        if (femaleCount == 4)
            AchievementManager.ReportAchievement(AcheivementKey.PartyWomen, 1);
        if (maleCount == 4)
            AchievementManager.ReportAchievement(AcheivementKey.PartyMen, 1);
    }

    public static void IncreaseNumber(ref Int32 data, Int32 num = 1)
    {
        if (data < Int32.MaxValue - num)
            data += num;
    }

    public static void IncreaseNumber(ref Int16 data, Int16 num = 1)
    {
        if (data < Int16.MaxValue - num)
            data += num;
    }

    public static void GetReachLv99Achievement(Int32 lv)
    {
        if (lv >= ff9level.LEVEL_COUNT)
            AchievementManager.ReportAchievement(AcheivementKey.CharLv99, 1);
    }

    public static Boolean UpdateAbilitiesAchievement(Int32 abilId, Boolean autoReport)
    {
        Boolean gotAchievement = false;
        if (abilId == 0)
            return gotAchievement;
        if (ff9abil.IsAbilityActive(abilId))
        {
            if (!FF9StateSystem.Achievement.abilities.Contains(abilId))
            {
                FF9StateSystem.Achievement.abilities.Add(abilId);
                gotAchievement = true;
            }
        }
        else
        {
            if (!FF9StateSystem.Achievement.abilities.Contains(abilId))
            {
                FF9StateSystem.Achievement.abilities.Add(abilId);
                gotAchievement = true;
            }
            if (!FF9StateSystem.Achievement.passiveAbilities.Contains(abilId))
            {
                FF9StateSystem.Achievement.passiveAbilities.Add(abilId);
                gotAchievement = true;
            }
        }
        if (autoReport && gotAchievement)
            BattleAchievement.SendAbilitiesAchievement();
        return gotAchievement;
    }

    public static void SendAbilitiesAchievement()
    {
        AchievementManager.ReportAchievement(AcheivementKey.AllPasssiveAbility, FF9StateSystem.Achievement.passiveAbilities.Count);
        AchievementManager.ReportAchievement(AcheivementKey.AllAbility, FF9StateSystem.Achievement.abilities.Count);
    }

    public static AchievementState achievement = FF9StateSystem.Achievement;
}
