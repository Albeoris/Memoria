extern alias Original;

using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using Memoria;
using UnityEngine;
using Object = UnityEngine.Object;

#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

// ReSharper disable EmptyConstructor
// ReSharper disable RedundantAssignment
// ReSharper disable ArrangeThisQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToConstant.Global
// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable InconsistentNaming

[ExportedType("7&³ó#!!!L±Ĭĉ5!!!9î'tłĲă³Ë©V1ĎĔXNÏ~Ĵ­QipôèßļFàı°A=Á¿bč*Ûk:ĤĲËüąõ±ČøŀÏģYĔdĢ0ÂñäĄÙĤD°Ùô¦ċÖô2±äô8!!!ß]~Ð.ĀwOaī»þ÷ĢêĊß%īĥÌÖÃvÆÞ±§]¿¿=ČûČĝÎl<Ĳ­°lÿŃpÏ9ñĸðPO®bÛ]d±½÷êNÎß?Ï¾ªÕK63wºĶĀě7ÿĆ4!?ÁïÞi&!!!ÙÁð½ńńńń&!!!õ¬ĢŁäöylē5'åcbLyńńńńńńńńĻ±hEńńńń$!!!¼4÷ĪãÒćW#!!!ùcĝ´ńńńńđO7Gńńńń&!!!ëÎ¼ĈÞĚBQûæêÃãÒćW#!!!·Bą±ńńńńÐa$9ńńńń$!!!üw·½Ģ;iĭ#!!!ĠġàÞńńńń")]
public class PartySettingUI : UIScene
{
    public enum Mode
    {
        Menu,
        Select,
        None
    }

    public class CharacterDetailPartyHUD : CharacterDetailHUD
    {
        public UILabel EmptyLabel;

        public UISprite[] StatusesAvatar;

        public CharacterDetailPartyHUD(GameObject go, Boolean isTargetHud) : base(go, isTargetHud)
        {
            this.EmptyLabel = go.GetChild(1).GetComponent<UILabel>();
            this.StatusesAvatar = new[]
            {
                this.AvatarSprite.gameObject.GetChild(0).GetComponent<UISprite>(),
                this.AvatarSprite.gameObject.GetChild(1).GetComponent<UISprite>(),
                this.AvatarSprite.gameObject.GetChild(2).GetComponent<UISprite>()
            };
        }
    }

    public class CharacterOutsidePartyHud
    {
        public GameObject Self;

        public GameObject MoveButton;

        public UISprite Avatar;

        public UISprite[] StatusesAvatar;

        public CharacterOutsidePartyHud(GameObject go)
        {
            this.Self = go;
            this.MoveButton = go.GetChild(0);
            this.Avatar = go.GetChild(2).GetComponent<UISprite>();
            this.StatusesAvatar = new[]
            {
                go.GetChild(2).GetChild(0).GetComponent<UISprite>(),
                go.GetChild(2).GetChild(1).GetComponent<UISprite>(),
                go.GetChild(2).GetChild(2).GetComponent<UISprite>()
            };
        }
    }

    public class PartySelect
    {
        public Mode Group;

        public Int32 Index;

        public PartySelect(Mode _group, Int32 _index)
        {
            this.Group = _group;
            this.Index = _index;
        }
    }

    public static Int32 FF9PARTY_PLAYER_MAX = 8;

    public static Byte FF9PARTY_NONE = 255;

    private static String SelectCharGroupButton = "Party.Select";

    private static String MoveCharGroupButton = "Party.Move";

    public GameObject HelpDespLabelGameObject;

    public GameObject CurrentPartyPanel;

    public GameObject OutsidePartyPanel;

    public GameObject HelpInfoPanel;

    public GameObject ScreenFadeGameObject;

    private UILabel helpLabel;

    private CharacterDetailPartyHUD currentCharacterHud;

    private List<CharacterDetailPartyHUD> currentPartyHudList = new List<CharacterDetailPartyHUD>();

    private List<CharacterOutsidePartyHud> outsidePartyHudList = new List<CharacterOutsidePartyHud>();

    private PartySelect currentCharacterSelect = new PartySelect(Mode.None, 0);

    private Byte currentCharacterId;

    private FF9PARTY_INFO info = new FF9PARTY_INFO();

