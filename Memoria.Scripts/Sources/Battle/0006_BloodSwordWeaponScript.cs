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
            if (!_v.Caster.HasSupportAbility(SupportAbility1.Accuracy) && !_v.TryPhysicalHit())
                return;

            _v.WeaponPhisicalParams(CalcAttackBonus.Simple);
            _v.BonusSupportAbilitiesAttack();
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.GambleDefence();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.BonusBackstabAndPenaltyLongDistance();
            _v.TryCriticalHit();
            _v.PenaltyReverseAttack();
            _v.PrepareHpDraining();

            Int16 damage = (Int16)Math.Min(9999, _v.Context.EnsurePowerDifference * _v.Context.EnsureAttack);
            _v.Target.HpDamage = damage;
            _v.Caster.HpDamage = damage;

            _v.ConsumeMpAttack();
        }
    }
}