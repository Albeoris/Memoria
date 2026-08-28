using Microsoft.Win32.SafeHandles;
using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Memoria.Launcher
{
    #region JunctionPoint
    /// <summary>
    /// Provides access to NTFS junction points in .Net.
    /// http://www.codeproject.com/Articles/15633/Manipulating-NTFS-Junction-Points-in-NET
    /// </summary>
    public static class JunctionPoint
    {
        /// <summary>
        /// The file or directory is not a reparse point.
        /// </summary>
        private const int ERROR_NOT_A_REPARSE_POINT = 4390;

        /// <summary>
        /// The reparse point attribute cannot be set because it conflicts with an existing attribute.
        /// </summary>
        private const int ERROR_REPARSE_ATTRIBUTE_CONFLICT = 4391;

        /// <summary>
        /// The data present in the reparse point buffer is invalid.
        /// </summary>
        private const int ERROR_INVALID_REPARSE_DATA = 4392;

        /// <summary>
        /// The tag present in the reparse point buffer is invalid.
        /// </summary>
        private const int ERROR_REPARSE_TAG_INVALID = 4393;

        /// <summary>
        /// There is a mismatch between the tag specified in the request and the tag present in the reparse point.
        /// </summary>
        private const int ERROR_REPARSE_TAG_MISMATCH = 4394;

        /// <summary>
        /// Command to set the reparse point data block.
        /// </summary>
        private const int FSCTL_SET_REPARSE_POINT = 0x000900A4;

        /// <summary>
        /// Command to get the reparse point data block.
        /// </summary>
        private const int FSCTL_GET_REPARSE_POINT = 0x000900A8;

        /// <summary>
        /// Command to delete the reparse point data base.
        /// </summary>
        private const int FSCTL_DELETE_REPARSE_POINT = 0x000900AC;

        /// <summary>
        /// Reparse point tag used to identify mount points and junction points.
        /// </summary>
        private const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;

        /// <summary>
        /// This prefix indicates to NTFS that the path is to be treated as a non-interpreted
        /// path in the virtual file system.
        /// </summary>
        private const string NonInterpretedPathPrefix = @"\??\";

        [Flags]
        private enum EFileAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000,
        }

        [Flags]
        private enum EFileShare : uint
        {
            None = 0x00000000,
            Read = 0x00000001,
            Write = 0x00000002,
            Delete = 0x00000004,
        }

        private enum ECreationDisposition : uint
        {
            New = 1,
            CreateAlways = 2,
            OpenExisting = 3,
            OpenAlways = 4,
            TruncateExisting = 5,
        }

        [Flags]
        private enum EFileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct REPARSE_DATA_BUFFER
        {
            /// <summary>
            /// Reparse point tag. Must be a Microsoft reparse point tag.
            /// </summary>
            public uint ReparseTag;

            /// <summary>
            /// Size, in bytes, of the data after the Reserved member. This can be calculated by:
            /// (4 * sizeof(ushort)) + SubstituteNameLength + PrintNameLength + 
            /// (namesAreNullTerminated ? 2 * sizeof(char) : 0);
            /// </summary>
            public ushort ReparseDataLength;

            /// <summary>
            /// Reserved; do not use. 
            /// </summary>
            public ushort Reserved;

            /// <summary>
            /// Offset, in bytes, of the substitute name string in the PathBuffer array.
            /// </summary>
            public ushort SubstituteNameOffset;

            /// <summary>
            /// Length, in bytes, of the substitute name string. If this string is null-terminated,
            /// SubstituteNameLength does not include space for the null character.
            /// </summary>
            public ushort SubstituteNameLength;

            /// <summary>
            /// Offset, in bytes, of the print name string in the PathBuffer array.
            /// </summary>
            public ushort PrintNameOffset;

            /// <summary>
            /// Length, in bytes, of the print name string. If this string is null-terminated,
            /// PrintNameLength does not include space for the null character. 
            /// </summary>
            public ushort PrintNameLength;

            /// <summary>
            /// A buffer containing the unicode-encoded path string. The path string contains
            /// the substitute name string and print name string.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF0)]
            public byte[] PathBuffer;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
            IntPtr InBuffer, int nInBufferSize,
            IntPtr OutBuffer, int nOutBufferSize,
            out int pBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            EFileAccess dwDesiredAccess,
            EFileShare dwShareMode,
            IntPtr lpSecurityAttributes,
            ECreationDisposition dwCreationDisposition,
            EFileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        /// <summary>
        /// Creates a junction point from the specified directory to the specified target directory.
        /// </summary>
        /// <remarks>
        /// Only works on NTFS.
        /// </remarks>
        /// <param name="junctionPoint">The junction point path</param>
        /// <param name="targetDir">The target directory</param>
        /// <param name="overwrite">If true overwrites an existing reparse point or empty directory</param>
        /// <exception cref="IOException">Thrown when the junction point could not be created or when
        /// an existing directory was found and <paramref name="overwrite" /> if false</exception>
        public static void Create(string junctionPoint, string targetDir, bool overwrite)
        {
            targetDir = Path.GetFullPath(targetDir);

            if (!Directory.Exists(targetDir))
                throw new IOException("Target path does not exist or is not a directory.");

            if (Directory.Exists(junctionPoint))
            {
                if (!overwrite)
                    throw new IOException("Directory already exists and overwrite parameter is false.");
            }
            else
            {
                Directory.CreateDirectory(junctionPoint);
            }

            using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericWrite))
            {
                byte[] targetDirBytes = Encoding.Unicode.GetBytes(NonInterpretedPathPrefix + Path.GetFullPath(targetDir));

                REPARSE_DATA_BUFFER reparseDataBuffer = new REPARSE_DATA_BUFFER();

                reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
                reparseDataBuffer.ReparseDataLength = (ushort)(targetDirBytes.Length + 12);
                reparseDataBuffer.SubstituteNameOffset = 0;
                reparseDataBuffer.SubstituteNameLength = (ushort)targetDirBytes.Length;
                reparseDataBuffer.PrintNameOffset = (ushort)(targetDirBytes.Length + 2);
                reparseDataBuffer.PrintNameLength = 0;
                reparseDataBuffer.PathBuffer = new byte[0x3ff0];
                Array.Copy(targetDirBytes, reparseDataBuffer.PathBuffer, targetDirBytes.Length);

                int inBufferSize = Marshal.SizeOf(reparseDataBuffer);
                IntPtr inBuffer = Marshal.AllocHGlobal(inBufferSize);

                try
                {
                    Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                    int bytesReturned;
                    bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_SET_REPARSE_POINT,
                        inBuffer, targetDirBytes.Length + 20, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);

                    if (!result)
                        ThrowLastWin32Error("Unable to create junction point.");
                }
                finally
                {
                    Marshal.FreeHGlobal(inBuffer);
                }
            }
        }

        /// <summary>
        /// Deletes a junction point at the specified source directory along with the directory itself.
        /// Does nothing if the junction point does not exist.
        /// </summary>
        /// <remarks>
        /// Only works on NTFS.
        /// </remarks>
        /// <param name="junctionPoint">The junction point path</param>
        public static void Delete(string junctionPoint)
        {
            if (!Directory.Exists(junctionPoint))
            {
                if (File.Exists(junctionPoint))
                    throw new IOException("Path is not a junction point.");

                return;
            }

            using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericWrite))
            {
                REPARSE_DATA_BUFFER reparseDataBuffer = new REPARSE_DATA_BUFFER();

                reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
                reparseDataBuffer.ReparseDataLength = 0;
                reparseDataBuffer.PathBuffer = new byte[0x3ff0];

                int inBufferSize = Marshal.SizeOf(reparseDataBuffer);
                IntPtr inBuffer = Marshal.AllocHGlobal(inBufferSize);
                try
                {
                    Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                    int bytesReturned;
                    bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_DELETE_REPARSE_POINT,
                        inBuffer, 8, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);

                    if (!result)
                        ThrowLastWin32Error("Unable to delete junction point.");
                }
                finally
                {
                    Marshal.FreeHGlobal(inBuffer);
                }

                try
                {
                    Directory.Delete(junctionPoint);
                }
                catch (IOException ex)
                {
                    throw new IOException("Unable to delete junction point.", ex);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified path exists and refers to a junction point.
        /// </summary>
        /// <param name="path">The junction point path</param>
        /// <returns>True if the specified path represents a junction point</returns>
        /// <exception cref="IOException">Thrown if the specified path is invalid
        /// or some other error occurs</exception>
        public static bool Exists(string path)
        {
            if (!Directory.Exists(path))
                return false;

            using (SafeFileHandle handle = OpenReparsePoint(path, EFileAccess.GenericRead))
            {
                string target = InternalGetTarget(handle);
                return target != null;
            }
        }

        /// <summary>
        /// Gets the target of the specified junction point.
        /// </summary>
        /// <remarks>
        /// Only works on NTFS.
        /// </remarks>
        /// <param name="junctionPoint">The junction point path</param>
        /// <returns>The target of the junction point</returns>
        /// <exception cref="IOException">Thrown when the specified path does not
        /// exist, is invalid, is not a junction point, or some other error occurs</exception>
        public static string GetTarget(string junctionPoint)
        {
            using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericRead))
            {
                string target = InternalGetTarget(handle);
                return (string)target ?? throw new IOException("Path is not a junction point.");
            }
        }

        private static string InternalGetTarget(SafeFileHandle handle)
        {
            int outBufferSize = Marshal.SizeOf(typeof(REPARSE_DATA_BUFFER));
            IntPtr outBuffer = Marshal.AllocHGlobal(outBufferSize);

            try
            {
                int bytesReturned;
                bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_GET_REPARSE_POINT,
                    IntPtr.Zero, 0, outBuffer, outBufferSize, out bytesReturned, IntPtr.Zero);

                if (!result)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error == ERROR_NOT_A_REPARSE_POINT)
                        return null;

                    ThrowLastWin32Error("Unable to get information about junction point.");
                }

                REPARSE_DATA_BUFFER reparseDataBuffer = (REPARSE_DATA_BUFFER)
                    Marshal.PtrToStructure(outBuffer, typeof(REPARSE_DATA_BUFFER));

                if (reparseDataBuffer.ReparseTag != IO_REPARSE_TAG_MOUNT_POINT)
                    return null;

                string targetDir = Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer,
                    reparseDataBuffer.SubstituteNameOffset, reparseDataBuffer.SubstituteNameLength);

                if (targetDir.StartsWith(NonInterpretedPathPrefix))
                    targetDir = targetDir.Substring(NonInterpretedPathPrefix.Length);

                return targetDir;
            }
            finally
            {
                Marshal.FreeHGlobal(outBuffer);
            }
        }

        private static SafeFileHandle OpenReparsePoint(string reparsePoint, EFileAccess accessMode)
        {
            SafeFileHandle reparsePointHandle = new SafeFileHandle(CreateFile(reparsePoint, accessMode,
                EFileShare.Read | EFileShare.Write | EFileShare.Delete,
                IntPtr.Zero, ECreationDisposition.OpenExisting,
                EFileAttributes.BackupSemantics | EFileAttributes.OpenReparsePoint, IntPtr.Zero), true);

            if (Marshal.GetLastWin32Error() != 0)
                ThrowLastWin32Error("Unable to open reparse point.");

            return reparsePointHandle;
        }

        private static void ThrowLastWin32Error(string message)
        {
            throw new IOException(message, Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
        }
    }
    #endregion

    #region Extractor

    public static class ExtractorSharpCompress
    {
        private static readonly NLog.Logger _log = AppLogger.GetLogger();

        // This is slow with some archives (7zip)
        public static void ExtractAllFileFromArchive(string archivePath, string extractTo, CancellationToken cancellationToken, Action<int> progressCallbak = null)
        {
            if (!File.Exists(archivePath))
            {
                _log.Warn("Extraction skipped because archive was not found. Archive: {ArchivePath}", archivePath);
                return;
            }

            _log.Info("Starting archive extraction (SharpCompress). Archive: {ArchivePath}, Destination: {Destination}", archivePath, extractTo);
            try
            {
                using (var archive = ArchiveFactory.OpenArchive(archivePath))
                {
                    int total = 0;
                    foreach (var entry in archive.Entries)
                        if (!entry.IsDirectory) total++;

                    int current = 0;
                    foreach (var entry in archive.Entries)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            _log.Warn("Archive extraction cancelled by token. Archive: {ArchivePath}, Destination: {Destination}, ExtractedEntries: {ExtractedEntries}, TotalEntries: {TotalEntries}", archivePath, extractTo, current, total);
                            break;
                        }

                        if (!entry.IsDirectory)
                        {
                            try
                            {
                                entry.WriteToDirectory(extractTo, new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }
                            catch (Exception ex)
                            {
                                _log.Error(ex, "Failed extracting archive entry. Archive: {ArchivePath}, Destination: {Destination}, Entry: {EntryKey}", archivePath, extractTo, entry.Key);
                                throw;
                            }

                            current++;
                            progressCallbak?.Invoke((int)(100 * current / total));
                        }
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        if (Directory.Exists(extractTo))
                        {
                            try
                            {
                                Directory.Delete(extractTo, true);
                                _log.Info("Cleaned extraction destination after cancellation. Destination: {Destination}", extractTo);
                            }
                            catch (Exception ex)
                            {
                                _log.Warn(ex, "Failed to cleanup extraction destination after cancellation. Destination: {Destination}", extractTo);
                            }
                        }
                    }

                    _log.Info("Archive extraction completed (SharpCompress). Archive: {ArchivePath}, Destination: {Destination}, ExtractedEntries: {ExtractedEntries}, TotalEntries: {TotalEntries}", archivePath, extractTo, current, total);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Archive extraction failed (SharpCompress). Archive: {ArchivePath}, Destination: {Destination}", archivePath, extractTo);
                throw;
            }
        }
    }

    public static class Extractor
    {
        private static readonly NLog.Logger _log = AppLogger.GetLogger();

        private const String SevenZipPath = "7za.exe";
        public static void ExtractAllFileFromArchive(string archivePath, string extractTo, CancellationToken cancellationToken, Action<int> progressCallbak = null)
        {
            if (!File.Exists(archivePath))
            {
                _log.Warn("Extraction skipped because archive was not found. Archive: {ArchivePath}", archivePath);
                return;
            }

            _log.Info("Starting archive extraction (7za). Archive: {ArchivePath}, Destination: {Destination}", archivePath, extractTo);

            if (!File.Exists(SevenZipPath))
            {
                using Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("7za.exe");
                if (input == null)
                {
                    _log.Error("Unable to extract embedded 7za.exe resource. Archive: {ArchivePath}", archivePath);
                    throw new FileNotFoundException("Embedded 7za.exe resource was not found.", SevenZipPath);
                }

                using FileStream output = File.Create(SevenZipPath);
                input.CopyTo(output);
            }

            progressCallbak?.Invoke(0);
            using (Process process = new Process())
            {
                process.StartInfo.FileName = SevenZipPath;
                process.StartInfo.Arguments = $@"x -bsp1 -aoa -o""{extractTo}"" ""{archivePath}""";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;

                String error = null;
                String stderr = null;
                process.OutputDataReceived += (s, e) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            if (!process.HasExited)
                                process.Kill();
                        }
                        catch (Exception ex)
                        {
                            _log.Warn(ex, "Failed to kill 7za process after cancellation. Archive: {ArchivePath}", archivePath);
                        }

                        return;
                    }

                    string data = e.Data ?? "";
                    if (data.Contains("Can't open as archive"))
                        error = data;
                    int pos = data.IndexOf('%');
                    if (pos >= 0 && int.TryParse(data.Substring(0, pos), out int progress))
                        progressCallbak?.Invoke(progress);
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    string data = e.Data ?? "";
                    if (!String.IsNullOrWhiteSpace(data))
                        stderr = data;
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (error != null)
                {
                    _log.Error("7za reported archive format error. Archive: {ArchivePath}, Destination: {Destination}, Error: {Error}", archivePath, extractTo, error);
                    throw new FileFormatException(error);
                }

                if (process.ExitCode != 0)
                {
                    String failure = !String.IsNullOrWhiteSpace(stderr)
                        ? stderr
                        : $"7za exited with code {process.ExitCode}.";
                    _log.Error("7za extraction failed. Archive: {ArchivePath}, Destination: {Destination}, ExitCode: {ExitCode}, Error: {Error}", archivePath, extractTo, process.ExitCode, failure);
                    throw new InvalidOperationException(failure);
                }

                progressCallbak?.Invoke(100);
                _log.Info("Archive extraction completed (7za). Archive: {ArchivePath}, Destination: {Destination}", archivePath, extractTo);
            }
            if (File.Exists(SevenZipPath))
                File.Delete(SevenZipPath);
        }
    }
    #endregion

    #region ThrottledWeb

    public sealed class ThrottledDownloadProgressChangedEventArgs : EventArgs
    {
        public ThrottledDownloadProgressChangedEventArgs(Int64 bytesReceived, Int64 totalBytesToReceive)
        {
            BytesReceived = bytesReceived;
            TotalBytesToReceive = totalBytesToReceive;
        }

        public Int64 BytesReceived { get; }
        public Int64 TotalBytesToReceive { get; }

        public Int32 ProgressPercentage
        {
            get
            {
                if (TotalBytesToReceive <= 0)
                    return 0;

                Double progress = BytesReceived * 100d / TotalBytesToReceive;
                return (Int32)Math.Max(0, Math.Min(100, Math.Round(progress)));
            }
        }
    }

    public class ThrottledHttpClient : IDisposable
    {
        private static readonly NLog.Logger _log = AppLogger.GetLogger();

        private readonly HttpClient _client;
        private readonly Timer _timer;
        private readonly object _stateLock = new object();
        private CancellationTokenSource _downloadCts;
        private bool _isBusy;
        private bool _updatePending;

        public event EventHandler<ThrottledDownloadProgressChangedEventArgs> DownloadProgressChanged;
        public event AsyncCompletedEventHandler DownloadFileCompleted;

        public bool IsBusy
        {
            get
            {
                lock (_stateLock)
                    return _isBusy;
            }
        }

        public ThrottledHttpClient()
        {
            _client = HttpClients.CreateDownloadClient();

            _timer = new Timer(100);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        public void CancelAsync()
        {
            lock (_stateLock)
            {
                _downloadCts?.Cancel();
            }
        }

        public void DownloadFileAsync(Uri address, String fileName)
        {
            _log.Info("Requesting {Uri}", address);

            CancellationTokenSource cts;
            lock (_stateLock)
            {
                if (_isBusy)
                    throw new InvalidOperationException("A download is already in progress.");

                _downloadCts = new CancellationTokenSource();
                cts = _downloadCts;
                _isBusy = true;
            }

            _ = DownloadFileInternalAsync(address, fileName, cts);
        }

        private async Task DownloadFileInternalAsync(Uri address, String fileName, CancellationTokenSource cts)
        {
            Exception error = null;
            bool cancelled = false;

            try
            {
                using (HttpResponseMessage response = await HttpClients.GetWithDohFallbackAsync(_client, address, HttpCompletionOption.ResponseHeadersRead, cts.Token).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    Int64 totalBytes = response.Content.Headers.ContentLength ?? -1;
                    Int64 bytesReceived = 0;

                    using (Stream input = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (FileStream output = File.Create(fileName))
                    {
                        Byte[] buffer = new Byte[32 * 1024];
                        Int32 read;
                        while ((read = await input.ReadAsync(buffer, 0, buffer.Length, cts.Token).ConfigureAwait(false)) > 0)
                        {
                            await output.WriteAsync(buffer, 0, read, cts.Token).ConfigureAwait(false);
                            bytesReceived += read;
                            RaiseDownloadProgressChanged(bytesReceived, totalBytes);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                cancelled = true;
                _log.Warn("Download was cancelled for {Uri}.", address);
            }
            catch (Exception ex)
            {
                error = ex;
                _log.Error(ex, "Download failed for {Uri}.", address);
            }
            finally
            {
                lock (_stateLock)
                {
                    _isBusy = false;
                    if (ReferenceEquals(_downloadCts, cts))
                    {
                        _downloadCts.Dispose();
                        _downloadCts = null;
                    }
                }

                if (error == null && !cancelled)
                    _log.Info("Download completed successfully for {Uri}.", address);

                DownloadFileCompleted?.Invoke(this, new AsyncCompletedEventArgs(error, cancelled, null));
            }
        }

        private void RaiseDownloadProgressChanged(Int64 bytesReceived, Int64 totalBytesToReceive)
        {
            if (_updatePending)
                return;

            _updatePending = true;
            DownloadProgressChanged?.Invoke(this, new ThrottledDownloadProgressChangedEventArgs(bytesReceived, totalBytesToReceive));
        }

        public void Dispose()
        {
            lock (_stateLock)
            {
                _downloadCts?.Cancel();
                _downloadCts?.Dispose();
                _downloadCts = null;
                _isBusy = false;
            }

            _timer.Stop();
            _timer.Elapsed -= OnTimerElapsed;
            _timer.Dispose();
            _client.Dispose();
        }

        private void OnTimerElapsed(Object sender, ElapsedEventArgs e)
        {
            _updatePending = false;
        }
    }
    #endregion
}
