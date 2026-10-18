using Memoria;
using Memoria.Assets;
using Memoria.Prime;
using Memoria.Prime.CSV;
using System;
using System.Collections.Generic;
using System.IO;

namespace FF9
{
    public static class ff9equip
    {
        public static Dictionary<Int32, ItemStats> ItemStatsData;

        static ff9equip()
        {
            LoadStats();
        }

        private static void LoadStats()
        {
            try
            {
                ItemStatsData = new Dictionary<Int32, ItemStats>();
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.StatsFile;
                foreach (ItemStats[] stats in AssetManager.EnumerateCsvFromLowToHigh<ItemStats>(inputPath))
                    foreach (ItemStats stat in stats)
                        ItemStatsData[stat.Id] = stat.Data;
                if (ItemStatsData.Count == 0)
                    throw new FileNotFoundException($"Cannot load item stats because a file does not exist: [{DataResources.Items.Directory + DataResources.Items.StatsFile}].", DataResources.Items.Directory + DataResources.Items.StatsFile);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{nameof(ff9equip)}] Load item stats failed.");
                UIManager.Input.ConfirmQuit();
            }
        }
    }
}
