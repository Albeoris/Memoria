using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Armour Break
    /// </summary>
    [BattleScript(Id)]
    public sealed class ArmourBreakScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0033;

        private readonly BattleCalculator _v;

        public ArmourBreakScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.MagicAccuracy();
            _v.Target.PenaltyShellHitRate();
            if (_v.TryMagicHit())
                _v.Target.PhisicalDefence /= 2;
        }

        public Single RateTarget()
        {
            Int32 defenceDiff = _v.Target.PhisicalDefence / 2;

            Single result = defenceDiff * BattleScriptAccuracyEstimate.RatePlayerAttackEvade(_v.Context.Evade);

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