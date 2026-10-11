using System.Globalization;
using System.Text;

namespace Recompiler.Runtime;

/// <summary>
/// The variadic arguments of a ported <c>printf</c>-family call, read the way the original's stack holds them: on x86 4-byte
/// slots (<c>uint* va</c>; a <c>double</c> or 64-bit integer takes two), on x64 8-byte slots (<c>ulong* va</c>).
/// </summary>
public unsafe struct VaList
{
    private byte* _cursor;
    private readonly int _slot;

    private VaList(byte* start, int slot)
    {
        _cursor = start;
        _slot = slot;
    }

    /// <summary>x86 slots (4 bytes).</summary>
    public static VaList X86(uint* va) => new((byte*)va, 4);

    /// <summary>x64 slots (8 bytes).</summary>
    public static VaList X64(ulong* va) => new((byte*)va, 8);

    public int NextInt32()
    {
        int value = *(int*)_cursor;
        _cursor += _slot;
        return value;
    }

    public long NextInt64()
    {
        long value = *(long*)_cursor;
        _cursor += 8;
        return value;
    }

    public double NextDouble()
    {
        double value = *(double*)_cursor;
        _cursor += 8;
        return value;
    }

    public nint NextPointer() => _slot == 4 ? (nint)(uint)NextInt32() : (nint)NextInt64();

    /// <summary>The pointer width of the original (<c>%p</c> digits, <c>%Iu</c>).</summary>
    public readonly int PointerSize => _slot;
}

/// <summary>
/// The C runtime's <c>printf</c> formatting in the C locale, for ported <c>sprintf</c>/<c>_snprintf</c>/<c>vsprintf</c> calls:
/// flags <c>- + space # 0</c>, width and precision (also <c>*</c>), length modifiers <c>h hh l ll L I I32 I64 z</c>,
/// conversions <c>d i u o x X c s p e E f F g G %</c> (<c>%n</c> stores the count). MSVC behaviour where it differs from
/// C99: <c>%s</c> of a null pointer prints <c>(null)</c>, <c>%p</c> prints the pointer as upper-case hex padded to the
/// pointer width. Exponents have <see cref="ExponentDigits"/> digits (2 like the UCRT; set 3 for programs built with the
/// pre-2015 CRT, VC6 to VS2013). Strings are 8-bit (<see cref="Encoding"/>, Latin-1 by default so every byte survives).
/// </summary>
public static unsafe class CFormat
{
    /// <summary>Minimum digits of a <c>%e</c>/<c>%g</c> exponent: 2 (UCRT) or 3 (msvcrt, VC6 to VS2013).</summary>
    public static int ExponentDigits { get; set; } = 2;

    public static Encoding Encoding { get; set; } = Encoding.Latin1;

    /// <summary><c>sprintf(buffer, format, ...)</c> with x86 arguments; returns the length written (without the NUL).</summary>
    public static int Sprintf(byte* buffer, byte* format, uint* va) => Store(buffer, int.MaxValue, Format(format, VaList.X86(va), out _), msvcTruncation: false);

    /// <summary><c>sprintf(buffer, format, ...)</c> with x64 arguments.</summary>
    public static int Sprintf(byte* buffer, byte* format, ulong* va) => Store(buffer, int.MaxValue, Format(format, VaList.X64(va), out _), msvcTruncation: false);

    /// <summary>C99 <c>snprintf</c>: writes at most <paramref name="count"/> bytes including the NUL, returns the full length.</summary>
    public static int Snprintf(byte* buffer, nuint count, byte* format, uint* va) => Store(buffer, Clamp(count), Format(format, VaList.X86(va), out _), msvcTruncation: false);

    public static int Snprintf(byte* buffer, nuint count, byte* format, ulong* va) => Store(buffer, Clamp(count), Format(format, VaList.X64(va), out _), msvcTruncation: false);

    /// <summary>
    /// MSVC <c>_snprintf</c>/<c>_vsnprintf</c>: writes at most <paramref name="count"/> bytes; when the text does not fit it
    /// returns -1 and leaves the buffer without a terminating NUL; when it fits exactly no NUL is written either.
    /// </summary>
    public static int SnprintfMsvc(byte* buffer, nuint count, byte* format, uint* va) => Store(buffer, Clamp(count), Format(format, VaList.X86(va), out _), msvcTruncation: true);

    public static int SnprintfMsvc(byte* buffer, nuint count, byte* format, ulong* va) => Store(buffer, Clamp(count), Format(format, VaList.X64(va), out _), msvcTruncation: true);

    /// <summary>The formatted bytes of a NUL-terminated format.</summary>
    public static byte[] Format(byte* format, VaList va, out int length)
    {
        byte[] result = Format(NativeString.AsSpan(format), ref va);
        length = result.Length;
        return result;
    }

