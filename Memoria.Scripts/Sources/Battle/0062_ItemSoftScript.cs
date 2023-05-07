using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Soft (Item)
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemSoftScript : IBattleScript, IEstimateBattleScript
    {
        public const Int32 Id = 0062;

        private readonly BattleCalculator _v;

        public ItemSoftScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            if (!_v.Target.HasCategory(EnemyCategory.Stone))
            {
                _v.TryRemoveItemStatuses();
                return;
            }

            if (_v.Target.CanBeAttacked())
            {
                _v.Target.Kill(_v.Caster);
                UiState.SetBattleFollowFormatMessage(BattleMesages.BecameTooSoftToLive);
            }
        }

        public Single RateTarget()
        {
            if (_v.Target.HasCategory(EnemyCategory.Stone))
            {
                if (_v.Target.CanBeAttacked())
                    return -1 * BattleScriptStatusEstimate.RateStatus(BattleStatus.Death);

                return 0;
            }

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