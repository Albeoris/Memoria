using System;

[Serializable]
public class UISpriteData
{
	public Boolean hasBorder
	{
		get
		{
			return (this.borderLeft | this.borderRight | this.borderTop | this.borderBottom) != 0;
		}
	}

	public Boolean hasPadding
	{
		get
		{
			return (this.paddingLeft | this.paddingRight | this.paddingTop | this.paddingBottom) != 0;
		}
	}

	public void SetRect(Int32 x, Int32 y, Int32 width, Int32 height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	public void SetPadding(Int32 left, Int32 bottom, Int32 right, Int32 top)
	{
		this.paddingLeft = left;
		this.paddingBottom = bottom;
		this.paddingRight = right;
		this.paddingTop = top;
	}

	public void SetBorder(Int32 left, Int32 bottom, Int32 right, Int32 top)
	{
		this.borderLeft = left;
		this.borderBottom = bottom;
		this.borderRight = right;
		this.borderTop = top;
	}

	public void CopyFrom(UISpriteData sd)
	{
		this.name = sd.name;
		this.x = sd.x;
		this.y = sd.y;
		this.width = sd.width;
		this.height = sd.height;
		this.borderLeft = sd.borderLeft;
		this.borderRight = sd.borderRight;
		this.borderTop = sd.borderTop;
		this.borderBottom = sd.borderBottom;
		this.paddingLeft = sd.paddingLeft;
		this.paddingRight = sd.paddingRight;
		this.paddingTop = sd.paddingTop;
		this.paddingBottom = sd.paddingBottom;
	}

	public void CopyBorderFrom(UISpriteData sd)
	{
		this.borderLeft = sd.borderLeft;
		this.borderRight = sd.borderRight;
		this.borderTop = sd.borderTop;
		this.borderBottom = sd.borderBottom;
	}

	public String name = "Sprite";

	public Int32 x;

	public Int32 y;

	public Int32 width;

	public Int32 height;

	public Int32 borderLeft;

	public Int32 borderRight;

	public Int32 borderTop;

	public Int32 borderBottom;

	public Int32 paddingLeft;

	public Int32 paddingRight;

	public Int32 paddingTop;

	public Int32 paddingBottom;
}
