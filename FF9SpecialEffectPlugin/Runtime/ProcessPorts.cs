using System.ComponentModel;

namespace Recompiler.Runtime;

/// <summary>Starts real processes (the default of <see cref="Ports.Process"/>).</summary>
public sealed class SystemProcessLauncher : IProcessLauncher
{
    private const int ErrorFileNotFound = 2;
    private const int ErrorPathNotFound = 3;
    private const int ErrorAccessDenied = 5;
    private const int ErrorInvalidParameter = 87;

    public int Start(string path, string? arguments, bool wait) => Start(path, arguments, wait, null) is { Started: true } started ? started.Value : -1;

    public ProcessStartResult Start(string path, string? arguments, bool wait, string? workingDirectory)
    {
        if (workingDirectory is not null && !Directory.Exists(workingDirectory))
        {
            return ProcessStartResult.Failed(ErrorPathNotFound, $"The working directory '{workingDirectory}' does not exist.");
        }

        try
        {
            var start = new System.Diagnostics.ProcessStartInfo(path, arguments ?? string.Empty) { UseShellExecute = false };
            if (workingDirectory is not null)
            {
                start.WorkingDirectory = workingDirectory;
            }

            using System.Diagnostics.Process? process = System.Diagnostics.Process.Start(start);
            if (process is null)
            {
                return ProcessStartResult.Failed(ErrorInvalidParameter, $"'{path}' started no process.");
            }

            if (!wait)
            {
                return ProcessStartResult.Running(process.Id);
            }

            process.WaitForExit();
            return ProcessStartResult.Exited(process.ExitCode);
        }
        catch (Win32Exception exception)
        {
            return ProcessStartResult.Failed(exception.NativeErrorCode, exception.Message);
        }
        catch (FileNotFoundException exception)
        {
            return ProcessStartResult.Failed(ErrorFileNotFound, exception.Message);
        }
        catch (DirectoryNotFoundException exception)
        {
            return ProcessStartResult.Failed(ErrorPathNotFound, exception.Message);
        }
        catch (UnauthorizedAccessException exception)
        {
            return ProcessStartResult.Failed(ErrorAccessDenied, exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return ProcessStartResult.Failed(ErrorInvalidParameter, exception.Message);
        }
    }

    /// <summary>Sets the priority class of this process through <see cref="System.Diagnostics.Process.PriorityClass"/>.</summary>
    public bool SetPriorityClass(uint priorityClass)
    {
        System.Diagnostics.ProcessPriorityClass? target = priorityClass switch
        {
            PriorityClasses.Idle => System.Diagnostics.ProcessPriorityClass.Idle,
            PriorityClasses.BelowNormal => System.Diagnostics.ProcessPriorityClass.BelowNormal,
            PriorityClasses.Normal => System.Diagnostics.ProcessPriorityClass.Normal,
            PriorityClasses.AboveNormal => System.Diagnostics.ProcessPriorityClass.AboveNormal,
            PriorityClasses.High => System.Diagnostics.ProcessPriorityClass.High,
            PriorityClasses.Realtime => System.Diagnostics.ProcessPriorityClass.RealTime,
            _ => null,
        };
        if (target is null)
        {
            return false;
        }

        try
        {
            using System.Diagnostics.Process current = System.Diagnostics.Process.GetCurrentProcess();
            current.PriorityClass = target.Value;
            return true;
        }
        catch (Exception exception) when (exception is Win32Exception or InvalidOperationException or PlatformNotSupportedException)
        {
            return false;
        }
    }

    /// <summary>
    /// Sets the calling thread's priority through <see cref="Thread.Priority"/>: <c>THREAD_PRIORITY_IDLE</c> maps to
    /// <see cref="ThreadPriority.Lowest"/> and <c>THREAD_PRIORITY_TIME_CRITICAL</c> to <see cref="ThreadPriority.Highest"/>,
    /// the nearest levels .NET offers.
    /// </summary>
    public bool SetThreadPriority(int priority)
    {
        ThreadPriority? target = priority switch
        {
            PriorityClasses.ThreadIdle or PriorityClasses.ThreadLowest => ThreadPriority.Lowest,
            PriorityClasses.ThreadBelowNormal => ThreadPriority.BelowNormal,
            PriorityClasses.ThreadNormal => ThreadPriority.Normal,
            PriorityClasses.ThreadAboveNormal => ThreadPriority.AboveNormal,
            PriorityClasses.ThreadHighest or PriorityClasses.ThreadTimeCritical => ThreadPriority.Highest,
            _ => null,
        };
        if (target is null)
        {
            return false;
        }

        try
        {
            Thread.CurrentThread.Priority = target.Value;
            return true;
        }
        catch (Exception exception) when (exception is ThreadStateException or PlatformNotSupportedException)
        {
            return false;
        }
    }
}

/// <summary>
/// Records every start (<c>path args (wait) [in dir]</c>) and answers with <see cref="ExitCode"/>; nothing runs. Paths listed in
/// <see cref="Missing"/> fail with <c>ERROR_FILE_NOT_FOUND</c>, so a test covers the original's error path. Priority changes
/// are recorded in <see cref="PriorityEvents"/> and change nothing.
/// </summary>
public sealed class RecordingProcessLauncher : IProcessLauncher
{
    private readonly List<string> _events = [];
    private readonly List<string> _priorityEvents = [];

    /// <summary>What a waited start returns (default 0); a start without waiting returns a fake process id from 1000 up.</summary>
    public int ExitCode { get; set; }

    /// <summary>Programs that "do not exist" (compared case-insensitively).</summary>
    public HashSet<string> Missing { get; } = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<string> Events => _events;

    public int Start(string path, string? arguments, bool wait) => Start(path, arguments, wait, null) is { Started: true } started ? started.Value : -1;

    public ProcessStartResult Start(string path, string? arguments, bool wait, string? workingDirectory)
    {
        _events.Add($"{path}{(string.IsNullOrEmpty(arguments) ? string.Empty : " " + arguments)}{(wait ? " (wait)" : string.Empty)}{(workingDirectory is null ? string.Empty : " in " + workingDirectory)}");
        if (Missing.Contains(path))
        {
            return ProcessStartResult.Failed(2, $"'{path}' was not found.");
        }

        return wait ? ProcessStartResult.Exited(ExitCode) : ProcessStartResult.Running(1000 + _events.Count);
    }

    /// <summary>Priority changes in call order (<c>priority-class 0x80</c>, <c>thread-priority 2</c>); nothing is changed for real.</summary>
    public IReadOnlyList<string> PriorityEvents => _priorityEvents;

    /// <summary>The last accepted <see cref="SetPriorityClass"/> value (<see cref="PriorityClasses.Normal"/> at first).</summary>
    public uint PriorityClass { get; private set; } = PriorityClasses.Normal;

    /// <summary>The last accepted <see cref="SetThreadPriority"/> value (<see cref="PriorityClasses.ThreadNormal"/> at first).</summary>
    public int ThreadPriority { get; private set; } = PriorityClasses.ThreadNormal;

    public bool SetPriorityClass(uint priorityClass)
    {
        _priorityEvents.Add($"priority-class 0x{priorityClass:X}");
        if (!PriorityClasses.IsKnown(priorityClass))
        {
            return false;
        }

        PriorityClass = priorityClass;
        return true;
    }

    public bool SetThreadPriority(int priority)
    {
        _priorityEvents.Add($"thread-priority {priority}");
        if (!PriorityClasses.IsKnownThreadPriority(priority))
        {
            return false;
        }

        ThreadPriority = priority;
        return true;
    }
}
