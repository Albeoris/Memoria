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

        public void SetPhisicalDefense()
        {
            _context.DefensePower = PhisicalDefence;
        }

        public void PhysicalPenaltyAndBonusAttack()
        {
            PenaltyDefenceAttack();
            BonusSleepOrMiniAttack();
        }

        public void PenaltyDefenceAttack()
        {
            if (IsUnderStatus(BattleStatus.Defend | BattleStatus.Protect))
                _context.Attack >>= 1;
        }

        public void PenaltyShellAttack()
        {
            if (IsUnderStatus(BattleStatus.Shell))
                _context.Attack >>= 1;
        }

        public void PenaltyShellHitRate()
        {
            if (IsUnderStatus(BattleStatus.Shell))
                _context.HitRate >>= 1;
        }

        public void PenaltyDefenceHitRate()
        {
            if (IsUnderStatus(BattleStatus.Defend))
                _context.HitRate /= 2;
        }

        public void PenaltyDistractHitRate()
        {
            if (HasSupportAbility(SupportAbility1.Distract))
                _context.HitRate /= 2;
        }

        public void PenaltyBanishHitRate()
        {
            if (IsUnderStatus(BattleStatus.Vanish))
                _context.HitRate = 0;
        }

        public void PenaltyPhysicalEvade()
        {
            const BattleStatus status = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Virus
                                        | BattleStatus.Blind | BattleStatus.Confuse | BattleStatus.Stop | BattleStatus.Sleep | BattleStatus.Freeze;

            if (IsUnderStatus(status))
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
            if (IsUnderStatus(BattleStatus.Sleep | BattleStatus.Mini))
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

        public void TryAlterStatuses(BattleStatus status, Boolean changeContext)
        {
            UInt32 result = btl_stat.AlterStatuses(Data, (UInt32)status);
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
            if (!IsUnderStatus(BattleStatus.Petrify | BattleStatus.Death))
                return true;

            _context.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean CanBeRevived()
        {
            if (IsUnderStatus(BattleStatus.Petrify))
            {
                _context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            if (IsUnderStatus(BattleStatus.Death) && IsZombie)
            {
                _context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            return true;
        }

        public Boolean CanBeHealed()
        {
            if (!IsUnderStatus(BattleStatus.Death))
                return true;

            _context.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean CheckUnsafetyOrMiss()
        {
            if (!IsUnderStatus(BattleStatus.EasyKill))
                return true;

            _context.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean CheckUnsafetyOrGuard()
        {
            if (!IsUnderStatus(BattleStatus.EasyKill))
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
            if (!IsUnderStatus(BattleStatus.Freeze) || IsUnderStatus(BattleStatus.Petrify))
                return false;

            btl_cmd.KillSpecificCommand(Data, 62);
            Kill();
            UIManager.Battle.SetBattleFollowMessage((Int32)BattleMesages.ImpactCrushes);
            return true;
        }

        public void GambleDefence()
        {
            if (_context.DefensePower != 0 && HasSupportAbility(SupportAbility1.GambleDefence))
                _context.DefensePower = (Int16)(Comn.random16() % (_context.DefensePower << 1));
        }

        public void RaiseTrouble()
        {
            if (IsUnderStatus(BattleStatus.Trouble))
                Data.fig_info |= Param.FIG_INFO_TROUBLE;
        }
    }
}