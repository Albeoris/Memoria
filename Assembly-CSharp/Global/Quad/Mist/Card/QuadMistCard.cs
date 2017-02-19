using System;
using UnityEngine;
using Object = System.Object;

public class QuadMistCard
{
	public QuadMistCard()
	{
	}

	public QuadMistCard(QuadMistCard card)
	{
		this.id = card.id;
		this.side = card.side;
		this.atk = card.atk;
		this.type = card.type;
		this.pdef = card.pdef;
		this.mdef = card.mdef;
		this.cpoint = card.cpoint;
		this.arrow = card.arrow;
	}

	public void LevelUpInMatch()
	{
		switch (UnityEngine.Random.Range(0, 3))
		{
		case 0:
			if (this.atk != CardPool.GetMaxStatCard((Int32)this.id).atk)
			{
				this.atk = (Byte)(this.atk + 1);
			}
			break;
		case 1:
			if (this.pdef != CardPool.GetMaxStatCard((Int32)this.id).pdef)
			{
				this.pdef = (Byte)(this.pdef + 1);
			}
			break;
		case 2:
			if (this.mdef != CardPool.GetMaxStatCard((Int32)this.id).mdef)
			{
				this.mdef = (Byte)(this.mdef + 1);
			}
			break;
		}
	}

	public void LevelUpInBattle()
	{
		switch (this.type)
		{
		case QuadMistCard.Type.PHYSICAL:
			if (UnityEngine.Random.Range(0, 64) == 0)
			{
				this.type = QuadMistCard.Type.FLEXIABLE;
			}
			break;
		case QuadMistCard.Type.MAGIC:
			if (UnityEngine.Random.Range(0, 64) == 0)
			{
				this.type = QuadMistCard.Type.FLEXIABLE;
			}
			break;
		case QuadMistCard.Type.FLEXIABLE:
			if (UnityEngine.Random.Range(0, 128) == 0)
			{
				this.type = QuadMistCard.Type.ASSAULT;
			}
			break;
		}
	}

	public override String ToString()
	{
		Char c = 'p';
		if (this.type == QuadMistCard.Type.MAGIC)
		{
			c = 'm';
		}
		if (this.type == QuadMistCard.Type.FLEXIABLE)
		{
			c = 'x';
		}
		if (this.type == QuadMistCard.Type.ASSAULT)
		{
			c = 'a';
		}
		return String.Concat(new Object[]
		{
			(this.atk >> 4).ToString("X").ToLower()[0],
			c.ToString(),
			(this.pdef >> 4).ToString("X").ToLower()[0],
			(this.mdef >> 4).ToString("X").ToLower()[0]
		});
	}

	public Boolean IsBlock
	{
		get
		{
			return this.id >= 100;
		}
	}

	public Boolean isTheSameCard(QuadMistCard card)
	{
		global::Debug.Log(String.Concat(new Object[]
		{
			"isTheSameCard 1 current card: id = ",
			this.id,
			", atk = ",
			this.atk,
			", arrow = ",
			this.arrow,
			", type = ",
			this.type,
			", pdef = ",
			this.pdef,
			", mdef = ",
			this.mdef
		}));
		global::Debug.Log(String.Concat(new Object[]
		{
			"isTheSameCard 2 taken   card: id = ",
			card.id,
			", atk = ",
			card.atk,
			", arrow = ",
			card.arrow,
			", type = ",
			card.type,
			", pdef = ",
			card.pdef,
			", mdef = ",
			card.mdef
		}));
		if (this.id == card.id && this.atk == card.atk && this.arrow == card.arrow && this.type == card.type && this.pdef == card.pdef && this.mdef == card.mdef)
		{
			global::Debug.Log("isTheSameCard 3 return true");
			return true;
		}
		global::Debug.Log("isTheSameCard 4 return false");
		return false;
	}

	public Byte id;

	public Byte side;

	public Byte atk;

	public QuadMistCard.Type type;

	public Byte pdef;

	public Byte mdef;

	public Byte cpoint;

	public Byte arrow;

	public enum Type
	{
		PHYSICAL,
		MAGIC,
		FLEXIABLE,
		ASSAULT
	}
}
