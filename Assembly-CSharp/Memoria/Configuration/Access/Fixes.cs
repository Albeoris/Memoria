using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Fixes
        {
            public static Boolean Enabled => Instance._fixes.Enabled.Value;

            public static Boolean IsKeepRestTimeInBattle => Enabled && Instance._fixes.KeepRestTimeInBattle.Value;
        }
    }
}