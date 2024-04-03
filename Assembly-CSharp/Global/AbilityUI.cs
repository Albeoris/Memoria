using System;
using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using System.Collections.Generic;
using System.Linq;
using Memoria;
using Memoria.Assets;
using Memoria.Scenes;
using Memoria.Data;
using Memoria.Database;
using Memoria.Field;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable EmptyConstructor
// ReSharper disable ArrangeThisQualifier
// ReSharper disable RedundantExplicitArraySize
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable SuspiciousTypeConversion.Global

public class AbilityUI : UIScene
{
    public const Int32 FF9FABIL_EVENT_NOMAGIC = 227;
    public GameObject TransitionGroup;
    public GameObject UseSubMenu;
    public GameObject EquipSubMenu;
    public GameObject HelpDespLabelGameObject;
    public GameObject SubMenuPanel;
    public GameObject ActiveAbilityListPanel;
    public GameObject SupportAbilityListPanel;
    public GameObject TargetListPanel;
    public GameObject CommandPanel;
    public GameObject MagicStonePanel;
    public GameObject CharacterArrowPanel;
    public GameObject CharacterDetailPanel;
    public GameObject AbilityDetailPanel;
    public GameObject ScreenFadeGameObject;
    private static Single TargetPositionXOffset;
    private static String SubMenuGroupButton;
    private static String ActionAbilityGroupButton;
    private static String SupportAbilityGroupButton;
    private static String TargetGroupButton;
    private UILabel useSubMenuLabel;
    private UILabel equipSubMenuLabel;
    private UIPanel targetListPanelComponent;
    private GameObject allTargetButton;
    private GameObject allTargetHitArea;
    private UIButton allTargetButtonComponent;
    private BoxCollider allTargetButtonCollider;
    private UILabel allTargetButtonLabel;
    private UILabel commandLabel;
    private CharacterDetailHUD characterHud;
    private AbilityInfoHUD abilityInfoHud;
    private List<CharacterDetailHUD> targetHudList;
    private GameObject submenuArrowGameObject;
    private HonoTweenPosition targetTransition;
    private HonoAvatarTweenPosition avatarTransition;
    private RecycleListPopulator activeAbilityScrollList;
    private RecycleListPopulator supportAbilityScrollList;
    private Boolean isAAEnable;
    private Boolean isSAEnable;
    private List<Int32> aaIdList;
    private List<Int32> saIdList;
    private Int32 currentPartyIndex;
    private SubMenu currentSubMenu;
    private Int32 firstActiveAbility;
    private Int32 currentAbilityIndex;
    private Boolean fastSwitch;
    private Boolean multiTarget;
    private Boolean canMultiTarget;
    private Dictionary<Int32, Int32> equipmentPartInAbilityDict;
    private Dictionary<Int32, RegularItem[]> equipmentIdInAbilityDict;
    private GOScrollablePanel _abilityPanel;
    private GOScrollablePanel _supportPanel;

    public static Color[] BoostedAbilityColor = new Color[]
    {
        new Color(1f, 1f, 1f),
        new Color(0.7f, 1f, 0.7f),
        new Color(1f, 0.2f, 0.5f),
        new Color(0.6f, 0.6f, 1f),
        new Color(1f, 1f, 0.4f),
        new Color(1f, 0.5f, 0.2f)
    };

    public Int32 CurrentPartyIndex
    {
        set => this.currentPartyIndex = value;
    }

    static AbilityUI()
    {
        TargetPositionXOffset = 338f;
        SubMenuGroupButton = "Ability.SubMenu";
        ActionAbilityGroupButton = "Ability.ActionAbility";
        SupportAbilityGroupButton = "Ability.SupportAbility";
        TargetGroupButton = "Ability.Target";
    }

    public AbilityUI()
    {
        this.targetHudList = new List<CharacterDetailHUD>();
        this.isAAEnable = true;
        this.isSAEnable = true;
        this.aaIdList = new List<Int32>();
        this.saIdList = new List<Int32>();
        this.equipmentPartInAbilityDict = new Dictionary<Int32, Int32>();
        this.equipmentIdInAbilityDict = new Dictionary<Int32, RegularItem[]>();
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate afterShowAction = () =>
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.SubMenuPanel.SetActive(false);
            ButtonGroupState.SetScrollButtonToGroup(this.activeAbilityScrollList.ScrollButton, ActionAbilityGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(this.supportAbilityScrollList.ScrollButton, SupportAbilityGroupButton);
            ButtonGroupState.SetPointerDepthToGroup(4, ActionAbilityGroupButton);
            ButtonGroupState.SetPointerDepthToGroup(7, TargetGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(40f, 0.0f), ActionAbilityGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(40f, 0.0f), SupportAbilityGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(this.ActiveAbilityListPanel.GetComponent<UIWidget>(), this.activeAbilityScrollList.cellHeight, ActionAbilityGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(this.SupportAbilityListPanel.GetComponent<UIWidget>(), this.supportAbilityScrollList.cellHeight, SupportAbilityGroupButton);
            ButtonGroupState.ActiveGroup = SubMenuGroupButton;
            afterFinished?.Invoke();
        };

        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(afterShowAction);
        this.UpdateUserInterface();
        this.SwitchCharacter(true);
        this.DisplayHelp();
        this.DisplaySubMenuArrow(true);
        this.DisplayAllButton();
        this.activeAbilityScrollList.ScrollButton.DisplayScrollButton(false, false);
        this.supportAbilityScrollList.ScrollButton.DisplayScrollButton(false, false);
    }

    public void UpdateUserInterface()
    {
        if (!Configuration.Interface.IsEnabled)
            return;
        const Int32 originalLineCount = 6;
        const Single buttonOriginalHeight = 92f;
        const Single panelOriginalWidth = 1488f;
        const Single panelOriginalHeight = originalLineCount * buttonOriginalHeight;
        Int32 linePerPage = Configuration.Interface.MenuAbilityRowCount;
        Int32 lineHeight = (Int32)Math.Round(panelOriginalHeight / linePerPage);
        Single scaleFactor = lineHeight / buttonOriginalHeight;
        _abilityPanel.SubPanel.ChangeDims(2, linePerPage, panelOriginalWidth / 2f, lineHeight);
        _abilityPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _abilityPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.152f, relTop: 0.848f, relLeft: 0.091f, relRight: 0.795f);
        _abilityPanel.SubPanel.ButtonPrefab.NumberLabel.SetAnchor(target: _abilityPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.152f, relTop: 0.848f, relLeft: 0.8f, relRight: 0.92f);
        _abilityPanel.SubPanel.ButtonPrefab.NameLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _abilityPanel.SubPanel.ButtonPrefab.NumberLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _abilityPanel.SubPanel.RecycleListPopulator.RefreshTableView();
        _supportPanel.SubPanel.ChangeDims(2, linePerPage, panelOriginalWidth / 2f, lineHeight);
        _supportPanel.SubPanel.ButtonPrefab.IconSprite.SetAnchor(target: _supportPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.152f, relTop: 0.848f, relLeft: 0.09f, relRight: 0.176f);
        _supportPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _supportPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.152f, relTop: 0.848f, relLeft: 0.24f, relRight: 0.795f);
        _supportPanel.SubPanel.ButtonPrefab.NumberLabel.SetAnchor(target: _supportPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.152f, relTop: 0.848f, relLeft: 0.8f, relRight: 0.92f);
        _supportPanel.SubPanel.ButtonPrefab.NameLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _supportPanel.SubPanel.ButtonPrefab.NumberLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _supportPanel.SubPanel.RecycleListPopulator.RefreshTableView();
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
        if (this.fastSwitch)
            return;
        PersistenSingleton<UIManager>.Instance.MainMenuScene.StartSubmenuTweenIn();
        this.RemoveCursorMemorize();
    }

