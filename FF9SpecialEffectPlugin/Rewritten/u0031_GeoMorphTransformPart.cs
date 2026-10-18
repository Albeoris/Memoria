using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    private const string GeoMorphFile = @"..\..\SpecialEffectCode\sonoda\Geo\geomorph.cpp";

    /// <summary>
    /// geomorph.cpp vertex pass for part <paramref name="a2"/> of the PsgModelHandle <paramref name="a1"/>, morphing
    /// towards the same part of the PsgModel at handle+128 with the factor <paramref name="a3"/> (GTE IR0, 4096 = 1).
    /// For every bone (PsgModel.BoneCount, 32-byte matrices at handle+56) with a non-zero vertex count (u16 table at
    /// part Addr20): combines the view matrix of slot 5 of off_180066C68 with the bone matrix (RT through GteRtIr per
    /// column, TR = view.RT * bone.TR + view.TR), interpolates each vertex with INTPL (IR = target vertex, FC = source
    /// vertex: target + a3 * (source - target)) into the screen tables at 0x1802191A0 (packed x | y &lt;&lt; 16) and
    /// 0x180212440 (z), then projects them in place three at a time with RTPS (SXY and SZ1..3). Returns 0.
    /// </summary>
    public static long GeoMorphTransformPart(long a1, byte a2, ushort a3)
    {
        Recompiler.Runtime.Tracer.Enter("GeoMorphTransformPart(a1, a2, a3)", a1, a2, a3);
        long mapper = (long)unk_180576A10;
        var handle = (PsgModelHandle*)a1;
        var model = (byte*)PsxAddrToPtr(mapper, handle->ModelAddr);
        var parts = (byte*)PsxAddrToPtr(mapper, ((PsgModel*)model)->PartsAddr);
        var part = (PsgMeshPart*)(parts + 40 * a2);
        var morphParts = (byte*)PsxAddrToPtr(mapper, ((PsgModel*)handle->field_128)->PartsAddr);
        var morphPart = (PsgMeshPart*)(morphParts + 40 * a2);
        short* bone = (short*)handle->field_56;
        long result = model[2];
        uint vcount = 0;
        long countOffset = 0;
        int* sxyTable = (int*)Unsafe.AsPointer(ref dword_1802191A0);
        int* szTable = (int*)Unsafe.AsPointer(ref dword_180212440);
        ushort* combined = stackalloc ushort[16];
        byte* gte = xmmword_180211F40;
        int* ir23 = (int*)Unsafe.AsPointer(ref qword_180211FE8);
        int* farColor = (int*)Unsafe.AsPointer(ref qword_180211F94);

        while (result != 0)
        {
            var counts = (byte*)PsxAddrToPtr(mapper, part->Addr20);
            int count = *(ushort*)(counts + countOffset);
            countOffset += 2;
            if (count != 0)
            {
                if (vcount >= 0x1B58)
                {
                    GeoWassert("vcount < GEO_MAX_VERTICES", GeoMorphFile, 0x3F);
                }

                uint* table = (uint*)off_180066C68;
                byte* view = (byte*)PsxAddrToPtr(mapper, table[5]);
                GslLoadMatrix((byte*)PsxAddrToPtr(mapper, table[5]));

                // combined RT = view RT * bone RT, column by column (the view RT is already loaded).
                GteMulMatrix0((long)view, bone, combined);

                var source = (byte*)PsxAddrToPtr(mapper, part->Addr24);
                short* v0 = (short*)Unsafe.AsPointer(ref qword_180211FC0);
                v0[0] = bone[10];
                v0[1] = bone[12];
                v0[2] = bone[14];
                v0[3] = bone[16];
                GteRtV0Tr();

                var target = (byte*)PsxAddrToPtr(mapper, morphPart->Addr24);
                int translationX = dword_180212024;
                long translationYz = *(long*)Unsafe.AsPointer(ref dword_180212028);
                dword_180211FE0 = a3;

                byte* src = source + 8L * vcount;
                byte* dst = target + 8L * vcount;
                int* sxy = sxyTable + vcount;
                int* sz = szTable + vcount;
                for (int n = count; n != 0; n--)
                {
                    farColor[0] = (short)*(int*)src;
                    farColor[1] = *(int*)src >> 16;
                    dword_180211F9C = *(short*)(src + 4);
                    dword_180211FE4 = (short)*(int*)dst;
                    ir23[0] = *(int*)dst >> 16;
                    ir23[1] = *(short*)(dst + 4);
                    src += 8;
                    dst += 8;
                    GteIntpl();
                    *sz = dword_18021202C;
                    *sxy = (ushort)dword_180212024 | (dword_180212028 << 16);
                    sxy++;
                    sz++;
                }

                *(UInt128*)gte = *(UInt128*)combined;
                dword_180211F50 = combined[8];
                *(int*)(gte + 20) = translationX;
                *(long*)(gte + 24) = translationYz;

                sxy = sxyTable + vcount;
                sz = szTable + vcount;
                vcount += (uint)count;
                GeoMorphTransformPartLoad(sxy, sz);
                do
                {
                    count -= 3;
                    GteRtps(0);
                    GteRtps(1);
                    GteRtps(2);
                    GeoMorphTransformPartLoad(sxy + 3, sz + 3);
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

    /// <summary>Loads V0..V2 from three packed XY words and three Z words of the screen tables.</summary>
    private static void GeoMorphTransformPartLoad(int* xy, int* z)
    {
        Recompiler.Runtime.Tracer.Enter("GeoMorphTransformPartLoad(xy, z)", (nint)xy, (nint)z);
        int* v = (int*)Unsafe.AsPointer(ref qword_180211FC0);
        v[0] = xy[0];
        v[1] = z[0];
        dword_180211FC8 = xy[1];
        dword_180211FCC = z[1];
        dword_180211FD0 = xy[2];
        dword_180211FD4 = z[2];
    }
}
