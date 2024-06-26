using Memoria.Prime.Collections;
using System;
using System.Collections.Generic;

public partial class FF9DBAll
{
	public const Int32 SFX_EVENT_SYSTEM_SE000000 = 104;

	public const Int32 SFX_EVENT_SYSTEM_SE000001 = 103;

	public const Int32 SFX_EVENT_SYSTEM_SE000002 = 102;

	public const Int32 SFX_EVENT_SYSTEM_SE000003 = 101;

	public const Int32 SFX_EVENT_SYSTEM_SE000004 = 108;

	public const Int32 SFX_EVENT_SYSTEM_SE000005 = 107;

	public const Int32 SFX_EVENT_SYSTEM_SE000006 = 106;

	public const Int32 SFX_EVENT_SYSTEM_SE000007 = 105;

	public const Int32 SFX_EVENT_SYSTEM_SE000008 = 109;

	public const Int32 SFX_EVENT_SYSTEM_SE000009 = 636;

	public const Int32 SFX_EVENT_SYSTEM_SE000010 = 635;

	public const Int32 SFX_EVENT_SYSTEM_SE000011 = 634;

	public const Int32 SFX_EVENT_SYSTEM_SE000012 = 633;

	public const Int32 SFX_EVENT_SYSTEM_SE000013 = 637;

	public const Int32 SFX_EVENT_SYSTEM_SE000014 = 638;

	public const Int32 SFX_EVENT_SYSTEM_SE000015 = 682;

	public const Int32 SFX_EVENT_SYSTEM_SE000016 = 683;

	public const Int32 SFX_EVENT_SYSTEM_SE000017 = 1043;

	public const Int32 SFX_EVENT_SYSTEM_SE000018 = 1044;

	public const Int32 SFX_EVENT_SYSTEM_SE000019 = 1045;

	public const Int32 SFX_EVENT_SYSTEM_SE000020 = 1046;

	public const Int32 SFX_EVENT_SYSTEM_SE000021 = 1047;

	public const Int32 SFX_EVENT_SYSTEM_SE000022 = 1230;

	public const Int32 SFX_EVENT_SYSTEM_SE000026 = 1261;

	public const Int32 SFX_EVENT_SYSTEM_SE000027 = 1324;

	public const Int32 SFX_EVENT_SYSTEM_SE000028 = 1325;

	public const Int32 SFX_EVENT_SYSTEM_SE000029 = 1350;

	public const Int32 SFX_EVENT_SYSTEM_SE000030 = 1351;

	public const Int32 SFX_EVENT_SYSTEM_SE000031 = 1361;

	public const Int32 SFX_EVENT_SYSTEM_SE000032 = 1362;

	public const Int32 SFX_EVENT_SYSTEM_SE000033 = 1363;

	public const Int32 SFX_EVENT_SYSTEM_SE000034 = 1617;

	public const Int32 SFX_EVENT_SYSTEM_SE000035 = 1991;

	public const Int32 SFX_EVENT_SYSTEM_SE000036 = 2342;

	public const Int32 SFX_EVENT_SYSTEM_SE000037 = 2631;

	public const Int32 SFX_EVENT_SYSTEM_SE000038 = 2632;

	public const Int32 SFX_EVENT_SYSTEM_SE000040 = 2633;

	public const Int32 SFX_EVENT_SYSTEM_SE000042 = 2979;

	public const Int32 SFX_EVENT_SYSTEM_SE000043 = 3096;

	public const Int32 SFX_EVENT_SYSTEM_SE000047 = 3105;

	public const Int32 SFX_EVENT_SYSTEM_SE000049 = 3110;

	public const Int32 SFX_EVENT_WORLD_SE200000 = 883;

	public const Int32 SFX_EVENT_WORLD_SE200001 = 884;

	public const Int32 SFX_EVENT_WORLD_SE200002 = 885;

	public const Int32 SFX_EVENT_WORLD_SE200003 = 886;

	public const Int32 SFX_EVENT_WORLD_SE200004 = 887;

	public const Int32 SFX_EVENT_WORLD_SE200005 = 888;

	public const Int32 SFX_EVENT_WORLD_SE200006 = 889;

	public const Int32 SFX_EVENT_WORLD_SE200007 = 890;

	public const Int32 SFX_EVENT_WORLD_SE200008 = 891;

	public const Int32 SFX_EVENT_WORLD_SE200009 = 1098;

	public const Int32 SFX_EVENT_WORLD_SE200010 = 1099;

	public const Int32 SFX_EVENT_WORLD_SE200011 = 892;

	public const Int32 SFX_EVENT_WORLD_SE200012 = 1100;

	public const Int32 SFX_EVENT_WORLD_SE200013 = 893;

	public const Int32 SFX_EVENT_WORLD_SE200014 = 894;

	public const Int32 SFX_EVENT_WORLD_SE200015 = 895;

	public const Int32 SFX_EVENT_WORLD_SE200016 = 896;

	public const Int32 SFX_EVENT_WORLD_SE200020 = 1231;

	public const Int32 SFX_EVENT_WORLD_SE200024 = 1364;

	public const Int32 SFX_EVENT_WORLD_SE200025 = 1365;

	public const Int32 SFX_EVENT_WORLD_SE200026 = 1430;

	public const Int32 SFX_EVENT_WORLD_SE200027 = 1497;

	public const Int32 SFX_EVENT_WORLD_SE200028 = 1498;

	public const Int32 SFX_EVENT_WORLD_SE200029 = 1499;

	public const Int32 SFX_BTL_SE010000 = 31;

	public const Int32 SFX_BTL_SE010001 = 21;

	public const Int32 SFX_BTL_SE010002 = 9;

	public const Int32 SFX_BTL_SE010003 = 1;

	public const Int32 SFX_BTL_SE010004 = 71;

	public const Int32 SFX_BTL_SE010005 = 63;

	public const Int32 SFX_BTL_SE010006 = 52;

	public const Int32 SFX_BTL_SE010007 = 45;

	public const Int32 SFX_BTL_SE010008 = 91;

	public const Int32 SFX_BTL_SE010009 = 83;

	public const Int32 SFX_BTL_SE010010 = 30;

	public const Int32 SFX_BTL_SE010011 = 20;

	public const Int32 SFX_BTL_SE010012 = 8;

	public const Int32 SFX_BTL_SE010013 = 0;

	public const Int32 SFX_BTL_SE010014 = 70;

	public const Int32 SFX_BTL_SE010015 = 62;

	public const Int32 SFX_BTL_SE010016 = 51;

	public const Int32 SFX_BTL_SE010017 = 44;

	public const Int32 SFX_BTL_SE010018 = 90;

	public const Int32 SFX_BTL_SE010019 = 82;

	public const Int32 SFX_BTL_SE010020 = 32;

	public const Int32 SFX_BTL_SE010021 = 22;

	public const Int32 SFX_BTL_SE010025 = 897;

	public const Int32 SFX_BTL_SE010029 = 898;

	public const Int32 SFX_BTL_SE010032 = 899;

	public const Int32 SFX_BTL_SE010033 = 900;

	public const Int32 SFX_BTL_SE010034 = 901;

	public const Int32 SFX_BTL_SE010035 = 902;

	public const Int32 SFX_BTL_SE010036 = 903;

	public const Int32 SFX_BTL_SE010037 = 904;

	public const Int32 SFX_BTL_SE010038 = 905;

	public const Int32 SFX_BTL_SE010039 = 906;

	public const Int32 SFX_BTL_SE010040 = 1110;

	public const Int32 SFX_BTL_SE010041 = 2752;

	public const Int32 SFX_BTL_SE010128 = 673;

	public const Int32 SFX_BTL_SE010132 = 664;

	public const Int32 SFX_BTL_SE010136 = 668;

	public const Int32 SFX_BTL_SE010140 = 665;

	public const Int32 SFX_BTL_SE010144 = 669;

	public const Int32 SFX_BTL_SE010148 = 671;

	public const Int32 SFX_BTL_SE010152 = 663;

	public const Int32 SFX_BTL_SE010156 = 667;

	public const Int32 SFX_BTL_SE010160 = 666;

	public const Int32 SFX_BTL_SE010164 = 670;

	public const Int32 SFX_BTL_SE010168 = 672;

	public const Int32 SFX_BTL_SE010172 = 674;

	public const Int32 SFX_BTL_SE010176 = 678;

	public const Int32 SFX_BTL_SE010180 = 677;

	public const Int32 SFX_BTL_SE010182 = 726;

	public const Int32 SFX_BTL_SE010184 = 680;

	public const Int32 SFX_BTL_SE010188 = 681;

	public const Int32 SFX_BTL_SE010192 = 675;

	public const Int32 SFX_BTL_SE010196 = 679;

	public const Int32 SFX_BTL_SE010200 = 676;

	public const Int32 SFX_BTL_SE010204 = 691;

	public const Int32 SFX_BTL_SE010208 = 725;

	public const Int32 SFX_BTL_SE010212 = 724;

	public const Int32 SFX_BTL_SE010216 = 1556;

	public const Int32 SFX_BTL_SE010220 = 1557;

	public const Int32 SFX_BTL_SE010224 = 1558;

	public const Int32 SFX_BTL_SE010225 = 2774;

	public const Int32 SFX_BTL_SE010226 = 3091;

	public const Int32 SFX_BTL_SE010228 = 3092;

	public const Int32 SFX_BTL_SE010229 = 3093;

	public const Int32 SFX_BTL_SE010232 = 3097;

	public const Int32 SFX_BTL_SE010256 = 121;

	public const Int32 SFX_BTL_SE010257 = 120;

	public const Int32 SFX_BTL_SE010258 = 2906;

	public const Int32 SFX_BTL_SE010259 = 2907;

	public const Int32 SFX_BTL_SE010260 = 2908;

	public const Int32 SFX_BTL_SE020000 = 39;

	public const Int32 SFX_BTL_SE020001 = 27;

	public const Int32 SFX_BTL_SE020002 = 15;

	public const Int32 SFX_BTL_SE020004 = 77;

	public const Int32 SFX_BTL_SE020005 = 67;

	public const Int32 SFX_BTL_SE020006 = 58;

	public const Int32 SFX_BTL_SE020008 = 97;

	public const Int32 SFX_BTL_SE020009 = 87;

	public const Int32 SFX_BTL_SE020010 = 38;

	public const Int32 SFX_BTL_SE020012 = 14;

	public const Int32 SFX_BTL_SE020013 = 4;

	public const Int32 SFX_BTL_SE020014 = 76;

	public const Int32 SFX_BTL_SE020016 = 57;

	public const Int32 SFX_BTL_SE020017 = 48;

	public const Int32 SFX_BTL_SE020018 = 96;

	public const Int32 SFX_BTL_SE020020 = 41;

	public const Int32 SFX_BTL_SE020021 = 28;

	public const Int32 SFX_BTL_SE020022 = 17;

	public const Int32 SFX_BTL_SE020024 = 79;

	public const Int32 SFX_BTL_SE020025 = 68;

	public const Int32 SFX_BTL_SE020026 = 60;

	public const Int32 SFX_BTL_SE020028 = 99;

	public const Int32 SFX_BTL_SE020029 = 88;

	public const Int32 SFX_BTL_SE020030 = 40;

	public const Int32 SFX_BTL_SE020032 = 16;

	public const Int32 SFX_BTL_SE020033 = 6;

	public const Int32 SFX_BTL_SE020034 = 78;

	public const Int32 SFX_BTL_SE020036 = 59;

	public const Int32 SFX_BTL_SE020037 = 50;

	public const Int32 SFX_BTL_SE020038 = 98;

	public const Int32 SFX_BTL_SE020040 = 35;

	public const Int32 SFX_BTL_SE020041 = 24;

	public const Int32 SFX_BTL_SE020042 = 11;

	public const Int32 SFX_BTL_SE020044 = 73;

	public const Int32 SFX_BTL_SE020045 = 64;

	public const Int32 SFX_BTL_SE020046 = 54;

	public const Int32 SFX_BTL_SE020048 = 93;

	public const Int32 SFX_BTL_SE020049 = 84;

	public const Int32 SFX_BTL_SE020050 = 34;

	public const Int32 SFX_BTL_SE020052 = 10;

	public const Int32 SFX_BTL_SE020053 = 2;

	public const Int32 SFX_BTL_SE020054 = 72;

	public const Int32 SFX_BTL_SE020056 = 53;

	public const Int32 SFX_BTL_SE020057 = 46;

	public const Int32 SFX_BTL_SE020058 = 92;

	public const Int32 SFX_BTL_SE020060 = 37;

	public const Int32 SFX_BTL_SE020061 = 25;

	public const Int32 SFX_BTL_SE020062 = 13;

	public const Int32 SFX_BTL_SE020064 = 75;

	public const Int32 SFX_BTL_SE020065 = 65;

	public const Int32 SFX_BTL_SE020066 = 56;

	public const Int32 SFX_BTL_SE020068 = 95;

	public const Int32 SFX_BTL_SE020069 = 85;

	public const Int32 SFX_BTL_SE020070 = 36;

	public const Int32 SFX_BTL_SE020072 = 12;

	public const Int32 SFX_BTL_SE020073 = 3;

	public const Int32 SFX_BTL_SE020074 = 74;

	public const Int32 SFX_BTL_SE020076 = 55;

	public const Int32 SFX_BTL_SE020077 = 47;

	public const Int32 SFX_BTL_SE020078 = 94;

	public const Int32 SFX_BTL_SE020080 = 43;

	public const Int32 SFX_BTL_SE020081 = 29;

	public const Int32 SFX_BTL_SE020082 = 19;

	public const Int32 SFX_BTL_SE020084 = 81;

	public const Int32 SFX_BTL_SE020085 = 69;

	public const Int32 SFX_BTL_SE020086 = 61;

	public const Int32 SFX_BTL_SE020088 = 100;

	public const Int32 SFX_BTL_SE020089 = 89;

	public const Int32 SFX_BTL_SE020090 = 42;

	public const Int32 SFX_BTL_SE020092 = 18;

	public const Int32 SFX_BTL_SE020093 = 7;

	public const Int32 SFX_BTL_SE020094 = 80;

	public const Int32 SFX_BTL_SE020096 = 218;

	public const Int32 SFX_BTL_SE020097 = 202;

	public const Int32 SFX_BTL_SE020098 = 284;

	public const Int32 SFX_BTL_SE020100 = 196;

	public const Int32 SFX_BTL_SE020101 = 178;

	public const Int32 SFX_BTL_SE020102 = 167;

	public const Int32 SFX_BTL_SE020104 = 260;

	public const Int32 SFX_BTL_SE020105 = 238;

	public const Int32 SFX_BTL_SE020106 = 223;

	public const Int32 SFX_BTL_SE020108 = 324;

	public const Int32 SFX_BTL_SE020109 = 321;

	public const Int32 SFX_BTL_SE020110 = 310;

	public const Int32 SFX_BTL_SE020112 = 166;

	public const Int32 SFX_BTL_SE020113 = 146;

	public const Int32 SFX_BTL_SE020114 = 259;

	public const Int32 SFX_BTL_SE020116 = 222;

	public const Int32 SFX_BTL_SE020117 = 205;

	public const Int32 SFX_BTL_SE020118 = 289;

	public const Int32 SFX_BTL_SE020120 = 641;

	public const Int32 SFX_BTL_SE020121 = 640;

	public const Int32 SFX_BTL_SE020122 = 639;

	public const Int32 SFX_BTL_SE020124 = 261;

	public const Int32 SFX_BTL_SE020125 = 239;

	public const Int32 SFX_BTL_SE020126 = 225;

	public const Int32 SFX_BTL_SE020128 = 325;

	public const Int32 SFX_BTL_SE020129 = 322;

	public const Int32 SFX_BTL_SE020130 = 311;

	public const Int32 SFX_BTL_SE020132 = 301;

	public const Int32 SFX_BTL_SE020133 = 297;

	public const Int32 SFX_BTL_SE020134 = 320;

	public const Int32 SFX_BTL_SE020136 = 224;

	public const Int32 SFX_BTL_SE020137 = 206;

	public const Int32 SFX_BTL_SE020138 = 290;

	public const Int32 SFX_BTL_SE020140 = 308;

	public const Int32 SFX_BTL_SE020141 = 304;

	public const Int32 SFX_BTL_SE020142 = 299;

	public const Int32 SFX_BTL_SE020144 = 319;

	public const Int32 SFX_BTL_SE020145 = 317;

	public const Int32 SFX_BTL_SE020146 = 316;

	public const Int32 SFX_BTL_SE020148 = 286;

	public const Int32 SFX_BTL_SE020149 = 271;

	public const Int32 SFX_BTL_SE020150 = 194;

	public const Int32 SFX_BTL_SE020152 = 164;

	public const Int32 SFX_BTL_SE020153 = 144;

	public const Int32 SFX_BTL_SE020154 = 256;

	public const Int32 SFX_BTL_SE020156 = 219;

	public const Int32 SFX_BTL_SE020157 = 203;

	public const Int32 SFX_BTL_SE020158 = 285;

	public const Int32 SFX_BTL_SE020160 = 309;

	public const Int32 SFX_BTL_SE020161 = 305;

	public const Int32 SFX_BTL_SE020162 = 300;

	public const Int32 SFX_BTL_SE020164 = 258;

	public const Int32 SFX_BTL_SE020165 = 237;

	public const Int32 SFX_BTL_SE020166 = 221;

	public const Int32 SFX_BTL_SE020168 = 288;

	public const Int32 SFX_BTL_SE020169 = 272;

	public const Int32 SFX_BTL_SE020170 = 195;

	public const Int32 SFX_BTL_SE020172 = 165;

	public const Int32 SFX_BTL_SE020173 = 145;

	public const Int32 SFX_BTL_SE020174 = 257;

	public const Int32 SFX_BTL_SE020176 = 220;

	public const Int32 SFX_BTL_SE020177 = 204;

	public const Int32 SFX_BTL_SE020178 = 287;

	public const Int32 SFX_BTL_SE020180 = 313;

	public const Int32 SFX_BTL_SE020181 = 307;

	public const Int32 SFX_BTL_SE020182 = 303;

	public const Int32 SFX_BTL_SE020184 = 263;

	public const Int32 SFX_BTL_SE020185 = 240;

	public const Int32 SFX_BTL_SE020186 = 227;

	public const Int32 SFX_BTL_SE020188 = 292;

	public const Int32 SFX_BTL_SE020189 = 273;

	public const Int32 SFX_BTL_SE020190 = 197;

	public const Int32 SFX_BTL_SE020192 = 168;

	public const Int32 SFX_BTL_SE020193 = 147;

	public const Int32 SFX_BTL_SE020194 = 262;

	public const Int32 SFX_BTL_SE020196 = 226;

