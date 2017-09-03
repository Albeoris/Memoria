using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Import
        {
            public static Boolean Enabled => Instance._import.Enabled.Value;
            public static String Path => Instance._import.Path.Value;
            public static Boolean Text => Instance._import.Text.Value;
            public static Boolean Graphics => Instance._import.Graphics.Value;
            public static Boolean Audio => Enabled && Instance._import.Audio.Value;
            public static Boolean Field => Enabled && Instance._import.Field.Value;
        }
    }
}