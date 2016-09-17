using System;

namespace Memoria
{
    public static class DataResources
    {
        private const String Items = "Items/";
        private const String Characters = "Characters/";
        private const String Abilities = "Abilities/";
        private const String Scripts = "Scripts/";

        public static readonly String DataDirectoryPath = AssetManagerUtil.GetStreamingAssetsPath() + "/Data/";
        public static readonly String ItemsDirectory = DataDirectoryPath + Items;
        public static readonly String CharactersDirectory = DataDirectoryPath + Characters;
        public static readonly String CharacterAbilitiesDirectory = CharactersDirectory + Abilities;
        public static readonly String ScriptsDirectory = AssetManagerUtil.GetStreamingAssetsPath() + '/' + Scripts;

        public static String GetCsvCharacterAbilitiesPath(CharacterPresetId presetId)
        {
            return CharacterAbilitiesDirectory + presetId + ".csv";
        }
    }
}