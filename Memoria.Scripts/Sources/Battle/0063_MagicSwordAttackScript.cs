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
            _v.Caster.SetLowPhysicalAttack();
            _v.SetWeaponPowerSum();
            _v.Target.SetPhysicalDefense();
            _v.BonusElement();
            if (_v.CanAttackMagic())
                _v.CalcHpDamage();
        }
    }
}