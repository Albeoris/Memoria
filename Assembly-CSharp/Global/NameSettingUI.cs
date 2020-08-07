using System;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;
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
    private static readonly Int32[] _idChar;
    private static readonly Byte[] _slot;

    public Int32 SubNo
    {
        get { return _subNumber; }
        set { _subNumber = value; }
    }

    static NameSettingUI()
    {
        _idChar = new Int32[12] { 0, 1, 2, 3, 4, 5, 6, 7, 11, 8, 9, 10 };
        _slot = new Byte[12] { 0, 1, 2, 3, 4, 5, 6, 7, 5, 6, 7, 8 };
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(afterFinished);
        GetID();
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
        NameInputField.value = FF9TextTool.CharacterDefaultName(_currentCharId);
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

    private void GetID()
    {
        _isDefaultName = _subNumber < 12;
        _currentCharId = FF9Name_EventToChar(!_isDefaultName ? _subNumber - 12 : _subNumber);
        _currentSlotId = FF9Name_CharToSlot(_currentCharId);
    }

    private void SetData()
    {
		String[] nameSpriteInfo;
        Background.sprite2D = AssetManager.Load<Sprite>("EmbeddedAsset/UI/Sprites/" + GetBackgroundSpritePath(), out nameSpriteInfo, false);
        MaxCharacterLabel.text = Localization.Get("MaxCharacters") + (Application.platform != RuntimePlatform.WindowsPlayer ? String.Empty : Localization.Get("MaxCharacters2"));
        CharacterProfile.text = FF9TextTool.CharacterProfile(_currentCharId);
        NameInputField.value = _isDefaultName
                                   ? FF9TextTool.CharacterDefaultName(_currentCharId)
                                   : FF9StateSystem.Common.FF9.player[_currentSlotId].name;
    }

    private String GetBackgroundSpritePath()
    {
        switch (_currentCharId)
        {
            case 0:
                return "name00";
            case 1:
                return "name01";
            case 2:
                return "name02";
            case 3:
                return "name03";
            case 4:
                return "name06";
            case 5:
                return "name04";
            case 6:
                return "name05";
            case 7:
                return "name07";
            default:
                return String.Empty;
        }
    }

    public void SetCharacterName()
    {
        FF9StateSystem.Common.FF9.player[_currentSlotId].name = NameInputField.value;
        Hide(() => PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.FieldHUD));
    }

    private Char ValidateInput(String text, Int32 charIndex, Char addedChar)
    {
        if (Char.IsLetter(addedChar))
            return addedChar;
        if (Regex.IsMatch(addedChar.ToString(), "[^\\u3041-\\u3096\\u30A0-\\u30FF\\u3400-\\u4DB5\\u4E00-\\u9FCB\\uF900-\\uFA6A\\u0021-\\u007E\\u00C0-\\u00FF\\uFF41-\\uFF5A]"))
            return Char.MinValue;
        return addedChar;
    }

    public static  Int32 FF9Name_EventToChar(Int32 eventID)
    {
        return _idChar[eventID];
    }

    public static  Int32 FF9Name_CharToSlot(Int32 charID)
    {
        return _slot[charID];
    }

    private void Awake()
    {
        FadingComponent = ScreenFadeGameObject.GetComponent<HonoFading>();
    }

    private void Start()
    {
        NameInputField.onValidate = ValidateInput;
    }
}