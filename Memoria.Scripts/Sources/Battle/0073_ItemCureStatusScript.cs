using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Echo Screen, Antidote, Eye Drops, Magic Tag, Vaccine, Remedy, Annoyntment, Gysahl Greens
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemCureStatusScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0073;

        private readonly BattleCalculator _v;

        public ItemCureStatusScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.TargetCommand.TryRemoveItemStatuses();
        }

        public Single RateTarget()
        {
            BattleStatus playerStatus = _v.Target.CurrentStatus;
            BattleStatus removeStatus = _v.Command.ItemStatus;
            BattleStatus removedStatus = playerStatus & removeStatus;
            Int32 rating = BattleScriptStatusEstimate.RateStatuses(removedStatus);

            if (_v.Target.IsPlayer)
                return -1 * rating;

            return rating;
        }
    }
}