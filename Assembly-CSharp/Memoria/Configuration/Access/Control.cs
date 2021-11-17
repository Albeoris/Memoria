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
            public static Int32 UseAbsoluteOrientation => Instance._analogControl.UseAbsoluteOrientation.Value;
            public static Boolean UseAbsoluteOrientationStick => UseAbsoluteOrientation == 1 || UseAbsoluteOrientation == 3;
            public static Boolean UseAbsoluteOrientationKeys => UseAbsoluteOrientation == 1 || UseAbsoluteOrientation == 2;
        }
    }
}