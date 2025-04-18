using Memoria.Prime;
using Memoria.Prime.CSV;
using Memoria.Prime.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Memoria.Assets
{
    internal sealed class LanguageMap
    {
        public const String LanguageKey = "KEY";
        public const String SymbolKey = "Symbol";
        public const String ReadingDirectionKey = "ReadingDirection";
        public const String DigitShapesKey = "DigitShapes";

        private readonly String[] _knownLanguages;
        private readonly Dictionary<String, SortedList<String, String>> _languages;
        private readonly SortedList<String, String> _failback;
        private readonly String _failbackLanguage;

        private String _currentLanguage;
        private String _currentSymbol;
        private SortedList<String, String> _current;
        private SortedList<String, String> _secondary;

        public LanguageMap()
        {
            TextCSVReader reader = new TextCSVReader(ReadEmbeddedTable());
            List<String> cells = reader.ReadCSV();
            if (cells.Count < 2 || cells[0] != LanguageKey)
                throw new CsvParseException("Invalid localisation file.");

            Int32 languageCount = cells.Count - 1;
            Dictionary<Int32, String> cellLanguages = new Dictionary<Int32, String>(languageCount);

            _languages = new Dictionary<String, SortedList<String, String>>(languageCount);
            _knownLanguages = new String[languageCount];

            for (Int32 i = 1; i < cells.Count; i++)
            {
                String language = cells[i];
                cellLanguages.Add(i, language);

                _knownLanguages[i - 1] = language;

                SortedList<String, String> dic = new SortedList<String, String>();
                dic.Add(LanguageKey, language);

                _languages.Add(language, dic);
            }

            ReadText(reader, cellLanguages, true);

            _failbackLanguage = cellLanguages[1];
            _failback = _languages[_failbackLanguage];
            _current = _failback;
            _secondary = _failback;

            LoadModText(cellLanguages);
            LoadExternalText();
        }

        public String CurrentLanguage => _currentLanguage;
        public String CurrentSymbol => _currentSymbol;
        public ICollection<String> KnownLanguages => _knownLanguages;

        public SortedList<String, String> ProvideDictionary(String symbol)
        {
            if (symbol == _currentSymbol)
                return _current;

            return _languages.Values.FirstOrDefault(languageDic => languageDic[SymbolKey] == symbol);
        }

        public void Broadcast()
        {
            SelectLanguage(LanguagePrefs.Key);
            UIRoot.Broadcast("OnLocalize");
        }

        public void SelectSecondaryLanguage(String language)
        {
            if (_languages.TryGetValue(language, out var languageDic))
            {
                _secondary = languageDic;
            }
            else
            {
                _secondary = _failback;
                Log.Error($"[LocalizationDictionary] Cannot find localisation data for the language [{language}].");
            }
        }

        public void SelectLanguage(String language)
        {
            if (_languages.TryGetValue(language, out var languageDic))
            {
                _current = languageDic;
                _currentLanguage = language;
                LanguagePrefs.Key = language;
            }
            else
            {
                _current = _failback;
                _currentLanguage = _failbackLanguage;
                LanguagePrefs.Key = null;
                Log.Error($"[LocalizationDictionary] Cannot find localisation data for the language [{language}].");
            }
            _currentSymbol = Get(SymbolKey);
            NGUIText.readingDirection = Localization.GetWithDefault(ReadingDirectionKey) == UnicodeBIDI.DIRECTION_NAME_RIGHT_TO_LEFT ? UnicodeBIDI.LanguageReadingDirection.RightToLeft : UnicodeBIDI.LanguageReadingDirection.LeftToRight;
            if (!Localization.GetWithDefault(DigitShapesKey).TryEnumParse(out NGUIText.digitShapes))
                NGUIText.digitShapes = UnicodeBIDI.DigitShapes.Latin;
        }

        private Boolean TryGetAlternativeKey(String key, out String alternativeKey)
        {
            switch (UICamera.currentScheme)
            {
                case UICamera.ControlScheme.Touch:
                    alternativeKey = key + " Mobile";
                    return true;
                case UICamera.ControlScheme.Controller:
                    alternativeKey = key + " Controller";
                    return true;
            }

            alternativeKey = null;
            return false;
        }

        public String Get(String key, Boolean secondaryLang = false)
        {
            SortedList<String, String> dict = secondaryLang ? _secondary : _current;
            if (TryGetAlternativeKey(key, out var alternativeKey) && dict.TryGetValue(alternativeKey, out var alternativeValue))
                return alternativeValue;
            if (dict.TryGetValue(key, out var value))
                return value;
            return key;
        }

        private void ReadText(TextCSVReader reader, Dictionary<Int32, String> cellLanguages, Boolean init)
        {
            Boolean firstEntry = true;
            while (reader.HasMoreEntries)
            {
                List<String> cells = reader.ReadCSV();
                if (cells == null || cells.Count < 2)
                    continue;

                String key = cells[0];
                if (String.IsNullOrEmpty(key))
                    continue;

                if (firstEntry && !init && key == LanguageKey)
                {
                    Dictionary<Int32, String> customLayout = new Dictionary<Int32, String>();
                    for (Int32 i = 1; i < cells.Count; i++)
                    {
                        String value = cells[i];
                        foreach (String language in cellLanguages.Values)
                        {
                            if (_languages[language][LanguageKey] == value)
                            {
                                customLayout.Add(i, language);
                                break;
                            }
                        }
                    }
                    cellLanguages = customLayout;
                }
                else
                {
                    for (Int32 i = 1; i < cells.Count; i++)
                    {
                        String value = cells[i];
                        if (!cellLanguages.TryGetValue(i, out String language))
                            continue;
                        if (init)
                            StoreValue(language, key, value);
                        else
                            _languages[language][key] = value;
                    }
                }
                firstEntry = false;
            }
        }

        private void LoadModText(Dictionary<Int32, String> cellLanguages)
        {
            String inputPath = DataResources.Text.PureDirectory + DataResources.Text.LocalizationPatchFile;
            foreach (AssetManager.AssetFolder folder in AssetManager.FolderLowToHigh)
            {
                if (folder.TryFindAssetInModOnDisc(inputPath, out String fullPath, AssetManagerUtil.GetStreamingAssetsPath() + "/"))
                {
                    TextCSVReader reader = TextCSVReader.Open(fullPath);
                    if (reader != null)
                        ReadText(reader, cellLanguages, false);
                }
            }
        }

        private void LoadExternalText()
        {
            if (!Configuration.Import.Text)
                return;

            foreach (SortedList<String, String> languageDic in _languages.Values)
            {
                if (!languageDic.TryGetValue(SymbolKey, out String symbol))
                    continue;

                TextResourceReference importReference = ModTextResources.Import.GetSymbolPath(symbol, ModTextResources.SystemReference);
                if (!importReference.IsExists(out TextResourcePath path))
                {
                    Log.Warning($"[LocalizationDictionary] Loading was skipped because a file does not exists: [{importReference}]...");
                    return;
                }

                Log.Message($"[LocalizationDictionary] Loading from [{importReference}]...");

                TxtEntry[] entries = path.ReadAll();
                foreach (TxtEntry entry in entries)
                {
                    if (entry == null)
                        continue;

                    switch (entry.Prefix)
                    {
                        case LanguageKey:
                        case SymbolKey:
                        case "Name":
                        case "":
                            continue;
                    }

                    languageDic[entry.Prefix] = entry.Value;
                }
            }
            Log.Message("[LocalizationDictionary] Loading completed successfully.");
        }

        private static String[] ReadEmbeddedTable()
        {
            return AssetManager.LoadString("EmbeddedAsset/Manifest/Text/Localization.txt").Split('\n');
        }

        private void StoreValue(String language, String key, String value)
        {
            SortedList<String, String> target = _languages[language];
            try
            {
                target.Add(key, value);
            }
            catch (ArgumentException)
            {
                Debug.LogWarning("Localization key '" + key + "' is already present");
            }
            catch (Exception ex)
            {
                Debug.LogError("Unable to add '" + key + "' to the Localization dictionary.\n" + ex.Message);
            }
        }

        public String SymbolToLanguage(String symbol)
        {
            foreach (var kvp in _languages)
                if (kvp.Value.TryGetValue(SymbolKey, out String langSymbol) && symbol == langSymbol)
                    return kvp.Key;
            return LanguageName.EnglishUS;
        }

        public String LanguageToSymbol(String lang)
        {
            if (_languages.TryGetValue(lang, out SortedList<String, String> dict) && dict.TryGetValue(SymbolKey, out String symbol))
                return symbol;
            return LanguageName.SymbolEnglishUS;
        }

        private static class LanguagePrefs
        {
            private const String DefaultLanguage = LanguageName.EnglishUS;

            public static String Key
            {
                get => PlayerPrefs.GetString("Language", DefaultLanguage);
                set
                {
                    if (value == null)
                        PlayerPrefs.DeleteKey("Language");
                    else
                        PlayerPrefs.SetString("Language", value);
                }
            }
        }
    }
}
