namespace Recompiler.Runtime;

/// <summary>Source of time for rewritten logic.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }

    /// <summary>Milliseconds since an arbitrary origin (<c>GetTickCount</c>/<c>timeGetTime</c> semantics; cast to <c>uint</c> for the DWORD).</summary>
    long TickCount { get; }

    /// <summary>A high-resolution counter (<c>QueryPerformanceCounter</c>, <c>rdtsc</c> loops) in <see cref="HighResolutionFrequency"/> units per second.</summary>
    long HighResolutionTicks => TickCount * 10_000;

    /// <summary>Counts per second of <see cref="HighResolutionTicks"/> (<c>QueryPerformanceFrequency</c>).</summary>
    long HighResolutionFrequency => 10_000_000;

    /// <summary>
    /// <c>Sleep(ms)</c>: the system clock blocks the calling thread (<c>INFINITE</c>, <c>0xFFFFFFFF</c>, forever; 0 yields),
    /// <see cref="DeterministicClock"/> never blocks and advances its time by <paramref name="milliseconds"/> instead, so a
    /// polling loop around <c>Sleep</c> sees time pass in tests. The default blocks like the system clock.
    /// </summary>
    void Sleep(uint milliseconds) => SystemClock.Block(milliseconds);
}

/// <summary>Source of randomness for rewritten logic; seedable so runs can be reproduced.</summary>
public interface IRandomSource
{
    void Seed(uint seed);

    /// <summary>Next value in <c>[0, int.MaxValue)</c>.</summary>
    int Next();

    uint NextUInt32();
}

/// <summary>Standard output and error of the program.</summary>
public interface IConsoleSink
{
    void Write(string text);
    void WriteError(string text);
}

/// <summary>The file system as the program sees it; paths are the program's own (relative to its working directory).</summary>
public interface IFileSystem
{
    bool FileExists(string path);
    byte[] ReadAllBytes(string path);
    void WriteAllBytes(string path, ReadOnlySpan<byte> data);
    Stream OpenRead(string path);
    Stream Create(string path);
    void CreateDirectory(string path);
    IEnumerable<string> EnumerateFiles(string directory, string pattern);

    /// <summary>
    /// Opens a stream with explicit mode, access and sharing (what <c>_open</c>/<c>_sopen</c>/<c>CreateFile</c> need);
    /// the default covers reading an existing file and creating a new one.
    /// </summary>
    Stream Open(string path, FileMode mode, FileAccess access, FileShare share) => (mode, access) switch
    {
        (FileMode.Open, FileAccess.Read) => OpenRead(path),
        (FileMode.Create or FileMode.CreateNew or FileMode.Truncate, _) => Create(path),
        _ => throw new NotSupportedException($"{GetType().Name} cannot open '{path}' with {mode}/{access}."),
    };

    /// <summary>The program's working directory (<c>_getcwd</c>, <c>GetCurrentDirectory</c>).</summary>
    string CurrentDirectory => ".";

    /// <summary>Changes the working directory (<c>_chdir</c>, <c>SetCurrentDirectory</c>).</summary>
    void SetCurrentDirectory(string path) => throw new NotSupportedException($"{GetType().Name} has a fixed working directory.");

    /// <summary>Deletes a file (<c>remove</c>, <c>_unlink</c>, <c>DeleteFile</c>); false when it did not exist.</summary>
    bool DeleteFile(string path) => throw new NotSupportedException($"{GetType().Name} cannot delete files.");

    /// <summary>
    /// <c>GetDriveType(root)</c>: the <see cref="DriveTypes"/> value (<c>DRIVE_*</c>) of a root such as <c>C:\</c>;
    /// <c>null</c> means the root of <see cref="CurrentDirectory"/>, a path that is not a root is <see cref="DriveTypes.NoRootDirectory"/>.
    /// The default reports every drive as <see cref="DriveTypes.Fixed"/>.
    /// </summary>
    uint DriveType(string? root) => DriveTypes.Fixed;
}

