using System;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Weapon: Blood Sword
    /// </summary>
    [BattleScript(Id)]
    public sealed class EnemyPhisicalAttackScript : IBattleScript
    {
        public const Int32 Id = 0008;

        private readonly BattleCalculator _v;

        public EnemyPhisicalAttackScript(BattleCalculator v)
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

            _v.NormalPhisicalParams();
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.GambleDefence();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.BonusBackstabAndPenaltyLongDistance();
            _v.CasterCommand.BonusElement();
            if (!_v.CanAttackElementalCommand())
                return;

            _v.TryCriticalHit();
            _v.CalcPhysicalHpDamage();
            _v.Target.RaiseTrouble();
            _v.TargetCommand.TryAlterMagicStatuses();
        }
    }
}