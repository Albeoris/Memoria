using Memoria.Data;
using Memoria.Prime;
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

            if (_v.Target.CanBeAttacked() && _v.Target.IsUnderAnyStatus(BattleStatus.Mini))
                return _v.Target.IsPlayer ? 20 : -20;

            return 0;
        }
    }
}
