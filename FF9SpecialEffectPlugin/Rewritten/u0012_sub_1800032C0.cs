using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// PSX libgte matrix helpers (MATRIX = 3x3 s16 in 4.12 fixed point, 2 bytes pad, s32 translation at +20/+24/+28)
/// and the MIPS register/argument accessors of the recompiled-PSX call context.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>
    /// a2[i] = (int)clamp(a1[i] * 4096.8, -32768, 32767) for the 9 floats (computed in double, NaN gives 32767);
    /// returns the last converted value as a zero-extended 32-bit integer.
    /// </summary>
    public static long FloatMatrixToFixed(float* a1, ushort* a2)
    {
        Recompiler.Runtime.Tracer.Enter("FloatMatrixToFixed(a1, a2)", (nint)a1, (nint)a2);
        for (int i = 0; i < 8; i++)
        {
            double v = a1[i] * 4096.8;
            if (v <= 32767.0)
            {
                if (v < -32768.0)
                {
                    v = -32768.0;
                }
            }
            else
            {
                v = 32767.0;
            }

            a2[i] = (ushort)(int)v;
        }

        double last = a1[8] * 4096.8;
        long result;
        if (last <= 32767.0)
        {
            result = unchecked((uint)-32768);
            if (last >= -32768.0)
            {
                result = (uint)(int)last;
            }
        }
        else
        {
            result = 32767;
        }

        a2[8] = (ushort)result;
        return result;
    }

    /// <summary>
    /// a3 = a1 * a2 for 3x3 4.12 matrices, computed in single precision (each element divided by 4096.8f) and
    /// converted back by FloatMatrixToFixed; all inputs are read before a3 is written (a3 may alias a1 or a2).
    /// </summary>
    public static long MulMatrix0(short* a1, short* a2, ushort* a3)
    {
        Recompiler.Runtime.Tracer.Enter("MulMatrix0(a1, a2, a3)", (nint)a1, (nint)a2, (nint)a3);
        const float Scale = 4096.8f;
        Span<float> m0 = stackalloc float[9];
        Span<float> m1 = stackalloc float[9];
        for (int i = 0; i < 9; i++)
        {
            m0[i] = a1[i] / Scale;
            m1[i] = a2[i] / Scale;
        }

        Span<float> product = stackalloc float[10];
        float* productAddress = (float*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(product));
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                product[row * 3 + col] = m1[3 + col] * m0[row * 3 + 1] + m1[col] * m0[row * 3] + m1[6 + col] * m0[row * 3 + 2];
            }
        }

        FloatMatrixToFixed(productAddress, a3);
        return (long)a3;
    }

    /// <summary>m = Rx(a1) * m, a1 in 4096 units per turn; cos/sin scaled by 4096.8 and truncated.</summary>
    public static long RotMatrixX(short a1, short* a2)
    {
        Recompiler.Runtime.Tracer.Enter("RotMatrixX(a1, a2)", a1, (nint)a2);
        RotationTerms(a1, out short c, out short s);
        Shape_MulMatrix0_a1 r = default;
        r.field_0 = 4096;
        r.field_2 = 0;
        r.field_4 = 0;
        r.field_6 = 0;
        r.field_8 = c;
        r.field_10 = (short)-s;
        r.field_12 = 0;
        r.field_14 = s;
        r.field_16 = c;
        return MulMatrix0(((short*)&r), a2, (ushort*)a2);
    }

    /// <summary>m = Ry(a1) * m.</summary>
    public static long RotMatrixY(short a1, short* a2)
    {
        Recompiler.Runtime.Tracer.Enter("RotMatrixY(a1, a2)", a1, (nint)a2);
        RotationTerms(a1, out short c, out short s);
        Shape_MulMatrix0_a1 r = default;
        r.field_0 = c;
        r.field_2 = 0;
        r.field_4 = s;
        r.field_6 = 0;
        r.field_8 = 4096;
        r.field_10 = 0;
        r.field_12 = (short)-s;
        r.field_14 = 0;
        r.field_16 = c;
        return MulMatrix0(((short*)&r), a2, (ushort*)a2);
    }

    /// <summary>m = Rz(a1) * m.</summary>
    public static long RotMatrixZ(short a1, short* a2)
    {
        Recompiler.Runtime.Tracer.Enter("RotMatrixZ(a1, a2)", a1, (nint)a2);
        RotationTerms(a1, out short c, out short s);
        Shape_MulMatrix0_a1 r = default;
        r.field_0 = c;
        r.field_2 = (short)-s;
        r.field_4 = 0;
        r.field_6 = s;
        r.field_8 = c;
        r.field_10 = 0;
        r.field_12 = 0;
        r.field_14 = 0;
        r.field_16 = 4096;
        return MulMatrix0(((short*)&r), a2, (ushort*)a2);
    }

    /// <summary>a2 = identity (rotation part only), then Rz(a1[2]), Rx(a1[0]), Ry(a1[1]): a2 = Ry * Rx * Rz.</summary>
    public static long RotMatrixYXZ(short* a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("RotMatrixYXZ(a1, a2)", (nint)a1, a2);
        *(uint*)(a2 + 14) = 0x10000000;
        *(uint*)(a2 + 6) = 0x10000000;
        *(uint*)a2 = 4096;
        *(ushort*)(a2 + 4) = 0;
        *(uint*)(a2 + 10) = 0;
        long m = RotMatrixZ(a1[2], (short*)a2);
        m = RotMatrixX(a1[0], (short*)m);
        return RotMatrixY(a1[1], (short*)m);
    }

    /// <summary>Column j of a1 multiplied by a2[j] (unsigned 32-bit product, >> 12, truncated to 16 bits).</summary>
    public static short* ScaleMatrix(short* a1, uint* a2)
    {
        Recompiler.Runtime.Tracer.Enter("ScaleMatrix(a1, a2)", (nint)a1, (nint)a2);
        for (int i = 0; i < 9; i++)
        {
            a1[i] = unchecked((short)((a2[i % 3] * (uint)a1[i]) >> 12));
        }

        return a1;
    }

    /// <summary>a1 = RT * a1, RT taken from a copy of the GTE rotation/translation registers (0x180211F40, 32 bytes).</summary>
    public static long MulRotMatrix(short* a1)
    {
        Recompiler.Runtime.Tracer.Enter("MulRotMatrix(a1)", (nint)a1);
        short* rt = stackalloc short[16];
        System.Buffer.MemoryCopy(xmmword_180211F40, rt, 32, 32);
        return MulMatrix0(rt, a1, (ushort*)a1);
    }

    /// <summary>
    /// GTE MVMVA RT * IR (sf=1, lm=0, no translation): MAC1..3 = (RT * IR) >> 12, low halves of IR1..3 = MAC saturated
    /// to signed 16 bits; returns the register leftover 0xFFFF8000.
    /// </summary>
    public static long GteRtIr()
    {
        Recompiler.Runtime.Tracer.Enter("GteRtIr()");
        short* rt = (short*)xmmword_180211F40;
        short rt33 = (short)dword_180211F50;
        short* ir1 = (short*)Unsafe.AsPointer(ref dword_180211FE4);
        short* ir23 = (short*)Unsafe.AsPointer(ref qword_180211FE8);
        int x = ir1[0];
        int y = ir23[0];
        int z = ir23[2];

        int mac1 = unchecked(x * rt[0] + z * rt[2] + y * rt[1]) >> 12;
        dword_180212024 = mac1;
        int mac2 = unchecked(x * rt[3] + z * rt[5] + y * rt[4]) >> 12;
        dword_180212028 = mac2;
        int mac3 = unchecked(z * rt33 + y * rt[7] + x * rt[6]) >> 12;
        dword_18021202C = mac3;

        ir1[0] = IntplSatS16(mac1);
        ir23[0] = IntplSatS16(mac2);
        ir23[2] = IntplSatS16(mac3);
        return 4294934528L;
    }

    /// <summary>
    /// RT = rotation of a1; for each column j of a2: IR1..3 = column (sign-extended), GteRtIr, IR sign-extended from
    /// its low half and stored as column j of a3. Returns IR3 of the last column as a zero-extended 32-bit value.
    /// </summary>
    public static long GteMulMatrix0(long a1, short* a2, ushort* a3)
    {
        Recompiler.Runtime.Tracer.Enter("GteMulMatrix0(a1, a2, a3)", a1, (nint)a2, (nint)a3);
        *(ulong*)xmmword_180211F40 = *(ulong*)a1;
        *(ulong*)(xmmword_180211F40 + 8) = *(ulong*)(a1 + 8);
        dword_180211F50 = *(ushort*)(a1 + 16);

        int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        long result = 0;
        for (int col = 0; col < 3; col++)
        {
            dword_180211FE4 = a2[col];
            ir23[0] = a2[col + 3];
            ir23[1] = a2[col + 6];
            GteRtIr();
            dword_180211FE4 = (short)dword_180211FE4;
            a3[col] = (ushort)dword_180211FE4;
            ir23[0] = (short)ir23[0];
            a3[col + 3] = (ushort)ir23[0];
            result = (uint)(short)ir23[1];
            ir23[1] = (int)result;
            a3[col + 6] = (ushort)result;
        }

        return result;
    }

    /// <summary>
    /// PSX CompMatrix through the GTE: a3.m = a1.m * a2.m, TR = a1.t, V0 = low halves of a2.t plus the s16 at a2+32,
    /// a3.t = MAC1..3 of GteRtV0Tr (a1.m * a2.t + a1.t). Returns a3.
    /// </summary>
    public static long CompMatrix(long a1, short* a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("CompMatrix(a1, a2, a3)", a1, (nint)a2, a3);
        GteMulMatrix0(a1, a2, (ushort*)a3);
        *(uint*)Unsafe.AsPointer(ref qword_180211F54) = *(uint*)(a1 + 20);
        *(ulong*)unk_180211F58 = *(ulong*)(a1 + 24);
        short* v0 = (short*)Unsafe.AsPointer(ref qword_180211FC0);
        v0[0] = a2[10];
        v0[1] = a2[12];
        v0[2] = a2[14];
        v0[3] = a2[16];
        GteRtV0Tr();
        *(int*)(a3 + 20) = dword_180212024;
        *(int*)(a3 + 24) = dword_180212028;
        *(int*)(a3 + 28) = dword_18021202C;
        return a3;
    }

    /// <summary>MIPS register write: register a2 of the bank a1 = a3, ignored for r0. Returns a2 (0 for r0).</summary>
    public static long PsxSetGpr(long a1, int a2, int a3)
    {
        Recompiler.Runtime.Tracer.Enter("PsxSetGpr(a1, a2, a3)", a1, a2, a3);
        if (a2 != 0)
        {
            *(int*)(a1 + 4L * a2) = a3;
        }

        return a2;
    }

    /// <summary>o32 call argument a2 of the current register bank: a0..a3 for 0..3, otherwise the word at sp + 4 * a2.</summary>
    public static long PsxGetCallArg(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxGetCallArg(a1, a2)", a1, a2);
        var context = (PsxCallContext*)a1;
        uint* gpr = CurrentGprBank(context);
        if ((uint)a2 <= 3)
        {
            return gpr[RegA0 + a2];
        }

        return *(uint*)(PsxAddrToPtr((long)context->memory, gpr[RegSp]) + (uint)(4 * a2));
    }

    /// <summary>PsxGetCallArg resolved as a PSX address to a host pointer; 0 when the argument is 0.</summary>
    public static long PsxGetCallArgPtr(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxGetCallArgPtr(a1, a2)", a1, a2);
        var context = (PsxCallContext*)a1;
        uint* gpr = CurrentGprBank(context);
        uint address = (uint)a2 <= 3
            ? gpr[RegA0 + a2]
            : *(uint*)(PsxAddrToPtr((long)context->memory, gpr[RegSp]) + (uint)(4 * a2));
        return address != 0 ? PsxAddrToPtr((long)context->memory, address) : 0;
    }

    private const int RegA0 = 4;
    private const int RegSp = 29;

    /// <summary>The 32 GPRs of the bank selected by PsxCallContext.bank (byte offset bank &lt;&lt; 7).</summary>
    private static uint* CurrentGprBank(PsxCallContext* context)
    {
        Recompiler.Runtime.Tracer.Enter("CurrentGprBank(context)", (nint)context);
        return (uint*)((byte*)((uint*)&context->gprBanks) + ((long)context->bank << 7));
    }

    /// <summary>cos/sin of a 4096-per-turn angle in double, scaled by 4096.8 and truncated to s16.</summary>
    private static void RotationTerms(short angle, out short c, out short s)
    {
        Recompiler.Runtime.Tracer.Enter("RotationTerms(angle)", angle);
        double radians = (double)angle * 0.000244140625 * 360.0 * 3.141592653589793 / 180.0;
        c = (short)(int)(Math.Cos(radians) * 4096.8);
        s = (short)(int)(Math.Sin(radians) * 4096.8);
    }
}
