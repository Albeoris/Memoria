using System;
using System.IO;
using System.Collections.Generic;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;

public static class TripleTriad
{
    public static readonly Dictionary<TripleTriadId, TripleTriadCard> TripleTriadCardStats;

    static TripleTriad()
    {
        TripleTriadCardStats = LoadBaseStats();
    }

    private static Dictionary<TripleTriadId, TripleTriadCard> LoadBaseStats()
    {
        try
        {
            String inputPath = DataResources.TetraMaster.PureDirectory + DataResources.TetraMaster.TripleTriadFile;
            Dictionary<TripleTriadId, TripleTriadCard> result = new Dictionary<TripleTriadId, TripleTriadCard>();
            foreach (TripleTriadCard[] stats in AssetManager.EnumerateCsvFromLowToHigh<TripleTriadCard>(inputPath))
                foreach (TripleTriadCard stat in stats)
                    result[stat.Id] = stat;
            if (result.Count == 0)
                throw new FileNotFoundException($"Cannot load triple triad card because a file does not exist: [{DataResources.TetraMaster.Directory + DataResources.TetraMaster.TripleTriadFile}].", DataResources.Characters.Directory + DataResources.TetraMaster.TripleTriadFile);
            for (Int32 i = 0; i < 99; i++)
                if (!result.ContainsKey((TripleTriadId)i))
                    throw new NotSupportedException($"You must set base stats for at least 100 card, with IDs between 0 and 99.");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[TripleTriad] Load triple triad card failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }
}