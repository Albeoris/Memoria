using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// 0 when <paramref name="a1"/> lies outside [a2, a2 + a3]; otherwise *a6 = a5 * (a1 - a2) / a3 (mirrored to
    /// a5 - value when <paramref name="a4"/> is negative) and 1. Signed 32-bit arithmetic, a3 == 0 faults.
    /// </summary>
    public static long SfxRangeFraction(int a1, int a2, int a3, int a4, int a5, int* a6)
    {
        Recompiler.Runtime.Tracer.Enter("SfxRangeFraction(a1, a2, a3, a4, a5, a6)", a1, a2, a3, a4, a5, (nint)a6);
        if (a1 < a2 || a1 > unchecked(a2 + a3))
        {
            return 0;
        }

        int value = unchecked(a5 * (a1 - a2)) / a3;
        *a6 = value;
        if (a4 < 0)
        {
            *a6 = unchecked(a5 - value);
        }

        return 1;
    }

    /// <summary>
    /// Builds RT = Ry(a2[1]) * Rx(a2[0]) * Rz(a2[2]) (4096 units per turn) and TR = the s16 position
    /// <paramref name="a1"/>, transforms V0 = (0, 0, a3) and stores MAC1..3 as three s16 at <paramref name="a4"/>;
    /// then reloads the view matrix (slot 5 of off_180066C68) into the GTE and returns its TRZ.
    /// </summary>
    public static long SfxRotTransPointZ(short* a1, short* a2, int a3, ushort* a4)
    {
        Recompiler.Runtime.Tracer.Enter("SfxRotTransPointZ(a1, a2, a3, a4)", (nint)a1, (nint)a2, a3, (nint)a4);
        Matrix matrix = default;
        matrix.field_0 = 4096;
        matrix.field_4 = 0;
        matrix.field_6 = 0x10000000;
        matrix.union_10_8 = 0x1000000000000000UL;
        var m = (short*)RotMatrixZ(a2[2], ((short*)&matrix));
        m = (short*)RotMatrixX(a2[0], m);
        RotMatrixY(a2[1], m);

        byte* gte = xmmword_180211F40;
        dword_180211F50 = (ushort)matrix.union_16_2;
        *(UInt128*)gte = *(UInt128*)((short*)&matrix);
        *(int*)(gte + 20) = a1[0];
        *(int*)(gte + 24) = a1[1];
        *(int*)(gte + 28) = a1[2];
        return SfxRotTransPointZFinish(a3, a4);
    }

    /// <summary>
    /// Loads the 32-byte GTE matrix <paramref name="a1"/> (RT + TR), transforms V0 = (0, 0, a2) and stores MAC1..3 as
    /// three s16 at <paramref name="a3"/>; then reloads the view matrix and returns its TRZ.
    /// </summary>
    public static long SfxMatrixPointZ(long a1, int a2, ushort* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxMatrixPointZ(a1, a2, a3)", a1, a2, (nint)a3);
        GslLoadMatrix((byte*)a1);
        return SfxRotTransPointZFinish(a2, a3);
    }

    /// <summary>
    /// Loads the GTE matrix <paramref name="a1"/>, transforms V0 = (0, 0, 4096) to the direction d = MAC1..3 and stores
    /// the angles that aim along it at <paramref name="a2"/> (4096 units per turn): [1] = yaw from atan2(d.x, d.z),
    /// [0] = -1024 - pitch from atan2((int)sqrt(d.x^2 + d.z^2) via GTE SQR, d.y), [2] = 0. Then reloads the view matrix
    /// and returns its TRZ.
    /// </summary>
    public static long SfxMatrixAimAngles(long a1, ushort* a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxMatrixAimAngles(a1, a2)", a1, (nint)a2);
        GslLoadMatrix((byte*)a1);
        qword_180211FC0 = 0x100000000000L;
        GteRtV0Tr();
        int y = (short)dword_180212028;
        int x = (short)dword_180212024;
        int z = (short)dword_18021202C;
        dword_180211FE4 = x;
        *(int*)Unsafe.AsPointer(ref qword_180211FE8) = z;
        GteSqr();

        a2[1] = (ushort)Cvttsd2si(-Math.Atan2(x, z) / 6.283185307179586 * -4096.0);
        int squared = unchecked(dword_180212024 + dword_180212028);
        a2[2] = 0;
        double horizontal = Cvttsd2si(Math.Sqrt(squared));
        double pitch = Math.Atan2(horizontal, y);
        a2[0] = (ushort)(-1024 - Cvttsd2si(-pitch / 6.283185307179586 * 4096.0));
        return SfxRotTransPointZLoadView();
    }

    /// <summary>
    /// Shortest signed distance between the 12-bit angles a1 and a2: *a3 (when not null) = |a1 - a2| folded to at most
    /// 2048; returns 1 when a1 is ahead (0xFFFFFFFF otherwise), flipped when the difference wrapped.
    /// </summary>
    public static long SfxAngleDelta(short a1, short a2, int* a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxAngleDelta(a1, a2, a3)", a1, a2, (nint)a3);
        int to = a2 & 0xFFF;
        int from = a1 & 0xFFF;
        uint sign = from > to ? 1u : 0xFFFFFFFFu;
        int delta = Math.Abs(from - to);
        if (delta > 2048)
        {
            sign = (uint)-(int)sign;
            delta = 4096 - delta;
        }

        if (a3 != null)
        {
            *a3 = delta;
        }

        return sign;
    }

    /// <summary>
    /// Turns the 12-bit angles a2[0], a2[1] towards a1[0], a1[1] along the shortest way by the fraction a3/4096 of
    /// each distance. Returns the step of the second angle as an unsigned 32-bit value.
    /// </summary>
    public static long SfxTurnTowards(ushort* a1, ushort* a2, int a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxTurnTowards(a1, a2, a3)", (nint)a1, (nint)a2, a3);
        int sign1 = -1;
        int from0 = *a1 & 0xFFF;
        int sign0 = -1;
        int to0 = *a2 & 0xFFF;
        if (from0 > to0)
        {
            sign0 = 1;
        }

        int delta0 = Math.Abs(from0 - to0);
        if (delta0 > 2048)
        {
            sign0 = -sign0;
            delta0 = 4096 - delta0;
        }

        short second = (short)a2[1];
        int to1 = second & 0xFFF;
        *a2 = (ushort)(*a2 + ((a3 * delta0) >> 12) * sign0);
        int from1 = a1[1] & 0xFFF;
        if (from1 > to1)
        {
            sign1 = 1;
        }

        int delta1 = Math.Abs(from1 - to1);
        if (delta1 > 2048)
        {
            sign1 = -sign1;
            delta1 = 4096 - delta1;
        }

        long result = (uint)((a3 * delta1) >> 12);
        a2[1] = (ushort)(second + result * sign1);
        return result;
    }

    /// <summary>
    /// Loads the view matrix (slot 5 of off_180066C68), transforms V0 = *a1 with RT * V0 + TR and stores MAC1..3 as
    /// the translation (+20, +24, +28) of the matrix <paramref name="a2"/> (a stack scratch matrix when null), which
    /// also becomes the GTE TR. Returns the new TRZ.
    /// </summary>
    public static long SfxViewTransformTr(long* a1, byte* a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxViewTransformTr(a1, a2)", (nint)a1, (nint)a2);
        SfxRotTransPointZLoadView();
        qword_180211FC0 = *a1;
        GteRtV0Tr();
        byte* scratch = stackalloc byte[32];
        byte* target = a2 != null ? a2 : scratch;
        *(int*)(target + 20) = dword_180212024;
        *(long*)(target + 24) = *(long*)Unsafe.AsPointer(ref dword_180212028);
        byte* gte = xmmword_180211F40;
        *(int*)(gte + 20) = *(int*)(target + 20);
        *(int*)(gte + 24) = *(int*)(target + 24);
        *(int*)(gte + 28) = *(int*)(target + 28);
        return *(uint*)(target + 28);
    }

    /// <summary>V0 = (0, 0, z); RT * V0 + TR; MAC1..3 as three s16 at <paramref name="output"/>; reloads the view matrix.</summary>
    private static long SfxRotTransPointZFinish(int z, ushort* output)
    {
        Recompiler.Runtime.Tracer.Enter("SfxRotTransPointZFinish(z, output)", z, (nint)output);
        *(int*)Unsafe.AsPointer(ref qword_180211FC0) = 0;
        *((int*)Unsafe.AsPointer(ref qword_180211FC0) + 1) = z;
        GteRtV0Tr();
        output[0] = (ushort)dword_180212024;
        output[1] = (ushort)dword_180212028;
        output[2] = (ushort)dword_18021202C;
        return SfxRotTransPointZLoadView();
    }

    /// <summary>Loads the view matrix of slot 5 of off_180066C68 (RT and TR) into the GTE; returns its TRZ zero-extended.</summary>
    private static long SfxRotTransPointZLoadView()
    {
        Recompiler.Runtime.Tracer.Enter("SfxRotTransPointZLoadView()");
        byte* view = (byte*)PsxAddrToPtr((long)unk_180576A10, ((uint*)off_180066C68)[5]);
        GslLoadMatrix(view);
        return *(uint*)(view + 28);
    }
}