    private void RemoveCursorMemorize()
    {
        this.currentSubMenu = SubMenu.Use;
        ButtonGroupState.RemoveCursorMemorize(SubMenuGroupButton);
        ButtonGroupState.RemoveCursorMemorize(ActionAbilityGroupButton);
        ButtonGroupState.RemoveCursorMemorize(SupportAbilityGroupButton);
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go))
        {
            if (ButtonGroupState.ActiveGroup == SubMenuGroupButton)
            {
                this.currentSubMenu = this.GetSubMenuFromGameObject(go);
                if (this.currentSubMenu == SubMenu.Use && this.isAAEnable)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    ButtonGroupState.ActiveGroup = ActionAbilityGroupButton;
                    this.SetAbilityInfo(true);
                    this.DisplayPlayerArrow(false);
                    this.DisplaySubMenuArrow(false);
                    ButtonGroupState.SetSecondaryOnGroup(SubMenuGroupButton);
                    ButtonGroupState.HoldActiveStateOnGroup(SubMenuGroupButton);
                }
                else if (this.currentSubMenu == SubMenu.Equip && this.isSAEnable)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    ButtonGroupState.ActiveGroup = SupportAbilityGroupButton;
                    this.SetAbilityInfo(true);
                    this.DisplayPlayerArrow(false);
                    this.DisplaySubMenuArrow(false);
                    ButtonGroupState.SetSecondaryOnGroup(SubMenuGroupButton);
                    ButtonGroupState.HoldActiveStateOnGroup(SubMenuGroupButton);
                }
                else
                {
                    FF9Sfx.FF9SFX_Play(102);
                }
            }
            else if (ButtonGroupState.ActiveGroup == ActionAbilityGroupButton)
            {
                if (ButtonGroupState.ContainButtonInGroup(go, ActionAbilityGroupButton))
                {
                    Character player = FF9StateSystem.Common.FF9.party.GetCharacter(this.currentPartyIndex);
                    this.currentAbilityIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                    Int32 abilId = this.aaIdList[this.currentAbilityIndex];
                    this.canMultiTarget = this.IsMulti(abilId);
                    if (abilId != 0 && ff9abil.IsAbilityActive(abilId))
                    {
                        if (this.CheckAAType(abilId, player) == AbilityType.Enable)
                        {
                            if (this.canMultiTarget)
                            {
                                this.allTargetButtonCollider.enabled = true;
                                this.allTargetButtonComponent.SetState(UIButtonColor.State.Normal, true);
                                this.allTargetButtonLabel.color = FF9TextTool.White;
                            }
                            else
                            {
                                this.allTargetButtonCollider.enabled = false;
                                this.allTargetButtonComponent.SetState(UIButtonColor.State.Normal, true);
                                this.allTargetButtonLabel.color = FF9TextTool.Gray;
                            }
                            FF9Sfx.FF9SFX_Play(103);
                            this.currentAbilityIndex = go.transform.GetSiblingIndex();
                            if (this.currentAbilityIndex % 2 == 0)
                            {
                                this.targetTransition.animatedInStartPosition = new Vector3(1543f, 0.0f, 0.0f);
                                this.targetTransition.animatedOutEndPosition = new Vector3(1543f, 0.0f, 0.0f);
                                this.TargetListPanel.transform.localPosition = new Vector3(TargetPositionXOffset, 0.0f, 0.0f);
                            }
                            else
                            {
                                this.targetTransition.animatedInStartPosition = new Vector3(-1543f, 0.0f, 0.0f);
                                this.targetTransition.animatedOutEndPosition = new Vector3(-1543f, 0.0f, 0.0f);
                                this.TargetListPanel.transform.localPosition = new Vector3((Single)(-TargetPositionXOffset - 60.0), 0.0f, 0.0f);
                            }
                            this.targetTransition.DestinationPosition = new Vector3[1]
                            {
                                this.TargetListPanel.transform.localPosition
                            };
                            this.DisplayTarget();
                            this.Loading = true;
                            this.targetTransition.TweenIn(new Byte[1], () =>
                            {
                                this.Loading = false;
                                ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
                                ButtonGroupState.ActiveGroup = TargetGroupButton;
                                ButtonGroupState.HoldActiveStateOnGroup(ActionAbilityGroupButton);
                            });
                        }
                        else
                        {
                            FF9Sfx.FF9SFX_Play(102);
                        }
                    }
                }
                else
                {
                    this.OnSecondaryGroupClick(go);
                }
            }
            else if (ButtonGroupState.ActiveGroup == SupportAbilityGroupButton)
            {
                if (ButtonGroupState.ContainButtonInGroup(go, SupportAbilityGroupButton))
                {
                    Character player = FF9StateSystem.Common.FF9.party.GetCharacter(this.currentPartyIndex);
                    this.currentAbilityIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                    if (go.GetChild(0).activeSelf)
                    {
                        Int32 abilityId = this.saIdList[this.currentAbilityIndex];
                        AbilityType abilityType = this.CheckSAType(abilityId, player);
                        CharacterAbilityGems saData = ff9abil.GetSupportAbilityGem(abilityId);
                        SupportAbility supportId = ff9abil.GetSupportAbilityFromAbilityId(abilityId);
                        if (abilityType == AbilityType.Enable)
                        {
                            PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
                            FF9Sfx.FF9SFX_Play(107);
                            ff9abil.FF9Abil_SetEnableSA(player.Data, supportId, true);
                            player.Data.cur.capa -= saData.GemsCount;
                            ff9play.FF9Play_Update(player.Data);
                            this.DisplaySA();
                            this.DisplayCharacter(true);
                        }
                        else if (abilityType == AbilityType.Selected)
                        {
                            PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
                            FF9Sfx.FF9SFX_Play(107);
                            Int32 boostMaxLevel = ff9abil.GetBoostedAbilityMaxLevel(player, supportId);
                            if (boostMaxLevel > 0)
                            {
                                Int32 boostLevel = Math.Min(boostMaxLevel, ff9abil.GetBoostedAbilityLevel(player, supportId));
                                List<SupportAbility> boostedList = ff9abil.GetBoostedAbilityList(supportId);
                                Boolean enableNext = boostLevel < boostMaxLevel;
                                if (enableNext)
								{
                                    CharacterAbilityGems nextBoost = ff9abil._FF9Abil_SaData[boostedList[boostLevel]];
                                    enableNext = this.CheckSAType(ff9abil.GetAbilityIdFromSupportAbility(nextBoost.Id), player) == AbilityType.Enable;
                                    if (enableNext)
                                    {
                                        ff9abil.FF9Abil_SetEnableSA(player.Data, nextBoost.Id, true);
                                        player.Data.cur.capa -= nextBoost.GemsCount;
                                    }
                                }
                                if (!enableNext)
                                {
                                    foreach (SupportAbility boosted in boostedList)
									{
                                        if (ff9abil.FF9Abil_IsEnableSA(player.Data.saExtended, boosted))
                                        {
                                            CharacterAbilityGems boostedGem = ff9abil._FF9Abil_SaData[boosted];
                                            ff9abil.FF9Abil_SetEnableSA(player.Data, boosted, false);
                                            player.Data.cur.capa += boostedGem.GemsCount;
                                        }
									}
                                    ff9abil.FF9Abil_SetEnableSA(player.Data, supportId, false);
                                    player.Data.cur.capa += saData.GemsCount;
                                }
                            }
                            else
                            {
                                ff9abil.FF9Abil_SetEnableSA(player.Data, supportId, false);
                                player.Data.cur.capa += saData.GemsCount;
                            }
                            ff9play.FF9Play_Update(player.Data);
                            this.DisplaySA();
                            this.DisplayCharacter(true);
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
                }
                else
                {
                    this.OnSecondaryGroupClick(go);
                }
            }
            else if (ButtonGroupState.ActiveGroup == TargetGroupButton && (ButtonGroupState.ContainButtonInGroup(go, TargetGroupButton) || go == this.allTargetHitArea))
            {
                Boolean canUseAbility = false;
                Int32 memberIndex = go.transform.GetSiblingIndex();
                PLAYER caster = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
                BattleAbilityId abilId = ff9abil.GetActiveAbilityFromAbilityId(this.aaIdList[this.currentAbilityIndex]);
                AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[abilId];
                if (!this.multiTarget)
                {
                    canUseAbility = SFieldCalculator.FieldCalcMain(caster, FF9StateSystem.Common.FF9.party.member[memberIndex], aaData, aaData.Ref.ScriptId, 0U);
                }
                else
                {
                    for (Int32 i = 0; i < 4; ++i)
                        if (FF9StateSystem.Common.FF9.party.member[i] != null)
                            canUseAbility |= SFieldCalculator.FieldCalcMain(caster, FF9StateSystem.Common.FF9.party.member[i], aaData, aaData.Ref.ScriptId, 1U);
                }
                if (canUseAbility)
                {
                    PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
                    FF9Sfx.FF9SFX_Play(106);
                    Int32 mpCost = GetMp(aaData);
                    if (!FF9StateSystem.Settings.IsHpMpFull)
                        caster.cur.mp = (UInt32)(caster.cur.mp - mpCost);
                    if (caster.cur.mp < mpCost)
                    {
                        this.DisplayAA();
                        this.TargetListPanel.SetActive(false);
                        ButtonGroupState.ActiveGroup = ActionAbilityGroupButton;
                    }
                    FF9StateSystem.EventState.IncreaseAAUsageCounter(abilId);
                    BattleAchievement.IncreaseNumber(ref FF9StateSystem.Achievement.whtMag_no, 1);
                    AchievementManager.ReportAchievement(AcheivementKey.WhtMag200, FF9StateSystem.Achievement.whtMag_no);
                    this.DisplayTarget();
                    this.DisplayCharacter(true);
                }
                else
                {
                    FF9Sfx.FF9SFX_Play(102);
                }
            }
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go))
        {
            if (ButtonGroupState.ActiveGroup == SubMenuGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.fastSwitch = false;
                this.Hide(() =>
                {
                    PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = false;
                    PersistenSingleton<UIManager>.Instance.MainMenuScene.CurrentSubMenu = MainMenuUI.SubMenu.Ability;
                    PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.MainMenu);
                });
            }
            else if (ButtonGroupState.ActiveGroup == ActionAbilityGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.SetAbilityInfo(false);
                this.DisplayPlayerArrow(true);
                this.DisplaySubMenuArrow(true);
                ButtonGroupState.ActiveGroup = SubMenuGroupButton;
            }
            else if (ButtonGroupState.ActiveGroup == SupportAbilityGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.SetAbilityInfo(false);
                this.DisplayPlayerArrow(true);
                this.DisplaySubMenuArrow(true);
                ButtonGroupState.ActiveGroup = SubMenuGroupButton;
            }
            else if (ButtonGroupState.ActiveGroup == TargetGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                this.SetMultipleTarget(false);
                this.Loading = true;
                // ISSUE: method pointer
                this.targetTransition.TweenOut(new Byte[1], () =>
                {
                    this.Loading = false;
                    ButtonGroupState.ActiveGroup = ActionAbilityGroupButton;
                });
            }
        }
        return true;
    }

    public override Boolean OnKeySpecial(GameObject go)
    {
        if (base.OnKeySpecial(go) && ButtonGroupState.ActiveGroup == SubMenuGroupButton)
        {
            if (PersistenSingleton<UIManager>.Instance.MainMenuScene.IsSubMenuEnabled(MainMenuUI.SubMenu.Equip))
            {
                FF9Sfx.FF9SFX_Play(103);
                this.fastSwitch = true;
                this.Hide(() =>
                {
                    this.RemoveCursorMemorize();
                    PersistenSingleton<UIManager>.Instance.EquipScene.CurrentPartyIndex = this.currentPartyIndex;
                    PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Equip);
                });
            }
            else
            {
                FF9Sfx.FF9SFX_Play(102);
            }
        }
        else if (ButtonGroupState.ActiveGroup == SupportAbilityGroupButton)
        {
            if (ButtonGroupState.ContainButtonInGroup(go, SupportAbilityGroupButton))
            {
                Character player = FF9StateSystem.Common.FF9.party.GetCharacter(this.currentPartyIndex);
                this.currentAbilityIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                if (go.GetChild(0).activeSelf)
                {
                    Int32 abilityId = this.saIdList[this.currentAbilityIndex];
                    AbilityType abilityType = this.CheckSAType(abilityId, player);
                    CharacterAbilityGems saData = ff9abil.GetSupportAbilityGem(abilityId);
                    SupportAbility supportId = ff9abil.GetSupportAbilityFromAbilityId(abilityId);
                    if (abilityType == AbilityType.Enable)
                    {
                        PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
                        FF9Sfx.FF9SFX_Play(107);
                        ff9abil.FF9Abil_SetEnableSA(player.Data, supportId, true);
                        player.Data.cur.capa -= saData.GemsCount;
                        Int32 boostMaxLevel = ff9abil.GetBoostedAbilityMaxLevel(player, supportId);
                        if (boostMaxLevel > 0)
                        {
                            List<SupportAbility> boostedList = ff9abil.GetBoostedAbilityList(supportId);
                            foreach (SupportAbility boosted in boostedList)
                            {
                                if (this.CheckSAType(ff9abil.GetAbilityIdFromSupportAbility(boosted), player) == AbilityType.Enable)
                                {
                                    CharacterAbilityGems boostedGem = ff9abil._FF9Abil_SaData[boosted];
                                    ff9abil.FF9Abil_SetEnableSA(player.Data, boosted, true);
                                    player.Data.cur.capa -= boostedGem.GemsCount;
                                }
                                else
								{
                                    break;
								}
                            }
                        }
                        ff9play.FF9Play_Update(player.Data);
                        this.DisplaySA();
                        this.DisplayCharacter(true);
                    }
                    else if (abilityType == AbilityType.Selected)
                    {
                        PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
                        FF9Sfx.FF9SFX_Play(107);
                        Int32 boostMaxLevel = ff9abil.GetBoostedAbilityMaxLevel(player, supportId);
                        if (boostMaxLevel > 0)
                        {
                            Int32 boostLevel = Math.Min(boostMaxLevel, ff9abil.GetBoostedAbilityLevel(player, supportId));
                            List<SupportAbility> boostedList = ff9abil.GetBoostedAbilityList(supportId);
                            if (boostLevel > 0)
                            {
                                supportId = boostedList[boostLevel - 1];
                                saData = ff9abil._FF9Abil_SaData[supportId];
                            }
                        }
                        ff9abil.FF9Abil_SetEnableSA(player.Data, supportId, false);
                        player.Data.cur.capa += saData.GemsCount;
                        ff9play.FF9Play_Update(player.Data);
                        this.DisplaySA();
                        this.DisplayCharacter(true);
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
            }
        }
        return true;
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        if (base.OnKeyLeftBumper(go))
        {
            if (ButtonGroupState.ActiveGroup == SubMenuGroupButton)
            {
                if (this.CharacterArrowPanel.activeSelf)
                {
                    FF9Sfx.FF9SFX_Play(1047);
                    Int32 prev = ff9play.FF9Play_GetPrev(this.currentPartyIndex);
                    if (prev != this.currentPartyIndex)
                    {
                        this.currentPartyIndex = prev;
                        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
                        String spritName = FF9UIDataTool.AvatarSpriteName(player.info.serial_no);
                        ButtonGroupState.RemoveCursorMemorize(ActionAbilityGroupButton);
                        ButtonGroupState.RemoveCursorMemorize(SupportAbilityGroupButton);
                        this.ShowPointerWhenLoading = true;
                        this.Loading = true;
                        Boolean isKnockOut = player.cur.hp == 0;

                        this.avatarTransition.Change(spritName, HonoAvatarTweenPosition.Direction.LeftToRight, isKnockOut, () =>
                        {
                            this.DisplayHelp();
                            this.DisplayCharacter(true);
                            this.Loading = false;
                            this.ShowPointerWhenLoading = false;
                        });
                        this.SwitchCharacter(false);
                    }
                }
            }
            else if (ButtonGroupState.ActiveGroup == TargetGroupButton)
            {
                FF9Sfx.FF9SFX_Play(103);
                this.ToggleMultipleTarget();
            }
        }
        return true;
    }

    public override Boolean OnKeyRightBumper(GameObject go)
    {
        if (base.OnKeyRightBumper(go))
        {
            if (ButtonGroupState.ActiveGroup == SubMenuGroupButton)
            {
                if (this.CharacterArrowPanel.activeSelf)
                {
                    FF9Sfx.FF9SFX_Play(1047);
                    Int32 next = ff9play.FF9Play_GetNext(this.currentPartyIndex);
                    if (next != this.currentPartyIndex)
                    {
                        this.currentPartyIndex = next;
                        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
                        String spritName = FF9UIDataTool.AvatarSpriteName(player.info.serial_no);
                        ButtonGroupState.RemoveCursorMemorize(ActionAbilityGroupButton);
                        ButtonGroupState.RemoveCursorMemorize(SupportAbilityGroupButton);
                        this.ShowPointerWhenLoading = true;
                        this.Loading = true;
                        Boolean isKnockOut = player.cur.hp == 0;
                        this.avatarTransition.Change(spritName, HonoAvatarTweenPosition.Direction.RightToLeft, isKnockOut, () =>
                        {
                            this.DisplayHelp();
                            this.DisplayCharacter(true);
                            this.Loading = false;
                            this.ShowPointerWhenLoading = false;
                        });
                        this.SwitchCharacter(false);
                    }
                }
            }
            else if (ButtonGroupState.ActiveGroup == TargetGroupButton)
            {
                FF9Sfx.FF9SFX_Play(103);
                this.ToggleMultipleTarget();
            }
        }
        return true;
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (base.OnItemSelect(go))
        {
            if (ButtonGroupState.ActiveGroup == SubMenuGroupButton)
            {
                if (this.currentSubMenu != this.GetSubMenuFromGameObject(go))
                {
                    this.currentSubMenu = this.GetSubMenuFromGameObject(go);
                    switch (this.currentSubMenu)
                    {
                        case SubMenu.Use:
                            this.CommandPanel.SetActive(true);
                            this.ActiveAbilityListPanel.SetActive(true);
                            this.MagicStonePanel.SetActive(false);
                            this.SupportAbilityListPanel.SetActive(false);
                            this.DisplayAA();
                            break;
                        case SubMenu.Equip:
                            this.CommandPanel.SetActive(false);
                            this.ActiveAbilityListPanel.SetActive(false);
                            this.MagicStonePanel.SetActive(true);
                            this.SupportAbilityListPanel.SetActive(true);
                            this.DisplaySA();
                            break;
                    }
                }
            }
            else if (ButtonGroupState.ActiveGroup == ActionAbilityGroupButton)
            {
                if (this.currentAbilityIndex != go.GetComponent<RecycleListItem>().ItemDataIndex)
                {
                    this.currentAbilityIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                    this.SetAbilityInfo(true);
                }
            }
            else if (ButtonGroupState.ActiveGroup == SupportAbilityGroupButton && this.currentAbilityIndex != go.GetComponent<RecycleListItem>().ItemDataIndex)
            {
                this.currentAbilityIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                this.SetAbilityInfo(true);
            }
        }
        return true;
    }

    private void OnAllTargetClick(GameObject go)
    {
        FF9Sfx.FF9SFX_Play(103);
        this.ToggleMultipleTarget();
    }

    private void OnSecondaryGroupClick(GameObject go)
    {
        ButtonGroupState.HoldActiveStateOnGroup(go, SubMenuGroupButton);
        if (ButtonGroupState.ActiveGroup == ActionAbilityGroupButton)
        {
            FF9Sfx.muteSfx = true;
            this.OnKeyCancel(this.activeAbilityScrollList.GetItem(this.currentAbilityIndex).gameObject);
            FF9Sfx.muteSfx = false;
            this.OnKeyConfirm(go);
        }
        else if (ButtonGroupState.ActiveGroup == SupportAbilityGroupButton)
        {
            FF9Sfx.muteSfx = true;
            this.OnKeyCancel(this.supportAbilityScrollList.GetItem(this.currentAbilityIndex).gameObject);
            FF9Sfx.muteSfx = false;
            this.OnKeyConfirm(go);
        }
    }

    private void DisplayHelp()
    {
        Character player = FF9StateSystem.Common.FF9.party.GetCharacter(this.currentPartyIndex);
        ButtonGroupState useSubMenu = this.UseSubMenu.GetComponent<ButtonGroupState>();
        ButtonGroupState equipSubMenu = this.EquipSubMenu.GetComponent<ButtonGroupState>();
        String mobileSuffix = FF9StateSystem.MobilePlatform ? "Mobile" : "";
        String localizationPrefix;
        String help = Localization.Get("UseAbilityHelp");
        if (FF9StateSystem.EventState.gEventGlobal[FF9FABIL_EVENT_NOMAGIC] != 0 || IsSubMenuDisabledByMainMenu(true))
            localizationPrefix = "UseAbilityNoMagic";
        else if (this.aaIdList.Count != 0)
            localizationPrefix = "UseAbilityHelpStatus";
        else
            localizationPrefix = "UseAbilityHelpForever";
        help += Localization.Get(localizationPrefix + mobileSuffix);
        useSubMenu.Help.Text = help;
        help = Localization.Get("EquipAbilityHelp");
        if (!ff9abil.FF9Abil_HasSA(player))
            localizationPrefix = player.IsSubCharacter ? "EquipAbilityHelpNow" : "EquipAbilityForever";
        else
            localizationPrefix = IsSubMenuDisabledByMainMenu(false) ? "EquipAbilityHelpNow" : "EquipAbilityPlayer";
        help += Localization.Get(localizationPrefix + mobileSuffix);
        equipSubMenu.Help.Text = help;
        this.HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);
    }

    private void DisplaySubMenuArrow(Boolean isEnable)
    {
        if (isEnable)
            this.submenuArrowGameObject.SetActive(!FF9StateSystem.PCPlatform);
        else
            this.submenuArrowGameObject.SetActive(false);
    }

    private void DisplayPlayerArrow(Boolean isEnable)
    {
        if (isEnable)
        {
            Int32 num = 0;
            foreach (PLAYER player in FF9StateSystem.Common.FF9.party.member)
            {
                if (player != null)
                    ++num;
            }
            this.CharacterArrowPanel.SetActive(num > 1);
        }
        else
            this.CharacterArrowPanel.SetActive(false);
    }

    private void DisplayCharacter(Boolean updateAvatar)
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        FF9UIDataTool.DisplayCharacterDetail(player, this.characterHud);
        if (!updateAvatar)
            return;
        FF9UIDataTool.DisplayCharacterAvatar(player, new Vector3(), new Vector3(), this.characterHud.AvatarSprite, false);
    }

    private void DisplayAllButton()
    {
        if (FF9StateSystem.PCPlatform)
        {
            this.targetListPanelComponent.baseClipRegion = new Vector4(this.targetListPanelComponent.baseClipRegion.x, this.targetListPanelComponent.baseClipRegion.y, this.targetListPanelComponent.baseClipRegion.z, 778f);
            this.targetTransition.DestinationPosition[0] = new Vector3(347f, 30f, 0.0f);
            this.allTargetButton.SetActive(false);
            this.allTargetHitArea.SetActive(false);
        }
        else
        {
            this.targetListPanelComponent.baseClipRegion = new Vector4(this.targetListPanelComponent.baseClipRegion.x, this.targetListPanelComponent.baseClipRegion.y, this.targetListPanelComponent.baseClipRegion.z, 868f);
            this.targetTransition.DestinationPosition[0] = new Vector3(347f, 0.0f, 0.0f);
            this.allTargetButton.SetActive(true);
            this.allTargetHitArea.SetActive(false);
        }
    }

    private void SetAbilityInfo(Boolean isVisible)
    {
        if (isVisible)
        {
            Character player = FF9StateSystem.Common.FF9.party.GetCharacter(this.currentPartyIndex);
            Int32 abilId;
            AbilityType abilityType;
            Boolean isShowText;
            if (this.currentSubMenu == SubMenu.Use)
            {
                abilId = this.aaIdList[this.currentAbilityIndex];
                abilityType = this.CheckAAType(abilId, player);
                isShowText = (ff9abil.GetActionAbility(abilId).Type & 2) == 0;
            }
            else
            {
                abilId = this.saIdList[this.currentAbilityIndex];
                abilityType = this.CheckSAType(abilId, player);
                isShowText = true;
            }
            this.abilityInfoHud.ClearEquipmentIcon();
            if (abilityType == AbilityType.NoDraw)
            {
                this.abilityInfoHud.APBar.Slider.gameObject.SetActive(false);
            }
            else
            {
                this.abilityInfoHud.APBar.Slider.gameObject.SetActive(true);
                if (ff9abil.FF9Abil_HasAp(player))
                    FF9UIDataTool.DisplayAPBar(player.Data, abilId, isShowText, this.abilityInfoHud.APBar);
                Int32 spriteSlot = 0;
                if (this.equipmentPartInAbilityDict.ContainsKey(abilId))
                {
                    for (Int32 i = 0; i < 5; ++i)
                    {
                        if ((this.equipmentPartInAbilityDict[abilId] & (1 << i)) != 0)
                        {
                            this.abilityInfoHud.EquipmentSpriteList[spriteSlot].alpha = 1f;
                            FF9UIDataTool.DisplayItem(this.equipmentIdInAbilityDict[abilId][i], this.abilityInfoHud.EquipmentSpriteList[spriteSlot], null, true);
                            ++spriteSlot;
                        }
                    }
                    for (Int32 i = 0; i < this.abilityInfoHud.EquipmentSpriteList.Length; ++i)
                    {
                        if (i >= spriteSlot)
                            this.abilityInfoHud.EquipmentSpriteList[i].alpha = 0.0f;
                    }
                }
            }
            this.DisplayCommandName();
        }
        else
        {
            this.abilityInfoHud.ClearEquipmentIcon();
            this.abilityInfoHud.APBar.Slider.gameObject.SetActive(false);
            this.commandLabel.text = String.Empty;
        }
    }

    private void DisplayCommandName()
    {
        if (this.currentSubMenu != SubMenu.Use)
            return;
        Character player = FF9StateSystem.Common.FF9.party.GetCharacter(this.currentPartyIndex);
        Int32 abilityId = this.aaIdList[this.currentAbilityIndex];
        this.commandLabel.text = this.CheckAAType(abilityId, player) == AbilityType.NoDraw
            ? String.Empty
            : FF9TextTool.CommandName(GetCommand(abilityId, player));
    }

    private void DisplayAA()
    {
        this.firstActiveAbility = -1;
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        Character player = FF9StateSystem.Common.FF9.party.GetCharacter(this.currentPartyIndex);
        using (List<Int32>.Enumerator enumerator = this.aaIdList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                AbilityListData abilityListData = new AbilityListData
                {
                    Id = enumerator.Current,
                    Type = this.CheckAAType(enumerator.Current, player)
                };
                inDataList.Add(abilityListData);
                if (this.firstActiveAbility == -1 && abilityListData.Type == AbilityType.Enable)
                    this.firstActiveAbility = inDataList.Count - 1;
            }
        }
        if (this.firstActiveAbility == -1)
            this.firstActiveAbility = 0;
        if (this.activeAbilityScrollList.ItemsPool.Count == 0)
        {
            this.activeAbilityScrollList.PopulateListItemWithData = DisplayAADetail;
            this.activeAbilityScrollList.OnRecycleListItemClick += OnListItemClick;
            this.activeAbilityScrollList.InitTableView(inDataList, this.firstActiveAbility);
        }
        else
        {
            this.activeAbilityScrollList.SetOriginalData(inDataList);
            if (ButtonGroupState.HaveCursorMemorize(ActionAbilityGroupButton))
                return;
            this.activeAbilityScrollList.JumpToIndex(this.firstActiveAbility, false);
        }
    }

    private void DisplayAADetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        AbilityListData abilityListData = (AbilityListData)data;
        ItemListDetailHUD itemListDetailHud = new ItemListDetailHUD(item.gameObject);
        if (isInit)
            this.DisplayWindowBackground(item.gameObject, null);
        if (abilityListData.Type == AbilityType.NoDraw)
        {
            itemListDetailHud.Content.SetActive(false);
            ButtonGroupState.SetButtonAnimation(itemListDetailHud.Self, false);
            itemListDetailHud.Button.Help.Enable = false;
        }
        else
        {
            BattleAbilityId battleAbilId = ff9abil.GetActiveAbilityFromAbilityId(abilityListData.Id);
            itemListDetailHud.Content.SetActive(true);
            ButtonGroupState.SetButtonAnimation(itemListDetailHud.Self, abilityListData.Type == AbilityType.Enable);
            BattleAbilityId patchedId = this.PatchAbility(battleAbilId);
            Int32 mp = GetMp(FF9StateSystem.Battle.FF9Battle.aa_data[patchedId]);
            itemListDetailHud.NameLabel.text = FF9TextTool.ActionAbilityName(patchedId);
            itemListDetailHud.NumberLabel.text = mp != 0 ? mp.ToString() : String.Empty;
            if (abilityListData.Type == AbilityType.CantSpell)
            {
                itemListDetailHud.NameLabel.color = FF9TextTool.Gray;
                itemListDetailHud.NumberLabel.color = FF9TextTool.Gray;
            }
            else if (abilityListData.Type == AbilityType.Enable)
            {
                itemListDetailHud.NameLabel.color = FF9TextTool.White;
                itemListDetailHud.NumberLabel.color = FF9TextTool.White;
            }
            itemListDetailHud.Button.Help.Enable = true;
            itemListDetailHud.Button.Help.Text = FF9TextTool.ActionAbilityHelpDescription(patchedId);
        }
    }

    private BattleAbilityId PatchAbility(BattleAbilityId id)
    {
        return BattleAbilityHelper.Patch(id, FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex]);
    }

    private void DisplaySA()
    {
        this.firstActiveAbility = -1;
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        Character player = FF9StateSystem.Common.FF9.party.GetCharacter(this.currentPartyIndex);
        using (List<Int32>.Enumerator enumerator = this.saIdList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                AbilityListData abilityListData = new AbilityListData
                {
                    Id = enumerator.Current,
                    Type = this.CheckSAType(enumerator.Current, player)
                };
                inDataList.Add(abilityListData);
                if (this.firstActiveAbility == -1 && (abilityListData.Type == AbilityType.Enable || abilityListData.Type == AbilityType.Selected || abilityListData.Type == AbilityType.CantDisable))
                    this.firstActiveAbility = inDataList.Count - 1;
            }
        }
        if (this.firstActiveAbility == -1)
            this.firstActiveAbility = 0;
        if (this.supportAbilityScrollList.ItemsPool.Count == 0)
        {
            this.supportAbilityScrollList.PopulateListItemWithData = DisplaySADetail;
            this.supportAbilityScrollList.OnRecycleListItemClick += OnListItemClick;
            this.supportAbilityScrollList.InitTableView(inDataList, this.firstActiveAbility);
        }
        else
        {
            this.supportAbilityScrollList.SetOriginalData(inDataList);
            if (ButtonGroupState.HaveCursorMemorize(SupportAbilityGroupButton))
                return;
            this.supportAbilityScrollList.JumpToIndex(this.firstActiveAbility, false);
        }
    }

    private void DisplaySADetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        AbilityListData abilityListData = (AbilityListData)data;
        ItemListDetailWithIconHUD detailWithIconHud = new ItemListDetailWithIconHUD(item.gameObject, true);
        if (isInit)
            this.DisplayWindowBackground(item.gameObject, null);
        CharacterAbilityGems saData = ff9abil.GetSupportAbilityGem(abilityListData.Id);
        if (abilityListData.Type == AbilityType.NoDraw)
        {
            detailWithIconHud.Content.SetActive(false);
            ButtonGroupState.SetButtonAnimation(detailWithIconHud.Self, false);
            detailWithIconHud.Button.Help.Enable = false;
        }
        else
        {
            SupportAbility supportId = ff9abil.GetSupportAbilityFromAbilityId(abilityListData.Id);
            detailWithIconHud.Content.SetActive(true);
            ButtonGroupState.SetButtonAnimation(detailWithIconHud.Self, true);
            detailWithIconHud.NameLabel.text = FF9TextTool.SupportAbilityName(supportId);
            detailWithIconHud.NumberLabel.text = saData.GemsCount.ToString();
            detailWithIconHud.IconSprite.preventPixelPerfect = Configuration.Interface.IsEnabled;
            detailWithIconHud.IconSprite.color = Color.white;
            if (abilityListData.Type == AbilityType.CantSpell)
            {
                detailWithIconHud.NameLabel.color = FF9TextTool.Gray;
                detailWithIconHud.NumberLabel.color = FF9TextTool.Gray;
                detailWithIconHud.IconSprite.atlas = FF9UIDataTool.WindowAtlas;
                detailWithIconHud.IconSprite.spriteName = "skill_stone_gem_null";
                detailWithIconHud.IconSpriteAnimation.Pause();
            }
            else if (abilityListData.Type == AbilityType.Enable)
            {
                detailWithIconHud.NameLabel.color = FF9TextTool.White;
                detailWithIconHud.NumberLabel.color = FF9TextTool.White;
                detailWithIconHud.IconSprite.atlas = FF9UIDataTool.WindowAtlas;
                detailWithIconHud.IconSprite.spriteName = "skill_stone_gem_slot";
                detailWithIconHud.IconSpriteAnimation.Pause();
            }
            else if (abilityListData.Type == AbilityType.Selected || abilityListData.Type == AbilityType.CantDisable)
            {
                Color labelColor = abilityListData.Type == AbilityType.Selected ? FF9TextTool.White : FF9TextTool.Gray;
                Character player = FF9StateSystem.Common.FF9.party.GetCharacter(this.currentPartyIndex);
                detailWithIconHud.NameLabel.color = labelColor;
                detailWithIconHud.NumberLabel.color = labelColor;
                detailWithIconHud.IconSprite.color = BoostedAbilityColor[0];
                detailWithIconHud.IconSprite.atlas = FF9UIDataTool.IconAtlas;
                detailWithIconHud.IconSpriteAnimation.namePrefix = "skill_stone_gem_";
                detailWithIconHud.IconSpriteAnimation.ResetToBeginning();
                Int32 maxLevel = ff9abil.GetBoostedAbilityMaxLevel(player, supportId);
                if (maxLevel > 0)
                {
                    List<SupportAbility> boostedList = ff9abil.GetBoostedAbilityList(supportId);
                    Int32 level = Math.Min(maxLevel, ff9abil.GetBoostedAbilityLevel(player, supportId));
                    Int32 stoneCost = saData.GemsCount;
                    for (Int32 i = 0; i < level; i++)
                        stoneCost += ff9abil._FF9Abil_SaData[boostedList[i]].GemsCount;
                    if (level > 0)
                        supportId = boostedList[level - 1];
                    detailWithIconHud.NameLabel.text = FF9TextTool.SupportAbilityName(supportId);
                    detailWithIconHud.NumberLabel.text = stoneCost.ToString();
                    detailWithIconHud.IconSprite.color = BoostedAbilityColor[Math.Min(level, BoostedAbilityColor.Length - 1)];
                }
            }
            detailWithIconHud.Button.Help.Enable = true;
            detailWithIconHud.Button.Help.Text = FF9TextTool.SupportAbilityHelpDescription(supportId);
            ButtonGroupState.RefreshHelpDialog();
        }
    }

    private void DisplayTarget()
    {
        Int32 num = 0;
        foreach (PLAYER player in FF9StateSystem.Common.FF9.party.member)
        {
            CharacterDetailHUD charHud = this.targetHudList[num++];
            charHud.Self.SetActive(true);
            if (player != null)
            {
                charHud.Content.SetActive(true);
                FF9UIDataTool.DisplayCharacterDetail(player, charHud);
                FF9UIDataTool.DisplayCharacterAvatar(player, new Vector2(), new Vector2(), charHud.AvatarSprite, false);
                switch (ff9abil.GetActionAbility(this.aaIdList[this.currentAbilityIndex]).Info.DisplayStats)
                {
                    case TargetDisplay.None:
                    case TargetDisplay.Hp:
                    case TargetDisplay.Mp:
                        charHud.HPPanel.SetActive(true);
                        charHud.MPPanel.SetActive(true);
                        charHud.StatusesPanel.SetActive(false);
                        continue;
                    case TargetDisplay.Buffs:
                    case TargetDisplay.Debuffs:
                        charHud.HPPanel.SetActive(false);
                        charHud.MPPanel.SetActive(false);
                        charHud.StatusesPanel.SetActive(true);
                        continue;
                    default:
                        continue;
                }
            }
            else
                charHud.Content.SetActive(false);
        }
        this.SetAvailableCharacter();
    }

    private Boolean IsMulti(Int32 abil_id)
    {
        BattleAbilityId battleAbilityId = ff9abil.GetActiveAbilityFromAbilityId(abil_id);
        switch (FF9BattleDB.CharacterActions[battleAbilityId].Info.Target)
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

    private AbilityType CheckAAType(Int32 abilityId, Character player)
    {
        BattleAbilityId battleAbilId = ff9abil.GetActiveAbilityFromAbilityId(abilityId);
        AA_DATA aa_data = FF9BattleDB.CharacterActions[battleAbilId];

        if (!this.equipmentPartInAbilityDict.ContainsKey(abilityId))
        {
            Int32 index = ff9abil.FF9Abil_GetIndex(player.Data, abilityId);
            if (index < 0)
                return AbilityType.NoDraw;

            if (ff9abil.FF9Abil_HasAp(player))
            {
                if ((Configuration.Battle.LockEquippedAbilities == 2 || Configuration.Battle.LockEquippedAbilities == 3) && player.Index != CharacterId.Quina)
                    return AbilityType.NoDraw;
                Int32 currentAp = player.Data.pa[index];
                Int32 learnAp = ff9abil._FF9Abil_PaData[player.PresetId][index].Ap;
                if (currentAp < learnAp)
                    return AbilityType.NoDraw;
            }
        }

        return (player.Data.status & BattleStatusConst.CannotUseAbilityInMenu) != 0 || (aa_data.Type & 1) == 0 || GetMp(aa_data) > player.Data.cur.mp ? AbilityType.CantSpell : AbilityType.Enable;
    }

    private AbilityType CheckSAType(Int32 abilityId, Character player)
    {
        if (!ff9abil.IsAbilitySupport(abilityId))
            return AbilityType.NoDraw;

        if (Configuration.Battle.LockEquippedAbilities == 1 || Configuration.Battle.LockEquippedAbilities == 3)
        {
            if (ff9abil.FF9Abil_GetIndex(player.Data, abilityId) < 0)
                return AbilityType.NoDraw;
            return this.equipmentPartInAbilityDict.ContainsKey(abilityId) ? AbilityType.CantDisable : AbilityType.CantSpell;
        }

        if (ff9abil.FF9Abil_IsEnableSA(player.Data.saExtended, ff9abil.GetSupportAbilityFromAbilityId(abilityId)))
            return AbilityType.Selected;

        if (!this.equipmentPartInAbilityDict.ContainsKey(abilityId))
        {
            Int32 index = ff9abil.FF9Abil_GetIndex(player.Data, abilityId);
            if (index < 0)
                return AbilityType.NoDraw;

            if (ff9abil.FF9Abil_HasAp(player))
            {
                Int32 currentAp = player.Data.pa[index];
                Int32 learnAp = ff9abil._FF9Abil_PaData[player.PresetId][index].Ap;
                if (currentAp < learnAp)
                    return AbilityType.NoDraw;
            }
        }

        return ff9abil.GetSupportAbilityGem(abilityId).GemsCount > player.Data.cur.capa ? AbilityType.CantSpell : AbilityType.Enable;
    }

    private static Int32 GetMp(AA_DATA aa_data)
    {
        Int32 mpCost = aa_data.MP;
        if ((aa_data.Type & 4) != 0 && FF9StateSystem.EventState.gEventGlobal[18] != 0)
            mpCost <<= 2;
        return mpCost;
    }

    private static BattleCommandId GetCommand(Int32 abil_id, Character play)
    {
        BattleAbilityId battleAbilId = ff9abil.GetActiveAbilityFromAbilityId(abil_id);
        for (Int32 commandNumber = 0; commandNumber < 2; ++commandNumber)
        {
            BattleCommandId cmdId = CharacterCommands.CommandSets[play.PresetId].GetRegular(commandNumber);
            CharacterCommand ff9Command = CharacterCommands.Commands[cmdId];
            foreach (BattleAbilityId abilId in ff9Command.EnumerateAbilities())
                if (BattleAbilityHelper.Patch(abilId, play.Data) == battleAbilId)
                    return cmdId;
        }
        return BattleCommandId.None;
    }

    private void SwitchCharacter(Boolean updateAvatar)
    {
        this.InitialData();
        ButtonGroupState.RemoveCursorMemorize(ActionAbilityGroupButton);
        ButtonGroupState.RemoveCursorMemorize(SupportAbilityGroupButton);
        this.DisplayPlayerArrow(true);
        this.DisplayCharacter(updateAvatar);
        if (this.currentSubMenu == SubMenu.Use)
        {
            this.ActiveAbilityListPanel.SetActive(true);
            this.SupportAbilityListPanel.SetActive(false);
            this.DisplayAA();
        }
        else
        {
            if (this.currentSubMenu != SubMenu.Equip)
                return;
            this.ActiveAbilityListPanel.SetActive(false);
            this.SupportAbilityListPanel.SetActive(true);
            this.DisplaySA();
        }
    }

    private void SetAvailableCharacter()
    {
        List<CharacterDetailHUD> targetList = new List<CharacterDetailHUD>();
        using (List<CharacterDetailHUD>.Enumerator enumerator = this.targetHudList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                CharacterDetailHUD curCharacter = enumerator.Current;
                if (!this.multiTarget)
                {
                    if (curCharacter.Content.activeSelf)
                    {
                        targetList.Add(curCharacter);
                        ButtonGroupState.SetButtonEnable(curCharacter.Self, true);
                    }
                    else
                    {
                        ButtonGroupState.SetButtonEnable(curCharacter.Self, false);
                    }
                }
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

    private void SetMultipleTarget(Boolean isActive)
    {
        ButtonGroupState.SetAllTarget(isActive);
        this.allTargetHitArea.SetActive(isActive);
        this.multiTarget = isActive;
        using (List<CharacterDetailHUD>.Enumerator enumerator = this.targetHudList.GetEnumerator())
        {
            while (enumerator.MoveNext())
                enumerator.Current.Self.GetComponent<ButtonGroupState>().Help.Enable = !isActive;
        }
        ButtonGroupState.UpdateActiveButton();
    }

    private void ToggleMultipleTarget()
    {
        if (!this.canMultiTarget)
            return;

        this.SetMultipleTarget(!this.multiTarget);
    }

    private void InitialData()
    {
        List<SupportAbility> boostedList = new List<SupportAbility>();
        this.aaIdList.Clear();
        this.saIdList.Clear();
        this.equipmentPartInAbilityDict.Clear();
        this.equipmentIdInAbilityDict.Clear();
        Character player = FF9StateSystem.Common.FF9.party.GetCharacter(this.currentPartyIndex);
        if (ff9abil._FF9Abil_PaData.ContainsKey(player.PresetId))
        {
            CharacterAbility[] charAbil = ff9abil._FF9Abil_PaData[player.PresetId];
            foreach (CharacterAbility paData in charAbil)
            {
                if (paData.Id != 0)
                {
                    if (paData.IsPassive)
                    {
                        this.saIdList.Add(paData.Id);
                        foreach (SupportAbility boosted in ff9abil.GetBoostedAbilityList(paData.PassiveId))
                        {
                            if (charAbil.Any(abil => abil.IsPassive && abil.PassiveId == boosted))
                                boostedList.Add(boosted);
                            else
                                break;
                        }
                    }
                    else
                    {
                        this.aaIdList.Add(paData.Id);
                    }
                }
            }
        }
        this.saIdList.RemoveAll(id => boostedList.Contains(ff9abil.GetSupportAbilityFromAbilityId(id)));
        for (Int32 i = 0; i < 5; ++i)
        {
            RegularItem itemId = player.Equipment[i];
            if (itemId != RegularItem.NoItem)
            {
                foreach (Int32 abilId in ff9item._FF9Item_Data[itemId].ability)
                {
                    if (!this.equipmentPartInAbilityDict.ContainsKey(abilId))
                        this.equipmentPartInAbilityDict[abilId] = 0;
                    this.equipmentPartInAbilityDict[abilId] |= 1 << i;
                    if (!this.equipmentIdInAbilityDict.ContainsKey(abilId))
                        this.equipmentIdInAbilityDict[abilId] = new RegularItem[5];
                    this.equipmentIdInAbilityDict[abilId][i] = itemId;
                }
            }
        }
        this.isAAEnable = !IsSubMenuDisabledByMainMenu(true) && player.Data.cur.hp > 0 && (player.Data.status & BattleStatusConst.CannotUseAbilityInMenu) == 0 && this.aaIdList.Count > 0 && FF9StateSystem.EventState.gEventGlobal[FF9FABIL_EVENT_NOMAGIC] == 0;
        this.isSAEnable = !IsSubMenuDisabledByMainMenu(false) && ff9abil.FF9Abil_HasSA(player);
        this.useSubMenuLabel.color = !this.isAAEnable ? FF9TextTool.Gray : FF9TextTool.White;
        this.equipSubMenuLabel.color = !this.isSAEnable ? FF9TextTool.Gray : FF9TextTool.White;
        ButtonGroupState.SetButtonAnimation(this.UseSubMenu, this.isAAEnable);
        ButtonGroupState.SetButtonAnimation(this.EquipSubMenu, this.isSAEnable);
        if (ButtonGroupState.ActiveGroup != SubMenuGroupButton)
            return;

        ButtonGroupState.MuteActiveSound = true;
        ButtonGroupState.ActiveButton = ButtonGroupState.ActiveButton;
        ButtonGroupState.MuteActiveSound = false;
    }

    public SubMenu GetSubMenuFromGameObject(GameObject go)
    {
        if (go == this.UseSubMenu)
            return SubMenu.Use;
        return go == this.EquipSubMenu ? SubMenu.Equip : SubMenu.None;
    }

    private void Awake()
    {
        this.FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
        UIEventListener.Get(this.UseSubMenu).Click += this.onClick;
        UIEventListener.Get(this.EquipSubMenu).Click += this.onClick;
        this.abilityInfoHud = new AbilityInfoHUD(this.AbilityDetailPanel);
        this.abilityInfoHud.ClearEquipmentIcon();
        this.characterHud = new CharacterDetailHUD(this.CharacterDetailPanel, false);
        this.commandLabel = this.CommandPanel.GetChild(0).GetComponent<UILabel>();
        foreach (Component component in this.TargetListPanel.transform.GetChild(0).transform)
        {
            GameObject obj = component.gameObject;
            UIEventListener.Get(obj).Click += this.onClick;
            this.targetHudList.Add(new CharacterDetailHUD(obj, true));
            if (FF9StateSystem.MobilePlatform)
                gameObject.GetComponent<ButtonGroupState>().Help.TextKey = "TargetHelpMobile";
        }

        this.targetListPanelComponent = this.TargetListPanel.GetComponent<UIPanel>();
        this.allTargetButton = this.TargetListPanel.GetChild(1);
        this.allTargetButtonComponent = this.allTargetButton.GetComponent<UIButton>();
        this.allTargetButtonCollider = this.allTargetButton.GetComponent<BoxCollider>();
        this.allTargetButtonLabel = this.allTargetButton.GetChild(1).GetComponent<UILabel>();
        this.allTargetHitArea = this.TargetListPanel.GetChild(2);
        UIEventListener.Get(this.allTargetButton).Click += OnAllTargetClick;
        this.useSubMenuLabel = this.UseSubMenu.GetChild(1).GetComponent<UILabel>();
        this.equipSubMenuLabel = this.EquipSubMenu.GetChild(1).GetComponent<UILabel>();
        this.submenuArrowGameObject = this.SubMenuPanel.GetChild(0);
        this.activeAbilityScrollList = this.ActiveAbilityListPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        this.supportAbilityScrollList = this.SupportAbilityListPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        this.targetTransition = this.TransitionGroup.GetChild(0).GetComponent<HonoTweenPosition>();
        this.avatarTransition = this.CharacterDetailPanel.GetChild(0).GetChild(6).GetChild(0).GetComponent<HonoAvatarTweenPosition>();
        this._abilityPanel = new GOScrollablePanel(this.ActiveAbilityListPanel);
        this._supportPanel = new GOScrollablePanel(this.SupportAbilityListPanel);
    }

    private Boolean IsSubMenuDisabledByMainMenu(Boolean useMenu)
    {
        String subMenuStr = useMenu ? "ActiveAbility" : "SupportingAbility";
        HashSet<String> enabledSet = PersistenSingleton<UIManager>.Instance.MainMenuScene.EnabledSubMenus;
        return enabledSet.Count > 0 && !enabledSet.Contains(MainMenuUI.SubMenu.Ability.ToString()) && !enabledSet.Contains(subMenuStr);
    }

    public class AbilityInfoHUD
    {
        public GameObject Self;
        public ButtonGroupState Button;
        public APBarHUD APBar;
        public UISprite[] EquipmentSpriteList;

        public AbilityInfoHUD(GameObject go)
        {
            this.Self = go;
            this.Button = go.GetComponent<ButtonGroupState>();
            this.APBar = new APBarHUD(go.GetChild(0).GetChild(2));
            this.EquipmentSpriteList = new UISprite[5]
            {
                go.GetChild(1).GetChild(2).GetChild(0).GetComponent<UISprite>(),
                go.GetChild(1).GetChild(2).GetChild(1).GetComponent<UISprite>(),
                go.GetChild(1).GetChild(2).GetChild(2).GetComponent<UISprite>(),
                go.GetChild(1).GetChild(2).GetChild(3).GetComponent<UISprite>(),
                go.GetChild(1).GetChild(2).GetChild(4).GetComponent<UISprite>()
            };
        }

        public void ClearEquipmentIcon()
        {
            foreach (UISprite uiSprite in this.EquipmentSpriteList)
                uiSprite.spriteName = String.Empty;
        }
    }

    public enum SubMenu
    {
        Use,
        Equip,
        None,
    }

    public enum AbilityType
    {
        NoDraw,
        Unknow,
        CantSpell,
        CantDisable,
        Enable,
        Selected,
        Max,
    }

    public class AbilityListData : ListDataTypeBase
    {
        public Int32 Id;
        public AbilityType Type;

        public AbilityListData()
        {
        }
    }
}
