using System;
using UnityEngine;

[Serializable]
public class BMSymbol
{
	public Int32 length
	{
		get
		{
			if (this.mLength == 0)
			{
				this.mLength = this.sequence.Length;
			}
			return this.mLength;
		}
	}

	public Int32 offsetX
	{
		get
		{
			return this.mOffsetX;
		}
	}

	public Int32 offsetY
	{
		get
		{
			return this.mOffsetY;
		}
	}

	public Int32 width
	{
		get
		{
			return this.mWidth;
		}
	}

	public Int32 height
	{
		get
		{
			return this.mHeight;
		}
	}

	public Int32 advance
	{
		get
		{
			return this.mAdvance;
		}
	}

	public Rect uvRect
	{
		get
		{
			return this.mUV;
		}
	}

	public void MarkAsChanged()
	{
		this.mIsValid = false;
	}

	public Boolean Validate(UIAtlas atlas)
	{
		if (atlas == (UnityEngine.Object)null)
		{
			return false;
		}
		if (!this.mIsValid)
		{
			if (String.IsNullOrEmpty(this.spriteName))
			{
				return false;
			}
			this.mSprite = ((!(atlas != (UnityEngine.Object)null)) ? null : atlas.GetSprite(this.spriteName));
			if (this.mSprite != null)
			{
				Texture texture = atlas.texture;
				if (texture == (UnityEngine.Object)null)
				{
					this.mSprite = (UISpriteData)null;
				}
				else
				{
					this.mUV = new Rect((Single)this.mSprite.x, (Single)this.mSprite.y, (Single)this.mSprite.width, (Single)this.mSprite.height);
					this.mUV = NGUIMath.ConvertToTexCoords(this.mUV, texture.width, texture.height);
					this.mOffsetX = this.mSprite.paddingLeft;
					this.mOffsetY = this.mSprite.paddingTop;
					this.mWidth = this.mSprite.width;
					this.mHeight = this.mSprite.height;
					this.mAdvance = this.mSprite.width + (this.mSprite.paddingLeft + this.mSprite.paddingRight);
					this.mIsValid = true;
				}
			}
		}
		return this.mSprite != (UISpriteData)null;
	}

	public String sequence;

	public String spriteName;

	private UISpriteData mSprite;

	private Boolean mIsValid;

	private Int32 mLength;

	private Int32 mOffsetX;

	private Int32 mOffsetY;

	private Int32 mWidth;

	private Int32 mHeight;

	private Int32 mAdvance;

	private Rect mUV;
}