	public const Int32 SFX_BTL_SE020197 = 207;

	public const Int32 SFX_BTL_SE020198 = 291;

	public const Int32 SFX_BTL_SE020200 = 184;

	public const Int32 SFX_BTL_SE020201 = 171;

	public const Int32 SFX_BTL_SE020202 = 152;

	public const Int32 SFX_BTL_SE020204 = 246;

	public const Int32 SFX_BTL_SE020205 = 232;

	public const Int32 SFX_BTL_SE020206 = 213;

	public const Int32 SFX_BTL_SE020208 = 280;

	public const Int32 SFX_BTL_SE020209 = 267;

	public const Int32 SFX_BTL_SE020210 = 183;

	public const Int32 SFX_BTL_SE020212 = 151;

	public const Int32 SFX_BTL_SE020213 = 138;

	public const Int32 SFX_BTL_SE020214 = 245;

	public const Int32 SFX_BTL_SE020216 = 212;

	public const Int32 SFX_BTL_SE020217 = 201;

	public const Int32 SFX_BTL_SE020218 = 279;

	public const Int32 SFX_BTL_SE020220 = 186;

	public const Int32 SFX_BTL_SE020221 = 172;

	public const Int32 SFX_BTL_SE020222 = 153;

	public const Int32 SFX_BTL_SE020224 = 247;

	public const Int32 SFX_BTL_SE020225 = 233;

	public const Int32 SFX_BTL_SE020226 = 214;

	public const Int32 SFX_BTL_SE020228 = 281;

	public const Int32 SFX_BTL_SE020229 = 268;

	public const Int32 SFX_BTL_SE020230 = 185;

	public const Int32 SFX_BTL_SE020232 = 298;

	public const Int32 SFX_BTL_SE020233 = 296;

	public const Int32 SFX_BTL_SE020234 = 318;

	public const Int32 SFX_BTL_SE020236 = 315;

	public const Int32 SFX_BTL_SE020237 = 314;

	public const Int32 SFX_BTL_SE020238 = 323;

	public const Int32 SFX_BTL_SE020240 = 180;

	public const Int32 SFX_BTL_SE020241 = 169;

	public const Int32 SFX_BTL_SE020242 = 148;

	public const Int32 SFX_BTL_SE020248 = 276;

	public const Int32 SFX_BTL_SE020249 = 265;

	public const Int32 SFX_BTL_SE020250 = 179;

	public const Int32 SFX_BTL_SE020252 = 327;

	public const Int32 SFX_BTL_SE020253 = 326;

	public const Int32 SFX_BTL_SE020254 = 341;

	public const Int32 SFX_BTL_SE020256 = 335;

	public const Int32 SFX_BTL_SE020257 = 331;

	public const Int32 SFX_BTL_SE020258 = 343;

	public const Int32 SFX_BTL_SE020260 = 182;

	public const Int32 SFX_BTL_SE020261 = 170;

	public const Int32 SFX_BTL_SE020262 = 150;

	public const Int32 SFX_BTL_SE020264 = 244;

	public const Int32 SFX_BTL_SE020265 = 231;

	public const Int32 SFX_BTL_SE020266 = 211;

	public const Int32 SFX_BTL_SE020268 = 278;

	public const Int32 SFX_BTL_SE020269 = 266;

	public const Int32 SFX_BTL_SE020270 = 181;

	public const Int32 SFX_BTL_SE020272 = 149;

	public const Int32 SFX_BTL_SE020273 = 137;

	public const Int32 SFX_BTL_SE020274 = 243;

	public const Int32 SFX_BTL_SE020276 = 200;

	public const Int32 SFX_BTL_SE020277 = 209;

	public const Int32 SFX_BTL_SE020278 = 210;

	public const Int32 SFX_BTL_SE020280 = 187;

	public const Int32 SFX_BTL_SE020281 = 173;

	public const Int32 SFX_BTL_SE020282 = 1457;

	public const Int32 SFX_BTL_SE020284 = 249;

	public const Int32 SFX_BTL_SE020285 = 234;

	public const Int32 SFX_BTL_SE020286 = 215;

	public const Int32 SFX_BTL_SE020288 = 382;

	public const Int32 SFX_BTL_SE020289 = 379;

	public const Int32 SFX_BTL_SE020290 = 357;

	public const Int32 SFX_BTL_SE020292 = 154;

	public const Int32 SFX_BTL_SE020293 = 139;

	public const Int32 SFX_BTL_SE020294 = 248;

	public const Int32 SFX_BTL_SE020296 = 336;

	public const Int32 SFX_BTL_SE020297 = 332;

	public const Int32 SFX_BTL_SE020298 = 344;

	public const Int32 SFX_BTL_SE020300 = 190;

	public const Int32 SFX_BTL_SE020301 = 175;

	public const Int32 SFX_BTL_SE020302 = 160;

	public const Int32 SFX_BTL_SE020308 = 282;

	public const Int32 SFX_BTL_SE020309 = 269;

	public const Int32 SFX_BTL_SE020310 = 189;

	public const Int32 SFX_BTL_SE020312 = 159;

	public const Int32 SFX_BTL_SE020313 = 142;

	public const Int32 SFX_BTL_SE020314 = 252;

	public const Int32 SFX_BTL_SE020316 = 338;

	public const Int32 SFX_BTL_SE020317 = 333;

	public const Int32 SFX_BTL_SE020318 = 345;

	public const Int32 SFX_BTL_SE020320 = 191;

	public const Int32 SFX_BTL_SE020321 = 176;

	public const Int32 SFX_BTL_SE020322 = 162;

	public const Int32 SFX_BTL_SE020324 = 413;

	public const Int32 SFX_BTL_SE020325 = 411;

	public const Int32 SFX_BTL_SE020326 = 409;

	public const Int32 SFX_BTL_SE020328 = 608;

	public const Int32 SFX_BTL_SE020329 = 607;

	public const Int32 SFX_BTL_SE020330 = 606;

	public const Int32 SFX_BTL_SE020332 = 161;

	public const Int32 SFX_BTL_SE020333 = 143;

	public const Int32 SFX_BTL_SE020334 = 254;

	public const Int32 SFX_BTL_SE020336 = 339;

	public const Int32 SFX_BTL_SE020337 = 334;

	public const Int32 SFX_BTL_SE020338 = 346;

	public const Int32 SFX_BTL_SE020340 = 330;

	public const Int32 SFX_BTL_SE020341 = 329;

	public const Int32 SFX_BTL_SE020342 = 328;

	public const Int32 SFX_BTL_SE020344 = 342;

	public const Int32 SFX_BTL_SE020345 = 340;

	public const Int32 SFX_BTL_SE020346 = 337;

	public const Int32 SFX_BTL_SE020348 = 391;

	public const Int32 SFX_BTL_SE020349 = 390;

	public const Int32 SFX_BTL_SE020350 = 389;

	public const Int32 SFX_BTL_SE020352 = 156;

	public const Int32 SFX_BTL_SE020353 = 140;

	public const Int32 SFX_BTL_SE020354 = 250;

	public const Int32 SFX_BTL_SE020356 = 408;

	public const Int32 SFX_BTL_SE020357 = 407;

	public const Int32 SFX_BTL_SE020358 = 416;

	public const Int32 SFX_BTL_SE020360 = 188;

	public const Int32 SFX_BTL_SE020361 = 174;

	public const Int32 SFX_BTL_SE020362 = 158;

	public const Int32 SFX_BTL_SE020364 = 375;

	public const Int32 SFX_BTL_SE020365 = 373;

	public const Int32 SFX_BTL_SE020366 = 368;

	public const Int32 SFX_BTL_SE020368 = 384;

	public const Int32 SFX_BTL_SE020369 = 380;

	public const Int32 SFX_BTL_SE020370 = 358;

	public const Int32 SFX_BTL_SE020372 = 157;

	public const Int32 SFX_BTL_SE020373 = 141;

	public const Int32 SFX_BTL_SE020374 = 251;

	public const Int32 SFX_BTL_SE020376 = 367;

	public const Int32 SFX_BTL_SE020377 = 363;

	public const Int32 SFX_BTL_SE020378 = 383;

	public const Int32 SFX_BTL_SE020380 = 193;

	public const Int32 SFX_BTL_SE020381 = 177;

	public const Int32 SFX_BTL_SE020382 = 163;

	public const Int32 SFX_BTL_SE020384 = 2204;

	public const Int32 SFX_BTL_SE020385 = 2205;

	public const Int32 SFX_BTL_SE020386 = 2206;

	public const Int32 SFX_BTL_SE030000 = 33;

	public const Int32 SFX_BTL_SE030001 = 23;

	public const Int32 SFX_BTL_SE030002 = 131;

	public const Int32 SFX_BTL_SE030003 = 129;

	public const Int32 SFX_BTL_SE030004 = 399;

	public const Int32 SFX_BTL_SE030005 = 730;

	public const Int32 SFX_BTL_SE030007 = 397;

	public const Int32 SFX_BTL_SE030008 = 401;

	public const Int32 SFX_BTL_SE030010 = 395;

	public const Int32 SFX_BTL_SE030011 = 393;

	public const Int32 SFX_BTL_SE030012 = 740;

	public const Int32 SFX_BTL_SE030013 = 128;

	public const Int32 SFX_BTL_SE030014 = 136;

	public const Int32 SFX_BTL_SE030015 = 741;

	public const Int32 SFX_BTL_SE030016 = 742;

	public const Int32 SFX_BTL_SE030017 = 743;

	public const Int32 SFX_BTL_SE030018 = 744;

	public const Int32 SFX_BTL_SE030019 = 111;

	public const Int32 SFX_BTL_SE030020 = 115;

	public const Int32 SFX_BTL_SE030021 = 110;

	public const Int32 SFX_BTL_SE030022 = 745;

	public const Int32 SFX_BTL_SE030023 = 746;

	public const Int32 SFX_BTL_SE030025 = 119;

	public const Int32 SFX_BTL_SE030026 = 117;

	public const Int32 SFX_BTL_SE030027 = 747;

	public const Int32 SFX_BTL_SE030028 = 748;

	public const Int32 SFX_BTL_SE030030 = 749;

	public const Int32 SFX_BTL_SE030032 = 750;

	public const Int32 SFX_BTL_SE030033 = 751;

	public const Int32 SFX_BTL_SE030034 = 752;

	public const Int32 SFX_BTL_SE030035 = 753;

	public const Int32 SFX_BTL_SE030036 = 754;

	public const Int32 SFX_BTL_SE030037 = 755;

	public const Int32 SFX_BTL_SE030039 = 756;

	public const Int32 SFX_BTL_SE030040 = 134;

	public const Int32 SFX_BTL_SE030041 = 757;

	public const Int32 SFX_BTL_SE030042 = 1262;

	public const Int32 SFX_BTL_SE030044 = 758;

	public const Int32 SFX_BTL_SE030045 = 759;

	public const Int32 SFX_BTL_SE030046 = 760;

	public const Int32 SFX_BTL_SE030047 = 761;

	public const Int32 SFX_BTL_SE030048 = 762;

	public const Int32 SFX_BTL_SE030049 = 763;

	public const Int32 SFX_BTL_SE030050 = 764;

	public const Int32 SFX_BTL_SE030052 = 765;

	public const Int32 SFX_BTL_SE030053 = 766;

	public const Int32 SFX_BTL_SE030056 = 767;

	public const Int32 SFX_BTL_SE030057 = 768;

	public const Int32 SFX_BTL_SE030058 = 769;

	public const Int32 SFX_BTL_SE030060 = 770;

	public const Int32 SFX_BTL_SE030062 = 113;

	public const Int32 SFX_BTL_SE030063 = 771;

	public const Int32 SFX_BTL_SE030064 = 772;

	public const Int32 SFX_BTL_SE030065 = 773;

	public const Int32 SFX_BTL_SE030066 = 774;

	public const Int32 SFX_BTL_SE030067 = 775;

	public const Int32 SFX_BTL_SE030068 = 776;

	public const Int32 SFX_BTL_SE030069 = 777;

	public const Int32 SFX_BTL_SE030070 = 778;

	public const Int32 SFX_BTL_SE030071 = 779;

	public const Int32 SFX_BTL_SE030072 = 1263;

	public const Int32 SFX_BTL_SE030073 = 780;

	public const Int32 SFX_BTL_SE030074 = 781;

	public const Int32 SFX_BTL_SE030075 = 782;

	public const Int32 SFX_BTL_SE030076 = 783;

	public const Int32 SFX_BTL_SE030077 = 784;

	public const Int32 SFX_BTL_SE030079 = 1264;

	public const Int32 SFX_BTL_SE030081 = 1265;

	public const Int32 SFX_BTL_SE030082 = 785;

	public const Int32 SFX_BTL_SE030083 = 1266;

	public const Int32 SFX_BTL_SE030084 = 786;

	public const Int32 SFX_BTL_SE030085 = 787;

	public const Int32 SFX_BTL_SE030086 = 788;

	public const Int32 SFX_BTL_SE030087 = 789;

	public const Int32 SFX_BTL_SE030088 = 790;

	public const Int32 SFX_BTL_SE030092 = 791;

	public const Int32 SFX_BTL_SE030093 = 792;

	public const Int32 SFX_BTL_SE030094 = 1267;

	public const Int32 SFX_BTL_SE030102 = 1268;

	public const Int32 SFX_BTL_SE030103 = 1269;

	public const Int32 SFX_BTL_SE030108 = 734;

	public const Int32 SFX_BTL_SE030109 = 732;

	public const Int32 SFX_BTL_SE030110 = 728;

	public const Int32 SFX_BTL_SE030111 = 1270;

	public const Int32 SFX_BTL_SE030112 = 793;

	public const Int32 SFX_BTL_SE030114 = 1271;

	public const Int32 SFX_BTL_SE030115 = 1272;

	public const Int32 SFX_BTL_SE030116 = 1273;

	public const Int32 SFX_BTL_SE030117 = 794;

	public const Int32 SFX_BTL_SE030118 = 1274;

	public const Int32 SFX_BTL_SE030121 = 795;

	public const Int32 SFX_BTL_SE030123 = 796;

	public const Int32 SFX_BTL_SE030124 = 1275;

	public const Int32 SFX_BTL_SE030127 = 1276;

	public const Int32 SFX_BTL_SE030131 = 1277;

	public const Int32 SFX_BTL_SE030136 = 1278;

	public const Int32 SFX_BTL_SE030137 = 1279;

	public const Int32 SFX_BTL_SE030138 = 797;

	public const Int32 SFX_BTL_SE030140 = 1280;

	public const Int32 SFX_BTL_SE030141 = 1281;

	public const Int32 SFX_BTL_SE030142 = 1282;

	public const Int32 SFX_BTL_SE030143 = 798;

	public const Int32 SFX_BTL_SE030145 = 1283;

	public const Int32 SFX_BTL_SE030146 = 1284;

	public const Int32 SFX_BTL_SE030147 = 1285;

	public const Int32 SFX_BTL_SE030156 = 2113;

	public const Int32 SFX_BTL_SE030157 = 2115;

	public const Int32 SFX_BTL_SE030159 = 2116;

	public const Int32 SFX_BTL_SE030160 = 2117;

	public const Int32 SFX_BTL_SE030161 = 2118;

	public const Int32 SFX_BTL_SE030163 = 2119;

	public const Int32 SFX_BTL_SE030164 = 2120;

	public const Int32 SFX_BTL_SE030166 = 799;

	public const Int32 SFX_BTL_SE030167 = 1310;

	public const Int32 SFX_BTL_SE030176 = 800;

	public const Int32 SFX_BTL_SE030189 = 801;

	public const Int32 SFX_BTL_SE030190 = 802;

	public const Int32 SFX_BTL_SE030191 = 2121;

	public const Int32 SFX_BTL_SE030192 = 2122;

	public const Int32 SFX_BTL_SE030193 = 2123;

	public const Int32 SFX_BTL_SE030194 = 2124;

	public const Int32 SFX_BTL_SE030197 = 2125;

	public const Int32 SFX_BTL_SE031001 = 132;

	public const Int32 SFX_BTL_SE031002 = 130;

	public const Int32 SFX_BTL_SE031003 = 127;

	public const Int32 SFX_BTL_SE031004 = 398;

	public const Int32 SFX_BTL_SE031005 = 729;

	public const Int32 SFX_BTL_SE031007 = 396;

	public const Int32 SFX_BTL_SE031008 = 400;

	public const Int32 SFX_BTL_SE031010 = 394;

	public const Int32 SFX_BTL_SE031011 = 392;

	public const Int32 SFX_BTL_SE031012 = 803;

	public const Int32 SFX_BTL_SE031013 = 126;

	public const Int32 SFX_BTL_SE031014 = 135;

	public const Int32 SFX_BTL_SE031015 = 804;

	public const Int32 SFX_BTL_SE031016 = 805;

	public const Int32 SFX_BTL_SE031017 = 806;

	public const Int32 SFX_BTL_SE031018 = 807;

	public const Int32 SFX_BTL_SE031019 = 808;

	public const Int32 SFX_BTL_SE031020 = 123;

	public const Int32 SFX_BTL_SE031021 = 809;

	public const Int32 SFX_BTL_SE031022 = 810;

	public const Int32 SFX_BTL_SE031023 = 811;

	public const Int32 SFX_BTL_SE031025 = 125;

	public const Int32 SFX_BTL_SE031026 = 124;

	public const Int32 SFX_BTL_SE031027 = 812;

	public const Int32 SFX_BTL_SE031028 = 813;

	public const Int32 SFX_BTL_SE031030 = 814;

	public const Int32 SFX_BTL_SE031032 = 815;

	public const Int32 SFX_BTL_SE031033 = 816;

	public const Int32 SFX_BTL_SE031034 = 817;

	public const Int32 SFX_BTL_SE031035 = 818;

	public const Int32 SFX_BTL_SE031036 = 819;

	public const Int32 SFX_BTL_SE031037 = 820;

	public const Int32 SFX_BTL_SE031039 = 821;

	public const Int32 SFX_BTL_SE031040 = 133;

	public const Int32 SFX_BTL_SE031041 = 822;

	public const Int32 SFX_BTL_SE031042 = 1286;

	public const Int32 SFX_BTL_SE031044 = 823;

	public const Int32 SFX_BTL_SE031045 = 824;

	public const Int32 SFX_BTL_SE031046 = 825;

	public const Int32 SFX_BTL_SE031047 = 826;

	public const Int32 SFX_BTL_SE031048 = 827;

	public const Int32 SFX_BTL_SE031049 = 828;

	public const Int32 SFX_BTL_SE031050 = 829;

	public const Int32 SFX_BTL_SE031052 = 830;

	public const Int32 SFX_BTL_SE031053 = 831;

	public const Int32 SFX_BTL_SE031056 = 832;

