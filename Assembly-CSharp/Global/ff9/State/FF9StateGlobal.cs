using System;
using System.Collections.Generic;
using Memoria;
using UnityEngine;

public partial class FF9StateGlobal
{
	/* List of fldMapNo:
	50		-> Prima Vista/Cargo Room
	51		-> Prima Vista/Cargo Room
	52		-> Prima Vista/Meeting Rm
	53		-> Prima Vista/Meeting Rm
	54		-> Prima Vista/Hallway
	55		-> Prima Vista/Music Room
	56		-> Prima Vista/Engine Roo
	57		-> Prima Vista/Engine Roo
	58		-> Prima Vista/Storage
	59		-> Prima Vista/Interior
	60		-> Prima Vista/Interior
	61		-> Prima Vista/Interior
	62		-> Prima Vista/Interior
	63		-> Prima Vista/Interior
	64		-> A. Castle/Public Seats
	65		-> Prima Vista/Interior
	66		-> Prima Vista/Interior
	67		-> Prima Vista/Interior
	68		-> A. Castle/Throne
	69		-> A. Castle/Throne
	70		-> Opening-For FMV
	100		-> Alexandria/Main Street
	101		-> Alexandria/Main Street
	102		-> Alexandria/Main Street
	103		-> Alexandria/Square
	104		-> Alexandria/Shop
	105		-> Alexandria/Alley
	106		-> Alexandria/By the Stee
	107		-> Alexandria/Main Street
	108		-> Alexandria/Item Shop
	109		-> Alexandria/Wpn. Shop
	110		-> Alexandria/Synthesist
	111		-> Alexandria/Inn
	112		-> Alexandria/Pub
	113		-> Alexandria/Residence
	114		-> Alexandria/Residence
	115		-> Alexandria/Steeple
	116		-> Alexandria/Rooftop
	117		-> Alexandria/Mini-Theate
	150		-> A. Castle/Guardhouse
	151		-> A. Castle/Throne
	153		-> A. Castle/Hallway
	154		-> A. Castle/Hallway
	155		-> A. Castle/Library
	156		-> A. Castle/Guest Room
	157		-> A. Castle/Kitchen
	158		-> A. Castle/Courtyard
	159		-> A. Castle/Courtyard
	160		-> A. Castle/Courtyard
	161		-> A. Castle/Courtyard
	162		-> A. Castle/West Tower
	163		-> A. Castle/West Tower
	164		-> A. Castle/West Tower
	165		-> A. Castle/West Tower
	166		-> A. Castle/West Tower
	167		-> A. Castle/Library
	200		-> Prima Vista/Hallway
	201		-> Prima Vista/Bridge
	202		-> Prima Vista/Cargo Room
	203		-> Prima Vista/Meeting Rm
	204		-> Prima Vista/Hallway
	205		-> Prima Vista/Hallway
	206		-> Prima Vista/Crash Site
	207		-> Prima Vista/Cabin
	208		-> Prima Vista/Storage
	209		-> Prima Vista/Event
	250		-> Evil Forest/Trail
	251		-> Evil Forest/Trail
	252		-> Evil Forest/Trail
	253		-> Evil Forest/Spring
	254		-> Evil Forest/Swamp
	255		-> Evil Forest/Riverbank
	256		-> Evil Forest/Trail
	257		-> Evil Forest/Nest
	258		-> Evil Forest/Trail
	259		-> Evil Forest/Trail
	260		-> Evil Forest/Trail
	261		-> Evil Forest/Exit
	262		-> Evil Forest/Exit
	300		-> Ice Cavern/Entrance
	301		-> Ice Cavern/Ice Path
	302		-> Ice Cavern/Ice Path
	303		-> Ice Cavern/Icicle Fiel
	304		-> Ice Cavern/Ice Path
	305		-> Ice Cavern/Ice Path
	306		-> Ice Cavern/Cave
	307		-> Ice Cavern/Ice Path
	308		-> Ice Cavern/Waterfall
	309		-> Ice Cavern/Waterfall
	310		-> Ice Cavern/Waterfall
	311		-> Ice Cavern/Exit
	312		-> Ice Cavern/Outside
	350		-> Dali/Village Road
	351		-> Dali/Inn
	352		-> Dali/Inn
	353		-> Dali/Mayor’s House
	354		-> Dali/Wpn. Shop
	355		-> Dali/Pub
	356		-> Dali/Windmill 1F
	357		-> Dali/Windmill 2F
	358		-> Dali/Windmill
	359		-> Dali/???
	400		-> Dali/Underground
	401		-> Dali/Underground
	402		-> Dali/Production Area
	403		-> Dali/Production Area
	404		-> Dali/Entrance
	405		-> Dali/Underground
	406		-> Dali/Underground
	407		-> Dali/Storage Area
	408		-> Dali/Storage Area
	450		-> Dali/Field
	451		-> Dali/Field
	452		-> Dali/Field
	453		-> Dali/Field
	454		-> Dali/Field
	455		-> Mountain/Base
	456		-> Mountain/Summit
	457		-> Mountain/Shack
	500		-> Cargo Ship/Deck
	501		-> Cargo Ship/Deck
	502		-> Cargo Ship/Bridge
	503		-> Cargo Ship/Bridge
	504		-> Cargo Ship/Engine Room
	505		-> Cargo Ship/Rear Deck
	506		-> Cargo Ship/Deck
	507		-> Cargo Ship/Deck
	550		-> Lindblum/Hunter’s Gate
	551		-> Lindblum/B.D. Station
	552		-> Lindblum/Main Street
	553		-> Lindblum/Inn
	554		-> Lindblum/Inn
	555		-> Lindblum/Shopping Area
	556		-> Lindblum/Church Street
	557		-> Lindblum/Church
	558		-> Lindblum/Residence
	559		-> Lindblum/Square
	560		-> Lindblum/Synthesist
	561		-> Lindblum/Item Shop
	562		-> Lindblum/Wpn. Shop
	563		-> Lindblum/T.D. Station
	564		-> Lindblum/Studio
	565		-> Lindblum/Hideout
	566		-> Lindblum/Station Area
	567		-> Lindblum/Theater Ave.
	568		-> Lindblum/Theater
	569		-> Lindblum/Square
	570		-> Lindblum/Industrial Wa
	571		-> Lindblum/The Doom Pub
	572		-> Lindblum/I.D. Station
	573		-> Lindblum/Residence
	574		-> Lindblum/Festival
	575		-> Lindblum/Festival
	576		-> Lindblum/Festival
	600		-> L. Castle/Royal Cham.
	601		-> L. Castle/Lift
	602		-> L. Castle/Dragon’s Gate
	603		-> L. Castle/Base Level
	604		-> L. Castle/Harbor
	605		-> L. Castle/Serpent’s G.
	606		-> L. Castle/Event
	607		-> L. Castle/Hangar
	608		-> L. Castle/Factory
	609		-> L. Castle/Castle Bridg
	610		-> L. Castle/Castle Stati
	611		-> L. Castle/Guest Room
	612		-> L. Castle/Hallway
	613		-> L. Castle/Hallway
	614		-> L. Castle/Airship Dock
	615		-> L. Castle/Telescope
	616		-> L. Castle/Machine Room
	617		-> L. Castle/Hallway
	618		-> L. Castle/Conf. Room
	619		-> L. Castle/Hall
	620		-> L. Castle/Lift
	650		-> Marsh/Entrance
	651		-> Marsh/Shore
	652		-> Marsh/Shore
	653		-> Marsh/Shore
	654		-> Marsh/Shore
	655		-> Marsh/Thicket
	657		-> Marsh/Pond
	660		-> Marsh/Master’s House
	661		-> Marsh/Master’s House
	701		-> Gizamaluke/Entrance
	702		-> Gizamaluke/Cavern
	703		-> Gizamaluke/Cavern
	704		-> Gizamaluke/Bell Room
	705		-> Gizamaluke/Bell Room
	706		-> Gizamaluke/Cavern
	707		-> Gizamaluke/Sacred Room
	750		-> Burmecia/Gate
	751		-> Burmecia/Suburb
	752		-> Burmecia/Suburb
	753		-> Burmecia/Suburb
	754		-> Burmecia/Residence
	755		-> Burmecia/Residence
	756		-> Burmecia/Residence
	757		-> Burmecia/Residence
	758		-> Burmecia/Pathway
	759		-> Burmecia/Uptown Area
	760		-> Burmecia/Uptown Area
	761		-> Burmecia/Uptown Area
	762		-> Burmecia/Uptown Area
	763		-> Burmecia/Square
	764		-> Burmecia/Vault
	765		-> Burmecia/Armory
	766		-> Burmecia/Palace
	767		-> Burmecia/Palace
	768		-> Burmecia/Palace
	800		-> S. Gate/Bohden Gate
	801		-> S. Gate/Bohden Gate
	802		-> S. Gate/Bohden Sta.
	806		-> S. Gate/Dali Gate
	813		-> S. Gate/Berkmea
	814		-> S. Gate/Berkmea
	850		-> S. Gate/Bohden Arch
	851		-> N. Gate/Burm. Arch
	852		-> N. Gate/Melda Arch
	1256	-> Pinnacle Rocks/Entry
	2950	-> Chocobo’s Forest
	2953	-> Chocobo’s Dream World
	152		-> Evil Forest/Memory
	656		-> Marsh/Pond
	658		-> Marsh/Pond
	659		-> Marsh/Pond
	662		-> Marsh/Thicket
	663		-> Marsh/Cave
	769		-> Burmecia/Palace
	803		-> S. Gate/Alexan. Sta.
	804		-> S. Gate/Trail
	805		-> S. Gate/Bridge
	807		-> S. Gate/Treno Gate
	809		-> S. Gate/Summit Sta.
	810		-> S. Gate/Rest Stop
	811		-> S. Gate/Railroad
	812		-> S. Gate/Summit Sta.
	815		-> S. Gate/Berkmea
	816		-> S. Gate/Berkmea
	900		-> Treno/Pub
	901		-> Treno/Bishop’s House
	902		-> Treno/Synthesist
	903		-> Treno/Card Stadium
	904		-> Treno/Knight’s House
	905		-> Treno/Walkway
	906		-> Treno/King’s House
	907		-> Treno/Queen’s House
	908		-> Treno/Gate
	909		-> Treno/Auction Site
	910		-> Treno/Knight’s House
	911		-> Treno/Queen’s House
	912		-> Treno/Slums
	913		-> Treno/Tot Residence
	914		-> Treno/Tot Residence
	915		-> Treno/Bishop’s House
	916		-> Treno/Dock
	930		-> Treno/Tot Residence
	931		-> Treno/Event
	932		-> Treno/Event
	950		-> Gargan Roo/Entrance
	951		-> Gargan Roo/Passage
	952		-> Gargan Roo/Platform
	953		-> Gargan Roo/Switch Pt.
	954		-> Gargan Roo/Tunnel
	955		-> Gargan Roo/Tunnel
	956		-> Gargan Roo/Last Stop
	957		-> Gargan Roo/Passage
	1000	-> Cleyra/Tree Roots
	1001	-> Cleyra/Tree Roots
	1002	-> Cleyra/Tree Roots
	1003	-> Cleyra/Tree Trunk
	1004	-> Cleyra/Tree Trunk
	1005	-> Cleyra/Tree Trunk
	1006	-> Cleyra/Tree Trunk
	1007	-> Cleyra/Tree Trunk
	1008	-> Cleyra/Tree Trunk
	1009	-> Cleyra/Tree Trunk
	1010	-> Cleyra/Bridge
	1011	-> Cleyra/Tree Trunk
	1012	-> Cleyra/Tree Trunk
	1013	-> Cleyra/Tree Trunk
	1014	-> Cleyra/Tree Trunk
	1015	-> Cleyra/Tree Trunk
	1016	-> Cleyra/Tree Trunk
	1017	-> Cleyra/Tree Trunk
	1018	-> Cleyra/Tree Trunk
	1050	-> Cleyra/Tree Trunk
	1051	-> Cleyra/Entrance
	1052	-> Cleyra/Sandpit
	1053	-> Cleyra/Water Mill Area
	1054	-> Cleyra/Windmill Area
	1055	-> Cleyra/Town Area
	1056	-> Cleyra/Inn
	1057	-> Cleyra/Observation Poi
	1058	-> Cleyra/Cathedral
	1059	-> Cleyra/Cathedral
	1060	-> Cleyra/Cathedral
	1100	-> Cleyra/Tree Trunk
	1101	-> Cleyra/Entrance
	1102	-> Cleyra/Sandpit
	1103	-> Cleyra/Water Mill Area
	1104	-> Cleyra/Windmill Area
	1105	-> Cleyra/Town
	1106	-> Cleyra/Inn
	1107	-> Cleyra/Observation Poi
	1108	-> Cleyra/Cathedral
	1109	-> Cleyra/Cathedral
	1110	-> Cleyra/Cathedral
	1150	-> Red Rose/Deck
	1151	-> Red Rose/Cabin
	1152	-> Red Rose/Cabin
	1153	-> Red Rose/Bridge
	1200	-> A. Castle/Throne
	1201	-> A. Castle/Underground
	1202	-> A. Castle/Staircase
	1203	-> A. Castle/Staircase
	1204	-> A. Castle/Underground
	1205	-> A. Castle/Chapel
	1206	-> A. Castle/Queen’s Cham
	1207	-> A. Castle/Garnet’s Roo
	1208	-> A. Castle/Dungeon
	1209	-> A. Castle/Dungeon
	1210	-> A. Castle/West Tower
	1211	-> A. Castle/East Tower
	1212	-> A. Castle/East Tower
	1213	-> A. Castle/Guardhouse
	1214	-> A. Castle/Hallway
	1215	-> A. Castle/Hallway
	1216	-> A. Castle/Library
	1217	-> A. Castle/Guest Room
	1218	-> A. Castle/Kitchen
	1219	-> A. Castle/Courtyard
	1220	-> A. Castle/Courtyard
	1221	-> A. Castle/Courtyard
	1222	-> A. Castle/Courtyard
	1223	-> A. Castle/Queen’s Cham
	1224	-> A. Castle/Interior
	1225	-> A. Castle/Queen’s Cham
	1226	-> A. Castle/Library
	1227	-> A. Castle/Public Seats
	1250	-> A. Castle/Event
	1251	-> Pinnacle Rocks/Hole
	1252	-> Pinnacle Rocks/Path
	1253	-> Pinnacle Rocks/Path
	1254	-> Pinnacle Rocks/Path
	1255	-> Pinnacle Rocks/Entry
	1300	-> Lindblum/Hunter’s Gate
	1301	-> Lindblum/B.D. Station
	1302	-> Lindblum/Main Street
	1303	-> Lindblum/Inn
	1304	-> Lindblum/Inn
	1305	-> Lindblum/Shopping Area
	1306	-> Lindblum/Residence
	1307	-> Lindblum/Square
	1308	-> Lindblum/Synthesist
	1309	-> Lindblum/Wpn. Shop
	1310	-> Lindblum/T.D. Station
	1311	-> Lindblum/Studio
	1312	-> Lindblum/Hideout
	1313	-> Lindblum/Station Area
	1314	-> Lindblum/Theater Ave.
	1315	-> Lindblum/Town Walls
	1350	-> L. Castle/Royal Cham.
	1351	-> L. Castle/Lift
	1352	-> L. Castle/Dragon’s Gat
	1353	-> L. Castle/Base Level
	1354	-> L. Castle/Harbor
	1355	-> L. Castle/Serpent’s Ga
	1356	-> L. Castle/Event
	1357	-> L. Castle/Hangar
	1358	-> L. Castle/Factory
	1359	-> L. Castle/Castle Bridg
	1360	-> L. Castle/Castle Stati
	1361	-> L. Castle/Guest Room
	1362	-> L. Castle/Hallway
	1363	-> L. Castle/Hallway
	1364	-> L. Castle/Airship Dock
	1365	-> L. Castle/Telescope
	1366	-> L. Castle/Machine Room
	1367	-> L. Castle/Hallway
	1368	-> L. Castle/Conf. Room
	1369	-> L. Castle/Hall
	1370	-> L. Castle/Lift
	1400	-> Fossil Roo/Cavern
	1401	-> Fossil Roo/Cavern
	1402	-> Fossil Roo/Cavern
	1403	-> Fossil Roo/Cavern
	1404	-> Fossil Roo/Cavern
	1405	-> Fossil Roo/Nest
	1406	-> Fossil Roo/Nest
	1407	-> Fossil Roo/Nest
	1408	-> Fossil Roo/Nest
	1409	-> Fossil Roo/Nest
	1410	-> Fossil Roo/Nest
	1411	-> Fossil Roo/Nest
	1412	-> Fossil Roo/Nest
	1413	-> Fossil Roo/Nest
	1414	-> Fossil Roo/Nest
	1415	-> Fossil Roo/Nest
	1416	-> Fossil Roo/Nest
	1417	-> Fossil Roo/Nest
	1418	-> Fossil Roo/Cavern
	1419	-> Fossil Roo/Cavern
	1420	-> Fossil Roo/Cavern
	1421	-> Fossil Roo/Mining Site
	1422	-> Fossil Roo/Entrance
	1423	-> Fossil Roo/Passage
	1424	-> Fossil Roo/Passage
	1425	-> Fossil Roo/Exit
	1450	-> Mage Village/Entrance
	1451	-> Mage Village/Pond
	1452	-> Mage Village/Cemetery
	1453	-> Mage Village/Cemetery
	1454	-> Mage Village/Inn
	1455	-> Mage Village/Synthesis
	1456	-> Mage Village/Wpn. Shop
	1457	-> Mage Village/Rooftop
	1458	-> Mage Village/Water Mil
	1459	-> Mage Village/Water Mil
	1460	-> Mage Village/Item Shop
	1461	-> Mage Village/Event
	1462	-> Mage Village/Event
	1463	-> Dead Forest/Grove
	1464	-> Dead Forest/Dead End
	1500	-> Conde Petie/Entrance
	1501	-> Conde Petie/Corridor
	1502	-> Conde Petie/Corridor
	1503	-> Conde Petie/Wpn. Stall
	1504	-> Conde Petie/Exit
	1505	-> Conde Petie/Shrine
	1506	-> Conde Petie/Event
	1507	-> Conde Petie/Pathway
	1508	-> Conde Petie/Inn
	1509	-> Conde Petie/Item Shop
	1550	-> Mountain Path/Trail
	1551	-> Mountain Path/Trail
	1552	-> Mountain Path/Trail
	1553	-> Mountain Path/Roots
	1554	-> Mountain Path/Roots
	1555	-> Mountain Path/Roots
	1556	-> Mountain Path/Roots
	1557	-> Mountain Path/Roots
	1600	-> Mdn. Sari/Entrance
	1601	-> Mdn. Sari/Open Area
	1602	-> Mdn. Sari/Path
	1603	-> Mdn. Sari/Path
	1604	-> Mdn. Sari/Eidolon Wall
	1605	-> Mdn. Sari/Eidolon Wall
	1606	-> Mdn. Sari/Resting Room
	1607	-> Mdn. Sari/Kitchen
	1608	-> Mdn. Sari/Secret Room
	1609	-> Mdn. Sari/Cove
	1610	-> Mdn. Sari/Cove
	1650	-> Iifa Tree/Outer Seal
	1651	-> Iifa Tree/Tree Roots
	1652	-> Iifa Tree/Roots
	1653	-> Iifa Tree/Tree Roots
	1654	-> Iifa Tree/Tree Trunk
	1655	-> Iifa Tree/Tree Path
	1656	-> Iifa Tree/Eidolon Moun
	1657	-> Iifa Tree/Tree Roots
	1658	-> Iifa Tree/Silver Drago
	1659	-> Iifa Tree/Seashore
	1660	-> Brahne’s Fleet/Event
	1661	-> Brahne’s Fleet/Event
	1662	-> Brahne’s Fleet/Event
	1663	-> Iifa Tree/Tree Trunk
	1750	-> Iifa Tree/Hollow Roots
	1751	-> Iifa Tree/Inner Roots
	1752	-> Iifa Tree/Inner Roots
	1753	-> Iifa Tree/Inner Roots
	1754	-> Iifa Tree/Leaves
	1755	-> Iifa Tree/Bottom
	1756	-> Iifa Tree/Bottom
	1757	-> Iifa Tree/Outer Seal
	1758	-> Iifa Tree/Tree Roots
	1759	-> Iifa Tree/Roots
	1800	-> A. Castle/Tomb
	1950	-> Quan’s/Cave
	1951	-> Quan’s/Cave
	1952	-> Quan’s/Cave
	1953	-> Quan’s/Fishing Area
	808		-> S. Gate/Bridge
	853		-> S. Gate/Treno Arch
	854		-> S. Gate/Bohden Arch
	855		-> N. Gate/Burm. Arch
	856		-> N. Gate/Melda Arch
	1801	-> A. Castle/Queen’s Cham
	1802	-> A. Castle/Garnet’s Roo
	1803	-> A. Castle/Guardhouse
	1806	-> A. Castle/Hallway
	1807	-> A. Castle/Hallway
	1808	-> A. Castle/Library
	1809	-> A. Castle/Guest Room
	1810	-> A. Castle/Kitchen
	1811	-> A. Castle/Courtyard
	1812	-> A. Castle/Courtyard
	1813	-> A. Castle/Courtyard
	1814	-> A. Castle/Courtyard
	1815	-> A. Castle/Courtyard
	1816	-> A. Castle/Courtyard
	1817	-> A. Castle/Neptune
	1818	-> A. Castle/Neptune
	1819	-> A. Castle/Port
	1820	-> A. Castle/West Tower
	1821	-> A. Castle/Interior
	1822	-> A. Castle/Library
	1823	-> A. Castle/Hallway
	1824	-> A. Castle/Public Seats
	1850	-> Alexandria/Main Street
	1851	-> Alexandria/Main Street
	1852	-> Alexandria/Main Street
	1853	-> Alexandria/Square
	1854	-> Alexandria/Alley
	1855	-> Alexandria/By the Stee
	1856	-> Alexandria/Main Street
	1857	-> Alexandria/Item Shop
	1858	-> Alexandria/Wpn. Shop
	1859	-> Alexandria/Synthesist
	1860	-> Alexandria/Inn
	1861	-> Alexandria/Pub
	1862	-> Alexandria/Residence
	1863	-> Alexandria/Residence
	1864	-> Alexandria/Mini-Theate
	1865	-> Alexandria/Steeple
	1866	-> Alexandria/Dock
	1900	-> Treno/Pub
	1901	-> Treno/Bishop’s House
	1902	-> Treno/Synthesist
	1903	-> Treno/Card Stadium
	1904	-> Treno/Knight’s House
	1905	-> Treno/Walkway
	1906	-> Treno/King’s House
	1907	-> Treno/Queen’s House
	1908	-> Treno/Gate
	1909	-> Treno/Auction House
	1910	-> Treno/Knight’s House
	1911	-> Treno/Queen’s House
	1912	-> Treno/Treno Slums
	1913	-> Treno/Tot Residence
	1914	-> Treno/Tot Residence
	1915	-> Treno/Bishop’s House
	1916	-> Treno/Knight’s House
	2000	-> Hilda Garde 2/Deck
	2001	-> A. Castle/Garnet’s Roo
	2002	-> A. Castle/Altar
	2003	-> A. Castle/Altar
	2004	-> A. Castle/Altar
	2005	-> A. Castle/Altar
	2006	-> A. Castle/Altar
	2007	-> A. Castle/Altar
	2008	-> A. Castle/Altar
	2009	-> A. Castle/Throne
	2050	-> Alexandria/Main Street
	2051	-> Alexandria/Main Street
	2052	-> Alexandria/Main Street
	2053	-> Alexandria/Square
	2054	-> Alexandria/Main Street
	2055	-> Invincible/Garland
	2100	-> Lindblum/Hunter’s Gate
	2101	-> Lindblum/B.D. Station
	2102	-> Lindblum/Main Street
	2103	-> Lindblum/Inn
	2104	-> Lindblum/Inn
	2105	-> Lindblum/Shopping Area
	2106	-> Lindblum/Residence
	2107	-> Lindblum/Square
	2108	-> Lindblum/Synthesist
	2109	-> Lindblum/Wpn. Shop
	2110	-> Lindblum/T.D. Station
	2111	-> Lindblum/Studio
	2112	-> Lindblum/Hideout
	2113	-> Lindblum/Station Area
	2114	-> Lindblum/Theater Ave.
	2150	-> L. Castle/Royal Cham.
	2151	-> L. Castle/Lift
	2152	-> L. Castle/Dragon’s Gat
	2153	-> L. Castle/Base Level
	2155	-> L. Castle/Serpent’s Ga
	2157	-> L. Castle/Hangar
	2158	-> L. Castle/Factory
	2159	-> L. Castle/Castle Bridg
	2160	-> L. Castle/Castle Stati
	2161	-> L. Castle/Guest Room
	2162	-> L. Castle/Hallway
	2163	-> L. Castle/Hallway
	2164	-> L. Castle/Airship Dock
	2167	-> L. Castle/Machine Room
	2168	-> L. Castle/Hallway
	2169	-> L. Castle/Conf. Room
	2170	-> L. Castle/Hall
	2171	-> L. Castle/Lift
	2172	-> L. Castle/Telescope
	2173	-> L. Castle/Harbor
	2200	-> Palace/Dungeon
	2201	-> Palace/Dungeon
	2202	-> Palace/Dungeon
	2203	-> Palace/Rack
	2204	-> Palace/Odyssey
	2205	-> Palace/Odyssey
	2206	-> Palace/Hallway
	2207	-> Palace/Hall
	2208	-> Palace/Hallway
	2209	-> Palace/Sanctum
	2211	-> Palace/Dock
	2212	-> Palace/Entrance
	2213	-> Palace/Lobby
	2214	-> Palace/Light Chamber
	2215	-> Palace/Fire Chamber
	2216	-> Palace/Hallway
	2217	-> Palace/Stairwell
	2220	-> Palace/Library
	2221	-> Palace/Shadow Cham.
	2222	-> Palace/Top Level
	2250	-> Oeilvert/Outside
	2251	-> Oeilvert/Entrance
	2252	-> Oeilvert/Hall
	2253	-> Oeilvert/Planetarium
	2254	-> Oeilvert/Ship Display
	2255	-> Oeilvert/Stairwell
	2256	-> Oeilvert/Grand Hall
	2257	-> Oeilvert/Display
	2258	-> Oeilvert/Narration
	2259	-> Oeilvert/Star Display
	2260	-> Oeilvert/Tombstone
	2261	-> Oeilvert/Bridge
	2300	-> Esto Gaza/Terrace
	2301	-> Esto Gaza/Altar
	2302	-> Esto Gaza/Altar
	2303	-> Esto Gaza/Shop
	2304	-> Esto Gaza/Terrace
	2305	-> Esto Gaza/Path
	2350	-> Gulug/Interior
	2351	-> Gulug/Well
	2352	-> Gulug/Room
	2353	-> Gulug/Room
	2354	-> Gulug/Room
	2355	-> Gulug/Room
	2356	-> Gulug/Room
	2357	-> Gulug/Room
	2358	-> Gulug/Path
	2359	-> Gulug/Path
	2360	-> Gulug/Path
	2361	-> Gulug/Well
	2362	-> Gulug/Path
	2363	-> Gulug/Path
	2364	-> Gulug/Extraction Site
	2365	-> Gulug/Extraction Site
	2400	-> A. Castle/Neptune
	2401	-> A. Castle/Neptune
	2402	-> A. Castle/Port
	2403	-> A. Castle/Port
	2404	-> A. Castle/Courtyard
	2405	-> A. Castle/Courtyard
	2406	-> A. Castle/Courtyard
	2450	-> Alexandria/Main Street
	2451	-> Alexandria/Main Street
	2452	-> Alexandria/Square
	2453	-> Alexandria/Alley
	2454	-> Alexandria/Steeple
	2455	-> Alexandria/Inn
	2456	-> Alexandria/Steeple
	2457	-> Alexandria/Mini-Theate
	2458	-> Alexandria/Dock
	2500	-> I. Castle/Entrance
	2502	-> I. Castle/Hall
	2503	-> I. Castle/Inverted Hal
	2504	-> I. Castle/Small Room
	2505	-> I. Castle/Inverted Roo
	2506	-> I. Castle/Stairwell
	2507	-> I. Castle/Stairwell
	2508	-> I. Castle/Lift
	2509	-> I. Castle/Lift
	2510	-> I. Castle/Mural Room
	2512	-> I. Castle/Mural Room
	2513	-> I. Castle/Sword Room
	2550	-> Earth Shrine/Entrance
	2551	-> Water Shrine/Entrance
	2552	-> Earth Shrine/Interior
	2553	-> Wind Shrine/Interior
	2554	-> Earth Shrine/Passage
	2600	-> Terra/Hilltop
	2601	-> Terra/Downhill Path
	2602	-> Terra/Stepping Stones
	2603	-> Terra/Path by the Pond
	2604	-> Terra/Bridge
	2605	-> Terra/Treetop
	2606	-> Terra/Tree base
	2607	-> Terra/Bridge
	2608	-> Terra/Event
	2650	-> Bran Bal/Stairway
	2651	-> Bran Bal/Entrance
	2652	-> Bran Bal/Gate
	2653	-> Bran Bal/Pond
	2654	-> Bran Bal/Pond
	2655	-> Bran Bal/Storage
	2656	-> Bran Bal/Meeting Hall
	2657	-> Bran Bal/Storage
	2658	-> Bran Bal/Laboratory
	2659	-> Bran Bal/Bridge
	2660	-> Bran Bal/Hilltop
	2661	-> Bran Bal/Entrance
	2700	-> Pand./Gate
	2701	-> Pand./Path
	2702	-> Pand./Observatory
	2703	-> Pand./Observatory
	2704	-> Pand./Observatory
	2705	-> Pand./Mind Control
	2706	-> Pand./Hall
	2707	-> Pand./Hall
	2708	-> Pand./Hall
	2709	-> Pand./Generator
	2710	-> Pand./Bridge
	2711	-> Pand./Control Room
	2712	-> Pand./Elevator
	2713	-> Pand./Elevator
	2714	-> Pand./Maze
	2715	-> Pand./Event
	2716	-> Pand./Event
	2717	-> Pand./Outer Path
	2718	-> Pand./Bridge
	2719	-> Pand./Exit
	2720	-> Pand./Event
	2750	-> Invincible/Bridge
	2753	-> Invincible/Core
	2800	-> Daguerreo/Entrance
	2801	-> Daguerreo/Right Hall
	2802	-> Daguerreo/Left Hall
	2803	-> Daguerreo/2nd Floor
	2850	-> Hilda Garde 3/Bridge
	2851	-> Hilda Garde 3/Engine
	2852	-> Hilda Garde 3/Deck
	2853	-> Hilda Garde 3/Deck
	2854	-> Hilda Garde 3/Bridge
	2855	-> Blue Narciss/Bridge
	2856	-> Blue Narciss/Bridge
	2951	-> Chocobo’s Lagoon
	2952	-> Chocobo’s Air Garden
	2954	-> Chocobo’s Paradise
	2955	-> Chocobo’s Paradise
	3100	-> Mognet Central
	1700	-> Mdn. Sari/Entrance
	1701	-> Mdn. Sari/Open Area
	1702	-> Mdn. Sari/Path
	1703	-> Mdn. Sari/Path
	1704	-> Mdn. Sari/Eidolon Wall
	1705	-> Mdn. Sari/Resting Room
	1706	-> Mdn. Sari/Kitchen
	1707	-> Mdn. Sari/Secret Room
	1917	-> Treno/Knight’s House
	2154	-> L. Castle/Harbor
	2165	-> L. Castle/Airship Dock
	2166	-> L. Castle/Telescope
	2501	-> I. Castle/Entrance
	2751	-> Invincible/Bridge
	2752	-> Invincible/Bridge
	2754	-> Invincible/Controls
	2755	-> Hilda Garde/Bridge
	2756	-> Red Rose/Bridge
	2900	-> Memoria/Outside
	2901	-> Memoria/Entrance
	2902	-> Memoria/Stairs of Time
	2903	-> Memoria/Recollection
	2904	-> Memoria/Outer Path
	2905	-> Memoria/The Past
	2906	-> Memoria/Oblivion
	2907	-> Memoria/Recollection
	2908	-> Memoria/Time Interval
	2909	-> Memoria/Ruins
	2910	-> Memoria/Lost Memory
	2911	-> Memoria/Familiar Past
	2912	-> Memoria/World Fusion
	2913	-> Memoria/Portal
	2914	-> Memoria/Birth
	2915	-> Memoria/Ocean
	2916	-> Memoria/Time Warp
	2917	-> Memoria/Gaia’s Birth
	2918	-> Memoria/Stairs
	2919	-> Memoria/Gate to Space
	2920	-> Memoria/Emptiness
	2921	-> Memoria/To the Origin
	2922	-> Crystal World
	2923	-> Crystal World
	2924	-> Crystal World
	2925	-> Crystal World
	2926	-> Crystal World
	2927	-> Crystal World
	2928	-> Hill of Despair
	2929	-> last/cw mbg a
	2930	-> last/cw mbg 0
	2931	-> last/g3 brg 0
	2932	-> last/rr brg 0
	2933	-> last/cw mbg 1
	2934	-> last/cw mbg 2
	3000	-> Ending/AT
	3001	-> Ending/AC
	3002	-> Ending/AC
	3003	-> Ending/AC
	3004	-> Ending/BU
	3005	-> Ending/KM
	3006	-> Ending/LB
	3007	-> Ending/TR
	3008	-> Ending/TH
	3009	-> Ending/TH
	3010	-> Ending/TH
	3011	-> Ending/TH
	3012	-> Ending/AC
	3050	-> Mage Village/Entrance
	3051	-> Mage Village/Pond
	3052	-> Mage Village/Cemetery
	3053	-> Mage Village/Inn
	3054	-> Mage Village/Synthesis
	3055	-> Mage Village/Wpn. Shop
	3056	-> Mage Village/Rooftop
	3057	-> Mage Village/Water Mil
	3058	-> Mage Village/Water Mil
	3059	-> Mage Village/Item Shop
	*/
	public FF9StateGlobal()
	{
		this.ot = new UInt32[2];
		this.player = new PLAYER[9];
		for (Int32 i = 0; i < (Int32)this.player.Length; i++)
		{
			this.player[i] = new PLAYER();
		}
		this.party = new PARTY_DATA();
		this.item = new FF9ITEM[256];
		this.rare_item = new Byte[64];
		this.charArray = new Dictionary<Int32, FF9Char>();
	    this.Frogs = new FrogHandler();
		this.steal_no = 0;
		this.dragon_no = 0;
	}

