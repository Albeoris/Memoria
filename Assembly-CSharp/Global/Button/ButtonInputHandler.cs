using System;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInputHandler : InputHandler
{
	public override void SetDelegate(InputHandler inputHandler)
	{
		this.delegateInputHandler = (InputDelegatorHandler)inputHandler;
	}

	public override void HandlePreSelection(PreBoard preBoard, Hand playerHand, ref InputResult result)
	{
		this.prevPreSelect = this.preSelect;
		if (this.preSelect == -1)
		{
			this.preSelect = 0;
		}
		if (UIManager.Input.GetKey(Control.Down) && this.preSelect % 10 != 9)
		{
			this.preSelect++;
		}
		if (UIManager.Input.GetKey(Control.Up) && this.preSelect % 10 != 0)
		{
			this.preSelect--;
		}
		if (UIManager.Input.GetKey(Control.Right) && this.preSelect / 10 != 9)
		{
			this.preSelect += 10;
		}
		if (UIManager.Input.GetKey(Control.Left) && this.preSelect / 10 != 0)
		{
			this.preSelect -= 10;
		}
		if (this.prevPreSelect != this.preSelect || !this.launched)
		{
			preBoard.SetPreviewCardID(this.preSelect);
			this.launched = true;
		}
		if (UIManager.Input.GetKey(Control.LeftBumper))
		{
			preBoard.PrevCard();
			SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_MOVE);
		}
		if (UIManager.Input.GetKey(Control.RightBumper))
		{
			preBoard.NextCard();
			SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_MOVE);
		}
		if (UIManager.Input.GetKey(Control.Confirm))
		{
			if (playerHand.Count != 5 && preBoard.CountSelected() > 0)
			{
				QuadMistCard item = preBoard.RemoveSelected();
				playerHand.Add(item);
				if (playerHand.Count == 5)
				{
					result.Used();
					this.launched = false;
					return;
				}
			}
			else if (playerHand.Count == 5)
			{
				result.Used();
				this.launched = false;
				return;
			}
			if (QuadMistGame.main.CardNameDialogSlider.IsShowCardName)
			{
				QuadMistGame.main.CardNameDialogSlider.ShowCardNameDialog(playerHand);
			}
			else
			{
				QuadMistGame.main.CardNameDialogSlider.HideCardNameDialog(playerHand);
			}
		}
		else if (UIManager.Input.GetKey(Control.Cancel))
		{
			Int32 num = playerHand.Count - 1;
			if (num >= 0)
			{
				preBoard.Add(playerHand[num]);
				playerHand.RemoveAt(num);
			}
			SoundEffect.Play(QuadMistSoundID.MINI_SE_CANCEL);
		}
	}

	public override void HandleYourCardSelection(Board board, Hand playerHand, ref InputResult result)
	{
		if (this.requestUpdatePlayerHandSelect)
		{
			playerHand.UpdateShadowCard(playerHand.Select);
			playerHand.UpdateCursorToShadowCard();
			this.requestUpdatePlayerHandSelect = false;
		}
		this.hasCalledHandlePostSelection = false;
		if (playerHand.State == Hand.STATE.PLAYER_SELECT_CARD)
		{
			if (this.delegateInputHandler.LastActiveInputHandler == InputDelegatorHandler.InputType.Mouse)
			{
				this.playSelect = playerHand.Select;
			}
			this.prevPlaySelect = this.playSelect;
			playerHand.cursor.gameObject.SetActive(true);
			playerHand.CardCursor.SetNormalState();
			if (this.playSelect >= playerHand.Count - 1)
			{
				this.playSelect = playerHand.Count - 1;
			}
			else if (this.playSelect < 0)
			{
				this.playSelect = 0;
			}
			Boolean flag = false;
			if (UIManager.Input.GetKeyTrigger(Control.Down) && this.playSelect != playerHand.Count - 1)
			{
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
				this.playSelect++;
				flag = true;
			}
			if (UIManager.Input.GetKeyTrigger(Control.Up) && this.playSelect != 0)
			{
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
				this.playSelect--;
				flag = true;
			}
			if (this.delegateInputHandler.LastActiveInputHandler == InputDelegatorHandler.InputType.Keyboard)
			{
				playerHand.Select = this.playSelect;
				this.launched = true;
			}
			if (flag)
			{
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
				if (QuadMistGame.main.CardNameDialogSlider.IsShowCardName)
				{
					QuadMistGame.main.CardNameDialogSlider.ShowCardNameDialog(playerHand);
				}
				else
				{
					QuadMistGame.main.CardNameDialogSlider.HideCardNameDialog(playerHand);
				}
			}
			if (UIManager.Input.GetKeyTrigger(Control.Confirm))
			{
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
				this.delegateInputHandler.StrictActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
				playerHand.State = Hand.STATE.PLAYER_SELECT_BOARD;
				this.launched = false;
				board.ShowBoardCursor();
				playerHand.HideShadowCard();
			}
		}
		else if (playerHand.State == Hand.STATE.PLAYER_SELECT_BOARD)
		{
			this.prevBoardSelectX = this.boardSelectX;
			this.prevBoardSelectY = this.boardSelectY;
			board.cursor.gameObject.SetActive(true);
			Boolean flag2 = false;
			if (UIManager.Input.GetKeyTrigger(Control.Up) && this.boardSelectY != 0)
			{
				this.boardSelectY--;
				flag2 = true;
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			}
			if (UIManager.Input.GetKeyTrigger(Control.Down) && this.boardSelectY != 3)
			{
				this.boardSelectY++;
				flag2 = true;
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			}
			if (UIManager.Input.GetKeyTrigger(Control.Left) && this.boardSelectX != 0)
			{
				this.boardSelectX--;
				flag2 = true;
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			}
			if (UIManager.Input.GetKeyTrigger(Control.Right) && this.boardSelectX != 3)
			{
				this.boardSelectX++;
				flag2 = true;
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			}
			if (flag2)
			{
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
			}
			if (this.prevBoardSelectX != this.boardSelectX || this.prevBoardSelectY != this.boardSelectY || !this.launched)
			{
				Vector2 boardCursorPosition = new Vector2((Single)this.boardSelectX, (Single)this.boardSelectY);
				Single num = 0.429f;
				Single num2 = -0.525f;
				Single num3 = -1f;
				Single num4 = 0.73f;
				boardCursorPosition.x *= num;
				boardCursorPosition.y *= num2;
				boardCursorPosition.x += num3;
				boardCursorPosition.y += num4;
				board.SetBoardCursorPosition(boardCursorPosition);
				this.launched = true;
			}
			if (UIManager.Input.GetKeyTrigger(Control.Confirm) && board.IsFree(this.boardSelectX, this.boardSelectY))
			{
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_MOVE);
				QuadMistGame.main.CardNameDialogSlider.HideCardNameDialog(playerHand);
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
				this.delegateInputHandler.StrictActiveInputHandler = InputDelegatorHandler.InputType.None;
				result.selectedCard = playerHand[this.playSelect];
				playerHand.RemoveAt(this.playSelect);
				result.x = this.boardSelectX;
				result.y = this.boardSelectY;
				result.Used();
				this.launched = false;
				board.HideBoardCursor();
				playerHand.HideCardCursor();
				this.requestUpdatePlayerHandSelect = true;
			}
			else if (UIManager.Input.GetKeyTrigger(Control.Confirm))
			{
				SoundEffect.Play(QuadMistSoundID.MINI_SE_WARNING);
			}
			else if (UIManager.Input.GetKeyTrigger(Control.Cancel))
			{
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
				this.delegateInputHandler.StrictActiveInputHandler = InputDelegatorHandler.InputType.None;
				playerHand.State = Hand.STATE.PLAYER_SELECT_CARD;
				board.HideBoardCursor();
				playerHand.ShowCardCursor();
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CANCEL);
			}
		}
	}

	public override void HandleTargetCardSelection(Board board, QuadMistCard origin, List<QuadMistCard> selectable, ref InputResult result)
	{
		board.ShowBoardCursor();
		if (!this.blacken)
		{
			for (Int32 i = 0; i < (Int32)board.field.Length; i++)
			{
				QuadMistCardUI cardUI = board.GetCardUI(i);
				if (cardUI != (UnityEngine.Object)null && cardUI != board.GetCardUI(origin))
				{
					cardUI.Black = true;
				}
			}
			this.blacken = true;
		}
		if (this.counter >= Anim.TickToTime(16) && !this.launched)
		{
			foreach (QuadMistCard card in selectable)
			{
				board.GetCardUI(card).Black = false;
				board.GetCardUI(card).Select = true;
			}
			this.launched = true;
		}
		if (this.launched)
		{
			this.prevBoardSelectX = this.boardSelectX;
			this.prevBoardSelectY = this.boardSelectY;
			board.cursor.gameObject.SetActive(true);
			if (UIManager.Input.GetKeyTrigger(Control.Up) && this.boardSelectY != 0)
			{
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
				this.boardSelectY--;
			}
			if (UIManager.Input.GetKeyTrigger(Control.Down) && this.boardSelectY != 3)
			{
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
				this.boardSelectY++;
			}
			if (UIManager.Input.GetKeyTrigger(Control.Left) && this.boardSelectX != 0)
			{
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
				this.boardSelectX--;
			}
			if (UIManager.Input.GetKeyTrigger(Control.Right) && this.boardSelectX != 3)
			{
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
				this.boardSelectX++;
			}
			if (this.prevBoardSelectX != this.boardSelectX || this.prevBoardSelectY != this.boardSelectY)
			{
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
				board.PlaceCursor(this.boardSelectX, this.boardSelectY);
				Vector2 boardCursorPosition = new Vector2((Single)this.boardSelectX, (Single)this.boardSelectY);
				Single num = 0.429f;
				Single num2 = -0.525f;
				Single num3 = -1f;
				Single num4 = 0.73f;
				boardCursorPosition.x *= num;
				boardCursorPosition.y *= num2;
				boardCursorPosition.x += num3;
				boardCursorPosition.y += num4;
				board.SetBoardCursorPosition(boardCursorPosition);
			}
			if (UIManager.Input.GetKeyTrigger(Control.Confirm))
			{
				this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
				if (selectable.Contains(board[this.boardSelectX, this.boardSelectY]))
				{
					result.index = selectable.IndexOf(board[this.boardSelectX, this.boardSelectY]);
					result.Used();
					SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_MOVE);
				}
			}
			if (result.IsValid())
			{
				QuadMistCardUI[] field = board.field;
				for (Int32 j = 0; j < (Int32)field.Length; j++)
				{
					QuadMistCardUI quadMistCardUI = field[j];
					quadMistCardUI.Black = false;
					quadMistCardUI.Select = false;
				}
				this.launched = false;
				this.blacken = false;
				this.counter = 0f;
				board.cursor.gameObject.SetActive(false);
				board.HideBoardCursor();
			}
		}
		else
		{
			this.counter += Time.deltaTime;
		}
	}

	public override void HandlePostSelection(Hand enemyHand, List<Int32> selectable, ref InputResult result)
	{
		enemyHand.cursor.gameObject.SetActive(true);
		if (this.endSelect < 0)
		{
			this.endSelect = 0;
		}
		else if (this.endSelect > selectable.Count - 1)
		{
			this.endSelect = selectable.Count - 1;
		}
		this.prevEndSelect = this.endSelect;
		Boolean flag = false;
		if (!this.hasCalledHandlePostSelection)
		{
			this.hasCalledHandlePostSelection = true;
			this.endSelect = 0;
			flag = true;
		}
		if (UIManager.Input.GetKeyTrigger(Control.Up) && this.endSelect != 0)
		{
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			this.endSelect--;
			flag = true;
		}
		if (UIManager.Input.GetKeyTrigger(Control.Down) && this.endSelect != selectable.Count - 1)
		{
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			this.endSelect++;
			flag = true;
		}
		if (flag)
		{
			SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
			enemyHand.Select = selectable[this.endSelect];
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			Int32 num = selectable[this.endSelect];
			result.selectedCard = enemyHand[num];
			result.Used();
			result.selectedHandIndex = num;
			enemyHand.cursor.gameObject.SetActive(false);
			this.launched = false;
		}
		if (UIManager.Input.GetKeyTrigger(Control.Confirm))
		{
			SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_MOVE);
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			Int32 num2 = selectable[this.endSelect];
			result.selectedCard = enemyHand[num2];
			result.Used();
			result.selectedHandIndex = num2;
			enemyHand.cursor.gameObject.SetActive(false);
			this.launched = false;
		}
	}

	public override void HandleConfirmation(ref InputResult result)
	{
		if (UIManager.Input.GetKeyTrigger(Control.Confirm))
		{
			SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_MOVE);
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			result.Used();
		}
	}

	public override void HandleDialog(ref InputResult result)
	{
		if (UIManager.Input.GetKeyTrigger(Control.Right))
		{
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			if (QuadMistConfirmDialog.IsOK)
			{
				QuadMistConfirmDialog.MessageSelect(false);
			}
		}
		if (UIManager.Input.GetKeyTrigger(Control.Left))
		{
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			if (!QuadMistConfirmDialog.IsOK)
			{
				QuadMistConfirmDialog.MessageSelect(true);
			}
		}
		if (UIManager.Input.GetKeyTrigger(Control.Confirm))
		{
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Keyboard;
			result.Used();
		}
	}

	public override void OnCursorForceChanged(Int32 cardIndex)
	{
		if (QuadMistGame.main.GameState == GAME_STATE.PLAY)
		{
			this.playSelect = cardIndex;
		}
		else if (QuadMistGame.main.GameState == GAME_STATE.POSTGAME)
		{
			this.endSelect = cardIndex;
		}
	}

	private Int32 preSelect = -1;

	private Int32 playSelect;

	private Int32 endSelect;

	private Int32 boardSelectX;

	private Int32 boardSelectY;

	private Int32 prevPlaySelect;

	private Int32 prevPreSelect;

	private Int32 prevBoardSelectX;

	private Int32 prevBoardSelectY;

	private Int32 prevEndSelect;

	private Single counter;

	private Boolean blacken;

	private Boolean launched;

	private Boolean hasCalledHandlePostSelection;

	private InputDelegatorHandler delegateInputHandler;

	private Boolean requestUpdatePlayerHandSelect = true;
}