	public const Int32 SFX_BTL_SE031057 = 833;

	public const Int32 SFX_BTL_SE031058 = 834;

	public const Int32 SFX_BTL_SE031060 = 835;

	public const Int32 SFX_BTL_SE031062 = 122;

	public const Int32 SFX_BTL_SE031063 = 836;

	public const Int32 SFX_BTL_SE031064 = 837;

	public const Int32 SFX_BTL_SE031065 = 838;

	public const Int32 SFX_BTL_SE031066 = 839;

	public const Int32 SFX_BTL_SE031067 = 840;

	public const Int32 SFX_BTL_SE031068 = 841;

	public const Int32 SFX_BTL_SE031069 = 842;

	public const Int32 SFX_BTL_SE031070 = 843;

	public const Int32 SFX_BTL_SE031071 = 844;

	public const Int32 SFX_BTL_SE031072 = 1287;

	public const Int32 SFX_BTL_SE031073 = 845;

	public const Int32 SFX_BTL_SE031074 = 846;

	public const Int32 SFX_BTL_SE031075 = 847;

	public const Int32 SFX_BTL_SE031076 = 848;

	public const Int32 SFX_BTL_SE031077 = 849;

	public const Int32 SFX_BTL_SE031079 = 1288;

	public const Int32 SFX_BTL_SE031081 = 1289;

	public const Int32 SFX_BTL_SE031082 = 850;

	public const Int32 SFX_BTL_SE031083 = 1290;

	public const Int32 SFX_BTL_SE031084 = 851;

	public const Int32 SFX_BTL_SE031085 = 852;

	public const Int32 SFX_BTL_SE031086 = 853;

	public const Int32 SFX_BTL_SE031087 = 854;

	public const Int32 SFX_BTL_SE031088 = 855;

	public const Int32 SFX_BTL_SE031092 = 856;

	public const Int32 SFX_BTL_SE031093 = 857;

	public const Int32 SFX_BTL_SE031094 = 1291;

	public const Int32 SFX_BTL_SE031102 = 1292;

	public const Int32 SFX_BTL_SE031103 = 1293;

	public const Int32 SFX_BTL_SE031108 = 733;

	public const Int32 SFX_BTL_SE031109 = 731;

	public const Int32 SFX_BTL_SE031110 = 727;

	public const Int32 SFX_BTL_SE031111 = 1294;

	public const Int32 SFX_BTL_SE031112 = 858;

	public const Int32 SFX_BTL_SE031114 = 1295;

	public const Int32 SFX_BTL_SE031115 = 1296;

	public const Int32 SFX_BTL_SE031116 = 1297;

	public const Int32 SFX_BTL_SE031117 = 859;

	public const Int32 SFX_BTL_SE031118 = 1298;

	public const Int32 SFX_BTL_SE031120 = 114;

	public const Int32 SFX_BTL_SE031121 = 860;

	public const Int32 SFX_BTL_SE031123 = 861;

	public const Int32 SFX_BTL_SE031124 = 1299;

	public const Int32 SFX_BTL_SE031125 = 118;

	public const Int32 SFX_BTL_SE031126 = 116;

	public const Int32 SFX_BTL_SE031127 = 1300;

	public const Int32 SFX_BTL_SE031131 = 1301;

	public const Int32 SFX_BTL_SE031136 = 1302;

	public const Int32 SFX_BTL_SE031137 = 1303;

	public const Int32 SFX_BTL_SE031138 = 862;

	public const Int32 SFX_BTL_SE031140 = 1304;

	public const Int32 SFX_BTL_SE031141 = 1305;

	public const Int32 SFX_BTL_SE031142 = 1306;

	public const Int32 SFX_BTL_SE031143 = 863;

	public const Int32 SFX_BTL_SE031145 = 1307;

	public const Int32 SFX_BTL_SE031146 = 1308;

	public const Int32 SFX_BTL_SE031147 = 1309;

	public const Int32 SFX_BTL_SE031156 = 2109;

	public const Int32 SFX_BTL_SE031157 = 2110;

	public const Int32 SFX_BTL_SE031159 = 2111;

	public const Int32 SFX_BTL_SE031160 = 2112;

	public const Int32 SFX_BTL_SE031161 = 2114;

	public const Int32 SFX_BTL_SE031163 = 2126;

	public const Int32 SFX_BTL_SE031164 = 2127;

	public const Int32 SFX_BTL_SE031166 = 864;

	public const Int32 SFX_BTL_SE031167 = 1311;

	public const Int32 SFX_BTL_SE031176 = 865;

	public const Int32 SFX_BTL_SE031189 = 866;

	public const Int32 SFX_BTL_SE031190 = 867;

	public const Int32 SFX_BTL_SE031191 = 2128;

	public const Int32 SFX_BTL_SE031192 = 2129;

	public const Int32 SFX_BTL_SE031193 = 2130;

	public const Int32 SFX_BTL_SE031194 = 2131;

	public const Int32 SFX_BTL_SE031197 = 2132;

	public const Int32 SFX_BTL_SE500000 = 452;

	public const Int32 SFX_BTL_SE500001 = 444;

	public const Int32 SFX_BTL_SE500002 = 437;

	public const Int32 SFX_BTL_SE500004 = 476;

	public const Int32 SFX_BTL_SE500005 = 470;

	public const Int32 SFX_BTL_SE500006 = 465;

	public const Int32 SFX_BTL_SE500008 = 426;

	public const Int32 SFX_BTL_SE500009 = 419;

	public const Int32 SFX_BTL_SE500010 = 451;

	public const Int32 SFX_BTL_SE500012 = 216;

	public const Int32 SFX_BTL_SE500013 = 230;

	public const Int32 SFX_BTL_SE500014 = 235;

	public const Int32 SFX_BTL_SE500016 = 464;

	public const Int32 SFX_BTL_SE500017 = 458;

	public const Int32 SFX_BTL_SE500018 = 425;

	public const Int32 SFX_BTL_SE500020 = 454;

	public const Int32 SFX_BTL_SE500021 = 445;

	public const Int32 SFX_BTL_SE500022 = 439;

	public const Int32 SFX_BTL_SE500024 = 478;

	public const Int32 SFX_BTL_SE500025 = 471;

	public const Int32 SFX_BTL_SE500026 = 467;

	public const Int32 SFX_BTL_SE500028 = 428;

	public const Int32 SFX_BTL_SE500029 = 420;

	public const Int32 SFX_BTL_SE500030 = 453;

	public const Int32 SFX_BTL_SE500032 = 438;

	public const Int32 SFX_BTL_SE500033 = 431;

	public const Int32 SFX_BTL_SE500034 = 477;

	public const Int32 SFX_BTL_SE500036 = 466;

	public const Int32 SFX_BTL_SE500037 = 459;

	public const Int32 SFX_BTL_SE500038 = 427;

	public const Int32 SFX_BTL_SE500040 = 448;

	public const Int32 SFX_BTL_SE500041 = 442;

	public const Int32 SFX_BTL_SE500042 = 434;

	public const Int32 SFX_BTL_SE500044 = 473;

	public const Int32 SFX_BTL_SE500045 = 468;

	public const Int32 SFX_BTL_SE500046 = 461;

	public const Int32 SFX_BTL_SE500048 = 422;

	public const Int32 SFX_BTL_SE500049 = 417;

	public const Int32 SFX_BTL_SE500050 = 447;

	public const Int32 SFX_BTL_SE500052 = 433;

	public const Int32 SFX_BTL_SE500053 = 429;

	public const Int32 SFX_BTL_SE500054 = 472;

	public const Int32 SFX_BTL_SE500056 = 460;

	public const Int32 SFX_BTL_SE500057 = 456;

	public const Int32 SFX_BTL_SE500058 = 421;

	public const Int32 SFX_BTL_SE500060 = 450;

	public const Int32 SFX_BTL_SE500061 = 443;

	public const Int32 SFX_BTL_SE500062 = 436;

	public const Int32 SFX_BTL_SE500064 = 475;

	public const Int32 SFX_BTL_SE500065 = 469;

	public const Int32 SFX_BTL_SE500066 = 463;

	public const Int32 SFX_BTL_SE500068 = 424;

	public const Int32 SFX_BTL_SE500069 = 418;

	public const Int32 SFX_BTL_SE500070 = 449;

	public const Int32 SFX_BTL_SE500072 = 435;

	public const Int32 SFX_BTL_SE500073 = 430;

	public const Int32 SFX_BTL_SE500074 = 474;

	public const Int32 SFX_BTL_SE500076 = 462;

	public const Int32 SFX_BTL_SE500077 = 457;

	public const Int32 SFX_BTL_SE500078 = 423;

	public const Int32 SFX_BTL_SE500080 = 455;

	public const Int32 SFX_BTL_SE500081 = 446;

	public const Int32 SFX_BTL_SE500082 = 441;

	public const Int32 SFX_BTL_SE500084 = 516;

	public const Int32 SFX_BTL_SE500085 = 513;

	public const Int32 SFX_BTL_SE500086 = 510;

	public const Int32 SFX_BTL_SE500088 = 486;

	public const Int32 SFX_BTL_SE500089 = 480;

	public const Int32 SFX_BTL_SE500090 = 500;

	public const Int32 SFX_BTL_SE500092 = 440;

	public const Int32 SFX_BTL_SE500093 = 432;

	public const Int32 SFX_BTL_SE500094 = 479;

	public const Int32 SFX_BTL_SE500096 = 509;

	public const Int32 SFX_BTL_SE500097 = 506;

	public const Int32 SFX_BTL_SE500098 = 485;

	public const Int32 SFX_BTL_SE500100 = 503;

	public const Int32 SFX_BTL_SE500101 = 498;

	public const Int32 SFX_BTL_SE500102 = 495;

	public const Int32 SFX_BTL_SE500104 = 518;

	public const Int32 SFX_BTL_SE500105 = 514;

	public const Int32 SFX_BTL_SE500106 = 1048;

	public const Int32 SFX_BTL_SE500108 = 489;

	public const Int32 SFX_BTL_SE500109 = 482;

	public const Int32 SFX_BTL_SE500110 = 502;

	public const Int32 SFX_BTL_SE500112 = 494;

	public const Int32 SFX_BTL_SE500113 = 491;

	public const Int32 SFX_BTL_SE500114 = 517;

	public const Int32 SFX_BTL_SE500116 = 584;

	public const Int32 SFX_BTL_SE500117 = 575;

	public const Int32 SFX_BTL_SE500118 = 532;

	public const Int32 SFX_BTL_SE500120 = 571;

	public const Int32 SFX_BTL_SE500121 = 560;

	public const Int32 SFX_BTL_SE500122 = 554;

	public const Int32 SFX_BTL_SE500124 = 604;

	public const Int32 SFX_BTL_SE500125 = 592;

	public const Int32 SFX_BTL_SE500126 = 586;

	public const Int32 SFX_BTL_SE500128 = 534;

	public const Int32 SFX_BTL_SE500129 = 525;

	public const Int32 SFX_BTL_SE500130 = 570;

	public const Int32 SFX_BTL_SE500132 = 553;

	public const Int32 SFX_BTL_SE500133 = 542;

	public const Int32 SFX_BTL_SE500134 = 603;

	public const Int32 SFX_BTL_SE500136 = 585;

	public const Int32 SFX_BTL_SE500137 = 576;

	public const Int32 SFX_BTL_SE500138 = 533;

	public const Int32 SFX_BTL_SE500140 = 568;

	public const Int32 SFX_BTL_SE500141 = 558;

	public const Int32 SFX_BTL_SE500142 = 550;

	public const Int32 SFX_BTL_SE500144 = 601;

	public const Int32 SFX_BTL_SE500145 = 591;

	public const Int32 SFX_BTL_SE500146 = 583;

	public const Int32 SFX_BTL_SE500148 = 531;

	public const Int32 SFX_BTL_SE500149 = 524;

	public const Int32 SFX_BTL_SE500150 = 567;

	public const Int32 SFX_BTL_SE500152 = 549;

	public const Int32 SFX_BTL_SE500153 = 540;

	public const Int32 SFX_BTL_SE500154 = 600;

	public const Int32 SFX_BTL_SE500156 = 610;

	public const Int32 SFX_BTL_SE500157 = 609;

	public const Int32 SFX_BTL_SE500158 = 605;

	public const Int32 SFX_BTL_SE500160 = 569;

	public const Int32 SFX_BTL_SE500161 = 559;

	public const Int32 SFX_BTL_SE500162 = 552;

	public const Int32 SFX_BTL_SE500164 = 613;

	public const Int32 SFX_BTL_SE500165 = 612;

	public const Int32 SFX_BTL_SE500166 = 611;

	public const Int32 SFX_BTL_SE500168 = 488;

	public const Int32 SFX_BTL_SE500169 = 481;

	public const Int32 SFX_BTL_SE500170 = 501;

	public const Int32 SFX_BTL_SE500172 = 551;

	public const Int32 SFX_BTL_SE500173 = 541;

	public const Int32 SFX_BTL_SE500174 = 602;

	public const Int32 SFX_BTL_SE500176 = 511;

	public const Int32 SFX_BTL_SE500177 = 507;

	public const Int32 SFX_BTL_SE500178 = 487;

	public const Int32 SFX_BTL_SE500180 = 572;

	public const Int32 SFX_BTL_SE500181 = 561;

	public const Int32 SFX_BTL_SE500182 = 555;

	public const Int32 SFX_BTL_SE500184 = 520;

	public const Int32 SFX_BTL_SE500185 = 515;

	public const Int32 SFX_BTL_SE500186 = 512;

	public const Int32 SFX_BTL_SE500188 = 490;

	public const Int32 SFX_BTL_SE500189 = 483;

	public const Int32 SFX_BTL_SE500190 = 504;

	public const Int32 SFX_BTL_SE500192 = 496;

	public const Int32 SFX_BTL_SE500193 = 492;

	public const Int32 SFX_BTL_SE500194 = 519;

	public const Int32 SFX_BTL_SE500196 = 587;

	public const Int32 SFX_BTL_SE500197 = 577;

	public const Int32 SFX_BTL_SE500198 = 535;

	public const Int32 SFX_BTL_SE500200 = 499;

	public const Int32 SFX_BTL_SE500201 = 497;

	public const Int32 SFX_BTL_SE500202 = 493;

	public const Int32 SFX_BTL_SE500204 = 597;

	public const Int32 SFX_BTL_SE500205 = 589;

	public const Int32 SFX_BTL_SE500206 = 580;

	public const Int32 SFX_BTL_SE500208 = 528;

	public const Int32 SFX_BTL_SE500209 = 522;

	public const Int32 SFX_BTL_SE500210 = 564;

	public const Int32 SFX_BTL_SE500212 = 546;

	public const Int32 SFX_BTL_SE500213 = 538;

	public const Int32 SFX_BTL_SE500214 = 596;

	public const Int32 SFX_BTL_SE500216 = 508;

	public const Int32 SFX_BTL_SE500217 = 505;

	public const Int32 SFX_BTL_SE500218 = 484;

	public const Int32 SFX_BTL_SE500220 = 566;

	public const Int32 SFX_BTL_SE500221 = 557;

	public const Int32 SFX_BTL_SE500222 = 548;

	public const Int32 SFX_BTL_SE500224 = 599;

	public const Int32 SFX_BTL_SE500225 = 590;

	public const Int32 SFX_BTL_SE500226 = 582;

	public const Int32 SFX_BTL_SE500228 = 530;

	public const Int32 SFX_BTL_SE500229 = 523;

	public const Int32 SFX_BTL_SE500230 = 565;

	public const Int32 SFX_BTL_SE500232 = 547;

	public const Int32 SFX_BTL_SE500233 = 539;

	public const Int32 SFX_BTL_SE500234 = 598;

	public const Int32 SFX_BTL_SE500236 = 581;

	public const Int32 SFX_BTL_SE500237 = 574;

	public const Int32 SFX_BTL_SE500238 = 529;

	public const Int32 SFX_BTL_SE500240 = 563;

	public const Int32 SFX_BTL_SE500241 = 556;

	public const Int32 SFX_BTL_SE500242 = 544;

	public const Int32 SFX_BTL_SE500244 = 594;

	public const Int32 SFX_BTL_SE500245 = 588;

	public const Int32 SFX_BTL_SE500246 = 579;

	public const Int32 SFX_BTL_SE500248 = 527;

	public const Int32 SFX_BTL_SE500249 = 521;

	public const Int32 SFX_BTL_SE500250 = 562;

	public const Int32 SFX_BTL_SE500252 = 543;

	public const Int32 SFX_BTL_SE500253 = 536;

	public const Int32 SFX_BTL_SE500254 = 593;

	public const Int32 SFX_BTL_SE500256 = 578;

	public const Int32 SFX_BTL_SE500257 = 573;

	public const Int32 SFX_BTL_SE500258 = 526;

	public const Int32 SFX_BTL_SE500260 = 155;

	public const Int32 SFX_BTL_SE500261 = 66;

	public const Int32 SFX_BTL_SE500262 = 49;

	public const Int32 SFX_BTL_SE500264 = 198;

	public const Int32 SFX_BTL_SE500265 = 192;

	public const Int32 SFX_BTL_SE500266 = 1546;

	public const Int32 SFX_BTL_SE500268 = 26;

	public const Int32 SFX_BTL_SE500269 = 5;

	public const Int32 SFX_BTL_SE500270 = 86;

	public const Int32 SFX_BTL_SE500272 = 545;

	public const Int32 SFX_BTL_SE500273 = 537;

	public const Int32 SFX_BTL_SE500274 = 595;

	public const Int32 SFX_BTL_SE500276 = 710;

	public const Int32 SFX_BTL_SE500277 = 707;

	public const Int32 SFX_BTL_SE500278 = 686;

	public const Int32 SFX_BTL_SE500280 = 703;

	public const Int32 SFX_BTL_SE500281 = 699;

	public const Int32 SFX_BTL_SE500282 = 695;

	public const Int32 SFX_BTL_SE500284 = 720;

	public const Int32 SFX_BTL_SE500285 = 716;

	public const Int32 SFX_BTL_SE500286 = 712;

	public const Int32 SFX_BTL_SE500288 = 688;

	public const Int32 SFX_BTL_SE500289 = 684;

	public const Int32 SFX_BTL_SE500290 = 702;

	public const Int32 SFX_BTL_SE500292 = 694;

	public const Int32 SFX_BTL_SE500293 = 692;

	public const Int32 SFX_BTL_SE500294 = 719;

	public const Int32 SFX_BTL_SE500296 = 711;

	public const Int32 SFX_BTL_SE500297 = 708;

	public const Int32 SFX_BTL_SE500298 = 687;

	public const Int32 SFX_BTL_SE500300 = 705;

	public const Int32 SFX_BTL_SE500301 = 700;

