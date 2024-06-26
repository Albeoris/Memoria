using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputHandler : MonoBehaviour
{
	public abstract void SetDelegate(InputHandler inputHandler);

	public abstract void HandlePreSelection(PreBoard preBoard, Hand hand, ref InputResult result);

	public abstract void HandleYourCardSelection(Board board, Hand hand, ref InputResult result);

	public abstract void HandleTargetCardSelection(Board board, QuadMistCard origin, List<QuadMistCard> selectable, ref InputResult result);

	public abstract void HandlePostSelection(Hand hand, List<Int32> selectable, ref InputResult result);

	public abstract void HandleConfirmation(ref InputResult result);

	public abstract void HandleDialog(ref InputResult result);

	public abstract void OnCursorForceChanged(Int32 cardIndex);
}
