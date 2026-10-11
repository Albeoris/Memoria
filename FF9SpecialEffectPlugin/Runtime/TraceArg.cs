using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Recompiler.Runtime;

/// <summary>
/// Formats one traced argument value for a <see cref="Tracer"/> line: <c>bool</c> as <c>true</c>/<c>false</c>, integers in
/// decimal, <c>nint</c>/<c>nuint</c> as <c>0x</c> hex, floating point round-trip, <c>char</c> as <c>'c'</c>, strings quoted
/// and escaped (cut after <see cref="MaxStringLength"/> characters, <c>…</c> after the closing quote), enums by name, arrays
/// as <c>T[len]</c>, <c>null</c>, and any other value as <c>{TypeName}</c> (never a user <c>ToString</c>). Primitives are
/// tested with <c>typeof(T) ==</c> chains the compiler folds per instantiation, so they are never boxed.
/// </summary>
internal static class TraceArg
{
    /// <summary>Strings longer than this are cut and marked with <c>…</c> after the closing quote.</summary>
    public const int MaxStringLength = 64;

    /// <summary>Appends the text of <paramref name="value"/> to <paramref name="builder"/>.</summary>
    public static void Append<T>(StringBuilder builder, T value)
    {
        if (typeof(T) == typeof(int))
        {
            AppendFormatted(builder, Unsafe.As<T, int>(ref value), default);
        }
        else if (typeof(T) == typeof(uint))
        {
            AppendFormatted(builder, Unsafe.As<T, uint>(ref value), default);
        }
        else if (typeof(T) == typeof(nint))
        {
            AppendHex(builder, (nuint)Unsafe.As<T, nint>(ref value));
        }
        else if (typeof(T) == typeof(nuint))
        {
            AppendHex(builder, Unsafe.As<T, nuint>(ref value));
        }
        else if (typeof(T) == typeof(bool))
        {
            builder.Append(Unsafe.As<T, bool>(ref value) ? "true" : "false");
        }
        else if (typeof(T) == typeof(byte))
        {
            AppendFormatted(builder, Unsafe.As<T, byte>(ref value), default);
        }
        else if (typeof(T) == typeof(sbyte))
        {
            AppendFormatted(builder, Unsafe.As<T, sbyte>(ref value), default);
        }
        else if (typeof(T) == typeof(short))
        {
            AppendFormatted(builder, Unsafe.As<T, short>(ref value), default);
        }
        else if (typeof(T) == typeof(ushort))
        {
            AppendFormatted(builder, Unsafe.As<T, ushort>(ref value), default);
        }
        else if (typeof(T) == typeof(long))
        {
            AppendFormatted(builder, Unsafe.As<T, long>(ref value), default);
        }
        else if (typeof(T) == typeof(ulong))
        {
            AppendFormatted(builder, Unsafe.As<T, ulong>(ref value), default);
        }
        else if (typeof(T) == typeof(float))
        {
            AppendFormatted(builder, Unsafe.As<T, float>(ref value), "R");
        }
        else if (typeof(T) == typeof(double))
        {
            AppendFormatted(builder, Unsafe.As<T, double>(ref value), "R");
        }
        else if (typeof(T) == typeof(char))
        {
            AppendChar(builder, Unsafe.As<T, char>(ref value));
        }
        else if (typeof(T) == typeof(string))
        {
            AppendString(builder, Unsafe.As<T, string?>(ref value));
        }
        else
        {
            AppendObject(builder, value);
        }
    }

    /// <summary>Everything that is not a primitive: boxed once (enums, structs) and dispatched on the runtime type.</summary>
    private static void AppendObject(StringBuilder builder, object? value)
    {
        switch (value)
        {
            case null:
                builder.Append("null");
                break;
            case string text:
                AppendString(builder, text);
                break;
            case Enum:
                builder.Append(value.ToString());
                break;
            case Array array:
                builder.Append(TypeName(array.GetType().GetElementType())).Append('[');
                AppendFormatted(builder, array.Length, default);
                builder.Append(']');
                break;
            case decimal number:
                AppendFormatted(builder, number, default);
                break;
            case Half number:
                AppendFormatted(builder, number, "R");
                break;
            case Guid guid:
                AppendFormatted(builder, guid, "D");
                break;
            case DateTime time:
                AppendFormatted(builder, time, "O");
                break;
            case DateTimeOffset time:
                AppendFormatted(builder, time, "O");
                break;
            case TimeSpan span:
                AppendFormatted(builder, span, "c");
                break;
            case Version version:
                builder.Append(version.ToString());
                break;
            default:
                // Boxed primitives (a Nullable<int> with a value, an object parameter) still print as values.
                if (!AppendBoxedPrimitive(builder, value))
                {
                    builder.Append('{').Append(TypeName(value.GetType())).Append('}');
                }

                break;
        }
    }

