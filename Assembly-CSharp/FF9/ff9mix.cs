using System;

namespace FF9
{
	public class ff9mix
	{
		public static void FF9Mix_Buy(Byte synthesis_id)
		{
			FF9MIX_DATA ff9MIX_DATA = ff9mix._FF9Mix.item[(Int32)synthesis_id];
			Int32 num;
			if ((num = ff9item.FF9Item_Add((Int32)ff9MIX_DATA.dst, (Int32)ff9mix._FF9Mix.mix_ct)) > 0)
			{
				ff9item.FF9Item_Remove((Int32)ff9MIX_DATA.src[0], num);
				ff9item.FF9Item_Remove((Int32)ff9MIX_DATA.src[1], num);
				FF9StateSystem.Common.FF9.party.gil -= UInt32.Parse(((Int32)ff9MIX_DATA.price * num).ToString());
			}
		}

		public static FF9MIX_INFO _FF9Mix;

		public static Int32 FF9MIX_SRC_MAX = 2;

		public static FF9MIX_DATA[] _FF9MIX_DATA = new FF9MIX_DATA[]
		{
			new FF9MIX_DATA(300, new Byte[]
			{
				1,
				2
			}, 7, 79),
			new FF9MIX_DATA(700, new Byte[]
			{
				2,
				2
			}, 8, 95),
			new FF9MIX_DATA(1000, new Byte[]
			{
				2,
				3
			}, 9, 92),
			new FF9MIX_DATA(2000, new Byte[]
			{
				3,
				3
			}, 10, 88),
			new FF9MIX_DATA(9000, new Byte[]
			{
				3,
				4
			}, 11, 112),
			new FF9MIX_DATA(12000, new Byte[]
			{
				4,
				5
			}, 12, 96),
			new FF9MIX_DATA(16000, new Byte[]
			{
				5,
				6
			}, 13, 64),
			new FF9MIX_DATA(16000, new Byte[]
			{
				45,
				46
			}, 49, 64),
			new FF9MIX_DATA(11000, new Byte[]
			{
				51,
				218
			}, 55, 64),
			new FF9MIX_DATA(24000, new Byte[]
			{
				197,
				107
			}, 101, 64),
			new FF9MIX_DATA(8000, new Byte[]
			{
				104,
				99
			}, 111, 64),
			new FF9MIX_DATA(15000, new Byte[]
			{
				141,
				128
			}, 134, 64),
			new FF9MIX_DATA(20000, new Byte[]
			{
				129,
				204
			}, 135, 64),
			new FF9MIX_DATA(20000, new Byte[]
			{
				142,
				200
			}, 147, 64),
			new FF9MIX_DATA(20000, new Byte[]
			{
				163,
				95
			}, 166, 64),
			new FF9MIX_DATA(26000, new Byte[]
			{
				154,
				58
			}, 167, 64),
			new FF9MIX_DATA(1000, new Byte[]
			{
				88,
				115
			}, 168, 63),
			new FF9MIX_DATA(2000, new Byte[]
			{
				150,
				118
			}, 169, 60),
			new FF9MIX_DATA(3000, new Byte[]
			{
				70,
				156
			}, 170, 48),
			new FF9MIX_DATA(6000, new Byte[]
			{
				81,
				168
			}, 171, 32),
			new FF9MIX_DATA(8000, new Byte[]
			{
				161,
				97
			}, 172, 32),
			new FF9MIX_DATA(8000, new Byte[]
			{
				161,
				96
			}, 173, 32),
			new FF9MIX_DATA(20000, new Byte[]
			{
				170,
				90
			}, 174, 64),
			new FF9MIX_DATA(30000, new Byte[]
			{
				172,
				173
			}, 175, 128),
			new FF9MIX_DATA(50000, new Byte[]
			{
				0,
				254
			}, 176, 128),
			new FF9MIX_DATA(45000, new Byte[]
			{
				18,
				180
			}, 191, 64),
			new FF9MIX_DATA(300, new Byte[]
			{
				112,
				149
			}, 192, 95),
			new FF9MIX_DATA(400, new Byte[]
			{
				114,
				115
			}, 212, 95),
			new FF9MIX_DATA(500, new Byte[]
			{
				90,
				89
			}, 202, 95),
			new FF9MIX_DATA(900, new Byte[]
			{
				192,
				79
			}, 194, 94),
			new FF9MIX_DATA(1000, new Byte[]
			{
				117,
				136
			}, 218, 62),
			new FF9MIX_DATA(1200, new Byte[]
			{
				73,
				57
			}, 206, 62),
			new FF9MIX_DATA(1300, new Byte[]
			{
				178,
				242
			}, 213, 94),
			new FF9MIX_DATA(1500, new Byte[]
			{
				194,
				91
			}, 193, 60),
			new FF9MIX_DATA(1800, new Byte[]
			{
				80,
				139
			}, 219, 60),
			new FF9MIX_DATA(2000, new Byte[]
			{
				202,
				179
			}, 200, 60),
			new FF9MIX_DATA(3000, new Byte[]
			{
				91,
				59
			}, 203, 56),
			new FF9MIX_DATA(3200, new Byte[]
			{
				93,
				242
			}, 214, 56),
			new FF9MIX_DATA(3500, new Byte[]
			{
				120,
				52
			}, 220, 56),
			new FF9MIX_DATA(7000, new Byte[]
			{
				199,
				203
			}, 205, 56),
			new FF9MIX_DATA(4000, new Byte[]
			{
				213,
				231
			}, 199, 48),
			new FF9MIX_DATA(4000, new Byte[]
			{
				193,
				249
			}, 196, 48),
			new FF9MIX_DATA(4000, new Byte[]
			{
				122,
				157
			}, 201, 48),
			new FF9MIX_DATA(5000, new Byte[]
			{
				229,
				239
			}, 216, 48),
			new FF9MIX_DATA(6000, new Byte[]
			{
				94,
				230
			}, 207, 32),
			new FF9MIX_DATA(6500, new Byte[]
			{
				196,
				87
			}, 197, 32),
			new FF9MIX_DATA(7000, new Byte[]
			{
				227,
				199
			}, 208, 32),
			new FF9MIX_DATA(8000, new Byte[]
			{
				214,
				219
			}, 215, 32),
			new FF9MIX_DATA(12000, new Byte[]
			{
				197,
				228
			}, 198, 64),
			new FF9MIX_DATA(24000, new Byte[]
			{
				203,
				38
			}, 204, 64),
			new FF9MIX_DATA(40000, new Byte[]
			{
				250,
				208
			}, 209, 128),
			new FF9MIX_DATA(50000, new Byte[]
			{
				210,
				210
			}, 211, 128),
			new FF9MIX_DATA(350, new Byte[]
			{
				254,
				247
			}, 224, 224),
			new FF9MIX_DATA(200, new Byte[]
			{
				254,
				248
			}, 225, 224),
			new FF9MIX_DATA(100, new Byte[]
			{
				254,
				242
			}, 231, 224),
			new FF9MIX_DATA(200, new Byte[]
			{
				254,
				243
			}, 232, 224),
			new FF9MIX_DATA(100, new Byte[]
			{
				254,
				236
			}, 233, 224),
			new FF9MIX_DATA(100, new Byte[]
			{
				254,
				244
			}, 234, 224),
			new FF9MIX_DATA(400, new Byte[]
			{
				254,
				252
			}, 235, 192),
			new FF9MIX_DATA(25000, new Byte[]
			{
				0,
				211
			}, 210, 128),
			new FF9MIX_DATA(50000, new Byte[]
			{
				31,
				103
			}, 26, 128),
			new FF9MIX_DATA(300, new Byte[]
			{
				240,
				251
			}, 249, 128),
			new FF9MIX_DATA(500, new Byte[]
			{
				241,
				246
			}, 238, 128),
			new FF9MIX_DATA(50000, new Byte[]
			{
				92,
				12
			}, 98, 32)
		};
	}
}
