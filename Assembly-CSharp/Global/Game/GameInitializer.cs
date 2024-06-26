using Assets.Scripts.Common;
using System;

public static class GameInitializer
{
    public static void Initial()
    {
        SceneDirector.GetDefaultFadeInTransition();
        OSDLogger.SetBundleVersion(BundleScene.BundleVersion);
    }
}
