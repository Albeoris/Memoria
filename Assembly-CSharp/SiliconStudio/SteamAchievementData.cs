using System;
using System.Runtime.InteropServices;

namespace SiliconStudio
{
	public struct SteamAchievementData
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
		public String id;

		public Int32 completed;
	}
}
