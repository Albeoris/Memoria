using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainMenuUI : UIScene
{
    public static MenuUIControlPanel UIControlPanel { get; set; }

    public Vector3 FrontRowPosition => new Vector3(-418f, 0f, 0f);
    public Vector3 BackRowPosition => new Vector3(-392f, 0f, 0f);

    public MainMenuUI.SubMenu CurrentSubMenu
    {
        get => this.currentMenu;
        set
        {
            this.currentMenu = value;
            ButtonGroupState.SetCursorStartSelect(this.GetGameObjectFromSubMenu(this.currentMenu), MainMenuUI.SubMenuGroupButton);
            ButtonGroupState.RemoveCursorMemorize(MainMenuUI.SubMenuGroupButton);
        }
    }

    public Boolean NeedTweenAndHideSubMenu
    {
        set
        {
            this.isNeedCharacterTween = value;
            this.isNeedHideSubMenu = value;
        }
    }

    public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate afterShowAction = delegate
        {
            this.AfterShowCharacter();
        };
        if (afterFinished != null)
            afterShowAction += afterFinished;
        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(afterShowAction);
        PersistenSingleton<UIManager>.Instance.MenuOpenEvent();
        FF9StateSystem.Settings.SetMasterSkill();
        this.DisplayWindowBackground(this.SubMenuPanel, null);
        this.DisplayCharacter();
        this.DisplayGeneralInfo();
        this.DisplayTime(true);
        if (this.isNeedCharacterTween)
        {
            this.SubMenuPanel.SetActive(true);
            this.HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);
            base.Loading = true;
            this.characterTransition.TweenIn(new Byte[]
            {
                0,
                1,
                2,
                3
            }, delegate
            {
                base.Loading = false;
            });
        }
        this.ItemSubMenu.GetComponentInChildren<UILabel>().color = IsSubMenuEnabled(SubMenu.Item) ? FF9TextTool.White : FF9TextTool.Gray;
        this.AbilitySubMenu.GetComponentInChildren<UILabel>().color = IsSubMenuEnabled(SubMenu.Ability) ? FF9TextTool.White : FF9TextTool.Gray;
        this.EquipSubMenu.GetComponentInChildren<UILabel>().color = IsSubMenuEnabled(SubMenu.Equip) ? FF9TextTool.White : FF9TextTool.Gray;
        this.StatusSubMenu.GetComponentInChildren<UILabel>().color = IsSubMenuEnabled(SubMenu.Status) ? FF9TextTool.White : FF9TextTool.Gray;
        this.OrderSubMenu.GetComponentInChildren<UILabel>().color = IsSubMenuEnabled(SubMenu.Order) ? FF9TextTool.White : FF9TextTool.Gray;
        this.CardSubMenu.GetComponentInChildren<UILabel>().color = IsSubMenuEnabled(SubMenu.Card) ? FF9TextTool.White : FF9TextTool.Gray;
        this.ConfigSubMenu.GetComponentInChildren<UILabel>().color = IsSubMenuEnabled(SubMenu.Config) ? FF9TextTool.White : FF9TextTool.Gray;
        if (this.PartySubMenu != null)
            this.PartySubMenu.GetComponentInChildren<UILabel>().color = IsSubMenuEnabled(SubMenu.Party) ? FF9TextTool.White : FF9TextTool.Gray;
        if (!UIManager.IsUIStateMenu(PersistenSingleton<UIManager>.Instance.PreviousState))
            ImpactfulActionCount = 0;
    }

    public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
    {
        UIScene.SceneVoidDelegate afterHideAction = delegate
        {
            MainMenuUI.UIControlPanel?.ExitMenu();
            if (this.isNeedHideSubMenu)
            {
                SceneDirector.FF9Wipe_FadeInEx(12);
                this.SubMenuPanel.SetActive(false);
            }
        };
        if (afterFinished != null)
            afterHideAction += afterFinished;
        if (this.isNeedCharacterTween)
        {
            this.screenFadePanel.depth = 10;
            this.RemoveCursorMemorize();
        }
        else
        {
            this.screenFadePanel.depth = 7;
            this.submenuTransition.AnimationTime = (!FF9StateSystem.Settings.IsFastForward) ? Configuration.Interface.FadeDuration : Configuration.Interface.FadeDuration / FF9StateSystem.Settings.FastForwardFactor;
            this.submenuTransition.TweenOut(null);
        }
        base.Hide(afterHideAction);
    }

    public void StartSubmenuTweenIn()
    {
        this.submenuTransition.AnimationTime = (!FF9StateSystem.Settings.IsFastForward) ? Configuration.Interface.FadeDuration : Configuration.Interface.FadeDuration / FF9StateSystem.Settings.FastForwardFactor;
        this.submenuTransition.TweenIn((Action)null);
    }

    public void SetSubmenuVisibility(Boolean isVisible)
    {
        this.SubMenuPanel.SetActive(isVisible);
    }

    private void RemoveCursorMemorize()
    {
        this.characterMemorize = (GameObject)null;
        this.characterOrderMemorize = (GameObject)null;
        this.currentMenu = MainMenuUI.SubMenu.Item;
        ButtonGroupState.RemoveCursorMemorize(MainMenuUI.SubMenuGroupButton);
        ButtonGroupState.RemoveCursorMemorize(MainMenuUI.CharacterGroupButton);
        ButtonGroupState.RemoveCursorMemorize(MainMenuUI.OrderGroupButton);
        ButtonGroupState.DisableAllGroup(false);
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go))
        {
            Single shiftFactor = this.PartySubMenu == null ? 1f : 7f / 8f;
            if (ButtonGroupState.ActiveGroup == MainMenuUI.SubMenuGroupButton)
            {
                if (IsSubMenuEnabled(this.GetSubMenuFromGameObject(go)))
                {
                    FF9Sfx.FF9SFX_Play(103);
                    this.currentMenu = this.GetSubMenuFromGameObject(go);
                    switch (this.currentMenu)
                    {
                        case MainMenuUI.SubMenu.Item:
                            this.NeedTweenAndHideSubMenu = false;
                            this.submenuTransition.ShiftContentClip = new Vector2(0f, 9f);
                            this.ItemSubMenu.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, false);
                            this.Hide(delegate
                            {
                                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Item);
                                base.Loading = true;
                            });
                            break;
                        case MainMenuUI.SubMenu.Ability:
                        case MainMenuUI.SubMenu.Equip:
                        case MainMenuUI.SubMenu.Status:
                            this.SetAvailableCharacter(false);
                            this.DisplayHelp(this.currentMenu);
                            if (this.characterMemorize != null && !this.characterMemorize.GetComponent<ButtonGroupState>().enabled)
                                this.characterMemorize = this.CharacterHUDList[this.GetFirstPlayer()].Self;
                            ButtonGroupState.SetCursorMemorize(this.characterMemorize, MainMenuUI.CharacterGroupButton);
                            ButtonGroupState.ActiveGroup = MainMenuUI.CharacterGroupButton;
                            ButtonGroupState.SetSecondaryOnGroup(MainMenuUI.SubMenuGroupButton);
                            ButtonGroupState.HoldActiveStateOnGroup(MainMenuUI.SubMenuGroupButton);
                            break;
                        case MainMenuUI.SubMenu.Order:
                            this.SetAvailableCharacter(true);
                            this.DisplayHelp(this.currentMenu);
                            ButtonGroupState.SetCursorMemorize(this.characterOrderMemorize, MainMenuUI.CharacterGroupButton);
                            ButtonGroupState.ActiveGroup = MainMenuUI.CharacterGroupButton;
                            ButtonGroupState.SetSecondaryOnGroup(MainMenuUI.SubMenuGroupButton);
                            ButtonGroupState.HoldActiveStateOnGroup(MainMenuUI.SubMenuGroupButton);
                            break;
                        case MainMenuUI.SubMenu.Card:
                            this.NeedTweenAndHideSubMenu = false;
                            this.submenuTransition.ShiftContentClip = new Vector2(0f, 9f + shiftFactor * 490f);
                            this.CardSubMenu.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, false);
                            this.Hide(delegate
                            {
                                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Card);
                                base.Loading = true;
                            });
                            break;
                        case MainMenuUI.SubMenu.Config:
                            this.NeedTweenAndHideSubMenu = false;
                            this.submenuTransition.ShiftContentClip = new Vector2(0f, 9f + shiftFactor * (this.PartySubMenu == null ? 588f : 686f));
                            this.ConfigSubMenu.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, false);
                            this.Hide(delegate
                            {
                                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Config);
                                base.Loading = true;
                            });
                            break;
                        case MainMenuUI.SubMenu.Party:
                            this.NeedTweenAndHideSubMenu = false;
                            this.submenuTransition.ShiftContentClip = new Vector2(0f, 9f + shiftFactor * 588f);
                            this.PartySubMenu.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, false);
                            this.Hide(delegate
                            {
                                PersistenSingleton<UIManager>.Instance.PartySettingScene.AccessFromMenu = true;
                                PersistenSingleton<UIManager>.Instance.PartySettingScene.Info = UISceneHelper.GetCurrentPartyForMenu();
                                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.PartySetting);
                                base.Loading = true;
                            });
                            break;
                    }
                }
                else
                {
                    FF9Sfx.FF9SFX_Play(102);
                }
            }
            else if (ButtonGroupState.ActiveGroup == MainMenuUI.CharacterGroupButton)
            {
                if (ButtonGroupState.ContainButtonInGroup(go, MainMenuUI.CharacterGroupButton))
                {
                    FF9Sfx.FF9SFX_Play(103);
                    this.currentCharacterIndex = go.transform.GetSiblingIndex();
                    PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentCharacterIndex];
                    if (player != null)
                    {
                        switch (this.currentMenu)
                        {
                            case MainMenuUI.SubMenu.Ability:
                                this.characterMemorize = go;
                                this.NeedTweenAndHideSubMenu = false;
                                this.submenuTransition.ShiftContentClip = new Vector2(0f, 9f + shiftFactor * 98f);
                                this.AbilitySubMenu.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, false);
                                this.Hide(delegate
                                {
                                    PersistenSingleton<UIManager>.Instance.AbilityScene.CurrentPartyIndex = this.currentCharacterIndex;
                                    PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Ability);
                                });
                                break;
                            case MainMenuUI.SubMenu.Equip:
                                this.characterMemorize = go;
                                this.NeedTweenAndHideSubMenu = false;
                                this.submenuTransition.ShiftContentClip = new Vector2(0f, 9f + shiftFactor * 196f);
                                this.EquipSubMenu.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, false);
                                this.Hide(delegate
                                {
                                    PersistenSingleton<UIManager>.Instance.EquipScene.CurrentPartyIndex = this.currentCharacterIndex;
                                    PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Equip);
                                });
                                break;
                            case MainMenuUI.SubMenu.Status:
                                this.characterMemorize = go;
                                this.NeedTweenAndHideSubMenu = false;
                                this.submenuTransition.ShiftContentClip = new Vector2(0f, 9f + shiftFactor * 294f);
                                this.StatusSubMenu.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, false);
                                this.Hide(delegate
                                {
                                    PersistenSingleton<UIManager>.Instance.StatusScene.CurrentPartyIndex = this.currentCharacterIndex;
                                    PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Status);
                                });
                                break;
                            case MainMenuUI.SubMenu.Order:
                                this.characterOrderMemorize = go;
                                ButtonGroupState.SetCursorMemorize(this.CharacterOrderGameObjectList[this.currentCharacterIndex], MainMenuUI.OrderGroupButton);
                                ButtonGroupState.ActiveGroup = MainMenuUI.OrderGroupButton;
                                ButtonGroupState.HoldActiveStateOnGroup(MainMenuUI.CharacterGroupButton);
                                break;
                        }
                    }
                    else if (this.currentMenu == MainMenuUI.SubMenu.Order)
                    {
                        this.characterOrderMemorize = go;
                        ButtonGroupState.SetCursorMemorize(this.CharacterOrderGameObjectList[this.currentCharacterIndex], MainMenuUI.OrderGroupButton);
                        ButtonGroupState.ActiveGroup = MainMenuUI.OrderGroupButton;
                        ButtonGroupState.HoldActiveStateOnGroup(MainMenuUI.CharacterGroupButton);
                    }
                }
                else
                {
                    this.OnSecondaryGroupClick(go);
                }
            }
            else if (ButtonGroupState.ActiveGroup == MainMenuUI.OrderGroupButton)
            {
                if (ButtonGroupState.ContainButtonInGroup(go, MainMenuUI.OrderGroupButton))
                {
                    FF9Sfx.FF9SFX_Play(103);
                    this.currentOrder = go.transform.parent.GetSiblingIndex();
                    this.ToggleOrder();
                    this.DisplayCharacter();
                    ButtonGroupState.SetCursorMemorize(this.CharacterHUDList[this.currentOrder].Self, MainMenuUI.CharacterGroupButton);
                    ButtonGroupState.ActiveGroup = MainMenuUI.CharacterGroupButton;
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
            if (ButtonGroupState.ActiveGroup == MainMenuUI.SubMenuGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.NeedTweenAndHideSubMenu = true;
                this.Hide(delegate
                {
                    EnabledSubMenus.Clear();
                    if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.World)
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.WorldHUD);
                    else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Battle)
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.BattleHUD);
                    else
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.FieldHUD);
                });
            }
            else if (ButtonGroupState.ActiveGroup == MainMenuUI.CharacterGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                if (this.currentMenu == MainMenuUI.SubMenu.Order)
                    this.characterOrderMemorize = go;
                else
                    this.characterMemorize = go;
                ButtonGroupState.ActiveGroup = MainMenuUI.SubMenuGroupButton;
            }
            else if (ButtonGroupState.ActiveGroup == MainMenuUI.OrderGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                ButtonGroupState.ActiveGroup = MainMenuUI.CharacterGroupButton;
            }
        }
        return true;
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (base.OnItemSelect(go))
        {
            if (ButtonGroupState.ActiveGroup == MainMenuUI.CharacterGroupButton)
            {
                Int32 selectedIndex = go.transform.GetSiblingIndex();
                if (this.currentCharacterIndex != selectedIndex)
                    this.currentCharacterIndex = selectedIndex;
            }
            else if (ButtonGroupState.ActiveGroup == MainMenuUI.OrderGroupButton)
            {
                Int32 selectedIndex = go.transform.parent.GetSiblingIndex();
                if (this.currentOrder != selectedIndex)
                    this.currentOrder = selectedIndex;
            }
        }
        return true;
    }

    public Boolean IsSubMenuEnabled(MainMenuUI.SubMenu subMenu)
    {
        if (EnabledSubMenus.Count == 0)
            return true;
        if (subMenu == SubMenu.Ability && (EnabledSubMenus.Contains("ActiveAbility") || EnabledSubMenus.Contains("SupportingAbility")))
            return true;
        return EnabledSubMenus.Contains(subMenu.ToString());
    }

    public MainMenuUI.SubMenu GetFirstAvailableSubMenu()
    {
        foreach (MainMenuUI.SubMenu subMenu in Enum.GetValues(typeof(MainMenuUI.SubMenu)))
            if (IsSubMenuEnabled(subMenu))
                return subMenu;
        return MainMenuUI.SubMenu.None;
    }

    private void OnSecondaryGroupClick(GameObject go)
    {
        ButtonGroupState.HoldActiveStateOnGroup(go, MainMenuUI.SubMenuGroupButton);
        if (ButtonGroupState.ActiveGroup == MainMenuUI.CharacterGroupButton)
        {
            FF9Sfx.muteSfx = true;
            this.OnKeyCancel(this.CharacterListPanel.GetChild(this.currentCharacterIndex));
            FF9Sfx.muteSfx = false;
            this.OnKeyConfirm(go);
        }
        else if (ButtonGroupState.ActiveGroup == MainMenuUI.OrderGroupButton)
        {
            FF9Sfx.muteSfx = true;
            GameObject child = this.CharacterListPanel.GetChild(this.currentOrder);
            this.OnKeyCancel(child.GetChild(1));
            this.OnKeyCancel(this.CharacterListPanel.GetChild(this.currentCharacterIndex));
            FF9Sfx.muteSfx = false;
            this.OnKeyConfirm(go);
        }
    }

    public void AfterShowCharacter()
    {
        ButtonGroupState.SetCursorStartSelect(this.GetGameObjectFromSubMenu(this.currentMenu), MainMenuUI.SubMenuGroupButton);
        ButtonGroupState.RemoveCursorMemorize(MainMenuUI.SubMenuGroupButton);
        ButtonGroupState.SetPointerDepthToGroup(10, MainMenuUI.SubMenuGroupButton);
        ButtonGroupState.SetPointerDepthToGroup(12, MainMenuUI.OrderGroupButton);
        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(10f, 0f), MainMenuUI.CharacterGroupButton);
        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(30f, -28f), MainMenuUI.OrderGroupButton);
        ButtonGroupState.ActiveGroup = MainMenuUI.SubMenuGroupButton;
    }

    private void DisplayHelp(MainMenuUI.SubMenu currentMenu)
    {
        if (currentMenu == MainMenuUI.SubMenu.Ability || currentMenu == MainMenuUI.SubMenu.Order)
            foreach (CharacterDetailHUD characterHUD in this.CharacterHUDList)
                characterHUD.Self.GetComponent<ButtonGroupState>().Help.TextKey = FF9StateSystem.MobilePlatform ? "TargetHelpMobile" : "TargetHelp";
        else if (currentMenu == MainMenuUI.SubMenu.Equip)
        {
            Int32 memberIndex = 0;
            foreach (CharacterDetailHUD characterHUD in this.CharacterHUDList)
            {
                ButtonGroupState button = characterHUD.Self.GetComponent<ButtonGroupState>();
                String help = Localization.Get(FF9StateSystem.MobilePlatform ? "TargetHelpMobile" : "TargetHelp") + "\n";
                PLAYER player = FF9StateSystem.Common.FF9.party.member[memberIndex];
                if (player != null)
                {
                    for (Int32 i = 0; i < 5; i++)
                    {
                        help += "[ICON=" + (625 + i).ToString() + "] [FEED=1]:[FEED=2]";
                        RegularItem equipId = player.equip[i];
                        if (equipId != RegularItem.NoItem)
                        {
                            FF9ITEM_DATA itemData = ff9item._FF9Item_Data[equipId];
                            String itemIconSpriteName = "item" + itemData.shape.ToString("0#") + "_" + itemData.color.ToString("0#");
                            Int32 spriteKey = FF9UIDataTool.IconSpriteName.FirstOrDefault(pair => pair.Value == itemIconSpriteName).Key;
                            help += $"[ICON={spriteKey}] [FEED=1]{FF9TextTool.ItemName(equipId)}";
                        }
                        if (i < 4)
                            help += "\n";
                    }
                }
                button.Help.TextKey = String.Empty;
                button.Help.Text = help;
                memberIndex++;
            }
        }
        else if (currentMenu == MainMenuUI.SubMenu.Status)
        {
            Int32 memberIndex = 0;
            foreach (CharacterDetailHUD characterHUD in this.CharacterHUDList)
            {
                ButtonGroupState button = characterHUD.Self.GetComponent<ButtonGroupState>();
                String help = Localization.Get(FF9StateSystem.MobilePlatform ? "TargetHelpMobile" : "TargetHelp") + "\n";
                PLAYER player = FF9StateSystem.Common.FF9.party.member[memberIndex];
                if (player != null)
                {
                    UInt32 exp = (player.level < ff9level.LEVEL_COUNT) ? ff9level.CharacterLevelUps[player.level].ExperienceToLevel : player.exp;
                    Int32 expSpriteKey = (Localization.CurrentLanguage == "English(US)" || Localization.CurrentLanguage == "English(UK)" || Localization.CurrentLanguage == "German") ? 82 : 64;
                    help += $"{Localization.Get("EXP")}[XTAB={expSpriteKey}]{player.exp}\n";
                    help += $"{Localization.Get("NextLevel")}[XTAB={expSpriteKey}]{exp - player.exp}";
                }
                button.Help.TextKey = String.Empty;
                button.Help.Text = help;
                memberIndex++;
            }
        }
    }

    private void DisplayCharacter()
    {
        Int32 num = 0;
        PLAYER[] member = FF9StateSystem.Common.FF9.party.member;
        for (Int32 i = 0; i < (Int32)member.Length; i++)
        {
            PLAYER player = member[i];
            CharacterDetailHUD characterDetailHUD = this.CharacterHUDList[num++];
            characterDetailHUD.Self.SetActive(true);
            if (player != null)
            {
                characterDetailHUD.Content.SetActive(true);
                FF9UIDataTool.DisplayCharacterDetail(player, characterDetailHUD);
                FF9UIDataTool.DisplayCharacterAvatar(player, this.FrontRowPosition, this.BackRowPosition, characterDetailHUD.AvatarSprite, true);
            }
            else
            {
                characterDetailHUD.Content.SetActive(false);
            }
        }
    }

    private void DisplayGeneralInfo()
    {
        this.gilLabel.text = Localization.GetWithDefault("GilSymbol").Replace("%", FF9StateSystem.Common.FF9.party.gil.ToString());
        this.locationNameLabel.text = FF9StateSystem.Common.FF9.mapNameStr;
    }

    private void DisplayTime(Boolean ForceUpdateColor)
    {
        Color color = FF9TextTool.White;
        Double num = FF9StateSystem.Settings.time % 360000.0;
        switch ((Int32)(FF9StateSystem.Settings.time / 360000.0))
        {
            case 0:
                color = FF9TextTool.White;
                break;
            case 1:
                color = FF9TextTool.Red;
                break;
            case 2:
                color = FF9TextTool.Yellow;
                break;
            case 3:
                color = FF9TextTool.Cyan;
                break;
            case 4:
                color = FF9TextTool.Magenta;
                break;
            case 5:
                color = FF9TextTool.Green;
                break;
            default:
                num = 359999.0;
                color = FF9TextTool.Green;
                break;
        }
        this.hourLabel.text = ((Int32)(num / 3600.0)).ToString("0#");
        this.minuteLabel.text = ((Int32)(num / 60.0) % 60).ToString("0#");
        this.secondLabel.text = ((Int32)num % 60).ToString("0#");
        if ((Single)((Int32)FF9StateSystem.Settings.time) % 360000f == 0f || ForceUpdateColor)
        {
            this.hourLabel.color = color;
            this.minuteLabel.color = color;
            this.secondLabel.color = color;
            UILabel[] array = this.colonLabel;
            for (Int32 i = 0; i < (Int32)array.Length; i++)
            {
                UILabel uilabel = array[i];
                uilabel.color = color;
            }
        }
    }

    private GameObject GetGameObjectFromSubMenu(MainMenuUI.SubMenu subMenu)
    {
        switch (subMenu)
        {
            case MainMenuUI.SubMenu.Item:
                return this.ItemSubMenu;
            case MainMenuUI.SubMenu.Ability:
                return this.AbilitySubMenu;
            case MainMenuUI.SubMenu.Equip:
                return this.EquipSubMenu;
            case MainMenuUI.SubMenu.Status:
                return this.StatusSubMenu;
            case MainMenuUI.SubMenu.Order:
                return this.OrderSubMenu;
            case MainMenuUI.SubMenu.Card:
                return this.CardSubMenu;
            case MainMenuUI.SubMenu.Config:
                return this.ConfigSubMenu;
            case MainMenuUI.SubMenu.Party:
                return this.PartySubMenu;
            default:
                return this.ItemSubMenu;
        }
    }

    private MainMenuUI.SubMenu GetSubMenuFromGameObject(GameObject go)
    {
        if (go == this.ItemSubMenu)
            return MainMenuUI.SubMenu.Item;
        if (go == this.ConfigSubMenu)
            return MainMenuUI.SubMenu.Config;
        if (go == this.CardSubMenu)
            return MainMenuUI.SubMenu.Card;
        if (go == this.AbilitySubMenu)
            return MainMenuUI.SubMenu.Ability;
        if (go == this.EquipSubMenu)
            return MainMenuUI.SubMenu.Equip;
        if (go == this.StatusSubMenu)
            return MainMenuUI.SubMenu.Status;
        if (go == this.OrderSubMenu)
            return MainMenuUI.SubMenu.Order;
        if (go == this.PartySubMenu)
            return MainMenuUI.SubMenu.Party;
        return MainMenuUI.SubMenu.None;
    }

    private Int32 GetFirstPlayer()
    {
        PLAYER[] member = FF9StateSystem.Common.FF9.party.member;
        for (Int32 i = 0; i < member.Length; i++)
            if (member[i] != null)
                return i;
        return -1;
    }

    private void ToggleOrder()
    {
        if (this.currentCharacterIndex == this.currentOrder)
        {
            if (FF9StateSystem.Common.FF9.party.member[this.currentCharacterIndex] != null)
            {
                PLAYER_INFO info = FF9StateSystem.Common.FF9.party.member[this.currentCharacterIndex].info;
                info.row = (Byte)(info.row ^ 1);
            }
        }
        else
        {
            PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentCharacterIndex];
            FF9StateSystem.Common.FF9.party.member[this.currentCharacterIndex] = FF9StateSystem.Common.FF9.party.member[this.currentOrder];
            FF9StateSystem.Common.FF9.party.member[this.currentOrder] = player;
        }
        ImpactfulActionCount++;
    }

    private void SetAvailableCharacter(Boolean includeEmpty)
    {
        List<CharacterDetailHUD> list = new List<CharacterDetailHUD>();
        if (!includeEmpty)
        {
            foreach (CharacterDetailHUD characterDetailHUD in this.CharacterHUDList)
            {
                if (characterDetailHUD.Content.activeSelf)
                {
                    list.Add(characterDetailHUD);
                    ButtonGroupState.SetButtonEnable(characterDetailHUD.Self, true);
                }
                else
                {
                    ButtonGroupState.SetButtonEnable(characterDetailHUD.Self, false);
                }
            }
        }
        else
        {
            foreach (CharacterDetailHUD characterDetailHUD2 in this.CharacterHUDList)
            {
                list.Add(characterDetailHUD2);
                ButtonGroupState.SetButtonEnable(characterDetailHUD2.Self, true);
            }
        }
        for (Int32 i = 0; i < list.Count; i++)
        {
            Int32 index = i - 1;
            Int32 index2 = i + 1;
            if (i == 0)
            {
                index = list.Count - 1;
            }
            if (i == list.Count - 1)
            {
                index2 = 0;
            }
            UIKeyNavigation component = list[i].Self.GetComponent<UIKeyNavigation>();
            component.onUp = list[index].Self;
            component.onDown = list[index2].Self;
        }
    }

    protected void Update()
    {
        FF9StateSystem.Settings.UpdateTickTime();
        this.DisplayTime(false);
    }

    private void OnGUI()
    {
    }

    protected void Awake()
    {
        base.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
        this.ItemSubMenu = this.SubMenuPanel.GetChild(0).GetChild(0).GetChild(0);
        this.AbilitySubMenu = this.SubMenuPanel.GetChild(0).GetChild(0).GetChild(1);
        this.EquipSubMenu = this.SubMenuPanel.GetChild(0).GetChild(0).GetChild(2);
        this.StatusSubMenu = this.SubMenuPanel.GetChild(0).GetChild(0).GetChild(3);
        this.OrderSubMenu = this.SubMenuPanel.GetChild(0).GetChild(0).GetChild(4);
        this.CardSubMenu = this.SubMenuPanel.GetChild(0).GetChild(0).GetChild(5);
        this.ConfigSubMenu = this.SubMenuPanel.GetChild(0).GetChild(0).GetChild(6);
        if (this.PartySubMenu == null && (Configuration.Hacks.AllCharactersAvailable > 0 || Configuration.Battle.IsMenuEnabledInBattle(SubMenu.Party)))
        {
            UITable table = this.SubMenuPanel.GetChild(0).GetChild(0).GetComponent<UITable>();
            this.PartySubMenu = UnityEngine.Object.Instantiate(this.CardSubMenu);
            UILocalize partyLocalize = this.PartySubMenu.GetChild(0).GetComponent<UILocalize>();
            ButtonGroupState.HelpDetail partyHelp = this.PartySubMenu.GetComponent<ButtonGroupState>().Help;
            partyLocalize.key = "Party";
            partyHelp.TextKey = "Party";
            this.ConfigSubMenu.transform.parent = null;
            this.PartySubMenu.transform.parent = table.transform;
            this.ConfigSubMenu.transform.parent = table.transform;
            this.CardSubMenu.GetComponent<UIKeyNavigation>().onDown = this.PartySubMenu;
            this.PartySubMenu.GetComponent<UIKeyNavigation>().onDown = this.ConfigSubMenu;
            this.ConfigSubMenu.GetComponent<UIKeyNavigation>().onUp = this.PartySubMenu;
            this.PartySubMenu.GetComponent<UIKeyNavigation>().onUp = this.CardSubMenu;
            Vector3 buttonScale = new Vector3(1f, 7f / 8f, 1f);
            this.ItemSubMenu.transform.localScale = buttonScale;
            this.AbilitySubMenu.transform.localScale = buttonScale;
            this.EquipSubMenu.transform.localScale = buttonScale;
            this.StatusSubMenu.transform.localScale = buttonScale;
            this.OrderSubMenu.transform.localScale = buttonScale;
            this.CardSubMenu.transform.localScale = buttonScale;
            this.PartySubMenu.transform.localScale = buttonScale;
            this.ConfigSubMenu.transform.localScale = buttonScale;
            //this.PartySubMenu.active = true;
            table.repositionNow = true;
        }

        UIEventListener.Get(this.ItemSubMenu).Click += onClick;
        UIEventListener.Get(this.AbilitySubMenu).Click += onClick;
        UIEventListener.Get(this.EquipSubMenu).Click += onClick;
        UIEventListener.Get(this.StatusSubMenu).Click += onClick;
        UIEventListener.Get(this.OrderSubMenu).Click += onClick;
        UIEventListener.Get(this.CardSubMenu).Click += onClick;
        UIEventListener.Get(this.ConfigSubMenu).Click += onClick;
        if (this.PartySubMenu != null)
            UIEventListener.Get(this.PartySubMenu).Click += onClick;

        foreach (Transform tr in this.CharacterListPanel.transform)
        {
            GameObject go = tr.gameObject;
            UIEventListener.Get(go).Click += onClick;

            CharacterDetailHUD item = new CharacterDetailHUD(go, false);
            this.CharacterHUDList.Add(item);

            GameObject child = go.GetChild(1);
            UIEventListener.Get(child).Click += onClick;

            this.CharacterOrderGameObjectList.Add(child);
            if (FF9StateSystem.MobilePlatform)
                go.GetComponent<ButtonGroupState>().Help.TextKey = "TargetHelpMobile";
        }

        this.gilLabel = this.GenericInfoPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
        this.hourLabel = this.GenericInfoPanel.GetChild(0).GetChild(1).GetComponent<UILabel>();
        this.minuteLabel = this.GenericInfoPanel.GetChild(0).GetChild(3).GetComponent<UILabel>();
        this.secondLabel = this.GenericInfoPanel.GetChild(0).GetChild(5).GetComponent<UILabel>();
        this.colonLabel = new UILabel[]
        {
            this.GenericInfoPanel.GetChild(0).GetChild(2).GetComponent<UILabel>(),
            this.GenericInfoPanel.GetChild(0).GetChild(4).GetComponent<UILabel>()
        };
        this.locationNameLabel = this.LocationInfoPanel.GetChild(0).GetComponent<UILabel>();
        this.characterTransition = this.TransitionGroup.GetChild(0).GetComponent<HonoTweenPosition>();
        this.submenuTransition = this.SubMenuTransitionGameObject.GetComponent<HonoTweenClipping>();
        this.screenFadePanel = this.ScreenFadeGameObject.GetParent().GetComponent<UIPanel>();
        this.characterMemorize = this.CharacterHUDList[this.GetFirstPlayer()].Self;
        this.characterOrderMemorize = this.CharacterHUDList[0].Self;

        UIWidget locationFrame = this.LocationInfoPanel.GetComponent<UIWidget>();
        locationFrame.leftAnchor.target = this.transform;
        locationFrame.leftAnchor.Set(0f, 30f);

        this.Background = new GOMenuBackground(this.transform.GetChild(4).gameObject, "main_menu_bg");
    }

    public GameObject SubMenuPanel;
    public GameObject TransitionGroup;
    public GameObject CharacterListPanel;
    public GameObject GenericInfoPanel;
    public GameObject LocationInfoPanel;
    public GameObject HelpDespLabelGameObject;
    public GameObject ScreenFadeGameObject;
    public GameObject SubMenuTransitionGameObject;

    public HashSet<String> EnabledSubMenus = new HashSet<String>();

    public Int32 ImpactfulActionCount;

    private static String SubMenuGroupButton = "MainMenu.SubMenu";
    private static String CharacterGroupButton = "MainMenu.Character";
    private static String OrderGroupButton = "MainMenu.Order";

    private GameObject ItemSubMenu;
    private GameObject AbilitySubMenu;
    private GameObject EquipSubMenu;
    private GameObject StatusSubMenu;
    private GameObject OrderSubMenu;
    private GameObject CardSubMenu;
    private GameObject ConfigSubMenu;
    [NonSerialized]
    private GameObject PartySubMenu;
    [NonSerialized]
    private GOMenuBackground Background;

    private UILabel gilLabel;
    private UILabel hourLabel;
    private UILabel minuteLabel;
    private UILabel secondLabel;
    private UILabel[] colonLabel;
    private UILabel locationNameLabel;

    private UIPanel screenFadePanel;
    private HonoTweenPosition characterTransition;
    private HonoTweenClipping submenuTransition;

    private Boolean isNeedCharacterTween = true;
    private Boolean isNeedHideSubMenu = true;

    private MainMenuUI.SubMenu currentMenu;
    private Int32 currentCharacterIndex = -1;
    private Int32 currentOrder = -1;

    private List<CharacterDetailHUD> CharacterHUDList = new List<CharacterDetailHUD>();
    private List<GameObject> CharacterOrderGameObjectList = new List<GameObject>();

    private GameObject characterMemorize;
    private GameObject characterOrderMemorize;

    public enum SubMenu
    {
        Item,
        Ability,
        Equip,
        Status,
        Order,
        Card,
        Config,
        Party,
        None
    }
}
