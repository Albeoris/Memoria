using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Spear (mass)
    /// </summary>
    [BattleScript(Id)]
    public sealed class MassSpearScript : IBattleScript
    {
        public const Int32 Id = 0083;

        private readonly BattleCalculator _v;

        public MassSpearScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;

            _v.WeaponPhisicalParams(CalcAttackBonus.Random);
            if (_v.Caster.HasSupportAbility(SupportAbility1.HighJump))
                _v.Context.Attack *= 2;
            else
                _v.Context.Attack = (Int16)(_v.Context.Attack * 3 >> 1);

            _v.Caster.PenaltyMini();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            if (_v.Context.Attack < 4)
                _v.Context.Attack = 4;

            _v.Target.HpDamage = (Int16)Math.Min(9999, _v.Context.EnsurePowerDifference * _v.Context.Attack / BattleState.TargetCount(false));
        }
    }
}