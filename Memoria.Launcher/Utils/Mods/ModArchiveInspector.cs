#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memoria.Launcher.Utils.Archives;
using Memoria.Launcher.Utils.IO;
using SharpCompress.Archives;

namespace Memoria.Launcher.Utils.Mods
{
    internal sealed class ModArchiveInspector
    {
        private static readonly HashSet<String> ModMarkers = new(StringComparer.OrdinalIgnoreCase)
        {
            "DictionaryPatch.txt",
            "FF9_Data",
            "Memoria.ini",
            "ModDescription.xml",
            "StreamingAssets"
        };

        public ModArchiveRoot FindModRoot(String archivePath, IEnumerable<String> knownRootNames) =>
            FindModRootCore(archivePath, Array.Empty<SafeRelativePath>(), knownRootNames);

        public ModArchiveRoot FindModRoot(
            String archivePath,
            String preferredRootName,
            IEnumerable<String> knownRootNames)
        {
            SafeRelativePath preferredRoot = SafeRelativePath.Parse(preferredRootName, nameof(preferredRootName));
            return FindModRootCore(archivePath, new[] { preferredRoot }, knownRootNames);
        }

        private static ModArchiveRoot FindModRootCore(
            String archivePath,
            IReadOnlyCollection<SafeRelativePath> preferredRoots,
            IEnumerable<String> knownRootNames)
        {
            if (String.IsNullOrWhiteSpace(archivePath))
                throw new ArgumentException("The archive path cannot be empty or whitespace.", nameof(archivePath));
            if (preferredRoots == null)
                throw new ArgumentNullException(nameof(preferredRoots));
            if (knownRootNames == null)
                throw new ArgumentNullException(nameof(knownRootNames));

            String[] knownRoots = knownRootNames
                .Select(value => SafeRelativePath.Parse(value, nameof(knownRootNames)).Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            using IArchive archive = ArchiveFactory.OpenArchive(archivePath);
            foreach (IArchiveEntry entry in archive.Entries)
            {
                String? key = entry.Key;
                if (String.IsNullOrWhiteSpace(key))
                    throw new ArchiveExtractionException("The archive contains an entry without a path.");

                SafeRelativePath entryPath = ArchiveEntryPathPolicy.Validate(key!, entry.LinkTarget);
                String directory = Path.GetDirectoryName(entryPath.Value) ?? String.Empty;

                foreach (SafeRelativePath preferredRoot in preferredRoots)
                {
                    if (IsInRoot(entryPath.Value, preferredRoot.Value))
                        return ModArchiveRoot.FromRelativePath(preferredRoot.Value);
                }
                if (knownRoots.Contains(directory, StringComparer.OrdinalIgnoreCase))
                    return ModArchiveRoot.FromRelativePath(directory);

                String name = Path.GetFileName(entryPath.Value);
                if (ModMarkers.Contains(name) && IsRootOrDirectChild(directory))
                {
                    return directory.Length == 0
                        ? ModArchiveRoot.ExtractionDirectory
                        : ModArchiveRoot.FromRelativePath(directory);
                }
            }

            throw new InvalidDataException("The archive does not contain a recognizable Memoria mod structure.");
        }

        private static Boolean IsInRoot(String entryPath, String rootPath) =>
            entryPath.Equals(rootPath, StringComparison.OrdinalIgnoreCase) ||
            entryPath.StartsWith(rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

        private static Boolean IsRootOrDirectChild(String directory) =>
            directory.Length == 0 || String.IsNullOrEmpty(Path.GetDirectoryName(directory));
    }
}
