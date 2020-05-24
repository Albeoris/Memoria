using System;
using System.Linq;

public static class NarrowMapList
{
    public static Boolean IsCurrentMapNarrow() => IsNarrowMap(FF9StateSystem.Common.FF9.fldMapNo);
    public static Boolean IsNarrowMap(Int32 mapId) => Array.BinarySearch(List, mapId) >= 0;

    private static readonly Int32[] List = new[]
    {
        0052, // Prima Vista/Meeting Rm
        0053, // Prima Vista/Meeting Rm
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
        0103, // Alexandria/Square
        0104, // Alexandria/Shop
        0105, // Alexandria/Alley
        0108, // Alexandria/Item Shop
        0116, // Alexandria/Rooftop
        0150, // A. Castle/Guardhouse
        0153, // A. Castle/Hallway
        0154, // A. Castle/Hallway
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
        0209, // Prima Vista/Event
        0255, // Evil Forest/Riverbank
        0256, // Evil Forest/Trail
        0259, // Evil Forest/Trail
        0261, // Evil Forest/Exit
        0301, // Ice Cavern/Ice Path
        0305, // Ice Cavern/Ice Path
        0306, // Ice Cavern/Cave
        0308, // Ice Cavern/Waterfall
        0309, // Ice Cavern/Waterfall
        0310, // Ice Cavern/Waterfall
        0311, // Ice Cavern/Exit
        0351, // Dali/Inn
        0352, // Dali/Inn
        0354, // Dali/Wpn. Shop
        0355, // Dali/Pub
        0357, // Dali/Windmill 2F
        0358, // Dali/Windmill
        0359, // Dali/???
        0400, // Dali/Underground
        0452, // Dali/Field
        0453, // Dali/Field
        0454, // Dali/Field
        0455, // Mountain/Base
        0456, // Mountain/Summit
        0457, // Mountain/Shack
        0500, // Cargo Ship/Deck
        0502, // Cargo Ship/Bridge
        0503, // Cargo Ship/Bridge
        0504, // Cargo Ship/Engine Room
        0505, // Cargo Ship/Rear Deck
        0506, // Cargo Ship/Deck
        0551, // Lindblum/B.D. Station
        0553, // Lindblum/Inn
        0560, // Lindblum/Synthesist
        0562, // Lindblum/Wpn. Shop
        0567, // Lindblum/Theater Ave.
        0570, // Lindblum/Industrial Wa
        0574, // Lindblum/Festival
        0576, // Lindblum/Festival
        0600, // L. Castle/Royal Cham.
        0601, // L. Castle/Lift
        0606, // L. Castle/Event
        0607, // L. Castle/Hangar
        0609, // L. Castle/Castle Bridg
        0615, // L. Castle/Telescope
        0701, // Gizamaluke/Entrance
        0750, // Burmecia/Gate
        0754, // Burmecia/Residence
        0758, // Burmecia/Pathway
        0760, // Burmecia/Uptown Area
        0764, // Burmecia/Vault
        0765, // Burmecia/Armory
        0766, // Burmecia/Palace
        0800, // S. Gate/Bohden Gate
        0802, // S. Gate/Bohden Sta.
        0813, // S. Gate/Berkmea
        0814, // S. Gate/Berkmea
        0816, // S. Gate/Berkmea
        0916, // Treno/Dock
        0930, // Treno/Tot Residence
        0932, // Treno/Event
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
        1015, // Cleyra/Tree Trunk
        1017, // Cleyra/Tree Trunk
        1050, // Cleyra/Tree Trunk
        1051, // Cleyra/Entrance
        1053, // Cleyra/Water Mill Area
        1054, // Cleyra/Windmill Area
        1055, // Cleyra/Town Area
        1057, // Cleyra/Observation Poi
        1058, // Cleyra/Cathedral
        1059, // Cleyra/Cathedral
        1060, // Cleyra/Cathedral
        1100, // Cleyra/Tree Trunk
        1108, // Cleyra/Cathedral
        1110, // Cleyra/Cathedral
        1150, // Red Rose/Deck
        1151, // Red Rose/Cabin
        1153, // Red Rose/Bridge
        1200, // A. Castle/Throne
        1205, // A. Castle/Chapel
        1208, // A. Castle/Dungeon
        1210, // A. Castle/West Tower
        1212, // A. Castle/East Tower
        1213, // A. Castle/Guardhouse
        1214, // A. Castle/Hallway
        1215, // A. Castle/Hallway
        1221, // A. Castle/Courtyard
        1222, // A. Castle/Courtyard
        1226, // A. Castle/Library
        1250, // A. Castle/Event
        1251, // Pinnacle Rocks/Hole
        1252, // Pinnacle Rocks/Path
        1253, // Pinnacle Rocks/Path
        1300, // Lindblum/Hunter’s Gate
        1301, // Lindblum/B.D. Station
        1303, // Lindblum/Inn
        1308, // Lindblum/Synthesist
        1309, // Lindblum/Wpn. Shop
        1314, // Lindblum/Theater Ave.
        1357, // L. Castle/Hangar
        1401, // Fossil Roo/Cavern
        1403, // Fossil Roo/Cavern
        1406, // Fossil Roo/Nest
        1410, // Fossil Roo/Nest
        1452, // Mage Village/Cemetery
        1453, // Mage Village/Cemetery
        1455, // Mage Village/Synthesis
        1457, // Mage Village/Rooftop
        1459, // Mage Village/Water Mil
        1462, // Mage Village/Event
        1463, // Dead Forest/Grove
        1464, // Dead Forest/Dead End
        1507, // Conde Petie/Pathway
        1509, // Conde Petie/Item Shop
        1556, // Mountain Path/Roots
        1557, // Mountain Path/Roots
        1604, // Mdn. Sari/Eidolon Wall
        1605, // Mdn. Sari/Eidolon Wall
        1606, // Mdn. Sari/Resting Room
        1607, // Mdn. Sari/Kitchen
        1608, // Mdn. Sari/Secret Room
        1609, // Mdn. Sari/Cove
        1610, // Mdn. Sari/Cove
        1651, // Iifa Tree/Tree Roots
        1652, // Iifa Tree/Roots
        1653, // Iifa Tree/Tree Roots
        1655, // Iifa Tree/Tree Path
        1656, // Iifa Tree/Eidolon Moun
        1657, // Iifa Tree/Tree Roots
        1658, // Iifa Tree/Silver Drago
        1663, // Iifa Tree/Tree Trunk
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
        1758, // Iifa Tree/Tree Roots
        1800, // A. Castle/Tomb
        1803, // A. Castle/Guardhouse
        1806, // A. Castle/Hallway
        1807, // A. Castle/Hallway
        1813, // A. Castle/Courtyard
        1814, // A. Castle/Courtyard
        1816, // A. Castle/Courtyard
        1817, // A. Castle/Neptune
        1818, // A. Castle/Neptune
        1820, // A. Castle/West Tower
        1822, // A. Castle/Library
        1823, // A. Castle/Hallway
        1850, // Alexandria/Main Street
        1853, // Alexandria/Square
        1854, // Alexandria/Alley
        1857, // Alexandria/Item Shop
        1858, // Alexandria/Wpn. Shop
        1866, // Alexandria/Dock
        1953, // Quan’s/Fishing Area
        2000, // Hilda Garde 2/Deck
        2005, // A. Castle/Altar
        2007, // A. Castle/Altar
        2008, // A. Castle/Altar
        2010,
        2050, // Alexandria/Main Street
        2053, // Alexandria/Square
        2101, // Lindblum/B.D. Station
        2103, // Lindblum/Inn
        2108, // Lindblum/Synthesist
        2109, // Lindblum/Wpn. Shop
        2114, // Lindblum/Theater Ave.
        2150, // L. Castle/Royal Cham.
        2151, // L. Castle/Lift
        2157, // L. Castle/Hangar
        2159, // L. Castle/Castle Bridg
        2202, // Palace/Dungeon
        2203, // Palace/Rack
        2204, // Palace/Odyssey
        2205, // Palace/Odyssey
        2208, // Palace/Hallway
        2217, // Palace/Stairwell
        2220, // Palace/Library
        2250, // Oeilvert/Outside
        2254, // Oeilvert/Ship Display
        2255, // Oeilvert/Stairwell
        2257, // Oeilvert/Display
        2259, // Oeilvert/Star Display
        2260, // Oeilvert/Tombstone
        2261, // Oeilvert/Bridge
        2305, // Esto Gaza/Path
        2351, // Gulug/Well
        2354, // Gulug/Room
        2356, // Gulug/Room
        2361, // Gulug/Well
        2363, // Gulug/Path
        2400, // A. Castle/Neptune
        2405, // A. Castle/Courtyard
        2450, // Alexandria/Main Street
        2453, // Alexandria/Alley
        2458, // Alexandria/Dock
        2500, // I. Castle/Entrance
        2501, // I. Castle/Entrance
        2502, // I. Castle/Hall
        2504, // I. Castle/Small Room
        2505, // I. Castle/Inverted Roo
        2510, // I. Castle/Mural Room
        2512, // I. Castle/Mural Room
        2552, // Earth Shrine/Interior
        2600, // Terra/Hilltop
        2602, // Terra/Stepping Stones
        2605, // Terra/Treetop
        2606, // Terra/Tree base
        2607, // Terra/Bridge
        2608, // Terra/Event
        2651, // Bran Bal/Entrance
        2654, // Bran Bal/Pond
        2657, // Bran Bal/Storage
        2660, // Bran Bal/Hilltop
        2701, // Pand./Path
        2715, // Pand./Event
        2716, // Pand./Event
        2719, // Pand./Exit
        2752, // Invincible/Bridge
        2755, // Hilda Garde/Bridge
        2756, // Red Rose/Bridge
        2850, // Hilda Garde 3/Bridge
        2851, // Hilda Garde 3/Engine
        2900, // Memoria/Outside
        2901, // Memoria/Entrance
        2902, // Memoria/Stairs of Time
        2903, // Memoria/Recollection
        2904, // Memoria/Outer Path
        2908, // Memoria/Time Interval
        2909, // Memoria/Ruins
        2910, // Memoria/Lost Memory
        2912, // Memoria/World Fusion
        2913, // Memoria/Portal
        2914, // Memoria/Birth
        2917, // Memoria/Gaia’s Birth
        2918, // Memoria/Stairs
        2919, // Memoria/Gate to Space
        2920, // Memoria/Emptiness
        2922,
        2924,
        2925,
        2926,
        2927, // Crystal World
        2929, // last/cw mbg a
        2930, // last/cw mbg 0
        2932, // last/cw brg 0
        2933, // last/cw mbg 1
        2934, // last/cw mbg 2
        2950, // Chocobo’s Forest
        2953, // Chocobo’s Dream World
        3008, // Ending/Prima Vista - Meeting Room
        3010, // Ending/TH
        3011, // Ending/TH
        3052, // Mage Village/Cemetery
        3054, // Mage Village/Synthesis
        3056, // Mage Village/Rooftop
        3058, // Mage Village/Water Mil
    }.OrderBy(a => a).ToArray();
}