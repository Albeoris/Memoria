using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiler.Runtime;

/// <summary>
/// Information about the module that contains the generated code (the replacement DLL or EXE itself). No static
/// constructor (only default-valued fields; a test checks it): <see cref="RuntimeStarted"/> must be readable in a module
/// whose NativeAOT start-up never ran.
/// </summary>
public static unsafe partial class ModuleLifetime
{
    private const uint GetModuleHandleExFlagFromAddress = 0x00000004;
    private const uint GetModuleHandleExFlagUnchangedRefCount = 0x00000002;

    private static string? _modulePath;
    private static int _initializersRan;
    private static int _runtimeStarted;

    /// <summary>
    /// True once this module's .NET start-up ran (its module initializers; under NativeAOT: a DLL on the first call into it,
    /// an EXE before its <c>Main</c>). False in an executable that another process loaded as a library (the <c>recomp
    /// verify</c> host calling its <c>recomp_test_*</c> exports): its start-up, run from its own entry point, never ran, the
    /// loader bound none of its imports (every P/Invoke jumps to an unresolved import slot) and the first class constructor
    /// faults. A plain static field read: no import, allocation, lock or class constructor.
    /// </summary>
    public static bool RuntimeStarted => Volatile.Read(ref _runtimeStarted) != 0;

    /// <summary>
    /// True once the original's CRT initializers (generated <c>Native.RunModuleInitializers</c>, embedded data image) have run;
    /// they run once, before the first access to the data image. Always false in shared mode: the original's own CRT ran them.
    /// </summary>
    public static bool InitializersRan => Volatile.Read(ref _initializersRan) != 0;

    /// <summary>
    /// Runs the module's initializers unless they already ran to completion; one that throws leaves them to run again. The
    /// caller serializes calls: <see cref="ModuleData"/> calls it under its gate while it materializes the image.
    /// </summary>
    public static void RunInitializers(Action? initializers)
    {
        if (initializers is null || InitializersRan)
        {
            return;
        }

        initializers();
        Volatile.Write(ref _initializersRan, 1);
    }

    /// <summary>Full path of the replacement module on disk.</summary>
    public static string ModulePath => _modulePath ??= ResolveModulePath();

    /// <summary>Directory of the replacement module; the renamed original lives next to it.</summary>
    public static string ModuleDirectory => Path.GetDirectoryName(ModulePath) ?? AppContext.BaseDirectory;

    /// <summary>Test hook: pretends the start-up did (or did not) run.</summary>
    internal static void SetRuntimeStartedForTests(bool started) => Volatile.Write(ref _runtimeStarted, started ? 1 : 0);

    /// <summary>Part of the module's start-up: marks <see cref="RuntimeStarted"/>.</summary>
    [ModuleInitializer]
    [SuppressMessage("Usage", "CA2255:The 'ModuleInitializer' attribute should not be used in libraries",
        Justification = "The flag must be set by the start-up itself; a library loaded without it must see it unset.")]
    internal static void OnRuntimeStarted() => Volatile.Write(ref _runtimeStarted, 1);

    private static string ResolveModulePath()
    {
        if (!OperatingSystem.IsWindows())
        {
            return AppContext.BaseDirectory;
        }

        delegate* unmanaged<void> anchor = &Anchor;
        nint module;
        if (!GetModuleHandleExW(GetModuleHandleExFlagFromAddress | GetModuleHandleExFlagUnchangedRefCount, (nint)anchor, &module))
        {
            return AppContext.BaseDirectory;
        }

        char* buffer = stackalloc char[1024];
        uint length = GetModuleFileNameW(module, buffer, 1024);
        return length == 0 ? AppContext.BaseDirectory : new string(buffer, 0, (int)length);
    }

    [UnmanagedCallersOnly]
    private static void Anchor()
    {
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetModuleHandleExW(uint flags, nint address, nint* module);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial uint GetModuleFileNameW(nint module, char* fileName, uint size);
}
