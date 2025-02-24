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
                this.mLength = this.sequence.Length;
            return this.mLength;
        }
    }

    public Int32 offsetX => this.mOffsetX;
    public Int32 offsetY => this.mOffsetY;
    public Int32 width => this.mWidth;
    public Int32 height => this.mHeight;
    public Int32 advance => this.mAdvance;
    public Rect uvRect => this.mUV;

    public void MarkAsChanged()
    {
        this.mIsValid = false;
    }

    public Boolean Validate(UIAtlas atlas)
    {
        if (atlas == null)
            return false;
        if (!this.mIsValid)
        {
            if (String.IsNullOrEmpty(this.spriteName))
                return false;
            this.mSprite = atlas?.GetSprite(this.spriteName);
            if (this.mSprite != null)
            {
                Texture texture = atlas.texture;
                if (texture == null)
                {
                    this.mSprite = null;
                }
                else
                {
                    this.mUV = new Rect(this.mSprite.x, this.mSprite.y, this.mSprite.width, this.mSprite.height);
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
        return this.mSprite != null;
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
