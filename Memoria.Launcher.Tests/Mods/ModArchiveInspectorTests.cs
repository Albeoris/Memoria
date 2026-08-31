using System.IO.Compression;
using Memoria.Launcher.Tests.Infrastructure;
using Memoria.Launcher.Utils.Archives;
using Memoria.Launcher.Utils.Mods;
using Xunit;

namespace Memoria.Launcher.Tests.Mods;

public sealed class ModArchiveInspectorTests
{
    [Fact]
    public void FindModRoot_detects_a_single_wrapping_directory_from_a_mod_marker()
    {
        using TemporaryDirectory temporaryDirectory = new();
        String archivePath = Path.Combine(temporaryDirectory.FullPath, "downloaded-file.rar");
        using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
            archive.CreateEntry("FreyaGamePlus/ModDescription.xml");
        ModArchiveInspector inspector = new();

        ModArchiveRoot root = inspector.FindModRoot(archivePath, knownRootNames: []);

        Assert.Equal("FreyaGamePlus", root.RelativePath);
    }

    [Fact]
    public void FindModRoot_rejects_an_unsafe_entry_before_it_can_be_extracted()
    {
        using TemporaryDirectory temporaryDirectory = new();
        String archivePath = Path.Combine(temporaryDirectory.FullPath, "malicious.zip");
        using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
            archive.CreateEntry("../outside.txt");
        ModArchiveInspector inspector = new();

        Assert.Throws<ArchiveExtractionException>(() =>
            inspector.FindModRoot(archivePath, knownRootNames: []));
    }

    [Fact]
    public void FindModRoot_throws_when_the_archive_has_no_recognizable_mod_root()
    {
        using TemporaryDirectory temporaryDirectory = new TemporaryDirectory();
        String archivePath = Path.Combine(temporaryDirectory.FullPath, "not-a-mod.zip");
        using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
            archive.CreateEntry("Documents/readme.txt");
        ModArchiveInspector inspector = new ModArchiveInspector();

        InvalidDataException exception = Assert.Throws<InvalidDataException>(() =>
            inspector.FindModRoot(archivePath, knownRootNames: []));

        Assert.Contains("recognizable", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FindModRoot_does_not_infer_structure_from_archive_name()
    {
        using TemporaryDirectory temporaryDirectory = new();
        String archivePath = Path.Combine(temporaryDirectory.FullPath, "Freya Game release.zip");
        using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
            archive.CreateEntry("Documents/readme.txt");
        ModArchiveInspector inspector = new();

        Assert.Throws<InvalidDataException>(() =>
            inspector.FindModRoot(archivePath, knownRootNames: ["Freya Game"]));
    }
}
