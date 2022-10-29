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

	private static string getAbillityNameName(BattleAbilityId id)
    {
		switch (id)
		{

			case BattleAbilityId.Carbuncle1:
			case BattleAbilityId.Carbuncle2:
			case BattleAbilityId.Carbuncle3:
			case BattleAbilityId.Carbuncle4:
				return "Carbuncle";
			case BattleAbilityId.Fenrir1:
			case BattleAbilityId.Fenrir2:
				return "Fenrir";
			default:
				return Enum.GetName(typeof(BattleAbilityId), id);
		}
	}

	private static string getCommandName(CMD_DATA cmd)
    {
		switch (cmd.cmd_no)
		{
			case BattleCommandId.Jump2:
				return "Jump";
			case BattleCommandId.SummonGarnet:
			case BattleCommandId.SummonEiko:
				return "Summon_" + getAbillityNameName((BattleAbilityId)cmd.sub_no) + ((cmd.info.short_summon == 1) ? "_short" : "_full");
			case BattleCommandId.WhiteMagicGarnet:
			case BattleCommandId.WhiteMagicEiko:
			case BattleCommandId.WhiteMagicCinna1:
			case BattleCommandId.WhiteMagicCinna2:
				return "WhiteMagic_" + getAbillityNameName((BattleAbilityId)cmd.sub_no);
			case BattleCommandId.HolySword1:
			case BattleCommandId.HolySword2:
				return "HolySword_" + getAbillityNameName((BattleAbilityId)cmd.sub_no);
			case BattleCommandId.RedMagic1:
			case BattleCommandId.RedMagic2:
				return "RedMagic_" + getAbillityNameName((BattleAbilityId)cmd.sub_no);
			case BattleCommandId.YellowMagic1:
			case BattleCommandId.YellowMagic2:
				return "YellowMagic_" + getAbillityNameName((BattleAbilityId)cmd.sub_no);
			case BattleCommandId.StageMagicZidane:
			case BattleCommandId.StageMagicBlank:
			case BattleCommandId.StageMagicMarcus:
			case BattleCommandId.StageMagicCinna:
				return "StageMagic";
			default:
				return Enum.GetName(typeof(BattleCommandId), cmd.cmd_no);
		}
	}

	private static System.Random rand = new System.Random();

	public static void PlayBattleActionTakenVoice(BattleUnit unit, CMD_DATA cmd)
    {
		string cmdName = getCommandName(cmd);
        
		// hopefully this is a uniuqe ID set.
		int soundIndex = 2140000000 + ((int)cmd.cmd_no * 10000) + (cmd.sub_no*100) + unit.Id;

		String vaPath = String.Format("Voices/{0}/battle/shared/va_use_{1}_{2}", Localization.GetSymbol(), cmdName, unit.Player.Name).ToLower();
		if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
		{
			SoundLib.VALog(String.Format("Player Char used Abbility, msg:{0}, caster:{1} path:{2} (not found)", cmdName.ToLower(), unit.Player.Name, vaPath));
			return;
		}

		int randomNumber = rand.Next(0, Configuration.Audio.CharAttackAudioChance);
		if (randomNumber == (int)(Configuration.Audio.CharAttackAudioChance / 2) || (int)(Configuration.Audio.CharAttackAudioChance / 2) == 0)
		{
			SoundLib.VALog(String.Format("Player Char used Abbility, msg:{0}, caster:{1} path:{2}", cmdName.ToLower(), unit.Player.Name, vaPath));
			CreateLoadThenPlayVoice(soundIndex, vaPath);
		}
	}

	public static void PlayBattleActionRecivedVoice(BattleUnit unit, CMD_DATA cmd)
	{
		string cmdName = getCommandName(cmd);
		// hopefully this is a uniuqe ID set.
		int soundIndex = 2141000000 + ((int)cmd.cmd_no * 10000) + (cmd.sub_no * 100) + unit.Id;

		String vaPath = String.Format("Voices/{0}/battle/shared/va_hit_{1}_{2}", Localization.GetSymbol(), cmdName, unit.Player.Name).ToLower();
		if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
		{
			SoundLib.VALog(String.Format("Player Char hit with Abbility, msg:{0}, caster:{1} path:{2} (not found)", cmdName.ToLower(), unit.Player.Name, vaPath));
			return;
		}

		int randomNumber = rand.Next(0, Configuration.Audio.CharHitAudioChance);
		if (randomNumber == (int)(Configuration.Audio.CharHitAudioChance / 2) || (int)(Configuration.Audio.CharHitAudioChance / 2) == 0)
		{

			SoundLib.VALog(String.Format("Player Char hit with Abbility, msg:{0}, caster:{1} path:{2}", cmdName.ToLower(), unit.Player.Name, vaPath));

			CreateLoadThenPlayVoice(soundIndex, vaPath);
		}
	}

	public enum BattleStartType : byte
	{
		Normal = 0,
		Special = 2,
		Back = 3,
		Preemtive = 4
    }

	public enum BattleEndType : byte
    {
		GameOver = 1,
		Victory = 2,
		Forced = 3
    }

	public static void PlayBattleStartVoice(ref PLAYER[] characters, BattleStartType bst)
    {
		string battleType = "normal";
        switch (bst)
        {
			case BattleStartType.Special:
				battleType = "special";
				break;
			case BattleStartType.Back:
				battleType = "back";
				break;
			case BattleStartType.Preemtive:
				battleType = "preemtive";
				break;
		}
		foreach (PLAYER character in characters)
		{
			String playerName = character.Name;
			int soundIndex = 2142000000 + (int)character.info.serial_no;

			String vaPath = String.Format("Voices/{0}/battle/shared/va_{1}_start_{2}", Localization.GetSymbol(), playerName, battleType).ToLower();
			if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
			{
				SoundLib.VALog(String.Format("field:battle/shared, BattleStart:{0} StartType:{1} path:{2} (not found)", playerName, battleType, vaPath));
				return;
			}

			int randomNumber = rand.Next(0, Configuration.Audio.StartBattleChance);
			if (randomNumber == (int)(Configuration.Audio.StartBattleChance / 2) || (int)(Configuration.Audio.StartBattleChance / 2) == 0)
			{
				CreateLoadThenPlayVoice(soundIndex, vaPath);
			}
		}
	}

	public static void PlayBattleEndVoice(ref PLAYER[] characters, BattleEndType bet)
	{
		string endType = "victory";
		switch (bet)
		{
			case BattleEndType.GameOver:
				endType = "gameover";
				break;
			case BattleEndType.Forced:
				endType = "forced";
				break;
		}
		foreach (PLAYER character in characters)
		{
			String playerName = character.Name;
			int soundIndex = 2143000000 + (int)character.info.serial_no;

			String vaPath = String.Format("Voices/{0}/battle/shared/va_{1}_end_{2}", Localization.GetSymbol(), playerName, endType).ToLower();
			if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
			{
				SoundLib.VALog(String.Format("field:battle/shared, BattleEnd:{0}, EndType:{1} path:{2} (not found)", playerName, endType, vaPath));
				return;
			}

			int randomNumber = rand.Next(0, Configuration.Audio.EndBattleChance);
			if (randomNumber == (int)(Configuration.Audio.EndBattleChance / 2) || (int)(Configuration.Audio.EndBattleChance / 2) == 0)
			{
				CreateLoadThenPlayVoice(soundIndex, vaPath);
			}
		}
	}

	public static void PlayBattleDeathVoice(BTL_DATA btl)
	{
		int randomNumber = rand.Next(0, Configuration.Audio.CharDeathChance);
		BattleUnit bu = new BattleUnit(btl);

        if (bu.IsPlayer && (randomNumber == (int)(Configuration.Audio.CharDeathChance / 2) || (int)(Configuration.Audio.CharDeathChance / 2) == 0))
        {
			string playerName = bu.Player.Name;
			int soundIndex = 2145051000 + (int)bu.Player.PresetId;

			String vaPath = String.Format("Voices/{0}/battle/shared/va_{1}_died", Localization.GetSymbol(), playerName).ToLower();
			if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
			{
				SoundLib.VALog(String.Format("field:battle/shared, death:{0}, path:{1} (not found)", playerName, vaPath));
				return;
			}

			SoundLib.VALog(String.Format("field:battle/shared, death:{0}, path:{1}", playerName, vaPath));

			CreateLoadThenPlayVoice(soundIndex, vaPath);
		}
	}

	public static void PlayBattleAutoLifeVoice(BTL_DATA btl)
	{
		int randomNumber = rand.Next(0, Configuration.Audio.CharAutoLifeChance);
		BattleUnit bu = new BattleUnit(btl);

		if (bu.IsPlayer && (randomNumber == (int)(Configuration.Audio.CharAutoLifeChance / 2) || (int)(Configuration.Audio.CharAutoLifeChance / 2) == 0))
		{
			string playerName = bu.Player.Name;
			int soundIndex = 2145052000 + (int)bu.Player.PresetId;

			String vaPath = String.Format("Voices/{0}/battle/shared/va_{1}_autoressed", Localization.GetSymbol(), playerName).ToLower();
			if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
			{
				SoundLib.VALog(String.Format("field:battle/shared, autolife:{0}, path:{1} (not found)", playerName, vaPath));
				return;
			}

			SoundLib.VALog(String.Format("field:battle/shared, autolife:{0}, path:{1}", playerName, vaPath));

			CreateLoadThenPlayVoice(soundIndex, vaPath);
		}
	}
	public static void PlayBattleStatusRemoved(BTL_DATA btl, BattleStatus status)
	{
		int randomNumber = rand.Next(0, Configuration.Audio.CharStatusRemovedChance);
		BattleUnit bu = new BattleUnit(btl);

		if (bu.IsPlayer && (randomNumber == (int)(Configuration.Audio.CharStatusRemovedChance / 2) || (int)(Configuration.Audio.CharStatusRemovedChance / 2) == 0))
		{
			string playerName = bu.Player.Name;
			string statusName = Enum.GetName(typeof(BattleStatus), status);
			int soundIndex = 2145053000 + (int)bu.Player.PresetId;

			String vaPath = String.Format("Voices/{0}/battle/shared/va_{1}_{2}_removed", Localization.GetSymbol(), playerName, statusName).ToLower();
			if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
			{
				SoundLib.VALog(String.Format("field:battle/shared, statusRemoved:{0}, char:{1} path:{2} (not found)", statusName, playerName, vaPath));
				return;
			}

			SoundLib.VALog(String.Format("field:battle/shared, statusRemoved:{0}, char:{1} path:{2}", statusName, playerName, vaPath));

			CreateLoadThenPlayVoice(soundIndex, vaPath);
		}
	}

	public static void PlayBattleStatusAdded(BTL_DATA btl, BattleStatus status)
	{
		int randomNumber = rand.Next(0, Configuration.Audio.CharStatusAfflictedChance);
		BattleUnit bu = new BattleUnit(btl);

		if (bu.IsPlayer && (randomNumber == (int)(Configuration.Audio.CharStatusAfflictedChance / 2) || (int)(Configuration.Audio.CharStatusAfflictedChance / 2) == 0))
		{
			string playerName = bu.Player.Name;
			string statusName = Enum.GetName(typeof(BattleStatus), status);
			int soundIndex = 2145053000 + (int)bu.Player.PresetId;

			String vaPath = String.Format("Voices/{0}/battle/shared/va_{1}_{2}_afflicted", Localization.GetSymbol(), playerName, statusName).ToLower();
			if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
			{
				SoundLib.VALog(String.Format("field:battle/shared, statusAfflicted:{0}, char:{1} path:{2} (not found)", statusName, playerName, vaPath));
				return;
			}

			SoundLib.VALog(String.Format("field:battle/shared, statusAfflicted:{0}, char:{1} path:{2}", statusName, playerName, vaPath));

			CreateLoadThenPlayVoice(soundIndex, vaPath);
		}
	}

	private static void CreateLoadThenPlayVoice(int soundIndex, string vaPath)
    {
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

	public static void PlayBattleVoice(Int32 va_id, String text, Boolean asSharedMessage = false, Int32 btlId = -1)
    {
		if (btlId < 0)
			btlId = FF9StateSystem.Battle.battleMapIndex;
		String btlFolder = asSharedMessage ? "general" : btlId.ToString();

		String vaPath = String.Format("Voices/{0}/battle/{2}/va_{1}", Localization.GetSymbol(), va_id, btlFolder).ToLower();
		if (!(AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false)))
		{
			SoundLib.VALog(String.Format("field:battle/{0}, msg:{1}, text:{2} path:{3} (not found)", btlFolder, va_id, text, vaPath));
			return;
		}

		SoundLib.VALog(String.Format("field:battle/{0}, msg:{1}, text:{2} path:{3}", btlFolder, va_id, text, vaPath));

		CreateLoadThenPlayVoice(va_id, vaPath);
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
