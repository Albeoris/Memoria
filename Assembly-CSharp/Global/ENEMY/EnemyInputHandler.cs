using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemyInputHandler : InputHandler
{
	public void Start()
	{
		this.ai = base.GetComponent<EnemyAI>();
	}

	public override void HandlePreSelection(PreBoard preBoard, Hand hand, ref InputResult result)
	{
		throw new NotImplementedException();
	}

	public override void HandleYourCardSelection(Board board, Hand hand, ref InputResult result)
	{
		if (!this.animated)
		{
			hand.State = Hand.STATE.ENEMY_PLAY;
			this.ai.Think(hand);
			this.animated = true;
		}
		else if (this.animated && !this.ai.thinking)
		{
			this.ai.SetCard(board, hand, ref result);
			hand.RemoveAt(result.index);
			result.Used();
			this.animated = false;
		}
	}

	public override void HandleTargetCardSelection(Board board, QuadMistCard origin, List<QuadMistCard> selectable, ref InputResult result)
	{
		result.index = 0;
		result.Used();
	}

	public override void HandlePostSelection(Hand hand, List<Int32> selectable, ref InputResult result)
	{
		result.selectedCard = hand[selectable[0]];
		result.Used();
	}

	public override void HandleConfirmation(ref InputResult result)
	{
		throw new NotImplementedException();
	}

	public override void HandleDialog(ref InputResult result)
	{
		throw new NotImplementedException();
	}

	public override void SetDelegate(InputHandler inputHandler)
	{
		throw new NotImplementedException();
	}

	public override void OnCursorForceChanged(Int32 cardIndex)
	{
		throw new NotImplementedException();
	}

	private Boolean animated;

	private EnemyAI ai;
}
