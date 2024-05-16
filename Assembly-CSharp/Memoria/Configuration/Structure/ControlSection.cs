using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class AnalogControlSection : IniSection
        {
            public readonly IniValue<Int32> StickThreshold;
            public readonly IniValue<Int32> MinimumSpeed;
            public readonly IniValue<Int32> UseAbsoluteOrientation;

            public AnalogControlSection() : base(nameof(AnalogControlSection), true)
            {
                StickThreshold = BindInt32(nameof(StickThreshold), 10);
                MinimumSpeed = BindInt32(nameof(MinimumSpeed), 5);
                UseAbsoluteOrientation = BindInt32(nameof(UseAbsoluteOrientation), 0);
            }
        }

        private sealed class ControlSection : IniSection
        {
            public readonly IniValue<Int32> DisableMouse;
            public readonly IniArray<String> DialogProgressButtons;
            public readonly IniValue<Boolean> WrapSomeMenus;
            public readonly IniValue<Boolean> BattleAutoConfirm;
            public readonly IniValue<Boolean> PSXScrollingMethod;
            public readonly IniValue<Boolean> PSXMovementMethod;
            public readonly IniValue<Boolean> AlwaysCaptureGamepad;

            public ControlSection() : base(nameof(ControlSection), true)
            {
                DisableMouse = BindInt32(nameof(DisableMouse), 0);
                DialogProgressButtons = BindStringArray(nameof(DialogProgressButtons), new String[1] { "Confirm" });
                WrapSomeMenus = BindBoolean(nameof(WrapSomeMenus), true);
                BattleAutoConfirm = BindBoolean(nameof(BattleAutoConfirm), true);
                PSXScrollingMethod = BindBoolean(nameof(PSXScrollingMethod), true);
                PSXMovementMethod = BindBoolean(nameof(PSXMovementMethod), false);
                AlwaysCaptureGamepad = BindBoolean(nameof(AlwaysCaptureGamepad), false);
            }
        }
    }
}
