using Memoria.Prime.Ini;
using System;
using UnityEngine;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class AnalogControlSection : IniSection
        {
            public readonly IniValue<Int32> StickThreshold;
            public readonly IniValue<Int32> MinimumSpeed;
            public readonly IniValue<Int32> UseAbsoluteOrientation;
            public readonly IniValue<Boolean> RightStickCamera;
            public readonly IniValue<Boolean> InvertedCameraY;
            public readonly IniValue<Boolean> InvertedFlightY;

            public AnalogControlSection() : base(nameof(AnalogControlSection), true)
            {
                StickThreshold = BindInt32(nameof(StickThreshold), 10);
                MinimumSpeed = BindInt32(nameof(MinimumSpeed), 5);
                UseAbsoluteOrientation = BindInt32(nameof(UseAbsoluteOrientation), 3);
                RightStickCamera = BindBoolean(nameof(RightStickCamera), true);
                InvertedCameraY = BindBoolean(nameof(InvertedCameraY), false);
                InvertedFlightY = BindBoolean(nameof(InvertedFlightY), true);
            }
        }

        private sealed class ControlSection : IniSection
        {
            public readonly IniArray<String> KeyBindings;
            public readonly IniValue<Int32> DisableMouse;
            public readonly IniArray<String> DialogProgressButtons;
            public readonly IniValue<Boolean> WrapSomeMenus;
            public readonly IniValue<Boolean> BattleAutoConfirm;
            public readonly IniValue<Boolean> TurboDialog;
            public readonly IniValue<Boolean> SoftReset;
            public readonly IniValue<Boolean> PSXScrollingMethod;
            public readonly IniValue<Boolean> PSXMovementMethod;
            public readonly IniValue<Boolean> AlwaysCaptureGamepad;
            public readonly IniValue<Boolean> SwapConfirmCancel;
            public readonly IniValue<Int32> KeyCodeLeftStick;
            public readonly IniValue<Int32> KeyCodeRightStick;

            public ControlSection() : base(nameof(ControlSection), true)
            {
                KeyBindings = BindStringArray(nameof(KeyBindings), ["W", "A", "S" ,"D", "Backspace", "Alpha1"]);
                DisableMouse = BindInt32(nameof(DisableMouse), 0);
                DialogProgressButtons = BindStringArray(nameof(DialogProgressButtons), ["Confirm"]);
                WrapSomeMenus = BindBoolean(nameof(WrapSomeMenus), true);
                BattleAutoConfirm = BindBoolean(nameof(BattleAutoConfirm), true);
                TurboDialog = BindBoolean(nameof(TurboDialog), false);
                SoftReset = BindBoolean(nameof(SoftReset), false);
                PSXScrollingMethod = BindBoolean(nameof(PSXScrollingMethod), true);
                PSXMovementMethod = BindBoolean(nameof(PSXMovementMethod), false);
                AlwaysCaptureGamepad = BindBoolean(nameof(AlwaysCaptureGamepad), true);
                SwapConfirmCancel = BindBoolean(nameof(SwapConfirmCancel), false);
                KeyCodeLeftStick = BindInt32(nameof(KeyCodeLeftStick), 340);
                KeyCodeRightStick = BindInt32(nameof(KeyCodeRightStick), 341);
            }
        }
    }
}
