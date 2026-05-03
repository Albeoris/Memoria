using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Database;
using Memoria.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
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
    private const Int32 FF9FABIL_EVENT_NOMAGIC = 227;
    private const Single TargetPositionXOffset = 338f;
    private const String SubMenuGroupButton = "Ability.SubMenu";
    private const String ActionAbilityGroupButton = "Ability.ActionAbility";
    private const String SupportAbilityGroupButton = "Ability.SupportAbility";
    private const String TargetGroupButton = "Ability.Target";

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
    [NonSerialized]
    private GOMenuBackground _background;

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
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(40f, 0f), ActionAbilityGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(40f, 0f), SupportAbilityGroupButton);
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
        this.CommandPanel.SetActive(true);
        this.MagicStonePanel.SetActive(false);
        this.UpdateUserInterface();
    }

    public void UpdateUserInterface()
    {
        abilityInfoHud.SetupEquipmentList();
        if (NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft)
        {
            abilityInfoHud.APLabel.SetAnchor(abilityInfoHud.Self.transform, 0f, 0.5f, 1f, 1f, 30, 32, -30, -22);
            abilityInfoHud.APColon.rawText = "";
            abilityInfoHud.EquipmentLabel.SetAnchor(abilityInfoHud.Self.transform, 0f, 0.5f, 1f, 0.5f, 30, -19, -30, 18);
            abilityInfoHud.APBar.SliderSprite.SetAnchor(abilityInfoHud.Self.transform, 0f, 1f, 0f, 1f, 54, -64, 234, -30);
            abilityInfoHud.EquipmentColon.rawText = "";
        }
        else
        {
            abilityInfoHud.APBar.SliderSprite.SetAnchor(abilityInfoHud.Self.transform, 1f, 1f, 1f, 1f, -234, -64, -54, -30);
        }

        if (!Configuration.Interface.IsEnabled)
            return;

        _abilityPanel.ScrollButton.Panel.alpha = 0.5f;
        _supportPanel.ScrollButton.Panel.alpha = 0.5f;
        const Int32 originalLineCount = 6;
        const Int32 originalColumnCount = 2;
        const Single buttonOriginalHeight = 92f;
        const Single panelOriginalWidth = 1488f;
        const Single panelOriginalHeight = originalLineCount * buttonOriginalHeight;

        // ----------- ACTIVE ABILITIES ----------- //

        Int32 linePerPage = Configuration.Interface.MenuAbilityRowCount;
        Int32 columnPerPage = (Int32)Math.Floor((Single)(originalColumnCount * ((Single)linePerPage / originalLineCount)));

        if (originalColumnCount * originalLineCount >= this.aaIdList.Count) // 2 columns suffice
        {
            linePerPage = originalLineCount;
            columnPerPage = originalColumnCount;
        }
        else if (linePerPage >= originalLineCount * 2 && (originalColumnCount + 1) * (originalLineCount + originalLineCount / originalColumnCount) >= this.aaIdList.Count) // 3 columns suffice
        {
            linePerPage = originalLineCount + originalLineCount / originalColumnCount;
            columnPerPage = originalColumnCount + 1;
        }
        else if (columnPerPage == 4 && this.aaIdList.Count > 27 && this.aaIdList.Count <= 30)
        {
            columnPerPage = 3;
            linePerPage = 10;
        }
        else if (columnPerPage == 4 && this.aaIdList.Count > 30 && this.aaIdList.Count <= 33)
        {
            columnPerPage = 3;
            linePerPage = 11;
        }
        else if (columnPerPage == 4 && this.aaIdList.Count > 33 && this.aaIdList.Count <= 36)
        {
            columnPerPage = 3;
            linePerPage = 12;
        }
        Int32 lineHeight = (Int32)Math.Round(panelOriginalHeight / linePerPage);
        Single scaleFactor = lineHeight / buttonOriginalHeight;

        Single alphaColumnTitles = (columnPerPage > originalColumnCount) ? 0f : 1f;
        _abilityPanel.Background.Panel.Name.Label.alpha = alphaColumnTitles;
        _abilityPanel.Background.Panel.Info.Label.alpha = alphaColumnTitles;
        _abilityPanel.Background.Panel.Name2.Label.alpha = alphaColumnTitles;
        _abilityPanel.Background.Panel.Info2.Label.alpha = alphaColumnTitles;

        if (columnPerPage * linePerPage >= this.aaIdList.Count)
            _abilityPanel.ScrollButton.Panel.alpha = 0f;

        _abilityPanel.SubPanel.ChangeDims(columnPerPage, linePerPage, panelOriginalWidth / columnPerPage, lineHeight);
        _abilityPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _abilityPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.152f, relTop: 0.848f, relLeft: 0.091f, relRight: 0.795f);
        _abilityPanel.SubPanel.ButtonPrefab.NameLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _abilityPanel.SubPanel.ButtonPrefab.NameLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));

        _abilityPanel.SubPanel.ButtonPrefab.NumberLabel.SetAnchor(target: _abilityPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.152f, relTop: 0.848f, relLeft: 0.8f, relRight: 0.92f);
        _abilityPanel.SubPanel.ButtonPrefab.NumberLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _abilityPanel.SubPanel.ButtonPrefab.NumberLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));
        _abilityPanel.SubPanel.RecycleListPopulator.RefreshTableView();

        // ----------- SUPPORT ABILITIES ----------- //

        linePerPage = Configuration.Interface.MenuAbilityRowCount;
        columnPerPage = (Int32)Math.Floor((Single)(originalColumnCount * ((Single)linePerPage / originalLineCount)));
        if (originalColumnCount * originalLineCount >= this.saIdList.Count) // 2 columns suffice
        {
            linePerPage = originalLineCount;
            columnPerPage = originalColumnCount;
        }
        else if (linePerPage >= originalLineCount * 2 && (originalColumnCount + 1) * (originalLineCount + originalLineCount / originalColumnCount) >= this.saIdList.Count) // 3 columns suffice
        {
            linePerPage = originalLineCount + originalLineCount / originalColumnCount;
            columnPerPage = originalColumnCount + 1;
        }
        else if (columnPerPage == 4 && this.saIdList.Count > 27 && this.saIdList.Count <= 30)
        {
            columnPerPage = 3;
            linePerPage = 10;
        }
        else if (columnPerPage == 4 && this.saIdList.Count > 30 && this.saIdList.Count <= 33)
        {
            columnPerPage = 3;
            linePerPage = 11;
        }
        else if (columnPerPage == 4 && this.saIdList.Count > 33 && this.saIdList.Count <= 36)
        {
            columnPerPage = 3;
            linePerPage = 12;
        }
        lineHeight = (Int32)Math.Round(panelOriginalHeight / linePerPage);
        scaleFactor = lineHeight / buttonOriginalHeight;

        alphaColumnTitles = (columnPerPage > originalColumnCount) ? 0f : 1f;
        _supportPanel.Background.Panel.Name.Label.alpha = alphaColumnTitles;
        _supportPanel.Background.Panel.Info.Label.alpha = alphaColumnTitles;
        _supportPanel.Background.Panel.Name2.Label.alpha = alphaColumnTitles;
        _supportPanel.Background.Panel.Info2.Label.alpha = alphaColumnTitles;

        if (columnPerPage * linePerPage >= this.saIdList.Count)
            _supportPanel.ScrollButton.Panel.alpha = 0f;

        _supportPanel.SubPanel.ChangeDims(columnPerPage, linePerPage, panelOriginalWidth / columnPerPage, lineHeight);
        _supportPanel.SubPanel.ButtonPrefab.IconSprite.SetAnchor(target: _supportPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.152f, relTop: 0.848f, relLeft: 0.09f, relRight: 0.176f);
        _supportPanel.SubPanel.ButtonPrefab.IconSprite.width = _supportPanel.SubPanel.ButtonPrefab.IconSprite.height;

        _supportPanel.SubPanel.ButtonPrefab.NameLabel.SetAnchor(target: _supportPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.152f, relTop: 0.848f, relLeft: 0.24f, relRight: 0.795f);
        _supportPanel.SubPanel.ButtonPrefab.NameLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _supportPanel.SubPanel.ButtonPrefab.NameLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));

        _supportPanel.SubPanel.ButtonPrefab.NumberLabel.SetAnchor(target: _supportPanel.SubPanel.ButtonPrefab.Transform, relBottom: 0.152f, relTop: 0.848f, relLeft: 0.8f, relRight: 0.92f);
        _supportPanel.SubPanel.ButtonPrefab.NumberLabel.fontSize = (Int32)Math.Round(36f * scaleFactor);
        _supportPanel.SubPanel.ButtonPrefab.NumberLabel.effectDistance = new Vector2((Int32)Math.Round(4f * scaleFactor), (Int32)Math.Round(4f * scaleFactor));
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

    public void OnLocalize()
    {
        if (!isActiveAndEnabled)
            return;
        if (ButtonGroupState.ActiveGroup == ActionAbilityGroupButton)
            this.DisplayCommandName();
        if (this.activeAbilityScrollList.isActiveAndEnabled)
            this.activeAbilityScrollList.UpdateTableViewImp();
        if (this.supportAbilityScrollList.isActiveAndEnabled)
            this.supportAbilityScrollList.UpdateTableViewImp();
        this.DisplayHelp();
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (!base.OnKeyConfirm(go))
            return true;
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
                PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
                this.currentAbilityIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                Int32 abilId = this.aaIdList[this.currentAbilityIndex];
                if (abilId != 0 && ff9abil.IsAbilityActive(abilId))
                {
                    BattleAbilityId battleAbilId = ff9abil.GetActiveAbilityFromAbilityId(abilId);
                    BattleAbilityId patchedId = this.PatchAbility(battleAbilId);
                    TargetType targetType = FF9BattleDB.CharacterActions[patchedId].Info.Target;
                    this.canMultiTarget = this.CanToggleMulti(targetType);
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
                            this.TargetListPanel.transform.localPosition = new Vector3(-TargetPositionXOffset - 60f, 0.0f, 0.0f);
                        }
                        this.targetTransition.DestinationPosition = [this.TargetListPanel.transform.localPosition];
                        this.DisplayTarget(targetType == TargetType.Self ? player : null);
                        this.Loading = true;
                        this.targetTransition.TweenIn([0], () =>
                        {
                            this.Loading = false;
                            ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
                            ButtonGroupState.ActiveGroup = TargetGroupButton;
                            ButtonGroupState.HoldActiveStateOnGroup(ActionAbilityGroupButton);
                            this.SetMultipleTarget(targetType == TargetType.All || targetType == TargetType.AllAlly || targetType == TargetType.AllEnemy || targetType == TargetType.Everyone);
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
                PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
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
                        ff9abil.FF9Abil_SetEnableSA(player, supportId, true);
                        player.cur.capa = (UInt32)(player.cur.capa - ff9abil.GetSAGemCostFromPlayer(player, supportId));
                        ff9play.FF9Play_Update(player);
                        this.DisplaySA();
                        this.DisplayCharacter(true);
                    }
                    else if (abilityType == AbilityType.Selected)
                    {
                        List<SupportAbility> NonForcedSAList = ff9abil.GetNonForcedSAInHierarchy(player, supportId);
                        if (NonForcedSAList.Count == 0) // If all SA in the hierarchy are forced, do nothing.
                        {
                            FF9Sfx.FF9SFX_Play(102);
                            return true;
                        }
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
                                if (player.saBanish.Contains(nextBoost.Id))
                                {
                                    enableNext = false;
                                }
                                else
                                {
                                    enableNext = this.CheckSAType(ff9abil.GetAbilityIdFromSupportAbility(nextBoost.Id), player) == AbilityType.Enable;
                                    if (enableNext)
                                    {
                                        ff9abil.FF9Abil_SetEnableSA(player, nextBoost.Id, true);
                                        player.cur.capa = (UInt32)(player.cur.capa - ff9abil.GetSAGemCostFromPlayer(player, nextBoost.Id));
                                    }
                                }

                            }
                            if (!enableNext)
                            {
                                foreach (SupportAbility boosted in boostedList)
                                {
                                    if (ff9abil.FF9Abil_IsEnableSA(player.saExtended, boosted))
                                    {
                                        CharacterAbilityGems boostedGem = ff9abil._FF9Abil_SaData[boosted];
                                        ff9abil.FF9Abil_SetEnableSA(player, boosted, false);
                                        player.cur.capa = (UInt32)(player.cur.capa + ff9abil.GetSAGemCostFromPlayer(player, boosted));
                                    }
                                }
                                ff9abil.FF9Abil_SetEnableSA(player, supportId, false);
                                player.cur.capa = (UInt32)(player.cur.capa + ff9abil.GetSAGemCostFromPlayer(player, supportId));
                            }
                        }
                        else
                        {
                            ff9abil.FF9Abil_SetEnableSA(player, supportId, false);
                            player.cur.capa = (UInt32)(player.cur.capa + ff9abil.GetSAGemCostFromPlayer(player, supportId));
                        }
                        ff9play.FF9Play_Update(player);
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
            BattleAbilityId patchedId = this.PatchAbility(abilId);
            AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[patchedId];
            if (!this.multiTarget)
            {
                canUseAbility = SFieldCalculator.FieldCalcMain(caster, FF9StateSystem.Common.FF9.party.member[memberIndex], patchedId, aaData, 0u);
            }
            else
            {
                for (Int32 i = 0; i < 4; ++i)
                {
                    PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
                    if (player != null)
                        canUseAbility |= SFieldCalculator.FieldCalcMain(caster, player, patchedId, aaData, 1u);
                }
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
                FF9StateSystem.EventState.IncreaseAAUsageCounter(patchedId);
                BattleAchievement.IncreaseNumber(ref FF9StateSystem.Achievement.whtMag_no, 1);
                AchievementManager.ReportAchievement(AcheivementKey.WhtMag200, FF9StateSystem.Achievement.whtMag_no);
                this.DisplayTarget(aaData.Info.Target == TargetType.Self ? caster : null);
                this.DisplayCharacter(true);
            }
            else
            {
                FF9Sfx.FF9SFX_Play(102);
            }
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (!base.OnKeyCancel(go))
            return true;
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
                PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
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
                        ff9abil.FF9Abil_SetEnableSA(player, supportId, true);
                        player.cur.capa = (UInt32)(player.cur.capa - ff9abil.GetSAGemCostFromPlayer(player, supportId));
                        Int32 boostMaxLevel = ff9abil.GetBoostedAbilityMaxLevel(player, supportId);
                        if (boostMaxLevel > 0)
                        {
                            List<SupportAbility> boostedList = ff9abil.GetBoostedAbilityList(supportId);
                            foreach (SupportAbility boosted in boostedList)
                            {
                                AbilityType SaType = this.CheckSAType(ff9abil.GetAbilityIdFromSupportAbility(boosted), player);
                                if (SaType == AbilityType.Enable || (player.saForced.Contains(boosted) && SaType == AbilityType.Selected))
                                {
                                    CharacterAbilityGems boostedGem = ff9abil._FF9Abil_SaData[boosted];
                                    ff9abil.FF9Abil_SetEnableSA(player, boosted, true);
                                    player.cur.capa = (UInt32)(player.cur.capa - ff9abil.GetSAGemCostFromPlayer(player, boosted));
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        ff9play.FF9Play_Update(player);
                        this.DisplaySA();
                        this.DisplayCharacter(true);
                    }
                    else if (abilityType == AbilityType.Selected)
                    {
                        List<SupportAbility> NonForcedSAList = ff9abil.GetNonForcedSAInHierarchy(player, supportId);
                        if (NonForcedSAList.Count == 0) // If all SA in the hierarchy are forced, do nothing.
                        {
                            FF9Sfx.FF9SFX_Play(102);
                            return true;
                        }
                        PersistenSingleton<UIManager>.Instance.MainMenuScene.ImpactfulActionCount++;
                        FF9Sfx.FF9SFX_Play(107);
                        Int32 boostMaxLevel = ff9abil.GetBoostedAbilityMaxLevel(player, supportId);
                        if (boostMaxLevel > 0)
                        {
                            Int32 boostLevel = Math.Min(boostMaxLevel, ff9abil.GetBoostedAbilityLevel(player, supportId));
                            List<SupportAbility> boostedList = ff9abil.GetBoostedAbilityList(supportId);
                            Boolean PreviousSAAvailable = false;
                            while (!PreviousSAAvailable)
                            {
                                if (boostLevel > 0)
                                {
                                    if (!player.saForced.Contains(boostedList[boostLevel - 1]))
                                    {
                                        supportId = boostedList[boostLevel - 1];
                                        PreviousSAAvailable = true;
                                    }
                                }
                                boostLevel--;
                                if (player.saForced.Contains(supportId) && boostLevel < 0)
                                {
                                    foreach (SupportAbility boosted in boostedList)
                                    {
                                        AbilityType SaType = this.CheckSAType(ff9abil.GetAbilityIdFromSupportAbility(boosted), player);
                                        if (SaType == AbilityType.Enable || (player.saForced.Contains(boosted) && SaType == AbilityType.Selected))
                                        {
                                            CharacterAbilityGems boostedGem = ff9abil._FF9Abil_SaData[boosted];
                                            ff9abil.FF9Abil_SetEnableSA(player, boosted, true);
                                            player.cur.capa = (UInt32)(player.cur.capa - ff9abil.GetSAGemCostFromPlayer(player, boosted));
                                        }
                                        else
                                            PreviousSAAvailable = true;
                                    }
                                }
                                else
                                    PreviousSAAvailable = true;
                            }
                        }
                        ff9abil.FF9Abil_SetEnableSA(player, supportId, false);
                        player.cur.capa = (UInt32)(player.cur.capa + ff9abil.GetSAGemCostFromPlayer(player, supportId));
                        ff9play.FF9Play_Update(player);
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
        if (!base.OnKeyLeftBumper(go))
            return true;
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
                    UpdateUserInterface();
                }
            }
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
                    UpdateUserInterface();
                }
            }
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
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
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
            this.CharacterArrowPanel.SetActive(FF9StateSystem.Common.FF9.party.member.Count(player => player != null) > 1);
        else
            this.CharacterArrowPanel.SetActive(false);
    }

    private void DisplayCharacter(Boolean updateAvatar)
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        FF9UIDataTool.DisplayCharacterDetail(player, this.characterHud);
        if (updateAvatar)
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
            PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
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
                    FF9UIDataTool.DisplayAPBar(player, abilId, isShowText, this.abilityInfoHud.APBar);
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
            this.commandLabel.rawText = String.Empty;
        }
    }

    private void DisplayCommandName()
    {
        if (this.currentSubMenu != SubMenu.Use)
            return;
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        Int32 abilityId = this.aaIdList[this.currentAbilityIndex];
        this.commandLabel.rawText = this.CheckAAType(abilityId, player) == AbilityType.NoDraw
            ? String.Empty
            : FF9TextTool.CommandName(GetCommand(abilityId, player));
    }

    private void DisplayAA()
    {
        this.firstActiveAbility = -1;
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        foreach (Int32 abilId in this.aaIdList)
        {
            AbilityListData abilityListData = new AbilityListData
            {
                Id = abilId,
                Type = this.CheckAAType(abilId, player)
            };
            inDataList.Add(abilityListData);
            if (this.firstActiveAbility == -1 && abilityListData.Type == AbilityType.Enable)
                this.firstActiveAbility = inDataList.Count - 1;
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
            this.activeAbilityScrollList.JumpToIndex(this.firstActiveAbility, true);
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
            itemListDetailHud.NameLabel.rawText = FF9TextTool.ActionAbilityName(patchedId);
            itemListDetailHud.NumberLabel.rawText = mp != 0 ? mp.ToString() : String.Empty;
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
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        foreach (Int32 abilId in this.saIdList)
        {
            AbilityListData abilityListData = new AbilityListData
            {
                Id = abilId,
                Type = this.CheckSAType(abilId, player)
            };
            inDataList.Add(abilityListData);
            if (this.firstActiveAbility == -1 && (abilityListData.Type == AbilityType.Enable || abilityListData.Type == AbilityType.Selected || abilityListData.Type == AbilityType.CantDisable))
                this.firstActiveAbility = inDataList.Count - 1;
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
            this.supportAbilityScrollList.JumpToIndex(this.firstActiveAbility, true);
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
            PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
            detailWithIconHud.Content.SetActive(true);
            ButtonGroupState.SetButtonAnimation(detailWithIconHud.Self, true);
            detailWithIconHud.NameLabel.rawText = FF9TextTool.SupportAbilityName(supportId);
            detailWithIconHud.NumberLabel.rawText = ff9abil.GetSAGemCostFromPlayer(player, supportId).ToString();
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
                    Int32 stoneCost = ff9abil.GetSAGemCostFromPlayer(player, supportId);
                    for (Int32 i = 0; i < level; i++)
                        stoneCost += ff9abil.GetSAGemCostFromPlayer(player, boostedList[i]);
                    if (level > 0)
                        supportId = boostedList[level - 1];
                    detailWithIconHud.NameLabel.rawText = FF9TextTool.SupportAbilityName(supportId);
                    detailWithIconHud.NumberLabel.rawText = stoneCost.ToString();
                    detailWithIconHud.IconSprite.color = BoostedAbilityColor[Math.Min(level, BoostedAbilityColor.Length - 1)];
                }
            }
            detailWithIconHud.Button.Help.Enable = true;
            detailWithIconHud.Button.Help.Text = FF9TextTool.SupportAbilityHelpDescription(supportId);
            ButtonGroupState.RefreshHelpDialog();
        }
    }

    private void DisplayTarget(PLAYER onlySelfMode = null)
    {
        Int32 hudIndex = 0;
        foreach (PLAYER player in FF9StateSystem.Common.FF9.party.member)
        {
            CharacterDetailHUD charHud = this.targetHudList[hudIndex++];
            charHud.Self.SetActive(true);
            if (player == null || (onlySelfMode != null && player != onlySelfMode))
            {
                charHud.Content.SetActive(false);
                continue;
            }
            charHud.Content.SetActive(true);
            FF9UIDataTool.DisplayCharacterDetail(player, charHud);
            FF9UIDataTool.DisplayCharacterAvatar(player, new Vector2(), new Vector2(), charHud.AvatarSprite, false);
            AA_DATA patchedAbil = FF9StateSystem.Battle.FF9Battle.aa_data[this.PatchAbility(ff9abil.GetActiveAbilityFromAbilityId(this.aaIdList[this.currentAbilityIndex]))];
            switch (patchedAbil.Info.DisplayStats)
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
        this.SetAvailableCharacter();
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

    private AbilityType CheckAAType(Int32 abilityId, PLAYER player)
    {
        BattleAbilityId patchedId = this.PatchAbility(ff9abil.GetActiveAbilityFromAbilityId(abilityId));
        AA_DATA patchedAbil = FF9BattleDB.CharacterActions[patchedId];

        if (!this.equipmentPartInAbilityDict.ContainsKey(abilityId))
        {
            Int32 index = ff9abil.FF9Abil_GetIndex(player, abilityId);
            if (index < 0)
                return AbilityType.NoDraw;

            if (ff9abil.FF9Abil_HasAp(player))
            {
                if ((Configuration.Battle.LockEquippedAbilities == 2 || Configuration.Battle.LockEquippedAbilities == 3) && player.Index != CharacterId.Quina)
                    return AbilityType.NoDraw;
                Int32 currentAp = player.pa[index];
                Int32 learnAp = ff9abil._FF9Abil_PaData[player.PresetId][index].Ap;
                if (currentAp < learnAp)
                    return AbilityType.NoDraw;
            }
        }

        return (player.status & BattleStatusConst.CannotUseAbilityInMenu) != 0 || (patchedAbil.Type & 1) == 0 || GetMp(patchedAbil) > player.cur.mp ? AbilityType.CantSpell : AbilityType.Enable;
    }

    private AbilityType CheckSAType(Int32 abilityId, PLAYER player)
    {
        if (!ff9abil.IsAbilitySupport(abilityId))
            return AbilityType.NoDraw;

        if (player.saHidden.Contains(ff9abil.GetSupportAbilityFromAbilityId(abilityId)))
            return AbilityType.NoDraw;

        if (player.saBanish.Contains(ff9abil.GetSupportAbilityFromAbilityId(abilityId)))
            return player.saForced.Contains(ff9abil.GetSupportAbilityFromAbilityId(abilityId)) ? AbilityType.CantDisable : AbilityType.CantSpell;

        if (Configuration.Battle.LockEquippedAbilities == 1 || Configuration.Battle.LockEquippedAbilities == 3)
        {
            if (ff9abil.FF9Abil_GetIndex(player, abilityId) < 0)
                return AbilityType.NoDraw;
            return (this.equipmentPartInAbilityDict.ContainsKey(abilityId) || player.saForced.Contains(ff9abil.GetSupportAbilityFromAbilityId(abilityId))) ? AbilityType.CantDisable : AbilityType.CantSpell;
        }

        if (ff9abil.FF9Abil_IsEnableSA(player.saExtended, ff9abil.GetSupportAbilityFromAbilityId(abilityId)))
            return AbilityType.Selected;

        if (!this.equipmentPartInAbilityDict.ContainsKey(abilityId))
        {
            Int32 index = ff9abil.FF9Abil_GetIndex(player, abilityId);
            if (index < 0)
                return AbilityType.NoDraw;

            if (ff9abil.FF9Abil_HasAp(player))
            {
                Int32 currentAp = player.pa[index];
                Int32 learnAp = ff9abil._FF9Abil_PaData[player.PresetId][index].Ap;
                if (currentAp < learnAp)
                    return AbilityType.NoDraw;
            }
        }

        return ff9abil.GetSupportAbilityGem(abilityId).GemsCount > player.cur.capa ? AbilityType.CantSpell : AbilityType.Enable;
    }

    private static Int32 GetMp(AA_DATA aa_data)
    {
        Int32 mpCost = aa_data.MP;
        if ((aa_data.Type & 4) != 0 && battle.GARNET_SUMMON_FLAG != 0)
            mpCost <<= 2;
        return mpCost;
    }

    private static BattleCommandId GetCommand(Int32 abil_id, PLAYER play)
    {
        BattleAbilityId battleAbilId = ff9abil.GetActiveAbilityFromAbilityId(abil_id);
        foreach (BattleCommandMenu menu in CharacterCommandSet.SupportedMenus)
        {
            BattleCommandId cmdId = CharacterCommands.CommandSets[play.PresetId].GetRegular(menu);
            foreach (BattleAbilityId abilId in CharacterCommands.Commands[cmdId].EnumerateAbilities())
                if (abilId == battleAbilId)
                    return cmdId;
            BattleCommandId patchedId = BattleCommandHelper.Patch(cmdId, menu, play);
            if (patchedId != cmdId)
                foreach (BattleAbilityId abilId in CharacterCommands.Commands[patchedId].EnumerateAbilities())
                    if (abilId == battleAbilId)
                        return cmdId;
        }
        foreach (BattleCommandMenu menu in CharacterCommandSet.SupportedMenus)
        {
            BattleCommandId cmdId = CharacterCommands.CommandSets[play.PresetId].GetTrance(menu);
            foreach (BattleAbilityId abilId in CharacterCommands.Commands[cmdId].EnumerateAbilities())
                if (abilId == battleAbilId)
                    return cmdId;
            BattleCommandId patchedId = BattleCommandHelper.Patch(cmdId, menu, play);
            if (patchedId != cmdId)
                foreach (BattleAbilityId abilId in CharacterCommands.Commands[patchedId].EnumerateAbilities())
                    if (abilId == battleAbilId)
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
        foreach (CharacterDetailHUD charHUD in this.targetHudList)
        {
            if (!this.multiTarget)
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
        foreach (CharacterDetailHUD charHUD in this.targetHudList)
            charHUD.Self.GetComponent<ButtonGroupState>().Help.Enable = !isActive;
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
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
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
            RegularItem itemId = player.equip[i];
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
        this.isAAEnable = !IsSubMenuDisabledByMainMenu(true) && player.cur.hp > 0 && (player.status & BattleStatusConst.CannotUseAbilityInMenu) == 0 && this.aaIdList.Count > 0 && FF9StateSystem.EventState.gEventGlobal[FF9FABIL_EVENT_NOMAGIC] == 0;
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
        this._background = new GOMenuBackground(this.transform.GetChild(7).gameObject, "ability_bg");
        this.abilityInfoHud.Background.Caption.Label.rightAnchor.Set(1f, -40);
        this.CharacterDetailPanel.GetChild(4).GetChild(3).GetComponent<UILabel>().rightAnchor.Set(1f, -20);
        this._abilityPanel.Background.Panel.Name.Label.fixedAlignment = true;
        this._abilityPanel.Background.Panel.Name2.Label.fixedAlignment = true;
        this._supportPanel.Background.Panel.Name.Label.fixedAlignment = true;
        this._supportPanel.Background.Panel.Name2.Label.fixedAlignment = true;
        this._supportPanel.Background.Panel.Info.Label.leftAnchor.Set(0f, 68);
        this._supportPanel.Background.Panel.Info2.Label.leftAnchor.Set(0.5f, 68);
        this._supportPanel.Background.Panel.Info.Localize.key = "MagicStoneCaption";
        this._supportPanel.Background.Panel.Info2.Localize.key = "MagicStoneCaption";
        this.TargetListPanel.GetChild(3).GetChild(2).GetComponent<UILabel>().rightAnchor.Set(1f, -40);
        this.useSubMenuLabel.leftAnchor.Set(0f, 0);
        this.useSubMenuLabel.rightAnchor.Set(1f, 0);
        this.equipSubMenuLabel.leftAnchor.Set(0f, 0);
        this.equipSubMenuLabel.rightAnchor.Set(1f, 0);
    }

    private Boolean IsSubMenuDisabledByMainMenu(Boolean useMenu)
    {
        String subMenuStr = useMenu ? "ActiveAbility" : "SupportingAbility";
        HashSet<String> enabledSet = PersistenSingleton<UIManager>.Instance.MainMenuScene.EnabledSubMenus;
        return enabledSet.Count > 0 && !enabledSet.Contains(MainMenuUI.SubMenu.Ability.ToString()) && !enabledSet.Contains(subMenuStr);
    }

    internal class AbilityInfoHUD
    {
        public GameObject Self;
        public ButtonGroupState Button;
        public UILabel APLabel;
        public UILabel APColon;
        public APBarHUD APBar;
        public UILabel EquipmentLabel;
        public UILabel EquipmentColon;
        public UISprite[] EquipmentSpriteList;
        public GOFrameBackground Background;

        public AbilityInfoHUD(GameObject go)
        {
            this.Self = go;
            this.Button = go.GetComponent<ButtonGroupState>();
            this.APLabel = go.GetChild(0).GetChild(0).GetComponent<UILabel>();
            this.APColon = go.GetChild(0).GetChild(1).GetComponent<UILabel>();
            this.APBar = new APBarHUD(go.GetChild(0).GetChild(2));
            this.EquipmentLabel = go.GetChild(1).GetChild(0).GetComponent<UILabel>();
            this.EquipmentColon = go.GetChild(1).GetChild(1).GetComponent<UILabel>();
            this.SetupEquipmentList();
            this.Background = new GOFrameBackground(go.GetChild(2));
        }

        public void ClearEquipmentIcon()
        {
            foreach (UISprite uiSprite in this.EquipmentSpriteList)
                uiSprite.spriteName = String.Empty;
        }

        public void SetupEquipmentList()
        {
            if (NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft)
            {
                this.EquipmentSpriteList =
                [
                    this.Self.GetChild(1).GetChild(2).GetChild(4).GetComponent<UISprite>(),
                    this.Self.GetChild(1).GetChild(2).GetChild(3).GetComponent<UISprite>(),
                    this.Self.GetChild(1).GetChild(2).GetChild(2).GetComponent<UISprite>(),
                    this.Self.GetChild(1).GetChild(2).GetChild(1).GetComponent<UISprite>(),
                    this.Self.GetChild(1).GetChild(2).GetChild(0).GetComponent<UISprite>()
                ];
            }
            else
            {
                this.EquipmentSpriteList =
                [
                    this.Self.GetChild(1).GetChild(2).GetChild(0).GetComponent<UISprite>(),
                    this.Self.GetChild(1).GetChild(2).GetChild(1).GetComponent<UISprite>(),
                    this.Self.GetChild(1).GetChild(2).GetChild(2).GetComponent<UISprite>(),
                    this.Self.GetChild(1).GetChild(2).GetChild(3).GetComponent<UISprite>(),
                    this.Self.GetChild(1).GetChild(2).GetChild(4).GetComponent<UISprite>()
                ];
            }
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
