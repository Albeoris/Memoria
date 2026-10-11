using System.Globalization;
using System.Text;

namespace Recompiler.Runtime;

/// <summary>The input of a <see cref="CScan"/> run, one byte at a time: <see cref="Read"/> returns a byte or -1 at the end, <see cref="Unread"/> pushes the last byte back (one level).</summary>
public interface IScanInput
{
    int Read();

    void Unread(int value);
}

/// <summary>
/// The C runtime's <c>scanf</c> family in the C locale, for ported <c>sscanf</c>/<c>fscanf</c> calls: whitespace directives,
/// literal characters, <c>%%</c>, and the conversions <c>d i u o x X p</c> (with <c>hh h l ll I64</c>), <c>e f g E G</c>
/// (float, <c>l</c>/<c>L</c> double), <c>s c [set]</c> (<c>l</c> stores wide characters) and <c>n</c>, with field widths and
/// <c>*</c> suppression. Returns the number of fields assigned, or -1 (<c>EOF</c>) when the input ends before the first
/// conversion; a matching failure stops the scan. Pointer arguments come from the original's stack as <see cref="VaList"/>
/// slots. Integers that overflow wrap to the stored size like the CRT; text is 8-bit.
/// </summary>
public static unsafe class CScan
{
    public const int EOF = -1;

    /// <summary><c>sscanf(input, format, ...)</c> with x86 arguments.</summary>
    public static int Sscanf(byte* input, byte* format, uint* va) => Scan(new StringInput(input), NativeString.AsSpan(format), VaList.X86(va));

    /// <summary><c>sscanf(input, format, ...)</c> with x64 arguments.</summary>
    public static int Sscanf(byte* input, byte* format, ulong* va) => Scan(new StringInput(input), NativeString.AsSpan(format), VaList.X64(va));

    /// <summary>Runs <paramref name="format"/> over <paramref name="input"/>, storing through the pointers in <paramref name="va"/>.</summary>
    public static int Scan(IScanInput input, ReadOnlySpan<byte> format, VaList va)
    {
        ArgumentNullException.ThrowIfNull(input);
        var reader = new Reader(input);
        int assigned = 0;
        bool converted = false;
        int i = 0;
        while (i < format.Length)
        {
            byte directive = format[i];
            if (IsSpace(directive))
            {
                reader.SkipSpace();
                while (i < format.Length && IsSpace(format[i]))
                {
                    i++;
                }

                continue;
            }

            if (directive != (byte)'%' || i + 1 < format.Length && format[i + 1] == (byte)'%')
            {
                if (directive == (byte)'%')
                {
                    reader.SkipSpace();
                    i++;
                }

                int c = reader.Read();
                if (c != directive)
                {
                    if (c < 0)
                    {
                        return converted ? assigned : EOF;
                    }

                    reader.Unread(c);
                    return assigned;
                }

                i++;
                continue;
            }

            i++;
            if (i >= format.Length)
            {
                break;
            }

            bool suppress = format[i] == (byte)'*';
            if (suppress)
            {
                i++;
            }

            int width = 0;
            while (i < format.Length && format[i] is >= (byte)'0' and <= (byte)'9')
            {
                width = width * 10 + (format[i++] - '0');
            }

            Length length = ParseLength(format, ref i);
            int size = IntegerSize(length);
            bool wide = length == Length.Long;
            if (i >= format.Length)
            {
                break;
            }

            byte conversion = format[i++];
            Outcome outcome;
            switch (conversion)
            {
                case (byte)'n':
                    if (!suppress)
                    {
                        Store((byte*)va.NextPointer(), size, reader.Count);
                    }

                    continue;
                case (byte)'d':
                case (byte)'u':
                    outcome = ScanInteger(reader, width, 10, suppress, size, ref va);
                    break;
                case (byte)'i':
                    outcome = ScanInteger(reader, width, 0, suppress, size, ref va);
                    break;
                case (byte)'o':
                    outcome = ScanInteger(reader, width, 8, suppress, size, ref va);
                    break;
                case (byte)'x':
                case (byte)'X':
                    outcome = ScanInteger(reader, width, 16, suppress, size, ref va);
                    break;
                case (byte)'p':
                    outcome = ScanInteger(reader, width, 16, suppress, va.PointerSize, ref va);
                    break;
                case (byte)'e':
                case (byte)'E':
                case (byte)'f':
                case (byte)'g':
                case (byte)'G':
                    outcome = ScanFloat(reader, width, suppress, length is Length.Long or Length.LongDouble, ref va);
                    break;
                case (byte)'s':
                    reader.SkipSpace();
                    outcome = ScanChars(reader, width, suppress, wide, terminate: true, static c => !IsSpace((byte)c), ref va);
                    break;
                case (byte)'c':
                    outcome = ScanChars(reader, width == 0 ? 1 : width, suppress, wide, terminate: false, static _ => true, ref va);
                    break;
                case (byte)'[':
                    bool[] set = ParseSet(format, ref i);
                    outcome = ScanChars(reader, width, suppress, wide, terminate: true, c => set[c], ref va);
                    break;
                default:
                    return assigned;
            }

            if (outcome == Outcome.EndOfInput)
            {
                return converted ? assigned : EOF;
            }

            if (outcome == Outcome.Mismatch)
            {
                return assigned;
            }

            converted = true;
            if (!suppress)
            {
                assigned++;
            }
        }

        return assigned;
    }

