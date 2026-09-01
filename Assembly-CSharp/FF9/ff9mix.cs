using Memoria.Assets;
using Memoria.Prime;
using System;
using System.Collections.Generic;

namespace FF9
{
    public static class ff9mix
    {
        public static Int32 FF9MIX_SRC_MAX = 2;
        public static Dictionary<Int32, FF9MIX_DATA> SynthesisData;

        static ff9mix()
        {
            LoadSynthesis();
        }

        private static void LoadSynthesis()
        {
            try
            {
                SynthesisData = new Dictionary<Int32, FF9MIX_DATA>();
                String inputPath = DataResources.Items.PureDirectory + DataResources.Items.SynthesisFile;
                foreach (FF9MIX_DATA[] mixes in AssetManager.EnumerateCsvFromLowToHigh<FF9MIX_DATA>(inputPath))
                    foreach (FF9MIX_DATA mix in mixes)
                        SynthesisData[mix.Id] = mix.Data;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9mix] Load synthesis info failed.");
                UIManager.Input.ConfirmQuit();
            }
        }
    }
}
