using System.Buffers.Binary;

namespace Recompiler.Runtime;

/// <summary>Where the relocation table of an embedded data image lies in its blob (<c>ModuleData.bin</c>).</summary>
public readonly struct ModuleDataRelocationTable
{
    public ModuleDataRelocationTable(int blobOffset, int count)
    {
        BlobOffset = blobOffset;
        Count = count;
    }

    /// <summary>Offset of the table header in the blob.</summary>
    public int BlobOffset { get; }

    /// <summary>Entries in the table; 0 means there is no table.</summary>
    public int Count { get; }
}

/// <summary>What <see cref="ModuleDataRelocator.Apply"/> did to an embedded data image.</summary>
public sealed class ModuleDataFixups
{
    public ModuleDataFixups(int dataPointers, int codePointers, IReadOnlyList<uint> unresolvedCodePointers)
    {
        DataPointers = dataPointers;
        CodePointers = codePointers;
        UnresolvedCodePointers = unresolvedCodePointers;
    }

    public static ModuleDataFixups None { get; } = new(0, 0, []);

    /// <summary>Slots pointed at the embedded block.</summary>
    public int DataPointers { get; }

    /// <summary>Slots pointed at a managed method the code resolver handed out.</summary>
    public int CodePointers { get; }

    /// <summary>Target RVAs of code slots the resolver had no address for (distinct, ascending); those slots keep the original's value.</summary>
    public IReadOnlyList<uint> UnresolvedCodePointers { get; }
}

/// <summary>
/// Fixes up the pointers stored in an embedded data image. The original's data hold preferred-base addresses; the
/// relocation table <c>recomp scaffold</c> appends to <c>ModuleData.bin</c> lists every slot that points into the image
/// (<c>data</c>: rewritten to the block) or into code (<c>code</c>: rewritten to what the code resolver returns for the
/// target RVA, the managed method generated in <c>CodeAddresses.g.cs</c>; 0 leaves the slot and records the target).
/// Table format: header <c>RLOC</c>, version 1, entry count, entry size 12; entries <c>u32 slotRva, u32 targetRva,
/// u8 kind (1 data, 2 code), u8 width, u16 reserved</c>, little-endian.
/// </summary>
public static unsafe class ModuleDataRelocator
{
    public const uint Magic = 0x434F4C52;
    public const uint Version = 1;
    public const int HeaderSize = 16;
    public const int EntrySize = 12;
    private const byte DataKind = 1;
    private const byte CodeKind = 2;

    /// <summary>
    /// Applies <paramref name="table"/> (header and entries) to the block that holds <c>[rangeStart, rangeEnd)</c>.
    /// <paramref name="codeResolver"/> maps a code RVA to a callable address (0 when there is none); without one, every
    /// code slot stays as stored.
    /// </summary>
    public static ModuleDataFixups Apply(byte* block, uint rangeStart, uint rangeEnd, ReadOnlySpan<byte> table, Func<uint, nint>? codeResolver)
    {
        if (table.Length < HeaderSize
            || BinaryPrimitives.ReadUInt32LittleEndian(table) != Magic
            || BinaryPrimitives.ReadUInt32LittleEndian(table[4..]) != Version
            || BinaryPrimitives.ReadUInt32LittleEndian(table[12..]) != EntrySize)
        {
            throw Corrupt("has no valid header");
        }

        uint count = BinaryPrimitives.ReadUInt32LittleEndian(table[8..]);
        if ((ulong)table.Length < HeaderSize + ((ulong)count * EntrySize))
        {
            throw Corrupt("is truncated");
        }

        nint delta = (nint)(block - rangeStart);
        int data = 0;
        int code = 0;
        var unresolved = new SortedSet<uint>();
        var resolved = new Dictionary<uint, nint>();
        for (int i = 0; i < (int)count; i++)
        {
            ReadOnlySpan<byte> entry = table.Slice(HeaderSize + (i * EntrySize), EntrySize);
            uint slot = BinaryPrimitives.ReadUInt32LittleEndian(entry);
            uint target = BinaryPrimitives.ReadUInt32LittleEndian(entry[4..]);
            byte kind = entry[8];
            int width = entry[9];
            if (width != sizeof(nint))
            {
                throw new InvalidOperationException($"The data image relocation at RVA 0x{slot:x} is {width} bytes wide but this process uses {sizeof(nint)}-byte pointers; build the replacement for the original's architecture.");
            }

            if (slot < rangeStart || (ulong)slot + (ulong)width > rangeEnd)
            {
                throw Corrupt($"names slot 0x{slot:x} outside the image");
            }

            nint* at = (nint*)(block + (slot - rangeStart));
            switch (kind)
            {
                case DataKind:
                    if (target < rangeStart || target > rangeEnd)
                    {
                        throw Corrupt($"points slot 0x{slot:x} at 0x{target:x} outside the image");
                    }

                    *at = delta + (nint)target;
                    data++;
                    break;
                case CodeKind:
                    if (!resolved.TryGetValue(target, out nint address))
                    {
                        address = codeResolver?.Invoke(target) ?? 0;
                        resolved[target] = address;
                    }

                    if (address != 0)
                    {
                        *at = address;
                        code++;
                    }
                    else
                    {
                        unresolved.Add(target);
                    }

                    break;
                default:
                    throw Corrupt($"has unknown kind {kind} at slot 0x{slot:x}");
            }
        }

        return new ModuleDataFixups(data, code, [.. unresolved]);
    }

    private static InvalidOperationException Corrupt(string what) =>
        new($"The relocation table of the embedded data image {what}; re-run 'recomp scaffold' and rebuild.");
}
