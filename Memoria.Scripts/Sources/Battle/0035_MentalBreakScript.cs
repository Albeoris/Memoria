using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Mental Break
    /// </summary>
    [BattleScript(Id)]
    public sealed class MentalBreakScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0035;

        private readonly BattleCalculator _v;

        public MentalBreakScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.MagicAccuracy();
            _v.Target.PenaltyShellHitRate();
            if (_v.TryMagicHit())
                _v.Target.MagicDefence /= 2;
        }

        public Single RateTarget()
        {
            Int32 defenceDiff = _v.Target.MagicDefence / 2;

            Single result = defenceDiff * BattleScriptAccuracyEstimate.RatePlayerAttackEvade(_v.Context.Evade);

            if (_v.Target.IsUnderStatus(BattleStatus.Shell))
                result *= BattleScriptAccuracyEstimate.RatePlayerAttackHit(_v.Context.HitRate >> 1);
            else
                result *= BattleScriptAccuracyEstimate.RatePlayerAttackHit(_v.Context.HitRate);

            if (_v.Target.IsPlayer)
                result *= -1;

            return result;
        }
    }
}