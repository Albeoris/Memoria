using System;
using System.Collections.Generic;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class ShopItems : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public Byte[] ItemIds;

        public Byte this[Int32 index] => ItemIds[index];
        public Int32 Length => ItemIds.Length;

        public void ParseEntry(String[] raw)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            List<Byte> itemIds = new List<Byte>(raw.Length - 1);
            for (Int32 i = 2; i < raw.Length; i++)
            {
                String value = raw[i];
                if (String.IsNullOrEmpty(value))
                    continue;

                Byte itemId = CsvParser.ByteOrMinusOne(value);
                if (itemId == Byte.MaxValue)
                    break;

                itemIds.Add(itemId);
            }
            ItemIds = itemIds.ToArray();
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.String(Comment);
            sw.Int32(Id);

            foreach (Byte itemId in ItemIds)
                sw.ByteOrMinusOne(itemId);

            sw.ByteOrMinusOne(Byte.MaxValue);
        }
    }
}
