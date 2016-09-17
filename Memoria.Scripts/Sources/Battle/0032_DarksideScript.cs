using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Darkside
    /// </summary>
    [BattleScript(Id)]
    public sealed class DarksideScript : IBattleScript
    {
        public const Int32 Id = 0032;

        private readonly BattleCalculator _v;

        public DarksideScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.WeaponPhisicalParams();
            _v.CasterCommand.BonusElement();
            if (!_v.CanAttackElementalCommand())
                return;

            _v.Target.Flags |= CalcFlag.HpAlteration;

            Int16 damage = (Int16)Math.Min(9999, _v.Context.EnsureAttack * (_v.Context.AttackPower - _v.Context.DefensePower));
            if (damage < 1)
                return;

            _v.Caster.Flags |= CalcFlag.HpAlteration;
            _v.Caster.HpDamage = (Int16)(_v.Caster.MaximumHp >> 3);

            if ((_v.Context.Flags & BattleCalcFlags.Absorb) != 0)
                _v.Target.Flags |= CalcFlag.HpRecovery;

            _v.Target.HpDamage = damage;
        }
    }
}