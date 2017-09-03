using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class ControlSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Int32> StickThreshold = IniValue.Int32(nameof(StickThreshold));
            public readonly IniValue<Int32> MinimumSpeed = IniValue.Int32(nameof(MinimumSpeed));

            public ControlSection() : base("AnalogControl")
            {
                Enabled.Value = false;
                StickThreshold.Value = 20;
                MinimumSpeed.Value = 5;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return StickThreshold;
                yield return MinimumSpeed;
            }
        }
    }
}