using Memoria.Assets;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.IO;

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
				String inputPath = DataResources.Items.PureDirectory + DataResources.Items.StatsFile;
				Dictionary<Int32, ItemStats> result = new Dictionary<Int32, ItemStats>();
				foreach (ItemStats[] stats in AssetManager.EnumerateCsvFromLowToHigh<ItemStats>(inputPath))
					foreach (ItemStats stat in stats)
						result[stat.Id] = stat;
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
