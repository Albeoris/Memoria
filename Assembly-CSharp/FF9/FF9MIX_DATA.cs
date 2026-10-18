using Memoria.Data;
using Memoria.Prime.CSV;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FF9
{
    public class FF9MIX_DATA : ICsvEntry
    {
        public String Comment;
        public Int32 Id;
        public FF9MIX_DATA Data;

        public HashSet<Int32> Shops;
        public UInt32 Price;
        public RegularItem Result;
        public RegularItem[] Ingredients;

        public static FF9MIX_DATA GetExisting(Int32 id)
        {
            if (ff9mix.SynthesisData.TryGetValue(id, out FF9MIX_DATA result))
                return result;
            throw new NotSupportedException($"The option AppendMode must be used to patch existing entries but the entry {id} doesn't exist");
        }

        public void ParseDataEntry(String[] raw, CsvMetaData metadata, ref Int32 index)
        {
            if (metadata.HasField("Shops"))
            {
                if (metadata.HasOption("UseShopList"))
                {
                    Int32[] synthArray = CsvParser.Int32Array(raw[index++]);
                    Shops = [.. synthArray];
                }
                else
                {
                    Byte synthFlags = CsvParser.Byte(raw[index++]);
                    Shops = new HashSet<Int32>();
                    for (Int32 i = 0; i < 8; i++)
                        if ((synthFlags & (1 << i)) != 0)
                            Shops.Add(32 + i);
                }
            }
            if (metadata.HasField("Price")) Price = CsvParser.UInt32(raw[index++]);
            if (metadata.HasField("Result")) Result = (RegularItem)CsvParser.Item(raw[index++]);

            if (metadata.HasField("Ingredients"))
            {
                List<RegularItem> ingredientList = new List<RegularItem>();
                while (index < raw.Length)
                {
                    Int32[] itemArray = CsvParser.ItemArray(raw[index++]);
                    foreach (Int32 itemInt in itemArray)
                        ingredientList.Add((RegularItem)itemInt);
                }
                Ingredients = ingredientList.ToArray();
            }
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Comment = CsvParser.String(raw[index++]);
            Id = CsvParser.Int32(raw[index++]);
            Data = metadata.IsAppendMode ? GetExisting(Id) : this;
            Data.ParseDataEntry(raw, metadata, ref index);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32(Id);

            if (metadata.HasOption("UseShopList"))
            {
                sw.Int32Array(Shops.ToArray());
            }
            else
            {
                Byte synthFlags = 0;
                for (Int32 i = 0; i < 8; i++)
                    if (Shops.Contains(32 + i))
                        synthFlags |= (Byte)(1 << i);
                sw.Byte(synthFlags);
            }
            sw.UInt32(Price);
            sw.Item((Int32)Result);

            sw.ItemArray(Ingredients.Select(it => (Int32)it).ToArray());
        }

        public Dictionary<RegularItem, Int32> IngredientsAsDictionary()
        {
            Dictionary<RegularItem, Int32> ingrCount = new Dictionary<RegularItem, Int32>();
            foreach (RegularItem ingr in Ingredients)
            {
                if (ingr == RegularItem.NoItem)
                    continue;
                if (!ingrCount.TryGetValue(ingr, out Int32 count))
                    count = 0;
                ingrCount[ingr] = ++count;
            }
            return ingrCount;
        }

        public Boolean CanBeSynthesized()
        {
            if (ff9item.FF9Item_GetCount(Result) >= ff9item.FF9ITEM_COUNT_MAX || FF9StateSystem.Common.FF9.party.gil < Price)
                return false;
            return !IngredientsAsDictionary().Any(kvp => ff9item.FF9Item_GetCount(kvp.Key) < kvp.Value);
        }
    }
}
