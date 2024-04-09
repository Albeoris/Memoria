using System;
using System.Collections.Generic;
using System.Linq;
using Memoria;
using Memoria.Prime;
public static class NarrowMapList
{
    public static Boolean IsCurrentMapNarrow(Int32 ScreenWidth) => IsNarrowMap(FF9StateSystem.Common.FF9.fldMapNo, PersistenSingleton<EventEngine>.Instance?.fieldmap?.camIdx ?? -1, ScreenWidth);
    public static Boolean IsNarrowMap(Int32 mapId, Int32 camId, Int32 ScreenWidth)
    {
        if (SpecificScenesNarrow(mapId))
            return true;

        if (MapWidth(mapId) <= ScreenWidth)
            return true;


        //if (ListFullNarrow.Contains(mapId))
        //    return true;
        
        //if (ListPartialNarrow.TryGetValue(mapId, out HashSet<Int32> narrowCams) && narrowCams.Contains(camId))
        //    return true;
        //Log.Message("camId:" + camId + ", mapid:" + mapId);

        return false;
    }
    public static Boolean SpecificScenesNarrow(Int32 mapId)
    {
        Int32 currIndex = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
        foreach (KeyValuePair<int, int> entry in SpecificScenesNarrow_List)
        {
            if (mapId == entry.Key && currIndex == entry.Value)
                return true;
            // hall alexandria: meeting garnet, zorn thorn, steiner calling
            if (mapId == 153 && (currIndex == 325 || currIndex == 328 || currIndex == 316)) 
                return true;
        }
        return false;
    }

    public static Int32 MapWidth(int mapId)
    {
        Int32 width = 500;

        if (ListFullNarrow.Contains(mapId) || SpecificScenesNarrow(mapId))
            width = 320;

        foreach (KeyValuePair<int, int> entry in actualNarrowMapWidthDict)
        {
            if (mapId == entry.Key && !SpecificScenesNarrow(entry.Key))
                width = (Int32)entry.Value;
        }

        //Log.Message("width:" + width + "PersistenSingleton<EventEngine>.Instance?.fieldmap?.camIdx" + PersistenSingleton<EventEngine>.Instance?.fieldmap?.camIdx);
        return width;
    }

    public static readonly Dictionary<int, int> SpecificScenesNarrow_List = new Dictionary<int, int>
    {
        // {mapNo,index}
        {50,0},      // first scene
        {150,325},   // Zidane infiltrate Alex Castle
        //{154,304},   // cutscene zorn&thorn
        //{153,328}, // steiner guards call // can't have twice the same key TOFIX
        {254,26},    // MBG103 - Evil Forest
        {352,3},     // Arrival at Dali: vivi visible before sleeping
        {355,18},    // Steiner to the barmaid
        {600,32},    // Throne, meet cid
        {606,0},     // telescope
        {615,57},    // Meet garnet on Lindblum castle
        {1554,7},    // MBG109 - roots
        //{1600,9999}, // First time Madain Sari
        {1602,16},   // scene at Madain Sari night w/ Vivi/Zidane/Eiko eavesdropping, bugged if you see too much
        {1823,331},  // Garnet coronation, garnet visible
        {1815,0},    // Love quiproquo at the docks
        {1816,315},  // Love quiproquo at the docks
        {2007,2},    // MBG111 - Alex castle changing
        {2211,8},    // Lindblum meeting after Alexander scene: ATE with kuja at his ship, Zorn & Thorn visible too soon and blending
        {2705,-1},   // Pandemonium, you're not alone sequence, several glitches
        {2706,-1},   // Pandemonium, you're not alone sequence, several glitches
        {2707,-1},   // Pandemonium, you're not alone sequence, several glitches
        {2708,-1},   // Pandemonium, you're not alone sequence, several glitches
        {2711,0},    // Pandemonium, people are waiting in line after Kuja is defeated
        {2905,154}   // MBG118 - Memoria pink castle
    };

    public static readonly Dictionary<Int32, HashSet<Int32>> ListPartialNarrow = new Dictionary<Int32, HashSet<Int32>>()
    {
        // Not yet implemented
        // For now, using this "per camera" narrow list bugs, surely because of the camera position shift in FieldMap.CenterCameraOnPlayer
        //{ 0154, new HashSet<Int32>() { 0 } }, // A. Castle/Hallway
        //{ 1215, new HashSet<Int32>() { 0 } }, // A. Castle/Hallway
        //{ 1807, new HashSet<Int32>() { 0 } }, // A. Castle/Hallway

    };

