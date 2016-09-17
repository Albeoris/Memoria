using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Lancer
    /// </summary>
    [BattleScript(Id)]
    public sealed class LancerScript : IBattleScript
    {
        public const Int32 Id = 0039;

        private readonly BattleCalculator _v;

        public LancerScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.WeaponPhisicalParams();
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.Target.Flags |= CalcFlag.HpAlteration | CalcFlag.MpAlteration;
            if (_v.Context.PowerDifference <= 0)
                return;

            Int32 damage = Math.Min(9999, _v.Context.PowerDifference * _v.Context.EnsureAttack);
            _v.Target.HpDamage = (Int16)damage;
            _v.Target.MpDamage = (Int16)(damage >> 4);
        }
    }
}