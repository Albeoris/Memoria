using System;
using System.Collections.Generic;
using System.IO;
using Memoria;
using UnityEngine;

public class SfxSoundPlayer : SoundPlayer
{
	public void SetVolume(Int32 volume)
	{
		this.playerVolume = volume / 100f;
		this.UpdatePlayingSoundVolume();
	}

	private void UpdatePlayingSoundVolume()
	{
		foreach (Int32 key in this.playingDict.Keys)
		{
			SoundProfile soundProfile = this.playingDict[key];
			if (this.residentSoundDatabase.Read(soundProfile.SoundIndex) != null)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.playerVolume, 0);
			}
		}
	}

	public Int32 GetResidentSoundCount()
	{
		return this.residentSoundDatabase.ReadAll().Count;
	}

	public void LoadAllResidentSoundData()
	{
		List<String> list = SoundMetaData.ResidentSfxSoundIndex[0];
		for (Int32 i = 0; i < list.Count; i++)
		{
			Int32 soundIndex = i;
			String text = list[i];
			String fileName = Path.GetFileName(text);
			SoundProfile soundProfile = new SoundProfile();
			soundProfile.Code = soundIndex.ToString();
			soundProfile.Name = fileName;
			soundProfile.SoundIndex = soundIndex;
			soundProfile.ResourceID = text;
			soundProfile.SoundProfileType = SoundProfileType.Sfx;
			this.residentSoundDatabase.Create(soundProfile);
			base.LoadResource(soundProfile, this.residentSoundDatabase, new SoundPlayer.LoadResourceCallback(this.LoadSoundResourceCallback));
		}
	}

	public void UnloadAllResidentSoundData()
	{
		this.UnloadAllSoundData(this.residentSoundDatabase);
	}

	public void LoadSoundData(Int32 specialEffectID)
	{
		this.CurrentSpecialEffectID = specialEffectID;
		this.UnloadAllSoundData(this.soundDatabase);
		List<String> list = SoundMetaData.SfxSoundIndex[specialEffectID];
		for (Int32 i = 0; i < list.Count; i++)
		{
			Int32 soundIndex = this.GetSoundIndex(specialEffectID, i, this.GetResidentSoundCount());
			String text = list[i];
			String fileName = Path.GetFileName(text);
			SoundProfile soundProfile = new SoundProfile();
			soundProfile.Code = soundIndex.ToString();
			soundProfile.Name = fileName;
			soundProfile.SoundIndex = soundIndex;
			soundProfile.ResourceID = text;
			soundProfile.SoundProfileType = SoundProfileType.Sfx;
			this.soundDatabase.Create(soundProfile);
			this.loadingSoundProfile = soundProfile;
			base.LoadResource(soundProfile, this.soundDatabase, new SoundPlayer.LoadResourceCallback(this.LoadSoundResourceCallback));
			this.loadingSoundProfile = (SoundProfile)null;
		}
	}

    public SoundProfile PlaySfxSound(Int32 soundIndexInSpecialEffect, Single soundVolume = 1f, Single panning = 0f, Single pitch = 1f)
    {
        SoundProfile soundProfile;
        if (soundIndexInSpecialEffect < this.GetResidentSoundCount())
        {
            soundProfile = this.residentSoundDatabase.Read(soundIndexInSpecialEffect);
        }
        else
        {
            soundProfile = this.soundDatabase.Read(this.GetSoundIndex(this.CurrentSpecialEffectID, soundIndexInSpecialEffect, 0));
        }
        if (soundProfile == null)
        {
            SoundLib.LogError("PlaySfxSound, soundProfile is null");
            return (SoundProfile)null;
        }
        if (this.lastSfxSoundIndex == soundProfile.SoundIndex)
        {
            if (this.playingAtFrameCount == Time.frameCount)
            {
                this.playingSameSfxCount++;
                if (this.playingSameSfxCount >= 3)
                {
                    return (SoundProfile)null;
                }
            }
            else
            {
                this.playingSameSfxCount = 1;
            }
        }
        else
        {
            this.playingSameSfxCount = 1;
        }
        this.lastSfxSoundIndex = soundProfile.SoundIndex;
        this.playingAtFrameCount = Time.frameCount;
        Int32 limitNumber = (Int32)((!FF9StateSystem.Settings.IsFastForward) ? 3 : 2);
        this.LimitPlayingSfx(soundProfile.SoundIndex, limitNumber);
        soundProfile.SoundVolume = soundVolume;
        soundProfile.Panning = panning;
        soundProfile.Pitch = pitch;
        soundProfile.StartPlayTime = Time.time;
        base.CreateSound(soundProfile);
        ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);
        if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) != 0)
        {
            ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.playerVolume, 0);
            SoundLib.Log("Panning: " + soundProfile.Panning);
            ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPanning(soundProfile.SoundID, soundProfile.Panning, 0);
            Int32 fastForwardFactor = HonoBehaviorSystem.Instance.GetFastForwardFactor();
            if (fastForwardFactor != 1)
            {
                Single pitch2 = (Single)fastForwardFactor * soundProfile.Pitch;
                soundProfile.Pitch = pitch2;
            }
            ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, soundProfile.Pitch, 0);
        }
        else
        {
            SoundLib.Log("failed to play sound");
            soundProfile.SoundID = 0;
        }
        if (!this.playingDict.ContainsKey(soundProfile.SoundID))
        {
            this.playingDict.Add(soundProfile.SoundID, soundProfile);
        }
        return soundProfile;
    }


    private void LimitPlayingSfx(Int32 soundIndex, Int32 limitNumber)
    {
        SoundProfile soundProfile = (SoundProfile)null;
        Single num = Single.MaxValue;
        Int32 num2 = 0;
        foreach (KeyValuePair<Int32, SoundProfile> keyValuePair in this.playingDict)
        {
            SoundProfile value = keyValuePair.Value;
            if (value.SoundIndex == soundIndex)
            {
                num2++;
                if (value.StartPlayTime < num)
                {
                    soundProfile = value;
                    num = value.StartPlayTime;
                }
            }
        }
        if (num2 >= limitNumber)
        {
            ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, 0);
            this.playingDict.Remove(soundProfile.SoundID);
        }
    }

    public Boolean IsPlaying(Int32 soundIndexInSpecialEffect)
	{
		SoundProfile soundProfile;
		if (soundIndexInSpecialEffect < this.GetResidentSoundCount())
		{
			soundProfile = this.residentSoundDatabase.Read(soundIndexInSpecialEffect);
		}
		else
		{
			soundProfile = this.soundDatabase.Read(this.GetSoundIndex(this.CurrentSpecialEffectID, soundIndexInSpecialEffect, 0));
		}
		if (soundProfile == null)
		{
			SoundLib.LogError("PlaySfxSound, soundProfile is null");
			return false;
		}
		return ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 1;
	}

	public void StopSound(Int32 soundIndexInSpecialEffect)
	{
		SoundProfile soundProfile;
		if (soundIndexInSpecialEffect < this.GetResidentSoundCount())
		{
			soundProfile = this.residentSoundDatabase.Read(soundIndexInSpecialEffect);
		}
		else
		{
			soundProfile = this.soundDatabase.Read(this.GetSoundIndex(this.CurrentSpecialEffectID, soundIndexInSpecialEffect, 0));
		}
		if (soundProfile == null)
		{
			SoundLib.LogError("StopSfxSound, soundProfile is null");
			return;
		}
		if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 1)
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, 0);
		}
	}

	public void StopAllSounds()
	{
		Dictionary<Int32, SoundProfile> dictionary = new Dictionary<Int32, SoundProfile>();
		Dictionary<Int32, SoundProfile> dictionary2 = this.residentSoundDatabase.ReadAll();
		Dictionary<Int32, SoundProfile> dictionary3 = this.soundDatabase.ReadAll();
		foreach (Int32 key in dictionary2.Keys)
		{
			dictionary.Add(key, dictionary2[key]);
		}
		foreach (Int32 key2 in dictionary3.Keys)
		{
			dictionary.Add(key2, dictionary3[key2]);
		}
		foreach (Int32 key3 in dictionary.Keys)
		{
			SoundProfile soundProfile = dictionary[key3];
			if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 1)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, 0);
			}
		}
	}

	public void PauseAllSounds()
	{
		Dictionary<Int32, SoundProfile> dictionary = new Dictionary<Int32, SoundProfile>();
		Dictionary<Int32, SoundProfile> dictionary2 = this.residentSoundDatabase.ReadAll();
		Dictionary<Int32, SoundProfile> dictionary3 = this.soundDatabase.ReadAll();
		foreach (Int32 key in dictionary2.Keys)
		{
			dictionary.Add(key, dictionary2[key]);
		}
		foreach (Int32 key2 in dictionary3.Keys)
		{
			dictionary.Add(key2, dictionary3[key2]);
		}
		foreach (Int32 key3 in dictionary.Keys)
		{
			SoundProfile soundProfile = dictionary[key3];
			if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 1)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 1, 0);
			}
		}
	}

	public void ResumeAllSounds()
	{
		Dictionary<Int32, SoundProfile> dictionary = new Dictionary<Int32, SoundProfile>();
		Dictionary<Int32, SoundProfile> dictionary2 = this.residentSoundDatabase.ReadAll();
		Dictionary<Int32, SoundProfile> dictionary3 = this.soundDatabase.ReadAll();
		foreach (Int32 key in dictionary2.Keys)
		{
			dictionary.Add(key, dictionary2[key]);
		}
		foreach (Int32 key2 in dictionary3.Keys)
		{
			dictionary.Add(key2, dictionary3[key2]);
		}
		foreach (Int32 key3 in dictionary.Keys)
		{
			SoundProfile soundProfile = dictionary[key3];
			if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 1)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 0, 0);
			}
		}
	}

	public override void Update()
	{
		foreach (Int32 num in this.playingDict.Keys)
		{
			if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(num) == 0)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(num, 0);
				this.playingRemoveList.Add(num);
				SoundLib.Log("Sound End, Stop success");
			}
		}
		foreach (Int32 key in this.playingRemoveList)
		{
			if (!this.playingDict.Remove(key))
			{
				SoundLib.Log("Remove playingSet failure!");
			}
		}
		this.playingRemoveList.Clear();
	}

	private void LoadSoundResourceCallback(SoundDatabase soundDatabase, Boolean isError)
	{
		if (!isError)
		{
			SoundLib.Log("LoadSoundResource is success");
		}
		else
		{
			SoundLib.LogError("LoadSoundResource has Error");
			this.soundDatabase.Delete(this.loadingSoundProfile);
		}
	}

	private void UnloadAllSoundData(SoundDatabase soundDatabase)
	{
		Dictionary<Int32, SoundProfile> dictionary = soundDatabase.ReadAll();
		Dictionary<Int32, SoundProfile> dictionary2 = new Dictionary<Int32, SoundProfile>();
		foreach (Int32 key in dictionary.Keys)
		{
			dictionary2.Add(key, dictionary[key]);
		}
		foreach (Int32 key2 in dictionary2.Keys)
		{
			SoundProfile soundProfile = dictionary2[key2];
			if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 0)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundProfile.SoundID, 0);
			}
			base.UnloadResource(soundProfile, soundDatabase);
		}
		if (soundDatabase.ReadAll().Count != 0)
		{
			SoundLib.LogError("soundDatabase: Count: " + soundDatabase.ReadAll().Count + " It should be 0");
		}
	}

	private Int32 GetSoundIndex(Int32 specialEffectID, Int32 soundIndexInSpecialEffect, Int32 residentSpecialEffectOffset = 0)
	{
		return specialEffectID * 100 + soundIndexInSpecialEffect + residentSpecialEffectOffset;
	}

	private SoundDatabase residentSoundDatabase = new SoundDatabase();

	private SoundDatabase soundDatabase = new SoundDatabase();

	public Int32 CurrentSpecialEffectID;

	private Dictionary<Int32, SoundProfile> playingDict = new Dictionary<Int32, SoundProfile>();

	private List<Int32> playingRemoveList = new List<Int32>();

    private Int32 lastSfxSoundIndex = -1;

    private Int32 playingSameSfxCount;

    private Int32 playingAtFrameCount;

    private Single playerVolume = 1f;

	private SoundProfile loadingSoundProfile;
}
