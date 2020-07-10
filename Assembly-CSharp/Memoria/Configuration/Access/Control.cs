using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class AnalogControl
        {
            public static Boolean Enabled => Instance._analogControl.Enabled.Value;
            public static Int32 StickThreshold => Instance._analogControl.StickThreshold.Value;
            public static Int32 MinimumSpeed => Instance._analogControl.MinimumSpeed.Value;
            public static Boolean UseAbsoluteOrientation => Instance._analogControl.UseAbsoluteOrientation.Value;
        }
    }
}