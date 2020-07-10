using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Weapon: Blood Sword
    /// </summary>
    [BattleScript(Id)]
    public sealed class BloodSwordWeaponScript : IBattleScript
    {
        public const Int32 Id = 0006;

        private readonly BattleCalculator _v;

        public BloodSwordWeaponScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.IsCasterNotTarget() || !_v.Target.CanBeAttacked() || _v.Target.TryKillFrozen())
                return;

            _v.PhysicalAccuracy();
            if (!_v.TryPhysicalHit())
                return;

            _v.WeaponPhisicalParams(CalcAttackBonus.Simple);
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.BonusBackstabAndPenaltyLongDistance();
            _v.TryCriticalHit();
            _v.PenaltyReverseAttack();
            _v.PrepareHpDraining();
            _v.CalcPhysicalHpDamage();
            _v.Caster.HpDamage = _v.Target.HpDamage;
        }
    }
}