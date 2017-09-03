using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Hacks
        {
            public static Boolean Enabled => Instance._hacks.Enabled.Value;
            public static Int32 AllCharactersAvailable => Enabled ? Instance._hacks.AllCharactersAvailable.Value : 0;
            public static Int32 RopeJumpingIncrement => Enabled ? Instance._hacks.RopeJumpingIncrement.Value : 1;
            public static Int32 FrogCatchingIncrement => Enabled ? Instance._hacks.FrogCatchingIncrement.Value : 1;
            public static Int32 HippaulRacingViviSpeed => Enabled ? Instance._hacks.HippaulRacingViviSpeed.Value : 33;
        }
    }
}