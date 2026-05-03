using System;

public static class BitUtil
{
    public static UInt32 ReadBits(UInt32 input, ref Byte startBit, Byte numBits)
    {
        UInt32 mask = (UInt32)Math.Pow(2, numBits) - 1u;
        UInt32 result = input >> startBit & mask;
        startBit = (Byte)(startBit + numBits);
        return result;
    }

    public static Byte InvertFlag(Byte value, Byte flag)
    {
        return (value & flag) == flag
            ? (Byte)(value & ~flag)
            : (Byte)(value | flag);
    }
}
