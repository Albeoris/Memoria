using System;
using System.IO;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;
using NCalc;

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
            String inputPath = DataResources.Characters.Directory + DataResources.Characters.BaseStatsFile;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"File with base stats of characters not found: [{inputPath}]");

            CharacterBaseStats[] baseStats = CsvReader.Read<CharacterBaseStats>(inputPath);
            if (baseStats.Length < 12)
                throw new NotSupportedException($"You must set base stats for at least {12} characters, but there {baseStats.Length}.");

			EntryCollection<CharacterBaseStats> result = EntryCollection.CreateWithDefaultElement(baseStats, e => e.Id);
			for (Int32 i = Configuration.Mod.FolderNames.Length - 1; i >= 0; i--)
			{
				inputPath = DataResources.Characters.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.Characters.BaseStatsFile;
				if (File.Exists(inputPath))
				{
					baseStats = CsvReader.Read<CharacterBaseStats>(inputPath);
					foreach (CharacterBaseStats it in baseStats)
						result[it.Id] = it;
				}
			}
			return result;
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
			CharacterLevelUp[] levels;

			String inputPath;
			for (Int32 i = 0; i < Configuration.Mod.FolderNames.Length; i++)
			{
				inputPath = DataResources.Characters.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.Characters.Leveling;
				if (File.Exists(inputPath))
				{
					levels = CsvReader.Read<CharacterLevelUp>(inputPath);
					if (levels.Length < CharacterLevelUp.LevelCount)
						throw new NotSupportedException($"You must set level up info for {CharacterLevelUp.LevelCount} levels, but there {levels.Length}.");
					return levels;
				}
			}
			inputPath = DataResources.Characters.Directory + DataResources.Characters.Leveling;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"File with leveling info not found: [{inputPath}]");

            levels = CsvReader.Read<CharacterLevelUp>(inputPath);
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

    public static Int32 FF9Level_GetDex(PLAYER player, Int32 lv, Boolean lvup)
	{
		FF9LEVEL_BONUS bonus = player.bonus;
		CharacterBaseStats ff9LEVEL_BASE = ff9level.CharacterBaseStats[(Int32)ff9play.CharacterPresetToID(player.PresetId)];
		if (lvup)
		{
			Int32 num = 0;
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 0);
			bonus.dex = (UInt16)(bonus.dex + (UInt16)(num + num2));
		}
		Int32 num3 = Math.Min(50, (Int32)ff9LEVEL_BASE.Dexterity + lv / 10 + (bonus.dex >> 5));
		if (Configuration.Battle.SpeedStatFormula.Length > 0)
		{
			Expression e = new Expression(Configuration.Battle.SpeedStatFormula);
			NCalcUtility.InitializeExpressionPlayer(ref e, player);
			e.Parameters["Level"] = lv; // overrides "player.level"
			e.Parameters["SpeedBonus"] = (Int32)bonus.dex; // As it is, SpeedBonus contains only bonuses from equipment, no bonus is gotten from level ups
			e.Parameters["SpeedBase"] = (Int32)ff9LEVEL_BASE.Dexterity;
			e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
			e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
			Int32 val = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
			if (val >= 0)
				num3 = Math.Min(val, Byte.MaxValue); // "player.basis.dex" is a Byte, so it's better to force a 255 limit there
		}
		return num3;
	}

	public static Int32 FF9Level_GetStr(PLAYER player, Int32 lv, Boolean lvup)
	{
		FF9LEVEL_BONUS bonus = player.bonus;
		CharacterBaseStats ff9LEVEL_BASE = ff9level.CharacterBaseStats[(Int32)ff9play.CharacterPresetToID(player.PresetId)];
		if (lvup)
		{
			Int32 num = (player.cur.capa != 0) ? 0 : 3;
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 1);
			bonus.str = (UInt16)(bonus.str + (UInt16)(num + num2));
		}
		Int32 num3 = Math.Min(99, (Int32)ff9LEVEL_BASE.Strength + lv * 3 / 10 + (bonus.str >> 5));
		if (Configuration.Battle.StrengthStatFormula.Length > 0)
		{
			Expression e = new Expression(Configuration.Battle.StrengthStatFormula);
			NCalcUtility.InitializeExpressionPlayer(ref e, player);
			e.Parameters["Level"] = lv; // overrides "player.level"
			e.Parameters["StrengthBonus"] = (Int32)bonus.str; // As it is, StrengthBonus contains both bonuses from equipment and from level ups (x3)
			e.Parameters["StrengthBase"] = (Int32)ff9LEVEL_BASE.Strength;
			e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
			e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
			Int32 val = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
			if (val >= 0)
				num3 = Math.Min(val, Byte.MaxValue); // "player.basis.str" is a Byte, so it's better to force a 255 limit there
		}
		return num3;
	}

	public static Int32 FF9Level_GetMgc(PLAYER player, Int32 lv, Boolean lvup)
	{
		FF9LEVEL_BONUS bonus = player.bonus;
		CharacterBaseStats ff9LEVEL_BASE = ff9level.CharacterBaseStats[(Int32)ff9play.CharacterPresetToID(player.PresetId)];
		if (lvup)
		{
			Int32 num = (player.cur.capa != 0) ? 0 : 3;
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 2);
			bonus.mgc = (UInt16)(bonus.mgc + (UInt16)(num + num2));
		}
		Int32 num3 = Math.Min(99, (Int32)ff9LEVEL_BASE.Magic + lv * 3 / 10 + (bonus.mgc >> 5));
		if (Configuration.Battle.MagicStatFormula.Length > 0)
		{
			Expression e = new Expression(Configuration.Battle.MagicStatFormula);
			NCalcUtility.InitializeExpressionPlayer(ref e, player);
			e.Parameters["Level"] = lv; // overrides "player.level"
			e.Parameters["MagicBonus"] = (Int32)bonus.mgc; // As it is, MagicBonus contains both bonuses from equipment and from level ups (x3)
			e.Parameters["MagicBase"] = (Int32)ff9LEVEL_BASE.Magic;
			e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
			e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
			Int32 val = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
			if (val >= 0)
				num3 = Math.Min(val, Byte.MaxValue); // "player.basis.mgc" is a Byte, so it's better to force a 255 limit there
		}
		return num3;
	}

	public static Int32 FF9Level_GetWpr(PLAYER player, Int32 lv, Boolean lvup)
	{
		FF9LEVEL_BONUS bonus = player.bonus;
		CharacterBaseStats ff9LEVEL_BASE = ff9level.CharacterBaseStats[(Int32)ff9play.CharacterPresetToID(player.PresetId)];
		if (lvup)
		{
			Int32 num = (player.cur.capa != 0) ? 0 : 1;
			Int32 num2 = ff9level.FF9Level_GetEquipBonus(player.equip, 3);
			bonus.wpr = (UInt16)(bonus.wpr + (UInt16)(num + num2));
		}
		Int32 num3 = Math.Min(50, (Int32)ff9LEVEL_BASE.Will + lv * 3 / 20 + (bonus.wpr >> 5));
		if (Configuration.Battle.SpiritStatFormula.Length > 0)
		{
			Expression e = new Expression(Configuration.Battle.SpiritStatFormula);
			NCalcUtility.InitializeExpressionPlayer(ref e, player);
			e.Parameters["Level"] = lv; // overrides "player.level"
			e.Parameters["SpiritBonus"] = (Int32)bonus.wpr; // As it is, SpiritBonus contains both bonuses from equipment and from level ups (x1)
			e.Parameters["SpiritBase"] = (Int32)ff9LEVEL_BASE.Will;
			e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
			e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
			Int32 val = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
			if (val >= 0)
				num3 = Math.Min(val, Byte.MaxValue); // "player.basis.wpr" is a Byte, so it's better to force a 255 limit there
		}
		return num3;
	}

	public static Int32 FF9Level_GetCap(PLAYER player, Int32 lv, Boolean lvup)
	{
		FF9LEVEL_BONUS bonus = player.bonus;
		CharacterBaseStats ff9LEVEL_BASE = ff9level.CharacterBaseStats[(Int32)ff9play.CharacterPresetToID(player.PresetId)];
		if (lvup)
		{
			Int32 num = (player.cur.capa != 0) ? 0 : 5;
			Int32 num2 = 0;
			bonus.cap = (UInt16)(bonus.cap + (UInt16)(num + num2));
		}
		Int32 gemCount = Math.Min(99, (Int32)ff9LEVEL_BASE.Gems + lv * 4 / 10 + (bonus.cap >> 5));
		if (Configuration.Battle.MagicStoneStockFormula.Length > 0)
		{
			Expression e = new Expression(Configuration.Battle.MagicStoneStockFormula);
			NCalcUtility.InitializeExpressionPlayer(ref e, player);
			e.Parameters["Level"] = lv; // overrides "player.level"
			e.Parameters["MagicStoneBonus"] = (Int32)bonus.cap; // MagicStoneBonus contains the bonus from level ups (x5)
			e.Parameters["MagicStoneBase"] = (Int32)ff9LEVEL_BASE.Gems;
			e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
			e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
			Int32 val = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
			if (val >= 0)
				gemCount = Math.Min(val, UInt16.MaxValue); // "player.basis.cap" is a UInt16, so it's better to force a limit there
		}
		return gemCount;
	}

	public static UInt32 FF9Level_GetHp(Int32 lv, Int32 str)
	{
		UInt32 maxHp = (UInt32)(ff9level.CharacterLevelUps[lv - 1].BonusHP * str / 50);
		if (maxHp > 9999)
			maxHp = 9999;
		return maxHp;
	}

	public static UInt32 FF9Level_GetMp(Int32 lv, Int32 mgc)
	{
		UInt32 maxMp = (UInt32)(ff9level.CharacterLevelUps[lv - 1].BonusMP * mgc / 100);
		if (maxMp > 999)
			maxMp = 999;
		return maxMp;
	}

	public static Int32 FF9Level_GetEquipBonus(CharacterEquipment equip, Int32 base_type)
	{
		Int32 bonus = 0;
		for (Int32 i = 0; i < 5; i++)
		{
			if (equip[i] != 255)
			{
				FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[equip[i]];
				ItemStats equip_PRIVILEGE = ff9equip.ItemStatsData[ff9ITEM_DATA.bonus];
				switch (base_type)
				{
				case 0:
					bonus += equip_PRIVILEGE.dex;
					break;
				case 1:
					bonus += equip_PRIVILEGE.str;
					break;
				case 2:
					bonus += equip_PRIVILEGE.mgc;
					break;
				case 3:
					bonus += equip_PRIVILEGE.wpr;
					break;
				}
			}
		}
		return bonus;
	}

	// public const Byte FF9LEVEL_DEX_MAX = 50;
	// public const Byte FF9LEVEL_STR_MAX = 99;
	// public const Byte FF9LEVEL_MGC_MAX = 99;
	// public const Byte FF9LEVEL_WPR_MAX = 50;
	// public const Byte FF9LEVEL_CAP_MAX = 99;
	// public const Int32 FF9LEVEL_HP_MAX = 9999;
	// public const Int32 FF9LEVEL_MP_MAX = 999;
}
