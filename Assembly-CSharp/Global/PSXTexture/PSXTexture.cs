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
		Int32 num = (clutX << 4) + (clutY << 10);
		Int32 num2 = 0;
		switch (TP)
		{
		case 0:
			for (Int32 i = 0; i < h; i++)
			{
				Int32 num3 = (TY + i << 10) + TX;
				for (Int32 j = 0; j < (int) w >> 2; j++)
				{
					UInt16 num4 = PSXTextureMgr.originalVram[num3];
					Int32 num5 = (Int32)(num4 & 15);
					this.ConvertABGR16toABGR32(num2, num + num5);
					num2++;
					num5 = (num4 >> 4 & 15);
					this.ConvertABGR16toABGR32(num2, num + num5);
					num2++;
					num5 = (num4 >> 8 & 15);
					this.ConvertABGR16toABGR32(num2, num + num5);
					num2++;
					num5 = num4 >> 12;
					this.ConvertABGR16toABGR32(num2, num + num5);
					num2++;
					num3++;
				}
			}
			break;
		case 1:
			for (Int32 k = 0; k < h; k++)
			{
				Int32 num6 = (TY + k << 10) + TX;
				for (Int32 l = 0; l < (w >> 1); l++)
				{
					UInt16 num7 = PSXTextureMgr.originalVram[num6];
					Int32 num8 = (Int32)(num7 & 255);
					this.ConvertABGR16toABGR32(num2, num + num8);
					num2++;
					num8 = num7 >> 8;
					this.ConvertABGR16toABGR32(num2, num + num8);
					num2++;
					num6++;
				}
			}
			break;
		case 2:
			for (Int32 m = 0; m < h; m++)
			{
				Int32 num9 = (TY + m << 10) + TX;
				for (Int32 n = 0; n < w; n++)
				{
					this.ConvertABGR16toABGR32(num2, num9);
					num2++;
					num9++;
				}
			}
			break;
		default:
			for (Int32 num10 = 0; num10 < h; num10++)
			{
				Int32 num11 = (TY + num10 << 10) + TX;
				for (Int32 num12 = 0; num12 < w; num12++)
				{
					PSXTexture.pixels[num2].r = (Byte)(PSXTextureMgr.originalVram[num11] & 255);
					PSXTexture.pixels[num2].g = (Byte)((PSXTextureMgr.originalVram[num11] & 65280) >> 8);
					PSXTexture.pixels[num2].b = 0;
					PSXTexture.pixels[num2].a = Byte.MaxValue;
					num2++;
					num11++;
				}
			}
			break;
		}
	}

	private void ConvertABGR16toABGR32(Int32 uniIndex, Int32 psxIndex)
	{
		UInt16 num = PSXTextureMgr.originalVram[psxIndex];
		PSXTexture.pixels[uniIndex].r = (Byte)((num & 31) << 3);
		PSXTexture.pixels[uniIndex].g = (Byte)((num & 992) >> 2);
		PSXTexture.pixels[uniIndex].b = (Byte)((num & 31744) >> 7);
		if ((num & 32768) != 0)
		{
			PSXTexture.pixels[uniIndex].a = Byte.MaxValue;
		}
		else if ((num & 32767) != 0)
		{
			PSXTexture.pixels[uniIndex].a = Byte.MaxValue;
		}
		else
		{
			PSXTexture.pixels[uniIndex].a = 0;
		}
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
