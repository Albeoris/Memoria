using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;
using System;
using System.Collections.Generic;
using System.IO;

namespace FF9
{
    public class ff9mix
    {
        public static FF9MIX_INFO _FF9Mix;
        public static Int32 FF9MIX_SRC_MAX = 2;
        public static readonly Dictionary<Int32, FF9MIX_DATA> SynthesisData;

        static ff9mix()
        {
            SynthesisData = LoadSynthesis();
        }

        private static Dictionary<Int32, FF9MIX_DATA> LoadSynthesis()
        {
            try
            {
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.SynthesisFile;
                Dictionary<Int32, FF9MIX_DATA> result = new Dictionary<Int32, FF9MIX_DATA>();
                foreach (FF9MIX_DATA[] mixes in AssetManager.EnumerateCsvFromLowToHigh<FF9MIX_DATA>(inputPath))
                    foreach (FF9MIX_DATA mix in mixes)
                        result[mix.Id] = mix;
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9mix] Load synthesis info failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        public static void FF9Mix_Buy(Byte synthId)
        {
            FF9MIX_DATA synthData = ff9mix._FF9Mix.item[synthId];
            Int32 synthCount;
            if ((synthCount = ff9item.FF9Item_Add(synthData.Result, ff9mix._FF9Mix.mix_ct)) > 0)
            {
                ff9item.FF9Item_Remove(synthData.Ingredients[0], synthCount);
                ff9item.FF9Item_Remove(synthData.Ingredients[1], synthCount);
                FF9StateSystem.Common.FF9.party.gil -= synthData.Price * (UInt32)synthCount;
            }
        }
    }
}
