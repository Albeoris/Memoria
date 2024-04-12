using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
	/// <summary>
	/// Steal (enemy)
	/// </summary>
	[BattleScript(Id)]
	public sealed class EnemyStealScript : IBattleScript
	{
		public const Int32 Id = 0101;

		private readonly BattleCalculator _v;

		public EnemyStealScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			_v.PhysicalAccuracy();
			if (_v.TryPhysicalHit())
				RemoveItem();
			else
				UiState.SetBattleFollowFormatMessage(BattleMesages.CouldNotStealAnything);
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
