using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memoria.Prime.Ini;

namespace Memoria.Assets.Import.Graphics
{
    class PsdInfo : Ini
    {
        #region Static

        public static void Sample()
        {
            PsdInfo ini = PsdInfo.Load("PsdInfo.ini");
            Boolean LayerOrder = ini.OrderByDepth;
            Boolean Reversed = ini.ReverseOrder;
        }

        public static PsdInfo Load(String filePath)
        {
            PsdInfo result = new PsdInfo();

            IniReader reader = new IniReader(filePath);
            reader.Read(result);

            return result;
        }

        #endregion

        private PsdSection _psdSection;

        public Boolean OrderByDepth => _psdSection.OrderByDepth.Value;
        public Boolean ReverseOrder => _psdSection.ReverseOrder.Value;

        public PsdInfo()
        {
            BindingSection(out _psdSection, v => _psdSection = v);
        }

        private sealed class PsdSection : IniSection
        {
            public readonly IniValue<Boolean> OrderByDepth;
            public readonly IniValue<Boolean> ReverseOrder;

            public PsdSection() : base(nameof(PsdSection), true)
            {
                OrderByDepth = BindBoolean(nameof(OrderByDepth), false);
                ReverseOrder = BindBoolean(nameof(ReverseOrder), false);
            }
        }
    }
}