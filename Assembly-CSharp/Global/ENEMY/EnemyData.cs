using Memoria.Data;
using System;
using UnityEngine;

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
			if (rnd >= probability[i])
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
			QuadMistCard quadMistCard = CardPool.CreateQuadMistCard(GetCardID());
			quadMistCard.side = 1;
			hand.Add(quadMistCard);
		}
	}

	public static void Setup(Hand hand)
	{
		EnemyData.Main.Initialize(hand);
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

	private Int32[] probability = new Int32[]
	{
		20,
		40,
		60,
		80,
		100,
		120,
		140,
		160,
		180,
		200,
		220,
		240,
		252,
		254,
		255
	};

	private Byte[] enemyData;
}
