using System.Runtime.InteropServices;

namespace Recompiler.Runtime;

/// <summary>
/// The CRT's <c>FILE</c> (<c>stdio.h</c> of VC6 to VS2013): the layout compiled code reads through the <c>feof</c>,
/// <c>ferror</c>, <c>_fileno</c>, <c>getc</c> and <c>putc</c> macros. <see cref="CrtStdio"/> keeps <see cref="Flag"/> and
/// <see cref="File"/> current and <see cref="Count"/> at zero, so an inlined <c>getc</c>/<c>putc</c> always falls through to
/// <c>_filbuf</c>/<c>_flsbuf</c> (<see cref="CrtStdio.Filbuf"/>, <see cref="CrtStdio.Flsbuf"/>).
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct CrtFile
{
    public byte* Pointer;
    public int Count;
    public byte* Base;
    public int Flag;
    public int File;
    public int CharBuffer;
    public int BufferSize;
    public byte* TemporaryFileName;
}

/// <summary>
/// The CRT stream layer (<c>fopen</c>, <c>_fsopen</c>, <c>fclose</c>, <c>fread</c>, <c>fwrite</c>, <c>fgets</c>, <c>fputs</c>,
/// <c>fgetc</c>, <c>fputc</c>, <c>ungetc</c>, <c>fprintf</c>, <c>fscanf</c>, <c>fseek</c>, <c>ftell</c>, <c>rewind</c>,
/// <c>feof</c>, <c>ferror</c>, <c>clearerr</c>, <c>fflush</c>, <c>_fileno</c>) over <see cref="CrtFiles"/>, so a stream and the
/// low-level handle under it share one table. Every stream is a native <see cref="CrtFile"/> block (zeroed, CRT layout), so
/// rewritten code passes <c>FILE*</c> around, compares it with NULL and reads its flags as the original did; closed blocks
/// are reused like the CRT's stream slots and never freed. Failures return NULL or <c>EOF</c> and set
/// <see cref="CrtFiles.Errno"/>. Modes follow MSVC: <c>r</c>, <c>w</c> (create, truncate), <c>a</c> (create, every write at
/// the end), <c>+</c> (update), <c>t</c>/<c>b</c> (text is the default through <see cref="CrtFiles.DefaultMode"/>: CR LF read
/// as LF, Ctrl-Z ends the file, LF written as CR LF), <c>c n S R T D</c> accepted; an unknown character ends the mode like
/// VC6. Streams are unbuffered (writes reach the port at once; switching between reading and writing on an update stream
/// needs no <c>fseek</c>), and <c>setvbuf</c> is accepted and ignored. <see cref="Stdin"/>, <see cref="Stdout"/> and
/// <see cref="Stderr"/> sit on handles 0-2 (output goes to <see cref="Ports.Console"/>).
/// </summary>
public static unsafe class CrtStdio
{
    public const int EOF = -1;

    // _flag bits (stdio.h).
    public const int IOREAD = 0x0001;
    public const int IOWRT = 0x0002;
    public const int IOEOF = 0x0010;
    public const int IOERR = 0x0020;
    public const int IOSTRG = 0x0040;
    public const int IORW = 0x0080;

    /// <summary>The CRT's stream limit (<c>_NSTREAM_</c>), standard streams included.</summary>
    public const int MaxStreams = 512;

    private const int O_SHORT_LIVED = 0x1000;
    private static readonly object Gate = new();
    private static readonly Dictionary<nint, StreamState> Streams = [];
    private static readonly Stack<nint> FreeBlocks = new();
    private static readonly CrtFile*[] StandardStreams = new CrtFile*[3];

    /// <summary><c>stdin</c> (handle 0).</summary>
    public static CrtFile* Stdin => Standard(0);

    /// <summary><c>stdout</c> (handle 1, <see cref="Ports.Console"/>).</summary>
    public static CrtFile* Stdout => Standard(1);

    /// <summary><c>stderr</c> (handle 2, <see cref="Ports.Console"/> error).</summary>
    public static CrtFile* Stderr => Standard(2);

