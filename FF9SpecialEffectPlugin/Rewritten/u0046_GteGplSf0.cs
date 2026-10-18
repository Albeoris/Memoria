using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// GTE GPL with sf=0: MAC1..3 += IR0 * IR1..3 (no shift, int16 views of the IR globals), then
    /// <see cref="GteStoreMacResult"/>. Returns the RGBC code byte.
    /// </summary>
    public static long GteGplSf0()
    {
        Recompiler.Runtime.Tracer.Enter("GteGplSf0()");
        short ir0 = (short)dword_180211FE0;
        short* ir23 = (short*)Unsafe.AsPointer(ref qword_180211FE8);
        int mac1 = unchecked(ir0 * (short)dword_180211FE4 + dword_180212024);
        dword_180212024 = mac1;
        int mac2 = unchecked(ir0 * ir23[0] + dword_180212028);
        dword_180212028 = mac2;
        int mac3 = unchecked(ir0 * ir23[2] + dword_18021202C);
        dword_18021202C = mac3;
        return GteStoreMacResult(mac1, mac2, mac3);
    }

    /// <summary>
    /// Look angles from point a1 to point a2 (s16 x/y/z): IR1 = dx, IR2 = dz, GteSqr; a3[1] = yaw
    /// trunc(-atan2(dx, dz) / 2pi * -4096), a3[2] = 0, a3[0] = -1024 - pitch with pitch =
    /// trunc(-atan2(trunc(sqrt(MAC1 + MAC2)), dy) / 2pi * 4096). Returns the pitch as a zero-extended 32-bit value.
    /// </summary>
    public static long LookAnglesBetween(short* a1, short* a2, ushort* a3)
    {
        Recompiler.Runtime.Tracer.Enter("LookAnglesBetween(a1, a2, a3)", (nint)a1, (nint)a2, (nint)a3);
        int y1 = a1[1];
        int y2 = a2[1];
        int dx = a2[0] - a1[0];
        int dz = a2[2] - a1[2];
        dword_180211FE4 = dx;
        *(int*)Unsafe.AsPointer(ref qword_180211FE8) = dz;
        GteSqr();
        a3[1] = (ushort)Cvttsd2si(-Math.Atan2(dx, dz) / 6.283185307179586 * -4096.0);
        int squared = unchecked(dword_180212024 + dword_180212028);
        a3[2] = 0;
        double horizontal = Cvttsd2si(Math.Sqrt(squared));
        long result = (uint)Cvttsd2si(-Math.Atan2(horizontal, y2 - y1) / 6.283185307179586 * 4096.0);
        a3[0] = (ushort)(-1024 - result);
        return result;
    }

    /// <summary>INTPL of one channel: IR0 = a3, IR1 = a1, RFC = a2; returns MAC1 as a zero-extended 32-bit value.</summary>
    public static long GteIntplScalar(int a1, int a2, int a3)
    {
        Recompiler.Runtime.Tracer.Enter("GteIntplScalar(a1, a2, a3)", a1, a2, a3);
        dword_180211FE0 = a3;
        dword_180211FE4 = a1;
        *(int*)Unsafe.AsPointer(ref qword_180211F94) = a2;
        GteIntpl();
        return (uint)dword_180212024;
    }

    /// <summary>trunc((1 - a3 / a4) * a1 + a2 * (a3 / a4)) in double (cvttsd2si), as a zero-extended 32-bit value.</summary>
    public static long LerpIntRatio(int a1, int a2, int a3, int a4)
    {
        Recompiler.Runtime.Tracer.Enter("LerpIntRatio(a1, a2, a3, a4)", a1, a2, a3, a4);
        double t = (double)a3 / a4;
        return (uint)Cvttsd2si((1.0 - t) * a1 + a2 * t);
    }

    /// <summary>
    /// INTPL of a vector: IR0 = a3, IR1..3 = a1 (s16 x3, sign-extended), FC = a2 (s16 x3); a4 = low halves of MAC1..3.
    /// Returns the low half of MAC3 zero-extended.
    /// </summary>
    public static long GteIntplVector(short* a1, short* a2, int a3, ushort* a4)
    {
        Recompiler.Runtime.Tracer.Enter("GteIntplVector(a1, a2, a3, a4)", (nint)a1, (nint)a2, a3, (nint)a4);
        dword_180211FE0 = a3;
        dword_180211FE4 = a1[0];
        int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        ir23[0] = a1[1];
        ir23[1] = a1[2];
        int* farColor = (int*)Unsafe.AsPointer(ref qword_180211F94);
        farColor[0] = a2[0];
        farColor[1] = a2[1];
        dword_180211F9C = a2[2];
        GteIntpl();
        a4[0] = (ushort)dword_180212024;
        a4[1] = (ushort)dword_180212028;
        long result = (ushort)dword_18021202C;
        a4[2] = (ushort)result;
        return result;
    }
}
