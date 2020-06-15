using System;
using System.IO;
using FF9;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;

public static class ff9level
{
    public static readonly EntryCollection<CharacterBaseStats> CharacterBaseStats;
    public static readonly CharacterLevelUp[] CharacterLevelUps;

    static ff9level()
    {
        CharacterBaseStats = LoadBaseStats();
        CharacterLevelUps = LoadLeveling();
    }

    private static EntryCollection<CharacterBaseStats> LoadBaseStats()
    {
        try
        {
            String inputPath = DataResources.Characters.BaseStatsFile;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"File with base stats of characters not found: [{inputPath}]");

            CharacterBaseStats[] baseStats = CsvReader.Read<CharacterBaseStats>(inputPath);
            if (baseStats.Length < CharacterId.CharacterCount)
                throw new NotSupportedException($"You must set base stats for {CharacterId.CharacterCount} characters, but there {baseStats.Length}.");

            return EntryCollection.CreateWithDefaultElement(baseStats, e => e.Id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ff9level] Load base stats of characters failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    private static CharacterLevelUp[] LoadLeveling()
    {
        try
        {
            String inputPath = DataResources.Characters.Leveling;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"File with leveling info not found: [{inputPath}]");

            CharacterLevelUp[] levels = CsvReader.Read<CharacterLevelUp>(inputPath);
            if (levels.Length < CharacterLevelUp.LevelCount)
                throw new NotSupportedException($"You must set level up info for {CharacterLevelUp.LevelCount} levels, but there {levels.Length}.");

            return levels;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ff9level] Load leveling info failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    public static Int32 FF9Level_GetDex(Int32 slot_id, Int32 lv, Boolean lvup)
	{
		PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
		FF9LEVEL_BONUS bonus = player.bonus;
		CharacterBaseStats ff9LEVEL_BASE = ff9level.CharacterBaseStats[ff9play.FF9Play_GetCharID(player.PresetId)];
		if (lvup)
		{
			Int32 num = 0;
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 0);
			FF9LEVEL_BONUS ff9LEVEL_BONUS = bonus;
			ff9LEVEL_BONUS.dex = (UInt16)(ff9LEVEL_BONUS.dex + (UInt16)(num + num2));
		}
		Int32 num3 = (Int32)ff9LEVEL_BASE.Dexterity + lv / 10 + (bonus.dex >> 5);
		if (num3 > 50)
		{
			num3 = 50;
		}
		return num3;
	}

