using System;
using Memoria.Data;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class IdMap : ICsvEntry
    {
        // Note: this MappedId can be defined by a column in Actions.csv instead
        public String Comment;
        public BattleAbilityId Id;
        public Byte MappedId;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Id = (BattleAbilityId)CsvParser.Int32(raw[1]);
            MappedId = CsvParser.Byte(raw[2]);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32((Int32)Id);
            sw.Byte(MappedId);
        }
    }
}