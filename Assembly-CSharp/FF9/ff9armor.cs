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
                Dictionary<Int32, ItemDefence> result = new Dictionary<Int32, ItemDefence>();
                ItemDefence[] items;
                String inputPath;
                String[] dir = Configuration.Mod.AllFolderNames;
                for (Int32 i = dir.Length - 1; i >= 0; --i)
                {
                    inputPath = DataResources.Items.ModDirectory(dir[i]) + DataResources.Items.ArmorsFile;
                    if (File.Exists(inputPath))
                    {
                        items = CsvReader.Read<ItemDefence>(inputPath);
                        for (Int32 j = 0; j < items.Length; j++)
                            result[items[j].Id] = items[j];
                    }
                }
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load armors because a file does not exist: [{DataResources.Items.Directory + DataResources.Items.ArmorsFile}].", DataResources.Items.Directory + DataResources.Items.ArmorsFile);
                for (Int32 j = 0; j < ARMOR_COUNT; j++)
                    if (!result.ContainsKey(j))
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
