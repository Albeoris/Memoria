using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NCalc;
using Memoria.Prime;
using Memoria.Assets;

namespace Memoria.Data
{
    static class BattleVoice
    {
        public const String BattleVoicePath = "BattleVoiceEffects.txt";

        static BattleVoice()
        {
            if (!Configuration.VoiceActing.Enabled)
                return;

            foreach (String folder in Configuration.Mod.AllFolderNames)
            {
                String inputPath = String.IsNullOrEmpty(folder) ? BattleVoicePath : folder + "/" + BattleVoicePath;
                if (File.Exists(inputPath))
                    ParseEffect(File.ReadAllText(inputPath));
            }
        }

        public static void InitBattle()
		{
            _currentVoicePlay.Clear();
        }

        private class BattleSpeaker
		{
            public CharacterId playerId = CharacterId.NONE;
            public String enemyModel = null;
            public Int32 enemyBattleId = -1;

            public Boolean CheckIsCharacter(BTL_DATA btl)
			{
                if (btl.bi.player != 0)
                    return playerId != CharacterId.NONE && (CharacterId)btl.bi.slot_no == playerId;
                if (playerId != CharacterId.NONE)
                    return false;
                String modelName = (btl.dms_geo_id == -1) ? String.Empty : FF9BattleDB.GEO.GetValue(btl.dms_geo_id);
                return (enemyModel == null || enemyModel == modelName) && (enemyBattleId < 0 || enemyBattleId == FF9StateSystem.Battle.battleMapIndex);
            }

            public BTL_DATA FindBtlUnlimited()
			{
                FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
                for (Int32 i = 0; i < 8; i++)
                    if (CheckIsCharacter(ff9Battle.btl_data[i]))
                        return ff9Battle.btl_data[i];
                return null;
            }

            public static Boolean CheckCanSpeak(BTL_DATA btl, Int32 voicePriority, BattleStatus statusException = 0)
            {
                if (btl.bi.disappear != 0)
                    return false;
                if (btl_stat.CheckStatus(btl, BattleStatus.CannotSpeak & ~statusException))
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
            public String AudioPath = "";
            public Int32 Priority = 0;

            public Boolean CheckSpeakerAll(BTL_DATA statusExceptionBtl = null, BattleStatus statusException = 0)
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
                    if (!BattleSpeaker.CheckCanSpeak(btl, Priority, statusExceptionBtl == btl ? statusException : 0))
                        return false;
                }
                return true;
            }

            public Boolean CheckIsFirstSpeaker(BTL_DATA btl, BattleStatus statusException = 0)
            {
                if (Speakers.Count == 0)
                    return false;
                return Speakers[0].CheckIsCharacter(btl) && BattleSpeaker.CheckCanSpeak(btl, Priority, statusException);
            }
        }

        private class BattleInOut : GenericVoiceEffect
        {
            public String When = "BattleStart"; // "GameOver", "Defeated", "VictoryPose", "Victory", "Flee", "BattleInterrupted", "EnemyEscape"
        }
        private class BattleAct : GenericVoiceEffect
        {
            // SomeoneElse: unused option for now
            public Boolean SomeoneElse = false;
            public String When = "CommandPerform"; // "CommandInput", "HitEffect", "Cover"
        }
        private class BattleHitted : GenericVoiceEffect
        {
            public Boolean SomeoneElse = false;
        }
        private class BattleStatusChange : GenericVoiceEffect
        {
            public Boolean SomeoneElse = false;
            public String When = "Added"; // "Removed", "Used"
            public BattleStatus Status = 0;
        }

        private static List<BattleInOut> InOutEffect = new List<BattleInOut>();
        private static List<BattleAct> ActEffect = new List<BattleAct>();
        private static List<BattleHitted> HittedEffect = new List<BattleHitted>();
        private static List<BattleStatusChange> StatusChangeEffect = new List<BattleStatusChange>();

        private static Dictionary<BTL_DATA, KeyValuePair<Int32, SoundProfile>> _currentVoicePlay = new Dictionary<BTL_DATA, KeyValuePair<Int32, SoundProfile>>();

        private static void PlayVoiceEffect(GenericVoiceEffect voiceEffect)
		{
            List<BTL_DATA> speakerBtlList = new List<BTL_DATA>();
            foreach (BattleSpeaker speaker in voiceEffect.Speakers)
			{
                BTL_DATA btl = speaker.FindBtlUnlimited();
                if (btl != null)
                    speakerBtlList.Add(btl);
            }
            String soundPath = $"Voices/{Localization.GetSymbol()}/{voiceEffect.AudioPath}";
            Boolean soundExists = AssetManager.HasAssetOnDisc("Sounds/" + soundPath + ".akb", true, true) || AssetManager.HasAssetOnDisc("Sounds/" + soundPath + ".ogg", true, false);
            SoundLib.VALog($"battlevoice:{voiceEffect.GetType()} character:{(speakerBtlList.Count > 0 ? new BattleUnit(speakerBtlList[0]).Name : "no speaker")} path:{soundPath}" + (soundExists ? "" : " (not found)"));
            SoundProfile audioProfile = VoicePlayer.CreateLoadThenPlayVoice(soundPath.GetHashCode(), soundPath,
            () =>
            {
                foreach (BTL_DATA btl in speakerBtlList)
                    _currentVoicePlay.Remove(btl);
            });
            KeyValuePair<Int32, SoundProfile> playingVoice;
            foreach (BTL_DATA btl in speakerBtlList)
			{
                if (_currentVoicePlay.TryGetValue(btl, out playingVoice))
                    SoundLib.voicePlayer.StopSound(playingVoice.Value);
                _currentVoicePlay[btl] = new KeyValuePair<Int32, SoundProfile>(voiceEffect.Priority, audioProfile);
            }
        }

