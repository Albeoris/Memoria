using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Fixes
        {
            public static Boolean IsKeepRestTimeInBattle => Instance._fixes.KeepRestTimeInBattle;
        }
    }
}
