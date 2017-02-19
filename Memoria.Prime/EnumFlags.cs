using System;
using System.Collections.Generic;

namespace Memoria.Prime
{
    public static class EnumFlags<T>
    {
        public static readonly String[] Names;
        public static readonly T[] Values;
        public static readonly UInt64[] Integers;

        static EnumFlags()
        {
            List<String> names = new List<String>(EnumCache<T>.Names.Length);
            List<T> values = new List<T>(EnumCache<T>.Values.Length);
            List<UInt64> integers = new List<UInt64>(EnumCache<T>.Integers.Length);

            for (Int32 index = 0; index < EnumCache<T>.Integers.Length; index++)
            {
                UInt64 integer = EnumCache<T>.Integers[index];
                if (integer == 0)
                    continue;

                if (NumberOfSetBits(integer) > 1)
                    continue;

                names.Add(EnumCache<T>.Names[index]);
                values.Add(EnumCache<T>.Values[index]);
                integers.Add(integer);
            }

            Names = names.ToArray();
            Values = values.ToArray();
            Integers = integers.ToArray();
        }

        private static Int32 NumberOfSetBits(UInt64 bits)
        {
            bits = bits - ((bits >> 1) & 0x5555555555555555UL);
            bits = (bits & 0x3333333333333333UL) + ((bits >> 2) & 0x3333333333333333UL);
            return (Int32)(unchecked(((bits + (bits >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }
    }
}