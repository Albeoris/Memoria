using System;
using System.IO;
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
                String inputPath = DataResources.Items.ArmorsFile;
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"[ff9armor] Cannot load armors because a file does not exist: [{inputPath}].", inputPath);

                ItemDefence[] items = CsvReader.Read<ItemDefence>(inputPath);
                if (items.Length < 136)
                    throw new NotSupportedException($"You must set at least 136 armors, but there {items.Length}. Any number of items will be available after a game stabilization.");

                return EntryCollection.CreateWithDefaultElement(items, i => i.Id);
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