	public static Int32 FF9Level_GetStr(Int32 slot_id, Int32 lv, Boolean lvup)
	{
		PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
		FF9LEVEL_BONUS bonus = player.bonus;
		CharacterBaseStats ff9LEVEL_BASE = ff9level.CharacterBaseStats[ff9play.FF9Play_GetCharID(player.PresetId)];
		if (lvup)
		{
			Int32 num = (Int32)((player.cur.capa != 0) ? 0 : 3);
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 1);
			FF9LEVEL_BONUS ff9LEVEL_BONUS = bonus;
			ff9LEVEL_BONUS.str = (UInt16)(ff9LEVEL_BONUS.str + (UInt16)(num + num2));
		}
		Int32 num3 = (Int32)ff9LEVEL_BASE.Strength + lv * 3 / 10 + (bonus.str >> 5);
		if (num3 > 99)
		{
			num3 = 99;
		}
		return num3;
	}

	public static Int32 FF9Level_GetMgc(Int32 slot_id, Int32 lv, Boolean lvup)
	{
		PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
		FF9LEVEL_BONUS bonus = player.bonus;
		CharacterBaseStats ff9LEVEL_BASE = ff9level.CharacterBaseStats[ff9play.FF9Play_GetCharID(player.PresetId)];
		if (lvup)
		{
			Int32 num = (Int32)((player.cur.capa != 0) ? 0 : 3);
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 2);
			FF9LEVEL_BONUS ff9LEVEL_BONUS = bonus;
			ff9LEVEL_BONUS.mgc = (UInt16)(ff9LEVEL_BONUS.mgc + (UInt16)(num + num2));
		}
		Int32 num3 = (Int32)ff9LEVEL_BASE.Magic + lv * 3 / 10 + (bonus.mgc >> 5);
		if (num3 > 99)
		{
			num3 = 99;
		}
		return num3;
	}

	public static Int32 FF9Level_GetWpr(Int32 slot_id, Int32 lv, Boolean lvup)
	{
		PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
		FF9LEVEL_BONUS bonus = player.bonus;
		CharacterBaseStats ff9LEVEL_BASE = ff9level.CharacterBaseStats[ff9play.FF9Play_GetCharID(player.PresetId)];
		if (lvup)
		{
			Int32 num = (Int32)((player.cur.capa != 0) ? 0 : 1);
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 3);
			FF9LEVEL_BONUS ff9LEVEL_BONUS = bonus;
			ff9LEVEL_BONUS.wpr = (UInt16)(ff9LEVEL_BONUS.wpr + (UInt16)(num + num2));
		}
		Int32 num3 = (Int32)ff9LEVEL_BASE.Will + lv * 3 / 20 + (bonus.wpr >> 5);
		if (num3 > 50)
		{
			num3 = 50;
		}
		return num3;
	}

	public static Int32 FF9Level_GetCap(Int32 slot_id, Int32 lv, Boolean lvup)
	{
		PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
		FF9LEVEL_BONUS bonus = player.bonus;
		CharacterBaseStats ff9LEVEL_BASE = ff9level.CharacterBaseStats[ff9play.FF9Play_GetCharID(player.PresetId)];
		if (lvup)
		{
			Int32 num = (Int32)((player.cur.capa != 0) ? 0 : 5);
			Int32 num2 = 0;
			FF9LEVEL_BONUS ff9LEVEL_BONUS = bonus;
			ff9LEVEL_BONUS.cap = (UInt16)(ff9LEVEL_BONUS.cap + (UInt16)(num + num2));
		}
		Int32 gemCount = (Int32)ff9LEVEL_BASE.Gems + lv * 4 / 10 + (bonus.cap >> 5);
		if (gemCount > 99)
		{
			gemCount = 99;
		}
		return gemCount;
	}

	public static UInt32 FF9Level_GetHp(Int32 lv, Int32 str)
	{
		UInt32 num = (UInt32)(ff9level.CharacterLevelUps[lv - 1].BonusHP * str / 50);
		if (num > 9999)
		{
			num = 9999;
		}
		return num;
	}

	public static UInt32 FF9Level_GetMp(Int32 lv, Int32 mgc)
	{
		UInt32 num = (UInt32)(ff9level.CharacterLevelUps[lv - 1].BonusMP * mgc / 100);
		if (num > 999)
		{
			num = 999;
		}
		return num;
	}

	public static Int32 FF9Level_GetEquipBonus(CharacterEquipment equip, Int32 base_type)
	{
		Int32 num = 0;
		for (Int32 i = 0; i < 5; i++)
		{
			if (equip[i] != 255)
			{
				FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[(Int32)equip[i]];
				ItemStats equip_PRIVILEGE = ff9equip.ItemStatsData[(Int32)ff9ITEM_DATA.bonus];
				switch (base_type)
				{
				case 0:
					num += (Int32)equip_PRIVILEGE.dex;
					break;
				case 1:
					num += (Int32)equip_PRIVILEGE.str;
					break;
				case 2:
					num += (Int32)equip_PRIVILEGE.mgc;
					break;
				case 3:
					num += (Int32)equip_PRIVILEGE.wpr;
					break;
				}
			}
		}
		return num;
	}

	// public const Byte FF9LEVEL_DEX_MAX = 50;
	// public const Byte FF9LEVEL_STR_MAX = 99;
	// public const Byte FF9LEVEL_MGC_MAX = 99;
	// public const Byte FF9LEVEL_WPR_MAX = 50;
	// public const Byte FF9LEVEL_CAP_MAX = 99;
	// public const Int32 FF9LEVEL_HP_MAX = 9999;
	// public const Int32 FF9LEVEL_MP_MAX = 999;
}
