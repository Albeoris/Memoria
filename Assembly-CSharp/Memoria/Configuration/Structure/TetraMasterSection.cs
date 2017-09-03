using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
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
    }
}