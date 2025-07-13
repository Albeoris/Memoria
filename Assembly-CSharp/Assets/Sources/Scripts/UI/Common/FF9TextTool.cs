using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
    public class FF9TextTool : PersistenSingleton<FF9TextTool>
    {
        public static Color White => new Color(0.784313738f, 0.784313738f, 0.784313738f);
        public static Color Gray => new Color(0.5647059f, 0.5647059f, 0.5647059f);
        public static Color Black => new Color(0.145f, 0.145f, 0.145f);
        public static Color Green => new Color(0.470588237f, 0.784313738f, 0.2509804f);
        public static Color Cyan => new Color(0.407843143f, 0.7529412f, 0.847058833f);
        public static Color Red => new Color(0.8156863f, 0.3764706f, 0.3137255f);
        public static Color Yellow => new Color(0.784313738f, 0.6901961f, 0.2509804f);
        public static Color DarkYellow => new Color(0.588f, 0.584f, 0.267f);
        public static Color Magenta => new Color(0.721568644f, 0.5019608f, 0.8784314f);
        public static Color DarkBlue => new Color(0.219607843f, 0.219607843f, 0.2509804f);

        public static Int32 FieldZoneId => fieldZoneId;
        public static Int32 BattleZoneId => battleZoneId;

        public static readonly BattleImporter BattleImporter = new BattleImporter();
        public static readonly FieldImporter FieldImporter = new FieldImporter();

        public static void ImportArrayToDictionary<T>(String[] entries, Action<T, String> setter)
        {
            Int32 id = 0;
            for (Int32 i = 0; i < entries.Length; i++)
            {
                FF9TextTool.ProcessEntryId(ref entries[i], ref id);
                setter((T)(object)id++, entries[i]);
            }
        }

        public static void ImportWithCumulativeModFiles<T>(String assetPath, Action<T, String> setter)
        {
            foreach (String textFile in AssetManager.LoadStringMultiple(assetPath).Reverse())
            {
                if (String.IsNullOrEmpty(textFile))
                    continue;
                String[] entries = EmbadedSentenseLoader.ExtractSentenseEnd(textFile);
                FF9TextTool.ImportArrayToDictionary<T>(entries, setter);
            }
        }

        public static void ImportStrtWithCumulativeModFiles<T>(String assetPath, Dictionary<Int32, String> dict)
        {
            String pathDir = Path.GetDirectoryName(assetPath) + "/";
            foreach (String textFile in AssetManager.LoadStringMultiple(assetPath).Reverse())
                if (!String.IsNullOrEmpty(textFile))
                    FF9TextTool.ExtractSentense(dict, ProcessLoadMessTags(textFile, pathDir));
        }

        private static String ProcessLoadMessTags(String mesFile, String pathDir)
        {
            Int32 loadMessPos = mesFile.IndexOf("[LOADMES=");
            if (loadMessPos < 0)
                return mesFile;
            Int32 tagIntroLength = "[LOADMES=".Length;
            String result = mesFile.Substring(0, loadMessPos);
            Int32 tagEnd = 0;
            while (loadMessPos >= 0)
            {
                tagEnd = mesFile.IndexOf(']', loadMessPos + tagIntroLength);
                if (tagEnd < 0)
                {
                    tagEnd = mesFile.Length - 1;
                    break;
                }
                String mesFileName = mesFile.Substring(loadMessPos + tagIntroLength, tagEnd - (loadMessPos + tagIntroLength));
                String mesKey = mesFileName + (EmbadedTextResources.CurrentSymbol ?? Localization.CurrentSymbol);
                if (!FF9TextTool.sharedTexts.TryGetValue(mesKey, out String mesContent))
                {
                    mesContent = AssetManager.LoadString(pathDir + mesFileName + ".mes");
                    FF9TextTool.sharedTexts.Add(mesKey, mesContent);
                }
                if (!String.IsNullOrEmpty(mesContent))
                    result += mesContent;
                loadMessPos = mesFile.IndexOf("[LOADMES=", tagEnd + 1);
                if (loadMessPos >= 0)
                    result += mesFile.Substring(tagEnd + 1, loadMessPos - (tagEnd + 1));
            }
            result += mesFile.Substring(tagEnd + 1);
            return result;
        }

        public static String[] GetBattleText(Int32 battleZoneId)
        {
            return EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.GetCurrentPath("/Battle/" + battleZoneId + ".mes"));
        }

        public static IEnumerator InitializeBattleText()
        {
            //Log.Message(nameof(InitializeBattleText));
            BattleImporter.InitializationTask?.WaitSafe();
            return InitializeBattleTextInternal().GetEnumerator();
        }

        public static IEnumerator InitializeFieldText()
        {
            //Log.Message(nameof(InitializeFieldText));
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

        public static IEnumerator InitializeCharacterNamesText()
        {
            Log.Message(nameof(InitializeCharacterNamesText));
            return InitializeCharacterNamesTextInternal().GetEnumerator();
        }

        private static IEnumerable InitializeCharacterNamesTextInternal()
        {
            IsLoading = true;

            CharacterNamesImporter importer = new CharacterNamesImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            IsLoading = false;
        }

        private static IEnumerable InitializeEtcTextInternal()
        {
            IsLoading = true;

            foreach (EtcImporter importer in EtcImporter.EnumerateImporters())
            {
                foreach (var state in importer.LoadAsync())
                    yield return state;
            }

            IsLoading = false;
        }

        private static IEnumerable InitializeLocationTextInternal()
        {
            IsLoading = true;

            LocationNameImporter importer = new LocationNameImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            IsLoading = false;
        }

        private static IEnumerable InitializeCommandTextInternal()
        {
            IsLoading = true;

            CommandImporter importer = new CommandImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            IsLoading = false;
        }

        private static IEnumerable InitializeAbilityTextInternal()
        {
            IsLoading = true;

            SingleFileImporter importer = new AbilityImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            importer = new SkillImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            IsLoading = false;
        }

        private static IEnumerable InitializeImportantItemTextInternal()
        {
            IsLoading = true;

            KeyItemImporter importer = new KeyItemImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            IsLoading = false;
        }

        private static IEnumerable InitializeItemTextInternal()
        {
            IsLoading = true;

            ItemImporter importer = new ItemImporter();
            foreach (var state in importer.LoadAsync())
                yield return state;

            IsLoading = false;
        }

        private static IEnumerable InitializeBattleTextInternal()
        {
            if (battleZoneId == -1)
            {
                PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
                yield return 0;
                yield break;
            }

            IsLoading = true;

            foreach (var state in BattleImporter.LoadAsync())
                yield return state;

            if (Configuration.Lang.DualLanguageMode != 0 && !String.IsNullOrEmpty(Configuration.Lang.DualLanguage))
            {
                EmbadedTextResources.CurrentSymbol = Configuration.Lang.DualLanguage;
                loadSecondaryZone = true;
                BattleImporter.LoadSync();
                loadSecondaryZone = false;
                EmbadedTextResources.CurrentSymbol = null;
            }

            IsLoading = false;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
        }

        private static IEnumerable InitializeFieldTextInternal()
        {
            PersistenSingleton<UIManager>.Instance.SetEventEnable(false);

            if (fieldZoneId == -1)
            {
                PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
                yield return 0;
                yield break;
            }

            IsLoading = true;

            foreach (var state in FieldImporter.LoadAsync())
                yield return state;

            if (Configuration.Lang.DualLanguageMode != 0 && !String.IsNullOrEmpty(Configuration.Lang.DualLanguage))
            {
                EmbadedTextResources.CurrentSymbol = Configuration.Lang.DualLanguage;
                loadSecondaryZone = true;
                FieldImporter.LoadSync();
                loadSecondaryZone = false;
                EmbadedTextResources.CurrentSymbol = null;
            }

            IsLoading = false;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
        }

        public static String GetFieldTextFileName(Int32 fieldZoneId)
        {
            String str = fieldZoneId.ToString();
            if (FF9StateSystem.MobilePlatform && fieldZoneId == 71)
                str += "m";
            return str;
        }

        public static String CharacterDefaultName(CharacterId charId)
        {
            if (characterNames == null)
                characterNames = CharacterNamesFormatter.CharacterDefaultNames();
            return characterNames.TryGetValue(charId, out String name) ? name : "Unknown";
        }

        public static Dictionary<Int32, String> ExtractSentense(Dictionary<Int32, String> table, String text)
        {
            String[] strBlocks = text.Split(DELIM_TEXTID, StringSplitOptions.RemoveEmptyEntries);
            for (Int32 i = 0; i < strBlocks.Length; i++)
            {
                Int32 id = 0;
                if (i > 0)
                {
                    Int32 endPos = strBlocks[i].IndexOf(']');
                    Int32.TryParse(strBlocks[i].Substring(0, endPos), out id);
                    strBlocks[i] = strBlocks[i].Substring(endPos + 1);
                    if (String.IsNullOrEmpty(strBlocks[i]))
                        continue;
                }
                String[] strSequence = strBlocks[i].Split(DELIM_STRT, StringSplitOptions.None);
                for (Int32 j = 1; j < strSequence.Length; j++)
                    table[id++] = "[STRT=" + strSequence[j];
            }
            return table;
        }

        /// <summary>Dummied: table texts are now extracted whenever they are requested from other texts</summary>
        public static String[][] ExtractTableText(IEnumerable<String> extactedList)
        {
            String[] array = extactedList.Where(t => t.Contains("[TBLE=")).ToArray();
            Int32 length = array.Length;
            if (length > 0)
            {
                String[][] tables = new String[length][];
                for (Int32 index = 0; index < length; ++index)
                {
                    String str = array[index];
                    for (Int32 i = 0; i + 5 <= str.Length; i++)
                    {
                        Int32 num1 = i;
                        if (str.Substring(i, 5) == "[" + NGUIText.TableStart)
                        {
                            Int32 num2 = str.IndexOf(']', i + 4);
                            Int32 startIndex2 = num2 + 1;
                            Int32 num3 = str.IndexOf('[', startIndex2);
                            String[] strArray3 = str.Substring(startIndex2, num3 - num2 - 1).Split('\n');
                            tables[index] = strArray3;
                            num1 = num3 - 1;
                        }
                        i = num1;
                    }
                }
                return tables;
            }

            array = extactedList.Where(t => t.Contains("{Table ")).ToArray();
            length = array.Length;
            if (length > 0)
            {
                String[][] table = new String[length][];
                for (Int32 index = 0; index < length; ++index)
                {
                    String str = array[index];
                    for (Int32 i = 0; i + 6 <= str.Length; ++i)
                    {
                        Int32 num1 = i;
                        if (str.Substring(i, 6) == "{Table")
                        {
                            Int32 endingTag = str.IndexOf('}', i + 5);
                            Int32 nextIndex = endingTag + 1;
                            Int32 nextTag = str.IndexOf('{', nextIndex);
                            if (nextTag < 0)
                                nextTag = str.IndexOf('[', nextIndex);

                            String[] strArray3 = str.Substring(nextIndex, nextTag - endingTag - 1).Split('\n');
                            table[index] = strArray3;
                            num1 = nextTag - 1;
                        }
                        i = num1;
                    }
                }
                return table;
            }

            return null;
        }

        public IEnumerator UpdateTextLocalization(Action setMenuLanguageCallback)
        {
            Log.Message($"Updating text localization [{FF9StateSystem.Settings.CurrentLanguage}]");
            Single start = Time.realtimeSinceStartup;
            yield return base.StartCoroutine(FF9TextTool.InitializeItemText());
            yield return base.StartCoroutine(FF9TextTool.InitializeImportantItemText());
            yield return base.StartCoroutine(FF9TextTool.InitializeAbilityText());
            yield return base.StartCoroutine(FF9TextTool.InitializeCommandText());
            yield return base.StartCoroutine(FF9TextTool.InitializeBattleText());
            yield return base.StartCoroutine(FF9TextTool.InitializeLocationText());
            yield return base.StartCoroutine(FF9TextTool.InitializeEtcText());
            yield return base.StartCoroutine(FF9TextTool.InitializeCharacterNamesText());
            if (setMenuLanguageCallback != null)
                setMenuLanguageCallback();
            TextPatcher.expressionCache.Clear();
            // DualLanguage: This doesn't count the initialization time of the 2nd language
            Log.Message($"Total text update time: {Mathf.Round((Time.realtimeSinceStartup - start) * 100f) / 100f}s");
            yield break;
        }

        public static void UpdateTextLocalizationNow()
        {
            new ItemImporter().LoadSync();
            new KeyItemImporter().LoadSync();
            new AbilityImporter().LoadSync();
            new SkillImporter().LoadSync();
            new CommandImporter().LoadSync();
            new LocationNameImporter().LoadSync();
            foreach (EtcImporter importer in EtcImporter.EnumerateImporters())
                importer.LoadSync();
            new CharacterNamesImporter().LoadSync();
        }

        public IEnumerator UpdateFieldText(Int32 _zoneId)
        {
            FF9TextTool.fieldZoneId = _zoneId;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
            yield return base.StartCoroutine(FF9TextTool.InitializeFieldText());
            yield break;
        }

        public static Boolean UpdateFieldTextNow(Int32 _zoneId)
        {
            FF9TextTool.fieldZoneId = _zoneId;
            return FF9TextTool.FieldImporter.LoadSync();
        }

        public IEnumerator UpdateBattleText(Int32 _zoneId)
        {
            FF9TextTool.battleZoneId = _zoneId;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
            yield return base.StartCoroutine(FF9TextTool.InitializeBattleText());
            yield break;
        }

        public static Boolean UpdateBattleTextNow(Int32 _zoneId)
        {
            FF9TextTool.battleZoneId = _zoneId;
            return FF9TextTool.BattleImporter.LoadSync();
        }

        public static String ItemName(RegularItem id)
        {
            if (!DisplayBatch.itemName.TryGetValue(id, out String result))
                result = String.Empty;
            else if (Configuration.Lang.DualLanguageMode == 2 && SecondaryBatch.itemName.TryGetValue(id, out String translation) && !String.IsNullOrEmpty(translation))
                result += " - " + translation;
            return result;
        }

        public static String ItemHelpDescription(RegularItem id)
        {
            return DisplayBatch.itemHelpDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ItemBattleDescription(RegularItem id)
        {
            return DisplayBatch.itemBattleDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ImportantItemName(Int32 id)
        {
            if (!DisplayBatch.importantItemName.TryGetValue(id, out String result))
                result = String.Empty;
            else if (Configuration.Lang.DualLanguageMode == 2 && SecondaryBatch.importantItemName.TryGetValue(id, out String translation) && !String.IsNullOrEmpty(translation))
                result += " - " + translation;
            return result;
        }

        public static String ImportantItemHelpDescription(Int32 id)
        {
            return DisplayBatch.importantItemHelpDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ImportantItemSkin(Int32 id)
        {
            return DisplayBatch.importantSkinDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ActionAbilityName(BattleAbilityId id)
        {
            if (!DisplayBatch.actionAbilityName.TryGetValue(id, out String result))
                result = String.Empty;
            else if (Configuration.Lang.DualLanguageMode == 2 && SecondaryBatch.actionAbilityName.TryGetValue(id, out String translation) && !String.IsNullOrEmpty(translation))
                result += " - " + translation;
            return result;
        }

        public static String ActionAbilityHelpDescription(BattleAbilityId id)
        {
            return DisplayBatch.actionAbilityHelpDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String SupportAbilityName(SupportAbility id)
        {
            if (!DisplayBatch.supportAbilityName.TryGetValue(id, out String result))
                result = String.Empty;
            else if (Configuration.Lang.DualLanguageMode == 2 && SecondaryBatch.supportAbilityName.TryGetValue(id, out String translation) && !String.IsNullOrEmpty(translation))
                result += " - " + translation;
            return result;
        }

        public static String SupportAbilityHelpDescription(SupportAbility id)
        {
            return DisplayBatch.supportAbilityHelpDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String CommandName(BattleCommandId id)
        {
            if (!DisplayBatch.commandName.TryGetValue(id, out String result))
            {
                result = String.Empty;
            }
            else
            {
                if (id == BattleCommandId.AccessMenu && (result == "None" || result == "みてい"))
                    result = Localization.Get("Menu");
                // Disable that behaviour for commands, as the displays are terrible
                //if (Configuration.Lang.DualLanguageMode == 2 && SecondaryBatch.commandName.TryGetValue(id, out String translation) && !String.IsNullOrEmpty(translation))
                //{
                //    if (id == BattleCommandId.AccessMenu && (translation == "None" || translation == "みてい"))
                //    {
                //        Localization.UseSecondaryLanguage = true;
                //        translation = Localization.Get("Menu");
                //        Localization.UseSecondaryLanguage = false;
                //    }
                //    result += " - " + translation;
                //}
            }
            return result;
        }

        public static String CommandHelpDescription(BattleCommandId id)
        {
            if (id == BattleCommandId.AccessMenu && DisplayBatch.commandHelpDesc.TryGetValue(id, out String help))
                if (help == "" || help == "みていを使います。")
                    return Localization.Get("Menu");
            return DisplayBatch.commandHelpDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String FieldText(Int32 textId)
        {
            if (Localization.UseSecondaryLanguage)
                textId = UniversalTextId.GetMatchingTextId(Localization.CurrentSymbol, Localization.CurrentDisplaySymbol, FF9TextTool.fieldZoneId, textId);
            return DisplayBatch.fieldText.TryGetValue(textId, out String result) ? TextOpCodeModifier.Modify(result, textId) : String.Empty;
        }

        public static String CharacterProfile(Int32 charId)
        {
            return Localization.Get("CharacterProfile" + charId);
        }

        public static String LocationName(Int32 id)
        {
            if (!DisplayBatch.locationName.TryGetValue(id, out String result))
                result = String.Empty;
            else if (Configuration.Lang.DualLanguageMode == 2 && SecondaryBatch.locationName.TryGetValue(id, out String translation) && !String.IsNullOrEmpty(translation))
                result += " - " + translation;
            return result;
        }

        public static Boolean IsBattleTextLoaded => MainBatch.battleText != null;

        public static String BattleText(Int32 id)
        {
            return DisplayBatch.battleText.TryGetValue(id, out String result) ? result : Localization.Get("EnemyDummy");
        }

        public static String CardName(TetraMasterCardId id)
        {
            if (!DisplayBatch.cardName.TryGetValue((Int32)id, out String result))
                result = String.Empty;
            else if (Configuration.Lang.DualLanguageMode == 2 && SecondaryBatch.cardName.TryGetValue((Int32)id, out String translation) && !String.IsNullOrEmpty(translation))
                result += " - " + translation;
            return result;
        }

        public static String ChocoboUIText(Int32 id)
        {
            return DisplayBatch.chocoUIText.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String CardLevelName(Int32 id)
        {
            return DisplayBatch.cardLvName.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String BattleFollowText(Int32 id)
        {
            return DisplayBatch.followText.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String BattleLibraText(Int32 id)
        {
            return DisplayBatch.libraText.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String BattleCommandTitleText(Int32 id)
        {
            return DisplayBatch.cmdTitleText.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String WorldLocationText(Int32 id)
        {
            if (!DisplayBatch.worldLocationText.TryGetValue(id, out String result))
                result = String.Empty;
            else if (Configuration.Lang.DualLanguageMode == 2 && SecondaryBatch.worldLocationText.TryGetValue(id, out String translation) && !String.IsNullOrEmpty(translation))
                result += " - " + translation;
            return result;
        }

        public static String RemoveOpCode(String str)
        {
            String pattern = @"\[[^\]]*\]|\{[^\}]*\}";
            return Regex.Replace(str, pattern, String.Empty);
        }

        public static String[] GetTableText(UInt32 index)
        {
            if (DisplayBatch.tableText.TryGetValue(index, out String[] table))
                return table;
            if (!DisplayBatch.fieldText.TryGetValue((Int32)index, out String rawText))
                return null;
            table = DialogBoxSymbols.ParseTextSplitTags(rawText).ToArray();
            DisplayBatch.tableText[index] = table;
            return table;
        }

        public static String GetDialogCaptionText(Dialog.CaptionType captionType)
        {
            switch (captionType)
            {
                case Dialog.CaptionType.Mognet:
                case Dialog.CaptionType.ActiveTimeEvent:
                case Dialog.CaptionType.Chocobo:
                case Dialog.CaptionType.Notice:
                    return Localization.Get(captionType.ToString());
            }
            return String.Empty;
        }

        public static event Action<Int32> FieldTextUpdated;

        private static Int32 fieldZoneId = -1;
        private static Int32 battleZoneId = -1;
        private static Boolean loadSecondaryZone = false;

        private static Dictionary<String, String> sharedTexts = new Dictionary<String, String>();

        public static TextBatch MainBatch = new TextBatch();
        public static TextBatch SecondaryBatch = new TextBatch();
        public static TextBatch DisplayBatch => Localization.UseSecondaryLanguage ? SecondaryBatch : MainBatch;
        public static TextBatch LoadingZoneBatch => loadSecondaryZone ? SecondaryBatch : MainBatch;

        public static Dictionary<CharacterId, String> characterNames = null;

        public static Boolean IsLoading = false;
        public static Byte ChocographNameStartIndex = 10;
        public static Byte ChocographDetailStartIndex = 58;
        public static Byte ChocographHelpStartIndex = 34;

        public static void LoadSecondaryLanguage(String symbol)
        {
            EmbadedTextResources.CurrentSymbol = symbol;
            loadSecondaryZone = true;
            UpdateTextLocalizationNow();
            if (battleZoneId != -1)
                FF9TextTool.BattleImporter.LoadSync();
            if (fieldZoneId != -1)
                FF9TextTool.FieldImporter.LoadSync();
            loadSecondaryZone = false;
            EmbadedTextResources.CurrentSymbol = null;
        }

        public static void SetSupportAbilityName(SupportAbility id, String value)
        {
            DisplayBatch.supportAbilityName[id] = TextPatcher.PatchDatabaseString(value, typeof(SupportAbility).Name, (Int32)id, true, false);
        }

        public static void SetSupportAbilityHelpDesc(SupportAbility id, String value)
        {
            DisplayBatch.supportAbilityHelpDesc[id] = TextPatcher.PatchDatabaseString(value, typeof(SupportAbility).Name, (Int32)id, false, true);
        }

        public static void SetFieldText(String[] value)
        {
            LoadingZoneBatch.fieldText.Clear();
            FF9TextTool.ImportArrayToDictionary<Int32>(value, FF9TextTool.SetFieldText);
        }

        public static void SetFieldText(Int32 id, String value)
        {
            LoadingZoneBatch.fieldText[id] = value;
            FieldTextUpdated?.Invoke(id);
        }

        public static void ClearTableText()
        {
            MainBatch.tableText.Clear();
            SecondaryBatch.tableText.Clear();
        }

        public static void SetBattleText(String[] value)
        {
            LoadingZoneBatch.battleText.Clear();
            FF9TextTool.ImportArrayToDictionary<Int32>(value, FF9TextTool.SetBattleText);
        }

        public static void SetBattleText(Int32 id, String value)
        {
            LoadingZoneBatch.battleText[id] = value;
        }

        public static void SetCommandName(BattleCommandId id, String value)
        {
            DisplayBatch.commandName[id] = TextPatcher.PatchDatabaseString(value, typeof(BattleCommandId).Name, (Int32)id, true, false);
        }

        public static void SetCommandHelpDesc(BattleCommandId id, String value)
        {
            DisplayBatch.commandHelpDesc[id] = TextPatcher.PatchDatabaseString(value, typeof(BattleCommandId).Name, (Int32)id, false, true);
        }

        public static void SetCmdTitleText(Int32 id, String value)
        {
            DisplayBatch.cmdTitleText[id] = value;
        }

        public static void SetFollowText(Int32 id, String value)
        {
            DisplayBatch.followText[id] = value;
        }

        public static void SetCardLvName(Int32 id, String value)
        {
            DisplayBatch.cardLvName[id] = value;
        }

        public static void SetCardName(Int32 id, String value)
        {
            DisplayBatch.cardName[id] = TextPatcher.PatchDatabaseString(value, typeof(TetraMasterCardId).Name, id, true, false);
        }

        public static void SetCardName(TetraMasterCardId id, String value)
        {
            SetCardName((Int32)id, value);
        }

        public static void SetChocoUiText(Int32 id, String value)
        {
            DisplayBatch.chocoUIText[id] = value;
        }

        public static void SetLibraText(Int32 id, String value)
        {
            DisplayBatch.libraText[id] = value;
        }

        public static void SetWorldLocationText(Int32 id, String value)
        {
            DisplayBatch.worldLocationText[id] = value;
        }

        public static void SetItemName(RegularItem id, String value)
        {
            DisplayBatch.itemName[id] = TextPatcher.PatchDatabaseString(value, typeof(RegularItem).Name, (Int32)id, true, false);
        }

        public static void SetItemHelpDesc(RegularItem id, String value)
        {
            DisplayBatch.itemHelpDesc[id] = TextPatcher.PatchDatabaseString(value, typeof(RegularItem).Name, (Int32)id, false, true);
        }

        public static void SetItemBattleDesc(RegularItem id, String value)
        {
            DisplayBatch.itemBattleDesc[id] = TextPatcher.PatchDatabaseString(value, typeof(RegularItem).Name, (Int32)id, false, true);
        }

        public static void SetImportantItemName(Int32 id, String value)
        {
            DisplayBatch.importantItemName[id] = TextPatcher.PatchDatabaseString(value, "KeyItem", id, true, false);
        }

        public static void SetImportantItemHelpDesc(Int32 id, String value)
        {
            DisplayBatch.importantItemHelpDesc[id] = TextPatcher.PatchDatabaseString(value, "KeyItem", id, false, true);
        }

        public static void SetImportantSkinDesc(Int32 id, String value)
        {
            DisplayBatch.importantSkinDesc[id] = TextPatcher.PatchDatabaseString(value, "KeyItem", id, false, false);
        }

        public static void SetActionAbilityName(BattleAbilityId id, String value)
        {
            DisplayBatch.actionAbilityName[id] = TextPatcher.PatchDatabaseString(value, typeof(BattleAbilityId).Name, (Int32)id, true, false);
        }

        public static void SetActionAbilityHelpDesc(BattleAbilityId id, String value)
        {
            DisplayBatch.actionAbilityHelpDesc[id] = TextPatcher.PatchDatabaseString(value, typeof(BattleAbilityId).Name, (Int32)id, false, true);
        }

        public static void SetCharacterNames(Dictionary<CharacterId, String> value)
        {
            characterNames = value;
        }

        public static void ChangeCharacterName(CharacterId charId, String value)
        {
            if (characterNames == null)
                characterNames = CharacterNamesFormatter.CharacterDefaultNames();
            characterNames[charId] = value;
        }

        private static void ProcessEntryId(ref String entry, ref Int32 id)
        {
            if (entry.StartsWith("[TXID=") && entry.Contains("]"))
            {
                Int32 endPos = entry.IndexOf(']');
                Int32.TryParse(entry.Substring(6, endPos - 6), out id);
                entry = entry.Substring(endPos + 1);
            }
        }

        private static String[] DELIM_TEXTID = ["[TXID="];
        private static String[] DELIM_STRT = ["[STRT="];

        public struct TextBatch
        {
            public TextBatch() { }

            public void UpdateFieldZone(Int32 zoneId, String symbol)
            {
                fieldZoneId = zoneId;
                fieldLangSymbol = symbol;
            }

            public Int32 fieldZoneId = -1;
            public String fieldLangSymbol = String.Empty;
            public Dictionary<Int32, String> fieldText = new Dictionary<Int32, String>();
            public Dictionary<Int32, String> battleText = new Dictionary<Int32, String>();

            public Dictionary<RegularItem, String> itemName = new Dictionary<RegularItem, String>();
            public Dictionary<RegularItem, String> itemBattleDesc = new Dictionary<RegularItem, String>();
            public Dictionary<RegularItem, String> itemHelpDesc = new Dictionary<RegularItem, String>();

            public Dictionary<Int32, String> importantSkinDesc = new Dictionary<Int32, String>();
            public Dictionary<Int32, String> importantItemHelpDesc = new Dictionary<Int32, String>();
            public Dictionary<Int32, String> importantItemName = new Dictionary<Int32, String>();

            public Dictionary<SupportAbility, String> supportAbilityHelpDesc = new Dictionary<SupportAbility, String>();
            public Dictionary<SupportAbility, String> supportAbilityName = new Dictionary<SupportAbility, String>();

            public Dictionary<BattleAbilityId, String> actionAbilityName = new Dictionary<BattleAbilityId, String>();
            public Dictionary<BattleAbilityId, String> actionAbilityHelpDesc = new Dictionary<BattleAbilityId, String>();

            public Dictionary<BattleCommandId, String> commandName = new Dictionary<BattleCommandId, String>();
            public Dictionary<BattleCommandId, String> commandHelpDesc = new Dictionary<BattleCommandId, String>();

            public Dictionary<Int32, String> cardName = new Dictionary<Int32, String>();
            public Dictionary<Int32, String> chocoUIText = new Dictionary<Int32, String>();
            public Dictionary<Int32, String> cardLvName = new Dictionary<Int32, String>();
            public Dictionary<Int32, String> followText = new Dictionary<Int32, String>();
            public Dictionary<Int32, String> cmdTitleText = new Dictionary<Int32, String>();
            public Dictionary<Int32, String> libraText = new Dictionary<Int32, String>();
            public Dictionary<Int32, String> worldLocationText = new Dictionary<Int32, String>();

            public Dictionary<UInt32, String[]> tableText = new Dictionary<UInt32, String[]>();
            public Dictionary<Int32, String> locationName = new Dictionary<Int32, String>();
        }
    }
}
