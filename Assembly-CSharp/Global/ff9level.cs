using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.CSV;
using NCalc;
using System;
using System.Collections.Generic;
using System.IO;

public static class ff9level
{
    public const Int32 LEVEL_COUNT = 99;

    public static readonly Dictionary<CharacterId, CharacterBaseStats> CharacterBaseStats;
    public static readonly CharacterLevelUp[] CharacterLevelUps;

    static ff9level()
    {
        CharacterBaseStats = LoadBaseStats();
        CharacterLevelUps = LoadLeveling();
    }

    private static Dictionary<CharacterId, CharacterBaseStats> LoadBaseStats()
    {
        try
        {
            String inputPath = DataResources.Characters.PureDirectory + DataResources.Characters.BaseStatsFile;
            Dictionary<CharacterId, CharacterBaseStats> result = new Dictionary<CharacterId, CharacterBaseStats>();
            foreach (CharacterBaseStats[] stats in AssetManager.EnumerateCsvFromLowToHigh<CharacterBaseStats>(inputPath))
                foreach (CharacterBaseStats stat in stats)
                    result[stat.Id] = stat;
            if (result.Count == 0)
                throw new FileNotFoundException($"Cannot load base stats because a file does not exist: [{DataResources.Characters.Directory + DataResources.Characters.BaseStatsFile}].", DataResources.Characters.Directory + DataResources.Characters.BaseStatsFile);
            for (Int32 i = 0; i < 12; i++)
                if (!result.ContainsKey((CharacterId)i))
                    throw new NotSupportedException($"You must set base stats for at least 12 characters, with IDs between 0 and 11.");
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
            String inputPath = DataResources.Characters.PureDirectory + DataResources.Characters.Leveling;
            CharacterLevelUp[] levels = AssetManager.GetCsvWithHighestPriority<CharacterLevelUp>(inputPath);
            if (levels == null)
            {
                inputPath = DataResources.Characters.Directory + DataResources.Characters.Leveling;
                throw new FileNotFoundException($"File with leveling info not found: [{inputPath}]", inputPath);
            }
            if (levels.Length < LEVEL_COUNT)
                throw new NotSupportedException($"You must set level up info for {LEVEL_COUNT} levels, but there {levels.Length}.");
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
        CharacterBaseStats baseStats = ff9level.CharacterBaseStats[player.Index];
        if (lvup)
        {
            Int32 capaBonus = 0;
            Int32 equipBonus = ff9level.FF9Level_GetEquipBonus(player.equip, 0);
            bonus.dex += (UInt16)(capaBonus + equipBonus);
        }
        Int32 speed = Math.Min(50, baseStats.Dexterity + lv / 10 + (bonus.dex >> 5));
        if (!String.IsNullOrEmpty(Configuration.Battle.SpeedStatFormula))
        {
            Expression e = new Expression(Configuration.Battle.SpeedStatFormula);
            NCalcUtility.InitializeExpressionPlayer(ref e, player);
            e.Parameters["Level"] = (Int32)lv; // overrides "player.level"
            e.Parameters["SpeedBonus"] = (Int32)bonus.dex; // As it is, SpeedBonus contains only bonuses from equipment, no bonus is gotten from level ups
            e.Parameters["SpeedBase"] = (Int32)baseStats.Dexterity;
            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            Int32 val = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
            if (val >= 0)
                speed = Math.Min(val, Byte.MaxValue); // "player.basis.dex" is a Byte, so it's better to force a 255 limit there
        }
        return speed;
    }

    public static Int32 FF9Level_GetStr(PLAYER player, Int32 lv, Boolean lvup)
    {
        FF9LEVEL_BONUS bonus = player.bonus;
        CharacterBaseStats baseStats = ff9level.CharacterBaseStats[player.Index];
        if (lvup)
        {
            Int32 capaBonus = (player.cur.capa != 0) ? 0 : 3;
            Int32 equipBonus = ff9level.FF9Level_GetEquipBonus(player.equip, 1);
            bonus.str += (UInt16)(capaBonus + equipBonus);
        }
        Int32 strength = Math.Min(99, baseStats.Strength + lv * 3 / 10 + (bonus.str >> 5));
        if (!String.IsNullOrEmpty(Configuration.Battle.StrengthStatFormula))
        {
            Expression e = new Expression(Configuration.Battle.StrengthStatFormula);
            NCalcUtility.InitializeExpressionPlayer(ref e, player);
            e.Parameters["Level"] = (Int32)lv; // overrides "player.level"
            e.Parameters["StrengthBonus"] = (Int32)bonus.str; // As it is, StrengthBonus contains both bonuses from equipment and from level ups (x3)
            e.Parameters["StrengthBase"] = (Int32)baseStats.Strength;
            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            Int32 val = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
            if (val >= 0)
                strength = Math.Min(val, Byte.MaxValue); // "player.basis.str" is a Byte, so it's better to force a 255 limit there
        }
        return strength;
    }

    public static Int32 FF9Level_GetMgc(PLAYER player, Int32 lv, Boolean lvup)
    {
        FF9LEVEL_BONUS bonus = player.bonus;
        CharacterBaseStats baseStats = ff9level.CharacterBaseStats[player.Index];
        if (lvup)
        {
            Int32 capaBonus = (player.cur.capa != 0) ? 0 : 3;
            Int32 equipBonus = ff9level.FF9Level_GetEquipBonus(player.equip, 2);
            bonus.mgc += (UInt16)(capaBonus + equipBonus);
        }
        Int32 magic = Math.Min(99, baseStats.Magic + lv * 3 / 10 + (bonus.mgc >> 5));
        if (!String.IsNullOrEmpty(Configuration.Battle.MagicStatFormula))
        {
            Expression e = new Expression(Configuration.Battle.MagicStatFormula);
            NCalcUtility.InitializeExpressionPlayer(ref e, player);
            e.Parameters["Level"] = (Int32)lv; // overrides "player.level"
            e.Parameters["MagicBonus"] = (Int32)bonus.mgc; // As it is, MagicBonus contains both bonuses from equipment and from level ups (x3)
            e.Parameters["MagicBase"] = (Int32)baseStats.Magic;
            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            Int32 val = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
            if (val >= 0)
                magic = Math.Min(val, Byte.MaxValue); // "player.basis.mgc" is a Byte, so it's better to force a 255 limit there
        }
        return magic;
    }

    public static Int32 FF9Level_GetWpr(PLAYER player, Int32 lv, Boolean lvup)
    {
        FF9LEVEL_BONUS bonus = player.bonus;
        CharacterBaseStats baseStats = ff9level.CharacterBaseStats[player.Index];
        if (lvup)
        {
            Int32 capaBonus = (player.cur.capa != 0) ? 0 : 1;
            Int32 equipBonus = ff9level.FF9Level_GetEquipBonus(player.equip, 3);
            bonus.wpr += (UInt16)(capaBonus + equipBonus);
        }
        Int32 spirit = Math.Min(50, baseStats.Will + lv * 3 / 20 + (bonus.wpr >> 5));
        if (!String.IsNullOrEmpty(Configuration.Battle.SpiritStatFormula))
        {
            Expression e = new Expression(Configuration.Battle.SpiritStatFormula);
            NCalcUtility.InitializeExpressionPlayer(ref e, player);
            e.Parameters["Level"] = (Int32)lv; // overrides "player.level"
            e.Parameters["SpiritBonus"] = (Int32)bonus.wpr; // As it is, SpiritBonus contains both bonuses from equipment and from level ups (x1)
            e.Parameters["SpiritBase"] = (Int32)baseStats.Will;
            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            Int32 val = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
            if (val >= 0)
                spirit = Math.Min(val, Byte.MaxValue); // "player.basis.wpr" is a Byte, so it's better to force a 255 limit there
        }
        return spirit;
    }

    public static UInt32 FF9Level_GetCap(PLAYER player, Int32 lv, Boolean lvup)
    {
        FF9LEVEL_BONUS bonus = player.bonus;
        CharacterBaseStats baseStats = ff9level.CharacterBaseStats[player.Index];
        // It seems that "bonus.cap" was meant to give a small amount of magic stone bonuses (~1 every 6 levels)
        // but only as long as all the stones are used when leveling up
        // However, since "ff9play.FF9Play_Build" resets "player.cur", the bonus is always given instead
        // Same goes with the few "capaBonus" points given for other stats
        if (lvup)
        {
            Int32 capaBonus = (player.cur.capa != 0) ? 0 : 5;
            Int32 equipBonus = 0;
            bonus.cap += (UInt16)(capaBonus + equipBonus);
        }
        UInt32 gemCount = (UInt32)(baseStats.Gems + lv * 4 / 10 + (bonus.cap >> 5));
        if (!String.IsNullOrEmpty(Configuration.Battle.MagicStoneStockFormula))
        {
            Expression e = new Expression(Configuration.Battle.MagicStoneStockFormula);
            NCalcUtility.InitializeExpressionPlayer(ref e, player);
            e.Parameters["Level"] = lv; // overrides "player.level"
            e.Parameters["MagicStoneBonus"] = (Int32)bonus.cap; // MagicStoneBonus contains the bonus from level ups (x5)
            e.Parameters["MagicStoneBase"] = (Int32)baseStats.Gems;
            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            Int32 val = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
            if (val >= 0)
                gemCount = (UInt32)val;
        }
        return gemCount;
    }

    public static UInt32 FF9Level_GetHp(PLAYER play, Int32 lv, Int32 str)
    {
        UInt32 maxHp = (UInt32)(ff9level.CharacterLevelUps[lv - 1].BonusHP * str / 50);
        if (maxHp > play.maxHpLimit)
            maxHp = play.maxHpLimit;
        return maxHp;
    }

    public static UInt32 FF9Level_GetMp(PLAYER play, Int32 lv, Int32 mgc)
    {
        UInt32 maxMp = (UInt32)(ff9level.CharacterLevelUps[lv - 1].BonusMP * mgc / 100);
        if (maxMp > play.maxMpLimit)
            maxMp = play.maxMpLimit;
        return maxMp;
    }

    public static Int32 FF9Level_GetEquipBonus(CharacterEquipment equip, Int32 base_type)
    {
        Int32 bonus = 0;
        for (Int32 i = 0; i < 5; i++)
        {
            if (equip[i] != RegularItem.NoItem)
            {
                FF9ITEM_DATA itemData = ff9item._FF9Item_Data[equip[i]];
                ItemStats equipPrivilege = ff9equip.ItemStatsData[itemData.bonus];
                switch (base_type)
                {
                    case 0:
                        bonus += equipPrivilege.dex;
                        break;
                    case 1:
                        bonus += equipPrivilege.str;
                        break;
                    case 2:
                        bonus += equipPrivilege.mgc;
                        break;
                    case 3:
                        bonus += equipPrivilege.wpr;
                        break;
                }
            }
        }
        return bonus;
    }
}
