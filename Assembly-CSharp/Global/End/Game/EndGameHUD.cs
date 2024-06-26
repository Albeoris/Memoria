using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;

public class EndGameHUD : UIScene
{
	public override void Show(UIScene.SceneVoidDelegate afterFinished = null)
	{
		base.Show(afterFinished);
		this.hasDoubleAllowed = false;
		this.hasSplitAllowed = false;
		this.hasJustChangeToPlayState = true;
		this.changeWageAmountCounter = 0.115f;
		this.lastChangeWageButtonState = Control.None;
		this.doubleButton = this.doubleButtonGo.GetComponent<UIButton>();
		this.doubleButtonLabel = this.doubleButtonGo.GetComponentInChildren<UILabel>();
		this.splitButton = this.splitButtonGo.GetComponent<UIButton>();
		this.splitButtonLabel = this.splitButtonGo.GetComponentInChildren<UILabel>();
		this.buttonGoList = new List<GameObject>
		{
			this.commitButtonGo,
			this.standButtonGo,
			this.hitButtonGo,
			this.doubleButtonGo,
			this.splitButtonGo,
			this.backButtonGo
		};
        this.BankRollLabel.text = EndGameMain.Instance.bankRoll.ToString();
        this.WageAmountLabel.text = EndGameMain.Instance.wager.ToString();
        foreach (GameObject go in this.buttonGoList)
		{
			UIEventListener uieventListener = UIEventListener.Get(go);
			uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.onClick));
		}
		UIEventListener uieventListener2 = UIEventListener.Get(this.minusButtonGo);
		uieventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onPress, new UIEventListener.BoolDelegate(this.onMinusOrPlusPress));
		SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
	}

	public override void Hide(UIScene.SceneVoidDelegate afterFinished = null)
	{
		base.NextSceneIsModal = false;
		UIScene.SceneVoidDelegate sceneVoidDelegate = delegate
		{
			PersistenSingleton<UIManager>.Instance.State = UIManager.UIState.Initial;
		};
		if (afterFinished != null)
		{
			sceneVoidDelegate = (UIScene.SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
		}
		base.Hide(sceneVoidDelegate);
		ButtonGroupState.DisableAllGroup(true);
	}

	private void onMinusOrPlusPress(GameObject go, Boolean isDown)
	{
		if (isDown)
		{
			if (go == this.minusButtonGo)
			{
				this.isDecreaseWager = true;
			}
			else if (go == this.plusButtonGo)
			{
				this.isIncreaseWager = true;
			}
		}
		else
		{
			this.isDecreaseWager = false;
			this.isIncreaseWager = false;
		}
		base.onPress(go, isDown);
	}

	public override Boolean OnKeyConfirm(GameObject go)
	{
		if (base.OnKeyConfirm(go))
		{
			if (go == this.commitButtonGo)
			{
				this.CommitWager();
			}
			else if (go == this.standButtonGo)
			{
				this.Stand();
			}
			else if (go == this.hitButtonGo)
			{
				this.Hit();
			}
			else if (go == this.doubleButtonGo)
			{
				this.Double();
			}
			else if (go == this.splitButtonGo)
			{
				this.Split();
			}
		}
		return true;
	}

	public override Boolean OnKeyCancel(GameObject go)
	{
		if (base.OnKeyCancel(go))
		{
			this.Exit();
		}
		return true;
	}

	public override Boolean OnKeySelect(GameObject go)
	{
		return false;
	}

	private void Update()
	{
		if (EndGameMain.Instance == (UnityEngine.Object)null)
		{
			return;
		}
		EndGameDef.FF9EndingGameState endingGameState = EndGameMain.Instance.endGame.GetEndingGameState();
		if (endingGameState == EndGameDef.FF9EndingGameState.ENDGAME_STATE_WAGER)
		{
			if (this.hasJustChangeToPlayState)
			{
				this.hasJustChangeToPlayState = false;
				ButtonGroupState.ActiveGroup = EndGameHUD.BlackJackWagerGroupButton;
			}
			Int64 bankRoll = EndGameMain.Instance.bankRoll;
			Int64 wager = EndGameMain.Instance.wager;
			if (this.prevBankRoll == -1L || this.prevBankRoll != bankRoll)
			{
				this.BankRollLabel.text = bankRoll.ToString();
				this.prevBankRoll = bankRoll;
			}
			if (this.prevWager == -1L || this.prevWager != wager)
			{
				this.WageAmountLabel.text = wager.ToString();
				this.prevWager = wager;
			}
			this.ValidateWager();
			if (UIManager.Input.GetKey(Control.LeftBumper) || (UIManager.Input.GetKey(Control.Confirm) && ButtonGroupState.ActiveButton == this.minusButtonGo) || this.isDecreaseWager)
			{
				if (this.lastChangeWageButtonState != Control.LeftBumper && this.lastChangeWageButtonState != Control.RightBumper)
				{
					this.DecreaseWager();
					this.changeWageAmountCounter = 0.276000023f;
				}
				else
				{
					this.changeWageAmountCounter -= Time.deltaTime;
					if (this.changeWageAmountCounter <= 0f && this.lastChangeWageButtonState != Control.RightBumper)
					{
						this.DecreaseWager();
						if (this.changeWageAmountCounter < 0.0575f)
						{
							this.changeWageAmountCounter = 0f;
						}
						this.changeWageAmountCounter += 0.115f;
					}
				}
				this.lastChangeWageButtonState = Control.LeftBumper;
			}
			else if (this.lastChangeWageButtonState == Control.LeftBumper)
			{
				this.lastChangeWageButtonState = Control.None;
			}
			if (UIManager.Input.GetKey(Control.RightBumper) || (UIManager.Input.GetKey(Control.Confirm) && ButtonGroupState.ActiveButton == this.plusButtonGo) || this.isIncreaseWager)
			{
				if (this.lastChangeWageButtonState != Control.RightBumper && this.lastChangeWageButtonState != Control.LeftBumper)
				{
					this.IncreaseWager();
					this.changeWageAmountCounter = 0.276000023f;
				}
				else
				{
					this.changeWageAmountCounter -= Time.deltaTime;
					if (this.changeWageAmountCounter <= 0f && this.lastChangeWageButtonState != Control.LeftBumper)
					{
						this.IncreaseWager();
						if (this.changeWageAmountCounter < 0.0575f)
						{
							this.changeWageAmountCounter = 0f;
						}
						this.changeWageAmountCounter += 0.115f;
					}
				}
				this.lastChangeWageButtonState = Control.RightBumper;
			}
			else if (this.lastChangeWageButtonState == Control.RightBumper)
			{
				this.lastChangeWageButtonState = Control.None;
			}
		}
		else
		{
			if (!this.hasJustChangeToPlayState)
			{
				this.HasJustChangeToPlayState();
			}
			this.hasJustChangeToPlayState = true;
			if (EndGameMain.Instance.endGame.ff9endingGameDoubleAllowed != this.hasDoubleAllowed)
			{
				this.SetEnableDoubleButton(EndGameMain.Instance.endGame.ff9endingGameDoubleAllowed);
				this.hasDoubleAllowed = EndGameMain.Instance.endGame.ff9endingGameDoubleAllowed;
			}
			if (EndGameMain.Instance.endGame.ff9endingGameSplitAllowed != this.hasSplitAllowed)
			{
				this.SetEnableSplitButton(EndGameMain.Instance.endGame.ff9endingGameSplitAllowed);
				this.hasSplitAllowed = EndGameMain.Instance.endGame.ff9endingGameSplitAllowed;
			}
		}
		this.dealerCardTotalLabel.text = EndGameMain.Instance.endGameScore.dealerCardTotal;
		if (String.IsNullOrEmpty(EndGameMain.Instance.endGameScore.splitCardTotal))
		{
			this.playerSplitCardTotalLabel.text = String.Empty;
			this.playerSplitMinTotalLabel.text = String.Empty;
			this.splitCardTotalLabel.text = String.Empty;
			this.splitMinTotalLabel.text = String.Empty;
			this.playerCardTotalLabel.text = EndGameMain.Instance.endGameScore.playerCardTotal;
			this.playerMinTotalLabel.text = EndGameMain.Instance.endGameScore.playerMinTotal;
		}
		else
		{
			this.playerCardTotalLabel.text = String.Empty;
			this.playerMinTotalLabel.text = String.Empty;
			this.playerSplitCardTotalLabel.text = EndGameMain.Instance.endGameScore.playerCardTotal;
			this.playerSplitMinTotalLabel.text = EndGameMain.Instance.endGameScore.playerMinTotal;
			this.splitCardTotalLabel.text = EndGameMain.Instance.endGameScore.splitCardTotal;
			this.splitMinTotalLabel.text = EndGameMain.Instance.endGameScore.splitMinTotal;
		}
	}

	private void SetEnableDoubleButton(Boolean isEnable)
	{
		if (isEnable)
		{
			this.doubleButtonLabel.text = "DOUBLE";
			this.doubleButton.isEnabled = true;
		}
		else
		{
			this.doubleButtonLabel.text = "-----";
			this.doubleButton.isEnabled = false;
			ButtonGroupState.ActiveButton = this.standButtonGo;
		}
	}

	private void SetEnableSplitButton(Boolean isEnable)
	{
		if (isEnable)
		{
			this.splitButtonLabel.text = "SPLIT";
			this.splitButton.isEnabled = true;
		}
		else
		{
			this.splitButtonLabel.text = "-----";
			this.splitButton.isEnabled = false;
			ButtonGroupState.ActiveButton = this.standButtonGo;
		}
	}

	private void HasJustChangeToPlayState()
	{
		ButtonGroupState.ActiveGroup = EndGameHUD.BlackJackDealGroupButton;
		ButtonGroupState.ActiveButton = this.standButtonGo;
		if (!this.hasDoubleAllowed)
		{
			this.doubleButtonLabel.text = "-----";
			this.doubleButton.isEnabled = false;
		}
		if (!this.hasSplitAllowed)
		{
			this.splitButtonLabel.text = "-----";
			this.splitButton.isEnabled = false;
		}
	}

	private void ValidateWager()
	{
		EndGameMain.Instance.ValidateWager();
	}

	private void IncreaseWager()
	{
		EndGameMain.Instance.IncreaseWager();
	}

	private void DecreaseWager()
	{
		EndGameMain.Instance.DecreaseWager();
	}

	private void CommitWager()
	{
		if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_BET)
		{
			EndGameMain.Instance.endGame.ff9endingGameWager = EndGameMain.Instance.wager;
			EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT);
		}
	}

	private void Stand()
	{
		if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION)
		{
			EndAct.Endact_SetMode(EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_STAND);
			EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT);
		}
	}

	private void Hit()
	{
		if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION)
		{
			EndAct.Endact_SetMode(EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_HIT);
			EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT);
		}
	}

	private void Double()
	{
		if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION)
		{
			EndAct.Endact_SetMode(EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_DOUBLE);
			EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT);
		}
	}

	private void Split()
	{
		if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION)
		{
			EndAct.Endact_SetMode(EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_SPLIT);
			EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT);
		}
	}

	private void Exit()
	{
		EndGameMain.Instance.endGame.FF9SFX_Play(103u);
		FF9Snd.ff9endsnd_song_vol_intpl(156, 60, 0);
		SoundLib.GetAllSoundDispatchPlayer().StopCurrentSong(60);
		SceneDirector.Replace(PersistenSingleton<SceneDirector>.Instance.LastScene, SceneTransition.FadeOutToBlack_FadeIn, true);
	}

	private const Single CHANGE_WAGE_AMOUNT_DURATION = 0.115f;

	public UILabel WageAmountLabel;

	public UILabel BankRollLabel;

	private Int64 prevBankRoll = -1L;

	private Int64 prevWager = -1L;

	private Boolean hasDoubleAllowed;

	private Boolean hasSplitAllowed;

	private Boolean hasJustChangeToPlayState;

	private Single changeWageAmountCounter;

	private Control lastChangeWageButtonState;

	private static String BlackJackWagerGroupButton = "BlackJack.Buttons.Wager";

	private static String BlackJackDealGroupButton = "BlackJack.Buttons.Deal";

	private List<GameObject> buttonGoList;

	private UIButton doubleButton;

	private UILabel doubleButtonLabel;

	private UIButton splitButton;

	private UILabel splitButtonLabel;

	public GameObject minusButtonGo;

	public GameObject plusButtonGo;

	public GameObject commitButtonGo;

	public GameObject standButtonGo;

	public GameObject hitButtonGo;

	public GameObject doubleButtonGo;

	public GameObject splitButtonGo;

	public GameObject backButtonGo;

	public UILabel dealerCardTotalLabel;

	public UILabel playerCardTotalLabel;

	public UILabel playerMinTotalLabel;

	public UILabel playerSplitCardTotalLabel;

	public UILabel playerSplitMinTotalLabel;

	public UILabel splitCardTotalLabel;

	public UILabel splitMinTotalLabel;

	private Boolean isDecreaseWager;

	private Boolean isIncreaseWager;
}
