using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Assets;

namespace Memoria.Scenes
{
	public class MenuUIControlPanel : ControlPanel
	{
		public const Int32 ITEM_ROW_MIN = 6;
		public const Int32 ITEM_ROW_MAX = 12;
		public const Int32 ABILITY_ROW_MIN = 4;
		public const Int32 ABILITY_ROW_MAX = 10;
		public const Int32 EQUIP_ROW_MIN = 4;
		public const Int32 EQUIP_ROW_MAX = 8;
		public const Int32 CHOCOGRAPH_ROW_MIN = 4;
		public const Int32 CHOCOGRAPH_ROW_MAX = 8;

		public UIScene Scene;

		public MenuUIControlPanel()
			: base(PersistenSingleton<UIManager>.Instance.MainMenuScene.transform, Localization.Get("ConfigCaption"))
		{
			Scene = null;

			AddSimpleLabel(Localization.Get("Item"), NGUIText.Alignment.Left, 1);
			controlItemRowCount = AddRollOption(ControlRoll<Int32>.GetNonEmptyIndexList(ITEM_ROW_MAX - ITEM_ROW_MIN + 1), sel => $"{sel + ITEM_ROW_MIN} rows", SetItemRowCount);
			controlItemRowCount.Selection = Configuration.Interface.MenuItemRowCount - ITEM_ROW_MIN;
			PanelAddRow();
			AddSimpleLabel(Localization.Get("Ability"), NGUIText.Alignment.Left, 1);
			controlAbilityRowCount = AddRollOption(ControlRoll<Int32>.GetNonEmptyIndexList(ABILITY_ROW_MAX - ABILITY_ROW_MIN + 1), sel => $"{sel + ABILITY_ROW_MIN} rows", SetAbilityRowCount);
			controlAbilityRowCount.Selection = Configuration.Interface.MenuAbilityRowCount - ABILITY_ROW_MIN;
			PanelAddRow();
			AddSimpleLabel(Localization.Get("Equip"), NGUIText.Alignment.Left, 1);
			controlShopItemRowCount = AddRollOption(ControlRoll<Int32>.GetNonEmptyIndexList(EQUIP_ROW_MAX - EQUIP_ROW_MIN + 1), sel => $"{sel + EQUIP_ROW_MIN} rows", SetEquipRowCount);
			controlShopItemRowCount.Selection = Configuration.Interface.MenuEquipRowCount - EQUIP_ROW_MIN;
			PanelAddRow();
			AddSimpleLabel(Localization.Get("Chocograph"), NGUIText.Alignment.Left, 1);
			controlShopItemRowCount = AddRollOption(ControlRoll<Int32>.GetNonEmptyIndexList(CHOCOGRAPH_ROW_MAX - CHOCOGRAPH_ROW_MIN + 1), sel => $"{sel + CHOCOGRAPH_ROW_MIN} rows", SetChocographRowCount);
			controlShopItemRowCount.Selection = Configuration.Interface.MenuChocographRowCount - CHOCOGRAPH_ROW_MIN;

			EndInitialization(UIWidget.Pivot.Right);
			onHide = () =>
			{
				Configuration.Interface.SaveMenuValues();
				(Scene as AbilityUI)?.UpdateUserInterface();
				(Scene as ChocographUI)?.UpdateUserInterface();
				(Scene as EquipUI)?.UpdateUserInterface();
				(Scene as ItemUI)?.UpdateUserInterface();
				(Scene as ShopUI)?.UpdateUserInterface();
				Scene?.gameObject.SetActive(true);
			};
			onShow = () => Scene?.gameObject.SetActive(false);
		}

		public void ExitMenu()
		{
			if (Show)
			{
				Scene = null;
				Show = false;
			}
		}

		private void SetItemRowCount(Int32 count)
		{
			Configuration.Interface.MenuItemRowCount = count + ITEM_ROW_MIN;
		}

		private void SetAbilityRowCount(Int32 count)
		{
			Configuration.Interface.MenuAbilityRowCount = count + ABILITY_ROW_MIN;
		}

		private void SetEquipRowCount(Int32 count)
		{
			Configuration.Interface.MenuEquipRowCount = count + EQUIP_ROW_MIN;
		}

		private void SetChocographRowCount(Int32 count)
		{
			Configuration.Interface.MenuChocographRowCount = count + CHOCOGRAPH_ROW_MIN;
		}

		private ControlRoll<Int32> controlItemRowCount;
		private ControlRoll<Int32> controlKeyItemRowCount;
		private ControlRoll<Int32> controlShopItemRowCount;
		private ControlRoll<Int32> controlAbilityRowCount;
		private ControlRoll<Int32> controlSupportRowCount;
	}
}
