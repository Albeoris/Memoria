using Memoria.Assets;
using Memoria.Prime;
using System;
using System.Collections.Generic;

namespace FF9
{
    public static class ff9mix
    {
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
    }
}
