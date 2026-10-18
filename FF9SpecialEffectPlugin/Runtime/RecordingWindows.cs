using System.Globalization;

namespace Recompiler.Runtime;

/// <summary>
/// Windows for tests and headless runs: records every call as a line, hands out handles from 0x1000 up, and serves
/// <see cref="PeekMessage"/>/<see cref="GetMessage"/> from a scripted queue (<see cref="Enqueue"/>); an empty queue reads as
/// "no message" for <c>PeekMessage</c> and as <c>WM_QUIT</c> for <c>GetMessage</c>, so message loops end. A window whose class
/// was registered with an entry from <c>RegisterWindowProcedure</c> (managed or <c>[UnmanagedCallersOnly]</c>) really receives <see cref="DispatchMessage"/>/<see cref="SendMessage"/> (and <c>WM_CREATE</c>), so a test drives the rewritten
/// window procedure without a desktop. Keys (<see cref="KeysDown"/>), device capabilities (<see cref="DeviceCaps"/>), metrics
/// and the client size are scripted; <see cref="Tick"/> queues <c>WM_TIMER</c> for every live timer.
/// </summary>
public sealed unsafe class RecordingWindows : IWindowSystem
{
    private const uint WmCreate = 0x0001;
    private const uint WmQuit = 0x0012;
    private const uint WmTimer = 0x0113;
    private const uint WsCaption = 0x00C00000;
    private const uint WsThickFrame = 0x00040000;
    private const uint WsBorder = 0x00800000;

    private readonly List<string> _events = [];
    private readonly Queue<WindowMessage> _queue = new();
    private readonly Dictionary<string, WindowClass> _classes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<nint, (WindowClass Class, Dictionary<int, nint> Longs)> _windows = [];
    private readonly SortedDictionary<(nint Window, nuint Id), uint> _timers = [];
    private readonly HashSet<int> _pressedSinceLastRead = [];
    private readonly HashSet<nint> _unmanagedProcedures = [];
    private nint _nextHandle = 0x1000;
    private ushort _nextAtom = 0xC000;
    private nuint _nextTimer = 1;
    private int _cursorCount;

    public IReadOnlyList<string> Events => _events;

    /// <summary>What <see cref="GetSystemMetrics"/> answers (default 0); <c>SM_CXSCREEN</c> 0 and <c>SM_CYSCREEN</c> 1 are preset to 640x480.</summary>
    public Dictionary<int, int> Metrics { get; } = new() { [0] = 640, [1] = 480 };

    /// <summary>What <see cref="GetDeviceCaps"/> answers (default 0): <c>HORZRES</c> 640, <c>VERTRES</c> 480, <c>BITSPIXEL</c> 32, <c>PLANES</c> 1, <c>LOGPIXELSX/Y</c> 96, <c>VREFRESH</c> 60.</summary>
    public Dictionary<int, int> DeviceCaps { get; } = new() { [8] = 640, [10] = 480, [12] = 32, [14] = 1, [88] = 96, [90] = 96, [116] = 60 };

    /// <summary>Virtual keys currently down (<see cref="GetAsyncKeyState"/>, <see cref="GetKeyState"/>); <see cref="Press"/> also marks "pressed since the last read".</summary>
    public HashSet<int> KeysDown { get; } = [];

    /// <summary>The client size <see cref="GetClientRect"/> and <see cref="BeginPaint"/> report for every window.</summary>
    public (int Width, int Height) ClientSize { get; set; } = (640, 480);

    public void Enqueue(WindowMessage message) => _queue.Enqueue(message);

    /// <summary>Puts <paramref name="key"/> down; the next <see cref="GetAsyncKeyState"/> also reports bit 0.</summary>
    public void Press(int key)
    {
        KeysDown.Add(key);
        _pressedSinceLastRead.Add(key);
    }

    public void Release(int key) => KeysDown.Remove(key);

    /// <summary>Queues one <c>WM_TIMER</c> per live timer, in (window, id) order, as if every period had elapsed once.</summary>
    public void Tick()
    {
        foreach ((nint window, nuint id) in _timers.Keys)
        {
            _queue.Enqueue(new WindowMessage { Window = window, Message = WmTimer, WParam = (nint)id });
        }
    }

    public nint RegisterWindowProcedure(delegate*<nint, uint, nint, nint, nint> procedure) => WindowProcedures.Entry(procedure);

    public nint RegisterWindowProcedure(delegate*<nint, uint, nuint, nint, nint> procedure) => WindowProcedures.Entry(procedure);

    /// <summary>The <c>nuint</c>-WPARAM shape of a generated <c>CodeThunk_</c>: remembered like the <c>nint</c> one.</summary>
    public nint RegisterWindowProcedure(delegate* unmanaged[Stdcall]<nint, uint, nuint, nint, nint> procedure) =>
        RegisterWindowProcedure((delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)procedure);