    /// <summary><c>fopen(path, mode)</c>: the stream, or NULL with <see cref="CrtFiles.Errno"/> set.</summary>
    public static CrtFile* Fopen(byte* path, byte* mode) => Fsopen(path, mode, CrtFiles.SH_DENYNO);

    /// <summary><c>_fsopen(path, mode, shflag)</c> with an <c>_SH_*</c> sharing mode (<see cref="CrtFiles.SH_DENYNO"/> is what <c>fopen</c> passes).</summary>
    public static CrtFile* Fsopen(byte* path, byte* mode, int shflag)
    {
        if (path is null || mode is null)
        {
            return Fail(CrtFiles.EINVAL);
        }

        return Fsopen(NativeString.ToString(path, CrtFiles.PathEncoding)!, NativeString.ToString(mode, CrtFiles.PathEncoding)!, shflag);
    }

    /// <summary><c>_wfopen(path, mode)</c>.</summary>
    public static CrtFile* Wfopen(char* path, char* mode) =>
        path is null || mode is null ? Fail(CrtFiles.EINVAL) : Fsopen(NativeString.ToString(path)!, NativeString.ToString(mode)!, CrtFiles.SH_DENYNO);

    /// <summary><c>fopen</c> with managed strings (ports that already hold them, tests).</summary>
    public static CrtFile* Fopen(string path, string mode) => Fsopen(path, mode, CrtFiles.SH_DENYNO);

    public static CrtFile* Fsopen(string path, string mode, int shflag)
    {
        if (!TryParseMode(mode, out int oflag, out int streamFlag))
        {
            return Fail(CrtFiles.EINVAL);
        }

        lock (Gate)
        {
            if (Streams.Count >= MaxStreams)
            {
                return Fail(CrtFiles.EMFILE);
            }

            int fd = CrtFiles.Open(path, oflag, shflag, CrtFiles.S_IREAD | CrtFiles.S_IWRITE);
            if (fd < 0)
            {
                return null;
            }

            return Register(fd, streamFlag);
        }
    }

    /// <summary>
    /// Parses an <c>fopen</c> mode the way MSVC's <c>_openfile</c> does: the <c>_O_*</c> flags for <see cref="CrtFiles.Open(string, int, int, int)"/>
    /// and the stream's <c>_flag</c>; false for a mode that does not start with <c>r</c>, <c>w</c> or <c>a</c>.
    /// </summary>
    public static bool TryParseMode(string mode, out int oflag, out int streamFlag)
    {
        ArgumentNullException.ThrowIfNull(mode);
        (oflag, streamFlag) = mode.Length == 0 ? (0, 0) : mode[0] switch
        {
            'r' => (CrtFiles.O_RDONLY, IOREAD),
            'w' => (CrtFiles.O_WRONLY | CrtFiles.O_CREAT | CrtFiles.O_TRUNC, IOWRT),
            'a' => (CrtFiles.O_WRONLY | CrtFiles.O_CREAT | CrtFiles.O_APPEND, IOWRT),
            _ => (0, 0),
        };
        if (streamFlag == 0)
        {
            return false;
        }

        bool update = false;
        bool commit = false;
        for (int i = 1; i < mode.Length; i++)
        {
            char c = mode[i];
            switch (c)
            {
                case '+' when !update:
                    update = true;
                    oflag = (oflag & ~CrtFiles.O_WRONLY) | CrtFiles.O_RDWR;
                    streamFlag = IORW;
                    continue;
                case 'b' or 't' when (oflag & (CrtFiles.O_TEXT | CrtFiles.O_BINARY)) == 0:
                    oflag |= c == 'b' ? CrtFiles.O_BINARY : CrtFiles.O_TEXT;
                    continue;
                case 'c' or 'n' when !commit:
                    commit = true;
                    continue;
                case 'S' when (oflag & (CrtFiles.O_SEQUENTIAL | CrtFiles.O_RANDOM)) == 0:
                    oflag |= CrtFiles.O_SEQUENTIAL;
                    continue;
                case 'R' when (oflag & (CrtFiles.O_SEQUENTIAL | CrtFiles.O_RANDOM)) == 0:
                    oflag |= CrtFiles.O_RANDOM;
                    continue;
                case 'T' when (oflag & O_SHORT_LIVED) == 0:
                    oflag |= O_SHORT_LIVED;
                    continue;
                case 'D' when (oflag & CrtFiles.O_TEMPORARY) == 0:
                    oflag |= CrtFiles.O_TEMPORARY;
                    continue;
            }

            break; // VC6 stops at the first character it does not expect
        }

        return true;
    }

