using System;

namespace FF9
{
	public class ff9weap
	{
		public const Int32 FF9WEAPON_START = 0;

		public static WEAPON[] _FF9Weapon_Data = new WEAPON[]
		{
			new WEAPON(1, 0, "GEO_WEP_B1_011", new BTL_REF(5, 12, 0, 0), new Int16[2]),
			new WEAPON(5, 0, "GEO_WEP_B1_012", new BTL_REF(1, 12, 0, 0), new Int16[]
			{
				17,
				200
			}),
			new WEAPON(5, 9, "GEO_WEP_B1_013", new BTL_REF(1, 14, 0, 20), new Int16[]
			{
				81,
				166
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_014", new BTL_REF(1, 18, 0, 0), new Int16[]
			{
				17,
				200
			}),
			new WEAPON(5, 18, "GEO_WEP_B1_015", new BTL_REF(1, 30, 0, 35), new Int16[]
			{
				-32,
				200
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_016", new BTL_REF(1, 42, 0, 0), new Int16[]
			{
				5,
				201
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_017", new BTL_REF(1, 71, 0, 0), new Int16[]
			{
				0,
				199
			}),
			new WEAPON(5, 9, "GEO_WEP_B1_018", new BTL_REF(2, 21, 0, 0), new Int16[]
			{
				0,
				385
			}),
			new WEAPON(5, 14, "GEO_WEP_B1_019", new BTL_REF(2, 24, 0, 0), new Int16[]
			{
				29,
				385
			}),
			new WEAPON(5, 31, "GEO_WEP_B1_020", new BTL_REF(2, 31, 0, 0), new Int16[]
			{
				0,
				376
			}),
			new WEAPON(5, 20, "GEO_WEP_B1_021", new BTL_REF(2, 37, 0, 0), new Int16[]
			{
				0,
				380
			}),
			new WEAPON(5, 12, "GEO_WEP_B1_022", new BTL_REF(2, 44, 0, 0), new Int16[]
			{
				0,
				381
			}),
			new WEAPON(13, 22, "GEO_WEP_B1_023", new BTL_REF(2, 53, 0, 0), new Int16[]
			{
				-5,
				388
			}),
			new WEAPON(5, 25, "GEO_WEP_B1_024", new BTL_REF(2, 62, 0, 0), new Int16[]
			{
				-17,
				383
			}),
			new WEAPON(5, 10, "GEO_WEP_B1_025", new BTL_REF(2, 86, 0, 0), new Int16[]
			{
				0,
				352
			}),
			new WEAPON(5, 17, "GEO_WEP_B1_026", new BTL_REF(2, 100, 0, 0), new Int16[]
			{
				0,
				376
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_027", new BTL_REF(1, 12, 0, 0), new Int16[]
			{
				0,
				324
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_028", new BTL_REF(1, 16, 0, 0), new Int16[]
			{
				0,
				324
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_029", new BTL_REF(1, 20, 0, 0), new Int16[]
			{
				0,
				324
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_030", new BTL_REF(6, 24, 0, 0), new Int16[]
			{
				39,
				311
			}),
			new WEAPON(5, 28, "GEO_WEP_B1_031", new BTL_REF(1, 35, 2, 10), new Int16[]
			{
				0,
				315
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_032", new BTL_REF(1, 38, 4, 0), new Int16[]
			{
				0,
				324
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_033", new BTL_REF(1, 42, 0, 0), new Int16[]
			{
				0,
				324
			}),
			new WEAPON(5, 29, "GEO_WEP_B1_034", new BTL_REF(1, 46, 1, 10), new Int16[]
			{
				39,
				311
			}),
			new WEAPON(5, 14, "GEO_WEP_B1_035", new BTL_REF(1, 57, 0, 25), new Int16[]
			{
				0,
				327
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_036", new BTL_REF(1, 65, 0, 0), new Int16[]
			{
				82,
				274
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_037", new BTL_REF(7, 23, 0, 0), new Int16[]
			{
				0,
				352
			}),
			new WEAPON(5, 17, "GEO_WEP_B1_038", new BTL_REF(2, 74, 0, 20), new Int16[]
			{
				0,
				430
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_039", new BTL_REF(2, 77, 64, 0), new Int16[]
			{
				0,
				413
			}),
			new WEAPON(5, 18, "GEO_WEP_B1_040", new BTL_REF(2, 87, 0, 30), new Int16[]
			{
				0,
				445
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_041", new BTL_REF(2, 108, 64, 0), new Int16[]
			{
				0,
				413
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_042", new BTL_REF(1, 18, 0, 0), new Int16[]
			{
				0,
				524
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_043", new BTL_REF(1, 20, 0, 0), new Int16[]
			{
				0,
				524
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_044", new BTL_REF(1, 25, 0, 0), new Int16[]
			{
				-3,
				483
			}),
			new WEAPON(5, 28, "GEO_WEP_B1_045", new BTL_REF(1, 31, 2, 10), new Int16[]
			{
				0,
				524
			}),
			new WEAPON(5, 14, "GEO_WEP_B1_046", new BTL_REF(1, 37, 0, 20), new Int16[]
			{
				0,
				398
			}),
			new WEAPON(5, 19, "GEO_WEP_B1_047", new BTL_REF(1, 42, 0, 10), new Int16[]
			{
				0,
				539
			}),
			new WEAPON(5, 22, "GEO_WEP_B1_048", new BTL_REF(1, 52, 0, 10), new Int16[]
			{
				0,
				521
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_049", new BTL_REF(1, 62, 64, 0), new Int16[]
			{
				0,
				519
			}),
			new WEAPON(5, 12, "GEO_WEP_B1_050", new BTL_REF(1, 71, 0, 15), new Int16[]
			{
				0,
				647
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_051", new BTL_REF(1, 77, 0, 0), new Int16[]
			{
				-1,
				648
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_052", new BTL_REF(1, 23, 0, 0), new Int16[]
			{
				-39,
				257
			}),
			new WEAPON(5, 20, "GEO_WEP_B1_053", new BTL_REF(1, 33, 0, 40), new Int16[]
			{
				-39,
				257
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_054", new BTL_REF(1, 39, 0, 0), new Int16[]
			{
				-39,
				257
			}),
			new WEAPON(5, 37, "GEO_WEP_B1_055", new BTL_REF(1, 45, 0, 15), new Int16[]
			{
				-93,
				254
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_056", new BTL_REF(1, 53, 16, 0), new Int16[]
			{
				-64,
				203
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_057", new BTL_REF(1, 62, 0, 0), new Int16[]
			{
				-39,
				257
			}),
			new WEAPON(5, 21, "GEO_WEP_B1_058", new BTL_REF(1, 70, 0, 15), new Int16[]
			{
				-78,
				237
			}),
			new WEAPON(5, 31, "GEO_WEP_B1_059", new BTL_REF(1, 75, 32, 15), new Int16[]
			{
				32,
				158
			}),
			new WEAPON(5, 13, "GEO_WEP_B1_060", new BTL_REF(1, 79, 0, 25), new Int16[]
			{
				-54,
				190
			}),
			new WEAPON(5, 14, "GEO_WEP_B1_061", new BTL_REF(1, 83, 0, 30), new Int16[]
			{
				-3,
				260
			}),
			new WEAPON(6, 0, "GEO_WEP_B1_062", new BTL_REF(3, 13, 32, 0), new Int16[]
			{
				79,
				262
			}),
			new WEAPON(6, 0, "GEO_WEP_B1_063", new BTL_REF(3, 17, 32, 0), new Int16[]
			{
				79,
				262
			}),
			new WEAPON(6, 0, "GEO_WEP_B1_064", new BTL_REF(3, 23, 32, 0), new Int16[]
			{
				98,
				246
			}),
			new WEAPON(6, 0, "GEO_WEP_B1_065", new BTL_REF(3, 27, 32, 0), new Int16[]
			{
				127,
				245
			}),
			new WEAPON(6, 0, "GEO_WEP_B1_066", new BTL_REF(3, 35, 32, 0), new Int16[]
			{
				86,
				254
			}),
			new WEAPON(6, 0, "GEO_WEP_B1_067", new BTL_REF(3, 45, 32, 0), new Int16[]
			{
				53,
				305
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_068", new BTL_REF(1, 11, 0, 0), new Int16[]
			{
				-1,
				177
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_069", new BTL_REF(1, 14, 0, 0), new Int16[]
			{
				-1,
				177
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_070", new BTL_REF(1, 16, 0, 0), new Int16[]
			{
				-1,
				177
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_071", new BTL_REF(1, 23, 0, 0), new Int16[]
			{
				-1,
				177
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_072", new BTL_REF(1, 27, 64, 0), new Int16[]
			{
				0,
				193
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_073", new BTL_REF(1, 31, 0, 0), new Int16[]
			{
				0,
				172
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_074", new BTL_REF(1, 36, 0, 0), new Int16[]
			{
				0,
				222
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_075", new BTL_REF(1, 17, 0, 0), new Int16[]
			{
				0,
				118
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_076", new BTL_REF(1, 21, 0, 0), new Int16[]
			{
				0,
				118
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_077", new BTL_REF(1, 24, 0, 0), new Int16[]
			{
				15,
				194
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_078", new BTL_REF(1, 27, 0, 0), new Int16[]
			{
				-50,
				157
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_079", new BTL_REF(1, 30, 0, 0), new Int16[]
			{
				0,
				118
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_080", new BTL_REF(1, 33, 0, 0), new Int16[]
			{
				61,
				170
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_081", new BTL_REF(1, 12, 0, 0), new Int16[]
			{
				-12,
				175
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_082", new BTL_REF(1, 16, 1, 0), new Int16[]
			{
				-12,
				175
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_083", new BTL_REF(1, 16, 2, 0), new Int16[]
			{
				-12,
				175
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_084", new BTL_REF(1, 16, 4, 0), new Int16[]
			{
				-12,
				175
			}),
			new WEAPON(5, 18, "GEO_WEP_B1_085", new BTL_REF(1, 23, 0, 20), new Int16[]
			{
				-24,
				171
			}),
			new WEAPON(5, 12, "GEO_WEP_B1_086", new BTL_REF(1, 27, 0, 20), new Int16[]
			{
				0,
				153
			}),
			new WEAPON(5, 31, "GEO_WEP_B1_087", new BTL_REF(1, 29, 0, 10), new Int16[]
			{
				0,
				184
			}),
			new WEAPON(5, 9, "GEO_WEP_B1_088", new BTL_REF(1, 32, 0, 15), new Int16[]
			{
				-1,
				200
			}),
			new WEAPON(5, 10, "GEO_WEP_B1_089", new BTL_REF(1, 35, 0, 15), new Int16[]
			{
				0,
				166
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_090", new BTL_REF(5, 21, 0, 0), new Int16[]
			{
				0,
				226
			}),
			new WEAPON(5, 22, "GEO_WEP_B1_091", new BTL_REF(5, 34, 0, 10), new Int16[]
			{
				0,
				226
			}),
			new WEAPON(5, 0, "GEO_WEP_B1_092", new BTL_REF(5, 42, 0, 0), new Int16[]
			{
				0,
				226
			}),
			new WEAPON(5, 18, "GEO_WEP_B1_093", new BTL_REF(5, 53, 0, 15), new Int16[]
			{
				0,
				226
			}),
			new WEAPON(13, 17, "GEO_WEP_B1_094", new BTL_REF(5, 68, 0, 20), new Int16[]
			{
				-20,
				227
			}),
			new WEAPON(13, 19, "GEO_WEP_B1_095", new BTL_REF(5, 77, 0, 10), new Int16[]
			{
				-5,
				239
			}),
			new WEAPON(6, 0, (String)null, new BTL_REF(0, 25, 0, 0), new Int16[2]),
			new WEAPON(6, 0, (String)null, new BTL_REF(0, 36, 0, 0), new Int16[2]),
			new WEAPON(6, 0, (String)null, new BTL_REF(0, 64, 0, 0), new Int16[2])
		};
	}
}
