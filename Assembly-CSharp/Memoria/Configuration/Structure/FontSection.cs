using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class FontSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniArray<String> Names = IniValue.StringArray(nameof(Names));
            public readonly IniValue<Int32> Size = IniValue.Int32(nameof(Size));

            public FontSection() : base("Font")
            {
                Enabled.Value = false;
                Names.Value = new[] { "Arial", "Times Bold" };
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return Names;
                yield return Size;
            }
        }
    }
}