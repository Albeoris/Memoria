using System;

namespace Memoria.Scripts.Battle
{
    [BattleScript(Id)]
    public sealed class MagicRecoveryScript : IBattleScript
    {
        public const Int32 Id = 0010;

        private readonly BattleCalculator _v;

        public MagicRecoveryScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.NormalMagicParams();
            _v.Caster.BonusConcentrate();
            _v.Caster.PenaltyMini();
            _v.PenaltyCommandDividedAttack();
            _v.TargetCommand.CalcHpMagicRecovery();
        }
    }
}