        public static void TriggerOnBattleInOut(String when)
        {
            if (!Configuration.VoiceActing.Enabled)
                return;

            try
            {
                List<BattleInOut> retainedEffects = new List<BattleInOut>();
                Int32 retainedPriority = Int32.MinValue;
                foreach (BattleInOut effect in InOutEffect)
                {
                    if (String.Compare(effect.When, when) != 0 || effect.Priority < retainedPriority || !effect.CheckSpeakerAll())
                        continue;
                    if (!String.IsNullOrEmpty(effect.Condition))
                    {
                        Expression c = new Expression(effect.Condition);
                        BattleUnit unit = new BattleUnit(effect.Speakers[0].FindBtlUnlimited());
                        NCalcUtility.InitializeExpressionUnit(ref c, unit);
                        c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
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
            catch (Exception err)
            {
                Log.Error(err);
            }
        }

        public static void TriggerOnBattleAct(BTL_DATA actingChar, String when, CMD_DATA cmdUsed, BattleCalculator calc = null)
        {
            if (!Configuration.VoiceActing.Enabled)
                return;

            try
            {
                List<BattleAct> retainedEffects = new List<BattleAct>();
                Int32 retainedPriority = Int32.MinValue;
                foreach (BattleAct effect in ActEffect)
                {
                    if (String.Compare(effect.When, when) != 0 || effect.Priority < retainedPriority || !effect.CheckSpeakerAll() || !effect.CheckIsFirstSpeaker(actingChar))
                        continue;
                    if (!String.IsNullOrEmpty(effect.Condition))
                    {
                        Expression c = new Expression(effect.Condition);
                        BattleUnit unit = new BattleUnit(actingChar);
                        BattleCommand cmd = new BattleCommand(cmdUsed);
                        NCalcUtility.InitializeExpressionUnit(ref c, unit);
                        NCalcUtility.InitializeExpressionCommand(ref c, cmd);
                        if (calc != null) // Should be the case only when "HitEffect"
                            NCalcUtility.InitializeExpressionAbilityContext(ref c, calc);
                        c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
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
            catch (Exception err)
			{
                Log.Error(err);
			}
        }

        public static void TriggerOnHitted(BTL_DATA hittedChar, BattleCalculator calc)
        {
            if (!Configuration.VoiceActing.Enabled)
                return;

            try
            {
                List<BattleHitted> retainedEffects = new List<BattleHitted>();
                Int32 retainedPriority = Int32.MinValue;
                foreach (BattleHitted effect in HittedEffect)
                {
                    if (effect.Priority < retainedPriority || !effect.CheckSpeakerAll() || !effect.CheckIsFirstSpeaker(hittedChar))
                        continue;
                    if (!String.IsNullOrEmpty(effect.Condition))
                    {
                        Expression c = new Expression(effect.Condition);
                        BattleUnit unit = new BattleUnit(hittedChar);
                        NCalcUtility.InitializeExpressionUnit(ref c, unit);
                        NCalcUtility.InitializeExpressionCommand(ref c, calc.Command);
                        NCalcUtility.InitializeExpressionAbilityContext(ref c, calc);
                        c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                        c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
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
            catch (Exception err)
            {
                Log.Error(err);
            }
        }

        public static void TriggerOnStatusChange(BTL_DATA statusedChar, String when, BattleStatus whichStatus)
        {
            if (!Configuration.VoiceActing.Enabled)
                return;

            try
            {
                List<BattleStatusChange> retainedEffects = new List<BattleStatusChange>();
                Int32 retainedPriority = Int32.MinValue;
                foreach (BattleStatusChange effect in StatusChangeEffect)
                {
                    if (String.Compare(effect.When, when) != 0 || (whichStatus & effect.Status) == 0 || effect.Priority < retainedPriority || !effect.CheckSpeakerAll(statusedChar, effect.Status))
                        continue;
                    Boolean discardStatusChecks = String.Compare(when, "Removed") != 0;
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
                        if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                            continue;
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
            catch (Exception err)
            {
                Log.Error(err);
            }
        }

        private static void ParseEffect(String effectCode)
        {
            MatchCollection codeMatches = new Regex(@"^(>BattleInOut|>Act|>Hitted|>StatusChange)\b", RegexOptions.Multiline).Matches(effectCode);
            for (Int32 i = 0; i < codeMatches.Count; i++)
            {
                String bvCode = codeMatches[i].Groups[1].Value;
                Int32 endPos, startPos = codeMatches[i].Groups[1].Captures[0].Index + codeMatches[i].Groups[1].Value.Length + 1;
                if (i + 1 == codeMatches.Count)
                    endPos = effectCode.Length;
                else
                    endPos = codeMatches[i + 1].Groups[1].Captures[0].Index;
                Int32 eolPos = effectCode.IndexOf('\n', startPos);
                if (eolPos < 0)
                    continue;
                String charArgFull = effectCode.Substring(startPos, eolPos - startPos);
                startPos += charArgFull.Length + 1;
                String bvArgs = effectCode.Substring(startPos, endPos - startPos);
                List<BattleSpeaker> newSpeakers = new List<BattleSpeaker>();
                foreach (String charArg in charArgFull.Split(new char[] { ' ', '\t' }))
                {
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
                        catch (Exception err)
						{
                            Log.Warning($"[{nameof(BattleVoice)}] Unrecognized player character {charArgToken[0]}");
						}
                    }
                    else if (charArgToken.Length == 2)
					{
                        // The speaker is an enemy identified by its battle ID and/or its model name
                        BattleSpeaker speak = new BattleSpeaker();
                        if (charArgToken[0].Length > 0)
                            Int32.TryParse(charArgToken[0], out speak.enemyBattleId);
                        if (charArgToken[1].Length > 0)
                            speak.enemyModel = charArgToken[1];
                        newSpeakers.Add(speak);
                    }
                }
                if (newSpeakers.Count == 0)
                {
                    Log.Warning($"[{nameof(BattleVoice)}] Expected a speaker for the effect {bvCode}");
                    continue;
                }
                String path = null;
                Match pathMatch = new Regex(@"\bVoicePath:(\S+)").Match(bvArgs);
                if (pathMatch.Success)
                    path = pathMatch.Groups[1].Value;
                if (String.IsNullOrEmpty(path))
                {
                    Log.Warning($"[{nameof(BattleVoice)}] Expected a voice audio path for the effect {bvCode}");
                    continue;
                }
                String condition = null;
                Match conditionMatch = new Regex(@"\[Condition\](.*?)\[/Condition\]").Match(bvArgs);
                if (conditionMatch.Success)
                    condition = conditionMatch.Groups[1].Value;
                Int32 priority = 0;
                Match priorityMatch = new Regex(@"\bPriority:(\d+)").Match(bvArgs);
                if (priorityMatch.Success)
                    Int32.TryParse(priorityMatch.Groups[1].Value, out priority);
                if (String.Compare(bvCode, ">BattleInOut") == 0)
                {
                    BattleInOut newEffect = new BattleInOut();
                    newEffect.Speakers = newSpeakers;
                    newEffect.AudioPath = path;
                    newEffect.Condition = condition;
                    newEffect.Priority = priority;
                    Match whenMatch = new Regex(@"\bWhen(\w+)\b").Match(bvArgs);
                    if (whenMatch.Success)
                        newEffect.When = whenMatch.Groups[1].Value;
                    InOutEffect.Add(newEffect);
                }
                else if (String.Compare(bvCode, ">Act") == 0)
                {
                    BattleAct newEffect = new BattleAct();
                    newEffect.Speakers = newSpeakers;
                    newEffect.AudioPath = path;
                    newEffect.Condition = condition;
                    newEffect.Priority = priority;
                    Match whenMatch = new Regex(@"\bWhen(\w+)\b").Match(bvArgs);
                    if (whenMatch.Success)
                        newEffect.When = whenMatch.Groups[1].Value;
                    ActEffect.Add(newEffect);
                }
                else if (String.Compare(bvCode, ">Hitted") == 0)
                {
                    BattleHitted newEffect = new BattleHitted();
                    newEffect.Speakers = newSpeakers;
                    newEffect.AudioPath = path;
                    newEffect.Condition = condition;
                    newEffect.Priority = priority;
                    HittedEffect.Add(newEffect);
                }
                else if (String.Compare(bvCode, ">StatusChange") == 0)
                {
                    BattleStatusChange newEffect = new BattleStatusChange();
                    newEffect.Speakers = newSpeakers;
                    newEffect.AudioPath = path;
                    newEffect.Condition = condition;
                    newEffect.Priority = priority;
                    Match whenMatch = new Regex(@"\bWhen(\w+)\b").Match(bvArgs);
                    if (whenMatch.Success)
                        newEffect.When = whenMatch.Groups[1].Value;
                    try
                    {
                        Match statusMatch = new Regex(@"\bStatus:([\w ,]+)\b").Match(bvArgs);
                        if (statusMatch.Success)
                            newEffect.Status = (BattleStatus)Enum.Parse(typeof(BattleStatus), statusMatch.Groups[1].Value);
                    }
                    catch (Exception err)
                    {
                    }
                    if (newEffect.Status == 0)
                    {
                        Log.Warning($"[{nameof(BattleVoice)}] Expected a status for the effect {bvCode}");
                        continue;
                    }
                    StatusChangeEffect.Add(newEffect);
                }
            }
        }
    }
}
