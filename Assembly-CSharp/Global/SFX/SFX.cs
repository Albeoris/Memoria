using AOT;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public static class SFX
{
    static SFX()
    {
        SFX.fxNearZ = 100f;
        SFX.fxFarZ = 65536f;
        SFX.S1 = 1; // Rendering order: Opa then Add then Sub by default (playParam == 0 defaults to SFX.S3)
        SFX.S2 = 2; // SFX.Si => Sub renderer goes in i-th position
        SFX.S3 = 3;
        SFX.C1 = 4; // Color intensity of SFXMesh: SFX.C1 => normal colors (default)
        SFX.C2 = 8; // SFX.C2 => Colors x 1.5
        SFX.C3 = 12; // SFX.C3 => Colors x 2
        SFX.M1 = 1073741824; // Unused
        SFX.M2 = -2147483648;
        SFX.A1 = 536870912; // SFX.A1 => Add renderer goes before Opa
        SFX.O1 = 268435456; // Color threashold: 0.05f (default) or 0.0295f (SFX.O1)
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
        SFX.IsDebugMesh = false; // Very slow
        SFX.IsDebugObjMesh = false;
        SFX.isDebugPrintCode = false;
        SFX.isDebugViewport = false;
        SFX.isDebugLine = false;
        SFX.isDebugCam = false;
        SFX.isDebugMode = false;
        SFX.isDebugFilter = true;
        SFX.isDebugMeshIndex = 0;
        SFX.isRunning = false;
        SFX.currentEffectID = SpecialEffect.Special_No_Effect;
        SFX.frameIndex = 0;
        SFX.isSystemRun = false;
        SFX.isUpdated = false;
        SFX.screenWidth = 1f;
        SFX.screenHeight = 1f;
        SFX.screenWidthOffset = 0f;
        SFX.screenRatio = 1f;
        SFX.screenWidthRatio = 1f;
        SFX.screenHeightRatio = 1f;
    }

    public static Boolean IsRunning()
    {
        if (Configuration.Battle.SFXRework)
            return UnifiedBattleSequencer.runningActions.Count > 0;
        return SFX.isRunning;
    }

    public static void UpdatePlugin()
    {
        try
        {
            if (SFX.isSystemRun)
            {
                SFX.UpdateScreenSize();
                // SFX_Update is managed by UnifiedBatteSequencer and SFXDataMesh in SFXRework mode
                if (!Configuration.Battle.SFXRework && !((SFX.IsDebugObjMesh || SFX.IsDebugMesh) && SFX.isUpdated))
                {
                    SFX.isRunning = SFX.SFX_Update(ref SFX.frameIndex);
                    if (!SFX.isRunning)
                    {
                        if (SFX.IsDebugMesh)
                            SFXRender.SaveSFXDataMeshes();
                        SFX.currentEffectID = SpecialEffect.Special_No_Effect;
                    }
                    if (SFX.isRunning)
                    {
                        SFX.isUpdated = true;
                        PSXTextureMgr.isCaptureBlur = true;
                        if (SFX.preventStepInOut >= 0 && SFX.frameIndex - SFX.preventStepInOut > 2)
                            SFX.preventStepInOut = -1;
                        if (SFX.currentEffectID == SpecialEffect.Ark__Full)
                        {
                            if (SFX.frameIndex == 1004)
                                SFX.subOrder = 2;
                            if (SFX.frameIndex == 1193)
                                SFX.subOrder = 0;
                        }
                        if (SFX.currentEffectID == SpecialEffect.Sinkhole)
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
                }
                vib.VIB_service();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to update SFX plugin.");
        }
    }

    public static void LateUpdatePlugin()
    {
        // SFX_LateUpdate and SFXRender.Update are managed by UnifiedBatteSequencer and SFXDataMesh in SFXRework mode
        if (!Configuration.Battle.SFXRework && SFX.isSystemRun && SFX.isUpdated)
        {
            SFX.isUpdated = false;
            SFX.SFX_LateUpdate();
            SFXRender.Update();
        }
    }

    public static void PostRender()
    {
        // SFXRender.Render is managed by UnifiedBatteSequencer and SFXDataMesh in SFXRework mode
        if (SFX.isSystemRun)
        {
            SFX.ResetViewPort();
            if (!Configuration.Battle.SFXRework && SFX.isRunning)
                SFXRender.Render();
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
                SFX.screenWidth = Screen.height * PSX.PSX_SCREEN_RATIO;
                SFX.screenWidthOffset = (Screen.width - SFX.screenWidth) * 0.5f;
            }
            else
            {
                SFX.screenHeight = Screen.width;
                SFX.screenWidth = Screen.width * PSX.PSX_SCREEN_RATIO;
                SFX.screenWidthOffset = (Screen.height - SFX.screenWidth) * 0.5f;
            }
        }
    }

    private class DLLMethods
    {
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
    }
    public static void SFX_InitSystem(Callback method)
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_InitSystem " + method.ToString());
        DLLMethods.SFX_InitSystem(method);
    }
    public static void SFX_StartPlungeCamera(IntPtr btlseq, Int32 btlseqLen, Int32 camOffset, Int32 projOffset)
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_StartPlungeCamera " + btlseqLen + " " + camOffset + " " + projOffset);
        DLLMethods.SFX_StartPlungeCamera(btlseq, btlseqLen, camOffset, projOffset);
    }
    public static void SFX_SkipCameraAnimation(Int32 skip)
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_SkipCameraAnimation " + skip);
        DLLMethods.SFX_SkipCameraAnimation(skip);
    }
    public static void SFX_InitBattle(IntPtr param)
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_InitBattle ");
        DLLMethods.SFX_InitBattle(param);
    }
    public static Boolean SFX_Update(ref Int32 frameIndex)
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_Update " + frameIndex);
        return DLLMethods.SFX_Update(ref frameIndex);
    }
    public static void SFX_LateUpdate()
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_LateUpdate ");
        DLLMethods.SFX_LateUpdate();
    }
    public static IntPtr SFX_UpdateCamera(Int32 isDebug)
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_UpdateCamera " + isDebug);
        return DLLMethods.SFX_UpdateCamera(isDebug);
    }
    public static void SFX_MoveFreeCamera(Int32 type, Int32 x, Int32 y)
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_MoveFreeCamera " + type + " " + x + " " + y);
        DLLMethods.SFX_MoveFreeCamera(type, x, y);
    }
    public static Int32 SFX_SendFloatData(Int32 type, Int32 btl_id, Single arg0, Single arg1, Single arg2)
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_SendFloatData " + type + " " + btl_id + " " + arg0 + " " + arg1 + " " + arg2);
        return DLLMethods.SFX_SendFloatData(type, btl_id, arg0, arg1, arg2);
    }
    public static Int32 SFX_SendIntData(Int32 type, Int32 arg0, Int32 arg1, Int32 arg2)
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_SendIntData " + type + " " + arg0 + " " + arg1 + " " + arg2);
        return DLLMethods.SFX_SendIntData(type, arg0, arg1, arg2);
    }
    public static void SFX_Play(Int32 effnum, IntPtr bin, Int32 size, IntPtr req)
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_Play " + effnum + " " + size);
        DLLMethods.SFX_Play(effnum, bin, size, req);
    }
    public static Boolean SFX_BeginRender()
    {
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] SFX_BeginRender");
        return DLLMethods.SFX_BeginRender();
    }
    public static IntPtr SFX_GetPrim(ref Int32 otz)
    {
        return DLLMethods.SFX_GetPrim(ref otz);
    }

    [MonoPInvokeCallback(typeof(Callback))]
    public static unsafe Int32 BattleCallback(Int32 fullCode, Int32 arg0, Int32 arg1, Int32 arg2, Int32 arg3, void* p)
    {
        if (SFX.hijackedCallback != null)
            return SFX.hijackedCallback(fullCode, arg0, arg1, arg2, arg3, p);
        Int32 code = fullCode >> 24;
        if (SFX.isDebugPrintCode)
            Log.Message("[SFX] Callback " + (SFX.COMMAND)code + " " + arg0 + " " + arg1 + " " + arg2 + " " + arg3 + " (" + (fullCode & 255) + ")");
        if (code == 100) // Load the rectangle [x, y, w, h] = [arg0, arg1, arg2, arg3] from a PSX-like Vram (TIM format)
        {
            PSXTextureMgr.LoadImage(arg0, arg1, arg2, arg3, (UInt16*)p);
            return 0;
        }
        if (!SFX.isSystemRun)
            return 0;
        if (SFX.isDebugMode)
            return SFX.DebugRoomCallback(code, arg0, arg1, arg2, arg3, p);
        switch (code)
        {
            case 32: // Play/Stop Sound
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
            case 101: // Pass the Vram rectangle back to FF9SpecialEffectPlugin.dll
                PSXTextureMgr.StoreImage(arg0, arg1, arg2, arg3, (UInt16*)p);
                return 0;
            case 102: // Update the rectangle [arg0, arg1, p[2], p[3]] with the image of the rectangle [p[0], p[1], p[2], p[3]]
                PSXTextureMgr.MoveImage(arg0, arg1, (Int16*)p);
                return 0;
            case 110: // btl_seq controls the end of battles (game over, victory animation, fleeing...)
                FF9StateSystem.Battle.FF9Battle.btl_seq++;
                return 0;
            case 111: // "Battle Camera: Auto / Fixed"
                return (Int32)FF9StateSystem.Settings.cfg.camera;
            case 112: // Some flag used in AI scripts (Steiner's Moonlight Slash, Kuja's Transform, Trance Kuja's Ultima (Crystal) and Necron's death)
                FF9StateSystem.EventState.gEventGlobal[199] |= 16;
                return 0;
            case 113: // Display Ability Casting Name
            {
                CMD_DATA curCmdPtr = btl_util.getCurCmdPtr();
                if (curCmdPtr != null)
                    UIManager.Battle.SetBattleCommandTitle(curCmdPtr);
                return 0;
            }
            case 114: // Show/Hide Cursor
                if (arg0 != 0)
                    FF9StateSystem.Battle.FF9Battle.cmd_status |= 2;
                else
                    FF9StateSystem.Battle.FF9Battle.cmd_status &= 0xFFFD;
                return 0;
            case 115: // Is Cursor Shown
                return ((FF9StateSystem.Battle.FF9Battle.cmd_status & 2) == 0) ? 0 : 1;
            case 116:
                return 0;
            case 117: // Get Background Intensity, for fading backgrounds (also used to fake a light dim)
                return battlebg.nf_GetBbgIntensity();
            case 118: // Set Background Intensity
                battlebg.nf_SetBbgIntensity((Byte)arg0);
                return 0;
            case 119: // Controller Vibration
                switch (arg0)
                {
                    case 0: // Stop Vibration
                        vib.VIB_purge();
                        break;
                    case 1: // Vibrate (full parameters)
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
                    case 2: // Vibrate
                        vib.VIB_setActive(false);
                        vib.VIB_setTrackActive(1, vib.VIB_SAMPLE_LO, true);
                        vib.VIB_setTrackActive(1, vib.VIB_SAMPLE_HI, true);
                        vib.VIB_vibrate(1);
                        break;
                }
                return 0;
            case 120:   // Take a screenshot of the background's ground in the Vram:
                        // Texture X -> p[0] & 0x3FC0
                        // Texture Y -> p[1] & 0x100
                        // Screen Position X -> p[0] & 0x3F + p[2] / 2
                        // Screen Position Y -> p[1] & 0xFF + p[3] / 2
                        // Camera shift -> (p[4], p[5], p[6])
                PSXTextureMgr.isBgCapture = true;
                Marshal.Copy((IntPtr)p, PSXTextureMgr.bgParam, 0, 7);
                return 0;
            case 121: // Return btl_id of player characters that were not removed from the battle (with Snort / Swallow)
                return battle.btl_bonus.member_flag;
            case 122: // Back Attack, Preemptive, Normal
                return (Byte)FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType;
            case 123: // Return the number of targetable player characters
            {
                Int32 validPlayerTarget = 0;
                for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
                    if (next.bi.player != 0 && !btl_stat.CheckStatus(next, BattleStatus.Death | BattleStatus.Jump))
                        validPlayerTarget++;
                return validPlayerTarget;
            }
            case 124: // Return the current battle camera index (these cameras are predefined per btlseq)
                return FF9StateSystem.Battle.FF9Battle.seq_work_set.CameraNo;
            case 125: // Set Sound Pitch (default is 4 for a pitch of 1f)
                SFX.soundFPS = arg0;
                return 0;
        }
        return BattleCallbackWithBtl(code, arg0, arg1, arg2, arg3, p, fullCode & 255);
    }

    private static unsafe Int32 BattleCallbackWithBtl(Int32 code, Int32 arg0, Int32 arg1, Int32 arg2, Int32 arg3, void* p, Int32 btlid)
    {
        BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next;
        while (next.next != null)
        {
            if (next.btl_id == btlid)
                break;
            next = next.next;
        }
        if (/*next == null || */next.btl_id == 0)
            return (code != 9) ? 0 : 1;
        Boolean correctBtlid = (int)next.btl_id == btlid;
        switch (code)
        {
            case 1: // Get Position
                switch (arg0)
                {
                    case 0: // Current 3D
                        *(Int16*)p = (Int16)next.pos.x;
                        *(Int16*)((Byte*)p + 2) = (Int16)(-(Int16)(next.pos.y + next.attachOffset));
                        *(Int16*)((Byte*)p + 4) = (Int16)next.pos.z;
                        break;
                    case 1: // Base pos 3D
                        *(Int16*)p = (Int16)next.base_pos.x;
                        *(Int16*)((Byte*)p + 2) = (Int16)(-(Int16)(next.base_pos.y + next.attachOffset));
                        *(Int16*)((Byte*)p + 4) = (Int16)next.base_pos.z;
                        break;
                    case 2: // Current 2D
                        *(Int16*)p = (Int16)next.pos.x;
                        *(Int16*)((Byte*)p + 4) = (Int16)next.pos.z;
                        break;
                    case 3: // Base pos 2D
                        *(Int16*)p = (Int16)next.base_pos.x;
                        *(Int16*)((Byte*)p + 4) = (Int16)next.base_pos.z;
                        break;
                }
                break;
            case 2: // Set Position
                if (SFX.preventStepInOut >= 0 && next.btl_id == SFX.request.exe.btl_id)
                {
                    SFX.preventStepInOut = SFX.frameIndex;
                    break;
                }
                next.pos.x = *(Int16*)p;
                next.pos.y = -(*(Int16*)((Byte*)p + 2) + (Single)next.attachOffset);
                next.pos.z = *(Int16*)((Byte*)p + 4);
                break;
            case 3: // Get Angles
                switch (arg0)
                {
                    case 0: // Difference between base and current
                        *(Int16*)p = (Int16)((next.evt.rotBattle.eulerAngles.x - next.rot.eulerAngles.x) * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)((next.evt.rotBattle.eulerAngles.y - next.rot.eulerAngles.y) * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((next.evt.rotBattle.eulerAngles.z - next.rot.eulerAngles.z) * 11.3777781f);
                        break;
                    case 1: // Current
                        *(Int16*)p = (Int16)(next.rot.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)(next.rot.eulerAngles.y * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((next.rot.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                    case 2: // Base
                        *(Int16*)p = (Int16)(next.evt.rotBattle.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 2) = (Int16)(next.evt.rotBattle.eulerAngles.y * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((next.evt.rotBattle.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                    case 3: // Base, no orientation (horizontal angle)
                        *(Int16*)p = (Int16)(next.evt.rotBattle.eulerAngles.x * 11.3777781f);
                        *(Int16*)((Byte*)p + 4) = (Int16)((next.evt.rotBattle.eulerAngles.z - 180f) * 11.3777781f);
                        break;
                }
                break;
            case 4: // Set Angles
                switch (arg0)
                {
                    case 0: // To base angle
                        next.rot.eulerAngles = next.evt.rotBattle.eulerAngles;
                        break;
                    case 1: // To base angle + 180 degree
                        next.rot.eulerAngles = new Vector3(next.rot.eulerAngles.x, next.evt.rotBattle.eulerAngles.y + 180f, next.rot.eulerAngles.z);
                        break;
                    case 2: // To value
                        next.rot.eulerAngles = new Vector3(*(Int16*)p * 0.087890625f, *(Int16*)((Byte*)p + 2) * 0.087890625f - 180f, *(Int16*)((Byte*)p + 4) * 0.087890625f + 180f);
                        break;
                }
                break;
            case 5: // Get Size
                *(Int32*)p = (Int32)(next.gameObject.transform.localScale.x * 4096f);
                *(Int32*)((Byte*)p + 4) = (Int32)(next.gameObject.transform.localScale.y * 4096f);
                *(Int32*)((Byte*)p + 8) = (Int32)(next.gameObject.transform.localScale.z * 4096f);
                break;
            case 6: // Set Size
                if ((arg0 & 128) == 0)
                    geo.geoScaleReset(next);
                else
                    next.flags |= geo.GEO_FLAGS_SCALE;
                if ((arg0 & 1) == 1)
                    next.gameObject.transform.localScale = new Vector3(arg1 / 4096f, arg2 / 4096f, arg3 / 4096f);
                break;
            case 7: // Get geo flags
                return next.flags;
            case 8: // Is using Auto-Potion
                if (FF9StateSystem.Battle.FF9Battle.cur_cmd != null)
                    return (FF9StateSystem.Battle.FF9Battle.cur_cmd.cmd_no != BattleCommandId.AutoPotion || !btl_cmd.CheckUsingCommand(next.cmd[0])) ? 0 : 1;
                return 0;
            case 9: // Get Current Animation's frame count
            {
                UInt16 frameCount = (UInt16)GeoAnim.geoAnimGetNumFrames(next);
                if (frameCount == 0)
                    frameCount = 1;
                return frameCount;
            }
            case 10: // Get Current Animation's current frame
                return next.evt.animFrame;
            case 11: // Set Current Animation's current frame
            {
                UInt16 frameMax = (UInt16)GeoAnim.geoAnimGetNumFrames(next);
                next.evt.animFrame = frameMax <= arg0 ? (Byte)(frameMax - 1) : (Byte)arg0;
                break;
            }
            case 12: // Set Current Animation
                if (arg0 == -1)
                {
                    arg0 = next.bi.def_idle;
                    CMD_DATA curCmd = FF9StateSystem.Battle.FF9Battle.cur_cmd;
                    if (curCmd != null && curCmd.info.cmd_motion)
                    {
                        curCmd.info.cmd_motion = false;
                        btl_mot.EndCommandMotion(curCmd);
                    }
                }
                if (arg0 < 6 || next.bi.player != 0)
                {
                    // Use Freya's jump animations instead of her CHANT/MAGIC animations
                    if ((SFX.currentEffectID == SpecialEffect.Jump || SFX.currentEffectID == SpecialEffect.Spear) && (arg0 == (Byte)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_CHANT || arg0 == (Byte)BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT || arg0 == (Byte)BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC))
                    {
                        String goName = next.gameObject.name;
                        goName = goName.Trim();
                        if (goName.CompareTo("192(Clone)") == 0 || goName.CompareTo("585(Clone)") == 0)
                        {
                            if (arg0 == (Byte)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_CHANT)
                                btl_mot.setMotion(next, "ANH_MAIN_B0_011_110");
                            if (arg0 == (Byte)BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT)
                                btl_mot.setMotion(next, "ANH_MAIN_B0_011_111");
                            if (arg0 == (Byte)BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC)
                                btl_mot.setMotion(next, "ANH_MAIN_B0_011_112");
                            next.animFlag |= (UInt16)EventEngine.afLoop;
                            next.evt.animFrame = 0;
                            break;
                        }
                    }
                    if ((arg0 == 29 || arg0 == 30) && next.is_monster_transform && next.btl_id == SFX.request.exe.btl_id)
                    {
                        SFX.preventStepInOut = SFX.frameIndex;
                        break;
                    }
                    btl_mot.setMotion(next, (Byte)arg0);
                    next.animFlag |= (UInt16)EventEngine.afLoop;
                    next.evt.animFrame = 0;
                }
                break;
            case 13: // Stop Animation
            {
                // Handle Ultima (Pandemonium / Crystal World) such that it doesn't freeze the characters
                // In order to use the SFX Ultima as normal spells, this fix is required
                //  + adding two effect points (damage point & figure point) in the efXXX file
                //  + reset the BBG transparency at the end ("Set battle scene transparency 255 5" in Hades Workshop)
                //  + make sure that all the characters are shown at the end (removing all the lines "Show/hide characters" in Hades Workshop has a good-looking result)
                // Also, the caster moves away when using SFX 384 (Ultima in Crystal World), so either reset the caster's position at the end of the sequencing or use the Pandemonium version
                if ((SFX.currentEffectID == SpecialEffect.Special_Ultima_Terra || SFX.currentEffectID == SpecialEffect.Special_Ultima_Memoria) && FF9StateSystem.Battle.FF9Battle.btl_phase == FF9StateBattleSystem.PHASE_NORMAL)
                    return next.bi.stop_anim;
                Byte stop_anim = next.bi.stop_anim;
                next.bi.stop_anim = (Byte)arg0;
                return stop_anim;
            }
            case 14: // Get Bone Stance
            {
                Matrix4x4 matrix4x;
                try
                {
                    matrix4x = next.gameObject.transform.GetChildByName("bone" + arg1.ToString("D3")).localToWorldMatrix;
                    if (next.bi.disappear != 0 && next.gameObject.transform.localPosition.y == -10000f)
                        matrix4x.m13 += 10000f; // The flickering "fix" from BTL_DATA.SetDisappear
                }
                catch (NullReferenceException)
                {
                    matrix4x = Matrix4x4.identity;
                }
                switch (arg0)
                {
                    case 0: // Get Bone Position
                        *(Int16*)p = (Int16)matrix4x.m03;
                        *(Int16*)((Byte*)p + 2) = (Int16)(-(Int16)matrix4x.m13);
                        *(Int16*)((Byte*)p + 4) = (Int16)matrix4x.m23;
                        break;
                    case 1: // Get Bone Height
                        return -(Int32)matrix4x.m13;
                    case 2: // Get Bone Orientation & Position
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
                        tptr[1] = -(Int32)matrix4x.m13;
                        tptr[2] = (Int32)matrix4x.m23;
                        break;
                }
                break;
            }
            case 15: // Is Targetable
                return next.bi.target;
            case 16: // Reset Stand Animation
                btl_mot.SetDefaultIdle(next);
                break;
            case 17: // Is Hidden (no model, no targeting)
                return next.bi.disappear != 0 ? 1 : 0;
            case 18: // Set Hidden On/Off
                if (SFX.currentEffectID == SpecialEffect.Sinkhole)
                {
                    next.bi.stop_anim = 1;
                    foreach (Material material in battlebg.GetShaders(2))
                        material.SetInt("_ZWrite", 1); // Turn the ground into an opaque material
                }
                else
                {
                    Byte priority = (Byte)(SFX.currentEffectID == SpecialEffect.Jump || SFX.currentEffectID == SpecialEffect.Spear || SFX.currentEffectID == SpecialEffect.Spear_Trance ? 2 :
                        SFX.currentEffectID == SpecialEffect.Special_Sealion_Engage ? 3 : 1);
                    next.SetDisappear(arg0 != 0, priority);
                    if (arg0 == 0)
                        btlseq.DispCharacter(next, false);
                }
                break;
            case 19: // Show/Hide Weapon
                for (Int32 i = 0; i < next.weaponMeshCount; i++)
                    if (arg0 != 0)
                        geo.geoWeaponMeshShow(next, i);
                    else
                        geo.geoWeaponMeshHide(next, i);
                break;
            case 20: // Status
                switch (arg0)
                {
                    case 0: // Has Status
                        return ((next.stat.cur & (BattleStatus)(arg2 << 16 | arg1)) == 0u) ? 0 : 1;
                    case 1: // Has Status or Permanent Status
                    {
                        BattleStatus status = (BattleStatus)(arg2 << 16 | arg1);
                        if (SFX.currentEffectID == SpecialEffect.Roulette && (status & BattleStatus.Death) != 0u && !correctBtlid)
                            return 1;
                        return btl_stat.CheckStatus(next, status) ? 1 : 0;
                    }
                    case 2: // Remove many statuses
                        btl_stat.RemoveStatuses(new BattleUnit(next), ~(BattleStatus.EasyKill | BattleStatus.Death | BattleStatus.LowHP | BattleStatus.Stop | BattleStatus.ChangeStat));
                        break;
                    case 3: // Reset Statuses
                        btl_stat.InitStatus(next);
                        break;
                }
                break;
            case 21: // Has a main command queued (Ready)
                return next.bi.cmd_idle;
            case 22: // Is attached to another enemy
                return next.bi.slave;
            case 23: // Effect Point
                if (btl_util.getCurCmdPtr() != null && SFX.frameIndex > SFX.effectPointFrame)
                {
                    btl_util.getCurCmdPtr().info.effect_counter++;
                    SFX.effectPointFrame = SFX.frameIndex;
                }
                btl_cmd.ExecVfxCommand(next);
                break;
            case 24: // Figure Point
                btl2d.Btl2dReq(next);
                break;
            case 25: // Show/Hide mesh
                switch (arg0)
                {
                    case 0: // Hide all
                        btl_mot.HideMesh(next, 65535, false);
                        break;
                    case 1: // Hide for Vanish
                        btl_mot.HideMesh(next, next.mesh_banish, true);
                        break;
                    case 2: // Show all
                        btl_mot.ShowMesh(next, 65535, false);
                        break;
                    case 3: // Semi-transparent fade, light
                        SFX.fade = arg1;
                        if ((next.flags & geo.GEO_FLAGS_RENDER) != 0 && (next.flags & geo.GEO_FLAGS_CLIP) == 0)
                            btl_stat.GeoAddColor2DrawPacket(next.gameObject, (Int16)(SFX.fade - 128), (Int16)(SFX.fade - 128), (Int16)(SFX.fade - 128));
                        break;
                    case 4: // Semi-transparent fade, severe (a typical fading goes from arg1 == 128 to 0 forth and back)
                    {
                        Boolean weap = arg1 < 256;
                        SFX.fade = arg1;
                        if (!weap)
                            arg1 -= 256;
                        if (next.bi.player != 0 && next.weapon_geo != null && weap && (next.weaponFlags & geo.GEO_FLAGS_RENDER) != 0 && (next.weaponFlags & geo.GEO_FLAGS_CLIP) == 0)
                        {
                            btl_stat.GeoAddColor2DrawPacket(next.weapon_geo, (Int16)(arg1 - 128), (Int16)(arg1 - 128), (Int16)(arg1 - 128));
                            if (arg1 < 70)
                                btl_util.GeoSetABR(next.weapon_geo, "GEO_POLYFLAGS_TRANS_100_PLUS_25");
                        }
                        if ((next.flags & geo.GEO_FLAGS_RENDER) != 0 && (next.flags & geo.GEO_FLAGS_CLIP) == 0)
                        {
                            btl_stat.GeoAddColor2DrawPacket(next.gameObject, (Int16)(arg1 - 128), (Int16)(arg1 - 128), (Int16)(arg1 - 128));
                            if (arg1 < 70)
                                btl_util.GeoSetABR(next.gameObject, "GEO_POLYFLAGS_TRANS_100_PLUS_25");
                        }
                        break;
                    }
                }
                break;
            case 26: // Number of meshes
                return next.meshCount;
            case 27: // Update color
                btl_stat.SetPresentColor(next);
                break;
            case 28: // Play/Stop Texture Animation
            {
                switch (code) // Detect, which has an eye texture animation bug, calls it 4 times:
                              // 28 1 0 0 0
                              // 28 0 0 0 0
                              // 28 0 1 0 0
                              // 28 2 0 0 0
                {
                    case 0:
                        GeoTexAnim.geoTexAnimStop(next.texanimptr, arg1 != 0 ? 1 : 0);
                        break;
                    case 1:
                        GeoTexAnim.geoTexAnimStop(next.texanimptr, 2);
                        break;
                    case 2:
                        GeoTexAnim.geoTexAnimPlay(next.texanimptr, 2);
                        break;
                }
                break;
            }
            case 29: // Play Sound (Jump)
                btl_util.SetBattleSfx(next, 1110, 127);
                break;
            case 30: // Play Weapon Sound (see FF9Snd.ff9battleSoundWeaponSndEffect02, first entry)
                btlsnd.ff9btlsnd_sndeffect_play(btlsnd.ff9btlsnd_weapon_sfx(next.bi.line_no, FF9BatteSoundWeaponSndEffectType.FF9BTLSND_WEAPONSNDEFFECTTYPE_ATTACK), 0, 127, SeSnd.S_SeGetPos((UInt64)arg0));
                break;
            case 31: // Play Weapon Sound (see FF9Snd.ff9battleSoundWeaponSndEffect02, second entry)
                if (btl_util.getCurCmdPtr() != null)
                {
                    BattleUnit targ = btl_scrp.FindBattleUnit(btl_util.getCurCmdPtr().tar_id);
                    if (targ != null && (targ.Data.fig.info & Param.FIG_INFO_MISS) != 0)
                        break;
                }
                btlsnd.ff9btlsnd_sndeffect_play(btlsnd.ff9btlsnd_weapon_sfx(next.bi.line_no, FF9BatteSoundWeaponSndEffectType.FF9BTLSND_WEAPONSNDEFFECTTYPE_HIT), 0, 127, 128);
                break;
            case 33: // Play Casting Animation (Dragon)
            {
                // Freya's Dragon casting animation (looping or launch)
                switch (arg1)
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
                }
                String goName = next.gameObject.name;
                goName = goName.Trim();
                if (goName.CompareTo("192(Clone)") == 0 || goName.CompareTo("585(Clone)") == 0) // GEO_MAIN_B0_011's and GEO_MAIN_B0_033's internal names (Freya/Trance Freya)
                    btl_mot.setMotion(next, arg0 != 0 ? "ANH_MAIN_B0_011_202" : "ANH_MAIN_B0_011_201");
                else
                    btl_mot.setMotion(next, arg0 != 0 ? BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC : BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT);
                next.evt.animFrame = 0;
                if (arg0 == 0)
                    next.animFlag |= (UInt16)EventEngine.afLoop;
                break;
            }
        }
        return 0;
    }

    public static void CreateEffectDebugData()
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
                if ((fF9Battle.btl_load_status & ff9btl.LOAD_ALL) == ff9btl.LOAD_ALL)
                {
                    if (fF9Battle.btl_scene.Info.SpecialStart && !FF9StateSystem.Battle.isDebug)
                        fF9Battle.btl_phase = FF9StateBattleSystem.PHASE_EVENT;
                    else
                        fF9Battle.btl_phase = FF9StateBattleSystem.PHASE_MENU_ON;
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
        SFXData.Reinit();
        SFXDataCamera.currentCameraEngine = SFXDataCamera.CameraEngine.SFX_PLUGIN;
        if (Configuration.Battle.SFXRework)
            SFXChannel.LoadAll();
    }

    public static void StartDebugRoom()
    {
        SFX.StartCommon(true);
        SFX.InitBattleParty();
    }

    public static void StartCommon(Boolean mode)
    {
        SFX.isDebugMode = mode;
        SFX.hijackedCallback = null;
        SFXMesh.Init();
        SFXRender.Init();
        unsafe
        {
            SFX.SFX_InitSystem(SFX.BattleCallback);
        }
        SFX.isSystemRun = true;
        SFX.lastPlayedExeId = 0;
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
        if (Configuration.Battle.SFXRework)
        {
            UnifiedBattleSequencer.EndBattle();
            SFXChannel.EndBattle();
        }
        SFX.isSystemRun = false;
    }

    public static void StartPlungeCamera()
    {
        SFXDataCamera.currentCameraEngine = SFXDataCamera.CameraEngine.SFX_PLUGIN;
        Int32 projOffset = 0;
        if (FF9StateSystem.Battle.battleMapIndex == 21) // Sealion + Black Waltz 1
            projOffset = 100;
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
            sFX_INIT_PARAM.btl_scene_patAddr_putX = sB2_PATTERN.Monster[0].Xpos;
            sFX_INIT_PARAM.btl_scene_patAddr_putZ = sB2_PATTERN.Monster[0].Zpos;
            sFX_INIT_PARAM.btl_scene_FixedCamera1 = FF9StateSystem.Battle.FF9Battle.btl_scene.Info.FixedCamera1 ? (Byte)1 : (Byte)0;
            sFX_INIT_PARAM.btl_scene_FixedCamera2 = FF9StateSystem.Battle.FF9Battle.btl_scene.Info.FixedCamera2 ? (Byte)1 : (Byte)0;
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
        init.bi_slot_no = Math.Min(btl.bi.slot_no, (Byte)CharacterOldIndex.Beatrix);
        init.bi_slave = btl.bi.slave;
        init.bi_line_no = btl.bi.line_no;
        if (btl.bi.player != 0)
        {
            init.player_serial_no = Math.Min((Byte)btl_util.getSerialNumber(btl), (Byte)CharacterSerialNumber.BEATRIX);
            init.player_equip = btl.weapon.HitSfx;
            init.player_wep_bone = btl_util.getPlayerPtr(btl).wep_bone;
            init.enemy_radius = btl.radius_collision;
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
                //Byte typeNo = btlseq.instance.btl_list[4].typeNo;
                init.enemy_radius = btl.radius_collision; //FF9StateSystem.Battle.FF9Battle.btl_scene.MonAddr[typeNo].Radius;
                init.geo_radius = btlseq.instance.btl_list[4].radius_effect;
                init.geo_height = init.enemy_radius * 2;
                ENEMY enemyPtr = btl_util.getEnemyPtr(btl);
                init.enemy_cam_bone0 = enemyPtr.et.cam_bone[0];
                init.enemy_cam_bone1 = enemyPtr.et.cam_bone[1];
                init.enemy_cam_bone2 = enemyPtr.et.cam_bone[2];
            }
            else
            {
                init.enemy_radius = 200;
                init.geo_radius = btl.radius_effect;
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
            SFX.SetReqParam(ref SFX.request.mexe, mexe);
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

    public static void Play(SpecialEffect effNum)
    {
        SFX.currentEffectID = effNum;
        SFX.streamIndex = 0;
        SFX.soundFPS = 4;
        SFX.soundCallCount = 0;
        SFX.cameraOffset = 0f;
        Int32 num = SFX.playParam[(Int32)effNum] & 3;
        if (num == 0)
            SFX.subOrder = SFX.defaultSubOrder;
        else
            SFX.subOrder = num - 1;
        Int32 num2 = SFX.playParam[(Int32)effNum] >> 2 & 3;
        if (num2 == 0)
            SFX.colIntensity = SFX.defaultColIntensity;
        else
            SFX.colIntensity = num2 - 1;
        SFX.pixelOffset = 0;
        SFX.colThreshold = (SFX.playParam[(Int32)effNum] >> 29 & 1);
        SFX.addOrder = (SFX.playParam[(Int32)effNum] >> 28 & 1);
        SFX.SoundClear();
        SFX.isRunning = true;
        SFX.frameIndex = 0;
        SFX.effectPointFrame = -1;
        if (SFX.request.exe.btl_id <= 8)
            SFX.lastPlayedExeId = SFX.request.exe.btl_id;
        PSXTextureMgr.Reset();
        SFXMesh.DetectEyeFrameStart = -1;
        Int32 num3 = Marshal.SizeOf(SFX.request);
        Byte[] value = new Byte[num3];
        GCHandle gCHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
        Marshal.StructureToPtr(SFX.request, gCHandle.AddrOfPinnedObject(), false);
        SoundLib.LoadSfxSoundData((Int32)effNum);
        if (effNum == SpecialEffect.Special_Necron_Death)
            PSXTextureMgr.SpEff435();
        String path = "SpecialEffects/ef" + ((Int32)effNum).ToString("D3");
        Byte[] binAsset = AssetManager.LoadBytes(path, true);
        if (binAsset != null)
        {
            GCHandle gCHandle2 = GCHandle.Alloc(binAsset, GCHandleType.Pinned);
            SFX.SFX_Play((Int32)effNum, gCHandle2.AddrOfPinnedObject(), binAsset.Length, gCHandle.AddrOfPinnedObject());
            gCHandle2.Free();
        }
        else
        {
            SFX.SFX_Play((Int32)effNum, (IntPtr)null, 0, gCHandle.AddrOfPinnedObject());
        }
        gCHandle.Free();
    }

    public static void SetCameraTarget(Vector3 pos, BTL_DATA exe, BTL_DATA trg)
    {
        SFXDataCamera.currentCameraEngine = SFXDataCamera.CameraEngine.SFX_PLUGIN;
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
            switch (FF9StateSystem.Battle.battleMapIndex)
            {
                case 155: // Amdusias (YANA)
                    arg = 160;
                    break;
                case 160: // Abadon (YANA)
                    arg = 240;
                    break;
                case 303: // Plant Brain
                    if (cam == 9)
                        arg = 120;
                    break;
                case 163: // Shell Dragon (YANA)
                    arg = 120;
                    break;
                    //case 161: // Carve Spider + Axe Beak (World Map)
                    //case 162: // Carve Spider + Axe Beak (World Map)
            }
        }
        SFXDataCamera.currentCameraEngine = SFXDataCamera.CameraEngine.SFX_PLUGIN;
        SFX.SFX_SendIntData(3, cam, arg, 0);
    }

    public static void SetEnemyCamera(BTL_DATA btl)
    {
        Int32 arg = 0; // Seems to be either a camera distance or a fov angle
        switch (FF9StateSystem.Battle.battleMapIndex)
        {
            case 4: // Beatrix (Burmecia)
            case 73: // Beatrix (Alexandria)
            case 299: // Beatrix (Cleyra)
                arg = 200;
                break;
            case 83: // Lani
                arg = 140;
                break;
            case 163: // Shell Dragon (YANA)
            case 302: // Prison Cage (Garnet)
                arg = 150;
                break;
                //case 300: // Antlion (Cleyra)
                //case 301: // Prison Cage (Vivi)
        }
        SFXDataCamera.currentCameraEngine = SFXDataCamera.CameraEngine.SFX_PLUGIN;
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
            SFX.channel[i, 0] = -1;
    }

    public static Int32 AdjustSoundIndex(Int32 dno, Int32 sce, out Boolean shouldSkip)
    {
        dno += 12;
        shouldSkip = false;
        if (sce != 0)
        {
            switch (SFX.currentEffectID)
            {
                case SpecialEffect.Phoenix_Rebirth_Flame:
                    return dno + 12;
                case SpecialEffect.Madeen__Full:
                    return dno + 11;
                case SpecialEffect.Ark__Full:
                    if (SFX.soundCallCount >= 28)
                        return dno + 36;
                    else if (SFX.soundCallCount >= 20)
                        return dno + 20;
                    else if (SFX.soundCallCount >= 12)
                        return dno + 16;
                    else
                        return dno + 8;
                case SpecialEffect.Ark__Short:
                    return dno + 4;
                case SpecialEffect.Bahamut__Full:
                    return dno + 14;
            }
        }
        switch (SFX.currentEffectID)
        {
            case SpecialEffect.Ark__Short:
                if (SFX.soundCallCount >= 8)
                    return dno + 8;
                break;
            case SpecialEffect.Ark__Full:
                if (SFX.soundCallCount >= 28)
                    return dno + 28;
                else if (SFX.soundCallCount >= 12)
                    return dno + 12;
                else if (SFX.soundCallCount == 0)
                    SFX.soundFPS = -1;
                break;
            case SpecialEffect.Lucky_Seven:
                return dno - 7;
            case SpecialEffect.Odin__Full:
                if (SFX.soundCallCount == 0)
                    SFX.soundFPS = -3;
                break;
            case SpecialEffect.Roulette:
                if (dno != 12)
                    return dno - 3;
                break;
            case SpecialEffect.Bahamut__Full:
                if (SFX.soundCallCount == 4)
                    SFX.soundFPS = -2;
                return dno - 2;
            case SpecialEffect.Leviathan__Full:
                if (dno == 15)
                    shouldSkip = true;
                return dno;
            case SpecialEffect.Explosion:
                if (dno != 12)
                    shouldSkip = true;
                return dno;
            case SpecialEffect.Venom_Powder:
                return dno - 3;
        }
        return dno;
    }

    public static void SoundPlay(Int32 dno, Int32 attr, Int32 sce)
    {
        Boolean skip;
        Int32 num = AdjustSoundIndex(dno, sce, out skip);
        if (skip)
            return;
        SFX.soundCallCount++;
        Single pitch;
        if (SFX.currentEffectID == SpecialEffect.Odin__Short)
        {
            pitch = 1.25f;
        }
        else
        {
            switch (SFX.soundFPS + 3)
            {
                case 0:
                    pitch = 0.725f;
                    break;
                case 1:
                    pitch = 0.85f;
                    break;
                case 2:
                    pitch = 0.76f;
                    break;
                case 6:
                    pitch = 0.75f;
                    break;
                default:
                    pitch = 1f;
                    break;
            }
        }
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
        Int32 num = (dno & 0xFFFFFF) * 3 + SFX.seChantIndex;
        if ((dno & 0xFFFFFF) == 3)
        {
            if (SFX.seChantIndex % 3 == 0)
                SoundLib.PlaySoundEffect(REFLECT_SOUND_ID, 1f, 0f, 1f);
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
        switch (SFX.currentEffectID)
        {
            case SpecialEffect.Phoenix_Rebirth_Flame:
                num += 16;
                break;
            case SpecialEffect.Fenrir_Wind__Full:
                num += 16;
                break;
            case SpecialEffect.Bahamut__Full:
                num += 26 + SFX.streamIndex;
                break;
            case SpecialEffect.Fenrir_Earth__Full:
                num += 14;
                break;
            case SpecialEffect.Phoenix__Full:
                num += 8;
                break;
            case SpecialEffect.Leviathan__Full:
                num += 12;
                break;
            case SpecialEffect.Ramuh__Full:
                num += 9;
                break;
            case SpecialEffect.Madeen__Full:
                num += 19;
                break;
            case SpecialEffect.Odin__Full:
                num += 16;
                break;
            case SpecialEffect.Ifrit__Full:
                break;
            case SpecialEffect.Ark__Full:
                num += 40 + SFX.streamIndex;
                break;
            case SpecialEffect.Ramuh__Short:
                num += 6;
                break;
            default:
                return;
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

    public static Boolean ShouldPlayAlternateCamera(CMD_DATA cmd)
    {
        if (cmd.aa.Info.DefaultCamera)
            return true;
        if (FF9StateSystem.Settings.cfg.camera == 1UL || Comn.random8() >= 128)
            return false;
        BattleStatus pStat = cmd.regist.stat.CurrentIncludeOnHold;
        for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
            if ((next.btl_id & cmd.tar_id) != 0)
                pStat |= next.stat.CurrentIncludeOnHold;
        return (pStat & BattleStatusConst.PreventAlternateCamera) == 0u;
    }

    public const Int32 DEBUG_EFFECT_ID = 126;

    public const Int32 effMax = 511;

    public const Single ONE = 4096f;

    public const Single EUL2PSX = 11.3777781f;

    public const Single PSX2EUL = 0.087890625f;

    public enum COMMAND
    {
        COMMAND_GET_POSITION = 1,
        COMMAND_SET_POSITION = 2,
        COMMAND_GET_ROTATE = 3,
        COMMAND_SET_ROTATE = 4,
        COMMAND_GET_SCALE = 5,
        COMMAND_SET_SCALE = 6,
        COMMAND_GET_GEO_FLAG = 7,
        COMMAND_GET_AUTO_POTION_COMMAND = 8,
        COMMAND_GET_MOTION_FRAME_MAX = 9,
        COMMAND_GET_MOTION_FRAME = 10,
        COMMAND_SET_MOTION_FRAME = 11,
        COMMAND_SET_MOTION = 12,
        COMMAND_STOP_MOTION = 13,
        COMMAND_GET_MATRIX = 14,
        COMMAND_IS_TARGET = 15,
        COMMAND_SET_DEFAULT_IDLE = 16,
        COMMAND_GET_DISAPPEAR = 17,
        COMMAND_SET_DISAPPEAR = 18,
        COMMAND_SHOW_WEAPON = 19,
        COMMAND_CHECK_STATUS = 20,
        COMMAND_GET_CMD_IDLE = 21,
        COMMAND_GET_SLAVE = 22,
        COMMAND_EXEC_VFX = 23,
        COMMAND_BTL_2D_REQ = 24,
        COMMAND_SHOW_MESH = 25,
        COMMAND_GET_MESH_COUNT = 26,
        COMMAND_SET_PRESENT_COLOR = 27,
        COMMAND_EYE_BLINK = 28,
        COMMAND_SFX_BATTLE = 29,
        COMMAND_SFX_PLAYER_ATTACK = 30,
        COMMAND_SFX_PLAY_HIT = 31,
        COMMAND_SFX_PLAY = 32,
        COMMAND_SET_SPMOTION = 33,
        COMMAND_LOAD_IMAGE = 100,
        COMMAND_STORE_IMAGE = 101,
        COMMAND_MOVE_IMAGE = 102,
        COMMAND_INCREMENT_BATTLE_SEQ = 110,
        COMMAND_GET_CAMERA_CONFIG = 111,
        COMMAND_LBOSS_FLAG_ENABLE = 112,
        COMMAND_EFFECT_TITLE = 113,
        COMMAND_SET_CURSOR = 114,
        COMMAND_GET_CURSOR = 115,
        COMMAND_SFX_PAUSE = 116,
        COMMAND_GET_BG_INTENSITY = 117,
        COMMAND_SET_BG_INTENSITY = 118,
        COMMAND_VIBRATE = 119,
        COMMAND_CREATE_TEXTURE = 120,
        COMMAND_GET_BONUS = 121,
        COMMAND_GET_START_TYPE = 122,
        COMMAND_GET_ALIVE_COUNT = 123,
        COMMAND_GET_CAMERA_NUMBER = 124,
        COMMAND_SET_FPS = 125
    }

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

    public const String REFLECT_SOUND_PATH = "Sounds02/SE02/reflect";

    public static SpecialEffect currentEffectID;

    public static Boolean isDebugAutoPlay;

    public static Boolean isDebugPng;

    public static Boolean IsDebugMesh;

    public static Boolean IsDebugObjMesh;

    public static Boolean isDebugPrintCode;

    public static Boolean isDebugViewport;

    public static Boolean isDebugLine;

    public static Boolean isDebugCam;

    public static Boolean isDebugMode;

    public static Boolean isDebugFilter;

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

    public static Callback hijackedCallback = null;

    public static Int32 preventStepInOut = -1;

    public static Int32 effectPointFrame = -1;

    public static UInt16 lastPlayedExeId = 0;
}