/// <summary>The <c>DRIVE_*</c> values of <c>GetDriveType</c> (<c>winbase.h</c>); they equal the <see cref="System.IO.DriveType"/> members.</summary>
public static class DriveTypes
{
    public const uint Unknown = 0;
    public const uint NoRootDirectory = 1;
    public const uint Removable = 2;
    public const uint Fixed = 3;
    public const uint Remote = 4;
    public const uint CdRom = 5;
    public const uint RamDisk = 6;

    /// <summary>
    /// The drive root a <c>GetDriveType</c> argument names, upper-cased (<c>C:\</c>, or <c>\\server\share\</c>); <c>null</c>
    /// for anything else (the API needs the trailing backslash). A <c>null</c> path means the root of <paramref name="currentDirectory"/>.
    /// </summary>
    public static string? RootOf(string? path, string currentDirectory)
    {
        string candidate = (path ?? currentDirectory).Replace('/', '\\');
        if (path is null)
        {
            return candidate.Length >= 2 && char.IsAsciiLetter(candidate[0]) && candidate[1] == ':' ? char.ToUpperInvariant(candidate[0]) + ":\\" : null;
        }

        if (candidate.Length == 3 && char.IsAsciiLetter(candidate[0]) && candidate[1] == ':' && candidate[2] == '\\')
        {
            return char.ToUpperInvariant(candidate[0]) + ":\\";
        }

        bool unc = candidate.Length > 4 && candidate.StartsWith(@"\\", StringComparison.Ordinal) && candidate.EndsWith('\\')
            && candidate[2..^1].Split('\\') is [{ Length: > 0 }, { Length: > 0 }];
        return unc ? candidate.ToUpperInvariant() : null;
    }
}

/// <summary>Predefined registry roots (<c>HKEY_*</c>).</summary>
public enum RegistryRoot
{
    ClassesRoot,
    CurrentUser,
    LocalMachine,
    Users,
    CurrentConfig,
}

/// <summary>
/// The Windows registry as the program sees it: string, DWORD and binary values under <c>root\key</c>. Keys use
/// backslashes as in the original (<c>Software\Vendor\App</c>); a missing key or value reads as <c>null</c>.
/// </summary>
public interface IRegistry
{
    string? ReadString(RegistryRoot root, string key, string name);
    uint? ReadDword(RegistryRoot root, string key, string name);
    void WriteString(RegistryRoot root, string key, string name, string value);
    void WriteDword(RegistryRoot root, string key, string name, uint value);
    bool DeleteValue(RegistryRoot root, string key, string name);
    bool KeyExists(RegistryRoot root, string key);

    /// <summary>A <c>REG_BINARY</c> value (a 16-byte GUID, a packed struct), or <c>null</c>.</summary>
    byte[]? ReadBinary(RegistryRoot root, string key, string name) => null;

    void WriteBinary(RegistryRoot root, string key, string name, ReadOnlySpan<byte> value) => throw new NotSupportedException($"{GetType().Name} cannot write binary values.");

    /// <summary>The names of the sub-keys of <paramref name="key"/> in the order <c>RegEnumKeyEx</c> returns them (sorted here); empty when the key is missing.</summary>
    IReadOnlyList<string> SubKeyNames(RegistryRoot root, string key) => [];

    /// <summary>The value names of <paramref name="key"/> (<c>RegEnumValue</c>), sorted; empty when the key is missing.</summary>
    IReadOnlyList<string> ValueNames(RegistryRoot root, string key) => [];

    /// <summary>
    /// A value as <c>RegQueryValueExW</c> returns it: its kind (<c>REG_SZ</c>, <c>REG_DWORD</c>, <c>REG_BINARY</c>, ...) and raw
    /// bytes (strings UTF-16LE with their terminating NUL, <see cref="RegistryValue.AnsiData"/> for the A form); <c>null</c>
    /// when the key or value is missing.
    /// </summary>
    RegistryValue? ReadValue(RegistryRoot root, string key, string name) => null;
}

