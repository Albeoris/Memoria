using System.Runtime.InteropServices;

namespace Recompiler.Runtime;

/// <summary>The Win32 <c>MSG</c> structure, field for field.</summary>
[StructLayout(LayoutKind.Sequential)]
public struct WindowMessage
{
    public nint Window;
    public uint Message;
    public nint WParam;
    public nint LParam;
    public uint Time;
    public int X;
    public int Y;
}

/// <summary>The Win32 <c>RECT</c> structure.</summary>
[StructLayout(LayoutKind.Sequential)]
public struct WindowRect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public WindowRect(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public readonly int Width => Right - Left;

    public readonly int Height => Bottom - Top;
}

/// <summary>The Win32 <c>PAINTSTRUCT</c> structure (<c>BeginPaint</c>/<c>EndPaint</c>), field for field.</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct PaintInfo
{
    public nint Dc;
    public int Erase;
    public WindowRect Paint;
    public int Restore;
    public int IncUpdate;
    public fixed byte Reserved[32];
}

/// <summary>
/// A window class as <c>RegisterClass(Ex)A/W</c> takes it. <see cref="MenuName"/> names a menu resource by string; a menu
/// identified by number (<c>MAKEINTRESOURCE(101)</c>) goes in <see cref="MenuResourceId"/>. <see cref="Ansi"/> registers the
/// class with <c>RegisterClassExA</c>: its window procedure then receives ANSI text (<c>WM_CHAR</c>, <c>WM_SETTEXT</c>), as the
/// original's did when it called the A functions.
/// </summary>
public sealed record WindowClass(string ClassName)
{
    /// <summary><c>CS_*</c> style bits.</summary>
    public uint Style { get; init; }

    /// <summary>The native entry of the window procedure (<c>IWindowSystem.RegisterWindowProcedure</c>).</summary>
    public nint WindowProcedure { get; init; }

    /// <summary><c>cbClsExtra</c>: bytes reserved after the class structure (<c>GetClassLong</c>).</summary>
    public int ClassExtraBytes { get; init; }

    /// <summary><c>cbWndExtra</c>: bytes reserved per window (<see cref="IWindowSystem.GetWindowLong"/> with a non-negative index).</summary>
    public int WindowExtraBytes { get; init; }

    public nint Instance { get; init; }

    public nint Icon { get; init; }

    public nint Cursor { get; init; }

    /// <summary>The background brush: a brush handle, <c>COLOR_* + 1</c>, or <see cref="IWindowSystem.GetStockObject"/>.</summary>
    public nint Background { get; init; }

    public string? MenuName { get; init; }

    /// <summary>A menu resource identified by number; used when <see cref="MenuName"/> is <c>null</c> and the id is not 0.</summary>
    public ushort MenuResourceId { get; init; }

    public nint SmallIcon { get; init; }

    /// <summary>Registers with the A function (<c>RegisterClassExA</c>) instead of the W one.</summary>
    public bool Ansi { get; init; }
}

/// <summary>
/// The thin window surface a ported Win32 program needs for its main window, message loop, painting, input and timers.
/// Handles stay native-sized integers, so the core keeps the original's control flow while the shell (the default,
/// <see cref="Win32Windows"/>) or a test (<see cref="RecordingWindows"/>) decides what a window is. Strings are the program's
/// own (ANSI text decoded by the caller). A program that used the A functions passes <c>ansi: true</c> (and
/// <see cref="WindowClass.Ansi"/>), so its window procedure gets ANSI text as the original's did; the default is W.
/// The window procedure is a plain static method registered once with <c>RegisterWindowProcedure</c>,
/// which hands out a native <c>__stdcall</c> entry on x86 (the platform convention on x64): a unit never writes its own thunk.
/// </summary>
public unsafe interface IWindowSystem
{
    /// <summary><c>RegisterClassExA/W</c>: returns the class atom, 0 on failure.</summary>
    ushort RegisterClass(WindowClass windowClass);

    /// <summary><c>RegisterClass</c> with the common fields (W, no extra bytes); see <see cref="RegisterClass(WindowClass)"/>.</summary>
    ushort RegisterClass(string className, uint style, nint windowProcedure, nint instance, nint icon, nint cursor, nint background, string? menuName) =>
        RegisterClass(new WindowClass(className) { Style = style, WindowProcedure = windowProcedure, Instance = instance, Icon = icon, Cursor = cursor, Background = background, MenuName = menuName });

    /// <summary>
    /// The native entry (<c>lpfnWndProc</c>, <c>SetWindowLong(GWL_WNDPROC)</c>) for a managed window procedure: a
    /// <c>__stdcall</c> entry on x86, the platform convention on x64, from a fixed pool (<see cref="WindowProcedures.Capacity"/>);
    /// the same procedure always gets the same entry.
    /// </summary>
    nint RegisterWindowProcedure(delegate*<nint, uint, nint, nint, nint> procedure) => WindowProcedures.Entry(procedure);

    /// <summary>The native entry of a procedure that is already <c>[UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]</c>.</summary>
    nint RegisterWindowProcedure(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint> procedure) => (nint)procedure;

