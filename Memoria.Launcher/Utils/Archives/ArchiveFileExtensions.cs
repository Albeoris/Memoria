using System;
using System.Collections.Generic;
using System.IO;

namespace Memoria.Launcher.Utils.Archives
{
    internal static class ArchiveFileExtensions
    {
        private static readonly HashSet<String> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".7z",
            ".bzip2",
            ".gz",
            ".gzip",
            ".lzip",
            ".rar",
            ".tar",
            ".unrar",
            ".zip"
        };

        public static Boolean IsSupportedFile(String filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("The file path cannot be empty or whitespace.", nameof(filePath));

            return SupportedExtensions.Contains(Path.GetExtension(filePath));
        }
    }
}
