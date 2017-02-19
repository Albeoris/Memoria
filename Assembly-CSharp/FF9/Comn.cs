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

		public const Int32 ONE = 4096;

		public const Byte TRUE = 1;

		public const Byte FALSE = 0;

		public const Byte ON = 1;

		public const Byte OFF = 0;

		public const Byte YES = 1;

		public const Byte NO = 0;
	}
}
