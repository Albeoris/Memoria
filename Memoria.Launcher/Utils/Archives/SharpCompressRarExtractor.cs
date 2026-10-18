#nullable enable

using NLog;
using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Memoria.Launcher.Utils.IO;

namespace Memoria.Launcher.Utils.Archives
{
    internal sealed class SharpCompressRarExtractor : IArchiveExtractor
    {
        private const Int32 BufferSize = 64 * 1024;

        private readonly GameRootDirectory _gameRoot;
        private readonly Logger _log;

        public SharpCompressRarExtractor(String gameRootPath)
        {
            _gameRoot = new GameRootDirectory(gameRootPath);
            _log = AppLogger.GetLogger(nameof(SharpCompressRarExtractor));
        }

        public void Extract(
            String archivePath,
            String destinationPath,
            CancellationToken cancellationToken,
            Action<Int32>? progress = null)
        {
            String fullArchivePath = GetFullPath(archivePath, nameof(archivePath));
            String fullDestinationPath = GetFullPath(destinationPath, nameof(destinationPath));
            _gameRoot.EnsureContained(fullDestinationPath);
            cancellationToken.ThrowIfCancellationRequested();

            _log.Info("Extracting RAR with SharpCompress. Archive: {ArchivePath}, Destination: {DestinationPath}", fullArchivePath, fullDestinationPath);
            try
            {
                FileSystemAccessProbe.EnsureReadableFile(fullArchivePath);
                FileSystemAccessProbe.EnsureWritableDirectory(fullDestinationPath);

                using (IArchive archive = ArchiveFactory.OpenArchive(fullArchivePath))
                {
                    if (archive.Type != ArchiveType.Rar)
                        throw new ArchiveExtractionException($"The file '{fullArchivePath}' is not a valid RAR archive.");

                    ValidatedArchiveEntry[] entries = ValidateEntries(archive.Entries);
                    ExtractEntries(entries, fullDestinationPath, cancellationToken, progress);
                }

                _gameRoot.EnsureSafeTree(fullDestinationPath);
                progress?.Invoke(100);
                _log.Info("RAR extracted with SharpCompress. Archive: {ArchivePath}, Destination: {DestinationPath}", fullArchivePath, fullDestinationPath);
            }
            catch (OperationCanceledException)
            {
                _log.Info("RAR extraction cancelled. Archive: {ArchivePath}", fullArchivePath);
                throw;
            }
            catch (ArchiveExtractionException exception)
            {
                _log.Error(exception, "RAR extraction failed. Archive: {ArchivePath}, Destination: {DestinationPath}", fullArchivePath, fullDestinationPath);
                throw;
            }
            catch (Exception exception)
            {
                ArchiveExtractionException extractionException = new ArchiveExtractionException(
                    $"SharpCompress could not extract the RAR archive '{fullArchivePath}' to '{fullDestinationPath}'.",
                    exception);
                _log.Error(extractionException, "RAR extraction failed. Archive: {ArchivePath}, Destination: {DestinationPath}", fullArchivePath, fullDestinationPath);
                throw extractionException;
            }
        }

        private static ValidatedArchiveEntry[] ValidateEntries(IEnumerable<IArchiveEntry> entries)
        {
            return entries.Select(entry =>
            {
                if (String.IsNullOrWhiteSpace(entry.Key))
                    throw new ArchiveExtractionException("The RAR archive contains an entry without a path.");

                SafeRelativePath path = ArchiveEntryPathPolicy.Validate(entry.Key, entry.LinkTarget);
                return new ValidatedArchiveEntry(entry, path);
            }).ToArray();
        }

        private void ExtractEntries(
            IReadOnlyCollection<ValidatedArchiveEntry> entries,
            String destinationPath,
            CancellationToken cancellationToken,
            Action<Int32>? progress)
        {
            Int64 totalBytes = entries
                .Where(static entry => !entry.Entry.IsDirectory)
                .Aggregate<ValidatedArchiveEntry, Int64>(0, static (total, entry) => checked(total + entry.Entry.Size));
            Int64 extractedBytes = 0;
            progress?.Invoke(0);

            foreach (ValidatedArchiveEntry validatedEntry in entries)
            {
                cancellationToken.ThrowIfCancellationRequested();
                IArchiveEntry entry = validatedEntry.Entry;
                String outputPath = _gameRoot.ResolveWithin(destinationPath, validatedEntry.Path);
                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(outputPath);
                    continue;
                }

                String parentDirectory = Path.GetDirectoryName(outputPath)
                    ?? throw new ArchiveExtractionException($"The archive entry '{entry.Key}' does not have a valid parent directory.");
                Directory.CreateDirectory(parentDirectory);
                using Stream input = entry.OpenEntryStream();
                using FileStream output = new FileStream(
                    outputPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    BufferSize,
                    FileOptions.SequentialScan);

                extractedBytes = CopyEntry(input, output, extractedBytes, totalBytes, cancellationToken, progress);
            }
        }

        private static Int64 CopyEntry(
            Stream input,
            Stream output,
            Int64 extractedBytes,
            Int64 totalBytes,
            CancellationToken cancellationToken,
            Action<Int32>? progress)
        {
            Byte[] buffer = new Byte[BufferSize];
            Int32 lastProgress = CalculateProgress(extractedBytes, totalBytes);
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Int32 read = input.Read(buffer, 0, buffer.Length);
                if (read == 0)
                    return extractedBytes;

                output.Write(buffer, 0, read);
                extractedBytes += read;
                Int32 currentProgress = CalculateProgress(extractedBytes, totalBytes);
                if (currentProgress == lastProgress)
                    continue;

                lastProgress = currentProgress;
                progress?.Invoke(currentProgress);
            }
        }

        private static Int32 CalculateProgress(Int64 extractedBytes, Int64 totalBytes)
        {
            if (totalBytes <= 0)
                return 0;

            return (Int32)Math.Min(99, (Double)extractedBytes / totalBytes * 100);
        }

        private static String GetFullPath(String path, String parameterName)
        {
            if (path == null)
                throw new ArgumentNullException(parameterName);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("The path cannot be empty or whitespace.", parameterName);

            try
            {
                return Path.GetFullPath(path);
            }
            catch (Exception exception) when (exception is ArgumentException || exception is NotSupportedException || exception is PathTooLongException)
            {
                throw new ArgumentException("The path is not a valid file-system path.", parameterName, exception);
            }
        }

        private sealed class ValidatedArchiveEntry
        {
            public ValidatedArchiveEntry(IArchiveEntry entry, SafeRelativePath path)
            {
                Entry = entry ?? throw new ArgumentNullException(nameof(entry));
                Path = path ?? throw new ArgumentNullException(nameof(path));
            }

            public IArchiveEntry Entry { get; }
            public SafeRelativePath Path { get; }
        }
    }
}
