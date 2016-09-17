using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Absorb Strength
    /// </summary>
    [BattleScript(Id)]
    public sealed class DrainHpScript : IBattleScript
    {
        public const Int32 Id = 0016;

        private readonly BattleCalculator _v;

        public DrainHpScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.IsCasterNotTarget() || !_v.Target.CanBeAttacked())
                return;

            _v.NormalMagicParams();
            _v.Caster.PenaltyMini();
            _v.Target.PenaltyShellAttack();
            _v.PrepareHpDraining();
            if (_v.Context.PowerDifference < 1)
                return;

            Int16 damage = (Int16)Math.Min(9999, _v.Context.PowerDifference * _v.Context.EnsureAttack);
            _v.Target.HpDamage = damage;
            _v.Caster.HpDamage = damage;
        }
    }
}