using System;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
	private void Start()
	{
		for (Int32 i = 0; i < (Int32)this.cards.Length; i++)
		{
			this.cards[i] = new List<QuadMistCard>();
		}
	}

	public void CreateCards()
	{
		this.Clear();
		List<QuadMistCard> cardList = QuadMistDatabase.GetCardList();
		foreach (QuadMistCard c in cardList)
		{
			this.Add(c);
		}
	}

	public void Add(QuadMistCard c)
	{
		this.cards[(Int32)c.id].Add(c);
	}

	public void Remove(QuadMistCard c)
	{
		this.cards[(Int32)c.id].Remove(c);
	}

	public void Clear()
	{
		for (Int32 i = 0; i < (Int32)this.cards.Length; i++)
		{
			this.cards[i].Clear();
		}
	}

	public Int32 Count(Int32 id)
	{
		return this.cards[id].Count;
	}

	public List<QuadMistCard> GetCardsWithID(Int32 id)
	{
		return this.cards[id];
	}

	public List<QuadMistCard>[] cards = new List<QuadMistCard>[100];
}
