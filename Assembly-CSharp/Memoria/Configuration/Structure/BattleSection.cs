using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class BattleSection : IniSection
        {
            public readonly IniValue<Boolean> SFXRework;
            public readonly IniValue<Int32> Speed;
            public readonly IniValue<Boolean> NoAutoTrance;
            public readonly IniValue<Int32> EncounterInterval;
            public readonly IniValue<Int32> EncounterInitial;
            public readonly IniValue<Boolean> PersistentDangerValue;
            public readonly IniValue<Boolean> PSXEncounterMethod;
            public readonly IniValue<Int32> AutoPotionOverhealLimit;
            public readonly IniValue<Boolean> GarnetConcentrate;
            public readonly IniValue<Boolean> SelectBestTarget;
            public readonly IniValue<Boolean> BreakDamageLimit;
            public readonly IniValue<Boolean> ViviAutoAttack;
            public readonly IniValue<Boolean> CountersBetterTarget;
            public readonly IniValue<Int32> LockEquippedAbilities;
            public readonly IniValue<Int32> SummonPriorityCount;
            public readonly IniValue<Boolean> CurseUseWeaponElement;
            public readonly IniValue<Int32> FloatEvadeBonus;
            public readonly IniValue<Int32> AccessMenus;
            public readonly IniArray<String> AvailableMenus;
            /* CustomBattleFlagsMeaning:
             0 - default
             1 - Alternate Fantasy:
              [AA_DATA.Type & 0x8]: "Contact attacks", take front/back row into account, back attacks, Ipsen's curse and miss on vanish if physical
              [AA_DATA.Type & 0x10]: "Use weapon's properties", apply Killer abilities, weapon's elemental properties and weapon's status if "Add Status" is on
              [AA_DATA.Type & 0x20]: "Can critical", uses "BattleCalculator.TryCritical"
              [ENEMY_INFO.flags & 0x4]: "Has 10 000 HP trigger", consider that the enemy has 10000 HP less than what its data says (for Scan, HP-based heal or damage...)
            */
            public readonly IniValue<Int32> CustomBattleFlagsMeaning;
            /* SpareChangeGilSpentFormula (see "btl_cmd.SpendSpareChangeGil"):
             default: SpareChangeGilSpentFormula = Power * CasterLevel
             Alternate Fantasy: SpareChangeGilSpentFormula = Power * Gil / 100
            */
            public readonly IniValue<String> SpareChangeGilSpentFormula;
            /* Status durations and ticks formulas (see "btl_stat"):
             default: StatusDurationFormula = ContiCnt * (IsNegativeStatus ? 60 - 8 * TargetSpirit : 8 * TargetSpirit)
                      StatusTickFormula = OprCnt * (IsNegativeStatus ? 60 - 4 * TargetSpirit : 4 * TargetSpirit)
             Alternate Fantasy: StatusDurationFormula = ContiCnt * (StatusIndex == 31 || StatusIndex == 27 ? 240 + 4 * TargetSpirit : 200)
                                StatusTickFormula = OprCnt * 150
            */
            public readonly IniValue<String> StatusDurationFormula;
            public readonly IniValue<String> StatusTickFormula;
            public readonly IniValue<String> PoisonHPDamage;
            public readonly IniValue<String> PoisonMPDamage;
            public readonly IniValue<String> RegenHPRecovery;
            public readonly IniValue<String> SpeedStatFormula;
            public readonly IniValue<String> StrengthStatFormula;
            public readonly IniValue<String> MagicStatFormula;
            public readonly IniValue<String> SpiritStatFormula;
            public readonly IniValue<String> MagicStoneStockFormula;
            public readonly IniValue<String> TranceDecreaseFormula;
            public readonly IniValue<String> RebirthFlame;

            public BattleSection() : base(nameof(BattleSection), false)
            {
                SFXRework = BindBoolean(nameof(SFXRework), true);
                Speed = BindInt32(nameof(Speed), 0);
                NoAutoTrance = BindBoolean(nameof(NoAutoTrance), false);
                EncounterInterval = BindInt32(nameof(EncounterInterval), 960);
                EncounterInitial = BindInt32(nameof(EncounterInitial), -1440);
                PersistentDangerValue = BindBoolean(nameof(PersistentDangerValue), false);
                PSXEncounterMethod = BindBoolean(nameof(PSXEncounterMethod), false);
                AutoPotionOverhealLimit = BindInt32(nameof(AutoPotionOverhealLimit), -1);
                GarnetConcentrate = BindBoolean(nameof(GarnetConcentrate), false);
                SelectBestTarget = BindBoolean(nameof(SelectBestTarget), true);
                BreakDamageLimit = BindBoolean(nameof(BreakDamageLimit), false);
                ViviAutoAttack = BindBoolean(nameof(ViviAutoAttack), false);
                CountersBetterTarget = BindBoolean(nameof(CountersBetterTarget), true);
                LockEquippedAbilities = BindInt32(nameof(LockEquippedAbilities), 0);
                SummonPriorityCount = BindInt32(nameof(SummonPriorityCount), 0);
                CurseUseWeaponElement = BindBoolean(nameof(CurseUseWeaponElement), false);
                FloatEvadeBonus = BindInt32(nameof(FloatEvadeBonus), 0);
                AccessMenus = BindInt32(nameof(AccessMenus), 0);
                AvailableMenus = BindStringArray(nameof(AvailableMenus), new String[] { "Equip", "SupportingAbility" });
                CustomBattleFlagsMeaning = BindInt32(nameof(CustomBattleFlagsMeaning), 0);
                SpareChangeGilSpentFormula = BindString(nameof(SpareChangeGilSpentFormula), "");
                StatusDurationFormula = BindString(nameof(StatusDurationFormula), "");
                StatusTickFormula = BindString(nameof(StatusTickFormula), "");
                PoisonHPDamage = BindString(nameof(PoisonHPDamage), "");
                PoisonMPDamage = BindString(nameof(PoisonMPDamage), "");
                RegenHPRecovery = BindString(nameof(RegenHPRecovery), "");
                SpeedStatFormula = BindString(nameof(SpeedStatFormula), "");
                StrengthStatFormula = BindString(nameof(StrengthStatFormula), "");
                MagicStatFormula = BindString(nameof(MagicStatFormula), "");
                SpiritStatFormula = BindString(nameof(SpiritStatFormula), "");
                MagicStoneStockFormula = BindString(nameof(MagicStoneStockFormula), "");
                TranceDecreaseFormula = BindString(nameof(TranceDecreaseFormula), "");
                RebirthFlame = BindString(nameof(RebirthFlame), "");
            }
        }
    }
}
