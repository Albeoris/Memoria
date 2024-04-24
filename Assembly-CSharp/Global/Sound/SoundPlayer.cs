using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SimpleJSON;

public abstract class SoundPlayer
{
	public SoundPlayer()
	{
		this.resourceLoadingCounter = 0;
	}

	public abstract void Update();

	public Boolean InitializePlugin()
	{
		SoundLib.Log("InitializePlugin()");
		if (this.m_isInitialized)
		{
			return true;
		}
		Int32 num = ISdLibAPIProxy.Instance.SdSoundSystem_Create(String.Empty);
		if (num < 0)
		{
			return false;
		}
		this.m_isInitialized = true;
		return true;
	}

	public void FinalizePlugin()
	{
		SoundLib.Log("FinalizePlugin()");
		if (!this.m_isInitialized)
		{
			return;
		}
		ISdLibAPIProxy.Instance.SdSoundSystem_Release();
		this.m_isInitialized = false;
	}

	public void CreateSound(SoundProfile soundProfile) => StaticCreateSound(soundProfile);
	public void StartSound(SoundProfile soundProfile, Single playerVolume = 1f) => StaticStartSound(soundProfile, playerVolume);
	public void PauseSound(SoundProfile soundProfile) => StaticPauseSound(soundProfile);
	public void ResumeSound(SoundProfile soundProfile) => StaticResumeSound(soundProfile);
	public void StopSound(SoundProfile soundProfile) => StaticStopSound(soundProfile);

	public static void StaticCreateSound(SoundProfile soundProfile)
	{
		Int32 num = ISdLibAPIProxy.Instance.SdSoundSystem_CreateSound(soundProfile.BankID);
		if (num == 0)
		{
			SoundLib.Log("CreateSound failure");
			return;
		}
		soundProfile.SoundID = num;
		SoundLib.Log("CreateSound Success");
	}

	public static void StaticStartSound(SoundProfile soundProfile, Single playerVolume = 1f)
	{
		if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 0)
		{
			SoundLib.Log("failed to play sound");
			soundProfile.SoundID = 0;
			return;
        }
        ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * playerVolume, 0);
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPanning(soundProfile.SoundID, soundProfile.Panning, 0);
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, soundProfile.Pitch, 0);
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);
		SoundLib.Log("StartSound Success");
	}

	public static void StaticPauseSound(SoundProfile soundProfile)
	{
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 1, 0);
		SoundLib.Log("PauseSound Success");
	}

	public static void StaticResumeSound(SoundProfile soundProfile)
	{
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 0, 0);
		SoundLib.Log("ResumeSound Success");
	}

	public static void StaticStopSound(SoundProfile soundProfile)
	{
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, 0);
		SoundLib.Log("StopSound Success");
	}

	protected void LoadResource(SoundProfile soundProfile, SoundDatabase soundDatabase, SoundPlayer.LoadResourceCallback callback)
	{
		if (this.resourceLoadingCounter > 0)
		{
			callback((SoundDatabase)null, true);
			return;
		}
		this.resourceLoadingCounter = 1;
		this.activeCallback = callback;
		SoundLoaderProxy.Instance.Load(soundProfile, new ISoundLoader.ResultCallback(this.RegisterBankAsCallback), soundDatabase);
	}

	protected void LoadResource(String metaData, SoundDatabase soundDatabase, SoundPlayer.LoadResourceCallback callback)
	{
		if (this.resourceLoadingCounter > 0)
		{
			callback((SoundDatabase)null, true);
			return;
		}
		this.resourceLoadingCounter = 0;
		this.activeCallback = callback;
		JSONNode jsonnode = JSONNode.Parse(metaData);
		JSONArray asArray = jsonnode["data"].AsArray;
		this.resourceLoadingCounter = asArray.Count;
		for (Int32 i = 0; i < asArray.Count; i++)
		{
			JSONClass asObject = asArray[i].AsObject;
			String text = asObject["name"];
			Int32 asInt = asObject["soundIndex"].AsInt;
			String strA = asObject["type"];
			SoundProfileType soundProfileType;
			if (String.Compare(strA, "SoundEffect") == 0)
			{
				soundProfileType = SoundProfileType.SoundEffect;
			}
			else if (String.Compare(strA, "Music") == 0)
			{
				soundProfileType = SoundProfileType.Music;
			}
			else if (String.Compare(strA, "MovieAudio") == 0)
			{
				soundProfileType = SoundProfileType.MovieAudio;
			}
			else if (String.Compare(strA, "Song") == 0)
			{
				soundProfileType = SoundProfileType.Song;
			}
			else
			{
				soundProfileType = SoundProfileType.Default;
			}
			SoundProfile soundProfile = new SoundProfile();
			soundProfile.Code = asInt.ToString();
			soundProfile.Name = text;
			soundProfile.SoundIndex = asInt;
			soundProfile.ResourceID = text;
			soundProfile.SoundProfileType = soundProfileType;
			SoundLoaderProxy.Instance.Load(soundProfile, new ISoundLoader.ResultCallback(this.RegisterBankAsCallback), soundDatabase);
		}
	}

	private void RegisterBankAsCallback(SoundProfile profile, SoundDatabase soundDatabase)
	{
		if (profile != null && soundDatabase != null)
		{
			soundDatabase.Create(profile);
			this.resourceLoadingCounter--;
			if (this.resourceLoadingCounter == 0)
			{
				this.activeCallback(soundDatabase, false);
			}
			else
			{
				SoundLib.Log("resourceLoadingCounter > 0! resourceLoadingCounter: " + this.resourceLoadingCounter);
				this.activeCallback(soundDatabase, true);
			}
		}
		else
		{
			SoundLib.Log("either profile OR soundDatabase is null");
			this.resourceLoadingCounter--;
			if (this.resourceLoadingCounter == 0)
			{
				this.activeCallback(soundDatabase, true);
			}
			else
			{
				SoundLib.Log("resourceLoadingCounter > 0! resourceLoadingCounter: " + this.resourceLoadingCounter);
				this.activeCallback(soundDatabase, true);
			}
		}
	}

	protected void UnloadResource(SoundDatabase soundDatabase)
	{
		Dictionary<Int32, SoundProfile> dictionary = soundDatabase.ReadAll();
		foreach (KeyValuePair<Int32, SoundProfile> keyValuePair in dictionary)
		{
			SoundPlayer.StaticUnregisterBank(keyValuePair.Value);
		}
		soundDatabase.DeleteAll();
	}

	protected void UnloadResource(SoundProfile soundProfile, SoundDatabase soundDatabase)
	{
		SoundPlayer.StaticUnregisterBank(soundProfile);
		soundDatabase.Delete(soundProfile);
	}

	public static void StaticUnregisterBank(SoundProfile soundProfile)
	{
		SoundLib.Log("UnregisterBank: " + soundProfile.Name);
		if (soundProfile.BankID != 0)
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_RemoveData(soundProfile.BankID);
			soundProfile.BankID = 0;
		}
		if (IntPtr.Zero != soundProfile.AkbBin)
		{
			Marshal.FreeHGlobal(soundProfile.AkbBin);
			soundProfile.AkbBin = IntPtr.Zero;
		}
	}

	public abstract Single Volume { get; }

	private Boolean m_isInitialized;

	private Int32 resourceLoadingCounter;

	private SoundPlayer.LoadResourceCallback activeCallback;

	protected delegate void LoadResourceCallback(SoundDatabase soundDatabase, Boolean isError);
}
