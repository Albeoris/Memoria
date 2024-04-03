using Assets.Sources.Scripts.UI.Common;
using Memoria;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TitleUI;

public class Hand : MonoBehaviour, IEnumerable, IEnumerable<QuadMistCard>, IList<QuadMistCard>, ICollection<QuadMistCard>
{
	Boolean ICollection<QuadMistCard>.Remove(QuadMistCard item)
	{
		Boolean result = cards.Remove(item);
		OnListChanged();
		return result;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private static Vector3 DEFAULT_TILING(Int32 count)
	{
		return new Vector3(0f, -(1.28f / (Single)Mathf.Max(count - 1, 4)), -1f);
	}

	public Boolean IsDragged
	{
		get
		{
			return _dragged;
		}
		set
		{
			_dragged = value;
		}
	}

	public void HideCardCursor()
	{
		CardCursor.Hide();
	}

	public void ShowCardCursor()
	{
		CardCursor.Show();
	}

	public void SetCardCursorTopMost()
	{
		Vector3 position = CardCursor.gameObject.transform.position;
		position.z = Board.USE_TRIPLETRIAD_BOARD ? -18f : -1.8f;
		CardCursor.gameObject.transform.position = position;
	}

    public void HideShadowCard()
	{
		shadowCard.gameObject.SetActive(false);
	}

	public void ShowShadowCard()
	{
		shadowCard.gameObject.SetActive(true);
	}

	public Hand.STATE State
	{
		get
		{
			return _state;
		}
		set
		{
			if (State != value)
			{
				OnStateChanged(State, value);
				_state = value;
				Select = Select;
				OnSelectChanged(Select, Select);
			}
		}
	}

	public Int32 EnemyFakeSelect
	{
		set
		{
			if (value >= Count || value < 0)
			{
				value = -1;
			}
			if (Select != value)
			{
				OnEnemyFakeSelectChanged(Select, value);
				_select = value;
			}
		}
	}

	public void ForceUpdateCursor()
	{
		OnSelectChanged(-1, _select);
	}

	public Int32 Select
	{
		get
		{
			return _select;
		}
		set
		{
			if (value == Count)
			{
				value = Count - 1;
			}
			if (value < 0)
			{
				value = -1;
			}
			if (Select != value)
			{
				OnSelectChanged(Select, value);
				_select = value;
			}
		}
	}

	public QuadMistCardUI SelectedUI
	{
		get
		{
			if (Select != -1)
			{
				return cardUIs[Select];
			}
			return (QuadMistCardUI)null;
		}
	}

	public Int32 GetIndexByWorldPoint(Vector3 worldPoint)
	{
		Int32 num = -1;
		for (Int32 i = 0; i < (Int32)cardUIs.Length; i++)
		{
			if (cardUIs[i].gameObject.activeSelf && cardUIs[i].Contains(worldPoint))
			{
				if (num != -1 && cardUIs[i].transform.position.z < cardUIs[num].transform.position.z)
				{
					num = i;
				}
				else if (num == -1)
				{
					num = i;
				}
			}
		}
		return num;
	}

	public QuadMistCardUI GetCardUI(Int32 i)
	{
		return cardUIs[i];
	}

	public QuadMistCardUI GetCardUI(QuadMistCard card)
	{
		QuadMistCardUI[] array = cardUIs;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			QuadMistCardUI quadMistCardUI = array[i];
			if (quadMistCardUI.Data == card)
			{
				return quadMistCardUI;
			}
		}
		return (QuadMistCardUI)null;
	}

	private void Start()
	{
		InitResources();
		for (Int32 i = 0; i < (Int32)cardUIs.Length; i++)
		{
			cardUIs[i].gameObject.SetActive(false);
		}
		Select = -1;
	}

	private void InitResources()
	{
		InitCards();
		InitShadowCard();
	}

	private void InitCards()
	{
		for (Int32 i = 0; i < Hand.MAX_CARDS; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(cardPrefab);
			cardUIs[i] = gameObject.GetComponent<QuadMistCardUI>();
			gameObject.name = "cardUIs" + ((i > 9) ? String.Empty : "0") + i;
			gameObject.transform.parent = transform;
			gameObject.SetActive(false);
        }
	}

