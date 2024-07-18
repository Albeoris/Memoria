using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// White Draw
    /// </summary>
    [BattleScript(Id)]
    public sealed class ThrowScript : IBattleScript
    {
        public const Int32 Id = 0042;

        private readonly BattleCalculator _v;

        public ThrowScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Target.TryKillFrozen())
                return;

            _v.Caster.SetLowPhysicalAttack();
            _v.Target.SetPhysicalDefense();
            _v.Context.AttackPower = _v.Command.Weapon.Power << 1;

            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.PhysicalPenaltyAndBonusAttack();

            if (_v.Target.HasCategory(EnemyCategory.Flight))
                ++_v.Context.DamageModifierCount;

            _v.CalcPhysicalHpDamage();
        }
    }
}
