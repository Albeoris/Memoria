using Memoria.Prime.Ini;
using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class FontSection : IniSection
        {
            private static readonly String[] DefaultFonts = { "Arial", "Times Bold" };

            public readonly IniArray<String> Names;
            public readonly IniValue<Int32> Size;

            public FontSection() : base(nameof(Font), false)
            {
                Names = BindStringArray(nameof(Names), DefaultFonts);
                Size = BindInt32(nameof(Size), 24);
            }
        }
    }
}
