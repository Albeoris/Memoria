using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria.Assets;
using Memoria.Data;
using UnityEngine;
using Object = System.Object;

public class ShopUI : UIScene
{
	public Int32 Id
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
		}
	}

	public ShopUI.ShopType Type
	{
		get
		{
			return this.type;
		}
		set
		{
			this.type = value;
		}
	}

	public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
	{
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
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
			if (this.type != ShopUI.ShopType.Synthesis)
			{
				ButtonGroupState.ActiveGroup = ShopUI.SubMenuGroupButton;
			}
			else
			{
				ButtonGroupState.ActiveGroup = ShopUI.WeaponGroupButton;
			}
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
		base.Show(sceneVoidDelegate);
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

	public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
	{
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		base.Hide(sceneVoidDelegate);
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
		this.currentMenu = ShopUI.SubMenu.Buy;
		this.itemIdList.Clear();
		this.isItemEnableList.Clear();
		this.availableCharaList.Clear();
		this.mixItemList.Clear();
		this.type = (ShopUI.ShopType)ff9shop.FF9Shop_GetType(this.id);
		this.isGrocery = false;
	    if (this.type == ShopUI.ShopType.Item)
	    {
	        ShopItems assortiment = ff9buy.ShopItems[this.id];
	        for (Int32 i = 0; i < assortiment.Length; i++)
	        {
	            Byte itemId = assortiment[i];
	            Byte itemType = ff9item._FF9Item_Data[itemId].type;
	            if ((itemType & ff9buy.FF9BUY_TYPE_WEAPON) != 0)
	            {
	                this.isGrocery = true;
	                this.type = ShopUI.ShopType.Weapon;
	                break;
	            }
	        }
	    }
	    if (this.type == ShopUI.ShopType.Synthesis)
		{
			this.AnalyzeArgument();
			this.UpdatePartyInfo();
		}
		else if (this.type == ShopUI.ShopType.Weapon)
		{
			this.UpdatePartyInfo();
		}
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
					this.DisplayInfo((ShopUI.ShopType)((this.currentMenu != ShopUI.SubMenu.Sell) ? this.Type : ShopUI.ShopType.Sell));
					this.shopItemScrollList.JumpToIndex(0, false);
					ButtonGroupState.RemoveCursorMemorize(ShopUI.ItemGroupButton);
					ButtonGroupState.ActiveGroup = ShopUI.ItemGroupButton;
				}
				else
				{
					FF9Sfx.FF9SFX_Play(103);
					this.currentItemIndex = 0;
					this.DisplayInfo((ShopUI.ShopType)((this.currentMenu != ShopUI.SubMenu.Sell) ? this.Type : ShopUI.ShopType.Sell));
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
						{
							this.StartCountItem();
						}
						else
						{
							this.StartCountMix();
						}
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
					Int32 key = this.sellItemIdList[this.currentItemIndex];
					if (!this.soldItemIdDict.ContainsKey(key))
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
						Int32 num = ff9item.FF9Item_Add(this.itemIdList[this.currentItemIndex], this.count);
						FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[this.itemIdList[this.currentItemIndex]];
						if (num != 0)
						{
							FF9StateSystem.Common.FF9.party.gil -= (UInt32)((Int32)ff9ITEM_DATA.price * num);
							this.DisplayInfo((ShopUI.ShopType)((this.currentMenu != ShopUI.SubMenu.Sell) ? this.Type : ShopUI.ShopType.Sell));
							this.DisplayItem();
							ButtonGroupState.ActiveGroup = ShopUI.ItemGroupButton;
						}
					}
					else if (this.type == ShopUI.ShopType.Weapon)
					{
						FF9Sfx.FF9SFX_Play(1045);
						Int32 num2 = ff9item.FF9Item_Add(this.itemIdList[this.currentItemIndex], this.count);
						FF9ITEM_DATA ff9ITEM_DATA2 = ff9item._FF9Item_Data[this.itemIdList[this.currentItemIndex]];
						if (num2 != 0)
						{
							FF9StateSystem.Common.FF9.party.gil -= (UInt32)((Int32)ff9ITEM_DATA2.price * num2);
							this.DisplayInfo((ShopUI.ShopType)((this.currentMenu != ShopUI.SubMenu.Sell) ? this.Type : ShopUI.ShopType.Sell));
							this.DisplayWeapon();
							ButtonGroupState.ActiveGroup = ShopUI.WeaponGroupButton;
						}
					}
					else if (this.type == ShopUI.ShopType.Synthesis)
					{
						FF9Sfx.FF9SFX_Play(1045);
						FF9MIX_DATA ff9MIX_DATA = this.mixItemList[this.currentItemIndex];
						Int32 num3 = ff9item.FF9Item_Add((Int32)ff9MIX_DATA.Result, this.count);
						if (num3 != 0)
						{
							ff9item.FF9Item_Remove((Int32)ff9MIX_DATA.Ingredients[0], num3);
							ff9item.FF9Item_Remove((Int32)ff9MIX_DATA.Ingredients[1], num3);
							FF9StateSystem.Common.FF9.party.gil -= (UInt32)((Int32)ff9MIX_DATA.Price * num3);
							this.DisplayInfo((ShopUI.ShopType)((this.currentMenu != ShopUI.SubMenu.Sell) ? this.Type : ShopUI.ShopType.Sell));
							this.DisplayWeapon();
							ButtonGroupState.ActiveGroup = ShopUI.WeaponGroupButton;
							FF9StateSystem.Achievement.synthesisCount++;
							AchievementManager.ReportAchievement(AcheivementKey.Synth10, FF9StateSystem.Achievement.synthesisCount);
							AchievementManager.ReportAchievement(AcheivementKey.Synth30, FF9StateSystem.Achievement.synthesisCount);
						}
					}
				}
				else
				{
					FF9Sfx.FF9SFX_Play(1045);
					Int32 num4 = this.sellItemIdList[this.currentItemIndex];
					FF9ITEM_DATA ff9ITEM_DATA3 = ff9item._FF9Item_Data[num4];
					Int32 num5 = ff9ITEM_DATA3.price >> 1;
					Int32 num6 = ff9item.FF9Item_Remove(num4, this.count);
					if (num6 != 0)
					{
						FF9StateSystem.Common.FF9.party.gil += (UInt32)(num5 * this.count);
						if (FF9StateSystem.Common.FF9.party.gil > 9999999u)
						{
							FF9StateSystem.Common.FF9.party.gil = 9999999u;
						}
					}
					if (ff9item.FF9Item_GetCount(num4) == 0)
					{
						this.soldItemIdDict[num4] = this.currentItemIndex;
					}
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
					{
						ButtonGroupState.ActiveGroup = ShopUI.ItemGroupButton;
					}
					else if (this.type == ShopUI.ShopType.Weapon || this.type == ShopUI.ShopType.Synthesis)
					{
						ButtonGroupState.ActiveGroup = ShopUI.WeaponGroupButton;
					}
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
		return !(ButtonGroupState.ActiveGroup == ShopUI.QuantityGroupButton) && base.OnKeySelect(go);
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
					if (subMenu != ShopUI.SubMenu.Buy)
					{
						if (subMenu == ShopUI.SubMenu.Sell)
						{
							this.SetShopType(ShopUI.ShopType.Sell);
						}
					}
					else
					{
						this.SetShopType(this.Type);
					}
				}
			}
			else if (ButtonGroupState.ActiveGroup == ShopUI.ItemGroupButton)
			{
				if (this.currentItemIndex != go.GetComponent<RecycleListItem>().ItemDataIndex)
				{
					this.currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
					this.DisplayInfo((ShopUI.ShopType)((this.currentMenu != ShopUI.SubMenu.Sell) ? this.Type : ShopUI.ShopType.Sell));
				}
			}
			else if (ButtonGroupState.ActiveGroup == ShopUI.WeaponGroupButton)
			{
				if (this.currentItemIndex != go.GetComponent<RecycleListItem>().ItemDataIndex)
				{
					this.currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
					this.DisplayInfo((ShopUI.ShopType)((this.currentMenu != ShopUI.SubMenu.Sell) ? this.Type : ShopUI.ShopType.Sell));
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
			{
				this.MinusQuantity(1);
			}
			else if (key == KeyCode.DownArrow)
			{
				this.MinusQuantity(10);
			}
			else if (key == KeyCode.RightArrow)
			{
				this.PlusQuantity(1);
			}
			else if (key == KeyCode.UpArrow)
			{
				this.PlusQuantity(10);
			}
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
			this.triggerCounter = 0.115f;
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
			this.triggerCounter = 0.115f;
			this.keepPressingTime = 0f;
			this.isPlusQuantity = false;
		}
	}

	private void onClickMinus(GameObject go)
	{
		this.MinusQuantity(1);
		this.triggerCounter = 0.115f;
	}

	private void onClickPlus(GameObject go)
	{
		this.PlusQuantity(1);
		this.triggerCounter = 0.115f;
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
			Int32 num = this.itemIdList[this.currentItemIndex];
			this.itemFundLabel.text = FF9StateSystem.Common.FF9.party.gil.ToString() + "[YSUB=1.3][sub]G";
			this.itemCountLabel.text = ff9item.FF9Item_GetCount(num).ToString();
			this.requiredItem1Hud.Self.SetActive(false);
			this.requiredItem2Hud.Self.SetActive(false);
		}
		else if (shopType == ShopUI.ShopType.Weapon)
		{
			Int32 num2 = this.itemIdList[this.currentItemIndex];
			this.weaponFundLabel.text = FF9StateSystem.Common.FF9.party.gil.ToString() + "[YSUB=1.3][sub]G";
			this.weaponCountLabel.text = ff9item.FF9Item_GetCount(this.itemIdList[this.currentItemIndex]).ToString();
			this.weaponEquipLabel.text = ff9item.FF9Item_GetEquipCount(this.itemIdList[this.currentItemIndex]).ToString();
			if (ff9item.FF9Item_GetEquipPart(num2) != -1)
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
			FF9MIX_DATA ff9MIX_DATA = this.mixItemList[this.currentItemIndex];
			this.weaponFundLabel.text = FF9StateSystem.Common.FF9.party.gil.ToString() + "[YSUB=1.3][sub]G";
			this.weaponCountLabel.text = ff9item.FF9Item_GetCount((Int32)ff9MIX_DATA.Result).ToString();
			this.weaponEquipLabel.color = FF9TextTool.White;
			this.weaponEquipLabel.text = ff9item.FF9Item_GetEquipCount((Int32)ff9MIX_DATA.Result).ToString();
			this.requiredItem1Hud.Self.SetActive(true);
			FF9UIDataTool.DisplayItem((Int32)ff9MIX_DATA.Ingredients[0], this.requiredItem1Hud.IconSprite, this.requiredItem1Hud.NameLabel, ff9item.FF9Item_GetCount((Int32)ff9MIX_DATA.Ingredients[0]) != 0);
			if (ff9MIX_DATA.Ingredients[1] != 255)
			{
				this.requiredItem2Hud.Self.SetActive(true);
				if (ff9MIX_DATA.Ingredients[0] != ff9MIX_DATA.Ingredients[1])
				{
					FF9UIDataTool.DisplayItem((Int32)ff9MIX_DATA.Ingredients[1], this.requiredItem2Hud.IconSprite, this.requiredItem2Hud.NameLabel, ff9item.FF9Item_GetCount((Int32)ff9MIX_DATA.Ingredients[1]) != 0);
				}
				else
				{
					FF9UIDataTool.DisplayItem((Int32)ff9MIX_DATA.Ingredients[1], this.requiredItem2Hud.IconSprite, this.requiredItem2Hud.NameLabel, ff9item.FF9Item_GetCount((Int32)ff9MIX_DATA.Ingredients[1]) > 1);
				}
			}
			else
			{
				this.requiredItem2Hud.Self.SetActive(false);
			}
		}
	}

	private void DisplayItem()
	{
		if (this.currentMenu == ShopUI.SubMenu.Sell)
		{
			return;
		}
		this.itemIdList.Clear();
		this.isItemEnableList.Clear();
		List<ListDataTypeBase> list = new List<ListDataTypeBase>();
	    ShopItems assortiment = ff9buy.ShopItems[this.id];

        for (Int32 i = 0; i < assortiment.Length; i++)
		{
			Int32 itemId = assortiment[i];
			FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[itemId];
			Boolean flag = ff9item.FF9Item_GetCount(itemId) < 99 && FF9StateSystem.Common.FF9.party.gil >= (UInt32)ff9ITEM_DATA.price;
			this.isItemEnableList.Add(flag);
			this.itemIdList.Add(itemId);
			list.Add(new ShopUI.ShopItemListData
			{
				Id = itemId,
				Price = (Int32)ff9ITEM_DATA.price,
				Enable = flag
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
		{
			this.DisplayWindowBackground(item.gameObject, (UIAtlas)null);
		}
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
		{
			return;
		}
		this.itemIdList.Clear();
		this.isItemEnableList.Clear();
		List<ListDataTypeBase> list = new List<ListDataTypeBase>();
		if (this.type == ShopUI.ShopType.Weapon)
		{
		    ShopItems assortiment = ff9buy.ShopItems[this.id];

            for (Int32 i = 0; i < assortiment.Length; i++)
			{
				Int32 itemId = assortiment[i];
				FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[itemId];
				Boolean flag = ff9item.FF9Item_GetCount(itemId) < 99 && FF9StateSystem.Common.FF9.party.gil >= (UInt32)ff9ITEM_DATA.price;
				this.isItemEnableList.Add(flag);
				this.itemIdList.Add(itemId);
				list.Add(new ShopUI.ShopItemListData
				{
					Id = itemId,
					Price = (Int32)ff9ITEM_DATA.price,
					Enable = flag
				});
			}
		}
		else if (this.type == ShopUI.ShopType.Synthesis)
		{
			for (Int32 j = 0; j < this.mixItemList.Count; j++)
			{
				FF9MIX_DATA ff9MIX_DATA = this.mixItemList[j];
				Boolean canBeSynthesized = ff9item.FF9Item_GetCount((Int32)ff9MIX_DATA.Result) < 99 && FF9StateSystem.Common.FF9.party.gil >= (UInt32)ff9MIX_DATA.Price;

			    if (ff9MIX_DATA.Ingredients[0] == ff9MIX_DATA.Ingredients[1] && ff9MIX_DATA.Ingredients[0] != 255)
			    {
			        Byte itemId = ff9MIX_DATA.Ingredients[0]; // Fix it if you change ff9mix.FF9MIX_SRC_MAX
			        if (ff9item.FF9Item_GetCount(itemId) < 2)
			            canBeSynthesized = false;
			    }
			    else
			    {
			        for (Int32 k = 0; canBeSynthesized && k < ff9mix.FF9MIX_SRC_MAX; k++)
			        {
			            if (ff9MIX_DATA.Ingredients[k] == 255)
			                continue;

                        Byte itemId = ff9MIX_DATA.Ingredients[k];
			            if (ff9item.FF9Item_GetCount(itemId) < 1)
			                canBeSynthesized = false;
			        }
			    }
			    this.itemIdList.Add((Int32)ff9MIX_DATA.Result);
				this.isItemEnableList.Add(canBeSynthesized);
				list.Add(new ShopUI.ShopItemListData
				{
					Id = (Int32)ff9MIX_DATA.Result,
					Price = (Int32)ff9MIX_DATA.Price,
					Enable = canBeSynthesized
				});
			}
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
		{
			this.DisplayWindowBackground(item.gameObject, (UIAtlas)null);
		}
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
		FF9ITEM[] item = FF9StateSystem.Common.FF9.item;
		for (Int32 i = 0; i < (Int32)item.Length; i++)
		{
			FF9ITEM ff9ITEM = item[i];
			if (ff9ITEM.count > 0)
			{
				if (this.soldItemIdDict.ContainsKey((Int32)ff9ITEM.id))
				{
					this.soldItemIdDict.Remove((Int32)ff9ITEM.id);
				}
				this.sellItemIdList.Add((Int32)ff9ITEM.id);
				FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[(Int32)ff9ITEM.id];
				list.Add(new ShopUI.ShopSellItemListData
				{
					Id = (Int32)ff9ITEM.id,
					Count = (Int32)ff9ITEM.count
				});
			}
		}
		foreach (KeyValuePair<Int32, Int32> keyValuePair in from key in this.soldItemIdDict
		orderby key.Value
		select key)
		{
			this.sellItemIdList.Insert(keyValuePair.Value, keyValuePair.Key);
			FF9ITEM_DATA ff9ITEM_DATA2 = ff9item._FF9Item_Data[keyValuePair.Key];
			ShopUI.ShopSellItemListData shopSellItemListData = new ShopUI.ShopSellItemListData();
			shopSellItemListData.Id = keyValuePair.Key;
			shopSellItemListData.Count = 0;
			list.Insert(keyValuePair.Value, shopSellItemListData);
		}
		if (this.shopSellItemScrollList.ItemsPool.Count == 0)
		{
			this.shopSellItemScrollList.PopulateListItemWithData = new Action<Transform, ListDataTypeBase, Int32, Boolean>(this.DisplaySellItemDetail);
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
		{
			this.DisplayWindowBackground(item.gameObject, (UIAtlas)null);
		}
		if (shopSellItemListData.Count > 0)
		{
			FF9UIDataTool.DisplayItem(shopSellItemListData.Id, itemListDetailWithIconHUD.IconSprite, itemListDetailWithIconHUD.NameLabel, true);
			itemListDetailWithIconHUD.NumberLabel.text = shopSellItemListData.Count.ToString();
			itemListDetailWithIconHUD.NumberLabel.color = FF9TextTool.White;
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
		Int32 cureentItemId = this.itemIdList[this.currentItemIndex];
		Int32 equipPart = ff9item.FF9Item_GetEquipPart(cureentItemId);
		Boolean isArmor = equipPart > 0;
		if (equipPart < 0)
			return;
		this.characterParamCaptionLabel.text = isArmor ? Localization.Get("DefenseCaption") : Localization.Get("AttackCaption");
		Int32 hudIndex = 0;
		foreach (CharacterId charId in this.availableCharaList)
		{
			ShopUI.CharacterWeaponInfoHUD characterWeaponInfoHUD = this.charInfoHud[hudIndex++];
			PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(charId);
			UInt64 playerMask = ff9feqp.GetCharacterEquipMask(player);
			Boolean canEquip = (ff9item._FF9Item_Data[cureentItemId].equip & playerMask) != 0;
			characterWeaponInfoHUD.AvatarSprite.gameObject.SetActive(true);
			FF9UIDataTool.DisplayCharacterAvatar(player, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), characterWeaponInfoHUD.AvatarSprite, false);
			if (canEquip)
			{
				Int32 oldRating;
				Int32 newRating;
				if (!isArmor)
				{
					oldRating = (Int32)ff9weap.WeaponData[(Int32)player.equip[0]].Ref.Power;
					newRating = (Int32)ff9weap.WeaponData[cureentItemId].Ref.Power;
				}
				else
				{
				    CharacterEquipment equipCopy = player.equip.Clone();
					equipCopy[equipPart] = (Byte)cureentItemId;
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
				this.confirmPriceLabel.text = (this.count * (Int32)ff9item._FF9Item_Data[this.itemIdList[this.currentItemIndex]].price).ToString() + "[YSUB=1.3][sub]G";
			}
			else
			{
				FF9UIDataTool.DisplayTextLocalize(this.HelpLabel, "MixQtyHelp");
				FF9UIDataTool.DisplayItem((Int32)this.mixItemList[this.currentItemIndex].Result, this.confirmItemHud.IconSprite, this.confirmItemHud.NameLabel, true);
				this.confirmPriceLabel.text = (this.count * (Int32)this.mixItemList[this.currentItemIndex].Price).ToString() + "[YSUB=1.3][sub]G";
			}
		}
		else
		{
			this.InputQuantityDialog.SetActive(true);
			this.InputQuantityDialog.transform.localPosition = new Vector3(0f, 50f, 0f);
			FF9UIDataTool.DisplayTextLocalize(this.HelpLabel, "SellQtyHelp");
			this.InfoDialog.SetActive(true);
			Int32 num = this.sellItemIdList[this.currentItemIndex];
			FF9UIDataTool.DisplayItem(num, this.confirmItemHud.IconSprite, this.confirmItemHud.NameLabel, true);
			Int32 num2 = ff9item._FF9Item_Data[num].price >> 1;
			this.confirmQuantityLabel.text = this.count.ToString();
			this.confirmPriceLabel.text = (this.count * num2).ToString() + "[YSUB=1.3][sub]G";
			Boolean flag = (ff9item._FF9Item_Data[num].type & 248) != 0;
			this.confirmFundLabel.text = FF9StateSystem.Common.FF9.party.gil.ToString() + "[YSUB=1.3][sub]G";
			this.confirmCountLabel.text = ff9item.FF9Item_GetCount(num).ToString();
			if (flag)
			{
				this.confirmEquipLabel.color = FF9TextTool.White;
				this.confirmEquipTextLabel.color = FF9TextTool.White;
				this.confirmEquipLabel.text = ff9item.FF9Item_GetEquipCount(num).ToString();
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
			this.itemFundLabel.text = FF9StateSystem.Common.FF9.party.gil.ToString() + "[YSUB=1.3][sub]G";
			this.itemCountLabel.text = "0";
		}
		else if (shopType == ShopUI.ShopType.Weapon)
		{
			this.weaponFundLabel.text = FF9StateSystem.Common.FF9.party.gil.ToString() + "[YSUB=1.3][sub]G";
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
		this.DisplayHelpPanel((ShopUI.ShopType)((this.currentMenu != ShopUI.SubMenu.Sell) ? this.Type : ShopUI.ShopType.Sell));
	}

	private void UpdatePartyInfo()
	{
		for (Int32 i = 0; i < FF9StateSystem.Common.PlayerCount; i++)
		{
			if (FF9StateSystem.Common.FF9.player[i].info.party != 0)
			{
				this.availableCharaList.Add(FF9StateSystem.Common.FF9.player[i].info.slot_no);
				if (this.availableCharaList.Count >= 8) // TODO: allow to somehow display more than 8 characters
					break;
			}
		}
	}

    private void AnalyzeArgument()
    {
        Int32 shopNumber = this.id - 32;
        Byte shopFlag = Byte.Parse((1 << shopNumber).ToString());
        foreach (FF9MIX_DATA data in ff9mix.SynthesisData)
        {
            if ((data.Shops & shopFlag) <= 0 || data.Result == 255)
                continue;

            mixItemList.Add(data);
            if (mixItemList.Count > 31)
                break;
        }

        foreach (FF9MIX_DATA data in mixItemList)
        {
            Byte itemType = ff9item._FF9Item_Data[data.Result].type;
            if ((itemType & 248) > 0)
                this.mixPartyList.Add(true);
            else
                this.mixPartyList.Add(false);
        }
    }

    private void StartCountItem()
	{
		Int32 num = this.itemIdList[this.currentItemIndex];
		Int32 num2 = ff9item.FF9Item_GetCount(num);
		FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[num];
		if (99 > num2 && (UInt32)ff9ITEM_DATA.price <= FF9StateSystem.Common.FF9.party.gil)
		{
			this.count = 1;
			UInt32 num3 = (UInt32)((ff9ITEM_DATA.price == 0) ? 99u : (FF9StateSystem.Common.FF9.party.gil / (UInt32)ff9ITEM_DATA.price));
			this.maxCount = (Int32)Mathf.Min((Single)(99 - num2), num3);
			this.minCount = 1;
		}
		this.isPlusQuantity = false;
		this.isMinusQuantity = false;
	}

	private void StartCountMix()
	{
		FF9MIX_DATA ff9MIX_DATA = this.mixItemList[this.currentItemIndex];
		Int32 num = (Int32)((ff9MIX_DATA.Ingredients[0] != ff9MIX_DATA.Ingredients[1]) ? 1 : 2);
		Int32 num2 = ff9item.FF9Item_GetCount((Int32)ff9MIX_DATA.Result);
		if (99 > num2 && (UInt32)ff9MIX_DATA.Price <= FF9StateSystem.Common.FF9.party.gil && (ff9MIX_DATA.Ingredients[0] == 255 || num <= ff9item.FF9Item_GetCount((Int32)ff9MIX_DATA.Ingredients[0])) && (ff9MIX_DATA.Ingredients[1] == 255 || num <= ff9item.FF9Item_GetCount((Int32)ff9MIX_DATA.Ingredients[1])))
		{
			this.count = 1;
			this.minCount = 1;
			UInt32 num3 = (UInt32)((ff9MIX_DATA.Price == 0) ? 99u : (FF9StateSystem.Common.FF9.party.gil / (UInt32)ff9MIX_DATA.Price));
			this.maxCount = (Int32)Mathf.Min((Single)(99 - num2), num3);
			if (ff9MIX_DATA.Ingredients[0] != 255)
			{
				this.maxCount = Mathf.Min(this.maxCount, ff9item.FF9Item_GetCount((Int32)ff9MIX_DATA.Ingredients[0]) / num);
			}
			if (ff9MIX_DATA.Ingredients[1] != 255)
			{
				this.maxCount = Mathf.Min(this.maxCount, ff9item.FF9Item_GetCount((Int32)ff9MIX_DATA.Ingredients[1]) / num);
			}
		}
		this.isPlusQuantity = false;
		this.isMinusQuantity = false;
	}

	private void StartCountSell()
	{
		Int32 num = this.sellItemIdList[this.currentItemIndex];
		this.count = 1;
		this.minCount = 1;
		this.maxCount = ff9item.FF9Item_GetCount(num);
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
		{
			if (FF9StateSystem.PCPlatform)
			{
				this.SynthesisHelpDespLabelGo.SetActive(true);
			}
			else
			{
				this.SynthesisHelpDespLabelGo.SetActive(false);
			}
		}
		else if (FF9StateSystem.PCPlatform)
		{
			this.NonSynthesisHelpDespLabelGo.SetActive(true);
		}
		else
		{
			this.NonSynthesisHelpDespLabelGo.SetActive(false);
		}
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
	}

	private ShopUI.SubMenu GetSubMenuFromGameObject(GameObject go)
	{
		if (go == this.BuySubMenu)
		{
			return ShopUI.SubMenu.Buy;
		}
		if (go == this.SellSubMenu)
		{
			return ShopUI.SubMenu.Sell;
		}
		return ShopUI.SubMenu.None;
	}

	private void Awake()
	{
		base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
		this.itemFundLabel = this.ItemInfoPanel.GetChild(0).GetChild(1).GetComponent<UILabel>();
		this.itemCountLabel = this.ItemInfoPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
		this.weaponFundLabel = this.WeaponInfoPanel.GetChild(0).GetChild(1).GetComponent<UILabel>();
		this.weaponCountLabel = this.WeaponInfoPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
		this.weaponEquipLabel = this.WeaponInfoPanel.GetChild(2).GetChild(1).GetComponent<UILabel>();
		this.weaponEquipTextLabel = this.WeaponInfoPanel.GetChild(2).GetChild(0).GetComponent<UILabel>();
		foreach (Object obj in this.CharacterParamInfoPanel.GetChild(0).transform)
		{
			Transform transform = (Transform)obj;
			ShopUI.CharacterWeaponInfoHUD item = new ShopUI.CharacterWeaponInfoHUD(transform.gameObject);
			this.charInfoHud.Add(item);
		}
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
		UIEventListener uieventListener = UIEventListener.Get(this.BuySubMenu);
		uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.onClick));
		UIEventListener uieventListener2 = UIEventListener.Get(this.SellSubMenu);
		uieventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener2.onClick, new UIEventListener.VoidDelegate(this.onClick));
		UIEventListener uieventListener3 = UIEventListener.Get(this.InputQuantityDialog.GetChild(2));
		uieventListener3.onNavigate = (UIEventListener.KeyCodeDelegate)Delegate.Combine(uieventListener3.onNavigate, new UIEventListener.KeyCodeDelegate(this.OnKeyQuantity));
		UIEventListener uieventListener4 = UIEventListener.Get(this.InputQuantityDialog.GetChild(2).GetChild(1));
		uieventListener4.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener4.onPress, new UIEventListener.BoolDelegate(this.onPressPlus));
		UIEventListener uieventListener5 = UIEventListener.Get(this.InputQuantityDialog.GetChild(2).GetChild(2));
		uieventListener5.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener5.onPress, new UIEventListener.BoolDelegate(this.onPressMinus));
		UIEventListener uieventListener6 = UIEventListener.Get(this.InputQuantityDialog.GetChild(2).GetChild(1));
		uieventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener6.onClick, new UIEventListener.VoidDelegate(this.onClickPlus));
		UIEventListener uieventListener7 = UIEventListener.Get(this.InputQuantityDialog.GetChild(2).GetChild(2));
		uieventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener7.onClick, new UIEventListener.VoidDelegate(this.onClickMinus));
		this.triggerCounter = 0.115f;
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
					if (this.keepPressingTime >= 1f)
					{
						this.PlusQuantity(10);
					}
					else
					{
						this.PlusQuantity(1);
					}
					this.triggerCounter = 0.115f;
				}
			}
			else if (this.isMinusQuantity)
			{
				this.triggerCounter -= Time.deltaTime;
				this.keepPressingTime += Time.deltaTime;
				if (this.triggerCounter <= 0f)
				{
					if (this.keepPressingTime >= 1f)
					{
						this.MinusQuantity(10);
					}
					else
					{
						this.MinusQuantity(1);
					}
					this.triggerCounter = 0.115f;
				}
			}
		}
	}

	private void PlusQuantity(Int32 quantity)
	{
		Int32 num = this.count;
		Int32 num2 = Mathf.Min(this.maxCount, this.count + quantity);
		if (num != num2)
		{
			FF9Sfx.FF9SFX_Play(103);
			this.count = num2;
			this.DisplayConfirmDialog((ShopUI.ShopType)((this.currentMenu != ShopUI.SubMenu.Sell) ? this.Type : ShopUI.ShopType.Sell));
		}
	}

	private void MinusQuantity(Int32 quantity)
	{
		Int32 num = this.count;
		Int32 num2 = Mathf.Max(this.minCount, this.count - quantity);
		if (num != num2)
		{
			FF9Sfx.FF9SFX_Play(103);
			this.count = num2;
			this.DisplayConfirmDialog((ShopUI.ShopType)((this.currentMenu != ShopUI.SubMenu.Sell) ? this.Type : ShopUI.ShopType.Sell));
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

	private List<ShopUI.CharacterWeaponInfoHUD> charInfoHud = new List<ShopUI.CharacterWeaponInfoHUD>();

	private Int32 id;

	private List<Int32> itemIdList = new List<Int32>();

	private List<Boolean> isItemEnableList = new List<Boolean>();

	private List<FF9MIX_DATA> mixItemList = new List<FF9MIX_DATA>();

	private List<Boolean> mixPartyList = new List<Boolean>();

	private List<Int32> sellItemIdList = new List<Int32>();

	private Dictionary<Int32, Int32> soldItemIdDict = new Dictionary<Int32, Int32>();

	private List<CharacterId> availableCharaList = new List<CharacterId>();

	private ShopUI.ShopType type;

	private ShopUI.SubMenu currentMenu;

	private Int32 currentItemIndex = -1;

	private Boolean isGrocery;

	private Int32 minCount;

	private Int32 maxCount;

	private Int32 count;

	private Single triggerCounter;

	private Boolean isPlusQuantity;

	private Boolean isMinusQuantity;

	private Single keepPressingTime;

	private Byte[] partMask = new Byte[]
	{
		128,
		32,
		64,
		16,
		8
	};

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
		public Int32 Id;

		public Boolean Enable;

		public Int32 Price;
	}

	public class ShopSellItemListData : ListDataTypeBase
	{
		public Int32 Id;

		public Int32 Count;
	}
}