    /// <summary>Formats with managed arguments (tests, ports that already hold the values): <c>int</c>, <c>uint</c>, <c>long</c>, <c>ulong</c>, <c>double</c>, <c>string</c>, <c>char</c>, <c>nint</c>.</summary>
    public static string Format(string format, params object?[] arguments)
    {
        // Pack the arguments into x64 slots and run the same formatter: one code path for both forms.
        ulong* slots = stackalloc ulong[Math.Max(1, arguments.Length)];
        var pinned = new List<System.Runtime.InteropServices.GCHandle>();
        try
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                slots[i] = arguments[i] switch
                {
                    null => 0,
                    int v => unchecked((ulong)v),
                    uint v => v,
                    long v => unchecked((ulong)v),
                    ulong v => v,
                    short v => unchecked((ulong)v),
                    ushort v => v,
                    byte v => v,
                    sbyte v => unchecked((ulong)v),
                    char v => v,
                    bool v => v ? 1UL : 0UL,
                    nint v => unchecked((ulong)v),
                    nuint v => v,
                    double v => BitConverter.DoubleToUInt64Bits(v),
                    float v => BitConverter.DoubleToUInt64Bits(v),
                    string v => Pin(Encoding.GetBytes(v + "\0"), pinned),
                    byte[] v => Pin(v, pinned),
                    _ => throw new ArgumentException($"Unsupported printf argument type {arguments[i]!.GetType().Name}.", nameof(arguments)),
                };
            }

