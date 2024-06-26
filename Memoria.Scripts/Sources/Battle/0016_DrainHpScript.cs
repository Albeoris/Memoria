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
            _v.Caster.EnemyTranceBonusAttack();
            _v.Caster.PenaltyMini();
            _v.Target.PenaltyShellAttack();
            _v.PrepareHpDraining();
            if (_v.Context.PowerDifference < 1)
                return;

            _v.CalcHpDamage();
            _v.Caster.HpDamage = _v.Target.HpDamage;
        }
    }
}