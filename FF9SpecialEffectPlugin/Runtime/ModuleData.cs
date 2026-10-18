using System.Reflection;
using System.Runtime.InteropServices;

namespace Recompiler.Runtime;

/// <summary>One section of an embedded data image: where it lives in the original (RVA) and where its bytes are in the blob.</summary>
public readonly struct ModuleDataSection
{
    public ModuleDataSection(uint rva, uint rawSize, uint virtualSize, int blobOffset)
    {
        Rva = rva;
        RawSize = rawSize;
        VirtualSize = virtualSize;
        BlobOffset = blobOffset;
    }

    public uint Rva { get; }

    /// <summary>Bytes present in the blob; the rest of <see cref="VirtualSize"/> is zero-filled (BSS).</summary>
    public uint RawSize { get; }

    public uint VirtualSize { get; }

    public int BlobOffset { get; }
}

/// <summary>
/// The original module's data image: the arrays, records, arenas and tables the native code keeps in its data
/// sections, addressed by RVA exactly as the decompiler shows them. Rewritten code reaches them through the typed
/// accessors generated in <c>ModuleData.g.cs</c> (<c>ref int dword_1802208D0</c>, <c>byte* unk_1802208D0</c>) or
/// directly with <see cref="At"/>. The generated file picks the source:
/// <list type="bullet">
/// <item><c>shared</c>: <see cref="Base"/> is the base address of the loaded original module, so managed exports
/// and the exports still forwarded to the original read and write one state (strangler phase).</item>
/// <item><c>embedded</c>: a <c>NativeMemory</c> block covering the original's data range, filled at first use from
/// the section bytes embedded as the <c>ModuleData.bin</c> resource; <see cref="Base"/> is the fictitious module
/// base that puts the block at its RVAs. Pointers stored inside the data are fixed up from the relocation table
/// appended to the blob (<see cref="ModuleDataRelocator"/>): into the block, or to the managed method the code resolver
/// (<c>Native.CodeAt</c>) hands out; code targets without one keep the original's value and are listed in
/// <see cref="UnresolvedCodePointers"/>.</item>
/// </list>
/// </summary>
public static unsafe class ModuleData
{
    private static readonly Lock Gate = new();
    private static Func<nint>? _resolver;
    private static Func<uint, nint>? _codeResolver;
    private static Action? _initializers;
    private static nint _base;

    // The base while the initializers run: they read the image on the materializing thread (the gate is re-entrant),
    // every other thread waits on the gate until they are done.
    private static nint _initializing;
    private static string _mode = "unconfigured";
    private static ModuleDataFixups _fixups = ModuleDataFixups.None;

    /// <summary><c>shared</c>, <c>embedded</c> or <c>unconfigured</c>; set when the image is first resolved.</summary>
    public static string Mode => _mode;

    /// <summary>Address that, plus an RVA, gives the data of the original at that RVA. Resolved on first use.</summary>
    public static nint Base
    {
        get
        {
            nint resolved = Volatile.Read(ref _base);
            return resolved != 0 ? resolved : Initialize();
        }
    }

    /// <summary>Pointer to the data at <paramref name="rva"/> (an address of the analysis minus the image base).</summary>
    public static byte* At(uint rva) => (byte*)Base + rva;

    /// <summary>
    /// Embedded mode: RVAs of the code the original's data points to that had no managed address when the image was
    /// materialized (unit not rewritten, signature not callable unmanaged); those slots keep the original's value.
    /// Empty before <see cref="Base"/> is first read and in shared mode.
    /// </summary>
    public static IReadOnlyList<uint> UnresolvedCodePointers => _fixups.UnresolvedCodePointers;

    /// <summary>Embedded mode: data slots pointed at the block when the image was materialized.</summary>
    public static int FixedDataPointers => _fixups.DataPointers;

    /// <summary>Embedded mode: code slots pointed at managed methods when the image was materialized.</summary>
    public static int ResolvedCodePointers => _fixups.CodePointers;

    /// <summary>Installs the resolver the generated <c>ModuleData.g.cs</c> chose; nothing is loaded or allocated until <see cref="Base"/> is read.</summary>
    public static void Configure(Func<nint> resolver) => Configure(resolver, null);

    /// <summary>
    /// Installs the resolver and the code resolver (<c>Native.CodeAt</c>: code RVA to the managed method's address, 0
    /// when there is none) that <see cref="Embedded(string, uint, uint, ReadOnlySpan{ModuleDataSection}, ModuleDataRelocationTable)"/>
    /// uses for the code pointers stored in the data.
    /// </summary>
    public static void Configure(Func<nint> resolver, Func<uint, nint>? codeResolver) => Configure(resolver, codeResolver, null);

