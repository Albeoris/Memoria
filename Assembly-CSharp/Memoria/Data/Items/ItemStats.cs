using Memoria.Prime.CSV;
using System;

namespace FF9
{
    public class ItemStats : ICsvEntry
    {
        public String Comment;
        public Int32 Id;
        public ItemStats Data;

        public Byte dex;
        public Byte str;
        public Byte mgc;
        public Byte wpr;
        public Byte p_up_attr;
        public DEF_ATTR def_attr = new DEF_ATTR();

        public static ItemStats GetExisting(Int32 id)
        {
            if (ff9equip.ItemStatsData.TryGetValue(id, out ItemStats result))
                return result;
            throw new NotSupportedException($"The option AppendMode must be used to patch existing entries but the entry {id} doesn't exist");
        }

        public void ParseDataEntry(String[] raw, CsvMetaData metadata, ref Int32 index)
        {
            if (metadata.HasField("Speed")) dex = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("Strength")) str = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("Magic")) mgc = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("Spirit")) wpr = CsvParser.Byte(raw[index++]);

            if (metadata.HasField("BonusElement")) p_up_attr = CsvParser.Byte(raw[index++]);

            if (metadata.HasField("GuardElement")) def_attr.invalid = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("AbsorbElement")) def_attr.absorb = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("HalfElement")) def_attr.half = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("WeakElement")) def_attr.weak = CsvParser.Byte(raw[index++]);
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

            sw.Byte(dex);
            sw.Byte(str);
            sw.Byte(mgc);
            sw.Byte(wpr);

            sw.Byte(p_up_attr);

            sw.Byte(def_attr.invalid);
            sw.Byte(def_attr.absorb);
            sw.Byte(def_attr.half);
            sw.Byte(def_attr.weak);
        }
    }
}
