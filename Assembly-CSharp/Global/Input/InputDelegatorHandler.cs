using System;
using System.Collections.Generic;
using UnityEngine;

public class InputDelegatorHandler : InputHandler
{
	public InputDelegatorHandler.InputType LastActiveInputHandler
	{
		get
		{
			return this.lastActiveInputHandler;
		}
		set
		{
			this.lastActiveInputHandler = value;
		}
	}

	public InputDelegatorHandler.InputType StrictActiveInputHandler
	{
		get
		{
			return this.strictActiveInputHandler;
		}
		set
		{
			this.strictActiveInputHandler = value;
		}
	}

	private void Start()
	{
		if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor)
		{
			this.inputs.Add(this.MouseInput);
			this.inputs.Add(this.KeyboardInput);
		}
		else
		{
			this.inputs.Add(this.MouseInput);
			this.inputs.Add(this.KeyboardInput);
		}
		foreach (InputHandler inputHandler in this.inputs)
		{
			inputHandler.SetDelegate(this);
		}
		this.lastActiveInputHandler = InputDelegatorHandler.InputType.None;
		this.strictActiveInputHandler = InputDelegatorHandler.InputType.None;
	}

	public override void HandlePreSelection(PreBoard preBoard, Hand playerHand, ref InputResult result)
	{
		if (this.strictActiveInputHandler != InputDelegatorHandler.InputType.None)
		{
			this.inputs[(Int32)this.strictActiveInputHandler].HandlePreSelection(preBoard, playerHand, ref result);
		}
		else
		{
			foreach (InputHandler inputHandler in this.inputs)
			{
				inputHandler.HandlePreSelection(preBoard, playerHand, ref result);
			}
		}
	}

	public override void HandleYourCardSelection(Board board, Hand playerHand, ref InputResult result)
	{
		if (this.strictActiveInputHandler != InputDelegatorHandler.InputType.None)
		{
			this.inputs[(Int32)this.strictActiveInputHandler].HandleYourCardSelection(board, playerHand, ref result);
		}
		else
		{
			foreach (InputHandler inputHandler in this.inputs)
			{
				inputHandler.HandleYourCardSelection(board, playerHand, ref result);
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			SpriteClickable component = this.MouseInput.CardNameToggleButton.GetComponent<SpriteClickable>();
			if (component != (UnityEngine.Object)null && component.Contains(worldPoint) && this.MouseInput.CardNameDialogSlider.IsReady)
			{
				this.MouseInput.CardNameDialogSlider.IsShowCardName = !this.MouseInput.CardNameDialogSlider.IsShowCardName;
				if (this.MouseInput.CardNameDialogSlider.IsShowCardName)
				{
					this.MouseInput.CardNameDialogSlider.ShowCardNameDialog(playerHand);
				}
				else
				{
					this.MouseInput.CardNameDialogSlider.HideCardNameDialog(playerHand);
				}
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CURSOL);
			}
		}
	}

	public override void HandleTargetCardSelection(Board board, QuadMistCard origin, List<QuadMistCard> selectable, ref InputResult result)
	{
		if (this.strictActiveInputHandler != InputDelegatorHandler.InputType.None)
		{
			this.inputs[(Int32)this.strictActiveInputHandler].HandleTargetCardSelection(board, origin, selectable, ref result);
		}
		else
		{
			foreach (InputHandler inputHandler in this.inputs)
			{
				inputHandler.HandleTargetCardSelection(board, origin, selectable, ref result);
			}
		}
	}

	public override void HandlePostSelection(Hand enemyHand, List<Int32> selectable, ref InputResult result)
	{
		if (this.strictActiveInputHandler != InputDelegatorHandler.InputType.None)
		{
			this.inputs[(Int32)this.strictActiveInputHandler].HandlePostSelection(enemyHand, selectable, ref result);
		}
		else
		{
			foreach (InputHandler inputHandler in this.inputs)
			{
				inputHandler.HandlePostSelection(enemyHand, selectable, ref result);
			}
		}
	}

	public override void HandleConfirmation(ref InputResult result)
	{
		if (this.strictActiveInputHandler != InputDelegatorHandler.InputType.None)
		{
			this.inputs[(Int32)this.strictActiveInputHandler].HandleConfirmation(ref result);
		}
		else
		{
			foreach (InputHandler inputHandler in this.inputs)
			{
				inputHandler.HandleConfirmation(ref result);
			}
		}
	}

	public override void HandleDialog(ref InputResult result)
	{
		if (this.strictActiveInputHandler != InputDelegatorHandler.InputType.None)
		{
			this.inputs[(Int32)this.strictActiveInputHandler].HandleDialog(ref result);
		}
		else
		{
			foreach (InputHandler inputHandler in this.inputs)
			{
				inputHandler.HandleDialog(ref result);
			}
		}
	}

	public override void SetDelegate(InputHandler inputHandler)
	{
		throw new NotImplementedException();
	}

	public override void OnCursorForceChanged(Int32 cardIndex)
	{
		foreach (InputHandler inputHandler in this.inputs)
		{
			inputHandler.OnCursorForceChanged(cardIndex);
		}
	}

	public MouseInputHandler MouseInput;

	public ButtonInputHandler KeyboardInput;

	private InputDelegatorHandler.InputType lastActiveInputHandler;

	private InputDelegatorHandler.InputType strictActiveInputHandler;

	private List<InputHandler> inputs = new List<InputHandler>();

	public enum InputType
	{
		Mouse,
		Keyboard,
		None
	}
}
