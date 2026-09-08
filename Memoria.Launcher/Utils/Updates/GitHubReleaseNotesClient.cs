using NLog;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Memoria.Launcher.Utils.Downloads;

namespace Memoria.Launcher.Utils.Updates
{
    internal sealed class GitHubReleaseNotesClient : IDisposable
    {
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(20);
        private static readonly Uri StableReleaseApi = new Uri("https://api.github.com/repos/Albeoris/Memoria/releases/latest");
        private static readonly Uri CanaryReleaseApi = new Uri("https://api.github.com/repos/Albeoris/Memoria/releases/tags/canary");
        private readonly Logger _log = AppLogger.GetLogger(nameof(GitHubReleaseNotesClient));
        private readonly HttpClient _httpClient = ResilientHttpClient.CreateClient();
        private Boolean _disposed;

        public async Task<ReleaseNotesDocument> GetAsync(UpdateBuild build, CancellationToken cancellationToken)
        {
            if (build == null)
                throw new ArgumentNullException(nameof(build));
            if (_disposed)
                throw new ObjectDisposedException(nameof(GitHubReleaseNotesClient));

            Uri source = build.Kind == UpdateBuildKind.Stable ? StableReleaseApi : CanaryReleaseApi;
            try
            {
                using CancellationTokenSource requestCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                requestCancellation.CancelAfter(RequestTimeout);
                using HttpResponseMessage response = await ResilientHttpClient.GetAsync(_httpClient, source, HttpCompletionOption.ResponseContentRead, requestCancellation.Token).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw FileDownloader.CreateHttpResponseException(source, response);

                String json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                GitHubReleaseDto release = Deserialize(json);
                if (!Uri.TryCreate(release.HtmlUrl, UriKind.Absolute, out Uri releasePage))
                    throw new FormatException($"The GitHub release contains an invalid page URI: '{release.HtmlUrl}'.");

                String title = String.IsNullOrWhiteSpace(release.Name) ? release.TagName : release.Name;
                ReleaseNotesDocument result = new ReleaseNotesDocument(build, title, release.TagName, releasePage, GitHubReleaseDescriptionParser.Parse(release.Body));
                _log.Info("Loaded release notes. Build: {Build}, Tag: {Tag}, Uri: {Uri}", build.Name, release.TagName, source);
                return result;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                _log.Error(exception, "Unable to load release notes. Build: {Build}, Uri: {Uri}", build.Name, source);
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            ResilientHttpClient.DisposeClient(_httpClient);
        }

        private static GitHubReleaseDto Deserialize(String json)
        {
            if (String.IsNullOrWhiteSpace(json))
                throw new FormatException("The GitHub release response is empty.");

            using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            if (!(new DataContractJsonSerializer(typeof(GitHubReleaseDto)).ReadObject(stream) is GitHubReleaseDto release))
                throw new FormatException("The GitHub release response could not be deserialized.");
            if (String.IsNullOrWhiteSpace(release.TagName))
                throw new FormatException("The GitHub release has no tag name.");
            return release;
        }

        [DataContract]
        private sealed class GitHubReleaseDto
        {
            [DataMember(Name = "name")]
            public String Name { get; set; } = String.Empty;
            [DataMember(Name = "tag_name")]
            public String TagName { get; set; } = String.Empty;
            [DataMember(Name = "html_url")]
            public String HtmlUrl { get; set; } = String.Empty;
            [DataMember(Name = "body")]
            public String Body { get; set; } = String.Empty;
        }
    }
}
