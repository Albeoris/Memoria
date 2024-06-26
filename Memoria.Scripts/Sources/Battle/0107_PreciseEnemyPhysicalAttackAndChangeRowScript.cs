using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// ???
    /// </summary>
    [BattleScript(Id)]
    public sealed class PreciseEnemyPhysicalAttackAndChangeRowScript : IBattleScript
    {
        public const Int32 Id = 0107;

        private readonly BattleCalculator _v;

        public PreciseEnemyPhysicalAttackAndChangeRowScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.NormalPhysicalParams();
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.BonusBackstabAndPenaltyLongDistance();
            _v.BonusElement();
            if (!_v.CanAttackElementalCommand())
                return;

            _v.CalcHpDamage();
            if (_v.Target.Row != 0)
                _v.Target.ChangeRow();

            _v.TryAlterMagicStatuses();
        }
    }
}
