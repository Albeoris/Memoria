extern alias Original;
using System;
using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using System.Collections.Generic;
using System.Linq;
using Memoria;
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

[ExportedType("Q¦ĥğ#!!!ŁüĸĘV!!!ēpìÐ@°çsĝ+ÓÈĖëĤñÏ~Ĵ­ĠċĳUċĚĂåüĘjŀ¤*ÕĔÄ2:íğçÐÇTÆÝĭĭ*óv-WĚÈ=Á¿b?:E_¢Y»ĨīfÈÜĜãĆ«²ģ¬ĻĜ$k#£ĂûĘĒ2ıHĦ£QġĢÍĳ/ñļIv¤ķjĖ¾'iíiĝÝĸĵUēß¹°ÃĭıCdaõĜĥôîĺ4āã¾ß1Ø§GCēíyè`sĴÆØA1¥Ē)łďôĽa4ÎfĕOº>=X¹áæĩĊ[ġ=īDĸ,É8:û»kłþĩ-EA8Q5õĿá>lĳĢ¬ÀO!!!F|èÅuě8;kļ>ĝĈÉãçW¡¯ĮæºSā­·Þ9mķ!ĽĮËÎ@ĩġĄìt=ĈĨw·ïÿÉPTÃób6Ġ³ºį´ċ^ĕþÎðĎ'§mÜøBĦĞøòÚńăÌĥcrXdĹ¯=ÐĢ¼Hão6ČR_ĄďĿ(vîcĎÊý¥£Ö7jĦĥIPĨĐŀiĝ°ZďIn×ĀęŁ·ğĽW0>H}À.*(PĹäèĺ|É¬líĶTU¹ô­!já|ĆĶĊÔß=©3ĴP~GEUúĵ&!!!ÛĂ×Éńńńń&!!!ëÎ¼Ĉ8ĘėûxĪÜĤ½ú¼³$!!!ó]eðĘ¬s£ńńńńëm];ńńńń&!!!õ¬ĢŁÎEóÚ¡üÚ4<jNDńńńńńńńńKamÛńńńń)!!!õ¬ĢŁc`g8ÜxēF¼$7®ĕhhè>vØTpFjñńńńńńńńńĔĦÒĩńńńń$!!!ßR/ğĜwāİ#!!!é}3®ńńńń")]
public class AbilityUI : UIScene
{
    public const int FF9FABIL_EVENT_NOMAGIC = 227;
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
    private static float TargetPositionXOffset;
    private static string SubMenuGroupButton;
    private static string ActionAbilityGroupButton;
    private static string SupportAbilityGroupButton;
    private static string TargetGroupButton;
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
    private bool isAAEnable;
    private bool isSAEnable;
    private List<int> aaIdList;
    private List<int> saIdList;
    private int currentPartyIndex;
    private SubMenu currentSubMenu;
    private int firstActiveAbility;
    private int currentAbilityIndex;
    private bool fastSwitch;
    private bool multiTarget;
    private bool canMultiTarget;
    private Dictionary<int, int> equipmentPartInAbilityDict;
    private Dictionary<int, int[]> equipmentIdInAbilityDict;
    private fld_calc fieldCalc;

    public int CurrentPartyIndex
    {
        set { this.currentPartyIndex = value; }
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
        this.aaIdList = new List<int>();
        this.saIdList = new List<int>();
        this.equipmentPartInAbilityDict = new Dictionary<int, int>();
        this.equipmentIdInAbilityDict = new Dictionary<int, int[]>();
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate action = () =>
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
        base.Show(action);
        this.SwitchCharacter(true);
        this.DisplayHelp();
        this.DisplaySubMenuArrow(true);
        this.DisplayAllButton();
        this.activeAbilityScrollList.ScrollButton.DisplayScrollButton(false, false);
        this.supportAbilityScrollList.ScrollButton.DisplayScrollButton(false, false);
    }

