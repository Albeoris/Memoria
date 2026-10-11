using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Recompiler.Runtime;

/// <summary>Windows through user32 and gdi32 (the default of <see cref="Ports.Windows"/> on Windows); <c>ansi</c> selects the A entry points.</summary>
[SupportedOSPlatform("windows")]
public sealed unsafe partial class Win32Windows : IWindowSystem
{
    public nint RegisterWindowProcedure(delegate*<nint, uint, nint, nint, nint> procedure) => WindowProcedures.Entry(procedure);

    public nint RegisterWindowProcedure(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint> procedure) => (nint)procedure;

    public nint RegisterWindowProcedure(delegate*<nint, uint, nuint, nint, nint> procedure) => WindowProcedures.Entry(procedure);

    public nint RegisterWindowProcedure(delegate* unmanaged[Stdcall]<nint, uint, nuint, nint, nint> procedure) => (nint)procedure;

    public ushort RegisterClass(string className, uint style, nint windowProcedure, nint instance, nint icon, nint cursor, nint background, string? menuName) =>
        RegisterClass(new WindowClass(className) { Style = style, WindowProcedure = windowProcedure, Instance = instance, Icon = icon, Cursor = cursor, Background = background, MenuName = menuName });

    public ushort RegisterClass(WindowClass windowClass)
    {
        ArgumentNullException.ThrowIfNull(windowClass);
        if (windowClass.Ansi)
        {
            nint className = Marshal.StringToHGlobalAnsi(windowClass.ClassName);
            nint menuName = windowClass.MenuName is null ? 0 : Marshal.StringToHGlobalAnsi(windowClass.MenuName);
            try
            {
                WndClassEx native = Describe(windowClass, className, menuName);
                return RegisterClassExA(&native);
            }
            finally
            {
                Marshal.FreeHGlobal(className);
                Marshal.FreeHGlobal(menuName);
            }
        }

        fixed (char* className = windowClass.ClassName)
        fixed (char* menuName = windowClass.MenuName)
        {
            WndClassEx native = Describe(windowClass, (nint)className, (nint)menuName);
            return RegisterClassExW(&native);
        }
    }

    public nint CreateWindow(uint exStyle, string className, string? title, uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter, bool ansi = false) =>
        ansi
            ? CreateWindowExA(exStyle, className, title, style, x, y, width, height, parent, menu, instance, parameter)
            : CreateWindowExW(exStyle, className, title, style, x, y, width, height, parent, menu, instance, parameter);

    public bool DestroyWindow(nint window) => DestroyWindowNative(window) != 0;

    public bool ShowWindow(nint window, int command) => ShowWindowNative(window, command) != 0;

    public bool UpdateWindow(nint window) => UpdateWindowNative(window) != 0;

    public bool SetWindowText(nint window, string text, bool ansi = false) => (ansi ? SetWindowTextA(window, text) : SetWindowTextW(window, text)) != 0;

    public bool SetWindowPos(nint window, nint insertAfter, int x, int y, int width, int height, uint flags) => SetWindowPosNative(window, insertAfter, x, y, width, height, flags) != 0;

    public bool AdjustWindowRect(ref WindowRect rect, uint style, bool menu, uint exStyle = 0)
    {
        WindowRect copy = rect;
        bool done = AdjustWindowRectExNative(&copy, style, menu ? 1 : 0, exStyle) != 0;
        rect = copy;
        return done;
    }

    public bool GetClientRect(nint window, out WindowRect rect)
    {
        WindowRect result;
        bool done = GetClientRectNative(window, &result) != 0;
        rect = result;
        return done;
    }

    public bool InvalidateRect(nint window, WindowRect? rect, bool erase)
    {
        if (rect is { } value)
        {
            return InvalidateRectNative(window, &value, erase ? 1 : 0) != 0;
        }

        return InvalidateRectNative(window, null, erase ? 1 : 0) != 0;
    }

