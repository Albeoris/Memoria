using Memoria.Launcher.Utils.Updates;
using Xunit;

namespace Memoria.Launcher.Tests.Updates;

public sealed class GitHubReleaseDescriptionParserTests
{
    [Fact]
    public void ParsesHeadingsBulletsLinksAndParagraphs()
    {
        const String markdown = "![banner](https://example.com/banner.png)\n\n[Complete changelog](https://example.com/changelog)\n\n## Features\n\n- Added `feature` with **formatting**\n\nRegular [linked text](https://example.com).";

        IReadOnlyList<ReleaseNotesBlock> blocks = GitHubReleaseDescriptionParser.Parse(markdown);

        Assert.Collection(blocks, block => { Assert.Equal(ReleaseNotesBlockKind.Link, block.Kind); Assert.Equal("Complete changelog", block.Text); Assert.Equal("https://example.com/changelog", block.Link.AbsoluteUri); }, block => { Assert.Equal(ReleaseNotesBlockKind.Heading, block.Kind); Assert.Equal("Features", block.Text); }, block => { Assert.Equal(ReleaseNotesBlockKind.Bullet, block.Kind); Assert.Equal("Added feature with formatting", block.Text); }, block => { Assert.Equal(ReleaseNotesBlockKind.Paragraph, block.Kind); Assert.Equal("Regular linked text.", block.Text); });
    }
}
