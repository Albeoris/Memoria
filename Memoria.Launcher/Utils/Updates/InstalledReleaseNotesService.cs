using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Memoria.Launcher.Utils.Updates
{
    internal enum InstalledReleaseLookupStatus
    {
        Found,
        NoMatch,
        Unavailable
    }

    internal sealed class InstalledReleaseLookupResult
    {
        private InstalledReleaseLookupResult(InstalledReleaseLookupStatus status, ReleaseNotesDocument releaseNotes)
        {
            Status = status;
            ReleaseNotes = releaseNotes;
        }

        public InstalledReleaseLookupStatus Status { get; }
        public ReleaseNotesDocument ReleaseNotes { get; }
        public Boolean IsDefinitive => Status != InstalledReleaseLookupStatus.Unavailable;

        public static InstalledReleaseLookupResult Found(ReleaseNotesDocument releaseNotes) => new InstalledReleaseLookupResult(InstalledReleaseLookupStatus.Found, releaseNotes ?? throw new ArgumentNullException(nameof(releaseNotes)));
        public static InstalledReleaseLookupResult NoMatch() => new InstalledReleaseLookupResult(InstalledReleaseLookupStatus.NoMatch, null);
        public static InstalledReleaseLookupResult Unavailable() => new InstalledReleaseLookupResult(InstalledReleaseLookupStatus.Unavailable, null);
    }

    internal sealed class InstalledReleaseNotesService
    {
        private readonly Logger _log = AppLogger.GetLogger(nameof(InstalledReleaseNotesService));

        public async Task<InstalledReleaseLookupResult> FindAsync(DateTime installedVersion, CancellationToken cancellationToken = default)
        {
            Boolean allBuildsAvailable = true;
            UpdateBuild matchingBuild = null;
            using (LauncherUpdateMetadataClient metadataClient = new LauncherUpdateMetadataClient())
            {
                foreach (UpdateBuild build in UpdateBuildCatalog.All)
                {
                    try
                    {
                        UpdateBuildInfo buildInfo = await metadataClient.GetAsync(build, cancellationToken).ConfigureAwait(false);
                        if (UpdateVersionComparer.Compare(installedVersion, buildInfo.PublishedAtUtc) == UpdateVersionRelation.Reinstall)
                        {
                            matchingBuild = build;
                            break;
                        }
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        allBuildsAvailable = false;
                        _log.Warn(exception, "Unable to compare the installed version with the {Build} release.", build.Name);
                    }
                }
            }

            if (matchingBuild == null)
            {
                _log.Info("No published release matches the installed build {InstalledVersion:O}. MetadataComplete: {MetadataComplete}", Normalize(installedVersion), allBuildsAvailable);
                return allBuildsAvailable ? InstalledReleaseLookupResult.NoMatch() : InstalledReleaseLookupResult.Unavailable();
            }

            try
            {
                using GitHubReleaseNotesClient releaseClient = new GitHubReleaseNotesClient();
                ReleaseNotesDocument releaseNotes = await releaseClient.GetAsync(matchingBuild, cancellationToken).ConfigureAwait(false);
                return InstalledReleaseLookupResult.Found(releaseNotes);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                _log.Warn(exception, "The installed build was identified as {Build}, but its release notes could not be loaded.", matchingBuild.Name);
                return InstalledReleaseLookupResult.Unavailable();
            }
        }

        private static DateTime Normalize(DateTime value)
        {
            return value.Kind == DateTimeKind.Utc ? value : value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}
