using System;
using System.Collections.Generic;
using System.IO;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;

namespace FF9
{
    public class ff9mixitem
    {
        public static readonly Dictionary<RegularItem, MixItems> MixItemsData;

        static ff9mixitem()
        {
            MixItemsData = LoadSynthesis();
        }

        private static Dictionary<RegularItem, MixItems> LoadSynthesis()
        {
            try
            {
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.MixItemsFile;
                Dictionary<RegularItem, MixItems> result = new Dictionary<RegularItem, MixItems>();
                foreach (MixItems[] ingredients in AssetManager.EnumerateCsvFromLowToHigh<MixItems>(inputPath))
                    foreach (MixItems ItemMixed in ingredients)
                        result[(RegularItem)ItemMixed.Result] = ItemMixed;
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9mixitem] Load mix items failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }
    }
}
