using System;
using System.Collections.Generic;

[Serializable]
public class BMGlyph
{
	public Int32 GetKerning(Int32 previousChar)
	{
		if (this.kerning != null && previousChar != 0)
		{
			Int32 i = 0;
			Int32 count = this.kerning.Count;
			while (i < count)
			{
				if (this.kerning[i] == previousChar)
				{
					return this.kerning[i + 1];
				}
				i += 2;
			}
		}
		return 0;
	}

	public void SetKerning(Int32 previousChar, Int32 amount)
	{
		if (this.kerning == null)
		{
			this.kerning = new List<Int32>();
		}
		for (Int32 i = 0; i < this.kerning.Count; i += 2)
		{
			if (this.kerning[i] == previousChar)
			{
				this.kerning[i + 1] = amount;
				return;
			}
		}
		this.kerning.Add(previousChar);
		this.kerning.Add(amount);
	}

	public void Trim(Int32 xMin, Int32 yMin, Int32 xMax, Int32 yMax)
	{
		Int32 num = this.x + this.width;
		Int32 num2 = this.y + this.height;
		if (this.x < xMin)
		{
			Int32 num3 = xMin - this.x;
			this.x += num3;
			this.width -= num3;
			this.offsetX += num3;
		}
		if (this.y < yMin)
		{
			Int32 num4 = yMin - this.y;
			this.y += num4;
			this.height -= num4;
			this.offsetY += num4;
		}
		if (num > xMax)
		{
			this.width -= num - xMax;
		}
		if (num2 > yMax)
		{
			this.height -= num2 - yMax;
		}
	}

	public Int32 index;

	public Int32 x;

	public Int32 y;

	public Int32 width;

	public Int32 height;

	public Int32 offsetX;

	public Int32 offsetY;

	public Int32 advance;

	public Int32 channel;

	public List<Int32> kerning;
}
