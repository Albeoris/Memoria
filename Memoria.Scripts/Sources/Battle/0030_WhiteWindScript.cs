using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// White Wind
    /// </summary>
    [BattleScript(Id)]
    public sealed class WhiteWindScript : IBattleScript
    {
        public const Int32 Id = 0030;

        private readonly BattleCalculator _v;

        public WhiteWindScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= CalcFlag.HpAlteration;

            if (!_v.Target.IsZombie)
                _v.Target.Flags |= CalcFlag.HpRecovery;

            if (_v.Command.Power == 0) // Value in vanilla
                _v.Target.HpDamage = (Int32)(_v.Caster.MaximumHp / 3);
            else
                _v.Target.HpDamage = (Int32)(_v.Caster.MaximumHp * _v.Command.Power / 100);
        }
    }
}
