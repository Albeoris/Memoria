using System;

namespace FF9
{
	public class ccommon
	{
		public static Int32 max(Int32 a, Int32 b)
		{
			return (Int32)((a <= b) ? b : a);
		}

		public static Int32 min(Int32 a, Int32 b)
		{
			return (Int32)((a >= b) ? b : a);
		}
	}
}
