using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Hacks
        {
            public static Boolean Enabled => Instance._hacks.Enabled.Value;
            public static Int32 AllCharactersAvailable => Instance._hacks.AllCharactersAvailable;
            public static Int32 RopeJumpingIncrement => Instance._hacks.RopeJumpingIncrement;
            public static Int32 SwordplayAssistance => Instance._hacks.SwordplayAssistance;
            public static Int32 FrogCatchingIncrement => Instance._hacks.FrogCatchingIncrement;
            public static Int32 HippaulRacingViviSpeed => Instance._hacks.HippaulRacingViviSpeed;
            public static Int32 StealingAlwaysWorks => Instance._hacks.StealingAlwaysWorks;
            public static Int32 DisableNameChoice => Instance._hacks.DisableNameChoice;
            public static Boolean ExcaliburIINoTimeLimit => Instance._hacks.ExcaliburIINoTimeLimit;
        }
    }
}
