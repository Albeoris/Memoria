using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadMistDatabase : MonoBehaviour
{
	private void Awake()
	{
		QuadMistDatabase.instance = this;
	}

	private void Start()
	{
		QuadMistDatabase.LoadData();
	}

	public static void LoadData()
	{
		QuadMistDatabase.instance.data = QuadMistDatabase.ReadDataFromSharedData();
		if (QuadMistDatabase.instance.data == null)
		{
			QuadMistDatabase.instance.data = new FF9SAVE_MINIGAME();
		}
	}

	public static void SaveData()
	{
		QuadMistDatabase.WriteCurrentDataToSharedData(QuadMistDatabase.instance.data);
	}

	public static void CreateDataIfLessThanFive()
	{
		if (QuadMistDatabase.instance.data.MiniGameCard.Count < 5)
		{
			for (Int32 i = 0; i < 8; i++)
			{
				if (QuadMistDatabase.instance.data.MiniGameCard.Count < 100)
				{
					QuadMistDatabase.instance.data.MiniGameCard.Add(CardPool.CreateQuadMistCard(UnityEngine.Random.Range(0, 100)));
				}
			}
			QuadMistDatabase.SaveData();
		}
	}

	public static List<QuadMistCard> GetCardList()
	{
		return QuadMistDatabase.instance.data.MiniGameCard;
	}

	public static void SetCardList(List<QuadMistCard> cards)
	{
		QuadMistDatabase.instance.data.MiniGameCard = cards;
	}

	public static Int32 GetCardCount(QuadMistCard card)
	{
		Int32 num = 0;
		foreach (QuadMistCard quadMistCard in QuadMistDatabase.instance.data.MiniGameCard)
		{
			if (quadMistCard.id == card.id)
			{
				num++;
			}
		}
		return num;
	}

	public static void Add(QuadMistCard card)
	{
		QuadMistDatabase.instance.data.MiniGameCard.Add(card);
	}

	public static void Remove(QuadMistCard card)
	{
		foreach (QuadMistCard quadMistCard in QuadMistDatabase.instance.data.MiniGameCard)
		{
			if (quadMistCard.id == card.id)
			{
				QuadMistDatabase.instance.data.MiniGameCard.Remove(quadMistCard);
				break;
			}
		}
	}

	public static Int16 GetWinCount()
	{
		return QuadMistDatabase.instance.data.sWin;
	}

	public static void SetWinCount(Int16 count)
	{
		QuadMistDatabase.instance.data.sWin = count;
	}

	public static Int16 GetLoseCount()
	{
		return QuadMistDatabase.instance.data.sLose;
	}

	public static void SetLoseCount(Int16 count)
	{
		QuadMistDatabase.instance.data.sLose = count;
	}

	public static Int16 GetDrawCount()
	{
		return QuadMistDatabase.instance.data.sDraw;
	}

	public static void SetDrawCount(Int16 count)
	{
		QuadMistDatabase.instance.data.sDraw = count;
	}

	public static CardIcon.Attribute MiniGame_GetCardAttribute(Int32 ID)
	{
		return CardIcon.GetCardAttribute(ID);
	}

	public static List<QuadMistCard> MiniGame_GetCardBinPtr()
	{
		return FF9StateSystem.MiniGame.SavedData.MiniGameCard;
	}

	public static Int32 MiniGame_GetCardKindCount()
	{
		Int32[] array = new Int32[100];
		for (Int32 i = 0; i < 100; i++)
		{
			array[i] = 0;
		}
		foreach (QuadMistCard quadMistCard in FF9StateSystem.MiniGame.SavedData.MiniGameCard)
		{
			array[(Int32)quadMistCard.id]++;
		}
		Int32 num = 0;
		for (Int32 j = 0; j < 100; j++)
		{
			if (array[j] != 0)
			{
				num++;
			}
		}
		return num;
	}

	public static Int32 MiniGame_GetAllCardCount()
	{
		return FF9StateSystem.MiniGame.SavedData.MiniGameCard.Count;
	}

	public static Int32 MiniGame_GetWinCount()
	{
		return (Int32)FF9StateSystem.MiniGame.SavedData.sWin;
	}

	public static Int32 MiniGame_GetLoseCount()
	{
		return (Int32)FF9StateSystem.MiniGame.SavedData.sLose;
	}

	public static Int32 MiniGame_GetDrawCount()
	{
		return (Int32)FF9StateSystem.MiniGame.SavedData.sDraw;
	}

	public static QuadMistCard MiniGame_GetCardInfoPtr(Int32 ID, Int32 Offset)
	{
		Int32 num = 0;
		foreach (QuadMistCard quadMistCard in FF9StateSystem.MiniGame.SavedData.MiniGameCard)
		{
			if ((Int32)quadMistCard.id == ID)
			{
				if (Offset == num)
				{
					return quadMistCard;
				}
				num++;
			}
		}
		return (QuadMistCard)null;
	}

	public static Int32 MiniGame_GetCardCount(Int32 ID)
	{
		Int32 num = 0;
		foreach (QuadMistCard quadMistCard in FF9StateSystem.MiniGame.SavedData.MiniGameCard)
		{
			if ((Int32)quadMistCard.id == ID)
			{
				num++;
			}
		}
		return num;
	}

	public static Int32 MiniGame_AwayCard(Int32 ID, Int32 offset)
	{
		List<QuadMistCard> miniGameCard = FF9StateSystem.MiniGame.SavedData.MiniGameCard;
		QuadMistCard quadMistCard = (QuadMistCard)null;
		Int32 num = 0;
		foreach (QuadMistCard quadMistCard2 in miniGameCard)
		{
			if ((Int32)quadMistCard2.id == ID)
			{
				if (num == offset)
				{
					quadMistCard = quadMistCard2;
					break;
				}
				num++;
			}
		}
		if (quadMistCard != null)
		{
			miniGameCard.Remove(quadMistCard);
			FF9StateSystem.MiniGame.SavedData.MiniGameCard = miniGameCard;
			return 0;
		}
		return -1;
	}

	public static void MiniGame_AwayAllCard()
	{
		FF9StateSystem.MiniGame.SavedData.MiniGameCard.Clear();
	}

	public static Int32 MiniGame_SetCard(Int32 ID)
	{
		if (QuadMistDatabase.MiniGame_GetAllCardCount() >= 100)
		{
			return -1;
		}
		QuadMistCard item = CardPool.CreateQuadMistCard(ID);
		FF9StateSystem.MiniGame.SavedData.MiniGameCard.Add(item);
		if (QuadMistDatabase.MiniGame_GetCardCount(ID) == 1)
		{
			return 1;
		}
		return 0;
	}

	public static void MiniGame_ContinueInit()
	{
		QuadMistDatabase.MiniGame_LastBattleResult = 1;
	}

	public static void MiniGame_SetLastBattleResult(Int32 Result)
	{
		QuadMistDatabase.MiniGame_LastBattleResult = Result;
	}

	public static Int32 MiniGame_GetLastBattleResult()
	{
		return QuadMistDatabase.MiniGame_LastBattleResult;
	}

	private static FF9SAVE_MINIGAME ReadDataFromSharedData()
	{
		return QuadMistDatabase.DoDeepCopyWidhCardsAlwaysBeYours(FF9StateSystem.MiniGame.SavedData);
	}

	private static void WriteCurrentDataToSharedData(FF9SAVE_MINIGAME data)
	{
		FF9StateSystem.MiniGame.SavedData = QuadMistDatabase.DoDeepCopyWidhCardsAlwaysBeYours(data);
	}

	private static FF9SAVE_MINIGAME DoDeepCopyWidhCardsAlwaysBeYours(FF9SAVE_MINIGAME originalData)
	{
		FF9SAVE_MINIGAME ff9SAVE_MINIGAME = new FF9SAVE_MINIGAME();
		ff9SAVE_MINIGAME.sWin = originalData.sWin;
		ff9SAVE_MINIGAME.sLose = originalData.sLose;
		ff9SAVE_MINIGAME.sDraw = originalData.sDraw;
		foreach (QuadMistCard quadMistCard in originalData.MiniGameCard)
		{
			QuadMistCard quadMistCard2 = new QuadMistCard();
			quadMistCard2.id = quadMistCard.id;
			quadMistCard2.side = 0;
			quadMistCard2.atk = quadMistCard.atk;
			quadMistCard2.type = quadMistCard.type;
			quadMistCard2.pdef = quadMistCard.pdef;
			quadMistCard2.mdef = quadMistCard.mdef;
			quadMistCard2.cpoint = quadMistCard.cpoint;
			quadMistCard2.arrow = quadMistCard.arrow;
			ff9SAVE_MINIGAME.MiniGameCard.Add(quadMistCard2);
		}
		return ff9SAVE_MINIGAME;
	}

	public const Int32 MINIGAME_CARDMAX = 100;

	public const Int32 MINIGAME_NO_CARD = 255;

	public const Int32 MINIGAME_SETCARD_FAIL = -1;

	public const Int32 MINIGAME_SETCARD_NOMAL = 0;

	public const Int32 MINIGAME_SETCARD_NEW = 1;

	public const Int32 MINIGAME_LASTBATTLE_WIN = 0;

	public const Int32 MINIGAME_LASTBATTLE_LOSE = 1;

	public const Int32 MINIGAME_LASTBATTLE_DRAW = 2;

	private static QuadMistDatabase instance;

	private FF9SAVE_MINIGAME data;

	public static Int32 MiniGame_LastBattleResult = 1;
}
