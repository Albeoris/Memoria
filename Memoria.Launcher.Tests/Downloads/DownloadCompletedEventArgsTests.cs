using Memoria.Launcher.Utils.Downloads;
using Xunit;

namespace Memoria.Launcher.Tests.Downloads;

public sealed class DownloadCompletedEventArgsTests
{
    [Fact]
    public void Successful_and_failed_results_expose_only_their_valid_payload()
    {
        Uri source = new Uri("https://example.test/file.rar");
        DownloadedFile file = new DownloadedFile(source, source, Path.Combine(Path.GetTempPath(), "file.rar"));
        InvalidDataException error = new InvalidDataException("failure");

        DownloadCompletedEventArgs success = DownloadCompletedEventArgs.Completed(file);
        DownloadCompletedEventArgs failure = DownloadCompletedEventArgs.Failed(error);

        Assert.Same(file, success.GetDownloadedFile());
        Assert.Throws<InvalidOperationException>(() => success.GetError());
        Assert.Same(error, failure.GetError());
        Assert.Throws<InvalidOperationException>(() => failure.GetDownloadedFile());
    }
}
