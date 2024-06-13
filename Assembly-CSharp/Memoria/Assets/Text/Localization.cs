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

        /// <summary>For keys not present in vanilla</summary>
        public static String GetWithDefault(String key)
        {
            String value = Provider.Get(key);
            if (!String.Equals(value, key))
                return value;
            if (_defaultDictionary.TryGetValue(key, out value))
                return value;
            return key;
        }

        private static Dictionary<String, String> _defaultDictionary = new Dictionary<String, String>()
        {
            { "GilSymbol", "%[YSUB=1.3][sub]G" },
            { "FailedMixMessage", "The combination failed!" }
        };
    }
}
