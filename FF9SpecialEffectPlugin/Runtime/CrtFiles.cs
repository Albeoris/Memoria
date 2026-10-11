using System.Text;

namespace Recompiler.Runtime;

/// <summary>
/// The MSVC CRT low-level I/O layer (<c>_open</c>, <c>_sopen</c>, <c>_read</c>, <c>_write</c>, <c>_lseek</c>,
/// <c>_lseeki64</c>, <c>_close</c>, <c>_filelength</c>, <c>_tell</c>, <c>_eof</c>, <c>_commit</c>) over <see cref="Ports.Files"/> streams:
/// one process-wide table of small integer handles, shared by every unit, so a handle opened in one function and read in
/// another behaves as in the original. Handles 0-2 are stdin/stdout/stderr (writes go to <see cref="Ports.Console"/>); files
/// get the lowest free handle from 3 up, like the CRT. Failures return -1 and set <see cref="Errno"/> and
/// <see cref="DosErrno"/> to the CRT's values. Text mode (the CRT default without <c>_O_BINARY</c>) translates CR LF to LF
/// and stops at Ctrl-Z on read, LF to CR LF on write.
/// </summary>
public static unsafe class CrtFiles
{
    // _O_* open flags (fcntl.h).
    public const int O_RDONLY = 0x0000;
    public const int O_WRONLY = 0x0001;
    public const int O_RDWR = 0x0002;
    public const int O_APPEND = 0x0008;
    public const int O_CREAT = 0x0100;
    public const int O_TRUNC = 0x0200;
    public const int O_EXCL = 0x0400;
    public const int O_TEXT = 0x4000;
    public const int O_BINARY = 0x8000;
    public const int O_NOINHERIT = 0x0080;
    public const int O_TEMPORARY = 0x0040;
    public const int O_SEQUENTIAL = 0x0020;
    public const int O_RANDOM = 0x0010;

    // _S_* permission flags (sys/stat.h): only _S_IWRITE matters on Windows (read-only file without it).
    public const int S_IREAD = 0x0100;
    public const int S_IWRITE = 0x0080;

    // _SH_* sharing modes (share.h).
    public const int SH_DENYRW = 0x10;
    public const int SH_DENYWR = 0x20;
    public const int SH_DENYRD = 0x30;
    public const int SH_DENYNO = 0x40;

    // SEEK_* origins.
    public const int SEEK_SET = 0;
    public const int SEEK_CUR = 1;
    public const int SEEK_END = 2;

    // errno values (errno.h) and the Win32 errors the CRT keeps in _doserrno.
    public const int ENOENT = 2;
    public const int EBADF = 9;
    public const int EACCES = 13;
    public const int EEXIST = 17;
    public const int EINVAL = 22;
    public const int EMFILE = 24;
    public const int ENOSPC = 28;
    public const int ERANGE = 34;
    public const uint ERROR_FILE_NOT_FOUND = 2;
    public const uint ERROR_PATH_NOT_FOUND = 3;
    public const uint ERROR_TOO_MANY_OPEN_FILES = 4;
    public const uint ERROR_ACCESS_DENIED = 5;
    public const uint ERROR_INVALID_HANDLE = 6;
    public const uint ERROR_SHARING_VIOLATION = 32;
    public const uint ERROR_FILE_EXISTS = 80;
    public const uint ERROR_NEGATIVE_SEEK = 131;

    /// <summary>The CRT's handle limit (<c>_NHANDLE_</c> of the static CRT is 2048).</summary>
    public const int MaxHandles = 2048;

    private const int FirstFileHandle = 3;
    private static readonly object Gate = new();
    private static readonly Dictionary<int, OpenFile> Table = [];

    /// <summary><c>errno</c> after the last failing call.</summary>
    public static int Errno { get; set; }

    /// <summary><c>_doserrno</c> after the last failing call (the Win32 error behind <see cref="Errno"/>).</summary>
    public static uint DosErrno { get; set; }

    /// <summary>The translation mode for opens that name neither <c>_O_TEXT</c> nor <c>_O_BINARY</c> (<c>_fmode</c>; text by default).</summary>
    public static int DefaultMode { get; set; } = O_TEXT;

    /// <summary>The 8-bit encoding of paths and console text (the original's ANSI code page; Latin-1 keeps every byte).</summary>
    public static Encoding PathEncoding { get; set; } = Encoding.Latin1;

