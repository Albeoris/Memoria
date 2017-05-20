using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Weapon 1-5, 7
    /// </summary>
    public abstract class BaseWeaponScript : IBattleScript
    {
        private readonly BattleCalculator _v;
        private readonly CalcAttackBonus _bonus;

        protected BaseWeaponScript(BattleCalculator v, CalcAttackBonus bonus)
        {
            _v = v;
            _bonus = bonus;
        }

        public virtual void Perform()
        {
            if (_v.Target.TryKillFrozen())
                return;

            _v.PhysicalAccuracy();
            if (!_v.Caster.HasSupportAbility(SupportAbility1.Accuracy) && !_v.TryPhysicalHit())
                return;

            _v.WeaponPhisicalParams(_bonus);
            _v.BonusSupportAbilitiesAttack();
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.GambleDefence();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            if (_v.Caster.IsUnderStatus(BattleStatus.Trance) && _v.Caster.PlayerIndex == CharacterPresetId.Steiner)
                _v.Context.Attack *= 2;

            _v.BonusBackstabAndPenaltyLongDistance();
            _v.Caster.BonusWeaponElement();
            if (_v.CanAttackWeaponElementalCommand())
            {
                _v.TryCriticalHit();
                _v.PenaltyReverseAttack();
                _v.CalcPhysicalHpDamage();
                _v.Target.RaiseTrouble();
                _v.ConsumeMpAttack();

                TryAddWeaponStatus();
            }
        }

        private void TryAddWeaponStatus()
        {
            if (_v.Caster.HasSupportAbility(SupportAbility1.AddStatus))
            {
                _v.Context.StatusRate = _v.Caster.WeaponRate;
                if (_v.Context.StatusRate > GameRandom.Next16() % 100)
                    _v.Context.Flags |= BattleCalcFlags.AddStat;
            }
        }
    }
}