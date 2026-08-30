using System.IO.Compression;
using Memoria.Launcher.Tests.Infrastructure;
using Memoria.Launcher.Utils.Archives;
using Xunit;

namespace Memoria.Launcher.Tests.Archives;

public sealed class SharpCompressRarExtractorTests
{
    [Fact]
    public void Extract_rejects_a_different_archive_format_renamed_to_rar()
    {
        using TemporaryDirectory gameDirectory = new();
        String archivePath = Path.Combine(gameDirectory.FullPath, "renamed.rar");
        using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
            archive.CreateEntry("ModDescription.xml");
        String destinationPath = Path.Combine(gameDirectory.FullPath, "Extracted");
        SharpCompressRarExtractor extractor = new(gameDirectory.FullPath);

        ArchiveExtractionException exception = Assert.Throws<ArchiveExtractionException>(() =>
            extractor.Extract(archivePath, destinationPath, CancellationToken.None));

        Assert.Contains("not a valid RAR", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(Directory.EnumerateFileSystemEntries(destinationPath));
    }
}
