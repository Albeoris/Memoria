using System.Runtime.InteropServices;
using System.Text;

namespace Recompiler.Runtime;

/// <summary>
/// Builds the <c>argc/argv</c> (or <c>wargv</c>) block a C <c>main</c> expects from managed
/// <c>string[] args</c>: argv[0] is the program path, the array is NUL-terminated, and every string
/// is NUL-terminated native memory owned by this object.
/// </summary>
public sealed unsafe class NativeArgv : IDisposable
{
    private readonly List<nint> _allocations = [];

    private NativeArgv(int count, nint pointer)
    {
        Count = count;
        Pointer = pointer;
    }

    /// <summary><c>argc</c>.</summary>
    public int Count { get; }

    /// <summary><c>argv</c>: a <c>byte**</c> (narrow) or <c>char**</c> (wide) with a trailing null entry.</summary>
    public nint Pointer { get; }

    /// <summary>A null <c>envp</c>; CRT-provided environments are not reproduced.</summary>
    public nint Environment => 0;

    /// <summary>Narrow (<c>char**</c>, UTF-8) argv for <c>main</c>.</summary>
    public static NativeArgv Narrow(string[] args) => Create(args, wide: false);

    /// <summary>Wide (<c>wchar_t**</c>) argv for <c>wmain</c>.</summary>
    public static NativeArgv Wide(string[] args) => Create(args, wide: true);

    /// <summary>The command line without the program name, as <c>WinMain</c> receives it (narrow, UTF-8).</summary>
    public static NativeArgv CommandLineTail(string[] args)
    {
        string joined = string.Join(' ', args.Select(a => a.Contains(' ', StringComparison.Ordinal) ? "\"" + a + "\"" : a));
        return Create([joined], wide: false, includeProgram: false);
    }

    private static NativeArgv Create(string[] args, bool wide, bool includeProgram = true)
    {
        var values = includeProgram ? args.Prepend(System.Environment.ProcessPath ?? "program").ToArray() : args;
        nint array = (nint)NativeMemory.AllocZeroed((nuint)(values.Length + 1), (nuint)sizeof(nint));
        var result = new NativeArgv(values.Length, array);
        result._allocations.Add(array);
        for (int i = 0; i < values.Length; i++)
        {
            byte[] bytes = wide ? Encoding.Unicode.GetBytes(values[i] + "\0") : Encoding.UTF8.GetBytes(values[i] + "\0");
            nint text = (nint)NativeMemory.Alloc((nuint)bytes.Length);
            bytes.AsSpan().CopyTo(new Span<byte>((void*)text, bytes.Length));
            ((nint*)array)[i] = text;
            result._allocations.Add(text);
        }

        return result;
    }

    public void Dispose()
    {
        foreach (nint allocation in _allocations)
        {
            NativeMemory.Free((void*)allocation);
        }

        _allocations.Clear();
    }
}
