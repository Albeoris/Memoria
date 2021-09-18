using System;
using Memoria;
using UnityEngine;

public class VoicePlayer : SoundPlayer
{
	public VoicePlayer()
	{
		this.playerVolume = Configuration.Audio.MusicVolume / 100f;
		this.playerPitch = 1f;
		this.playerPanning = 0f;
		this.fadeInDuration = 0f;
		this.fadeOutDuration = 0f;
		this.fadeInTimeRemain = 0f;
		this.stateTransition = new SdLibSoundProfileStateGraph();
		this.stateTransition.Add(new SdLibSoundProfileStateGraph.TransitionDelegate(base.CreateSound), SoundProfileState.Idle, SoundProfileState.Created);
		this.stateTransition.Add(new SdLibSoundProfileStateGraph.TransitionDelegate(base.CreateSound), SoundProfileState.Stopped, SoundProfileState.Created);
		this.stateTransition.Add(new SdLibSoundProfileStateGraph.TransitionDelegate(this.StartSoundCrossfadeIn), SoundProfileState.Created, SoundProfileState.CrossfadeIn);
		this.stateTransition.Add(new SdLibSoundProfileStateGraph.TransitionDelegate(this.CrossFadeInFinish), SoundProfileState.CrossfadeIn, SoundProfileState.Played);
		this.stateTransition.Add(new SdLibSoundProfileStateGraph.TransitionDelegate(base.ResumeSound), SoundProfileState.Paused, SoundProfileState.Played);
		this.stateTransition.Add(new SdLibSoundProfileStateGraph.TransitionDelegate(base.PauseSound), SoundProfileState.Played, SoundProfileState.Paused);
		this.stateTransition.Add(new SdLibSoundProfileStateGraph.TransitionDelegate(base.PauseSound), SoundProfileState.CrossfadeIn, SoundProfileState.Paused);
		this.stateTransition.Add(new SdLibSoundProfileStateGraph.TransitionDelegate(base.StopSound), SoundProfileState.Paused, SoundProfileState.Stopped);
		this.stateTransition.Add(new SdLibSoundProfileStateGraph.TransitionDelegate(base.StopSound), SoundProfileState.Played, SoundProfileState.Stopped);
		this.stateTransition.Add(new SdLibSoundProfileStateGraph.TransitionDelegate(base.StopSound), SoundProfileState.CrossfadeIn, SoundProfileState.Stopped);
	}

	public void LoadMusic(String metaData)
	{
		base.LoadResource(metaData, this.soundDatabase, new SoundPlayer.LoadResourceCallback(this.LoadSoundResourceCallback));
	}

	private void LoadSoundResourceCallback(SoundDatabase soundDatabase, Boolean isError)
	{
		SoundLib.Log("LoadVoiceResourceCallback: " + ((!isError) ? "Success" : "Error"));
	}

	public void UnloadMusic()
	{
		base.UnloadResource(this.soundDatabase);
	}

	public void PlayMusic(Int32 soundIndex)
	{
		this.PlayMusic(soundIndex, 0, SoundProfileType.Music);
	}

	public void PlayMusic(Int32 soundIndex, Int32 fadeIn, SoundProfileType type = SoundProfileType.Music)
	{
		SoundProfile soundProfile = this.soundDatabase.Read(soundIndex);
		if (soundProfile == null)
		{
			soundProfile = this.onTheFlySoundDatabase.Read(soundIndex);
		}
		if (soundProfile != null)
		{
			this.PlayMusic(soundProfile, fadeIn);
		}
		else
		{
			soundProfile = SoundMetaData.GetSoundProfile(soundIndex, type);
			this.onTheFlyLoadedSoundProfile = soundProfile;
			this.onTheFlyLoadedFadeIn = fadeIn;
			if (this.onTheFlySoundDatabase.ReadAll().Count >= 10)
			{
				SoundLib.Log("Unload on the fly voice database.");
				base.UnloadResource(this.onTheFlySoundDatabase);
			}
			base.LoadResource(soundProfile, this.onTheFlySoundDatabase, new SoundPlayer.LoadResourceCallback(this.LoadOnTheFlySoundResourceCallback));
		}
	}

