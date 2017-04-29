using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Memoria.Assets;
using Memoria.Prime;
using UnityEngine;

namespace Assets.Sources.Scripts.UI.Common
{
    public class FF9TextTool : PersistenSingleton<FF9TextTool>
    {
        public static Color White => new Color(0.784313738f, 0.784313738f, 0.784313738f);
        public static Color Gray => new Color(0.5647059f, 0.5647059f, 0.5647059f);
        public static Color Green => new Color(0.470588237f, 0.784313738f, 0.2509804f);
        public static Color Cyan => new Color(0.407843143f, 0.7529412f, 0.847058833f);
        public static Color Red => new Color(0.8156863f, 0.3764706f, 0.3137255f);
        public static Color Yellow => new Color(0.784313738f, 0.6901961f, 0.2509804f);
        public static Color Magenta => new Color(0.721568644f, 0.5019608f, 0.8784314f);

        public static Int32 FieldZoneId => fieldZoneId;
        public static Int32 BattleZoneId => battleZoneId;
        public static Dictionary<Int32, String> LocationNames => locationName;

        public static readonly BattleImporter BattleImporter = new BattleImporter();
        public static readonly FieldImporter FieldImporter = new FieldImporter();

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

        //private static readonly HashSet<int> zones = new HashSet<int>();
        private static IEnumerable InitializeFieldTextInternal()
        {
            PersistenSingleton<UIManager>.Instance.SetEventEnable(false);

            if (fieldZoneId == -1)
            {
                PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
                yield return 0;
                yield break;
            }

            //if (zones.Add(fieldZoneId))
            //    Log.Message($"InitializeFieldTextInternal: {fieldZoneId}");
            IsLoading = true;

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
            //    FF9TextTool.SetFieldText(text);
            //    FF9TextTool.SetTableText(ExtractTableText(text));
            //}

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

        public static String[] ExtractSentense(String text)
        {
            String[] strArray1 = Regex.Split(text, "\\[STRT\\=");
            String[] strArray2 = new String[strArray1.Length - 1];
            for (Int32 index = 1; index < strArray1.Length; ++index)
                strArray2[index - 1] = "[STRT=" + strArray1[index];
            return strArray2;
        }

        public static String[][] ExtractTableText(String[] extactedList)
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
            if (setMenuLanguageCallback != null)
            {
                setMenuLanguageCallback();
            }
            yield break;
        }

        public IEnumerator UpdateFieldText(Int32 _zoneId)
        {
            FF9TextTool.fieldZoneId = _zoneId;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
            yield return base.StartCoroutine(FF9TextTool.InitializeFieldText());
            yield break;
        }

        public IEnumerator UpdateBattleText(Int32 _zoneId)
        {
            FF9TextTool.battleZoneId = _zoneId;
            PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
            yield return base.StartCoroutine(FF9TextTool.InitializeBattleText());
            yield break;
        }

        public static String ItemName(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.itemName.Length) ? String.Empty : FF9TextTool.itemName[id];
        }

