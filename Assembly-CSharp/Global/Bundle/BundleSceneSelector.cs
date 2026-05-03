using Memoria;
using System;
using System.IO;
using UnityEngine;

public class BundleSceneSelector : MonoBehaviour
{
    private void Awake()
    {
        WindowManager.AlignWindow();
        global::Debug.Log("10 BundleSceneSelector.Awake");
        Boolean flag = false;

        if (SteamSdkWrapper.SteamAPIRestartAppIfNecessary(377840))
        {
            Application.Quit();
            return;
        }

        SteamSdkWrapper.TryInitialize();

        if (Application.platform == RuntimePlatform.IPhonePlayer || flag)
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
