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

        public static class Control
        {
            public static Int32 DisableMouse => Instance._control.DisableMouse.Value;
            public static Boolean DisableMouseForMenus => (DisableMouse & 1) != 0;
            public static Boolean DisableMouseInFields => (DisableMouse & 2) != 0;
            public static Boolean DisableMouseInBattles => (DisableMouse & 4) != 0;
            public static String[] DialogProgressButtons => Instance._control.DialogProgressButtons.Value;
            public static Boolean WrapSomeMenus => Instance._control.WrapSomeMenus.Value;
            public static Boolean ScrollLikePSX => Instance._control.ScrollLikePSX.Value;
        }
    }
}