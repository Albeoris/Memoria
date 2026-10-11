using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Recompiler.Runtime;

/// <summary>
/// Everything <see cref="Tracer"/> needs beyond its gate: the config lookup and watcher, the session, the log writer, the
/// call-site cache and the per-thread line buffers. This class has a static constructor (its field initializers), so only
/// the tracer's slow path touches it, and only after <see cref="ModuleLifetime.RuntimeStarted"/> said the module's start-up
/// ran; <see cref="Tracer"/> itself stays without one.
/// </summary>
internal static partial class TracerState
{
    private const int StateUnknown = 0;
    private const int StateOffForGood = 1;
    private const int StateConfigured = 2;
    private const int MaxCachedSites = 1 << 16;

    private static readonly Lock SyncRoot = new();
    private static readonly ConcurrentDictionary<object, TraceSite> Sites = new(ReferenceEqualityComparer.Instance);
    private static readonly HashSet<string> OpenedLogs = new(StringComparer.OrdinalIgnoreCase);

    // 0: unknown, 1: off for good (no config file at first access), 2: configured (file watched).
    private static int _state;
    private static TraceSession? _session;
    private static TraceLogWriter? _writer;
    private static TraceConfigWatcher? _watcher;
    private static string? _configPath;
    private static string? _lastLogPath;
    private static string? _moduleName;
    private static bool _configApplied;
    private static int _generation;
    private static bool _testMode;
    private static string? _testConfigPath;
    private static int _configLookups;
    private static int _configLoads;

    [ThreadStatic]
    private static StringBuilder? _line;

    [ThreadStatic]
    private static char[]? _chars;

    [ThreadStatic]
    private static bool _busy;

    [ThreadStatic]
    private static int _threadId;

    /// <summary>The current config traces something; resolves first access.</summary>
    internal static bool IsActive
    {
        get
        {
            if (Volatile.Read(ref _state) == StateUnknown)
            {
                Initialize();
            }

            return Tracer.Gate == Tracer.GateOn;
        }
    }

    /// <summary>Config lookups since start (or the last <see cref="ResetForTests"/>): 1 at most, at first access.</summary>
    internal static int ConfigLookups => Volatile.Read(ref _configLookups);

    /// <summary>Config file reads applied (first load and every reload, valid or not).</summary>
    internal static int ConfigLoads => Volatile.Read(ref _configLoads);

    /// <summary>The config file in use; null before first access or without one.</summary>
    internal static string? ConfigPath => Volatile.Read(ref _configPath);

    /// <summary>See <see cref="Tracer.ResetForTests"/>.</summary>
    internal static void ResetForTests(string? configPath, string moduleName)
    {
        lock (SyncRoot)
        {
            _watcher?.Dispose();
            _watcher = null;
            _writer?.Dispose();
            _writer = null;
            Volatile.Write(ref _session, null);
            Volatile.Write(ref _configPath, null);
            _lastLogPath = null;
            _configApplied = false;
            OpenedLogs.Clear();
            Sites.Clear();
            _testMode = configPath is not null;
            _testConfigPath = configPath;
            _moduleName = moduleName;
            Volatile.Write(ref _configLookups, 0);
            Volatile.Write(ref _configLoads, 0);
            TraceBreaker.UseTestDebugger(attached: false);
            Volatile.Write(ref _state, StateUnknown);
            Tracer.SetGate(Tracer.GateUnknown);
        }
    }

    /// <summary>Ends the call <see cref="TryBegin"/> started on this thread.</summary>
    internal static void EndCall() => _busy = false;