    public nint GetWindowLong(nint window, int index, bool ansi = false) => IntPtr.Size == 4
        ? ansi ? GetWindowLongA(window, index) : GetWindowLongW(window, index)
        : ansi ? GetWindowLongPtrA(window, index) : GetWindowLongPtrW(window, index);

    public nint SetWindowLong(nint window, int index, nint value, bool ansi = false) => IntPtr.Size == 4
        ? ansi ? SetWindowLongA(window, index, (int)value) : SetWindowLongW(window, index, (int)value)
        : ansi ? SetWindowLongPtrA(window, index, value) : SetWindowLongPtrW(window, index, value);

    public bool PeekMessage(out WindowMessage message, nint window, uint filterMin, uint filterMax, uint remove, bool ansi = false)
    {
        WindowMessage result;
        bool got = (ansi ? PeekMessageA(&result, window, filterMin, filterMax, remove) : PeekMessageW(&result, window, filterMin, filterMax, remove)) != 0;
        message = result;
        return got;
    }

    public bool GetMessage(out WindowMessage message, nint window, uint filterMin, uint filterMax, bool ansi = false)
    {
        WindowMessage result;
        bool got = (ansi ? GetMessageA(&result, window, filterMin, filterMax) : GetMessageW(&result, window, filterMin, filterMax)) > 0;
        message = result;
        return got;
    }

    public bool WaitMessage() => WaitMessageNative() != 0;

    public bool TranslateMessage(in WindowMessage message)
    {
        WindowMessage copy = message;
        return TranslateMessageNative(&copy) != 0;
    }

    public nint DispatchMessage(in WindowMessage message, bool ansi = false)
    {
        WindowMessage copy = message;
        return ansi ? DispatchMessageA(&copy) : DispatchMessageW(&copy);
    }

    public void PostQuitMessage(int exitCode) => PostQuitMessageNative(exitCode);

    public bool PostMessage(nint window, uint message, nint wParam, nint lParam, bool ansi = false) =>
        (ansi ? PostMessageA(window, message, wParam, lParam) : PostMessageW(window, message, wParam, lParam)) != 0;

    public nint SendMessage(nint window, uint message, nint wParam, nint lParam, bool ansi = false) =>
        ansi ? SendMessageA(window, message, wParam, lParam) : SendMessageW(window, message, wParam, lParam);

    public nint DefWindowProc(nint window, uint message, nint wParam, nint lParam, bool ansi = false) =>
        ansi ? DefWindowProcA(window, message, wParam, lParam) : DefWindowProcW(window, message, wParam, lParam);

    public nuint SetTimer(nint window, nuint id, uint elapse, nint timerProcedure = 0) => SetTimerNative(window, id, elapse, timerProcedure);

    public bool KillTimer(nint window, nuint id) => KillTimerNative(window, id) != 0;

    public nint BeginPaint(nint window, out PaintInfo paint)
    {
        PaintInfo result;
        nint dc = BeginPaintNative(window, &result);
        paint = result;
        return dc;
    }

    public bool EndPaint(nint window, in PaintInfo paint)
    {
        PaintInfo copy = paint;
        return EndPaintNative(window, &copy) != 0;
    }

    public nint GetDC(nint window) => GetDCNative(window);

    public int ReleaseDC(nint window, nint dc) => ReleaseDCNative(window, dc);

    public int GetDeviceCaps(nint dc, int index) => GetDeviceCapsNative(dc, index);

    public nint GetStockObject(int index) => GetStockObjectNative(index);

    public short GetAsyncKeyState(int key) => GetAsyncKeyStateNative(key);

    public short GetKeyState(int key) => GetKeyStateNative(key);

    public bool SystemParametersInfo(uint action, uint parameter, nint data, uint winIni) => SystemParametersInfoW(action, parameter, data, winIni) != 0;

    public int GetSystemMetrics(int index) => GetSystemMetricsNative(index);

    public nint LoadCursor(nint instance, nint name) => LoadCursorW(instance, name);

