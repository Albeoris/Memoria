using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Prime;
using Memoria.Scenes;
using System;
using UnityEngine;

public class QuitUI : MonoBehaviour
{
    public const String WarningMenuGroupButton = "Quit.Warning";

    public static Boolean AcceptQuit = true;

    public GameObject WarningDialog;
    public Boolean isShowQuitUI;

    private String previousActiveGroup;
    private Boolean previousPlayerControlEnable;
    private Boolean previousMenuControlEnable;
    private Boolean previousEventEnable;
    private Boolean previousDisablePrimaryKey;
    private Single previousTimescale;
    private Single previousVibLeft;
    private Single previousVibRight;
    private Action onFinishHideQuitUI;

    public void Show(Action resumeCallback)
    {
        if (isShowQuitUI)
            return;

        isShowQuitUI = true;
        gameObject.SetActive(true);
        DisplayWindowBackground(gameObject);
        onFinishHideQuitUI = resumeCallback;
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
            PersistenSingleton<UIManager>.Instance.Dialogs.PauseAllDialog(true);
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
            return;
        isShowQuitUI = false;
        gameObject.SetActive(false);
        if (String.IsNullOrEmpty(previousActiveGroup))
            ButtonGroupState.DisableAllGroup();
        else
            ButtonGroupState.ActiveGroup = previousActiveGroup;
        FF9StateSystem.Settings.StartGameTime = Time.time;
        PersistenSingleton<UIManager>.Instance.SetEventEnable(previousEventEnable);
        PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(previousMenuControlEnable);
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(previousPlayerControlEnable, null);
        PersistenSingleton<HonoInputManager>.Instance.DisablePrimaryKey = previousDisablePrimaryKey;
        if (PersistenSingleton<UIManager>.Instance.Dialogs != null)
            PersistenSingleton<UIManager>.Instance.Dialogs.PauseAllDialog(false);
        if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.QuadMistBattle)
            QuadMistGame.main.Resume();
        else if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.EndGame || PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.Title)
            Time.timeScale = previousTimescale;
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
        String text = ButtonGroupState.ActiveGroup;
        if (text.Equals(WarningMenuGroupButton))
            text = String.Empty;
        previousActiveGroup = text;
    }

    public void onPress(GameObject go, Boolean isDown)
    {
        if (isDown)
            return;

        Int32 currentTouchID = UICamera.currentTouchID;
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
                    ButtonGroupState buttonInGroup = go.GetComponent<ButtonGroupState>();
                    if (!buttonInGroup || buttonInGroup.ProcessTouch())
                        OnKeyConfirm(go);
                }
                break;
        }
    }

    public void onClick(GameObject go)
    {
        onPress(go, false);
    }

    public Boolean OnKeyConfirm(GameObject go)
    {
        if (ButtonGroupState.ActiveGroup == WarningMenuGroupButton)
        {
            Int32 buttonId = go.transform.GetSiblingIndex();
            if (buttonId == 2)
                UIManager.Input.ConfirmQuit();
            else if (buttonId == 3)
                Hide();
        }
        return true;
    }

    public Boolean OnKeyCancel(GameObject go)
    {
        Hide();
        return true;
    }

    public void DisplayWindowBackground(GameObject go, UIAtlas forceColor = null)
    {
        UIAtlas atlas = forceColor == null ? (UIAtlas)(UnityEngine.Object)FF9UIDataTool.WindowAtlas : forceColor;
        foreach (UISprite sprite in go.GetComponentsInChildren<UISprite>(true))
            if (sprite.gameObject.tag == "Window Color" && sprite.atlas != FF9UIDataTool.GeneralAtlas && sprite.atlas != FF9UIDataTool.IconAtlas && sprite.atlas != atlas)
                sprite.atlas = atlas;
    }

    public void Awake()
    {
        UIScene.SetupYesNoLabels(WarningDialog.GetChild(0).GetChild(2), WarningDialog.GetChild(0).GetChild(3), onClick);
    }
}
