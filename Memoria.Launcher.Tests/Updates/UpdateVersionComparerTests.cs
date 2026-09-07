using Memoria.Launcher.Utils.Updates;
using Xunit;

namespace Memoria.Launcher.Tests.Updates;

public sealed class UpdateVersionComparerTests
{
    private static readonly DateTime CurrentVersion = new DateTime(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void OlderTargetIsDowngrade()
    {
        UpdateVersionRelation result = UpdateVersionComparer.Compare(CurrentVersion, CurrentVersion.AddMinutes(-1));

        Assert.Equal(UpdateVersionRelation.Downgrade, result);
    }

    [Fact]
    public void NewerTargetIsUpgrade()
    {
        UpdateVersionRelation result = UpdateVersionComparer.Compare(CurrentVersion, CurrentVersion.AddMinutes(1));

        Assert.Equal(UpdateVersionRelation.Upgrade, result);
    }

    [Fact]
    public void SameTargetIsReinstall()
    {
        UpdateVersionRelation result = UpdateVersionComparer.Compare(CurrentVersion, CurrentVersion);

        Assert.Equal(UpdateVersionRelation.Reinstall, result);
    }

    [Fact]
    public void UnspecifiedBuildTimestampIsTreatedAsUtc()
    {
        DateTime unspecified = DateTime.SpecifyKind(CurrentVersion, DateTimeKind.Unspecified);

        UpdateVersionRelation result = UpdateVersionComparer.Compare(unspecified, CurrentVersion);

        Assert.Equal(UpdateVersionRelation.Reinstall, result);
    }
}
