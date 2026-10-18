using System;
using System.IO;
using System.Security.Cryptography;
using NLog;
using Memoria.Launcher.Utils.Downloads;

namespace Memoria.Launcher.Utils.Updates
{
    internal static class UpdatePackageVerifier
    {
        private static readonly Logger _log = AppLogger.GetLogger(nameof(UpdatePackageVerifier));

        public static void VerifySha256(String path, String expectedDigest)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("The update package path cannot be empty.", nameof(path));
            if (String.IsNullOrWhiteSpace(expectedDigest) || expectedDigest.Length != 64)
            {
                _log.Error("SHA-256 validation failed: GitHub did not provide a valid digest. Path: {Path}, ExpectedDigest: {ExpectedDigest}", path, expectedDigest);
                throw new DownloadException(DownloadFailureKind.InvalidResponseMetadata, "The update server did not provide a valid SHA-256 digest for the installer.");
            }

            String actualDigest;
            using (SHA256 sha256 = SHA256.Create())
            using (FileStream stream = File.OpenRead(path))
                actualDigest = BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", String.Empty).ToLowerInvariant();

            if (!String.Equals(actualDigest, expectedDigest, StringComparison.OrdinalIgnoreCase))
            {
                _log.Error("SHA-256 validation failed for the downloaded installer. Path: {Path}, ExpectedDigest: {ExpectedDigest}, ActualDigest: {ActualDigest}", path, expectedDigest, actualDigest);
                throw new DownloadException(DownloadFailureKind.UnexpectedContent, "The downloaded installer failed its SHA-256 integrity check.");
            }

            _log.Info("SHA-256 validated for the downloaded installer. Path: {Path}, Digest: {Digest}", path, actualDigest);
        }
    }
}