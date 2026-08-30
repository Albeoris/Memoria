#nullable enable

using System;
using System.IO;
using Memoria.Launcher.Utils.IO;

namespace Memoria.Launcher.Utils.Mods
{
    internal abstract class ModDownloadFormat
    {
        private const String SingleFilePrefix = "SingleFileWithPath:";

        public abstract ModPackageType PackageType { get; }

        public abstract void ValidateExtension(String actualExtension);

        public static ModDownloadFormat Parse(String? downloadFormat)
        {
            if (String.IsNullOrWhiteSpace(downloadFormat))
                return new UnspecifiedArchiveModDownloadFormat();

            String value = downloadFormat!.Trim();
            if (value.StartsWith(SingleFilePrefix, StringComparison.OrdinalIgnoreCase))
            {
                String filePath = value.Substring(SingleFilePrefix.Length);
                return new SingleFileModDownloadFormat(SafeRelativePath.Parse(filePath, nameof(downloadFormat)));
            }

            if (value.StartsWith(".", StringComparison.Ordinal))
                throw new ArgumentException("DownloadFormat must contain an extension without a leading period.", nameof(downloadFormat));
            if (value.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || value.IndexOfAny(new[] { '/', '\\' }) >= 0)
                throw new ArgumentException("DownloadFormat is not a valid file extension.", nameof(downloadFormat));

            return new ExpectedArchiveModDownloadFormat(value);
        }
    }

    internal abstract class ArchiveModDownloadFormat : ModDownloadFormat
    {
        public sealed override ModPackageType PackageType => ModPackageType.Archive;

        public sealed override void ValidateExtension(String actualExtension)
        {
            if (String.IsNullOrWhiteSpace(actualExtension))
                throw new InvalidDataException("The downloaded file does not have an extension.");
            ValidateExpectedExtension(actualExtension);
        }

        protected abstract void ValidateExpectedExtension(String actualExtension);
    }

    internal sealed class UnspecifiedArchiveModDownloadFormat : ArchiveModDownloadFormat
    {
        protected override void ValidateExpectedExtension(String actualExtension)
        {
        }
    }

    internal sealed class ExpectedArchiveModDownloadFormat : ArchiveModDownloadFormat
    {
        public ExpectedArchiveModDownloadFormat(String expectedExtension)
        {
            if (String.IsNullOrWhiteSpace(expectedExtension))
                throw new ArgumentException("The expected archive extension cannot be empty or whitespace.", nameof(expectedExtension));

            ExpectedExtension = expectedExtension;
        }

        public String ExpectedExtension { get; }

        protected override void ValidateExpectedExtension(String actualExtension)
        {
            if (!ExpectedExtension.Equals(actualExtension, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException($"The downloaded file extension '{actualExtension}' does not match DownloadFormat '{ExpectedExtension}'.");
            }
        }
    }

    internal sealed class SingleFileModDownloadFormat : ModDownloadFormat
    {
        public SingleFileModDownloadFormat(SafeRelativePath filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public override ModPackageType PackageType => ModPackageType.SingleFile;
        public SafeRelativePath FilePath { get; }

        public override void ValidateExtension(String actualExtension)
        {
            // The target path is defined by the catalog and does not depend on the server-side file name.
        }
    }
}
