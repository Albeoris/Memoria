using System;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Field;
using Memoria.Prime;
using Memoria.Scenes.BattleHUD;
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
    public static String SubMenuGroupButton;
    public static String ItemGroupButton;
    public static String KeyItemGroupButton;
    public static String TargetGroupButton;
    public static String ArrangeMenuGroupButton;
    public static String ItemArrangeGroupButton;
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
    private static Int32 FF9FITEM_RARE_MAX;
    private static Int32 FF9FITEM_RARE_NONE;
    private static Int32 FF9FITEM_CHOCOBO_NONE;
    private static Int32 FF9FITEM_ID_VEGETABLE;
    private static Int32 FF9FITEM_EVENT_VEGETABLE;
    private static Int32 FF9FITEM_EVENT_KIND_CHOCOBO;
    private static Int32 MaxItemSlot;
    private static Single TargetPositionXOffset;
    private readonly List<Int32> _itemIdList;
    private readonly List<Int32> _usedItemIdList;
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

    static ItemUI()
    {
        SubMenuGroupButton = "Item.SubMenu";
        ItemGroupButton = "Item.Item";
        KeyItemGroupButton = "Item.KeyItem";
        TargetGroupButton = "Item.Target";
        ArrangeMenuGroupButton = "Item.ArrangeMenu";
        ItemArrangeGroupButton = "Item.Arrange";
        FF9FITEM_RARE_MAX = 80;
        FF9FITEM_RARE_NONE = Byte.MaxValue;
        FF9FITEM_ID_VEGETABLE = 251;
        FF9FITEM_EVENT_VEGETABLE = 181;
        FF9FITEM_EVENT_KIND_CHOCOBO = 191;
        MaxItemSlot = 256;
        TargetPositionXOffset = 338f;
    }

    public ItemUI()
    {
        _itemIdList = new List<Int32>();
        _usedItemIdList = new List<Int32>();
        //this._ketIdList = new List<Int32>();
        _keyItemIdList = new List<Int32>();
        _targetHudList = new List<CharacterDetailHUD>();
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate action = () =>
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.SubMenuPanel.SetActive(false);
            ButtonGroupState.SetPointerDepthToGroup(4, ItemGroupButton);
            ButtonGroupState.SetPointerDepthToGroup(7, TargetGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(54f, 0.0f), ItemGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(54f, 0.0f), KeyItemGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(160f, 10f), ArrangeMenuGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(48f, -6f), ItemArrangeGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(_itemScrollList.ScrollButton, ItemGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(_itemScrollList.ScrollButton, ItemArrangeGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(_keyItemScrollList.ScrollButton, KeyItemGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(ItemListPanel.GetComponent<UIWidget>(), _itemScrollList.cellHeight, ItemGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(ItemListPanel.GetComponent<UIWidget>(), _itemScrollList.cellHeight, ItemArrangeGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(KeyItemListPanel.GetComponent<UIWidget>(), _keyItemScrollList.cellHeight, KeyItemGroupButton);
            ButtonGroupState.ActiveGroup = SubMenuGroupButton;
            StartCoroutine(Show_dalay());
            afterFinished?.Invoke();
        };

        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(action);
        ItemListPanel.SetActive(true);
        KeyItemListPanel.SetActive(false);
        DisplayItem();
        HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);
        _itemScrollList.ScrollButton.DisplayScrollButton(false, false);
        _keyItemScrollList.ScrollButton.DisplayScrollButton(false, false);
    }

    [DebuggerHidden]
    private IEnumerator Show_dalay()
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
        base.Hide(afterFinished);
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

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go))
        {
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
                        Int32 itemId = _itemIdList[_currentItemIndex];
                        FF9ITEM_DATA ff9ItemData = ff9item._FF9Item_Data[itemId];
                        if (citem.YCITEM_IS_ITEM(itemId))
                        {
                            ITEM_DATA tbl = ff9item._FF9Item_Info[itemId - 224];
                            if ((ff9ItemData.type & 1) == 1)
                            {
                                if (!_usedItemIdList.Contains(itemId))
                                {
                                    if ((FF9FITEM_ID_VEGETABLE != itemId ? tbl.info.DisplayStats : 0) == 0)
                                    {
                                        if (SFieldCalculator.FieldCalcMain(player, player, tbl, tbl.Ref.ScriptId, 0U))
                                        {
                                            FF9Sfx.FF9SFX_Play(106);
                                            ff9item.FF9Item_Remove(itemId, 1);
                                            if (ff9item.FF9Item_GetCount(itemId) == 0)
                                                _usedItemIdList.Add(itemId);
                                            DisplayItem();
                                        }
                                        else
                                            FF9Sfx.FF9SFX_Play(102);
                                    }
                                    else
                                    {
                                        FF9Sfx.FF9SFX_Play(103);
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
                                            TargetListPanel.transform.localPosition = new Vector3((Single)(-TargetPositionXOffset - 60.0), 0.0f, 0.0f);
                                        }
                                        _targetTransition.DestinationPosition = new Vector3[1]
                                        {
                                            TargetListPanel.transform.localPosition
                                        };
                                        DisplayTarget();
                                        Loading = true;
                                        _targetTransition.TweenIn(new Byte[1], () =>
                                        {
                                            Loading = false;
                                            ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
                                            ButtonGroupState.ActiveGroup = TargetGroupButton;
                                            ButtonGroupState.HoldActiveStateOnGroup(ItemGroupButton);
                                        });
                                    }
                                }
                                else
                                    FF9Sfx.FF9SFX_Play(102);
                            }
                            else
                                FF9Sfx.FF9SFX_Play(102);
                        }
                        else
                            FF9Sfx.FF9SFX_Play(102);
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
                    OnSecondaryGroupClick(go);
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
                        FF9Sfx.FF9SFX_Play(102);
                }
                else
                    OnSecondaryGroupClick(go);
            }
            else if (ButtonGroupState.ActiveGroup == TargetGroupButton)
            {
                if (ButtonGroupState.ContainButtonInGroup(go, TargetGroupButton))
                {
                    Int32 siblingIndex = go.transform.GetSiblingIndex();
                    Int32 itemId = _itemIdList[_currentItemIndex];
                    PLAYER player = FF9StateSystem.Common.FF9.party.member[siblingIndex];
                    ITEM_DATA tbl = ff9item._FF9Item_Info[itemId - 224];
                    if (SFieldCalculator.FieldCalcMain(player, player, tbl, tbl.Ref.ScriptId, 0U))
                    {
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
                        FF9Sfx.FF9SFX_Play(102);
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
                    OnSecondaryGroupClick(go);
            }
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go))
        {
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
        }
        return true;
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        base.OnKeyLeftBumper(go);
        if (ButtonGroupState.ActiveGroup == ItemArrangeGroupButton)
            _itemScrollList.ActiveNumber = _currentItemIndex;
        return true;
    }

    public override Boolean OnKeyRightBumper(GameObject go)
    {
        base.OnKeyRightBumper(go);
        if (ButtonGroupState.ActiveGroup == ItemArrangeGroupButton)
            _itemScrollList.ActiveNumber = _currentItemIndex;
        return true;
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (base.OnItemSelect(go))
        {
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
                Boolean flag = (ff9item._FF9Item_Data[ff9Item.id].type & 1) != 0;
                FieldItemListData fieldItemListData = new FieldItemListData
                {
                    Enable = flag,
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
        itemDetailHud.NumberLabel.text = fieldItemListData.ItemCount.ToString();
        itemDetailHud.NumberLabel.color = !fieldItemListData.Enable ? FF9TextTool.Gray : FF9TextTool.White;
        itemDetailHud.Button.Help.Enable = true;
        itemDetailHud.Button.Help.Text = FF9TextTool.ItemHelpDescription(fieldItemListData.ItemId);
        itemDetailHud.ManualButton.Help.Enable = true;
        itemDetailHud.ManualButton.Help.Text = FF9TextTool.ItemHelpDescription(fieldItemListData.ItemId);
    }

    private void DisplayKeyItem()
    {
        _keyItemIdList.Clear();
        for (Int32 id = 0; id < FF9FITEM_RARE_MAX; ++id)
        {
            if (ff9item.FF9Item_IsExistImportant(id))
                _keyItemIdList.Add(id);
        }
        if (_keyItemIdList.Count == 0)
            _keyItemIdList.Add(FF9FITEM_RARE_NONE);
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        using (List<Int32>.Enumerator enumerator = _keyItemIdList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                Byte current = (Byte)enumerator.Current;
                FieldKeyItemListData fieldKeyItemListData = new FieldKeyItemListData { KeyItemId = current };
                inDataList.Add(fieldKeyItemListData);
            }
        }
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
            keyItemDetailHud.NameLabel.text = FF9TextTool.ImportantItemName(fieldKeyItemListData.KeyItemId);
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
        Int32 num = 0;
        foreach (PLAYER player in FF9StateSystem.Common.FF9.party.member)
        {
            CharacterDetailHUD targetHud = _targetHudList[num++];
            targetHud.Self.SetActive(true);
            if (player != null)
            {
                targetHud.Content.SetActive(true);
                FF9UIDataTool.DisplayCharacterDetail(player, targetHud);
                FF9UIDataTool.DisplayCharacterAvatar(player, new Vector2(), new Vector2(), targetHud.AvatarSprite, false);
                switch (ff9item._FF9Item_Info[_itemIdList[_currentItemIndex] - 224].info.DisplayStats)
                {
                    case TargetDisplay.None:
                    case TargetDisplay.Hp:
                    case TargetDisplay.Mp:
                        targetHud.HPPanel.SetActive(true);
                        targetHud.MPPanel.SetActive(true);
                        targetHud.StatusesPanel.SetActive(false);
                        continue;
                    case TargetDisplay.Debufs:
                    case TargetDisplay.Bufs:
                        targetHud.HPPanel.SetActive(false);
                        targetHud.MPPanel.SetActive(false);
                        targetHud.StatusesPanel.SetActive(true);
                        continue;
                    default:
                        continue;
                }
            }
            else
                targetHud.Content.SetActive(false);
        }
        SetAvalableCharacter(false);
    }

    private void DisplayKeyItemSkin(Boolean visibility)
    {
        if (visibility)
        {
            Int32 keyItemId = _keyItemIdList[_currentItemIndex];
            _keyItemDetailName.text = FF9TextTool.ImportantItemName(keyItemId);
            _keyItemDetailDescription.spacingY = _defaultSkinLabelSpacingY;
            String text = FF9TextTool.ImportantItemSkin(keyItemId);
            Single additionalWidth = 0.0f;
            _keyItemDetailDescription.text = _keyItemDetailDescription.PhrasePreOpcodeSymbol(text, ref additionalWidth);
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

    private void SetAvalableCharacter(Boolean includeEmpty)
    {
        List<CharacterDetailHUD> characterDetailHudList = new List<CharacterDetailHUD>();
        if (!includeEmpty)
        {
            using (List<CharacterDetailHUD>.Enumerator enumerator = _targetHudList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    CharacterDetailHUD current = enumerator.Current;
                    if (current.Content.activeSelf)
                    {
                        characterDetailHudList.Add(current);
                        ButtonGroupState.SetButtonEnable(current.Self, true);
                    }
                    else
                        ButtonGroupState.SetButtonEnable(current.Self, false);
                }
            }
        }
        else
        {
            using (List<CharacterDetailHUD>.Enumerator enumerator = _targetHudList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    CharacterDetailHUD current = enumerator.Current;
                    characterDetailHudList.Add(current);
                    ButtonGroupState.SetButtonEnable(current.Self, true);
                }
            }
        }
        for (Int32 index1 = 0; index1 < characterDetailHudList.Count; ++index1)
        {
            Int32 index2 = index1 - 1;
            Int32 index3 = index1 + 1;
            if (index1 == 0)
                index2 = characterDetailHudList.Count - 1;
            if (index1 == characterDetailHudList.Count - 1)
                index3 = 0;
            UIKeyNavigation component = characterDetailHudList[index1].Self.GetComponent<UIKeyNavigation>();
            component.onUp = characterDetailHudList[index2].Self;
            component.onDown = characterDetailHudList[index3].Self;
        }
    }

    private void ArrangeAuto()
    {
        FF9ITEM[] ff9ItemArray = new FF9ITEM[256];
        Int32 num = 0;
        for (Int32 index = 0; index < 256; ++index)
        {
            if (FF9StateSystem.Common.FF9.item[index].count != 0 && FF9StateSystem.Common.FF9.item[index].id != Byte.MaxValue)
                ff9ItemArray[num++] = new FF9ITEM(FF9StateSystem.Common.FF9.item[index].id, FF9StateSystem.Common.FF9.item[index].count);
        }
        for (Int32 index1 = 0; index1 < num - 1; ++index1)
        {
            for (Int32 index2 = index1 + 1; index2 < num; ++index2)
            {
                FF9ITEM_DATA ff9ItemData1 = ff9item._FF9Item_Data[ff9ItemArray[index1].id];
                FF9ITEM_DATA ff9ItemData2 = ff9item._FF9Item_Data[ff9ItemArray[index2].id];
                if (ff9ItemData1.sort > ff9ItemData2.sort)
                {
                    FF9ITEM ff9Item = ff9ItemArray[index1];
                    ff9ItemArray[index1] = ff9ItemArray[index2];
                    ff9ItemArray[index2] = ff9Item;
                }
                else if (ff9ItemData1.sort == ff9ItemData2.sort && ff9ItemArray[index1].id > ff9ItemArray[index2].id)
                {
                    FF9ITEM ff9Item = ff9ItemArray[index1];
                    ff9ItemArray[index1] = ff9ItemArray[index2];
                    ff9ItemArray[index2] = ff9Item;
                }
            }
        }
        for (Int32 index = 0; index < num; ++index)
        {
            FF9StateSystem.Common.FF9.item[index].id = ff9ItemArray[index].id;
            FF9StateSystem.Common.FF9.item[index].count = ff9ItemArray[index].count;
        }
        for (Int32 index = num; index < 256; ++index)
            FF9StateSystem.Common.FF9.item[index].count = 0;
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
        if (PersistenSingleton<FF9StateSystem>.Instance.mode != 3 || FF9FITEM_CHOCOBO_NONE == FF9StateSystem.EventState.gEventGlobal[FF9FITEM_EVENT_KIND_CHOCOBO] || (FF9StateSystem.EventState.gEventGlobal[FF9FITEM_EVENT_VEGETABLE] != 0 || !ff9.w_frameChocoboCheck()))
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

        SceneVoidDelegate fAmCache3 = () =>
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.SubMenuPanel.SetActive(false);
            PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.WorldHUD);
            PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = true;
            PersistenSingleton<UIManager>.Instance.MainMenuScene.ScreenFadeGameObject.GetParent().GetComponent<UIPanel>().depth = 10;
        };

        Hide(fAmCache3);
    }

    private void Awake()
    {
        FadingComponent = ScreenFadeGameObject.GetComponent<HonoFading>();
        UIEventListener uiEventListener1 = UIEventListener.Get(UseSubMenu);
        uiEventListener1.Click += onClick;

        UIEventListener uiEventListener2 = UIEventListener.Get(ArrangeSubMenu);
        uiEventListener2.Click += onClick;

        UIEventListener uiEventListener3 = UIEventListener.Get(KeySubMenu);
        uiEventListener3.Click += onClick;

        foreach (Component component in TargetListPanel.GetChild(0).transform)
        {
            GameObject go = component.gameObject;

            UIEventListener uiEventListener4 = UIEventListener.Get(go);
            uiEventListener4.Click += onClick;

            _targetHudList.Add(new CharacterDetailHUD(go, true));
            if (FF9StateSystem.MobilePlatform)
                go.GetComponent<ButtonGroupState>().Help.TextKey = "TargetHelpMobile";
        }

        //this._importantItemHitArea = KeyItemDetailPanel.GetChild(0);
        _keyItemDetailName = KeyItemDetailPanel.GetChild(1).GetComponent<UILabel>();
        _keyItemDetailDescription = KeyItemDetailPanel.GetChild(2).GetComponent<UILabel>();
        _keyItemDetailDescription.PrintIconAfterProcessedText = true;
        _defaultSkinLabelSpacingY = _keyItemDetailDescription.spacingY;
        _itemScrollList = ItemListPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        _keyItemScrollList = KeyItemListPanel.GetChild(1).GetComponent<RecycleListPopulator>();

        RemoveLeftAnchorFromItemNumberLabels();

        UIEventListener uiEventListener5 = UIEventListener.Get(ArrangeDialog.GetChild(0).GetChild(0));
        uiEventListener5.Click += onClick;

        UIEventListener uiEventListener6 = UIEventListener.Get(ArrangeDialog.GetChild(0).GetChild(1));
        uiEventListener6.Click += onClick;

        _targetTransition = TransitionGroup.GetChild(0).GetComponent<HonoTweenPosition>();
        _keyItemSkinTransition = TransitionGroup.GetChild(1).GetComponent<HonoTweenPosition>();
        _arrangeTransition = TransitionGroup.GetChild(2).GetComponent<HonoTweenClipping>();
    }

    private void RemoveLeftAnchorFromItemNumberLabels()
    {
        GameObject itemListCaptionBackgroundPanel = ItemListPanel.GetChild(2).GetChild(4);
        foreach (UILabel label in itemListCaptionBackgroundPanel.GetComponentsInChildren<UILabel>(true))
        {
            if (label.text == "NUM")
            {
                label.leftAnchor.absolute -= 90;
                label.UpdateAnchors();
                return;
            }
        }
    }

    private struct KeyItemDetailHUD
    {
        //public GameObject Self;
        public ButtonGroupState Button;
        public UILabel NameLabel;
        public UISprite NewIconSprite;
        public UISprite NewIconLabelSprite;

        public KeyItemDetailHUD(GameObject go)
        {
            //Self = go;
            Button = go.GetComponent<ButtonGroupState>();
            NameLabel = go.GetChild(0).GetComponent<UILabel>();
            NewIconSprite = go.GetChild(1).GetChild(0).GetComponent<UISprite>();
            NewIconLabelSprite = go.GetChild(1).GetChild(1).GetComponent<UISprite>();
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
        public Int32 ItemId;
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