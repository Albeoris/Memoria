using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Database;
using UnityEngine;

// ReSharper disable NotAccessedField.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ArrangeThisQualifier
// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

public class StatusUI : UIScene
{
    public GameObject TransitionGroup;
    public GameObject HelpDespLabelGameObject;
    public GameObject HitPointPanel;
    public GameObject CharacterDetailPanel;
    public GameObject CharacterArrowPanel;
    public GameObject ParameterDetailPanel;
    public GameObject ETCInfo;
    public GameObject EquipmentDetailPanel;
    public GameObject CommandDetailPanel;
    public GameObject ScreenFadeGameObject;
    public List<GameObject> AbilityPanelList;

    public Int32 CurrentPartyIndex
    {
        set { _currentPartyIndex = value; }
    }

    private Int32 _currentPartyIndex;
    private Byte _currentAbilityPanelAmount;
    private CharacterDetailHUD _characterHud;
    private ParameterDetailHUD _parameterHud;
    private EquipmentDetailHud _equipmentHud;
    private GameObject _tranceGameObject;
    private UISlider _tranceSlider;
    private UILabel _expLabel;
    private UILabel _nextLvLabel;
    private UILabel _attackLabel;
    private UILabel _ability1Label;
    private UILabel _ability2Label;
    private UILabel _itemLabel;
    private HonoTweenPosition _abilityPanelTransition;
    private HonoAvatarTweenPosition _avatarTransition;
    private readonly List<AbilityItemHUD> _abilityHudList;
    private readonly List<GameObject> _abilityCaptionList;
    private Boolean _fastSwitch;

    public StatusUI()
    {
        _abilityHudList = new List<AbilityItemHUD>();
        _abilityCaptionList = new List<GameObject>();
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        base.Show(() =>
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.SubMenuPanel.SetActive(false);
            afterFinished?.Invoke();
        });

        DisplayPlayerArrow(true);
        DisplayAllCharacterInfo(true);
        if (ButtonGroupState.HelpEnabled)
            DisplayHelp(false);

        HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);
    }

    public override void Hide(SceneVoidDelegate afterFinished = null)
    {
        base.Hide(afterFinished);
        if (_fastSwitch)
            return;
        PersistenSingleton<UIManager>.Instance.MainMenuScene.StartSubmenuTweenIn();
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (!base.OnKeyConfirm(go))
            return true;

        FF9Sfx.FF9SFX_Play(1047);
        if (_currentAbilityPanelAmount < 6)
        {
            if (_currentAbilityPanelAmount == 0)
                _abilityCaptionList[0].SetActive(true);
            _abilityPanelTransition.TweenIn(new[] { _currentAbilityPanelAmount }, () =>
            {
                Loading = false;
                for (Int32 index = 0; index < (Int32)_currentAbilityPanelAmount - 1; ++index)
                    _abilityCaptionList[index].SetActive(false);
                for (Int32 index = _currentAbilityPanelAmount; index < _abilityCaptionList.Count; ++index)
                    _abilityCaptionList[index].SetActive(true);
            });
            Loading = true;
            ++_currentAbilityPanelAmount;
        }
        else if (_currentAbilityPanelAmount == 6)
        {
            IEnumerable<Int32> source = Enumerable.Range(0, _currentAbilityPanelAmount);

            _abilityPanelTransition.TweenOut(
                source.Select(v => (Byte)v).ToArray(),
                () => Loading = false);

            Loading = true;
            _currentAbilityPanelAmount = 0;
        }

        DisplayPlayerArrow(_currentAbilityPanelAmount == 0);
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (!base.OnKeyCancel(go))
            return true;

        if (_currentAbilityPanelAmount > 0)
        {
            FF9Sfx.FF9SFX_Play(1047);
            IEnumerable<Int32> source = Enumerable.Range(0, _currentAbilityPanelAmount);

            _abilityPanelTransition.TweenOut(
                source.Select(v => (Byte)v).ToArray(),
                () => Loading = false);

            Loading = true;
            DisplayPlayerArrow(true);
            _currentAbilityPanelAmount = 0;
        }
        else
        {
            FF9Sfx.FF9SFX_Play(101);
            _fastSwitch = false;

            Hide(() =>
            {
                PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = false;
                PersistenSingleton<UIManager>.Instance.MainMenuScene.CurrentSubMenu = MainMenuUI.SubMenu.Status;
                PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.MainMenu);
            });
        }
        return true;
    }

    public override Boolean OnKeySelect(GameObject go)
    {
        if (!base.OnKeySelect(go))
            return true;

        ButtonGroupState.HelpEnabled = !ButtonGroupState.HelpEnabled;
        DisplayHelp(true);
        return true;
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        if (!base.OnKeyLeftBumper(go) || !CharacterArrowPanel.activeSelf)
            return true;

        FF9Sfx.FF9SFX_Play(1047);
        Int32 prev = ff9play.FF9Play_GetPrev(_currentPartyIndex);
        if (prev == _currentPartyIndex)
            return true;

        _currentPartyIndex = prev;

        PLAYER player = FF9StateSystem.Common.FF9.party.member[this._currentPartyIndex];
        String spritName = FF9UIDataTool.AvatarSpriteName(player.info.serial_no);
        Loading = true;
        Boolean isKnockOut = player.cur.hp == 0;

        _avatarTransition.Change(spritName, HonoAvatarTweenPosition.Direction.LeftToRight, isKnockOut, () =>
        {
            DisplayPlayer(true);
            Loading = false;
        });

        DisplayAllCharacterInfo(false);
        return true;
    }

    public override Boolean OnKeyRightBumper(GameObject go)
    {
        if (!base.OnKeyRightBumper(go) || !CharacterArrowPanel.activeSelf)
            return true;

        FF9Sfx.FF9SFX_Play(1047);
        Int32 next = ff9play.FF9Play_GetNext(_currentPartyIndex);
        if (next == _currentPartyIndex)
            return true;

        _currentPartyIndex = next;
        PLAYER player = FF9StateSystem.Common.FF9.party.member[this._currentPartyIndex];
        String spritName = FF9UIDataTool.AvatarSpriteName(player.info.serial_no);
        Loading = true;
        Boolean isKnockOut = player.cur.hp == 0;

        _avatarTransition.Change(spritName, HonoAvatarTweenPosition.Direction.RightToLeft, isKnockOut, () =>
        {
            DisplayPlayer(true);
            Loading = false;
        });

        DisplayAllCharacterInfo(false);
        return true;
    }

    private void DisplayHelp(Boolean isPlaySE)
    {
        if (ButtonGroupState.HelpEnabled)
        {
            Singleton<HelpDialog>.Instance.Phrase = Localization.Get("StatusDetailHelp");
            Singleton<HelpDialog>.Instance.PointerLimitRect = UIManager.UIScreenCoOrdinate;
            Singleton<HelpDialog>.Instance.Position = new Vector3(524f, -68f);
            Singleton<HelpDialog>.Instance.Tail = false;
            Singleton<HelpDialog>.Instance.Depth = 5;
            Singleton<HelpDialog>.Instance.ShowDialog();
            if (!isPlaySE)
                return;
            FF9Sfx.FF9SFX_Play(682);
        }
        else
        {
            Singleton<HelpDialog>.Instance.HideDialog();
            if (!isPlaySE)
                return;
            FF9Sfx.FF9SFX_Play(101);
        }
    }

    private void DisplayPlayerArrow(Boolean isEnable)
    {
        if (isEnable)
        {
            Int32 count = FF9StateSystem.Common.FF9.party.member.Count(player => player != null);
            CharacterArrowPanel.SetActive(count > 1);
        }
        else
            CharacterArrowPanel.SetActive(false);
    }

    private void DisplayAllCharacterInfo(Boolean updateAvatar)
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[_currentPartyIndex];
        DisplayPlayer(updateAvatar);
        _parameterHud.SpeedLabel.text = player.elem.dex.ToString();
        _parameterHud.StrengthLabel.text = player.elem.str.ToString();
        _parameterHud.MagicLabel.text = player.elem.mgc.ToString();
        _parameterHud.SpiritLabel.text = player.elem.wpr.ToString();
        _parameterHud.AttackLabel.text = ff9weap.WeaponData[player.equip[0]].Ref.Power.ToString();
        _parameterHud.DefendLabel.text = player.defence.PhisicalDefence.ToString();
        _parameterHud.EvadeLabel.text = player.defence.PhisicalEvade.ToString();
        _parameterHud.MagicDefLabel.text = player.defence.MagicalDefence.ToString();
        _parameterHud.MagicEvaLabel.text = player.defence.MagicalEvade.ToString();

        UInt32 exp = player.level < 99 ? ff9level.CharacterLevelUps[player.level].ExperienceToLevel : player.exp;
        if (FF9StateSystem.EventState.gEventGlobal[16] != 0 && (player.category & 16) == 0)
        {
            _tranceGameObject.SetActive(true);
            _tranceSlider.value = player.trance / 256f;
        }
        else
        {
            _tranceGameObject.SetActive(false);
        }

        _expLabel.text = player.exp.ToString();
        _nextLvLabel.text = (exp - player.exp).ToString();

        FF9UIDataTool.DisplayItem(player.equip[0], _equipmentHud.Weapon.IconSprite, _equipmentHud.Weapon.NameLabel, true);
        FF9UIDataTool.DisplayItem(player.equip[1], _equipmentHud.Head.IconSprite, _equipmentHud.Head.NameLabel, true);
        FF9UIDataTool.DisplayItem(player.equip[2], _equipmentHud.Wrist.IconSprite, _equipmentHud.Wrist.NameLabel, true);
        FF9UIDataTool.DisplayItem(player.equip[3], _equipmentHud.Body.IconSprite, _equipmentHud.Body.NameLabel, true);
        FF9UIDataTool.DisplayItem(player.equip[4], _equipmentHud.Accessory.IconSprite, _equipmentHud.Accessory.NameLabel, true);

        Byte presetId = (Byte)FF9StateSystem.Common.FF9.party.member[_currentPartyIndex].info.menu_type;
        Byte command1 = (Byte)CharacterCommands.CommandSets[presetId].Regular1;
        Byte command2 = (Byte)CharacterCommands.CommandSets[presetId].Regular2;
        _attackLabel.text = FF9TextTool.CommandName(1);
        _ability1Label.text = FF9TextTool.CommandName(command1);
        _ability2Label.text = FF9TextTool.CommandName(command2);
        _itemLabel.text = FF9TextTool.CommandName(14);

        for (Int32 index = 0; index < _abilityHudList.Count; ++index)
            DrawAbilityInfo(_abilityHudList[index], index);
    }

    private void DisplayPlayer(Boolean updateAvatar)
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[_currentPartyIndex];
        FF9UIDataTool.DisplayCharacterDetail(player, _characterHud);
        if (!updateAvatar)
            return;

        FF9UIDataTool.DisplayCharacterAvatar(player, new Vector3(), new Vector3(), _characterHud.AvatarSprite, false);
    }

    private void DrawAbilityInfo(AbilityItemHUD abilityHud, Int32 index)
    {
        PLAYER player = FF9StateSystem.Common.FF9.party.member[_currentPartyIndex];
        Boolean hasAP = ff9abil.FF9Abil_HasAp(new Character(player));
        if (index >= player.pa.Length || (hasAP && player.pa[index] == 0))
        {
            abilityHud.Self.SetActive(false);
            return;
        }

        Int32 abilId = ff9abil._FF9Abil_PaData[player.PresetId][index].Id;
        //int num1 = ff9abil._FF9Abil_PaData[player.info.menu_type][index].max_ap;
        if (abilId == 0)
        {
            abilityHud.Self.SetActive(false);
        }
        else
        {
            //int num2 = player.pa[index];
            String abilName;
            String stoneSprite;
            Boolean isShowText;
            if (abilId < 192)
            {
                AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[abilId];
                abilName = FF9TextTool.ActionAbilityName(abilId);
                stoneSprite = "ability_stone";
                isShowText = (aaData.Type & 2) == 0;
            }
            else
            {
                //SA_DATA saData = ff9abil._FF9Abil_SaData[index1 - 192];
                abilName = FF9TextTool.SupportAbilityName(abilId - 192);
                stoneSprite = !ff9abil.FF9Abil_IsEnableSA(player.sa, abilId) ? "skill_stone_off" : "skill_stone_on";
                isShowText = true;
            }
            abilityHud.Self.SetActive(true);
            abilityHud.IconSprite.spriteName = stoneSprite;
            abilityHud.NameLabel.text = abilName;
            FF9UIDataTool.DisplayAPBar(player, abilId, isShowText, abilityHud.APBar);
        }
    }

    protected void Awake()
    {
        FadingComponent = ScreenFadeGameObject.GetComponent<HonoFading>();
        _characterHud = new CharacterDetailHUD(CharacterDetailPanel, false);
        _parameterHud = new ParameterDetailHUD(ParameterDetailPanel);
        _equipmentHud = new EquipmentDetailHud(EquipmentDetailPanel);
        _tranceGameObject = ETCInfo.GetChild(0);
        _tranceSlider = ETCInfo.GetChild(0).GetChild(2).GetComponent<UISlider>();
        _expLabel = ETCInfo.GetChild(1).GetChild(2).GetComponent<UILabel>();
        _nextLvLabel = ETCInfo.GetChild(2).GetChild(3).GetComponent<UILabel>();
        _attackLabel = CommandDetailPanel.GetChild(0).GetComponent<UILabel>();
        _ability1Label = CommandDetailPanel.GetChild(1).GetComponent<UILabel>();
        _ability2Label = CommandDetailPanel.GetChild(2).GetComponent<UILabel>();
        _itemLabel = CommandDetailPanel.GetChild(3).GetComponent<UILabel>();
        for (Int32 index = 0; index < AbilityPanelList.Count; ++index)
        {
            for (Int32 childIndex = 0; childIndex < 8; ++childIndex)
            {
                //int num = index * 8 + childIndex;
                _abilityHudList.Add(new AbilityItemHUD(AbilityPanelList[index].GetChild(0).GetChild(childIndex)));
            }
            _abilityCaptionList.Add(AbilityPanelList[index].GetChild(1).GetChild(3));
        }
        _abilityPanelTransition = TransitionGroup.GetChild(0).GetComponent<HonoTweenPosition>();
        _avatarTransition = CharacterDetailPanel.GetChild(0).GetChild(6).GetChild(0).GetComponent<HonoAvatarTweenPosition>();

        this._characterHud.MagicStoneLabel.SetAnchor((Transform)null);
        this._characterHud.MagicStoneLabel.width = 60;
        this._characterHud.MagicStoneLabel.height = 52;
        this._parameterHud.SpeedLabel.SetAnchor((Transform)null);
        this._parameterHud.StrengthLabel.SetAnchor((Transform)null);
        this._parameterHud.MagicLabel.SetAnchor((Transform)null);
        this._parameterHud.SpiritLabel.SetAnchor((Transform)null);
        this._parameterHud.AttackLabel.SetAnchor((Transform)null);
        this._parameterHud.DefendLabel.SetAnchor((Transform)null);
        this._parameterHud.EvadeLabel.SetAnchor((Transform)null);
        this._parameterHud.MagicDefLabel.SetAnchor((Transform)null);
        this._parameterHud.MagicEvaLabel.SetAnchor((Transform)null);

        // Update 27.02.2017
        this._parameterHud.SpeedLabel.width = 90;
        this._parameterHud.StrengthLabel.width = 90;
        this._parameterHud.MagicLabel.width = 90;
        this._parameterHud.SpiritLabel.width = 90;
        this._parameterHud.AttackLabel.width = 90;
        this._parameterHud.DefendLabel.width = 90;
        this._parameterHud.EvadeLabel.width = 90;
        this._parameterHud.MagicDefLabel.width = 90;
        this._parameterHud.MagicEvaLabel.width = 90;
        this._parameterHud.SpeedLabel.height = 40;
        this._parameterHud.StrengthLabel.height = 40;
        this._parameterHud.MagicLabel.height = 40;
        this._parameterHud.SpiritLabel.height = 40;
        this._parameterHud.AttackLabel.height = 40;
        this._parameterHud.DefendLabel.height = 40;
        this._parameterHud.EvadeLabel.height = 40;
        this._parameterHud.MagicDefLabel.height = 40;
        this._parameterHud.MagicEvaLabel.height = 40;
    }

    public class ParameterDetailHUD
    {
        public GameObject Self;
        public UILabel SpeedLabel;
        public UILabel StrengthLabel;
        public UILabel MagicLabel;
        public UILabel SpiritLabel;
        public UILabel AttackLabel;
        public UILabel DefendLabel;
        public UILabel EvadeLabel;
        public UILabel MagicDefLabel;
        public UILabel MagicEvaLabel;

        public ParameterDetailHUD(GameObject go)
        {
            Self = go;
            SpeedLabel = go.GetChild(0).GetChild(2).GetComponent<UILabel>();
            StrengthLabel = go.GetChild(1).GetChild(2).GetComponent<UILabel>();
            MagicLabel = go.GetChild(2).GetChild(2).GetComponent<UILabel>();
            SpiritLabel = go.GetChild(3).GetChild(2).GetComponent<UILabel>();
            AttackLabel = go.GetChild(5).GetChild(2).GetComponent<UILabel>();
            DefendLabel = go.GetChild(6).GetChild(2).GetComponent<UILabel>();
            EvadeLabel = go.GetChild(7).GetChild(2).GetComponent<UILabel>();
            MagicDefLabel = go.GetChild(8).GetChild(2).GetComponent<UILabel>();
            MagicEvaLabel = go.GetChild(9).GetChild(2).GetComponent<UILabel>();
        }
    }
}