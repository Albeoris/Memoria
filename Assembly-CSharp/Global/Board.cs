using System;
using System.Collections;
using System.Collections.Generic;
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
			Int32 x = i % Board.SIZE_X;
			Int32 y = i / Board.SIZE_X;
			Single factor = Board.USE_TRIPLETRIAD_BOARD ? 0.0001f : 0.01f;
			Single adjustementX = Board.USE_TRIPLETRIAD_BOARD ? 0.06f : 0f; // Adjust positions for Triple Triad.
			Single adjustementY = Board.USE_TRIPLETRIAD_BOARD ? 0.08f : 0f; // Adjust positions for Triple Triad.
			field[i].gameObject.SetActive(false);
			field[i].transform.localPosition = new Vector3(x * (QuadMistCardUI.SIZE_W + Board.FIELD_LINE_W * factor) + adjustementX, -(y * (QuadMistCardUI.SIZE_H + Board.FIELD_LINE_H * factor)) - adjustementY, 0f);
			if (Board.USE_TRIPLETRIAD_BOARD) // Resize Triple Triad cards
				field[i].transform.localScale = new Vector3(1.3f, 1.25f, 1f);
			field[i].name = x + "," + y;
		}
		background.color = new Color(1f, 1f, 1f, 0f);
		hash = new Dictionary<QuadMistCard, QuadMistCardUI>();
	}

	public QuadMistCard this[Int32 i, Int32 j]
	{
		get
		{
			if (i >= Board.SIZE_X || i < 0)
				return null;
			if (j >= Board.SIZE_Y || j < 0)
				return null;
			return this[i + j * Board.SIZE_X];
		}
		set
		{
			this[i + j * Board.SIZE_X] = value;
		}
	}

	public QuadMistCard this[Int32 i]
	{
		get => field[i].Data;
		set
		{
			if (field[i].Data != null)
				hash.Remove(field[i].Data);
			field[i].Data = value;
			if (value != null)
				hash.Add(value, field[i]);
		}
	}

	public Boolean IsFree(Int32 i, Int32 j)
	{
		return IsFree(i + j * Board.SIZE_X);
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
			cursor.transform.position = field[i + j * Board.SIZE_X].transform.position + new Vector3(0f, 0f, -1f);
		}
	}

	public QuadMistCardUI GetCardUI(QuadMistCard card)
	{
		hash.TryGetValue(card, out QuadMistCardUI result);
		return result;
	}

	public QuadMistCardUI GetCardUI(Int32 i, Int32 j)
	{
		if (i >= Board.SIZE_X || i < 0)
			return null;
		if (j >= Board.SIZE_Y || j < 0)
			return null;
		return field[i + j * Board.SIZE_X];
	}

	public QuadMistCardUI GetCardUI(Int32 i)
	{
		if (i < 0 || i >= field.Length)
			return null;
		return field[i];
	}

	public Vector2 GetCardLocation(QuadMistCard card)
	{
		for (Int32 i = 0; i < Board.SIZE_X; i++)
			for (Int32 j = 0; j < Board.SIZE_Y; j++)
				if (card == this[i, j])
					return new Vector2(i, j);
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
			QuadMistCard quadMistCard = this[(Int32)(x + vector.x), (Int32)(y + vector.y)];
			if (quadMistCard != null && !quadMistCard.IsBlock)
				array[i] = quadMistCard;
			else
				array[i] = null;
		}
		return array;
	}

    public QuadMistCard[] GetAdjacentCardsTripleTriad(QuadMistCard card)
    {
        Vector2 cardLocation = GetCardLocation(card);
        return GetAdjacentCardsTripleTriad((Int32)cardLocation.x, (Int32)cardLocation.y);
    }

    public QuadMistCard[] GetAdjacentCardsTripleTriad(Int32 x, Int32 y)
    {
        QuadMistCard[] array = new QuadMistCard[CardArrow.MAX_ARROWNUM];
        for (Int32 i = 0; i < CardArrow.MAX_ARROWNUM; i++)
        {
			if (i % 2 == 1)
				continue;
            Vector3 vector = CardArrow.ToOffset((CardArrow.Type)i);
            QuadMistCard quadMistCard = this[(Int32)(x + vector.x), (Int32)(y + vector.y)];
            if (quadMistCard != null && !quadMistCard.IsBlock)
                array[i] = quadMistCard;
            else
                array[i] = null;
        }
        return array;
    }

    public Vector2 GetVectorByWorldPoint(Vector3 worldPoint, Boolean checkForFree = false)
	{
		Int32 indexByWorldPoint = GetIndexByWorldPoint(worldPoint, checkForFree);
		if (indexByWorldPoint == -1)
			return new Vector2(-100f, -100f);
		return new Vector2(indexByWorldPoint % Board.SIZE_X, indexByWorldPoint / Board.SIZE_X);
	}

	public Int32 GetIndexByWorldPoint(Vector3 worldPoint, Boolean checkForFree = false)
	{
		for (Int32 i = 0; i < Board.SIZE_X * Board.SIZE_Y; i++)
			if ((!checkForFree || IsFree(i)) && field[i].Contains(worldPoint))
				return i;
		return -1;
	}

	public void Clear()
	{
		for (Int32 i = 0; i < Board.SIZE_X * Board.SIZE_Y; i++)
			this[i] = null;
	}

	public IEnumerator FadeInBoard()
	{
		background.color = new Color(1f, 1f, 1f, 0f);
		if (Configuration.TetraMaster.TripleTriad <= 1)
			background.gameObject.SetActive(true);
		for (Int32 tick = 0; tick <= 32; tick++)
		{
			background.color = new Color(1f, 1f, 1f, tick * 8f / 255f);
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
		Int32 pos = UnityEngine.Random.Range(0, Board.SIZE_X * Board.SIZE_Y);
		while (!IsFree(pos))
			pos = UnityEngine.Random.Range(0, Board.SIZE_X * Board.SIZE_Y);
		field[pos].Data = CardPool.GetBlockCard(UnityEngine.Random.Range(0, 2));
		field[pos].gameObject.SetActive(true);
		return field[pos];
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

	public static Boolean USE_TRIPLETRIAD_BOARD = Configuration.TetraMaster.TripleTriad >= 2;
	public static Int32 SIZE_X = USE_TRIPLETRIAD_BOARD ? 3 : 4;
	public static Int32 SIZE_Y = USE_TRIPLETRIAD_BOARD ? 3 : 4;
	public static Int32 FIELD_LINE_W = 1;
	public static Int32 FIELD_LINE_H = 1;

	public QuadMistCursor cursor;
	public QuadMistCardUI[] field = new QuadMistCardUI[Board.SIZE_X * Board.SIZE_Y];
	public SpriteRenderer background;
	private Dictionary<QuadMistCard, QuadMistCardUI> hash;
	public GameObject cardPrefab;
	public QuadMistCardCursor BoardCursor;
}
