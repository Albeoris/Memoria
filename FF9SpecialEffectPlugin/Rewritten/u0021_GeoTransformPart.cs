using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// geo.cpp vertex pass for part <paramref name="a2"/> of the PsgModelHandle <paramref name="a1"/>. For every bone
    /// (PsgModel.BoneCount, 32-byte GTE matrices at handle+56) with a non-zero vertex count (u16 table at part Addr20):
    /// combines the view matrix of slot 5 of off_180066C68 with the bone matrix (RT = view.RT * bone.RT through
    /// GteMulMatrix0, TR = view.RT * bone.TR + view.TR through GteRtV0Tr), then projects the bone's 8-byte vertices of
    /// part Addr24 three at a time with RTPS, writing SXY0..2 to the table at 0x1802191A0 and SZ1..3 to the table at
    /// 0x180212440 (both indexed by the running vertex count). Returns 0.
    /// </summary>
    public static long GeoTransformPart(long a1, byte a2)
    {
        Recompiler.Runtime.Tracer.Enter("GeoTransformPart(a1, a2)", a1, a2);
        long mapper = (long)unk_180576A10;
        var handle = (PsgModelHandle*)a1;
        var model = (byte*)PsxAddrToPtr(mapper, handle->ModelAddr);
        var parts = (byte*)PsxAddrToPtr(mapper, ((PsgModel*)model)->PartsAddr);
        var part = (PsgMeshPart*)(parts + 40 * a2);
        short* bone = (short*)handle->field_56;
        long result = model[2];
        uint vcount = 0;
        long countOffset = 0;
        int* sxyTable = (int*)Unsafe.AsPointer(ref dword_1802191A0);
        int* szTable = (int*)Unsafe.AsPointer(ref dword_180212440);
        ushort* combined = stackalloc ushort[16];
        byte* gte = xmmword_180211F40;

        while (result != 0)
        {
            var counts = (byte*)PsxAddrToPtr(mapper, part->Addr20);
            int count = *(ushort*)(counts + countOffset);
            countOffset += 2;
            if (count != 0)
            {
                if (vcount >= 0x1B58)
                {
                    GeoWassert("vcount < GEO_MAX_VERTICES", GeoCppFile, 0x59);
                }

                int* sz = szTable + vcount;
                int* sxy = sxyTable + vcount;
                var vertex = (int*)((byte*)PsxAddrToPtr(mapper, part->Addr24) + 8L * vcount);
                vcount += (uint)count;

                uint* table = (uint*)off_180066C68;
                GteMulMatrix0(PsxAddrToPtr(mapper, table[5]), bone, combined);
                GslLoadMatrix((byte*)PsxAddrToPtr(mapper, table[5]));

                // V0 = low halves of the bone's TRX/TRY/TRZ; the high word of VZ0 is the s16 after the matrix.
                short* v0 = (short*)Unsafe.AsPointer(ref qword_180211FC0);
                v0[0] = bone[10];
                v0[1] = bone[12];
                v0[2] = bone[14];
                v0[3] = bone[16];
                GteRtV0Tr();

                *(UInt128*)gte = *(UInt128*)combined;
                dword_180211F50 = combined[8];
                *(int*)(gte + 20) = dword_180212024;
                *(long*)(gte + 24) = *(long*)Unsafe.AsPointer(ref dword_180212028);

                GeoTransformPartLoadVertices(vertex);
                do
                {
                    vertex += 6;
                    count -= 3;
                    GteRtps(0);
                    GteRtps(1);
                    GteRtps(2);
                    GeoTransformPartLoadVertices(vertex);
                    sxy[0] = dword_180211FF0;
                    sxy[1] = dword_180211FF4;
                    sxy[2] = dword_180211FF8;
                    sz[0] = dword_180212004;
                    sz[1] = dword_180212008;
                    sz[2] = dword_18021200C;
                    sxy += 3;
                    sz += 3;
                }
                while (count > 0);
            }

            bone += 16;
            --result;
        }

        return result;
    }

    /// <summary>Loads three 8-byte vertices into the GTE vector registers V0..V2 (0x180211FC0..0x180211FD7).</summary>
    private static void GeoTransformPartLoadVertices(int* vertex)
    {
        Recompiler.Runtime.Tracer.Enter("GeoTransformPartLoadVertices(vertex)", (nint)vertex);
        qword_180211FC0 = *(long*)vertex;
        dword_180211FC8 = vertex[2];
        dword_180211FCC = vertex[3];
        dword_180211FD0 = vertex[4];
        dword_180211FD4 = vertex[5];
    }
}
