using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Memoria
{
    internal sealed class LocalizationDictionary
    {
        private readonly object _lock = new object();
        private Dictionary<String, String> _replacement = new Dictionary<String, String>();

        private Dictionary<String, String> _oldDictionary = new Dictionary<String, String>();
        private Dictionary<String, String[]> _dictionary = new Dictionary<String, String[]>();
        private String[] _languages;
        private Int32 _languageIndex = -1;
        private Boolean _merging;
        private String _language;

        public Int32 CurrentLanguageIndex => _languageIndex;

        public Boolean TryLoadCSV(Byte[] bytes, Boolean merge)
        {
            if (bytes == null)
                return false;

            ByteReader byteReader = new ByteReader(bytes);
            BetterList<String> betterList = byteReader.ReadCSV();
            if (betterList.size < 2)
                return false;

            betterList.RemoveAt(0);
            lock (_lock)
            {
                String[] newLanguages;
                Dictionary<String, Int32> languageIndices = PrepareLanguages(merge, betterList, out newLanguages);
                while (true)
                {
                    BetterList<string> newValues;
                    do
                    {
                        newValues = byteReader.ReadCSV();
                        if (newValues == null || newValues.size == 0)
                            goto label_33;

                    } while (string.IsNullOrEmpty(newValues[0]));

                    AddCSV(newValues, newLanguages, languageIndices);
                }

                label_33:
                if (_merging || Localization.onLocalize == null)
                    return true;

                _merging = true;

                Localization.OnLocalizeNotification localizeNotification = Localization.onLocalize;
                Localization.onLocalize = null;
                localizeNotification();
                Localization.onLocalize = localizeNotification;

                _merging = false;
                return true;
            }
        }

        public void Set(String languageName, Dictionary<String, String> dic)
        {
            lock (_lock)
            {
                _language = languageName;
                PlayerPrefs.SetString("Language", _language);

                _oldDictionary = dic;
                Localization.localizationHasBeenSet = true;

                _languageIndex = -1;
                _languages = new[] {languageName};

                Localization.onLocalize?.Invoke();
                UIRoot.Broadcast("OnLocalize");
            }
        }

        public void Set(string key, string value)
        {
            lock (_lock)
            {
                if (_oldDictionary.ContainsKey(key))
                    _oldDictionary[key] = value;
                else
                    _oldDictionary.Add(key, value);
            }
        }

        public Dictionary<string, string[]> ProvideDictionary()
        {
            lock (_lock)
            {
                if (!Localization.localizationHasBeenSet)
                    LoadDictionary(PlayerPrefs.GetString("Language", "English"));

                return _dictionary;
            }
        }

        public void SetDictionary(Dictionary<string, string[]> value)
        {
            lock (_lock)
            {
                Localization.localizationHasBeenSet = value != null;
                _dictionary = value;
            }
        }

        public String[] ProvideLanguages()
        {
            lock (_lock)
            {
                if (!Localization.localizationHasBeenSet)
                    LoadDictionary(PlayerPrefs.GetString("Language", "English"));
                return _languages;
            }
        }

        public String ProvideLanguage()
        {
            lock (_lock)
            {
                if (!String.IsNullOrEmpty(_language))
                    return _language;

                _language = PlayerPrefs.GetString("Language", "English");
                LoadAndSelect(_language);
                return _language;
            }
        }

        public void SetLanguage(String value)
        {
            lock (_lock)
            {
                if (_language == value)
                    return;

                _language = value;
                LoadAndSelect(value);
            }
        }

        public void SetReplacement(Dictionary<String, String> dic)
        {
            lock (_replacement)
                _replacement = dic;
        }

        public void ReplaceKey(String key, String val)
        {
            lock (_replacement)
            {
                if (!String.IsNullOrEmpty(val))
                    _replacement[key] = val;
                else
                    _replacement.Remove(key);
            }
        }

        public void ClearReplacements()
        {
            lock (_replacement)
                _replacement.Clear();
        }

        public Boolean TryGetReplacement(String key, out String value)
        {
            lock (_replacement)
                return _replacement.TryGetValue(key, out value);
        }

        public String Get(String key)
        {
            lock (_lock)
            {
                if (!Localization.localizationHasBeenSet)
                    LoadDictionary(PlayerPrefs.GetString("Language", "English(US)"));

                if (_languages == null)
                {
                    Debug.LogError("No localization data present");
                    return null;
                }

                String lang = Localization.language;
                if (_languageIndex == -1)
                {
                    for (int index = 0; index < _languages.Length; ++index)
                    {
                        if (_languages[index] != lang)
                            continue;

                        _languageIndex = index;
                        break;
                    }
                }

                if (_languageIndex == -1)
                {
                    _languageIndex = 0;
                    _language = _languages[0];
                    Debug.LogWarning("Language not found: " + lang);
                }

                String str1;
                String[] strArray;
                switch (UICamera.currentScheme)
                {
                    case UICamera.ControlScheme.Touch:
                        String key1 = key + " Mobile";
                        if (TryGetReplacement(key1, out str1))
                            return str1;

                        if (_languageIndex != -1 && _dictionary.TryGetValue(key1, out strArray) && _languageIndex < strArray.Length)
                            return strArray[_languageIndex];

                        if (_oldDictionary.TryGetValue(key1, out str1))
                            return str1;

                        break;
                    case UICamera.ControlScheme.Controller:
                        string key2 = key + " Controller";
                        if (TryGetReplacement(key2, out str1))
                            return str1;
                        if (_languageIndex != -1 && _dictionary.TryGetValue(key2, out strArray) && _languageIndex < strArray.Length)
                            return strArray[_languageIndex];
                        if (_oldDictionary.TryGetValue(key2, out str1))
                            return str1;
                        break;
                }

                if (TryGetReplacement(key, out str1))
                    return str1;

                if (_languageIndex != -1 && _dictionary.TryGetValue(key, out strArray))
                {
                    if (_languageIndex >= strArray.Length)
                        return strArray[0];

                    String str2 = strArray[_languageIndex];
                    if (String.IsNullOrEmpty(str2))
                        str2 = strArray[0];

                    return str2;
                }

                return _oldDictionary.TryGetValue(key, out str1) ? str1 : key;
            }
        }

        public bool Exists(string key)
        {
            lock (_lock)
            {
                if (!Localization.localizationHasBeenSet)
                    Localization.language = PlayerPrefs.GetString("Language", "English");

                return _dictionary.ContainsKey(key) || _oldDictionary.ContainsKey(key);
            }
        }

        private void ReplaceDictionary()
        {
            try
            {
                if (_languageIndex < 0)
                    return;

                if (!Configuration.Import.Enabled || !Configuration.Import.Text)
                    return;

                String importPath = ModTextResources.Import.GetCurrentPath(ModTextResources.SystemPath);
                if (!File.Exists(importPath))
                {
                    Log.Warning($"[LocalizationDictionary] Loading was skipped because a file does not exists: [{importPath}]...");
                }

                Log.Message($"[LocalizationDictionary] Loading from [{importPath}]...");

                TxtEntry[] entries = TxtReader.ReadStrings(importPath);
                Dictionary<string, string> dic = CreateReplaceDictionary(entries);

                Log.Message("[LocalizationDictionary] Loading completed successfully.");
                SetReplacement(dic);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[LocalizationDictionary] Failed to load text.");
            }
        }

        private static Dictionary<String, String> CreateReplaceDictionary(TxtEntry[] entries)
        {
            Dictionary<String, String> dic = new Dictionary<String, String>();
            foreach (TxtEntry entry in entries)
            {
                switch (entry.Prefix)
                {
                    case "KEY":
                    case "Symbol":
                    case "Name":
                    case "":
                        continue;
                }

                dic[entry.Prefix] = entry.Value;
            }
            return dic;
        }

        private Dictionary<String, Int32> PrepareLanguages(Boolean merge, BetterList<String> betterList, out String[] newLanguages)
        {
            newLanguages = null;
            if (String.IsNullOrEmpty(_language))
                Localization.localizationHasBeenSet = false;

            if (Localization.localizationHasBeenSet && (merge || _merging) && (_languages?.Length > 0))
                newLanguages = MergeLanguages(betterList);
            else
                LoadLanguages(betterList);

            Dictionary<String, Int32> languageIndices = new Dictionary<String, Int32>();
            if (_languages == null)
                return languageIndices;

            for (Int32 index = 0; index < _languages.Length; ++index)
                languageIndices.Add(_languages[index], index);

            return languageIndices;
        }

        private String[] MergeLanguages(BetterList<String> betterList)
        {
            String[] newLanguages = new String[betterList.size];
            for (int index = 0; index < betterList.size; ++index)
                newLanguages[index] = betterList[index];

            for (int index = 0; index < betterList.size; ++index)
            {
                if (HasLanguage(betterList[index]))
                    continue;

                Int32 newSize = _languages.Length + 1;
                Array.Resize(ref _languages, newSize);
                _languages[newSize - 1] = betterList[index];

                Dictionary<String, String[]> dic = new Dictionary<String, String[]>();
                foreach (KeyValuePair<String, String[]> current in _dictionary)
                {
                    String[] array = current.Value;
                    Array.Resize(ref array, newSize);
                    array[newSize - 1] = array[0];
                    dic.Add(current.Key, array);
                }
                _dictionary = dic;
            }

            return newLanguages;
        }

        private void LoadLanguages(BetterList<string> betterList)
        {
            _dictionary.Clear();
            _languages = new String[betterList.size];

            if (!Localization.localizationHasBeenSet)
            {
                _language = PlayerPrefs.GetString("Language", betterList[0]);
                Localization.localizationHasBeenSet = true;
            }

            for (Int32 index = 0; index < betterList.size; ++index)
            {
                _languages[index] = betterList[index];
                if (_languages[index] == _language)
                    _languageIndex = index;
            }
        }

        private Boolean LoadDictionary(String value)
        {
            Byte[] bytes = null;
            if (!Localization.localizationHasBeenSet)
            {
                if (Localization.loadFunction == null)
                {
                    TextAsset textAsset = Resources.Load<TextAsset>("EmbeddedAsset/Manifest/Text/Localization.txt");
                    if (textAsset != null)
                        bytes = textAsset.bytes;
                }
                else
                {
                    bytes = Localization.loadFunction("Localization");
                }

                Localization.localizationHasBeenSet = true;
            }

            if (TryLoadCSV(bytes, false))
                return true;

            if (String.IsNullOrEmpty(value))
                value = _language;

            if (String.IsNullOrEmpty(value))
                return false;

            if (Localization.loadFunction == null)
            {
                TextAsset textAsset = Resources.Load<TextAsset>(value);
                if (textAsset != null)
                    bytes = textAsset.bytes;
            }
            else
            {
                bytes = Localization.loadFunction(value);
            }

            if (bytes == null)
                return false;

            ByteReader byteReader = new ByteReader(bytes);
            Set(value, byteReader.ReadDictionary());
            return true;
        }

        private Boolean HasLanguage(String languageName)
        {
            int index = 0;
            for (int length = _languages.Length; index < length; ++index)
            {
                if (_languages[index] == languageName)
                    return true;
            }
            return false;
        }

        private void AddCSV(BetterList<String> newValues, String[] newLanguages, Dictionary<String, Int32> languageIndices)
        {
            if (newValues.size < 2)
                return;

            String key = newValues[0];
            if (String.IsNullOrEmpty(key))
                return;

            String[] strings = ExtractStrings(newValues, newLanguages, languageIndices);
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = strings;
                if (newLanguages != null)
                    return;

                Debug.LogWarning("Localization key '" + key + "' is already present");
            }
            else
            {
                try
                {
                    _dictionary.Add(key, strings);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Unable to add '" + key + "' to the Localization dictionary.\n" + ex.Message);
                }
            }
        }

        private String[] ExtractStrings(BetterList<String> added, String[] newLanguages, Dictionary<String, Int32> languageIndices)
        {
            String[] result;
            if (newLanguages == null)
            {
                result = new String[_languages.Length];
                Int32 index1 = 1;
                for (Int32 index2 = Mathf.Min(added.size, result.Length + 1); index1 < index2; ++index1)
                    result[index1 - 1] = added[index1];

                return result;
            }

            String key = added[0];
            if (!_dictionary.TryGetValue(key, out result))
                result = new String[_languages.Length];

            for (Int32 i = 0; i < newLanguages.Length; i++)
            {
                String lang = newLanguages[i];
                Int32 langIndex = languageIndices[lang];
                result[langIndex] = added[i + 1];
            }

            return result;
        }

        private bool SelectLanguage(String lang)
        {
            _languageIndex = -1;
            if (_dictionary.Count == 0)
                return false;

            int index = 0;
            for (int length = _languages.Length; index < length; ++index)
            {
                if (_languages[index] != lang)
                    continue;

                _oldDictionary.Clear();
                _languageIndex = index;
                _language = lang;
                PlayerPrefs.SetString("Language", _language);
                Localization.onLocalize?.Invoke();
                UIRoot.Broadcast("OnLocalize");
                return true;
            }

            return false;
        }

        private void LoadAndSelect(string value)
        {
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (_dictionary.Count == 0 && !LoadDictionary(value))
                        return;
                    if (SelectLanguage(value))
                        return;
                }

                if (_oldDictionary.Count > 0)
                    return;

                _oldDictionary.Clear();
                _dictionary.Clear();

                if (String.IsNullOrEmpty(value))
                    PlayerPrefs.DeleteKey("Language");
            }
            finally
            {
                ReplaceDictionary();
            }
        }
    }
}