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

		public const UInt16 FF9_ATTR_BATTLE_LOADBBG = 1;

		public const UInt16 FF9_ATTR_BATTLE_LOADNPC = 2;

		public const UInt16 FF9_ATTR_BATTLE_LOADCHR = 4;

		public const UInt16 FF9_ATTR_BATTLE_INITBMENU = 16;

		public const UInt16 FF9_ATTR_BATTLE_CDSHELLOPEN = 32;

		public const UInt16 FF9_ATTR_BATTLE_INITHINT = 64;

		public const UInt16 FF9_ATTR_BATTLE_NOPUTDISPENV = 256;

		public const UInt16 FF9_ATTR_BATTLE_NOPUTDRAWENV = 512;

		public const UInt16 FF9_ATTR_BATTLE_EXITBATTLE = 4096;
	}
}
