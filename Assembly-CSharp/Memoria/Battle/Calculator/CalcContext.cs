using System;
using System.Collections.Generic;
using Memoria.Data;

namespace Memoria
{
    public sealed class CalcContext
    {
        public Int32 StatusRate;
        public Int32 AttackPower;
        public Int32 DefensePower;
        public Int32 Evade;
        public Int32 Attack;
        public Int32 HitRate;
        public BattleCalcFlags Flags;
        public SByte DamageModifierCount = 0; // By default, damage bonus/malus is done by multiplying Attack by 1.5 or 0.5 (it stacks multiplicatively). Using DamageModifierCount, bonus/malus are computed at the end of the effect and stacks much less (additively, with decreasing bonus if there are many)
        public BattleStatus AddedStatuses = 0; // A list of statuses added to the target (through "BattleTarget.TryAlterStatuses")
        public Int16 TranceIncrease = 0; // The increase/decrease of the trance gauge if required (damaged by an enemy); it is set at the start of CalcMain and can be modified before it applies
        public RegularItem ItemSteal = RegularItem.NoItem; // Just a variable to remember which item was stolen
        public HashSet<SupportAbility> DisabledSA = new HashSet<SupportAbility>(); // The support abilities that are disabled by successfully applied SA features
        public Boolean IsDrain = false; // Keep the Caster.Hp/MpDamage at the same value of Target.Hp/MpDamage even when modified by external effects
        public BattleActionThread sfxThread; // Current sfxthreadn, based from the effect point (from UnifiedBattleSequencer)

        public Int32 PowerDifference => AttackPower - DefensePower;
        public Boolean IsAbsorb => (Flags & BattleCalcFlags.Absorb) != 0;

        public Int32 EnsureAttack => Attack < 1 ? (Attack = 1) : Attack;
        public Int32 EnsurePowerDifference => PowerDifference < 1 ? 1 : PowerDifference;
    }
}