    public FF9PARTY_INFO Info
    {
        get { return this.info; }
        set { this.info = value; }
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        TryHackPartyInfo(this.info);

        SceneVoidDelegate sceneVoidDelegate = delegate
        {
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(18f, -18f), MoveCharGroupButton);
            ButtonGroupState.SetPointerDepthToGroup(6, MoveCharGroupButton);
            ButtonGroupState.ActiveGroup = SelectCharGroupButton;
        };
        if (afterFinished != null)
        {
            sceneVoidDelegate = (SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(sceneVoidDelegate);
        this.DisplayCharacter();
        this.DisplayCharacterInfo(this.currentCharacterId);
        this.DisplayHelpInfo();
        this.HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);
    }

    public override void Hide(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate sceneVoidDelegate = delegate
        {
            SceneDirector.FF9Wipe_FadeInEx(12);
        };
        if (afterFinished != null)
        {
            sceneVoidDelegate = (SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        base.Hide(sceneVoidDelegate);
        BattleAchievement.UpdateParty();
        this.RemoveCursorMemorize();
    }

    private void RemoveCursorMemorize()
    {
        ButtonGroupState.RemoveCursorMemorize(SelectCharGroupButton);
        ButtonGroupState.RemoveCursorMemorize(MoveCharGroupButton);
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go))
        {
            if (ButtonGroupState.ActiveGroup == SelectCharGroupButton)
            {
                FF9Sfx.FF9SFX_Play(103);
                this.currentCharacterSelect = this.GetCurrentSelect(go);
                this.currentCharacterId = this.GetCurrentId(go);
                ButtonGroupState.SetCursorStartSelect((this.currentCharacterSelect.Group != Mode.Menu) ? go.GetChild(0) : go.GetChild(2), MoveCharGroupButton);
                ButtonGroupState.RemoveCursorMemorize(MoveCharGroupButton);
                ButtonGroupState.ActiveGroup = MoveCharGroupButton;
                ButtonGroupState.HoldActiveStateOnGroup(SelectCharGroupButton);
                foreach (CharacterOutsidePartyHud current in this.outsidePartyHudList)
                {
                    ButtonGroupState.SetButtonEnable(current.MoveButton, this.currentCharacterId == FF9PARTY_NONE || !this.info.fix[this.currentCharacterId]);
                }
            }
            else if (ButtonGroupState.ActiveGroup == MoveCharGroupButton)
            {
                PartySelect currentSelect = this.GetCurrentSelect(go);
                Byte currentId = this.GetCurrentId(go);
                if (this.currentCharacterSelect.Group == Mode.Select && currentId != FF9PARTY_NONE && this.info.fix[currentId])
                {
                    FF9Sfx.FF9SFX_Play(102);
                }
                else
                {
                    FF9Sfx.FF9SFX_Play(103);
                    this.SwapCharacter(this.currentCharacterSelect, currentSelect);
                    this.DisplayCharacter();
                    this.DisplayCharacterInfo(this.currentCharacterId);
                    ButtonGroupState.SetCursorMemorize(go.transform.parent.gameObject, SelectCharGroupButton);
                    ButtonGroupState.ActiveGroup = SelectCharGroupButton;
                }
            }
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go))
        {
            if (ButtonGroupState.ActiveGroup == SelectCharGroupButton)
            {
                if (this.FF9Party_Check())
                {
                    for (Int32 i = 0; i < this.info.party_ct; i++)
                    {
                        ff9play.FF9Play_SetParty(i, (FF9PARTY_NONE != this.info.menu[i]) ? this.info.menu[i] : -1);
                    }
                    FF9Sfx.FF9SFX_Play(103);
                    this.Hide(delegate
                    {
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(PersistenSingleton<UIManager>.Instance.HUDState);
                    });
                }
                else
                {
                    FF9Sfx.FF9SFX_Play(102);
                }
            }
            else if (ButtonGroupState.ActiveGroup == MoveCharGroupButton)
            {
                ButtonGroupState.ActiveGroup = SelectCharGroupButton;
                FF9Sfx.FF9SFX_Play(101);
            }
        }
        return true;
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (base.OnItemSelect(go))
        {
            if (ButtonGroupState.ActiveGroup == SelectCharGroupButton)
            {
                Byte currentId = this.GetCurrentId(go);
                if (this.currentCharacterId != currentId)
                {
                    this.currentCharacterId = currentId;
                    this.DisplayCharacterInfo(this.currentCharacterId);
                }
            }
            else if (ButtonGroupState.ActiveGroup == MoveCharGroupButton)
            {
                Byte currentId2 = this.GetCurrentId(go);
                this.DisplayCharacterInfo(currentId2);
            }
        }
        return true;
    }

    private void DisplayHelpInfo()
    {
        switch (this.info.party_ct)
        {
            case 1:
                this.helpLabel.text = Localization.Get("PartyHelp1Desp");
                break;
            case 2:
                this.helpLabel.text = Localization.Get("PartyHelp2Desp");
                break;
            case 3:
                this.helpLabel.text = Localization.Get("PartyHelp3Desp");
                break;
            case 4:
                this.helpLabel.text = Localization.Get("PartyHelp4Desp");
                break;
            default:
                this.helpLabel.text = Localization.Get("PartyHelp4Desp");
                break;
        }
    }

    private void DisplayCharacter()
    {
        Int32 num = 0;
        Byte[] menu = this.info.menu;
        for (Int32 i = 0; i < menu.Length; i++)
        {
            Byte b = menu[i];
            CharacterDetailPartyHUD characterDetailPartyHUD = this.currentPartyHudList[num++];
            if (b != FF9PARTY_NONE)
            {
                PLAYER player = FF9StateSystem.Common.FF9.player[b];
                characterDetailPartyHUD.Self.SetActive(true);
                characterDetailPartyHUD.Content.SetActive(true);
                characterDetailPartyHUD.EmptyLabel.gameObject.SetActive(false);
                FF9UIDataTool.DisplayCharacterDetail(player, characterDetailPartyHUD);
                this.DisplayCharacterPartyAvatar(b, player, (UISprite)(Object)characterDetailPartyHUD.AvatarSprite, characterDetailPartyHUD.StatusesAvatar);
            }
            else
            {
                characterDetailPartyHUD.EmptyLabel.text = String.Format(Localization.Get("EmptyCharNumber"), num);
                characterDetailPartyHUD.Content.SetActive(false);
                characterDetailPartyHUD.EmptyLabel.gameObject.SetActive(true);
            }
        }
        num = 0;
        Byte[] select = this.info.select;
        for (Int32 j = 0; j < select.Length; j++)
        {
            Byte b2 = select[j];
            CharacterOutsidePartyHud characterOutsidePartyHud = this.outsidePartyHudList[num++];
            if (b2 != FF9PARTY_NONE)
            {
                PLAYER player2 = FF9StateSystem.Common.FF9.player[b2];
                this.DisplayCharacterPartyAvatar(b2, player2, characterOutsidePartyHud.Avatar, characterOutsidePartyHud.StatusesAvatar);
            }
            else
            {
                characterOutsidePartyHud.Avatar.alpha = 0f;
            }
        }
    }

    private void DisplayCharacterInfo(Int32 charId)
    {
        if (charId != FF9PARTY_NONE)
        {
            PLAYER player = FF9StateSystem.Common.FF9.player[charId];
            this.currentCharacterHud.Content.SetActive(true);
            FF9UIDataTool.DisplayCharacterDetail(player, this.currentCharacterHud);
            this.DisplayCharacterPartyAvatar((Byte)charId, player, (UISprite)(Object)this.currentCharacterHud.AvatarSprite, this.currentCharacterHud.StatusesAvatar);
        }
        else
        {
            this.currentCharacterHud.Content.SetActive(false);
        }
    }

    private void DisplayCharacterPartyAvatar(Byte id, PLAYER player, UISprite avatarSprite, UISprite[] avatarStatusAlignment)
    {
        FF9UIDataTool.DisplayCharacterAvatar(player, default(Vector3), default(Vector3), (Original::UISprite)(Object)avatarSprite, false);
        avatarSprite.alpha = ((!this.info.fix[id]) ? 1f : 0.5f);
        for (Int32 i = 0; i < avatarStatusAlignment.Length; i++)
        {
            UISprite uISprite = avatarStatusAlignment[i];
            uISprite.alpha = 0f;
        }
        Int32 num = 0;
        if ((player.status & 2) != 0)
        {
            avatarStatusAlignment[num].spriteName = Localization.Get("PartyStatusTextPoison");
            avatarStatusAlignment[num].alpha = 1f;
            num++;
        }
        if ((player.status & 1) != 0)
        {
            avatarStatusAlignment[num].spriteName = Localization.Get("PartyStatusTextStone");
            avatarStatusAlignment[num].alpha = 1f;
            num++;
        }
        if (player.cur.hp == 0)
        {
            avatarStatusAlignment[num].spriteName = "text_ko";
            avatarStatusAlignment[num].alpha = 1f;
            num++;
        }
    }

    private Boolean FF9Party_Check()
    {
        Int32 num2;
        Int32 i;
        Int32 num = i = (num2 = 0);
        while (i < 4)
        {
            Int32 num3 = this.info.menu[i];
            if (FF9PARTY_NONE != num3)
            {
                num++;
                if (FF9StateSystem.Common.FF9.player[num3].cur.hp != 0 && (FF9StateSystem.Common.FF9.player[num3].status & 3) == 0)
                {
                    num2++;
                }
            }
            i++;
        }
        return num >= this.info.party_ct && num2 != 0;
    }

    private Byte GetCurrentId(GameObject go)
    {
        Int32 siblingIndex;
        GameObject obj;
        if (ButtonGroupState.ActiveGroup == SelectCharGroupButton)
        {
            siblingIndex = go.transform.GetSiblingIndex();
            obj = go;
        }
        else
        {
            siblingIndex = go.transform.parent.GetSiblingIndex();
            obj = go.transform.parent.gameObject;
        }
        if (obj.transform.parent.parent == this.CurrentPartyPanel.transform)
        {
            return this.info.menu[siblingIndex];
        }
        return this.info.select[siblingIndex];
    }

    private PartySelect GetCurrentSelect(GameObject go)
    {
        if (ButtonGroupState.ActiveGroup == SelectCharGroupButton)
        {
            return new PartySelect((!(go.transform.parent.parent == this.CurrentPartyPanel.transform)) ? Mode.Select : Mode.Menu, go.transform.GetSiblingIndex());
        }
        return new PartySelect((!(go.transform.parent.parent.parent == this.CurrentPartyPanel.transform)) ? Mode.Select : Mode.Menu, go.transform.parent.GetSiblingIndex());
    }

    private void SwapCharacter(PartySelect oldSelect, PartySelect newSelect)
    {
        Byte b = this.currentCharacterId;
        Byte b2 = FF9PARTY_NONE;
        if (newSelect.Group == Mode.Menu)
        {
            b2 = this.info.menu[newSelect.Index];
            this.info.menu[newSelect.Index] = b;
        }
        else if (newSelect.Group == Mode.Select)
        {
            b2 = this.info.select[newSelect.Index];
            this.info.select[newSelect.Index] = b;
        }
        if (oldSelect.Group == Mode.Menu)
        {
            this.info.menu[oldSelect.Index] = b2;
        }
        else if (oldSelect.Group == Mode.Select)
        {
            this.info.select[oldSelect.Index] = b2;
        }
    }

    private void Awake()
    {
        FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
        this.helpLabel = this.HelpInfoPanel.GetChild(0).GetComponent<UILabel>();
        foreach (Transform trans in this.CurrentPartyPanel.GetChild(0).transform)
        {
            GameObject obj = trans.gameObject;
            UIEventListener expr_5D = UIEventListener.Get(obj);
            expr_5D.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_5D.onClick, new UIEventListener.VoidDelegate(this.onClick));
            UIEventListener expr_8B = UIEventListener.Get(obj.GetChild(2));
            expr_8B.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_8B.onClick, new UIEventListener.VoidDelegate(this.onClick));
            CharacterDetailPartyHUD item = new CharacterDetailPartyHUD(obj, true);
            this.currentPartyHudList.Add(item);
        }
        foreach (Transform transform2 in this.OutsidePartyPanel.GetChild(1).GetChild(0).transform)
        {
            GameObject gameObject2 = transform2.gameObject;
            UIEventListener expr_127 = UIEventListener.Get(gameObject2);
            expr_127.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_127.onClick, new UIEventListener.VoidDelegate(this.onClick));
            UIEventListener expr_156 = UIEventListener.Get(gameObject2.GetChild(0));
            expr_156.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_156.onClick, new UIEventListener.VoidDelegate(this.onClick));
            CharacterOutsidePartyHud item2 = new CharacterOutsidePartyHud(gameObject2);
            this.outsidePartyHudList.Add(item2);
        }
        this.currentCharacterHud = new CharacterDetailPartyHUD(this.OutsidePartyPanel.GetChild(0), false);
    }

    private static void TryHackPartyInfo(FF9PARTY_INFO partyInfo)
    {
        if (Configuration.Hacks.AllCharactersAvailable < 1)
            return;

        if (Configuration.Hacks.AllCharactersAvailable == 2)
        {
            foreach (PLAYER player in FF9StateSystem.Common.FF9.player)
                TryHackPlayer(player, (Byte)(player.category & ~16));
        }
        else if (Configuration.Hacks.AllCharactersAvailable == 3)
        {
            foreach (PLAYER player in FF9StateSystem.Common.FF9.player)
                TryHackPlayer(player, (Byte)(player.category | 16));
        }

        Int32 availabilityMask = -1;

        for (Int32 memberIndex = 0; memberIndex < 4; ++memberIndex)
        {
            PLAYER member = FF9StateSystem.Common.FF9.party.member[memberIndex];
            if (member != null)
            {
                partyInfo.menu[memberIndex] = member.info.slot_no;
                availabilityMask &= ~(1 << partyInfo.menu[memberIndex]);
            }
            else
            {
                partyInfo.menu[memberIndex] = Byte.MaxValue;
            }
        }

        Byte characterIndex = 0;
        Byte slotIndex = 0;
        while (slotIndex < 9 && characterIndex < FF9PARTY_PLAYER_MAX && availabilityMask != 0)
        {
            if ((availabilityMask & 1) > 0)
                partyInfo.select[characterIndex++] = slotIndex;

            ++slotIndex;
            availabilityMask >>= 1;
        }
    }

    private static void TryHackPlayer(PLAYER player, Byte category)
    {
        switch ((CharacterExtraPresetId)player.info.menu_type)
        {
            case CharacterExtraPresetId.StageZidane:
                player.info.menu_type = (Byte)CharacterPresetId.Zidane;
                break;
            case CharacterExtraPresetId.StageCinna:
                player.info.menu_type = (Byte)CharacterPresetId.Cinna2;
                break;
            case CharacterExtraPresetId.StageMarcus:
                player.info.menu_type = (Byte)CharacterPresetId.Marcus2;
                break;
            case CharacterExtraPresetId.StageBlank:
                player.info.menu_type = (Byte)CharacterPresetId.Blank2;
                break;
        }

        switch ((CharacterPresetId)player.info.menu_type)
        {
            case CharacterPresetId.Cinna1:
                player.info.menu_type = (Byte)CharacterPresetId.Cinna2;
                break;
            case CharacterPresetId.Marcus1:
                player.info.menu_type = (Byte)CharacterPresetId.Marcus2;
                break;
            case CharacterPresetId.Blank1:
                player.info.menu_type = (Byte)CharacterPresetId.Blank2;
                break;
        }

        if (player.category == category)
            return;

        player.category = category;
        switch ((CharacterPresetId)player.info.menu_type)
        {
            case CharacterPresetId.Quina:
                player.info.menu_type = (Byte)CharacterPresetId.Cinna2;
                break;
            case CharacterPresetId.Eiko:
                player.info.menu_type = (Byte)CharacterPresetId.Marcus2;
                break;
            case CharacterPresetId.Amarant:
                player.info.menu_type = (Byte)CharacterPresetId.Blank2;
                break;

            case CharacterPresetId.Cinna2:
                player.info.menu_type = (Byte)CharacterPresetId.Quina;
                break;
            case CharacterPresetId.Marcus2:
                player.info.menu_type = (Byte)CharacterPresetId.Eiko;
                break;
            case CharacterPresetId.Blank2:
                player.info.menu_type = (Byte)CharacterPresetId.Amarant;
                break;
        }

        Int32 equipId = ff9play.FF9Play_GetCharID3(player);
        ff9play.FF9Play_Change(player.info.slot_no, true, equipId);
    }
}