/// <summary>The <c>REG_*</c> value types, numbered as in <c>winnt.h</c>.</summary>
public enum RegistryValueType
{
    None = 0,
    String = 1,
    ExpandString = 2,
    Binary = 3,
    Dword = 4,
    DwordBigEndian = 5,
    Link = 6,
    MultiString = 7,
    ResourceList = 8,
    Qword = 11,
}

/// <summary>One registry value with its type and the bytes <c>RegQueryValueExW</c> copies out (<c>lpType</c>, <c>lpData</c>, <c>lpcbData</c>).</summary>
public sealed record RegistryValue(RegistryValueType Kind, byte[] Data)
{
    /// <summary>The bytes <c>RegQueryValueExA</c> copies out: string kinds converted from UTF-16 to <paramref name="encoding"/> (Latin-1 when <c>null</c>, the code page CFormat uses) with their NULs; other kinds unchanged.</summary>
    public byte[] AnsiData(System.Text.Encoding? encoding = null)
    {
        if (Kind is not (RegistryValueType.String or RegistryValueType.ExpandString or RegistryValueType.MultiString))
        {
            return (byte[])Data.Clone();
        }

        return (encoding ?? System.Text.Encoding.Latin1).GetBytes(System.Text.Encoding.Unicode.GetString(Data));
    }

    /// <summary>A <c>REG_SZ</c> value: UTF-16LE text and its terminating NUL.</summary>
    public static RegistryValue FromString(string text, RegistryValueType kind = RegistryValueType.String) => new(kind, System.Text.Encoding.Unicode.GetBytes(text + "\0"));

    /// <summary>A <c>REG_MULTI_SZ</c> value: each string NUL-terminated, then a final NUL.</summary>
    public static RegistryValue FromStrings(IEnumerable<string> texts) => new(RegistryValueType.MultiString, System.Text.Encoding.Unicode.GetBytes(string.Concat(texts.Select(t => t + "\0")) + "\0"));

    public static RegistryValue FromDword(uint value) => new(RegistryValueType.Dword, BitConverter.GetBytes(value));

    public static RegistryValue FromQword(ulong value) => new(RegistryValueType.Qword, BitConverter.GetBytes(value));
}

/// <summary>
/// Minimal user-interface surface of a program: a message box, a yes/no question and a non-blocking notification
/// (balloon, status text, beep). The shell wires it to Win32; tests record the calls and script the answers.
/// </summary>
public interface IUserInterface
{
    void MessageBox(string text, string caption);
    bool Confirm(string text, string caption);
    void Notify(string text);

    /// <summary>
    /// <c>MessageBox(owner, text, caption, flags)</c> as the original calls it: returns the button id (<c>IDOK</c> 1,
    /// <c>IDCANCEL</c> 2, <c>IDYES</c> 6, <c>IDNO</c> 7). The default maps yes/no and OK/cancel boxes onto <see cref="Confirm"/>.
    /// </summary>
    int MessageBox(nint owner, string text, string caption, uint flags)
    {
        switch (flags & 0xF)
        {
            case MessageBoxButtons.YesNo:
            case MessageBoxButtons.YesNoCancel:
                return Confirm(text, caption) ? MessageBoxButtons.IdYes : MessageBoxButtons.IdNo;
            case MessageBoxButtons.OkCancel:
            case MessageBoxButtons.RetryCancel:
                return Confirm(text, caption) ? ((flags & 0xF) == MessageBoxButtons.OkCancel ? MessageBoxButtons.IdOk : MessageBoxButtons.IdRetry) : MessageBoxButtons.IdCancel;
            default:
                MessageBox(text, caption);
                return MessageBoxButtons.IdOk;
        }
    }
}