    /// <summary><c>fclose(stream)</c>: 0, or <c>EOF</c> for a stream that is not open or a handle that fails to close.</summary>
    public static int Fclose(void* stream)
    {
        lock (Gate)
        {
            if (!Streams.Remove((nint)stream, out StreamState? state))
            {
                CrtFiles.Errno = CrtFiles.EINVAL;
                return EOF;
            }

            CrtFile* file = (CrtFile*)stream;
            int result = 0;
            if (state.Fd >= 3 && CrtFiles.Close(state.Fd) < 0)
            {
                result = EOF;
            }

            if (state.Fd < 3)
            {
                StandardStreams[state.Fd] = null;
            }

            *file = default;
            file->File = -1;
            FreeBlocks.Push((nint)stream);
            return result;
        }
    }

    /// <summary><c>_fcloseall()</c>: closes every stream but the standard ones; returns how many were closed.</summary>
    public static int CloseAll()
    {
        List<nint> open;
        lock (Gate)
        {
            open = Streams.Where(s => s.Value.Fd >= 3).Select(s => s.Key).Order().ToList();
        }

        foreach (nint stream in open)
        {
            Fclose((void*)stream);
        }

        return open.Count;
    }

    /// <summary><c>fread(buffer, size, count, stream)</c>: the number of whole elements read; short counts set the end-of-file or error flag.</summary>
    public static nuint Fread(void* buffer, nuint size, nuint count, void* stream)
    {
        nuint total = size * count;
        if (total == 0)
        {
            return 0;
        }

        lock (Gate)
        {
            if (!TryGet(stream, out StreamState? state, out CrtFile* file) || !CanRead(file))
            {
                return 0;
            }

            byte* target = (byte*)buffer;
            nuint done = 0;
            if (state.Pushback >= 0)
            {
                target[done++] = (byte)state.Pushback;
                state.Pushback = -1;
            }

            while (done < total)
            {
                uint chunk = (uint)Math.Min(total - done, int.MaxValue);
                int read = CrtFiles.Read(file->File, target + done, chunk);
                if (read <= 0)
                {
                    file->Flag |= read == 0 ? IOEOF : IOERR;
                    break;
                }

                done += (nuint)read;
            }

            return done / size;
        }
    }

    /// <summary><c>fwrite(buffer, size, count, stream)</c>: the number of whole elements written.</summary>
    public static nuint Fwrite(void* buffer, nuint size, nuint count, void* stream)
    {
        nuint total = size * count;
        if (total == 0)
        {
            return 0;
        }

        lock (Gate)
        {
            if (!TryGet(stream, out StreamState? state, out CrtFile* file))
            {
                return 0;
            }

            int written = Write(state, file, (byte*)buffer, total);
            return written < 0 ? 0 : (nuint)written / size;
        }
    }

    /// <summary><c>fgetc(stream)</c> (and <c>getc</c>): the next byte, or <c>EOF</c> at the end or on error.</summary>
    public static int Fgetc(void* stream)
    {
        lock (Gate)
        {
            return TryGet(stream, out StreamState? state, out CrtFile* file) ? ReadByte(state, file) : EOF;
        }
    }

    /// <summary><c>_filbuf(stream)</c>, what an inlined <c>getc</c> calls when the buffer count runs out: <see cref="Fgetc"/>, count reset.</summary>
    public static int Filbuf(void* stream)
    {
        int value = Fgetc(stream);
        if (stream is not null)
        {
            ((CrtFile*)stream)->Count = 0;
        }

        return value;
    }

