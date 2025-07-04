using System.Runtime.InteropServices;

public static class JSL
{
    [StructLayout(LayoutKind.Sequential)]
    public struct State
    {
        public int buttons;
        public float lTrigger;
        public float rTrigger;
        public float stickLX;
        public float stickLY;
        public float stickRX;
        public float stickRY;
    }

    public enum Button
    {
        Up = 1,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        Plus = 1 << 4,
        Options = 1 << 4,
        Minus = 1 << 5,
        Share = 1 << 5,
        LClick = 1 << 6,
        RClick = 1 << 7,
        L = 1 << 8,
        R = 1 << 9,
        ZL = 1 << 10,
        ZR = 1 << 11,
        S = 1 << 12,
        E = 1 << 13,
        W = 1 << 14,
        N = 1 << 15,
        Home = 1 << 16,
        PS = 1 << 16,
        Capture = 1 << 17,
        TouchpadClick = 1 << 17,
        SL = 1 << 18,
        SR = 1 << 19
    }

    private const string Library = "JoyShockLibrary";

    [DllImport(Library, EntryPoint = "JslConnectDevices")]
    public static extern int ConnectDevices();
    [DllImport("JoyShockLibrary", EntryPoint = "JslDisconnectAndDisposeAll")]
    public static extern void DisconnectAndDisposeAll();
    [DllImport(Library, EntryPoint = "JslStillConnected")]
    public static extern bool StillConnected(int deviceId);
    [DllImport(Library, EntryPoint = "JslGetConnectedDeviceHandles")]
    public static extern int GetConnectedDeviceHandles(int[] deviceHandleArray, int size);
    [DllImport(Library, EntryPoint = "JslGetSimpleState", CallingConvention = CallingConvention.Cdecl)]
    public static extern State GetSimpleState(int deviceId);
    [DllImport(Library, EntryPoint = "JslSetRumble")]
    public static extern void SetRumble(int deviceId, int smallRumble, int bigRumble);
}
