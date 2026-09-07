using Memoria.Launcher.Utils.Updates;
using Xunit;

namespace Memoria.Launcher.Tests.Updates;

public sealed class InstalledBuildVersionTests
{
    private static readonly DateTime VersionUtc = new DateTime(2026, 9, 7, 14, 26, 6, DateTimeKind.Utc);

    [Fact]
    public void SerializedVersionMatchesTheSameBuild()
    {
        String storedVersion = InstalledBuildVersion.Serialize(VersionUtc);

        Assert.True(InstalledBuildVersion.Matches(storedVersion, VersionUtc));
    }

    [Fact]
    public void DifferentBuildDoesNotMatch()
    {
        String storedVersion = InstalledBuildVersion.Serialize(VersionUtc);

        Assert.False(InstalledBuildVersion.Matches(storedVersion, VersionUtc.AddSeconds(2)));
    }

    [Fact]
    public void LegacyDateDoesNotMatchAnExactBuild()
    {
        Assert.False(InstalledBuildVersion.Matches("2026.09.07", VersionUtc));
    }
}
