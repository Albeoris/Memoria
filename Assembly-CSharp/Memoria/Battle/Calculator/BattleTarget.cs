using FF9;
using Memoria.Data;
using System;

namespace Memoria
{
    public sealed class BattleTarget : BattleUnit
    {
        private readonly CalcContext _context;
        private readonly BattleStatus _initialCheckPoint;

        public BattleTarget(BTL_DATA data, CalcContext context)
            : base(data)
        {
            _context = context;
            _initialCheckPoint = CheckPointData();
        }

        public BattleStatus AddededCheckPointStatuses => CheckPointData() & ~_initialCheckPoint;

        // TODO: These are already in BattleUnit... but removing them will invalidate compatibility with the Memoria.Scripts.dll of mods using it
        // Remove these fields the next time compatibility has to be broken anyway
        public EffectElement GuardElement
        {
            get { return Data.def_attr.invalid.ToEffectElement(); }
            set { Data.def_attr.invalid = new ElementSet(value); }
        }

        public EffectElement HalfElement
        {
            get { return Data.def_attr.half.ToEffectElement(); }
            set { Data.def_attr.half = new ElementSet(value); }
        }

        public EffectElement AbsorbElement
        {
            get { return Data.def_attr.absorb.ToEffectElement(); }
            set { Data.def_attr.absorb = new ElementSet(value); }
        }

        public EffectElement WeakElement
        {
            get { return Data.def_attr.weak.ToEffectElement(); }
            set { Data.def_attr.weak = new ElementSet(value); }
        }

        public void SetMagicDefense()
        {
            _context.DefensePower = MagicDefence;
        }

        public void SetPhysicalDefense()
        {
            _context.DefensePower = PhysicalDefence;
        }

        public void PhysicalPenaltyAndBonusAttack()
        {
            PenaltyDefenceAttack();
            BonusSleepOrMiniAttack();
        }

        public void PenaltyDefenceAttack()
        {
            if (IsUnderAnyStatus(BattleStatus.Defend | BattleStatus.Protect))
                --_context.DamageModifierCount;
        }

        public void PenaltyShellAttack()
        {
            if (IsUnderAnyStatus(BattleStatus.Shell))
                --_context.DamageModifierCount;
        }

        public void PenaltyShellHitRate()
        {
            if (IsUnderAnyStatus(BattleStatus.Shell))
                _context.HitRate >>= 1;
        }

        public void PenaltyDefenceHitRate()
        {
            if (IsUnderAnyStatus(BattleStatus.Defend))
                _context.HitRate /= 2;
        }

        public void PenaltyDistractHitRate()
        {
            // Dummied
            if (HasSupportAbility(SupportAbility1.Distract))
                _context.HitRate /= 2;
        }

        public void PenaltyBanishHitRate()
        {
            if (IsUnderAnyStatus(BattleStatus.Vanish))
                _context.HitRate = 0;
        }

        public void PenaltyPhysicalEvade()
        {
            if (IsUnderAnyStatus(BattleStatusConst.PenaltyEvade))
                _context.Evade = 0;
        }

        private void BonusSleepOrMiniAttack()
        {
            if (IsUnderAnyStatus(BattleStatus.Sleep | BattleStatus.Mini))
                ++_context.DamageModifierCount;
        }

        #region LEGACY ELEMENTS API
        public void PenaltyHalfElement(EffectElement element)
        {
            if (IsHalfElement(element))
                --_context.DamageModifierCount;
        }

        public void PenaltyAbsorbElement(EffectElement element)
        {
            if (CanAbsorbElement(element))
                _context.DefensePower = 0;
        }

        public void BonusWeakElement(EffectElement element)
        {
            if (IsWeakElement(element))
                ++_context.DamageModifierCount;
        }

        public Boolean IsGuardElement(EffectElement element)
        {
            return (GuardElement & element) != 0;
        }

        public Boolean IsHalfElement(EffectElement element)
        {
            return (HalfElement & element) != 0;
        }

        public Boolean IsAbsorbElement(EffectElement element)
        {
            return (AbsorbElement & element) != 0;
        }

        public Boolean IsWeakElement(EffectElement element)
        {
            return (WeakElement & element) != 0;
        }

        public Boolean CanGuardElement(EffectElement element)
        {
            if (IsGuardElement(element))
            {
                _context.Flags |= BattleCalcFlags.Guard;
                return true;
            }
            return false;
        }

        public Boolean CanAbsorbElement(EffectElement element)
        {
            if (IsAbsorbElement(element))
            {
                _context.Flags |= BattleCalcFlags.Absorb;
                return true;
            }
            return false;
        }

        public Boolean CanAttackElement(EffectElement element)
        {
            if (CanGuardElement(element))
                return false;
            PenaltyHalfElement(element);
            PenaltyAbsorbElement(element);
            BonusWeakElement(element);
            AlterStatuses(element);
            return true;
        }

        public void AlterStatuses(EffectElement element)
        {
            if ((element & (EffectElement.Cold | EffectElement.Aqua)) != 0)
                RemoveStatus(BattleStatus.Heat);
            if ((element & EffectElement.Fire) != 0)
                RemoveStatus(BattleStatus.Freeze);
        }
        #endregion

        public void PenaltyHalfElement(ElementSet element)
        {
            if (IsHalfElement(element))
                --_context.DamageModifierCount;
        }

        public void PenaltyAbsorbElement(ElementSet element)
        {
            if (CanAbsorbElement(element))
                _context.DefensePower = 0;
        }

