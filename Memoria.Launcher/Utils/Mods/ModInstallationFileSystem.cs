#nullable enable

using System;
using System.IO;
using Memoria.Launcher.Utils.IO;

namespace Memoria.Launcher.Utils.Mods
{
    internal sealed class ModInstallationFileSystem
    {
        private const String TemporaryDirectoryName = "MemoriaInstallTmp";

        private readonly GameRootDirectory _gameRoot;

        public ModInstallationFileSystem(String gameRootPath)
        {
            _gameRoot = new GameRootDirectory(gameRootPath);
            TemporaryDirectory = Resolve(TemporaryDirectoryName, nameof(TemporaryDirectoryName));
            DownloadDirectory = Resolve(Path.Combine(TemporaryDirectoryName, "Downloads"), nameof(DownloadDirectory));
            ExtractionDirectory = Resolve(Path.Combine(TemporaryDirectoryName, "Extraction"), nameof(ExtractionDirectory));
        }

        public String GameRootPath => _gameRoot.RootPath;
        public String TemporaryDirectory { get; }
        public String DownloadDirectory { get; }
        public String ExtractionDirectory { get; }

        public String GetInstallationDirectory(String installationPath) =>
            ResolveInstallationDirectory(installationPath, nameof(installationPath));

        public String GetExtractedModDirectory(ModArchiveRoot archiveRoot)
        {
            if (archiveRoot == null)
                throw new ArgumentNullException(nameof(archiveRoot));
            if (archiveRoot.IsExtractionDirectory)
                return ExtractionDirectory;

            return _gameRoot.ResolveWithin(
                ExtractionDirectory,
                SafeRelativePath.Parse(archiveRoot.RelativePath, nameof(archiveRoot)));
        }

        public String GetFileInInstallation(String installationDirectory, SafeRelativePath relativeFilePath) =>
            _gameRoot.ResolveWithin(installationDirectory, relativeFilePath);

        public String GetDescriptionFile(String installationDirectory, String descriptionFileName) =>
            _gameRoot.ResolveWithin(
                installationDirectory,
                SafeRelativePath.Parse(descriptionFileName, nameof(descriptionFileName)));

        public String GetContentManifestFile(String installationDirectory, String manifestFileName) =>
            _gameRoot.ResolveWithin(
                installationDirectory,
                SafeRelativePath.Parse(manifestFileName, nameof(manifestFileName)));

        public void PrepareDownloadDirectory()
        {
            CreateSafeDirectory(DownloadDirectory);
        }

        public void ResetExtractionDirectory()
        {
            DeleteDirectoryIfExists(ExtractionDirectory);
            Directory.CreateDirectory(ExtractionDirectory);
            _gameRoot.EnsureContained(ExtractionDirectory);
        }

        public void DeleteExtractionDirectory() => DeleteDirectoryIfExists(ExtractionDirectory);

        public void DeleteTemporaryDirectory() => DeleteDirectoryIfExists(TemporaryDirectory);

        public void DeleteDownloadedFile(String path)
        {
            String fullPath = GetDownloadFilePath(path);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public void DeleteInstallationDirectory(String installationDirectory)
        {
            EnsureInstallationDirectory(installationDirectory);
            DeleteDirectoryIfExists(installationDirectory);
        }

        public void ReplaceInstallationDirectory(String extractedModDirectory, String installationDirectory)
        {
            String fullExtractedModDirectory = ValidateExtractedModDirectory(extractedModDirectory);
            EnsureInstallationDirectory(installationDirectory);
            String? installationParent = Path.GetDirectoryName(installationDirectory);
            if (String.IsNullOrWhiteSpace(installationParent))
                throw new InvalidDataException("The installation directory does not have a parent directory.");

            CreateSafeDirectory(installationParent!, allowRoot: true);
            if (!Directory.Exists(installationDirectory))
            {
                Directory.Move(fullExtractedModDirectory, installationDirectory);
                return;
            }

            CreateSafeDirectory(TemporaryDirectory);
            String backupDirectory = _gameRoot.ResolveWithin(
                TemporaryDirectory,
                SafeRelativePath.Parse($"PreviousInstallation-{Guid.NewGuid():N}", "backupDirectory"));
            Directory.Move(installationDirectory, backupDirectory);
            try
            {
                Directory.Move(fullExtractedModDirectory, installationDirectory);
            }
            catch (Exception installationException)
            {
                try
                {
                    Directory.Move(backupDirectory, installationDirectory);
                }
                catch (Exception rollbackException)
                {
                    throw new AggregateException(
                        "The new mod could not be installed and the previous installation could not be restored.",
                        installationException,
                        rollbackException);
                }

                throw;
            }

            DeleteDirectoryIfExists(backupDirectory);
        }

        public void EnsureDownloadedFile(String path)
        {
            String fullPath = GetDownloadFilePath(path);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("The downloaded mod file does not exist.", fullPath);
        }

        private void EnsureInstallationDirectory(String installationDirectory)
        {
            _gameRoot.EnsureContained(installationDirectory);
            String temporaryPrefix = TemporaryDirectory + Path.DirectorySeparatorChar;
            if (installationDirectory.Equals(TemporaryDirectory, StringComparison.OrdinalIgnoreCase) ||
                installationDirectory.StartsWith(temporaryPrefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"A mod cannot be installed into the reserved temporary directory '{TemporaryDirectory}'.",
                    nameof(installationDirectory));
            }
        }

        private void DeleteDirectoryIfExists(String directoryPath)
        {
            _gameRoot.EnsureContained(directoryPath);
            DirectoryTreeDeleter.Delete(directoryPath);
        }

        private String ValidateExtractedModDirectory(String extractedModDirectory)
        {
            if (String.IsNullOrWhiteSpace(extractedModDirectory))
                throw new ArgumentException("The extracted mod directory cannot be empty or whitespace.", nameof(extractedModDirectory));

            _gameRoot.EnsureContained(extractedModDirectory);
            String fullExtractedModDirectory = Path.GetFullPath(extractedModDirectory);
            String extractionPrefix = ExtractionDirectory + Path.DirectorySeparatorChar;
            if (!fullExtractedModDirectory.Equals(ExtractionDirectory, StringComparison.OrdinalIgnoreCase) &&
                !fullExtractedModDirectory.StartsWith(extractionPrefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The mod source is outside the extraction directory.", nameof(extractedModDirectory));
            }

            return fullExtractedModDirectory;
        }

        private void CreateSafeDirectory(String directoryPath, Boolean allowRoot = false)
        {
            _gameRoot.EnsureContained(directoryPath, allowRoot);
            Directory.CreateDirectory(directoryPath);
            _gameRoot.EnsureContained(directoryPath, allowRoot);
        }

        private String ResolveInstallationDirectory(String relativePath, String parameterName)
        {
            String resolvedPath = Resolve(relativePath, parameterName);
            EnsureInstallationDirectory(resolvedPath);
            return resolvedPath;
        }

        private String Resolve(String relativePath, String parameterName) =>
            _gameRoot.Resolve(SafeRelativePath.Parse(relativePath, parameterName));

        private String GetDownloadFilePath(String path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("The downloaded file path cannot be empty or whitespace.", nameof(path));

            _gameRoot.EnsureContained(path);
            String fullPath = Path.GetFullPath(path);
            String? parentDirectory = Path.GetDirectoryName(fullPath);
            if (!String.Equals(parentDirectory, DownloadDirectory, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The downloaded file is outside the mod download directory.", nameof(path));

            return fullPath;
        }
    }
}
