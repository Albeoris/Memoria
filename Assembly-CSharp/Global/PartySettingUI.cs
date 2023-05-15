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
        public GameObject Highlight;
        public UISprite Avatar;
        public UISprite[] StatusesAvatar;

        public CharacterOutsidePartyHud(GameObject go)
        {
            this.Self = go;
            this.MoveButton = go.GetChild(0);
            this.Highlight = go.GetChild(1);
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
    private CharacterId currentCharacterId;
    private FF9PARTY_INFO info;

    [NonSerialized]
    private Int32 currentFloor;
    private Int32 FloorMax => this.info == null || this.info.select == null || this.info.select.Length <= 8 ? 0 : (this.info.select.Length - 5) / 4;

    public FF9PARTY_INFO Info
    {
        get { return this.info; }
        set { this.info = value; }
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        currentFloor = 0;
        TryHackPartyInfo(this.info);
        ExtendSelectSlots();

        SceneVoidDelegate sceneVoidDelegate = delegate
        {
            if (Configuration.Graphics.WidescreenSupport)
            {
                ButtonGroupState.SetPointerOffsetToGroup(new Vector2(150f, -18f), MoveCharGroupButton);
                foreach (CharacterOutsidePartyHud charHud in this.outsidePartyHudList)
                    charHud.Highlight.transform.localScale = new Vector3(0.45f, 1f, 1f);
            }
            else
            {
                ButtonGroupState.SetPointerOffsetToGroup(new Vector2(18f, -18f), MoveCharGroupButton);
                foreach (CharacterOutsidePartyHud charHud in this.outsidePartyHudList)
                    charHud.Highlight.transform.localScale = new Vector3(1f, 1f, 1f);
            }

            ButtonGroupState.SetPointerDepthToGroup(6, MoveCharGroupButton);
            ButtonGroupState.ActiveGroup = SelectCharGroupButton;
        };
        if (afterFinished != null)
            sceneVoidDelegate = (SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(sceneVoidDelegate);
        this.DisplayCharacters();
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
            sceneVoidDelegate = (SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        base.Hide(sceneVoidDelegate);
        BattleAchievement.UpdateParty();
        this.RemoveCursorMemorize();
    }

    private void RemoveCursorMemorize()
    {
        ButtonGroupState.RemoveCursorMemorize(SelectCharGroupButton);
        ButtonGroupState.RemoveCursorMemorize(MoveCharGroupButton);
    }

    public override GameObject OnKeyNavigate(KeyCode direction, GameObject currentObj, GameObject nextObj)
    {
        if (nextObj != null || currentObj == null)
            return nextObj;
        Boolean canChangeFloor = false;
        if (ButtonGroupState.ActiveGroup == SelectCharGroupButton)
            canChangeFloor = currentObj.transform.parent.parent.parent == this.OutsidePartyPanel.transform;
        else if (ButtonGroupState.ActiveGroup == MoveCharGroupButton)
            canChangeFloor = currentObj.transform.parent.parent.parent.parent == this.OutsidePartyPanel.transform && this.currentCharacterSelect.Group == Mode.Menu;
        if (!canChangeFloor)
            return null;
        if (direction == KeyCode.DownArrow && currentFloor < FloorMax)
		{
            currentFloor++;
            DisplayOutsideCharacters();
            OnItemSelect(currentObj);
            FF9Sfx.FF9SFX_Play(103);
        }
        else if (direction == KeyCode.UpArrow && currentFloor > 0)
        {
            currentFloor--;
            DisplayOutsideCharacters();
            OnItemSelect(currentObj);
            FF9Sfx.FF9SFX_Play(103);
        }
        return null;
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (!base.OnKeyConfirm(go))
            return true;
        
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
                ButtonGroupState.SetButtonEnable(current.MoveButton, this.currentCharacterId == CharacterId.NONE || !this.info.fix.Contains(this.currentCharacterId));
        }
        else if (ButtonGroupState.ActiveGroup == MoveCharGroupButton)
        {
            PartySelect currentSelect = this.GetCurrentSelect(go);
            CharacterId currentId = this.GetCurrentId(go);
            if (this.currentCharacterSelect.Group == Mode.Select && currentId != CharacterId.NONE && this.info.fix.Contains(currentId))
            {
                FF9Sfx.FF9SFX_Play(102);
            }
            else
            {
                FF9Sfx.FF9SFX_Play(103);
                this.SwapCharacter(this.currentCharacterSelect, currentSelect);
                this.DisplayCharacters();
                this.DisplayCharacterInfo(this.currentCharacterId);
                ButtonGroupState.SetCursorMemorize(go.transform.parent.gameObject, SelectCharGroupButton);
                ButtonGroupState.ActiveGroup = SelectCharGroupButton;
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
                    for (Int32 i = 0; i < 4; i++)
                        ff9play.FF9Play_SetParty(i, this.info.menu[i]);
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
                CharacterId currentId = this.GetCurrentId(go);
                if (this.currentCharacterId != currentId)
                {
                    this.currentCharacterId = currentId;
                    this.DisplayCharacterInfo(this.currentCharacterId);
                }
            }
            else if (ButtonGroupState.ActiveGroup == MoveCharGroupButton)
            {
                CharacterId currentId = this.GetCurrentId(go);
                this.DisplayCharacterInfo(currentId);
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

    private void DisplayCharacters()
    {
        Int32 hudIndex = 0;
        CharacterId[] menu = this.info.menu;
        for (Int32 i = 0; i < menu.Length; i++)
        {
            CharacterDetailPartyHUD characterDetailPartyHUD = this.currentPartyHudList[hudIndex++];
            if (menu[i] != CharacterId.NONE)
            {
                PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(menu[i]);
                characterDetailPartyHUD.Self.SetActive(true);
                characterDetailPartyHUD.Content.SetActive(true);
                characterDetailPartyHUD.EmptyLabel.gameObject.SetActive(false);
                FF9UIDataTool.DisplayCharacterDetail(player, characterDetailPartyHUD);
                this.DisplayCharacterPartyAvatar(menu[i], player, (UISprite)(Object)characterDetailPartyHUD.AvatarSprite, characterDetailPartyHUD.StatusesAvatar);
            }
            else
            {
                characterDetailPartyHUD.EmptyLabel.text = String.Format(Localization.Get("EmptyCharNumber"), hudIndex);
                characterDetailPartyHUD.Content.SetActive(false);
                characterDetailPartyHUD.EmptyLabel.gameObject.SetActive(true);
            }
        }
        DisplayOutsideCharacters();
    }

    private void DisplayOutsideCharacters()
    {
        for (Int32 i = 0; i < 8; i++)
        {
            CharacterOutsidePartyHud characterOutsidePartyHud = this.outsidePartyHudList[i];
            CharacterId select = this.info.select[4 * currentFloor + i];
            if (select != CharacterId.NONE)
                this.DisplayCharacterPartyAvatar(select, FF9StateSystem.Common.FF9.GetPlayer(select), characterOutsidePartyHud.Avatar, characterOutsidePartyHud.StatusesAvatar);
            else
                characterOutsidePartyHud.Avatar.alpha = 0f;
        }
    }

    private void DisplayCharacterInfo(CharacterId charId)
    {
        if (charId != CharacterId.NONE)
        {
            PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(charId);
            this.currentCharacterHud.Content.SetActive(true);
            FF9UIDataTool.DisplayCharacterDetail(player, this.currentCharacterHud);
            this.DisplayCharacterPartyAvatar(charId, player, (UISprite)(Object)this.currentCharacterHud.AvatarSprite, this.currentCharacterHud.StatusesAvatar);
        }
        else
        {
            this.currentCharacterHud.Content.SetActive(false);
        }
    }

    private void DisplayCharacterPartyAvatar(CharacterId id, PLAYER player, UISprite avatarSprite, UISprite[] avatarStatusAlignment)
    {
        FF9UIDataTool.DisplayCharacterAvatar(player, default(Vector3), default(Vector3), avatarSprite, false);
        avatarSprite.alpha = this.info.fix.Contains(id) ? 0.5f : 1f;
        for (Int32 i = 0; i < avatarStatusAlignment.Length; i++)
        {
            UISprite uISprite = avatarStatusAlignment[i];
            uISprite.alpha = 0f;
        }
        Int32 statusSlot = 0;
        if ((player.status & BattleStatus.Venom) != 0)
        {
            avatarStatusAlignment[statusSlot].spriteName = Localization.Get("PartyStatusTextPoison");
            avatarStatusAlignment[statusSlot].alpha = 1f;
            statusSlot++;
        }
        if ((player.status & BattleStatus.Petrify) != 0)
        {
            avatarStatusAlignment[statusSlot].spriteName = Localization.Get("PartyStatusTextStone");
            avatarStatusAlignment[statusSlot].alpha = 1f;
            statusSlot++;
        }
        if (player.cur.hp == 0)
        {
            avatarStatusAlignment[statusSlot].spriteName = "text_ko";
            avatarStatusAlignment[statusSlot].alpha = 1f;
            statusSlot++;
        }
    }

    private Boolean FF9Party_Check()
    {
        Int32 healthyCnt = 0;
        Int32 charCnt = 0;
        for (Int32 i = 0; i < 4;i++)
        {
            if (this.info.menu[i] != CharacterId.NONE)
            {
                charCnt++;
                PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(this.info.menu[i]);
                if (player.cur.hp != 0 && (player.status & BattleStatusConst.BattleEndInMenu) == 0)
                    healthyCnt++;
            }
        }
        return charCnt >= this.info.party_ct && healthyCnt != 0;
    }

    private CharacterId GetCurrentId(GameObject go)
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
            return this.info.menu[siblingIndex];
        return this.info.select[4 * currentFloor + siblingIndex];
    }

    private PartySelect GetCurrentSelect(GameObject go)
    {
        if (ButtonGroupState.ActiveGroup == SelectCharGroupButton)
            return new PartySelect((go.transform.parent.parent != this.CurrentPartyPanel.transform) ? Mode.Select : Mode.Menu, go.transform.GetSiblingIndex());
        return new PartySelect((go.transform.parent.parent.parent != this.CurrentPartyPanel.transform) ? Mode.Select : Mode.Menu, go.transform.parent.GetSiblingIndex());
    }

    private void SwapCharacter(PartySelect oldSelect, PartySelect newSelect)
    {
        CharacterId char1 = this.currentCharacterId;
        CharacterId char2 = CharacterId.NONE;
        if (newSelect.Group == Mode.Menu)
        {
            char2 = this.info.menu[newSelect.Index];
            this.info.menu[newSelect.Index] = char1;
        }
        else if (newSelect.Group == Mode.Select)
        {
            char2 = this.info.select[4 * currentFloor + newSelect.Index];
            this.info.select[4 * currentFloor + newSelect.Index] = char1;
        }
        if (oldSelect.Group == Mode.Menu)
            this.info.menu[oldSelect.Index] = char2;
        else if (oldSelect.Group == Mode.Select)
            this.info.select[4 * currentFloor + oldSelect.Index] = char2;
    }

    private void Awake()
    {
        FadingComponent = this.ScreenFadeGameObject.GetComponent<HonoFading>();
        this.helpLabel = this.HelpInfoPanel.GetChild(0).GetComponent<UILabel>();
        foreach (Transform transf in this.CurrentPartyPanel.GetChild(0).transform)
        {
            GameObject go = transf.gameObject;
            UIEventListener.Get(go).onClick += new UIEventListener.VoidDelegate(this.onClick);
            UIEventListener.Get(go.GetChild(2)).onClick += new UIEventListener.VoidDelegate(this.onClick);
            this.currentPartyHudList.Add(new CharacterDetailPartyHUD(go, true));
        }
        foreach (Transform transf in this.OutsidePartyPanel.GetChild(1).GetChild(0).transform)
        {
            GameObject go = transf.gameObject;
            UIEventListener.Get(go).onClick += new UIEventListener.VoidDelegate(this.onClick);
            UIEventListener.Get(go.GetChild(0)).onClick += new UIEventListener.VoidDelegate(this.onClick);
            this.outsidePartyHudList.Add(new CharacterOutsidePartyHud(go));
        }
        this.currentCharacterHud = new CharacterDetailPartyHUD(this.OutsidePartyPanel.GetChild(0), false);
    }

    private void ExtendSelectSlots()
	{
        Int32 oldCount;
        if (this.info.select == null)
        {
            oldCount = 0;
            this.info.select = new CharacterId[8];
        }
        else
		{
            oldCount = this.info.select.Length;
        }
        Int32 count = Math.Max(8, oldCount);
        if ((count % 4) != 0)
            count += 4 - (count % 4);
        if (count != oldCount)
        {
            Array.Resize(ref this.info.select, count);
            for (Int32 i = oldCount; i < count; i++)
                this.info.select[i] = CharacterId.NONE;
        }
    }

    private static void TryHackPartyInfo(FF9PARTY_INFO partyInfo)
    {
        if (Configuration.Hacks.AllCharactersAvailable < 1)
            return;

        List<CharacterId> selectList = new List<CharacterId>();
        foreach (PLAYER p in FF9StateSystem.Common.FF9.PlayerList)
            selectList.Add(p.info.slot_no);

        for (Int32 memberIndex = 0; memberIndex < 4; ++memberIndex)
        {
            PLAYER member = FF9StateSystem.Common.FF9.party.member[memberIndex];
            if (member != null)
            {
                partyInfo.menu[memberIndex] = member.info.slot_no;
                selectList.Remove(member.info.slot_no);
            }
            else
            {
                partyInfo.menu[memberIndex] = CharacterId.NONE;
            }
        }

        partyInfo.select = selectList.ToArray();
        partyInfo.party_ct = 1;
    }
}