using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// GTE GPF with sf=0: MAC1..3 = IR0 * IR1..3 (no shift) over the int16 views of the IR globals, then the shared
    /// IR/RGB FIFO tail. Returns the RGBC code byte.
    /// </summary>
    public static long GteGpfSf0()
    {
        Recompiler.Runtime.Tracer.Enter("GteGpfSf0()");
        short ir0 = (short)dword_180211FE0;
        short* ir23 = (short*)Unsafe.AsPointer(ref qword_180211FE8);
        int mac1 = ir0 * (short)dword_180211FE4;
        int mac2 = ir0 * ir23[0];
        dword_180212024 = mac1;
        dword_180212028 = mac2;
        int mac3 = ir0 * ir23[2];
        dword_18021202C = mac3;
        return GteStoreMacResult(mac1, mac2, mac3);
    }

    /// <summary>
    /// Host pointer of record (a1[31] - 1) of the table described by <paramref name="a2"/> (+20 PSX address, +12 s16
    /// stride); null when the address is 0, the bare offset when the address does not resolve.
    /// </summary>
    public static byte* SfxIndexedRecordPtr(long a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("SfxIndexedRecordPtr(a1, a2)", a1, a2);
        uint address = *(uint*)(a2 + 20);
        if (address == 0)
        {
            return null;
        }

        byte* table = ResolveGlobalPsxAddress(address);
        return table + ((*(byte*)(a1 + 31) - 1) * *(short*)(a2 + 12));
    }

    /// <summary>
    /// Rotates (0, 0, <paramref name="a2"/>) by Ry(a1[1]) * Rx(a1[0]) * Rz(a1[2]) with zero translation on the GTE
    /// (RTV0TR), stores the low words of MAC1..3 to <paramref name="a3"/>, then reloads RT/TR from the matrix at
    /// context word 5. Returns the TRZ dword loaded.
    /// </summary>
    public static long RotateDepthVector(short* a1, int a2, ushort* a3)
    {
        Recompiler.Runtime.Tracer.Enter("RotateDepthVector(a1, a2, a3)", (nint)a1, a2, (nint)a3);
        Span<short> matrix = stackalloc short[16];
        short* matrixAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(matrix));
        RotationToLookAnglesBuildMatrix(matrix, a1[2], a1[0], a1[1]);
        RotationToLookAnglesLoadRotation(matrixAddress);
        qword_180211F54 = 0;
        *(int*)unk_180211F5C = 0;
        qword_180211FC0 = (long)((ulong)(uint)a2 << 32);
        GteRtV0Tr();
        RotationToLookAnglesStoreMac(a3);
        return RotationToLookAnglesRestore();
    }

    /// <summary>
    /// Transforms V0 = (packed VXY <paramref name="a5"/>, VZ <paramref name="a3"/>) by Ry * Rx * Rz of the packed
    /// angles *a4 (x low, y high) and a6 (z) plus translation a1[0..2] (RTV0TR), stores the low words of MAC1..3 to
    /// <paramref name="a2"/>, then reloads RT/TR from the matrix at context word 5. Returns the TRZ dword loaded.
    /// </summary>
    public static long TransformPointByAngles(short* a1, ushort* a2, int a3, int* a4, int a5, short a6)
    {
        Recompiler.Runtime.Tracer.Enter("TransformPointByAngles(a1, a2, a3, a4, a5, a6)", (nint)a1, (nint)a2, a3, (nint)a4, a5, a6);
        int angles = *a4;
        Span<short> matrix = stackalloc short[10];
        short* matrixAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(matrix));
        RotationToLookAnglesBuildMatrix(matrix, a6, (short)angles, (short)(angles >> 16));
        RotationToLookAnglesLoadRotation(matrixAddress);
        *(int*)unk_180211F5C = a1[2];
        *(int*)Unsafe.AsPointer(ref qword_180211F54) = a1[0];
        *(int*)unk_180211F58 = a1[1];
        long* v0 = (long*)Unsafe.AsPointer(ref qword_180211FC0);
        ((int*)v0)[0] = a5;
        ((int*)v0)[1] = a3;
        GteRtV0Tr();
        RotationToLookAnglesStoreMac(a2);
        return RotationToLookAnglesRestore();
    }

    /// <summary>
    /// Direction (x, y, z) = Ry * Rx * Rz(a3) * (4096, 0, 0) for the packed angles *a2 (x low, y high), on the GTE
    /// with zero translation; a1[1] = yaw atan2(x, z) in 4096 units per turn, a1[2] = 0, a1[0] = -1024 minus the
    /// pitch atan2((int)sqrt(x*x + z*z), y) (squares through GTE SQR). Reloads RT/TR from the matrix at context
    /// word 5 and returns the TRZ dword loaded.
    /// </summary>
    public static long RotationToLookAngles(ushort* a1, int* a2, short a3)
    {
        Recompiler.Runtime.Tracer.Enter("RotationToLookAngles(a1, a2, a3)", (nint)a1, (nint)a2, a3);
        int angles = *a2;
        Span<short> matrix = stackalloc short[16];
        short* matrixAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(matrix));
        RotationToLookAnglesBuildMatrix(matrix, a3, (short)angles, (short)(angles >> 16));
        RotationToLookAnglesLoadRotation(matrixAddress);
        qword_180211F54 = 0;
        *(int*)unk_180211F5C = 0;
        qword_180211FC0 = 4096;
        GteRtV0Tr();
        int x = (short)dword_180212024;
        int z = (short)dword_18021202C;
        int y = (short)dword_180212028;
        dword_180211FE4 = x;
        *(int*)Unsafe.AsPointer(ref qword_180211FE8) = z;
        GteSqr();
        a1[1] = (ushort)Cvttsd2si(-Math.Atan2(x, z) / 6.283185307179586 * -4096.0);
        int squares = dword_180212024 + dword_180212028;
        a1[2] = 0;
        double horizontal = Cvttsd2si(Math.Sqrt(squares));
        double pitch = Math.Atan2(horizontal, y);
        a1[0] = (ushort)(-1024 - Cvttsd2si(-pitch / 6.283185307179586 * 4096.0));
        return RotationToLookAnglesRestore();
    }

    /// <summary>
    /// V0 = (*(u32*)a1, *(s16*)(a1 + 4)) through RTV0TR with the current RT/TR; *a2 = MAC3, *a3 = (int)sqrt of the
    /// GTE SQR sum IR1^2 + IR2^2. Returns 1024 + the yaw atan2(MAC2, MAC1) in 4096 units per turn.
    /// </summary>
    public static long VectorToYawDistance(long a1, uint* a2, uint* a3)
    {
        Recompiler.Runtime.Tracer.Enter("VectorToYawDistance(a1, a2, a3)", a1, (nint)a2, (nint)a3);
        Shape_VectorToYawDistance_a1* shape_VectorToYawDistance_a1 = (Shape_VectorToYawDistance_a1*)a1;
        long* v0 = (long*)Unsafe.AsPointer(ref qword_180211FC0);
        ((uint*)v0)[0] = shape_VectorToYawDistance_a1->field_0;
        ((int*)v0)[1] = shape_VectorToYawDistance_a1->field_4;
        GteRtV0Tr();
        int mac1 = dword_180212024;
        int mac2 = dword_180212028;
        *a2 = (uint)dword_18021202C;
        GteSqr();
        *a3 = (uint)Cvttsd2si(Math.Sqrt(dword_180212024 + dword_180212028));
        return (uint)(1024 - Cvttsd2si(-Math.Atan2(mac2, mac1) / 6.283185307179586 * -4096.0));
    }

    /// <summary>Identity s16 3x3 matrix at <paramref name="matrix"/>, then Rz(z), Rx(x), Ry(y) applied in that order.</summary>
    private static void RotationToLookAnglesBuildMatrix(Span<short> matrix, short z, short x, short y)
    {
        Recompiler.Runtime.Tracer.Enter("RotationToLookAnglesBuildMatrix(matrix, z, x, y)", matrix.Length, z, x, y);
        short* matrixAddress = (short*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(matrix));
        for (int i = 0; i < 9; i++)
        {
            matrix[i] = 0;
        }

        matrix[0] = 4096;
        matrix[4] = 4096;
        matrix[8] = 4096;
        short* m = (short*)RotMatrixZ(z, matrixAddress);
        m = (short*)RotMatrixX(x, m);
        RotMatrixY(y, m);
    }

    /// <summary>GTE RT = the s16 3x3 matrix (RT11..RT32 as 16 bytes, RT33 zero-extended into dword_180211F50).</summary>
    private static void RotationToLookAnglesLoadRotation(short* matrix)
    {
        Recompiler.Runtime.Tracer.Enter("RotationToLookAnglesLoadRotation(matrix)", (nint)matrix);
        *(UInt128*)xmmword_180211F40 = *(UInt128*)matrix;
        dword_180211F50 = (ushort)matrix[8];
    }

    /// <summary>Stores the low words of MAC1..3 to <paramref name="target"/>[0..2].</summary>
    private static void RotationToLookAnglesStoreMac(ushort* target)
    {
        Recompiler.Runtime.Tracer.Enter("RotationToLookAnglesStoreMac(target)", (nint)target);
        target[0] = (ushort)dword_180212024;
        target[1] = (ushort)dword_180212028;
        target[2] = (ushort)dword_18021202C;
    }

    /// <summary>Reloads GTE RT/TR from the 32-byte matrix at context word 5; returns its TRZ dword.</summary>
    private static long RotationToLookAnglesRestore()
    {
        Recompiler.Runtime.Tracer.Enter("RotationToLookAnglesRestore()");
        byte* matrix = ResolveGlobalPsxAddress(ContextAddress(5));
        GslLoadMatrix(matrix);
        return *(uint*)(matrix + 28);
    }
}
