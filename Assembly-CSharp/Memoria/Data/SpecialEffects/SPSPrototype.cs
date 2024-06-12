using System;
using Memoria.Prime.CSV;
using UnityEngine;

namespace Memoria.Data
{
    public class SPSPrototype : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public String BinaryPath;
        public String TexturePath;
        public Byte ShaderType;
        public Single BattleScale = 4f;
        public Single BattleDistance = 5f;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;

            Comment = CsvParser.String(raw[index++]);
            Id = CsvParser.Int32(raw[index++]);

            BinaryPath = CsvParser.String(raw[index++]);
            TexturePath = CsvParser.String(raw[index++]);
            ShaderType = CsvParser.Byte(raw[index++]);
            BattleScale = CsvParser.Single(raw[index++]);
            BattleDistance = CsvParser.Single(raw[index++]);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32(Id);

            sw.String(BinaryPath);
            sw.String(TexturePath);
            sw.Byte(ShaderType);
            sw.Single(BattleScale);
            sw.Single(BattleDistance);
        }
    }
}
