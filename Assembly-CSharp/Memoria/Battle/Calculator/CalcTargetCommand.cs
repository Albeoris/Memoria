using System;
using FF9;
using Memoria.Data;
using UnityEngine;

namespace Memoria
{
    public sealed class CalcTargetCommand
    {
        private readonly BattleTarget _target;
        private readonly BattleCommand _command;
        private readonly CalcContext _context;

        public CalcTargetCommand(BattleTarget target, BattleCommand command, CalcContext context)
        {
            _target = target;
            _command = command;

            _context = context;
        }

        public Boolean CanUseCommandForFlight()
        {
            if (_target.IsLevitate && _command.IsGround)
            {
                _context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            return true;
        }

        public Boolean IsTargetLevelMultipleOfCommandRate()
        {
            if (_target.Level % _command.HitRate == 0)
                return true;

            _context.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public void CalcHpDamage()
        {
            _target.Flags |= CalcFlag.HpAlteration;

            Int32 damage = Math.Max(1, _context.PowerDifference);

            damage = damage * _context.EnsureAttack;
            if (_command.IsShortSummon)
                damage = damage * 2 / 3;

            if (damage > 9999)
                damage = 9999;

            if (_context.IsAbsorb)
                _target.Flags |= CalcFlag.HpRecovery;

            _target.HpDamage = (Int16)damage;
        }

        public void CalcProportionDamage()
        {
            if (_context.Attack > 100)
                _context.Attack = 100;

            Int32 damage = _target.MaximumHp * _context.Attack / 100;
            if (_command.IsShortSummon)
                damage = damage * 2 / 3;

            if (damage > 9999)
                damage = 9999;

            _target.Flags |= CalcFlag.HpAlteration;
            if (_context.IsAbsorb)
                _target.Flags |= CalcFlag.HpRecovery;

            _target.HpDamage = (Int16)damage;
        }

        public void CalcHpMagicRecovery()
        {
            _target.Flags |= CalcFlag.HpAlteration;

            Int32 recovery = Math.Min(9999, _context.AttackPower * _context.Attack);

            if (!_target.IsZombie)
                _target.Flags |= CalcFlag.HpRecovery;

            _target.HpDamage = (Int16)recovery;
        }

        public void TryAlterMagicStatuses()
        {
            if (_command.HitRate > Comn.random16() % 100)
                _target.TryAlterStatuses(_command.AbilityStatus, false);
        }

        public Boolean TryMagicHit()
        {
            if (_context.HitRate <= Comn.random16() % 100)
            {
                _context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            if (_context.Evade > Comn.random16() % 100)
            {
                _context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            return true;
        }

        public void TryAlterCommandStatuses()
        {
            BattleStatus status = _command.AbilityStatus;
            if (!_command.IsShortSummon && _command.Id == BattleCommandId.Summon2)
                status |= BattleStatus.Protes;

            _target.TryAlterStatuses(status, true);
        }

        public void TryRemoveAbilityStatuses()
        {
            if (!_target.TryRemoveStatuses(_command.AbilityStatus))
                _context.Flags |= BattleCalcFlags.Miss;
        }

        public void TryRemoveItemStatuses()
        {
            if (!_target.TryRemoveStatuses(_command.ItemStatus))
                _context.Flags |= BattleCalcFlags.Miss;
        }

        public void InstantKill()
        {
            if (_command.Power == 0)
            {
                _target.Kill();
                return;
            }

            if (_target.IsUnderStatus(BattleStatus.Disable))
            {
                _context.Flags |= BattleCalcFlags.Miss;
                return;
            }

            _context.Flags |= BattleCalcFlags.DirectHP;
            _target.Data.cur.hp = _command.Power;
            _target.FaceTheEnemy();
        }
    }
}