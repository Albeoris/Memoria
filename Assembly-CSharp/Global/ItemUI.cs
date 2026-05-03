using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Scenes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable RedundantExplicitArraySize
// ReSharper disable EmptyConstructor

#pragma warning disable 414
#pragma warning disable 649

public class ItemUI : UIScene
{
    public const String SubMenuGroupButton = "Item.SubMenu";
    public const String ItemGroupButton = "Item.Item";
    public const String KeyItemGroupButton = "Item.KeyItem";
    public const String TargetGroupButton = "Item.Target";
    public const String ArrangeMenuGroupButton = "Item.ArrangeMenu";
    public const String ItemArrangeGroupButton = "Item.Arrange";

    public GameObject TransitionGroup;
    public GameObject UseSubMenu;
    public GameObject ArrangeSubMenu;
    public GameObject KeySubMenu;
    public GameObject HelpDespLabelGameObject;
    public GameObject ItemListPanel;
    public GameObject KeyItemListPanel;
    public GameObject TargetListPanel;
    public GameObject KeyItemDetailPanel;
    public GameObject ArrangeDialog;
    public GameObject ScreenFadeGameObject;

    private const Int32 FF9FITEM_RARE_NONE = Byte.MaxValue;
    private const Int32 FF9FITEM_CHOCOBO_NONE = 0;
    private const Int32 FF9FITEM_EVENT_VEGETABLE = 181;
    private const Int32 FF9FITEM_EVENT_KIND_CHOCOBO = 191;
    private const Single TargetPositionXOffset = 338f;

    private readonly List<RegularItem> _itemIdList;
    private readonly List<RegularItem> _usedItemIdList;
    //private List<Int32> _ketIdList;
    private readonly List<Int32> _keyItemIdList;
    private List<CharacterDetailHUD> _targetHudList;
    private RecycleListPopulator _itemScrollList;
    private RecycleListPopulator _keyItemScrollList;
    private UILabel _keyItemDetailName;
    private UILabel _keyItemDetailDescription;
    //private GameObject _importantItemHitArea;
    private HonoTweenPosition _targetTransition;
    private HonoTweenPosition _keyItemSkinTransition;
    private HonoTweenClipping _arrangeTransition;
    private SubMenu _currentMenu;
    private Boolean _isShowingKeyItemDesp;
    private Boolean _fastSwitch;
    private Boolean _switchingItem;
    private Int32 _currentArrangeMode;
    private Int32 _currentArrangeItemIndex;
    private Int32 _currentItemIndex;
    private Dialog _chocoboDialog;
    private Int32 _defaultSkinLabelSpacingY;
    private GOScrollablePanel _itemPanel;
    private GOScrollablePanel _keyItemPanel;
    [NonSerialized]
    private ArrangeDialogHUD _arrangeDialog;
    [NonSerialized]
    private GOMenuBackground _background;

    [NonSerialized]
    private Boolean _multiTarget;
    [NonSerialized]
    private Boolean _canMultiTarget;

    public ItemUI()
    {
        _itemIdList = new List<RegularItem>();
        _usedItemIdList = new List<RegularItem>();
        //this._ketIdList = new List<Int32>();
        _keyItemIdList = new List<Int32>();
        _targetHudList = new List<CharacterDetailHUD>();
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate afterShowAction = () =>
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.SubMenuPanel.SetActive(false);
            ButtonGroupState.SetPointerDepthToGroup(4, ItemGroupButton);
            ButtonGroupState.SetPointerDepthToGroup(7, TargetGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(54f, 0.0f), ItemGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(54f, 0.0f), KeyItemGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(-20f, 10f), ArrangeMenuGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(48f, -6f), ItemArrangeGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(_itemScrollList.ScrollButton, ItemGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(_itemScrollList.ScrollButton, ItemArrangeGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(_keyItemScrollList.ScrollButton, KeyItemGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(ItemListPanel.GetComponent<UIWidget>(), _itemScrollList.cellHeight, ItemGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(ItemListPanel.GetComponent<UIWidget>(), _itemScrollList.cellHeight, ItemArrangeGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(KeyItemListPanel.GetComponent<UIWidget>(), _keyItemScrollList.cellHeight, KeyItemGroupButton);
            ButtonGroupState.ActiveGroup = SubMenuGroupButton;
            StartCoroutine(Show_delay());
            afterFinished?.Invoke();
        };

        _fastSwitch = false;
        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(afterShowAction);
        UpdateUserInterface();
        ItemListPanel.SetActive(true);
        KeyItemListPanel.SetActive(false);
        DisplayItem();
        HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);
        _itemScrollList.ScrollButton.DisplayScrollButton(false, false);
        _keyItemScrollList.ScrollButton.DisplayScrollButton(false, false);
        UpdateUserInterface();
    }

    public void UpdateUserInterface()
    {
        if (!Configuration.Interface.IsEnabled)
            return;
        _itemPanel.ScrollButton.Panel.alpha = 0.5f;
        _keyItemPanel.ScrollButton.Panel.alpha = 0.5f;
        const Int32 originalLineCount = 8;
        const Int32 originalColumnCount = 2;
        const Single buttonOriginalHeight = 98f;
        const Single panelOriginalWidth = 1490f;
        const Single panelOriginalHeight = originalLineCount * buttonOriginalHeight;

        Int32 linePerPage = Configuration.Interface.MenuItemRowCount;
        Int32 columnPerPage = (Int32)Math.Floor((Single)(originalColumnCount * ((Single)linePerPage / originalLineCount)));
        if (originalColumnCount * originalLineCount >= this._itemIdList.Count) // 2 columns suffice
        {
            linePerPage = originalLineCount;
            columnPerPage = originalColumnCount;
        }
        else if (linePerPage >= originalLineCount * 2 && (originalColumnCount + 1) * (originalLineCount + originalLineCount / originalColumnCount) >= this._itemIdList.Count) // 3 columns suffice
        {
            linePerPage = originalLineCount + originalLineCount / originalColumnCount;
            columnPerPage = originalColumnCount + 1;
        }

        Int32 lineHeight = (Int32)Math.Round(panelOriginalHeight / linePerPage);
        Single scaleFactor = lineHeight / buttonOriginalHeight;

        Single alphaColumnTitles = (columnPerPage > originalColumnCount) ? 0f : 1f;
        _itemPanel.Background.Panel.Name.Label.alpha = alphaColumnTitles;
        _itemPanel.Background.Panel.Info.Label.alpha = alphaColumnTitles;
        _itemPanel.Background.Panel.Name2.Label.alpha = alphaColumnTitles;
        _itemPanel.Background.Panel.Info2.Label.alpha = alphaColumnTitles;

        _itemPanel.SubPanel.ChangeDims(columnPerPage, linePerPage, panelOriginalWidth / columnPerPage, lineHeight);
        _itemPanel.SubPanel.ButtonPrefab.IconSprite.SetAnchor(target: _itemPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.105f, relRight: 0.191f);
        _itemPanel.SubPanel.ButtonPrefab.IconSprite.width = _itemPanel.SubPanel.ButtonPrefab.IconSprite.height;

        _itemPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _itemPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.215f, relRight: 0.795f);
        _itemPanel.SubPanel.ButtonPrefab.NameLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _itemPanel.SubPanel.ButtonPrefab.NameLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));

        _itemPanel.SubPanel.ButtonPrefab.NumberLabel.SetAnchor(target: _itemPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.8f, relRight: 0.9f);
        _itemPanel.SubPanel.ButtonPrefab.NumberLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _itemPanel.SubPanel.ButtonPrefab.NumberLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));

