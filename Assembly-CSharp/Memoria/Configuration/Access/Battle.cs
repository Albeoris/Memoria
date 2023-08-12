using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Battle
        {
            public static Boolean SFXRework => Instance._battle.SFXRework || Speed > 0;
            public static Int32 Speed => GetBattleSpeed();
            public static Boolean NoAutoTrance => Instance._battle.NoAutoTrance;
            public static Int32 EncounterInterval => Instance._battle.EncounterInterval;
            public static Int32 EncounterInitial => Instance._battle.EncounterInitial;
            public static Boolean PersistentDangerValue => Instance._battle.PersistentDangerValue;
            public static Boolean PSXEncounterMethod => Instance._battle.PSXEncounterMethod;

            /// <summary>
            /// Maximum over heal in percents.
            /// </summary>
            public static Int32 AutoPotionOverhealLimit => Instance._battle.AutoPotionOverhealLimit;

            public static Boolean GarnetConcentrate => Instance._battle.GarnetConcentrate;
            public static Boolean SelectBestTarget => Instance._battle.SelectBestTarget;
            public static Boolean BreakDamageLimit => Instance._battle.BreakDamageLimit;

            public static Boolean ViviAutoAttack => Instance._battle.ViviAutoAttack;
            public static Boolean CountersBetterTarget => Instance._battle.CountersBetterTarget;
            public static Int32 LockEquippedAbilities => Instance._battle.LockEquippedAbilities;
            public static Int32 SummonPriorityCount => Instance._battle.SummonPriorityCount;
            public static Boolean CurseUseWeaponElement => Instance._battle.CurseUseWeaponElement;
            public static Int32 FloatEvadeBonus => Instance._battle.FloatEvadeBonus;
            public static Int32 AccessMenus => Instance._battle.AccessMenus;
            public static String[] AvailableMenus => Instance._battle.AvailableMenus;
            public static Int32 CustomBattleFlagsMeaning => Instance._battle.CustomBattleFlagsMeaning;
            public static String SpareChangeGilSpentFormula => Instance._battle.SpareChangeGilSpentFormula;
            public static String StatusDurationFormula => Instance._battle.StatusDurationFormula;
            public static String StatusTickFormula => Instance._battle.StatusTickFormula;
            public static String SpeedStatFormula => Instance._battle.SpeedStatFormula;
            public static String StrengthStatFormula => Instance._battle.StrengthStatFormula;
            public static String MagicStatFormula => Instance._battle.MagicStatFormula;
            public static String SpiritStatFormula => Instance._battle.SpiritStatFormula;
            public static String MagicStoneStockFormula => Instance._battle.MagicStoneStockFormula;
            public static String TranceDecreaseFormula => Instance._battle.TranceDecreaseFormula;

            private static Int32 GetBattleSpeed()
            {
                Int32 value = Math.Max(Instance._battle.Speed, Instance._hacks.BattleSpeed);
                if (value == 0)
                    return 0;

                return value;
            }
        }
    }
}