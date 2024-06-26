using Memoria.Prime.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Memoria.Speedrun
{
    static class SpeedrunSettings
    {
        public static List<Split> Splits = new List<Split>();
        public static Int32 CurrentSplitIndex = -1;

        public static Boolean LogGameTimeEnabled => Configuration.Speedrun.IsEnabled && GameTimeLogStream != null;

        static SpeedrunSettings()
        {
            if (!Configuration.Speedrun.IsEnabled)
                return;
            LoadSplitSetting();
            InitGameTimeLog();
        }

        public static void LogGameTime()
        {
            if (!LogGameTimeEnabled)
                return;
            Monitor.Enter(GameTimeLogStream);
            Int32 allSeconds = (Int32)FF9StateSystem.Settings.time;
            Int32 hours = allSeconds / 3600;
            Int32 minutes = (allSeconds / 60) % 60;
            Int32 seconds = allSeconds % 60;
            Int32 secondhundredths = (Int32)(100 * FF9StateSystem.Settings.time) % 100;
            GameTimeLogStream.BaseStream.Position = 0;
            GameTimeLogStream.Write($"{hours:D2}:{minutes:D2}:{seconds:D2}.{secondhundredths:D2}");
            GameTimeLogStream.Flush();
            Monitor.Exit(GameTimeLogStream);
        }

        private static Boolean LoadSplitSetting()
        {
            CurrentSplitIndex = -1;
            Splits.Clear();
            if (String.IsNullOrEmpty(Configuration.Speedrun.SplitSettingsPath) || !File.Exists(Configuration.Speedrun.SplitSettingsPath))
                return false;
            using (Stream input = File.OpenRead(Configuration.Speedrun.SplitSettingsPath))
            using (StreamReader reader = new StreamReader(input))
            {
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine().Trim();
                    if (String.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("//"))
                        continue;
                    if (line.StartsWith("Split"))
                        ProcessSplitLine(line.Substring("Split".Length).Trim());
                    else if (line.StartsWith("Trigger"))
                        ProcessTriggerLine(line.Substring("Trigger".Length).Trim());
                }
            }
            CurrentSplitIndex = -1;
            return true;
        }

        private static Boolean InitGameTimeLog()
        {
            if (String.IsNullOrEmpty(Configuration.Speedrun.LogGameTimePath) || !Directory.Exists(Path.GetDirectoryName(Configuration.Speedrun.LogGameTimePath)))
                return false;
            try
            {
                Stream stream = new FileStream(Configuration.Speedrun.LogGameTimePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));
                GameTimeLogStream = new StreamWriter(stream);
            }
            catch
            {
                GameTimeLogStream = null;
                return false;
            }
            return true;
        }

        private static void ProcessSplitLine(String splitArg)
        {
            CurrentSplitIndex++;
            Splits.Add(new Split());
            Splits[CurrentSplitIndex].Name = splitArg;
        }

        private static void ProcessTriggerLine(String triggerArg)
        {
            if (CurrentSplitIndex < 0 || CurrentSplitIndex >= Splits.Count)
                return;
            Split current = Splits[CurrentSplitIndex];
            String[] argList = triggerArg.Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (argList.Length < 1)
                return;
            if (!argList[0].TryEnumParse(out Split.TriggerType triggerType))
                return;
            Int32 scenario = -1;
            switch (triggerType)
            {
                case Split.TriggerType.FIELD_TRANSITION:
                    if (argList.Length < 3)
                        return;
                    if (!Int32.TryParse(argList[1], out Int32 from) || !Int32.TryParse(argList[2], out Int32 to))
                        return;
                    if (argList.Length > 3 && !Int32.TryParse(argList[3], out scenario))
                        scenario = -1;
                    current.AddFieldTransitionCondition(from, to, scenario);
                    break;
                case Split.TriggerType.BATTLE_WIN:
                case Split.TriggerType.BATTLE_STOP:
                case Split.TriggerType.BATTLE_END:
                    if (argList.Length < 2)
                        return;
                    if (!Int32.TryParse(argList[1], out Int32 battleId))
                        return;
                    if (argList.Length > 2 && !Int32.TryParse(argList[2], out scenario))
                        scenario = -1;
                    current.AddBattleCondition(triggerType, battleId, scenario);
                    break;
            }
        }

        private static StreamWriter GameTimeLogStream = null;
    }
}