	public void NextLoopRegion(Int32 soundIndex)
	{
		SoundProfile soundProfile = this.soundDatabase.Read(soundIndex);
		if (soundProfile == null)
		{
			soundProfile = this.onTheFlySoundDatabase.Read(soundIndex);
		}
		if (soundProfile != null)
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetNextLoopRegion(soundProfile.SoundID);
		}
		else
		{
			SoundLib.Log("NextLoopRegion(), soundProfile is null!");
		}
	}

	private void LoadOnTheFlySoundResourceCallback(SoundDatabase soundDatabase, Boolean isError)
	{
		if (!isError)
		{
			if (this.onTheFlyLoadedSoundProfile != null)
			{
				this.PlayMusic(this.onTheFlyLoadedSoundProfile, this.onTheFlyLoadedFadeIn);
			}
		}
		else
		{
			SoundLib.Log("LoadOnTheFlySoundResourceCallback is Error");
		}
	}

	public void PlayMusic(SoundProfile soundProfileFromIndex, Int32 fadeIn)
	{
		this.fadeInDuration = (Single)fadeIn / 1000f;
		this.fadeInTimeRemain = this.fadeInDuration;
		if (this.activeSoundProfile == soundProfileFromIndex)
		{
			if (this.activeSoundProfile.SoundProfileState == SoundProfileState.Paused)
			{
				this.stateTransition.Transition(soundProfileFromIndex, new SdLibSoundProfileStateGraph.TransitionDelegate(base.ResumeSound));
				this.SetMusicVolume(this.playerVolume * this.optionVolume, soundProfileFromIndex);
				this.SetMusicPanning(this.playerPanning, soundProfileFromIndex);
				this.SetMusicPitch(this.playerPitch, soundProfileFromIndex);
			}
		}
		else if (this.upcomingSoundProfile == soundProfileFromIndex)
		{
			if (this.activeSoundProfile != null && this.stateTransition.Transition(this.activeSoundProfile, new SdLibSoundProfileStateGraph.TransitionDelegate(base.StopSound)) == 0)
			{
				this.activeSoundProfile = (SoundProfile)null;
			}
			if (this.upcomingSoundProfile != null && (this.upcomingSoundProfile.SoundProfileState == SoundProfileState.CrossfadeIn || this.upcomingSoundProfile.SoundProfileState == SoundProfileState.Paused))
			{
				this.stateTransition.Transition(this.upcomingSoundProfile, new SdLibSoundProfileStateGraph.TransitionDelegate(base.ResumeSound));
				this.upcomingSoundProfile.SoundVolume = this.playerVolume * this.optionVolume;
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(this.upcomingSoundProfile.SoundID, this.playerVolume * this.optionVolume, (Int32)(this.fadeInTimeRemain * 1000f));
				this.SetMusicPanning(this.playerPanning, this.upcomingSoundProfile);
				this.SetMusicPitch(this.playerPitch, this.upcomingSoundProfile);
				this.activeSoundProfile = this.upcomingSoundProfile;
				this.activeSoundProfile.SoundProfileState = SoundProfileState.Played;
				this.upcomingSoundProfile = (SoundProfile)null;
			}
		}
		else
		{
			if (this.upcomingSoundProfile != null)
			{
				if (this.activeSoundProfile != null)
				{
					if (this.stateTransition.Transition(this.activeSoundProfile, new SdLibSoundProfileStateGraph.TransitionDelegate(base.StopSound)) == 0)
					{
						this.activeSoundProfile = this.upcomingSoundProfile;
						this.upcomingSoundProfile = (SoundProfile)null;
					}
				}
				else
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(this.upcomingSoundProfile.SoundID, fadeIn);
					this.upcomingSoundProfile.SoundProfileState = SoundProfileState.Stopped;
					this.upcomingSoundProfile = (SoundProfile)null;
				}
			}
			this.stateTransition.Transition(soundProfileFromIndex, new SdLibSoundProfileStateGraph.TransitionDelegate(base.CreateSound));
			if (this.stateTransition.Transition(soundProfileFromIndex, new SdLibSoundProfileStateGraph.TransitionDelegate(this.StartSoundCrossfadeIn)) == 0)
			{
				this.fadeOutDuration = (Single)fadeIn / 1000f;
				if (this.activeSoundProfile != null)
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(this.activeSoundProfile.SoundID, fadeIn);
					this.activeSoundProfile.SoundProfileState = SoundProfileState.Stopped;
					this.activeSoundProfile = (SoundProfile)null;
				}
			}
		}
	}

	private void StartSoundCrossfadeIn(SoundProfile soundProfile)
	{
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);
		if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 0)
		{
			SoundLib.Log("failed to play sound");
			soundProfile.SoundID = 0;
			return;
		}
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, 0f, 0);
		soundProfile.SoundVolume = this.playerVolume * this.optionVolume;
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, this.playerVolume * this.optionVolume, (Int32)(this.fadeInDuration * 1000f));
		this.SetMusicPanning(this.playerPanning, soundProfile);
		this.SetMusicPitch(this.playerPitch, soundProfile);
		this.upcomingSoundProfile = soundProfile;
	}

	public void CrossFadeInFinish(SoundProfile soundProfile)
	{
	}

	public void PauseMusic()
	{
		this.stateTransition.Transition(this.activeSoundProfile, new SdLibSoundProfileStateGraph.TransitionDelegate(base.PauseSound));
		this.stateTransition.Transition(this.upcomingSoundProfile, new SdLibSoundProfileStateGraph.TransitionDelegate(base.PauseSound));
	}

	public void ResumeMusic()
	{
		this.stateTransition.Transition(this.activeSoundProfile, new SdLibSoundProfileStateGraph.TransitionDelegate(base.ResumeSound));
		this.stateTransition.Transition(this.upcomingSoundProfile, new SdLibSoundProfileStateGraph.TransitionDelegate(base.ResumeSound));
	}

	public Int32 GetActiveSoundID()
	{
		if (this.activeSoundProfile != null)
		{
			return this.activeSoundProfile.SoundID;
		}
		return -1;
	}

	public void StopMusic()
	{
		if (this.stateTransition.Transition(this.activeSoundProfile, new SdLibSoundProfileStateGraph.TransitionDelegate(base.StopSound)) == 0)
		{
			this.activeSoundProfile = (SoundProfile)null;
		}
		if (this.stateTransition.Transition(this.upcomingSoundProfile, new SdLibSoundProfileStateGraph.TransitionDelegate(base.StopSound)) == 0)
		{
			this.upcomingSoundProfile = (SoundProfile)null;
		}
	}

	public void StopMusic(Int32 fadeOut)
	{
		if (this.activeSoundProfile != null)
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(this.activeSoundProfile.SoundID, fadeOut);
			this.activeSoundProfile.SoundProfileState = SoundProfileState.Stopped;
			this.activeSoundProfile = (SoundProfile)null;
		}
		if (this.upcomingSoundProfile != null)
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(this.upcomingSoundProfile.SoundID, fadeOut);
			this.upcomingSoundProfile.SoundProfileState = SoundProfileState.Stopped;
			this.upcomingSoundProfile = (SoundProfile)null;
		}
	}

	public void SetOptionVolume(Int32 volume)
	{
		this.optionVolume = volume / 100f;
		this.SetMusicVolume(this.playerVolume);
	}

	public void SetMusicVolume(Single volume)
	{
		Single oldPlayerVolume = this.playerVolume;
		this.playerVolume = volume;
		if (this.activeSoundProfile != null && this.activeSoundProfile.SoundProfileState == SoundProfileState.Played)
		{
			this.SetMusicVolume(volume, this.activeSoundProfile);
		}
		if (this.upcomingSoundProfile != null && this.upcomingSoundProfile.SoundProfileState == SoundProfileState.CrossfadeIn)
		{
			this.SetMusicVolumeWhileFade(oldPlayerVolume, volume, this.upcomingSoundProfile);
			ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(this.upcomingSoundProfile.SoundID, volume, (Int32)(this.fadeInTimeRemain * 1000f));
		}
	}

	private void SetMusicVolume(Single volume, SoundProfile soundProfile)
	{
		soundProfile.SoundVolume = this.optionVolume * volume;
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume, 0);
	}

	private void SetMusicVolumeWhileFade(Single oldPlayerVolume, Single newPlayerVolume, SoundProfile soundProfile)
	{
		Single num = ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_GetVolume(soundProfile.SoundID);
		Single volume = 0f;
		if (oldPlayerVolume != 0f)
		{
			Single num2 = num / oldPlayerVolume;
			Single num3 = newPlayerVolume - oldPlayerVolume;
			volume = num + num3 * num2;
		}
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, volume, 0);
	}

	public void SetMusicPanning(Single panning)
	{
		this.playerPanning = panning;
		if (this.activeSoundProfile != null)
		{
			this.SetMusicPanning(panning, this.activeSoundProfile);
		}
	}

	private void SetMusicPanning(Single panning, SoundProfile soundProfile)
	{
		soundProfile.Panning = panning;
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPanning(soundProfile.SoundID, soundProfile.Panning, 0);
	}

	public void SetMusicPitch(Single pitch)
	{
		this.playerPitch = pitch;
		if (this.activeSoundProfile != null)
		{
			this.SetMusicPitch(pitch, this.activeSoundProfile);
		}
	}

	private void SetMusicPitch(Single pitch, SoundProfile soundProfile)
	{
		soundProfile.Pitch = pitch;
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, soundProfile.Pitch, 0);
	}

	public void SeekActiveSound(Int32 offsetTimeMSec)
	{
		if (this.activeSoundProfile == null)
		{
			SoundLib.Log("(activeSoundProfile == null");
			return;
		}
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(this.activeSoundProfile.SoundID, 0);
		this.activeSoundProfile.SoundID = ISdLibAPIProxy.Instance.SdSoundSystem_CreateSound(this.activeSoundProfile.BankID);
		Int32 num = ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(this.activeSoundProfile.SoundID, offsetTimeMSec);
		if (num != 0)
		{
			SoundLib.LogError("startSoundErrno errors with value: " + num);
		}
	}

	public override void Update()
	{
		if (this.upcomingSoundProfile != null)
		{
			Single num = ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_GetVolume(this.upcomingSoundProfile.SoundID);
			this.fadeInTimeRemain -= Time.deltaTime;
			if (this.fadeInTimeRemain <= 0f)
			{
				if (this.stateTransition.Transition(this.upcomingSoundProfile, new SdLibSoundProfileStateGraph.TransitionDelegate(this.CrossFadeInFinish)) == 0)
				{
					this.activeSoundProfile = this.upcomingSoundProfile;
					this.upcomingSoundProfile = (SoundProfile)null;
				}
				this.fadeInTimeRemain = 0f;
				this.fadeInDuration = 0f;
			}
		}
	}

	public SoundDatabase soundDatabase = new SoundDatabase();

	private SoundDatabase onTheFlySoundDatabase = new SoundDatabase();

	private SdLibSoundProfileStateGraph stateTransition;

	protected SoundProfile activeSoundProfile;

	private SoundProfile upcomingSoundProfile;

	private Single playerVolume;

	private Single playerPitch;

	private Single playerPanning;

	private Single fadeInDuration;

	private Single fadeOutDuration;

	private Single fadeInTimeRemain;

	private SoundProfile onTheFlyLoadedSoundProfile;

	private Int32 onTheFlyLoadedFadeIn;

	private Single optionVolume = 1f;
}
