using System;
using System.Collections.Generic;

namespace Memoria
{
    public sealed partial class Configuration : Ini
    {
        private readonly FontSection _font = new FontSection();
        private readonly CheatsSection _cheats = new CheatsSection();
        private readonly ImportSection _import = new ImportSection();
        private readonly ExportSection _export = new ExportSection();
        private readonly HacksSection _hacks = new HacksSection();

        internal override IEnumerable<IniSection> GetSections()
        {
            yield return _font;
            yield return _cheats;
            yield return _import;
            yield return _export;
            yield return _hacks;
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

        private sealed class CheatsSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));

            public readonly IniValue<Boolean> Rotation = IniValue.Boolean(nameof(Rotation));
            public readonly IniValue<Boolean> Perspective = IniValue.Boolean(nameof(Perspective));

            public readonly IniValue<Boolean> SpeedMode = IniValue.Boolean(nameof(SpeedMode));
            public readonly IniValue<Int32> SpeedFactor = IniValue.Int32(nameof(SpeedFactor));

            public readonly IniValue<Boolean> BattleAssistance = IniValue.Boolean(nameof(BattleAssistance));
            public readonly IniValue<Boolean> Attack9999 = IniValue.Boolean(nameof(Attack9999));
            public readonly IniValue<Boolean> NoRandomEncounter = IniValue.Boolean(nameof(NoRandomEncounter));
            public readonly IniValue<Boolean> MasterSkill = IniValue.Boolean(nameof(MasterSkill));
            public readonly IniValue<Boolean> LvMax = IniValue.Boolean(nameof(LvMax));
            public readonly IniValue<Boolean> GilMax = IniValue.Boolean(nameof(GilMax));

            public CheatsSection() : base("Cheats")
            {
                Enabled.Value = false;

                Rotation.Value = true;
                Perspective.Value = true;

                SpeedMode.Value = true;
                SpeedFactor.Value = 5;

                BattleAssistance.Value = false;
                Attack9999.Value = false;
                NoRandomEncounter.Value = false;
                MasterSkill.Value = false;
                LvMax.Value = false;
                GilMax.Value = false;
            }

            internal override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return Rotation;
                yield return Perspective;
                yield return SpeedMode;
                yield return SpeedFactor;
                yield return BattleAssistance;
                yield return Attack9999;
                yield return NoRandomEncounter;
                yield return MasterSkill;
                yield return LvMax;
                yield return GilMax;
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
                Path.Value = "%StreamingAssets%";
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
            public readonly IniArray<String> Languages = IniValue.StringArray(nameof(Languages));
            public readonly IniValue<Boolean> Text = IniValue.Boolean(nameof(Text));
            public readonly IniValue<Boolean> Field = IniValue.Boolean(nameof(Field));

            public ExportSection() : base("Export")
            {
                Enabled.Value = false;
                Path.Value = "%StreamingAssets%";
                Languages.Value = new[] {"US", "UK", "JP", "ES", "FR", "GR", "IT"};
                Text.Value = true;
            }

            internal override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return Path;
                yield return Languages;
                yield return Text;
                yield return Field;
            }
        }

        private sealed class HacksSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Boolean> AllCharactersAvailable = IniValue.Boolean(nameof(AllCharactersAvailable));

            public HacksSection() : base("Hacks")
            {
                Enabled.Value = false;
                AllCharactersAvailable.Value = false;
            }

            internal override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return AllCharactersAvailable;
            }
        }
    }
}