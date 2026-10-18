using System.Globalization;
using System.Text;

namespace Recompiler.Runtime;

/// <summary>
/// Structured event journal of a rewritten program, one JSON object per line:
/// <c>{"seq":1,"kind":"load-archive","path":"a.tpk","entries":"12"}</c>. Enabled by setting
/// <c>RECOMP_EVENTS</c> to a file path; otherwise every call is a no-op. Headless scenario runs compare
/// these journals between the original's expectations and the replacement.
/// </summary>
public static class EventLog
{
    public const string EnvironmentVariable = "RECOMP_EVENTS";

    private static readonly Lock Gate = new();
    private static readonly StreamWriter? Writer = Open();
    private static long _sequence;

    public static bool IsEnabled => Writer is not null;

    public static void Emit(string kind, params ReadOnlySpan<(string Key, string Value)> fields)
    {
        if (Writer is null)
        {
            return;
        }

        var builder = new StringBuilder(128);
        builder.Append("{\"seq\":").Append(Interlocked.Increment(ref _sequence).ToString(CultureInfo.InvariantCulture));
        builder.Append(",\"kind\":\"").Append(Escape(kind)).Append('"');
        foreach ((string key, string value) in fields)
        {
            builder.Append(",\"").Append(Escape(key)).Append("\":\"").Append(Escape(value)).Append('"');
        }

        builder.Append('}');
        lock (Gate)
        {
            Writer.WriteLine(builder.ToString());
            Writer.Flush();
        }
    }

    private static string Escape(string text)
    {
        var builder = new StringBuilder(text.Length + 4);
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

            return new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read), new UTF8Encoding(false));
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
