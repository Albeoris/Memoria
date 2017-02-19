using System;
using UnityEngine;

public class EventButton : MonoBehaviour
{
	private void Start()
	{
		if (this.AlwaysShow)
		{
			base.gameObject.SetActive(true);
		}
		else if (FF9StateSystem.MobilePlatform)
		{
			base.gameObject.SetActive(true);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
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
		Boolean result;
		if (FF9StateSystem.Settings.IsFastForward)
		{
			result = ((EventHUD.CurrentHUD != MinigameHUD.RacingHippaul) ? this.isPressed : this.isClicked);
		}
		else
		{
			result = this.isPressed;
		}
		return result;
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
			EventInput.ReceiveInput(EventInput.Lcircle | 8192u);
			break;
		case Control.Cancel:
			EventInput.ReceiveInput(16384u | EventInput.Lx);
			break;
		case Control.Menu:
			EventInput.ReceiveInput(16781312u);
			break;
		case Control.Special:
			EventInput.ReceiveInput(557056u);
			break;
		case Control.LeftBumper:
			EventInput.ReceiveInput(1049600u);
			break;
		case Control.RightBumper:
			EventInput.ReceiveInput(2099200u);
			break;
		case Control.LeftTrigger:
			EventInput.ReceiveInput(4194560u);
			break;
		case Control.RightTrigger:
			EventInput.ReceiveInput(8389120u);
			break;
		case Control.Pause:
			EventInput.ReceiveInput(8u);
			break;
		case Control.Select:
			EventInput.ReceiveInput(1u);
			break;
		case Control.Up:
			EventInput.ReceiveInput(16u);
			break;
		case Control.Down:
			EventInput.ReceiveInput(64u);
			break;
		case Control.Left:
			EventInput.ReceiveInput(128u);
			break;
		case Control.Right:
			EventInput.ReceiveInput(32u);
			break;
		}
	}

	private void Update()
	{
		if (this.CheckInput())
		{
			this.SendInputToEvent();
		}
		this.CleanUp();
	}

	public Control KeyCommand = Control.None;

	public Boolean AlwaysShow;

	private Boolean isPressed;

	private Boolean isClicked;
}