        public static String ItemHelpDescription(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.itemHelpDesc.Length) ? String.Empty : FF9TextTool.itemHelpDesc[id];
        }

        public static String ItemBattleDescription(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.itemBattleDesc.Length) ? String.Empty : FF9TextTool.itemBattleDesc[id];
        }

        public static String ImportantItemName(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.importantItemName.Length) ? String.Empty : FF9TextTool.importantItemName[id];
        }

        public static String ImportantItemHelpDescription(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.importantItemHelpDesc.Length) ? String.Empty : FF9TextTool.importantItemHelpDesc[id];
        }

        public static String ImportantItemSkin(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.importantSkinDesc.Length) ? String.Empty : FF9TextTool.importantSkinDesc[id];
        }

        public static String ActionAbilityName(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.actionAbilityName.Length) ? String.Empty : FF9TextTool.actionAbilityName[id];
        }

        public static String ActionAbilityHelpDescription(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.actionAbilityHelpDesc.Length) ? String.Empty : FF9TextTool.actionAbilityHelpDesc[id];
        }

        public static String SupportAbilityName(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.supportAbilityName.Length) ? String.Empty : FF9TextTool.supportAbilityName[id];
        }

        public static String SupportAbilityHelpDescription(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.supportAbilityHelpDesc.Length) ? String.Empty : FF9TextTool.supportAbilityHelpDesc[id];
        }

        public static String CommandName(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.commandName.Length) ? String.Empty : FF9TextTool.commandName[id];
        }

        public static String CommandHelpDescription(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.commandHelpDesc.Length) ? String.Empty : FF9TextTool.commandHelpDesc[id];
        }

        public static String FieldText(Int32 textId)
        {
            return (textId >= (Int32)FF9TextTool.fieldText.Length) ? String.Empty : FF9TextTool.fieldText[textId];
        }

        public static String CharacterProfile(Int32 charId)
        {
            return Localization.Get("CharacterProfile" + charId);
        }

        public static String CharacterDefaultName(Int32 id)
        {
            String[] array = null;
            if (FF9StateSystem.Settings.CurrentLanguage == null)
            {
                FF9StateSystem.Settings.CurrentLanguage = FF9StateSystem.Settings.GetSystemLanguage();
            }
            String currentLanguage = FF9StateSystem.Settings.CurrentLanguage;
            switch (currentLanguage)
            {
                case "English(US)":
                    array = new String[]
                    {
                        "Zidane",
                        "Vivi",
                        "Dagger",
                        "Steiner",
                        "Freya",
                        "Quina",
                        "Eiko",
                        "Amarant",
                        "Cinna",
                        "Cinna",
                        "Marcus",
                        "Marcus",
                        "Blank",
                        "Blank",
                        "Beatrix",
                        "Beatrix",
                        "Zidane",
                        "Cinna",
                        "Marcus",
                        "Blank"
                    };
                    break;
                case "English(UK)":
                    array = new String[]
                    {
                        "Zidane",
                        "Vivi",
                        "Dagger",
                        "Steiner",
                        "Freya",
                        "Quina",
                        "Eiko",
                        "Amarant",
                        "Cinna",
                        "Cinna",
                        "Marcus",
                        "Marcus",
                        "Blank",
                        "Blank",
                        "Beatrix",
                        "Beatrix",
                        "Zidane",
                        "Cinna",
                        "Marcus",
                        "Blank"
                    };
                    break;
                case "Japanese":
                    array = new String[]
                    {
                        "ジタン",
                        "ビビ",
                        "ダガー",
                        "スタイナー",
                        "フライヤ",
                        "クイナ",
                        "エーコ",
                        "サラマンダー",
                        "シナ",
                        "シナ",
                        "マーカス",
                        "マーカス",
                        "ブランク",
                        "ブランク",
                        "ベアトリクス",
                        "ベアトリクス",
                        "ジタン",
                        "シナ",
                        "マーカス",
                        "ブランク"
                    };
                    break;
                case "German":
                    array = new String[]
                    {
                        "Zidane",
                        "Vivi",
                        "Lili",
                        "Steiner",
                        "Freya",
                        "Quina",
                        "Eiko",
                        "Mahagon",
                        "Cinna",
                        "Cinna",
                        "Marcus",
                        "Marcus",
                        "Blank",
                        "Blank",
                        "Beatrix",
                        "Beatrix",
                        "Zidane",
                        "Cinna",
                        "Marcus",
                        "Blank"
                    };
                    break;
                case "French":
                    array = new String[]
                    {
                        "Djidane",
                        "Bibi",
                        "Dagga",
                        "Steiner",
                        "Freyja",
                        "Kweena",
                        "Eiko",
                        "Tarask",
                        "Cina",
                        "Cina",
                        "Markus",
                        "Markus",
                        "Frank",
                        "Frank",
                        "Beate",
                        "Beate",
                        "Djidane",
                        "Cina",
                        "Markus",
                        "Frank"
                    };
                    break;
                case "Italian":
                    array = new String[]
                    {
                        "Gidan",
                        "Vivi",
                        "Daga",
                        "Steiner",
                        "Freija",
                        "Quina",
                        "Eiko",
                        "Amarant",
                        "Er Cina",
                        "Er Cina",
                        "Marcus",
                        "Marcus",
                        "Blank",
                        "Blank",
                        "Beatrix",
                        "Beatrix",
                        "Gidan",
                        "Er Cina",
                        "Marcus",
                        "Blank"
                    };
                    break;
                case "Spanish":
                    array = new String[]
                    {
                        "Yitán",
                        "Vivi",
                        "Daga",
                        "Steiner",
                        "Freija",
                        "Quina",
                        "Eiko",
                        "Amarant",
                        "Cinna",
                        "Cinna",
                        "Marcus",
                        "Marcus",
                        "Blank",
                        "Blank",
                        "Beatrix",
                        "Beatrix",
                        "Yitán",
                        "Cinna",
                        "Marcus",
                        "Blank"
                    };
                    break;
            }
            return (array != null) ? ((id >= (Int32)array.Length) ? String.Empty : array[id]) : String.Empty;
        }

        public static String LocationName(Int32 id)
        {
            return (!FF9TextTool.locationName.ContainsKey(id)) ? String.Empty : FF9TextTool.locationName[id];
        }

        public static Boolean IsBattleTextLoaded => FF9TextTool.battleText != null;

        public static String BattleText(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.battleText.Length) ? Localization.Get("EnemyDummy") : FF9TextTool.battleText[id];
        }

        public static String CardName(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.cardName.Length) ? String.Empty : FF9TextTool.cardName[id];
        }

        public static String ChocoboUIText(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.chocoUIText.Length) ? String.Empty : FF9TextTool.chocoUIText[id];
        }

        public static String CardLevelName(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.cardLvName.Length) ? String.Empty : FF9TextTool.cardLvName[id];
        }

        public static String BattleFollowText(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.followText.Length) ? String.Empty : FF9TextTool.followText[id];
        }

        public static String BattleLibraText(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.libraText.Length) ? String.Empty : FF9TextTool.libraText[id];
        }

        public static String BattleCommandTitleText(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.cmdTitleText.Length) ? String.Empty : FF9TextTool.cmdTitleText[id];
        }

        public static String WorldLocationText(Int32 id)
        {
            return (id >= (Int32)FF9TextTool.worldLocationText.Length) ? String.Empty : FF9TextTool.worldLocationText[id];
        }

        public static String RemoveOpCode(String textList)
        {
            String pattern = @"\[[^\]]*\]|\{[^\}]*\}";
            return Regex.Replace(textList, pattern, String.Empty);
        }

        public static String[] GetTableText(UInt32 index)
        {
            return ((UInt64)index >= (UInt64)((Int64)FF9TextTool.tableText.Length)) ? null : FF9TextTool.tableText[(Int32)((UIntPtr)index)];
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

        public static event Action FieldTextUpdated;

        private static Int32 fieldZoneId = -1;

        private static Int32 battleZoneId = -1;

        private static String[] fieldText;

        private static String[] battleText;

        private static String[] itemName;

        private static String[] itemBattleDesc;

        private static String[] itemHelpDesc;

        private static String[] importantSkinDesc;

        private static String[] importantItemHelpDesc;

        private static String[] importantItemName;

        private static String[] supportAbilityHelpDesc;

        private static String[] supportAbilityName;

        private static String[] actionAbilityName;

        private static String[] actionAbilityHelpDesc;

        private static String[] commandName;

        private static String[] commandHelpDesc;

        private static String[] cardName;

        private static String[] chocoUIText;

        private static String[] cardLvName;

        private static String[] followText;

        private static String[] cmdTitleText;

        private static String[] libraText;

        private static String[] worldLocationText;

        private static String[][] tableText;

        private static Dictionary<Int32, String> locationName = new Dictionary<Int32, String>();

        public static Boolean IsLoading = false;

        public static Byte ChocographNameStartIndex = 10;

        public static Byte ChocographDetailStartIndex = 58;

        public static Byte ChocographHelpStartIndex = 34;

        public static void SetSupportAbilityName(String[] abilityNames)
        {
            supportAbilityName = abilityNames;
        }

        public static void SetSupportAbilityHelpDesc(String[] abilityHelps)
        {
            supportAbilityHelpDesc = abilityHelps;
        }

        public static void SetFieldText(String[] value)
        {
            fieldText = value;
            FieldTextUpdated?.Invoke();
        }

        public static void SetTableText(String[][] value)
        {
            tableText = value;
        }

        public static void SetBattleText(String[] value)
        {
            battleText = value;
        }

        public static void SetCommandName(String[] value)
        {
            commandName = value;
        }

        public static void SetCommandHelpDesc(String[] value)
        {
            commandHelpDesc = value;
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
            cardName = value;
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

        public static void SetItemName(String[] value)
        {
            itemName = value;
        }

        public static void SetItemHelpDesc(String[] value)
        {
            itemHelpDesc = value;
        }

        public static void SetItemBattleDesc(String[] value)
        {
            itemBattleDesc = value;
        }

        public static void SetImportantItemName(String[] value)
        {
            importantItemName = value;
        }

        public static void SetImportantItemHelpDesc(String[] value)
        {
            importantItemHelpDesc = value;
        }

        public static void SetImportantSkinDesc(String[] value)
        {
            importantSkinDesc = value;
        }

        public static void SetActionAbilityName(String[] value)
        {
            actionAbilityName = value;
        }

        public static void SetActionAbilityHelpDesc(String[] value)
        {
            actionAbilityHelpDesc = value;
        }
    }
}