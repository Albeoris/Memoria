using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Export
        {
            public static Boolean Enabled => Instance._export.Enabled;

            public static String Path => Instance._export.Path;
            public static String[] Languages => Instance._export.Languages;
            public static Boolean Text => Instance._export.Text;
            public static Boolean Graphics => Instance._export.Graphics;
            public static Boolean Audio => Instance._export.Audio;
            public static Boolean Field => Instance._export.Field;
            public static Boolean Battle => Instance._export.Battle;

            public static Int32 TextFormat = 1;
        }
    }
}