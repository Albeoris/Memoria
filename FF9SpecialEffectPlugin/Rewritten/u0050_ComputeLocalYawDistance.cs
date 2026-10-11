using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Yaw and distance of the vector a1 - a2 seen from the angles a3 (X = -a3[0], Y = -a3[1], Z = 0): loads that
    /// rotation as GTE RT with TR = 0, runs VectorToYawDistance (MAC3 to *a4, planar distance to *a5, yaw to *a6),
    /// then restores RT and TR from the camera matrix at PSX table slot 5. Returns TRZ.
    /// </summary>
    public static long ComputeLocalYawDistance(ushort* a1, ushort* a2, ushort* a3, uint* a4, uint* a5, uint* a6)
    {
        Recompiler.Runtime.Tracer.Enter("ComputeLocalYawDistance(a1, a2, a3, a4, a5, a6)", (nint)a1, (nint)a2, (nint)a3, (nint)a4, (nint)a5, (nint)a6);
        Span<short> delta = stackalloc short[4];
        short* deltaAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(delta));
        delta[0] = (short)(a1[0] - a2[0]);
        delta[1] = (short)(a1[1] - a2[1]);
        delta[2] = (short)(a1[2] - a2[2]);

        // MATRIX m (3x3 s16 + pad + t[3]); only the identity part is initialised, as in the original.
        Matrix matrix = default;
        ((short*)&matrix)[0] = 0x1000;
        ((short*)&matrix)[1] = 0;
        matrix.field_4 = 0;
        ((short*)&matrix)[3] = 0;
        ((short*)&matrix)[4] = 0x1000;
        ((short*)&matrix)[5] = 0;
        ((short*)&matrix)[6] = 0;
        ((short*)&matrix)[7] = 0;
        matrix.union_16_2 = 0x1000;

        short angleX = (short)-(short)a3[0];
        short angleY = (short)-(short)a3[1];
        long m = RotMatrixZ(0, ((short*)&matrix));
        m = RotMatrixY(angleY, (short*)m);
        RotMatrixX(angleX, (short*)m);

        // RT = rotation, TR = 0.
        System.Buffer.MemoryCopy(((short*)&matrix), xmmword_180211F40, 16, 16);
        dword_180211F50 = (ushort)matrix.union_16_2;
        qword_180211F54 = 0;
        dword_180211F5C = 0;

        *a6 = (uint)VectorToYawDistance((long)deltaAddress, a4, a5);

        uint* table = (uint*)off_180066C68;
        byte* camera = (byte*)PsxAddrToPtr((long)unk_180576A10, table[5]);
        uint* rt = (uint*)xmmword_180211F40;
        rt[0] = ((Camera*)camera)->field_0;
        rt[1] = ((Camera*)camera)->field_4;
        rt[2] = ((Camera*)camera)->field_8;
        rt[3] = ((Camera*)camera)->field_12;
        dword_180211F50 = ((Camera*)camera)->field_16;

        camera = (byte*)PsxAddrToPtr((long)unk_180576A10, table[5]);
        *(uint*)Unsafe.AsPointer(ref qword_180211F54) = ((Camera*)camera)->field_20;
        *(uint*)unk_180211F58 = ((Camera*)camera)->field_24;
        uint trz = ((Camera*)camera)->field_28;
        dword_180211F5C = (int)trz;
        return trz;
    }

    /// <summary>
    /// GTE OP (sf = 1, lm = 0): outer product of the RT diagonal (R11, R22, R33) and IR1..3 into MAC1..3, IR1..3 =
    /// MAC clamped to s16 (only the low halves of the IR dwords are written). Returns the clamp constant 0xFFFF8000
    /// left in eax.
    /// </summary>
    public static long GteOp()
    {
        Recompiler.Runtime.Tracer.Enter("GteOp()");
        short* ir1 = (short*)Unsafe.AsPointer(ref dword_180211FE4);
        short* ir2 = (short*)Unsafe.AsPointer(ref qword_180211FE8);
        short* ir3 = (short*)unk_180211FEC;
        short* rt = (short*)xmmword_180211F40;
        int d1 = rt[0];
        int d2 = rt[4];
        int d3 = (short)dword_180211F50;

        int mac1 = (int)(((long)(d2 * *ir3) - (long)(d3 * *ir2)) >> 12);
        int mac2 = (int)(((long)(d3 * *ir1) - (long)(d1 * *ir3)) >> 12);
        int mac3 = (int)(((long)(d1 * *ir2) - (long)(d2 * *ir1)) >> 12);
        dword_180212024 = mac1;
        dword_180212028 = mac2;
        dword_18021202C = mac3;

        *ir1 = IntplSatS16(mac1);
        *ir2 = IntplSatS16(mac2);
        *ir3 = IntplSatS16(mac3);
        return 0xFFFF8000L;
    }
}
