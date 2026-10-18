namespace Recompiler.Runtime.Com;

/// <summary>Common <c>HRESULT</c> values for COM-style exports (e.g. 7-Zip's <c>CreateObject</c>).</summary>
public static class Hresult
{
    public const int S_OK = 0;
    public const int S_FALSE = 1;
    public const int E_NOTIMPL = unchecked((int)0x80004001);
    public const int E_NOINTERFACE = unchecked((int)0x80004002);
    public const int E_POINTER = unchecked((int)0x80004003);
    public const int E_ABORT = unchecked((int)0x80004004);
    public const int E_FAIL = unchecked((int)0x80004005);
    public const int E_UNEXPECTED = unchecked((int)0x8000FFFF);
    public const int E_ACCESSDENIED = unchecked((int)0x80070005);
    public const int E_HANDLE = unchecked((int)0x80070006);
    public const int E_OUTOFMEMORY = unchecked((int)0x8007000E);
    public const int E_INVALIDARG = unchecked((int)0x80070057);
    public const int CLASS_E_CLASSNOTAVAILABLE = unchecked((int)0x80040111);
    public const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);

    public static bool Succeeded(int hr) => hr >= 0;
    public static bool Failed(int hr) => hr < 0;

    /// <summary><c>HRESULT_FROM_WIN32</c>.</summary>
    public static int FromWin32(int error) =>
        error <= 0 ? error : unchecked((int)((error & 0x0000FFFF) | 0x80070000));
}
