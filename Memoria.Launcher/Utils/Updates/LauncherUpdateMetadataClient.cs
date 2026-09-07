using NLog;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Memoria.Launcher.Utils.Downloads;

namespace Memoria.Launcher.Utils.Updates
{
    internal sealed class LauncherUpdateMetadataClient : IDisposable
    {
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(20);
        private readonly Logger _log = AppLogger.GetLogger(nameof(LauncherUpdateMetadataClient));
        private readonly HttpClient _httpClient = ResilientHttpClient.CreateClient();
        private Boolean _disposed;

        public async Task<UpdateBuildInfo> GetAsync(UpdateBuild build, CancellationToken cancellationToken)
        {
            if (build == null)
                throw new ArgumentNullException(nameof(build));
            if (_disposed)
                throw new ObjectDisposedException(nameof(LauncherUpdateMetadataClient));

            try
            {
                using CancellationTokenSource requestCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                requestCancellation.CancelAfter(RequestTimeout);
                using HttpResponseMessage response = await ResilientHttpClient.SendAsync(_httpClient, HttpMethod.Head, build.Source, HttpCompletionOption.ResponseHeadersRead, requestCancellation.Token).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw FileDownloader.CreateHttpResponseException(build.Source, response);

                DateTime? publishedAtUtc = response.Content.Headers.LastModified?.UtcDateTime;
                if (!publishedAtUtc.HasValue)
                    throw new DownloadException(DownloadFailureKind.InvalidResponseMetadata, $"The update server did not provide a publication time for the {build.Name} build.");

                return new UpdateBuildInfo(build, publishedAtUtc.Value, response.Content.Headers.ContentLength ?? -1);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (DownloadException exception)
            {
                _log.Error(exception, "Unable to read update metadata. Build: {Build}, Uri: {Uri}", build.Name, build.Source);
                throw;
            }
            catch (Exception exception)
            {
                DownloadException downloadException = new DownloadException(DownloadFailureKind.Network, $"Memoria could not check the {build.Name} build. Check your connection, proxy or firewall settings and try again.", exception);
                _log.Error(downloadException, "Unable to read update metadata. Build: {Build}, Uri: {Uri}", build.Name, build.Source);
                throw downloadException;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            ResilientHttpClient.DisposeClient(_httpClient);
        }
    }
}
