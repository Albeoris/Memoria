using System;

namespace Memoria
{
    public sealed class CalcCasterCommand
    {
        private readonly BattleCaster _caster;
        private readonly BattleCommand _command;
        private readonly CalcContext _context;

        public CalcCasterCommand(BattleCaster caster, BattleCommand command, CalcContext context)
        {
            _caster = caster;
            _command = command;

            _context = context;
        }

        public void SetWeaponPower()
        {
            _context.AttackPower = (Int16)(_caster.WeaponPower * _command.Power / 10);
        }

        public void SetWeaponPowerSum()
        {
            _context.AttackPower = (Int16)(_caster.WeaponPower + _command.Power);
        }

        public void BonusElement()
        {
            if (_command.HasElement(_caster.BonusElement))
                _context.Attack = (Int16)(_context.Attack * 3 >> 1);
        }
    }
}