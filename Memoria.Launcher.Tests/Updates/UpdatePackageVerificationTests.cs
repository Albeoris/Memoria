using Memoria.Launcher.Utils.Downloads;
using Memoria.Launcher.Utils.Updates;
using Memoria.Launcher.Tests.Infrastructure;
using System.Security.Cryptography;
using Xunit;

namespace Memoria.Launcher.Tests.Updates;

public sealed class UpdatePackageVerificationTests
{
    [Fact]
    public void GitHubDigestIsSelectedByAssetNameAndNormalized()
    {
        const String json = "{\"assets\":[{\"name\":\"other.exe\",\"digest\":\"sha256:0000000000000000000000000000000000000000000000000000000000000000\"},{\"name\":\"Memoria.Patcher.exe\",\"digest\":\"sha256:ABCDEFABCDEFABCDEFABCDEFABCDEFABCDEFABCDEFABCDEFABCDEFABCDEFABCD\"}]}";

        String digest = GitHubReleaseAssetDigest.Parse(json, "Memoria.Patcher.exe");

        Assert.Equal("abcdefabcdefabcdefabcdefabcdefabcdefabcdefabcdefabcdefabcdefabcd", digest);
    }

    [Theory]
    [InlineData("{\"assets\":[]}")]
    [InlineData("{\"assets\":[{\"name\":\"Memoria.Patcher.exe\"}]}")]
    [InlineData("{\"assets\":[{\"name\":\"Memoria.Patcher.exe\",\"digest\":\"sha256:not-a-hash\"}]}")]
    public void MissingOrInvalidGitHubDigestIsRejected(String json)
    {
        Assert.Throws<FormatException>(() => GitHubReleaseAssetDigest.Parse(json, "Memoria.Patcher.exe"));
    }

    [Fact]
    public void DownloadedPackageMustMatchTheExpectedDigest()
    {
        using TemporaryDirectory directory = new TemporaryDirectory();
        String path = Path.Combine(directory.FullPath, "Memoria.Patcher.exe");
        File.WriteAllText(path, "trusted installer bytes");
        String digest = ComputeSha256(path);

        UpdatePackageVerifier.VerifySha256(path, digest);

        Assert.Throws<DownloadException>(() => UpdatePackageVerifier.VerifySha256(path, new String('0', 64)));
    }

    private static String ComputeSha256(String path)
    {
        using SHA256 sha256 = SHA256.Create();
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(sha256.ComputeHash(stream));
    }
}