    public nint LoadIcon(nint instance, nint name) => LoadIconW(instance, name);

    public int ShowCursor(bool show) => ShowCursorNative(show ? 1 : 0);

    /// <summary><c>WNDCLASSEX</c> for either character set; the menu is the name or <c>MAKEINTRESOURCE(id)</c>.</summary>
    private static WndClassEx Describe(WindowClass windowClass, nint className, nint menuName) => new()
    {
        Size = (uint)sizeof(WndClassEx),
        Style = windowClass.Style,
        WindowProcedure = windowClass.WindowProcedure,
        ClassExtra = windowClass.ClassExtraBytes,
        WindowExtra = windowClass.WindowExtraBytes,
        Instance = windowClass.Instance,
        Icon = windowClass.Icon,
        Cursor = windowClass.Cursor,
        Background = windowClass.Background,
        MenuName = menuName != 0 ? menuName : windowClass.MenuResourceId,
        ClassName = className,
        SmallIcon = windowClass.SmallIcon,
    };

    [StructLayout(LayoutKind.Sequential)]
    private struct WndClassEx
    {
        public uint Size;
        public uint Style;
        public nint WindowProcedure;
        public int ClassExtra;
        public int WindowExtra;
        public nint Instance;
        public nint Icon;
        public nint Cursor;
        public nint Background;
        public nint MenuName;
        public nint ClassName;
        public nint SmallIcon;
    }

    [LibraryImport("user32.dll")]
    private static partial ushort RegisterClassExW(WndClassEx* windowClass);

    [LibraryImport("user32.dll")]
    private static partial ushort RegisterClassExA(WndClassEx* windowClass);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateWindowExW(uint exStyle, string className, string? title, uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);

    [LibraryImport("user32.dll")]
    private static partial nint CreateWindowExA(uint exStyle, [MarshalAs(UnmanagedType.LPStr)] string className, [MarshalAs(UnmanagedType.LPStr)] string? title, uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);

    [LibraryImport("user32.dll", EntryPoint = "DestroyWindow")]
    private static partial int DestroyWindowNative(nint window);

    [LibraryImport("user32.dll", EntryPoint = "ShowWindow")]
    private static partial int ShowWindowNative(nint window, int command);

    [LibraryImport("user32.dll", EntryPoint = "UpdateWindow")]
    private static partial int UpdateWindowNative(nint window);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int SetWindowTextW(nint window, string text);

    [LibraryImport("user32.dll")]
    private static partial int SetWindowTextA(nint window, [MarshalAs(UnmanagedType.LPStr)] string text);

