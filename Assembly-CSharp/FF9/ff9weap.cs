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
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.WeaponsFile;
                Dictionary<Int32, ItemAttack> result = new Dictionary<Int32, ItemAttack>();
                foreach (ItemAttack[] attacks in AssetManager.EnumerateCsvFromLowToHigh<ItemAttack>(inputPath))
                    foreach (ItemAttack attack in attacks)
                        result[attack.Id] = attack;
                if (result.Count == 0)
                    throw new FileNotFoundException($"Cannot load weapons because a file does not exist: [{DataResources.Items.Directory + DataResources.Items.WeaponsFile}].", DataResources.Items.Directory + DataResources.Items.WeaponsFile);
                for (Int32 i = 0; i < WEAPON_COUNT; i++)
                    if (!result.ContainsKey(i))
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
