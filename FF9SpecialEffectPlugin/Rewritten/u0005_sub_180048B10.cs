using Recompiler.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Number of 32-byte task slots in the table at 0x180323278.</summary>
    private const int PsxTaskSlotCount = 11;

    /// <summary>RVA of sub_180049170, the handler installed for task kinds &gt;= 128 (kind - 128 kept in the slot).</summary>
    private const uint PsxTaskScriptHandlerRva = 0x49170;

    /// <summary>
    /// Claims a free slot of the 11-entry task table at 0x180323278 (asserts when none is free), installs the handler
    /// of kind <paramref name="a1"/> and runs it twice: phase 0 reports the buffers it needs (scratch bytes, entry
    /// count, entry size, entry flags), which are allocated from the slot's arenas and registered as PSX addresses;
    /// phase 1 starts the task with (a3, a4). A nonzero phase-1 result finishes the task at once: the slot is released
    /// and both buffers are refilled with 0xFFFFFFFF (arena free-fill).
    /// </summary>
    public static long StartPsxTask(int a1, short a2, int a3, int a4)
    {
        Tracer.Enter("StartPsxTask(a1, a2, a3, a4)", a1, a2, a3, a4);
        PsxTaskSlot* slot = (PsxTaskSlot*)unk_180323278;
        int i;
        for (i = 0; i < PsxTaskSlotCount; ++i)
        {
            if (slot->used == 0)
            {
                break;
            }

            ++slot;
        }

        if (i == PsxTaskSlotCount)
        {
            PsxCompatibilityAssert();
        }

        ++dword_180323270;
        slot->index = (ushort)i;
        slot->used = 1;
        slot->useOwnArena = 1;
        slot->state = 0;
        slot->arenaSlot = a2;

        nint handlerAddress;
        if (a1 < 128)
        {
            handlerAddress = a1 >= 48
                ? ((nint*)ModuleData.At(0x4ab80))[a1]
                : ((nint*)ModuleData.At(0x4aff0))[a1];
        }
        else
        {
            slot->param = (uint)(a1 - 128);
            handlerAddress = Code_PsxTaskScriptHandler; // original code address in shared mode, the rewritten handler in embedded mode (RVA PsxTaskScriptHandlerRva)
        }

        nint context = off_180066C68;
        slot->handler = (ulong)handlerAddress;
        Stack16_StartPsxTask_request request = default;
        request.union_0_8 = 0;
        request.field_8 = 0;
        request.field_12 = 0;
        *(ulong*)&slot->scratchAddr = 0;

        var handler = (delegate* unmanaged<long, long, long, long, long, long>)slot->handler;
        if ((uint)handler(0, 0, context, (long)slot, (long)((int*)&request)) == 0)
        {
            uint entryCount = (uint)request.union_4_4;
            if (entryCount != 0)
            {
                uint entrySize = (uint)request.field_8 & 0xFFFFFFFC;
                if ((request.field_8 & 3) != 0)
                {
                    entrySize += 4;
                }

                int* entries = PsxObjectAlloc((long)slot, unchecked((int)(entryCount * (entrySize + 4) + 16)));
                uint entriesAddress = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)entries);
                int count = request.union_4_4;
                int size = request.field_8;
                int flags = request.field_12;
                slot->entriesAddr = entriesAddress;
                PsxInitEntryBlock(entriesAddress, (long)slot, size, count, flags);
            }

            if (request.union_0_4 != 0)
            {
                int* scratch = PsxObjectAlloc((long)slot, AlignUp4(request.union_0_4));
                slot->scratchAddr = (uint)PsxAddrMapperRegister((long)unk_180576A10, (ulong)scratch);
            }
        }

        Span<int> args = stackalloc int[2];
        int* argsAddress = (int*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(args));
        args[0] = a3;
        args[1] = a4;
        byte* scratchData = ResolveGlobalPsxAddress(slot->scratchAddr);
        handler = (delegate* unmanaged<long, long, long, long, long, long>)slot->handler;
        long result = handler(1, (long)scratchData, off_180066C68, (long)slot, (long)argsAddress);
        if ((uint)result == 0)
        {
            return result;
        }

        slot->used = 0;
        --dword_180323270;
        if (slot->scratchAddr != 0)
        {
            result = FillPsxBlockFree(ResolveGlobalPsxAddress(slot->scratchAddr), result);
        }

        uint entriesAddr = slot->entriesAddr;
        if (entriesAddr == 0)
        {
            return result;
        }

        return FillPsxBlockFree(ResolveGlobalPsxAddress(entriesAddr), result);
    }

    /// <summary>
    /// Refills an arena block (size in the dword before <paramref name="data"/>) with 0xFFFFFFFF starting at that size
    /// dword; returns size / 4 when it is not positive as int, 0xFFFFFFFF after filling, the way the original's
    /// result register ends up.
    /// </summary>
    private static long FillPsxBlockFree(byte* data, long result)
    {
        Tracer.Enter("FillPsxBlockFree(data, result)", (nint)data, result);
        uint* cursor = (uint*)(data - 4);
        result = *((uint*)data - 1) / 4;
        if ((int)result > 0)
        {
            long remaining = (uint)result;
            result = 0xFFFFFFFFL;
            while (remaining != 0)
            {
                *cursor = 0xFFFFFFFF;
                ++cursor;
                --remaining;
            }
        }

        return result;
    }

    /// <summary>
    /// Starts task kind 7 with arg 0 when byte +36 of the PSX block addressed by dword +12 of the context at
    /// off_180066C68 is 1 (255 otherwise) and 16 * <paramref name="a1"/>; the slot's arena index is -1.
    /// </summary>
    public static long StartPsxTask7(int a1)
    {
        Tracer.Enter("StartPsxTask7(a1)", a1);
        uint address = ((uint*)off_180066C68)[3];
        byte* block = ResolveGlobalPsxAddress(address);
        int value = 255;
        if (block[36] == 1)
        {
            value = 0;
        }

        return StartPsxTask(7, -1, value, 16 * a1);
    }
}
