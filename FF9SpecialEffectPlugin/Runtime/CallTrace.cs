using System.Globalization;
using System.Text;

namespace Recompiler.Runtime;

/// <summary>
/// Records calls made through <c>Trace</c>-mode export stubs as JSON Lines, one object per call:
/// <c>{"seq":1,"thread":4,"export":"tm_add","args":[{"name":"a","type":"int","value":"2"}],"return":{"type":"int","value":"5"}}</c>.
/// Tracing is enabled by setting the <c>RECOMP_TRACE</c> environment variable to an output file path;
/// without it every call is a no-op so traced builds can ship to real users.
/// </summary>
public static class CallTrace
{
    public const string EnvironmentVariable = "RECOMP_TRACE";

    private static readonly Lock Gate = new();
    private static readonly StreamWriter? Writer = Open();
    private static long _sequence;

    public static bool IsEnabled => Writer is not null;

    /// <summary>A traced value: scalars are formatted as decimal, pointers as hex, strings verbatim.</summary>
    public readonly record struct Value(string Name, string Type, string Text)
    {
        public static Value Of(string name, int value) => new(name, "int", value.ToString(CultureInfo.InvariantCulture));
        public static Value Of(string name, uint value) => new(name, "uint", value.ToString(CultureInfo.InvariantCulture));
        public static Value Of(string name, long value) => new(name, "long", value.ToString(CultureInfo.InvariantCulture));
        public static Value Of(string name, ulong value) => new(name, "ulong", value.ToString(CultureInfo.InvariantCulture));
        public static Value Of(string name, short value) => new(name, "short", value.ToString(CultureInfo.InvariantCulture));
        public static Value Of(string name, ushort value) => new(name, "ushort", value.ToString(CultureInfo.InvariantCulture));
        public static Value Of(string name, byte value) => new(name, "byte", value.ToString(CultureInfo.InvariantCulture));
        public static Value Of(string name, sbyte value) => new(name, "sbyte", value.ToString(CultureInfo.InvariantCulture));
        public static Value Of(string name, char value) => new(name, "char", ((int)value).ToString(CultureInfo.InvariantCulture));
        public static Value Of(string name, float value) => new(name, "float", value.ToString("R", CultureInfo.InvariantCulture));
        public static Value Of(string name, double value) => new(name, "double", value.ToString("R", CultureInfo.InvariantCulture));
        public static Value Of(string name, nint value) => new(name, "nint", "0x" + ((long)value).ToString("x", CultureInfo.InvariantCulture));
        public static Value Of(string name, nuint value) => new(name, "nuint", "0x" + ((ulong)value).ToString("x", CultureInfo.InvariantCulture));
        public static unsafe Value Of(string name, void* value) => new(name, "pointer", "0x" + ((ulong)value).ToString("x", CultureInfo.InvariantCulture));
        public static Value Of(string name, Guid value) => new(name, "guid", value.ToString("D"));
        public static Value Void(string name) => new(name, "void", string.Empty);
    }

    public static void Record(string export, ReadOnlySpan<Value> arguments, Value result)
    {
        if (Writer is null)
        {
            return;
        }

        var builder = new StringBuilder(256);
        builder.Append("{\"seq\":").Append(Interlocked.Increment(ref _sequence).ToString(CultureInfo.InvariantCulture));
        builder.Append(",\"thread\":").Append(Environment.CurrentManagedThreadId.ToString(CultureInfo.InvariantCulture));
        builder.Append(",\"export\":\"").Append(Escape(export)).Append("\",\"args\":[");
        for (int i = 0; i < arguments.Length; i++)
        {
            if (i > 0)
            {
                builder.Append(',');
            }

            AppendValue(builder, arguments[i]);
        }

        builder.Append("],\"return\":");
        AppendValue(builder, result);
        builder.Append('}');

        lock (Gate)
        {
            Writer.WriteLine(builder.ToString());
            Writer.Flush();
        }
    }

    private static void AppendValue(StringBuilder builder, Value value)
    {
        builder.Append("{\"name\":\"").Append(Escape(value.Name))
            .Append("\",\"type\":\"").Append(Escape(value.Type))
            .Append("\",\"value\":\"").Append(Escape(value.Text)).Append("\"}");
    }

    private static string Escape(string text)
    {
        if (text.AsSpan().IndexOfAny('"', '\\') < 0 && !text.Any(c => c < ' '))
        {
            return text;
        }

        var builder = new StringBuilder(text.Length + 8);
        foreach (char c in text)
        {
            switch (c)
            {
                case '"': builder.Append("\\\""); break;
                case '\\': builder.Append("\\\\"); break;
                case '\n': builder.Append("\\n"); break;
                case '\r': builder.Append("\\r"); break;
                case '\t': builder.Append("\\t"); break;
                default:
                    if (c < ' ')
                    {
                        builder.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        builder.Append(c);
                    }

                    break;
            }
        }

        return builder.ToString();
    }

    private static StreamWriter? Open()
    {
        string? path = Environment.GetEnvironmentVariable(EnvironmentVariable);
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        try
        {
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            return new StreamWriter(stream, new UTF8Encoding(false));
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }
}