    /// <summary><c>_open(path, oflag, pmode)</c>.</summary>
    public static int Open(byte* path, int oflag, int pmode = S_IREAD | S_IWRITE) =>
        path is null ? Fail(EINVAL, 0) : Open(NativeString.ToString(path, PathEncoding)!, oflag, SH_DENYNO, pmode);

    /// <summary><c>_sopen(path, oflag, shflag, pmode)</c>.</summary>
    public static int SOpen(byte* path, int oflag, int shflag, int pmode = S_IREAD | S_IWRITE) =>
        path is null ? Fail(EINVAL, 0) : Open(NativeString.ToString(path, PathEncoding)!, oflag, shflag, pmode);

    /// <summary>Opens <paramref name="path"/> with CRT flags; returns the handle or -1.</summary>
    public static int Open(string path, int oflag, int shflag = SH_DENYNO, int pmode = S_IREAD | S_IWRITE)
    {
        FileAccess access = (oflag & 3) switch
        {
            O_WRONLY => FileAccess.Write,
            O_RDWR => FileAccess.ReadWrite,
            _ => FileAccess.Read,
        };
        bool create = (oflag & O_CREAT) != 0;
        bool truncate = (oflag & O_TRUNC) != 0;
        FileMode mode = (create, (oflag & O_EXCL) != 0, truncate) switch
        {
            (true, true, _) => FileMode.CreateNew,
            (true, false, true) => FileMode.Create,
            (true, false, false) => FileMode.OpenOrCreate,
            (false, _, true) => FileMode.Truncate,
            _ => FileMode.Open,
        };
        if (access == FileAccess.Read && mode is FileMode.Truncate or FileMode.Create)
        {
            access = FileAccess.ReadWrite; // the CRT opens for writing to truncate; reads still work
        }

        FileShare share = shflag switch
        {
            SH_DENYRW => FileShare.None,
            SH_DENYWR => FileShare.Read,
            SH_DENYRD => FileShare.Write,
            _ => FileShare.ReadWrite,
        };

        lock (Gate)
        {
            int handle = FirstFileHandle;
            while (Table.ContainsKey(handle))
            {
                handle++;
            }

            if (handle >= MaxHandles)
            {
                return Fail(EMFILE, ERROR_TOO_MANY_OPEN_FILES);
            }

            Stream stream;
            try
            {
                stream = Ports.Files.Open(path, mode, access, share);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or NotSupportedException or ArgumentException)
            {
                (int errno, uint doserrno) = Map(exception);
                return Fail(errno, doserrno);
            }

            bool text = (oflag & O_BINARY) == 0 && ((oflag & O_TEXT) != 0 || (DefaultMode & O_TEXT) != 0);
            Table[handle] = new OpenFile(stream, text, (oflag & O_APPEND) != 0, path);
            return handle;
        }
    }

    /// <summary><c>_read(fd, buffer, count)</c>: bytes read (after text translation), 0 at end of file, -1 on error.</summary>
    public static int Read(int fd, void* buffer, uint count)
    {
        lock (Gate)
        {
            if (!TryGet(fd, out OpenFile? file) || !file.Stream.CanRead)
            {
                return Fail(EBADF, ERROR_INVALID_HANDLE);
            }

            if (count == 0 || file.AtCtrlZ)
            {
                return 0;
            }

            if (count > int.MaxValue)
            {
                return Fail(EINVAL, 0);
            }

            var target = new Span<byte>(buffer, (int)count);
            int read = ReadFully(file.Stream, target);
            return file.Text ? TranslateRead(file, target, read) : read;
        }
    }

    /// <summary><c>_write(fd, buffer, count)</c>: the number of bytes of <paramref name="buffer"/> written (before text translation), -1 on error.</summary>
    public static int Write(int fd, void* buffer, uint count)
    {
        if (count > int.MaxValue)
        {
            return Fail(EINVAL, 0);
        }

        var source = new ReadOnlySpan<byte>(buffer, (int)count);
        if (fd is 1 or 2)
        {
            string text = PathEncoding.GetString(source);
            if (fd == 1)
            {
                Ports.Console.Write(text);
            }
            else
            {
                Ports.Console.WriteError(text);
            }

            return (int)count;
        }

        lock (Gate)
        {
            if (!TryGet(fd, out OpenFile? file) || !file.Stream.CanWrite)
            {
                return Fail(EBADF, ERROR_INVALID_HANDLE);
            }

            try
            {
                if (file.Append)
                {
                    file.Stream.Seek(0, SeekOrigin.End);
                }

                if (!file.Text)
                {
                    file.Stream.Write(source);
                }
                else
                {
                    foreach (byte value in source)
                    {
                        if (value == (byte)'\n')
                        {
                            file.Stream.WriteByte((byte)'\r');
                        }

                        file.Stream.WriteByte(value);
                    }
                }

                file.AtCtrlZ = false;
                return (int)count;
            }
            catch (IOException)
            {
                return Fail(ENOSPC, 112);
            }
        }
    }

