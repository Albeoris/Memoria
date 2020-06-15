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
            public static Int32 EncounterInitial => Instance._battle.EncounterInitial;
            public static Int32 AutoPotionOverhealLimit => Instance._battle.AutoPotionOverhealLimit;
            public static Boolean GarnetConcentrate => Instance._battle.GarnetConcentrate;
            public static Boolean SelectBestTarget => Instance._battle.SelectBestTarget;
            public static Boolean ViviAutoAttack => Instance._battle.ViviAutoAttack;
            public static Boolean CountersBetterTarget => Instance._battle.CountersBetterTarget;
            public static Int32 SummonPriorityCount => Instance._battle.SummonPriorityCount;
            public static Int32 SpareChangeGilSpentFormula => Instance._battle.SpareChangeGilSpentFormula;
            public static Boolean CurseUseWeaponElement => Instance._battle.CurseUseWeaponElement;
            public static Int32 FloatEvadeBonus => Instance._battle.FloatEvadeBonus;
            public static Int32 CustomBattleFlagsMeaning => Instance._battle.CustomBattleFlagsMeaning;
        }
    }
}