namespace Memoria.Launcher.Tests.Infrastructure;

internal sealed class TemporaryDirectory : IDisposable
{
    public TemporaryDirectory()
    {
        FullPath = Path.Combine(Path.GetTempPath(), "Memoria.Launcher.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(FullPath);
    }

    public String FullPath { get; }

    public void Dispose()
    {
        if (Directory.Exists(FullPath))
            Directory.Delete(FullPath, recursive: true);
    }
}