    /// <summary><c>fputc(c, stream)</c> (and <c>putc</c>): the byte written, or <c>EOF</c>.</summary>
    public static int Fputc(int c, void* stream)
    {
        lock (Gate)
        {
            if (!TryGet(stream, out StreamState? state, out CrtFile* file))
            {
                return EOF;
            }

            byte value = unchecked((byte)c);
            return Write(state, file, &value, 1) == 1 ? value : EOF;
        }
    }

    /// <summary><c>_flsbuf(c, stream)</c>, what an inlined <c>putc</c> calls: <see cref="Fputc"/>, count reset.</summary>
    public static int Flsbuf(int c, void* stream)
    {
        int value = Fputc(c, stream);
        if (stream is not null)
        {
            ((CrtFile*)stream)->Count = 0;
        }

        return value;
    }

    /// <summary><c>ungetc(c, stream)</c>: one byte of push-back (clears end-of-file); <c>EOF</c> when <paramref name="c"/> is <c>EOF</c> or a byte is pushed back already.</summary>
    public static int Ungetc(int c, void* stream)
    {
        lock (Gate)
        {
            if (c == EOF || !TryGet(stream, out StreamState? state, out CrtFile* file) || state.Pushback >= 0)
            {
                return EOF;
            }

            state.Pushback = unchecked((byte)c);
            file->Flag &= ~IOEOF;
            return state.Pushback;
        }
    }

    /// <summary>
    /// <c>fgets(buffer, count, stream)</c>: reads up to <paramref name="count"/> - 1 bytes, stopping after a newline (which is
    /// kept), and NUL-terminates; NULL when the end comes before any byte (the buffer is then untouched) or for a bad argument.
    /// </summary>
    public static byte* Fgets(byte* buffer, int count, void* stream)
    {
        if (buffer is null || count <= 0)
        {
            CrtFiles.Errno = CrtFiles.EINVAL;
            return null;
        }

        lock (Gate)
        {
            if (!TryGet(stream, out StreamState? state, out CrtFile* file))
            {
                return null;
            }

            int length = 0;
            while (length < count - 1)
            {
                int c = ReadByte(state, file);
                if (c == EOF)
                {
                    if (length == 0)
                    {
                        return null;
                    }

                    break;
                }

                buffer[length++] = (byte)c;
                if (c == '\n')
                {
                    break;
                }
            }

            buffer[length] = 0;
            return buffer;
        }
    }

    /// <summary><c>fputs(text, stream)</c>: 0 (what the MSVC CRT returns on success), or <c>EOF</c>.</summary>
    public static int Fputs(byte* text, void* stream)
    {
        if (text is null)
        {
            CrtFiles.Errno = CrtFiles.EINVAL;
            return EOF;
        }

        nuint length = NativeString.Length(text);
        lock (Gate)
        {
            if (!TryGet(stream, out StreamState? state, out CrtFile* file))
            {
                return EOF;
            }

            return length == 0 || Write(state, file, text, length) == (int)length ? 0 : EOF;
        }
    }

    /// <summary><c>fprintf(stream, format, ...)</c> with x86 arguments (also <c>vfprintf</c>: a <c>va_list</c> is the same slot pointer): the bytes written, or a negative value.</summary>
    public static int Fprintf(void* stream, byte* format, uint* va) => Print(stream, CFormat.Format(format, VaList.X86(va), out _));

    /// <summary><c>fprintf(stream, format, ...)</c> with x64 arguments.</summary>
    public static int Fprintf(void* stream, byte* format, ulong* va) => Print(stream, CFormat.Format(format, VaList.X64(va), out _));

    /// <summary><c>fprintf</c> with managed arguments (see <see cref="CFormat.Format(string, object?[])"/>).</summary>
    public static int Fprintf(void* stream, string format, params object?[] arguments) => Print(stream, CFormat.Encoding.GetBytes(CFormat.Format(format, arguments)));

    /// <summary><c>fscanf(stream, format, ...)</c> with x86 arguments (see <see cref="CScan"/> for the conversions).</summary>
    public static int Fscanf(void* stream, byte* format, uint* va) => Scan(stream, NativeString.AsSpan(format), VaList.X86(va));

