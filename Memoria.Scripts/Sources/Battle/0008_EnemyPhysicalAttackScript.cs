using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Weapon: Blood Sword
    /// </summary>
    [BattleScript(Id)]
    public sealed class EnemyPhysicalAttackScript : IBattleScript
    {
        public const Int32 Id = 0008;

        private readonly BattleCalculator _v;

        public EnemyPhysicalAttackScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (_v.Target.TryKillFrozen())
                return;

            _v.PhysicalAccuracy();
            if (!_v.TryPhysicalHit())
                return;

            _v.NormalPhysicalParams();
            _v.Caster.EnemyTranceBonusAttack();
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.BonusBackstabAndPenaltyLongDistance();
            _v.BonusElement();
            if (!_v.CanAttackElementalCommand())
                return;

            _v.TryCriticalHit();
            _v.CalcPhysicalHpDamage();
            _v.RaiseTrouble();
            _v.TryAlterMagicStatuses();
        }
    }
}
