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
	public class BattleUIControlPanel : ControlPanel
	{
		public const Single POSITION_MIN_X = -1000f;
		public const Single POSITION_MAX_X = 1000f;
		public const Single POSITION_MIN_Y = -500f;
		public const Single POSITION_MAX_Y = 500f;
		public const Single MENU_MIN_WIDTH = 100f;
		public const Single MENU_MAX_WIDTH = 800f;
		public const Single MENU_MIN_HEIGHT = 50f;
		public const Single MENU_MAX_HEIGHT = 400f;
		public const Single DETAIL_MIN_WIDTH = 150f;
		public const Single DETAIL_MAX_WIDTH = 1000f;
		public const Single DETAIL_MIN_HEIGHT = 50f;
		public const Single DETAIL_MAX_HEIGHT = 400f;

		public readonly BattleHUD Scene;

		public BattleUIControlPanel(BattleHUD hud)
			: base(hud.transform, Localization.Get("ConfigCaption"))
		{
			Scene = hud;

			//controlEnabled = AddToggleOption("Use Custom Interface", Configuration.Interface.IsEnabled, SetEnabled);
			controlPSXMenu = AddToggleOption("Legacy Interface", Configuration.Interface.PSXBattleMenu, SetPSXBattleMenu);
			controlRowCount = AddRollOption(ControlRoll<Int32>.GetNonEmptyIndexList(3), sel => $"{sel + 4} rows", SetRowCount);
			controlRowCount.Selection = Configuration.Interface.BattleRowCount - 4;
			controlColumnCount = AddRollOption(ControlRoll<Int32>.GetNonEmptyIndexList(3), sel => $"{sel + 1} column{(sel > 0 ? "s" : "")}", SetColumnCount);
			controlColumnCount.Selection = Configuration.Interface.BattleColumnCount - 1;
			PanelAddRow();
			controlMenuX = AddSlider("Menu X Position", Configuration.Interface.BattleMenuPos.x, val => ControlSlider.LinearScaleIn(val, POSITION_MIN_X, POSITION_MAX_X), t => ControlSlider.LinearScaleOut(t, POSITION_MIN_X, POSITION_MAX_X), SetMenuX);
			controlDetailX = AddSlider("Detail X Position", Configuration.Interface.BattleDetailPos.x, val => ControlSlider.LinearScaleIn(val, POSITION_MIN_X, POSITION_MAX_X), t => ControlSlider.LinearScaleOut(t, POSITION_MIN_X, POSITION_MAX_X), SetDetailX);
			PanelAddRow();
			controlMenuY = AddSlider("Menu Y Position", Configuration.Interface.BattleMenuPos.y, val => ControlSlider.LinearScaleIn(val, POSITION_MIN_Y, POSITION_MAX_Y), t => ControlSlider.LinearScaleOut(t, POSITION_MIN_Y, POSITION_MAX_Y), SetMenuY);
			controlDetailY = AddSlider("Detail Y Position", Configuration.Interface.BattleDetailPos.y, val => ControlSlider.LinearScaleIn(val, POSITION_MIN_Y, POSITION_MAX_Y), t => ControlSlider.LinearScaleOut(t, POSITION_MIN_Y, POSITION_MAX_Y), SetDetailY);
			PanelAddRow();
			controlMenuWidth = AddSlider("Menu Width", Configuration.Interface.BattleMenuSize.x, val => ControlSlider.LinearScaleIn(val, MENU_MIN_WIDTH, MENU_MAX_WIDTH), t => ControlSlider.LinearScaleOut(t, MENU_MIN_WIDTH, MENU_MAX_WIDTH), SetMenuWidth);
			controlDetailWidth = AddSlider("Detail Width", Configuration.Interface.BattleDetailSize.x, val => ControlSlider.LinearScaleIn(val, DETAIL_MIN_WIDTH, DETAIL_MAX_WIDTH), t => ControlSlider.LinearScaleOut(t, DETAIL_MIN_WIDTH, DETAIL_MAX_WIDTH), SetDetailWidth);
			PanelAddRow();
			controlMenuHeight = AddSlider("Menu Height", Configuration.Interface.BattleMenuSize.y, val => ControlSlider.LinearScaleIn(val, MENU_MIN_HEIGHT, MENU_MAX_HEIGHT), t => ControlSlider.LinearScaleOut(t, MENU_MIN_HEIGHT, MENU_MAX_HEIGHT), SetMenuHeight);
			controlDetailHeight = AddSlider("Detail Height", Configuration.Interface.BattleDetailSize.y, val => ControlSlider.LinearScaleIn(val, DETAIL_MIN_HEIGHT, DETAIL_MAX_HEIGHT), t => ControlSlider.LinearScaleOut(t, DETAIL_MIN_HEIGHT, DETAIL_MAX_HEIGHT), SetDetailHeight);
			EndInitialization(UIWidget.Pivot.Bottom);

			SetEnabled(true);
			onHide = Configuration.Interface.SaveBattleValues;
		}

		private void SetEnabled(Boolean enabled)
		{
			Configuration.Interface.IsEnabled = enabled;
			controlPSXMenu.IsEnabled = enabled;
			controlRowCount.IsEnabled = enabled;
			controlColumnCount.IsEnabled = enabled;
			controlMenuX.IsEnabled = enabled;
			controlMenuY.IsEnabled = enabled;
			controlMenuWidth.IsEnabled = enabled;
			controlMenuHeight.IsEnabled = enabled;
			controlDetailX.IsEnabled = enabled;
			controlDetailY.IsEnabled = enabled;
			controlDetailWidth.IsEnabled = enabled;
			controlDetailHeight.IsEnabled = enabled;
			Scene.UpdateUserInterface(true);
		}

		private void SetPSXBattleMenu(Boolean psx)
		{
			Configuration.Interface.PSXBattleMenu = psx;
			Scene.UpdateUserInterface(true);
		}

		private void SetRowCount(Int32 count)
		{
			Configuration.Interface.BattleRowCount = count + 4;
			Scene.UpdateUserInterface(true);
		}

		private void SetColumnCount(Int32 count)
		{
			Configuration.Interface.BattleColumnCount = count + 1;
			Scene.UpdateUserInterface(true);
		}

		private void SetMenuX(Single displayX)
		{
			Configuration.Interface.BattleMenuPos = new Vector2(displayX, Configuration.Interface.BattleMenuPos.y);
			Scene.UpdateUserInterface(true);
		}

		private void SetMenuY(Single displayY)
		{
			Configuration.Interface.BattleMenuPos = new Vector2(Configuration.Interface.BattleMenuPos.x, displayY);
			Scene.UpdateUserInterface(true);
		}

		private void SetMenuWidth(Single displayWidth)
		{
			Configuration.Interface.BattleMenuSize = new Vector2(displayWidth, Configuration.Interface.BattleMenuSize.y);
			Scene.UpdateUserInterface(true);
		}

		private void SetMenuHeight(Single displayHeight)
		{
			Configuration.Interface.BattleMenuSize = new Vector2(Configuration.Interface.BattleMenuSize.x, displayHeight);
			Scene.UpdateUserInterface(true);
		}

		private void SetDetailX(Single displayX)
		{
			Configuration.Interface.BattleDetailPos = new Vector2(displayX, Configuration.Interface.BattleDetailPos.y);
			Scene.UpdateUserInterface(true);
		}

		private void SetDetailY(Single displayY)
		{
			Configuration.Interface.BattleDetailPos = new Vector2(Configuration.Interface.BattleDetailPos.x, displayY);
			Scene.UpdateUserInterface(true);
		}

		private void SetDetailWidth(Single displayWidth)
		{
			Configuration.Interface.BattleDetailSize = new Vector2(displayWidth, Configuration.Interface.BattleDetailSize.y);
			Scene.UpdateUserInterface(true);
		}

		private void SetDetailHeight(Single displayHeight)
		{
			Configuration.Interface.BattleDetailSize = new Vector2(Configuration.Interface.BattleDetailSize.x, displayHeight);
			Scene.UpdateUserInterface(true);
		}

		private ControlToggle controlEnabled;
		private ControlToggle controlPSXMenu;
		private ControlRoll<Int32> controlRowCount;
		private ControlRoll<Int32> controlColumnCount;
		private ControlSlider controlMenuX;
		private ControlSlider controlMenuY;
		private ControlSlider controlMenuWidth;
		private ControlSlider controlMenuHeight;
		private ControlSlider controlDetailX;
		private ControlSlider controlDetailY;
		private ControlSlider controlDetailWidth;
		private ControlSlider controlDetailHeight;
	}
}
