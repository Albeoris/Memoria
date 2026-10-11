using System.Diagnostics;

namespace Recompiler.Runtime;

/// <summary>The real clock (the default of <see cref="Ports.Clock"/>): system time, the tick count and the performance counter.</summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public long TickCount => Environment.TickCount64;
    public long HighResolutionTicks => Stopwatch.GetTimestamp();
    public long HighResolutionFrequency => Stopwatch.Frequency;

    /// <summary>Blocks the calling thread: <c>Sleep(INFINITE)</c> forever, <c>Sleep(0)</c> gives up the rest of the time slice.</summary>
    public void Sleep(uint milliseconds) => Block(milliseconds);

    /// <summary><c>Sleep</c> on the real thread (<c>INFINITE</c>, 0xFFFFFFFF, waits forever; longer finite waits are capped at <see cref="int.MaxValue"/> ms).</summary>
    internal static void Block(uint milliseconds) =>
        Thread.Sleep(milliseconds == uint.MaxValue ? Timeout.Infinite : (int)Math.Min(milliseconds, int.MaxValue));
}

/// <summary>
/// Deterministic time for tests and the function oracle: starts at a given instant and advances by <see cref="Quantum"/>
/// on every read of a time value (UtcNow, TickCount, HighResolutionTicks), so a busy-wait loop (<c>while (GetTickCount() - start &lt; 1000)</c>, an <c>rdtsc</c>
/// calibration) terminates after a predictable number of iterations, and two runs read the same sequence. Each read
/// returns the current value, then advances: the first <see cref="UtcNow"/> equals the start instant. <see cref="Sleep"/>
/// never blocks; it advances the time by the requested milliseconds.
/// </summary>
public sealed class DeterministicClock : IClock
{
    /// <summary>One millisecond: a 1000 ms busy wait takes about a thousand reads.</summary>
    public static readonly TimeSpan DefaultQuantum = TimeSpan.FromMilliseconds(1);

    private readonly DateTimeOffset _start;
    private readonly object _gate = new();
    private long _elapsedTicks;

    public DeterministicClock(DateTimeOffset start, TimeSpan? quantum = null)
    {
        _start = start;
        Quantum = quantum ?? DefaultQuantum;
        if (Quantum < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(quantum), quantum, "The quantum cannot be negative.");
        }
    }

    /// <summary>How far time moves on every read (<see cref="TimeSpan.Zero"/> freezes it).</summary>
    public TimeSpan Quantum { get; }

    /// <summary>The time elapsed since the start, without advancing.</summary>
    public TimeSpan Elapsed
    {
        get
        {
            lock (_gate)
            {
                return TimeSpan.FromTicks(_elapsedTicks);
            }
        }
    }

    public DateTimeOffset UtcNow => _start + TimeSpan.FromTicks(Read());

    public long TickCount => Read() / TimeSpan.TicksPerMillisecond;

    /// <summary>100 ns units (<see cref="HighResolutionFrequency"/> = 10 000 000).</summary>
    public long HighResolutionTicks => Read();

    public long HighResolutionFrequency => TimeSpan.TicksPerSecond;

    /// <summary>
    /// <c>Sleep(ms)</c> without blocking: time moves forward by <paramref name="milliseconds"/> (on top of the read quantum),
    /// so <c>Sleep(100)</c> between two <c>GetTickCount</c> reads shows 100 ms more; <c>INFINITE</c> advances 0xFFFFFFFF ms.
    /// </summary>
    public void Sleep(uint milliseconds) => Advance(TimeSpan.FromMilliseconds(milliseconds));

    /// <summary>Moves time forward explicitly (a test simulating a pause).</summary>
    public void Advance(TimeSpan by)
    {
        lock (_gate)
        {
            _elapsedTicks += by.Ticks;
        }
    }

    private long Read()
    {
        lock (_gate)
        {
            long value = _elapsedTicks;
            _elapsedTicks += Quantum.Ticks;
            return value;
        }
    }
}
