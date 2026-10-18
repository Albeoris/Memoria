using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Chakra
    /// </summary>
    [BattleScript(Id)]
    public sealed class ChakraScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0037;

        private readonly BattleCalculator _v;

        public ChakraScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.Target.Flags |= (CalcFlag.HpDamageOrHeal | CalcFlag.MpDamageOrHeal);

            _v.Target.HpDamage = (Int32)(_v.Target.MaximumHp * _v.Command.Power / 100);
            _v.Target.MpDamage = (Int32)(_v.Target.MaximumMp * _v.Command.Power / 100);
        }

        public Single RateTarget()
        {
            _v.Target.HpDamage = (Int32)(_v.Target.MaximumHp * _v.Command.Power / 100);
            _v.Target.MpDamage = (Int32)(_v.Target.MaximumMp * _v.Command.Power / 100);

            Single rateHP = _v.Target.HpDamage * BattleScriptDamageEstimate.RateHpMp((Int32)_v.Target.CurrentHp, (Int32)_v.Target.MaximumHp);
            Single rateMP = _v.Target.MpDamage * BattleScriptDamageEstimate.RateHpMp((Int32)_v.Target.CurrentMp, (Int32)_v.Target.MaximumMp) * 10;

            Single rate = rateHP + rateMP;

            if (!_v.Target.IsPlayer)
                rate *= -1;

            return rate;
        }
    }
}
