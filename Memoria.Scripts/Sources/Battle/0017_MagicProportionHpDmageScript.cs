using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Demi, Aqua Breath, Demi Shock, Worm Hole
    /// </summary>
    [BattleScript(Id)]
    public sealed class MagicProportionHpDmageScript : IBattleScript
    {
        public const Int32 Id = 0017;

        private readonly BattleCalculator _v;

        public MagicProportionHpDmageScript(BattleCalculator v)
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
            if (_v.CanAttackMagic())
                _v.CalcProportionDamage();
        }
    }
}