    /// <summary>
    /// Installs the resolvers and the module's initializers (<c>Native.RunModuleInitializers</c>: the original's CRT
    /// initializers, rewritten), which run once through <see cref="ModuleLifetime.RunInitializers"/> right after the image is
    /// first resolved and before any other thread sees it.
    /// </summary>
    public static void Configure(Func<nint> resolver, Func<uint, nint>? codeResolver, Action? initializers)
    {
        lock (Gate)
        {
            _resolver = resolver;
            _codeResolver = codeResolver;
            _initializers = initializers;
            _base = 0;
            _fixups = ModuleDataFixups.None;
        }
    }

    /// <summary>Shared source: the original module loaded next to the replacement; its own data sections are the image.</summary>
    public static nint Shared(string originalModuleFileName)
    {
        nint moduleBase = OriginalModule.Shared(originalModuleFileName).BaseAddress;
        _mode = "shared";
        return moduleBase;
    }

    /// <summary>
    /// Embedded source: allocates <c>[rangeStart, rangeEnd)</c> zero-filled and copies every section's bytes from the
    /// embedded resource <paramref name="resourceName"/>; returns the base that maps RVAs into the block.
    /// </summary>
    public static nint Embedded(string resourceName, uint rangeStart, uint rangeEnd, ReadOnlySpan<ModuleDataSection> sections) =>
        Embedded(resourceName, rangeStart, rangeEnd, sections, default);

    /// <summary>
    /// Embedded source with pointer fix-up: after the sections are copied, the relocation table at
    /// <paramref name="relocations"/> in the blob points every <c>data</c> slot at the block and every <c>code</c> slot at
    /// the address the code resolver installed with <see cref="Configure(Func{nint}, Func{uint, nint})"/> returns.
    /// </summary>
    public static nint Embedded(string resourceName, uint rangeStart, uint rangeEnd, ReadOnlySpan<ModuleDataSection> sections, ModuleDataRelocationTable relocations)
    {
        if (rangeEnd < rangeStart)
        {
            throw new ArgumentOutOfRangeException(nameof(rangeEnd), "The data range ends before it starts.");
        }

        byte[] blob = ReadResource(resourceName);
        nuint size = Math.Max(1u, rangeEnd - rangeStart);
        byte* block = (byte*)NativeMemory.AllocZeroed(size);
        foreach (ModuleDataSection section in sections)
        {
            if (section.Rva < rangeStart || section.Rva + section.RawSize > rangeEnd || section.BlobOffset + section.RawSize > (uint)blob.Length)
            {
                NativeMemory.Free(block);
                throw new InvalidOperationException($"Embedded data section at RVA 0x{section.Rva:x} does not fit the image range or the blob; re-run 'recomp scaffold'.");
            }

            blob.AsSpan(section.BlobOffset, (int)section.RawSize).CopyTo(new Span<byte>(block + (section.Rva - rangeStart), (int)section.RawSize));
        }

        if (relocations.Count > 0)
        {
            if (relocations.BlobOffset < 0 || relocations.BlobOffset > blob.Length)
            {
                NativeMemory.Free(block);
                throw new InvalidOperationException("The relocation table of the embedded data image lies outside the blob; re-run 'recomp scaffold'.");
            }

            try
            {
                _fixups = ModuleDataRelocator.Apply(block, rangeStart, rangeEnd, blob.AsSpan(relocations.BlobOffset), _codeResolver);
            }
            catch
            {
                NativeMemory.Free(block);
                throw;
            }
        }

        _mode = "embedded";
        return (nint)(block - rangeStart);
    }

    private static nint Initialize()
    {
        lock (Gate)
        {
            if (_base != 0)
            {
                return _base;
            }

            if (_initializing != 0)
            {
                return _initializing;
            }

            Func<nint> resolver = _resolver ?? throw new InvalidOperationException("ModuleData is not configured: the generated ModuleData.g.cs is missing or its module initializer did not run; re-run 'recomp scaffold'.");
            nint resolved = resolver();
            if (resolved == 0)
            {
                throw new InvalidOperationException("The module data image resolved to a null base.");
            }

            _initializing = resolved;
            try
            {
                ModuleLifetime.RunInitializers(_initializers);
            }
            finally
            {
                _initializing = 0;
            }

            Volatile.Write(ref _base, resolved);
            return resolved;
        }
    }

    private static byte[] ReadResource(string resourceName)
    {
        Assembly assembly = typeof(ModuleData).Assembly;
        using Stream stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' is missing from {assembly.GetName().Name}; re-run 'recomp scaffold' and rebuild (the workspace Directory.Build.targets embeds ModuleData.bin).");
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }
}
