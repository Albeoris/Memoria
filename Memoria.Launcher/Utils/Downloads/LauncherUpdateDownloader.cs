using NLog;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Memoria.Launcher.Utils.Downloads
{
    internal sealed class LauncherUpdateDownloader : IDisposable
    {
        private readonly Logger _log;
        private readonly HttpClient _httpClient;
        private readonly FileDownloader _fileDownloader;
        private readonly CancellationToken _cancellationToken;
        private Boolean _disposed;

        public LauncherUpdateDownloader(CancellationToken cancellationToken = default)
        {
            _log = AppLogger.GetLogger(nameof(LauncherUpdateDownloader));
            _httpClient = ResilientHttpClient.CreateClient();
            _fileDownloader = new FileDownloader();
            _cancellationToken = cancellationToken;
        }

        public Boolean IsCancellationRequested
        {
            get
            {
                return _cancellationToken.IsCancellationRequested;
            }
        }

        public Task<RemoteFileInfo> GetRemoteFileInfoAsync(Uri source)
        {
            ValidateSource(source);
            ThrowIfDisposed();
            return GetRemoteFileInfoCoreAsync(source);
        }

        public async Task DownloadAsync(Uri source, String destinationPath, IProgress<Int64> progress = null)
        {
            FileDownloader.ValidateSource(source);
            DownloadDestination destination = DownloadDestination.ForFile(destinationPath);
            ThrowIfDisposed();

            Int64 previouslyReported = 0;
            IProgress<DownloadProgress> byteProgress = progress == null
                ? null
                : new InlineProgress<DownloadProgress>(value =>
                {
                    Int64 increment = value.BytesReceived - previouslyReported;
                    previouslyReported = value.BytesReceived;
                    if (increment > 0)
                        progress.Report(increment);
                });
            await _fileDownloader.DownloadAsync(source, destination, byteProgress, _cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _fileDownloader.Dispose();
            ResilientHttpClient.DisposeClient(_httpClient);
        }

        private async Task<RemoteFileInfo> GetRemoteFileInfoCoreAsync(Uri source)
        {
            try
            {
                using (HttpResponseMessage response = await ResilientHttpClient.SendAsync(
                           _httpClient,
                           HttpMethod.Head,
                           source,
                           HttpCompletionOption.ResponseHeadersRead,
                           _cancellationToken).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                        throw FileDownloader.CreateHttpResponseException(source, response);

                    return new RemoteFileInfo(
                        source,
                        response.Content.Headers.ContentLength ?? -1,
                        response.Content.Headers.LastModified?.UtcDateTime ?? DateTime.MinValue);
                }
            }
            catch (OperationCanceledException) when (_cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (DownloadException exception)
            {
                _log.Error(exception, "Unable to read remote file information. Uri: {Uri}", source);
                throw;
            }
            catch (Exception exception)
            {
                DownloadException userException = new DownloadException(
                    DownloadFailureKind.Network,
                    $"Memoria could not check '{source}' for updates. Check your connection, proxy or firewall settings, " +
                    "and verify that the link opens in a browser.",
                    exception);
                _log.Error(userException, "Unable to read remote file information. Uri: {Uri}", source);
                throw userException;
            }
        }

        private static void ValidateSource(Uri source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.IsAbsoluteUri)
                throw new ArgumentException("The source URI must be absolute.", nameof(source));
            if (source.Scheme != Uri.UriSchemeHttp && source.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("Only HTTP and HTTPS URIs are supported.", nameof(source));
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LauncherUpdateDownloader));
        }

        private sealed class InlineProgress<T> : IProgress<T>
        {
            private readonly Action<T> _report;

            public InlineProgress(Action<T> report)
            {
                _report = report ?? throw new ArgumentNullException(nameof(report));
            }

            public void Report(T value)
            {
                _report(value);
            }
        }
    }
}
