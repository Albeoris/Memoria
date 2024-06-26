using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Power Break
    /// </summary>
    [BattleScript(Id)]
    public sealed class PowerBreakScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0034;

        private readonly BattleCalculator _v;

        public PowerBreakScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.MagicAccuracy();
            _v.Target.PenaltyShellHitRate();
            if (_v.TryMagicHit())
                _v.Target.Strength = (Byte)(_v.Target.Strength * 3 / 4);
        }

        public Single RateTarget()
        {
            Int32 strengthDiff = _v.Target.Strength - _v.Target.Strength * 3 / 4;

            Single result = strengthDiff * BattleScriptAccuracyEstimate.RatePlayerAttackEvade(_v.Context.Evade);

            if (_v.Target.IsUnderAnyStatus(BattleStatus.Shell))
                result *= BattleScriptAccuracyEstimate.RatePlayerAttackHit(_v.Context.HitRate >> 1);
            else
                result *= BattleScriptAccuracyEstimate.RatePlayerAttackHit(_v.Context.HitRate);

            if (_v.Target.IsPlayer)
                result *= -1;

            return result;
        }
    }
}
