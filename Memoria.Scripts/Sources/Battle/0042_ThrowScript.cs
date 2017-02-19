using System;
using Memoria.Data;

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

            _v.Caster.SetLowPhisicalAttack();
            _v.Target.SetPhisicalDefense();
            _v.Context.AttackPower = (Int16)(_v.Command.Weapon.Power << 1);

            if (_v.Caster.HasSupportAbility(SupportAbility1.PowerThrow))
                _v.Context.Attack = (Int16)(_v.Context.Attack * 3 >> 1);

            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.PhysicalPenaltyAndBonusAttack();

            if (_v.Target.HasCategory(EnemyCategory.Flight))
                _v.Context.Attack = (Int16)(_v.Context.Attack * 3 >> 1);

            _v.CalcPhysicalHpDamage();
        }
    }
}