#nullable enable

using System;
using System.IO;
using Memoria.Launcher.Utils.Archives;
using Memoria.Launcher.Utils.Downloads;
using Memoria.Launcher.Utils.IO;

namespace Memoria.Launcher.Utils.Mods
{
    internal sealed class ModDownloadPlan
    {
        private readonly ModInstallationFileSystem _fileSystem;

        public ModDownloadPlan(
            ModInstallationFileSystem fileSystem,
            String modName,
            String? installationPath,
            String? downloadFormat)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            if (String.IsNullOrWhiteSpace(modName))
                throw new ArgumentException("The mod name cannot be empty or whitespace.", nameof(modName));

            Format = ModDownloadFormat.Parse(downloadFormat);
            InstallationPath = String.IsNullOrWhiteSpace(installationPath) ? modName : installationPath!.Trim();
            InstallationDirectory = fileSystem.GetInstallationDirectory(InstallationPath);
        }

        public ModDownloadFormat Format { get; }
        public String InstallationPath { get; }
        public String InstallationDirectory { get; }
        public String DownloadDirectory => _fileSystem.DownloadDirectory;

        public void PrepareDownloadDirectory() => _fileSystem.PrepareDownloadDirectory();

        public String GetSingleFileDestination()
        {
            if (!(Format is SingleFileModDownloadFormat singleFileFormat))
                throw new InvalidOperationException("Only a single-file download has a single-file destination.");

            return _fileSystem.GetFileInInstallation(InstallationDirectory, singleFileFormat.FilePath);
        }

        public SafeRelativePath GetSingleFilePath()
        {
            if (!(Format is SingleFileModDownloadFormat singleFileFormat))
                throw new InvalidOperationException("Only a single-file download has a single-file path.");

            return singleFileFormat.FilePath;
        }

        public void Validate(DownloadedFile downloadedFile)
        {
            if (downloadedFile == null)
                throw new ArgumentNullException(nameof(downloadedFile));

            _fileSystem.EnsureDownloadedFile(downloadedFile.FullPath);
            Format.ValidateExtension(downloadedFile.Extension);
            if (Format.PackageType == ModPackageType.Archive && !ArchiveFileExtensions.IsSupportedFile(downloadedFile.FullPath))
            {
                throw new InvalidDataException(
                    $"The downloaded file '{downloadedFile.FileName}' is not a supported archive. " +
                    "Supported formats are 7z, bzip2, gz, gzip, lzip, rar, tar, unrar and zip.");
            }
        }
    }
}
