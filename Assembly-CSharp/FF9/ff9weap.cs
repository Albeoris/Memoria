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
	public static class ff9weap
	{
	    public const Int32 WEAPON_START = 0;
        public const Int32 WEAPON_COUNT = 88;

        public static Dictionary<Int32, ItemAttack> WeaponData;

        static ff9weap()
	    {
	        WeaponData = LoadWeapons();
	    }

        private static Dictionary<Int32, ItemAttack> LoadWeapons()
        {
            try
            {
                Dictionary<Int32, ItemAttack> result = new Dictionary<Int32, ItemAttack>();
                ItemAttack[] items;
                String inputPath;
                String[] dir = Configuration.Mod.AllFolderNames;
                for (Int32 i = dir.Length - 1; i >= 0; --i)
                {
                    inputPath = DataResources.Items.ModDirectory(dir[i]) + DataResources.Items.WeaponsFile;
                    if (File.Exists(inputPath))
                    {
                        items = CsvReader.Read<ItemAttack>(inputPath);
                        for (Int32 j = 0; j < items.Length; j++)
                            result[items[j].Id] = items[j];
                    }
                }
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load weapons because a file does not exist: [{DataResources.Items.Directory + DataResources.Items.WeaponsFile}].", DataResources.Items.Directory + DataResources.Items.WeaponsFile);
                for (Int32 j = 0; j < WEAPON_COUNT; j++)
                    if (!result.ContainsKey(j))
                        throw new NotSupportedException($"You must define at least the 88 weapons, with IDs between 0 and 87.");
                return result;
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
