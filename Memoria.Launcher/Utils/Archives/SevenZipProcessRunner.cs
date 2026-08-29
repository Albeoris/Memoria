using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Memoria.Launcher.Utils.Archives
{
    internal sealed class SevenZipProcessRunner
    {
        private const Int32 PollIntervalMilliseconds = 100;
        private const Int32 MaximumCapturedOutputCharacters = 32 * 1024;

        private readonly Logger _log;

        public SevenZipProcessRunner(Logger log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public void Extract(
            String executablePath,
            String archivePath,
            String destinationPath,
            CancellationToken cancellationToken,
            Action<Int32> progress)
        {
            using Process process = CreateProcess(executablePath, archivePath, destinationPath);
            StringBuilder standardError = new StringBuilder();
            StringBuilder standardOutput = new StringBuilder();
            Int32 latestProgress = -1;

            process.OutputDataReceived += (_, args) =>
            {
                AppendLine(standardOutput, args.Data);
                if (TryParseProgress(args.Data, out Int32 value))
                    Interlocked.Exchange(ref latestProgress, value);
            };
            process.ErrorDataReceived += (_, args) => AppendLine(standardError, args.Data);

            try
            {
                process.Start();
            }
            catch (Win32Exception exception)
            {
                throw CreateStartException(executablePath, exception);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            Int32 reportedProgress = -1;
            Boolean terminationRequested = false;
            while (!process.WaitForExit(PollIntervalMilliseconds))
            {
                ReportProgress(progress, ref reportedProgress, Volatile.Read(ref latestProgress));
                if (!cancellationToken.IsCancellationRequested || terminationRequested)
                    continue;

                terminationRequested = true;
                TryTerminate(process, archivePath);
            }

            // The parameterless call waits until the asynchronous output readers are fully drained.
            process.WaitForExit();
            ReportProgress(progress, ref reportedProgress, Volatile.Read(ref latestProgress));

            cancellationToken.ThrowIfCancellationRequested();
            SevenZipExitCode exitCode = (SevenZipExitCode)process.ExitCode;
            if (exitCode != SevenZipExitCode.Success)
                throw CreateExtractionException(exitCode, standardError, standardOutput);
        }

        private static Process CreateProcess(String executablePath, String archivePath, String destinationPath)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = $"x -bsp1 -aoa -o\"{destinationPath}\" \"{archivePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = destinationPath
                }
            };
        }

        private static void AppendLine(StringBuilder output, String line)
        {
            if (String.IsNullOrWhiteSpace(line))
                return;

            lock (output)
            {
                output.AppendLine(line);
                if (output.Length > MaximumCapturedOutputCharacters)
                    output.Remove(0, output.Length - MaximumCapturedOutputCharacters);
            }
        }

        private static Boolean TryParseProgress(String line, out Int32 progress)
        {
            progress = 0;
            if (String.IsNullOrWhiteSpace(line))
                return false;

            Int32 percentIndex = line.IndexOf('%');
            if (percentIndex < 0)
                return false;

            String value = line.Substring(0, percentIndex).Trim();
            Int32 lastSpace = value.LastIndexOf(' ');
            if (lastSpace >= 0)
                value = value.Substring(lastSpace + 1);

            return Int32.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out progress)
                && progress >= 0
                && progress <= 100;
        }

        private static void ReportProgress(Action<Int32> callback, ref Int32 reportedProgress, Int32 latestProgress)
        {
            if (callback == null || latestProgress < 0 || latestProgress == reportedProgress)
                return;

            reportedProgress = latestProgress;
            callback(latestProgress);
        }

        private void TryTerminate(Process process, String archivePath)
        {
            try
            {
                if (!process.HasExited)
                    process.Kill();
            }
            catch (Exception exception)
            {
                // Cancellation must never crash the process from an asynchronous callback.
                _log.Warn(exception, "Unable to terminate 7-Zip after cancellation. Archive: {ArchivePath}, ProcessId: {ProcessId}", archivePath, SafeGetProcessId(process));
            }
        }

        private static Int32 SafeGetProcessId(Process process)
        {
            try
            {
                return process.Id;
            }
            catch
            {
                return -1;
            }
        }

        private static ArchiveExtractionException CreateStartException(String executablePath, Win32Exception exception)
        {
            if (exception.NativeErrorCode == 5)
            {
                return new ArchiveExtractionException(
                    $"Windows denied permission to start 7-Zip from '{executablePath}'. " +
                    "Allow Memoria Launcher and 7za.exe in your antivirus or Windows Controlled Folder Access settings, " +
                    "then try again. Running the launcher from a folder owned by your account may also help.",
                    exception);
            }

            return new ArchiveExtractionException(
                "Memoria could not start 7-Zip. Reinstall or update the launcher and check whether antivirus software quarantined 7za.exe.",
                exception);
        }

        private static ArchiveExtractionException CreateExtractionException(
            SevenZipExitCode exitCode,
            StringBuilder standardError,
            StringBuilder standardOutput)
        {
            String details = GetLastUsefulLine(standardError);
            if (String.IsNullOrWhiteSpace(details))
                details = GetLastUsefulLine(standardOutput);

            String recommendation = exitCode switch
            {
                SevenZipExitCode.Warning => "The archive was only partially extracted. Download it again and verify that enough disk space is available.",
                SevenZipExitCode.FatalError => "The archive may be incomplete or corrupt. Download it again; if the problem persists, contact the mod author.",
                SevenZipExitCode.CommandLineError => "The bundled 7-Zip installation may be damaged. Reinstall or update Memoria Launcher.",
                SevenZipExitCode.NotEnoughMemory => "Close other applications or increase the Windows paging file, then try again.",
                SevenZipExitCode.Cancelled => "Extraction was stopped. Start the installation again when ready.",
                _ => "Check the archive, available disk space, antivirus settings, and write permission for the destination, then try again."
            };
            String diagnostic = String.IsNullOrWhiteSpace(details) ? String.Empty : $" 7-Zip reported: {details}";

            return new ArchiveExtractionException($"7-Zip could not extract the archive (exit code {(Int32)exitCode}). {recommendation}{diagnostic}");
        }

        private static String GetLastUsefulLine(StringBuilder output)
        {
            String text;
            lock (output)
                text = output.ToString().Trim();

            if (text.Length == 0)
                return null;

            String[] lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Length == 0 ? null : lines[lines.Length - 1].Trim();
        }
    }
}
