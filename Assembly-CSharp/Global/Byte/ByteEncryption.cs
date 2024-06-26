using System;

public class ByteEncryption
{
	public static void Decryption(Byte[] src, ref Byte[] dst)
	{
		Int64 num = 1024L;
		Int64 num2 = (Int64)src.Length;
		Int64 num3 = num2 - num;
		Int64 num4 = 0L;
		Int32 num5 = 0;
		while ((Int64)num5 < num3)
		{
			if (num4 < num)
			{
				dst[num5] = src[(Int32)(checked((IntPtr)(unchecked(num2 - num + num4))))];
				num4 += 1L;
			}
			else if ((Int64)num5 < num2)
			{
				dst[num5] = src[num5];
			}
			num5++;
		}
	}

	public static Byte[] Decryption(Byte[] bytes)
	{
		Int64 num = 1024L;
		Int64 num2 = (Int64)bytes.Length;
		Int64 num3 = num2 - num;
		Byte[] array = new Byte[num3];
		Int64 num4 = 0L;
		Int32 num5 = 0;
		while ((Int64)num5 < num3)
		{
			if (num4 < num)
			{
				array[num5] = bytes[(Int32)(checked((IntPtr)(unchecked(num2 - num + num4))))];
				num4 += 1L;
			}
			else if ((Int64)num5 < num2)
			{
				array[num5] = bytes[num5];
			}
			num5++;
		}
		return array;
	}

	public static Byte[] Encryption(Byte[] bytes)
	{
		Int64 num = 1024L;
		Int64 num2 = (Int64)bytes.Length;
		Int64 num3 = num2 + num;
		Byte[] array = new Byte[num3];
		Int64 num4 = 0L;
		Int64 num5 = 0L;
		Int32 num6 = 0;
		while ((Int64)num6 < num3)
		{
			if (num4 < num)
			{
				array[num6] = bytes[(Int32)(checked((IntPtr)(unchecked(num4 + num))))];
				num4 += 1L;
			}
			else if ((Int64)num6 < num2)
			{
				array[num6] = bytes[num6];
			}
			else
			{
				array[num6] = bytes[(Int32)(checked((IntPtr)num5))];
				num5 += 1L;
			}
			num6++;
		}
		return array;
	}
}
