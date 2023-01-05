using System;
using System.IO;
using System.Collections.Generic;
using Memoria;
using Memoria.Assets;
using Memoria.Prime;
using Memoria.Prime.CSV;

namespace FF9
{
	public class ff9equip
	{
        public static readonly Dictionary<Int32, ItemStats> ItemStatsData;

        static ff9equip()
        {
            ItemStatsData = LoadStats();
        }

	    private static Dictionary<Int32, ItemStats> LoadStats()
	    {
	        try
            {
                Dictionary<Int32, ItemStats> result = new Dictionary<Int32, ItemStats>();
                ItemStats[] stats;
                String inputPath;
                String[] dir = Configuration.Mod.AllFolderNames;
                for (Int32 i = dir.Length - 1; i >= 0; --i)
                {
                    inputPath = DataResources.Items.ModDirectory(dir[i]) + DataResources.Items.StatsFile;
                    if (File.Exists(inputPath))
                    {
                        stats = CsvReader.Read<ItemStats>(inputPath);
                        for (Int32 j = 0; j < stats.Length; j++)
                            result[stats[j].Id] = stats[j];
                    }
                }
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load item stats because a file does not exist: [{DataResources.Items.Directory + DataResources.Items.StatsFile}].", DataResources.Items.Directory + DataResources.Items.StatsFile);
                return result;
	        }
	        catch (Exception ex)
	        {
	            Log.Error(ex, $"[{nameof(ff9equip)}] Load item stats failed.");
	            UIManager.Input.ConfirmQuit();
	            return null;
	        }
	    }
	}
}
