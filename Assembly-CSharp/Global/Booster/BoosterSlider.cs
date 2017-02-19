using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Memoria.Assets;
using UnityEngine;
using Object = System.Object;

public class BoosterSlider : MonoBehaviour
{
	public GameObject BoosterSliderPanel
	{
		get
		{
			return this.boosterSliderPanel;
		}
	}

	public Boolean IsSliderActive
	{
		get
		{
			return this.isSliderActive;
		}
	}

	private void onClick(GameObject go)
	{
		if (this.canSlideBooster())
		{
			if (go == this.OutsideBoosterHitPoint)
			{
				this.CloseBoosterPanel(delegate
				{
				});
			}
			else if (go == this.ArrowHitPoint)
			{
				this.OpenBoosterPanel();
			}
			else
			{
				BoosterSlider.BoosterButton boosterButton = this.boosterList.FirstOrDefault((BoosterSlider.BoosterButton button) => button.Self == go);
				if (boosterButton.BoosterType == BoosterType.BattleAssistance && (FF9StateSystem.Battle.isNoBoosterMap() || (Int32)FF9StateSystem.Battle.FF9Battle.btl_escape_fade != 32) && SceneDirector.IsBattleScene())
				{
					this.SetBoosterButton(BoosterType.BattleAssistance, false);
					return;
				}
				Boolean flag = false;
				if (boosterButton.Toggle.Count<UIToggle>() > 0)
				{
					flag = boosterButton.Toggle[0].value;
					this.SetBoosterHudIcon(boosterButton.BoosterType, flag);
				}
				switch (boosterButton.BoosterType)
				{
				case BoosterType.BattleAssistance:
				case BoosterType.HighSpeedMode:
				case BoosterType.Rotation:
				case BoosterType.Attack9999:
				case BoosterType.NoRandomEncounter:
				case BoosterType.Perspective:
					FF9StateSystem.Settings.CallBoosterButtonFuntion(boosterButton.BoosterType, flag);
					break;
				case BoosterType.Help:
					FF9StateSystem.Settings.CallBoosterButtonFuntion(boosterButton.BoosterType, true);
					break;
				}
			}
		}
	}

	private void onDrag(GameObject go, Vector2 delta)
	{
		if (this.canSlideBooster() && go == this.ArrowHitPoint)
		{
			this.OpenBoosterPanel();
		}
	}

	private void UICameraOnDragStart(GameObject go)
	{
		if (this.canSlideBooster())
		{
			this.startDraggedPos = UICamera.currentTouch.pos;
		}
	}

	private void UICameraOnDrag(GameObject go, Vector2 delta)
	{
		if (this.canSlideBooster())
		{
			if (!this.waitForDrag)
			{
				return;
			}
			if (this.startDraggedPos.y < this.screenButtommostPos || this.startDraggedPos.y > this.screenTopmostPos)
			{
				return;
			}
			if (this.startDraggedPos.x > this.screenLeftmostPos - this.touchRange && this.startDraggedPos.x < this.screenLeftmostPos + this.touchRange && delta.x > 0f)
			{
				this.waitForDrag = false;
				this.arrowHitPointTween.StopAnimation();
				this.arrowHitPointTween.TweenIn(new Byte[1], new UIScene.SceneVoidDelegate(this.AfterTweenIn));
				this.isArrowHitPointActive = true;
			}
		}
	}

	private void UICameraOnPress(GameObject go, Boolean isPress)
	{
		if (this.canSlideBooster() && isPress && this.isArrowHitPointActive && go != this.ArrowHitPoint)
		{
			this.isArrowHitPointActive = false;
			this.arrowHitPointTween.StopAnimation();
			this.arrowHitPointTween.TweenOut(new Byte[1], delegate
			{
				this.waitForDrag = true;
			});
		}
	}

	private Boolean canSlideBooster()
	{
		Boolean flag = !MBG.IsNull && !MBG.Instance.IsFinished();
		return (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.FieldHUD && !flag) || PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.BattleHUD || PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.WorldHUD || (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.Pause && !flag);
	}

	private void AfterTweenIn()
	{
		base.StopCoroutine("WaitAndHideArrow");
		base.StartCoroutine("WaitAndHideArrow");
	}

	private IEnumerator WaitAndHideArrow()
	{
		yield return new WaitForSeconds(2f);
		this.isArrowHitPointActive = false;
		this.arrowHitPointTween.TweenOut(new Byte[1], delegate
		{
			this.waitForDrag = true;
		});
		yield break;
	}

