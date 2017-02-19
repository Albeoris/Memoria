using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class SoundLoaderResources : ISoundLoader
{
	public override void Initial()
	{
	}

	public override void Load(SoundProfile profile, ISoundLoader.ResultCallback callback, SoundDatabase soundDatabase)
	{
		String text = "Sounds/" + profile.ResourceID + ".akb";
		TextAsset textAsset = AssetManager.Load<TextAsset>(text, false);
		SoundLib.Log("Load: " + text);
		if ((UnityEngine.Object)null == textAsset)
		{
			SoundLib.LogError("File not found AT path: " + text);
			callback((SoundProfile)null, (SoundDatabase)null);
			return;
		}
		Byte[] bytes = textAsset.bytes;
		IntPtr intPtr = Marshal.AllocHGlobal((Int32)textAsset.bytes.Length);
		Marshal.Copy(bytes, 0, intPtr, (Int32)textAsset.bytes.Length);
		Int32 bankID = ISdLibAPIProxy.Instance.SdSoundSystem_AddData(intPtr);
		profile.AkbBin = intPtr;
		profile.BankID = bankID;
		callback(profile, soundDatabase);
	}
}
