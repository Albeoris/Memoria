using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Battle
        {
            public static Boolean NoAutoTrance => Instance._battle.NoAutoTrance;
            public static Int32 Speed => Math.Max(Instance._battle.Speed, Instance._hacks.BattleSpeed);
            public static Int32 EncounterInterval => Instance._battle.EncounterInterval;
            public static Int32 AutoPotionOverhealLimit => Instance._battle.AutoPotionOverhealLimit;
            public static Boolean GarnetConcentrate => Instance._battle.GarnetConcentrate;
            public static Boolean SelectBestTarget => Instance._battle.SelectBestTarget;
            public static Int32 StealAugment => Instance._battle.StealAugment;
        }
    }
}