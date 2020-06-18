using System;
using Memoria.Data;

namespace Memoria
{
    public sealed class CalcContext
    {
        public Int16 StatusRate;
        public Int32 AttackPower;
        public Int32 DefensePower;
        public Int16 Evade;
        public Int32 Attack;
        public Int16 HitRate;
        public BattleCalcFlags Flags;
        public SByte dmg_modifier_count = 0;
        public BattleStatus added_status = 0; // A list of statuses added to the target (through "BattleTarget.TryAlterStatuses")

        public Int32 PowerDifference => AttackPower - DefensePower;
        public Boolean IsAbsorb => (Flags & BattleCalcFlags.Absorb) != 0;

        public Int32 EnsureAttack => Attack < 1 ? (Attack = 1) : Attack;
        public Int32 EnsurePowerDifference => PowerDifference < 1 ? 1 : PowerDifference;
    }
}