	public FF9Char FF9GetCharPtr(Int32 uid)
	{
		return this.charArray[uid];
	}

	public void ff9ResetStateGlobal()
	{
		this.hintmap_id = 0;
	}

	public const Int32 FF9_SIZE_OT = 4096;

	public const Int32 FF9_BUFFER_COUNT = 2;

	public UInt32 attr;

	public Char usage;

	public Byte id;

	public Byte mainUsed;

	public UInt16 proj;

	public Int16 fldMapNo;

	public Int16 btlMapNo;

	public UInt32[] ot;

	public Matrix4x4 cam;

	public Byte npcCount;

	public Byte npcUsed;

	public Int16 fldLocNo;

	public PLAYER[] player;

	public PARTY_DATA party;

	public FrogHandler Frogs;

	public Int16 steal_no;

	public Int16 dragon_no;

	public Byte btl_result; // FF9.battle_result

	public Byte btl_flag;

	public Byte btl_rain;

	public Byte steiner_state;

	public FF9ITEM[] item;

	public Byte[] rare_item;

	public SByte btlSubMapNo;

	public Int16 wldMapNo;

	public Int16 wldLocNo;

	public UInt16 miniGameArg;

	public Int32 hintmap_id;

	public String mapNameStr;

	public ff9.sworldState worldState = new ff9.sworldState();

	public Boolean timerControl;

	public Boolean timerDisplay;

	public Vector2 projectionOffset;

	public CharInitFuncPtr charInitFuncPtr;

	public Dictionary<Int32, FF9Char> charArray;
}
