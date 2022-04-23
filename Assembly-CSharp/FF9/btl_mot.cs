using System;
using System.Collections.Generic;
using Memoria;
using Memoria.Data;
using UnityEngine;

namespace FF9
{
	public class btl_mot
	{
		static btl_mot()
		{
			// Note: this type is marked as 'beforefieldinit'.
			// Battle motion IDs for player characters are sorted like these:
			//  0:  stand (MP_IDLE_NORMAL),
			//  1:  low HP (MP_IDLE_DYING),
			//  2:  hitted (MP_DAMAGE1),
			//  3:  hitted strongly (MP_DAMAGE2),
			//  4:  KO (MP_DISABLE),
			//  5:  low HP -> stand (MP_GET_UP_DYING),
			//  6:  KO -> low HP (MP_GET_UP_DISABLE),
			//  7:  stand -> low HP (MP_DOWN_DYING),
			//  8:  low HP -> KO (MP_DOWN_DISABLE),
			//  9:  ready (MP_IDLE_CMD),
			//  10: stand -> ready (MP_NORMAL_TO_CMD),
			//  11: low HP -> ready (MP_DYING_TO_CMD),
			//  12: stand -> defend (MP_IDLE_TO_DEF),
			//  13: defend (MP_DEFENCE),
			//  14: defend -> stand (MP_DEF_TO_IDLE),
			//  15: cover (MP_COVER),
			//  16: dodge (MP_AVOID),
			//  17: flee (MP_ESCAPE),
			//  18: victory (MP_WIN),
			//  19: stand victory (MP_WIN_LOOP),
			//  20: stand -> run (MP_SET),
			//  21: run (MP_RUN),
			//  22: run -> attack (MP_RUN_TO_ATTACK),
			//  23: attack (MP_ATTACK),
			//  24: jump back (MP_BACK),
			//  25: jump back -> stand (MP_ATK_TO_NORMAL),
			//  26: stand -> cast (MP_IDLE_TO_CHANT),
			//  27: cast (MP_CHANT),
			//  28: cast -> stand (MP_MAGIC),
			//  29: move forward (MP_STEP_FORWARD),
			//  30: move backward (MP_STEP_BACK),
			//  31: item (MP_ITEM1),
			//  32: ready -> stand (MP_CMD_TO_NORMAL),
			//  33: cast alternate (MP_SPECIAL1)
			String[,] array = new String[19, 34];
			array[0, 0] = "ANH_MAIN_B0_000_000";
			array[0, 1] = "ANH_MAIN_B0_000_022";
			array[0, 2] = "ANH_MAIN_B0_000_300";
			array[0, 3] = "ANH_MAIN_B0_000_310";
			array[0, 4] = "ANH_MAIN_B0_000_033";
			array[0, 5] = "ANH_MAIN_B0_000_020";
			array[0, 6] = "ANH_MAIN_B0_000_032";
			array[0, 7] = "ANH_MAIN_B0_000_002";
			array[0, 8] = "ANH_MAIN_B0_000_023";
			array[0, 9] = "ANH_MAIN_B0_000_011";
			array[0, 10] = "ANH_MAIN_B0_000_001";
			array[0, 11] = "ANH_MAIN_B0_000_021";
			array[0, 12] = "ANH_MAIN_B0_000_400";
			array[0, 13] = "ANH_MAIN_B0_000_401";
			array[0, 14] = "ANH_MAIN_B0_000_402";
			array[0, 15] = "ANH_MAIN_B0_000_410";
			array[0, 16] = "ANH_MAIN_B0_000_420";
			array[0, 17] = "ANH_MAIN_B0_000_430";
			array[0, 18] = "ANH_MAIN_B0_000_500";
			array[0, 19] = "ANH_MAIN_B0_000_501";
			array[0, 20] = "ANH_MAIN_B0_000_100";
			array[0, 21] = "ANH_MAIN_B0_000_101";
			array[0, 22] = "ANH_MAIN_B0_000_102";
			array[0, 23] = "ANH_MAIN_B0_000_103";
			array[0, 24] = "ANH_MAIN_B0_000_104";
			array[0, 25] = "ANH_MAIN_B0_000_105";
			array[0, 26] = "ANH_MAIN_B0_000_200";
			array[0, 27] = "ANH_MAIN_B0_000_201";
			array[0, 28] = "ANH_MAIN_B0_000_202";
			array[0, 29] = "ANH_MAIN_B0_000_040";
			array[0, 30] = "ANH_MAIN_B0_000_050";
			array[0, 31] = "ANH_MAIN_B0_000_210";
			array[0, 32] = "ANH_MAIN_B0_000_010";
			array[0, 33] = "ANH_MAIN_B0_000_220";
			array[1, 0] = "ANH_MAIN_B0_001_000";
			array[1, 1] = "ANH_MAIN_B0_001_022";
			array[1, 2] = "ANH_MAIN_B0_001_300";
			array[1, 3] = "ANH_MAIN_B0_001_310";
			array[1, 4] = "ANH_MAIN_B0_001_033";
			array[1, 5] = "ANH_MAIN_B0_001_020";
			array[1, 6] = "ANH_MAIN_B0_001_032";
			array[1, 7] = "ANH_MAIN_B0_001_002";
			array[1, 8] = "ANH_MAIN_B0_001_023";
			array[1, 9] = "ANH_MAIN_B0_001_011";
			array[1, 10] = "ANH_MAIN_B0_001_001";
			array[1, 11] = "ANH_MAIN_B0_001_021";
			array[1, 12] = "ANH_MAIN_B0_001_400";
			array[1, 13] = "ANH_MAIN_B0_001_401";
			array[1, 14] = "ANH_MAIN_B0_001_402";
			array[1, 15] = "ANH_MAIN_B0_001_410";
			array[1, 16] = "ANH_MAIN_B0_001_420";
			array[1, 17] = "ANH_MAIN_B0_001_430";
			array[1, 18] = "ANH_MAIN_B0_001_500";
			array[1, 19] = "ANH_MAIN_B0_001_501";
			array[1, 20] = "ANH_MAIN_B0_001_100";
			array[1, 21] = "ANH_MAIN_B0_001_101";
			array[1, 22] = "ANH_MAIN_B0_001_102";
			array[1, 23] = "ANH_MAIN_B0_001_103";
			array[1, 24] = "ANH_MAIN_B0_001_104";
			array[1, 25] = "ANH_MAIN_B0_001_105";
			array[1, 26] = "ANH_MAIN_B0_001_200";
			array[1, 27] = "ANH_MAIN_B0_001_201";
			array[1, 28] = "ANH_MAIN_B0_001_202";
			array[1, 29] = "ANH_MAIN_B0_001_040";
			array[1, 30] = "ANH_MAIN_B0_001_050";
			array[1, 31] = "ANH_MAIN_B0_001_210";
			array[1, 32] = "ANH_MAIN_B0_001_010";
			array[1, 33] = "ANH_MAIN_B0_001_220";
			array[2, 0] = "ANH_MAIN_B0_006_000";
			array[2, 1] = "ANH_MAIN_B0_006_022";
			array[2, 2] = "ANH_MAIN_B0_006_300";
			array[2, 3] = "ANH_MAIN_B0_006_310";
			array[2, 4] = "ANH_MAIN_B0_006_033";
			array[2, 5] = "ANH_MAIN_B0_006_020";
			array[2, 6] = "ANH_MAIN_B0_006_032";
			array[2, 7] = "ANH_MAIN_B0_006_002";
			array[2, 8] = "ANH_MAIN_B0_006_023";
			array[2, 9] = "ANH_MAIN_B0_006_011";
			array[2, 10] = "ANH_MAIN_B0_006_001";
			array[2, 11] = "ANH_MAIN_B0_006_021";
			array[2, 12] = "ANH_MAIN_B0_006_400";
			array[2, 13] = "ANH_MAIN_B0_006_401";
			array[2, 14] = "ANH_MAIN_B0_006_402";
			array[2, 15] = "ANH_MAIN_B0_006_410";
			array[2, 16] = "ANH_MAIN_B0_006_420";
			array[2, 17] = "ANH_MAIN_B0_006_430";
			array[2, 18] = "ANH_MAIN_B0_006_500";
			array[2, 19] = "ANH_MAIN_B0_006_501";
			array[2, 20] = "ANH_MAIN_B0_006_100";
			array[2, 21] = "ANH_MAIN_B0_006_101";
			array[2, 22] = "ANH_MAIN_B0_006_102";
			array[2, 23] = "ANH_MAIN_B0_006_103";
			array[2, 24] = "ANH_MAIN_B0_006_104";
			array[2, 25] = "ANH_MAIN_B0_006_105";
			array[2, 26] = "ANH_MAIN_B0_006_200";
			array[2, 27] = "ANH_MAIN_B0_006_201";
			array[2, 28] = "ANH_MAIN_B0_006_202";
			array[2, 29] = "ANH_MAIN_B0_006_040";
			array[2, 30] = "ANH_MAIN_B0_006_050";
			array[2, 31] = "ANH_MAIN_B0_006_210";
			array[2, 32] = "ANH_MAIN_B0_006_010";
			array[2, 33] = "ANH_MAIN_B0_006_220";
			array[3, 0] = "ANH_MAIN_B0_002_000";
			array[3, 1] = "ANH_MAIN_B0_002_022";
			array[3, 2] = "ANH_MAIN_B0_002_300";
			array[3, 3] = "ANH_MAIN_B0_002_310";
			array[3, 4] = "ANH_MAIN_B0_002_033";
			array[3, 5] = "ANH_MAIN_B0_002_020";
			array[3, 6] = "ANH_MAIN_B0_002_032";
			array[3, 7] = "ANH_MAIN_B0_002_002";
			array[3, 8] = "ANH_MAIN_B0_002_023";
			array[3, 9] = "ANH_MAIN_B0_002_011";
			array[3, 10] = "ANH_MAIN_B0_002_001";
			array[3, 11] = "ANH_MAIN_B0_002_021";
			array[3, 12] = "ANH_MAIN_B0_002_400";
			array[3, 13] = "ANH_MAIN_B0_002_401";
			array[3, 14] = "ANH_MAIN_B0_002_402";
			array[3, 15] = "ANH_MAIN_B0_002_410";
			array[3, 16] = "ANH_MAIN_B0_002_420";
			array[3, 17] = "ANH_MAIN_B0_002_430";
			array[3, 18] = "ANH_MAIN_B0_002_500";
			array[3, 19] = "ANH_MAIN_B0_002_501";
			array[3, 20] = "ANH_MAIN_B0_002_100";
			array[3, 21] = "ANH_MAIN_B0_002_101";
			array[3, 22] = "ANH_MAIN_B0_002_102";
			array[3, 23] = "ANH_MAIN_B0_002_103";
			array[3, 24] = "ANH_MAIN_B0_002_104";
			array[3, 25] = "ANH_MAIN_B0_002_105";
			array[3, 26] = "ANH_MAIN_B0_002_200";
			array[3, 27] = "ANH_MAIN_B0_002_201";
			array[3, 28] = "ANH_MAIN_B0_002_202";
			array[3, 29] = "ANH_MAIN_B0_002_040";
			array[3, 30] = "ANH_MAIN_B0_002_050";
			array[3, 31] = "ANH_MAIN_B0_002_210";
			array[3, 32] = "ANH_MAIN_B0_002_010";
			array[3, 33] = "ANH_MAIN_B0_002_000";
			array[4, 0] = "ANH_MAIN_B0_003_000";
			array[4, 1] = "ANH_MAIN_B0_003_022";
			array[4, 2] = "ANH_MAIN_B0_003_300";
			array[4, 3] = "ANH_MAIN_B0_003_310";
			array[4, 4] = "ANH_MAIN_B0_003_033";
			array[4, 5] = "ANH_MAIN_B0_003_020";
			array[4, 6] = "ANH_MAIN_B0_003_032";
			array[4, 7] = "ANH_MAIN_B0_003_002";
			array[4, 8] = "ANH_MAIN_B0_003_023";
			array[4, 9] = "ANH_MAIN_B0_003_011";
			array[4, 10] = "ANH_MAIN_B0_003_001";
			array[4, 11] = "ANH_MAIN_B0_003_021";
			array[4, 12] = "ANH_MAIN_B0_003_400";
			array[4, 13] = "ANH_MAIN_B0_003_401";
			array[4, 14] = "ANH_MAIN_B0_003_402";
			array[4, 15] = "ANH_MAIN_B0_003_410";
			array[4, 16] = "ANH_MAIN_B0_003_420";
			array[4, 17] = "ANH_MAIN_B0_003_430";
			array[4, 18] = "ANH_MAIN_B0_003_500";
			array[4, 19] = "ANH_MAIN_B0_003_501";
			array[4, 20] = "ANH_MAIN_B0_003_100";
			array[4, 21] = "ANH_MAIN_B0_003_000";
			array[4, 22] = "ANH_MAIN_B0_003_000";
			array[4, 23] = "ANH_MAIN_B0_003_000";
			array[4, 24] = "ANH_MAIN_B0_003_000";
			array[4, 25] = "ANH_MAIN_B0_003_000";
			array[4, 26] = "ANH_MAIN_B0_003_200";
			array[4, 27] = "ANH_MAIN_B0_003_201";
			array[4, 28] = "ANH_MAIN_B0_003_202";
			array[4, 29] = "ANH_MAIN_B0_003_040";
			array[4, 30] = "ANH_MAIN_B0_003_050";
			array[4, 31] = "ANH_MAIN_B0_003_210";
			array[4, 32] = "ANH_MAIN_B0_003_010";
			array[4, 33] = "ANH_MAIN_B0_003_000";
			array[5, 0] = "ANH_MAIN_B0_004_000";
			array[5, 1] = "ANH_MAIN_B0_004_022";
			array[5, 2] = "ANH_MAIN_B0_004_300";
			array[5, 3] = "ANH_MAIN_B0_004_310";
			array[5, 4] = "ANH_MAIN_B0_004_033";
			array[5, 5] = "ANH_MAIN_B0_004_020";
			array[5, 6] = "ANH_MAIN_B0_004_032";
			array[5, 7] = "ANH_MAIN_B0_004_002";
			array[5, 8] = "ANH_MAIN_B0_004_023";
			array[5, 9] = "ANH_MAIN_B0_004_011";
			array[5, 10] = "ANH_MAIN_B0_004_001";
			array[5, 11] = "ANH_MAIN_B0_004_021";
			array[5, 12] = "ANH_MAIN_B0_004_400";
			array[5, 13] = "ANH_MAIN_B0_004_401";
			array[5, 14] = "ANH_MAIN_B0_004_402";
			array[5, 15] = "ANH_MAIN_B0_004_410";
			array[5, 16] = "ANH_MAIN_B0_004_420";
			array[5, 17] = "ANH_MAIN_B0_004_430";
			array[5, 18] = "ANH_MAIN_B0_004_500";
			array[5, 19] = "ANH_MAIN_B0_004_501";
			array[5, 20] = "ANH_MAIN_B0_004_100";
			array[5, 21] = "ANH_MAIN_B0_004_101";
			array[5, 22] = "ANH_MAIN_B0_004_102";
			array[5, 23] = "ANH_MAIN_B0_004_103";
			array[5, 24] = "ANH_MAIN_B0_004_104";
			array[5, 25] = "ANH_MAIN_B0_004_105";
			array[5, 26] = "ANH_MAIN_B0_004_200";
			array[5, 27] = "ANH_MAIN_B0_004_201";
			array[5, 28] = "ANH_MAIN_B0_004_202";
			array[5, 29] = "ANH_MAIN_B0_004_040";
			array[5, 30] = "ANH_MAIN_B0_004_050";
			array[5, 31] = "ANH_MAIN_B0_004_210";
			array[5, 32] = "ANH_MAIN_B0_004_010";
			array[5, 33] = "ANH_MAIN_B0_004_000";
			array[6, 0] = "ANH_MAIN_B0_005_000";
			array[6, 1] = "ANH_MAIN_B0_005_022";
			array[6, 2] = "ANH_MAIN_B0_005_300";
			array[6, 3] = "ANH_MAIN_B0_005_310";
			array[6, 4] = "ANH_MAIN_B0_005_033";
			array[6, 5] = "ANH_MAIN_B0_005_020";
			array[6, 6] = "ANH_MAIN_B0_005_032";
			array[6, 7] = "ANH_MAIN_B0_005_002";
			array[6, 8] = "ANH_MAIN_B0_005_023";
			array[6, 9] = "ANH_MAIN_B0_005_011";
			array[6, 10] = "ANH_MAIN_B0_005_001";
			array[6, 11] = "ANH_MAIN_B0_005_021";
			array[6, 12] = "ANH_MAIN_B0_005_400";
			array[6, 13] = "ANH_MAIN_B0_005_401";
			array[6, 14] = "ANH_MAIN_B0_005_402";
			array[6, 15] = "ANH_MAIN_B0_005_410";
			array[6, 16] = "ANH_MAIN_B0_005_420";
			array[6, 17] = "ANH_MAIN_B0_005_430";
			array[6, 18] = "ANH_MAIN_B0_005_500";
			array[6, 19] = "ANH_MAIN_B0_005_501";
			array[6, 20] = "ANH_MAIN_B0_005_100";
			array[6, 21] = "ANH_MAIN_B0_005_000";
			array[6, 22] = "ANH_MAIN_B0_005_000";
			array[6, 23] = "ANH_MAIN_B0_005_000";
			array[6, 24] = "ANH_MAIN_B0_005_000";
			array[6, 25] = "ANH_MAIN_B0_005_000";
			array[6, 26] = "ANH_MAIN_B0_005_200";
			array[6, 27] = "ANH_MAIN_B0_005_201";
			array[6, 28] = "ANH_MAIN_B0_005_202";
			array[6, 29] = "ANH_MAIN_B0_005_040";
			array[6, 30] = "ANH_MAIN_B0_005_050";
			array[6, 31] = "ANH_MAIN_B0_005_210";
			array[6, 32] = "ANH_MAIN_B0_005_010";
			array[6, 33] = "ANH_MAIN_B0_005_000";
			array[7, 0] = "ANH_MAIN_B0_007_000";
			array[7, 1] = "ANH_MAIN_B0_007_022";
			array[7, 2] = "ANH_MAIN_B0_007_300";
			array[7, 3] = "ANH_MAIN_B0_007_310";
			array[7, 4] = "ANH_MAIN_B0_007_033";
			array[7, 5] = "ANH_MAIN_B0_007_020";
			array[7, 6] = "ANH_MAIN_B0_007_032";
			array[7, 7] = "ANH_MAIN_B0_007_002";
			array[7, 8] = "ANH_MAIN_B0_007_023";
			array[7, 9] = "ANH_MAIN_B0_007_011";
			array[7, 10] = "ANH_MAIN_B0_007_001";
			array[7, 11] = "ANH_MAIN_B0_007_021";
			array[7, 12] = "ANH_MAIN_B0_007_400";
			array[7, 13] = "ANH_MAIN_B0_007_401";
			array[7, 14] = "ANH_MAIN_B0_007_402";
			array[7, 15] = "ANH_MAIN_B0_007_410";
			array[7, 16] = "ANH_MAIN_B0_007_420";
			array[7, 17] = "ANH_MAIN_B0_007_430";
			array[7, 18] = "ANH_MAIN_B0_007_500";
			array[7, 19] = "ANH_MAIN_B0_007_501";
			array[7, 20] = "ANH_MAIN_B0_007_100";
			array[7, 21] = "ANH_MAIN_B0_007_101";
			array[7, 22] = "ANH_MAIN_B0_007_102";
			array[7, 23] = "ANH_MAIN_B0_007_103";
			array[7, 24] = "ANH_MAIN_B0_007_104";
			array[7, 25] = "ANH_MAIN_B0_007_105";
			array[7, 26] = "ANH_MAIN_B0_007_200";
			array[7, 27] = "ANH_MAIN_B0_007_201";
			array[7, 28] = "ANH_MAIN_B0_007_202";
			array[7, 29] = "ANH_MAIN_B0_007_040";
			array[7, 30] = "ANH_MAIN_B0_007_050";
			array[7, 31] = "ANH_MAIN_B0_007_210";
			array[7, 32] = "ANH_MAIN_B0_007_010";
			array[7, 33] = "ANH_MAIN_B0_007_220";
			array[8, 0] = "ANH_MAIN_B0_007_000";
			array[8, 1] = "ANH_MAIN_B0_007_022";
			array[8, 2] = "ANH_MAIN_B0_007_300";
			array[8, 3] = "ANH_MAIN_B0_007_310";
			array[8, 4] = "ANH_MAIN_B0_007_033";
			array[8, 5] = "ANH_MAIN_B0_007_020";
			array[8, 6] = "ANH_MAIN_B0_007_032";
			array[8, 7] = "ANH_MAIN_B0_007_002";
			array[8, 8] = "ANH_MAIN_B0_007_023";
			array[8, 9] = "ANH_MAIN_B0_007_011";
			array[8, 10] = "ANH_MAIN_B0_007_001";
			array[8, 11] = "ANH_MAIN_B0_007_021";
			array[8, 12] = "ANH_MAIN_B0_007_400";
			array[8, 13] = "ANH_MAIN_B0_007_401";
			array[8, 14] = "ANH_MAIN_B0_007_402";
			array[8, 15] = "ANH_MAIN_B0_007_410";
			array[8, 16] = "ANH_MAIN_B0_007_420";
			array[8, 17] = "ANH_MAIN_B0_007_430";
			array[8, 18] = "ANH_MAIN_B0_007_500";
			array[8, 19] = "ANH_MAIN_B0_007_501";
			array[8, 20] = "ANH_MAIN_B0_007_100";
			array[8, 21] = "ANH_MAIN_B0_007_101";
			array[8, 22] = "ANH_MAIN_B0_007_102";
			array[8, 23] = "ANH_MAIN_B0_007_103";
			array[8, 24] = "ANH_MAIN_B0_007_104";
			array[8, 25] = "ANH_MAIN_B0_007_105";
			array[8, 26] = "ANH_MAIN_B0_007_200";
			array[8, 27] = "ANH_MAIN_B0_007_201";
			array[8, 28] = "ANH_MAIN_B0_007_202";
			array[8, 29] = "ANH_MAIN_B0_007_040";
			array[8, 30] = "ANH_MAIN_B0_007_050";
			array[8, 31] = "ANH_MAIN_B0_007_210";
			array[8, 32] = "ANH_MAIN_B0_007_010";
			array[8, 33] = "ANH_MAIN_B0_007_220";
			array[9, 0] = "ANH_MAIN_B0_008_000";
			array[9, 1] = "ANH_MAIN_B0_008_022";
			array[9, 2] = "ANH_MAIN_B0_008_300";
			array[9, 3] = "ANH_MAIN_B0_008_310";
			array[9, 4] = "ANH_MAIN_B0_008_033";
			array[9, 5] = "ANH_MAIN_B0_008_020";
			array[9, 6] = "ANH_MAIN_B0_008_032";
			array[9, 7] = "ANH_MAIN_B0_008_002";
			array[9, 8] = "ANH_MAIN_B0_008_023";
			array[9, 9] = "ANH_MAIN_B0_008_011";
			array[9, 10] = "ANH_MAIN_B0_008_001";
			array[9, 11] = "ANH_MAIN_B0_008_021";
			array[9, 12] = "ANH_MAIN_B0_008_400";
			array[9, 13] = "ANH_MAIN_B0_008_401";
			array[9, 14] = "ANH_MAIN_B0_008_402";
			array[9, 15] = "ANH_MAIN_B0_008_410";
			array[9, 16] = "ANH_MAIN_B0_008_420";
			array[9, 17] = "ANH_MAIN_B0_008_430";
			array[9, 18] = "ANH_MAIN_B0_008_500";
			array[9, 19] = "ANH_MAIN_B0_008_501";
			array[9, 20] = "ANH_MAIN_B0_008_100";
			array[9, 21] = "ANH_MAIN_B0_008_101";
			array[9, 22] = "ANH_MAIN_B0_008_102";
			array[9, 23] = "ANH_MAIN_B0_008_103";
			array[9, 24] = "ANH_MAIN_B0_008_104";
			array[9, 25] = "ANH_MAIN_B0_008_105";
			array[9, 26] = "ANH_MAIN_B0_008_200";
			array[9, 27] = "ANH_MAIN_B0_008_201";
			array[9, 28] = "ANH_MAIN_B0_008_202";
			array[9, 29] = "ANH_MAIN_B0_008_040";
			array[9, 30] = "ANH_MAIN_B0_008_050";
			array[9, 31] = "ANH_MAIN_B0_008_210";
			array[9, 32] = "ANH_MAIN_B0_008_010";
			array[9, 33] = "ANH_MAIN_B0_008_220";
			array[10, 0] = "ANH_MAIN_B0_009_000";
			array[10, 1] = "ANH_MAIN_B0_009_022";
			array[10, 2] = "ANH_MAIN_B0_009_300";
			array[10, 3] = "ANH_MAIN_B0_009_310";
			array[10, 4] = "ANH_MAIN_B0_009_033";
			array[10, 5] = "ANH_MAIN_B0_009_020";
			array[10, 6] = "ANH_MAIN_B0_009_032";
			array[10, 7] = "ANH_MAIN_B0_009_002";
			array[10, 8] = "ANH_MAIN_B0_009_023";
			array[10, 9] = "ANH_MAIN_B0_009_011";
			array[10, 10] = "ANH_MAIN_B0_009_001";
			array[10, 11] = "ANH_MAIN_B0_009_021";
			array[10, 12] = "ANH_MAIN_B0_009_400";
			array[10, 13] = "ANH_MAIN_B0_009_401";
			array[10, 14] = "ANH_MAIN_B0_009_402";
			array[10, 15] = "ANH_MAIN_B0_009_410";
			array[10, 16] = "ANH_MAIN_B0_009_420";
			array[10, 17] = "ANH_MAIN_B0_009_430";
			array[10, 18] = "ANH_MAIN_B0_009_500";
			array[10, 19] = "ANH_MAIN_B0_009_501";
			array[10, 20] = "ANH_MAIN_B0_009_100";
			array[10, 21] = "ANH_MAIN_B0_009_101";
			array[10, 22] = "ANH_MAIN_B0_009_102";
			array[10, 23] = "ANH_MAIN_B0_009_103";
			array[10, 24] = "ANH_MAIN_B0_009_104";
			array[10, 25] = "ANH_MAIN_B0_009_105";
			array[10, 26] = "ANH_MAIN_B0_009_200";
			array[10, 27] = "ANH_MAIN_B0_009_201";
			array[10, 28] = "ANH_MAIN_B0_009_202";
			array[10, 29] = "ANH_MAIN_B0_009_040";
			array[10, 30] = "ANH_MAIN_B0_009_050";
			array[10, 31] = "ANH_MAIN_B0_009_210";
			array[10, 32] = "ANH_MAIN_B0_009_010";
			array[10, 33] = "ANH_MAIN_B0_009_000";
			array[11, 0] = "ANH_MAIN_B0_010_000";
			array[11, 1] = "ANH_MAIN_B0_010_022";
			array[11, 2] = "ANH_MAIN_B0_010_300";
			array[11, 3] = "ANH_MAIN_B0_010_310";
			array[11, 4] = "ANH_MAIN_B0_010_033";
			array[11, 5] = "ANH_MAIN_B0_010_020";
			array[11, 6] = "ANH_MAIN_B0_010_032";
			array[11, 7] = "ANH_MAIN_B0_010_002";
			array[11, 8] = "ANH_MAIN_B0_010_023";
			array[11, 9] = "ANH_MAIN_B0_010_011";
			array[11, 10] = "ANH_MAIN_B0_010_001";
			array[11, 11] = "ANH_MAIN_B0_010_021";
			array[11, 12] = "ANH_MAIN_B0_010_400";
			array[11, 13] = "ANH_MAIN_B0_010_401";
			array[11, 14] = "ANH_MAIN_B0_010_402";
			array[11, 15] = "ANH_MAIN_B0_010_410";
			array[11, 16] = "ANH_MAIN_B0_010_420";
			array[11, 17] = "ANH_MAIN_B0_010_430";
			array[11, 18] = "ANH_MAIN_B0_010_500";
			array[11, 19] = "ANH_MAIN_B0_010_501";
			array[11, 20] = "ANH_MAIN_B0_010_100";
			array[11, 21] = "ANH_MAIN_B0_010_000";
			array[11, 22] = "ANH_MAIN_B0_010_000";
			array[11, 23] = "ANH_MAIN_B0_010_000";
			array[11, 24] = "ANH_MAIN_B0_010_000";
			array[11, 25] = "ANH_MAIN_B0_010_000";
			array[11, 26] = "ANH_MAIN_B0_010_200";
			array[11, 27] = "ANH_MAIN_B0_010_201";
			array[11, 28] = "ANH_MAIN_B0_010_202";
			array[11, 29] = "ANH_MAIN_B0_010_040";
			array[11, 30] = "ANH_MAIN_B0_010_050";
			array[11, 31] = "ANH_MAIN_B0_010_210";
			array[11, 32] = "ANH_MAIN_B0_010_010";
			array[11, 33] = "ANH_MAIN_B0_010_000";
			array[12, 0] = "ANH_MAIN_B0_011_000";
			array[12, 1] = "ANH_MAIN_B0_011_022";
			array[12, 2] = "ANH_MAIN_B0_011_300";
			array[12, 3] = "ANH_MAIN_B0_011_310";
			array[12, 4] = "ANH_MAIN_B0_011_033";
			array[12, 5] = "ANH_MAIN_B0_011_020";
			array[12, 6] = "ANH_MAIN_B0_011_032";
			array[12, 7] = "ANH_MAIN_B0_011_002";
			array[12, 8] = "ANH_MAIN_B0_011_023";
			array[12, 9] = "ANH_MAIN_B0_011_011";
			array[12, 10] = "ANH_MAIN_B0_011_001";
			array[12, 11] = "ANH_MAIN_B0_011_021";
			array[12, 12] = "ANH_MAIN_B0_011_400";
			array[12, 13] = "ANH_MAIN_B0_011_401";
			array[12, 14] = "ANH_MAIN_B0_011_402";
			array[12, 15] = "ANH_MAIN_B0_011_410";
			array[12, 16] = "ANH_MAIN_B0_011_420";
			array[12, 17] = "ANH_MAIN_B0_011_430";
			array[12, 18] = "ANH_MAIN_B0_011_500";
			array[12, 19] = "ANH_MAIN_B0_011_501";
			array[12, 20] = "ANH_MAIN_B0_011_100";
			array[12, 21] = "ANH_MAIN_B0_011_101";
			array[12, 22] = "ANH_MAIN_B0_011_102";
			array[12, 23] = "ANH_MAIN_B0_011_103";
			array[12, 24] = "ANH_MAIN_B0_011_104";
			array[12, 25] = "ANH_MAIN_B0_011_105";
			array[12, 26] = "ANH_MAIN_B0_011_200"; // Swapped with ANH_MAIN_B0_011_110 for consistency (prepare jump)
			array[12, 27] = "ANH_MAIN_B0_011_201"; // Swapped with ANH_MAIN_B0_011_111 for consistency (jump)
			array[12, 28] = "ANH_MAIN_B0_011_202"; // Swapped with ANH_MAIN_B0_011_112 for consistency (spear)
			array[12, 29] = "ANH_MAIN_B0_011_040";
			array[12, 30] = "ANH_MAIN_B0_011_050";
			array[12, 31] = "ANH_MAIN_B0_011_210";
			array[12, 32] = "ANH_MAIN_B0_011_010";
			array[12, 33] = "ANH_MAIN_B0_011_113";
			array[13, 0] = "ANH_MAIN_B0_012_000";
			array[13, 1] = "ANH_MAIN_B0_012_022";
			array[13, 2] = "ANH_MAIN_B0_012_300";
			array[13, 3] = "ANH_MAIN_B0_012_310";
			array[13, 4] = "ANH_MAIN_B0_012_033";
			array[13, 5] = "ANH_MAIN_B0_012_020";
			array[13, 6] = "ANH_MAIN_B0_012_032";
			array[13, 7] = "ANH_MAIN_B0_012_002";
			array[13, 8] = "ANH_MAIN_B0_012_023";
			array[13, 9] = "ANH_MAIN_B0_012_011";
			array[13, 10] = "ANH_MAIN_B0_012_001";
			array[13, 11] = "ANH_MAIN_B0_012_021";
			array[13, 12] = "ANH_MAIN_B0_012_400";
			array[13, 13] = "ANH_MAIN_B0_012_401";
			array[13, 14] = "ANH_MAIN_B0_012_402";
			array[13, 15] = "ANH_MAIN_B0_012_410";
			array[13, 16] = "ANH_MAIN_B0_012_420";
			array[13, 17] = "ANH_MAIN_B0_012_430";
			array[13, 18] = "ANH_MAIN_B0_012_500";
			array[13, 19] = "ANH_MAIN_B0_012_501";
			array[13, 20] = "ANH_MAIN_B0_012_100";
			array[13, 21] = "ANH_MAIN_B0_012_101";
			array[13, 22] = "ANH_MAIN_B0_012_102";
			array[13, 23] = "ANH_MAIN_B0_012_103";
			array[13, 24] = "ANH_MAIN_B0_012_104";
			array[13, 25] = "ANH_MAIN_B0_012_105";
			array[13, 26] = "ANH_MAIN_B0_012_200";
			array[13, 27] = "ANH_MAIN_B0_012_201";
			array[13, 28] = "ANH_MAIN_B0_012_202";
			array[13, 29] = "ANH_MAIN_B0_012_040";
			array[13, 30] = "ANH_MAIN_B0_012_050";
			array[13, 31] = "ANH_MAIN_B0_012_210";
			array[13, 32] = "ANH_MAIN_B0_012_010";
			array[13, 33] = "ANH_MAIN_B0_012_220";
			array[14, 0] = "ANH_MAIN_B0_013_000";
			array[14, 1] = "ANH_MAIN_B0_013_022";
			array[14, 2] = "ANH_MAIN_B0_013_300";
			array[14, 3] = "ANH_MAIN_B0_013_310";
			array[14, 4] = "ANH_MAIN_B0_013_033";
			array[14, 5] = "ANH_MAIN_B0_013_020";
			array[14, 6] = "ANH_MAIN_B0_013_032";
			array[14, 7] = "ANH_MAIN_B0_013_002";
			array[14, 8] = "ANH_MAIN_B0_013_023";
			array[14, 9] = "ANH_MAIN_B0_013_011";
			array[14, 10] = "ANH_MAIN_B0_013_001";
			array[14, 11] = "ANH_MAIN_B0_013_021";
			array[14, 12] = "ANH_MAIN_B0_013_400";
			array[14, 13] = "ANH_MAIN_B0_013_401";
			array[14, 14] = "ANH_MAIN_B0_013_402";
			array[14, 15] = "ANH_MAIN_B0_013_410";
			array[14, 16] = "ANH_MAIN_B0_013_420";
			array[14, 17] = "ANH_MAIN_B0_013_430";
			array[14, 18] = "ANH_MAIN_B0_013_500";
			array[14, 19] = "ANH_MAIN_B0_013_501";
			array[14, 20] = "ANH_MAIN_B0_013_100";
			array[14, 21] = "ANH_MAIN_B0_013_101";
			array[14, 22] = "ANH_MAIN_B0_013_102";
			array[14, 23] = "ANH_MAIN_B0_013_103";
			array[14, 24] = "ANH_MAIN_B0_013_104";
			array[14, 25] = "ANH_MAIN_B0_013_105";
			array[14, 26] = "ANH_MAIN_B0_013_200";
			array[14, 27] = "ANH_MAIN_B0_013_201";
			array[14, 28] = "ANH_MAIN_B0_013_202";
			array[14, 29] = "ANH_MAIN_B0_013_040";
			array[14, 30] = "ANH_MAIN_B0_013_050";
			array[14, 31] = "ANH_MAIN_B0_013_210";
			array[14, 32] = "ANH_MAIN_B0_013_010";
			array[14, 33] = "ANH_MAIN_B0_013_220";
			array[15, 0] = "ANH_MAIN_B0_014_000";
			array[15, 1] = "ANH_MAIN_B0_014_022";
			array[15, 2] = "ANH_MAIN_B0_014_300";
			array[15, 3] = "ANH_MAIN_B0_014_310";
			array[15, 4] = "ANH_MAIN_B0_014_033";
			array[15, 5] = "ANH_MAIN_B0_014_020";
			array[15, 6] = "ANH_MAIN_B0_014_032";
			array[15, 7] = "ANH_MAIN_B0_014_002";
			array[15, 8] = "ANH_MAIN_B0_014_023";
			array[15, 9] = "ANH_MAIN_B0_014_011";
			array[15, 10] = "ANH_MAIN_B0_014_001";
			array[15, 11] = "ANH_MAIN_B0_014_021";
			array[15, 12] = "ANH_MAIN_B0_014_400";
			array[15, 13] = "ANH_MAIN_B0_014_401";
			array[15, 14] = "ANH_MAIN_B0_014_402";
			array[15, 15] = "ANH_MAIN_B0_014_410";
			array[15, 16] = "ANH_MAIN_B0_014_420";
			array[15, 17] = "ANH_MAIN_B0_014_430";
			array[15, 18] = "ANH_MAIN_B0_014_500";
			array[15, 19] = "ANH_MAIN_B0_014_501";
			array[15, 20] = "ANH_MAIN_B0_014_100";
			array[15, 21] = "ANH_MAIN_B0_014_101";
			array[15, 22] = "ANH_MAIN_B0_014_102";
			array[15, 23] = "ANH_MAIN_B0_014_103";
			array[15, 24] = "ANH_MAIN_B0_014_104";
			array[15, 25] = "ANH_MAIN_B0_014_105";
			array[15, 26] = "ANH_MAIN_B0_014_200";
			array[15, 27] = "ANH_MAIN_B0_014_201";
			array[15, 28] = "ANH_MAIN_B0_014_202";
			array[15, 29] = "ANH_MAIN_B0_014_040";
			array[15, 30] = "ANH_MAIN_B0_014_050";
			array[15, 31] = "ANH_MAIN_B0_014_210";
			array[15, 32] = "ANH_MAIN_B0_014_010";
			array[15, 33] = "ANH_MAIN_B0_014_220";
			array[16, 0] = "ANH_MAIN_B0_015_000";
			array[16, 1] = "ANH_MAIN_B0_015_022";
			array[16, 2] = "ANH_MAIN_B0_015_300";
			array[16, 3] = "ANH_MAIN_B0_015_310";
			array[16, 4] = "ANH_MAIN_B0_015_033";
			array[16, 5] = "ANH_MAIN_B0_015_020";
			array[16, 6] = "ANH_MAIN_B0_015_032";
			array[16, 7] = "ANH_MAIN_B0_015_002";
			array[16, 8] = "ANH_MAIN_B0_015_023";
			array[16, 9] = "ANH_MAIN_B0_015_011";
			array[16, 10] = "ANH_MAIN_B0_015_001";
			array[16, 11] = "ANH_MAIN_B0_015_021";
			array[16, 12] = "ANH_MAIN_B0_015_400";
			array[16, 13] = "ANH_MAIN_B0_015_401";
			array[16, 14] = "ANH_MAIN_B0_015_402";
			array[16, 15] = "ANH_MAIN_B0_015_410";
			array[16, 16] = "ANH_MAIN_B0_015_420";
			array[16, 17] = "ANH_MAIN_B0_015_430";
			array[16, 18] = "ANH_MAIN_B0_015_500";
			array[16, 19] = "ANH_MAIN_B0_015_501";
			array[16, 20] = "ANH_MAIN_B0_015_100";
			array[16, 21] = "ANH_MAIN_B0_015_101";
			array[16, 22] = "ANH_MAIN_B0_015_102";
			array[16, 23] = "ANH_MAIN_B0_015_103";
			array[16, 24] = "ANH_MAIN_B0_015_104";
			array[16, 25] = "ANH_MAIN_B0_015_105";
			array[16, 26] = "ANH_MAIN_B0_015_200";
			array[16, 27] = "ANH_MAIN_B0_015_201";
			array[16, 28] = "ANH_MAIN_B0_015_202";
			array[16, 29] = "ANH_MAIN_B0_015_040";
			array[16, 30] = "ANH_MAIN_B0_015_050";
			array[16, 31] = "ANH_MAIN_B0_015_210";
			array[16, 32] = "ANH_MAIN_B0_015_010";
			array[16, 33] = "ANH_MAIN_B0_015_220";
			array[17, 0] = "ANH_MAIN_B0_016_000";
			array[17, 1] = "ANH_MAIN_B0_016_022";
			array[17, 2] = "ANH_MAIN_B0_016_300";
			array[17, 3] = "ANH_MAIN_B0_016_310";
			array[17, 4] = "ANH_MAIN_B0_016_033";
			array[17, 5] = "ANH_MAIN_B0_016_020";
			array[17, 6] = "ANH_MAIN_B0_016_032";
			array[17, 7] = "ANH_MAIN_B0_016_002";
			array[17, 8] = "ANH_MAIN_B0_016_023";
			array[17, 9] = "ANH_MAIN_B0_016_011";
			array[17, 10] = "ANH_MAIN_B0_016_001";
			array[17, 11] = "ANH_MAIN_B0_016_021";
			array[17, 12] = "ANH_MAIN_B0_016_400";
			array[17, 13] = "ANH_MAIN_B0_016_401";
			array[17, 14] = "ANH_MAIN_B0_016_402";
			array[17, 15] = "ANH_MAIN_B0_016_410";
			array[17, 16] = "ANH_MAIN_B0_016_420";
			array[17, 17] = "ANH_MAIN_B0_016_430";
			array[17, 18] = "ANH_MAIN_B0_016_500";
			array[17, 19] = "ANH_MAIN_B0_016_501";
			array[17, 20] = "ANH_MAIN_B0_016_100";
			array[17, 21] = "ANH_MAIN_B0_016_101";
			array[17, 22] = "ANH_MAIN_B0_016_102";
			array[17, 23] = "ANH_MAIN_B0_016_103";
			array[17, 24] = "ANH_MAIN_B0_016_104";
			array[17, 25] = "ANH_MAIN_B0_016_105";
			array[17, 26] = "ANH_MAIN_B0_016_200";
			array[17, 27] = "ANH_MAIN_B0_016_201";
			array[17, 28] = "ANH_MAIN_B0_016_202";
			array[17, 29] = "ANH_MAIN_B0_016_040";
			array[17, 30] = "ANH_MAIN_B0_016_050";
			array[17, 31] = "ANH_MAIN_B0_016_210";
			array[17, 32] = "ANH_MAIN_B0_016_010";
			array[17, 33] = "ANH_MAIN_B0_016_220";
			array[18, 0] = "ANH_MAIN_B0_017_000";
			array[18, 1] = "ANH_MAIN_B0_017_022";
			array[18, 2] = "ANH_MAIN_B0_017_300";
			array[18, 3] = "ANH_MAIN_B0_017_310";
			array[18, 4] = "ANH_MAIN_B0_017_033";
			array[18, 5] = "ANH_MAIN_B0_017_020";
			array[18, 6] = "ANH_MAIN_B0_017_032";
			array[18, 7] = "ANH_MAIN_B0_017_002";
			array[18, 8] = "ANH_MAIN_B0_017_023";
			array[18, 9] = "ANH_MAIN_B0_017_011";
			array[18, 10] = "ANH_MAIN_B0_017_001";
			array[18, 11] = "ANH_MAIN_B0_017_021";
			array[18, 12] = "ANH_MAIN_B0_017_400";
			array[18, 13] = "ANH_MAIN_B0_017_401";
			array[18, 14] = "ANH_MAIN_B0_017_402";
			array[18, 15] = "ANH_MAIN_B0_017_410";
			array[18, 16] = "ANH_MAIN_B0_017_420";
			array[18, 17] = "ANH_MAIN_B0_017_430";
			array[18, 18] = "ANH_MAIN_B0_017_500";
			array[18, 19] = "ANH_MAIN_B0_017_501";
			array[18, 20] = "ANH_MAIN_B0_017_100";
			array[18, 21] = "ANH_MAIN_B0_017_101";
			array[18, 22] = "ANH_MAIN_B0_017_102";
			array[18, 23] = "ANH_MAIN_B0_017_103";
			array[18, 24] = "ANH_MAIN_B0_017_104";
			array[18, 25] = "ANH_MAIN_B0_017_105";
			array[18, 26] = "ANH_MAIN_B0_017_200";
			array[18, 27] = "ANH_MAIN_B0_017_201";
			array[18, 28] = "ANH_MAIN_B0_017_202";
			array[18, 29] = "ANH_MAIN_B0_017_040";
			array[18, 30] = "ANH_MAIN_B0_017_050";
			array[18, 31] = "ANH_MAIN_B0_017_210";
			array[18, 32] = "ANH_MAIN_B0_017_010";
			array[18, 33] = "ANH_MAIN_B0_017_000";
			btl_mot.mot = array;
			// Starting and ending stances of each regular animation
			btl_mot.mot_stance = new BattlePlayerCharacter.PlayerMotionStance[35, 2]
			{
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 0:  MP_IDLE_NORMAL
				{ BattlePlayerCharacter.PlayerMotionStance.DYING, BattlePlayerCharacter.PlayerMotionStance.DYING }, // 1:  MP_IDLE_DYING
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 2:  MP_DAMAGE1
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.DISABLE }, // 3:  MP_DAMAGE2
				{ BattlePlayerCharacter.PlayerMotionStance.DISABLE, BattlePlayerCharacter.PlayerMotionStance.DISABLE }, // 4:  MP_DISABLE
				{ BattlePlayerCharacter.PlayerMotionStance.DYING, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 5:  MP_GET_UP_DYING
				{ BattlePlayerCharacter.PlayerMotionStance.DISABLE, BattlePlayerCharacter.PlayerMotionStance.DYING }, // 6:  MP_GET_UP_DISABLE
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.DYING }, // 7:  MP_DOWN_DYING
				{ BattlePlayerCharacter.PlayerMotionStance.DYING, BattlePlayerCharacter.PlayerMotionStance.DISABLE }, // 8:  MP_DOWN_DISABLE
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 9:  MP_IDLE_CMD
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 10: MP_NORMAL_TO_CMD
				{ BattlePlayerCharacter.PlayerMotionStance.DYING, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 11: MP_DYING_TO_CMD
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.DEFEND }, // 12: MP_IDLE_TO_DEF
				{ BattlePlayerCharacter.PlayerMotionStance.DEFEND, BattlePlayerCharacter.PlayerMotionStance.DEFEND }, // 13: MP_DEFENCE
				{ BattlePlayerCharacter.PlayerMotionStance.DEFEND, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 14: MP_DEF_TO_IDLE
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_ANY_IDLE }, // 15: MP_COVER
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_ANY_IDLE }, // 16: MP_AVOID
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_ANY_IDLE }, // 17: MP_ESCAPE
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.WIN }, // 18: MP_WIN
				{ BattlePlayerCharacter.PlayerMotionStance.WIN, BattlePlayerCharacter.PlayerMotionStance.WIN }, // 19: MP_WIN_LOOP
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT }, // 20: MP_SET
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT }, // 21: MP_RUN
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT }, // 22: MP_RUN_TO_ATTACK
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT }, // 23: MP_ATTACK
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT }, // 24: MP_BACK
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 25: MP_ATK_TO_NORMAL
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 26: MP_IDLE_TO_CHANT
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 27: MP_CHANT
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 28: MP_MAGIC
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 29: MP_STEP_FORWARD
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 30: MP_STEP_BACK
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 31: MP_ITEM1
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 32: MP_CMD_TO_NORMAL
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_UNKNOWN, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_UNKNOWN }, // 33: MP_SPECIAL1
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_UNKNOWN, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_UNKNOWN }  // 34: MP_MAX
			};
			// List of animations that can't be interrupted by eg. the escape key
			unstoppable_mot = new HashSet<BattlePlayerCharacter.PlayerMotionIndex>
			{
				BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1,
				BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2,
				BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE,
				BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DISABLE
			};
		}

	    public static void setMotion(BattleUnit btl, Byte index)
        {
			setMotion(btl.Data, btl.Data.mot[(Int32)index]);
		}

        public static void setMotion(BTL_DATA btl, Byte index)
		{
			setMotion(btl, btl.mot[(Int32)index]);
		}

		public static void setMotion(BTL_DATA btl, BattlePlayerCharacter.PlayerMotionIndex index)
		{
			setMotion(btl, btl.mot[(Int32)index]);
		}

		public static BattlePlayerCharacter.PlayerMotionIndex getMotion(BattleUnit btl)
		{
			return getMotion(btl.Data);
		}

        public static BattlePlayerCharacter.PlayerMotionIndex getMotion(BTL_DATA btl)
		{
			for (Int32 i = 0; i < btl.mot.Length; i++)
				if (btl.currentAnimationName == btl.mot[i])
					return (BattlePlayerCharacter.PlayerMotionIndex)i;
			return BattlePlayerCharacter.PlayerMotionIndex.MP_MAX;
		}

		public static void setMotion(BTL_DATA btl, String name)
		{
			btl.currentAnimationName = name;
			btl.animSpeed = 1f;
			btl.animFlag = 0;
			btl.animEndFrame = IsAnimationFrozen(btl);
		}

		public static Boolean checkMotion(BTL_DATA btl, Byte index)
		{
			if ((Int32)index > (Int32)btl.mot.Length)
				return false;
			String b = btl.mot[(Int32)index];
			return btl.currentAnimationName == b;
		}

		public static Boolean checkMotion(BTL_DATA btl, BattlePlayerCharacter.PlayerMotionIndex index)
		{
			if ((Int32)index > (Int32)btl.mot.Length)
				return false;
			String b = btl.mot[(Int32)index];
			return btl.currentAnimationName == b;
		}

		public static Boolean IsLoopingMotion(BattlePlayerCharacter.PlayerMotionIndex index)
		{
			return index == BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_COVER
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_AVOID
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_WIN_LOOP
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_RUN
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT;
		}

		public static void PlayAnim(BTL_DATA btl)
		{
			if (btl.currentAnimationName == null)
				return;
			GameObject gameObject = btl.gameObject;
			String currentAnimationName = btl.currentAnimationName;
			UInt16 animMaxFrame = GeoAnim.geoAnimGetNumFrames(btl, currentAnimationName);
			Byte animFrame = btl.evt.animFrame;
			if (!gameObject.GetComponent<Animation>().IsPlaying(currentAnimationName))
			{
				if (gameObject.GetComponent<Animation>().GetClip(currentAnimationName) != (UnityEngine.Object)null)
				{
					AnimationState clipState = gameObject.GetComponent<Animation>()[currentAnimationName];
					gameObject.GetComponent<Animation>().Play(currentAnimationName);
					clipState.speed = 0f;
					if (animMaxFrame == 0)
						clipState.time = 0f;
					else
						clipState.time = (Single)animFrame / (Single)animMaxFrame * gameObject.GetComponent<Animation>()[currentAnimationName].length;
					gameObject.GetComponent<Animation>().Sample();
				}
			}
			else
			{
				AnimationState clipState = gameObject.GetComponent<Animation>()[currentAnimationName];
				clipState.speed = 0f;
				if (animMaxFrame == 0)
					clipState.time = 0f;
				else
					clipState.time = (Single)animFrame / (Single)animMaxFrame * gameObject.GetComponent<Animation>()[currentAnimationName].length;
				gameObject.GetComponent<Animation>().Sample();
			}
		}

		public static Int32 GetDirection(BTL_DATA btl)
		{
			return (Int32)btl.evt.rotBattle.eulerAngles[1];
		}

	    public static Int32 GetDirection(BattleUnit btl)
	    {
			return (Int32)btl.Data.evt.rotBattle.eulerAngles[1];
		}

	    public static void setSlavePos(BTL_DATA btl, ref Vector3 pos)
		{
			pos[0] = btl.gameObject.transform.GetChildByName("bone" + btl.tar_bone.ToString("D3")).position.x;
			pos[1] = 0f;
			pos[2] = btl.gameObject.transform.GetChildByName("bone" + btl.tar_bone.ToString("D3")).position.z;
		}

		public static void setBasePos(BTL_DATA btl)
		{
			btl.pos[0] = btl.base_pos[0];
			btl.pos[1] = btl.base_pos[1];
			btl.pos[2] = btl.base_pos[2];
		}

		public static Boolean IsAnimationFrozen(BTL_DATA btl)
		{
			return btl.bi.slave != 0
				|| btl_stat.CheckStatus(btl, BattleStatus.Immobilized)
				|| (btl.animFlag & EventEngine.afFreeze) != 0
				|| btl.bi.stop_anim != 0;
		}

		public static Boolean IsEnemyDyingAnimation(BTL_DATA btl, String anim)
		{
			return anim == btl.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE]
				|| anim == btl.mot[5]
				|| (btl_util.getEnemyPtr(btl).info.die_dmg != 0 && anim == btl.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2]);
		}

		public static void DieSequence(BTL_DATA btl)
		{
			if (btl.die_seq != 0)
			{
				if (btl.bi.player == 0)
				{
					FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
					ENEMY enemyPtr = btl_util.getEnemyPtr(btl);
					if (btl.bi.slave != 0 && btl.die_seq < 5)
						btl.die_seq = 5;
					switch (btl.die_seq)
					{
						case 1:
							//btl_mot.setMotion(btl, (Byte)(4 + btl.bi.def_idle));
							//btl.evt.animFrame = 0;
							btl.die_seq++;
							btl_util.SetEnemyDieSound(btl, enemyPtr.et.die_snd_no);
							break;
						case 2:
							//if ((UInt16)btl.evt.animFrame >= GeoAnim.getAnimationLoopFrame(btl))
							if (btl.animEndFrame && btl_mot.IsEnemyDyingAnimation(btl, btl.endedAnimationName))
							{
								btl.evt.animFrame = (Byte)GeoAnim.geoAnimGetNumFrames(btl);
								btl.bi.stop_anim = 1;
								btl.die_seq++;
							}
							break;
						case 3:
							if (Configuration.Battle.Speed >= 3 && (btl_util.IsBtlUsingCommand(btl) || (enemyPtr.info.die_fade_rate >= 32 && btl_util.IsBtlTargetOfCommand(btl))))
								return;
							btl_util.SetBattleSfx(btl, 121, 127);
							btl_util.SetBattleSfx(btl, 120, 127);
							btl.die_seq++;
							btl_util.SetEnemyFadeToPacket(btl, (Int32)enemyPtr.info.die_fade_rate);
							if (enemyPtr.info.die_fade_rate == 0)
								btl.die_seq = 5;
							else
								enemyPtr.info.die_fade_rate -= 2;
							break;
						case 4:
							btl_util.SetEnemyFadeToPacket(btl, (Int32)enemyPtr.info.die_fade_rate);
							if (enemyPtr.info.die_fade_rate == 0)
								btl.die_seq = 5;
							else if (Configuration.Battle.Speed < 3 || enemyPtr.info.die_fade_rate > 2 || !btl_util.IsBtlBusy(btl, btl_util.BusyMode.ANY_CURRENT))
								enemyPtr.info.die_fade_rate -= 2;
							break;
						case 5:
							if (!btl_util.IsBtlBusy(btl, btl_util.BusyMode.ANY_CURRENT))
							{
								FF9StateGlobal ff = FF9StateSystem.Common.FF9;
								if (btl_util.CheckEnemyCategory(btl, 8) && ff.dragon_no < 9999)
									ff.dragon_no++;
								if (ff.btl_result != 4)
									btl_sys.SetBonus(enemyPtr.et);
								btl.die_seq++;
								if (ff9Battle.btl_phase != 5 || (ff9Battle.btl_seq != 3 && ff9Battle.btl_seq != 2))
									btl_sys.CheckBattlePhase(btl);
								btl_sys.DelCharacter(btl);
							}
							break;
					}
				}
				else
				{
					if (btl.die_seq < 4 && (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE) || btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE)))
					{
						if (btl.is_monster_transform)
							btl_util.SetBattleSfx(btl, btl.monster_transform.death_sound, 127);
						btl.die_seq = 4;
					}
					else if ((btl.die_seq == 4 && btl.animEndFrame) || btl.die_seq == 5)
					{
						if (btl_mot.DecidePlayerDieSequence(btl))
							btl_sys.CheckBattlePhase(btl);
					}
					// Old version
					//switch (btl.die_seq)
					//{
					//	case 1:
					//		if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD))
					//		{
					//			btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_CMD_TO_NORMAL);
					//			btl.evt.animFrame = 0;
					//		}
					//		else if (!btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_CMD_TO_NORMAL) || (UInt16)btl.evt.animFrame >= GeoAnim.geoAnimGetNumFrames(btl))
					//		{
					//			btl.die_seq = 2;
					//			if (btl.bi.def_idle == 1)
					//			{
					//				btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE);
					//				btl.die_seq = 4;
					//				if (btl.is_monster_transform)
					//					btl_util.SetBattleSfx(btl, btl.monster_transform.death_sound, 127);
					//			}
					//			else
					//			{
					//				btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DYING);
					//				btl.die_seq = 3;
					//			}
					//			btl.evt.animFrame = 0;
					//		}
					//		break;
					//	case 2:
					//		if (btl.bi.def_idle == 1)
					//		{
					//			btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE);
					//			btl.die_seq = 4;
					//			if (btl.is_monster_transform)
					//				btl_util.SetBattleSfx(btl, btl.monster_transform.death_sound, 127);
					//		}
					//		else
					//		{
					//			btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DYING);
					//			btl.die_seq = 3;
					//		}
					//		btl.evt.animFrame = 0;
					//		break;
					//	case 3:
					//		if ((UInt16)btl.evt.animFrame >= GeoAnim.geoAnimGetNumFrames(btl))
					//		{
					//			btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE);
					//			btl.evt.animFrame = 0;
					//			if (btl.is_monster_transform)
					//				btl_util.SetBattleSfx(btl, btl.monster_transform.death_sound, 127);
					//			btl.die_seq++;
					//		}
					//		break;
					//	case 4:
					//		if ((UInt16)btl.evt.animFrame >= GeoAnim.geoAnimGetNumFrames(btl))
					//			if (btl_mot.DecidePlayerDieSequence(btl))
					//				btl_sys.CheckBattlePhase(btl);
					//		break;
					//	case 5:
					//		if (btl_mot.DecidePlayerDieSequence(btl))
					//			btl_sys.CheckBattlePhase(btl);
					//		break;
					//}
				}
			}
		}

		public static Boolean DecidePlayerDieSequence(BTL_DATA btl)
		{
			if (btl.is_monster_transform && btl.monster_transform.cancel_on_death)
				new BattleUnit(btl).ReleaseChangeToMonster();
			GeoTexAnim.geoTexAnimStop(btl.texanimptr, 2);
			GeoTexAnim.geoTexAnimPlayOnce(btl.texanimptr, 0);
			if (btl.bi.player != 0)
			{
				GeoTexAnim.geoTexAnimStop(btl.tranceTexanimptr, 2);
				GeoTexAnim.geoTexAnimPlayOnce(btl.tranceTexanimptr, 0);
			}
			if (btl_stat.CheckStatus(btl, BattleStatus.AutoLife))
			{
				//btl.die_seq = 7;
				//btl_cmd.SetCommand(btl.cmd[5], BattleCommandId.SysReraise, 0u, btl.btl_id, 0u);
				btl_stat.RemoveStatus(btl, BattleStatus.AutoLife);
				UIManager.Battle.RemovePlayerFromAction((Int32)btl.btl_id, true);
				btl.cur.hp = 1;
				btl_stat.RemoveStatus(btl, BattleStatus.Death);
				FF9StateSystem.Settings.SetHPFull();
				btl_mot.SetDefaultIdle(btl);
				return false;
			}
			btl.die_seq = 6;
			return true;
		}

		public static BattlePlayerCharacter.PlayerMotionStance StartingMotionStance(BattlePlayerCharacter.PlayerMotionIndex motion)
		{
			return btl_mot.mot_stance[(Int32)motion, 0];
		}

		public static BattlePlayerCharacter.PlayerMotionStance EndingMotionStance(BattlePlayerCharacter.PlayerMotionIndex motion)
		{
			return btl_mot.mot_stance[(Int32)motion, 1];
		}

		public static BattlePlayerCharacter.PlayerMotionIndex StanceTransition(BattlePlayerCharacter.PlayerMotionStance from, BattlePlayerCharacter.PlayerMotionStance to)
		{
			if (from == BattlePlayerCharacter.PlayerMotionStance.NORMAL)
			{
				if (to == BattlePlayerCharacter.PlayerMotionStance.DYING || to == BattlePlayerCharacter.PlayerMotionStance.DISABLE)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DYING;
				if (to == BattlePlayerCharacter.PlayerMotionStance.CMD || to == BattlePlayerCharacter.PlayerMotionStance.DEFEND)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_NORMAL_TO_CMD;
			}
			if (from == BattlePlayerCharacter.PlayerMotionStance.DYING)
			{
				if (to == BattlePlayerCharacter.PlayerMotionStance.NORMAL)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DYING;
				if (to == BattlePlayerCharacter.PlayerMotionStance.DISABLE)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE;
				if (to == BattlePlayerCharacter.PlayerMotionStance.CMD || to == BattlePlayerCharacter.PlayerMotionStance.DEFEND)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_DYING_TO_CMD;
			}
			if (from == BattlePlayerCharacter.PlayerMotionStance.DISABLE)
				return BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DISABLE;
			if (from == BattlePlayerCharacter.PlayerMotionStance.CMD)
			{
				if (to == BattlePlayerCharacter.PlayerMotionStance.NORMAL || to == BattlePlayerCharacter.PlayerMotionStance.DYING || to == BattlePlayerCharacter.PlayerMotionStance.DISABLE)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_CMD_TO_NORMAL;
				if (to == BattlePlayerCharacter.PlayerMotionStance.DEFEND)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_DEF;
			}
			if (from == BattlePlayerCharacter.PlayerMotionStance.DEFEND)
				return BattlePlayerCharacter.PlayerMotionIndex.MP_DEF_TO_IDLE;
			if (to == BattlePlayerCharacter.PlayerMotionStance.WIN)
				return BattlePlayerCharacter.PlayerMotionIndex.MP_WIN;
			return BattlePlayerCharacter.PlayerMotionIndex.MP_MAX;
		}

		public static void SetDefaultIdle(BTL_DATA btl, Boolean isEndOfAnim = false)
		{
			BattlePlayerCharacter.PlayerMotionIndex targetAnim = (BattlePlayerCharacter.PlayerMotionIndex)btl.bi.def_idle;
			BattlePlayerCharacter.PlayerMotionIndex currentAnim = btl_mot.getMotion(btl);
			if (btl_stat.CheckStatus(btl, BattleStatus.Immobilized))
			{
				if (btl.bi.player != 0 && btl_stat.CheckStatus(btl, BattleStatus.Venom) && !btl.is_monster_transform)
				{
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING);
					btl.evt.animFrame = 0;
				}
				return;
			}

			Boolean useCmdMotion = btl_util.IsBtlUsingCommandMotion(btl);
			if (btl.bi.player == 0 || btl.is_monster_transform)
			{
				if (useCmdMotion && currentAnim != BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2 && currentAnim != BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1)
					targetAnim = currentAnim;
				else if ((btl.die_seq == 1 || btl.die_seq == 2) && btl.bi.player == 0 && btl_util.getEnemyPtr(btl).info.die_dmg != 0)
					targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2;
				else if (btl.die_seq == 1 || btl.die_seq == 2)
					targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE + btl.bi.def_idle;
				else if (btl.die_seq != 0)
					targetAnim = currentAnim;
				if (currentAnim == targetAnim)
				{
					if (isEndOfAnim)
						btl.evt.animFrame = 0;
					return;
				}
				btl_mot.setMotion(btl, targetAnim);
				btl.evt.animFrame = 0;
				return;
			}
			if (FF9StateSystem.Battle.FF9Battle.btl_phase == 6 && !btl_stat.CheckStatus(btl, BattleStatus.BattleEnd) && FF9StateSystem.Battle.FF9Battle.btl_scene.Info.WinPose != 0 && btl_util.getPlayerPtr(btl).info.win_pose != 0)
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_WIN_LOOP;
			else if (btl.bi.cover != 0)
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_COVER;
			else if (useCmdMotion && isEndOfAnim)
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD;
			else if (useCmdMotion)
				return;
			else if (!isEndOfAnim && unstoppable_mot.Contains(currentAnim))
				return;
			else if ((FF9StateSystem.Battle.FF9Battle.cmd_status & 1) != 0 && !btl_stat.CheckStatus(btl, BattleStatus.CannotEscape))
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE;
			else if (Status.checkCurStat(btl, BattleStatus.Death))
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE;
			else if (btl.bi.dodge != 0)
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_AVOID;
			else if (btl_stat.CheckStatus(btl, BattleStatus.Defend))
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE;
			else if (FF9StateSystem.Battle.FF9Battle.btl_escape_key != 0 && !btl_stat.CheckStatus(btl, BattleStatus.CannotEscape))
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE;
			else if (btl.bi.cmd_idle == 1)
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD;
			if (currentAnim == targetAnim)
			{
				if (isEndOfAnim)
					btl.evt.animFrame = 0;
				return;
			}
			BattlePlayerCharacter.PlayerMotionStance previousStance = btl_mot.EndingMotionStance(currentAnim);
			BattlePlayerCharacter.PlayerMotionStance nextStance = btl_mot.StartingMotionStance(targetAnim);
			if (previousStance == BattlePlayerCharacter.PlayerMotionStance.SPECIAL_ANY_IDLE)
			{
				if (nextStance == BattlePlayerCharacter.PlayerMotionStance.NORMAL || nextStance == BattlePlayerCharacter.PlayerMotionStance.CMD || nextStance == BattlePlayerCharacter.PlayerMotionStance.DYING || nextStance == BattlePlayerCharacter.PlayerMotionStance.DEFEND)
				{
					btl_mot.setMotion(btl, targetAnim);
					btl.evt.animFrame = 0;
					return;
				}
				if (btl_stat.CheckStatus(btl, BattleStatus.Defend))
					previousStance = BattlePlayerCharacter.PlayerMotionStance.DEFEND;
				else if(useCmdMotion || btl.bi.cmd_idle == 1)
					previousStance = BattlePlayerCharacter.PlayerMotionStance.CMD;
				else if(btl.bi.def_idle == 1)
					previousStance = BattlePlayerCharacter.PlayerMotionStance.DYING;
				else
					previousStance = BattlePlayerCharacter.PlayerMotionStance.NORMAL;
			}
			if (previousStance == nextStance || previousStance == BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT || previousStance == BattlePlayerCharacter.PlayerMotionStance.SPECIAL_UNKNOWN || nextStance == BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT)
			{
				btl_mot.setMotion(btl, targetAnim);
				btl.evt.animFrame = 0;
				return;
			}
			BattlePlayerCharacter.PlayerMotionIndex transition = btl_mot.StanceTransition(previousStance, nextStance);
			if (transition == BattlePlayerCharacter.PlayerMotionIndex.MP_MAX)
			{
				btl_mot.setMotion(btl, targetAnim);
				btl.evt.animFrame = 0;
			}
			else
			{
				btl_mot.setMotion(btl, transition);
				btl.evt.animFrame = 0;
			}
		}

		public static Boolean SetDefaultIdleOld(BTL_DATA btl) // Not used anymore
		{
			FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
			CMD_DATA cur_cmd = ff9Battle.cur_cmd;
			if (Status.checkCurStat(btl, BattleStatus.Death))
			{
				if (btl.bi.player != 0 && btl.bi.dmg_mot_f == 0 && cur_cmd != null && btl != cur_cmd.regist && btl.die_seq == 0 && !btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE) && !btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD))
					btl_mot.setMotion(btl, btl.bi.def_idle);
				return false;
			}
			if (cur_cmd != null && (btl_stat.CheckStatus(btl, BattleStatus.FrozenAnimation) || btl.bi.dmg_mot_f != 0 || (btl_util.getSerialNumber(btl) == 2 && cur_cmd.cmd_no == BattleCommandId.MagicSword)))
				return false;
			if (cur_cmd != null && btl == cur_cmd.regist && (cur_cmd.cmd_no < BattleCommandId.EnemyReaction || cur_cmd.cmd_no > BattleCommandId.SysReraise))
			{
				//if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_NORMAL_TO_CMD) || btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DYING_TO_CMD))
				if (btl.bi.player != 0)
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD);
				return false;
			}
			if (btl.bi.player != 0)
			{
				if (btl.bi.cover == 0 && btl.bi.dodge == 0)
				{
					if ((ff9Battle.btl_escape_key != 0 || (ff9Battle.cmd_status & 1) != 0) && !btl_stat.CheckStatus(btl, BattleStatus.CannotEscape))
						btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE);
					else if (btl_stat.CheckStatus(btl, BattleStatus.Defend))
						btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE);
					else if (btl.bi.cmd_idle != 0)
					{
						if (btl_mot.checkMotion(btl, btl.bi.def_idle))
							btl_mot.setMotion(btl, (Byte)(10 + btl.bi.def_idle));
						else
							btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD);
					}
					else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL) && btl_stat.CheckStatus(btl, BattleStatus.IdleDying))
					{
						global::Debug.LogWarning(btl.gameObject.name + " Dead");
						btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DYING);
					}
					else if ((btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING) || btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DISABLE)) && !btl_stat.CheckStatus(btl, BattleStatus.IdleDying))
						btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DYING);
					else
						btl_mot.setMotion(btl, btl.bi.def_idle);
				}
			}
			else
			{
				btl_mot.setMotion(btl, btl.bi.def_idle);
			}
			btl.evt.animFrame = 0;
			return true;
		}

		public static void SetDamageMotion(BattleUnit btl, CMD_DATA cmd)
		{
			if (btl_util.IsBtlUsingCommandMotion(btl.Data, true))
			{
				if (!btl.IsPlayer && btl.Data.cur.hp == 0 && btl.Enemy.Data.info.die_atk == 0)
					btl_util.SetEnemyDieSound(btl.Data, btl.EnemyType.die_snd_no);
				return;
			}
			// TODO: have some system to handle critical damage and evade movements (in SBattleCalculator.CalcResult) for potential other movement conflicts
			if ((btl.Data.fig_info & Param.FIG_INFO_HP_CRITICAL) != 0)
				btl.Data.pos[2] += (((btl.Data.rot.eulerAngles[1] + 90) % 360) < 180 ? 400 : -400) >> 1;
			btl.Data.bi.dmg_mot_f = 1;

			if (btl.IsPlayer)
			{
				if (cmd != null && (cmd.AbilityType & 129) == 129)
				{
					btl_mot.setMotion(btl.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2);
					if (btl.Data.cur.hp == 0)
						btl.Data.die_seq = 1;
				}
				else
				{
					btl_mot.setMotion(btl.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1);
				}
				btl.Data.evt.animFrame = 0;
			}
			else if (btl.Data.cur.hp == 0 && btl.Enemy.Data.info.die_dmg != 0 && btl.Enemy.Data.info.die_atk == 0)
			{
				btl_mot.setMotion(btl.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2);
				btl.Data.evt.animFrame = 0;
				btl.Data.die_seq = 1;
			}
			else
			{
				if (btl.IsSlave)
					btl = btl_util.GetMasterEnemyBtlPtr();
				btl_mot.setMotion(btl.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1 + btl.Data.bi.def_idle);
				btl.Data.evt.animFrame = 0;
			}
		}

		public static void EndCommandMotion(CMD_DATA cmd)
		{
			// Ensure the dying sequence after a command (including BattleCommandId.EnemyDying)
			if (cmd.regist != null && cmd.regist.die_seq == 6)
				btl_sys.CheckBattlePhase(cmd.regist);
			if (cmd.cmd_no > BattleCommandId.EnemyReaction && cmd.cmd_no < BattleCommandId.ScriptCounter1)
				return;
			if (cmd.regist != null && Status.checkCurStat(cmd.regist, BattleStatus.Death) && cmd.regist.die_seq == 0)
			{
				if (cmd.regist.bi.player == 0 && btl_util.getEnemyPtr(cmd.regist).info.die_atk != 0 && cmd.cmd_no != BattleCommandId.EnemyDying)
					return;
				if (cmd.regist.bi.player == 0 && btl_mot.IsEnemyDyingAnimation(cmd.regist, cmd.regist.currentAnimationName))
					cmd.regist.die_seq = 2;
				else
					cmd.regist.die_seq = 1;
			}
		}

		public static Boolean ControlDamageMotion(CMD_DATA cmd) // Unused anymore
		{
			Boolean result = true;
			if (cmd.info.cover == 0)
			{
				for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
				{
					if (next.bi.dmg_mot_f != 0)
					{
						result = false;
						if (next.animEndFrame)
						{
							next.pos[2] = next.base_pos[2];
							if (next.bi.player != 0)
								btl_mot.PlayerDamageMotion(next);
							else
								btl_mot.EnemyDamageMotion(next);
							btl_mot.SetDefaultIdle(next);
						}
					}
				}
			}
			return result;
		}

		private static void PlayerDamageMotion(BTL_DATA btl) // Unused anymore
		{
			if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1))
			{
				if (Status.checkCurStat(btl, BattleStatus.Death))
				{
					btl.die_seq = 1;
					btl.bi.dmg_mot_f = 0;
				}
				else if (btl.bi.cmd_idle != 0)
				{
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_NORMAL_TO_CMD);
				}
				else
				{
					btl.bi.dmg_mot_f = 0;
				}
			}
			else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2))
			{
				if (Status.checkCurStat(btl, BattleStatus.Death))
				{
					btl.die_seq = 5;
					btl.bi.dmg_mot_f = 0;
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE);
				}
				else
				{
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DISABLE);
				}
			}
			else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DISABLE))
			{
				if (btl.bi.cmd_idle != 0)
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DYING_TO_CMD);
				else if (btl_stat.CheckStatus(btl, BattleStatus.IdleDying))
					btl.bi.dmg_mot_f = 0;
				else
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DYING);
			}
			else
			{
				btl.bi.dmg_mot_f = 0;
			}
			btl.evt.animFrame = 0;
		}

		public static void EnemyDamageMotion(BTL_DATA btl) // Unused anymore
		{
			btl.bi.dmg_mot_f = 0;
			if (Status.checkCurStat(btl, BattleStatus.Death))
			{
				if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2) && btl_util.getEnemyPtr(btl).info.die_dmg != 0)
				{
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE);
					btl.evt.animFrame--;
					btl.bi.stop_anim = 1;
					btl.die_seq = 3;
				}
				else if (btl_util.getEnemyPtr(btl).info.die_atk == 0 && btl.bi.death_f == 0)
				{
					btl.die_seq = 1;
				}
				else
				{
					btl_mot.setMotion(btl, btl.bi.def_idle);
				}
			}
		}

		public static void HideMesh(BTL_DATA btl, UInt16 mesh, Boolean isBanish = false)
		{
			Boolean flag = false;
			String path = (btl.dms_geo_id == -1) ? String.Empty : FF9BattleDB.GEO.GetValue((Int32)btl.dms_geo_id);
			if (ModelFactory.IsUseAsEnemyCharacter(path) && isBanish)
			{
				mesh = UInt16.MaxValue;
				flag = true;
			}
			for (Int32 i = 0; i < 16; i++)
				if (((Int32)mesh & 1 << i) != 0)
					geo.geoMeshHide(btl, i);
			if (mesh == 65535)
				for (Int32 i = 0; i < btl.weaponMeshCount; i++)
					geo.geoWeaponMeshHide(btl, i);
		}

		public static void ShowMesh(BTL_DATA btl, UInt16 mesh, Boolean isBanish = false)
		{
            String path = (btl.dms_geo_id == -1) ? String.Empty : FF9BattleDB.GEO.GetValue((Int32)btl.dms_geo_id);
            if (ModelFactory.IsUseAsEnemyCharacter(path) && isBanish)
				mesh = UInt16.MaxValue;
			if (btl.bi.player == 0)
				btl.flags &= (UInt16)~geo.GEO_FLAGS_RENDER;
			for (Int32 i = 0; i < 16; i++)
				if (((Int32)mesh & 1 << i) != 0)
					geo.geoMeshShow(btl, i);
			if (mesh == 65535)
				for (Int32 i = 0; i < btl.weaponMeshCount; i++)
					geo.geoWeaponMeshShow(btl, i);
		}

		public static void HideWeapon(BTL_DATA btl)
		{
			for (Int32 i = 0; i < btl.weaponMeshCount; i++)
				geo.geoWeaponMeshHide(btl, i);
		}

		public static void ShowWeapon(BTL_DATA btl)
		{
			for (Int32 i = 0; i < btl.weaponMeshCount; i++)
				geo.geoWeaponMeshShow(btl, i);
		}

		public static void SetPlayerDefMotion(BTL_DATA btl, UInt32 serial_no, UInt32 cnt)
		{
			FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
			for (UInt32 num = 0u; num < 34u; num += 1u)
				ff9Battle.p_mot[(Int32)((UIntPtr)cnt)][(Int32)((UIntPtr)num)] = btl_mot.mot[(Int32)((UIntPtr)serial_no), (Int32)((UIntPtr)num)];
			btl.mot = ff9Battle.p_mot[(Int32)((UIntPtr)cnt)];
			btl.animFlag = 0;
			btl.animSpeed = 1f;
			btl.animFrameFrac = 0f;
			btl.animEndFrame = false;
		}

		public static String[,] mot;
		public static BattlePlayerCharacter.PlayerMotionStance[,] mot_stance;
		public static HashSet<BattlePlayerCharacter.PlayerMotionIndex> unstoppable_mot;
	}
}
