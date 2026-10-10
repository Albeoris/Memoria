using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Memoria.Launcher.Utils.Updates
{
    internal static class GitHubReleaseAssetDigest
    {
        public static String Parse(String json, String assetName)
        {
            if (String.IsNullOrWhiteSpace(json))
                throw new FormatException("The GitHub release response is empty.");
            if (String.IsNullOrWhiteSpace(assetName))
                throw new ArgumentException("The asset name cannot be empty.", nameof(assetName));

            GitHubReleaseDto release;
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                if (!(new DataContractJsonSerializer(typeof(GitHubReleaseDto)).ReadObject(stream) is GitHubReleaseDto parsedRelease))
                    throw new FormatException("The GitHub release response could not be deserialized.");
                release = parsedRelease;
            }

            GitHubReleaseAssetDto matchingAsset = null;
            foreach (GitHubReleaseAssetDto asset in release.Assets ?? Array.Empty<GitHubReleaseAssetDto>())
            {
                if (!String.Equals(asset.Name, assetName, StringComparison.Ordinal))
                    continue;
                if (matchingAsset != null)
                    throw new FormatException($"The GitHub release contains multiple assets named '{assetName}'.");
                matchingAsset = asset;
            }

            if (matchingAsset == null)
                throw new FormatException($"The GitHub release does not contain the '{assetName}' asset.");

            const String prefix = "sha256:";
            String digest = matchingAsset.Digest ?? String.Empty;
            if (!digest.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || digest.Length != prefix.Length + 64)
                throw new FormatException($"The GitHub asset '{assetName}' does not have a valid SHA-256 digest.");

            String hexDigest = digest.Substring(prefix.Length);
            foreach (Char character in hexDigest)
            {
                if (!Uri.IsHexDigit(character))
                    throw new FormatException($"The GitHub asset '{assetName}' does not have a valid SHA-256 digest.");
            }

            return hexDigest.ToLowerInvariant();
        }

        [DataContract]
        private sealed class GitHubReleaseDto
        {
            [DataMember(Name = "assets")]
            public GitHubReleaseAssetDto[] Assets { get; set; }
        }

        [DataContract]
        private sealed class GitHubReleaseAssetDto
        {
            [DataMember(Name = "name")]
            public String Name { get; set; }
            [DataMember(Name = "digest")]
            public String Digest { get; set; }
        }
    }
}