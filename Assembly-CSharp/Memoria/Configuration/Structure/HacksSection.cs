using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class HacksSection : IniSection
        {
            public readonly IniValue<Int32> BattleSpeed;
            public readonly IniValue<Int32> AllCharactersAvailable;
            public readonly IniValue<Int32> RopeJumpingIncrement;
            public readonly IniValue<Int32> FrogCatchingIncrement;
            public readonly IniValue<Int32> HippaulRacingViviSpeed;

            public HacksSection() : base(nameof(HacksSection), false)
            {
                BattleSpeed = BindInt32(nameof(BattleSpeed), 0);
                AllCharactersAvailable = BindInt32(nameof(AllCharactersAvailable), 0);
                RopeJumpingIncrement = BindInt32(nameof(RopeJumpingIncrement), 1);
                FrogCatchingIncrement = BindInt32(nameof(FrogCatchingIncrement), 1);
                HippaulRacingViviSpeed = BindInt32(nameof(HippaulRacingViviSpeed), 33);
            }
        }
    }
}