using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>Hi_Register*Model: claim an effect-model slot (32 at 0x180220230) or the summon slot (0x180220830).</summary>
internal static unsafe partial class Native
{
    /// <summary>Pairs in the zeroed (du, dv) table handed to PsgModelHandleInit (indexed by a texture byte).</summary>
    private const int RegisterSummonModelUvPairs = 256;

    /// <summary>Hi_RegisterGouEffModel: claims a slot of kind 1 for host model a1, no UV offsets. Returns the slot id.</summary>
    public static long RegisterGouEffModel(ulong a1)
    {
        Recompiler.Runtime.Tracer.Enter("RegisterGouEffModel(a1)", a1);
        EffModelSlot* slot = RegisterSummonModelClaimEffSlot("Hi_RegisterGouEffModel()\nmemory not enough!", 1, a1);
        PsgModelHandleInit((long)slot->model, 0, 0);
        return slot->id;
    }

    /// <summary>
    /// Hi_RegisterTexEffModel: claims a slot of kind 2 for host model a1, stores (a2, a3) at slot +8/+10 and shifts
    /// the UVs of texture 0 by (a4, a5). Returns the slot id.
    /// </summary>
    public static long RegisterTexEffModel(ulong a1, short a2, short a3, short a4, short a5)
    {
        Recompiler.Runtime.Tracer.Enter("RegisterTexEffModel(a1, a2, a3, a4, a5)", a1, a2, a3, a4, a5);
        EffModelSlot* slot = RegisterSummonModelClaimEffSlot("Hi_RegisterTexEffModel()\nmemory not enough!", 2, a1);
        Span<ushort> offsets = stackalloc ushort[512];
        ushort* offsetsAddress = (ushort*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(offsets));
        new Span<ushort>(offsetsAddress, 2 * RegisterSummonModelUvPairs).Clear();
        offsets[0] = (ushort)a4;
        *(short*)((byte*)slot + 8) = a2;
        *(short*)((byte*)slot + 10) = a3;
        offsets[1] = (ushort)a5;
        PsgModelHandleInit((long)slot->model, (long)((byte*)slot + 8), (long)offsetsAddress);
        return slot->id;
    }

    /// <summary>
    /// Hi_RegisterTexListModel: claims a slot of kind 2 for host model a1, copies the 24 bytes at a2 to slot +8 and
    /// initializes the handle with that copy and the UV offset table a3. Returns the slot id.
    /// </summary>
    public static long RegisterTexListModel(ulong a1, long a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("RegisterTexListModel(a1, a2, a3)", a1, a2, a3);
        EffModelSlot* slot = RegisterSummonModelClaimEffSlot("Hi_RegisterTexListModel()\nmemory not enough!", 2, a1);
        long model = (long)slot->model;
        System.Buffer.MemoryCopy((void*)a2, (byte*)slot + 8, 24, 24);
        PsgModelHandleInit(model, (long)((byte*)slot + 8), a3);
        return slot->id;
    }

    /// <summary>Hi_RegisterTexPtrModel: claims a slot of kind 2 for host model a1, handle init with (a2, a3). Returns the slot id.</summary>
    public static long RegisterTexPtrModel(ulong a1, long a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("RegisterTexPtrModel(a1, a2, a3)", a1, a2, a3);
        EffModelSlot* slot = RegisterSummonModelClaimEffSlot("Hi_RegisterTexPtrModel()\nmemory not enough!", 2, a1);
        PsgModelHandleInit((long)slot->model, a2, a3);
        return slot->id;
    }

