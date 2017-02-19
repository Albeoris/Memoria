using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Spare Change
    /// </summary>
    [BattleScript(Id)]
    public sealed class SpareChangeScript : IBattleScript
    {
        public const Int32 Id = 0038;

        private readonly BattleCalculator _v;

        public SpareChangeScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;

            Int32 power = _v.Command.Power * _v.Caster.Level;
            Int64 gil = GameState.Gil + power;
            Int64 damage = power * power / 10U * _v.Caster.Will / gil;
            if (_v.Command.Id == BattleCommandId.Elan)
                damage /= BattleState.TargetCount(false);

            _v.Target.HpDamage = (Int16)Math.Min(9999, damage);
        }
    }
}