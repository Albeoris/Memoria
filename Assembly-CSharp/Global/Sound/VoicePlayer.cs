using System;
using System.Collections.Generic;
using Memoria;
using Memoria.Assets;
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

	/*
	 * Overrides to stop duplicate files playing when multiple text boxes appear at once
	 */

	private static Dictionary<string, Boolean> preventMultiPlay = new Dictionary<string, Boolean>()
	{
		["Voices/US/2/va_46"] = false
	};

	new public void StartSound(SoundProfile soundProfile, Single playerVolume = 1f) => StaticStartSound(soundProfile, playerVolume);

	new public static void StaticStartSound(SoundProfile soundProfile, Single playerVolume = 1f)
	{
        if (preventMultiPlay.ContainsKey(soundProfile.Name) && preventMultiPlay[soundProfile.Name]) {
			return;
        }
        else
        {
			if (preventMultiPlay.ContainsKey(soundProfile.Name))
			{
				preventMultiPlay[soundProfile.Name] = true;
			}
		}

		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);
		if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 0)
		{
			SoundLib.Log("failed to play sound");
			soundProfile.SoundID = 0;
			return;
		}
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * playerVolume, 0);
		SoundLib.Log("Panning: " + soundProfile.Panning);
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPanning(soundProfile.SoundID, soundProfile.Panning, 0);
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, soundProfile.Pitch, 0);
		SoundLib.Log("StartSound Success");
	}

	public static void PlayBattleVoice(Int32 va_id, String text, Boolean asSharedMessage = false, Int32 btlId = -1)
    {
		if (btlId < 0)
			btlId = FF9StateSystem.Battle.battleMapIndex;
		String btlFolder = asSharedMessage ? "general" : btlId.ToString();
		String vaPath = String.Format("Voices/{0}/battle/{2}/va_{1}", Localization.GetSymbol(), va_id, btlFolder);
		if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
		{
			SoundLib.VALog(String.Format("field:battle/{0}, msg:{1}, text:{2} path:{3} (not found)", btlFolder, va_id, text, vaPath));
			return;
		}

		SoundLib.VALog(String.Format("field:battle/{0}, msg:{1}, text:{2} path:{3}", btlFolder, va_id, text, vaPath));
		var currentVAFile = new SoundProfile
		{
			Code = va_id.ToString(),
			Name = vaPath,
			SoundIndex = va_id,
			ResourceID = vaPath,
			SoundProfileType = SoundProfileType.Voice,
			SoundVolume = 1f,
			Panning = 0f,
			Pitch = 0.5f
		};

		SoundLoaderProxy.Instance.Load(currentVAFile,
		(soundProfile, db) =>
		{
			if (soundProfile != null)
			{
				SoundLib.voicePlayer.CreateSound(soundProfile);
				SoundLib.voicePlayer.StartSound(soundProfile);
				if (db.ReadAll().ContainsKey(soundProfile.SoundIndex))
					db.Update(soundProfile);
				else
					db.Create(soundProfile);
			}
		},
		ETb.voiceDatabase);
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

	private void SetMusicPanning(Single panning, SoundProfile soundProfile)
	{
		soundProfile.Panning = panning;
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPanning(soundProfile.SoundID, soundProfile.Panning, 0);
	}

	private void SetMusicPitch(Single pitch, SoundProfile soundProfile)
	{
		soundProfile.Pitch = pitch;
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, soundProfile.Pitch, 0);
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
