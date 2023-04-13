using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Battle
        {
            public static Boolean SFXRework => Instance._battle.SFXRework || Speed >= 3;
            public static Int32 Speed => GetBattleSpeed();
            public static Boolean NoAutoTrance => Configuration.Mod.TranceSeek ? false : Instance._battle.NoAutoTrance;
            public static Int32 EncounterInterval => Configuration.Mod.TranceSeek ? 960 : Instance._battle.EncounterInterval;
            public static Int32 EncounterInitial => Configuration.Mod.TranceSeek ? -1440 : Instance._battle.EncounterInitial;
            public static Boolean PersistentDangerValue => Configuration.Mod.TranceSeek ? false : Instance._battle.PersistentDangerValue;
            public static Boolean PSXEncounterMethod => Configuration.Mod.TranceSeek ? true : Instance._battle.PSXEncounterMethod;

            /// <summary>
            /// Maximum over heal in percents.
            /// </summary>
            public static Int32 AutoPotionOverhealLimit => Configuration.Mod.TranceSeek ? -1 : Instance._battle.AutoPotionOverhealLimit;

            public static Boolean GarnetConcentrate => Instance._battle.GarnetConcentrate;
            public static Boolean SelectBestTarget => Instance._battle.SelectBestTarget;
            public static Boolean BreakDamageLimit => Instance._battle.BreakDamageLimit;

            public static Boolean ViviAutoAttack => Instance._battle.ViviAutoAttack;
            public static Boolean CountersBetterTarget => Configuration.Mod.TranceSeek ? false : Instance._battle.CountersBetterTarget;
            public static Int32 LockEquippedAbilities => Configuration.Mod.TranceSeek ? 0 : Instance._battle.LockEquippedAbilities;
            public static Int32 SummonPriorityCount => Configuration.Mod.TranceSeek ? 0 : Instance._battle.SummonPriorityCount;
            public static Boolean CurseUseWeaponElement => Configuration.Mod.TranceSeek ? false : Instance._battle.CurseUseWeaponElement;
            public static Int32 FloatEvadeBonus => Configuration.Mod.TranceSeek ? 0 : Instance._battle.FloatEvadeBonus;
            public static Int32 AccessMenus => Configuration.Mod.TranceSeek ? 0 : Instance._battle.AccessMenus;
            public static String[] AvailableMenus => Instance._battle.AvailableMenus;
            public static Int32 CustomBattleFlagsMeaning => Configuration.Mod.TranceSeek ? 0 : Instance._battle.CustomBattleFlagsMeaning;
            public static String SpareChangeGilSpentFormula => Configuration.Mod.TranceSeek ? "" : Instance._battle.SpareChangeGilSpentFormula;
            public static String StatusDurationFormula => Configuration.Mod.TranceSeek ? "" : Instance._battle.StatusDurationFormula;
            public static String StatusTickFormula => Configuration.Mod.TranceSeek ? "" : Instance._battle.StatusTickFormula;
            public static String SpeedStatFormula => Configuration.Mod.TranceSeek ? "" : Instance._battle.SpeedStatFormula;
            public static String StrengthStatFormula => Configuration.Mod.TranceSeek ? "" : Instance._battle.StrengthStatFormula;
            public static String MagicStatFormula => Configuration.Mod.TranceSeek ? "" : Instance._battle.MagicStatFormula;
            public static String SpiritStatFormula => Configuration.Mod.TranceSeek ? "" : Instance._battle.SpiritStatFormula;
            public static String MagicStoneStockFormula => Configuration.Mod.TranceSeek ? "" : Instance._battle.MagicStoneStockFormula;

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