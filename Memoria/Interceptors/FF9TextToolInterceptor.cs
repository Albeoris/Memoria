using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria
{
    public static class FF9TextToolInterceptor
    {
        public static readonly BattleImporter BattleImporter = new BattleImporter();
        public static readonly FieldImporter FieldImporter = new FieldImporter();

        public static IEnumerator InitializeFieldText()
        {
            Log.Message(nameof(InitializeFieldText));
            FieldImporter.InitializationTask?.WaitSafe();
            return InitializeFieldTextInternal().GetEnumerator();
        }

        public static IEnumerator InitializeItemText()
        {
            Log.Message(nameof(InitializeItemText));
            return InitializeItemTextInternal().GetEnumerator();
        }

        public static IEnumerator InitializeImportantItemText()
        {
            Log.Message(nameof(InitializeImportantItemText));
            return InitializeImportantItemTextInternal().GetEnumerator();
        }

        public static IEnumerator InitializeAbilityText()
        {
            Log.Message(nameof(InitializeAbilityText));
            return InitializeAbilityTextInternal().GetEnumerator();
        }

        public static IEnumerator InitializeCommandText()
        {
            Log.Message(nameof(InitializeCommandText));
            return InitializeCommandTextInternal().GetEnumerator();
        }

        public static IEnumerator InitializeBattleText()
        {
            Log.Message(nameof(InitializeBattleText));
            BattleImporter.InitializationTask?.WaitSafe();
            return InitializeBattleTextInternal().GetEnumerator();
        }

        public static IEnumerator InitializeLocationText()
        {
            Log.Message(nameof(InitializeLocationText));
            return InitializeLocationTextInternal().GetEnumerator();
        }

        public static IEnumerator InitializeEtcText()
        {
            Log.Message(nameof(InitializeEtcText));
            return InitializeEtcTextInternal().GetEnumerator();
        }

        private static IEnumerable InitializeEtcTextInternal()
        {
            FF9TextTool.IsLoading = true;

            foreach (EtcImporter importer in EtcImporter.EnumerateImporters())
            {
                foreach (var state in importer.LoadAsync())
                    yield return state;
            }

            FF9TextTool.IsLoading = false;
        }

        private static IEnumerable InitializeLocationTextInternal()
        {
            FF9TextTool.IsLoading = true;

            LocationNameImporter importer = new LocationNameImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            FF9TextTool.IsLoading = false;
        }

        private static IEnumerable InitializeBattleTextInternal()
        {
            Int32 battleZoneId = FF9TextToolAccessor.GetBattleZoneId();
            if (battleZoneId == -1)
            {
                PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
                yield return 0;
                yield break;
            }

            FF9TextTool.IsLoading = true;

            foreach (var state in BattleImporter.LoadAsync())
                yield return state;

            FF9TextTool.IsLoading = false;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
        }

        private static IEnumerable InitializeCommandTextInternal()
        {
            FF9TextTool.IsLoading = true;

            CommandImporter importer = new CommandImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            FF9TextTool.IsLoading = false;
        }

        private static IEnumerable InitializeAbilityTextInternal()
        {
            FF9TextTool.IsLoading = true;

            SingleFileImporter importer = new AbilityImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            importer = new SkillImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            FF9TextTool.IsLoading = false;
        }

        private static IEnumerable InitializeImportantItemTextInternal()
        {
            FF9TextTool.IsLoading = true;

            KeyItemImporter importer = new KeyItemImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            FF9TextTool.IsLoading = false;
        }

        private static IEnumerable InitializeItemTextInternal()
        {
            FF9TextTool.IsLoading = true;

            ItemImporter importer = new ItemImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            FF9TextTool.IsLoading = false;
        }

        private static IEnumerable InitializeFieldTextInternal()
        {
            PersistenSingleton<UIManager>.Instance.SetEventEnable(false);

            Int32 fieldZoneId = FF9TextToolAccessor.GetFieldZoneId();
            if (fieldZoneId == -1)
            {
                PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
                yield return 0;
                yield break;
            }

            FF9TextTool.IsLoading = true;

            foreach (var state in FieldImporter.LoadAsync())
            {
                yield return state;
            }

            //TextLoader loader = new TextLoader("/Field/" + GetFieldTextFileName(fieldZoneId) + ".mes");
            //while (loader.Loading)
            //    yield return 0;

            //if (loader.Text != null)
            //{
            //    String source = TextOpCodeModifier.Modify(loader.Text);
            //    String[] text = ExtractSentense(source);
            //    FF9TextToolAccessor.SetFieldText(text);
            //    FF9TextToolAccessor.SetTableText(ExtractTableText(text));
            //}

            FF9TextTool.IsLoading = false;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
        }

        public static string GetFieldTextFileName(int fieldZoneId)
        {
            string str = fieldZoneId.ToString();
            if (FF9StateSystem.MobilePlatform && fieldZoneId == 71)
                str += "m";
            return str;
        }

        public static string[] ExtractSentense(string text)
        {
            String[] strArray1 = Regex.Split(text, "\\[STRT\\=");
            String[] strArray2 = new string[strArray1.Length - 1];
            for (int index = 1; index < strArray1.Length; ++index)
                strArray2[index - 1] = "[STRT=" + strArray1[index];
            return strArray2;
        }

        public static string[] ExtractSentenseEnd(string text)
        {
            return text.Split(new[] {"[ENDN]"}, StringSplitOptions.None);
        }

        public static string[][] ExtractTableText(string[] extactedList)
        {
            string[] array = extactedList.Where(t => t.Contains("[TBLE=")).ToArray();
            int length = array.Length;
            if (length <= 0)
                return null;

            string[][] strArray1 = new string[length][];
            for (int index = 0; index < length; ++index)
            {
                string str = array[index];
                for (int startIndex1 = 0; startIndex1 < str.Length && startIndex1 + 5 <= str.Length; ++startIndex1)
                {
                    int num1 = startIndex1;
                    if (str.Substring(startIndex1, 5) == "[" + NGUIText.TableStart)
                    {
                        int num2 = str.IndexOf(']', startIndex1 + 4);
                        int startIndex2 = num2 + 1;
                        int num3 = str.IndexOf('[', startIndex2);
                        string[] strArray3 = str.Substring(startIndex2, num3 - num2 - 1).Split('\n');
                        strArray1[index] = strArray3;
                        num1 = num3 - 1;
                    }
                    startIndex1 = num1;
                }
            }
            return strArray1;
        }
    }
}