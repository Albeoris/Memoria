using System;

namespace Memoria.Launcher.Utils.Updates
{
    internal sealed class UpdateBuildInfo
    {
        public UpdateBuildInfo(UpdateBuild build, DateTime publishedAtUtc, Int64 contentLength, String sha256Digest)
        {
            Build = build ?? throw new ArgumentNullException(nameof(build));
            if (publishedAtUtc == DateTime.MinValue)
                throw new ArgumentOutOfRangeException(nameof(publishedAtUtc));
            if (contentLength < -1)
                throw new ArgumentOutOfRangeException(nameof(contentLength));
            if (String.IsNullOrWhiteSpace(sha256Digest))
                throw new ArgumentException("The update SHA-256 digest cannot be empty.", nameof(sha256Digest));

            PublishedAtUtc = NormalizeAsUtc(publishedAtUtc);
            ContentLength = contentLength;
            Sha256Digest = sha256Digest;
        }

        public UpdateBuild Build { get; }
        public DateTime PublishedAtUtc { get; }
        public Int64 ContentLength { get; }
        public String Sha256Digest { get; }

        private static DateTime NormalizeAsUtc(DateTime value)
        {
            return value.Kind == DateTimeKind.Utc ? value : value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}
