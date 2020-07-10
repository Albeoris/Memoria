using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Free Energy, Tidal Flame, Scoop Art, Shift Break, Stellar Circle 5, Meo Twister, Solution 9, Grand Lethal, No Mercy, Stock Break, Shock
    /// </summary>
    [BattleScript(Id)]
    public sealed class PhisicalAttackScript : IBattleScript
    {
        public const Int32 Id = 0019;

        private readonly BattleCalculator _v;

        public PhisicalAttackScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Caster.IsPlayer)
                _v.WeaponPhisicalParams();
            else
                _v.NormalPhisicalParams();
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.CasterCommand.BonusElement();
            if (_v.CanAttackWeaponElementalCommand())
            {
                _v.CalcHpDamage();
                _v.TryAlterMagicStatuses();
            }
        }
    }
}