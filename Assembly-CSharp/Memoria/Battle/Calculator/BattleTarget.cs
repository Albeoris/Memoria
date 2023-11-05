using System;
using FF9;
using Memoria.Data;

namespace Memoria
{
    public sealed class BattleTarget : BattleUnit
    {
        private readonly CalcContext _context;

        public BattleTarget(BTL_DATA data, CalcContext context)
            : base(data)
        {
            _context = context;
        }

        public EffectElement GuardElement
        {
            get { return (EffectElement)Data.def_attr.invalid; }
            set { Data.def_attr.invalid = (Byte)value; }
        }

        public EffectElement HalfElement
        {
            get { return (EffectElement)Data.def_attr.half; }
            set { Data.def_attr.half = (Byte)value; }
        }

        public EffectElement AbsorbElement
        {
            get { return (EffectElement)Data.def_attr.absorb; }
            set { Data.def_attr.absorb = (Byte)value; }
        }

        public EffectElement WeakElement
        {
            get { return (EffectElement)Data.def_attr.weak; }
            set { Data.def_attr.weak = (Byte)value; }
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
                _context.Attack >>= 1;
        }

        public void PenaltyShellAttack()
        {
            if (IsUnderAnyStatus(BattleStatus.Shell))
                _context.Attack >>= 1;
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

        public void PenaltyHalfElement(EffectElement element)
        {
            if (IsHalfElement(element))
                _context.Attack >>= 1;
        }

        public void PenaltyAbsorbElement(EffectElement element)
        {
           if (CanAbsorbElement(element))
                _context.DefensePower = 0;
        }

        public void BonusWeakElement(EffectElement element)
        {
            if (IsWeakElement(element))
                _context.Attack = (Int16)(_context.Attack * 3 >> 1);
        }

        private void BonusSleepOrMiniAttack()
        {
            if (IsUnderAnyStatus(BattleStatus.Sleep | BattleStatus.Mini))
                _context.Attack = (Int16)(_context.Attack * 3 >> 1);
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

        public Boolean IsRunningAway()
        {
            return IsPlayer && FF9StateSystem.Battle.FF9Battle.btl_escape_key != 0;
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

        public void TryAlterStatuses(BattleStatus status, Boolean changeContext, BattleUnit inflicter = null)
        {
            BattleStatus prev_status = this.PermanentStatus | this.CurrentStatus;
            UInt32 result = btl_stat.AlterStatuses(Data, status, inflicter?.Data, true);
            this._context.AddedStatuses |= (this.PermanentStatus | this.CurrentStatus) & ~prev_status;
            if (changeContext)
            {
                if (result == 0)
                    _context.Flags |= BattleCalcFlags.Guard;
                else if (result == 1)
                    _context.Flags |= BattleCalcFlags.Miss;
            }
        }

        public void AlterStatuses(EffectElement element)
        {
            if ((element & (EffectElement.Cold | EffectElement.Aqua)) != 0)
                RemoveStatus(BattleStatus.Heat);

            if ((element & EffectElement.Fire) != 0)
                RemoveStatus(BattleStatus.Freeze);
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

            if (Configuration.Mod.TranceSeek && IsUnderAnyStatus(BattleStatus.EasyKill)) // TRANCE SEEK - Boss can't die when freeze. Can't move in Memoria.Scripts because depending with SBattleCalculator :(
                return false;

            BattleVoice.TriggerOnStatusChange(Data, "Used", BattleStatus.Freeze);
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
    }
}