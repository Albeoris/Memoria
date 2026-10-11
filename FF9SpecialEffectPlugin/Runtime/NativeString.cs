using System.Runtime.InteropServices;
using System.Text;

namespace Recompiler.Runtime;

/// <summary>Helpers for NUL-terminated strings that cross the native ABI.</summary>
public static unsafe class NativeString
{
    /// <summary>Length in bytes of a NUL-terminated 8-bit string (<c>strlen</c>). Returns 0 for <c>null</c>.</summary>
    public static nuint Length(byte* text) =>
        text is null ? 0 : (nuint)MemoryMarshal.CreateReadOnlySpanFromNullTerminated(text).Length;

    /// <summary>Length in UTF-16 code units of a NUL-terminated wide string (<c>wcslen</c>). Returns 0 for <c>null</c>.</summary>
    public static nuint Length(char* text) =>
        text is null ? 0 : (nuint)MemoryMarshal.CreateReadOnlySpanFromNullTerminated(text).Length;

    /// <summary>The bytes of a NUL-terminated 8-bit string, without the terminator.</summary>
    public static ReadOnlySpan<byte> AsSpan(byte* text) =>
        text is null ? default : MemoryMarshal.CreateReadOnlySpanFromNullTerminated(text);

    /// <summary>The characters of a NUL-terminated wide string, without the terminator.</summary>
    public static ReadOnlySpan<char> AsSpan(char* text) =>
        text is null ? default : MemoryMarshal.CreateReadOnlySpanFromNullTerminated(text);

    /// <summary>Decodes a NUL-terminated 8-bit string. <paramref name="encoding"/> defaults to UTF-8.</summary>
    public static string? ToString(byte* text, Encoding? encoding = null) =>
        text is null ? null : (encoding ?? Encoding.UTF8).GetString(AsSpan(text));

    /// <summary>Decodes a NUL-terminated wide string.</summary>
    public static string? ToString(char* text) => text is null ? null : new string(AsSpan(text));

    /// <summary>
    /// Copies <paramref name="source"/> into <paramref name="destination"/> and NUL-terminates it,
    /// truncating when <paramref name="capacity"/> (in bytes, including the terminator) is too small.
    /// Returns the number of bytes written excluding the terminator (<c>strncpy_s</c>-like, always terminated).
    /// </summary>
    public static int Copy(ReadOnlySpan<byte> source, byte* destination, nuint capacity)
    {
        if (destination is null || capacity == 0)
        {
            return 0;
        }

        int count = (int)Math.Min((ulong)source.Length, capacity - 1);
        source[..count].CopyTo(new Span<byte>(destination, count));
        destination[count] = 0;
        return count;
    }

    /// <summary>Wide-string variant of <see cref="Copy(ReadOnlySpan{byte}, byte*, nuint)"/>; capacity is in characters.</summary>
    public static int Copy(ReadOnlySpan<char> source, char* destination, nuint capacity)
    {
        if (destination is null || capacity == 0)
        {
            return 0;
        }

        int count = (int)Math.Min((ulong)source.Length, capacity - 1);
        source[..count].CopyTo(new Span<char>(destination, count));
        destination[count] = '\0';
        return count;
    }

    /// <summary>Encodes a managed string as a UTF-8 NUL-terminated buffer owned by the caller (free with <see cref="NativeMemory.Free"/>).</summary>
    public static byte* AllocateUtf8(string? text)
    {
        if (text is null)
        {
            return null;
        }

        int byteCount = Encoding.UTF8.GetByteCount(text);
        byte* buffer = (byte*)NativeMemory.Alloc((nuint)byteCount + 1);
        Encoding.UTF8.GetBytes(text, new Span<byte>(buffer, byteCount));
        buffer[byteCount] = 0;
        return buffer;
    }
}
