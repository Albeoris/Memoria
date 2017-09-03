using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Debug
        {
            public static Boolean Enabled => Instance._debug.Enabled.Value;
            public static Boolean SigningEventObjects => Enabled && Instance._debug.SigningEventObjects.Value;
        }
    }
}