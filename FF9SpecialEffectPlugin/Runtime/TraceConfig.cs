using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Recompiler.Runtime;

/// <summary>
/// One parsed <see cref="Tracer"/> configuration (JSON with comments and trailing commas; every key optional, unknown keys
/// ignored): <c>enabled</c>, <c>log</c>, <c>append</c>, <c>functions</c>, <c>exclude</c>, <c>args</c>, <c>timestamps</c>,
/// <c>threads</c>, <c>cap {perFunction,total}</c>, <c>break {match,regex,count,wait}</c> and <c>modules {Name: {...}}</c>,
/// whose section for the running module is merged key by key over the top level. Immutable once parsed.
/// </summary>
internal sealed class TraceConfig
{
    private static readonly JsonDocumentOptions Options = new() { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };

    private TraceConfig()
    {
    }

    /// <summary>False turns tracing off while the file is still watched.</summary>
    public bool Enabled { get; private set; } = true;

    /// <summary>The log path as written (relative to the config file's directory); null: <c>&lt;module&gt;.trace.log</c>.</summary>
    public string? Log { get; private set; }

    /// <summary>False: the log is truncated when this process first opens it.</summary>
    public bool Append { get; private set; }

    /// <summary>Globs (<c>*</c>, <c>?</c>, case-sensitive) against <c>name</c> or <c>module!name</c>; a call is traced when one matches.</summary>
    public IReadOnlyList<string> Functions { get; private set; } = ["*"];

    /// <summary>Globs that exclude a function the <see cref="Functions"/> selected.</summary>
    public IReadOnlyList<string> Exclude { get; private set; } = [];

    /// <summary>False: lines carry the function name only.</summary>
    public bool Args { get; private set; } = true;

    public bool Timestamps { get; private set; } = true;

    public bool Threads { get; private set; } = true;

    /// <summary>Lines per function before it goes quiet; 0 = unlimited.</summary>
    public long CapPerFunction { get; private set; }

    /// <summary>Lines in all before tracing stops; 0 = unlimited.</summary>
    public long CapTotal { get; private set; }

    /// <summary>Text (or regex with <see cref="BreakRegex"/>) a written line must contain to break; null: never break.</summary>
    public string? BreakMatch { get; private set; }

    public bool BreakRegex { get; private set; }

    /// <summary>Breaks for the first N matching lines; 0 or less: every matching line.</summary>
    public int BreakCount { get; private set; } = 1;

    /// <summary>Seconds to wait for a debugger to attach when none is attached at a break; 0: skip at once.</summary>
    public double BreakWait { get; private set; }

    /// <summary>The compiled <see cref="BreakMatch"/> when <see cref="BreakRegex"/> is set.</summary>
    public Regex? BreakPattern { get; private set; }

    /// <summary>Parses <paramref name="json"/>; throws <see cref="JsonException"/>, <see cref="FormatException"/> or <see cref="ArgumentException"/> (bad regex).</summary>
    public static TraceConfig Parse(string json, string moduleName)
    {
        using JsonDocument document = JsonDocument.Parse(json, Options);
        JsonElement root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new FormatException("the config must be a JSON object");
        }

