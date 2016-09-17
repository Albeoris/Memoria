using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Armour Break
    /// </summary>
    [BattleScript(Id)]
    public sealed class ArmourBreakScript : IBattleScript
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
            if (_v.TargetCommand.TryMagicHit())
                _v.Target.PhisicalDefence /= 2;
        }
    }
}