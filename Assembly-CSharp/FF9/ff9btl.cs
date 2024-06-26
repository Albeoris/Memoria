using System;

namespace FF9
{
	public class ff9btl
	{
		public static Int32 ff9btl_set_bone(Byte start, Byte end)
		{
			return (Int32)(end & Byte.MaxValue) << 8 | (Int32)(start & Byte.MaxValue);
		}

		public static Int32 ff9btl_get_bonestart(Int32 bone)
		{
			return bone & 255;
		}

		public static Int32 ff9btl_get_boneend(Int32 bone)
		{
			return bone >> 8 & 255;
		}

		public const Byte LOAD_BBG = 1;
		public const Byte LOAD_NPC = 2;
		public const Byte LOAD_CHR = 4;
		public const Byte LOAD_INITNPC = 8;
		public const Byte LOAD_INITCHR = 16;
		public const Byte LOAD_FADENPC = 32;
		public const Byte LOAD_FADECHR = 64;
		public const Byte LOAD_ALL = LOAD_BBG | LOAD_NPC | LOAD_CHR;

		[Flags]
		public enum ATTR : UInt16
		{
			LOADBBG = 1,
			LOADNPC = 2,
			LOADCHR = 4,
			INITBMENU = 16,
			CDSHELLOPEN = 32,
			INITHINT = 64,
			NOPUTDISPENV = 256,
			NOPUTDRAWENV = 512,
			EXITBATTLE = 4096
		}
	}
}
