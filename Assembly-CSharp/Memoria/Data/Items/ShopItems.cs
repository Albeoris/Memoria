using System;
using System.Linq;
using System.Collections.Generic;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class ShopItems : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public RegularItem[] ItemIds;

        public RegularItem this[Int32 index] => ItemIds[index];
        public Int32 Length => ItemIds.Length;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            List<RegularItem> itemIds = new List<RegularItem>();
            for (Int32 i = 2; i < raw.Length; i++)
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
                    itemIds.Add(itemId);
                }
                if (stop)
                    break;
            }
            ItemIds = itemIds.ToArray();
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32(Id);

            sw.ItemArray(ItemIds.Select(it => (Int32)it).ToArray());
        }
    }
}
