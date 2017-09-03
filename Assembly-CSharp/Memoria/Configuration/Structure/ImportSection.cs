using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class ImportSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<String> Path = IniValue.Path(nameof(Path));
            public readonly IniValue<Boolean> Text = IniValue.Boolean(nameof(Text));
            public readonly IniValue<Boolean> Graphics = IniValue.Boolean(nameof(Graphics));
            public readonly IniValue<Boolean> Field = IniValue.Boolean(nameof(Field));
            public readonly IniValue<Boolean> Audio = IniValue.Boolean(nameof(Audio));

            public ImportSection() : base("Import")
            {
                Enabled.Value = false;
                Path.Value = "%StreamingAssets%";
                Text.Value = true;
                Graphics.Value = true;
                Audio.Value = true;
                Field.Value = false;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return Path;
                yield return Text;
                yield return Graphics;
                yield return Audio;
                yield return Field;
            }
        }
    }
}