    /// <summary><c>_lseek(fd, offset, origin)</c>: the new position, -1 on error (a position past 2 GB fails with <see cref="EINVAL"/>).</summary>
    public static int LSeek(int fd, int offset, int origin)
    {
        long position = LSeekI64(fd, offset, origin);
        if (position > int.MaxValue)
        {
            return Fail(EINVAL, 0);
        }

        return (int)position;
    }

    /// <summary><c>_lseeki64(fd, offset, origin)</c>.</summary>
    public static long LSeekI64(int fd, long offset, int origin)
    {
        lock (Gate)
        {
            if (!TryGet(fd, out OpenFile? file) || !file.Stream.CanSeek)
            {
                return Fail(EBADF, ERROR_INVALID_HANDLE);
            }

            long basePosition = origin switch
            {
                SEEK_SET => 0,
                SEEK_CUR => file.Stream.Position,
                SEEK_END => file.Stream.Length,
                _ => -1,
            };
            if (basePosition < 0)
            {
                return Fail(EINVAL, 87);
            }

            long target = basePosition + offset;
            if (target < 0)
            {
                return Fail(EINVAL, ERROR_NEGATIVE_SEEK);
            }

            file.Stream.Position = target;
            file.AtCtrlZ = false;
            return target;
        }
    }

    /// <summary><c>_tell(fd)</c>.</summary>
    public static int Tell(int fd) => LSeek(fd, 0, SEEK_CUR);

    /// <summary><c>_filelength(fd)</c>: the size in bytes, -1 on error.</summary>
    public static int FileLength(int fd)
    {
        long length = FileLengthI64(fd);
        return length > int.MaxValue ? Fail(EINVAL, 0) : (int)length;
    }

    /// <summary><c>_filelengthi64(fd)</c>.</summary>
    public static long FileLengthI64(int fd)
    {
        lock (Gate)
        {
            return TryGet(fd, out OpenFile? file) && file.Stream.CanSeek ? file.Stream.Length : Fail(EBADF, ERROR_INVALID_HANDLE);
        }
    }

    /// <summary><c>_eof(fd)</c>: 1 at end of file, 0 before it, -1 on error.</summary>
    public static int Eof(int fd)
    {
        lock (Gate)
        {
            if (!TryGet(fd, out OpenFile? file) || !file.Stream.CanSeek)
            {
                return Fail(EBADF, ERROR_INVALID_HANDLE);
            }

            return file.Stream.Position >= file.Stream.Length ? 1 : 0;
        }
    }

    /// <summary><c>_close(fd)</c>: 0, or -1 for a handle that is not open.</summary>
    public static int Close(int fd)
    {
        lock (Gate)
        {
            if (!Table.Remove(fd, out OpenFile? file))
            {
                return Fail(EBADF, ERROR_INVALID_HANDLE);
            }

            file.Stream.Dispose();
            return 0;
        }
    }

    /// <summary><c>_commit(fd)</c>: writes the handle's buffered data to the device; 0, or -1 for a handle that is not open.</summary>
    public static int Commit(int fd) => Flush(fd, toDisk: true);

    /// <summary>Hands buffered data of the port's stream to the file system (what <c>fflush</c> leaves behind: the CRT's <c>_write</c> is unbuffered).</summary>
    internal static int Flush(int fd, bool toDisk)
    {
        if (fd is >= 0 and < FirstFileHandle)
        {
            return 0;
        }

        lock (Gate)
        {
            if (!TryGet(fd, out OpenFile? file))
            {
                return Fail(EBADF, ERROR_INVALID_HANDLE);
            }

            try
            {
                if (toDisk && file.Stream is FileStream fileStream)
                {
                    fileStream.Flush(flushToDisk: true);
                }
                else
                {
                    file.Stream.Flush();
                }

                return 0;
            }
            catch (IOException)
            {
                return Fail(EBADF, ERROR_INVALID_HANDLE);
            }
        }
    }

    /// <summary>True when <paramref name="fd"/> is an open file handle (or 0-2).</summary>
    public static bool IsOpen(int fd)
    {
        lock (Gate)
        {
            return fd is >= 0 and < FirstFileHandle || Table.ContainsKey(fd);
        }
    }

