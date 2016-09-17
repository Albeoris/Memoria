using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Magic Sword
    /// </summary>
    [BattleScript(Id)]
    public sealed class MagicSwordAttackScript : IBattleScript
    {
        public const Int32 Id = 0063;

        private readonly BattleCalculator _v;

        public MagicSwordAttackScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Caster.SetLowPhisicalAttack();
            _v.CasterCommand.SetWeaponPowerSum();
            _v.Target.SetPhisicalDefense();
            _v.CasterCommand.BonusElement();
            if (_v.CanAttackMagic())
                _v.TargetCommand.CalcHpDamage();
        }
    }
}