using Memoria.Launcher.Tests.Infrastructure;
using Xunit;

namespace Memoria.Launcher.Tests.Launcher;

public sealed class SteamLauncherBootstrapTests
{
    [Fact]
    public void RestartIfNecessary_SkipsNonSteamInstallation()
    {
        using TemporaryDirectory directory = new TemporaryDirectory();
        Boolean invoked = false;

        Boolean restarted = SteamLauncherBootstrap.RestartIfNecessary(directory.FullPath, (_, _) =>
        {
            invoked = true;
            return true;
        });

        Assert.False(restarted);
        Assert.False(invoked);
    }

    [Fact]
    public void RestartIfNecessary_RemovesGeneratedAppIdFileWhenSteamRestartsLauncher()
    {
        using TemporaryDirectory directory = CreateSteamInstallation();
        String appIdPath = Path.Combine(directory.FullPath, "steam_appid.txt");
        File.WriteAllText(appIdPath, SteamLauncherBootstrap.ApplicationId.ToString());

        Boolean restarted = SteamLauncherBootstrap.RestartIfNecessary(directory.FullPath, (_, appId) =>
        {
            Assert.Equal(SteamLauncherBootstrap.ApplicationId, appId);
            Assert.False(File.Exists(appIdPath));
            return true;
        });

        Assert.True(restarted);
        Assert.False(File.Exists(appIdPath));
    }

    [Fact]
    public void RestartIfNecessary_RestoresAppIdFileWhenAlreadyLaunchedThroughSteam()
    {
        using TemporaryDirectory directory = CreateSteamInstallation();
        String appIdPath = Path.Combine(directory.FullPath, "steam_appid.txt");
        Byte[] originalContents = { 0xEF, 0xBB, 0xBF, 0x33, 0x37, 0x37, 0x38, 0x34, 0x30, 0x0D, 0x0A };
        File.WriteAllBytes(appIdPath, originalContents);

        Boolean restarted = SteamLauncherBootstrap.RestartIfNecessary(directory.FullPath, (_, _) => false);

        Assert.False(restarted);
        Assert.Equal(originalContents, File.ReadAllBytes(appIdPath));
    }

    [Fact]
    public void RestartIfNecessary_RestoresAppIdFileWhenSteamApiFails()
    {
        using TemporaryDirectory directory = CreateSteamInstallation();
        String appIdPath = Path.Combine(directory.FullPath, "steam_appid.txt");
        File.WriteAllText(appIdPath, SteamLauncherBootstrap.ApplicationId.ToString());

        Assert.Throws<InvalidOperationException>(() => SteamLauncherBootstrap.RestartIfNecessary(directory.FullPath, (_, _) => throw new InvalidOperationException("Steam API failure")));
        Assert.True(File.Exists(appIdPath));
    }

    [Fact]
    public void RestartIfNecessary_PreservesUnexpectedAppIdFile()
    {
        using TemporaryDirectory directory = CreateSteamInstallation();
        String appIdPath = Path.Combine(directory.FullPath, "steam_appid.txt");
        File.WriteAllText(appIdPath, "123");
        Boolean invoked = false;

        Boolean restarted = SteamLauncherBootstrap.RestartIfNecessary(directory.FullPath, (_, _) =>
        {
            invoked = true;
            return true;
        });

        Assert.False(restarted);
        Assert.False(invoked);
        Assert.Equal("123", File.ReadAllText(appIdPath));
    }

    private static TemporaryDirectory CreateSteamInstallation()
    {
        TemporaryDirectory directory = new TemporaryDirectory();
        String x64Directory = Path.Combine(directory.FullPath, "x64");
        Directory.CreateDirectory(x64Directory);
        File.WriteAllBytes(Path.Combine(x64Directory, "steam_api64.dll"), Array.Empty<Byte>());
        return directory;
    }
}
