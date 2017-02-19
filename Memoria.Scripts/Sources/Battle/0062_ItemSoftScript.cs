using System;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Soft (Item)
    /// </summary>
    [BattleScript(Id)]
    public sealed class ItemSoftScript : IBattleScript
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
                _v.TargetCommand.TryRemoveItemStatuses();
                return;
            }

            if (_v.Target.CanBeAttacked())
            {
                _v.Target.Kill();
                UiState.SetBattleFollowFormatMessage(BattleMesages.BecameTooSoftToLive);
            }
        }
    }
}