using FF9;
using Memoria.Assets;
using Memoria.Prime;
using Memoria.Scripts;
using NCalc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Memoria.Data
{
    public static class BattleVoice
    {
        public const String BattleVoicePath = "BattleVoiceEffects.txt";

        //--- Events for scripting
        public static readonly IOverloadVABattleScript BattleScript = ScriptsLoader.GetOverloadedMethod(typeof(IOverloadVABattleScript)) as IOverloadVABattleScript;

        public delegate Boolean BattleInOutDelegate(BattleMoment when);
        public delegate Boolean BattleActDelegate(BattleUnit unit, BattleCalculator calc, BattleMoment when);
        public delegate Boolean StatusChangeDelegate(BattleUnit unit, BattleCalculator calc, BattleStatusId status, BattleMoment when);
        public delegate void BattleDialogDelegate(Int32 voiceId, String text);

        public static event BattleInOutDelegate OnBattleInOut;
        public static event BattleActDelegate OnAct;
        public static event BattleActDelegate OnHit;
        public static event StatusChangeDelegate OnStatusChange;
        public static event BattleDialogDelegate OnDialogAudioStart;
        public static event BattleDialogDelegate OnDialogAudioEnd;

        public static void InvokeOnBattleDialogAudioStart(Int32 voiceId, String text) => OnDialogAudioStart.Invoke(voiceId, text);
        public static void InvokeOnBattleDialogAudioEnd(Int32 voiceId, String text) => OnDialogAudioEnd.Invoke(voiceId, text);
        //---

        static BattleVoice()
        {
            if (!Configuration.VoiceActing.Enabled)
                return;

            FileSystemWatcher watcher = new FileSystemWatcher("./", $"*{BattleVoicePath}");
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += (sender, e) =>
            {
                if (e.ChangeType != WatcherChangeTypes.Changed) return;
                SoundLib.VALog($"File changed: '{e.FullPath}'");
                isDirty = true;
            };
            watcher.EnableRaisingEvents = true;

            LoadEffects();
            try
            {
                BattleScript?.Initialize();
            }
            catch (Exception ex)
            {
                Log.Error($"[VoiceActing] Error while running BattleScript.Initialize");
                Log.Error(ex);
            }
        }

        public static void Initialize() { }

        public static void InitBattle()
        {
            _currentVoicePlay.Clear();
        }

        public enum BattleMoment
        {
            Unknown,
            // BattleInOut
            BattleStart, GameOver, Defeated, VictoryPose, Victory, Flee, BattleInterrupted, EnemyEscape,
            // BattleAct
            CommandPerform, CommandInput, HitEffect, Cover,
            // Hitted
            Damaged, Healed, Ability, Dodged, Missed,
            // BattleStatusChange
            Added, Removed, Used
        }

        public class BattleSpeaker
        {
            public CharacterId playerId = CharacterId.NONE;
            public Int32 enemyModelId = -1;
            public Int32 enemyBattleId = -1;

            public BattleSpeaker() { }

            //--- Helping methods for scripting
            public BattleSpeaker(BattleUnit unit)
            {
                if (unit.IsPlayer)
                {
                    playerId = unit.PlayerIndex;
                }
                else
                {
                    enemyModelId = unit.Data.dms_geo_id;
                    enemyBattleId = FF9StateSystem.Battle.battleMapIndex;
                }
            }

            public Boolean CanSpeak(Int32 voicePriority = 0, BattleStatusId statusException = BattleStatusId.None)
            {
                FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
                for (Int32 i = 0; i < 8; i++)
                    if (CheckIsCharacter(ff9Battle.btl_data[i]))
                        return CheckCanSpeak(ff9Battle.btl_data[i], voicePriority, statusException);
                return false;
            }

            public Boolean Equals(BattleSpeaker speaker)
            {
                return speaker.playerId == playerId && speaker.enemyModelId == enemyModelId && speaker.enemyBattleId == enemyBattleId;
            }
            //---

            public Boolean CheckIsCharacter(BTL_DATA btl)
            {
                if (btl.bi.player != 0)
                    return playerId != CharacterId.NONE && (CharacterId)btl.bi.slot_no == playerId;
                if (playerId != CharacterId.NONE)
                    return false;
                return (enemyModelId < 0 || enemyModelId == btl.dms_geo_id) && (enemyBattleId < 0 || enemyBattleId == FF9StateSystem.Battle.battleMapIndex);
            }

            public BattleUnit FindBattleUnit()
            {
                FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
                for (Int32 i = 0; i < 8; i++)
                    if (CheckIsCharacter(ff9Battle.btl_data[i]))
                        return new BattleUnit(ff9Battle.btl_data[i]);
                return null;
            }

            public static Boolean CheckCanSpeak(BTL_DATA btl, Int32 voicePriority, BattleStatusId statusException = BattleStatusId.None)
            {
                if (btl.bi.disappear != 0 && (statusException != BattleStatusId.Jump || !btl_stat.CheckStatus(btl, BattleStatus.Jump)))
                    return false;
                if (btl_stat.CheckStatus(btl, BattleStatusConst.CannotSpeak & ~statusException.ToBattleStatus()))
                    return false;
                KeyValuePair<Int32, SoundProfile> playingVoice;
                if (_currentVoicePlay.TryGetValue(btl, out playingVoice))
                    return voicePriority > playingVoice.Key;
                return true;
            }
        }

        private class GenericVoiceEffect
        {
            public List<BattleSpeaker> Speakers = new List<BattleSpeaker>();
            public String Condition = "";
            public String[] AudioPaths;
            public Int32 Priority = 0;
            public Int32 lastPlayed = -1;
            public Int32 ConditionLine;

            public Boolean CheckSpeakerAll(BTL_DATA statusExceptionBtl = null, BattleStatusId statusException = BattleStatusId.None)
            {
                foreach (BattleSpeaker speaker in Speakers)
                {
                    BTL_DATA btl = null;
                    for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
                    {
                        if (speaker.CheckIsCharacter(next))
                        {
                            btl = next;
                            break;
                        }
                    }
                    if (btl == null)
                        return false;
                    if (!BattleSpeaker.CheckCanSpeak(btl, Priority, (statusExceptionBtl == null || statusExceptionBtl == btl) ? statusException : BattleStatusId.None))
                        return false;
                }
                return true;
            }

            public Boolean CheckIsFirstSpeaker(BTL_DATA btl, BattleStatusId statusException = BattleStatusId.None)
            {
                if (Speakers.Count == 0)
                    return false;
                return Speakers[0].CheckIsCharacter(btl) && BattleSpeaker.CheckCanSpeak(new BattleUnit(btl), Priority, statusException);
            }
        }

        private class BattleInOut : GenericVoiceEffect
        {
            public BattleMoment When = BattleMoment.BattleStart; // "GameOver", "Defeated", "VictoryPose", "Victory", "Flee", "BattleInterrupted", "EnemyEscape"
        }
        private class BattleAct : GenericVoiceEffect
        {
            // SomeoneElse: unused option for now
            public Boolean SomeoneElse = false;
            public BattleMoment When = BattleMoment.CommandPerform; // "CommandInput", "HitEffect", "Cover"
        }
        private class BattleHitted : GenericVoiceEffect
        {
            public Boolean SomeoneElse = false;
            public BattleMoment When = BattleMoment.Ability; // "Damaged", "Healed", "Ability", "Dodged", "Missed"
        }
        private class BattleStatusChange : GenericVoiceEffect
        {
            public Boolean SomeoneElse = false;
            public BattleMoment When = BattleMoment.Added; // "Removed", "Used"
            public BattleStatusId Status = 0;
        }

        private static List<BattleInOut> InOutEffect = new List<BattleInOut>();
        private static List<BattleAct> ActEffect = new List<BattleAct>();
        private static List<BattleHitted> HittedEffect = new List<BattleHitted>();
        private static List<BattleStatusChange> StatusChangeEffect = new List<BattleStatusChange>();

        private static Dictionary<BTL_DATA, KeyValuePair<Int32, SoundProfile>> _currentVoicePlay = new Dictionary<BTL_DATA, KeyValuePair<Int32, SoundProfile>>();

        private static Boolean isDirty = true;

        public static CharacterId VictoryFocusIndex => SFX.lastPlayedExeId != 0 && SFX.lastPlayedExeId < 16 ? btl_scrp.FindBattleUnit(SFX.lastPlayedExeId)?.PlayerIndex ?? CharacterId.NONE : CharacterId.NONE;

        public static BattleCalculator CurrentCalc;

        private static void LoadEffects()
        {
            isDirty = false;

            InOutEffect.Clear();
            ActEffect.Clear();
            HittedEffect.Clear();
            StatusChangeEffect.Clear();

            foreach (AssetManager.AssetFolder folder in AssetManager.FolderLowToHigh)
            {
                if (folder.TryFindAssetInModOnDisc(BattleVoicePath, out String fullPath))
                {
                    SoundLib.VALog($"Parsing: '{fullPath}'");
                    ParseEffect(File.ReadAllText(fullPath));
                }
            }
        }

        //--- Helping methods for scripting
        public static Int32 PlayVoice(BattleUnit speaker, String audioPath, Int32 priority = 0, Action onFinished = null)
        {
            return PlayVoice([speaker.Data], audioPath, priority, onFinished: onFinished);
        }

        public static void StopVoice(BattleUnit unit)
        {
            KeyValuePair<Int32, SoundProfile> playingVoice;
            if (_currentVoicePlay.TryGetValue(unit.Data, out playingVoice))
            {
                SoundLib.VoicePlayer.StopSound(playingVoice.Value);
                _currentVoicePlay.Remove(unit.Data);
            }
        }

        public static void StopAllVoices()
        {
            foreach (KeyValuePair<Int32, SoundProfile> playingVoice in _currentVoicePlay.Values)
                SoundLib.VoicePlayer.StopSound(playingVoice.Value);
            _currentVoicePlay.Clear();
        }

        public static Int32 GetPlayingVoicesCount()
        {
            return _currentVoicePlay.Count;
        }
        //---

        private static Int32 PlayVoice(List<BTL_DATA> speakerBtlList, String audioPath, Int32 priority = 0, String type = "Script", Action onFinished = null)
        {
            String soundPath = $"Voices/{Localization.CurrentSymbol}/{audioPath}";
            Boolean soundExists = AssetManager.HasAssetOnDisc("Sounds/" + soundPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + soundPath + ".ogg", true, false);
            SoundLib.VALog($"battlevoice:{type} character:{(speakerBtlList.Count > 0 ? new BattleUnit(speakerBtlList[0]).Name : "no speaker")} path:{soundPath}" + (soundExists ? "" : " (not found)"));
            if (!soundExists)
            {
                onFinished?.Invoke();
                return -1;
            }

            SoundProfile audioProfile = VoicePlayer.CreateLoadThenPlayVoice(soundPath.GetHashCode(), soundPath,
            () =>
            {
                foreach (BTL_DATA btl in speakerBtlList)
                    _currentVoicePlay.Remove(btl);
                onFinished?.Invoke();
            });
            KeyValuePair<Int32, SoundProfile> playingVoice;
            foreach (BTL_DATA btl in speakerBtlList)
            {
                if (_currentVoicePlay.TryGetValue(btl, out playingVoice))
                    SoundLib.VoicePlayer.StopSound(playingVoice.Value);
                _currentVoicePlay[btl] = new KeyValuePair<Int32, SoundProfile>(priority, audioProfile);
            }
            return audioProfile.SoundID;
        }

        private static void PlayVoiceEffect(GenericVoiceEffect voiceEffect)
        {
            List<BTL_DATA> speakerBtlList = new List<BTL_DATA>();
            foreach (BattleSpeaker speaker in voiceEffect.Speakers)
            {
                BTL_DATA btl = speaker.FindBattleUnit();
                if (btl != null)
                    speakerBtlList.Add(btl);
            }

            Int32 audioIndex = 0;
            if (voiceEffect.AudioPaths.Length > 1)
            {
                // Pick a random audio excluding the last one that was played in that situation
                if (voiceEffect.lastPlayed >= 0 && voiceEffect.lastPlayed < voiceEffect.AudioPaths.Length)
                {
                    audioIndex = UnityEngine.Random.Range(0, voiceEffect.AudioPaths.Length - 1);
                    if (audioIndex >= voiceEffect.lastPlayed)
                        audioIndex++;
                }
                else
                {
                    audioIndex = UnityEngine.Random.Range(0, voiceEffect.AudioPaths.Length);
                }
            }
            voiceEffect.lastPlayed = audioIndex;

            PlayVoice(speakerBtlList, voiceEffect.AudioPaths[audioIndex], voiceEffect.Priority, voiceEffect.GetType().ToString());
        }

        public static void TriggerOnBattleInOut(BattleMoment when)
        {
            if (!Configuration.VoiceActing.Enabled)
                return;
            try
            {
                if (OnBattleInOut?.Invoke(when) ?? false)
                    return;
            }
            catch (Exception e)
            {
                Log.Error($"[VoiceActing] Error while running script event OnBattleInOut");
                Log.Error(e);
            }

            if (isDirty) LoadEffects();

            List<BattleInOut> retainedEffects = new List<BattleInOut>();
            Int32 retainedPriority = Int32.MinValue;
            BattleStatusId statusException = (when == BattleMoment.GameOver || when == BattleMoment.Defeated) ? BattleStatusId.Death : BattleStatusId.None;
            foreach (BattleInOut effect in InOutEffect)
            {
                if (effect.When != when || effect.Priority < retainedPriority || !effect.CheckSpeakerAll(null, statusException))
                    continue;
                if (!String.IsNullOrEmpty(effect.Condition))
                {
                    Expression c = new Expression(effect.Condition);
                    BattleUnit unit = effect.Speakers[0].FindBattleUnit();
                    NCalcUtility.InitializeExpressionUnit(ref c, unit);
                    c.Parameters["VictoryFocusIndex"] = (UInt32)VictoryFocusIndex;
                    c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                    c.EvaluateParameter += NCalcUtility.commonNCalcParameters;

                    try
                    {
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
                    }
                    catch (Exception err)
                    {
                        Log.Error($"[VoiceActing] Couldn't evaluate condition: '{effect.Condition.Trim()}' at line {effect.ConditionLine}");
                        Log.Error(err);
                        continue;
                    }
                }
                if (effect.Priority > retainedPriority)
                {
                    retainedEffects.Clear();
                    retainedPriority = effect.Priority;
                }
                retainedEffects.Add(effect);
            }
            if (retainedEffects.Count == 0)
                return;
            PlayVoiceEffect(retainedEffects[UnityEngine.Random.Range(0, retainedEffects.Count)]);
        }

        public static void TriggerOnBattleAct(BTL_DATA actingChar, BattleMoment when, CMD_DATA cmdUsed, BattleCalculator calc = null)
        {
            if (!Configuration.VoiceActing.Enabled)
                return;

            if (BattleScript != null)
            {
                BattleCalculator v = calc;
                if (v is null)
                {
                    BTL_DATA target = null;
                    BattleCommand command = new BattleCommand(cmdUsed);
                    if (Comn.countBits(command.Data.tar_id) == 1) target = btl_scrp.FindBattleUnit(command.Data.tar_id) ?? null;
                    if (target == null) target = actingChar; // Target cannot be null, we use the actingChar when no target is valid (i.e multi-target)
                    v = new BattleCalculator(actingChar, target, command);
                }
                try
                {
                    if (OnAct?.Invoke(new BattleUnit(actingChar), v, when) ?? false)
                        return;
                }
                catch (Exception e)
                {
                    Log.Error($"[VoiceActing] Error while running script event OnBattleAct");
                    Log.Error(e);
                }
            }
            if (isDirty) LoadEffects();

            List<BattleAct> retainedEffects = new List<BattleAct>();
            Int32 retainedPriority = Int32.MinValue;
            foreach (BattleAct effect in ActEffect)
            {
                if (effect.When != when || effect.Priority < retainedPriority || !effect.CheckSpeakerAll() || !effect.CheckIsFirstSpeaker(actingChar))
                    continue;
                if (!String.IsNullOrEmpty(effect.Condition))
                {
                    Expression c = new Expression(effect.Condition);
                    BattleUnit unit = new BattleUnit(actingChar);
                    BattleCommand cmd = new BattleCommand(cmdUsed);
                    NCalcUtility.InitializeExpressionUnit(ref c, unit);
                    List<BTL_DATA> t = FF9.btl_util.findAllBtlData(cmdUsed.tar_id);
                    if (t.Count > 0)
                    {
                        BattleUnit target = new BattleUnit(t[0]);
                        NCalcUtility.InitializeExpressionUnit(ref c, target, "Target");
                    }
                    else
                    {
                        NCalcUtility.InitializeExpressionNullableUnit(ref c, null, "Target");
                    }
                    NCalcUtility.InitializeExpressionCommand(ref c, cmd);
                    if (calc != null) // Should be the case only when "HitEffect"
                        NCalcUtility.InitializeExpressionAbilityContext(ref c, calc);
                    c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                    c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                    try
                    {
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
                    }
                    catch (Exception err)
                    {
                        Log.Error($"[VoiceActing] Couldn't evaluate condition: '{effect.Condition.Trim()}' at line {effect.ConditionLine}");
                        Log.Error(err);
                        continue;
                    }
                }
                if (effect.Priority > retainedPriority)
                {
                    retainedEffects.Clear();
                    retainedPriority = effect.Priority;
                }
                retainedEffects.Add(effect);
            }
            if (retainedEffects.Count == 0)
                return;
            PlayVoiceEffect(retainedEffects[UnityEngine.Random.Range(0, retainedEffects.Count)]);
        }

        public static void TriggerOnHitted(BTL_DATA hittedChar, BattleMoment when, BattleCalculator calc)
        {
            if (!Configuration.VoiceActing.Enabled)
                return;

            try
            {
                if (OnHit?.Invoke(new BattleUnit(hittedChar), calc, when) ?? false)
                    return;
            }
            catch (Exception e)
            {
                Log.Error($"[VoiceActing] Error while running script event OnHit");
                Log.Error(e);
            }

            if (isDirty) LoadEffects();

            List<BattleHitted> retainedEffects = new List<BattleHitted>();
            Int32 retainedPriority = Int32.MinValue;
            foreach (BattleHitted effect in HittedEffect)
            {
                if (effect.When != when || effect.Priority < retainedPriority || !effect.CheckSpeakerAll() || !effect.CheckIsFirstSpeaker(hittedChar))
                    continue;
                if (!String.IsNullOrEmpty(effect.Condition))
                {
                    Expression c = new Expression(effect.Condition);
                    BattleUnit unit = new BattleUnit(hittedChar);
                    NCalcUtility.InitializeExpressionUnit(ref c, unit);
                    NCalcUtility.InitializeExpressionUnit(ref c, calc.Caster, "Caster");
                    NCalcUtility.InitializeExpressionCommand(ref c, calc.Command);
                    NCalcUtility.InitializeExpressionAbilityContext(ref c, calc);
                    c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                    c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                    try
                    {
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
                    }
                    catch (Exception err)
                    {
                        Log.Error($"[VoiceActing] Couldn't evaluate condition: '{effect.Condition.Trim()}' at line {effect.ConditionLine}");
                        Log.Error(err);
                        continue;
                    }
                }
                if (effect.Priority > retainedPriority)
                {
                    retainedEffects.Clear();
                    retainedPriority = effect.Priority;
                }
                retainedEffects.Add(effect);
            }
            if (retainedEffects.Count == 0)
                return;
            PlayVoiceEffect(retainedEffects[UnityEngine.Random.Range(0, retainedEffects.Count)]);
        }

        public static void TriggerOnStatusChange(BTL_DATA statusedChar, BattleMoment when, BattleStatusId whichStatus)
        {
            if (!Configuration.VoiceActing.Enabled)
                return;

            try
            {
                if (OnStatusChange?.Invoke(new BattleUnit(statusedChar), CurrentCalc, whichStatus, when) ?? false)
                    return;
            }
            catch (Exception e)
            {
                Log.Error($"[VoiceActing] Error while running script event OnStatusChange");
                Log.Error(e);
            }

            if (isDirty) LoadEffects();

            List<BattleStatusChange> retainedEffects = new List<BattleStatusChange>();
            Int32 retainedPriority = Int32.MinValue;
            Boolean discardStatusChecks = when != BattleMoment.Removed;
            foreach (BattleStatusChange effect in StatusChangeEffect)
            {
                if (whichStatus != effect.Status || effect.When != when || effect.Priority < retainedPriority || !effect.CheckSpeakerAll(statusedChar, effect.Status))
                    continue;
                if (discardStatusChecks && !effect.CheckIsFirstSpeaker(statusedChar, effect.Status))
                    continue;
                if (!discardStatusChecks && !effect.CheckIsFirstSpeaker(statusedChar))
                    continue;
                if (!String.IsNullOrEmpty(effect.Condition))
                {
                    Expression c = new Expression(effect.Condition);
                    BattleUnit unit = new BattleUnit(statusedChar);
                    NCalcUtility.InitializeExpressionUnit(ref c, unit);
                    c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                    c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                    try
                    {
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
                    }
                    catch (Exception err)
                    {
                        Log.Error($"[VoiceActing] Couldn't evaluate condition: '{effect.Condition.Trim()}' at line {effect.ConditionLine}");
                        Log.Error(err);
                        continue;
                    }
                }
                if (effect.Priority > retainedPriority)
                {
                    retainedEffects.Clear();
                    retainedPriority = effect.Priority;
                }
                retainedEffects.Add(effect);
            }
            if (retainedEffects.Count == 0)
                return;
            PlayVoiceEffect(retainedEffects[UnityEngine.Random.Range(0, retainedEffects.Count)]);
        }

        private static String CleanLine(String line)
        {
            return Regex.Replace(line, "//.*", "").Trim();
        }

        private static void ParseEffect(String effectCode)
        {
            String[] lines = effectCode.Split('\n');
            for (Int32 i = 0; i < lines.Length; i++)
            {
                lines[i] = CleanLine(lines[i]);

                // Find the next effect
                while (!lines[i].StartsWith(">"))
                {
                    i++;
                    if (i >= lines.Length) return; // End of file
                    lines[i] = CleanLine(lines[i]);
                }

                // Get speakers
                Int32 effectLine = i;
                Int32 conditionLine = i;
                String effect = "";
                List<BattleSpeaker> newSpeakers = new List<BattleSpeaker>();
                foreach (String charArg in lines[i].Split([' ', '\t']))
                {
                    if (charArg.StartsWith(">"))
                    {
                        effect = charArg;
                        continue;
                    }
                    if (String.IsNullOrEmpty(charArg))
                        continue;
                    String[] charArgToken = charArg.Trim().Split(':');
                    if (charArgToken.Length == 1)
                    {
                        // The speaker is a player character identified by its CharacterId
                        try
                        {
                            BattleSpeaker speak = new BattleSpeaker();
                            speak.playerId = (CharacterId)Enum.Parse(typeof(CharacterId), charArgToken[0]);
                            newSpeakers.Add(speak);
                        }
                        catch (Exception)
                        {
                            Log.Warning($"[{nameof(BattleVoice)}] Unrecognized player character {charArgToken[0]} at line {i + 1}");
                        }
                    }
                    else if (charArgToken.Length == 2)
                    {
                        // The speaker is an enemy identified by its battle ID and/or its model name
                        BattleSpeaker speak = new BattleSpeaker();
                        if (charArgToken[0].Length > 0)
                            Int32.TryParse(charArgToken[0], out speak.enemyBattleId);
                        // Note: Empty string is valid, allowing to target any enemy. Useful for debugging.
                        if (charArgToken[1].Length > 0)
                        {
                            if (Int32.TryParse(charArgToken[1], out int modelid))
                            {
                                // Verify the number is a valid ModelId
                                if (!FF9BattleDB.GEO.ContainsKey(modelid))
                                {
                                    Log.Warning($"[{nameof(BattleVoice)}] Invalid model id '{modelid}' at line {i + 1}");
                                    continue;
                                }
                                speak.enemyModelId = modelid;
                            }
                            // Look for the model name
                            else if (!FF9BattleDB.GEO.TryGetKey(charArgToken[1], out speak.enemyModelId))
                            {
                                Log.Warning($"[{nameof(BattleVoice)}] Invalid model name '{charArgToken[1]}' at line {i + 1}");
                                continue;
                            }
                        }
                        newSpeakers.Add(speak);
                    }
                }
                if (newSpeakers.Count == 0)
                {
                    Log.Warning($"[{nameof(BattleVoice)}] Expected a speaker for the effect '{lines[i]}' at line {i + 1}");
                    continue;
                }

                BattleMoment moment = BattleMoment.Unknown;
                Int32 priority = 0;
                String status = null;
                String[] paths = null;
                String condition = null;

                // Parse each lines til next effect
                while ((i + 1) < lines.Length && !lines[i + 1].StartsWith(">"))
                {
                    i++;
                    lines[i] = CleanLine(lines[i]);
                    if (lines[i].Length == 0) continue;

                    // Parse When
                    if (lines[i].StartsWith("When"))
                    {
                        if (moment != BattleMoment.Unknown)
                        {
                            Log.Warning($"[{nameof(BattleVoice)}] Battle moment is defined more than once at line {i + 1}");
                            continue;
                        }

                        String when = lines[i].Substring("When".Length);
                        try
                        {
                            moment = (BattleMoment)Enum.Parse(typeof(BattleMoment), when, true);
                        }
                        catch
                        {
                            Log.Warning($"[{nameof(BattleVoice)}] Unrecognized battle moment '{lines[i]}' at line {i + 1}");
                            continue;
                        }
                    }
                    // Parse priority
                    else if (lines[i].StartsWith("Priority:"))
                    {
                        if (priority != 0)
                        {
                            Log.Warning($"[{nameof(BattleVoice)}] Priority: is defined more than once at line {i + 1}");
                            continue;
                        }
                        String priorityArg = lines[i].Substring("Priority:".Length).Trim();
                        if (!Int32.TryParse(priorityArg, out priority))
                        {
                            Log.Warning($"[{nameof(BattleVoice)}] '{priorityArg}' is not a valid value for priority at line {i + 1}");
                        }
                    }
                    // Parse Status
                    else if (lines[i].StartsWith("Status:"))
                    {
                        if (status != null)
                        {
                            Log.Warning($"[{nameof(BattleVoice)}] Status: is defined more than once at line {i + 1}");
                            continue;
                        }
                        String statusArg = lines[i].Substring("Status:".Length).Trim();
                        try
                        {
                            Enum.Parse(typeof(BattleStatusId), statusArg);
                            status = statusArg;
                        }
                        catch
                        {
                            Log.Warning($"[{nameof(BattleVoice)}] '{statusArg}' is not a valid value for status at line {i + 1}");
                        }
                    }
                    // Parse voice path
                    else if (lines[i].StartsWith("VoicePath:"))
                    {
                        if (paths != null)
                        {
                            Log.Warning($"[{nameof(BattleVoice)}] VoicePath: is defined more than once at line {i + 1}");
                            continue;
                        }
                        String pathsValue = lines[i].Substring("VoicePath:".Length);
                        if (pathsValue.IndexOf(',') > 0)
                        {
                            Int32 p = pathsValue.LastIndexOf('/');
                            String folder = (p < 0) ? "" : pathsValue.Substring(0, p + 1);
                            String[] files = (p < 0) ? pathsValue.Split(',') : pathsValue.Substring(p + 1).Split(',');
                            paths = new String[files.Length];
                            for (Int32 j = 0; j < files.Length; j++)
                                paths[j] = folder + files[j].Trim();
                        }
                        else if (!String.IsNullOrEmpty(pathsValue))
                        {
                            paths = [pathsValue];
                        }
                    }
                    else
                    {
                        // Parse condition
                        Match conditionMatch = new Regex(@"\[Condition\](.*?)\[/Condition\]").Match(lines[i]);
                        if (conditionMatch.Success)
                        {
                            if (condition != null)
                            {
                                Log.Warning($"[{nameof(BattleVoice)}] Condition is defined more than once at line {i + 1}");
                                continue;
                            }
                            condition = conditionMatch.Groups[1].Value.Trim();
                            conditionLine = i + 1;
                        }
                    }
                }

                // Verify we have a path
                if (paths == null)
                {
                    Log.Warning($"[{nameof(BattleVoice)}] Expected voice audio path(s) for the effect {effect} at line {effectLine + 1}");
                    continue;
                }

                // Add the effect
                if (effect == ">BattleInOut")
                {
                    if (moment != BattleMoment.Unknown && (moment < BattleMoment.BattleStart || moment > BattleMoment.EnemyEscape))
                    {
                        Log.Warning($"[{nameof(BattleVoice)}] Invalid battle moment 'When{moment}' for {effect} at line {effectLine + 1}");
                        continue;
                    }

                    BattleInOut newEffect = new BattleInOut();
                    newEffect.ConditionLine = conditionLine;
                    newEffect.Speakers = newSpeakers;
                    newEffect.AudioPaths = paths;
                    newEffect.Condition = condition;
                    newEffect.Priority = priority;
                    if (moment != BattleMoment.Unknown)
                        newEffect.When = moment;
                    InOutEffect.Add(newEffect);
                }
                else if (effect == ">Act")
                {
                    if (moment != BattleMoment.Unknown && (moment < BattleMoment.CommandInput || moment > BattleMoment.Cover))
                    {
                        Log.Warning($"[{nameof(BattleVoice)}] Invalid battle moment 'When{moment}' for {effect} at line {effectLine + 1}");
                        continue;
                    }
                    BattleAct newEffect = new BattleAct();
                    newEffect.ConditionLine = conditionLine;
                    newEffect.Speakers = newSpeakers;
                    newEffect.AudioPaths = paths;
                    newEffect.Condition = condition;
                    newEffect.Priority = priority;
                    if (moment != BattleMoment.Unknown)
                        newEffect.When = moment;
                    ActEffect.Add(newEffect);
                }
                else if (String.Compare(effect, ">Hitted") == 0)
                {
                    if (moment != BattleMoment.Unknown && (moment < BattleMoment.Damaged || moment > BattleMoment.Missed))
                    {
                        Log.Warning($"[{nameof(BattleVoice)}] Invalid battle moment 'When{moment}' for {effect} at line {effectLine + 1}");
                        continue;
                    }
                    BattleHitted newEffect = new BattleHitted();
                    newEffect.ConditionLine = conditionLine;
                    newEffect.Speakers = newSpeakers;
                    newEffect.AudioPaths = paths;
                    newEffect.Condition = condition;
                    newEffect.Priority = priority;
                    if (moment != BattleMoment.Unknown)
                        newEffect.When = moment;
                    HittedEffect.Add(newEffect);
                }
                else if (String.Compare(effect, ">StatusChange") == 0)
                {
                    if (status == null)
                    {
                        Log.Warning($"[{nameof(BattleVoice)}] Expected a status for the effect {effect} at line {effectLine + 1}");
                        continue;
                    }
                    if (moment != BattleMoment.Unknown && (moment < BattleMoment.Added || moment > BattleMoment.Used))
                    {
                        Log.Warning($"[{nameof(BattleVoice)}] Invalid battle moment 'When{moment}' for {effect} at line {effectLine + 1}");
                        continue;
                    }
                    BattleStatusChange newEffect = new BattleStatusChange();
                    newEffect.ConditionLine = conditionLine;
                    newEffect.Speakers = newSpeakers;
                    newEffect.AudioPaths = paths;
                    newEffect.Condition = condition;
                    newEffect.Priority = priority;
                    if (moment != BattleMoment.Unknown)
                        newEffect.When = moment;
                    newEffect.Status = (BattleStatusId)Enum.Parse(typeof(BattleStatusId), status);
                    StatusChangeEffect.Add(newEffect);
                }
            }
        }
    }
}
