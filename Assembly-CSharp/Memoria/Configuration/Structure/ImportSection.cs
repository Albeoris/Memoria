using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class ImportSection : IniSection
        {
            public readonly IniValue<String> Path;
            public readonly IniValue<Boolean> Text;
            public readonly IniValue<Boolean> Graphics;
            public readonly IniValue<Boolean> Field;
            public readonly IniValue<Boolean> Audio;

            public ImportSection() : base(nameof(ImportSection), false)
            {
                Path = BindPath(nameof(Path), "%StreamingAssets%");
                Text = BindBoolean(nameof(Text), false);
                Graphics = BindBoolean(nameof(Graphics), false);
                Audio = BindBoolean(nameof(Audio), false);
                Field = BindBoolean(nameof(Field), false);
            }
        }
    }
}