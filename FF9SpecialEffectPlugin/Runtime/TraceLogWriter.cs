using System.Buffers;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Recompiler.Runtime;

/// <summary>
/// The <see cref="Tracer"/> log file: UTF-8 lines ending in LF, each written straight to the end of the file (no buffer, so
/// every line is on disk when the call returns, and a reader that truncates the file is followed). Opened with
/// <c>FileShare.ReadWrite | FileShare.Delete</c> so <c>recomp trace show</c> can read and clear it while the process runs.
/// An IO failure marks the writer dead: later writes return false and never throw.
/// </summary>
internal sealed class TraceLogWriter : IDisposable
{
    private readonly Lock _gate = new();
    private SafeFileHandle? _handle;

    private TraceLogWriter(string path, SafeFileHandle handle)
    {
        Path = path;
        _handle = handle;
    }

    /// <summary>Full path of the log file.</summary>
    public string Path { get; }

    /// <summary>True once a write failed or the writer was disposed.</summary>
    public bool IsDead => Volatile.Read(ref _handle) is null;

    /// <summary>Opens (creating the directory) or returns null when the file cannot be opened.</summary>
    public static TraceLogWriter? Open(string path, bool truncate)
    {
        try
        {
            string? directory = System.IO.Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            SafeFileHandle handle = File.OpenHandle(path, truncate ? FileMode.Create : FileMode.OpenOrCreate, FileAccess.Write,
                FileShare.ReadWrite | FileShare.Delete);
            return new TraceLogWriter(path, handle);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            return null;
        }
    }

    /// <summary>Appends <paramref name="text"/> and a LF; false when the writer is dead.</summary>
    public bool WriteLine(ReadOnlySpan<char> text)
    {
        byte[] bytes = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(text.Length) + 1);
        try
        {
            int count = Encoding.UTF8.GetBytes(text, bytes);
            bytes[count++] = (byte)'\n';
            lock (_gate)
            {
                if (_handle is null)
                {
                    return false;
                }

                try
                {
                    RandomAccess.Write(_handle, bytes.AsSpan(0, count), RandomAccess.GetLength(_handle));
                    return true;
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ObjectDisposedException)
                {
                    CloseLocked();
                    return false;
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            CloseLocked();
        }
    }

    private void CloseLocked()
    {
        SafeFileHandle? handle = _handle;
        Volatile.Write(ref _handle, null);
        handle?.Dispose();
    }
}
