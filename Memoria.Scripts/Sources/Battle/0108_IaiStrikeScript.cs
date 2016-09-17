using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Iai Strike
    /// </summary>
    [BattleScript(Id)]
    public sealed class IaiStrikeScript : IBattleScript
    {
        public const Int32 Id = 0108;

        private readonly BattleCalculator _v;

        public IaiStrikeScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CheckUnsafetyOrGuard())
                return;

            _v.MagicAccuracy();
            _v.Target.PenaltyShellHitRate();
            _v.PenaltyCommandDividedHitRate();
            if (_v.TargetCommand.TryMagicHit())
                _v.TargetCommand.TryAlterCommandStatuses();
        }
    }
}