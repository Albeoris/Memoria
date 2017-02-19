using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Spear
    /// </summary>
    [BattleScript(Id)]
    public sealed class SpearScript : IBattleScript
    {
        public const Int32 Id = 0048;

        private readonly BattleCalculator _v;

        public SpearScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.WeaponPhisicalParams(CalcAttackBonus.Simple);
            if (_v.Caster.HasSupportAbility(SupportAbility1.HighJump))
                _v.Context.Attack *= 2;
            else
                _v.Context.Attack = (Int16)(_v.Context.Attack * 3 >> 1);

            _v.Caster.PenaltyMini();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.CalcPhysicalHpDamage();
        }
    }
}