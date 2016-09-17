using System;

namespace Memoria.Scripts.Battle
{
    [BattleScript(Id)]
    public sealed class MagicAttackScript : IBattleScript
    {
        public const Int32 Id = 0009;

        private readonly BattleCalculator _v;

        public MagicAttackScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.NormalMagicParams();
            _v.Caster.PenaltyMini();
            _v.Target.PenaltyShellAttack();
            _v.PenaltyCommandDividedAttack();
            _v.CasterCommand.BonusElement();

            if (_v.CanAttackMagic())
            {
                _v.TargetCommand.CalcHpDamage();
                _v.TargetCommand.TryAlterMagicStatuses();
            }
        }
    }
}