using Recompiler.Runtime;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    private const string PsxMapperBanner = "*********************************************************\n";

    /// <summary>
    /// ClearOTagR over a 4096-entry ordering table: entry i+1 links to the PSX address of entry i (through the
    /// global PsxVirtualAddrMapper64 at unk_180576A10), entry 0 is the 0xFFFFFF terminator.
    /// </summary>
    public static uint* PsxClearOTagR(uint* a1)
    {
        Tracer.Enter("PsxClearOTagR(a1)", (nint)a1);
        var mapper = (PsxAddrMapper*)unk_180576A10;
        for (int i = 4094; i >= 0; i--)
        {
            uint* slot = &a1[i];
            uint mapped = PsxAddrMapperMap(mapper, (ulong)slot, unsignedCountGrow: false);
            a1[i + 1] = mapped & 0xFFFFFF;
        }

        *a1 = 0xFFFFFF;
        return a1;
    }

    /// <summary>
    /// PsxVirtualAddrMapper64: returns the PSX-style address of host address a2 inside the window that holds it
    /// (closest start), registering a new 0x300000-byte window at a2 when none does; 0 for a null address or a full table.
    /// </summary>
    public static long PsxAddrMapperRegister(long a1, ulong a2)
    {
        Tracer.Enter("PsxAddrMapperRegister(a1, a2)", a1, a2);
        return PsxAddrMapperMap((PsxAddrMapper*)a1, a2, unsignedCountGrow: true);
    }

    /// <summary>
    /// Shared body of the mapper. The out-of-line function compares the new count unsigned against the stored one,
    /// the copy inlined in PsxClearOTagR compares signed; both are kept.
    /// </summary>
    private static uint PsxAddrMapperMap(PsxAddrMapper* mapper, ulong address, bool unsignedCountGrow)
    {
        Tracer.Enter("PsxAddrMapperMap(mapper, address, unsignedCountGrow)", (nint)mapper, address, unsignedCountGrow);
        if (address == 0)
        {
            Ports.Console.Write(PsxMapperBanner);
            Ports.Console.Write("Registering NULL address in PsxVirtualAddrMapper64.\n");
            return 0;
        }

        var entries = (PsxAddrMapEntry*)((byte*)&mapper->entries);
        int count = mapper->count;
        int best = -1;
        ulong bestDistance = 3145728;
        for (int i = 0; i < count; i++)
        {
            PsxAddrMapEntry* entry = &entries[i];
            if (entry->used != 0)
            {
                ulong start = entry->start;
                if (address >= start && address < entry->end && bestDistance > address - start)
                {
                    bestDistance = address - start;
                    best = i;
                }
            }
        }

        if (best >= 0)
        {
            return Encode(in *mapper, entries, best, address);
        }

        if (count >= 254)
        {
            Ports.Console.Write(PsxMapperBanner);
            Ports.Console.Write("Too match addresses to register in PsxVirtualAddrMapper64.\n");
            return 0;
        }

        int slot = 0;
        if (count >= 0)
        {
            do
            {
                if (slot != 128 && entries[slot].used == 0)
                {
                    break;
                }

                ++slot;
            }
            while (slot <= count);

            if (slot == 128)
            {
                slot = 129;
            }
        }

        PsxAddrMapEntry* created = &entries[slot];
        created->baseAddress = address - 0x20000;
        created->index = (byte)slot;
        created->used = 1;
        created->start = address;
        created->end = address + 3145728;

        int grown = slot + 1;
        bool grow = unsignedCountGrow ? (uint)grown > (uint)mapper->count : grown > mapper->count;
        if (grow)
        {
            mapper->count = grown;
        }

        return Encode(in *mapper, entries, created->index, address);
    }

    private static uint Encode(in PsxAddrMapper mapper, PsxAddrMapEntry* entries, int index, ulong address)
    {
        Tracer.Enter("Encode(mapper, entries, index, address)", mapper, (nint)entries, index, address);
        if (index == 128)
        {
            return ((uint)address - (uint)mapper.ramHostBase) | 0x80000000;
        }

        return ((uint)index << 24) | 0xC00000 | ((uint)address - (uint)entries[index].baseAddress);
    }
}
