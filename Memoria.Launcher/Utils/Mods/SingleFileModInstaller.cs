#nullable enable

using System;
using System.IO;
using Memoria.Launcher.Utils.Downloads;

namespace Memoria.Launcher.Utils.Mods
{
    internal sealed class SingleFileModInstaller
    {
        private readonly ModInstallationFileSystem _fileSystem;

        public SingleFileModInstaller(ModInstallationFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public String Install(DownloadedFile downloadedFile, ModDownloadPlan plan, String manifestFileName)
        {
            if (downloadedFile == null)
                throw new ArgumentNullException(nameof(downloadedFile));
            if (plan == null)
                throw new ArgumentNullException(nameof(plan));

            plan.Validate(downloadedFile);
            String destinationPath = plan.GetSingleFileDestination();
            String destinationDirectory = Path.GetDirectoryName(destinationPath)
                ?? throw new InvalidDataException("The single-file destination does not have a parent directory.");
            Directory.CreateDirectory(destinationDirectory);
            File.Move(downloadedFile.FullPath, destinationPath);

            if (ModContentManifest.TryGetEntry(plan.GetSingleFilePath(), out String manifestEntry))
            {
                String manifestPath = _fileSystem.GetContentManifestFile(plan.InstallationDirectory, manifestFileName);
                File.WriteAllText(manifestPath, manifestEntry + Environment.NewLine);
            }

            return destinationPath;
        }
    }
}
