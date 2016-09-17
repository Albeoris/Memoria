using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// LV4 Holy
    /// </summary>
    [BattleScript(Id)]
    public sealed class Lv4HolyScript : IBattleScript
    {
        public const Int32 Id = 0023;

        private readonly BattleCalculator _v;

        public Lv4HolyScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.TargetCommand.IsTargetLevelMultipleOfCommandRate() && _v.Target.CanBeAttacked())
            {
                _v.NormalMagicParams();
                _v.Caster.PenaltyMini();
                _v.Target.PenaltyShellAttack();
                _v.CasterCommand.BonusElement();

                if (_v.CanAttackElementalCommand())
                    _v.TargetCommand.CalcHpDamage();
            }
        }
    }
}