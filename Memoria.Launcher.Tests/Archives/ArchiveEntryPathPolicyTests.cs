using Memoria.Launcher.Utils.Archives;
using Xunit;

namespace Memoria.Launcher.Tests.Archives;

public sealed class ArchiveEntryPathPolicyTests
{
    [Theory]
    [InlineData("../outside.txt")]
    [InlineData("safe/../../outside.txt")]
    [InlineData("C:\\outside.txt")]
    [InlineData("/absolute/outside.txt")]
    public void Validate_rejects_archive_entries_that_can_escape_the_extraction_directory(String entryPath)
    {
        ArchiveExtractionException exception = Assert.Throws<ArchiveExtractionException>(() =>
            ArchiveEntryPathPolicy.Validate(entryPath));

        Assert.Contains(entryPath, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_rejects_links_even_when_the_entry_path_is_safe()
    {
        ArchiveExtractionException exception = Assert.Throws<ArchiveExtractionException>(() =>
            ArchiveEntryPathPolicy.Validate("StreamingAssets/data.bin", "../../outside.bin"));

        Assert.Contains("links are not allowed", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