        _itemPanel.SubPanel.RecycleListPopulator.RefreshTableView();

        KeyItemDetailHUD keyItemPrefab = new KeyItemDetailHUD(_keyItemPanel.SubPanel.ButtonPrefab.GameObject);
        _keyItemPanel.SubPanel.ChangeDims(2, linePerPage, panelOriginalWidth / 2f, lineHeight);
        //snouz: can't make these icons work when extending the key item menu, so 2 columns it stays
        //_keyItemPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _keyItemPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.105f, relRight: 0.75f);
        //keyItemPrefab.NewIcon.SetAnchor(target: _keyItemPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.75f, relRight: 0.9f);
        keyItemPrefab.NewIcon.SetDimensions((Int32)Math.Round(117f * scaleFactor), (Int32)Math.Round(64f * scaleFactor));
        keyItemPrefab.NewIconSprite.SetDimensions((Int32)Math.Round(44f * scaleFactor), (Int32)Math.Round(58f * scaleFactor));
        keyItemPrefab.NewIconLabelSprite.SetAnchor(keyItemPrefab.NewIconSprite.transform, 0.6f, 0f, 0.6f, 0f, 0, 0, 90f * scaleFactor, 58f * scaleFactor);
        keyItemPrefab.NameLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        keyItemPrefab.NameLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));
        _keyItemPanel.SubPanel.RecycleListPopulator.RefreshTableView();

    }

    [DebuggerHidden]
    private IEnumerator Show_delay()
    {
        yield return new WaitForEndOfFrame();

        if (_itemIdList.Count > 0)
        {
            ButtonGroupState.ActiveGroup = ItemGroupButton;
            ButtonGroupState.SetSecondaryOnGroup(SubMenuGroupButton);
            ButtonGroupState.HoldActiveStateOnGroup(SubMenuGroupButton);
        }
    }

    public override void Hide(SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate afterHideAction = delegate
        {
            MainMenuUI.UIControlPanel?.ExitMenu();
        };
        if (afterFinished != null)
            afterHideAction += afterFinished;
        base.Hide(afterHideAction);
        if (_fastSwitch)
            return;
        PersistenSingleton<UIManager>.Instance.MainMenuScene.StartSubmenuTweenIn();
        _usedItemIdList.Clear();
        RemoveCursorMemorize();
    }

    private void RemoveCursorMemorize()
    {
        _currentArrangeMode = 0;
        ButtonGroupState.RemoveCursorMemorize(SubMenuGroupButton);
        ButtonGroupState.RemoveCursorMemorize(ItemGroupButton);
        ButtonGroupState.RemoveCursorMemorize(KeyItemGroupButton);
    }

    public void OnLocalize()
    {
        if (!isActiveAndEnabled)
            return;
        if (_itemScrollList.isActiveAndEnabled)
            _itemScrollList.UpdateTableViewImp();
        if (_keyItemScrollList.isActiveAndEnabled)
            _keyItemScrollList.UpdateTableViewImp();
        if (_isShowingKeyItemDesp)
        {
            Int32 keyItemId = _keyItemIdList[_currentItemIndex];
            _keyItemDetailName.rawText = FF9TextTool.ImportantItemName(keyItemId);
            _keyItemDetailDescription.spacingY = _defaultSkinLabelSpacingY;
            _keyItemDetailDescription.rawText = FF9TextTool.ImportantItemSkin(keyItemId);
        }
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (!base.OnKeyConfirm(go))
            return true;
        if (_isShowingKeyItemDesp)
        {
            FF9Sfx.FF9SFX_Play(103);
            DisplayKeyItemSkin(false);
        }
        if (ButtonGroupState.ActiveGroup == SubMenuGroupButton)
        {
            FF9Sfx.FF9SFX_Play(103);
            _currentMenu = GetSubMenuFromGameObject(go);
            switch (_currentMenu)
            {
                case SubMenu.Use:
                    _currentArrangeMode = 0;
                    ButtonGroupState.ActiveGroup = ItemGroupButton;
                    ButtonGroupState.SetSecondaryOnGroup(SubMenuGroupButton);
                    ButtonGroupState.HoldActiveStateOnGroup(SubMenuGroupButton);
                    break;
                case SubMenu.Arrange:
                    _arrangeTransition.TweenIn(() =>
                    {
                        Loading = false;
                        ButtonGroupState.ActiveGroup = ArrangeMenuGroupButton;
                        ButtonGroupState.HoldActiveStateOnGroup(SubMenuGroupButton);
                        _arrangeDialog.Background.Caption.Label.Parser.ResetBeforeVariableTags();
                    });
                    Loading = true;
                    break;
                case SubMenu.Key:
                    if (_keyItemIdList.Count > 0)
                    {
                        ButtonGroupState.ActiveGroup = KeyItemGroupButton;
                        ButtonGroupState.SetSecondaryOnGroup(SubMenuGroupButton);
                        ButtonGroupState.HoldActiveStateOnGroup(SubMenuGroupButton);
                    }
                    break;
            }
        }
        else if (ButtonGroupState.ActiveGroup == ArrangeMenuGroupButton)
        {
            FF9Sfx.FF9SFX_Play(103);
            _currentArrangeMode = go.transform.GetSiblingIndex() + 1;
            switch (_currentArrangeMode)
            {
                case 1:
                    _arrangeTransition.TweenOut(() => Loading = false);
                    Loading = true;
                    ButtonGroupState.ActiveGroup = SubMenuGroupButton;
                    ArrangeAuto();
                    DisplayItem();
                    break;
                case 2:
                    _arrangeTransition.TweenOut(() => Loading = false);
                    Loading = true;
                    ButtonGroupState.ActiveGroup = ItemGroupButton;
                    ButtonGroupState.SetSecondaryOnGroup(SubMenuGroupButton);
                    ButtonGroupState.HoldActiveStateOnGroup(SubMenuGroupButton);
                    DisplayItem();
                    break;
            }
        }
        else if (ButtonGroupState.ActiveGroup == ItemGroupButton)
        {
            if (ButtonGroupState.ContainButtonInGroup(go, ItemGroupButton))
            {
                _currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                if (_currentArrangeMode == 0)
                {
                    PLAYER player = FF9StateSystem.Common.FF9.party.member[0];
                    RegularItem itemId = _itemIdList[_currentItemIndex];
                    FF9ITEM_DATA itemData = ff9item._FF9Item_Data[itemId];
                    if (ff9item.HasItemEffect(itemId) && (itemData.type & ItemType.Usable) != 0 && !_usedItemIdList.Contains(itemId))
                    {
                        if (itemId == RegularItem.GysahlGreens)
                        {
                            if (PersistenSingleton<UIManager>.Instance.ItemScene.FF9FItem_Vegetable())
                            {
                                PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
                                FF9Sfx.FF9SFX_Play(106);
                                ff9item.FF9Item_Remove(itemId, 1);
                                if (ff9item.FF9Item_GetCount(itemId) == 0)
                                    _usedItemIdList.Add(itemId);
                                DisplayItem();
                            }
                            else
                            {
                                FF9Sfx.FF9SFX_Play(102);
                            }
                        }
                        else
                        {
                            FF9Sfx.FF9SFX_Play(103);
                            TargetType targetType = ff9item.GetItemEffect(itemId).info.Target;
                            this._canMultiTarget = this.CanToggleMulti(targetType);
                            if (_currentItemIndex % 2 == 0)
                            {
                                _targetTransition.animatedInStartPosition = new Vector3(1543f, 0.0f, 0.0f);
                                _targetTransition.animatedOutEndPosition = new Vector3(1543f, 0.0f, 0.0f);
                                TargetListPanel.transform.localPosition = new Vector3(TargetPositionXOffset, 0.0f, 0.0f);
                            }
                            else
                            {
                                _targetTransition.animatedInStartPosition = new Vector3(-1543f, 0.0f, 0.0f);
                                _targetTransition.animatedOutEndPosition = new Vector3(-1543f, 0.0f, 0.0f);
                                TargetListPanel.transform.localPosition = new Vector3(-TargetPositionXOffset - 60f, 0.0f, 0.0f);
                            }
                            _targetTransition.DestinationPosition = [TargetListPanel.transform.localPosition];
                            DisplayTarget();
                            Loading = true;
                            _targetTransition.TweenIn([0], () =>
                            {
                                Loading = false;
                                ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
                                ButtonGroupState.ActiveGroup = TargetGroupButton;
                                ButtonGroupState.HoldActiveStateOnGroup(ItemGroupButton);
                                this.SetMultipleTarget(targetType == TargetType.All || targetType == TargetType.AllAlly || targetType == TargetType.AllEnemy || targetType == TargetType.Everyone);
                            });
                        }
                    }
                    else
                    {
                        FF9Sfx.FF9SFX_Play(102);
                    }
                }
                else if (_currentArrangeMode == 2)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    ButtonGroupState.SetCursorMemorize(go.GetChild(1), ItemArrangeGroupButton);
                    ButtonGroupState.ActiveGroup = ItemArrangeGroupButton;
                    ButtonGroupState.HoldActiveStateOnGroup(ItemGroupButton);
                    ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.Hide, ItemGroupButton);
                }
            }
            else
            {
                OnSecondaryGroupClick(go);
            }
        }
        else if (ButtonGroupState.ActiveGroup == KeyItemGroupButton)
        {
            if (ButtonGroupState.ContainButtonInGroup(go, KeyItemGroupButton))
            {
                _currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                if (_keyItemIdList[_currentItemIndex] != FF9FITEM_RARE_NONE)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    DisplayKeyItemSkin(true);
                }
                else
                {
                    FF9Sfx.FF9SFX_Play(102);
                }
            }
            else
            {
                OnSecondaryGroupClick(go);
            }
        }
        else if (ButtonGroupState.ActiveGroup == TargetGroupButton)
        {
            if (ButtonGroupState.ContainButtonInGroup(go, TargetGroupButton))
            {
                Int32 siblingIndex = go.transform.GetSiblingIndex();
                RegularItem itemId = _itemIdList[_currentItemIndex];
                Boolean canUseAbility = false;
                if (!this._multiTarget)
                {
                    PLAYER player = FF9StateSystem.Common.FF9.party.member[siblingIndex];
                    canUseAbility = SFieldCalculator.FieldCalcMain(player, player, itemId, 0u);
                }
                else
                {
                    for (Int32 i = 0; i < 4; ++i)
                    {
                        PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
                        if (player != null)
                            canUseAbility |= SFieldCalculator.FieldCalcMain(player, player, itemId, 1u);
                    }
                }
                if (canUseAbility)
                {
                    PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
                    FF9Sfx.FF9SFX_Play(106);
                    ff9item.FF9Item_Remove(itemId, 1);
                    if (ff9item.FF9Item_GetCount(itemId) > 0)
                    {
                        DisplayItem();
                        DisplayTarget();
                    }
                    else
                    {
                        _usedItemIdList.Add(itemId);
                        DisplayItem();
                        ButtonGroupState.ActiveGroup = ItemGroupButton;
                        Loading = true;
                        // ISSUE: method pointer
                        _targetTransition.TweenOut(new Byte[1], () => Loading = false);
                    }
                }
                else
                {
                    FF9Sfx.FF9SFX_Play(102);
                }
            }
        }
        else if (ButtonGroupState.ActiveGroup == ItemArrangeGroupButton)
        {
            if (ButtonGroupState.ContainButtonInGroup(go, ItemArrangeGroupButton))
            {
                FF9Sfx.FF9SFX_Play(103);
                _currentArrangeItemIndex = go.transform.parent.GetComponent<RecycleListItem>().ItemDataIndex;
                FF9ITEM ff9Item = FF9StateSystem.Common.FF9.item[_currentItemIndex];
                FF9StateSystem.Common.FF9.item[_currentItemIndex] = FF9StateSystem.Common.FF9.item[_currentArrangeItemIndex];
                FF9StateSystem.Common.FF9.item[_currentArrangeItemIndex] = ff9Item;
                _switchingItem = true;
                DisplayItem();
                _itemScrollList.JumpToIndex(_currentArrangeItemIndex, false);
                ButtonGroupState.RemoveCursorMemorize(ItemGroupButton);
                ButtonGroupState.ActiveGroup = ItemGroupButton;
                ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.Limit, ItemGroupButton);
                _switchingItem = false;
            }
            else
            {
                OnSecondaryGroupClick(go);
            }
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (!base.OnKeyCancel(go))
            return true;
        if (_isShowingKeyItemDesp)
        {
            FF9Sfx.FF9SFX_Play(101);
            DisplayKeyItemSkin(false);
        }
        if (ButtonGroupState.ActiveGroup == SubMenuGroupButton)
        {
            FF9Sfx.FF9SFX_Play(101);
            _fastSwitch = false;
            Hide(() =>
            {
                PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = false;
                PersistenSingleton<UIManager>.Instance.MainMenuScene.CurrentSubMenu = MainMenuUI.SubMenu.Item;
                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.MainMenu);
            });
        }
        else if (ButtonGroupState.ActiveGroup == ArrangeMenuGroupButton)
        {
            FF9Sfx.FF9SFX_Play(101);
            _currentArrangeMode = 0;
            // ISSUE: method pointer
            _arrangeTransition.TweenOut(() => Loading = false);
            Loading = true;
            ButtonGroupState.ActiveGroup = SubMenuGroupButton;
        }
        else if (ButtonGroupState.ActiveGroup == ItemGroupButton)
        {
            FF9Sfx.FF9SFX_Play(101);
            ButtonGroupState.ActiveGroup = SubMenuGroupButton;
        }
        else if (ButtonGroupState.ActiveGroup == KeyItemGroupButton)
        {
            FF9Sfx.FF9SFX_Play(101);
            ButtonGroupState.ActiveGroup = SubMenuGroupButton;
        }
        else if (ButtonGroupState.ActiveGroup == TargetGroupButton)
        {
            FF9Sfx.FF9SFX_Play(101);
            this.SetMultipleTarget(false);
            ButtonGroupState.ActiveGroup = ItemGroupButton;
            Loading = true;
            // ISSUE: method pointer
            _targetTransition.TweenOut(new Byte[1], () => Loading = false);
        }
        else if (ButtonGroupState.ActiveGroup == ItemArrangeGroupButton)
        {
            FF9Sfx.FF9SFX_Play(101);
            Int32 itemDataIndex = go.transform.parent.GetComponent<RecycleListItem>().ItemDataIndex;
            _switchingItem = true;
            _currentItemIndex = itemDataIndex;
            _itemScrollList.JumpToIndex(_currentItemIndex, false);
            ButtonGroupState.RemoveCursorMemorize(ItemGroupButton);
            ButtonGroupState.ActiveGroup = ItemGroupButton;
            ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.Limit, ItemGroupButton);
            _switchingItem = false;
        }
        return true;
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        if (!base.OnKeyLeftBumper(go))
            return true;
        if (ButtonGroupState.ActiveGroup == ItemArrangeGroupButton)
        {
            _itemScrollList.ActiveNumber = _currentItemIndex;
        }
        else if (ButtonGroupState.ActiveGroup == TargetGroupButton)
        {
            FF9Sfx.FF9SFX_Play(103);
            this.ToggleMultipleTarget();
        }
        return true;
    }

    public override Boolean OnKeyRightBumper(GameObject go)
    {
        if (!base.OnKeyRightBumper(go))
            return true;
        if (ButtonGroupState.ActiveGroup == ItemArrangeGroupButton)
        {
            _itemScrollList.ActiveNumber = _currentItemIndex;
        }
        else if (ButtonGroupState.ActiveGroup == TargetGroupButton)
        {
            FF9Sfx.FF9SFX_Play(103);
            this.ToggleMultipleTarget();
        }
        return true;
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (!base.OnItemSelect(go))
            return true;
        if (ButtonGroupState.ActiveGroup == SubMenuGroupButton)
        {
            if (_currentMenu != GetSubMenuFromGameObject(go))
            {
                _currentMenu = GetSubMenuFromGameObject(go);
                switch (_currentMenu)
                {
                    case SubMenu.Use:
                    case SubMenu.Arrange:
                        if (!ItemListPanel.activeSelf)
                        {
                            ItemListPanel.SetActive(true);
                            KeyItemListPanel.SetActive(false);
                            _usedItemIdList.Clear();
                            DisplayItem();
                        }
                        break;
                    case SubMenu.Key:
                        if (!KeyItemListPanel.activeSelf)
                        {
                            ItemListPanel.SetActive(false);
                            KeyItemListPanel.SetActive(true);
                            DisplayKeyItem();
                        }
                        break;
                }
            }
        }
        else if (ButtonGroupState.ActiveGroup == ItemGroupButton || ButtonGroupState.ActiveGroup == KeyItemGroupButton)
        {
            Int32 itemDataIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
            _currentItemIndex = itemDataIndex;
        }
        else if (ButtonGroupState.ActiveGroup == ItemArrangeGroupButton)
        {
            Int32 itemDataIndex = go.transform.parent.GetComponent<RecycleListItem>().ItemDataIndex;
            _currentArrangeItemIndex = itemDataIndex;
        }
        return true;
    }

    private void OnSecondaryGroupClick(GameObject go)
    {
        ButtonGroupState.HoldActiveStateOnGroup(go, SubMenuGroupButton);
        if (ButtonGroupState.ActiveGroup == ItemGroupButton)
        {
            FF9Sfx.muteSfx = true;
            OnKeyCancel(_itemScrollList.GetItem(_currentItemIndex).gameObject);
            FF9Sfx.muteSfx = false;
            OnKeyConfirm(go);
        }
        else if (ButtonGroupState.ActiveGroup == KeyItemGroupButton)
        {
            FF9Sfx.muteSfx = true;
            OnKeyCancel(_keyItemScrollList.GetItem(_currentItemIndex).gameObject);
            FF9Sfx.muteSfx = false;
            OnKeyConfirm(go);
        }
        else
        {
            if (ButtonGroupState.ActiveGroup != ItemArrangeGroupButton)
                return;

            GameObject currentItemObject = _itemScrollList.GetItem(_currentItemIndex).gameObject;
            FF9Sfx.muteSfx = true;
            OnKeyCancel(currentItemObject.GetChild(1));
            OnKeyCancel(currentItemObject);
            FF9Sfx.muteSfx = false;
            OnKeyConfirm(go);
        }
    }

    private void DisplayItem()
    {
        _itemIdList.Clear();
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        foreach (FF9ITEM ff9Item in FF9StateSystem.Common.FF9.item)
        {
            if (ff9Item.count > 0)
            {
                _itemIdList.Add(ff9Item.id);
                Boolean usableInMenu = (ff9item._FF9Item_Data[ff9Item.id].type & ItemType.Usable) != 0;
                FieldItemListData fieldItemListData = new FieldItemListData
                {
                    Enable = usableInMenu,
                    ItemId = ff9Item.id,
                    ItemCount = ff9Item.count
                };
                inDataList.Add(fieldItemListData);
            }
            else if (_usedItemIdList.Contains(ff9Item.id))
            {
                _itemIdList.Add(ff9Item.id);
                FieldItemListData fieldItemListData = new FieldItemListData
                {
                    Enable = false,
                    ItemId = ff9Item.id,
                    ItemCount = 0
                };
                inDataList.Add(fieldItemListData);
            }
        }
        if (_itemScrollList.ItemsPool.Count == 0)
        {
            _itemScrollList.PopulateListItemWithData = DisplayItemDetail;
            _itemScrollList.OnRecycleListItemClick += OnListItemClick;
            _itemScrollList.InitTableView(inDataList, 0);
        }
        else
        {
            _itemScrollList.SetOriginalData(inDataList);
            if (ButtonGroupState.HaveCursorMemorize(ItemGroupButton) || ButtonGroupState.ActiveGroup == ItemArrangeGroupButton)
                return;
            _itemScrollList.JumpToIndex(0, false);
        }
    }

    private void DisplayItemDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        FieldItemListData fieldItemListData = (FieldItemListData)data;
        ItemDetailHUD itemDetailHud = new ItemDetailHUD(item.gameObject);
        if (isInit)
        {
            UIEventListener uiEventListener = UIEventListener.Get(itemDetailHud.ManualButton.gameObject);
            uiEventListener.Select += itemDetailHud.Self.GetComponent<ScrollItemKeyNavigation>().OnOtherObjectSelect;
            DisplayWindowBackground(item.gameObject, null);
        }
        if (ButtonGroupState.ActiveGroup == ItemArrangeGroupButton)
        {
            if (_currentItemIndex == index && !_switchingItem)
            {
                if (isInit)
                    ButtonGroupState.HoldActiveStateOnGroup(item.gameObject, ItemGroupButton);
                ButtonGroupState.SetButtonAnimation(item.gameObject, false);
                ButtonGroupState.SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior.Hide, ItemGroupButton);
            }
            else
                ButtonGroupState.SetButtonAnimation(item.gameObject, true);
        }
        FF9UIDataTool.DisplayItem(fieldItemListData.ItemId, itemDetailHud.IconSprite, itemDetailHud.NameLabel, fieldItemListData.Enable);
        itemDetailHud.NumberLabel.rawText = fieldItemListData.ItemCount.ToString();
        itemDetailHud.NumberLabel.color = !fieldItemListData.Enable ? FF9TextTool.Gray : FF9TextTool.White;
        itemDetailHud.Button.Help.Enable = true;
        itemDetailHud.Button.Help.Text = FF9TextTool.ItemHelpDescription(fieldItemListData.ItemId);
        itemDetailHud.ManualButton.Help.Enable = true;
        itemDetailHud.ManualButton.Help.Text = FF9TextTool.ItemHelpDescription(fieldItemListData.ItemId);
    }

    private void DisplayKeyItem()
    {
        _keyItemIdList.Clear();
        foreach (Int32 id in FF9StateSystem.Common.FF9.rare_item_obtained)
            _keyItemIdList.Add(id);
        if (_keyItemIdList.Count == 0)
            _keyItemIdList.Add(FF9FITEM_RARE_NONE);
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        foreach (Int32 keyItemId in _keyItemIdList)
            inDataList.Add(new FieldKeyItemListData { KeyItemId = keyItemId });
        if (_keyItemScrollList.ItemsPool.Count == 0)
        {
            _keyItemScrollList.PopulateListItemWithData = DisplayKeyItemDetail;
            _keyItemScrollList.OnRecycleListItemClick += OnListItemClick;
            _keyItemScrollList.InitTableView(inDataList, 0);
        }
        else
        {
            _keyItemScrollList.SetOriginalData(inDataList);
            if (ButtonGroupState.HaveCursorMemorize(KeyItemGroupButton))
                return;
            _keyItemScrollList.JumpToIndex(0, false);
        }
    }

    private void DisplayKeyItemDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        FieldKeyItemListData fieldKeyItemListData = (FieldKeyItemListData)data;
        KeyItemDetailHUD keyItemDetailHud = new KeyItemDetailHUD(item.gameObject);
        if (isInit)
            DisplayWindowBackground(item.gameObject, null);
        if (fieldKeyItemListData.KeyItemId == FF9FITEM_RARE_NONE)
        {
            keyItemDetailHud.Button.Help.Enable = false;
            keyItemDetailHud.NewIconSprite.spriteName = String.Empty;
            keyItemDetailHud.NewIconLabelSprite.gameObject.SetActive(false);
            keyItemDetailHud.NameLabel.gameObject.SetActive(false);
        }
        else
        {
            keyItemDetailHud.NameLabel.gameObject.SetActive(true);
            keyItemDetailHud.NameLabel.rawText = FF9TextTool.ImportantItemName(fieldKeyItemListData.KeyItemId);
            if (ff9item.FF9Item_IsUsedImportant(fieldKeyItemListData.KeyItemId))
            {
                keyItemDetailHud.NewIconSprite.spriteName = String.Empty;
                keyItemDetailHud.NewIconLabelSprite.gameObject.SetActive(false);
            }
            else
            {
                keyItemDetailHud.NewIconSprite.spriteName = "icon_new_exclamation";
                keyItemDetailHud.NewIconLabelSprite.gameObject.SetActive(true);
            }
            keyItemDetailHud.Button.Help.Enable = true;
            keyItemDetailHud.Button.Help.Text = FF9TextTool.ImportantItemHelpDescription(fieldKeyItemListData.KeyItemId);
        }
    }

    private void DisplayTarget()
    {
        Int32 hudIndex = 0;
        foreach (PLAYER player in FF9StateSystem.Common.FF9.party.member)
        {
            CharacterDetailHUD targetHud = _targetHudList[hudIndex++];
            targetHud.Self.SetActive(true);
            if (player == null)
            {
                targetHud.Content.SetActive(false);
                continue;
            }
            targetHud.Content.SetActive(true);
            FF9UIDataTool.DisplayCharacterDetail(player, targetHud);
            FF9UIDataTool.DisplayCharacterAvatar(player, new Vector2(), new Vector2(), targetHud.AvatarSprite, false);
            switch (ff9item.GetItemEffect(_itemIdList[_currentItemIndex]).info.DisplayStats)
            {
                case TargetDisplay.None:
                case TargetDisplay.Hp:
                case TargetDisplay.Mp:
                    targetHud.HPPanel.SetActive(true);
                    targetHud.MPPanel.SetActive(true);
                    targetHud.StatusesPanel.SetActive(false);
                    continue;
                case TargetDisplay.Debuffs:
                case TargetDisplay.Buffs:
                    targetHud.HPPanel.SetActive(false);
                    targetHud.MPPanel.SetActive(false);
                    targetHud.StatusesPanel.SetActive(true);
                    continue;
                default:
                    continue;
            }
        }
        SetAvailableCharacter(false);
    }

    private void DisplayKeyItemSkin(Boolean visibility)
    {
        if (visibility)
        {
            Int32 keyItemId = _keyItemIdList[_currentItemIndex];
            _keyItemDetailName.rawText = FF9TextTool.ImportantItemName(keyItemId);
            _keyItemDetailDescription.spacingY = _defaultSkinLabelSpacingY;
            _keyItemDetailDescription.rawText = FF9TextTool.ImportantItemSkin(keyItemId);
            Loading = true;
            // ISSUE: method pointer
            _keyItemSkinTransition.TweenIn(new Byte[1], () =>
            {
                Loading = false;
                _isShowingKeyItemDesp = true;
            });
            ButtonGroupState.DisableAllGroup(false);
            ButtonGroupState.HoldActiveStateOnGroup(KeyItemGroupButton);
        }
        else
        {
            Loading = true;
            _isShowingKeyItemDesp = false;
            // ISSUE: method pointer
            _keyItemSkinTransition.TweenOut(new Byte[1], () =>
            {
                Loading = false;
                ButtonGroupState.ActiveGroup = KeyItemGroupButton;
                Singleton<PointerManager>.Instance.SetPointerBlinkAt(ButtonGroupState.ActiveButton, false);
            });
            ff9item.FF9Item_UseImportant(_keyItemIdList[_currentItemIndex]);
            DisplayKeyItem();
        }
    }

    private void SetAvailableCharacter(Boolean includeEmpty)
    {
        List<CharacterDetailHUD> targetList = new List<CharacterDetailHUD>();
        if (!includeEmpty)
        {
            foreach (CharacterDetailHUD charHUD in this._targetHudList)
            {
                if (!this._multiTarget)
                {
                    if (charHUD.Content.activeSelf)
                    {
                        targetList.Add(charHUD);
                        ButtonGroupState.SetButtonEnable(charHUD.Self, true);
                    }
                    else
                    {
                        ButtonGroupState.SetButtonEnable(charHUD.Self, false);
                    }
                }
            }
        }
        else
        {
            foreach (CharacterDetailHUD charHUD in this._targetHudList)
            {
                targetList.Add(charHUD);
                ButtonGroupState.SetButtonEnable(charHUD.Self, true);
            }
        }
        for (Int32 charIndex = 0; charIndex < targetList.Count; ++charIndex)
        {
            Int32 prevIndex = charIndex - 1;
            Int32 nextIndex = charIndex + 1;
            if (charIndex == 0)
                prevIndex = targetList.Count - 1;
            if (charIndex == targetList.Count - 1)
                nextIndex = 0;
            UIKeyNavigation navig = targetList[charIndex].Self.GetComponent<UIKeyNavigation>();
            navig.onUp = targetList[prevIndex].Self;
            navig.onDown = targetList[nextIndex].Self;
        }
    }

    private Boolean CanToggleMulti(TargetType targetType)
    {
        switch (targetType)
        {
            case TargetType.ManyAny:
            case TargetType.ManyAlly:
            case TargetType.ManyEnemy:
                break;
            default:
                return false;
        }

        Int32 membersCount = 0;
        for (Int32 index = 0; index < 4; ++index)
            if (FF9StateSystem.Common.FF9.party.member[index] != null)
                if (++membersCount > 1)
                    return true;

        return false;
    }

    private void SetMultipleTarget(Boolean isActive)
    {
        ButtonGroupState.SetAllTarget(isActive);
        this._multiTarget = isActive;
        foreach (CharacterDetailHUD charHUD in this._targetHudList)
            charHUD.Self.GetComponent<ButtonGroupState>().Help.Enable = !isActive;
        ButtonGroupState.UpdateActiveButton();
    }

    private void ToggleMultipleTarget()
    {
        if (!this._canMultiTarget)
            return;

        this.SetMultipleTarget(!this._multiTarget);
    }

    private void ArrangeAuto()
    {
        FF9StateSystem.Common.FF9.item.RemoveAll(item => item.id == RegularItem.NoItem || item.count <= 0);
        FF9StateSystem.Common.FF9.item.Sort((i1, i2) =>
        {
            if (i1.id == i2.id)
                return 0;
            FF9ITEM_DATA itemData1 = ff9item._FF9Item_Data[i1.id];
            FF9ITEM_DATA itemData2 = ff9item._FF9Item_Data[i2.id];
            return itemData1.sort > itemData2.sort
                || itemData1.sort == itemData2.sort && i1.id > i2.id
                ? 1 : -1;
        });
    }

    public SubMenu GetSubMenuFromGameObject(GameObject go)
    {
        if (go == UseSubMenu)
            return SubMenu.Use;
        if (go == ArrangeSubMenu)
            return SubMenu.Arrange;
        return go == KeySubMenu ? SubMenu.Key : SubMenu.None;
    }

    public Boolean FF9FItem_Vegetable()
    {
        if (PersistenSingleton<FF9StateSystem>.Instance.mode != 3 || FF9FITEM_CHOCOBO_NONE == FF9StateSystem.EventState.gEventGlobal[FF9FITEM_EVENT_KIND_CHOCOBO] || FF9StateSystem.EventState.gEventGlobal[FF9FITEM_EVENT_VEGETABLE] != 0 || !ff9.w_frameChocoboCheck())
            return false;
        FF9StateSystem.EventState.gEventGlobal[FF9FITEM_EVENT_VEGETABLE] = 1;
        _chocoboDialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get("UseChocoboItem"), (Int32)(400.0 / UIManager.ResourceXMultipier), 1, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.Chocobo);
        _chocoboDialog.Panel.depth = 2;
        _chocoboDialog.PhraseLabel.parent.GetComponent<UIPanel>().depth = 3;
        StartCoroutine(WaitForFade());
        Loading = true;
        return true;
    }

    [DebuggerHidden]
    private IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(1.5f);

        PersistenSingleton<UIManager>.Instance.MainMenuScene.ScreenFadeGameObject.GetParent().GetComponent<UIPanel>().depth = 100;
        _chocoboDialog.Hide();

        SceneDirector.FF9Wipe_FadeInEx(12);
        _fastSwitch = false;
        Hide(delegate()
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.SubMenuPanel.SetActive(false);
            PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.WorldHUD);
            PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = true;
            PersistenSingleton<UIManager>.Instance.MainMenuScene.ScreenFadeGameObject.GetParent().GetComponent<UIPanel>().depth = 10;
        });
    }

    private void Awake()
    {
        FadingComponent = ScreenFadeGameObject.GetComponent<HonoFading>();
        UIEventListener.Get(UseSubMenu).Click += onClick;
        UIEventListener.Get(ArrangeSubMenu).Click += onClick;
        UIEventListener.Get(KeySubMenu).Click += onClick;

        foreach (Component component in TargetListPanel.GetChild(0).transform)
        {
            GameObject go = component.gameObject;
            UIEventListener.Get(go).Click += onClick;
            _targetHudList.Add(new CharacterDetailHUD(go, true));
            if (FF9StateSystem.MobilePlatform)
                go.GetComponent<ButtonGroupState>().Help.TextKey = "TargetHelpMobile";
        }

        //this._importantItemHitArea = KeyItemDetailPanel.GetChild(0);
        _keyItemDetailName = KeyItemDetailPanel.GetChild(1).GetComponent<UILabel>();
        _keyItemDetailDescription = KeyItemDetailPanel.GetChild(2).GetComponent<UILabel>();
        _defaultSkinLabelSpacingY = _keyItemDetailDescription.spacingY;
        _itemScrollList = ItemListPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        _keyItemScrollList = KeyItemListPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        _itemPanel = new GOScrollablePanel(ItemListPanel);
        _keyItemPanel = new GOScrollablePanel(KeyItemListPanel);
        _arrangeDialog = new ArrangeDialogHUD(ArrangeDialog);

        foreach (var button in _arrangeDialog.Choices)
            button.EventListener.Click += onClick;

        _targetTransition = TransitionGroup.GetChild(0).GetComponent<HonoTweenPosition>();
        _keyItemSkinTransition = TransitionGroup.GetChild(1).GetComponent<HonoTweenPosition>();
        _arrangeTransition = TransitionGroup.GetChild(2).GetComponent<HonoTweenClipping>();

        _background = new GOMenuBackground(this.transform.GetChild(6).gameObject, "item_bg");

        _itemPanel.Background.Panel.Name.Label.fixedAlignment = true;
        _itemPanel.Background.Panel.Name2.Label.fixedAlignment = true;
        _itemPanel.Background.Panel.Info.Label.leftAnchor.absolute -= 90;
        _itemPanel.Background.Panel.Info2.Label.leftAnchor.absolute -= 90;
        _keyItemPanel.Background.Panel.Name.Label.fixedAlignment = true;
        _keyItemPanel.Background.Panel.Info.Label.fixedAlignment = true;
    }

    private struct KeyItemDetailHUD
    {
        public GameObject Self;
        public ButtonGroupState Button;
        public UILabel NameLabel;
        public UIWidget NewIcon;
        public UISprite NewIconSprite;
        public UISprite NewIconLabelSprite;

        public KeyItemDetailHUD(GameObject go)
        {
            Self = go;
            Button = go.GetComponent<ButtonGroupState>();
            NameLabel = go.GetChild(0).GetComponent<UILabel>();
            NewIcon = go.GetChild(1).GetComponent<UIWidget>();
            NewIconSprite = go.GetChild(1).GetChild(0).GetComponent<UISprite>();
            NewIconLabelSprite = go.GetChild(1).GetChild(1).GetComponent<UISprite>();
        }
    }

    private class ArrangeDialogHUD : GOPanel
    {
        public readonly UIPanel ChoicePanel;
        public readonly List<GOSimpleButton<GOLocalizableLabel>> Choices;
        public readonly GOFrameBackground Background;

        public ArrangeDialogHUD(GameObject go) : base(go)
        {
            ChoicePanel = go.GetChild(0).GetComponent<UIPanel>();
            Int32 choiceCount = go.GetChild(0).transform.childCount;
            Choices = new List<GOSimpleButton<GOLocalizableLabel>>(choiceCount);
            for (Int32 i = 0; i < choiceCount; i++)
                Choices.Add(new GOSimpleButton<GOLocalizableLabel>(go.GetChild(0).GetChild(i)));
            Background = new GOFrameBackground(go.GetChild(1));
        }
    }

    private class ItemDetailHUD : ItemListDetailWithIconHUD
    {
        public ButtonGroupState ManualButton;

        public ItemDetailHUD(GameObject go)
            : base(go, true)
        {
            ManualButton = go.GetChild(1).GetComponent<ButtonGroupState>();
        }
    }

    public enum SubMenu
    {
        Use,
        Arrange,
        Key,
        None,
    }

    public class FieldItemListData : ListDataTypeBase
    {
        public Boolean Enable;
        public RegularItem ItemId;
        public Int32 ItemCount;

        public FieldItemListData()
        {
        }
    }

    public class FieldKeyItemListData : ListDataTypeBase
    {
        public Int32 KeyItemId;

        public FieldKeyItemListData()
        {
        }
    }
}
