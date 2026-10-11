using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// GTE GPF: MAC1..3 = (IR0 * IR1..3) &gt;&gt; 12 over the int16 views of the IR globals, then
    /// <see cref="GteStoreMacResult"/>. Returns the RGBC code byte.
    /// </summary>
    public static long GteGpf()
    {
        Recompiler.Runtime.Tracer.Enter("GteGpf()");
        short ir0 = (short)dword_180211FE0;
        short* ir23 = (short*)Unsafe.AsPointer(ref qword_180211FE8);
        int mac1 = (ir0 * (short)dword_180211FE4) >> 12;
        int mac2 = (ir0 * ir23[0]) >> 12;
        dword_180212024 = mac1;
        dword_180212028 = mac2;
        int mac3 = (ir0 * ir23[2]) >> 12;
        dword_18021202C = mac3;
        return GteStoreMacResult(mac1, mac2, mac3);
    }

    /// <summary>GTE GPL: MAC1..3 += (IR0 * IR1..3) &gt;&gt; 12, then <see cref="GteStoreMacResult"/>.</summary>
    public static long GteGpl()
    {
        Recompiler.Runtime.Tracer.Enter("GteGpl()");
        short ir0 = (short)dword_180211FE0;
        short* ir23 = (short*)Unsafe.AsPointer(ref qword_180211FE8);
        int mac1 = ((ir0 * (short)dword_180211FE4) >> 12) + dword_180212024;
        dword_180212024 = mac1;
        int mac2 = ((ir0 * ir23[0]) >> 12) + dword_180212028;
        dword_180212028 = mac2;
        int mac3 = ((ir0 * ir23[2]) >> 12) + dword_18021202C;
        dword_18021202C = mac3;
        return GteStoreMacResult(mac1, mac2, mac3);
    }

    /// <summary>
    /// GTE INTPL: MAC = IR + ((sat16(FC - IR) * IR0) &gt;&gt; 12) per channel with the far color RFC/GFC
    /// (qword_180211F94 lo/hi) and BFC (dword_180211F9C), then <see cref="GteStoreMacResult"/>.
    /// </summary>
    public static long GteIntpl()
    {
        Recompiler.Runtime.Tracer.Enter("GteIntpl()");
        short ir0 = (short)dword_180211FE0;
        short* ir23 = (short*)Unsafe.AsPointer(ref qword_180211FE8);
        int* farColor = (int*)Unsafe.AsPointer(ref qword_180211F94);

        short ir1 = (short)dword_180211FE4;
        int mac1 = ir1 + ((IntplSatS16(farColor[0] - ir1) * ir0) >> 12);
        dword_180212024 = mac1;
        short ir2 = ir23[0];
        int mac2 = ir2 + ((IntplSatS16(farColor[1] - ir2) * ir0) >> 12);
        dword_180212028 = mac2;
        short ir3 = ir23[2];
        int mac3 = ir3 + ((IntplSatS16(dword_180211F9C - ir3) * ir0) >> 12);
        dword_18021202C = mac3;
        return GteStoreMacResult(mac1, mac2, mac3);
    }

    /// <summary>
    /// libgte LoadAverageShort12: IR1..3 = v0 (a1), IR0 = p0 (a3), GPF; IR1..3 = v1 (a2), IR0 = p1 (a4), GPL; then the
    /// IR registers are sign-extended from their int16 views and written to <paramref name="a5"/>. Returns a5[2].
    /// </summary>
    public static long GteLoadAverageShort12(short* a1, short* a2, int a3, int a4, ushort* a5)
    {
        Recompiler.Runtime.Tracer.Enter("GteLoadAverageShort12(a1, a2, a3, a4, a5)", (nint)a1, (nint)a2, a3, a4, (nint)a5);
        int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        int v0y = a1[1];
        int v0z = a1[2];
        int v1x = a2[0];
        int v1y = a2[1];
        int v1z = a2[2];
        dword_180211FE4 = a1[0];
        ir23[0] = v0y;
        ir23[1] = v0z;
        dword_180211FE0 = a3;
        GteGpf();
        dword_180211FE0 = a4;
        dword_180211FE4 = v1x;
        ir23[0] = v1y;
        ir23[1] = v1z;
        GteGpl();

        ir23[0] = (short)ir23[0];
        short y = (short)ir23[0];
        dword_180211FE4 = (short)dword_180211FE4;
        ir23[1] = (short)ir23[1];
        ushort z = (ushort)ir23[1];
        a5[0] = (ushort)dword_180211FE4;
        a5[1] = (ushort)y;
        a5[2] = z;
        return z;
    }

    /// <summary>
    /// For every active object (byte +16) of the list at qword_1800694F0 (next at +0): host message
    /// HostCommand.SetPresentColor | id (+24), then <see cref="HostSendCommand06IfFlag1000"/> and <see cref="HostSendCommand19IfFlag400"/>.
    /// Returns 447 unless the PSX header addressed by dword 0 of off_180066C68 holds 447 at +2; then sends
    /// HostCommand.SetDefaultIdle | id of the object at +80 of the block addressed by dword 3 (plain HostCommand.SetDefaultIdle without one) and
    /// returns the host answer, or 0 without a callback.
    /// </summary>
    public static long HostNotifyActiveObjects()
    {
        Recompiler.Runtime.Tracer.Enter("HostNotifyActiveObjects()");
        for (long i = BattleObjectList; i != 0; i = *(long*)i)
        {
            if (*(byte*)(i + 16) != 0)
            {
                if (BattleCallbackAddress != null)
                {
                    Native.InvokeBattleCallback((long)GetBattleCallback((long)BattleCallbackAddress), *(ushort*)(i + 24) | HostCommand.SetPresentColor, 0, 0, 0, 0, 0);
                }

                HostResetScaleIfFlag1000(i);
                HostFadeMeshIfFlag400(i);
            }
        }

        uint* context = (uint*)off_180066C68;
        byte* header = ResolveGlobalPsxAddress(context[0]);
        long result = 447;
        if (*((ushort*)header + 1) != 447)
        {
            return result;
        }

        byte* block = ResolveGlobalPsxAddress(context[3]);
        result = (long)BattleCallbackAddress;
        long current = *((long*)block + 10);
        if (BattleCallbackAddress != null)
        {
            var callback = GetBattleCallback((long)BattleCallbackAddress);
            if (current != 0)
            {
                return Native.InvokeBattleCallback((long)callback, *(ushort*)(current + 24) | HostCommand.SetDefaultIdle, 0, 0, 0, 0, 0);
            }

            return Native.InvokeBattleCallback((long)callback, HostCommand.SetDefaultIdle, 0, 0, 0, 0, 0);
        }

        return result;
    }

    /// <summary>
    /// dword_180323268 = 1 when the PSX header (dword 0 of off_180066C68) has bit 0x8 in byte +28. Otherwise 0 when the
    /// host answers 1 to HostCommand.GetCameraConfig or the current object (+80 of the block at dword 3) has host flag HostCommand.SetDefaultIdle or
    /// HostCommand.SetRotate; else (dword_180069718 &amp; 0x800) == 0 when byte +16 of context qword 0 is set, (&amp; 0x1000) == 0 otherwise.
    /// </summary>
    public static void RecomputeGate323268()
    {
        Recompiler.Runtime.Tracer.Enter("RecomputeGate323268()");
        uint* context = (uint*)off_180066C68;
        byte* header = ResolveGlobalPsxAddress(context[0]);
        if ((header[28] & 8) != 0)
        {
            dword_180323268 = 1;
            return;
        }

        byte* block = ResolveGlobalPsxAddress(context[3]);
        long current = *((long*)block + 10);
        long entry = PsxGetContextQword(0);
        if ((BattleCallbackAddress != null && (uint)Native.InvokeBattleCallback((long)GetBattleCallback((long)BattleCallbackAddress), HostCommand.GetCameraConfig, 0, 0, 0, 0, 0) == 1)
            || HostCallbackSend(current, HostCommand.SetDefaultIdle) != 0
            || HostCallbackSend(current, HostCommand.SetRotate) != 0)
        {
            dword_180323268 = 0;
        }
        else if (*(byte*)(entry + 16) != 0)
        {
            dword_180323268 = (dword_180069718 & 0x800) == 0 ? 1 : 0;
        }
        else
        {
            dword_180323268 = (dword_180069718 & 0x1000) == 0 ? 1 : 0;
        }
    }

    /// <summary>
    /// Shared tail of GPF/GPL/INTPL: IR1..3 low words = sat16(MAC1..3) (the upper words are kept), RGB FIFO shifts
    /// (RGB0 = RGB1, RGB1 = RGB2) and RGB2 = satU8(MAC &gt;&gt; 4) with the code byte of RGBC; returns that code byte.
    /// </summary>
    private static long GteStoreMacResult(int mac1, int mac2, int mac3)
    {
        Recompiler.Runtime.Tracer.Enter("GteStoreMacResult(mac1, mac2, mac3)", mac1, mac2, mac3);
        short* ir1 = (short*)Unsafe.AsPointer(ref dword_180211FE4);
        short* ir23 = (short*)Unsafe.AsPointer(ref qword_180211FE8);
        ir1[0] = IntplSatS16(mac1);
        ir23[0] = IntplSatS16(mac2);
        ir23[2] = IntplSatS16(mac3);
        dword_180212010 = dword_180212014;
        dword_180212014 = dword_180212018;
        byte* rgb2 = (byte*)Unsafe.AsPointer(ref dword_180212018);
        rgb2[0] = IntplSatU8(mac1 >> 4);
        rgb2[1] = IntplSatU8(mac2 >> 4);
        rgb2[2] = IntplSatU8(mac3 >> 4);
        byte code = ((byte*)Unsafe.AsPointer(ref dword_180211FD8))[3];
        rgb2[3] = code;
        return code;
    }

    /// <summary>The host callback (qword_1801C1DE8) with the prototype the decompiler gives it.</summary>
    private static BattleCallbackFn GetBattleCallback(long address)
    {
        Recompiler.Runtime.Tracer.Enter("GetBattleCallback(address)", address);
        return (BattleCallbackFn)address;
    }
}
