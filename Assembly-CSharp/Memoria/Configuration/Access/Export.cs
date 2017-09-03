using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Export
        {
            public static Boolean Enabled => Instance._export.Enabled.Value;
            public static String Path => Instance._export.Path.Value;
            public static String[] Languages => Instance._export.Languages.Value;
            public static Boolean Text => Instance._export.Text.Value;
            public static Int32 TextFormat = 1;
            public static Boolean Graphics => Instance._export.Graphics.Value;
            public static Boolean Audio => Enabled && Instance._export.Audio.Value;
            public static Boolean Field => Enabled && Instance._export.Field.Value;
            public static Boolean Battle => Instance._export.Battle.Value;
        }
    }
}