using System;
using FF9;
using Memoria.Data;

namespace Memoria
{
    public sealed class BattleCaster : BattleUnit
    {
        private readonly CalcContext _context;

        public BattleCaster(BTL_DATA data, CalcContext context)
            : base(data)
        {
            _context = context;
        }

        public void SetMagicAttack()
        {
            _context.Attack = (Int16)(Magic + Comn.random16() % (1 + (Level + Magic >> 3)));
        }

        public void SetPhisicalAttack()
        {
            _context.Attack = (Int16)(Strength + Comn.random16() % (1 + (Level + Strength >> 2)));
        }

        public void SetLowPhisicalAttack()
        {
            _context.Attack = (Int16)(Strength + Comn.random16() % (1 + (Level + Strength >> 3)));
        }

        public void PhysicalPenaltyAndBonusAttack()
        {
            if (IsUnderAnyStatus(BattleStatus.Mini))
                _context.Attack = 1;
            else if (IsUnderAnyStatus(BattleStatus.Berserk | BattleStatus.Trance))
                _context.Attack = (Int16)(_context.Attack * 3 >> 1);
            }

        public void PenaltyMini()
        {
            if (IsUnderAnyStatus(BattleStatus.Mini))
                _context.Attack /= 2;
        }

        public void PenaltyPhysicalHitRate()
        {
            if (Configuration.Mod.TranceSeek)
            {
                if (IsUnderAnyStatus(BattleStatus.Blind))
                    _context.HitRate /= 2;
            }
            else
            {
                if (IsUnderAnyStatus(BattleStatus.Blind | BattleStatus.Confuse))
                    _context.HitRate /= 2;
            }
        }

        public void BonusConcentrate()
        {
            // Dummied
            if (HasSupportAbility(SupportAbility2.Concentrate))
                _context.Attack = (Int16)(_context.Attack * 3 >> 1);
        }

        public void BonusPhysicalEvade()
        {
            if (IsUnderAnyStatus(BattleStatus.Trance | BattleStatus.Vanish))
                _context.Evade = 0;
        }

        public void BonusWeaponElement()
        {
            if ((WeaponElement & BonusElement) != 0)
                _context.Attack = (Int16)(_context.Attack * 3 >> 1);
        }
    }
}