        public void BonusWeakElement(ElementSet element)
        {
            if (IsWeakElement(element))
                ++_context.DamageModifierCount;
        }

        public Boolean IsGuardElement(ElementSet element)
        {
            return ElementSet.CountCommon(GuardElementEx, element) > 0;
        }

        public Boolean IsHalfElement(ElementSet element)
        {
            return ElementSet.CountCommon(HalfElementEx, element) > 0;
        }

        public Boolean IsAbsorbElement(ElementSet element)
        {
            return ElementSet.CountCommon(AbsorbElementEx, element) > 0;
        }

        public Boolean IsWeakElement(ElementSet element)
        {
            return ElementSet.CountCommon(WeakElementEx, element) > 0;
        }

        public Boolean CanGuardElement(ElementSet element)
        {
            if (IsGuardElement(element))
            {
                _context.Flags |= BattleCalcFlags.Guard;
                return true;
            }
            return false;
        }

        public Boolean CanAbsorbElement(ElementSet element)
        {
            if (IsAbsorbElement(element))
            {
                _context.Flags |= BattleCalcFlags.Absorb;
                return true;
            }
            return false;
        }

        public Boolean CanAttackElement(ElementSet element)
        {
            if (CanGuardElement(element))
                return false;
            PenaltyHalfElement(element);
            PenaltyAbsorbElement(element);
            BonusWeakElement(element);
            AlterStatuses(element);
            return true;
        }

        public void AlterStatuses(ElementSet element)
        {
            if (element.Contains(ElementToken.Cold) || element.Contains(ElementToken.Aqua))
                RemoveStatus(BattleStatus.Heat);
            if (element.Contains(ElementToken.Fire))
                RemoveStatus(BattleStatus.Freeze);
        }

        public Boolean IsRunningAway()
        {
            return IsPlayer && FF9StateSystem.Battle.FF9Battle.btl_escape_key != 0;
        }

        public void TryAlterStatuses(BattleStatus status, Boolean changeContext, BattleUnit inflicter = null)
        {
            BattleStatus prev_status = this.PermanentStatus | this.CurrentStatus;
            UInt32 result = btl_stat.AlterStatuses(this, status, inflicter, true);
            this._context.AddedStatuses |= (this.PermanentStatus | this.CurrentStatus) & ~prev_status;
            if (changeContext)
            {
                if (result == btl_stat.ALTER_RESIST)
                    _context.Flags |= BattleCalcFlags.Guard;
                else if (result == btl_stat.ALTER_INVALID)
                    _context.Flags |= BattleCalcFlags.Miss;
            }
        }

        public void TryAlterSingleStatus(BattleStatusId status, Boolean changeContext, BattleUnit inflicter = null, params Object[] parameters)
        {
            BattleStatus prev_status = this.PermanentStatus | this.CurrentStatus;
            UInt32 result = btl_stat.AlterStatus(this, status, inflicter, true, parameters);
            this._context.AddedStatuses |= (this.PermanentStatus | this.CurrentStatus) & ~prev_status;
            if (changeContext)
            {
                if (result == btl_stat.ALTER_RESIST)
                    _context.Flags |= BattleCalcFlags.Guard;
                else if (result == btl_stat.ALTER_INVALID)
                    _context.Flags |= BattleCalcFlags.Miss;
            }
        }

        public Boolean CanBeAttacked()
        {
            if (!IsUnderAnyStatus(BattleStatus.Petrify | BattleStatus.Death))
                return true;

            _context.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean CanBeRevived()
        {
            if (IsUnderAnyStatus(BattleStatus.Petrify))
            {
                _context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            if (IsUnderAnyStatus(BattleStatus.Death) && IsZombie)
            {
                _context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            return true;
        }

        public Boolean CanBeHealed()
        {
            if (!IsUnderAnyStatus(BattleStatus.Death))
                return true;

            _context.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean CheckUnsafetyOrMiss()
        {
            if (!IsUnderAnyStatus(BattleStatus.EasyKill))
                return true;

            _context.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean CheckUnsafetyOrGuard()
        {
            if (!IsUnderAnyStatus(BattleStatus.EasyKill))
                return true;

            _context.Flags |= BattleCalcFlags.Guard;
            return false;
        }

        public Boolean CheckIsPlayer()
        {
            if (IsPlayer)
                return true;

            _context.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean TryKillFrozen()
        {
            if (!IsUnderAnyStatus(BattleStatus.Freeze) || IsUnderAnyStatus(BattleStatus.Petrify))
                return false;
            if (IsUnderAnyStatus(BattleStatus.EasyKill)) // Behaviour added by Memoria with no influence on vanilla - Boss can't die when frozen
                return false;

            BattleVoice.TriggerOnStatusChange(Data, BattleVoice.BattleMoment.Used, BattleStatusId.Freeze);
            btl_cmd.KillSpecificCommand(Data, BattleCommandId.SysStone);
            Kill();
            UIManager.Battle.SetBattleFollowMessage(BattleMesages.ImpactCrushes);
            return true;
        }

        public void GambleDefence()
        {
            // Dummied
            if (_context.DefensePower != 0 && HasSupportAbility(SupportAbility1.GambleDefence))
                _context.DefensePower = (Int16)(Comn.random16() % (_context.DefensePower << 1));
        }

        private BattleStatus CheckPointData()
        {
            if (Data.cur.hp == 0) // Using this instead of "CurrentHp" avoids considering bosses under 10 000 HP as dead here
                return BattleStatus.Death;
            else
                return btl_para.CheckPointDataStatus(this);
        }
    }
}
