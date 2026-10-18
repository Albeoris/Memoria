using Memoria.Launcher.Tests.Infrastructure;
using Memoria.Launcher.Utils.Mods;
using Xunit;

namespace Memoria.Launcher.Tests.Mods;

public sealed class ModDescriptionFileTests
{
    [Fact]
    public void EnsureExists_creates_a_missing_catalog_description_in_the_installed_mod_directory()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        ModDescriptionFile descriptionFile = new(fileSystem);
        String installationDirectory = fileSystem.GetInstallationDirectory("FreyaGamePlus");
        Directory.CreateDirectory(installationDirectory);
        String? writerDirectory = null;

        descriptionFile.EnsureExists(
            installationDirectory,
            "ModDescription.xml",
            directory =>
            {
                writerDirectory = directory;
                File.WriteAllText(Path.Combine(directory, "ModDescription.xml"), "<Mod />");
            });

        Assert.Equal(installationDirectory, writerDirectory);
        Assert.True(File.Exists(Path.Combine(installationDirectory, "ModDescription.xml")));
    }

    [Fact]
    public void EnsureExists_preserves_a_description_supplied_by_the_archive()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        ModDescriptionFile descriptionFile = new(fileSystem);
        String installationDirectory = fileSystem.GetInstallationDirectory("ExistingMod");
        Directory.CreateDirectory(installationDirectory);
        String descriptionPath = Path.Combine(installationDirectory, "ModDescription.xml");
        File.WriteAllText(descriptionPath, "archive metadata");

        descriptionFile.EnsureExists(
            installationDirectory,
            "ModDescription.xml",
            _ => throw new InvalidOperationException("The existing description must not be replaced."));

        Assert.Equal("archive metadata", File.ReadAllText(descriptionPath));
    }

    [Fact]
    public void EnsureExists_rejects_a_writer_that_does_not_create_the_required_file()
    {
        using TemporaryDirectory gameDirectory = new();
        ModInstallationFileSystem fileSystem = new(gameDirectory.FullPath);
        ModDescriptionFile descriptionFile = new(fileSystem);
        String installationDirectory = fileSystem.GetInstallationDirectory("BrokenMod");
        Directory.CreateDirectory(installationDirectory);

        InvalidDataException exception = Assert.Throws<InvalidDataException>(() =>
            descriptionFile.EnsureExists(installationDirectory, "ModDescription.xml", _ => { }));

        Assert.Contains("could not be created", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
