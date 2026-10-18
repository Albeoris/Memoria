namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Clears the in-use flag and model pointer of the 32 effect-model slots at 0x180220230 and renumbers them;
    /// returns the id field one past the last slot.
    /// </summary>
    public static ushort* ResetEffModelSlots()
    {
        Recompiler.Runtime.Tracer.Enter("ResetEffModelSlots()");
        var slots = (EffModelSlot*)(unk_180220252 - 34);
        for (int i = 0; i < 32; i++)
        {
            slots[i].id = (ushort)i;
            slots[i].inUse = 0;
            slots[i].model = 0;
        }

        return (ushort*)(unk_180220252 + 32 * sizeof(EffModelSlot));
    }

    /// <summary>
    /// Clears the first 88-byte record at 0x180220830 (nine qwords, the dword at +72 and the qword at +76), then stores
    /// a1, a1 + 200, ... at +0 of the first a2 records (no bound check); returns the record after the last one assigned.
    /// </summary>
    public static long* InitModelTable88(long a1, long a2)
    {
        Recompiler.Runtime.Tracer.Enter("InitModelTable88(a1, a2)", a1, a2);
        // The original clears 8 bytes here (mov qword ptr [18022087Ch], 0 at 0x180015A22; claim SFX-016), not the 1 byte the pseudocode shows.
        *(ulong*)unk_18022087C = 0;
        qword_180220830 = 0;
        qword_180220838 = 0;
        qword_180220850 = 0;
        qword_180220858 = 0;
        qword_180220840 = 0;
        qword_180220860 = 0;
        qword_180220868 = 0;
        qword_180220848 = 0;
        qword_180220870 = 0;
        dword_180220878 = 0;

        long* result = (long*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref qword_180220830);
        if ((int)a2 > 0)
        {
            for (uint n = (uint)a2; n != 0; n--)
            {
                *result = a1;
                a1 += 200;
                result += 11;
            }
        }

        return result;
    }

    /// <summary>
    /// Inverse of <see cref="PsxAddrToPtr"/>: maps host pointer a2 to a PSX address through the used window of mapper a1
    /// with the smallest offset a2 - start (below 0x300000, first wins on ties). Window 128 is RAM:
    /// (a2 - ramHostBase) | 0x80000000; window i: (i &lt;&lt; 24) | 0xC00000 | (a2 - baseAddress). 0 when no window matches.
    /// </summary>
    public static long PsxPtrToAddr(long a1, ulong a2)
    {
        Recompiler.Runtime.Tracer.Enter("PsxPtrToAddr(a1, a2)", a1, a2);
        var mapper = (PsxAddrMapper*)a1;
        int count = mapper->count;
        if (count <= 0)
        {
            return 0;
        }

        var entries = (PsxAddrMapEntry*)((byte*)&mapper->entries);
        int best = -1;
        ulong bestOffset = 0x300000;
        for (int i = 0; i < count; i++)
        {
            PsxAddrMapEntry* entry = entries + i;
            if (entry->used != 0)
            {
                ulong start = entry->start;
                if (a2 >= start && a2 < entry->end && bestOffset > a2 - start)
                {
                    bestOffset = a2 - start;
                    best = i;
                }
            }
        }

        if (best < 0)
        {
            return 0;
        }

        if (best == 128)
        {
            return (uint)((a2 - (uint)mapper->ramHostBase) | 0x80000000);
        }

        return (uint)((ulong)(long)((best << 24) | 0xC00000) | (a2 - (uint)entries[best].baseAddress));
    }
}
