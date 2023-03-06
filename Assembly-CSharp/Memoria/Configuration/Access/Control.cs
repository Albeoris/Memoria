using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class AnalogControl
        {
            public static Boolean Enabled => Instance._analogControl.Enabled;
            public static Int32 StickThreshold => Instance._analogControl.StickThreshold;
            public static Int32 MinimumSpeed => Instance._analogControl.MinimumSpeed;
            public static Int32 UseAbsoluteOrientation => Instance._analogControl.UseAbsoluteOrientation;
            public static Boolean UseAbsoluteOrientationStick => UseAbsoluteOrientation == 1 || UseAbsoluteOrientation == 3;
            public static Boolean UseAbsoluteOrientationKeys => UseAbsoluteOrientation == 1 || UseAbsoluteOrientation == 2;
        }

        public static class Control
        {
            public static Int32 DisableMouse => Instance._control.DisableMouse;
            public static Boolean DisableMouseForMenus => (DisableMouse & 1) != 0;
            public static Boolean DisableMouseInFields => (DisableMouse & 2) != 0;
            public static Boolean DisableMouseInBattles => (DisableMouse & 4) != 0;
            public static String[] DialogProgressButtons => Instance._control.DialogProgressButtons;
            public static Boolean WrapSomeMenus => Instance._control.WrapSomeMenus;
            public static Boolean PSXScrollingMethod => Instance._control.PSXScrollingMethod;
            /*
            The PSX movement algorithm is different than the remaster's movement algorithm
            In remaster, characters move at a given speed (eg. 60/tick for running with player character) in the horizontal plane, then the height is adjusted to launch on a valid walkmesh triangle
            In PSX, it seems that characters move at that speed in the plane containing the current triangle (and then the height is adjusted just like remaster)
            If that is true, it results in characters moving faster on steep slopes for the remaster version, and in characters moving either a bit slower or a bit faster on irregular walkmeshes ("irregular" = triangles are not aligned on the same plane)
            */
            public static Boolean PSXMovementMethod => Instance._control.PSXMovementMethod;
        }
    }
}