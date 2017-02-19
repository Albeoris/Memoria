using System;
using UnityEngine;

namespace Assets.Sources.Graphics.Movie
{
	internal static class MobileMovieManager
	{
		public static void NativeUpdate()
		{
			if (!MobileMovieManager.hasInitialized)
			{
				return;
			}
			GL.IssuePluginEvent(7);
		}

		public static void NativeGraphicsInitialize()
		{
			MobileMovieManager.hasInitialized = true;
		}

		private static Boolean hasInitialized;
	}
}
