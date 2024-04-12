using System;
using System.IO;

namespace Memoria.Prime
{
	public static class MathEx
	{
		public static Int32 BitCount(Int32 value)
		{
			value = value - ((value >> 1) & 0x55555555);
			value = (value & 0x33333333) + ((value >> 2) & 0x33333333);
			return (((value + (value >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
		}

		public static void WriteToFile(String outputPath, Int32[,,,] map)
		{
			Int32 width = map.GetUpperBound(0);
			Int32 height = map.GetUpperBound(1);
			Int32 depth = map.GetUpperBound(2);

			using (FileStream output = File.Create(outputPath))
			using (BinaryWriter bw = new BinaryWriter(output))
			{
				bw.Write(width);
				bw.Write(height);
				bw.Write(depth);

				using (UnsafeTypeCache<byte>.ChangeArrayTypes(map, sizeof(Byte)))
				{
					Byte[] byteArray = (byte[])(object)map;
					output.Write(byteArray, 0, byteArray.Length);
				}
			}
		}
	}
}