    /// <summary>A managed procedure that takes <c>WPARAM</c> as <c>nuint</c> (the generated <c>CodeThunk_</c> shape); same pool and entry as the <c>nint</c> overload.</summary>
    nint RegisterWindowProcedure(delegate*<nint, uint, nuint, nint, nint> procedure) => WindowProcedures.Entry(procedure);

    /// <summary>An <c>[UnmanagedCallersOnly]</c> <c>__stdcall</c> procedure that takes <c>WPARAM</c> as <c>nuint</c> (a generated <c>CodeThunk_</c>).</summary>
    nint RegisterWindowProcedure(delegate* unmanaged[Stdcall]<nint, uint, nuint, nint, nint> procedure) => RegisterWindowProcedure((delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)procedure);

    /// <summary><c>CreateWindowExA/W</c>: returns the window handle, 0 on failure.</summary>
    nint CreateWindow(uint exStyle, string className, string? title, uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter, bool ansi = false);

    bool DestroyWindow(nint window);

    bool ShowWindow(nint window, int command);

    bool UpdateWindow(nint window);

    bool SetWindowText(nint window, string text, bool ansi = false);

    bool SetWindowPos(nint window, nint insertAfter, int x, int y, int width, int height, uint flags);

    /// <summary><c>AdjustWindowRectEx</c>: grows the client rectangle to the window rectangle for the style.</summary>
    bool AdjustWindowRect(ref WindowRect rect, uint style, bool menu, uint exStyle = 0);

    bool GetClientRect(nint window, out WindowRect rect);

    /// <summary><c>InvalidateRect</c>; <c>null</c> invalidates the whole client area.</summary>
    bool InvalidateRect(nint window, WindowRect? rect, bool erase);

    /// <summary><c>GetWindowLong(Ptr)A/W</c>: a <c>GWL_*</c> value or, for a non-negative index, the window's extra bytes.</summary>
    nint GetWindowLong(nint window, int index, bool ansi = false);

    /// <summary><c>SetWindowLong(Ptr)A/W</c>: returns the previous value.</summary>
    nint SetWindowLong(nint window, int index, nint value, bool ansi = false);

    /// <summary><c>PeekMessageA/W</c>: true when a message was retrieved into <paramref name="message"/>.</summary>
    bool PeekMessage(out WindowMessage message, nint window, uint filterMin, uint filterMax, uint remove, bool ansi = false);

    /// <summary><c>GetMessageA/W</c>: false on <c>WM_QUIT</c> (and on error).</summary>
    bool GetMessage(out WindowMessage message, nint window, uint filterMin, uint filterMax, bool ansi = false);

    /// <summary><c>WaitMessage</c>: blocks until a message is queued.</summary>
    bool WaitMessage();

    bool TranslateMessage(in WindowMessage message);

    nint DispatchMessage(in WindowMessage message, bool ansi = false);

    void PostQuitMessage(int exitCode);

    bool PostMessage(nint window, uint message, nint wParam, nint lParam, bool ansi = false);

    nint SendMessage(nint window, uint message, nint wParam, nint lParam, bool ansi = false);

    nint DefWindowProc(nint window, uint message, nint wParam, nint lParam, bool ansi = false);

    /// <summary><c>SetTimer</c>: the timer id (<paramref name="id"/>, or a new one when <paramref name="window"/> is 0); 0 on failure.</summary>
    nuint SetTimer(nint window, nuint id, uint elapse, nint timerProcedure = 0);

    bool KillTimer(nint window, nuint id);

    /// <summary><c>BeginPaint</c>: the paint device context, with <paramref name="paint"/> filled.</summary>
    nint BeginPaint(nint window, out PaintInfo paint);

    bool EndPaint(nint window, in PaintInfo paint);

    nint GetDC(nint window);

    int ReleaseDC(nint window, nint dc);

    /// <summary><c>GetDeviceCaps</c> (<c>HORZRES</c> 8, <c>VERTRES</c> 10, <c>BITSPIXEL</c> 12, <c>PLANES</c> 14, <c>LOGPIXELSX</c> 88, <c>VREFRESH</c> 116).</summary>
    int GetDeviceCaps(nint dc, int index);

    /// <summary><c>GetStockObject</c> (<c>WHITE_BRUSH</c> 0, <c>BLACK_BRUSH</c> 4, <c>NULL_BRUSH</c> 5, <c>SYSTEM_FONT</c> 13, ...).</summary>
    nint GetStockObject(int index);

    /// <summary><c>GetAsyncKeyState</c>: bit 15 set while the key is down, bit 0 when it was pressed since the last call.</summary>
    short GetAsyncKeyState(int key);

    /// <summary><c>GetKeyState</c>: bit 15 set while down, bit 0 the toggle state.</summary>
    short GetKeyState(int key);

    /// <summary><c>SystemParametersInfo</c> with the original's pointer argument.</summary>
    bool SystemParametersInfo(uint action, uint parameter, nint data, uint winIni);

    int GetSystemMetrics(int index);

    /// <summary><c>LoadCursor</c>/<c>LoadIcon</c> with a resource id (<c>IDC_ARROW</c> 32512) or name pointer.</summary>
    nint LoadCursor(nint instance, nint name);

    nint LoadIcon(nint instance, nint name);

    int ShowCursor(bool show);
}
