using Assets.Sources.Scripts.UI.Common;
using Global.Sound.SaXAudio;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class VoicePlayer : SoundPlayer
{
    private static Dialog specialDialog;
    private static Int32 specialLastPlayed;
    private static Int32 specialCount = 0;

    public static Boolean closeDialogOnFinish = false;

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

        if (soundOfDialog.TryGetValue(dialog, out SoundProfile attachedVoice))
            return ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(attachedVoice.SoundID) == 1;
        return false;
    }

    public static Boolean RegisterDialogVoice(Dialog dialog, SoundProfile soundProfile, Boolean update = false)
    {
        if (!Configuration.VoiceActing.Enabled || dialog == null || soundProfile == null || (!update && soundOfDialog.ContainsKey(dialog)))
            return false;
        soundOfDialog[dialog] = soundProfile;
        return true;
    }

    public void StartSound(SoundProfile soundProfile, Action onFinished = null)
    {
        if (onFinished != null)
        {
            if (ISdLibAPIProxy.Instance is SdLibAPIWithSaXAudio)
            {
                SaXAudio.OnFinishedDelegate handler = null;
                handler = (soundID) =>
                {
                    if (!watcherOfSound.ContainsKey(soundProfile))
                    {
                        SaXAudio.OnVoiceFinished -= handler;
                        return;
                    }
                    if (soundProfile.SoundID == soundID)
                    {
                        SaXAudio.OnVoiceFinished -= handler;
                        watcherOfSound.Remove(soundProfile);
                        onFinished();
                    }
                };
                watcherOfSound[soundProfile] = null;
                SaXAudio.OnVoiceFinished += handler;
            }
            else
            {
                Thread onFinishThread = new Thread(() =>
                {
                    try
                    {
                        // we need to delay if we run it instantly we're at 0 = 0 which is useless
                        Thread.Sleep(500);
                        while (true)
                        {
                            Int32 currentTime = ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_GetElapsedPlaybackTime(soundProfile.SoundID);
                            if (currentTime == 0)
                            {
                                onFinished();
                                break;
                            }
                            Thread.Sleep(50);
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

        // Compile the list of candidate paths for the file name
        List<String> candidates = new List<String>();
        String lang = Localization.CurrentSymbol;
        String pageIndex = dialog.SubPage.Count > 1 ? $"_P{Math.Max(0, dialog.CurrentPage - 1)}" : "";

        // Path for the hunt/hot and cold
        String specialAppend = GetSpecialAppend(FieldZoneId, messageNumber);
        if (specialAppend.Length > 0)
            candidates.Add($"Voices/{lang}/{FieldZoneId}/va_{messageNumber}{specialAppend}");

        // Path using the object id
        if (dialog.Po != null)
            candidates.Add($"Voices/{lang}/{FieldZoneId}/va_{messageNumber}_ID{dialog.Po.uid}{pageIndex}");

        // Path using the character name at the top of the box
        String[] msgStrings = dialog.ChoicePhrases;
        String msgString = msgStrings[0];
        if (msgString.Length > 0 && (
            // Languages have various ways of presenting the name
            msgString.Contains("\n“") || // English
            msgString.Contains("\n「") || // Japanese
            msgString.Contains(":\n") || // German, French
            msgString.Contains("\n─") // Italian, Spanish
            ))
        {
            String name = msgString.Split('\n')[0].Replace(":", "").Trim();
            candidates.Add($"Voices/{lang}/{FieldZoneId}/va_{messageNumber}_{name}{pageIndex}");
        }

        // Default path
        candidates.Add($"Voices/{lang}/{FieldZoneId}/va_{messageNumber}{pageIndex}");

        // Try to find one of the candidates
        Boolean found = false;
        foreach (String path in candidates)
        {
            if (AssetManager.HasAssetOnDisc($"Sounds/{path}.akb", true, true) || AssetManager.HasAssetOnDisc($"Sounds/{path}.ogg", true, false))
            {
                SoundLib.VALog($"field:{FieldZoneId}, msg:{messageNumber}, text:{msgString}, path:{path}");
                // Special dialog
                if (path.Contains("_taken") || path.Contains("_held"))
                {
                    if (specialDialog != null) FieldZoneReleaseVoice(specialDialog, true);
                    specialDialog = dialog;
                }
                soundOfDialog[dialog] = CreateLoadThenPlayVoice(path.GetHashCode(), path, () => AfterSoundFinished(dialog));

                found = true;
                break;
            }
        }

        Boolean hasChoices = dialog.ChoiceNumber > 0;
        Boolean isMsgEmpty = msgString.Length == 0;
        if (!found)
        {
            SoundLib.VALog($"field:{FieldZoneId}, msg:{messageNumber}, text:{msgString}, path(s):'{String.Join("', '", candidates.ToArray().Reverse().ToArray())}' (not found)");
            isMsgEmpty = true;
        }

        if (hasChoices)
        {
            soundOfDialog.TryGetValue(dialog, out SoundProfile dialogProfile);
            dialog.OnOptionChange = (Int32 msg, Int32 optionIndex) =>
            {
                if (dialog.CurrentState != Dialog.State.CompleteAnimation || !dialog.IsChoiceReady)
                    return;

                // We don't want to interrupt the main dialog voice line
                soundOfDialog.TryGetValue(dialog, out SoundProfile attachedVoice);
                if (attachedVoice != null && attachedVoice == dialogProfile && ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(attachedVoice.SoundID) == 1)
                    return;

                Boolean found = false;
                List<String> choiceCandidates = [];
                Int32 selectedVisibleOption = dialog.ActiveIndexes.Count > 0 ? Math.Max(0, dialog.ActiveIndexes.FindIndex(index => index == optionIndex)) : optionIndex;
                String optString = selectedVisibleOption + 1 < msgStrings.Length ? msgStrings[selectedVisibleOption + 1].Trim() : "[Invalid option index]";
                foreach (String path in candidates)
                {
                    String vaOptionPathMain = path + "_" + optionIndex;
                    choiceCandidates.Add(vaOptionPathMain);

                    if (AssetManager.HasAssetOnDisc($"Sounds/{vaOptionPathMain}.akb", true, true) || AssetManager.HasAssetOnDisc($"Sounds/{vaOptionPathMain}.ogg", true, false))
                    {
                        FieldZoneReleaseVoice(dialog, true);
                        SoundLib.VALog($"field:{FieldZoneId}, msg:{messageNumber}, opt:{optionIndex}, text:{optString} path:{vaOptionPathMain}");
                        soundOfDialog[dialog] = CreateLoadThenPlayVoice(vaOptionPathMain.GetHashCode(), vaOptionPathMain, () => AfterSoundFinished_Default(dialog));

                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    if (closeDialogOnFinish)
                    {
                        dialog.OnKeyConfirm(null);
                        closeDialogOnFinish = false;
                    }
                    SoundLib.VALog($"field:{FieldZoneId}, msg:{messageNumber}, text:{optString}, path(s):'{String.Join("', '", choiceCandidates.ToArray().Reverse().ToArray())}' (not found)");
                }
            };

            if (isMsgEmpty) new Thread(() => AfterSoundFinished_SelectChoice(dialog)).Start();
        }
    }

    private static void AfterSoundFinished_SelectChoice(Dialog dialog)
    {
        if (dialog == specialDialog) specialDialog = null;
        soundOfDialog.Remove(dialog);
        while (dialog.CurrentState == Dialog.State.OpenAnimation || dialog.CurrentState == Dialog.State.TextAnimation || !dialog.IsChoiceReady)
            Thread.Sleep(1000 / Configuration.Graphics.FieldTPS);
        dialog.OnOptionChange?.Invoke(dialog.Id, dialog.SelectChoice); // Simulate a choice change so the default selected line plays
    }

    private static void AfterSoundFinished_Dismiss(Dialog dialog)
    {
        if (dialog == specialDialog) specialDialog = null;
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
            ETb.sKey &= ~EventInput.GetKeyMaskFromControl(Control.Confirm);
            EventInput.ReceiveInput(EventInput.GetKeyMaskFromControl(Control.Confirm));
        }
    }

    private static void AfterSoundFinished_Default(Dialog dialog)
    {
        if (dialog == specialDialog) specialDialog = null;
        soundOfDialog.Remove(dialog);

        if (closeDialogOnFinish)
        {
            dialog.OnKeyConfirm(null);
            closeDialogOnFinish = false;
        }
    }

    private static void AfterSoundFinished_Battle(Int32 va_id, String text)
    {
        try
        {
            BattleVoice.InvokeOnBattleDialogAudioEnd(va_id, text);
        }
        catch (Exception e)
        {
            Log.Error($"[VoiceActing] Error while running BattleScript.OnBattleDialogAudioEnd");
            Log.Error(e);
        }
    }

    public static void AfterSoundFinished(Dialog dialog)
    {
        if (dialog.ChoiceNumber > 0)
            AfterSoundFinished_SelectChoice(dialog);
        else if (Configuration.VoiceActing.AutoDismissDialogAfterCompletion || dialog.CloseAfterVoiceActing)
            AfterSoundFinished_Dismiss(dialog);
        else
            AfterSoundFinished_Default(dialog);
    }

    private static Dictionary<String, Int32[]> specialMessageIds = new Dictionary<String, Int32[]>()
    {
        // Hunt, H&C start, H&C points
        {"US", [540, 228, 301]},
        {"UK", [540, 228, 301]},
        {"JP", [560, 227, 306]},
        {"GR", [560, 228, 307]},
        {"FR", [550, 228, 307]},
        {"IT", [560, 228, 307]},
        {"ES", [552, 228, 307]}
    };
    private static String GetSpecialAppend(Int32 FieldZoneId, Int32 messageNumber)
    {
        String specialAppend = "";
        // (special) Festival of the Hunt has take the lead and holds the lead
        switch (FieldZoneId)
        {
            case 276:
            {
                // Festival of the hunt
                if (FF9StateSystem.EventState.ScenarioCounter > 3170 && FF9StateSystem.EventState.ScenarioCounter < 3180 && specialMessageIds.TryGetValue(Localization.CurrentSymbol, out Int32[] numbers))
                {
                    // Points message for one of the 8 participants
                    if (messageNumber >= numbers[0] && messageNumber <= numbers[0] + 7)
                    {
                        if (messageNumber == specialLastPlayed)
                        {
                            if (specialDialog != null) return "";
                            // Use Var_GenUInt8_510 ... Var_GenUInt8_517
                            Int32 varAddr = 510 + messageNumber - numbers[0];
                            Byte varValue = FF9StateSystem.EventState.gEventGlobal[varAddr];
                            specialAppend = "_held_" + varValue;
                            if (varValue < Byte.MaxValue)
                                FF9StateSystem.EventState.gEventGlobal[varAddr]++;
                        }
                        else
                        {
                            // Use Var_GenUInt8_518 ... Var_GenUInt8_525
                            Int32 varAddr = 518 + messageNumber - numbers[0];
                            Byte varValue = FF9StateSystem.EventState.gEventGlobal[varAddr];
                            specialAppend = "_taken_" + varValue;
                            if (varValue < Byte.MaxValue)
                                FF9StateSystem.EventState.gEventGlobal[varAddr]++;
                            specialLastPlayed = messageNumber;
                        }
                    }
                }
                break;
            }
            // hot and cold
            case 945:
            {
                if (specialMessageIds.TryGetValue(Localization.CurrentSymbol, out Int32[] numbers))
                {
                    if (messageNumber >= numbers[1] && messageNumber <= numbers[1] + 3) // Game start
                        specialCount = 0;
                    // count up for each time you find something.
                    if (messageNumber == numbers[2]) // gained points
                        specialAppend = "_" + specialCount++;
                }
                break;
            }
        }
        return specialAppend;
    }

    public static void FieldZoneDialogClosed(Dialog dialog)
    {
        if (FF9StateSystem.EventState.ScenarioCounter != 3175) // 3175 = Festival of the Hunt
            FieldZoneReleaseVoice(dialog, Configuration.VoiceActing.StopVoiceWhenDialogDismissed && !dialog.IsClosedByScript);
    }

    public static void FieldZoneReleaseVoice(Dialog dialog, Boolean stopSound)
    {
        if (soundOfDialog.TryGetValue(dialog, out SoundProfile attachedVoice))
        {
            if (stopSound && ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(attachedVoice.SoundID) == 1)
                SoundLib.VoicePlayer.StopSound(attachedVoice);
            if (watcherOfSound.TryGetValue(attachedVoice, out Thread soundWatcher))
            {
                soundWatcher?.Interrupt();
                watcherOfSound.Remove(attachedVoice);
            }
            soundOfDialog.Remove(dialog);
        }
    }

    public static SoundProfile CreateLoadThenPlayVoice(Int32 soundIndex, String vaPath, Action onFinished = null)
    {
        // Occasionally clear unused voices from the database
        if (ETb.voiceDatabase.ReadAll().Count > 10)
        {
            List<SoundProfile> toDelete = new List<SoundProfile>();
            foreach (SoundProfile profile in ETb.voiceDatabase.ReadAll().Values)
            {
                if (profile.SoundIndex == soundIndex) continue;

                Boolean isUsed = AudioEffectManager.IsSaXAudio ? SaXAudio.GetVoiceCount(profile.BankID) > 0 : ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(profile.SoundID) > 0;
                if (!isUsed)
                {
                    StaticUnregisterBank(profile);
                    toDelete.Add(profile);
                }
            }
            foreach (SoundProfile profile in toDelete)
            {
                ETb.voiceDatabase.Delete(profile);
            }
        }

        SoundProfile soundProfile = ETb.voiceDatabase.Read(soundIndex);
        if (soundProfile == null)
        {
            soundProfile = new SoundProfile
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
        }
        else
        {
            SoundLib.VoicePlayer.CreateSound(soundProfile);
            SoundLib.VoicePlayer.StartSound(soundProfile, onFinished);
        }

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

        String vaPath = String.Format("Voices/{0}/battle/{2}/va_{1}", Localization.CurrentSymbol, va_id, btlFolder).ToLower();
        if (!AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".akb", true, true) && !AssetManager.HasAssetOnDisc("Sounds/" + vaPath + ".ogg", true, false))
        {
            SoundLib.VALog(String.Format("field:battle/{0}, msg:{1}, text:{2} path:{3} (not found)", btlFolder, va_id, text, vaPath));
            return;
        }

        SoundLib.VALog(String.Format("field:battle/{0}, msg:{1}, text:{2} path:{3}", btlFolder, va_id, text, vaPath));

        CreateLoadThenPlayVoice(va_id, vaPath, () => AfterSoundFinished_Battle(va_id, text));

        try
        {
            BattleVoice.InvokeOnBattleDialogAudioStart(va_id, text);
        }
        catch (Exception e)
        {
            Log.Error($"[VoiceActing] Error while running BattleScript.OnBattleDialogAudioStart");
            Log.Error(e);
        }
    }

    public static Boolean HoldDialogUntilSoundEnds(Int32 zoneId, Int32 universalTextId, Int32 mapNo)
    {
        if (zoneId == 2 && mapNo >= 59 && mapNo <= 67) // 'I want to be your canary' stage (early game)
            return true;
        if (zoneId == 187) // Ending scene (Vivi monologue and stage afterwards)
            return true;
        if (zoneId == 189 && (universalTextId == 137 || universalTextId == 138)) // Cid and Baku come at the rescue against Silverdragons ("All ships, clear a path for the Invincible!" and "Can't let you guys steal the show by yourselves!")
            return true;
        if (zoneId == 89 && universalTextId >= 132 && universalTextId <= 135) // Alexander summoning chant ("O holy guardian, hear our prayers." etc)
            return true;
        if (zoneId == 484 && universalTextId == 165) // Mount Gulug: Kuja meets the party after Eiko defeated Zorn & Thorn ("How can that-")
            return true;
        return false;
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
