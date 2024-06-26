using Memoria.Prime.CSV;
using System;
using UnityEngine;

namespace Memoria.Data
{
    public class SHPPrototype : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public String TextureBasePath;
        public Int32 TextureCount;
        public Byte ShaderType;
        public Int32 CycleDuration;

        private Texture2D[] textures = null;
        public Texture2D[] Textures
        {
            get
            {
                if (textures != null)
                    return textures;
                textures = new Texture2D[TextureCount];
                for (Int32 i = 0; i < TextureCount; i++)
                {
                    textures[i] = AssetManager.Load<Texture2D>(TextureBasePath.Replace("%", (i + 1).ToString()));
                    if (textures[i] == null)
                        textures[i] = new Texture2D(0, 0);
                }
                return textures;
            }
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;

            Comment = CsvParser.String(raw[index++]);
            Id = CsvParser.Int32(raw[index++]);

            TextureBasePath = CsvParser.String(raw[index++]);
            TextureCount = CsvParser.Int32(raw[index++]);
            ShaderType = CsvParser.Byte(raw[index++]);
            CycleDuration = CsvParser.Int32(raw[index++]);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32(Id);

            sw.String(TextureBasePath);
            sw.Int32(TextureCount);
            sw.Byte(ShaderType);
            sw.Int32(CycleDuration);
        }
    }
}