	public const Int32 SFX_BTL_SE500302 = 697;

	public const Int32 SFX_BTL_SE500304 = 722;

	public const Int32 SFX_BTL_SE500305 = 717;

	public const Int32 SFX_BTL_SE500306 = 714;

	public const Int32 SFX_BTL_SE500308 = 690;

	public const Int32 SFX_BTL_SE500309 = 685;

	public const Int32 SFX_BTL_SE500310 = 704;

	public const Int32 SFX_BTL_SE500312 = 696;

	public const Int32 SFX_BTL_SE500313 = 693;

	public const Int32 SFX_BTL_SE500314 = 721;

	public const Int32 SFX_BTL_SE500316 = 713;

	public const Int32 SFX_BTL_SE500317 = 709;

	public const Int32 SFX_BTL_SE500318 = 689;

	public const Int32 SFX_BTL_SE500320 = 706;

	public const Int32 SFX_BTL_SE500321 = 701;

	public const Int32 SFX_BTL_SE500322 = 698;

	public const Int32 SFX_BTL_SE500324 = 723;

	public const Int32 SFX_BTL_SE500325 = 718;

	public const Int32 SFX_BTL_SE500326 = 715;

	public const Int32 SFX_BTL_SE500328 = 998;

	public const Int32 SFX_BTL_SE500329 = 999;

	public const Int32 SFX_BTL_SE500330 = 1000;

	public const Int32 SFX_BTL_SE500332 = 1001;

	public const Int32 SFX_BTL_SE500333 = 1002;

	public const Int32 SFX_BTL_SE500334 = 1003;

	public const Int32 SFX_BTL_SE500336 = 1004;

	public const Int32 SFX_BTL_SE500337 = 1005;

	public const Int32 SFX_BTL_SE500338 = 1006;

	public const Int32 SFX_BTL_SE500340 = 1007;

	public const Int32 SFX_BTL_SE500341 = 1008;

	public const Int32 SFX_BTL_SE500342 = 1009;

	public const Int32 SFX_BTL_SE500344 = 1010;

	public const Int32 SFX_BTL_SE500345 = 1011;

	public const Int32 SFX_BTL_SE500346 = 1012;

	public const Int32 SFX_BTL_SE500348 = 1013;

	public const Int32 SFX_BTL_SE500349 = 1014;

	public const Int32 SFX_BTL_SE500350 = 1015;

	public const Int32 SFX_BTL_SE500352 = 1016;

	public const Int32 SFX_BTL_SE500353 = 1017;

	public const Int32 SFX_BTL_SE500354 = 1018;

	public const Int32 SFX_BTL_SE500356 = 1019;

	public const Int32 SFX_BTL_SE500357 = 1020;

	public const Int32 SFX_BTL_SE500358 = 1021;

	public const Int32 SFX_BTL_SE500360 = 1022;

	public const Int32 SFX_BTL_SE500361 = 1023;

	public const Int32 SFX_BTL_SE500362 = 1024;

	public const Int32 SFX_BTL_SE500364 = 1025;

	public const Int32 SFX_BTL_SE500365 = 1026;

	public const Int32 SFX_BTL_SE500366 = 1027;

	public const Int32 SFX_BTL_SE500368 = 1028;

	public const Int32 SFX_BTL_SE500369 = 1029;

	public const Int32 SFX_BTL_SE500370 = 1030;

	public const Int32 SFX_BTL_SE500372 = 1031;

	public const Int32 SFX_BTL_SE500373 = 1032;

	public const Int32 SFX_BTL_SE500374 = 1033;

	public const Int32 SFX_BTL_SE500376 = 1034;

	public const Int32 SFX_BTL_SE500377 = 1035;

	public const Int32 SFX_BTL_SE500378 = 1036;

	public const Int32 SFX_BTL_SE500380 = 1037;

	public const Int32 SFX_BTL_SE500381 = 1038;

	public const Int32 SFX_BTL_SE500382 = 1039;

	public const Int32 SFX_BTL_SE500384 = 1040;

	public const Int32 SFX_BTL_SE500385 = 1041;

	public const Int32 SFX_BTL_SE500386 = 1042;

	public const Int32 SFX_BTL_SE500388 = 1107;

	public const Int32 SFX_BTL_SE500389 = 1108;

	public const Int32 SFX_BTL_SE500390 = 1109;

	public const Int32 SFX_BTL_SE500392 = 1113;

	public const Int32 SFX_BTL_SE500393 = 1114;

	public const Int32 SFX_BTL_SE500394 = 1115;

	public const Int32 SFX_BTL_SE500396 = 1116;

	public const Int32 SFX_BTL_SE500397 = 1117;

	public const Int32 SFX_BTL_SE500398 = 1118;

	public const Int32 SFX_BTL_SE500400 = 1119;

	public const Int32 SFX_BTL_SE500401 = 1120;

	public const Int32 SFX_BTL_SE500402 = 1121;

	public const Int32 SFX_BTL_SE500404 = 1128;

	public const Int32 SFX_BTL_SE500405 = 1129;

	public const Int32 SFX_BTL_SE500406 = 1130;

	public const Int32 SFX_BTL_SE500412 = 1122;

	public const Int32 SFX_BTL_SE500413 = 1123;

	public const Int32 SFX_BTL_SE500414 = 1124;

	public const Int32 SFX_BTL_SE500416 = 1257;

	public const Int32 SFX_BTL_SE500420 = 1125;

	public const Int32 SFX_BTL_SE500421 = 1126;

	public const Int32 SFX_BTL_SE500422 = 1127;

	public const Int32 SFX_BTL_SE500424 = 242;

	public const Int32 SFX_BTL_SE500425 = 253;

	public const Int32 SFX_BTL_SE500426 = 277;

	public const Int32 SFX_BTL_SE500428 = 2441;

	public const Int32 SFX_BTL_SE500429 = 2442;

	public const Int32 SFX_BTL_SE500430 = 2443;

	public const Int32 SFX_BTL_SE500432 = 2444;

	public const Int32 SFX_BTL_SE500433 = 2445;

	public const Int32 SFX_BTL_SE500434 = 2446;

	public const Int32 SFX_BTL_SE500436 = 2904;

	public const Int32 SFX_BTL_SE500437 = 2905;

	public const Int32 SFX_BTL_SE510000 = 926;

	public const Int32 SFX_BTL_SE510001 = 927;

	public const Int32 SFX_BTL_SE510002 = 928;

	public const Int32 SFX_BTL_SE510004 = 929;

	public const Int32 SFX_BTL_SE510005 = 930;

	public const Int32 SFX_BTL_SE510006 = 931;

	public const Int32 SFX_BTL_SE510008 = 932;

	public const Int32 SFX_BTL_SE510009 = 933;

	public const Int32 SFX_BTL_SE510010 = 934;

	public const Int32 SFX_BTL_SE510012 = 935;

	public const Int32 SFX_BTL_SE510013 = 936;

	public const Int32 SFX_BTL_SE510014 = 937;

	public const Int32 SFX_BTL_SE510016 = 938;

	public const Int32 SFX_BTL_SE510017 = 939;

	public const Int32 SFX_BTL_SE510018 = 940;

	public const Int32 SFX_BTL_SE510020 = 941;

	public const Int32 SFX_BTL_SE510021 = 942;

	public const Int32 SFX_BTL_SE510022 = 943;

	public const Int32 SFX_BTL_SE510024 = 944;

	public const Int32 SFX_BTL_SE510025 = 945;

	public const Int32 SFX_BTL_SE510026 = 946;

	public const Int32 SFX_BTL_SE510028 = 947;

	public const Int32 SFX_BTL_SE510029 = 948;

	public const Int32 SFX_BTL_SE510030 = 949;

	public const Int32 SFX_BTL_SE510032 = 950;

	public const Int32 SFX_BTL_SE510033 = 951;

	public const Int32 SFX_BTL_SE510034 = 952;

	public const Int32 SFX_BTL_SE510036 = 953;

	public const Int32 SFX_BTL_SE510037 = 954;

	public const Int32 SFX_BTL_SE510038 = 955;

	public const Int32 SFX_BTL_SE510040 = 956;

	public const Int32 SFX_BTL_SE510041 = 957;

	public const Int32 SFX_BTL_SE510042 = 958;

	public const Int32 SFX_BTL_SE510044 = 959;

	public const Int32 SFX_BTL_SE510045 = 960;

	public const Int32 SFX_BTL_SE510046 = 961;

	public const Int32 SFX_BTL_SE510048 = 962;

	public const Int32 SFX_BTL_SE510049 = 963;

	public const Int32 SFX_BTL_SE510050 = 964;

	public const Int32 SFX_BTL_SE510052 = 965;

	public const Int32 SFX_BTL_SE510053 = 966;

	public const Int32 SFX_BTL_SE510054 = 967;

	public const Int32 SFX_BTL_SE510056 = 968;

	public const Int32 SFX_BTL_SE510057 = 969;

	public const Int32 SFX_BTL_SE510058 = 970;

	public const Int32 SFX_BTL_SE510060 = 971;

	public const Int32 SFX_BTL_SE510061 = 972;

	public const Int32 SFX_BTL_SE510062 = 973;

	public const Int32 SFX_BTL_SE510064 = 974;

	public const Int32 SFX_BTL_SE510065 = 975;

	public const Int32 SFX_BTL_SE510066 = 976;

	public const Int32 SFX_BTL_SE510068 = 977;

	public const Int32 SFX_BTL_SE510069 = 978;

	public const Int32 SFX_BTL_SE510070 = 979;

	public const Int32 SFX_BTL_SE510072 = 980;

	public const Int32 SFX_BTL_SE510073 = 981;

	public const Int32 SFX_BTL_SE510074 = 982;

	public const Int32 SFX_BTL_SE510076 = 983;

	public const Int32 SFX_BTL_SE510077 = 984;

	public const Int32 SFX_BTL_SE510078 = 985;

	public const Int32 SFX_BTL_SE510080 = 986;

	public const Int32 SFX_BTL_SE510081 = 987;

	public const Int32 SFX_BTL_SE510082 = 988;

	public const Int32 SFX_BTL_SE510084 = 989;

	public const Int32 SFX_BTL_SE510085 = 990;

	public const Int32 SFX_BTL_SE510086 = 991;

	public const Int32 SFX_BTL_SE510088 = 992;

	public const Int32 SFX_BTL_SE510089 = 993;

	public const Int32 SFX_BTL_SE510090 = 994;

	public const Int32 SFX_BTL_SE510092 = 995;

	public const Int32 SFX_BTL_SE510093 = 996;

	public const Int32 SFX_BTL_SE510094 = 997;

	public const Int32 SFX_BTL_SE510096 = 1049;

	public const Int32 SFX_BTL_SE510097 = 1050;

	public const Int32 SFX_BTL_SE510098 = 1051;

	public const Int32 SFX_BTL_SE510100 = 1052;

	public const Int32 SFX_BTL_SE510101 = 1053;

	public const Int32 SFX_BTL_SE510102 = 1054;

	public const Int32 SFX_BTL_SE510104 = 1055;

	public const Int32 SFX_BTL_SE510105 = 1056;

	public const Int32 SFX_BTL_SE510106 = 1057;

	public const Int32 SFX_BTL_SE510108 = 1058;

	public const Int32 SFX_BTL_SE510109 = 1059;

	public const Int32 SFX_BTL_SE510110 = 1060;

	public const Int32 SFX_BTL_SE510112 = 1061;

	public const Int32 SFX_BTL_SE510116 = 1062;

	public const Int32 SFX_BTL_SE510117 = 1063;

	public const Int32 SFX_BTL_SE510118 = 1064;

	public const Int32 SFX_BTL_SE510120 = 1065;

	public const Int32 SFX_BTL_SE510121 = 1066;

	public const Int32 SFX_BTL_SE510122 = 1067;

	public const Int32 SFX_BTL_SE510124 = 1068;

	public const Int32 SFX_BTL_SE510125 = 1069;

	public const Int32 SFX_BTL_SE510126 = 1070;

	public const Int32 SFX_BTL_SE510128 = 1071;

	public const Int32 SFX_BTL_SE510129 = 1072;

	public const Int32 SFX_BTL_SE510130 = 1073;

	public const Int32 SFX_BTL_SE510132 = 1074;

	public const Int32 SFX_BTL_SE510133 = 1075;

	public const Int32 SFX_BTL_SE510134 = 1076;

	public const Int32 SFX_BTL_SE510136 = 1077;

	public const Int32 SFX_BTL_SE510137 = 1078;

	public const Int32 SFX_BTL_SE510138 = 1079;

	public const Int32 SFX_BTL_SE510140 = 1080;

	public const Int32 SFX_BTL_SE510141 = 1081;

	public const Int32 SFX_BTL_SE510142 = 1082;

	public const Int32 SFX_BTL_SE510144 = 1083;

	public const Int32 SFX_BTL_SE510145 = 1084;

	public const Int32 SFX_BTL_SE510146 = 1085;

	public const Int32 SFX_BTL_SE510148 = 1086;

	public const Int32 SFX_BTL_SE510149 = 1087;

	public const Int32 SFX_BTL_SE510150 = 1088;

	public const Int32 SFX_BTL_SE510152 = 1089;

	public const Int32 SFX_BTL_SE510153 = 1090;

	public const Int32 SFX_BTL_SE510154 = 1091;

	public const Int32 SFX_BTL_SE510156 = 1092;

	public const Int32 SFX_BTL_SE510157 = 1093;

	public const Int32 SFX_BTL_SE510158 = 1094;

	public const Int32 SFX_BTL_SE510160 = 1095;

	public const Int32 SFX_BTL_SE510161 = 1096;

	public const Int32 SFX_BTL_SE510162 = 1097;

	public const Int32 SFX_BTL_SE510164 = 1101;

	public const Int32 SFX_BTL_SE510165 = 1102;

	public const Int32 SFX_BTL_SE510166 = 1103;

	public const Int32 SFX_BTL_SE510168 = 1104;

	public const Int32 SFX_BTL_SE510169 = 1105;

	public const Int32 SFX_BTL_SE510170 = 1106;

	public const Int32 SFX_BTL_SE510172 = 1111;

	public const Int32 SFX_BTL_SE510173 = 1216;

	public const Int32 SFX_BTL_SE510174 = 1217;

	public const Int32 SFX_BTL_SE510175 = 1218;

	public const Int32 SFX_BTL_SE510176 = 1112;

	public const Int32 SFX_BTL_SE510177 = 1219;

	public const Int32 SFX_BTL_SE510178 = 1220;

	public const Int32 SFX_BTL_SE510179 = 1221;

	public const Int32 SFX_BTL_SE510180 = 1131;

	public const Int32 SFX_BTL_SE510181 = 1222;

	public const Int32 SFX_BTL_SE510182 = 1223;

	public const Int32 SFX_BTL_SE510183 = 1224;

	public const Int32 SFX_BTL_SE510184 = 1132;

	public const Int32 SFX_BTL_SE510185 = 1225;

	public const Int32 SFX_BTL_SE510186 = 1226;

	public const Int32 SFX_BTL_SE510187 = 1227;

	public const Int32 SFX_BTL_SE510188 = 1133;

	public const Int32 SFX_BTL_SE510189 = 1134;

	public const Int32 SFX_BTL_SE510190 = 1135;

	public const Int32 SFX_BTL_SE510192 = 1136;

	public const Int32 SFX_BTL_SE510193 = 1137;

	public const Int32 SFX_BTL_SE510194 = 1138;

	public const Int32 SFX_BTL_SE510196 = 1139;

	public const Int32 SFX_BTL_SE510197 = 1140;

	public const Int32 SFX_BTL_SE510200 = 1141;

	public const Int32 SFX_BTL_SE510201 = 1142;

	public const Int32 SFX_BTL_SE510202 = 1143;

	public const Int32 SFX_BTL_SE510204 = 1144;

	public const Int32 SFX_BTL_SE510205 = 1145;

	public const Int32 SFX_BTL_SE510206 = 1146;

	public const Int32 SFX_BTL_SE510208 = 1251;

	public const Int32 SFX_BTL_SE510209 = 1252;

	public const Int32 SFX_BTL_SE510210 = 1253;

	public const Int32 SFX_BTL_SE510212 = 1147;

	public const Int32 SFX_BTL_SE510213 = 1148;

	public const Int32 SFX_BTL_SE510214 = 1149;

	public const Int32 SFX_BTL_SE510216 = 1150;

	public const Int32 SFX_BTL_SE510217 = 1151;

	public const Int32 SFX_BTL_SE510218 = 1152;

	public const Int32 SFX_BTL_SE510220 = 1153;

	public const Int32 SFX_BTL_SE510221 = 1154;

	public const Int32 SFX_BTL_SE510222 = 1232;

	public const Int32 SFX_BTL_SE510224 = 1170;

	public const Int32 SFX_BTL_SE510225 = 1171;

	public const Int32 SFX_BTL_SE510226 = 1172;

	public const Int32 SFX_BTL_SE510228 = 1155;

	public const Int32 SFX_BTL_SE510229 = 1156;

	public const Int32 SFX_BTL_SE510230 = 1157;

	public const Int32 SFX_BTL_SE510232 = 1158;

	public const Int32 SFX_BTL_SE510233 = 1159;

	public const Int32 SFX_BTL_SE510234 = 1160;

	public const Int32 SFX_BTL_SE510236 = 1233;

	public const Int32 SFX_BTL_SE510237 = 1234;

	public const Int32 SFX_BTL_SE510238 = 1235;

	public const Int32 SFX_BTL_SE510240 = 1161;

	public const Int32 SFX_BTL_SE510241 = 1162;

	public const Int32 SFX_BTL_SE510242 = 1163;

	public const Int32 SFX_BTL_SE510244 = 1254;

	public const Int32 SFX_BTL_SE510245 = 1255;

	public const Int32 SFX_BTL_SE510246 = 1256;

	public const Int32 SFX_BTL_SE510248 = 1164;

	public const Int32 SFX_BTL_SE510249 = 1165;

	public const Int32 SFX_BTL_SE510250 = 1166;

	public const Int32 SFX_BTL_SE510252 = 1167;

	public const Int32 SFX_BTL_SE510253 = 1168;

	public const Int32 SFX_BTL_SE510254 = 1169;

	public const Int32 SFX_BTL_SE510256 = 2643;

	public const Int32 SFX_BTL_SE510257 = 2644;

	public const Int32 SFX_BTL_SE510258 = 2645;

	public const Int32 SFX_BTL_SE510260 = 1173;

	public const Int32 SFX_BTL_SE510261 = 1174;

	public const Int32 SFX_BTL_SE510262 = 1175;

	public const Int32 SFX_BTL_SE510264 = 1176;

	public const Int32 SFX_BTL_SE510265 = 1177;

