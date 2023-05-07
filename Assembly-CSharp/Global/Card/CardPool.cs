using System;
using UnityEngine;
using Memoria;
using Memoria.Data;

public class CardPool : MonoBehaviour
{
	private void Awake()
	{
		CardPool.main = this;
		LoadMetaData();
	}

	public void LoadMetaData()
	{
		Byte[] bytes = AssetManager.LoadBytes(dataPath);
		for (Int32 i = 0; i < CardPool.TOTAL_CARDS; i++)
		{
			cardData[i] = new QuadMistCard();
			cardData[i].id = (TetraMasterCardId)i;
			cardData[i].side = 0;
			cardData[i].atk = bytes[i * 5];
			cardData[i].type = (QuadMistCard.Type)bytes[i * 5 + 1];
			cardData[i].pdef = bytes[i * 5 + 2];
			cardData[i].mdef = bytes[i * 5 + 3];
			cardData[i].cpoint = bytes[i * 5 + 4];
		}
	}

	public static QuadMistCard CreateQuadMistCard(TetraMasterCardId id)
	{
		QuadMistCard newCard = new QuadMistCard(CardPool.GetMaxStatCard(id));
		newCard.atk = (Byte)(newCard.atk / 2 + UnityEngine.Random.Range(0, newCard.atk / 2));
		newCard.pdef = (Byte)(newCard.pdef / 2 + UnityEngine.Random.Range(0, newCard.pdef / 2));
		newCard.mdef = (Byte)(newCard.mdef / 2 + UnityEngine.Random.Range(0, newCard.mdef / 2));
		if (UnityEngine.Random.Range(0, 127) == 0)
			newCard.type = QuadMistCard.Type.FLEXIABLE;
		newCard.arrow = CardPool.RandomCardArrow();
		return newCard;
	}

	private static Byte RandomCardArrow()
	{
		Byte b = 0;
		Int32 num = UnityEngine.Random.Range(0, 100);
		Int32 num2 = 0;
		Int32 i;
		for (i = 0; i < (Int32)CardPool.probability.Length; i++)
		{
			if (num >= num2 && num < num2 + CardPool.probability[i])
			{
				break;
			}
			num2 += CardPool.probability[i];
		}
		Int32[] array = new Int32[]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7
		};
		for (Int32 j = 0; j < 8; j++)
		{
			Int32 num3 = UnityEngine.Random.Range(0, 8);
			Int32 num4 = array[j];
			array[j] = array[num3];
			array[num3] = num4;
		}
		Int32 num5 = 0;
		while (i != 0)
		{
			Int32 num6 = array[num5];
			b = (Byte)(b | (Byte)(1 << num6));
			i--;
			num5++;
		}
		return b;
	}

	public static QuadMistCard GetMaxStatCard(TetraMasterCardId id)
	{
		return CardPool.main.cardData[(Int32)id];
	}

	public static QuadMistCard GetBlockCard(Int32 id)
	{
		return new QuadMistCard
		{
			id = (TetraMasterCardId)(CardPool.TOTAL_CARDS + id)
		};
	}

	public String dataPath = "EmbeddedAsset/QuadMist/MINIGAME_CARD_DATA_ADDRESS";

	public static CardPool main;

	public static Byte TOTAL_CARDS = 100;

	private QuadMistCard[] cardData = new QuadMistCard[CardPool.TOTAL_CARDS];

	private static Int32[] probability = new Int32[]
	{
		1,
		8,
		25,
		31,
		18,
		9,
		5,
		2,
		1
	};
}