	private void OpenBoosterPanel()
	{
		this.ArrowHitPoint.SetActive(false);
		this.isArrowHitPointActive = false;
		this.waitForDrag = false;
		this.isSliderActive = true;
		this.boosterTween.TweenIn(new Byte[1], (UIScene.SceneVoidDelegate)null);
		base.StopCoroutine("WaitAndHideArrow");
		this.OutsideBoosterHitPoint.SetActive(true);
	}

	public void OpenBoosterPanelImmediately()
	{
		if (this.canSlideBooster())
		{
			this.animateHide = false;
			this.OutsideBoosterHitPoint.SetActive(false);
			base.StopCoroutine("WaitAndHideArrow");
			this.boosterTween.StopAnimation();
			this.ClearBooster();
			if (PersistenSingleton<UIManager>.Instance.UnityScene != UIManager.Scene.Battle)
			{
				this.boosterList[4].Self.SetActive(true);
				Int32[] buttonId = new Int32[]
				{
					0,
					1,
					3,
					4
				};
				this.GetButtonCurrentStatus(buttonId);
			}
			else
			{
				Int32[] buttonId2 = new Int32[]
				{
					0,
					1,
					3
				};
				this.GetButtonCurrentStatus(buttonId2);
			}
			this.ArrowHitPoint.SetActive(false);
			this.isArrowHitPointActive = false;
			this.waitForDrag = false;
			this.isSliderActive = true;
			if (!this.boosterSliderPanel.activeSelf)
			{
				this.boosterSliderPanel.SetActive(true);
			}
			this.boosterSliderPanel.transform.localPosition = this.boosterTween.DestinationPosition[0];
		}
	}

