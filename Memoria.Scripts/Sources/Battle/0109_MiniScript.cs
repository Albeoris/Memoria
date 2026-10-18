using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Iai Strike
    /// </summary>
    [BattleScript(Id)]
    public sealed class MiniScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0109;

        private readonly BattleCalculator _v;

        public MiniScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Target.IsUnderAnyStatus(BattleStatus.Mini))
            {
                _v.TryAlterCommandStatuses();
                return;
            }

            _v.MagicAccuracy();
            _v.Target.PenaltyShellHitRate();
            _v.PenaltyCommandDividedHitRate();
            if (_v.TryMagicHit())
                _v.TryAlterCommandStatuses();
        }

        public Single RateTarget()
        {
            if (_v.Target.IsUnderAnyStatus(BattleStatusConst.ApplyReflect) && !_v.Command.IsReflectNull)
                return 0;

            Int32 statusRate = BattleScriptStatusEstimate.RateStatuses(_v.Command.AbilityStatus);
            Single accuracyRate = 1f;
            Boolean clearStatus = _v.Target.IsUnderAnyStatus(BattleStatus.Mini) && (_v.Command.AbilityStatus & BattleStatus.Mini) != 0;
            if (!_v.Target.IsUnderAnyStatus(BattleStatus.Mini))
            {
                accuracyRate = BattleScriptAccuracyEstimate.RatePlayerAttackEvade(_v.Context.Evade);
                if (_v.Target.IsUnderAnyStatus(BattleStatus.Shell))
                    accuracyRate *= BattleScriptAccuracyEstimate.RatePlayerAttackHit(_v.Context.HitRate >> 1);
                else
                    accuracyRate *= BattleScriptAccuracyEstimate.RatePlayerAttackHit(_v.Context.HitRate);
            }
            return accuracyRate * statusRate * (clearStatus ^ _v.Target.IsPlayer ? 1 : -1);
        }
    }
}
