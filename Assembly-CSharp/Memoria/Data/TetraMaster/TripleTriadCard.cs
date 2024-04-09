using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public class TripleTriadCard : ICsvEntry
    {
        public String Comment;
        public TetraMasterCardId Id;

        public Byte atk;
        public Byte mdef;
        public Byte matk;
        public Byte pdef;
        public String icon;
        public UInt16 rank;
        public String element;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Id = (TetraMasterCardId)CsvParser.Byte(raw[1]);

            atk = CsvParser.Byte(raw[2]);
            mdef = CsvParser.Byte(raw[3]);
            matk = CsvParser.Byte(raw[4]);
            pdef = CsvParser.Byte(raw[5]);
            icon = CsvParser.String(raw[6]);
            if (metadata.HasOption($"Include{nameof(rank)}"))
                rank = UInt16.Parse(raw[7]);
            else
                rank = 1;
            if (metadata.HasOption($"Include{nameof(element)}"))
                element = CsvParser.String(raw[8]);
            else
                element = "";
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32((Int32)Id);

            sw.Byte(atk);
            sw.Byte(mdef);
            sw.Byte(matk);
            sw.Byte(pdef);
            sw.String(icon);
            sw.UInt16(rank);
            sw.String(element);
        }
    }
}