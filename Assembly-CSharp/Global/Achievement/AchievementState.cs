using System;
using System.Collections.Generic;
using Assets.SiliconSocial;
using Memoria.Data;
using UnityEngine;

public class AchievementState : MonoBehaviour
{
	public void Initial()
	{
		this.AteCheck = new Int32[100];
		this.EvtReservedArray = new Int32[17];
		this.blkMag_no = 0;
		this.whtMag_no = 0;
		this.bluMag_no = 0;
		this.summon_no = 0;
		this.enemy_no = 0;
		this.backAtk_no = 0;
		this.defence_no = 0;
		this.trance_no = 0;
		this.abnormal_status = 0u;
		this.summon_shiva = false;
		this.summon_ifrit = false;
		this.summon_ramuh = false;
		this.summon_carbuncle_reflector = false;
		this.summon_carbuncle_haste = false;
		this.summon_carbuncle_protect = false;
		this.summon_carbuncle_shell = false;
		this.summon_fenrir_earth = false;
		this.summon_fenrir_wind = false;
		this.summon_atomos = false;
		this.summon_phoenix = false;
		this.summon_leviathan = false;
		this.summon_odin = false;
		this.summon_madeen = false;
		this.summon_bahamut = false;
		this.summon_arc = false;
		this.abilities = new HashSet<Int32>();
		this.passiveAbilities = new HashSet<Int32>();
		this.synthesisCount = 0;
		this.AuctionTime = 0;
		this.StiltzkinBuy = 0;
		this.QuadmistWinList = new HashSet<Int32>();
	}

	public Int32 increaseStealCount()
	{
		this.EvtReservedArray[1]++;
		return this.EvtReservedArray[1];
	}

	public AchievementStatusesEnum GetNormalAchievementStatuses(AcheivementKey key)
	{
		if (AchievementState.IsSystemAchievement(key))
		{
			return AchievementStatusesEnum.Invalid;
		}
		Int32 achievementIntIndex = this.GetAchievementIntIndex(key);
		Int32 achievementBitIndex = this.GetAchievementBitIndex(key);
		return AchievementState.ConvertDataToAchievementStatus(this.EvtReservedArray[achievementIntIndex], achievementBitIndex);
	}

	public void SetNormalAchievementStatuses(AcheivementKey key, AchievementStatusesEnum status)
	{
		if (AchievementState.IsSystemAchievement(key))
		{
			return;
		}
		Int32 achievementIntIndex = this.GetAchievementIntIndex(key);
		Int32 achievementBitIndex = this.GetAchievementBitIndex(key);
		Int32 num = AchievementState.ConvertAchievementStatusToData(status, achievementBitIndex);
		Int32 num2 = 3 << achievementBitIndex;
		this.EvtReservedArray[achievementIntIndex] &= ~num2;
		this.EvtReservedArray[achievementIntIndex] |= num;
	}

	public static Boolean IsSystemAchievement(AcheivementKey key)
	{
		return key == AcheivementKey.CompleteGame || key == AcheivementKey.Blackjack;
	}

	private Int32 GetAchievementIntIndex(AcheivementKey key)
	{
		Int32 num = (Int32)((Int32)key * (Int32)AcheivementKey.BlkMag100 / (Int32)AcheivementKey.Rope100);
		return 2 + num;
	}

	private Int32 GetAchievementBitIndex(AcheivementKey key)
	{
		return (Int32)((Int32)key * (Int32)AcheivementKey.BlkMag100 % (Int32)AcheivementKey.Rope100);
	}

	public static AchievementStatusesEnum ConvertDataToAchievementStatus(Int32 data, Int32 bitShifted)
	{
		Int32 num = data >> bitShifted;
		return (AchievementStatusesEnum)(num & 3);
	}

	public static Int32 ConvertAchievementStatusToData(AchievementStatusesEnum status, Int32 bitShifted)
	{
		Int32 num = (Int32)((Int32)status << (bitShifted & 31));
		Int32 num2 = 3 << bitShifted;
		return num & num2;
	}

	public const Int32 ATE_CHECK_SIZE = 100;

	public const Int32 EVT_RESERVED_ARRAY_SIZE = 17;

	public const Int32 MAX_ABILITIES = 221;

	public const Int32 MAX_PASSIVE_ABILITIES = 63;

	public const Int32 MAX_QUADMIST_WIN_LIST = 300;

	public Int32[] AteCheck = new Int32[100];

	public Int32[] EvtReservedArray = new Int32[17];

	public Int32 blkMag_no;

	public Int32 whtMag_no;

	public Int32 bluMag_no;

	public Int32 summon_no;

	public Int32 enemy_no;

	public Int32 backAtk_no;

	public Int32 defence_no;

	public Int32 trance_no;

	public UInt32 abnormal_status;

	public Boolean summon_shiva;

	public Boolean summon_ifrit;

	public Boolean summon_ramuh;

	public Boolean summon_carbuncle_reflector;

	public Boolean summon_carbuncle_haste;

	public Boolean summon_carbuncle_protect;

	public Boolean summon_carbuncle_shell;

	public Boolean summon_fenrir_earth;

	public Boolean summon_fenrir_wind;

	public Boolean summon_atomos;

	public Boolean summon_phoenix;

	public Boolean summon_leviathan;

	public Boolean summon_odin;

	public Boolean summon_madeen;

	public Boolean summon_bahamut;

	public Boolean summon_arc;

	public HashSet<Int32> abilities = new HashSet<Int32>();

	public HashSet<Int32> passiveAbilities = new HashSet<Int32>();

	public Int32 synthesisCount;

	public Int32 AuctionTime;

	public Int32 StiltzkinBuy;

	public HashSet<Int32> QuadmistWinList = new HashSet<Int32>();
}