    public ushort RegisterClass(string className, uint style, nint windowProcedure, nint instance, nint icon, nint cursor, nint background, string? menuName) =>
        RegisterClass(new WindowClass(className) { Style = style, WindowProcedure = windowProcedure, Instance = instance, Icon = icon, Cursor = cursor, Background = background, MenuName = menuName });

    /// <summary>Remembers the entry so <see cref="DispatchMessage"/> and <see cref="SendMessage"/> call it.</summary>
    public nint RegisterWindowProcedure(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint> procedure)
    {
        _unmanagedProcedures.Add((nint)procedure);
        return (nint)procedure;
    }

    public ushort RegisterClass(WindowClass windowClass)
    {
        ArgumentNullException.ThrowIfNull(windowClass);
        _classes[windowClass.ClassName] = windowClass;
        string extra = windowClass.ClassExtraBytes != 0 || windowClass.WindowExtraBytes != 0 ? $" extra={windowClass.ClassExtraBytes}/{windowClass.WindowExtraBytes}" : string.Empty;
        string menu = windowClass.MenuName is { } name ? $" menu={name}" : windowClass.MenuResourceId != 0 ? $" menu=#{windowClass.MenuResourceId}" : string.Empty;
        _events.Add($"RegisterClass{(windowClass.Ansi ? "A" : string.Empty)} {windowClass.ClassName} style=0x{windowClass.Style:x}{extra}{menu}");
        return _nextAtom++;
    }

    public nint CreateWindow(uint exStyle, string className, string? title, uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter, bool ansi = false)
    {
        nint handle = _nextHandle;
        _nextHandle += 4;
        _events.Add($"CreateWindow {className} \"{title}\" style=0x{style:x} {x},{y} {width}x{height} -> 0x{handle:x}");
        WindowClass windowClass = _classes.TryGetValue(className, out WindowClass? registered) ? registered : new WindowClass(className);
        _windows[handle] = (windowClass, []);
        if (WindowProcedures.ManagedOf(windowClass.WindowProcedure) is not null || _unmanagedProcedures.Contains(windowClass.WindowProcedure))
        {
            Call(handle, WmCreate, 0, parameter);
        }

        return handle;
    }

    public bool DestroyWindow(nint window)
    {
        _windows.Remove(window);
        return Record($"DestroyWindow 0x{window:x}", true);
    }

    public bool ShowWindow(nint window, int command) => Record($"ShowWindow 0x{window:x} {command}", false);

    public bool UpdateWindow(nint window) => Record($"UpdateWindow 0x{window:x}", true);

    public bool SetWindowText(nint window, string text, bool ansi = false) => Record($"SetWindowText 0x{window:x} \"{text}\"", true);

    public bool SetWindowPos(nint window, nint insertAfter, int x, int y, int width, int height, uint flags) => Record($"SetWindowPos 0x{window:x} {x},{y} {width}x{height} flags=0x{flags:x}", true);

    /// <summary>Fixed frame metrics: a 23-pixel caption, an 8-pixel sizing frame (3 for a plain border), a 20-pixel menu bar.</summary>
    public bool AdjustWindowRect(ref WindowRect rect, uint style, bool menu, uint exStyle = 0)
    {
        int frame = (style & WsThickFrame) != 0 ? 8 : (style & (WsBorder | WsCaption)) != 0 ? 3 : 0;
        int caption = (style & WsCaption) == WsCaption ? 23 : 0;
        rect = new WindowRect(rect.Left - frame, rect.Top - frame - caption - (menu ? 20 : 0), rect.Right + frame, rect.Bottom + frame);
        return true;
    }

    public bool GetClientRect(nint window, out WindowRect rect)
    {
        rect = new WindowRect(0, 0, ClientSize.Width, ClientSize.Height);
        return true;
    }

    public bool InvalidateRect(nint window, WindowRect? rect, bool erase) =>
        Record($"InvalidateRect 0x{window:x}{(rect is { } r ? $" {r.Left},{r.Top},{r.Right},{r.Bottom}" : string.Empty)}{(erase ? " erase" : string.Empty)}", true);

    public nint GetWindowLong(nint window, int index, bool ansi = false) =>
        _windows.TryGetValue(window, out var state) && state.Longs.TryGetValue(index, out nint value) ? value
        : index == -4 && _windows.TryGetValue(window, out var known) ? known.Class.WindowProcedure
        : 0;

    public nint SetWindowLong(nint window, int index, nint value, bool ansi = false)
    {
        nint previous = GetWindowLong(window, index, ansi);
        if (_windows.TryGetValue(window, out var state))
        {
            state.Longs[index] = value;
        }

        _events.Add($"SetWindowLong 0x{window:x} {index.ToString(CultureInfo.InvariantCulture)} 0x{value:x}");
        return previous;
    }

