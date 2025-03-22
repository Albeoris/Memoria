using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
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

        public static Int32 FieldZoneId => fieldZoneId;
        public static Int32 BattleZoneId => battleZoneId;
        public static Dictionary<Int32, String> LocationNames => locationName;

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
            foreach (String textFile in AssetManager.LoadStringMultiple(assetPath).Reverse())
                if (!String.IsNullOrEmpty(textFile))
                    FF9TextTool.ExtractSentense(dict, textFile);
        }

        public static String[] GetBattleText(Int32 battleZoneId)
        {
            return EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.GetCurrentPath("/Battle/" + battleZoneId + ".mes"));
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

            IsLoading = false;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
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
            if (FF9StateSystem.Settings.CurrentLanguage == null)
                FF9StateSystem.Settings.CurrentLanguage = FF9StateSystem.Settings.GetSystemLanguage();

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
            FF9TextTool.locationName.Clear();
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
            yield break;
        }

        public IEnumerator UpdateFieldText(Int32 _zoneId)
        {
            FF9TextTool.fieldZoneId = _zoneId;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
            yield return base.StartCoroutine(FF9TextTool.InitializeFieldText());
            yield break;
        }

        public Boolean UpdateFieldTextNow(Int32 _zoneId)
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

        public Boolean UpdateBattleTextNow(Int32 _zoneId)
        {
            FF9TextTool.battleZoneId = _zoneId;
            return FF9TextTool.BattleImporter.LoadSync();
        }

        public static String ItemName(RegularItem id)
        {
            return FF9TextTool.itemName.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ItemHelpDescription(RegularItem id)
        {
            return FF9TextTool.itemHelpDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ItemBattleDescription(RegularItem id)
        {
            return FF9TextTool.itemBattleDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ImportantItemName(Int32 id)
        {
            return FF9TextTool.importantItemName.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ImportantItemHelpDescription(Int32 id)
        {
            return FF9TextTool.importantItemHelpDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ImportantItemSkin(Int32 id)
        {
            return FF9TextTool.importantSkinDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ActionAbilityName(BattleAbilityId id)
        {
            return FF9TextTool.actionAbilityName.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ActionAbilityHelpDescription(BattleAbilityId id)
        {
            return FF9TextTool.actionAbilityHelpDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String SupportAbilityName(SupportAbility id)
        {
            return FF9TextTool.supportAbilityName.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String SupportAbilityHelpDescription(SupportAbility id)
        {
            return FF9TextTool.supportAbilityHelpDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String CommandName(BattleCommandId id)
        {
            Boolean isValid = FF9TextTool.commandName.TryGetValue(id, out String name);
            if (id == BattleCommandId.AccessMenu && isValid)
                if (name == "None" || name == "みてい")
                    return Localization.Get("Menu");
            return isValid ? name : String.Empty;
        }

        public static String CommandHelpDescription(BattleCommandId id)
        {
            if (id == BattleCommandId.AccessMenu && FF9TextTool.commandHelpDesc.TryGetValue(id, out String help))
                if (help == "" || help == "みていを使います。")
                    return Localization.Get("Menu");
            return FF9TextTool.commandHelpDesc.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String FieldText(Int32 textId)
        {
            return FF9TextTool.fieldText.TryGetValue(textId, out String result) ? TextOpCodeModifier.Modify(result) : String.Empty;
        }

        public static String CharacterProfile(Int32 charId)
        {
            return Localization.Get("CharacterProfile" + charId);
        }

        public static String LocationName(Int32 id)
        {
            return FF9TextTool.locationName.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static Boolean IsBattleTextLoaded => FF9TextTool.battleText != null;

        public static String BattleText(Int32 id)
        {
            return FF9TextTool.battleText.TryGetValue(id, out String result) ? result : Localization.Get("EnemyDummy");
        }

        public static String CardName(TetraMasterCardId id)
        {
            return FF9TextTool.cardName.TryGetValue(id, out String result) ? result : String.Empty;
        }

        public static String ChocoboUIText(Int32 id)
        {
            return (id >= FF9TextTool.chocoUIText.Length) ? String.Empty : FF9TextTool.chocoUIText[id];
        }

        public static String CardLevelName(Int32 id)
        {
            return (id >= FF9TextTool.cardLvName.Length) ? String.Empty : FF9TextTool.cardLvName[id];
        }

        public static String BattleFollowText(Int32 id)
        {
            return (id >= FF9TextTool.followText.Length) ? String.Empty : FF9TextTool.followText[id];
        }

        public static String BattleLibraText(Int32 id)
        {
            return (id >= FF9TextTool.libraText.Length) ? String.Empty : FF9TextTool.libraText[id];
        }

        public static String BattleCommandTitleText(Int32 id)
        {
            return (id >= FF9TextTool.cmdTitleText.Length) ? String.Empty : FF9TextTool.cmdTitleText[id];
        }

        public static String WorldLocationText(Int32 id)
        {
            return (id >= FF9TextTool.worldLocationText.Length) ? String.Empty : FF9TextTool.worldLocationText[id];
        }

        public static String RemoveOpCode(String str)
        {
            String pattern = @"\[[^\]]*\]|\{[^\}]*\}";
            return Regex.Replace(str, pattern, String.Empty);
        }

        public static String[] GetTableText(UInt32 index)
        {
            if (FF9TextTool.tableText.TryGetValue(index, out String[] table))
                return table;
            if (!FF9TextTool.fieldText.TryGetValue((Int32)index, out String rawText))
                return null;
            table = DialogBoxSymbols.ParseTextSplitTags(rawText).ToArray();
            FF9TextTool.tableText[index] = table;
            return table;
        }

        public static String GetDialogCaptionText(Dialog.CaptionType captionType)
        {
            String result = String.Empty;
            switch (captionType)
            {
                case Dialog.CaptionType.Mognet:
                case Dialog.CaptionType.ActiveTimeEvent:
                case Dialog.CaptionType.Chocobo:
                case Dialog.CaptionType.Notice:
                    result = Localization.Get(captionType.ToString());
                    break;
            }
            return result;
        }

        public static event Action<Int32> FieldTextUpdated;

        private static Int32 fieldZoneId = -1;
        private static Int32 battleZoneId = -1;
        public static Dictionary<Int32, String> fieldText = new Dictionary<Int32, String>();
        public static Dictionary<Int32, String> battleText = new Dictionary<Int32, String>();

        public static Dictionary<RegularItem, String> itemName = new Dictionary<RegularItem, String>();
        public static Dictionary<RegularItem, String> itemBattleDesc = new Dictionary<RegularItem, String>();
        public static Dictionary<RegularItem, String> itemHelpDesc = new Dictionary<RegularItem, String>();

        public static Dictionary<Int32, String> importantSkinDesc = new Dictionary<Int32, String>();
        public static Dictionary<Int32, String> importantItemHelpDesc = new Dictionary<Int32, String>();
        public static Dictionary<Int32, String> importantItemName = new Dictionary<Int32, String>();

        public static Dictionary<SupportAbility, String> supportAbilityHelpDesc = new Dictionary<SupportAbility, String>();
        public static Dictionary<SupportAbility, String> supportAbilityName = new Dictionary<SupportAbility, String>();

        public static Dictionary<BattleAbilityId, String> actionAbilityName = new Dictionary<BattleAbilityId, String>();
        public static Dictionary<BattleAbilityId, String> actionAbilityHelpDesc = new Dictionary<BattleAbilityId, String>();

        public static Dictionary<CharacterId, String> characterNames;

        public static Dictionary<BattleCommandId, String> commandName = new Dictionary<BattleCommandId, String>();
        public static Dictionary<BattleCommandId, String> commandHelpDesc = new Dictionary<BattleCommandId, String>();

        public static Dictionary<TetraMasterCardId, String> cardName = new Dictionary<TetraMasterCardId, String>();
        public static String[] chocoUIText;
        public static String[] cardLvName;
        public static String[] followText;
        public static String[] cmdTitleText;
        public static String[] libraText;
        public static String[] worldLocationText;

        private static Dictionary<UInt32, String[]> tableText = new Dictionary<UInt32, String[]>();

        public static Dictionary<Int32, String> locationName = new Dictionary<Int32, String>();

        public static Boolean IsLoading = false;
        public static Byte ChocographNameStartIndex = 10;
        public static Byte ChocographDetailStartIndex = 58;
        public static Byte ChocographHelpStartIndex = 34;

        public static void SetSupportAbilityName(SupportAbility id, String value)
        {
            supportAbilityName[id] = TextPatcher.PatchDatabaseString(value, typeof(SupportAbility).Name, (Int32)id, true, false);
        }

        public static void SetSupportAbilityHelpDesc(SupportAbility id, String value)
        {
            supportAbilityHelpDesc[id] = TextPatcher.PatchDatabaseString(value, typeof(SupportAbility).Name, (Int32)id, false, true);
        }

        public static void SetFieldText(String[] value)
        {
            fieldText.Clear();
            FF9TextTool.ImportArrayToDictionary<Int32>(value, FF9TextTool.SetFieldText);
        }

        public static void SetFieldText(Int32 id, String value)
        {
            fieldText[id] = value;
            FieldTextUpdated?.Invoke(id);
        }

        public static void ClearTableText()
        {
            tableText.Clear();
        }

        public static void SetBattleText(String[] value)
        {
            battleText.Clear();
            FF9TextTool.ImportArrayToDictionary<Int32>(value, FF9TextTool.SetBattleText);
        }

        public static void SetBattleText(Int32 id, String value)
        {
            battleText[id] = value;
        }

        public static void SetCommandName(BattleCommandId id, String value)
        {
            commandName[id] = TextPatcher.PatchDatabaseString(value, typeof(BattleCommandId).Name, (Int32)id, true, false);
        }

        public static void SetCommandHelpDesc(BattleCommandId id, String value)
        {
            commandHelpDesc[id] = TextPatcher.PatchDatabaseString(value, typeof(BattleCommandId).Name, (Int32)id, false, true);
        }

        public static void SetCmdTitleText(String[] value)
        {
            cmdTitleText = value;
        }

        public static void SetFollowText(String[] value)
        {
            followText = value;
        }

        public static void SetCardLvName(String[] value)
        {
            cardLvName = value;
        }

        public static void SetCardName(String[] value)
        {
            FF9TextTool.ImportArrayToDictionary<TetraMasterCardId>(value, FF9TextTool.SetCardName);
        }

        public static void SetCardName(TetraMasterCardId id, String value)
        {
            cardName[id] = TextPatcher.PatchDatabaseString(value, typeof(TetraMasterCardId).Name, (Int32)id, true, false);
        }

        public static void SetChocoUiText(String[] value)
        {
            chocoUIText = value;
        }

        public static void SetLibraText(String[] value)
        {
            libraText = value;
        }

        public static void SetWorldLocationText(String[] value)
        {
            worldLocationText = value;
        }

        public static void SetItemName(RegularItem id, String value)
        {
            itemName[id] = TextPatcher.PatchDatabaseString(value, typeof(RegularItem).Name, (Int32)id, true, false);
        }

        public static void SetItemHelpDesc(RegularItem id, String value)
        {
            itemHelpDesc[id] = TextPatcher.PatchDatabaseString(value, typeof(RegularItem).Name, (Int32)id, false, true);
        }

        public static void SetItemBattleDesc(RegularItem id, String value)
        {
            itemBattleDesc[id] = TextPatcher.PatchDatabaseString(value, typeof(RegularItem).Name, (Int32)id, false, true);
        }

        public static void SetImportantItemName(Int32 id, String value)
        {
            importantItemName[id] = TextPatcher.PatchDatabaseString(value, "KeyItem", id, true, false);
        }

        public static void SetImportantItemHelpDesc(Int32 id, String value)
        {
            importantItemHelpDesc[id] = TextPatcher.PatchDatabaseString(value, "KeyItem", id, false, true);
        }

        public static void SetImportantSkinDesc(Int32 id, String value)
        {
            importantSkinDesc[id] = TextPatcher.PatchDatabaseString(value, "KeyItem", id, false, false);
        }

        public static void SetActionAbilityName(BattleAbilityId id, String value)
        {
            actionAbilityName[id] = TextPatcher.PatchDatabaseString(value, typeof(BattleAbilityId).Name, (Int32)id, true, false);
        }

        public static void SetActionAbilityHelpDesc(BattleAbilityId id, String value)
        {
            actionAbilityHelpDesc[id] = TextPatcher.PatchDatabaseString(value, typeof(BattleAbilityId).Name, (Int32)id, false, true);
        }

        public static void SetCharacterNames(Dictionary<CharacterId, String> value)
        {
            characterNames = value;
        }

        public static void ChangeCharacterName(CharacterId charId, String value)
        {
            if (FF9StateSystem.Settings.CurrentLanguage == null)
                FF9StateSystem.Settings.CurrentLanguage = FF9StateSystem.Settings.GetSystemLanguage();
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
    }
}
