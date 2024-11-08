using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Xml;

namespace Memoria.Launcher
{
    public sealed partial class Lang
    {
        public static String LangName = "en";
        public static ResourceDictionary Res => Application.Current.Resources;

        public static String[] LauncherLanguageList = { "en", "de", "es", "fr", "it", "jp", "pt-BR", "ru", "tr", "uk", "zh-CN" };

        public static String[] LauncherLanguageNames = { "English", "Deutsch", "Español", "Français", "Italiano", "日本語", "Português (brasileiro)", "Русский", "Türkçe", "Українська", "中文" };

        public static void Initialize()
        {
            try
            {
                LoadLanguageResources("en");
                Assembly assembly = Assembly.GetExecutingAssembly();

                XmlElement cur = null;

                IniFile iniFile = IniFile.SettingsIni;
                String forcedLang = iniFile.GetSetting("Memoria", "LauncherLanguage");

                String[] fileNames = String.IsNullOrEmpty(forcedLang) ?
                    [CultureInfo.CurrentCulture.Name, CultureInfo.CurrentCulture.TwoLetterISOLanguageName, CultureInfo.CurrentCulture.ThreeLetterISOLanguageName] :
                    [forcedLang, CultureInfo.CurrentCulture.Name, CultureInfo.CurrentCulture.TwoLetterISOLanguageName, CultureInfo.CurrentCulture.ThreeLetterISOLanguageName,];
                foreach (String name in fileNames)
                {
                    cur = XmlHelper.LoadEmbadedDocument(assembly, $"Languages.{name}.xml");
                    if (cur != null)
                    {
                        LoadLanguageResources(cur);
                        LangName = name;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize Lang Data.");
                Console.WriteLine(ex);
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        #region Instance

        public static Boolean LoadLanguageResources(String lang)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            XmlElement element = XmlHelper.LoadEmbadedDocument(assembly, $"Languages.{lang}.xml");
            if (element == null)
            {
                return false;
            }
            LoadLanguageResources(element);
            LangName = lang;
            return true;
        }

        private static void LoadLanguageResources(XmlElement rootElement)
        {
            foreach (XmlElement node in rootElement)
            {
                foreach (XmlAttribute at in node.Attributes)
                {
                    Application.Current.Resources[$"{node.Name}.{at.Name}"] = at.Value;
                }
            }
        }
        #endregion
    }
}