    /// <summary>The program path a handle was opened with (diagnostics, traces), or <c>null</c>.</summary>
    public static string? PathOf(int fd)
    {
        lock (Gate)
        {
            return Table.TryGetValue(fd, out OpenFile? file) ? file.Path : null;
        }
    }

    /// <summary>Closes every handle (tests; the CRT's <c>_fcloseall</c>-at-exit). Returns how many were open.</summary>
    public static int CloseAll()
    {
        lock (Gate)
        {
            int count = Table.Count;
            foreach (OpenFile file in Table.Values)
            {
                file.Stream.Dispose();
            }

            Table.Clear();
            Errno = 0;
            DosErrno = 0;
            return count;
        }
    }

    /// <summary>
    /// <c>_getcwd(buffer, size)</c>: copies <see cref="IFileSystem.CurrentDirectory"/> (NUL-terminated) and returns
    /// <paramref name="buffer"/>; a null buffer allocates one with <see cref="System.Runtime.InteropServices.NativeMemory"/>;
    /// returns null with <see cref="ERANGE"/> when it does not fit.
    /// </summary>
    public static byte* GetCwd(byte* buffer, int size)
    {
        byte[] bytes = PathEncoding.GetBytes(Ports.Files.CurrentDirectory);
        if (buffer is null)
        {
            int length = Math.Max(size, bytes.Length + 1);
            buffer = (byte*)System.Runtime.InteropServices.NativeMemory.Alloc((nuint)length);
            size = length;
        }

        if (bytes.Length + 1 > size)
        {
            Fail(ERANGE, 0);
            return null;
        }

        bytes.CopyTo(new Span<byte>(buffer, size));
        buffer[bytes.Length] = 0;
        return buffer;
    }

    private static bool TryGet(int fd, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out OpenFile? file) => Table.TryGetValue(fd, out file);

    private static int ReadFully(Stream stream, Span<byte> target)
    {
        int total = 0;
        while (total < target.Length)
        {
            int read = stream.Read(target[total..]);
            if (read == 0)
            {
                break;
            }

            total += read;
        }

        return total;
    }

    /// <summary>CR LF becomes LF in place; Ctrl-Z ends the file (the stream is positioned on it, later reads return 0).</summary>
    private static int TranslateRead(OpenFile file, Span<byte> buffer, int read)
    {
        int output = 0;
        for (int i = 0; i < read; i++)
        {
            byte value = buffer[i];
            if (value == 0x1A)
            {
                file.AtCtrlZ = true;
                if (file.Stream.CanSeek)
                {
                    file.Stream.Position -= read - i;
                }

                break;
            }

            if (value == (byte)'\r')
            {
                if (i + 1 < read)
                {
                    if (buffer[i + 1] == (byte)'\n')
                    {
                        continue; // the LF follows
                    }
                }
                else if (file.Stream.CanSeek)
                {
                    // CR at the end of the buffer: peek the next byte like the CRT does.
                    int next = file.Stream.ReadByte();
                    if (next == '\n')
                    {
                        buffer[output++] = (byte)'\n';
                        continue;
                    }

                    if (next >= 0)
                    {
                        file.Stream.Position--;
                    }
                }
            }

            buffer[output++] = value;
        }

        return output;
    }

    private static (int Errno, uint DosErrno) Map(Exception exception) => exception switch
    {
        FileNotFoundException => (ENOENT, ERROR_FILE_NOT_FOUND),
        DirectoryNotFoundException => (ENOENT, ERROR_PATH_NOT_FOUND),
        UnauthorizedAccessException => (EACCES, ERROR_ACCESS_DENIED),
        IOException io when (io.HResult & 0xFFFF) == ERROR_FILE_EXISTS => (EEXIST, ERROR_FILE_EXISTS),
        IOException io when (io.HResult & 0xFFFF) == ERROR_SHARING_VIOLATION => (EACCES, ERROR_SHARING_VIOLATION),
        IOException => (EACCES, ERROR_ACCESS_DENIED),
        _ => (EINVAL, 87),
    };

    private static int Fail(int errno, uint doserrno)
    {
        Errno = errno;
        DosErrno = doserrno;
        return -1;
    }

    private sealed class OpenFile(Stream stream, bool text, bool append, string path)
    {
        public Stream Stream { get; } = stream;
        public bool Text { get; } = text;
        public bool Append { get; } = append;
        public string Path { get; } = path;
        public bool AtCtrlZ { get; set; }
    }
}
