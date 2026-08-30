using System;

namespace Memoria.Launcher.Utils.Mods
{
    internal sealed class ExtractedModArchive
    {
        public ExtractedModArchive(Mod mod, String directoryPath, Boolean hasDescriptionFile)
        {
            Mod = mod ?? throw new ArgumentNullException(nameof(mod));
            if (String.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("The extracted mod directory cannot be empty or whitespace.", nameof(directoryPath));

            DirectoryPath = directoryPath;
            HasDescriptionFile = hasDescriptionFile;
        }

        public Mod Mod { get; }
        public String DirectoryPath { get; }
        public Boolean HasDescriptionFile { get; }
    }
}
