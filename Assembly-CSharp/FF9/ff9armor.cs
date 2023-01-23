using System;
using System.IO;
using System.Collections.Generic;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;

namespace FF9
{
	public static class ff9armor
	{
	    public const Int32 ARMOR_START = 88;
        public const Int32 ARMOR_COUNT = 136;

        public static Dictionary<Int32, ItemDefence> ArmorData;

        static ff9armor()
	    {
	        ArmorData = LoadArmors();
	    }

        private static Dictionary<Int32, ItemDefence> LoadArmors()
        {
            try
            {
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.ArmorsFile;
                Dictionary<Int32, ItemDefence> result = new Dictionary<Int32, ItemDefence>();
                foreach (ItemDefence[] defences in AssetManager.EnumerateCsvFromLowToHigh<ItemDefence>(inputPath))
                    foreach (ItemDefence defence in defences)
                        result[defence.Id] = defence;
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load armors because a file does not exist: [{DataResources.Items.Directory + DataResources.Items.ArmorsFile}].", DataResources.Items.Directory + DataResources.Items.ArmorsFile);
                for (Int32 i = 0; i < ARMOR_COUNT; i++)
                    if (!result.ContainsKey(i))
                        throw new NotSupportedException($"You must define at least the 88 armors, with IDs between 0 and 135.");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9armor] Load armors failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }
	}
}