    public static readonly HashSet<Int32> ListFullNarrow = new HashSet<Int32>()
    {
        0052, // Prima Vista/Meeting Rm
        0053, // Prima Vista/Meeting Rm
        0055, // Prima Vista/Music Room
        0056, // S. Gate
        0058, // Prima Vista/Storage
        0059, // Prima Vista/Interior
        0060, // Prima Vista/Interior
        0061, // Prima Vista/Interior
        0062, // Prima Vista/Interior
        0063, // Prima Vista/Interior
        0065, // Prima Vista/Interior
        0066, // Prima Vista/Interior
        0067, // Prima Vista/Interior
        0068, // A. Castle/Throne
        0069, // A. Castle/Throne
        0100, // Alexandria/Main Street
        0102, // Alexandria/Main Street
        0104, // Alexandria/Shop
        0105, // Alexandria/Alley
        0108, // Alexandria/Item Shop
        0109, // Alexandria/Wpn. Shop
        0114, // Alexandria/Residence
        0116, // Alexandria/Rooftop
        0150, // A. Castle/Guardhouse
        0151, // A. Castle/Throne
        0153, // A. Castle/Hallway
        0154,
        0157, // A. Castle/Kitchen
        0160, // A. Castle/Courtyard
        0161, // A. Castle/Courtyard
        0162, // A. Castle/West Tower
        0163, // A. Castle/West Tower
        0164, // A. Castle/West Tower
        0165, // A. Castle/West Tower
        0166, // A. Castle/West Tower
        0167, // A. Castle/Library
        0201, // Prima Vista/Bridge
        0203, // Prima Vista/Meeting Rm
        0205, // Prima Vista/Hallway
        0206, // Prima Vista/Crash Site
        0207, // Prima Vista/Cabin
        0209, // Prima Vista/Event
        0251, // Evil Forest/Trail
        0252, // Evil Forest/Trail
        //0254, // Evil Forest/Swamp
        0255, // Evil Forest/Riverbank
        0256, // Evil Forest/Trail
        0259, // Evil Forest/Trail
        0261, // Evil Forest/Exit
        0262, // Evil Forest/Exit
        0300, // Ice Cavern/Entrance
        0301, // Ice Cavern/Ice Path
        0305, // Ice Cavern/Ice Path
        0306, // Ice Cavern/Cave
        0308, // Ice Cavern/Waterfall
        0309, // Ice Cavern/Waterfall
        0310, // Ice Cavern/Waterfall
        0311, // Ice Cavern/Exit
        0354, // Dali/Wpn. Shop
        0357, // Dali/Windmill 2F
        0358, // Dali/Windmill
        0359, // Dali/???
        0400, // Dali/Underground
        0405,
        0407,
        0452, // Dali/Field
        0453, // Dali/Field
        0454, // Dali/Field
        0455, // Mountain/Base
        0456, // Mountain/Summit
        0457, // Mountain/Shack
        0500, // Cargo Ship/Deck
        0502, // Cargo Ship/Bridge
        0503, // Cargo Ship/Bridge
        0505, // Cargo Ship/Rear Deck
        0506, // Cargo Ship/Deck
        0550,
        0551, // Lindblum/B.D. Station
        0553, // Lindblum/Inn
        0556,
        0560, // Lindblum/Synthesist
        0561,
        0565,
        0566,
        0567, // Lindblum/Theater Ave.
        0568,
        0569,
        0570, // Lindblum/Industrial Wa
        0571,
        0574, // Lindblum/Festival
        0576, // Lindblum/Festival
        0600, // L. Castle/Royal Cham.
        0601, // L. Castle/Lift
        0606, // L. Castle/Event
        0607, // L. Castle/Hangar
        0609, // L. Castle/Castle Bridg
        0613,
        0620,
        0656,
        0657,
        0658,
        0659,
        0663,
        0701, // Gizamaluke/Entrance
        0705,
        0750, // Burmecia/Gate
        0751,
        0753,
        0754, // Burmecia/Residence
        0755,
        0758, // Burmecia/Pathway
        0760, // Burmecia/Uptown Area
        0764, // Burmecia/Vault
        0765, // Burmecia/Armory
        0766, // Burmecia/Palace
        0767, // Burmecia/Palace
        0800, // S. Gate/Bohden Gate
        0802, // S. Gate/Bohden Sta.
        0803,
        0806,
        0813, // S. Gate/Berkmea
        0814, // S. Gate/Berkmea
        0816, // S. Gate/Berkmea
        0851,
        0855,
        0901,
        0911,
        0913,
        0930, // Treno/Tot Residence
        0932, // Treno/Event
        0950,
        0951, // Gargan Roo/Passage
        0954, // Gargan Roo/Tunnel
        1000, // Cleyra/Tree Roots
        1001, // Cleyra/Tree Roots
        1002, // Cleyra/Tree Roots
        1003, // Cleyra/Tree Trunk
        1006, // Cleyra/Tree Trunk
        1007, // Cleyra/Tree Trunk
        1009, // Cleyra/Tree Trunk
        1013, // Cleyra/Tree Trunk
        1017, // Cleyra/Tree Trunk
        1018,
        1050, // Cleyra/Tree Trunk
        1054, // Cleyra/Windmill Area
        1058, // Cleyra/Cathedral
        1100, // Cleyra/Tree Trunk
        1104,
        1108, // Cleyra/Cathedral
        1150, // Red Rose/Deck
        1151, // Red Rose/Cabin
        1153, // Red Rose/Bridge
        1200, // A. Castle/Throne
        1201,
        1205, // A. Castle/Chapel
        1208, // A. Castle/Dungeon
        1210, // A. Castle/West Tower
        1212, // A. Castle/East Tower
        1213, // A. Castle/Guardhouse
        1214, // A. Castle/Hallway
        1215,
        1216,
        1218,
        1221, // A. Castle/Courtyard
        1222, // A. Castle/Courtyard
        1226, // A. Castle/Library
        1250, // A. Castle/Event
        1251, // Pinnacle Rocks/Hole
        1252, // Pinnacle Rocks/Path
        1253, // Pinnacle Rocks/Path
        1254,
        1300, // Lindblum/Hunter’s Gate
        1301, // Lindblum/B.D. Station
        1303, // Lindblum/Inn
        1308, // Lindblum/Synthesist
        1312,
        1313,
        1314, // Lindblum/Theater Ave.
        1350,
        1351,
        1356,
        1357, // L. Castle/Hangar
        1359,
        1363,
        1370,
        1401, // Fossil Roo/Cavern
        1403, // Fossil Roo/Cavern
        1404,
        1406, // Fossil Roo/Nest
        1408,
        1410, // Fossil Roo/Nest
        1414,
        1424,
        1452, // Mage Village/Cemetery
        1453, // Mage Village/Cemetery
        1455, // Mage Village/Synthesis
        1456,
        1457, // Mage Village/Rooftop
        1458,
        1459, // Mage Village/Water Mil
        1463, // Dead Forest/Grove
        1464, // Dead Forest/Dead End
        1500,
        1506,
        1507, // Conde Petie/Pathway
        1508,
        1509, // Conde Petie/Item Shop
        1556, // Mountain Path/Roots
        1557, // Mountain Path/Roots
        1600,
        1601,
        1602,
        1604, // Mdn. Sari/Eidolon Wall
        1605, // Mdn. Sari/Eidolon Wall
        1606, // Mdn. Sari/Resting Room
        1607, // Mdn. Sari/Kitchen
        1608, // Mdn. Sari/Secret Room
        1609, // Mdn. Sari/Cove
        1610, // Mdn. Sari/Cove
        1650,
        //1651, // Iifa Tree/Tree Roots
        1652, // Iifa Tree/Roots
        1655, // Iifa Tree/Tree Path
        1656, // Iifa Tree/Eidolon Moun
        1657, // Iifa Tree/Tree Roots
        1658, // Iifa Tree/Silver Dragon
        1660,
        1661,
        1662,
        1663,
        1700,
        1701,
        1702,
        1704, // Mdn. Sari/Eidolon Wall
        1705, // Mdn. Sari/Resting Room
        1706, // Mdn. Sari/Kitchen
        1707, // Mdn. Sari/Secret Room
        1750, // Iifa Tree/Hollow Roots
        1751, // Iifa Tree/Inner Roots
        1752, // Iifa Tree/Inner Roots
        1753, // Iifa Tree/Inner Roots
        1755, // Iifa Tree/Bottom
        1756, // Iifa Tree/Bottom
        1757,
        //1758, // Iifa Tree/Tree Roots
        1800, // A. Castle/Tomb
        1803, // A. Castle/Guardhouse
        1806, // A. Castle/Hallway
        1807, // A. Castle/Hallway
        1808,
        1810,
        1813, // A. Castle/Courtyard
        1814, // A. Castle/Courtyard
        //1816, // A. Castle/Courtyard
        1817, // A. Castle/Neptune
        1818, // A. Castle/Neptune
        1820, // A. Castle/West Tower
        1822, // A. Castle/Library
        1850, // Alexandria/Main Street
        1852,
        1854, // Alexandria/Alley
        1857, // Alexandria/Item Shop
        1858, // Alexandria/Wpn. Shop
        1863,
        1866, // Alexandria/Dock
        1901,
        1911,
        1913,
        1951,
        1952,
        1953, // Quan’s/Fishing Area
        2000, // Hilda Garde 2/Deck
        2002,
        2004,
        2005, // A. Castle/Altar
        2006,
        //2007, // A. Castle/Altar
        2008, // A. Castle/Altar
        2050, // Alexandria/Main Street
        2052,
        2055,
        2101, // Lindblum/B.D. Station
        2103, // Lindblum/Inn
        2108, // Lindblum/Synthesist
        2109, // Lindblum/Wpn. Shop
        2112,
        2113,
        2114, // Lindblum/Theater Ave.
        2150, // L. Castle/Royal Cham.
        2151, // L. Castle/Lift
        2157, // L. Castle/Hangar
        2159, // L. Castle/Castle Bridg
        2163,
        2171,
        2200,
        2202, // Palace/Dungeon
        2203, // Palace/Rack
        2204, // Palace/Odyssey
        2205, // Palace/Odyssey
        2208, // Palace/Hallway
        2212,
        2213,
        2217, // Palace/Stairwell
        2222,
        2250, // Oeilvert/Outside
        2254, // Oeilvert/Ship Display
        2255, // Oeilvert/Stairwell
        2257, // Oeilvert/Display
        2260, // Oeilvert/Tombstone
        2261, // Oeilvert/Bridge
        2303,
        2305, // Esto Gaza/Path
        2351, // Gulug/Well
        2352,
        2353,
        2354, // Gulug/Room
        2355,
        2356, // Gulug/Room
        2361, // Gulug/Well
        2362,
        2363, // Gulug/Path
        2365,
        2400, // A. Castle/Neptune
        2405, // A. Castle/Courtyard
        2406,
        2450, // Alexandria/Main Street
        2451,
        2453, // Alexandria/Alley
        2458, // Alexandria/Dock
        2500, // I. Castle/Entrance
        2501, // I. Castle/Entrance
        2502, // I. Castle/Hall
        2503,
        2505, // I. Castle/Inverted Roo
        2510, // I. Castle/Mural Room
        2512, // I. Castle/Mural Room
        2513,
        2551,
        2552, // Earth Shrine/Interior
        2601,
        2602, // Terra/Stepping Stones
        2606, // Terra/Tree base
        2608, // Terra/Event
        2650,
        2654, // Bran Bal/Pond
        2657, // Bran Bal/Storage
        2658,
        2701, // Pand./Path
        2706,
        2715, // Pand./Event
        2719, // Pand./Exit
        2752, // Invincible/Bridge
        2756, // Red Rose/Bridge
        2851, // Hilda Garde 3/Engine
        2855,
        2856,
        2900, // Memoria/Outside
        2901, // Memoria/Entrance
        2902, // Memoria/Stairs of Time
        2904, // Memoria/Outer Path
        //2905,
        2906,
        2908, // Memoria/Time Interval
        2909, // Memoria/Ruins
        2910, // Memoria/Lost Memory
        2912, // Memoria/World Fusion
        2913, // Memoria/Portal
        2914, // Memoria/Birth
        2915,
        2917, // Memoria/Gaia’s Birth
        2918, // Memoria/Stairs
        2919, // Memoria/Gate to Space
        2920,
        2922,
        2924,
        2925,
        2926,
        2927, // Crystal World
        2928,
        2929, // last/cw mbg a
        2930, // last/cw mbg 0
        2932, // last/cw brg 0
        2933, // last/cw mbg 1
        2934, // last/cw mbg 2
        2950, // Chocobo’s Forest
        2953, // Chocobo’s Dream World
        3001,
        3005,
        3008, // Ending/Prima Vista - Meeting Room
        3009,
        3010, // Ending/TH
        3011, // Ending/TH
        3052, // Mage Village/Cemetery
        3054, // Mage Village/Synthesis
        3055,
        3056, // Mage Village/Rooftop
        3057,
        3058, // Mage Village/Water Mil
        3100, // Mog Post
    };

