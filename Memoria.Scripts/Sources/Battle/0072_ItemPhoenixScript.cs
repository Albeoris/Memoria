using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    ///  Phoenix Down, Phoenix Pinion
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemPhoenixScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0072;

        private readonly BattleCalculator _v;

        public ItemPhoenixScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.CanBeRevived())
                return;
            
            if (_v.Target.IsZombie)
            {
                if ((_v.Target.CurrentHp = (UInt32)(GameRandom.Next8() % 10)) == 0)
                    _v.Target.Kill();
            }
            else if (_v.Target.CheckIsPlayer())
            {
                if (_v.Target.IsUnderStatus(BattleStatus.Death))
                    _v.Target.CurrentHp = (UInt32)(1 + GameRandom.Next8() % 10);

                _v.TargetCommand.TryRemoveItemStatuses();
            }
        }

        public Single RateTarget()
        {
            if (!_v.Target.CanBeRevived())
                return 0;

            if (_v.Target.IsZombie)
            {
                Single result = BattleScriptStatusEstimate.RateStatus(BattleStatus.Death) * 0.1f;
                if (!_v.Target.IsPlayer)
                    result *= -1;
                return result;
            }

            if (!_v.Target.IsPlayer)
                return 0;

            BattleStatus playerStatus = _v.Target.CurrentStatus;
            BattleStatus removeStatus = _v.Command.ItemStatus;
            BattleStatus removedStatus = playerStatus & removeStatus;
            Int32 rating = BattleScriptStatusEstimate.RateStatuses(removedStatus);

            return -1 * rating;
        }
    }
}