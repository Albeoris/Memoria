using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.IO;

namespace FF9
{
    public static class ff9weap
    {
        public const Int32 WEAPON_START = 0;
        public const Int32 WEAPON_COUNT = 88;

        public static Dictionary<Int32, ItemAttack> WeaponData;

        static ff9weap()
        {
            LoadWeapons();
        }

        private static void LoadWeapons()
        {
            try
            {
                WeaponData = new Dictionary<Int32, ItemAttack>();
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.WeaponsFile;
                foreach (ItemAttack[] attacks in AssetManager.EnumerateCsvFromLowToHigh<ItemAttack>(inputPath))
                    foreach (ItemAttack attack in attacks)
                        WeaponData[attack.Id] = attack.Data;
                if (WeaponData.Count == 0)
                    throw new FileNotFoundException($"Cannot load weapons because a file does not exist: [{DataResources.Items.Directory + DataResources.Items.WeaponsFile}].", DataResources.Items.Directory + DataResources.Items.WeaponsFile);
                for (Int32 i = 0; i < WEAPON_COUNT; i++)
                    if (!WeaponData.ContainsKey(i))
                        throw new NotSupportedException($"You must define at least the 88 weapons, with IDs between 0 and 87.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9weap] Load weapons failed.");
                UIManager.Input.ConfirmQuit();
            }
        }
    }
}
