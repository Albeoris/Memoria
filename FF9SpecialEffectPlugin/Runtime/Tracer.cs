using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Recompiler.Runtime;

/// <summary>
/// Function tracing for rewritten code. Each rewritten function starts with one tool-owned line,
/// <c>Tracer.Enter("tm_add(a, b)", a, b);</c>, that logs <c>TinyMath!tm_add(a=2, b=3)</c> when a config file next to the
/// module selects it. The calls exist only in builds that define <see cref="ConditionSymbol"/> (Debug, or
/// <c>-p:RecompTrace=true</c>); compiled in but off, a call costs one static load and compare.
/// <para>
/// The first call looks for the config (<see cref="EnvironmentVariable"/>, then <c>&lt;moduleDir&gt;/&lt;module&gt;.trace.json</c>,
/// then <c>&lt;moduleDir&gt;/</c><see cref="SharedConfigFileName"/>). Without one tracing stays off for the process lifetime
/// and no file is checked again; with one it is loaded and watched: edits apply after a short debounce, invalid JSON keeps
/// the previous config, deleting the file turns tracing off and recreating it turns it back on.
/// </para>
/// <para>
/// Before any of that, every call of the slow path checks <see cref="ModuleLifetime.RuntimeStarted"/> (one static field
/// read). An executable loaded as a library by another process (the <c>recomp verify</c> host) never ran its NativeAOT
/// start-up: its imports are unbound and no class constructor can run in it, so the call returns without touching
/// anything else and the gate stays as it is. A DLL (started on the first call into it), or the executable running as the
/// program, goes on to the config lookup. An executable replacement therefore traces only as the running program.
/// </para>
/// <para>
/// This class has no static constructor (only default-valued fields; a test checks it): the inlined gate may run in a
/// module whose start-up never ran. Everything that needs initialization lives in <see cref="TracerState"/>, which only
/// the slow path touches after the image check.
/// </para>
/// </summary>
public static class Tracer
{
    /// <summary>The compilation symbol the <see cref="Enter(string)"/> overloads are conditional on.</summary>
    public const string ConditionSymbol = "RECOMP_TRACE";

    /// <summary>An explicit config file path; the highest-precedence lookup.</summary>
    public const string EnvironmentVariable = "RECOMP_TRACE_CONFIG";

    /// <summary>A config next to the module shared by several modules; its <c>modules</c> section holds per-module keys.</summary>
    public const string SharedConfigFileName = "recomp.trace.json";

    internal const int GateUnknown = 0;
    internal const int GateOff = 1;
    internal const int GateOn = 2;

    // The fast path reads only _gate. 0: first access pending, 1: off (no config, disabled, cap reached), 2: on. Written
    // only by TracerState, after the start-up check.
    private static int _gate;

    /// <summary>True when the trace calls are compiled into this module and the current config traces something.</summary>
    public static bool IsEnabled
    {
        get
        {
#if RECOMP_TRACE
            return IsActive;
#else
            return false;
#endif
        }
    }

    /// <summary>The current config traces something (first access resolved), regardless of how this file was compiled.</summary>
    internal static bool IsActive => ModuleLifetime.RuntimeStarted && TracerState.IsActive;

    /// <summary>Config lookups since start (or the last <see cref="ResetForTests"/>): 1 at most, at first access.</summary>
    internal static int ConfigLookups => TracerState.ConfigLookups;

    /// <summary>Config file reads applied (first load and every reload, valid or not).</summary>
    internal static int ConfigLoads => TracerState.ConfigLoads;

    /// <summary>The config file in use; null before first access or without one.</summary>
    internal static string? ConfigPath => TracerState.ConfigPath;

    /// <summary>The gate the fast path reads (<see cref="GateUnknown"/>, <see cref="GateOff"/>, <see cref="GateOn"/>).</summary>
    internal static int Gate => Volatile.Read(ref _gate);

    /// <summary>The per-module config file name: <c>&lt;module&gt;.trace.json</c>.</summary>
    public static string ConfigFileNameFor(string moduleName) => moduleName + ".trace.json";

    /// <summary>The default log file name, in the config file's directory: <c>&lt;module&gt;.trace.log</c>.</summary>
    public static string DefaultLogFileNameFor(string moduleName) => moduleName + ".trace.log";

