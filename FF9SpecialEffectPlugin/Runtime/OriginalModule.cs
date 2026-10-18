using System.Runtime.InteropServices;

namespace Recompiler.Runtime;

/// <summary>
/// Access to the renamed original module that ships next to the replacement. Exports that are not
/// rewritten yet are forwarded to it; <c>Trace</c>-mode stubs call it through typed function pointers.
/// </summary>
public sealed class OriginalModule
{
    private static readonly Lock Gate = new();
    private static OriginalModule? _shared;

    private readonly nint _handle;
    private readonly Dictionary<string, nint> _exports = new(StringComparer.Ordinal);

    private OriginalModule(string path)
    {
        Path = path;
        _handle = NativeLibrary.Load(path);
    }

    public string Path { get; }

    /// <summary>Base address of the loaded original (its <c>HMODULE</c>): RVAs of the analysis are offsets from it.</summary>
    public nint BaseAddress => _handle;

    /// <summary>Loads (once) the original module <paramref name="fileName"/> from the replacement's directory.</summary>
    public static OriginalModule Shared(string fileName)
    {
        if (_shared is not null)
        {
            return _shared;
        }

        lock (Gate)
        {
            return _shared ??= new OriginalModule(System.IO.Path.Combine(ModuleLifetime.ModuleDirectory, fileName));
        }
    }

    /// <summary>Returns the address of an export, throwing a descriptive error when it is missing.</summary>
    public nint GetExport(string name)
    {
        lock (_exports)
        {
            if (_exports.TryGetValue(name, out nint cached))
            {
                return cached;
            }

            if (!NativeLibrary.TryGetExport(_handle, name, out nint address))
            {
                throw new EntryPointNotFoundException($"'{Path}' has no export named '{name}'.");
            }

            _exports[name] = address;
            return address;
        }
    }

    /// <summary>Returns the address of an export by ordinal (for nameless exports).</summary>
    public nint GetExport(ushort ordinal)
    {
        string key = "#" + ordinal;
        lock (_exports)
        {
            if (_exports.TryGetValue(key, out nint cached))
            {
                return cached;
            }

            nint address = GetProcAddress(_handle, ordinal);
            if (address == 0)
            {
                throw new EntryPointNotFoundException($"'{Path}' has no export with ordinal {ordinal}.");
            }

            _exports[key] = address;
            return address;
        }
    }

    [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", ExactSpelling = true)]
    private static extern nint GetProcAddress(nint module, nint ordinal);
}
