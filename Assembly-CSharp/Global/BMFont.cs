using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BMFont
{
	public Boolean isValid
	{
		get
		{
			return this.mSaved.Count > 0;
		}
	}

	public Int32 charSize
	{
		get
		{
			return this.mSize;
		}
		set
		{
			this.mSize = value;
		}
	}

	public Int32 baseOffset
	{
		get
		{
			return this.mBase;
		}
		set
		{
			this.mBase = value;
		}
	}

	public Int32 texWidth
	{
		get
		{
			return this.mWidth;
		}
		set
		{
			this.mWidth = value;
		}
	}

	public Int32 texHeight
	{
		get
		{
			return this.mHeight;
		}
		set
		{
			this.mHeight = value;
		}
	}

	public Int32 glyphCount
	{
		get
		{
			return (Int32)((!this.isValid) ? 0 : this.mSaved.Count);
		}
	}

	public String spriteName
	{
		get
		{
			return this.mSpriteName;
		}
		set
		{
			this.mSpriteName = value;
		}
	}

	public List<BMGlyph> glyphs
	{
		get
		{
			return this.mSaved;
		}
	}

	public BMGlyph GetGlyph(Int32 index, Boolean createIfMissing)
	{
		BMGlyph bmglyph = (BMGlyph)null;
		if (this.mDict.Count == 0)
		{
			Int32 i = 0;
			Int32 count = this.mSaved.Count;
			while (i < count)
			{
				BMGlyph bmglyph2 = this.mSaved[i];
				this.mDict.Add(bmglyph2.index, bmglyph2);
				i++;
			}
		}
		if (!this.mDict.TryGetValue(index, out bmglyph) && createIfMissing)
		{
			bmglyph = new BMGlyph();
			bmglyph.index = index;
			this.mSaved.Add(bmglyph);
			this.mDict.Add(index, bmglyph);
		}
		return bmglyph;
	}

	public BMGlyph GetGlyph(Int32 index)
	{
		return this.GetGlyph(index, false);
	}

	public void Clear()
	{
		this.mDict.Clear();
		this.mSaved.Clear();
	}

	public void Trim(Int32 xMin, Int32 yMin, Int32 xMax, Int32 yMax)
	{
		if (this.isValid)
		{
			Int32 i = 0;
			Int32 count = this.mSaved.Count;
			while (i < count)
			{
				BMGlyph bmglyph = this.mSaved[i];
				if (bmglyph != null)
				{
					bmglyph.Trim(xMin, yMin, xMax, yMax);
				}
				i++;
			}
		}
	}

	[HideInInspector]
	[SerializeField]
	private Int32 mSize = 16;

	[HideInInspector]
	[SerializeField]
	private Int32 mBase;

	[SerializeField]
	[HideInInspector]
	private Int32 mWidth;

	[HideInInspector]
	[SerializeField]
	private Int32 mHeight;

	[HideInInspector]
	[SerializeField]
	private String mSpriteName;

	[HideInInspector]
	[SerializeField]
	private List<BMGlyph> mSaved = new List<BMGlyph>();

	private Dictionary<Int32, BMGlyph> mDict = new Dictionary<Int32, BMGlyph>();
}