	private void InitShadowCard()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(cardPrefab);
		shadowCard = gameObject.GetComponent<QuadMistCardUI>();
		gameObject.name = "shadowCardUI";
		gameObject.transform.parent = transform;
		gameObject.SetActive(false);
		shadowCard.Black = true;
	}

    private void OnStateChanged(Hand.STATE oldState, Hand.STATE newState)
	{
		for (Int32 i = 0; i < (Int32)cardUIs.Length; i++)
		{
			QuadMistCardUI quadMistCardUI = cardUIs[i];
			Boolean ShowCard = !Configuration.TetraMaster.ShowEnemyDeck;  
			if (QuadMistGame.HasTripleTrialRule_Open)
                ShowCard = false;
            switch (newState)
			{
			case Hand.STATE.ENEMY_HIDE:
				quadMistCardUI.Flip = ShowCard;
				transform.localPosition = Hand.ENEMY_POSITION;
				quadMistCardUI.transform.localPosition = new Vector3(0f, -3.2f, 0f);
				break;
			case Hand.STATE.ENEMY_SHOW:
                quadMistCardUI.Flip = ShowCard;
                    transform.localPosition = Hand.ENEMY_POSITION;
				SlideToEnemyHand(quadMistCardUI.transform, Hand.ENEMY_TILING * i, i);
				break;
			case Hand.STATE.ENEMY_WAIT:
                quadMistCardUI.Flip = ShowCard;
                    transform.localPosition = Hand.ENEMY_POSITION;
				quadMistCardUI.transform.localPosition = Hand.ENEMY_TILING * i;
				break;
			case Hand.STATE.ENEMY_PLAY:
                quadMistCardUI.Flip = ShowCard;
                    transform.localPosition = Hand.ENEMY_POSITION;
				quadMistCardUI.transform.localPosition = Hand.ENEMY_TILING * i;
				break;
			case Hand.STATE.ENEMY_POSTGAME:
				quadMistCardUI.Flip = false;
				transform.localPosition = Hand.ENEMY_POST_POSITION;
				SlideTo(quadMistCardUI.transform, Hand.DEFAULT_TILING(Count) * i, Anim.TickToTime(20));
				break;
			case Hand.STATE.PLAYER_PREGAME:
				quadMistCardUI.Flip = false;
				transform.localPosition = Hand.PLAYER_POSITION;
                if (Board.USE_TRIPLETRIAD_BOARD)
                {
//                    quadMistCardUI.cardDisplay.Element = true;
                    quadMistCardUI.transform.localPosition = new Vector3(Hand.PLAYER_TILING_L.x * i, Hand.PLAYER_TILING_L.y * i, -i);
                    quadMistCardUI.transform.localScale = Vector3.one;
                }
				else
				{
                    quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_S * i;
                    quadMistCardUI.transform.localScale = Hand.PLAYER_SCALE_S;
                }
				break;
			case Hand.STATE.PLAYER_WAIT:
				transform.localPosition = Hand.PLAYER_POSITION;
				if (Board.USE_TRIPLETRIAD_BOARD)
				{
                        quadMistCardUI.transform.localPosition = new Vector3(Hand.PLAYER_TILING_L.x * i, Hand.PLAYER_TILING_L.y * i, -i);
                        quadMistCardUI.transform.localScale = Vector3.one;
                }
				else
				{
					if (Count == 5)
                    {
						quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_S * i;
						quadMistCardUI.transform.localScale = Hand.PLAYER_SCALE_S;
                    }
                    else
                    {
						quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_L * i;
						quadMistCardUI.transform.localScale = Vector3.one;
                    }
				}
				break;
			case Hand.STATE.PLAYER_SELECT_CARD:
			{
				transform.localPosition = Hand.PLAYER_POSITION;
				Single duration = SpeedFormula(quadMistCardUI.transform, Hand.PLAYER_TILING_S * i);
                if (Board.USE_TRIPLETRIAD_BOARD)
                {
                    StartCoroutine(SlideCardBackToTheFormerPosition(quadMistCardUI.transform, new Vector3(Hand.PLAYER_TILING_L.x * i, Hand.PLAYER_TILING_L.y * i, -i), duration));
					break;
                }
                if (Count == 5)
				{
					StartCoroutine(SlideCardBackToTheFormerPosition(quadMistCardUI.transform, Hand.PLAYER_TILING_S * i, duration));
					ScaleTo(quadMistCardUI.transform, Hand.PLAYER_SCALE_S, duration);
				}
				else
				{
					StartCoroutine(SlideCardBackToTheFormerPosition(quadMistCardUI.transform, Hand.PLAYER_TILING_L * i, duration));
				}
				CardCursor.Hide();
				break;
			}
			case Hand.STATE.PLAYER_SELECT_BOARD:
				transform.localPosition = Hand.PLAYER_POSITION;
                if (Board.USE_TRIPLETRIAD_BOARD)
                {
                    quadMistCardUI.transform.localPosition = new Vector3(Hand.PLAYER_TILING_L.x * i, Hand.PLAYER_TILING_L.y * i, -i);
                    quadMistCardUI.transform.localScale = Vector3.one;
					break;
                }
                if (Count == 5)
				{
					quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_S * i;
					quadMistCardUI.transform.localScale = Hand.PLAYER_SCALE_S;
				}
				else
				{
					quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_L * i;
					quadMistCardUI.transform.localScale = Vector3.one;
				}
				break;
			case Hand.STATE.PLAYER_POSTGAME:
				transform.localPosition = Hand.PLAYER_POST_POSITION;
				quadMistCardUI.transform.localScale = Vector3.one;
				SlideTo(quadMistCardUI.transform, Hand.DEFAULT_TILING(Count) * i, Anim.TickToTime(20));
				break;
			default:
				transform.position = Hand.PLAYER_POSITION;
				break;
			}
		}
	}

	public void ClearFakeSelectData()
	{
		for (Int32 i = 0; i < 10; i++)
		{
			enemyFakeOffsetList[i] = 0f;
		}
	}

	private void OnEnemyFakeSelectChanged(Int32 oldSelect, Int32 newSelect)
	{
		Single num = 0.12f;
		for (Int32 i = 0; i < (Int32)cardUIs.Length; i++)
		{
			QuadMistCardUI quadMistCardUI = cardUIs[i];
			if (i == newSelect)
			{
				Vector3 position = quadMistCardUI.transform.position;
				position.x += num;
				quadMistCardUI.transform.position = position;
				enemyFakeOffsetList[i] = 0.12f;
			}
			else if (i == oldSelect)
			{
				Vector3 position2 = quadMistCardUI.transform.position;
				position2.x -= enemyFakeOffsetList[i];
				quadMistCardUI.transform.position = position2;
				enemyFakeOffsetList[i] = 0f;
			}
		}
	}

	public void UpdateShadowCard(Int32 select)
	{
		QuadMistCardUI quadMistCardUI = cardUIs[select];
		shadowCard.Data = quadMistCardUI.Data;
		Vector3 position = quadMistCardUI.transform.position;
		Vector3 localScale = quadMistCardUI.transform.localScale;
		shadowCard.transform.position = new Vector3(position.x, position.y, position.z + 5f);
		shadowCard.transform.localScale = localScale;
	}

	public void UpdateCursorToShadowCard()
	{
		Vector3 position = shadowCard.transform.position;
		CardCursor.Show();
		Single num = -0.16f;
		if (Count != 5)
		{
			num = -0.248f;
		}
		CardCursor.transform.position = new Vector3(position.x - 0.1f, position.y + num, position.z - 2f);
	}

	public void UpdateEnemyCardCursorToPosition(Vector3 cardPosition)
	{
		CardCursor.gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
		Vector3 localPosition = CardCursor.gameObject.transform.localPosition;
		localPosition.z = -10f;
		localPosition.x = 0.65f;
		localPosition.y = cardPosition.y - 0.05f;
		CardCursor.gameObject.transform.localPosition = localPosition;
	}

	private void OnSelectChanged(Int32 oldSelect, Int32 newSelect)
	{
		cursor.Active = false;
		cursor.Black = false;
		Int32 i = 0;
		while (i < cardUIs.Length)
		{
			QuadMistCardUI quadMistCardUI = cardUIs[i];
			quadMistCardUI.ResetEffect();
			switch (State)
			{
			case Hand.STATE.PLAYER_SELECT_CARD:
				if (i == newSelect)
				{
					if (cardAnimatingCount == 0 || lastSelectedCard != newSelect)
						UpdateShadowCard(newSelect);
					if (Count == 5 && Configuration.TetraMaster.TripleTriad <= 1)
						cursor.transform.position = quadMistCardUI.transform.position + Hand.CURSOR_OFFSET_S;
					else
						cursor.transform.position = quadMistCardUI.transform.position + Hand.CURSOR_OFFSET_L;
					cursor.Active = true;
					if (oldSelect != newSelect)
						UpdateCursorToShadowCard();
					lastSelectedCard = newSelect;
				}
				break;
			case Hand.STATE.PLAYER_SELECT_BOARD:
				if (i == newSelect)
				{
					cursor.Active = true;
					cursor.Black = true;
					quadMistCardUI.Black = false;
					CardCursor.SetNormalState();
					if (Count == 5 && Configuration.TetraMaster.TripleTriad <= 1)
						cursor.transform.position = quadMistCardUI.transform.position + Hand.CURSOR_OFFSET_S;
					else
						cursor.transform.position = quadMistCardUI.transform.position + Hand.CURSOR_OFFSET_L;
				}
				else
				{
					quadMistCardUI.Black = true;
					CardCursor.SetBlackState();
				}
				break;
			}
			i++;
		}
	}

	private void OnListChanged()
	{
		Int32 select = Select;
		Select = -1;
		for (Int32 i = 0; i < cardUIs.Length; i++)
		{
			if (i < Count)
			{
				cardUIs[i].gameObject.SetActive(true);
				cardUIs[i].Data = this[i];
			}
			else
			{
				cardUIs[i].gameObject.SetActive(false);
			}
		}
		OnListChanged2();
		Select = select;
	}

	private void OnListChanged2()
	{
		Int32 i = 0;
		while (i < cardUIs.Length)
		{
			QuadMistCardUI quadMistCardUI = cardUIs[i];
			switch (State)
			{
			case Hand.STATE.ENEMY_HIDE:
				quadMistCardUI.Flip = true;
				transform.localPosition = Hand.ENEMY_POSITION;
				quadMistCardUI.transform.localPosition = new Vector3(0f, -3.2f, 0f);
				break;
			case Hand.STATE.ENEMY_PLAY:
				quadMistCardUI.Flip = true;
				transform.localPosition = Hand.ENEMY_POSITION;
				quadMistCardUI.transform.localPosition = Hand.ENEMY_TILING * i;
				break;
			case Hand.STATE.ENEMY_POSTGAME:
				quadMistCardUI.Flip = false;
				transform.localPosition = Hand.ENEMY_POST_POSITION;
				quadMistCardUI.transform.localPosition = Hand.DEFAULT_TILING(Count) * i;
				break;
			case Hand.STATE.PLAYER_PREGAME:
				transform.localPosition = Hand.PLAYER_POSITION;
                if (Board.USE_TRIPLETRIAD_BOARD)
                {
                    quadMistCardUI.transform.localPosition = new Vector3(Hand.PLAYER_TILING_L.x * i, Hand.PLAYER_TILING_L.y * i, -i);
                    quadMistCardUI.transform.localScale = Vector3.one;
					break;
                }
                quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_S * i;
				quadMistCardUI.transform.localScale = Hand.PLAYER_SCALE_S;
				break;
			case Hand.STATE.PLAYER_SELECT_BOARD:
				transform.localPosition = Hand.PLAYER_POSITION;
                if (Board.USE_TRIPLETRIAD_BOARD)
                {
                    quadMistCardUI.transform.localPosition = new Vector3(Hand.PLAYER_TILING_L.x * i, Hand.PLAYER_TILING_L.y * i, -i);
                    quadMistCardUI.transform.localScale = Vector3.one;
                    break;
                }
                if (Count == 5)
				{
					quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_S * i;
					quadMistCardUI.transform.localScale = Hand.PLAYER_SCALE_S;
				}
				else
				{
					quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_L * i;
					quadMistCardUI.transform.localScale = Vector3.one;
				}
				break;
			case Hand.STATE.PLAYER_POSTGAME:
				transform.localPosition = Hand.PLAYER_POST_POSITION;
				quadMistCardUI.transform.localScale = Vector3.one;
				quadMistCardUI.transform.localPosition = Hand.DEFAULT_TILING(Count) * i;
				break;
			}
			i++;
		}
	}

	private void SlideTo(Transform t, Vector3 mov, Single time)
	{
		StartCoroutine(Anim.MoveLerp(t, mov, time, true));
	}

    public int CardAnimatingCount
    {
        get
        {
            return cardAnimatingCount;
        }
    }

    private IEnumerator SlideCardBackToTheFormerPosition(Transform origin, Vector3 target, Single duration)
	{
		cardAnimatingCount++;
		yield return StartCoroutine(Anim.MoveLerp(origin, target, duration, true));
		cardAnimatingCount--;
		yield break;
	}

	private void SlideToDelayed(Transform t, Vector3 mov, Single time, Single delayed)
	{
		StartCoroutine(Anim.Sequence(new IEnumerator[]
		{
			Anim.Delay(delayed),
			Anim.MoveLerp(t, mov, time, true)
		}));
	}

	private void SlideToEnemyHand(Transform t, Vector3 mov, Int32 i)
	{
		StartCoroutine(SlideEnemyCard(t, mov, i));
	}

	private void ScaleTo(Transform t, Vector3 scal, Single time)
	{
		if (!Mathf.Approximately(t.localScale.x, scal.x) && !Mathf.Approximately(t.localScale.y, scal.y))
		{
			StartCoroutine(Anim.ScaleLerp(t.transform, scal, time));
		}
	}

	private Single SpeedFormula(Transform t, Vector3 mov)
	{
		return Vector3.Distance(t.localPosition, mov) * (1f / Hand.RECOVER_SPEED);
	}

	private IEnumerator SlideEnemyCard(Transform t, Vector3 fin, Int32 i)
	{
		Vector3 pos = t.localPosition;
		for (Int32 tick = 0; tick <= 40; tick++)
		{
            Int32 trans = 15 + i * 6 - tick;
			if (trans <= 0)
				break;
			t.transform.localPosition = new Vector3(pos.x, -(trans * trans + i * 14) * (Board.USE_TRIPLETRIAD_BOARD ? 0.03f : 0.01f), -i);
			yield return StartCoroutine(Anim.Tick());
		}
		SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_MOVE);
		t.transform.localPosition = fin;
		yield break;
	}

	public void ApplyScaleToSelectedCard(Int32 cardIndex)
	{
		QuadMistCardUI quadMistCardUI = cardUIs[cardIndex];
		if (Count == 5)
			ScaleTo(quadMistCardUI.transform, Hand.PLAYER_SCALE_L, Hand.SCALE_SPEED);
	}

	public void SetCardScaleBecauseOfUserCancellation()
	{
		if (Count == 5)
		{
			for (Int32 i = 0; i < 5; i++)
			{
				QuadMistCardUI quadMistCardUI = cardUIs[i];
				ScaleTo(quadMistCardUI.transform, Hand.PLAYER_SCALE_S, Hand.SCALE_SPEED);
			}
		}
	}

	public void Add(QuadMistCard item)
	{
		cards.Add(item);
		OnListChanged();
	}

	public void ReplaceCard(Int32 cardsArrayIndex, QuadMistCard newCard)
	{
		cards[cardsArrayIndex].id = newCard.id;
		cards[cardsArrayIndex].atk = newCard.atk;
		cards[cardsArrayIndex].arrow = newCard.arrow;
		cards[cardsArrayIndex].type = newCard.type;
		cards[cardsArrayIndex].pdef = newCard.pdef;
		cards[cardsArrayIndex].mdef = newCard.mdef;
		OnListChanged();
	}

	public void AddWithoutChanged(QuadMistCard item)
	{
		cards.Add(item);
		Int32 select = Select;
		Select = -1;
		for (Int32 i = 0; i < (Int32)cardUIs.Length; i++)
		{
			if (i < Count)
			{
				cardUIs[i].gameObject.SetActive(true);
				cardUIs[i].Data = this[i];
			}
			else
			{
				cardUIs[i].gameObject.SetActive(false);
			}
		}
		Select = select;
	}

	public Int32 IndexOf(QuadMistCard item)
	{
		return cards.IndexOf(item);
	}

	public void Insert(Int32 index, QuadMistCard item)
	{
		cards.Insert(index, item);
		OnListChanged();
	}

	public Boolean Remove(QuadMistCard card)
	{
		Boolean result = cards.Remove(card);
		OnListChanged();
		return result;
	}

	public void RemoveAt(Int32 index)
	{
		cards.RemoveAt(index);
		OnListChanged();
	}

	public QuadMistCard this[Int32 index]
	{
		get
		{
			return cards[index];
		}
		set
		{
			cards[index] = value;
		}
	}

	public void Clear()
	{
		cards.Clear();
		OnListChanged();
	}

	public Boolean Contains(QuadMistCard item)
	{
		return cards.Contains(item);
	}

	public void CopyTo(QuadMistCard[] array, Int32 arrayIndex)
	{
		cards.CopyTo(array, arrayIndex);
	}

	public Int32 Count
	{
		get
		{
			return cards.Count;
		}
	}

	public Boolean IsReadOnly
	{
		get
		{
			return false;
		}
	}

	public IEnumerator<QuadMistCard> GetEnumerator()
	{
		foreach (QuadMistCard item in cards)
		{
			yield return item;
		}
		yield break;
	}

	public List<QuadMistCard> GetQuadMistCards()
	{
		return cards;
	}

	private static Int32 MAX_CARDS = 10;

	private static Single RECOVER_SPEED = 22f;

	private static Single SCALE_SPEED = 0.2f;

	private static Vector3 ENEMY_POSITION = new Vector3(0.16f, Board.USE_TRIPLETRIAD_BOARD ? -0.06f : -0.26f, -1f);
	private static Vector3 ENEMY_POST_POSITION = new Vector3(0.16f, Board.USE_TRIPLETRIAD_BOARD ? -0.05f : -0.25f, -1f);
	private static Vector3 ENEMY_TILING = new Vector3(0f, Board.USE_TRIPLETRIAD_BOARD ? -0.34f : -0.14f, -1f);
	private static Vector3 PLAYER_SCALE_S = new Vector3(0.8f, 0.8f, 1f);
	private static Vector3 PLAYER_SCALE_L = new Vector3(1f, 1f, 1f);
	private static Vector3 PLAYER_TILING_S = new Vector3(0f, -0.42f, 0f);
	private static Vector3 PLAYER_TILING_L = new Vector3(0f, Board.USE_TRIPLETRIAD_BOARD ? -0.34f : -0.52f, 0f);
	private static Vector3 PLAYER_POSITION = new Vector3(2.6f, -0.08f, -1f);
	private static Vector3 PLAYER_POST_POSITION = new Vector3(2.6f, Board.USE_TRIPLETRIAD_BOARD ? -0.05f : -0.26f, -1f);
	private static Vector3 CURSOR_OFFSET_S = new Vector3(-0.15f, -0.06f, -1f);
	private static Vector3 CURSOR_OFFSET_L = new Vector3(-0.18f, -0.03f, -1f);
	private static Vector3 CURSOR_OFFSET_REVERSE = new Vector3(0.58f, -0.05f, -1f);

	public QuadMistCursor cursor;

	public QuadMistCardUI[] cardUIs = new QuadMistCardUI[Hand.MAX_CARDS];

	private QuadMistCardUI shadowCard;

    public QuadMistCardCursor CardCursor;

	public GameObject cardPrefab;

    public UISprite CardElementSprite;

    private List<QuadMistCard> cards = new List<QuadMistCard>(Hand.MAX_CARDS);

	private Int32 _select;

	private Boolean _dragged;

	private Hand.STATE _state;

	private Single[] enemyFakeOffsetList = new Single[10];

	private Int32 lastSelectedCard = -1;

	private Int32 cardAnimatingCount;

	public enum STATE
	{
		ENEMY_HIDE,
		ENEMY_SHOW,
		ENEMY_WAIT,
		ENEMY_PLAY,
		ENEMY_POSTGAME,
		PLAYER_PREGAME,
		PLAYER_WAIT,
		PLAYER_SELECT_CARD,
		PLAYER_SELECT_BOARD,
		PLAYER_POSTGAME
	}
}
