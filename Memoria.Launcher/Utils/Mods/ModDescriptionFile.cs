#nullable enable

using System;
using System.IO;

namespace Memoria.Launcher.Utils.Mods
{
    internal sealed class ModDescriptionFile
    {
        private readonly ModInstallationFileSystem _fileSystem;

        public ModDescriptionFile(ModInstallationFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public void EnsureExists(
            String installationDirectory,
            String descriptionFileName,
            Action<String> createDescription)
        {
            if (createDescription == null)
                throw new ArgumentNullException(nameof(createDescription));

            String descriptionPath = _fileSystem.GetDescriptionFile(installationDirectory, descriptionFileName);
            if (File.Exists(descriptionPath))
                return;

            createDescription(installationDirectory);
            if (!File.Exists(descriptionPath))
            {
                throw new InvalidDataException(
                    $"The installed mod does not contain '{descriptionFileName}', and its catalog description could not be created.");
            }
        }
    }
}
