using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Power Break
    /// </summary>
    [BattleScript(Id)]
    public sealed class PowerBreakScript : IBattleScript
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
            if (_v.TargetCommand.TryMagicHit())
                _v.Target.Strength = (Byte)(_v.Target.Strength * 3 / 4);
        }
    }
}