    public static readonly Dictionary<int, int> mapCameraMargin = new Dictionary<int, int>
    {
        //{mapNo,pixels on each side to crop because of scrollable}
        {1051,8},
        {1057,16},
        {1058,16},
        {1060,16},
        {1652,16},
        {1653,16},
        //{154,16},
    };

    public static readonly Dictionary<int, int> actualNarrowMapWidthDict = new Dictionary<int, int>
    {
        //{mapNo,(actualWidth - 2)}
        //{153,430},
        {203,334},
        //{502,334},
        //{503,334},
        {760,334},
        {814,334},
        {816,334},
        {1151,334},
        {1458,334},
        {1500,334},
        {1506,334},
        {1605,334},
        {1606,334},
        {1608,334},
        {1660,334},
        {1661,334},
        {1662,334},
        {1705,334},
        {1707,334},
        {1751,334},
        {2202,334},
        {2204,334},
        {2205,334},
        {2208,334},
        {2254,334},
        {2257,334},
        {2303,334},
        {2365,334},
        {2513,334},
        {2756,334},
        {2932,334},
        {3057,334},
        {114,350},
        {550,350},
        {620,350},
        {802,350},
        {803,350},
        {1212,350},
        {1300,350},
        {1370,350},
        {1508,350},
        {1650,350},
        {1752,350},
        {1757,350},
        {1863,350},
        {1951,350},
        {1952,350},
        {2000,350},
        {2055,350},
        {2771,350},
        {2203,350},
        {2261,350},
        {2356,350},
        {2362,350},
        {2500,350},
        {2501,350},
        {2654,350},
        {60,366},
        {150,366},
        {161,366},
        {201,366},
        {262,366},
        {565,366},
        {911,366},
        {1213,366},
        {1222,366},
        {1251,366},
        {1254,366},
        {1312,366},
        {1403,366},
        {1803,366},
        {1814,366},
        {1817,366},
        {1911,366},
        {1953,366},
        {2002,366},
        {2004,366},
        {2006,366},
        {2112,366},
        {2400,366},
        {2502,366},
        {2503,366},
        {2650,366},
        {2904,366},
        {2913,366},
        {2928,366},
        {3100,366},
        {102,382},
        {109,382},
        {162,382},
        {206,382},
        {207,382},
        {251,382},
        {252,382},
        {407,382},
        {553,382},
        {556,382},
        {705,382},
        {751,382},
        {813,382},
        {950,382},
        {1017,382},
        {1018,382},
        {1058,380},
        {1108,380},
        {1201,382},
        {1210,382},
        {1303,382},
        {1404,382},
        {1452,382},
        {1453,382},
        {1509,382},
        {1656,382},
        {1820,382},
        {1852,382},
        {1858,382},
        {2052,382},
        {2103,382},
        {2200,382},
        {2222,382},
        {2355,382},
        {2406,382},
        {2451,382},
        {2602,382},
        {2657,382},
        {2851,382},
        {2855,382},
        {2856,382},
        {2915,382},
        {3052,382},
        {55,398},
        {157,398},
        {405,398},
        {456,398},
        {505,398},
        {561,398},
        {566,398},
        {568,398},
        {569,398},
        {571,398},
        {613,398},
        {656,398},
        {657,398},
        {658,398},
        {659,398},
        {663,398},
        {753,398},
        {755,398},
        {806,398},
        {851,398},
        {855,398},
        {901,398},
        {913,398},
        {1054,398},
        {1104,398},
        {1153,398},
        {1218,398},
        {1313,398},
        {1363,398},
        {1408,398},
        {1414,398},
        {1424,398},
        {1456,398},
        {1600,398},
        {1601,398},
        {1602,398},
        {1700,398},
        {1701,398},
        {1702,398},
        {1810,398},
        {1901,398},
        {1913,398},
        {2113,398},
        {2163,398},
        {2212,398},
        {2213,398},
        {2352,398},
        {2353,398},
        {2551,398},
        {2601,398},
        {2658,398},
        {2706,398},
        {2906,398},
        {3005,398},
        {3055,398},
        {1205,384},
        //{154,352},
        //{1215,352},
        {1805,352},
        //{1807,352},
        {1652,336},
        {2552,352},
    };
}
