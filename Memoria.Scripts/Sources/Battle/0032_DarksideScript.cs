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
            if (_v.Caster.IsPlayer)
                _v.WeaponPhisicalParams();
            else
                _v.NormalPhisicalParams();
            _v.CasterCommand.BonusElement();
            if (!_v.CanAttackElementalCommand())
                return;

            _v.CalcHpDamage();

            _v.Caster.Flags |= CalcFlag.HpAlteration;
            _v.Caster.HpDamage = (Int32)(_v.Caster.MaximumHp >> 3);
        }
    }
}