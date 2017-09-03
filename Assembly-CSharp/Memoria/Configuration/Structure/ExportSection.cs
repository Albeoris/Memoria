using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class ExportSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<String> Path = IniValue.Path(nameof(Path));
            public readonly IniArray<String> Languages = IniValue.StringArray(nameof(Languages));
            public readonly IniValue<Boolean> Text = IniValue.Boolean(nameof(Text));
            public readonly IniValue<Boolean> Graphics = IniValue.Boolean(nameof(Graphics));
            public readonly IniValue<Boolean> Audio = IniValue.Boolean(nameof(Audio));
            public readonly IniValue<Boolean> Field = IniValue.Boolean(nameof(Field));
            public readonly IniValue<Boolean> Battle = IniValue.Boolean(nameof(Battle));

            public ExportSection() : base("Export")
            {
                Enabled.Value = false;
                Path.Value = "%StreamingAssets%";
                Languages.Value = new[] { "US", "UK", "JP", "ES", "FR", "GR", "IT" };
                Text.Value = true;
                Graphics.Value = true;
                Audio.Value = true;
                Field.Value = false;
                Battle.Value = false;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return Path;
                yield return Languages;
                yield return Text;
                yield return Graphics;
                yield return Audio;
                yield return Field;
                yield return Battle;
            }
        }
    }
}