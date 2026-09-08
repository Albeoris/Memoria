using NLog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Memoria.Launcher.Utils.Downloads;

namespace Memoria.Launcher.Utils.Updates
{
    internal sealed class LauncherUpdateService
    {
        private const String PatcherFileName = "Memoria.Patcher.exe";
        private readonly Logger _log = AppLogger.GetLogger(nameof(LauncherUpdateService));

        public async Task<UpdateBuildInfo> CheckAsync(UpdateBuild build, CancellationToken cancellationToken = default)
        {
            if (build == null)
                throw new ArgumentNullException(nameof(build));

            try
            {
                using LauncherUpdateMetadataClient metadataClient = new LauncherUpdateMetadataClient();
                return await metadataClient.GetAsync(build, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (!(exception is OperationCanceledException && cancellationToken.IsCancellationRequested))
            {
                _log.Error(exception, "Unable to check the {Build} launcher build. Uri: {Uri}", build.Name, build.Source);
                throw;
            }
        }

        public async Task<PendingUpdatePackage> DownloadAsync(UpdateBuildInfo buildInfo, String applicationDirectory, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
        {
            if (buildInfo == null)
                throw new ArgumentNullException(nameof(buildInfo));
            if (String.IsNullOrWhiteSpace(applicationDirectory))
                throw new ArgumentException("The application directory cannot be empty.", nameof(applicationDirectory));

            String fullDirectory = Path.GetFullPath(applicationDirectory);
            String pendingPath = Path.Combine(fullDirectory, $".{PatcherFileName}.{Guid.NewGuid():N}.pending");
            String targetPath = Path.Combine(fullDirectory, PatcherFileName);
            try
            {
                using FileDownloader downloader = new FileDownloader();
                await downloader.DownloadAsync(buildInfo.Build.Source, pendingPath, progress, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                return new PendingUpdatePackage(pendingPath, targetPath);
            }
            catch (Exception exception) when (!(exception is OperationCanceledException && cancellationToken.IsCancellationRequested))
            {
                _log.Error(exception, "Unable to download the {Build} launcher build. Uri: {Uri}", buildInfo.Build.Name, buildInfo.Build.Source);
                TryDeletePendingFile(pendingPath);
                throw;
            }
            catch
            {
                TryDeletePendingFile(pendingPath);
                throw;
            }
        }

        private void TryDeletePendingFile(String pendingPath)
        {
            if (!File.Exists(pendingPath))
                return;

            try
            {
                File.Delete(pendingPath);
            }
            catch (Exception exception)
            {
                _log.Warn(exception, "Unable to delete a cancelled launcher update. Path: {Path}", pendingPath);
            }
        }
    }
}
