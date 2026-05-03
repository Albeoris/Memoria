using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;

namespace FF9
{
    public class ff9mixitem
    {
        public static readonly Dictionary<Int32, MixItems> MixItemsData;

        static ff9mixitem()
        {
            MixItemsData = LoadMixes();
        }

        private static Dictionary<Int32, MixItems> LoadMixes()
        {
            try
            {
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.MixItemsFile;
                Dictionary<Int32, MixItems> result = new Dictionary<Int32, MixItems>();
                foreach (MixItems[] mixDatabase in AssetManager.EnumerateCsvFromLowToHigh<MixItems>(inputPath))
                    foreach (MixItems mix in mixDatabase)
                        result[mix.Id] = mix;

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
