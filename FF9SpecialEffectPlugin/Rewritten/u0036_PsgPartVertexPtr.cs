namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Hi_GetSplitMdlVertex(id, part): formats "id:%d cannot use!" into Buffer and hangs when effect-model slot
    /// <paramref name="a1"/> is not in use or has no handle; otherwise returns ComputePsgVertexBytes(handle, a2).
    /// The decompiler lost the second argument of this tail call; the caller passes (id, part).
    /// </summary>
    public static long GetSplitMdlVertex(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("GetSplitMdlVertex(a1, a2)", a1, a2);
        var slot = (EffModelSlot*)unk_180220230 + a1;
        if (slot->inUse == 0 || slot->model == 0)
        {
            WriteAnsi(Buffer, FormattableString.Invariant($"Hi_GetSplitMdlVertex()\nid:{a1} cannot use!"));
            HiraishiFatalHang();
        }

        return ComputePsgVertexBytes((long)slot->model, a2);
    }

    /// <summary>
    /// For part <paramref name="a2"/> of the PsgModelHandle <paramref name="a1"/>: the host vertex buffer
    /// <paramref name="a3"/> holds the vertices of the eight primitive kinds back to back (8 bytes per vertex, 4 per
    /// quad, 3 per triangle, in PsgMeshPart count order). Stores the mapper-registered PSX address of kind
    /// <paramref name="a5"/>'s block in *<paramref name="a4"/> and returns that kind's primitive count; any other kind
    /// stores 0 and returns 0.
    /// </summary>
    public static long PsgPartVertexPtr(long a1, ushort a2, ulong a3, uint* a4, int a5)
    {
        Recompiler.Runtime.Tracer.Enter("PsgPartVertexPtr(a1, a2, a3, a4, a5)", a1, a2, a3, (nint)a4, a5);
        Shape_PsgBuildBoneMatrices_a1* shape_PsgBuildBoneMatrices_a1 = (Shape_PsgBuildBoneMatrices_a1*)a1;
        long mapper = (long)unk_180576A10;
        var model = (PsgModel*)PsxAddrToPtr(mapper, shape_PsgBuildBoneMatrices_a1->field_8);
        ushort* counts = (ushort*)((byte*)PsxAddrToPtr(mapper, model->PartsAddr) + 40 * a2);
        if ((uint)a5 > 7)
        {
            *a4 = 0;
            return 0;
        }

        ulong address = a3;
        for (int kind = 0; kind < a5; kind++)
        {
            ulong perPrimitive = (kind & 1) == 0 ? 32UL : 24UL;
            address += perPrimitive * counts[kind + 1];
        }

        *a4 = (uint)PsxAddrMapperRegister(mapper, address);
        return counts[a5 + 1];
    }

    /// <summary>
    /// Hi_GetMdlVertexPtr(id, part, buffer, out address, kind): formats "id:%d cannot use!" into Buffer and hangs when
    /// effect-model slot <paramref name="a1"/> is unusable; otherwise forwards to <see cref="PsgPartVertexPtr"/> with the
    /// slot's handle. a3/a4 are 64-bit pointers in the caller (the decompiler typed them int).
    /// </summary>
    public static long GetMdlVertexPtr(int a1, ushort a2, ulong a3, uint* a4, int a5)
    {
        Recompiler.Runtime.Tracer.Enter("GetMdlVertexPtr(a1, a2, a3, a4, a5)", a1, a2, a3, (nint)a4, a5);
        var slot = (EffModelSlot*)unk_180220230 + a1;
        ulong handle = slot->model;
        if (slot->inUse == 0 || handle == 0)
        {
            WriteAnsi(Buffer, FormattableString.Invariant($"Hi_GetMdlVertexPtr()\nid:{a1} cannot use!"));
            HiraishiFatalHang();
        }

        return PsgPartVertexPtr((long)handle, a2, a3, a4, a5);
    }
}
