#nullable disable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModValidationService
    {
        private readonly IReadOnlyList<IModFileValidator> _validators;
        private readonly Int32 _workerCount;

        public ModValidationService(IEnumerable<IModFileValidator> validators, Int32? workerCount = null)
        {
            _validators = (validators ?? throw new ArgumentNullException(nameof(validators))).ToList();
            _workerCount = workerCount ?? Environment.ProcessorCount;
            if (_workerCount < 1)
                throw new ArgumentOutOfRangeException(nameof(workerCount));
        }

        public async Task<ModValidationReport> ValidateDirectoryAsync(String rootDirectory, String rootName, IProgress<ModValidationProgress> progress, CancellationToken cancellationToken)
        {
            String normalizedRoot = Path.GetFullPath(rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory)));
            if (!Directory.Exists(normalizedRoot))
                throw new DirectoryNotFoundException($"The mod directory does not exist: {normalizedRoot}");

            Int32 discoveredFiles = 0;
            Int32 completedFiles = 0;
            Int32 enumerationCompleted = 0;
            ConcurrentBag<ModValidationResult> results = new();
            ProgressReporter progressReporter = new(progress);
            using (BlockingCollection<ModValidationFile> queue = new(Math.Max(16, _workerCount * 4)))
            {
                Task producer = Task.Run(() =>
                {
                    try
                    {
                        foreach (String fullPath in EnumerateFiles(normalizedRoot, cancellationToken))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            ModValidationFile file = new(normalizedRoot, fullPath);
                            queue.Add(file, cancellationToken);
                            Int32 discovered = Interlocked.Increment(ref discoveredFiles);
                            progressReporter.Report(file.RelativePath, discovered, Volatile.Read(ref completedFiles), false, null);
                        }
                    }
                    finally
                    {
                        Volatile.Write(ref enumerationCompleted, 1);
                        queue.CompleteAdding();
                        progressReporter.Report(String.Empty, Volatile.Read(ref discoveredFiles), Volatile.Read(ref completedFiles), true, null, true);
                    }
                });

                Task[] workers = Enumerable.Range(0, _workerCount).Select(_ => Task.Run(() =>
                {
                    foreach (ModValidationFile file in queue.GetConsumingEnumerable(cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        progressReporter.Report(file.RelativePath, Volatile.Read(ref discoveredFiles), Volatile.Read(ref completedFiles), Volatile.Read(ref enumerationCompleted) != 0, null);
                        ModValidationResult result = ValidateFile(file, cancellationToken);
                        results.Add(result);
                        Int32 completed = Interlocked.Increment(ref completedFiles);
                        progressReporter.Report(file.RelativePath, Volatile.Read(ref discoveredFiles), completed, Volatile.Read(ref enumerationCompleted) != 0, result);
                    }
                })).ToArray();

                await Task.WhenAll(workers.Concat(new[] { producer }));
                progressReporter.Report(String.Empty, Volatile.Read(ref discoveredFiles), Volatile.Read(ref completedFiles), true, null, true);
            }

            return new ModValidationReport(rootName, results);
        }

        public Task<ModValidationResult> ValidateFileAsync(ModValidationFile file, CancellationToken cancellationToken)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            return Task.Run(() => ValidateFile(file, cancellationToken), cancellationToken);
        }

        public Boolean CanValidate(ModValidationFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            return _validators.Any(item => item.CanValidate(file));
        }

        private ModValidationResult ValidateFile(ModValidationFile file, CancellationToken cancellationToken)
        {
            IModFileValidator validator = _validators.FirstOrDefault(item => item.CanValidate(file));
            if (validator == null)
                return new ModValidationResult(file, ModValidationStatus.Skipped, String.Empty, new[] { new ModValidationDiagnostic(ModValidationDiagnosticSeverity.Information, "This file is not included in the current validation pass.") });

            try
            {
                return validator.Validate(file, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                return new ModValidationResult(file, ModValidationStatus.Error, validator.Name, new[] { new ModValidationDiagnostic(ModValidationDiagnosticSeverity.Error, exception.ToString()) });
            }
        }

        private static IEnumerable<String> EnumerateFiles(String rootDirectory, CancellationToken cancellationToken)
        {
            Stack<String> directories = new();
            directories.Push(rootDirectory);
            while (directories.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                String directory = directories.Pop();
                foreach (String entry in Directory.EnumerateFileSystemEntries(directory))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    FileAttributes attributes = File.GetAttributes(entry);
                    if ((attributes & FileAttributes.ReparsePoint) != 0)
                        continue;
                    if ((attributes & FileAttributes.Directory) != 0)
                        directories.Push(entry);
                    else
                        yield return entry;
                }
            }
        }

        private sealed class ProgressReporter
        {
            private static readonly Int64 MinimumInterval = Math.Max(1, Stopwatch.Frequency / 10);
            private readonly IProgress<ModValidationProgress> _progress;
            private Int64 _lastTimestamp;

            public ProgressReporter(IProgress<ModValidationProgress> progress)
            {
                _progress = progress;
            }

            public void Report(String currentFile, Int32 discoveredFiles, Int32 completedFiles, Boolean enumerationCompleted, ModValidationResult completedResult, Boolean force = false)
            {
                if (_progress == null)
                    return;

                Int64 now = Stopwatch.GetTimestamp();
                Int64 previous = Interlocked.Read(ref _lastTimestamp);
                if (!force && now - previous < MinimumInterval)
                    return;
                if (!force && Interlocked.CompareExchange(ref _lastTimestamp, now, previous) != previous)
                    return;
                if (force)
                    Interlocked.Exchange(ref _lastTimestamp, now);
                _progress.Report(new ModValidationProgress(currentFile, discoveredFiles, completedFiles, enumerationCompleted, completedResult));
            }
        }
    }
}
