using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Prime;
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

	public static Int32 MiniGame_AwayCard(Int32 cardId, Int32 cardIndex)
	{
		List<QuadMistCard> miniGameCard = FF9StateSystem.MiniGame.SavedData.MiniGameCard;
		QuadMistCard quadMistCard = (QuadMistCard)null;
		Int32 num = 0;
		foreach (QuadMistCard quadMistCard2 in miniGameCard)
		{
			if ((Int32)quadMistCard2.id == cardId)
			{
				if (num == cardIndex)
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

    public static void DiscardUnnecessaryCards()
    {
        List<QuadMistCard> cards = FF9StateSystem.MiniGame.SavedData.MiniGameCard;

        Int32 minDeckSize = Configuration.TetraMaster.DiscardMinDeckSize;
        if (cards.Count <= minDeckSize)
        {
            Log.Warning($"You don't have enough cards to discard. You can change a minimum deck size via Mememoria.ini: [{nameof(Configuration.TetraMaster)}].{nameof(Configuration.TetraMaster.DiscardMinDeckSize)}");
            return;
        }

        Dictionary<Byte, List<QuadMistCard>> cardsById = new Dictionary<Byte, List<QuadMistCard>>();
        Dictionary<Byte, List<QuadMistCard>> cardsByArrow = new Dictionary<Byte, List<QuadMistCard>>();

        foreach (QuadMistCard card in cards)
        {
            List<QuadMistCard> list;

            if (!cardsById.TryGetValue(card.id, out list))
            {
                list = new List<QuadMistCard>();
                cardsById.Add(card.id, list);
            }
            list.Add(card);

            if (!cardsByArrow.TryGetValue(card.arrow, out list))
            {
                list = new List<QuadMistCard>();
                cardsByArrow.Add(card.arrow, list);
            }
            list.Add(card);
        }

        foreach (List<QuadMistCard> list in cardsById.Values)
            list.Sort(CompareCard);
        foreach (List<QuadMistCard> list in cardsByArrow.Values)
            list.Sort(CompareCard);

        Int32 maxSameArrows = Configuration.TetraMaster.DiscardKeepSameArrow;
        Int32 maxSameType = Configuration.TetraMaster.DiscardKeepSameType;

        Log.Message("-------------------");
        Log.Message("Discarding cards...");

        foreach (KeyValuePair<Byte, List<QuadMistCard>> pair in cardsByArrow.OrderByDescending(p => MathEx.BitCount(p.Key)))
        {
            Int32 sameArrows = maxSameArrows;
            List<QuadMistCard> list = pair.Value;

            for (Int32 i = 0; (i < list.Count && i < list.Count - sameArrows); i++)
            {
                QuadMistCard card = list[i];
                if (!CanDiscard(card))
                {
                    sameArrows--;
                    continue;
                }

                List<QuadMistCard> sameCards = cardsById[card.id];
                if (sameCards.Count <= maxSameType)
                {
                    sameArrows--;
                    continue;
                }

                list.RemoveAt(i);
                sameCards.Remove(card);
                cards.Remove(card);
                i--;
                LogDiscardingCard(card);

                if (cards.Count <= minDeckSize)
                    break;
            }

            if (cards.Count <= minDeckSize)
                break;
        }

        foreach (KeyValuePair<Byte, List<QuadMistCard>> pair in cardsById.OrderBy(p => p.Key))
        {
            Int32 sameType = maxSameType;

            List<QuadMistCard> list = pair.Value;

            for (Int32 i = 0; (i < list.Count && i < list.Count - sameType); i++)
            {
                QuadMistCard card = list[i];
                if (!CanDiscard(card))
                {
                    sameType--;
                    continue;
                }

                List<QuadMistCard> arrowCards = cardsByArrow[card.arrow];
                if (arrowCards.Count <= maxSameArrows)
                {
                    sameType--;
                    continue;
                }

                list.RemoveAt(i);
                arrowCards.Remove(card);
                cards.Remove(card);
                i--;
                LogDiscardingCard(card);

                if (cards.Count <= minDeckSize)
                    break;
            }

            if (cards.Count <= minDeckSize)
                break;
        }

        Log.Message("-------------------");
    }

    private static void LogDiscardingCard(QuadMistCard card)
    {
        String cardName = FF9TextTool.CardName(card.id);
        String displayInfo = card.ToString();
        Int32 arrowCount = MathEx.BitCount(card.arrow);

        Log.Message($"Discard the card: {cardName} ({displayInfo}, {arrowCount} arrows) [Id: {card.id}, Type: {card.type}, Attack: {card.atk}, P.Def: {card.pdef}, M.Def: {card.mdef}]");
    }

    private static Boolean CanDiscard(QuadMistCard card)
    {
        if (Configuration.TetraMaster.DiscardExclusions.Contains(card.id))
            return false;

        if (card.type == QuadMistCard.Type.ASSAULT && !Configuration.TetraMaster.DiscardAssaultCards)
            return false;
        if (card.type == QuadMistCard.Type.FLEXIABLE && !Configuration.TetraMaster.DiscardFlexibleCards)
            return false;

        if (card.atk > Configuration.TetraMaster.DiscardMaxAttack)
            return false;
        if (card.pdef > Configuration.TetraMaster.DiscardMaxPDef)
            return false;
        if (card.mdef > Configuration.TetraMaster.DiscardMaxMDef)
            return false;

        if (card.atk + card.pdef + card.mdef >= Configuration.TetraMaster.DiscardMaxSum)
            return false;

        return true;
    }

    private static Int32 CompareCardReverse(QuadMistCard x, QuadMistCard y)
    {
        return CompareCard(x, y) * -1;
    }

    private static Int32 CompareCard(QuadMistCard x, QuadMistCard y)
    {
        if (x.cpoint > y.cpoint)
            return 1;
        if (y.cpoint > x.cpoint)
            return -1;

        if (x.type == QuadMistCard.Type.ASSAULT && y.type != QuadMistCard.Type.ASSAULT)
            return 1;
        if (y.type == QuadMistCard.Type.ASSAULT && x.type != QuadMistCard.Type.ASSAULT)
            return -1;

        return (x.atk + x.pdef + x.mdef) * (x.type >= QuadMistCard.Type.FLEXIABLE ? 1.5 : 1).CompareTo(
                   (y.atk + y.pdef + y.mdef) * (y.type >= QuadMistCard.Type.FLEXIABLE ? 1.5 : 1));
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
