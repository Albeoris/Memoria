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
        // TODO: Remove this legacy Stable timestamp correction after the next Stable release publishes manifest.json.
        private static readonly DateTime LegacyStableAssetTimeUtc = new DateTime(2025, 7, 13, 21, 55, 2, DateTimeKind.Utc);
        private static readonly DateTime LegacyStableBuildTimeUtc = new DateTime(2025, 7, 13, 22, 50, 2, DateTimeKind.Utc);
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
                try
                {
                    return await GetFromManifestAsync(build, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    _log.Warn(exception, "Unable to read the update manifest; falling back to legacy HEAD metadata. Build: {Build}, ManifestUri: {ManifestUri}, PatcherUri: {PatcherUri}", build.Name, build.ManifestSource, build.Source);
                }

                return await GetFromLegacyHeadersAsync(build, cancellationToken).ConfigureAwait(false);
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

        private async Task<UpdateBuildInfo> GetFromManifestAsync(UpdateBuild build, CancellationToken cancellationToken)
        {
            using CancellationTokenSource requestCancellation = CreateRequestCancellation(cancellationToken);
            using HttpResponseMessage response = await ResilientHttpClient.GetAsync(_httpClient, build.ManifestSource, HttpCompletionOption.ResponseContentRead, requestCancellation.Token).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                throw FileDownloader.CreateHttpResponseException(build.ManifestSource, response);

            String json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            UpdateManifest manifest = UpdateManifest.Parse(json);
            _log.Info("Update metadata loaded from manifest. Build: {Build}, BuildTimeUtc: {BuildTimeUtc:O}, PatcherSize: {PatcherSize}, Uri: {Uri}", build.Name, manifest.BuildTime, manifest.PatcherSize, build.ManifestSource);
            return new UpdateBuildInfo(build, manifest.BuildTime, manifest.PatcherSize);
        }

        private async Task<UpdateBuildInfo> GetFromLegacyHeadersAsync(UpdateBuild build, CancellationToken cancellationToken)
        {
            using CancellationTokenSource requestCancellation = CreateRequestCancellation(cancellationToken);
            using HttpResponseMessage response = await ResilientHttpClient.SendAsync(_httpClient, HttpMethod.Head, build.Source, HttpCompletionOption.ResponseHeadersRead, requestCancellation.Token).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                throw FileDownloader.CreateHttpResponseException(build.Source, response);

            DateTime? assetTimeUtc = response.Content.Headers.LastModified?.UtcDateTime;
            if (!assetTimeUtc.HasValue)
                throw new DownloadException(DownloadFailureKind.InvalidResponseMetadata, $"The update server did not provide a publication time for the {build.Name} build.");

            DateTime buildTimeUtc = CorrectLegacyStableBuildTime(build, assetTimeUtc.Value);
            _log.Info("Update metadata loaded from legacy HEAD response. Build: {Build}, AssetTimeUtc: {AssetTimeUtc:O}, BuildTimeUtc: {BuildTimeUtc:O}, Uri: {Uri}", build.Name, assetTimeUtc.Value, buildTimeUtc, build.Source);
            return new UpdateBuildInfo(build, buildTimeUtc, response.Content.Headers.ContentLength ?? -1);
        }

        private static DateTime CorrectLegacyStableBuildTime(UpdateBuild build, DateTime assetTimeUtc)
        {
            return build.Kind == UpdateBuildKind.Stable && assetTimeUtc == LegacyStableAssetTimeUtc ? LegacyStableBuildTimeUtc : assetTimeUtc;
        }

        private static CancellationTokenSource CreateRequestCancellation(CancellationToken cancellationToken)
        {
            CancellationTokenSource requestCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            requestCancellation.CancelAfter(RequestTimeout);
            return requestCancellation;
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
