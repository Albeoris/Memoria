using System;
using UnityEngine;

public class CardIconUI : MonoBehaviour
{
	public Int32 ID
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
			this.UpdateIcon();
		}
	}

	public Int32 Count
	{
		get
		{
			return this.count;
		}
		set
		{
			this.count = value;
			if (this.count > 0)
			{
				this.countText.Text = this.count.ToString();
			}
			else
			{
				this.countText.Text = String.Empty;
			}
			this.UpdateIcon();
		}
	}

	public void UpdateIcon()
	{
		if (this.count == 0)
		{
			this.iconSprite.ID = CardIcon.EMPTY_ATTRIBUTE;
		}
		else if (this.count == 1)
		{
			this.iconSprite.ID = (Int32)(CardIcon.GetCardAttribute(this.id) + CardIcon.SINGLE);
		}
		else
		{
			this.iconSprite.ID = (Int32)(CardIcon.GetCardAttribute(this.id) + CardIcon.MULTIPLE);
		}
	}

	public Boolean Contains(Vector3 worldPoint)
	{
		return this.iconSprite.GetComponent<SpriteClickable>().Contains(worldPoint);
	}

	public static Single SIZE_H = 0.15f;

	public static Single SIZE_W = 0.15f;

	public SpriteDisplay iconSprite;

	public SpriteText countText;

	private Int32 id;

	private Int32 count;
}
