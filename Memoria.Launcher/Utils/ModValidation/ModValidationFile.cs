using System;
using System.IO;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModValidationFile
    {
        public ModValidationFile(String rootDirectory, String fullPath)
        {
            RootDirectory = Path.GetFullPath(rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory)));
            FullPath = Path.GetFullPath(fullPath ?? throw new ArgumentNullException(nameof(fullPath)));
            RelativePath = GetRelativePath(RootDirectory, FullPath);
        }

        public String RootDirectory { get; }
        public String FullPath { get; }
        public String RelativePath { get; }

        private static String GetRelativePath(String rootDirectory, String fullPath)
        {
            String normalizedRoot = rootDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            Uri rootUri = new(normalizedRoot, UriKind.Absolute);
            Uri fileUri = new(fullPath, UriKind.Absolute);
            return Uri.UnescapeDataString(rootUri.MakeRelativeUri(fileUri).ToString()).Replace('/', Path.DirectorySeparatorChar);
        }
    }
}