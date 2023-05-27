using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class TranceMonsterSection : IniSection
        {
            public readonly IniValue<Boolean> ActiveTranceAllMobs;
            public readonly IniValue<String> TranceValueInit;
            public readonly IniValue<String> TranceIncreaseMonster;
            public readonly IniValue<String> DeltaTranceMonster;
            public readonly IniValue<String> BonusDamageTranceMonster;
            public readonly IniValue<String> ColorTranceMonster;
            public readonly IniValue<String> ColorTextTranceCMD;
            public readonly IniValue<Boolean> OverTrance;
            public readonly IniValue<Int32> OverTranceChance;
            public readonly IniValue<String> BonusOverTranceEXP;
            public readonly IniValue<Int32> BonusOverTranceAP;
            public readonly IniValue<String> BonusOverTranceGils;
            public readonly IniValue<String> TranceMobsExclusion;

            public TranceMonsterSection() : base(nameof(TranceMonsterSection), false)
            {
                ActiveTranceAllMobs = BindBoolean(nameof(ActiveTranceAllMobs), true);
                TranceValueInit = BindString(nameof(TranceValueInit), "");
                TranceIncreaseMonster = BindString(nameof(TranceIncreaseMonster), "");
                DeltaTranceMonster = BindString(nameof(DeltaTranceMonster), "");
                BonusDamageTranceMonster = BindString(nameof(BonusDamageTranceMonster), "");
                ColorTranceMonster = BindString(nameof(ColorTranceMonster), "");
                ColorTextTranceCMD = BindString(nameof(ColorTextTranceCMD), "");
                OverTrance = BindBoolean(nameof(OverTrance), false);
                OverTranceChance = BindInt32(nameof(OverTranceChance), 1024);
                BonusOverTranceEXP = BindString(nameof(BonusOverTranceEXP), "");
                BonusOverTranceAP = BindInt32(nameof(BonusOverTranceAP), 1);
                BonusOverTranceGils = BindString(nameof(BonusOverTranceGils), "");
                TranceMobsExclusion = BindString(nameof(TranceMobsExclusion), "5 566 567");
            }
        }
    }
}