        var config = new TraceConfig();
        config.Apply(root);
        if (root.TryGetProperty("modules", out JsonElement modules))
        {
            if (modules.ValueKind != JsonValueKind.Object)
            {
                throw new FormatException("'modules' must be an object of module sections");
            }

            foreach (JsonProperty section in modules.EnumerateObject())
            {
                if (string.Equals(section.Name, moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    if (section.Value.ValueKind != JsonValueKind.Object)
                    {
                        throw new FormatException($"'modules.{section.Name}' must be an object");
                    }

                    config.Apply(section.Value);
                }
            }
        }

        if (config.BreakRegex && config.BreakMatch is not null)
        {
            config.BreakPattern = new Regex(config.BreakMatch, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
        }

        return config;
    }

    /// <summary>The selection: one of <see cref="Functions"/> matches and none of <see cref="Exclude"/>.</summary>
    public bool Selects(string name, string qualifiedName) =>
        AnyMatch(Functions, name, qualifiedName) && !AnyMatch(Exclude, name, qualifiedName);

    /// <summary>The full path of the log file for a config file in <paramref name="configDirectory"/>.</summary>
    public string LogPath(string configDirectory, string moduleName) =>
        Path.GetFullPath(Path.Combine(configDirectory, string.IsNullOrWhiteSpace(Log) ? Tracer.DefaultLogFileNameFor(moduleName) : Log));

    private static bool AnyMatch(IReadOnlyList<string> globs, string name, string qualifiedName)
    {
        foreach (string glob in globs)
        {
            if (TraceGlob.IsMatch(glob, name) || TraceGlob.IsMatch(glob, qualifiedName))
            {
                return true;
            }
        }

        return false;
    }

    private void Apply(JsonElement section)
    {
        foreach (JsonProperty property in section.EnumerateObject())
        {
            switch (property.Name)
            {
                case "enabled": Enabled = Bool(property); break;
                case "log": Log = Text(property); break;
                case "append": Append = Bool(property); break;
                case "functions": Functions = Globs(property); break;
                case "exclude": Exclude = Globs(property); break;
                case "args": Args = Bool(property); break;
                case "timestamps": Timestamps = Bool(property); break;
                case "threads": Threads = Bool(property); break;
                case "cap": ApplyCap(property); break;
                case "break": ApplyBreak(property); break;
                default: break;
            }
        }
    }

    private void ApplyCap(JsonProperty cap)
    {
        RequireObject(cap);
        foreach (JsonProperty property in cap.Value.EnumerateObject())
        {
            switch (property.Name)
            {
                case "perFunction": CapPerFunction = Count(property); break;
                case "total": CapTotal = Count(property); break;
                default: break;
            }
        }
    }

    private void ApplyBreak(JsonProperty breakSection)
    {
        if (breakSection.Value.ValueKind == JsonValueKind.String)
        {
            BreakMatch = breakSection.Value.GetString();
            return;
        }

        if (breakSection.Value.ValueKind == JsonValueKind.Null)
        {
            BreakMatch = null;
            return;
        }

        RequireObject(breakSection);
        foreach (JsonProperty property in breakSection.Value.EnumerateObject())
        {
            switch (property.Name)
            {
                case "match": BreakMatch = Text(property); break;
                case "regex": BreakRegex = Bool(property); break;
                case "count": BreakCount = (int)Math.Clamp(Count(property), 0, int.MaxValue); break;
                case "wait": BreakWait = Seconds(property); break;
                default: break;
            }
        }
    }

    private static void RequireObject(JsonProperty property)
    {
        if (property.Value.ValueKind != JsonValueKind.Object)
        {
            throw new FormatException($"'{property.Name}' must be an object");
        }
    }

    private static bool Bool(JsonProperty property) => property.Value.ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        _ => throw new FormatException($"'{property.Name}' must be true or false"),
    };

    private static string? Text(JsonProperty property) => property.Value.ValueKind switch
    {
        JsonValueKind.String => property.Value.GetString(),
        JsonValueKind.Null => null,
        _ => throw new FormatException($"'{property.Name}' must be a string"),
    };

    private static long Count(JsonProperty property) =>
        property.Value.ValueKind == JsonValueKind.Number && property.Value.TryGetInt64(out long value) && value >= 0
            ? value
            : throw new FormatException($"'{property.Name}' must be a whole number >= 0");

    private static double Seconds(JsonProperty property) =>
        property.Value.ValueKind == JsonValueKind.Number && property.Value.TryGetDouble(out double value) && value >= 0
            ? value
            : throw new FormatException($"'{property.Name}' must be a number of seconds >= 0");

    private static string[] Globs(JsonProperty property)
    {
        if (property.Value.ValueKind == JsonValueKind.String)
        {
            return [property.Value.GetString()!];
        }

        if (property.Value.ValueKind != JsonValueKind.Array)
        {
            throw new FormatException($"'{property.Name}' must be a glob or an array of globs");
        }

        var globs = new List<string>();
        foreach (JsonElement item in property.Value.EnumerateArray())
        {
            globs.Add(item.ValueKind == JsonValueKind.String
                ? item.GetString()!
                : throw new FormatException($"'{property.Name}' entries must be strings"));
        }

        return [.. globs];
    }
}

