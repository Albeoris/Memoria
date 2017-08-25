using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration : Ini
    {
        private readonly FontSection _font = new FontSection();
        private readonly ControlSection _ctrl = new ControlSection();
        private readonly GraphicsSection _graphics = new GraphicsSection();
        private readonly CheatsSection _cheats = new CheatsSection();
        private readonly ImportSection _import = new ImportSection();
        private readonly ExportSection _export = new ExportSection();
        private readonly TetraMasterSection _tetraMaster = new TetraMasterSection();
        private readonly FixesSection _fixes = new FixesSection();
        private readonly HacksSection _hacks = new HacksSection();
        private readonly DebugSection _debug = new DebugSection();

        public override IEnumerable<IniSection> GetSections()
        {
            yield return _font;
            yield return _ctrl;
            yield return _graphics;
            yield return _cheats;
            yield return _import;
            yield return _export;
            yield return _tetraMaster;
            yield return _fixes;
            yield return _hacks;
            yield return _debug;
        }

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

        private sealed class ControlSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Int32> StickThreshold = IniValue.Int32(nameof(StickThreshold));
            public readonly IniValue<Int32> MinimumSpeed = IniValue.Int32(nameof(MinimumSpeed));

            public ControlSection() : base("AnalogControl")
            {
                Enabled.Value = false;
                StickThreshold.Value = 10;
                MinimumSpeed.Value = 5;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return StickThreshold;
                yield return MinimumSpeed;
            }
        }

        private sealed class GraphicsSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Int32> BattleFPS = IniValue.Int32(nameof(BattleFPS));
            public readonly IniValue<Int32> BattleSwirlFrames = IniValue.Int32(nameof(BattleSwirlFrames));
            public readonly IniValue<Boolean> WidescreenSupport = IniValue.Boolean(nameof(WidescreenSupport));
            public readonly IniValue<Boolean> SkipIntros = IniValue.Boolean(nameof(SkipIntros));

            public GraphicsSection() : base("Graphics")
            {
                Enabled.Value = false;
                BattleFPS.Value = 30;
                BattleSwirlFrames.Value = 25;
                WidescreenSupport.Value = false;
                SkipIntros.Value = false;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return BattleFPS;
                yield return BattleSwirlFrames;
                yield return WidescreenSupport;
                yield return SkipIntros;
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

            protected override IEnumerable<IniValue> GetValues()
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

        private sealed class TetraMasterSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Int32> ReduceRandom = IniValue.Int32(nameof(ReduceRandom));

            public readonly IniValue<Boolean> DiscardAutoButton = IniValue.Boolean(nameof(DiscardAutoButton));
            public readonly IniValue<Boolean> DiscardAssaultCards = IniValue.Boolean(nameof(DiscardAssaultCards));
            public readonly IniValue<Boolean> DiscardFlexibleCards = IniValue.Boolean(nameof(DiscardFlexibleCards));
            public readonly IniValue<Int32> DiscardMaxAttack = IniValue.Int32(nameof(DiscardMaxAttack));
            public readonly IniValue<Int32> DiscardMaxPDef = IniValue.Int32(nameof(DiscardMaxPDef));
            public readonly IniValue<Int32> DiscardMaxMDef = IniValue.Int32(nameof(DiscardMaxMDef));
            public readonly IniValue<Int32> DiscardMaxSum = IniValue.Int32(nameof(DiscardMaxSum));
            public readonly IniValue<Int32> DiscardMinDeckSize = IniValue.Int32(nameof(DiscardMinDeckSize));
            public readonly IniValue<Int32> DiscardKeepSameType = IniValue.Int32(nameof(DiscardKeepSameType));
            public readonly IniValue<Int32> DiscardKeepSameArrow = IniValue.Int32(nameof(DiscardKeepSameArrow));
            public readonly IniSet<Int32> DiscardExclusions = IniValue.Int32Set(nameof(DiscardExclusions));

            public TetraMasterSection() : base("TetraMaster")
            {
                Enabled.Value = true;
                ReduceRandom.Value = 1;

                DiscardAutoButton.Value = true;
                DiscardAssaultCards.Value = false;
                DiscardFlexibleCards.Value = true;
                DiscardMaxAttack.Value = 224;
                DiscardMaxPDef.Value = 255;
                DiscardMaxMDef.Value = 255;
                DiscardMaxSum.Value = 480;
                DiscardMinDeckSize.Value = 10;
                DiscardKeepSameType.Value = 1;
                DiscardKeepSameArrow.Value = 0;
                DiscardExclusions.Value = new HashSet<Int32>(new[] {56, 75, 76, 77, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 98, 99, 100});
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return ReduceRandom;

                yield return DiscardAutoButton;
                yield return DiscardAssaultCards;
                yield return DiscardFlexibleCards;
                yield return DiscardMaxAttack;
                yield return DiscardMaxPDef;
                yield return DiscardMaxMDef;
                yield return DiscardMaxSum;
                yield return DiscardMinDeckSize;
                yield return DiscardKeepSameType;
                yield return DiscardKeepSameArrow;
                yield return DiscardExclusions;
            }
        }

        private sealed class FixesSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Boolean> KeepRestTimeInBattle = IniValue.Boolean(nameof(KeepRestTimeInBattle));

            public FixesSection() : base("Fixes")
            {
                Enabled.Value = false;
                KeepRestTimeInBattle.Value = true;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return KeepRestTimeInBattle;
            }
        }

        private sealed class HacksSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Int32> BattleSpeed = IniValue.Int32(nameof(BattleSpeed));
            public readonly IniValue<Int32> AllCharactersAvailable = IniValue.Int32(nameof(AllCharactersAvailable));
            public readonly IniValue<Int32> RopeJumpingIncrement = IniValue.Int32(nameof(RopeJumpingIncrement));
            public readonly IniValue<Int32> HippaulRacingViviSpeed = IniValue.Int32(nameof(HippaulRacingViviSpeed));

            public HacksSection() : base("Hacks")
            {
                Enabled.Value = false;
                BattleSpeed.Value = 1;
                AllCharactersAvailable.Value = 0;
                RopeJumpingIncrement.Value = 1;
                HippaulRacingViviSpeed.Value = 33;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return BattleSpeed;
                yield return AllCharactersAvailable;
                yield return RopeJumpingIncrement;
                yield return HippaulRacingViviSpeed;
            }
        }

        private sealed class DebugSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Boolean> SigningEventObjects = IniValue.Boolean(nameof(SigningEventObjects));

            public DebugSection() : base("Debug")
            {
                Enabled.Value = false;
                SigningEventObjects.Value = false;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return SigningEventObjects;
            }
        }
    }
}