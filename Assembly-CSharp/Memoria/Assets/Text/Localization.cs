using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    public static class Localization
    {
        internal static readonly LanguageMap Provider;

        static Localization()
        {
            Provider = new LanguageMap();
            Provider.Broadcast();
        }

        public static ICollection<String> KnownLanguages => Provider.KnownLanguages;

        public static String CurrentLanguage
        {
            get => Provider.CurrentLanguage;
            set => Provider.SelectLanguage(value);
        }

        public static String GetSymbol()
        {
            return Provider.CurrentSymbol;
        }

        public static String Get(String key)
        {
            return Provider.Get(key);
        }

        /// <summary>Might be useful in the future to base a translation mod on another language</summary>
        public static String GetFallbackSymbol()
        {
            return "US";
        }

        /// <summary>For keys not present in vanilla</summary>
        public static String GetWithDefault(String key)
        {
            String value = Provider.Get(key);
            if (!String.Equals(value, key))
                return value;
            if (_defaultDictionary.TryGetValue(key, out Dictionary<String, String> defaultValue))
            {
                if (defaultValue.TryGetValue(Provider.CurrentSymbol, out value))
                    return value;
                else if (defaultValue.TryGetValue(GetFallbackSymbol(), out value))
                    return value;
            }
            return key;
        }

        private static Dictionary<String, Dictionary<String, String>> _defaultDictionary = new Dictionary<String, Dictionary<String, String>>()
        {
            { "GilSymbol", new Dictionary<String, String>()
                {
                    { "US", "%[YSUB=1.3][sub]G" }
                }
            },
            { "FailedMixMessage", new Dictionary<String, String>()
                {
                    { "US", "The combination failed!" },
                    { "UK", "The combination failed!" },
                    { "JP", "調合失敗" },
                    { "ES", "La mezcla falló!" },
                    { "FR", "Le mélange a échoué !" },
                    { "GR", "Mischung fehlgeschlagen!" },
                    { "IT", "La miscela non è riuscita!" }
                }
            }
        };
    }
}
