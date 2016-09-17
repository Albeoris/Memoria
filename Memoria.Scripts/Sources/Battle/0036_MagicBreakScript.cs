using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Magic Break
    /// </summary>
    [BattleScript(Id)]
    public sealed class MagicBreakScript : IBattleScript
    {
        public const Int32 Id = 0036;

        private readonly BattleCalculator _v;

        public MagicBreakScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.MagicAccuracy();
            _v.Target.PenaltyShellHitRate();
            if (_v.TargetCommand.TryMagicHit())
                _v.Target.Magic = (Byte)(_v.Target.Magic * 3 / 4);
        }
    }
}