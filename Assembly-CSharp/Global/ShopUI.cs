using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopUI : UIScene
{
    public Int32 Id
    {
        get => this.id;
        set => this.id = value;
    }

    public ShopUI.ShopType Type
    {
        get => this.type;
        set => this.type = value;
    }

    public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate afterShowAction = delegate
        {
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(50f, 0f), ShopUI.ItemGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(50f, 0f), ShopUI.WeaponGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(50f, 0f), ShopUI.SellItemGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(this.ItemListPanel.GetComponent<UIWidget>(), this.shopItemScrollList.cellHeight, ShopUI.ItemGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(this.WeaponListPanel.GetComponent<UIWidget>(), this.shopWeaponScrollList.cellHeight, ShopUI.WeaponGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(this.SellItemListPanel.GetComponent<UIWidget>(), this.shopSellItemScrollList.cellHeight, ShopUI.SellItemGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(this.shopItemScrollList.ScrollButton, ShopUI.ItemGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(this.shopWeaponScrollList.ScrollButton, ShopUI.WeaponGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(this.shopSellItemScrollList.ScrollButton, ShopUI.SellItemGroupButton);
            ButtonGroupState.SetPointerDepthToGroup(4, ShopUI.SellItemGroupButton);
            if (this.type == ShopUI.ShopType.Synthesis) // No Buy/Sell options: directly select items
                ButtonGroupState.ActiveGroup = ShopUI.WeaponGroupButton;
            else
                ButtonGroupState.ActiveGroup = ShopUI.SubMenuGroupButton;
        };
        if (afterFinished != null)
            afterShowAction += afterFinished;
        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(afterShowAction);
        this.UpdateUserInterface();
        this.InitializeData();
        if (this.type != ShopUI.ShopType.Synthesis)
        {
            this.NonSynthesisControlPanel.SetActive(true);
            this.SynthesisControlPanel.SetActive(false);
        }
        else
        {
            this.NonSynthesisControlPanel.SetActive(false);
            this.SynthesisControlPanel.SetActive(true);
        }
        this.SetShopType(this.type);
        this.shopItemScrollList.ScrollButton.DisplayScrollButton(false, false);
        this.shopWeaponScrollList.ScrollButton.DisplayScrollButton(false, false);
        this.shopSellItemScrollList.ScrollButton.DisplayScrollButton(false, false);
    }

    public void UpdateUserInterface()
    {
        if (!Configuration.Interface.IsEnabled)
            return;
        _shopItemPanel.ScrollButton.Panel.alpha = 0.5f;
        _shopWeaponPanel.ScrollButton.Panel.alpha = 0.5f;
        _shopSellItemPanel.ScrollButton.Panel.alpha = 0.5f;
        const Int32 originalLineCount = 8;
        Int32 originalColumnCount = 1;
        const Int32 reducedOriginalLineCount = 5;
        const Single buttonOriginalHeight = 98f;
        const Single buyPanelOriginalWidth = 916f;
        const Single panelOriginalHeight = originalLineCount * buttonOriginalHeight;
        const Single panelReducedOriginalHeight = reducedOriginalLineCount * buttonOriginalHeight;

        // ----------- SHOP CATALOG ----------- //

        Int32 linePerPage = Configuration.Interface.MenuItemRowCount;
        Int32 columnPerPage = (Int32)Math.Floor((Single)(originalColumnCount * ((Single)linePerPage / originalLineCount)));
        if (originalColumnCount * originalLineCount >= this.itemIdList.Count) // 1 columns suffice
        {
            linePerPage = originalLineCount;
            columnPerPage = originalColumnCount;
        }

        Int32 lineHeight = (Int32)Math.Round(panelOriginalHeight / linePerPage);
        Int32 reducedLinePerPage = (Int32)Math.Round(linePerPage * 0.65f);
        Int32 reducedLineHeight = (Int32)Math.Round(panelReducedOriginalHeight / reducedLinePerPage);
        Single scaleFactor = lineHeight / buttonOriginalHeight;


        Single alphaColumnTitles = (columnPerPage > originalColumnCount) ? 0f : 1f;
        _shopItemPanel.Background.Panel.Name.Label.alpha = alphaColumnTitles;
        _shopItemPanel.Background.Panel.Info.Label.alpha = alphaColumnTitles;
        _shopWeaponPanel.Background.Panel.Name.Label.alpha = alphaColumnTitles;
        _shopWeaponPanel.Background.Panel.Info.Label.alpha = alphaColumnTitles;

        _shopItemPanel.SubPanel.ChangeDims(columnPerPage, linePerPage, buyPanelOriginalWidth / columnPerPage, lineHeight);
        _shopItemPanel.SubPanel.ButtonPrefab.IconSprite.SetAnchor(target: _shopItemPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.085f, relRight: 0.155f);
        _shopItemPanel.SubPanel.ButtonPrefab.IconSprite.width = _shopItemPanel.SubPanel.ButtonPrefab.IconSprite.height;

        _shopItemPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _shopItemPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.175f, relRight: 0.65f);
        _shopItemPanel.SubPanel.ButtonPrefab.NameLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _shopItemPanel.SubPanel.ButtonPrefab.NameLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));

        _shopItemPanel.SubPanel.ButtonPrefab.NumberLabel.SetAnchor(target: _shopItemPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.65f, relRight: 0.92f);
        _shopItemPanel.SubPanel.ButtonPrefab.NumberLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _shopItemPanel.SubPanel.ButtonPrefab.NumberLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));
        _shopItemPanel.SubPanel.RecycleListPopulator.RefreshTableView();

        _shopWeaponPanel.SubPanel.ChangeDims(columnPerPage, reducedLinePerPage, buyPanelOriginalWidth / columnPerPage, reducedLineHeight);
        _shopWeaponPanel.SubPanel.ButtonPrefab.IconSprite.SetAnchor(target: _shopWeaponPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.085f, relRight: 0.155f);
        _shopWeaponPanel.SubPanel.ButtonPrefab.IconSprite.width = _shopWeaponPanel.SubPanel.ButtonPrefab.IconSprite.height;

        _shopWeaponPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _shopWeaponPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.175f, relRight: 0.65f);
        _shopWeaponPanel.SubPanel.ButtonPrefab.NameLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _shopWeaponPanel.SubPanel.ButtonPrefab.NameLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));

        _shopWeaponPanel.SubPanel.ButtonPrefab.NumberLabel.SetAnchor(target: _shopWeaponPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.65f, relRight: 0.92f);
        _shopWeaponPanel.SubPanel.ButtonPrefab.NumberLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _shopWeaponPanel.SubPanel.ButtonPrefab.NumberLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));
        _shopWeaponPanel.SubPanel.RecycleListPopulator.RefreshTableView();

        // ----------- SHOP SELLING ----------- //

        originalColumnCount = 2;
        linePerPage = Configuration.Interface.MenuItemRowCount;
        columnPerPage = (Int32)Math.Floor((Single)(originalColumnCount * ((Single)linePerPage / originalLineCount)));
        if (originalColumnCount * originalLineCount >= this.sellItemIdList.Count) // 2 columns suffice
        {
            linePerPage = originalLineCount;
            columnPerPage = originalColumnCount;
        }
        else if (linePerPage >= originalLineCount * 2 && (originalColumnCount + 1) * (originalLineCount + originalLineCount / originalColumnCount) >= this.sellItemIdList.Count) // 3 columns suffice
        {
            linePerPage = originalLineCount + originalLineCount / originalColumnCount;
            columnPerPage = originalColumnCount + 1;
        }

        lineHeight = (Int32)Math.Round(panelOriginalHeight / linePerPage);
        scaleFactor = lineHeight / buttonOriginalHeight;

        alphaColumnTitles = (columnPerPage > originalColumnCount) ? 0f : 1f;
        _shopSellItemPanel.Background.Panel.Name.Label.alpha = alphaColumnTitles;
        _shopSellItemPanel.Background.Panel.Info.Label.alpha = alphaColumnTitles;
        _shopSellItemPanel.Background.Panel.Name2.Label.alpha = alphaColumnTitles;
        _shopSellItemPanel.Background.Panel.Info2.Label.alpha = alphaColumnTitles;

        const Single sellPanelOriginalWidth = 1490f;
        _shopSellItemPanel.SubPanel.ChangeDims(columnPerPage, linePerPage, sellPanelOriginalWidth / columnPerPage, lineHeight);
        _shopSellItemPanel.SubPanel.ButtonPrefab.IconSprite.SetAnchor(target: _shopSellItemPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.105f, relRight: 0.191f);
        _shopSellItemPanel.SubPanel.ButtonPrefab.IconSprite.width = _shopSellItemPanel.SubPanel.ButtonPrefab.IconSprite.height;

        _shopSellItemPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _shopSellItemPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.215f, relRight: 0.795f);
        _shopSellItemPanel.SubPanel.ButtonPrefab.NameLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _shopSellItemPanel.SubPanel.ButtonPrefab.NameLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));

        _shopSellItemPanel.SubPanel.ButtonPrefab.NumberLabel.SetAnchor(target: _shopSellItemPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.184f, relTop: 0.816f, relLeft: 0.8f, relRight: 0.9f);
        _shopSellItemPanel.SubPanel.ButtonPrefab.NumberLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _shopSellItemPanel.SubPanel.ButtonPrefab.NumberLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));
        _shopSellItemPanel.SubPanel.RecycleListPopulator.RefreshTableView();
    }

    public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate afterHideAction = delegate
        {
            MainMenuUI.UIControlPanel?.ExitMenu();
            PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
        };
        if (afterFinished != null)
            afterHideAction += afterFinished;
        base.Hide(afterHideAction);
        this.RemoveCursorMemorize();
        this.soldItemIdDict.Clear();
    }

    private void RemoveCursorMemorize()
    {
        ButtonGroupState.RemoveCursorMemorize(ShopUI.SubMenuGroupButton);
        ButtonGroupState.RemoveCursorMemorize(ShopUI.ItemGroupButton);
        ButtonGroupState.RemoveCursorMemorize(ShopUI.WeaponGroupButton);
        ButtonGroupState.RemoveCursorMemorize(ShopUI.SellItemGroupButton);
    }

    private void InitializeData()
    {
        this.currentItemIndex = -1;
        this.availableCharaStart = 0;
        this.currentMenu = ShopUI.SubMenu.Buy;
        this.itemIdList.Clear();
        this.isItemEnableList.Clear();
        this.availableCharaList.Clear();
        this.mixItemList.Clear();
        this.type = ff9shop.FF9Shop_GetType(this.id);
        this.isGrocery = false;
        this.InitializeMixList();
        if (this.type == ShopUI.ShopType.Item)
        {
            ShopItems assortiment = ff9buy.ShopItems[this.id];
            if (this.mixItemList.Count > 0 || assortiment.ItemIds.Any(itemId => (ff9item._FF9Item_Data[itemId].type & ff9buy.FF9BUY_TYPE_WEAPON) != 0))
            {
                this.isGrocery = true;
                this.type = ShopUI.ShopType.Weapon;
            }
        }
        if (this.type != ShopUI.ShopType.Item)
            this.UpdatePartyInfo();
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go))
        {
            if (ButtonGroupState.ActiveGroup == ShopUI.SubMenuGroupButton)
            {
                this.currentMenu = this.GetSubMenuFromGameObject(go);
                if (this.currentMenu == ShopUI.SubMenu.Sell)
                {
                    if (this.sellItemIdList.Count != 0)
                    {
                        FF9Sfx.FF9SFX_Play(103);
                        this.currentItemIndex = 0;
                        this.shopSellItemScrollList.JumpToIndex(0, false);
                        ButtonGroupState.RemoveCursorMemorize(ShopUI.SellItemGroupButton);
                        ButtonGroupState.ActiveGroup = ShopUI.SellItemGroupButton;
                    }
                    else
                    {
                        FF9Sfx.FF9SFX_Play(102);
                    }
                }
                else if (this.type == ShopUI.ShopType.Item)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    this.currentItemIndex = 0;
                    this.DisplayInfo(this.currentMenu != ShopUI.SubMenu.Sell ? this.Type : ShopUI.ShopType.Sell);
                    this.shopItemScrollList.JumpToIndex(0, false);
                    ButtonGroupState.RemoveCursorMemorize(ShopUI.ItemGroupButton);
                    ButtonGroupState.ActiveGroup = ShopUI.ItemGroupButton;
                }
                else
                {
                    FF9Sfx.FF9SFX_Play(103);
                    this.currentItemIndex = 0;
                    this.DisplayInfo(this.currentMenu != ShopUI.SubMenu.Sell ? this.Type : ShopUI.ShopType.Sell);
                    this.DisplayCharacterInfo();
                    this.shopWeaponScrollList.JumpToIndex(0, false);
                    ButtonGroupState.RemoveCursorMemorize(ShopUI.WeaponGroupButton);
                    ButtonGroupState.ActiveGroup = ShopUI.WeaponGroupButton;
                }
                ButtonGroupState.SetSecondaryOnGroup(ShopUI.SubMenuGroupButton);
                ButtonGroupState.HoldActiveStateOnGroup(ShopUI.SubMenuGroupButton);
            }
            else if (ButtonGroupState.ActiveGroup == ShopUI.ItemGroupButton)
            {
                if (ButtonGroupState.ContainButtonInGroup(go, ShopUI.ItemGroupButton))
                {
                    this.currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                    if (this.isItemEnableList[this.currentItemIndex])
                    {
                        FF9Sfx.FF9SFX_Play(103);
                        this.StartCountItem();
                        this.DisplayConfirmDialog(this.Type);
                        ButtonGroupState.ActiveGroup = ShopUI.QuantityGroupButton;
                        ButtonGroupState.HoldActiveStateOnGroup(ShopUI.ItemGroupButton);
                    }
                    else
                    {
                        FF9Sfx.FF9SFX_Play(102);
                    }
                }
                else
                {
                    this.OnSecondaryGroupClick(go);
                }
            }
            else if (ButtonGroupState.ActiveGroup == ShopUI.WeaponGroupButton)
            {
                if (ButtonGroupState.ContainButtonInGroup(go, ShopUI.WeaponGroupButton))
                {
                    this.currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                    if (this.isItemEnableList[this.currentItemIndex])
                    {
                        FF9Sfx.FF9SFX_Play(103);
                        if (this.type == ShopUI.ShopType.Weapon)
                            this.StartCountItem();
                        else
                            this.StartCountMix();
                        this.DisplayConfirmDialog(this.Type);
                        ButtonGroupState.ActiveGroup = ShopUI.QuantityGroupButton;
                        ButtonGroupState.HoldActiveStateOnGroup(ShopUI.WeaponGroupButton);
                    }
                    else
                    {
                        FF9Sfx.FF9SFX_Play(102);
                    }
                }
                else
                {
                    this.OnSecondaryGroupClick(go);
                }
            }
            else if (ButtonGroupState.ActiveGroup == ShopUI.SellItemGroupButton)
            {
                if (ButtonGroupState.ContainButtonInGroup(go, ShopUI.SellItemGroupButton))
                {
                    this.currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                    RegularItem sellingItem = this.sellItemIdList[this.currentItemIndex];
                    if (!this.soldItemIdDict.ContainsKey(sellingItem) && ff9item._FF9Item_Data[sellingItem].selling_price >= 0)
                    {
                        FF9Sfx.FF9SFX_Play(103);
                        this.StartCountSell();
                        this.DisplayConfirmDialog(ShopUI.ShopType.Sell);
                        ButtonGroupState.ActiveGroup = ShopUI.QuantityGroupButton;
                        ButtonGroupState.HoldActiveStateOnGroup(ShopUI.SellItemGroupButton);
                    }
                    else
                    {
                        FF9Sfx.FF9SFX_Play(102);
                    }
                }
                else
                {
                    this.OnSecondaryGroupClick(go);
                }
            }
            else if (ButtonGroupState.ActiveGroup == ShopUI.QuantityGroupButton && ButtonGroupState.ContainButtonInGroup(go, ShopUI.QuantityGroupButton))
            {
                this.ClearConfirmDialog();
                if (this.currentMenu == ShopUI.SubMenu.Buy)
                {
                    if (this.type == ShopUI.ShopType.Item)
                    {
                        FF9Sfx.FF9SFX_Play(1045);
                        Int32 countAdded = ff9item.FF9Item_Add(this.itemIdList[this.currentItemIndex], this.count);
                        FF9ITEM_DATA item = ff9item._FF9Item_Data[this.itemIdList[this.currentItemIndex]];
                        if (countAdded != 0)
                        {
                            FF9StateSystem.Common.FF9.party.gil -= (UInt32)(item.price * countAdded);
                            this.DisplayInfo(this.currentMenu != ShopUI.SubMenu.Sell ? this.Type : ShopUI.ShopType.Sell);
                            this.DisplayItem();
                            ButtonGroupState.ActiveGroup = ShopUI.ItemGroupButton;
                        }
                    }
                    else if (this.type == ShopUI.ShopType.Weapon)
                    {
                        FF9Sfx.FF9SFX_Play(1045);
                        Int32 countAdded = ff9item.FF9Item_Add(this.itemIdList[this.currentItemIndex], this.count);
                        FF9ITEM_DATA item = ff9item._FF9Item_Data[this.itemIdList[this.currentItemIndex]];
                        if (countAdded != 0)
                        {
                            FF9StateSystem.Common.FF9.party.gil -= (UInt32)(item.price * countAdded);
                            this.DisplayInfo(this.currentMenu != ShopUI.SubMenu.Sell ? this.Type : ShopUI.ShopType.Sell);
                            this.DisplayWeapon();
                            ButtonGroupState.ActiveGroup = ShopUI.WeaponGroupButton;
                        }
                    }
                    else if (this.type == ShopUI.ShopType.Synthesis)
                    {
                        FF9Sfx.FF9SFX_Play(1045);
                        FF9MIX_DATA synth = this.mixItemList[this.currentItemIndex - this.mixStartIndex];
                        Int32 countAdded = ff9item.FF9Item_Add(synth.Result, this.count);
                        if (countAdded != 0)
                        {
                            foreach (RegularItem ingr in synth.Ingredients.Where(ingr => ingr != RegularItem.NoItem))
                                ff9item.FF9Item_Remove(ingr, countAdded);
                            FF9StateSystem.Common.FF9.party.gil -= (UInt32)(synth.Price * countAdded);
                            this.DisplayInfo(this.currentMenu != ShopUI.SubMenu.Sell ? this.Type : ShopUI.ShopType.Sell);
                            this.UpdateWeaponOrSynthType(true);
                            this.DisplayWeapon();
                            this.UpdateWeaponOrSynthType();
                            ButtonGroupState.ActiveGroup = ShopUI.WeaponGroupButton;
                            FF9StateSystem.Achievement.synthesisCount++;
                            AchievementManager.ReportAchievement(AcheivementKey.Synth10, FF9StateSystem.Achievement.synthesisCount);
                            AchievementManager.ReportAchievement(AcheivementKey.Synth30, FF9StateSystem.Achievement.synthesisCount);
                        }
                    }
                }
                else
                {
                    RegularItem sellingItem = this.sellItemIdList[this.currentItemIndex];
                    FF9ITEM_DATA item = ff9item._FF9Item_Data[sellingItem];
                    Int32 sellingPrice = item.selling_price;
                    if (sellingPrice < 0)
                    {
                        FF9Sfx.FF9SFX_Play(102);
                        return true;
                    }
                    FF9Sfx.FF9SFX_Play(1045);
                    Int32 countRemoved = ff9item.FF9Item_Remove(sellingItem, this.count);
                    if (countRemoved != 0)
                    {
                        FF9StateSystem.Common.FF9.party.gil += (UInt32)(sellingPrice * this.count);
                        if (FF9StateSystem.Common.FF9.party.gil > 9999999u)
                            FF9StateSystem.Common.FF9.party.gil = 9999999u;
                    }
                    if (ff9item.FF9Item_GetCount(sellingItem) == 0)
                        this.soldItemIdDict[sellingItem] = this.currentItemIndex;
                    this.DisplaySellItem();
                    if (this.sellItemIdList.Count == 0)
                    {
                        Singleton<PointerManager>.Instance.RemoveAllPointer();
                        ButtonGroupState.ActiveGroup = ShopUI.SubMenuGroupButton;
                    }
                    else
                    {
                        ButtonGroupState.ActiveGroup = ShopUI.SellItemGroupButton;
                    }
                }
            }
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go))
        {
            if (ButtonGroupState.ActiveGroup == ShopUI.SubMenuGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.Hide(delegate
                {
                    PersistenSingleton<UIManager>.Instance.ChangeUIState(PersistenSingleton<UIManager>.Instance.HUDState);
                });
            }
            else if (ButtonGroupState.ActiveGroup == ShopUI.ItemGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.ClearInfo(this.type);
                ButtonGroupState.ActiveGroup = ShopUI.SubMenuGroupButton;
            }
            else if (ButtonGroupState.ActiveGroup == ShopUI.WeaponGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.UpdateWeaponOrSynthType(true);
                if (this.Type != ShopUI.ShopType.Synthesis)
                {
                    this.ClearCharacterInfo();
                    this.ClearInfo(this.type);
                    ButtonGroupState.ActiveGroup = ShopUI.SubMenuGroupButton;
                }
                else
                {
                    this.Hide(delegate
                    {
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(PersistenSingleton<UIManager>.Instance.HUDState);
                    });
                }
            }
            else if (ButtonGroupState.ActiveGroup == ShopUI.SellItemGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                ButtonGroupState.ActiveGroup = ShopUI.SubMenuGroupButton;
            }
            else if (ButtonGroupState.ActiveGroup == ShopUI.QuantityGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.ClearConfirmDialog();
                if (this.currentMenu == ShopUI.SubMenu.Buy)
                {
                    if (this.type == ShopUI.ShopType.Item)
                        ButtonGroupState.ActiveGroup = ShopUI.ItemGroupButton;
                    else if (this.type == ShopUI.ShopType.Weapon || this.type == ShopUI.ShopType.Synthesis)
                        ButtonGroupState.ActiveGroup = ShopUI.WeaponGroupButton;
                }
                else
                {
                    ButtonGroupState.ActiveGroup = ShopUI.SellItemGroupButton;
                }
            }
        }
        return true;
    }

    public override Boolean OnKeySelect(GameObject go)
    {
        return ButtonGroupState.ActiveGroup != ShopUI.QuantityGroupButton && base.OnKeySelect(go);
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (base.OnItemSelect(go))
        {
            if (ButtonGroupState.ActiveGroup == ShopUI.SubMenuGroupButton)
            {
                if (this.currentMenu != this.GetSubMenuFromGameObject(go))
                {
                    this.currentMenu = this.GetSubMenuFromGameObject(go);
                    ShopUI.SubMenu subMenu = this.currentMenu;
                    if (subMenu == ShopUI.SubMenu.Buy)
                        this.SetShopType(this.Type);
                    else if (subMenu == ShopUI.SubMenu.Sell)
                        this.SetShopType(ShopUI.ShopType.Sell);
                }
            }
            else if (ButtonGroupState.ActiveGroup == ShopUI.ItemGroupButton)
            {
                if (this.currentItemIndex != go.GetComponent<RecycleListItem>().ItemDataIndex)
                {
                    this.currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                    this.DisplayInfo(this.currentMenu != ShopUI.SubMenu.Sell ? this.Type : ShopUI.ShopType.Sell);
                }
            }
            else if (ButtonGroupState.ActiveGroup == ShopUI.WeaponGroupButton)
            {
                if (this.currentItemIndex != go.GetComponent<RecycleListItem>().ItemDataIndex)
                {
                    this.currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                    this.UpdateWeaponOrSynthType();
                    this.DisplayInfo(this.currentMenu != ShopUI.SubMenu.Sell ? this.Type : ShopUI.ShopType.Sell);
                    this.DisplayCharacterInfo();
                }
            }
            else if (ButtonGroupState.ActiveGroup == ShopUI.SellItemGroupButton && this.currentItemIndex != go.GetComponent<RecycleListItem>().ItemDataIndex)
            {
                this.currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
            }
        }
        return true;
    }

    private void OnKeyQuantity(GameObject go, KeyCode key)
    {
        if (ButtonGroupState.ActiveGroup == ShopUI.QuantityGroupButton)
        {
            if (key == KeyCode.LeftArrow)
                this.MinusQuantity(1);
            else if (key == KeyCode.DownArrow)
                this.MinusQuantity(10);
            else if (key == KeyCode.RightArrow)
                this.PlusQuantity(1);
            else if (key == KeyCode.UpArrow)
                this.PlusQuantity(10);
        }
    }

    private void onPressMinus(GameObject go, Boolean isPress)
    {
        if (isPress)
        {
            this.isMinusQuantity = true;
        }
        else
        {
            this.triggerCounter = ShopUI.TRIGGER_DURATION;
            this.keepPressingTime = 0f;
            this.isMinusQuantity = false;
        }
    }

    private void onPressPlus(GameObject go, Boolean isPress)
    {
        if (isPress)
        {
            this.isPlusQuantity = true;
        }
        else
        {
            this.triggerCounter = ShopUI.TRIGGER_DURATION;
            this.keepPressingTime = 0f;
            this.isPlusQuantity = false;
        }
    }

    private void onClickMinus(GameObject go)
    {
        this.MinusQuantity(1);
        this.triggerCounter = ShopUI.TRIGGER_DURATION;
    }

    private void onClickPlus(GameObject go)
    {
        this.PlusQuantity(1);
        this.triggerCounter = ShopUI.TRIGGER_DURATION;
    }

    private void OnSecondaryGroupClick(GameObject go)
    {
        ButtonGroupState.HoldActiveStateOnGroup(go, ShopUI.SubMenuGroupButton);
        if (ButtonGroupState.ActiveGroup == ShopUI.ItemGroupButton)
        {
            FF9Sfx.muteSfx = true;
            this.OnKeyCancel(this.shopItemScrollList.GetItem(this.currentItemIndex).gameObject);
            FF9Sfx.muteSfx = false;
            this.OnKeyConfirm(go);
        }
        else if (ButtonGroupState.ActiveGroup == ShopUI.WeaponGroupButton)
        {
            FF9Sfx.muteSfx = true;
            this.OnKeyCancel(this.shopWeaponScrollList.GetItem(this.currentItemIndex).gameObject);
            FF9Sfx.muteSfx = false;
            this.OnKeyConfirm(go);
        }
        else if (ButtonGroupState.ActiveGroup == ShopUI.SellItemGroupButton)
        {
            FF9Sfx.muteSfx = true;
            this.OnKeyCancel(this.shopSellItemScrollList.GetItem(this.currentItemIndex).gameObject);
            FF9Sfx.muteSfx = false;
            this.OnKeyConfirm(go);
        }
    }

    public override Boolean OnKeyLeftTrigger(GameObject go)
    {
        if (this.availableCharaStart > 0)
        {
            this.availableCharaStart--;
            this.DisplayCharacterInfo();
        }
        return true;
    }

    public override Boolean OnKeyRightTrigger(GameObject go)
    {
        if (this.availableCharaStart + this.charInfoHud.Count < this.availableCharaList.Count)
        {
            this.availableCharaStart++;
            this.DisplayCharacterInfo();
        }
        return true;
    }

    private void DisplayHelpPanel(ShopUI.ShopType shopType)
    {
        switch (shopType)
        {
            case ShopUI.ShopType.Item:
                FF9UIDataTool.DisplayTextLocalize(this.ShopTitleLabel, "ItemShop");
                FF9UIDataTool.DisplayTextLocalize(this.HelpLabel, "BuyHelp");
                break;
            case ShopUI.ShopType.Weapon:
                if (this.isGrocery)
                {
                    FF9UIDataTool.DisplayTextLocalize(this.ShopTitleLabel, "ItemShop");
                    FF9UIDataTool.DisplayTextLocalize(this.HelpLabel, "BuyHelp");
                }
                else
                {
                    FF9UIDataTool.DisplayTextLocalize(this.ShopTitleLabel, "WeaponShop");
                    FF9UIDataTool.DisplayTextLocalize(this.HelpLabel, "BuyHelp");
                }
                break;
            case ShopUI.ShopType.Synthesis:
                FF9UIDataTool.DisplayTextLocalize(this.ShopTitleLabel, "SynthesisShop");
                FF9UIDataTool.DisplayTextLocalize(this.HelpLabel, "SynthHelp");
                break;
            case ShopUI.ShopType.Sell:
                FF9UIDataTool.DisplayTextLocalize(this.HelpLabel, "SellHelp");
                break;
        }
    }

    private void DisplayInfo(ShopUI.ShopType shopType)
    {
        if (shopType == ShopUI.ShopType.Item)
        {
            RegularItem itemId = this.itemIdList[this.currentItemIndex];
            this.itemFundLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", FF9StateSystem.Common.FF9.party.gil.ToString());
            this.itemCountLabel.text = ff9item.FF9Item_GetCount(itemId).ToString();
            this.requiredItem1Hud.Self.SetActive(false);
            this.requiredItem2Hud.Self.SetActive(false);
        }
        else if (shopType == ShopUI.ShopType.Weapon)
        {
            RegularItem itemId = this.itemIdList[this.currentItemIndex];
            this.weaponFundLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", FF9StateSystem.Common.FF9.party.gil.ToString());
            this.weaponCountLabel.text = ff9item.FF9Item_GetCount(this.itemIdList[this.currentItemIndex]).ToString();
            this.weaponEquipLabel.text = ff9item.FF9Item_GetEquipCount(this.itemIdList[this.currentItemIndex]).ToString();
            if ((ff9item._FF9Item_Data[itemId].type & ItemType.AnyEquipment) != 0)
            {
                this.weaponEquipLabel.color = FF9TextTool.White;
                this.weaponEquipTextLabel.color = FF9TextTool.White;
            }
            else
            {
                this.weaponEquipLabel.color = FF9TextTool.Gray;
                this.weaponEquipTextLabel.color = FF9TextTool.Gray;
            }
            this.requiredItem1Hud.Self.SetActive(false);
            this.requiredItem2Hud.Self.SetActive(false);
        }
        else if (shopType == ShopUI.ShopType.Synthesis)
        {
            FF9MIX_DATA synth = this.mixItemList[this.currentItemIndex - this.mixStartIndex];
            this.weaponFundLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", FF9StateSystem.Common.FF9.party.gil.ToString());
            this.weaponCountLabel.text = ff9item.FF9Item_GetCount(synth.Result).ToString();
            this.weaponEquipLabel.text = ff9item.FF9Item_GetEquipCount(synth.Result).ToString();
            if ((ff9item._FF9Item_Data[synth.Result].type & ItemType.AnyEquipment) != 0)
            {
                this.weaponEquipLabel.color = FF9TextTool.White;
                this.weaponEquipTextLabel.color = FF9TextTool.White;
            }
            else
            {
                this.weaponEquipLabel.color = FF9TextTool.Gray;
                this.weaponEquipTextLabel.color = FF9TextTool.Gray;
            }
            ItemListDetailWithIconHUD[] hud = new ItemListDetailWithIconHUD[] { this.requiredItem1Hud, this.requiredItem2Hud };
            Dictionary<RegularItem, Int32> ingredients = synth.IngredientsAsDictionary();
            if (synth.Ingredients.Length <= 2)
            {
                for (Int32 i = 1; i >= 0; i--)
                {
                    if (i < synth.Ingredients.Length && synth.Ingredients[i] != RegularItem.NoItem)
                    {
                        hud[i].Self.SetActive(true);
                        FF9UIDataTool.DisplayItem(synth.Ingredients[i], hud[i].IconSprite, hud[i].NameLabel, ff9item.FF9Item_GetCount(synth.Ingredients[i]) >= ingredients[synth.Ingredients[i]], true);
                        ingredients[synth.Ingredients[i]]--;
                    }
                    else
                    {
                        hud[i].Self.SetActive(false);
                    }
                }
            }
            else
            {
                Dictionary<RegularItem, Int32>[] ingrSplit = new Dictionary<RegularItem, Int32>[] { new Dictionary<RegularItem, Int32>(), new Dictionary<RegularItem, Int32>() };
                Dictionary<RegularItem, Boolean>[] ingrEnabled = new Dictionary<RegularItem, Boolean>[] { new Dictionary<RegularItem, Boolean>(), new Dictionary<RegularItem, Boolean>() };
                Int32 numInFirstLabel = (ingredients.Count + 1) / 2;
                Int32 ingrNum = 0;
                foreach (KeyValuePair<RegularItem, Int32> kvp in ingredients)
                {
                    Int32 splitIndex = ingrNum < numInFirstLabel ? 0 : 1;
                    ingrSplit[splitIndex].Add(kvp.Key, kvp.Value);
                    ingrEnabled[splitIndex].Add(kvp.Key, ff9item.FF9Item_GetCount(kvp.Key) >= kvp.Value);
                    ingrNum++;
                }
                for (Int32 i = 0; i < 2; i++)
                {
                    hud[i].Self.SetActive(true);
                    FF9UIDataTool.DisplayMultipleItems(ingrSplit[i], hud[i].IconSprite, hud[i].NameLabel, ingrEnabled[i], true);
                }
            }
        }
    }

    private void DisplayItem()
    {
        if (this.currentMenu == ShopUI.SubMenu.Sell)
            return;
        this.itemIdList.Clear();
        this.isItemEnableList.Clear();
        List<ListDataTypeBase> list = new List<ListDataTypeBase>();
        ShopItems assortiment = ff9buy.ShopItems[this.id];

        for (Int32 i = 0; i < assortiment.Length; i++)
        {
            RegularItem itemId = assortiment[i];
            FF9ITEM_DATA item = ff9item._FF9Item_Data[itemId];
            Boolean isEnabled = ff9item.FF9Item_GetCount(itemId) < ff9item.FF9ITEM_COUNT_MAX && FF9StateSystem.Common.FF9.party.gil >= item.price;
            this.isItemEnableList.Add(isEnabled);
            this.itemIdList.Add(itemId);
            list.Add(new ShopUI.ShopItemListData
            {
                Id = itemId,
                Price = item.price,
                Enable = isEnabled
            });
        }
        if (this.shopItemScrollList.ItemsPool.Count == 0)
        {
            this.shopItemScrollList.PopulateListItemWithData = new Action<Transform, ListDataTypeBase, Int32, Boolean>(this.DisplayItemDetail);
            this.shopItemScrollList.OnRecycleListItemClick += this.OnListItemClick;
            this.shopItemScrollList.InitTableView(list, 0);
        }
        else
        {
            this.shopItemScrollList.SetOriginalData(list);
        }
    }

    private void DisplayItemDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        ShopUI.ShopItemListData shopItemListData = (ShopUI.ShopItemListData)data;
        ItemListDetailWithIconHUD itemListDetailWithIconHUD = new ItemListDetailWithIconHUD(item.gameObject, true);
        if (isInit)
            this.DisplayWindowBackground(item.gameObject, null);
        FF9UIDataTool.DisplayItem(shopItemListData.Id, itemListDetailWithIconHUD.IconSprite, itemListDetailWithIconHUD.NameLabel, shopItemListData.Enable);
        itemListDetailWithIconHUD.NumberLabel.text = shopItemListData.Price.ToString();
        if (shopItemListData.Enable)
        {
            itemListDetailWithIconHUD.NameLabel.color = FF9TextTool.White;
            itemListDetailWithIconHUD.NumberLabel.color = FF9TextTool.White;
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, true);
        }
        else
        {
            itemListDetailWithIconHUD.NameLabel.color = FF9TextTool.Gray;
            itemListDetailWithIconHUD.NumberLabel.color = FF9TextTool.Gray;
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, false);
        }
        itemListDetailWithIconHUD.Button.Help.TextKey = String.Empty;
        itemListDetailWithIconHUD.Button.Help.Text = FF9TextTool.ItemHelpDescription(shopItemListData.Id);
    }

    private void DisplayWeapon()
    {
        if (this.currentMenu == ShopUI.SubMenu.Sell)
            return;
        this.itemIdList.Clear();
        this.isItemEnableList.Clear();
        List<ListDataTypeBase> list = new List<ListDataTypeBase>();
        if (this.type == ShopUI.ShopType.Weapon)
        {
            ShopItems assortiment = ff9buy.ShopItems[this.id];

            for (Int32 i = 0; i < assortiment.Length; i++)
            {
                RegularItem itemId = assortiment[i];
                FF9ITEM_DATA item = ff9item._FF9Item_Data[itemId];
                Boolean isEnabled = ff9item.FF9Item_GetCount(itemId) < ff9item.FF9ITEM_COUNT_MAX && FF9StateSystem.Common.FF9.party.gil >= item.price;
                this.isItemEnableList.Add(isEnabled);
                this.itemIdList.Add(itemId);
                list.Add(new ShopUI.ShopItemListData
                {
                    Id = itemId,
                    Price = item.price,
                    Enable = isEnabled
                });
            }
        }
        this.mixStartIndex = list.Count;
        for (Int32 i = 0; i < this.mixItemList.Count; i++)
        {
            FF9MIX_DATA synth = this.mixItemList[i];
            Boolean canBeSynthesized = synth.CanBeSynthesized();
            this.itemIdList.Add(synth.Result);
            this.isItemEnableList.Add(canBeSynthesized);
            list.Add(new ShopUI.ShopItemListData
            {
                Id = synth.Result,
                Price = synth.Price,
                Enable = canBeSynthesized
            });
        }
        if (this.shopWeaponScrollList.ItemsPool.Count == 0)
        {
            this.shopWeaponScrollList.PopulateListItemWithData = new Action<Transform, ListDataTypeBase, Int32, Boolean>(this.DisplayWeaponDetail);
            this.shopWeaponScrollList.OnRecycleListItemClick += this.OnListItemClick;
            this.shopWeaponScrollList.InitTableView(list, 0);
        }
        else
        {
            this.shopWeaponScrollList.SetOriginalData(list);
        }
    }

    private void DisplayWeaponDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        ShopUI.ShopItemListData shopItemListData = (ShopUI.ShopItemListData)data;
        ItemListDetailWithIconHUD itemListDetailWithIconHUD = new ItemListDetailWithIconHUD(item.gameObject, true);
        if (isInit)
            this.DisplayWindowBackground(item.gameObject, null);
        FF9UIDataTool.DisplayItem(shopItemListData.Id, itemListDetailWithIconHUD.IconSprite, itemListDetailWithIconHUD.NameLabel, shopItemListData.Enable);
        itemListDetailWithIconHUD.NumberLabel.text = shopItemListData.Price.ToString();
        if (shopItemListData.Enable)
        {
            itemListDetailWithIconHUD.NameLabel.color = FF9TextTool.White;
            itemListDetailWithIconHUD.NumberLabel.color = FF9TextTool.White;
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, true);
        }
        else
        {
            itemListDetailWithIconHUD.NameLabel.color = FF9TextTool.Gray;
            itemListDetailWithIconHUD.NumberLabel.color = FF9TextTool.Gray;
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, false);
        }
        itemListDetailWithIconHUD.Button.Help.TextKey = String.Empty;
        itemListDetailWithIconHUD.Button.Help.Text = FF9TextTool.ItemHelpDescription(shopItemListData.Id);
    }

    public void DisplaySellItem()
    {
        this.sellItemIdList.Clear();
        List<ListDataTypeBase> list = new List<ListDataTypeBase>();
        foreach (FF9ITEM item in FF9StateSystem.Common.FF9.item)
        {
            if (item.count > 0)
            {
                if (this.soldItemIdDict.ContainsKey(item.id))
                    this.soldItemIdDict.Remove(item.id);
                this.sellItemIdList.Add(item.id);
                list.Add(new ShopUI.ShopSellItemListData
                {
                    Id = item.id,
                    Count = item.count
                });
            }
        }
        foreach (KeyValuePair<RegularItem, Int32> keyValuePair in from key in this.soldItemIdDict
                                                                  orderby key.Value
                                                                  select key)
        {
            this.sellItemIdList.Insert(keyValuePair.Value, keyValuePair.Key);
            ShopUI.ShopSellItemListData sellData = new ShopUI.ShopSellItemListData();
            sellData.Id = keyValuePair.Key;
            sellData.Count = 0;
            list.Insert(keyValuePair.Value, sellData);
        }
        if (this.shopSellItemScrollList.ItemsPool.Count == 0)
        {
            this.shopSellItemScrollList.PopulateListItemWithData = this.DisplaySellItemDetail;
            this.shopSellItemScrollList.OnRecycleListItemClick += this.OnListItemClick;
            this.shopSellItemScrollList.InitTableView(list, 0);
        }
        else
        {
            this.shopSellItemScrollList.SetOriginalData(list);
        }
    }

    private void DisplaySellItemDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        ShopUI.ShopSellItemListData shopSellItemListData = (ShopUI.ShopSellItemListData)data;
        ItemListDetailWithIconHUD itemListDetailWithIconHUD = new ItemListDetailWithIconHUD(item.gameObject, true);
        if (isInit)
            this.DisplayWindowBackground(item.gameObject, null);
        if (shopSellItemListData.Count > 0)
        {
            Boolean canSell = ff9item._FF9Item_Data[shopSellItemListData.Id].selling_price >= 0;
            FF9UIDataTool.DisplayItem(shopSellItemListData.Id, itemListDetailWithIconHUD.IconSprite, itemListDetailWithIconHUD.NameLabel, canSell);
            itemListDetailWithIconHUD.NumberLabel.text = shopSellItemListData.Count.ToString();
            itemListDetailWithIconHUD.NumberLabel.color = canSell ? FF9TextTool.White : FF9TextTool.Gray;
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, true);
        }
        else
        {
            FF9UIDataTool.DisplayItem(shopSellItemListData.Id, itemListDetailWithIconHUD.IconSprite, itemListDetailWithIconHUD.NameLabel, false);
            itemListDetailWithIconHUD.NumberLabel.text = shopSellItemListData.Count.ToString();
            itemListDetailWithIconHUD.NumberLabel.color = FF9TextTool.Gray;
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, false);
        }
        itemListDetailWithIconHUD.Button.Help.TextKey = String.Empty;
        itemListDetailWithIconHUD.Button.Help.Text = FF9TextTool.ItemHelpDescription(shopSellItemListData.Id);
    }

    private void DisplayCharacterInfo()
    {
        this.ClearCharacterInfo();
        if (this.currentItemIndex < 0 || this.currentMenu == ShopUI.SubMenu.Sell)
            return;
        RegularItem currentItemId = this.itemIdList[this.currentItemIndex];
        Int32 equipPart = ff9item.FF9Item_GetEquipPart(currentItemId);
        Boolean isArmor = equipPart > 0;
        if (equipPart < 0)
            return;
        this.characterParamCaptionLabel.text = isArmor ? Localization.Get("DefenseCaption") : Localization.Get("AttackCaption");
        Int32 hudIndex = 0;
        for (Int32 i = this.availableCharaStart; i < this.availableCharaList.Count; i++)
        {
            if (hudIndex >= this.charInfoHud.Count)
                break;
            ShopUI.CharacterWeaponInfoHUD characterWeaponInfoHUD = this.charInfoHud[hudIndex++];
            PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(this.availableCharaList[i]);
            UInt64 playerMask = ff9feqp.GetCharacterEquipMask(player);
            Boolean canEquip = (ff9item._FF9Item_Data[currentItemId].equip & playerMask) != 0;
            characterWeaponInfoHUD.AvatarSprite.gameObject.SetActive(true);
            FF9UIDataTool.DisplayCharacterAvatar(player, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), characterWeaponInfoHUD.AvatarSprite, false);
            if (canEquip)
            {
                Int32 oldRating;
                Int32 newRating;
                if (!isArmor)
                {
                    oldRating = ff9item.GetItemWeapon(player.equip[0]).Ref.Power;
                    newRating = ff9item.GetItemWeapon(currentItemId).Ref.Power;
                }
                else
                {
                    CharacterEquipment equipCopy = player.equip.Clone();
                    equipCopy[equipPart] = currentItemId;
                    oldRating = ff9shop.FF9Shop_GetDefence(equipPart, player.equip);
                    newRating = ff9shop.FF9Shop_GetDefence(equipPart, equipCopy);
                }
                characterWeaponInfoHUD.AvatarSprite.color = new Color(1f, 1f, 1f, 1f);
                characterWeaponInfoHUD.EquipmentTypeSprite.gameObject.SetActive(true);
                characterWeaponInfoHUD.EquipmentTypeSprite.spriteName = "shop_icon_part_" + equipPart.ToString();
                characterWeaponInfoHUD.ParamValueLabel.gameObject.SetActive(true);
                if (equipPart == 4)
                {
                    characterWeaponInfoHUD.ParamValueLabel.text = "???";
                    characterWeaponInfoHUD.ChangeArrowSprite.gameObject.SetActive(false);
                }
                else
                {
                    characterWeaponInfoHUD.ParamValueLabel.text = newRating.ToString();
                    if (oldRating < newRating)
                    {
                        characterWeaponInfoHUD.ChangeArrowSprite.gameObject.SetActive(true);
                        characterWeaponInfoHUD.ChangeArrowSprite.spriteName = "shop_icon_parameter_up";
                    }
                    else if (oldRating > newRating)
                    {
                        characterWeaponInfoHUD.ChangeArrowSprite.gameObject.SetActive(true);
                        characterWeaponInfoHUD.ChangeArrowSprite.spriteName = "shop_icon_parameter_down";
                    }
                    else
                    {
                        characterWeaponInfoHUD.ChangeArrowSprite.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                characterWeaponInfoHUD.AvatarSprite.color = new Color(1f, 1f, 1f, 0.5882353f);
                characterWeaponInfoHUD.ParamValueLabel.gameObject.SetActive(false);
                characterWeaponInfoHUD.ChangeArrowSprite.gameObject.SetActive(false);
                characterWeaponInfoHUD.EquipmentTypeSprite.gameObject.SetActive(false);
            }
        }
    }

    private void DisplayConfirmDialog(ShopUI.ShopType shopType)
    {
        if (shopType != ShopUI.ShopType.Sell)
        {
            this.InputQuantityDialog.SetActive(true);
            this.InputQuantityDialog.transform.localPosition = new Vector3(0f, -16f, 0f);
            this.confirmQuantityLabel.text = this.count.ToString();
            if (shopType == ShopUI.ShopType.Item || shopType == ShopUI.ShopType.Weapon)
            {
                FF9UIDataTool.DisplayTextLocalize(this.HelpLabel, "BuyQtyHelp");
                FF9UIDataTool.DisplayItem(this.itemIdList[this.currentItemIndex], this.confirmItemHud.IconSprite, this.confirmItemHud.NameLabel, true);
                this.confirmPriceLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", (this.count * ff9item._FF9Item_Data[this.itemIdList[this.currentItemIndex]].price).ToString());
            }
            else
            {
                FF9UIDataTool.DisplayTextLocalize(this.HelpLabel, "MixQtyHelp");
                FF9UIDataTool.DisplayItem(this.mixItemList[this.currentItemIndex - this.mixStartIndex].Result, this.confirmItemHud.IconSprite, this.confirmItemHud.NameLabel, true);
                this.confirmPriceLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", (this.count * this.mixItemList[this.currentItemIndex - this.mixStartIndex].Price).ToString());
            }
        }
        else
        {
            RegularItem itemId = this.sellItemIdList[this.currentItemIndex];
            Int32 sellingPrice = ff9item._FF9Item_Data[itemId].selling_price;
            this.InputQuantityDialog.SetActive(true);
            this.InputQuantityDialog.transform.localPosition = new Vector3(0f, 50f, 0f);
            FF9UIDataTool.DisplayTextLocalize(this.HelpLabel, "SellQtyHelp");
            this.InfoDialog.SetActive(true);
            FF9UIDataTool.DisplayItem(itemId, this.confirmItemHud.IconSprite, this.confirmItemHud.NameLabel, true);
            this.confirmQuantityLabel.text = this.count.ToString();
            this.confirmPriceLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", (this.count * sellingPrice).ToString());
            Boolean isEquipment = (ff9item._FF9Item_Data[itemId].type & ItemType.AnyEquipment) != 0;
            this.confirmFundLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", FF9StateSystem.Common.FF9.party.gil.ToString());
            this.confirmCountLabel.text = ff9item.FF9Item_GetCount(itemId).ToString();
            if (isEquipment)
            {
                this.confirmEquipLabel.color = FF9TextTool.White;
                this.confirmEquipTextLabel.color = FF9TextTool.White;
                this.confirmEquipLabel.text = ff9item.FF9Item_GetEquipCount(itemId).ToString();
            }
            else
            {
                this.confirmEquipLabel.color = FF9TextTool.Gray;
                this.confirmEquipTextLabel.color = FF9TextTool.Gray;
                this.confirmEquipLabel.text = String.Empty;
            }
        }
        this.ConfirmDialogHitPoint.SetActive(true);
    }

    private void ClearCharacterInfo()
    {
        this.characterParamCaptionLabel.text = String.Empty;
        foreach (ShopUI.CharacterWeaponInfoHUD characterWeaponInfoHUD in this.charInfoHud)
        {
            characterWeaponInfoHUD.AvatarSprite.gameObject.SetActive(false);
            characterWeaponInfoHUD.ParamValueLabel.gameObject.SetActive(false);
            characterWeaponInfoHUD.ChangeArrowSprite.gameObject.SetActive(false);
            characterWeaponInfoHUD.EquipmentTypeSprite.gameObject.SetActive(false);
        }
    }

    private void ClearInfo(ShopUI.ShopType shopType)
    {
        if (shopType == ShopUI.ShopType.Item)
        {
            this.itemFundLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", FF9StateSystem.Common.FF9.party.gil.ToString());
            this.itemCountLabel.text = "0";
        }
        else if (shopType == ShopUI.ShopType.Weapon)
        {
            this.weaponFundLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", FF9StateSystem.Common.FF9.party.gil.ToString());
            this.weaponCountLabel.text = "0";
            this.weaponEquipTextLabel.color = FF9TextTool.Gray;
            this.weaponEquipLabel.color = FF9TextTool.Gray;
            this.weaponEquipLabel.text = "0";
        }
    }

    private void ClearConfirmDialog()
    {
        this.InputQuantityDialog.SetActive(false);
        this.InfoDialog.SetActive(false);
        this.ConfirmDialogHitPoint.SetActive(false);
        this.DisplayHelpPanel(this.currentMenu != ShopUI.SubMenu.Sell ? this.Type : ShopUI.ShopType.Sell);
    }

    private void UpdatePartyInfo()
    {
        foreach (PLAYER player in FF9StateSystem.Common.FF9.PlayerList)
            if (player.info.party != 0)
                this.availableCharaList.Add(player.info.slot_no);
    }

    private void InitializeMixList()
    {
        foreach (FF9MIX_DATA data in ff9mix.SynthesisData.Values)
            if (data.Shops.Contains(this.id) && data.Result != RegularItem.NoItem)
                mixItemList.Add(data);

        foreach (FF9MIX_DATA data in mixItemList)
            this.mixPartyList.Add((ff9item._FF9Item_Data[data.Result].type & ItemType.AnyEquipment) != 0);
    }

    private void StartCountItem()
    {
        RegularItem itemId = this.itemIdList[this.currentItemIndex];
        Int32 itemCount = ff9item.FF9Item_GetCount(itemId);
        FF9ITEM_DATA item = ff9item._FF9Item_Data[itemId];
        if (itemCount < ff9item.FF9ITEM_COUNT_MAX && item.price <= FF9StateSystem.Common.FF9.party.gil)
        {
            this.count = 1;
            Int32 maxBuy = (Int32)(item.price == 0 ? ff9item.FF9ITEM_COUNT_MAX : FF9StateSystem.Common.FF9.party.gil / item.price);
            this.maxCount = Math.Min(ff9item.FF9ITEM_COUNT_MAX - itemCount, maxBuy);
            this.minCount = 1;
        }
        this.isPlusQuantity = false;
        this.isMinusQuantity = false;
    }

    private void StartCountMix()
    {
        FF9MIX_DATA synth = this.mixItemList[this.currentItemIndex - this.mixStartIndex];
        Int32 resultItemCount = ff9item.FF9Item_GetCount(synth.Result);
        if (synth.CanBeSynthesized())
        {
            this.count = 1;
            this.minCount = 1;
            Int32 maxBuy = (Int32)(synth.Price == 0 ? ff9item.FF9ITEM_COUNT_MAX : FF9StateSystem.Common.FF9.party.gil / synth.Price);
            this.maxCount = Math.Min(ff9item.FF9ITEM_COUNT_MAX - resultItemCount, maxBuy);
            foreach (KeyValuePair<RegularItem, Int32> kvp in synth.IngredientsAsDictionary())
                this.maxCount = Math.Min(this.maxCount, ff9item.FF9Item_GetCount(kvp.Key) / kvp.Value);
        }
        this.isPlusQuantity = false;
        this.isMinusQuantity = false;
    }

    private void StartCountSell()
    {
        RegularItem itemId = this.sellItemIdList[this.currentItemIndex];
        this.count = 1;
        this.minCount = 1;
        this.maxCount = ff9item.FF9Item_GetCount(itemId);
        this.isPlusQuantity = false;
        this.isMinusQuantity = false;
    }

    private void SetShopType(ShopUI.ShopType shopType)
    {
        this.DisplayHelpPanel(shopType);
        this.ItemShopPanel.SetActive(false);
        this.WeaponAndSynthShopPanel.SetActive(false);
        this.SellShopPanel.SetActive(false);
        this.SynthesisPartInfoPanel.SetActive(false);
        if (shopType == ShopUI.ShopType.Synthesis)
            this.SynthesisHelpDespLabelGo.SetActive(FF9StateSystem.PCPlatform);
        else
            this.NonSynthesisHelpDespLabelGo.SetActive(FF9StateSystem.PCPlatform);
        this.confirmQuantityLabel.SetAnchor((Transform)null);
        if (shopType == ShopUI.ShopType.Item)
        {
            this.ItemShopPanel.SetActive(true);
            this.DisplayItem();
            this.shopItemScrollList.JumpToIndex(0, false);
            this.ClearInfo(shopType);
        }
        else if (shopType == ShopUI.ShopType.Weapon)
        {
            this.WeaponAndSynthShopPanel.SetActive(true);
            this.DisplayWeapon();
            this.shopWeaponScrollList.JumpToIndex(0, false);
            this.ClearInfo(this.Type);
            this.ClearCharacterInfo();
        }
        else if (shopType == ShopUI.ShopType.Synthesis)
        {
            this.WeaponAndSynthShopPanel.SetActive(true);
            this.SynthesisPartInfoPanel.SetActive(true);
            this.currentItemIndex = 0;
            this.DisplayWeapon();
            this.shopWeaponScrollList.JumpToIndex(0, false);
            this.DisplayInfo(shopType);
            this.DisplayCharacterInfo();
        }
        else if (shopType == ShopUI.ShopType.Sell)
        {
            this.SellShopPanel.SetActive(true);
            this.DisplaySellItem();
            this.shopSellItemScrollList.JumpToIndex(0, false);
        }
        this.UpdateUserInterface();
    }

    private ShopUI.SubMenu GetSubMenuFromGameObject(GameObject go)
    {
        if (go == this.BuySubMenu)
            return ShopUI.SubMenu.Buy;
        if (go == this.SellSubMenu)
            return ShopUI.SubMenu.Sell;
        return ShopUI.SubMenu.None;
    }

    private void Awake()
    {
        base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
        this.itemFundLabel = this.ItemInfoPanel.GetChild(0).GetChild(1).GetComponent<UILabel>();
        this.itemCountLabel = this.ItemInfoPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
        this.ItemInfoPanel.GetChild(2).GetChild(0).SetActive(false); // Completly hide "Equipped: 0" in item shops
        this.ItemInfoPanel.GetChild(2).GetChild(1).SetActive(false);
        this.weaponFundLabel = this.WeaponInfoPanel.GetChild(0).GetChild(1).GetComponent<UILabel>();
        this.weaponCountLabel = this.WeaponInfoPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
        this.weaponEquipLabel = this.WeaponInfoPanel.GetChild(2).GetChild(1).GetComponent<UILabel>();
        this.weaponEquipTextLabel = this.WeaponInfoPanel.GetChild(2).GetChild(0).GetComponent<UILabel>();
        foreach (Transform transform in this.CharacterParamInfoPanel.GetChild(0).transform)
            this.charInfoHud.Add(new ShopUI.CharacterWeaponInfoHUD(transform.gameObject));
        this.requiredItem1Hud = new ItemListDetailWithIconHUD(this.RequiredItemPanel.GetChild(1), false);
        this.requiredItem2Hud = new ItemListDetailWithIconHUD(this.RequiredItemPanel.GetChild(2), false);
        this.confirmItemHud = new ItemListDetailWithIconHUD(this.InputQuantityDialog.GetChild(0), false);
        this.confirmQuantityLabel = this.InputQuantityDialog.GetChild(0).GetChild(2).GetComponent<UILabel>();
        this.confirmPriceLabel = this.InputQuantityDialog.GetChild(3).GetChild(1).GetComponent<UILabel>();
        this.confirmFundLabel = this.InfoDialog.GetChild(0).GetChild(1).GetComponent<UILabel>();
        this.confirmCountLabel = this.InfoDialog.GetChild(1).GetChild(1).GetComponent<UILabel>();
        this.confirmEquipLabel = this.InfoDialog.GetChild(2).GetChild(1).GetComponent<UILabel>();
        this.confirmEquipTextLabel = this.InfoDialog.GetChild(2).GetChild(0).GetComponent<UILabel>();
        this.characterParamCaptionLabel = this.CharacterParamInfoPanel.GetChild(1).GetChild(4).GetChild(0).GetComponent<UILabel>();
        this.shopItemScrollList = this.ItemListPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        this.shopWeaponScrollList = this.WeaponListPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        this.shopSellItemScrollList = this.SellItemListPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        this._shopItemPanel = new GOScrollablePanel(this.ItemListPanel);
        this._shopWeaponPanel = new GOScrollablePanel(this.WeaponListPanel);
        this._shopSellItemPanel = new GOScrollablePanel(this.SellItemListPanel);
        UIEventListener.Get(this.BuySubMenu).onClick += this.onClick;
        UIEventListener.Get(this.SellSubMenu).onClick += this.onClick;
        UIEventListener.Get(this.InputQuantityDialog.GetChild(2)).onNavigate += this.OnKeyQuantity;
        UIEventListener.Get(this.InputQuantityDialog.GetChild(2).GetChild(1)).onPress += this.onPressPlus;
        UIEventListener.Get(this.InputQuantityDialog.GetChild(2).GetChild(2)).onPress += this.onPressMinus;
        UIEventListener.Get(this.InputQuantityDialog.GetChild(2).GetChild(1)).onClick += this.onClickPlus;
        UIEventListener.Get(this.InputQuantityDialog.GetChild(2).GetChild(2)).onClick += this.onClickMinus;
        this.triggerCounter = ShopUI.TRIGGER_DURATION;
        this.confirmQuantityLabel.width = 60;
        this.confirmQuantityLabel.height = 64;
    }

    private void Update()
    {
        if (ButtonGroupState.ActiveGroup == ShopUI.QuantityGroupButton)
        {
            if (this.isPlusQuantity)
            {
                this.triggerCounter -= Time.deltaTime;
                this.keepPressingTime += Time.deltaTime;
                if (this.triggerCounter <= 0f)
                {
                    if (this.keepPressingTime >= ShopUI.CHANGE_QUANTITY_DURATION)
                        this.PlusQuantity(10);
                    else
                        this.PlusQuantity(1);
                    this.triggerCounter = ShopUI.TRIGGER_DURATION;
                }
            }
            else if (this.isMinusQuantity)
            {
                this.triggerCounter -= Time.deltaTime;
                this.keepPressingTime += Time.deltaTime;
                if (this.triggerCounter <= 0f)
                {
                    if (this.keepPressingTime >= ShopUI.CHANGE_QUANTITY_DURATION)
                        this.MinusQuantity(10);
                    else
                        this.MinusQuantity(1);
                    this.triggerCounter = ShopUI.TRIGGER_DURATION;
                }
            }
        }
    }

    private void PlusQuantity(Int32 quantity)
    {
        Int32 newCount = Mathf.Min(this.maxCount, this.count + quantity);
        if (this.count != newCount)
        {
            FF9Sfx.FF9SFX_Play(103);
            this.count = newCount;
            this.DisplayConfirmDialog(this.currentMenu != ShopUI.SubMenu.Sell ? this.Type : ShopUI.ShopType.Sell);
        }
    }

    private void MinusQuantity(Int32 quantity)
    {
        Int32 newCount = Mathf.Max(this.minCount, this.count - quantity);
        if (this.count != newCount)
        {
            FF9Sfx.FF9SFX_Play(103);
            this.count = newCount;
            this.DisplayConfirmDialog(this.currentMenu != ShopUI.SubMenu.Sell ? this.Type : ShopUI.ShopType.Sell);
        }
    }

    private void UpdateWeaponOrSynthType(Boolean exitSelection = false)
    {
        if (this.mixItemList.Count > 0 && this.mixStartIndex > 0)
        {
            ShopType newType = !exitSelection && this.currentItemIndex >= this.mixStartIndex ? ShopType.Synthesis : ShopType.Weapon;
            if (this.type != newType)
            {
                this.type = newType;
                this.SynthesisPartInfoPanel.SetActive(newType == ShopType.Synthesis);
                this.DisplayHelpPanel(newType);
            }
        }
    }

    private const Single TRIGGER_DURATION = 0.115f;
    private const Single CHANGE_QUANTITY_DURATION = 1f;

    private static String SubMenuGroupButton = "Shop.SubMenu";
    private static String ItemGroupButton = "Shop.Item";
    private static String WeaponGroupButton = "Shop.Weapon";
    private static String SellItemGroupButton = "Shop.Sell";
    private static String QuantityGroupButton = "Shop.Quantity";

    public GameObject BuySubMenu;
    public GameObject SellSubMenu;
    public GameObject ShopTitleLabel;
    public GameObject HelpLabel;
    public GameObject ItemShopPanel;
    public GameObject WeaponAndSynthShopPanel;
    public GameObject SellShopPanel;
    public GameObject SynthesisPartInfoPanel;
    public GameObject NonSynthesisControlPanel;
    public GameObject NonSynthesisHelpDespLabelGo;
    public GameObject SynthesisControlPanel;
    public GameObject SynthesisHelpDespLabelGo;
    public GameObject InputQuantityDialog;
    public GameObject InfoDialog;
    public GameObject ConfirmDialogHitPoint;
    public GameObject ItemInfoPanel;
    public GameObject WeaponInfoPanel;
    public GameObject ItemListPanel;
    public GameObject WeaponListPanel;
    public GameObject SellItemListPanel;
    public GameObject CharacterParamInfoPanel;
    public GameObject RequiredItemPanel;
    public GameObject ScreenFadeGameObject;

    private UILabel itemFundLabel;
    private UILabel itemCountLabel;
    private UILabel weaponFundLabel;
    private UILabel weaponCountLabel;
    private UILabel weaponEquipTextLabel;
    private UILabel weaponEquipLabel;

    private ItemListDetailWithIconHUD requiredItem1Hud;
    private ItemListDetailWithIconHUD requiredItem2Hud;
    private ItemListDetailWithIconHUD confirmItemHud;

    private UILabel confirmQuantityLabel;
    private UILabel confirmPriceLabel;
    private UILabel confirmFundLabel;
    private UILabel confirmCountLabel;
    private UILabel confirmEquipLabel;
    private UILabel confirmEquipTextLabel;
    private UILabel characterParamCaptionLabel;

    private RecycleListPopulator shopItemScrollList;
    private RecycleListPopulator shopWeaponScrollList;
    private RecycleListPopulator shopSellItemScrollList;
    private GOScrollablePanel _shopItemPanel;
    private GOScrollablePanel _shopWeaponPanel;
    private GOScrollablePanel _shopSellItemPanel;

    private List<ShopUI.CharacterWeaponInfoHUD> charInfoHud = new List<ShopUI.CharacterWeaponInfoHUD>();

    private Int32 id;

    private List<RegularItem> itemIdList = new List<RegularItem>();
    private List<Boolean> isItemEnableList = new List<Boolean>();
    private List<FF9MIX_DATA> mixItemList = new List<FF9MIX_DATA>();
    private List<Boolean> mixPartyList = new List<Boolean>();
    private List<RegularItem> sellItemIdList = new List<RegularItem>();
    private Dictionary<RegularItem, Int32> soldItemIdDict = new Dictionary<RegularItem, Int32>();
    private List<CharacterId> availableCharaList = new List<CharacterId>();

    private ShopUI.ShopType type;
    private ShopUI.SubMenu currentMenu;

    private Int32 currentItemIndex = -1;
    private Int32 mixStartIndex = 0;

    private Boolean isGrocery;

    private Int32 availableCharaStart;

    private Int32 minCount;
    private Int32 maxCount;
    private Int32 count;

    private Single triggerCounter;
    private Boolean isPlusQuantity;
    private Boolean isMinusQuantity;
    private Single keepPressingTime;

    private class CharacterWeaponInfoHUD
    {
        public CharacterWeaponInfoHUD(GameObject go)
        {
            this.Self = go;
            this.ParamValueLabel = go.GetChild(0).GetComponent<UILabel>();
            this.AvatarSprite = go.GetChild(1).GetComponent<UISprite>();
            this.ChangeArrowSprite = go.GetChild(2).GetComponent<UISprite>();
            this.EquipmentTypeSprite = go.GetChild(3).GetComponent<UISprite>();
        }

        public GameObject Self;
        public UILabel ParamValueLabel;
        public UISprite AvatarSprite;
        public UISprite ChangeArrowSprite;
        public UISprite EquipmentTypeSprite;
    }

    public enum SubMenu
    {
        Buy,
        Sell,
        None
    }

    public enum ShopType
    {
        Item = 0,
        Weapon = 1,
        Synthesis = 2,
        Sell = 3,
        None = 4
    }

    public class ShopItemListData : ListDataTypeBase
    {
        public RegularItem Id;
        public Boolean Enable;
        public UInt32 Price;
    }

    public class ShopSellItemListData : ListDataTypeBase
    {
        public RegularItem Id;
        public Int32 Count;
    }
}
