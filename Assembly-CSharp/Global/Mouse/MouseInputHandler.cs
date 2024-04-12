using System;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputHandler : InputHandler
{
	public override void SetDelegate(InputHandler inputHandler)
	{
		this.delegateInputHandler = (InputDelegatorHandler)inputHandler;
	}

	public override void HandlePreSelection(PreBoard preBoard, Hand playerHand, ref InputResult result)
	{
		if (Input.GetMouseButtonUp(0))
		{
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Int32 indexByWorldPoint = preBoard.GetIndexByWorldPoint(worldPoint);
			if (indexByWorldPoint >= 0)
			{
				preBoard.SetPreviewCardID(indexByWorldPoint);
			}
			else
			{
				if (preBoard.GetPreviewByWorldPoint(worldPoint))
				{
					if (playerHand.Count != 5)
					{
						QuadMistCard item = preBoard.RemoveSelected();
						playerHand.Add(item);
						if (playerHand.Count == 5)
						{
							result.Used();
							return;
						}
					}
					else if (playerHand.Count == 5)
					{
						result.Used();
						return;
					}
				}
				Int32 lrbyWorldPoint = preBoard.GetLRByWorldPoint(worldPoint);
				if (lrbyWorldPoint == 1)
				{
					preBoard.NextCard();
				}
				if (lrbyWorldPoint == -1)
				{
					preBoard.PrevCard();
				}
			}
			Int32 indexByWorldPoint2 = playerHand.GetIndexByWorldPoint(worldPoint);
			if (indexByWorldPoint2 >= 0)
			{
				preBoard.Add(playerHand[indexByWorldPoint2]);
				playerHand.RemoveAt(indexByWorldPoint2);
			}
		}
	}

	public override void HandleYourCardSelection(Board board, Hand playerHand, ref InputResult result)
	{
		if (playerHand.State == Hand.STATE.PLAYER_SELECT_CARD)
		{
			playerHand.ShowCardCursor();
			playerHand.SetCardCursorTopMost();
			if (playerHand.Select == -1)
			{
				playerHand.Select = 0;
			}
			if (this.requestUpdatePlayerHandSelect)
			{
				playerHand.ForceUpdateCursor();
				playerHand.UpdateShadowCard(playerHand.Select);
				this.requestUpdatePlayerHandSelect = false;
			}
		}
		Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (Input.GetMouseButtonDown(0) && this.delegateInputHandler.StrictActiveInputHandler != InputDelegatorHandler.InputType.Keyboard)
		{
			if (this.delegateInputHandler.LastActiveInputHandler != InputDelegatorHandler.InputType.Mouse)
			{
				board.HideBoardCursor();
				playerHand.HideCardCursor();
				playerHand.HideShadowCard();
			}
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Mouse;
			Int32 select = playerHand.Select;
			playerHand.Select = playerHand.GetIndexByWorldPoint(vector);
			this.delegateInputHandler.OnCursorForceChanged(playerHand.Select);
			if (playerHand.Select != -1)
			{
				playerHand.CardCursor.Show();
				board.ShowBoardCursor();
				playerHand.IsDragged = true;
				this.delegateInputHandler.StrictActiveInputHandler = InputDelegatorHandler.InputType.Mouse;
				playerHand.ApplyScaleToSelectedCard(playerHand.Select);
				if (playerHand.Count == 5)
				{
					Vector3 vector2 = new Vector3(1.16f, 0.82f, 0f);
				}
				else
				{
					Vector3 vector2 = new Vector3(1.16f, 0.7f, 0f);
				}
				if (QuadMistGame.main.CardNameDialogSlider.IsShowCardName && (!QuadMistGame.main.CardNameDialogSlider.IsShowing || select != playerHand.Select))
				{
					QuadMistGame.main.CardNameDialogSlider.ShowCardNameDialog(playerHand);
				}
				playerHand.ShowShadowCard();
				playerHand.State = Hand.STATE.PLAYER_SELECT_BOARD;
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
			}
			if (playerHand.Select == -1)
			{
				playerHand.Select = select;
				if (playerHand.CardAnimatingCount == 0)
				{
					playerHand.ForceUpdateCursor();
					playerHand.UpdateShadowCard(playerHand.Select);
				}
			}
		}
		if (playerHand.IsDragged)
		{
			if (playerHand.SelectedUI == (UnityEngine.Object)null)
			{
				playerHand.SetCardScaleBecauseOfUserCancellation();
			}
			else
			{
				vector.z = -6f;
				playerHand.SelectedUI.transform.position = vector + new Vector3(-playerHand.SelectedUI.Size.x / 2f, playerHand.SelectedUI.Size.y / 2f, 0f);
				Vector2 vectorByWorldPoint = board.GetVectorByWorldPoint(vector, true);
				Single num = 0.429f;
				Single num2 = -0.525f;
				Single num3 = -1f;
				Single num4 = 0.73f;
				vectorByWorldPoint.x *= num;
				vectorByWorldPoint.y *= num2;
				vectorByWorldPoint.x += num3;
				vectorByWorldPoint.y += num4;
				board.SetBoardCursorPosition(vectorByWorldPoint);
				this.dragingCursor = vectorByWorldPoint;
			}
		}
		if (Input.GetMouseButtonUp(0) && this.delegateInputHandler.StrictActiveInputHandler == InputDelegatorHandler.InputType.Mouse)
		{
			this.dragingCursor = Vector2.zero;
			this.delegateInputHandler.StrictActiveInputHandler = InputDelegatorHandler.InputType.None;
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Mouse;
			Vector2 vectorByWorldPoint2 = board.GetVectorByWorldPoint(vector, true);
			if (vectorByWorldPoint2.x != -100f && playerHand.Select != -1)
			{
				result.selectedCard = playerHand[playerHand.Select];
				playerHand.Remove(result.selectedCard);
				result.x = (Int32)vectorByWorldPoint2.x;
				result.y = (Int32)vectorByWorldPoint2.y;
				result.Used();
				playerHand.HideShadowCard();
				QuadMistGame.main.CardNameDialogSlider.HideCardNameDialog(playerHand);
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
				this.requestUpdatePlayerHandSelect = true;
			}
			else
			{
				playerHand.State = Hand.STATE.PLAYER_SELECT_CARD;
				playerHand.SetCardScaleBecauseOfUserCancellation();
			}
			playerHand.HideCardCursor();
			board.HideBoardCursor();
			playerHand.SetCardCursorTopMost();
			playerHand.CardCursor.SetNormalState();
			playerHand.IsDragged = false;
		}
	}

	public override void HandleTargetCardSelection(Board board, QuadMistCard origin, List<QuadMistCard> selectable, ref InputResult result)
	{
		board.ShowBoardCursor();
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if ((this.lastMousePosition - Input.mousePosition).magnitude > 1f)
		{
			worldPoint.z = -6f;
			Vector2 vectorByWorldPoint = board.GetVectorByWorldPoint(worldPoint, false);
			Single num = 0.429f;
			Single num2 = -0.525f;
			Single num3 = -1f;
			Single num4 = 0.73f;
			vectorByWorldPoint.x *= num;
			vectorByWorldPoint.y *= num2;
			vectorByWorldPoint.x += num3;
			vectorByWorldPoint.y += num4;
			board.SetBoardCursorPosition(vectorByWorldPoint);
			this.lastMousePosition = Input.mousePosition;
		}
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
			if (Input.GetMouseButtonUp(0))
			{
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
				for (Int32 j = 0; j < selectable.Count; j++)
				{
					if (board.GetCardUI(selectable[j]).Contains(worldPoint))
					{
						result.index = j;
						result.Used();
						break;
					}
				}
				if (result.IsValid())
				{
					QuadMistCardUI[] field = board.field;
					for (Int32 k = 0; k < (Int32)field.Length; k++)
					{
						QuadMistCardUI quadMistCardUI = field[k];
						quadMistCardUI.Black = false;
						quadMistCardUI.Select = false;
					}
					this.launched = false;
					this.blacken = false;
					this.counter = 0f;
					board.ShowBoardCursor();
				}
			}
		}
		else
		{
			this.counter += Time.deltaTime;
		}
	}

	public override void HandlePostSelection(Hand enemyHand, List<Int32> selectable, ref InputResult result)
	{
		if (Input.GetMouseButtonDown(0))
		{
			QuadMistGame.main.IsSeizingCard = true;
		}
		if (Input.GetMouseButtonUp(0) && QuadMistGame.main.IsSeizingCard)
		{
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Mouse;
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Int32 indexByWorldPoint = enemyHand.GetIndexByWorldPoint(worldPoint);
			this.delegateInputHandler.OnCursorForceChanged(indexByWorldPoint);
			if (selectable.Contains(indexByWorldPoint))
			{
				result.selectedCard = enemyHand[indexByWorldPoint];
				result.Used();
				result.selectedHandIndex = indexByWorldPoint;
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
			}
			QuadMistGame.main.IsSeizingCard = false;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			QuadMistGame.main.IsSeizingCard = false;
		}
	}

	public override void HandleConfirmation(ref InputResult result)
	{
		if (Input.GetMouseButtonUp(0))
		{
			SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Mouse;
			result.Used();
		}
	}

	public override void HandleDialog(ref InputResult result)
	{
		if (Input.GetMouseButtonUp(0))
		{
			this.delegateInputHandler.LastActiveInputHandler = InputDelegatorHandler.InputType.Mouse;
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (QuadMistConfirmDialog.IsShowing && QuadMistConfirmDialog.MessageSelect(worldPos))
			{
				result.Used();
			}
		}
	}

	public override void OnCursorForceChanged(Int32 cardIndex)
	{
	}

	public GameObject CardNameToggleButton;

	public QuadMistCardNameDialogSlider CardNameDialogSlider;

	private Single counter;

	private Boolean blacken;

	private Boolean launched;

	private InputDelegatorHandler delegateInputHandler;

	private Vector2 dragingCursor = default(Vector2);

	private Boolean requestUpdatePlayerHandSelect = true;

	private Vector3 lastMousePosition = default(Vector3);
}
