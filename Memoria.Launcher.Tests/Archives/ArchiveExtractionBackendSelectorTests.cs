using Memoria.Launcher.Utils.Archives;
using Xunit;

namespace Memoria.Launcher.Tests.Archives;

public sealed class ArchiveExtractionBackendSelectorTests
{
    [Theory]
    [InlineData("Freya Game+ v0.5.rar")]
    [InlineData("C:\\Games\\FINAL FANTASY IX\\MemoriaInstallTmp\\MOD.RAR")]
    public void Select_uses_SharpCompress_for_rar_archives(String archivePath)
    {
        ArchiveExtractionBackend result = ArchiveExtractionBackendSelector.Select(archivePath);

        Assert.Equal(ArchiveExtractionBackend.SharpCompress, result);
    }

    [Theory]
    [InlineData("mod.zip")]
    [InlineData("mod.7z")]
    [InlineData("mod.unrar")]
    public void Select_keeps_other_formats_on_SevenZip(String archivePath)
    {
        ArchiveExtractionBackend result = ArchiveExtractionBackendSelector.Select(archivePath);

        Assert.Equal(ArchiveExtractionBackend.SevenZip, result);
    }
}
