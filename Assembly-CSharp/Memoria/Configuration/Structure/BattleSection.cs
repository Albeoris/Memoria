using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class BattleSection : IniSection
        {
            public readonly IniValue<Boolean> Enabled = IniValue.Boolean(nameof(Enabled));
            public readonly IniValue<Int32> Speed = IniValue.Int32(nameof(Speed));
            public readonly IniValue<Boolean> NoAutoTrance = IniValue.Boolean(nameof(NoAutoTrance));

            public BattleSection() : base("Battle")
            {
                Enabled.Value = false;
                Speed.Value = 0;
                NoAutoTrance.Value = false;
            }

            protected override IEnumerable<IniValue> GetValues()
            {
                yield return Enabled;
                yield return Speed;
                yield return NoAutoTrance;
            }
        }
    }
}