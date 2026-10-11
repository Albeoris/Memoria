using System.Runtime.CompilerServices;

namespace Recompiler.Runtime;

/// <summary>
/// Pointer-based equivalents of the C memory functions, for rewritten code that still deals with raw
/// buffers at the ABI boundary. Inside the module prefer <see cref="Span{T}"/> operations.
/// </summary>
public static unsafe class Crt
{
    /// <summary><c>memcpy</c>/<c>memmove</c>: overlapping ranges are handled correctly.</summary>
    public static void* Memcpy(void* destination, void* source, nuint count)
    {
        Buffer.MemoryCopy(source, destination, count, count);
        return destination;
    }

    /// <summary><c>memset</c>.</summary>
    public static void* Memset(void* destination, byte value, nuint count)
    {
        Unsafe.InitBlockUnaligned(destination, value, checked((uint)count));
        return destination;
    }

    /// <summary><c>memcmp</c>: returns a negative, zero or positive value like the CRT.</summary>
    public static int Memcmp(void* left, void* right, nuint count)
    {
        var a = new ReadOnlySpan<byte>(left, checked((int)count));
        var b = new ReadOnlySpan<byte>(right, checked((int)count));
        return a.SequenceCompareTo(b);
    }

    /// <summary><c>strcmp</c> for NUL-terminated 8-bit strings.</summary>
    public static int Strcmp(byte* left, byte* right) =>
        NativeString.AsSpan(left).SequenceCompareTo(NativeString.AsSpan(right));

    /// <summary><c>wcscmp</c> for NUL-terminated wide strings.</summary>
    public static int Wcscmp(char* left, char* right) =>
        NativeString.AsSpan(left).SequenceCompareTo(NativeString.AsSpan(right));

    /// <summary>
    /// <c>atoi</c>/<c>atol</c> as the VC6..VS2013 CRT computes them: leading C-locale white space, one optional sign, decimal
    /// digits up to the first other byte; the 32-bit total wraps on overflow (no clamping, no <c>errno</c>). NULL reads as 0.
    /// </summary>
    public static int Atoi(byte* text) => unchecked((int)Accumulate(text));

    /// <summary><c>atol</c>: the same as <see cref="Atoi"/> (a Windows <c>long</c> is 32 bits).</summary>
    public static int Atol(byte* text) => Atoi(text);

    /// <summary><c>_atoi64</c>: like <see cref="Atoi"/> with a 64-bit total.</summary>
    public static long Atoi64(byte* text) => Accumulate(text);

    private static long Accumulate(byte* text)
    {
        if (text is null)
        {
            return 0;
        }

        while (*text is (byte)' ' or >= 0x09 and <= 0x0D)
        {
            text++;
        }

        bool negative = *text == (byte)'-';
        if (*text is (byte)'-' or (byte)'+')
        {
            text++;
        }

        long total = 0;
        while (*text is >= (byte)'0' and <= (byte)'9')
        {
            total = unchecked((total * 10) + (*text - '0'));
            text++;
        }

        return negative ? unchecked(-total) : total;
    }
}
