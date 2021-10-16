using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memoria.Prime;
using Memoria.Prime.CSV;
using UnityEngine;
using Object = System.Object;

namespace Memoria.Assets
{
    internal sealed class LanguageMap
    {
        private const String LanguageKey = "KEY";
        private const String SymbolKey = "Symbol";

        private readonly String[] _knownLanguages;
        private readonly Dictionary<String, SortedList<String, String>> _languages;
        private readonly SortedList<String, String> _failback;
        private readonly String _failbackLanguage;

        private String _currentLanguage;
        private String _currentSymbol;
        private SortedList<String, String> _current;

        public LanguageMap()
        {
            Byte[] tableData = ReadEmbadedTable();

            ByteReader reader = new ByteReader(tableData);
            BetterList<String> cells = reader.ReadCSV();
            if (cells.size < 2 || cells[0] != LanguageKey)
                throw new CsvParseException("Invalid localisation file.");

            Int32 languageCount = cells.size - 1;
            Dictionary<Int32, String> cellLanguages = new Dictionary<Int32, String>(languageCount);

            _languages = new Dictionary<String, SortedList<String, String>>(languageCount);
            _knownLanguages = new String[languageCount];

            for (Int32 i = 1; i < cells.size; i++)
            {
                String language = cells[i];
                cellLanguages.Add(i, language);

                _knownLanguages[i - 1] = language;

                SortedList<String, String> dic = new SortedList<String, String>();
                dic.Add(LanguageKey, language);

                _languages.Add(language, dic);
            }

            ReadText(reader, cellLanguages);

            _failbackLanguage = cellLanguages[1];
            _failback = _languages[_failbackLanguage];
            _current = _failback;

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
            UIRoot.Broadcast("OnLocalize");
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

        public String Get(String key)
        {
            if (TryGetAlternativeKey(key, out var alternativeKey))
            {
                if (TryGetValue(alternativeKey, out var alternativeValue))
                    return alternativeValue;
            }

            if (TryGetValue(key, out var value))
                return value;

            return key;
        }

        private Boolean TryGetValue(String key, out String value)
        {
            if (_current.TryGetValue(key, out value))
                return true;

            if (_failback.TryGetValue(key, out value))
                return true;

            return false;
        }

        private void ReadText(ByteReader reader, Dictionary<Int32, String> cellLanguages)
        {
            while (reader.canRead)
            {
                BetterList<String> cells = reader.ReadCSV();
                if (cells == null || cells.size < 2)
                    continue;

                String key = cells[0];
                if (String.IsNullOrEmpty(key))
                    continue;

                for (Int32 i = 1; i < cells.size; i++)
                {
                    String value = cells[i];
                    String language = cellLanguages[i];
                    StoreValue(language, key, value);
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

                String importPath = ModTextResources.Import.GetSymbolPath(symbol, ModTextResources.SystemPath);
                if (!File.Exists(importPath))
                {
                    Log.Warning($"[LocalizationDictionary] Loading was skipped because a file does not exists: [{importPath}]...");
                    return;
                }

                Log.Message($"[LocalizationDictionary] Loading from [{importPath}]...");

                TxtEntry[] entries = TxtReader.ReadStrings(importPath);
                foreach (TxtEntry entry in entries)
                {
                    if (entry == null)
                        continue;

                    switch (entry.Prefix)
                    {
                        case "KEY":
                        case "Symbol":
                        case "Name":
                        case "":
                            continue;
                    }

                    languageDic[entry.Prefix] = entry.Value;
                }
            }
            Log.Message("[LocalizationDictionary] Loading completed successfully.");
        }

        private static Byte[] ReadEmbadedTable()
        {
            return AssetManager.LoadBytes("EmbeddedAsset/Manifest/Text/Localization.txt", out _);
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

        private static class LanguagePrefs
        {
            private const String DefaultLanguage = "English(US)";

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