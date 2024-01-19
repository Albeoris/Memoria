using System;
using System.IO;
using System.Collections.Generic;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;

public static class TripleTriad
{
    public static readonly Dictionary<TetraMasterCardId, TripleTriadCard> TripleTriadCardStats;
    public static readonly Dictionary<Int32, TripleTriadRules> TripleTriadRules;

    static TripleTriad()
    {
        TripleTriadCardStats = LoadBaseStats();
        TripleTriadRules = LoadTripleTriadRules();
    }

    private static Dictionary<TetraMasterCardId, TripleTriadCard> LoadBaseStats()
    {
        try
        {
            String inputPath = DataResources.TetraMaster.PureDirectory + DataResources.TetraMaster.TripleTriadFile;
            Dictionary<TetraMasterCardId, TripleTriadCard> result = new Dictionary<TetraMasterCardId, TripleTriadCard>();
            foreach (TripleTriadCard[] stats in AssetManager.EnumerateCsvFromLowToHigh<TripleTriadCard>(inputPath))
                foreach (TripleTriadCard stat in stats)
                    result[stat.Id] = stat;
            if (result.Count == 0)
                throw new FileNotFoundException($"Cannot load triple triad card because a file does not exist: [{DataResources.TetraMaster.Directory + DataResources.TetraMaster.TripleTriadFile}].");
            for (Int32 i = 0; i < CardPool.TOTAL_CARDS; i++)
                if (!result.ContainsKey((TetraMasterCardId)i))
                    throw new NotSupportedException($"You must set base stats for at least {CardPool.TOTAL_CARDS} card, with IDs between 0 and {CardPool.TOTAL_CARDS - 1}.");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[TripleTriad] Load triple triad card failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    private static Dictionary<Int32, TripleTriadRules> LoadTripleTriadRules()
    {
        try
        {
            String inputPath = DataResources.TetraMaster.PureDirectory + DataResources.TetraMaster.TripleTriadRulesFile;
            Dictionary<Int32, TripleTriadRules> result = new Dictionary<Int32, TripleTriadRules>();
            foreach (TripleTriadRules[] rules in AssetManager.EnumerateCsvFromLowToHigh<TripleTriadRules>(inputPath))
                foreach (TripleTriadRules rule in rules)
                {
                    result[rule.Field] = rule;
                }
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[TripleTriad] Load Triple Triad rules missing.");
            return null;
        }
    }
}