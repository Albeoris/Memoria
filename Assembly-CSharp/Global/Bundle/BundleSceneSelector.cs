using System;
using UnityEngine;
using Memoria;
using Memoria.Prime;

public class BundleSceneSelector : MonoBehaviour
{
    private void Awake()
    {
        // Allow running the game directly from the game's executable instead of Steam -> Launcher -> FF9.exe
        String envDir = Environment.CurrentDirectory;
        if (envDir.EndsWith("\\x86") || envDir.EndsWith("\\x64"))
            Environment.CurrentDirectory = envDir.Substring(0, envDir.Length - 4);

        WindowManager.AlignWindow();
        global::Debug.Log("10 BundleSceneSelector.Awake");

        try
        {
            if (SteamSdkWrapper.SteamAPIRestartAppIfNecessary(SteamSdkWrapper.ApplicationSteamId))
            {
                Application.Quit();
                return;
            }
        }
        catch (EntryPointNotFoundException)
        {
            Log.Warning($"[{nameof(BundleSceneSelector)}] {SteamSdkWrapper.LibraryName}.dll has a missing entry point; maybe try running the game in mode x64");
        }

        SteamSdkWrapper.TryInitialize();

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (BundleSceneIOS.Are3CompressedBundlesCached())
            {
                global::Debug.Log("All 3 compressed bundles are cachaed. Go to SplashScreen");
                Application.LoadLevel("SplashScreen");
            }
            else
            {
                global::Debug.Log("Some compressed bundles are NOT cached. Go to Bundle-iOS");
                Application.LoadLevel("Bundle-iOS");
            }
        }
        else
        {
            Application.LoadLevel("SplashScreen");
        }
    }
}
