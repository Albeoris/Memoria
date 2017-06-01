using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memoria.Prime.Ini;

namespace Memoria.Assets.Import.Graphics
{
    public sealed class AtlasInfo : Ini
    {
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

        private readonly AtlasSection _atlasSection = new AtlasSection();

        public Int32 TotalAtlasesFromAtlasSection => _atlasSection.TotalAtlases.Value;
        public Int32 TileSizeFromAtlasSection => _atlasSection.TileSize.Value;
        public Int32 AtlasSideFromAtlasSection => _atlasSection.AtlasSide.Value;

        public override IEnumerable<IniSection> GetSections()
        {
            yield return _atlasSection;
        }

        private sealed class AtlasSection : IniSection
        {
            public readonly IniValue<Int32> TotalAtlases = IniValue.Int32(nameof(TotalAtlases));
            public readonly IniValue<Int32> TileSize = IniValue.Int32(nameof(TileSize));
            public readonly IniValue<Int32> AtlasSide = IniValue.Int32(nameof(AtlasSide));

            public AtlasSection() : base("AtlasSection")
            {
                TileSize.Value = 32; // DefaultValue
                TotalAtlases.Value = 1;
                AtlasSide.Value = 2048;
                
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return TileSize;
                yield return TotalAtlases;
                yield return AtlasSide;
            }
        }
    }
}