/// <summary>The <c>MB_*</c> button groups and <c>ID*</c> results of <c>MessageBox</c>.</summary>
public static class MessageBoxButtons
{
    public const uint Ok = 0x0;
    public const uint OkCancel = 0x1;
    public const uint AbortRetryIgnore = 0x2;
    public const uint YesNoCancel = 0x3;
    public const uint YesNo = 0x4;
    public const uint RetryCancel = 0x5;
    public const int IdOk = 1;
    public const int IdCancel = 2;
    public const int IdAbort = 3;
    public const int IdRetry = 4;
    public const int IdIgnore = 5;
    public const int IdYes = 6;
    public const int IdNo = 7;
}

/// <summary>
/// The outcome of <see cref="IProcessLauncher.Start(string, string?, bool, string?)"/>: whether the program started, the exit code
/// (waited) or process id (not waited), and on failure the Win32 error code <c>GetLastError</c> would have returned
/// (<c>ERROR_FILE_NOT_FOUND</c> 2, <c>ERROR_PATH_NOT_FOUND</c> 3, <c>ERROR_ACCESS_DENIED</c> 5, ...) with the exception message.
/// </summary>
public sealed record ProcessStartResult(bool Started, int Value, int Win32Error, string? Error)
{
    public static ProcessStartResult Exited(int exitCode) => new(true, exitCode, 0, null);

    public static ProcessStartResult Running(int processId) => new(true, processId, 0, null);

    public static ProcessStartResult Failed(int win32Error, string? error) => new(false, -1, win32Error, error);
}

/// <summary>Starting other programs (<c>CreateProcess</c>, <c>ShellExecute</c>, <c>WinExec</c>, <c>_spawn*</c>, <c>system</c>).</summary>
public interface IProcessLauncher
{
    /// <summary>
    /// Starts <paramref name="path"/> with <paramref name="arguments"/> in <paramref name="workingDirectory"/> (the caller's when
    /// <c>null</c>, <c>lpCurrentDirectory</c>); with <paramref name="wait"/> blocks for the exit code, otherwise the result carries
    /// the process id. A program that cannot be started yields the Win32 error code and the reason.
    /// </summary>
    ProcessStartResult Start(string path, string? arguments, bool wait, string? workingDirectory);

    /// <summary>The short form: the exit code (waited) or process id, -1 when the program cannot be started.</summary>
    int Start(string path, string? arguments, bool wait) => Start(path, arguments, wait, null) is { Started: true } started ? started.Value : -1;

    /// <summary>
    /// <c>SetPriorityClass(GetCurrentProcess(), priorityClass)</c> with a <see cref="PriorityClasses"/> value; false (the BOOL
    /// FALSE of the original) for an unknown class or when the system refuses. The default accepts and ignores the call.
    /// </summary>
    bool SetPriorityClass(uint priorityClass) => PriorityClasses.IsKnown(priorityClass);

    /// <summary>
    /// <c>SetThreadPriority(GetCurrentThread(), priority)</c> for the calling thread with a <c>THREAD_PRIORITY_*</c> value
    /// (-15, -2..2, 15); false for any other value. The default accepts and ignores the call.
    /// </summary>
    bool SetThreadPriority(int priority) => PriorityClasses.IsKnownThreadPriority(priority);
}

/// <summary>The <c>*_PRIORITY_CLASS</c> values of <c>SetPriorityClass</c> and the <c>THREAD_PRIORITY_*</c> values of <c>SetThreadPriority</c>.</summary>
public static class PriorityClasses
{
    public const uint Idle = 0x40;
    public const uint BelowNormal = 0x4000;
    public const uint Normal = 0x20;
    public const uint AboveNormal = 0x8000;
    public const uint High = 0x80;
    public const uint Realtime = 0x100;

    public const int ThreadIdle = -15;
    public const int ThreadLowest = -2;
    public const int ThreadBelowNormal = -1;
    public const int ThreadNormal = 0;
    public const int ThreadAboveNormal = 1;
    public const int ThreadHighest = 2;
    public const int ThreadTimeCritical = 15;

