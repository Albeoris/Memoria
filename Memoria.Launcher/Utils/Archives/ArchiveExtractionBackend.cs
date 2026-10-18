using System;
using System.IO;

namespace Memoria.Launcher.Utils.Archives
{
    internal enum ArchiveExtractionBackend
    {
        SevenZip,
        SharpCompress
    }

    internal static class ArchiveExtractionBackendSelector
    {
        public static ArchiveExtractionBackend Select(String archivePath)
        {
            if (archivePath == null)
                throw new ArgumentNullException(nameof(archivePath));
            if (String.IsNullOrWhiteSpace(archivePath))
                throw new ArgumentException("The archive path cannot be empty or whitespace.", nameof(archivePath));

            return String.Equals(Path.GetExtension(archivePath), ".rar", StringComparison.OrdinalIgnoreCase)
                ? ArchiveExtractionBackend.SharpCompress
                : ArchiveExtractionBackend.SevenZip;
        }
    }
}
