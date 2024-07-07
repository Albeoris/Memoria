using System;
using UnityEngine;

namespace FF9
{
    public class Comn
    {
        public static Int32 random8()
        {
            return UnityEngine.Random.Range(0, 256);
        }

        public static Int32 random16()
        {
            return UnityEngine.Random.Range(0, 65536);
        }

        public static UInt32 randomID(UInt32 id)
        {
            // Same as "EventEngine.OperatorSelect": pick a random bit in "id" seen as a bit list
            Byte[] numArray = new Byte[32];
            Int32 count = 0;
            Int32 bit = 0;
            while (bit < 32)
            {
                if ((id & 1) != 0)
                    numArray[count++] = (Byte)bit;
                ++bit;
                id >>= 1;
            }
            if (count == 0)
                return 0;
            Int32 index = count * Comn.random8() >> 8;
            return 1u << numArray[index];
        }

        /// <summary>
        /// Counts the number of set bits (bits with a value of 1) in an unsigned 64-bit integer.
        /// </summary>
        /// <param name="bitList">The unsigned 64-bit integer to count the set bits in.</param>
        /// <returns>The count of set bits in the input integer.</returns>
        public static Byte countBits(UInt64 bitList)
        {
            Byte count = 0;
            while (bitList != 0)
            {
                count++;
                bitList &= bitList - 1;
            }
            return count;
        }

        public static UInt64 firstBitSet(UInt64 bitList)
        {
            UInt64 bit = 1;
            while (bit != 0)
            {
                if ((bitList & bit) != 0)
                    return bit;
                bit <<= 1;
            }
            throw new Exception("[Comn] Trying to find firstBitSet of 0");
        }

        public static Byte firstBitSetIndex(UInt64 bitList)
        {
            Byte pos = 0;
            UInt64 bit = 1;
            while (bit != 0)
            {
                if ((bitList & bit) != 0)
                    return pos;
                bit <<= 1;
                ++pos;
            }
            throw new Exception("[Comn] Trying to find firstBitSetIndex of 0");
        }
    }
}