    public bool PeekMessage(out WindowMessage message, nint window, uint filterMin, uint filterMax, uint remove, bool ansi = false)
    {
        if (_queue.Count == 0)
        {
            message = default;
            return false;
        }

        message = (remove & 1) != 0 ? _queue.Dequeue() : _queue.Peek();
        return true;
    }

    public bool GetMessage(out WindowMessage message, nint window, uint filterMin, uint filterMax, bool ansi = false)
    {
        message = _queue.Count > 0 ? _queue.Dequeue() : new WindowMessage { Message = WmQuit };
        return message.Message != WmQuit;
    }

    /// <summary>Returns at once: an empty queue would block forever, so the next <see cref="GetMessage"/> reads <c>WM_QUIT</c> instead.</summary>
    public bool WaitMessage() => Record("WaitMessage", true);

    public bool TranslateMessage(in WindowMessage message) => false;

    public nint DispatchMessage(in WindowMessage message, bool ansi = false)
    {
        _events.Add($"DispatchMessage 0x{message.Window:x} 0x{message.Message:x}");
        return Call(message.Window, message.Message, message.WParam, message.LParam);
    }

    public void PostQuitMessage(int exitCode)
    {
        _events.Add($"PostQuitMessage {exitCode}");
        _queue.Enqueue(new WindowMessage { Message = WmQuit, WParam = exitCode });
    }

    public bool PostMessage(nint window, uint message, nint wParam, nint lParam, bool ansi = false)
    {
        _events.Add($"PostMessage 0x{window:x} 0x{message:x}");
        _queue.Enqueue(new WindowMessage { Window = window, Message = message, WParam = wParam, LParam = lParam });
        return true;
    }

    public nint SendMessage(nint window, uint message, nint wParam, nint lParam, bool ansi = false)
    {
        _events.Add($"SendMessage 0x{window:x} 0x{message:x}");
        return Call(window, message, wParam, lParam);
    }

    public nint DefWindowProc(nint window, uint message, nint wParam, nint lParam, bool ansi = false) => 0;

    public nuint SetTimer(nint window, nuint id, uint elapse, nint timerProcedure = 0)
    {
        nuint timer = window == 0 ? _nextTimer++ : id;
        _timers[(window, timer)] = elapse;
        _events.Add($"SetTimer 0x{window:x} {timer} {elapse}ms");
        return timer;
    }

    public bool KillTimer(nint window, nuint id) => Record($"KillTimer 0x{window:x} {id}", _timers.Remove((window, id)));

    public nint BeginPaint(nint window, out PaintInfo paint)
    {
        paint = default;
        paint.Dc = 0x2000 + window;
        paint.Paint = new WindowRect(0, 0, ClientSize.Width, ClientSize.Height);
        _events.Add($"BeginPaint 0x{window:x}");
        return paint.Dc;
    }

    public bool EndPaint(nint window, in PaintInfo paint) => Record($"EndPaint 0x{window:x}", true);

    public nint GetDC(nint window) => 0x2000 + window;

    public int ReleaseDC(nint window, nint dc) => 1;

    public int GetDeviceCaps(nint dc, int index) => DeviceCaps.TryGetValue(index, out int value) ? value : 0;

    /// <summary>A fixed fake handle per stock object (<c>0x5000 + index</c>).</summary>
    public nint GetStockObject(int index) => 0x5000 + index;

    public short GetAsyncKeyState(int key)
    {
        int state = (KeysDown.Contains(key) ? 0x8000 : 0) | (_pressedSinceLastRead.Remove(key) ? 1 : 0);
        return unchecked((short)state);
    }

    public short GetKeyState(int key) => unchecked((short)(KeysDown.Contains(key) ? 0x8000 : 0));

    public bool SystemParametersInfo(uint action, uint parameter, nint data, uint winIni) => Record($"SystemParametersInfo 0x{action:x} {parameter}", true);

    public int GetSystemMetrics(int index) => Metrics.TryGetValue(index, out int value) ? value : 0;

    public nint LoadCursor(nint instance, nint name) => 0x7F00 + (name & 0xFF);

    public nint LoadIcon(nint instance, nint name) => 0x7E00 + (name & 0xFF);

    public int ShowCursor(bool show) => show ? ++_cursorCount : --_cursorCount;

    /// <summary>Runs the window's managed procedure (its class's, or one set with <c>GWL_WNDPROC</c>); 0 when it has none.</summary>
    private nint Call(nint window, uint message, nint wParam, nint lParam)
    {
        nint entry = GetWindowLong(window, -4);
        delegate*<nint, uint, nint, nint, nint> procedure = WindowProcedures.ManagedOf(entry);
        if (procedure is not null)
        {
            return procedure(window, message, wParam, lParam);
        }

        return _unmanagedProcedures.Contains(entry) ? ((delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)entry)(window, message, wParam, lParam) : 0;
    }

    private bool Record(string line, bool result)
    {
        _events.Add(line);
        return result;
    }
}
