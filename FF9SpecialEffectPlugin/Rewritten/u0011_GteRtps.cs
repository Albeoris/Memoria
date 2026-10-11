using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// PSX GTE emulation over the module's register file (0x180211F40..0x18021202F) and the host messages that
/// pass rectangles to the host callback at 0x1801C1DE8.
/// Register map: RT 0x..F40 (9 x s16, RT33 in dword_180211F50), TR 0x..F54/F58/F5C, FC 0x..F94/F98/F9C,
/// OFX/OFY/H/DQA/DQB 0x..FA0..FB0, V0..V2 0x..FC0 (8 bytes each), RGBC 0x..FD8, OTZ 0x..FDC, IR0..IR3 0x..FE0..FEC,
/// SXY0..2 0x..FF0..FF8, SZ0..3 0x..2000..200C, RGB0..2 0x..2010..2018, MAC0..3 0x..2020..202C.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>SQR (sf=0, lm=1): MAC1..3 = IR1..3 squared, IR1..3 = MAC clamped to 0..0x7FFF; returns the clamped MAC2.</summary>
    public static long GteSqr()
    {
        Recompiler.Runtime.Tracer.Enter("GteSqr()");
        short* ir23 = GteRegs.S16(ref qword_180211FE8);
        short ir1 = *GteRegs.S16(ref dword_180211FE4);
        int mac1 = ir1 * ir1;
        int mac2 = ir23[0] * ir23[0];
        int mac3 = ir23[2] * ir23[2];
        dword_180212024 = mac1;
        dword_180212028 = mac2;
        dword_18021202C = mac3;

        *GteRegs.S16(ref dword_180211FE4) = GteRegs.ClampU15(mac1);
        long result = mac2 > 0x7FFF ? 0x7FFF : mac2 < 0 ? 0 : (uint)mac2;
        ir23[0] = (short)result;
        ir23[2] = GteRegs.ClampU15(mac3);
        return result;
    }

    /// <summary>
    /// Fills the argument records of a native-to-PSX call: for every character of the signature a2, 'v' takes the low
    /// 32 bits and 'p' the full 64 bits of the next vararg slot of a3; other characters leave their record untouched.
    /// Records are indexed by character position; the count is strlen(a2).
    /// </summary>
    public static long PsxCallParseArgs(long a1, long a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("PsxCallParseArgs(a1, a2, a3)", a1, a2, a3);
        byte* signature = (byte*)a2;
        long length = -1;
        do
        {
            ++length;
        }
        while (signature[length] != 0);

        var context = (PsxCallContext*)a1;
        long result = a3;
        if ((int)length > 0)
        {
            long* slot = (long*)a3 - 1;
            byte* record = ((byte*)&context->args);
            for (long i = 0; i < (int)length; i++, record += 16)
            {
                byte kind = signature[i];
                if (kind == (byte)'v')
                {
                    slot++;
                    var arg = (PsxCallArg*)record;
                    arg->isPointer = 0;
                    arg->pointer = 0;
                    arg->value = *(uint*)slot;
                }
                else if (kind == (byte)'p')
                {
                    slot++;
                    var arg = (PsxCallArg*)record;
                    arg->isPointer = 1;
                    arg->value = 0;
                    arg->pointer = *(ulong*)slot;
                }
            }

            // The original returns its running cursor (record + 4) after the loop.
            result = (long)(record + 4);
        }

        context->argCount = (int)length;
        return result;
    }

    /// <summary>MFC2: data register a1; VZ0..VZ2 (1, 3, 5) and IR0..IR3 (8..11) are first sign-extended in place from their low 16 bits.</summary>
    public static long GteReadData(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("GteReadData(a1)", a1);
        int* data = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        if ((((a1 - 1) & unchecked((int)0xFFFFFFF9)) == 0 && a1 != 7) || (uint)(a1 - 8) <= 3)
        {
            data[a1] = ((short*)data)[2 * a1];
        }

        return (uint)data[a1];
    }

    /// <summary>MAC1..3 = TR + (RT * V0) >> 12, IR1..3 = MAC clamped to signed 16 bits; always returns 0xFFFF8000.</summary>
    public static long GteRtV0Tr()
    {
        Recompiler.Runtime.Tracer.Enter("GteRtV0Tr()");
        short* rt = (short*)xmmword_180211F40;
        short rt33 = (short)dword_180211F50;
        short* v = GteRegs.S16(ref qword_180211FC0);

        int mac1 = GteRegs.TrX + ((v[0] * rt[0] + v[2] * rt[2] + v[1] * rt[1]) >> 12);
        dword_180212024 = mac1;
        int mac2 = GteRegs.TrY + ((v[0] * rt[3] + v[2] * rt[5] + v[1] * rt[4]) >> 12);
        dword_180212028 = mac2;
        int mac3 = dword_180211F5C + ((v[0] * rt[6] + v[2] * rt33 + v[1] * rt[7]) >> 12);
        dword_18021202C = mac3;

        *GteRegs.S16(ref dword_180211FE4) = GteRegs.ClampS16(mac1);
        short* ir23 = GteRegs.S16(ref qword_180211FE8);
        ir23[0] = GteRegs.ClampS16(mac2);
        ir23[2] = GteRegs.ClampS16(mac3);
        return 4294934528L;
    }

    /// <summary>
    /// RTPS of vector a1: MAC/IR1..3 = TR + (RT * V) >> 12, SZ and SXY FIFOs advance, SZ3 = MAC3 clamped to 0..0xFFFF,
    /// q = (H &lt;&lt; 16) / SZ3 (0x1FFFF when SZ3 is 0), SX2/SY2 = OF + (q * IR) >> 16 clamped to -1024..1023,
    /// MAC0 = DQB + low32(address of DQA) * q, IR0 = MAC0 >> 12 clamped to 0..4095; returns the unclamped-high/zero-clamped IR0 value.
    /// </summary>
    public static long GteRtps(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("GteRtps(a1)", a1);
        short* rt = (short*)xmmword_180211F40;
        short rt33 = (short)dword_180211F50;
        short* v = (short*)((long*)Unsafe.AsPointer(ref qword_180211FC0) + a1);

        int mac1 = GteRegs.TrX + ((v[2] * rt[2] + rt[1] * v[1] + rt[0] * v[0]) >> 12);
        dword_180212024 = mac1;
        int mac2 = GteRegs.TrY + ((v[2] * rt[5] + rt[4] * v[1] + rt[3] * v[0]) >> 12);
        dword_180212028 = mac2;
        int mac3 = dword_180211F5C + ((v[2] * rt33 + rt[6] * v[0] + rt[7] * v[1]) >> 12);
        dword_18021202C = mac3;

        short ir1 = GteRegs.ClampS16(mac1);
        short ir2 = GteRegs.ClampS16(mac2);
        *GteRegs.S16(ref dword_180211FE4) = ir1;
        short* ir23 = GteRegs.S16(ref qword_180211FE8);
        ir23[0] = ir2;
        ir23[2] = GteRegs.ClampS16(mac3);

        dword_180212000 = dword_180212004;
        dword_180212004 = dword_180212008;
        dword_180212008 = dword_18021200C;
        ushort sz3 = mac3 > 0xFFFF ? (ushort)0xFFFF : mac3 < 0 ? (ushort)0 : (ushort)mac3;
        *(ushort*)Unsafe.AsPointer(ref dword_18021200C) = sz3;
        dword_180211FF0 = dword_180211FF4;
        dword_180211FF4 = dword_180211FF8;

        int q = sz3 != 0 ? ((short)dword_180211FA8 << 16) / sz3 : 0x1FFFF;

        short* sxy2 = GteRegs.S16(ref dword_180211FF8);
        int sx = (int)(dword_180211FA0 + ((q * (long)ir1) >> 16));
        sxy2[0] = sx > 1023 ? (short)1023 : sx < -1024 ? (short)-1024 : (short)sx;
        int sy = (int)(dword_180211FA4 + ((q * (long)ir2) >> 16));
        sxy2[1] = sy > 1023 ? (short)1023 : sy < -1024 ? (short)-1024 : (short)sy;

        // Native LEA at 0x180004078 and IMUL eax,ecx at 0x180004086 multiply by the storage address itself.
        // This preserves the original address-dependent operation; reading the DQA value changes its semantics.
        dword_180212020 = unchecked(dword_180211FB0 + (int)(nuint)GteDqa * q);
        long result = (uint)(dword_180212020 >> 12);
        short* ir0 = GteRegs.S16(ref dword_180211FE0);
        if ((int)result <= 4095)
        {
            if ((int)result < 0)
            {
                result = 0;
            }

            *ir0 = (short)result;
        }
        else
        {
            *ir0 = 4095;
        }

        return result;
    }

    /// <summary>MAC0 = ((SZ0 + SZ1 + SZ2) &lt;&lt; 12) / 3 (as stored at 0x..2000..2008), OTZ = MAC0 >> 14 clamped to 0..0xFFFF.</summary>
    public static long GteAvsz3()
    {
        Recompiler.Runtime.Tracer.Enter("GteAvsz3()");
        int mac0 = (((ushort)dword_180212008 + (ushort)dword_180212004 + (ushort)dword_180212000) << 12) / 3;
        dword_180212020 = mac0;
        long result = mac0 >> 14;
        short* otz = GteRegs.S16(ref dword_180211FDC);
        if ((mac0 >> 14) <= 0xFFFF)
        {
            if ((int)result < 0)
            {
                result = 0;
            }

            *otz = (short)result;
        }
        else
        {
            *otz = -1;
        }

        return result;
    }

    /// <summary>
    /// DPCS: MAC1..3 = ((C &lt;&lt; 16) + IR0 * (FC - 16 * C)) >> 12 for the RGBC channels, IR1..3 clamped to signed 16 bits,
    /// RGB FIFO advances and RGB2 = MAC >> 4 clamped to 0..255 with the RGBC code byte, which is returned.
    /// </summary>
    public static long GteDpcs()
    {
        Recompiler.Runtime.Tracer.Enter("GteDpcs()");
        byte* rgbc = (byte*)Unsafe.AsPointer(ref dword_180211FD8);
        int r = rgbc[0];
        int g = rgbc[1];
        int b = rgbc[2];
        short ir0 = (short)dword_180211FE0;

        long mac1 = ((r << 16) + ir0 * (long)(int)(qword_180211F94 - 16 * r)) >> 12;
        dword_180212024 = (int)mac1;
        // movsxd r9,eax at 0x180004B97 sign-extends the 32-bit GFC - 16 * G difference.
        long mac2 = ((g << 16) + ir0 * (long)unchecked((int)(qword_180211F94 >> 32) - 16 * g)) >> 12;
        dword_180212028 = (int)mac2;
        long mac3 = ((b << 16) + ir0 * (long)(dword_180211F9C - 16 * b)) >> 12;
        dword_18021202C = (int)mac3;

        *GteRegs.S16(ref dword_180211FE4) = GteRegs.ClampS16((int)mac1);
        short* ir23 = GteRegs.S16(ref qword_180211FE8);
        ir23[0] = GteRegs.ClampS16((int)mac2);
        ir23[2] = GteRegs.ClampS16((int)mac3);

        dword_180212010 = dword_180212014;
        dword_180212014 = dword_180212018;
        byte* rgb2 = (byte*)Unsafe.AsPointer(ref dword_180212018);
        rgb2[0] = GteRegs.ClampU8((int)mac1 >> 4);
        rgb2[1] = GteRegs.ClampU8((int)mac2 >> 4);
        rgb2[2] = GteRegs.ClampU8((int)mac3 >> 4);
        rgb2[3] = rgbc[3];
        return rgbc[3];
    }

    /// <summary>Message 0x64000000 (x, y, w, h, a2) to the host callback when w and h are non-zero; always 0.</summary>
    public static long HostSendRect64(short* a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostSendRect64(a1, a2)", (nint)a1, a2);
        short w = a1[2];
        if (w != 0)
        {
            short h = a1[3];
            if (h != 0 && BattleCallbackAddress != null)
            {
                GteRegs.Callback(0x64000000, GteRegs.Arg(a1[0]), GteRegs.Arg(a1[1]), GteRegs.Arg(w), (ulong)(long)h, a2);
            }
        }

        return 0;
    }

    /// <summary>Message 0x65000000 (x, y, w, h, a2) to the host callback; always 0.</summary>
    public static long HostSendRect65(short* a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("HostSendRect65(a1, a2)", (nint)a1, a2);
        if (BattleCallbackAddress != null)
        {
            GteRegs.Callback(0x65000000, GteRegs.Arg(a1[0]), GteRegs.Arg(a1[1]), GteRegs.Arg(a1[2]), (ulong)(long)a1[3], a2);
        }

        return 0;
    }

    /// <summary>Empty handler: returns 0.</summary>
    public static long sub_180002FD0()
    {
        Recompiler.Runtime.Tracer.Enter("sub_180002FD0()");
        return 0;
    }

    /// <summary>
    /// Clips rect a1 (x, y, w, h) to the 1024x512 VRAM origin (negative x/y move a2/a3 and shrink w/h; x > 1023 or
    /// y > 511 sends nothing) and sends message 0x66000000 (a2, a3, 0, 0, &amp;rect) to the host callback; always 0.
    /// </summary>
    public static long HostSendVramRect66(short* a1, uint a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("HostSendVramRect66(a1, a2, a3)", (nint)a1, a2, a3);
        Span<short> rect = stackalloc short[4];
        short* rectAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(rect));
        short x = a1[0];
        if (x >= 0)
        {
            if (x > 1023)
            {
                return 0;
            }

            rect[0] = x;
            rect[2] = a1[2];
        }
        else
        {
            rect[0] = 0;
            rect[2] = (short)(x + a1[2]);
            a2 -= unchecked((uint)x);
        }

        short y = a1[1];
        if (y >= 0)
        {
            if (y > 511)
            {
                return 0;
            }

            rect[1] = y;
            rect[3] = a1[3];
        }
        else
        {
            rect[1] = 0;
            rect[3] = (short)(y + a1[3]);
            a3 = (uint)(a3 - y);
        }

        if (BattleCallbackAddress != null)
        {
            GteRegs.Callback(0x66000000, a2, (ulong)a3, 0, 0, (long)rectAddress);
        }

        return 0;
    }
}

