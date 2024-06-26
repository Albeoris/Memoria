﻿using System;
using Assets.Scripts.Common;

public static class GameInitializer
{
	public static void Initial()
	{
		SceneDirector.GetDefaultFadeInTransition();
		OSDLogger.SetBundleVersion(BundleScene.BundleVersion);
	}
}
