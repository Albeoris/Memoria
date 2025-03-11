using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;
using Memoria.Data;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable RedundantExplicitArraySize

public sealed class NameSettingUI : UIScene
{
    public UI2DSprite Background;
    public UILabel CharacterProfile;
    public UILabel MaxCharacterLabel;
    public UISprite NameHeader;
    public UIInput NameInputField;
    public GameObject ResetButton;
    public GameObject ConfirmButton;
    public GameObject Warning;
    public GameObject ScreenFadeGameObject;
    private Boolean _isDefaultName;
    private Int32 _subNumber;
    private Int32 _currentCharId;
    private Int32 _currentSlotId;

    public static Boolean IsDefaultName(CharacterId subNo)
    {
        return (Int32)subNo < 12;
    }

    public CharacterId SubNo
    {
        get => (CharacterId)_subNumber;
        set => InitID((Int32)value);
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(afterFinished);
        SetData();
        NameInputField.isSelected = true;
        Warning.SetActive(false);
        PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(true);
        PersistenSingleton<HonoInputManager>.Instance.DisablePrimaryKey = true;
    }

    public override void Hide(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate action = () => PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(PersistenSingleton<UIManager>.Instance.IsMenuControlEnable);
        if (afterFinished != null)
            action = (SceneVoidDelegate)Delegate.Combine(action, afterFinished);
        base.Hide(action);
        PersistenSingleton<HonoInputManager>.Instance.DisablePrimaryKey = false;
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (!base.OnKeyConfirm(go))
            return true;

        if (NameInputField.isSelected)
            NameInputField.RemoveFocus();
        else
            OnConfirmButtonClick();

        return true;
    }

    public override Boolean OnKeyMenu(GameObject go)
    {
        if (base.OnKeyMenu(go))
            OnResetButtonClick();

        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go))
            OnFocusTextFieldButtonClick();

        return true;
    }

    public void OnResetButtonClick()
    {
        FF9Sfx.FF9SFX_Play(103);
        NameInputField.value = FF9TextTool.CharacterDefaultName(SubNo);
    }

    public void OnConfirmButtonClick()
    {
        NameInputField.value = NameInputField.value.Trim();
        if (NameInputField.value == String.Empty)
        {
            FF9Sfx.FF9SFX_Play(102);
            Warning.SetActive(true);
        }
        else
        {
            FF9Sfx.FF9SFX_Play(103);
            SetCharacterName();
        }
    }

    public void OnFocusTextFieldButtonClick()
    {
        StartCoroutine(DelayFocusTextField());
    }

    [DebuggerHidden]
    private IEnumerator DelayFocusTextField()
    {
        yield return new WaitForSeconds(0.5f);
        if (!NameInputField.isSelected)
            NameInputField.isSelected = true;
    }

    private void InitID(Int32 id)
    {
        _isDefaultName = id < 12;
        if (_isDefaultName)
            _subNumber = id;
        else
            _subNumber = id - 12;
        _currentCharId = _subNumber;
        _currentSlotId = _subNumber;
    }

    private void SetData()
    {
        Background.sprite2D = AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/" + GetBackgroundSpritePath(), false);
        MaxCharacterLabel.rawText = Localization.Get("MaxCharacters") + (Application.platform != RuntimePlatform.WindowsPlayer ? String.Empty : Localization.Get("MaxCharacters2"));
        CharacterProfile.rawText = FF9TextTool.CharacterProfile(_subNumber);
        NameInputField.value = _isDefaultName ? FF9TextTool.CharacterDefaultName(SubNo) : FF9StateSystem.Common.FF9.GetPlayer(SubNo).Name;
    }

    private String GetBackgroundSpritePath()
    {
        if (SubNo == CharacterId.Freya)
            return "name06";
        if (SubNo == CharacterId.Quina)
            return "name04";
        if (SubNo == CharacterId.Eiko)
            return "name05";
        return $"name{_subNumber:D2}";
    }

    public void SetCharacterName()
    {
        FF9StateSystem.Common.FF9.GetPlayer(SubNo).Name = NameInputField.value.Trim();
        Hide(() => PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.FieldHUD));
    }

    private Char ValidateInput(String text, Int32 charIndex, Char addedChar)
    {
        if (Char.IsLetter(addedChar) || (addedChar == ' ' && charIndex > 0))
            return addedChar;
        // Might want to add a couple of allowed characters (Myanmar? things like that, that are not recognised with IsLetter...)
        UnicodeBIDI.CharacterClass unicodeClass = UnicodeBIDI.GetBIDIClass(addedChar);
        if (unicodeClass == UnicodeBIDI.CharacterClass.Arabic_Letter || unicodeClass == UnicodeBIDI.CharacterClass.Arabic_Number)
            return addedChar;
        if (Regex.IsMatch(addedChar.ToString(), "[^\\u3041-\\u3096\\u30A0-\\u30FF\\u3400-\\u4DB5\\u4E00-\\u9FCB\\uF900-\\uFA6A\\u0021-\\u007E\\u00C0-\\u00FF\\uFF41-\\uFF5A]"))
            return '\0';
        return addedChar;
    }

    private void Awake()
    {
        FadingComponent = ScreenFadeGameObject.GetComponent<HonoFading>();
        NameInputField.characterLimit = 12;
    }

    private void Start()
    {
        NameInputField.onValidate = ValidateInput;
    }
}
