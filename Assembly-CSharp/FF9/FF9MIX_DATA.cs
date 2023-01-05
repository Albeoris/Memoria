using System;
using System.Linq;
using System.Collections.Generic;
using Memoria.Data;
using Memoria.Prime.CSV;

namespace FF9
{
    public class FF9MIX_DATA : ICsvEntry
    {
        private const Int32 SynthesisItemCount = 2;

        public String Comment;
        public Int32 Id;

        public HashSet<Int32> Shops;
        public UInt32 Price;
        public RegularItem Result;
        public RegularItem[] Ingredients;

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

            Ingredients = new RegularItem[SynthesisItemCount];

            for (Int32 i = 0; i < SynthesisItemCount; i++)
                Ingredients[i] = (RegularItem)CsvParser.Item(raw[5+i]);
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

            foreach (RegularItem itemId in Ingredients)
                sw.Item((Int32)itemId);
        }
    }
}