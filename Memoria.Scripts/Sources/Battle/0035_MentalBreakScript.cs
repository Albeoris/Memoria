using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Mental Break
    /// </summary>
    [BattleScript(Id)]
    public sealed class MentalBreakScript : IBattleScript
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
            if (_v.TargetCommand.TryMagicHit())
                _v.Target.MagicDefence /= 2;
        }
    }
}