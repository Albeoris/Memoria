using System;
using System.Text;

namespace Memoria.Assets
{
	public static class ModTextResources
	{
		public const String SystemPath = "/System.strings";
		private const String AbilitiesPath = "/Abilities.strings";
		private const String CommandsPath = "/Commands.strings";
		private const String SkillsPath = "/Skills.strings";
		private const String BattlePath = "/Battle.strings";
		private const String ItemsPath = "/Items.strings";
		private const String KeyItemsPath = "/KeyItems.strings";
		private const String CardLevelsPath = "/ETC/CardLevels.strings";
		private const String CardTitlesPath = "/ETC/CardTitles.strings";
		private const String BattleCommandsPath = "/ETC/BattleCommands.strings";
		private const String BattleMessagesPath = "/ETC/BattleMessages.strings";
		private const String LibraPath = "/ETC/Libra.strings";
		private const String WorldLocationsPath = "/ETC/WorldLocations.strings";
		private const String ChocoboPath = "/ETC/Chocobo.strings";

		private const String LocationNamesDirectoryPath = "/Location";
		private const String FieldsDirectoryPath = "/Field";
		private const String FieldTagsPath = FieldsDirectoryPath + "/FieldTags.strings";
		private const String CharacterNamesPath = "/CharacterNames.strings";

		private const String ManifestDirectoryPath = "/Manifest";
		private const String CreditsAmazonPath = "/StaffCredits_Amazon.txt";
		private const String CreditsAndroidPath = "/StaffCredits_AndroidSQEXMarket.txt";
		private const String CreditsMobilePath = "/StaffCredits_Mobile.txt";
		private const String CreditsEStorePath = "/StaffCredits_EStore.txt";
		private const String CreditsSteamPath = "/StaffCredits_Steam.txt";
		private const String CreditsPath = "/StaffCredits.txt";

		public static class Export
		{
			public static String CurrentSymbol { get; set; }
			public static String System => GetCurrentPath(SystemPath);
			public static String Abilities => GetCurrentPath(AbilitiesPath);
			public static String Commands => GetCurrentPath(CommandsPath);
			public static String Skills => GetCurrentPath(SkillsPath);
			public static String Battle => GetCurrentPath(BattlePath);
			public static String Items => GetCurrentPath(ItemsPath);
			public static String KeyItems => GetCurrentPath(KeyItemsPath);
			public static String CardLevels => GetCurrentPath(CardLevelsPath);
			public static String CardTitles => GetCurrentPath(CardTitlesPath);
			public static String BattleCommands => GetCurrentPath(BattleCommandsPath);
			public static String BattleMessages => GetCurrentPath(BattleMessagesPath);
			public static String Libra => GetCurrentPath(LibraPath);
			public static String WorldLocations => GetCurrentPath(WorldLocationsPath);
			public static String Chocobo => GetCurrentPath(ChocoboPath);
			public static String FieldTags => GetCurrentPath(FieldTagsPath);
			public static String CharacterNames => GetCurrentPath(CharacterNamesPath);
			public static String LocationNamesDirectory => GetCurrentPath(LocationNamesDirectoryPath);
			public static String FieldsDirectory => GetCurrentPath(FieldsDirectoryPath);

			public static String ManifestDirectory => GetCurrentPath(ManifestDirectoryPath);

			public static String CreditsAmazon => ManifestDirectory + CreditsAmazonPath;
			public static String CreditsMobile => ManifestDirectory + CreditsMobilePath;
			public static String CreditsEStore => ManifestDirectory + CreditsEStorePath;
			public static String CreditsSteam => ManifestDirectory + CreditsSteamPath;
			public static String Credits => ManifestDirectory + CreditsPath;

			public static String GetCurrentPath(String relativePath)
			{
				StringBuilder sb = new StringBuilder(64);
				sb.Append(Configuration.Export.Path);
				if (sb.Length > 0 && sb[sb.Length - 1] != '/' && sb[sb.Length - 1] != '\\')
					sb.Append('/');
				sb.Append("Text/");
				sb.Append(CurrentSymbol ?? Localization.GetSymbol());
				sb.Append(relativePath);
				return sb.ToString();
			}
		}

		public static class Import
		{
			public static String Abilities => GetCurrentPath(AbilitiesPath);
			public static String Commands => GetCurrentPath(CommandsPath);
			public static String Skills => GetCurrentPath(SkillsPath);
			public static String Battle => GetCurrentPath(BattlePath);
			public static String Items => GetCurrentPath(ItemsPath);
			public static String KeyItems => GetCurrentPath(KeyItemsPath);
			public static String CardLevels => GetCurrentPath(CardLevelsPath);
			public static String CardTitles => GetCurrentPath(CardTitlesPath);
			public static String BattleCommands => GetCurrentPath(BattleCommandsPath);
			public static String BattleMessages => GetCurrentPath(BattleMessagesPath);
			public static String Libra => GetCurrentPath(LibraPath);
			public static String WorldLocations => GetCurrentPath(WorldLocationsPath);
			public static String Chocobo => GetCurrentPath(ChocoboPath);
			public static String FieldTags => GetCurrentPath(FieldTagsPath);
			public static String CharacterNames => GetCurrentPath(CharacterNamesPath);
			public static String LocationNamesDirectory => GetCurrentPath(LocationNamesDirectoryPath);
			public static String FieldsDirectory => GetCurrentPath(FieldsDirectoryPath);

			public static String ManifestDirectory => GetCurrentPath(ManifestDirectoryPath);

			public static String CreditsAmazon => ManifestDirectory + CreditsAmazonPath;
			public static String CreditsAndroid => ManifestDirectory + CreditsAndroidPath;
			public static String CreditsMobile => ManifestDirectory + CreditsMobilePath;
			public static String CreditsEStore => ManifestDirectory + CreditsEStorePath;
			public static String CreditsSteam => ManifestDirectory + CreditsSteamPath;
			public static String Credits => ManifestDirectory + CreditsPath;

			public static String GetCurrentPath(String relativePath)
			{
				return GetSymbolPath(Localization.GetSymbol(), relativePath);
			}

			public static String GetSymbolPath(String symbol, String relativePath)
			{
				StringBuilder sb = new StringBuilder(64);
				sb.Append(Configuration.Import.Path);
				if (sb.Length > 0 && sb[sb.Length - 1] != '/' && sb[sb.Length - 1] != '\\')
					sb.Append('/');
				sb.Append("Text/");
				sb.Append(symbol);
				sb.Append(relativePath);
				return sb.ToString();
			}
		}
	}
}
