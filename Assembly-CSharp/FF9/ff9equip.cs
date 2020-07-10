using System;
using System.IO;
using Memoria;
using Memoria.Assets;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;

namespace FF9
{
	public class ff9equip
	{
		public const Byte FF9EQUIP_NONE = 255;

        public static readonly EntryCollection<ItemStats> ItemStatsData;

        static ff9equip()
        {
            ItemStatsData = LoadStats();
        }

	    private static EntryCollection<ItemStats> LoadStats()
	    {
	        try
	        {
	            String inputPath = DataResources.Items.Directory + DataResources.Items.StatsFile;
	            if (!File.Exists(inputPath))
	                throw new FileNotFoundException($"[{nameof(ff9equip)}] Cannot load items stats because a file does not exist: [{inputPath}].", inputPath);

	            ItemStats[] items = CsvReader.Read<ItemStats>(inputPath);
	            if (items.Length < 88)
	                throw new NotSupportedException($"[{nameof(ff9equip)}] You must set at least 176 item stats, but there {items.Length}. Any number of items will be available after a game stabilization.");

				EntryCollection<ItemStats> result = EntryCollection.CreateWithDefaultElement(items, i => i.Id);
				for (Int32 i = Configuration.Mod.FolderNames.Length - 1; i >= 0; i--)
				{
					inputPath = DataResources.Items.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.Items.StatsFile;
					if (File.Exists(inputPath))
					{
						items = CsvReader.Read<ItemStats>(inputPath);
						foreach (ItemStats it in items)
							result[it.Id] = it;
					}
				}
				return result;
	        }
	        catch (Exception ex)
	        {
	            Log.Error(ex, $"[{nameof(ff9equip)}] Load weapons failed.");
	            UIManager.Input.ConfirmQuit();
	            return null;
	        }
	    }
	}
}
