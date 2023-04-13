using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using Memoria;
using Memoria.Prime;
using UnityEngine;

public class Board : MonoBehaviour
{
	public void InitResources()
	{
		for (Int32 i = 0; i < Board.SIZE_X * Board.SIZE_Y; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(cardPrefab);
			field[i] = gameObject.GetComponent<QuadMistCardUI>();
			gameObject.name = "fileCard" + ((i > 9) ? String.Empty : "0") + i;
			gameObject.transform.parent = base.transform;
			gameObject.SetActive(false);
		}
	}

	private void Start()
	{
		for (Int32 i = 0; i < Board.SIZE_X * Board.SIZE_Y; i++)
		{
			Int32 num = i % ((Configuration.TetraMaster.TripleTriad < 2) ? 4 : 3);
			Int32 num2 = i / ((Configuration.TetraMaster.TripleTriad < 2) ? 4 : 3);
			field[i].gameObject.SetActive(false);
			field[i].transform.localPosition = new Vector3((Single)num * (QuadMistCardUI.SIZE_W + (Single)Board.FIELD_LINE_W * 0.01f), -((Single)num2 * (QuadMistCardUI.SIZE_H + (Single)Board.FIELD_LINE_H * 0.01f)), 0f);
			field[i].name = num + "," + num2;
        }
		background.color = new Color(1f, 1f, 1f, 0f);
		hash = new Dictionary<QuadMistCard, QuadMistCardUI>();
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
			return this[i + j * ((Configuration.TetraMaster.TripleTriad < 2) ? 4 : 3)];
		}
		set
		{
			this[i + j * ((Configuration.TetraMaster.TripleTriad < 2) ? 4 : 3)] = value;
		}
	}

	public QuadMistCard this[Int32 i]
	{
		get
		{
			return field[i].Data;
		}
		set
		{
			if (field[i].Data != null)
			{
				hash.Remove(field[i].Data);
			}
			field[i].Data = value;
			if (value != null)
			{
				hash.Add(value, field[i]);
			}
		}
	}

	public Boolean IsFree(Int32 i, Int32 j)
	{
		return IsFree(i + j * ((Configuration.TetraMaster.TripleTriad < 2) ? 4 : 3));
	}

	public Boolean IsFree(Int32 i)
	{
		return !field[i].gameObject.activeSelf;
	}

	public void PlaceCursor(Int32 i, Int32 j)
	{
		if (i < 0 || i >= Board.SIZE_X || j < 0 || j >= Board.SIZE_Y)
		{
			cursor.Active = false;
		}
		else
		{
			cursor.Active = true;
			cursor.transform.position = field[i + j * ((Configuration.TetraMaster.TripleTriad < 2) ? 4 : 3)].transform.position + new Vector3(0f, 0f, -1f);
		}
	}

	public QuadMistCardUI GetCardUI(QuadMistCard card)
	{
		QuadMistCardUI result = (QuadMistCardUI)null;
		hash.TryGetValue(card, out result);
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
		return field[i + j * ((Configuration.TetraMaster.TripleTriad < 2) ? 4 : 3)];
	}

	public QuadMistCardUI GetCardUI(Single i, Single j)
	{
		return GetCardUI((Int32)i, (Int32)j);
	}

	public QuadMistCardUI GetCardUI(Int32 i)
	{
		if (i < 0 || i >= (Int32)field.Length)
		{
			return (QuadMistCardUI)null;
		}
		return field[i];
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
		Vector2 cardLocation = GetCardLocation(card);
		return GetAdjacentCards((Int32)cardLocation.x, (Int32)cardLocation.y);
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
		Int32 indexByWorldPoint = GetIndexByWorldPoint(worldPoint, checkForFree);
		if (indexByWorldPoint == -1)
		{
			return new Vector2(-100f, -100f);
		}
		return new Vector2((Single)(indexByWorldPoint % ((Configuration.TetraMaster.TripleTriad < 2) ? 4 : 3)), (Single)(indexByWorldPoint / ((Configuration.TetraMaster.TripleTriad < 2) ? 4 : 3)));
	}

	public Int32 GetIndexByWorldPoint(Vector3 worldPoint, Boolean checkForFree = false)
	{
		for (Int32 i = 0; i < Board.SIZE_X * Board.SIZE_Y; i++)
		{
			if ((!checkForFree || IsFree(i)) && field[i].Contains(worldPoint))
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
		background.color = new Color(1f, 1f, 1f, 0f);
		if (Configuration.TetraMaster.TripleTriad < 2)
			background.gameObject.SetActive(true);
		for (Int32 tick = 0; tick <= 32; tick++)
		{
			alpha = (Single)(tick * 8) / 255f;
			background.color = new Color(1f, 1f, 1f, alpha);
			yield return base.StartCoroutine(Anim.Tick());
		}
		background.color = new Color(1f, 1f, 1f, 1f);
		yield break;
	}

	public IEnumerator ScaleInBlocks(Int32 n)
	{
		Int32 tick = 0;
		Single bigwidth = 0f;
		Single bigheight = 0f;
		for (Int32 i = 0; i < n; i++)
		{
			QuadMistCardUI block = RandomBlock();
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
		while (!IsFree(num))
		{
			num = UnityEngine.Random.Range(0, 16);
		}
		field[num].Data = CardPool.GetBlockCard((Int32)((Byte)UnityEngine.Random.Range(0, 2)));
		field[num].gameObject.SetActive(true);
		return field[num];
	}

	public void SetBoardCursorPosition(Vector2 position)
	{
		BoardCursor.gameObject.transform.position = new Vector3(position.x, position.y, -8f);
	}

	public void ShowBoardCursor()
	{
		BoardCursor.Show();
	}

	public void HideBoardCursor()
	{
		BoardCursor.Hide();
	}

	public static Int32 SIZE_X = (Configuration.TetraMaster.TripleTriad < 2) ? 4 : 3;

	public static Int32 SIZE_Y = (Configuration.TetraMaster.TripleTriad < 2) ? 4 : 3;

    public static Int32 FIELD_LINE_W = (Configuration.TetraMaster.TripleTriad < 2) ? 1 : 10;

    public static Int32 FIELD_LINE_H = (Configuration.TetraMaster.TripleTriad < 2) ? 1 : 10;

    public QuadMistCursor cursor;

	public QuadMistCardUI[] field = new QuadMistCardUI[Board.SIZE_X * Board.SIZE_Y];

	public SpriteRenderer background;

	private Dictionary<QuadMistCard, QuadMistCardUI> hash;

	public GameObject cardPrefab;

	public QuadMistCardCursor BoardCursor;
}