    public override void Hide(SceneVoidDelegate afterFinished = null)
    {
        base.Hide(afterFinished);
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

    public override bool OnKeyConfirm(GameObject go)
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
                    FF9Sfx.FF9SFX_Play(102);
            }
            else if (ButtonGroupState.ActiveGroup == ActionAbilityGroupButton)
            {
                if (ButtonGroupState.ContainButtonInGroup(go, ActionAbilityGroupButton))
                {
                    PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
                    this.currentAbilityIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                    int num = this.aaIdList[this.currentAbilityIndex];
                    this.canMultiTarget = this.IsMulti(num);
                    if (num != 0 && num < 192)
                    {
                        if (this.CheckAAType(num, player) == AbilityType.Enable)
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
                                this.TargetListPanel.transform.localPosition = new Vector3((float)(-TargetPositionXOffset - 60.0), 0.0f, 0.0f);
                            }
                            this.targetTransition.DestinationPosition = new Vector3[1]
                            {
                                this.TargetListPanel.transform.localPosition
                            };
                            this.DisplayTarget();
                            this.Loading = true;
                            this.targetTransition.TweenIn(new byte[1], () =>
                            {
                                this.Loading = false;
                                ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
                                ButtonGroupState.ActiveGroup = TargetGroupButton;
                                ButtonGroupState.HoldActiveStateOnGroup(ActionAbilityGroupButton);
                            });
                        }
                        else
                            FF9Sfx.FF9SFX_Play(102);
                    }
                }
                else
                    this.OnSecondaryGroupClick(go);
            }
            else if (ButtonGroupState.ActiveGroup == SupportAbilityGroupButton)
            {
                if (ButtonGroupState.ContainButtonInGroup(go, SupportAbilityGroupButton))
                {
                    PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
                    this.currentAbilityIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                    if (go.GetChild(0).activeSelf)
                    {
                        int num = this.saIdList[this.currentAbilityIndex];
                        AbilityType abilityType = this.CheckSAType(num, player);
                        SA_DATA saData = ff9abil._FF9Abil_SaData[num - 192];
                        if (abilityType == AbilityType.Enable)
                        {
                            FF9Sfx.FF9SFX_Play(107);
                            ff9abil.FF9Abil_SetEnableSA(player.info.slot_no, num, true);
                            player.cur.capa -= saData.capa_val;
                            ff9play.FF9Play_Update(player);
                            this.DisplaySA();
                            this.DisplayCharacter(true);
                        }
                        else if (abilityType == AbilityType.Selected)
                        {
                            FF9Sfx.FF9SFX_Play(107);
                            ff9abil.FF9Abil_SetEnableSA(player.info.slot_no, num, false);
                            player.cur.capa += saData.capa_val;
                            ff9play.FF9Play_Update(player);
                            this.DisplaySA();
                            this.DisplayCharacter(true);
                        }
                        else
                            FF9Sfx.FF9SFX_Play(102);
                    }
                    else
                        FF9Sfx.FF9SFX_Play(102);
                }
                else
                    this.OnSecondaryGroupClick(go);
            }
            else if (ButtonGroupState.ActiveGroup == TargetGroupButton && (ButtonGroupState.ContainButtonInGroup(go, TargetGroupButton) || go == this.allTargetHitArea))
            {
                bool flag = false;
                int siblingIndex = go.transform.GetSiblingIndex();
                PLAYER caster = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
                AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[this.aaIdList[this.currentAbilityIndex]];
                if (!this.multiTarget)
                {
                    flag = this.fieldCalc.FieldCalcMain(caster, FF9StateSystem.Common.FF9.party.member[siblingIndex], aaData, aaData.Ref.prog_no, 0U);
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (FF9StateSystem.Common.FF9.party.member[index] != null)
                            flag |= this.fieldCalc.FieldCalcMain(caster, FF9StateSystem.Common.FF9.party.member[index], aaData, aaData.Ref.prog_no, 1U);
                    }
                }
                if (flag)
                {
                    FF9Sfx.FF9SFX_Play(106);
                    short num = (short)GetMp(aaData);
                    if (!FF9StateSystem.Settings.IsHpMpFull)
                        caster.cur.mp -= num;
                    if (caster.cur.mp < num)
                    {
                        this.DisplayAA();
                        this.TargetListPanel.SetActive(false);
                        ButtonGroupState.ActiveGroup = ActionAbilityGroupButton;
                    }
                    BattleAchievement.IncreseNumber(ref FF9StateSystem.Achievement.whtMag_no, 1);
                    AchievementManager.ReportAchievement(AcheivementKey.WhtMag200, FF9StateSystem.Achievement.whtMag_no);
                    this.DisplayTarget();
                    this.DisplayCharacter(true);
                }
                else
                    FF9Sfx.FF9SFX_Play(102);
            }
        }
        return true;
    }

    public override bool OnKeyCancel(GameObject go)
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
                this.targetTransition.TweenOut(new byte[1], () =>
                {
                    this.Loading = false;
                    ButtonGroupState.ActiveGroup = ActionAbilityGroupButton;
                });
            }
        }
        return true;
    }

    public override bool OnKeySpecial(GameObject go)
    {
        if (base.OnKeySpecial(go) && ButtonGroupState.ActiveGroup == SubMenuGroupButton)
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
        return true;
    }

    public override bool OnKeyLeftBumper(GameObject go)
    {
        if (base.OnKeyLeftBumper(go))
        {
            if (ButtonGroupState.ActiveGroup == SubMenuGroupButton)
            {
                if (this.CharacterArrowPanel.activeSelf)
                {
                    FF9Sfx.FF9SFX_Play(1047);
                    int prev = ff9play.FF9Play_GetPrev(this.currentPartyIndex);
                    if (prev != this.currentPartyIndex)
                    {
                        this.currentPartyIndex = prev;
                        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
                        string spritName = FF9UIDataTool.AvatarSpriteName(player.info.serial_no);
                        ButtonGroupState.RemoveCursorMemorize(ActionAbilityGroupButton);
                        ButtonGroupState.RemoveCursorMemorize(SupportAbilityGroupButton);
                        this.ShowPointerWhenLoading = true;
                        this.Loading = true;
                        bool isKnockOut = player.cur.hp == 0;
                        // ISSUE: method pointer
                        this.avatarTransition.Change(spritName, HonoAvatarTweenPosition.Direction.LeftToRight, isKnockOut, () =>
                        {
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

    public override bool OnKeyRightBumper(GameObject go)
    {
        if (base.OnKeyRightBumper(go))
        {
            if (ButtonGroupState.ActiveGroup == SubMenuGroupButton)
            {
                if (this.CharacterArrowPanel.activeSelf)
                {
                    FF9Sfx.FF9SFX_Play(1047);
                    int next = ff9play.FF9Play_GetNext(this.currentPartyIndex);
                    if (next != this.currentPartyIndex)
                    {
                        this.currentPartyIndex = next;
                        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
                        string spritName = FF9UIDataTool.AvatarSpriteName(player.info.serial_no);
                        ButtonGroupState.RemoveCursorMemorize(ActionAbilityGroupButton);
                        ButtonGroupState.RemoveCursorMemorize(SupportAbilityGroupButton);
                        this.ShowPointerWhenLoading = true;
                        this.Loading = true;
                        bool isKnockOut = player.cur.hp == 0;
                        this.avatarTransition.Change(spritName, HonoAvatarTweenPosition.Direction.RightToLeft, isKnockOut, () =>
                        {
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

    public override bool OnItemSelect(GameObject go)
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
        else
        {
            if (ButtonGroupState.ActiveGroup != SupportAbilityGroupButton)
                return;

            FF9Sfx.muteSfx = true;
            this.OnKeyCancel(this.supportAbilityScrollList.GetItem(this.currentAbilityIndex).gameObject);
            FF9Sfx.muteSfx = false;
            this.OnKeyConfirm(go);
        }
    }

    private void DisplayHelp()
    {
        PLAYER play = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        ButtonGroupState component1 = this.UseSubMenu.GetComponent<ButtonGroupState>();
        ButtonGroupState component2 = this.EquipSubMenu.GetComponent<ButtonGroupState>();
        string str1 = Localization.Get("UseAbilityHelp");
        string str2 = (int)FF9StateSystem.EventState.gEventGlobal[227] == 0 ? (!FF9StateSystem.MobilePlatform ? str1 + (this.aaIdList.Count != 0 ? Localization.Get("UseAbilityHelpStatus") : Localization.Get("UseAbilityHelpForever")) : str1 + (this.aaIdList.Count != 0 ? Localization.Get("UseAbilityHelpStatusMobile") : Localization.Get("UseAbilityHelpForeverMobile"))) : (!FF9StateSystem.MobilePlatform ? str1 + Localization.Get("UseAbilityNoMagic") : str1 + Localization.Get("UseAbilityNoMagicMobile"));
        component1.Help.Text = str2;
        string str3 = Localization.Get("EquipAbilityHelp");

        if (!ff9abil.FF9Abil_HasAp(play))
            str3 = !FF9StateSystem.MobilePlatform ? str3 + (((int)play.category & 16) != 0 ? Localization.Get("EquipAbilityHelpNow") : Localization.Get("EquipAbilityForever")) : str3 + (((int)play.category & 16) != 0 ? Localization.Get("EquipAbilityHelpNowMobile") : Localization.Get("EquipAbilityForeverMobile"));

        component2.Help.Text = str3;
        this.HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);
    }

    private void DisplaySubMenuArrow(bool isEnable)
    {
        if (isEnable)
            this.submenuArrowGameObject.SetActive(!FF9StateSystem.PCPlatform);
        else
            this.submenuArrowGameObject.SetActive(false);
    }

    private void DisplayPlayerArrow(bool isEnable)
    {
        if (isEnable)
        {
            int num = 0;
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

    private void DisplayCharacter(bool updateAvatar)
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

    private void SetAbilityInfo(bool isVisible)
    {
        if (isVisible)
        {
            PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
            int index1;
            AbilityType abilityType;
            bool isShowText;
            if (this.currentSubMenu == SubMenu.Use)
            {
                index1 = this.aaIdList[this.currentAbilityIndex];
                abilityType = this.CheckAAType(index1, player);
                isShowText = (FF9StateSystem.Battle.FF9Battle.aa_data[index1].Type & 2) == 0;
            }
            else
            {
                index1 = this.saIdList[this.currentAbilityIndex];
                abilityType = this.CheckSAType(index1, player);
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
                    FF9UIDataTool.DisplayAPBar(player, index1, isShowText, this.abilityInfoHud.APBar);
                int index2 = 0;
                if (this.equipmentPartInAbilityDict.ContainsKey(index1))
                {
                    for (int index3 = 0; index3 < 5; ++index3)
                    {
                        if ((this.equipmentPartInAbilityDict[index1] & 1 << index3) != 0)
                        {
                            this.abilityInfoHud.EquipmentSpriteList[index2].alpha = 1f;
                            FF9UIDataTool.DisplayItem(this.equipmentIdInAbilityDict[index1][index3], (Original::UISprite)(object)this.abilityInfoHud.EquipmentSpriteList[index2], null, true);
                            ++index2;
                        }
                    }
                    for (int index3 = 0; index3 < this.abilityInfoHud.EquipmentSpriteList.Length; ++index3)
                    {
                        if (index3 >= index2)
                            this.abilityInfoHud.EquipmentSpriteList[index3].alpha = 0.0f;
                    }
                }
            }
            this.DisplayCommandName();
        }
        else
        {
            this.abilityInfoHud.ClearEquipmentIcon();
            this.abilityInfoHud.APBar.Slider.gameObject.SetActive(false);
            this.commandLabel.text = string.Empty;
        }
    }

    private void DisplayCommandName()
    {
        if (this.currentSubMenu != SubMenu.Use)
            return;
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        int num = this.aaIdList[this.currentAbilityIndex];
        this.commandLabel.text = this.CheckAAType(num, player) == AbilityType.NoDraw
            ? string.Empty
            : FF9TextTool.CommandName(GetCommand(num, player));
    }

    private void DisplayAA()
    {
        this.firstActiveAbility = -1;
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        using (List<int>.Enumerator enumerator = this.aaIdList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                byte num = (byte)enumerator.Current;
                AbilityListData abilityListData = new AbilityListData
                {
                    Id = num,
                    Type = this.CheckAAType(num, player)
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

    private void DisplayAADetail(Transform item, ListDataTypeBase data, int index, bool isInit)
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
            itemListDetailHud.Content.SetActive(true);
            ButtonGroupState.SetButtonAnimation(itemListDetailHud.Self, abilityListData.Type == AbilityType.Enable);
            int mp = GetMp(FF9StateSystem.Battle.FF9Battle.aa_data[abilityListData.Id]);
            itemListDetailHud.NameLabel.text = FF9TextTool.ActionAbilityName(abilityListData.Id);
            itemListDetailHud.NumberLabel.text = mp != 0 ? mp.ToString() : string.Empty;
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
            itemListDetailHud.Button.Help.Text = FF9TextTool.ActionAbilityHelpDescription(abilityListData.Id);
        }
    }

    private void DisplaySA()
    {
        this.firstActiveAbility = -1;
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        using (List<int>.Enumerator enumerator = this.saIdList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                byte num = (byte)enumerator.Current;
                AbilityListData abilityListData = new AbilityListData
                {
                    Id = num,
                    Type = this.CheckSAType(num, player)
                };
                inDataList.Add(abilityListData);
                if (this.firstActiveAbility == -1 && (abilityListData.Type == AbilityType.Enable || abilityListData.Type == AbilityType.Selected))
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

    private void DisplaySADetail(Transform item, ListDataTypeBase data, int index, bool isInit)
    {
        AbilityListData abilityListData = (AbilityListData)data;
        ItemListDetailWithIconHUD detailWithIconHud = new ItemListDetailWithIconHUD(item.gameObject, true);
        if (isInit)
            this.DisplayWindowBackground(item.gameObject, null);
        SA_DATA saData = ff9abil._FF9Abil_SaData[abilityListData.Id - 192];
        if (abilityListData.Type == AbilityType.NoDraw)
        {
            detailWithIconHud.Content.SetActive(false);
            ButtonGroupState.SetButtonAnimation(detailWithIconHud.Self, false);
            detailWithIconHud.Button.Help.Enable = false;
        }
        else
        {
            detailWithIconHud.Content.SetActive(true);
            ButtonGroupState.SetButtonAnimation(detailWithIconHud.Self, true);
            detailWithIconHud.NameLabel.text = FF9TextTool.SupportAbilityName(abilityListData.Id - 192);
            detailWithIconHud.NumberLabel.text = saData.capa_val.ToString();
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
            else if (abilityListData.Type == AbilityType.Selected)
            {
                detailWithIconHud.NameLabel.color = FF9TextTool.White;
                detailWithIconHud.NumberLabel.color = FF9TextTool.White;
                detailWithIconHud.IconSprite.atlas = FF9UIDataTool.IconAtlas;
                detailWithIconHud.IconSpriteAnimation.namePrefix = "skill_stone_gem_";
                detailWithIconHud.IconSpriteAnimation.ResetToBeginning();
            }
            detailWithIconHud.Button.Help.Enable = true;
            detailWithIconHud.Button.Help.Text = FF9TextTool.SupportAbilityHelpDescription(abilityListData.Id - 192);
        }
    }

    private void DisplayTarget()
    {
        int num = 0;
        foreach (PLAYER player in FF9StateSystem.Common.FF9.party.member)
        {
            CharacterDetailHUD charHud = this.targetHudList[num++];
            charHud.Self.SetActive(true);
            if (player != null)
            {
                charHud.Content.SetActive(true);
                FF9UIDataTool.DisplayCharacterDetail(player, charHud);
                FF9UIDataTool.DisplayCharacterAvatar(player, new Vector2(), new Vector2(), charHud.AvatarSprite, false);
                switch (FF9StateSystem.Battle.FF9Battle.aa_data[this.aaIdList[this.currentAbilityIndex]].Info.sub_win)
                {
                    case 0:
                    case 1:
                    case 2:
                        charHud.HPPanel.SetActive(true);
                        charHud.MPPanel.SetActive(true);
                        charHud.StatusesPanel.SetActive(false);
                        continue;
                    case 3:
                    case 4:
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
        this.SetAvalableCharacter();
    }

    private bool IsMulti(int abil_id)
    {
        int num1 = FF9BattleDB.aa_data[abil_id].Info.cursor;
        int num2 = 0;
        for (int index = 0; index < 4; ++index)
        {
            if (FF9StateSystem.Common.FF9.party.member[index] != null)
                ++num2;
        }
        return num2 > 1 && (num1 >= 3 && num1 <= 5);
    }

    private AbilityType CheckAAType(int abilityId, PLAYER player)
    {
        AA_DATA aa_data = FF9BattleDB.aa_data[abilityId];

        if (!this.equipmentPartInAbilityDict.ContainsKey(abilityId))
        {
            int index = ff9abil.FF9Abil_GetIndex(player.info.slot_no, abilityId);
            if (index < 0)
                return AbilityType.NoDraw;

            if (ff9abil.FF9Abil_HasAp(player))
            {
                int num1 = player.pa[index];
                int num2 = ff9abil._FF9Abil_PaData[player.info.menu_type][index].max_ap;
                if (num1 == 0 || num1 < num2)
                    return AbilityType.NoDraw;
            }
        }

        return (player.status & 9) != 0 || (aa_data.Type & 1) == 0 || GetMp(aa_data) > player.cur.mp ? AbilityType.CantSpell : AbilityType.Enable;
    }

    private AbilityType CheckSAType(int abilityId, PLAYER player)
    {
        if (abilityId < 192)
            return AbilityType.NoDraw;

        if (ff9abil.FF9Abil_IsEnableSA(player.sa, abilityId))
            return AbilityType.Selected;

        if (!this.equipmentPartInAbilityDict.ContainsKey(abilityId))
        {
            int index = ff9abil.FF9Abil_GetIndex(player.info.slot_no, abilityId);
            if (index < 0)
                return AbilityType.NoDraw;

            if (ff9abil.FF9Abil_HasAp(player))
            {
                int num1 = player.pa[index];
                int num2 = ff9abil._FF9Abil_PaData[player.info.menu_type][index].max_ap;
                if (num1 == 0 || num1 < num2)
                    return AbilityType.NoDraw;
            }
        }

        return ff9abil._FF9Abil_SaData[abilityId - 192].capa_val > player.cur.capa ? AbilityType.CantSpell : AbilityType.Enable;
    }

    private static int GetMp(AA_DATA aa_data)
    {
        int num = aa_data.MP;
        if ((aa_data.Type & 4) != 0 && FF9StateSystem.EventState.gEventGlobal[18] != 0)
            num <<= 2;
        return num;
    }

    private static int GetCommand(Int32 abil_id, PLAYER play)
    {
        for (int commandNumber = 0; commandNumber < 2; ++commandNumber)
        {
            Int32 index2 = (Int32)BattleCommands.CommandSets[play.info.menu_type].GetRegular(commandNumber);
            CharacterCommand ff9Command = BattleCommands.Commands[index2];
            if (ff9Command.Type != CharacterCommandType.Ability)
                continue;
            if (ff9Command.Abilities.Any(abilityIndex => abil_id == abilityIndex))
                return index2;
        }
        return 0;
    }

    private void SwitchCharacter(bool updateAvatar)
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

    private void SetAvalableCharacter()
    {
        List<CharacterDetailHUD> list = new List<CharacterDetailHUD>();
        using (List<CharacterDetailHUD>.Enumerator enumerator = this.targetHudList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                CharacterDetailHUD current = enumerator.Current;
                if (!this.multiTarget)
                {
                    if (current.Content.activeSelf)
                    {
                        list.Add(current);
                        ButtonGroupState.SetButtonEnable(current.Self, true);
                    }
                    else
                        ButtonGroupState.SetButtonEnable(current.Self, false);
                }
            }
        }
        for (int index1 = 0; index1 < list.Count; ++index1)
        {
            int index2 = index1 - 1;
            int index3 = index1 + 1;
            if (index1 == 0)
                index2 = list.Count - 1;
            else if (index1 == list.Count - 1)
                index3 = 0;
            UIKeyNavigation component = list[index1].Self.GetComponent<UIKeyNavigation>();
            component.onUp = list[index2].Self;
            component.onDown = list[index3].Self;
        }
    }

    private void SetMultipleTarget(bool isActive)
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
        this.aaIdList.Clear();
        this.saIdList.Clear();
        this.equipmentPartInAbilityDict.Clear();
        this.equipmentIdInAbilityDict.Clear();
        PLAYER play = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this.currentPartyIndex];
        foreach (PA_DATA paData in ff9abil._FF9Abil_PaData[player.info.menu_type])
        {
            if (paData.id != 0)
            {
                if (paData.id < 192)
                    this.aaIdList.Add(paData.id);
                else
                    this.saIdList.Add(paData.id);
            }
        }
        for (int index1 = 0; index1 < 5; ++index1)
        {
            byte num1 = play.equip[index1];
            if (num1 != byte.MaxValue)
            {
                foreach (byte num2 in ff9item._FF9Item_Data[num1].ability)
                {
                    if (!this.equipmentPartInAbilityDict.ContainsKey(num2))
                        this.equipmentPartInAbilityDict[num2] = 0;
                    Dictionary<int, int> dictionary;
                    int index2;
                    (dictionary = this.equipmentPartInAbilityDict)[index2 = num2] = dictionary[index2] + (1 << index1);
                    if (!this.equipmentIdInAbilityDict.ContainsKey(num2))
                        this.equipmentIdInAbilityDict[num2] = new int[5];
                    this.equipmentIdInAbilityDict[num2][index1] = num1;
                }
            }
        }
        this.isAAEnable = play.cur.hp > 0 && (play.status & 9) == 0 && this.aaIdList.Count > 0 && FF9StateSystem.EventState.gEventGlobal[227] == 0;
        this.isSAEnable = ff9abil.FF9Abil_HasAp(play);
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
        UIEventListener uiEventListener1 = UIEventListener.Get(this.UseSubMenu);
        uiEventListener1.Click += this.onClick;
        UIEventListener uiEventListener2 = UIEventListener.Get(this.EquipSubMenu);
        uiEventListener2.Click += this.onClick;
        this.abilityInfoHud = new AbilityInfoHUD(this.AbilityDetailPanel);
        this.abilityInfoHud.ClearEquipmentIcon();
        this.characterHud = new CharacterDetailHUD(this.CharacterDetailPanel, false);
        this.commandLabel = this.CommandPanel.GetChild(0).GetComponent<UILabel>();
        foreach (Component component in this.TargetListPanel.transform.GetChild(0).transform)
        {
            GameObject obj = component.gameObject;
            UIEventListener uiEventListener3 = UIEventListener.Get(obj);
            uiEventListener3.Click += this.onClick;
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
        UIEventListener uiEventListener4 = UIEventListener.Get(this.allTargetButton);
        uiEventListener4.Click += OnAllTargetClick;
        this.useSubMenuLabel = this.UseSubMenu.GetChild(1).GetComponent<UILabel>();
        this.equipSubMenuLabel = this.EquipSubMenu.GetChild(1).GetComponent<UILabel>();
        this.submenuArrowGameObject = this.SubMenuPanel.GetChild(0);
        this.activeAbilityScrollList = this.ActiveAbilityListPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        this.supportAbilityScrollList = this.SupportAbilityListPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        this.targetTransition = this.TransitionGroup.GetChild(0).GetComponent<HonoTweenPosition>();
        this.avatarTransition = this.CharacterDetailPanel.GetChild(0).GetChild(6).GetChild(0).GetComponent<HonoAvatarTweenPosition>();
        this.fieldCalc = new fld_calc();
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
                uiSprite.spriteName = string.Empty;
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
        Enable,
        Selected,
        Max,
    }

    public class AbilityListData : ListDataTypeBase
    {
        public int Id;
        public AbilityType Type;

        public AbilityListData()
        {
        }
    }
}