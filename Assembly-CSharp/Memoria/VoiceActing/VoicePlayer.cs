﻿using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class VoicePlayer : SoundPlayer
{
    private static ushort specialLastPlayed;
    private static string specialAppend = "";
    private static ushort specialCount = 0;
    public VoicePlayer()
    {
        this.playerPitch = 1f;
        this.playerPanning = 0f;
        this.fadeInDuration = 0f;
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

    public void StartSound(SoundProfile soundProfile, Action onFinished = null)
    {
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
                catch (Exception)
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
        ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.Volume, 0);
        ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPanning(soundProfile.SoundID, soundProfile.Panning, 0);
        ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(soundProfile.SoundID, soundProfile.Pitch, 0);
        ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);
        SoundLib.Log("StartSound Success");
    }

    public static void PlayFieldZoneDialogAudio(Int32 FieldZoneId, Int32 messageNumber, Dialog dialog)
    {
        if (!Configuration.VoiceActing.Enabled)
            return;

        specialAppend = "";
        // (special) Festival of the Hunt has take the lead and holds the lead
        switch (FieldZoneId)
        {
            case 276:
            {
                switch (messageNumber)
                {
                    case 540:// Zidane
                    case 541:// Vivi
                    case 542:// Frya
                    case 543:// Lani
                    case 544:// Gourmand
                    case 545:// Belna
                    case 546:// Genero
                    case 547:// Ivan
                    {
                        var idShort = Convert.ToUInt16(messageNumber);
                        if (idShort == specialLastPlayed)
                        {
                            specialAppend = "_held_" + specialCount;
                            specialCount += 1;
                        }
                        else
                        {
                            specialAppend = "_taken";
                            specialLastPlayed = idShort;
                            specialCount = 0;
                        }
                        break;
                    }
                }
                break;
            }
            // hot and cold
            case 945:
            {
                if(messageNumber == 230)
                {
                    specialCount = 0;
                }
                // count up for each time you find something.
                if(messageNumber == 301) // gained points
                {
                    specialAppend = "_" + specialCount;
                    specialCount += 1;
                }
                break;
            }
        }

        String vaPath = String.Format("Voices/{0}/{1}/va_{2}{3}", Localization.GetSymbol(), FieldZoneId, messageNumber, specialAppend);
        Boolean useAlternatePath = false;
        String vaAlternatePath = null;
        String vaAlternatePath2 = null;

        if (dialog.Po != null)
        {
            vaAlternatePath = String.Format("Voices/{0}/{1}/va_{2}_{3}{4}", Localization.GetSymbol(), FieldZoneId, messageNumber, dialog.Po.uid, specialAppend);
            if (AssetManager.HasAssetOnDisc("Sounds/" + vaAlternatePath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaAlternatePath + ".ogg", true, false))
            {
                vaPath = vaAlternatePath;
                useAlternatePath = true;
            }
        }
        if (dialog.SubPage.Count > 1)
        {
            String pageIndex = "_" + Math.Max(0, dialog.CurrentPage - 1).ToString();
            vaPath += pageIndex;
            if (!String.IsNullOrEmpty(vaAlternatePath))
                vaAlternatePath += pageIndex;
        }

        String[] msgStrings = dialog.Phrase.Split(new String[] { "[CHOO]" }, StringSplitOptions.None);
        String msgString = msgStrings.Length > 0 ? messageOpcodeRegex.Replace(msgStrings[0], (match) => { return ""; }) : "";

        if (!useAlternatePath && msgString.Length > 0 && msgString.Contains("\n“"))
        {
            string name = msgString.Split('\n')[0].Trim();
            vaAlternatePath2 = string.Format("Voices/{0}/{1}/va_{2}_{3}{4}", Localization.GetSymbol(), FieldZoneId, messageNumber, name, specialAppend);
            if (AssetManager.HasAssetOnDisc("Sounds/" + vaAlternatePath2 + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaAlternatePath2 + ".ogg", true, false))
            {
                vaPath = vaAlternatePath = vaAlternatePath2;
                useAlternatePath = true;
            }
        }

        Boolean hasChoices = dialog.ChoiceNumber > 0;
        Boolean isMsgEmpty = msgString.Length == 0;
        Boolean shouldDismiss = Configuration.VoiceActing.AutoDismissDialogAfterCompletion && !hasChoices;
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
                {
                    ETb.sKey0 &= ~(EventInput.Pcircle | EventInput.Lcircle);
                    EventInput.ReceiveInput(EventInput.Pcircle | EventInput.Lcircle);
                }
            };
        Action playSelectChoiceAction =
            () =>
            {
                soundOfDialog.Remove(dialog);
                while (dialog.CurrentState == Dialog.State.OpenAnimation || dialog.CurrentState == Dialog.State.TextAnimation || !dialog.IsChoiceReady)
                    Thread.Sleep(1000 / Configuration.Graphics.FieldTPS);
                dialog.OnOptionChange?.Invoke(dialog.Id, dialog.SelectChoice); // Simulate a choice change so the default selected line plays
            };

        if (useAlternatePath)
        {
            SoundLib.VALog(String.Format("field:{0}, msg:{1}, text:{2}, path:{3}", FieldZoneId, messageNumber, msgString, vaAlternatePath));
            soundOfDialog[dialog] = CreateLoadThenPlayVoice(vaAlternatePath.GetHashCode(), vaAlternatePath, shouldDismiss ? dismissAction : (hasChoices ? playSelectChoiceAction : nonDismissAction));
        }
        else if (AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false))
        {
            SoundLib.VALog(String.Format("field:{0}, msg:{1}, text:{2}, path:{3}", FieldZoneId, messageNumber, msgString, vaPath));
            soundOfDialog[dialog] = CreateLoadThenPlayVoice(vaPath.GetHashCode(), vaPath, shouldDismiss ? dismissAction : (hasChoices ? playSelectChoiceAction : nonDismissAction));
        }
        else
        {
            if (String.IsNullOrEmpty(vaAlternatePath))
                SoundLib.VALog(String.Format("field:{0}, msg:{1}, text:{2}, path:{3} (not found)", FieldZoneId, messageNumber, msgString, vaPath));
            else if (String.IsNullOrEmpty(vaAlternatePath2))
                SoundLib.VALog(String.Format("field:{0}, msg:{1}, text:{2}, path:{3}, multiplay-path:{4} (not found)", FieldZoneId, messageNumber, msgString, vaPath, vaAlternatePath));
            else
                SoundLib.VALog(String.Format("field:{0}, msg:{1}, text:{2}, path:{3}, multiplay-path:{4} named-path:{5} (not found)", FieldZoneId, messageNumber, msgString, vaPath, vaAlternatePath, vaAlternatePath2));
            isMsgEmpty = true;
        }

        if (hasChoices)
        {
            dialog.OnOptionChange = (Int32 msg, Int32 optionIndex) =>
            {
                if (dialog.CurrentState != Dialog.State.CompleteAnimation || !dialog.IsChoiceReady)
                    return;

                String vaOptionPath = vaPath + "_" + optionIndex;
                String[] options = msgStrings.Length >= 2 ? msgStrings[1].Split('\n') : new String[0];
                Int32 selectedVisibleOption = dialog.ActiveIndexes.Count > 0 ? Math.Max(0, dialog.ActiveIndexes.FindIndex(index => index == optionIndex)) : optionIndex;
                String optString = selectedVisibleOption < options.Length ? messageOpcodeRegex.Replace(options[selectedVisibleOption].Trim(), (match) => { return ""; }) : "[Invalid option index]";

                if (!AssetManager.HasAssetOnDisc("Sounds/" + vaOptionPath + ".akb", true, true) && !AssetManager.HasAssetOnDisc("Sounds/" + vaOptionPath + ".ogg", true, false))
                {
                    SoundLib.VALog(String.Format("field:{0}, msg:{1}, opt:{2}, text:{3} path:{4} (not found)", FieldZoneId, messageNumber, optionIndex, optString, vaOptionPath));
                }
                else
                {
                    FieldZoneReleaseVoice(dialog, true);
                    SoundLib.VALog(String.Format("field:{0}, msg:{1}, opt:{2}, text:{3} path:{4}", FieldZoneId, messageNumber, optionIndex, optString, vaOptionPath));
                    soundOfDialog[dialog] = CreateLoadThenPlayVoice(vaOptionPath.GetHashCode(), vaOptionPath, nonDismissAction);
                }
            };

            if (isMsgEmpty) new Thread(() => { playSelectChoiceAction(); }).Start();
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
                SoundLib.VoicePlayer.StopSound(attachedVoice);
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
            Pitch = Configuration.Audio.Backend == 0 ? 0.5f : 1f // SdLib needs 0.5f for some reason
        };

        SoundLoaderProxy.Instance.Load(soundProfile,
        (soundProfile, db) =>
        {
            if (soundProfile != null)
            {
                SoundLib.VoicePlayer.CreateSound(soundProfile);
                SoundLib.VoicePlayer.StartSound(soundProfile, onFinished);
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

        if (!asSharedMessage && UIManager.Battle.IsMessageQueued(FF9TextTool.BattleText(va_id)))
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
        if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(soundProfile.SoundID) == 0)
        {
            SoundLib.Log("failed to play sound");
            soundProfile.SoundID = 0;
            return;
        }
        ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, 0f, 0);
        ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundProfile.SoundID, soundProfile.SoundVolume * this.Volume, (Int32)(this.fadeInDuration * 1000f));
        this.SetMusicPanning(this.playerPanning, soundProfile);
        this.SetMusicPitch(this.playerPitch, soundProfile);
        ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Start(soundProfile.SoundID, 0);
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

    private SdLibSoundProfileStateGraph stateTransition;

    protected SoundProfile activeSoundProfile;

    private SoundProfile upcomingSoundProfile;

    public override Single Volume => Configuration.VoiceActing.Volume / 100f;

    private Single playerPitch;

    private Single playerPanning;

    private Single fadeInDuration;

    private Single fadeOutDuration;

    private Single fadeInTimeRemain;
}
