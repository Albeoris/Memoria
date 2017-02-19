using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	public void InitResources()
	{
		for (Int32 i = 0; i < Board.SIZE_X * Board.SIZE_Y; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.cardPrefab);
			this.field[i] = gameObject.GetComponent<QuadMistCardUI>();
			gameObject.name = "fileCard" + ((i > 9) ? String.Empty : "0") + i;
			gameObject.transform.parent = base.transform;
			gameObject.SetActive(false);
		}
	}

	private void Start()
	{
		for (Int32 i = 0; i < Board.SIZE_X * Board.SIZE_Y; i++)
		{
			Int32 num = i % 4;
			Int32 num2 = i / 4;
			this.field[i].gameObject.SetActive(false);
			this.field[i].transform.localPosition = new Vector3((Single)num * (QuadMistCardUI.SIZE_W + (Single)Board.FIELD_LINE_W * 0.01f), -((Single)num2 * (QuadMistCardUI.SIZE_H + (Single)Board.FIELD_LINE_H * 0.01f)), 0f);
			this.field[i].name = num + "," + num2;
		}
		this.background.color = new Color(1f, 1f, 1f, 0f);
		this.hash = new Dictionary<QuadMistCard, QuadMistCardUI>();
	}

	public QuadMistCard this[Single i, Single j]
	{
		get
		{
			return this[(Int32)i, (Int32)j];
		}
		set
		{
			this[(Int32)i, (Int32)j] = value;
		}
	}

	public QuadMistCard this[Int32 i, Int32 j]
	{
		get
		{
			if (i >= Board.SIZE_X || i < 0)
			{
				return (QuadMistCard)null;
			}
			if (j >= Board.SIZE_Y || j < 0)
			{
				return (QuadMistCard)null;
			}
			return this[i + j * 4];
		}
		set
		{
			this[i + j * 4] = value;
		}
	}

	public QuadMistCard this[Int32 i]
	{
		get
		{
			return this.field[i].Data;
		}
		set
		{
			if (this.field[i].Data != null)
			{
				this.hash.Remove(this.field[i].Data);
			}
			this.field[i].Data = value;
			if (value != null)
			{
				this.hash.Add(value, this.field[i]);
			}
		}
	}

	public Boolean IsFree(Int32 i, Int32 j)
	{
		return this.IsFree(i + j * 4);
	}

	public Boolean IsFree(Int32 i)
	{
		return !this.field[i].gameObject.activeSelf;
	}

	public void PlaceCursor(Int32 i, Int32 j)
	{
		if (i < 0 || i >= Board.SIZE_X || j < 0 || j >= Board.SIZE_Y)
		{
			this.cursor.Active = false;
		}
		else
		{
			this.cursor.Active = true;
			this.cursor.transform.position = this.field[i + j * 4].transform.position + new Vector3(0f, 0f, -1f);
		}
	}

	public QuadMistCardUI GetCardUI(QuadMistCard card)
	{
		QuadMistCardUI result = (QuadMistCardUI)null;
		this.hash.TryGetValue(card, out result);
		return result;
	}

	public QuadMistCardUI GetCardUI(Int32 i, Int32 j)
	{
		if (i >= Board.SIZE_X || i < 0)
		{
			return (QuadMistCardUI)null;
		}
		if (j >= Board.SIZE_Y || j < 0)
		{
			return (QuadMistCardUI)null;
		}
		return this.field[i + j * 4];
	}

	public QuadMistCardUI GetCardUI(Single i, Single j)
	{
		return this.GetCardUI((Int32)i, (Int32)j);
	}

	public QuadMistCardUI GetCardUI(Int32 i)
	{
		if (i < 0 || i >= (Int32)this.field.Length)
		{
			return (QuadMistCardUI)null;
		}
		return this.field[i];
	}

	public Vector2 GetCardLocation(QuadMistCard card)
	{
		for (Int32 i = 0; i < Board.SIZE_X; i++)
		{
			for (Int32 j = 0; j < Board.SIZE_Y; j++)
			{
				if (card == this[i, j])
				{
					return new Vector2((Single)i, (Single)j);
				}
			}
		}
		return new Vector2(-1f, -1f);
	}

	public QuadMistCard[] GetAdjacentCards(QuadMistCard card)
	{
		Vector2 cardLocation = this.GetCardLocation(card);
		return this.GetAdjacentCards((Int32)cardLocation.x, (Int32)cardLocation.y);
	}

	public QuadMistCard[] GetAdjacentCards(Int32 x, Int32 y)
	{
		QuadMistCard[] array = new QuadMistCard[CardArrow.MAX_ARROWNUM];
		for (Int32 i = 0; i < CardArrow.MAX_ARROWNUM; i++)
		{
			Vector3 vector = CardArrow.ToOffset((CardArrow.Type)i);
			QuadMistCard quadMistCard = this[(Int32)((Single)x + vector.x), (Int32)((Single)y + vector.y)];
			if (quadMistCard != null && !quadMistCard.IsBlock)
			{
				array[i] = quadMistCard;
			}
			else
			{
				array[i] = (QuadMistCard)null;
			}
		}
		return array;
	}

	public Vector2 GetVectorByWorldPoint(Vector3 worldPoint, Boolean checkForFree = false)
	{
		Int32 indexByWorldPoint = this.GetIndexByWorldPoint(worldPoint, checkForFree);
		if (indexByWorldPoint == -1)
		{
			return new Vector2(-100f, -100f);
		}
		return new Vector2((Single)(indexByWorldPoint % 4), (Single)(indexByWorldPoint / 4));
	}

	public Int32 GetIndexByWorldPoint(Vector3 worldPoint, Boolean checkForFree = false)
	{
		for (Int32 i = 0; i < Board.SIZE_X * Board.SIZE_Y; i++)
		{
			if ((!checkForFree || this.IsFree(i)) && this.field[i].Contains(worldPoint))
			{
				return i;
			}
		}
		return -1;
	}

	public void Clear()
	{
		for (Int32 i = 0; i < Board.SIZE_X * Board.SIZE_Y; i++)
		{
			this[i] = (QuadMistCard)null;
		}
	}

	public IEnumerator FadeInBoard()
	{
		Single alpha = 0f;
		this.background.color = new Color(1f, 1f, 1f, 0f);
		this.background.gameObject.SetActive(true);
		for (Int32 tick = 0; tick <= 32; tick++)
		{
			alpha = (Single)(tick * 8) / 255f;
			this.background.color = new Color(1f, 1f, 1f, alpha);
			yield return base.StartCoroutine(Anim.Tick());
		}
		this.background.color = new Color(1f, 1f, 1f, 1f);
		yield break;
	}

	public IEnumerator ScaleInBlocks(Int32 n)
	{
		Int32 tick = 0;
		Single bigwidth = 0f;
		Single bigheight = 0f;
		for (Int32 i = 0; i < n; i++)
		{
			QuadMistCardUI block = this.RandomBlock();
			Vector3 pos = block.transform.localPosition;
			Vector3 scale = block.transform.localScale;
			for (Int32 j = 0; j <= 8; j++)
			{
				tick = 10 - j;
				if (tick < 0)
				{
					tick = 0;
				}
				bigwidth = (Single)(tick * 4 / 3) * 0.01f;
				bigheight = (Single)(tick * 5 / 3) * 0.01f;
				block.transform.localScale = new Vector3((QuadMistCardUI.SIZE_W + bigwidth * 2f) / QuadMistCardUI.SIZE_W, (QuadMistCardUI.SIZE_H + bigheight * 2f) / QuadMistCardUI.SIZE_H, 1f);
				block.transform.localPosition = pos + new Vector3(-bigwidth, bigheight, 0f);
				yield return base.StartCoroutine(Anim.Tick());
			}
			block.transform.localPosition = pos;
			block.transform.localScale = scale;
			SoundEffect.Play(QuadMistSoundID.MINI_SE_WALL);
		}
		yield break;
	}

	private QuadMistCardUI RandomBlock()
	{
		Int32 num = UnityEngine.Random.Range(0, 16);
		while (!this.IsFree(num))
		{
			num = UnityEngine.Random.Range(0, 16);
		}
		this.field[num].Data = CardPool.GetBlockCard((Int32)((Byte)UnityEngine.Random.Range(0, 2)));
		this.field[num].gameObject.SetActive(true);
		return this.field[num];
	}

	public void SetBoardCursorPosition(Vector2 position)
	{
		this.BoardCursor.gameObject.transform.position = new Vector3(position.x, position.y, -8f);
	}

	public void ShowBoardCursor()
	{
		this.BoardCursor.Show();
	}

	public void HideBoardCursor()
	{
		this.BoardCursor.Hide();
	}

	public static Int32 SIZE_X = 4;

	public static Int32 SIZE_Y = 4;

	public static Int32 FIELD_LINE_W = 1;

	public static Int32 FIELD_LINE_H = 1;

	public QuadMistCursor cursor;

	public QuadMistCardUI[] field = new QuadMistCardUI[Board.SIZE_X * Board.SIZE_Y];

	public SpriteRenderer background;

	private Dictionary<QuadMistCard, QuadMistCardUI> hash;

	public GameObject cardPrefab;

	public QuadMistCardCursor BoardCursor;
}
