using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Memoria;
using Memoria.Assets;
using UnityEngine;

public class VoicePlayer : SoundPlayer
{
	public VoicePlayer()
	{
		this.playerVolume = 1f;
		this.playerPitch = 0.5f;
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

	public static Boolean HasDialogVoice(Dialog dialog)
	{
		if (!Configuration.VoiceActing.Enabled)
			return false;

		SoundProfile attachedVoice;
		if (soundOfDialog.TryGetValue(dialog, out attachedVoice))
			return ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(attachedVoice.SoundID) == 1;
		return false;
	}

	public new void StartSound(SoundProfile soundProfile, Single playerVolume = 1f) => StaticStartSound(soundProfile, playerVolume);
	public void StartSound(SoundProfile soundProfile, Single playerVolume = 1f, Action onFinished = null) => StaticStartSound(soundProfile, playerVolume, onFinished);

	public static void StaticStartSound(SoundProfile soundProfile, Single playerVolume = 1f, Action onFinished = null)
	{
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);

		if (onFinished != null)
		{
			Thread onFinishThread = new Thread(() =>
			{
				try
				{
					// we need to delay if we run it instantly we're at 0 = 0 which is usless
					Thread.Sleep(1000);
					while (true)
					{
						Int32 currentTime = ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_GetElapsedPlaybackTime(soundProfile.SoundID);
						if (currentTime == 0)
						{
							onFinished();
							break;
						}
						Thread.Sleep(200);
					}
					watcherOfSound.Remove(soundProfile);
				}
				catch (Exception err)
				{
					watcherOfSound.Remove(soundProfile);
				}
			});
			watcherOfSound[soundProfile] = onFinishThread;
			onFinishThread.Start(soundProfile);
		}

		if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 0)
		{
			SoundLib.Log("failed to play sound");
			soundProfile.SoundID = 0;
			return;
		}
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, 1f, 0);
		SoundLib.Log("Panning: " + soundProfile.Panning);
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPanning(soundProfile.SoundID, soundProfile.Panning, 0);
		ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, soundProfile.Pitch, 0);
		SoundLib.Log("StartSound Success");
	}

	public static void PlayFieldZoneDialogAudio(Int32 FieldZoneId, Int32 messageNumber, Dialog dialog)
	{
		if (!Configuration.VoiceActing.Enabled)
			return;

		Boolean useAlternatePath = false;
		String vaAlternatePath = null;
		if (dialog.ChoiceNumber <= 0 && dialog.SubPage.Count <= 1 && dialog.Po != null)
		{
			vaAlternatePath = String.Format("Voices/{0}/{1}/va_{2}_{3}", Localization.GetSymbol(), FieldZoneId, messageNumber, dialog.Po.uid);
			if (AssetManager.HasAssetOnDisc("Sounds/" + vaAlternatePath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaAlternatePath + ".ogg", true, false))
				useAlternatePath = true;
		}
		String vaPath = String.Format("Voices/{0}/{1}/va_{2}", Localization.GetSymbol(), FieldZoneId, messageNumber);
		if (dialog.SubPage.Count > 1)
			vaPath += "_" + Math.Max(0, dialog.CurrentPage - 1).ToString();

		String[] msgStrings = dialog.Phrase.Split(new String[] { "[CHOO]" }, StringSplitOptions.None);
		String msgString = msgStrings.Length > 0 ? messageOpcodeRegex.Replace(msgStrings[0], (match) => { return ""; }) : "";

		Boolean shouldDismiss = Configuration.VoiceActing.AutoDismissDialogAfterCompletion && dialog.ChoiceNumber <= 0;
		Action nonDismissAction = () => { soundOfDialog.Remove(dialog); };
		Action dismissAction =
			() =>
			{
				soundOfDialog.Remove(dialog);
				if (dialog.EndMode > 0 && FF9StateSystem.Common.FF9.fldMapNo != 3009 && FF9StateSystem.Common.FF9.fldMapNo != 3010)
					return; // Timed dialog: let the dialog's own timed automatic closure handle it
				while (dialog.CurrentState == Dialog.State.OpenAnimation || dialog.CurrentState == Dialog.State.TextAnimation)
					Thread.Sleep(1000 / Configuration.Graphics.FieldTPS);
				if (!dialog.gameObject.activeInHierarchy || dialog.CurrentState != Dialog.State.CompleteAnimation)
					return;
				if (!dialog.FlagButtonInh) // This dialog can be closed normally
					dialog.ForceClose();
				else if (VoicePlayer.scriptRequestedButtonPress) // It looks like this dialog is closed by the script on a key press
					EventInput.ReceiveInput(EventInput.Pcircle | EventInput.Lcircle);
			};

		if (useAlternatePath)
		{
			SoundLib.VALog(String.Format("field:{0}, msg:{1}, text:{2}, path:{3}", FieldZoneId, messageNumber, msgString, vaAlternatePath));
			soundOfDialog[dialog] = CreateLoadThenPlayVoice(vaAlternatePath.GetHashCode(), vaAlternatePath, shouldDismiss ? dismissAction : nonDismissAction);
		}
		else if (AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false))
		{
			SoundLib.VALog(String.Format("field:{0}, msg:{1}, text:{2}, path:{3}", FieldZoneId, messageNumber, msgString, vaPath));
			soundOfDialog[dialog] = CreateLoadThenPlayVoice(vaPath.GetHashCode(), vaPath, shouldDismiss ? dismissAction : nonDismissAction);
		}
		else
		{
			if (String.IsNullOrEmpty(vaAlternatePath))
				SoundLib.VALog(String.Format("field:{0}, msg:{1}, text:{2}, path:{3} (not found)", FieldZoneId, messageNumber, msgString, vaPath));
			else
				SoundLib.VALog(String.Format("field:{0}, msg:{1}, text:{2}, path:{3}, multiplay-path:{4} (not found)", FieldZoneId, messageNumber, msgString, vaPath, vaAlternatePath));
		}

		if (dialog.ChoiceNumber > 0)
		{
			dialog.OnOptionChange = (Int32 msg, Int32 optionIndex) =>
			{
				if (dialog.CurrentState != Dialog.State.CompleteAnimation)
					return;

				FieldZoneReleaseVoice(dialog, true);

				String vaOptionPath = String.Format("Voices/{0}/{1}/va_{2}_{3}", Localization.GetSymbol(), FieldZoneId, messageNumber, optionIndex);

				String[] options = msgStrings.Length >= 2 ? msgStrings[1].Split('\n') : new String[0];
				Int32 selectedVisibleOption = dialog.ActiveIndexes.Count > 0 ? Math.Max(0, dialog.ActiveIndexes.FindIndex(index => index == optionIndex)) : optionIndex;
				String optString = selectedVisibleOption < options.Length ? messageOpcodeRegex.Replace(options[selectedVisibleOption].Trim(), (match) => { return ""; }) : "[Invalid option index]";

				if (!AssetManager.HasAssetOnDisc("Sounds/" + vaOptionPath + ".akb", true, true) && !AssetManager.HasAssetOnDisc("Sounds/" + vaOptionPath + ".ogg", true, false))
				{
					SoundLib.VALog(String.Format("field:{0}, msg:{1}, opt:{2}, text:{3} path:{4} (not found)", FieldZoneId, messageNumber, optionIndex, optString, vaOptionPath));
				}
				else
				{
					SoundLib.VALog(String.Format("field:{0}, msg:{1}, opt:{2}, text:{3} path:{4}", FieldZoneId, messageNumber, optionIndex, optString, vaOptionPath));
					soundOfDialog[dialog] = CreateLoadThenPlayVoice(vaOptionPath.GetHashCode(), vaOptionPath, nonDismissAction);
				}
			};
        }
	}

	public static void FieldZoneDialogClosed(Dialog dialog)
	{
		FieldZoneReleaseVoice(dialog, Configuration.VoiceActing.StopVoiceWhenDialogDismissed && !dialog.IsClosedByScript);
	}

	private static void FieldZoneReleaseVoice(Dialog dialog, Boolean stopSound)
	{
		SoundProfile attachedVoice;
		if (soundOfDialog.TryGetValue(dialog, out attachedVoice))
		{
			if (stopSound && ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(attachedVoice.SoundID) == 1)
				SoundLib.voicePlayer.StopSound(attachedVoice);
			Thread soundWatcher;
			if (watcherOfSound.TryGetValue(attachedVoice, out soundWatcher))
			{
				soundWatcher.Interrupt();
				watcherOfSound.Remove(attachedVoice);
			}
			soundOfDialog.Remove(dialog);
		}
	}

	public static SoundProfile CreateLoadThenPlayVoice(Int32 soundIndex, String vaPath, Action onFinished = null)
    {
		SoundProfile soundProfile = new SoundProfile
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

		SoundLoaderProxy.Instance.Load(soundProfile,
		(soundProfile, db) =>
		{
			if (soundProfile != null)
			{
				SoundLib.voicePlayer.CreateSound(soundProfile);
				SoundLib.voicePlayer.StartSound(soundProfile, 1, onFinished);
				if (db.ReadAll().ContainsKey(soundProfile.SoundIndex))
					db.Update(soundProfile);
				else
					db.Create(soundProfile);
			}
		},
		ETb.voiceDatabase);

		return soundProfile;
	}

	public static void PlayBattleVoice(Int32 va_id, String text, Boolean asSharedMessage = false, Int32 battleId = -1)
	{
		if (!Configuration.VoiceActing.Enabled)
			return;

		if (battleId < 0)
			battleId = FF9StateSystem.Battle.battleMapIndex;
		String btlFolder = asSharedMessage ? "general" : battleId.ToString();

		String vaPath = String.Format("Voices/{0}/battle/{2}/va_{1}", Localization.GetSymbol(), va_id, btlFolder).ToLower();
		if (!AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) && !AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false))
		{
			SoundLib.VALog(String.Format("field:battle/{0}, msg:{1}, text:{2} path:{3} (not found)", btlFolder, va_id, text, vaPath));
			return;
		}

		SoundLib.VALog(String.Format("field:battle/{0}, msg:{1}, text:{2} path:{3}", btlFolder, va_id, text, vaPath));

		CreateLoadThenPlayVoice(va_id, vaPath);
	}

	public void PauseAllSounds()
	{
		Dictionary<Int32, SoundProfile> database = ETb.voiceDatabase.ReadAll();
		foreach (SoundProfile soundProfile in database.Values)
			if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 1)
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 1, 0);
	}

	public void ResumeAllSounds()
	{
		Dictionary<Int32, SoundProfile> database = ETb.voiceDatabase.ReadAll();
		foreach (SoundProfile soundProfile in database.Values)
			if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 1)
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(soundProfile.SoundID, 0, 0);
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
		if (!Configuration.VoiceActing.Enabled)
			return;

		if (this.upcomingSoundProfile != null)
		{
			Single num = ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_GetVolume(this.upcomingSoundProfile.SoundID);
			this.fadeInTimeRemain -= Time.deltaTime;
			if (this.fadeInTimeRemain <= 0f)
			{
				if (this.stateTransition.Transition(this.upcomingSoundProfile, new SdLibSoundProfileStateGraph.TransitionDelegate(this.CrossFadeInFinish)) == 0)
				{
					this.activeSoundProfile = this.upcomingSoundProfile;
					this.upcomingSoundProfile = null;
				}
				this.fadeInTimeRemain = 0f;
				this.fadeInDuration = 0f;
			}
		}
	}

	private static readonly Regex messageOpcodeRegex = new Regex(@"\[[A-Za-z0-9=]*\]");

	public static Dictionary<Dialog, SoundProfile> soundOfDialog = new Dictionary<Dialog, SoundProfile>();

	public static Dictionary<SoundProfile, Thread> watcherOfSound = new Dictionary<SoundProfile, Thread>();

	public static Boolean scriptRequestedButtonPress = false;

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
