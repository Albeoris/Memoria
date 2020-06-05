using System;
using System.Linq;
using Assets.SiliconSocial;
using Memoria.Data;

public class BattleAchievement
{
	public static void UpdateEndBattleAchievement()
	{
		Int32 battleMapIndex = FF9StateSystem.Battle.battleMapIndex;
		BTL_SCENE btl_scene = FF9StateSystem.Battle.FF9Battle.btl_scene;
		SB2_PATTERN sb2_PATTERN = btl_scene.PatAddr[(Int32)FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum];
		BattleAchievement.IncreseNumber(ref BattleAchievement.achievement.enemy_no, (Int32)sb2_PATTERN.MonCount);
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
			Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(50900);
			Int32 num = varManually >> 3 & 31;
			Int32 num2 = num * 100 / 16;
			if (num2 >= 100)
			{
				AchievementManager.ReportAchievement(AcheivementKey.AllOX, 1);
			}
		}
		else if (battleMapIndex == 920 || battleMapIndex == 921)
		{
			Int32 varManually2 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(405732);
			if (varManually2 == 1)
			{
				AchievementManager.ReportAchievement(AcheivementKey.YanBlessing, 1);
			}
		}
		else if (battleMapIndex == 339 && BattleAchievement.IsChallengingPlayerIsGarnet())
		{
			AchievementManager.ReportAchievement(AcheivementKey.DefeatBehemoth, 1);
		}
	}

	private static Boolean IsChallengingPlayerIsGarnet()
	{
		Boolean result = false;
		for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
		{
			if (next.bi.player != 0 && next.bi.slot_no == 2)
			{
				result = true;
			}
		}
		return result;
	}

	public static void UpdateCommandAchievement(CMD_DATA cmd)
	{
	    BattleCommandId cmd_no = cmd.cmd_no;
		if (cmd_no == BattleCommandId.Defend)
		{
			BattleAchievement.IncreseNumber(ref BattleAchievement.achievement.defence_no, 1);
			AchievementManager.ReportAchievement(AcheivementKey.Defense50, BattleAchievement.achievement.defence_no);
		}
		else if (cmd_no == BattleCommandId.BlackMagic || cmd_no == BattleCommandId.DoubleBlackMagic)
		{
			BattleAchievement.IncreseNumber(ref BattleAchievement.achievement.blkMag_no, 1);
			AchievementManager.ReportAchievement(AcheivementKey.BlkMag100, BattleAchievement.achievement.blkMag_no);
		}
		else if (cmd_no == BattleCommandId.WhiteMagicGarnet || cmd_no == BattleCommandId.WhiteMagicEiko || cmd_no == BattleCommandId.DoubleWhiteMagic)
		{
			BattleAchievement.IncreseNumber(ref BattleAchievement.achievement.whtMag_no, 1);
			AchievementManager.ReportAchievement(AcheivementKey.WhtMag200, BattleAchievement.achievement.whtMag_no);
		}
		else if (cmd_no == BattleCommandId.BlueMagic)
		{
			BattleAchievement.IncreseNumber(ref BattleAchievement.achievement.bluMag_no, 1);
			AchievementManager.ReportAchievement(AcheivementKey.BluMag100, BattleAchievement.achievement.bluMag_no);
		}
		else if (cmd_no == BattleCommandId.SummonGarnet || cmd_no == BattleCommandId.Phantom || cmd_no == BattleCommandId.SummonEiko)
		{
			BattleAchievement.IncreseNumber(ref BattleAchievement.achievement.summon_no, 1);
			AchievementManager.ReportAchievement(AcheivementKey.Summon50, BattleAchievement.achievement.summon_no);
			if (cmd.sub_no == 49)
			{
				BattleAchievement.achievement.summon_shiva = true;
				AchievementManager.ReportAchievement(AcheivementKey.SummonShiva, 1);
			}
			else if (cmd.sub_no == 51)
			{
				BattleAchievement.achievement.summon_ifrit = true;
				AchievementManager.ReportAchievement(AcheivementKey.SummonIfrit, 1);
			}
			else if (cmd.sub_no == 53)
			{
				BattleAchievement.achievement.summon_ramuh = true;
				AchievementManager.ReportAchievement(AcheivementKey.SummonRamuh, 1);
			}
			else if (cmd.sub_no == 55)
			{
				BattleAchievement.achievement.summon_atomos = true;
				AchievementManager.ReportAchievement(AcheivementKey.SummonAtomos, 1);
			}
			else if (cmd.sub_no == 58)
			{
				BattleAchievement.achievement.summon_odin = true;
				AchievementManager.ReportAchievement(AcheivementKey.SummonOdin, 1);
			}
			else if (cmd.sub_no == 60)
			{
				BattleAchievement.achievement.summon_leviathan = true;
				AchievementManager.ReportAchievement(AcheivementKey.SummonLeviathan, 1);
			}
			else if (cmd.sub_no == 62)
			{
				BattleAchievement.achievement.summon_bahamut = true;
				AchievementManager.ReportAchievement(AcheivementKey.SummonBahamut, 1);
			}
			else if (cmd.sub_no == 64)
			{
				BattleAchievement.achievement.summon_arc = true;
				AchievementManager.ReportAchievement(AcheivementKey.SummonArk, 1);
			}
			else if (cmd.sub_no == 68 || cmd.sub_no == 69 || cmd.sub_no == 70 || cmd.sub_no == 71)
			{
				if (cmd.sub_no == 68)
				{
					BattleAchievement.achievement.summon_carbuncle_haste = true;
				}
				else if (cmd.sub_no == 69)
				{
					BattleAchievement.achievement.summon_carbuncle_protect = true;
				}
				else if (cmd.sub_no == 70)
				{
					BattleAchievement.achievement.summon_carbuncle_reflector = true;
				}
				else if (cmd.sub_no == 71)
				{
					BattleAchievement.achievement.summon_carbuncle_shell = true;
				}
				AchievementManager.ReportAchievement(AcheivementKey.SummonCarbuncle, 1);
			}
			else if (cmd.sub_no == 66 || cmd.sub_no == 67)
			{
				if (cmd.sub_no == 66)
				{
					BattleAchievement.achievement.summon_fenrir_earth = true;
				}
				else if (cmd.sub_no == 67)
				{
					BattleAchievement.achievement.summon_fenrir_wind = true;
				}
				AchievementManager.ReportAchievement(AcheivementKey.SummonFenrir, 1);
			}
			else if (cmd.sub_no == 72)
			{
				BattleAchievement.achievement.summon_phoenix = true;
				AchievementManager.ReportAchievement(AcheivementKey.SummonPhoenix, 1);
			}
			else if (cmd.sub_no == 74)
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
		BattleAchievement.IncreseNumber(ref BattleAchievement.achievement.backAtk_no, 1);
		AchievementManager.ReportAchievement(AcheivementKey.BackAttack30, BattleAchievement.achievement.backAtk_no);
	}

	public static void UpdateAbnormalStatus(BattleStatus status)
	{
		BattleAchievement.achievement.abnormal_status |= (UInt32) status;
		if ((BattleAchievement.achievement.abnormal_status & (UInt64) BattleStatus.Achievement) == (UInt64) BattleStatus.Achievement)
		{
			AchievementManager.ReportAchievement(AcheivementKey.AbnormalStatus, 1);
		}
	}

	public static void UpdateTranceStatus()
	{
		BattleAchievement.IncreseNumber(ref BattleAchievement.achievement.trance_no, 1);
		AchievementManager.ReportAchievement(AcheivementKey.Trance1, BattleAchievement.achievement.trance_no);
		AchievementManager.ReportAchievement(AcheivementKey.Trance50, BattleAchievement.achievement.trance_no);
	}

	public static void UpdateParty()
	{
		PLAYER[] member = FF9StateSystem.Common.FF9.party.member;
		Int32[] source = new Int32[]
		{
			3,
			4,
			5,
			6,
			9,
			10,
			11,
			12
		};
		Int32[] source2 = new Int32[]
		{
			0,
			1,
			2,
			7,
			8,
			13
		};
		Int32 num = 0;
		Int32 num2 = 0;
		for (Int32 i = 0; i < 4; i++)
		{
			if (member[i] != null)
			{
				Byte serial_no = member[i].info.serial_no;
				if (source.Contains((Int32)serial_no))
				{
					num++;
				}
				if (source2.Contains((Int32)serial_no))
				{
					num2++;
				}
			}
		}
		Debug.Log(String.Concat(new Object[]
		{
			"femaleCount = ",
			num,
			", maleCount = ",
			num2
		}));
		if (num == 4)
		{
			AchievementManager.ReportAchievement(AcheivementKey.PartyWomen, 1);
		}
		if (num2 == 4)
		{
			AchievementManager.ReportAchievement(AcheivementKey.PartyMen, 1);
		}
	}

	public static void IncreseNumber(ref Int32 data, Int32 num = 1)
	{
		if (data < 2147483647 - num)
		{
			data += num;
		}
	}

	public static void IncreseNumber(ref Int16 data, Int32 num = 1)
	{
		if ((Int32)data < 32767 - num)
		{
			data = (Int16)(data + (Int16)num);
		}
	}

	public static void GetReachLv99Achievement(Int32 lv)
	{
		if (lv >= 99)
		{
			AchievementManager.ReportAchievement(AcheivementKey.CharLv99, 1);
		}
	}

	public static Boolean UpdateAbilitiesAchievement(Int32 abilId, Boolean autoReport)
	{
		Boolean flag = false;
		if (abilId == 0)
		{
			return flag;
		}
		if (abilId < 192)
		{
			if (!FF9StateSystem.Achievement.abilities.Contains(abilId))
			{
				FF9StateSystem.Achievement.abilities.Add(abilId);
				flag = true;
			}
		}
		else
		{
			if (!FF9StateSystem.Achievement.abilities.Contains(abilId))
			{
				FF9StateSystem.Achievement.abilities.Add(abilId);
				flag = true;
			}
			if (!FF9StateSystem.Achievement.passiveAbilities.Contains(abilId))
			{
				FF9StateSystem.Achievement.passiveAbilities.Add(abilId);
				flag = true;
			}
		}
		if (autoReport && flag)
		{
			BattleAchievement.SendAbilitiesAchievement();
		}
		return flag;
	}

	public static void SendAbilitiesAchievement()
	{
		AchievementManager.ReportAchievement(AcheivementKey.AllPasssiveAbility, FF9StateSystem.Achievement.passiveAbilities.Count);
		AchievementManager.ReportAchievement(AcheivementKey.AllAbility, FF9StateSystem.Achievement.abilities.Count);
	}

	public static AchievementState achievement = FF9StateSystem.Achievement;
}
