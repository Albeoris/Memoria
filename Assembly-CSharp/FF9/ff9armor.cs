using System;
using System.IO;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;

namespace FF9
{
	public class ff9armor
	{
	    public const Byte FF9ARMOR_START = 88;

        public static readonly EntryCollection<ItemDefence> ArmorData;

        static ff9armor()
	    {
	        ArmorData = LoadArmors();
	    }

        private static EntryCollection<ItemDefence> LoadArmors()
        {
            try
            {
                String inputPath = DataResources.Items.Directory + DataResources.Items.ArmorsFile;
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"[ff9armor] Cannot load armors because a file does not exist: [{inputPath}].", inputPath);

                ItemDefence[] items = CsvReader.Read<ItemDefence>(inputPath);
                if (items.Length < 136)
                    throw new NotSupportedException($"You must set at least 136 armors, but there {items.Length}. Any number of items will be available after a game stabilization.");

                EntryCollection<ItemDefence> result = EntryCollection.CreateWithDefaultElement(items, i => i.Id);
                for (Int32 i = Configuration.Mod.FolderNames.Length-1; i >= 0; i--)
                {
                    inputPath = DataResources.Items.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.Items.ArmorsFile;
                    if (File.Exists(inputPath))
                    {
                        items = CsvReader.Read<ItemDefence>(inputPath);
                        foreach (ItemDefence it in items)
                            result[it.Id] = it;
                    }
                }
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
