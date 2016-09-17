using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Silence, Confuse, Berserk, Blind, Sleep, Slow, Stop, Poison, Break, Doom, Bad Breath, Night, Freeze, Mustard Bomb, Annoy, Countdown
    /// </summary>
    [BattleScript(Id)]
    public sealed class MagicApplyNegativeStatusScript : IBattleScript
    {
        public const Int32 Id = 0011;

        private readonly BattleCalculator _v;

        public MagicApplyNegativeStatusScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.MagicAccuracy();
            _v.Target.PenaltyShellHitRate();
            _v.PenaltyCommandDividedHitRate();
            if (_v.TargetCommand.TryMagicHit())
                _v.TargetCommand.TryAlterCommandStatuses();
        }
    }
}