using System;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;

namespace Memoria.Scripts.Battle
{
    /// <summary>
    /// Mug (enemy)
    /// </summary>
    [BattleScript(Id)]
    public sealed class EnemyMugScript : IBattleScript
    {
        public const Int32 Id = 0102;

        private readonly BattleCalculator _v;

        public EnemyMugScript(BattleCalculator v)
        {
            _v = v;
        }

        public void Perform()
        {
            _v.NormalPhisicalParams();
            _v.Caster.PhysicalPenaltyAndBonusAttack();
            _v.Target.GambleDefence();
            _v.Target.PhysicalPenaltyAndBonusAttack();
            _v.BonusBackstabAndPenaltyLongDistance();
            _v.TargetCommand.CalcHpDamage();
            RemoveItem();
        }

        private void RemoveItem()
        {
            Byte itemId = (Byte)_v.Command.HitRate;
            if (ff9item.FF9Item_GetCount(itemId) == 0)
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.CouldNotStealAnything);
            }
            else
            {
                BattleItem.RemoveFromInventory(itemId);
                UiState.SetBattleFollowFormatMessage(BattleMesages.WasStolen, FF9TextTool.ItemName(itemId));
            }
        }
    }
}