namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>Line of every <c>assert(0)</c> in psx_compatibility.cpp reached by this unit (0x312).</summary>
    private const int PsxCompatibilityAssertLine = 0x312;

    /// <summary>
    /// <c>_wassert(L"0", L"..\\..\\SpecialEffectCode\\psx\\source\\psx_compatibility.cpp", 0x312)</c>: the original
    /// reports the failed assertion and aborts the process (see the divergence for the Retry/Ignore dialog).
    /// </summary>
    private static void PsxCompatibilityAssert()
    {
        Recompiler.Runtime.Tracer.Enter("PsxCompatibilityAssert()");
        System.Environment.FailFast(
            "Assertion failed: 0, file ..\\..\\SpecialEffectCode\\psx\\source\\psx_compatibility.cpp, line "
            + PsxCompatibilityAssertLine);
    }

    /// <summary>Rounds a byte count up to a multiple of 4 the way the original does (signed arithmetic).</summary>
    private static int AlignUp4(int size)
    {
        Recompiler.Runtime.Tracer.Enter("AlignUp4(size)", size);
        int aligned = size & unchecked((int)0xFFFFFFFC);
        if ((size & 3) != 0)
        {
            aligned += 4;
        }

        return aligned;
    }

    /// <summary>
    /// Finds the record with id <paramref name="a1"/> in the variable-length table at 0x1802208D0 and returns a pointer
    /// to its payload. Record header: byte 0 = id low 8 bits, byte 1 bit 7 = id bit 8, bits 0..6 = count of 3-byte
    /// payload entries. Asserts after 512 records without a match.
    /// </summary>
    public static byte* PsxFindTableRecord(int a1)
    {
        Recompiler.Runtime.Tracer.Enter("PsxFindTableRecord(a1)", a1);
        int index = 0;
        byte* record = unk_1802208D0;
        while (true)
        {
            sbyte flags = (sbyte)record[1];
            int id = record[0];
            if (flags < 0)
            {
                id += 256;
            }

            if (id == a1)
            {
                break;
            }

            ++index;
            record += 3 * (flags & 0x7F) + 2;
            if (index >= 512)
            {
                PsxCompatibilityAssert();
            }
        }

        return record + 2;
    }

    /// <summary>
    /// Builds a packet header at PSX address <paramref name="a1"/> (resolved inline like PsxAddrToPtr over the global
    /// PSX context): +0 ushort stride (align4(a3) + 4), +2 ushort count a4, +4 uint a5, +8 ulong a2; then clears the
    /// first dword of each of the a4 entries (stride bytes apart) starting at +16 and returns the pointer past them.
    /// </summary>
    public static uint* PsxInitEntryBlock(uint a1, long a2, int a3, int a4, int a5)
    {
        Recompiler.Runtime.Tracer.Enter("PsxInitEntryBlock(a1, a2, a3, a4, a5)", a1, a2, a3, a4, a5);
        int entrySize = AlignUp4(a3);
        byte* header = ResolveGlobalPsxAddress(a1);
        Header2* header2 = (Header2*)header;

        int stride = entrySize + 4;
        uint* result = (uint*)(header + 16);
        header2->field_4 = (uint)a5;
        header2->field_0 = (ushort)stride;
        header2->field_2 = (ushort)a4;
        header2->field_8 = a2;
        if (a4 > 0)
        {
            long remaining = (uint)a4;
            do
            {
                *result = 0;
                result = (uint*)((byte*)result + stride);
                --remaining;
            }
            while (remaining != 0);
        }

        return result;
    }

    /// <summary>
    /// Allocates align4(a2) bytes from the bump arena a1 (<see cref="PsxBumpArena"/>, data at +16): asserts when the
    /// request exceeds the capacity, rewinds the arena when the remainder is too small, asserts unless the block is
    /// all 0xFFFFFFFF (free-fill), zeroes it, stores its size in the first dword and returns the dword after it.
    /// </summary>
    public static int* PsxArenaAlloc(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxArenaAlloc(a1, a2)", a1, a2);
        PsxBumpArena* arena = (PsxBumpArena*)a1;
        int size = AlignUp4(a2);
        if (arena->Capacity < (uint)size)
        {
            PsxCompatibilityAssert();
        }

        if (arena->Remaining < (uint)size)
        {
            uint capacity = arena->Capacity;
            arena->Cursor = (byte*)a1 + 16;
            arena->Remaining = capacity;
        }

        int* block = (int*)arena->Cursor;
        arena->Remaining -= (uint)size;
        arena->Cursor = (byte*)block + size;
        return FinishArenaBlock(block, size);
    }

    /// <summary>
    /// Allocates a2 + 4 bytes for the object a1: +8 short = slot in the PSX address table at 0x18032322C (-1 = none),
    /// +10 ushort = "use the object's own arena" flag. The object's arena (resolved PSX address) is used while the flag
    /// is set and it has room; otherwise the global arena at 0x180620BE0 is used, rewinding it (and setting the flag)
    /// when it is exhausted. Asserts after two rewinds or when the request exceeds the global capacity.
    /// </summary>
    public static int* PsxObjectAlloc(long a1, int a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxObjectAlloc(a1, a2)", a1, a2);
        short slot = *(short*)(a1 + 8);
        int size = a2 + 4;
        byte* objectArena = null;
        if (slot != -1)
        {
            uint address = ((uint*)unk_18032322C)[slot];
            if (address != 0)
            {
                objectArena = ResolveGlobalPsxAddress(address);
            }
        }

        int attempts = 0;
        while (true)
        {
            if (*(ushort*)(a1 + 10) != 0 && objectArena != null)
            {
                int aligned = AlignUp4(size);
                if (((ObjectArena*)objectArena)->field_8 >= (uint)aligned)
                {
                    return PsxArenaAlloc((long)objectArena, size);
                }

                ((ObjectArena*)objectArena)->field_8 = ((ObjectArena*)objectArena)->field_12;
                ((ObjectArena*)objectArena)->field_0 = (long)(objectArena + 16);
                *(ushort*)(a1 + 10) = 0;
            }
            else
            {
                int aligned = AlignUp4(size);
                if (dword_180620BE8 >= aligned)
                {
                    break;
                }

                qword_180620BE0 = (long)unk_180620BF0;
                dword_180620BE8 = dword_180620BEC;
                *(ushort*)(a1 + 10) = 1;
            }

            if (++attempts >= 2)
            {
                PsxCompatibilityAssert();
            }
        }

        int remaining = dword_180620BE8;
        int blockSize = AlignUp4(size);
        if (dword_180620BEC < blockSize)
        {
            PsxCompatibilityAssert();
        }

        int* block = (int*)unk_180620BF0;
        if (dword_180620BE8 >= blockSize)
        {
            block = (int*)qword_180620BE0;
        }
        else
        {
            remaining = dword_180620BEC;
        }

        dword_180620BE8 = remaining - blockSize;
        qword_180620BE0 = (long)block + blockSize;
        return FinishArenaBlock(block, blockSize);
    }

    /// <summary>
    /// Shared tail of both arena allocators: asserts unless the block is entirely 0xFFFFFFFF, zeroes it, writes the
    /// block size into the first dword and returns the next dword.
    /// </summary>
    private static int* FinishArenaBlock(int* block, int size)
    {
        Recompiler.Runtime.Tracer.Enter("FinishArenaBlock(block, size)", (nint)block, size);
        int words = size / 4;
        if (words > 0)
        {
            int checkedWords = 0;
            int* cursor = block;
            while (*cursor == -1)
            {
                ++checkedWords;
                ++cursor;
                if (checkedWords >= words)
                {
                    break;
                }
            }

            if (checkedWords < words)
            {
                PsxCompatibilityAssert();
            }

            for (long i = 0; i < (uint)words; i++)
            {
                block[i] = 0;
            }
        }

        *block = size;
        return block + 1;
    }
}