	public void CloseBoosterPanelImmediately()
	{
		if (this.isSliderActive)
		{
			if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Field || PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Battle)
			{
				this.SetBoosterState(PersistenSingleton<UIManager>.Instance.UnityScene);
			}
			else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.World)
			{
				this.SetBoosterState(UIManager.World.CurrentCharacterStateIndex);
			}
			this.OutsideBoosterHitPoint.SetActive(false);
			this.waitForDrag = true;
			this.isSliderActive = false;
			this.boosterSliderPanel.transform.localPosition = this.boosterTween.animatedInStartPosition;
		}
	}

	private void CloseBoosterPanel(UIScene.SceneVoidDelegate action)
	{
		if (this.animateHide)
		{
			return;
		}
		this.animateHide = true;
		this.isSliderActive = false;
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			this.animateHide = false;
			this.waitForDrag = true;
			this.OutsideBoosterHitPoint.SetActive(false);
		};
		if (action != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, action);
		}
		this.boosterTween.StopAnimation();
		this.boosterTween.TweenOut(new Byte[1], sceneVoidDelegate);
	}

	public void CloseBoosterPanel()
	{
		if (this.isSliderActive)
		{
			this.CloseBoosterPanel((UIScene.SceneVoidDelegate)null);
		}
	}

	public void SetBoosterButton(BoosterType type, Boolean isActive)
	{
		UIToggle[] toggle = this.boosterList[(Int32)type].Toggle;
		for (Int32 i = 0; i < (Int32)toggle.Length; i++)
		{
			UIToggle uitoggle = toggle[i];
			uitoggle.value = isActive;
		}
	}

	public void SetBoosterHudIcon(BoosterType type, Boolean isActive)
	{
		PersistenSingleton<OverlayCanvas>.Instance.overlayBoosterUI.SetBoosterIcon(type, isActive);
	}

	public void SetBoosterState(UIManager.Scene unityScene)
	{
		this.ClearBooster();
		switch (unityScene)
		{
		case UIManager.Scene.Field:
		{
			this.boosterList[6].Self.SetActive(true);
			this.boosterList[4].Self.SetActive(true);
			this.currentThirdButton = this.boosterList[6].Self;
			Int32[] buttonId = new Int32[]
			{
				0,
				1,
				3,
				4
			};
			this.GetButtonCurrentStatus(buttonId);
			break;
		}
		case UIManager.Scene.Battle:
		{
			this.boosterList[6].Self.SetActive(true);
			this.currentThirdButton = this.boosterList[6].Self;
			Int32[] buttonId = new Int32[]
			{
				0,
				1,
				3
			};
			this.GetButtonCurrentStatus(buttonId);
			break;
		}
		}
	}

	public void SetBoosterState(Int32 currentCharacterStateIndex)
	{
		this.ClearBooster();
		switch (currentCharacterStateIndex)
		{
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		{
			this.boosterList[4].Self.SetActive(true);
			this.boosterList[2].Self.SetActive(false);
			this.boosterList[5].Self.SetActive(false);
			this.currentThirdButton = this.boosterList[2].Self;
			this.boosterList[2].Icon.spriteName = "button_rotate";
			this.boosterList[2].IconToggle.spriteName = "button_rotate_act";
			Int32[] buttonId = new Int32[]
			{
				0,
				1,
				2,
				3,
				4,
				5
			};
			this.GetButtonCurrentStatus(buttonId);
			break;
		}
		case 6:
		{
			this.boosterList[4].Self.SetActive(true);
			this.boosterList[2].Self.SetActive(false);
			this.currentThirdButton = this.boosterList[2].Self;
			this.boosterList[2].Icon.spriteName = "button_align";
			this.boosterList[2].IconToggle.spriteName = "button_align_act";
			Int32[] buttonId = new Int32[]
			{
				0,
				1,
				2,
				3,
				4
			};
			this.GetButtonCurrentStatus(buttonId);
			break;
		}
		case 7:
		{
			this.boosterList[4].Self.SetActive(true);
			this.boosterList[2].Self.SetActive(false);
			this.boosterList[5].Self.SetActive(false);
			this.currentThirdButton = this.boosterList[2].Self;
			this.boosterList[2].Icon.spriteName = "button_align";
			this.boosterList[2].IconToggle.spriteName = "button_align_act";
			Int32[] buttonId = new Int32[]
			{
				0,
				1,
				2,
				3,
				4,
				5
			};
			this.GetButtonCurrentStatus(buttonId);
			break;
		}
		case 8:
		case 9:
		{
			this.boosterList[4].Self.SetActive(true);
			this.boosterList[2].Self.SetActive(false);
			this.currentThirdButton = this.boosterList[2].Self;
			this.boosterList[2].Icon.spriteName = "button_align";
			this.boosterList[2].IconToggle.spriteName = "button_align_act";
			Int32[] buttonId = new Int32[]
			{
				0,
				1,
				2,
				3,
				4
			};
			this.GetButtonCurrentStatus(buttonId);
			break;
		}
		}
	}

	public void ShowWaringDialog(BoosterType type, Action callback = null)
	{
		if (this.needComfirmType == BoosterType.None && (PersistenSingleton<UIManager>.Instance.IsPlayerControlEnable || PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.Config) && EventHUD.CurrentHUD == MinigameHUD.None)
		{
			this.needComfirmType = type;
			String text = String.Empty;
			switch (type)
			{
			case BoosterType.MasterSkill:
				text += Localization.Get("BoosterWarningMaster");
				break;
			case BoosterType.LvMax:
				text += Localization.Get("BoosterWarningLvMax");
				break;
			case BoosterType.GilMax:
				text += Localization.Get("BoosterWarningGilMax");
				break;
			}
			if (FF9StateSystem.aaaaPlatform)
			{
				text += Localization.Get("BoosterWarningaaaa");
			}
			else
			{
				text += Localization.Get("BoosterWarningNotaaaa");
			}
			text += Localization.Get("BoosterWarningCommonChoice");
			ETb.sChoose = 1;
			Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(text, 0, 0, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, new Vector2(0f, 0f), Dialog.CaptionType.None);
			ButtonGroupState.SetPointerOffsetToGroup(new Vector2(620f, 0f), Dialog.DialogGroupButton);
			dialog.AfterDialogHidden = new Dialog.DialogIntDelegate(this.OnConfirmDialogHidden);
			this.warningCallback = callback;
			if (PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.Config)
			{
				PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (Action)null);
				PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
				PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
			}
			PersistenSingleton<UIManager>.Instance.IsWarningDialogEnable = true;
			if (this.OutsideBoosterHitPoint.activeSelf)
			{
				this.CloseBoosterPanel((UIScene.SceneVoidDelegate)null);
			}
		}
		else
		{
			FF9Sfx.FF9SFX_Play(102);
		}
	}

	public void OnConfirmDialogHidden(Int32 choice)
	{
		ButtonGroupState.SetPointerOffsetToGroup(Dialog.DefaultOffset, Dialog.DialogGroupButton);
		if (choice == 0)
		{
			FF9StateSystem.Settings.CallBoosterButtonFuntion(this.needComfirmType, true);
			PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(this.needComfirmType, true);
		}
		this.needComfirmType = BoosterType.None;
		if (PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.Config)
		{
			PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(PersistenSingleton<EventEngine>.Instance.GetUserControl(), (Action)null);
			PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(PersistenSingleton<EventEngine>.Instance.GetUserControl());
			PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
		}
		PersistenSingleton<UIManager>.Instance.IsWarningDialogEnable = false;
		if (this.warningCallback != null)
		{
			this.warningCallback();
		}
	}

	private void ClearBooster()
	{
		if (this.currentThirdButton != (UnityEngine.Object)null)
		{
			this.currentThirdButton.SetActive(false);
		}
		this.boosterList[4].Self.SetActive(false);
		this.boosterList[5].Self.SetActive(false);
	}

	private void GetButtonCurrentStatus(Int32[] buttonId)
	{
		for (Int32 i = 0; i < (Int32)buttonId.Length; i++)
		{
			Int32 num = buttonId[i];
			Boolean flag = FF9StateSystem.Settings.IsBoosterButtonActive[num];
			UIToggle[] toggle = this.boosterList[num].Toggle;
			for (Int32 j = 0; j < (Int32)toggle.Length; j++)
			{
				UIToggle uitoggle = toggle[j];
				uitoggle.value = flag;
			}
			this.SetBoosterHudIcon((BoosterType)num, flag);
		}
	}

	public void Initial()
	{
		if (!this.mStart)
		{
			this.screenLeftmostPos = UIManager.UIPillarBoxSize.x;
			this.screenButtommostPos = UIManager.UILetterBoxSize;
			this.screenTopmostPos = (Single)Screen.height - UIManager.UILetterBoxSize;
			this.touchRange = (Single)(((Single)Screen.width / (Single)Screen.height - 1.333f >= 0.0004f) ? 100 : 150);
			this.mStart = true;
		}
	}

	private void Awake()
	{
		foreach (Object obj in this.BoosterListPanel.transform)
		{
			Transform transform = (Transform)obj;
			BoosterSlider.BoosterButton boosterButton = new BoosterSlider.BoosterButton(transform.gameObject);
			boosterButton.BoosterType = (BoosterType)transform.transform.GetSiblingIndex();
			this.boosterList.Add(boosterButton);
			if (FF9StateSystem.MobileAndaaaaPlatform)
			{
				UIEventListener uieventListener = UIEventListener.Get(transform.gameObject);
				uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.onClick));
			}
		}
		this.boosterSliderPanel = this.BoosterListPanel.GetParent();
		this.arrowHitPointTween = this.TransitionPanel.GetChild(0).GetComponent<HonoTweenPosition>();
		this.boosterTween = this.TransitionPanel.GetChild(1).GetComponent<HonoTweenPosition>();
	}

	public GameObject BoosterListPanel;

	public GameObject TransitionPanel;

	public GameObject ArrowHitPoint;

	public GameObject OutsideBoosterHitPoint;

	private GameObject currentThirdButton;

	private List<BoosterSlider.BoosterButton> boosterList = new List<BoosterSlider.BoosterButton>();

	private UITable boosterHudTable;

	private HonoTweenPosition boosterTween;

	private HonoTweenPosition arrowHitPointTween;

	private GameObject boosterSliderPanel;

	private Boolean isArrowHitPointActive;

	private Boolean waitForDrag = true;

	private Boolean animateHide;

	private Boolean isSliderActive;

	private Vector2 startDraggedPos;

	private Single screenLeftmostPos;

	private Single screenTopmostPos;

	private Single screenButtommostPos;

	private BoosterType needComfirmType = BoosterType.None;

	private Single touchRange = 100f;

	private Boolean mStart;

	private Action warningCallback;

	public enum BoosterIcon
	{
		HighSpeedMode,
		BattleAssistance,
		Attack9999,
		NoRandomEncounter,
		MasterSkill,
		None
	}

	public class BoosterButton
	{
		public BoosterButton(GameObject go)
		{
			this.Self = go;
			this.Toggle = go.GetComponents<UIToggle>();
			this.Icon = go.GetChild(0).GetComponent<UISprite>();
			this.IconToggle = go.GetChild(1).GetComponent<UISprite>();
		}

		public GameObject Self;

		public BoosterType BoosterType;

		public UIToggle[] Toggle;

		public UISprite Icon;

		public UISprite IconToggle;
	}
}
