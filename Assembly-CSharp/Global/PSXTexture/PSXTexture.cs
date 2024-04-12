using System;
using UnityEngine;

public class PSXTexture
{
	public PSXTexture()
	{
		this.key = PSXTexture.EMPTY_KEY;
		this.tx = (this.ty = 0);
		this.tw = (this.th = 0);
		this.texture = (Texture2D)null;
	}

	~PSXTexture()
	{
	}

	public void SetFilter(FilterMode filter)
	{
		this.texture.filterMode = filter;
	}

	public void GenTexture(Int32 TP, Int32 TY, Int32 TX, Int32 clutY, Int32 clutX, Int32 w = 256, Int32 h = 256, FilterMode filter = FilterMode.Point, TextureWrapMode wrap = TextureWrapMode.Clamp)
	{
		if (this.texture == (UnityEngine.Object)null || this.tw != w || this.th != h)
		{
			this.texture = new Texture2D(w, h, TextureFormat.ARGB32, false);
			this.tw = w;
			this.th = h;
		}
		this.texture.filterMode = filter;
		this.texture.wrapMode = wrap;
		PSXTexture.pixels = ((this.tw != 256 || this.th != 256) ? new Color32[this.tw * this.th] : PSXTextureMgr.pixels256x256);
		this.tx = TX;
		this.ty = TY;
		this.CreateBufferColor32(TP, TX, TY, this.tw, this.th, clutX, clutY);
		this.texture.SetPixels32(PSXTexture.pixels);
		this.texture.Apply();
		PSXTexture.pixels = null;
	}

	private void CreateBufferColor32(Int32 TP, Int32 TX, Int32 TY, Int32 w, Int32 h, Int32 clutX, Int32 clutY)
	{
		TY <<= 8;
		TX <<= 6;
		Int32 psxIndexBase = (clutX << 4) + (clutY << 10);
		Int32 uniIndex = 0;
		switch (TP)
		{
			case 0:
				for (Int32 y = 0; y < h; y++)
				{
					Int32 vramIndex = (TY + y << 10) + TX;
					for (Int32 x = 0; x < (w >> 2); x++)
					{
						UInt16 paletteInfo = PSXTextureMgr.originalVram[vramIndex];
						Int32 paletteIndex = paletteInfo & 0xF;
						this.ConvertABGR16toABGR32(uniIndex, psxIndexBase + paletteIndex);
						uniIndex++;
						paletteIndex = (paletteInfo >> 4 & 0xF);
						this.ConvertABGR16toABGR32(uniIndex, psxIndexBase + paletteIndex);
						uniIndex++;
						paletteIndex = (paletteInfo >> 8 & 0xF);
						this.ConvertABGR16toABGR32(uniIndex, psxIndexBase + paletteIndex);
						uniIndex++;
						paletteIndex = paletteInfo >> 12;
						this.ConvertABGR16toABGR32(uniIndex, psxIndexBase + paletteIndex);
						uniIndex++;
						vramIndex++;
					}
				}
				break;
			case 1:
				for (Int32 y = 0; y < h; y++)
				{
					Int32 vramIndex = (TY + y << 10) + TX;
					for (Int32 x = 0; x < (w >> 1); x++)
					{
						UInt16 paletteInfo = PSXTextureMgr.originalVram[vramIndex];
						Int32 paletteIndex = paletteInfo & 0xFF;
						this.ConvertABGR16toABGR32(uniIndex, psxIndexBase + paletteIndex);
						uniIndex++;
						paletteIndex = paletteInfo >> 8;
						this.ConvertABGR16toABGR32(uniIndex, psxIndexBase + paletteIndex);
						uniIndex++;
						vramIndex++;
					}
				}
				break;
			case 2:
				for (Int32 y = 0; y < h; y++)
				{
					Int32 vramIndex = (TY + y << 10) + TX;
					for (Int32 x = 0; x < w; x++)
					{
						this.ConvertABGR16toABGR32(uniIndex, vramIndex);
						uniIndex++;
						vramIndex++;
					}
				}
				break;
			default:
				for (Int32 y = 0; y < h; y++)
				{
					Int32 vramIndex = (TY + y << 10) + TX;
					for (Int32 x = 0; x < w; x++)
					{
						PSXTexture.pixels[uniIndex].r = (Byte)(PSXTextureMgr.originalVram[vramIndex] & 0xFF);
						PSXTexture.pixels[uniIndex].g = (Byte)((PSXTextureMgr.originalVram[vramIndex] & 0xFF00) >> 8);
						PSXTexture.pixels[uniIndex].b = 0;
						PSXTexture.pixels[uniIndex].a = Byte.MaxValue;
						uniIndex++;
						vramIndex++;
					}
				}
				break;
		}
	}

	private void ConvertABGR16toABGR32(Int32 uniIndex, Int32 psxIndex)
	{
		UInt16 num = PSXTextureMgr.originalVram[psxIndex];
		PSXTexture.pixels[uniIndex].r = (Byte)((num & 0x1F) << 3);
		PSXTexture.pixels[uniIndex].g = (Byte)((num & 0x3E0) >> 2);
		PSXTexture.pixels[uniIndex].b = (Byte)((num & 0x7C00) >> 7);
		if ((num & 0x8000) != 0)
			PSXTexture.pixels[uniIndex].a = Byte.MaxValue;
		else if ((num & 0x7FFF) != 0)
			PSXTexture.pixels[uniIndex].a = Byte.MaxValue;
		else
			PSXTexture.pixels[uniIndex].a = 0;
	}

	private UInt32 GetDataFromBit(UInt32 input, Int32 start, Int32 end)
	{
		UInt32 num = 0u;
		for (Int32 i = start; i <= end; i++)
		{
			UInt32 num2 = 1u;
			num2 <<= i;
			num |= (input & num2);
		}
		return num >> start;
	}

	public static UInt32 EMPTY_KEY = UInt32.MaxValue;

	private static Color32[] pixels;

	public UInt32 key;

	public Texture2D texture;

	public Int32 tx;

	public Int32 ty;

	private Int32 tw;

	private Int32 th;
}
