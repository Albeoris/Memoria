using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using AOT;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using UnityEngine;

public class SFX
{
    public SFX()
    {
    }

    static SFX()
    {
        SFX.fxNearZ = 100f;
        SFX.fxFarZ = 65536f;
        SFX.S1 = 1;
        SFX.S2 = 2;
        SFX.S3 = 3;
        SFX.C1 = 4;
        SFX.C2 = 8;
        SFX.C3 = 12;
        SFX.M1 = 1073741824;
        SFX.M2 = -2147483648;
        SFX.A1 = 536870912;
        SFX.O1 = 268435456;
        SFX.playParam = new Int32[]
        {
            SFX.C1 + SFX.S1,
            0,
            SFX.C1 + SFX.S1,
            0,
            SFX.C1,
            0,
            SFX.C1,
            SFX.C1 + SFX.S1,
            0,
            0,
            0,
            SFX.M1 + SFX.S3,
            0,
            0,
            SFX.S3,
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
            SFX.S3,
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
            SFX.M1 + SFX.S3,
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
            SFX.S3,
            SFX.M1,
            0,
            0,
            0,
            0,
            SFX.C1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            SFX.C1 + SFX.S1,
            SFX.C1 + SFX.S1,
            SFX.C1 + SFX.S1,
            0,
            0,
            0,
            SFX.M1,
            SFX.C1,
            SFX.S3,
            SFX.S3,
            0,
            0,
            0,
            0,
            0,
            0,
            SFX.A1,
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
            SFX.S3,
            0,
            SFX.S3,
            0,
            0,
            0,
            0,
            0,
            0,
            SFX.C1 + SFX.S1,
            0,
            0,
            0,
            0,
            0,
            SFX.S3,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            SFX.M1 + SFX.C2,
            SFX.C1,
            SFX.C1,
            0,
            0,
            0,
            0,
            0,
            0,
            SFX.C1 + SFX.S1,
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
            SFX.M1,
            SFX.S2,
            0,
            0,
            0,
            0,
            SFX.M1,
            0,
            SFX.M1,
            0,
            SFX.S1,
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
            SFX.M1,
            0,
            0,
            0,
            0,
            0,
            SFX.M1,
            SFX.C1 + SFX.S1,
            SFX.C1 + SFX.S1,
            SFX.C1 + SFX.S1,
            0,
            0,
            0,
            SFX.C1,
            SFX.C1,
            SFX.C1,
            0,
            0,
            0,
            0,
            0,
            0,
            SFX.S1,
            SFX.S2,
            0,
            0,
            SFX.M1 + SFX.C1,
            0,
            SFX.M1,
            0,
            0,
            0,
            SFX.M1,
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
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            SFX.S1 + SFX.C2 + SFX.M1,
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
            SFX.S1,
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
            SFX.S1 + SFX.C1,
            SFX.M1,
            0,
            0,
            SFX.S1,
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
            SFX.M1 + SFX.S2,
            SFX.M1,
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
            SFX.S2 + SFX.O1,
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
            SFX.M1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            SFX.M1
        };
        SFX.subOrder = 0;
        SFX.defaultSubOrder = 2;
        SFX.colIntensity = 0;
        SFX.defaultColIntensity = 2;
        SFX.pixelOffset = 0;
        SFX.colThreshold = 0;
        SFX.addOrder = 0;
        SFX.TEST_SPEED = 0.75f;
        SFX.MAX_INDEX = 64;
        SFX.channel = new Int32[64, 2];
        SFX.isDebugAutoPlay = false;
        SFX.isDebugPng = false;
        SFX.isDebugViewport = false;
        SFX.isDebugLine = false;
        SFX.isDebugCam = false;
        SFX.isDebugMode = false;
        SFX.isDebugFillter = true;
        SFX.isDebugMeshIndex = 0;
        SFX.isRunning = false;
        SFX.frameIndex = 0;
        SFX.isSystemRun = false;
        SFX.isUpdated = false;
        SFX.screenWidth = 1f;
        SFX.screenHeight = 1f;
        SFX.screenWidthOffset = 0f;
        SFX.screenRatio = 1f;
        SFX.screenWidthRatio = 1f;
        SFX.screenHeightRatio = 1f;
        SFX.effTeble = new Int32[]
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            17,
            21,
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29,
            30,
            31,
            32,
            33,
            34,
            35,
            36,
            38,
            40,
            41,
            42,
            43,
            44,
            45,
            46,
            47,
            48,
            49,
            50,
            51,
            52,
            53,
            54,
            55,
            56,
            57,
            58,
            59,
            60,
            61,
            62,
            63,
            64,
            65,
            66,
            67,
            68,
            69,
            70,
            71,
            72,
            73,
            74,
            75,
            76,
            77,
            78,
            79,
            81,
            82,
            83,
            85,
            86,
            87,
            88,
            89,
            90,
            92,
            93,
            94,
            95,
            96,
            97,
            98,
            99,
            120,
            121,
            122,
            123,
            124,
            125,
            126,
            127,
            128,
            129,
            130,
            131,
            132,
            133,
            134,
            135,
            136,
            137,
            138,
            139,
            140,
            141,
            142,
            143,
            144,
            145,
            146,
            147,
            148,
            149,
            150,
            151,
            152,
            153,
            154,
            155,
            156,
            157,
            158,
            159,
            160,
            161,
            162,
            163,
            164,
            165,
            166,
            167,
            168,
            169,
            170,
            171,
            172,
            173,
            174,
            175,
            176,
            177,
            178,
            179,
            180,
            181,
            182,
            183,
            184,
            185,
            186,
            187,
            188,
            189,
            190,
            191,
            192,
            193,
            194,
            195,
            196,
            197,
            198,
            199,
            201,
            202,
            203,
            204,
            205,
            206,
            207,
            208,
            209,
            210,
            211,
            212,
            213,
            214,
            215,
            216,
            217,
            218,
            219,
            220,
            221,
            222,
            223,
            224,
            225,
            226,
            227,
            228,
            229,
            230,
            231,
            232,
            233,
            234,
            235,
            236,
            237,
            238,
            239,
            240,
            241,
            242,
            243,
            244,
            245,
            246,
            247,
            248,
            249,
            250,
            251,
            252,
            253,
            254,
            255,
            256,
            257,
            258,
            259,
            260,
            261,
            262,
            274,
            275,
            276,
            277,
            278,
            279,
            280,
            281,
            282,
            283,
            284,
            285,
            286,
            287,
            288,
            289,
            290,
            291,
            292,
            293,
            294,
            295,
            296,
            297,
            298,
            299,
            300,
            301,
            302,
            303,
            304,
            305,
            306,
            307,
            308,
            309,
            310,
            311,
            312,
            377,
            378,
            381,
            382,
            383,
            384,
            385,
            386,
            387,
            388,
            389,
            390,
            391,
            392,
            394,
            395,
            396,
            397,
            398,
            399,
            400,
            401,
            402,
            403,
            404,
            405,
            406,
            407,
            408,
            409,
            410,
            411,
            412,
            413,
            414,
            415,
            416,
            417,
            418,
            419,
            420,
            421,
            422,
            423,
            424,
            425,
            427,
            428,
            429,
            431,
            432,
            433,
            434,
            435,
            436,
            431,
            432,
            433,
            434,
            435,
            436,
            443,
            445,
            446,
            447,
            457,
            458,
            459,
            460,
            489,
            490,
            491,
            492,
            493,
            494,
            495,
            496,
            497,
            498,
            499,
            500,
            501,
            502,
            503,
            504,
            505,
            506,
            507,
            508,
            509,
            510
        };
    }

    private static Int32 UpdatePluginErrorCount = 0;

    public static void UpdatePlugin()
    {
        try
        {
            if (SFX.isSystemRun)
            {
                SFX.UpdateScreenSize();
                SFX.isRunning = SFX.SFX_Update(ref SFX.frameIndex);
                if (SFX.isRunning)
                {
                    SFX.isUpdated = true;
                    Int32 num = SFX.currentEffectID;
                    PSXTextureMgr.isCaptureBlur = num != 274;
                    if (SFX.preventStepInOut >= 0 && SFX.frameIndex - SFX.preventStepInOut > 2)
                        SFX.preventStepInOut = -1;
                    if (SFX.currentEffectID == 381)
                    {
                        if (SFX.frameIndex == 1004)
                        {
                            SFX.subOrder = 2;
                        }
                        if (SFX.frameIndex == 1193)
                        {
                            SFX.subOrder = 0;
                        }
                    }
                    if (SFX.currentEffectID == 301)
                    {
                        // Fix the effect of Antlion's death: it sunk in the sand, in the PSX version
                        BTL_DATA btl;
                        for (btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
                            if (btl.btl_id == SFX.request.trg0.btl_id)
                                break;
                        if (btl != null && btl.bi.stop_anim != 0)
                            btl.pos.y -= 20;
                    }
                }
                vib.VIB_service();
            }
        }
        catch (Exception ex)
        {
            Int32 errorCount = Interlocked.Increment(ref UpdatePluginErrorCount);
            Log.Error(ex, "Failed to update SFX plugin. ErrorCount: " + errorCount);
            if (errorCount > 1000)
            {
                Interlocked.Exchange(ref UpdatePluginErrorCount, 0);
                throw;
            }
        }
    }

    public static void LateUpdatePlugin()
    {
        if (SFX.isSystemRun && SFX.isUpdated)
        {
            SFX.isUpdated = false;
            SFX.SFX_LateUpdate();
            SFXRender.Update();
        }
    }

    public static void PostRender()
    {
        if (SFX.isSystemRun)
        {
            SFX.ResetViewPort();
            if (SFX.isRunning)
            {
                SFXRender.Render();
            }
        }
    }

    public static void DebugOnGUI()
    {
    }

    public static void ResetViewPort()
    {
        if (!SFX.isDebugViewport)
        {
            GL.Viewport(new Rect(SFX.screenWidthOffset, 0f, SFX.screenWidth, SFX.screenHeight));
            GL.LoadIdentity();
            Matrix4x4 mat = Matrix4x4.Ortho(0f, FieldMap.PsxScreenWidth, FieldMap.PsxScreenHeightNative, 0f, SFX.fxNearZ, SFX.fxFarZ);
            PsxCamera.SetOrthoZ(ref mat, SFX.fxNearZ, SFX.fxFarZ);
            GL.LoadProjectionMatrix(mat);
        }
        else
        {
            GL.Viewport(new Rect(SFX.screenWidthOffset, 0f, SFX.screenWidth, SFX.screenHeight));
            GL.LoadIdentity();
            Matrix4x4 mat2 = Matrix4x4.Ortho(-FieldMap.PsxScreenWidth, FieldMap.PsxScreenWidth * 2, FieldMap.PsxScreenHeightNative * 2, -FieldMap.PsxScreenHeightNative, SFX.fxNearZ, SFX.fxFarZ);
            PsxCamera.SetOrthoZ(ref mat2, SFX.fxNearZ, SFX.fxFarZ);
            GL.LoadProjectionMatrix(mat2);
        }
    }

    public static void SkipCameraAnimation(Int32 skip)
    {
        SFX.SFX_SkipCameraAnimation(skip);
    }

    public static void UpdateScreenSize()
    {
        if (Configuration.Graphics.WidescreenSupport)
        {
            SFX.screenWidth = Screen.width;
            SFX.screenHeight = Screen.height;
            SFX.screenWidthOffset = 0;
            SFX.screenHeightRatio = 1f;
            SFX.screenWidthRatio = 1f;
        }
        else
        {
            Single num = Screen.width / (Single)Screen.height;
            if (num >= 1.0)
            {
                SFX.screenHeight = Screen.height;
                SFX.screenWidth = Screen.height * 1.4545455f;
                SFX.screenWidthOffset = (Screen.width - SFX.screenWidth) * 0.5f;
            }
            else
            {
                SFX.screenHeight = Screen.width;
                SFX.screenWidth = Screen.width * 1.4545455f;
                SFX.screenWidthOffset = (Screen.height - SFX.screenWidth) * 0.5f;
            }
        }
    }

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern void SFX_InitSystem(Callback method);

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern void SFX_StartPlungeCamera(IntPtr btlseq, Int32 btlseqLen, Int32 camOffset, Int32 projOffset);

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern void SFX_SkipCameraAnimation(Int32 skip);

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern void SFX_InitBattle(IntPtr param);

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern Boolean SFX_Update(ref Int32 frameIndex);

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern void SFX_LateUpdate();

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern IntPtr SFX_UpdateCamera(Int32 isDebug);

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern void SFX_MoveFreeCamera(Int32 type, Int32 x, Int32 y);

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern Int32 SFX_SendFloatData(Int32 type, Int32 btl_id, Single arg0, Single arg1, Single arg2);

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern Int32 SFX_SendIntData(Int32 type, Int32 arg0, Int32 arg1, Int32 arg2);

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern void SFX_Play(Int32 effnum, IntPtr bin, Int32 size, IntPtr req);

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern Boolean SFX_BeginRender();

    [DllImport("FF9SpecialEffectPlugin")]
    public static extern IntPtr SFX_GetPrim(ref Int32 otz);

    [MonoPInvokeCallback(typeof(Callback))]
    public static unsafe Int32 BattleCallback(Int32 code, Int32 arg0, Int32 arg1, Int32 arg2, Int32 arg3, void* p)
    {
        Int32 num = code >> 24;
        if (num == 100)
        {
            PSXTextureMgr.LoadImage(arg0, arg1, arg2, arg3, (UInt16*)p);
            return 0;
        }
        if (!SFX.isSystemRun)
        {
            return 0;
        }
        if (SFX.isDebugMode)
        {
            return SFX.DebugRoomCallback(code, arg0, arg1, arg2, arg3, p);
        }
        Int32 num2 = num;
        switch (num2)
        {
            case 101:
                PSXTextureMgr.StoreImage(arg0, arg1, arg2, arg3, (UInt16*)p);
                return 0;
            case 102:
                PSXTextureMgr.MoveImage(arg0, arg1, (Int16*)p);
                return 0;
            //case 103:
            //case 104:
            //case 105:
            //case 106:
            //case 107:
            //case 108:
            //case 109:
            default:
                {
                    return OtherBattleCallback(code, arg0, arg1, arg2, arg3, p, num2, num);
                }
            case 110:
                {
                    FF9StateBattleSystem expr_E1 = FF9StateSystem.Battle.FF9Battle;
                    expr_E1.btl_seq += 1;
                    return 0;
                }
            case 111:
                return (Int32)FF9StateSystem.Settings.cfg.camera;
            case 112:
                {
                    Byte[] expr_116_cp_0 = FF9StateSystem.EventState.gEventGlobal;
                    Int32 expr_116_cp_1 = 199;
                    expr_116_cp_0[expr_116_cp_1] |= 16;
                    return 0;
                }
            case 113:
                {
                    CMD_DATA curCmdPtr = FF9.btl_util.getCurCmdPtr();
                    if (curCmdPtr != null)
                    {
                        UIManager.Battle.SetBattleCommandTitle(curCmdPtr);
                    }
                    return 0;
                }
            case 114:
                if (arg0 != 0)
                {
                    FF9StateBattleSystem expr_148 = FF9StateSystem.Battle.FF9Battle;
                    expr_148.cmd_status |= 2;
                }
                else
                {
                    FF9StateBattleSystem expr_165 = FF9StateSystem.Battle.FF9Battle;
                    expr_165.cmd_status &= 65533;
                }
                return 0;
            case 115:
                return ((FF9StateSystem.Battle.FF9Battle.cmd_status & 2) == 0) ? 0 : 1;
            case 116:
                return 0;
            case 117:
                return battlebg.nf_GetBbgIntensity();
            case 118:
                battlebg.nf_SetBbgIntensity((Byte)arg0);
                return 0;
            case 119:
                switch (arg0)
                {
                    case 0:
                        vib.VIB_purge();
                        break;
                    case 1:
                        {
                            Byte[] array = new Byte[1800];
                            Marshal.Copy((IntPtr)p, array, 0, 1800);
                            MemoryStream input = new MemoryStream(array);
                            BinaryReader binaryReader = new BinaryReader(input);
                            vib.VIB_init(binaryReader);
                            vib.VIB_setTrackActive(0, vib.VIB_SAMPLE_LO, true);
                            vib.VIB_setTrackActive(0, vib.VIB_SAMPLE_HI, true);
                            vib.VIB_vibrate(1);
                            binaryReader.Close();
                            break;
                        }
                    case 2:
                        vib.VIB_setActive(false);
                        vib.VIB_setTrackActive(1, vib.VIB_SAMPLE_LO, true);
                        vib.VIB_setTrackActive(1, vib.VIB_SAMPLE_HI, true);
                        vib.VIB_vibrate(1);
                        break;
                }
                return 0;
            case 120:
                PSXTextureMgr.isBgCapture = true;
                Marshal.Copy((IntPtr)p, PSXTextureMgr.bgParam, 0, 7);
                return 0;
            case 121:
                return battle.btl_bonus.member_flag;
            case 122:
                return (Byte)FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType;
            case 123:
                {
                    Int32 num10 = 0;
                    for (BTL_DATA next2 = FF9StateSystem.Battle.FF9Battle.btl_list.next; next2 != null; next2 = next2.next)
                    {
                        if (next2.bi.player != 0 && !FF9.Status.checkCurStat(next2, BattleStatus.Death | BattleStatus.Jump))
                        {
                            num10++;
                        }
                    }
                    return num10;
                }
            case 124:
                return FF9StateSystem.Battle.FF9Battle.seq_work_set.CameraNo;
            case 125:
                SFX.soundFPS = arg0;
                return 0;
        }
    }

    private static unsafe Int32 OtherBattleCallback(Int32 code, Int32 arg0, Int32 arg1, Int32 arg2, Int32 arg3, void* p, Int32 num2, Int32 num)
    {
        if (num2 == 32)
        {
            switch (arg0)
            {
                case 0:
                    SFX.SoundPlay(arg1, arg2, arg3);
                    break;
                case 1:
                    SFX.SoundPlayChant(arg1, arg2, arg3);
                    break;
                case 2:
                    SFX.SoundStop(arg1, arg2);
                    break;
                case 3:
                    SFX.StreamPlay(arg1);
                    break;
            }
            return 0;
        }
        Int32 num3 = code & 255;
        BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next;
        while (next.next != null)
        {
            if (next.btl_id == num3)
            {
                break;
            }
            next = next.next;
        }
        bool flag2 = (int)next.btl_id == num3;
        if (next == null || next.btl_id == 0)
        {
            return (num != 9) ? 0 : 1;
        }
        switch (num)
        {
            case 1:
                switch (arg0)
                {
                    case 0:
                        *(Int16*)p = (Int16)next.pos.x;
                        *(Int16*)((Byte*)p + 2) = (Int16)(-(Int16)(next.pos.y + next.attachOffset));
                        *(Int16*)((Byte*)p + 4) = (Int16)next.pos.z;
                        break;
                    case 1:
                        *(Int16*)p = (Int16)next.base_pos.x;
                        *(Int16*)((Byte*)p + 2) = (Int16)(-(Int16)(next.base_pos.y + next.attachOffset));
                        *(Int16*)((Byte*)p + 4) = (Int16)next.base_pos.z;
                        break;
                    case 2:
                        *(Int16*)p = (Int16)next.pos.x;
                        *(Int16*)((Byte*)p + 4) = (Int16)next.pos.z;
                        break;
                    case 3:
                        *(Int16*)p = (Int16)next.base_pos.x;
                        *(Int16*)((Byte*)p + 4) = (Int16)next.base_pos.z;
                        break;
                }
                break;
            case 2:
                if (SFX.preventStepInOut >= 0 && next.btl_id == SFX.request.exe.btl_id)
                {
                    SFX.preventStepInOut = SFX.frameIndex;
                    break;
                }
                next.pos.x = *(Int16*)p;
                next.pos.y = -(*(Int16*)((Byte*)p + 2) + (Single)next.attachOffset);
                next.pos.z = *(Int16*)((Byte*)p + 4);
                break;
            case 3:
                switch (arg0)
                {
                    case 0:
                        *(Int16*)p = (Int16)((next.evt.rotBattle.eulerAngles.x - next.rot.eulerAngles.x) * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)((next.evt.rotBattle.eulerAngles.y - next.rot.eulerAngles.y) * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((next.evt.rotBattle.eulerAngles.z - next.rot.eulerAngles.z) * 11.3777781f);
                        break;
                    case 1:
                        *(Int16*)p = (Int16)(next.rot.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)(next.rot.eulerAngles.y * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((next.rot.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                    case 2:
                        *(Int16*)p = (Int16)(next.evt.rotBattle.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)(next.evt.rotBattle.eulerAngles.y * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((next.evt.rotBattle.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                    case 3:
                        *(Int16*)p = (Int16)(next.evt.rotBattle.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((next.evt.rotBattle.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                }
                break;
            case 4:
                switch (arg0)
                {
                    case 0:
                        next.rot.eulerAngles = next.evt.rotBattle.eulerAngles;
                        break;
                    case 1:
                        next.rot.eulerAngles = new Vector3(next.rot.eulerAngles.x, next.evt.rotBattle.eulerAngles.y + 180f, next.rot.eulerAngles.z);
                        break;
                    case 2:
                        next.rot.eulerAngles = new Vector3(*(Int16*)p * 0.087890625f, *(Int16*)((Byte*)p + 2) * 0.087890625f - 180f, *(Int16*)((Byte*)p + 4) * 0.087890625f + 180f);
                        break;
                }
                break;
            case 5:
                *(Int32*)p = (Int32)(next.gameObject.transform.localScale.x * 4096f);
                *(Int32*)((Byte*)p + 4) = (Int32)(next.gameObject.transform.localScale.y * 4096f);
                *(Int32*)((Byte*)p + 8) = (Int32)(next.gameObject.transform.localScale.z * 4096f);
                break;
            case 6:
                if ((arg0 & 128) == 0)
                {
                    FF9.geo.geoScaleReset(next);
                }
                else
                {
                    next.flags |= FF9.geo.GEO_FLAGS_SCALE;
                }
                if ((arg0 & 1) == 1)
                {
                    next.gameObject.transform.localScale = new Vector3(arg1 / 4096f, arg2 / 4096f, arg3 / 4096f);
                }
                break;
            case 7:
                return next.flags;
            case 8:
                if (FF9StateSystem.Battle.FF9Battle.cur_cmd != null)
                {
                    return (FF9StateSystem.Battle.FF9Battle.cur_cmd.cmd_no != BattleCommandId.AutoPotion || !btl_cmd.CheckUsingCommand(next.cmd[0])) ? 0 : 1;
                }
                return 0;
            case 9:
                {
                    UInt16 num4 = FF9.GeoAnim.geoAnimGetNumFrames(next);
                    if (num4 == 0)
                    {
                        num4 = 1;
                    }
                    return num4;
                }
            case 10:
                return next.evt.animFrame;
            case 11:
                {
                    UInt16 num5 = FF9.GeoAnim.geoAnimGetNumFrames(next);
                    next.evt.animFrame = (((Int32)num5 <= arg0) ? ((Byte)(num5 - 1)) : ((Byte)arg0));
                    break;
                }
            case 12:
                if (arg0 == -1)
                {
                    arg0 = next.bi.def_idle;
                }
                if (arg0 < 6 || next.bi.player != 0)
                {
                    // Use Freya's special casting animations instead of her CHANT/MAGIC animations (jump)
                    if (SFX.currentEffectID != 393 && (arg0 == (Byte)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_CHANT || arg0 == (Byte)BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT || arg0 == (Byte)BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC))
                    {
                        String goName = next.gameObject.name;
                        goName.Trim();
                        if (goName.CompareTo("192(Clone)") == 0 || goName.CompareTo("585(Clone)") == 0)
                        {
                            if (arg0 == (Byte)BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT)
                                FF9.btl_mot.setMotion(next, "ANH_MAIN_B0_011_201");
                            else if (arg0 == (Byte)BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC)
                                FF9.btl_mot.setMotion(next, "ANH_MAIN_B0_011_202");
                            break;
                        }
                    }
                    if ((arg0 == 29 || arg0 == 30) && next.is_monster_transform && next.btl_id == SFX.request.exe.btl_id)
                    {
                        SFX.preventStepInOut = SFX.frameIndex;
                        break;
                    }
                    FF9.btl_mot.setMotion(next, (Byte)arg0);
                    next.evt.animFrame = 0;
                }
                break;
            case 13:
                {
                    // Handle Ultima (Pandemonium / Crystal World) such that it doesn't freeze the characters
                    // In order to use the SFX Ultima as normal spells, this fix is required
                    //  + adding two effect points (damage point & figure point) in the efXXX file
                    //  + reset the BBG transparency at the end ("Set battle scene transparency 255 5" in Hades Workshop)
                    //  + make sure that all the characters are shown at the end (removing all the lines "Show/hide characters" in Hades Workshop has a good-looking result)
                    // Also, the caster moves away when using SFX 384 (Ultima in Crystal World), so either reset the caster's position at the end of the sequencing or use the Pandemonium version
                    if ((SFX.currentEffectID == 492 || SFX.currentEffectID == 384) && FF9StateSystem.Battle.FF9Battle.btl_phase == 4)
                        return next.bi.stop_anim;
                    Byte stop_anim = next.bi.stop_anim;
                    next.bi.stop_anim = (Byte)arg0;
                    return stop_anim;
                }
            case 14:
                {
                    Matrix4x4 matrix4x;
                    try
                    {
                        matrix4x = next.gameObject.transform.GetChildByName("bone" + arg1.ToString("D3")).localToWorldMatrix;
                    }
                    catch (NullReferenceException)
                    {
                        matrix4x = Matrix4x4.identity;
                    }
                    switch (arg0)
                    {
                        case 0:
                            *(Int16*)p = (Int16)matrix4x.m03;
                            *(Int16*)((Byte*)p + 2) = (Int16)(-(Int16)(matrix4x.m13 + next.attachOffset));
                            *(Int16*)((Byte*)p + 4) = (Int16)matrix4x.m23;
                            break;
                        case 1:
                            return -(Int32)(matrix4x.m13 + next.attachOffset);
                        case 2:
                            PSXMAT* matPtr = (PSXMAT*)p;
                            Int16* rptr = matPtr->r;
                            Int32* tptr = matPtr->t;

                            matPtr->pad = 0;

                            rptr[0] = (Int16)(matrix4x.m00 * -4096f);
                            rptr[1] = (Int16)(matrix4x.m01 * -4096f);
                            rptr[2] = (Int16)(matrix4x.m02 * 4096f);
                            rptr[3] = (Int16)(matrix4x.m10 * -4096f);
                            rptr[4] = (Int16)(matrix4x.m11 * 4096f);
                            rptr[5] = (Int16)(matrix4x.m12 * -4096f);
                            rptr[6] = (Int16)(matrix4x.m20 * 4096f);
                            rptr[7] = (Int16)(matrix4x.m21 * 4096f);
                            rptr[8] = (Int16)(matrix4x.m22 * 4096f);

                            tptr[0] = (Int32)matrix4x.m03;
                            tptr[1] = -(Int32)(matrix4x.m13 + next.attachOffset);
                            tptr[2] = (Int32)matrix4x.m23;
                            break;
                    }
                    break;
                }
            case 15:
                return next.bi.target;
            case 16:
                FF9.btl_mot.SetDefaultIdle(next);
                break;
            case 17:
                return next.bi.disappear;
            case 18:
                if (SFX.currentEffectID == 301)
                {
                    next.bi.stop_anim = 1;
                    foreach (Material material in battlebg.GetShaders(2))
                        material.SetInt("_ZWrite", 1); // Turn the ground into an opaque material
                }
                else
                {
                    next.SetDisappear((Byte)arg0);
                    if (arg0 == 0)
                    {
                        btlseq.DispCharacter(next);
                    }
                }
                break;
            case 19:
                for (Int32 i = 0; i < next.weaponMeshCount; i++)
                {
                    if (arg0 != 0)
                    {
                        FF9.geo.geoWeaponMeshShow(next, i);
                    }
                    else
                    {
                        FF9.geo.geoWeaponMeshHide(next, i);
                    }
                }
                break;
            case 20:
                switch (arg0)
                {
                    case 0:
                        {
                            BattleStatus num6 = (BattleStatus)(arg2 << 16 | arg1);
                            return ((next.stat.cur & num6) == 0u) ? 0 : 1;
                        }
                    case 1:
                        {
                            BattleStatus num6 = (BattleStatus)(arg2 << 16 | arg1);
                            if (SFX.currentEffectID == 237 && (num6 & BattleStatus.Death) != 0u && !flag2)
                            {
                                return 1;
                            }
                            return ((next.stat.permanent & num6) == 0u && (next.stat.cur & num6) == 0u) ? 0 : 1;
                        }
                    case 2:
                        btl_stat.RemoveStatuses(next, BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Virus | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Zombie | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.AutoLife | BattleStatus.Trance | BattleStatus.Defend | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Regen | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Float | BattleStatus.Shell | BattleStatus.Protect | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Vanish | BattleStatus.Doom | BattleStatus.Mini | BattleStatus.Reflect | BattleStatus.Jump | BattleStatus.GradualPetrify);
                        break;
                    case 3:
                        btl_stat.InitStatus(next);
                        break;
                }
                break;
            case 21:
                return next.bi.cmd_idle;
            case 22:
                return next.bi.slave;
            case 23:
                btl_cmd.ExecVfxCommand(next);
                break;
            case 24:
                btl2d.Btl2dReq(next);
                break;
            case 25:
                switch (arg0)
                {
                    case 0:
                        FF9.btl_mot.HideMesh(next, 65535, false);
                        break;
                    case 1:
                        FF9.btl_mot.HideMesh(next, next.mesh_banish, true);
                        break;
                    case 2:
                        FF9.btl_mot.ShowMesh(next, 65535, false);
                        break;
                    case 3:
                        SFX.fade = arg1;
                        if ((next.flags & FF9.geo.GEO_FLAGS_RENDER) != 0 && (next.flags & FF9.geo.GEO_FLAGS_CLIP) == 0)
                        {
                            btl_stat.GeoAddColor2DrawPacket(next.gameObject, (Int16)(SFX.fade - 128), (Int16)(SFX.fade - 128), (Int16)(SFX.fade - 128));
                        }
                        break;
                    case 4:
                        {
                            SFX.fade = arg1;
                            Int32 num7 = SFX.fade;
                            Int32 num8;
                            if (SFX.fade >= 256)
                            {
                                num8 = 0;
                                num7 = SFX.fade - 256;
                            }
                            else
                            {
                                num8 = 1;
                            }
                            if (next.bi.player != 0 && next.weapon_geo != null && num8 != 0 && (next.weaponFlags & FF9.geo.GEO_FLAGS_RENDER) != 0 && (next.weaponFlags & FF9.geo.GEO_FLAGS_CLIP) == 0)
                            {
                                btl_stat.GeoAddColor2DrawPacket(next.weapon_geo, (Int16)(num7 - 128), (Int16)(num7 - 128), (Int16)(num7 - 128));
                                if (num7 < 70)
                                {
                                    FF9.btl_util.GeoSetABR(next.weapon_geo, "GEO_POLYFLAGS_TRANS_100_PLUS_25");
                                }
                            }
                            if ((next.flags & FF9.geo.GEO_FLAGS_RENDER) != 0 && (next.flags & FF9.geo.GEO_FLAGS_CLIP) == 0)
                            {
                                btl_stat.GeoAddColor2DrawPacket(next.gameObject, (Int16)(num7 - 128), (Int16)(num7 - 128), (Int16)(num7 - 128));
                                if (num7 < 70)
                                {
                                    FF9.btl_util.GeoSetABR(next.gameObject, "GEO_POLYFLAGS_TRANS_100_PLUS_25");
                                }
                            }
                            break;
                        }
                }
                break;
            case 26:
                return next.meshCount;
            case 27:
                btl_stat.SetPresentColor(next);
                break;
            case 28:
                {
                    Boolean flag = arg1 != 0;
                    switch (num)
                    {
                        case 0:
                            {
                                Int32 anum = (!flag) ? 0 : 1;
                                GeoTexAnim.geoTexAnimStop(next.texanimptr, anum);
                                break;
                            }
                        case 1:
                            GeoTexAnim.geoTexAnimStop(next.texanimptr, 2);
                            break;
                        case 2:
                            GeoTexAnim.geoTexAnimPlay(next.texanimptr, 2);
                            break;
                    }
                    break;
                }
            case 29:
                FF9.btl_util.SetBattleSfx(next, 1110, 127);
                break;
            case 30:
                btlsnd.ff9btlsnd_sndeffect_play(btlsnd.ff9btlsnd_weapon_sfx(next.bi.line_no, FF9BatteSoundWeaponSndEffectType.FF9BTLSND_WEAPONSNDEFFECTTYPE_ATTACK), 0, 127, SeSnd.S_SeGetPos((UInt64)arg0));
                break;
            case 31:
                btlsnd.ff9btlsnd_sndeffect_play(btlsnd.ff9btlsnd_weapon_sfx(next.bi.line_no, FF9BatteSoundWeaponSndEffectType.FF9BTLSND_WEAPONSNDEFFECTTYPE_HIT), 0, 127, 128);
                break;
            case 33:
                {
                    // Freya's Dragon casting animation (looping or launch)
                    Int32 num9 = arg1;
                    switch (num9)
                    {
                        case 61: // Luna
                            arg0 = ((arg0 != 20) ? 1 : 0);
                            break;
                        case 83: // White Draw
                            arg0 = ((arg0 != 9) ? 1 : 0);
                            break;
                        case 168: // Reis' Wind
                            arg0 = ((arg0 != 11) ? 1 : 0);
                            break;
                        case 296: // Dragon Breath
                            arg0 = ((arg0 != 19) ? 1 : 0);
                            break;
                        case 387: // Cherry Blossom
                            arg0 = ((arg0 != 18) ? 1 : 0);
                            break;
                        case 490: // Dragon's Crest
                            arg0 = ((arg0 != 14) ? 1 : 0);
                            break;
                        case 491: // Six Dragons
                            arg0 = ((arg0 != 18) ? 1 : 0);
                            break;
                            /* The sequence code 0x64 ("Play Freya's casting anim" in Hades Workshop) that generates this callback code is not used in other SFX natively but why not...
                            default:
                                Debug.LogError("No match special effect motion");
                                return 0;*/
                    }
                    String goName = next.gameObject.name;
                    goName.Trim();
                    if (goName.CompareTo("192(Clone)") == 0 || goName.CompareTo("585(Clone)") == 0) // GEO_MAIN_B0_011's and GEO_MAIN_B0_033's internal names (Freya/Trance Freya)
                        FF9.btl_mot.setMotion(next, arg0 != 0 ? "ANH_MAIN_B0_011_202" : "ANH_MAIN_B0_011_201");
                    else
                        FF9.btl_mot.setMotion(next, arg0 != 0 ? BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC : BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT);
                    next.evt.animFrame = 0;
                    break;
                }
        }
        return 0;
    }

    public static void CrateEffectDebugData()
    {
        SFX.cpos.vx = 0;
        SFX.cpos.vy = 0;
        SFX.cpos.vz = 0;
        SFX.cpos_org.vx = 0;
        SFX.cpos_org.vy = 0;
        SFX.cpos_org.vz = 0;
        SFX.crot.vx = 0;
        SFX.crot.vy = 0;
        SFX.crot.vz = 0;
        SFX.crot_org.vx = 0;
        SFX.crot_org.vy = 0;
        SFX.crot_org.vz = 0;
        SFX.cscl.vx = 4096;
        SFX.crot.vy = 4096;
        SFX.crot.vz = 4096;
    }

    public static unsafe Int32 DebugRoomCallback(Int32 code, Int32 arg0, Int32 arg1, Int32 arg2, Int32 arg3, void* p)
    {
        Int32 num = code >> 24;
        Int32 num2 = num;
        switch (num2)
        {
            case 100:
                PSXTextureMgr.LoadImage(arg0, arg1, arg2, arg3, (UInt16*)p);
                return 0;
            case 101:
                PSXTextureMgr.StoreImage(arg0, arg1, arg2, arg3, (UInt16*)p);
                return 0;
            case 102:
                PSXTextureMgr.MoveImage(arg0, arg1, (Int16*)p);
                return 0;
            default:
                if (num2 == 110 || num2 == 111)
                {
                    return 0;
                }
                if (num2 == 32)
                {
                    if (arg0 != 0)
                    {
                        if (arg0 == 1)
                        {
                            SFX.SoundStop(arg1, arg2);
                        }
                    }
                    else
                    {
                        SFX.SoundPlay(arg1, arg2, arg3);
                    }
                    return 0;
                }
                if (num2 != 119)
                {
                    Single[] array = new Single[]
                    {
                        1f,
                        0f,
                        0f,
                        0f,
                        0f,
                        1f,
                        0f,
                        0f,
                        0f,
                        0f,
                        1f,
                        0f,
                        0f,
                        0f,
                        0f,
                        1f
                    };
                    switch (num)
                    {
                        case 1:
                            switch (arg0)
                            {
                                case 0:
                                    *(Int16*)p = SFX.cpos.vx;
                                    *(Int16*)((Byte*)p + 2) = SFX.cpos.vy;
                                    *(Int16*)((Byte*)p + 4) = SFX.cpos.vz;
                                    break;
                                case 1:
                                    *(Int16*)p = SFX.cpos_org.vx;
                                    *(Int16*)((Byte*)p + 2) = SFX.cpos_org.vy;
                                    *(Int16*)((Byte*)p + 4) = SFX.cpos_org.vz;
                                    break;
                                case 2:
                                    *(Int16*)p = SFX.cpos.vx;
                                    *(Int16*)((Byte*)p + 4) = SFX.cpos.vz;
                                    break;
                                case 3:
                                    *(Int16*)p = SFX.cpos_org.vx;
                                    *(Int16*)((Byte*)p + 4) = SFX.cpos_org.vz;
                                    break;
                            }
                            break;
                        case 2:
                            SFX.cpos.vx = *(Int16*)p;
                            SFX.cpos.vy = *(Int16*)((Byte*)p + 2);
                            SFX.cpos.vz = *(Int16*)((Byte*)p + 4);
                            break;
                        case 3:
                            switch (arg0)
                            {
                                case 0:
                                    *(Int16*)p = (Int16)(SFX.crot_org.vx - SFX.crot.vx);
                                    *(Int16*)((Byte*)p + 2) = (Int16)(SFX.crot_org.vy - SFX.crot.vy);
                                    *(Int16*)((Byte*)p + 4) = (Int16)(SFX.crot_org.vz - SFX.crot.vz);
                                    break;
                                case 1:
                                    *(Int16*)p = SFX.crot.vx;
                                    *(Int16*)((Byte*)p + 2) = SFX.crot.vy;
                                    *(Int16*)((Byte*)p + 4) = SFX.crot.vz;
                                    break;
                                case 2:
                                    *(Int16*)p = SFX.crot_org.vx;
                                    *(Int16*)((Byte*)p + 2) = SFX.crot_org.vy;
                                    *(Int16*)((Byte*)p + 4) = SFX.crot_org.vz;
                                    break;
                                case 3:
                                    *(Int16*)p = SFX.crot.vx;
                                    *(Int16*)((Byte*)p + 4) = SFX.crot.vz;
                                    break;
                            }
                            break;
                        case 4:
                            switch (arg0)
                            {
                                case 0:
                                    SFX.crot.vx = SFX.crot_org.vx;
                                    SFX.crot.vy = SFX.crot_org.vy;
                                    SFX.crot.vz = SFX.crot_org.vz;
                                    break;
                                case 1:
                                    SFX.crot.vy = SFX.crot_org.vy;
                                    break;
                                case 2:
                                    SFX.crot.vx = *(Int16*)p;
                                    SFX.crot.vy = *(Int16*)((Byte*)p + 2);
                                    SFX.crot.vz = *(Int16*)((Byte*)p + 4);
                                    break;
                            }
                            break;
                        case 5:
                            *(Int32*)p = SFX.cscl.vx;
                            *(Int32*)((Byte*)p + 4) = SFX.cscl.vy;
                            *(Int32*)((Byte*)p + 8) = SFX.cscl.vz;
                            break;
                        case 6:
                            if ((arg0 & 1) == 1)
                            {
                                SFX.cscl.vx = (Int16)arg1;
                                SFX.cscl.vy = (Int16)arg2;
                                SFX.cscl.vz = (Int16)arg3;
                            }
                            break;
                        case 9:
                            return 1;
                        case 14:
                            switch (arg0)
                            {
                                case 0:
                                    *(Int16*)p = SFX.cpos.vx;
                                    *(Int16*)((Byte*)p + 2) = SFX.cpos.vy;
                                    *(Int16*)((Byte*)p + 4) = SFX.cpos.vz;
                                    break;
                                case 1:
                                    return SFX.cpos.vy;
                            }
                            break;
                        case 15:
                            return 1;
                    }
                    return 0;
                }
                switch (arg0)
                {
                    case 0:
                        vib.VIB_purge();
                        break;
                    case 1:
                        {
                            Byte[] array2 = new Byte[1800];
                            Marshal.Copy((IntPtr)p, array2, 0, 1800);
                            MemoryStream input = new MemoryStream(array2);
                            BinaryReader binaryReader = new BinaryReader(input);
                            vib.VIB_init(binaryReader);
                            vib.VIB_setTrackActive(0, vib.VIB_SAMPLE_LO, true);
                            vib.VIB_setTrackActive(0, vib.VIB_SAMPLE_HI, true);
                            vib.VIB_vibrate(1);
                            binaryReader.Close();
                            break;
                        }
                    case 2:
                        vib.VIB_setActive(false);
                        vib.VIB_setTrackActive(1, vib.VIB_SAMPLE_LO, true);
                        vib.VIB_setTrackActive(1, vib.VIB_SAMPLE_HI, true);
                        vib.VIB_vibrate(1);
                        break;
                }
                return 0;
        }
    }

    public static void UpdateCamera()
    {
        if (SFX.isSystemRun && !FF9StateSystem.Battle.isTutorial && !SFX.isDebugCam)
        {
            Int32 isDebug = (!SFX.isDebugMode) ? 0 : 1;
            IntPtr source = SFX.SFX_UpdateCamera(isDebug);
            Single[] array = new Single[13];
            Marshal.Copy(source, array, 0, 13);
            Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
            SFX.fxNearZ = array[12];
            SFX.fxFarZ = 65535f;
            camera.nearClipPlane = SFX.fxNearZ;
            camera.farClipPlane = SFX.fxFarZ;
            camera.worldToCameraMatrix = PsxCamera.PsxMatrix2UnityMatrix(array, SFX.cameraOffset);
            camera.projectionMatrix = PsxCamera.PsxProj2UnityProj(SFX.fxNearZ, SFX.fxFarZ);
        }
        if (SFX.GetCameraPhase() == 1)
        {
            if (!SFX.isDebugMode)
            {
                FF9StateBattleSystem fF9Battle = FF9StateSystem.Battle.FF9Battle;
                if ((fF9Battle.btl_load_status & 7) == 7)
                {
                    if (fF9Battle.btl_scene.Info.SpecialStart != 0 && !FF9StateSystem.Battle.isDebug)
                    {
                        fF9Battle.btl_phase = 1;
                    }
                    else
                    {
                        fF9Battle.btl_phase = 3;
                    }
                    SFX.InitBattleParty();
                    SFX.SetCameraPhase(0);
                }
            }
            else
            {
                SFX.InitBattleParty();
                SFX.SetCameraPhase(0);
            }
        }
    }

    public static void MoveFreeCamera(Int32 type, Int32 x, Int32 y)
    {
        SFX.SFX_MoveFreeCamera(type, x, y);
    }

    public static void StartBattle()
    {
        SFX.StartCommon(false);
        SFX.StartPlungeCamera();
    }

    public static void StartDebugRoom()
    {
        SFX.StartCommon(true);
        SFX.InitBattleParty();
    }

    public static void StartCommon(Boolean mode)
    {
        SFX.isDebugMode = mode;
        SFXMesh.Init();
        SFXRender.Init();
        unsafe
        {
            SFX.SFX_InitSystem(SFX.BattleCallback);
        }
        SFX.isSystemRun = true;
    }

    public static void EndDebugRoom()
    {
        SFX.EndBattle();
    }

    public static void EndBattle()
    {
        SFXRender.Release();
        SFXMesh.Release();
        PSXTextureMgr.ClearObject();
        PSXTextureMgr.Release();
        SFX.isSystemRun = false;
    }

    public static void StartPlungeCamera()
    {
        Int32 projOffset = 0;
        if (FF9StateSystem.Battle.battleMapIndex == 21)
        {
            projOffset = 100;
        }
        GCHandle gCHandle = GCHandle.Alloc(btlseq.instance.data, GCHandleType.Pinned);
        SFX.SFX_StartPlungeCamera(gCHandle.AddrOfPinnedObject(), btlseq.instance.data.Length, btlseq.instance.camOffset, projOffset);
        gCHandle.Free();
    }

    public static void InitBattleParty()
    {
        SFX_INIT_PARAM sFX_INIT_PARAM = default(SFX_INIT_PARAM);
        if (!SFX.isDebugMode)
        {
            SB2_PATTERN[] patAddr = FF9StateSystem.Battle.FF9Battle.btl_scene.PatAddr;
            SB2_PATTERN sB2_PATTERN = patAddr[FF9StateSystem.Battle.FF9Battle.btl_scene.PatNum];
            sFX_INIT_PARAM.btl_scene_patAddr_putX = sB2_PATTERN.Put[0].Xpos;
            sFX_INIT_PARAM.btl_scene_patAddr_putZ = sB2_PATTERN.Put[0].Zpos;
            sFX_INIT_PARAM.btl_scene_FixedCamera1 = FF9StateSystem.Battle.FF9Battle.btl_scene.Info.FixedCamera1;
            sFX_INIT_PARAM.btl_scene_FixedCamera2 = FF9StateSystem.Battle.FF9Battle.btl_scene.Info.FixedCamera2;
            BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next;
            Int32 num = 0;
            while (next != null)
            {
                SFX.InitBattleCharacter(ref sFX_INIT_PARAM, num, next);
                num++;
                next = next.next;
            }
        }
        else
        {
            sFX_INIT_PARAM.btl_scene_patAddr_putX = 0;
            sFX_INIT_PARAM.btl_scene_patAddr_putZ = 0;
            sFX_INIT_PARAM.btl_scene_FixedCamera1 = 0;
            sFX_INIT_PARAM.btl_scene_FixedCamera2 = 0;
            BTL_DATA btl_list = FF9StateSystem.Battle.FF9Battle.btl_list;
            Int32 i = 0;
            UInt16 num2 = 1;
            while (i < 8)
            {
                btl_list.btl_id = num2;
                btl_list.bi.player = (Byte)((i >= 4) ? 0 : 1);
                SFX.InitBattleCharacter(ref sFX_INIT_PARAM, i, btl_list);
                num2 = (UInt16)(num2 << 1);
                i++;
            }
        }
        GCHandle gCHandle = GCHandle.Alloc(sFX_INIT_PARAM, GCHandleType.Pinned);
        SFX.SFX_InitBattle(gCHandle.AddrOfPinnedObject());
        gCHandle.Free();
    }

    public static void InitBattleCharacter(ref SFX_INIT_PARAM param, Int32 index, BTL_DATA btl)
    {
        switch (index)
        {
            case 0:
                SFX.InitBtlData(ref param.btl_data_init0, btl);
                break;
            case 1:
                SFX.InitBtlData(ref param.btl_data_init1, btl);
                break;
            case 2:
                SFX.InitBtlData(ref param.btl_data_init2, btl);
                break;
            case 3:
                SFX.InitBtlData(ref param.btl_data_init3, btl);
                break;
            case 4:
                SFX.InitBtlData(ref param.btl_data_init4, btl);
                break;
            case 5:
                SFX.InitBtlData(ref param.btl_data_init5, btl);
                break;
            case 6:
                SFX.InitBtlData(ref param.btl_data_init6, btl);
                break;
            case 7:
                SFX.InitBtlData(ref param.btl_data_init7, btl);
                break;
        }
    }

    public static void InitBtlData(ref BTL_DATA_INIT init, BTL_DATA btl)
    {
        init.tar_bone = btl.tar_bone;
        init.btl_id = btl.btl_id;
        init.bi_player = btl.bi.player;
        init.bi_slot_no = btl.bi.slot_no;
        init.bi_slave = btl.bi.slave;
        init.bi_line_no = btl.bi.line_no;
        if (btl.bi.player != 0)
        {
            init.player_serial_no = FF9.btl_util.getSerialNumber(btl);
            init.player_equip = FF9.btl_util.getWeaponNumber(btl);
            init.player_wep_bone = FF9.btl_util.getPlayerPtr(btl).wep_bone;
            init.enemy_radius = 256;
            init.geo_radius = init.enemy_radius;
            init.geo_height = init.enemy_radius * 2;
            init.weapon_category = (Byte)(SFX.isDebugMode ? 0 : btl.weapon.Category);
            init.weapon_offset0 = (Int16)(SFX.isDebugMode ? 0 : btl.weapon.Offset1);
            init.weapon_offset1 = (Int16)(SFX.isDebugMode ? 0 : btl.weapon.Offset2);
            if (btl.is_monster_transform)
            {
                init.enemy_cam_bone0 = btl.monster_transform.cam_bone[0];
                init.enemy_cam_bone1 = btl.monster_transform.cam_bone[1];
                init.enemy_cam_bone2 = btl.monster_transform.cam_bone[2];
            }
            else
            {
                init.enemy_cam_bone0 = 0;
                init.enemy_cam_bone1 = 0;
                init.enemy_cam_bone2 = 0;
            }
        }
        else
        {
            init.player_serial_no = 0;
            init.player_equip = 0;
            init.player_wep_bone = 0;
            init.weapon_category = 0;
            init.weapon_offset0 = 0;
            init.weapon_offset1 = 0;
            if (!SFX.isDebugMode)
            {
                Byte typeNo = btlseq.instance.btl_list[4].typeNo;
                init.enemy_radius = FF9StateSystem.Battle.FF9Battle.btl_scene.MonAddr[typeNo].Radius;
                init.geo_radius = btlseq.instance.btl_list[4].radius;
                init.geo_height = init.enemy_radius * 2;
                ENEMY enemyPtr = FF9.btl_util.getEnemyPtr(btl);
                init.enemy_cam_bone0 = enemyPtr.et.cam_bone[0];
                init.enemy_cam_bone1 = enemyPtr.et.cam_bone[1];
                init.enemy_cam_bone2 = enemyPtr.et.cam_bone[2];
            }
            else
            {
                init.enemy_radius = 200;
                init.geo_radius = btl.radius;
                init.geo_height = init.enemy_radius * 2;
                init.enemy_cam_bone0 = 0;
                init.enemy_cam_bone1 = 0;
                init.enemy_cam_bone2 = 0;
            }
        }
    }

    public static void Begin(UInt16 flgs, Int16 arg0, Byte[] monbone, PSX_LIBGTE.VECTOR trgcpos)
    {
        SFX.request.flgs = flgs;
        SFX.request.arg0 = arg0;
        SFX.request.monbone0 = monbone[0];
        SFX.request.monbone1 = monbone[1];
        SFX.request.trgcpos_x = (Int16)trgcpos.vx;
        SFX.request.trgcpos_y = (Int16)trgcpos.vy;
        SFX.request.trgcpos_z = (Int16)trgcpos.vz;
        SFX.request.trgno = 0;
        SFX.request.rtrgno = 0;
    }

    public static void SetDebug()
    {
        SFX.request.exe.btl_id = 1;
        SFX.request.trg0.btl_id = 16;
        SFX.request.trgno = 1;
    }

    public static void SetExe(BTL_DATA exe)
    {
        SFX.SetReqParam(ref SFX.request.exe, exe);
    }

    public static void SetMExe(BTL_DATA mexe)
    {
        if (mexe != null)
        {
            SFX.SetReqParam(ref SFX.request.mexe, mexe);
        }
    }

    public static void SetTrg(BTL_DATA trg)
    {
        SFX.SetReqParam(ref SFX.request.trg0, trg);
        SFX.request.trgno = 1;
    }

    public static void SetTrg(BTL_DATA[] trg, SByte num)
    {
        for (Int32 i = 0; i < num; i++)
        {
            switch (i)
            {
                case 0:
                    SFX.SetReqParam(ref SFX.request.trg0, trg[i]);
                    break;
                case 1:
                    SFX.SetReqParam(ref SFX.request.trg1, trg[i]);
                    break;
                case 2:
                    SFX.SetReqParam(ref SFX.request.trg2, trg[i]);
                    break;
                case 3:
                    SFX.SetReqParam(ref SFX.request.trg3, trg[i]);
                    break;
                case 4:
                    SFX.SetReqParam(ref SFX.request.trg4, trg[i]);
                    break;
                case 5:
                    SFX.SetReqParam(ref SFX.request.trg5, trg[i]);
                    break;
                case 6:
                    SFX.SetReqParam(ref SFX.request.trg6, trg[i]);
                    break;
                case 7:
                    SFX.SetReqParam(ref SFX.request.trg7, trg[i]);
                    break;
            }
        }
        SFX.request.trgno = num;
    }

    public static void SetRTrg(BTL_DATA[] rtrg, SByte num)
    {
        for (Int32 i = 0; i < num; i++)
        {
            switch (i)
            {
                case 0:
                    SFX.SetReqParam(ref SFX.request.rtrg0, rtrg[i]);
                    break;
                case 1:
                    SFX.SetReqParam(ref SFX.request.rtrg1, rtrg[i]);
                    break;
                case 2:
                    SFX.SetReqParam(ref SFX.request.rtrg2, rtrg[i]);
                    break;
                case 3:
                    SFX.SetReqParam(ref SFX.request.rtrg3, rtrg[i]);
                    break;
            }
        }
        SFX.request.rtrgno = num;
    }

    public static void SetRTrgTest(BTL_DATA rtrg)
    {
        SFX.SetReqParam(ref SFX.request.rtrg0, rtrg);
        SFX.request.rtrgno = 1;
    }

    public static void SetReqParam(ref BTL_DATA_REQ req, BTL_DATA btl)
    {
        req.btl_id = (UInt16)(SFX.isDebugMode ? 1 : btl.btl_id);
    }

    public static void Play(Int32 effNum)
    {
        SFX.currentEffectID = effNum;
        SFX.streamIndex = 0;
        SFX.soundFPS = 4;
        SFX.soundCallCount = 0;
        SFX.cameraOffset = 0f;
        Int32 num = SFX.playParam[effNum] & 3;
        if (num == 0)
        {
            SFX.subOrder = SFX.defaultSubOrder;
        }
        else
        {
            SFX.subOrder = num - 1;
        }
        Int32 num2 = SFX.playParam[effNum] >> 2 & 3;
        if (num2 == 0)
        {
            SFX.colIntensity = SFX.defaultColIntensity;
        }
        else
        {
            SFX.colIntensity = num2 - 1;
        }
        SFX.pixelOffset = 0;
        SFX.colThreshold = (SFX.playParam[effNum] >> 29 & 1);
        SFX.addOrder = (SFX.playParam[effNum] >> 28 & 1);
        SFX.SoundClear();
        SFX.isRunning = true;
        SFX.frameIndex = 0;
        PSXTextureMgr.Reset();
        Int32 num3 = Marshal.SizeOf(SFX.request);
        Byte[] value = new Byte[num3];
        GCHandle gCHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
        Marshal.StructureToPtr(SFX.request, gCHandle.AddrOfPinnedObject(), false);
        SoundLib.LoadSfxSoundData(effNum);
        if (effNum == 435)
        {
            PSXTextureMgr.SpEff435();
        }
        String path = "SpecialEffects/ef" + effNum.ToString("D3");
		String[] efInfo;
        Byte[] binAsset = AssetManager.LoadBytes(path, out efInfo, false);
        if (binAsset != null)
        {
            GCHandle gCHandle2 = GCHandle.Alloc(binAsset, GCHandleType.Pinned);
            SFX.SFX_Play(effNum, gCHandle2.AddrOfPinnedObject(), binAsset.Length, gCHandle.AddrOfPinnedObject());
            gCHandle2.Free();
        }
        else
        {
            SFX.SFX_Play(effNum, (IntPtr)null, 0, gCHandle.AddrOfPinnedObject());
        }
        gCHandle.Free();
    }

    public static void SetCameraTarget(Vector3 pos, BTL_DATA exe, BTL_DATA trg)
    {
        SFX.SFX_SendFloatData(1, 0, pos.x, pos.y, pos.z);
        SFX.SFX_SendIntData(1, exe.btl_id, 0, 0);
        SFX.SFX_SendIntData(2, trg.btl_id, 0, 0);
    }

    public static void SetCamera(int cam)
    {
        int arg = 0;
        if (cam == 2)
        {
            arg = 150;
        }
        else
        {
            int battleMapIndex = FF9StateSystem.Battle.battleMapIndex;
            switch (battleMapIndex)
            {
                case 160:
                    arg = 160;
                    break;
                case 161:
                case 162:
                    IL_3B:
                    if (battleMapIndex != 155)
                    {
                        if (battleMapIndex == 303)
                        {
                            if (cam == 9)
                            {
                                arg = 120;
                            }
                        }
                    }
                    else
                    {
                        arg = 240;
                    }
                    break;
                case 163:
                    arg = 120;
                    break;
                default:
                    goto IL_3B;
            }
        }
        SFX.SFX_SendIntData(3, cam, arg, 0);
    }

    public static void SetEnemyCamera(BTL_DATA btl)
    {
        Int32 arg = 0;
        Int32 battleMapIndex = FF9StateSystem.Battle.battleMapIndex;
        switch (battleMapIndex)
        {
            case 299:
                goto IL_50;
            //case 300:
            //case 301:
            default:
                if (battleMapIndex == 4 || battleMapIndex == 73)
                {
                    goto IL_50;
                }
                if (battleMapIndex == 83)
                {
                    arg = 140;
                    goto IL_71;
                }
                if (battleMapIndex != 163)
                {
                    goto IL_71;
                }
                goto IL_66;
            case 302:
                goto IL_66;
        }
        IL_50:
        arg = 200;
        goto IL_71;
        IL_66:
        arg = 150;
        IL_71:
        SFX.SFX_SendIntData(4, btl.btl_id, arg, 0);
    }

    public static Int32 GetEffectOvRun()
    {
        return SFX.SFX_SendIntData(5, 0, 0, 0);
    }

    public static Int32 GetEffCamTrigger()
    {
        return SFX.SFX_SendIntData(6, 0, 0, 0);
    }

    public static void SetEffCamTrigger()
    {
        SFX.SFX_SendIntData(7, 0, 0, 0);
    }

    public static Int32 GetTaskMonsteraStartOK()
    {
        return SFX.SFX_SendIntData(8, 0, 0, 0);
    }

    public static void SetTaskMonsteraStart()
    {
        SFX.SFX_SendIntData(9, 0, 0, 0);
    }

    public static Int32 GetCameraPhase()
    {
        return SFX.SFX_SendIntData(10, 0, 0, 0);
    }

    public static void SetCameraPhase(Int32 phase)
    {
        SFX.SFX_SendIntData(11, phase, 0, 0);
    }

    public static Int32 GetEffectJTexUsed()
    {
        return SFX.SFX_SendIntData(12, 0, 0, 0);
    }

    public static void SoundClear()
    {
        SFX.seChantIndex = 0;
        SFX.seNormalIndex = 0;
        for (Int32 i = 0; i < SFX.MAX_INDEX; i++)
        {
            SFX.channel[i, 0] = -1;
        }
    }

    public static void SoundPlay(Int32 dno, Int32 attr, Int32 sce)
    {
        Int32 num = 12 + dno;
        Int32 num2;
        if (sce != 0)
        {
            num2 = SFX.currentEffectID;
            switch (num2)
            {
                case 225:
                    num += 12;
                    goto IL_1EE;
                //case 226:
                default:
                    if (num2 == 251)
                    {
                        num += 11;
                        goto IL_1EE;
                    }
                    if (num2 == 381)
                    {
                        if (SFX.soundCallCount >= 28)
                        {
                            num += 36;
                        }
                        else if (SFX.soundCallCount >= 20)
                        {
                            num += 20;
                        }
                        else if (SFX.soundCallCount >= 12)
                        {
                            num += 16;
                        }
                        else
                        {
                            num += 8;
                        }
                        goto IL_1EE;
                    }
                    if (num2 != 447)
                    {
                        goto IL_1EE;
                    }
                    num += 4;
                    goto IL_1EE;
                case 227:
                    num += 14;
                    goto IL_1EE;
            }
        }
        num2 = SFX.currentEffectID;
        if (num2 != 10)
        {
            if (num2 != 31)
            {
                if (num2 != 179)
                {
                    if (num2 != 227)
                    {
                        if (num2 != 237)
                        {
                            if (num2 != 261)
                            {
                                if (num2 != 312)
                                {
                                    if (num2 != 381)
                                    {
                                        if (num2 == 447)
                                        {
                                            if (SFX.soundCallCount >= 8)
                                            {
                                                num += 8;
                                            }
                                        }
                                    }
                                    else if (SFX.soundCallCount >= 28)
                                    {
                                        num += 28;
                                    }
                                    else if (SFX.soundCallCount >= 12)
                                    {
                                        num += 12;
                                    }
                                    else if (SFX.soundCallCount == 0)
                                    {
                                        SFX.soundFPS = -1;
                                    }
                                }
                                else
                                {
                                    num -= 7;
                                }
                            }
                            else if (SFX.soundCallCount == 0)
                            {
                                SFX.soundFPS = -3;
                            }
                        }
                        else if (dno != 0)
                        {
                            num -= 3;
                        }
                    }
                    else
                    {
                        num -= 2;
                        if (SFX.soundCallCount == 4)
                        {
                            SFX.soundFPS = -2;
                        }
                    }
                }
                else if (dno == 3)
                {
                    return;
                }
            }
            else if (dno != 0)
            {
                return;
            }
        }
        else
        {
            num -= 3;
        }
        IL_1EE:
        SFX.soundCallCount++;
        Single pitch;
        if (SFX.currentEffectID == 424)
        {
            pitch = 1.25f;
        }
        else
        {
            num2 = SFX.soundFPS;
            switch (num2 + 3)
            {
                case 0:
                    pitch = 0.725f;
                    goto IL_27A;
                case 1:
                    pitch = 0.85f;
                    goto IL_27A;
                case 2:
                    pitch = 0.76f;
                    goto IL_27A;
                case 6:
                    pitch = 0.75f;
                    goto IL_27A;
            }
            pitch = 1f;
        }
        IL_27A:
        SoundLib.PlaySfxSound(num, 1f, 0f, pitch);
        for (Int32 i = 0; i < SFX.MAX_INDEX; i++)
        {
            if (SFX.channel[i, 0] == -1)
            {
                SFX.channel[i, 0] = attr;
                SFX.channel[i, 1] = num;
                break;
            }
        }
    }

    public static void SoundPlayChant(Int32 dno, Int32 attr, Int32 position)
    {
        Int32 num = (dno & 16777215) * 3 + SFX.seChantIndex;
        if ((dno & 16777215) == 3)
        {
            if (SFX.seChantIndex % 3 == 0)
                SoundLib.PlaySoundEffect(REFLECT_SOUND_ID, 5f, 0f, 1f); // DEBUG: using 5f as sound volume because the current sound fix has a low volume... must fix that
        }
        else
        {
            SoundLib.PlaySfxSound(num, 1f, 0f, 1f);
        }
        SFX.seChantIndex++;
        for (Int32 i = 0; i < SFX.MAX_INDEX; i++)
        {
            if (SFX.channel[i, 0] == -1)
            {
                SFX.channel[i, 0] = attr;
                SFX.channel[i, 1] = num;
                break;
            }
        }
    }

    public static void SoundStop(Int32 eff, Int32 attr)
    {
        for (Int32 i = 0; i < SFX.MAX_INDEX; i++)
        {
            if (SFX.channel[i, 0] == attr)
            {
                SoundLib.StopSfxSound(SFX.channel[i, 1]);
                SFX.channel[i, 0] = -1;
            }
        }
    }

    public static void StreamPlay(Int32 rflg)
    {
        Int32 num = 12;
        Int32 num2 = SFX.currentEffectID;
        switch (num2)
        {
            case 225:
                num += 16;
                break;
            case 226:
                num += 16;
                break;
            case 227:
                num += 26 + SFX.streamIndex;
                break;
            default:
                if (num2 != 210)
                {
                    if (num2 != 211)
                    {
                        if (num2 != 179)
                        {
                            if (num2 != 186)
                            {
                                if (num2 != 251)
                                {
                                    if (num2 != 261)
                                    {
                                        if (num2 != 276)
                                        {
                                            if (num2 != 381)
                                            {
                                                if (num2 != 415)
                                                {
                                                    return;
                                                }
                                                num += 6;
                                            }
                                            else
                                            {
                                                num += 40 + SFX.streamIndex;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        num += 16;
                                    }
                                }
                                else
                                {
                                    num += 19;
                                }
                            }
                            else
                            {
                                num += 9;
                            }
                        }
                        else
                        {
                            num += 12;
                        }
                    }
                    else
                    {
                        num += 8;
                    }
                }
                else
                {
                    num += 14;
                }
                break;
        }
        SFX.streamIndex++;
        SoundLib.PlaySfxSound(num, 1.3f, 0f, 1f);
        for (Int32 i = 0; i < SFX.MAX_INDEX; i++)
        {
            if (SFX.channel[i, 0] == -1)
            {
                SFX.channel[i, 0] = -2;
                SFX.channel[i, 1] = num;
                break;
            }
        }
    }

    public const Int32 DEBUG_EFFECT_ID = 126;

    public const Int32 effMax = 511;

    public const Single ONE = 4096f;

    public const Single EUL2PSX = 11.3777781f;

    public const Single PSX2EUL = 0.087890625f;

    public const Int32 COMMAND_GET_POSITION = 1;

    public const Int32 COMMAND_SET_POSITION = 2;

    public const Int32 COMMAND_GET_ROTATE = 3;

    public const Int32 COMMAND_SET_ROTATE = 4;

    public const Int32 COMMAND_GET_SCALE = 5;

    public const Int32 COMMAND_SET_SCALE = 6;

    public const Int32 COMMAND_GET_GEO_FLAG = 7;

    public const Int32 COMMAND_GET_AUTO_POTION_COMMAND = 8;

    public const Int32 COMMAND_GET_MOTION_FRAME_MAX = 9;

    public const Int32 COMMAND_GET_MOTION_FRAME = 10;

    public const Int32 COMMAND_SET_MOTION_FRAME = 11;

    public const Int32 COMMAND_SET_MOTION = 12;

    public const Int32 COMMAND_STOP_MOTION = 13;

    public const Int32 COMMAND_GET_MATRIX = 14;

    public const Int32 COMMAND_IS_TARGET = 15;

    public const Int32 COMMAND_SET_DEFAULT_IDLE = 16;

    public const Int32 COMMAND_GET_DISAPPEAR = 17;

    public const Int32 COMMAND_SET_DISAPPEAR = 18;

    public const Int32 COMMAND_SHOW_WEAPON = 19;

    public const Int32 COMMAND_CHECK_STATUS = 20;

    public const Int32 COMMAND_GET_CMD_IDLE = 21;

    public const Int32 COMMAND_GET_SLAVE = 22;

    public const Int32 COMMAND_EXEC_VFX = 23;

    public const Int32 COMMAND_BTL_2D_REQ = 24;

    public const Int32 COMMAND_SHOW_MESH = 25;

    public const Int32 COMMAND_GET_MESH_COUNT = 26;

    public const Int32 COMMAND_SET_PRESENT_COLOR = 27;

    public const Int32 COMMAND_EYE_BLINK = 28;

    public const Int32 COMMAND_SFX_BATTLE = 29;

    public const Int32 COMMAND_SFX_PLAYER_ATTACK = 30;

    public const Int32 COMMAND_SFX_PLAY_HIT = 31;

    public const Int32 COMMAND_SFX_PLAY = 32;

    public const Int32 COMMAND_SET_SPMOTION = 33;

    public const Int32 COMMAND_LOAD_IMAGE = 100;

    public const Int32 COMMAND_STORE_IMAGE = 101;

    public const Int32 COMMAND_MOVE_IMAGE = 102;

    public const Int32 COMMAND_INCREMENT_BATTLE_SEQ = 110;

    public const Int32 COMMAND_GET_CAMERA_CONFIG = 111;

    public const Int32 COMMAND_LBOSS_FLAG_ENABLE = 112;

    public const Int32 COMMANS_EFFECT_TILE = 113;

    public const Int32 COMMAND_SET_CURSOR = 114;

    public const Int32 COMMAND_GET_CURSOR = 115;

    public const Int32 COMMAND_SFX_PAUSE = 116;

    public const Int32 COMMAND_GET_BG_INTENSITY = 117;

    public const Int32 COMMAND_SET_BG_INTENSITY = 118;

    public const Int32 COMMAND_VIBRATE = 119;

    public const Int32 COMMAND_CREATE_TEXTURE = 120;

    public const Int32 COMMAND_GET_BONUS = 121;

    public const Int32 COMMAND_GET_START_TYPE = 122;

    public const Int32 COMMAND_GET_ALIVE_COUNT = 123;

    public const Int32 COMMAND_GET_CAMERA_NUMBER = 124;

    public const Int32 COMMAND_SET_FPS = 125;

    public const Int32 S_ChrTAnm_PLAY = 0;

    public const Int32 S_ChrTAnm_PAUSE = 1;

    public const Int32 S_ChrTAnm_RET = 2;

    public const Single FAR_Z = 65535f;

    public const UInt16 S_REQFLG_NOEXEACT = 1;

    public const UInt16 S_REQFLG_ITEMC = 2;

    public const UInt16 S_REQFLG_MGC_SWORD = 4;

    public const UInt16 S_REQFLG_MGC_OR_SUMMON = 8;

    public const UInt16 S_REQFLG_REF_NEXT = 16;

    public const Int32 SEND_FLOAT_CAMERA_TARGET = 1;

    public const Int32 REFLECT_SOUND_ID = 350; // Thanks to LovelsDarkness and DoomOyster

    public static Int32 currentEffectID;

    public static Boolean isDebugAutoPlay;

    public static Boolean isDebugPng;

    public static Boolean isDebugViewport;

    public static Boolean isDebugLine;

    public static Boolean isDebugCam;

    public static Boolean isDebugMode;

    public static Boolean isDebugFillter;

    public static Int32 isDebugMeshIndex;

    public static Boolean isRunning;

    public static Int32 frameIndex;

    public static Boolean isSystemRun;

    public static Boolean isUpdated;

    public static Single screenWidth;

    public static Single screenHeight;

    public static Single screenWidthOffset;

    public static Single screenRatio;

    public static Single screenWidthRatio;

    public static Single screenHeightRatio;

    public static Int32[] effTeble;

    public static Int32 fade;

    public static PSX_LIBGTE.SVECTOR cpos;

    public static PSX_LIBGTE.SVECTOR cpos_org;

    public static PSX_LIBGTE.SVECTOR cscl;

    public static PSX_LIBGTE.SVECTOR cscl_org;

    public static PSX_LIBGTE.SVECTOR crot;

    public static PSX_LIBGTE.SVECTOR crot_org;

    public static Single fxNearZ;

    public static Single fxFarZ;

    public static Single cameraOffset;

    public static Int32 S1;

    public static Int32 S2;

    public static Int32 S3;

    public static Int32 C1;

    public static Int32 C2;

    public static Int32 C3;

    public static Int32 M1;

    public static Int32 M2;

    public static Int32 A1;

    public static Int32 O1;

    public static Int32[] playParam;

    public static S_Eff_Req_Org request;

    public static Int32 subOrder;

    public static Int32 defaultSubOrder;

    public static Int32 colIntensity;

    public static Int32 defaultColIntensity;

    public static Int32 pixelOffset;

    public static Int32 colThreshold;

    public static Int32 addOrder;

    public static Int32 streamIndex;

    public static Int32 soundFPS;

    public static Int32 soundCallCount;

    public static Single TEST_SPEED;

    public static Int32 MAX_INDEX;

    public static Int32[,] channel;

    public static Int32 seChantIndex;

    public static Int32 seNormalIndex;

    public struct PSXMAT
    {
        public unsafe fixed Int16 r[9];

        public Int16 pad;

        public unsafe fixed Int32 t[3];
    }

    public unsafe delegate Int32 Callback(Int32 code, Int32 arg0, Int32 arg1, Int32 arg2, Int32 arg3, void* p);

    public static Int32 preventStepInOut = -1;
}