	public const Int32 SFX_BTL_SE510266 = 1178;

	public const Int32 SFX_BTL_SE510268 = 1179;

	public const Int32 SFX_BTL_SE510269 = 1180;

	public const Int32 SFX_BTL_SE510270 = 1181;

	public const Int32 SFX_BTL_SE510272 = 1258;

	public const Int32 SFX_BTL_SE510273 = 1259;

	public const Int32 SFX_BTL_SE510274 = 1260;

	public const Int32 SFX_BTL_SE510280 = 1547;

	public const Int32 SFX_BTL_SE510281 = 1548;

	public const Int32 SFX_BTL_SE510282 = 1549;

	public const Int32 SFX_BTL_SE510284 = 1182;

	public const Int32 SFX_BTL_SE510285 = 1183;

	public const Int32 SFX_BTL_SE510286 = 1184;

	public const Int32 SFX_BTL_SE510288 = 1185;

	public const Int32 SFX_BTL_SE510289 = 1186;

	public const Int32 SFX_BTL_SE510290 = 1187;

	public const Int32 SFX_BTL_SE510292 = 1188;

	public const Int32 SFX_BTL_SE510293 = 1189;

	public const Int32 SFX_BTL_SE510294 = 1190;

	public const Int32 SFX_BTL_SE510296 = 1191;

	public const Int32 SFX_BTL_SE510297 = 1192;

	public const Int32 SFX_BTL_SE510298 = 1193;

	public const Int32 SFX_BTL_SE510300 = 1236;

	public const Int32 SFX_BTL_SE510301 = 1237;

	public const Int32 SFX_BTL_SE510302 = 1238;

	public const Int32 SFX_BTL_SE510304 = 1194;

	public const Int32 SFX_BTL_SE510305 = 1195;

	public const Int32 SFX_BTL_SE510306 = 1196;

	public const Int32 SFX_BTL_SE510308 = 1239;

	public const Int32 SFX_BTL_SE510309 = 1240;

	public const Int32 SFX_BTL_SE510310 = 1241;

	public const Int32 SFX_BTL_SE510312 = 1197;

	public const Int32 SFX_BTL_SE510313 = 1198;

	public const Int32 SFX_BTL_SE510314 = 1199;

	public const Int32 SFX_BTL_SE510316 = 1200;

	public const Int32 SFX_BTL_SE510317 = 1201;

	public const Int32 SFX_BTL_SE510318 = 1202;

	public const Int32 SFX_BTL_SE510320 = 1203;

	public const Int32 SFX_BTL_SE510321 = 1204;

	public const Int32 SFX_BTL_SE510322 = 1205;

	public const Int32 SFX_BTL_SE510324 = 1206;

	public const Int32 SFX_BTL_SE510325 = 1207;

	public const Int32 SFX_BTL_SE510326 = 1208;

	public const Int32 SFX_BTL_SE510328 = 1209;

	public const Int32 SFX_BTL_SE510329 = 1210;

	public const Int32 SFX_BTL_SE510330 = 1211;

	public const Int32 SFX_BTL_SE510336 = 1242;

	public const Int32 SFX_BTL_SE510337 = 1243;

	public const Int32 SFX_BTL_SE510338 = 1244;

	public const Int32 SFX_BTL_SE510340 = 1245;

	public const Int32 SFX_BTL_SE510341 = 1246;

	public const Int32 SFX_BTL_SE510342 = 1247;

	public const Int32 SFX_BTL_SE510344 = 1458;

	public const Int32 SFX_BTL_SE510345 = 1459;

	public const Int32 SFX_BTL_SE510346 = 1460;

	public const Int32 SFX_BTL_SE510348 = 1212;

	public const Int32 SFX_BTL_SE510349 = 1213;

	public const Int32 SFX_BTL_SE510350 = 1214;

	public const Int32 SFX_BTL_SE510352 = 1215;

	public const Int32 SFX_BTL_SE510353 = 1228;

	public const Int32 SFX_BTL_SE510354 = 1229;

	public const Int32 SFX_BTL_SE510356 = 1730;

	public const Int32 SFX_BTL_SE510357 = 1731;

	public const Int32 SFX_BTL_SE510358 = 1732;

	public const Int32 SFX_BTL_SE510360 = 1248;

	public const Int32 SFX_BTL_SE510361 = 1249;

	public const Int32 SFX_BTL_SE510362 = 1250;

	public const Int32 SFX_BTL_SE510364 = 2334;

	public const Int32 SFX_BTL_SE510365 = 2335;

	public const Int32 SFX_BTL_SE510366 = 2336;

	public const Int32 SFX_BTL_SE510368 = 2634;

	public const Int32 SFX_BTL_SE510369 = 2635;

	public const Int32 SFX_BTL_SE510370 = 2636;

	public const Int32 SFX_BTL_SE510372 = 2637;

	public const Int32 SFX_BTL_SE510373 = 2638;

	public const Int32 SFX_BTL_SE510374 = 2639;

	public const Int32 SFX_BTL_SE510376 = 2640;

	public const Int32 SFX_BTL_SE510377 = 2641;

	public const Int32 SFX_BTL_SE510378 = 2642;

	public const Int32 SFX_BTL_SE520000 = 1326;

	public const Int32 SFX_BTL_SE520001 = 1327;

	public const Int32 SFX_BTL_SE520004 = 1328;

	public const Int32 SFX_BTL_SE520005 = 1329;

	public const Int32 SFX_BTL_SE520006 = 1330;

	public const Int32 SFX_BTL_SE520008 = 1381;

	public const Int32 SFX_BTL_SE520009 = 1382;

	public const Int32 SFX_BTL_SE520010 = 1383;

	public const Int32 SFX_BTL_SE520012 = 1384;

	public const Int32 SFX_BTL_SE520013 = 1385;

	public const Int32 SFX_BTL_SE520014 = 1386;

	public const Int32 SFX_BTL_SE520016 = 1387;

	public const Int32 SFX_BTL_SE520017 = 1388;

	public const Int32 SFX_BTL_SE520018 = 1389;

	public const Int32 SFX_BTL_SE520020 = 1390;

	public const Int32 SFX_BTL_SE520021 = 1391;

	public const Int32 SFX_BTL_SE520022 = 1392;

	public const Int32 SFX_BTL_SE520024 = 1447;

	public const Int32 SFX_BTL_SE520025 = 1448;

	public const Int32 SFX_BTL_SE520026 = 1449;

	public const Int32 SFX_BTL_SE520028 = 1450;

	public const Int32 SFX_BTL_SE520029 = 1451;

	public const Int32 SFX_BTL_SE520030 = 1452;

	public const Int32 SFX_BTL_SE520032 = 1453;

	public const Int32 SFX_BTL_SE520033 = 1454;

	public const Int32 SFX_BTL_SE520034 = 1455;

	public const Int32 SFX_BTL_SE520036 = 1509;

	public const Int32 SFX_BTL_SE520037 = 1502;

	public const Int32 SFX_BTL_SE520038 = 1503;

	public const Int32 SFX_BTL_SE520040 = 1504;

	public const Int32 SFX_BTL_SE520041 = 1505;

	public const Int32 SFX_BTL_SE520042 = 1506;

	public const Int32 SFX_BTL_SE520044 = 1507;

	public const Int32 SFX_BTL_SE520045 = 1508;

	public const Int32 SFX_BTL_SE520046 = 1501;

	public const Int32 SFX_BTL_SE520048 = 1486;

	public const Int32 SFX_BTL_SE520049 = 1494;

	public const Int32 SFX_BTL_SE520050 = 1514;

	public const Int32 SFX_BTL_SE520052 = 1515;

	public const Int32 SFX_BTL_SE520053 = 1516;

	public const Int32 SFX_BTL_SE520054 = 1517;

	public const Int32 SFX_BTL_SE520056 = 1518;

	public const Int32 SFX_BTL_SE520057 = 1519;

	public const Int32 SFX_BTL_SE520058 = 1520;

	public const Int32 SFX_BTL_SE520060 = 1521;

	public const Int32 SFX_BTL_SE520061 = 1522;

	public const Int32 SFX_BTL_SE520062 = 1523;

	public const Int32 SFX_BTL_SE520064 = 1524;

	public const Int32 SFX_BTL_SE520065 = 1525;

	public const Int32 SFX_BTL_SE520066 = 1526;

	public const Int32 SFX_BTL_SE520068 = 1527;

	public const Int32 SFX_BTL_SE520069 = 1528;

	public const Int32 SFX_BTL_SE520070 = 1529;

	public const Int32 SFX_BTL_SE520072 = 1530;

	public const Int32 SFX_BTL_SE520073 = 1531;

	public const Int32 SFX_BTL_SE520074 = 1532;

	public const Int32 SFX_BTL_SE520076 = 1533;

	public const Int32 SFX_BTL_SE520077 = 1534;

	public const Int32 SFX_BTL_SE520078 = 1535;

	public const Int32 SFX_BTL_SE520080 = 1536;

	public const Int32 SFX_BTL_SE520081 = 1537;

	public const Int32 SFX_BTL_SE520082 = 1538;

	public const Int32 SFX_BTL_SE520084 = 1539;

	public const Int32 SFX_BTL_SE520085 = 1540;

	public const Int32 SFX_BTL_SE520086 = 1541;

	public const Int32 SFX_BTL_SE520087 = 2134;

	public const Int32 SFX_BTL_SE520088 = 1542;

	public const Int32 SFX_BTL_SE520089 = 1543;

	public const Int32 SFX_BTL_SE520090 = 1544;

	public const Int32 SFX_BTL_SE520091 = 2135;

	public const Int32 SFX_BTL_SE520092 = 1550;

	public const Int32 SFX_BTL_SE520093 = 1551;

	public const Int32 SFX_BTL_SE520094 = 1552;

	public const Int32 SFX_BTL_SE520096 = 1553;

	public const Int32 SFX_BTL_SE520097 = 1554;

	public const Int32 SFX_BTL_SE520098 = 1555;

	public const Int32 SFX_BTL_SE520100 = 1618;

	public const Int32 SFX_BTL_SE520101 = 1619;

	public const Int32 SFX_BTL_SE520102 = 1620;

	public const Int32 SFX_BTL_SE520104 = 1621;

	public const Int32 SFX_BTL_SE520105 = 1622;

	public const Int32 SFX_BTL_SE520106 = 1623;

	public const Int32 SFX_BTL_SE520108 = 1624;

	public const Int32 SFX_BTL_SE520109 = 1625;

	public const Int32 SFX_BTL_SE520110 = 1626;

	public const Int32 SFX_BTL_SE520112 = 1627;

	public const Int32 SFX_BTL_SE520113 = 1628;

	public const Int32 SFX_BTL_SE520114 = 1629;

	public const Int32 SFX_BTL_SE520116 = 1630;

	public const Int32 SFX_BTL_SE520117 = 1631;

	public const Int32 SFX_BTL_SE520118 = 1632;

	public const Int32 SFX_BTL_SE520120 = 1633;

	public const Int32 SFX_BTL_SE520121 = 1634;

	public const Int32 SFX_BTL_SE520122 = 1635;

	public const Int32 SFX_BTL_SE520124 = 1636;

	public const Int32 SFX_BTL_SE520125 = 1637;

	public const Int32 SFX_BTL_SE520126 = 1638;

	public const Int32 SFX_BTL_SE520128 = 1639;

	public const Int32 SFX_BTL_SE520129 = 1640;

	public const Int32 SFX_BTL_SE520130 = 1641;

	public const Int32 SFX_BTL_SE520132 = 1642;

	public const Int32 SFX_BTL_SE520133 = 1643;

	public const Int32 SFX_BTL_SE520134 = 1644;

	public const Int32 SFX_BTL_SE520136 = 1645;

	public const Int32 SFX_BTL_SE520137 = 1646;

	public const Int32 SFX_BTL_SE520138 = 1647;

	public const Int32 SFX_BTL_SE520140 = 1648;

	public const Int32 SFX_BTL_SE520141 = 1649;

	public const Int32 SFX_BTL_SE520142 = 1650;

	public const Int32 SFX_BTL_SE520144 = 1651;

	public const Int32 SFX_BTL_SE520145 = 1652;

	public const Int32 SFX_BTL_SE520146 = 1653;

	public const Int32 SFX_BTL_SE520148 = 1654;

	public const Int32 SFX_BTL_SE520149 = 1655;

	public const Int32 SFX_BTL_SE520150 = 1656;

	public const Int32 SFX_BTL_SE520152 = 1683;

	public const Int32 SFX_BTL_SE520153 = 1684;

	public const Int32 SFX_BTL_SE520154 = 1685;

	public const Int32 SFX_BTL_SE520156 = 1686;

	public const Int32 SFX_BTL_SE520157 = 1687;

	public const Int32 SFX_BTL_SE520158 = 1688;

	public const Int32 SFX_BTL_SE520160 = 1689;

	public const Int32 SFX_BTL_SE520161 = 1690;

	public const Int32 SFX_BTL_SE520162 = 1691;

	public const Int32 SFX_BTL_SE520164 = 1779;

	public const Int32 SFX_BTL_SE520165 = 1780;

	public const Int32 SFX_BTL_SE520166 = 1781;

	public const Int32 SFX_BTL_SE520168 = 1782;

	public const Int32 SFX_BTL_SE520169 = 1783;

	public const Int32 SFX_BTL_SE520170 = 1784;

	public const Int32 SFX_BTL_SE520172 = 1785;

	public const Int32 SFX_BTL_SE520173 = 1786;

	public const Int32 SFX_BTL_SE520174 = 1787;

	public const Int32 SFX_BTL_SE520176 = 1788;

	public const Int32 SFX_BTL_SE520177 = 1789;

	public const Int32 SFX_BTL_SE520178 = 1790;

	public const Int32 SFX_BTL_SE520180 = 1791;

	public const Int32 SFX_BTL_SE520181 = 1792;

	public const Int32 SFX_BTL_SE520182 = 1793;

	public const Int32 SFX_BTL_SE520184 = 1794;

	public const Int32 SFX_BTL_SE520185 = 1795;

	public const Int32 SFX_BTL_SE520186 = 1796;

	public const Int32 SFX_BTL_SE520188 = 1797;

	public const Int32 SFX_BTL_SE520189 = 1798;

	public const Int32 SFX_BTL_SE520190 = 1799;

	public const Int32 SFX_BTL_SE520192 = 1800;

	public const Int32 SFX_BTL_SE520193 = 1801;

	public const Int32 SFX_BTL_SE520194 = 1802;

	public const Int32 SFX_BTL_SE520196 = 1803;

	public const Int32 SFX_BTL_SE520197 = 1804;

	public const Int32 SFX_BTL_SE520198 = 1805;

	public const Int32 SFX_BTL_SE520200 = 1806;

	public const Int32 SFX_BTL_SE520201 = 1807;

	public const Int32 SFX_BTL_SE520202 = 1808;

	public const Int32 SFX_BTL_SE520204 = 1809;

	public const Int32 SFX_BTL_SE520205 = 1810;

	public const Int32 SFX_BTL_SE520206 = 1811;

	public const Int32 SFX_BTL_SE520208 = 1812;

	public const Int32 SFX_BTL_SE520209 = 1813;

	public const Int32 SFX_BTL_SE520210 = 1814;

	public const Int32 SFX_BTL_SE520212 = 1815;

	public const Int32 SFX_BTL_SE520213 = 1816;

	public const Int32 SFX_BTL_SE520216 = 1817;

	public const Int32 SFX_BTL_SE520217 = 1818;

	public const Int32 SFX_BTL_SE520218 = 1819;

	public const Int32 SFX_BTL_SE520220 = 1820;

	public const Int32 SFX_BTL_SE520221 = 1821;

	public const Int32 SFX_BTL_SE520222 = 1822;

	public const Int32 SFX_BTL_SE520224 = 1823;

	public const Int32 SFX_BTL_SE520225 = 1824;

	public const Int32 SFX_BTL_SE520226 = 1825;

	public const Int32 SFX_BTL_SE520228 = 1826;

	public const Int32 SFX_BTL_SE520229 = 1827;

	public const Int32 SFX_BTL_SE520230 = 1828;

	public const Int32 SFX_BTL_SE520232 = 1829;

	public const Int32 SFX_BTL_SE520233 = 1830;

	public const Int32 SFX_BTL_SE520234 = 1831;

	public const Int32 SFX_BTL_SE520236 = 1832;

	public const Int32 SFX_BTL_SE520237 = 1833;

	public const Int32 SFX_BTL_SE520238 = 1834;

	public const Int32 SFX_BTL_SE520240 = 1835;

	public const Int32 SFX_BTL_SE520241 = 1836;

	public const Int32 SFX_BTL_SE520244 = 1837;

	public const Int32 SFX_BTL_SE520245 = 1838;

	public const Int32 SFX_BTL_SE520246 = 1839;

	public const Int32 SFX_BTL_SE520248 = 1840;

	public const Int32 SFX_BTL_SE520249 = 1841;

	public const Int32 SFX_BTL_SE520250 = 1842;

	public const Int32 SFX_BTL_SE520252 = 1843;

	public const Int32 SFX_BTL_SE520253 = 1844;

	public const Int32 SFX_BTL_SE520254 = 1845;

	public const Int32 SFX_BTL_SE520256 = 1846;

	public const Int32 SFX_BTL_SE520257 = 1847;

	public const Int32 SFX_BTL_SE520258 = 1848;

	public const Int32 SFX_BTL_SE520260 = 1849;

	public const Int32 SFX_BTL_SE520261 = 1850;

	public const Int32 SFX_BTL_SE520262 = 1851;

	public const Int32 SFX_BTL_SE520264 = 1891;

	public const Int32 SFX_BTL_SE520265 = 1892;

	public const Int32 SFX_BTL_SE520266 = 1893;

	public const Int32 SFX_BTL_SE520268 = 1894;

	public const Int32 SFX_BTL_SE520269 = 1895;

	public const Int32 SFX_BTL_SE520270 = 1896;

	public const Int32 SFX_BTL_SE520272 = 1897;

	public const Int32 SFX_BTL_SE520273 = 1898;

	public const Int32 SFX_BTL_SE520274 = 1899;

	public const Int32 SFX_BTL_SE520276 = 1900;

	public const Int32 SFX_BTL_SE520277 = 1901;

	public const Int32 SFX_BTL_SE520278 = 1902;

	public const Int32 SFX_BTL_SE520280 = 1903;

	public const Int32 SFX_BTL_SE520281 = 1904;

	public const Int32 SFX_BTL_SE520282 = 1905;

	public const Int32 SFX_BTL_SE520284 = 1906;

	public const Int32 SFX_BTL_SE520285 = 1907;

	public const Int32 SFX_BTL_SE520286 = 1908;

