using System;
using UnityEngine;

public class EventButton : MonoBehaviour
{
    private void Start()
    {
        base.gameObject.SetActive(this.AlwaysShow || FF9StateSystem.MobilePlatform);
    }

    protected virtual void OnClick()
    {
        this.isClicked = true;
    }

    protected virtual void OnPress(Boolean isDown)
    {
        this.isPressed = isDown;
    }

    private Boolean CheckInput()
    {
        return FF9StateSystem.Settings.IsFastForward && EventHUD.CurrentHUD == MinigameHUD.RacingHippaul ? this.isClicked : this.isPressed;
    }

    private void CleanUp()
    {
        this.isClicked = false;
    }

    private void SendInputToEvent()
    {
        EventInput.ReceiveInput(EventInput.GetKeyMaskFromControl(this.KeyCommand));
    }

    private void Update()
    {
        if (this.CheckInput())
            this.SendInputToEvent();
        this.CleanUp();
    }

    public Control KeyCommand = Control.None;
    public Boolean AlwaysShow;

    private Boolean isPressed;
    private Boolean isClicked;
}
