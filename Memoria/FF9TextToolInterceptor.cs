using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;

namespace Memoria
{
    public static class FF9TextToolInterceptor
    {
        public static IEnumerator InitializeFieldText()
        {
            Log.Message(nameof(InitializeFieldText));
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

            TextLoader loader = new TextLoader("/ETC/minista.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetCardName(loader.ExtractSentenseEnd());

            loader = new TextLoader("/ETC/ff9choco.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetChocoUiText(loader.ExtractSentenseEnd());

            loader = new TextLoader("/ETC/card.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetCardLvName(loader.ExtractSentenseEnd());

            loader = new TextLoader("/ETC/cmdtitle.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetCmdTitleText(loader.ExtractSentenseEnd());

            loader = new TextLoader("/ETC/follow.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetFollowText(loader.ExtractSentenseEnd());

            loader = new TextLoader("/ETC/libra.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetLibraText(loader.ExtractSentenseEnd());

            loader = new TextLoader("/ETC/worldloc.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetWorldLocationText(loader.ExtractSentenseEnd());

            FF9TextTool.IsLoading = false;
        }

        private static IEnumerable InitializeLocationTextInternal()
        {
            FF9TextTool.IsLoading = true;

            TextLoader loader = new TextLoader("/Location/loc_name.mes");
            while (loader.Loading)
                yield return 0;

            Dictionary<int, string> locationNames = FF9TextToolAccessor.GetLocationName();
            String[] array = loader.Text.Split('\r');
            for (int i = 0; i < array.Length; i++)
            {
                String str = array[i];
                str = str.Replace("\n", string.Empty);
                if (!String.IsNullOrEmpty(str))
                {
                    string key = str.Split(':')[0];
                    string value = str.Split(':')[1];
                    locationNames[int.Parse(key)] = FF9TextTool.RemoveOpCode(value);
                }
            }

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

            TextLoader loader = new TextLoader("/Battle/" + battleZoneId + ".mes");
            while (loader.Loading)
                yield return 0;
            if (loader.Text != null)
                FF9TextToolAccessor.SetBattleText(loader.ExtractSentenseEnd());

            FF9TextTool.IsLoading = false;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
        }

        private static IEnumerable InitializeCommandTextInternal()
        {
            FF9TextTool.IsLoading = true;

            TextLoader loader = new TextLoader("/Command/com_name.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetCommandName(loader.ExtractSentenseEnd());

            loader = new TextLoader("/Command/com_help.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetCommandHelpDesc(loader.ExtractSentenseEnd());

            FF9TextTool.IsLoading = false;
        }

        private static IEnumerable InitializeAbilityTextInternal()
        {
            FF9TextTool.IsLoading = true;

            TextLoader loader = new TextLoader("/Ability/sa_help.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetSupportAbilityHelpDesc(loader.ExtractSentenseEnd());

            loader = new TextLoader("/Ability/sa_name.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetSupportAbilityName(loader.ExtractSentenseEnd());

            loader = new TextLoader("/Ability/aa_help.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetActionAbilityHelpDesc(loader.ExtractSentenseEnd());

            loader = new TextLoader("/Ability/aa_name.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetActionAbilityName(loader.ExtractSentenseEnd());

            FF9TextTool.IsLoading = false;
        }

        private static IEnumerable InitializeImportantItemTextInternal()
        {
            FF9TextTool.IsLoading = true;

            TextLoader loader = new TextLoader("/KeyItem/imp_help.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetImportantItemHelpDesc(loader.ExtractSentenseEnd());

            loader = new TextLoader("/KeyItem/imp_name.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetImportantItemName(loader.ExtractSentenseEnd());

            loader = new TextLoader("/KeyItem/imp_skin.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetImportantSkinDesc(loader.ExtractSentenseEnd());

            FF9TextTool.IsLoading = false;
        }

        private static IEnumerable InitializeItemTextInternal()
        {
            FF9TextTool.IsLoading = true;

            TextLoader loader = new TextLoader("/Item/itm_btl.mes");
            while (loader.Loading)
                yield return 0;

            FF9TextToolAccessor.SetItemBattleDesc(loader.ExtractSentenseEnd());
            loader = new TextLoader("/Item/itm_help.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetItemHelpDesc(loader.ExtractSentenseEnd());

            loader = new TextLoader("/Item/itm_name.mes");
            while (loader.Loading)
                yield return 0;
            FF9TextToolAccessor.SetItemName(loader.ExtractSentenseEnd());

            FF9TextTool.IsLoading = false;
        }

        private static IEnumerable InitializeFieldTextInternal()
        {
            Int32 fieldZoneId = FF9TextToolAccessor.GetFieldZoneId();
            if (fieldZoneId == -1)
            {
                PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
                yield return 0;
                yield break;
            }

            FF9TextTool.IsLoading = true;
            TextLoader loader = new TextLoader("/Field/" + GetFieldTextFileName(fieldZoneId) + ".mes");
            while (loader.Loading)
                yield return 0;

            if (loader.Text != null)
            {
                String source = TextOpCodeModifier.Modify(loader.Text);
                String[] text = ExtractSentense(source);
                FF9TextToolAccessor.SetFieldText(text);
                FF9TextToolAccessor.SetTableText(ExtractTableText(text));
            }

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

        private static void ReplaceFieldText(String[] originalSource, Int32 fieldZoneId)
        {
            const String directoryPath = @"Resources\Fields";

            Log.Message("ReplaceFieldText");
            Directory.CreateDirectory(directoryPath);

            String zoneKey = fieldZoneId.ToString("D4", CultureInfo.InvariantCulture);
            String fileName = Path.Combine(directoryPath, zoneKey + ".strings");
            if (File.Exists(fileName))
            {
                Log.Message($"File [{fileName}] is exist. Loading...");
                using (FileStream input = File.OpenRead(fileName))
                {
                    String storedName;
                    TxtEntry[] entries = new TxtReader(input, StringsFormatter.Instance).Read(out storedName);
                    if (originalSource?.Length == entries.Length)
                    {
                        for (int i = 0; i < entries.Length; i++)
                            originalSource[i] = entries[i].Value;
                    }
                }
                Log.Message("Success!");
            }
            else
            {
                Log.Message($"File [{fileName}] does not exist. Creating...");
                using (FileStream output = File.Create(fileName))
                {
                    TxtEntry[] entries = originalSource.Select(s => new TxtEntry {Prefix = zoneKey, Value = s}).ToArray();
                    new TxtWriter(output, StringsFormatter.Instance).Write(fileName, entries);
                }
                Log.Message("Success!");
            }
        }

        private sealed class TextLoader
        {
            private readonly AssetManagerRequest _request;

            public String Text { get; private set; }
            public Boolean Loading => IsLoading();

            public TextLoader(String relativePath)
            {
                String fileName = Localization.GetPath() + relativePath;
                _request = AssetManager.LoadAsync<TextAsset>(fileName);
            }

            public String[] ExtractSentenseEnd()
            {
                return FF9TextToolInterceptor.ExtractSentenseEnd(Text);
            }

            private Boolean IsLoading()
            {
                if (!_request.isDone)
                    return true;

                TextAsset asset = (TextAsset)_request.asset;
                if (asset != null)
                    Text = asset.text;

                return false;
            }
        }
    }
}