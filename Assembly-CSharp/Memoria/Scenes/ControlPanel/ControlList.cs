using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Scenes
{
	public class ControlList<T>
	{
		// List passed to this control should never be empty
		public readonly ControlHitBox ListOpener;
		public readonly RecycleListPopulator Populator;
		public readonly ButtonGroupState ButtonGroup;
		public readonly Boolean IsMultiSelection;

		private List<T> _objectList;
		private Func<T, String> LabelPicker;
		private Func<T, Boolean> ValidChecker;
		private Func<T, Boolean> EnabledChecker;

		public List<T> ObjectList
		{
			get => _objectList;
			set
			{
				_objectList = value;
				UpdateList();
			}
		}

		public ControlList(ControlPanel control, Int32 panelIndex,
			List<T> list, String name, Boolean isMultiSelection,
			Func<T, String> labelPicker, Func<T, Boolean> validChecker, Func<T, Boolean> enabledChecker, Action<T> selectAction)
		{
			IsMultiSelection = isMultiSelection;
			_objectList = list;
			LabelPicker = labelPicker;
			ValidChecker = validChecker;
			EnabledChecker = enabledChecker;
			UIWidget panel = control.GetPanel(panelIndex);
			Int32 listPanelIndex = control.CreateSubPanel(name, panelIndex);
			Populator = control.CreateUIElementForPanel<RecycleListPopulator>(control.GetPanel(listPanelIndex));
			ButtonGroup = Populator.itemPrefab.gameObject.GetComponent<ButtonGroupState>();
			ButtonGroup.GroupName = $"ControlList.{name}";
			ButtonGroup.Help = new ButtonGroupState.HelpDetail() { Enable = false };
			ButtonGroupState.DisablePointerCursor.Add(ButtonGroup.GroupName);
			if (IsMultiSelection)
			{
				SetupToggleList(0, selectAction);
			}
			else
			{
				Action<T> listAction = obj => control.SetActivePanel(false, listPanelIndex);
				if (selectAction != null)
					listAction += selectAction;
				SetupSelectList(0, listAction);
			}
			ListOpener = control.AddHitBoxOption(name, () =>
			{
				ButtonGroupState.ActiveGroup = ButtonGroup.GroupName;
				control.SetActivePanel(true, listPanelIndex);
			}, panelIndex);
		}

		private void SetupSelectList(Int32 selectIndex, Action<T> onClick)
		{
			List<ListDataTypeBase> listData = ListDataFromObjectList();
			RecycleListPopulator.RecycleListItemClick clickFunc = obj =>
			{
				Int32 index = obj.GetComponent<RecycleListItem>().ItemDataIndex;
				SelectListData selectData = listData[index] as SelectListData;
				if (!selectData.Valid)
					return;
				if (onClick != null)
					onClick(_objectList[index]);
				UpdateList();
			};
			Populator.PopulateListItemWithData = SelectListPopulator;
			Populator.OnRecycleListItemClick += clickFunc;
			Populator.InitTableView(listData, selectIndex);
		}

		private void SetupToggleList(Int32 selectIndex, Action<T> onClick)
		{
			List<ListDataTypeBase> listData = ListDataFromObjectList();
			RecycleListPopulator.RecycleListItemClick clickFunc = obj =>
			{
				Int32 index = obj.GetComponent<RecycleListItem>().ItemDataIndex;
				ToggleListData toggleData = listData[index] as ToggleListData;
				if (!toggleData.Valid)
					return;
				toggleData.IsToggled = !toggleData.IsToggled;
				if (onClick != null)
					onClick(_objectList[index]);
				UpdateList();
			};
			Populator.PopulateListItemWithData = ToggleListPopulator;
			Populator.OnRecycleListItemClick += clickFunc;
			Populator.InitTableView(listData, selectIndex);
		}

		private void SelectListPopulator(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
		{
			SelectListData selectData = (SelectListData)data;
			ItemListDetailWithIconHUD button = new ItemListDetailWithIconHUD(item.gameObject, true);
			UIWidget widget = item.GetComponent<UIWidget>();
			button.Content.SetActive(true);
			ButtonGroupState.SetButtonAnimation(button.Self, true);
			button.NameLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
			button.NameLabel.alignment = NGUIText.Alignment.Left;
			button.NameLabel.text = selectData.Label;
			button.NameLabel.color = selectData.Valid ? FF9TextTool.White : FF9TextTool.Gray;
			button.NumberLabel.text = String.Empty;
			button.IconSpriteAnimation.Pause();
			button.IconSprite.enabled = false;
			widget.pivot = UIWidget.Pivot.Left;
			widget.leftAnchor.Set(item.parent.parent, 0f, 0);
			widget.rightAnchor.Set(item.parent.parent, 1f, 0);
			button.NameLabel.leftAnchor.Set(0f, 0);
			button.NameLabel.rightAnchor.Set(button.NameLabel.leftAnchor.target, 0f, 500);
		}

		private void ToggleListPopulator(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
		{
			ToggleListData toggleData = (ToggleListData)data;
			ItemListDetailWithIconHUD button = new ItemListDetailWithIconHUD(item.gameObject, true);
			UIWidget widget = item.GetComponent<UIWidget>();
			button.Content.SetActive(true);
			ButtonGroupState.SetButtonAnimation(button.Self, true);
			button.NameLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
			button.NameLabel.alignment = NGUIText.Alignment.Left;
			button.NameLabel.text = toggleData.Label;
			button.NameLabel.color = toggleData.Valid ? FF9TextTool.White : FF9TextTool.Gray;
			button.NumberLabel.text = String.Empty;
			if (toggleData.IsToggled)
			{
				button.IconSprite.atlas = FF9UIDataTool.IconAtlas;
				button.IconSprite.spriteName = "skill_stone_gem";
				//button.IconSpriteAnimation.namePrefix = "skill_stone_gem_";
				//button.IconSpriteAnimation.ResetToBeginning();
			}
			else
			{
				button.IconSprite.atlas = FF9UIDataTool.BlueAtlas;
				button.IconSprite.spriteName = "skill_stone_gem_slot";
				//button.IconSpriteAnimation.Pause();
			}
			button.IconSpriteAnimation.Pause();
			widget.pivot = UIWidget.Pivot.Left;
			widget.leftAnchor.Set(item.parent.parent, 0f, 0);
			widget.rightAnchor.Set(item.parent.parent, 1f, 0);
		}

		private void UpdateList()
		{
			Populator.SetOriginalData(ListDataFromObjectList(), false);
		}

		private List<ListDataTypeBase> ListDataFromObjectList()
		{
			List<ListDataTypeBase> listData = new List<ListDataTypeBase>();
			foreach (T obj in _objectList)
			{
				SelectListData toggleData = IsMultiSelection ? new ToggleListData() : new SelectListData();
				toggleData.Valid = ValidChecker(obj);
				toggleData.Label = LabelPicker(obj);
				if (IsMultiSelection)
					(toggleData as ToggleListData).IsToggled = EnabledChecker(obj);
				listData.Add(toggleData);
			}
			return listData;
		}

		private class SelectListData : ListDataTypeBase
		{
			public Boolean Valid { get; set; }
			public String Label { get; set; }
		}

		private class ToggleListData : SelectListData
		{
			public Boolean IsToggled { get; set; }
		}
	}
}
