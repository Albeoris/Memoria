using System;

namespace Memoria
{
    public static class EmbadedTextResources
    {
        private const String AbilityNamesPath = "/Ability/sa_name.mes";
        private const String AbilityHelpsPath = "/Ability/sa_help.mes";
        private const String CommandNamesPath = "/Command/com_name.mes";
        private const String CommandHelpsPath = "/Command/com_help.mes";
        private const String SkillNamesPath = "/Ability/aa_name.mes";
        private const String SkillHelpsPath = "/Ability/aa_help.mes";
        private const String ItemNamesPath = "/Item/itm_name.mes";
        private const String ItemHelpsPath = "/Item/itm_help.mes";
        private const String ItemBattlePath = "/Item/itm_btl.mes";
        private const String KeyitemNamesPath = "/KeyItem/imp_name.mes";
        private const String KeyitemHelpsPath = "/KeyItem/imp_help.mes";
        private const String KeyItemDescriptionsPath = "/KeyItem/imp_skin.mes";
        private const String CardLevelsPath = "/ETC/card.mes";
        private const String CardTitlesPath = "/ETC/minista.mes";
        private const String BattleCommandsPath = "/ETC/cmdtitle.mes";
        private const String BattleMessagesPath = "/ETC/follow.mes";
        private const String LibraPath = "/ETC/libra.mes";
        private const String WorldLocationsPath = "/ETC/worldloc.mes";
        private const String ChocoboPath = "/ETC/ff9choco.mes";
        private const String LocationNamesPath = "/Location/loc_name.mes";

        public static String AbilityNames => GetCurrentPath(AbilityNamesPath);
        public static String AbilityHelps => GetCurrentPath(AbilityHelpsPath);
        public static String CommandNames => GetCurrentPath(CommandNamesPath);
        public static String CommandHelps => GetCurrentPath(CommandHelpsPath);
        public static String SkillNames => GetCurrentPath(SkillNamesPath);
        public static String SkillHelps => GetCurrentPath(SkillHelpsPath);
        public static String ItemNames => GetCurrentPath(ItemNamesPath);
        public static String ItemHelps => GetCurrentPath(ItemHelpsPath);
        public static String ItemBattle => GetCurrentPath(ItemBattlePath);
        public static String KeyItemNames => GetCurrentPath(KeyitemNamesPath);
        public static String KeyItemHelps => GetCurrentPath(KeyitemHelpsPath);
        public static String KeyItemDescriptions => GetCurrentPath(KeyItemDescriptionsPath);
        public static String CardLevels => GetCurrentPath(CardLevelsPath);
        public static String CardTitles => GetCurrentPath(CardTitlesPath);
        public static String BattleCommands => GetCurrentPath(BattleCommandsPath);
        public static String BattleMessages => GetCurrentPath(BattleMessagesPath);
        public static String Libra => GetCurrentPath(LibraPath);
        public static String WorldLocations => GetCurrentPath(WorldLocationsPath);
        public static String Chocobo => GetCurrentPath(ChocoboPath);
        public static String LocationNames => GetCurrentPath(LocationNamesPath);

        public static String GetCurrentPath(String relativePath)
        {
            return Localization.GetPath() + relativePath;
        }
    }
}