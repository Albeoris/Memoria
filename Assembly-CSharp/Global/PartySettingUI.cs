using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using UnityEngine;
using Object = System.Object;

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
            if (Configuration.Graphics.WidescreenSupport)
            {
                ButtonGroupState.SetPointerOffsetToGroup(new Vector2(150, -18f), MoveCharGroupButton);
            }
            else
            {
                ButtonGroupState.SetPointerOffsetToGroup(new Vector2(18f, -18f), MoveCharGroupButton);
            }

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
        FF9UIDataTool.DisplayCharacterAvatar(player, default(Vector3), default(Vector3), avatarSprite, false);
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
        CharacterPresetId presetId = player.PresetId;

        if (presetId == CharacterPresetId.StageZidane)
            player.PresetId = presetId = CharacterPresetId.Zidane;
        else if (presetId == CharacterPresetId.Cinna1 || presetId == CharacterPresetId.StageCinna)
            player.PresetId = presetId = CharacterPresetId.Cinna2;
        else if (presetId == CharacterPresetId.Marcus1 || presetId == CharacterPresetId.StageMarcus)
            player.PresetId = presetId = CharacterPresetId.Marcus2;
        else if (presetId == CharacterPresetId.Blank1 || presetId == CharacterPresetId.StageBlank)
            player.PresetId = presetId = CharacterPresetId.Blank2;

        if (player.category == category)
            return;

        player.category = category;
        if (presetId == CharacterPresetId.Quina)
            player.PresetId = CharacterPresetId.Cinna2;
        else if (presetId == CharacterPresetId.Eiko)
            player.PresetId = CharacterPresetId.Marcus2;
        else if (presetId == CharacterPresetId.Amarant)
            player.PresetId = CharacterPresetId.Blank2;
        else if (presetId == CharacterPresetId.Cinna2)
            player.PresetId = CharacterPresetId.Quina;
        else if (presetId == CharacterPresetId.Marcus2)
            player.PresetId = CharacterPresetId.Eiko;
        else if (presetId == CharacterPresetId.Blank2)
            player.PresetId = CharacterPresetId.Amarant;

        CharacterId equipId = ff9play.FF9Play_GetCharID2(player.Index, player.IsSubCharacter);
        ff9play.FF9Play_Change(player.info.slot_no, true, equipId);
    }
}