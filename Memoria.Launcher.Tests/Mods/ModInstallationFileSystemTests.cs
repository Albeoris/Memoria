using Memoria.Launcher.Tests.Infrastructure;
using Memoria.Launcher.Utils.Mods;
using Xunit;

namespace Memoria.Launcher.Tests.Mods;

public sealed class ModInstallationFileSystemTests
{
    [Fact]
    public void DeleteInstallationDirectory_removes_nested_read_only_files()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        String installationDirectory = fileSystem.GetInstallationDirectory("MoguriFiles");
        String atlasDirectory = Path.Combine(installationDirectory, "StreamingAssets", "Assets");
        Directory.CreateDirectory(atlasDirectory);
        String readOnlyFile = Path.Combine(atlasDirectory, "10_0.png");
        File.WriteAllText(readOnlyFile, "texture");
        File.SetAttributes(readOnlyFile, File.GetAttributes(readOnlyFile) | FileAttributes.ReadOnly);

        fileSystem.DeleteInstallationDirectory(installationDirectory);

        Assert.False(Directory.Exists(installationDirectory));
    }

    [Fact]
    public void DeleteInstallationDirectory_rejects_a_directory_outside_the_game_root()
    {
        using TemporaryDirectory gameDirectory = new();
        using TemporaryDirectory unrelatedDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        String keepFile = Path.Combine(unrelatedDirectory.FullPath, "keep.txt");
        File.WriteAllText(keepFile, "keep");

        Assert.Throws<ArgumentException>(() =>
            fileSystem.DeleteInstallationDirectory(unrelatedDirectory.FullPath));

        Assert.True(File.Exists(keepFile));
    }

    [Fact]
    public void DeleteDownloadedFile_removes_only_the_requested_file_from_the_download_directory()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        fileSystem.PrepareDownloadDirectory();
        String archivePath = Path.Combine(fileSystem.DownloadDirectory, "Freya Game+ v0.5.rar");
        String otherArchivePath = Path.Combine(fileSystem.DownloadDirectory, "keep.zip");
        File.WriteAllText(archivePath, "rar");
        File.WriteAllText(otherArchivePath, "zip");

        fileSystem.DeleteDownloadedFile(archivePath);

        Assert.False(File.Exists(archivePath));
        Assert.True(File.Exists(otherArchivePath));
    }

    [Fact]
    public void DeleteDownloadedFile_rejects_a_file_outside_the_download_directory()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        String unrelatedFile = Path.Combine(gameDirectory.FullPath, "keep.rar");
        File.WriteAllText(unrelatedFile, "rar");

        Assert.Throws<ArgumentException>(() => fileSystem.DeleteDownloadedFile(unrelatedFile));

        Assert.True(File.Exists(unrelatedFile));
    }

    [Fact]
    public void ReplaceInstallationDirectory_rejects_a_source_outside_the_extraction_directory()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        String unrelatedDirectory = Path.Combine(gameDirectory.FullPath, "Unrelated");
        Directory.CreateDirectory(unrelatedDirectory);
        File.WriteAllText(Path.Combine(unrelatedDirectory, "keep.txt"), "keep");
        String destination = fileSystem.GetInstallationDirectory("Mods/Installed");

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            fileSystem.ReplaceInstallationDirectory(unrelatedDirectory, destination));

        Assert.Contains("outside the extraction directory", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.True(File.Exists(Path.Combine(unrelatedDirectory, "keep.txt")));
        Assert.False(Directory.Exists(destination));
    }

    [Fact]
    public void ReplaceInstallationDirectory_rejects_a_parent_transition_in_the_source_path()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        fileSystem.ResetExtractionDirectory();
        String unrelatedDirectory = Path.Combine(fileSystem.ExtractionDirectory, "..", "Unrelated");
        Directory.CreateDirectory(Path.GetFullPath(unrelatedDirectory));
        String destination = fileSystem.GetInstallationDirectory("Mods/Installed");

        Assert.Throws<ArgumentException>(() =>
            fileSystem.ReplaceInstallationDirectory(unrelatedDirectory, destination));
    }

    [Fact]
    public void ResetExtractionDirectory_removes_only_the_reserved_extraction_tree()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        Directory.CreateDirectory(fileSystem.ExtractionDirectory);
        File.WriteAllText(Path.Combine(fileSystem.ExtractionDirectory, "stale.txt"), "stale");
        String unrelatedFile = Path.Combine(gameDirectory.FullPath, "keep.txt");
        File.WriteAllText(unrelatedFile, "keep");

        fileSystem.ResetExtractionDirectory();

        Assert.True(Directory.Exists(fileSystem.ExtractionDirectory));
        Assert.Empty(Directory.EnumerateFileSystemEntries(fileSystem.ExtractionDirectory));
        Assert.Equal("keep", File.ReadAllText(unrelatedFile));
    }

    [Fact]
    public void ReplaceInstallationDirectory_creates_a_safe_nested_parent_and_moves_the_tree()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        fileSystem.ResetExtractionDirectory();
        String extractedMod = fileSystem.GetExtractedModDirectory(ModArchiveRoot.FromRelativePath("Wrapper"));
        Directory.CreateDirectory(extractedMod);
        File.WriteAllText(Path.Combine(extractedMod, "ModDescription.xml"), "content");
        String destination = fileSystem.GetInstallationDirectory("Mods/Freya");

        fileSystem.ReplaceInstallationDirectory(extractedMod, destination);

        Assert.False(Directory.Exists(extractedMod));
        Assert.Equal("content", File.ReadAllText(Path.Combine(destination, "ModDescription.xml")));
    }

    [Fact]
    public void ReplaceInstallationDirectory_restores_the_previous_mod_when_the_new_source_is_missing()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        fileSystem.ResetExtractionDirectory();
        String missingExtractedMod = fileSystem.GetExtractedModDirectory(ModArchiveRoot.FromRelativePath("Missing"));
        String destination = fileSystem.GetInstallationDirectory("Mods/Freya");
        Directory.CreateDirectory(destination);
        String existingFile = Path.Combine(destination, "existing.txt");
        File.WriteAllText(existingFile, "keep");

        Assert.Throws<DirectoryNotFoundException>(() =>
            fileSystem.ReplaceInstallationDirectory(missingExtractedMod, destination));

        Assert.Equal("keep", File.ReadAllText(existingFile));
    }
}
