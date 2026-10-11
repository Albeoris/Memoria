using System.Runtime.CompilerServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>sizeof of the two per-vertex int arrays at 0x180212440 / 0x1802191A0 (GEO_MAX_VERTICES 7000 x 4).</summary>
    private const int GeoVertexArrayBytes = 28000;

    /// <summary>The bound the original asserts on UV indices: sizeof(screenxy) in bytes, compared to an index.</summary>
    private const int GeoScreenXyBytes = 0x6D60;

    private const string GeoCppFile = @"..\..\SpecialEffectCode\sonoda\Geo\geo.cpp";

    /// <summary>
    /// Resets the 200-byte model handle a1 (a2 stored at +40), registers its PsgModel with the PSX address mapper and,
    /// the first time only (Flags bit 0 clear), relocates the model's offsets (+12, +16 and the five per-part fields at
    /// +20..+36) into PSX addresses. With a3 (table of (du, dv) ushort pairs per texture index) it adds each pair to the
    /// UV bytes of every primitive corner, each UV index once per part. Returns what the original leaves in rax.
    /// </summary>
    public static long PsgModelHandleInit(long a1, long a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("PsgModelHandleInit(a1, a2, a3)", a1, a2, a3);
        long ctx = (long)unk_180576A10;
        var handle = (PsgModelHandle*)a1;
        var model = (PsgModel*)PsxAddrToPtr(ctx, handle->ModelAddr);
        handle->Arg40 = (ulong)a2;
        handle->field_48 = 0;
        handle->field_136 = 0;
        handle->field_128 = 0;
        handle->field_96 = 0;
        handle->field_124 = 4096;
        handle->field_56 = 0;
        handle->field_24 = 0;
        handle->field_112 = 0;
        handle->field_120 = 268439552;
        handle->field_160 = 0;
        handle->field_164 = 0;
        handle->field_32 = 0;
        handle->State = 0;
        PsxAddrMapperRegister(ctx, (ulong)model);

        long result = model->Flags;
        if ((result & 1) != 0)
        {
            return result;
        }

        byte* modelBytes = (byte*)model;
        uint offset12 = model->Addr12;
        model->Flags = (ushort)(result | 1);
        model->Addr12 = (uint)PsxPtrToAddr(ctx, (ulong)(modelBytes + offset12));
        result = PsxPtrToAddr(ctx, (ulong)(modelBytes + model->PartsAddr));
        model->PartsAddr = (uint)result;
        byte* parts = (byte*)ResolveTracked((uint)result, ref result);

        for (uint i = 0; i < model->PartCount; i++)
        {
            uint* fields = (uint*)(parts + 40 * i + 20);
            for (int k = 0; k < 5; k++)
            {
                fields[k] = (uint)PsxPtrToAddr(ctx, (ulong)(modelBytes + fields[k]));
            }

            result = model->PartCount;
        }

        if (a3 == 0)
        {
            return result;
        }

        int* visited = (int*)Unsafe.AsPointer(ref dword_1802191A0);
        for (uint i = 0; i < model->PartCount; i++)
        {
            Unsafe.InitBlockUnaligned(Unsafe.AsPointer(ref dword_180212440), 0, GeoVertexArrayBytes);
            Unsafe.InitBlockUnaligned(visited, 0, GeoVertexArrayBytes);
            var part = (PsgMeshPart*)((byte*)PsxAddrToPtr(ctx, model->PartsAddr) + 40 * i);
            byte* prims = (byte*)PsxAddrToPtr(ctx, part->PrimitivesAddr);
            byte* uv = (byte*)PsxAddrToPtr(ctx, part->UvTableAddr);
            prims = PsgShiftUvs(prims, part->Quad24Count, 24, 19, 8, 4, uv, a3, visited, 0x3C9);
            prims = PsgShiftUvs(prims, part->Tri20Count, 20, 6, 12, 3, uv, a3, visited, 0x3E0);
            prims = PsgShiftUvs(prims, part->Quad32Count, 32, 29, 8, 4, uv, a3, visited, 0x3F8);
            PsgShiftUvs(prims, part->Tri24Count, 24, 22, 6, 3, uv, a3, visited, 0x40F);
            result = model->PartCount;
        }

        return result;
    }

    /// <summary>
    /// One primitive kind of <see cref="PsgModelHandleInit"/>: for each of <paramref name="count"/> records of
    /// <paramref name="stride"/> bytes, the texture byte at <paramref name="textureOffset"/> selects a (du, dv) pair of
    /// <paramref name="offsets"/>; every corner UV index not yet visited gets du/dv added to its two UV bytes (byte
    /// arithmetic), then the bound is asserted (after the write, as in the original) and the index is marked visited.
    /// Returns the record pointer after the last record.
    /// </summary>
    private static byte* PsgShiftUvs(
        byte* prims, int count, int stride, int textureOffset, int indexOffset, int corners, byte* uv, long offsets,
        int* visited, uint assertLine)
    {
        Recompiler.Runtime.Tracer.Enter("PsgShiftUvs(prims, count, stride, textureOffset, indexOffset, corners, uv, offsets, visited, assertLine)", (nint)prims, count, stride, textureOffset, indexOffset, corners, (nint)uv, offsets, (nint)visited, assertLine);
        for (int n = 0; n < count; n++)
        {
            ushort* pair = (ushort*)(offsets + 4 * prims[textureOffset]);
            byte du = (byte)pair[0];
            byte dv = (byte)pair[1];
            ushort* index = (ushort*)(prims + indexOffset);
            for (int k = 0; k < corners; k++)
            {
                ushort v = index[k];
                if (visited[v] == 0)
                {
                    uv[2 * v] += du;
                    uv[2 * v + 1] += dv;
                    if (v >= GeoScreenXyBytes)
                    {
                        GeoWassert("polyptr->uv[k] < sizeof(screenxy)", GeoCppFile, assertLine);
                    }

                    visited[v] = 1;
                }
            }

            prims += stride;
        }

        return prims;
    }

    /// <summary>
    /// <c>_wassert(expression, file, line)</c> of the sonoda Geo sources: reported and the process aborted, the same
    /// way as the psx_compatibility.cpp assertion (DIV-004).
    /// </summary>
    private static void GeoWassert(string expression, string file, uint line)
    {
        Recompiler.Runtime.Tracer.Enter("GeoWassert(expression, file, line)", expression, file, line);
        System.Environment.FailFast("Assertion failed: " + expression + ", file " + file + ", line " + line);
    }

    /// <summary>
    /// Hi_RegisterSolidEffModel: claims the first effect-model slot not in use (formats "memory not enough" into Buffer
    /// and hangs when all 32 are taken or the slot has no handle), maps host model a1 to a PSX address in the handle,
    /// clears handle+16 and initializes the handle without UV offsets. Returns the slot id.
    /// </summary>
    public static long RegisterSolidEffModel(ulong a1)
    {
        Recompiler.Runtime.Tracer.Enter("RegisterSolidEffModel(a1)", a1);
        var slot = (EffModelSlot*)unk_180220230;
        int n = 0;
        while (slot->inUse != 0)
        {
            ++n;
            ++slot;
            if (n >= 32)
            {
                WriteAnsi(Buffer, "Hi_RegisterSolidEffModel()\nmemory not enough!");
                HiraishiFatalHang();
            }
        }

        if (slot->model == 0)
        {
            WriteAnsi(Buffer, "Hi_RegisterSolidEffModel()\nmemory not enough!");
            HiraishiFatalHang();
        }

        *(ushort*)&slot->inUse = 1;
        slot->field_36 = 0;
        slot->field_40 = 0;
        var handle = (PsgModelHandle*)slot->model;
        handle->ModelAddr = (uint)PsxAddrMapperRegister((long)unk_180576A10, a1);
        handle->field_16 = 0;
        PsgModelHandleInit((long)slot->model, 0, 0);
        return slot->id;
    }
}
