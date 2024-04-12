using System;
using UnityEngine;

[RequireComponent(typeof(SpriteDisplay))]
public class CardArrow : MonoBehaviour
{
	public Boolean Small
	{
		get
		{
			return _small;
		}
		set
		{
			_small = value;
			if (Small)
			{
				base.transform.localPosition = CardArrow.POSITION2[(Int32)type];
			}
			else
			{
				base.transform.localPosition = CardArrow.POSITION[(Int32)type];
			}
		}
	}

	public CardArrow.Type type
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
			base.name = value.ToString();
			Small = Small;
			base.GetComponent<SpriteDisplay>().ID = (Int32)type;
		}
	}

	public static Vector3 ToOffset(CardArrow.Type arrow)
	{
		Int32 num = 0;
		Int32 num2 = 0;
		switch (arrow)
		{
			case CardArrow.Type.UP:
				num2 = -1;
				break;
			case CardArrow.Type.RIGHT_UP:
				num2 = -1;
				num = 1;
				break;
			case CardArrow.Type.RIGHT:
				num = 1;
				break;
			case CardArrow.Type.RIGHT_DOWN:
				num = 1;
				num2 = 1;
				break;
			case CardArrow.Type.DOWN:
				num2 = 1;
				break;
			case CardArrow.Type.LEFT_DOWN:
				num = -1;
				num2 = 1;
				break;
			case CardArrow.Type.LEFT:
				num = -1;
				break;
			case CardArrow.Type.LEFT_UP:
				num = -1;
				num2 = -1;
				break;
		}
		return new Vector3((Single)num, (Single)num2, 0f);
	}

	public static Byte Reverse(Byte arrow)
	{
		return (Byte)((Byte)(arrow << 4) | (Byte)(arrow >> 4));
	}

	public static Boolean HasDirection(Byte arrow, CardArrow.Type direction)
	{
		return ((Int32)arrow & 1 << (Int32)direction) > 0;
	}

	public static Int32 CheckDirection(Byte arrow1, Byte arrow2, CardArrow.Type direction)
	{
		Boolean flag = CardArrow.HasDirection(arrow1, direction);
		Boolean flag2 = CardArrow.HasDirection(CardArrow.Reverse(arrow2), direction);
		if (flag && flag2)
		{
			return 2;
		}
		if (flag)
		{
			return 1;
		}
		if (flag2)
		{
			return -1;
		}
		return 0;
	}

	public static Int32 MAX_ARROWNUM = 8;

	private static Vector3[] POSITION = new Vector3[]
	{
		new Vector3(0.1681f, -0.0043f),
		new Vector3(0.3258f, -0.0147f),
		new Vector3(0.3414f, -0.214f),
		new Vector3(0.3261f, -0.4182f),
		new Vector3(0.1679f, -0.4309f),
		new Vector3(0.016f, -0.4175f),
		new Vector3(0.0014f, -0.217f),
		new Vector3(0.0152f, -0.0161f),
		Vector3.zero
	};

	private static Vector3[] POSITION2 = new Vector3[]
	{
		new Vector3(0.13f, 0f),
		new Vector3(0.25f, 0f),
		new Vector3(0.26f, -0.16f),
		new Vector3(0.25f, -0.32f),
		new Vector3(0.13f, -0.33f),
		new Vector3(0f, -0.32f),
		new Vector3(0f, -0.16f),
		Vector3.zero,
		Vector3.zero
	};

	[SerializeField]
	[HideInInspector]
	private Boolean _small;

	[SerializeField]
	[HideInInspector]
	private CardArrow.Type _type;

	public enum Type
	{
		UP,
		RIGHT_UP,
		RIGHT,
		RIGHT_DOWN,
		DOWN,
		LEFT_DOWN,
		LEFT,
		LEFT_UP
	}
}
