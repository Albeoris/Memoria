using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Prime;
using Memoria.Prime.CSV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public static class ff9
{
    static ff9()
    {
        ff9.w_movementWaterStatus = new UInt32[2] { 0x23ED0000u, 0u };
        ff9.w_movementSinkArray = new Int16[,]
        {
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            {
                400,
                150,
                250,
                200,
                450,
                450,
                300,
                300,
                0
            },
            {
                0,
                300,
                300,
                300,
                300,
                300,
                300,
                300,
                0
            },
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            }
        };
        ff9.w_moveCHRCache = new ff9.s_moveCHRCache[11];
        ff9.w_moveDummyCharacter = null;
        ff9.w_isMogActive = false;
        ff9.honoUpdateTime = 0.05f;
        ff9.PrintDebug = true;
        ff9.w_cameraFocusAim = Vector3.zero;
        ff9.w_cameraHit = new ff9.s_moveCHRCache[]
        {
            new ff9.s_moveCHRCache(),
            new ff9.s_moveCHRCache(),
            new ff9.s_moveCHRCache(),
            new ff9.s_moveCHRCache()
        };
        ff9.w_cameraForcePlace = -1;
        ff9.w_cameraDebugDist = 4096;
        ff9.w_cameraArea2Place = new Byte[]
        {
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            1,
            1,
            1,
            1,
            1,
            1,
            0,
            0,
            0,
            0,
            0,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2
        };
        ff9.w_cameraPosstat = new ff9.s_cameraPosstat[]
        {
            new ff9.s_cameraPosstat
            {
                cameraPers = 320f,
                cameraDistance = 5000f,
                cameraHeight = -2200f,
                cameraCorrect = -3000f,
                aimHeight = -1000f
            },
            new ff9.s_cameraPosstat
            {
                cameraPers = 350f,
                cameraDistance = 6000f,
                cameraHeight = -3200f,
                cameraCorrect = -3000f,
                aimHeight = -1200f
            },
            new ff9.s_cameraPosstat
            {
                cameraPers = 350f,
                cameraDistance = 5500f,
                cameraHeight = -2400f,
                cameraCorrect = -3000f,
                aimHeight = -500f
            },
            new ff9.s_cameraPosstat
            {
                cameraPers = 350f,
                cameraDistance = 7000f,
                cameraHeight = -4400f,
                cameraCorrect = -500f,
                aimHeight = -1500f
            },
            new ff9.s_cameraPosstat
            {
                cameraPers = 320f,
                cameraDistance = 6750f,
                cameraHeight = -400f,
                cameraCorrect = -3600f,
                aimHeight = 0f
            },
            new ff9.s_cameraPosstat
            {
                cameraPers = 320f,
                cameraDistance = 6750f,
                cameraHeight = -400f,
                cameraCorrect = -3400f,
                aimHeight = 0f
            },
            new ff9.s_cameraPosstat
            {
                cameraPers = 320f,
                cameraDistance = 6750f,
                cameraHeight = -400f,
                cameraCorrect = -3600f,
                aimHeight = 0f
            },
            new ff9.s_cameraPosstat
            {
                cameraPers = 350f,
                cameraDistance = 4000f,
                cameraHeight = -7500f,
                cameraCorrect = -3000f,
                aimHeight = -1200f
            },
            new ff9.s_cameraPosstat
            {
                cameraPers = 320f,
                cameraDistance = 5000f,
                cameraHeight = -400f,
                cameraCorrect = -3500f,
                aimHeight = 0f
            },
            new ff9.s_cameraPosstat
            {
                cameraPers = 380f,
                cameraDistance = 7000f,
                cameraHeight = -2200f,
                cameraCorrect = -2400f,
                aimHeight = -1500f
            }
        };
        ff9.s_cameraElement[,] array2 = new ff9.s_cameraElement[5, 3];
        array2[0, 0] = new ff9.s_cameraElement
        {
            down = 0,
            up = 2,
            fly = 7
        };
        array2[0, 1] = new ff9.s_cameraElement
        {
            down = 3,
            up = 2,
            fly = 7
        };
        array2[0, 2] = new ff9.s_cameraElement
        {
            down = 1,
            up = 2,
            fly = 7
        };
        array2[1, 0] = new ff9.s_cameraElement
        {
            down = 1,
            up = 2,
            fly = 7
        };
        array2[1, 1] = new ff9.s_cameraElement
        {
            down = 3,
            up = 2,
            fly = 7
        };
        array2[1, 2] = new ff9.s_cameraElement
        {
            down = 1,
            up = 2,
            fly = 7
        };
        array2[2, 0] = new ff9.s_cameraElement
        {
            down = 9,
            up = 2,
            fly = 7
        };
        array2[2, 1] = new ff9.s_cameraElement
        {
            down = 9,
            up = 2,
            fly = 7
        };
        array2[2, 2] = new ff9.s_cameraElement
        {
            down = 9,
            up = 2,
            fly = 7
        };
        array2[3, 0] = new ff9.s_cameraElement
        {
            down = 6,
            up = 6,
            fly = 6
        };
        array2[3, 1] = new ff9.s_cameraElement
        {
            down = 5,
            up = 5,
            fly = 5
        };
        array2[3, 2] = new ff9.s_cameraElement
        {
            down = 4,
            up = 4,
            fly = 4
        };
        array2[4, 0] = new ff9.s_cameraElement
        {
            down = 8,
            up = 8,
            fly = 8
        };
        array2[4, 1] = new ff9.s_cameraElement
        {
            down = 8,
            up = 8,
            fly = 8
        };
        array2[4, 2] = new ff9.s_cameraElement
        {
            down = 8,
            up = 8,
            fly = 8
        };
        ff9.w_cameraElement = array2;
        ff9.nsp = 1f;
        ff9.sa0 = 1;
        ff9.sa1 = 4;
        ff9.sa2 = 0;
        ff9.sa3 = 1f;
        ff9.sa4 = 16;
        ff9.vn1 = 16384;
        ff9.w_evaCoreSPSCurrentSize = 46000;
        ff9.w_evaCoreSPSSpeedtSize = 300;
        ff9.w_effectModelPos = new Vector3[9];
        ff9.w_effectModelRot = new Vector3[9];
        ff9.w_effectMillPos = new Vector3(1094.296875f, 30.40625f, -806.50390625f);
        ff9.w_effectMillRot = Vector3.zero;
        ff9.w_effectTwisPos = new Vector3(894.55859375f, 0f, -777.05859375f);
        ff9.w_effectLastPos = new Vector3(767.9765625f, 42.5078125f, -320.3203125f);
        ff9.w_effectLastPos1 = new Vector3(767.9765625f, 46.875f, -320.3203125f);
        ff9.w_effectTwisRot = Vector3.zero;
        ff9.w_effectLast1Rot = Vector3.zero;
        ff9.w_effectLast2Rot = Vector3.zero;
        ff9.w_effectLast3Rot = Vector3.zero;
        ff9.w_effectLastRot = new Vector3[10];
        ff9.w_effectLastRotT = new Vector3[10];
        ff9.w_effectLastDis = new Int16[10];
        ff9.w_effectLastDisT = new Int16[10];
        ff9.w_effectLastDisC = new SByte[10];
        ff9.w_effectSpilRot = new Vector3[3];
        ff9.w_effectBlockAvail = new Boolean[13];
        ff9.w_fileImageSectorInfo = new ff9.sw_ImageSector[256];
        ff9.w_fileImageTopSector = new Int32[5];
        ff9.w_fileImagename = new String[5];
        ff9.w_fileImagenameServer1 = new String[]
        {
            "WorldMap/wmap/disc1/discmr.img",
            "WorldMap/wmap/disc1/pk_stat_.img",
            "WorldMap/wmap/disc1/pk_temp_.img",
            "WorldMap/wmap/disc1/model_hm.nwp",
            "WorldMap/wmap/disc1/model_vm.nwp"
        };
        ff9.w_fileImagenameServer4 = new String[]
        {
            "WorldMap/wmap/disc4/discmr.img",
            "WorldMap/wmap/disc4/pk_stat_.img",
            "WorldMap/wmap/disc4/pk_temp_.img",
            "WorldMap/wmap/disc4/model_hm.nwp",
            "WorldMap/wmap/disc4/model_vm.nwp"
        };
        ff9.w_frameLanguage = 0;
        ff9.w_frameTime = 0;
        ff9.w_frameStatus = 2;
        ff9.w_frameWire = 0;
        ff9.w_frameMenuon = false;
        ff9.w_frameFog = 0;
        ff9.w_frameServer = 1;
        ff9.w_frameEventEnable = true;
        ff9.w_frameDebugLevel = 0u;
        ff9.w_frameEncountEnable = false;
        ff9.w_frameEncountMask = true;
        ff9.w_frameScriptParam = new Int32[4];
        ff9.w_frameLoc = false;
        ff9.w_framePacketPeak = 0u;
        ff9.w_framePacketPer = 0u;
        ff9.w_frameEffectTest = 0;
        ff9.w_frameEffectSize = 4096;
        ff9.w_frameEncountTrue = true;
        ff9.w_frameError = 0;
        ff9.w_frameDebugMode = false;
        ff9.w_frameDebugVRAM = 0;
        ff9.w_frameLine = 0u;
        ff9.kframeEventStartLoop = 4;
        ff9.w_light = new Light[3];
        ff9.kFF9FogCharHI = -20.62109375f;
        ff9.kFF9FogCharLOW = -12.12109375f;
        ff9.p1 = 4;
        ff9.p2 = 8;
        ff9.p3 = 32;
        ff9.WH1 = 8f;
        ff9.WH1b = 19.53125f;
        ff9.SH1 = 7.03125f;
        ff9.WH2 = 0.390625f;
        ff9.WH2b = 19.53125f;
        ff9.SH2 = 7.03125f;
        ff9.cn1 = 32f;
        ff9.cn2 = 0f;
        ff9.cn3 = 54.6875f;
        ff9.cn4 = 256f;
        ff9.cn5 = 1.5f;
        ff9.forceUsingMobileInput = false;
        ff9.w_musicSet = new Byte[]
        {
            69,		// Over the Hill
            100,	// Ukulele de Chocobo
            112,	// Aboard the Hilda Garde
            45,		// Another Nightmare
            95,		// Kuja's Theme
            96,		// Desert Palace
            61,		// Faerie Battle
            62		// Steiner's Delusion
        };
        ff9.navipos[,] array3 = new ff9.navipos[2, 64];
        array3[0, 0] = new ff9.navipos
        {
            vx = 190,
            vy = 48,
            tx = 345560,
            ty = -173564
        };
        array3[0, 1] = new ff9.navipos
        {
            vx = 173,
            vy = 55,
            tx = 328680,
            ty = -178910
        };
        array3[0, 2] = new ff9.navipos
        {
            vx = 156,
            vy = 68,
            tx = 311680,
            ty = -191323
        };
        array3[0, 3] = new ff9.navipos
        {
            vx = 150,
            vy = 81,
            tx = 306501,
            ty = -204700
        };
        array3[0, 4] = new ff9.navipos
        {
            vx = 205,
            vy = 115,
            tx = 358057,
            ty = -236117
        };
        array3[0, 5] = new ff9.navipos
        {
            vx = 173,
            vy = 125,
            tx = 328239,
            ty = -245345
        };
        array3[0, 6] = new ff9.navipos
        {
            vx = 149,
            vy = 104,
            tx = 300090,
            ty = -230918
        };
        array3[0, 7] = new ff9.navipos
        {
            vx = 148,
            vy = 109,
            tx = 300090,
            ty = -230918
        };
        array3[0, 8] = new ff9.navipos
        {
            vx = 142,
            vy = 105,
            tx = 300090,
            ty = -230918
        };
        array3[0, 9] = new ff9.navipos
        {
            vx = 138,
            vy = 111,
            tx = 300090,
            ty = -230918
        };
        array3[0, 10] = new ff9.navipos
        {
            vx = 135,
            vy = 119,
            tx = 300090,
            ty = -230918
        };
        array3[0, 11] = new ff9.navipos
        {
            vx = 140,
            vy = 93,
            tx = 297056,
            ty = -215824
        };
        array3[0, 12] = new ff9.navipos
        {
            vx = 127,
            vy = 81,
            tx = 284585,
            ty = -203240
        };
        array3[0, 13] = new ff9.navipos
        {
            vx = 122,
            vy = 84,
            tx = 280253,
            ty = -206433
        };
        array3[0, 14] = new ff9.navipos
        {
            vx = 113,
            vy = 71,
            tx = 265584,
            ty = -193337
        };
        array3[0, 15] = new ff9.navipos
        {
            vx = 96,
            vy = 70,
            tx = 265584,
            ty = -193337
        };
        array3[0, 16] = new ff9.navipos
        {
            vx = 93,
            vy = 83,
            tx = 254112,
            ty = -205731
        };
        array3[0, 17] = new ff9.navipos
        {
            vx = 85,
            vy = 57,
            tx = 246186,
            ty = -182059
        };
        array3[0, 18] = new ff9.navipos
        {
            vx = 68,
            vy = 76,
            tx = 229548,
            ty = -198479
        };
        array3[0, 19] = new ff9.navipos
        {
            vx = 121,
            vy = 125,
            tx = 278526,
            ty = -243814
        };
        array3[0, 20] = new ff9.navipos
        {
            vx = 90,
            vy = 103,
            tx = 250721,
            ty = -224068
        };
        array3[0, 21] = new ff9.navipos
        {
            vx = 77,
            vy = 121,
            tx = 239319,
            ty = -241340
        };
        array3[0, 22] = new ff9.navipos
        {
            vx = 80,
            vy = 161,
            tx = 241248,
            ty = -279572
        };
        array3[0, 23] = new ff9.navipos
        {
            vx = 72,
            vy = 143,
            tx = 234496,
            ty = -260825
        };
        array3[0, 24] = new ff9.navipos
        {
            vx = 67,
            vy = 160,
            tx = 229342,
            ty = -278220
        };
        array3[0, 25] = new ff9.navipos
        {
            vx = 60,
            vy = 163,
            tx = 221494,
            ty = -282188
        };
        array3[0, 26] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 27] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 28] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 29] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 30] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 31] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 32] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 33] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 34] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 35] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 36] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 37] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 38] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 39] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 40] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 41] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 42] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 43] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 44] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 45] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 46] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 47] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 48] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 49] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 50] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 51] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 52] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 53] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 54] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 55] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 56] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 57] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 58] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 59] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 60] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 61] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 62] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[0, 63] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[1, 0] = new ff9.navipos
        {
            vx = 211,
            vy = 106,
            tx = 345560,
            ty = -173564
        };
        array3[1, 1] = new ff9.navipos
        {
            vx = 200,
            vy = 111,
            tx = 328680,
            ty = -178910
        };
        array3[1, 2] = new ff9.navipos
        {
            vx = 190,
            vy = 117,
            tx = 311680,
            ty = -191323
        };
        array3[1, 3] = new ff9.navipos
        {
            vx = 188,
            vy = 127,
            tx = 306501,
            ty = -204700
        };
        array3[1, 4] = new ff9.navipos
        {
            vx = 219,
            vy = 144,
            tx = 358057,
            ty = -236117
        };
        array3[1, 5] = new ff9.navipos
        {
            vx = 201,
            vy = 150,
            tx = 328239,
            ty = -245345
        };
        array3[1, 6] = new ff9.navipos
        {
            vx = 183,
            vy = 141,
            tx = 300090,
            ty = -230918
        };
        array3[1, 7] = new ff9.navipos
        {
            vx = 183,
            vy = 141,
            tx = 300090,
            ty = -230918
        };
        array3[1, 8] = new ff9.navipos
        {
            vx = 183,
            vy = 141,
            tx = 300090,
            ty = -230918
        };
        array3[1, 9] = new ff9.navipos
        {
            vx = 183,
            vy = 141,
            tx = 300090,
            ty = -230918
        };
        array3[1, 10] = new ff9.navipos
        {
            vx = 183,
            vy = 141,
            tx = 300090,
            ty = -230918
        };
        array3[1, 11] = new ff9.navipos
        {
            vx = 181,
            vy = 132,
            tx = 297056,
            ty = -215824
        };
        array3[1, 12] = new ff9.navipos
        {
            vx = 176,
            vy = 124,
            tx = 284585,
            ty = -203240
        };
        array3[1, 13] = new ff9.navipos
        {
            vx = 172,
            vy = 127,
            tx = 280253,
            ty = -206433
        };
        array3[1, 14] = new ff9.navipos
        {
            vx = 163,
            vy = 118,
            tx = 265584,
            ty = -193337
        };
        array3[1, 15] = new ff9.navipos
        {
            vx = 163,
            vy = 118,
            tx = 265584,
            ty = -193337
        };
        array3[1, 16] = new ff9.navipos
        {
            vx = 156,
            vy = 126,
            tx = 254112,
            ty = -205731
        };
        array3[1, 17] = new ff9.navipos
        {
            vx = 152,
            vy = 111,
            tx = 246186,
            ty = -182059
        };
        array3[1, 18] = new ff9.navipos
        {
            vx = 140,
            vy = 123,
            tx = 229548,
            ty = -198479
        };
        array3[1, 19] = new ff9.navipos
        {
            vx = 170,
            vy = 149,
            tx = 278526,
            ty = -243814
        };
        array3[1, 20] = new ff9.navipos
        {
            vx = 153,
            vy = 137,
            tx = 250721,
            ty = -224068
        };
        array3[1, 21] = new ff9.navipos
        {
            vx = 147,
            vy = 148,
            tx = 239319,
            ty = -241340
        };
        array3[1, 22] = new ff9.navipos
        {
            vx = 147,
            vy = 170,
            tx = 241248,
            ty = -279572
        };
        array3[1, 23] = new ff9.navipos
        {
            vx = 143,
            vy = 161,
            tx = 234496,
            ty = -260825
        };
        array3[1, 24] = new ff9.navipos
        {
            vx = 140,
            vy = 170,
            tx = 229342,
            ty = -278220
        };
        array3[1, 25] = new ff9.navipos
        {
            vx = 135,
            vy = 172,
            tx = 221494,
            ty = -282188
        };
        array3[1, 26] = new ff9.navipos
        {
            vx = 183,
            vy = 56,
            tx = 299041,
            ty = -94130
        };
        array3[1, 27] = new ff9.navipos
        {
            vx = 185,
            vy = 44,
            tx = 303345,
            ty = -71006
        };
        array3[1, 28] = new ff9.navipos
        {
            vx = 170,
            vy = 16,
            tx = 275329,
            ty = -27154
        };
        array3[1, 29] = new ff9.navipos
        {
            vx = 160,
            vy = 48,
            tx = 262598,
            ty = -79454
        };
        array3[1, 30] = new ff9.navipos
        {
            vx = 150,
            vy = 61,
            tx = 244288,
            ty = -100007
        };
        array3[1, 31] = new ff9.navipos
        {
            vx = 144,
            vy = 63,
            tx = 235576,
            ty = -102928
        };
        array3[1, 32] = new ff9.navipos
        {
            vx = 141,
            vy = 55,
            tx = 229037,
            ty = -90135
        };
        array3[1, 33] = new ff9.navipos
        {
            vx = 142,
            vy = 30,
            tx = 233101,
            ty = -48649
        };
        array3[1, 34] = new ff9.navipos
        {
            vx = 138,
            vy = 36,
            tx = 226757,
            ty = -59696
        };
        array3[1, 35] = new ff9.navipos
        {
            vx = 133,
            vy = 43,
            tx = 217668,
            ty = -70533
        };
        array3[1, 36] = new ff9.navipos
        {
            vx = 119,
            vy = 50,
            tx = 200097,
            ty = -82107
        };
        array3[1, 37] = new ff9.navipos
        {
            vx = 93,
            vy = 177,
            tx = 151941,
            ty = -289631
        };
        array3[1, 38] = new ff9.navipos
        {
            vx = 80,
            vy = 139,
            tx = 131348,
            ty = -231368
        };
        array3[1, 39] = new ff9.navipos
        {
            vx = 59,
            vy = 161,
            tx = 97345,
            ty = -262526
        };
        array3[1, 40] = new ff9.navipos
        {
            vx = 48,
            vy = 147,
            tx = 78285,
            ty = -239115
        };
        array3[1, 41] = new ff9.navipos
        {
            vx = 61,
            vy = 125,
            tx = 101014,
            ty = -204347
        };
        array3[1, 42] = new ff9.navipos
        {
            vx = 52,
            vy = 89,
            tx = 83497,
            ty = -145686
        };
        array3[1, 43] = new ff9.navipos
        {
            vx = 35,
            vy = 95,
            tx = 57147,
            ty = -155460
        };
        array3[1, 44] = new ff9.navipos
        {
            vx = 30,
            vy = 74,
            tx = 49042,
            ty = -121464
        };
        array3[1, 45] = new ff9.navipos
        {
            vx = 54,
            vy = 77,
            tx = 87500,
            ty = -125280
        };
        array3[1, 46] = new ff9.navipos
        {
            vx = 71,
            vy = 49,
            tx = 116381,
            ty = -81633
        };
        array3[1, 47] = new ff9.navipos
        {
            vx = 58,
            vy = 37,
            tx = 95358,
            ty = -61173
        };
        array3[1, 48] = new ff9.navipos
        {
            vx = 79,
            vy = 18,
            tx = 129014,
            ty = -29732
        };
        array3[1, 49] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[1, 50] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[1, 51] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[1, 52] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[1, 53] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[1, 54] = new ff9.navipos
        {
            vx = 203,
            vy = 100,
            tx = 334274,
            ty = -164988
        };
        array3[1, 55] = new ff9.navipos
        {
            vx = 194,
            vy = 64,
            tx = 320846,
            ty = -111720
        };
        array3[1, 56] = new ff9.navipos
        {
            vx = 73,
            vy = 97,
            tx = 119880,
            ty = -159015
        };
        array3[1, 57] = new ff9.navipos
        {
            vx = 74,
            vy = 167,
            tx = 120775,
            ty = -275601
        };
        array3[1, 58] = new ff9.navipos
        {
            vx = 114,
            vy = 94,
            tx = 187061,
            ty = -151788
        };
        array3[1, 59] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[1, 60] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[1, 61] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[1, 62] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        array3[1, 63] = new ff9.navipos
        {
            vx = 0,
            vy = 0,
            tx = 0,
            ty = 0
        };
        ff9.w_naviLocationPos = array3;
        ff9.WorldTitleFadeInMode = 1;
        ff9.WorldTitleFadeOutMode = 2;
        ff9.WorldTitleCloseMode = 3;
        ff9.lastTitleDrawState = 0;
        ff9.ggammawork = new ff9.CVECTOR[256];
        ff9.w_nwpZDepth = new Byte[1024];
        ff9.w_nwbAreaPage = new ff9.sNWBPage[17];
        ff9.w_nwbColor = new ff9.sNWBColor[2];
        ff9.rayStartOffsetYFromSky = 400f;
        ff9.rayStartOffsetY = 2.34375f;
        ff9.rayDistance = 2.8f;
        ff9.defaultHeight = 0f;
        ff9.w_texturePixelAnimCnt = new SByte[2];
        ff9.w_texturePixelAnimPat = new SByte[]
        {
            0,
            1,
            2,
            3,
            2,
            1
        };
        ff9.w_texturePixelScrollWork = new Int16[8];
        ff9.w_texturePaletScrollWork = new Int16[18];
        ff9.w_textureWork = new UInt16[2, 2048];
        ff9.w_textureWorkOffset = new UInt16[18];
        ff9.w_textureWorkScroll = new Byte[18];
        ff9.volcanoPosition = new Vector3(505.4f, 9.1f, -113.8f);
        ff9.w_weatherColor = new ff9.sw_weatherColor();
        ff9.w_worldAreaZone = new Byte[]
        {
            0,
            0,
            1,
            1,
            2,
            2,
            2,
            2,
            3,
            3,
            4,
            4,
            5,
            5,
            6,
            7,
            7,
            7,
            7,
            7,
            7,
            7,
            8,
            9,
            9,
            9,
            10,
            11,
            11,
            11,
            11,
            11,
            12,
            12,
            13,
            13,
            14,
            14,
            14,
            14,
            15,
            15,
            15,
            16,
            16,
            17,
            18,
            18,
            18,
            18,
            18,
            19,
            19,
            19,
            19,
            20,
            20,
            21,
            21,
            21,
            22,
            22,
            23,
            24
        };
        ff9.w_worldZoneFigure = new Byte[]
        {
            3,
            3,
            7,
            4,
            5,
            7,
            2,
            6,
            2,
            9,
            2,
            7,
            6,
            4,
            7,
            10,
            5,
            7,
            7,
            6,
            4,
            6,
            4,
            2,
            2,
            0
        };
        ff9.w_worldZoneInfo = new Byte[26];
        ff9.w_worldLocDistance = new Int32[3];
        ff9.w_worldLocX = new Int32[]
        {
            229279, // Cleyra
            130781, // Wind Shrine
            299201  // Earth Shrine
        };
        ff9.w_worldLocZ = new Int32[]
        {
            -198701,
            -227819,
            -93859
        };
        ff9.w_worldLocSENum = new Byte[]
        {
            26,
            26,
            25
        };
        ff9.w_worldLocSEFlg = new Byte[3];
        for (Int32 i = 0; i < ff9.w_moveCHRCache.Length; i++)
            ff9.w_moveCHRCache[i] = new ff9.s_moveCHRCache();
        ff9.w_moveCHRControl = new ff9.s_moveCHRControl[12]
        {
            new ff9.s_moveCHRControl // 0: walking by foot
            {
                type = 0,
                flg_gake = 0,
                speed_move = 112,
                speed_rotation = 64,
                speed_updown = 0,
                speed_roll = 0,
                speed_rollback = 0,
                flg_fly = false,
                flg_upcam = true,
                fetchdist = 2,
                music = 0,
                se = 0,
                encount = true,
                radius = 0,
                camrot = true,
                pad2 = 0,
                limit = new UInt32[]
                {
                    0x0010667Fu,
                    0xD8FF3CFFu
                }
            },
            new ff9.s_moveCHRControl // 1: Yellow Chocobo
            {
                type = 0,
                flg_gake = 0,
                speed_move = 200,
                speed_rotation = 64,
                speed_updown = 0,
                speed_roll = 0,
                speed_rollback = 0,
                flg_fly = false,
                flg_upcam = true,
                fetchdist = 2,
                music = 1,
                se = 0,
                encount = false,
                radius = 300,
                camrot = true,
                pad2 = 0,
                limit = new UInt32[]
                {
                    0x0010667Fu,
                    0xD8FF3CFFu
                }
            },
            new ff9.s_moveCHRControl // 2: Light Blue Chocobo
            {
                type = 0,
                flg_gake = 1,
                speed_move = 200,
                speed_rotation = 64,
                speed_updown = 0,
                speed_roll = 0,
                speed_rollback = 0,
                flg_fly = false,
                flg_upcam = true,
                fetchdist = 2,
                music = 1,
                se = 0,
                encount = false,
                radius = 300,
                camrot = true,
                pad2 = 0,
                limit = new UInt32[]
                {
                    0x20F8667Fu,
                    0xD8FF3CFFu
                }
            },
            new ff9.s_moveCHRControl // 3: Red Chocobo
            {
                type = 0,
                flg_gake = 1,
                speed_move = 200,
                speed_rotation = 64,
                speed_updown = 0,
                speed_roll = 0,
                speed_rollback = 0,
                flg_fly = false,
                flg_upcam = true,
                fetchdist = 2,
                music = 1,
                se = 0,
                encount = false,
                radius = 300,
                camrot = true,
                pad2 = 0,
                limit = new UInt32[]
                {
                    0x20FE667Fu,
                    0xD8FF3CFFu
                }
            },
            new ff9.s_moveCHRControl // 4: Deep Blue Chocobo
            {
                type = 0,
                flg_gake = 1,
                speed_move = 200,
                speed_rotation = 64,
                speed_updown = 0,
                speed_roll = 0,
                speed_rollback = 0,
                flg_fly = false,
                flg_upcam = true,
                fetchdist = 2,
                music = 1,
                se = 0,
                encount = false,
                radius = 300,
                camrot = true,
                pad2 = 0,
                limit = new UInt32[]
                {
                    0x23FF667Fu,
                    0xD8FF3CFFu
                }
            },
            new ff9.s_moveCHRControl // 5: Gold Chocobo (ground)
            {
                type = 0,
                flg_gake = 1,
                speed_move = 200,
                speed_rotation = 64,
                speed_updown = 0,
                speed_roll = 0,
                speed_rollback = 0,
                flg_fly = false,
                flg_upcam = true,
                fetchdist = 2,
                music = 1,
                se = 0,
                encount = false,
                radius = 300,
                camrot = true,
                pad2 = 0,
                limit = new UInt32[]
                {
                    0x23FF667Fu,
                    0xD8FF3CFFu
                }
            },
            new ff9.s_moveCHRControl // 6: Gold Chocobo (air)
            {
                type = 1,
                flg_gake = 0,
                speed_move = 300,
                speed_rotation = 30,
                speed_updown = 128,
                speed_roll = 64,
                speed_rollback = 0,
                flg_fly = true,
                flg_upcam = false,
                fetchdist = 3,
                music = 1,
                se = 0,
                encount = false,
                radius = 400,
                camrot = false,
                type_cam = 4,
                pad2 = 0,
                limit = new UInt32[]
                {
                    0x7FFF667Fu,
                    0xD8FF3CFFu
                }
            },
            new ff9.s_moveCHRControl // 7: Blue Narciss
            {
                type = 2,
                flg_gake = 0,
                speed_move = 240,
                speed_rotation = 36,
                speed_updown = 0,
                speed_roll = 0,
                speed_rollback = 0,
                flg_fly = false,
                flg_upcam = true,
                fetchdist = 3,
                music = 0,
                se = 24,
                encount = false,
                radius = 560,
                camrot = false,
                pad2 = 0,
                limit = new UInt32[]
                {
                        0x2600000u,
                        0u
                }
            },
            new ff9.s_moveCHRControl // 8: Hilda Garde III
            {
                type = 1,
                flg_gake = 0,
                speed_move = 500,
                speed_rotation = 13,
                speed_updown = Byte.MaxValue,
                speed_roll = SByte.MaxValue,
                speed_rollback = 0,
                flg_fly = true,
                flg_upcam = false,
                fetchdist = 4,
                music = 2,
                se = 23,
                encount = false,
                radius = 640,
                camrot = false,
                pad2 = 0,
                limit = new UInt32[]
                {
                    0x7FFF667Fu,
                    0xD8FF3CFFu
                }
            },
            new ff9.s_moveCHRControl // 9: Invincible
            {
                type = 1,
                flg_gake = 0,
                speed_move = 500,
                speed_rotation = 13,
                speed_updown = Byte.MaxValue,
                speed_roll = SByte.MaxValue,
                speed_rollback = 0,
                flg_fly = true,
                flg_upcam = false,
                fetchdist = 4,
                music = 2,
                se = 22,
                encount = false,
                radius = 720,
                camrot = false,
                pad2 = 0,
                limit = new UInt32[]
                {
                    0x7FFF667Fu,
                    0xD8FF3CFFu
                }
            },
            new ff9.s_moveCHRControl // 10: ???
            {
                type = 3,
                flg_gake = 0,
                speed_move = 140,
                speed_rotation = 32,
                speed_updown = Byte.MaxValue,
                speed_roll = 0,
                speed_rollback = 0,
                flg_fly = false,
                flg_upcam = true,
                fetchdist = 3,
                music = 0,
                se = 0,
                encount = false,
                radius = 480,
                camrot = false,
                pad2 = 0,
                limit = new UInt32[]
                {
                    UInt32.MaxValue,
                    UInt32.MaxValue
                }
            },
            new ff9.s_moveCHRControl // 11: ???
            {
                type = 0,
                flg_gake = 0,
                speed_move = 0,
                speed_rotation = 0,
                speed_updown = 0,
                speed_roll = 0,
                speed_rollback = 0,
                flg_fly = false,
                flg_upcam = false,
                fetchdist = 0,
                music = 0,
                se = 0,
                encount = false,
                radius = 0,
                camrot = false,
                pad2 = 0,
                limit = new UInt32[2]
            }
        };
        if (FF9StateSystem.World.FixTypeCam)
        {
            ff9.w_moveCHRControl[0].type_cam = 0;
            ff9.w_moveCHRControl[1].type_cam = 1;
            ff9.w_moveCHRControl[2].type_cam = 1;
            ff9.w_moveCHRControl[3].type_cam = 1;
            ff9.w_moveCHRControl[4].type_cam = 1;
            ff9.w_moveCHRControl[5].type_cam = 1;
            ff9.w_moveCHRControl[6].type_cam = 4;
            ff9.w_moveCHRControl[7].type_cam = 2;
            ff9.w_moveCHRControl[8].type_cam = 3;
            ff9.w_moveCHRControl[9].type_cam = 3;
            ff9.w_moveCHRControl[10].type_cam = 0;
        }
        ff9.w_moveCHRStatus = new ff9.s_moveCHRStatus[22]
        {
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 0,
                shadow_amp = 0,
                flg_fly = 0,
                control = 11,
                cache = 10,
                shadow_bone = 0,
                shadow_offset = 0,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 1,
                shadow_size = 6,
                shadow_amp = 36,
                flg_fly = 0,
                control = 0,
                cache = 0,
                shadow_bone = 1,
                shadow_offset = -20,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 1,
                shadow_size = 6,
                shadow_amp = 36,
                flg_fly = 0,
                control = 0,
                cache = 0,
                shadow_bone = 1,
                shadow_offset = -20,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 1,
                shadow_size = 10,
                shadow_amp = 36,
                flg_fly = 0,
                control = 1,
                cache = 1,
                shadow_bone = 1,
                shadow_offset = -20,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 1,
                shadow_size = 10,
                shadow_amp = 36,
                flg_fly = 0,
                control = 2,
                cache = 1,
                shadow_bone = 1,
                shadow_offset = -20,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 1,
                shadow_size = 10,
                shadow_amp = 36,
                flg_fly = 0,
                control = 3,
                cache = 1,
                shadow_bone = 1,
                shadow_offset = -20,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 1,
                shadow_size = 10,
                shadow_amp = 36,
                flg_fly = 0,
                control = 4,
                cache = 1,
                shadow_bone = 1,
                shadow_offset = -20,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 1,
                shadow_size = 10,
                shadow_amp = 36,
                flg_fly = 0,
                control = 5,
                cache = 1,
                shadow_bone = 1,
                shadow_offset = -20,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 2,
                shadow_size = 16,
                shadow_amp = 36,
                flg_fly = 0,
                control = 7,
                cache = 2,
                shadow_bone = 1,
                shadow_offset = 0,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 16,
                shadow_amp = 56,
                flg_fly = 1,
                control = 8,
                cache = 2,
                shadow_bone = 1,
                shadow_offset = -20,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 16,
                shadow_amp = 56,
                flg_fly = 1,
                control = 9,
                cache = 2,
                shadow_bone = 1,
                shadow_offset = -20,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 1,
                shadow_size = 6,
                shadow_amp = 36,
                flg_fly = 0,
                control = 11,
                cache = 3,
                shadow_bone = 1,
                shadow_offset = 0,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 6,
                shadow_amp = 36,
                flg_fly = 2,
                control = 11,
                cache = 9,
                shadow_bone = 1,
                shadow_offset = 0,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 1,
                shadow_size = 0,
                shadow_amp = 0,
                flg_fly = 2,
                control = 11,
                cache = 10,
                shadow_bone = 1,
                shadow_offset = 0,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 30,
                shadow_amp = 100,
                flg_fly = 0,
                control = 11,
                cache = 10,
                shadow_bone = 1,
                shadow_offset = 0,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 16,
                shadow_amp = 36,
                flg_fly = 1,
                control = 11,
                cache = 4,
                shadow_bone = 2,
                shadow_offset = 0,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 16,
                shadow_amp = 36,
                flg_fly = 1,
                control = 11,
                cache = 4,
                shadow_bone = 1,
                shadow_offset = 0,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 12,
                shadow_amp = 36,
                flg_fly = 1,
                control = 11,
                cache = 4,
                shadow_bone = 2,
                shadow_offset = 40,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 12,
                shadow_amp = 36,
                flg_fly = 1,
                control = 11,
                cache = 5,
                shadow_bone = 2,
                shadow_offset = 40,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 12,
                shadow_amp = 36,
                flg_fly = 1,
                control = 11,
                cache = 6,
                shadow_bone = 2,
                shadow_offset = 40,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 12,
                shadow_amp = 36,
                flg_fly = 1,
                control = 11,
                cache = 7,
                shadow_bone = 2,
                shadow_offset = 40,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            },
            new ff9.s_moveCHRStatus
            {
                slice_type = 0,
                shadow_size = 12,
                shadow_amp = 36,
                flg_fly = 1,
                control = 11,
                cache = 8,
                shadow_bone = 2,
                shadow_offset = 40,
                id = 0,
                slice_height = 0f,
                ground_height = 0f
            }
        };
    }

    public static void setVector(ref Vector3 v, Single _x, Single _y, Single _z)
    {
        v.x = _x;
        v.y = _y;
        v.z = _z;
    }

    public static void VectorNormal(ref Vector3 v0, out Vector3 v1)
    {
        v1 = v0.normalized;
    }

    /*
    public static void VectorNormalS(ref Vector3 v0, out Vector3 v1)
    {
        v1 = v0.normalized;
    }
    */

    public static Single ratan2(Single y, Single x)
    {
        Single num = Mathf.Atan2(y, x);
        return num * 57.29578f;
    }

    public static Single rsin(Single a)
    {
        Single f = a * 0.0174532924f;
        return Mathf.Sin(f);
    }

    public static Single rcos(Single a)
    {
        Single f = a * 0.0174532924f;
        return Mathf.Cos(f);
    }

    public static Single SquareRoot0(Single a)
    {
        return Mathf.Sqrt(a);
    }

    public static Int32 rsin(Int32 fixedPointAngle)
    {
        Single degree = fixedPointAngle / 4096f * 360f;
        degree *= 0.0174532924f;
        Single sinRadian = Mathf.Sin(degree);
        return (Int32)(sinRadian * 4096f);
    }

    public static Int32 rcos(Int32 fixedPointAngle)
    {
        Single degree = fixedPointAngle / 4096f * 360f;
        degree *= 0.0174532924f;
        Single cosRadian = Mathf.Cos(degree);
        return (Int32)(cosRadian * 4096f);
    }

    public static Single abs(Single _X)
    {
        return Mathf.Abs(_X);
    }

    public static Int32 abs(Int32 _X)
    {
        return Math.Abs(_X);
    }

    public static Vector3 ApplyMatrixLV(ref Matrix4x4 m, ref Vector3 v0, out Vector3 v1)
    {
        v1 = m.MultiplyVector(v0);
        return v1;
    }

    public static void gte_OuterProduct12(ref Vector3 v0, ref Vector3 v1, out Vector3 v2)
    {
        v2 = Vector3.Cross(v0, v1);
    }

    /*
    public static void InitWipe_WorldMap()
    {
    }

    public static void ServiceWipe_WorldMap()
    {
    }

    public static void StartWipe_WorldMap(Int32 type, Int32 mode, Int32 frame, Int64 color)
    {
    }

    public static Boolean isPosObj(Obj obj)
    {
        return obj.cid == 4;
    }
    */

    public static WMActor GetControlChar()
    {
        if (FF9StateSystem.World.IsBeeScene)
        {
            if (ff9.w_moveCHRControlPtr == null)
                ff9.w_moveCHRControlPtr = ff9.w_moveCHRControl[ff9.w_moveCHRControl_No];
            return WMActor.ControlledDebugDebugActor;
        }
        if (PersistenSingleton<EventEngine>.Instance.GetControlChar() != null)
            return ((Actor)PersistenSingleton<EventEngine>.Instance.GetControlChar()).wmActor;
        return null;
    }

    public static PosObj GetControlChar_PosObj()
    {
        if (FF9StateSystem.World.IsBeeScene)
        {
            if (ff9.w_moveCHRControlPtr == null)
                ff9.w_moveCHRControlPtr = ff9.w_moveCHRControl[ff9.w_moveCHRControl_No];
            return WMActor.ControlledDebugDebugActor.originalActor;
        }
        return PersistenSingleton<EventEngine>.Instance.GetControlChar();
    }

    public static Boolean GetUserControl()
    {
        return FF9StateSystem.World.IsBeeScene || PersistenSingleton<EventEngine>.Instance.GetUserControl();
    }

    public static Boolean objIsVisible(Obj obj)
    {
        return FF9StateSystem.World.IsBeeScene || PersistenSingleton<EventEngine>.Instance.objIsVisible(obj);
    }

    public static ObjList GetActiveObjList()
    {
        if (FF9StateSystem.World.IsBeeScene)
            return ff9.world.ActorList;
        return PersistenSingleton<EventEngine>.Instance.GetActiveObjList();
    }

    public static Boolean GetEventAim()
    {
        return !FF9StateSystem.World.IsBeeScene && PersistenSingleton<EventEngine>.Instance.GetEventAim() != null;
    }

    public static Boolean GetEventEye()
    {
        return !FF9StateSystem.World.IsBeeScene && PersistenSingleton<EventEngine>.Instance.GetEventEye() != null;
    }

    public static Int32 WorldEvent(Int32 x, Int32 z, Int32 id)
    {
        Int32 num = 0x8000 | (z << 8 & 0x3F00) | (x << 2 & 0xFC) | (id & 3);
        if (!FF9StateSystem.World.IsBeeScene)
        {
            if (ff9.w_isMogActive)
                return num;
            PersistenSingleton<EventEngine>.Instance.Request(PersistenSingleton<EventEngine>.Instance.FindObjByUID(0), 1, num, false);
        }
        return num;
    }

    /*
    public static Int32 WorldEventDebug(Int32 x, Int32 z, Int32 id)
    {
        return 0x8000 | (z << 8 & 0x3F00) | (x << 2 & 0xFC) | (id & 3);
    }
    */

    public static Int32 ServiceEvents()
    {
        if (FF9StateSystem.World.IsBeeScene)
            return 0;
        return PersistenSingleton<EventEngine>.Instance.ServiceEvents();
    }

    public static Byte byte_gEventGlobal(Int32 index)
    {
        return FF9StateSystem.EventState.gEventGlobal[index];
    }

    public static Byte byte_gEventGlobal_keventNaviModeNo()
    {
        if (FF9StateSystem.World.IsBeeScene)
            return 0;
        return FF9StateSystem.EventState.gEventGlobal[100];
    }

    public static void byte_gEventGlobal_updateNaviMode()
    {
        FF9StateSystem.EventState.gEventGlobal[100] = (Byte)ff9.w_naviMode;
    }

    public static void byte_gEventGlobal_Write(Int32 index, Byte value)
    {
        FF9StateSystem.EventState.gEventGlobal[index] = value;
    }

    /*
    public static void short_gEventGlobal_Write(Int32 index, Int16 value)
    {
        FF9StateSystem.EventState.gEventGlobal[index] = (Byte)(value & 255);
        FF9StateSystem.EventState.gEventGlobal[index + 1] = (Byte)(value >> 8 & 255);
    }
    */

    public static void ushort_gEventGlobal_Write(Int32 index, UInt16 value)
    {
        FF9StateSystem.EventState.gEventGlobal[index] = (Byte)(value & 255);
        FF9StateSystem.EventState.gEventGlobal[index + 1] = (Byte)(value >> 8 & 255);
    }

    public static UInt16 ushort_gEventGlobal(Int32 index)
    {
        if (FF9StateSystem.World.IsBeeScene && index == 0)
        {
            Int32 num = ff9.tweaker.w_frameScenePtr;
            global::Debug.LogWarning("This happens only in DebugScene!: fakeEvent = " + num);
            return (UInt16)num;
        }
        UInt16 num2 = FF9StateSystem.EventState.gEventGlobal[index + 1];
        num2 = (UInt16)(num2 << 8);
        return (UInt16)(num2 | FF9StateSystem.EventState.gEventGlobal[index]);
    }

    public static Boolean IsBeeScene => FF9StateSystem.World.IsBeeScene;

    public static Byte ushort_gEventGlobal_keventScriptNo()
    {
        if (ff9.IsBeeScene)
            return 1;
        return ff9.byte_gEventGlobal(102);
    }

    public static UInt16 ushort_gEventGlobal_keventNaviLocF0()
    {
        return ff9.ushort_gEventGlobal(92);
    }

    public static UInt16 ushort_gEventGlobal_keventNaviLocF1()
    {
        return ff9.ushort_gEventGlobal(94);
    }

    public static UInt16 ushort_gEventGlobal_keventNaviLocF2()
    {
        return ff9.ushort_gEventGlobal(96);
    }

    public static UInt16 ushort_gEventGlobal_keventNaviLocF3()
    {
        return ff9.ushort_gEventGlobal(98);
    }

    public static Int32 m_GetIDEvent(Int32 IDALL)
    {
        return (IDALL & 0xC000) >> 14;
    }

    public static Int32 m_GetIDArea(Int32 IDALL)
    {
        return (IDALL & 0x3F00) >> 8;
    }

    public static Int32 m_GetIDTopograph(Int32 IDALL)
    {
        return (IDALL & 0xFC) >> 2;
    }

    public static Int32 m_moveActorID
    {
        get
        {
            if (ff9.w_moveActorPtr == null)
                return 0;
            return ff9.w_moveCHRStatus[ff9.w_moveActorPtr.originalActor.index].id;
        }
    }

    /*
    public static String GetName_kWorldPackEffect(Int32 index)
    {
        switch (index)
        {
            case 54:
                return "kWorldPackEffectTwister";
            case 55:
                return "kWorldPackEffectSpiral0";
            case 56:
                return "kWorldPackEffectSpiral1";
            case 57:
                return "kWorldPackEffectSpiral2";
            case 58:
                return "kWorldPackEffectWindmill";
            case 59:
                return "kWorldPackEffectCore";
            case 60:
                return "kWorldPackEffectSky";
            case 61:
                return "kWorldPackEffectSphere1";
            case 62:
                return "kWorldPackEffectSphere2";
            case 63:
                return "kWorldPackEffectArch";
            case 64:
                return "kWorldPackEffectBlack";
            case 65:
                return "kWorldPackEffectThunder1";
            case 66:
                return "kWorldPackEffectThunder2";
            case 67:
                return "kWorldPackModelSea";
            default:
                return "default";
        }
    }
    */

    public static FF9StateGlobal FF9Global => FF9StateSystem.Common.FF9;
    public static FF9StateSystem FF9Sys => PersistenSingleton<FF9StateSystem>.Instance;
    public static WMWatcher watcher => Singleton<WMWatcher>.Instance;
    public static WMWorld world => Singleton<WMWorld>.Instance;
    private static WMRenderTextureBank renderTextureBank => Singleton<WMRenderTextureBank>.Instance;
    public static WMTweaker tweaker => Singleton<WMTweaker>.Instance;

    public static Single S(Int32 fixedPoint) => fixedPoint * 0.00390625f;

    public static Int32 UnityUnit(Single f) => (Int32)(f * 256f);

    public static Int16 UnityRot(Single rotation)
    {
        return (Int16)(rotation * 11.3777781f);
    }

    public static Single PsxRot(Int32 rotation)
    {
        return rotation * 0.087890625f;
    }

    public static Single PsxScale(Int32 scale)
    {
        return scale * 0.000244140625f;
    }

    public static Vector3 PosDiff(Vector3 a, Vector3 b)
    {
        Vector3 diff = b - a;
        while (diff.x > 768f)
            diff.x -= 1536f;
        while (diff.x < -768f)
            diff.x += 1536f;
        while (diff.z > 640f)
            diff.z -= 1280f;
        while (diff.z < -640f)
            diff.z += 1280f;
        return diff;
    }

    public static Boolean NearlyEqual(Single a, Single b)
    {
        Single num = 1E-05f;
        Single num2 = Math.Abs(a);
        Single num3 = Math.Abs(b);
        Single num4 = Math.Abs(a - b);
        if (a == b)
            return true;
        if (a == 0f || b == 0f || num4 < 1.401298E-45f)
            return num4 < num * Single.Epsilon;
        return num4 / Math.Min(num2 + num3, Single.MaxValue) < num;
    }

    [Conditional("FF9PC_PRINT_ENABLED")]
    public static void printf(String message, ff9.LogLevel level = LogLevel.Info)
    {
        if (!ff9.PrintDebug)
            return;
        switch (level)
        {
            case LogLevel.Info:
                global::Debug.Log(message);
                break;
            case LogLevel.Warning:
                global::Debug.LogWarning(message);
                break;
            case LogLevel.Error:
                global::Debug.LogError(message);
                break;
        }
    }

    public static void FF9Pc_SeekReadB(String fileName, Int32 pos, UInt32 size, Byte[] buffer)
    {
        if (buffer.Length < size)
        {
        }
        Byte[] binAsset = AssetManager.LoadBytes(fileName);
        if (binAsset == null)
        {
        }
        using (MemoryStream memoryStream = new MemoryStream(binAsset))
        {
            memoryStream.Position = pos;
            memoryStream.Read(buffer, 0, (Int32)size);
        }
    }

    public static Int32 rand()
    {
        return UnityEngine.Random.Range(0, Int32.MaxValue);
    }

    public static Int32 random8()
    {
        return UnityEngine.Random.Range(0, 256);
    }

    public static Int32 random16()
    {
        return UnityEngine.Random.Range(0, 65536);
    }

    /*
    public static void FF9Wipe_WhiteInEx(Int32 frame)
    {
        if (FF9StateSystem.World.IsBeeScene)
            return;
        SceneDirector.FF9Wipe_WhiteInEx(frame);
    }
    */

    public static void FF9Wipe_WhiteOutEx(Int32 frame)
    {
        if (FF9StateSystem.World.IsBeeScene)
            return;
        SceneDirector.FF9Wipe_WhiteOutEx(frame);
    }

    public static void FF9Wipe_FadeOutEx(Int32 frame)
    {
        if (FF9StateSystem.World.IsBeeScene)
            return;
        SceneDirector.FF9Wipe_FadeOutEx(frame);
    }

    public static Matrix4x4 RotMatrix(ref Vector3 VRot, out Matrix4x4 MRot)
    {
        Quaternion q = Quaternion.Euler(VRot.x, VRot.y, VRot.z);
        MRot = Matrix4x4.TRS(Vector3.zero, q, new Vector3(1f, 1f, 1f));
        return MRot;
    }

    public static String SC_COUNTER_ToString(Int32 value)
    {
        if (value == 1500)
            return "SC_COUNTER_ARMOR_BLANK_START";
        if (value == 1600)
            return "SC_COUNTER_ARMOR_BLANK_END";
        if (value == 2400)
            return "SC_COUNTER_WMTITLE_BUNMEI_ON";
        if (value == 2990)
            return "SC_COUNTER_SGATE_DESTROYED";
        if (value == 4990)
            return "SC_COUNTER_CLAY_DESTROYED";
        if (value == 5598)
            return "SC_COUNTER_LIND_DESTROYED";
        if (value == 5990)
            return "SC_COUNTER_FOG_END";
        if (value == 6200)
            return "SC_COUNTER_KUROMA_APPEAR";
        if (value == 6875)
            return "SC_COUNTER_DAGGER_AWAKE";
        if (value == 6990)
            return "SC_COUNTER_SGATE_RECOVER";
        if (value == 8800)
            return "SC_COUNTER_ALEX_DESTROYED";
        if (value == 9450)
            return "SC_COUNTER_SUNA_MAKYU_ON";
        if (value == 9600)
            return "SC_COUNTER_WMTITLE_NEW_ON";
        if (value == 9890)
            return "SC_COUNTER_SUNA_MAKYU_OFF";
        if (value == 10300)
            return "SC_COUNTER_CUT_HAIR";
        if (value == 10400)
            return "SC_COUNTER_GET_HILDA3";
        if (value == 10600)
            return "SC_COUNTER_HOKORA_START";
        if (value == 10700)
            return "SC_COUNTER_HOKORA_END";
        if (value == 11090)
            return "SC_COUNTER_LAST_WORLD";
        return String.Empty;
    }

    public static void w_blockSystemConstructor()
    {
        ff9.w_blockReady = false;
    }

    public static void w_cameraSystemConstructor()
    {
        if (!ff9.converted_w_cameraPosstat)
        {
            for (Int32 i = 0; i < ff9.w_cameraPosstat.Length; i++)
            {
                ff9.s_cameraPosstat s_cameraPosstat = ff9.w_cameraPosstat[i];
                s_cameraPosstat.cameraDistance *= 0.00390625f;
                s_cameraPosstat.cameraHeight = s_cameraPosstat.cameraHeight * 0.00390625f * -1f;
                s_cameraPosstat.cameraCorrect = s_cameraPosstat.cameraCorrect * 0.00390625f * -1f;
                s_cameraPosstat.aimHeight = s_cameraPosstat.aimHeight * 0.00390625f * -1f;
                ff9.w_cameraPosstat[i] = s_cameraPosstat;
            }
            ff9.converted_w_cameraPosstat = true;
        }
        ff9.w_cameraSysData = ff9.FF9Global.worldState;
        ff9.w_cameraSysDataCamera = ff9.w_cameraSysData.statecamera;
        ff9.setVector(ref ff9.w_cameraUpvector, 0f, 78.125f, 0f);
        ff9.w_cameraFixMode = false;
        ff9.w_cameraFixModeY = false;
        ff9.w_cameraRotAngle = 0f;
        ff9.w_cameraChrFakeFlag = false;
        if (ff9.FF9Sys.prevMode == 1 && ff9.FF9Global.worldState.loadgameflg == 0)
        {
            ff9.w_cameraSysDataCamera.upperCounter = 0;
            ff9.w_cameraSysDataCamera.upperCounterSpeed = 0;
            ff9.w_cameraSysDataCamera.upperCounterForce = false;
            ff9.w_cameraSysDataCamera.rotation = 0f;
            ff9.w_cameraSysData.cameraNotrot = false;
        }
        if (ff9.w_cameraSysDataCamera.upperCounterSpeed < 0)
            ff9.w_cameraSysDataCamera.upperCounter = 0;
        if (ff9.w_cameraSysDataCamera.upperCounterSpeed > 0)
            ff9.w_cameraSysDataCamera.upperCounter = 4096;
        ff9.w_cameraChangeCounter = 0;
        ff9.w_cameraChangeCounterSpeed = 0;
        ff9.w_cameraPosstatNow = ff9.w_cameraPosstat[1];
        ff9.w_cameraSmoothX = false;
        ff9.w_cameraSmoothY = false;
        ff9.w_cameraSmoothZ = false;
        ff9.w_cameraFuzzy = true;
        ff9.w_cameraFirstDeside = 0;
        FF9StateSystem.Settings.IsBoosterButtonActive[5] = (ff9.w_cameraSysDataCamera.upperCounter == 4096);
        FF9StateSystem.Settings.IsBoosterButtonActive[2] = !ff9.w_cameraSysData.cameraNotrot;
    }

    public static void w_cameraMapConstructor()
    {
        ff9.w_cameraChangeUpdate();
        ff9.w_cameraSetEyeAim();
    }

    public static void w_cameraChange(Int32 mode, Int32 step)
    {
        ff9.w_cameraChangeCounter = 0;
        ff9.w_cameraChangeChr = (Byte)mode;
        ff9.w_cameraPosstatPrev = ff9.w_cameraPosstatNow;
        if (ff9.w_blockReady)
            ff9.w_cameraChangeCounterSpeed = (Int16)step;
        else
            ff9.w_cameraChangeCounterSpeed = 0;
    }

    public static void w_cameraSmooth()
    {
        ff9.w_cameraSmoothX = true;
        ff9.w_cameraSmoothY = true;
        ff9.w_cameraSmoothZ = true;
    }

    private static Single cameraAimTweak = 0;
    private static void ProcessCameraControl(Single stickX, Single stickY, Single speedX, Single speedY)
    {
        if (Mathf.Abs(stickY) <= Configuration.AnalogControl.StickThreshold)
            stickY = 0f;
        stickY *= Configuration.AnalogControl.InvertedCameraY;
        Single target = Mathf.LerpUnclamped(0, ff9.w_cameraWorldEye.y, stickY);
        cameraAimTweak += (target - cameraAimTweak) * (1 - Mathf.Exp(-0.1f * speedY));

        if (Mathf.Abs(stickX) <= Configuration.AnalogControl.StickThreshold)
            stickX = 0f;
        ff9.w_cameraSysDataCamera.rotation += stickX * speedX;
        ff9.w_cameraSysDataCamera.rotation %= 360f;
    }

    public static void w_cameraUpdate()
    {
        if (!ff9.w_cameraFixMode)
            ff9.w_cameraChangeUpdate();

        Single fovDivider = ff9.w_moveCHRControlPtr.type_cam == 2 ? 9.5f : 8f; // exception for blue narciss
        Boolean isAShip = ff9.w_moveCHRControlPtr.type == 1 || ff9.w_moveCHRControlPtr.type == 2; // flight (ship/choco) 1, boat 2
        //Log.Message("ff9.w_moveCHRControlPtr.type_cam " + ff9.w_moveCHRControlPtr.type_cam + " ff9.w_moveCHRControlPtr.type " + ff9.w_moveCHRControlPtr.type);

        Single speedmultiplier = (isAShip && ff9.w_frameScenePtr != 2980 && w_moveCHRControl_XZSpeed > 0) ? Mathf.Max(w_moveCHRControl_XZSpeed * Configuration.Worldmap.FieldOfViewSpeedBoost / 20f) : 0f; // w_moveCHRControl_XZSpeed normally -1 to 2
        Single fovSetting = ff9.w_frameScenePtr != 2980 ? Configuration.Worldmap.FieldOfView : 43.75f;
        ff9.world.MainCamera.fieldOfView = (ff9.w_cameraPosstatNow.cameraPers / fovDivider) * (fovSetting / 44f) + speedmultiplier;
        if (ff9.w_moveCHRControlPtr.type == 1)
            ff9.world.MainCamera.fieldOfView = fovSetting + speedmultiplier;
        if (ff9.w_moveCHRControlPtr.type_cam == 1)
        {
            Int32 topoID = ff9.w_frameGetParameter(193);
            if (topoID == 36 || topoID == 37 || topoID == 38)
                ff9.world.MainCamera.fieldOfView = fovSetting + speedmultiplier;
        }
        ff9.w_cameraSysDataCamera.rotation %= 360f;
        ff9.w_cameraAimOffset = cameraAimTweak + ff9.w_cameraPosstatNow.aimHeight * Configuration.Worldmap.CameraAimHeight / 100f;
        ff9.w_cameraEyeOffset.y = ff9.w_cameraPosstatNow.cameraHeight * Configuration.Worldmap.CameraHeight / 100f;
        ff9.w_cameraEyeOffset.x = ff9.rsin(ff9.w_cameraSysDataCamera.rotation) * (ff9.w_cameraPosstatNow.cameraDistance * ff9.w_cameraDebugDist / 4096f) * (Configuration.Worldmap.CameraDistance / 100f);
        ff9.w_cameraEyeOffset.z = ff9.rcos(ff9.w_cameraSysDataCamera.rotation) * (ff9.w_cameraPosstatNow.cameraDistance * ff9.w_cameraDebugDist / 4096f) * (Configuration.Worldmap.CameraDistance / 100f);
        if (!ff9.w_cameraFixMode)
            ff9.w_cameraSetEyeAim();
        ff9.setVector(ref ff9.w_cameraFocusEye, ff9.w_cameraWorldAim.x - ff9.w_cameraWorldEye.x, ff9.w_cameraWorldAim.y - ff9.w_cameraWorldEye.y, ff9.w_cameraWorldAim.z - ff9.w_cameraWorldEye.z);
        if (ff9.tweaker.CustomCamera)
        {

            Vector3 zero = Vector3.zero;
            zero.y = ff9.w_cameraSysDataCamera.rotation;
            Matrix4x4 matrix4x;
            ff9.RotMatrix(ref zero, out matrix4x);
            ff9.ApplyMatrixLV(ref matrix4x, ref ff9.w_cameraUpvector, out Vector3 vec);
            ff9.w_camera_makematrix(ff9.w_frameCameraPtr, ff9.w_cameraFocusEye, ff9.w_cameraFocusAim, vec, ff9.w_cameraWorldEye);

        }
        else
        {
            Vector3 b = new Vector3(0f, ff9.tweaker.w_cameraWorldEye_Y * 0.00390625f, 0f);
            Vector3 b2 = new Vector3(0f, ff9.tweaker.w_cameraWorldAim_Y * 0.00390625f, 0f);
            if (FF9StateSystem.World.FixTypeCam)
            {
                if (ff9.w_moveCHRControlPtr.type_cam == 0) // on foot
                {
                    ff9.tweaker.FixTypeCamEyeY = 558;
                    ff9.tweaker.FixTypeCamAimY = 312;
                    ff9.tweaker.FixTypeCamEyeYTarget = 558;
                    ff9.tweaker.FixTypeCamAimYTarget = 312;
                }
                if (ff9.tweaker.FixTypeCamEyeYCurrent < ff9.tweaker.FixTypeCamEyeYTarget)
                {
                    ff9.tweaker.FixTypeCamEyeYCurrent += 25;
                    if (ff9.tweaker.FixTypeCamEyeYCurrent > ff9.tweaker.FixTypeCamEyeYTarget)
                        ff9.tweaker.FixTypeCamEyeYCurrent = ff9.tweaker.FixTypeCamEyeYTarget;
                }
                else if (ff9.tweaker.FixTypeCamEyeYCurrent > ff9.tweaker.FixTypeCamEyeYTarget)
                {
                    ff9.tweaker.FixTypeCamEyeYCurrent -= 25;
                    if (ff9.tweaker.FixTypeCamEyeYCurrent < ff9.tweaker.FixTypeCamEyeYTarget)
                        ff9.tweaker.FixTypeCamEyeYCurrent = ff9.tweaker.FixTypeCamEyeYTarget;
                }
                if (ff9.tweaker.FixTypeCamAimYCurrent < ff9.tweaker.FixTypeCamAimYTarget)
                {
                    ff9.tweaker.FixTypeCamAimYCurrent += 25;
                    if (ff9.tweaker.FixTypeCamAimYCurrent > ff9.tweaker.FixTypeCamAimYTarget)
                        ff9.tweaker.FixTypeCamAimYCurrent = ff9.tweaker.FixTypeCamAimYTarget;
                }
                else if (ff9.tweaker.FixTypeCamAimYCurrent > ff9.tweaker.FixTypeCamAimYTarget)
                {
                    ff9.tweaker.FixTypeCamAimYCurrent -= 25;
                    if (ff9.tweaker.FixTypeCamAimYCurrent < ff9.tweaker.FixTypeCamAimYTarget)
                        ff9.tweaker.FixTypeCamAimYCurrent = ff9.tweaker.FixTypeCamAimYTarget;
                }
                ff9.tweaker.FixTypeCamEyeY = ff9.tweaker.FixTypeCamEyeYCurrent;
                ff9.tweaker.FixTypeCamAimY = ff9.tweaker.FixTypeCamAimYCurrent;
                b = new Vector3(0f, ff9.tweaker.FixTypeCamEyeY * 0.00390625f * Configuration.Worldmap.CameraHeight / 100f, 0f);
                b2 = new Vector3(0f, ff9.tweaker.FixTypeCamAimY * 0.00390625f * Configuration.Worldmap.CameraAimHeight / 100f, 0f);
            }
            Camera mainCamera = Singleton<WMWorld>.Instance.MainCamera;
            mainCamera.transform.position = ff9.w_cameraWorldEye + b;
            mainCamera.transform.LookAt(ff9.w_cameraWorldAim + b2);
            if (!ff9.w_moveCHRControl_LR && !ff9.w_cameraSysData.cameraNotrot)
            {
                Vector3 eulerAngles = mainCamera.transform.rotation.eulerAngles;
                mainCamera.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, ff9.w_cameraRotAngle);
            }
        }
        ff9.w_cameraDirVector.x = ff9.w_cameraWorldAim.x - ff9.w_cameraWorldEye.x;
        ff9.w_cameraDirVector.y = 0f;
        ff9.w_cameraDirVector.z = ff9.w_cameraWorldAim.z - ff9.w_cameraWorldEye.z;
        if (ff9.w_cameraDirVector.x > 65535f)
            ff9.w_cameraDirVector.x = ff9.w_cameraDirVector.x - 393216f;
        if (ff9.w_cameraDirVector.z > 65535f)
            ff9.w_cameraDirVector.z = ff9.w_cameraDirVector.z - 327680f;
        if (ff9.w_cameraDirVector.x < -65535f)
            ff9.w_cameraDirVector.x = ff9.w_cameraDirVector.x + 393216f;
        if (ff9.w_cameraDirVector.z < -65535f)
            ff9.w_cameraDirVector.z = ff9.w_cameraDirVector.z + 327680f;
        Single angle = ff9.ratan2(ff9.w_cameraDirVector.z, ff9.w_cameraDirVector.x);
        angle += ff9.w_cameraRotAngle * 0f;
        ff9.w_cameraDirVector.x = ff9.rcos(angle);
        ff9.w_cameraDirVector.z = ff9.rsin(angle);
        ff9.VectorNormal(ref ff9.w_cameraDirVector, out ff9.w_cameraDirVector);
        if (ff9.w_frameScenePtr < 4990 && ff9.m_GetIDArea(ff9.m_moveActorID) == 12)
            ff9.w_cameraSysDataCamera.upperCounterForce = true;
        ff9.w_cameraDebugDist = ff9.tweaker.w_cameraDebugDist;
    }

    public static void w_cameraSetEyeAim()
    {
        Single num = 17.578125f;
        Single num3 = 0.0625f;
        Single y = 0f;
        if (!ff9.GetEventAim())
        {
            Single posX = ff9.w_moveActorPtr.pos[0];
            Single posY = ff9.w_moveActorPtr.pos[1] + ff9.w_cameraAimOffset;
            Single posZ = ff9.w_moveActorPtr.pos[2];
            ff9.w_cameraInter = (ff9.w_cameraSmoothX | ff9.w_cameraSmoothY | ff9.w_cameraSmoothZ);
            ff9.w_cameraInter = false;
            if (ff9.w_cameraInter)
            {
                if (ff9.w_cameraSmoothX)
                {
                    if (ff9.abs(ff9.w_cameraWorldAim.x - posX) < num3)
                    {
                        ff9.w_cameraSmoothX = false;
                    }
                    else
                    {
                        Vector3 vector = ff9.w_cameraWorldAim;
                        vector.x += ff9.GetControlChar().pos[0] - ff9.w_cameraWorldAim.x / 8f;
                        ff9.w_cameraWorldAim = vector;
                    }
                }
                if (ff9.w_cameraSmoothY)
                {
                    if (ff9.abs(ff9.w_cameraWorldAim.y - posY) < num3)
                    {
                        ff9.w_cameraSmoothY = false;
                    }
                    else
                    {
                        Vector3 vector = ff9.w_cameraWorldAim;
                        vector.y += (ff9.GetControlChar().pos[1] - ff9.w_cameraWorldAim.y + ff9.w_cameraAimOffset) / 8f;
                        ff9.w_cameraWorldAim = vector;
                    }
                }
                if (ff9.w_cameraSmoothZ)
                {
                    if (ff9.abs(ff9.w_cameraWorldAim.z - posZ) < num3)
                    {
                        ff9.w_cameraSmoothZ = false;
                    }
                    else
                    {
                        Vector3 vector = ff9.w_cameraWorldAim;
                        ff9.w_cameraWorldAim.z = ff9.w_cameraWorldAim.z + (ff9.GetControlChar().pos[2] - ff9.w_cameraWorldAim.z) / 8f;
                        ff9.w_cameraWorldAim = vector;
                    }
                }
                if (!ff9.w_cameraSmoothX)
                {
                    ff9.w_cameraWorldAim.x = posX;
                }
                if (!ff9.w_cameraSmoothY)
                {
                    ff9.w_cameraWorldAim.y = posY;
                }
                if (!ff9.w_cameraSmoothZ)
                {
                    ff9.w_cameraWorldAim.z = posZ;
                }
            }
            else
            {
                Single diffY = posY - ff9.w_cameraWorldAim.y;
                Single num8 = 0f;
                Int32 num9 = 0;
                if (diffY > 0.5f)
                {
                    num9 = 2;
                }
                if (diffY < -0.5f)
                {
                    num9 = 1;
                }
                ff9.nsp = ff9.abs(diffY) / 32f;
                if (ff9.nsp < 0.125f)
                {
                    ff9.nsp = 0.125f;
                }
                if (ff9.nsp > 1f)
                {
                    ff9.nsp = 1f;
                }
                if (num9 != 0)
                {
                    switch (ff9.w_moveCHRControlPtr.type)
                    {
                        case 0:
                        case 3:
                        {
                            Int32 num10 = num9;
                            if (num10 != 1)
                            {
                                if (num10 == 2)
                                {
                                    num8 = (diffY - 0.5f) * 1f / (32f * ff9.nsp);
                                }
                            }
                            else
                            {
                                num8 = (diffY + 0.5f) * 1f / (32f * ff9.nsp);
                            }
                            break;
                        }
                        case 1:
                        case 2:
                        {
                            Int32 num10 = num9;
                            if (num10 != 1)
                            {
                                if (num10 == 2)
                                {
                                    num8 = (diffY - 0.5f) / 8f;
                                }
                            }
                            else
                            {
                                num8 = (diffY + 0.5f) / 2f;
                            }
                            break;
                        }
                    }
                }
                ff9.w_cameraWorldAim.x = posX;
                ff9.w_cameraWorldAim.z = posZ;
                if (ff9.abs(num8) > 0.015625f && !ff9.w_cameraFixModeY)
                {
                    ff9.w_cameraWorldAim.y = ff9.w_cameraWorldAim.y + num8 * 2f;
                }
            }
        }
        else
        {
            ff9.w_moveActorPtr.pos0 = ff9.w_cameraWorldAim.x;
            ff9.w_moveActorPtr.pos1 = ff9.w_cameraWorldAim.y;
            ff9.w_moveActorPtr.pos2 = ff9.w_cameraWorldAim.z;
        }
        if (!ff9.GetEventEye())
        {
            Single num11 = 1f;
            Single num12 = 0f;
            Int32 idall = -1;
            Int32 num13 = -1;
            Int32 num14 = -1;
            Int32 num15 = -1;
            Int32 num16 = -1;
            Vector3 vector = default(Vector3);
            Single num17 = 1f;
            Single num18 = ff9.w_moveActorPtr.pos[0] + ff9.w_cameraEyeOffset.x;
            Single num19 = ff9.w_moveActorPtr.pos[1] + ff9.w_cameraEyeOffset.y;
            Single num20 = ff9.w_moveActorPtr.pos[2] + ff9.w_cameraEyeOffset.z;
            vector.x = num18 + ff9.w_cameraDirVector.x / 4f;
            vector.y = num19;
            vector.z = num20 + ff9.w_cameraDirVector.z / 4f;
            WMPhysics.UseInfiniteRaycast = true;
            WMPhysics.CastRayFromSky = true;
            WMPhysics.IgnoreExceptions = true;
            if (ff9.w_cameraFuzzy)
            {
                Single num21 = 5.5f;
                vector.x += num21;
                Int32 num22;
                Single num23;
                ff9.w_cellHit(ref vector, ref num13, out num22, ff9.w_cameraHit[0], out num23);
                vector.x -= num21 * 2f;
                Single num24;
                ff9.w_cellHit(ref vector, ref num14, out num22, ff9.w_cameraHit[1], out num24);
                vector.x += num21;
                vector.z += num21;
                Single num25;
                ff9.w_cellHit(ref vector, ref num15, out num22, ff9.w_cameraHit[2], out num25);
                vector.z -= num21 * 2f;
                Single num26;
                ff9.w_cellHit(ref vector, ref num16, out num22, ff9.w_cameraHit[3], out num26);
                num12 = num23;
                idall = num13;
                if (num24 > num12)
                {
                    num12 = num24;
                    idall = num14;
                }
                if (num25 > num12)
                {
                    num12 = num25;
                    idall = num15;
                }
                if (num26 > num12)
                {
                    num12 = num26;
                    idall = num16;
                }
            }
            else
            {
                Int32 num22;
                ff9.w_cellHit(ref vector, ref idall, out num22, ff9.w_cameraHit[0], out num12);
            }
            WMPhysics.IgnoreExceptions = false;
            WMPhysics.UseInfiniteRaycast = false;
            WMPhysics.CastRayFromSky = false;
            num12 = ff9.w_moveCHRControlPtr.type == 1 ? Mathf.Min(num12, 33.2f) : Mathf.Min(num12, 37.1f);
            if (ff9.w_cameraPosstatNow.cameraCorrect + num12 > num19)
            {
                num11 = 1f;
                if (ff9.m_GetIDTopograph(idall) == 49 || ff9.w_movePadDOWN || ff9.w_movePadLR)
                {
                    num11 = 4f;
                }
                if (ff9.w_cameraSysDataCamera.upperCounter != 0 && ff9.w_cameraSysDataCamera.upperCounter != 4096)
                {
                    num11 = 8f;
                }
                num19 = ff9.w_cameraPosstatNow.cameraCorrect + num12;
            }
            if (ff9.w_moveCHRControlPtr.type == 1 && num19 < num)
            {
                num19 = num;
            }
            Single num7 = num19 - ff9.w_cameraWorldEye.y;
            Single num8 = 0f;
            Int32 num9 = 0;
            if (num7 > num17)
            {
                num9 = 2;
            }
            if (num7 < -num17)
            {
                num9 = 1;
            }
            ff9.nsp = Mathf.Clamp(ff9.abs(num7) / 16f, ff9.sa2, ff9.sa3);
            if (num9 != 0)
            {
                switch (ff9.w_moveCHRControlPtr.type)
                {
                    case 0:
                    case 3:
                    {
                        Int32 num10 = num9;
                        if (num10 != 1)
                        {
                            if (num10 == 2)
                            {
                                num8 = (num7 - num17) * 1f / ((ff9.S(ff9.sa4 - 2) * ff9.nsp * 8f - ff9.S(ff9.sa4 + 2) * ff9.nsp) / num11);
                            }
                        }
                        else
                        {
                            num8 = (num7 + num17) * 1f / (ff9.S(ff9.sa4) * ff9.nsp * 3f);
                        }
                        Single num27 = ff9.abs(num8);
                        if (num27 > 2f)
                        {
                            num27 = 2f;
                        }
                        num27 = (2f - num27) * ff9.vn1 / 4096f + 16f;
                        num8 *= num27;
                        num8 /= 4096f;
                        break;
                    }
                    case 1:
                    case 2:
                    {
                        Int32 num10 = num9;
                        if (num10 != 1)
                        {
                            if (num10 == 2)
                            {
                                num8 = (num7 - num17) / 4f;
                            }
                        }
                        else
                        {
                            num8 = (num7 + num17) / 32f;
                        }
                        break;
                    }
                }
            }
            ff9.w_cameraWorldEye.x = num18;
            ff9.w_cameraWorldEye.z = num20;
            if (ff9.abs(num8) > 0.015625f && !ff9.w_cameraFixModeY)
            {
                ff9.w_cameraWorldEye.y = ff9.w_cameraWorldEye.y + num8;
            }
            y = num19;
        }
        Single num28 = ff9.w_cameraWorldEye.x + 767.46484375f;
        Single num29 = ff9.w_cameraWorldEye.z + 320.19140625f;
        Single num30 = 54.6875f;
        if (ff9.abs(num28) < num30 && ff9.abs(num29) < num30)
        {
            Single num31 = num30 * num30 - num28 * num28 - num29 * num29;
            Single num32 = ff9.SquareRoot0(num31) - 8.59375f;
            if (num31 > 0f && ff9.w_cameraWorldEye.y < num32)
            {
                ff9.w_cameraWorldEye.y = num32;
            }
        }
        Byte b = ff9.w_cameraFirstDeside;
        if (b != 0)
        {
            if (b == 1)
            {
                ff9.w_cameraFirstDeside = 2;
                if ((ff9.byte_gEventGlobal(101) & 1) != 0)
                {
                    SceneDirector.FF9Wipe_WhiteIn();
                    Byte b2 = FF9StateSystem.EventState.gEventGlobal[101];
                    b2 = (Byte)(b2 & -2);
                    ff9.byte_gEventGlobal_Write(101, b2);
                }
                else
                {
                    SceneDirector.FF9Wipe_FadeIn();
                }
            }
        }
        else if (ff9.w_blockReady)
        {
            ff9.w_cameraFirstDeside = 1;
            ff9.w_cameraWorldEye.y = y;
            ff9.w_cameraWorldAim.y = ff9.w_moveActorPtr.pos[1] + ff9.w_cameraAimOffset;
        }
        ff9.w_cameraWorldEye.y = Mathf.Min(ff9.w_cameraWorldEye.y, 71.80859375f);
    }

    public static void w_cameraChangeUpdate()
    {
        Int32 area = ff9.w_cameraArea2Place[ff9.w_frameGetParameter(192)];
        Int32 type_cam = ff9.w_moveCHRControlPtr.type_cam;
        if (ff9.w_cameraForcePlace != -1)
        {
            area = ff9.w_cameraForcePlace;
        }
        if (ff9.w_cameraSysDataCamera.upperCounterForce)
        {
            ff9.w_cameraSysDataCamera.upperCounterSpeed = -128;
        }
        ff9.sworldStateCamera sworldStateCamera = ff9.w_cameraSysDataCamera;
        sworldStateCamera.upperCounter += (Int16)ff9.w_cameraSysDataCamera.upperCounterSpeed;
        if (ff9.w_cameraSysDataCamera.upperCounter < 1)
        {
            ff9.w_cameraSysDataCamera.upperCounter = 0;
            ff9.w_cameraSysDataCamera.upperCounterSpeed = 0;
            ff9.w_cameraSysDataCamera.upperCounterForce = false;
        }
        if (ff9.w_cameraSysDataCamera.upperCounter > 4095)
        {
            ff9.w_cameraSysDataCamera.upperCounter = 4096;
            ff9.w_cameraSysDataCamera.upperCounterSpeed = 0;
            ff9.w_cameraSysDataCamera.upperCounterForce = false;
        }
        if (ff9.w_cameraChangeCounterSpeed != 0)
        {
            ff9.w_cameraChangeCounter += ff9.w_cameraChangeCounterSpeed;
            if (ff9.w_cameraChangeCounter > 4096)
                ff9.w_cameraChangeCounter = 4096;
            ff9.s_cameraPosstat s_cameraPosstat;
            ff9.w_cameraGetPosstat(ff9.w_moveActorPtr.pos[1], ff9.w_cameraChangeChr, area, ff9.w_cameraSysDataCamera.upperCounter, out s_cameraPosstat);
            ff9.w_cameraPosstatNow.cameraPers = ff9.w_frameInter16(ff9.w_cameraPosstatPrev.cameraPers, s_cameraPosstat.cameraPers, ff9.w_cameraChangeCounter, 9);
            ff9.w_cameraPosstatNow.cameraDistance = ff9.w_frameInter16(ff9.w_cameraPosstatPrev.cameraDistance, s_cameraPosstat.cameraDistance, ff9.w_cameraChangeCounter, 15);
            ff9.w_cameraPosstatNow.cameraHeight = ff9.w_frameInter16(ff9.w_cameraPosstatPrev.cameraHeight, s_cameraPosstat.cameraHeight, ff9.w_cameraChangeCounter, 14);
            ff9.w_cameraPosstatNow.cameraCorrect = ff9.w_frameInter16(ff9.w_cameraPosstatPrev.cameraCorrect, s_cameraPosstat.cameraCorrect, ff9.w_cameraChangeCounter, 14);
            ff9.w_cameraPosstatNow.aimHeight = ff9.w_frameInter16(ff9.w_cameraPosstatPrev.aimHeight, s_cameraPosstat.aimHeight, ff9.w_cameraChangeCounter, 14);
            if (ff9.w_cameraChangeCounter >= 4096)
                ff9.w_cameraChangeCounterSpeed = 0;
        }
        else
        {
            ff9.w_cameraGetPosstat(ff9.w_moveActorPtr.pos[1], type_cam, area, ff9.rsin(ff9.w_cameraSysDataCamera.upperCounter / 4), out ff9.w_cameraPosstatNow);
        }
    }

    public static void w_cameraGetPosstat(Single height, Int32 chr, Int32 area, Int32 upper, out ff9.s_cameraPosstat data)
    {
        ff9.s_cameraPosstat s_cameraPosstat = ff9.w_cameraPosstat[ff9.w_cameraElement[chr, area].fly];
        ff9.s_cameraPosstat s_cameraPosstat2 = ff9.w_cameraPosstat[ff9.w_cameraElement[chr, area].down];
        ff9.s_cameraPosstat s_cameraPosstat3 = ff9.w_cameraPosstat[ff9.w_cameraElement[chr, area].up];
        UInt16 t = ff9.w_cameraGetHeightParam(height);
        ff9.s_cameraPosstat s_cameraPosstat4;
        s_cameraPosstat4.cameraPers = ff9.w_frameInter16(s_cameraPosstat2.cameraPers, s_cameraPosstat3.cameraPers, t, 9);
        s_cameraPosstat4.cameraDistance = ff9.w_frameInter16(s_cameraPosstat2.cameraDistance, s_cameraPosstat3.cameraDistance, t, 14);
        s_cameraPosstat4.cameraHeight = ff9.w_frameInter16(s_cameraPosstat2.cameraHeight, s_cameraPosstat3.cameraHeight, t, 13);
        s_cameraPosstat4.cameraCorrect = ff9.w_frameInter16(s_cameraPosstat2.cameraCorrect, s_cameraPosstat3.cameraCorrect, t, 13);
        s_cameraPosstat4.aimHeight = ff9.w_frameInter16(s_cameraPosstat2.aimHeight, s_cameraPosstat3.aimHeight, t, 13);
        data.cameraPers = ff9.w_frameInter16(s_cameraPosstat4.cameraPers, s_cameraPosstat.cameraPers, upper, 9);
        data.cameraDistance = ff9.w_frameInter16(s_cameraPosstat4.cameraDistance, s_cameraPosstat.cameraDistance, upper, 14);
        data.cameraHeight = ff9.w_frameInter16(s_cameraPosstat4.cameraHeight, s_cameraPosstat.cameraHeight, upper, 13);
        data.cameraCorrect = ff9.w_frameInter16(s_cameraPosstat4.cameraCorrect, s_cameraPosstat.cameraCorrect, upper, 13);
        data.aimHeight = ff9.w_frameInter16(s_cameraPosstat4.aimHeight, s_cameraPosstat.aimHeight, upper, 13);
    }

    public static UInt16 w_cameraGetHeightParam(Single height)
    {
        Int32 hparam = (Int32)(height * 256f);
        hparam += 1000;
        hparam *= 4096;
        hparam /= 4500;
        if (hparam > 4096)
            hparam = 4096;
        if (hparam < 0)
            hparam = 0;
        return (UInt16)hparam;
    }

    public static void w_cameraChangeTrigger()
    {
        if (ff9.w_cameraTriggerTime >= RealTime.time - 0.1f)
            return;
        ff9.w_cameraTriggerTime = RealTime.time;
        if (!ff9.w_cameraSysDataCamera.upperCounterForce && (ff9.w_frameScenePtr >= 4990 || ff9.m_GetIDArea(ff9.m_moveActorID) != 12))
        {
            Boolean atTip = false;
            if (ff9.w_cameraSysDataCamera.upperCounter == 0)
            {
                atTip = true;
                ff9.w_cameraSysDataCamera.upperCounterSpeed = 256;
            }
            if (ff9.w_cameraSysDataCamera.upperCounter == 4096)
            {
                atTip = true;
                ff9.w_cameraSysDataCamera.upperCounterSpeed = -256;
            }
            if (!atTip)
                ff9.w_cameraSysDataCamera.upperCounterSpeed = -ff9.w_cameraSysDataCamera.upperCounterSpeed;
        }
    }

    public static void w_cameraChangeNotrotMode()
    {
        if (ff9.w_cameraNotrotTime >= RealTime.time - 0.1f)
            return;
        ff9.w_cameraNotrotTime = RealTime.time;
        if (ff9.w_cameraSysData.cameraNotrot)
            ff9.w_movementSoftRot = true;
        ff9.w_cameraSysData.cameraNotrot = !ff9.w_cameraSysData.cameraNotrot;
    }

    public static void w_camera_makematrix(Camera camera, Vector3 t, Vector3 h, Vector3 v, Vector3 eye)
    {
        Vector3 vector = default(Vector3);
        ff9.setVector(ref vector, t.x - h.x, t.y - h.y, t.z - h.z);
        Vector3 lhs;
        ff9.VectorNormal(ref vector, out lhs);
        ff9.gte_OuterProduct12(ref v, ref lhs, out vector);
        Vector3 lhs2;
        ff9.VectorNormal(ref vector, out lhs2);
        ff9.gte_OuterProduct12(ref lhs, ref lhs2, out vector);
        Vector3 lhs3;
        ff9.VectorNormal(ref vector, out lhs3);
        Matrix4x4 matrix4x = default(Matrix4x4);
        matrix4x.m00 = lhs2.x;
        matrix4x.m01 = lhs2.y;
        matrix4x.m02 = lhs2.z;
        matrix4x.m10 = lhs3.x;
        matrix4x.m11 = lhs3.y;
        matrix4x.m12 = lhs3.z;
        matrix4x.m20 = lhs.x;
        matrix4x.m21 = lhs.y;
        matrix4x.m22 = lhs.z;
        matrix4x.m30 = 0f;
        matrix4x.m31 = 0f;
        matrix4x.m32 = 0f;
        matrix4x.m33 = 1f;
        matrix4x.m03 = -Vector3.Dot(lhs2, eye);
        matrix4x.m13 = -Vector3.Dot(lhs3, eye);
        matrix4x.m23 = -Vector3.Dot(lhs, eye);
        Vector3 pos = new Vector3(-eye.x, -eye.y, -eye.z);
        Matrix4x4 matrix4x2 = Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 1f, 1f));
        Matrix4x4 matrix4x3 = matrix4x;
        camera.worldToCameraMatrix = matrix4x3;
        global::Debug.Log(matrix4x3);
    }

    /*
    public static Int32 w_cameraGetMovepoint()
    {
        if (ff9.w_cameraSysDataCamera.upperCounter == 0)
        {
            return 0;
        }
        if (ff9.w_cameraSysDataCamera.upperCounter == 4096)
        {
            return 2;
        }
        return 1;
    }
    */

    public static Vector3 w_cameraGetAimPtr()
    {
        return ff9.w_cameraWorldAim;
    }

    public static void w_cameraSetAimPtr(Vector3 vector)
    {
        ff9.w_cameraWorldAim = vector * 0.00390625f;
    }

    public static Vector3 w_cameraGetEyePtr()
    {
        return ff9.w_cameraWorldEye;
    }

    public static void w_cameraSetEyePtr(Vector3 vector)
    {
        ff9.w_cameraWorldEye = vector * 0.00390625f;
    }

    /*
    public static void w_cellSystemConstructor()
    {
    }
    */

    public static Boolean w_cellHit(ref Vector3 position, ref Int32 id, out Int32 pno, ff9.s_moveCHRCache cache, out Single height)
    {
        id = 0;
        pno = 0;
        Vector3 vector = position;
        height = ff9.w_nwpHit(ref vector, out id, out pno, cache);
        if ((ff9.w_moveCHRControl_No == 3 || ff9.w_moveCHRControl_No == 2) && (id == 56 || id == 57))
        {
            id = 54;
        }
        return true;
    }

    public static void w_effectSystemConstructor()
    {
        for (Int64 num = 0L; num < 10L; num += 1L)
        {
            ff9.w_effectLastRot[(Int32)(checked(num))].x = ff9.PsxRot((Int16)(num * 128L));
            checked
            {
                ff9.w_effectLastRot[(Int32)((IntPtr)num)].y = ff9.PsxRot(ff9.rand() % 4096);
                ff9.w_effectLastRot[(Int32)((IntPtr)num)].z = 0f;
                ff9.w_effectLastDis[(Int32)((IntPtr)num)] = unchecked((Int16)(512L + 20L * num));
                ff9.w_effectLastDisT[(Int32)((IntPtr)num)] = 2048;
                ff9.w_effectLastDisC[(Int32)((IntPtr)num)] = 0;
                ff9.w_effectLastRotT[(Int32)((IntPtr)num)].x = ff9.rand() % 4096;
                ff9.w_effectLastRotT[(Int32)((IntPtr)num)].y = ff9.rand() % 4096;
                ff9.w_effectLastRotT[(Int32)((IntPtr)num)].z = 0f;
                Transform transform = ff9.world.GetkWorldPackEffectArch();
                if (transform)
                {
                    Quaternion rotation = Quaternion.Euler(ff9.w_effectLastRot[(Int32)((IntPtr)num)]);
                    transform.rotation = rotation;
                }
            }
        }
        for (Int64 num = 0L; num < 9L; num += 1L)
        {
            checked
            {
                ff9.setVector(ref ff9.w_effectModelPos[(Int32)((IntPtr)num)], 0f, 0f, 0f);
                ff9.setVector(ref ff9.w_effectModelRot[(Int32)((IntPtr)num)], 0f, 0f, 0f);
            }
        }
        ff9.setVector(ref ff9.w_effectModelPos[0], 1094.296875f, 30.40625f, -806.50390625f); // Dali
        ff9.setVector(ref ff9.w_effectModelPos[1], 896.35546875f, -1.953125f, -776.27734375f); // Cleyra
        ff9.setVector(ref ff9.w_effectModelPos[6], 767.9765625f, 42.5078125f, -320.3203125f); // Memoria
        ff9.w_effectModelPos[2] = ff9.w_effectModelPos[1];
        ff9.w_effectModelPos[3] = ff9.w_effectModelPos[1];
        ff9.w_effectModelPos[4] = ff9.w_effectModelPos[1];
        ff9.w_effectModelPos[5] = ff9.w_effectModelPos[1];
        ff9.w_effectModelPos[7] = ff9.w_effectModelPos[6];
        ff9.w_effectModelPos[8] = ff9.w_effectModelPos[6];
        ff9.w_effectTwisRot.y = 0f;
        ff9.w_effectLast1Rot.y = 0f;
        ff9.w_effectLast2Rot.y = 0f;
        ff9.w_effectSpilRot[0].y = 0f;
        ff9.w_effectSpilRot[1].y = 0f;
        ff9.w_effectSpilRot[2].y = 0f;
        ff9.w_effectMoveStockTrue = 0;
        ff9.w_effectMoveStockHeightTrue = 0;
    }

    public static void w_cellService()
    {
        if (ff9.world.Block31 != null && ff9.world.Block31.Form == 2)
        {
            ff9.w_effectServiceSP(WorldEffect.FireShrine);
            ff9.w_effectUpdateSP(WorldEffect.FireShrine);
        }
        if (ff9.world.Block115 != null)
        {
            ff9.w_effectServiceSP(WorldEffect.SandPit);
            ff9.w_effectUpdateSP(WorldEffect.SandPit);
        }
        ff9.w_effectServiceSP(WorldEffect.SandStorm);
        ff9.w_effectUpdateSP(WorldEffect.SandStorm);
        ff9.w_effectServiceSP(WorldEffect.AlexandriaWaterfall);
        ff9.w_effectUpdateSP(WorldEffect.AlexandriaWaterfall);
        ff9.w_effectServiceSP(WorldEffect.Memoria);
        ff9.w_effectUpdateSP(WorldEffect.Memoria);
        ff9.w_effectServiceSP(WorldEffect.Windmill);
        ff9.w_effectUpdateSP(WorldEffect.Windmill);
        if (ff9.world.Block219 != null && ff9.world.Block219.Form == 2)
        {
            ff9.w_effectServiceSP(WorldEffect.WaterShrine);
            ff9.w_effectUpdateSP(WorldEffect.WaterShrine);
        }
        // The effect is located at the Wind Shrine but has no visual (on PSX version as well)
        //ff9.w_effectServiceSP(WorldEffect.WindShrine);
        //ff9.w_effectUpdateSP(WorldEffect.WindShrine);
    }

    public static void w_effectServiceSP(WorldEffect no)
    {
        switch (no)
        {
            case WorldEffect.SandStorm:
                if (WorldConfiguration.UseWorldEffect(WorldEffect.SandStorm))
                {
                    Vector3 localScale = new Vector3(1.46484375f, 1f, 1.46484375f);
                    Quaternion rotation = Quaternion.Euler(ff9.w_effectTwisRot);
                    Transform transform = ff9.world.kWorldPackEffectCore;
                    transform.rotation = rotation;
                    transform.localScale = localScale;
                    ff9.world.SetAbsolutePositionOf(transform, ff9.w_effectTwisPos + new Vector3(0f, 0f, 0f), 0f);
                    transform = ff9.world.kWorldPackEffectTwister;
                    transform.rotation = rotation;
                    transform.localScale = localScale;
                    ff9.world.SetAbsolutePositionOf(transform, ff9.w_effectTwisPos, 0f);
                    rotation = Quaternion.Euler(ff9.w_effectSpilRot[0]);
                    transform = ff9.world.kWorldPackEffectSpiral0;
                    transform.rotation = rotation;
                    ff9.world.SetAbsolutePositionOf(transform, ff9.w_effectTwisPos, 0f);
                    rotation = Quaternion.Euler(ff9.w_effectSpilRot[1]);
                    transform = ff9.world.kWorldPackEffectSpiral1;
                    transform.rotation = rotation;
                    ff9.world.SetAbsolutePositionOf(transform, ff9.w_effectTwisPos, 0f);
                    rotation = Quaternion.Euler(ff9.w_effectSpilRot[2]);
                    transform = ff9.world.kWorldPackEffectSpiral2;
                    transform.rotation = rotation;
                    ff9.world.SetAbsolutePositionOf(transform, ff9.w_effectTwisPos, 0f);
                    Single dist = Vector3.Distance(ff9.w_moveActorPtr.RealPosition, ff9.w_effectTwisPos);
                    ff9.world.SetTwisterRenderQueue(dist < 51.81f ? 3010 : 2450);
                }
                break;
            case WorldEffect.Memoria:
                if (WorldConfiguration.UseWorldEffect(WorldEffect.Memoria))
                {
                    Transform effectBlackTransform = ff9.world.kWorldPackEffectBlack;
                    Transform effectSphere1Transform = ff9.world.kWorldPackEffectSphere1;
                    Transform effectSphere2Transform = ff9.world.kWorldPackEffectSphere2;
                    Camera mainCamera = Singleton<WMWorld>.Instance.MainCamera;
                    if (ff9.w_frameCounter == 10)
                    {
                        ff9.w_effectRegist_FixedPoint(ff9.UnityUnit(ff9.w_effectLastPos.x), ff9.UnityUnit(-ff9.w_effectLastPos.y), ff9.UnityUnit(ff9.w_effectLastPos.z), SPSConst.WorldSPSEffect.MEMORIA, 72988, 0);
                        ff9.w_evaCoreSPS = ff9.w_effectRegist_FixedPoint(ff9.UnityUnit(ff9.w_effectLastPos.x), ff9.UnityUnit(-ff9.w_effectLastPos.y), ff9.UnityUnit(ff9.w_effectLastPos.z), SPSConst.WorldSPSEffect.MEMORIA, 23000 * 2, 0);
                    }
                    effectBlackTransform.rotation = Quaternion.Euler(ff9.w_effectLast3Rot);
                    ff9.world.SetAbsolutePositionOf(effectBlackTransform, ff9.w_effectLastPos, 0f);
                    effectBlackTransform.LookAt(effectBlackTransform.position + mainCamera.transform.rotation * Vector3.back, mainCamera.transform.rotation * Vector3.up);
                    effectSphere1Transform.rotation = Quaternion.Euler(ff9.w_effectLast1Rot);
                    ff9.world.SetAbsolutePositionOf(effectSphere1Transform, ff9.w_effectLastPos, 0f);
                    effectSphere2Transform.rotation = Quaternion.Euler(ff9.w_effectLast2Rot);
                    ff9.world.SetAbsolutePositionOf(effectSphere2Transform, ff9.w_effectLastPos, 0f);
                    for (Int32 i = 0; i < 10; i++)
                    {
                        if (ff9.w_effectLastDisC[i] == 0)
                        {
                            if (ff9.rand() % 5 == 0)
                            {
                                ff9.w_effectLastRotT[i].x = ff9.PsxRot(ff9.rand() % 4096);
                                ff9.w_effectLastRotT[i].y = ff9.PsxRot(ff9.rand() % 4096);
                                ff9.w_effectLastRotT[i].z = ff9.PsxRot(ff9.rand() % 4096);
                                ff9.w_effectLastDisT[i] = (Int16)(1024 + ff9.rand() % 1024);
                                ff9.w_effectLastDisC[i] = 1;
                            }
                        }
                        else
                        {
                            ff9.w_effectLastDisT[i] += (Int16)(ff9.rand() % 86 - 10);
                            SByte[] array2 = ff9.w_effectLastDisC;
                            if (++ff9.w_effectLastDisC[i] == 32)
                                ff9.w_effectLastDisC[i] = 0;
                        }
                        if (ff9.w_effectLastDisC[i] != 0)
                        {
                            Transform thunderTransform = null;
                            Int32 rnd = ff9.rand() % 3;
                            if (rnd == 0)
                                thunderTransform = ff9.world.GetkWorldPackEffectThunder1();
                            else if (rnd == 1)
                                thunderTransform = ff9.world.GetkWorldPackEffectThunder2();
                            if (thunderTransform != null)
                            {
                                ff9.world.SetAbsolutePositionOf(thunderTransform, ff9.w_effectLastPos, 0f);
                                thunderTransform.rotation = Quaternion.Euler(ff9.w_effectLastRotT[i]);
                                thunderTransform.localScale = new Vector3(ff9.PsxScale(12192), ff9.PsxScale(ff9.w_effectLastDisT[i]), ff9.PsxScale(6192));
                                Transform sparkle = thunderTransform.GetChild(0);
                                sparkle.localPosition = new Vector3(0f, 2.734375f, 0f);
                                Material material = sparkle.GetComponent<Renderer>().material;
                                Color color = material.GetColor("_TintColor");
                                color.a = (Byte)(255 - ff9.w_effectLastDisC[i] * 7) / 255f;
                                material.SetColor("_TintColor", color);
                            }
                        }
                    }
                    for (Int32 i = 0; i < 10; i++)
                    {
                        Int32 step = (ff9.rsin(ff9.w_frameCounter * 16 + i * 400) >> 5) + 128;
                        if (step == 0)
                            ff9.w_effectLastDis[i] = 1024;
                        else
                            ff9.w_effectLastDis[i] += 3;
                        step = Math.Min(step, 255);
                        Transform effectArchTransform = ff9.world.GetkWorldPackEffectArch();
                        ff9.world.SetAbsolutePositionOf(effectArchTransform, ff9.w_effectLastPos, 0f);
                        effectArchTransform.localScale = new Vector3(ff9.PsxScale(6192), ff9.PsxScale(6192), ff9.PsxScale(6192));
                        effectArchTransform.Rotate(new Vector3(0f, ff9.PsxRot(10 + i / 4), 0f), Space.Self);
                        Transform archSmoke = effectArchTransform.GetChild(0);
                        archSmoke.localPosition = new Vector3(0f, 0f, ff9.S(-ff9.w_effectLastDis[i] + 300));
                        Material smokeMat = archSmoke.GetComponent<Renderer>().material;
                        Color color = smokeMat.GetColor("_TintColor");
                        color.a = step / 255f;
                        smokeMat.SetColor("_TintColor", color);
                    }
                }
                break;
            case WorldEffect.Windmill:
            {
                Transform effectWindmillTransform = ff9.world.kWorldPackEffectWindmill;
                effectWindmillTransform.rotation = Quaternion.Euler(ff9.w_effectMillRot);
                ff9.world.SetAbsolutePositionOf(effectWindmillTransform, ff9.w_effectMillPos, 0f);
                break;
            }
        }
    }

    public static void w_effectUpdateSP(WorldEffect no)
    {
        Boolean useEffect = false;
        switch (no)
        {
            case WorldEffect.FireShrine:
                if (WorldConfiguration.UseWorldEffect(WorldEffect.FireShrine))
                {
                    ff9.w_textureScrollFire = true;
                    useEffect = true;
                }
                break;
            case WorldEffect.SandStorm:
            {
                ff9.w_effectTwisRot.y -= ff9.PsxRot(256);
                ff9.w_effectSpilRot[0].y -= ff9.PsxRot(-256);
                ff9.w_effectSpilRot[1].y -= ff9.PsxRot(64);
                ff9.w_effectSpilRot[2].y -= ff9.PsxRot(384);
                useEffect = true;
                break;
            }
            case WorldEffect.Memoria:
                ff9.w_effectLast1Rot.z = ff9.w_effectLast1Rot.z - ff9.PsxRot(8);
                ff9.w_effectLast1Rot.y = ff9.w_effectLast1Rot.y - ff9.PsxRot(16);
                ff9.w_effectLast2Rot.z = ff9.w_effectLast2Rot.z - ff9.PsxRot(16);
                ff9.w_effectLast2Rot.y = ff9.w_effectLast2Rot.y - ff9.PsxRot(8);
                for (Int32 i = 0; i < 10; i++)
                {
                    ff9.w_effectLastRotT[i].x += ff9.PsxRot(ff9.rand() % 13 - 6);
                    ff9.w_effectLastRotT[i].y += ff9.PsxRot(ff9.rand() % 13 - 6);
                }
                break;
            case WorldEffect.Windmill:
                if (WorldConfiguration.UseWorldEffect(WorldEffect.Windmill))
                    ff9.w_effectMillRot.y += ff9.PsxRot(64);
                break;
            default:
                if (WorldConfiguration.UseWorldEffect(no))
                    useEffect = true;
                break;
        }
        if (useEffect)
        {
            Byte noIndex = (Byte)no;
            if (noIndex >= ff9.w_effectDataList.Length)
            {
                global::Debug.LogWarning("no " + no + " doesn't exist");
                return;
            }
            for (Int32 i = 0; i < ff9.w_effectDataList[noIndex].effectData.Count; i++)
            {
                ff9.s_effectData s_effectData = ff9.w_effectDataList[noIndex].effectData[i];
                if (UnityEngine.Random.Range(0, 4096) < s_effectData.rnd)
                {
                    Int32 x = s_effectData.x - s_effectData.rx / 2;
                    Int32 y = s_effectData.y - s_effectData.ry / 2;
                    Int32 z = s_effectData.z - s_effectData.rz / 2;
                    if (s_effectData.rx != 0)
                        x += ff9.random16() % s_effectData.rx;
                    if (s_effectData.ry != 0)
                        y += ff9.random16() % s_effectData.ry;
                    if (s_effectData.rz != 0)
                        z += ff9.random16() % s_effectData.rz;
                    if (no == WorldEffect.SandPit)
                        y += ff9.UnityUnit(-1.229375f);
                    ff9.w_effectRegist_FixedPoint(x, y, z, s_effectData.no, s_effectData.size);
                }
            }
        }
    }

    public static void w_effectRegist(Single x, Single y, Single z, SPSConst.WorldSPSEffect no, Int32 size)
    {
        ff9.world.WorldSPSSystem.EffectRegist(x, y, z, no, size);
    }

    public static void w_effectRegist_Event(Int32 x, Int32 y, Int32 z, Int32 no, Int32 size)
    {
        Single x2 = ff9.S(x);
        Single y2 = ff9.S(-y);
        Single z2 = ff9.S(z);
        ff9.world.SetAbsolutePositionOf(out Vector3 absPos, new Vector3(x2, y2, z2));
        ff9.world.WorldSPSSystem.EffectRegist(absPos.x, absPos.y, absPos.z, (SPSConst.WorldSPSEffect)no, size);
    }

    public static SPSEffect w_effectRegist_FixedPoint(Int32 x, Int32 y, Int32 z, SPSConst.WorldSPSEffect no, Int32 size, Int32 frame = 0)
    {
        Single x2 = ff9.S(x);
        Single y2 = ff9.S(-y);
        Single z2 = ff9.S(z);
        ff9.world.SetAbsolutePositionOf(out Vector3 absPos, new Vector3(x2, y2, z2));
        Int32 index = ff9.world.WorldSPSSystem.EffectRegist(absPos.x, absPos.y, absPos.z, no, size);
        if (index < 0)
            return null;
        SPSEffect worldSPS = ff9.world.WorldSPSSystem.SpsList[index];
        worldSPS.curFrame = frame << 4;
        return worldSPS;
    }

    public static void w_fileSystemConstructor()
    {
        String[] array = new String[5];
        if (ff9.w_frameDisc == 1)
            array = ff9.w_fileImagenameServer1;
        else
            array = ff9.w_fileImagenameServer4;
        for (Int32 i = 0; i < 5; i++)
        {
            ff9.w_fileImagename[i] = array[i];
            ff9.w_fileImageTopSector[i] = 0;
        }
        Byte[] buffer = new Byte[2048];
        ff9.FF9Pc_SeekReadB(ff9.w_fileImagename[0], 0, 2048u, buffer);
        MemoryStream memoryStream = new MemoryStream(buffer);
        BinaryReader binaryReader = new BinaryReader(memoryStream);
        for (Int32 j = 0; j < ff9.w_fileImageSectorInfo.Length; j++)
        {
            ff9.w_fileImageSectorInfo[j].start = binaryReader.ReadInt32();
            ff9.w_fileImageSectorInfo[j].length = binaryReader.ReadInt32();
        }
        binaryReader.Close();
        memoryStream.Close();
    }

    public static void w_frameSystemConstructor()
    {
        if ((ff9.byte_gEventGlobal(101) & 1) != 0)
            ff9.FF9Wipe_WhiteOutEx(1);
        else
            ff9.FF9Wipe_FadeOutEx(1);
        ff9.w_frameScenePtr = ff9.ushort_gEventGlobal(0);
        ff9.w_frameDisc = WorldConfiguration.GetDisc();
        ff9.w_frameCameraPtr = GameObject.Find("WorldCamera").GetComponent<Camera>();
        ff9.rainRenderer = ff9.w_frameCameraPtr.gameObject.GetComponent<WorldRainRenderer>();
        ff9.w_frameCounter = 0;
        ff9.w_frameCounterReady = 0;
        ff9.w_framePhase = 0;
        ff9.w_frameRain = false;
        ff9.w_frameCloud = true;
        ff9.w_frameInternalSwitchEnable = true;
        ff9.w_frameShadowOTOffset = 0;
        ff9.w_musicSystemConstructor();
        ff9.w_memorySystemConstructor();
        ff9.w_fileSystemConstructor();
        ff9.w_cameraSystemConstructor();
        ff9.w_effectSystemConstructor();
        ff9.w_movementSystemConstructor();
        ff9.w_worldSystemConstructor();
        ff9.w_naviSystemConstructor();
        ff9.w_weatherSystemConstructor();
        ff9.w_lightSystemConstructor();
    }

    public static void w_frameMapConstructor()
    {
        ff9.w_movementChange();
        ff9.w_musicUpdate();
    }

    public static void w_frameMapConstructor2()
    {
        ff9.w_worldMapConstructor();
        ff9.w_movementMapConstructor();
        ff9.w_cameraMapConstructor();
        ff9.w_weatherMapConstructor();
        ff9.w_musicMapConstructor();
        ff9.w_movementChrConstructor();
    }

    public static Int32 w_frameMainRoutine()
    {
        ff9.w_frameInitialize = false;
        if (ff9.w_frameInternalSwitchEnable)
            ff9.FF9Global.worldState.internall = true;
        ff9.w_frameResult = 0;
        if ((ff9.FF9Global.attr & 2u) == 0u)
        {
            ff9.w_frameUpdate();
            if (ff9.w_frameCounter == ff9.kframeEventStartLoop + 1)
            {
                Boolean loadOnlyInSight = FF9StateSystem.World.LoadingType == WorldState.Loading.SplitAsync;
                ff9.world.LoadBlocks(false); // loadOnlyInSight
                ff9.w_movementChrInitSlice();
                ff9.w_blockReady = true;
            }
        }
        if (ff9.w_frameCounter == ff9.kframeEventStartLoop && ff9.w_framePhase == 0)
        {
            ff9.w_frameMapConstructor2();
            ff9.w_framePhase = 2;
            WMBeeMenu wmbeeMenu = UnityEngine.Object.FindObjectOfType<WMBeeMenu>();
            if (wmbeeMenu)
            {
                if (PersistenSingleton<SceneDirector>.Instance.CurrentScene == "WorldMapDebug")
                {
                    wmbeeMenu.ShowOnlyBackButton = false;
                    ff9.w_naviMapno = 1;
                }
                else
                {
                    wmbeeMenu.ShowOnlyBackButton = true;
                }
            }
            PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.WorldHUD);
        }
        ff9.w_frameService();
        ff9.w_frameCounter++;
        ff9.w_frameCounterReady++;
        if (ff9.w_frameResult == 3 && ff9.m_GetIDTopograph(ff9.m_moveActorID) == 52)
            ff9.w_frameResult = 0;
        if (ff9.w_frameInternalSwitchEnable)
            ff9.FF9Global.worldState.internall = false;
        ff9.totaltime = 0;
        ff9.totaltime += ff9.updatetime;
        ff9.totaltime += ff9.eventtime;
        ff9.totaltime += ff9.servicetime;
        return ff9.w_frameResult;
    }

    public static void w_frameUpdateEvent()
    {
        if ((ff9.w_framePhase == 0 || ff9.w_framePhase == 2) && (ff9.w_framePhase == 0 || ff9.w_framePhase == 2))
            ff9.w_frameResult = ff9.ServiceEvents();
        String wldLocName = ff9.w_worldLocationName();
        if (String.IsNullOrEmpty(wldLocName))
            PlayerWindow.Instance.SetTitle($"World Map: {FF9StateSystem.Common.FF9.wldMapNo}");
        else
            PlayerWindow.Instance.SetTitle($"World Map: {FF9StateSystem.Common.FF9.wldMapNo}, {wldLocName}");
    }

    public static String w_worldLocationName()
    {
        if (ff9.w_moveActorPtr == null || ff9.w_moveActorPtr.originalActor == null)
            return String.Empty;
        Int32 idall = ff9.w_moveCHRStatus[ff9.w_moveActorPtr.originalActor.index].id;
        if (ff9.m_GetIDArea(idall) != 0 || ff9.m_GetIDTopograph(idall) == 0 || ff9.m_GetIDTopograph(idall) == 37)
            return FF9TextTool.WorldLocationText(ff9.m_GetIDArea(idall));
        //if (ff9.w_moveCHRControl_No >= 0 && ff9.w_moveCHRControl_No < ff9.w_moveCHRControl.Length)
        //    return ff9.w_moveCHRControl_No.ToString();
        return String.Empty;
    }

    private static void w_frameService()
    {
        if (ff9.w_framePhase == 2)
        {
            if (ff9.w_naviMode != 2)
            {
                ff9.w_frameLine = 100u;
                ff9.w_movementService();
                ff9.w_frameLine = 101u;
                ff9.w_worldService();
                ff9.w_frameLine = 102u;
                ff9.w_naviService();
                ff9.w_frameLine = 103u;
                ff9.world.WorldSPSSystem.GenerateSPS();
                ff9.w_frameLine = 104u;
                ff9.rainRenderer.ServiceRain();
            }
            else
            {
                ff9.w_naviService();
            }
        }
    }

    public static void w_frameUpdate()
    {
        switch (ff9.w_framePhase)
        {
            case 0:
                ff9.w_frameUpdateEvent();
                if (ff9.GetControlChar())
                    ff9.w_moveActorPtr = ff9.GetControlChar();
                ff9.world.ActorList = ff9.GetActiveObjList();
                break;
            case 1:
            case 2:
                if (ff9.w_frameCounter == 10)
                {
                    ff9.w_movementChrFixBug();
                    ff9.w_movementChrFixBug_Chocobo();
                }
                if (ff9.w_naviMode != 2)
                {
                    ff9.world.OnUpdate20FPS();
                    ff9.w_movementUpdate();
                    ff9.w_frameUpdateEvent();
                    ff9.world.WorldSPSSystem.EffectUpdate();
                    ff9.w_cameraUpdate();
                    ff9.w_worldUpdate();
                    ff9.w_weatherUpdate();
                    ff9.w_textureUpdate();
                }
                else
                {
                    ff9.w_musicSEVolumeIntpl(38, 20, 0);
                    ff9.w_frameUpdateEvent();
                }
                break;
        }
        ff9.w_moveCHRControl_Move = ff9.UnityUnit(ff9.w_moveActorPtr.pos[0]) != ff9.UnityUnit(ff9.w_moveActorPtr.lastx) || ff9.UnityUnit(ff9.w_moveActorPtr.pos[1]) != ff9.UnityUnit(ff9.w_moveActorPtr.lasty) || ff9.UnityUnit(ff9.w_moveActorPtr.pos[2]) != ff9.UnityUnit(ff9.w_moveActorPtr.lastz);
        ff9.w_frameLine = 10u;
        ff9.w_musicUpdate();
    }

    public static WMPad w_getPadPush()
    {
        return ff9.kPadPush;
    }

    /*
    public static void w_frameSetParameter(Int32 function, Boolean value)
    {
        ff9.w_frameSetParameter(function, (!value) ? 0 : 1);
    }
    */

    public static void w_frameSetParameter(Int32 function, Int32 value)
    {
        Boolean flag = value == 1;
        switch (function)
        {
            case 0:
                if (value == 4)
                {
                    ff9.w_weatherAutoChange = true;
                    ff9.w_weatherModeOld = -1;
                }
                else
                {
                    ff9.w_weatherAutoChange = false;
                    ff9.w_weatherDest(value, 2, ff9.w_frameScriptParam[0]);
                }
                break;
            case 1:
                ff9.w_movementChange();
                break;
            case 2:
                ff9.w_naviActive = flag;
                PersistenSingleton<UIManager>.Instance.WorldHUDScene.SetMinimapVisible(flag);
                break;
            case 3:
                break;
            case 4:
                ff9.w_naviMode = value;
                break;
            case 5:
                ff9.w_cameraFixModeY = flag;
                break;
            case 6:
                ff9.w_frameScriptParam[0] = value;
                break;
            case 7:
                ff9.w_frameScriptParam[1] = value;
                break;
            case 8:
                ff9.w_frameScriptParam[2] = value;
                break;
            case 9:
                ff9.w_frameScriptParam[3] = value;
                break;
            case 10:
                ff9.w_effectRegist_Event(ff9.w_frameScriptParam[0], ff9.w_frameScriptParam[1], ff9.w_frameScriptParam[2], value, ff9.w_frameScriptParam[3]);
                break;
            case 11:
                ff9.w_naviFadeInTime = (UInt32)(value + 5);
                ff9.w_setNaviFadeInTime = true;
                break;
            case 14:
                ff9.w_frameScriptParam[0] = value;
                break;
            case 15:
                ff9.w_weatherAutoChange = flag;
                break;
            case 16:
                ff9.w_frameScriptParam[0] = value;
                break;
            case 17:
                ff9.w_frameScriptParam[1] = value;
                break;
            case 18:
                ff9.w_frameScriptParam[2] = value;
                break;
            case 19:
                ff9.w_frameScriptParam[3] = value;
                break;
            case 20:
                ff9.w_musicSEPlay(ff9.w_frameScriptParam[0], (Byte)ff9.w_frameScriptParam[1]);
                break;
            case 21:
                ff9.w_musicSEVolume(ff9.w_frameScriptParam[0], (Byte)ff9.w_frameScriptParam[1]);
                break;
            case 22:
                ff9.w_musicSEVolumeIntpl(ff9.w_frameScriptParam[0], (UInt16)ff9.w_frameScriptParam[2], (Byte)ff9.w_frameScriptParam[1]);
                break;
            case 23:
                ff9.w_musicSEStop(ff9.w_frameScriptParam[0]);
                break;
            case 24:
                FF9Snd.ff9wldsnd_song_vol_intplall(ff9.w_frameScriptParam[2], ff9.w_frameScriptParam[1]);
                break;
            case 25:
                ff9.w_cameraSmooth();
                break;
            case 26:
                ff9.w_frameEventBattleProb = (UInt16)value;
                break;
            case 27:
                if (flag)
                {
                    ff9.w_naviCursolMove = true;
                }
                else
                {
                    ff9.w_naviCursolMove = false;
                }
                PersistenSingleton<UIManager>.Instance.WorldHUDScene.ShowMapPointer(ff9.w_naviCursolMove);
                break;
            case 28:
            {
                Vector3 vector = new Vector3(ff9.w_frameScriptParam[0], ff9.w_frameScriptParam[1], ff9.w_frameScriptParam[2]) * 0.00390625f;
                if (!ff9.w_movementGetGetoff(ref vector))
                {
                    ff9.w_frameScriptParam[0] = 10000;
                    ff9.w_frameScriptParam[1] = -10000;
                    ff9.w_frameScriptParam[2] = 10000;
                }
                else
                {
                    ff9.w_frameScriptParam[0] = ff9.UnityUnit(vector.x);
                    ff9.w_frameScriptParam[1] = ff9.UnityUnit(vector.y);
                    ff9.w_frameScriptParam[2] = ff9.UnityUnit(vector.z);
                    Int32 num2 = ff9.w_frameScriptParam[0];
                    Int32 num3 = ff9.w_frameScriptParam[1];
                    Int32 num4 = ff9.w_frameScriptParam[2];
                    ff9.world.GetUnityPositionOf_FixedPoint(ref num2, ref num3, ref num4);
                    Int32 num5 = num2;
                    Int32 num6 = num3;
                    Int32 num7 = num4;
                    Vector3 vector2 = new Vector3(num5, num6, num7) * 0.00390625f;
                    WMPhysics.CastRayFromSky = true;
                    WMPhysics.IgnoreExceptions = true;
                    WMPhysics.UseInfiniteRaycast = true;
                    Int32 num8;
                    Int32 num9;
                    Single num10;
                    Boolean flag2 = ff9.w_nwpHitBool(ref vector2, out num8, out num9, null, out num10);
                    if (!flag2)
                    {
                        Boolean flag3 = false;
                        for (Int32 i = 20; i < 200; i += 20)
                        {
                            for (Int32 j = 20; j < 200; j += 20)
                            {
                                num5 = num2 + i;
                                num7 = num4 + j;
                                vector2 = new Vector3(num5, num6, num7) * 0.00390625f;
                                flag2 = ff9.w_nwpHitBool(ref vector2, out num8, out num9, null, out num10);
                                if (flag2)
                                {
                                    flag3 = true;
                                    break;
                                }
                                num5 = num2 - i;
                                num7 = num4 + j;
                                vector2 = new Vector3(num5, num6, num7) * 0.00390625f;
                                flag2 = ff9.w_nwpHitBool(ref vector2, out num8, out num9, null, out num10);
                                if (flag2)
                                {
                                    flag3 = true;
                                    break;
                                }
                                num5 = num2 + i;
                                num7 = num4 - j;
                                vector2 = new Vector3(num5, num6, num7) * 0.00390625f;
                                flag2 = ff9.w_nwpHitBool(ref vector2, out num8, out num9, null, out num10);
                                if (flag2)
                                {
                                    flag3 = true;
                                    break;
                                }
                                num5 = num2 - i;
                                num7 = num4 - j;
                                vector2 = new Vector3(num5, num6, num7) * 0.00390625f;
                                flag2 = ff9.w_nwpHitBool(ref vector2, out num8, out num9, null, out num10);
                            }
                            if (flag3)
                            {
                                break;
                            }
                        }
                    }
                    if (flag2)
                    {
                        ff9.world.GetAbsolutePositionOf_FixedPoint(ref num5, ref num6, ref num7);
                        ff9.w_frameScriptParam[0] = num5;
                        ff9.w_frameScriptParam[1] = num6;
                        ff9.w_frameScriptParam[2] = num7;
                    }
                    else
                    {
                        ff9.w_frameScriptParam[0] = 10000;
                        ff9.w_frameScriptParam[1] = -10000;
                        ff9.w_frameScriptParam[2] = 10000;
                    }
                    WMPhysics.CastRayFromSky = false;
                    WMPhysics.IgnoreExceptions = false;
                    WMPhysics.UseInfiniteRaycast = false;
                }
                global::Debug.Log("getoffPos = " + vector);
                Vector3 vector3 = vector * 256f;
                global::Debug.Log("getoffPosFP = " + vector3);
                break;
            }
            case 29:
                ff9.w_frameChocoboCall();
                break;
            case 30:
                ff9.w_movementChangeSP1();
                break;
            case 31:
                ff9.w_movementChangeSP2();
                break;
            case 32:
                //ff9.w_musicFade(value);
                break;
            case 33:
                FF9Snd.ff9wldsnd_song_vol_intplall(value, 0);
                break;
            case 34:
                ff9.w_movementAutoPilotON();
                break;
            case 35:
                if (flag)
                {
                    ff9.w_frameMogChoice = true;
                }
                else
                {
                    ff9.w_frameMogChoice = false;
                }
                break;
            case 36:
                if (flag)
                {
                    ff9.w_naviCursolDraw = true;
                }
                else
                {
                    ff9.w_naviCursolDraw = false;
                }
                break;
            case 37:
                ff9.w_moveCHRControl_XZAlpha = (ff9.w_moveCHRControl_YAlpha = 0f);
                break;
            case 38:
                ff9.w_movementAutoPilotOFF();
                break;
            case 39:
                ff9.w_frameCDUse = 3;
                break;
            case 40:
                ff9.w_frameCDUse = 0;
                break;
            case 500:
                if (value < 0 || value > 22)
                {
                    global::Debug.LogWarning("kframeChangeCharactorStatus_Debug: value is out of range.");
                }
                else
                {
                    global::Debug.Log("function kframeChangeCharactorStatus_Debug: " + value);
                    Int32 num = 0;
                    Int32 statusIndex = 0;
                    switch (value)
                    {
                        case 1:
                            num = 0;
                            statusIndex = 1;
                            break;
                        case 2:
                            num = 0;
                            statusIndex = 2;
                            break;
                        case 3:
                            num = 1;
                            statusIndex = 3;
                            break;
                        case 4:
                            num = 2;
                            statusIndex = 4;
                            break;
                        case 5:
                            num = 3;
                            statusIndex = 5;
                            break;
                        case 6:
                            num = 4;
                            statusIndex = 6;
                            break;
                        case 7:
                            num = 5;
                            statusIndex = 7;
                            break;
                        case 8:
                            num = 6;
                            statusIndex = 7;
                            break;
                        case 9:
                            num = 7;
                            statusIndex = 8;
                            break;
                        case 10:
                            num = 8;
                            statusIndex = 9;
                            break;
                        case 11:
                            num = 9;
                            statusIndex = 10;
                            break;
                    }
                    ff9.w_moveCHRControl_No = (SByte)num;
                    WMActor.SetControlledDebugActor(statusIndex, num);
                    ff9.w_movementChange();
                    switch (ff9.w_moveActorPtr.originalActor.index)
                    {
                        case 8:
                            ff9.w_movePlanePtr = ff9.w_moveActorPtr;
                            break;
                        case 9:
                        case 10:
                            ff9.w_movePlanePtr = ff9.w_moveActorPtr;
                            break;
                    }
                    ff9.w_movementChrInitSlice();
                }
                break;
            case 501:
                if (value < 2990 || value > 11090)
                {
                    global::Debug.LogWarning("kframeSetWorldMapState_Debug: value is out of range.");
                }
                else
                {
                    global::Debug.Log("function kframeSetWorldMapState_Debug: " + ff9.SC_COUNTER_ToString(value));
                    if (value < 11090)
                    {
                        Singleton<WMWorld>.Instance.ResetBlockForms();
                        ff9.w_frameDisc = 1;
                        ff9.w_frameScenePtr = (UInt16)value;
                        ff9.w_worldChangeBlockSet();
                        Singleton<WMWorld>.Instance.SetDisc(ff9.w_frameDisc);
                    }
                    else
                    {
                        ff9.w_frameDisc = 4;
                        ff9.w_frameScenePtr = (UInt16)value;
                        Singleton<WMWorld>.Instance.SetDisc(ff9.w_frameDisc);
                    }
                }
                break;
            case 502:
                global::Debug.Log("function kframeResetWorldBlockSet_Debug: " + value);
                ff9.w_frameDisc = 1;
                Singleton<WMWorld>.Instance.SetDisc(ff9.w_frameDisc);
                break;
        }
    }

    public static Int32 w_frameGetParameter(Int32 function)
    {
        Int32 idall = 0;
        if (ff9.w_moveActorPtr != null && ff9.w_moveActorPtr.originalActor != null)
            idall = ff9.w_moveCHRStatus[ff9.w_moveActorPtr.originalActor.index].id;
        switch (function)
        {
            case 192:
                return ff9.m_GetIDArea(idall);
            case 193:
                return ff9.m_GetIDTopograph(idall);
            case 194:
            {
                if (ff9.w_moveActorPtr == null)
                    return -1;
                if (ff9.w_moveActorPtr.originalActor == null)
                    return -1;
                Int32 num;
                if (ff9.w_moveCHRControl_No == 6)
                {
                    num = (Int32)(ff9.w_moveCHRStatus[ff9.w_moveActorPtr.originalActor.index].ground_height * 256f - 400f + 100f);
                    return -num;
                }
                num = (Int32)((ff9.w_moveCHRStatus[ff9.w_moveActorPtr.originalActor.index].ground_height + ff9.w_moveCHRStatus[ff9.w_moveActorPtr.originalActor.index].slice_height) * 256f);
                return -num;
            }
            case 195:
                return ff9.w_frameScriptParam[0];
            case 196:
                return -ff9.w_frameScriptParam[1];
            case 197:
                return ff9.w_frameScriptParam[2];
            case 198:
                return ff9.FF9Global.hintmap_id;
            case 199:
            {
                Single rotation = ff9.w_cameraSysDataCamera.rotation;
                Int32 num2 = EventEngineUtils.ConvertFloatAngleToFixedPoint(rotation);
                return num2 >> 4;
            }
            case 200:
                return 0;
            case 201:
                return (Int32)(ff9.w_moveActorPtr.RealPosition.x * 256f);
            case 202:
                return (Int32)(ff9.w_moveActorPtr.RealPosition.y * 256f);
            case 203:
                return (Int32)(ff9.w_moveActorPtr.RealPosition.z * 256f);
            case 204:
                return (Int32)(ff9.w_weatherDistanceEva * 256f);
            case 205:
            {
                if (FF9StateSystem.Settings.IsNoEncounter)
                    return 0;
                int num = 0;
                if (UIManager.World.CurrentState != WorldHUD.State.FullMap)
                {
                    long num3 = (long)Comn.random8();
                    long num4 = (long)Comn.random8();
                    long num5 = num3 << 8 | num4;
                    long num6 = num5 % (long)(ff9.w_frameEventBattleProb + 1);
                    if (ff9.w_frameEncountEnable && ff9.w_moveCHRControl_Move && ff9.m_GetIDTopograph(idall) >= 36 && ff9.m_GetIDTopograph(idall) <= 38 && ff9.w_frameCounter > 400 && num6 == 1L)
                        num = 1;
                }
                return num;
            }
            case 207:
            {
                Int32 area = ff9.m_GetIDArea(idall);
                return ff9.w_worldArea2Zone(area);
            }
            case 208:
                return (Int32)(ff9.w_moveCHRStatus[11].slice_height * 256f);
            case 209:
                if (ff9.w_framePhase == 2)
                    return 1;
                return 0;
            case 210:
                return ff9.w_naviLocationDraw;
        }
        return -1;
    }

    public static Int32 w_framePackExtractPosition(Byte[] data, Int32 no)
    {
        MemoryStream memoryStream = new MemoryStream(data);
        BinaryReader binaryReader = new BinaryReader(memoryStream);
        Int32 count = binaryReader.ReadInt32();
        if (no > count + 1)
            global::Debug.Log($"w_framePackExtractPosition requested pack {no} but there are only {count}");
        for (Int32 i = 0; i < no; i++)
            binaryReader.ReadInt32();
        Int32 pos = binaryReader.ReadInt32();
        memoryStream.Close();
        binaryReader.Close();
        return pos;
    }

    public static Int32 w_frameInter(Int32 from, Int32 to, Int32 t)
    {
        Int32 num = 0;
        num += from * (4096 - t) >> 12;
        return num + (to * t >> 12);
    }

    public static Int32 w_frameInter16(Int16 from, Int16 to, Int32 t, Int32 bit)
    {
        Int32 num = 0;
        Int32 num2 = 16 - bit;
        num += (from << num2) * (1024 - t / 4) >> 10;
        num += (to << num2) * (t / 4) >> 10;
        return num >> num2;
    }

    public static Single w_frameInter16(Single from, Single to, Int32 t, Int32 bit)
    {
        Single t2 = t / 4096f;
        return Mathf.Lerp(from, to, t2);
    }

    public static Color w_frameInter16(Color from, Color to, Int32 t, Int32 bit)
    {
        Single t2 = t / 4096f;
        return Color.Lerp(from, to, t2);
    }

    /*
    public static Int32 w_frameInter8(Char from, Char to, Int32 t)
    {
        Int32 num = 0;
        num += ((Int32)from << 10) * (4096 - t) >> 12;
        num += ((Int32)to << 10) * t >> 12;
        return num >> 10;
    }
    */

    public static Boolean w_frameChocoboCheck()
    {
        Byte index = ff9.w_moveActorPtr.originalActor.index;
        if (index == 1 || index == 2)
        {
            Int32 num = ff9.m_GetIDTopograph(ff9.m_moveActorID);
            switch (num)
            {
                case 3: // Chocobo tracks: Mist Continent (and archipelago)
                case 18: // Chocobo tracks: Outer Continent
                case 21: // Chocobo tracks: Forgotten Continent (north)
                case 22: // Chocobo tracks: Forgotten Continent (center)
                case 28: // Chocobo tracks: Lost Continent
                    return true;
                default:
                    return false;
            }
        }
        return false;
    }

    public static void w_frameChocoboCall()
    {
        ff9.w_movementChrInitSlice();
    }

    public static void w_lightSystemConstructor()
    {
        ff9.w_light[0] = GameObject.Find("DirectionalLight0").GetComponent<Light>();
        ff9.w_light[1] = GameObject.Find("DirectionalLight1").GetComponent<Light>();
        ff9.w_light[2] = GameObject.Find("DirectionalLight2").GetComponent<Light>();
        ff9.w_light[0].transform.rotation = Quaternion.LookRotation(Vector3.down);
        ff9.w_light[1].transform.rotation = Quaternion.LookRotation(Vector3.right);
        ff9.w_light[2].transform.rotation = Quaternion.LookRotation(new Vector3(0f, -1f, -4f).normalized);
        ff9.w_lightChangeStat();
    }

    public static void w_lightChangeStat()
    {
        //Color32 color = default(Color32);
        //color.r = (Byte)(ff9.w_weatherColor.Color[16].ambient.vx);
        //color.g = (Byte)(ff9.w_weatherColor.Color[16].ambient.vy);
        //color.b = (Byte)(ff9.w_weatherColor.Color[16].ambient.vz);
        //color.a = Byte.MaxValue;
        Single lightColorFactor = ff9.w_weatherColor.Color[16].lightColorFactor;
        Color a = default(Color);
        a.r = ff9.w_weatherColor.Color[16].light0.vx * 0.000244140625f;
        a.g = ff9.w_weatherColor.Color[16].light0.vy * 0.000244140625f;
        a.b = ff9.w_weatherColor.Color[16].light0.vz * 0.000244140625f;
        ff9.w_light[0].color = a * lightColorFactor;
        a.r = ff9.w_weatherColor.Color[16].light1.vx * 0.000244140625f;
        a.g = ff9.w_weatherColor.Color[16].light1.vy * 0.000244140625f;
        a.b = ff9.w_weatherColor.Color[16].light1.vz * 0.000244140625f;
        ff9.w_light[1].color = a * lightColorFactor;
        a.r = ff9.w_weatherColor.Color[16].light2.vx * 0.000244140625f;
        a.g = ff9.w_weatherColor.Color[16].light2.vy * 0.000244140625f;
        a.b = ff9.w_weatherColor.Color[16].light2.vz * 0.000244140625f;
        ff9.w_light[2].color = a * lightColorFactor;
    }

    public static void w_memorySystemConstructor()
    {
        ff9.w_memoryTextureanim = new Byte[43008];
        ff9.w_memorySPSData = new Byte[92160];
    }

    public static void w_movementSystemConstructor()
    {
        ff9.w_moveActorPtr = ff9.w_moveDummyCharacter;
        ff9.w_moveHumanPtr = null;
        ff9.w_movePlanePtr = null;
        ff9.w_moveChocoboPtr = null;
        ff9.w_moveMogPtr = null;
        ff9.w_moveCHRControl_No = 0;
        ff9.w_moveCHRControlPtr = null;
        ff9.w_moveCHRControl_Move = false;
        ff9.w_moveCHRControl_LR = false;
        ff9.w_moveCHRControl_XZSpeed = 0f;
        ff9.w_moveCHRControl_YSpeed = 0f;
        ff9.w_moveCHRControl_RotSpeed = 0f;
        ff9.w_moveCHRControl_XZAlpha = 0f;
        ff9.w_moveCHRControl_YAlpha = 0f;
        ff9.w_moveCHRControl_RotTrue = 0f;
        ff9.w_moveCHRControl_Polyno = 0u;
        ff9.w_movementCamRemain = 4096f;
        ff9.w_moveYDiff = 0f;
        ff9.w_moveDebugNochr = 1;
        ff9.w_moveDoping = false;
        ff9.w_moveDebugChocoboColorForce = 0;
        ff9.w_moveAutoPilot = 0;
        ff9.w_movementSoftRot = false;
        ff9.w_movePadLR = false;
        ff9.w_movePadDOWN = false;
        for (Int32 i = 0; i < 11; i++)
        {
        }
        for (Int32 i = 0; i < 22; i++)
        {
            ff9.w_moveCHRStatus[i].id = 0;
            ff9.w_moveCHRStatus[i].slice_height = 0f;
            ff9.w_moveCHRStatus[i].ground_height = 0f;
        }
        if (ff9.w_frameDisc == 4)
        {
            ff9.w_moveCHRControl[9].music = 3;
            ff9.w_moveCHRControl[0].music = 3;
        }
        else
        {
            ff9.w_moveCHRControl[9].music = 2;
            ff9.w_moveCHRControl[0].music = 0;
        }
    }

    public static void w_movementMapConstructor()
    {
        Int32 index = 0;
        for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                if (posObj.index >= 3 && posObj.index <= 7)
                {
                    switch (posObj.index)
                    {
                        case 3:
                            index = 0;
                            break;
                        case 4:
                            index = 1;
                            break;
                        case 5:
                            index = 2;
                            break;
                        case 6:
                            index = 3;
                            break;
                        case 7:
                            index = 4;
                            break;
                    }
                    if (!FF9StateSystem.World.IsBeeScene)
                    {
                        ff9.world.LoadCurrentChocoboTexture(index);
                        GameObject gameObject = ((Actor)posObj).wmActor.gameObject;
                        Transform transform = gameObject.transform.Find("GEO_SUB_W0_003(Clone)/mesh0");
                        if (transform == null)
                        {
                            transform = gameObject.transform.Find("308(Clone)/mesh0");
                        }
                        Transform transform2 = gameObject.transform.Find("GEO_SUB_W0_003(Clone)/mesh1");
                        if (transform2 == null)
                        {
                            transform2 = gameObject.transform.Find("308(Clone)/mesh1");
                        }
                        Transform transform3 = gameObject.transform.Find("GEO_SUB_W0_003(Clone)/mesh2");
                        if (transform3 == null)
                        {
                            transform3 = gameObject.transform.Find("308(Clone)/mesh2");
                        }
                        Renderer component = transform.GetComponent<Renderer>();
                        Material material = component.material;
                        material.mainTexture = ff9.world.CurrentChocoboTextures[1];
                        component = transform2.GetComponent<Renderer>();
                        material = component.material;
                        material.mainTexture = ff9.world.CurrentChocoboTextures[0];
                        component = transform3.GetComponent<Renderer>();
                        material = component.material;
                        material.mainTexture = ff9.world.CurrentChocoboTextures[2];
                    }
                }
                if (posObj.index == 8 && !FF9StateSystem.World.IsBeeScene)
                {
                    GameObject gameObject2 = ((Actor)posObj).wmActor.gameObject;
                    Transform transform4 = gameObject2.transform.Find("GEO_SUB_W0_008(Clone)");
                    if (transform4 == null)
                    {
                        transform4 = gameObject2.transform.Find("321(Clone)");
                    }
                    Vector3 localPosition = transform4.localPosition;
                    localPosition.z = 1f;
                    transform4.localPosition = localPosition;
                }
            }
        }
    }

    public static void w_movementChrConstructor()
    {
        for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                switch (posObj.index)
                {
                    case 1:
                    case 2:
                        ff9.w_moveHumanPtr = ((Actor)posObj).wmActor;
                        break;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        ff9.w_moveChocoboPtr = ((Actor)posObj).wmActor;
                        break;
                    case 8:
                        ff9.w_movePlanePtr = ((Actor)posObj).wmActor;
                        break;
                    case 9:
                    case 10:
                        ff9.w_movePlanePtr = ((Actor)posObj).wmActor;
                        break;
                }
                if (posObj == ff9.GetControlChar_PosObj())
                {
                    if (ff9.FF9Sys.prevMode == 1 && !ff9.FF9Global.worldState.internall)
                    {
                        if (ff9.w_moveCHRControlPtr.camrot)
                        {
                            ff9.w_cameraSysDataCamera.rotationRev = ff9.PsxRot(2048);
                        }
                        else
                        {
                            ff9.w_cameraSysDataCamera.rotationRev = ff9.PsxRot(0);
                        }
                        ff9.w_cameraSysDataCamera.rotation = posObj.rot[1] + ff9.w_cameraSysDataCamera.rotationRev;
                    }
                    ff9.w_moveCHRControl_RotTrue = ((Actor)posObj).wmActor.rot[1];
                }
            }
        }
        ff9.FF9Global.worldState.loadgameflg = 0;
        ff9.w_frameInternalSwitchEnable = true;
    }

    public static void w_movementChrInitSlice()
    {
        WMPhysics.UseInfiniteRaycast = true;
        WMPhysics.CastRayFromSky = true;
        for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                Vector3 pos = ((Actor)posObj).wmActor.pos;
                ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[posObj.index];
                Boolean flag = false;
                Int32 num;
                Single ground_height;
                ff9.w_cellHit(ref pos, ref s_moveCHRStatus.id, out num, null, out ground_height);
                s_moveCHRStatus.ground_height = ground_height;
                s_moveCHRStatus.slice_height = ff9.w_movementGetSliceHeight(s_moveCHRStatus.slice_type, s_moveCHRStatus.id, ref flag);
                ff9.w_movementSetheight(posObj);
                posObj.lastx = posObj.pos[0];
            }
        }
        WMPhysics.CastRayFromSky = false;
        WMPhysics.UseInfiniteRaycast = false;
    }

    public static void w_movementChrFixBug()
    {
        WMActor wmactor = null;
        WMActor wmactor2 = null;
        WMActor wmactor3 = null;
        WMActor wmactor4 = null;
        for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                if (obj.index == 1)
                {
                    wmactor = ((Actor)posObj).wmActor;
                }
                if (obj.index >= 3 && obj.index <= 7)
                {
                    wmactor2 = ((Actor)posObj).wmActor;
                }
                if (obj.index == 9 || obj.index == 10)
                {
                    wmactor3 = ((Actor)posObj).wmActor;
                }
            }
        }
        if (wmactor == null)
        {
            return;
        }
        if (wmactor2 != null)
        {
            wmactor4 = wmactor2;
        }
        if (wmactor3 != null)
        {
            wmactor4 = wmactor3;
        }
        if (wmactor2 != null && wmactor3 != null)
        {
            Single num = Vector3.Distance(wmactor.pos, wmactor2.pos);
            Single num2 = Vector3.Distance(wmactor.pos, wmactor3.pos);
            if (num < num2)
            {
                wmactor4 = wmactor2;
            }
            else
            {
                wmactor4 = wmactor3;
            }
        }
        Single x = wmactor.pos.x;
        Single y = wmactor.pos.y;
        Single z = wmactor.pos.z;
        Single x2 = x;
        Single y2 = y;
        Single z2 = z;
        Vector3 vector = new Vector3(x2, y2, z2);
        WMPhysics.CastRayFromSky = true;
        WMPhysics.IgnoreExceptions = true;
        WMPhysics.UseInfiniteRaycast = true;
        Int32 num3;
        Int32 num4;
        Single y3;
        Boolean flag = ff9.w_nwpHitBool(ref vector, out num3, out num4, null, out y3);
        if (flag)
        {
            return;
        }
        Vector3 nearestObjectPos = wmactor.pos;
        if (wmactor4 != null)
        {
            nearestObjectPos = wmactor4.pos;
        }
        List<Vector3> list = new List<Vector3>();
        for (Int32 i = 0; i < 360; i += 45)
        {
            Vector3 pos = ff9.w_moveActorPtr.pos;
            pos.x += ff9.rcos((Single)i) * 0.2f;
            pos.z += ff9.rsin((Single)i) * 0.2f;
            list.Add(pos);
        }
        list.Sort(delegate (Vector3 p1, Vector3 p2)
        {
            Vector3 nop = nearestObjectPos;
            nop.y = 0f;
            p1.y = 0f;
            p2.y = 0f;
            Single num5 = Vector3.Distance(p1, nop);
            Single num6 = Vector3.Distance(p2, nop);
            if (num5 > num6)
            {
                return -1;
            }
            if (num5 < num6)
            {
                return 1;
            }
            return 0;
        });
        for (Int32 j = 0; j < list.Count; j++)
        {
            vector = list[j];
            flag = ff9.w_nwpHitBool(ref vector, out num3, out num4, null, out y3);
            if (flag)
            {
                break;
            }
        }
        if (flag)
        {
            wmactor.pos = new Vector3(x2, y3, z2);
        }
        else if (wmactor4)
        {
            wmactor.pos = wmactor4.pos;
        }
        WMPhysics.CastRayFromSky = false;
        WMPhysics.IgnoreExceptions = false;
        WMPhysics.UseInfiniteRaycast = false;
    }

    public static void w_movementChrFixBug_Chocobo()
    {
        WMActor wmactor = null;
        for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                if (obj.index == 1)
                {
                    WMActor wmActor = ((Actor)posObj).wmActor;
                }
                if (obj.index >= 3 && obj.index <= 7)
                {
                    wmactor = ((Actor)posObj).wmActor;
                }
                if (obj.index == 9 || obj.index == 10)
                {
                    WMActor wmActor2 = ((Actor)posObj).wmActor;
                }
            }
        }
        if (wmactor == null)
        {
            return;
        }
        Single x = wmactor.pos.x;
        Single y = wmactor.pos.y;
        Single z = wmactor.pos.z;
        Single x2 = x;
        Single y2 = y;
        Single z2 = z;
        Vector3 vector = new Vector3(x2, y2, z2);
        WMPhysics.CastRayFromSky = true;
        WMPhysics.IgnoreExceptions = true;
        WMPhysics.UseInfiniteRaycast = true;
        Int32 num;
        Int32 num2;
        Single y3;
        if (!ff9.w_nwpHitBool(ref vector, out num, out num2, null, out y3))
        {
            vector.x += 0.1f;
        }
        if (!ff9.w_nwpHitBool(ref vector, out num, out num2, null, out y3))
        {
            vector.x += 0.2f;
        }
        Boolean flag = ff9.w_nwpHitBool(ref vector, out num, out num2, null, out y3);
        if (flag)
        {
            wmactor.pos = new Vector3(vector.x, y3, vector.z);
            wmactor.lastx = wmactor.pos.x;
            wmactor.lasty = wmactor.pos.y;
            wmactor.lastz = wmactor.pos.z;
        }
        WMPhysics.CastRayFromSky = false;
        WMPhysics.IgnoreExceptions = false;
        WMPhysics.UseInfiniteRaycast = false;
    }

    public static void w_movementChrVerifyValidCastPosition(ref Int32 posX, ref Int32 posY, ref Int32 posZ)
    {
        Int32 num = posX;
        Int32 num2 = posY;
        Int32 num3 = posZ;
        ff9.world.GetUnityPositionOf_FixedPoint(ref num, ref num2, ref num3);
        Int32 num4 = num;
        Int32 num5 = num2;
        Int32 num6 = num3;
        Vector3 vector = new Vector3(num4, num5, num6) * 0.00390625f;
        WMPhysics.CastRayFromSky = true;
        WMPhysics.IgnoreExceptions = true;
        WMPhysics.UseInfiniteRaycast = true;
        Int32 num7;
        Int32 num8;
        Single num9;
        Boolean flag = ff9.w_nwpHitBool(ref vector, out num7, out num8, null, out num9);
        if (!flag)
        {
            Boolean flag2 = false;
            for (Int32 i = 20; i < 200; i += 20)
            {
                for (Int32 j = 20; j < 200; j += 20)
                {
                    num4 = num + i;
                    num6 = num3 + j;
                    vector = new Vector3(num4, num5, num6) * 0.00390625f;
                    flag = ff9.w_nwpHitBool(ref vector, out num7, out num8, null, out num9);
                    if (flag)
                    {
                        flag2 = true;
                        break;
                    }
                    num4 = num - i;
                    num6 = num3 + j;
                    vector = new Vector3(num4, num5, num6) * 0.00390625f;
                    flag = ff9.w_nwpHitBool(ref vector, out num7, out num8, null, out num9);
                    if (flag)
                    {
                        flag2 = true;
                        break;
                    }
                    num4 = num + i;
                    num6 = num3 - j;
                    vector = new Vector3(num4, num5, num6) * 0.00390625f;
                    flag = ff9.w_nwpHitBool(ref vector, out num7, out num8, null, out num9);
                    if (flag)
                    {
                        flag2 = true;
                        break;
                    }
                    num4 = num - i;
                    num6 = num3 - j;
                    vector = new Vector3(num4, num5, num6) * 0.00390625f;
                    flag = ff9.w_nwpHitBool(ref vector, out num7, out num8, null, out num9);
                }
                if (flag2)
                {
                    break;
                }
            }
        }
        if (flag)
        {
            ff9.world.GetAbsolutePositionOf_FixedPoint(ref num4, ref num5, ref num6);
            posX = num4;
            posY = num5;
            posZ = num6;
        }
        WMPhysics.CastRayFromSky = false;
        WMPhysics.IgnoreExceptions = false;
        WMPhysics.UseInfiniteRaycast = false;
    }

    /*
    public static void w_movementChrInitSlice(Int32 uid)
    {
        WMPhysics.UseInfiniteRaycast = true;
        WMPhysics.CastRayFromSky = true;
        for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                if (obj.uid == uid)
                {
                    WMActor wmActor = ((Actor)posObj).wmActor;
                    if (!(wmActor == null))
                    {
                        Vector3 pos = wmActor.pos;
                        ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[posObj.index];
                        Boolean flag = false;
                        Int32 num;
                        Single ground_height;
                        ff9.w_cellHit(ref pos, ref s_moveCHRStatus.id, out num, null, out ground_height);
                        s_moveCHRStatus.ground_height = ground_height;
                        s_moveCHRStatus.slice_height = ff9.w_movementGetSliceHeight(s_moveCHRStatus.slice_type, s_moveCHRStatus.id, ref flag);
                        ff9.w_movementSetheight(posObj);
                        posObj.lastx = posObj.pos[0];
                    }
                }
            }
        }
        WMPhysics.CastRayFromSky = false;
        WMPhysics.UseInfiniteRaycast = false;
    }
    */

    public static void w_movementService()
    {
        for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                if (ff9.objIsVisible(obj))
                {
                    Boolean useOwnShadow = true;
                    Vector3 pos = ((Actor)obj).wmActor.pos;
                    if (posObj.index == 8)
                        useOwnShadow = false;
                    if ((posObj.index == 1 || posObj.index == 2) && ff9.w_moveCHRControl_No >= 1 && ff9.w_moveCHRControl_No <= 6)
                        useOwnShadow = false;
                    Boolean displayShadow = useOwnShadow && ff9.w_moveCHRStatus[posObj.index].slice_height >= -0.01171875f;
                    ff9.w_FF9DisplayShadow(posObj, ref pos, 0, displayShadow);
                }
                else
                {
                    Vector3 pos2 = ((Actor)obj).wmActor.pos;
                    ff9.w_FF9DisplayShadow(posObj, ref pos2, 0, false);
                }
            }
        }
    }

    public static void w_FF9DisplayShadow(PosObj chr, ref Vector3 pos, Int32 sz, Boolean display)
    {
        Int32 modelID = chr.modelID;
        WMShadow wmshadow = ff9.world.GetShadow(chr);
        if (wmshadow == null)
        {
            if (chr.index == 1 || chr.index == 2)
            {
                Single scale = 0.8545044f;
                wmshadow = ff9.world.AddShadow(chr, new Vector3(scale, scale, scale));
            }
            else if (chr.index >= 3 && chr.index <= 7)
            {
                Single scale = 1.430598f;
                wmshadow = ff9.world.AddShadow(chr, new Vector3(scale, scale, scale));
            }
            else if (chr.index == 9 || chr.index == 10)
            {
                Single scale = 1.430598f;
                wmshadow = ff9.world.AddShadow(chr, new Vector3(scale, scale, scale));
            }
            else
            {
                Single scale = 1f;
                wmshadow = ff9.world.AddShadow(chr, new Vector3(scale, scale, scale));
            }
        }
        if (display)
        {
            if (!wmshadow.gameObject.activeSelf)
            {
                wmshadow.gameObject.SetActive(true);
            }
        }
        else if (wmshadow.gameObject.activeSelf)
        {
            wmshadow.gameObject.SetActive(false);
            return;
        }
        ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[chr.index];
        if (chr.index != 14 && wmshadow)
        {
            Int32 num2 = (Int32)(s_moveCHRStatus.ground_height * 256f - chr.pos[1]);
            pos.y += ff9.S(num2);
            pos.y -= ff9.S(num2) / 8f;
            Int32 num3 = 3096 - num2;
            num3 = 4096 - num2 / 2;
            Int32 num4 = 4096 - num2 / 4;
            ff9.w_frameShadowOTOffset = (Int16)(-s_moveCHRStatus.shadow_size * 4 - s_moveCHRStatus.shadow_offset);
            if (s_moveCHRStatus.shadow_size != 0)
            {
                num4 = s_moveCHRStatus.shadow_size * num4 >> 11;
                num3 = s_moveCHRStatus.shadow_amp * num3 >> 12;
                Int32 num5 = 32768 - sz >> 3;
                num5 *= ff9.w_weatherColor.Color[16].fogAMP;
                num5 >>= 12;
                if (num5 < 0)
                {
                    num5 = 0;
                }
                if (num5 > 4095)
                {
                    num5 = 4095;
                }
                num3 *= num5;
                num3 >>= 12;
                if (num3 > 255)
                {
                    num3 = 255;
                }
                if (num3 < 0)
                {
                    num3 = 0;
                }
                Single num6 = ff9.tweaker.ShadowScale;
                num6 = 300f;
                Single num7 = ff9.PsxScale(num4) * num6;
                wmshadow.transform.localScale = new Vector3(num7, num7, num7);
                Single value = 1f - num3 / 100f;
                wmshadow.Material.SetFloat("_Amp", value);
                Single num8 = 0.1f;
                if (chr.index == 8 || chr.index == 9)
                {
                    num8 = 0.6f;
                }
                wmshadow.transform.position = new Vector3(pos.x, s_moveCHRStatus.ground_height + num8, pos.z);
            }
        }
    }

    public static void w_movementUpdate()
    {
        ff9.w_frameEncountEnable = false;
        ff9.w_moveCHRControl_XZSpeed = 0f;
        ff9.w_moveCHRControl_YSpeed = 0f;
        ff9.w_moveCHRControl_RotSpeed = 0f;
        if (ff9.GetControlChar() != null)
        {
            ff9.w_moveActorPtr = ff9.GetControlChar();
            ff9.w_moveActorPtr.lastx = ff9.w_moveActorPtr.pos0;
            ff9.w_moveActorPtr.lasty = ff9.w_moveActorPtr.pos1;
            ff9.w_moveActorPtr.lastz = ff9.w_moveActorPtr.pos2;
        }
        for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[posObj.index];
                if (ff9.objIsVisible(obj))
                {
                    Byte index = posObj.index;
                    if (index != 7)
                    {
                        if (index == 11)
                        {
                            ff9.w_moveMogPtr = ((Actor)posObj).wmActor;
                        }
                    }
                    else
                    {
                        s_moveCHRStatus.flg_fly = (Byte)(ff9.w_moveCHRControl_No != 6 ? 0 : 1);
                    }
                    if ((Actor)posObj == ff9.w_moveActorPtr.originalActor && ff9.GetUserControl())
                    {
                        ff9.w_movementControl(s_moveCHRStatus);
                    }
                    else
                    {
                        WMPhysics.UseInfiniteRaycast = true;
                        WMPhysics.CastRayFromSky = (ff9.w_moveCHRStatus[(int)posObj.index].flg_fly != 0);
                        WMPhysics.IgnoreExceptions = true;
                        s_moveCHRStatus = ff9.w_moveCHRStatus[(int)posObj.index];
                        ff9.s_moveCHRCache s_moveCHRCache = ff9.w_moveCHRCache[(int)s_moveCHRStatus.cache];
                        Vector3 pos = ((Actor)posObj).wmActor.pos;
                        int num;
                        float ground_height;
                        ff9.w_cellHit(ref pos, ref s_moveCHRStatus.id, out num, null, out ground_height);
                        if (num == -1 && !WMPhysics.CastRayFromSky)
                        {
                            WMPhysics.CastRayFromSky = true;
                            ff9.w_cellHit(ref pos, ref s_moveCHRStatus.id, out num, null, out ground_height);
                        }
                        if (posObj.index == 8)
                        {
                            ground_height = 0f;
                        }
                        if (s_moveCHRStatus.id == 0xFEE && (posObj.index == 1 || posObj.index == 2 || posObj.index == 11 || posObj.index == 3 || posObj.index == 4 || posObj.index == 5 || posObj.index == 6 || (posObj.index == 7 && s_moveCHRStatus.flg_fly == 0)))
                        {
                            ground_height = ((Actor)posObj).wmActor.pos1;
                            s_moveCHRStatus.id = 0xFD2;
                        }
                        if (s_moveCHRStatus.id == 0x18EE && (posObj.index == 5 || posObj.index == 6 || (posObj.index == 7 && s_moveCHRStatus.flg_fly == 0)))
                        {
                            ground_height = ((Actor)posObj).wmActor.pos1;
                            s_moveCHRStatus.id = 0xC4;
                        }
                        if (s_moveCHRStatus.id == 0x7EE && (posObj.index == 5 || posObj.index == 6 || (posObj.index == 7 && s_moveCHRStatus.flg_fly == 0)))
                        {
                            ground_height = ((Actor)posObj).wmActor.pos1;
                            s_moveCHRStatus.id = 0xC4;
                        }
                        if (s_moveCHRStatus.id == 0x1BEE && (posObj.index == 1 || posObj.index == 2 || posObj.index == 11 || posObj.index == 3 || posObj.index == 4 || posObj.index == 5 || posObj.index == 6 || (posObj.index == 7 && s_moveCHRStatus.flg_fly == 0)))
                        {
                            ground_height = ((Actor)posObj).wmActor.pos1;
                            s_moveCHRStatus.id = 0x1B44;
                        }
                        if (s_moveCHRStatus.id == 0x2CEE && (posObj.index == 1 || posObj.index == 2 || posObj.index == 11 || posObj.index == 3 || posObj.index == 4 || posObj.index == 5 || posObj.index == 6 || (posObj.index == 7 && s_moveCHRStatus.flg_fly == 0)))
                        {
                            ground_height = ((Actor)posObj).wmActor.pos1;
                            s_moveCHRStatus.id = 0x6C4C;
                        }
                        s_moveCHRStatus.ground_height = ground_height;
                        ((Actor)posObj).wmActor.pos = pos;
                        WMPhysics.UseInfiniteRaycast = false;
                        WMPhysics.CastRayFromSky = false;
                        WMPhysics.IgnoreExceptions = false;
                    }
                    ff9.w_movementSetheight(posObj);
                }
            }
        }
        switch (ff9.w_moveCHRControlPtr.type)
        {
            case 0:
            case 3:
                if (ff9.w_blockReady)
                {
                    ff9.w_movementHumanCamOperation();
                }
                break;
            case 1:
            case 2:
                ff9.w_movementPlaneCamOperation();
                break;
        }
        if (ff9.w_moveCHRControl_No >= 1 && ff9.w_moveCHRControl_No <= 6)
        {
            ff9.w_moveCHRStatus[1].slice_height = ff9.w_moveCHRStatus[ff9.w_moveActorPtr.originalActor.index].slice_height;
        }
        if (ff9.w_moveChocoboPtr)
        {
            Vector3 rot = ff9.w_moveChocoboPtr.rot;
            if (rot[2] > 180f)
            {
                Int32 index3;
                Int32 index2 = index3 = 2;
                Single num2 = rot[index3];
                rot[index2] = num2 - 360f;
            }
            if (rot[2] < 0f)
            {
                Int32 index3;
                Int32 index4 = index3 = 2;
                Single num2 = rot[index3];
                rot[index4] = num2 + ff9.PsxRot(10);
            }
            if (rot[2] > 0f)
            {
                Int32 index3;
                Int32 index5 = index3 = 2;
                Single num2 = rot[index3];
                rot[index5] = num2 - ff9.PsxRot(10);
            }
            if (rot[2] < ff9.PsxRot(10) && rot[2] > ff9.PsxRot(-10))
            {
                rot[2] = 0f;
            }
            ff9.w_moveChocoboPtr.rot = rot;
        }
        if (ff9.w_movePlanePtr)
        {
            Vector3 rot2 = ff9.w_movePlanePtr.rot;
            if (rot2[2] > 180f)
            {
                Int32 index3;
                Int32 index6 = index3 = 2;
                Single num2 = rot2[index3];
                rot2[index6] = num2 - 360f;
            }
            Single num3 = ff9.PsxRot(14);
            if (rot2[2] < num3)
            {
                Int32 index3;
                Int32 index7 = index3 = 2;
                Single num2 = rot2[index3];
                rot2[index7] = num2 + ff9.PsxRot(10);
            }
            if (rot2[2] > num3)
            {
                Int32 index3;
                Int32 index8 = index3 = 2;
                Single num2 = rot2[index3];
                rot2[index8] = num2 - ff9.PsxRot(10);
            }
            if (rot2[2] < num3 + ff9.PsxRot(10) && rot2[2] > num3 - ff9.PsxRot(10))
            {
                rot2[2] = num3;
            }
            ff9.w_movePlanePtr.rot = rot2;
        }
        if (ff9.GetControlChar() != null)
        {
            if (ff9.UnityRot(ff9.w_moveCHRControl_RotSpeed) == 0 || ff9.w_moveCHRControl_XZSpeed < ff9.S(300))
            {
                if (ff9.w_cameraRotAngle > 0f)
                {
                    ff9.w_cameraRotAngle -= ff9.PsxRot(10);
                    if (ff9.w_cameraRotAngle < 0f)
                    {
                        ff9.w_cameraRotAngle = 0f;
                    }
                }
                if (ff9.w_cameraRotAngle < 0f)
                {
                    ff9.w_cameraRotAngle += ff9.PsxRot(10);
                    if (ff9.w_cameraRotAngle > 0f)
                    {
                        ff9.w_cameraRotAngle = 0f;
                    }
                }
            }
            ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[ff9.w_moveActorPtr.originalActor.index];
            if (ff9.m_GetIDEvent(s_moveCHRStatus.id) != 0 && ff9.w_frameEventEnable)
            {
                Int32 num4;
                ff9.w_worldPos2Cell(ff9.w_moveActorPtr.RealPosition[0], ff9.w_moveActorPtr.RealPosition[2], out num4);
                Int32 x = num4 % 48;
                Int32 z = num4 / 48;
                Int32 id = ff9.m_GetIDEvent(s_moveCHRStatus.id);
                ff9.WorldEvent(x, z, id);
            }
        }
    }

    /*
    private static void w_Test()
    {
        WMActor wmactor = null;
        WMActor wmactor2 = null;
        WMActor wmactor3 = null;
        WMActor wmactor4 = null;
        for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                if (obj.uid == 12)
                {
                    wmactor = ((Actor)posObj).wmActor;
                }
                if (obj.uid == 5)
                {
                    wmactor2 = ((Actor)posObj).wmActor;
                }
                if (obj.uid == 6)
                {
                    wmactor3 = ((Actor)posObj).wmActor;
                }
            }
        }
        if (wmactor == null)
        {
            return;
        }
        if (wmactor2 != null)
        {
            wmactor4 = wmactor2;
        }
        if (wmactor3 != null)
        {
            wmactor4 = wmactor3;
        }
        if (wmactor2 != null && wmactor3 != null)
        {
            Single num = Vector3.Distance(wmactor.pos, wmactor2.pos);
            Single num2 = Vector3.Distance(wmactor.pos, wmactor3.pos);
            if (num < num2)
            {
                wmactor4 = wmactor2;
            }
            else
            {
                wmactor4 = wmactor3;
            }
        }
        Single x = wmactor.pos.x;
        Single y = wmactor.pos.y;
        Single z = wmactor.pos.z;
        Single x2 = x;
        Single y2 = y;
        Single z2 = z;
        Vector3 vector = new Vector3(x2, y2, z2);
        WMPhysics.CastRayFromSky = true;
        WMPhysics.IgnoreExceptions = true;
        WMPhysics.UseInfiniteRaycast = true;
        Int32 num3;
        Int32 num4;
        Single y3;
        Boolean flag = ff9.w_nwpHitBool(ref vector, out num3, out num4, null, out y3);
        Vector3 nearestObjectPos = wmactor.pos;
        if (wmactor4 != null)
        {
            nearestObjectPos = wmactor4.pos;
        }
        List<Vector3> list = new List<Vector3>();
        for (Int32 i = 0; i < 360; i += 45)
        {
            Vector3 pos = ff9.w_moveActorPtr.pos;
            pos.x += ff9.rcos((Single)i) * 2f;
            pos.z += ff9.rsin((Single)i) * 2f;
            list.Add(pos);
        }
        list.Sort(delegate (Vector3 p1, Vector3 p2)
        {
            Vector3 nop = nearestObjectPos;
            nop.y = 0f;
            p1.y = 0f;
            p2.y = 0f;
            Single num5 = Vector3.Distance(p1, nop);
            Single num6 = Vector3.Distance(p2, nop);
            if (num5 > num6)
            {
                return -1;
            }
            if (num5 < num6)
            {
                return 1;
            }
            return 0;
        });
        for (Int32 j = 0; j < list.Count; j++)
        {
            Color color = Color.green;
            if (j == ff9.tweaker.testIndex)
            {
                color = Color.magenta;
            }
            Vector3 start = list[j];
            start.y = 100f;
            Vector3 end = list[j];
            global::Debug.DrawLine(start, end, color, ff9.honoUpdateTime, true);
        }
        if (flag)
        {
            wmactor.pos = new Vector3(x2, y3, z2);
        }
        WMPhysics.CastRayFromSky = false;
        WMPhysics.IgnoreExceptions = false;
        WMPhysics.UseInfiniteRaycast = false;
    }
    */

    public static void w_movementSetheight(PosObj chr)
    {
        if (ff9.w_blockReady)
        {
            ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[chr.index];
            if (s_moveCHRStatus.cache != 10)
            {
                Boolean flag = false;
                Single num = ff9.w_movementGetSliceHeight(s_moveCHRStatus.slice_type, s_moveCHRStatus.id, ref flag);
                if (s_moveCHRStatus.flg_fly == 0)
                {
                    if (s_moveCHRStatus.slice_type != 0)
                    {
                        if (s_moveCHRStatus.control == 7)
                        {
                            s_moveCHRStatus.slice_height = ff9.S(-180);
                        }
                        else if (flag || ff9.UnityUnit(num) == 0)
                        {
                            s_moveCHRStatus.slice_height = num;
                        }
                        else
                        {
                            if (s_moveCHRStatus.slice_height < num - 0.08203125f)
                            {
                                s_moveCHRStatus.slice_height += 0.078125f;
                            }
                            if (s_moveCHRStatus.slice_height > num + 0.08203125f)
                            {
                                s_moveCHRStatus.slice_height -= 0.078125f;
                            }
                        }
                    }
                    ((Actor)chr).wmActor.pos1 = s_moveCHRStatus.ground_height + s_moveCHRStatus.slice_height;
                }
                else
                {
                    Single num2 = s_moveCHRStatus.ground_height + num;
                    if (((Actor)chr).wmActor.pos1 < num2)
                    {
                        ((Actor)chr).wmActor.pos1 = num2;
                    }
                    if (ff9.w_moveActorPtr != null && !ff9.w_cameraFixMode && ff9.w_moveActorPtr.pos[1] > 42.1875f && !ff9.w_cameraFixModeY)
                    {
                        ff9.w_moveActorPtr.pos1 = 42.1875f;
                    }
                    s_moveCHRStatus.slice_height = ((Actor)chr).wmActor.pos1 - s_moveCHRStatus.ground_height;
                    if (s_moveCHRStatus.slice_height > 0f)
                    {
                        s_moveCHRStatus.slice_height = 0f;
                    }
                }
            }
            ((Actor)chr).wmActor.SetFogByHeight();
        }
    }

    public static void w_movementControl(ff9.s_moveCHRStatus status)
    {
        if (ff9.w_moveCHRControlPtr.encount && ff9.w_moveCHRControl_Move && ff9.w_frameGetParameter(193) != 52 && ff9.w_naviTitle == -1)
        {
            ff9.w_frameEncountEnable = ff9.w_frameEncountMask;
        }
        if (!ff9.w_frameMenuon && ff9.w_naviMode != 2 && ff9.w_blockReady && !ff9.w_cameraInter)
        {
            ff9.w_movementBasicOperation();
        }
        Single num = ff9.w_moveCHRControl_XZSpeed;
        if (ff9.w_moveCHRControl_XZSpeed < 0f)
        {
            num = ff9.w_moveCHRControl_XZSpeed / 2f;
        }
        ff9.w_nwbCache = true;
        if (!ff9.w_moveCHRControlPtr.flg_fly)
        {
            Int32 num2 = ff9.w_moveCHRControl_No;
            for (Single num3 = 0f; num3 < ff9.PsxRot(1024); num3 += ff9.PsxRot(128))
            {
                Vector3 pos = ff9.w_moveActorPtr.pos;
                Int32 id = -1;
                Int32 num4 = -1;
                Single rotation = num3;
                Boolean flag = ff9.w_movementRoundCheck(ref pos, rotation, ff9.w_moveCHRControl_No, num, ref id, ref num4);
                if (!flag)
                {
                    rotation = -num3;
                    flag = ff9.w_movementRoundCheck(ref pos, rotation, ff9.w_moveCHRControl_No, num, ref id, ref num4);
                }
                if (flag)
                {
                    Single num5 = pos.x - ff9.w_moveActorPtr.lastx;
                    Single num6 = pos.y - ff9.w_moveActorPtr.lasty;
                    Single num7 = pos.z - ff9.w_moveActorPtr.lastz;
                    Boolean flag2 = false;
                    Single num8;
                    if (ff9.w_movementGetSliceHeight(status.slice_type, id, ref flag2) != 0f)
                    {
                        num8 = ff9.SquareRoot0(num5 * num5 + num7 * num7);
                    }
                    else
                    {
                        num8 = ff9.SquareRoot0(num5 * num5 + num6 * num6 + num7 * num7);
                    }
                    Single num9;
                    if (ff9.UnityUnit(num8) != 0)
                    {
                        num9 = num * num / num8;
                        if (num < 0f)
                        {
                            num9 = -num9;
                        }
                    }
                    else
                    {
                        num9 = 0f;
                    }
                    Vector3 pos2 = ff9.w_moveActorPtr.pos;
                    Boolean flag3 = ff9.w_movementRoundCheck(ref pos2, rotation, ff9.w_moveCHRControl_No, num9, ref id, ref num4);
                    if (flag3)
                    {
                        ff9.w_moveActorPtr.pos = pos2;
                        status.id = id;
                        status.ground_height = pos2.y;
                        ff9.w_moveCHRControlPoly = (Byte)num4;
                        break;
                    }
                }
            }
        }
        else
        {
            WMPhysics.UseInfiniteRaycast = true;
            WMPhysics.CastRayFromSky = true;
            WMPhysics.IgnoreExceptions = true;
            WMBlock absoluteBlock = ff9.world.GetAbsoluteBlock(ff9.w_moveActorPtr.transform);
            Vector3 pos3 = ff9.w_moveActorPtr.pos;
            Int32 id2 = -1;
            Int32 num10 = -1;
            Boolean flag3 = ff9.w_movementRoundCheck(ref pos3, 0f, ff9.w_moveCHRControl_No, num, ref id2, ref num10);
            if (flag3)
            {
                Vector3 pos4 = ff9.w_moveActorPtr.pos;
                pos4[0] = pos3.x;
                pos4[2] = pos3.z;
                ff9.w_moveYDiff = ff9.w_moveActorPtr.pos[1] - 42.1875f;
                Int32 index2;
                Int32 index = index2 = 1;
                Single num11 = pos4[index2];
                pos4[index] = num11 + ff9.w_moveCHRControl_YSpeed;
                ff9.w_moveActorPtr.pos = pos4;
                status.id = id2;
                status.ground_height = pos3.y;
                ff9.w_moveCHRControlPoly = (Byte)num10;
            }
            WMPhysics.IgnoreExceptions = false;
            WMPhysics.UseInfiniteRaycast = false;
            WMPhysics.CastRayFromSky = false;
        }
    }

    public static Single w_movementGetSliceHeight(Int32 type, Int32 id, ref Boolean imd)
    {
        Int32 num = ff9.m_GetIDTopograph(id);
        Int32 num2;
        switch (num)
        {
            case 36:
            case 37:
            case 38:
                num2 = 0; break;
            case 53:
                num2 = 1; break;
            case 54:
                num2 = 2; break;
            case 55:
                num2 = 3; break;
            case 56:
                num2 = 4; break;
            case 57:
                num2 = 5; break;
            case 51:
                num2 = 6; break;
            case 48:
                num2 = 7; break;
            case 49:
            case 50:
            case 52:
            default:
                num2 = 8; break;
        }
        imd = (num2 <= 0);
        Int32 num3 = ff9.w_movementSinkArray[type, num2];
        if (ff9.w_moveActorPtr != null && ff9.w_moveActorPtr.originalActor != null)
        {
            ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[ff9.w_moveActorPtr.originalActor.index];
            if (s_moveCHRStatus.control == 5)
            {
            }
        }
        if (num3 != 0)
        {
            num3 -= 100;
        }
        return ff9.S(-num3);
    }

    public static Boolean w_movementRoundCheck(ref Vector3 pos, Single rotation, Int32 type, Single speed, ref Int32 id, ref Int32 polyno)
    {
        int num = 0;
        ff9.s_moveCHRControl s_moveCHRControl = ff9.w_moveCHRControl[type];
        ff9.s_moveCHRCache cache = ff9.w_moveCHRCache[(int)ff9.w_moveCHRStatus[(int)ff9.w_moveActorPtr.originalActor.index].cache];
        Vector3 vector = pos;
        vector.x += ff9.rsin(ff9.w_moveCHRControl_RotTrue + ff9.PsxRot(2048) + rotation) * speed;
        vector.z += ff9.rcos(ff9.w_moveCHRControl_RotTrue + ff9.PsxRot(2048) + rotation) * speed;
        int num2 = ff9.m_GetIDTopograph(ff9.m_moveActorID);
        int num3;
        if ((num2 == 52 || num2 == 49) && ff9.w_moveCHRControlPtr.type != 1)
        {
            ff9.w_cellHit(ref vector, ref num, out num3, null, out vector.y);
        }
        else
        {
            ff9.w_cellHit(ref vector, ref num, out num3, cache, out vector.y);
        }
        if (num3 >= 0)
        {
            bool flag = ff9.w_movementCheckTopographID(s_moveCHRControl.limit, (uint)num);
            if (flag && s_moveCHRControl.flg_gake != 0)
            {
                bool flag2 = ff9.w_movementCheckTopographID(ff9.w_movementWaterStatus, (uint)num);
                bool flag3 = ff9.w_movementCheckTopographID(ff9.w_movementGroundStatus, (uint)num);
                bool flag4 = ff9.w_movementCheckTopographID(ff9.w_movementWaterStatus, ff9.m_moveActorID);
                bool flag5 = ff9.w_movementCheckTopographID(ff9.w_movementGroundStatus, ff9.m_moveActorID);
                if (flag3 && flag4)
                {
                    flag = false;
                }
                if (flag5 && flag2)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                pos = vector;
                polyno = num3;
                id = num;
                return true;
            }
        }
        return false;
    }

    private static Boolean checkGettingOffPosSpecially(Single ux, Single uz)
    {
        Vector3 a = new Vector3(ux, 0f, uz);
        a.y = 0f;
        Vector3 b = new Vector3(189.2f, 5.8f, -475.2f);
        Single num = Vector3.Distance(a, b);
        if (num < 10.12f)
        {
            return false;
        }
        Vector3 a2 = new Vector3(ux, 0f, uz);
        a2.y = 0f;
        Vector3 b2 = new Vector3(1088f, 15.8f, -952.4f);
        Single num2 = Vector3.Distance(a2, b2);
        return num2 >= 18.5f;
    }

    public static Boolean w_movementGetGetoff(ref Vector3 pos)
    {
        Single num = 1.953125f;
        Single[] array = new Single[2];
        Boolean flag = false;
        Boolean flag2 = false;
        Vector3 vector = Vector3.zero;
        if (ff9.w_worldLocDistance[1] < 22 && ff9.m_GetIDTopograph(ff9.m_moveActorID) == 23)
        {
            return false;
        }
        vector = ff9.w_moveActorPtr.pos;
        Int32 num2 = -1;
        Int32 num3 = -1;
        ff9.w_nwbTEST = true;
        WMBlock absoluteBlock = ff9.world.GetAbsoluteBlock(ff9.w_moveActorPtr.transform);
        switch (ff9.w_moveCHRControl_No)
        {
            case 7:
                if (!ff9.w_movementRoundCheck(ref vector, 0f, 10, 0f, ref num2, ref num3))
                {
                    return false;
                }
                if (ff9.m_GetIDTopograph(num2) != 53)
                {
                    return false;
                }
                break;
            case 8:
            case 9:
            {
                Int32 num4 = ff9.UnityUnit(ff9.w_moveActorPtr.RealPosition.x);
                Int32 num5 = ff9.UnityUnit(ff9.w_moveActorPtr.RealPosition.z);
                Int32 num6 = (num4 >> 3) - 6140;
                Int32 num7 = (num5 >> 3) - -15176;
                Int32 num8 = (Int32)Mathf.Sqrt(num6 * num6 + num7 * num7);
                if (num8 < 120)
                {
                    return false;
                }
                if (!ff9.checkGettingOffPosSpecially(ff9.w_moveActorPtr.RealPosition.x, ff9.w_moveActorPtr.RealPosition.z))
                {
                    return false;
                }
                if (!ff9.w_movementRoundCheck(ref vector, 0f, 0, 0f, ref num2, ref num3))
                {
                    return false;
                }
                Vector3 realPosition = ff9.w_moveActorPtr.RealPosition;
                realPosition.y = 0f;
                Vector3 b = new Vector3(896.4f, 2.8f, -350f);
                Single num9 = Vector3.Distance(realPosition, b);
                if (num9 < 10f)
                {
                    return false;
                }
                Int32 num10 = ff9.m_GetIDTopograph(num2);
                if (num10 == 45 || num10 == 46 || num10 == 52)
                {
                    return false;
                }
                break;
            }
            default:
            {
                Boolean flag3 = ff9.w_movementRoundCheck(ref vector, 0f, 0, 0f, ref num2, ref num3);
                Int32 num4 = ff9.UnityUnit(ff9.w_moveActorPtr.RealPosition.x);
                Int32 num5 = ff9.UnityUnit(ff9.w_moveActorPtr.RealPosition.z);
                Vector3 realPosition2 = ff9.w_moveActorPtr.RealPosition;
                realPosition2.y = 0f;
                Vector3 b2 = new Vector3(896.4f, 2.8f, -350f);
                Single num11 = Vector3.Distance(realPosition2, b2);
                if (num11 < 10f)
                {
                    return false;
                }
                if (!ff9.checkGettingOffPosSpecially(ff9.w_moveActorPtr.RealPosition.x, ff9.w_moveActorPtr.RealPosition.z))
                {
                    return false;
                }
                if (!flag3)
                {
                    return false;
                }
                Int32 num10 = ff9.m_GetIDTopograph(num2);
                if (num10 == 52)
                {
                    return false;
                }
                break;
            }
        }
        ff9.w_nwbTEST = false;
        for (Single num12 = 0f; num12 < ff9.PsxRot(2048); num12 += ff9.PsxRot(256))
        {
            array[0] = (num12 + ff9.PsxRot(3072)) % ff9.PsxRot(4096);
            array[1] = -num12 + ff9.PsxRot(3072);
            WMPhysics.CastRayFromSky = true;
            WMPhysics.IgnoreExceptions = true;
            WMPhysics.UseInfiniteRaycast = true;
            for (Int32 i = 0; i < 2; i++)
            {
                if (!flag)
                {
                    Boolean flag4 = true;
                    Int32 num13 = 8;
                    for (Int32 j = 1; j <= 8; j++)
                    {
                        if (ff9.w_moveCHRControl_No == 7)
                        {
                            num13 = 4;
                        }
                        if (ff9.w_moveCHRControl_No == 9 || ff9.w_moveCHRControl_No == 8)
                        {
                            num13 = 5;
                            if (i == 0 && (Int32)num12 == 0 && j == 8)
                            {
                                num13 = 8;
                            }
                        }
                        Int32 fixedPoint = ff9.w_moveCHRControlPtr.radius * j / num13;
                        vector = ff9.w_moveActorPtr.pos;
                        flag = ff9.w_movementRoundCheck(ref vector, array[i], 0, ff9.S(fixedPoint), ref num2, ref num3);
                        flag4 = (flag4 && flag);
                    }
                    flag = flag4;
                    if (flag)
                    {
                        SByte b3 = ff9.w_moveCHRControl_No;
                        switch (b3)
                        {
                            case 7:
                                if (ff9.w_movementCheckTopographID(ff9.w_movementGroundStatus, num2))
                                {
                                    flag = false;
                                }
                                break;
                            case 8:
                            case 9:
                            {
                                Single num14 = ff9.S(ff9.w_frameGetParameter(194));
                                if (ff9.abs(-vector.y - num14) > ff9.S(350))
                                {
                                    flag = false;
                                }
                                break;
                            }
                            default:
                                if (b3 == 0)
                                {
                                    if (ff9.abs(vector.y - ff9.w_moveActorPtr.pos1) > num)
                                    {
                                        flag = false;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            WMPhysics.CastRayFromSky = false;
            WMPhysics.IgnoreExceptions = false;
            WMPhysics.UseInfiniteRaycast = false;
            if (flag)
            {
                vector.y += ff9.w_movementGetSliceHeight(1, num2, ref flag2);
                pos = ff9.world.GetAbsolutePositionOf(vector);
                return true;
            }
        }
        return false;
    }

    public static Boolean w_movementCheckTopographID(UInt32[] check, Int32 id)
    {
        return ff9.w_movementCheckTopographID(check, (UInt32)id);
    }

    public static Boolean w_movementCheckTopographID(UInt32[] check, UInt32 id)
    {
        UInt32 num = (id & 0xFCu) >> 2;
        if (num > 31u)
        {
            if ((check[0] >> (Int32)(num - 32u) & 1u) != 0u)
            {
                return true;
            }
        }
        else if ((check[1] >> (Int32)num & 1u) != 0u)
        {
            return true;
        }
        return false;
    }

    public static void w_movementChange()
    {
        ff9.w_moveCHRControl_XZAlpha = 0f;
        ff9.w_moveCHRControl_YAlpha = 0f;
        ff9.w_movementAutoPilotOFF();
        SByte previousTransport = ff9.w_moveCHRControl_No;
        if (!FF9StateSystem.World.IsBeeScene)
            ff9.w_moveCHRControl_No = (SByte)FF9StateSystem.EventState.gEventGlobal[190];
        ff9.s_moveCHRControl s_moveCHRControl = ff9.w_moveCHRControlPtr;
        ff9.s_moveCHRControl s_moveCHRControl2 = ff9.w_moveCHRControl[ff9.w_moveCHRControl_No];
        ff9.w_moveCHRControlPtr = s_moveCHRControl2;
        if (previousTransport == 7) // Blue Narciss
        {
            ff9.w_musicSEVolumeIntpl(s_moveCHRControl.se, AllSoundDispatchPlayer.ConvertMillisecToTick(1000f), 0);
            ff9.w_musicSEStop(s_moveCHRControl.se, 1000);
        }
        if (s_moveCHRControl2.se != 0)
        {
            ff9.w_musicSEPlay(s_moveCHRControl2.se, 0);
            if (ff9.w_moveCHRControl_No != 7) // Blue Narciss
                ff9.w_musicSEVolumeIntpl(s_moveCHRControl2.se, 60, 63);
            if (ff9.w_moveCHRControl_No == 8 || ff9.w_moveCHRControl_No == 9) // Hilda Garde III or Invincible
                ff9.w_musicSEPlay(38, 0);
            ff9.w_cameraSysDataCamera.upperCounterForce = true;
        }
        if (s_moveCHRControl == null || s_moveCHRControl.music != s_moveCHRControl2.music)
        {
            ff9.w_musicRequestSend(s_moveCHRControl2.music);
            global::Debug.Log("w_musicRequestSend " + s_moveCHRControl2.music);
        }
        if (ff9.w_moveCHRControl_No == 0 && previousTransport >= 1 && previousTransport <= 6) // From chocobo to ground
            global::Debug.LogWarning("Come visit this later! w_moveCHRControl_RotTrue = (w_moveChocoboPtr.rot[1] + 1024) & 0xfff");
        if (ff9.w_moveCHRControl_No >= 1 && ff9.w_moveCHRControl_No <= 6 && previousTransport == 0 && ff9.w_moveChocoboPtr != null)
            ff9.w_moveCHRControl_RotTrue = ff9.w_moveChocoboPtr.rot[1]; // From ground to chocobo
        if (ff9.w_moveCHRControl[previousTransport].flg_fly == ff9.w_moveCHRControl[ff9.w_moveCHRControl_No].flg_fly)
            ff9.w_cameraChange(s_moveCHRControl2.type_cam, 512);
        else
            ff9.w_cameraChange(s_moveCHRControl2.type_cam, 64);
        ff9.w_cameraFuzzy = !ff9.w_moveCHRControlPtr.flg_fly;
        ff9.w_movementUpdate();
        ff9.w_tweakSomeValues();
    }

    public static void w_movementChangeSP1()
    {
        ff9.w_movementAutoPilotOFF();
        ff9.w_moveCHRControl_XZAlpha = 0f;
        ff9.w_moveCHRControl_YAlpha = 0f;
        ff9.s_moveCHRControl currentTransport = ff9.w_moveCHRControlPtr;
        ff9.s_moveCHRControl nextTransport = ff9.w_moveCHRControl[0];
        UInt16 tick = AllSoundDispatchPlayer.ConvertMillisecToTick(3000);
        ff9.w_musicSEVolumeIntpl(currentTransport.se, tick, 0);
        ff9.w_musicSEVolumeIntpl(38, tick, 0);
        ff9.w_musicRequestSend(nextTransport.music);
        ff9.w_cameraChange(nextTransport.type_cam, 64);
    }

    public static void w_tweakSomeValues()
    {
        Byte statusNo = WMUIData.StatusNo;
        Int32 controlNo = WMUIData.ControlNo;
        if (statusNo == 1 && controlNo == 0)
        {
            ff9.tweaker.w_cameraWorldEye_Y = 558;
            ff9.tweaker.w_cameraWorldAim_Y = 312;
        }
        if (statusNo == 2 && controlNo == 0)
        {
            ff9.tweaker.w_cameraWorldEye_Y = 558;
            ff9.tweaker.w_cameraWorldAim_Y = 312;
        }
        if (statusNo == 3 && controlNo == 1)
        {
            ff9.tweaker.w_cameraWorldEye_Y = 558;
            ff9.tweaker.w_cameraWorldAim_Y = 312;
        }
        if (statusNo == 4 && controlNo == 2)
        {
            ff9.tweaker.w_cameraWorldEye_Y = 558;
            ff9.tweaker.w_cameraWorldAim_Y = 312;
        }
        if (statusNo == 5 && controlNo == 3)
        {
            ff9.tweaker.w_cameraWorldEye_Y = 558;
            ff9.tweaker.w_cameraWorldAim_Y = 312;
        }
        if (statusNo == 6 && controlNo == 4)
        {
            ff9.tweaker.w_cameraWorldEye_Y = 558;
            ff9.tweaker.w_cameraWorldAim_Y = 312;
        }
        if (statusNo == 7 && controlNo == 5)
        {
            ff9.tweaker.w_cameraWorldEye_Y = 558;
            ff9.tweaker.w_cameraWorldAim_Y = 312;
        }
        if (statusNo == 7 && controlNo == 6)
        {
            ff9.tweaker.w_cameraWorldEye_Y = 1419;
            ff9.tweaker.w_cameraWorldAim_Y = 996;
        }
        if (statusNo == 8 && controlNo == 7)
        {
            ff9.tweaker.w_cameraWorldEye_Y = -1024;
            ff9.tweaker.w_cameraWorldAim_Y = 0;
        }
        if (statusNo == 9 && controlNo == 8)
        {
            ff9.tweaker.w_cameraWorldEye_Y = -1024;
            ff9.tweaker.w_cameraWorldAim_Y = 0;
        }
        if (statusNo == 10 && controlNo == 9)
        {
            ff9.tweaker.w_cameraWorldEye_Y = -1024;
            ff9.tweaker.w_cameraWorldAim_Y = 0;
        }
        if (FF9StateSystem.World.FixTypeCam)
        {
            if (ff9.w_moveCHRControlPtr.type_cam == 0)
            {
                ff9.tweaker.FixTypeCamEyeY = 558;
                ff9.tweaker.FixTypeCamAimY = 312;
                ff9.tweaker.FixTypeCamEyeYTarget = 558;
                ff9.tweaker.FixTypeCamAimYTarget = 312;
            }
            else if (ff9.w_moveCHRControlPtr.type_cam == 1)
            {
                ff9.tweaker.FixTypeCamEyeY = 558;
                ff9.tweaker.FixTypeCamAimY = 312;
                ff9.tweaker.FixTypeCamEyeYTarget = 558;
                ff9.tweaker.FixTypeCamAimYTarget = 312;
            }
            else if (ff9.w_moveCHRControlPtr.type_cam == 4)
            {
                ff9.tweaker.FixTypeCamEyeY = 1419;
                ff9.tweaker.FixTypeCamAimY = 996;
            }
            else if (ff9.w_moveCHRControlPtr.type_cam == 2)
            {
                ff9.tweaker.FixTypeCamEyeY = 320;
                ff9.tweaker.FixTypeCamAimY = 701;
                ff9.tweaker.FixTypeCamEyeYTarget = 320;
                ff9.tweaker.FixTypeCamAimYTarget = 701;
            }
            else if (ff9.w_moveCHRControlPtr.type_cam == 3)
            {
                ff9.tweaker.FixTypeCamEyeY = 1419;
                ff9.tweaker.FixTypeCamAimY = 996;
                ff9.tweaker.FixTypeCamEyeYTarget = 1419;
                ff9.tweaker.FixTypeCamAimYTarget = 996;
            }
        }
    }

    public static void w_movementChangeSP2()
    {
        PersistenSingleton<EventEngine>.Instance.ResetIdleTimer(0);
        ff9.w_musicSEStop(38);
        ff9.w_musicSEStop(ff9.w_moveCHRControlPtr.se);
        ff9.w_moveCHRControl_No = (SByte)ff9.byte_gEventGlobal(190);
        ff9.w_moveCHRControlPtr = ff9.w_moveCHRControl[ff9.w_moveCHRControl_No];
        if (ff9.GetControlChar() != null)
            ff9.w_moveActorPtr = ff9.GetControlChar();
        ff9.w_cameraFuzzy = !ff9.w_moveCHRControlPtr.flg_fly;
        ff9.w_movementUpdate();
    }

    private static void w_movementBasicOperation()
    {
        switch (ff9.w_moveCHRControlPtr.type)
        {
            case 0:
                ff9.w_movementHumanOperation();
                break;
            case 1:
                ff9.w_movementPlaneOperation();
                break;
            case 2:
                ff9.w_movementShipOperation();
                break;
        }
        if (ff9.w_moveCHRControlPtr.flg_upcam && ff9.w_getPadPush().kPadR2)
            ff9.w_cameraChangeTrigger();
        if (ff9.w_getPadPush().kPadL2)
            ff9.w_cameraChangeNotrotMode();
    }

    private static void w_movementHumanOperation()
    {
        ff9.w_moveGetPadStateLX(out Int32 leftStickX);
        ff9.w_moveGetPadStateLY(out Int32 leftStickY);
        leftStickY = -leftStickY;
        Single moveDirection = (-ff9.ratan2(leftStickY, leftStickX) + ff9.PsxRot(1024)) % 360f;
        Int32 moveSpeed = leftStickX != 0 || leftStickY != 0 ? 4096 : 0;
        ff9.w_movePadLR = false;
        ff9.w_movePadDOWN = ff9.Pad.kPadLDown;
        if (ff9.Pad.kPadL1)
        {
            ff9.w_movePadLR = true;
            ff9.w_moveCHRControl_LR = true;
            ff9.w_cameraSysDataCamera.rotation -= ff9.PsxRot(32);
        }
        if (ff9.Pad.kPadR1)
        {
            ff9.w_movePadLR = true;
            ff9.w_moveCHRControl_LR = true;
            ff9.w_cameraSysDataCamera.rotation += ff9.PsxRot(32);
        }
        if (Configuration.AnalogControl.RightStickCamera)
        {
            Single rightStickX = UnityXInput.XInputManager.Instance.CurrentState.ThumbSticks.Right.X;
            Single rightStickY = UnityXInput.XInputManager.Instance.CurrentState.ThumbSticks.Right.Y;
            ProcessCameraControl(rightStickX, rightStickY, 6f, 2f);
        }
        if (moveSpeed != 0 || ff9.w_movePadLR)
        {
            ff9.w_cameraSysDataCamera.rotationMax = ff9.rsin(moveDirection);
            if (moveSpeed != 0)
            {
                ff9.w_moveCHRControl_LR = false;
                if (moveDirection >= ff9.PsxRot(1536) && moveDirection <= ff9.PsxRot(2560))
                    ff9.w_cameraSysDataCamera.rotationRev = ff9.PsxRot(2048);
                else
                    ff9.w_cameraSysDataCamera.rotationRev = 0f;
                ff9.w_moveCHRControl_XZSpeed = ff9.S(ff9.w_moveCHRControlPtr.speed_move * moveSpeed >> 12);
                ff9.w_moveCHRControl_RotTrue = (ff9.w_cameraSysDataCamera.rotation + moveDirection) % 360f;
            }
        }
        Single aimRot = ff9.w_moveCHRControl_RotTrue;
        if (aimRot - ff9.w_moveActorPtr.rot1 > ff9.PsxRot(2048))
            aimRot -= ff9.PsxRot(4096);
        if (aimRot - ff9.w_moveActorPtr.rot1 < ff9.PsxRot(-2048))
            aimRot += ff9.PsxRot(4096);
        Single diffRot = aimRot - ff9.w_moveActorPtr.rot1;
        if (leftStickX != 0 || leftStickY != 0)
        {
            Single newRot = ff9.w_moveActorPtr.rot1;
            newRot += diffRot / 3f;
            newRot %= 360f;
            ff9.w_moveActorPtr.rot1 = newRot;
        }
        if (ff9.w_moveCHRControl_Move && moveSpeed != 0)
        {
            if (ff9.w_movementCheckTopographID(ff9.w_movementWaterStatus, ff9.m_moveActorID))
                ff9.w_effectRegist(ff9.w_moveActorPtr.pos[0], ff9.S(100), ff9.w_moveActorPtr.pos[2], SPSConst.WorldSPSEffect.MOVE_WATER, 24000);
            if (ff9.w_frameCounter % 4 == 0)
            {
                if (ff9.m_GetIDTopograph(ff9.m_moveActorID) == 30 || ff9.m_GetIDTopograph(ff9.m_moveActorID) == 34 || ff9.m_GetIDTopograph(ff9.m_moveActorID) == 35)
                    ff9.w_effectRegist(ff9.w_moveActorPtr.pos[0], ff9.w_moveActorPtr.pos[1], ff9.w_moveActorPtr.pos[2], SPSConst.WorldSPSEffect.MOVE_BEACH, 2304);
                if (ff9.m_GetIDTopograph(ff9.m_moveActorID) == 41)
                    ff9.w_effectRegist(ff9.w_moveActorPtr.pos[0], ff9.w_moveActorPtr.pos[1], ff9.w_moveActorPtr.pos[2], SPSConst.WorldSPSEffect.MOVE_DESERT, 8192);
            }
            if (ff9.w_frameCounter % 2 == 0 && ff9.m_GetIDTopograph(ff9.m_moveActorID) >= 36 && ff9.m_GetIDTopograph(ff9.m_moveActorID) <= 38)
            {
                ff9.w_effectRegist(ff9.w_moveActorPtr.lastx + ff9.S(ff9.random8() - 128), ff9.w_moveActorPtr.lasty, ff9.w_moveActorPtr.lastz + ff9.S(ff9.random8() - 128), SPSConst.WorldSPSEffect.MOVE_FOREST, 28032);
                ff9.w_effectRegist(ff9.w_moveActorPtr.lastx + ff9.S(ff9.random8() - 128), ff9.w_moveActorPtr.lasty, ff9.w_moveActorPtr.lastz + ff9.S(ff9.random8() - 128), SPSConst.WorldSPSEffect.MOVE_FOREST, 28032);
            }
        }
    }

    private static void w_movementShipOperation()
    {
        ff9.w_moveGetPadStateR(out Int32 moveFrontBackAlt);
        ff9.w_moveGetPadStateLX(out Int32 moveRotate);
        ff9.w_moveGetPadStateLY(out Int32 moveFrontBack);
        moveFrontBack = -moveFrontBack;
        if (moveFrontBackAlt > 0 && moveFrontBackAlt > moveFrontBack)
            moveFrontBack = moveFrontBackAlt;
        if (moveFrontBackAlt < 0 && moveFrontBackAlt < moveFrontBack)
            moveFrontBack = moveFrontBackAlt;
        if (FF9StateSystem.Settings.IsFastForward)
            ff9.w_moveCHRControl_RotSpeed = ff9.PsxRot((Int32)((Int32)ff9.w_moveCHRControlPtr.speed_rotation * moveRotate / 256f));
        else
            ff9.w_moveCHRControl_RotSpeed = ff9.PsxRot(ff9.w_moveCHRControlPtr.speed_rotation * moveRotate >> 7);
        ff9.w_moveCHRControl_LR = false;
        if (ff9.Pad.kPadL1)
        {
            ff9.w_cameraSysDataCamera.rotation -= ff9.PsxRot(32);
            ff9.w_moveCHRControl_LR = true;
        }
        if (ff9.Pad.kPadR1)
        {
            ff9.w_cameraSysDataCamera.rotation += ff9.PsxRot(32);
            ff9.w_moveCHRControl_LR = true;
        }
        if (Configuration.AnalogControl.RightStickCamera)
        {
            Single rightStickX = UnityXInput.XInputManager.Instance.CurrentState.ThumbSticks.Right.X;
            Single rightStickY = 0f;
            if (Configuration.Worldmap.AlternateControls)
                rightStickY = UnityXInput.XInputManager.Instance.CurrentState.ThumbSticks.Right.Y;
            ProcessCameraControl(rightStickX, rightStickY, 6f, 2f);
        }
        ff9.w_cameraSysDataCamera.rotationRev = 0f;
        Single moveAcc = ff9.S(ff9.w_moveCHRControlPtr.speed_move * moveFrontBack / ff9.p1);
        ff9.w_moveCHRControl_XZAlpha += (moveAcc - ff9.w_moveCHRControl_XZAlpha) / ff9.p2;
        ff9.w_moveCHRControl_XZSpeed = ff9.w_moveCHRControl_XZAlpha / ff9.p3;
        Int32 soundVolume = Math.Min(127, ff9.abs((Int32)(ff9.w_moveCHRControl_XZSpeed * 256f)) / 8 + 20);
        ff9.w_musicSEVolumeIntpl(ff9.w_moveCHRControlPtr.se, 60, (Byte)soundVolume);
        Vector3 actorRot = ff9.w_moveActorPtr.rot;
        actorRot[1] += ff9.w_moveCHRControl_RotSpeed;
        actorRot[1] %= 360f;
        ff9.w_moveCHRControl_RotTrue = actorRot[1];
        ff9.w_moveActorPtr.rot = actorRot;
        if (ff9.abs(ff9.w_moveCHRControl_XZSpeed) > ff9.S(80) && ff9.w_moveCHRControl_Move)
        {
            Int32 moveDirection = ff9.w_moveCHRControl_XZSpeed > 0f ? 0 : 2048;
            Int32 dx = (Int32)(ff9.rsin(moveDirection + (Int32)ff9.UnityRot(ff9.w_moveActorPtr.rot[1])) / 4f);
            Int32 dz = (Int32)(ff9.rcos(moveDirection + (Int32)ff9.UnityRot(ff9.w_moveActorPtr.rot[1])) / 4f);
            ff9.w_effectRegist(ff9.w_moveActorPtr.pos[0] + ff9.S(dx), ff9.S(100), ff9.w_moveActorPtr.pos[2] + ff9.S(dz), SPSConst.WorldSPSEffect.MOVE_SHIP, 15000);
            if (ff9.w_frameCounter % 2 != 0)
            {
                dx = (Int32)(ff9.rsin(moveDirection + (Int32)ff9.UnityRot(ff9.w_moveActorPtr.rot[1]) + 128) / 8f);
                dz = (Int32)(ff9.rcos(moveDirection + (Int32)ff9.UnityRot(ff9.w_moveActorPtr.rot[1]) + 128) / 8f);
                ff9.w_effectRegist(ff9.w_moveActorPtr.pos[0] + ff9.S(dx), ff9.S(100), ff9.w_moveActorPtr.pos[2] + ff9.S(dz), SPSConst.WorldSPSEffect.MOVE_SHIP_TRAIL, 15000);
                dx = (Int32)(ff9.rsin(moveDirection + (Int32)ff9.UnityRot(ff9.w_moveActorPtr.rot[1]) - 128) / 8f);
                dz = (Int32)(ff9.rcos(moveDirection + (Int32)ff9.UnityRot(ff9.w_moveActorPtr.rot[1]) - 128) / 8f);
                ff9.w_effectRegist(ff9.w_moveActorPtr.pos[0] + ff9.S(dx), ff9.S(100), ff9.w_moveActorPtr.pos[2] + ff9.S(dz), SPSConst.WorldSPSEffect.MOVE_SHIP_TRAIL, 15000);
            }
        }
    }

    private static void w_movementPlaneOperation()
    {
        ff9.w_moveGetPadStateLX(out Int32 leftStickX);
        ff9.w_moveGetPadStateLY(out Int32 leftStickY);
        ff9.w_moveGetPadStateR(out Int32 rightStick);
        if (Configuration.Worldmap.AlternateControls)
        {
            Single y = UnityXInput.XInputManager.Instance.CurrentState.ThumbSticks.Right.Y;
            if (Mathf.Abs(y) <= Configuration.AnalogControl.StickThreshold)
            {
                y = 0f;
            }
            leftStickY = (Int32)(-y * 128f);
        }
        ff9.w_movePadDOWN = rightStick < 0;
        leftStickY *= (Int32)Configuration.AnalogControl.InvertedFlightY;
        Int32 cameraChangeThreshold = leftStickX;
        ff9.w_moveCHRControl_LR = false;
        if (ff9.Pad.kPadR1 && ff9.Pad.kPadL1 && !ff9.w_cameraSysData.cameraNotrot && ff9.w_moveAutoPilot != 0)
            ff9.w_cameraChangeNotrotMode();
        if (!ff9.w_cameraSysData.cameraNotrot)
        {
            if (ff9.Pad.kPadL1)
                leftStickX = -127;
            if (ff9.Pad.kPadR1)
                leftStickX = 127;
        }
        else
        {
            if (ff9.Pad.kPadR1)
            {
                ff9.w_cameraSysDataCamera.rotation += ff9.PsxRot(16);
                ff9.w_moveCHRControl_LR = true;
            }
            if (ff9.Pad.kPadL1)
            {
                ff9.w_cameraSysDataCamera.rotation -= ff9.PsxRot(16);
                ff9.w_moveCHRControl_LR = true;
            }
        }
        if (Configuration.AnalogControl.RightStickCamera)
        {
            Single rightStickX = UnityXInput.XInputManager.Instance.CurrentState.ThumbSticks.Right.X;
            ProcessCameraControl(rightStickX, 0, 6f, 0f);
        }
        ff9.w_cameraSysDataCamera.rotationRev = 0f;
        ff9.w_moveCHRControl_RotSpeed = ff9.PsxRot(ff9.w_moveCHRControlPtr.speed_rotation * leftStickX >> 7);
        Single verticalMovementSpeed = ff9.S(-(Int32)ff9.w_moveCHRControlPtr.speed_updown * leftStickY / ff9.p1);
        ff9.w_moveCHRControl_YAlpha += (verticalMovementSpeed - ff9.w_moveCHRControl_YAlpha) / ff9.p2;
        ff9.w_moveCHRControl_YSpeed = ff9.w_moveCHRControl_YAlpha / ff9.p3;
        verticalMovementSpeed = ff9.S(ff9.w_moveCHRControlPtr.speed_move * rightStick / ff9.p1);
        ff9.w_moveCHRControl_XZAlpha += (verticalMovementSpeed - ff9.w_moveCHRControl_XZAlpha) / ff9.p2;
        ff9.w_moveCHRControl_XZSpeed = ff9.w_moveCHRControl_XZAlpha / ff9.p3;
        if (ff9.w_moveDoping)
        {
            ff9.w_moveCHRControl_RotSpeed *= 2f;
            ff9.w_moveCHRControl_XZSpeed *= 2f;
        }
        if (ff9.w_moveCHRControl_No == 9)
        {
            ff9.w_moveCHRControl_RotSpeed *= 600f - ff9.w_moveCHRControl_XZSpeed * 256f / 2f;
            ff9.w_moveCHRControl_RotSpeed /= 256f;
        }
        if (ff9.w_moveCHRControl_No == 8)
        {
            ff9.w_moveCHRControl_RotSpeed *= 580f - ff9.w_moveCHRControl_XZSpeed * 256f / 2f;
            ff9.w_moveCHRControl_RotSpeed /= 256f;
        }
        if (leftStickX != 0 || rightStick < 0)
            ff9.w_movementAutoPilotOFF();
        Byte autoPilotMode = ff9.w_moveAutoPilot;
        if (autoPilotMode != 1)
        {
            if (autoPilotMode == 2)
            {
                Single horizontalMovementSpeed = -(ff9.S(ff9.w_naviLocationPos[ff9.w_naviMapno, ff9.w_frameAutoid].tx) - ff9.w_moveActorPtr.RealPosition[0]);
                Single speedFactorUpDown = -(ff9.S(ff9.w_naviLocationPos[ff9.w_naviMapno, ff9.w_frameAutoid].ty) - ff9.w_moveActorPtr.RealPosition[2]);
                if (horizontalMovementSpeed > ff9.S(196608))
                    horizontalMovementSpeed -= ff9.S(393216);
                if (horizontalMovementSpeed < ff9.S(-196608))
                    horizontalMovementSpeed += ff9.S(393216);
                if (speedFactorUpDown > ff9.S(163840))
                    speedFactorUpDown -= ff9.S(327680);
                if (speedFactorUpDown < ff9.S(-163840))
                    speedFactorUpDown += ff9.S(327680);
                Single speedFactorLR = (-ff9.ratan2(speedFactorUpDown, horizontalMovementSpeed) + ff9.PsxRot(1024)) % 360f;
                Single speedFactorForwardBackward = speedFactorLR - ff9.w_moveActorPtr.rot[1];
                if (speedFactorForwardBackward < ff9.PsxRot(-2048))
                    speedFactorForwardBackward += ff9.PsxRot(4096);
                if (speedFactorForwardBackward > ff9.PsxRot(2048))
                    speedFactorForwardBackward -= ff9.PsxRot(4096);
                if (ff9.w_moveCHRControl_AutoPrev > ff9.S(50))
                {
                    if (speedFactorForwardBackward > ff9.PsxRot(20))
                        ff9.w_moveCHRControl_RotSpeed = ff9.PsxRot(20);
                    else if (speedFactorForwardBackward < ff9.PsxRot(-20))
                        ff9.w_moveCHRControl_RotSpeed = ff9.PsxRot(-20);
                    else if (ff9.w_moveAutoPilot == 2)
                        ff9.w_moveAutoPilot = 1;
                }
            }
        }
        else
        {
            Single distanceToTargetX = (ff9.S(ff9.w_naviLocationPos[ff9.w_naviMapno, ff9.w_frameAutoid].tx) - ff9.w_moveActorPtr.RealPosition[0]) / 32f;
            Single distanceToTargetY = (ff9.S(ff9.w_naviLocationPos[ff9.w_naviMapno, ff9.w_frameAutoid].ty) - ff9.w_moveActorPtr.RealPosition[2]) / 32f;
            ff9.w_moveCHRControl_XZSpeed = Math.Min(ff9.S(400), ff9.SquareRoot0(distanceToTargetX * distanceToTargetX + distanceToTargetY * distanceToTargetY));
            if (ff9.w_moveCHRControl_AutoPrev < ff9.w_moveCHRControl_XZSpeed || ff9.w_moveCHRControl_XZSpeed < ff9.S(50))
                ff9.w_movementAutoPilotOFF();
            ff9.w_moveCHRControl_AutoPrev = ff9.w_moveCHRControl_XZSpeed;
        }
        Vector3 actorRot = ff9.w_moveActorPtr.rot;
        if (actorRot[2] > 180f)
            actorRot[2] -= 360f;
        actorRot[1] += ff9.w_moveCHRControl_RotSpeed;
        actorRot[1] %= 360f;
        ff9.w_moveCHRControl_RotTrue = actorRot[1] % 360f;

        Single CameraTiltShip = Configuration.Worldmap.CameraTiltShip / 100f;
        if (ff9.w_moveCHRControl_RotSpeed > 0f && ff9.w_cameraRotAngle > ff9.PsxRot(-(cameraChangeThreshold * 1)))
            ff9.w_cameraRotAngle -= ff9.PsxRot(cameraChangeThreshold / 8) * CameraTiltShip;
        if (ff9.w_moveCHRControl_RotSpeed < 0f && ff9.w_cameraRotAngle < ff9.PsxRot(-(cameraChangeThreshold * 1)))
            ff9.w_cameraRotAngle -= ff9.PsxRot(cameraChangeThreshold / 8) * CameraTiltShip;
        if (ff9.UnityUnit(ff9.w_moveCHRControl_XZSpeed) == 0)
            ff9.w_cameraRotAngle -= ff9.PsxRot(cameraChangeThreshold / 8) * CameraTiltShip;
        ff9.w_cameraRotAngle = Mathf.Clamp(ff9.w_cameraRotAngle, ff9.PsxRot(-ff9.w_moveCHRControlPtr.speed_roll) * CameraTiltShip, ff9.PsxRot(ff9.w_moveCHRControlPtr.speed_roll) * CameraTiltShip);

        if (ff9.w_moveCHRControl_RotSpeed > 0f && actorRot[2] > ff9.PsxRot(-(cameraChangeThreshold * 2)))
            actorRot[2] += ff9.PsxRot(cameraChangeThreshold / 4);
        if (ff9.w_moveCHRControl_RotSpeed < 0f && actorRot[2] < ff9.PsxRot(-(cameraChangeThreshold * 2)))
            actorRot[2] += ff9.PsxRot(cameraChangeThreshold / 4);
        actorRot[2] = Mathf.Clamp(actorRot[2], ff9.PsxRot(-ff9.w_moveCHRControlPtr.speed_roll * 2), ff9.PsxRot(ff9.w_moveCHRControlPtr.speed_roll * 2));
        ff9.w_moveActorPtr.rot = actorRot;
        if ((ff9.w_frameCounter & 1) != 0)
        {
            Int32 topographId = ff9.m_GetIDTopograph(ff9.m_moveActorID);
            Boolean isAboveWater = ff9.w_movementCheckTopographID(ff9.w_movementWaterStatus, ff9.m_moveActorID);
            switch (ff9.w_moveActorPtr.originalActor.index)
            {
                case 7:
                {
                    if (isAboveWater && ff9.abs(ff9.w_moveCHRControl_XZSpeed) > ff9.S(30) && ff9.w_moveActorPtr.pos[1] <= ff9.WH2)
                        ff9.w_effectRegist(ff9.w_moveActorPtr.pos[0], ff9.S(200), ff9.w_moveActorPtr.pos[2], SPSConst.WorldSPSEffect.MOVE_CHOCOBO_ABOVE_WATER, 19200);
                    Int32 rotationSpeed = (Int32)(ff9.w_moveCHRControl_XZSpeed * 256f);
                    if (topographId == 41 && rotationSpeed != 0 && ff9.w_moveActorPtr.pos[1] <= ff9.SH2)
                        ff9.w_effectRegist(ff9.w_moveActorPtr.pos[0], ff9.w_moveActorPtr.pos[1] + ff9.S(-100), ff9.w_moveActorPtr.pos[2], SPSConst.WorldSPSEffect.MOVE_CHOCOBO_ABOVE_DESERT, 25600);
                    break;
                }
                case 9:
                case 10:
                {
                    if (isAboveWater && ff9.abs(ff9.w_moveCHRControl_XZSpeed) > ff9.S(60) && ff9.w_moveActorPtr.pos[1] <= ff9.WH1)
                    {
                        ff9.w_effectMoveStockHeight = 4096 - ff9.abs((Int32)(ff9.w_moveActorPtr.pos[1] * 256f) - 400) * 2;
                        ff9.w_effectRegist(ff9.w_moveActorPtr.pos[0], ff9.S(200), ff9.w_moveActorPtr.pos[2], SPSConst.WorldSPSEffect.MOVE_AIRSHIP_ABOVE_WATER, 32000);
                        Int32 effectStockHeight = Math.Min(511, ff9.abs((Int32)(ff9.w_moveCHRControl_XZSpeed * 256f)));
                        Int32 heightThreshold = Math.Min(48, (Int32)(ff9.abs(ff9.w_moveActorPtr.pos[1]) * 256f) >> 6);
                        heightThreshold = 48 - heightThreshold;
                        heightThreshold *= effectStockHeight;
                        heightThreshold >>= 9;
                        ff9.w_musicSEVolumeIntpl(38, 20, (Byte)heightThreshold);
                    }
                    else
                    {
                        ff9.w_musicSEVolumeIntpl(38, 30, 0);
                    }
                    Int32 speedThreshold = (Int32)(ff9.w_moveCHRControl_XZSpeed * 256f);
                    if (topographId == 41 && speedThreshold != 0 && ff9.w_moveActorPtr.pos[1] <= ff9.SH1)
                    {
                        ff9.w_effectMoveStockHeight = 4096 - ((Int32)(ff9.w_moveActorPtr.pos[1] * 256f) + 1300) * 2;
                        ff9.w_effectRegist(ff9.w_moveActorPtr.pos[0], ff9.w_moveActorPtr.pos[1] + ff9.S(-100), ff9.w_moveActorPtr.pos[2], SPSConst.WorldSPSEffect.MOVE_AIRSHIP_ABOVE_DESERT, 32000);
                    }
                    break;
                }
            }
        }
    }

    private static void w_movementHumanCamOperation()
    {
        Single cameraRotation = (ff9.w_cameraSysDataCamera.rotation + ff9.w_cameraSysDataCamera.rotationRev) % 360f;
        Single characterRotation = ff9.w_moveCHRControl_RotTrue % 360f;
        if (cameraRotation - characterRotation >= ff9.PsxRot(2048))
            cameraRotation -= ff9.PsxRot(4096);
        if (cameraRotation - characterRotation < ff9.PsxRot(-2048))
            cameraRotation += ff9.PsxRot(4096);
        Single rotationDifference = (ff9.w_moveCHRControl_RotTrue - cameraRotation) * ff9.PsxRot(3072) / ff9.PsxRot(4096);
        ff9.cn5 = ff9.abs(ff9.w_cameraSysDataCamera.rotationMax) / 8f;
        if (ff9.abs(rotationDifference) < ff9.cn5)
            rotationDifference = 0f;
        if (ff9.UnityUnit(ff9.w_moveCHRControl_XZSpeed) != 0)
            ff9.w_movementCamRemain = 4096f;
        else
            ff9.w_movementCamRemain = Math.Max(0f, ff9.w_movementCamRemain - 512f);
        if (!ff9.w_moveCHRControl_LR && !ff9.w_cameraSysData.cameraNotrot && ff9.w_framePhase == 2)
        {
            ff9.cn2 = Mathf.Clamp(ff9.abs(rotationDifference), ff9.cn3, ff9.cn4);
            Single rotationFactor = rotationDifference * 1f / ff9.cn2;
            Single rotationSpeedFactor = ((ff9.PsxRot(2048) - ff9.abs(rotationDifference)) * ff9.cn1 + 2048f) * 2f;
            rotationSpeedFactor = rotationSpeedFactor * ff9.w_movementCamRemain / 4096f;
            ff9.w_cameraSysDataCamera.rotation += rotationFactor * rotationSpeedFactor / 4096f;
        }
        ff9.w_cameraSysDataCamera.rotation %= 360f;
    }

    private static void w_movementPlaneCamOperation()
    {
        Single cameraRotation = ff9.w_cameraSysDataCamera.rotation;
        if (ff9.w_cameraSysDataCamera.rotation - ff9.w_moveActorPtr.rot[1] > ff9.PsxRot(2048))
            ff9.w_cameraSysDataCamera.rotation -= ff9.PsxRot(4096);
        if (ff9.w_cameraSysDataCamera.rotation - ff9.w_moveActorPtr.rot[1] < ff9.PsxRot(-2048))
            ff9.w_cameraSysDataCamera.rotation += ff9.PsxRot(4096);
        if (cameraRotation < 0f)
            cameraRotation += ff9.PsxRot(4096);
        cameraRotation %= ff9.PsxRot(4096);
        if (cameraRotation - ff9.w_moveActorPtr.rot[1] > ff9.PsxRot(2048))
            cameraRotation -= ff9.PsxRot(4096);
        if (cameraRotation - ff9.w_moveActorPtr.rot[1] < ff9.PsxRot(-2048))
            cameraRotation += ff9.PsxRot(4096);
        Single rotationDifference = ff9.w_moveActorPtr.rot[1] - cameraRotation;
        rotationDifference %= ff9.PsxRot(4096);
        if (!ff9.w_moveCHRControl_LR && !ff9.w_cameraSysData.cameraNotrot)
        {
            Int32 rotationDelta = ff9.UnityRot(rotationDifference);
            Int32 rotationFactor = 16;
            if (ff9.w_movementSoftRot)
            {
                rotationFactor = ff9.abs(rotationDelta) * 256 >> 12;
                if (rotationFactor == 0)
                    rotationFactor = 1;
            }
            Int32 rotationIncrement = rotationDelta % 4096 / rotationFactor;
            if (rotationIncrement != 0)
            {
                ff9.w_cameraSysDataCamera.rotation += ff9.PsxRot((UInt16)rotationIncrement);
            }
            else if (rotationFactor > 1)
            {
                do
                {
                    rotationFactor /= 2;
                    if (rotationFactor == 0)
                        break;
                    rotationIncrement = rotationDelta % 4096 / rotationFactor;
                }
                while (rotationIncrement == 0);
                ff9.w_cameraSysDataCamera.rotation += ff9.PsxRot((UInt16)rotationIncrement);
            }
            else
            {
                ff9.w_cameraSysDataCamera.rotation = ff9.w_moveActorPtr.rot[1];
            }
            ff9.w_cameraSysDataCamera.rotation %= 360f;
            if (ff9.abs(rotationDelta) < 100)
                ff9.w_movementSoftRot = false;
        }
    }

    public static void w_movementAutoPilotON()
    {
        ff9.w_moveAutoPilot = 2;
        ff9.w_moveCHRControl_AutoPrev = 10000f;
        ff9.w_moveAutoPilotPrevCammode = ff9.w_cameraSysData.cameraNotrot;
    }

    public static void w_movementAutoPilotOFF()
    {
        if (ff9.w_moveAutoPilot != 0)
        {
            ff9.w_moveAutoPilot = 0;
            ff9.w_moveCHRControl_AutoPrev = 0f;
            if (ff9.w_moveAutoPilotPrevCammode != ff9.w_cameraSysData.cameraNotrot)
                ff9.w_cameraChangeNotrotMode();
            ff9.w_cameraSysData.cameraNotrot = ff9.w_moveAutoPilotPrevCammode;
        }
    }

    public static void w_moveGetPadStateLX(out Int32 vx)
    {
        Single x = PersistenSingleton<HonoInputManager>.Instance.GetAxis().x;
        Boolean goLeft = x < -0.1f;
        Boolean goRight = x > 0.1f;
        vx = 0;
        if (Configuration.AnalogControl.Enabled)
        {
            if (goLeft || goRight)
                vx = (Int32)(x * 128.0f);
        }
        else
        {
            if (goLeft)
                vx = -128;
            if (goRight)
                vx = 128;
        }
        if (!EventInput.IsMovementControl)
            vx = 0;
    }

    public static void w_moveGetPadStateLY(out Int32 vy)
    {
        Boolean flyingVehicle = ff9.w_moveCHRControlPtr != null && ff9.w_moveCHRControlPtr.flg_fly;
        Single y = HonoInputManager.Instance.GetAxis().y;
        if ((Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android || ff9.forceUsingMobileInput) && flyingVehicle)
        {
            Boolean goDown = false;
            Boolean goUp = false;
            SourceControl sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetSource(Control.Down);
            if (sourceControl == SourceControl.Touch)
            {
                goDown = UIManager.Input.GetKey(Control.Down);
            }
            else
            {
                sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetDirectionAxisSource();
                if (sourceControl == SourceControl.Joystick || sourceControl == SourceControl.KeyBoard)
                {
                    goDown = y > 0.1f;
                    goUp = y < -0.1f;
                }
            }
            sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetSource(Control.Up);
            if (sourceControl == SourceControl.Touch)
            {
                goUp = UIManager.Input.GetKey(Control.Up);
            }
            else
            {
                sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetDirectionAxisSource();
                if (sourceControl == SourceControl.Joystick || sourceControl == SourceControl.KeyBoard)
                {
                    goDown = y > 0.1f;
                    goUp = y < -0.1f;
                }
            }
            vy = 0;
            if (goDown)
                vy = -128;
            if (goUp)
                vy = 128;
        }
        else
        {
            Boolean goDown = y > 0.1f;
            Boolean goUp = y < -0.1f;
            vy = 0;
            if (Configuration.AnalogControl.Enabled)
            {
                if (goDown || goUp)
                    vy = (Int32)(-y * 128.0f);
            }
            else
            {
                if (goDown)
                    vy = -128;
                if (goUp)
                    vy = 128;
            }
        }
        if (!EventInput.IsMovementControl)
            vy = 0;
    }

    public static void w_moveGetPadStateR(out Int32 vy)
    {
        Boolean flyingVehicle = ff9.w_moveCHRControlPtr != null && ff9.w_moveCHRControlPtr.flg_fly;
        Boolean goBackward = false;
        Boolean goForward = false;
        if ((Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android || ff9.forceUsingMobileInput) && flyingVehicle)
        {
            SourceControl sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetDirectionAxisSource();
            if (sourceControl == SourceControl.Touch)
            {
                Single y = PersistenSingleton<HonoInputManager>.Instance.GetAxis().y;
                goBackward = y < -0.1f;
                goForward = y > 0.1f;
            }
            else
            {
                sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetSource(Control.Special);
                if (sourceControl == SourceControl.Joystick || sourceControl == SourceControl.KeyBoard)
                    goBackward = UIManager.Input.GetKey(Control.Special);
                sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetSource(Control.Confirm);
                if (sourceControl == SourceControl.Joystick || sourceControl == SourceControl.KeyBoard)
                    goForward = UIManager.Input.GetKey(Control.Confirm);
            }
        }
        else
        {
            Single rightStickY = UnityXInput.XInputManager.Instance.CurrentState.ThumbSticks.Right.Y;
            if (Configuration.Worldmap.AlternateControls)
                rightStickY = UnityXInput.XInputManager.Instance.CurrentState.ThumbSticks.Left.Y;
            if (Mathf.Abs(rightStickY) > Configuration.AnalogControl.StickThreshold)
            {
                if (Configuration.AnalogControl.Enabled && !Configuration.Worldmap.AlternateControls)
                    vy = (Int32)(rightStickY * 128.0f);
                else if (rightStickY < 0f)
                    vy = -128;
                else
                    vy = 128;
                return;
            }
            goBackward = UIManager.Input.GetKey(Control.Special);
            goForward = UIManager.Input.GetKey(Control.Confirm);
        }
        vy = 0;
        if (goBackward)
            vy = -128;
        if (goForward)
            vy = 128;
    }

    public static void w_musicSystemConstructor()
    {
        ff9.w_musicProgress = 0;
        ff9.w_musicPlayNo = -1;
        ff9.w_musicPlayNoIdx = -1;
        ff9.w_musicRequestNoIdx = -1;
        ff9.w_musicFirstPlay = 0;
    }

    public static void w_musicMapConstructor()
    {
    }

    public static void w_musicUpdate()
    {
        SByte b = ff9.w_musicProgress;
        if (b != 1)
        {
            if (b == 2)
            {
                if (FF9Snd.ff9wldsnd_sync() == 0)
                {
                    ff9.w_frameCDUse = 0;
                    ff9.w_musicProgress = 0;
                }
            }
        }
        else if (ff9.w_frameCDUse == 0)
        {
            ff9.w_frameCDUse = 2;
            if (ff9.w_musicPlayNoIdx != -1)
            {
                ff9.w_musicStop((UInt16)ff9.w_musicPlayNoIdx);
            }
            ff9.w_musicPlay((UInt16)ff9.w_musicRequestNoIdx);
            ff9.w_musicPlayNoIdx = ff9.w_musicRequestNoIdx;
            ff9.w_musicProgress = 2;
        }
    }

    public static void w_musicPlay(UInt16 musicno)
    {
        byte b = ff9.byte_gEventGlobal(104);
        ff9.w_musicPlayNo = ff9.w_musicChoice(musicno);
        ff9.w_musicFirstPlay = 1;
        AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
        int suspendSongID = allSoundDispatchPlayer.GetSuspendSongID();
        int suspendSongID2 = allSoundDispatchPlayer.GetSuspendSongID();
        if (ff9.w_musicPlayNo == suspendSongID && suspendSongID != -1 && suspendSongID2 != -1 && suspendSongID == suspendSongID2)
        {
            FF9Snd.ff9wldsnd_song_restore();
        }
        else
        {
            SoundLib.GetAllSoundDispatchPlayer().StopAndClearSuspendBGM(-1);
            FF9Snd.ff9wldsnd_song_play(ff9.w_musicPlayNo);
        }
        FF9Snd.LatestWorldPlayedSong = ff9.w_musicPlayNo;
        int currentMusicId = FF9Snd.GetCurrentMusicId();
        int currentMusicId2 = FF9Snd.GetCurrentMusicId();
        if (b == 0)
        {
            b = 125;
        }
        if (currentMusicId != currentMusicId2)
        {
            FF9Snd.ff9wldsnd_song_vol_fadeall(60, 0, (int)b);
        }
    }

    public static void w_musicStop(UInt16 musicno)
    {
        Int32 songid = ff9.w_musicChoice(musicno);
        FF9Snd.ff9wldsnd_song_stop(songid);
    }

    public static Int32 w_musicChoice(UInt16 musicno)
    {
        if (musicno >= ff9.w_musicSet.Length)
            musicno = 0;
        if (musicno == 0)
        {
            UInt16 num = ff9.w_frameScenePtr;
            if (num >= 9600 && num < 9890)
            {
                musicno = 4;
            }
            if (num >= 9890 && num < 9910)
            {
                musicno = 5;
            }
            if (ff9.w_frameDisc == 4)
            {
                musicno = 3;
            }
        }
        return ff9.w_musicSet[(Int32)musicno];
    }

    /*
    public static Boolean w_musicContinue()
    {
        UInt16 num = ff9.ushort_gEventGlobal(0);
        Byte b = ff9.byte_gEventGlobal(190);
        return (num >= 9600 && num < 9890) || (num >= 9890 && num < 9910) || (b == 8 || b == 9);
    }
    */

    public static void w_musicSEPlay(Int32 seno, Byte vol)
    {
        if (FF9StateSystem.World.IsBeeScene)
        {
            return;
        }
        global::Debug.Log("w_musicSEPlay + " + seno);
        ff9.s_musicID s_musicID;
        if (ff9.w_musicGetID(seno, out s_musicID))
        {
            Int32 attributeNum = s_musicID.attr;
            for (Int32 i = 0; i < s_musicID.figure; i++)
            {
                FF9Snd.ff9wldsnd_sndeffect_play(s_musicID.id[i], attributeNum, vol, 127);
                attributeNum >>= 1;
            }
        }
    }

    public static void w_musicSEVolume(Int64 seno, Byte vol)
    {
        ff9.s_musicID s_musicID;
        if (ff9.w_musicProgress == 0 && ff9.w_musicGetID(seno, out s_musicID))
        {
            Int32 attributeNum = s_musicID.attr;
            for (Int32 i = 0; i < s_musicID.figure; i++)
            {
                FF9Snd.ff9wldsnd_sndeffect_vol(s_musicID.id[i], attributeNum, vol);
                attributeNum >>= 1;
            }
        }
    }

    public static void w_musicSEVolumeIntpl(Int32 seno, UInt16 tick, Byte vol)
    {
        ff9.s_musicID s_musicID;
        if (ff9.w_musicGetID(seno, out s_musicID))
        {
            Int32 attributeNum = s_musicID.attr;
            for (Int32 i = 0; i < s_musicID.figure; i++)
            {
                FF9Snd.ff9wldsnd_sndeffect_vol_intpl(s_musicID.id[i], attributeNum, tick, vol);
                attributeNum >>= 1;
            }
        }
    }

    public static void w_musicSEStop(Int32 seno)
    {
        ff9.s_musicID s_musicID;
        if (ff9.w_musicGetID(seno, out s_musicID))
        {
            Int32 attributeNum = s_musicID.attr;
            for (Int32 i = 0; i < s_musicID.figure; i++)
            {
                FF9Snd.ff9wldsnd_sndeffect_stop(s_musicID.id[i], attributeNum);
                attributeNum >>= 1;
            }
        }
    }

    public static void w_musicSEStop(int seno, int timeInMilliseconds)
    {
        ff9.s_musicID s_musicID;
        if (ff9.w_musicGetID((long)seno, out s_musicID))
        {
            for (int i = 0; i < s_musicID.figure; i++)
            {
                FF9Snd.ff9wldsnd_sndeffect_stop(s_musicID.id[i], timeInMilliseconds);
                timeInMilliseconds >>= 1;
            }
        }
    }

    public static Boolean w_musicGetID(Int64 seno, out ff9.s_musicID data)
    {
        ff9.s_musicID s_musicID = new ff9.s_musicID();
        s_musicID.id[0] = -1;
        if (seno >= 0L && seno <= 5L)
        {
            switch ((Int32)seno)
            {
                case 0:
                    s_musicID.id[0] = 104;
                    break;
                case 1:
                    s_musicID.id[0] = 102;
                    break;
                case 2:
                    s_musicID.id[0] = 108;
                    break;
                case 3:
                    s_musicID.id[0] = 637;
                    break;
                case 4:
                    s_musicID.id[0] = 638;
                    break;
                case 5:
                    s_musicID.id[0] = 682;
                    break;
            }
        }
        if (seno == 39L)
        {
            s_musicID.id[0] = 103;
        }
        if (s_musicID.id[0] != -1)
        {
            s_musicID.attr = 8388608;
            s_musicID.figure = 1;
            data = s_musicID;
            return true;
        }
        if (seno >= 30L && seno <= 42L)
        {
            switch ((Int32)(seno - 30L))
            {
                case 0:
                    s_musicID.id[0] = 1099;
                    break;
                case 1:
                    s_musicID.id[0] = 1231;
                    break;
                case 2:
                    s_musicID.id[0] = 1430;
                    break;
                case 4:
                    s_musicID.id[0] = 1100;
                    break;
                case 5:
                    s_musicID.id[0] = 892;
                    break;
                case 6:
                    s_musicID.id[0] = 1098;
                    break;
                case 7:
                    s_musicID.id[0] = 890;
                    break;
                case 8:
                    s_musicID.id[0] = 1499;
                    break;
                case 10:
                    s_musicID.id[0] = 891;
                    break;
                case 11:
                    s_musicID.id[0] = 2631;
                    break;
                case 12:
                    s_musicID.id[0] = 3110;
                    break;
            }
        }
        if (s_musicID.id[0] != -1)
        {
            s_musicID.attr = 0;
            s_musicID.figure = 1;
            data = s_musicID;
            return true;
        }
        if (seno != 6L)
        {
            if (seno == 7L)
            {
                s_musicID.id[0] = 1363;
            }
        }
        else
        {
            s_musicID.id[0] = 1362;
        }
        if (s_musicID.id[0] != -1)
        {
            s_musicID.attr = 0;
            s_musicID.figure = 1;
            data = s_musicID;
            return true;
        }
        if (seno >= 20L && seno <= 33L)
        {
            switch ((Int32)(seno - 20L))
            {
                case 0:
                    s_musicID.attr = 0;
                    s_musicID.id[0] = 884;
                    s_musicID.id[1] = 885;
                    break;
                case 1:
                    s_musicID.attr = 0;
                    s_musicID.id[0] = 886;
                    s_musicID.id[1] = 887;
                    break;
                case 2:
                    s_musicID.attr = 0;
                    s_musicID.id[0] = 888;
                    s_musicID.id[1] = 889;
                    break;
                case 3:
                    s_musicID.attr = 0;
                    s_musicID.id[0] = 1364;
                    s_musicID.id[1] = 1365;
                    break;
                case 4:
                    s_musicID.attr = 0;
                    s_musicID.id[0] = 1497;
                    s_musicID.id[1] = 1498;
                    break;
                case 5:
                    s_musicID.attr = 2048;
                    s_musicID.id[0] = 893;
                    s_musicID.id[1] = 894;
                    break;
                case 6:
                    s_musicID.attr = 2048;
                    s_musicID.id[0] = 895;
                    s_musicID.id[1] = 896;
                    break;
                case 13:
                    s_musicID.attr = 0;
                    s_musicID.id[0] = 890;
                    s_musicID.id[1] = 891;
                    break;
            }
        }
        if (s_musicID.id[0] != -1)
        {
            s_musicID.figure = 2;
            data = s_musicID;
            return true;
        }
        data = new ff9.s_musicID();
        return false;
    }

    public static void w_musicRequestSend(Int32 no)
    {
        if (ff9.w_musicProgress == 0 && ff9.w_musicRequestNoIdx != no)
        {
            ff9.w_musicProgress = 1;
            ff9.w_musicRequestNoIdx = no;
        }
    }
    /*
    public static void w_musicFade(Int32 inter)
    {
    }
    */

    /*
    public static void w_musicMapDestructor()
    {
        for (Int32 i = 0; i < 7; i++)
            ff9.w_musicSEStop(i);
        for (Int32 i = 20; i < 26; i++)
            ff9.w_musicSEStop(i);
        for (Int32 i = 30; i < 42; i++)
            ff9.w_musicSEStop(i);
        if (ff9.w_musicCheck() && ff9.w_frameResult != 3)
            ff9.FF9Global.attr |= 0x400000u;
    }

    public static Boolean w_musicCheck()
    {
        if (ff9.w_musicPlayNo == ff9.w_musicSet[0])
            return true;
        if (ff9.w_musicPlayNo == ff9.w_musicSet[1])
            return true;
        if (ff9.w_moveCHRControl_No == 0)
        {
            if (ff9.w_musicPlayNo == ff9.w_musicSet[3])
                return true;
            if (ff9.w_musicPlayNo == ff9.w_musicSet[4])
                return true;
        }
        return false;
    }
    */

    public static void w_naviSystemConstructor()
    {
        ff9.w_naviCursolMove = true;
        ff9.w_naviActive = true;
        ff9.w_naviMode = ff9.byte_gEventGlobal_keventNaviModeNo();
        if (ff9.w_naviMode >= 2)
        {
            // Fix for #697
            ff9.w_naviMode = 0;
            ff9.byte_gEventGlobal_updateNaviMode();
        }
        ff9.w_naviModeOld = -1;
        ff9.w_naviTitleColor = 0;
        ff9.w_naviFadeInTime = ff9.w_setNaviFadeInTime ? ff9.w_naviFadeInTime : 20u;
        ff9.w_naviPointLoc = 63;
        ff9.w_naviPointLocFlg = false;
        ff9.w_naviLocationDraw = ff9.WorldTitleCloseMode;
        ff9.w_naviCursolDraw = true;
        ff9.w_naviDrawItems = true;
        ff9.w_setNaviFadeInTime = false;
        if (ff9.w_frameScenePtr >= 5990)
        {
            UInt16 knownLocations = ff9.ushort_gEventGlobal(92);
            knownLocations |= 0x7C0; // Treno & South Gates
            ff9.ushort_gEventGlobal_Write(92, knownLocations);
            knownLocations = ff9.ushort_gEventGlobal(92);
            if ((knownLocations & 0xC000) != 0)
            {
                knownLocations |= 0xC000; // Dali and neighbourhood
                ff9.ushort_gEventGlobal_Write(92, knownLocations);
            }
        }
    }

    public static void w_naviGetPos(Single x, Single z, out Single sx, out Single sy, Int32 mode)
    {
        sx = 0f;
        sy = 0f;
        if (ff9.w_naviMapno == 0)
        {
            Single num = 645.12f;
            Single num2 = 428.800018f;
            sx = (x - num) / (1536f - num);
            sy = 1f - (-z - num2) / (1280f - num2);
        }
        else if (ff9.w_naviMapno == 1)
        {
            sx = x / 1536f;
            sy = 1f - -z / 1280f;
        }
    }

    public static Boolean w_naviLocationAvailable(Int32 no)
    {
        Int32 num = 0;
        UInt16 num2 = 0;
        if (no >= 0 && no < 16)
        {
            num2 = ff9.ushort_gEventGlobal_keventNaviLocF0();
            num = no;
        }
        if (no >= 16 && no < 32)
        {
            num2 = ff9.ushort_gEventGlobal_keventNaviLocF1();
            num = no - 16;
        }
        if (no >= 32 && no < 48)
        {
            num2 = ff9.ushort_gEventGlobal_keventNaviLocF2();
            num = no - 32;
        }
        if (no >= 48 && no < 64)
        {
            num2 = ff9.ushort_gEventGlobal_keventNaviLocF3();
            num = no - 48;
        }
        return (ff9.w_naviLocationPos[ff9.w_naviMapno, no].vx | ff9.w_naviLocationPos[ff9.w_naviMapno, no].vy) != 0 && (num2 >> num & 1) != 0;
    }

    public static void w_naviTitleElement()
    {
        UInt32[] durations = WorldConfiguration.GetTitleSpriteDurations(ff9.w_naviTitle);
        UInt32 fadeInDuration = Math.Max(2, durations[0]);
        UInt32 fadeOutDuration = Math.Max(1, durations[2]);
        UInt32 fadeoutTime = ff9.w_naviFadeInTime + fadeInDuration + durations[1];
        UInt32 closeTime = fadeoutTime + fadeOutDuration + 1u;
        ff9.w_naviLocationDraw = 0;
        if (ff9.w_frameCounterReady > ff9.w_naviFadeInTime)
            ff9.w_naviLocationDraw = ff9.WorldTitleFadeInMode;
        if (ff9.w_frameCounterReady > fadeoutTime)
            ff9.w_naviLocationDraw = ff9.WorldTitleFadeOutMode;
        if (ff9.w_frameCounterReady > closeTime)
            ff9.w_naviLocationDraw = ff9.WorldTitleCloseMode;
        if (ff9.w_frameCounterReady >= closeTime)
        {
            EventInput.PSXCntlClearPadMask(0, EventInput.Special | EventInput.NaviControl);
            PersistenSingleton<UIManager>.Instance.WorldHUDScene.EnableMapButton = true;
        }
        else
        {
            EventInput.PSXCntlSetPadMask(0, EventInput.Special | EventInput.NaviControl);
            PersistenSingleton<UIManager>.Instance.WorldHUDScene.EnableMapButton = false;
        }
        if (ff9.w_naviLocationDraw == ff9.WorldTitleFadeInMode)
        {
            if (ff9.w_naviTitleColor == 0)
            {
                ff9.w_frameScenePtr += 10;
                ff9.ushort_gEventGlobal_Write(0, ff9.w_frameScenePtr);
            }
            if (ff9.w_naviTitleColor != 128)
                ff9.w_naviTitleColor += 4;
            if (ff9.w_naviLocationDraw != ff9.lastTitleDrawState)
            {
                PersistenSingleton<UIManager>.Instance.WorldHUDScene.SetContinentTitleSprite(ff9.w_naviTitle);
                PersistenSingleton<UIManager>.Instance.WorldHUDScene.EnableContinentTitle(true);
                PersistenSingleton<UIManager>.Instance.WorldHUDScene.ShowContinentTitle((Int32)fadeInDuration);
                PersistenSingleton<UIManager>.Instance.WorldHUDScene.SetButtonVisible(false);
            }
        }
        else if (ff9.w_naviLocationDraw == ff9.WorldTitleFadeOutMode)
        {
            if (ff9.w_naviTitleColor != 0)
                ff9.w_naviTitleColor -= 4;
            if (ff9.w_naviLocationDraw != ff9.lastTitleDrawState)
                PersistenSingleton<UIManager>.Instance.WorldHUDScene.HideContinentTitle((Int32)fadeOutDuration);
        }
        else if (ff9.w_naviLocationDraw == ff9.WorldTitleCloseMode)
        {
            ff9.w_naviTitle = -1;
            if (ff9.w_naviLocationDraw != ff9.lastTitleDrawState)
            {
                ff9.w_setNaviFadeInTime = false;
                PersistenSingleton<UIManager>.Instance.WorldHUDScene.EnableContinentTitle(false);
                PersistenSingleton<UIManager>.Instance.WorldHUDScene.CurrentCharacterStateIndex = -1;
            }
        }
        ff9.lastTitleDrawState = ff9.w_naviLocationDraw;
    }

    public static void w_naviService()
    {
        if (ff9.w_naviTitle >= 0)
        {
            ff9.w_naviTitleElement();
        }
    }

    public static void w_frameSystemClean()
    {
        ff9.FF9Global.worldState.loadgameflg = 1;
    }

    public static void w_frameNewSession()
    {
        if (ff9.FF9Global.worldState.loadgameflg == 0)
        {
            ff9.FF9Global.worldState.loadgameflg = 2;
        }
    }

    /*
    public static void w_nwpSystemConstructor()
    {
        ff9.w_nwpMakePages();
        ff9.w_nwbCache = true;
        ff9.w_nwbPolyCheck = 0;
        ff9.w_nwbTEST = false;
    }
    */

    public static void w_nwpChangeStat()
    {
        ff9.w_nwbColor[0].w_nwbFarColor.r = (Byte)ff9.w_weatherColor.Color[16].fogDW.vx;
        ff9.w_nwbColor[0].w_nwbFarColor.g = (Byte)ff9.w_weatherColor.Color[16].fogDW.vy;
        ff9.w_nwbColor[0].w_nwbFarColor.b = (Byte)ff9.w_weatherColor.Color[16].fogDW.vz;
        ff9.w_nwbColor[0].w_nwbFogValG = ff9.w_weatherColor.Color[16].goffsetdw;
        ff9.w_nwbColor[0].w_nwbFogValT = ff9.w_weatherColor.Color[16].toffsetdw;
        ff9.w_nwbColor[1].w_nwbFarColor.r = (Byte)ff9.w_weatherColor.Color[16].fogUP.vx;
        ff9.w_nwbColor[1].w_nwbFarColor.g = (Byte)ff9.w_weatherColor.Color[16].fogUP.vy;
        ff9.w_nwbColor[1].w_nwbFarColor.b = (Byte)ff9.w_weatherColor.Color[16].fogUP.vz;
        ff9.w_nwbColor[1].w_nwbFogValG = ff9.w_weatherColor.Color[16].goffsetup;
        ff9.w_nwbColor[1].w_nwbFogValT = ff9.w_weatherColor.Color[16].toffsetup;
        ff9.w_nwbColorCloud.w_nwbFarColor.r = (Byte)ff9.w_weatherColor.Color[16].fogCL.vx;
        ff9.w_nwbColorCloud.w_nwbFarColor.g = (Byte)ff9.w_weatherColor.Color[16].fogCL.vy;
        ff9.w_nwbColorCloud.w_nwbFarColor.b = (Byte)ff9.w_weatherColor.Color[16].fogCL.vz;
        ff9.w_nwbColorCloud.w_nwbFogValG = ff9.w_weatherColor.Color[16].goffsetcl;
        ff9.w_nwbColorCloud.w_nwbFogValT = ff9.w_weatherColor.Color[16].toffsetcl;
        Color32 c = new Color32
        {
            r = ff9.w_nwbColor[0].w_nwbFarColor.r,
            g = ff9.w_nwbColor[0].w_nwbFarColor.g,
            b = ff9.w_nwbColor[0].w_nwbFarColor.b
        };
        RenderSettings.fogColor = c;
        ff9.w_frameCameraPtr.backgroundColor = c;
        RenderSettings.ambientLight = new Color32
        {
            r = ff9.w_nwbColor[1].w_nwbFarColor.r,
            g = ff9.w_nwbColor[1].w_nwbFarColor.g,
            b = ff9.w_nwbColor[1].w_nwbFarColor.b
        };
        Single offsetX = ff9.w_weatherColor.Color[16].offsetX;
        Single scaleY = ff9.w_weatherColor.Color[16].scaleY;
        Color skyFogColor = ff9.w_weatherColor.Color[16].skyFogColor;
        Material skyDowm_FogMaterial = ff9.world.SkyDowm_FogMaterial;
        skyDowm_FogMaterial.SetColor("_Color", skyFogColor);
        Color skyBgColor = ff9.w_weatherColor.Color[16].skyBgColor;
        Material skyDowm_BgMaterial = ff9.world.SkyDowm_BgMaterial;
        skyDowm_BgMaterial.SetColor("_Color", skyBgColor);
        ff9.angleTest += 0.1f;
        if (ff9.angleTest > 360f)
        {
            ff9.angleTest -= 360f;
        }
        Single rotation = ff9.w_cameraSysDataCamera.rotation;
        Single rotation2 = ff9.w_cameraSysDataCamera.rotation;
        Material skyDome_SkyMaterial = ff9.world.SkyDome_SkyMaterial;
        Vector2 mainTextureOffset = skyDome_SkyMaterial.mainTextureOffset;
        mainTextureOffset.x += ff9.rsin(rotation2) * ff9.tweaker.CloudSpeed;
        mainTextureOffset.y += -ff9.rcos(rotation2) * ff9.tweaker.CloudSpeed;
        mainTextureOffset.x %= 1f;
        mainTextureOffset.y %= 1f;
        skyDome_SkyMaterial.mainTextureOffset = mainTextureOffset;
    }

    /*
    public static void w_nwpInitialize(ff9.sNWBBlockHeader block)
    {
    }

    private static void w_nwpMakePages()
    {
    }
    */

    public static Single w_nwpHit(ref Vector3 pos, out Int32 id, out Int32 pno, ff9.s_moveCHRCache cache)
    {
        id = 0;
        pno = -1;
        Single result = 0f;
        Vector3 origin = pos;
        origin.y += ((!WMPhysics.CastRayFromSky) ? ff9.rayStartOffsetY : ff9.rayStartOffsetYFromSky);
        Single distance = (!WMPhysics.UseInfiniteRaycast) ? ff9.rayDistance : Single.PositiveInfinity;
        Vector3 down = Vector3.down;
        WMBlock absoluteBlock = ff9.world.GetAbsoluteBlock(pos);
        if (absoluteBlock == null)
        {
            return ff9.defaultHeight;
        }
        if (!absoluteBlock.IsReady)
        {
            return ff9.defaultHeight;
        }
        Ray ray = new Ray(origin, down);
        WMRaycastHit wmraycastHit;
        Int32 num;
        if (absoluteBlock.Raycast(ray, out wmraycastHit, distance, out num, cache))
        {
            result = wmraycastHit.point.y;
            id = num;
            pno = 1000;
        }
        return result;
    }

    public static Boolean w_nwpHitBool(ref Vector3 pos, out Int32 id, out Int32 pno, ff9.s_moveCHRCache cache, out Single height)
    {
        id = 0;
        pno = -1;
        height = 0f;
        Vector3 origin = pos;
        origin.y += ((!WMPhysics.CastRayFromSky) ? ff9.rayStartOffsetY : ff9.rayStartOffsetYFromSky);
        Single distance = (!WMPhysics.UseInfiniteRaycast) ? ff9.rayDistance : Single.PositiveInfinity;
        Vector3 down = Vector3.down;
        WMBlock absoluteBlock = ff9.world.GetAbsoluteBlock(pos);
        if (absoluteBlock == null)
        {
            return false;
        }
        if (!absoluteBlock.IsReady)
        {
            return false;
        }
        Ray ray = new Ray(origin, down);
        WMRaycastHit wmraycastHit;
        Int32 num;
        if (absoluteBlock.Raycast(ray, out wmraycastHit, distance, out num, cache))
        {
            id = num;
            height = wmraycastHit.point.y;
            pno = 1000;
            return true;
        }
        return false;
    }

    public static void SaveWorld(out ff9.FF9SAVE_WORLD data)
    {
        data = new ff9.FF9SAVE_WORLD();
        data.hintmap = (UInt32)ff9.FF9Global.hintmap_id;
        data.cameraState = ff9.FF9Global.worldState.statecamera;
    }

    public static void LoadWorld(ff9.FF9SAVE_WORLD data)
    {
        ff9.FF9Global.hintmap_id = (Int32)data.hintmap;
        ff9.FF9Global.worldState.statecamera = data.cameraState;
    }

    public static ff9.SVECTOR[] w_NormalTable
    {
        get
        {
            if (!ff9.initialized_w_NormalTable)
            {
                ff9.Initialize_w_NormalTable();
            }
            return ff9._w_NormalTable;
        }
    }

    private static void Initialize_w_NormalTable()
    {
        ff9._w_NormalTable = new ff9.SVECTOR[240];
        ff9._w_NormalTable[0].vx = 4012;
        ff9._w_NormalTable[0].vy = -69;
        ff9._w_NormalTable[0].vz = 818;
        ff9._w_NormalTable[0].pad = 0;
        ff9._w_NormalTable[1].vx = 4012;
        ff9._w_NormalTable[1].vy = -700;
        ff9._w_NormalTable[1].vz = 428;
        ff9._w_NormalTable[1].pad = 0;
        ff9._w_NormalTable[2].vx = 4012;
        ff9._w_NormalTable[2].vy = 700;
        ff9._w_NormalTable[2].vz = -428;
        ff9._w_NormalTable[2].pad = 0;
        ff9._w_NormalTable[3].vx = 4012;
        ff9._w_NormalTable[3].vy = 69;
        ff9._w_NormalTable[3].vz = -818;
        ff9._w_NormalTable[3].pad = 0;
        ff9._w_NormalTable[4].vx = 4011;
        ff9._w_NormalTable[4].vy = 705;
        ff9._w_NormalTable[4].vz = 435;
        ff9._w_NormalTable[4].pad = 0;
        ff9._w_NormalTable[5].vx = 4011;
        ff9._w_NormalTable[5].vy = -705;
        ff9._w_NormalTable[5].vz = -435;
        ff9._w_NormalTable[5].pad = 0;
        ff9._w_NormalTable[6].vx = 3853;
        ff9._w_NormalTable[6].vy = -1181;
        ff9._w_NormalTable[6].vz = -730;
        ff9._w_NormalTable[6].pad = 0;
        ff9._w_NormalTable[7].vx = 3853;
        ff9._w_NormalTable[7].vy = 1181;
        ff9._w_NormalTable[7].vz = 730;
        ff9._w_NormalTable[7].pad = 0;
        ff9._w_NormalTable[8].vx = 3727;
        ff9._w_NormalTable[8].vy = 69;
        ff9._w_NormalTable[8].vz = -1696;
        ff9._w_NormalTable[8].pad = 0;
        ff9._w_NormalTable[9].vx = 3727;
        ff9._w_NormalTable[9].vy = -1486;
        ff9._w_NormalTable[9].vz = 821;
        ff9._w_NormalTable[9].pad = 0;
        ff9._w_NormalTable[10].vx = 3727;
        ff9._w_NormalTable[10].vy = -69;
        ff9._w_NormalTable[10].vz = 1696;
        ff9._w_NormalTable[10].pad = 0;
        ff9._w_NormalTable[11].vx = 3727;
        ff9._w_NormalTable[11].vy = 1486;
        ff9._w_NormalTable[11].vz = -821;
        ff9._w_NormalTable[11].pad = 0;
        ff9._w_NormalTable[12].vx = 3546;
        ff9._w_NormalTable[12].vy = 1181;
        ff9._w_NormalTable[12].vz = 1673;
        ff9._w_NormalTable[12].pad = 0;
        ff9._w_NormalTable[13].vx = 3546;
        ff9._w_NormalTable[13].vy = -1181;
        ff9._w_NormalTable[13].vz = -1673;
        ff9._w_NormalTable[13].pad = 0;
        ff9._w_NormalTable[14].vx = 3546;
        ff9._w_NormalTable[14].vy = -2025;
        ff9._w_NormalTable[14].vz = -308;
        ff9._w_NormalTable[14].pad = 0;
        ff9._w_NormalTable[15].vx = 3546;
        ff9._w_NormalTable[15].vy = 2025;
        ff9._w_NormalTable[15].vz = 308;
        ff9._w_NormalTable[15].pad = 0;
        ff9._w_NormalTable[16].vx = 3501;
        ff9._w_NormalTable[16].vy = 705;
        ff9._w_NormalTable[16].vz = 2005;
        ff9._w_NormalTable[16].pad = 0;
        ff9._w_NormalTable[17].vx = 3501;
        ff9._w_NormalTable[17].vy = -705;
        ff9._w_NormalTable[17].vz = -2005;
        ff9._w_NormalTable[17].pad = 0;
        ff9._w_NormalTable[18].vx = 3501;
        ff9._w_NormalTable[18].vy = -2108;
        ff9._w_NormalTable[18].vz = 265;
        ff9._w_NormalTable[18].pad = 0;
        ff9._w_NormalTable[19].vx = 3501;
        ff9._w_NormalTable[19].vy = 2108;
        ff9._w_NormalTable[19].vz = -265;
        ff9._w_NormalTable[19].pad = 0;
        ff9._w_NormalTable[20].vx = 3498;
        ff9._w_NormalTable[20].vy = 1486;
        ff9._w_NormalTable[20].vz = -1526;
        ff9._w_NormalTable[20].pad = 0;
        ff9._w_NormalTable[21].vx = 3498;
        ff9._w_NormalTable[21].vy = 700;
        ff9._w_NormalTable[21].vz = -2011;
        ff9._w_NormalTable[21].pad = 0;
        ff9._w_NormalTable[22].vx = 3498;
        ff9._w_NormalTable[22].vy = -700;
        ff9._w_NormalTable[22].vz = 2011;
        ff9._w_NormalTable[22].pad = 0;
        ff9._w_NormalTable[23].vx = 3498;
        ff9._w_NormalTable[23].vy = -1486;
        ff9._w_NormalTable[23].vz = 1526;
        ff9._w_NormalTable[23].pad = 0;
        ff9._w_NormalTable[24].vx = 3050;
        ff9._w_NormalTable[24].vy = 2547;
        ff9._w_NormalTable[24].vz = 991;
        ff9._w_NormalTable[24].pad = 0;
        ff9._w_NormalTable[25].vx = 3050;
        ff9._w_NormalTable[25].vy = 2025;
        ff9._w_NormalTable[25].vz = 1835;
        ff9._w_NormalTable[25].pad = 0;
        ff9._w_NormalTable[26].vx = 3050;
        ff9._w_NormalTable[26].vy = -2547;
        ff9._w_NormalTable[26].vz = -991;
        ff9._w_NormalTable[26].pad = 0;
        ff9._w_NormalTable[27].vx = 3050;
        ff9._w_NormalTable[27].vy = -2025;
        ff9._w_NormalTable[27].vz = -1835;
        ff9._w_NormalTable[27].pad = 0;
        ff9._w_NormalTable[28].vx = 2994;
        ff9._w_NormalTable[28].vy = -2733;
        ff9._w_NormalTable[28].vz = 583;
        ff9._w_NormalTable[28].pad = 0;
        ff9._w_NormalTable[29].vx = 2994;
        ff9._w_NormalTable[29].vy = 700;
        ff9._w_NormalTable[29].vz = 2705;
        ff9._w_NormalTable[29].pad = 0;
        ff9._w_NormalTable[30].vx = 2994;
        ff9._w_NormalTable[30].vy = 2733;
        ff9._w_NormalTable[30].vz = -583;
        ff9._w_NormalTable[30].pad = 0;
        ff9._w_NormalTable[31].vx = 2994;
        ff9._w_NormalTable[31].vy = -700;
        ff9._w_NormalTable[31].vz = -2705;
        ff9._w_NormalTable[31].pad = 0;
        ff9._w_NormalTable[32].vx = 2988;
        ff9._w_NormalTable[32].vy = -705;
        ff9._w_NormalTable[32].vz = 2710;
        ff9._w_NormalTable[32].pad = 0;
        ff9._w_NormalTable[33].vx = 2988;
        ff9._w_NormalTable[33].vy = 705;
        ff9._w_NormalTable[33].vz = -2710;
        ff9._w_NormalTable[33].pad = 0;
        ff9._w_NormalTable[34].vx = 2988;
        ff9._w_NormalTable[34].vy = -2108;
        ff9._w_NormalTable[34].vz = 1843;
        ff9._w_NormalTable[34].pad = 0;
        ff9._w_NormalTable[35].vx = 2988;
        ff9._w_NormalTable[35].vy = 2108;
        ff9._w_NormalTable[35].vz = -1843;
        ff9._w_NormalTable[35].pad = 0;
        ff9._w_NormalTable[36].vx = 2765;
        ff9._w_NormalTable[36].vy = 69;
        ff9._w_NormalTable[36].vz = 3020;
        ff9._w_NormalTable[36].pad = 0;
        ff9._w_NormalTable[37].vx = 2765;
        ff9._w_NormalTable[37].vy = -69;
        ff9._w_NormalTable[37].vz = -3020;
        ff9._w_NormalTable[37].pad = 0;
        ff9._w_NormalTable[38].vx = 2765;
        ff9._w_NormalTable[38].vy = -2733;
        ff9._w_NormalTable[38].vz = 1288;
        ff9._w_NormalTable[38].pad = 0;
        ff9._w_NormalTable[39].vx = 2765;
        ff9._w_NormalTable[39].vy = 2733;
        ff9._w_NormalTable[39].vz = -1288;
        ff9._w_NormalTable[39].pad = 0;
        ff9._w_NormalTable[40].vx = 2687;
        ff9._w_NormalTable[40].vy = -2025;
        ff9._w_NormalTable[40].vz = 2334;
        ff9._w_NormalTable[40].pad = 0;
        ff9._w_NormalTable[41].vx = 2687;
        ff9._w_NormalTable[41].vy = -1181;
        ff9._w_NormalTable[41].vz = 2855;
        ff9._w_NormalTable[41].pad = 0;
        ff9._w_NormalTable[42].vx = 2687;
        ff9._w_NormalTable[42].vy = 1181;
        ff9._w_NormalTable[42].vz = -2855;
        ff9._w_NormalTable[42].pad = 0;
        ff9._w_NormalTable[43].vx = 2687;
        ff9._w_NormalTable[43].vy = 2025;
        ff9._w_NormalTable[43].vz = -2334;
        ff9._w_NormalTable[43].pad = 0;
        ff9._w_NormalTable[44].vx = 2676;
        ff9._w_NormalTable[44].vy = 2108;
        ff9._w_NormalTable[44].vz = 2273;
        ff9._w_NormalTable[44].pad = 0;
        ff9._w_NormalTable[45].vx = 2676;
        ff9._w_NormalTable[45].vy = 2976;
        ff9._w_NormalTable[45].vz = 869;
        ff9._w_NormalTable[45].pad = 0;
        ff9._w_NormalTable[46].vx = 2676;
        ff9._w_NormalTable[46].vy = -2976;
        ff9._w_NormalTable[46].vz = -869;
        ff9._w_NormalTable[46].pad = 0;
        ff9._w_NormalTable[47].vx = 2676;
        ff9._w_NormalTable[47].vy = -2108;
        ff9._w_NormalTable[47].vz = -2273;
        ff9._w_NormalTable[47].pad = 0;
        ff9._w_NormalTable[48].vx = 2532;
        ff9._w_NormalTable[48].vy = -3218;
        ff9._w_NormalTable[48].vz = -52;
        ff9._w_NormalTable[48].pad = 0;
        ff9._w_NormalTable[49].vx = 2532;
        ff9._w_NormalTable[49].vy = 3218;
        ff9._w_NormalTable[49].vz = 52;
        ff9._w_NormalTable[49].pad = 0;
        ff9._w_NormalTable[50].vx = 2532;
        ff9._w_NormalTable[50].vy = -1486;
        ff9._w_NormalTable[50].vz = -2855;
        ff9._w_NormalTable[50].pad = 0;
        ff9._w_NormalTable[51].vx = 2532;
        ff9._w_NormalTable[51].vy = 1486;
        ff9._w_NormalTable[51].vz = 2855;
        ff9._w_NormalTable[51].pad = 0;
        ff9._w_NormalTable[52].vx = 2079;
        ff9._w_NormalTable[52].vy = 2733;
        ff9._w_NormalTable[52].vz = 2231;
        ff9._w_NormalTable[52].pad = 0;
        ff9._w_NormalTable[53].vx = 2079;
        ff9._w_NormalTable[53].vy = 3218;
        ff9._w_NormalTable[53].vz = 1446;
        ff9._w_NormalTable[53].pad = 0;
        ff9._w_NormalTable[54].vx = 2079;
        ff9._w_NormalTable[54].vy = -3218;
        ff9._w_NormalTable[54].vz = -1446;
        ff9._w_NormalTable[54].pad = 0;
        ff9._w_NormalTable[55].vx = 2079;
        ff9._w_NormalTable[55].vy = -2733;
        ff9._w_NormalTable[55].vz = -2231;
        ff9._w_NormalTable[55].pad = 0;
        ff9._w_NormalTable[56].vx = 2018;
        ff9._w_NormalTable[56].vy = -3218;
        ff9._w_NormalTable[56].vz = 1531;
        ff9._w_NormalTable[56].pad = 0;
        ff9._w_NormalTable[57].vx = 2018;
        ff9._w_NormalTable[57].vy = 69;
        ff9._w_NormalTable[57].vz = 3563;
        ff9._w_NormalTable[57].pad = 0;
        ff9._w_NormalTable[58].vx = 2018;
        ff9._w_NormalTable[58].vy = -69;
        ff9._w_NormalTable[58].vz = -3563;
        ff9._w_NormalTable[58].pad = 0;
        ff9._w_NormalTable[59].vx = 2018;
        ff9._w_NormalTable[59].vy = 3218;
        ff9._w_NormalTable[59].vz = -1531;
        ff9._w_NormalTable[59].pad = 0;
        ff9._w_NormalTable[60].vx = 1932;
        ff9._w_NormalTable[60].vy = 1486;
        ff9._w_NormalTable[60].vz = 3291;
        ff9._w_NormalTable[60].pad = 0;
        ff9._w_NormalTable[61].vx = 1932;
        ff9._w_NormalTable[61].vy = -1486;
        ff9._w_NormalTable[61].vz = -3291;
        ff9._w_NormalTable[61].pad = 0;
        ff9._w_NormalTable[62].vx = 1932;
        ff9._w_NormalTable[62].vy = 3608;
        ff9._w_NormalTable[62].vz = -142;
        ff9._w_NormalTable[62].pad = 0;
        ff9._w_NormalTable[63].vx = 1932;
        ff9._w_NormalTable[63].vy = -3608;
        ff9._w_NormalTable[63].vz = 142;
        ff9._w_NormalTable[63].pad = 0;
        ff9._w_NormalTable[64].vx = 1885;
        ff9._w_NormalTable[64].vy = -1181;
        ff9._w_NormalTable[64].vz = 3438;
        ff9._w_NormalTable[64].pad = 0;
        ff9._w_NormalTable[65].vx = 1885;
        ff9._w_NormalTable[65].vy = 1181;
        ff9._w_NormalTable[65].vz = -3438;
        ff9._w_NormalTable[65].pad = 0;
        ff9._w_NormalTable[66].vx = 1885;
        ff9._w_NormalTable[66].vy = -2547;
        ff9._w_NormalTable[66].vz = 2594;
        ff9._w_NormalTable[66].pad = 0;
        ff9._w_NormalTable[67].vx = 1885;
        ff9._w_NormalTable[67].vy = 2547;
        ff9._w_NormalTable[67].vz = -2594;
        ff9._w_NormalTable[67].pad = 0;
        ff9._w_NormalTable[68].vx = 1654;
        ff9._w_NormalTable[68].vy = 705;
        ff9._w_NormalTable[68].vz = -3680;
        ff9._w_NormalTable[68].pad = 0;
        ff9._w_NormalTable[69].vx = 1654;
        ff9._w_NormalTable[69].vy = -2976;
        ff9._w_NormalTable[69].vz = 2276;
        ff9._w_NormalTable[69].pad = 0;
        ff9._w_NormalTable[70].vx = 1654;
        ff9._w_NormalTable[70].vy = 2976;
        ff9._w_NormalTable[70].vz = -2276;
        ff9._w_NormalTable[70].pad = 0;
        ff9._w_NormalTable[71].vx = 1654;
        ff9._w_NormalTable[71].vy = -705;
        ff9._w_NormalTable[71].vz = 3680;
        ff9._w_NormalTable[71].pad = 0;
        ff9._w_NormalTable[72].vx = 1647;
        ff9._w_NormalTable[72].vy = -700;
        ff9._w_NormalTable[72].vz = -3683;
        ff9._w_NormalTable[72].pad = 0;
        ff9._w_NormalTable[73].vx = 1647;
        ff9._w_NormalTable[73].vy = -3608;
        ff9._w_NormalTable[73].vz = 1020;
        ff9._w_NormalTable[73].pad = 0;
        ff9._w_NormalTable[74].vx = 1647;
        ff9._w_NormalTable[74].vy = 700;
        ff9._w_NormalTable[74].vz = 3683;
        ff9._w_NormalTable[74].pad = 0;
        ff9._w_NormalTable[75].vx = 1647;
        ff9._w_NormalTable[75].vy = 3608;
        ff9._w_NormalTable[75].vz = -1020;
        ff9._w_NormalTable[75].pad = 0;
        ff9._w_NormalTable[76].vx = 1480;
        ff9._w_NormalTable[76].vy = -2733;
        ff9._w_NormalTable[76].vz = -2667;
        ff9._w_NormalTable[76].pad = 0;
        ff9._w_NormalTable[77].vx = 1480;
        ff9._w_NormalTable[77].vy = 2733;
        ff9._w_NormalTable[77].vz = 2667;
        ff9._w_NormalTable[77].pad = 0;
        ff9._w_NormalTable[78].vx = 1480;
        ff9._w_NormalTable[78].vy = 3608;
        ff9._w_NormalTable[78].vz = 1251;
        ff9._w_NormalTable[78].pad = 0;
        ff9._w_NormalTable[79].vx = 1480;
        ff9._w_NormalTable[79].vy = -3608;
        ff9._w_NormalTable[79].vz = -1251;
        ff9._w_NormalTable[79].pad = 0;
        ff9._w_NormalTable[80].vx = 1389;
        ff9._w_NormalTable[80].vy = 2025;
        ff9._w_NormalTable[80].vz = -3277;
        ff9._w_NormalTable[80].pad = 0;
        ff9._w_NormalTable[81].vx = 1389;
        ff9._w_NormalTable[81].vy = -2025;
        ff9._w_NormalTable[81].vz = 3277;
        ff9._w_NormalTable[81].pad = 0;
        ff9._w_NormalTable[82].vx = 1334;
        ff9._w_NormalTable[82].vy = 2108;
        ff9._w_NormalTable[82].vz = 3247;
        ff9._w_NormalTable[82].pad = 0;
        ff9._w_NormalTable[83].vx = 1334;
        ff9._w_NormalTable[83].vy = -2108;
        ff9._w_NormalTable[83].vz = -3247;
        ff9._w_NormalTable[83].pad = 0;
        ff9._w_NormalTable[84].vx = 1334;
        ff9._w_NormalTable[84].vy = -3848;
        ff9._w_NormalTable[84].vz = -433;
        ff9._w_NormalTable[84].pad = 0;
        ff9._w_NormalTable[85].vx = 1334;
        ff9._w_NormalTable[85].vy = 3848;
        ff9._w_NormalTable[85].vz = 433;
        ff9._w_NormalTable[85].pad = 0;
        ff9._w_NormalTable[86].vx = 832;
        ff9._w_NormalTable[86].vy = 3218;
        ff9._w_NormalTable[86].vz = -2392;
        ff9._w_NormalTable[86].pad = 0;
        ff9._w_NormalTable[87].vx = 832;
        ff9._w_NormalTable[87].vy = -3218;
        ff9._w_NormalTable[87].vz = 2392;
        ff9._w_NormalTable[87].pad = 0;
        ff9._w_NormalTable[88].vx = 832;
        ff9._w_NormalTable[88].vy = 700;
        ff9._w_NormalTable[88].vz = -3948;
        ff9._w_NormalTable[88].pad = 0;
        ff9._w_NormalTable[89].vx = 832;
        ff9._w_NormalTable[89].vy = -700;
        ff9._w_NormalTable[89].vz = 3948;
        ff9._w_NormalTable[89].pad = 0;
        ff9._w_NormalTable[90].vx = 829;
        ff9._w_NormalTable[90].vy = 2108;
        ff9._w_NormalTable[90].vz = -3412;
        ff9._w_NormalTable[90].pad = 0;
        ff9._w_NormalTable[91].vx = 829;
        ff9._w_NormalTable[91].vy = -2108;
        ff9._w_NormalTable[91].vz = 3412;
        ff9._w_NormalTable[91].pad = 0;
        ff9._w_NormalTable[92].vx = 824;
        ff9._w_NormalTable[92].vy = 3848;
        ff9._w_NormalTable[92].vz = -1135;
        ff9._w_NormalTable[92].pad = 0;
        ff9._w_NormalTable[93].vx = 824;
        ff9._w_NormalTable[93].vy = -3848;
        ff9._w_NormalTable[93].vz = 1135;
        ff9._w_NormalTable[93].pad = 0;
        ff9._w_NormalTable[94].vx = 824;
        ff9._w_NormalTable[94].vy = 705;
        ff9._w_NormalTable[94].vz = 3949;
        ff9._w_NormalTable[94].pad = 0;
        ff9._w_NormalTable[95].vx = 824;
        ff9._w_NormalTable[95].vy = -705;
        ff9._w_NormalTable[95].vz = -3949;
        ff9._w_NormalTable[95].pad = 0;
        ff9._w_NormalTable[96].vx = 802;
        ff9._w_NormalTable[96].vy = -4008;
        ff9._w_NormalTable[96].vz = -260;
        ff9._w_NormalTable[96].pad = 0;
        ff9._w_NormalTable[97].vx = 802;
        ff9._w_NormalTable[97].vy = 4008;
        ff9._w_NormalTable[97].vz = 260;
        ff9._w_NormalTable[97].pad = 0;
        ff9._w_NormalTable[98].vx = 802;
        ff9._w_NormalTable[98].vy = 2025;
        ff9._w_NormalTable[98].vz = 3468;
        ff9._w_NormalTable[98].pad = 0;
        ff9._w_NormalTable[99].vx = 802;
        ff9._w_NormalTable[99].vy = -2025;
        ff9._w_NormalTable[99].vz = -3468;
        ff9._w_NormalTable[99].pad = 0;
        ff9._w_NormalTable[100].vx = 732;
        ff9._w_NormalTable[100].vy = -3608;
        ff9._w_NormalTable[100].vz = -1794;
        ff9._w_NormalTable[100].pad = 0;
        ff9._w_NormalTable[101].vx = 732;
        ff9._w_NormalTable[101].vy = -3218;
        ff9._w_NormalTable[101].vz = -2425;
        ff9._w_NormalTable[101].pad = 0;
        ff9._w_NormalTable[102].vx = 732;
        ff9._w_NormalTable[102].vy = 3608;
        ff9._w_NormalTable[102].vz = 1794;
        ff9._w_NormalTable[102].pad = 0;
        ff9._w_NormalTable[103].vx = 732;
        ff9._w_NormalTable[103].vy = 3218;
        ff9._w_NormalTable[103].vz = 2425;
        ff9._w_NormalTable[103].pad = 0;
        ff9._w_NormalTable[104].vx = 496;
        ff9._w_NormalTable[104].vy = -4008;
        ff9._w_NormalTable[104].vz = 682;
        ff9._w_NormalTable[104].pad = 0;
        ff9._w_NormalTable[105].vx = 496;
        ff9._w_NormalTable[105].vy = 4008;
        ff9._w_NormalTable[105].vz = -682;
        ff9._w_NormalTable[105].pad = 0;
        ff9._w_NormalTable[106].vx = 496;
        ff9._w_NormalTable[106].vy = 1181;
        ff9._w_NormalTable[106].vz = 3890;
        ff9._w_NormalTable[106].pad = 0;
        ff9._w_NormalTable[107].vx = 496;
        ff9._w_NormalTable[107].vy = -1181;
        ff9._w_NormalTable[107].vz = -3890;
        ff9._w_NormalTable[107].pad = 0;
        ff9._w_NormalTable[108].vx = 461;
        ff9._w_NormalTable[108].vy = 69;
        ff9._w_NormalTable[108].vz = -4069;
        ff9._w_NormalTable[108].pad = 0;
        ff9._w_NormalTable[109].vx = 461;
        ff9._w_NormalTable[109].vy = -69;
        ff9._w_NormalTable[109].vz = 4069;
        ff9._w_NormalTable[109].pad = 0;
        ff9._w_NormalTable[110].vx = 461;
        ff9._w_NormalTable[110].vy = 3608;
        ff9._w_NormalTable[110].vz = -1882;
        ff9._w_NormalTable[110].pad = 0;
        ff9._w_NormalTable[111].vx = 461;
        ff9._w_NormalTable[111].vy = -3608;
        ff9._w_NormalTable[111].vz = 1882;
        ff9._w_NormalTable[111].pad = 0;
        ff9._w_NormalTable[112].vx = 370;
        ff9._w_NormalTable[112].vy = 2733;
        ff9._w_NormalTable[112].vz = -3028;
        ff9._w_NormalTable[112].pad = 0;
        ff9._w_NormalTable[113].vx = 370;
        ff9._w_NormalTable[113].vy = 1486;
        ff9._w_NormalTable[113].vz = -3798;
        ff9._w_NormalTable[113].pad = 0;
        ff9._w_NormalTable[114].vx = 370;
        ff9._w_NormalTable[114].vy = -2733;
        ff9._w_NormalTable[114].vz = 3028;
        ff9._w_NormalTable[114].pad = 0;
        ff9._w_NormalTable[115].vx = 370;
        ff9._w_NormalTable[115].vy = -1486;
        ff9._w_NormalTable[115].vz = 3798;
        ff9._w_NormalTable[115].pad = 0;
        ff9._w_NormalTable[116].vx = 0;
        ff9._w_NormalTable[116].vy = -2976;
        ff9._w_NormalTable[116].vz = -2814;
        ff9._w_NormalTable[116].pad = 0;
        ff9._w_NormalTable[117].vx = 0;
        ff9._w_NormalTable[117].vy = -2547;
        ff9._w_NormalTable[117].vz = -3207;
        ff9._w_NormalTable[117].pad = 0;
        ff9._w_NormalTable[118].vx = 0;
        ff9._w_NormalTable[118].vy = 2547;
        ff9._w_NormalTable[118].vz = 3207;
        ff9._w_NormalTable[118].pad = 0;
        ff9._w_NormalTable[119].vx = 0;
        ff9._w_NormalTable[119].vy = 2976;
        ff9._w_NormalTable[119].vz = 2814;
        ff9._w_NormalTable[119].pad = 0;
        ff9._w_NormalTable[120].vx = 0;
        ff9._w_NormalTable[120].vy = 3848;
        ff9._w_NormalTable[120].vz = 1403;
        ff9._w_NormalTable[120].pad = 0;
        ff9._w_NormalTable[121].vx = 0;
        ff9._w_NormalTable[121].vy = 4008;
        ff9._w_NormalTable[121].vz = 843;
        ff9._w_NormalTable[121].pad = 0;
        ff9._w_NormalTable[122].vx = 0;
        ff9._w_NormalTable[122].vy = -4008;
        ff9._w_NormalTable[122].vz = -843;
        ff9._w_NormalTable[122].pad = 0;
        ff9._w_NormalTable[123].vx = 0;
        ff9._w_NormalTable[123].vy = -3848;
        ff9._w_NormalTable[123].vz = -1403;
        ff9._w_NormalTable[123].pad = 0;
        ff9._w_NormalTable[124].vx = -370;
        ff9._w_NormalTable[124].vy = -1486;
        ff9._w_NormalTable[124].vz = 3798;
        ff9._w_NormalTable[124].pad = 0;
        ff9._w_NormalTable[125].vx = -370;
        ff9._w_NormalTable[125].vy = 1486;
        ff9._w_NormalTable[125].vz = -3798;
        ff9._w_NormalTable[125].pad = 0;
        ff9._w_NormalTable[126].vx = -370;
        ff9._w_NormalTable[126].vy = -2733;
        ff9._w_NormalTable[126].vz = 3028;
        ff9._w_NormalTable[126].pad = 0;
        ff9._w_NormalTable[127].vx = -370;
        ff9._w_NormalTable[127].vy = 2733;
        ff9._w_NormalTable[127].vz = -3028;
        ff9._w_NormalTable[127].pad = 0;
        ff9._w_NormalTable[128].vx = -461;
        ff9._w_NormalTable[128].vy = 3608;
        ff9._w_NormalTable[128].vz = -1882;
        ff9._w_NormalTable[128].pad = 0;
        ff9._w_NormalTable[129].vx = -461;
        ff9._w_NormalTable[129].vy = -3608;
        ff9._w_NormalTable[129].vz = 1882;
        ff9._w_NormalTable[129].pad = 0;
        ff9._w_NormalTable[130].vx = -461;
        ff9._w_NormalTable[130].vy = -69;
        ff9._w_NormalTable[130].vz = 4069;
        ff9._w_NormalTable[130].pad = 0;
        ff9._w_NormalTable[131].vx = -461;
        ff9._w_NormalTable[131].vy = 69;
        ff9._w_NormalTable[131].vz = -4069;
        ff9._w_NormalTable[131].pad = 0;
        ff9._w_NormalTable[132].vx = -496;
        ff9._w_NormalTable[132].vy = 1181;
        ff9._w_NormalTable[132].vz = 3890;
        ff9._w_NormalTable[132].pad = 0;
        ff9._w_NormalTable[133].vx = -496;
        ff9._w_NormalTable[133].vy = 4008;
        ff9._w_NormalTable[133].vz = -682;
        ff9._w_NormalTable[133].pad = 0;
        ff9._w_NormalTable[134].vx = -496;
        ff9._w_NormalTable[134].vy = -4008;
        ff9._w_NormalTable[134].vz = 682;
        ff9._w_NormalTable[134].pad = 0;
        ff9._w_NormalTable[135].vx = -496;
        ff9._w_NormalTable[135].vy = -1181;
        ff9._w_NormalTable[135].vz = -3890;
        ff9._w_NormalTable[135].pad = 0;
        ff9._w_NormalTable[136].vx = -732;
        ff9._w_NormalTable[136].vy = 3218;
        ff9._w_NormalTable[136].vz = 2425;
        ff9._w_NormalTable[136].pad = 0;
        ff9._w_NormalTable[137].vx = -732;
        ff9._w_NormalTable[137].vy = 3608;
        ff9._w_NormalTable[137].vz = 1794;
        ff9._w_NormalTable[137].pad = 0;
        ff9._w_NormalTable[138].vx = -732;
        ff9._w_NormalTable[138].vy = -3608;
        ff9._w_NormalTable[138].vz = -1794;
        ff9._w_NormalTable[138].pad = 0;
        ff9._w_NormalTable[139].vx = -732;
        ff9._w_NormalTable[139].vy = -3218;
        ff9._w_NormalTable[139].vz = -2425;
        ff9._w_NormalTable[139].pad = 0;
        ff9._w_NormalTable[140].vx = -802;
        ff9._w_NormalTable[140].vy = -2025;
        ff9._w_NormalTable[140].vz = -3468;
        ff9._w_NormalTable[140].pad = 0;
        ff9._w_NormalTable[141].vx = -802;
        ff9._w_NormalTable[141].vy = 2025;
        ff9._w_NormalTable[141].vz = 3468;
        ff9._w_NormalTable[141].pad = 0;
        ff9._w_NormalTable[142].vx = -802;
        ff9._w_NormalTable[142].vy = 4008;
        ff9._w_NormalTable[142].vz = 260;
        ff9._w_NormalTable[142].pad = 0;
        ff9._w_NormalTable[143].vx = -802;
        ff9._w_NormalTable[143].vy = -4008;
        ff9._w_NormalTable[143].vz = -260;
        ff9._w_NormalTable[143].pad = 0;
        ff9._w_NormalTable[144].vx = -824;
        ff9._w_NormalTable[144].vy = 705;
        ff9._w_NormalTable[144].vz = 3949;
        ff9._w_NormalTable[144].pad = 0;
        ff9._w_NormalTable[145].vx = -824;
        ff9._w_NormalTable[145].vy = -705;
        ff9._w_NormalTable[145].vz = -3949;
        ff9._w_NormalTable[145].pad = 0;
        ff9._w_NormalTable[146].vx = -824;
        ff9._w_NormalTable[146].vy = 3848;
        ff9._w_NormalTable[146].vz = -1135;
        ff9._w_NormalTable[146].pad = 0;
        ff9._w_NormalTable[147].vx = -824;
        ff9._w_NormalTable[147].vy = -3848;
        ff9._w_NormalTable[147].vz = 1135;
        ff9._w_NormalTable[147].pad = 0;
        ff9._w_NormalTable[148].vx = -829;
        ff9._w_NormalTable[148].vy = -2108;
        ff9._w_NormalTable[148].vz = 3412;
        ff9._w_NormalTable[148].pad = 0;
        ff9._w_NormalTable[149].vx = -829;
        ff9._w_NormalTable[149].vy = 2108;
        ff9._w_NormalTable[149].vz = -3412;
        ff9._w_NormalTable[149].pad = 0;
        ff9._w_NormalTable[150].vx = -832;
        ff9._w_NormalTable[150].vy = -700;
        ff9._w_NormalTable[150].vz = 3948;
        ff9._w_NormalTable[150].pad = 0;
        ff9._w_NormalTable[151].vx = -832;
        ff9._w_NormalTable[151].vy = 700;
        ff9._w_NormalTable[151].vz = -3948;
        ff9._w_NormalTable[151].pad = 0;
        ff9._w_NormalTable[152].vx = -832;
        ff9._w_NormalTable[152].vy = -3218;
        ff9._w_NormalTable[152].vz = 2392;
        ff9._w_NormalTable[152].pad = 0;
        ff9._w_NormalTable[153].vx = -832;
        ff9._w_NormalTable[153].vy = 3218;
        ff9._w_NormalTable[153].vz = -2392;
        ff9._w_NormalTable[153].pad = 0;
        ff9._w_NormalTable[154].vx = -1334;
        ff9._w_NormalTable[154].vy = 3848;
        ff9._w_NormalTable[154].vz = 433;
        ff9._w_NormalTable[154].pad = 0;
        ff9._w_NormalTable[155].vx = -1334;
        ff9._w_NormalTable[155].vy = -3848;
        ff9._w_NormalTable[155].vz = -433;
        ff9._w_NormalTable[155].pad = 0;
        ff9._w_NormalTable[156].vx = -1334;
        ff9._w_NormalTable[156].vy = 2108;
        ff9._w_NormalTable[156].vz = 3247;
        ff9._w_NormalTable[156].pad = 0;
        ff9._w_NormalTable[157].vx = -1334;
        ff9._w_NormalTable[157].vy = -2108;
        ff9._w_NormalTable[157].vz = -3247;
        ff9._w_NormalTable[157].pad = 0;
        ff9._w_NormalTable[158].vx = -1389;
        ff9._w_NormalTable[158].vy = -2025;
        ff9._w_NormalTable[158].vz = 3277;
        ff9._w_NormalTable[158].pad = 0;
        ff9._w_NormalTable[159].vx = -1389;
        ff9._w_NormalTable[159].vy = 2025;
        ff9._w_NormalTable[159].vz = -3277;
        ff9._w_NormalTable[159].pad = 0;
        ff9._w_NormalTable[160].vx = -1479;
        ff9._w_NormalTable[160].vy = 3608;
        ff9._w_NormalTable[160].vz = 1251;
        ff9._w_NormalTable[160].pad = 0;
        ff9._w_NormalTable[161].vx = -1480;
        ff9._w_NormalTable[161].vy = -3608;
        ff9._w_NormalTable[161].vz = -1251;
        ff9._w_NormalTable[161].pad = 0;
        ff9._w_NormalTable[162].vx = -1480;
        ff9._w_NormalTable[162].vy = 2733;
        ff9._w_NormalTable[162].vz = 2667;
        ff9._w_NormalTable[162].pad = 0;
        ff9._w_NormalTable[163].vx = -1480;
        ff9._w_NormalTable[163].vy = -2733;
        ff9._w_NormalTable[163].vz = -2667;
        ff9._w_NormalTable[163].pad = 0;
        ff9._w_NormalTable[164].vx = -1647;
        ff9._w_NormalTable[164].vy = -3608;
        ff9._w_NormalTable[164].vz = 1020;
        ff9._w_NormalTable[164].pad = 0;
        ff9._w_NormalTable[165].vx = -1647;
        ff9._w_NormalTable[165].vy = 3608;
        ff9._w_NormalTable[165].vz = -1020;
        ff9._w_NormalTable[165].pad = 0;
        ff9._w_NormalTable[166].vx = -1647;
        ff9._w_NormalTable[166].vy = 700;
        ff9._w_NormalTable[166].vz = 3683;
        ff9._w_NormalTable[166].pad = 0;
        ff9._w_NormalTable[167].vx = -1647;
        ff9._w_NormalTable[167].vy = -700;
        ff9._w_NormalTable[167].vz = -3683;
        ff9._w_NormalTable[167].pad = 0;
        ff9._w_NormalTable[168].vx = -1654;
        ff9._w_NormalTable[168].vy = -2976;
        ff9._w_NormalTable[168].vz = 2276;
        ff9._w_NormalTable[168].pad = 0;
        ff9._w_NormalTable[169].vx = -1654;
        ff9._w_NormalTable[169].vy = 2976;
        ff9._w_NormalTable[169].vz = -2276;
        ff9._w_NormalTable[169].pad = 0;
        ff9._w_NormalTable[170].vx = -1654;
        ff9._w_NormalTable[170].vy = -705;
        ff9._w_NormalTable[170].vz = 3680;
        ff9._w_NormalTable[170].pad = 0;
        ff9._w_NormalTable[171].vx = -1654;
        ff9._w_NormalTable[171].vy = 705;
        ff9._w_NormalTable[171].vz = -3680;
        ff9._w_NormalTable[171].pad = 0;
        ff9._w_NormalTable[172].vx = -1885;
        ff9._w_NormalTable[172].vy = -2547;
        ff9._w_NormalTable[172].vz = 2594;
        ff9._w_NormalTable[172].pad = 0;
        ff9._w_NormalTable[173].vx = -1885;
        ff9._w_NormalTable[173].vy = 2547;
        ff9._w_NormalTable[173].vz = -2594;
        ff9._w_NormalTable[173].pad = 0;
        ff9._w_NormalTable[174].vx = -1885;
        ff9._w_NormalTable[174].vy = -1181;
        ff9._w_NormalTable[174].vz = 3438;
        ff9._w_NormalTable[174].pad = 0;
        ff9._w_NormalTable[175].vx = -1885;
        ff9._w_NormalTable[175].vy = 1181;
        ff9._w_NormalTable[175].vz = -3438;
        ff9._w_NormalTable[175].pad = 0;
        ff9._w_NormalTable[176].vx = -1932;
        ff9._w_NormalTable[176].vy = 3608;
        ff9._w_NormalTable[176].vz = -142;
        ff9._w_NormalTable[176].pad = 0;
        ff9._w_NormalTable[177].vx = -1932;
        ff9._w_NormalTable[177].vy = -3608;
        ff9._w_NormalTable[177].vz = 142;
        ff9._w_NormalTable[177].pad = 0;
        ff9._w_NormalTable[178].vx = -1932;
        ff9._w_NormalTable[178].vy = 1486;
        ff9._w_NormalTable[178].vz = 3291;
        ff9._w_NormalTable[178].pad = 0;
        ff9._w_NormalTable[179].vx = -1932;
        ff9._w_NormalTable[179].vy = -1486;
        ff9._w_NormalTable[179].vz = -3291;
        ff9._w_NormalTable[179].pad = 0;
        ff9._w_NormalTable[180].vx = -2018;
        ff9._w_NormalTable[180].vy = -3218;
        ff9._w_NormalTable[180].vz = 1531;
        ff9._w_NormalTable[180].pad = 0;
        ff9._w_NormalTable[181].vx = -2018;
        ff9._w_NormalTable[181].vy = 3218;
        ff9._w_NormalTable[181].vz = -1531;
        ff9._w_NormalTable[181].pad = 0;
        ff9._w_NormalTable[182].vx = -2018;
        ff9._w_NormalTable[182].vy = 69;
        ff9._w_NormalTable[182].vz = 3563;
        ff9._w_NormalTable[182].pad = 0;
        ff9._w_NormalTable[183].vx = -2018;
        ff9._w_NormalTable[183].vy = -69;
        ff9._w_NormalTable[183].vz = -3563;
        ff9._w_NormalTable[183].pad = 0;
        ff9._w_NormalTable[184].vx = -2079;
        ff9._w_NormalTable[184].vy = 2733;
        ff9._w_NormalTable[184].vz = 2231;
        ff9._w_NormalTable[184].pad = 0;
        ff9._w_NormalTable[185].vx = -2079;
        ff9._w_NormalTable[185].vy = -2733;
        ff9._w_NormalTable[185].vz = -2231;
        ff9._w_NormalTable[185].pad = 0;
        ff9._w_NormalTable[186].vx = -2079;
        ff9._w_NormalTable[186].vy = -3218;
        ff9._w_NormalTable[186].vz = -1446;
        ff9._w_NormalTable[186].pad = 0;
        ff9._w_NormalTable[187].vx = -2079;
        ff9._w_NormalTable[187].vy = 3218;
        ff9._w_NormalTable[187].vz = 1446;
        ff9._w_NormalTable[187].pad = 0;
        ff9._w_NormalTable[188].vx = -2532;
        ff9._w_NormalTable[188].vy = 1486;
        ff9._w_NormalTable[188].vz = 2855;
        ff9._w_NormalTable[188].pad = 0;
        ff9._w_NormalTable[189].vx = -2532;
        ff9._w_NormalTable[189].vy = -3218;
        ff9._w_NormalTable[189].vz = -52;
        ff9._w_NormalTable[189].pad = 0;
        ff9._w_NormalTable[190].vx = -2532;
        ff9._w_NormalTable[190].vy = -1486;
        ff9._w_NormalTable[190].vz = -2855;
        ff9._w_NormalTable[190].pad = 0;
        ff9._w_NormalTable[191].vx = -2532;
        ff9._w_NormalTable[191].vy = 3218;
        ff9._w_NormalTable[191].vz = 52;
        ff9._w_NormalTable[191].pad = 0;
        ff9._w_NormalTable[192].vx = -2676;
        ff9._w_NormalTable[192].vy = 2108;
        ff9._w_NormalTable[192].vz = 2273;
        ff9._w_NormalTable[192].pad = 0;
        ff9._w_NormalTable[193].vx = -2676;
        ff9._w_NormalTable[193].vy = -2108;
        ff9._w_NormalTable[193].vz = -2273;
        ff9._w_NormalTable[193].pad = 0;
        ff9._w_NormalTable[194].vx = -2676;
        ff9._w_NormalTable[194].vy = -2976;
        ff9._w_NormalTable[194].vz = -869;
        ff9._w_NormalTable[194].pad = 0;
        ff9._w_NormalTable[195].vx = -2676;
        ff9._w_NormalTable[195].vy = 2976;
        ff9._w_NormalTable[195].vz = 869;
        ff9._w_NormalTable[195].pad = 0;
        ff9._w_NormalTable[196].vx = -2687;
        ff9._w_NormalTable[196].vy = -1181;
        ff9._w_NormalTable[196].vz = 2855;
        ff9._w_NormalTable[196].pad = 0;
        ff9._w_NormalTable[197].vx = -2687;
        ff9._w_NormalTable[197].vy = 2025;
        ff9._w_NormalTable[197].vz = -2334;
        ff9._w_NormalTable[197].pad = 0;
        ff9._w_NormalTable[198].vx = -2687;
        ff9._w_NormalTable[198].vy = -2025;
        ff9._w_NormalTable[198].vz = 2334;
        ff9._w_NormalTable[198].pad = 0;
        ff9._w_NormalTable[199].vx = -2687;
        ff9._w_NormalTable[199].vy = 1181;
        ff9._w_NormalTable[199].vz = -2855;
        ff9._w_NormalTable[199].pad = 0;
        ff9._w_NormalTable[200].vx = -2765;
        ff9._w_NormalTable[200].vy = 69;
        ff9._w_NormalTable[200].vz = 3020;
        ff9._w_NormalTable[200].pad = 0;
        ff9._w_NormalTable[201].vx = -2765;
        ff9._w_NormalTable[201].vy = -2733;
        ff9._w_NormalTable[201].vz = 1288;
        ff9._w_NormalTable[201].pad = 0;
        ff9._w_NormalTable[202].vx = -2765;
        ff9._w_NormalTable[202].vy = 2733;
        ff9._w_NormalTable[202].vz = -1288;
        ff9._w_NormalTable[202].pad = 0;
        ff9._w_NormalTable[203].vx = -2765;
        ff9._w_NormalTable[203].vy = -69;
        ff9._w_NormalTable[203].vz = -3020;
        ff9._w_NormalTable[203].pad = 0;
        ff9._w_NormalTable[204].vx = -2988;
        ff9._w_NormalTable[204].vy = -705;
        ff9._w_NormalTable[204].vz = 2710;
        ff9._w_NormalTable[204].pad = 0;
        ff9._w_NormalTable[205].vx = -2988;
        ff9._w_NormalTable[205].vy = -2108;
        ff9._w_NormalTable[205].vz = 1843;
        ff9._w_NormalTable[205].pad = 0;
        ff9._w_NormalTable[206].vx = -2988;
        ff9._w_NormalTable[206].vy = 2108;
        ff9._w_NormalTable[206].vz = -1843;
        ff9._w_NormalTable[206].pad = 0;
        ff9._w_NormalTable[207].vx = -2988;
        ff9._w_NormalTable[207].vy = 705;
        ff9._w_NormalTable[207].vz = -2710;
        ff9._w_NormalTable[207].pad = 0;
        ff9._w_NormalTable[208].vx = -2994;
        ff9._w_NormalTable[208].vy = -2733;
        ff9._w_NormalTable[208].vz = 583;
        ff9._w_NormalTable[208].pad = 0;
        ff9._w_NormalTable[209].vx = -2994;
        ff9._w_NormalTable[209].vy = 2733;
        ff9._w_NormalTable[209].vz = -583;
        ff9._w_NormalTable[209].pad = 0;
        ff9._w_NormalTable[210].vx = -2994;
        ff9._w_NormalTable[210].vy = -700;
        ff9._w_NormalTable[210].vz = -2705;
        ff9._w_NormalTable[210].pad = 0;
        ff9._w_NormalTable[211].vx = -2994;
        ff9._w_NormalTable[211].vy = 700;
        ff9._w_NormalTable[211].vz = 2705;
        ff9._w_NormalTable[211].pad = 0;
        ff9._w_NormalTable[212].vx = -3050;
        ff9._w_NormalTable[212].vy = -2025;
        ff9._w_NormalTable[212].vz = -1835;
        ff9._w_NormalTable[212].pad = 0;
        ff9._w_NormalTable[213].vx = -3050;
        ff9._w_NormalTable[213].vy = 2025;
        ff9._w_NormalTable[213].vz = 1835;
        ff9._w_NormalTable[213].pad = 0;
        ff9._w_NormalTable[214].vx = -3050;
        ff9._w_NormalTable[214].vy = 2547;
        ff9._w_NormalTable[214].vz = 991;
        ff9._w_NormalTable[214].pad = 0;
        ff9._w_NormalTable[215].vx = -3050;
        ff9._w_NormalTable[215].vy = -2547;
        ff9._w_NormalTable[215].vz = -991;
        ff9._w_NormalTable[215].pad = 0;
        ff9._w_NormalTable[216].vx = -3498;
        ff9._w_NormalTable[216].vy = 700;
        ff9._w_NormalTable[216].vz = -2011;
        ff9._w_NormalTable[216].pad = 0;
        ff9._w_NormalTable[217].vx = -3498;
        ff9._w_NormalTable[217].vy = 1486;
        ff9._w_NormalTable[217].vz = -1526;
        ff9._w_NormalTable[217].pad = 0;
        ff9._w_NormalTable[218].vx = -3498;
        ff9._w_NormalTable[218].vy = -700;
        ff9._w_NormalTable[218].vz = 2011;
        ff9._w_NormalTable[218].pad = 0;
        ff9._w_NormalTable[219].vx = -3498;
        ff9._w_NormalTable[219].vy = -1486;
        ff9._w_NormalTable[219].vz = 1526;
        ff9._w_NormalTable[219].pad = 0;
        ff9._w_NormalTable[220].vx = -3501;
        ff9._w_NormalTable[220].vy = 705;
        ff9._w_NormalTable[220].vz = 2005;
        ff9._w_NormalTable[220].pad = 0;
        ff9._w_NormalTable[221].vx = -3501;
        ff9._w_NormalTable[221].vy = -705;
        ff9._w_NormalTable[221].vz = -2005;
        ff9._w_NormalTable[221].pad = 0;
        ff9._w_NormalTable[222].vx = -3501;
        ff9._w_NormalTable[222].vy = -2108;
        ff9._w_NormalTable[222].vz = 265;
        ff9._w_NormalTable[222].pad = 0;
        ff9._w_NormalTable[223].vx = -3501;
        ff9._w_NormalTable[223].vy = 2108;
        ff9._w_NormalTable[223].vz = -265;
        ff9._w_NormalTable[223].pad = 0;
        ff9._w_NormalTable[224].vx = -3546;
        ff9._w_NormalTable[224].vy = 2025;
        ff9._w_NormalTable[224].vz = 308;
        ff9._w_NormalTable[224].pad = 0;
        ff9._w_NormalTable[225].vx = -3546;
        ff9._w_NormalTable[225].vy = -2025;
        ff9._w_NormalTable[225].vz = -308;
        ff9._w_NormalTable[225].pad = 0;
        ff9._w_NormalTable[226].vx = -3546;
        ff9._w_NormalTable[226].vy = 1181;
        ff9._w_NormalTable[226].vz = 1673;
        ff9._w_NormalTable[226].pad = 0;
        ff9._w_NormalTable[227].vx = -3546;
        ff9._w_NormalTable[227].vy = -1181;
        ff9._w_NormalTable[227].vz = -1673;
        ff9._w_NormalTable[227].pad = 0;
        ff9._w_NormalTable[228].vx = -3727;
        ff9._w_NormalTable[228].vy = -1486;
        ff9._w_NormalTable[228].vz = 821;
        ff9._w_NormalTable[228].pad = 0;
        ff9._w_NormalTable[229].vx = -3727;
        ff9._w_NormalTable[229].vy = 1486;
        ff9._w_NormalTable[229].vz = -821;
        ff9._w_NormalTable[229].pad = 0;
        ff9._w_NormalTable[230].vx = -3727;
        ff9._w_NormalTable[230].vy = 69;
        ff9._w_NormalTable[230].vz = -1696;
        ff9._w_NormalTable[230].pad = 0;
        ff9._w_NormalTable[231].vx = -3727;
        ff9._w_NormalTable[231].vy = -69;
        ff9._w_NormalTable[231].vz = 1696;
        ff9._w_NormalTable[231].pad = 0;
        ff9._w_NormalTable[232].vx = -3853;
        ff9._w_NormalTable[232].vy = 1181;
        ff9._w_NormalTable[232].vz = 730;
        ff9._w_NormalTable[232].pad = 0;
        ff9._w_NormalTable[233].vx = -3853;
        ff9._w_NormalTable[233].vy = -1181;
        ff9._w_NormalTable[233].vz = -730;
        ff9._w_NormalTable[233].pad = 0;
        ff9._w_NormalTable[234].vx = -4011;
        ff9._w_NormalTable[234].vy = 705;
        ff9._w_NormalTable[234].vz = 435;
        ff9._w_NormalTable[234].pad = 0;
        ff9._w_NormalTable[235].vx = -4011;
        ff9._w_NormalTable[235].vy = -705;
        ff9._w_NormalTable[235].vz = -435;
        ff9._w_NormalTable[235].pad = 0;
        ff9._w_NormalTable[236].vx = -4012;
        ff9._w_NormalTable[236].vy = -69;
        ff9._w_NormalTable[236].vz = 818;
        ff9._w_NormalTable[236].pad = 0;
        ff9._w_NormalTable[237].vx = -4012;
        ff9._w_NormalTable[237].vy = -700;
        ff9._w_NormalTable[237].vz = 428;
        ff9._w_NormalTable[237].pad = 0;
        ff9._w_NormalTable[238].vx = -4012;
        ff9._w_NormalTable[238].vy = 700;
        ff9._w_NormalTable[238].vz = -428;
        ff9._w_NormalTable[238].pad = 0;
        ff9._w_NormalTable[239].vx = -4012;
        ff9._w_NormalTable[239].vy = 69;
        ff9._w_NormalTable[239].vz = -818;
        ff9._w_NormalTable[239].pad = 0;
        ff9.initialized_w_NormalTable = true;
    }

    /*
    public static Vector3 GetNormalFrom_w_NormalTable(Int32 index)
    {
        if (index < 0)
        {
            index = 0;
            global::Debug.LogWarning("index < 0");
        }
        ff9.SVECTOR svector = ff9.w_NormalTable[index];
        return new Vector3
        {
            x = svector.vx / 4096f,
            y = svector.vy / 4096f,
            z = svector.vz / 4096f
        };
    }

    public static Vector3[] BuildNormalsFrome_w_NormalTable()
    {
        Vector3[] array = new Vector3[ff9.w_NormalTable.Length];
        for (Int32 i = 0; i < array.Length; i++)
        {
            array[i] = ff9.GetNormalFrom_w_NormalTable(i);
        }
        return array;
    }
    */

    public static void w_textureUpdate()
    {
        ff9.w_texturePixelAnime();
        ff9.w_texturePaletScroll();
    }

    public static void w_texturePixelAnime()
    {
        Int32 speed = ff9.w_textureProjWork.texturePixelAnime[0].speed;
        if (speed != 0 && ff9.w_frameCounter % speed == 0)
        {
            ff9.renderTextureBank.UpdateBeach1();
            SByte[] array = ff9.w_texturePixelAnimCnt;
            Int32 num = 0;
            array[num] = (SByte)(array[num] + 1);
            SByte[] array2 = ff9.w_texturePixelAnimCnt;
            Int32 num2 = 0;
            array2[num2] = (SByte)(array2[num2] % 6);
        }
        speed = ff9.w_textureProjWork.texturePixelAnime[1].speed;
        if (speed != 0 && (ff9.w_frameCounter + 2) % speed == 0)
        {
            ff9.renderTextureBank.UpdateBeach2();
            SByte[] array3 = ff9.w_texturePixelAnimCnt;
            Int32 num3 = 1;
            array3[num3] = (SByte)(array3[num3] + 1);
            SByte[] array4 = ff9.w_texturePixelAnimCnt;
            Int32 num4 = 1;
            array4[num4] = (SByte)(array4[num4] % 6);
        }
    }

    public static void w_texturePaletScroll()
    {
        if (ff9.w_moveActorPtr != null)
        {
            Single num = Vector3.Distance(ff9.w_moveActorPtr.RealPosition, ff9.volcanoPosition);
            if (num < 120f)
            {
                ff9.renderTextureBank.UpdateVolcanoCrater1();
                ff9.renderTextureBank.UpdateVolcanoLava1();
                ff9.renderTextureBank.UpdateVolcanoCrater2();
                ff9.renderTextureBank.UpdateVolcanoLava2();
            }
        }
        ff9.renderTextureBank.Sea_IndexOffset += 0.2f;
        ff9.renderTextureBank.Sea_IndexOffset %= ff9.renderTextureBank.Sea_IndexOffsetMax;
        ff9.renderTextureBank.River_IndexOffset += 0.2f;
        ff9.renderTextureBank.River_IndexOffset %= ff9.renderTextureBank.River_IndexOffsetMax;
        ff9.renderTextureBank.Sea6_IndexOffset += 0.25f;
        ff9.renderTextureBank.Sea6_IndexOffset %= ff9.renderTextureBank.Sea6_IndexOffsetMax;
        Material quicksandMaterial = ff9.world.QuicksandMaterial;
        if (quicksandMaterial == null)
        {
            ff9.world.LoadQuicksandMaterial();
            quicksandMaterial = ff9.world.QuicksandMaterial;
        }
        Vector2 mainTextureOffset = quicksandMaterial.mainTextureOffset;
        mainTextureOffset.y += ff9.tweaker.QuicksandScrollSpeed;
        quicksandMaterial.mainTextureOffset = mainTextureOffset;
        Material waterShrineMaterial = ff9.world.WaterShrineMaterial;
        if (waterShrineMaterial == null)
        {
            ff9.world.LoadWaterShrineMaterial();
            waterShrineMaterial = ff9.world.WaterShrineMaterial;
        }
        mainTextureOffset = waterShrineMaterial.mainTextureOffset;
        mainTextureOffset.y += ff9.tweaker.WaterShrineScrollSpeed;
        waterShrineMaterial.mainTextureOffset = mainTextureOffset;
    }

    public static void w_weatherSystemConstructor()
    {
        ff9.w_weatherAutoChange = true;
        ff9.w_weatherMode = -2;
        ff9.w_weatherModeOld = -1;
        ff9.w_weatherColorCounter = 0;
        ff9.w_weatherColorCounterSpeed = 128;
    }

    public static void w_weatherMapConstructor()
    {
        ff9.w_weatherModeOld = ff9.w_weatherModeCheck();
        ff9.w_frameFog = (Byte)((!ff9.w_weatherFogCheck()) ? 0 : 1);
        ff9.w_weatherDest(ff9.w_weatherModeOld, ff9.w_frameFog, 4096);
    }

    public static void w_weatherUpdate()
    {
        if (ff9.w_weatherAutoChange)
        {
            ff9.w_weatherDeside();
        }
        if (ff9.w_weatherColorCounter >= 0)
        {
            ff9.w_weatherColorCounter += ff9.w_weatherColorCounterSpeed;
            if (ff9.w_weatherColorCounter > 4096)
            {
                ff9.w_weatherColorCounter = 4096;
            }
            ff9.w_weatherInterColor(ref ff9.w_weatherColor.Color[17], ff9.w_weatherColor.Color[19], ff9.w_weatherColor.Color[20], ff9.w_weatherColorCounter, ff9.w_weatherColorCounter);
            ff9.w_weatherInterColor(ref ff9.w_weatherColor.Color[18], ff9.w_weatherColor.Color[21], ff9.w_weatherColor.Color[22], ff9.w_weatherColorCounter, ff9.w_weatherColorCounter);
            if (ff9.w_weatherColorCounter == 4096)
            {
                ff9.w_weatherColorCounter = -1;
            }
        }
        Int32 num = (Int32)(ff9.w_cameraWorldAim.y * 256f);
        num -= 2200;
        num *= 4096;
        num /= 4500;
        if (num > 4096)
        {
            num = 4096;
        }
        if (num < 0)
        {
        }
        num = 4096;
        ff9.w_weatherInterColor(ref ff9.w_weatherColor.Color[16], ff9.w_weatherColor.Color[17], ff9.w_weatherColor.Color[18], num, num);
        ff9.w_nwpChangeStat();
        ff9.w_lightChangeStat();
    }

    public static void w_weatherDeside()
    {
        Int32 num = ff9.w_weatherModeCheck();
        if (num != ff9.w_weatherModeOld)
        {
            ff9.w_weatherModeOld = num;
            ff9.w_weatherDest(num, ff9.w_weatherFogCheck() ? 1 : 0, 128);
        }
        if (ff9.ushort_gEventGlobal_keventScriptNo() == 0 && ff9.w_frameCounterReady == 10)
        {
            switch (ff9.m_GetIDArea(ff9.m_moveActorID))
            {
                case 9:
                case 12:
                case 13:
                    ff9.w_weatherColor.Color[3].goffsetup = 32600;
                    ff9.w_weatherColor.Color[3].toffsetup = 32600;
                    ff9.w_weatherDest(num, (!ff9.w_weatherFogCheck()) ? 0 : 1, 128);
                    ff9.w_frameCloud = false;
                    break;
            }
        }
    }

    public static void w_weatherModeCheck_ChangeSettings()
    {
        // RenderSettings apply to geometry, shader apply to objects (ship)
        if (FF9StateSystem.World.IsBeeScene)
        {
            ff9.w_frameFog = 0;
            ff9.w_frameFog = (Byte)((!ff9.tweaker.w_frameFog) ? 0 : 1);
        }
        Single MistStartDistance_base = Configuration.Worldmap.MistStartDistance_base / 100f; // default 80
        Single MistStartMul = Configuration.Worldmap.MistStartDistance / 100f; // default 80
        Single MistEndMul = Configuration.Worldmap.MistEndDistance / 100f; // 80
        if (MistStartMul >= MistEndMul)
            MistEndMul = MistStartMul + 0.001f;
        Single fogStartMul = Configuration.Worldmap.FogStartDistance / 100f;
        Single fogEndMul = Configuration.Worldmap.FogEndDistance / 100f;
        if (fogStartMul >= fogEndMul)
            fogEndMul = fogStartMul + 0.001f;
        GlobalFog globalFog = GameObject.Find("WorldCamera").GetComponent<GlobalFog>(); // ff9.world.GlobalFog;
        //Log.Message("globalFog.enabled:" + globalFog.enabled + " ff9.w_frameFog:" + ff9.w_frameFog + " w_frameScenePtr:" + w_frameScenePtr);
        if (globalFog.enabled)
        {
            globalFog.distanceFog = true;
            globalFog.useRadialDistance = true;

            if (ff9.world.SkyDome_Fog.gameObject.activeSelf)
                ff9.world.SkyDome_Fog.gameObject.SetActive(false);

            if (ff9.w_frameFog == 1) // mist
            {
                globalFog.heightFog = true;
                globalFog.height = 29f;
                globalFog.heightDensity = Configuration.Worldmap.MistDensity / 100f; // 0.07f; 0-10
                globalFog.startDistance = 100f * MistStartDistance_base; // 55.5f;
                /*
                RenderSettings.fogStartDistance = 26.7f * MistStartMul;
                RenderSettings.fogEndDistance = 80f * MistEndMul;
                Single start = Mathf.Lerp(26f, 77.4f, t);
                Single end = Mathf.Lerp(65.9f, 114.9f, t);
                Shader.SetGlobalFloat("_FogStartDistance", start * MistStartMul);
                Shader.SetGlobalFloat("_FogEndDistance", end * MistEndMul);
                */
                RenderSettings.fogStartDistance = 100f * MistStartMul; // 26.7f
                RenderSettings.fogEndDistance = 100f * MistEndMul; // 80f
                Single t = ((ff9.w_frameCameraPtr.transform.position.y / 52.1875f) - 0.40f) * 2.0f;
                Single start = Mathf.Lerp(RenderSettings.fogStartDistance * 0.80f, RenderSettings.fogStartDistance, t) + globalFog.startDistance;
                Single end = Mathf.Lerp(RenderSettings.fogEndDistance * 0.80f, RenderSettings.fogEndDistance, t) + globalFog.startDistance;
                //Log.Message("t:" + t + " - start:" + start + " - end:" + end);
                Shader.SetGlobalFloat("_FogStartDistance", start * MistStartMul);
                Shader.SetGlobalFloat("_FogEndDistance", end * MistEndMul);
                if (ff9.IsEventWorldMap)
                {
                    globalFog.startDistance = 55.5f;
                    RenderSettings.fogStartDistance = 46.8f;
                    RenderSettings.fogEndDistance = 82.8f;
                    start = Mathf.Lerp(46.8f, 82.8f, t);
                    end = Mathf.Lerp(85.8f, 114.2f, t);
                    Shader.SetGlobalFloat("_FogStartDistance", start);
                    Shader.SetGlobalFloat("_FogEndDistance", end);
                }
            }
            else if (ff9.w_frameFog == 0) // no mist (disc 2.5-3)
            {
                globalFog.heightFog = false;
                globalFog.height = 59.7f;
                globalFog.heightDensity = 10f;
                globalFog.startDistance = 0f;
                /*
                RenderSettings.fogStartDistance = 86.25f * fogStartMul;
                RenderSettings.fogEndDistance = 142.2f * fogEndMul;
                Single start = Mathf.Lerp(88.6f, 92.6f, t);
                Single end = Mathf.Lerp(158.8f, 168.5f, t);
                Shader.SetGlobalFloat("_FogStartDistance", start);
                Shader.SetGlobalFloat("_FogEndDistance", end);
                */
                RenderSettings.fogStartDistance = 100f * fogStartMul; // 86.25f
                RenderSettings.fogEndDistance = 100f * fogEndMul; // 142.2f
                Shader.SetGlobalFloat("_FogStartDistance", RenderSettings.fogStartDistance);
                Shader.SetGlobalFloat("_FogEndDistance", RenderSettings.fogEndDistance);
            }
        }
        else
        {
            if (ff9.w_frameFog == 1)
            {
                RenderSettings.fogStartDistance = 100f * MistStartMul; // 43.8f
                RenderSettings.fogEndDistance = 100f * MistEndMul; // 96.7f
                Shader.SetGlobalFloat("_FogStartDistance", RenderSettings.fogStartDistance);
                Shader.SetGlobalFloat("_FogEndDistance", RenderSettings.fogEndDistance);
            }
            else if (ff9.w_frameFog == 0)
            {
                RenderSettings.fogStartDistance = 100f * fogStartMul; // 130.4f
                RenderSettings.fogEndDistance = 100f * fogEndMul; // 165.9f
                Shader.SetGlobalFloat("_FogStartDistance", RenderSettings.fogStartDistance);
                Shader.SetGlobalFloat("_FogEndDistance", RenderSettings.fogEndDistance);
            }
        }
    }

    public static Int32 w_weatherModeCheck()
    {
        if (ff9.w_moveActorPtr == null)
        {
            return -1;
        }
        ff9.w_weatherModeCheck_ChangeSettings();
        return WorldConfiguration.GetWeatherLight();
    }

    public static Boolean w_weatherFogCheck()
    {
        return WorldConfiguration.UseMist();
    }

    public static void w_weatherDest(Int32 weather, Int32 fog, Int32 speed)
    {
        if (weather == 5)
        {
            weather = ff9.w_weatherMode;
        }
        if (fog == 2)
        {
            fog = ff9.w_frameFog;
        }
        ff9.w_weatherMode = weather;
        ff9.w_frameFog = (Byte)fog;
        ff9.w_weatherColorCounterSpeed = speed;
        ff9.w_weatherColorCounter = 0;
        ff9.w_weatherColor.Color[19] = ff9.w_weatherColor.Color[17];
        ff9.w_weatherColor.Color[21] = ff9.w_weatherColor.Color[18];
        switch (weather)
        {
            case 0:
                ff9.w_weatherColor.Color[20] = ff9.w_weatherColor.Color[3];
                ff9.w_weatherColor.Color[22] = ff9.w_weatherColor.Color[2];
                break;
            case 1:
                ff9.w_weatherColor.Color[20] = ff9.w_weatherColor.Color[7];
                ff9.w_weatherColor.Color[22] = ff9.w_weatherColor.Color[6];
                break;
            case 2:
                ff9.w_weatherColor.Color[20] = ff9.w_weatherColor.Color[11];
                ff9.w_weatherColor.Color[22] = ff9.w_weatherColor.Color[10];
                break;
            case 3:
                ff9.w_weatherColor.Color[20] = ff9.w_weatherColor.Color[15];
                ff9.w_weatherColor.Color[22] = ff9.w_weatherColor.Color[14];
                break;
            default:
            {
                String message = String.Format("Illigal Weather Mode : {0}", weather);
                global::Debug.Log(message);
                break;
            }
        }
    }

    public static void w_weatherInterColor(ref ff9.sw_weatherColorElement result, ff9.sw_weatherColorElement from, ff9.sw_weatherColorElement to, Int32 t1, Int32 t2)
    {
        if (t1 < 0)
        {
            t1 = 0;
        }
        if (t1 > 4096)
        {
            t1 = 4096;
        }
        if (t2 < 0)
        {
            t2 = 0;
        }
        if (t2 > 4096)
        {
            t2 = 4096;
        }
        ff9.SVECTOR svector = result.light0;
        ff9.SVECTOR svector2 = from.light0;
        ff9.SVECTOR svector3 = to.light0;
        svector.vx = (Int16)ff9.w_frameInter16(svector2.vx, svector3.vx, t1, 13);
        svector.vy = (Int16)ff9.w_frameInter16(svector2.vy, svector3.vy, t1, 13);
        svector.vz = (Int16)ff9.w_frameInter16(svector2.vz, svector3.vz, t1, 13);
        result.light0 = svector;
        svector = result.light1;
        svector2 = from.light1;
        svector3 = to.light1;
        svector.vx = (Int16)ff9.w_frameInter16(svector2.vx, svector3.vx, t1, 13);
        svector.vy = (Int16)ff9.w_frameInter16(svector2.vy, svector3.vy, t1, 13);
        svector.vz = (Int16)ff9.w_frameInter16(svector2.vz, svector3.vz, t1, 13);
        result.light1 = svector;
        svector = result.light2;
        svector2 = from.light2;
        svector3 = to.light2;
        svector.vx = (Int16)ff9.w_frameInter16(svector2.vx, svector3.vx, t1, 13);
        svector.vy = (Int16)ff9.w_frameInter16(svector2.vy, svector3.vy, t1, 13);
        svector.vz = (Int16)ff9.w_frameInter16(svector2.vz, svector3.vz, t1, 13);
        result.light2 = svector;
        svector = result.light0c;
        svector2 = from.light0c;
        svector3 = to.light0c;
        svector.vx = (Int16)ff9.w_frameInter16(svector2.vx, svector3.vx, t1, 13);
        svector.vy = (Int16)ff9.w_frameInter16(svector2.vy, svector3.vy, t1, 13);
        svector.vz = (Int16)ff9.w_frameInter16(svector2.vz, svector3.vz, t1, 13);
        result.light0c = svector;
        svector = result.chrBIAS;
        svector2 = from.chrBIAS;
        svector3 = to.chrBIAS;
        svector.vx = (Int16)ff9.w_frameInter16(svector2.vx, svector3.vx, t1, 13);
        svector.vy = (Int16)ff9.w_frameInter16(svector2.vy, svector3.vy, t1, 13);
        svector.vz = (Int16)ff9.w_frameInter16(svector2.vz, svector3.vz, t1, 13);
        result.chrBIAS = svector;
        result.fogAMP = (UInt16)ff9.w_frameInter16((Int16)from.fogAMP, (Int16)to.fogAMP, t1, 13);
        svector = result.ambient;
        svector2 = from.ambient;
        svector3 = to.ambient;
        Int32 from2 = svector2.vx << 12;
        Int32 to2 = svector3.vx << 12;
        svector.vx = (Int16)(ff9.w_frameInter(from2, to2, t1) >> 12);
        from2 = svector2.vy << 12;
        to2 = svector3.vy << 12;
        svector.vy = (Int16)(ff9.w_frameInter(from2, to2, t1) >> 12);
        from2 = svector2.vz << 12;
        to2 = svector3.vz << 12;
        svector.vz = (Int16)(ff9.w_frameInter(from2, to2, t1) >> 12);
        result.ambient = svector;
        svector = result.ambientcl;
        svector2 = from.ambientcl;
        svector3 = to.ambientcl;
        Int32 from3 = svector2.vx << 12;
        Int32 to3 = svector3.vx << 12;
        svector.vx = (Int16)(ff9.w_frameInter(from3, to3, t1) >> 12);
        from3 = svector2.vy << 12;
        to3 = svector3.vy << 12;
        svector.vy = (Int16)(ff9.w_frameInter(from3, to3, t1) >> 12);
        from3 = svector2.vz << 12;
        to3 = svector3.vz << 12;
        svector.vz = (Int16)(ff9.w_frameInter(from3, to3, t1) >> 12);
        result.ambientcl = svector;
        svector = result.fogUP;
        svector2 = from.fogUP;
        svector3 = to.fogUP;
        svector.vx = (Int16)ff9.w_frameInter(svector2.vx, svector3.vx, t1);
        svector.vy = (Int16)ff9.w_frameInter(svector2.vy, svector3.vy, t1);
        svector.vz = (Int16)ff9.w_frameInter(svector2.vz, svector3.vz, t1);
        result.fogUP = svector;
        svector = result.fogDW;
        svector2 = from.fogDW;
        svector3 = to.fogDW;
        svector.vx = (Int16)ff9.w_frameInter(svector2.vx, svector3.vx, t1);
        svector.vy = (Int16)ff9.w_frameInter(svector2.vy, svector3.vy, t1);
        svector.vz = (Int16)ff9.w_frameInter(svector2.vz, svector3.vz, t1);
        result.fogDW = svector;
        svector = result.fogCL;
        svector2 = from.fogCL;
        svector3 = to.fogCL;
        svector.vx = (Int16)ff9.w_frameInter(svector2.vx, svector3.vx, t1);
        svector.vy = (Int16)ff9.w_frameInter(svector2.vy, svector3.vy, t1);
        svector.vz = (Int16)ff9.w_frameInter(svector2.vz, svector3.vz, t1);
        result.fogCL = svector;
        result.goffsetup = (UInt16)ff9.w_frameInter(from.goffsetup, to.goffsetup, t1);
        result.toffsetup = (UInt16)ff9.w_frameInter(from.toffsetup, to.toffsetup, t1);
        result.goffsetdw = (UInt16)ff9.w_frameInter(from.goffsetdw, to.goffsetdw, t1);
        result.toffsetdw = (UInt16)ff9.w_frameInter(from.toffsetdw, to.toffsetdw, t1);
        result.goffsetcl = (UInt16)ff9.w_frameInter(from.goffsetcl, to.goffsetcl, t1);
        result.toffsetcl = (UInt16)ff9.w_frameInter(from.toffsetcl, to.toffsetcl, t1);
        result.goffsetup = (UInt16)(result.goffsetup & (Int64)(-2));
        result.toffsetup = (UInt16)(result.toffsetup & (Int64)(-2));
        result.goffsetdw = (UInt16)(result.goffsetdw & (Int64)(-2));
        result.toffsetdw = (UInt16)(result.toffsetdw & (Int64)(-2));
        result.goffsetcl = (UInt16)(result.goffsetcl & (Int64)(-2));
        result.toffsetcl = (UInt16)(result.toffsetcl & (Int64)(-2));
        Single offsetX = result.offsetX;
        Single offsetX2 = from.offsetX;
        Single offsetX3 = to.offsetX;
        offsetX = ff9.w_frameInter16(offsetX2, offsetX3, t1, 13);
        result.offsetX = offsetX;
        Single scaleY = result.scaleY;
        Single scaleY2 = from.scaleY;
        Single scaleY3 = to.scaleY;
        scaleY = ff9.w_frameInter16(scaleY2, scaleY3, t1, 13);
        result.scaleY = scaleY;
        Color skyBgColor = result.skyBgColor;
        Color skyBgColor2 = from.skyBgColor;
        Color skyBgColor3 = to.skyBgColor;
        skyBgColor = ff9.w_frameInter16(skyBgColor2, skyBgColor3, t1, 13);
        result.skyBgColor = skyBgColor;
        Color skyFogColor = result.skyFogColor;
        Color skyFogColor2 = from.skyFogColor;
        Color skyFogColor3 = to.skyFogColor;
        skyFogColor = ff9.w_frameInter16(skyFogColor2, skyFogColor3, t1, 13);
        result.skyFogColor = skyFogColor;
        Single lightColorFactor = result.lightColorFactor;
        Single lightColorFactor2 = from.lightColorFactor;
        Single lightColorFactor3 = to.lightColorFactor;
        lightColorFactor = ff9.w_frameInter16(lightColorFactor2, lightColorFactor3, t1, 13);
        result.lightColorFactor = lightColorFactor;
    }

    public static void w_worldSystemConstructor()
    {
        Int64 num = 0L;
        //ff9.w_cellSystemConstructor();
        ff9.w_blockSystemConstructor();
        for (Int64 num2 = 0L; num2 < 26L; num2 += 1L)
        {
            ff9.w_worldZoneInfo[(Int32)(checked(num2))] = (Byte)num;
            num += ff9.w_worldZoneFigure[(Int32)(checked((IntPtr)num2))] * 2;
        }
        ff9.FF9Pc_SeekReadB(ff9.w_fileImagename[0], ff9.w_fileImageSectorInfo[1].start * 2048, (UInt32)(ff9.w_fileImageSectorInfo[1].length * 2048), ff9.w_memorySPSData);
        ff9.w_worldChangeBlockSet();
        if (ff9.w_frameScenePtr >= 5990)
            ff9.w_naviMapno = 1;
        else
            ff9.w_naviMapno = 0;
        ff9.w_naviTitle = -1;
        switch (ff9.w_frameScenePtr)
        {
            case 2400:
                ff9.w_naviTitle = 0;
                break;
            case 5990:
                ff9.w_naviTitle = 1;
                break;
            case 9605:
                ff9.w_naviTitle = 2;
                break;
            case 9890:
                ff9.w_naviTitle = 3;
                break;
        }
        ff9.w_worldSeaBlockPtr = ff9.w_framePackGetPtr_sNWBBlockHeader(ff9.w_memorySPSData, 66);
        ff9.w_frameBattleScenePtr = ff9.w_framePackGetPtr_w_frameBattleScenePtr(ff9.w_memorySPSData, 3);
        ff9.w_textureProjWork = ff9.w_framePackGetPtr_w_textureProjWork(ff9.w_memorySPSData, 6);
        ff9.w_worldEncountSpecial = ff9.w_framePackGetPtr_w_worldEncountSpecial(ff9.w_memorySPSData, 4);
        ff9.w_effectDataList = new ff9.s_effectDataList[12];
        for (Int32 i = 41; i < 53; i++)
            ff9.w_effectDataList[i - 41] = ff9.w_framePackGetPtr_s_effectDataList(ff9.w_memorySPSData, i);
        ff9.w_EvaCore = ff9.w_framePackGetPtr_Eva(ff9.w_memorySPSData, 37);
        if (w_friendlyBattles.Count == 0)
        {
            for (Int32 i = 0; i < 9; i++)
            {
                for (Int32 j = 0; j < 12; j++)
                {
                    Int32 specialArea = ff9.w_worldEncountSpecial[i].area[j] - 1;
                    if (specialArea > 0 && specialArea < ff9.w_frameBattleScenePtr.Length)
                        w_friendlyBattles.Add(ff9.w_frameBattleScenePtr[specialArea].scene[3]);
                }
            }
        }
    }

    public static ff9.sNWBBlockHeader w_framePackGetPtr_sNWBBlockHeader(Byte[] data, Int32 no)
    {
        Int32 pos = ff9.w_framePackExtractPosition(data, no);
        ff9.sNWBBlockHeader result;
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                memoryStream.Position = pos;
                ff9.sNWBBlockHeader sNWBBlockHeader = binaryReader.ReadNWBBlockHeader();
                result = sNWBBlockHeader;
            }
        }
        return result;
    }

    /*
    public static Byte[] w_framePackGetPtr_w_frameMessagePtr(Byte[] data, Int32 no)
    {
        if (no != 0)
        {
            global::Debug.LogError("no must be kWorldPackEffectAreaBin.");
        }
        Int32 pos = ff9.w_framePackExtractPosition(data, no);
        Byte[] result;
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            using (new BinaryReader(memoryStream))
            {
                memoryStream.Position = pos;
                result = null;
            }
        }
        return result;
    }
    */

    public static EncountData[] w_framePackGetPtr_w_frameBattleScenePtr(Byte[] data, Int32 no)
    {
        if (no != 3)
        {
            global::Debug.LogError("no must be kWorldPackEncountTable.");
        }
        Int32 pos = ff9.w_framePackExtractPosition(data, no);
        EncountData[] result;
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                memoryStream.Position = pos;
                EncountData[] array2 = binaryReader.ReadEncountData();
                result = array2;
            }
        }
        return result;
    }

    /*
    public static ff9.sw_weatherColor w_framePackGetPtr_sw_weatherColor(Byte[] data, Int32 no)
    {
        if (no != 5)
        {
            global::Debug.LogError("no must be kWorldPackColorTable.");
        }
        Int32 pos = ff9.w_framePackExtractPosition(data, no);
        ff9.sw_weatherColor result;
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                memoryStream.Position = pos;
                ff9.sw_weatherColor sw_weatherColor = binaryReader.ReadWeatherColor();
                result = sw_weatherColor;
            }
        }
        return result;
    }
    */

    public static ff9.stextureProject w_framePackGetPtr_w_textureProjWork(Byte[] data, Int32 no)
    {
        if (no != 6)
        {
            global::Debug.LogError("no must be kWorldPackAnimationTable.");
        }
        Int32 pos = ff9.w_framePackExtractPosition(data, no);
        ff9.stextureProject result;
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                memoryStream.Position = pos;
                ff9.stextureProject stextureProject = binaryReader.ReadTextureProjWork();
                result = stextureProject;
            }
        }
        return result;
    }

    public static ff9.sworldEncountSpecial[] w_framePackGetPtr_w_worldEncountSpecial(Byte[] data, Int32 no)
    {
        if (no != 4)
        {
            global::Debug.LogError("no must be kWorldPackEncountSpecial.");
        }
        Int32 pos = ff9.w_framePackExtractPosition(data, no);
        ff9.sworldEncountSpecial[] result;
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                memoryStream.Position = pos;
                ff9.sworldEncountSpecial[] array2 = binaryReader.ReadWorldEncountSpecial();
                result = array2;
            }
        }
        return result;
    }

    /*
    public static ff9.s_effectData w_framePackGetPtr_s_effectData(Byte[] data, Int32 no)
    {
        Int32 pos = ff9.w_framePackExtractPosition(data, no);
        ff9.s_effectData result;
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                memoryStream.Position = pos;
                ff9.s_effectData s_effectData = binaryReader.ReadEffectData();
                result = s_effectData;
            }
        }
        return result;
    }
    */

    public static ff9.s_effectDataList w_framePackGetPtr_s_effectDataList(Byte[] data, Int32 no)
    {
        Int32 pos = ff9.w_framePackExtractPosition(data, no);
        ff9.s_effectDataList result;
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                memoryStream.Position = pos;
                ff9.s_effectDataList s_effectDataList = binaryReader.ReadEffectDataList();
                result = s_effectDataList;
            }
        }
        return result;
    }

    public static Byte[] w_framePackGetPtr_Eva(Byte[] data, Int32 no)
    {
        Int32 pos = ff9.w_framePackExtractPosition(data, no);
        Byte[] result;
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                memoryStream.Position = pos;
                Byte[] array2 = binaryReader.ReadBytes(data.Length);
                result = array2;
            }
        }
        return result;
    }

    public static void w_worldMapConstructor()
    {
        UInt16 num = 0;
        num = (UInt16)(num | ff9.byte_gEventGlobal(194));
        num = (UInt16)(num | (UInt16)(ff9.byte_gEventGlobal(198) << 8));
        for (Int32 i = 0; i < 9; i++)
        {
            if ((num >> i & 1) != 0)
            {
                for (Int32 j = 0; j < 12; j++)
                {
                    UInt16 num2 = ff9.w_worldEncountSpecial[i].area[j];
                    if (num2 - 1 > 0 && num2 - 1 < ff9.w_frameBattleScenePtr.Length)
                    {
                        ff9.w_frameBattleScenePtr[num2 - 1].scene[3] = ff9.w_frameBattleScenePtr[num2 - 1].scene[2];
                    }
                }
            }
        }
        for (Int32 i = 0; i < 3; i++)
        {
            ff9.w_worldLocDistance[i] = 65535;
            ff9.w_worldLocSEFlg[i] = 0;
        }
    }

    public static void w_worldService()
    {
        ff9.w_cellService();
        if (ff9.w_frameCloud)
        {
        }
    }

    public static void w_worldUpdate()
    {
        Int32 index = ff9.w_moveActorPtr.originalActor.index;
        Int32 id = ff9.w_moveCHRStatus[index].id;
        if (ff9.w_musicFirstPlay != 0)
        {
        }
        for (Int32 i = 0; i < 3; i++)
        {
            Int32 num = (ff9.UnityUnit(ff9.w_moveActorPtr.RealPosition[0]) >> 8) - (ff9.w_worldLocX[i] >> 8);
            Int32 num2 = (ff9.UnityUnit(ff9.w_moveActorPtr.RealPosition[2]) >> 8) - (ff9.w_worldLocZ[i] >> 8);
            ff9.w_worldLocDistance[i] = (Int32)ff9.SquareRoot0(num * num + num2 * num2);
            if (!WorldConfiguration.UseWorldEffect(WorldEffect.SandStorm) && i == 0)
                ff9.w_worldLocDistance[0] = 100;
            Int32 num3 = ff9.w_worldLocDistance[i];
            if (num3 < 63)
            {
                if (ff9.w_worldLocSEFlg[i] == 0)
                {
                    ff9.w_worldLocSEFlg[i] = 1;
                    ff9.w_musicSEPlay(ff9.w_worldLocSENum[i], 0);
                }
                else
                {
                    Int64 num4 = 127 - num3 * 2;
                    if (num4 > 0L)
                    {
                        ff9.w_musicSEVolume(ff9.w_worldLocSENum[i], (Byte)num4);
                    }
                }
            }
            else if (ff9.w_worldLocSEFlg[i] == 1)
            {
                ff9.w_worldLocSEFlg[i] = 0;
                ff9.w_musicSEStop(ff9.w_worldLocSENum[i]);
            }
        }
        if (ff9.w_worldLocDistance[2] < 40)
        {
            Int32 num5 = 120 - ff9.w_worldLocDistance[2] * 3;
            if (num5 != 0 && ff9.w_blockReady)
            {
                ff9.w_cameraWorldAim.y = ff9.w_cameraWorldAim.y + (UnityEngine.Random.Range(0f, ff9.S(num5)) - ff9.S(num5) / 2f);
                ff9.w_cameraWorldAim.x = ff9.w_cameraWorldAim.x + (UnityEngine.Random.Range(0f, ff9.S(num5)) - ff9.S(num5) / 2f);
                ff9.w_cameraWorldAim.z = ff9.w_cameraWorldAim.z + (UnityEngine.Random.Range(0f, ff9.S(num5)) - ff9.S(num5) / 2f);
            }
        }
        if (ff9.w_worldLocDistance[1] < 34 && ff9.GetUserControl() && ff9.m_GetIDTopograph(id) == 23 && index >= 1 && index <= 7)
        {
            Vector3 pos = ff9.w_moveActorPtr.pos;
            pos[2] += ff9.S((34 - ff9.w_worldLocDistance[1]) * 8);
            ff9.w_moveActorPtr.pos = pos;
            if (ff9.NearlyEqual(ff9.w_moveActorPtr.pos[0] - ff9.w_moveActorPtr.lastx, 0f) && ff9.NearlyEqual(ff9.w_moveActorPtr.pos[2] - ff9.w_moveActorPtr.lastz, 0f))
            {
                global::Debug.LogWarning("Remove this if you see it 1!");
                ff9.w_moveActorPtr.lastx += ff9.S(1);
            }
        }
        Byte rainStrength;
        Int32 rainSpeed;
        WorldConfiguration.GetRainParameters(out rainStrength, out rainSpeed);
        if (rainStrength > 0)
        {
            ff9.rainRenderer.SetRainParam(rainStrength, rainSpeed);
            ff9.w_frameRain = true;
            FF9StateSystem.Common.FF9.btl_rain = (Byte)(rainStrength >> 4);
        }
        else
        {
            ff9.w_frameRain = false;
            ff9.rainRenderer.SetRainParam(0, 0);
            FF9StateSystem.Common.FF9.btl_rain = 0;
        }
    }

    public static void w_worldChangeBlockSet()
    {
        if (WorldConfiguration.UsePlaceAlternateForm(WorldPlace.SouthGate_Gate)) // South Gate destroyed form
        {
            ff9.mw_worldSetFormBit(18, 14);
            ff9.mw_worldSetFormBit(17, 12);
        }
        if (WorldConfiguration.UsePlaceAlternateForm(WorldPlace.Alexandria)) // Alexandria destroyed form
        {
            ff9.mw_worldSetFormBit(19, 10);
            ff9.mw_worldSetFormBit(19, 11);
            ff9.mw_worldSetFormBit(20, 10);
            ff9.mw_worldSetFormBit(20, 11);
        }
        if (WorldConfiguration.UsePlaceAlternateForm(WorldPlace.FireShrine)) // Fire Shrine opened form
        {
            ff9.mw_worldSetFormBit(7, 1);
            ff9.mw_worldSetFormBit(8, 1);
            ff9.mw_worldSetFormBit(14, 15);
        }
        if (WorldConfiguration.UsePlaceAlternateForm(WorldPlace.Lindblum)) // Lindblum destroyed form
        {
            ff9.mw_worldSetFormBit(13, 16);
            ff9.mw_worldSetFormBit(13, 17);
            ff9.mw_worldSetFormBit(14, 16);
            ff9.mw_worldSetFormBit(14, 17);
        }
        if (WorldConfiguration.UsePlaceAlternateForm(WorldPlace.Cleyra)) // Cleyra destroyed form
        {
            ff9.mw_worldSetFormBit(13, 12);
            ff9.mw_worldSetFormBit(14, 12);
        }
        if (WorldConfiguration.UsePlaceAlternateForm(WorldPlace.BlackMageVillage)) // Black Mage Village entered form (?)
        {
            ff9.mw_worldSetFormBit(14, 6);
            ff9.mw_worldSetFormBit(21, 10);
            ff9.mw_worldSetFormBit(22, 14);
        }
        if (WorldConfiguration.UsePlaceAlternateForm(WorldPlace.WaterShrine)) // Water Shrine opened form
        {
            ff9.mw_worldSetFormBit(3, 9);
            ff9.mw_worldSetFormBit(9, 1);
        }
        if (WorldConfiguration.UsePlaceAlternateForm(WorldPlace.MognetCentral)) // Mognet is opened
        {
            ff9.mw_worldSetFormBit(16, 1);
            ff9.mw_worldSetFormBit(13, 4);
            ff9.mw_worldSetFormBit(14, 5);
        }
        if (WorldConfiguration.UsePlaceAlternateForm(WorldPlace.ChocoboParadise)) // Chocobo's Paradise is opened
        {
            ff9.mw_worldSetFormBit(0, 0);
            ff9.mw_worldSetFormBit(16, 14);
            ff9.mw_worldSetFormBit(9, 17);
        }
    }

    public static void mw_worldSetFormBit(Int32 x, Int32 z)
    {
        if (ff9.world.InitialBlocks == null)
        {
            global::Debug.LogWarning("This is not supposed to happen!");
            return;
        }
        WMBlock wmblock = ff9.world.InitialBlocks[x, z];
        wmblock.SetForm(2);
    }

    /*
    public static void mw_worldResetFormBit(Int32 x, Int32 z)
    {
        WMBlock wmblock = ff9.world.InitialBlocks[x, z];
        wmblock.SetForm(1);
    }
    */

    public static Int32 w_worldArea2Zone(Int32 area)
    {
        return ff9.w_worldAreaZone[area];
    }

    public static EncountData w_worldGetBattleScenePtr()
    {
        Int32 useAlternate = 0;
        Int32 zoneId = ff9.w_worldArea2Zone(ff9.m_GetIDArea(ff9.m_moveActorID));
        Int32 zoneInfoStart = ff9.w_worldZoneInfo[zoneId];
        Int32 zoneInfoEnd = ff9.w_worldZoneInfo[zoneId + 1];
        Int32 i;
        for (i = zoneInfoStart; i < zoneInfoEnd; i++)
        {
            Byte pattern = ff9.w_frameBattleScenePtr[i].pattern;
            Byte pad = ff9.w_frameBattleScenePtr[i].pad;
            Byte topographId = (Byte)(pattern >> 2);
            Byte hasFog = (Byte)(pad & 1);
            Byte b = (Byte)(pad >> 1);
            if (topographId == ff9.m_GetIDTopograph(ff9.m_moveActorID) && hasFog == ff9.w_frameFog)
            {
                if (ff9.w_frameDisc == 4 && i < 100)
                    useAlternate = 254;
                else
                    useAlternate = 0;
                Int32 pattern2 = ff9.w_frameBattleScenePtr[i + useAlternate].pattern;
                return ff9.w_frameBattleScenePtr[i + useAlternate];
            }
        }
        return ff9.w_frameBattleScenePtr[i + useAlternate - 1];
    }

    public static void w_worldPos2Cell(Single x, Single z, out Int32 cell)
    {
        Int32 num = (Int32)(x / 32f);
        Int32 num2 = (Int32)(z / -32f);
        cell = num2 * 48 + num;
    }

    public static void ff9worldInternalBattleEncountStart()
    {
        FF9StateGlobal ff = FF9StateSystem.Common.FF9;
        FF9StateWorldSystem ff9World = FF9StateSystem.World.FF9World;
        ff.attr |= 0x2Fu;
        ff9World.attr &= ~0x400u;
        ff9World.attr |= 0x300u;
    }

    public static void ff9worldInternalBattleEncountService()
    {
        FF9StateGlobal ff = FF9StateSystem.Common.FF9;
        FF9StateSystem instance = PersistenSingleton<FF9StateSystem>.Instance;
        FF9StateWorldSystem ff9World = FF9StateSystem.World.FF9World;
        ff.attr &= ~0x2Fu;
        ff9World.attr &= ~0x300u;
        instance.attr |= 0x1000u;
    }

    public static void ff9InitStateWorldMap(Int32 MapNo)
    {
        FF9StateGlobal ff = FF9StateSystem.Common.FF9;
        FF9StateSystem stateSystem = PersistenSingleton<FF9StateSystem>.Instance;
        FF9StateWorldSystem ff9World = FF9StateSystem.World.FF9World;
        FF9StateWorldMap map = FF9StateSystem.World.FF9World.map;
        stateSystem.attr &= ~0x3001u;
        ff9World.attr |= 0x400u;
        map.nextMapNo = ff.wldMapNo = (Int16)MapNo;
        if (!FF9StateSystem.World.IsBeeScene)
        {
            SFXData.Reinit();
            EventEngine eventEngine = PersistenSingleton<EventEngine>.Instance;
            String ebFileName = FF9DBAll.EventDB[MapNo];
            map.evtPtr = EventEngineUtils.loadEventData(ebFileName, EventEngineUtils.ebSubFolderWorld);
            eventEngine.StartEvents(map.evtPtr);
            ETb.InitMessage();
            eventEngine.updateModelsToBeAdded();
        }
    }

    public static void ff9ShutdownStateWorldMap()
    {
        FF9StateWorldMap map = FF9StateSystem.World.FF9World.map;
        FF9StateSystem instance = PersistenSingleton<FF9StateSystem>.Instance;
        FF9StateGlobal ff = FF9StateSystem.Common.FF9;
        switch (map.nextMode)
        {
            case 1:
                ff.fldMapNo = map.nextMapNo;
                instance.mode = 1;
                instance.prevMode = 3;
                break;
            case 2:
                ff.btlMapNo = map.nextMapNo;
                FF9StateSystem.Battle.battleMapIndex = ff.btlMapNo;
                instance.mode = 2;
                instance.prevMode = 3;
                break;
        }
    }

    public const Int32 kSystemImage_AnimTexture = 0;
    public const Int32 kSystemImage_EffectData = 1;
    public const Int32 kSystemImage_BlockHeaderH = 2;
    public const Int32 kSystemImage_BlockHeaderV = 3;
    public const Int32 kSystemImage_BlockArea = 4;
    public const Int32 kSystemImage_Texture = 5;
    public const Int32 kSystemImage_Navimap = 6;
    public const Int32 kSystemImage_Hintmap = 7;
    public const Int32 kSystemImage_StaticTexture = 8;
    public const Int32 kSystemImage_Dynamic4Texture = 9;
    public const Int32 kSystemImage_EffectTexture = 10;
    public const Int32 kMapImage_ModelH = 11;
    public const Int32 kMapImage_ModelV = 12;

    public const Int32 kw_blockQFigure = 16;

    public const Int32 kw_blockBufferSize = 18432;

    public const Int32 kw_blockDistance = 16384;

    public const Int32 kw_blockCellFigure = 2;

    public const Int32 kw_blockCellFigureSQ = 4;

    public const Int32 k_cameraPosstatLookup = 0;
    public const Int32 k_cameraPosstatDown = 1;
    public const Int32 k_cameraPosstatUp = 2;
    public const Int32 k_cameraPosstatUpper = 3;
    public const Int32 k_cameraPosstatFlyEtc = 4;
    public const Int32 k_cameraPosstatFlyShin = 5;
    public const Int32 k_cameraPosstatFlyBun = 6;
    public const Int32 k_cameraPosstatLookdown = 7;
    public const Int32 k_cameraPosstatFlyChoco = 8;
    public const Int32 k_cameraPosstatShip = 9;
    public const Int32 k_cameraPosstatFigure = 10;

    public const Int32 k_cameraCharacterZidane = 0;
    public const Int32 k_cameraCharacterChocobo = 1;
    public const Int32 k_cameraCharacterBlue = 2;
    public const Int32 k_cameraCharacterInvin = 3;
    public const Int32 k_cameraCharacterFlyChoco = 4;
    public const Int32 k_cameraCharacterFigure = 5;

    public const Int32 k_cameraPlaceBunmei = 0;
    public const Int32 k_cameraPlaceShintairiku = 1;
    public const Int32 k_cameraPlaceEtc = 2;
    public const Int32 k_cameraPlaceFigure = 3;

    public const Int32 k_cameraDumper0 = 255;

    public const Int32 k_cameraDumper1 = 0;

    public const Int32 kw_cellDistance = 8192;

    public const Int32 kEffectBlockSGate = 1;
    public const Int32 kEffectBlockHokoraHi = 2;
    public const Int32 kEffectBlockSabaku = 3;
    public const Int32 kEffectBlockKureira = 4;
    public const Int32 kEffectBlockTaki = 5;
    public const Int32 kEffectBlockEva = 6;
    public const Int32 kEffectBlockDari = 7;
    public const Int32 kEffectBlockDagereo1 = 8;
    public const Int32 kEffectBlockDagereo2 = 9;
    public const Int32 kEffectBlockHokoraMizu = 10;
    public const Int32 kEffectBlockHokoraKaze = 11;
    public const Int32 kEffectBlockHokoraChi = 12;
    public const Int32 kEffectBlockFigure = 13;

    public const Int32 kEffectModelWindMill = 0;
    public const Int32 kEffectModelTwister = 1;
    public const Int32 kEffectModelTwisterR = 2;
    public const Int32 kEffectModelSpiral0 = 3;
    public const Int32 kEffectModelSpiral1 = 4;
    public const Int32 kEffectModelSpiral2 = 5;
    public const Int32 kEffectModelLast0 = 6;
    public const Int32 kEffectModelLast1 = 7;
    public const Int32 kEffectModelLastR = 8;
    public const Int32 kEffectModelFigure = 9;

    public const Int32 eNone = 0;
    public const Int32 eWait = 1;
    public const Int32 eDelete = 2;
    public const Int32 eEncount = 3;
    public const Int32 eJump = 4;
    public const Int32 eWJump = 5;
    public const Int32 eStop = 6;
    public const Int32 eMinigame = 7;
    public const Int32 eGameover = 8;

    public const Int32 classObj = 0;
    public const Int32 classSeq = 1;
    public const Int32 classThread = 2;
    public const Int32 classQuad = 3;
    public const Int32 classActor = 4;
    public const Int32 classEncPoint = 5;

    public const Int32 flagShow = 1;
    public const Int32 flagCollInhC = 2;
    public const Int32 flagCollInhNC = 4;
    public const Int32 flagTalkInh = 8;
    public const Int32 flagLockFreeInh = 16;
    public const Int32 flagHideInh = 32;
    public const Int32 flagTurn = 128;

    public const Int32 stateNew = 0;
    public const Int32 stateRunning = 1;
    public const Int32 stateInit = 2;
    public const Int32 stateSuspend = 3;

    public const Int32 kw_fileCDSectorSize = 2048;

    public const Int32 kw_fileOriginal = 0;
    public const Int32 kw_fileStatic = 1;
    public const Int32 kw_fileTemp = 2;
    public const Int32 kw_fileHGeometory = 3;
    public const Int32 kw_fileVGeometory = 4;
    public const Int32 kw_fileFigure = 5;

    public const Int32 kframePhaseInitEvent = 0;
    public const Int32 kframePhaseReadCD = 1;
    public const Int32 kframePhaseMainLoop = 2;

    public const Int32 kframeCDNone = 0;
    public const Int32 kframeCDGeom = 1;
    public const Int32 kframeCDMusic = 2;
    public const Int32 kframeCDMenu = 3;

    public const Int32 kmovementForestID1 = 36;
    public const Int32 kmovementForestID2 = 37;
    public const Int32 kmovementForestID3 = 38;

    public const Int32 kmovementShallowID1 = 53;
    public const Int32 kmovementShallowID2 = 54;
    public const Int32 kmovementShallowID3 = 55;

    public const Int32 kmovementSeaID1 = 56;
    public const Int32 kmovementSeaID2 = 57;

    public const Int32 kmovementRiverID1 = 51;
    public const Int32 kmovementRiverID2 = 48;

    public const Int32 kmovementBridgeID1 = 52;

    public const Int32 kmovementKaze = 23;

    public const Int32 kmovementCacheSize = 10;

    public const Single kmovementMaximumHeight = 42.1875f;

    public const Int32 WM_ID_UNDEFINED = 0;
    public const Int32 WM_ID_CHR_ZIDANE = 1;
    public const Int32 WM_ID_CHR_DAGGER = 2;
    public const Int32 WM_ID_CHR_CHO_NML = 3;
    public const Int32 WM_ID_CHR_CHO_SHL = 4;
    public const Int32 WM_ID_CHR_CHO_MNT = 5;
    public const Int32 WM_ID_CHR_CHO_SEA = 6;
    public const Int32 WM_ID_CHR_CHO_SKY = 7;
    public const Int32 WM_ID_CHR_BLUE = 8;
    public const Int32 WM_ID_CHR_HLD_3 = 9;
    public const Int32 WM_ID_CHR_INVINCIBLE = 10;
    public const Int32 WM_ID_SUB_MOG = 11;
    public const Int32 WM_ID_SUB_TREASURE = 12;
    public const Int32 WM_ID_SUB_TENT = 13;
    public const Int32 WM_ID_SUB_SHADOW = 14;
    public const Int32 WM_ID_EVENT_CARGO = 15;
    public const Int32 WM_ID_EVENT_HLD_1 = 16;
    public const Int32 WM_ID_EXT_SHIP_A = 17;
    public const Int32 WM_ID_EXT_SHIP_A_SML = 18;
    public const Int32 WM_ID_EXT_SHIP_B = 19;
    public const Int32 WM_ID_EXT_SHIP_B_SML = 20;
    public const Int32 WM_ID_EXT_SHIP_C = 21;
    public const Int32 WM_ID_FIGURE = 22;

    public const Int32 k_moveCHRControl_Human = 0;
    public const Int32 k_moveCHRControl_Nomalchocobo = 1;
    public const Int32 k_moveCHRControl_Asasechocobo = 2;
    public const Int32 k_moveCHRControl_Yamachocobo = 3;
    public const Int32 k_moveCHRControl_Umichocobo = 4;
    public const Int32 k_moveCHRControl_SoraWalkchocobo = 5;
    public const Int32 k_moveCHRControl_SoraFlychocobo = 6;
    public const Int32 k_moveCHRControl_Bluenalusisu = 7;
    public const Int32 k_moveCHRControl_Hirudagarude = 8;
    public const Int32 k_moveCHRControl_Invincible = 9;
    public const Int32 k_moveCHRControl_Debug = 10;
    public const Int32 k_moveCHRControl_None = 11;
    public const Int32 k_moveCHRControl_Element = 12;

    public const Int32 k_moveCHRContolType_Human = 0;
    public const Int32 k_moveCHRContolType_Plane = 1;
    public const Int32 k_moveCHRContolType_Ship = 2;
    public const Int32 k_moveCHRContolType_Debug = 3;

    public const Int32 k_moveCHRStatus_Undefined = 0;
    public const Int32 k_moveCHRStatus_Zidane = 1;
    public const Int32 k_moveCHRStatus_Dagger = 2;
    public const Int32 k_moveCHRStatus_ChoNml = 3;
    public const Int32 k_moveCHRStatus_ChoShl = 4;
    public const Int32 k_moveCHRStatus_ChoMnt = 5;
    public const Int32 k_moveCHRStatus_ChoSea = 6;
    public const Int32 k_moveCHRStatus_ChoSky = 7;
    public const Int32 k_moveCHRStatus_Blue = 8;
    public const Int32 k_moveCHRStatus_Hilda3 = 9;
    public const Int32 k_moveCHRStatus_Invin = 10;
    public const Int32 k_moveCHRStatus_Mog = 11;
    public const Int32 k_moveCHRStatus_Treasure = 12;
    public const Int32 k_moveCHRStatus_Tent = 13;
    public const Int32 k_moveCHRStatus_Shadow = 14;
    public const Int32 k_moveCHRStatus_Cargo = 15;
    public const Int32 k_moveCHRStatus_Hilda1 = 16;
    public const Int32 k_moveCHRStatus_ShipA = 17;
    public const Int32 k_moveCHRStatus_ShipASml = 18;
    public const Int32 k_moveCHRStatus_ShipB = 19;
    public const Int32 k_moveCHRStatus_ShipBSml = 20;
    public const Int32 k_moveCHRStatus_ShipC = 21;
    public const Int32 k_moveCHRStatus_Element = 22;

    public const Int32 k_moveCHRStatusSlice_Type0 = 0;
    public const Int32 k_moveCHRStatusSlice_Type1 = 1;
    public const Int32 k_moveCHRStatusSlice_Type2 = 2;

    public const Int32 k_moveCHRCache_Actor = 0;
    public const Int32 k_moveCHRCache_Choco = 1;
    public const Int32 k_moveCHRCache_Plane = 2;
    public const Int32 k_moveCHRCache_Mog = 3;
    public const Int32 k_moveCHRCache_NPC1 = 4;
    public const Int32 k_moveCHRCache_NPC2 = 5;
    public const Int32 k_moveCHRCache_NPC3 = 6;
    public const Int32 k_moveCHRCache_NPC4 = 7;
    public const Int32 k_moveCHRCache_NPC5 = 8;
    public const Int32 k_moveCHRCache_Temp = 9;
    public const Int32 k_moveCHRCache_None = 10;
    public const Int32 k_moveCHRCache_Element = 11;

    public const Int32 kmusicFieldSongFigure = 5;

    public const Int32 kmusicProgressReady = 0;
    public const Int32 kmusicProgressRequest = 1;
    public const Int32 kmusicProgressReading = 2;

    public const Int32 kmusicNormal = 0;
    public const Int32 kmusicChocobo = 1;
    public const Int32 kmusicAirPlane = 2;
    public const Int32 kmusicDisc4 = 3;
    public const Int32 kmusicField1 = 4;
    public const Int32 kmusicField2 = 5;
    public const Int32 kmusicField3 = 6;
    public const Int32 kmusicField4 = 7;

    public const Int32 kmusicWavePlaneAndChocobo = 0;
    public const Int32 kmusicWaveNormalAndChocobo = 1;

    public const Int32 kmusicSEAttrGeneral = 524288;

    public const Int32 kmusicSEAttrPlane = 0;

    public const Int32 kmusicSEAttrEvent = 2048;

    public const Int32 kmusicSEAttrOther = 128;

    public const Int32 kmusicSESystemReserve0 = 0;
    public const Int32 kmusicSESystemReserve1 = 1;
    public const Int32 kmusicSESystemReserve2 = 2;
    public const Int32 kmusicSESystemReserve3 = 3;
    public const Int32 kmusicSESystemReserve4 = 4;
    public const Int32 kmusicSESystemReserve5 = 5;
    public const Int32 kmusicSESystemReserve6 = 6;
    public const Int32 kmusicSESystemReserve7 = 7;

    public const Int32 kmusicSEAirPlaneCargo = 20;
    public const Int32 kmusicSEAirPlaneHilda1 = 21;
    public const Int32 kmusicSEAirPlaneInvin = 22;
    public const Int32 kmusicSEAirPlaneHilda3 = 23;
    public const Int32 kmusicSEAirPlaneBlue = 24;
    public const Int32 kmusicSEZimen = 25;
    public const Int32 kmusicSEKaze = 26;

    public const Int32 kmusicSEReserveShiromoto0 = 30;
    public const Int32 kmusicSEReserveShiromoto1 = 31;
    public const Int32 kmusicSEReserveShiromoto2 = 32;
    public const Int32 kmusicSEReserveShiromoto3 = 33;
    public const Int32 kmusicSEReserveShiromoto4 = 34;
    public const Int32 kmusicSEReserveShiromoto5 = 35;
    public const Int32 kmusicSEReserveShiromoto6 = 36;
    public const Int32 kmusicSEReserveShiromoto7 = 37;
    public const Int32 kmusicSEReserveShiromoto8 = 38;
    public const Int32 kmusicSEReserveShiromoto9 = 39;
    public const Int32 kmusicSEReserveShiromoto10 = 40;
    public const Int32 kmusicSEReserveShiromoto11 = 41;
    public const Int32 kmusicSEReserveShiromoto12 = 42;

    public const Int32 knaviNone = 0;
    public const Int32 knaviSmall = 1;
    public const Int32 knaviLarge = 2;
    public const Int32 knaviHint = 3;

    public const Int32 knaviSmallBunmei = 0;
    public const Int32 knaviLargeBunmei = 1;
    public const Int32 knaviSmallZentai = 2;
    public const Int32 knaviLargeZentai = 3;

    public const Int32 knaveOTDepth = 10;

    public const Int32 knaviHintmapX = 249;

    public const Int32 knaviHintmapY = 96;

    public const Int32 knaviHintmapPW = 42;

    public const Int32 knaviHintmapPH = 29;

    public const Int32 knaviHintmapU = 48;

    public const Int32 knaviHintmapV = 176;

    public const Int32 knaviHintPX = 42;

    public const Int32 knaviHintPY = 28;

    public const Int32 knaviHintPosX = 249;

    public const Int32 knaviHintPosY = 96;

    public const Int32 knaviSNakaPX = 96;

    public const Int32 knaviSNakaPY = 80;

    public const Int32 knaviSNakaPosX = 200;

    public const Int32 knaviSNakaPosY = 130;

    public const Int32 knaviWakuPX = 256;

    public const Int32 knaviWakuPY = 216;

    public const Int32 knaviWakuPosX = 32;

    public const Int32 knaviWakuPosY = 1;

    public const Int32 knaviLNakaPX = 240;

    public const Int32 knaviLNakaPY = 200;

    public const Int32 knaviLNakaPosX = 40;

    public const Int32 knaviLNakaPosY = 9;

    public const Int32 knaviEmblemPX = 46;

    public const Int32 knaviEmblemPY = 36;

    public const Int32 knaviEmblemPosX = 32;

    public const Int32 knaviEmblemPosY = 181;

    public const Int32 kw_textureAnimeProjWork = 512;

    public const Int32 kw_texturePixelScroll = 8;

    public const Int32 kw_texturePixelAnime = 4;

    public const Int32 kw_texturePaletScroll = 18;

    public const Int32 kw_textureWorkPosx = 640;

    public const Int32 kw_textureWorkPosy = 248;

    public const Int32 kw_weatherFineU = 0;
    public const Int32 kw_weatherFineD = 1;
    public const Int32 kw_weatherFineUM = 2;
    public const Int32 kw_weatherFineDM = 3;
    public const Int32 kw_weatherSunsetU = 4;
    public const Int32 kw_weatherSunsetD = 5;
    public const Int32 kw_weatherSunsetUM = 6;
    public const Int32 kw_weatherSunsetDM = 7;
    public const Int32 kw_weatherNightU = 8;
    public const Int32 kw_weatherNightD = 9;
    public const Int32 kw_weatherNightUM = 10;
    public const Int32 kw_weatherNightDM = 11;
    public const Int32 kw_weatherEvaU = 12;
    public const Int32 kw_weatherEvaD = 13;
    public const Int32 kw_weatherEvaUM = 14;
    public const Int32 kw_weatherEvaDM = 15;
    public const Int32 kw_weatherNow = 16;
    public const Int32 kw_weatherFrom = 17;
    public const Int32 kw_weatherTo = 18;
    public const Int32 kw_weatherFrom1 = 19;
    public const Int32 kw_weatherFrom2 = 20;
    public const Int32 kw_weatherTo1 = 21;
    public const Int32 kw_weatherTo2 = 22;
    public const Int32 kw_weatherElement = 23;

    public const Int32 kw_weatherFine = 0;
    public const Int32 kw_weatherSunset = 1;
    public const Int32 kw_weatherNight = 2;
    public const Int32 kw_weatherEva = 3;
    public const Int32 kw_weatherAuto = 4;
    public const Int32 kw_weatherNochange = 5;

    public const Int32 kw_weatherFogOFF = 0;
    public const Int32 kw_weatherFogON = 1;
    public const Int32 kw_weatherFogNochange = 2;

    public const Int32 kw_worldBlockMaxX = 24;

    public const Int32 kw_worldBlockMaxZ = 20;

    public const Int32 kw_worldSizeX = 393216;

    public const Int32 kw_worldSizeZ = 327680;

    public const Int32 kWorldCamInDist = 32768;

    public const Int32 kWorldPackEffectAreaBin = 0;
    public const Int32 kWorldPackChocoboPal = 1;
    public const Int32 kWorldPackPaletVolcano = 2;
    public const Int32 kWorldPackEncountTable = 3;
    public const Int32 kWorldPackEncountSpecial = 4;
    public const Int32 kWorldPackColorTable = 5;
    public const Int32 kWorldPackAnimationTable = 6;
    public const Int32 kWorldPackSPSData = 7;

    public const Int32 kWorldPackEffectBin = 41;

    public const Int32 kWorldPackEffectTwister = 53;
    public const Int32 kWorldPackEffectSpiral0 = 54;
    public const Int32 kWorldPackEffectSpiral1 = 55;
    public const Int32 kWorldPackEffectSpiral2 = 56;
    public const Int32 kWorldPackEffectWindmill = 57;
    public const Int32 kWorldPackEffectCore = 58;
    public const Int32 kWorldPackEffectSky = 59;
    public const Int32 kWorldPackEffectSphere1 = 60;
    public const Int32 kWorldPackEffectSphere2 = 61;
    public const Int32 kWorldPackEffectArch = 62;
    public const Int32 kWorldPackEffectBlack = 63;
    public const Int32 kWorldPackEffectThunder1 = 64;
    public const Int32 kWorldPackEffectThunder2 = 65;
    public const Int32 kWorldPackModelSea = 66;

    public const Int32 kworldLocTatsumaki = 0;
    public const Int32 kworldLocKazehokora = 1;
    public const Int32 kworldLocChihokora = 2;
    public const Int32 kworldLocFigure = 3;

    public const Int32 kworldSENone = 0;
    public const Int32 kworldSEPlay = 1;
    public const Int32 kworldSEIntpl = 2;

    public const Int32 AREA_A = 0;
    public const Int32 AREA_B = 1;
    public const Int32 AREA_C = 2;
    public const Int32 AREA_D = 3;
    public const Int32 AREA_E = 4;
    public const Int32 AREA_F = 5;
    public const Int32 AREA_G = 6;
    public const Int32 AREA_H = 7;
    public const Int32 AREA_I = 8;
    public const Int32 AREA_J = 9;
    public const Int32 AREA_K = 10;
    public const Int32 AREA_L = 11;
    public const Int32 AREA_M = 12;
    public const Int32 AREA_N = 13;
    public const Int32 AREA_O = 14;
    public const Int32 AREA_P = 15;

    //private const Single toPsxRot = 11.3777781f;

    //private const Single toUnityRot = 0.087890625f;

    //private const Single toUnityScale = 0.000244140625f;

    public const Int32 SC_COUNTER_ARMOR_BLANK_START = 1500;
    public const Int32 SC_COUNTER_ARMOR_BLANK_END = 1600;
    public const Int32 SC_COUNTER_WMTITLE_BUNMEI_ON = 2400;
    public const Int32 SC_COUNTER_SGATE_DESTROYED = 2990;
    public const Int32 SC_COUNTER_CLAY_DESTROYED = 4990;
    public const Int32 SC_COUNTER_LIND_DESTROYED = 5598;
    public const Int32 SC_COUNTER_FOG_END = 5990;
    public const Int32 SC_COUNTER_KUROMA_APPEAR = 6200;
    public const Int32 SC_COUNTER_DAGGER_AWAKE = 6875;
    public const Int32 SC_COUNTER_SGATE_RECOVER = 6990;
    public const Int32 SC_COUNTER_ALEX_DESTROYED = 8800;
    public const Int32 SC_COUNTER_SUNA_MAKYU_ON = 9450;
    public const Int32 SC_COUNTER_WMTITLE_NEW_ON = 9600;
    public const Int32 SC_COUNTER_SUNA_MAKYU_OFF = 9890;
    public const Int32 SC_COUNTER_CUT_HAIR = 10300;
    public const Int32 SC_COUNTER_GET_HILDA3 = 10400;
    public const Int32 SC_COUNTER_HOKORA_START = 10600;
    public const Int32 SC_COUNTER_HOKORA_END = 10700;
    public const Int32 SC_COUNTER_LAST_WORLD = 11090;

    private const Int32 CAMSAME = 0;
    private const Int32 CAMDOWN = 1;
    private const Int32 CAMUP = 2;

    public const Int32 w_evaCoreSPSMinSize = 46000;

    public const Int32 w_evaCoreSPSMaxSize = 60000;

    private const Int32 ExtraFrameForFadeIn = 5;

    public const Int32 knaviTitlePX = 128;

    public const Int32 knaviTitlePY = 64;

    public const Int32 knaviTitlePosX = 96;

    public const Int32 knaviTitlePosY = 50;

    public const Int32 knaviTitleLPX = 256;

    public const Int32 knaviTitleLPY = 32;

    public const Int32 knaviTitleLPosX = 96;

    public const Int32 knaviTitleLPosY = 50;

    public const Int32 LOC_PX1 = 329728;

    public const Int32 LOC_PZ1 = -240896;

    public const Int32 LOC_DS1 = 22016;

    public const Int32 LOC_PX2 = 330496;

    public const Int32 LOC_PZ2 = -237568;

    public const Int32 LOC_DS2 = 15104;

    public const Int32 LOC_PX3 = 128000;

    public const Int32 LOC_PZ3 = -202240;

    public const Int32 LOC_DS3 = 46080;

    public const Int32 LOC_PX4 = 196158;

    public const Int32 LOC_PZ4 = -81825;

    public const Int32 LOC_DS4 = 26080;

    public const Int32 w_worldBlockFigureX = 24;

    public const Int32 w_worldBlockFigureZ = 20;

    public const Int32 w_worldCellFigureX = 48;

    public const Int32 w_worldCellFigureZ = 40;

    public const Int32 w_worldDistanceX = 393216;

    public const Int32 w_worldDistanceZ = 327680;

    public const Int32 knaviHintmapPX = 792;

    public const Int32 knaviHintmapPY = 176;

    public const Int32 knaviHintmapCX = 640;

    public const Int32 knaviHintmapCY = 246;

    public const Int32 kframeWeather = 0;
    public const Int32 kframeActor = 1;
    public const Int32 kframeNaviActive = 2;
    public const Int32 kframeCamera = 3;
    public const Int32 kframeNaviMode = 4;
    public const Int32 kframeCameraFix = 5;
    public const Int32 kframeEffectPosX = 6;
    public const Int32 kframeEffectPosY = 7;
    public const Int32 kframeEffectPosZ = 8;
    public const Int32 kframeEffectSize = 9;
    public const Int32 kframeEffectID = 10;
    public const Int32 kframeTitleTime = 11;
    public const Int32 kframeDistance1 = 12;
    public const Int32 kframeDistance2 = 13;
    public const Int32 kframeWeatherSpeed = 14;
    public const Int32 kframeWeatherAuto = 15;
    public const Int32 kframeSEParam0 = 16;
    public const Int32 kframeSEParam1 = 17;
    public const Int32 kframeSEParam2 = 18;
    public const Int32 kframeSEParam3 = 19;
    public const Int32 kframeSEPlay = 20;
    public const Int32 kframeSEVol = 21;
    public const Int32 kframeSEVolIntpl = 22;
    public const Int32 kframeSEStop = 23;
    public const Int32 kframeMusicVol = 24;
    public const Int32 kframePRMCameraSmooth = 25;
    public const Int32 kframeEventBattleProb = 26;
    public const Int32 kframeEventNaviCursol = 27;
    public const Int32 kframeGetOff = 28;
    public const Int32 kframeChocoboCall = 29;
    public const Int32 kframeActorSP1 = 30;
    public const Int32 kframeActorSP2 = 31;
    public const Int32 kframeMusicFade = 32;
    public const Int32 kframeMusicFadeForce = 33;
    public const Int32 kframeAutoPilot = 34;
    public const Int32 kframeMogChoice = 35;
    public const Int32 kframeCursolDraw = 36;
    public const Int32 kframeAlphaZero = 37;
    public const Int32 kframeAutoPilotOFF = 38;
    public const Int32 kframeCDUseON = 39;
    public const Int32 kframeCDUseOFF = 40;

    public const Int32 kframeAreaID = 192;
    public const Int32 kframeTopographID = 193;
    public const Int32 kframeGetoffVehiclePosY = 194;
    public const Int32 kframeGetoffHumanPosX = 195;
    public const Int32 kframeGetoffHumanPosY = 196;
    public const Int32 kframeGetoffHumanPosZ = 197;
    public const Int32 kframeMapID = 198;
    public const Int32 kframeCameraRot = 199;
    public const Int32 kframeCDAccess = 200;
    public const Int32 kframeCharacterPosX = 201;
    public const Int32 kframeCharacterPosY = 202;
    public const Int32 kframeCharacterPosZ = 203;
    public const Int32 kframeDistanceEva = 204;
    public const Int32 kframeEventBattle = 205;
    public const Int32 kframeCharacterRotY = 206;
    public const Int32 kframeZoneID = 207;
    public const Int32 kframeMogSlice = 208;
    public const Int32 kframeBlockReady = 209;
    public const Int32 kframeLocationTitle = 210;
    public const Int32 kframeNaviDrawItem = 211;

    public const Int32 kframeChangeCharactorStatus_Debug = 500;
    public const Int32 kframeSetWorldMapState_Debug = 501;
    public const Int32 kframeResetWorldBlockSet_Debug = 502;

    public static Boolean w_blockReady = true;

    public static readonly WMPad kPadPush = new WMPad(WMPadType.Push);

    public static readonly UInt32[] w_movementGroundStatus = new UInt32[]
    {
        0x5C126670u,
        0x18FF3CFFu
    };

    public static readonly UInt32[] w_movementWaterStatus;

    public static readonly Int16[,] w_movementSinkArray;

    public static readonly ff9.s_moveCHRCache[] w_moveCHRCache;

    public static ff9.s_moveCHRControl[] w_moveCHRControl;

    public static ff9.s_moveCHRStatus[] w_moveCHRStatus;

    public static WMActor w_moveDummyCharacter;

    public static WMActor w_moveActorPtr;

    public static WMActor w_moveHumanPtr;

    public static WMActor w_movePlanePtr;

    public static WMActor w_moveChocoboPtr;

    public static WMActor w_moveMogPtr;

    public static Boolean w_isMogActive;

    public static SByte w_moveCHRControl_No;

    public static Byte w_moveCHRControlPoly;

    public static ff9.s_moveCHRControl w_moveCHRControlPtr;

    public static Boolean w_moveCHRControl_Move;

    public static Boolean w_moveCHRControl_LR;

    public static Single w_moveCHRControl_XZSpeed;

    public static Single w_moveCHRControl_YSpeed;

    public static Single w_moveCHRControl_RotSpeed;

    public static Single w_moveCHRControl_XZAlpha;

    public static Single w_moveCHRControl_YAlpha;

    public static Single w_moveCHRControl_RotTrue;

    public static UInt32 w_moveCHRControl_Polyno;

    public static Single w_movementCamRemain;

    public static Single w_moveCHRControl_AutoPrev;

    public static Boolean w_movementSoftRot;

    public static Single w_moveYDiff;

    public static Int32 w_moveDebugChocoboColorForce;

    public static Int32 w_moveDebugNochr;

    public static Boolean w_moveDoping;

    public static Boolean w_movePadLR;

    public static Boolean w_movePadDOWN;

    public static Byte w_moveAutoPilot;

    public static Boolean w_moveAutoPilotPrevCammode;

    public static Single honoUpdateTime;

    public static Boolean PrintDebug;

    public static ff9.sworldState w_cameraSysData;

    public static ff9.sworldStateCamera w_cameraSysDataCamera;

    public static Boolean w_cameraFixMode;

    public static Boolean w_cameraFixModeY;

    public static Boolean w_cameraChrFakeFlag;

    public static Boolean w_cameraFuzzy;

    public static Vector3 w_cameraWorldEye;

    public static Vector3 w_cameraWorldAim;

    public static Vector3 w_cameraFocusEye;

    public static Vector3 w_cameraFocusAim;

    public static Vector3 w_cameraDirVector;

    public static Vector3 w_cameraUpvector;

    public static Vector3 w_cameraEyeOffset;

    public static Single w_cameraAimOffset;

    public static Int16 w_cameraChangeCounter;

    public static Int16 w_cameraChangeCounterSpeed;

    public static Int16 w_cameraChangeChr;

    public static Boolean w_cameraSmoothX;

    public static Boolean w_cameraSmoothY;

    public static Boolean w_cameraSmoothZ;

    public static Boolean w_cameraInter;

    public static Byte w_cameraFirstDeside;

    public static Single w_cameraRotAngle;

    public static readonly ff9.s_moveCHRCache[] w_cameraHit;

    public static SByte w_cameraForcePlace;

    public static Int32 w_cameraDebugDist;

    public static ff9.s_cameraPosstat w_cameraPosstatPrev;

    public static ff9.s_cameraPosstat w_cameraPosstatNow;

    public static readonly Byte[] w_cameraArea2Place;

    public static readonly ff9.s_cameraPosstat[] w_cameraPosstat;

    public static readonly ff9.s_cameraElement[,] w_cameraElement;

    private static Boolean converted_w_cameraPosstat;

    public static Single nsp;

    public static Int32 sa0;

    public static Int32 sa1;

    public static Int32 sa2;

    public static Single sa3;

    public static Int32 sa4;

    public static Int32 vn1;

    public static ff9.s_effectDataList[] w_effectDataList;

    public static Byte[] w_EvaCore;

    public static SPSEffect w_evaCoreSPS;

    public static Int32 w_evaCoreSPSCurrentSize;

    public static Int32 w_evaCoreSPSSpeedtSize;

    public static Vector3[] w_effectModelPos;

    public static Vector3[] w_effectModelRot;

    public static Vector3 w_effectMillPos;

    public static Vector3 w_effectMillRot;

    public static Vector3 w_effectTwisPos;

    public static Vector3 w_effectLastPos;

    public static Vector3 w_effectLastPos1;

    public static Vector3 w_effectTwisRot;

    public static Vector3 w_effectLast1Rot;

    public static Vector3 w_effectLast2Rot;

    public static Vector3 w_effectLast3Rot;

    public static Vector3[] w_effectLastRot;

    public static Vector3[] w_effectLastRotT;

    public static Int16[] w_effectLastDis;

    public static Int16[] w_effectLastDisT;

    public static SByte[] w_effectLastDisC;

    public static Vector3[] w_effectSpilRot;

    public static Boolean[] w_effectBlockAvail;

    public static Int32 w_effectMoveStock;

    public static Int32 w_effectMoveStockTrue;

    public static Int32 w_effectMoveStockHeight;

    public static Int32 w_effectMoveStockHeightTrue;

    public static Int32 effect16FrameCounter;

    public static ff9.sw_ImageSector[] w_fileImageSectorInfo;

    public static Int32[] w_fileImageTopSector;

    public static String[] w_fileImagename;

    public static String[] w_fileImagenameServer1;

    public static String[] w_fileImagenameServer4;

    public static Byte w_frameCDUse;

    public static Int32 w_frameResult;

    public static UInt16 w_frameScenePtr;

    public static Int32 w_frameLanguage;

    public static Byte w_framePhase;

    public static Int32 w_frameTime;

    public static SByte w_frameStatus;

    public static Byte w_frameWire;

    public static Boolean w_frameMenuon;

    public static Byte w_frameFog;

    public static Byte w_frameServer;

    public static Byte w_frameDisc;

    public static Boolean w_frameRain;

    public static Boolean w_frameCloud;

    public static Boolean w_frameEventEnable;

    public static Int32 w_frameCounter;

    public static Int32 w_frameCounterReady;

    public static UInt32 w_frameDebugLevel;

    public static Int16 w_frameShadowOTOffset;

    public static Byte w_frameIDPtr;

    public static Camera w_frameCameraPtr;

    public static Byte[] w_frameMessagePtr;

    public static EncountData[] w_frameBattleScenePtr;

    public static SByte w_frameChrMovement;

    public static Boolean w_frameEncountEnable;

    public static Boolean w_frameEncountMask;

    public static Int32[] w_frameScriptParam;

    public static UInt16 w_frameEventBattleProb;

    public static Int32 w_frameAutoid;

    public static Boolean w_frameMogChoice;

    public static Boolean w_frameLoc;

    public static Int32 updatetime;

    public static Int32 servicetime;

    public static Int32 eventtime;

    public static Int32 totaltime;

    public static Int32 service_movement;

    public static Int32 service_ezmenu;

    public static Int32 service_world;

    public static UInt32 w_framePacketPeak;

    public static UInt32 w_framePacketSize;

    public static UInt32 w_framePacketPer;

    public static SByte w_frameEffectTest;

    public static Int32 w_frameEffectSize;

    public static Boolean w_frameEncountTrue;

    public static Boolean w_frameInternalSwitchEnable;

    public static Boolean w_frameInitialize;

    public static Char w_frameHamari;

    public static Boolean w_frameMenuOpen;

    public static SByte w_frameError;

    public static Boolean w_frameDebugMode;

    public static Int16 w_frameDebugVRAM;

    public static UInt32 w_framePadMask;

    public static UInt32 w_frameLine;

    public static Int32 kframeEventStartLoop;

    public static WorldRainRenderer rainRenderer;

    public static Light[] w_light;

    public static Byte[] w_memoryTextureanim;

    public static Byte[] w_memorySPSData;

    private static Boolean w_movementService_Notified;

    private static readonly Single kFF9FogCharHI;

    private static readonly Single kFF9FogCharLOW;

    private static Int32 p1;

    private static Int32 p2;

    private static Int32 p3;

    private static Single WH1;

    private static Single WH1b;

    private static Single SH1;

    private static Single WH2;

    private static Single WH2b;

    private static Single SH2;

    private static Single cn1;

    private static Single cn2;

    private static Single cn3;

    private static Single cn4;

    private static Single cn5;

    private static Boolean forceUsingMobileInput;

    public static SByte w_musicProgress;

    public static Int32 w_musicRequestNoIdx;

    public static Int32 w_musicPlayNoIdx;

    public static Int32 w_musicPlayNo;

    public static Int32 w_musicFirstPlay;

    public static Byte[] w_musicSet;

    public static Int32 w_naviMode;

    public static Int32 w_naviModeOld;

    public static SByte w_naviMapno;

    public static SByte w_naviTitle;

    public static Byte w_naviTitleColor;

    public static Byte w_naviPointLoc;

    public static Boolean w_naviPointLocFlg;

    public static UInt32 w_naviFadeInTime;

    public static Boolean w_naviActive;

    public static Boolean w_naviCursolMove;

    public static Boolean w_naviCursolDraw;

    public static SByte w_naviLocationDraw;

    public static Boolean w_naviDrawItems;

    public static Boolean w_setNaviFadeInTime;

    public static readonly ff9.navipos[,] w_naviLocationPos;

    private static readonly SByte WorldTitleFadeInMode;

    private static readonly SByte WorldTitleFadeOutMode;

    private static readonly SByte WorldTitleCloseMode;

    private static SByte lastTitleDrawState;

    public static ff9.CVECTOR[] ggammawork;

    public static Byte[] w_nwpZDepth;

    public static ff9.sNWBPage[] w_nwbAreaPage;

    public static ff9.sNWBPage w_nwbAreaPagePtr;

    public static ff9.sNWBColor[] w_nwbColor;

    public static ff9.sNWBColor w_nwbColorCloud;

    public static Boolean w_nwbCache;

    public static Byte w_nwbPolyCheck;

    public static Boolean w_nwbTEST;

    private static Single angleTest;

    private static Single rayStartOffsetYFromSky;

    private static Single rayStartOffsetY;

    private static Single rayDistance;

    private static Single defaultHeight;

    private static Boolean initialized_w_NormalTable;

    private static ff9.SVECTOR[] _w_NormalTable;

    public static ff9.stextureProject w_textureProjWork;

    public static SByte[] w_texturePixelAnimCnt;

    public static SByte[] w_texturePixelAnimPat;

    public static Int16[] w_texturePixelScrollWork;

    public static Int16[] w_texturePaletScrollWork;

    public static UInt16[,] w_textureWork;

    public static UInt16[] w_textureWorkOffset;

    public static Byte[] w_textureWorkScroll;

    public static Boolean w_textureScrollFire;

    private static Vector3 volcanoPosition;

    public static Boolean w_weatherAutoChange;

    public static Int32 w_weatherMode;

    public static Int32 w_weatherModeOld;

    public static ff9.sw_weatherColor w_weatherColor;

    public static Int32 w_weatherColorCounter;

    public static Int32 w_weatherColorCounterSpeed;

    public static Single w_weatherDistanceEva;

    public static readonly Byte[] w_worldAreaZone;

    public static readonly Byte[] w_worldZoneFigure;

    public static readonly Byte[] w_worldZoneInfo;

    public static readonly Int32[] w_worldLocDistance;

    public static readonly Int32[] w_worldLocX;

    public static readonly Int32[] w_worldLocZ;

    public static readonly Byte[] w_worldLocSENum;

    public static readonly Byte[] w_worldLocSEFlg;

    public static Byte w_worldPlaneSENum;

    public static Byte w_worldPlaneSEFlg;

    public static ff9.sworldEncountSpecial[] w_worldEncountSpecial;

    public static ff9.sNWBBlockHeader w_worldSeaBlockPtr;

    private static Single w_cameraTriggerTime = 0f;
    private static Single w_cameraNotrotTime = 0f;

    public static HashSet<UInt16> w_friendlyBattles = new HashSet<UInt16>();
    public static HashSet<UInt16> w_ragtimeBattles = new HashSet<UInt16>()
    {
        627, // Green forest, Quizz result
        634, // Green forest + Mist, Quizz result
        753, // Brown forest, Quizz result
        755, // Black forest, Quizz result
        941, // Green forest + Mist, Quizz question
        942, // Green forest, Quizz question
        943, // Black forest, Quizz question
        944  // Brown forest, Quizz question
    };

    // World maps that consist only of cutscenes
    public static Boolean IsEventWorldMap => eventWorldMaps.Contains(FF9StateSystem.Common.FF9.wldMapNo);
    public static HashSet<Int16> eventWorldMaps = new HashSet<Int16>()
    {
        9001, // World Map/Event: Cargo Ship
        9004, // World Map/Event: Hilda Garde 1
        9006, // World Map/Event: Track Kuja
        9012, // World Map/Event: Chocobo Treasure
    };

    public struct VECTOR
    {
        public Int32 vx;

        public Int32 vy;

        public Int32 vz;

        public Int32 pad;
    }

    public struct SVECTOR
    {
        public SVECTOR(Int16 x, Int16 y, Int16 z, Int16 pad)
        {
            this.vx = x;
            this.vy = y;
            this.vz = z;
            this.pad = pad;
        }

        public Color ToColor()
        {
            return new Color(this.vx / 4096f, this.vy / 4096f, this.vz / 4096f);
        }

        public Color32 ToByteColor()
        {
            return new Color32((Byte)this.vx, (Byte)this.vy, (Byte)this.vz, Byte.MaxValue);
        }

        public Int16 vx;

        public Int16 vy;

        public Int16 vz;

        public Int16 pad;
    }

    public struct CVECTOR
    {
        public Byte r;

        public Byte g;

        public Byte b;

        public Byte cd;
    }

    public class s_cameraElement
    {
        public Byte down;

        public Byte up;

        public Byte fly;
    }

    public struct s_cameraPosstat
    {
        public Single cameraPers;

        public Single cameraDistance;

        public Single cameraHeight;

        public Single cameraCorrect;

        public Single aimHeight;
    }

    public class s_effectData
    {
        public Int32 x;
        public Int32 y;
        public Int32 z;

        public Int32 rx;
        public Int32 ry;
        public Int32 rz;

        public Int16 vx;
        public Int16 vy;
        public Int16 vz;

        public Int16 ax;
        public Int16 ay;
        public Int16 az;

        public SPSConst.WorldSPSEffect no;
        public Int16 size;
        public Int16 rnd;
        public Int16 temp;
        public Int32[] pad = new Int32[5];
    }

    public class s_effectDataList
    {
        public s_effectDataList()
        {
            this.effectData = new List<ff9.s_effectData>();
        }

        public List<ff9.s_effectData> effectData { get; private set; }
    }

    public struct sw_ImageSector
    {
        public Int32 start;

        public Int32 length;
    }

    public static class Pad
    {
        public static Boolean kPadLLeft => Input.GetKey(HonoInputManager.MemoriaKeyBindings[1]); // A
        public static Boolean kPadLRight => Input.GetKey(HonoInputManager.MemoriaKeyBindings[3]); // D
        public static Boolean kPadLUp => Input.GetKey(HonoInputManager.MemoriaKeyBindings[0]); // W
        public static Boolean kPadLDown => Input.GetKey(HonoInputManager.MemoriaKeyBindings[2]); // S
        public static Boolean kPadL1 => UIManager.Input.GetKey(Control.LeftBumper);
        public static Boolean kPadL2 => Input.GetKey(KeyCode.E);
        public static Boolean kPadRLeft => Input.GetKey(KeyCode.J);
        public static Boolean kPadRRight => Input.GetKey(KeyCode.L);
        public static Boolean kPadRUp => Input.GetKey(KeyCode.I);
        public static Boolean kPadRDown => Input.GetKey(KeyCode.K);
        public static Boolean kPadR1 => UIManager.Input.GetKey(Control.RightBumper);
        public static Boolean kPadR2 => Input.GetKey(KeyCode.O);
        public static Boolean kPadSelect => UIManager.Input.GetKey(Control.Select);
        public static Boolean kPadStart => UIManager.Input.GetKey(Control.Pause);
    }

    public class s_moveCHRControl : ICsvEntry
    {
        public Byte type;
        public Byte flg_gake;
        public Int16 speed_move;
        public Byte speed_rotation;
        public Byte speed_updown;
        public SByte speed_roll;
        public SByte speed_rollback;
        public Boolean flg_fly;
        public Boolean flg_upcam;
        public Byte fetchdist;
        public Byte music;
        public Byte se;
        public Boolean encount;
        public Int16 radius;
        public Boolean camrot;
        public Byte type_cam;
        public UInt16 pad2;
        public UInt32[] limit = new UInt32[2];

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            type = CsvParser.Byte(raw[index++]);
            flg_gake = CsvParser.Byte(raw[index++]);
            speed_move = CsvParser.Int16(raw[index++]);
            speed_rotation = CsvParser.Byte(raw[index++]);
            speed_updown = CsvParser.Byte(raw[index++]);
            speed_roll = CsvParser.SByte(raw[index++]);
            speed_rollback = CsvParser.SByte(raw[index++]);
            flg_fly = CsvParser.Boolean(raw[index++]);
            flg_upcam = CsvParser.Boolean(raw[index++]);
            fetchdist = CsvParser.Byte(raw[index++]);
            music = CsvParser.Byte(raw[index++]);
            se = CsvParser.Byte(raw[index++]);
            encount = CsvParser.Boolean(raw[index++]);
            radius = CsvParser.Int16(raw[index++]);
            camrot = CsvParser.Boolean(raw[index++]);
            type_cam = CsvParser.Byte(raw[index++]);
            pad2 = CsvParser.UInt16(raw[index++]);
            limit[0] = CsvParser.UInt32(raw[index++]);
            limit[1] = CsvParser.UInt32(raw[index++]);
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            writer.Byte(type);
            writer.Byte(flg_gake);
            writer.Int16(speed_move);
            writer.Byte(speed_rotation);
            writer.Byte(speed_updown);
            writer.SByte(speed_roll);
            writer.SByte(speed_rollback);
            writer.Boolean(flg_fly);
            writer.Boolean(flg_upcam);
            writer.Byte(fetchdist);
            writer.Byte(music);
            writer.Byte(se);
            writer.Boolean(encount);
            writer.Int16(radius);
            writer.Boolean(camrot);
            writer.Byte(type_cam);
            writer.UInt16(pad2);
            writer.UInt32(limit[0]);
            writer.UInt32(limit[1]);
        }
    }

    public class s_moveCHRStatus
    {
        public Byte slice_type;

        public Byte shadow_size;

        public Byte shadow_amp;

        public Byte flg_fly;

        public Byte control;

        public Byte cache;

        public Byte shadow_bone;

        public SByte shadow_offset;

        public Int32 id;

        public Single slice_height;

        public Single ground_height;
    }

    public class s_moveCHRCache
    {
        public Int32 Number;

        public Byte[] OnObject = new Byte[10];

        public Int32[] TriangleIndices = new Int32[10];

        public Int32[] WalkMeshIndices = new Int32[10];

        public WMBlock[] Blocks = new WMBlock[10];
    }

    public class s_musicID
    {
        public Int32 attr;

        public Int32 figure;

        public Int32[] id = new Int32[3];
    }

    public class s_musicSE
    {
        public Int32 attr;

        public Int32 id;
    }

    public struct navipos
    {
        public Int16 vx;

        public Int16 vy;

        public Int32 tx;

        public Int32 ty;
    }

    public struct sNWBColor
    {
        public ff9.CVECTOR w_nwbFarColor;

        public UInt16 w_nwbFogValG;

        public UInt16 w_nwbFogValT;
    }

    public struct sNWBPage
    {
        public UInt16 tpage4;

        public UInt16 clut4;

        public Byte tex_u4;

        public Byte tex_v4;

        public UInt16 pad;

        public UInt16 tpage8;

        public UInt16 clut8;
    }

    public class sNWBCellHeader
    {
        public Byte vtxno;

        public Byte areaid;

        public UInt16 surno;

        public UInt32 offsetvertex;

        public UInt32 offsetsurface;

        public Byte[] checkpoint = new Byte[20];

        public Byte checkfig;

        public Byte[] special = new Byte[3];
    }

    public class sNWBBlockHeader
    {
        public SByte cellno;

        public SByte timno;

        public SByte areaid;

        public Byte special;

        public UInt32 offsettexture;

        public UInt32[] offsetcell = new UInt32[1];

        public ff9.sNWBCellHeader[] cellHeader;
    }

    public struct stexturePixelScroll
    {
        public UInt16 posx;

        public UInt16 posy;

        public UInt16 width;

        public UInt16 hight;

        public UInt16 speed;
    }

    public struct stexturePixelAnime
    {
        public UInt16 posx;

        public UInt16 posy;

        public UInt16 width;

        public UInt16 height;

        public UInt16 speed;
    }

    public struct stexturePaletScroll
    {
        public UInt16 posx;

        public UInt16 posy;

        public UInt16 offset;

        public UInt16 length;

        public UInt16 speed;
    }

    public class stextureProject
    {
        public ff9.stexturePixelScroll[] texturePixelScroll = new ff9.stexturePixelScroll[8];

        public ff9.stexturePixelAnime[] texturePixelAnime = new ff9.stexturePixelAnime[4];

        public ff9.stexturePaletScroll[] texturePaletScroll = new ff9.stexturePaletScroll[18];
    }

    public class sw_weatherColorElement : ICsvEntry
    {
        public ff9.SVECTOR light0 = new ff9.SVECTOR();
        public ff9.SVECTOR light1 = new ff9.SVECTOR();
        public ff9.SVECTOR light2 = new ff9.SVECTOR();
        public ff9.SVECTOR light0c = new ff9.SVECTOR();
        public ff9.SVECTOR ambient = new ff9.SVECTOR();
        public ff9.SVECTOR ambientcl = new ff9.SVECTOR();
        public UInt16 goffsetup;
        public UInt16 toffsetup;
        public ff9.SVECTOR fogUP = new ff9.SVECTOR();
        public UInt16 goffsetdw;
        public UInt16 toffsetdw;
        public ff9.SVECTOR fogDW = new ff9.SVECTOR();
        public UInt16 goffsetcl;
        public UInt16 toffsetcl;
        public ff9.SVECTOR fogCL = new ff9.SVECTOR();
        public ff9.SVECTOR chrBIAS = new ff9.SVECTOR();
        public UInt16 fogAMP;
        public Single offsetX;
        public Single scaleY;
        public Color skyBgColor = new Color();
        public Color skyFogColor = new Color();
        public Single lightColorFactor;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            light0.vx = CsvParser.Int16(raw[index++]);
            light0.vy = CsvParser.Int16(raw[index++]);
            light0.vz = CsvParser.Int16(raw[index++]);
            light1.vx = CsvParser.Int16(raw[index++]);
            light1.vy = CsvParser.Int16(raw[index++]);
            light1.vz = CsvParser.Int16(raw[index++]);
            light2.vx = CsvParser.Int16(raw[index++]);
            light2.vy = CsvParser.Int16(raw[index++]);
            light2.vz = CsvParser.Int16(raw[index++]);
            light0c.vx = CsvParser.Int16(raw[index++]);
            light0c.vy = CsvParser.Int16(raw[index++]);
            light0c.vz = CsvParser.Int16(raw[index++]);
            ambient.vx = CsvParser.Int16(raw[index++]);
            ambient.vy = CsvParser.Int16(raw[index++]);
            ambient.vz = CsvParser.Int16(raw[index++]);
            ambientcl.vx = CsvParser.Int16(raw[index++]);
            ambientcl.vy = CsvParser.Int16(raw[index++]);
            ambientcl.vz = CsvParser.Int16(raw[index++]);
            goffsetup = CsvParser.UInt16(raw[index++]);
            toffsetup = CsvParser.UInt16(raw[index++]);
            fogUP.vx = CsvParser.Int16(raw[index++]);
            fogUP.vy = CsvParser.Int16(raw[index++]);
            fogUP.vz = CsvParser.Int16(raw[index++]);
            goffsetdw = CsvParser.UInt16(raw[index++]);
            toffsetdw = CsvParser.UInt16(raw[index++]);
            fogDW.vx = CsvParser.Int16(raw[index++]);
            fogDW.vy = CsvParser.Int16(raw[index++]);
            fogDW.vz = CsvParser.Int16(raw[index++]);
            goffsetcl = CsvParser.UInt16(raw[index++]);
            toffsetcl = CsvParser.UInt16(raw[index++]);
            fogCL.vx = CsvParser.Int16(raw[index++]);
            fogCL.vy = CsvParser.Int16(raw[index++]);
            fogCL.vz = CsvParser.Int16(raw[index++]);
            chrBIAS.vx = CsvParser.Int16(raw[index++]);
            chrBIAS.vy = CsvParser.Int16(raw[index++]);
            chrBIAS.vz = CsvParser.Int16(raw[index++]);
            fogAMP = CsvParser.UInt16(raw[index++]);
            offsetX = CsvParser.Single(raw[index++]);
            scaleY = CsvParser.Single(raw[index++]);
            skyBgColor.r = CsvParser.Single(raw[index++]);
            skyBgColor.g = CsvParser.Single(raw[index++]);
            skyBgColor.b = CsvParser.Single(raw[index++]);
            skyBgColor.a = CsvParser.Single(raw[index++]);
            skyFogColor.r = CsvParser.Single(raw[index++]);
            skyFogColor.g = CsvParser.Single(raw[index++]);
            skyFogColor.b = CsvParser.Single(raw[index++]);
            skyFogColor.a = CsvParser.Single(raw[index++]);
            lightColorFactor = CsvParser.Single(raw[index++]);
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            writer.Int16(light0.vx);
            writer.Int16(light0.vy);
            writer.Int16(light0.vz);
            writer.Int16(light1.vx);
            writer.Int16(light1.vy);
            writer.Int16(light1.vz);
            writer.Int16(light2.vx);
            writer.Int16(light2.vy);
            writer.Int16(light2.vz);
            writer.Int16(light0c.vx);
            writer.Int16(light0c.vy);
            writer.Int16(light0c.vz);
            writer.Int16(ambient.vx);
            writer.Int16(ambient.vy);
            writer.Int16(ambient.vz);
            writer.Int16(ambientcl.vx);
            writer.Int16(ambientcl.vy);
            writer.Int16(ambientcl.vz);
            writer.UInt16(goffsetup);
            writer.UInt16(toffsetup);
            writer.Int16(fogUP.vx);
            writer.Int16(fogUP.vy);
            writer.Int16(fogUP.vz);
            writer.UInt16(goffsetdw);
            writer.UInt16(toffsetdw);
            writer.Int16(fogDW.vx);
            writer.Int16(fogDW.vy);
            writer.Int16(fogDW.vz);
            writer.UInt16(goffsetcl);
            writer.UInt16(toffsetcl);
            writer.Int16(fogCL.vx);
            writer.Int16(fogCL.vy);
            writer.Int16(fogCL.vz);
            writer.Int16(chrBIAS.vx);
            writer.Int16(chrBIAS.vy);
            writer.Int16(chrBIAS.vz);
            writer.UInt16(fogAMP);
            writer.Single(offsetX);
            writer.Single(scaleY);
            writer.Single(skyBgColor.r);
            writer.Single(skyBgColor.g);
            writer.Single(skyBgColor.b);
            writer.Single(skyBgColor.a);
            writer.Single(skyFogColor.r);
            writer.Single(skyFogColor.g);
            writer.Single(skyFogColor.b);
            writer.Single(skyFogColor.a);
            writer.Single(lightColorFactor);
        }
    }

    public class sw_weatherColor
    {
        public sw_weatherColor()
        {
            for (Int32 i = 0; i < this.Color.Length; i++)
                this.Color[i] = new ff9.sw_weatherColorElement();
            this.Color[0].light0 = new ff9.SVECTOR(100, 100, 100, 0);
            this.Color[0].light1 = new ff9.SVECTOR(724, 724, 724, 0);
            this.Color[0].light2 = new ff9.SVECTOR(1648, 1548, 1348, 0);
            this.Color[0].light0c = new ff9.SVECTOR(256, 768, 1696, 0);
            this.Color[0].ambient = new ff9.SVECTOR(26, 26, 26, 0);
            this.Color[0].ambientcl = new ff9.SVECTOR(45, 49, 64, 0);
            this.Color[0].goffsetup = 12000;
            this.Color[0].toffsetup = 12000;
            this.Color[0].fogUP = new ff9.SVECTOR(205, 217, 247, 0);
            this.Color[0].goffsetdw = 11000;
            this.Color[0].toffsetdw = 11000;
            this.Color[0].fogDW = new ff9.SVECTOR(205, 217, 247, 0);
            this.Color[0].goffsetcl = 12000;
            this.Color[0].toffsetcl = 0;
            this.Color[0].fogCL = new ff9.SVECTOR(205, 217, 247, 0);
            this.Color[0].chrBIAS = new ff9.SVECTOR(3596, 3596, 3596, 0);
            this.Color[0].fogAMP = 4096;
            this.Color[0].skyBgColor = new Color32(124, 179, 217, Byte.MaxValue);
            this.Color[0].skyFogColor = new Color32(Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, Byte.MaxValue);
            this.Color[0].lightColorFactor = 1f;
            this.Color[1].light0 = new ff9.SVECTOR(1012, 1012, 1012, 0);
            this.Color[1].light1 = new ff9.SVECTOR(724, 724, 724, 0);
            this.Color[1].light2 = new ff9.SVECTOR(1648, 1548, 1348, 0);
            this.Color[1].light0c = new ff9.SVECTOR(450, 1000, 1900, 0);
            this.Color[1].ambient = new ff9.SVECTOR(26, 26, 26, 0);
            this.Color[1].ambientcl = new ff9.SVECTOR(48, 48, 48, 0);
            this.Color[1].goffsetup = 10000;
            this.Color[1].toffsetup = 10000;
            this.Color[1].fogUP = new ff9.SVECTOR(151, 170, 255, 0);
            this.Color[1].goffsetdw = 11000;
            this.Color[1].toffsetdw = 11000;
            this.Color[1].fogDW = new ff9.SVECTOR(184, 212, 255, 0);
            this.Color[1].goffsetcl = 9000;
            this.Color[1].toffsetcl = 0;
            this.Color[1].fogCL = new ff9.SVECTOR(184, 212, 255, 0);
            this.Color[1].chrBIAS = new ff9.SVECTOR(3596, 3596, 3596, 0);
            this.Color[1].fogAMP = 4096;
            this.Color[1].skyBgColor = new Color32(124, 179, 217, Byte.MaxValue);
            this.Color[1].skyFogColor = new Color32(Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, Byte.MaxValue);
            this.Color[1].lightColorFactor = 1f;
            this.Color[2].light0 = new ff9.SVECTOR(1012, 1012, 1012, 0);
            this.Color[2].light1 = new ff9.SVECTOR(724, 724, 724, 0);
            this.Color[2].light2 = new ff9.SVECTOR(1648, 1548, 1348, 0);
            this.Color[2].light0c = new ff9.SVECTOR(256, 768, 1596, 0);
            this.Color[2].ambient = new ff9.SVECTOR(200, 200, 200, 0);
            this.Color[2].ambientcl = new ff9.SVECTOR(45, 49, 64, 0);
            this.Color[2].goffsetup = 17000;
            this.Color[2].toffsetup = 16000;
            this.Color[2].fogUP = new ff9.SVECTOR(207, 207, 247, 0);
            this.Color[2].goffsetdw = 24008;
            this.Color[2].toffsetdw = 32000;
            this.Color[2].fogDW = new ff9.SVECTOR(205, 217, 247, 0);
            this.Color[2].goffsetcl = 12000;
            this.Color[2].toffsetcl = 0;
            this.Color[2].fogCL = new ff9.SVECTOR(205, 217, 247, 0);
            this.Color[2].chrBIAS = new ff9.SVECTOR(3596, 3596, 3596, 0);
            this.Color[2].fogAMP = 4096;
            this.Color[2].offsetX = 0.15f;
            this.Color[2].scaleY = 0.07f;
            this.Color[2].skyBgColor = new Color32(124, 179, 217, Byte.MaxValue);
            this.Color[2].skyFogColor = new Color32(Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, Byte.MaxValue);
            this.Color[2].lightColorFactor = 1f;
            this.Color[3].light0 = new ff9.SVECTOR(200, 200, 300, 0);
            this.Color[3].light1 = new ff9.SVECTOR(0, 0, 0, 0);
            this.Color[3].light2 = new ff9.SVECTOR(748, 748, 976, 0);
            this.Color[3].light0c = new ff9.SVECTOR(448, 532, 1152, 0);
            this.Color[3].ambient = new ff9.SVECTOR(200, 200, 200, 0);
            this.Color[3].ambientcl = new ff9.SVECTOR(48, 48, 48, 0);
            this.Color[3].goffsetup = 23104;
            this.Color[3].toffsetup = 27592;
            this.Color[3].fogUP = new ff9.SVECTOR(136, 148, 175, 0);
            this.Color[3].goffsetdw = 28556;
            this.Color[3].toffsetdw = 29536;
            this.Color[3].fogDW = new ff9.SVECTOR(136, 148, 175, 0);
            this.Color[3].goffsetcl = 17000;
            this.Color[3].toffsetcl = 13000;
            this.Color[3].fogCL = new ff9.SVECTOR(136, 148, 175, 0);
            this.Color[3].chrBIAS = new ff9.SVECTOR(2296, 2396, 2396, 0);
            this.Color[3].fogAMP = 1296;
            this.Color[3].offsetX = 0.15f;
            this.Color[3].scaleY = 0.07f;
            this.Color[3].skyBgColor = new Color32(124, 179, 217, Byte.MaxValue);
            this.Color[3].skyFogColor = new Color32(Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, Byte.MaxValue);
            this.Color[3].lightColorFactor = 1f;
            this.Color[4].light0 = new ff9.SVECTOR(1168, 868, 468, 0);
            this.Color[4].light1 = new ff9.SVECTOR(1112, 812, 512, 0);
            this.Color[4].light2 = new ff9.SVECTOR(1736, 1580, 1280, 0);
            this.Color[4].light0c = new ff9.SVECTOR(2100, 2000, 800, 0);
            this.Color[4].ambient = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[4].ambientcl = new ff9.SVECTOR(38, 12, 12, 0);
            this.Color[4].goffsetup = 9312;
            this.Color[4].toffsetup = 9280;
            this.Color[4].fogUP = new ff9.SVECTOR(230, 120, 40, 0);
            this.Color[4].goffsetdw = 10000;
            this.Color[4].toffsetdw = 12000;
            this.Color[4].fogDW = new ff9.SVECTOR(230, 148, 56, 0);
            this.Color[4].goffsetcl = 10288;
            this.Color[4].toffsetcl = 0;
            this.Color[4].fogCL = new ff9.SVECTOR(230, 148, 56, 0);
            this.Color[4].chrBIAS = new ff9.SVECTOR(3896, 3196, 2896, 0);
            this.Color[4].fogAMP = 4096;
            this.Color[4].skyBgColor = new Color32(187, 149, 100, Byte.MaxValue);
            this.Color[4].lightColorFactor = 0.68f;
            this.Color[5].light0 = new ff9.SVECTOR(1168, 868, 468, 0);
            this.Color[5].light1 = new ff9.SVECTOR(1112, 812, 512, 0);
            this.Color[5].light2 = new ff9.SVECTOR(1736, 1580, 1280, 0);
            this.Color[5].light0c = new ff9.SVECTOR(2100, 2000, 800, 0);
            this.Color[5].ambient = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[5].ambientcl = new ff9.SVECTOR(38, 12, 12, 0);
            this.Color[5].goffsetup = 12000;
            this.Color[5].toffsetup = 12000;
            this.Color[5].fogUP = new ff9.SVECTOR(230, 120, 40, 0);
            this.Color[5].goffsetdw = 10000;
            this.Color[5].toffsetdw = 12000;
            this.Color[5].fogDW = new ff9.SVECTOR(230, 148, 56, 0);
            this.Color[5].goffsetcl = 10824;
            this.Color[5].toffsetcl = 0;
            this.Color[5].fogCL = new ff9.SVECTOR(230, 148, 56, 0);
            this.Color[5].chrBIAS = new ff9.SVECTOR(3896, 3196, 2896, 0);
            this.Color[5].fogAMP = 4096;
            this.Color[5].skyBgColor = new Color32(187, 149, 100, Byte.MaxValue);
            this.Color[5].skyFogColor = new Color32(Byte.MaxValue, 197, 106, Byte.MaxValue);
            this.Color[5].lightColorFactor = 0.68f;
            this.Color[6].light0 = new ff9.SVECTOR(1568, 868, 468, 0);
            this.Color[6].light1 = new ff9.SVECTOR(1112, 812, 512, 0);
            this.Color[6].light2 = new ff9.SVECTOR(1737, 1580, 1280, 0);
            this.Color[6].light0c = new ff9.SVECTOR(2700, 1600, 800, 0);
            this.Color[6].ambient = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[6].ambientcl = new ff9.SVECTOR(0, 0, 0, 0);
            this.Color[6].goffsetup = 16000;
            this.Color[6].toffsetup = 16000;
            this.Color[6].fogUP = new ff9.SVECTOR(245, 181, 80, 0);
            this.Color[6].goffsetdw = 24864;
            this.Color[6].toffsetdw = 26240;
            this.Color[6].fogDW = new ff9.SVECTOR(245, 181, 90, 0);
            this.Color[6].goffsetcl = 12288;
            this.Color[6].toffsetcl = 8000;
            this.Color[6].fogCL = new ff9.SVECTOR(245, 181, 90, 0);
            this.Color[6].chrBIAS = new ff9.SVECTOR(3896, 3196, 2896, 0);
            this.Color[6].fogAMP = 4096;
            this.Color[6].offsetX = -0.02f;
            this.Color[6].scaleY = 2.76f;
            this.Color[6].skyBgColor = new Color32(187, 149, 100, Byte.MaxValue);
            this.Color[6].skyFogColor = new Color32(Byte.MaxValue, 197, 106, Byte.MaxValue);
            this.Color[6].lightColorFactor = 0.68f;
            this.Color[7].light0 = new ff9.SVECTOR(1500, 868, 468, 0);
            this.Color[7].light1 = new ff9.SVECTOR(1112, 812, 512, 0);
            this.Color[7].light2 = new ff9.SVECTOR(1737, 1580, 1280, 0);
            this.Color[7].light0c = new ff9.SVECTOR(2000, 650, 0, 0);
            this.Color[7].ambient = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[7].ambientcl = new ff9.SVECTOR(0, 0, 0, 0);
            this.Color[7].goffsetup = 25120;
            this.Color[7].toffsetup = 31392;
            this.Color[7].fogUP = new ff9.SVECTOR(245, 181, 90, 0);
            this.Color[7].goffsetdw = 28000;
            this.Color[7].toffsetdw = 29000;
            this.Color[7].fogDW = new ff9.SVECTOR(245, 181, 90, 0);
            this.Color[7].goffsetcl = 15000;
            this.Color[7].toffsetcl = 15000;
            this.Color[7].fogCL = new ff9.SVECTOR(245, 181, 90, 0);
            this.Color[7].chrBIAS = new ff9.SVECTOR(3096, 2596, 2096, 0);
            this.Color[7].fogAMP = 1812;
            this.Color[7].offsetX = -0.02f;
            this.Color[7].scaleY = 2.76f;
            this.Color[7].skyBgColor = new Color32(187, 149, 100, Byte.MaxValue);
            this.Color[7].skyFogColor = new Color32(Byte.MaxValue, 197, 106, Byte.MaxValue);
            this.Color[7].lightColorFactor = 0.68f;
            this.Color[8].light0 = new ff9.SVECTOR(210, 210, 332, 0);
            this.Color[8].light1 = new ff9.SVECTOR(500, 447, 330, 0);
            this.Color[8].light2 = new ff9.SVECTOR(610, 500, 500, 0);
            this.Color[8].light0c = new ff9.SVECTOR(0, 320, 400, 0);
            this.Color[8].ambient = new ff9.SVECTOR(0, 20, 26, 0);
            this.Color[8].ambientcl = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[8].goffsetup = 10432;
            this.Color[8].toffsetup = 10592;
            this.Color[8].fogUP = new ff9.SVECTOR(0, 64, 70, 0);
            this.Color[8].goffsetdw = 11302;
            this.Color[8].toffsetdw = 9422;
            this.Color[8].fogDW = new ff9.SVECTOR(20, 46, 50, 0);
            this.Color[8].goffsetcl = 18000;
            this.Color[8].toffsetcl = 13000;
            this.Color[8].fogCL = new ff9.SVECTOR(20, 46, 50, 0);
            this.Color[8].chrBIAS = new ff9.SVECTOR(1496, 1796, 2196, 0);
            this.Color[8].fogAMP = 1296;
            this.Color[8].skyBgColor = new Color32(54, 131, 155, Byte.MaxValue);
            this.Color[8].skyFogColor = new Color32(116, 141, 200, Byte.MaxValue);
            this.Color[8].lightColorFactor = 2.25f;
            this.Color[9].light0 = new ff9.SVECTOR(210, 210, 332, 0);
            this.Color[9].light1 = new ff9.SVECTOR(500, 447, 330, 0);
            this.Color[9].light2 = new ff9.SVECTOR(610, 500, 500, 0);
            this.Color[9].light0c = new ff9.SVECTOR(0, 0, 0, 0);
            this.Color[9].ambient = new ff9.SVECTOR(0, 20, 26, 0);
            this.Color[9].ambientcl = new ff9.SVECTOR(0, 0, 0, 0);
            this.Color[9].goffsetup = 12144;
            this.Color[9].toffsetup = 10208;
            this.Color[9].fogUP = new ff9.SVECTOR(20, 46, 50, 0);
            this.Color[9].goffsetdw = 11406;
            this.Color[9].toffsetdw = 11328;
            this.Color[9].fogDW = new ff9.SVECTOR(20, 46, 50, 0);
            this.Color[9].goffsetcl = 29000;
            this.Color[9].toffsetcl = 14000;
            this.Color[9].fogCL = new ff9.SVECTOR(20, 46, 50, 0);
            this.Color[9].chrBIAS = new ff9.SVECTOR(1496, 1796, 2196, 0);
            this.Color[9].fogAMP = 1296;
            this.Color[9].skyBgColor = new Color32(54, 131, 155, Byte.MaxValue);
            this.Color[9].skyFogColor = new Color32(116, 141, 200, Byte.MaxValue);
            this.Color[9].lightColorFactor = 2.25f;
            this.Color[10].light0 = new ff9.SVECTOR(210, 210, 332, 0);
            this.Color[10].light1 = new ff9.SVECTOR(500, 447, 330, 0);
            this.Color[10].light2 = new ff9.SVECTOR(610, 500, 500, 0);
            this.Color[10].light0c = new ff9.SVECTOR(0, 320, 400, 0);
            this.Color[10].ambient = new ff9.SVECTOR(150, 150, 150, 0);
            this.Color[10].ambientcl = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[10].goffsetup = 15056;
            this.Color[10].toffsetup = 15672;
            this.Color[10].fogUP = new ff9.SVECTOR(80, 106, 130, 0);
            this.Color[10].goffsetdw = 27040;
            this.Color[10].toffsetdw = 30656;
            this.Color[10].fogDW = new ff9.SVECTOR(80, 106, 130, 0);
            this.Color[10].goffsetcl = 11970;
            this.Color[10].toffsetcl = 7000;
            this.Color[10].fogCL = new ff9.SVECTOR(80, 106, 130, 0);
            this.Color[10].chrBIAS = new ff9.SVECTOR(1496, 1796, 2196, 0);
            this.Color[10].fogAMP = 1296;
            this.Color[10].offsetX = -0.49f;
            this.Color[10].scaleY = 0.12f;
            this.Color[10].skyBgColor = new Color32(54, 131, 155, Byte.MaxValue);
            this.Color[10].skyFogColor = new Color32(116, 141, 200, Byte.MaxValue);
            this.Color[10].lightColorFactor = 2.25f;
            this.Color[11].light0 = new ff9.SVECTOR(410, 510, 632, 0);
            this.Color[11].light1 = new ff9.SVECTOR(1088, 1580, 1280, 0);
            this.Color[11].light2 = new ff9.SVECTOR(500, 500, 400, 0);
            this.Color[11].light0c = new ff9.SVECTOR(0, 0, 0, 0);
            this.Color[11].ambient = new ff9.SVECTOR(150, 150, 150, 0);
            this.Color[11].ambientcl = new ff9.SVECTOR(40, 48, 36, 0);
            this.Color[11].goffsetup = 29872;
            this.Color[11].toffsetup = 35872;
            this.Color[11].fogUP = new ff9.SVECTOR(80, 106, 130, 0);
            this.Color[11].goffsetdw = 27114;
            this.Color[11].toffsetdw = 34928;
            this.Color[11].fogDW = new ff9.SVECTOR(80, 106, 130, 0);
            this.Color[11].goffsetcl = 21753;
            this.Color[11].toffsetcl = 17312;
            this.Color[11].fogCL = new ff9.SVECTOR(80, 106, 130, 0);
            this.Color[11].chrBIAS = new ff9.SVECTOR(1396, 1596, 1696, 0);
            this.Color[11].fogAMP = 1296;
            this.Color[11].offsetX = -0.49f;
            this.Color[11].scaleY = 0.12f;
            this.Color[11].skyBgColor = new Color32(54, 131, 155, Byte.MaxValue);
            this.Color[11].skyFogColor = new Color32(116, 141, 200, Byte.MaxValue);
            this.Color[11].lightColorFactor = 2.25f;
            this.Color[12].light0 = new ff9.SVECTOR(1024, 1024, 1024, 0);
            this.Color[12].light1 = new ff9.SVECTOR(768, 1280, 2560, 0);
            this.Color[12].light2 = new ff9.SVECTOR(1280, 1280, 3072, 0);
            this.Color[12].light0c = new ff9.SVECTOR(0, 0, 512, 0);
            this.Color[12].ambient = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[12].ambientcl = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[12].goffsetup = 9568;
            this.Color[12].toffsetup = 11776;
            this.Color[12].fogUP = new ff9.SVECTOR(16, 16, 64, 0);
            this.Color[12].goffsetdw = 7200;
            this.Color[12].toffsetdw = 12352;
            this.Color[12].fogDW = new ff9.SVECTOR(16, 20, 96, 0);
            this.Color[12].goffsetcl = 9000;
            this.Color[12].toffsetcl = 0;
            this.Color[12].fogCL = new ff9.SVECTOR(20, 28, 64, 0);
            this.Color[12].chrBIAS = new ff9.SVECTOR(2496, 2696, 2896, 0);
            this.Color[12].fogAMP = 4096;
            this.Color[12].skyBgColor = new Color32(111, 119, 191, Byte.MaxValue);
            this.Color[12].skyFogColor = new Color32(127, 107, 211, Byte.MaxValue);
            this.Color[12].lightColorFactor = 1f;
            this.Color[13].light0 = new ff9.SVECTOR(1024, 1024, 1024, 0);
            this.Color[13].light1 = new ff9.SVECTOR(768, 1280, 2560, 0);
            this.Color[13].light2 = new ff9.SVECTOR(1280, 1280, 3072, 0);
            this.Color[13].light0c = new ff9.SVECTOR(0, 0, 512, 0);
            this.Color[13].ambient = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[13].ambientcl = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[13].goffsetup = 9600;
            this.Color[13].toffsetup = 9280;
            this.Color[13].fogUP = new ff9.SVECTOR(16, 28, 80, 0);
            this.Color[13].goffsetdw = 7552;
            this.Color[13].toffsetdw = 12352;
            this.Color[13].fogDW = new ff9.SVECTOR(28, 44, 92, 0);
            this.Color[13].goffsetcl = 4384;
            this.Color[13].toffsetcl = 0;
            this.Color[13].fogCL = new ff9.SVECTOR(23, 43, 95, 0);
            this.Color[13].chrBIAS = new ff9.SVECTOR(2496, 2696, 2896, 0);
            this.Color[13].fogAMP = 4096;
            this.Color[13].skyBgColor = new Color32(111, 119, 191, Byte.MaxValue);
            this.Color[13].skyFogColor = new Color32(127, 107, 211, Byte.MaxValue);
            this.Color[13].lightColorFactor = 1f;
            this.Color[14].light0 = new ff9.SVECTOR(400, 472, 416, 0);
            this.Color[14].light1 = new ff9.SVECTOR(1124, 1124, 1124, 0);
            this.Color[14].light2 = new ff9.SVECTOR(1248, 1248, 1248, 0);
            this.Color[14].light0c = new ff9.SVECTOR(464, 548, 1236, 0);
            this.Color[14].ambient = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[14].ambientcl = new ff9.SVECTOR(0, 0, 0, 0);
            this.Color[14].goffsetup = 12592;
            this.Color[14].toffsetup = 12000;
            this.Color[14].fogUP = new ff9.SVECTOR(103, 91, 155, 0);
            this.Color[14].goffsetdw = 25600;
            this.Color[14].toffsetdw = 32608;
            this.Color[14].fogDW = new ff9.SVECTOR(103, 91, 155, 0);
            this.Color[14].goffsetcl = 9804;
            this.Color[14].toffsetcl = 5864;
            this.Color[14].fogCL = new ff9.SVECTOR(102, 90, 154, 0);
            this.Color[14].chrBIAS = new ff9.SVECTOR(2496, 2696, 2896, 0);
            this.Color[14].fogAMP = 4096;
            this.Color[14].skyBgColor = new Color32(111, 119, 191, Byte.MaxValue);
            this.Color[14].skyFogColor = new Color32(127, 107, 211, Byte.MaxValue);
            this.Color[14].lightColorFactor = 1f;
            this.Color[15].light0 = new ff9.SVECTOR(400, 472, 416, 0);
            this.Color[15].light1 = new ff9.SVECTOR(1124, 1124, 1124, 0);
            this.Color[15].light2 = new ff9.SVECTOR(1248, 1248, 1248, 0);
            this.Color[15].light0c = new ff9.SVECTOR(768, 448, 1536, 0);
            this.Color[15].ambient = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[15].ambientcl = new ff9.SVECTOR(8, 8, 8, 0);
            this.Color[15].goffsetup = 28106;
            this.Color[15].toffsetup = 25793;
            this.Color[15].fogUP = new ff9.SVECTOR(101, 90, 146, 0);
            this.Color[15].goffsetdw = 31712;
            this.Color[15].toffsetdw = 31904;
            this.Color[15].fogDW = new ff9.SVECTOR(102, 90, 154, 0);
            this.Color[15].goffsetcl = 10304;
            this.Color[15].toffsetcl = 4864;
            this.Color[15].fogCL = new ff9.SVECTOR(102, 90, 154, 0);
            this.Color[15].chrBIAS = new ff9.SVECTOR(2896, 2796, 3196, 0);
            this.Color[15].fogAMP = 1296;
            this.Color[15].skyBgColor = new Color32(111, 119, 191, Byte.MaxValue);
            this.Color[15].skyFogColor = new Color32(127, 107, 211, Byte.MaxValue);
            this.Color[15].lightColorFactor = 1f;
            this.Color[16].light0 = new ff9.SVECTOR(200, 200, 300, 64);
            this.Color[16].light1 = new ff9.SVECTOR(0, 0, 0, 4);
            this.Color[16].light2 = new ff9.SVECTOR(748, 748, 976, 448);
            this.Color[16].light0c = new ff9.SVECTOR(448, 532, 1152, 704);
            this.Color[16].ambient = new ff9.SVECTOR(30, 30, 31, 256);
            this.Color[16].ambientcl = new ff9.SVECTOR(48, 48, 48, 32);
            this.Color[16].goffsetup = 23104;
            this.Color[16].toffsetup = 27592;
            this.Color[16].fogUP = new ff9.SVECTOR(136, 148, 175, 96);
            this.Color[16].goffsetdw = 28556;
            this.Color[16].toffsetdw = 29536;
            this.Color[16].fogDW = new ff9.SVECTOR(136, 148, 175, 864);
            this.Color[16].goffsetcl = 17000;
            this.Color[16].toffsetcl = 13000;
            this.Color[16].fogCL = new ff9.SVECTOR(136, 148, 175, 384);
            this.Color[16].chrBIAS = new ff9.SVECTOR(2296, 2396, 2396, 736);
            this.Color[16].fogAMP = 1296;
            this.Color[17].light0 = new ff9.SVECTOR(200, 200, 300, 704);
            this.Color[17].light1 = new ff9.SVECTOR(0, 0, 0, 8);
            this.Color[17].light2 = new ff9.SVECTOR(748, 748, 976, 32);
            this.Color[17].light0c = new ff9.SVECTOR(448, 532, 1152, 0);
            this.Color[17].ambient = new ff9.SVECTOR(30, 30, 31, 247);
            this.Color[17].ambientcl = new ff9.SVECTOR(48, 48, 48, 272);
            this.Color[17].goffsetup = 23104;
            this.Color[17].toffsetup = 27592;
            this.Color[17].fogUP = new ff9.SVECTOR(136, 148, 175, 241);
            this.Color[17].goffsetdw = 28556;
            this.Color[17].toffsetdw = 29536;
            this.Color[17].fogDW = new ff9.SVECTOR(136, 148, 175, 0);
            this.Color[17].goffsetcl = 17000;
            this.Color[17].toffsetcl = 13000;
            this.Color[17].fogCL = new ff9.SVECTOR(136, 148, 175, 130);
            this.Color[17].chrBIAS = new ff9.SVECTOR(2296, 2396, 2396, 0);
            this.Color[17].fogAMP = 1296;
            this.Color[18].light0 = new ff9.SVECTOR(1012, 1012, 1012, 0);
            this.Color[18].light1 = new ff9.SVECTOR(724, 724, 724, 243);
            this.Color[18].light2 = new ff9.SVECTOR(1648, 1548, 1348, 256);
            this.Color[18].light0c = new ff9.SVECTOR(256, 768, 1596, 96);
            this.Color[18].ambient = new ff9.SVECTOR(26, 26, 26, 4);
            this.Color[18].ambientcl = new ff9.SVECTOR(45, 49, 64, 0);
            this.Color[18].goffsetup = 17000;
            this.Color[18].toffsetup = 16000;
            this.Color[18].fogUP = new ff9.SVECTOR(207, 207, 247, 16);
            this.Color[18].goffsetdw = 24008;
            this.Color[18].toffsetdw = 32000;
            this.Color[18].fogDW = new ff9.SVECTOR(205, 217, 247, 16);
            this.Color[18].goffsetcl = 12000;
            this.Color[18].toffsetcl = 0;
            this.Color[18].fogCL = new ff9.SVECTOR(205, 217, 247, 640);
            this.Color[18].chrBIAS = new ff9.SVECTOR(3596, 3596, 3596, 256);
            this.Color[18].fogAMP = 4096;
            this.Color[19].light0 = new ff9.SVECTOR(32, 64, 8, 704);
            this.Color[19].light1 = new ff9.SVECTOR(320, 8, 32, 8);
            this.Color[19].light2 = new ff9.SVECTOR(712, 320, 8, 32);
            this.Color[19].light0c = new ff9.SVECTOR(8, 272, 245, 0);
            this.Color[19].ambient = new ff9.SVECTOR(6, 96, 272, 247);
            this.Color[19].ambientcl = new ff9.SVECTOR(0, 6, 96, 272);
            this.Color[19].goffsetup = 243;
            this.Color[19].toffsetup = 0;
            this.Color[19].fogUP = new ff9.SVECTOR(6, 96, 272, 241);
            this.Color[19].goffsetdw = 0;
            this.Color[19].toffsetdw = 6;
            this.Color[19].fogDW = new ff9.SVECTOR(96, 272, 242, 0);
            this.Color[19].goffsetcl = 4;
            this.Color[19].toffsetcl = 64;
            this.Color[19].fogCL = new ff9.SVECTOR(272, 229, 109, 130);
            this.Color[19].chrBIAS = new ff9.SVECTOR(512, 256, 245, 0);
            this.Color[19].fogAMP = 4;
            this.Color[20].light0 = new ff9.SVECTOR(200, 200, 300, 0);
            this.Color[20].light1 = new ff9.SVECTOR(0, 0, 0, 0);
            this.Color[20].light2 = new ff9.SVECTOR(748, 748, 976, 0);
            this.Color[20].light0c = new ff9.SVECTOR(448, 532, 1152, 0);
            this.Color[20].ambient = new ff9.SVECTOR(30, 30, 31, 0);
            this.Color[20].ambientcl = new ff9.SVECTOR(48, 48, 48, 0);
            this.Color[20].goffsetup = 23104;
            this.Color[20].toffsetup = 27592;
            this.Color[20].fogUP = new ff9.SVECTOR(136, 148, 175, 0);
            this.Color[20].goffsetdw = 28556;
            this.Color[20].toffsetdw = 29536;
            this.Color[20].fogDW = new ff9.SVECTOR(136, 148, 175, 0);
            this.Color[20].goffsetcl = 17000;
            this.Color[20].toffsetcl = 13000;
            this.Color[20].fogCL = new ff9.SVECTOR(136, 148, 175, 0);
            this.Color[20].chrBIAS = new ff9.SVECTOR(2296, 2396, 2396, 0);
            this.Color[20].fogAMP = 1296;
            this.Color[21].light0 = new ff9.SVECTOR(96, 256, 247, 0);
            this.Color[21].light1 = new ff9.SVECTOR(4, 96, 256, 243);
            this.Color[21].light2 = new ff9.SVECTOR(0, 4, 96, 256);
            this.Color[21].light0c = new ff9.SVECTOR(241, 0, 4, 96);
            this.Color[21].ambient = new ff9.SVECTOR(256, 242, 0, 4);
            this.Color[21].ambientcl = new ff9.SVECTOR(64, 256, 229, 0);
            this.Color[21].goffsetup = 10;
            this.Color[21].toffsetup = 40;
            this.Color[21].fogUP = new ff9.SVECTOR(848, 243, 0, 16);
            this.Color[21].goffsetdw = 16;
            this.Color[21].toffsetdw = 832;
            this.Color[21].fogDW = new ff9.SVECTOR(243, 0, 4, 16);
            this.Color[21].goffsetcl = 848;
            this.Color[21].toffsetcl = 243;
            this.Color[21].fogCL = new ff9.SVECTOR(0, 8, 64, 640);
            this.Color[21].chrBIAS = new ff9.SVECTOR(229, 0, 196, 256);
            this.Color[21].fogAMP = 640;
            this.Color[22].light0 = new ff9.SVECTOR(1012, 1012, 1012, 0);
            this.Color[22].light1 = new ff9.SVECTOR(724, 724, 724, 0);
            this.Color[22].light2 = new ff9.SVECTOR(1648, 1548, 1348, 0);
            this.Color[22].light0c = new ff9.SVECTOR(256, 768, 1596, 0);
            this.Color[22].ambient = new ff9.SVECTOR(26, 26, 26, 0);
            this.Color[22].ambientcl = new ff9.SVECTOR(45, 49, 64, 0);
            this.Color[22].goffsetup = 17000;
            this.Color[22].toffsetup = 16000;
            this.Color[22].fogUP = new ff9.SVECTOR(207, 207, 247, 0);
            this.Color[22].goffsetdw = 24008;
            this.Color[22].toffsetdw = 32000;
            this.Color[22].fogDW = new ff9.SVECTOR(205, 217, 247, 0);
            this.Color[22].goffsetcl = 12000;
            this.Color[22].toffsetcl = 0;
            this.Color[22].fogCL = new ff9.SVECTOR(205, 217, 247, 0);
            this.Color[22].chrBIAS = new ff9.SVECTOR(3596, 3596, 3596, 0);
            this.Color[22].fogAMP = 4096;
        }

        public ff9.sw_weatherColorElement[] Color = new ff9.sw_weatherColorElement[23];
    }

    public class sworldEncountSpecial
    {
        public UInt16[] area = new UInt16[12];
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public class FF9SAVE_WORLD
    {
        public FF9SAVE_WORLD()
        {
            this.cameraState = new ff9.sworldStateCamera();
        }

        public UInt32 hintmap;

        public ff9.sworldStateCamera cameraState;

        public Byte internall;

        public Byte[] pad = new Byte[3];
    }

    public class sworldStateCamera
    {
        public Single rotationMax;

        public Int16 upperCounter;

        public Int32 upperCounterSpeed;

        public Boolean upperCounterForce;

        public Single rotation;

        public Single rotationRev;
    }

    public class sworldState
    {
        public ff9.sworldStateCamera statecamera = new ff9.sworldStateCamera();

        public SByte loadgameflg;

        public Boolean cameraNotrot;

        public Boolean internall;

        public SByte[] pad = new SByte[21];
    }
}
