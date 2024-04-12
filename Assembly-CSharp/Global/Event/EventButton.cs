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
		switch (this.KeyCommand)
		{
			case Control.Confirm:
				EventInput.ReceiveInput(EventInput.Lcircle | EventInput.Pcircle);
				break;
			case Control.Cancel:
				EventInput.ReceiveInput(EventInput.Lx | EventInput.Px);
				break;
			case Control.Menu:
				EventInput.ReceiveInput(EventInput.Ltriangle | EventInput.Ptriangle);
				break;
			case Control.Special:
				EventInput.ReceiveInput(EventInput.Lsquare | EventInput.Psquare);
				break;
			case Control.LeftBumper:
				EventInput.ReceiveInput(EventInput.LL1 | EventInput.PL1);
				break;
			case Control.RightBumper:
				EventInput.ReceiveInput(EventInput.LR1 | EventInput.PR1);
				break;
			case Control.LeftTrigger:
				EventInput.ReceiveInput(EventInput.LL2 | EventInput.PL2);
				break;
			case Control.RightTrigger:
				EventInput.ReceiveInput(EventInput.LR2 | EventInput.PR2);
				break;
			case Control.Pause:
				EventInput.ReceiveInput(EventInput.Pstart);
				break;
			case Control.Select:
				EventInput.ReceiveInput(EventInput.Pselect);
				break;
			case Control.Up:
				EventInput.ReceiveInput(EventInput.Pup);
				break;
			case Control.Down:
				EventInput.ReceiveInput(EventInput.Pdown);
				break;
			case Control.Left:
				EventInput.ReceiveInput(EventInput.Pleft);
				break;
			case Control.Right:
				EventInput.ReceiveInput(EventInput.Pright);
				break;
		}
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
