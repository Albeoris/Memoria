using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;

public class EndingUI : UIScene
{
	public override void Show(UIScene.SceneVoidDelegate action = null)
	{
		SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
		base.Show(action);
		this.isReadyToBlackjack = false;
		this.isStartToBlackjack = false;
		this.blackjackKeyCodeCount = 0;
		SlideshowUI component = this.SlideShowObject.GetComponent<SlideshowUI>();
		component.SetupEndingText();
		component.SetupLastEndingText();
	}

	public override void Hide(UIScene.SceneVoidDelegate action = null)
	{
		base.Hide(action);
	}

	private void CheckBlackjackKey(Control keyCode)
	{
		if (!this.isReadyToBlackjack)
		{
			return;
		}
		if (this.blackjackKeyCodeCount < this.blackjackKeyCodeList.Count)
		{
			Control control = this.blackjackKeyCodeList[this.blackjackKeyCodeCount];
			if (control == keyCode)
			{
				this.blackjackKeyCodeCount++;
				if (this.blackjackKeyCodeCount == this.blackjackKeyCodeList.Count)
				{
					FF9Snd.ff9snd_sndeffect_play(103, 8388608, 127, 128);
					this.isStartToBlackjack = true;
				}
			}
			else
			{
				this.blackjackKeyCodeCount = 0;
			}
		}
		if (keyCode == Control.Pause)
		{
			if (this.isStartToBlackjack)
			{
				FF9Snd.ff9snd_sndeffect_play(3096, 8388608, 127, 128);
				SceneDirector.Replace("EndGame", SceneTransition.FadeOutToBlack_FadeIn, true);
				SceneDirector.ToggleFadeAll(false);
			}
			else
			{
				SceneDirector.Replace("Title", SceneTransition.FadeOutToBlack_FadeIn, true);
			}
		}
	}

	public override Boolean OnKeyLeftBumper(GameObject go)
	{
		this.CheckBlackjackKey(Control.LeftBumper);
		return base.OnKeyLeftBumper(go);
	}

	public override Boolean OnKeyLeftTrigger(GameObject go)
	{
		this.CheckBlackjackKey(Control.LeftTrigger);
		return base.OnKeyLeftTrigger(go);
	}

	public override Boolean OnKeyRightBumper(GameObject go)
	{
		this.CheckBlackjackKey(Control.RightBumper);
		return base.OnKeyRightBumper(go);
	}

	public override Boolean OnKeyRightTrigger(GameObject go)
	{
		this.CheckBlackjackKey(Control.RightTrigger);
		return base.OnKeyRightTrigger(go);
	}

	public override Boolean OnKeyConfirm(GameObject go)
	{
		if (base.OnKeyConfirm(go))
		{
			if (FF9StateSystem.MobilePlatform && this.isReadyToBlackjack)
			{
				SceneDirector.Replace("Title", SceneTransition.FadeOutToBlack_FadeIn, true);
				SceneDirector.ToggleFadeAll(false);
			}
			else
			{
				this.CheckBlackjackKey(Control.Confirm);
			}
		}
		return true;
	}

	public override Boolean OnKeyCancel(GameObject go)
	{
		this.CheckBlackjackKey(Control.Cancel);
		return base.OnKeyCancel(go);
	}

	public override Boolean OnKeyMenu(GameObject go)
	{
		this.CheckBlackjackKey(Control.Menu);
		return base.OnKeyMenu(go);
	}

	public override Boolean OnKeySpecial(GameObject go)
	{
		this.CheckBlackjackKey(Control.Special);
		return base.OnKeySpecial(go);
	}

	public override Boolean OnKeyPause(GameObject go)
	{
		this.CheckBlackjackKey(Control.Pause);
		return base.OnKeyPause(go);
	}

	public override Boolean OnKeySelect(GameObject go)
	{
		this.CheckBlackjackKey(Control.Select);
		return base.OnKeySelect(go);
	}

	private void OnKeyEnter(GameObject go, KeyCode key)
	{
		if (key == KeyCode.LeftArrow)
		{
			this.CheckBlackjackKey(Control.Left);
		}
		else if (key == KeyCode.DownArrow)
		{
			this.CheckBlackjackKey(Control.Down);
		}
		else if (key == KeyCode.RightArrow)
		{
			this.CheckBlackjackKey(Control.Right);
		}
		else if (key == KeyCode.UpArrow)
		{
			this.CheckBlackjackKey(Control.Up);
		}
	}

	public void ReadyToBlackjack()
	{
		this.isReadyToBlackjack = true;
		this.isStartToBlackjack = false;
		this.blackjackKeyCodeCount = 0;
	}

	private void Awake()
	{
		UIEventListener uieventListener = UIEventListener.Get(this.HitAreaObject);
		uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.onClick));
		UIEventListener uieventListener2 = UIEventListener.Get(this.HitAreaObject);
		uieventListener2.onNavigate = (UIEventListener.KeyCodeDelegate)Delegate.Combine(uieventListener2.onNavigate, new UIEventListener.KeyCodeDelegate(this.OnKeyEnter));
	}

	public GameObject HitAreaObject;

	public GameObject SlideShowObject;

	public SlideshowUI endingSlideshow;

	private List<Control> blackjackKeyCodeList = new List<Control>
	{
		Control.RightTrigger,
		Control.LeftBumper,
		Control.RightTrigger,
		Control.RightTrigger,
		Control.Up,
		Control.Cancel,
		Control.Right,
		Control.Confirm,
		Control.Down,
		Control.Menu,
		Control.LeftTrigger,
		Control.RightBumper,
		Control.RightTrigger,
		Control.LeftBumper,
		Control.Special,
		Control.Special
	};

	private Int32 blackjackKeyCodeCount;

	private Boolean isReadyToBlackjack;

	private Boolean isStartToBlackjack;

	private Boolean isActive = true;
}
