using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Cherry Blossom, Climhazzard
    /// </summary>
    [BattleScript(Id)]
    public sealed class OriginalMagicAttackScript : IBattleScript
    {
        public const Int32 Id = 0020;

        private readonly BattleCalculator _v;

        public OriginalMagicAttackScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.OriginalMagicParams();
            _v.Caster.PenaltyMini();
            _v.Target.PenaltyShellAttack();
            _v.CasterCommand.BonusElement();

            if (_v.CanAttackMagic())
            {
                _v.TargetCommand.CalcHpDamage();
                _v.TargetCommand.TryAlterMagicStatuses();
            }
        }
    }
}