using System;
using UnityEngine;

public class SpriteText : MonoBehaviour
{
	public String Text
	{
		get
		{
			return this._text;
		}
		set
		{
			if (this.alignment == TextAlignment.Left)
			{
				for (Int32 i = 0; i < (Int32)this.letters.Length; i++)
				{
					if (i < value.Length)
					{
						this.letters[i].sprite = this.font[(Int32)value[i]];
					}
					else
					{
						this.letters[i].sprite = (Sprite)null;
					}
				}
			}
			if (this.alignment == TextAlignment.Right)
			{
				String text = String.Empty;
				for (Int32 j = value.Length - 1; j >= 0; j--)
				{
					text += value[j];
				}
				for (Int32 k = 0; k < (Int32)this.letters.Length; k++)
				{
					if (k < text.Length)
					{
						this.letters[(Int32)this.letters.Length - k - 1].sprite = this.font[(Int32)text[k]];
					}
					else
					{
						this.letters[(Int32)this.letters.Length - k - 1].sprite = (Sprite)null;
					}
				}
			}
			this._text = value;
		}
	}

	public SpriteFont font;

	public SpriteRenderer[] letters;

	public TextAlignment alignment;

	private String _text;
}
