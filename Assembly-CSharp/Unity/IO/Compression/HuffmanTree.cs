using System;

namespace Unity.IO.Compression
{
	internal class HuffmanTree
	{
		public HuffmanTree(Byte[] codeLengths)
		{
			this.codeLengthArray = codeLengths;
			if ((Int32)this.codeLengthArray.Length == 288)
			{
				this.tableBits = 9;
			}
			else
			{
				this.tableBits = 7;
			}
			this.tableMask = (1 << this.tableBits) - 1;
			this.CreateTable();
		}

		public static HuffmanTree StaticLiteralLengthTree
		{
			get
			{
				return HuffmanTree.staticLiteralLengthTree;
			}
		}

		public static HuffmanTree StaticDistanceTree
		{
			get
			{
				return HuffmanTree.staticDistanceTree;
			}
		}

		private static Byte[] GetStaticLiteralTreeLength()
		{
			Byte[] array = new Byte[288];
			for (Int32 i = 0; i <= 143; i++)
			{
				array[i] = 8;
			}
			for (Int32 j = 144; j <= 255; j++)
			{
				array[j] = 9;
			}
			for (Int32 k = 256; k <= 279; k++)
			{
				array[k] = 7;
			}
			for (Int32 l = 280; l <= 287; l++)
			{
				array[l] = 8;
			}
			return array;
		}

		private static Byte[] GetStaticDistanceTreeLength()
		{
			Byte[] array = new Byte[32];
			for (Int32 i = 0; i < 32; i++)
			{
				array[i] = 5;
			}
			return array;
		}

		private UInt32[] CalculateHuffmanCode()
		{
			UInt32[] array = new UInt32[17];
			Byte[] array2 = this.codeLengthArray;
			for (Int32 i = 0; i < (Int32)array2.Length; i++)
			{
				Int32 num = (Int32)array2[i];
				array[num] += 1u;
			}
			array[0] = 0u;
			UInt32[] array3 = new UInt32[17];
			UInt32 num2 = 0u;
			for (Int32 j = 1; j <= 16; j++)
			{
				num2 = num2 + array[j - 1] << 1;
				array3[j] = num2;
			}
			UInt32[] array4 = new UInt32[288];
			for (Int32 k = 0; k < (Int32)this.codeLengthArray.Length; k++)
			{
				Int32 num3 = (Int32)this.codeLengthArray[k];
				if (num3 > 0)
				{
					array4[k] = FastEncoderStatics.BitReverse(array3[num3], num3);
					array3[num3] += 1u;
				}
			}
			return array4;
		}

		private void CreateTable()
		{
			UInt32[] array = this.CalculateHuffmanCode();
			this.table = new Int16[1 << this.tableBits];
			this.left = new Int16[2 * (Int32)this.codeLengthArray.Length];
			this.right = new Int16[2 * (Int32)this.codeLengthArray.Length];
			Int16 num = (Int16)this.codeLengthArray.Length;
			for (Int32 i = 0; i < (Int32)this.codeLengthArray.Length; i++)
			{
				Int32 num2 = (Int32)this.codeLengthArray[i];
				if (num2 > 0)
				{
					Int32 num3 = (Int32)array[i];
					if (num2 > this.tableBits)
					{
						Int32 num4 = num2 - this.tableBits;
						Int32 num5 = 1 << this.tableBits;
						Int32 num6 = num3 & (1 << this.tableBits) - 1;
						Int16[] array2 = this.table;
						do
						{
							Int16 num7 = array2[num6];
							if (num7 == 0)
							{
								array2[num6] = (Int16)(-num);
								num7 = (Int16)(-num);
								num = (Int16)(num + 1);
							}
							if (num7 > 0)
							{
								goto Block_6;
							}
							if ((num3 & num5) == 0)
							{
								array2 = this.left;
							}
							else
							{
								array2 = this.right;
							}
							num6 = (Int32)(-(Int32)num7);
							num5 <<= 1;
							num4--;
						}
						while (num4 != 0);
						array2[num6] = (Int16)i;
						goto IL_17E;
					Block_6:
						throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
					}
					Int32 num8 = 1 << num2;
					if (num3 >= num8)
					{
						throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
					}
					Int32 num9 = 1 << this.tableBits - num2;
					for (Int32 j = 0; j < num9; j++)
					{
						this.table[num3] = (Int16)i;
						num3 += num8;
					}
				}
			IL_17E:;
			}
		}

		public Int32 GetNextSymbol(InputBuffer input)
		{
			UInt32 num = input.TryLoad16Bits();
			if (input.AvailableBits == 0)
			{
				return -1;
			}
			Int32 num2 = (Int32)this.table[(Int32)(checked((IntPtr)(unchecked((UInt64)num & (UInt64)((Int64)this.tableMask)))))];
			if (num2 < 0)
			{
				UInt32 num3 = 1u << this.tableBits;
				do
				{
					num2 = -num2;
					if ((num & num3) == 0u)
					{
						num2 = (Int32)this.left[num2];
					}
					else
					{
						num2 = (Int32)this.right[num2];
					}
					num3 <<= 1;
				}
				while (num2 < 0);
			}
			Int32 num4 = (Int32)this.codeLengthArray[num2];
			if (num4 <= 0)
			{
				throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
			}
			if (num4 > input.AvailableBits)
			{
				return -1;
			}
			input.SkipBits(num4);
			return num2;
		}

		internal const Int32 MaxLiteralTreeElements = 288;

		internal const Int32 MaxDistTreeElements = 32;

		internal const Int32 EndOfBlockCode = 256;

		internal const Int32 NumberOfCodeLengthTreeElements = 19;

		private Int32 tableBits;

		private Int16[] table;

		private Int16[] left;

		private Int16[] right;

		private Byte[] codeLengthArray;

		private Int32 tableMask;

		private static HuffmanTree staticLiteralLengthTree = new HuffmanTree(HuffmanTree.GetStaticLiteralTreeLength());

		private static HuffmanTree staticDistanceTree = new HuffmanTree(HuffmanTree.GetStaticDistanceTreeLength());
	}
}
