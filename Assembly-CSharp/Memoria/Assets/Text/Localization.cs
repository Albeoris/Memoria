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
            // The base reading direction of the language
            { "ReadingDirection", new Dictionary<String, String>()
                {
                    { "US", UnicodeBIDI.DIRECTION_NAME_LEFT_TO_RIGHT },
                    { "UK", UnicodeBIDI.DIRECTION_NAME_LEFT_TO_RIGHT },
                    { "JP", UnicodeBIDI.DIRECTION_NAME_LEFT_TO_RIGHT },
                    { "ES", UnicodeBIDI.DIRECTION_NAME_LEFT_TO_RIGHT },
                    { "FR", UnicodeBIDI.DIRECTION_NAME_LEFT_TO_RIGHT },
                    { "GR", UnicodeBIDI.DIRECTION_NAME_LEFT_TO_RIGHT },
                    { "IT", UnicodeBIDI.DIRECTION_NAME_LEFT_TO_RIGHT }
                }
            },
            // Language name in the title menu's button
            { "NameShort", new Dictionary<String, String>()
                {
                    { "US", "US" },
                    { "UK", "UK" },
                    { "JP", "JP" },
                    { "ES", "ES" },
                    { "FR", "FR" },
                    { "GR", "GR" },
                    { "IT", "IT" }
                }
            },
            // Gil formatiing in multiple menu
            { "GilSymbol", new Dictionary<String, String>()
                {
                    { "US", "%[YSUB=1.3][sub]G" }
                }
            },
            // New options in the config menu
            { "SoundVolume", new Dictionary<String, String>()
                {
                    { "US", "Sound Volume" },
                    { "UK", "Sound Volume" },
                    { "JP", "サウンド音量" },
                    { "ES", "Volumen de sonidos" },
                    { "FR", "Volume des sons" },
                    { "GR", "Lautstärke der Geräusche" },
                    { "IT", "Volume dei suoni" }
                }
            },
            { "MusicVolume", new Dictionary<String, String>()
                {
                    { "US", "Music Volume" },
                    { "UK", "Music Volume" },
                    { "JP", "音楽の音量" },
                    { "ES", "Volumen de la música" },
                    { "FR", "Volume de la musique" },
                    { "GR", "Musiklautstärke" },
                    { "IT", "Volume della musica" }
                }
            },
            { "MovieVolume", new Dictionary<String, String>()
                {
                    { "US", "Movie Volume" },
                    { "UK", "Movie Volume" },
                    { "JP", "動画の音量" },
                    { "ES", "Volumen de la película" },
                    { "FR", "Volume des films" },
                    { "GR", "Filmlautstärke" },
                    { "IT", "Volume di film" }
                }
            },
            { "VoiceVolume", new Dictionary<String, String>()
                {
                    { "US", "Voice Volume" },
                    { "UK", "Voice Volume" },
                    { "JP", "音声音量" },
                    { "ES", "Volumen de voz" },
                    { "FR", "Volume des voix" },
                    { "GR", "Stimmen Lautstärke" },
                    { "IT", "Volume della voce" }
                }
            },
            { "ATBModeNormal", new Dictionary<String, String>()
                {
                    { "US", "ATB: Normal" },
                    { "UK", "ATB: Normal" },
                    { "JP", "ATBモード: 通常" },
                    { "ES", "ATB: Normal" },
                    { "FR", "ATB : Normal" },
                    { "GR", "ATB: Normaler" },
                    { "IT", "ATB: Normale" }
                }
            },
            { "ATBModeFast", new Dictionary<String, String>()
                {
                    { "US", "ATB: Fast" },
                    { "UK", "ATB: Fast" },
                    { "JP", "ATBモード: 高速" },
                    { "ES", "ATB: Rápido" },
                    { "FR", "ATB : Rapide" },
                    { "GR", "ATB: Schneller" },
                    { "IT", "ATB: Veloce" }
                }
            },
            { "ATBModeTurnBased", new Dictionary<String, String>()
                {
                    { "US", "ATB: Turn-Based" },
                    { "UK", "ATB: Turn-Based" },
                    { "JP", "ATBモード: ターン制" },
                    { "ES", "ATB: Por Turnos" },
                    { "FR", "ATB : Par Tour" },
                    { "GR", "ATB: Turn" },
                    { "IT", "ATB: A Turni" }
                }
            },
            { "ATBModeDynamic", new Dictionary<String, String>()
                {
                    { "US", "ATB: Dynamic" },
                    { "UK", "ATB: Dynamic" },
                    { "JP", "ATBモード: 動的" },
                    { "ES", "ATB: Dinámico" },
                    { "FR", "ATB: Dynamique" },
                    { "GR", "ATB: Dynamischer" },
                    { "IT", "ATB: Dinamica" }
                }
            },
            { "AutoText", new Dictionary<String, String>()
                {
                    { "US", "Auto Text" },
                    { "UK", "Auto Text" },
                    { "JP", "自動テキスト" },
                    { "ES", "Texto Automático" },
                    { "FR", "Texte Automatique" },
                    { "GR", "Autotext" },
                    { "IT", "Testo Automatico" }
                }
            },
            // Mix battle message
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
            // New parameters possibly displayed by Scan in battles
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
