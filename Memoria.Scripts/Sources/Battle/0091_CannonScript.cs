using System;
using FF9;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Cannon
    /// </summary>
    [BattleScript(Id)]
    public sealed class CannonScript : IBattleScript
    {
        public const Int32 Id = 0091;

        private readonly BattleCalculator _v;

        public CannonScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CheckUnsafetyOrMiss())
                return;

            _v.MagicAccuracy();
            _v.Target.PenaltyShellHitRate();
            _v.PenaltyCommandDividedHitRate();
            if (!_v.TryMagicHit())
                return;

            _v.SetCommandAttack();
            _v.CasterCommand.BonusElement();
            if (!_v.CanAttackMagic())
                return;

            _v.CalcCannonProportionDamage();
        }
    }
}