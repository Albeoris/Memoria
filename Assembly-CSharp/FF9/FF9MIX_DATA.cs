using System;
using Memoria.Prime.CSV;

namespace FF9
{
    public class FF9MIX_DATA : ICsvEntry
    {
        private const Int32 SynthesisItemCount = 2;

        public String Comment;
        public Int32 Id;

        public Byte Shops;
        public UInt16 Price;
        public Byte Result;
        public Byte[] Ingredients;

        public void ParseEntry(String[] raw)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            Shops = CsvParser.Byte(raw[2]);
            Price = CsvParser.UInt16(raw[3]);
            Result = CsvParser.Byte(raw[4]);

            Ingredients = new Byte[SynthesisItemCount];

            for (int i = 0; i < SynthesisItemCount; i++)
                Ingredients[i] = CsvParser.Byte(raw[5+i]);
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.String(Comment);
            sw.Int32(Id);

            sw.Byte(Shops);
            sw.UInt16(Price);
            sw.Byte(Result);

            foreach (Byte itemId in Ingredients)
                sw.Byte(itemId);
        }
    }
}