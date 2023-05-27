using System;
using System.Collections.Generic;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class TranceMonster
        {
            public static Boolean Enabled => Instance._trancemonster.Enabled;
            public static Boolean ActiveTranceAllMobs => Instance._trancemonster.ActiveTranceAllMobs;
            public static String TranceValueInit => Instance._trancemonster.TranceValueInit;
            public static String TranceIncreaseMonster => Instance._trancemonster.TranceIncreaseMonster;
            public static String DeltaTranceMonster => Instance._trancemonster.DeltaTranceMonster;
            public static String BonusDamageTranceMonster => Instance._trancemonster.BonusDamageTranceMonster;
            public static String ColorTranceMonster => Instance._trancemonster.ColorTranceMonster;
            public static String ColorTextTranceCMD => Instance._trancemonster.ColorTextTranceCMD;
            public static Boolean OverTrance => Instance._trancemonster.OverTrance;
            public static Int32 OverTranceChance => Instance._trancemonster.OverTranceChance;
            public static String BonusOverTranceEXP => Instance._trancemonster.BonusOverTranceEXP;
            public static Int32 BonusOverTranceAP => Instance._trancemonster.BonusOverTranceAP;
            public static String BonusOverTranceGils => Instance._trancemonster.BonusOverTranceGils;
            public static String TranceMobsExclusion => Instance._trancemonster.TranceMobsExclusion;
        }
    }
}