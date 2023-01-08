using System;
using System.Linq;
using System.Collections.Generic;
using Memoria.Data;
using Memoria.Prime.CSV;

namespace FF9
{
    public class FF9MIX_DATA : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public HashSet<Int32> Shops;
        public UInt32 Price;
        public RegularItem Result;
        public RegularItem[] Ingredients;

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

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            if (metadata.HasOption("UseShopList"))
            {
                Int32[] synthArray = CsvParser.Int32Array(raw[2]);
                Shops = new HashSet<Int32>(synthArray);
            }
            else
			{
                Byte synthFlags = CsvParser.Byte(raw[2]);
                Shops = new HashSet<Int32>();
                for (Int32 i = 0; i < 8; i++)
                    if ((synthFlags & (1 << i)) != 0)
                        Shops.Add(32 + i);
            }
            Price = CsvParser.UInt32(raw[3]);
            Result = (RegularItem)CsvParser.Item(raw[4]);

            List<RegularItem> ingredientList = new List<RegularItem>();
            for (Int32 i = 5; i < raw.Length; i++)
            {
                Int32[] itemArray = CsvParser.ItemArray(raw[i]);
                foreach (Int32 itemInt in itemArray)
                    ingredientList.Add((RegularItem)itemInt);
            }
            Ingredients = ingredientList.ToArray();
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
    }
}