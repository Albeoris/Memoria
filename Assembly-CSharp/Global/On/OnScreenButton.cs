using System;
using UnityEngine;

public class OnScreenButton : MonoBehaviour
{
    private Boolean IgnoreAutoBattleButton(SourceControl inputSource)
    {
        return FF9StateSystem.AndroidTVPlatform && FF9StateSystem.EnableAndroidTVJoystickMode && inputSource == SourceControl.Joystick && this.KeyCommand == Control.RightTrigger && PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.BattleHUD;
    }

    public Boolean IsForceSetHighlightKey
    {
        get
        {
            return this.isForceSetHighlightKey;
        }
        set
        {
            this.isForceSetHighlightKey = value;
        }
    }

    private void Awake()
    {
        if (this.AlwaysShow)
        {
            base.gameObject.SetActive(true);
            this.buttonGroupState = base.gameObject.GetComponent<ButtonGroupState>();
            this.buttonList = base.gameObject.GetComponents<UIButton>();
        }
        else if (FF9StateSystem.MobilePlatform)
        {
            base.gameObject.SetActive(true);
            this.buttonGroupState = base.gameObject.GetComponent<ButtonGroupState>();
            this.buttonList = base.gameObject.GetComponents<UIButton>();
        }
        else
        {
            base.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (this.KeyCommand != Control.None)
        {
            this.UpdateHightlightKeyCommand();
            this.processJoyStick();
        }
    }

    protected virtual void OnPress(Boolean isDown)
    {
        if (this.KeyCommand != Control.None && UIKeyTrigger.IsOnlyTouchAndLeftClick())
        {
            if (isDown)
            {
                UIManager.Input.SendKeyCode(this.KeyCommand, false);
                PersistenSingleton<HonoInputManager>.Instance.SetInputDownSources(SourceControl.Touch, this.KeyCommand);
                Singleton<DialogManager>.Instance.PressMesId = PersistenSingleton<UIManager>.Instance.Dialogs.CurMesId;
            }
            else
            {
                this.isPress = false;
                Boolean flag = false;
                if (UICamera.currentTouchID == -1)
                {
                    flag = true;
                }
                else if (UICamera.currentTouchID > -1 && UICamera.currentTouchID < 2)
                {
                    flag = (!(this.buttonGroupState != (UnityEngine.Object)null) || this.buttonGroupState.ProcessTouch());
                }
                if (flag)
                {
                    UICamera.Notify(PersistenSingleton<UIManager>.Instance.gameObject, "OnScreenButtonPressed", base.gameObject);
                }
                UIManager.Input.ResetKeyCode();
                Singleton<DialogManager>.Instance.ReleaseMesId = PersistenSingleton<UIManager>.Instance.Dialogs.CurMesId;
            }
        }
    }

    private void SetButtonState(UIButtonColor.State state)
    {
        UIButton[] array = this.buttonList;
        for (Int32 i = 0; i < (Int32)array.Length; i++)
        {
            UIButton uibutton = array[i];
            uibutton.SetState(state, false);
        }
    }

    private void processJoyStick()
    {
        if (this.buttonList != null)
        {
            SourceControl sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetSource(this.KeyCommand);
            switch (this.KeyCommand)
            {
                case Control.Up:
                case Control.Down:
                case Control.Left:
                case Control.Right:
                    if (sourceControl == SourceControl.None)
                        sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetDirectionAxisSource();
                    break;
            }
            if (this.IgnoreAutoBattleButton(sourceControl))
                return;
            Control index = sourceControl == SourceControl.Touch ? this.KeyCommand : this.highlightKeyCommand;
            if (PersistenSingleton<HonoInputManager>.Instance.IsInputUp(index))
                this.SetButtonState(UIButtonColor.State.Normal);
            if (PersistenSingleton<UIManager>.Instance.IsLoading || PersistenSingleton<UIManager>.Instance.IsPause)
                return;
            if (PersistenSingleton<HonoInputManager>.Instance.IsInputDown(index))
                this.SetButtonState(UIButtonColor.State.Pressed);
        }
    }

    private void UpdateHightlightKeyCommand()
    {
        if (!this.isForceSetHighlightKey && this.highlightKeyCommand != this.KeyCommand)
        {
            this.highlightKeyCommand = this.KeyCommand;
        }
    }

    public void SetHighlightKeyCommand(Control key)
    {
        this.highlightKeyCommand = key;
        this.isForceSetHighlightKey = true;
    }

    public Control KeyCommand = Control.None;

    public Boolean AlwaysShow;

    public Boolean PressToTrigger;

    private UIButton[] buttonList = new UIButton[0];

    private ButtonGroupState buttonGroupState;

    private Boolean isPress;

    [SerializeField]
    private Control highlightKeyCommand = Control.None;

    [SerializeField]
    private Boolean isForceSetHighlightKey;
}
