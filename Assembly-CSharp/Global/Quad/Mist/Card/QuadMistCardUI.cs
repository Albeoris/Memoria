using Memoria;
using System;
using System.Collections;
using UnityEngine;

public class QuadMistCardUI : MonoBehaviour
{
	private void Update()
	{
		if (cardDisplay.select.gameObject.activeSelf)
		{
			if (!isToDisplay)
			{
				cardDisplay.select.gameObject.transform.localPosition = farAwayPosition;
			}
			else
			{
				cardDisplay.select.gameObject.transform.localPosition = originalPosition;
			}
			if (DateTime.Now.Millisecond <= 500)
			{
				isToDisplay = true;
			}
			else
			{
				isToDisplay = false;
			}
		}
	}

	public Boolean Small
	{
		get
		{
			return _small;
		}
		set
		{
			_small = value;
			cardEffect.Small = _small;
			cardDisplay.Small = _small;
			cardArrows.Small = _small;
		}
	}

	public Single White
	{
		get
		{
			return cardEffect.White;
		}
		set
		{
			cardEffect.White = value;
		}
	}

	public Boolean Black
	{
		get
		{
			return cardEffect.Black > 0f;
		}
		set
		{
			cardEffect.Black = ((!value) ? 0f : 0.5f);
		}
	}

	public Int32 Side
	{
		get
		{
			return (Int32)Data.side;
		}
		set
		{
			_data.side = (Byte)value;
			cardDisplay.Side = value;
			if ((cardArrows.Arrow == 255) && Configuration.Mod.TranceSeek && Configuration.TetraMaster.TripleTriad < 2)
			{
				cardDisplay.frame.ID = 7 + value;

			}
        }
	}

	public Boolean Flip
	{
		get
		{
			return cardDisplay.Flip;
		}
		set
		{
			cardDisplay.Flip = value;
		}
	}

	public Boolean IsBlock
	{
		get
		{
			return cardDisplay.IsBlock;
		}
	}

	public Boolean Select
	{
		get
		{
			return cardDisplay.Select;
		}
		set
		{
			cardDisplay.Select = value;
		}
	}

	public QuadMistCard Data
	{
		get
		{
			return _data;
		}
		set
		{
			_data = value;
			if (value != null)
			{
				gameObject.SetActive(true);
				cardDisplay.Status = Data.ToString();
				cardDisplay.ID = (Int32)Data.id;
				cardDisplay.Side = (Int32)Data.side;
				if (Configuration.TetraMaster.TripleTriad < 2)
				{
                    cardArrows.Arrow = (Int32)Data.arrow;
                }
				else
				{
                    cardArrows.Arrow = (Int32)Data.arrow;
                }
                if ((cardArrows.Arrow == 255) && Configuration.Mod.TranceSeek && Configuration.TetraMaster.TripleTriad < 2)
                {
					cardDisplay.frame.ID = 7 + Data.side;
				}
            }
			else
			{
				gameObject.SetActive(false);
			}
		}
	}

	public Vector3 Size
	{
		get
		{
			return (!Small) ? new Vector3(QuadMistCardUI.SIZE_W, QuadMistCardUI.SIZE_H) : new Vector3(QuadMistCardUI.SIZESMALL_W, QuadMistCardUI.SIZESMALL_H, 0f);
        }
	}

	public IEnumerator FlashBattle(Action<QuadMistCardUI> actionThis)
	{
		Action action = delegate
		{
			actionThis(this);
		};
		return cardEffect.Flash3Battle(new Action[]
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
		return cardEffect.Flash2Combo(arrow, new Action[]
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
		return cardEffect.Flash1Normal(new Action[]
		{
			action
		});
	}

	public IEnumerator FlashArrow(Byte mask)
	{
		return cardEffect.FlashArrows((Int32)mask);
	}

	public void ResetEffect()
	{
		White = 0f;
		Black = false;
	}

	public Boolean Contains(Vector3 worldPoint)
	{
		return cardDisplay.Contains(worldPoint);
	}

	public static Int32 ENEMY_SIDE = 1;

	public static Int32 PLAYER_SIDE;

	public static Single SIZE_W = ((Configuration.TetraMaster.TripleTriad < 2) ? 0.42f : 0.54f); // Position des cartes

	public static Single SIZE_H = ((Configuration.TetraMaster.TripleTriad < 2) ? 0.51f : 0.64f); // Position des cartes

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
