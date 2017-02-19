using System;
using System.Collections;
using Memoria.Assets;
using UnityEngine;

public class TutorialUI : UIScene
{
	public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
	{
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			if (!this.isFromPause)
			{
				switch (this.DisplayMode)
				{
				case TutorialUI.Mode.Battle:
					this.DisplayBattleTutorial();
					break;
				case TutorialUI.Mode.QuadMist:
					this.DisplayQuadmistTutorial();
					break;
				case TutorialUI.Mode.BasicControl:
					this.DisplayBasicControlTutorial();
					break;
				}
			}
			this.isFromPause = false;
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		base.Show(sceneVoidDelegate);
		if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Battle)
		{
			FF9StateSystem.Battle.isTutorial = true;
		}
	}

	public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
	{
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			if (!this.isFromPause)
			{
				if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Battle)
				{
					UIManager.UIState previousState = PersistenSingleton<UIManager>.Instance.PreviousState;
					if (previousState != UIManager.UIState.Config)
					{
						FF9StateSystem.Battle.isTutorial = false;
						UIManager.Battle.NextSceneIsModal = true;
						PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.BattleHUD);
					}
					else
					{
						PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Config);
					}
				}
				else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Field)
				{
					UIManager.UIState previousState = PersistenSingleton<UIManager>.Instance.PreviousState;
					if (previousState != UIManager.UIState.Config)
					{
						UIManager.Field.NextSceneIsModal = true;
						PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.FieldHUD);
					}
					else
					{
						PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Config);
					}
				}
				else if (PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.QuadMist)
				{
					if (this.QuadmistTutorialID == 1)
					{
						PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.QuadMist);
					}
					else
					{
						PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.QuadMistBattle);
					}
				}
				else
				{
					PersistenSingleton<UIManager>.Instance.ChangeUIState(PersistenSingleton<UIManager>.Instance.HUDState);
				}
			}
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		base.Hide(sceneVoidDelegate);
	}

	public override Boolean OnKeyConfirm(GameObject go)
	{
		if (base.OnKeyConfirm(go))
		{
			FF9Sfx.FF9SFX_Play(103);
			if (this.DisplayMode == TutorialUI.Mode.Battle)
			{
				this.OnOKButtonClick();
			}
		}
		return true;
	}

	public override Boolean OnKeyPause(GameObject go)
	{
		if (PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.Config || PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.QuadMistBattle || PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.QuadMist)
		{
			return false;
		}
		base.OnKeyPause(go);
		base.NextSceneIsModal = true;
		this.isFromPause = true;
		this.Hide(delegate
		{
			PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Pause);
		});
		return true;
	}

	private void Awake()
	{
		this.ContentPanel.transform.localScale = new Vector3(0f, 0f, 0f);
		this.battleTutorialImage1 = this.ContentPanel.GetChild(0).GetComponent<UISprite>();
		this.battleTutorialImage2 = this.ContentPanel.GetChild(1).GetComponent<UISprite>();
		this.battleTutorialDialogImage2 = this.ContentPanel.GetChild(1).GetChild(0).GetChild(0).GetComponent<UISprite>();
		this.battleLeftLabel = this.ContentPanel.GetChild(0).GetChild(1).GetComponent<UILabel>();
		this.battleRightLabel = this.ContentPanel.GetChild(1).GetChild(1).GetComponent<UILabel>();
		this.battleBottomLabel = this.ContentPanel.GetChild(3).GetComponent<UILabel>();
	}

	private void HideTutorial()
	{
		base.Loading = false;
		if (PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.Config)
		{
			this.DisplayMode = TutorialUI.Mode.Battle;
			if (this.AfterFinished != null)
			{
				this.AfterFinished();
			}
		}
	}

	private void AnimatePanel(Vector3 scale)
	{
		this.ContentPanel.GetParent().SetActive(true);
		EventDelegate.Add(TweenScale.Begin(this.ContentPanel, this.duration, scale).onFinished, new EventDelegate.Callback(this.AfterShowBattleTutorial));
	}

	public void OnOKButtonClick()
	{
		this.AnimatePanel(new Vector3(0f, 0f, 0f));
		base.StartCoroutine(this.WaitAndHide());
	}

	private IEnumerator WaitAndHide()
	{
		yield return new WaitForSeconds(this.duration);
		this.Hide(delegate
		{
			this.ContentPanel.GetParent().SetActive(false);
			this.HideTutorial();
		});
		yield break;
	}

	private void AfterShowBattleTutorial()
	{
		base.Loading = false;
	}

	private void DisplayBattleTutorial()
	{
		String str = (!(Localization.language == "Japanese")) ? String.Empty : "_jp";
		this.battleTutorialImage1.spriteName = ((!FF9StateSystem.MobilePlatform) ? ("tutorial_pc_01" + str) : ("tutorial_mobile_01" + str));
		this.battleTutorialImage2.spriteName = ((!FF9StateSystem.MobilePlatform) ? ("tutorial_pc_02" + str) : ("tutorial_mobile_02" + str));
		this.battleTutorialDialogImage2.spriteName = Localization.Get((!FF9StateSystem.MobilePlatform) ? "TutorialTapOnCharacterIconPC" : "TutorialTapOnCharacterIconMobile");
		this.battleLeftLabel.text = Localization.Get((!FF9StateSystem.MobilePlatform) ? "TutorialLeftParagraphPC" : "TutorialLeftParagraphMobile");
		this.battleRightLabel.text = Localization.Get((!FF9StateSystem.MobilePlatform) ? "TutorialRightParagraphPC" : "TutorialRightParagraphMobile");
		base.Loading = true;
		this.AnimatePanel(new Vector3(1f, 1f, 1f));
		if (FF9StateSystem.PCPlatform)
		{
			this.battleBottomLabel.gameObject.SetActive(false);
		}
		else
		{
			this.battleBottomLabel.gameObject.SetActive(true);
		}
	}

	private void DisplayQuadmistTutorial()
	{
		if (this.QuadmistTutorialID > 3)
		{
			return;
		}
		base.Loading = true;
		String key = "QuadMistTutorial" + this.QuadmistTutorialID;
		Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get(key), 0, 0, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.None);
		dialog.AfterDialogShown = delegate(Int32 choice)
		{
			base.Loading = false;
		};
		dialog.AfterDialogHidden = new Dialog.DialogIntDelegate(this.AfterHideQuadmistTutorial);
		TweenPosition component = dialog.GetComponent<TweenPosition>();
		if (component != (UnityEngine.Object)null)
		{
			component.enabled = false;
		}
	}

	private void AfterHideQuadmistTutorial(Int32 choice)
	{
		this.Hide(delegate
		{
			this.HideTutorial();
		});
	}

	private void DisplayBasicControlTutorial()
	{
		base.Loading = true;
		String arg;
		if (FF9StateSystem.MobilePlatform)
		{
			if (FF9StateSystem.AndroidTVPlatform && PersistenSingleton<HonoInputManager>.Instance.IsControllerConnect)
			{
				arg = ((this.BasicControlTutorialID <= 0) ? "AndroidTV" : "PC");
				this.lastPage = 2;
			}
			else
			{
				arg = "Mobile";
				this.lastPage = 1;
			}
		}
		else
		{
			arg = "PC";
			this.lastPage = 2;
		}
		if (this.BasicControlTutorialID > this.lastPage)
		{
			return;
		}
		String key = arg + "BasicControlTutorial" + this.BasicControlTutorialID;
		Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get(key), 0, 0, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.None);
		dialog.AfterDialogShown = delegate(Int32 choice)
		{
			base.Loading = false;
		};
		dialog.AfterDialogHidden = new Dialog.DialogIntDelegate(this.AfterHideBasicControlTutorial);
	}

	private void AfterHideBasicControlTutorial(Int32 choice)
	{
		if (this.BasicControlTutorialID < this.lastPage)
		{
			this.BasicControlTutorialID++;
			this.DisplayBasicControlTutorial();
		}
		else
		{
			this.Hide(delegate
			{
				this.HideTutorial();
			});
		}
	}

	private const String QuadMistLocalizeKey = "QuadMistTutorial";

	private const String BasicControlLocalizeKey = "BasicControlTutorial";

	public GameObject ContentPanel;

	public TutorialUI.Mode DisplayMode;

	public Int32 QuadmistTutorialID;

	public Int32 BasicControlTutorialID;

	public UIScene.SceneVoidDelegate AfterFinished;

	private UISprite battleTutorialImage1;

	private UISprite battleTutorialImage2;

	private UISprite battleTutorialDialogImage2;

	private UILabel battleLeftLabel;

	private UILabel battleRightLabel;

	private UILabel battleBottomLabel;

	private Single duration = 0.16f;

	private String currentButtonGroup = String.Empty;

	private Boolean isFromPause;

	private Int32 lastPage = 1;

	public enum Mode
	{
		Battle,
		QuadMist,
		BasicControl
	}
}