            VaList va = VaList.X64(slots);
            return Encoding.GetString(Format(Encoding.GetBytes(format), ref va));
        }
        finally
        {
            foreach (System.Runtime.InteropServices.GCHandle handle in pinned)
            {
                handle.Free();
            }
        }
    }

    private static ulong Pin(byte[] bytes, List<System.Runtime.InteropServices.GCHandle> pinned)
    {
        System.Runtime.InteropServices.GCHandle handle = System.Runtime.InteropServices.GCHandle.Alloc(bytes, System.Runtime.InteropServices.GCHandleType.Pinned);
        pinned.Add(handle);
        return (ulong)handle.AddrOfPinnedObject();
    }

    private static int Clamp(nuint count) => count > int.MaxValue ? int.MaxValue : (int)count;

    private static int Store(byte* buffer, int capacity, byte[] text, bool msvcTruncation)
    {
        if (buffer is null || capacity == 0)
        {
            return msvcTruncation && text.Length > 0 ? -1 : text.Length;
        }

        if (msvcTruncation)
        {
            int copied = Math.Min(text.Length, capacity);
            text.AsSpan(0, copied).CopyTo(new Span<byte>(buffer, copied));
            if (text.Length < capacity)
            {
                buffer[text.Length] = 0;
            }

            return text.Length > capacity ? -1 : text.Length;
        }

        int length = Math.Min(text.Length, capacity - 1);
        text.AsSpan(0, length).CopyTo(new Span<byte>(buffer, length));
        buffer[length] = 0;
        return text.Length;
    }

    private static byte[] Format(ReadOnlySpan<byte> format, ref VaList va)
    {
        var output = new List<byte>(format.Length + 16);
        int i = 0;
        while (i < format.Length)
        {
            byte c = format[i++];
            if (c != (byte)'%')
            {
                output.Add(c);
                continue;
            }

            if (i >= format.Length)
            {
                break;
            }

            // Flags.
            bool left = false, plus = false, space = false, alternate = false, zero = false;
            for (; i < format.Length; i++)
            {
                switch ((char)format[i])
                {
                    case '-': left = true; continue;
                    case '+': plus = true; continue;
                    case ' ': space = true; continue;
                    case '#': alternate = true; continue;
                    case '0': zero = true; continue;
                }

                break;
            }

            int width = 0;
            if (i < format.Length && format[i] == (byte)'*')
            {
                width = va.NextInt32();
                if (width < 0)
                {
                    left = true;
                    width = -width;
                }

                i++;
            }
            else
            {
                while (i < format.Length && format[i] is >= (byte)'0' and <= (byte)'9')
                {
                    width = width * 10 + (format[i++] - '0');
                }
            }

            int precision = -1;
            if (i < format.Length && format[i] == (byte)'.')
            {
                i++;
                precision = 0;
                if (i < format.Length && format[i] == (byte)'*')
                {
                    precision = va.NextInt32();
                    i++;
                }
                else
                {
                    while (i < format.Length && format[i] is >= (byte)'0' and <= (byte)'9')
                    {
                        precision = precision * 10 + (format[i++] - '0');
                    }
                }
            }

            // Length modifiers.
            int size = 0; // 0 default, -1 short, -2 char, 1 long (32-bit on Windows), 2 64-bit, 3 pointer-sized
            while (i < format.Length)
            {
                char m = (char)format[i];
                if (m == 'h')
                {
                    size = size == -1 ? -2 : -1;
                    i++;
                }
                else if (m == 'l')
                {
                    size = size == 1 ? 2 : 1;
                    i++;
                }
                else if (m is 'L' or 'q' or 'j')
                {
                    size = 2;
                    i++;
                }
                else if (m is 'z' or 't')
                {
                    size = 3;
                    i++;
                }
                else if (m == 'I')
                {
                    i++;
                    if (i + 1 < format.Length && format[i] == (byte)'6' && format[i + 1] == (byte)'4')
                    {
                        size = 2;
                        i += 2;
                    }
                    else if (i + 1 < format.Length && format[i] == (byte)'3' && format[i + 1] == (byte)'2')
                    {
                        size = 1;
                        i += 2;
                    }
                    else
                    {
                        size = 3;
                    }
                }
                else
                {
                    break;
                }
            }

            if (i >= format.Length)
            {
                break;
            }

            char conversion = (char)format[i++];
            string body;
            string prefix = string.Empty;
            bool numeric = true;
            switch (conversion)
            {
                case '%':
                    output.Add((byte)'%');
                    continue;
                case 'd':
                case 'i':
                {
                    long value = ReadSigned(ref va, size);
                    body = Digits((ulong)Math.Abs((decimal)value), 10, false, precision);
                    prefix = value < 0 ? "-" : plus ? "+" : space ? " " : string.Empty;
                    break;
                }

                case 'u':
                case 'o':
                case 'x':
                case 'X':
                {
                    ulong value = ReadUnsigned(ref va, size);
                    int radix = conversion == 'u' ? 10 : conversion == 'o' ? 8 : 16;
                    body = Digits(value, radix, conversion == 'X', precision);
                    if (alternate && value != 0 && radix == 16)
                    {
                        prefix = conversion == 'X' ? "0X" : "0x";
                    }
                    else if (alternate && radix == 8 && !body.StartsWith('0'))
                    {
                        body = "0" + body;
                    }

                    break;
                }

                case 'p':
                {
                    nint value = va.NextPointer();
                    body = va.PointerSize == 4 ? ((uint)value).ToString("X8", CultureInfo.InvariantCulture) : ((ulong)value).ToString("X16", CultureInfo.InvariantCulture);

                    numeric = false;
                    break;
                }

                case 'c':
                {
                    byte value = (byte)va.NextInt32();
                    Pad(output, [value], string.Empty, width, left, zero: false);
                    continue;
                }

                case 's':
                case 'S':
                {
                    nint pointer = va.NextPointer();
                    byte[] text = pointer == 0 ? "(null)"u8.ToArray() : (size == 1 || conversion == 'S' ? Encoding.GetBytes(new string((char*)pointer)) : NativeString.AsSpan((byte*)pointer).ToArray());
                    if (precision >= 0 && text.Length > precision)
                    {
                        text = text[..precision];
                    }

                    Pad(output, text, string.Empty, width, left, zero: false);
                    continue;
                }

                case 'n':
                {
                    nint pointer = va.NextPointer();
                    if (pointer != 0)
                    {
                        *(int*)pointer = output.Count;
                    }

                    continue;
                }

                case 'f':
                case 'F':
                case 'e':
                case 'E':
                case 'g':
                case 'G':
                case 'a':
                case 'A':
                {
                    double value = va.NextDouble();
                    body = FloatBody(value, conversion, precision < 0 ? 6 : precision, alternate);
                    if (body.StartsWith('-'))
                    {
                        prefix = "-";
                        body = body[1..];
                    }
                    else
                    {
                        prefix = plus ? "+" : space ? " " : string.Empty;
                    }

                    if (double.IsNaN(value) || double.IsInfinity(value))
                    {
                        zero = false;
                    }

                    precision = -1; // the zero flag applies to floats regardless of precision
                    break;
                }

                default:
                    // Unknown conversion: printed literally, like the CRT's lenient mode.
                    output.Add((byte)'%');
                    output.Add((byte)conversion);
                    continue;
            }

            // The 0 flag is ignored with '-' and, for integers, when a precision is given.
            bool zeroPad = zero && !left && (precision < 0 || !numeric) && numeric;
            Pad(output, Encoding.ASCII.GetBytes(body), prefix, width, left, zeroPad);
        }

        return [.. output];
    }

    private static long ReadSigned(ref VaList va, int size) => size switch
    {
        2 => va.NextInt64(),
        3 => va.PointerSize == 8 ? va.NextInt64() : va.NextInt32(),
        -1 => (short)va.NextInt32(),
        -2 => (sbyte)va.NextInt32(),
        _ => va.NextInt32(),
    };

    private static ulong ReadUnsigned(ref VaList va, int size) => size switch
    {
        2 => (ulong)va.NextInt64(),
        3 => va.PointerSize == 8 ? (ulong)va.NextInt64() : (uint)va.NextInt32(),
        -1 => (ushort)va.NextInt32(),
        -2 => (byte)va.NextInt32(),
        _ => (uint)va.NextInt32(),
    };

    private static string Digits(ulong value, int radix, bool upper, int precision)
    {
        if (precision == 0 && value == 0)
        {
            return string.Empty;
        }

        string digits = radix switch
        {
            10 => value.ToString(CultureInfo.InvariantCulture),
            16 => value.ToString(upper ? "X" : "x", CultureInfo.InvariantCulture),
            _ => Convert.ToString(unchecked((long)value), 8),
        };
        return precision > digits.Length ? new string('0', precision - digits.Length) + digits : digits;
    }

    private static void Pad(List<byte> output, byte[] body, string prefix, int width, bool left, bool zero)
    {
        int padding = Math.Max(0, width - body.Length - prefix.Length);
        if (!left && !zero)
        {
            output.AddRange(Enumerable.Repeat((byte)' ', padding));
        }

        output.AddRange(Encoding.ASCII.GetBytes(prefix));
        if (!left && zero)
        {
            output.AddRange(Enumerable.Repeat((byte)'0', padding));
        }

        output.AddRange(body);
        if (left)
        {
            output.AddRange(Enumerable.Repeat((byte)' ', padding));
        }
    }

    private static string FloatBody(double value, char conversion, int precision, bool alternate)
    {
        bool upper = char.IsUpper(conversion);
        if (double.IsNaN(value))
        {
            return upper ? "NAN" : "nan";
        }

        if (double.IsInfinity(value))
        {
            return (value < 0 ? "-" : string.Empty) + (upper ? "INF" : "inf");
        }

        switch (char.ToLowerInvariant(conversion))
        {
            case 'f':
            {
                string fixedText = value.ToString("F" + precision.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
                if (fixedText.StartsWith('-') && fixedText.Trim('-', '0', '.').Length == 0 && !double.IsNegative(value))
                {
                    fixedText = fixedText[1..];
                }

                return alternate && precision == 0 ? fixedText + "." : fixedText;
            }

            case 'e':
                return Exponential(value, precision, upper, alternate);
            case 'a':
                return value.ToString(CultureInfo.InvariantCulture);
            default:
                return General(value, precision == 0 ? 1 : precision, upper, alternate);
        }
    }

    private static string Exponential(double value, int precision, bool upper, bool alternate)
    {
        string text = value.ToString((upper ? "E" : "e") + precision.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        // .NET prints e+006; normalize the exponent to ExponentDigits digits.
        int e = text.IndexOfAny(['e', 'E']);
        string mantissa = text[..e];
        int exponent = int.Parse(text[(e + 1)..], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
        if (alternate && precision == 0)
        {
            mantissa += ".";
        }

        string digits = Math.Abs(exponent).ToString(CultureInfo.InvariantCulture).PadLeft(ExponentDigits, '0');
        return $"{mantissa}{(upper ? 'E' : 'e')}{(exponent < 0 ? '-' : '+')}{digits}";
    }

    private static string General(double value, int precision, bool upper, bool alternate)
    {
        if (value == 0)
        {
            string zero = alternate ? "0." + new string('0', precision - 1) : "0";
            return double.IsNegative(value) ? "-" + zero : zero;
        }

        // The exponent after rounding to 'precision' significant digits decides between %e and %f.
        string probe = Exponential(value, precision - 1, upper, alternate: false);
        int e = probe.IndexOfAny(['e', 'E']);
        int exponent = int.Parse(probe[(e + 1)..], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
        string text = exponent < -4 || exponent >= precision
            ? probe
            : value.ToString("F" + (precision - 1 - exponent).ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        if (alternate)
        {
            return text;
        }

        // Without '#', trailing zeros of the fraction (and a bare point) go.
        int exponentAt = text.IndexOfAny(['e', 'E']);
        string mantissa = exponentAt < 0 ? text : text[..exponentAt];
        string suffix = exponentAt < 0 ? string.Empty : text[exponentAt..];
        if (mantissa.Contains('.', StringComparison.Ordinal))
        {
            mantissa = mantissa.TrimEnd('0').TrimEnd('.');
        }

        return mantissa + suffix;
    }
}
