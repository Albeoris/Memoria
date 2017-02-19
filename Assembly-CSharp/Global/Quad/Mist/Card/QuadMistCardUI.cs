using System;
using System.Collections;
using UnityEngine;

public class QuadMistCardUI : MonoBehaviour
{
	private void Update()
	{
		if (this.cardDisplay.select.gameObject.activeSelf)
		{
			if (!this.isToDisplay)
			{
				this.cardDisplay.select.gameObject.transform.localPosition = this.farAwayPosition;
			}
			else
			{
				this.cardDisplay.select.gameObject.transform.localPosition = this.originalPosition;
			}
			if (DateTime.Now.Millisecond <= 500)
			{
				this.isToDisplay = true;
			}
			else
			{
				this.isToDisplay = false;
			}
		}
	}

	public Boolean Small
	{
		get
		{
			return this._small;
		}
		set
		{
			this._small = value;
			this.cardEffect.Small = this._small;
			this.cardDisplay.Small = this._small;
			this.cardArrows.Small = this._small;
		}
	}

	public Single White
	{
		get
		{
			return this.cardEffect.White;
		}
		set
		{
			this.cardEffect.White = value;
		}
	}

	public Boolean Black
	{
		get
		{
			return this.cardEffect.Black > 0f;
		}
		set
		{
			this.cardEffect.Black = ((!value) ? 0f : 0.5f);
		}
	}

	public Int32 Side
	{
		get
		{
			return (Int32)this.Data.side;
		}
		set
		{
			this._data.side = (Byte)value;
			this.cardDisplay.Side = value;
		}
	}

	public Boolean Flip
	{
		get
		{
			return this.cardDisplay.Flip;
		}
		set
		{
			this.cardDisplay.Flip = value;
		}
	}

	public Boolean IsBlock
	{
		get
		{
			return this.cardDisplay.IsBlock;
		}
	}

	public Boolean Select
	{
		get
		{
			return this.cardDisplay.Select;
		}
		set
		{
			this.cardDisplay.Select = value;
		}
	}

	public QuadMistCard Data
	{
		get
		{
			return this._data;
		}
		set
		{
			this._data = value;
			if (value != null)
			{
				base.gameObject.SetActive(true);
				this.cardDisplay.Status = this.Data.ToString();
				this.cardDisplay.ID = (Int32)this.Data.id;
				this.cardDisplay.Side = (Int32)this.Data.side;
				this.cardArrows.Arrow = (Int32)this.Data.arrow;
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	public Vector3 Size
	{
		get
		{
			return (!this.Small) ? new Vector3(QuadMistCardUI.SIZE_W, QuadMistCardUI.SIZE_H) : new Vector3(QuadMistCardUI.SIZESMALL_W, QuadMistCardUI.SIZESMALL_H, 0f);
		}
	}

	public IEnumerator FlashBattle(Action<QuadMistCardUI> actionThis)
	{
		Action action = delegate
		{
			actionThis(this);
		};
		return this.cardEffect.Flash3Battle(new Action[]
		{
			action
		});
	}

	public IEnumerator FlashCombo(CardArrow.Type arrow, Action<QuadMistCardUI> actionThis)
	{
		Action action = delegate
		{
			actionThis(this);
		};
		return this.cardEffect.Flash2Combo(arrow, new Action[]
		{
			action
		});
	}

	public IEnumerator FlashNormal(Action<QuadMistCardUI> actionThis)
	{
		Action action = delegate
		{
			actionThis(this);
		};
		return this.cardEffect.Flash1Normal(new Action[]
		{
			action
		});
	}

	public IEnumerator FlashArrow(Byte mask)
	{
		return this.cardEffect.FlashArrows((Int32)mask);
	}

	public void ResetEffect()
	{
		this.White = 0f;
		this.Black = false;
	}

	public Boolean Contains(Vector3 worldPoint)
	{
		return this.cardDisplay.Contains(worldPoint);
	}

	public static Int32 ENEMY_SIDE = 1;

	public static Int32 PLAYER_SIDE;

	public static Single SIZE_W = 0.42f;

	public static Single SIZE_H = 0.51f;

	public static Single SIZESMALL_W = 0.34f;

	public static Single SIZESMALL_H = 0.41f;

	public CardEffect cardEffect;

	public CardDisplay cardDisplay;

	public CardArrows cardArrows;

	private Boolean isToDisplay;

	private Vector3 farAwayPosition = new Vector3(-10000f, -10000f, -10000f);

	private Vector3 originalPosition = new Vector3(-0.015f, -0.195f, -0.25f);

	private QuadMistCard _data;

	[HideInInspector]
	[SerializeField]
	private Boolean _small;
}
