using System;
using Memoria.Data;

namespace Memoria.Assets
{
    public static class DataResources
    {
        // In general:
        // - Default file path is obtained by DataResources.SECTION.Directory + DataResources.SECTION.TYPEFile
        // - File path inside one of the Configuration.Mod.FolderNames is obtained by DataResources.SECTION.ModDirectory(modName) + DataResources.SECTION.TYPEFile
        public static readonly String PureDataDirectory = "Data/";
        public static readonly String DataDirectory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDataDirectory;
        public static readonly String ScriptsDirectory = AssetManagerUtil.GetStreamingAssetsPath() + "/Scripts/";
        public static readonly String ShadersDirectory = AssetManagerUtil.GetStreamingAssetsPath() + "/Shaders/";

        public static String ScriptsModDirectory(String modFolder)
        {
            if (modFolder == null || modFolder.Length == 0)
                return ScriptsDirectory;
            return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/Scripts/";
        }
        public static String ShadersModDirectory(String modFolder)
        {
            if (modFolder == null || modFolder.Length == 0)
                return ShadersDirectory;
            return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/Shaders/";
        }

        public static class Items
        {
            public static readonly String PureDirectory = PureDataDirectory + "Items/";
            public static readonly String Directory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;

            public static String ItemsFile => "Items.csv";
            public static String StatsFile => "Stats.csv";
            public static String ArmorsFile => "Armors.csv";
            public static String WeaponsFile => "Weapons.csv";
            public static String SynthesisFile => "Synthesis.csv";
            public static String ItemEffectsFile => "ItemEffects.csv";
            public static String ShopItems => "ShopItems.csv";

            public static String ModDirectory(String modFolder)
            {
                if (modFolder == null || modFolder.Length == 0)
                    return Directory;
                return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;
            }
        }

        public static class Characters
        {
            public static readonly String PureDirectory = PureDataDirectory + "Characters/";
            public static readonly String Directory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;

            public static String CommandsFile => "Commands.csv";
            public static String CommandSetsFile => "CommandSets.csv";
            public static String CommandTitlesFile => "CommandTitles.csv";
            public static String DefaultEquipmentsFile => "DefaultEquipment.csv";
            public static String BaseStatsFile => "BaseStats.csv";
            public static String Leveling => "Leveling.csv";

            public static String ModDirectory(String modFolder)
            {
                if (modFolder == null || modFolder.Length == 0)
                    return Directory;
                return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;
            }

            public static class Abilities
            {
                // ReSharper disable once MemberHidesStaticFromOuterClass
                public static readonly String PureDirectory = Characters.PureDirectory + "Abilities/";
                public static readonly String Directory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;

                public static String GemsFile => "AbilityGems.csv";
                public static String SAFeaturesFile => "AbilityFeatures.txt";

                public static String GetPresetAbilitiesPath(CharacterPresetId presetId, String modFolder = null)
                {
                    if (modFolder == null || modFolder.Length == 0)
                        return Directory + presetId + ".csv";
                    return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory + presetId + ".csv";
                }
                public static String ModDirectory(String modFolder)
                {
                    if (modFolder == null || modFolder.Length == 0)
                        return Directory;
                    return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;
                }
            }
        }

        public static class Battle
        {
            public static readonly String PureDirectory = PureDataDirectory + "Battle/";
            public static readonly String Directory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;

            public static String StatusSetsFile => "StatusSets.csv";
            public static String ActionsFile => "Actions.csv";
            public static String StatusDataFile => "StatusData.csv";

            public static String ModDirectory(String modFolder)
            {
                if (modFolder == null || modFolder.Length == 0)
                    return Directory;
                return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;
            }
        }
    }
}