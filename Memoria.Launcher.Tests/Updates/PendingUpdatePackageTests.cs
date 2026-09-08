using Memoria.Launcher.Tests.Infrastructure;
using Memoria.Launcher.Utils.Updates;
using Xunit;

namespace Memoria.Launcher.Tests.Updates;

public sealed class PendingUpdatePackageTests
{
    [Fact]
    public void Cancelled_package_does_not_replace_the_existing_patcher()
    {
        using TemporaryDirectory directory = new TemporaryDirectory();
        String pendingPath = Path.Combine(directory.FullPath, "update.pending");
        String targetPath = Path.Combine(directory.FullPath, "Memoria.Patcher.exe");
        File.WriteAllText(pendingPath, "new");
        File.WriteAllText(targetPath, "old");
        using CancellationTokenSource cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        using (PendingUpdatePackage package = new PendingUpdatePackage(pendingPath, targetPath))
            Assert.Throws<OperationCanceledException>(() => package.Commit(cancellation.Token));

        Assert.Equal("old", File.ReadAllText(targetPath));
        Assert.False(File.Exists(pendingPath));
    }

    [Fact]
    public void Completed_package_atomically_replaces_the_existing_patcher()
    {
        using TemporaryDirectory directory = new TemporaryDirectory();
        String pendingPath = Path.Combine(directory.FullPath, "update.pending");
        String targetPath = Path.Combine(directory.FullPath, "Memoria.Patcher.exe");
        File.WriteAllText(pendingPath, "new");
        File.WriteAllText(targetPath, "old");

        using (PendingUpdatePackage package = new PendingUpdatePackage(pendingPath, targetPath))
            package.Commit(CancellationToken.None);

        Assert.Equal("new", File.ReadAllText(targetPath));
        Assert.False(File.Exists(pendingPath));
    }
}
