using System;
using UnityEngine;
using Memoria.Data;
using System.Collections.Generic;
using System.Linq;
using Memoria;
using Memoria.Prime;
using NCalc;

public class EnemyData : MonoBehaviour
{
	private void Awake()
	{
		Load();
		EnemyData.Main = this;
	}

	private void Load()
	{
		Byte[] stageData = AssetManager.LoadBytes(stageDataPath);
		Int32 miniGameArg = FF9StateSystem.Common.FF9.miniGameArg;
		cardLevel = stageData[miniGameArg * 2];
		Wise = stageData[miniGameArg * 2 + 1];
		enemyData = AssetManager.LoadBytes(cardLevelDataPath);
	}

	public TetraMasterCardId GetCardID()
	{
		Int32 rnd = UnityEngine.Random.Range(0, 256);
		Int32 cardIndex = 15;

        for (Int32 i = probability.Length - 1; i >= 0; i--)
		{
            Boolean ProcCard = rnd < probability[i];
            if (!String.IsNullOrEmpty(Configuration.TetraMaster.FormulaProbabilityCards))
            {
                QuadMistCard quadMistCard = CardPool.CreateQuadMistCard((TetraMasterCardId)enemyData[cardLevel * 16 + cardIndex]);
                Expression e = new Expression(Configuration.TetraMaster.FormulaProbabilityCards);
                e.Parameters["CardRank"] = TripleTriad.TripleTriadCardStats[(TetraMasterCardId)enemyData[cardLevel * 16 + i]].Rank;
                e.Parameters["CardId"] = quadMistCard.id;
                e.Parameters["ProbabilityCard"] = probability[i];
                e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                ProcCard = NCalcUtility.EvaluateNCalcCondition(e.Evaluate());
            }
            if (!ProcCard)
				break;
			
			cardIndex = i;
		}
        return (TetraMasterCardId)enemyData[cardLevel * 16 + cardIndex];
	}

    public void Initialize(Hand hand)
	{
		hand.Clear();
        if (Configuration.TetraMaster.PreventDuplicateCard > 0)
		{
            List<TetraMasterCardId> ListIdCard = new List<TetraMasterCardId>();
            for (Int32 i = 0; i < 16; i++) // [DV] Add every cards from enemy's deck in a list
            {
                ListIdCard.Add((TetraMasterCardId)enemyData[cardLevel * 16 + i]);
            }
            ListIdCard.Sort();
            List<TetraMasterCardId> ListNoDuplicateIdCard = ListIdCard.Distinct().ToList(); // [DV] Sort it by ID (from high to low)
            if (Configuration.TetraMaster.TripleTriad >= 2)
            {
                ListNoDuplicateIdCard = ListNoDuplicateIdCard.OrderBy(card => TripleTriad.TripleTriadCardStats[card].Rank).ToList(); // [DV] Sort it by rank (from high to low)
            }
            Log.Message("probability.Length = " + probability.Length);
            for (Int32 i = ListNoDuplicateIdCard.Count - 1; i > 0; i--)
            {
				if (hand.Count >= 5)
					break;

                Boolean ProcCard = true;
                QuadMistCard quadMistCard = CardPool.CreateQuadMistCard(ListNoDuplicateIdCard[i]);
                int probabilityindex = probability.Length - i;
                if (probabilityindex < 0)
                    probabilityindex = 0;
                if (probabilityindex < 15)
                {
                    Log.Message("probabilityindex = " + probabilityindex);
                    ProcCard = UnityEngine.Random.Range(0, 256) < probability[probabilityindex];
                    Log.Message("probability[probabilityindex] = " + probability[probabilityindex]);
                    if (!String.IsNullOrEmpty(Configuration.TetraMaster.FormulaProbabilityCards))
                    {
                        Expression e = new Expression(Configuration.TetraMaster.FormulaProbabilityCards);
                        e.Parameters["CardRank"] = TripleTriad.TripleTriadCardStats[ListNoDuplicateIdCard[i]].Rank;
                        e.Parameters["CardId"] = quadMistCard.id;
                        e.Parameters["ProbabilityCard"] = probability[probabilityindex];
                        e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        ProcCard = NCalcUtility.EvaluateNCalcCondition(e.Evaluate());
                    }
                }
                if (!ProcCard)
					continue;

                quadMistCard.side = 1;
                hand.Add(quadMistCard);
                if (Configuration.TetraMaster.UniqueCard.Contains((Int32)quadMistCard.id))
                    ListNoDuplicateIdCard.Remove(ListNoDuplicateIdCard[i]);
            }
            while (hand.Count < 5) // [DV] If the card game is not fully completed with "PreventDuplicateCard", fill the hand based on "ListNoDuplicateIdCard".
            {
                QuadMistCard quadMistCard = CardPool.CreateQuadMistCard(ListNoDuplicateIdCard[UnityEngine.Random.Range(0, ListNoDuplicateIdCard.Count - 1)]);
                quadMistCard.side = 1;
                hand.Add(quadMistCard);
            }
        }
		else
		{
            for (Int32 i = 0; i < 5; i++)
            {
                QuadMistCard quadMistCard = CardPool.CreateQuadMistCard(GetCardID());
                quadMistCard.side = 1;
                hand.Add(quadMistCard);
            }
        }
	}

	public static void Setup(Hand hand)
	{
        Main.Initialize(hand);
    }

	public static void RestorePlayerLostCard(Hand hand, Int32 cardArrayIndex, QuadMistCard lostCard)
	{
		hand.ReplaceCard(cardArrayIndex, lostCard);
	}

	public static EnemyData Main;

	public Int32 Wise;

	private String cardLevelDataPath = "EmbeddedAsset/QuadMist/MINIGAME_CARD_LEVEL_ADDRESS";

	private String stageDataPath = "EmbeddedAsset/QuadMist/MINIGAME_STAGE_ADDRESS";

	private Int32 cardLevel;

	private Int32[] probability = Configuration.TetraMaster.ValueProbabilityCards.Split(',').Select(p => Convert.ToInt32(p)).ToArray();

    private Byte[] enemyData;
}
