using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Font
        {
            public static Boolean Enabled => Instance._font.Enabled;
            public static String[] Names => Instance._font.Names;
            public static Int32 Size => Instance._font.Size;
        }
    }
}
