using Memoria.Launcher.Utils.Mods;
using Xunit;

namespace Memoria.Launcher.Tests.Mods;

public sealed class ModInstallationNameResolverTests
{
    [Fact]
    public void Resolve_uses_archive_name_when_mod_files_are_at_archive_root()
    {
        String result = ModInstallationNameResolver.Resolve(
            Path.Combine("downloads", "Freya Game+ v0.5.rar"),
            ModArchiveRoot.ExtractionDirectory);

        Assert.Equal("Freya Game+ v0.5", result);
    }

    [Fact]
    public void Resolve_uses_detected_mod_directory_instead_of_archive_name()
    {
        String result = ModInstallationNameResolver.Resolve(
            Path.Combine("downloads", "release.zip"),
            ModArchiveRoot.FromRelativePath(Path.Combine("package", "MoguriFiles")));

        Assert.Equal("MoguriFiles", result);
    }
}
