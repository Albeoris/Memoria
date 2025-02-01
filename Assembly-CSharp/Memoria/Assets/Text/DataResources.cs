using Memoria.Data;
using System;

namespace Memoria.Assets
{
    public static class DataResources
    {
        // In general:
        // - Default file path is obtained by DataResources.SECTION.PureDirectory + DataResources.SECTION.TYPEFile
        // - The Asset Path Prefix to be used in TryFindAssetInModOnDisc is: AssetManagerUtil.GetStreamingAssetsPath() + "/"
        public const String PureDataDirectory = "Data/";
        public const String PureScriptsDirectory = "Scripts/";
        public const String PureShadersDirectory = "Shaders/";
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

            public const String ItemsFile = "Items.csv";
            public const String StatsFile = "Stats.csv";
            public const String ArmorsFile = "Armors.csv";
            public const String WeaponsFile = "Weapons.csv";
            public const String SynthesisFile = "Synthesis.csv";
            public const String ItemEffectsFile = "ItemEffects.csv";
            public const String ShopItems = "ShopItems.csv";
            public const String InitialItemsFile = "InitialItems.csv";
            public const String MixItemsFile = "MixItems.csv";
            public const String ItemEquipPatchFile = "ItemEquipPatch.txt";
            public const String ShopPatchFile = "ShopPatch.txt";

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

            public const String CommandsFile = "Commands.csv";
            public const String CommandSetsFile = "CommandSets.csv";
            public const String CommandTitlesFile = "CommandTitles.csv";
            public const String DefaultEquipmentsFile = "DefaultEquipment.csv";
            public const String CharacterParametersFile = "CharacterParameters.csv";
            public const String CharacterBattleParametersFile = "BattleParameters.csv";
            public const String BaseStatsFile = "BaseStats.csv";
            public const String Leveling = "Leveling.csv";

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

                public const String GemsFile = "AbilityGems.csv";
                public const String SAFeaturesFile = "AbilityFeatures.txt";

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

            public const String StatusSetsFile = "StatusSets.csv";
            public const String ActionsFile = "Actions.csv";
            public const String StatusDataFile = "StatusData.csv";
            public const String MagicSwordSetFile = "MagicSwordSets.csv";

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

            public const String CHRControlFile = "TransportControls.csv";
            public const String WeatherColorFile = "WeatherColors.csv";
            public const String EnvironmentPatchFile = "Environment.txt";

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

            public const String LocalizationPatchFile = "LocalizationPatch.txt";

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

            public const String TripleTriadFile = "TripleTriad.csv";

            public static String ModDirectory(String modFolder)
            {
                if (String.IsNullOrEmpty(modFolder))
                    return Directory;
                return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;
            }
        }

        public static class SpecialEffects
        {
            public static readonly String PureDirectory = PureDataDirectory + "SpecialEffects/";
            public static readonly String Directory = AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;

            public const String SPSPrototypeFile = "Common/SPS.csv";
            public const String SHPPrototypeFile = "Common/SHP.csv";

            public static String ModDirectory(String modFolder)
            {
                if (String.IsNullOrEmpty(modFolder))
                    return Directory;
                return modFolder + "/" + AssetManagerUtil.GetStreamingAssetsPath() + "/" + PureDirectory;
            }
        }
    }
}
