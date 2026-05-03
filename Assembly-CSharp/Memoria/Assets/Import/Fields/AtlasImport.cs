using Memoria.Prime.Ini;
using System;
using System.Collections.Generic;
#pragma warning disable 420

namespace Memoria.Assets.Import.Graphics
{
    public sealed class AtlasInfo : Ini
    {
        #region Static

        public static void Sample()
        {
            AtlasInfo ini = AtlasInfo.Load("AtlasInfo.ini");
            Int32 tileSize = ini.TileSizeFromAtlasSection;
            Int32 totalAtlases = ini.TotalAtlasesFromAtlasSection;
            Int32 atlasSide = ini.AtlasSideFromAtlasSection;
        }

        public static AtlasInfo Load(String filePath)
        {
            AtlasInfo result = new AtlasInfo();

            IniReader reader = new IniReader(filePath);
            reader.Read(result);

            return result;
        }

        #endregion

        private volatile AtlasSection _atlasSection;

        public Int32 TotalAtlasesFromAtlasSection => _atlasSection.TotalAtlases.Value;
        public Int32 TileSizeFromAtlasSection => _atlasSection.TileSize.Value;
        public Int32 AtlasSideFromAtlasSection => _atlasSection.AtlasSide.Value;

        public AtlasInfo()
        {
            BindingSection(out _atlasSection, v => _atlasSection = v);
        }

        private sealed class AtlasSection : IniSection
        {
            public readonly IniValue<Int32> TotalAtlases;
            public readonly IniValue<Int32> TileSize;
            public readonly IniValue<Int32> AtlasSide;

            public AtlasSection() : base(nameof(AtlasSection), true)
            {
                TotalAtlases = BindInt32(nameof(TotalAtlases), 1);
                TileSize = BindInt32(nameof(TileSize), 32);
                AtlasSide = BindInt32(nameof(AtlasSide), 2048);
            }
        }
    }
}
