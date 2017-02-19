using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour, IEnumerable, IEnumerable<QuadMistCard>, IList<QuadMistCard>, ICollection<QuadMistCard>
{
	Boolean ICollection<QuadMistCard>.Remove(QuadMistCard item)
	{
		Boolean result = this.cards.Remove(item);
		this.OnListChanged();
		return result;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	private static Vector3 DEFAULT_TILING(Int32 count)
	{
		return new Vector3(0f, -(1.28f / (Single)Mathf.Max(count - 1, 4)), -1f);
	}

	public Boolean IsDragged
	{
		get
		{
			return this._dragged;
		}
		set
		{
			this._dragged = value;
		}
	}

	public void HideCardCursor()
	{
		this.CardCursor.Hide();
	}

	public void ShowCardCursor()
	{
		this.CardCursor.Show();
	}

	public void SetCardCursorTopMost()
	{
		Vector3 position = this.CardCursor.gameObject.transform.position;
		position.z = -1.8f;
		this.CardCursor.gameObject.transform.position = position;
	}

	public void HideShadowCard()
	{
		this.shadowCard.gameObject.SetActive(false);
	}

	public void ShowShadowCard()
	{
		this.shadowCard.gameObject.SetActive(true);
	}

	public Hand.STATE State
	{
		get
		{
			return this._state;
		}
		set
		{
			if (this.State != value)
			{
				this.OnStateChanged(this.State, value);
				this._state = value;
				this.Select = this.Select;
				this.OnSelectChanged(this.Select, this.Select);
			}
		}
	}

	public Int32 EnemyFakeSelect
	{
		set
		{
			if (value >= this.Count || value < 0)
			{
				value = -1;
			}
			if (this.Select != value)
			{
				this.OnEnemyFakeSelectChanged(this.Select, value);
				this._select = value;
			}
		}
	}

	public void ForceUpdateCursor()
	{
		this.OnSelectChanged(-1, this._select);
	}

	public Int32 Select
	{
		get
		{
			return this._select;
		}
		set
		{
			if (value == this.Count)
			{
				value = this.Count - 1;
			}
			if (value < 0)
			{
				value = -1;
			}
			if (this.Select != value)
			{
				this.OnSelectChanged(this.Select, value);
				this._select = value;
			}
		}
	}

	public QuadMistCardUI SelectedUI
	{
		get
		{
			if (this.Select != -1)
			{
				return this.cardUIs[this.Select];
			}
			return (QuadMistCardUI)null;
		}
	}

	public Int32 GetIndexByWorldPoint(Vector3 worldPoint)
	{
		Int32 num = -1;
		for (Int32 i = 0; i < (Int32)this.cardUIs.Length; i++)
		{
			if (this.cardUIs[i].gameObject.activeSelf && this.cardUIs[i].Contains(worldPoint))
			{
				if (num != -1 && this.cardUIs[i].transform.position.z < this.cardUIs[num].transform.position.z)
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
		return this.cardUIs[i];
	}

	public QuadMistCardUI GetCardUI(QuadMistCard card)
	{
		QuadMistCardUI[] array = this.cardUIs;
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
		this.InitResources();
		for (Int32 i = 0; i < (Int32)this.cardUIs.Length; i++)
		{
			this.cardUIs[i].gameObject.SetActive(false);
		}
		this.Select = -1;
	}

	private void InitResources()
	{
		this.InitCards();
		this.InitShadowCard();
	}

	private void InitCards()
	{
		for (Int32 i = 0; i < Hand.MAX_CARDS; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.cardPrefab);
			this.cardUIs[i] = gameObject.GetComponent<QuadMistCardUI>();
			gameObject.name = "cardUIs" + ((i > 9) ? String.Empty : "0") + i;
			gameObject.transform.parent = base.transform;
			gameObject.SetActive(false);
		}
	}

	private void InitShadowCard()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.cardPrefab);
		this.shadowCard = gameObject.GetComponent<QuadMistCardUI>();
		gameObject.name = "shadowCardUI";
		gameObject.transform.parent = base.transform;
		gameObject.SetActive(false);
		this.shadowCard.Black = true;
	}

	private void OnStateChanged(Hand.STATE oldState, Hand.STATE newState)
	{
		for (Int32 i = 0; i < (Int32)this.cardUIs.Length; i++)
		{
			QuadMistCardUI quadMistCardUI = this.cardUIs[i];
			switch (newState)
			{
			case Hand.STATE.ENEMY_HIDE:
				quadMistCardUI.Flip = true;
				base.transform.localPosition = Hand.ENEMY_POSITION;
				quadMistCardUI.transform.localPosition = new Vector3(0f, -3.2f, 0f);
				break;
			case Hand.STATE.ENEMY_SHOW:
				quadMistCardUI.Flip = true;
				base.transform.localPosition = Hand.ENEMY_POSITION;
				this.SlideToEnemyHand(quadMistCardUI.transform, Hand.ENEMY_TILING * (Single)i, i);
				break;
			case Hand.STATE.ENEMY_WAIT:
				quadMistCardUI.Flip = true;
				base.transform.localPosition = Hand.ENEMY_POSITION;
				quadMistCardUI.transform.localPosition = Hand.ENEMY_TILING * (Single)i;
				break;
			case Hand.STATE.ENEMY_PLAY:
				quadMistCardUI.Flip = true;
				base.transform.localPosition = Hand.ENEMY_POSITION;
				quadMistCardUI.transform.localPosition = Hand.ENEMY_TILING * (Single)i;
				break;
			case Hand.STATE.ENEMY_POSTGAME:
				quadMistCardUI.Flip = false;
				base.transform.localPosition = Hand.ENEMY_POST_POSITION;
				this.SlideTo(quadMistCardUI.transform, Hand.DEFAULT_TILING(this.Count) * (Single)i, Anim.TickToTime(20));
				break;
			case Hand.STATE.PLAYER_PREGAME:
				quadMistCardUI.Flip = false;
				base.transform.localPosition = Hand.PLAYER_POSITION;
				quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_S * (Single)i;
				quadMistCardUI.transform.localScale = Hand.PLAYER_SCALE_S;
				break;
			case Hand.STATE.PLAYER_WAIT:
				base.transform.localPosition = Hand.PLAYER_POSITION;
				if (this.Count == 5)
				{
					quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_S * (Single)i;
					quadMistCardUI.transform.localScale = Hand.PLAYER_SCALE_S;
				}
				else
				{
					quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_L * (Single)i;
					quadMistCardUI.transform.localScale = Vector3.one;
				}
				break;
			case Hand.STATE.PLAYER_SELECT_CARD:
			{
				base.transform.localPosition = Hand.PLAYER_POSITION;
				Single num = this.SpeedFormula(quadMistCardUI.transform, Hand.PLAYER_TILING_S * (Single)i);
				if (this.Count == 5)
				{
					base.StartCoroutine(this.SlideCardBackToTheFormerPosition(quadMistCardUI.transform, Hand.PLAYER_TILING_S * (Single)i, num));
					this.ScaleTo(quadMistCardUI.transform, Hand.PLAYER_SCALE_S, num);
				}
				else
				{
					base.StartCoroutine(this.SlideCardBackToTheFormerPosition(quadMistCardUI.transform, Hand.PLAYER_TILING_L * (Single)i, num));
				}
				this.CardCursor.Hide();
				break;
			}
			case Hand.STATE.PLAYER_SELECT_BOARD:
				base.transform.localPosition = Hand.PLAYER_POSITION;
				if (this.Count == 5)
				{
					quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_S * (Single)i;
					quadMistCardUI.transform.localScale = Hand.PLAYER_SCALE_S;
				}
				else
				{
					quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_L * (Single)i;
					quadMistCardUI.transform.localScale = Vector3.one;
				}
				break;
			case Hand.STATE.PLAYER_POSTGAME:
				base.transform.localPosition = Hand.PLAYER_POST_POSITION;
				quadMistCardUI.transform.localScale = Vector3.one;
				this.SlideTo(quadMistCardUI.transform, Hand.DEFAULT_TILING(this.Count) * (Single)i, Anim.TickToTime(20));
				break;
			default:
				base.transform.position = Hand.PLAYER_POSITION;
				break;
			}
		}
	}

	public void ClearFakeSelectData()
	{
		for (Int32 i = 0; i < 10; i++)
		{
			this.enemyFakeOffsetList[i] = 0f;
		}
	}

	private void OnEnemyFakeSelectChanged(Int32 oldSelect, Int32 newSelect)
	{
		Single num = 0.12f;
		for (Int32 i = 0; i < (Int32)this.cardUIs.Length; i++)
		{
			QuadMistCardUI quadMistCardUI = this.cardUIs[i];
			if (i == newSelect)
			{
				Vector3 position = quadMistCardUI.transform.position;
				position.x += num;
				quadMistCardUI.transform.position = position;
				this.enemyFakeOffsetList[i] = 0.12f;
			}
			else if (i == oldSelect)
			{
				Vector3 position2 = quadMistCardUI.transform.position;
				position2.x -= this.enemyFakeOffsetList[i];
				quadMistCardUI.transform.position = position2;
				this.enemyFakeOffsetList[i] = 0f;
			}
		}
	}

	public void UpdateShadowCard(Int32 select)
	{
		QuadMistCardUI quadMistCardUI = this.cardUIs[select];
		this.shadowCard.Data = quadMistCardUI.Data;
		Vector3 position = quadMistCardUI.transform.position;
		Vector3 localScale = quadMistCardUI.transform.localScale;
		this.shadowCard.transform.position = new Vector3(position.x, position.y, position.z + 5f);
		this.shadowCard.transform.localScale = localScale;
	}

	public void UpdateCursorToShadowCard()
	{
		Vector3 position = this.shadowCard.transform.position;
		this.CardCursor.Show();
		Single num = -0.16f;
		if (this.Count != 5)
		{
			num = -0.248f;
		}
		this.CardCursor.transform.position = new Vector3(position.x - 0.1f, position.y + num, position.z - 2f);
	}

	public void UpdateEnemyCardCursorToPosition(Vector3 cardPosition)
	{
		this.CardCursor.gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
		Vector3 localPosition = this.CardCursor.gameObject.transform.localPosition;
		localPosition.z = -10f;
		localPosition.x = 0.65f;
		localPosition.y = cardPosition.y - 0.05f;
		this.CardCursor.gameObject.transform.localPosition = localPosition;
	}

	private void OnSelectChanged(Int32 oldSelect, Int32 newSelect)
	{
		this.cursor.Active = false;
		this.cursor.Black = false;
		Int32 i = 0;
		while (i < (Int32)this.cardUIs.Length)
		{
			QuadMistCardUI quadMistCardUI = this.cardUIs[i];
			quadMistCardUI.ResetEffect();
			switch (this.State)
			{
			case Hand.STATE.PLAYER_SELECT_CARD:
				if (i == newSelect)
				{
					Boolean flag = this.cardAnimatingCount == 0;
					Boolean flag2 = this.lastSelectedCard != newSelect;
					if (flag || flag2)
					{
						this.UpdateShadowCard(newSelect);
					}
					if (this.Count == 5)
					{
						this.cursor.transform.position = quadMistCardUI.transform.position + Hand.CURSOR_OFFSET_S;
					}
					else
					{
						this.cursor.transform.position = quadMistCardUI.transform.position + Hand.CURSOR_OFFSET_L;
					}
					this.cursor.Active = true;
					if (oldSelect != newSelect)
					{
						this.UpdateCursorToShadowCard();
					}
					this.lastSelectedCard = newSelect;
				}
				break;
			case Hand.STATE.PLAYER_SELECT_BOARD:
				if (i == newSelect)
				{
					this.cursor.Active = true;
					this.cursor.Black = true;
					quadMistCardUI.Black = false;
					this.CardCursor.SetNormalState();
					if (this.Count == 5)
					{
						this.cursor.transform.position = quadMistCardUI.transform.position + Hand.CURSOR_OFFSET_S;
					}
					else
					{
						this.cursor.transform.position = quadMistCardUI.transform.position + Hand.CURSOR_OFFSET_L;
					}
				}
				else
				{
					quadMistCardUI.Black = true;
					this.CardCursor.SetBlackState();
				}
				break;
			}
			IL_1BB:
			i++;
			continue;
			goto IL_1BB;
		}
	}

	private void OnListChanged()
	{
		Int32 select = this.Select;
		this.Select = -1;
		for (Int32 i = 0; i < (Int32)this.cardUIs.Length; i++)
		{
			if (i < this.Count)
			{
				this.cardUIs[i].gameObject.SetActive(true);
				this.cardUIs[i].Data = this[i];
			}
			else
			{
				this.cardUIs[i].gameObject.SetActive(false);
			}
		}
		this.OnListChanged2();
		this.Select = select;
	}

	private void OnListChanged2()
	{
		Int32 i = 0;
		while (i < (Int32)this.cardUIs.Length)
		{
			QuadMistCardUI quadMistCardUI = this.cardUIs[i];
			switch (this.State)
			{
			case Hand.STATE.ENEMY_HIDE:
				quadMistCardUI.Flip = true;
				base.transform.localPosition = Hand.ENEMY_POSITION;
				quadMistCardUI.transform.localPosition = new Vector3(0f, -3.2f, 0f);
				break;
			case Hand.STATE.ENEMY_PLAY:
				quadMistCardUI.Flip = true;
				base.transform.localPosition = Hand.ENEMY_POSITION;
				quadMistCardUI.transform.localPosition = Hand.ENEMY_TILING * (Single)i;
				break;
			case Hand.STATE.ENEMY_POSTGAME:
				quadMistCardUI.Flip = false;
				base.transform.localPosition = Hand.ENEMY_POST_POSITION;
				quadMistCardUI.transform.localPosition = Hand.DEFAULT_TILING(this.Count) * (Single)i;
				break;
			case Hand.STATE.PLAYER_PREGAME:
				base.transform.localPosition = Hand.PLAYER_POSITION;
				quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_S * (Single)i;
				quadMistCardUI.transform.localScale = Hand.PLAYER_SCALE_S;
				break;
			case Hand.STATE.PLAYER_SELECT_BOARD:
				base.transform.localPosition = Hand.PLAYER_POSITION;
				if (this.Count == 5)
				{
					quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_S * (Single)i;
					quadMistCardUI.transform.localScale = Hand.PLAYER_SCALE_S;
				}
				else
				{
					quadMistCardUI.transform.localPosition = Hand.PLAYER_TILING_L * (Single)i;
					quadMistCardUI.transform.localScale = Vector3.one;
				}
				break;
			case Hand.STATE.PLAYER_POSTGAME:
				base.transform.localPosition = Hand.PLAYER_POST_POSITION;
				quadMistCardUI.transform.localScale = Vector3.one;
				quadMistCardUI.transform.localPosition = Hand.DEFAULT_TILING(this.Count) * (Single)i;
				break;
			}
			IL_1E8:
			i++;
			continue;
			goto IL_1E8;
		}
	}

	private void SlideTo(Transform t, Vector3 mov, Single time)
	{
		base.StartCoroutine(Anim.MoveLerp(t, mov, time, true));
	}

	private IEnumerator SlideCardBackToTheFormerPosition(Transform origin, Vector3 target, Single duration)
	{
		this.cardAnimatingCount++;
		yield return base.StartCoroutine(Anim.MoveLerp(origin, target, duration, true));
		this.cardAnimatingCount--;
		yield break;
	}

	private void SlideToDelayed(Transform t, Vector3 mov, Single time, Single delayed)
	{
		base.StartCoroutine(Anim.Sequence(new IEnumerator[]
		{
			Anim.Delay(delayed),
			Anim.MoveLerp(t, mov, time, true)
		}));
	}

	private void SlideToEnemyHand(Transform t, Vector3 mov, Int32 i)
	{
		base.StartCoroutine(this.SlideEnemyCard(t, mov, i));
	}

	private void ScaleTo(Transform t, Vector3 scal, Single time)
	{
		if (!Mathf.Approximately(t.localScale.x, scal.x) && !Mathf.Approximately(t.localScale.y, scal.y))
		{
			base.StartCoroutine(Anim.ScaleLerp(t.transform, scal, time));
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
			{
				break;
			}
			t.transform.localPosition = new Vector3(pos.x, (Single)(-(Single)(trans * trans + i * 14)) * 0.01f, (Single)(-(Single)i));
			yield return base.StartCoroutine(Anim.Tick());
		}
		SoundEffect.Play(QuadMistSoundID.MINI_SE_CARD_MOVE);
		t.transform.localPosition = fin;
		yield break;
	}

	public void ApplyScaleToSelectedCard(Int32 cardIndex)
	{
		QuadMistCardUI quadMistCardUI = this.cardUIs[cardIndex];
		if (this.Count == 5)
		{
			this.ScaleTo(quadMistCardUI.transform, Hand.PLAYER_SCALE_L, Hand.SCALE_SPEED);
		}
	}

	public void SetCardScaleBecauseOfUserCancellation()
	{
		if (this.Count == 5)
		{
			for (Int32 i = 0; i < 5; i++)
			{
				QuadMistCardUI quadMistCardUI = this.cardUIs[i];
				this.ScaleTo(quadMistCardUI.transform, Hand.PLAYER_SCALE_S, Hand.SCALE_SPEED);
			}
		}
	}

	public void Add(QuadMistCard item)
	{
		this.cards.Add(item);
		this.OnListChanged();
	}

	public void ReplaceCard(Int32 cardsArrayIndex, QuadMistCard newCard)
	{
		this.cards[cardsArrayIndex].id = newCard.id;
		this.cards[cardsArrayIndex].atk = newCard.atk;
		this.cards[cardsArrayIndex].arrow = newCard.arrow;
		this.cards[cardsArrayIndex].type = newCard.type;
		this.cards[cardsArrayIndex].pdef = newCard.pdef;
		this.cards[cardsArrayIndex].mdef = newCard.mdef;
		this.OnListChanged();
	}

	public void AddWithoutChanged(QuadMistCard item)
	{
		this.cards.Add(item);
		Int32 select = this.Select;
		this.Select = -1;
		for (Int32 i = 0; i < (Int32)this.cardUIs.Length; i++)
		{
			if (i < this.Count)
			{
				this.cardUIs[i].gameObject.SetActive(true);
				this.cardUIs[i].Data = this[i];
			}
			else
			{
				this.cardUIs[i].gameObject.SetActive(false);
			}
		}
		this.Select = select;
	}

	public Int32 IndexOf(QuadMistCard item)
	{
		return this.cards.IndexOf(item);
	}

	public void Insert(Int32 index, QuadMistCard item)
	{
		this.cards.Insert(index, item);
		this.OnListChanged();
	}

	public Boolean Remove(QuadMistCard card)
	{
		Boolean result = this.cards.Remove(card);
		this.OnListChanged();
		return result;
	}

	public void RemoveAt(Int32 index)
	{
		this.cards.RemoveAt(index);
		this.OnListChanged();
	}

	public QuadMistCard this[Int32 index]
	{
		get
		{
			return this.cards[index];
		}
		set
		{
			this.cards[index] = value;
		}
	}

	public void Clear()
	{
		this.cards.Clear();
		this.OnListChanged();
	}

	public Boolean Contains(QuadMistCard item)
	{
		return this.cards.Contains(item);
	}

	public void CopyTo(QuadMistCard[] array, Int32 arrayIndex)
	{
		this.cards.CopyTo(array, arrayIndex);
	}

	public Int32 Count
	{
		get
		{
			return this.cards.Count;
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
		foreach (QuadMistCard item in this.cards)
		{
			yield return item;
		}
		yield break;
	}

	public List<QuadMistCard> GetQuadMistCards()
	{
		return this.cards;
	}

	private static Int32 MAX_CARDS = 10;

	private static Single RECOVER_SPEED = 22f;

	private static Single SCALE_SPEED = 0.2f;

	private static Vector3 ENEMY_POSITION = new Vector3(0.16f, -0.26f, -1f);

	private static Vector3 ENEMY_POST_POSITION = new Vector3(0.16f, -0.25f, -1f);

	private static Vector3 ENEMY_TILING = new Vector3(0f, -0.14f, -1f);

	private static Vector3 PLAYER_SCALE_S = new Vector3(0.8f, 0.8f, 1f);

	private static Vector3 PLAYER_SCALE_L = new Vector3(1f, 1f, 1f);

	private static Vector3 PLAYER_TILING_S = new Vector3(0f, -0.42f, 0f);

	private static Vector3 PLAYER_TILING_L = new Vector3(0f, -0.52f, 0f);

	private static Vector3 PLAYER_POSITION = new Vector3(2.6f, -0.08f, -1f);

	private static Vector3 PLAYER_POST_POSITION = new Vector3(2.6f, -0.25f, -1f);

	private static Vector3 CURSOR_OFFSET_S = new Vector3(-0.15f, -0.06f, -1f);

	private static Vector3 CURSOR_OFFSET_L = new Vector3(-0.18f, -0.03f, -1f);

	private static Vector3 CURSOR_OFFSET_REVERSE = new Vector3(0.58f, -0.05f, -1f);

	public QuadMistCursor cursor;

	public QuadMistCardUI[] cardUIs = new QuadMistCardUI[Hand.MAX_CARDS];

	private QuadMistCardUI shadowCard;

	public QuadMistCardCursor CardCursor;

	public GameObject cardPrefab;

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
