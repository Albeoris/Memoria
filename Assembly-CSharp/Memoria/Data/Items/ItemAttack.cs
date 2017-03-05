using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public class ItemAttack : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public Byte Category;
        public Byte StatusIndex;
        public String ModelName;
        public UInt16 ModelId;
        public BTL_REF Ref;
        public Int16 Offset1;
        public Int16 Offset2;

        public void ParseEntry(String[] raw)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            Category = CsvParser.Byte(raw[2]);
            StatusIndex = CsvParser.Byte(raw[3]);
            ModelName = CsvParser.String(raw[4]);
            if (!String.IsNullOrEmpty(ModelName))
                ModelId = (UInt16)FF9BattleDB.GEO.GetKey(ModelName);

            Byte scriptId = CsvParser.Byte(raw[5]);
            Byte power = CsvParser.Byte(raw[6]);
            Byte elements = CsvParser.Byte(raw[7]);
            Byte rate = CsvParser.Byte(raw[8]);
            Ref = new BTL_REF(scriptId, power, elements, rate);

            Offset1 = Int16.Parse(raw[9]);
            Offset2 = Int16.Parse(raw[10]);
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.String(Comment);
            sw.Int32(Id);

            sw.Byte(Category);
            sw.Byte(StatusIndex);
            sw.String(ModelName);

            BTL_REF btl = Ref;
            sw.Byte(btl.ScriptId);
            sw.Byte(btl.Power);
            sw.Byte(btl.Elements);
            sw.Byte(btl.Rate);

            sw.Int16(Offset1);
            sw.Int16(Offset2);
        }
    }
}