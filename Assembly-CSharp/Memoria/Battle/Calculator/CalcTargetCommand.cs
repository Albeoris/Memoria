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
            if ((_target.IsLevitate && _command.IsGround) || (_target.MagicDefence == 255 && Configuration.Mod.TranceSeek))
            {
                _context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            return true;
        }

        public void TryAlterCommandStatuses()
        {
            BattleStatus status = _command.AbilityStatus;
            if (!_command.IsShortSummon && _command.Id == BattleCommandId.SummonEiko)
                status |= BattleStatus.Protect;

            _target.TryAlterStatuses(status, true);
        }

        public void TryAlterCommandStatusesTranceSeek(byte casterwill = 0)
        {
            BattleStatus status = _command.AbilityStatus;
            _target.TryAlterStatuses(status, true, false, casterwill);
        }

        public void TryRemoveAbilityStatuses(Boolean CureOld = false)
        {
            if (!_target.TryRemoveStatuses(_command.AbilityStatus) && !CureOld)
                _context.Flags |= BattleCalcFlags.Miss;
        }

        public void TryRemoveItemStatuses(Boolean CureOld = false)
        {
            if (!_target.TryRemoveStatuses(_command.ItemStatus) && !CureOld)
                _context.Flags |= BattleCalcFlags.Miss;
        }

        public void InstantKill()
        {
            if (_command.Power == 0)
            {
                if (_target.IsZombie)
                {
                    if (_target.CanBeAttacked())
                        _target.CurrentHp = _target.MaximumHp;
                }
                else if (_target.IsUnderAnyStatus(BattleStatus.EasyKill))
                    _context.Flags |= BattleCalcFlags.Guard;
                else
                    _target.Kill();
                return;
            }

            if (_target.IsUnderStatus(BattleStatus.Death))
            {
                _context.Flags |= BattleCalcFlags.Miss;
                return;
            }

            _context.Flags |= BattleCalcFlags.DirectHP;
            _target.CurrentHp = _command.Power;
            _target.FaceTheEnemy();
        }
    }
}