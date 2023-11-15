using System;
using System.Collections;
using UnityEngine;
using Memoria;
using Memoria.Data;

public class EnemyAI : MonoBehaviour
{
	public void SetCard(Board board, Hand hand, ref InputResult result)
	{
		wise = EnemyData.Main.Wise;
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
        if (UnityEngine.Random.Range(0, 100) > wiseResetChance[wise])
		{
			flipNoFightFactor = 0;
			fightFactor = 0;
		}
		Int32 bestChoiceScore = MIN_POINTS;
		Int32 bestScoreCount = 0;
        Int32 defenseside = 0;
        for (Int32 i = 0; i < hand.Count; i++)
		{
			for (Int32 x = 0; x < Board.SIZE_X; x++)
			{
				for (Int32 y = 0; y < Board.SIZE_Y; y++)
				{
                    choiceScore[x, y, i] = 0;
					if (!board.IsFree(x, y))
					{
						choiceScore[x, y, i] = MIN_POINTS;
					}
					else
					{
						Int32 flipCount = Check(board, hand[i], new Vector2(x, y), out defender);
                        if (Configuration.TetraMaster.TripleTriad >= 2)
						{
                           defenseside = CheckEmptySide(board, hand[i], new Vector2(x, y)); // [DV] Defense IA
                        }
                        if (flipCount > 0)
						{
                            if (Configuration.TetraMaster.TripleTriad >= 2)
							{
                                choiceScore[x, y, i] += (flipCount * 1000 + defenseside * 50) * flipNoFightFactor;
                            }  
							else
							{
                                choiceScore[x, y, i] += flipCount * flipNoFightFactor;
                            }
                        }
                        else if (Configuration.TetraMaster.TripleTriad >= 2 && flipCount == 0)
                        {
                            choiceScore[x, y, i] += (defenseside * 100) * flipNoFightFactor;
                        }
                        else if (flipCount < 0) // A battle would occur
						{
							BattleCalculation battleCalculation = QuadMistGame.main.Calculate(hand[i], defender, x, y);
							Int32 winChance = battleCalculation.atkStart * 100 / (battleCalculation.atkStart + battleCalculation.defStart);
                            if (Configuration.TetraMaster.TripleTriad > 0)
							{
                                fightFactor = 1;
                                if (wise == 3)
								{
									if (winChance < 51)
										winChance -= 100;
									else
										winChance = 145 - winChance;
								}
								else
								{
									if (winChance < 51)
									{
										if (winChance == 50 && (GameRandom.Next16() % (wise + 1)) == 0)
											winChance = 145 - winChance;
										else
											winChance -= 100;
									}
								}
								choiceScore[x, y, i] = winChance * fightFactor;
							}
							else
							{
								if (wise == 3)
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
		bestChoiceScore = MIN_POINTS;
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
		base.StartCoroutine(Thinking(hand));
	}

	private IEnumerator Thinking(Hand hand)
	{
		thinking = true;
		enemyThinkTick = UnityEngine.Random.Range(15, 31);
		Int32 sel = 0;
		hand.Select = sel;
		yield return base.StartCoroutine(Anim.Tick(enemyThinkTick));
		enemyThinkTick = UnityEngine.Random.Range(-2, 21);
		for (Int32 tick = 0; tick < enemyThinkTick; tick++)
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
		thinking = false;
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
					if (Configuration.TetraMaster.TripleTriad >= 2)
					{
						TripleTriadCard baseCardAttacker = TripleTriad.TripleTriadCardStats[yourCard.id];
						TripleTriadCard baseCardDefender = TripleTriad.TripleTriadCardStats[quadMistCard.id];
						if (i == 0 && baseCardAttacker.atk > baseCardDefender.matk)
							flipCount++;
						else if (i == 2 && baseCardAttacker.mdef > baseCardDefender.pdef)
							flipCount++;
						else if (i == 4 && baseCardAttacker.matk > baseCardDefender.atk)
							flipCount++;
						else if (i == 6 && baseCardAttacker.pdef > baseCardDefender.mdef)
							flipCount++;
					}
					else
					{
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
		}
		if (fightCount != 0)
			return -fightCount;
		if (flipCount != 0)
			return flipCount;
		return 0;
	}

	private Int32 CheckEmptySide(Board board, QuadMistCard yourCard, Vector2 origin)
	{
        Int32 valuedefense = 0;
		Int32 side = 0;
        QuadMistCard sideLeft = board[(Int32)(origin.x - 1), (Int32)(origin.y)];
        QuadMistCard sideRight = board[(Int32)(origin.x + 1), (Int32)(origin.y)];
        QuadMistCard sideUp = board[(Int32)(origin.x), (Int32)(origin.y - 1)];
        QuadMistCard sideDown = board[(Int32)(origin.x), (Int32)(origin.y + 1)];
        if (sideLeft == null && origin.x > 0)
        {
            valuedefense += TripleTriad.TripleTriadCardStats[yourCard.id].pdef;
            side++;
        }
        if (sideRight == null && origin.x < (Board.SIZE_X - 1))
        {
            valuedefense += TripleTriad.TripleTriadCardStats[yourCard.id].mdef;
            side++;
        }
        if (sideUp == null && origin.y > 0)
        {
            valuedefense += TripleTriad.TripleTriadCardStats[yourCard.id].atk;
            side++;
        }
        if (sideDown == null && origin.y < (Board.SIZE_Y - 1))
        {
            valuedefense += TripleTriad.TripleTriadCardStats[yourCard.id].matk;
            side++;
        }

        if (side > 0)
			valuedefense = valuedefense / side;
        return valuedefense;
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