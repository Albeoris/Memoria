using NLog;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading;
using Memoria.Launcher.Utils.IO;
using SharpCompress.Archives;

namespace Memoria.Launcher.Utils.Archives
{
    internal sealed class SevenZipExtractor : IArchiveExtractor
    {
        private readonly Logger _log;
        private readonly EmbeddedSevenZipExecutable _executable;
        private readonly GameRootDirectory _gameRoot;
        private readonly SevenZipProcessRunner _processRunner;

        public SevenZipExtractor(String gameRootPath)
        {
            _log = AppLogger.GetLogger(nameof(SevenZipExtractor));
            _executable = new EmbeddedSevenZipExecutable(Assembly.GetExecutingAssembly());
            _gameRoot = new GameRootDirectory(gameRootPath);
            _processRunner = new SevenZipProcessRunner(_log);
        }

        public void Extract(String archivePath, String destinationPath, CancellationToken cancellationToken, Action<Int32> progress = null)
        {
            String fullArchivePath = ValidatePath(archivePath, nameof(archivePath));
            String fullDestinationPath = ValidatePath(destinationPath, nameof(destinationPath));
            _gameRoot.EnsureContained(fullDestinationPath);
            cancellationToken.ThrowIfCancellationRequested();

            _log.Info("Extracting archive. Archive: {ArchivePath}, Destination: {DestinationPath}", fullArchivePath, fullDestinationPath);
            try
            {
                EnsureAccess(fullArchivePath, fullDestinationPath);
                ValidateArchiveEntries(fullArchivePath);
                String executablePath = _executable.GetPath();

                progress?.Invoke(0);
                _processRunner.Extract(executablePath, fullArchivePath, fullDestinationPath, cancellationToken, progress);
                _gameRoot.EnsureSafeTree(fullDestinationPath);
                progress?.Invoke(100);
                _log.Info("Archive extracted. Archive: {ArchivePath}, Destination: {DestinationPath}", fullArchivePath, fullDestinationPath);
            }
            catch (OperationCanceledException)
            {
                _log.Info("Archive extraction cancelled. Archive: {ArchivePath}", fullArchivePath);
                throw;
            }
            catch (ArchiveExtractionException exception)
            {
                _log.Error(exception, "Archive extraction failed. Archive: {ArchivePath}, Destination: {DestinationPath}", fullArchivePath, fullDestinationPath);
                throw;
            }
            catch (Exception exception) when (IsAccessOrIoFailure(exception))
            {
                ArchiveExtractionException userException = CreateFileSystemException(fullArchivePath, fullDestinationPath, exception);
                _log.Error(userException, "Archive extraction failed. Archive: {ArchivePath}, Destination: {DestinationPath}", fullArchivePath, fullDestinationPath);
                throw userException;
            }
        }

        private static String ValidatePath(String path, String parameterName)
        {
            if (path == null)
                throw new ArgumentNullException(parameterName);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("The path cannot be empty or whitespace.", parameterName);
            if (path.IndexOf('"') >= 0)
                throw new ArgumentException("The path cannot contain a quotation mark.", parameterName);

            try
            {
                return Path.GetFullPath(path);
            }
            catch (Exception exception) when (exception is ArgumentException || exception is NotSupportedException || exception is PathTooLongException)
            {
                throw new ArgumentException("The path is not a valid file-system path.", parameterName, exception);
            }
        }

        private static void EnsureAccess(String archivePath, String destinationPath)
        {
            FileSystemAccessProbe.EnsureReadableFile(archivePath);
            FileSystemAccessProbe.EnsureWritableDirectory(destinationPath);
        }

        private static void ValidateArchiveEntries(String archivePath)
        {
            using (IArchive archive = ArchiveFactory.OpenArchive(archivePath))
            {
                foreach (IArchiveEntry entry in archive.Entries)
                {
                    if (String.IsNullOrWhiteSpace(entry.Key))
                        throw new ArchiveExtractionException("The archive contains an entry without a path.");

                    ArchiveEntryPathPolicy.Validate(entry.Key, entry.LinkTarget);
                }
            }
        }

        private static Boolean IsAccessOrIoFailure(Exception exception)
        {
            return exception is UnauthorizedAccessException
                || exception is SecurityException
                || exception is IOException
                || exception is Win32Exception;
        }

        private static ArchiveExtractionException CreateFileSystemException(String archivePath, String destinationPath, Exception exception)
        {
            if (!File.Exists(archivePath))
            {
                return new ArchiveExtractionException(
                    $"The archive '{archivePath}' no longer exists. Download the mod again and retry the installation.",
                    exception);
            }

            return new ArchiveExtractionException(
                $"Memoria cannot read '{archivePath}' or write to '{destinationPath}'. " +
                "Close applications using these files, check available disk space, remove read-only attributes, " +
                "and allow Memoria Launcher in your antivirus or Windows Controlled Folder Access settings.",
                exception);
        }
    }
}
