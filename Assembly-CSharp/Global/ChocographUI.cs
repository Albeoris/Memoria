using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Scenes;
using UnityEngine;
using Object = System.Object;

public class ChocographUI : UIScene
{
	public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
	{
		UIScene.SceneVoidDelegate afterShowAction = delegate
		{
			ButtonGroupState.SetPointerDepthToGroup(4, ChocographUI.ItemGroupButton);
			ButtonGroupState.SetPointerOffsetToGroup(new Vector2(30f, 0f), ChocographUI.ItemGroupButton);
			ButtonGroupState.SetPointerLimitRectToGroup(this.ChocographListPanel.GetComponent<UIWidget>(), (Single)this.chocographScrollList.ItemHeight, ChocographUI.ItemGroupButton);
			ButtonGroupState.SetScrollButtonToGroup(this.chocoboScrollButton, ChocographUI.ItemGroupButton);
			ButtonGroupState.RemoveCursorMemorize(ChocographUI.SubMenuGroupButton);
			ButtonGroupState.ActiveGroup = ChocographUI.SubMenuGroupButton;
		};
		if (afterFinished != null)
			afterShowAction += afterFinished;
		SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
		base.Show(afterShowAction);
		this.UpdateUserInterface();
		this.ClearLatestUI();
		this.GetEventData();
		this.DisplayChocoboAbilityInfo();
		this.DisplayInventoryInfo();
		this.DisplayChocographList();
		this.HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);
		if (ChocographUI.CurrentSelectedChocograph != -1)
		{
			ButtonGroupState.SetCursorStartSelect(this.chocographItemList[ChocographUI.CurrentSelectedChocograph].Self, ChocographUI.ItemGroupButton);
			this.DisplaySelected(ChocographUI.CurrentSelectedChocograph);
			this.SetCancelButton(true);
			this.chocographScrollList.ScrollToIndex(ChocographUI.CurrentSelectedChocograph);
		}
		else
		{
			ButtonGroupState.SetCursorStartSelect(this.chocographItemList[0].Self, ChocographUI.ItemGroupButton);
			this.SetCancelButton(false);
			this.chocographScrollList.ScrollToIndex(0);
		}
		this.chocoboScrollButton.DisplayScrollButton(false, false);
	}

	public void UpdateUserInterface()
	{
		if (!Configuration.Interface.IsEnabled)
			return;
		const Int32 originalLineCount = 5;
		const Single buttonOriginalHeight = 86f;
		const Single panelOriginalWidth = 658f;
		const Single panelOriginalHeight = originalLineCount * buttonOriginalHeight;
		Int32 linePerPage = Configuration.Interface.MenuChocographRowCount;
		Int32 lineHeight = (Int32)Math.Round(panelOriginalHeight / linePerPage);
		Single scaleFactor = lineHeight / buttonOriginalHeight;
		_chocographPanel.SubPanel.ChangeDims(1, linePerPage, panelOriginalWidth, lineHeight);
		foreach (GOButtonPrefab button in _chocographPanel.SubPanel.Table.Entries)
		{
			button.IconSprite.SetAnchor(target: button.Transform, relBottom: 0.5f, relTop: 0.5f, bottom: -36f * scaleFactor, top: 36f * scaleFactor, relLeft: 0.064f, relRight: 0.064f, right: 72f * scaleFactor);
			button.NameLabel.SetAnchor(target: button.Transform, relBottom: 0.081f, relTop: 0.919f, relLeft: 0.064f, left: 90f * scaleFactor, relRight: 1f);
			button.NameLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
		}
	}

	public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
	{
		UIScene.SceneVoidDelegate afterHideAction = delegate
		{
			MainMenuUI.UIControlPanel?.ExitMenu();
			PersistenSingleton<UIManager>.Instance.ChangeUIState(PersistenSingleton<UIManager>.Instance.HUDState);
		};
		if (afterFinished != null)
			afterHideAction += afterFinished;
		base.Hide(afterHideAction);
		this.RemoveCursorMemorize();
	}

	private void RemoveCursorMemorize()
	{
		ButtonGroupState.RemoveCursorMemorize(ChocographUI.ItemGroupButton);
	}

	public override Boolean OnKeyConfirm(GameObject go)
	{
		if (base.OnKeyConfirm(go))
		{
			if (ButtonGroupState.ActiveGroup == ChocographUI.SubMenuGroupButton)
			{
				this.currentMenu = this.GetSubMenuFromGameObject(go);
				if (this.currentMenu == ChocographUI.SubMenu.Select)
				{
					FF9Sfx.FF9SFX_Play(103);
					ButtonGroupState.RemoveCursorMemorize(ChocographUI.ItemGroupButton);
					ButtonGroupState.ActiveGroup = ChocographUI.ItemGroupButton;
					ButtonGroupState.SetSecondaryOnGroup(ChocographUI.SubMenuGroupButton);
					ButtonGroupState.HoldActiveStateOnGroup(ChocographUI.SubMenuGroupButton);
				}
				else if (this.currentMenu == ChocographUI.SubMenu.Cancel)
				{
					if (this.hasSelectedItem)
					{
						FF9Sfx.FF9SFX_Play(107);
						this.SetCancelButton(false);
						FF9StateSystem.Common.FF9.hintmap_id = 0;
						ChocographUI.CurrentSelectedChocograph = -1;
						this.hasSelectedItem = false;
						this.SelectedContentPanel.SetActive(false);
					}
					else
					{
						FF9Sfx.FF9SFX_Play(102);
					}
				}
			}
			else if (ButtonGroupState.ActiveGroup == ChocographUI.ItemGroupButton)
			{
				if (ButtonGroupState.ContainButtonInGroup(go, ChocographUI.ItemGroupButton))
				{
					Int32 id = go.GetComponent<ScrollItemKeyNavigation>().ID;
					if (!this.hasMap[id])
					{
						FF9Sfx.FF9SFX_Play(102);
						return true;
					}
					if (this.ability <= this.GetIconType(id))
					{
						FF9Sfx.FF9SFX_Play(102);
						return true;
					}
					FF9Sfx.FF9SFX_Play(107);
					ChocographUI.CurrentSelectedChocograph = id;
					FF9StateSystem.Common.FF9.hintmap_id = 1 + ChocographUI.CurrentSelectedChocograph;
					this.DisplaySelected(id);
					this.SetCancelButton(true);
				}
				else
				{
					this.OnSecondaryGroupClick(go);
				}
			}
		}
		return true;
	}

	public override Boolean OnKeyCancel(GameObject go)
	{
		if (base.OnKeyCancel(go))
		{
			if (ButtonGroupState.ActiveGroup == ChocographUI.SubMenuGroupButton)
			{
				FF9Sfx.FF9SFX_Play(101);
				this.Hide((UIScene.SceneVoidDelegate)null);
			}
			if (ButtonGroupState.ActiveGroup == ChocographUI.ItemGroupButton)
			{
				FF9Sfx.FF9SFX_Play(101);
				ButtonGroupState.ActiveGroup = ChocographUI.SubMenuGroupButton;
				this.HintContentPanel.SetActive(false);
			}
		}
		return true;
	}

	public override Boolean OnItemSelect(GameObject go)
	{
		if (base.OnItemSelect(go) && ButtonGroupState.ActiveGroup == ChocographUI.ItemGroupButton)
		{
			this.currentSelectItemIndex = go.GetComponent<ScrollItemKeyNavigation>().ID;
			if (this.hasMap[this.currentSelectItemIndex])
			{
				this.HintContentPanel.SetActive(true);
				this.DisplayHint(this.currentSelectItemIndex);
			}
			else
			{
				this.HintContentPanel.SetActive(false);
			}
			ButtonGroupState.SetCursorStartSelect(go, ChocographUI.ItemGroupButton);
		}
		return true;
	}

	private void OnSecondaryGroupClick(GameObject go)
	{
		ButtonGroupState.HoldActiveStateOnGroup(go, ChocographUI.SubMenuGroupButton);
		if (ButtonGroupState.ActiveGroup == ChocographUI.ItemGroupButton)
		{
			FF9Sfx.muteSfx = true;
			this.OnKeyCancel(this.chocographItemList[this.currentSelectItemIndex].Self);
			FF9Sfx.muteSfx = false;
			this.OnKeyConfirm(go);
		}
	}

	private Int32 GetIconType(Int32 id)
	{
		ChocographUI.Icon[] array = new ChocographUI.Icon[]
		{
			ChocographUI.Icon.Field,
			ChocographUI.Icon.Field,
			ChocographUI.Icon.Field,
			ChocographUI.Icon.Field,
			ChocographUI.Icon.Field,
			ChocographUI.Icon.Field,
			ChocographUI.Icon.Reef,
			ChocographUI.Icon.Reef,
			ChocographUI.Icon.Reef,
			ChocographUI.Icon.Reef,
			ChocographUI.Icon.Reef,
			ChocographUI.Icon.Reef,
			ChocographUI.Icon.Mountain,
			ChocographUI.Icon.Mountain,
			ChocographUI.Icon.Mountain,
			ChocographUI.Icon.Mountain,
			ChocographUI.Icon.Sea,
			ChocographUI.Icon.Sea,
			ChocographUI.Icon.Sea,
			ChocographUI.Icon.Sea,
			ChocographUI.Icon.Sky,
			ChocographUI.Icon.Sky,
			ChocographUI.Icon.Sky,
			ChocographUI.Icon.Sky
		};
		return (Int32)array[id];
	}

	public static void UpdateEquipedHintMap()
	{
		if (FF9StateSystem.Common.FF9.hintmap_id > ChocographUI.HintMapMax)
		{
			FF9StateSystem.Common.FF9.hintmap_id = 0;
			ChocographUI.CurrentSelectedChocograph = -1;
		}
		else
		{
			ChocographUI.CurrentSelectedChocograph = Math.Max(FF9StateSystem.Common.FF9.hintmap_id, 0) - 1;
		}
	}

	private void GetEventData()
	{
		this.hasMap.Clear();
		this.hasGem.Clear();
		ChocographUI.UpdateEquipedHintMap();
		Int32 chocographFound = 0;
		Int32 chocographOpened = 0;
		this.ability = FF9StateSystem.EventState.gEventGlobal[191];
		this.mapCount = 0;
		this.gemCount = 0;
		for (Int32 i = 0; i < 3; i++)
		{
			chocographFound |= (Int32)FF9StateSystem.EventState.gEventGlobal[187 + i] << i * 8;
			chocographOpened |= (Int32)FF9StateSystem.EventState.gEventGlobal[184 + i] << i * 8;
		}
		for (Int32 i = 0; i < ChocographUI.HintMapMax; i++)
		{
			this.hasMap.Add((chocographFound & 1 << i) != 0);
			if (this.hasMap[i])
				this.mapCount++;
			this.hasGem.Add((chocographOpened & 1 << i) != 0);
			if (this.hasGem[i])
				this.gemCount++;
		}
	}

	private void DisplayChocoboAbilityInfo()
	{
		for (Int32 i = 0; i < this.ability; i++)
		{
			this.ChocoboAbilityInfo.AbilitySpriteList[i].spriteName = this.GetAbilitySprite(i);
		}
	}

	private void DisplayInventoryInfo()
	{
		this.InventoryNumber.text = this.mapCount.ToString();
		this.LocationFoundNumber.text = this.gemCount.ToString();
	}

	private void ClearLatestUI()
	{
		this.HintContentPanel.SetActive(false);
		this.SelectedContentPanel.SetActive(false);
		this.ChocoboAbilityInfo.ClearAbility();
		this.HintAbilityRequired.ClearAbility();
		this.ClearChocoGraphList();
	}

	private void DisplayChocographList()
	{
		for (Int32 i = 0; i < this.chocographItemList.Count; i++)
		{
			ChocographUI.ChocographItem chocographItem = this.chocographItemList[i];
			if (!this.hasMap[i])
			{
				chocographItem.Button.Help.Enable = false;
			}
			else
			{
				Boolean flag = this.ability > this.GetIconType(i);
				chocographItem.Content.SetActive(true);
				String iconSprite = this.GetIconSprite((ChocographUI.Icon)((!this.hasGem[i]) ? ((ChocographUI.Icon)((!flag) ? ChocographUI.Icon.BoxDisableClose : ChocographUI.Icon.BoxClose)) : ((ChocographUI.Icon)((!flag) ? ChocographUI.Icon.BoxDisableOpen : ChocographUI.Icon.BoxOpen))));
				chocographItem.IconSprite.spriteName = iconSprite;
				chocographItem.ItemName.text = FF9TextTool.ChocoboUIText(i + (Int32)FF9TextTool.ChocographNameStartIndex);
				chocographItem.ItemName.color = ((!flag) ? FF9TextTool.Gray : FF9TextTool.White);
				chocographItem.Button.Help.Enable = true;
				chocographItem.Button.Help.TextKey = String.Empty;
				if (flag)
				{
					chocographItem.Button.Help.TextKey = FF9TextTool.ChocoboUIText(i + (Int32)FF9TextTool.ChocographHelpStartIndex);
				}
				else
				{
					chocographItem.Button.Help.TextKey = FF9TextTool.ChocoboUIText(5);
				}
			}
		}
	}

	private void ClearChocoGraphList()
	{
		for (Int32 i = 0; i < this.chocographItemList.Count; i++)
		{
			ChocographUI.ChocographItem chocographItem = this.chocographItemList[i];
			chocographItem.Content.SetActive(false);
		}
	}

	private String GetIconSprite(ChocographUI.Icon icon)
	{
		switch (icon)
		{
		case ChocographUI.Icon.BoxOpen:
			return "chocograph_box_open";
		case ChocographUI.Icon.BoxDisableOpen:
			return "chocograph_box_open_null";
		case ChocographUI.Icon.BoxClose:
			return "chocograph_box_close";
		case ChocographUI.Icon.BoxDisableClose:
			return "chocograph_box_close_null";
		default:
			return String.Empty;
		}
	}

	private void DisplayHint(Int32 currentOnSelectItemIndex)
	{
		this.HintAbilityRequired.ClearAbility();
		this.HintMap.spriteName = "chocograph_map_" + currentOnSelectItemIndex.ToString("D" + 2);
		this.HintText.text = FF9TextTool.ChocoboUIText(currentOnSelectItemIndex + (Int32)FF9TextTool.ChocographDetailStartIndex);
		Int32 iconType = this.GetIconType(currentOnSelectItemIndex);
		for (Int32 i = 0; i <= iconType; i++)
		{
			String abilitySprite = this.GetAbilitySprite((Int32)((this.ability <= i) ? 5 : i));
			this.HintAbilityRequired.AbilitySpriteList[i].spriteName = abilitySprite;
		}
	}

	private String GetAbilitySprite(Int32 iconIndex)
	{
		switch (iconIndex)
		{
		case 0:
			return "chocograph_icon_field";
		case 1:
			return "chocograph_icon_reef";
		case 2:
			return "chocograph_icon_mt";
		case 3:
			return "chocograph_icon_sea";
		case 4:
			return "chocograph_icon_sky";
		case 5:
			return "chocograph_icon_unknown_null";
		default:
			return String.Empty;
		}
	}

	private void SetCancelButton(Boolean isEnable)
	{
		if (isEnable)
		{
			this.CancelSubMenu.GetChild(1).GetComponent<UILabel>().color = FF9TextTool.White;
			ButtonGroupState.SetButtonAnimation(this.CancelSubMenu, true);
		}
		else
		{
			this.CancelSubMenu.GetChild(1).GetComponent<UILabel>().color = FF9TextTool.Gray;
			ButtonGroupState.SetButtonAnimation(this.CancelSubMenu, false);
		}
	}

	private ChocographUI.SubMenu GetSubMenuFromGameObject(GameObject go)
	{
		if (go == this.SelectSubMenu)
		{
			return ChocographUI.SubMenu.Select;
		}
		if (go == this.CancelSubMenu)
		{
			return ChocographUI.SubMenu.Cancel;
		}
		return ChocographUI.SubMenu.None;
	}

	private void DisplaySelected(Int32 currentSelectedItemIndex)
	{
		this.hasSelectedItem = true;
		this.SelectedContentPanel.SetActive(true);
		this.SelectedMap.spriteName = "chocograph_map_" + currentSelectedItemIndex.ToString("0#");
		this.SelectedItemIcon.spriteName = this.chocographItemList[currentSelectedItemIndex].IconSprite.spriteName;
		this.SelectedItemLabel.text = this.chocographItemList[currentSelectedItemIndex].ItemName.text;
	}

	private void Awake()
	{
		base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
		UIEventListener.Get(this.SelectSubMenu).onClick += this.onClick;
		UIEventListener.Get(this.CancelSubMenu).onClick += this.onClick;
		foreach (Transform t in this.ChocographListPanel.GetChild(1).GetChild(0).transform)
		{
			this.chocographItemList.Add(new ChocographUI.ChocographItem(t.gameObject));
			UIEventListener.Get(t.gameObject).onClick += this.onClick;
		}
		this.chocoboScrollButton = this.ChocographListPanel.GetChild(0).GetComponent<ScrollButton>();
		this.chocographScrollList = this.ChocographListPanel.GetChild(1).GetComponent<SnapDragScrollView>();
		this._chocographPanel = new GOScrollablePanel(this.ChocographListPanel);
		this.ChocoboAbilityInfo = new ChocographUI.ChocoboAbility(this.ChocoboAbilityInfoPanel);
		this.ChocoboAbilityInfo.ClearAbility();
		this.HintAbilityRequired = new ChocographUI.ChocoboAbility(this.HintAbilityRequiredPanel);
		this.HintAbilityRequired.ClearAbility();
	}

	private const Int32 HintMapMax = 24;

	public static Int32 CurrentSelectedChocograph = -1;

	public GameObject ScreenFadeGameObject;
	public GameObject HelpDespLabelGameObject;
	public GameObject ChocographListPanel;
	public GameObject SelectedContentPanel;
	public GameObject HintContentPanel;
	public GameObject ChocoboAbilityInfoPanel;

	public UILabel InventoryNumber;
	public UILabel LocationFoundNumber;

	public UISprite SelectedMap;
	public UILabel SelectedItemLabel;
	public UISprite SelectedItemIcon;

	public UISprite HintMap;
	public UILabel HintText;
	public GameObject HintAbilityRequiredPanel;

	public GameObject SelectSubMenu;
	public GameObject CancelSubMenu;

	private ChocographUI.SubMenu currentMenu;

	private Int32 currentSelectItemIndex;

	private static String SubMenuGroupButton = "Chocograph.SubMenu";
	private static String ItemGroupButton = "Chocograph.Item";

	private ChocographUI.ChocoboAbility ChocoboAbilityInfo;
	private ChocographUI.ChocoboAbility HintAbilityRequired;

	private List<ChocographUI.ChocographItem> chocographItemList = new List<ChocographUI.ChocographItem>();

	private GOScrollablePanel _chocographPanel;
	private SnapDragScrollView chocographScrollList;
	private ScrollButton chocoboScrollButton;

	private Boolean hasSelectedItem;

	private Int32 ability = 1;

	private List<Boolean> hasMap = new List<Boolean>();
	private List<Boolean> hasGem = new List<Boolean>();

	private Int32 mapCount;
	private Int32 gemCount;

	public class ChocoboAbility
	{
		public ChocoboAbility(GameObject go)
		{
			this.AbilitySpriteList = new UISprite[]
			{
				go.GetChild(0).GetComponent<UISprite>(),
				go.GetChild(1).GetComponent<UISprite>(),
				go.GetChild(2).GetComponent<UISprite>(),
				go.GetChild(3).GetComponent<UISprite>(),
				go.GetChild(4).GetComponent<UISprite>()
			};
		}

		public void ClearAbility()
		{
			foreach (UISprite sprite in this.AbilitySpriteList)
				sprite.spriteName = String.Empty;
		}

		public UISprite[] AbilitySpriteList;
	}

	public class ChocographItem
	{
		public ChocographItem(GameObject go)
		{
			this.Self = go;
			this.Content = go.GetChild(0);
			this.Button = go.GetComponent<ButtonGroupState>();
			this.IconSprite = go.GetChild(0).GetChild(0).GetComponent<UISprite>();
			this.ItemName = go.GetChild(0).GetChild(1).GetComponent<UILabel>();
		}

		public GameObject Self;
		public GameObject Content;
		public ButtonGroupState Button;
		public UISprite IconSprite;
		public UILabel ItemName;
	}

	private enum SubMenu
	{
		Select,
		Cancel,
		None
	}

	private enum Icon
	{
		Field,
		Reef,
		Mountain,
		Sea,
		Sky,
		Unknown,
		BoxOpen,
		BoxDisableOpen,
		BoxClose,
		BoxDisableClose
	}
}
