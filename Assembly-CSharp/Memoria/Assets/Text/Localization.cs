using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    public static class Localization
    {
        internal static readonly LanguageMap Provider = new LanguageMap();

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
    }
}