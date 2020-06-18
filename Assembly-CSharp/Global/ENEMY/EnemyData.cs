using System;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
	private void Awake()
	{
		this.Load();
		EnemyData.Main = this;
	}

	private void Load()
	{
		String[] cardStratInfo;
		Byte[] bytes = AssetManager.LoadBytes(this.stageDataPath, out cardStratInfo, false);
		Int32 miniGameArg = (Int32)FF9StateSystem.Common.FF9.miniGameArg;
		Char c = (Char)bytes[miniGameArg * 2];
		Int32 num = (Int32)c;
		c = (Char)bytes[miniGameArg * 2 + 1];
		Int32 wise = (Int32)c;
		this.cardLevel = num;
		this.Wise = wise;
		String[] cardListInfo;
		this.enemyData = AssetManager.LoadBytes(this.cardLevelDataPath, out cardListInfo, false);
	}

	public Int32 GetCardID()
	{
		Int32 rnd = UnityEngine.Random.Range(0, 256);
		Int32 cardIndex = 15;
		
		for (Int32 i = this.probability.Length - 1; i >= 0; i--)
		{
			if (rnd >= this.probability[i])
				break;
			
			cardIndex = i;
		}
		return this.enemyData[this.cardLevel * 16 + cardIndex];
	}

	public void Initialize(Hand hand)
	{
		hand.Clear();
		for (Int32 i = 0; i < 5; i++)
		{
			QuadMistCard quadMistCard = CardPool.CreateQuadMistCard(this.GetCardID());
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
