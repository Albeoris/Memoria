using Memoria.Data;
using System;

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
            _v.WeaponPhysicalParams(CalcAttackBonus.Simple);
            ++_v.Context.DamageModifierCount;

            _v.Caster.PenaltyMini();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.CalcPhysicalHpDamage();
        }
    }
}
