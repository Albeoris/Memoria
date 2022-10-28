using System;
using System.Collections.Generic;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
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

	public static Dictionary<string, UInt16> preventMultiPlay { get; set; } = Configuration.Audio.preventMultiPlay;

	new public void StartSound(SoundProfile soundProfile, Single playerVolume = 1f) => StaticStartSound(soundProfile, playerVolume);

	new public static void StaticStartSound(SoundProfile soundProfile, Single playerVolume = 1f)
	{
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

	private static string getSummonName(byte summon_id)
    {
		switch (summon_id)
		{
			case 49:
				return "shiva";
			case 51:
				return "ifrit";
			case 53:
				return "ramuh";
			case 55:
				return "atomos";
			case 58:
				return "odin";
			case 60:
				return "leviathan";
			case 62:
				return "bahamut";
			case 64:
				return "arc";
			case 68:
			case 69:
			case 70:
			case 71:
				return "carbuncle";
			case 66:
			case 67:
				return "fenrir";
			case 72:
				return "phoenix";
			case 74:
				return "madeen";
		}
		return "";
	}

	private static System.Random rand = new System.Random();

	public static void PlayBattleActionTakenVoice(BattleUnit unit, BattleCommandId command_id, CMD_DATA cmd)
    {
		string cmdName = "";
        switch (command_id)
        {
			case BattleCommandId.Jump2:
				cmdName = "Jump";
				break;
			case BattleCommandId.SummonGarnet:
			case BattleCommandId.SummonEiko:
				cmdName = "Summon_" + getSummonName(cmd.sub_no) + ((cmd.info.short_summon == 1) ? "_short" : "_full");
				break;
			case BattleCommandId.WhiteMagicGarnet:
			case BattleCommandId.WhiteMagicEiko:
			case BattleCommandId.WhiteMagicCinna1:
			case BattleCommandId.WhiteMagicCinna2:
				cmdName = "WhiteMagic";
				break;
			case BattleCommandId.HolySword1:
			case BattleCommandId.HolySword2:
				cmdName = "HolySword";
				break;
			case BattleCommandId.RedMagic1:
			case BattleCommandId.RedMagic2:
				cmdName = "RedMagic";
				break;
			case BattleCommandId.YellowMagic1:
			case BattleCommandId.YellowMagic2:
				cmdName = "YellowMagic";
				break;
			case BattleCommandId.StageMagicZidane:
			case BattleCommandId.StageMagicBlank:
			case BattleCommandId.StageMagicMarcus:
			case BattleCommandId.StageMagicCinna:
				cmdName = "StageMagic";
				break;
			default: 
				cmdName = Enum.GetName(typeof(BattleCommandId), command_id);
				break;
		}
		// hopefully this is a uniuqe ID set.
		int soundIndex = 2147000000 + ((int)command_id * 1000) + (cmd.sub_no*10) + unit.Id;

		String vaPath = String.Format("Voices/{0}/battle/shared/va_use_{1}_{2}", Localization.GetSymbol(), cmdName.ToLower(), unit.Player.Name);
		if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
		{
			SoundLib.VALog(String.Format("Player Char used Abbility, msg:{0}, caster:{1} path:{2} (not found)", cmdName.ToLower(), unit.Player.Name, vaPath));
			return;
		}

		int randomNumber = rand.Next(0, Configuration.Audio.CharAttackAudioChance);
		if (randomNumber == (int)(Configuration.Audio.CharAttackAudioChance / 2) || (int)(Configuration.Audio.CharAttackAudioChance / 2) == 0)
		{
			SoundLib.VALog(String.Format("Player Char used Abbility, msg:{0}, caster:{1} path:{2}", cmdName.ToLower(), unit.Player.Name, vaPath));
			var currentVAFile = new SoundProfile
			{
				Code = soundIndex.ToString(),
				Name = vaPath,
				SoundIndex = soundIndex,
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
	}

	public static void PlayBattleActionRecivedVoice(BattleUnit unit, BattleCommandId command_id, CMD_DATA cmd)
	{
		string cmdName = "";
		switch (command_id)
		{
			case BattleCommandId.Jump2:
				cmdName = "Jump";
				break;
			case BattleCommandId.SummonGarnet:
			case BattleCommandId.SummonEiko:
				cmdName = "Summon_" + getSummonName(cmd.sub_no)+((cmd.info.short_summon == 1)? "_short":"_full");
				break;
			case BattleCommandId.WhiteMagicGarnet:
			case BattleCommandId.WhiteMagicEiko:
			case BattleCommandId.WhiteMagicCinna1:
			case BattleCommandId.WhiteMagicCinna2:
				cmdName = "WhiteMagic";
				break;
			case BattleCommandId.HolySword1:
			case BattleCommandId.HolySword2:
				cmdName = "HolySword";
				break;
			case BattleCommandId.RedMagic1:
			case BattleCommandId.RedMagic2:
				cmdName = "RedMagic";
				break;
			case BattleCommandId.YellowMagic1:
			case BattleCommandId.YellowMagic2:
				cmdName = "YellowMagic";
				break;
			case BattleCommandId.StageMagicZidane:
			case BattleCommandId.StageMagicBlank:
			case BattleCommandId.StageMagicMarcus:
			case BattleCommandId.StageMagicCinna:
				cmdName = "StageMagic";
				break;
			default:
				cmdName = Enum.GetName(typeof(BattleCommandId), command_id);
				break;
		}
		// hopefully this is a uniuqe ID set.
		int soundIndex = 2147100000 + ((int)command_id * 1000) + (cmd.sub_no * 10) + unit.Id;

		String vaPath = String.Format("Voices/{0}/battle/shared/va_hit_{1}_{2}", Localization.GetSymbol(), cmdName.ToLower(), unit.Player.Name);
		if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
		{
			SoundLib.VALog(String.Format("Player Char hit with Abbility, msg:{0}, caster:{1} path:{2} (not found)", cmdName.ToLower(), unit.Player.Name, vaPath));
			return;
		}

		int randomNumber = rand.Next(0, Configuration.Audio.CharHitAudioChance);
		if (randomNumber == (int)(Configuration.Audio.CharHitAudioChance / 2) || (int)(Configuration.Audio.CharHitAudioChance / 2) == 0)
		{

			SoundLib.VALog(String.Format("Player Char hit with Abbility, msg:{0}, caster:{1} path:{2}", cmdName.ToLower(), unit.Player.Name, vaPath));
			var currentVAFile = new SoundProfile
			{
				Code = soundIndex.ToString(),
				Name = vaPath,
				SoundIndex = soundIndex,
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
