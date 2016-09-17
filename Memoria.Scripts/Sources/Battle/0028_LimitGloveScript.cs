using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Limit Glove
    /// </summary>
    [BattleScript(Id)]
    public sealed class LimitGloveScript : IBattleScript
    {
        public const Int32 Id = 0028;

        private readonly BattleCalculator _v;

        public LimitGloveScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Caster.CurrentHp != 1)
            {
                _v.Context.Flags |= BattleCalcFlags.Miss;
                return;
            }

            _v.Target.Flags |= CalcFlag.HpAlteration;
            _v.Target.HpDamage = (Int16)(_v.Command.Power * 100 + _v.Command.HitRate);
        }
    }
}