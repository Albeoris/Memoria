using Memoria.Launcher.Tests.Infrastructure;
using Memoria.Launcher.Utils.Downloads;
using Memoria.Launcher.Utils.Mods;
using Xunit;

namespace Memoria.Launcher.Tests.Mods;

public sealed class ModDownloadPlanTests
{
    private static readonly Uri Source = new("https://mods.example/download");

    [Fact]
    public void Validate_accepts_the_actual_rar_file_when_DownloadFormat_is_not_specified()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        ModDownloadPlan plan = new(fileSystem, "Freya Game+", "FreyaGamePlus", null);
        DownloadedFile downloadedFile = CreateDownloadedFile(plan, "Freya Game+ v0.5.rar");

        Exception? exception = Record.Exception(() => plan.Validate(downloadedFile));

        Assert.Null(exception);
        Assert.Equal("rar", downloadedFile.Extension);
    }

    [Fact]
    public void Validate_rejects_a_file_that_disagrees_with_an_explicit_DownloadFormat()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        ModDownloadPlan plan = new(fileSystem, "Archive mod", "ArchiveMod", "zip");
        DownloadedFile downloadedFile = CreateDownloadedFile(plan, "archive.rar");

        InvalidDataException exception = Assert.Throws<InvalidDataException>(() => plan.Validate(downloadedFile));

        Assert.Contains("does not match", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("../OutsideMod", null)]
    [InlineData("MemoriaInstallTmp/Replacement", null)]
    [InlineData("SafeMod", "SingleFileWithPath:../../outside.png")]
    [InlineData("SafeMod", "SingleFileWithPath:C:\\outside.png")]
    public void Constructor_rejects_unsafe_catalog_paths(String installationPath, String? downloadFormat)
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);

        Assert.Throws<ArgumentException>(() =>
            new ModDownloadPlan(fileSystem, "Unsafe mod", installationPath, downloadFormat));
    }

    [Fact]
    public void Single_file_installer_moves_the_real_file_and_writes_a_normalized_manifest()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        ModDownloadPlan plan = new(
            fileSystem,
            "Portrait mod",
            "PortraitMod",
            "SingleFileWithPath:StreamingAssets/Textures/Portrait.PNG");
        Directory.CreateDirectory(plan.DownloadDirectory);
        String sourcePath = Path.Combine(plan.DownloadDirectory, "remote-name.png");
        File.WriteAllBytes(sourcePath, [1, 2, 3]);
        DownloadedFile downloadedFile = new(Source, Source, sourcePath);
        SingleFileModInstaller installer = new(fileSystem);

        String installedPath = installer.Install(downloadedFile, plan, "ModFileList.txt");

        Assert.Equal(plan.GetSingleFileDestination(), installedPath);
        Assert.Equal(new Byte[] { 1, 2, 3 }, File.ReadAllBytes(installedPath));
        String manifestPath = Path.Combine(plan.InstallationDirectory, "ModFileList.txt");
        Assert.Equal("textures/portrait.png" + Environment.NewLine, File.ReadAllText(manifestPath));
        Assert.False(File.Exists(sourcePath));
    }

    [Fact]
    public void Single_file_installer_does_not_require_the_remote_file_name_to_have_an_extension()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        ModDownloadPlan plan = new(
            fileSystem,
            "Portrait mod",
            "PortraitMod",
            "SingleFileWithPath:StreamingAssets/Textures/Portrait.png");
        Directory.CreateDirectory(plan.DownloadDirectory);
        String sourcePath = Path.Combine(plan.DownloadDirectory, "download");
        File.WriteAllBytes(sourcePath, [4, 5, 6]);
        DownloadedFile downloadedFile = new(Source, Source, sourcePath);

        String installedPath = new SingleFileModInstaller(fileSystem)
            .Install(downloadedFile, plan, "ModFileList.txt");

        Assert.EndsWith("Portrait.png", installedPath, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(new Byte[] { 4, 5, 6 }, File.ReadAllBytes(installedPath));
    }

    [Fact]
    public void Validate_rejects_a_downloaded_file_from_elsewhere_in_the_game_directory()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        ModDownloadPlan plan = new(fileSystem, "Archive mod", "ArchiveMod", null);
        String unrelatedPath = Path.Combine(gameDirectory.FullPath, "unrelated.rar");
        File.WriteAllBytes(unrelatedPath, [1]);
        DownloadedFile downloadedFile = new(Source, Source, unrelatedPath);

        Assert.Throws<ArgumentException>(() => plan.Validate(downloadedFile));
    }

    private static DownloadedFile CreateDownloadedFile(ModDownloadPlan plan, String fileName)
    {
        Directory.CreateDirectory(plan.DownloadDirectory);
        String path = Path.Combine(plan.DownloadDirectory, fileName);
        File.WriteAllBytes(path, [1]);
        return new DownloadedFile(Source, Source, path);
    }
}
