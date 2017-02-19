using System;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
	public String Status
	{
		set
		{
			this.status.Text = value;
		}
	}

	public Int32 ID
	{
		get
		{
			return this.character.ID;
		}
		set
		{
			if (value < 100)
			{
				this.character.ID = value;
				this.block.gameObject.SetActive(false);
			}
			else
			{
				this.block.ID = value - 100;
				this.block.gameObject.SetActive(true);
			}
		}
	}

	public Int32 Side
	{
		get
		{
			if (this.background != (UnityEngine.Object)null)
			{
				return this.background.ID;
			}
			return 0;
		}
		set
		{
			value = Mathf.Max(0, Mathf.Min(value, 1));
			if (this.background != (UnityEngine.Object)null)
			{
				this.background.ID = value;
			}
			if (this.frame != (UnityEngine.Object)null)
			{
				this.frame.ID = value + 5;
			}
		}
	}

	public Boolean Flip
	{
		get
		{
			return this.flip.gameObject.activeSelf;
		}
		set
		{
			this.flip.gameObject.SetActive(value);
		}
	}

	public Boolean Small
	{
		set
		{
			if (value)
			{
				base.transform.localScale = new Vector3(QuadMistCardUI.SIZESMALL_W / QuadMistCardUI.SIZE_W, QuadMistCardUI.SIZESMALL_H / QuadMistCardUI.SIZE_H, 1f);
				this.status.transform.localPosition = CardDisplay.STATUS_SMALL;
			}
			else
			{
				base.transform.localScale = Vector3.one;
				this.status.transform.localPosition = CardDisplay.STATUS;
			}
		}
	}

	public Boolean Select
	{
		get
		{
			return this.select.gameObject.activeSelf;
		}
		set
		{
			this.select.gameObject.SetActive(value);
		}
	}

	public Boolean IsBlock
	{
		get
		{
			return this.block.gameObject.activeSelf;
		}
	}

	public Boolean Contains(Vector3 worldPoint)
	{
		return this.background.GetComponent<SpriteClickable>().Contains(worldPoint);
	}

	public static Vector3 STATUS = new Vector3(0f, 0.02f, -0.02f);

	public static Vector3 STATUS_SMALL = new Vector3(-0.05f, 0.13f, -0.02f);

	public SpriteText status;

	public SpriteDisplay select;

	public SpriteDisplay character;

	public SpriteDisplay flip;

	public SpriteDisplay frame;

	public SpriteDisplay background;

	public SpriteDisplay block;
}
