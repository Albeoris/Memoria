using System;
using System.Collections.Generic;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Speedrun
        {
            public static Boolean IsEnabled => Instance._speedrun.Enabled;
            public static String SplitSettingsPath => Instance._speedrun.SplitSettingsPath;
            public static String LogGameTimePath => Instance._speedrun.LogGameTimePath;
        }
    }
}