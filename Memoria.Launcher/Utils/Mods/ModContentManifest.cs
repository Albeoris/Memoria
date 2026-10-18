#nullable enable

using System;
using System.Collections.Generic;
using Memoria.Launcher.Utils.IO;

namespace Memoria.Launcher.Utils.Mods
{
    internal static class ModContentManifest
    {
        private const String StreamingAssetsPrefix = "StreamingAssets/";
        private const String GameDataPrefix = "FF9_Data/";

        private static readonly HashSet<String> RootFiles = new(StringComparer.OrdinalIgnoreCase)
        {
            "battlepatch.txt",
            "battlevoiceeffects.txt",
            "dictionarypatch.txt",
            "memoria.ini"
        };

        private static readonly HashSet<String> ArchiveBundleFiles = new(StringComparer.OrdinalIgnoreCase)
        {
            "p0data11.bin",
            "p0data12.bin",
            "p0data13.bin",
            "p0data14.bin",
            "p0data15.bin",
            "p0data16.bin",
            "p0data17.bin",
            "p0data18.bin",
            "p0data19.bin",
            "p0data2.bin",
            "p0data3.bin",
            "p0data4.bin",
            "p0data5.bin",
            "p0data61.bin",
            "p0data62.bin",
            "p0data63.bin",
            "p0data7.bin"
        };

        public static Boolean TryGetEntry(SafeRelativePath installedFilePath, out String entry)
        {
            if (installedFilePath == null)
                throw new ArgumentNullException(nameof(installedFilePath));

            String normalizedPath = installedFilePath.Value.Replace('\\', '/');
            String candidate = String.Empty;
            if (normalizedPath.StartsWith(StreamingAssetsPrefix, StringComparison.OrdinalIgnoreCase))
                candidate = normalizedPath.Substring(StreamingAssetsPrefix.Length);
            else if (normalizedPath.StartsWith(GameDataPrefix, StringComparison.OrdinalIgnoreCase))
                candidate = normalizedPath.Substring(GameDataPrefix.Length);
            else if (RootFiles.Contains(normalizedPath))
                candidate = normalizedPath;

            if (String.IsNullOrWhiteSpace(candidate) || ArchiveBundleFiles.Contains(candidate))
            {
                entry = String.Empty;
                return false;
            }

            entry = candidate.ToLowerInvariant();
            return true;
        }
    }
}
