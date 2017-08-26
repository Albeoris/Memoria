using System;
using Memoria.Data;

namespace Memoria.Assets
{
    public static class DataResources
    {
        public static readonly String DataDirectory = AssetManagerUtil.GetStreamingAssetsPath() + "/Data/";
        public static readonly String ScriptsDirectory = AssetManagerUtil.GetStreamingAssetsPath() + "/Scripts/";
        public static readonly String ShadersDirectory = AssetManagerUtil.GetStreamingAssetsPath() + "/Shaders/";

        public static class Items
        {
            public static readonly String Directory = DataDirectory + "Items/";

            public static String ItemsFile => Directory + "Items.csv";
            public static String StatsFile => Directory + "Stats.csv";
            public static String ArmorsFile => Directory + "Armors.csv";
            public static String WeaponsFile => Directory + "Weapons.csv";
            public static String SynthesisFile => Directory + "Synthesis.csv";
            public static String ItemEffectsFile => Directory + "ItemEffects.csv";
            public static String ShopItems => Directory + "ShopItems.csv";
        }

        public static class Characters
        {
            public static readonly String Directory = DataDirectory + "Characters/";

            public static String CommandsFile => Directory + "Commands.csv";
            public static String CommandSetsFile => Directory + "CommandSets.csv";
            public static String CommandTitlesFile => Directory + "CommandTitles.csv";
            public static String DefaultEquipmentsFile => Directory + "DefaultEquipment.csv";
            public static String BaseStatsFile => Directory + "BaseStats.csv";
            public static String Leveling => Directory + "Leveling.csv";

            public static class Abilities
            {
                // ReSharper disable once MemberHidesStaticFromOuterClass
                public static readonly String Directory = Characters.Directory + "Abilities/";

                public static String GemsFile => Directory + "AbilityGems.csv";

                public static String GetPresetAbilitiesPath(CharacterPresetId presetId)
                {
                    return Directory + presetId + ".csv";
                }
            }
        }

        public static class Battle
        {
            public static readonly String Directory = DataDirectory + "Battle/";

            public static String StatusSetsFile => Directory + "StatusSets.csv";
            public static String ActionsFile => Directory + "Actions.csv";
        }
    }
}