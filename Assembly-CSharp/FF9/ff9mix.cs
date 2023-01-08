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
                String inputPath = DataResources.Items.Directory + DataResources.Items.SynthesisFile;
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"Cannot load synthesis info because a file does not exist: [{inputPath}].", inputPath);
                Dictionary<Int32, FF9MIX_DATA> result = new Dictionary<Int32, FF9MIX_DATA>();
                FF9MIX_DATA[] mixes;
                String[] dir = Configuration.Mod.AllFolderNames;
                for (Int32 i = dir.Length - 1; i >= 0; --i)
                {
                    inputPath = DataResources.Items.ModDirectory(dir[i]) + DataResources.Items.SynthesisFile;
                    if (File.Exists(inputPath))
                    {
                        mixes = CsvReader.Read<FF9MIX_DATA>(inputPath);
                        for (Int32 j = 0; j < mixes.Length; j++)
                            result[mixes[j].Id] = mixes[j];
                    }
                }
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