using System;

public class LanguageCode
{
    public static String ConvertToLanguageName(Int32 languageCode)
    {
        if (languageCode == 0)
        {
            return "English(US)";
        }
        if (languageCode == 1)
        {
            return "English(UK)";
        }
        if (languageCode == 2)
        {
            return "Japanese";
        }
        if (languageCode == 3)
        {
            return "German";
        }
        if (languageCode == 4)
        {
            return "French";
        }
        if (languageCode == 5)
        {
            return "Italian";
        }
        if (languageCode == 6)
        {
            return "Spanish";
        }
        return FF9StateSystem.Settings.GetSystemLanguage();
    }

    public const Int32 System = -1;

    public const Int32 EnglishUS = 0;

    public const Int32 EnglishUK = 1;

    public const Int32 Japanese = 2;

    public const Int32 German = 3;

    public const Int32 French = 4;

    public const Int32 Italian = 5;

    public const Int32 Spanish = 6;
}
