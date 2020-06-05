using System;
using UnityEngine;

public class BitUtil
{
	public static UInt32 ReadBits(UInt32 input, ref Byte startBit, Byte numBits)
	{
		UInt32 num = (UInt32) Mathf.Pow(2f, (Single) numBits) - 1u;
		UInt32 result = input >> (Int32) startBit & num;
		startBit = (Byte) (startBit + numBits);
		return result;
	}

	public static Byte InvertFlag(Byte value, Byte flag)
	{
		return (value & flag) == flag
			? (Byte) (value & ~flag)
			: (Byte) (value | flag);
	}
}