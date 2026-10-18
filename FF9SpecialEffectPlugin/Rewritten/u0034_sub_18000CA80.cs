using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Hi_SetSummonMotion / Hi_SetSummonMotFrame (summon slots at 0x180220830) and Hi_SplitMdlVertex: unsharing the
/// vertices of PSG mesh parts so that every primitive corner owns its vertex.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>Hi_SetSummonMotion: slot a2's model gets motion a1 at +16, frame 0. Unused slot: message, fatal hang.</summary>
    public static long SetSummonMotion(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("SetSummonMotion(a1, a2)", a1, a2);
        var slot = (SummonSlot*)Unsafe.AsPointer(ref qword_180220830) + a2;
        long result = 0;
        if (slot->inUse == 0 || (result = (long)slot->model) == 0)
        {
            WriteAnsi(Buffer, $"Hi_SetSummonMotion()\nid:{a2} cannot use!");
            HiraishiFatalHang();
        }

        slot->motionFrame = 0;
        *(long*)(result + 16) = a1;
        return result;
    }

    /// <summary>
    /// Hi_SetSummonMotFrame: frame = a2 when a2 &lt;= the u16 frame count at motion+2 and returns the motion pointer;
    /// otherwise frame 0 and returns 0. Unused slot: message, fatal hang.
    /// </summary>
    public static long SetSummonMotFrame(int a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("SetSummonMotFrame(a1, a2)", a1, a2);
        var slot = (SummonSlot*)Unsafe.AsPointer(ref qword_180220830) + a1;
        if (slot->inUse == 0 || slot->model == 0)
        {
            WriteAnsi(Buffer, $"Hi_SetSummonMotFrame()\nid:{a1} cannot use!");
            HiraishiFatalHang();
        }

        long result = *(long*)((long)slot->model + 16);
        if (*(ushort*)(result + 2) >= a2)
        {
            slot->motionFrame = (ushort)a2;
            return result;
        }

        slot->motionFrame = 0;
        return 0;
    }

    /// <summary>Hi_SplitMdlVertex: SplitModelParts on the model of effect-model slot a1. Unused slot: message, fatal hang.</summary>
    public static long SplitMdlVertex(int a1, long a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("SplitMdlVertex(a1, a2, a3)", a1, a2, a3);
        var slot = (EffModelSlot*)unk_180220230 + a1;
        long model = 0;
        if (slot->inUse == 0 || (model = (long)slot->model) == 0)
        {
            WriteAnsi(Buffer, $"Hi_SplitMdlVertex()\nid:{a1} cannot use!");
            HiraishiFatalHang();
        }

        // The original tail-returns the void SplitModelParts; its rax is not meaningful.
        SplitModelParts(model, (int)a2, a3);
        return 0;
    }

    /// <summary>
    /// For every part of the model (a2 == -1) or only part a2: SplitMeshPartVertices with the running output cursor a3,
    /// then caps the part's vertex count (u16 array at part[0].Addr20, one entry per part) at 7000.
    /// </summary>
    public static void SplitModelParts(long a1, int a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("SplitModelParts(a1, a2, a3)", a1, a2, a3);
        Shape_PsgBuildBoneMatrices_a1* shape_PsgBuildBoneMatrices_a1 = (Shape_PsgBuildBoneMatrices_a1*)a1;
        var model = (PsgModel*)ResolveGlobalPsxAddress(shape_PsgBuildBoneMatrices_a1->field_8);
        int partCount = model->PartCount;
        var parts = (PsgMeshPart*)ResolveGlobalPsxAddress(model->PartsAddr);
        var counts = (ushort*)ResolveGlobalPsxAddress(parts->Addr20);
        for (int part = 0; part < partCount; part++, counts++)
        {
            if (a2 == -1 || a2 == part)
            {
                a3 = SplitMeshPartVertices(a1, (ushort)part, a3);
                if (*counts > 7000)
                {
                    *counts = 7000;
                }
            }
        }
    }

    /// <summary>
    /// Mesh part a2: registers a3 with the PSX address mapper and stores its PSX address in Addr24 (vertex table), then
    /// walks the 8 primitive lists in order and, for each corner index, copies the 3 s16 of the old vertex (stride 8)
    /// to a3 + 8 * n, replaces the index by n and increments n (u16). Stores n at Addr20[a2]; returns a3 + 2 * n.
    /// </summary>
    public static long SplitMeshPartVertices(long a1, ushort a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("SplitMeshPartVertices(a1, a2, a3)", a1, a2, a3);
        Shape_PsgBuildBoneMatrices_a1* shape_PsgBuildBoneMatrices_a1 = (Shape_PsgBuildBoneMatrices_a1*)a1;
        var model = (PsgModel*)ResolveGlobalPsxAddress(shape_PsgBuildBoneMatrices_a1->field_8);
        var parts = (byte*)ResolveGlobalPsxAddress(model->PartsAddr);
        var part = (PsgMeshPart*)(parts + 40 * a2);
        byte* vertices = ResolveGlobalPsxAddress(part->Addr24);
        uint registered = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)a3);
        uint primitivesAddr = part->PrimitivesAddr;
        part->Addr24 = registered;
        byte* primitive = ResolveGlobalPsxAddress(primitivesAddr);

        ushort next = 0;
        ushort* counts = (ushort*)part;
        for (int kind = 0; kind < 8; kind++)
        {
            int stride = PrimitiveStrides[kind];
            int corners = PrimitiveCorners[kind];
            for (uint i = 0; i < counts[kind + 1]; i++, primitive += stride)
            {
                var index = (ushort*)primitive;
                for (int corner = 0; corner < corners; corner++, index++)
                {
                    var target = (ushort*)(a3 + 8L * next);
                    target[0] = *(ushort*)(vertices + 8 * *index);
                    target[1] = *(ushort*)(vertices + 8 * *index + 2);
                    target[2] = *(ushort*)(vertices + 8 * *index + 4);
                    *index = next++;
                }
            }
        }

        var vertexCounts = (ushort*)ResolveGlobalPsxAddress(part->Addr20);
        vertexCounts[a2] = next;
        return a3 + 2L * next;
    }

    /// <summary>Record sizes of the 8 primitive lists of a mesh part (counts at PsgMeshPart +2..+16).</summary>
    private static ReadOnlySpan<byte> PrimitiveStrides => [24, 20, 32, 24, 24, 20, 16, 12];

    /// <summary>Corner (vertex index) count of each primitive kind; the u16 indices start each record.</summary>
    private static ReadOnlySpan<byte> PrimitiveCorners => [4, 3, 4, 3, 4, 3, 4, 3];
}
