using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Icons
        {
            public static Boolean HideCursos => Instance._icons.HideCursor;
            public static Boolean HideExclamation => Instance._icons.HideExclamation;
            public static Boolean HideQuestion => Instance._icons.HideQuestion;
            public static Boolean HideCards => Instance._icons.HideCards;
            public static Boolean HideBeach => Instance._icons.HideBeach;
            public static Boolean HideSteam => Instance._icons.HideSteam;
        }
    }
}