/// <summary>Case-sensitive wildcard match: <c>*</c> any run of characters, <c>?</c> one character.</summary>
internal static class TraceGlob
{
    public static bool IsMatch(string pattern, string text)
    {
        int p = 0;
        int t = 0;
        int star = -1;
        int resume = 0;
        while (t < text.Length)
        {
            if (p < pattern.Length && (pattern[p] == '?' || pattern[p] == text[t]))
            {
                p++;
                t++;
            }
            else if (p < pattern.Length && pattern[p] == '*')
            {
                star = p++;
                resume = t;
            }
            else if (star >= 0)
            {
                p = star + 1;
                t = ++resume;
            }
            else
            {
                return false;
            }
        }

        while (p < pattern.Length && pattern[p] == '*')
        {
            p++;
        }

        return p == pattern.Length;
    }
}

/// <summary>
/// Finds the config file of a module, first existing wins: <c>$RECOMP_TRACE_CONFIG</c>, <c>&lt;moduleDir&gt;/&lt;module&gt;.trace.json</c>,
/// <c>&lt;moduleDir&gt;/recomp.trace.json</c>.
/// </summary>
internal static class TraceConfigLocator
{
    public static string? Find(string? explicitPath, string moduleDirectory, string moduleName)
    {
        if (!string.IsNullOrWhiteSpace(explicitPath) && File.Exists(explicitPath))
        {
            return Path.GetFullPath(explicitPath);
        }

        foreach (string name in (string[])[Tracer.ConfigFileNameFor(moduleName), Tracer.SharedConfigFileName])
        {
            string candidate = Path.Combine(moduleDirectory, name);
            if (File.Exists(candidate))
            {
                return Path.GetFullPath(candidate);
            }
        }

        return null;
    }
}

/// <summary>The outcome of reading a config file: a config, an error message, or the file is missing (both null).</summary>
internal readonly record struct TraceConfigLoad(TraceConfig? Config, string? Error)
{
    public const int Retries = 5;
    public const int RetryDelayMilliseconds = 20;

    public bool Missing => Config is null && Error is null;

    /// <summary>Reads and parses <paramref name="path"/>, retrying a file another process holds (an editor saving it).</summary>
    public static TraceConfigLoad From(string path, string moduleName)
    {
        for (int attempt = 0; ; attempt++)
        {
            string text;
            try
            {
                if (!File.Exists(path))
                {
                    return default;
                }

                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
                text = reader.ReadToEnd();
            }
            catch (FileNotFoundException)
            {
                return default;
            }
            catch (DirectoryNotFoundException)
            {
                return default;
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                if (attempt >= Retries)
                {
                    return new TraceConfigLoad(null, exception.Message);
                }

                Thread.Sleep(RetryDelayMilliseconds);
                continue;
            }

            try
            {
                return new TraceConfigLoad(TraceConfig.Parse(text, moduleName), null);
            }
            catch (Exception exception) when (exception is JsonException or FormatException or ArgumentException)
            {
                return new TraceConfigLoad(null, exception.Message);
            }
        }
    }
}

/// <summary>
/// Watches one config file (its directory, filtered to the file name) and calls back once per burst of changes, after
/// <see cref="DebounceMilliseconds"/> of quiet, on a thread-pool thread. Creation, edits, renames and deletion all count.
/// </summary>
internal sealed class TraceConfigWatcher : IDisposable
{
    public const int DebounceMilliseconds = 150;

    private readonly FileSystemWatcher _watcher;
    private readonly Timer _timer;
    private int _disposed;

    public TraceConfigWatcher(string path, Action<TraceConfigWatcher> changed)
    {
        string directory = Path.GetDirectoryName(Path.GetFullPath(path)) ?? ".";
        _timer = new Timer(_ =>
        {
            if (Volatile.Read(ref _disposed) == 0)
            {
                changed(this);
            }
        });
        _watcher = new FileSystemWatcher(directory, Path.GetFileName(path))
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime,
            IncludeSubdirectories = false,
        };
        _watcher.Changed += (_, _) => Schedule();
        _watcher.Created += (_, _) => Schedule();
        _watcher.Deleted += (_, _) => Schedule();
        _watcher.Renamed += (_, _) => Schedule();
        _watcher.Error += (_, _) => Schedule();
        _watcher.EnableRaisingEvents = true;
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        _watcher.Dispose();
        _timer.Dispose();
    }

    private void Schedule()
    {
        try
        {
            _timer.Change(DebounceMilliseconds, Timeout.Infinite);
        }
        catch (ObjectDisposedException)
        {
        }
    }
}
