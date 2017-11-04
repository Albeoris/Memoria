using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class BattleSection : IniSection
        {
            public readonly IniValue<Int32> Speed;
            public readonly IniValue<Boolean> NoAutoTrance;
            public readonly IniValue<Int32> EncounterInterval;

            public BattleSection() : base(nameof(BattleSection), false)
            {
                Speed = BindInt32(nameof(Speed), 0);
                NoAutoTrance = BindBoolean(nameof(NoAutoTrance), false);
                EncounterInterval = BindInt32(nameof(EncounterInterval), 960);
            }
        }
    }
}