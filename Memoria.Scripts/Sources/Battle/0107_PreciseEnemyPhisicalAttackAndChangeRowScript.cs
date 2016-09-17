using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// ???
    /// </summary>
    [BattleScript(Id)]
    public sealed class PreciseEnemyPhisicalAttackAndChangeRowScript : IBattleScript
    {
        public const Int32 Id = 0107;

        private readonly BattleCalculator _v;

        public PreciseEnemyPhisicalAttackAndChangeRowScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.NormalPhisicalParams();
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.GambleDefence();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.BonusBackstabAndPenaltyLongDistance();
            _v.CasterCommand.BonusElement();
            if (!_v.CanAttackElementalCommand())
                return;

            _v.TargetCommand.CalcHpDamage();
            if (_v.Target.Row != 0)
                _v.Target.ChangeRow();

            _v.TargetCommand.TryAlterMagicStatuses();
        }
    }
}