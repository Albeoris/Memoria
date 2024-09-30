using Memoria.Assets;
using Memoria.Prime.Ini;
using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class ExportSection : IniSection
        {
            private static readonly String[] DefaultLanguages = { "US", "UK", "JP", "ES", "FR", "GR", "IT" };

            public readonly IniValue<String> Path;
            public readonly IniArray<String> Languages;
            public readonly IniValue<Boolean> Text;
            public readonly IniValue<String> TextFileFormat;
            public readonly IniValue<Boolean> Graphics;
            public readonly IniValue<Boolean> Audio;
            public readonly IniValue<Boolean> Field;
            public readonly IniValue<Boolean> Battle;
            public readonly IniValue<Boolean> Translation;

            public ExportSection() : base(nameof(ExportSection), false)
            {
                Path = BindPath(nameof(Path), "%StreamingAssets%");
                Languages = BindStringArray(nameof(Languages), DefaultLanguages);
                Text = BindBoolean(nameof(Text), false);
                TextFileFormat = BindString(nameof(TextFileFormat), TextResourceFormat.Strings.GetFileExtension());
                Graphics = BindBoolean(nameof(Graphics), false);
                Audio = BindBoolean(nameof(Audio), false);
                Field = BindBoolean(nameof(Field), false);
                Battle = BindBoolean(nameof(Battle), false);
                Translation = BindBoolean(nameof(Translation), false);
            }
        }
    }
}
