#nullable enable

using System;
using System.IO;
using Memoria.Launcher.Utils.IO;

namespace Memoria.Launcher.Utils.Archives
{
    internal static class ArchiveEntryPathPolicy
    {
        public static SafeRelativePath Validate(String entryPath, String? linkTarget = null)
        {
            if (!String.IsNullOrWhiteSpace(linkTarget))
                throw new ArchiveExtractionException($"Archive links are not allowed. Entry '{entryPath}' points to '{linkTarget}'.");

            try
            {
                return SafeRelativePath.Parse(entryPath, nameof(entryPath));
            }
            catch (ArgumentException exception)
            {
                throw new ArchiveExtractionException(
                    $"The archive contains an unsafe path '{entryPath}'. Absolute paths and relative transitions are not allowed.",
                    exception);
            }
        }
    }
}
