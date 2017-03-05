using System;
using System.IO;
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
        public static readonly EntryCollection<FF9MIX_DATA> SynthesisData;

        static ff9mix()
        {
            SynthesisData = LoadSynthesis();
        }

        private static EntryCollection<FF9MIX_DATA> LoadSynthesis()
        {
            try
            {
                String inputPath = DataResources.Items.SynthesisFile;
                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"[ff9mix] Cannot load synthesis info because a file does not exist: [{inputPath}].", inputPath);

                FF9MIX_DATA[] items = CsvReader.Read<FF9MIX_DATA>(inputPath);
                if (items.Length < 64)
                    throw new NotSupportedException($"You must set at least 64 synthesis info, but there {items.Length}. Any number of items will be available after a game stabilization.");

                return EntryCollection.CreateWithDefaultElement(items, i => i.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9mix] Load synthesis info failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        public static void FF9Mix_Buy(Byte synthesis_id)
        {
            FF9MIX_DATA ff9MIX_DATA = ff9mix._FF9Mix.item[(Int32)synthesis_id];
            Int32 num;
            if ((num = ff9item.FF9Item_Add((Int32)ff9MIX_DATA.Result, (Int32)ff9mix._FF9Mix.mix_ct)) > 0)
            {
                ff9item.FF9Item_Remove((Int32)ff9MIX_DATA.Ingredients[0], num);
                ff9item.FF9Item_Remove((Int32)ff9MIX_DATA.Ingredients[1], num);
                FF9StateSystem.Common.FF9.party.gil -= UInt32.Parse(((Int32)ff9MIX_DATA.Price * num).ToString());
            }
        }
    }
}