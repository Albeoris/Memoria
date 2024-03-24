using System;
using System.Collections;
using UnityEngine;
using Assets.Sources.Scripts.UI.Common;
using Memoria;

public class QuadMistCardUI : MonoBehaviour
{
	private void Update()
	{
		if (cardDisplay.select.gameObject.activeSelf)
		{
			cardDisplay.select.gameObject.transform.localPosition = isToDisplay ? originalPosition : farAwayPosition;
			isToDisplay = DateTime.Now.Millisecond <= 500;
		}
	}

	public Boolean Small
	{
		get => _small;
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
		get => cardEffect.White;
		set => cardEffect.White = value;
	}

	public Boolean Black
	{
		get => cardEffect.Black > 0f;
		set => cardEffect.Black = value ? 0.5f : 0f;
	}

	public Int32 Side
	{
		get => Data.side;
		set
		{
			_data.side = (Byte)value;
			cardDisplay.Side = value;
			if (cardArrows.Arrow == Byte.MaxValue && QuadMistResourceManager.UseArrowGoldenFrame)
				cardDisplay.frame.ID = 7 + value;
        }
	}

	public Boolean Flip
	{
		get => cardDisplay.Flip;
		set => cardDisplay.Flip = value;
	}

	public Boolean IsBlock => cardDisplay.IsBlock;

	public Boolean Select
	{
		get => cardDisplay.Select;
		set => cardDisplay.Select = value;
	}

	public QuadMistCard Data
	{
		get => _data;
		set
		{
			_data = value;
			if (value != null)
			{
                gameObject.SetActive(true);
                cardDisplay.Status = Data.ToString();
				cardDisplay.ID = (Int32)Data.id;
				cardDisplay.Side = Data.side;
                cardArrows.Arrow = Data.arrow;
                if (cardArrows.Arrow == Byte.MaxValue && QuadMistResourceManager.UseArrowGoldenFrame)
					cardDisplay.frame.ID = 7 + Data.side;
                 // cardDisplay.Element = 0;
				 //cardDisplay.background.gameObject.SetActive(false);
                 //cardDisplay.character.gameObject.SetActive(false);
            }
			else
			{
                gameObject.SetActive(false);
			}
		}
	}

	public Vector3 Size => Small ? new Vector3(QuadMistCardUI.SIZESMALL_W, QuadMistCardUI.SIZESMALL_H, 0f) : new Vector3(QuadMistCardUI.SIZE_W, QuadMistCardUI.SIZE_H);

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
	public static Int32 PLAYER_SIDE = 0;
	public static Single SIZE_W = Board.USE_TRIPLETRIAD_BOARD ? 0.54f : 0.42f; // Card positions
	public static Single SIZE_H = Board.USE_TRIPLETRIAD_BOARD ? 0.64f : 0.51f; // Card positions
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
