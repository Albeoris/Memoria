using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// LV4 Holy
    /// </summary>
    [BattleScript(Id)]
    public sealed class LvHolyScript : IBattleScript
    {
        public const Int32 Id = 0023;

        private readonly BattleCalculator _v;

        public LvHolyScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.IsTargetLevelMultipleOfCommandRate() && _v.Target.CanBeAttacked())
            {
                _v.NormalMagicParams();
                _v.Caster.EnemyTranceBonusAttack();
                _v.Caster.PenaltyMini();
                _v.Target.PenaltyShellAttack();
                _v.BonusElement();

                if (_v.CanAttackElementalCommand())
                    _v.CalcHpDamage();
            }
        }
    }
}
