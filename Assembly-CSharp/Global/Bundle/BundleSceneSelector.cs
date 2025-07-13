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
        String[] files = Directory.GetFiles("./", "*steam_appid.txt", SearchOption.AllDirectories);
        String[] array = files;
        for (Int32 i = 0; i < (Int32)array.Length; i++)
        {
            String fileName = array[i];
            FileInfo fileInfo = new FileInfo(fileName);
            if (fileInfo.IsReadOnly)
            {
                fileInfo.IsReadOnly = false;
            }
            fileInfo.Delete();
        }

        if (SteamSdkWrapper.SteamAPIRestartAppIfNecessary())
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
