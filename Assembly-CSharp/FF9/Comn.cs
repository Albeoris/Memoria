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

		public const Int32 ONE = 4096;

		public const Byte TRUE = 1;

		public const Byte FALSE = 0;

		public const Byte ON = 1;

		public const Byte OFF = 0;

		public const Byte YES = 1;

		public const Byte NO = 0;
	}
}
