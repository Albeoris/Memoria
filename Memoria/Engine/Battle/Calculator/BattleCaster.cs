using System;
using FF9;

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
            if (IsUnderStatus(BattleStatus.Mini))
            {
                _context.Attack = 1;
            }
            else if (IsUnderStatus(BattleStatus.Berserk | BattleStatus.Trans))
            {
                _context.Attack = (Int16)(_context.Attack * 3 >> 1);
            }
        }

        public void PenaltyMini()
        {
            if (IsUnderStatus(BattleStatus.Mini))
                _context.Attack /= 2;
        }

        public void PenaltyPhysicalHitRate()
        {
            if (IsUnderStatus(BattleStatus.Dark | BattleStatus.Confu))
                _context.HitRate /= 2;
        }

        public void BonusConcentrate()
        {
            if (HasSupportAbility(SupportAbility2.Concentrate))
                _context.Attack = (Int16)(_context.Attack * 3 >> 1);
        }

        public void BonusPhysicalEvade()
        {
            if (IsUnderStatus(BattleStatus.Trans | BattleStatus.Banish))
                _context.Evade = 0;
        }

        public void BonusWeaponElement()
        {
            if ((WeaponElement & BonusElement) != 0)
                _context.Attack = (Int16)(_context.Attack * 3 >> 1);
        }
    }
}