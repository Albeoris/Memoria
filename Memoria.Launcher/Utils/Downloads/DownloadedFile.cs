using System;
using System.IO;

namespace Memoria.Launcher.Utils.Downloads
{
    public sealed class DownloadedFile
    {
        public DownloadedFile(Uri source, Uri effectiveSource, String fullPath)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            EffectiveSource = effectiveSource ?? throw new ArgumentNullException(nameof(effectiveSource));
            if (!source.IsAbsoluteUri || !effectiveSource.IsAbsoluteUri)
                throw new ArgumentException("Download URIs must be absolute.");
            if (String.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("The downloaded file path cannot be empty or whitespace.", nameof(fullPath));

            FullPath = Path.GetFullPath(fullPath);
            FileName = Path.GetFileName(FullPath);
            Extension = Path.GetExtension(FileName).TrimStart('.');
        }

        public Uri Source { get; }
        public Uri EffectiveSource { get; }
        public String FullPath { get; }
        public String FileName { get; }
        public String Extension { get; }
    }
}
