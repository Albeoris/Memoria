using System.ComponentModel;
using Memoria.Launcher.Utils.Archives;
using Xunit;

namespace Memoria.Launcher.Tests.Archives;

public sealed class ArchiveFailureMessageTests
{
    [Fact]
    public void Cleanup_failure_reports_that_the_mod_was_installed_and_preserves_the_exception_chain()
    {
        IOException cause = new("The file is locked.");

        String message = ArchiveFailureMessageBuilder.BuildCleanupFailureForMod(
            "Freya Game+",
            @"C:\Game\MemoriaInstallTmp\Downloads\Freya.rar",
            cause);

        Assert.Contains("was installed", message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Freya Game+", message, StringComparison.Ordinal);
        Assert.Contains(@"C:\Game\MemoriaInstallTmp\Downloads\Freya.rar", message, StringComparison.Ordinal);
        Assert.Contains("IOException: The file is locked.", message, StringComparison.Ordinal);
    }

    [Fact]
    public void SevenZipFailure_preserves_context_and_all_captured_process_output()
    {
        ArchiveExtractionException exception = SevenZipFailure.Create(
            SevenZipExitCode.FatalError,
            @"C:\Game\MemoriaInstallTmp\Downloads\Freya.rar",
            @"C:\Game\MemoriaInstallTmp\Extraction",
            "ERROR: Data Error\r\nERROR: CRC Failed",
            "Scanning the drive\r\n42% Broken file");

        Assert.Contains("Exit code: 2 (FatalError)", exception.Message, StringComparison.Ordinal);
        Assert.Contains(@"C:\Game\MemoriaInstallTmp\Downloads\Freya.rar", exception.Message, StringComparison.Ordinal);
        Assert.Contains(@"C:\Game\MemoriaInstallTmp\Extraction", exception.Message, StringComparison.Ordinal);
        Assert.Contains("ERROR: Data Error", exception.Message, StringComparison.Ordinal);
        Assert.Contains("ERROR: CRC Failed", exception.Message, StringComparison.Ordinal);
        Assert.Contains("Scanning the drive", exception.Message, StringComparison.Ordinal);
        Assert.Contains("42% Broken file", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Failure_message_contains_mod_archive_and_the_complete_exception_chain()
    {
        Win32Exception cause = new Win32Exception(5, "Access denied by Windows");
        ArchiveExtractionException failure = new ArchiveExtractionException("Could not start 7-Zip", cause);

        String message = ArchiveFailureMessageBuilder.BuildForMod("Freya Game+", @"C:\Game\Freya.rar", failure);

        Assert.Contains("Mod: Freya Game+", message, StringComparison.Ordinal);
        Assert.Contains(@"Archive: C:\Game\Freya.rar", message, StringComparison.Ordinal);
        Assert.Contains("ArchiveExtractionException: Could not start 7-Zip", message, StringComparison.Ordinal);
        Assert.Contains("Win32Exception: Access denied by Windows", message, StringComparison.Ordinal);
        Assert.Contains("Native error code: 5", message, StringComparison.Ordinal);
    }
}
