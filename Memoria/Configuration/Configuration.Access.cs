using System;

namespace Memoria
{
    public sealed partial class Configuration : Ini
    {
        public static class Font
        {
            public static Boolean Enabled => Instance._font.Enabled.Value;
            public static String[] Names => Instance._font.Names.Value;
            public static Int32 Size => Instance._font.Size.Value;
        }

        public static class Import
        {
            public static Boolean Enabled => Instance._import.Enabled.Value;
            public static String Path => Instance._import.Path.Value;
            public static Boolean Text => Instance._import.Text.Value;
        }

        public static class Export
        {
            public static Boolean Enabled => Instance._export.Enabled.Value;
            public static String Path => Instance._export.Path.Value;
            public static Boolean Text => Instance._export.Text.Value;
        }
    }
}