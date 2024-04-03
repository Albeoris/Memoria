using System;
using Memoria.Data;

namespace Memoria.Assets
{
    public static class DataResources
    {
        // In general:
        // - Default file path is obtained by DataResources.SECTION.PureDirectory + DataResources.SECTION.TYPEFile
        // - The Asset Path Prefix to be used in TryFindAssetInModOnDisc is: AssetManagerUtil.GetStreamingAssetsPath() + "/"
        public static readonly String PureDataDirectory = "Data/";
        public static readonly String PureScriptsDirectory = "Scripts/";
        public static readonly String PureShadersDirectory = "Shaders/";
        public static readonly String DataDirectory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDataDirectory;
        public static readonly String ScriptsDirectory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureScriptsDirectory;
        public static readonly String ShadersDirectory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureShadersDirectory;

        public static String ScriptsModDirectory(String modFolder)
        {
            if (String.IsNullOrEmpty(modFolder))
                return ScriptsDirectory;
            return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/Scripts/";
        }
        public static String ShadersModDirectory(String modFolder)
        {
            if (String.IsNullOrEmpty(modFolder))
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
            public static String InitialItemsFile => "InitialItems.csv";
            public static String ItemEquipPatchFile => "ItemEquipPatch.txt";

            public static String ModDirectory(String modFolder)
            {
                if (String.IsNullOrEmpty(modFolder))
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
            public static String CharacterParametersFile => "CharacterParameters.csv";
            public static String CharacterBattleParametersFile => "BattleParameters.csv";
            public static String BaseStatsFile => "BaseStats.csv";
            public static String Leveling => "Leveling.csv";

            public static String ModDirectory(String modFolder)
            {
                if (String.IsNullOrEmpty(modFolder))
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

                public static String GetPresetAbilitiesPath(CharacterPresetId presetId)
                {
                    return PureDirectory + presetId + ".csv";
                }

                public static String ModDirectory(String modFolder)
                {
                    if (String.IsNullOrEmpty(modFolder))
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
            public static String MagicSwordSetFile => "MagicSwordSets.csv";

            public static String ModDirectory(String modFolder)
            {
                if (String.IsNullOrEmpty(modFolder))
                    return Directory;
                return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;
            }
        }

        public static class World
		{
            public static readonly String PureDirectory = PureDataDirectory + "World/";
            public static readonly String Directory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;

            public static String CHRControlFile => "TransportControls.csv";
            public static String WeatherColorFile => "WeatherColors.csv";
            public static String EnvironmentPatchFile => "Environment.txt";

            public static String ModDirectory(String modFolder)
            {
                if (String.IsNullOrEmpty(modFolder))
                    return Directory;
                return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;
            }
        }

        public static class Text
        {
            public static readonly String PureDirectory = PureDataDirectory + "Text/";
            public static readonly String Directory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;

            public static String LocalizationPatchFile => "LocalizationPatch.txt";

            public static String ModDirectoryWithLang(String modFolder, String langSymbol)
            {
                if (String.IsNullOrEmpty(modFolder))
                    return Directory + langSymbol + "/";
                return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory + langSymbol + "/";
            }

            public static String ModDirectory(String modFolder)
            {
                if (String.IsNullOrEmpty(modFolder))
                    return Directory;
                return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;
            }
        }

        public static class TetraMaster
        {
            public static readonly String PureDirectory = PureDataDirectory + "TetraMaster/";
            public static readonly String Directory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;

            public static String TripleTriadFile => "TripleTriad.csv";

            public static String TripleTriadRulesFile => "TripleTriadRules.csv";

            public static String ModDirectory(String modFolder)
            {
                if (String.IsNullOrEmpty(modFolder))
                    return Directory;
                return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;
            }
        }
    }
}