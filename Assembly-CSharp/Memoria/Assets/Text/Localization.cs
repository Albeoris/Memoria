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
            },
            { "ElementWeak", new Dictionary<String, String>()
                {
                    { "US", "Weak to [FFCC00]%[FFFFFF]" },
                    { "UK", "Weak to [FFCC00]%[FFFFFF]" },
                    { "JP", "[FFCC00]%[FFFFFF]属性に弱い" },
                    { "ES", "Vulnerable al elemento [FFCC00]%[FFFFFF]" },
                    { "FR", "Sensible à [FFCC00]%[FFFFFF]" },
                    { "GR", "Anfällig gegen [FFCC00]%[FFFFFF]-Element" },
                    { "IT", "Debole all’elemento [FFCC00]%[FFFFFF]" }
                }
            },
            { "ElementResist", new Dictionary<String, String>()
                {
                    { "US", "Resistant to [FFCC00]%[FFFFFF]" },
                    { "UK", "Resistant to [FFCC00]%[FFFFFF]" },
                    { "JP", "[FFCC00]%[FFFFFF]属性を半減" },
                    { "ES", "Resistente al elemento [FFCC00]%[FFFFFF]" },
                    { "FR", "Résiste à [FFCC00]%[FFFFFF]" },
                    { "GR", "Resistent gegen [FFCC00]%[FFFFFF]-Element" },
                    { "IT", "Resistente all’elemento [FFCC00]%[FFFFFF]" }
                }
            },
            { "ElementImmune", new Dictionary<String, String>()
                {
                    { "US", "Immune to [FFCC00]%[FFFFFF]" },
                    { "UK", "Immune to [FFCC00]%[FFFFFF]" },
                    { "JP", "[FFCC00]%[FFFFFF]属性を無効" },
                    { "ES", "Inmune al elemento [FFCC00]%[FFFFFF]" },
                    { "FR", "Immunisé à [FFCC00]%[FFFFFF]" },
                    { "GR", "Immun gegen [FFCC00]%[FFFFFF]-Element" },
                    { "IT", "Immune all’elemento [FFCC00]%[FFFFFF]" }
                }
            },
            { "ElementAbsorb", new Dictionary<String, String>()
                {
                    { "US", "Absorb [FFCC00]%[FFFFFF]" },
                    { "UK", "Absorb [FFCC00]%[FFFFFF]" },
                    { "JP", "[FFCC00]%[FFFFFF]属性を吸収" },
                    { "ES", "Absorber el elemento [FFCC00]%[FFFFFF]" },
                    { "FR", "Absorbe l'élément [FFCC00]%[FFFFFF]" },
                    { "GR", "Absorbiert [FFCC00]%[FFFFFF]-Element" },
                    { "IT", "Assorbire l'elemento [FFCC00]%[FFFFFF]" }
                }
            },
            { "StatusImmune", new Dictionary<String, String>()
                {
                    { "US", "Immune: [FFCC00]%[FFFFFF]" },
                    { "UK", "Immune: [FFCC00]%[FFFFFF]" },
                    { "JP", "耐性 [FFCC00]%[FFFFFF]" },
                    { "ES", "Inmune: [FFCC00]%[FFFFFF]" },
                    { "FR", "Immunité : [FFCC00]%[FFFFFF]" },
                    { "GR", "Immunität [FFCC00]%[FFFFFF]" },
                    { "IT", "Immune: [FFCC00]%[FFFFFF]" }
                }
            },
            { "StatusAuto", new Dictionary<String, String>()
                {
                    { "US", "Auto: [FFCC00]%[FFFFFF]" },
                    { "UK", "Auto: [FFCC00]%[FFFFFF]" },
                    { "JP", "永続 [FFCC00]%[FFFFFF]" },
                    { "ES", "Auto: [FFCC00]%[FFFFFF]" },
                    { "FR", "Auto : [FFCC00]%[FFFFFF]" },
                    { "GR", "Auto [FFCC00]%[FFFFFF]" },
                    { "IT", "Sempre: [FFCC00]%[FFFFFF]" }
                }
            },
            { "StatusResist", new Dictionary<String, String>()
                {
                    { "US", "Resist: [FFCC00]%[FFFFFF]" },
                    { "UK", "Resist: [FFCC00]%[FFFFFF]" },
                    { "JP", "免疫 [FFCC00]%[FFFFFF]" },
                    { "ES", "Resistente: [FFCC00]%[FFFFFF]" },
                    { "FR", "Résistance : [FFCC00]%[FFFFFF]" },
                    { "GR", "Widersteht [FFCC00]%[FFFFFF]" },
                    { "IT", "Resistente: [FFCC00]%[FFFFFF]" }
                }
            },
            { "AttackList", new Dictionary<String, String>()
                {
                    { "US", "[b]Abilities[/b]" },
                    { "UK", "[b]Abilities[/b]" },
                    { "JP", "[b]アビリティ[/b]" },
                    { "ES", "[b]Habilidades[/b]" },
                    { "FR", "[b]Compétences[/b]" },
                    { "GR", "[b]Fähigkeiten[/b]" },
                    { "IT", "[b]Abilità[/b]" }
                }
            },
        };
    }
}