	public const Int32 SFX_BTL_SE520288 = 1909;

	public const Int32 SFX_BTL_SE520289 = 1910;

	public const Int32 SFX_BTL_SE520290 = 1911;

	public const Int32 SFX_BTL_SE520292 = 1912;

	public const Int32 SFX_BTL_SE520293 = 1913;

	public const Int32 SFX_BTL_SE520294 = 1914;

	public const Int32 SFX_BTL_SE520296 = 1915;

	public const Int32 SFX_BTL_SE520297 = 1916;

	public const Int32 SFX_BTL_SE520298 = 1917;

	public const Int32 SFX_BTL_SE520300 = 1918;

	public const Int32 SFX_BTL_SE520301 = 1919;

	public const Int32 SFX_BTL_SE520302 = 1920;

	public const Int32 SFX_BTL_SE520304 = 1921;

	public const Int32 SFX_BTL_SE520305 = 1922;

	public const Int32 SFX_BTL_SE520306 = 1923;

	public const Int32 SFX_BTL_SE520308 = 1924;

	public const Int32 SFX_BTL_SE520309 = 1925;

	public const Int32 SFX_BTL_SE520310 = 1926;

	public const Int32 SFX_BTL_SE520312 = 1927;

	public const Int32 SFX_BTL_SE520313 = 1928;

	public const Int32 SFX_BTL_SE520314 = 1929;

	public const Int32 SFX_BTL_SE520316 = 1930;

	public const Int32 SFX_BTL_SE520317 = 1931;

	public const Int32 SFX_BTL_SE520318 = 1932;

	public const Int32 SFX_BTL_SE520320 = 1933;

	public const Int32 SFX_BTL_SE520321 = 1934;

	public const Int32 SFX_BTL_SE520322 = 1935;

	public const Int32 SFX_BTL_SE520324 = 1936;

	public const Int32 SFX_BTL_SE520325 = 1937;

	public const Int32 SFX_BTL_SE520326 = 1938;

	public const Int32 SFX_BTL_SE520328 = 1939;

	public const Int32 SFX_BTL_SE520329 = 1940;

	public const Int32 SFX_BTL_SE520330 = 1941;

	public const Int32 SFX_BTL_SE530000 = 2005;

	public const Int32 SFX_BTL_SE530001 = 2006;

	public const Int32 SFX_BTL_SE530002 = 2007;

	public const Int32 SFX_BTL_SE530004 = 2008;

	public const Int32 SFX_BTL_SE530005 = 2009;

	public const Int32 SFX_BTL_SE530006 = 2010;

	public const Int32 SFX_BTL_SE530008 = 2011;

	public const Int32 SFX_BTL_SE530009 = 2012;

	public const Int32 SFX_BTL_SE530010 = 2013;

	public const Int32 SFX_BTL_SE530012 = 2014;

	public const Int32 SFX_BTL_SE530013 = 2015;

	public const Int32 SFX_BTL_SE530014 = 2016;

	public const Int32 SFX_BTL_SE530016 = 2017;

	public const Int32 SFX_BTL_SE530017 = 2018;

	public const Int32 SFX_BTL_SE530018 = 2019;

	public const Int32 SFX_BTL_SE530020 = 2020;

	public const Int32 SFX_BTL_SE530021 = 2021;

	public const Int32 SFX_BTL_SE530022 = 2022;

	public const Int32 SFX_BTL_SE530024 = 2023;

	public const Int32 SFX_BTL_SE530025 = 2024;

	public const Int32 SFX_BTL_SE530026 = 2025;

	public const Int32 SFX_BTL_SE530028 = 2026;

	public const Int32 SFX_BTL_SE530029 = 2027;

	public const Int32 SFX_BTL_SE530030 = 2028;

	public const Int32 SFX_BTL_SE530032 = 2029;

	public const Int32 SFX_BTL_SE530033 = 2030;

	public const Int32 SFX_BTL_SE530034 = 2031;

	public const Int32 SFX_BTL_SE530036 = 2032;

	public const Int32 SFX_BTL_SE530037 = 2033;

	public const Int32 SFX_BTL_SE530040 = 2034;

	public const Int32 SFX_BTL_SE530041 = 2035;

	public const Int32 SFX_BTL_SE530044 = 2036;

	public const Int32 SFX_BTL_SE530045 = 2037;

	public const Int32 SFX_BTL_SE530046 = 2038;

	public const Int32 SFX_BTL_SE530048 = 2039;

	public const Int32 SFX_BTL_SE530049 = 2040;

	public const Int32 SFX_BTL_SE530050 = 2041;

	public const Int32 SFX_BTL_SE530052 = 2042;

	public const Int32 SFX_BTL_SE530053 = 2043;

	public const Int32 SFX_BTL_SE530054 = 2044;

	public const Int32 SFX_BTL_SE530056 = 2045;

	public const Int32 SFX_BTL_SE530057 = 2046;

	public const Int32 SFX_BTL_SE530058 = 2047;

	public const Int32 SFX_BTL_SE530060 = 2048;

	public const Int32 SFX_BTL_SE530061 = 2049;

	public const Int32 SFX_BTL_SE530062 = 2050;

	public const Int32 SFX_BTL_SE530064 = 2051;

	public const Int32 SFX_BTL_SE530065 = 2052;

	public const Int32 SFX_BTL_SE530068 = 2053;

	public const Int32 SFX_BTL_SE530069 = 2054;

	public const Int32 SFX_BTL_SE530070 = 2055;

	public const Int32 SFX_BTL_SE530072 = 2056;

	public const Int32 SFX_BTL_SE530073 = 2057;

	public const Int32 SFX_BTL_SE530074 = 2058;

	public const Int32 SFX_BTL_SE530076 = 2065;

	public const Int32 SFX_BTL_SE530077 = 2066;

	public const Int32 SFX_BTL_SE530078 = 2067;

	public const Int32 SFX_BTL_SE530080 = 2068;

	public const Int32 SFX_BTL_SE530081 = 2069;

	public const Int32 SFX_BTL_SE530082 = 2070;

	public const Int32 SFX_BTL_SE530084 = 2071;

	public const Int32 SFX_BTL_SE530085 = 2072;

	public const Int32 SFX_BTL_SE530086 = 2073;

	public const Int32 SFX_BTL_SE530088 = 2074;

	public const Int32 SFX_BTL_SE530089 = 2075;

	public const Int32 SFX_BTL_SE530090 = 2076;

	public const Int32 SFX_BTL_SE530092 = 2077;

	public const Int32 SFX_BTL_SE530093 = 2078;

	public const Int32 SFX_BTL_SE530094 = 2079;

	public const Int32 SFX_BTL_SE530096 = 2080;

	public const Int32 SFX_BTL_SE530097 = 2081;

	public const Int32 SFX_BTL_SE530098 = 2082;

	public const Int32 SFX_BTL_SE530100 = 2102;

	public const Int32 SFX_BTL_SE530101 = 2103;

	public const Int32 SFX_BTL_SE530104 = 2104;

	public const Int32 SFX_BTL_SE530105 = 2105;

	public const Int32 SFX_BTL_SE530108 = 2106;

	public const Int32 SFX_BTL_SE530109 = 2107;

	public const Int32 SFX_BTL_SE530110 = 2108;

	public const Int32 SFX_BTL_SE530112 = 2136;

	public const Int32 SFX_BTL_SE530113 = 2137;

	public const Int32 SFX_BTL_SE530114 = 2138;

	public const Int32 SFX_BTL_SE530115 = 2139;

	public const Int32 SFX_BTL_SE530116 = 2140;

	public const Int32 SFX_BTL_SE530117 = 2141;

	public const Int32 SFX_BTL_SE530118 = 2142;

	public const Int32 SFX_BTL_SE530119 = 2143;

	public const Int32 SFX_BTL_SE530120 = 2144;

	public const Int32 SFX_BTL_SE530121 = 2145;

	public const Int32 SFX_BTL_SE530122 = 2146;

	public const Int32 SFX_BTL_SE530123 = 2147;

	public const Int32 SFX_BTL_SE530124 = 2148;

	public const Int32 SFX_BTL_SE530125 = 2149;

	public const Int32 SFX_BTL_SE530126 = 2150;

	public const Int32 SFX_BTL_SE530127 = 2151;

	public const Int32 SFX_BTL_SE530128 = 2152;

	public const Int32 SFX_BTL_SE530129 = 2153;

	public const Int32 SFX_BTL_SE530130 = 2154;

	public const Int32 SFX_BTL_SE530131 = 2155;

	public const Int32 SFX_BTL_SE530132 = 2156;

	public const Int32 SFX_BTL_SE530133 = 2157;

	public const Int32 SFX_BTL_SE530136 = 2158;

	public const Int32 SFX_BTL_SE530137 = 2159;

	public const Int32 SFX_BTL_SE530138 = 2160;

	public const Int32 SFX_BTL_SE530139 = 2161;

	public const Int32 SFX_BTL_SE530140 = 2162;

	public const Int32 SFX_BTL_SE530141 = 2163;

	public const Int32 SFX_BTL_SE530142 = 2164;

	public const Int32 SFX_BTL_SE530143 = 2165;

	public const Int32 SFX_BTL_SE530144 = 2166;

	public const Int32 SFX_BTL_SE530145 = 2167;

	public const Int32 SFX_BTL_SE530146 = 2168;

	public const Int32 SFX_BTL_SE530147 = 2169;

	public const Int32 SFX_BTL_SE530148 = 2170;

	public const Int32 SFX_BTL_SE530149 = 2171;

	public const Int32 SFX_BTL_SE530150 = 2172;

	public const Int32 SFX_BTL_SE530151 = 2173;

	public const Int32 SFX_BTL_SE530152 = 2174;

	public const Int32 SFX_BTL_SE530153 = 2175;

	public const Int32 SFX_BTL_SE530154 = 2337;

	public const Int32 SFX_BTL_SE530155 = 2338;

	public const Int32 SFX_BTL_SE530156 = 2207;

	public const Int32 SFX_BTL_SE530157 = 2208;

	public const Int32 SFX_BTL_SE530158 = 2209;

	public const Int32 SFX_BTL_SE530160 = 2210;

	public const Int32 SFX_BTL_SE530161 = 2211;

	public const Int32 SFX_BTL_SE530162 = 2212;

	public const Int32 SFX_BTL_SE530164 = 2213;

	public const Int32 SFX_BTL_SE530165 = 2214;

	public const Int32 SFX_BTL_SE530166 = 2215;

	public const Int32 SFX_BTL_SE530168 = 2216;

	public const Int32 SFX_BTL_SE530169 = 2217;

	public const Int32 SFX_BTL_SE530170 = 2218;

	public const Int32 SFX_BTL_SE530172 = 2219;

	public const Int32 SFX_BTL_SE530173 = 2220;

	public const Int32 SFX_BTL_SE530174 = 2221;

	public const Int32 SFX_BTL_SE530176 = 2222;

	public const Int32 SFX_BTL_SE530177 = 2223;

	public const Int32 SFX_BTL_SE530178 = 2224;

	public const Int32 SFX_BTL_SE530180 = 2225;

	public const Int32 SFX_BTL_SE530181 = 2226;

	public const Int32 SFX_BTL_SE530182 = 2227;

	public const Int32 SFX_BTL_SE530184 = 2228;

	public const Int32 SFX_BTL_SE530185 = 2229;

	public const Int32 SFX_BTL_SE530186 = 2230;

	public const Int32 SFX_BTL_SE530188 = 2231;

	public const Int32 SFX_BTL_SE530189 = 2232;

	public const Int32 SFX_BTL_SE530190 = 2233;

	public const Int32 SFX_BTL_SE530192 = 2234;

	public const Int32 SFX_BTL_SE530193 = 2235;

	public const Int32 SFX_BTL_SE530194 = 2236;

	public const Int32 SFX_BTL_SE530196 = 2237;

	public const Int32 SFX_BTL_SE530197 = 2238;

	public const Int32 SFX_BTL_SE530198 = 2239;

	public const Int32 SFX_BTL_SE530200 = 2240;

	public const Int32 SFX_BTL_SE530201 = 2241;

	public const Int32 SFX_BTL_SE530202 = 2242;

	public const Int32 SFX_BTL_SE530204 = 2243;

	public const Int32 SFX_BTL_SE530205 = 2244;

	public const Int32 SFX_BTL_SE530206 = 2245;

	public const Int32 SFX_BTL_SE530208 = 2246;

	public const Int32 SFX_BTL_SE530209 = 2247;

	public const Int32 SFX_BTL_SE530210 = 2248;

	public const Int32 SFX_BTL_SE530212 = 2249;

	public const Int32 SFX_BTL_SE530213 = 2250;

	public const Int32 SFX_BTL_SE530214 = 2251;

	public const Int32 SFX_BTL_SE530216 = 2252;

	public const Int32 SFX_BTL_SE530217 = 2253;

	public const Int32 SFX_BTL_SE530218 = 2254;

	public const Int32 SFX_BTL_SE530220 = 2255;

	public const Int32 SFX_BTL_SE530221 = 2256;

	public const Int32 SFX_BTL_SE530222 = 2257;

	public const Int32 SFX_BTL_SE530223 = 2258;

	public const Int32 SFX_BTL_SE530224 = 2259;

	public const Int32 SFX_BTL_SE530225 = 2260;

	public const Int32 SFX_BTL_SE530226 = 2261;

	public const Int32 SFX_BTL_SE530227 = 2262;

	public const Int32 SFX_BTL_SE530228 = 2263;

	public const Int32 SFX_BTL_SE530229 = 2264;

	public const Int32 SFX_BTL_SE530230 = 2265;

	public const Int32 SFX_BTL_SE530231 = 2266;

	public const Int32 SFX_BTL_SE530232 = 2267;

	public const Int32 SFX_BTL_SE530233 = 2268;

	public const Int32 SFX_BTL_SE530234 = 2269;

	public const Int32 SFX_BTL_SE530235 = 2270;

	public const Int32 SFX_BTL_SE530236 = 2271;

	public const Int32 SFX_BTL_SE530237 = 2272;

	public const Int32 SFX_BTL_SE530238 = 2273;

	public const Int32 SFX_BTL_SE530239 = 2274;

	public const Int32 SFX_BTL_SE530240 = 2275;

	public const Int32 SFX_BTL_SE530241 = 2276;

	public const Int32 SFX_BTL_SE530242 = 2277;

	public const Int32 SFX_BTL_SE530243 = 2278;

	public const Int32 SFX_BTL_SE530244 = 2312;

	public const Int32 SFX_BTL_SE530245 = 2313;

	public const Int32 SFX_BTL_SE530246 = 2314;

	public const Int32 SFX_BTL_SE530248 = 2315;

	public const Int32 SFX_BTL_SE530249 = 2316;

	public const Int32 SFX_BTL_SE530250 = 2317;

	public const Int32 SFX_BTL_SE530252 = 2318;

	public const Int32 SFX_BTL_SE530253 = 2319;

	public const Int32 SFX_BTL_SE530254 = 2320;

	public const Int32 SFX_BTL_SE530256 = 2321;

	public const Int32 SFX_BTL_SE530257 = 2322;

	public const Int32 SFX_BTL_SE530258 = 2323;

	public const Int32 SFX_BTL_SE530260 = 2324;

	public const Int32 SFX_BTL_SE530261 = 2325;

	public const Int32 SFX_BTL_SE530262 = 2326;

	public const Int32 SFX_BTL_SE530264 = 2327;

	public const Int32 SFX_BTL_SE530265 = 2328;

	public const Int32 SFX_BTL_SE530266 = 2329;

	public const Int32 SFX_BTL_SE530268 = 2339;

	public const Int32 SFX_BTL_SE530269 = 2340;

	public const Int32 SFX_BTL_SE530270 = 2341;

	public const Int32 SFX_BTL_SE530272 = 2344;

	public const Int32 SFX_BTL_SE530273 = 2345;

	public const Int32 SFX_BTL_SE530274 = 2346;

	public const Int32 SFX_BTL_SE530276 = 2347;

	public const Int32 SFX_BTL_SE530277 = 2348;

	public const Int32 SFX_BTL_SE530278 = 2349;

	public const Int32 SFX_BTL_SE530280 = 2350;

	public const Int32 SFX_BTL_SE530281 = 2351;

	public const Int32 SFX_BTL_SE530282 = 2352;

	public const Int32 SFX_BTL_SE530284 = 2353;

	public const Int32 SFX_BTL_SE530285 = 2354;

	public const Int32 SFX_BTL_SE530286 = 2355;

	public const Int32 SFX_BTL_SE530288 = 2356;

	public const Int32 SFX_BTL_SE530289 = 2357;

	public const Int32 SFX_BTL_SE530290 = 2358;

	public const Int32 SFX_BTL_SE530292 = 2359;

	public const Int32 SFX_BTL_SE530293 = 2360;

	public const Int32 SFX_BTL_SE530294 = 2361;

	public const Int32 SFX_BTL_SE530296 = 2362;

	public const Int32 SFX_BTL_SE530297 = 2363;

	public const Int32 SFX_BTL_SE530298 = 2364;

	public const Int32 SFX_BTL_SE530300 = 2365;

	public const Int32 SFX_BTL_SE530301 = 2366;

	public const Int32 SFX_BTL_SE530302 = 2367;

	public const Int32 SFX_BTL_SE530304 = 2368;

	public const Int32 SFX_BTL_SE530305 = 2369;

	public const Int32 SFX_BTL_SE530306 = 2370;

	public const Int32 SFX_BTL_SE530308 = 2371;

	public const Int32 SFX_BTL_SE530309 = 2372;

	public const Int32 SFX_BTL_SE530310 = 2373;

	public const Int32 SFX_BTL_SE530312 = 2374;

	public const Int32 SFX_BTL_SE530313 = 2375;

	public const Int32 SFX_BTL_SE530314 = 2376;

	public const Int32 SFX_BTL_SE530316 = 2377;

	public const Int32 SFX_BTL_SE530317 = 2378;

	public const Int32 SFX_BTL_SE530318 = 2379;

	public const Int32 SFX_BTL_SE540000 = 2387;

	public const Int32 SFX_BTL_SE540001 = 2388;

	public const Int32 SFX_BTL_SE540002 = 2389;

	public const Int32 SFX_BTL_SE540003 = 2390;

	public const Int32 SFX_BTL_SE540004 = 2391;

	public const Int32 SFX_BTL_SE540005 = 2392;

	public const Int32 SFX_BTL_SE540006 = 2393;

	public const Int32 SFX_BTL_SE540007 = 2394;

	public const Int32 SFX_BTL_SE540008 = 2395;

	public const Int32 SFX_BTL_SE540009 = 2396;

	public const Int32 SFX_BTL_SE540010 = 2397;

	public const Int32 SFX_BTL_SE540011 = 2398;