    private enum Outcome
    {
        Stored,
        Mismatch,
        EndOfInput,
    }

    private enum Length
    {
        None,
        Char,
        Short,
        Long,
        LongLong,
        LongDouble,
    }

    /// <summary>The integer bytes a length modifier stores: <c>hh</c> 1, <c>h</c> 2, none and <c>l</c> 4 (a Windows <c>long</c>), <c>ll</c>/<c>I64</c> 8.</summary>
    private static int IntegerSize(Length length) => length switch
    {
        Length.Char => 1,
        Length.Short => 2,
        Length.LongLong or Length.LongDouble => 8,
        _ => 4,
    };

    private static Length ParseLength(ReadOnlySpan<byte> format, ref int i)
    {
        if (i >= format.Length)
        {
            return Length.None;
        }

        ReadOnlySpan<byte> rest = format[i..];
        if (rest.StartsWith("hh"u8) || rest.StartsWith("ll"u8) || rest.StartsWith("I64"u8) || rest.StartsWith("I32"u8))
        {
            i += rest[0] == (byte)'I' ? 3 : 2;
            return rest[0] switch
            {
                (byte)'h' => Length.Char,
                (byte)'l' => Length.LongLong,
                _ => rest[1] == (byte)'6' ? Length.LongLong : Length.None,
            };
        }

        switch (rest[0])
        {
            case (byte)'h':
                i++;
                return Length.Short;
            case (byte)'l':
            case (byte)'w':
                i++;
                return Length.Long;
            case (byte)'L':
                i++;
                return Length.LongDouble;
            default:
                return Length.None;
        }
    }

    private static Outcome ScanInteger(Reader reader, int width, int radix, bool suppress, int size, ref VaList va)
    {
        reader.SkipSpace();
        int remaining = width == 0 ? int.MaxValue : width;
        int c = reader.Peek();
        if (c < 0)
        {
            return Outcome.EndOfInput;
        }

        bool negative = false;
        if (c is '+' or '-')
        {
            negative = c == '-';
            reader.Read();
            remaining--;
        }

        ulong value = 0;
        int digits = 0;
        if ((radix is 0 or 16) && remaining > 0 && reader.Peek() == '0')
        {
            reader.Read();
            remaining--;
            digits++;
            if (remaining > 0 && reader.Peek() is 'x' or 'X')
            {
                reader.Read();
                remaining--;
                radix = 16;
            }
            else if (radix == 0)
            {
                radix = 8;
            }
        }

        if (radix == 0)
        {
            radix = 10;
        }

        while (remaining > 0 && Digit(reader.Peek(), radix) is int digit and >= 0)
        {
            reader.Read();
            remaining--;
            digits++;
            value = unchecked(value * (ulong)radix + (ulong)digit);
        }

        if (digits == 0)
        {
            return reader.Peek() < 0 ? Outcome.EndOfInput : Outcome.Mismatch;
        }

        if (negative)
        {
            value = unchecked(0 - value);
        }

        if (!suppress)
        {
            Store((byte*)va.NextPointer(), size, unchecked((long)value));
        }

        return Outcome.Stored;
    }

    private static Outcome ScanFloat(Reader reader, int width, bool suppress, bool isDouble, ref VaList va)
    {
        reader.SkipSpace();
        if (reader.Peek() < 0)
        {
            return Outcome.EndOfInput;
        }

        int remaining = width == 0 ? int.MaxValue : width;
        var text = new StringBuilder();
        int digits = 0;
        if (reader.Peek() is '+' or '-')
        {
            text.Append((char)reader.Read());
            remaining--;
        }

        digits += TakeDigits(reader, text, ref remaining);
        if (remaining > 0 && reader.Peek() == '.')
        {
            text.Append((char)reader.Read());
            remaining--;
            digits += TakeDigits(reader, text, ref remaining);
        }

        if (digits == 0)
        {
            return reader.Peek() < 0 ? Outcome.EndOfInput : Outcome.Mismatch;
        }

        if (remaining > 0 && reader.Peek() is 'e' or 'E')
        {
            var exponent = new StringBuilder();
            exponent.Append((char)reader.Read());
            remaining--;
            if (remaining > 0 && reader.Peek() is '+' or '-')
            {
                exponent.Append((char)reader.Read());
                remaining--;
            }

            if (TakeDigits(reader, exponent, ref remaining) > 0)
            {
                text.Append(exponent);
            }
        }

        if (!suppress)
        {
            double value = double.Parse(text.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture);
            byte* target = (byte*)va.NextPointer();
            if (isDouble)
            {
                *(double*)target = value;
            }
            else
            {
                *(float*)target = (float)value;
            }
        }

        return Outcome.Stored;
    }