    /// <summary><c>fscanf(stream, format, ...)</c> with x64 arguments.</summary>
    public static int Fscanf(void* stream, byte* format, ulong* va) => Scan(stream, NativeString.AsSpan(format), VaList.X64(va));

    /// <summary><c>fseek(stream, offset, origin)</c>: 0, or -1 (bad origin, negative position, stream not open); clears end-of-file and the push-back.</summary>
    public static int Fseek(void* stream, int offset, int origin)
    {
        lock (Gate)
        {
            if (!TryGet(stream, out StreamState? state, out CrtFile* file))
            {
                return -1;
            }

            if (origin is not (CrtFiles.SEEK_SET or CrtFiles.SEEK_CUR or CrtFiles.SEEK_END))
            {
                CrtFiles.Errno = CrtFiles.EINVAL;
                return -1;
            }

            if (origin == CrtFiles.SEEK_CUR && state.Pushback >= 0)
            {
                offset--; // the pushed-back byte was read from the file already
            }

            state.Pushback = -1;
            file->Flag &= ~IOEOF;
            return CrtFiles.LSeek(file->File, offset, origin) < 0 ? -1 : 0;
        }
    }

    /// <summary><c>ftell(stream)</c>: the file position (the raw byte offset, also in text mode, like MSVC), or -1.</summary>
    public static int Ftell(void* stream)
    {
        lock (Gate)
        {
            if (!TryGet(stream, out StreamState? state, out CrtFile* file))
            {
                return -1;
            }

            int position = CrtFiles.Tell(file->File);
            return position < 0 ? -1 : position - (state.Pushback >= 0 ? 1 : 0);
        }
    }

    /// <summary><c>rewind(stream)</c>: back to the start, end-of-file and error flags cleared.</summary>
    public static void Rewind(void* stream)
    {
        if (Fseek(stream, 0, CrtFiles.SEEK_SET) == 0)
        {
            ((CrtFile*)stream)->Flag &= ~IOERR;
        }
    }

    /// <summary><c>feof(stream)</c>: the <see cref="IOEOF"/> bit (0x10) as the VC6 macro yields it, 0 before the end.</summary>
    public static int Feof(void* stream) => IsOpen(stream) ? ((CrtFile*)stream)->Flag & IOEOF : 0;

    /// <summary><c>ferror(stream)</c>: the <see cref="IOERR"/> bit (0x20), 0 without an error.</summary>
    public static int Ferror(void* stream) => IsOpen(stream) ? ((CrtFile*)stream)->Flag & IOERR : 0;

    /// <summary><c>clearerr(stream)</c>.</summary>
    public static void Clearerr(void* stream)
    {
        if (IsOpen(stream))
        {
            ((CrtFile*)stream)->Flag &= ~(IOEOF | IOERR);
        }
    }

    /// <summary><c>_fileno(stream)</c>: the <see cref="CrtFiles"/> handle, or -1.</summary>
    public static int Fileno(void* stream)
    {
        if (!IsOpen(stream))
        {
            CrtFiles.Errno = CrtFiles.EINVAL;
            return -1;
        }

        return ((CrtFile*)stream)->File;
    }

    /// <summary><c>fflush(stream)</c>: hands written data to the file system; NULL flushes every stream. 0, or <c>EOF</c> for a stream that is not open.</summary>
    public static int Fflush(void* stream)
    {
        lock (Gate)
        {
            if (stream is null)
            {
                foreach (StreamState open in Streams.Values)
                {
                    CrtFiles.Flush(open.Fd, toDisk: false);
                }

                return 0;
            }

            if (!TryGet(stream, out StreamState? state, out _))
            {
                return EOF;
            }

            return CrtFiles.Flush(state.Fd, toDisk: false) < 0 ? EOF : 0;
        }
    }

    /// <summary><c>setvbuf(stream, buffer, mode, size)</c>: accepted (0) and ignored, streams stay unbuffered; -1 for a stream that is not open.</summary>
    public static int Setvbuf(void* stream, byte* buffer, int mode, nuint size)
    {
        _ = buffer;
        _ = mode;
        _ = size;
        return IsOpen(stream) ? 0 : -1;
    }

