namespace Recompiler.Runtime;

/// <summary>
/// Win32 critical sections for rewritten code (<c>InitializeCriticalSection</c>, <c>EnterCriticalSection</c>,
/// <c>TryEnterCriticalSection</c>, <c>LeaveCriticalSection</c>, <c>DeleteCriticalSection</c>): one managed
/// <see cref="Lock"/> per <c>CRITICAL_SECTION</c> address, so the original's structs (globals of the data image, fields of
/// heap objects) keep their identity while the locking itself is managed. Re-entrant like Win32: the owning thread may
/// enter again and must leave as often. The native struct's bytes are never read or written. Entering a section that was
/// never initialized creates it (the CRT's own sections are set up by start-up code that is not ported); leaving a section
/// the calling thread does not hold does nothing (Win32 leaves that undefined).
/// </summary>
public static unsafe class CrtSync
{
    private static readonly Lock Gate = new();
    private static readonly Dictionary<nint, Lock> Sections = [];

    /// <summary><c>InitializeCriticalSection(section)</c>; initializing a live section again replaces it, as Win32 does.</summary>
    public static void Initialize(void* section) => Initialize((nint)section);

    public static void Initialize(nint section)
    {
        lock (Gate)
        {
            Sections[section] = new Lock();
        }
    }

    /// <summary><c>InitializeCriticalSectionAndSpinCount(section, spin)</c>: the spin count is ignored; returns TRUE.</summary>
    public static bool InitializeAndSpinCount(void* section, uint spinCount)
    {
        _ = spinCount;
        Initialize((nint)section);
        return true;
    }

    /// <summary><c>EnterCriticalSection(section)</c>: blocks until the calling thread owns the section.</summary>
    public static void Enter(void* section) => Enter((nint)section);

    public static void Enter(nint section) => Of(section).Enter();

    /// <summary><c>TryEnterCriticalSection(section)</c>: true when the calling thread now owns it (also when it already did).</summary>
    public static bool TryEnter(void* section) => TryEnter((nint)section);

    public static bool TryEnter(nint section) => Of(section).TryEnter();

    /// <summary><c>LeaveCriticalSection(section)</c>: releases one level of ownership.</summary>
    public static void Leave(void* section) => Leave((nint)section);

    public static void Leave(nint section)
    {
        Lock? target;
        lock (Gate)
        {
            Sections.TryGetValue(section, out target);
        }

        if (target is { IsHeldByCurrentThread: true })
        {
            target.Exit();
        }
    }

    /// <summary><c>DeleteCriticalSection(section)</c>: forgets the section; the address may be initialized again.</summary>
    public static void Delete(void* section) => Delete((nint)section);

    public static void Delete(nint section)
    {
        lock (Gate)
        {
            Sections.Remove(section);
        }
    }

    /// <summary>True when the calling thread owns the section (tests, assertions).</summary>
    public static bool IsHeld(void* section)
    {
        lock (Gate)
        {
            return Sections.TryGetValue((nint)section, out Lock? target) && target.IsHeldByCurrentThread;
        }
    }

    private static Lock Of(nint section)
    {
        lock (Gate)
        {
            if (!Sections.TryGetValue(section, out Lock? target))
            {
                Sections[section] = target = new Lock();
            }

            return target;
        }
    }
}
