using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memoria.Prime.Ini;

namespace Memoria.Assets.Import.Graphics
{
    class PsdInfo : Ini
    { 
       public static void Sample()
        {
            PsdInfo ini = PsdInfo.Load("PsdInfo.ini");
            String LayerOrder = ini.LayerOrderFromPsdSection;
            Int32 Reversed = ini.ReversedFromPsdSection;
        }

        public static PsdInfo Load(String filePath)
        {
            PsdInfo result = new PsdInfo();

            IniReader reader = new IniReader(filePath);
            reader.Read(result);

            return result;
        }

        private readonly PsdSection _psdSection = new PsdSection();

        public String LayerOrderFromPsdSection => _psdSection.LayerOrder.Value;
        public Int32 ReversedFromPsdSection => _psdSection.Reversed.Value;

        public override IEnumerable<IniSection> GetSections()
        {
            yield return _psdSection;
        }

        private sealed class PsdSection : IniSection
        {
            public readonly IniValue<String> LayerOrder = IniValue.String(nameof(LayerOrder));
            public readonly IniValue<Int32> Reversed = IniValue.Int32(nameof(Reversed));

            public PsdSection() : base("PsdSection")
            {
                LayerOrder.Value = "name"; // DefaultValue
                Reversed.Value = 0 ;

            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return LayerOrder;
                yield return Reversed;
            }
        }
    }
}