    /// <summary>Logs a call of <paramref name="function"/> (<c>"name(p1, p2)"</c> or <c>"name"</c>) without arguments.</summary>
    [Conditional(ConditionSymbol)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter(string function)
    {
        if (_gate != GateOff)
        {
            Slow(function);
        }
    }

    /// <summary>Logs a call of <paramref name="function"/> with one argument.</summary>
    [Conditional(ConditionSymbol)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter<T1>(string function, T1 a1)
    {
        if (_gate != GateOff)
        {
            Slow(function, a1);
        }
    }

    /// <summary>Logs a call of <paramref name="function"/> with two arguments.</summary>
    [Conditional(ConditionSymbol)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter<T1, T2>(string function, T1 a1, T2 a2)
    {
        if (_gate != GateOff)
        {
            Slow(function, a1, a2);
        }
    }

    /// <summary>Logs a call of <paramref name="function"/> with three arguments.</summary>
    [Conditional(ConditionSymbol)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter<T1, T2, T3>(string function, T1 a1, T2 a2, T3 a3)
    {
        if (_gate != GateOff)
        {
            Slow(function, a1, a2, a3);
        }
    }

    /// <summary>Logs a call of <paramref name="function"/> with four arguments.</summary>
    [Conditional(ConditionSymbol)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter<T1, T2, T3, T4>(string function, T1 a1, T2 a2, T3 a3, T4 a4)
    {
        if (_gate != GateOff)
        {
            Slow(function, a1, a2, a3, a4);
        }
    }

    /// <summary>Logs a call of <paramref name="function"/> with five arguments.</summary>
    [Conditional(ConditionSymbol)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter<T1, T2, T3, T4, T5>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
    {
        if (_gate != GateOff)
        {
            Slow(function, a1, a2, a3, a4, a5);
        }
    }

    /// <summary>Logs a call of <paramref name="function"/> with six arguments.</summary>
    [Conditional(ConditionSymbol)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter<T1, T2, T3, T4, T5, T6>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
    {
        if (_gate != GateOff)
        {
            Slow(function, a1, a2, a3, a4, a5, a6);
        }
    }

    /// <summary>Logs a call of <paramref name="function"/> with seven arguments.</summary>
    [Conditional(ConditionSymbol)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter<T1, T2, T3, T4, T5, T6, T7>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
    {
        if (_gate != GateOff)
        {
            Slow(function, a1, a2, a3, a4, a5, a6, a7);
        }
    }

    /// <summary>Logs a call of <paramref name="function"/> with eight arguments.</summary>
    [Conditional(ConditionSymbol)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter<T1, T2, T3, T4, T5, T6, T7, T8>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
    {
        if (_gate != GateOff)
        {
            Slow(function, a1, a2, a3, a4, a5, a6, a7, a8);
        }
    }

    /// <summary>Logs a call of <paramref name="function"/> with nine arguments.</summary>
    [Conditional(ConditionSymbol)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8,
        T9 a9)
    {
        if (_gate != GateOff)
        {
            Slow(function, a1, a2, a3, a4, a5, a6, a7, a8, a9);
        }
    }

    /// <summary>Logs a call of <paramref name="function"/> with ten arguments (the instrumenter traces the first ten parameters).</summary>
    [Conditional(ConditionSymbol)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7,
        T8 a8, T9 a9, T10 a10)
    {
        if (_gate != GateOff)
        {
            Slow(function, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
        }
    }

    /// <summary>
    /// Test hook: forgets all state (closing the log, stopping the watcher) so the next call runs first-access logic again,
    /// with <paramref name="moduleName"/> as the module and a debugger stand-in that is never attached and never breaks.
    /// <paramref name="configPath"/> is the config file to look for (it may not exist), or a directory searched like the
    /// module directory (<c>&lt;module&gt;.trace.json</c>, then <see cref="SharedConfigFileName"/>), or null for the normal
    /// lookup.
    /// </summary>
    internal static void ResetForTests(string? configPath, string moduleName) => TracerState.ResetForTests(configPath, moduleName);

    /// <summary>Sets the gate the fast path reads; <see cref="TracerState"/> calls it under its lock.</summary>
    internal static void SetGate(int gate) => Volatile.Write(ref _gate, gate);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Slow(string function)
    {
        if (!ModuleLifetime.RuntimeStarted || !TracerState.TryBegin(function, out TraceCall call))
        {
            return;
        }

        try
        {
            TracerState.Commit(call);
        }
        catch (Exception)
        {
            // Tracing never breaks the traced program.
        }
        finally
        {
            TracerState.EndCall();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Slow<T1>(string function, T1 a1)
    {
        if (!ModuleLifetime.RuntimeStarted || !TracerState.TryBegin(function, out TraceCall call))
        {
            return;
        }

        try
        {
            if (call.Args)
            {
                TracerState.Arg(call, 0, a1);
            }

            TracerState.Commit(call);
        }
        catch (Exception)
        {
            // Tracing never breaks the traced program.
        }
        finally
        {
            TracerState.EndCall();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Slow<T1, T2>(string function, T1 a1, T2 a2)
    {
        if (!ModuleLifetime.RuntimeStarted || !TracerState.TryBegin(function, out TraceCall call))
        {
            return;
        }

        try
        {
            if (call.Args)
            {
                TracerState.Arg(call, 0, a1);
                TracerState.Arg(call, 1, a2);
            }

            TracerState.Commit(call);
        }
        catch (Exception)
        {
            // Tracing never breaks the traced program.
        }
        finally
        {
            TracerState.EndCall();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Slow<T1, T2, T3>(string function, T1 a1, T2 a2, T3 a3)
    {
        if (!ModuleLifetime.RuntimeStarted || !TracerState.TryBegin(function, out TraceCall call))
        {
            return;
        }

        try
        {
            if (call.Args)
            {
                TracerState.Arg(call, 0, a1);
                TracerState.Arg(call, 1, a2);
                TracerState.Arg(call, 2, a3);
            }

            TracerState.Commit(call);
        }
        catch (Exception)
        {
            // Tracing never breaks the traced program.
        }
        finally
        {
            TracerState.EndCall();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Slow<T1, T2, T3, T4>(string function, T1 a1, T2 a2, T3 a3, T4 a4)
    {
        if (!ModuleLifetime.RuntimeStarted || !TracerState.TryBegin(function, out TraceCall call))
        {
            return;
        }

        try
        {
            if (call.Args)
            {
                TracerState.Arg(call, 0, a1);
                TracerState.Arg(call, 1, a2);
                TracerState.Arg(call, 2, a3);
                TracerState.Arg(call, 3, a4);
            }

            TracerState.Commit(call);
        }
        catch (Exception)
        {
            // Tracing never breaks the traced program.
        }
        finally
        {
            TracerState.EndCall();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Slow<T1, T2, T3, T4, T5>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
    {
        if (!ModuleLifetime.RuntimeStarted || !TracerState.TryBegin(function, out TraceCall call))
        {
            return;
        }

        try
        {
            if (call.Args)
            {
                TracerState.Arg(call, 0, a1);
                TracerState.Arg(call, 1, a2);
                TracerState.Arg(call, 2, a3);
                TracerState.Arg(call, 3, a4);
                TracerState.Arg(call, 4, a5);
            }

            TracerState.Commit(call);
        }
        catch (Exception)
        {
            // Tracing never breaks the traced program.
        }
        finally
        {
            TracerState.EndCall();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Slow<T1, T2, T3, T4, T5, T6>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
    {
        if (!ModuleLifetime.RuntimeStarted || !TracerState.TryBegin(function, out TraceCall call))
        {
            return;
        }

        try
        {
            if (call.Args)
            {
                TracerState.Arg(call, 0, a1);
                TracerState.Arg(call, 1, a2);
                TracerState.Arg(call, 2, a3);
                TracerState.Arg(call, 3, a4);
                TracerState.Arg(call, 4, a5);
                TracerState.Arg(call, 5, a6);
            }

            TracerState.Commit(call);
        }
        catch (Exception)
        {
            // Tracing never breaks the traced program.
        }
        finally
        {
            TracerState.EndCall();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Slow<T1, T2, T3, T4, T5, T6, T7>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
    {
        if (!ModuleLifetime.RuntimeStarted || !TracerState.TryBegin(function, out TraceCall call))
        {
            return;
        }

        try
        {
            if (call.Args)
            {
                TracerState.Arg(call, 0, a1);
                TracerState.Arg(call, 1, a2);
                TracerState.Arg(call, 2, a3);
                TracerState.Arg(call, 3, a4);
                TracerState.Arg(call, 4, a5);
                TracerState.Arg(call, 5, a6);
                TracerState.Arg(call, 6, a7);
            }

            TracerState.Commit(call);
        }
        catch (Exception)
        {
            // Tracing never breaks the traced program.
        }
        finally
        {
            TracerState.EndCall();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Slow<T1, T2, T3, T4, T5, T6, T7, T8>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
    {
        if (!ModuleLifetime.RuntimeStarted || !TracerState.TryBegin(function, out TraceCall call))
        {
            return;
        }

        try
        {
            if (call.Args)
            {
                TracerState.Arg(call, 0, a1);
                TracerState.Arg(call, 1, a2);
                TracerState.Arg(call, 2, a3);
                TracerState.Arg(call, 3, a4);
                TracerState.Arg(call, 4, a5);
                TracerState.Arg(call, 5, a6);
                TracerState.Arg(call, 6, a7);
                TracerState.Arg(call, 7, a8);
            }

            TracerState.Commit(call);
        }
        catch (Exception)
        {
            // Tracing never breaks the traced program.
        }
        finally
        {
            TracerState.EndCall();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Slow<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8,
        T9 a9)
    {
        if (!ModuleLifetime.RuntimeStarted || !TracerState.TryBegin(function, out TraceCall call))
        {
            return;
        }

        try
        {
            if (call.Args)
            {
                TracerState.Arg(call, 0, a1);
                TracerState.Arg(call, 1, a2);
                TracerState.Arg(call, 2, a3);
                TracerState.Arg(call, 3, a4);
                TracerState.Arg(call, 4, a5);
                TracerState.Arg(call, 5, a6);
                TracerState.Arg(call, 6, a7);
                TracerState.Arg(call, 7, a8);
                TracerState.Arg(call, 8, a9);
            }

            TracerState.Commit(call);
        }
        catch (Exception)
        {
            // Tracing never breaks the traced program.
        }
        finally
        {
            TracerState.EndCall();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Slow<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7,
        T8 a8, T9 a9, T10 a10)
    {
        if (!ModuleLifetime.RuntimeStarted || !TracerState.TryBegin(function, out TraceCall call))
        {
            return;
        }

        try
        {
            if (call.Args)
            {
                TracerState.Arg(call, 0, a1);
                TracerState.Arg(call, 1, a2);
                TracerState.Arg(call, 2, a3);
                TracerState.Arg(call, 3, a4);
                TracerState.Arg(call, 4, a5);
                TracerState.Arg(call, 5, a6);
                TracerState.Arg(call, 6, a7);
                TracerState.Arg(call, 7, a8);
                TracerState.Arg(call, 8, a9);
                TracerState.Arg(call, 9, a10);
            }

            TracerState.Commit(call);
        }
        catch (Exception)
        {
            // Tracing never breaks the traced program.
        }
        finally
        {
            TracerState.EndCall();
        }
    }
}


/// <summary>
/// One call site: its literal <c>"name(p1, p2)"</c> parsed once (cached by string reference) into the function name and
/// parameter names; a literal without parentheses has no names and its values print as <c>a1=</c>, <c>a2=</c>.
/// Holds the selection and the per-function count for the current config generation.
/// </summary>
internal sealed class TraceSite
{
    private static readonly string[] Positional = ["a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "a10"];

    private readonly string[]? _parameters;
    private Status? _status;

    private TraceSite(string name, string[]? parameters, string qualifiedName)
    {
        Name = name;
        _parameters = parameters;
        QualifiedName = qualifiedName;
    }

    public string Name { get; }

    /// <summary><c>module!name</c>, the form every log line carries.</summary>
    public string QualifiedName { get; }

    /// <summary>The parameter names from the literal; null when it had no parentheses.</summary>
    public IReadOnlyList<string>? Parameters => _parameters;

    public static TraceSite Parse(string literal, string moduleName)
    {
        string text = literal.Trim();
        int open = text.IndexOf('(', StringComparison.Ordinal);
        if (open < 0)
        {
            return new TraceSite(text, null, moduleName + "!" + text);
        }

        string name = text[..open].TrimEnd();
        int close = text.LastIndexOf(')');
        string inner = close > open ? text[(open + 1)..close] : text[(open + 1)..];
        var parameters = new List<string>();
        foreach (string part in inner.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            if (part is not ("…" or "..."))
            {
                parameters.Add(part);
            }
        }

        return new TraceSite(name, [.. parameters], moduleName + "!" + name);
    }

    /// <summary>The name printed before the value at <paramref name="index"/>: from the literal, else positional.</summary>
    public string ParameterName(int index)
    {
        if (_parameters is { } names && index < names.Length)
        {
            return names[index];
        }

        return index < Positional.Length ? Positional[index] : "a" + (index + 1).ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>The selection and count for <paramref name="session"/>, recomputed once per config generation.</summary>
    public Status StatusFor(TraceSession session)
    {
        Status? current = Volatile.Read(ref _status);
        if (current is not null && current.Generation == session.Generation)
        {
            return current;
        }

        var fresh = new Status(session.Generation, session.Config.Selects(Name, QualifiedName));
        Status? raced = Interlocked.CompareExchange(ref _status, fresh, current);
        return ReferenceEquals(raced, current) || raced is null || raced.Generation != session.Generation ? fresh : raced;
    }

    /// <summary>Per-generation state of a site; <see cref="Count"/> is updated with <see cref="Interlocked"/>.</summary>
    internal sealed class Status(int generation, bool selected)
    {
        public long Count;

        public int Generation { get; } = generation;

        public bool Selected { get; } = selected;
    }
}

/// <summary>The applied config with its log writer and the counters that restart with every config generation.</summary>
internal sealed class TraceSession(TraceConfig config, TraceLogWriter writer, int generation, string moduleName)
{
    public long Total;
    public int BreakHits;

    public TraceConfig Config { get; } = config;

    public TraceLogWriter Writer { get; } = writer;

    public int Generation { get; } = generation;

    public string ModuleName { get; } = moduleName;
}

/// <summary>
/// The break after a matching line. Attached means <see cref="Debugger.IsAttached"/> or <c>IsDebuggerPresent()</c> (a native
/// Win32 debugger such as <c>recomp debug session</c>). Attached: the log note, then on this thread
/// <c>OutputDebugStringW("[recomp-trace] break #n: line")</c> immediately followed by the breakpoint (native <c>DebugBreak</c>
/// under a native debugger, else <see cref="Debugger.Break"/>). Not attached: the debug string, then a wait of up to
/// <c>break.wait</c> seconds for a debugger (polled every 100 ms), else a "skipped" note. Tests swap in a stand-in that never breaks.
/// </summary>
internal static partial class TraceBreaker
{
    public const string DebugOutputPrefix = "[recomp-trace] ";
    public const int PollMilliseconds = 100;

    private static bool _testMode;
    private static bool _testAttached;
    private static int _testBreaks;
    private static string? _lastOutput;

    /// <summary>Breakpoints the test stand-in took.</summary>
    internal static int TestBreaks => Volatile.Read(ref _testBreaks);

    /// <summary>The last debug output (test stand-in only).</summary>
    internal static string? LastOutput => Volatile.Read(ref _lastOutput);

    /// <summary>Replaces the debugger with a stand-in: attached or not, it counts breakpoints instead of taking them.</summary>
    internal static void UseTestDebugger(bool attached)
    {
        _testMode = true;
        _testAttached = attached;
        Volatile.Write(ref _testBreaks, 0);
        Volatile.Write(ref _lastOutput, null);
    }

    public static void Break(TraceLogWriter writer, int number, string line, double waitSeconds)
    {
        string hit = number.ToString(CultureInfo.InvariantCulture);
        string output = DebugOutputPrefix + "break #" + hit + ": " + line;
        string note = "# break #" + hit + " after: " + line + "   (";
        if (IsAttached())
        {
            writer.WriteLine(note + "debugger attached)");
            BreakHere(output);
            return;
        }

        Output(output);
        if (waitSeconds <= 0)
        {
            writer.WriteLine(note + "no debugger attached: skipped)");
            return;
        }

        string seconds = waitSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        long start = Environment.TickCount64;
        long deadline = start + (long)(waitSeconds * 1000);
        while (Environment.TickCount64 < deadline)
        {
            Thread.Sleep(PollMilliseconds);
            if (IsAttached())
            {
                double waited = (Environment.TickCount64 - start) / 1000.0;
                writer.WriteLine(note + "debugger attached after " + waited.ToString("0.#", CultureInfo.InvariantCulture) + " s)");
                BreakHere(output);
                return;
            }
        }

        writer.WriteLine(note + "waited " + seconds + " s, none attached: skipped)");
    }

    /// <summary>The debug string on this thread immediately before the breakpoint: the debugger labels the stop with it.</summary>
    private static void BreakHere(string output)
    {
        Output(output);
        Trigger();
    }

    private static bool IsAttached()
    {
        if (_testMode)
        {
            return _testAttached;
        }

        return Debugger.IsAttached || (OperatingSystem.IsWindows() && IsDebuggerPresent());
    }

    private static void Output(string text)
    {
        if (_testMode)
        {
            Volatile.Write(ref _lastOutput, text);
            return;
        }

        if (OperatingSystem.IsWindows())
        {
            OutputDebugStringW(text);
        }
    }

    private static void Trigger()
    {
        if (_testMode)
        {
            Interlocked.Increment(ref _testBreaks);
            return;
        }

        if (OperatingSystem.IsWindows() && IsDebuggerPresent())
        {
            DebugBreak();
        }
        else
        {
            Debugger.Break();
        }
    }

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial void OutputDebugStringW(string text);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsDebuggerPresent();

    [LibraryImport("kernel32.dll")]
    private static partial void DebugBreak();
}
