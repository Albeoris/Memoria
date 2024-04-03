using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public class TripleTriadRules : ICsvEntry
    {
        public String Comment;
        public Int32 Field;

        public Boolean One;
        public Boolean Direct;
        public Boolean Diff;
        public Boolean All;
        public Boolean Open;
        public Boolean Random;
        public Boolean SuddenDeath;
        public Boolean Same;
        public Boolean Plus;
        public Boolean SameWall;
        public Boolean Elemental;
        public Boolean RandomRules;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Field = CsvParser.Int32(raw[1]);

            One = CsvParser.Boolean(raw[2]);
            Direct = CsvParser.Boolean(raw[3]);
            Diff = CsvParser.Boolean(raw[4]);
            All = CsvParser.Boolean(raw[5]);
            Open = CsvParser.Boolean(raw[6]);
            Random = CsvParser.Boolean(raw[7]);
            SuddenDeath = CsvParser.Boolean(raw[8]);
            Same = CsvParser.Boolean(raw[9]);
            Plus = CsvParser.Boolean(raw[10]);
            SameWall = CsvParser.Boolean(raw[11]);
            Elemental = CsvParser.Boolean(raw[12]);
            RandomRules = CsvParser.Boolean(raw[13]);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32(Field);

            sw.Boolean(One);
            sw.Boolean(Direct);
            sw.Boolean(Diff);
            sw.Boolean(All);
            sw.Boolean(Open);
            sw.Boolean(Random);
            sw.Boolean(SuddenDeath);
            sw.Boolean(Same);
            sw.Boolean(Plus);
            sw.Boolean(SameWall);
            sw.Boolean(Elemental);
            sw.Boolean(RandomRules);
        }
    }
}