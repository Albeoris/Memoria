using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class FixesSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Boolean> KeepRestTimeInBattle = IniValue.Boolean(nameof(KeepRestTimeInBattle));

            public FixesSection() : base("Fixes")
            {
                Enabled.Value = false;
                KeepRestTimeInBattle.Value = true;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return KeepRestTimeInBattle;
            }
        }
    }
}