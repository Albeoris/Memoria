namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Hi_DebugPSGData helper: walks the primitives of part a2 of the PSG model a1 (PSX addresses resolved through the
    /// global memory context) and widens the per-texture-group UV bounding boxes in a3 (<see cref="PsgUvBounds"/>)
    /// unless a3->SkipBounds is set. Returns the last value the original left in rax (callers pass it through).
    /// </summary>
    public static long AccumulatePsgUvBounds(long a1, byte a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("AccumulatePsgUvBounds(a1, a2, a3)", a1, a2, a3);
        Shape_PsgBuildBoneMatrices_a1* shape_PsgBuildBoneMatrices_a1 = (Shape_PsgBuildBoneMatrices_a1*)a1;
        long ctx = (long)unk_180576A10;
        byte* model = (byte*)PsxAddrToPtr(ctx, shape_PsgBuildBoneMatrices_a1->field_8);
        Header* header = (Header*)model;
        byte* parts = (byte*)PsxAddrToPtr(ctx, header->field_16);
        long result = a2;
        PsgMeshPart* part = (PsgMeshPart*)(parts + 40 * a2);
        ushort* uvs = (ushort*)ResolveTracked(part->UvTableAddr, ref result);
        byte* prim = (byte*)ResolveTracked(part->PrimitivesAddr, ref result);
        PsgUvBounds* bounds = (PsgUvBounds*)a3;
        Span<int> indices = stackalloc int[4];

        for (ushort n = part->Quad24Count; n != 0; n--, prim += 24)
        {
            uint ab = *(uint*)(prim + 8);
            uint cd = *(uint*)(prim + 12);
            result = ab;
            if (bounds->SkipBounds == 0)
            {
                indices[0] = (ushort)ab;
                indices[1] = (ushort)(ab >> 16);
                indices[2] = (ushort)cd;
                indices[3] = (ushort)(cd >> 16);
                WidenUvBounds(bounds, uvs, indices, 4, prim[19], ref result);
            }
        }

        for (ushort n = part->Tri20Count; n != 0; n--, prim += 20)
        {
            uint ab = *(uint*)(prim + 12);
            result = ab;
            if (bounds->SkipBounds == 0)
            {
                indices[0] = (ushort)ab;
                indices[1] = (ushort)(ab >> 16);
                indices[2] = *(ushort*)(prim + 16);
                WidenUvBounds(bounds, uvs, indices, 3, prim[6], ref result);
            }
        }

        for (ushort n = part->Quad32Count; n != 0; n--, prim += 32)
        {
            uint ab = *(uint*)(prim + 8);
            uint cd = *(uint*)(prim + 12);
            result = ab;
            if (bounds->SkipBounds == 0)
            {
                indices[0] = (ushort)ab;
                indices[1] = (ushort)(ab >> 16);
                indices[2] = (ushort)cd;
                indices[3] = (ushort)(cd >> 16);
                WidenUvBounds(bounds, uvs, indices, 4, prim[29], ref result);
            }
        }

        for (ushort n = part->Tri24Count; n != 0; n--, prim += 24)
        {
            uint bc = *(uint*)(prim + 8);
            result = bc;
            if (bounds->SkipBounds == 0)
            {
                indices[0] = *(ushort*)(prim + 6);
                indices[1] = (ushort)bc;
                indices[2] = (ushort)(bc >> 16);
                WidenUvBounds(bounds, uvs, indices, 3, prim[22], ref result);
            }
        }

        return result;
    }

    /// <summary>
    /// Folds the UVs (low byte u, high byte v of each table word) of one primitive into group <paramref name="group"/>:
    /// a value below the minimum only lowers the minimum, otherwise it may raise the maximum (the original's else-if).
    /// </summary>
    private static void WidenUvBounds(PsgUvBounds* bounds, ushort* uvs, ReadOnlySpan<int> indices, int count, byte group, ref long result)
    {
        Recompiler.Runtime.Tracer.Enter("WidenUvBounds(bounds, uvs, indices, count, group, result)", (nint)bounds, (nint)uvs, indices.Length, count, group, result);
        ushort* minU = (ushort*)((ushort*)&bounds->MinU);
        ushort* minV = (ushort*)((ushort*)&bounds->MinV);
        ushort* maxU = (ushort*)((ushort*)&bounds->MaxU);
        ushort* maxV = (ushort*)((ushort*)&bounds->MaxV);
        int* u = stackalloc int[4];
        int* v = stackalloc int[4];
        for (int i = 0; i < count; i++)
        {
            ushort uv = uvs[indices[i]];
            u[i] = (byte)uv;
            v[i] = uv >> 8;
        }

        for (int i = 0; i < count; i++)
        {
            if (minU[group] <= u[i])
            {
                if (maxU[group] < u[i])
                {
                    maxU[group] = (ushort)u[i];
                }
            }
            else
            {
                minU[group] = (ushort)u[i];
            }

            result = minV[group];
            if (minV[group] <= v[i])
            {
                result = maxV[group];
                if (maxV[group] < v[i])
                {
                    maxV[group] = (ushort)v[i];
                }
            }
            else
            {
                minV[group] = (ushort)v[i];
            }
        }
    }

    /// <summary>
    /// The inlined PsxAddrToPtr over the global context as it appears for the UV table and primitive list: same
    /// mapping, plus the intermediate values the original leaves in rax on the RAM and segment paths.
    /// </summary>
    private static long ResolveTracked(uint address, ref long result)
    {
        Recompiler.Runtime.Tracer.Enter("ResolveTracked(address, result)", address, result);
        if (address != 0)
        {
            if (address >> 24 == 128)
            {
                result = address & 0xFFFFFFF;
            }
            else
            {
                result = address & 0xC00000;
                if (result == 0xC00000)
                {
                    result = 32L * (address >> 24);
                }
            }
        }

        return PsxAddrToPtr((long)unk_180576A10, address);
    }
}
