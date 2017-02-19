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
		Int32[,,] array = new Int32[Board.SIZE_X, Board.SIZE_Y, hand.Count];
		QuadMistCard defender = new QuadMistCard();
		Int32 num;
		Int32 num2;
		if (enemyScore == playerScore)
		{
			num = 64;
			num2 = 1;
		}
		else if (enemyScore < playerScore)
		{
			num = 67;
			num2 = 1;
		}
		else
		{
			num = 1;
			num2 = 5;
		}
		if (UnityEngine.Random.Range(0, 100) > this.wiseResetChance[this.wise])
		{
			num = 0;
			num2 = 0;
		}
		Int32 num3 = this.MIN_POINTS;
		Int32 num4 = 0;
		for (Int32 i = 0; i < hand.Count; i++)
		{
			for (Int32 j = 0; j < Board.SIZE_X; j++)
			{
				for (Int32 k = 0; k < Board.SIZE_Y; k++)
				{
					array[j, k, i] = 0;
					if (!board.IsFree(j, k))
					{
						array[j, k, i] = this.MIN_POINTS;
					}
					else
					{
						Int32 num5 = this.Check(board, hand[i], new Vector2((Single)j, (Single)k), out defender);
						if (num5 > 0)
						{
							array[j, k, i] += num5 * num;
						}
						else if (num5 < 0)
						{
							BattleCalculation battleCalculation = QuadMistGame.main.Calculate(hand[i], defender);
							Int32 num6 = battleCalculation.atkStart * 100 / (battleCalculation.atkStart + battleCalculation.defStart);
							if (this.wise == 3)
							{
								num2 = 1;
								if (num6 < 60)
								{
									num6 -= 100;
								}
								else
								{
									num6 = 145 - num6;
								}
							}
							array[j, k, i] = num6 * num2;
						}
					}
					if (array[j, k, i] == num3)
					{
						num4++;
					}
					if (array[j, k, i] > num3)
					{
						num3 = array[j, k, i];
						num4 = 1;
					}
				}
			}
		}
		num3 = this.MIN_POINTS;
		result.x = 0;
		result.y = 0;
		result.index = 0;
		result.selectedCard = hand[0];
		for (Int32 l = 0; l < hand.Count; l++)
		{
			for (Int32 m = 0; m < Board.SIZE_X; m++)
			{
				for (Int32 n = 0; n < Board.SIZE_Y; n++)
				{
					Int32 num7 = 0;
					if (num3 < array[m, n, l])
					{
						num7++;
					}
					if (num3 == array[m, n, l] && UnityEngine.Random.Range(0, 1000) < 1000 / num4)
					{
						num7++;
					}
					if (num7 != 0)
					{
						result.x = m;
						result.y = n;
						result.index = l;
						result.selectedCard = hand[l];
						num3 = array[m, n, l];
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
			{
				SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_MOVE);
			}
			if (sel < hand.Count)
			{
				hand.EnemyFakeSelect = sel;
			}
			yield return base.StartCoroutine(Anim.Tick(2));
		}
		this.thinking = false;
		yield break;
	}

	private Int32 Check(Board board, QuadMistCard yourCard, Vector2 origin, out QuadMistCard target)
	{
		target = (QuadMistCard)null;
		QuadMistCard[] adjacentCards = board.GetAdjacentCards((Int32)origin.x, (Int32)origin.y);
		Int32 num = 0;
		Int32 num2 = 0;
		for (Int32 i = 0; i < (Int32)adjacentCards.Length; i++)
		{
			if (adjacentCards[i] != null)
			{
				QuadMistCard quadMistCard = adjacentCards[i];
				if (quadMistCard.side != yourCard.side)
				{
					Int32 num3 = CardArrow.CheckDirection(yourCard.arrow, quadMistCard.arrow, (CardArrow.Type)i);
					if (num3 == 1)
					{
						num++;
					}
					if (num3 == 2)
					{
						if (target == null)
						{
							target = adjacentCards[i];
						}
						num2++;
					}
				}
			}
		}
		if (num2 != 0)
		{
			return -num2;
		}
		if (num != 0)
		{
			return num;
		}
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
