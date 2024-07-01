using Memoria.Prime;
using System;
using System.Text;

namespace Memoria.Assets
{
    public static class ModTextResources
    {
        public static TextResourceReference SystemReference => new("/System");
        private static TextResourceReference AbilitiesReference => new("/Abilities");
        private static TextResourceReference CommandsReference => new("/Commands");
        private static TextResourceReference SkillsReference => new("/Skills");
        private static TextResourceReference BattleReference => new("/Battle");
        private static TextResourceReference ItemsReference => new("/Items");
        private static TextResourceReference KeyItemsReference => new("/KeyItems");
        private static TextResourceReference CardLevelsReference => new("/ETC/CardLevels");
        private static TextResourceReference CardTitlesReference => new("/ETC/CardTitles");
        private static TextResourceReference BattleCommandsReference => new("/ETC/BattleCommands");
        private static TextResourceReference BattleMessagesReference => new("/ETC/BattleMessages");
        private static TextResourceReference LibraReference => new("/ETC/Libra");
        private static TextResourceReference WorldLocationsReference => new("/ETC/WorldLocations");
        private static TextResourceReference ChocoboReference => new("/ETC/Chocobo");

        private const String LocationNamesDirectoryPath = "/Location";
        private const String FieldsDirectoryPath = "/Field";
        private static TextResourceReference FieldTagsReference => new(FieldsDirectoryPath + "/FieldTags");
        private static TextResourceReference CharacterNamesReference => new("/CharacterNames");

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
            public static TextResourcePath System => GetCurrentPath(SystemReference);
            public static TextResourcePath Abilities => GetCurrentPath(AbilitiesReference);
            public static TextResourcePath Commands => GetCurrentPath(CommandsReference);
            public static TextResourcePath Skills => GetCurrentPath(SkillsReference);
            public static TextResourcePath Battle => GetCurrentPath(BattleReference);
            public static TextResourcePath Items => GetCurrentPath(ItemsReference);
            public static TextResourcePath KeyItems => GetCurrentPath(KeyItemsReference);
            public static TextResourcePath CardLevels => GetCurrentPath(CardLevelsReference);
            public static TextResourcePath CardTitles => GetCurrentPath(CardTitlesReference);
            public static TextResourcePath BattleCommands => GetCurrentPath(BattleCommandsReference);
            public static TextResourcePath BattleMessages => GetCurrentPath(BattleMessagesReference);
            public static TextResourcePath Libra => GetCurrentPath(LibraReference);
            public static TextResourcePath WorldLocations => GetCurrentPath(WorldLocationsReference);
            public static TextResourcePath Chocobo => GetCurrentPath(ChocoboReference);
            public static TextResourcePath FieldTags => GetCurrentPath(FieldTagsReference);
            public static TextResourcePath CharacterNames => GetCurrentPath(CharacterNamesReference);
            public static String LocationNamesDirectory => GetCurrentPath(LocationNamesDirectoryPath);
            public static String FieldsDirectory => GetCurrentPath(FieldsDirectoryPath);

            public static String ManifestDirectory => GetCurrentPath(ManifestDirectoryPath);

            public static String CreditsAmazon => ManifestDirectory + CreditsAmazonPath;
            public static String CreditsMobile => ManifestDirectory + CreditsMobilePath;
            public static String CreditsEStore => ManifestDirectory + CreditsEStorePath;
            public static String CreditsSteam => ManifestDirectory + CreditsSteamPath;
            public static String Credits => ManifestDirectory + CreditsPath;

            public static TextResourcePath GetCurrentPath(TextResourceReference relativeReference)
            {
                String currentPath = GetCurrentPath(relativeReference.Value);
                return new TextResourcePath(new TextResourceReference(currentPath), Configuration.Export.TextFileFormat);
            }

            public static String GetCurrentPath(String relativePath)
            {
                StringBuilder sb = new(64);
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
            public static TextResourceReference Abilities => GetCurrentPath(AbilitiesReference);
            public static TextResourceReference Commands => GetCurrentPath(CommandsReference);
            public static TextResourceReference Skills => GetCurrentPath(SkillsReference);
            public static TextResourceReference Battle => GetCurrentPath(BattleReference);
            public static TextResourceReference Items => GetCurrentPath(ItemsReference);
            public static TextResourceReference KeyItems => GetCurrentPath(KeyItemsReference);
            public static TextResourceReference CardLevels => GetCurrentPath(CardLevelsReference);
            public static TextResourceReference CardTitles => GetCurrentPath(CardTitlesReference);
            public static TextResourceReference BattleCommands => GetCurrentPath(BattleCommandsReference);
            public static TextResourceReference BattleMessages => GetCurrentPath(BattleMessagesReference);
            public static TextResourceReference Libra => GetCurrentPath(LibraReference);
            public static TextResourceReference WorldLocations => GetCurrentPath(WorldLocationsReference);
            public static TextResourceReference Chocobo => GetCurrentPath(ChocoboReference);
            public static TextResourceReference FieldTags => GetCurrentPath(FieldTagsReference);
            public static TextResourceReference CharacterNames => GetCurrentPath(CharacterNamesReference);
            public static String LocationNamesDirectory => GetCurrentPath(LocationNamesDirectoryPath);
            public static String FieldsDirectory => GetCurrentPath(FieldsDirectoryPath);

            public static String ManifestDirectory => GetCurrentPath(ManifestDirectoryPath);

            public static String CreditsAmazon => ManifestDirectory + CreditsAmazonPath;
            public static String CreditsAndroid => ManifestDirectory + CreditsAndroidPath;
            public static String CreditsMobile => ManifestDirectory + CreditsMobilePath;
            public static String CreditsEStore => ManifestDirectory + CreditsEStorePath;
            public static String CreditsSteam => ManifestDirectory + CreditsSteamPath;
            public static String Credits => ManifestDirectory + CreditsPath;

            public static TextResourceReference GetCurrentPath(TextResourceReference relativeReference)
            {
                String currentPath = GetCurrentPath(relativeReference.Value);
                return new TextResourceReference(currentPath);
            }

            public static String GetCurrentPath(String relativePath)
            {
                return GetSymbolPath(Localization.GetSymbol(), relativePath);
            }

            public static TextResourceReference GetSymbolPath(String symbol, TextResourceReference relativeReference)
            {
                String currentPath = GetSymbolPath(symbol, relativeReference.Value);
                return new TextResourceReference(currentPath);
            }

            public static String GetSymbolPath(String symbol, String relativePath)
            {
                StringBuilder sb = new(64);
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