    [LibraryImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static partial int SetWindowPosNative(nint window, nint insertAfter, int x, int y, int width, int height, uint flags);

    [LibraryImport("user32.dll", EntryPoint = "AdjustWindowRectEx")]
    private static partial int AdjustWindowRectExNative(WindowRect* rect, uint style, int menu, uint exStyle);

    [LibraryImport("user32.dll", EntryPoint = "GetClientRect")]
    private static partial int GetClientRectNative(nint window, WindowRect* rect);

    [LibraryImport("user32.dll", EntryPoint = "InvalidateRect")]
    private static partial int InvalidateRectNative(nint window, WindowRect* rect, int erase);

    [LibraryImport("user32.dll")]
    private static partial nint GetWindowLongW(nint window, int index);

    [LibraryImport("user32.dll")]
    private static partial nint GetWindowLongA(nint window, int index);

    [LibraryImport("user32.dll")]
    private static partial nint GetWindowLongPtrW(nint window, int index);

    [LibraryImport("user32.dll")]
    private static partial nint GetWindowLongPtrA(nint window, int index);

    [LibraryImport("user32.dll")]
    private static partial nint SetWindowLongW(nint window, int index, int value);

    [LibraryImport("user32.dll")]
    private static partial nint SetWindowLongA(nint window, int index, int value);

    [LibraryImport("user32.dll")]
    private static partial nint SetWindowLongPtrW(nint window, int index, nint value);

    [LibraryImport("user32.dll")]
    private static partial nint SetWindowLongPtrA(nint window, int index, nint value);

    [LibraryImport("user32.dll")]
    private static partial int PeekMessageW(WindowMessage* message, nint window, uint filterMin, uint filterMax, uint remove);

    [LibraryImport("user32.dll")]
    private static partial int PeekMessageA(WindowMessage* message, nint window, uint filterMin, uint filterMax, uint remove);

    [LibraryImport("user32.dll")]
    private static partial int GetMessageW(WindowMessage* message, nint window, uint filterMin, uint filterMax);

    [LibraryImport("user32.dll")]
    private static partial int GetMessageA(WindowMessage* message, nint window, uint filterMin, uint filterMax);

    [LibraryImport("user32.dll", EntryPoint = "WaitMessage")]
    private static partial int WaitMessageNative();

    [LibraryImport("user32.dll", EntryPoint = "TranslateMessage")]
    private static partial int TranslateMessageNative(WindowMessage* message);

    [LibraryImport("user32.dll")]
    private static partial nint DispatchMessageW(WindowMessage* message);

    [LibraryImport("user32.dll")]
    private static partial nint DispatchMessageA(WindowMessage* message);

    [LibraryImport("user32.dll", EntryPoint = "PostQuitMessage")]
    private static partial void PostQuitMessageNative(int exitCode);

    [LibraryImport("user32.dll")]
    private static partial int PostMessageW(nint window, uint message, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    private static partial int PostMessageA(nint window, uint message, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    private static partial nint SendMessageW(nint window, uint message, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    private static partial nint SendMessageA(nint window, uint message, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    private static partial nint DefWindowProcW(nint window, uint message, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    private static partial nint DefWindowProcA(nint window, uint message, nint wParam, nint lParam);

    [LibraryImport("user32.dll", EntryPoint = "SetTimer")]
    private static partial nuint SetTimerNative(nint window, nuint id, uint elapse, nint timerProcedure);

    [LibraryImport("user32.dll", EntryPoint = "KillTimer")]
    private static partial int KillTimerNative(nint window, nuint id);

    [LibraryImport("user32.dll", EntryPoint = "BeginPaint")]
    private static partial nint BeginPaintNative(nint window, PaintInfo* paint);

    [LibraryImport("user32.dll", EntryPoint = "EndPaint")]
    private static partial int EndPaintNative(nint window, PaintInfo* paint);

    [LibraryImport("user32.dll", EntryPoint = "GetDC")]
    private static partial nint GetDCNative(nint window);

    [LibraryImport("user32.dll", EntryPoint = "ReleaseDC")]
    private static partial int ReleaseDCNative(nint window, nint dc);

    [LibraryImport("gdi32.dll", EntryPoint = "GetDeviceCaps")]
    private static partial int GetDeviceCapsNative(nint dc, int index);

    [LibraryImport("gdi32.dll", EntryPoint = "GetStockObject")]
    private static partial nint GetStockObjectNative(int index);

    [LibraryImport("user32.dll", EntryPoint = "GetAsyncKeyState")]
    private static partial short GetAsyncKeyStateNative(int key);

    [LibraryImport("user32.dll", EntryPoint = "GetKeyState")]
    private static partial short GetKeyStateNative(int key);

    [LibraryImport("user32.dll")]
    private static partial int SystemParametersInfoW(uint action, uint parameter, nint data, uint winIni);

    [LibraryImport("user32.dll", EntryPoint = "GetSystemMetrics")]
    private static partial int GetSystemMetricsNative(int index);

    [LibraryImport("user32.dll")]
    private static partial nint LoadCursorW(nint instance, nint name);

    [LibraryImport("user32.dll")]
    private static partial nint LoadIconW(nint instance, nint name);

    [LibraryImport("user32.dll", EntryPoint = "ShowCursor")]
    private static partial int ShowCursorNative(int show);
}