/// <summary>Sub-word views and clamps over the GTE register globals (file-local helpers of u0011).</summary>
file static unsafe class GteRegs
{
    /// <summary>TRX: low dword of the 8-byte accessor at 0x180211F54.</summary>
    public static int TrX => *(int*)Unsafe.AsPointer(ref Native.qword_180211F54);

    /// <summary>TRY: dword at 0x180211F58.</summary>
    public static int TrY => *(int*)Native.unk_180211F58;

    public static short* S16(ref int global) => (short*)Unsafe.AsPointer(ref global);

    public static short* S16(ref long global) => (short*)Unsafe.AsPointer(ref global);

    public static short ClampS16(int value) => value > 0x7FFF ? (short)0x7FFF : value < -32768 ? short.MinValue : (short)value;

    public static short ClampU15(int value) => value > 0x7FFF ? (short)0x7FFF : value < 0 ? (short)0 : (short)value;

    public static byte ClampU8(int value) => value > 255 ? (byte)255 : value < 0 ? (byte)0 : (byte)value;

    /// <summary>A 16-bit value passed as (unsigned int): sign-extended to 32 bits, zero-extended in the register.</summary>
    public static ulong Arg(short value) => unchecked((uint)value);

    /// <summary>Calls the host callback registered at 0x1801C1DE8 (its result is ignored by these senders).</summary>
    public static void Callback(ulong code, ulong a, ulong b, ulong c, ulong d, long e)
    {
        var callback = (BattleCallbackFn)Native.BattleCallbackAddress;
        Native.InvokeBattleCallback((long)callback, code, a, b, c, d, unchecked((ulong)e));
    }
}