	public const Int32 SFX_BTL_SE540012 = 2399;

	public const Int32 SFX_BTL_SE540013 = 2400;

	public const Int32 SFX_BTL_SE540014 = 2401;

	public const Int32 SFX_BTL_SE540015 = 2402;

	public const Int32 SFX_BTL_SE540016 = 2403;

	public const Int32 SFX_BTL_SE540017 = 2404;

	public const Int32 SFX_BTL_SE540018 = 2405;

	public const Int32 SFX_BTL_SE540019 = 2406;

	public const Int32 SFX_BTL_SE540020 = 2407;

	public const Int32 SFX_BTL_SE540021 = 2408;

	public const Int32 SFX_BTL_SE540022 = 2409;

	public const Int32 SFX_BTL_SE540023 = 2410;

	public const Int32 SFX_BTL_SE540024 = 2411;

	public const Int32 SFX_BTL_SE540025 = 2412;

	public const Int32 SFX_BTL_SE540026 = 2413;

	public const Int32 SFX_BTL_SE540027 = 2414;

	public const Int32 SFX_BTL_SE540028 = 2415;

	public const Int32 SFX_BTL_SE540029 = 2416;

	public const Int32 SFX_BTL_SE540030 = 2417;

	public const Int32 SFX_BTL_SE540031 = 2418;

	public const Int32 SFX_BTL_SE540032 = 2419;

	public const Int32 SFX_BTL_SE540033 = 2420;

	public const Int32 SFX_BTL_SE540034 = 2421;

	public const Int32 SFX_BTL_SE540035 = 2422;

	public const Int32 SFX_BTL_SE540036 = 2423;

	public const Int32 SFX_BTL_SE540037 = 2424;

	public const Int32 SFX_BTL_SE540038 = 2425;

	public const Int32 SFX_BTL_SE540039 = 2426;

	public const Int32 SFX_BTL_SE540040 = 2427;

	public const Int32 SFX_BTL_SE540041 = 2428;

	public const Int32 SFX_BTL_SE540042 = 2429;

	public const Int32 SFX_BTL_SE540044 = 2430;

	public const Int32 SFX_BTL_SE540045 = 2431;

	public const Int32 SFX_BTL_SE540046 = 2432;

	public const Int32 SFX_BTL_SE540048 = 2433;

	public const Int32 SFX_BTL_SE540049 = 2434;

	public const Int32 SFX_BTL_SE540052 = 2447;

	public const Int32 SFX_BTL_SE540053 = 2448;

	public const Int32 SFX_BTL_SE540054 = 2449;

	public const Int32 SFX_BTL_SE540056 = 2435;

	public const Int32 SFX_BTL_SE540057 = 2436;

	public const Int32 SFX_BTL_SE540058 = 2437;

	public const Int32 SFX_BTL_SE540060 = 2438;

	public const Int32 SFX_BTL_SE540061 = 2439;

	public const Int32 SFX_BTL_SE540062 = 2440;

	public const Int32 SFX_BTL_SE540064 = 2450;

	public const Int32 SFX_BTL_SE540065 = 2451;

	public const Int32 SFX_BTL_SE540066 = 2452;

	public const Int32 SFX_BTL_SE540068 = 2453;

	public const Int32 SFX_BTL_SE540069 = 2454;

	public const Int32 SFX_BTL_SE540070 = 2455;

	public const Int32 SFX_BTL_SE540072 = 2456;

	public const Int32 SFX_BTL_SE540073 = 2457;

	public const Int32 SFX_BTL_SE540076 = 2458;

	public const Int32 SFX_BTL_SE540077 = 2459;

	public const Int32 SFX_BTL_SE540080 = 2460;

	public const Int32 SFX_BTL_SE540081 = 2461;

	public const Int32 SFX_BTL_SE540082 = 2462;

	public const Int32 SFX_BTL_SE540084 = 2463;

	public const Int32 SFX_BTL_SE540085 = 2464;

	public const Int32 SFX_BTL_SE540086 = 2465;

	public const Int32 SFX_BTL_SE540088 = 2466;

	public const Int32 SFX_BTL_SE540089 = 2467;

	public const Int32 SFX_BTL_SE540090 = 2468;

	public const Int32 SFX_BTL_SE540092 = 2469;

	public const Int32 SFX_BTL_SE540093 = 2470;

	public const Int32 SFX_BTL_SE540094 = 2471;

	public const Int32 SFX_BTL_SE540096 = 2472;

	public const Int32 SFX_BTL_SE540097 = 2473;

	public const Int32 SFX_BTL_SE540098 = 2474;

	public const Int32 SFX_BTL_SE540100 = 2475;

	public const Int32 SFX_BTL_SE540101 = 2476;

	public const Int32 SFX_BTL_SE540102 = 2477;

	public const Int32 SFX_BTL_SE540104 = 2478;

	public const Int32 SFX_BTL_SE540105 = 2479;

	public const Int32 SFX_BTL_SE540106 = 2480;

	public const Int32 SFX_BTL_SE540108 = 2481;

	public const Int32 SFX_BTL_SE540109 = 2482;

	public const Int32 SFX_BTL_SE540110 = 2483;

	public const Int32 SFX_BTL_SE540112 = 2502;

	public const Int32 SFX_BTL_SE540113 = 2503;

	public const Int32 SFX_BTL_SE540114 = 2504;

	public const Int32 SFX_BTL_SE540115 = 2505;

	public const Int32 SFX_BTL_SE540116 = 2511;

	public const Int32 SFX_BTL_SE540117 = 2512;

	public const Int32 SFX_BTL_SE540118 = 2513;

	public const Int32 SFX_BTL_SE540120 = 2514;

	public const Int32 SFX_BTL_SE540121 = 2515;

	public const Int32 SFX_BTL_SE540122 = 2516;

	public const Int32 SFX_BTL_SE540124 = 2517;

	public const Int32 SFX_BTL_SE540125 = 2518;

	public const Int32 SFX_BTL_SE540126 = 2519;

	public const Int32 SFX_BTL_SE540128 = 2484;

	public const Int32 SFX_BTL_SE540129 = 2485;

	public const Int32 SFX_BTL_SE540130 = 2486;

	public const Int32 SFX_BTL_SE540132 = 2487;

	public const Int32 SFX_BTL_SE540133 = 2488;

	public const Int32 SFX_BTL_SE540134 = 2489;

	public const Int32 SFX_BTL_SE540136 = 2520;

	public const Int32 SFX_BTL_SE540137 = 2521;

	public const Int32 SFX_BTL_SE540138 = 2522;

	public const Int32 SFX_BTL_SE540140 = 2523;

	public const Int32 SFX_BTL_SE540141 = 2524;

	public const Int32 SFX_BTL_SE540142 = 2525;

	public const Int32 SFX_BTL_SE540144 = 2526;

	public const Int32 SFX_BTL_SE540145 = 2527;

	public const Int32 SFX_BTL_SE540146 = 2528;

	public const Int32 SFX_BTL_SE540148 = 2529;

	public const Int32 SFX_BTL_SE540149 = 2530;

	public const Int32 SFX_BTL_SE540150 = 2531;

	public const Int32 SFX_BTL_SE540152 = 2532;

	public const Int32 SFX_BTL_SE540153 = 2533;

	public const Int32 SFX_BTL_SE540154 = 2534;

	public const Int32 SFX_BTL_SE540156 = 2573;

	public const Int32 SFX_BTL_SE540157 = 2574;

	public const Int32 SFX_BTL_SE540158 = 2575;

	public const Int32 SFX_BTL_SE540160 = 2576;

	public const Int32 SFX_BTL_SE540161 = 2577;

	public const Int32 SFX_BTL_SE540162 = 2578;

	public const Int32 SFX_BTL_SE540164 = 2535;

	public const Int32 SFX_BTL_SE540165 = 2536;

	public const Int32 SFX_BTL_SE540166 = 2537;

	public const Int32 SFX_BTL_SE540167 = 2538;

	public const Int32 SFX_BTL_SE540168 = 2539;

	public const Int32 SFX_BTL_SE540169 = 2540;

	public const Int32 SFX_BTL_SE540170 = 2541;

	public const Int32 SFX_BTL_SE540171 = 2542;

	public const Int32 SFX_BTL_SE540172 = 2543;

	public const Int32 SFX_BTL_SE540173 = 2544;

	public const Int32 SFX_BTL_SE540174 = 2545;

	public const Int32 SFX_BTL_SE540175 = 2546;

	public const Int32 SFX_BTL_SE540176 = 2547;

	public const Int32 SFX_BTL_SE540177 = 2548;

	public const Int32 SFX_BTL_SE540178 = 2549;

	public const Int32 SFX_BTL_SE540179 = 2550;

	public const Int32 SFX_BTL_SE540180 = 2551;

	public const Int32 SFX_BTL_SE540181 = 2552;

	public const Int32 SFX_BTL_SE540182 = 2553;

	public const Int32 SFX_BTL_SE540183 = 2554;

	public const Int32 SFX_BTL_SE540184 = 2555;

	public const Int32 SFX_BTL_SE540185 = 2556;

	public const Int32 SFX_BTL_SE540186 = 2557;

	public const Int32 SFX_BTL_SE540187 = 2558;

	public const Int32 SFX_BTL_SE540188 = 2559;

	public const Int32 SFX_BTL_SE540189 = 2560;

	public const Int32 SFX_BTL_SE540190 = 2561;

	public const Int32 SFX_BTL_SE540191 = 2562;

	public const Int32 SFX_BTL_SE540192 = 2563;

	public const Int32 SFX_BTL_SE540193 = 2564;

	public const Int32 SFX_BTL_SE540194 = 2565;

	public const Int32 SFX_BTL_SE540196 = 2566;

	public const Int32 SFX_BTL_SE540197 = 2567;

	public const Int32 SFX_BTL_SE540198 = 2568;

	public const Int32 SFX_BTL_SE540200 = 2569;

	public const Int32 SFX_BTL_SE540201 = 2570;

	public const Int32 SFX_BTL_SE540202 = 2571;

	public const Int32 SFX_BTL_SE540204 = 2579;

	public const Int32 SFX_BTL_SE540205 = 2580;

	public const Int32 SFX_BTL_SE540206 = 2581;

	public const Int32 SFX_BTL_SE540208 = 2582;

	public const Int32 SFX_BTL_SE540209 = 2583;

	public const Int32 SFX_BTL_SE540210 = 2584;

	public const Int32 SFX_BTL_SE540212 = 2572;

	public const Int32 SFX_BTL_SE540213 = 2585;

	public const Int32 SFX_BTL_SE540214 = 2586;

	public const Int32 SFX_BTL_SE540216 = 2587;

	public const Int32 SFX_BTL_SE540217 = 2588;

	public const Int32 SFX_BTL_SE540218 = 2589;

	public const Int32 SFX_BTL_SE540220 = 2590;

	public const Int32 SFX_BTL_SE540221 = 2591;

	public const Int32 SFX_BTL_SE540222 = 2592;

	public const Int32 SFX_BTL_SE540224 = 2593;

	public const Int32 SFX_BTL_SE540225 = 2594;

	public const Int32 SFX_BTL_SE540226 = 2595;

	public const Int32 SFX_BTL_SE540228 = 2599;

	public const Int32 SFX_BTL_SE540229 = 2600;

	public const Int32 SFX_BTL_SE540230 = 2601;

	public const Int32 SFX_BTL_SE540232 = 2602;

	public const Int32 SFX_BTL_SE540233 = 2603;

	public const Int32 SFX_BTL_SE540234 = 2604;

	public const Int32 SFX_BTL_SE550000 = 2607;

	public const Int32 SFX_BTL_SE550001 = 2608;

	public const Int32 SFX_BTL_SE550002 = 2609;

	public const Int32 SFX_BTL_SE550004 = 2610;

	public const Int32 SFX_BTL_SE550005 = 2611;

	public const Int32 SFX_BTL_SE550006 = 2612;

	public const Int32 SFX_BTL_SE550008 = 2613;

	public const Int32 SFX_BTL_SE550009 = 2614;

	public const Int32 SFX_BTL_SE550010 = 2615;

	public const Int32 SFX_BTL_SE550012 = 2616;

	public const Int32 SFX_BTL_SE550013 = 2617;

	public const Int32 SFX_BTL_SE550014 = 2618;

	public const Int32 SFX_BTL_SE550016 = 2619;

	public const Int32 SFX_BTL_SE550017 = 2620;

	public const Int32 SFX_BTL_SE550018 = 2621;

	public const Int32 SFX_BTL_SE550020 = 2622;

	public const Int32 SFX_BTL_SE550021 = 2623;

	public const Int32 SFX_BTL_SE550022 = 2624;

	public const Int32 SFX_BTL_SE550023 = 2625;

	public const Int32 SFX_BTL_SE550024 = 2626;

	public const Int32 SFX_BTL_SE550025 = 2627;

	public const Int32 SFX_BTL_SE550026 = 2628;

	public const Int32 SFX_BTL_SE550027 = 2629;

	public const Int32 SFX_BTL_SE550028 = 2660;

	public const Int32 SFX_BTL_SE550029 = 2661;

	public const Int32 SFX_BTL_SE550030 = 2662;

	public const Int32 SFX_BTL_SE550032 = 2663;

	public const Int32 SFX_BTL_SE550033 = 2664;

	public const Int32 SFX_BTL_SE550034 = 2665;

	public const Int32 SFX_BTL_SE550036 = 2666;

	public const Int32 SFX_BTL_SE550037 = 2667;

	public const Int32 SFX_BTL_SE550038 = 2668;

	public const Int32 SFX_BTL_SE550040 = 2669;

	public const Int32 SFX_BTL_SE550041 = 2670;

	public const Int32 SFX_BTL_SE550042 = 2671;

	public const Int32 SFX_BTL_SE550044 = 2672;

	public const Int32 SFX_BTL_SE550045 = 2673;

	public const Int32 SFX_BTL_SE550046 = 2674;

	public const Int32 SFX_BTL_SE550048 = 2675;

	public const Int32 SFX_BTL_SE550049 = 2676;

	public const Int32 SFX_BTL_SE550050 = 2677;

	public const Int32 SFX_BTL_SE550052 = 2678;

	public const Int32 SFX_BTL_SE550053 = 2679;

	public const Int32 SFX_BTL_SE550054 = 2680;

	public const Int32 SFX_BTL_SE550056 = 2681;

	public const Int32 SFX_BTL_SE550057 = 2682;

	public const Int32 SFX_BTL_SE550058 = 2683;

	public const Int32 SFX_BTL_SE550060 = 2684;

	public const Int32 SFX_BTL_SE550061 = 2685;

	public const Int32 SFX_BTL_SE550062 = 2686;

	public const Int32 SFX_BTL_SE550064 = 2687;

	public const Int32 SFX_BTL_SE550065 = 2688;

	public const Int32 SFX_BTL_SE550066 = 2689;

	public const Int32 SFX_BTL_SE550068 = 2690;

	public const Int32 SFX_BTL_SE550069 = 2691;

	public const Int32 SFX_BTL_SE550070 = 2692;

	public const Int32 SFX_BTL_SE550071 = 2693;

	public const Int32 SFX_BTL_SE550072 = 2694;

	public const Int32 SFX_BTL_SE550073 = 2695;

	public const Int32 SFX_BTL_SE550074 = 2696;

	public const Int32 SFX_BTL_SE550076 = 2697;

	public const Int32 SFX_BTL_SE550077 = 2698;

	public const Int32 SFX_BTL_SE550078 = 2699;

	public const Int32 SFX_BTL_SE550079 = 2700;

	public const Int32 SFX_BTL_SE550080 = 2701;

	public const Int32 SFX_BTL_SE550081 = 2702;

	public const Int32 SFX_BTL_SE550082 = 2703;

	public const Int32 SFX_BTL_SE550083 = 2704;

	public const Int32 SFX_BTL_SE550084 = 2705;

	public const Int32 SFX_BTL_SE550085 = 2706;

	public const Int32 SFX_BTL_SE550086 = 2707;

	public const Int32 SFX_BTL_SE550087 = 2708;

	public const Int32 SFX_BTL_SE550088 = 2709;

	public const Int32 SFX_BTL_SE550089 = 2710;

	public const Int32 SFX_BTL_SE550090 = 2711;

	public const Int32 SFX_BTL_SE550091 = 2712;

	public const Int32 SFX_BTL_SE550092 = 2713;

	public const Int32 SFX_BTL_SE550093 = 2714;

	public const Int32 SFX_BTL_SE550094 = 2715;

	public const Int32 SFX_BTL_SE550095 = 2716;

	public const Int32 SFX_BTL_SE550096 = 2717;

	public const Int32 SFX_BTL_SE550097 = 2718;

	public const Int32 SFX_BTL_SE550098 = 2719;

	public const Int32 SFX_BTL_SE550099 = 2720;

	public const Int32 SFX_BTL_SE550100 = 2733;

	public const Int32 SFX_BTL_SE550101 = 2734;

	public const Int32 SFX_BTL_SE550102 = 2735;

	public const Int32 SFX_BTL_SE550103 = 2736;

	public const Int32 SFX_BTL_SE550104 = 2737;

	public const Int32 SFX_BTL_SE550105 = 2738;

	public const Int32 SFX_BTL_SE550106 = 2739;

	public const Int32 SFX_BTL_SE550107 = 2740;

	public const Int32 SFX_BTL_SE550108 = 2741;

	public const Int32 SFX_BTL_SE550109 = 2742;

	public const Int32 SFX_BTL_SE550110 = 2743;

	public const Int32 SFX_BTL_SE550111 = 2744;

	public const Int32 SFX_BTL_SE550112 = 2745;

	public const Int32 SFX_BTL_SE550113 = 2746;

	public const Int32 SFX_BTL_SE550114 = 2747;

	public const Int32 SFX_BTL_SE550116 = 2748;

	public const Int32 SFX_BTL_SE550117 = 2749;

	public const Int32 SFX_BTL_SE550118 = 2750;

	public const Int32 SFX_BTL_SE550120 = 2795;

	public const Int32 SFX_BTL_SE550121 = 2796;

	public const Int32 SFX_BTL_SE550122 = 2797;

	public const Int32 SFX_BTL_SE550123 = 2798;

	public const Int32 SFX_BTL_SE550124 = 2799;

	public const Int32 SFX_BTL_SE550125 = 2800;

	public const Int32 SFX_BTL_SE550126 = 2801;

	public const Int32 SFX_BTL_SE550127 = 2802;

	public const Int32 SFX_BTL_SE550128 = 2803;

	public const Int32 SFX_BTL_SE550129 = 2804;

	public const Int32 SFX_BTL_SE550130 = 2805;

	public const Int32 SFX_BTL_SE550131 = 2806;

