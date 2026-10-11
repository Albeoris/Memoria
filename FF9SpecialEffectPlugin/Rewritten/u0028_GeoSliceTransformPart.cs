using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    private const string GslFile = @"..\..\SpecialEffectCode\sonoda\Geo\geoslice.cpp";

    /// <summary>
    /// geoslice.cpp vertex pass for part <paramref name="a2"/> of the PsgModelHandle <paramref name="a1"/>. For every
    /// bone (PsgModel.BoneCount, 32-byte GTE matrices at handle+56) with a non-zero vertex count (u16 table at part
    /// Addr20): loads the bone matrix, transforms each 8-byte vertex of Addr24 with RT*V0+TR, stores IR1/IR2 as the
    /// vertex's XY (IR2 clamped to handle+126, which also sets bit 28 of the stored Z) and IR3 as its Z; then loads the
    /// matrix of slot 5 of off_180066C68 and projects the bone's vertices three at a time with RTPS, writing SXY and
    /// the low half of SZ back. Vertices are appended to the tables at 0x1802191A0 / 0x180212440. Returns 0.
    /// </summary>
    public static long GeoSliceTransformPart(long a1, byte a2)
    {
        Recompiler.Runtime.Tracer.Enter("GeoSliceTransformPart(a1, a2)", a1, a2);
        long mapper = (long)unk_180576A10;
        var handle = (PsgModelHandle*)a1;
        var model = (byte*)PsxAddrToPtr(mapper, handle->ModelAddr);
        var parts = (byte*)PsxAddrToPtr(mapper, ((PsgModel*)model)->PartsAddr);
        int limit = handle->SliceLimit;
        var part = (PsgMeshPart*)(parts + 40 * a2);
        int* vertex = (int*)PsxAddrToPtr(mapper, part->Addr24);
        long result = model[2];
        byte* bone = (byte*)handle->field_56;
        uint vcount = 0;
        int* sxy = (int*)Unsafe.AsPointer(ref dword_1802191A0);
        int* sz = (int*)Unsafe.AsPointer(ref dword_180212440);
        int* v = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        long countOffset = 0;
        while (result != 0)
        {
            byte* counts = (byte*)PsxAddrToPtr(mapper, part->Addr20);
            uint count = *(ushort*)(counts + countOffset);
            countOffset += 2;
            if (count != 0)
            {
                if (vcount >= 0x1B58)
                {
                    GeoWassert("vcount < GEO_MAX_VERTICES", GslFile, 0x48);
                }

                GslLoadMatrix(bone);
                int* xy = sxy + vcount;
                int* z = sz + vcount;
                for (uint k = 0; k < count; k++)
                {
                    v[0] = vertex[0];
                    v[1] = vertex[1];
                    vertex += 2;
                    GteRtV0Tr();
                    int y = (short)ir23[0];
                    int depth = (short)ir23[1];
                    dword_180211FE4 = (short)dword_180211FE4;
                    ((ushort*)(xy + k))[0] = (ushort)dword_180211FE4;
                    ir23[0] = y;
                    ir23[1] = depth;
                    int stored = (ushort)depth;
                    if (y >= limit)
                    {
                        y = limit;
                        stored = (ushort)depth | 0x10000000;
                    }

                    ((ushort*)(xy + k))[1] = (ushort)y;
                    z[k] = stored;
                }

                if (vcount >= 0x1B58)
                {
                    GeoWassert("vcount < GEO_MAX_VERTICES", GslFile, 0x80);
                }

                GslLoadMatrix((byte*)PsxAddrToPtr(mapper, ((uint*)off_180066C68)[5]));
                uint done = 0;
                do
                {
                    done += 3;
                    v[0] = xy[0];
                    v[1] = z[0];
                    dword_180211FC8 = xy[1];
                    dword_180211FCC = z[1];
                    dword_180211FD0 = xy[2];
                    dword_180211FD4 = z[2];
                    GteRtps(0);
                    GteRtps(1);
                    GteRtps(2);
                    xy[0] = dword_180211FF0;
                    xy[1] = dword_180211FF4;
                    xy[2] = dword_180211FF8;
                    *(ushort*)z = (ushort)dword_180212004;
                    *(ushort*)(z + 1) = (ushort)dword_180212008;
                    *(ushort*)(z + 2) = (ushort)dword_18021200C;
                    xy += 3;
                    z += 3;
                }
                while (done < count);

                vcount += count;
            }

            bone += 32;
            --result;
        }

        return result;
    }

    /// <summary>
    /// Loads a 32-byte GTE matrix into 0x180211F40: RT11..RT32 (16 bytes), RT33 zero-extended into dword_180211F50,
    /// TRX at +0x14 and TRY/TRZ (8 bytes) at +0x18.
    /// </summary>
    private static void GslLoadMatrix(byte* matrix)
    {
        Recompiler.Runtime.Tracer.Enter("GslLoadMatrix(matrix)", (nint)matrix);
        byte* gte = xmmword_180211F40;
        *(UInt128*)gte = *(UInt128*)matrix;
        dword_180211F50 = *(ushort*)(matrix + 16);
        *(uint*)(gte + 20) = *(uint*)(matrix + 20);
        *(ulong*)(gte + 24) = *(ulong*)(matrix + 24);
    }
}
