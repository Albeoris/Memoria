using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Module (re)initialisation run by every SFX_InitSystem call (no one-time guard in the original): registers the
    /// VRAM/template buffers with the PSX address mapper at unk_180576A10, allocates the 0x2EC0-byte call context,
    /// clears both ordering tables, uploads the VRAM template, resets the SFX globals and seeds the camera/view
    /// parameters. Returns the low dword of qword_1805789D0 (also copied to its high dword), zero-extended.
    /// </summary>
    public static long SfxInitModuleState()
    {
        Recompiler.Runtime.Tracer.Enter("SfxInitModuleState()");
        long mapper = (long)unk_180576A10;
        PsxAddrMapperRegister(mapper, (ulong)unk_1802208D0);
        PsxAddrMapperRegister(mapper, (ulong)Unsafe.AsPointer(ref word_1803208D0));
        PsxAddrMapperRegister(mapper, (ulong)unk_180063860);

        // operator new + memset + ctor; a null allocation stores 0.
        void* context = NativeMemory.Alloc(0x2EC0);
        qword_1803678E0 = context != null ? SfxInitModuleStateNewContext(context) : 0;

        // rep stosd, 32 dwords each.
        Unsafe.InitBlockUnaligned(Unsafe.AsPointer(ref qword_180211FC0), 0, 128);
        Unsafe.InitBlockUnaligned(xmmword_180211F40, 0, 128);

        off_180066C60 = (nint)unk_1800EDAB4;
        uint ot = (uint)PsxAddrMapperRegister(mapper, (ulong)unk_1800EDB28);
        ((uint*)off_180066C68)[8] = ot;
        PsxClearOTagR((uint*)ResolveGlobalPsxAddress(ot));

        off_180066C60 = (nint)unk_1800697B0;
        ot = (uint)PsxAddrMapperRegister(mapper, (ulong)unk_180069824);
        ((uint*)off_180066C68)[8] = ot;
        PsxClearOTagR((uint*)ResolveGlobalPsxAddress(ot));

        CurrentEffectId = 0;
        SfxVramInitUpload();
        SfxResetAndRegisterGlobals();
        dword_180211E4C = 0;
        ((uint*)off_180066C68)[11] = (uint)PsxAddrMapperRegister(mapper, (ulong)unk_180220060);
        Unsafe.InitBlockUnaligned(unk_180220060, 0, 0xD8);
        dword_1802200BE = 0x2000200;

        // View parameters at 0x180211E28..0x180211E5B (store order and widths as the original).
        *(ulong*)unk_180211E40 = 0;
        *(uint*)unk_180211E3C = 0;
        System.Buffer.MemoryCopy(xmmword_18004B710, xmmword_180211E28, 16, 16);
        *(short*)xmmword_180211E38 = unchecked((short)0xF2DC);
        dword_180211E50 = unchecked((int)0xFFFFFFCF);
        dword_180211E54 = 0x1D9;
        dword_180211E58 = 0x16D6;
        FreeCameraProjectionDistance = 400;

        // Copy into 0x180069730..0x180069751.
        System.Buffer.MemoryCopy(xmmword_18004B710, xmmword_180069730, 16, 16);
        System.Buffer.MemoryCopy(xmmword_180211E38, xmmword_180069740, 16, 16);
        *(int*)unk_180069744 = unchecked((int)0xFFFFFFCF);
        *(int*)unk_180069748 = 0x1D9;
        *(int*)unk_18006974C = 0x16D6;
        CameraProjectionDistance = 400;

        ((uint*)off_180066C68)[3] = (uint)PsxAddrMapperRegister(mapper, (ulong)unk_180211E80);
        Unsafe.InitBlockUnaligned(Unsafe.AsPointer(ref BattleObjectList), 0, 0x238);
        qword_180069720 = (long)unk_1801C1DF0;

        uint low = *(uint*)Unsafe.AsPointer(ref qword_1805789D0);
        *(uint*)unk_1805789D4 = low;
        return low;
    }

    /// <summary>
    /// Export: stores the host callback pointer at qword_1801C1DE8 (shared module data, used by HostCallbackSend and
    /// the other host message senders) and tail-calls the module initialisation.
    /// </summary>
    public static void SFX_InitSystem(BattleCallbackFn method)
    {
        Recompiler.Runtime.Tracer.Enter("SFX_InitSystem(method)", (nint)method);
        BattleCallbackAddress = method;
        SfxInitModuleState();
    }

    private static long SfxInitModuleStateNewContext(void* context)
    {
        Recompiler.Runtime.Tracer.Enter("SfxInitModuleStateNewContext(context)", (nint)context);
        Unsafe.InitBlockUnaligned(context, 0, 0x2EC0);
        return PsxCallContextCtor((long)context);
    }
}
