using System;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;
using Memoria.Prime.AKB2;

public class SoundLoaderResources : ISoundLoader
{
	public override void Initial()
	{
	}

	public override void Load(SoundProfile profile, ISoundLoader.ResultCallback callback, SoundDatabase soundDatabase)
	{
		String text = "Sounds/" + profile.ResourceID + ".akb";
		String[] akbInfo;
		Byte[] binAsset = AssetManager.LoadBytes(text, out akbInfo, false);
		SoundLib.Log("Load: " + text);
		if (binAsset == null)
		{
			SoundLib.LogError("File not found AT path: " + text);
			callback((SoundProfile)null, (SoundDatabase)null);
			return;
		}
		// Todo: maybe use SoundImporter.ReadAkbDataFromOgg
		// if (((binAsset[0] << 24) | (binAsset[1] << 16) | (binAsset[2] << 8) | binAsset[3]) == 0x4F676753)
		IntPtr intPtr = Marshal.AllocHGlobal((Int32)binAsset.Length);
		Marshal.Copy(binAsset, 0, intPtr, (Int32)binAsset.Length);
		if (akbInfo.Length > 0)
        {
			// Assume that AKB header is always of size 304 (split "intPtr" into AkbBin + OggBin if ever that changes)
			// Maybe use a constant instead of 304 ("SoundImporter.AkbHeaderSize" or something defined at a better place?)
			Byte[] akbBin = new byte[304];
			Marshal.Copy(intPtr, akbBin, 0, 304);
			AKB2Header akbHeader = new AKB2Header();
			akbHeader.ReadFromBytes(akbBin);
			foreach (String s in akbInfo)
            {
				String[] akbCode = s.Split(' ');
				if (akbCode.Length >= 2 && String.Compare(akbCode[0], "LoopStart") == 0)
					System.UInt32.TryParse(akbCode[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out akbHeader.LoopStart);
				else if (akbCode.Length >= 2 && String.Compare(akbCode[0], "LoopEnd") == 0)
					System.UInt32.TryParse(akbCode[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out akbHeader.LoopEnd);
				else if (akbCode.Length >= 2 && String.Compare(akbCode[0], "LoopStart2") == 0)
					System.UInt32.TryParse(akbCode[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out akbHeader.LoopStartAlternate);
				else if (akbCode.Length >= 2 && String.Compare(akbCode[0], "LoopEnd2") == 0)
					System.UInt32.TryParse(akbCode[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out akbHeader.LoopEndAlternate);
				else if (akbCode.Length >= 2 && String.Compare(akbCode[0], "SampleRate") == 0)
					System.UInt16.TryParse(akbCode[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out akbHeader.SampleRate);
			}
			Marshal.Copy(akbHeader.WriteToBytes(), 0, intPtr, 304);
		}
		Int32 bankID = ISdLibAPIProxy.Instance.SdSoundSystem_AddData(intPtr);
		profile.AkbBin = intPtr;
		profile.BankID = bankID;
		callback(profile, soundDatabase);
	}
}
