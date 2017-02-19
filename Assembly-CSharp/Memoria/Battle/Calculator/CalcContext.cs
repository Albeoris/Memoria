using System;

namespace Memoria
{
    public sealed class CalcContext
    {
        public Int16 StatusRate;
        public Int16 AttackPower;
        public Int16 DefensePower;
        public Int16 Evade;
        public Int16 Attack;
        public Int16 HitRate;
        public BattleCalcFlags Flags;

        public Int32 PowerDifference => AttackPower - DefensePower;
        public Boolean IsAbsorb => (Flags & BattleCalcFlags.Absorb) != 0;

        public Int16 EnsureAttack => Attack < 1 ? (Attack = 1) : Attack;
        public Int32 EnsurePowerDifference => PowerDifference < 1 ? 1 : PowerDifference;
    }
}