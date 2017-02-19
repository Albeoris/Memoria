using System;
using System.IO;
using UnityEngine;

public class PSXVram
{
	public PSXVram(UInt32 width, UInt32 height, Boolean createBuffer)
	{
		this.Init(width, height, createBuffer);
	}

	public PSXVram(Boolean createBuffer)
	{
		this.Init(1024u, 512u, createBuffer);
	}

	~PSXVram()
	{
		this.rawData = null;
	}

	public void Init(UInt32 width, UInt32 height, Boolean createBuffer)
	{
		this.width = width;
		this.height = height;
		this.pixelCount = width * height;
		if (createBuffer)
		{
			this.rawData = new Byte[this.pixelCount * 2u];
			this.Clear(false);
		}
		else
		{
			this.rawData = null;
		}
	}

	public void Clear(Boolean withTransparent = false)
	{
		if (this.rawData == null)
		{
			return;
		}
		UInt16 num = 32768;
		if (withTransparent)
		{
			num = 0;
		}
		for (UInt32 num2 = 0u; num2 < this.pixelCount; num2 += 1u)
		{
			this.rawData[(Int32)((UIntPtr)(num2 * 2u))] = (Byte)(num & 255);
			this.rawData[(Int32)((UIntPtr)(num2 * 2u + 1u))] = (Byte)(num >> 8 & 255);
		}
	}

