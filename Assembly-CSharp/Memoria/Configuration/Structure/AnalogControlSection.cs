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
            public readonly IniValue<Boolean> UseAbsoluteOrientation;

            public AnalogControlSection() : base(nameof(AnalogControlSection), true)
            {
                StickThreshold = BindInt32(nameof(StickThreshold), 10);
                MinimumSpeed = BindInt32(nameof(MinimumSpeed), 5);
                UseAbsoluteOrientation = BindBoolean(nameof(UseAbsoluteOrientation), false);
            }
        }
    }
}