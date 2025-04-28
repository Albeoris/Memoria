using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Scenes;
using NCalc;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class EquipUI : UIScene
{
    public Int32 CurrentPartyIndex
    {
        set => this.currentPartyIndex = value;
    }

    public void Update()
    {
        if (UIKeyTrigger.IsShiftKeyPressed ^ _equipForAbilityLearning)
            ChangeBestItemsButtonBehaviour(UIKeyTrigger.IsShiftKeyPressed);
        if (this.currentAbilityPage >= 0 && (ButtonGroupState.ActiveGroup == EquipUI.EquipmentGroupButton || ButtonGroupState.ActiveGroup == EquipUI.InventoryGroupButton) && !PersistenSingleton<UIManager>.Instance.IsPause)
        {
            this.currentAbilityPageCounter -= RealTime.deltaTime;
            if (this.currentAbilityPageCounter <= 0f)
            {
                PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
                FF9ITEM_DATA itemData;
                if (ButtonGroupState.ActiveGroup == EquipUI.EquipmentGroupButton)
                    itemData = ff9item._FF9Item_Data[player.equip[this.currentEquipPart]];
                else
                    itemData = ff9item._FF9Item_Data[this.itemIdList[this.currentEquipPart][this.currentItemIndex].id];
                Int32 abilPageCount = (itemData.ability.Sum(abilId => abilId != 0 ? 1 : 0) + 2) / 3;
                this.currentAbilityPage++;
                if (this.currentAbilityPage >= abilPageCount)
                    this.currentAbilityPage = 0;
                this.UpdateAbilityPage(player, itemData);
            }
        }
    }

    private void ChangeBestItemsButtonBehaviour(Boolean equipForAbilityLearning)
    {
        GameObject label = this.OptimizeSubMenu.GetChild(1);
        GOLocalizableLabel localizableLable = new GOLocalizableLabel(label);
        localizableLable.Localize.key = equipForAbilityLearning ? "Ability" : "Optimize";
        localizableLable.Localize.OnLocalize();
        _equipForAbilityLearning = equipForAbilityLearning;
    }

    public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate afterShowAction = delegate
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.SubMenuPanel.SetActive(false);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(10f, 0f), EquipUI.InventoryGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(this.equipSelectScrollList.ScrollButton, EquipUI.InventoryGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(new Vector4(-7f, -49f, 745f, 325f), EquipUI.InventoryGroupButton);
            ButtonGroupState.ActiveGroup = EquipUI.SubMenuGroupButton;
        };
        if (afterFinished != null)
            afterShowAction += afterFinished;

        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(afterShowAction);
        this.UpdateUserInterface();
        this.SwitchCharacter(true);
        this.DisplaySubMenuArrow(true);
        this.HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);

        this.equipSelectScrollList.ScrollButton.DisplayScrollButton(false, false);
    }

    public void UpdateUserInterface()
    {
        if (!Configuration.Interface.IsEnabled)
            return;
        const Int32 originalLineCount = 5;
        const Single buttonOriginalHeight = 90f;
        const Single panelOriginalWidth = 752f;
        const Single panelOriginalHeight = originalLineCount * buttonOriginalHeight;
        Int32 linePerPage = Configuration.Interface.MenuEquipRowCount;
        Int32 lineHeight = (Int32)Math.Round(panelOriginalHeight / linePerPage);
        Single scaleFactor = lineHeight / buttonOriginalHeight;

        _equipSelectPanel.SubPanel.ChangeDims(1, linePerPage, panelOriginalWidth, lineHeight);
        _equipSelectPanel.SubPanel.ButtonPrefab.IconSprite.SetAnchor(target: _equipSelectPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.144f, relTop: 0.856f, relLeft: 0.044f, relRight: 0.13f);
        _equipSelectPanel.SubPanel.ButtonPrefab.IconSprite.width = _equipSelectPanel.SubPanel.ButtonPrefab.IconSprite.height;

        _equipSelectPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _equipSelectPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.144f, relTop: 0.856f, relLeft: 0.154f, relRight: 0.795f);
        _equipSelectPanel.SubPanel.ButtonPrefab.NameLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _equipSelectPanel.SubPanel.ButtonPrefab.NameLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));

        _equipSelectPanel.SubPanel.ButtonPrefab.NumberLabel.SetAnchor(target: _equipSelectPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.144f, relTop: 0.856f, relLeft: 0.8f, relRight: 0.92f);
        _equipSelectPanel.SubPanel.ButtonPrefab.NumberLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _equipSelectPanel.SubPanel.ButtonPrefab.NumberLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));
        _equipSelectPanel.SubPanel.RecycleListPopulator.RefreshTableView();
    }

    public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate afterHideAction = delegate
        {
            MainMenuUI.UIControlPanel?.ExitMenu();
        };
        if (afterFinished != null)
            afterHideAction += afterFinished;
        base.Hide(afterHideAction);
        if (!this.fastSwitch)
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.StartSubmenuTweenIn();
            this.RemoveCursorMemorize();
        }
    }

    private void RemoveCursorMemorize()
    {
        ButtonGroupState.RemoveCursorMemorize(EquipUI.SubMenuGroupButton);
        ButtonGroupState.RemoveCursorMemorize(EquipUI.EquipmentGroupButton);
    }

    public void OnLocalize()
    {
        if (!isActiveAndEnabled)
            return;
        if (this.equipmentListCaption.isActiveAndEnabled)
        {
            switch (this.currentEquipPart)
            {
                case 0:
                    this.equipmentListCaption.rawText = Localization.Get("WeaponCaption");
                    break;
                case 1:
                    this.equipmentListCaption.rawText = Localization.Get("HeadCaption");
                    break;
                case 2:
                    this.equipmentListCaption.rawText = Localization.Get("WristCaption");
                    break;
                case 3:
                    this.equipmentListCaption.rawText = Localization.Get("ArmorCaption");
                    break;
                case 4:
                    this.equipmentListCaption.rawText = Localization.Get("AccessoryCaption");
                    break;
                default:
                    this.equipmentListCaption.rawText = Localization.Get("WeaponCaption");
                    break;
            }
        }
        if (this.equipSelectScrollList.isActiveAndEnabled)
            this.equipSelectScrollList.UpdateTableViewImp();
        this.DisplayEquipment();
        this.DisplayEquiptmentInfo();
        this.DisplayHelp();
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (!base.OnKeyConfirm(go))
            return true;

        if (ButtonGroupState.ActiveGroup == SubMenuGroupButton)
            OnKeyConfirmSubMenuGroupButton(go);
        else if (ButtonGroupState.ActiveGroup == EquipmentGroupButton)
            OnKeyConfirmEquipmentGroupButton(go);
        else if (ButtonGroupState.ActiveGroup == InventoryGroupButton && ButtonGroupState.ContainButtonInGroup(go, EquipUI.InventoryGroupButton))
            OnKeyConfirmInventoryGroupButton(go);

        return true;
    }

    private void OnKeyConfirmInventoryGroupButton(GameObject go)
    {
        this.currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
        if (this.itemIdList[this.currentEquipPart][this.currentItemIndex].id == RegularItem.NoItem)
        {
            FF9Sfx.FF9SFX_Play(102);
            return;
        }

        PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
        FF9Sfx.FF9SFX_Play(107);
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        RegularItem oldEquip = player.equip[this.currentEquipPart];
        RegularItem newEquip = this.itemIdList[this.currentEquipPart][this.currentItemIndex].id;
        if (oldEquip != RegularItem.NoItem)
        {
            if (oldEquip == RegularItem.Moonstone)
                ff9item.DecreaseMoonStoneCount();
            ff9item.FF9Item_Add(oldEquip, 1);
        }

        if (ff9item.FF9Item_Remove(newEquip, 1) != 0)
        {
            player.equip[this.currentEquipPart] = newEquip;
            this.UpdateCharacterData(player);
            this.DisplayEquipment();
            this.DisplayParameter();
            this.DisplayPlayer(true);
        }

        this.ClearChangeParameter();
        this.DisplayPlayerArrow(true);
        ButtonGroupState.ActiveGroup = EquipUI.EquipmentGroupButton;
        base.Loading = true;
        this.equipmentSelectionTransition.TweenOut([0], delegate { base.Loading = false; });
        this.equipmentPartCaption.SetActive(true);
    }

    private void OnKeyConfirmEquipmentGroupButton(GameObject go)
    {
        if (!ButtonGroupState.ContainButtonInGroup(go, EquipUI.EquipmentGroupButton))
        {
            this.OnSecondaryGroupClick(go);
            return;
        }

        this.currentEquipPart = go.transform.GetSiblingIndex();
        if (this.currentMenu == EquipUI.SubMenu.Off)
        {
            Boolean equipChanged = false;
            if (this.currentEquipPart != 0)
            {
                PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
                PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
                RegularItem itemId = player.equip[this.currentEquipPart];
                if (itemId != RegularItem.NoItem)
                {
                    FF9Sfx.FF9SFX_Play(107);
                    if (itemId == RegularItem.Moonstone)
                        ff9item.DecreaseMoonStoneCount();
                    ff9item.FF9Item_Add(itemId, 1);
                    player.equip[this.currentEquipPart] = RegularItem.NoItem;
                    this.UpdateCharacterData(player);
                    equipChanged = true;
                }
                else
                {
                    FF9Sfx.FF9SFX_Play(102);
                }
            }
            else
            {
                FF9Sfx.FF9SFX_Play(102);
            }

            this.DisplayParameter();
            this.DisplayEquipment();
            this.DisplayHelp();
            if (equipChanged)
            {
                ButtonGroupState.RefreshHelpDialog();
                this.DisplayPlayer(true);
            }
        }
        else
        {
            FF9Sfx.FF9SFX_Play(103);
            this.DisplayPlayerArrow(false);
            this.DisplayInventory();
            String key = String.Empty;
            switch (this.currentEquipPart)
            {
                case 0:
                    key = "WeaponCaption";
                    break;
                case 1:
                    key = "HeadCaption";
                    break;
                case 2:
                    key = "WristCaption";
                    break;
                case 3:
                    key = "ArmorCaption";
                    break;
                default:
                    key = "AccessoryCaption";
                    break;
            }

            this.selectedCaption.key = key;
            base.Loading = true;
            this.equipmentSelectionTransition.TweenIn([0], delegate
            {
                base.Loading = false;
                ButtonGroupState.RemoveCursorMemorize(EquipUI.InventoryGroupButton);
                ButtonGroupState.ActiveGroup = EquipUI.InventoryGroupButton;
                this.equipmentPartCaption.SetActive(false);
                this.currentItemIndex = 0;
                this.DisplayParameter();
                this.DisplayEquiptmentInfo();
                this.DisplayHelp();
            });
        }
    }

    private void OnKeyConfirmSubMenuGroupButton(GameObject go)
    {
        this.currentMenu = this.GetSubMenuFromGameObject(go);
        switch (this.currentMenu)
        {
            case EquipUI.SubMenu.Equip:
            case EquipUI.SubMenu.Off:
                FF9Sfx.FF9SFX_Play(103);
                ButtonGroupState.ActiveGroup = EquipUI.EquipmentGroupButton;
                ButtonGroupState.SetSecondaryOnGroup(EquipUI.SubMenuGroupButton);
                ButtonGroupState.HoldActiveStateOnGroup(EquipUI.SubMenuGroupButton);
                this.DisplaySubMenuArrow(false);
                this.DisplayEquiptmentInfo();
                this.DisplayParameter();
                break;
            case EquipUI.SubMenu.Optimize:
                PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
                if (_equipForAbilityLearning)
                    this.EquipForAbilityLearning();
                else
                    this.EquipStrongest();
                this.DisplayPlayer(true);
                this.DisplayParameter();
                this.DisplayEquipment();
                break;
        }
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (!base.OnKeyCancel(go))
            return true;

        if (ButtonGroupState.ActiveGroup == EquipUI.SubMenuGroupButton)
        {
            FF9Sfx.FF9SFX_Play(101);
            this.fastSwitch = false;
            this.Hide(delegate
            {
                PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = false;
                PersistenSingleton<UIManager>.Instance.MainMenuScene.CurrentSubMenu = MainMenuUI.SubMenu.Equip;
                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.MainMenu);
            });
        }
        else if (ButtonGroupState.ActiveGroup == EquipUI.EquipmentGroupButton)
        {
            FF9Sfx.FF9SFX_Play(101);
            ButtonGroupState.ActiveGroup = EquipUI.SubMenuGroupButton;
            this.ClearChangeParameter();
            this.ClearEquipmentInfo();
            this.DisplaySubMenuArrow(true);
        }
        else if (ButtonGroupState.ActiveGroup == EquipUI.InventoryGroupButton)
        {
            FF9Sfx.FF9SFX_Play(101);
            this.DisplayPlayerArrow(true);
            this.ClearChangeParameter();
            ButtonGroupState.ActiveGroup = EquipUI.EquipmentGroupButton;
            this.DisplayEquiptmentInfo();
            base.Loading = true;
            this.equipmentSelectionTransition.TweenOut([0], delegate { base.Loading = false; });
            this.equipmentPartCaption.SetActive(true);
        }

        return true;
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (!base.OnItemSelect(go))
            return true;

        if (ButtonGroupState.ActiveGroup == EquipUI.SubMenuGroupButton && this.currentMenu != this.GetSubMenuFromGameObject(go))
        {
            this.currentMenu = this.GetSubMenuFromGameObject(go);
            this.DisplayEquipment();
        }

        if (ButtonGroupState.ActiveGroup == EquipUI.EquipmentGroupButton)
        {
            if (this.currentEquipPart != go.transform.GetSiblingIndex())
            {
                this.currentEquipPart = go.transform.GetSiblingIndex();
                this.DisplayEquiptmentInfo();
                this.DisplayParameter();
            }
        }
        else if (ButtonGroupState.ActiveGroup == EquipUI.InventoryGroupButton && this.currentItemIndex != go.GetComponent<RecycleListItem>().ItemDataIndex)
        {
            this.currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
            this.DisplayEquiptmentInfo();
            this.DisplayParameter();
        }

        return true;
    }

    public override Boolean OnKeySpecial(GameObject go)
    {
        if (!base.OnKeySpecial(go) || ButtonGroupState.ActiveGroup != SubMenuGroupButton)
            return true;

        if (!PersistenSingleton<UIManager>.Instance.MainMenuScene.IsSubMenuEnabled(MainMenuUI.SubMenu.Ability))
        {
            FF9Sfx.FF9SFX_Play(102);
            return true;
        }
        FF9Sfx.FF9SFX_Play(103);
        this.fastSwitch = true;
        this.Hide(delegate
        {
            this.RemoveCursorMemorize();
            PersistenSingleton<UIManager>.Instance.AbilityScene.CurrentPartyIndex = this.currentPartyIndex;
            PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Ability);
        });

        return true;
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        if (!base.OnKeyLeftBumper(go)
            || (ButtonGroupState.ActiveGroup != EquipUI.SubMenuGroupButton && ButtonGroupState.ActiveGroup != EquipUI.EquipmentGroupButton)
            || !this.CharacterArrowPanel.activeSelf)
            return true;

        FF9Sfx.FF9SFX_Play(1047);
        Int32 prevIndex = ff9play.FF9Play_GetPrev(this.currentPartyIndex);
        if (prevIndex != this.currentPartyIndex)
        {
            this.currentPartyIndex = prevIndex;
            PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
            String spritName = FF9UIDataTool.AvatarSpriteName(player.info.serial_no);
            base.Loading = true;
            Boolean isKnockOut = player.cur.hp == 0;
            this.avatarTransition.Change(spritName, HonoAvatarTweenPosition.Direction.LeftToRight, isKnockOut, delegate
            {
                DisplayPlayer(true);
                base.Loading = false;
            });
            SwitchCharacter(false);
        }

        return true;
    }

    public override Boolean OnKeyRightBumper(GameObject go)
    {
        if (!base.OnKeyRightBumper(go)
            || (ButtonGroupState.ActiveGroup != SubMenuGroupButton && ButtonGroupState.ActiveGroup != EquipmentGroupButton)
            || !CharacterArrowPanel.activeSelf)
            return true;

        FF9Sfx.FF9SFX_Play(1047);
        Int32 nextIndex = ff9play.FF9Play_GetNext(this.currentPartyIndex);
        if (nextIndex != this.currentPartyIndex)
        {
            this.currentPartyIndex = nextIndex;
            PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
            String spritName = FF9UIDataTool.AvatarSpriteName(player.info.serial_no);
            Boolean isKnockOut = player.cur.hp == 0;
            this.avatarTransition.Change(spritName, HonoAvatarTweenPosition.Direction.RightToLeft, isKnockOut, delegate
            {
                this.DisplayPlayer(true);
                base.Loading = false;
            });
            this.SwitchCharacter(false);
            base.Loading = true;
        }

        return true;
    }

    private void OnSecondaryGroupClick(GameObject go)
    {
        ButtonGroupState.HoldActiveStateOnGroup(go, EquipUI.SubMenuGroupButton);
        if (ButtonGroupState.ActiveGroup == EquipUI.EquipmentGroupButton)
        {
            FF9Sfx.muteSfx = true;
            this.OnKeyCancel(this.equipmentHud.Self.GetChild(this.currentEquipPart));
            FF9Sfx.muteSfx = false;
            this.OnKeyConfirm(go);
        }
    }

    private void DisplayHelp()
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        CharacterEquipment equip = player.equip;
        for (Int32 i = 0; i < CharacterEquipment.Length; i++)
        {
            RegularItem itemId = equip[i];
            ButtonGroupState buttonGroupState = i switch
            {
                0 => this.equipmentHud.Weapon.Button,
                1 => this.equipmentHud.Head.Button,
                2 => this.equipmentHud.Wrist.Button,
                3 => this.equipmentHud.Body.Button,
                4 => this.equipmentHud.Accessory.Button,
                _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
            };

            if (itemId != RegularItem.NoItem)
            {
                buttonGroupState.Help.TextKey = String.Empty;
                buttonGroupState.Help.Text = FF9TextTool.ItemHelpDescription(itemId);
            }
            else
            {
                buttonGroupState.Help.TextKey = Localization.Get("NoEquipHelp");
            }
        }
    }

    private void DisplaySubMenuArrow(Boolean isEnable)
    {
        this.submenuArrowGameObject.SetActive(isEnable && !FF9StateSystem.PCPlatform);
    }

    private void DisplayPlayerArrow(Boolean isEnable)
    {
        if (isEnable)
        {
            Int32 playerCount = FF9StateSystem.Common.FF9.party.member.Count(p => p != null);
            this.CharacterArrowPanel.SetActive(playerCount > 1);
        }
        else
        {
            this.CharacterArrowPanel.SetActive(false);
        }
    }

    private void DisplayPlayer(Boolean updateAvatar)
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        FF9UIDataTool.DisplayCharacterDetail(player, this.characterHud);
        if (updateAvatar)
            FF9UIDataTool.DisplayCharacterAvatar(player, default(Vector3), default(Vector3), this.characterHud.AvatarSprite, false);
    }

    private void DisplayParameter()
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        Boolean checkOtherEquip = ButtonGroupState.ActiveGroup == EquipUI.InventoryGroupButton && this.currentItemIndex >= 0 && this.itemIdList[this.currentEquipPart][this.currentItemIndex].id != RegularItem.NoItem;
        Boolean checkUnequip = ButtonGroupState.ActiveGroup == EquipUI.EquipmentGroupButton && this.currentMenu == SubMenu.Off && this.currentEquipPart != 0 && player.equip[this.currentEquipPart] != RegularItem.NoItem;
        if (checkOtherEquip || checkUnequip)
        {
            for (Int32 i = 0; i < 9; i++)
            {
                this.parameterHud.ArrowSprite[i].gameObject.SetActive(true);
                this.parameterHud.NewParameterLabel[i].gameObject.SetActive(true);
            }
            RegularItem itemEquipped = player.equip[this.currentEquipPart];
            UInt32 currentMaxHp = player.max.hp;
            UInt32 currentMaxMp = player.max.mp;
            UInt32 previewMaxHp;
            UInt32 previewMaxMp;
            Int32[] currentStats = new Int32[9];
            Int32[] previewStats = new Int32[9];
            if (checkOtherEquip)
                player.equip[this.currentEquipPart] = this.itemIdList[this.currentEquipPart][this.currentItemIndex].id;
            else
                player.equip[this.currentEquipPart] = RegularItem.NoItem;
            if (player.equip[this.currentEquipPart] != itemEquipped)
            {
                ff9play.FF9Play_Update(player);
                previewMaxHp = player.max.hp;
                previewMaxMp = player.max.mp;
                for (Int32 i = 0; i < 9; i++)
                    previewStats[i] = player.GetPlayerStat(i);
                player.equip[this.currentEquipPart] = itemEquipped;
                ff9play.FF9Play_Update(player);
                for (Int32 i = 0; i < 9; i++)
                    currentStats[i] = player.GetPlayerStat(i);
            }
            else
            {
                previewMaxHp = currentMaxHp;
                previewMaxMp = currentMaxMp;
                for (Int32 i = 0; i < 9; i++)
                {
                    currentStats[i] = player.GetPlayerStat(i);
                    previewStats[i] = currentStats[i];
                }
            }
            this.characterHud.HPMaxLabel.rawText = previewMaxHp.ToString();
            this.characterHud.MPMaxLabel.rawText = previewMaxMp.ToString();
            this.characterHud.HPMaxLabel.color = currentMaxHp < previewMaxHp ? FF9TextTool.Green : (currentMaxHp > previewMaxHp ? FF9TextTool.Red : FF9TextTool.White);
            this.characterHud.MPMaxLabel.color = currentMaxMp < previewMaxMp ? FF9TextTool.Green : (currentMaxMp > previewMaxMp ? FF9TextTool.Red : FF9TextTool.White);
            for (Int32 i = 0; i < 9; i++)
            {
                this.parameterHud.ParameterLabel[i].rawText = currentStats[i].ToString();
                this.parameterHud.NewParameterLabel[i].rawText = previewStats[i].ToString();
                if (currentStats[i] < previewStats[i])
                {
                    this.parameterHud.ArrowTween[i].enabled = true;
                    this.parameterHud.NewParameterLabel[i].color = FF9TextTool.Green;
                }
                else if (currentStats[i] > previewStats[i])
                {
                    this.parameterHud.ArrowTween[i].enabled = true;
                    this.parameterHud.NewParameterLabel[i].color = FF9TextTool.Red;
                }
                else
                {
                    this.parameterHud.ArrowTween[i].enabled = false;
                    this.parameterHud.ArrowSprite[i].color = new Color(1f, 1f, 1f, 0.7058824f);
                    this.parameterHud.NewParameterLabel[i].color = FF9TextTool.White;
                }
            }
        }
        else
        {
            for (Int32 i = 0; i < 9; i++)
                this.parameterHud.ParameterLabel[i].rawText = player.GetPlayerStat(i).ToString();
            this.ClearChangeParameter();
        }
    }

    private void DisplayEquipment()
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        if (this.currentMenu == SubMenu.Off)
        {
            FF9UIDataTool.DisplayItem(player.equip[0], this.equipmentHud.Weapon.IconSprite, this.equipmentHud.Weapon.NameLabel, false);
            this.weaponPartColonLabel.color = FF9TextTool.Gray;
            this.weaponPartIconSprite.alpha = 0.5f;
        }
        else
        {
            FF9UIDataTool.DisplayItem(player.equip[0], this.equipmentHud.Weapon.IconSprite, this.equipmentHud.Weapon.NameLabel, true);
            this.weaponPartColonLabel.color = FF9TextTool.White;
            this.weaponPartIconSprite.alpha = 1f;
        }

        FF9UIDataTool.DisplayItem(player.equip[1], this.equipmentHud.Head.IconSprite, this.equipmentHud.Head.NameLabel, true);
        FF9UIDataTool.DisplayItem(player.equip[2], this.equipmentHud.Wrist.IconSprite, this.equipmentHud.Wrist.NameLabel, true);
        FF9UIDataTool.DisplayItem(player.equip[3], this.equipmentHud.Body.IconSprite, this.equipmentHud.Body.NameLabel, true);
        FF9UIDataTool.DisplayItem(player.equip[4], this.equipmentHud.Accessory.IconSprite, this.equipmentHud.Accessory.NameLabel, true);
        this.DisplayHelp();
    }

    private void DisplayEquiptmentInfo()
    {
        this.currentAbilityPage = -1;
        if (ButtonGroupState.ActiveGroup == EquipUI.EquipmentGroupButton || ButtonGroupState.ActiveGroup == EquipUI.InventoryGroupButton)
        {
            PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
            String spriteName = this.currentEquipPart switch
            {
                0 => "icon_equip_0",
                1 => "icon_equip_1",
                2 => "icon_equip_2",
                3 => "icon_equip_3",
                4 => "icon_equip_4",
                _ => String.Empty
            };

            this.equipmentSelectHud.Self.SetActive(true);
            this.equipmentSelectHud.PartIconSprite.spriteName = spriteName;
            RegularItem itemId;
            FF9ITEM_DATA itemData;
            if (ButtonGroupState.ActiveGroup == EquipUI.EquipmentGroupButton)
            {
                itemId = player.equip[this.currentEquipPart];
                itemData = ff9item._FF9Item_Data[itemId];
            }
            else
            {
                if (this.currentItemIndex == -1)
                    return;
                itemId = this.itemIdList[this.currentEquipPart][this.currentItemIndex].id;
                itemData = ff9item._FF9Item_Data[itemId];
            }

            FF9UIDataTool.DisplayItem(itemId, this.equipmentSelectHud.IconSprite, this.equipmentSelectHud.NameLabel, true);
            this.equipmentAbilitySelectHudList[0].Self.SetActive(false);
            this.equipmentAbilitySelectHudList[1].Self.SetActive(false);
            this.equipmentAbilitySelectHudList[2].Self.SetActive(false);
            Int32 abilPageCount = (itemData.ability.Sum(abilId => abilId != 0 ? 1 : 0) + 2) / 3;
            if (abilPageCount <= 1)
            {
                Int32 abilHudSlot = 0;
                foreach (Int32 abilityId in itemData.ability)
                {
                    if (abilityId == 0)
                        continue;
                    DisplayAbilityState(this.equipmentAbilitySelectHudList[abilHudSlot], player, abilityId, false);
                    abilHudSlot++;
                }
            }
            else
            {
                this.currentAbilityPage = 0;
                this.UpdateAbilityPage(player, itemData);
            }
        }
    }

    private void UpdateAbilityPage(PLAYER player, FF9ITEM_DATA itemData)
    {
        Int32 abilIndex = 0;
        Int32 abilHudSlot = 0;
        foreach (Int32 abilityId in itemData.ability)
        {
            if (abilityId == 0)
                continue;
            abilIndex++;
            if (abilIndex <= this.currentAbilityPage * 3)
                continue;
            DisplayAbilityState(this.equipmentAbilitySelectHudList[abilHudSlot], player, abilityId, true);
            abilHudSlot++;
            if (abilHudSlot >= 3)
                break;
        }
        while (abilHudSlot < 3)
            DisplayAbilityState(this.equipmentAbilitySelectHudList[abilHudSlot++], player, 0, false);
        this.currentAbilityPageCounter = 3f;
    }

    private static void DisplayAbilityState(AbilityItemHUD hud, PLAYER player, Int32 abilityId, Boolean fadingName)
    {
        if (abilityId == 0)
        {
            hud.Self.SetActive(false);
            return;
        }
        hud.Self.SetActive(true);
        String abilityName = ff9abil.IsAbilityActive(abilityId) ? FF9TextTool.ActionAbilityName(ff9abil.GetActiveAbilityFromAbilityId(abilityId)) : FF9TextTool.SupportAbilityName(ff9abil.GetSupportAbilityFromAbilityId(abilityId));
        if (fadingName)
            abilityName = "[ANIM=TextRGBA,Sinus,1,Constant,2,Sinus,3,0,1,1,0]" + abilityName;
        if (ff9abil.FF9Abil_HasAp(player))
        {
            Boolean hasAbil = ff9abil.FF9Abil_GetIndex(player, abilityId) >= 0;
            String stoneSprite;
            if (hasAbil)
            {
                if (ff9abil.IsAbilityActive(abilityId))
                    stoneSprite = "ability_stone";
                else
                    stoneSprite = ff9abil.FF9Abil_IsEnableSA(player.saExtended, ff9abil.GetSupportAbilityFromAbilityId(abilityId)) ? "skill_stone_on" : "skill_stone_off";
            }
            else
            {
                stoneSprite = ff9abil.IsAbilityActive(abilityId) ? "ability_stone_null" : "skill_stone_null";
            }
            if (hasAbil)
            {
                Boolean isShowText = ff9abil.IsAbilitySupport(abilityId) || (ff9abil.GetActionAbility(abilityId).Type & 2) == 0;
                hud.APBar.Self.SetActive(true);
                FF9UIDataTool.DisplayAPBar(player, abilityId, isShowText, hud.APBar);
            }
            else
            {
                hud.APBar.Self.SetActive(false);
            }
            hud.NameLabel.SetText(abilityName);
            hud.IconSprite.spriteName = stoneSprite;
            hud.NameLabel.color = hasAbil ? FF9TextTool.White : FF9TextTool.Gray;
        }
        else
        {
            String stoneSprite = ff9abil.IsAbilityActive(abilityId) ? "ability_stone_null" : "skill_stone_null";
            hud.NameLabel.SetText(abilityName);
            hud.IconSprite.spriteName = stoneSprite;
            hud.NameLabel.color = FF9TextTool.Gray;
            hud.APBar.Self.SetActive(false);
        }
    }

    private void DisplayInventory()
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        UInt64 characterMask = ff9feqp.GetCharacterEquipMask(player);
        ItemType equipSlotMask = this.partMask[this.currentEquipPart];
        switch (this.currentEquipPart)
        {
            case 0:
                this.equipmentListCaption.rawText = Localization.Get("WeaponCaption");
                break;
            case 1:
                this.equipmentListCaption.rawText = Localization.Get("HeadCaption");
                break;
            case 2:
                this.equipmentListCaption.rawText = Localization.Get("WristCaption");
                break;
            case 3:
                this.equipmentListCaption.rawText = Localization.Get("ArmorCaption");
                break;
            case 4:
                this.equipmentListCaption.rawText = Localization.Get("AccessoryCaption");
                break;
            default:
                this.equipmentListCaption.rawText = Localization.Get("WeaponCaption");
                break;
        }

        List<FF9ITEM> equipList = this.itemIdList[this.currentEquipPart];
        equipList.Clear();
        int resultIndex = 0;
        foreach (FF9ITEM item in FF9StateSystem.Common.FF9.item)
        {
            if (item.count <= 0)
                continue;

            FF9ITEM_DATA itemData = ff9item._FF9Item_Data[item.id];

            if (CanEquip(item, itemData, player, characterMask, equipSlotMask))
            {
                equipList.Add(item);
                resultIndex++;
            }
        }

        equipList.Sort(
            (i1, i2)
            =>
            {
                if (i1 == i2)
                    return 0;
                Single lvl1 = ff9item._FF9Item_Data[i1.id].eq_lv;
                Single lvl2 = ff9item._FF9Item_Data[i2.id].eq_lv;
                return lvl1 > lvl2 || (lvl1 == lvl2 && i1.id > i2.id) ? 1 : -1;
            });

        if (equipList.Count == 0)
            equipList.Add(new FF9ITEM(RegularItem.NoItem, 0));

        List<ListDataTypeBase> equipTable = new List<ListDataTypeBase>();
        foreach (FF9ITEM itemData in equipList)
        {
            equipTable.Add(new EquipInventoryListData
            {
                ItemData = itemData
            });
        }

        if (this.equipSelectScrollList.ItemsPool.Count == 0)
        {
            this.equipSelectScrollList.PopulateListItemWithData = this.DisplayInventoryInfo;
            this.equipSelectScrollList.OnRecycleListItemClick += this.OnListItemClick;
            this.equipSelectScrollList.InitTableView(equipTable, 0);
        }
        else
        {
            this.equipSelectScrollList.SetOriginalData(equipTable);
            this.equipSelectScrollList.JumpToIndex(0, false);
        }
    }

    private Boolean CanEquip(FF9ITEM item, FF9ITEM_DATA itemData, PLAYER player, UInt64 characterMask, ItemType equipSlotMask)
    {
        if ((itemData.equip & characterMask) == 0)
            return false;
        if ((itemData.type & equipSlotMask) == 0)
            return false;
        if (!String.IsNullOrEmpty(itemData.use_condition))
        {
            Expression c = new Expression(itemData.use_condition);
            NCalcUtility.InitializeExpressionPlayer(ref c, player);
            c.Parameters["CommandId"] = (Int32)BattleCommandId.None;
            c.Parameters["CommandMenu"] = (Int32)BattleCommandMenu.None;
            c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
            c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            return NCalcUtility.EvaluateNCalcCondition(c.Evaluate());
        }
        return true;
    }

    private void DisplayInventoryInfo(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        EquipInventoryListData equipInventoryListData = (EquipInventoryListData)data;
        ItemListDetailWithIconHUD itemListDetailWithIconHUD = new ItemListDetailWithIconHUD(item.gameObject, true);
        if (isInit)
        {
            this.DisplayWindowBackground(item.gameObject, null);
            itemListDetailWithIconHUD.Button.Help.Enable = false;
        }

        if (equipInventoryListData.ItemData.id != RegularItem.NoItem)
        {
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, true);
            itemListDetailWithIconHUD.IconSprite.gameObject.SetActive(true);
            itemListDetailWithIconHUD.NameLabel.gameObject.SetActive(true);
            itemListDetailWithIconHUD.NumberLabel.gameObject.SetActive(true);
            FF9UIDataTool.DisplayItem(equipInventoryListData.ItemData.id, itemListDetailWithIconHUD.IconSprite, itemListDetailWithIconHUD.NameLabel, true);
            itemListDetailWithIconHUD.NumberLabel.rawText = equipInventoryListData.ItemData.count.ToString();
            itemListDetailWithIconHUD.Button.Help.Enable = true;
            itemListDetailWithIconHUD.Button.Help.TextKey = String.Empty;
            itemListDetailWithIconHUD.Button.Help.Text = FF9TextTool.ItemHelpDescription(equipInventoryListData.ItemData.id);
        }
        else
        {
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, false);
            itemListDetailWithIconHUD.IconSprite.gameObject.SetActive(false);
            itemListDetailWithIconHUD.NameLabel.gameObject.SetActive(false);
            itemListDetailWithIconHUD.NumberLabel.gameObject.SetActive(false);
            itemListDetailWithIconHUD.Button.Help.Enable = false;
            itemListDetailWithIconHUD.Button.Help.TextKey = String.Empty;
            itemListDetailWithIconHUD.Button.Help.Text = String.Empty;
        }
    }

    private void ClearEquipmentInfo()
    {
        this.equipmentSelectHud.Self.SetActive(false);
        this.equipmentAbilitySelectHudList[0].Self.SetActive(false);
        this.equipmentAbilitySelectHudList[1].Self.SetActive(false);
        this.equipmentAbilitySelectHudList[2].Self.SetActive(false);
    }

    private void ClearChangeParameter()
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        this.characterHud.HPMaxLabel.rawText = player.max.hp.ToString();
        this.characterHud.MPMaxLabel.rawText = player.max.mp.ToString();
        this.characterHud.HPMaxLabel.color = FF9TextTool.White;
        this.characterHud.MPMaxLabel.color = FF9TextTool.White;
        for (Int32 i = 0; i < 9; i++)
        {
            this.parameterHud.ArrowSprite[i].gameObject.SetActive(false);
            this.parameterHud.NewParameterLabel[i].gameObject.SetActive(false);
        }
    }

    private void InitialData()
    {
        this.ClearEquipmentInfo();
    }

    private void SwitchCharacter(Boolean updateAvatar)
    {
        this.InitialData();
        this.DisplayPlayerArrow(true);
        this.DisplayPlayer(updateAvatar);
        this.DisplayParameter();
        this.DisplayEquipment();
        this.DisplayEquiptmentInfo();
    }

    private void UpdateCharacterData(PLAYER player)
    {
        ff9feqp.FF9FEqp_UpdatePlayer(player);
    }

    private void UpdateCharacterSA(PLAYER player)
    {
        ff9feqp.FF9FEqp_UpdateSA(player);
    }

    private void EquipForAbilityLearning()
    {
        const Int32 itemTypeCount = 5;

        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        CharacterEquipment equipment = player.equip;
        UInt64 characterMask = ff9feqp.GetCharacterEquipMask(player);

        LearnableItemAbilities learnable = new LearnableItemAbilities(player, characterMask);

        RegularItem[] toRemove = new RegularItem[itemTypeCount];
        RegularItem[] toKeep = new RegularItem[itemTypeCount];
        RegularItem[] toLearn = new RegularItem[itemTypeCount];
        RegularItem[] toEquip = new RegularItem[itemTypeCount];
        for (Int32 i = 0; i < itemTypeCount; i++)
        {
            toRemove[i] = RegularItem.NoItem;
            toKeep[i] = RegularItem.NoItem;
            toLearn[i] = RegularItem.NoItem;
            toEquip[i] = RegularItem.NoItem;
        }

        Boolean canLearn = false;
        Boolean canEquip = false;

        for (Int32 itemType = 0; itemType < itemTypeCount; itemType++)
        {
            RegularItem betterItemId = RegularItem.NoItem;
            Single betterLevel = Single.MinValue;
            RegularItem learnableItemId = RegularItem.NoItem;
            Single learnableLevel = Single.MaxValue;
            Int32 learnableLeftAp = Int32.MaxValue;

            RegularItem itemId = equipment[itemType];
            if (itemId != RegularItem.NoItem)
            {
                FF9ITEM_DATA equipedItem = ff9item._FF9Item_Data[itemId];

                betterLevel = equipedItem.eq_lv;
                betterItemId = itemId;

                if (learnable.IsLearnable(itemId, out learnableLeftAp))
                {
                    toLearn[itemType] = itemId;
                    learnableItemId = itemId;
                    learnableLevel = equipedItem.eq_lv;
                }
                else
                {
                    toRemove[itemType] = itemId;
                }
            }

            Boolean equiptable = false;
            foreach (FF9ITEM item in FF9StateSystem.Common.FF9.item)
            {
                if (item.count <= 0)
                    continue;

                FF9ITEM_DATA itemData = ff9item._FF9Item_Data[item.id];
                if (!CanEquip(item, itemData, player, characterMask, this.partMask[itemType]))
                    continue;

                if (itemData.eq_lv > betterLevel)
                {
                    betterItemId = item.id;
                    betterLevel = itemData.eq_lv;
                    equiptable = true;
                }

                Int32 leftAp;
                if (itemData.eq_lv < learnableLevel && learnable.IsLearnable(item.id, out leftAp) && leftAp <= learnableLeftAp)
                {
                    learnableItemId = item.id;
                    learnableLevel = itemData.eq_lv;
                    canLearn = true;
                }
            }

            toLearn[itemType] = learnableItemId;
            toEquip[itemType] = betterItemId;

            if (equiptable && learnableItemId == RegularItem.NoItem)
                canEquip = true;
        }

        for (Int32 itemType = 0; itemType < itemTypeCount; itemType++)
        {
            RegularItem itemId = toLearn[itemType];
            if (itemId != RegularItem.NoItem)
            {
                player.equip.Change(itemType, itemId);
                continue;
            }

            if (canLearn || !canEquip)
            {
                if (itemType != 0 && toRemove[itemType] != RegularItem.NoItem)
                    player.equip.Change(itemType, RegularItem.NoItem);

                continue;
            }

            itemId = toEquip[itemType];
            if (itemId != RegularItem.NoItem)
                player.equip.Change(itemType, itemId);
        }

        this.UpdateCharacterData(player);
        FF9Sfx.FF9SFX_Play(107);
    }

    private sealed class LearnableItemAbilities
    {
        private readonly Dictionary<RegularItem, Int32> _itemsWithLearnableAbilities;

        public LearnableItemAbilities(PLAYER player, UInt64 characterMask)
        {
            _itemsWithLearnableAbilities = InitializeMap(player, characterMask);
        }

        public Boolean IsLearnable(RegularItem itemId, out Int32 learnableLeftAp)
        {
            Int32 leftAp;
            if (_itemsWithLearnableAbilities.TryGetValue(itemId, out leftAp))
            {
                learnableLeftAp = leftAp;
                return true;
            }

            learnableLeftAp = Int32.MaxValue;
            return false;
        }

        private static Dictionary<RegularItem, Int32> InitializeMap(PLAYER player, UInt64 characterMask)
        {
            if (!ff9abil._FF9Abil_PaData.ContainsKey(player.PresetId))
                return new Dictionary<RegularItem, Int32>(0);

            Dictionary<Int32, Int32> abilityIdToLeftAp = new Dictionary<Int32, Int32>(128);

            CharacterAbility[] abilArray = ff9abil._FF9Abil_PaData[player.PresetId];
            foreach (CharacterAbility ability in abilArray)
            {
                Int32 abilityId = ability.Id;

                Int32 cur = ff9abil.FF9Abil_GetAp(player, abilityId);
                Int32 max = ff9abil.FF9Abil_GetMax(player, abilityId);

                if (cur < max)
                {
                    if (cur > 0)
                        abilityIdToLeftAp.Add(abilityId, max - cur);
                    else
                        abilityIdToLeftAp.Add(abilityId, Int32.MaxValue);
                }
            }

            Dictionary<UInt64, FF9ITEM_DATA> keyToItem = new Dictionary<UInt64, FF9ITEM_DATA>(255);
            Dictionary<FF9ITEM_DATA, RegularItem> itemDataToItemId = new Dictionary<FF9ITEM_DATA, RegularItem>(255);
            Dictionary<RegularItem, List<Int32>> itemToAbilities = new Dictionary<RegularItem, List<Int32>>(255);

            foreach (RegularItem itemId in EnumerateItemIds(player))
            {
                FF9ITEM_DATA itemData = ff9item._FF9Item_Data[itemId];
                if ((itemData.equip & characterMask) == 0)
                    continue;

                List<Int32> learnableAbilities = new List<Int32>(3);
                foreach (Int32 abilityId in itemData.ability)
                    if (abilityIdToLeftAp.ContainsKey(abilityId))
                        learnableAbilities.Add(abilityId);

                if (learnableAbilities.Count == 0)
                    continue;

                itemDataToItemId.Add(itemData, itemId);

                learnableAbilities.Sort();

                UInt64 key = itemData.equip;
                for (Int32 i = 0; i < learnableAbilities.Count; i++)
                    key |= (UInt64)learnableAbilities[i] << ((i + 2) * 8);

                FF9ITEM_DATA oldData;
                if (keyToItem.TryGetValue(key, out oldData))
                {
                    if (oldData.eq_lv < itemData.eq_lv)
                        keyToItem[key] = itemData;
                }
                else
                {
                    keyToItem.Add(key, itemData);
                }

                itemToAbilities.Add(itemId, learnableAbilities);
            }


            Dictionary<RegularItem, Int32> dic = new Dictionary<RegularItem, Int32>(keyToItem.Count);
            foreach (var pair in keyToItem)
            {
                FF9ITEM_DATA itemData = pair.Value;
                RegularItem itemId = itemDataToItemId[itemData];
                List<Int32> abilities = itemToAbilities[itemId];

                Int32 totalApLeft = 0;
                foreach (Byte abilityId in abilities)
                {
                    Int32 leftAp = abilityIdToLeftAp[abilityId];
                    if (leftAp != Int32.MaxValue)
                        totalApLeft += leftAp;
                }

                if (totalApLeft == 0)
                    totalApLeft = Int32.MaxValue;

                dic.Add(itemId, totalApLeft);
            }

            return dic;
        }

        private static IEnumerable<RegularItem> EnumerateItemIds(PLAYER player)
        {
            HashSet<RegularItem> values = new HashSet<RegularItem>();

            CharacterEquipment equipment = player.equip;
            for (Int32 i = 0; i < 5; i++)
            {
                RegularItem itemId = equipment[i];
                if (itemId != RegularItem.NoItem)
                    if (values.Add(itemId))
                        yield return itemId;
            }

            foreach (FF9ITEM item in FF9StateSystem.Common.FF9.item)
                if (item.count > 0 && values.Add(item.id))
                    yield return item.id;
        }
    }

    private static Boolean HasLearnableAbilities(FF9ITEM_DATA itemData, PLAYER player, out Int32 leftAp)
    {
        foreach (Int32 abilityId in itemData.ability)
        {
            if (abilityId == 0)
                continue;

            if (ff9abil.FF9Abil_GetIndex(player, abilityId) < 0)
                continue;

            Int32 cur = ff9abil.FF9Abil_GetAp(player, abilityId);
            Int32 max = ff9abil.FF9Abil_GetMax(player, abilityId);
            if (cur < max)
            {
                if (cur > 0)
                    leftAp = max - cur;
                else
                    leftAp = Int32.MaxValue;

                return true;
            }
        }

        leftAp = Int32.MaxValue;
        return false;
    }


    private void EquipStrongest()
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        RegularItem bestItemId = RegularItem.NoItem;
        UInt64 characterMask = ff9feqp.GetCharacterEquipMask(player);
        for (Int32 i = 0; i < 5; i++)
        {
            Single currentLevel = (player.equip[i] == RegularItem.NoItem) ? Single.MinValue : ff9item._FF9Item_Data[player.equip[i]].eq_lv;
            Single bestLevel = Single.MinValue;
            foreach (FF9ITEM item in FF9StateSystem.Common.FF9.item)
            {
                if (item.count > 0)
                {
                    FF9ITEM_DATA itemData = ff9item._FF9Item_Data[item.id];
                    if (CanEquip(item, itemData, player, characterMask, this.partMask[i]) && itemData.eq_lv > bestLevel)
                    {
                        bestLevel = itemData.eq_lv;
                        bestItemId = item.id;
                    }
                }
            }

            if (bestLevel > currentLevel)
            {
                if (player.equip[i] != RegularItem.NoItem)
                {
                    if (player.equip[i] == RegularItem.Moonstone)
                        ff9item.DecreaseMoonStoneCount();

                    ff9item.FF9Item_Add(player.equip[i], 1);
                }

                if (ff9item.FF9Item_Remove(bestItemId, 1) != 0)
                    player.equip[i] = bestItemId;
            }
        }

        this.UpdateCharacterData(player);
        FF9Sfx.FF9SFX_Play(107);
    }

    public SubMenu GetSubMenuFromGameObject(GameObject go)
    {
        if (go == this.EquipSubMenu)
            return SubMenu.Equip;
        if (go == this.OptimizeSubMenu)
            return SubMenu.Optimize;
        if (go == this.OffSubMenu)
            return SubMenu.Off;
        return SubMenu.None;
    }

    private void Awake()
    {
        base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
        UIEventListener.Get(this.EquipSubMenu).onClick += this.onClick;
        UIEventListener.Get(this.OptimizeSubMenu).onClick += this.onClick;
        UIEventListener.Get(this.OffSubMenu).onClick += this.onClick;
        this.characterHud = new CharacterDetailHUD(this.CharacterDetailPanel, false);
        this.parameterHud = new ParameterDetailCompareHUD(this.CharacterParameterPanel);
        this.equipmentHud = new EquipmentDetailHud(this.EquipmentPartListPanel.GetChild(0));
        this.weaponPartIconSprite = this.equipmentHud.Weapon.Self.GetChild(2).GetComponent<UISprite>();
        this.weaponPartColonLabel = this.equipmentHud.Weapon.Self.GetChild(3).GetComponent<UILabel>();
        this.equipmentSelectHud = new EquipmentItemListHUD(this.EquipmentAbilityPanel.GetChild(0));
        this.equipmentAbilitySelectHudList[0] = new AbilityItemHUD(this.EquipmentAbilityPanel.GetChild(1).GetChild(0));
        this.equipmentAbilitySelectHudList[1] = new AbilityItemHUD(this.EquipmentAbilityPanel.GetChild(1).GetChild(1));
        this.equipmentAbilitySelectHudList[2] = new AbilityItemHUD(this.EquipmentAbilityPanel.GetChild(1).GetChild(2));
        this.equipmentPartCaption = this.EquipmentPartListPanel.GetChild(1).GetChild(2);
        this.equipmentListCaption = this.EquipmentInventoryListPanel.GetChild(2).GetChild(4).GetChild(0).GetComponent<UILabel>();
        foreach (Transform t in this.EquipmentPartListPanel.GetChild(0).transform)
            UIEventListener.Get(t.gameObject).onClick += this.onClick;

        this.submenuArrowGameObject = this.SubMenuPanel.GetChild(0);
        this.equipSelectScrollList = this.EquipmentInventoryListPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        this._equipSelectPanel = new GOScrollablePanel(this.EquipmentInventoryListPanel);
        this.equipmentSelectionTransition = this.TransitionGroup.GetChild(0).GetComponent<HonoTweenPosition>();
        this.avatarTransition = this.CharacterDetailPanel.GetChild(0).GetChild(6).GetChild(0).GetComponent<HonoAvatarTweenPosition>();
        this.itemIdList.Add(new List<FF9ITEM>());
        this.itemIdList.Add(new List<FF9ITEM>());
        this.itemIdList.Add(new List<FF9ITEM>());
        this.itemIdList.Add(new List<FF9ITEM>());
        this.itemIdList.Add(new List<FF9ITEM>());

        this.selectedCaption = this.EquipmentInventoryListPanel.GetChild(2).GetChild(4).GetChild(0).GetComponent<UILocalize>();

        if (FF9StateSystem.MobilePlatform)
        {
            this.EquipSubMenu.GetComponent<ButtonGroupState>().Help.TextKey = "EquipDetailHelpForMobile";
            this.OptimizeSubMenu.GetComponent<ButtonGroupState>().Help.TextKey = "OptimizeHelpForMobile";
            this.OffSubMenu.GetComponent<ButtonGroupState>().Help.TextKey = "OffEquipmentHelpForMobile";
        }

        this._background = new GOMenuBackground(this.transform.GetChild(6).gameObject, "equip_bg");

        this.equipmentHud.Weapon.NameLabel.rightAnchor.Set(this.EquipmentPartListPanel.transform, 1f, -34);
        this.equipmentHud.Head.NameLabel.rightAnchor.Set(this.EquipmentPartListPanel.transform, 1f, -34);
        this.equipmentHud.Wrist.NameLabel.rightAnchor.Set(this.EquipmentPartListPanel.transform, 1f, -34);
        this.equipmentHud.Body.NameLabel.rightAnchor.Set(this.EquipmentPartListPanel.transform, 1f, -34);
        this.equipmentHud.Accessory.NameLabel.rightAnchor.Set(this.EquipmentPartListPanel.transform, 1f, -34);
        this.equipmentPartCaption.GetComponent<UILabel>().rightAnchor.Set(1f, -40);
        this._equipSelectPanel.Background.Panel.Name.Label.fixedAlignment = true;
        this.EquipSubMenu.GetChild(1).GetComponent<UILabel>().leftAnchor.Set(0f, 0);
        this.EquipSubMenu.GetChild(1).GetComponent<UILabel>().rightAnchor.Set(1f, 0);
        this.EquipSubMenu.GetChild(1).GetComponent<UILabel>().overflowMethod = UILabel.Overflow.ShrinkContent;
        this.OptimizeSubMenu.GetChild(1).GetComponent<UILabel>().leftAnchor.Set(0f, 0);
        this.OptimizeSubMenu.GetChild(1).GetComponent<UILabel>().rightAnchor.Set(1f, 0);
        this.OptimizeSubMenu.GetChild(1).GetComponent<UILabel>().overflowMethod = UILabel.Overflow.ShrinkContent;
        this.OffSubMenu.GetChild(1).GetComponent<UILabel>().leftAnchor.Set(0f, 0);
        this.OffSubMenu.GetChild(1).GetComponent<UILabel>().rightAnchor.Set(1f, 0);
        this.OffSubMenu.GetChild(1).GetComponent<UILabel>().overflowMethod = UILabel.Overflow.ShrinkContent;
    }

    public GameObject EquipSubMenu;
    public GameObject OptimizeSubMenu;
    public GameObject OffSubMenu;
    public GameObject HelpDespLabelGameObject;
    public GameObject SubMenuPanel;
    public GameObject TransitionGroup;
    public GameObject CharacterDetailPanel;
    public GameObject CharacterParameterPanel;
    public GameObject EquipmentAbilityPanel;
    public GameObject CharacterArrowPanel;
    public GameObject EquipmentPartListPanel;
    public GameObject EquipmentInventoryListPanel;
    public GameObject ScreenFadeGameObject;
    private UISprite weaponPartIconSprite;
    private UILabel weaponPartColonLabel;
    private UILabel equipmentListCaption;
    private CharacterDetailHUD characterHud;
    private ParameterDetailCompareHUD parameterHud;
    private EquipmentDetailHud equipmentHud;
    private GameObject equipmentPartCaption;
    private EquipmentItemListHUD equipmentSelectHud;
    private AbilityItemHUD[] equipmentAbilitySelectHudList = new AbilityItemHUD[3];
    private HonoTweenPosition equipmentSelectionTransition;
    private HonoAvatarTweenPosition avatarTransition;
    private GameObject submenuArrowGameObject;
    private RecycleListPopulator equipSelectScrollList;
    private GOScrollablePanel _equipSelectPanel;
    [NonSerialized]
    private GOMenuBackground _background;

    private static String SubMenuGroupButton = "Equip.SubMenu";
    private static String EquipmentGroupButton = "Equip.Equipment";
    private static String InventoryGroupButton = "Equip.Inventory";

    private List<List<FF9ITEM>> itemIdList = new List<List<FF9ITEM>>();
    private Int32 currentPartyIndex;
    private SubMenu currentMenu = SubMenu.None;
    private Int32 currentEquipPart = -1;
    private Int32 currentItemIndex = -1;
    private Boolean fastSwitch;
    private UILocalize selectedCaption;
    [NonSerialized]
    private Int32 currentAbilityPage;
    [NonSerialized]
    private Single currentAbilityPageCounter;

    private ItemType[] partMask = { ItemType.Weapon, ItemType.Helmet, ItemType.Armlet, ItemType.Armor, ItemType.Accessory };

    private Boolean _equipForAbilityLearning;

    public class ParameterDetailCompareHUD
    {
        public GameObject Self;
        public UILabel[] ParameterLabel = new UILabel[9];
        public UILabel[] NewParameterLabel = new UILabel[9];
        public UISprite[] ArrowSprite = new UISprite[9];
        public TweenAlpha[] ArrowTween = new TweenAlpha[9];

        public ParameterDetailCompareHUD(GameObject go)
        {
            this.Self = go;
            Int32 statIndex = 0;
            for (Int32 i = 0; i < 10; i++)
            {
                if (i == 4)
                    continue;
                this.ParameterLabel[statIndex] = go.GetChild(i).GetChild(1).GetComponent<UILabel>();
                this.NewParameterLabel[statIndex] = go.GetChild(i).GetChild(3).GetComponent<UILabel>();
                this.ArrowSprite[statIndex] = go.GetChild(i).GetChild(2).GetComponent<UISprite>();
                this.ArrowTween[statIndex] = go.GetChild(i).GetChild(2).GetComponent<TweenAlpha>();
                statIndex++;
            }
            // Spirit label fix: https://forums.qhimm.com/index.php?topic=14315.msg273804#msg273804
            NewParameterLabel[3].rightAnchor.absolute = 98;
        }
    }

    public enum SubMenu
    {
        Equip,
        Optimize,
        Off,
        None
    }

    public class EquipInventoryListData : ListDataTypeBase
    {
        public FF9ITEM ItemData;
    }
}