	public const Int32 SFX_BTL_SE550132 = 2807;

	public const Int32 SFX_BTL_SE550133 = 2808;

	public const Int32 SFX_BTL_SE550134 = 2809;

	public const Int32 SFX_BTL_SE550136 = 2810;

	public const Int32 SFX_BTL_SE550137 = 2811;

	public const Int32 SFX_BTL_SE550138 = 2812;

	public const Int32 SFX_BTL_SE550140 = 2815;

	public const Int32 SFX_BTL_SE550141 = 2816;

	public const Int32 SFX_BTL_SE550142 = 2817;

	public const Int32 SFX_BTL_SE550143 = 2818;

	public const Int32 SFX_BTL_SE550144 = 2819;

	public const Int32 SFX_BTL_SE550145 = 2820;

	public const Int32 SFX_BTL_SE550146 = 2821;

	public const Int32 SFX_BTL_SE550147 = 2822;

	public const Int32 SFX_BTL_SE550148 = 2823;

	public const Int32 SFX_BTL_SE550149 = 2824;

	public const Int32 SFX_BTL_SE550150 = 2825;

	public const Int32 SFX_BTL_SE550151 = 2826;

	public const Int32 SFX_BTL_SE550152 = 2827;

	public const Int32 SFX_BTL_SE550153 = 2828;

	public const Int32 SFX_BTL_SE550154 = 2829;

	public const Int32 SFX_BTL_SE550155 = 2830;

	public const Int32 SFX_BTL_SE550156 = 2831;

	public const Int32 SFX_BTL_SE550157 = 2832;

	public const Int32 SFX_BTL_SE550158 = 2833;

	public const Int32 SFX_BTL_SE550159 = 2834;

	public const Int32 SFX_BTL_SE550160 = 2835;

	public const Int32 SFX_BTL_SE550161 = 2836;

	public const Int32 SFX_BTL_SE550162 = 2837;

	public const Int32 SFX_BTL_SE550163 = 2838;

	public const Int32 SFX_BTL_SE550164 = 2839;

	public const Int32 SFX_BTL_SE550165 = 2840;

	public const Int32 SFX_BTL_SE550166 = 2841;

	public const Int32 SFX_BTL_SE550167 = 2842;

	public const Int32 SFX_BTL_SE550168 = 2843;

	public const Int32 SFX_BTL_SE550169 = 2844;

	public const Int32 SFX_BTL_SE550170 = 2845;

	public const Int32 SFX_BTL_SE550172 = 2846;

	public const Int32 SFX_BTL_SE550173 = 2847;

	public const Int32 SFX_BTL_SE550174 = 2848;

	public const Int32 SFX_BTL_SE550176 = 2849;

	public const Int32 SFX_BTL_SE550177 = 2850;

	public const Int32 SFX_BTL_SE550178 = 2851;

	public const Int32 SFX_BTL_SE550180 = 2852;

	public const Int32 SFX_BTL_SE550181 = 2853;

	public const Int32 SFX_BTL_SE550182 = 2854;

	public const Int32 SFX_BTL_SE550184 = 2855;

	public const Int32 SFX_BTL_SE550185 = 2856;

	public const Int32 SFX_BTL_SE550186 = 2857;

	public const Int32 SFX_BTL_SE550188 = 2858;

	public const Int32 SFX_BTL_SE550189 = 2859;

	public const Int32 SFX_BTL_SE550190 = 2860;

	public const Int32 SFX_BTL_SE550192 = 2861;

	public const Int32 SFX_BTL_SE550193 = 2862;

	public const Int32 SFX_BTL_SE550194 = 2863;

	public const Int32 SFX_BTL_SE550196 = 2873;

	public const Int32 SFX_BTL_SE550197 = 2874;

	public const Int32 SFX_BTL_SE550198 = 2875;

	public const Int32 SFX_BTL_SE550199 = 2876;

	public const Int32 SFX_BTL_SE550200 = 2877;

	public const Int32 SFX_BTL_SE550201 = 2878;

	public const Int32 SFX_BTL_SE550202 = 2879;

	public const Int32 SFX_BTL_SE550203 = 2932;

	public const Int32 SFX_BTL_SE550204 = 2880;

	public const Int32 SFX_BTL_SE550205 = 2881;

	public const Int32 SFX_BTL_SE550206 = 2882;

	public const Int32 SFX_BTL_SE550207 = 2883;

	public const Int32 SFX_BTL_SE550208 = 2884;

	public const Int32 SFX_BTL_SE550209 = 2885;

	public const Int32 SFX_BTL_SE550210 = 2886;

	public const Int32 SFX_BTL_SE550211 = 2887;

	public const Int32 SFX_BTL_SE550212 = 2888;

	public const Int32 SFX_BTL_SE550213 = 2889;

	public const Int32 SFX_BTL_SE550214 = 2890;

	public const Int32 SFX_BTL_SE550215 = 2891;

	public const Int32 SFX_BTL_SE550216 = 2892;

	public const Int32 SFX_BTL_SE550217 = 2893;

	public const Int32 SFX_BTL_SE550218 = 2894;

	public const Int32 SFX_BTL_SE550219 = 2895;

	public const Int32 SFX_BTL_SE550220 = 2896;

	public const Int32 SFX_BTL_SE550221 = 2897;

	public const Int32 SFX_BTL_SE550222 = 2898;

	public const Int32 SFX_BTL_SE550223 = 2899;

	public const Int32 SFX_BTL_SE550224 = 2900;

	public const Int32 SFX_BTL_SE550225 = 2901;

	public const Int32 SFX_BTL_SE550226 = 2902;

	public const Int32 SFX_BTL_SE550227 = 2903;

	public const Int32 SFX_BTL_SE560000 = 2910;

	public const Int32 SFX_BTL_SE560001 = 2911;

	public const Int32 SFX_BTL_SE560002 = 2912;

	public const Int32 SFX_BTL_SE560003 = 2913;

	public const Int32 SFX_BTL_SE560004 = 2914;

	public const Int32 SFX_BTL_SE560005 = 2915;

	public const Int32 SFX_BTL_SE560006 = 2916;

	public const Int32 SFX_BTL_SE560007 = 2917;

	public const Int32 SFX_BTL_SE560008 = 2918;

	public const Int32 SFX_BTL_SE560009 = 2919;

	public const Int32 SFX_BTL_SE560010 = 2920;

	public const Int32 SFX_BTL_SE560011 = 2921;

	public const Int32 SFX_BTL_SE560012 = 2922;

	public const Int32 SFX_BTL_SE560013 = 2923;

	public const Int32 SFX_BTL_SE560014 = 2924;

	public const Int32 SFX_BTL_SE560015 = 2925;

	public const Int32 SFX_BTL_SE560016 = 2929;

	public const Int32 SFX_BTL_SE560017 = 2930;

	public const Int32 SFX_BTL_SE560018 = 2931;

	public const Int32 SFX_BTL_SE560020 = 2933;

	public const Int32 SFX_BTL_SE560021 = 2934;

	public const Int32 SFX_BTL_SE560022 = 2935;

	public const Int32 SFX_BTL_SE560023 = 2936;

	public const Int32 SFX_BTL_SE560024 = 2937;

	public const Int32 SFX_BTL_SE560025 = 2938;

	public const Int32 SFX_BTL_SE560026 = 2939;

	public const Int32 SFX_BTL_SE560027 = 2940;

	public const Int32 SFX_BTL_SE560028 = 2951;

	public const Int32 SFX_BTL_SE560029 = 2952;

	public const Int32 SFX_BTL_SE560030 = 2953;

	public const Int32 SFX_BTL_SE560031 = 2954;

	public const Int32 SFX_BTL_SE560032 = 2955;

	public const Int32 SFX_BTL_SE560033 = 2956;

	public const Int32 SFX_BTL_SE560034 = 2957;

	public const Int32 SFX_BTL_SE560035 = 2958;

	public const Int32 SFX_BTL_SE560036 = 2959;

	public const Int32 SFX_BTL_SE560037 = 2960;

	public const Int32 SFX_BTL_SE560038 = 2961;

	public const Int32 SFX_BTL_SE560039 = 2962;

	public const Int32 SFX_BTL_SE560040 = 2963;

	public const Int32 SFX_BTL_SE560041 = 2964;

	public const Int32 SFX_BTL_SE560042 = 2965;

	public const Int32 SFX_BTL_SE560043 = 2966;

	public const Int32 SFX_SEP_MINIGAME_SE210000 = 1860;

	public const Int32 SFX_SEP_MINIGAME_SE210001 = 1853;

	public const Int32 SFX_SEP_MINIGAME_SE210002 = 1854;

	public const Int32 SFX_SEP_MINIGAME_SE210003 = 1855;

	public const Int32 SFX_SEP_MINIGAME_SE210004 = 1856;

	public const Int32 SFX_SEP_MINIGAME_SE210005 = 1857;

	public const Int32 SFX_SEP_MINIGAME_SE210006 = 1858;

	public const Int32 SFX_SEP_MINIGAME_SE210007 = 1859;

	public const Int32 SFX_SEP_MINIGAME_SE210008 = 1852;

	public const Int32 SFX_SEP_MINIGAME_SE210009 = 1861;

	public const Int32 SFX_SEP_MINIGAME_SE210010 = 1862;

	public const Int32 SFX_SEP_MINIGAME_SE210011 = 1863;

	public const Int32 SFX_SEP_MINIGAME_SE210012 = 1864;

	public const Int32 SFX_SEP_MINIGAME_SE210013 = 2331;

	public const Int32 SFX_SEP_MINIGAME_SE210014 = 2332;

	public const Int32 SFX_SEP_MINIGAME_SE210015 = 2333;

	public const Int32 SNG_MUSIC000 = 5;

	public const Int32 SNG_MUSIC001 = 6;

	public const Int32 SNG_MUSIC002 = 3;

	public const Int32 SNG_MUSIC003 = 4;

	public const Int32 SNG_MUSIC004 = 1;

	public const Int32 SNG_MUSIC005 = 2;

	public const Int32 SNG_MUSIC006 = 0;

	public const Int32 SNG_MUSIC007 = 10;

	public const Int32 SNG_MUSIC008 = 9;

	public const Int32 SNG_MUSIC009 = 15;

	public const Int32 SNG_MUSIC010 = 29;

	public const Int32 SNG_MUSIC011 = 32;

	public const Int32 SNG_MUSIC012 = 24;

	public const Int32 SNG_MUSIC013 = 27;

	public const Int32 SNG_MUSIC014 = 20;

	public const Int32 SNG_MUSIC015 = 22;

	public const Int32 SNG_MUSIC016 = 17;

	public const Int32 SNG_MUSIC017 = 18;

	public const Int32 SNG_MUSIC018 = 12;

	public const Int32 SNG_MUSIC019 = 14;

	public const Int32 SNG_MUSIC020 = 30;

	public const Int32 SNG_MUSIC021 = 33;

	public const Int32 SNG_MUSIC022 = 25;

	public const Int32 SNG_MUSIC023 = 34;

	public const Int32 SNG_MUSIC024 = 35;

	public const Int32 SNG_MUSIC025 = 36;

	public const Int32 SNG_MUSIC026 = 44;

	public const Int32 SNG_MUSIC027 = 49;

	public const Int32 SNG_MUSIC028 = 48;

	public const Int32 SNG_MUSIC029 = 50;

	public const Int32 SNG_MUSIC030 = 59;

	public const Int32 SNG_MUSIC031 = 60;

	public const Int32 SNG_MUSIC032 = 57;

	public const Int32 SNG_MUSIC033 = 58;

	public const Int32 SNG_MUSIC034 = 54;

	public const Int32 SNG_MUSIC035 = 56;

	public const Int32 SNG_MUSIC036 = 52;

	public const Int32 SNG_MUSIC037 = 53;

	public const Int32 SNG_MUSIC038 = 61;

	public const Int32 SNG_MUSIC039 = 62;

	public const Int32 SNG_MUSIC040 = 63;

	public const Int32 SNG_MUSIC041 = 65;

	public const Int32 SNG_MUSIC042 = 66;

	public const Int32 SNG_MUSIC043 = 67;

	public const Int32 SNG_MUSIC044 = 68;

	public const Int32 SNG_MUSIC045 = 69;

	public const Int32 SNG_MUSIC046 = 73;

	public const Int32 SNG_MUSIC047 = 74;

	public const Int32 SNG_MUSIC048 = 79;

	public const Int32 SNG_MUSIC049 = 80;

	public const Int32 SNG_MUSIC050 = 81;

	public const Int32 SNG_MUSIC051 = 70;

	public const Int32 SNG_MUSIC052 = 75;

	public const Int32 SNG_MUSIC053 = 76;

	public const Int32 SNG_MUSIC054 = 82;

	public const Int32 SNG_MUSIC055 = 83;

	public const Int32 SNG_MUSIC056 = 84;

	public const Int32 SNG_MUSIC057 = 85;

	public const Int32 SNG_MUSIC058 = 86;

	public const Int32 SNG_MUSIC059 = 87;

	public const Int32 SNG_MUSIC060 = 88;

	public const Int32 SNG_MUSIC061 = 89;

	public const Int32 SNG_MUSIC062 = 90;

	public const Int32 SNG_MUSIC063 = 91;

	public const Int32 SNG_MUSIC064 = 92;

	public const Int32 SNG_MUSIC065 = 93;

	public const Int32 SNG_MUSIC066 = 95;

	public const Int32 SNG_MUSIC067 = 96;

	public const Int32 SNG_MUSIC068 = 94;

	public const Int32 SNG_MUSIC069 = 97;

	public const Int32 SNG_MUSIC070 = 98;

	public const Int32 SNG_MUSIC071 = 99;

	public const Int32 SNG_MUSIC072 = 100;

	public const Int32 SNG_MUSIC073 = 101;

	public const Int32 SNG_MUSIC074 = 102;

	public const Int32 SNG_MUSIC075 = 103;

	public const Int32 SNG_MUSIC076 = 104;

	public const Int32 SNG_MUSIC077 = 105;

	public const Int32 SNG_MUSIC078 = 106;

	public const Int32 SNG_MUSIC079 = 108;

	public const Int32 SNG_MUSIC080 = 109;

	public const Int32 SNG_MUSIC081 = 110;

	public const Int32 SNG_MUSIC082 = 111;

	public const Int32 SNG_MUSIC083 = 112;

	public const Int32 SNG_MUSIC084 = 113;

	public const Int32 SNG_MUSIC085 = 115;

	public const Int32 SNG_MUSIC086 = 119;

	public const Int32 SNG_MUSIC087 = 121;

	public const Int32 SNG_MUSIC088 = 124;

	public const Int32 SNG_MUSIC089 = 125;

	public const Int32 SNG_MUSIC090 = 116;

	public const Int32 SNG_MUSIC091 = 117;

	public const Int32 SNG_MUSIC092 = 118;

	public const Int32 SNG_MUSIC093 = 129;

	public const Int32 SNG_MUSIC094 = 130;

	public const Int32 SNG_MUSIC095 = 131;

	public const Int32 SNG_MUSIC096 = 132;

	public const Int32 SNG_MUSIC097 = 133;

	public const Int32 SNG_MUSIC098 = 134;

	public const Int32 SNG_MUSIC099 = 135;

	public const Int32 SNG_MUSIC100 = 64;

	public const Int32 SNG_MUSIC101 = 71;

	public const Int32 SNG_MUSIC102 = 77;

	public const Int32 SNG_MUSIC103 = 136;

	public const Int32 SNG_MUSIC104 = 137;

	public const Int32 SNG_MUSIC105 = 139;

	public const Int32 SNG_MUSIC106 = 138;

	public const Int32 SNG_MUSIC107 = 8;

	public const Int32 SNG_MUSIC108 = 7;

	public const Int32 SNG_MUSIC109 = 140;

	public const Int32 SNG_MUSIC110 = 143;

	public const Int32 SNG_MUSIC111 = 145;

	public const Int32 SNG_MUSIC112 = 142;

	public const Int32 SNG_MUSIC113 = 144;

	public const Int32 SNG_MUSIC114 = 146;

	public const Int32 SNG_MUSIC115 = 41;

	public const Int32 SNG_MUSIC116 = 141;

	public const Int32 SNG_MUSIC117 = 45;

	public const Int32 SNG_MUSIC118 = 72;

	public const Int32 SNG_MUSIC119 = 78;

	public const Int32 SNG_MUSIC120 = 156;

	public const Int32 SNG_MUSIC121 = 148;

	public const Int32 SNG_MUSIC122 = 147;

	public const Int32 SNG_MUSIC123 = 149;

	public const Int32 SNG_MUSIC124 = 150;

	public const Int32 SNG_MUSIC125 = 151;

	public const Int32 SNG_MUSIC126 = 152;

	public const Int32 SNG_MUSIC127 = 155;

	public const Int32 SNG_MUSIC130 = 107;

	public const Int32 SNG_MUSIC131 = 114;

	public const Int32 SNG_MUSIC132 = 120;

	public const Int32 SNG_MUSIC133 = 122;

	public const Int32 SNG_MUSIC134 = 123;

	public const Int32 SNG_MUSIC135 = 126;

	public const Int32 SNG_MUSIC136 = 127;

	public const Int32 SNG_MUSIC137 = 128;

	public const Int32 SNG_MUSIC138 = 160;

	public const Int32 SNG_MUSIC139 = 157;

	public const Int32 SNG_MUSIC140 = 161;

	public const Int32 SNG_MUSIC155 = 153;

	public const Int32 SNG_MUSIC156 = 154;

	public const Int32 SNG_MUSIC160 = 158;

	public const Int32 SNG_MUSIC161 = 159;

	public const Int32 SNG_MUSIC162 = 162;

	public const Int32 SNG_MUSIC188 = 11;

	public const Int32 SNG_MUSIC189 = 13;

	public const Int32 SNG_MUSIC190 = 28;

	public const Int32 SNG_MUSIC191 = 31;

	public const Int32 SNG_MUSIC192 = 23;

	public const Int32 SNG_MUSIC193 = 26;

	public const Int32 SNG_MUSIC194 = 19;

	public const Int32 SNG_MUSIC195 = 21;

	public const Int32 SNG_MUSIC196 = 16;

	public const Int32 SNG_MUSIC197 = 39;

	public const Int32 SNG_MUSIC198 = 37;

	public const Int32 SNG_MUSIC199 = 38;

	public const Int32 SNG_MUSIC200 = 40;

	public const Int32 SNG_MUSIC201 = 43;

	public const Int32 SNG_MUSIC202 = 42;

	public const Int32 SNG_MUSIC203 = 47;

	public const Int32 SNG_MUSIC204 = 46;

	public const Int32 SNG_MUSIC205 = 55;

	public const Int32 SNG_MUSIC206 = 51;
}
