using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

internal class SoundLoaderStreamingAssets : ISoundLoader
{
    public SoundLoaderStreamingAssets()
    {
        this.Initial();
    }

    public override void Initial()
    {
    }

    public override void Load(SoundProfile profile, ISoundLoader.ResultCallback callback, SoundDatabase soundDatabase)
    {
        String text = AssetManagerUtil.GetStreamingAssetsPath() + "/Sounds/" + profile.ResourceID;
        SoundLib.Log("Load: " + text);
        FileInfo fileInfo = new FileInfo(text);
        Byte[] array = null;
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            array = File.ReadAllBytes(text);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
        }
        if (array != null)
        {
            IntPtr intPtr = Marshal.AllocHGlobal((Int32)fileInfo.Length);
            Marshal.Copy(array, 0, intPtr, (Int32)fileInfo.Length);
            Int32 bankID = ISdLibAPIProxy.Instance.SdSoundSystem_AddData(intPtr, profile);
            profile.AkbBin = intPtr;
            profile.BankID = bankID;
        }
        else
        {
            SoundLib.Log("akbBytes is null");
        }
        callback(profile, soundDatabase);
    }
}
