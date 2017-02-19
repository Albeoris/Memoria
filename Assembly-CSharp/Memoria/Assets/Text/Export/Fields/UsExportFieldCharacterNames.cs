using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    public sealed class UsExportFieldCharacterNames : ExportFieldCharacterNames
    {
        protected override Dictionary<String, String[]> InitializeFields()
        {
            return new Dictionary<String, String[]>
            {
                {"0022_EVT_STARTUP_TST_AC_E", Get0022_EVT_STARTUP_TST_AC_E()},
                {"0033_EVT_BATTLE_WM_0230", Get0033_EVT_BATTLE_WM_0230()},
                {"0047_EVT_STARTUP_SPS_E", Get0047_EVT_STARTUP_SPS_E()},
                {"0051_EVT_ALEX1_TS_CARGO_3", Get0051_EVT_ALEX1_TS_CARGO_3()},
                {"0074_EVT_BATTLE_SIOTES01", Get0074_EVT_BATTLE_SIOTES01()},
                {"0276_EVT_BATTLE_B3_114", Get0276_EVT_BATTLE_B3_114()},
                {"0358_EVT_DALI_V_DL_FWM", Get0358_EVT_DALI_V_DL_FWM()},
                {"0485_EVT_BATTLE_WM_0205", Get0485_EVT_BATTLE_WM_0205()},
                {"0595_EVT_BATTLE_WM_1000", Get0595_EVT_BATTLE_WM_1000()},
                {"0694_EVT_BATTLE_B3_124", Get0694_EVT_BATTLE_B3_124()},
                {"0738_EVT_BATTLE_CAM_046", Get0738_EVT_BATTLE_CAM_046()},
                {"0908_EVT_TRENO1_TR_GAT_0", Get0908_EVT_TRENO1_TR_GAT_0()}
            };
        }

        protected override String[] InitializeGeneral()
        {
            return new[]
            {
                // World
                "Alexandria",
                "Evil Forest",
                "Ice Cavern",
                "Treno",
                "South Gate",
                "Dali",
                "North Gate",
                "Gizamaluke’s Grotto",
                "King of Burmecia",
                "Burmecia",
                "Cleyra",
                "Chocobo’s Forest",
                "Pinnacle Rocks",
                "Lindblum",
                "Desert Palace",
                "Mognet Central",
                "Black Mage Village",
                "Fossil Roo",
                "Madain Sari",
                "Conde Petie Mountain Path",
                "Conde Petie",
                "Iifa Tree",
                "Daguerreo",
                "Oeilvert",
                "Ipsen’s Castle",
                "Shimmering Island",
                "Esto Gaza",
                "Memoria",

                // Moogles
                "Mosh",
                "Mogmi",
                "Mogrika",
                "Stiltzkin",
                "Artemicion",

                // Tantalus
                "Baku",
                "Cinna",
                "Blank",
                "Ruby",
                "Benero",
                "Zenero",
                "Genero",

                // Famous
                "Beatrix",
                "Cid Fabool",
                "Regent Cid",
                "Cid",
                "Minister Artania",
                "Artania",
                "Sir Fratley",
                "Fratley",
                "Doctor Tot",
                "Tot",
                "Hilda Garde",
                "Hilda",
                "Quale",
                "Quan",
                "Choco",
                "Mikoto",
                "Lowell",
                "Mog",
                "Puck",
                "Ramuh",

                // Enemies
                "Brahne",
                "Garland",
                "Kuja",
                "Lani",
                "Zorn and Thorn",
                "Zorn",
                "Thorn",
                "Black Mage",
                "Black Waltz",
                "No.",

                // Alexandria
                "Ashley",
                "Dante the Signmaker",
                "Dishmeister",
                "Doug",
                "Eggmeister",
                "Fish Man",
                "Flower Girl",
                "Hippaul",
                "Hippolady",
                "Ilia",
                "Kupo",
                "Nikolai",
                "Maggie",
                "Michelle",
                "Mick",
                "Onionmeister",
                "Pluto Knight",
                "Ovenmeister",
                "Mullenkedheim",
                "Dojebon",
                "Breireicht",
                "Laudo",
                "Kohel",
                "Blutzen",
                "Retired Boatman",
                "Boatman",
                "Ryan",
                "Ticketmaster",
                "Tom’s Mother",
                "Tom",
                "Tour Guide",

                // Dali
                "Dutiful Daughter Slai",
                "Mayor Kapu",

                // Lindblum
                "Alice",
                "Bobo",
                "Bunce",
                "Card Freak Gon",
                "Dragoos",
                "Gray",
                "Guy",
                "Kal",
                "Lucella",
                "Margaret",
                "Marsha",
                "Pepe",
                "Grandma Pickle",
                "Pigeon Lover",
                "Theodore",
                "Thomas",
                "Torres",
                "Wayne",
                "Wei",
                "Engineer Zebolt",

                // Treno
                "Card Game Usher",
                "Card Seller",
                "Mario",
                "Natalie",
                "Queen Stella",

// Cleyra
                "Adam",
                "Burmecian Soldier Dan",
                "Burmecian Soldier Din",
                "Burmecian Soldier Gary",
                "Burmecian Soldier Gidd",
                "Cleyran High Priest",
                "Jack",
                "Flower Maiden Sharon",
                "Moon Maiden Claire",
                "Star Maiden Nina",
                "Water Maiden Shannon",
                "Wind Maiden Eileen",
                "Forest Oracle Kildea",
                "Night Oracle Donnegan",
                "Sand Oracle Satrea",
                "Sky Oracle Mylan",
                "Sun Oracle Flourin",
                "Tree Oracle Wylan",
                "Refugee Learie",
                "Refugee Lorena",

// Madain Sari
                "Chimomo",
                "Mocha",
                "Moco",
                "Momatose",
                "Morrison",

// Daguerreo
                "Four-armed Man",

// Other
                "Lord Avon",
                "Princess Cornelia",
                "Gizamaluke",
                "Hunt",
                "Learie",
                "King Leo",
                "Mogster",
                "Moguo",
                "Part-time Worker Mary",
                "Part-time Worker Jeff",
                "Jeff",
                "Prince Schneider",
                "Shannon"
            };
        }

        private static String[] Get0033_EVT_BATTLE_WM_0230()
        {
            return new[]
            {
                "Alleyway Jack"
            };
        }

        private static String[] Get0047_EVT_STARTUP_SPS_E()
        {
            return new[]
            {
                "Bratty Marin",
                "Innkeeper Hal",
                "Slai’s Father",
                "Snot-nosed Gudo",
                "Shopkeeper Eve",
                "Trude",
                "Pasty Yacha",
                "Yaff"
            };
        }

        private static String[] Get0276_EVT_BATTLE_B3_114()
        {
            return new[]
            {
                "Chet",
                "Dolf",
                "Elena",
                "Gus",
                "Heather",
                "Locke",
                "Marian",
                "Marolo",
                "Aspiring Artist Michael",
                "Nimitz",
                "Poncho",
                "Grandma Potpourri",
                "Pricilla",
                "Rio",
                "Rita",
                "Roch",
                "Rosco",
                "Rupta",
                "Shig",
                "Priest Theodore",
                "Tiffany",
                "Tim",
                "Uzu",
                "Widget",
                "Yaup"
            };
        }

        private static String[] Get0485_EVT_BATTLE_WM_0205()
        {
            return new[]
            {
                "Devon",
                "Gabin",
                "Justin",
                "Struggling Artist Michael",
                "Nicole",
                "Seth"
            };
        }

        private static String[] Get0595_EVT_BATTLE_WM_1000()
        {
            return new[]
            {
                "Joanna",
                "Self-proclaimed Artist Michael",
                "Olivier"
            };
        }

        private static String[] Get0022_EVT_STARTUP_TST_AC_E()
        {
            return new[]
            {
                "Umaeda"
            };
        }

        private static String[] Get0738_EVT_BATTLE_CAM_046()
        {
            return new[]
            {
                "Cannoneer",
                "Senior Officer"
            };
        }

        private static String[] Get0908_EVT_TRENO1_TR_GAT_0()
        {
            return new[]
            {
                "Gatz",
                "Lisa"
            };
        }

        private static String[] Get0694_EVT_BATTLE_B3_124()
        {
            return new[]
            {
                "Dark Phantom",
                "Defense Phantom",
                "Master Phantom",
                "Strong Phantom"
            };
        }

        private static String[] Get0358_EVT_DALI_V_DL_FWM()
        {
            return new[]
            {
                "Colin",
                "Ipsen"
            };
        }

        private static String[] Get0074_EVT_BATTLE_SIOTES01()
        {
            return new[]
            {
                "Hans"
            };
        }

        private static String[] Get0051_EVT_ALEX1_TS_CARGO_3()
        {
            return new[]
            {
                "Moguta"
            };
        }
    }
}