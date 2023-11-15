using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Data;
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
			QuadMistDatabase.instance.data = new FF9SAVE_MINIGAME();
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
				if (QuadMistDatabase.instance.data.MiniGameCard.Count < Configuration.TetraMaster.MaxCardCount)
					QuadMistDatabase.instance.data.MiniGameCard.Add(CardPool.CreateQuadMistCard((TetraMasterCardId)UnityEngine.Random.Range(0, CardPool.TOTAL_CARDS)));
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
		return QuadMistDatabase.instance.data.MiniGameCard.Count(deckCard => deckCard.id == card.id);
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

	public static Int32 Remove(TetraMasterCardId cardId, Int32 count)
	{
		Int32 countDiscarded = 0;
		while (count > 0 && MiniGame_AwayCard(cardId, 0))
		{
			countDiscarded++;
			count--;
		}
		return countDiscarded;
	}

	public static Int16 WinCount
	{
		get => QuadMistDatabase.instance.data.sWin;
		set => QuadMistDatabase.instance.data.sWin = value;
	}

	public static Int16 LoseCount
	{
		get => QuadMistDatabase.instance.data.sLose;
		set => QuadMistDatabase.instance.data.sLose = value;
	}

	public static Int16 DrawCount
	{
		get => QuadMistDatabase.instance.data.sDraw;
		set => QuadMistDatabase.instance.data.sDraw = value;
	}

	public static CardIcon.Attribute MiniGame_GetCardAttribute(Int32 ID)
	{
		return CardIcon.GetCardAttribute(ID);
	}

	public static Int32 MiniGame_GetCardKindCount()
	{
		HashSet<TetraMasterCardId> kindOwned = new HashSet<TetraMasterCardId>();
		foreach (QuadMistCard quadMistCard in FF9StateSystem.MiniGame.SavedData.MiniGameCard)
			kindOwned.Add(quadMistCard.id);
		return kindOwned.Count;
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

	public static QuadMistCard MiniGame_GetCardInfoPtr(TetraMasterCardId ID, Int32 Offset)
	{
		Int32 num = 0;
		foreach (QuadMistCard quadMistCard in FF9StateSystem.MiniGame.SavedData.MiniGameCard)
		{
			if (quadMistCard.id == ID)
			{
				if (Offset == num)
					return quadMistCard;
				num++;
			}
		}
		return null;
	}

	public static Int32 MiniGame_GetCardCount(TetraMasterCardId ID)
	{
		return FF9StateSystem.MiniGame.SavedData.MiniGameCard.Count(deckCard => deckCard.id == ID);
	}

	public static Boolean MiniGame_AwayCard(TetraMasterCardId cardId, Int32 cardIndex)
	{
		List<QuadMistCard> miniGameCard = FF9StateSystem.MiniGame.SavedData.MiniGameCard;
		QuadMistCard removedCard = null;
		Int32 num = 0;
		foreach (QuadMistCard card in miniGameCard)
		{
			if (card.id == cardId)
			{
				if (num == cardIndex)
				{
					removedCard = card;
					break;
				}
				num++;
			}
		}
		if (removedCard != null)
		{
			miniGameCard.Remove(removedCard);
			FF9StateSystem.MiniGame.SavedData.MiniGameCard = miniGameCard;
			return true;
		}
		return false;
	}

	public static Int32 MiniGame_GetPlayerPoints()
	{
		Byte[] typePtsWorth = new Byte[] { 0, 0, 1, 2 };
		HashSet<Byte> arrowPatternUsed = new HashSet<Byte>();
		Int32 typePts = 0;
		Int32 arrowPts = 0;
		Int32 idPts = QuadMistDatabase.MiniGame_GetCardKindCount() * 10;
		foreach (QuadMistCard card in FF9StateSystem.MiniGame.SavedData.MiniGameCard)
		{
			if (!arrowPatternUsed.Contains(card.arrow))
				arrowPts += 5;
			arrowPatternUsed.Add(card.arrow);
			typePts += typePtsWorth[(Int32)card.type];
		}
		return idPts + typePts + arrowPts;
	}

	public static Int32 MiniGame_GetCollectorLevel()
	{
		Int16[] levelThresholds = new Int16[]
		{
			0,
			300,
			400,
			500,
			600,
			700,
			800,
			900,
			1000,
			1100,
			1200,
			1250,
			1300,
			1320,
			1330,
			1340,
			1350,
			1360,
			1370,
			1380,
			1390,
			1400,
			1420,
			1470,
			1510,
			1550,
			1600,
			1650,
			1680,
			1690,
			1698,
			1700
		};
		Int32 points = MiniGame_GetPlayerPoints();
		for (Int32 i = 1; i < FF9SAVE_MINIGAME.CollectorLevelMax; i++)
			if (points < levelThresholds[i])
				return i - 1;
		return FF9SAVE_MINIGAME.CollectorLevelMax - 1;
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

		Dictionary<TetraMasterCardId, List<QuadMistCard>> cardsById = new Dictionary<TetraMasterCardId, List<QuadMistCard>>();
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

		Int32 maxSameArrows = Configuration.TetraMaster.TripleTriad == 0 ? Configuration.TetraMaster.DiscardKeepSameArrow : 1;
		Int32 maxSameType = Configuration.TetraMaster.DiscardKeepSameType;

		Log.Message("-------------------");
		Log.Message("Discarding cards...");

		if (Configuration.TetraMaster.TripleTriad > 0)
		{
			foreach (KeyValuePair<Byte, List<QuadMistCard>> pair in cardsByArrow.OrderBy(p => MathEx.BitCount(p.Key)))
			{
				Int32 sameArrows = maxSameArrows;
				List<QuadMistCard> list = pair.Value;

				for (Int32 i = 0; (i < list.Count); i++)
				{
					QuadMistCard card = list[i];
					if (!CanDiscard(card))
					{
						sameArrows--;
						continue;
					}

					List<QuadMistCard> sameCards = cardsById[card.id];
					if (sameCards.Count <= (Configuration.TetraMaster.TripleTriad < 2 ? Configuration.TetraMaster.DiscardKeepSameArrow : 1))
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
		}
		else
		{
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

			foreach (KeyValuePair<TetraMasterCardId, List<QuadMistCard>> pair in cardsById.OrderBy(p => p.Key))
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
		if (Configuration.TetraMaster.DiscardExclusions.Contains((Int32)card.id + 1))
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

	public static Int32 MiniGame_SetCard(TetraMasterCardId cardId)
	{
		if (QuadMistDatabase.MiniGame_GetAllCardCount() >= Configuration.TetraMaster.MaxCardCount)
			return 0;
		QuadMistCard item = CardPool.CreateQuadMistCard(cardId);
		FF9StateSystem.MiniGame.SavedData.MiniGameCard.Add(item);
		return 1;
	}

	public static void MiniGame_ContinueInit()
	{
		QuadMistDatabase.MiniGame_LastBattleResult = MINIGAME_LASTBATTLE_LOSE;
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
			QuadMistCard deepCopy = new QuadMistCard();
			deepCopy.id = quadMistCard.id;
			deepCopy.side = 0;
			deepCopy.atk = quadMistCard.atk;
			deepCopy.type = quadMistCard.type;
			deepCopy.pdef = quadMistCard.pdef;
			deepCopy.mdef = quadMistCard.mdef;
			deepCopy.cpoint = quadMistCard.cpoint;
			deepCopy.arrow = quadMistCard.arrow;
			ff9SAVE_MINIGAME.MiniGameCard.Add(deepCopy);
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
