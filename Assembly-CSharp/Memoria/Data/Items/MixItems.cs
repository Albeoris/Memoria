using System;
using System.Linq;
using System.Collections.Generic;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public class MixItems : ICsvEntry
    {
        public String Comment;
        public Int32 Id;
        public RegularItem Result;
        public List<RegularItem> Ingredients = new List<RegularItem>();

        public RegularItem this[Int32 index] => Ingredients[index];
        public Int32 Length => Ingredients.Count;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);
            Result = (RegularItem)CsvParser.Int32(raw[2]);

            Ingredients.Clear();
            for (Int32 i = 3; i < raw.Length; i++)
            {
                String value = raw[i];
                if (String.IsNullOrEmpty(value))
                    continue;

                Int32[] itemArray = CsvParser.ItemArray(value);
                Boolean stop = false;
                foreach (Int32 itemInt in itemArray)
                {
                    RegularItem itemId = (RegularItem)itemInt;
                    if (itemId == RegularItem.NoItem)
                    {
                        stop = true;
                        break;
                    }                 
                    Ingredients.Add(itemId);
                }
                if (stop)
                    break;
            }
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            writer.String(Comment);
            writer.Int32(Id);
            writer.Item((Int32)Result);

            writer.ItemArray(Ingredients.Select(it => (Int32)it).ToArray());
        }

        public Dictionary<RegularItem, Int32> GetIngredientsAsDict()
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
    }
}
