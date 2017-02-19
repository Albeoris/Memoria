using System;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;

public class QuitUI : MonoBehaviour
{
    public static String WarningMenuGroupButton = "Quit.Warning";
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
        if (String.IsNullOrEmpty(previousActiveGroup))
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
        String text = ButtonGroupState.ActiveGroup;
        if (text.Equals(WarningMenuGroupButton))
        {
            text = String.Empty;
        }
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

    public void onClick(GameObject go)
    {
        onPress(go, false);
    }

    public Boolean OnKeyConfirm(GameObject go)
    {
        if (ButtonGroupState.ActiveGroup == WarningMenuGroupButton)
        {
            Int32 siblingIndex = go.transform.GetSiblingIndex();
            Int32 num = siblingIndex;
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

    public Boolean OnKeyCancel(GameObject go)
    {
        Hide();
        return true;
    }

    public void DisplayWindowBackground(GameObject go, UIAtlas forceColor = null)
    {
        UIAtlas uIAtlas = (!(forceColor != null)) ? (UIAtlas)(UnityEngine.Object)Assets.Sources.Scripts.UI.Common.FF9UIDataTool.WindowAtlas : forceColor;
        UISprite[] componentsInChildren = go.GetComponentsInChildren<UISprite>(true);
        UISprite[] array = componentsInChildren;
        for (Int32 i = 0; i < array.Length; i++)
        {
            UISprite uISprite = array[i];
            GameObject obj = uISprite.gameObject;
            if (obj.tag == "Window Color" && uISprite.atlas != Assets.Sources.Scripts.UI.Common.FF9UIDataTool.GeneralAtlas && uISprite.atlas != Assets.Sources.Scripts.UI.Common.FF9UIDataTool.IconAtlas && uISprite.atlas != uIAtlas)
                uISprite.atlas = uIAtlas;
        }
    }

    public void Awake()
    {
        UIEventListener.Get(WarningDialog.GetChild(0).GetChild(2)).Click += onClick;
        UIEventListener.Get(WarningDialog.GetChild(0).GetChild(3)).Click += onClick;
    }
}