    private static bool AppendBoxedPrimitive(StringBuilder builder, object value)
    {
        switch (value)
        {
            case int v: Append(builder, v); return true;
            case uint v: Append(builder, v); return true;
            case nint v: Append(builder, v); return true;
            case nuint v: Append(builder, v); return true;
            case bool v: Append(builder, v); return true;
            case byte v: Append(builder, v); return true;
            case sbyte v: Append(builder, v); return true;
            case short v: Append(builder, v); return true;
            case ushort v: Append(builder, v); return true;
            case long v: Append(builder, v); return true;
            case ulong v: Append(builder, v); return true;
            case float v: Append(builder, v); return true;
            case double v: Append(builder, v); return true;
            case char v: Append(builder, v); return true;
            default: return false;
        }
    }

    private static void AppendHex(StringBuilder builder, nuint value)
    {
        builder.Append("0x");
        AppendFormatted(builder, (ulong)value, "x");
    }

    private static void AppendFormatted<TValue>(StringBuilder builder, TValue value, ReadOnlySpan<char> format)
        where TValue : ISpanFormattable
    {
        Span<char> buffer = stackalloc char[64];
        if (value.TryFormat(buffer, out int written, format, CultureInfo.InvariantCulture))
        {
            builder.Append(buffer[..written]);
        }
        else
        {
            builder.Append(value.ToString(format.ToString(), CultureInfo.InvariantCulture));
        }
    }

    private static void AppendChar(StringBuilder builder, char value)
    {
        builder.Append('\'');
        AppendEscaped(builder, value, '\'');
        builder.Append('\'');
    }

    private static void AppendString(StringBuilder builder, string? value)
    {
        if (value is null)
        {
            builder.Append("null");
            return;
        }

        int length = Math.Min(value.Length, MaxStringLength);
        builder.Append('"');
        for (int i = 0; i < length; i++)
        {
            AppendEscaped(builder, value[i], '"');
        }

        builder.Append('"');
        if (value.Length > MaxStringLength)
        {
            builder.Append('…');
        }
    }

    private static void AppendEscaped(StringBuilder builder, char c, char quote)
    {
        switch (c)
        {
            case '\\': builder.Append(@"\\"); break;
            case '\n': builder.Append(@"\n"); break;
            case '\r': builder.Append(@"\r"); break;
            case '\t': builder.Append(@"\t"); break;
            case '\0': builder.Append(@"\0"); break;
            default:
                if (c == quote)
                {
                    builder.Append('\\').Append(c);
                }
                else if (c < ' ' || c == '\u007f' || char.IsSurrogate(c))
                {
                    builder.Append(@"\u");
                    AppendFormatted(builder, (int)c, "x4");
                }
                else
                {
                    builder.Append(c);
                }

                break;
        }
    }

    /// <summary>The C# keyword for primitives, otherwise the type's simple name.</summary>
    private static string TypeName(Type? type)
    {
        if (type is null)
        {
            return "?";
        }

        if (type.IsEnum)
        {
            return type.Name;
        }

        return Type.GetTypeCode(type) switch
        {
            TypeCode.Boolean => "bool",
            TypeCode.Char => "char",
            TypeCode.SByte => "sbyte",
            TypeCode.Byte => "byte",
            TypeCode.Int16 => "short",
            TypeCode.UInt16 => "ushort",
            TypeCode.Int32 => "int",
            TypeCode.UInt32 => "uint",
            TypeCode.Int64 => "long",
            TypeCode.UInt64 => "ulong",
            TypeCode.Single => "float",
            TypeCode.Double => "double",
            TypeCode.Decimal => "decimal",
            TypeCode.String => "string",
            _ when type == typeof(nint) => "nint",
            _ when type == typeof(nuint) => "nuint",
            _ when type == typeof(object) => "object",
            _ => type.Name,
        };
    }
}
