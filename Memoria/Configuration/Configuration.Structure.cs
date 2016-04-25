using System;
using System.Collections.Generic;

namespace Memoria
{
    public sealed partial class Configuration : Ini
    {
        private readonly FontSection _font = new FontSection();
        private readonly ImportSection _import = new ImportSection();
        private readonly ExportSection _export = new ExportSection();

        internal override IEnumerable<IniSection> GetSections()
        {
            yield return _font;
            yield return _import;
            yield return _export;
        }

        private sealed class FontSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniArray<String> Names = IniValue.StringArray(nameof(Names));
            public readonly IniValue<Int32> Size = IniValue.Int32(nameof(Size));

            public FontSection() : base("Font")
            {
                Enabled.Value = false;
                Names.Value = new[] {"Arial", "Times Bold"};
            }

            internal override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return Names;
                yield return Size;
            }
        }

        private sealed class ImportSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<String> Path = IniValue.Path(nameof(Path));
            public readonly IniValue<Boolean> Text = IniValue.Boolean(nameof(Text));

            public ImportSection() : base("Import")
            {
                Enabled.Value = false;
                Path.Value = "StreamingAssets";
                Text.Value = true;
            }

            internal override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return Path;
                yield return Text;
            }
        }

        private sealed class ExportSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<String> Path = IniValue.Path(nameof(Path));
            public readonly IniValue<Boolean> Text = IniValue.Boolean(nameof(Text));

            public ExportSection() : base("Export")
            {
                Enabled.Value = false;
                Path.Value = "StreamingAssets";
                Text.Value = true;
            }

            internal override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return Path;
                yield return Text;
            }
        }
    }
}