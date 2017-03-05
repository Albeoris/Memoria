using System;
using System.IO;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;

namespace FF9
{
	public class ff9weap
	{
	    public const Int32 FF9WEAPON_START = 0;

        public static readonly EntryCollection<ItemAttack> WeaponData;

        static ff9weap()
	    {
	        WeaponData = LoadWeapons();
	    }

        private static EntryCollection<ItemAttack> LoadWeapons()
        {
            try
            {
                String inputPath = DataResources.Items.WeaponsFile;
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"[ff9weap] Cannot load weapons because a file does not exist: [{inputPath}].", inputPath);

                ItemAttack[] items = CsvReader.Read<ItemAttack>(inputPath);
                if (items.Length < 88)
                    throw new NotSupportedException($"You must set at least 88 weapons, but there {items.Length}. Any number of items will be available after a game stabilization.");

                return EntryCollection.CreateWithDefaultElement(items, i => i.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9weap] Load weapons failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }
	}
}
