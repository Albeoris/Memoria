using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Import
        {
            public static Boolean Enabled => Instance._import.Enabled;
            public static String Path => Instance._import.Path;
            public static Boolean Text => Enabled && Instance._import.Text;
            public static Boolean Graphics => Enabled && Instance._import.Graphics;
            public static Boolean Audio => Enabled && Instance._import.Audio;
            public static Boolean Field => Enabled && Instance._import.Field;
        }
    }
}
