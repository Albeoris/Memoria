#nullable enable
#pragma warning disable CS8981 // lower-case type names mirror the native export names
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Recompiler.Runtime;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Exports
{
    private static OriginalModule Original => OriginalModule.Shared("FF9SpecialEffectPlugin_orig.dll");

    // SFX_BeginRender (ordinal 1): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_BeginRender")]
    public static sbyte SFX_BeginRender() => Native.SFX_BeginRender_0();

    // SFX_GetPrim (ordinal 2): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_GetPrim")]
    public static P_TAG* SFX_GetPrim(int* otz) => Native.SFX_GetPrim_0(otz);

    // SFX_InitBattle (ordinal 3): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_InitBattle")]
    public static void SFX_InitBattle(SFX_INIT_PARAM* param) => Native.SFX_InitBattle_0(param);

    // SFX_InitSystem (ordinal 4): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_InitSystem")]
    public static void SFX_InitSystem(delegate* unmanaged<int, int, int, int, int, void*, int> method) => Native.SFX_InitSystem(method);

    // SFX_LateUpdate (ordinal 5): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_LateUpdate")]
    public static void SFX_LateUpdate() => Native.SFX_LateUpdate();

    // SFX_MoveFreeCamera (ordinal 6): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_MoveFreeCamera")]
    public static void SFX_MoveFreeCamera(int type, int x, int y) => Native.SFX_MoveFreeCamera_0(type, x, y);

    // SFX_Play (ordinal 7): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_Play")]
    public static void SFX_Play(int effnum, byte* bin, int size, S_Eff_Req_Org* req) => Native.SFX_Play_0(effnum, bin, size, req);

    // SFX_SendFloatData (ordinal 8): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_SendFloatData")]
    public static int SFX_SendFloatData(int type, int btl_id, float arg0, float arg1, float arg2) => Native.SFX_SendFloatData(type, btl_id, arg0, arg1, arg2);

    // SFX_SendIntData (ordinal 9): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_SendIntData")]
    public static int SFX_SendIntData(int type, int arg0, int arg1, int arg2) => Native.SFX_SendIntData_0(type, arg0, arg1, arg2);

    // SFX_SkipCameraAnimation (ordinal 10): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_SkipCameraAnimation")]
    public static void SFX_SkipCameraAnimation(int skip) => Native.SFX_SkipCameraAnimation(skip);

    // SFX_StartPlungeCamera (ordinal 11): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_StartPlungeCamera")]
    public static void SFX_StartPlungeCamera(byte* btlseq, int btlseqLen, int camOffset, int projOffset) => Native.SFX_StartPlungeCamera_0(btlseq, btlseqLen, camOffset, projOffset);

    // SFX_Update (ordinal 12): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_Update")]
    public static sbyte SFX_Update(int* frameIndex) => Native.SFX_Update_0(frameIndex);

    // SFX_UpdateCamera (ordinal 13): managed
    [UnmanagedCallersOnly(EntryPoint = "SFX_UpdateCamera")]
    public static float* SFX_UpdateCamera(int isDebug) => Native.SFX_UpdateCamera_0(isDebug);
}