    /// <summary>
    /// Resolves first access, the call site and the selection and caps; on success starts the line
    /// (<c>time tN module!name(</c>) in the thread's builder and marks the thread busy (<see cref="EndCall"/> clears it).
    /// The caller checked <see cref="ModuleLifetime.RuntimeStarted"/> first.
    /// </summary>
    internal static bool TryBegin(string function, out TraceCall call)
    {
        call = default;
        if (_busy)
        {
            return false;
        }

        try
        {
            if (Volatile.Read(ref _state) == StateUnknown)
            {
                Initialize();
            }

            TraceSession? session = Volatile.Read(ref _session);
            if (session is null)
            {
                return false;
            }

            TraceSite site = SiteOf(function, session.ModuleName);
            TraceSite.Status status = site.StatusFor(session);
            if (!status.Selected)
            {
                return false;
            }

            TraceConfig config = session.Config;
            if (config.CapPerFunction > 0)
            {
                long count = Interlocked.Increment(ref status.Count);
                if (count > config.CapPerFunction)
                {
                    if (count == config.CapPerFunction + 1)
                    {
                        session.Writer.WriteLine(string.Create(CultureInfo.InvariantCulture,
                            $"# cap perFunction {config.CapPerFunction} reached: {site.QualifiedName}"));
                    }

                    return false;
                }
            }

            if (config.CapTotal > 0)
            {
                long total = Interlocked.Increment(ref session.Total);
                if (total > config.CapTotal)
                {
                    if (total == config.CapTotal + 1)
                    {
                        session.Writer.WriteLine(string.Create(CultureInfo.InvariantCulture,
                            $"# cap total {config.CapTotal} reached; tracing stopped"));
                        StopIfCurrent(session);
                    }

                    return false;
                }
            }

            StringBuilder line = _line ??= new StringBuilder(256);
            line.Clear();
            if (config.Timestamps)
            {
                Span<char> time = stackalloc char[16];
                if (DateTime.UtcNow.TryFormat(time, out int written, "HH:mm:ss.fff", CultureInfo.InvariantCulture))
                {
                    line.Append(time[..written]).Append(' ');
                }
            }

            if (config.Threads)
            {
                Span<char> id = stackalloc char[12];
                if (CurrentThreadId().TryFormat(id, out int written, default, CultureInfo.InvariantCulture))
                {
                    line.Append('t').Append(id[..written]).Append(' ');
                }
            }

            line.Append(site.QualifiedName);
            if (config.Args)
            {
                line.Append('(');
            }

            _busy = true;
            call = new TraceCall(line, site, session);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    internal static void Arg<T>(in TraceCall call, int index, T value)
    {
        StringBuilder line = call.Line;
        if (index > 0)
        {
            line.Append(", ");
        }

        line.Append(call.Site.ParameterName(index)).Append('=');
        TraceArg.Append(line, value);
    }

    /// <summary>Closes the line, writes it and breaks when it matches <c>break.match</c>.</summary>
    internal static void Commit(in TraceCall call)
    {
        StringBuilder line = call.Line;
        TraceSession session = call.Session;
        TraceConfig config = session.Config;
        if (config.Args)
        {
            line.Append(')');
        }

        int length = line.Length;
        char[] chars = _chars is { } buffer && buffer.Length >= length ? buffer : (_chars = new char[Math.Max(256, length * 2)]);
        line.CopyTo(0, chars, 0, length);
        ReadOnlySpan<char> text = chars.AsSpan(0, length);
        if (!session.Writer.WriteLine(text))
        {
            StopIfCurrent(session);
            return;
        }

        if (config.BreakMatch is not { } match)
        {
            return;
        }

        bool hit = config.BreakPattern is { } pattern ? pattern.IsMatch(text) : text.Contains(match, StringComparison.Ordinal);
        if (!hit)
        {
            return;
        }

        int number = Interlocked.Increment(ref session.BreakHits);
        if (config.BreakCount <= 0 || number <= config.BreakCount)
        {
            TraceBreaker.Break(session.Writer, number, text.ToString(), config.BreakWait);
        }
    }

    private static TraceSite SiteOf(string function, string moduleName)
    {
        if (Sites.TryGetValue(function, out TraceSite? site))
        {
            return site;
        }

        site = TraceSite.Parse(function, moduleName);
        if (Sites.Count < MaxCachedSites)
        {
            site = Sites.GetOrAdd(function, site);
        }

        return site;
    }

    private static int CurrentThreadId()
    {
        int id = _threadId;
        if (id == 0)
        {
            id = OperatingSystem.IsWindows() ? (int)GetCurrentThreadId() : Environment.CurrentManagedThreadId;
            _threadId = id;
        }

        return id;
    }

    /// <summary>First access: finds the config once; none means off for good, one is loaded and watched.</summary>
    private static void Initialize()
    {
        lock (SyncRoot)
        {
            if (_state != StateUnknown)
            {
                return;
            }

            try
            {
                string module = _moduleName ??= ResolveModuleName();
                Interlocked.Increment(ref _configLookups);
                string? path = FindConfig(module);
                if (path is null)
                {
                    Tracer.SetGate(Tracer.GateOff);
                    Volatile.Write(ref _state, StateOffForGood);
                    return;
                }

                Volatile.Write(ref _configPath, path);
                ApplyLocked(TraceConfigLoad.From(path, module), reload: false);
                Volatile.Write(ref _state, StateConfigured);
                try
                {
                    _watcher = new TraceConfigWatcher(path, OnConfigChanged);
                }
                catch (Exception)
                {
                    // No hot reload (directory not watchable); the config read now stays in force.
                }
            }
            catch (Exception)
            {
                Volatile.Write(ref _session, null);
                Tracer.SetGate(Tracer.GateOff);
                Volatile.Write(ref _state, StateOffForGood);
            }
        }
    }

    private static string? FindConfig(string module)
    {
        if (!_testMode)
        {
            return TraceConfigLocator.Find(Environment.GetEnvironmentVariable(Tracer.EnvironmentVariable), ModuleLifetime.ModuleDirectory, module);
        }

        string path = _testConfigPath!;
        if (Directory.Exists(path))
        {
            return TraceConfigLocator.Find(null, path, module);
        }

        return File.Exists(path) ? Path.GetFullPath(path) : null;
    }

    private static string ResolveModuleName()
    {
        string name = Path.GetFileNameWithoutExtension(ModuleLifetime.ModulePath);
        if (string.IsNullOrEmpty(name))
        {
            name = Path.GetFileNameWithoutExtension(Environment.ProcessPath) ?? string.Empty;
        }

        return name.Length == 0 ? "module" : name;
    }

    private static void OnConfigChanged(TraceConfigWatcher watcher)
    {
        try
        {
            string? path;
            string? module;
            lock (SyncRoot)
            {
                if (!ReferenceEquals(watcher, _watcher))
                {
                    return;
                }

                path = _configPath;
                module = _moduleName;
            }

            if (path is null || module is null)
            {
                return;
            }

            TraceConfigLoad load = TraceConfigLoad.From(path, module);
            lock (SyncRoot)
            {
                if (ReferenceEquals(watcher, _watcher))
                {
                    ApplyLocked(load, reload: true);
                }
            }
        }
        catch (Exception)
        {
            // A timer callback must never throw: that would end the process.
        }
    }

    /// <summary>Makes <paramref name="load"/> the current state: a new session, off, or an error note keeping the previous one.</summary>
    private static void ApplyLocked(TraceConfigLoad load, bool reload)
    {
        Interlocked.Increment(ref _configLoads);
        string module = _moduleName!;
        string directory = Path.GetDirectoryName(_configPath!) ?? ".";
        if (load.Missing)
        {
            DisableLocked("config file deleted");
            return;
        }

        if (load.Error is { } error)
        {
            string kept = _configApplied ? "previous config kept" : "tracing stays off until the file parses";
            NoteLocked($"# config error: {error.ReplaceLineEndings(" ")} ({kept})", directory, module);
            return;
        }

        TraceConfig config = load.Config!;
        _configApplied = true;
        if (!config.Enabled)
        {
            DisableLocked("\"enabled\": false");
            return;
        }

        string logPath = config.LogPath(directory, module);
        _lastLogPath = logPath;
        if (_writer is null || _writer.IsDead || !string.Equals(_writer.Path, logPath, StringComparison.OrdinalIgnoreCase))
        {
            _writer?.Dispose();
            bool first = OpenedLogs.Add(logPath);
            _writer = TraceLogWriter.Open(logPath, truncate: first && !config.Append);
            if (_writer is null)
            {
                Volatile.Write(ref _session, null);
                Tracer.SetGate(Tracer.GateOff);
                return;
            }

            _writer.WriteLine(string.Create(CultureInfo.InvariantCulture,
                $"# recomp trace {module} pid={Environment.ProcessId} config={_configPath} started={DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}"));
        }
        else if (reload)
        {
            _writer.WriteLine("# config reloaded");
        }

        Volatile.Write(ref _session, new TraceSession(config, _writer, ++_generation, module));
        Tracer.SetGate(Tracer.GateOn);
    }

    private static void DisableLocked(string reason)
    {
        Tracer.SetGate(Tracer.GateOff);
        Volatile.Write(ref _session, null);
        if (_writer is not null)
        {
            _writer.WriteLine("# tracing off: " + reason);
            _writer.Dispose();
            _writer = null;
        }
    }

    /// <summary>Writes a note to the open log, or appends it to the last (or default) log when tracing is off.</summary>
    private static void NoteLocked(string text, string directory, string module)
    {
        if (_writer is { IsDead: false } writer)
        {
            writer.WriteLine(text);
            return;
        }

        using TraceLogWriter? transient = TraceLogWriter.Open(_lastLogPath ?? Path.Combine(directory, Tracer.DefaultLogFileNameFor(module)), truncate: false);
        transient?.WriteLine(text);
    }

    private static void StopIfCurrent(TraceSession session)
    {
        lock (SyncRoot)
        {
            if (ReferenceEquals(Volatile.Read(ref _session), session))
            {
                Tracer.SetGate(Tracer.GateOff);
            }
        }
    }
    [LibraryImport("kernel32.dll")]
    private static partial uint GetCurrentThreadId();
}

/// <summary>A line being built on the calling thread.</summary>
internal readonly struct TraceCall(StringBuilder line, TraceSite site, TraceSession session)
{
    public StringBuilder Line { get; } = line;

    public TraceSite Site { get; } = site;

    public TraceSession Session { get; } = session;

    public bool Args => Session.Config.Args;
}
