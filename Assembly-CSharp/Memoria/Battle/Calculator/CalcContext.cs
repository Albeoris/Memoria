using Memoria.Data;
using System;
using System.Collections.Generic;

namespace Memoria
{
    public sealed class CalcContext
    {
        public CalcContext(BattleCalculator calc)
        {
            _calculator = calc;
        }

        private readonly BattleCalculator _calculator;

        public Int32 StatusRate;
        public Int32 AttackPower;
        public Int32 DefensePower;
        public Int32 Evade;
        public Int32 Attack;
        public Int32 HitRate;
        public BattleCalcFlags Flags;
        public BattleStatus AddedStatuses = 0; // A list of statuses added to the target (through "BattleTarget.TryAlterStatuses")
        public Int16 TranceIncrease = 0; // The increase/decrease of the trance gauge if required (damaged by an enemy); it is set at the start of CalcMain and can be modified before it applies
        public RegularItem ItemSteal = RegularItem.NoItem; // Just a variable to remember which item was stolen
        public HashSet<SupportAbility> DisabledSA = new HashSet<SupportAbility>(); // The support abilities that are disabled by successfully applied SA features
        public Boolean IsDrain = false; // Keep the Caster.Hp/MpDamage at the same value of Target.Hp/MpDamage even when modified by external effects
        public EatResult EatResult = 0;

        // Current sfxthread at the moment of the effect point (from UnifiedBattleSequencer, in SFXRework mode only)
        // This field may be removed and replaced by another system in the future
        public BattleActionThread sfxThread = null; // Warning: can be null

        public Int32 PowerDifference => AttackPower - DefensePower;
        public Boolean IsAbsorb => (Flags & BattleCalcFlags.Absorb) != 0;
        public Int32 EnsureAttack => Attack < 1 ? (Attack = 1) : Attack;
        public Int32 EnsurePowerDifference => PowerDifference < 1 ? 1 : PowerDifference;

        // By default, damage bonus/malus is done by multiplying Attack by 1.5 or 0.5 (it stacks multiplicatively)
        // That behaviour can be changed using IOverloadDamageModifierScript
        private Int32 _damageModifierCount = 0;
        public Int32 DamageModifierCount
        {
            get => _damageModifierCount;
            set
            {
                if (value == _damageModifierCount)
                    return;
                Int32 previousCount = _damageModifierCount;
                Int32 delta = value - previousCount;
                _damageModifierCount = value;
                if (BattleCalculator.DamageModifierScript != null)
                {
                    BattleCalculator.DamageModifierScript.OnDamageModifierChange(_calculator, previousCount, delta);
                }
                else
                {
                    // Default method
                    if (delta >= 0)
                    {
                        for (Int32 i = 0; i < delta; i++)
                            Attack = Attack * 3 >> 1;
                    }
                    else
                    {
                        delta = -delta;
                        for (Int32 i = 0; i < delta; i++)
                            Attack >>= 1;
                    }
                }
            }
        }

        public void DecreaseAttackDrastically()
        {
            if (BattleCalculator.DamageModifierScript != null)
            {
                BattleCalculator.DamageModifierScript.OnDamageDrasticReduction(_calculator);
            }
            else
            {
                // Default method
                Attack = 1;
            }
        }
    }
}
