using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class VoicePlayer : SoundPlayer
{
    private static Dialog specialDialog;
    private static ushort specialLastPlayed;
    private static ushort specialCount = 0;
    private static Dictionary<ushort, ushort> huntTakenCounter;
    private static Dictionary<ushort, ushort> huntHeldCounter;

    public const String SpecialVoicePath = "SpecialVoicePath.txt";
    private static FileSystemWatcher _specialWatcher;
    private struct SpecialLine
    {
        public Int32 fieldId;
        public Int32 messageId;
        public Int32 objectId;
        public String regex;
        public String voicePath;
    }
    private static List<SpecialLine> _specialLines = new List<SpecialLine>();

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

        if (_specialWatcher is null)
        {
            _specialWatcher = new FileSystemWatcher("./", $"*{SpecialVoicePath}");
            _specialWatcher.IncludeSubdirectories = true;
            _specialWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _specialWatcher.Changed += (sender, e) => ParseSpecialLines();
            _specialWatcher.EnableRaisingEvents = true;
            ParseSpecialLines();
        }
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

        String specialAppend = GetSpecialAppend(FieldZoneId, messageNumber);

        // Compile the list of candidate paths for the file name
        List<String> candidates = new List<string>();

        // Try to find one of the special voice lines
        foreach (SpecialLine line in _specialLines)
        {
            if (line.fieldId >= 0 && FieldZoneId != line.fieldId) continue;
            if (line.messageId >= 0 && messageNumber != line.messageId) continue;
            if (line.objectId >= 0 && dialog.Po?.uid != line.objectId) continue;

            Match m = Regex.Match(dialog.Phrase, line.regex);
            if (m.Success)
            {
                candidates.Add(line.voicePath);
            }
        }

        String lang = Localization.GetSymbol();
        String pageIndex = dialog.SubPage.Count > 1 ? $"_{Math.Max(0, dialog.CurrentPage - 1)}" : "";

        // Path for the hunt/hot and cold
        if (specialAppend.Length > 0)
        {
            String path = $"Voices/{lang}/{FieldZoneId}/va_{messageNumber}{specialAppend}";
            if (!AssetManager.HasAssetOnDisc($"Sounds/{path}.akb", true, true) && !AssetManager.HasAssetOnDisc($"Sounds/{path}.ogg", true, false))
            {
                // Cycle back to 0
                if (specialAppend.Contains("_held"))
                {
                    huntHeldCounter[Convert.ToUInt16(messageNumber)] = 1;
                    specialAppend = "_held_0";
                    path = $"Voices/{lang}/{FieldZoneId}/va_{messageNumber}{specialAppend}";
                }
                else if (specialAppend.Contains("_taken"))
                {
                    huntTakenCounter[Convert.ToUInt16(messageNumber)] = 1;
                    specialAppend = "_taken_0";
                    path = $"Voices/{lang}/{FieldZoneId}/va_{messageNumber}{specialAppend}";
                }
            }
            candidates.Add(path);
        }

        // Path using the object id
        if (dialog.Po != null)
            candidates.Add($"Voices/{lang}/{FieldZoneId}/va_{messageNumber}_{dialog.Po.uid}{pageIndex}");

        // Path using the character name at the top of the box
        String[] msgStrings = dialog.Phrase.Split(["[CHOO]"], StringSplitOptions.None);
        String msgString = msgStrings.Length > 0 ? messageOpcodeRegex.Replace(msgStrings[0], (match) => { return ""; }) : "";
        if (msgString.Length > 0 && (
            // Languages have various ways of presenting the name
            msgString.Contains("\n“") || // English
            msgString.Contains("\n「") || // Japanese
            msgString.Contains(":\n") || // German, French
            msgString.Contains("\n─") // Italian, Spanish
            ))
        {
            string name = msgString.Split('\n')[0].Replace(":", "").Trim();
            candidates.Add($"Voices/{lang}/{FieldZoneId}/va_{messageNumber}_{name}{pageIndex}");
        }

        // Default path
        candidates.Add($"Voices/{lang}/{FieldZoneId}/va_{messageNumber}{pageIndex}");

        Boolean hasChoices = dialog.ChoiceNumber > 0;
        Boolean isMsgEmpty = msgString.Length == 0;
        Boolean shouldDismiss = Configuration.VoiceActing.AutoDismissDialogAfterCompletion && !hasChoices;
        Action nonDismissAction = () =>
        {
            if (dialog == specialDialog) specialDialog = null;
            soundOfDialog.Remove(dialog);
        };
        Action dismissAction =
            () =>
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
            };
        Action playSelectChoiceAction =
            () =>
            {
                if (dialog == specialDialog) specialDialog = null;
                soundOfDialog.Remove(dialog);
                while (dialog.CurrentState == Dialog.State.OpenAnimation || dialog.CurrentState == Dialog.State.TextAnimation || !dialog.IsChoiceReady)
                    Thread.Sleep(1000 / Configuration.Graphics.FieldTPS);
                dialog.OnOptionChange?.Invoke(dialog.Id, dialog.SelectChoice); // Simulate a choice change so the default selected line plays
            };

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
                soundOfDialog[dialog] = CreateLoadThenPlayVoice(path.GetHashCode(), path, shouldDismiss ? dismissAction : (hasChoices ? playSelectChoiceAction : nonDismissAction));

                found = true;
                break;
            }
        }
        if (!found)
        {
            candidates.Reverse(); // Reverse for display
            SoundLib.VALog($"field:{FieldZoneId}, msg:{messageNumber}, text:{msgString}, path(s):'{String.Join("', '", candidates.ToArray())}' (not found)");
            isMsgEmpty = true;
        }

        if (hasChoices)
        {
            dialog.OnOptionChange = (Int32 msg, Int32 optionIndex) =>
            {
                if (dialog.CurrentState != Dialog.State.CompleteAnimation || !dialog.IsChoiceReady)
                    return;

                String vaOptionPath = candidates.Last() + "_" + optionIndex;
                String[] options = msgStrings.Length >= 2 ? msgStrings[1].Split('\n') : [];
                Int32 selectedVisibleOption = dialog.ActiveIndexes.Count > 0 ? Math.Max(0, dialog.ActiveIndexes.FindIndex(index => index == optionIndex)) : optionIndex;
                String optString = selectedVisibleOption < options.Length ? messageOpcodeRegex.Replace(options[selectedVisibleOption].Trim(), (match) => { return ""; }) : "[Invalid option index]";

                if (!AssetManager.HasAssetOnDisc($"Sounds/{vaOptionPath}.akb", true, true) && !AssetManager.HasAssetOnDisc($"Sounds/{vaOptionPath}.ogg", true, false))
                {
                    SoundLib.VALog($"field:{FieldZoneId}, msg:{messageNumber}, opt:{optionIndex}, text:{optString} path:{vaOptionPath} (not found)");
                }
                else
                {
                    FieldZoneReleaseVoice(dialog, true);
                    SoundLib.VALog($"field:{FieldZoneId}, msg:{messageNumber}, opt:{optionIndex}, text:{optString} path:{vaOptionPath}");
                    soundOfDialog[dialog] = CreateLoadThenPlayVoice(vaOptionPath.GetHashCode(), vaOptionPath, nonDismissAction);
                }
            };

            if (isMsgEmpty) new Thread(() => { playSelectChoiceAction(); }).Start();
        }
    }

    private static Dictionary<String, Int32[]> specialMessageIds = new Dictionary<String, Int32[]>()
    {
        // Hunt, H&C start, H&C points
        {"English(US)"  , [540, 228, 301]},
        {"English(UK)"  , [540, 228, 301]},
        {"Japanese"     , [560, 227, 306]},
        {"German"       , [560, 228, 307]},
        {"French"       , [550, 228, 307]},
        {"Italian"      , [560, 228, 307]},
        {"Spanish"      , [552, 228, 307]}
    };
    private static String GetSpecialAppend(Int32 FieldZoneId, Int32 messageNumber)
    {
        String specialAppend = "";
        // (special) Festival of the Hunt has take the lead and holds the lead
        switch (FieldZoneId)
        {
            case 22:
                if (FF9StateSystem.EventState.ScenarioCounter == 3180 && huntTakenCounter != null)
                {
                    // clean up memory
                    huntTakenCounter = null;
                    huntHeldCounter = null;
                }
                break;
            case 276:
            {
                // Festival of the hunt
                if (FF9StateSystem.EventState.ScenarioCounter > 3170 && FF9StateSystem.EventState.ScenarioCounter < 3180)
                {
                    Int32[] numbers = specialMessageIds.ContainsKey(Localization.CurrentLanguage) ? specialMessageIds[Localization.CurrentLanguage] : specialMessageIds["English(US)"];
                    // Points message for one of the 8 participants?
                    if (messageNumber >= numbers[0] && messageNumber <= numbers[0] + 7)
                    {
                        if (huntTakenCounter == null || huntHeldCounter == null)
                        {
                            huntTakenCounter = new Dictionary<ushort, ushort>();
                            huntHeldCounter = new Dictionary<ushort, ushort>();
                        }

                        var idShort = Convert.ToUInt16(messageNumber);
                        if (idShort == specialLastPlayed)
                        {
                            if (specialDialog != null) return "";
                            if (!huntHeldCounter.ContainsKey(idShort))
                                huntHeldCounter.Add(idShort, 0);
                            specialAppend = "_held_" + huntHeldCounter[idShort]++;
                        }
                        else
                        {
                            if (!huntTakenCounter.ContainsKey(idShort))
                                huntTakenCounter.Add(idShort, 0);
                            specialAppend = "_taken_" + huntTakenCounter[idShort]++; ;
                            specialLastPlayed = idShort;
                        }
                    }
                }
                break;
            }
            // hot and cold
            case 945:
            {
                Int32[] numbers = specialMessageIds.ContainsKey(Localization.CurrentLanguage) ? specialMessageIds[Localization.CurrentLanguage] : specialMessageIds["English(US)"];
                if (messageNumber >= numbers[1] && messageNumber <= numbers[1] + 3) // Game start
                {
                    specialCount = 0;
                }
                // count up for each time you find something.
                // 
                if (messageNumber == numbers[2]) // gained points
                {
                    specialAppend = "_" + specialCount;
                    specialCount += 1;
                }
                break;
            }
        }
        return specialAppend;
    }

    public static void FieldZoneDialogClosed(Dialog dialog)
    {
        if (huntHeldCounter == null && huntTakenCounter == null)
        {
            FieldZoneReleaseVoice(dialog, Configuration.VoiceActing.StopVoiceWhenDialogDismissed && !dialog.IsClosedByScript);
        }
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

    private void ParseSpecialLines()
    {
        _specialLines.Clear();
        foreach (AssetManager.AssetFolder folder in AssetManager.FolderLowToHigh)
        {
            if (folder.TryFindAssetInModOnDisc(SpecialVoicePath, out String fullPath))
            {
                SoundLib.VALog($"Parsing: '{fullPath}'");
                ParseFile(File.ReadAllText(fullPath));
            }
        }
    }

    private void ParseFile(String text)
    {
        MatchCollection codeMatches = new Regex(@"^(>DialogBoxLine)\b", RegexOptions.Multiline).Matches(text);
        for (Int32 i = 0; i < codeMatches.Count; i++)
        {
            String bvCode = codeMatches[i].Groups[1].Value;
            Int32 endPos, startPos = codeMatches[i].Groups[1].Captures[0].Index + codeMatches[i].Groups[1].Value.Length + 1;
            if (i + 1 == codeMatches.Count)
                endPos = text.Length;
            else
                endPos = codeMatches[i + 1].Groups[1].Captures[0].Index;
            Int32 eolPos = text.IndexOf('\n', startPos);
            if (eolPos < 0)
                continue;
            String charArgFull = text.Substring(startPos, eolPos - startPos);
            startPos += charArgFull.Length + 1;
            String bvArgs = text.Substring(startPos, endPos - startPos);

            Int32 field = -1;
            Match fieldMatch = new Regex(@"\bFieldId:(\d+)").Match(bvArgs);
            if (fieldMatch.Success)
                Int32.TryParse(fieldMatch.Groups[1].Value, out field);

            Int32 textId = -1;
            Match textIdMatch = new Regex(@"\bMessageId:(\d+)").Match(bvArgs);
            if (textIdMatch.Success)
                Int32.TryParse(textIdMatch.Groups[1].Value, out textId);

            Int32 objectId = -1;
            Match objectIdMatch = new Regex(@"\bObjectId:(\d+)").Match(bvArgs);
            if (objectIdMatch.Success)
                Int32.TryParse(textIdMatch.Groups[1].Value, out objectId);

            String regex = null;
            Match regexsMatch = new Regex(@"\bRegex:(.*)\b").Match(bvArgs);
            if (regexsMatch.Success)
            {
                regex = regexsMatch.Groups[1].Value.Trim();
            }
            if (regex == null)
            {
                Log.Warning($"[{nameof(VoicePlayer)}] Expected Regex for the special line {bvCode}");
                continue;
            }

            String path = null;
            Match pathsMatch = new Regex(@"\bVoicePath:(.*)\b").Match(bvArgs);
            if (pathsMatch.Success)
            {
                path = pathsMatch.Groups[1].Value.Trim();
            }
            if (path == null)
            {
                Log.Warning($"[{nameof(VoicePlayer)}] Expected VoicePath for the special line {bvCode}");
                continue;
            }

            _specialLines.Add(new SpecialLine
            {
                fieldId = field,
                messageId = textId,
                objectId = objectId,
                regex = regex,
                voicePath = path
            });
            //Log.Message($"[DEBUG] Special line: {field} {textId} {regex} {path}");
        }
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
