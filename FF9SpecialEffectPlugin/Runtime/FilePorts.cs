namespace Recompiler.Runtime;

/// <summary>The real file system (the default of <see cref="Ports.Files"/>); paths resolve against the process working directory.</summary>
public sealed class PhysicalFileSystem : IFileSystem
{
    public bool FileExists(string path) => File.Exists(path);
    public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);
    public void WriteAllBytes(string path, ReadOnlySpan<byte> data) => File.WriteAllBytes(path, data);
    public Stream OpenRead(string path) => File.OpenRead(path);
    public Stream Create(string path) => File.Create(path);
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);
    public IEnumerable<string> EnumerateFiles(string directory, string pattern) => Directory.EnumerateFiles(directory, pattern);
    public Stream Open(string path, FileMode mode, FileAccess access, FileShare share) => new FileStream(path, mode, access, share);
    public string CurrentDirectory => Directory.GetCurrentDirectory();
    public void SetCurrentDirectory(string path) => Directory.SetCurrentDirectory(path);

    public bool DeleteFile(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        File.Delete(path);
        return true;
    }

    /// <summary><c>GetDriveType</c> through <see cref="DriveInfo"/> (whose <see cref="System.IO.DriveType"/> numbers are the <c>DRIVE_*</c> values).</summary>
    public uint DriveType(string? root)
    {
        string? drive = DriveTypes.RootOf(root, CurrentDirectory);
        if (drive is null)
        {
            return DriveTypes.NoRootDirectory;
        }

        try
        {
            return (uint)new DriveInfo(drive).DriveType;
        }
        catch (Exception exception) when (exception is ArgumentException or IOException or UnauthorizedAccessException)
        {
            return DriveTypes.NoRootDirectory;
        }
    }
}

/// <summary>
/// A file system rooted in a directory of the host (a test's temporary folder): every program path, relative or absolute
/// with a drive, resolves below <see cref="Root"/>, and the working directory is virtual (<c>_getcwd</c> reports
/// <see cref="CurrentDirectory"/>, <c>C:\</c> by default), so ported code sees the paths it expects and tests stay isolated.
/// </summary>
public sealed class RootedFileSystem : IFileSystem
{
    public RootedFileSystem(string root, string currentDirectory = "C:\\")
    {
        Root = Path.GetFullPath(root);
        CurrentDirectory = currentDirectory;
    }

    public string Root { get; }

    public string CurrentDirectory { get; private set; }

    public void SetCurrentDirectory(string path) => CurrentDirectory = Combine(CurrentDirectory, path);

    public bool FileExists(string path) => File.Exists(Map(path));
    public byte[] ReadAllBytes(string path) => File.ReadAllBytes(Map(path));
    public void WriteAllBytes(string path, ReadOnlySpan<byte> data) => File.WriteAllBytes(Map(path), data);
    public Stream OpenRead(string path) => File.OpenRead(Map(path));
    public Stream Create(string path) => File.Create(Map(path));
    public void CreateDirectory(string path) => Directory.CreateDirectory(Map(path));
    public IEnumerable<string> EnumerateFiles(string directory, string pattern) =>
        Directory.EnumerateFiles(Map(directory), pattern).Select(f => Path.Combine(directory, Path.GetFileName(f)));
    public Stream Open(string path, FileMode mode, FileAccess access, FileShare share) => new FileStream(Map(path), mode, access, share);

    public bool DeleteFile(string path)
    {
        string mapped = Map(path);
        if (!File.Exists(mapped))
        {
            return false;
        }

        File.Delete(mapped);
        return true;
    }

    /// <summary>What <see cref="DriveType"/> reports for a root not listed in <see cref="Drives"/> (<see cref="DriveTypes.Fixed"/> by default).</summary>
    public uint DefaultDriveType { get; set; } = DriveTypes.Fixed;

    /// <summary>Scripted drive types by root (<c>D:\</c> = <see cref="DriveTypes.CdRom"/> for a game that looks for its CD), case-insensitive.</summary>
    public Dictionary<string, uint> Drives { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary><c>GetDriveType</c> of the virtual drives: <see cref="Drives"/>, else <see cref="DefaultDriveType"/>; a path that is no root is <c>DRIVE_NO_ROOT_DIR</c>.</summary>
    public uint DriveType(string? root)
    {
        string? drive = DriveTypes.RootOf(root, CurrentDirectory);
        if (drive is null)
        {
            return DriveTypes.NoRootDirectory;
        }

        return Drives.TryGetValue(drive, out uint type) ? type : DefaultDriveType;
    }

    /// <summary>The host path of a program path.</summary>
    public string Map(string path)
    {
        string full = Combine(CurrentDirectory, path);
        string withoutDrive = full.Length >= 2 && full[1] == ':' ? full[2..] : full;
        string relative = withoutDrive.Replace('/', '\\').TrimStart('\\');
        string mapped = Path.GetFullPath(Path.Combine(Root, relative.Replace('\\', Path.DirectorySeparatorChar)));
        if (!mapped.StartsWith(Root, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException($"'{path}' leaves the root {Root}.");
        }

        return mapped;
    }

    private static string Combine(string current, string path)
    {
        string normalized = path.Replace('/', '\\');
        if (normalized.Length >= 2 && normalized[1] == ':')
        {
            return normalized;
        }

        if (normalized.StartsWith('\\'))
        {
            return (current.Length >= 2 && current[1] == ':' ? current[..2] : string.Empty) + normalized;
        }

        return current.TrimEnd('\\') + "\\" + normalized;
    }
}
