using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.IO;

namespace FF9
{
    public static class ff9armor
    {
        public const Int32 ARMOR_START = 88;
        public const Int32 ARMOR_COUNT = 136;

        public static Dictionary<Int32, ItemDefence> ArmorData;

        static ff9armor()
        {
            LoadArmors();
        }

        private static void LoadArmors()
        {
            try
            {
                ArmorData = new Dictionary<Int32, ItemDefence>();
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.ArmorsFile;
                foreach (ItemDefence[] defences in AssetManager.EnumerateCsvFromLowToHigh<ItemDefence>(inputPath))
                    foreach (ItemDefence defence in defences)
                        ArmorData[defence.Id] = defence.Data;
                if (ArmorData.Count == 0)
                    throw new FileNotFoundException($"Cannot load armors because a file does not exist: [{DataResources.Items.Directory + DataResources.Items.ArmorsFile}].", DataResources.Items.Directory + DataResources.Items.ArmorsFile);
                for (Int32 i = 0; i < ARMOR_COUNT; i++)
                    if (!ArmorData.ContainsKey(i))
                        throw new NotSupportedException($"You must define at least the 88 armors, with IDs between 0 and 135.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9armor] Load armors failed.");
                UIManager.Input.ConfirmQuit();
            }
        }
    }
}