    /// <summary>True when <paramref name="stream"/> is a stream of this table that is open.</summary>
    public static bool IsOpen(void* stream)
    {
        lock (Gate)
        {
            return stream is not null && Streams.ContainsKey((nint)stream);
        }
    }

    private static CrtFile* Standard(int fd)
    {
        lock (Gate)
        {
            if (StandardStreams[fd] is null)
            {
                StandardStreams[fd] = Register(fd, fd == 0 ? IOREAD : IOWRT);
            }

            return StandardStreams[fd];
        }
    }

    private static CrtFile* Register(int fd, int streamFlag)
    {
        CrtFile* file = FreeBlocks.Count > 0 ? (CrtFile*)FreeBlocks.Pop() : (CrtFile*)NativeMemory.AllocZeroed((nuint)sizeof(CrtFile));
        *file = default;
        file->Flag = streamFlag;
        file->File = fd;
        Streams[(nint)file] = new StreamState(fd);
        return file;
    }

    private static int Print(void* stream, byte[] text)
    {
        lock (Gate)
        {
            if (!TryGet(stream, out StreamState? state, out CrtFile* file))
            {
                return -1;
            }

            if (text.Length == 0)
            {
                return 0;
            }

            fixed (byte* data = text)
            {
                return Write(state, file, data, (nuint)text.Length) < 0 ? -1 : text.Length;
            }
        }
    }

    private static int Scan(void* stream, ReadOnlySpan<byte> format, VaList va)
    {
        if (!IsOpen(stream))
        {
            CrtFiles.Errno = CrtFiles.EINVAL;
            return EOF;
        }

        return CScan.Scan(new StreamInput((nint)stream), format, va);
    }

    private static bool CanRead(CrtFile* file)
    {
        if ((file->Flag & (IOREAD | IORW)) != 0)
        {
            return true;
        }

        file->Flag |= IOERR;
        CrtFiles.Errno = CrtFiles.EBADF;
        return false;
    }

    private static int ReadByte(StreamState state, CrtFile* file)
    {
        if (state.Pushback >= 0)
        {
            int value = state.Pushback;
            state.Pushback = -1;
            return value;
        }

        if (!CanRead(file))
        {
            return EOF;
        }

        byte b;
        int read = CrtFiles.Read(file->File, &b, 1);
        if (read == 1)
        {
            return b;
        }

        file->Flag |= read == 0 ? IOEOF : IOERR;
        return EOF;
    }

    /// <summary>Writes through the handle: the bytes written, or -1 with the error flag set.</summary>
    private static int Write(StreamState state, CrtFile* file, byte* data, nuint count)
    {
        if ((file->Flag & (IOWRT | IORW)) == 0 || count > int.MaxValue)
        {
            file->Flag |= IOERR;
            CrtFiles.Errno = count > int.MaxValue ? CrtFiles.EINVAL : CrtFiles.EBADF;
            return -1;
        }

        state.Pushback = -1;
        int written = CrtFiles.Write(file->File, data, (uint)count);
        if (written < 0)
        {
            file->Flag |= IOERR;
        }

        return written;
    }

    private static bool TryGet(void* stream, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out StreamState? state, out CrtFile* file)
    {
        file = (CrtFile*)stream;
        if (stream is not null && Streams.TryGetValue((nint)stream, out state))
        {
            return true;
        }

        state = null;
        CrtFiles.Errno = CrtFiles.EINVAL;
        return false;
    }

    private static CrtFile* Fail(int errno)
    {
        CrtFiles.Errno = errno;
        return null;
    }

    private sealed class StreamState(int fd)
    {
        public int Fd { get; } = fd;

        /// <summary>The byte <c>ungetc</c> pushed back, -1 for none.</summary>
        public int Pushback { get; set; } = -1;
    }

    /// <summary><c>fscanf</c> input: <see cref="Fgetc"/> and <see cref="Ungetc"/>.</summary>
    private sealed class StreamInput(nint stream) : IScanInput
    {
        public int Read() => Fgetc((void*)stream);

        public void Unread(int value) => Ungetc(value, (void*)stream);
    }
}