    public static bool IsKnown(uint priorityClass) => priorityClass is Idle or BelowNormal or Normal or AboveNormal or High or Realtime;

    public static bool IsKnownThreadPriority(int priority) => priority is ThreadIdle or (>= ThreadLowest and <= ThreadHighest) or ThreadTimeCritical;
}

/// <summary>
/// The side-effect boundary of rewritten programs. Core (business-logic) code never touches
/// <c>DateTime.Now</c>, <c>Random</c>, <c>Console</c>, <c>File</c>, <c>Registry</c>, <c>Process</c>, windows or
/// <c>MessageBox</c> directly (nor P/Invoke); it goes through these ports, which default to the real world and which tests
/// replace by deterministic fakes (<see cref="UseDeterministic"/>).
/// </summary>
public static partial class Ports
{
    public static IClock Clock { get; set; } = new SystemClock();
    public static IRandomSource Random { get; set; } = new CrtRandom();
    public static IConsoleSink Console { get; set; } = new SystemConsole();
    public static IFileSystem Files { get; set; } = new PhysicalFileSystem();
    public static IRegistry Registry { get; set; } = DefaultRegistry();
    public static IUserInterface Ui { get; set; } = DefaultUi();

    /// <summary>Windows and the message loop (<c>RegisterClass</c>, <c>CreateWindowEx</c>, <c>PeekMessage</c>, ...).</summary>
    public static IWindowSystem Windows { get; set; } = DefaultWindows();

    public static IProcessLauncher Process { get; set; } = new SystemProcessLauncher();

    /// <summary>
    /// Deterministic time (starts at <paramref name="now"/> and advances by <see cref="DeterministicClock.DefaultQuantum"/> on
    /// every read, so busy-wait loops terminate; <c>Sleep</c> advances it without blocking), seeded randomness, an in-memory registry, a recording UI (every question
    /// answered <paramref name="confirmAnswer"/>), recording windows and process launcher: two runs with the same inputs
    /// produce the same outputs.
    /// </summary>
    public static void UseDeterministic(uint seed, DateTimeOffset now, bool confirmAnswer = true)
    {
        Clock = new DeterministicClock(now);
        var random = new CrtRandom();
        random.Seed(seed);
        Random = random;
        Registry = new InMemoryRegistry();
        Ui = new RecordingUi { ConfirmAnswer = confirmAnswer };
        Windows = new RecordingWindows();
        Process = new RecordingProcessLauncher();
    }

    private static IRegistry DefaultRegistry()
    {
        if (OperatingSystem.IsWindows())
        {
            return new WindowsRegistry();
        }

        return new InMemoryRegistry();
    }

    private static IUserInterface DefaultUi()
    {
        if (OperatingSystem.IsWindows())
        {
            return new Win32Ui();
        }

        return new RecordingUi();
    }

    private static IWindowSystem DefaultWindows()
    {
        if (OperatingSystem.IsWindows())
        {
            return new Win32Windows();
        }

        return new RecordingWindows();
    }

    /// <summary>The MSVC CRT <c>rand()</c> generator, so ported code that depended on its sequence stays bit-exact after <c>srand</c>.</summary>
    private sealed class CrtRandom : IRandomSource
    {
        private uint _state = 1;

        public void Seed(uint seed) => _state = seed;

        public int Next()
        {
            _state = unchecked(_state * 214013u + 2531011u);
            return (int)((_state >> 16) & 0x7FFF);
        }

        public uint NextUInt32() => (uint)(Next() << 17 | Next() << 2 | (Next() & 3));
    }

    private sealed class SystemConsole : IConsoleSink
    {
        public void Write(string text) => System.Console.Out.Write(text);
        public void WriteError(string text) => System.Console.Error.Write(text);
    }
}