	public void LoadTIMs(String baseResPath)
	{
		for (Int32 i = 0; i < 16; i++)
		{
			String name = baseResPath + i.ToString("D4") + ".tim";
			TextAsset textAsset = AssetManager.Load<TextAsset>(name, false);
			if (textAsset == (UnityEngine.Object)null)
			{
				break;
			}
			Byte[] bytes = textAsset.bytes;
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes)))
			{
				if (binaryReader.ReadUInt32() == 16u)
				{
					UInt32 num = binaryReader.ReadUInt32();
					UInt32 num2 = num & 7u;
					if ((num & 8u) == 8u)
					{
						binaryReader.ReadUInt32();
						UInt16 num3 = binaryReader.ReadUInt16();
						UInt16 num4 = binaryReader.ReadUInt16();
						UInt16 num5 = binaryReader.ReadUInt16();
						UInt16 num6 = binaryReader.ReadUInt16();
						Int32 count = (Int32)((Int64)(num5 * num6) * 2L);
						Byte[] array = binaryReader.ReadBytes(count);
						for (Int32 j = 0; j < (Int32)num6; j++)
						{
							Int32 index = ArrayUtil.GetIndex(0, j, (Int32)(num5 * 2), (Int32)num6);
							Int32 index2 = ArrayUtil.GetIndex((Int32)(num3 * 2), (Int32)num4 + j, (Int32)(this.width * 2u), (Int32)this.height);
							for (Int32 k = 0; k < (Int32)(num5 * 2); k++)
							{
								this.rawData[index2++] = array[index++];
							}
						}
					}
					UInt32 num7 = binaryReader.ReadUInt32();
					UInt16 num8 = binaryReader.ReadUInt16();
					UInt16 num9 = binaryReader.ReadUInt16();
					UInt16 num10 = binaryReader.ReadUInt16();
					UInt16 num11 = binaryReader.ReadUInt16();
					Int32 count2 = (Int32)(num7 - 4u - 2u - 2u - 2u - 2u);
					if (num2 == 0u)
					{
						count2 = (Int32)((Single)(num10 * num11) * 0.25f * 2f);
					}
					else if (num2 == 1u)
					{
						count2 = (Int32)((Single)(num10 * num11) * 0.5f * 2f);
					}
					else if (num2 == 2u)
					{
						count2 = (Int32)((Int64)(num10 * num11) * 2L);
					}
					else if (num2 == 3u)
					{
					}
					Byte[] array2 = binaryReader.ReadBytes(count2);
					for (Int32 l = 0; l < (Int32)num11; l++)
					{
						Int32 index3 = ArrayUtil.GetIndex(0, l, (Int32)(num10 * 2), (Int32)num11);
						Int32 index4 = ArrayUtil.GetIndex((Int32)(num8 * 2), (Int32)num9 + l, (Int32)(this.width * 2u), (Int32)this.height);
						for (Int32 m = 0; m < (Int32)(num10 * 2); m++)
						{
							this.rawData[index4++] = array2[index3++];
						}
					}
				}
			}
		}
	}

	public void LoadRAWVram(String resPath)
	{
		TextAsset textAsset = Resources.Load<TextAsset>(resPath);
		this.rawData = textAsset.bytes;
	}

	public void LoadRAWVram(Byte[] data)
	{
		this.rawData = data;
	}

	public void LoadImage(Rect rect, BinaryReader data)
	{
		Int32 num = (Int32)rect.width;
		Int32 num2 = (Int32)rect.height;
		for (Int32 i = 0; i < num2; i++)
		{
			for (Int32 j = 0; j < num; j++)
			{
				Int32 num3 = ((Int32)rect.y + i) * 1024 + ((Int32)rect.x + j);
				this.rawData[num3 * 2] = data.ReadByte();
				this.rawData[num3 * 2 + 1] = data.ReadByte();
			}
		}
	}

	public Color32[] CreateBufferColor32()
	{
		Color32[] array = new Color32[this.pixelCount];
		for (UInt32 num = 0u; num < this.pixelCount; num += 1u)
		{
			UInt32 num2 = num * 2u;
			UInt16 raw = (UInt16)((Int32)this.rawData[(Int32)((UIntPtr)num2)] | (Int32)this.rawData[(Int32)((UIntPtr)(num2 + 1u))] << 8);
			PSX.ConvertColor16toARGB(raw, out array[(Int32)((UIntPtr)num)].a, out array[(Int32)((UIntPtr)num)].r, out array[(Int32)((UIntPtr)num)].g, out array[(Int32)((UIntPtr)num)].b);
		}
		return array;
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

	private UInt16 GetPixel(Int32 pixelIndex)
	{
		Int32 num = pixelIndex * 2;
		return (UInt16)((Int32)this.rawData[num] | (Int32)this.rawData[num + 1] << 8);
	}

	public Color32[] CreateBufferColor32(Int32 tp, Int32 tx, Int32 ty, Int32 u, Int32 v, Int32 w, Int32 h, Int32 clutX, Int32 clutY, Boolean enableSemiTransparent, Int32 abr)
	{
		Color32[] array = new Color32[w * h];
		ty = PSXGPU.GetPixelCoorTY(ty);
		tx = PSXGPU.GetPixelCoorTX(tx);
		clutY = PSXGPU.GetPixelCoorClutY(clutY);
		clutX = PSXGPU.GetPixelCoorClutX(clutX);
		if (tp == 0)
		{
			Int32 num = w / 4;
			if (w % 4 != 0)
			{
				num++;
			}
			for (Int32 i = 0; i < h; i++)
			{
				Int32 num2 = 0;
				for (Int32 j = 0; j < num; j++)
				{
					Int32 num3 = tx + u / 4 + j;
					Int32 num4 = ty + v + i;
					Int32 pixelIndex = num3 + num4 * 1024;
					UInt16 pixel = this.GetPixel(pixelIndex);
					Int32 num5 = j * 4 + i * w;
					if (num2 == w)
					{
						break;
					}
					Int32 dataFromBit = (Int32)this.GetDataFromBit((UInt32)pixel, 0, 3);
					array[num5] = PSX.ConvertABGR16toABGR32(this.GetPixel(clutX + clutY * 1024 + dataFromBit), enableSemiTransparent, abr);
					num2++;
					num5++;
					if (num2 == w)
					{
						break;
					}
					dataFromBit = (Int32)this.GetDataFromBit((UInt32)pixel, 4, 7);
					array[num5] = PSX.ConvertABGR16toABGR32(this.GetPixel(clutX + clutY * 1024 + dataFromBit), enableSemiTransparent, abr);
					num2++;
					num5++;
					if (num2 == w)
					{
						break;
					}
					dataFromBit = (Int32)this.GetDataFromBit((UInt32)pixel, 8, 11);
					array[num5] = PSX.ConvertABGR16toABGR32(this.GetPixel(clutX + clutY * 1024 + dataFromBit), enableSemiTransparent, abr);
					num2++;
					num5++;
					if (num2 == w)
					{
						break;
					}
					dataFromBit = (Int32)this.GetDataFromBit((UInt32)pixel, 12, 15);
					array[num5] = PSX.ConvertABGR16toABGR32(this.GetPixel(clutX + clutY * 1024 + dataFromBit), enableSemiTransparent, abr);
					num2++;
				}
			}
		}
		else if (tp == 1)
		{
			Int32 num6 = w / 2;
			if (w % 2 != 0)
			{
				num6++;
			}
			for (Int32 k = 0; k < h; k++)
			{
				Int32 num7 = 0;
				for (Int32 l = 0; l < num6; l++)
				{
					Int32 num8 = tx + u / 2 + l;
					Int32 num9 = ty + v + k;
					Int32 pixelIndex2 = num8 + num9 * 1024;
					UInt16 pixel2 = this.GetPixel(pixelIndex2);
					Int32 num10 = l * 2 + k * w;
					if (num7 == w)
					{
						break;
					}
					Int32 dataFromBit2 = (Int32)this.GetDataFromBit((UInt32)pixel2, 0, 7);
					array[num10] = PSX.ConvertABGR16toABGR32(this.GetPixel(clutX + clutY * 1024 + dataFromBit2), enableSemiTransparent, abr);
					num7++;
					num10++;
					if (num7 == w)
					{
						break;
					}
					dataFromBit2 = (Int32)this.GetDataFromBit((UInt32)pixel2, 8, 15);
					array[num10] = PSX.ConvertABGR16toABGR32(this.GetPixel(clutX + clutY * 1024 + dataFromBit2), enableSemiTransparent, abr);
					num7++;
				}
			}
		}
		else if (tp == 2)
		{
			for (Int32 m = 0; m < h; m++)
			{
				for (Int32 n = 0; n < w; n++)
				{
					Int32 num11 = tx + u + n;
					Int32 num12 = ty + v + m;
					Int32 pixelIndex3 = num11 + num12 * 1024;
					Int32 num13 = n + m * w;
					array[num13] = PSX.ConvertABGR16toABGR32(this.GetPixel(pixelIndex3), enableSemiTransparent, abr);
				}
			}
		}
		return array;
	}

	public UInt32 width;

	public UInt32 height;

	public UInt32 pixelCount;

	public Byte[] rawData;
}
