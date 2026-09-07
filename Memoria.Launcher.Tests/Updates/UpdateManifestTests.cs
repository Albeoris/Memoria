using Memoria.Launcher.Utils.Updates;
using Xunit;

namespace Memoria.Launcher.Tests.Updates;

public sealed class UpdateManifestTests
{
    [Fact]
    public void ValidManifestIsParsed()
    {
        const String json = "{\"SchemaVersion\":1,\"BuildTimeUtc\":\"2026-09-07T14:26:06.0000000Z\",\"PatcherSize\":91547908}";

        UpdateManifest manifest = UpdateManifest.Parse(json);

        Assert.Equal(new DateTime(2026, 9, 7, 14, 26, 6, DateTimeKind.Utc), manifest.BuildTime);
        Assert.Equal(91547908, manifest.PatcherSize);
    }

    [Theory]
    [InlineData("")]
    [InlineData("{}")]
    [InlineData("{\"SchemaVersion\":2,\"BuildTimeUtc\":\"2026-09-07T14:26:06Z\",\"PatcherSize\":1}")]
    [InlineData("{\"SchemaVersion\":1,\"BuildTimeUtc\":\"invalid\",\"PatcherSize\":1}")]
    [InlineData("{\"SchemaVersion\":1,\"BuildTimeUtc\":\"2026-09-07T14:26:06Z\",\"PatcherSize\":0}")]
    public void InvalidManifestIsRejected(String json)
    {
        Assert.ThrowsAny<Exception>(() => UpdateManifest.Parse(json));
    }
}
