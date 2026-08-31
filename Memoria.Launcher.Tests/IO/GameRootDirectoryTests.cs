using Memoria.Launcher.Tests.Infrastructure;
using Memoria.Launcher.Utils.IO;
using Xunit;

namespace Memoria.Launcher.Tests.IO;

public sealed class GameRootDirectoryTests
{
    [Fact]
    public void Constructor_preserves_a_file_system_root()
    {
        String fileSystemRoot = Path.GetPathRoot(Path.GetTempPath())!;
        String childName = $"memoria-path-test-{Guid.NewGuid():N}";

        GameRootDirectory gameRoot = new(fileSystemRoot);
        String childPath = gameRoot.Resolve(SafeRelativePath.Parse(childName, nameof(childName)));

        Assert.Equal(fileSystemRoot, gameRoot.RootPath);
        Assert.Equal(Path.Combine(fileSystemRoot, childName), childPath);
    }

    [Theory]
    [InlineData("../outside")]
    [InlineData("mods/../../outside")]
    [InlineData("mods\\..\\outside")]
    [InlineData("C:\\outside")]
    [InlineData("\\\\server\\share\\outside")]
    public void Resolve_rejects_paths_that_can_leave_the_game_directory(String relativePath)
    {
        using TemporaryDirectory gameDirectory = new();
        GameRootDirectory gameRoot = new(gameDirectory.FullPath);

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            gameRoot.Resolve(SafeRelativePath.Parse(relativePath, nameof(relativePath))));

        Assert.Contains("path", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ResolveWithin_keeps_a_nested_mixed_separator_path_inside_its_base_directory()
    {
        using TemporaryDirectory gameDirectory = new();
        GameRootDirectory gameRoot = new(gameDirectory.FullPath);
        String modsDirectory = gameRoot.Resolve(SafeRelativePath.Parse("Mods", "path"));

        String result = gameRoot.ResolveWithin(
            modsDirectory,
            SafeRelativePath.Parse("Freya/StreamingAssets\\Data.bin", "path"));

        Assert.StartsWith(modsDirectory + Path.DirectorySeparatorChar, result, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(
            Path.Combine(modsDirectory, "Freya", "StreamingAssets", "Data.bin"),
            result);
    }

    [Fact]
    public void EnsureContained_rejects_relative_transitions_even_when_the_normalized_result_is_inside_the_game()
    {
        using TemporaryDirectory gameDirectory = new();
        GameRootDirectory gameRoot = new(gameDirectory.FullPath);
        String deceptivePath = Path.Combine(gameDirectory.FullPath, "Mods", "..", "OtherMod");

        Assert.Throws<ArgumentException>(() => gameRoot.EnsureContained(deceptivePath));
    }
}
