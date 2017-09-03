using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class HacksSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Int32> BattleSpeed = IniValue.Int32(nameof(BattleSpeed));
            public readonly IniValue<Int32> AllCharactersAvailable = IniValue.Int32(nameof(AllCharactersAvailable));
            public readonly IniValue<Int32> RopeJumpingIncrement = IniValue.Int32(nameof(RopeJumpingIncrement));
            public readonly IniValue<Int32> FrogCatchingIncrement = IniValue.Int32(nameof(FrogCatchingIncrement));
            public readonly IniValue<Int32> HippaulRacingViviSpeed = IniValue.Int32(nameof(HippaulRacingViviSpeed));

            public HacksSection() : base("Hacks")
            {
                Enabled.Value = false;
                BattleSpeed.Value = 0;
                AllCharactersAvailable.Value = 0;
                RopeJumpingIncrement.Value = 1;
                FrogCatchingIncrement.Value = 1;
                HippaulRacingViviSpeed.Value = 33;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return BattleSpeed;
                yield return AllCharactersAvailable;
                yield return RopeJumpingIncrement;
                yield return FrogCatchingIncrement;
                yield return HippaulRacingViviSpeed;
            }
        }
    }
}