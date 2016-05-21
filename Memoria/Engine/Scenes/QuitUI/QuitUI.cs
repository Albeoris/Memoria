using System;
using Memoria;
using UnityEngine;

#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

// ReSharper disable ArrangeThisQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
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
// ReSharper disable InconsistentNaming

[ExportedType("wįńĠńńńń/!!!ĭÉ)ĐĽ0ßħ¡,ĊĚYę7Kn0ù£yĭđīáû*Ä¹÷q;]ñVf.Öĝ;EËĻćXi¾jàµ_ĥ-!!!»Ú·»Ńī*SCBġ´V]Ļ¢ıã#¿zeĺĹ¹Åġú´¨ÌtlÈÏþÈÆ_ĪĢńĩSńńńń")]
public class QuitUI : MonoBehaviour
{
    public static string WarningMenuGroupButton = "Quit.Warning";
    public static bool AcceptQuit = true;
    public GameObject WarningDialog;
    public bool isShowQuitUI;

    private string previousActiveGroup;
    private bool previousPlayerControlEnable;
    private bool previousMenuControlEnable;
    private bool previousEventEnable;
    private bool previousDisablePrimaryKey;
    private float previousTimescale;
    private float previousVibLeft;
    private float previousVibRight;
    private Action onFinishHideQuitUI;

    public void Show(Action onFinishHideQuitUICallback)
    {
        if (isShowQuitUI)
            return;

        isShowQuitUI = true;
        gameObject.SetActive(true);
        DisplayWindowBackground(gameObject);
        onFinishHideQuitUI = onFinishHideQuitUICallback;
        previousPlayerControlEnable = PersistenSingleton<UIManager>.Instance.IsPlayerControlEnable;
        previousMenuControlEnable = PersistenSingleton<UIManager>.Instance.IsMenuControlEnable;
        previousEventEnable = PersistenSingleton<UIManager>.Instance.IsEventEnable;
        previousDisablePrimaryKey = PersistenSingleton<HonoInputManager>.Instance.DisablePrimaryKey;
        PersistenSingleton<HonoInputManager>.Instance.DisablePrimaryKey = false;
        FF9StateSystem.Settings.UpdateTickTime();
        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(-30f, 10f), WarningMenuGroupButton);
        ButtonGroupState.SetPointerDepthToGroup(107, WarningMenuGroupButton);
        ButtonGroupState.RemoveCursorMemorize(WarningMenuGroupButton);
        ButtonGroupState.ActiveGroup = WarningMenuGroupButton;
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);
        PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
        PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
        if (PersistenSingleton<UIManager>.Instance.Dialogs != null)
        {
            PersistenSingleton<UIManager>.Instance.Dialogs.PauseAllDialog(true);
        }
        if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.QuadMistBattle)
        {
            QuadMistGame.main.Pause();
        }
        else if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.EndGame || PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.Title)
        {
            previousTimescale = Time.timeScale;
            Time.timeScale = 0f;
        }
        previousVibLeft = vib.CurrentVibrateLeft;
        previousVibRight = vib.CurrentVibrateRight;
        vib.VIB_actuatorReset(0);
        vib.VIB_actuatorReset(1);
    }

    public void Hide()
    {
        if (!isShowQuitUI)
        {
            return;
        }
        isShowQuitUI = false;
        gameObject.SetActive(false);
        if (string.IsNullOrEmpty(previousActiveGroup))
        {
            ButtonGroupState.DisableAllGroup();
        }
        else
        {
            ButtonGroupState.ActiveGroup = previousActiveGroup;
        }
        FF9StateSystem.Settings.StartGameTime = Time.time;
        PersistenSingleton<UIManager>.Instance.SetEventEnable(previousEventEnable);
        PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(previousMenuControlEnable);
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(previousPlayerControlEnable, null);
        PersistenSingleton<HonoInputManager>.Instance.DisablePrimaryKey = previousDisablePrimaryKey;
        if (PersistenSingleton<UIManager>.Instance.Dialogs != null)
        {
            PersistenSingleton<UIManager>.Instance.Dialogs.PauseAllDialog(false);
        }
        if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.QuadMistBattle)
        {
            QuadMistGame.main.Resume();
        }
        else if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.EndGame || PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.Title)
        {
            Time.timeScale = previousTimescale;
        }
        vib.VIB_actuatorSet(0, previousVibLeft, previousVibRight);
        vib.VIB_actuatorSet(1, previousVibLeft, previousVibRight);
        if (onFinishHideQuitUI != null)
        {
            onFinishHideQuitUI();
            onFinishHideQuitUI = null;
        }
    }

    public void SetPreviousActiveGroup()
    {
        string text = ButtonGroupState.ActiveGroup;
        if (text.Equals(WarningMenuGroupButton))
        {
            text = string.Empty;
        }
        previousActiveGroup = text;
    }

    public void onPress(GameObject go, bool isDown)
    {
        if (!isDown)
        {
            int currentTouchID = UICamera.currentTouchID;
            switch (currentTouchID + 2)
            {
                case 1:
                    OnKeyConfirm(go);
                    break;
                case 2:
                case 3:
                    if (ButtonGroupState.ContainButtonInSecondaryGroup(go))
                    {
                        OnKeyConfirm(go);
                    }
                    else
                    {
                        ButtonGroupState component = go.GetComponent<ButtonGroupState>();
                        if (component)
                        {
                            if (component.ProcessTouch())
                            {
                                OnKeyConfirm(go);
                            }
                        }
                        else
                        {
                            OnKeyConfirm(go);
                        }
                    }
                    break;
            }
        }
    }

    public void onClick(GameObject go)
    {
        onPress(go, false);
    }

    public bool OnKeyConfirm(GameObject go)
    {
        if (ButtonGroupState.ActiveGroup == WarningMenuGroupButton)
        {
            int siblingIndex = go.transform.GetSiblingIndex();
            int num = siblingIndex;
            if (num != 2)
            {
                if (num == 3)
                {
                    Hide();
                }
            }
            else
            {
                UIManager.Input.ConfirmQuit();
            }
        }
        return true;
    }

    public bool OnKeyCancel(GameObject go)
    {
        Hide();
        return true;
    }

    public void DisplayWindowBackground(GameObject go, UIAtlas forceColor = null)
    {
        UIAtlas uIAtlas = (!(forceColor != null)) ? (UIAtlas)(UnityEngine.Object)Assets.Sources.Scripts.UI.Common.FF9UIDataTool.WindowAtlas : forceColor;
        UISprite[] componentsInChildren = go.GetComponentsInChildren<UISprite>(true);
        UISprite[] array = componentsInChildren;
        for (int i = 0; i < array.Length; i++)
        {
            UISprite uISprite = array[i];
            GameObject obj = uISprite.gameObject;
            if (obj.tag == "Window Color" && uISprite.atlas != Assets.Sources.Scripts.UI.Common.FF9UIDataTool.GeneralAtlas && uISprite.atlas != Assets.Sources.Scripts.UI.Common.FF9UIDataTool.IconAtlas && uISprite.atlas != uIAtlas)
                uISprite.atlas = uIAtlas;
        }
    }

    public void Awake()
    {
        UIEventListener expr_17 = UIEventListener.Get(WarningDialog.GetChild(0).GetChild(2));
        expr_17.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_17.onClick, new UIEventListener.VoidDelegate(onClick));
        UIEventListener expr_4F = UIEventListener.Get(WarningDialog.GetChild(0).GetChild(3));
        expr_4F.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_4F.onClick, new UIEventListener.VoidDelegate(onClick));
    }
}