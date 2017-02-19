using System;
using System.Runtime.InteropServices;
using AOT;

namespace Assets.SiliconSocial
{
	public static class HonoAchievementNotifier
	{
		[DllImport("__Internal")]
		private static extern void _NotifyAchievement(Int32 intID, String strID, Int32 progress, Boolean showsCompletionBanner, HonoAchievementNotifier.AcmCallback callback);

		[MonoPInvokeCallback(typeof(HonoAchievementNotifier.AcmCallback))]
		private static void AchievementCallback(Int32 intID, Int32 progress, Int32 percentProgress, Int32 result)
		{
			AchievementManager.ReportCallback((AcheivementKey)intID, progress, percentProgress, result != 0);
		}

		public static void Notify(Int32 intID, String strID, Int32 progress, Boolean showsCompletionBanner)
		{
			HonoAchievementNotifier._NotifyAchievement(intID, strID, progress, showsCompletionBanner, new HonoAchievementNotifier.AcmCallback(HonoAchievementNotifier.AchievementCallback));
		}

		public delegate void AcmCallback(Int32 intID, Int32 progress, Int32 percentProgress, Int32 result);
	}
}
