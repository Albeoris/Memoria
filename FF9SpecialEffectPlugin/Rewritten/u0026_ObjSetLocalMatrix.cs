using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Local MATRIX of a model object (at +64: 3x3 s16 rotation, pad, s32 translation at +84/+88/+92) and its stored
/// scale (s16 x3 at +120), applied through the GTE.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>
    /// Only when bit 1 of the flags byte at +0 is set: the rotation at +64 becomes rotation * diag(scale) with the
    /// scale s16s at +120/+122/+124, one column at a time through GteRtIr (results truncated to s16).
    /// </summary>
    public static void ObjApplyMatrixScale(long a1)
    {
        Recompiler.Runtime.Tracer.Enter("ObjApplyMatrixScale(a1)", a1);
        Shape_PsgBuildBoneMatrices_a1* shape_PsgBuildBoneMatrices_a1 = (Shape_PsgBuildBoneMatrices_a1*)a1;
        if ((shape_PsgBuildBoneMatrices_a1->field_0 & 2) == 0)
        {
            return;
        }

        int sx = shape_PsgBuildBoneMatrices_a1->union_120_2;
        System.Buffer.MemoryCopy((void*)(a1 + 64), xmmword_180211F40, 16, 16);
        int m22 = shape_PsgBuildBoneMatrices_a1->union_80_2;
        dword_180211FE4 = sx;
        dword_180211F50 = m22;
        qword_180211FE8 = 0;
        GteRtIr();

        int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        uint sy = (uint)shape_PsgBuildBoneMatrices_a1->union_122_2;
        shape_PsgBuildBoneMatrices_a1->union_64_2 = (ushort)dword_180211FE4;
        shape_PsgBuildBoneMatrices_a1->union_70_2 = (ushort)ir23[0];
        shape_PsgBuildBoneMatrices_a1->union_76_2 = (ushort)ir23[1];
        qword_180211FE8 = sy;
        dword_180211FE4 = 0;
        GteRtIr();

        int sz = shape_PsgBuildBoneMatrices_a1->field_124;
        shape_PsgBuildBoneMatrices_a1->union_66_2 = (ushort)dword_180211FE4;
        shape_PsgBuildBoneMatrices_a1->union_72_2 = (ushort)ir23[0];
        shape_PsgBuildBoneMatrices_a1->union_78_2 = (ushort)ir23[1];
        dword_180211FE4 = 0;
        ir23[1] = sz;
        ir23[0] = 0;
        GteRtIr();

        dword_180211FE4 = (short)dword_180211FE4;
        shape_PsgBuildBoneMatrices_a1->field_68 = (ushort)dword_180211FE4;
        ir23[0] = (short)ir23[0];
        shape_PsgBuildBoneMatrices_a1->union_74_2 = (ushort)ir23[0];
        ir23[1] = (short)ir23[1];
        shape_PsgBuildBoneMatrices_a1->union_80_2 = (ushort)ir23[1];
    }

    /// <summary>
    /// Rebuilds the local MATRIX at +64: identity rotated by Rz(a2[2]), Rx(a2[0]), Ry(a2[1]) (all 0 without
    /// <paramref name="a2"/>, still multiplied), translation from the three s32 of <paramref name="a3"/> (0 without),
    /// then scaled by diag(a4[0], a4[2], a4[4]) through GteMulMatrix0; without <paramref name="a4"/> the stored scale
    /// is reset to 4096 x3 and ObjApplyMatrixScale applies it (only for objects with flag bit 1).
    /// </summary>
    public static void ObjSetLocalMatrix(long a1, short* a2, uint* a3, short* a4)
    {
        Recompiler.Runtime.Tracer.Enter("ObjSetLocalMatrix(a1, a2, a3, a4)", a1, (nint)a2, (nint)a3, (nint)a4);
        Shape_PsgBuildBoneMatrices_a1* shape_PsgBuildBoneMatrices_a1 = (Shape_PsgBuildBoneMatrices_a1*)a1;
        long m = a1 + 64;
        shape_PsgBuildBoneMatrices_a1->union_78_4 = 0x10000000;
        shape_PsgBuildBoneMatrices_a1->union_70_4 = 0x10000000;
        shape_PsgBuildBoneMatrices_a1->field_68 = 0;
        shape_PsgBuildBoneMatrices_a1->union_64_4 = 4096;
        shape_PsgBuildBoneMatrices_a1->union_74_4 = 0;

        short* rotated;
        short y;
        if (a2 != null)
        {
            rotated = (short*)RotMatrixZ(a2[2], (short*)m);
            rotated = (short*)RotMatrixX(a2[0], rotated);
            y = a2[1];
        }
        else
        {
            rotated = (short*)RotMatrixZ(0, (short*)m);
            rotated = (short*)RotMatrixX(0, rotated);
            y = 0;
        }

        RotMatrixY(y, rotated);

        if (a3 != null)
        {
            *(uint*)(m + 20) = a3[0];
            *(uint*)(m + 24) = a3[1];
            *(uint*)(m + 28) = a3[2];
        }
        else
        {
            *(ulong*)(m + 20) = 0;
            *(uint*)(m + 28) = 0;
        }

        if (a4 != null)
        {
            // Scale MATRIX on the stack: the 16-byte constant at 0x18004B700 (4096 at [0] and [4]), then [8..15].
            Stack32_DrawEffModelByBone_scale scale = default;
            System.Buffer.MemoryCopy(xmmword_18004B700, ((short*)&scale), 16, 16);
            scale.field_0 = a4[0];
            scale.field_8 = a4[2];
            scale.union_16_8 = 4096;
            scale.union_16_2 = a4[4];
            scale.field_24 = 0;
            GteMulMatrix0(m, ((short*)&scale), (ushort*)m);
        }
        else
        {
            shape_PsgBuildBoneMatrices_a1->union_120_4 = 268439552;
            shape_PsgBuildBoneMatrices_a1->field_124 = 4096;
            ObjApplyMatrixScale(a1);
        }
    }
}
