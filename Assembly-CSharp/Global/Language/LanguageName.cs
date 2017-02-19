using System;

public class LanguageName
{
	public static Int32 ConvertToLanguageCode(String language)
	{
		if (String.Compare(language, "English(US)") == 0)
		{
			return 0;
		}
		if (String.Compare(language, "English(UK)") == 0)
		{
			return 1;
		}
		if (String.Compare(language, "Japanese") == 0)
		{
			return 2;
		}
		if (String.Compare(language, "German") == 0)
		{
			return 3;
		}
		if (String.Compare(language, "French") == 0)
		{
			return 4;
		}
		if (String.Compare(language, "Italian") == 0)
		{
			return 5;
		}
		if (String.Compare(language, "Spanish") == 0)
		{
			return 6;
		}
		return -1;
	}

	public const String EnglishUS = "English(US)";

	public const String EnglishUK = "English(UK)";

	public const String Japanese = "Japanese";

	public const String German = "German";

	public const String French = "French";

	public const String Italian = "Italian";

	public const String Spanish = "Spanish";
}
