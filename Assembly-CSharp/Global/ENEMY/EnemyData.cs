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

	public TetraMasterCardId GetCardID(Hand hand)
	{
        Int32 cardIndex = 15;
        Int32 Random = UnityEngine.Random.Range(0, 256);
        List<TetraMasterCardId> enemyHand = new List<TetraMasterCardId>();
        for (Int32 i = 0; i < hand.Count; i++) // [DV] Add every cards from enemy's deck in a list
        {
            enemyHand.Add(hand[i].id);
        }
        for (Int32 i = probability.Length - 1; i >= 0; i--)
		{
            Boolean ProcCard = Random >= probability[i];
            QuadMistCard quadMistCard = CardPool.CreateQuadMistCard((TetraMasterCardId)enemyData[cardLevel * 16 + i]);
            if (!String.IsNullOrEmpty(Configuration.TetraMaster.FormulaProbabilityCards))
            { 
                Expression e = new Expression(Configuration.TetraMaster.FormulaProbabilityCards);
                e.Parameters["CardRank"] = TripleTriad.TripleTriadCardStats[(TetraMasterCardId)enemyData[cardLevel * 16 + i]].rank;
                e.Parameters["CardId"] = quadMistCard.id;
                e.Parameters["ProbabilityCard"] = probability[i];
                e.Parameters["Random"] = Random;
                e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                ProcCard = NCalcUtility.EvaluateNCalcCondition(e.Evaluate());
            }
            if (Configuration.TetraMaster.UniqueCard.Contains((Int32)quadMistCard.id) && (enemyHand.Contains(quadMistCard.id) || QuadMistDatabase.MiniGame_GetCardCount(quadMistCard.id) != 0))
            {
                continue;
            }

            if (Configuration.TetraMaster.PreventDuplicateCard > 0 && enemyHand.Contains(quadMistCard.id))
                continue;

            if (ProcCard)
                break;

			cardIndex = i;
		}
        return (TetraMasterCardId)enemyData[cardLevel * 16 + cardIndex];
	}

    public void Initialize(Hand hand)
	{
		hand.Clear();      
        for (Int32 i = 0; i < 5; i++)
        {
            QuadMistCard quadMistCard = CardPool.CreateQuadMistCard(GetCardID(hand));
            quadMistCard.side = 1;
            hand.Add(quadMistCard);
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
