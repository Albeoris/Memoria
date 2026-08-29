using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Memoria.Launcher.Utils.IO;

namespace Memoria.Launcher.Utils.Downloads
{
    public sealed class FileDownloader : IDisposable
    {
        private const Int32 BufferSize = 32 * 1024;
        private const Int32 ProgressIntervalMilliseconds = 100;

        private readonly Logger _log;
        private readonly HttpClient _httpClient;
        private Boolean _disposed;

        public FileDownloader()
        {
            _log = AppLogger.GetLogger(nameof(FileDownloader));
            _httpClient = ResilientHttpClient.CreateClient();
        }

        public Task DownloadAsync(
            Uri source,
            String destinationPath,
            IProgress<DownloadProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            String fullDestinationPath = ValidateArguments(source, destinationPath);
            ThrowIfDisposed();
            return DownloadCoreAsync(source, fullDestinationPath, progress, cancellationToken);
        }

        internal static String ValidateArguments(Uri source, String destinationPath)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.IsAbsoluteUri)
                throw new ArgumentException("The download URI must be absolute.", nameof(source));
            if (source.Scheme != Uri.UriSchemeHttp && source.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("Only HTTP and HTTPS download URIs are supported.", nameof(source));
            if (destinationPath == null)
                throw new ArgumentNullException(nameof(destinationPath));
            if (String.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException("The destination path cannot be empty or whitespace.", nameof(destinationPath));

            try
            {
                return Path.GetFullPath(destinationPath);
            }
            catch (Exception exception) when (exception is ArgumentException || exception is NotSupportedException || exception is PathTooLongException)
            {
                throw new ArgumentException("The destination is not a valid file-system path.", nameof(destinationPath), exception);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            ResilientHttpClient.DisposeClient(_httpClient);
        }

        private async Task DownloadCoreAsync(
            Uri source,
            String destinationPath,
            IProgress<DownloadProgress> progress,
            CancellationToken cancellationToken)
        {
            String stagingPath = null;
            _log.Info("Downloading file. Uri: {Uri}, Destination: {DestinationPath}", source, destinationPath);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                String destinationDirectory = Path.GetDirectoryName(destinationPath);
                FileSystemAccessProbe.EnsureWritableDirectory(destinationDirectory);
                FileSystemAccessProbe.EnsureReplaceableFile(destinationPath);
                stagingPath = Path.Combine(destinationDirectory, $".{Path.GetFileName(destinationPath)}.{Guid.NewGuid():N}.download");

                using (HttpResponseMessage response = await ResilientHttpClient.GetAsync(
                           _httpClient,
                           source,
                           HttpCompletionOption.ResponseHeadersRead,
                           cancellationToken).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                        throw CreateHttpResponseException(source, response);

                    Int64 expectedBytes = response.Content.Headers.ContentLength ?? -1;
                    Int64 receivedBytes = await CopyResponseAsync(response, stagingPath, expectedBytes, progress, cancellationToken).ConfigureAwait(false);
                    if (expectedBytes >= 0 && receivedBytes != expectedBytes)
                    {
                        throw new DownloadException(
                            DownloadFailureKind.IncompleteContent,
                            $"The server sent only {receivedBytes} of {expectedBytes} bytes for '{source}'. " +
                            "Check your connection and free disk space, then download the file again.");
                    }

                    Commit(stagingPath, destinationPath);
                    stagingPath = null;
                    progress?.Report(new DownloadProgress(receivedBytes, expectedBytes));
                    _log.Info("File downloaded. Uri: {Uri}, Destination: {DestinationPath}, Bytes: {Bytes}", source, destinationPath, receivedBytes);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _log.Info("Download cancelled. Uri: {Uri}, Destination: {DestinationPath}", source, destinationPath);
                throw;
            }
            catch (OperationCanceledException exception)
            {
                DownloadException userException = new DownloadException(
                    DownloadFailureKind.Network,
                    $"The download of '{source}' timed out. Check your connection, proxy or firewall settings, then try again.",
                    exception);
                
                _log.Error(userException, "Download failed. Uri: {Uri}, Destination: {DestinationPath}", source, destinationPath);
                throw userException;
            }
            catch (DownloadException exception)
            {
                _log.Error(exception, "Download failed. Uri: {Uri}, Destination: {DestinationPath}", source, destinationPath);
                throw;
            }
            catch (Exception exception) when (exception is UnauthorizedAccessException || exception is SecurityException)
            {
                DownloadException userException = new DownloadException(
                    DownloadFailureKind.AccessDenied,
                    $"Windows denied access to '{destinationPath}'. Close applications using the file, remove its read-only attribute, " +
                    "and allow Memoria Launcher in your antivirus or Windows Controlled Folder Access settings, then try again.",
                    exception);
                _log.Error(userException, "Download failed. Uri: {Uri}, Destination: {DestinationPath}", source, destinationPath);
                throw userException;
            }
            catch (HttpRequestException exception)
            {
                DownloadException userException = new DownloadException(
                    DownloadFailureKind.Network,
                    $"Memoria could not download '{source}'. Check your internet connection, proxy or firewall settings, and whether the link opens in a browser, then try again.",
                    exception);
                _log.Error(userException, "Download failed. Uri: {Uri}, Destination: {DestinationPath}", source, destinationPath);
                throw userException;
            }
            catch (IOException exception)
            {
                DownloadException userException = new DownloadException(
                    DownloadFailureKind.Storage,
                    $"The download could not be saved to '{destinationPath}'. Check available disk space and folder permissions, " +
                    "close applications using the file, and try again.",
                    exception);
                
                _log.Error(userException, "Download failed. Uri: {Uri}, Destination: {DestinationPath}", source, destinationPath);
                throw userException;
            }
            finally
            {
                TryDeleteStagingFile(stagingPath, source);
            }
        }

        private static async Task<Int64> CopyResponseAsync(
            HttpResponseMessage response,
            String stagingPath,
            Int64 totalBytes,
            IProgress<DownloadProgress> progress,
            CancellationToken cancellationToken)
        {
            Int64 bytesReceived = 0;
            Byte[] buffer = new Byte[BufferSize];
            Stopwatch progressClock = Stopwatch.StartNew();

            using Stream input = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using FileStream output = new FileStream(
                stagingPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                BufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            while (true)
            {
                Int32 read = await input.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                if (read == 0)
                    break;

                await output.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                bytesReceived += read;
                if (progressClock.ElapsedMilliseconds < ProgressIntervalMilliseconds)
                    continue;

                progressClock.Restart();
                progress?.Report(new DownloadProgress(bytesReceived, totalBytes));
            }

            await output.FlushAsync(cancellationToken).ConfigureAwait(false);
            return bytesReceived;
        }

        private static void Commit(String stagingPath, String destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                File.Replace(stagingPath, destinationPath, destinationBackupFileName: null);
                return;
            }

            File.Move(stagingPath, destinationPath);
        }

        internal static DownloadException CreateHttpResponseException(Uri source, HttpResponseMessage response)
        {
            Int32 statusCode = (Int32)response.StatusCode;
            String advice = statusCode == 401 || statusCode == 403
                ? "The link requires permission or has expired. Open it in a browser and ask the mod author for a public download link."
                : statusCode == 404
                    ? "The file was not found. Check the link or ask the mod author to publish a new one."
                    : "Try opening the link in a browser. If it is unavailable there too, wait and retry or contact the mod author.";

            return new DownloadException(
                DownloadFailureKind.HttpResponse,
                $"The server refused the download of '{source}' with HTTP {statusCode} ({response.ReasonPhrase}). {advice}");
        }

        private void TryDeleteStagingFile(String stagingPath, Uri source)
        {
            if (String.IsNullOrEmpty(stagingPath) || !File.Exists(stagingPath))
                return;

            try
            {
                File.Delete(stagingPath);
            }
            catch (Exception exception)
            {
                _log.Warn(exception, "Unable to delete an incomplete download. Uri: {Uri}, StagingPath: {StagingPath}", source, stagingPath);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FileDownloader));
        }
    }
}
