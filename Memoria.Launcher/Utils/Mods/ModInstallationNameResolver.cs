using System;
using System.IO;

namespace Memoria.Launcher.Utils.Mods
{
    internal static class ModInstallationNameResolver
    {
        public static String Resolve(String archivePath, ModArchiveRoot root)
        {
            if (String.IsNullOrWhiteSpace(archivePath))
                throw new ArgumentException("The archive path cannot be empty or whitespace.", nameof(archivePath));
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            String installationName = root.IsExtractionDirectory
                ? Path.GetFileNameWithoutExtension(archivePath)
                : Path.GetFileName(root.RelativePath);
            if (String.IsNullOrWhiteSpace(installationName))
                throw new InvalidDataException($"The archive '{archivePath}' does not provide a valid mod installation name.");

            return installationName;
        }
    }
}