    /// <summary>
    /// Hi_RegisterSummonModel: claims the single summon slot (hangs when in use or without a handle) for the summon
    /// header a1: model address at +60, handle +16 = host pointer of the PSX address at +384, flag +81 = a2 != 0.
    /// For each of the s16 count at +4 entries k: slot +8+4k / +10+4k = header u16 at +24+2k / +36+2k, UV offset
    /// pair k = (0, header u16 at +48+2k), and with a2 the 8 bytes at a2 + 8k go to slot +32+8k. After the handle
    /// init, handle +112 = host pointer of the PSX address at +64. Returns the u16 at slot +82.
    /// </summary>
    public static long RegisterSummonModel(long a1, long a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("RegisterSummonModel(a1, a2, a3)", a1, a2, a3);
        var slot = (SummonSlot*)Unsafe.AsPointer(ref qword_180220830);
        int n = 0;
        while (slot->inUse != 0)
        {
            ++n;
            ++slot;
            if (n >= 1)
            {
                WriteAnsi(Buffer, "Hi_RegisterSummonModel()\nmemory not enough!");
                HiraishiFatalHang();
            }
        }

        if (slot->model == 0)
        {
            WriteAnsi(Buffer, "Hi_RegisterSummonModel()\nmemory not enough!");
            HiraishiFatalHang();
        }

        byte* s = (byte*)slot;
        slot->inUse = 1;
        long handle = (long)slot->model;
        slot->motionFrame = 0;
        *(uint*)(handle + 8) = *(uint*)(a1 + 60);
        *(long*)((long)slot->model + 16) = PsxAddrToPtr((long)unk_180576A10, *(uint*)(a1 + 384));
        s[81] = (byte)(a2 != 0 ? 1 : 0);

        ushort* offsets = stackalloc ushort[2 * RegisterSummonModelUvPairs];
        new Span<ushort>(offsets, 2 * RegisterSummonModelUvPairs).Clear();
        for (int k = 0; k < *(short*)(a1 + 4); k++)
        {
            ushort first = *(ushort*)(a1 + 24 + 2 * k);
            offsets[2 * k] = 0;
            *(ushort*)(s + 8 + 4 * k) = first;
            *(ushort*)(s + 10 + 4 * k) = *(ushort*)(a1 + 36 + 2 * k);
            offsets[2 * k + 1] = *(ushort*)(a1 + 48 + 2 * k);
            if (a2 != 0)
            {
                ushort* source = (ushort*)(a2 + 8L * k);
                ushort* destination = (ushort*)(s + 32 + 8 * k);
                destination[0] = source[0];
                destination[1] = source[1];
                destination[2] = source[2];
                destination[3] = source[3];
            }
        }

        PsgModelHandleInit((long)slot->model, (long)(s + 8), (long)offsets);
        *(long*)((long)slot->model + 112) = PsxAddrToPtr((long)unk_180576A10, *(uint*)(a1 + 64));
        return *(ushort*)(s + 82);
    }

    /// <summary>
    /// Shared head of the Hi_Register*EffModel family: first slot not in use (hangs with <paramref name="message"/>
    /// in Buffer when all 32 are taken or the slot has no handle); marks it in use, kind <paramref name="kind"/> at
    /// +36, clears +40, maps host model a1 into the handle and clears handle +16.
    /// </summary>
    private static EffModelSlot* RegisterSummonModelClaimEffSlot(string message, uint kind, ulong model)
    {
        Recompiler.Runtime.Tracer.Enter("RegisterSummonModelClaimEffSlot(message, kind, model)", message, kind, model);
        var slot = (EffModelSlot*)unk_180220230;
        int n = 0;
        while (slot->inUse != 0)
        {
            ++n;
            ++slot;
            if (n >= 32)
            {
                WriteAnsi(Buffer, message);
                HiraishiFatalHang();
            }
        }

        if (slot->model == 0)
        {
            WriteAnsi(Buffer, message);
            HiraishiFatalHang();
        }

        *(ushort*)&slot->inUse = 1;
        slot->field_36 = kind;
        slot->field_40 = 0;
        var handle = (PsgModelHandle*)slot->model;
        handle->ModelAddr = (uint)PsxAddrMapperRegister((long)unk_180576A10, model);
        handle->field_16 = 0;
        return slot;
    }
}