    private static int TakeDigits(Reader reader, StringBuilder text, ref int remaining)
    {
        int count = 0;
        while (remaining > 0 && reader.Peek() is >= '0' and <= '9')
        {
            text.Append((char)reader.Read());
            remaining--;
            count++;
        }

        return count;
    }

    /// <summary><c>%s</c>, <c>%c</c> and <c>%[</c>: copies matching bytes (at most <paramref name="width"/>, 0 = unlimited) and, unless <c>%c</c>, a NUL.</summary>
    private static Outcome ScanChars(Reader reader, int width, bool suppress, bool wide, bool terminate, Func<int, bool> accept, ref VaList va)
    {
        if (reader.Peek() < 0)
        {
            return Outcome.EndOfInput;
        }

        byte* target = suppress ? null : (byte*)va.NextPointer();
        int count = 0;
        while ((width == 0 || count < width) && reader.Peek() is int c and >= 0 && accept(c))
        {
            reader.Read();
            if (target is not null)
            {
                if (wide)
                {
                    ((char*)target)[count] = (char)c;
                }
                else
                {
                    target[count] = (byte)c;
                }
            }

            count++;
        }

        if (count == 0)
        {
            return reader.Peek() < 0 ? Outcome.EndOfInput : Outcome.Mismatch;
        }

        if (terminate && target is not null)
        {
            if (wide)
            {
                ((char*)target)[count] = '\0';
            }
            else
            {
                target[count] = 0;
            }
        }

        return Outcome.Stored;
    }

    /// <summary>The scan set after <c>%[</c>: <c>^</c> negates, a leading <c>]</c> is a member, <c>a-z</c> is a range (MSVC).</summary>
    private static bool[] ParseSet(ReadOnlySpan<byte> format, ref int i)
    {
        var set = new bool[256];
        bool negate = i < format.Length && format[i] == (byte)'^';
        if (negate)
        {
            i++;
        }

        bool first = true;
        while (i < format.Length && (format[i] != (byte)']' || first))
        {
            byte low = format[i++];
            if (i + 1 < format.Length && format[i] == (byte)'-' && format[i + 1] != (byte)']')
            {
                byte high = format[i + 1];
                i += 2;
                for (int c = Math.Min(low, high); c <= Math.Max(low, high); c++)
                {
                    set[c] = true;
                }
            }
            else
            {
                set[low] = true;
            }

            first = false;
        }

        if (i < format.Length)
        {
            i++; // the closing ]
        }

        if (negate)
        {
            for (int c = 0; c < set.Length; c++)
            {
                set[c] = !set[c];
            }
        }

        return set;
    }

    private static void Store(byte* target, int size, long value)
    {
        switch (size)
        {
            case 1:
                *target = unchecked((byte)value);
                break;
            case 2:
                *(short*)target = unchecked((short)value);
                break;
            case 8:
                *(long*)target = value;
                break;
            default:
                *(int*)target = unchecked((int)value);
                break;
        }
    }

    private static int Digit(int c, int radix)
    {
        int value = c switch
        {
            >= '0' and <= '9' => c - '0',
            >= 'a' and <= 'f' => c - 'a' + 10,
            >= 'A' and <= 'F' => c - 'A' + 10,
            _ => -1,
        };
        return value < radix ? value : -1;
    }

    private static bool IsSpace(byte c) => c is (byte)' ' or >= 0x09 and <= 0x0D;

    /// <summary>Counts what the conversions consumed (<c>%n</c>) and gives one byte of look-ahead.</summary>
    private sealed class Reader(IScanInput input)
    {
        public int Count { get; private set; }

        public int Read()
        {
            int c = input.Read();
            if (c >= 0)
            {
                Count++;
            }

            return c;
        }

        public void Unread(int c)
        {
            if (c >= 0)
            {
                input.Unread(c);
                Count--;
            }
        }

        public int Peek()
        {
            int c = input.Read();
            if (c >= 0)
            {
                input.Unread(c);
            }

            return c;
        }

        public void SkipSpace()
        {
            while (Peek() is int c and >= 0 && IsSpace((byte)c))
            {
                Read();
            }
        }
    }

    /// <summary>A NUL-terminated 8-bit string (<c>sscanf</c>).</summary>
    private sealed class StringInput(byte* text) : IScanInput
    {
        private byte* _cursor = text;

        public int Read()
        {
            if (_cursor is null || *_cursor == 0)
            {
                return -1;
            }

            return *_cursor++;
        }

        public void Unread(int value) => _cursor--;
    }
}
