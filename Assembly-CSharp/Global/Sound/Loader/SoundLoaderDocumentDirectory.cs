using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

internal class SoundLoaderDocumentDirectory : ISoundLoader
{
	public SoundLoaderDocumentDirectory()
	{
		this.Initial();
	}

	public override void Initial()
	{
		String path = Application.persistentDataPath + "/SoundEffect";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		String path2 = Application.persistentDataPath + "/Music";
		if (!Directory.Exists(path2))
		{
			Directory.CreateDirectory(path2);
		}
	}

	public override void Load(SoundProfile profile, ISoundLoader.ResultCallback callback, SoundDatabase soundDatabase)
	{
		String text = String.Empty;
		if (profile.SoundProfileType == SoundProfileType.SoundEffect)
		{
			text = Application.persistentDataPath + "/SoundEffect/" + profile.ResourceID;
		}
		else if (profile.SoundProfileType == SoundProfileType.Music)
		{
			text = Application.persistentDataPath + "/Music/" + profile.ResourceID;
		}
		else
		{
			text = Application.persistentDataPath + "/" + profile.ResourceID;
		}
		FileInfo fileInfo = new FileInfo(text);
		Byte[] source = File.ReadAllBytes(text);
		IntPtr intPtr = Marshal.AllocHGlobal((Int32)fileInfo.Length);
		Marshal.Copy(source, 0, intPtr, (Int32)fileInfo.Length);
		Int32 bankID = ISdLibAPIProxy.Instance.SdSoundSystem_AddData(intPtr, profile);
		profile.AkbBin = intPtr;
		profile.BankID = bankID;
		callback(profile, soundDatabase);
	}
}
