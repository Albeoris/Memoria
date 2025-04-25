using System;

public static class LanguageName
{
    public const String EnglishUS = "English(US)";
    public const String EnglishUK = "English(UK)";
    public const String Japanese = "Japanese";
    public const String German = "German";
    public const String French = "French";
    public const String Italian = "Italian";
    public const String Spanish = "Spanish";

    public const String SymbolEnglishUS = "US";
    public const String SymbolEnglishUK = "UK";
    public const String SymbolJapanese = "JP";
    public const String SymbolGerman = "GR";
    public const String SymbolFrench = "FR";
    public const String SymbolItalian = "IT";
    public const String SymbolSpanish = "ES";

    public static Int32 ConvertToLanguageCode(String language)
    {
        if (String.Equals(language, LanguageName.EnglishUS))
            return 0;
        if (String.Equals(language, LanguageName.EnglishUK))
            return 1;
        if (String.Equals(language, LanguageName.Japanese))
            return 2;
        if (String.Equals(language, LanguageName.German))
            return 3;
        if (String.Equals(language, LanguageName.French))
            return 4;
        if (String.Equals(language, LanguageName.Italian))
            return 5;
        if (String.Equals(language, LanguageName.Spanish))
            return 6;
        return -1;
    }

    public static String ConvertToLanguageName(Int32 languageCode)
    {
        if (languageCode == 0)
            return LanguageName.EnglishUS;
        if (languageCode == 1)
            return LanguageName.EnglishUK;
        if (languageCode == 2)
            return LanguageName.Japanese;
        if (languageCode == 3)
            return LanguageName.German;
        if (languageCode == 4)
            return LanguageName.French;
        if (languageCode == 5)
            return LanguageName.Italian;
        if (languageCode == 6)
            return LanguageName.Spanish;
        return FF9StateSystem.Settings.GetSystemLanguage();
    }
}
