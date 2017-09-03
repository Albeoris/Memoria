using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Control
        {
            public static Boolean Enabled => Instance._ctrl.Enabled.Value;
            public static Int32 StickThreshold => Instance._ctrl.StickThreshold.Value;
            public static Int32 MinimumSpeed => Instance._ctrl.MinimumSpeed.Value;
        }
    }
}