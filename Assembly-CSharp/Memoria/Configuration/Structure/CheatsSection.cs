using Memoria.Prime.Ini;
using System;
using System.Collections.Generic;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class CheatsSection : IniSection
        {
            public readonly IniValue<Boolean> SpeedMode;
            public readonly IniValue<Int32> SpeedFactor;
            public readonly IniValue<Boolean> SpeedTimer;

            public readonly IniValue<Boolean> BattleAssistance;
            public readonly IniValue<Boolean> Attack9999;
            public readonly IniValue<Boolean> NoRandomEncounter;
            public readonly IniValue<Boolean> MasterSkill;
            public readonly IniValue<Boolean> LvMax;
            public readonly IniValue<Boolean> GilMax;

            public CheatsSection() : base(nameof(CheatsSection), false)
            {
                SpeedMode = BindBoolean(nameof(SpeedMode), false);
                SpeedFactor = BindInt32(nameof(SpeedFactor), 3);
                SpeedTimer = BindBoolean(nameof(SpeedTimer), false);

                BattleAssistance = BindBoolean(nameof(BattleAssistance), false);
                Attack9999 = BindBoolean(nameof(Attack9999), false);
                NoRandomEncounter = BindBoolean(nameof(NoRandomEncounter), false);
                MasterSkill = BindBoolean(nameof(MasterSkill), false);
                LvMax = BindBoolean(nameof(LvMax), false);
                GilMax = BindBoolean(nameof(GilMax), false);
            }
        }
    }
}
