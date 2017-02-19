using System;

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
    }
}