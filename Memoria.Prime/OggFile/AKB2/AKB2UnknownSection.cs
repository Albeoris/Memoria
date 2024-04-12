using System;

namespace Memoria.Prime.AKB2
{
	public struct AKB2UnknownSection
	{
		public UInt32 Constant01;
		public UInt32 Zero02;
		public UInt32 Zero03;
		public UInt32 Zero04;
		public UInt32 Zero05;
		public UInt32 Zero06;

		public static unsafe void Initialize(AKB2UnknownSection* section)
		{
			section->Constant01 = 0x00004040;
		}
	}
}
