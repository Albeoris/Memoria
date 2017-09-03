using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Font
        {
            public static Boolean Enabled => Instance._font.Enabled.Value;
            public static String[] Names => Instance._font.Names.Value;
            public static Int32 Size => Instance._font.Size.Value;
        }
    }
}