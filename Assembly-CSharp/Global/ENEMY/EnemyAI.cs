using System;
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	public void SetCard(Board board, Hand hand, ref InputResult result)
	{
		this.wise = EnemyData.Main.Wise;
		Int32 playerScore = QuadMistGame.main.PlayerScore;
		Int32 enemyScore = QuadMistGame.main.EnemyScore;
		Int32[,,] choiceScore = new Int32[Board.SIZE_X, Board.SIZE_Y, hand.Count];
		QuadMistCard defender;
		Int32 flipNoFightFactor;
		Int32 fightFactor;
		if (enemyScore == playerScore)
		{
			flipNoFightFactor = 64;
			fightFactor = 1;
		}
		else if (enemyScore < playerScore)
		{
			flipNoFightFactor = 67;
			fightFactor = 1;
		}
		else
		{
			flipNoFightFactor = 1;
			fightFactor = 5;
		}
		if (UnityEngine.Random.Range(0, 100) > this.wiseResetChance[this.wise])
		{
			flipNoFightFactor = 0;
			fightFactor = 0;
		}
		Int32 bestChoiceScore = this.MIN_POINTS;
		Int32 bestScoreCount = 0;
		for (Int32 i = 0; i < hand.Count; i++)
		{
			for (Int32 x = 0; x < Board.SIZE_X; x++)
			{
				for (Int32 y = 0; y < Board.SIZE_Y; y++)
				{
					choiceScore[x, y, i] = 0;
					if (!board.IsFree(x, y))
					{
						choiceScore[x, y, i] = this.MIN_POINTS;
					}
					else
					{
						Int32 flipCount = this.Check(board, hand[i], new Vector2(x, y), out defender);
						if (flipCount > 0)
						{
							choiceScore[x, y, i] += flipCount * flipNoFightFactor;
						}
						else if (flipCount < 0) // A battle would occur
						{
							BattleCalculation battleCalculation = QuadMistGame.main.Calculate(hand[i], defender);
							Int32 winChance = battleCalculation.atkStart * 100 / (battleCalculation.atkStart + battleCalculation.defStart);
							if (this.wise == 3)
							{
								if (winChance < 60)
									choiceScore[x, y, i] = winChance - 100;
								else
									choiceScore[x, y, i] = 145 - winChance;
							}
							else
							{
								choiceScore[x, y, i] = winChance * fightFactor;
							}
						}
					}
					if (choiceScore[x, y, i] == bestChoiceScore)
						bestScoreCount++;
					if (choiceScore[x, y, i] > bestChoiceScore)
					{
						bestChoiceScore = choiceScore[x, y, i];
						bestScoreCount = 1;
					}
				}
			}
		}
		bestChoiceScore = this.MIN_POINTS;
		result.x = 0;
		result.y = 0;
		result.index = 0;
		result.selectedCard = hand[0];
		for (Int32 i = 0; i < hand.Count; i++)
		{
			for (Int32 x = 0; x < Board.SIZE_X; x++)
			{
				for (Int32 y = 0; y < Board.SIZE_Y; y++)
				{
					Boolean isBestChoice = false;
					if (bestChoiceScore < choiceScore[x, y, i])
						isBestChoice = true;
					// This random pick is uneven when there are several choices with the same best score:
					// - the first best-scored placement has way more chances to be kept (always higher than 1/e)
					// - the second best-scored placement has less chances to be used
					// - the k-th best-scored placement has increasingly more chances to be used
					// - up to the last best-scored placement which has 1/bestScoreCount chances to be used
					// Discounting the integer flooring of the division, the probability formulas would be:
					//  P(1) = ((n-1)/n)^(n-1)
					//  P(k) = ((n-1)/n)^(n-k) / n, for 2 <= k <= n
					// Concretely, for turns with many best scored choices (eg. 1st turn or if "wiseResetChance" proc),
					// putting the 1st card of the hand at the top-left-most position of the board will be more probable
					if (bestChoiceScore == choiceScore[x, y, i] && UnityEngine.Random.Range(0, 1000) < 1000 / bestScoreCount)
						isBestChoice = true;
					if (isBestChoice)
					{
						result.x = x;
						result.y = y;
						result.index = i;
						result.selectedCard = hand[i];
						bestChoiceScore = choiceScore[x, y, i];
					}
				}
			}
		}
		hand.ClearFakeSelectData();
	}

	public void Think(Hand hand)
	{
		base.StartCoroutine(this.Thinking(hand));
	}

	private IEnumerator Thinking(Hand hand)
	{
		this.thinking = true;
		this.enemyThinkTick = UnityEngine.Random.Range(15, 31);
		Int32 sel = 0;
		hand.Select = sel;
		yield return base.StartCoroutine(Anim.Tick(this.enemyThinkTick));
		this.enemyThinkTick = UnityEngine.Random.Range(-2, 21);
		for (Int32 tick = 0; tick < this.enemyThinkTick; tick++)
		{
			sel = tick / 4;
			if (tick % 4 == 0)
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_MOVE);
			if (sel < hand.Count)
				hand.EnemyFakeSelect = sel;
			yield return base.StartCoroutine(Anim.Tick(2));
		}
		this.thinking = false;
		yield break;
	}

	private Int32 Check(Board board, QuadMistCard yourCard, Vector2 origin, out QuadMistCard target)
	{
		target = null;
		QuadMistCard[] adjacentCards = board.GetAdjacentCards((Int32)origin.x, (Int32)origin.y);
		Int32 flipCount = 0;
		Int32 fightCount = 0;
		for (Int32 i = 0; i < adjacentCards.Length; i++)
		{
			if (adjacentCards[i] != null)
			{
				QuadMistCard quadMistCard = adjacentCards[i];
				if (quadMistCard.side != yourCard.side)
				{
					Int32 interactionType = CardArrow.CheckDirection(yourCard.arrow, quadMistCard.arrow, (CardArrow.Type)i);
					if (interactionType == 1)
						flipCount++;
					if (interactionType == 2)
					{
						if (target == null)
							target = adjacentCards[i];
						fightCount++;
					}
				}
			}
		}
		if (fightCount != 0)
			return -fightCount;
		if (flipCount != 0)
			return flipCount;
		return 0;
	}

	public Boolean thinking;

	private Int32 wise;

	private Int32[] wiseResetChance = new Int32[]
	{
		28,
		62,
		92,
		100
	};

	private Int32 MIN_POINTS = -32767;

	private Int32 enemyThinkTick;
}
