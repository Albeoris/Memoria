using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using System;

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
			_v.NormalPhysicalParams();
			_v.Caster.EnemyTranceBonusAttack();
			_v.Caster.PhysicalPenaltyAndBonusAttack();
			_v.Target.PhysicalPenaltyAndBonusAttack();
			_v.BonusBackstabAndPenaltyLongDistance();
			_v.CalcHpDamage();
			RemoveItem();
		}

		private void RemoveItem()
		{
			RegularItem itemId = (RegularItem)_v.Command.HitRate;
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
